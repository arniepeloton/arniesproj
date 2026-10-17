Imports System.IO
Imports System.Net

Namespace WebProj
    Public Class MallLessorMegaworldController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Mall Lessor Report")) Then Exit Function
            End If

            Return View()
        End Function

        Function DownloadReportFile() As ActionResult
            Dim pFileTitle As String
            Dim pContentType As String
            Dim pSourceURL As String
            Dim pLoginHash As String
            Dim pDR As DataRow

            pFileTitle = Common.WebRequest.QueryString("ReportLink")
            pLoginHash = Common.WebRequest.QueryString("LoginHash")
            If CStr2(pLoginHash).Length <> 50 Then
                If WebRequest.Instance.HandleAccess() = False Then Exit Function
            Else
                pDR = MallLessorController.LoginData.GetByLoginHash(pLoginHash)
                If pDR Is Nothing Then
                    Return Nothing
                End If
                If ReadyGuid(IO.Path.GetFileNameWithoutExtension(pFileTitle)) <> ReadyGuid(pDR("OrganizationID")) Then
                    Return Nothing ' not same OrganizationID
                End If
            End If

            pSourceURL = Common.ServerPath & "/Content/AyalaTenant/" & pFileTitle
            pContentType = "application/zip"

            Return File(pSourceURL, pContentType, pFileTitle)

        End Function

        <HttpPost()>
        Function GenerateReportFile() As String
            Dim strGeneratedLink As String
            Dim clsMallLessorMegaworld As New MallLessorMegaworld
            Dim dtaTable As DataTable
            Dim dtDate As Date
            Dim strTenantCode As String
            Dim strTenantName As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If IsGuid(WebRequest.Instance.FowstID) = False Then Return JsonDictionary.Serialize("Error", "Invalid FOWST")

            If (Common.WebRequest.Data("RangeType") = "Today") Then
                dtDate = DateTime.Today
            ElseIf (Common.WebRequest.Data("RangeType") = "Yesterday") Then
                dtDate = DateTime.Today.AddDays(-1)
            ElseIf (Common.WebRequest.Data("RangeType") = "By Date") Then
                dtDate = Convert.ToDateTime(Common.WebRequest.Data("SalesDate"))
            End If

            dtaTable = clsMallLessorMegaworld.getMegaworldTenantInfo(Common.WebRequest.ProData)

            If (dtaTable.Rows.Count > 0) Then
                strGeneratedLink = clsMallLessorMegaworld.GenerateLessorSalesReport(True, Common.WebRequest.ProData, dtDate, dtaTable.Rows(0), WebRequest.Instance.OrgID)
            End If

            strReturn = JsonDictionary.Serialize("GeneratedReport", strGeneratedLink)

            Return strReturn

        End Function

        Function GetFowstDeviceReports() As Object
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pDT As DataTable
                'Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                'Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                'Dim PageSearchText As String = Request.Form.GetValues("search[value]")(0).ToString()
                'Dim IsCountPage As Boolean
                'Dim PageColumnNo As String = Request.Form.GetValues("order[0][column]")(0).ToString()
                'Dim PageColumnDirection As String = Request.Form.GetValues("order[0][dir]")(0).ToString()
                Dim fowstid As String = Request.Params.Get("FowstID")
                Dim deviceid As String = Request.Params.Get("DeviceID")
                Dim month As String = Request.Params.Get("Month")
                Dim year As String = Request.Params.Get("Year")

                'Dim whereSearch As String = ""
                'Dim whereOrder As String = ""
                'Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

                'If PageSearchText <> "" Then
                '    Dim whereSearchContent As String = ""
                '    Dim operators As String = ""

                '    Dim arr As Object = Split(PageSearchText, " ")

                '    For i As Integer = 0 To arr.length - 1
                '        If whereSearchContent <> "" Then
                '            operators = " OR "
                '        End If

                '        whereSearchContent = whereSearchContent & operators & " ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + ' ' + " & vbCrLf &
                '                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + ' ' + " & vbCrLf &
                '                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                '                        "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + ' ' + " & vbCrLf &
                '                        "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + ' ' + " & vbCrLf &
                '                        "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '') " & vbCrLf &
                '                        " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                '    Next
                '    whereSearch = " WHERE (" & whereSearchContent & ")"
                'End If

                'If PageColumnNo = 0 Or PageColumnNo = 1 Then
                '    whereOrder = " ORDER BY mainsql.SalesDate " & PageColumnDirection
                'ElseIf PageColumnNo = 2 Then
                '    whereOrder = " ORDER BY mainsql.ReportDate " & PageColumnDirection
                'End If

                Dim q As String = " SELECT * " & vbCrLf &
                        " FROM TenantMonitoringLogs " & vbCrLf &
                        " WHERE FOWSTID = " & ReadyString(fowstid) & vbCrLf &
                        " AND DeviceID = " & ReadyString(deviceid) & vbCrLf &
                        " AND MONTH(SalesDate) = " & month & vbCrLf &
                        " AND YEAR(SalesDate) = " & year & vbCrLf &
                        " --AND BatchNo <> 0 " & vbCrLf &
                        " --AND LastTransactionID IS NOT NULL " & vbCrLf &
                        " ORDER BY SalesDate DESC "

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(q))

                'pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                '        q & vbCrLf &
                '        ") as mainsql ) as v2 " & whereLimit

                'pDT = Common.WebRequest.ProData.Execute(pSql)

                'Dim pDTCount As DataTable = Common.WebRequest.ProData.Execute(q)

                'Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)), .recordsTotal = pDTCount.Rows().Count, .recordsFiltered = pDTCount.Rows().Count}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Public Function IsSentToFtp()

            Try
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                Dim pSql = "SELECT FileStatus" & vbCrLf &
                    ",Reported" & vbCrLf &
                    " FROM TenantMonitoringLogs" & vbCrLf &
                    " WHERE FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
                    " AND DeviceID = " & ReadyString(Common.WebRequest.Data("DeviceID")) & vbCrLf &
                    " AND CONVERT(date, SalesDate) = " & ReadyString(CDate(Common.WebRequest.Data("SalesDate")).ToString(cGlobal.FormatDateSql)) & vbCrLf &
                    " AND Reported = 1" & vbCrLf &
                    " AND FileStatus = 'ftp-sent'"
                Dim pDR = WebRequest.Instance.ProData.ExecuteRow(pSql)
                If pDR Is Nothing Then
                    Return JsonDictionary.Serialize("Result", 0)
                Else
                    Return JsonDictionary.Serialize("Result", 1)
                End If
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        Public Function GetStats()

            Try
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
                Dim fowstid As String = Common.WebRequest.Data("FowstID")
                Dim deviceid As String = Common.WebRequest.Data("DeviceID")
                Dim pSql = " WITH CTE AS (" & vbCrLf &
                    " SELECT *" & vbCrLf &
                    " FROM TenantMonitoringLogs" & vbCrLf &
                    " WHERE FOWSTID = " & ReadyString(fowstid) & vbCrLf &
                    " AND DeviceID = " & ReadyString(deviceid) & vbCrLf &
                    " )" & vbCrLf &
                    " SELECT tblUnreported.Count AS UnreportedCount" & vbCrLf &
                    " ,tblUnreported.MinSalesDate AS UnreportedMinSalesDate" & vbCrLf &
                    " ,tblUnreported.MaxSalesDate AS UnreportedMaxSalesDate" & vbCrLf &
                    " ,tblReported.Count AS ReportedCount" & vbCrLf &
                    " ,tblReported.MinSalesDate AS ReportedMinSalesDate" & vbCrLf &
                    " ,tblReported.MaxSalesDate AS ReportedMaxSalesDate" & vbCrLf &
                    " FROM (" & vbCrLf &
                    " 	SELECT count(*) AS count" & vbCrLf &
                    "     ,MIN(SalesDate) AS MinSalesDate" & vbCrLf &
                    "     ,MAX(SalesDate) AS MaxSalesDate" & vbCrLf &
                    "     FROM cte" & vbCrLf &
                    "     WHERE ISNULL(Reported, 0) = 0" & vbCrLf &
                    " ) tblUnreported" & vbCrLf &
                    " ,(" & vbCrLf &
                    " 	SELECT COUNT(*) AS count" & vbCrLf &
                    "     ,MIN(SalesDate) AS MinSalesDate" & vbCrLf &
                    "     ,MAX(SalesDate) AS MaxSalesDate" & vbCrLf &
                    "     FROM CTE" & vbCrLf &
                    "     WHERE ISNULL(Reported, 0) = 1" & vbCrLf &
                    " ) tblReported"
                Dim pResult = WebRequest.Instance.ProData.ExecuteRow(pSql)
                Dim pOut = New JsonDictionary()
                pOut.Add("Results", pResult)
                Return pOut.Serialize()
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        Public Function Resend()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim salesdate As String = Common.WebRequest.Data("SalesDate")
                Dim pSalesDate = ParseDate(salesdate)
                Dim fowstid As String = Common.WebRequest.Data("FowstID")
                Dim deviceid As String = Common.WebRequest.Data("DeviceID")

                If pSalesDate IsNot Nothing Then
                    Dim p As New InsertUpdate("TenantMonitoringLogs")
                    p("Reported") = 0
                    p("ReportDate") = "NULL"
                    p("LastUpdated") = "getdate()"
                    p("FileStatus") = "NULL"
                    p("ErrorMessage") = "NULL"
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" FOWSTID = " & ReadyString(fowstid) & " AND DeviceID = " & ReadyString(deviceid) & " AND CONVERT(date, SalesDate) = " & ReadyString(salesdate)))
                End If

#If DEBUG Then
                If Now.Date = #2024/6/6# Then
                    Throw New ApplicationException("simulated error. ref: vbRT6UDNGFJS!")
                End If
#End If

                Dim mBusiness As Business = New Business(Common.WebRequest.ProData)
                Dim import As New ImportDataClass(mBusiness)
                Dim pSql = "SELECT FOWSTMallName" & vbCrLf &
                    ",FtpServer" & vbCrLf &
                    ",FtpUsername" & vbCrLf &
                    ",FtpPassword" & vbCrLf &
                    " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
                    " WHERE ID = " & ReadyString(fowstid) & vbCrLf &
                    " AND OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)
                Dim pDrFowst = WebRequest.Instance.ProData.ExecuteRow(pSql)
                If pDrFowst Is Nothing Then
                    Return JsonDictionary.Serialize("Error", "The specified fowst does not exist. ref: vb6MMJQM6BKJ!")
                End If
                Dim pMallName = CStr2(pDrFowst("FOWSTMallName"))
                Dim pFilename = import.ResendReport(fowstid, deviceid, pMallName)
                Dim pOut = New JsonDictionary()
                pOut.Add("Results", "ok")
                pOut.Add("Filename", pFilename)

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        <HttpPost>
        Public Function UploadToRobinsonFtp() As String

            Try
                If Request.Files.Count = 0 Then
                    Return JsonDictionary.Serialize("Error", "No file. vbEXNMB8E9RF!")
                End If
                Dim pOrgID = Common.WebRequest.Data("OrgID")
                If CStr2(pOrgID).Trim = "" Then
                    Return JsonDictionary.Serialize("Error", "OrgID POST data is missing from uploaded file.")
                End If
                If IsGuid(pOrgID) = False Then
                    Return JsonDictionary.Serialize("Error", "OrgID POST data must be a valid GUID.")
                End If
                pOrgID = ReadyGuid(pOrgID)
                WebRequest.Instance.OrgID = pOrgID
                Dim pError As String
                If WebRequest.Instance.HandleOrganization(True, pError) = False Then
                    Return JsonDictionary.Serialize("Error", "OrgID, " & pOrgID & ", does not point to a subscriber database. " & AppendIfNeeded(pError, ".") & " vbNEHJXDQE86!")
                End If

                Dim pFowstID = Common.WebRequest.Data("FowstID")
                If CStr2(pFowstID).Trim = "" Then
                    Return JsonDictionary.Serialize("Error", "FowstID POST data is missing from uploaded file.")
                End If
                If IsGuid(pFowstID) = False Then
                    Return JsonDictionary.Serialize("Error", "FowstID POST data must be a valid GUID.")
                End If
                pFowstID = ReadyGuid(pFowstID)

                Dim pSql = "SELECT OrganizationID" & vbCrLf &
                    ",FOWSTMallName" & vbCrLf &
                    ",FtpServer" & vbCrLf &
                    ",FtpUsername" & vbCrLf &
                    ",FtpPassword" & vbCrLf &
                    ",RobinsonTenantId" & vbCrLf &
                    " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
                    " WHERE ID = " & ReadyString(pFowstID)
                Dim pDrFowst = WebRequest.Instance.ProData.ExecuteRow(pSql)

                Dim pFtpError As String = ""
                Dim pFtpServer = CStr2(pDrFowst("FtpServer"))
                If String.IsNullOrWhiteSpace(pFtpServer) Then
                    pFtpError &= "FTP Server is blank. "
                End If
                Dim pFtpUsername = CStr2(pDrFowst("FtpUsername"))
                If String.IsNullOrWhiteSpace(pFtpUsername) Then
                    pFtpError &= "FTP Username is blank. "
                End If
                Dim pFtpPassword = CStr2(pDrFowst("FtpPassword"))
                If String.IsNullOrWhiteSpace(pFtpPassword) Then
                    pFtpError &= "FTP Password is blank. "
                End If
                If pFtpError <> "" Then
                    'pOut.Add("FtpError", pFtpError)
                End If

                Dim pBaseFilename = Request.Files(0).FileName
                Dim pFilename = Common.ServerPath & "temp\ToRobinsonFtp\" & pBaseFilename
                CreateFolder(pFilename)
                Request.Files(0).SaveAs(pFilename)
                ImportDataClass.RlcFtpUpload(vFtpServer:=pFtpServer,
                    vFtpUsername:=pFtpUsername,
                    vFtpPassword:=pFtpPassword,
                    vSourceFilename:=pFilename,
                    vDestinationFilename:=pBaseFilename)

                Return JsonDictionary.Serialize("Result", "ok")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        Function Download() As ActionResult

            Dim pOrgID = Common.WebRequest.QueryString("OrgID")
            Dim pFowstID = Common.WebRequest.QueryString("FowstID")
            Dim pDeviceID = Common.WebRequest.QueryString("DeviceID")
            Dim pFileTitle = Common.WebRequest.QueryString("Filename")
            Dim pFilename As String = Path.Combine(Common.ServerPath, "Content", "Robinson", pOrgID, pFowstID, pDeviceID, pFileTitle)
            If IO.File.Exists(pFilename) = False Then
                pFileTitle &= ".txt"
                pFilename = Path.Combine(Common.ServerPath, "Content", "Robinson", pOrgID, pFowstID, pDeviceID, pFileTitle)
            End If
            Dim pContentType As String = "application/txt"

            Try
                If IO.File.Exists(pFilename) Then
                    Return File(pFilename, pContentType, pFileTitle)
                End If
                Return Content("File does not exist")
            Catch ex As Exception
                Return Nothing
            End Try

        End Function
    End Class
End Namespace