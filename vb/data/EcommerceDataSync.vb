Imports WebProj.WebProj
Imports Newtonsoft.Json
Imports System.Net
Imports System.Collections.Specialized
Imports System.Threading

Public Class EcommerceDataSync
    Dim mTimer As Timers.Timer
    Shared mStatusText As String
    Shared mIsBusy As Boolean
    Dim mMileStone As Milestone
    Dim mLogFileTitle As String
    Private ReadOnly mExecuteLock As New SemaphoreSlim(1, 1)

    Public Function StatusText() As String

        Return mStatusText

    End Function

    Public Function IsBusy() As String

        Return If(mIsBusy, 1, 0)

    End Function

    Public Async Sub Execute()

        If mIsBusy Then
            Return
        End If

#If DEBUG Then
        If Debugger.IsAttached = False Then
            Return
        End If
#End If

        mIsBusy = True
        Await mExecuteLock.WaitAsync()
        Try
            LoadMileStone()
            If mMileStone Is Nothing Then
                mIsBusy = False
                Return
            End If
            If mMileStone.IsDone Then
                mIsBusy = False
                Return
            End If

            Dim pWebClient As New WebClient
            If String.IsNullOrWhiteSpace(mMileStone.PublicServerUrl) Then
                mMileStone.ErrorMessage = "EcommerceDataSourceServerIP must not be blank. ref: vbCKLCTA6WZN!"
                Return
            End If

            Dim pUrl = mMileStone.PublicServerUrl & "MobizExport/RequestDataFromServer"
            Dim pValues = New NameValueCollection()
            Dim pJson As New JsonDictionary()
            pJson.Add("DontCheckCommonFowst", "1")
            pJson.Add("ForEcommerceServer", "1")
            'pJson.Add("PeopleID", modClient.EmployeeWebGuidID)
            pJson.Add("PeopleID", "c530bdf3-ea03-4dd3-95cd-a384374aa570") ' frans_eize
            pJson.Add("OrgID", mMileStone.OrgID)
            pValues.Add("json", pJson.ToString())
            If String.IsNullOrWhiteSpace(mMileStone.FileID) Then

                Using pProData = ProData.CreateFromConnectionInfo(
                    vServer:=Common.ConnectionInfo.Server,
                    vDatabase:=Common.ConnectionInfo.Database,
                    vUsername:=Common.ConnectionInfo.Username,
                    vPassword:=Common.ConnectionInfo.Password,
                    vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)

                    Dim pBusiness = New Business(pProData)
                    pBusiness.SystemDefaults.EcommerceDataLastRequest = Now
                End Using

                mStatusText = "Initializing data request from ecommerce server"
                Dim pResult As Byte() = Await pWebClient.UploadValuesTaskAsync(New Uri(pUrl), "POST", pValues)
                Dim pResultText = System.Text.Encoding.UTF8.GetString(pResult)
                Try
                    Dim pResultJson As ResultJson = JsonConvert.DeserializeObject(Of ResultJson)(pResultText)
                    If CStr2(pResultJson.Result).Length = 75 Then
                        Call New Threading.Tasks.Task(
                            Sub()
                                SplitRemoteFile(pResultJson.Result())
                            End Sub).Start()
                    Else
                        mTimer = New Timers.Timer()
                        mTimer.Interval = 5000
                        AddHandler mTimer.Elapsed, AddressOf mTimer_Elapsed
                        mTimer.Start()
                    End If
                Catch ex As Exception
                    mStatusText = ex.Message
                    mIsBusy = False
                End Try
            Else
                Call New Threading.Tasks.Task(
                    Sub()
                        SplitRemoteFile(mMileStone.FileID)
                    End Sub).Start()
            End If
        Finally
            mExecuteLock.Release()
        End Try

    End Sub

    Private Sub mTimer_Elapsed()

        Dim pWebClient As New Net.WebClient
        Dim pUrl As String = mMileStone.PublicServerUrl & "MobizExport/RequestFollowUp"
        Dim pResultText = pWebClient.DownloadString(pUrl)

        Try
            Dim pResultJson As ResultJson = JsonConvert.DeserializeObject(Of ResultJson)(pResultText)
            If CStr2(pResultJson.Result()).Length = 75 Then
                mMileStone.FileID = pResultJson.Result()
                SaveMilestone()
                mTimer.Dispose()
                SplitRemoteFile(mMileStone.FileID)
                Return
            End If
            mStatusText = DateTimeStamp() & ": " & pResultJson.Result()
        Catch ex As Exception
            mStatusText = ex.Message
            mIsBusy = False
        End Try

    End Sub

    Public Class ResultJson

        Public d As dClass

        Public Function Result() As String
            If d IsNot Nothing Then
                If String.IsNullOrWhiteSpace(d.Result) = False Then
                    Return d.Result
                Else
                    Return d.Results
                End If
            End If
        End Function

        Public Class dClass
            Public Result As String
            Public Results As String
        End Class
    End Class

    Public Function ExecuteCompleted(vStatus As String, vError As String) As String

        If String.IsNullOrWhiteSpace(vError) = False Then
            mStatusText = "Error=" & vError
            mIsBusy = False
        Else
            mStatusText = vStatus
        End If

        Dim pOut As New JsonDictionary()
        pOut.Add("Status", vStatus)
        pOut.Add("Error", vError)

        Return pOut.Serialize()

    End Function

    Private Async Sub SplitRemoteFile(vFileID As String)

        Dim pError As String
        Dim pWebClient As New Net.WebClient
        Dim pUrl = mMileStone.PublicServerUrl & "MobizExport/SplitFile"
        Dim pSplitJson As MobizExportController.SplitFileJson
        Dim pDestinationFilename As String

        If String.IsNullOrWhiteSpace(mMileStone.ExtractedFolder) Then
            If mMileStone.SplitFile Is Nothing Then
                Dim pValues As New NameValueCollection()
                Dim pJson As New JsonDictionary()
                pJson.Add("fID", vFileID)
                'pJson.Add("fID", "5huem7k48lhitnxr4kk6ypba6ozdwdbyzi332y4hxtyeyfm27vsxshdh1frsa3t3dwwrz8okkms")
                pValues.Add("json", pJson.ToString())
                Dim pResult As Byte() = Await pWebClient.UploadValuesTaskAsync(New Uri(pUrl), "POST", pValues)
                Dim pResultText = System.Text.Encoding.UTF8.GetString(pResult)
                pSplitJson = JsonConvert.DeserializeObject(Of MobizExportController.SplitFileJson)(pResultText)
                mMileStone.SplitFile = pSplitJson
                SaveMilestone()
            Else
                pSplitJson = mMileStone.SplitFile
            End If
            mStatusText = "Splitting sqlite database files before downloading"
            Dim pDestinationFolder = AppPath & "temp\DownloadedSplits\" & pSplitJson.FolderTitle & "\"
            If IO.Directory.Exists(pDestinationFolder) = False Then
                IO.Directory.CreateDirectory(pDestinationFolder)
            End If
            mMileStone.SplitFileFolder = pDestinationFolder
            Dim pDestination1stSplitName As String
            For pFileIndex As Integer = 1 To pSplitJson.Count
                pWebClient = New Net.WebClient
                pUrl = mMileStone.PublicServerUrl & "MobizExport/DownloadSplit?FolderTitle=" & pSplitJson.FolderTitle & "&Index=" & pFileIndex & "&Count=" & pSplitJson.Count
                pDestinationFilename = pDestinationFolder & pSplitJson.FolderTitle & "[" & Format(pFileIndex, StrDup(CStr(pSplitJson.Count).Length, "0")) & " of " & pSplitJson.Count & "].zip"
                If pFileIndex = 1 Then
                    pDestination1stSplitName = pDestinationFilename
                End If
                mStatusText = "Downloading splitted database file (" & pFileIndex & " out of " & pSplitJson.Count & ")"
                pWebClient.DownloadFile(New Uri(pUrl), pDestinationFilename)
                mMileStone.SplitFileDownloadedIndex = pFileIndex
                SaveMilestone()
            Next pFileIndex

            mStatusText = "Merging downloaded splitted database files"
            MergeFiles(pDestination1stSplitName, pDestinationFilename, pError)
            If String.IsNullOrWhiteSpace(pError) = False Then
                mMileStone.ErrorMessage = pError
                Return
            End If
            mMileStone.MergedFilename = pDestinationFilename
            SaveMilestone()
        End If

        ImportFromSqlite(pDestinationFilename)

    End Sub

    Private Function ImportFromSqlite(vZipFilename As String) As String

        Dim pError As String
        Dim pLastImport As Date?
        Dim pDestinationFolder As String

        pDestinationFolder = mMileStone.ExtractedFolder
        If String.IsNullOrWhiteSpace(pDestinationFolder) Then
            pDestinationFolder = AppPath & "temp\DownloadedSplits\" & IO.Path.GetFileNameWithoutExtension(vZipFilename) & "\"
            If IO.Directory.Exists(pDestinationFolder) = False Then
                IO.Directory.CreateDirectory(pDestinationFolder)
            End If
            Dim pProZip As New ProZip(vZipFilename)
            pProZip.Unzip(pDestinationFolder, pError)
            If String.IsNullOrWhiteSpace(pError) = False Then
                Return pError
            End If
            mMileStone.ExtractedFolder = pDestinationFolder
            mMileStone.PrivateOrPublic = "private"
            SaveMilestone()
        End If

        Dim pRecordsCount As Long
        Dim pTotalSavedRecords As Long
        Dim pImportFromSqlite As ImportFromSqlite

        Using pPrivateProData = ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:=mMileStone.PrivateDatabaseName,
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)

            Dim pBusiness = New Business(pPrivateProData)
            pLastImport = pBusiness.SystemDefaults.EcommerceDataLastImport

            pImportFromSqlite = New ImportFromSqlite()
            pImportFromSqlite.MinimumDate = pLastImport
            pImportFromSqlite.SourceSqliteFilename = pDestinationFolder & mMileStone.OrgID & ".private.db"
            pRecordsCount += pImportFromSqlite.CountSqliteAllRecords()

            pImportFromSqlite = New ImportFromSqlite()
            pImportFromSqlite.MinimumDate = pLastImport
            pImportFromSqlite.SourceSqliteFilename = pDestinationFolder & mMileStone.OrgID & ".public.db"
            pRecordsCount += pImportFromSqlite.CountSqliteAllRecords()

            If mMileStone.PrivateOrPublic <> "public" Then
                pImportFromSqlite = New ImportFromSqlite()
                'pImportFromSqlite.RecordsToImport = 10
                pImportFromSqlite.MinimumDate = pLastImport
                pImportFromSqlite.TotalSavedRecords = pTotalSavedRecords
                pImportFromSqlite.TotalSqliteRecords = pRecordsCount
                pImportFromSqlite.DestinationProData = pPrivateProData
                pImportFromSqlite.SourceSqliteFilename = pDestinationFolder & mMileStone.OrgID & ".private.db"
                pImportFromSqlite.StartingTable = mMileStone.TableName
                pImportFromSqlite.StartingRowIndex = mMileStone.RowIndex
                AddHandler pImportFromSqlite.Log,
                    Sub(e As ImportFromSqlite.LogArgs)
                        mStatusText = e.Text
                    End Sub
                AddHandler pImportFromSqlite.DataImported,
                    Sub(e As ImportFromSqlite.DataImportedArgs)
                        mMileStone.TableName = e.Table
                        mMileStone.RowIndex = e.RowIndex
                        SaveMilestone()
                    End Sub
                pImportFromSqlite.Import()
                pTotalSavedRecords = pImportFromSqlite.TotalSavedRecords
            End If

            Using pProData = ProData.CreateFromConnectionInfo(
                vServer:=Common.ConnectionInfo.Server,
                vDatabase:=Common.ConnectionInfo.Database,
                vUsername:=Common.ConnectionInfo.Username,
                vPassword:=Common.ConnectionInfo.Password,
                vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)

                pImportFromSqlite = New ImportFromSqlite()
                'pImportFromSqlite.RecordsToImport = 10
                pImportFromSqlite.MinimumDate = pLastImport
                pImportFromSqlite.TotalSavedRecords = pTotalSavedRecords
                pImportFromSqlite.TotalSqliteRecords = pRecordsCount
                pImportFromSqlite.DestinationProData = pProData
                pImportFromSqlite.SourceSqliteFilename = pDestinationFolder & mMileStone.OrgID & ".public.db"
                If mMileStone.PrivateOrPublic = "public" Then
                    pImportFromSqlite.StartingTable = mMileStone.TableName
                    pImportFromSqlite.StartingRowIndex = mMileStone.RowIndex
                Else
                    mMileStone.PrivateOrPublic = "public"
                    mMileStone.TableName = Nothing
                    mMileStone.RowIndex = 0
                    SaveMilestone()
                End If
                AddHandler pImportFromSqlite.Log,
                    Sub(e As ImportFromSqlite.LogArgs)
                        mStatusText = e.Text
                    End Sub
                AddHandler pImportFromSqlite.DataImported,
                    Sub(e As ImportFromSqlite.DataImportedArgs)
                        mMileStone.TableName = e.Table
                        mMileStone.RowIndex = e.RowIndex
                        SaveMilestone()
                    End Sub
                pImportFromSqlite.Import()
            End Using

            AddLog("Deleting duplicate product categories...")
            Dim pDuplicateCategories = pBusiness.LibraryStock.DeleteDuplicateProductCategories()
            AddLog("Deleted " & pDuplicateCategories & " duplicate product categories")

            Dim pLastRequest = pBusiness.SystemDefaults.EcommerceDataLastRequest
            If pLastRequest IsNot Nothing Then
                pBusiness.SystemDefaults.EcommerceDataLastImport = pLastRequest
            End If
        End Using ' pPrivateProData

        mMileStone.IsDone = True
        SaveMilestone()
        mIsBusy = False

    End Function

    Public Class Milestone
        Public PublicServerUrl As String
        Public OrgID As String
        Public PrivateDatabaseName As String

        Public FileID As String
        Public SplitFile As MobizExportController.SplitFileJson
        Public SplitFileFolder As String
        Public SplitFileDownloadedIndex As Integer
        Public MergedFilename As String
        Public ExtractedFolder As String
        Public PrivateOrPublic As String
        Public TableName As String
        Public RowIndex As Integer ' 0-based
        Public IsDone As Boolean
        Public ErrorMessage As String
    End Class

    Public Sub SaveMilestone(vPublicServerUrl As String,
        vOrgID As String,
        vPrivateDatabaseName As String)

        mMileStone = New Milestone()
        mMileStone.PublicServerUrl = vPublicServerUrl
        mMileStone.OrgID = vOrgID
        mMileStone.PrivateDatabaseName = vPrivateDatabaseName
        SaveMilestone()

    End Sub

    Private Sub SaveMilestone()

        Dim pText = JsonConvert.SerializeObject(mMileStone, Formatting.Indented)
        FileText(MilestoneFilename()) = pText

    End Sub

    Private Sub LoadMileStone()

        Try
            Dim pText = FileText(MilestoneFilename())
            mMileStone = JsonConvert.DeserializeObject(Of Milestone)(pText)
        Catch ex As Exception
        End Try

    End Sub

    Private Function MilestoneFilename() As String

        Return AppPath & "temp\ECommerceDataSyncMilestone.txt"

    End Function

    Public Function CompareSqlServerData() As String

        Dim pOut As String
        Dim pServerProData As ProData

        Dim pCI As New ConnectionInfo
        pCI.Filename = AppPath & "settings\EcommerceServerSqlServer.data"
        If pCI.Open() Then
            pServerProData = ProData.CreateFromConnectionInfo(
                vServer:=pCI.Server,
                vDatabase:=pCI.Database,
                vUsername:=pCI.Username,
                vPassword:=pCI.Password,
                vIntegratedSecurity:=pCI.IntegratedSecurity)
        Else
            pOut = "Cant open " & pCI.Filename & vbCrLf &
                "Server ""data for comparison"" will be acquired via HTTP." & vbCrLf &
                ". ErrorMessage: " & pCI.ErrorMessage
            AddLog(pOut)
        End If

        If mMileStone Is Nothing Then
            LoadMileStone()
            If mMileStone Is Nothing Then
                pOut = "Milestone is not set. Please run ecommerce data sync first."
                AddLog(pOut)
                Return pOut
            End If
        End If

        If String.IsNullOrWhiteSpace(mMileStone.OrgID) Then
            pOut = "Milestone org ID is not set. Please run ecommerce data sync first."
            AddLog(pOut)
            Return pOut
        End If

        Using pServerProData
            Using pLocalProData = ProData.CreateFromConnectionInfo(
                vServer:=Common.ConnectionInfo.Server,
                vDatabase:=Common.ConnectionInfo.Database,
                vUsername:=Common.ConnectionInfo.Username,
                vPassword:=Common.ConnectionInfo.Password,
                vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)

                pOut &= CompareSqlServerProductsData(pServerProData, pLocalProData)
            End Using
        End Using

        Return pOut

    End Function

    Public Class RequestDataForComparisonResult
        Public DataForComparisonKey As String
        Public Result As String
    End Class

    Private Function RequestServerDataXml() As DataSet

        Dim pWebClient As New WebClient
        Dim pUrl = mMileStone.PublicServerUrl & "ECommerceDataSync/RequestDataForComparison"
        Dim pValues = New NameValueCollection()
        Dim pJson As New JsonDictionary()
        pJson.Add("OrgID", mMileStone.OrgID)
        pValues.Add("json", pJson.ToString())

        Do
            Dim pResult As Byte() = pWebClient.UploadValues(New Uri(pUrl), "POST", pValues)
            Dim pResultText = System.Text.Encoding.UTF8.GetString(pResult)
            Try
                Dim pResultJson = JsonConvert.DeserializeObject(Of RequestDataForComparisonResult)(pResultText)
                If String.IsNullOrWhiteSpace(pResultJson.DataForComparisonKey) Then
                    AddLog("GetServerDataXml: " & pResultJson.Result)
                    Threading.Thread.Sleep(5000)
                Else
                    Return DownloadServerDataXml()
                End If
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try
        Loop

    End Function

    Private Function DownloadServerDataXml() As DataSet

        Dim pWebClient As New WebClient
        Dim pUrl = mMileStone.PublicServerUrl & "ECommerceDataSync/DownloadDataForComparison"
        Dim pFilename = Common.ServerPath & "temp\ECommerceDataSyncDataForComparison\ServerData-" & DateTimeStamp()
        pWebClient.DownloadFile(New Uri(pUrl), pFilename & ".zip")

        Dim pZip As New ProZip()
        pZip.Open(pFilename & ".zip")
        pZip.UnzipFile("DataSet.xml", pFilename & ".xml")

        Dim pDS As New DataSet
        pDS.ReadXml(pFilename & ".xml")

        Return pDS

    End Function

    Public Shared Function GetDataForComparison(vProData As ProData, vOrgID As String) As DataSet

        vProData.CommandTimeout = 60 * 5
        Dim pDS As New DataSet
        Dim pSql = " -- products checksum" & vbCrLf &
            " SELECT MobizLibraryPIMProductPriceLevel.Price" & vbCrLf &
            " ,MobizLibraryPIMProductPriceLevel.ProductID" & vbCrLf &
            " ,MobizLibraryPIMProductPriceLevel.PriceLevelID" & vbCrLf &
            " ,CONVERT(VARCHAR(36), MobizLibraryPIMProductPriceLevel.ProductID) + '/' + CONVERT(VARCHAR(36), MobizLibraryPIMProductPriceLevel.PriceLevelID) AS ID" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductName" & vbCrLf &
            " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
            " ,MobizLibraryPIMProduct.LastUpdated" & vbCrLf &
            " FROM MobizLibraryPIMProductPriceLevel" & vbCrLf &
            " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = MobizLibraryPIMProductPriceLevel.ProductID" & vbCrLf &
            " WHERE MobizLibraryPIMProductPriceLevel.OrganizationID = " & ReadyString(vOrgID) & vbCrLf &
            " ORDER BY MobizLibraryPIMProduct.ProductName"
        Dim pDT = vProData.Execute(pSql)
        pDT.TableName = "products"
        pDS.Tables.Add(pDT)

        Return pDS

    End Function

    Private Function CompareSqlServerProductsData(vServerProData As ProData, vLocalProData As ProData) As String

        AddLog("Comparing products")

        Dim pDtServer As DataTable
        Dim pDtLocal As DataTable

        If pDtServer Is Nothing Then
            AddLog("Get product prices from server SQL Server")
            Dim pDsServer As DataSet
            If vServerProData Is Nothing Then
                pDsServer = RequestServerDataXml()
            Else
                pDsServer = GetDataForComparison(vServerProData, mMileStone.OrgID)
            End If
            pDtServer = pDsServer.Tables("products")

            AddLog("Product prices from server SQL Server contains " & Format(pDtServer.Rows.Count, cGlobal.FormatInteger) & " records.")

            AddLog("Get product prices from local SQL Server")
            Dim pDsLocal = GetDataForComparison(vLocalProData, mMileStone.OrgID)
            pDtLocal = pDsLocal.Tables("products")
            AddLog("Product prices from local SQL Server contains " & Format(pDtLocal.Rows.Count, cGlobal.FormatInteger) & " records.")

            SaveProductXml(pDtServer, 1)
            SaveProductXml(pDtLocal, 2)
        End If

        Dim pMissing As Integer
        Dim pDifferent As Integer
        Dim pSame As Integer
        Dim pTotal As Integer
        Dim pLastTotal As Integer
        Dim pLogDate As Date = Now
        Dim pDvLocal = New DataView(pDtLocal, "", "ID", DataViewRowState.CurrentRows)
        Dim pEarliestDiscrepancy = Now

        For Each pDrServer As DataRow In pDtServer.Rows
            Dim pSeconds = (Now - pLogDate).TotalSeconds
            If pSeconds >= 10 Then
                pLogDate = Now

                AddLog("Progress " & Format(pTotal / pDtServer.Rows.Count * 100, "0.00") & "%" &
                    "(" & Format(pTotal, cGlobal.FormatInteger) & " out of " & Format(pDtServer.Rows.Count, cGlobal.FormatInteger) & ")" &
                    "; Missing products: " & pMissing &
                    "; Different products: " & pDifferent &
                    "; Same products: " & pSame &
                    "; " & Format((pTotal - pLastTotal) / pSeconds, "0.00") & " comparisons per second")
            End If
            pTotal += 1

#If DEBUG Then
            If ReadyGuid(pDrServer("ProductID")) = ReadyGuid("45c3834d-0933-4f09-967a-6f92b8f1dba8") Then
                Debug.WriteLine("45c3834d-0933-4f09-967a-6f92b8f1dba8")
            End If
#End If

            Dim pDrLocalIndex = pDvLocal.Find(pDrServer("ID"))

            If pDrLocalIndex = -1 Then
                AddLog("[WARNING] Product ID:" & CStr2(pDrServer("ProductID")) & _
                       "; Product Number:" & CStr2(pDrServer("ProductNo")) & _
                       "; Product Name: " & CStr2(pDrServer("ProductName")) & _
                       "; Price: " & CStr2(pDrServer("Price")) & _
                       "; Last updated: " & CStr2(pDrServer("LastUpdated")) & _
                       " does not exist in this local server.")
                If pDrServer("LastUpdated") < pEarliestDiscrepancy Then
                    pEarliestDiscrepancy = pDrServer("LastUpdated")
                End If
                pMissing += 1
                Continue For
            End If

            Dim pDrvLocal = pDvLocal(pDrLocalIndex)
            If CDec2(pDrServer("Price")) <> CDec2(pDrvLocal("Price")) Then
                AddLog("[WARNING] Product ID:" & CStr2(pDrServer("ProductID")) & _
                    "; Product Number:" & CStr2(pDrServer("ProductNo")) & _
                    "; Product Name: " & CStr2(pDrServer("ProductName")) & _
                    "; Server Price: " & CStr2(pDrServer("Price")) & _
                    "; Local Price: " & CStr2(pDrvLocal("Price")) & _
                    "; Server last updated: " & CStr2(pDrServer("LastUpdated")) & _
                    "; Local last updated: " & CStr2(pDrvLocal("LastUpdated")) & _
                    " has different price.")
                If pDrServer("LastUpdated") < pEarliestDiscrepancy Then
                    pEarliestDiscrepancy = pDrServer("LastUpdated")
                End If
                pDifferent += 1
                Continue For
            End If
            pSame += 1
        Next

        Dim pOut = "Missing products: " & pMissing &
            "; Different products: " & pDifferent &
            "; Same products: " & pSame &
            "; Earliest discrepancy: " & pEarliestDiscrepancy
        AddLog(pOut)

        Return pOut

    End Function

    Private Function OpenProductXml(vIndex As Integer) As DataTable

        Try
            Dim pFilename = AppPath & "temp\logs\EcommerceDataSync\products" & vIndex & ".xml"
            Dim pDT As New DataTable
            pDT.ReadXml(pFilename)
            AddLog("Product XML successfully opened from " & pFilename)
            Return pDT
        Catch ex As Exception

        End Try

    End Function

    Private Sub SaveProductXml(vDT As DataTable, vIndex As Integer)

        Dim pFilename = AppPath & "temp\logs\EcommerceDataSync\products" & vIndex & ".xml"
        vDT.WriteXml(pFilename, XmlWriteMode.WriteSchema)

    End Sub

    Private Sub AddLog(vText As String)

        AppendToLog(vText, AppPath & "temp\logs\EcommerceDataSync", LogFileTitle())

    End Sub

    Private Function LogFileTitle() As String

        If String.IsNullOrWhiteSpace(mLogFileTitle) = False Then
            Return mLogFileTitle
        End If

        mLogFileTitle = "EcommerceDataSync_" & DateTimeStamp()

        Return mLogFileTitle

    End Function
End Class
