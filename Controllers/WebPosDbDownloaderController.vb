Imports System.Net
Imports System.Security.AccessControl
Imports System.Security.Principal
Imports Newtonsoft.Json
Imports WebProj.WebProj

' <System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)> is to allow concurrent [ajax] request for this MVC controller. https://stackoverflow.com/a/7034657/334045
<System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)>
Public Class WebPosDbDownloaderController
    Inherits System.Web.Mvc.AsyncController

    Private Shared mMessages As New Messages

    Function Index() As String

        Return DateTimeStamp()

    End Function

    Public Async Function TestAsync() As Threading.Tasks.Task

        AsyncManager.OutstandingOperations.Increment()
        Await Threading.Tasks.Task.Delay(1000)
        AsyncManager.Parameters("vResult") = DateTimeStamp()
        AsyncManager.OutstandingOperations.Decrement()

    End Function

    Public Function TestCompleted(vResult As String) As String

        Return vResult

    End Function

    <AsyncTimeout(Integer.MaxValue)>
    Public Function RequestDataAsync() As String

        AsyncManager.OutstandingOperations.Increment()
        RequestDataTask()

    End Function

    Public Function RequestDataCompleted(vResult As String) As String

        Return vResult

    End Function

    Public Async Function RequestDataTask() As Threading.Tasks.Task

        Dim pError As String

        Try
            Dim pOrgID = WebRequest.Instance.OrgID
            Dim pUuid = Common.WebRequest.Data("UUID")
            Dim pPeopleID = Common.WebRequest.Data("PeopleID")
            Dim pRemoteServer = GetRemoteServer()

            If String.IsNullOrWhiteSpace(pRemoteServer) Then
                pError = "WebPosDbDownloaderController.GetRemoteServer is null. Set the remote server first."
                mMessages.Add(pError)
                AsyncManager.Parameters("vResult") = JsonDictionary.Serialize("Error", pError)
                Return
            End If

            Using pProData = ProData.CreateFromConnectionInfo(
                vServer:=Common.ConnectionInfo.Server,
                vDatabase:="",
                vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity,
                vUsername:=Common.ConnectionInfo.Username,
                vPassword:=Common.ConnectionInfo.Password)

                If WebPosFlush.HasAttachedUnflushed(pProData) Then
                    pError = "Local machine has attached unflushed database. Please wait while we flush data to the server. ref: vb5VQHNDS3TQ!"
                    mMessages.Add(pError)
                    AsyncManager.Parameters("vResult") = JsonDictionary.Serialize("Result", "for follow up")
                    WebPosFlush.StartAsync()
                    Return
                End If
            End Using

            If Await ResumeSplitDownload(pOrgID) Then
                AsyncManager.Parameters("vResult") = JsonDictionary.Serialize("Result", "ok")
                Return
            End If

            Dim pJson = New JsonDictionary()
            pJson.Add("OrgID", pOrgID)
            pJson.Add("DeviceUUID", pUuid)
            pJson.Add("ForWebPos", "1")
            pJson.Add("PeopleID", pPeopleID)

            Dim pWeb = New TrailNetSuite.PatientWebClient()
            Dim pUrl = AppendIfNeeded(pRemoteServer, "/") & "MobizExport/RequestDataFromServer"
            Dim pLoop As Integer

            Do
                pLoop += 1
                If pLoop = 1 Then
                    mMessages.Add("Requesting data from server")
                Else
                    mMessages.Add("Following up data from server. Follow up #" & pLoop)
                End If
                Dim pResult = pWeb.UploadString(pUrl, pJson.ToString())
                pUrl = AppendIfNeeded(pRemoteServer, "/") & "MobizExport/RequestFollowUp"
                Try
                    pError = JsonConvert.DeserializeObject(pResult)("d")("Error")
                    If String.IsNullOrWhiteSpace(pError) = False Then
                        mMessages.Add(pError)
                        AsyncManager.Parameters("vResult") = JsonDictionary.Serialize("Error", pError)
                        Return
                    End If
                    Dim pKey = JsonConvert.DeserializeObject(pResult)("d")("Results")
                    If String.IsNullOrWhiteSpace(pKey) Then
                        pKey = JsonConvert.DeserializeObject(pResult)("d")("Result")
                    End If
                    If String.IsNullOrWhiteSpace(pKey) = False Then
                        If pKey = "Requesting Data from Server" _
                            OrElse pKey = "Creating Database" Then

                            Await Threading.Tasks.Task.Delay(10000)
                            Continue Do

                        ElseIf pKey.ToString().Length = 75 Then
                            GetSplitFileInfo(pKey, pOrgID)
                            Await ResumeSplitDownload(pOrgID)
                            AsyncManager.Parameters("vResult") = JsonDictionary.Serialize("Result", "done")
                            Return
                        ElseIf pKey = "Error" Then
                            AsyncManager.Parameters("vResult") = JsonDictionary.Serialize("Result", "unauthorized device")
                            Return
                        Else
                            AsyncManager.Parameters("vResult") = JsonDictionary.Serialize("Error", "" & pKey)
                            Return
                        End If
                        AsyncManager.Parameters("vResult") = JsonDictionary.Serialize("Result", "for follow up")
                        Return
                    End If
                    AsyncManager.Parameters("vResult") = JsonDictionary.Serialize("Error", "vbKEQZ63REXF! Unexpected result: " & pResult)
                    Return
                Catch ex As Exception
                    AsyncManager.Parameters("vResult") = JsonDictionary.Serialize("Error", "vbSUSVW5EH9J! Unable to parse result: " & pResult & ". Exception: " & ex.Message)
                    Return
                End Try
            Loop
        Catch ex As Exception
            AsyncManager.Parameters("vResult") = JsonDictionary.Serialize("Error", "vbRTLGFCXB75! " & ex.Message)
        Finally
            AsyncManager.OutstandingOperations.Decrement()
        End Try

    End Function

    Private Async Function FollowUp() As Threading.Tasks.Task(Of String)

    End Function

    Private Async Function ResumeSplitDownload(vOrgID As String) As Threading.Tasks.Task(Of Boolean)

        Dim pFolder = IO.Path.Combine(Common.ServerPath, "temp\WebPos\SplitDownload\" & ReadyGuid(vOrgID)) & "\"
        If IO.Directory.Exists(pFolder) = False Then
            mMessages.Add("Not resuming from download list file.")
            Return False
        End If
        Dim pFileText = FileText(pFolder & "SplitFileInfo.txt")
        If String.IsNullOrWhiteSpace(pFileText) Then
            mMessages.Add("Not resuming from download list file.")
            Return False
        End If
        mMessages.Add("Resuming from download list file: " & pFolder & "SplitFileInfo.txt")

        Try
            Dim pSplitFileInfo = JsonConvert.DeserializeObject(Of MobizExportController.SplitFileJson)(pFileText)
            If pSplitFileInfo.Count <= 0 Then
                Return False
            End If
            If pSplitFileInfo.Crcs.Count <> pSplitFileInfo.Count Then
                Return False
            End If

            Return Await DownloadSplits(pSplitFileInfo, vOrgID)
        Catch ex As Exception
            mMessages.Add("vbFPDJS8KW2H! " & ex.Message)
            AppendToDebug("vbFPDJS8KW2H! " & ex.Message, True, ex, True)
            Throw
        End Try

    End Function

    Private Async Function DownloadSplits(vSplitFile As MobizExportController.SplitFileJson, vOrgID As String) As Threading.Tasks.Task(Of Boolean)

        Dim pIndex As Integer
        Dim pFilenames As New List(Of String)
        Dim pFilename As String
        Dim pError As String
        Dim pActualCrc As String
        Dim pLastProgressTime As Date
        Dim pTimeoutTarget As Date

        For Each pCrc In vSplitFile.Crcs
            Dim pLastPercent As Integer = 0
            pIndex += 1
            pFilename = IO.Path.Combine(Common.ServerPath, "temp\WebPos\SplitDownload\" & ReadyGuid(vOrgID)) & "\Splitted.zip." & Format(pIndex, "000")
            If IO.File.Exists(pFilename) Then
                Continue For
            End If
            Dim pUrl = AppendIfNeeded(GetRemoteServer(), "/") & "MobizExport/DownloadSplit" &
                "?FolderTitle=" & vSplitFile.FolderTitle &
                "&Index=" & pIndex &
                "&Count=" & vSplitFile.Count
            Dim pWeb = New TrailNetSuite.PatientWebClient()
            mMessages.Add("Downloading " & pUrl)

            AddHandler pWeb.DownloadProgressChanged,
                Sub(sender As Object, e As System.Net.DownloadProgressChangedEventArgs)
                    pTimeoutTarget = Now.AddMinutes(1)
                    If (Math.Abs((Now - pLastProgressTime).TotalSeconds) >= 1 _
                        OrElse e.ProgressPercentage = 100) _
                        AndAlso pLastPercent <> e.ProgressPercentage Then

                        mMessages.Add(e.ProgressPercentage & "%")
                        pLastProgressTime = Now
                        pLastPercent = e.ProgressPercentage
                    End If
                End Sub

            pFilenames.Add(pFilename)
            mMessages.Add("Downloading " & pIndex & " of " & vSplitFile.Count)
            Do
                Try
                    pTimeoutTarget = Now.AddMinutes(1)
                    pWeb.CancelAsync()
                    Dim pDownloadTask = pWeb.DownloadFileTaskAsync(pUrl, pFilename & ".temp")
                    Dim pTimeoutTask = Threading.Tasks.Task.Run(
                        Sub()
                            Try
                                Do
                                    If pTimeoutTarget < Now Then
                                        Exit Do
                                    End If
                                    Threading.Thread.Sleep(5000)
                                Loop
                            Catch ex As Exception
                                mMessages.Add(ex.Message)
                            End Try
                        End Sub)
                    Dim pCompletedTask = Await Threading.Tasks.Task.WhenAny(pDownloadTask, pTimeoutTask)
                    If pCompletedTask Is pDownloadTask Then
                        ' download complete
                        mMessages.Add("Download segment complete. ref: vbWMBD5YEV3P!")
                        Exit Do
                    Else
                        ' timeout
                        mMessages.Add("Download timed out. ref: vbHCR5FU2KEH!")
                        pWeb.CancelAsync()
                    End If
                Catch ex As Exception
                    mMessages.Add(ex.Message & ". ref: vbUPT5AHFSDD!")
                End Try
                Await Threading.Tasks.Task.Delay(10000)
            Loop
            pActualCrc = ProPack.Security.Crypto.GetCRC32FromFile(pFilename & ".temp")
            If pCrc <> pActualCrc Then
                pError = "vbJV9PADX88T! Expected CRC: " & pCrc & "; actual CRC: " & pActualCrc & "; Filename: " & pFilename & ".temp; URL: " & pUrl
                Throw New ApplicationException(pError)
            End If
            MoveFile(pFilename & ".temp", pFilename)
        Next

        mMessages.Add("Merging downloaded segmented files")
        pFilename = IO.Path.Combine(Common.ServerPath, "temp\WebPos\SplitDownload\" & ReadyGuid(vOrgID)) & "\Merged.zip"
        If MergeFiles(pFilenames, pFilename, pError, True) = False Then
            mMessages.Add("An error occured while merging downloaded segmented files: " & pError)
            Return False
        End If
        pActualCrc = ProPack.Security.Crypto.GetCRC32FromFile(pFilename)
        If vSplitFile.Crc <> pActualCrc Then
            Try
                Dim pFolderToDelete = IO.Path.Combine(Common.ServerPath, "temp\WebPos\SplitDownload\" & ReadyGuid(vOrgID)) & "\"
                IO.Directory.Delete(pFolderToDelete, True)
            Catch ex As Exception
                ' just ignore any errors here
                Debug.WriteLine(ex.Message)
            End Try
            pError = "vbK4NSZEFL86! Expected CRC: " & vSplitFile.Crc & "; actual CRC: " & pActualCrc & "; Filename: " & pFilename & "; FolderTitle=" & vSplitFile.FolderTitle
            Throw New ApplicationException(pError)
        End If

        mMessages.Add("Decompressing merged file")
        Dim pFolder = IO.Path.Combine(Common.ServerPath, "temp\WebPos\Uncompressed\" & ReadyGuid(vOrgID)) & "\" & DateTimeStamp(False)
        If IO.Directory.Exists(pFolder) = False Then
            IO.Directory.CreateDirectory(pFolder)
        End If
        Dim pZip = New ProZip(pFilename)
        pZip.Unzip(pFolder, pError)
        If String.IsNullOrWhiteSpace(pError) = False Then
            mMessages.Add(pError)
            Throw New ApplicationException("vbJQ8UYG3A92! " & pError)
        End If

        DetachAndArchiveCurrentDatabases(pFolder, vOrgID)

        Return True

    End Function

    Private Shared Sub DetachAndArchiveCurrentDatabases(vSourceFolder As String, vOrgID As String)

        mMessages.Add("Attaching database files")
        AddDirectoryUser(AppendBackslash(Common.ServerPath) & "WebPosDb")

        Dim pError As String
        Dim pMdfs As New List(Of String)
        Dim pFilenames = IO.Directory.GetFiles(vSourceFolder)
        For Each pFilename In pFilenames
            If pFilename.ToLower().EndsWith(".mdf") Then
                pMdfs.Add(pFilename)
            End If
        Next

        Using pProData = ProData.CreateFromConnectionInfo( _
            vServer:=Common.ConnectionInfo.Server, _
            vDatabase:="", _
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity, _
            vUsername:=Common.ConnectionInfo.Username, _
            vPassword:=Common.ConnectionInfo.Password)

            Dim pDatabases = pProData.GetDatabases(True, True)
            For Each pMdf In pMdfs
                Dim pFileTitle = IO.Path.GetFileNameWithoutExtension(pMdf)
                'If pFileTitle.Trim().ToLower() = "mobiz" Then
                '    Continue For
                'End If
                If pDatabases.Contains(pFileTitle.Trim().ToLower()) = False Then
                    Continue For
                End If

                Dim pCurrentMdfPath = pProData.GetFirstDataFilePath(pFileTitle.Trim())
                If pCurrentMdfPath.StartsWith(AppendBackslash(Common.ServerPath) & "WebPosDb") Then
                    WebPosFlush.ReattachAsUnflushed(pProData, pFileTitle)
                    'pProData.ExecuteSQL("USE " & EscapeSqlName(pFileTitle))
                    'If WebPosFlush.AddUnflushedData(vProData:=pProData, _
                    '    rZip:=Nothing, _
                    '    oRecordCount:=1,
                    '    vLimit:=1) > 0 Then

                    '    pError = "There are unflushed data. Please flush all data then synchronize database again. ref: vbLN4FEL4BBG!"
                    '    mMessages.Add(pError)
                    '    Throw New ApplicationException(pError)
                    'End If
                Else
                    pError = "vbPD84LU7VQ7! Cannot detach database, " & pFileTitle & ", because it's MDF is outside WebPosDb (" & pCurrentMdfPath & ")"
                    mMessages.Add(pError)
                    Throw New ApplicationException(pError)
                End If

                With New Synonyms()
                    .Prefix = "Mobiz"

                    Try
                        .SourceDatabase = WebPosFlush.UnflushedPrefix & "Mobiz"
                        .ProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
                            .Server = Common.ConnectionInfo.Server,
                            .Database = WebPosFlush.UnflushedPrefix & pFileTitle,
                            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
                            .Username = Common.ConnectionInfo.Username,
                            .Password = Common.ConnectionInfo.Password})
                        .Generate()
                    Catch ex As System.Exception
                        AppendToDebug(, True, ex, True)
                    End Try
                End With
            Next

            'For Each pMdf In pMdfs
            '    Dim pFileTitle = IO.Path.GetFileNameWithoutExtension(pMdf)
            '    If pDatabases.Contains(pFileTitle.Trim().ToLower()) = False Then
            '        Continue For
            '    End If

            '    Dim pCurrentMdfPath = pProData.GetFirstDataFilePath(pFileTitle.Trim())
            '    If pCurrentMdfPath.StartsWith(AppendBackslash(Common.ServerPath) & "WebPosDb") Then
            '        Dim pDtDbFiles = pProData.GetDatabaseFiles(pFileTitle.Trim())
            '        pProData.DetachDatabase(pFileTitle.Trim())
            '        For Each pDrDbFile As DataRow In pDtDbFiles.Rows
            '            IO.File.Delete(pDrDbFile("physical_name"))
            '        Next
            '    Else
            '        pError = "vbPD84LU7VQ7! Cannot detach database, " & pFileTitle & ", because it's MDF is outside WebPosDb (" & pCurrentMdfPath & ")"
            '        mMessages.Add(pError)
            '        Throw New ApplicationException(pError)
            '    End If
            'Next

            Dim pAttachables As New List(Of String)

            Try
                For Each pMdf In pMdfs
                    Dim pFileTitle = IO.Path.GetFileNameWithoutExtension(pMdf)
                    Dim pDestinationMdf As String = AppendBackslash(Common.ServerPath) & "WebPosDb\" & pFileTitle & ".mdf"
                    MoveFile(pMdf, pDestinationMdf)
                    pAttachables.Add(pDestinationMdf)

                    pMdf = IO.Path.Combine( _
                        IO.Path.GetDirectoryName(pMdf),
                        IO.Path.GetFileNameWithoutExtension(pMdf)) & ".ldf"
                    If IO.File.Exists(pMdf) Then
                        pDestinationMdf = AppendBackslash(Common.ServerPath) & "WebPosDb\" & pFileTitle & ".ldf"
                        MoveFile(pMdf, pDestinationMdf)
                    End If
                Next
            Catch ex As Exception
                mMessages.Add(ex.Message)
                Throw
            End Try

            ' archive *.ldf files
            'Try
            '    Dim pLdfFilenames = IO.Directory.GetFiles(AppendBackslash(Common.ServerPath) & "WebPosDb\", "*.ldf")
            '    For Each pLdfFilename In pLdfFilenames
            '        Try
            '            MoveFile(pLdfFilename, pLdfFilename & "." & DateTimeStamp())
            '        Catch ex As Exception
            '            mMessages.Add("An error occured while moving LDF: " & ex.Message)
            '        End Try
            '    Next
            'Catch ex As Exception
            '    mMessages.Add(ex.Message)
            'End Try

            Dim pAttachedPrivateDatabaseName As String
            AddDirectoryUser(AppendBackslash(Common.ServerPath) & "WebPosDb") ' do this again here
            Try
                For Each pAttachable In pAttachables
                    Dim pFileTitle = IO.Path.GetFileNameWithoutExtension(pAttachable)
                    If DbConnectController.IsPrivateDatabase(pFileTitle) Then
                        pAttachedPrivateDatabaseName = pFileTitle
                    End If
                    AddFileUser(pAttachable)
                    Dim pLdf = IO.Path.Combine( _
                        IO.Path.GetDirectoryName(pAttachable),
                        IO.Path.GetFileNameWithoutExtension(pAttachable)) & ".ldf"
                    If IO.File.Exists(pLdf) Then
                        AddFileUser(pLdf)
                        pProData.AttachDatabase(pFileTitle, pAttachable, pLdf)
                    Else
                        pProData.AttachDatabase(pFileTitle, pAttachable)
                    End If
                Next
            Catch ex As Exception
                mMessages.Add(ex.Message)
                Throw
            End Try

            Try
                Dim pFolder = IO.Path.Combine(Common.ServerPath, "temp\WebPos\SplitDownload\" & ReadyGuid(vOrgID)) & "\"
                IO.Directory.Delete(pFolder, True)
            Catch ex As Exception
                mMessages.Add(ex.Message)
                Throw
            End Try

            CreateCacheDatabaseIfNeeded(pProData, pAttachedPrivateDatabaseName)
        End Using
        mMessages.Add("Done attaching database files")

    End Sub

    Public Shared Sub CreateCacheDatabaseIfNeeded(ByRef rProData As ProData, vSourceDatabaseName As String)

        If String.IsNullOrWhiteSpace(vSourceDatabaseName) Then
            Return
        End If

        Dim pDatabases = rProData.GetDatabases()
        If (From pDatabase In pDatabases
            Where pDatabase.ToLower() = "MobizWebPOSCache".ToLower()).Any Then

            mMessages.Add("MobizWebPOSCache already exist. No need to create another cache database. ref: vbSLJX76MPW6!")
            Return
        End If

        If (From pDatabase In pDatabases
            Where pDatabase.ToLower() = "temp_MobizWebPOSCache".ToLower()).Any Then

            mMessages.Add("temp_MobizWebPOSCache already exist. Deleting.")
            rProData.DetachDatabase("temp_MobizWebPOSCache")
            mMessages.Add("temp_MobizWebPOSCache was successfully detached")
        End If

        Dim pFolder = Common.ServerPath & "temp\WebPos\CacheDatabase\" & DateTimeStamp(False) & "\"
        IO.Directory.CreateDirectory(pFolder)
        mMessages.Add("Creating temp_MobizWebPOSCache in " & pFolder)
        rProData.CreateDatabase("temp_MobizWebPOSCache", pFolder)
        Using pProData = ProData.CreateFromConnectionInfo(New ConnectionInfo() With { _
            .Server = Common.ConnectionInfo.Server,
            .Database = "temp_MobizWebPOSCache",
            .IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity,
            .Username = Common.ConnectionInfo.Username,
            .Password = Common.ConnectionInfo.Password})

            ' put the schema to the blank database
            mMessages.Add("Adding schema to temp_MobizWebPOSCache from " & vSourceDatabaseName)
            rProData.Execute("USE " & EscapeSqlName(vSourceDatabaseName))
            Dim pDatabase As New DBCompare.Database(rProData)
            pDatabase.LoadFromSql(True, False, False, True)
            pDatabase.SetCompareStatusToUnique()
            DBCompare.Common.SqlDelimiter = vbCrLf & vbCrLf & "-- delimeter " & GenerateString(20) & vbCrLf & vbCrLf
            Dim pSql = pDatabase.GetScript()
            For Each pLine In pSql.Split({DBCompare.Common.SqlDelimiter}, StringSplitOptions.RemoveEmptyEntries)
                Try
                    pProData.ExecuteSQL(pLine)
                Catch ex As Exception
                    AppendToLog("vb79S8Z9TWTU! cache database " & ex.Message & vbCrLf & pLine, , "WebPosDbDownloaderController")
                    Throw
                End Try
            Next

            With New Synonyms()
                .Prefix = "Mobiz"
                Try
                    .SourceDatabase = "Mobiz"
                    .ProData = pProData
                    .Generate()
                Catch ex As System.Exception
                    AppendToDebug(, True, ex, True)
                End Try
            End With
        End Using ' END pProData of blank database

        ' detach the created cache database
        mMessages.Add("Detaching temp_MobizWebPOSCache")
        Dim pDtFiles = rProData.GetDatabaseFiles("temp_MobizWebPOSCache")
        rProData.DetachDatabase("temp_MobizWebPOSCache")
        Dim pMdf As String
        For Each pDrFiles As DataRow In pDtFiles.Rows
            Dim pDestinationFilename = pDrFiles("physical_name")
            If CStr2(pDrFiles("type_desc")).Trim().ToLower() = "rows" Then
                pDestinationFilename = Common.ServerPath & "WebPosDb\MobizWebPOSCache.mdf"
                pMdf = pDestinationFilename
            Else
                'pDestinationFilename = Common.ServerPath & "WebPosDb\MobizWebPOSCache.ldf" & IO.Path.GetFileName(pDestinationFilename)
                IO.File.Delete(pDestinationFilename)
                Continue For
            End If
            MoveFile(pDrFiles("physical_name"), pDestinationFilename)
        Next
        mMessages.Add("Attaching MobizWebPOSCache")
        rProData.AttachDatabase("MobizWebPOSCache", pMdf)

    End Sub

    Public Shared Sub AddFileUser(vFile As String)

        ' https://www.c-sharpcorner.com/blogs/add-users-and-permission-to-directory-and-files-by-using-c-sharp1
        Dim pDI = New IO.FileInfo(vFile)
        If pDI.Exists() = False Then
            Return
        End If
        Dim pDS As FileSecurity
        pDS = pDI.GetAccessControl()
        pDS.AddAccessRule(New FileSystemAccessRule(New SecurityIdentifier(WellKnownSidType.WorldSid, Nothing), FileSystemRights.FullControl, AccessControlType.Allow))
        pDI.SetAccessControl(pDS)

    End Sub

    Private Function GetSplitFileInfo(vKey As String, vOrgID As String)

        mMessages.Add("Requesting segmented downloadable files from server")
        Dim pJson = New JsonDictionary()
        pJson.Add("fID", vKey)
        Dim pWeb = New TrailNetSuite.PatientWebClient()
        Dim pUrl = AppendIfNeeded(GetRemoteServer(), "/") & "MobizExport/SplitFile"
        Dim pResult = pWeb.UploadString(pUrl, pJson.ToString())
        Dim pFilename = IO.Path.Combine(Common.ServerPath, "temp/WebPos/SplitDownload/" & ReadyGuid(vOrgID)) & "/SplitFileInfo.txt"
        CreateFolder(pFilename)
        FileText(pFilename) = pResult
        mMessages.Add("Done requesting segmented downloadable files from server")

    End Function

    Public Shared Function GetRemoteServer() As String

        Dim pFilename = IO.Path.Combine(Common.ServerPath, "settings", "WebPosRemoteServer.txt")
        If IO.File.Exists(pFilename) = False Then
            Return Nothing
        End If
        Dim pFileText = FileText(pFilename)

        Return pFileText

    End Function

    Public Function GetRecentMessages() As String

        Dim pLastTimeStamp = Common.WebRequest.Data("LastTimeStamp")
        Dim pTimeStamp As Date? = ParseDate(pLastTimeStamp)
        If pTimeStamp Is Nothing Then
            pTimeStamp = Now
        End If
        Dim pRecent = mMessages.GetRecent(pTimeStamp.Value)

        Dim pOut = JsonConvert.SerializeObject(pRecent)
        Return pOut

    End Function

    Private Shared mWebPosSettingsKey As String = "UTC95DRS3V"
    Public Class WebPosSettings
        Public MachineType As String ' int
        Public ExternalServerIp As String
        Public DeviceID As String
        Public AfterSaving As String ' int

        ' derived fields
        Public UUID As String
    End Class

    Private Shared Function WebPosSettingsFilename() As String
        Static pOut As String
        If pOut Is Nothing Then
            pOut = IO.Path.Combine(Common.ServerPath, "settings", "WebPosSettings.txtx")
        End If
        Return pOut
    End Function

    Public Shared Function GetWebPosSettings(vBusiness As Business) As WebPosSettings

        Dim pOut As New WebPosSettings

        Try
            If IO.File.Exists(WebPosSettingsFilename()) Then
                Dim pBytes = GetFileBytes(WebPosSettingsFilename())
                pBytes = ProPack.Security.Crypto.Decrypt(pBytes, mWebPosSettingsKey)
                Dim pStr = New System.Text.UTF7Encoding().GetString(pBytes)
                pOut = JsonConvert.DeserializeObject(Of WebPosSettings)(pStr)
                If String.IsNullOrWhiteSpace(pOut.DeviceID) = False Then
                    Dim pDR = vBusiness.LibraryDevice.ById(pOut.DeviceID)
                    If pDR IsNot Nothing Then
                        pOut.UUID = CStr2(pDR("UUID"))
                    End If
                End If
            End If
            pOut.AfterSaving = vBusiness.SystemDefaults.WebPosAfterSaving
        Catch ex As Exception
            Debug.WriteLine("GetWebPosSettings: vbWUF3QXCZAE! " & ex.Message)
        End Try

        Return pOut

    End Function

    Public Function GetWebPosSettings() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pOut = GetWebPosSettings(WebRequest.Instance.Business)

        Return JsonConvert.SerializeObject(pOut)

    End Function

    Public Function SetWebPosSettings() As String

        Try
            Dim pInput = JsonConvert.DeserializeObject(Of WebPosSettings)(WebRequest.Instance.InputStreamText)
            If IsNumeric(pInput.MachineType) = False Then
                Return JsonDictionary.Serialize("Error", "MachineType must be numeric")
            End If
            Dim pStr = JsonConvert.SerializeObject(pInput)
            Dim pBytes = New System.Text.UTF7Encoding().GetBytes(pStr)
            pBytes = ProPack.Security.Crypto.Encrypt(pBytes, mWebPosSettingsKey)
            SetFileBytes(WebPosSettingsFilename(), pBytes)
            WebRequest.Instance.Business.SystemDefaults.WebPosAfterSaving = CInt2(pInput.AfterSaving)

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function
End Class
