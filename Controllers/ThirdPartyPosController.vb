Imports System.IO
Imports System.Net
Imports System.Threading
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class ThirdPartyPosController
    Inherits System.Web.Mvc.Controller

    Shared ReadOnly mSemaphore As New SemaphoreSlim(1, 1)
    Shared mDateStarted As Date
    Shared mStatus As String

    Public Function Status() As String

        If String.IsNullOrWhiteSpace(mStatus) Then
            Return "no status yet. vbZKL5S2LFBB!"
        End If
        Return mStatus

    End Function

    Public Function DailyRun() As String

        Try
            If mSemaphore.Wait(2000) = False Then
                mStatus = "Another process is running since " & mDateStarted & ". vb6ESPZKPGMG!"
                Return mStatus
            End If

            mDateStarted = Now
            mStatus = "Started: " + mDateStarted

            Dim pBaseUrl = Common.VirtualPath & ""
            Task.Run(
                Async Function() As Task
                    Try
                        Await StartInternal()
                        FlushZipConvertInternal(pBaseUrl)
                        UploadResults()
                        CallImportOrganization()
                        mStatus = "Ended: " + DateTime.Now & "<br />" & mStatus
                    Catch ex As Exception
                        mStatus = "Started: " + mDateStarted & "<br />" &
                            "Current time: " + DateTime.Now & "<br />" &
                            "Error in task: " + ex.Message.Replace(vbCrLf, "<br />") & "<br />" &
                            GetStackTrace(, ex).Replace(vbCrLf, "<br />")
                    Finally
                        mSemaphore.Release()
                    End Try
                End Function)
            Return mStatus
        Catch ex As Exception
            mStatus = "Started: " + mDateStarted & "<br />" &
                "Current time: " + DateTime.Now & "<br />" &
                "Error: " + ex.Message.Replace(vbCrLf, "<br />") & "<br />" &
                GetStackTrace(, ex).Replace(vbCrLf, "<br />")
            mSemaphore.Release()
            Return mStatus
        End Try

    End Function

    Public Function Start() As String
        ' for azure

        Try
            If mSemaphore.Wait(2000) = False Then
                mStatus = "Another process is running since " & mDateStarted & ". vbZF2Y9Y2624!"
                Return mStatus
            End If

            mDateStarted = Now
            mStatus = "Started: " + mDateStarted

            Task.Run(
                Async Function() As Task
                    Try
                        Await StartInternal()
                        mStatus = "Ended: " + DateTime.Now & "<br />" & mStatus
                    Catch ex As Exception
                        mStatus = "Started: " + mDateStarted & "<br />" &
                            "Current time: " + DateTime.Now & "<br />" &
                            "Error in task: " + ex.Message.Replace(vbCrLf, "<br />") & "<br />" &
                            GetStackTrace(, ex).Replace(vbCrLf, "<br />")
                    Finally
                        mSemaphore.Release()
                    End Try
                End Function)
            Return mStatus
        Catch ex As Exception
            mStatus = "Started: " + mDateStarted & "<br />" &
                "Current time: " + DateTime.Now & "<br />" &
                "Error: " + ex.Message.Replace(vbCrLf, "<br />") & "<br />" &
                GetStackTrace(, ex).Replace(vbCrLf, "<br />")
            mSemaphore.Release()
            Return mStatus
        End Try

    End Function

    Private Async Function StartInternal() As Task
        ThirdPartyPos.Status =
            Sub(vText As String)
                mStatus = DateTime.Now & " " & vText
            End Sub
        Await ThirdPartyPos.Execute()
    End Function

    Public Function GetZipFilenames() As String

        Return ThirdPartyPos.GetZipFilenames()

    End Function

    Public Function DownloadZipFile() As ActionResult

#If DEBUG Then
        'Thread.Sleep(10000)
#End If
        Dim pFilename = WebRequest.Instance.QueryString("filename")
        pFilename = HttpUtility.UrlDecode(pFilename)
        pFilename = IO.Path.Combine(ThirdPartyPos.GetThirdPartyPosFtpFolder(), pFilename)
        If IO.File.Exists(pFilename) = False Then
            Return Content("file does not exist, vb8GRQL2LZEG!")
        End If

        Return File(pFilename, "application/zip", "file.zip")

    End Function

    Public Class FlushZipConverterFolderInfoClass
        Public OrgID As String
        Public DeviceID As String
    End Class

    Public Function FlushZipConvert() As String

        Try
            If mSemaphore.Wait(2000) = False Then
                mStatus = "Another process is running since " & mDateStarted & ". vbZF2Y9Y2624!"
                Return mStatus
            End If

            mDateStarted = Now
            mStatus = "Started: " + mDateStarted

            Dim pBaseUrl = Common.VirtualPath & ""
            Task.Run(
                Async Function() As Task
                    Try
                        FlushZipConvertInternal(pBaseUrl)
                        mStatus = "Ended: " + DateTime.Now & "<br />" & mStatus
                    Catch ex As Exception
                        mStatus = "Started: " + mDateStarted & "<br />" &
                            "Current time: " + DateTime.Now & "<br />" &
                            "Error in task: " + ex.Message.Replace(vbCrLf, "<br />") & "<br />" &
                            GetStackTrace(, ex).Replace(vbCrLf, "<br />")
                    Finally
                        mSemaphore.Release()
                    End Try
                End Function)
            Return mStatus
        Catch ex As Exception
            mStatus = "Started: " + mDateStarted & "<br />" &
                "Current time: " + DateTime.Now & "<br />" &
                "Error: " + ex.Message.Replace(vbCrLf, "<br />") & "<br />" &
                GetStackTrace(, ex).Replace(vbCrLf, "<br />")
            mSemaphore.Release()
            Return mStatus
        End Try

    End Function

    Private Sub FlushZipConvertInternal(vBaseUrl As String)

        Dim pFolder = IO.Path.Combine(Common.ServerPath, "temp\ThirdPartyPosFtpUploads")
        If IO.Directory.Exists(pFolder) = False Then
            mStatus = "Folder, " & pFolder & ", does not exist. vbDQQESWV9C6!"
            Return
        End If

        ' BEGIN uncomment para auto-zip csv files during debugging
        'Dim pZipFilename = IO.Path.Combine(pFolder, "convertibles.zip")
        'Dim pZip As New ProZip(pZipFilename)
        'Dim pPatterns = New List(Of String) ' regex pattern
        'pPatterns.Add(".*\.csv") ' should be lowercased
        'pPatterns.Add("info\.json") ' should be lowercased
        'Dim pFiles = Directory.EnumerateFiles(pFolder).Where(
        '    Function(vFilename As String)
        '        Dim pBaseFilename = Path.GetFileName(vFilename)
        '        pBaseFilename = pBaseFilename.ToLower()
        '        For Each pPattern In pPatterns
        '            Dim pMatched = Regex.IsMatch(pBaseFilename, pPattern)
        '            If pMatched Then
        '                Return True
        '            End If
        '        Next
        '    End Function)
        'For Each pFilename In pFiles
        '    pZip.AddFile(pFilename)
        'Next
        ' END uncomment para auto-zip csv files during debugging

        Dim pError As String
        Dim pFiles As IEnumerable(Of String)

        ' BEGIN CyberPOS
        If Directory.Exists(pFolder & "\CyberPOS") Then
            pFiles = Directory.EnumerateFiles(pFolder & "\CyberPOS", "*.csv", SearchOption.AllDirectories)
            For Each pFile In pFiles
                Dim pZipFilename = Path.ChangeExtension(pFile, "zip")
                If IO.File.Exists(pZipFilename) Then
                    Continue For
                End If
                Dim pResultFilename = Path.Combine(Path.GetDirectoryName(pFile), Path.GetFileNameWithoutExtension(pFile) & "_result.txt")
                If IO.File.Exists(pResultFilename) Then
                    Continue For
                End If
                Dim pRelativeFilename = GetRelativePath(pFolder & "\CyberPOS", pFile)
                pRelativeFilename = Trim2(pRelativeFilename, "\")
                If pRelativeFilename.Count(Function(x) x = "\") = 0 Then
                    ' the csv file is directly inside the CyberPOS folder. ignore this file
                    Continue For
                End If
                Dim pStoreName = pRelativeFilename.Substring(0, pRelativeFilename.IndexOf("\"))
                Dim pCyberPosFlushZipConverter = New CyberPosFlushZipConverter()
                pCyberPosFlushZipConverter.Execute(pFile, pStoreName, pError)
                If String.IsNullOrWhiteSpace(pError) = False Then
                    FileText(pResultFilename) = pError
                End If
            Next
        End If
        ' END CyberPOS

        ' BEGIN Condor
        If Directory.Exists(pFolder & "\Condor") Then
            pFiles = Directory.EnumerateFiles(pFolder & "\Condor", "*.zip", SearchOption.AllDirectories)
            For Each pFile In pFiles
                If pFile.EndsWith(".mobiz.zip", StringComparison.InvariantCultureIgnoreCase) Then
                    Continue For
                End If
                Dim pZipFilename = Path.ChangeExtension(pFile, "mobiz.zip")
                If IO.File.Exists(pZipFilename) Then
                    Continue For
                End If
                Dim pResultFilename = Path.Combine(Path.GetDirectoryName(pFile), Path.GetFileNameWithoutExtension(pFile) & "_result.txt")
                If IO.File.Exists(pResultFilename) Then
                    Continue For
                End If

                Dim pCondorFlushZipConverter = New CondorFlushZipConverter()
                pCondorFlushZipConverter.Execute(pFile, pError)
                If String.IsNullOrWhiteSpace(pError) = False Then
                    FileText(pResultFilename) = pError
                End If
            Next
        End If
        ' END Condor

        pFiles = Directory.EnumerateFiles(pFolder, "*.zip", SearchOption.AllDirectories)
        For Each pFile In pFiles
            If Path.GetFileName(pFile).StartsWith("flushable_") Then
                Continue For
            End If
            Dim pResultFilename = Path.Combine(Path.GetDirectoryName(pFile), Path.GetFileNameWithoutExtension(pFile) & "_result.txt")
            If IO.File.Exists(pResultFilename) Then
                Continue For
            End If

            Dim pVendor = GetRelativePath(pFolder, pFile)
            pVendor = Trim2(pVendor, "\")
            Dim p1stIndex = pVendor.IndexOf("\")
            pVendor = pVendor.Substring(0, p1stIndex)
            If String.Equals(pVendor, "Condor", StringComparison.InvariantCultureIgnoreCase) Then
                If pFile.EndsWith(".mobiz.zip", StringComparison.InvariantCultureIgnoreCase) = False Then
                    Continue For
                End If
            End If

            Try
                mStatus = "FlushZipConverter: " & pFile
                Using pFzc As New FlushZipConverter()
                    pFzc.BaseUrl = vBaseUrl
                    pFzc.Process(pFile, pError)
                End Using

                If String.IsNullOrWhiteSpace(pError) Then
                    FileText(pResultFilename) = "success"
                Else
                    FileText(pResultFilename) = pError
                End If
            Catch ex As Exception
                Dim pErrorFilename = Path.Combine(Path.GetDirectoryName(pFile), Path.GetFileNameWithoutExtension(pFile) & "_error.txt")
                mStatus = GetStackTrace(, ex)
                FileText(pErrorFilename) = mStatus
                mStatus = mStatus.Replace(vbCrLf, "<br />")
            End Try
        Next

    End Sub

    Public Function FlushZipConverterSummary() As ActionResult

        Try
            If mSemaphore.Wait(2000) = False Then
                mStatus = "Another process is running since " & mDateStarted & ". vbZF2Y9Y2624!"
                Return Content(mStatus)
            End If

            mDateStarted = Now
            mStatus = "Started: " + mDateStarted

            Dim pFolder = IO.Path.Combine(Common.ServerPath, "temp\ThirdPartyPosFtpUploads")

            Dim pDT = New DataTable()
            pDT.Columns.Add("Vendor")
            pDT.Columns.Add("Filename")
            pDT.Columns.Add("Result")
            pDT.Columns.Add("ResultDate", GetType(Date))
            pDT.Columns.Add("Error")

            FolderSummary(pFolder, "_result.txt", "Result", pDT)
            FolderSummary(pFolder, "_error.txt", "Error", pDT)
            FolderSummary(pFolder, "_flush.txt", Nothing, pDT)

            Dim pExcelFilename = IO.Path.Combine(Common.ServerPath, "temp\ThirdPartyPosFtpUploads\Summaries", "ThirdPartyVendorSummary_" & DateTimeStamp(False) & ".xlsx")
            CreateFolder(pExcelFilename)
            TableToExcelFile(pDT, "", "ResultDate", pExcelFilename)
            Return File(pExcelFilename, "application/excel", System.IO.Path.GetFileName(pExcelFilename))
        Catch ex As Exception
            mStatus = "Started: " + mDateStarted & "<br />" &
                "Current time: " + DateTime.Now & "<br />" &
                "Error: " + ex.Message.Replace(vbCrLf, "<br />") & "<br />" &
                GetStackTrace(, ex).Replace(vbCrLf, "<br />")
            Return Content(mStatus)
        Finally
            mSemaphore.Release()
        End Try

    End Function

    Public Function FlushZipConverterGroupedSummary() As ActionResult

        Try
            If mSemaphore.Wait(2000) = False Then
                mStatus = "Another process is running since " & mDateStarted & ". vbP9C5FVF6JW!"
                Return Content(mStatus)
            End If

            mDateStarted = Now
            mStatus = "Started: " + mDateStarted

            Dim pFolder = IO.Path.Combine(Common.ServerPath, "temp\ThirdPartyPosFtpUploads\Summaries")
            Dim pFiles = IO.Directory.GetFiles(pFolder, "ThirdPartyVendorSummary_*.xlsx")
            If pFiles.Count = 0 Then
                Return Content("No summary file. Browse /FlushZipConverterSummary first.")
            End If
            Dim pSortedFiles = pFiles.OrderByDescending(Function(x) x)
            Dim pError As String
            Dim pDT = ExcelToTable(pSortedFiles.First(), {"Result", "OrganizationName", "FowstName", "StoreName", "OrganizationID", "FowstID", "DeviceID"}, pError)
            If String.IsNullOrWhiteSpace(pError) = False Then
                Return Content(pError)
            End If
            pDT.DefaultView.RowFilter = "Result = 'success'"
            pDT.DefaultView.Sort = "FowstName"
            pDT = pDT.DefaultView.ToTable(True, "OrganizationName", "FowstName", "StoreName", "OrganizationID", "FowstID", "DeviceID")

            Dim pExcelFilename = IO.Path.Combine(Common.ServerPath, "temp\ThirdPartyPosFtpUploads\GroupsSummaries", "ThirdPartyVendorGroupedSummary_" & DateTimeStamp(False) & ".xlsx")
            CreateFolder(pExcelFilename)
            TableToExcelFile(pDT, "", "", pExcelFilename)
            Return File(pExcelFilename, "application/excel", System.IO.Path.GetFileName(pExcelFilename))
        Catch ex As Exception
            mStatus = "Started: " + mDateStarted & "<br />" &
                "Current time: " + DateTime.Now & "<br />" &
                "Error: " + ex.Message.Replace(vbCrLf, "<br />") & "<br />" &
                GetStackTrace(, ex).Replace(vbCrLf, "<br />")
            Return Content(mStatus)
        Finally
            mSemaphore.Release()
        End Try

    End Function

    Sub FolderSummary(vFolder As String, vSuffix As String, vFieldName As String, ByRef oDT As DataTable)

        If vSuffix = "_flush.txt" Then
            oDT.Columns.Add("OrganizationName")
            oDT.Columns.Add("FowstName")
            oDT.Columns.Add("StoreName")
            oDT.Columns.Add("MinInvoiceNumber", GetType(Integer))
            oDT.Columns.Add("MaxInvoiceNumber", GetType(Integer))
            oDT.Columns.Add("MinInvoiceDate", GetType(Date))
            oDT.Columns.Add("MaxInvoiceDate", GetType(Date))
            oDT.Columns.Add("InvoiceTotalAmount", GetType(Integer))
            oDT.Columns.Add("InvoiceCount", GetType(Integer))
            oDT.Columns.Add("SkippedInvoiceNumbers", GetType(Integer))
            oDT.Columns.Add("OrganizationID")
            oDT.Columns.Add("FowstID")
            oDT.Columns.Add("DeviceID")
            oDT.Columns.Add("FlushFilename")
        End If

        Dim pDR As DataRow
        Dim pFiles = Directory.EnumerateFiles(vFolder, "*" & vSuffix, SearchOption.AllDirectories)
        For Each pFile In pFiles
            If pFile.ToLower().Contains("SM_CALAMBA".ToLower()) Then
                Debug.WriteLine("")
            End If
            Dim pResultDate = System.IO.File.GetLastWriteTime(pFile)
            Dim pAgeInDays = Now - pResultDate
            If Math.Abs(pAgeInDays.TotalDays) > 30 Then
                Continue For
            End If
            Dim pVendor = GetRelativePath(vFolder, pFile)
            pVendor = Trim2(pVendor, "\")
            Dim p1stIndex = pVendor.IndexOf("\")
            Dim pRelativeFilename = pVendor.Substring(p1stIndex + 1, pVendor.Length - p1stIndex - 1)
            pVendor = pVendor.Substring(0, p1stIndex)

            If pRelativeFilename.Contains("ZIP_20250427") Then
                Debug.WriteLine("")
            End If
            Dim pRelativeFilenameWitZipSuffix = pRelativeFilename.Substring(0, pRelativeFilename.Length - vSuffix.Length) & ".zip"
            Dim pExistingRows = oDT.Select("Vendor = " & ReadyString(pVendor) &
                " AND Filename = " & ReadyString(pRelativeFilenameWitZipSuffix), "ResultDate DESC")
            If pExistingRows.Length > 0 Then
                pDR = pExistingRows(0)
            Else
                pDR = oDT.NewRow
                pDR("Vendor") = pVendor
            End If

            Dim pExistingRowDate = CDateNull(pDR("ResultDate"))
            If pExistingRowDate.HasValue = False Then
                pExistingRowDate = New Date(0)
            End If
            If pResultDate < pExistingRowDate Then
                pResultDate = pExistingRowDate
            End If

            If vSuffix = "_flush.txt" Then
                Dim pFileText = GetFileText(pFile, 6925793)
                Try
                    Dim pSummary = JsonConvert.DeserializeObject(Of FlushZipConverter.Summary)(pFileText)
                    pDR("OrganizationName") = pSummary.OrganizationName
                    pDR("OrganizationID") = pSummary.OrganizationID
                    pDR("FowstName") = pSummary.FowstName
                    pDR("FowstID") = pSummary.FowstID
                    pDR("StoreName") = pSummary.StoreName
                    pDR("MinInvoiceNumber") = If(pSummary.MinInvoiceNumber, DBNull.Value)
                    pDR("MaxInvoiceNumber") = If(pSummary.MaxInvoiceNumber, DBNull.Value)
                    pDR("MinInvoiceDate") = If(pSummary.MinInvoiceDate, DBNull.Value)
                    pDR("MaxInvoiceDate") = If(pSummary.MaxInvoiceDate, DBNull.Value)
                    pDR("InvoiceTotalAmount") = pSummary.InvoiceTotalAmount
                    pDR("InvoiceCount") = pSummary.InvoiceCount
                    pDR("SkippedInvoiceNumbers") = pSummary.SkippedInvoiceNumbers
                    pDR("DeviceID") = pSummary.DeviceID
                    pDR("FlushFilename") = pSummary.FlushFilename
                Catch ex As Exception
                    Throw New ApplicationException(ex.Message & vbCrLf &
                        "Text=" & pFileText, ex)
                End Try
            Else
                Dim pFileText = GetFileText(pFile, 5000)
                pDR(vFieldName) = pFileText
            End If
            pDR("ResultDate") = pResultDate
            pDR("Filename") = pRelativeFilenameWitZipSuffix
            If pExistingRows.Length = 0 Then
                oDT.Rows.Add(pDR)
            End If
        Next

    End Sub

    Public Function UploadResults() As String
        ' upload *_result.txt files from azure to ionos

        Try
            Return ThirdPartyPos.UploadResults().Replace(vbCrLf, "<br />")
        Catch ex As Exception
            mStatus = "Started: " + mDateStarted & "<br />" &
                "Current time: " + DateTime.Now & "<br />" &
                "Error: " + ex.Message.Replace(vbCrLf, "<br />") & "<br />" &
                GetStackTrace(, ex).Replace(vbCrLf, "<br />")
            Return mStatus
        End Try

    End Function

    Public Function GetResultsFilenames() As String
        'for ionos
        Try
            Return ThirdPartyPos.GetResultsFilenames(ThirdPartyPos.GetThirdPartyPosFtpFolder())

        Catch ex As Exception
            Return "Started: " + mDateStarted & "<br />" &
                "Current time: " + DateTime.Now & "<br />" &
                "Error: " + ex.Message.Replace(vbCrLf, "<br />") & "<br />" &
                GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        End Try

    End Function

    Public Function UploadResultFile() As String
        ' for ionos

        Try
            Dim pRelativeFilename = Request.Form("RelativeFilename")
            ThirdPartyPos.SaveUploadedResultFile(Request.Files(0), pRelativeFilename)
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

    End Function

    Function FlushThirdParty() As String

        Dim pDatabase As String

        Try
            If mSemaphore.Wait(2000) = False Then
                mStatus = "Another process is running since " & mDateStarted & ". vbZF2Y9Y2624!"
                Return mStatus
            End If
            mDateStarted = Now
            mStatus = "Started: " + mDateStarted
            Dim pOut As String = ""
            Dim pDtOrgs As DataTable
            Dim pFolders As String = ""
            Using pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                Dim pBusiness = New Business(pProData)
                pDtOrgs = pBusiness.LibraryOrganization.GetAll(True)
                For Each pDrOrg As DataRow In pDtOrgs.Rows
                    pDatabase = pDrOrg("Database")
                    Dim pSql = "USE " & EscapeSqlName(pDatabase)
                    pProData.ExecuteSQL(pSql)

                    Dim pDrDevice = pBusiness.LibraryDevice.ByUuid(FlushZipConverter.DeviceUuid)
                    If pDrDevice Is Nothing Then
                        Continue For
                    End If

                    Dim pFolder = Common.ServerPath & "temp\Uploads\" & ReadyGuid(pDrOrg("ID")) & "\" & ReadyGuid(pDrDevice("ID")) & "\"
                    If IO.Directory.Exists(pFolder) = False Then
                        Continue For
                    End If
                    If pFolders <> "" Then
                        pFolders &= "<br />" & vbCrLf
                    End If
                    pFolders &= pFolder
                    Dim pFiles = IO.Directory.GetFiles(pFolder, "*.zip")
                    For Each pFile In pFiles
                        Dim pFileNameWithoutExtension = IO.Path.GetFileNameWithoutExtension(pFile)
                        If pFileNameWithoutExtension.Length <> 25 Then
                            Continue For
                        End If
                        Dim pResultFilename = Path.Combine(Path.GetDirectoryName(pFile), Path.GetFileNameWithoutExtension(pFile) & "_result.txt")
                        Dim pResultFileDate As Date?
                        If IsResultFileOk(pResultFilename, pResultFileDate) Then
                            Continue For
                        End If

                        pSql = "SELECT ProcessStart" & vbCrLf &
                            ",ProcessEnd" & vbCrLf &
                            " FROM TransImports" & vbCrLf &
                            " WHERE Filename = " & ReadyString(pFileNameWithoutExtension) & vbCrLf
                        Dim pDrImport = pProData.ExecuteRow(pSql)
                        If pDrImport Is Nothing _
                            OrElse pDrImport("ProcessEnd") IsNot DBNull.Value Then

                            If pDrImport IsNot Nothing Then
                                pSql = "DELETE FROM TransImports" & vbCrLf &
                                    " WHERE Filename = " & ReadyString(pFileNameWithoutExtension) & vbCrLf
                                pSql = pProData.ExecuteSQL(pSql) ' output is used for debugging only
                            End If

                            pBusiness.TransFlushingReport.SaveTransImport(vDeviceID:=CStr2(pDrDevice("ID")),
                                vFilename:=pFileNameWithoutExtension,
                                vFullFilename:=pFile,
                                vDontDeleteUnprocessedTransImport:=True)
                        End If

                        Call New WebClient().DownloadStringTaskAsync(Common.VirtualPath & "/OfflineData/CallImportOrganization?OrgID=" & CStr2(pDrOrg("ID")))
                        If pOut <> "" Then
                            pOut &= "<br />" & vbCrLf
                        End If
                        pOut &= pFile
                    Next
                Next
            End Using

            If pOut = "" Then
                pOut = DateTimeStamp() & " - No unflushed zip file found in the following folders. <br />" & vbCrLf & pFolders
            End If
            Return pOut
        Catch ex As Exception
            Return "Current time: " + DateTime.Now & "<br />" & vbCrLf &
                "Database: " & pDatabase & "<br />" & vbCrLf &
                "Error: " + ex.Message.Replace(vbCrLf, "<br />") & "<br />" & vbCrLf &
                GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        Finally
            mSemaphore.Release()
        End Try

    End Function

    Private Class ResultClass
        Public d As dClass
        Public Class dClass
            Public Result As String
        End Class
    End Class

    Private Shared Function IsResultFileOk(vFilename As String, ByRef oFileDate As Date?) As Boolean

        If System.IO.File.Exists(vFilename) = False Then
            Return False
        End If

        oFileDate = New System.IO.FileInfo(vFilename).LastWriteTime
        Dim pFileText = FileText(vFilename)
        Dim pResult = New JavaScriptSerializer().Deserialize(Of ResultClass)(pFileText)
        If pResult?.d?.Result = "ok" Then
            Return True
        End If

    End Function

    Function Unflushed() As String

        Dim pDatabase As String

        Try
            Dim pIncludeAllDevices = CBool2(WebRequest.Instance.Data("IncludeAllDevices"))
            Dim pDtAll As DataTable
            Dim pDtOrgs As DataTable
            Using pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                Dim pBusiness = New Business(pProData)
                pDtOrgs = pBusiness.LibraryOrganization.GetAll(True)
                For Each pDrOrg As DataRow In pDtOrgs.Rows
                    pDatabase = pDrOrg("Database")
                    Dim pSql = "USE " & EscapeSqlName(pDatabase)
                    pProData.ExecuteSQL(pSql)

                    Dim pAnd As String
                    If pIncludeAllDevices = False Then
                        Dim pDrDevice = pBusiness.LibraryDevice.ByUuid(FlushZipConverter.DeviceUuid)
                        If pDrDevice Is Nothing Then
                            Continue For
                        End If
                        pAnd = " AND TransImports.DeviceID = " & ReadyString(pDrDevice("ID"))
                    End If

                    Dim pSubqueryFunc =
                        Function(vNot As Boolean)
                            Dim pSqlInternal = " SELECT COUNT(*) AS Count" & vbCrLf &
                                " ,MIN(TransImports.DateReceived) AS MinDateReceived" & vbCrLf &
                                " ,MIN(TransImports.ProcessStart) AS MinProcessStart" & vbCrLf &
                                " ,MAX(TransImports.DateReceived) AS MaxDateReceived" & vbCrLf &
                                " ,MAX(TransImports.ProcessStart) AS MaxProcessStart" & vbCrLf &
                                " FROM TransImports" & vbCrLf &
                                " WHERE TransImports.ProcessEnd IS " & If(vNot, "NOT ", "") & "NULL" & vbCrLf &
                                pAnd
                            Return pSqlInternal
                        End Function

                    pSql = "SELECT tblNoEnd.Count AS [Unended Count]" & vbCrLf &
                        ",tblNoEnd.MinDateReceived AS [Unended Min Date Received]" & vbCrLf &
                        ",tblNoEnd.MinProcessStart AS [Unended Min Process Start]" & vbCrLf &
                        ",tblNoEnd.MaxDateReceived AS [Unended Max Date Received]" & vbCrLf &
                        ",tblNoEnd.MaxProcessStart AS [Unended Max Process Start]" & vbCrLf &
                        ",tblHasEnd.Count AS [Ended Count]" & vbCrLf &
                        ",tblHasEnd.MinDateReceived AS [Ended Min Date Received]" & vbCrLf &
                        ",tblHasEnd.MinProcessStart AS [Ended Min Process Start]" & vbCrLf &
                        ",tblHasEnd.MaxDateReceived AS [Ended Max Date Received]" & vbCrLf &
                        ",tblHasEnd.MaxProcessStart AS [Ended Max Process Start]" & vbCrLf &
                        " FROM (" & vbCrLf &
                        pSubqueryFunc(False) &
                        ") tblNoEnd" & vbCrLf &
                        " CROSS APPLY (" & vbCrLf &
                        pSubqueryFunc(True) &
                        ") tblHasEnd"

                    Dim pDT = pProData.Execute(pSql)
                        pDT.Columns.Add("CompanyName")
                        pDT.Rows(0)("CompanyName") = pDrOrg("CompanyName")
                        pDT.Columns("CompanyName").SetOrdinal(0)
                        If pDtAll Is Nothing Then
                            pDtAll = pDT
                        Else
                            pDtAll.Merge(pDT)
                        End If
                    Next
            End Using

            Return JsonDictionary.Serialize(pDtAll)
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Function CallImportOrganization() As String

        Dim pDatabase As String

        Try
            Dim pDtOrgs As DataTable
            Using pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                Dim pBusiness = New Business(pProData)
                pDtOrgs = pBusiness.LibraryOrganization.GetAll(True)
                For Each pDrOrg As DataRow In pDtOrgs.Rows
                    pDatabase = pDrOrg("Database")
                    Dim pSql = "USE " & EscapeSqlName(pDatabase)
                    pProData.ExecuteSQL(pSql)

                    Dim pDrDevice = pBusiness.LibraryDevice.ByUuid(FlushZipConverter.DeviceUuid)
                    If pDrDevice Is Nothing Then
                        Continue For
                    End If
                    Call New WebClient().DownloadStringTaskAsync(Common.VirtualPath & "/OfflineData/CallImportOrganization?OrgID=" & CStr2(pDrOrg("ID")))
                Next
            End Using

            Return JsonDictionary.Serialize("Result", "ok")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    Function DeleteThirdPartyOkFlushFiles() As String

        Dim pDatabase As String

        Try
            If mSemaphore.Wait(2000) = False Then
                mStatus = "Another process is running since " & mDateStarted & ". vbZF2Y9Y2624!"
                Return mStatus
            End If
            mDateStarted = Now
            mStatus = "Started: " + mDateStarted
            Dim pOut As String = ""
            Dim pDtOrgs As DataTable
            Dim pFolders As String = ""
            Using pProData = ProData.CreateFromConnectionInfo(Common.ConnectionInfo)
                Dim pBusiness = New Business(pProData)
                pDtOrgs = pBusiness.LibraryOrganization.GetAll(True)
                For Each pDrOrg As DataRow In pDtOrgs.Rows
                    pDatabase = pDrOrg("Database")
                    Dim pSql = "USE " & EscapeSqlName(pDatabase)
                    pProData.ExecuteSQL(pSql)

                    Dim pDrDevice = pBusiness.LibraryDevice.ByUuid(FlushZipConverter.DeviceUuid)
                    If pDrDevice Is Nothing Then
                        Continue For
                    End If

                    Dim pFolder = Common.ServerPath & "temp\Uploads\" & ReadyGuid(pDrOrg("ID")) & "\" & ReadyGuid(pDrDevice("ID")) & "\"
                    If IO.Directory.Exists(pFolder) = False Then
                        Continue For
                    End If
                    If pFolders <> "" Then
                        pFolders &= "<br />" & vbCrLf
                    End If
                    pFolders &= pFolder
                    Dim pFiles = IO.Directory.GetFiles(pFolder, "*.zip")
                    For Each pFile In pFiles
                        Dim pFileNameWithoutExtension = Path.GetFileNameWithoutExtension(pFile)
                        If pFileNameWithoutExtension.Length <> 25 Then
                            Continue For
                        End If
                        Dim pResultFilename = Path.Combine(pFolder, pFileNameWithoutExtension & "_result.txt")
                        Dim pResultFileDate As Date?
                        If IsResultFileOk(pResultFilename, pResultFileDate) = False Then
                            Continue For
                        End If

                        Dim pDeletableFiles = IO.Directory.GetFiles(pFolder, pFileNameWithoutExtension & "*")
                        For Each pDeletetableFile In pDeletableFiles
                            IO.File.Delete(pDeletetableFile)
                            If pOut <> "" Then
                                pOut &= "<br />" & vbCrLf
                            End If
                            pOut &= pDeletetableFile
                        Next
                    Next
                Next
            End Using

            If pOut = "" Then
                pOut = DateTimeStamp() & " - No flushed zip file found in the following folders. <br />" & vbCrLf & pFolders
            End If
            Return pOut
        Catch ex As Exception
            Return "Current time: " + DateTime.Now & "<br />" & vbCrLf &
                "Database: " & pDatabase & "<br />" & vbCrLf &
                "Error: " + ex.Message.Replace(vbCrLf, "<br />") & "<br />" & vbCrLf &
                GetStackTrace(, ex).Replace(vbCrLf, "<br />")
        Finally
            mSemaphore.Release()
        End Try

    End Function

    Shared ReadOnly mSemaphoreVerify As New SemaphoreSlim(1)
    Shared mVerifyStart As Date
    Shared mVerifyStatus As String = ""
    Public Function Verify() As String

        Dim pDontRelease As Boolean

        Try
            If mSemaphoreVerify.Wait(2000) = False Then
                pDontRelease = True
                Return "Another Verify is running since " & mVerifyStart & ". vbAF3J43Y6HD!"
            End If

            Dim p = New ThirdPartyVerifier()
            p.ConnectionInfo = Common.ConnectionInfo

            TimedThread(
                Sub()
                    Try
                        mVerifyStart = Now
                        p.Execute()
                        mVerifyStatus = "all done " & DateTimeStamp()
                    Catch ex As Exception
                        mVerifyStatus = ex.Message & vbCrLf &
                            GetStackTrace(, ex)
                        AppendToDebug(ex.Message, True, ex, True)
                    Finally
                        mSemaphoreVerify.Release()
                    End Try
                End Sub
            )
            pDontRelease = True
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        Finally
            If pDontRelease = False Then
                mSemaphoreVerify.Release()
            End If
        End Try

        Return "Verify was successfully started. Browse /VerifyStatus next."

    End Function

    Public Function VerifyStatus() As String

        Return "mVerifyStart: " & mVerifyStart & "<br />" & vbCrLf &
            mVerifyStatus.Replace(vbCrLf, "<br />")

    End Function
End Class
