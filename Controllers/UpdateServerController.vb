Imports System.Net
Imports System.Threading
Imports Newtonsoft.Json

Public Class UpdateServerController
    Inherits System.Web.Mvc.Controller

    Shared mSettingsFilename As String
    Shared mFileFilterList As String
    Private Const mIpVerificationToken As String = "Mobiz Update Server Verification Token vbRW2QCXRBAZ!"
    Dim mTimer As Timers.Timer
    Dim mLogPrefix As String

    Public Function Index() As ActionResult

        Return Content(DateTimeStamp() & " ; ref: vb8P5YRHRSSF!")

    End Function

    Public Function Testing() As String

        Return "Testing " & DateTimeStamp() & " ; ref: vbWX98FWW9SW!"

    End Function

    <HttpPost()>
    Public Function GetMobizDTRPortNo()

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim dtrPort As String

        dtrPort = WebRequest.Instance.Business.SystemDefaults.MobizDTRPort

        Return JsonDictionary.Serialize("MobizDTRPort", dtrPort)

    End Function

    <HttpPost()>
    Public Function GetUpdateServerIp()

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pUpdateServerIp = FileText(SettingsFilename)

        Return JsonDictionary.Serialize("UpdateServerIp", pUpdateServerIp)

    End Function

    <HttpPost()>
    Public Function SaveUpdateServerIp()

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Try
            With WebRequest.Instance.Business.SystemDefaults
                .MobizDTRPort = Common.WebRequest.Data("MobizDTRPortNo")
            End With

            Dim pUpdateServerIp = Common.WebRequest.Data("UpdateServerIp")
            If String.IsNullOrWhiteSpace(pUpdateServerIp) Then
                FileText(SettingsFilename) = ""
                Return JsonDictionary.Serialize("Result", "ok")
            End If
            Dim pNormalizedIp = NormalizeUrl(pUpdateServerIp) & "UpdateServer/IpVerificationToken"
            Dim pWeb = New WebClient()
            Dim pResult = pWeb.DownloadString(pNormalizedIp)
            If pResult = mIpVerificationToken Then
                FileText(SettingsFilename) = pUpdateServerIp
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", "The specified IP does not point to a valid Mobiz server.")
            End If
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Private ReadOnly Property SettingsFilename
        Get

            If String.IsNullOrEmpty(mSettingsFilename) Then
                mSettingsFilename = IO.Path.Combine(Common.ServerPath, "settings", "UpdateServerIp.txt")
            End If

            Return mSettingsFilename

        End Get
    End Property

    Private Shared ReadOnly mExecuteLock As New Object

    <HttpGet>
    Public Function Execute() As String

        SyncLock mExecuteLock
            Dim pZip As ProZip
            Dim pRemoteServer As String

            Try
                pRemoteServer = WebRequest.Instance.QueryString("server")
            Catch ex As Exception
                ' just ignore any errors here
            End Try

            Try
                mLogPrefix = DateTimeStamp()
                Dim pRemoteFileListUrl = RemoteFileListUrl(pRemoteServer)
                If String.IsNullOrWhiteSpace(pRemoteFileListUrl) Then
                    Return JsonDictionary.Serialize("Error", AddLog("Update server URL is empty. Set it at System Administration > Update Server."))
                End If
                Dim pWeb = New TrailNetSuite.PatientWebClient
                'Dim pWeb = New WebClient
                Dim pZipFilename = IO.Path.Combine(Common.ServerPath, "temp", "UpdateServer", "RemoteFileList", DateTimeStamp() & ".zip")
                CreateFolder(pZipFilename)
                AddLog("Downloading file list from " & pRemoteFileListUrl)
                pWeb.DownloadFile(pRemoteFileListUrl, pZipFilename)

                Try
                    pZip = New ProZip(pZipFilename)
                Catch ex As Exception
                    Dim pZipText = GetFileText(pZipFilename, 10000)
                    Return JsonDictionary.Serialize("Error", pZipText)
                End Try
                If pZip.Exist("Files.txt") = False Then
                    Return JsonDictionary.Serialize("Error", AddLog("Zip file does not contain Files.txt"))
                End If

                Dim pFileJson = pZip.GetString("Files.txt")
                Dim pFileJsons = JsonConvert.DeserializeObject(Of FileJsons)(pFileJson)
                AddLog("Comparing local files and file list from server.")
                Dim pDiffFilenames = GetDiffFilenames(pFileJsons)

                If pDiffFilenames.Count = 0 Then
                    Return JsonDictionary.Serialize("Result", AddLog("No need to update."))
                End If
                pZipFilename = IO.Path.Combine(Common.ServerPath, "temp", "UpdateServer", "DiffFileNames", DateTimeStamp() & ".zip")
                CreateFolder(pZipFilename)
                Dim pAllDiffFilenames = ""
                pZip = New ProZip(pZipFilename)
                For Each pDiffFilename In pDiffFilenames
                    pAllDiffFilenames &= pDiffFilename & vbCrLf
                Next pDiffFilename
                pZip.AddString(pAllDiffFilenames, "Files.txt")
                AddLog("Requesting updated files")
                Dim pUploadResult = New WebClient().UploadFile(RemoteDiffFilesUploadUrl(pRemoteServer), pZipFilename)
                Dim pUploadResultText = System.Text.Encoding.UTF8.GetString(pUploadResult)
                Dim pUploadResultJson = JsonConvert.DeserializeObject(Of UploadResultJson)(pUploadResultText)
                If String.IsNullOrWhiteSpace(pUploadResultJson.ErrorMessage) = False Then
                    Return JsonDictionary.Serialize("Error", AddLog("Error during upload: " & pUploadResultJson.ErrorMessage))
                End If

                pZipFilename = IO.Path.Combine(Common.ServerPath, "temp", "UpdateServer", "FileUpdates", DateTimeStamp() & ".zip")
                CreateFolder(pZipFilename)

                Dim pDownloadAttempts As Integer = 0
                Dim pRemoteGetFilesUrl As String
                Do
                    pDownloadAttempts += 1
                    If pDownloadAttempts >= 10 Then
                        Return JsonDictionary.Serialize("Error", AddLog("Download attempts limit reached. " & pRemoteGetFilesUrl & " ref: vbKWS7FZ83QA!"))
                    End If
                    AddLog("Downloading updated files. Attempt #" & pDownloadAttempts)
                    Try
                        pRemoteGetFilesUrl = RemoteGetFilesUrl(pUploadResultJson.Key, pRemoteServer)
                        AddLog("pRemoteGetFilesUrl = " & pRemoteGetFilesUrl)
                        pWeb.DownloadFile(pRemoteGetFilesUrl, pZipFilename)
                        Dim pFileText = GetFileText(pZipFilename, 10000)
                        If CStr2(pFileText).IndexOf("in progress") = 0 Then
                            Continue Do
                        End If
                        Exit Do
                    Catch ex As Exception
                        Debug.WriteLine("vbR9HMXCBXMP! " & ex.Message)
                        Threading.Thread.Sleep(5000)
                    End Try
                Loop

                AddLog("Extracting updated files.")
                Dim pExtractResult = New DeployController().Extract(pZipFilename)
                Return JsonDictionary.Serialize("Result", AddLog(pExtractResult))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", AddLog(ex.Message & ". ref: vbPRBVQCB2DY!"))
            Finally
                If String.IsNullOrWhiteSpace(pRemoteServer) Then
                    ExecuteDelayed()
                End If
            End Try
        End SyncLock

    End Function

    Private Sub ExecuteDelayed()

        mTimer = New Timers.Timer()
#If DEBUG Then
        mTimer.Interval = 15 * 1000
#Else
        mTimer.Interval = 5 * 60 * 1000
#End If
        AddHandler mTimer.Elapsed,
            Sub()
                mTimer.Enabled = False
                Execute()
            End Sub
        mTimer.Enabled = True

    End Sub

    Private Function AddLog(vText As String) As String

        If String.IsNullOrWhiteSpace(mLogPrefix) Then
            mLogPrefix = "common"
        End If
        ProPack.UtilityRedundant.AppendToLog(vText, "temp\logs\" & "UpdateServer", mLogPrefix)

        Return vText

    End Function

    Public Class UploadResultJson
        Public ErrorMessage As String
        Public Key As String
    End Class

    Private Function GetDiffFilenames(vFilteredFiles As FileJsons) As List(Of String)

        Dim pOut = New List(Of String)
        For Each pFilteredFile In vFilteredFiles
            If pFilteredFile.RelativeName.StartsWith("\") Then
                pFilteredFile.RelativeName = pFilteredFile.RelativeName.Substring(1, pFilteredFile.RelativeName.Length - 1)
            End If
            Dim pFilename = IO.Path.Combine(Common.ServerPath, pFilteredFile.RelativeName)
            Dim pFileInfo = New IO.FileInfo(pFilename)
            If pFileInfo.Exists = False _
                OrElse pFilteredFile.FileSize <> pFileInfo.Length _
                OrElse pFilteredFile.Crc <> ProPack.Security.Crypto.GetCRC32FromFile(pFilename) Then

                Dim pRelativeName = GetRelativePath(Common.ServerPath, pFilename)
                pOut.Add(pRelativeName)
            End If
        Next pFilteredFile

        Return pOut

    End Function

    Public Class FileJson
        Public RelativeName As String
        Public FileSize As Integer
        Public Crc As String
        Public FileDate As Date
    End Class

    Public Class FileJsons
        Inherits List(Of FileJson)
    End Class

    Private Shared Function NormalizeUrl(vRawUrl As String) As String

        Dim pUrl = vRawUrl
        If pUrl.ToLower().StartsWith("http://") = False Then
            pUrl = "http://" & pUrl
        End If

        pUrl = AppendIfNeeded(pUrl, "/")

        Return pUrl

    End Function

    Private Function RemoteFileListUrl(Optional vServer As String = Nothing) As String

        Dim pUrl As String = vServer
        If String.IsNullOrWhiteSpace(vServer) Then
            pUrl = FileText(SettingsFilename)
        End If
        If String.IsNullOrEmpty(pUrl) Then
            Return Nothing
        End If

        If pUrl.ToLower().StartsWith("http://") = False Then
            pUrl = "http://" & pUrl
        End If

        pUrl = AppendIfNeeded(pUrl, "/")
        pUrl &= "UpdateServer/FilteredFileList"

        Return pUrl

    End Function

    Private Function RemoteDiffFilesUploadUrl(Optional vServer As String = Nothing) As String

        Dim pUrl As String = vServer
        If String.IsNullOrWhiteSpace(vServer) Then
            pUrl = FileText(SettingsFilename)
        End If
        If String.IsNullOrEmpty(pUrl) Then
            Return Nothing
        End If

        If pUrl.ToLower().StartsWith("http://") = False Then
            pUrl = "http://" & pUrl
        End If

        pUrl = AppendIfNeeded(pUrl, "/")
        pUrl &= "UpdateServer/DiffFilesUpload"

        Return pUrl

    End Function

    Private Function RemoteGetFilesUrl(vKey As String, Optional vServer As String = Nothing) As String

        Dim pUrl As String = vServer
        If String.IsNullOrWhiteSpace(vServer) Then
            pUrl = FileText(SettingsFilename)
        End If
        If String.IsNullOrEmpty(pUrl) Then
            Return Nothing
        End If

        If pUrl.ToLower().StartsWith("http://") = False Then
            pUrl = "http://" & pUrl
        End If

        pUrl = AppendIfNeeded(pUrl, "/")
        pUrl &= "UpdateServer/GetFiles?key=" & vKey

        Return pUrl

    End Function

    Shared ReadOnly mSemaphoreGetAllFiles As New SemaphoreSlim(1, 1)
    Shared mGetAllFilesStart As Date
    Public Function GetAllFiles() As ActionResult

        Dim pFileJson As FileJson
        Dim pDontRelease As Boolean

        Try
            If mSemaphoreGetAllFiles.Wait(2000) = False Then
                pDontRelease = True
                Return Content("Another GetAllFiles is running since " & mGetAllFilesStart & ". vb6UELQVHGTP!")
            End If

            Dim pCreateNew = CBool2(WebRequest.Instance.QueryString("CreateNew"))
            If pCreateNew = False Then
                Dim pFolder = IO.Path.Combine(cGlobal.AppPath, "temp", "UpdateServer", "LocalFiles")
                Dim pDirectory = New IO.DirectoryInfo(pFolder)
                If pDirectory.Exists() Then
                    Dim pFiles = pDirectory.GetFiles("GetAllFiles_*.zip")
                    Dim pSortedFiles = pFiles.OrderBy(Function(vFile) vFile.LastWriteTime).Reverse()
                    For Each pFile In pSortedFiles
                        If pFile.Name.Contains("_pending") Then
                            Exit For
                        Else
                            Return File(pFile.FullName, "application/zip", pFile.Name)
                        End If
                    Next
                End If
            End If

            Dim pList = FileText(FileFilterList())
            If String.IsNullOrWhiteSpace(pList) Then
                FileText(FileFilterList()) = ""
                Return Content(JsonDictionary.Serialize("Error", "UpdateServerFileFilterList.txt does not exist (or is blank) on the update server."))
            End If

            mGetAllFilesStart = Now
            TimedThread(
                Sub()
                    Try
                        Dim pFilters = pList.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
                        Dim pFileJsons = New FileJsons()
                        For Each pFilter In pFilters
                            If pFilter.StartsWith("\") Then
                                pFilter = pFilter.Substring(1, pFilter.Length - 1)
                            End If
                            Dim pFilename = IO.Path.Combine(cGlobal.AppPath, pFilter)
                            Dim pRelativeName = GetRelativePath(cGlobal.AppPath, pFilename)
                            Dim pFilteredFilenames As String()
                            Try
                                pFilteredFilenames = IO.Directory.GetFiles(cGlobal.AppPath, pRelativeName, IO.SearchOption.AllDirectories)
                            Catch ex As Exception
                                Continue For
                            End Try
                            For Each pFilteredFilename In pFilteredFilenames
                                pFileJson = New FileJson
                                pFileJson.RelativeName = GetRelativePath(cGlobal.AppPath, pFilteredFilename)
                                pFileJsons.Add(pFileJson)
                            Next pFilteredFilename
                        Next pFilter

                        Dim pExist = From p In pFileJsons
                                     Where p.RelativeName.ToLower() = "web.config"
                                     Select p
                        If pExist.Any() = False Then
                            pFileJson = New FileJson
                            pFileJson.RelativeName = "web.config"
                            pFileJsons.Add(pFileJson)
                        End If

                        pExist = From p In pFileJsons
                                 Where p.RelativeName.ToLower() = "Global.asax".ToLower()
                                 Select p
                        If pExist.Any() = False Then
                            pFileJson = New FileJson
                            pFileJson.RelativeName = "Global.asax"
                            pFileJsons.Add(pFileJson)
                        End If

                        Dim pZipFilename = IO.Path.Combine(cGlobal.AppPath, "temp", "UpdateServer", "LocalFiles", "GetAllFiles_pending.zip")
                        CreateFolder(pZipFilename)
                        Dim pZip = New ProZip(pZipFilename)
                        Dim pDtFiles = pZip.GetFilenames()
                        For Each pFileJson In pFileJsons
                            If pFileJson.RelativeName.StartsWith("\") Then
                                pFileJson.RelativeName = pFileJson.RelativeName.Substring(1, pFileJson.RelativeName.Length - 1)
                            End If
                            Dim pRelativeName = pFileJson.RelativeName.Replace("\", "/")
                            Dim pExistingFiles = pDtFiles.Select("FullName = " & ReadyString(pRelativeName))
                            If pExistingFiles.Count > 0 Then
                                Continue For
                            End If
                            Dim pFullFilename = IO.Path.Combine(cGlobal.AppPath, pFileJson.RelativeName)
                            pRelativeName = GetRelativePath(cGlobal.AppPath, pFileJson.RelativeName)
                            pZip.AddFileRename(pFullFilename, pRelativeName)
                        Next

                        Dim pZipFilename2 = IO.Path.Combine(IO.Path.GetDirectoryName(pZipFilename), "GetAllFiles_" & DateTimeStamp(False) & ".zip")
                        MoveFile(pZipFilename, pZipFilename2)
                    Catch ex As Exception
                        AppendToDebug(ex.Message, True, ex, True)
                    Finally
                        mSemaphoreGetAllFiles.Release()
                    End Try
                End Sub
            )
            pDontRelease = True

            Return Content("mGetAllFilesStart: " & mGetAllFilesStart)
        Catch ex As Exception
            Dim pStr = ex.Message & vbCrLf &
                GetStackTrace(, ex)
            pStr = pStr.Replace(vbCrLf, "<br />" & vbCrLf)
            Return Content(pStr)
        Finally
            If pDontRelease = False Then
                mSemaphoreGetAllFiles.Release()
            End If
        End Try

    End Function

    Public Function FilteredFileList() As ActionResult

        Dim pList = FileText(FileFilterList())
        If String.IsNullOrWhiteSpace(pList) Then
            FileText(FileFilterList()) = ""
            Return Content(JsonDictionary.Serialize("Error", "UpdateServerFileFilterList.txt does not exist (or is blank) on the update server."))
        End If
        Dim pFilters = pList.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        Dim pFileJsons = New FileJsons()
        For Each pFilter In pFilters
            If pFilter.StartsWith("\") Then
                pFilter = pFilter.Substring(1, pFilter.Length - 1)
            End If
            Dim pFilename = IO.Path.Combine(Common.ServerPath, pFilter)
            Dim pRelativeName = GetRelativePath(Common.ServerPath, pFilename)
            Dim pFilteredFilenames As String()
            Try
                pFilteredFilenames = IO.Directory.GetFiles(Common.ServerPath, pRelativeName, IO.SearchOption.AllDirectories)
            Catch ex As Exception
                Continue For
            End Try
            For Each pFilteredFilename In pFilteredFilenames
                Dim pFileJson = New FileJson
                pFileJson.RelativeName = GetRelativePath(Common.ServerPath, pFilteredFilename)
                Dim pFileInfo = New IO.FileInfo(pFilteredFilename)
                pFileJson.FileSize = pFileInfo.Length
                pFileJson.FileDate = pFileInfo.LastWriteTime
                pFileJson.Crc = ProPack.Security.Crypto.GetCRC32FromFile(pFilteredFilename)
                pFileJsons.Add(pFileJson)
            Next pFilteredFilename
        Next pFilter

        Dim pText = JsonConvert.SerializeObject(pFileJsons)
        Dim pZipFilename = IO.Path.Combine(Common.ServerPath, "temp", "UpdateServer", "FilteredFileList", "FilteredFileList_" & DateTimeStamp() & ".zip")
        CreateFolder(pZipFilename)
        Dim pZip = New ProZip(pZipFilename)
        pZip.AddString(pText, "Files.txt")

        Return File(pZipFilename, "application/zip")

    End Function

    Private ReadOnly Property FileFilterList As String
        Get

            If mFileFilterList Is Nothing Then
                mFileFilterList = IO.Path.Combine(Common.ServerPath, "settings", "UpdateServerFileFilterList.txt")
            End If

            Return mFileFilterList

        End Get
    End Property

    Public Function DiffFilesUpload() As String

        Dim pOut As New UploadResultJson()

        Try
            Dim pZipFilename = IO.Path.Combine(Common.ServerPath, "temp", "UpdateServer", "DiffFilesUpload", "DiffFilesUpload_" & DateTimeStamp() & ".zip")
            CreateFolder(pZipFilename)
            If Request.Files.Count <> 1 Then
                pOut.ErrorMessage = "Number of uploaded files must be 1. ref: vbT7GKVH55RD!"
                Return JsonConvert.SerializeObject(pOut)
            End If
            Request.Files(0).SaveAs(pZipFilename)
            Dim pZip = New ProZip(pZipFilename)
            If pZip.Exist("Files.txt") = False Then
                pOut.ErrorMessage = "Files.txt does not exist"
                Return JsonConvert.SerializeObject(pOut)
            End If
            Dim pKey = GenerateString(10)
            Dim pFilename = IO.Path.Combine(Common.ServerPath, "temp", "UpdateServer", "DiffFilesUpload", pKey & ".txt")
            pZip.UnzipFile("Files.txt", pFilename)
            pOut.Key = pKey
            Return JsonConvert.SerializeObject(pOut)
        Catch ex As Exception
            pOut.ErrorMessage = ex.Message
            Return JsonConvert.SerializeObject(pOut)
        End Try

    End Function

    Public Function IpVerificationToken() As String

        Return mIpVerificationToken

    End Function

    <HttpGet>
    Public Function GetFiles() As ActionResult

        Dim pKey = Common.WebRequest.QueryString("key")
        If String.IsNullOrWhiteSpace(pKey) Then
            Return Content("pKey is required. vbVC6VGA9VHS!")
        End If
        Dim pZipFilename = IO.Path.Combine(Common.ServerPath, "temp", "UpdateServer", "LocalFiles", "LocalFiles_" & pKey & ".zip")

        If IO.File.Exists(pZipFilename) Then
            Return File(pZipFilename, "application/zip")
        End If

        If IO.File.Exists(pZipFilename & ".temp") Then
            Return Content("in progress. vb8QMCPW3UCX!")
        End If

        Dim pFilename = IO.Path.Combine(Common.ServerPath, "temp", "UpdateServer", "DiffFilesUpload", pKey & ".txt")

        Dim pText = FileText(pFilename)
        CreateFolder(pZipFilename)
        Dim pZip = New ProZip(pZipFilename & ".temp")
        Dim pFilenames = pText.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        For Each pFilename In pFilenames
            If pFilename.StartsWith("\") Then
                pFilename = pFilename.Substring(1, pFilename.Length - 1)
            End If
            Dim pFullFilename = IO.Path.Combine(Common.ServerPath, pFilename)
            Dim pRelativeName = GetRelativePath(Common.ServerPath, pFilename)
            pZip.AddFileRename(pFullFilename, pRelativeName)
        Next pFilename
        Rename(pZipFilename & ".temp", pZipFilename)

        Return File(pZipFilename, "application/zip")

    End Function
End Class
