Imports System.Collections.Concurrent
Imports System.Collections.Specialized
Imports System.Threading
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class WebPosFlush
    Private Shared mThread As Thread
    Private Shared mThreadLock As New Object
    Private Shared mProData As ProData
    Private Shared mBusiness As Business
    Private Shared mDatabase As String
    Private Shared mRemoteServer As String
    Private Shared mMessages As New Messages
    Public Const UnflushedPrefix As String = "Unflushed_"
    Private Shared mNoPrefixUnflushedSalesReturnRowCount As Integer
    Private Shared mPrefixUnflushedSalesReturnRowCount As Integer
    Public Shared LastQueryTime As Date
    Private Shared mCurrentPrefix As String = ""

    Public Shared Function UnflushedSalesReturnRowCount() As Integer

        Return mNoPrefixUnflushedSalesReturnRowCount + mPrefixUnflushedSalesReturnRowCount

    End Function

    Private Shared Function Start() As String

        'Return "debugging only" ' debugging only
        SyncLock mThreadLock
            If mThread IsNot Nothing _
                AndAlso mThread.IsAlive Then

                Return "busy " & DateTimeStamp()
            End If
            mThread = TimedThread(
                Sub()
                    Debug.WriteLine("start here")
                    Dim pHasPrefix As Boolean = True
                    Do
                        If Init(If(pHasPrefix, UnflushedPrefix, "")) = False Then
                            pHasPrefix = Not pHasPrefix
                        End If
                        If UnflushedSalesReturnRowCount() = 0 Then
                            Thread.Sleep(30000)
                        Else
                            Thread.Sleep(5000)
                        End If

                        ' dummy code so we can exit the loop while debugging
                        If Now < New Date(2000, 1, 1) Then
                            Exit Do
                        End If
                    Loop
                    Debug.WriteLine("cant go here")
                End Sub)
            Return "started " & DateTimeStamp()
        End SyncLock

    End Function

    Public Shared Function StartAsync() As Task

        Start()

    End Function

    Public Shared Function GetRecentMessages(vTimeStamp As Date) As List(Of Messages.Message)

        Return mMessages.GetRecent(vTimeStamp)

    End Function

    Private Shared Function Init(vPrefix As String) As Boolean
        ' outputs:
        '   false: go the the 2nd Init
        '   true : dont go to the 2nd init

        mCurrentPrefix = vPrefix
        mRemoteServer = WebPosDbDownloaderController.GetRemoteServer()
        If String.IsNullOrWhiteSpace(mRemoteServer) Then
            mMessages.Add("The ""settings/WebPosRemoteServer.txt"" file is blank or does not exist.")
            Return False
        Else
            mMessages.Add("The remote server is " & mRemoteServer)
        End If
        Using pProData = ProData.CreateFromConnectionInfo(vServer:=Common.ConnectionInfo.Server,
            vDatabase:="",
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)

            mProData = pProData
            Try
                mProData.ExecuteSQL("USE " & EscapeSqlName(vPrefix & "Mobiz"))
            Catch ex As Exception
                If ex.Message.Contains("does not exist") Then
                    Return False
                Else
                    Throw
                End If
            End Try
            mBusiness = New Business(mProData)

            mMessages.Add("Looping in private databases for unflushed data")
            Dim pSql = "SELECT OrganizationID" & vbCrLf &
                ",[Database]" & vbCrLf &
                " FROM MobizLibrarySubscription" & vbCrLf &
                " ORDER BY [Database]"
            Dim pDtSubscriptions = mProData.Execute(pSql)
            Dim pDtFlushables = pDtSubscriptions.Clone()
            Dim pDatabases = mProData.GetDatabases(True, True)
            For Each pDrSubscription As DataRow In pDtSubscriptions.Rows
                Dim pDatabase = vPrefix.ToLower() & CStr2(pDrSubscription("Database")).Trim().ToLower()
                If pDatabase = vPrefix & "mobiz" Then
                    Continue For
                End If
                If pDatabases.Contains(pDatabase) Then
                    Dim pDbFilename = pProData.GetFirstDataFilePath(pDatabase)
                    If pDbFilename.StartsWith(AppendBackslash(Common.ServerPath) & "WebPosDb") Then
                        'If String.IsNullOrWhiteSpace(vPrefix) _
                        '    AndAlso CStr2(pDrSubscription("Database")).Trim().ToLower().StartsWith(UnflushedPrefix.ToLower()) Then

                        '    mMessages.Add("Skipping flushing of current database. Proceeding to flush the unflushed database.")
                        '    Return
                        'End If
                        pDtFlushables.ImportRow(pDrSubscription)
                    End If
                End If
            Next

            If pDtFlushables.Rows.Count = 0 Then
                Return False
            End If

            For Each pDrFlushable As DataRow In pDtFlushables.Rows
                mDatabase = vPrefix & pDrFlushable("Database")
                Dim pOut = FollowUp(ReadyGuid(pDrFlushable("OrganizationID")), mDatabase)
                Return pOut
            Next
        End Using

    End Function

    Private Shared Function FollowUp(vOrgID As String, vDatabase As String) As Boolean
        ' outputs:
        '   false: dont follow up this database
        '   true : must follow up this database

        mProData.ExecuteSQL("USE " & EscapeSqlName(vDatabase))
        Dim pUuid As String
        Dim pSettings = WebPosDbDownloaderController.GetWebPosSettings(mBusiness)
        If String.IsNullOrWhiteSpace(pSettings.UUID) Then
            pUuid = LoadDeviceUuidFirstLine()
        Else
            pUuid = pSettings.UUID
        End If
        If String.IsNullOrWhiteSpace(pUuid) Then
            mMessages.Add("Set this server's device profile in System Administration > WebPOS Settings. Old message for backward compatibility: WebPosUuids.txt is blank or does not exist. Please go to WebPOS first before flushing. ref: vbYJM7R6GS53!")
            Return False
        End If

        Dim pDrDevice = mBusiness.LibraryDevice.ByUuid(pUuid)
        If pDrDevice Is Nothing Then
            mMessages.Add("Device with UUID, " & pUuid & ", does not exist in database, " & vDatabase & ". ref: vbDUVQUXGCWM!")
            Return False
        End If

        Dim pResultText As String
        vOrgID = ReadyGuid(vOrgID)
        Dim pWC = New TrailNetSuite.PatientWebClient
        Dim pJson = New JsonDictionary()
        pJson.Add("OrgID", vOrgID)
        pJson.Add("DeviceID", CStr2(pDrDevice("ID")))
        pJson.Add("WebPos", 1)

        mMessages.Add("Consulting server for " & mDatabase)
        Try
            Dim pUrl = mRemoteServer & "/OfflineData/FollowUpImportData"
            pResultText = pWC.UploadString(pUrl, pJson.ToString())
            If String.IsNullOrWhiteSpace(pResultText) Then
                Return False
            End If
        Catch ex As Exception
            mMessages.Add(ex.Message & ". vbGTUANB8ESN!")
            Return False
        End Try

        Try
            Dim pFollowUpResult = JsonConvert.DeserializeObject(Of FollowUpResult)(pResultText)
            If pFollowUpResult.MustFollowUp Then
                mMessages.Add("Waiting for server to finish importing")
                Thread.Sleep(10000)
            ElseIf pFollowUpResult.MustUpload Then
                If FlushDatabase(vOrgID, vDatabase, pFollowUpResult.DeviceID) = False Then
                    Return False
                End If
            ElseIf CStr2(pFollowUpResult.d.Result) = "ok" Then
                DownloadImportResult(vOrgID, CStr2(pDrDevice("ID")), pFollowUpResult.d.Filename)
            ElseIf pFollowUpResult.d.Error Like "Device with UUID * does not exist on the current database." Then
                mMessages.Add(pFollowUpResult.d.Error)
                SaveDeviceUuidRemoveFirstLine()
                Return False
            ElseIf CStr2(pFollowUpResult.d.Error).Trim() <> "" Then
                mMessages.Add("The server encountered an error during flushing: " & pFollowUpResult.d.Error)
                Return False
            ElseIf pFollowUpResult.DeviceDataExportKey IsNot Nothing Then
                RequestDeviceDataExport(vOrgID, CStr2(pDrDevice("ID")), pFollowUpResult.DeviceDataExportKey)
            Else
                mMessages.Add("Unexpected result " & pResultText)
                Return False
            End If
        Catch ex As Exception
            ' just ignore for now
            mMessages.Add("Unable to JSON-parse message from server: " & pResultText & vbCrLf &
                "Error: " & ex.Message & vbCrLf &
                GetStackTrace(, ex))
            Return False
        End Try

        Return True

    End Function

    Public Class FollowUpResult
        Public d As dClass

        Public ReadOnly Property MustFollowUp As Boolean
            Get
                If d Is Nothing Then
                    Return False
                End If
                Return CBool2(d.MustFollowUp)
            End Get
        End Property

        Public ReadOnly Property MustUpload As Boolean
            Get
                If d Is Nothing Then
                    Return False
                End If
                Return CBool2(d.MustUpload)
            End Get
        End Property

        Public ReadOnly Property DeviceDataExportKey As String
            Get
                If d Is Nothing Then
                    Return Nothing
                End If
                If String.IsNullOrWhiteSpace(d.DeviceDataExportKey) = False Then
                    Return d.DeviceDataExportKey
                End If
            End Get
        End Property

        Public ReadOnly Property DeviceID As String
            Get
                If d Is Nothing Then
                    Return Nothing
                End If
                If String.IsNullOrWhiteSpace(d.DeviceID) = False Then
                    Return d.DeviceID
                End If
            End Get
        End Property

        Public Class dClass
            Public MustFollowUp As String
            Public MustUpload As String
            Public Filename As String
            Public Result As String
            Public DeviceDataExportKey As String
            Public DeviceID As String
            Public [Error] As String
        End Class
    End Class

    Private Shared Function FlushDatabase(vOrgID As String, vDatabase As String, vDestinationDeviceID As String) As Boolean

        Try
            Dim pUuid As String
            mMessages.Add("Querying database, " & vDatabase & ", for unflushed data")
            Dim pRecordCount As Integer
            Const pLimit As Integer = 25
            mProData.ExecuteSQL("USE " & EscapeSqlName(vDatabase))
            Dim pSettings = WebPosDbDownloaderController.GetWebPosSettings(mBusiness)
            If String.IsNullOrWhiteSpace(pSettings.UUID) Then
                pUuid = LoadDeviceUuidFirstLine()
            Else
                pUuid = pSettings.UUID
            End If
            If String.IsNullOrWhiteSpace(pUuid) Then
                mMessages.Add("Set this server's device profile in System Administration > WebPOS Settings. Old message for backward compatibility: WebPosUuids.txt is blank or does not exist. Please go to WebPOS first before flushing. ref: vbYJM7R6GS53!")
                Return False
            End If

            Dim pDrDevice = mBusiness.LibraryDevice.ByUuid(pUuid)
            If pDrDevice Is Nothing Then
                mMessages.Add("Device with UUID, " & pUuid & ", does not exist in database, " & vDatabase & ". ref: vb9MSG4HBFQC!")
                Return False
            End If

            Dim pFilename = AppendBackslash(Common.ServerPath) & "temp\WebPos\FlushOutbound\" & vOrgID & "\" & DateTimeStamp() & ".zip"
            CreateFolder(pFilename)
            Dim pZip As New ProZip(pFilename)
            Dim pJson As New JsonDictionary()
            pJson.Add("OrgID", vOrgID)
            pJson.Add("UUID", pUuid)
            pJson.Add("AppVersion", "WebPOS")
            pJson.Add("AppVersionName", "WebPOS")
            pJson.Add("AppBuildDate", Format(#1/1/2000#, cGlobal.FormatDateTimeSql))
            pJson.Add("AppInstallDate", Format(#1/1/2000#, cGlobal.FormatDateTimeSql))
            pZip.AddString(pJson.ToString(), "info.json")

            AddUnflushedData(vProData:=mProData,
                rZip:=pZip,
                oRecordCount:=pRecordCount,
                vLimit:=pLimit)

            If String.IsNullOrWhiteSpace(vDestinationDeviceID) = False Then
                Dim pDeviceDataExporter = New DeviceDataExporter()
                pDeviceDataExporter.Zip = pZip
                pDeviceDataExporter.Limit = pLimit
                pDeviceDataExporter.Business = mBusiness
                pDeviceDataExporter.OrgID = vOrgID
                pDeviceDataExporter.DestinationDeviceID = vDestinationDeviceID
                Dim pExportedCount = pDeviceDataExporter.Export()
                pRecordCount += pExportedCount
            End If

            If pRecordCount = 0 Then
                mMessages.Add("No unflushed data")
                If vDatabase.ToLower().StartsWith(UnflushedPrefix.ToLower()) Then
                    DetachAndArchive(vDatabase)
                    DetachAndArchive(UnflushedPrefix & "Mobiz")
                End If
                Return False
            End If
            Dim pUrl = mRemoteServer & "/OfflineData/Dump"
            Dim pValues = New NameValueCollection()
            pValues.Add("OrgID", vOrgID)
            pValues.Add("DeviceID", CStr2(pDrDevice("ID")))
            mMessages.Add("Uploading " & pRecordCount & " unflushed records from " & pFilename & " from " & vDatabase)
            Dim pResult = UploadFile(pFilename, pUrl, pValues)
            mMessages.Add("Finished uploading")
            Return True
        Catch ex As Exception
            mMessages.Add("vbY5UURS9UNX! Error in FlushDatabase. " & ex.Message)
            Throw
        End Try

    End Function

    Private Shared Async Sub RequestDeviceDataExport(vOrgID As String, vDeviceID As String, vKey As String)

        Dim pTimeoutTarget As Date
        Dim pWeb = New TrailNetSuite.PatientWebClient()
        Dim pUrl = mRemoteServer & "/OfflineData/DownloadDeviceData?OrgID=" & vOrgID & "&Key=" & vKey
        Dim pFilename = AppendBackslash(Common.ServerPath) & "temp\DeviceDataExporter\Inbound\" & vOrgID & "\" & vKey & ".zip"
        Dim pDownloadSuccess As Boolean

        Do
            Try
                CreateFolder(pFilename)
                mMessages.Add("Downloading from " & pUrl)
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
                    If pDownloadTask.Status = TaskStatus.RanToCompletion Then
                        IO.File.Move(pFilename & ".temp", pFilename)
                        pDownloadSuccess = True
                        mMessages.Add("DownloadDeviceDataExport complete. ref: vbUR8B4FD7TT!")
                    Else
                        Try
                            mMessages.Add("DownloadDeviceDataExport error after download. ref: vbXBJRTQQ7EK! " & pDownloadTask.Exception.InnerException.InnerException.Message)
                        Catch ex As Exception
                            mMessages.Add("DownloadDeviceDataExport unknown error. ref: vbSEQJS9GT6X! " & ex.Message)
                        End Try
                    End If

                    Exit Do
                Else
                    ' timeout
                    mMessages.Add("DownloadDeviceDataExport timed out. ref: vb59RNWLMH6Y!")
                    pWeb.CancelAsync()
                End If
            Catch ex As Exception
                mMessages.Add("DownloadDeviceDataExport error: " & ex.Message & ". ref: vb589EEYFM3K!")
            End Try
            Await Threading.Tasks.Task.Delay(10000)
        Loop

        If pDownloadSuccess Then
            Dim pFileContent = GetFileText(pFilename, 500)
            If pFileContent.StartsWith("Error:") Then
                mMessages.Add("vbP5AYKEL3Y8! DownloadDeviceData source error: " & pFileContent)
                Return
            End If
            Dim pFilename2 = Common.ServerPath & "temp\Uploads\" & vOrgID & "\" & vDeviceID & "\" & vKey & ".zip"
            CreateFolder(pFilename2)
            IO.File.Move(pFilename, pFilename2)
            Dim pDataImport = New ImportDataClass
            pDataImport.OrgID = vOrgID
            pDataImport.DeviceID = vDeviceID
            pDataImport.InputFilename = vKey
            Dim pIsDone As Boolean
            AddHandler pDataImport.Done,
                Sub()
                    pIsDone = True
                End Sub
            pDataImport.Start()
            Do
                If pIsDone Then
                    Exit Do
                End If
                Await Threading.Tasks.Task.Delay(1000)
            Loop
            If pDataImport.IsSuccess Then
                pFilename = Common.ServerPath & "temp\Uploads\" & vOrgID & "\" & vDeviceID & "\" & vKey & "_imported.zip"
                If IO.File.Exists(pFilename) Then
                    UploadImportedIDs(pFilename)
                End If
            End If
        End If

    End Sub

    Private Shared Sub UploadImportedIDs(vFilename As String)

        Dim pOrgID As String
        Dim pDeviceID As String

        Dim pInfo = New ProZip(vFilename).GetString("info.json")
        If CStr2(pInfo).Trim = "" Then
            Debug.WriteLine("Error: Info.json does not exist or is empty. vb7LPDRN29GD!")
            Return
        End If
        Try
            Dim pInfoJson = New JsonDictionary()
            pInfoJson.SetJson(pInfo)
            If pInfoJson.ContainsKey("OrgID") Then
                pOrgID = pInfoJson.Item("OrgID")
            Else
                Debug.WriteLine("Error: Info.json does not contain OrgID. vbSWX29CNERP!")
                Return
            End If
            If IsGuid(pOrgID) = False Then
                Debug.WriteLine("Error: Info.json OrgID is not a GUID. vbBB63FBK4F2!")
                Return
            End If

            If pInfoJson.ContainsKey("DeviceID") Then
                pDeviceID = pInfoJson.Item("DeviceID")
            Else
                Debug.WriteLine("Error: Info.json does not contain DeviceID. vbMPCQWPPE9B!")
                Return
            End If
            If IsGuid(pDeviceID) = False Then
                Debug.WriteLine("Error: Info.json DeviceID is not a GUID. vbZAWLUCCSXC!")
                Return
            End If
        Catch ex As System.Exception
            Debug.WriteLine("Error: Info.json does not contain a valid JSON. vbLYWXPBRHUT!")
            Return
        End Try

        Dim pUrl = mRemoteServer & "/OfflineData/DumpImportedIDs"
        Dim pValues = New NameValueCollection()
        pValues.Add("OrgID", pOrgID)
        pValues.Add("DeviceID", pDeviceID)
        mMessages.Add("Uploading " & vFilename & " to " & pUrl)
        Dim pUploadResult As String
        Try
            pUploadResult = UploadFile(vFilename, pUrl, pValues)
            mMessages.Add("Finished uploading. vbPD7D3E93UW!")
        Catch ex As Exception
            mMessages.Add("Error: " & ex.Message & " ref: vbDZKCA8D82A!")
            Return
        End Try

        Dim pTimeStamp As String
        Try
            Dim pInfoJson = New JsonDictionary()
            pInfoJson.SetJson(pUploadResult)
            If pInfoJson.ContainsKey("Error") Then
                mMessages.Add("Error in DumpImportedIDs: " & pInfoJson.Item("Error") & " ref: vb3FJBA8SWK7!")
                Return
            End If
            If pInfoJson.ContainsKey("TimeStamp") = False _
                OrElse String.IsNullOrWhiteSpace(pInfoJson.Item("TimeStamp")) Then

                mMessages.Add("Error: TimeStamp is missing in JSON. ref: vbMAYMVFUZ2R!")
                Return
            End If
            pTimeStamp = pInfoJson.Item("TimeStamp")
        Catch ex As Exception
            Debug.WriteLine("Error: Info.json does not contain a valid JSON. " & ex.Message & " ref: vbSX7FGY7PMY!")
            Return
        End Try

        Dim pResultText As String
        Dim pJson = New JsonDictionary()
        pJson.Add("OrgID", pOrgID)
        pJson.Add("DeviceID", pDeviceID)
        pJson.Add("TimeStamp", pTimeStamp)
        Try
            pUrl = mRemoteServer & "/OfflineData/ProcessImportedIDs"
            mMessages.Add("Calling " & pUrl)
            pResultText = New TrailNetSuite.PatientWebClient().UploadString(pUrl, pJson.ToString())
            If String.IsNullOrWhiteSpace(pResultText) Then
                mMessages.Add("No reply from server. vbFEFVJ6BDMH!")
                Return
            End If
        Catch ex As Exception
            mMessages.Add("Error: " & ex.Message & ". vbJREBYLXBXM!")
            Return
        End Try

        Try
            Dim pInfoJson = New JsonDictionary()
            pInfoJson.SetJson(pResultText)
            If pInfoJson.ContainsKey("Error") Then
                mMessages.Add("Error in UploadImportedIDs: " & pInfoJson.Item("Error") & " ref: vb5GALUVKYKU!")
                Return
            End If
            If pInfoJson.ContainsKey("Result") = False _
                OrElse String.IsNullOrWhiteSpace(pInfoJson.Item("Result")) Then

                mMessages.Add("vbEY3TSW6FZJ! Unexpected response: " & pResultText)
                Return
            End If
            If pInfoJson.Item("Result") = "ok" Then
                mMessages.Add("vbZVSFKEJMAP! Tags were successfully imported by the remote server.")
            End If
        Catch ex As Exception
            Debug.WriteLine("Error: Info.json does not contain a valid JSON. " & ex.Message & " ref: vbEF34MLL4SK!")
            Return
        End Try

    End Sub

    Private Shared Sub DetachAndArchive(vDatabase As String)

        If vDatabase.ToLower().StartsWith(UnflushedPrefix.ToLower()) = False Then
            Return
        End If
        mMessages.Add("Detaching database, " & vDatabase)
        Dim pDtFiles = mProData.GetDatabaseFiles(vDatabase)
        mProData.DetachDatabase(vDatabase)
        For Each pDrFile As DataRow In pDtFiles.Rows
            Dim pSourceFilename = pDrFile("physical_name")
            Dim pDestinationFilename = IO.Path.Combine(Common.ServerPath, "temp\WebPos\Archives", IO.Path.GetFileName(pSourceFilename) & "." & DateTimeStamp(False))
            mMessages.Add("Archiving " & vDatabase & " file, " & pSourceFilename & ", to " & pDestinationFilename)
            CreateFolder(pDestinationFilename)
            MoveFile(pSourceFilename, pDestinationFilename)
        Next

    End Sub

    Private Shared Function AddUnflushedData(vProData As ProData, rZip As ProZip, ByRef oRecordCount As Integer, vLimit As Integer) As Integer

        If String.IsNullOrWhiteSpace(mCurrentPrefix) Then
            mNoPrefixUnflushedSalesReturnRowCount = 0
        Else
            mPrefixUnflushedSalesReturnRowCount = 0
        End If

        Dim pInvoiceIds As String
        Dim pSqlWhere = " WHERE TransInvoice.DateImported IS NULL" & vbCrLf +
                " AND ISNULL(TransInvoice.IsDraft, 0) = 0" & vbCrLf +
                " AND (" & vbCrLf +
                "   (TransInvoice.InvoiceNumber > 0)" & vbCrLf +
                "   OR (ISNULL(TransInvoice.SalesOrderNo, 0) > 0)" & vbCrLf +
                "   OR (PurchaseOrderNo > 0)" & vbCrLf +
                ")"
        Dim pDT = vProData.Execute("SELECT TOP " & vLimit & "  * FROM TransInvoice" & vbCrLf &
            pSqlWhere &
            " ORDER BY TransInvoice.InvoiceNumber, TransInvoice.LastUpdated")
        If pDT.Rows.Count > 0 Then
            If vLimit = 1 Then
                Return pDT.Rows.Count
            End If
            pInvoiceIds = CStr2(ExtractFieldValues(pDT, "ID", Integer.MaxValue)(0)("ID"))
            mMessages.Add("TransInvoice: " & pDT.Rows.Count & " unflushed records")
            oRecordCount += pDT.Rows.Count
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransInvoice")
            End If

            pDT = vProData.Execute("SELECT TransInvoiceItems.* " +
                " FROM TransInvoiceItems" +
                " WHERE TransInvoiceItems.InvoiceID IN (" + pInvoiceIds + ")" +
                " ORDER BY TransInvoiceItems.LineNumber")
            If pDT.Rows.Count > 0 Then
                If vLimit = 1 Then
                    Return pDT.Rows.Count
                End If
                mMessages.Add("TransInvoiceItems: " & pDT.Rows.Count & " unflushed records")
                oRecordCount += pDT.Rows.Count
            End If
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransInvoiceItems")
            End If
            pDT = vProData.Execute("SELECT TransInvoiceItemsModifier.* " +
                " FROM TransInvoiceItemsModifier" +
                " WHERE TransInvoiceItemsModifier.InvoiceItemID IN " & vbCrLf +
                " (SELECT TransInvoiceItems.ID" +
                " 	FROM TransInvoiceItems" +
                " 	WHERE TransInvoiceItems.InvoiceID IN (" + pInvoiceIds + ")" +
                ")")
            If pDT.Rows.Count > 0 Then
                If vLimit = 1 Then
                    Return pDT.Rows.Count
                End If
                mMessages.Add("TransInvoiceItemsModifier: " & pDT.Rows.Count & " unflushed records")
                oRecordCount += pDT.Rows.Count
            End If
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransInvoiceItemsModifier")
            End If

            pDT = vProData.Execute("SELECT TransInvoiceDiscount.* " +
                " FROM TransInvoiceDiscount" +
                " WHERE TransInvoiceDiscount.InvoiceID IN (" + pInvoiceIds + ")")
            If pDT.Rows.Count > 0 Then
                If vLimit = 1 Then
                    Return pDT.Rows.Count
                End If
                mMessages.Add("TransInvoiceDiscount: " & pDT.Rows.Count & " unflushed records")
                oRecordCount += pDT.Rows.Count
            End If
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransInvoiceDiscount")
            End If

            pDT = vProData.Execute("SELECT TransInvoiceSeniorCitizenPWDCustomer.* " +
                " FROM TransInvoiceSeniorCitizenPWDCustomer" +
                " WHERE TransInvoiceSeniorCitizenPWDCustomer.InvoiceID IN (" + pInvoiceIds + ")")
            If pDT.Rows.Count > 0 Then
                If vLimit = 1 Then
                    Return pDT.Rows.Count
                End If
                mMessages.Add("TransInvoiceSeniorCitizenPWDCustomer: " & pDT.Rows.Count & " unflushed records")
                oRecordCount += pDT.Rows.Count
            End If
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransInvoiceSeniorCitizenPWDCustomer")
            End If

            pDT = vProData.Execute("SELECT TransInvoiceCharges.* " +
                " FROM TransInvoiceCharges" +
                " WHERE TransInvoiceCharges.InvoiceID IN (" + pInvoiceIds + ")")
            If pDT.Rows.Count > 0 Then
                If vLimit = 1 Then
                    Return pDT.Rows.Count
                End If
                mMessages.Add("TransInvoiceCharges: " & pDT.Rows.Count & " unflushed records")
                oRecordCount += pDT.Rows.Count
            End If
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransInvoiceCharges")
            End If

            pDT = vProData.Execute("SELECT TransInvoiceItemSerials.* " +
                " FROM TransInvoiceItemSerials" +
                " WHERE TransInvoiceItemSerials.InvoiceID IN (" + pInvoiceIds + ")")
            If pDT.Rows.Count > 0 Then
                If vLimit = 1 Then
                    Return pDT.Rows.Count
                End If
                mMessages.Add("TransInvoiceItemSerials: " & pDT.Rows.Count & " unflushed records")
                oRecordCount += pDT.Rows.Count
            End If
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransInvoiceItemSerials")
            End If

            pDT = vProData.Execute("SELECT TransInvoiceItemSalesReps.*" + vbCrLf +
                " FROM TransInvoiceItemSalesReps" + vbCrLf +
                " JOIN TransInvoiceItems ON TransInvoiceItemSalesReps.InvoiceID = TransInvoiceItems.InvoiceID" + vbCrLf +
                " WHERE TransInvoiceItems.InvoiceID IN (" + pInvoiceIds + ")")
            If pDT.Rows.Count > 0 Then
                If vLimit = 1 Then
                    Return pDT.Rows.Count
                End If
                mMessages.Add("TransInvoiceItemSalesReps: " & pDT.Rows.Count & " unflushed records")
                oRecordCount += pDT.Rows.Count
            End If
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransInvoiceItemSalesReps")
            End If

            pDT = vProData.Execute("SELECT TransInvoiceItemsRebates.* " +
                " FROM TransInvoiceItemsRebates" +
                " WHERE TransInvoiceItemsRebates.ModifierGroupID IN " +
                " (SELECT TransInvoiceItems.ModifierGroupID" +
                " 	FROM TransInvoiceItems" +
                " 	WHERE TransInvoiceItems.InvoiceID IN (" + pInvoiceIds + ")" +
                ")")
            If pDT.Rows.Count > 0 Then
                If vLimit = 1 Then
                    Return pDT.Rows.Count
                End If
                mMessages.Add("TransInvoiceItemsRebates: " & pDT.Rows.Count & " unflushed records")
                oRecordCount += pDT.Rows.Count
            End If
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransInvoiceItemsRebates")
            End If
        End If

        Dim pSql = "SELECT * FROM TransCustomerPayment" +
            " WHERE TransCustomerPayment.DateImported IS NULL" & vbCrLf
        'If String.IsNullOrWhiteSpace(pInvoiceIds) = False Then
        pSql &= " AND ID IN " & vbCrLf &
            "   (SELECT TransCustomerPaymentInvoices.CustomerPaymentID" & vbCrLf &
            "   FROM TransCustomerPaymentInvoices" & vbCrLf &
            "   -- WHERE TransCustomerPaymentInvoices.InvoiceID IN (" + pInvoiceIds + ")" & vbCrLf &
            "   )" & vbCrLf
        'End If
        pSql &= " ORDER BY TransCustomerPayment.LastUpdated"
        pDT = vProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            If vLimit = 1 Then
                Return pDT.Rows.Count
            End If
            mMessages.Add("TransCustomerPayment: " & pDT.Rows.Count & " unflushed records")
            oRecordCount += pDT.Rows.Count
        End If
        If pDT.Rows.Count > 0 Then
            If vLimit = 1 Then
                Return pDT.Rows.Count
            End If
            Dim pCustomerPaymentIds = CStr2(ExtractFieldValues(pDT, "ID", Integer.MaxValue)(0)("ID"))
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransCustomerPayment")
            End If

            pDT = vProData.Execute("SELECT TransCustomerPaymentInvoices.* " +
                    " FROM TransCustomerPaymentInvoices" +
                    " WHERE TransCustomerPaymentInvoices.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            If pDT.Rows.Count > 0 Then
                If vLimit = 1 Then
                    Return pDT.Rows.Count
                End If
                mMessages.Add("TransCustomerPaymentInvoices: " & pDT.Rows.Count & " unflushed records")
                oRecordCount += pDT.Rows.Count
            End If
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransCustomerPaymentInvoices")
            End If

            pDT = vProData.Execute("SELECT TransCustomerPaymentDebitCard.*" + vbCrLf +
                " FROM TransCustomerPaymentDebitCard" + vbCrLf +
                " WHERE TransCustomerPaymentDebitCard.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            If pDT.Rows.Count > 0 Then
                If vLimit = 1 Then
                    Return pDT.Rows.Count
                End If
                mMessages.Add("TransCustomerPaymentDebitCard: " & pDT.Rows.Count & " unflushed records")
                oRecordCount += pDT.Rows.Count
            End If
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransCustomerPaymentDebitCard")
            End If

            pDT = vProData.Execute("SELECT TransCustomerPaymentCreditCard.*" + vbCrLf +
                " FROM TransCustomerPaymentCreditCard" + vbCrLf +
                " WHERE TransCustomerPaymentCreditCard.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            If pDT.Rows.Count > 0 Then
                If vLimit = 1 Then
                    Return pDT.Rows.Count
                End If
                mMessages.Add("TransCustomerPaymentCreditCard: " & pDT.Rows.Count & " unflushed records")
                oRecordCount += pDT.Rows.Count
            End If
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransCustomerPaymentCreditCard")
            End If

            pDT = vProData.Execute("SELECT TransCustomerPaymentChecks.*" + vbCrLf +
            " FROM TransCustomerPaymentChecks" + vbCrLf +
            " WHERE TransCustomerPaymentChecks.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            If pDT.Rows.Count > 0 Then
                If vLimit = 1 Then
                    Return pDT.Rows.Count
                End If
                mMessages.Add("TransCustomerPaymentChecks: " & pDT.Rows.Count & " unflushed records")
                oRecordCount += pDT.Rows.Count
            End If
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransCustomerPaymentChecks")
            End If

            pDT = vProData.Execute("SELECT TransCustomerPaymentOther.*" + vbCrLf +
            " FROM TransCustomerPaymentOther" + vbCrLf +
            " WHERE TransCustomerPaymentOther.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
            If pDT.Rows.Count > 0 Then
                If vLimit = 1 Then
                    Return pDT.Rows.Count
                End If
                mMessages.Add("TransCustomerPaymentOther: " & pDT.Rows.Count & " unflushed records")
                oRecordCount += pDT.Rows.Count
            End If
            If rZip IsNot Nothing Then
                rZip.AddTableCsv(pDT, "TransCustomerPaymentOther")
            End If
        End If

        pDT = vProData.Execute("SELECT * FROM MobizLibraryOrganization WHERE IsFromDevice = 1 AND FlushToServer = 1")
        If pDT.Rows.Count > 0 Then
            If vLimit = 1 Then
                Return pDT.Rows.Count
            End If
            mMessages.Add("LibraryOrganization: " & pDT.Rows.Count & " unflushed records")
            oRecordCount += pDT.Rows.Count
        End If
        If rZip IsNot Nothing Then
            rZip.AddTableCsv(pDT, "LibraryOrganization")
        End If

        pDT = vProData.Execute("SELECT LibraryOrganizationCustomer.*" + vbCrLf +
                " FROM LibraryOrganizationCustomer" + vbCrLf +
                " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = LibraryOrganizationCustomer.OrganizationID" + vbCrLf +
                " WHERE MobizLibraryOrganization.IsFromDevice = 1 AND MobizLibraryOrganization.FlushToServer = 1")
        If pDT.Rows.Count > 0 Then
            If vLimit = 1 Then
                Return pDT.Rows.Count
            End If
            mMessages.Add("LibraryOrganizationCustomer: " & pDT.Rows.Count & " unflushed records")
            oRecordCount += pDT.Rows.Count
        End If
        If rZip IsNot Nothing Then
            rZip.AddTableCsv(pDT, "LibraryOrganizationCustomer")
        End If

        pDT = vProData.Execute("SELECT * FROM TransMemo WHERE DateImported IS NULL")
        If pDT.Rows.Count > 0 Then
            If vLimit = 1 Then
                Return pDT.Rows.Count
            End If
            mMessages.Add("TransMemo: " & pDT.Rows.Count & " unflushed records")
            oRecordCount += pDT.Rows.Count
            If String.IsNullOrWhiteSpace(mCurrentPrefix) Then
                mNoPrefixUnflushedSalesReturnRowCount += pDT.Rows.Count
            Else
                mPrefixUnflushedSalesReturnRowCount += pDT.Rows.Count
            End If
        End If
        If rZip IsNot Nothing Then
            rZip.AddTableCsv(pDT, "TransMemo")
        End If

        pDT = vProData.Execute("SELECT * FROM TransMemoSalesReturn WHERE DateImported IS NULL")
        If pDT.Rows.Count > 0 Then
            If vLimit = 1 Then
                Return pDT.Rows.Count
            End If
            mMessages.Add("TransMemoSalesReturn: " & pDT.Rows.Count & " unflushed records")
            oRecordCount += pDT.Rows.Count
            If String.IsNullOrWhiteSpace(mCurrentPrefix) Then
                mNoPrefixUnflushedSalesReturnRowCount += pDT.Rows.Count
            Else
                mPrefixUnflushedSalesReturnRowCount += pDT.Rows.Count
            End If
        End If
        If rZip IsNot Nothing Then
            rZip.AddTableCsv(pDT, "TransMemoSalesReturn")
        End If

        pDT = vProData.Execute("SELECT * FROM TransSalesReturnLedger WHERE DateImported IS NULL")
        If pDT.Rows.Count > 0 Then
            If vLimit = 1 Then
                Return pDT.Rows.Count
            End If
            mMessages.Add("TransSalesReturnLedger: " & pDT.Rows.Count & " unflushed records")
            oRecordCount += pDT.Rows.Count
            If String.IsNullOrWhiteSpace(mCurrentPrefix) Then
                mNoPrefixUnflushedSalesReturnRowCount += pDT.Rows.Count
            Else
                mPrefixUnflushedSalesReturnRowCount += pDT.Rows.Count
            End If
        End If
        If rZip IsNot Nothing Then
            rZip.AddTableCsv(pDT, "TransSalesReturnLedger")
        End If

        LastQueryTime = Now

    End Function

    Private Shared Sub DownloadImportResult(vOrgID As String, vDeviceID As String, vFilename As String)

        If String.IsNullOrWhiteSpace(vFilename) Then
            Return
        End If

        mMessages.Add("Downloading import result")
        Dim pUrl = AppendIfNeeded(mRemoteServer, "/") & "OfflineData/DownloadImportResult" &
            "?Filename=" & vFilename &
            "&OrgID=" & vOrgID &
            "&DeviceID=" & vDeviceID
        Dim pWeb = New TrailNetSuite.PatientWebClient()
        AddHandler pWeb.DownloadProgressChanged,
            Sub(sender As Object, e As System.Net.DownloadProgressChangedEventArgs)
                Debug.WriteLine(e.ProgressPercentage & "%")
            End Sub
        Dim pFilename = AppendBackslash(Common.ServerPath) & "temp\WebPos\FlushOutbound\" & vOrgID & "\" & vFilename & ".zip"
        CreateFolder(pFilename)
        pWeb.DownloadFile(pUrl, pFilename & ".temp")
        MoveFile(pFilename & ".temp", pFilename)
        UpdateImportTag(pFilename)

        Dim pJson = New JsonDictionary()
        pJson.Add("Filename", vFilename)
        pJson.Add("OrgID", vOrgID)
        pJson.Add("DeviceID", vDeviceID)
        pJson.Add("DeviceDate", Format(Now, cGlobal.FormatDateTimeSql))
        pUrl = AppendIfNeeded(mRemoteServer, "/") & "OfflineData/SetImportedDeviceDate"
        Dim pResult = pWeb.UploadString(pUrl, pJson.ToString())
        Debug.WriteLine(pResult)

    End Sub

    Private Shared Sub UpdateImportTag(vFilename As String, Optional vDatabase As String = Nothing)

        Dim pDatabase As String


        If String.IsNullOrWhiteSpace(vDatabase) Then
            pDatabase = mDatabase
        Else
            pDatabase = vDatabase
        End If

        If pDatabase.StartsWith(UnflushedPrefix) = False Then
            Try
                mProData.ExecuteSQL("USE " & EscapeSqlName(UnflushedPrefix & pDatabase))
                UpdateImportTag(vFilename, UnflushedPrefix & pDatabase)
            Catch ex As Exception
                mMessages.Add("vbRWMN39ZQEU! " & ex.Message)
            End Try
        End If

        mMessages.Add("Tagging imported records from file, " & vFilename & ", to database, " & pDatabase & ".")
        mProData.ExecuteSQL("USE " & EscapeSqlName(pDatabase))
        TagDatabaseTables("TransInvoice", vFilename)
        TagDatabaseTables("TransCustomerPayment", vFilename)
        TagDatabaseTables("MobizLibraryOrganization", vFilename)
        TagDatabaseTables("TransMemo", vFilename)
        TagDatabaseTables("TransMemoSalesReturn", vFilename)
        TagDatabaseTables("TransSalesReturnLedger", vFilename)

    End Sub

    Private Shared Sub TagDatabaseTables(vTableName As String, vFilename As String)

        Dim pTotalAffectedRecords = 0
        Dim pIU = New InsertUpdate(EscapeSqlName(vTableName))
        Dim pCsvReader = New CsvReader(vFilename, vTableName)
        Do
            Dim pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pIU("DateImported") = "GETDATE()"
            Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
#If DEBUG Then
            'mProData.BeginTransaction()
#End If
            Dim pAffectedRecords = mProData.ExecuteSQL(pSql) ' output is used for debugging only
#If DEBUG Then
            'mProData.RollbackTransaction()
#End If
            If pAffectedRecords = 0 Then
                mMessages.Add("No records were tagged in " & vTableName & ". ID = " & pLine("ID"))
            Else
                pTotalAffectedRecords += 1
            End If
        Loop

        If pTotalAffectedRecords > 0 Then
            mMessages.Add("Total tagged records in " & vTableName & " = " & pTotalAffectedRecords)
        End If

    End Sub

    Private Shared ReadOnly mSaveDeviceUuidLock = New Object()

    Public Shared Sub SaveDeviceUuid(vUuid As String)

        SyncLock mSaveDeviceUuidLock
            Dim pFilename = IO.Path.Combine(Common.ServerPath) & "settings\WebPosUuids.txt"
            Dim pFileText = FileText(pFilename)
            Dim pLines = pFileText.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            Dim pUniqueLines = UniqueItems(pLines)
            If pUniqueLines.Contains(vUuid) = False Then
                pUniqueLines.Add(vUuid)
                Dim pFileText2 = String.Join(vbCrLf, pUniqueLines)
                FileText(pFilename) = pFileText2
            End If
        End SyncLock

    End Sub

    Public Shared Function LoadDeviceUuidFirstLine() As String

        SyncLock mSaveDeviceUuidLock
            Dim pFilename = IO.Path.Combine(Common.ServerPath) & "settings\WebPosUuids.txt"
            Dim pFileText = FileText(pFilename)
            Dim pLines = pFileText.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            If pLines.Length > 0 Then
                Return pLines(0)
            End If
        End SyncLock

    End Function

    Public Shared Sub SaveDeviceUuidRemoveFirstLine()

        SyncLock mSaveDeviceUuidLock
            Dim pFilename = IO.Path.Combine(Common.ServerPath) & "settings\WebPosUuids.txt"
            Dim pFileText = FileText(pFilename)
            Dim pLines = pFileText.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            If pLines.Length > 0 Then
                Dim pFileText2 = String.Join(vbCrLf, pLines, 1, pLines.Length - 1)
                FileText(pFilename) = pFileText2
            End If
        End SyncLock

    End Sub

    Public Shared Function HasAttachedUnflushed(vProData As ProData) As Boolean

        Dim pDatabases = vProData.GetDatabases(True, True)
        For Each pDatabase In pDatabases
            If pDatabase.StartsWith(UnflushedPrefix.ToLower()) Then
                Return True
            End If
        Next

    End Function

    Public Shared Function ReattachAsUnflushed(vProData As ProData, vDatabaseName As String) As Integer

        Dim pMdf As String
        Dim pDtFiles = vProData.GetDatabaseFiles(vDatabaseName)
        vProData.DetachDatabase(vDatabaseName)
        For Each pDrFiles As DataRow In pDtFiles.Rows
            Dim pDestinationFilename = pDrFiles("physical_name")
            If CStr2(pDrFiles("type_desc")).Trim().ToLower() = "rows" Then
                pDestinationFilename = IO.Path.Combine(IO.Path.GetDirectoryName(pDestinationFilename), UnflushedPrefix & IO.Path.GetFileName(pDestinationFilename))
                pMdf = pDestinationFilename
            Else
                pDestinationFilename = IO.Path.Combine(IO.Path.GetDirectoryName(pDestinationFilename), UnflushedPrefix & IO.Path.GetFileNameWithoutExtension(pDestinationFilename)) & "_" & DateTimeStamp(False) & IO.Path.GetExtension(pDestinationFilename)
            End If
            MoveFile(pDrFiles("physical_name"), pDestinationFilename)
        Next
        vProData.AttachDatabase(UnflushedPrefix & vDatabaseName, pMdf)

    End Function
End Class

Public Class Messages
    Dim mItems As New LimitedConcurrentQueue(Of Message)(1000)
    Dim mNum As Integer
    Dim mLastTimeStamp As Date

    Public Function GetRecent(vTimeStamp As Date) As List(Of Message)

        Dim pRecent = From pItem In mItems
                      Where pItem.TimeStamp > vTimeStamp
                      Order By pItem.TimeStamp

        Dim pOut = pRecent.ToList()
        Return pOut

    End Function

    Shared ReadOnly mAddLock As New Object
    Public Sub Add(vText As String)

        SyncLock mAddLock

            Dim pItem = New Message
            mNum += 1
            Dim pNow = Now
            If mLastTimeStamp = pNow Then
                pNow = mLastTimeStamp.AddMilliseconds(1)
            End If
            pItem.Num = mNum
            pItem.TimeStamp = pNow
            pItem.Text = vText
            mItems.Enqueue(pItem)
            mLastTimeStamp = Now
        End SyncLock

    End Sub

    Public Class Message
        Public Num As Integer
        Public TimeStamp As Date
        Public Text As String
    End Class
End Class

Public Class LimitedConcurrentQueue(Of T)
    ' https://stackoverflow.com/a/50261664/334045
    Inherits ConcurrentQueue(Of T)

    Dim mLimit As Integer

    Sub New(vLimit As Integer)

        mLimit = vLimit

    End Sub

    Public Shadows Sub Enqueue(vItem As T)

        MyBase.Enqueue(vItem)
        If Count > mLimit Then
            Dim pDummy As T
            TryDequeue(pDummy)
        End If

    End Sub
End Class