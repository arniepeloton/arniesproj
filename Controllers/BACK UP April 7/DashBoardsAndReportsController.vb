Imports Aspose.Cells

Namespace WebProj
    Public Class DashBoardsAndReportsController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /DashBoardsAndReports

#Region "DashBoard"
        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function Sales() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function SalesOrder() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function
        Function SalesOrderStockViewer() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function
        Function Franchise() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function
        Function FranchiseRequest() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function
        Function crm() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function DownloadExcelReportFile() As ActionResult
            Dim pFileName = Common.WebRequest.QueryString("FileName")
            Dim pSourceURL As String = Common.ServerPath & "\temp\AuditTrail\" & pFileName
            Dim pContentType As String = "application/xls"

            Try
                Return File(pSourceURL, pContentType, pFileName)
            Catch ex As Exception
                Return Nothing
            End Try

        End Function
        Function DownloadSalesOrderExcelFile() As ActionResult
            Dim pFileName = Common.WebRequest.QueryString("FileName")
            Dim pSourceURL As String = Common.ServerPath & "\temp\SalesOrder\" & pFileName
            Dim pContentType As String = "application/xls"

            Try
                Return File(pSourceURL, pContentType, pFileName)
            Catch ex As Exception
                Return Nothing
            End Try

        End Function

        <HttpPost()>
        Function GetTotalGrossAndDiscounts() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
            clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))
            clsDashBoardsAndReports.FOWSTID = WebRequest.Instance.Data("Fowst")
            clsDashBoardsAndReports.IsRawData = Common.WebRequest.Data("IsRawData")
            clsDashBoardsAndReports.GroupBy = Common.WebRequest.Data("GroupBy")

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetTotalGrossAndDiscounts())

        End Function

        <HttpPost()>
        Function GetTotalGrossAndDiscountsByGROUP() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
            clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))
            clsDashBoardsAndReports.FOWSTID = WebRequest.Instance.Data("Fowst")
            clsDashBoardsAndReports.IsRawData = Common.WebRequest.Data("IsRawData")
            clsDashBoardsAndReports.GroupBy = Common.WebRequest.Data("GroupBy")

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetTotalGrossAndDiscounts())

        End Function

        <HttpPost()>
        Function GetTotalSalesReturnsData() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
            clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))
            clsDashBoardsAndReports.FOWSTID = WebRequest.Instance.Data("Fowst")
            clsDashBoardsAndReports.IsRawData = Common.WebRequest.Data("IsRawData")
            clsDashBoardsAndReports.GroupBy = Common.WebRequest.Data("GroupBy")

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetTotalSalesReturnsData())
        End Function

        <HttpPost()>
        Function GetTotalSalesReturnsDataByGROUP() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
            clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))
            clsDashBoardsAndReports.FOWSTID = WebRequest.Instance.Data("Fowst")
            clsDashBoardsAndReports.IsRawData = Common.WebRequest.Data("IsRawData")
            clsDashBoardsAndReports.GroupBy = Common.WebRequest.Data("GroupBy")

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetTotalSalesReturnsData())

        End Function

        <HttpPost()>
        Function GetTotalGrossAndDiscountsByFOWST() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
            clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))
            clsDashBoardsAndReports.FOWSTID = WebRequest.Instance.Data("Fowst")
            clsDashBoardsAndReports.IsRawData = Common.WebRequest.Data("IsRawData")
            clsDashBoardsAndReports.GroupBy = Common.WebRequest.Data("GroupBy")

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetTotalGrossAndDiscountsByFOWST())

        End Function

        Function GetFOWSTCount() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetFOWSTCount())

        End Function

        <HttpPost()>
        Function GetOrganizationName() As String
            Dim pSql As String
            Dim pDT As DataTable

            pSql = "select CompanyNameInitial, CompanyName from MobizLibraryOrganization where ID = " & ReadyString(WebRequest.Instance().OrgID)

            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)

        End Function

        <HttpPost()>
        Function GetPOSSalesReportByTrendOrSalesDistribution() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization


            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
            clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))
            clsDashBoardsAndReports.FOWSTID = WebRequest.Instance.Data("Fowst")
            clsDashBoardsAndReports.IsRawData = Common.WebRequest.Data("IsRawData")
            clsDashBoardsAndReports.GroupBy = Common.WebRequest.Data("GroupBy")

            If Not CBool(Common.WebRequest.Data("IsRawData")) Then
                clsDashBoardsAndReports.CheckSumForDashboardAndPOSTransaction()
            End If

            Return JsonDictionary.Serialize("ReportPOS", clsDashBoardsAndReports.GetPOSSalesReportByTrendOrSalesDistribution())

        End Function

        <HttpPost()>
        Function GetPOSSalesReportBySlice() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("ReportPOS", clsDashBoardsAndReports.GetPOSSalesReportBySlice())

        End Function

        <HttpPost()>
        Function GetDetailPOSSalesReportByDateAndTime() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("ReportPOS", clsDashBoardsAndReports.GetDetailPOSSalesReportByDateAndTime())
        End Function

        <HttpPost()>
        Function GetPageAccess() As String
            Dim dtSecurity As DataTable
            Dim mSecurity As New SecurityRole

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            dtSecurity = mSecurity.CheckSecurityCurrentUserAcccess("Show Dashboard First Page", Common.WebRequest.Data("OrgID"), WebSession.Instance.PeopleUserID)

            If dtSecurity.Rows.Count > 0 Then
                Return JsonDictionary.Serialize("Results", "True")
            Else
                Return JsonDictionary.Serialize("Results", "False")
            End If

        End Function
#End Region

#Region "POSTransactionDetail"
        Function POSTransactionDetail() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        <HttpPost()>
        Function GetRawData() As String
            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim pDTDate As DataTable
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            Dim strFileName As String
            Dim pSourceURL As String = Common.ServerPath & "temp\AuditTrail"

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Convert.ToBoolean(Convert.ToInt16(WebRequest.Instance.Data("IsMonth")))) Then

                pFinder = New Finder
                With pFinder
                    .ProData = Common.WebRequest.ProData

                    With .Fields
                        With .Add("DateFrom", SqlDbType.Date)
                            .Expression = ReadyString(WebRequest.Instance.Data("Month") & "/01/" & WebRequest.Instance.Data("Year"))
                        End With
                        With .Add("DateTo", SqlDbType.Date)
                            .Expression = "DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,'" & WebRequest.Instance.Data("Month") & "/01/" & WebRequest.Instance.Data("Year") & "')+1,0))"
                        End With
                    End With

                    pDTDate = .Requery
                End With

                If (pDTDate.Rows.Count > 0) Then
                    clsDashBoardsAndReports.DateFrom = Convert.ToDateTime(pDTDate.Rows(0).Item("DateFrom"))
                    clsDashBoardsAndReports.DateTo = Convert.ToDateTime(pDTDate.Rows(0).Item("DateTo"))
                Else
                    clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
                    clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))
                End If
            Else
                clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
                clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))
            End If


            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            clsDashBoardsAndReports.FOWSTID = WebRequest.Instance.Data("FOWSTID")
            clsDashBoardsAndReports.IsRawData = 0
            clsDashBoardsAndReports.GroupBy = "Day"

            strFileName = ""

            '''Do check First
            clsDashBoardsAndReports.CheckSumForDashboardAndPOSTransaction()
            pDT = clsDashBoardsAndReports.GetRawData()
            If (pDT.Rows.Count > 0) Then
                If Not System.IO.Directory.Exists(pSourceURL) Then
                    IO.Directory.CreateDirectory(pSourceURL)
                End If

                strFileName = WebRequest.Instance.Data("OrgID") & "-" & WebSession.Instance.PeopleUserID.ToUpper() & ".xls"

                If (IO.File.Exists(pSourceURL & "\PIMTemplate\" & strFileName)) Then
                    IO.File.Delete(pSourceURL & "\PIMTemplate\" & strFileName)
                End If

                TableToExcelFile(pDT, "", "", pSourceURL & "\" & strFileName)

            End If

            Return JsonDictionary.Serialize("POSTransactionDetail", strFileName)

        End Function

        <HttpPost()>
        Function GetSalesReturnsData() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
            clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))
            clsDashBoardsAndReports.FOWSTID = WebRequest.Instance.Data("Fowst")
            clsDashBoardsAndReports.IsRawData = CInt(Common.WebRequest.Data("IsRawData"))

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetSalesReturnsData())
        End Function

        <HttpPost()>
        Function GetLastPOSSalesTransaction() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetLastPOSSalesTransaction())
        End Function

        <HttpPost()>
        Function GetSummary() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("POSTransactionDetail", clsDashBoardsAndReports.GetSummary())
        End Function

        Function Settings() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function GetOrganizationProductFeatures() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetOrganizationProductFeatures())
        End Function

        Function GetAllProductFeatures() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetAllProductFeatures())
        End Function

        Function SaveFeatures() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.SaveFeatures())
        End Function

        Function RemoveFeature() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.RemoveFeature())
        End Function
#End Region

#Region "Device"
        Function Device() As ActionResult

            'If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            '    If WebRequest.Instance.HandleAccess = False Then Exit Function
            'End If

            Return View()

        End Function

        <HttpPost()>
        Public Function UploadGPS() As String

            Dim pFilename As String
            Dim pOut As JsonDictionary
            Dim pTimestamp As String

            pOut = New JsonDictionary
            Try
                pTimestamp = DateTimeStamp()
                pFilename = Common.ServerPath & "temp\DataExport\Download\Location" & pTimestamp & ".zip"
                Request.Files(0).SaveAs(pFilename)

                SaveGPSData(pFilename)
            Catch ex As System.Exception
                pOut.Add("Error", "An error occured while saving imported file: " & ex.Message)
                Return pOut.Serialize
            End Try

            pOut.Add("Result", "ok")

            Return pOut.Serialize

        End Function

        Function SaveGPSData(ByVal pFilename As String) As String

            Dim MyLocation As String()
            Dim p As New InsertUpdate("MobizTransDeviceLocation")
            Dim pSql As String
            Dim pLine As Dictionary(Of String, String)
            Dim pCsvReader As CsvReader
            Dim dtOrganizations As DataTable
            Dim lID As String
            
            pCsvReader = New CsvReader(pFilename, "Locations")

            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                
                MyLocation = Split(pLine("Location"), "-")

                pSql = "Select OrgID from MobizTransDeviceOrganizationPairing where UUID = " & ReadyString(MyLocation(0)) & " AND IsMonitoring = 1"

                dtOrganizations = Common.WebRequest.ProData.Execute(pSql)
                
                lID = Guid.NewGuid.ToString

                Try
                    p("ID") = ReadyString(lID)
                    p("UUID") = ReadyString(MyLocation(0))
                    p("Lat") = ReadyString(MyLocation(1))
                    p("Long") = ReadyString(MyLocation(2))
                    p("RegisteredDate") = ReadyString(CDate(MyLocation(3)))
                    p("Source") = ReadyString(MyLocation(4))

                    If MyLocation.Length > 5 Then
                        p("UTCTime") = ReadyString(MyLocation(5))
                    End If

                    pSql = p.GetInsert
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If dtOrganizations.Rows.Count > 0 Then
                        For i As Integer = 0 To dtOrganizations.Rows.Count
                            pSql = "Insert into TransDeviceLocationOrganization(TransDeviceLocationID, OrgID) values(" & ReadyString(lID) & "," & ReadyString(dtOrganizations.Rows(i).Item("OrgID")) & ")"
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        Next
                    End If
                Catch ex As Exception

                End Try
            Loop

        End Function

        Function GetGPSLocations() As String
            Dim pDT As DataTable

            Dim pFinder As Finder

            'for mobiz trax
            Dim dtTrax As DataTable
            Dim pSql As String

            pSql = "Select * from MobizTransDeviceOrganizationPairing where OrgID = " & ReadyString(Common.WebRequest.Data("OrgID")) & " AND UUID = " & ReadyString(Common.WebRequest.Data("UUID"))
            dtTrax = Common.WebRequest.ProData.Execute(pSql)

            If dtTrax.Rows.Count = 0 Then
                pSql = "Insert into MobizTransDeviceOrganizationPairing(OrgID, UUID) values(" & ReadyString(Common.WebRequest.Data("OrgID")) & ", " & ReadyString(Common.WebRequest.Data("UUID")) & ")"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData
                .Distinct = True

                With .Fields
                    .Add("Long", SqlDbType.Bit)
                    .Add("Lat", SqlDbType.Int)

                    With .Add("RegisteredDay", SqlDbType.VarChar)
                        .Expression = "convert(varchar(50),convert(date,RegisteredDate))"
                    End With

                    With .Add("RegisteredTime", SqlDbType.Date)
                        .Expression = "convert(time,RegisteredDate)"
                    End With
                    .Add("Source", SqlDbType.VarChar)
                End With

                .SqlFrom = " FROM MobizTransDeviceLocation " & vbCrLf &
                            " inner join MobizTransDeviceLocationOrganization " & vbCrLf &
                            " ON MobizTransDeviceLocationOrganization.TransDeviceLocationID = MobizTransDeviceLocation.ID " & vbCrLf
                .SqlWhere = " WHERE UUID = " & ReadyString(Common.WebRequest.Data("UUID")) & vbCrLf &
                            " AND convert(date,RegisteredDate) BETWEEN " & ReadyString(ParseDate(Common.WebRequest.Data("From"))) & vbCrLf &
                            " AND " & ReadyString(ParseDate(Common.WebRequest.Data("To"))) & vbCrLf &
                            " AND MobizTransDeviceLocationOrganization.OrgID = " & ReadyString(Common.WebRequest.Data("OrgID"))
                .SqlOrder = " ORDER by convert(varchar(50),convert(date,RegisteredDate)) , convert(time,RegisteredDate)  desc "

                pDT = .Requery

                Return JsonDictionary.Serialize("Results", pDT)
            End With
        End Function

        Function GetAllMonitoringOrganization() As String
            Dim pSql As String
            Dim pDT As DataTable

            pSql = "SELECT DISTINCT LibraryOrganization.CompanyName from TransDeviceOrganizationPairing " & vbCrLf &
                    "inner join LibraryOrganization on LibraryOrganization.ID = TransDeviceOrganizationPairing.OrgID " & vbCrLf &
                    "where IsMonitoring = 1 AND UUID = " & ReadyString(Common.WebRequest.Data("UUID"))

            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)
        End Function
#End Region

#Region "Order Central"

        <HttpPost()>
        Function GetMaxLimit() As String
            Dim d As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pOut As JsonDictionary
            pOut = New JsonDictionary

            pOut.Add("MaxDownload", CStr2(WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("SalesOrderMaxDownloadCount")))
            pOut.Add("MaxPrint", CStr2(WebRequest.Instance.Business.SystemDefaults.GetSystemDefault("SalesOrderMaxPrintCount")))

            Return pOut.Serialize
        End Function


        <HttpPost()>
        Function GetSalesOrderReport() As String
            Dim d As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            d.OrgID = WebRequest.Instance().OrgID
            Return JsonDictionary.Serialize("SalesOrderReport", d.GetSalesOrderReport(Common.WebRequest.Data("SearchString")))
        End Function

        <HttpPost()>
        Function UpdateOrderStatus() As String
            Dim d As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            d.InvoiceID = Common.WebRequest.Data("OrderID")


            Return JsonDictionary.Serialize("Status", d.UpdateOrderStatus(Common.WebRequest.Data("Status"), CInt2(Common.WebRequest.Data("DownloadCount")), CInt2(Common.WebRequest.Data("PrintCount"))))
        End Function

        <HttpPost()>
        Function GetUpdatedStatus() As String
            Dim clsDashboard As New DashBoardsAndReports
            Dim pOut As JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            
            pOut = New JsonDictionary
            Try
                Dim statusDetails As DataTable = clsDashboard.GetUpdatedStatus(Common.WebRequest.Data("InvoiceID"))
                pOut.Add("StatusCount", statusDetails)


                Dim statusLatest As DataTable = clsDashboard.GetLatestStatus(Common.WebRequest.Data("InvoiceID"))
                pOut.Add("Status", statusLatest)

                pOut.Add("Result", "ok")
            Catch ex As Exception
                pOut.Add("Error", ex.Message)
            End Try

            Return pOut.Serialize
        End Function

        <HttpPost()>
        Function GetSalesOrderDetails() As String
            Dim clsSalesPayments As New SalesPayments
            Dim clsPeople As New People
            Dim clsDashboard As New DashBoardsAndReports
            Dim strReturn As String
            Dim dtaTable As DataTable
            Dim intCntr As Integer
            Dim strTemp As String
            Dim strInvoiceNo As String = ""
            Dim strEmail As String()
            Dim strBody As String = ""
            Dim strmReader As System.IO.StreamReader
            Dim varBusinessName As String
            Dim pOut As JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary
            Try
                Dim pplName As DataTable = clsPeople.getPeopleInfoName(WebSession.Instance.PeopleUserID)
                pOut.Add("PeopleName", pplName.Rows(0).Item("PeopleName").ToString().Trim())

                Dim purchaseDetails As DataTable = clsDashboard.GetSalesOrderPurchaseDetails(Common.WebRequest.Data("InvoiceID"))
                pOut.Add("PurchaserName", purchaseDetails.Rows(0).Item("PurchaserName").ToString().Trim())
                pOut.Add("PurchaseOrderNo", purchaseDetails.Rows(0).Item("PurchaseOrderNo").ToString().Trim())

                If (purchaseDetails.Rows(0).Item("PurchaseOrderDate").ToString().Trim() = "1/1/0001 12:00:00 AM") Then
                    pOut.Add("PurchaseOrderDate", "")

                Else
                    pOut.Add("PurchaseOrderDate", purchaseDetails.Rows(0).Item("PurchaseOrderDate").ToString().Trim())

                End If

                '''Get Header Info
                dtaTable = New DataTable()
                dtaTable = clsSalesPayments.getSalesOrderHeaderInfo(Common.WebRequest.Data("FowstID"))
                If (dtaTable.Rows.Count > 0) Then

                    If (dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim() <> "") Then
                        varBusinessName = dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim()
                    Else
                        varBusinessName = dtaTable.Rows(0).Item("CorporateName").ToString().Trim()
                    End If
                    pOut.Add("BusinessName", varBusinessName)
                    pOut.Add("BusinessTradeName", dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim())
                    pOut.Add("CorporateName", dtaTable.Rows(0).Item("CorporateName").ToString().Trim())
                    pOut.Add("TIN", dtaTable.Rows(0).Item("TIN").ToString().Trim())
                    pOut.Add("CompleteAddress", dtaTable.Rows(0).Item("CompleteAddress").ToString().Trim())
                    pOut.Add("TelNo", dtaTable.Rows(0).Item("TelNo").ToString().Trim())

                End If

                '''Get Payment Info
                dtaTable = New DataTable()
                dtaTable = clsSalesPayments.getSalesPaymentInvoiceHeaderInfo(Common.WebRequest.Data("InvoiceID"))
                If (dtaTable.Rows.Count > 0) Then
                    pOut.Add("CustomerName", dtaTable.Rows(0).Item("CustomerName").ToString().Trim())
                    pOut.Add("CustomerAddress", dtaTable.Rows(0).Item("CustomerAddress").ToString().Trim())
                    pOut.Add("CustomerTIN", dtaTable.Rows(0).Item("CustomerTIN").ToString().Trim())
                    pOut.Add("BillingAddress", dtaTable.Rows(0).Item("BillingAddress").ToString().Trim())
                    pOut.Add("DeliveryAddress", dtaTable.Rows(0).Item("DeliveryAddress").ToString().Trim())
                    pOut.Add("SalesOrderNo", dtaTable.Rows(0).Item("SalesOrderNo").ToString().Trim())
                    pOut.Add("SalesOrderDate", Convert.ToDateTime(dtaTable.Rows(0).Item("SalesOrderDate")).ToString("MMM dd, yyyy HH:mm").Trim())
                    pOut.Add("SalesRepName", dtaTable.Rows(0).Item("CashierName").ToString().Trim())

                End If

                '''Get Payment Items
                dtaTable = New DataTable()
                dtaTable = clsSalesPayments.getSalesPaymentReceiptBodyInfo(Common.WebRequest.Data("InvoiceID"))
                If (dtaTable.Rows.Count > 0) Then
                    pOut.Add("StockData", dtaTable)

                End If

                '''Get Payment Amount
                dtaTable = New DataTable()
                dtaTable = clsSalesPayments.getSalesPaymentReceiptFooterAmountsInfo(Common.WebRequest.Data("InvoiceID"))
                If (dtaTable.Rows.Count > 0) Then

                    pOut.Add("SKUCount", Convert.ToInt32(dtaTable.Rows(0).Item("SKUCount")).ToString("#,##0").Trim())
                    pOut.Add("TotalAmountDue", Convert.ToDouble(dtaTable.Rows(0).Item("TotalAmountDue")).ToString("#,##0.00").Trim())
                    pOut.Add("DiscountAmount", Convert.ToDouble(dtaTable.Rows(0).Item("DiscountAmount")).ToString("#,##0.00").Trim())
                    pOut.Add("NetAmountDue", Convert.ToDouble(dtaTable.Rows(0).Item("NetAmountDue")).ToString("#,##0.00").Trim())
                    pOut.Add("TaxSales", Convert.ToDouble(dtaTable.Rows(0).Item("TaxSales")).ToString("#,##0.00").Trim())
                    pOut.Add("TaxAmount", Convert.ToDouble(dtaTable.Rows(0).Item("TaxAmount")).ToString("#,##0.00").Trim())

                End If

                pOut.Add("Result", "ok")
            Catch ex As Exception
                pOut.Add("Result", "error")
            End Try

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function GetRawSalesOrderData() As String
            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim pDTDate As DataTable
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            Dim strFileName As String
            Dim pSourceURL As String = Common.ServerPath & "temp\SalesOrder"

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pDT = clsDashBoardsAndReports.GetRawSalesOrderData(Common.WebRequest.Data("FowstID"), Common.WebRequest.Data("InvoiceID"), Common.WebRequest.Data("SalesOrderGroupNo"), "", "")
            If (pDT.Rows.Count > 0) Then
                If Not System.IO.Directory.Exists(pSourceURL) Then
                    IO.Directory.CreateDirectory(pSourceURL)
                End If

                strFileName = "SaleOrderData.csv"

                If (IO.File.Exists(pSourceURL & "\SalesOrder\" & strFileName)) Then
                    IO.File.Delete(pSourceURL & "\SalesOrder\" & strFileName)
                End If

                TableToExcelFile(pDT, "", "", pSourceURL & "\" & strFileName)

            End If

            Return JsonDictionary.Serialize("SalesOrderDetail", strFileName)

        End Function

        Function DownloadSalesOrderCsvFile() As ActionResult
            Dim pFileName = Common.WebRequest.QueryString("FileName")
            Dim pSourceURL As String = Common.ServerPath & "\temp\SalesOrder\" & pFileName
            Dim pContentType As String = "application/csv"

            Try
                Return File(pSourceURL, pContentType, pFileName)
            Catch ex As Exception
                Return Nothing
            End Try

        End Function

        Function GetInvoiceTypes() As String
            Dim clsDashboard As New DashBoardsAndReports

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsDashboard.GetInvoiceTypes())
        End Function

        Function SaveSalesInvoice() As String
            Dim pSql As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Update TransInvoice set " & vbCrLf &
                    "InvoiceNumber = " & CInt(Common.WebRequest.Data("InvoiceNumber")) & vbCrLf &
                    ", InvoiceType = " & ReadyString(Common.WebRequest.Data("InvoiceType")) & vbCrLf &
                    ", Remarks = " & ReadyString(Common.WebRequest.Data("Remarks")) & vbCrLf &
                    " Where ID = " & ReadyString(Common.WebRequest.Data("ID"))

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return "true"
        End Function

        Function SaveInvoiceType() As String
            Dim clsDashboard As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsDashboard.SaveInvoiceType())
        End Function

        <HttpPost()>
        Function SaveCreatedBy() As String
            Dim d As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            d.InvoiceID = Common.WebRequest.Data("OrderID")


            Return JsonDictionary.Serialize("SalesOrderReport", d.SaveCreatedBy(Common.WebRequest.Data("SalesRepID"), Common.WebRequest.Data("DateCreated")))
        End Function
#End Region

#Region "Franchise"
        <HttpPost()>
        Function GetFranchisees() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.GetFranchisees(Common.WebRequest.Data("SearchString"), False))
        End Function
        <HttpPost()>
        Function GetFranchiseeStatus() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.GetFranchiseeStatus(Common.WebRequest.Data("FranchiseID")))
        End Function

        <HttpPost()>
        Function GetFranchiseGroups() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.GetFranchiseGroups())
        End Function

        <HttpPost()>
        Function SaveRemoveRedFlagReason() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.SaveRemoveRedFlagReason(Common.WebRequest.Data("GroupCutOffID"), Common.WebRequest.Data("FranchiseeFowstID"), Common.WebRequest.Data("Reason")))
        End Function

        <HttpPost()>
        Function GetLimitRedFlag() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim limitRedFlag As Integer = WebRequest.Instance.Business.SystemDefaults.FranchiseRedFlagHours
            Return JsonDictionary.Serialize("FranchiseData", limitRedFlag)

        End Function
        <HttpPost()>
        Function GetFranchiseeGroupInfo() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.GetFranchiseeGroupInfo(Common.WebRequest.Data("FranchiseGroupID")))
        End Function

        <HttpPost()>
        Function GetFranchiseeOrders() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.GetFranchiseeOrders(Common.WebRequest.Data("GroupID"), ""))
        End Function
        <HttpPost()>
        Function GetFranchiseeOrdersUnassigned() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization


            clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
            clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))


            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.GetFranchiseeOrdersUnassigned())
        End Function

        <HttpPost()>
        Function GetFranchiseeLastReceived() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
            clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))

            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.GetFranchiseeLastReceived(Common.WebRequest.Data("GroupCutOffID")))
        End Function

        <HttpPost()>
        Function GetSalesBreakdownByFranchisee() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
            clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))

            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.GetSalesBreakdownByFranchisee(Common.WebRequest.Data("GroupCutOffID")))
        End Function

        <HttpPost()>
        Function GetProductOrders() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
            clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))

            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.GetProductOrders(Common.WebRequest.Data("GroupCutOffID")))
        End Function

        <HttpPost()>
        Function UpdateSalesOrderGroupNo() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.UpdateSalesOrderGroupNo(Common.WebRequest.Data("FranchiseGroupID")))
        End Function

        <HttpPost()>
        Function CheckFranchiseRedFlag() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.CheckFranchiseRedFlag(Common.WebRequest.Data("GroupCutOffID")))
        End Function

        <HttpPost()>
        Function ActivateFranchisee() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.ActivateFranchisee(Common.WebRequest.Data("FranchiseID"), Common.WebRequest.Data("FranchiseStatusID"), CBool2(Common.WebRequest.Data("IsFranchisee")), Common.WebRequest.Data("GroupID")))
        End Function

        <HttpPost()>
        Function RejectFranchisee() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.RejectFranchisee(Common.WebRequest.Data("FranchiseID"), Common.WebRequest.Data("FranchiseStatusID"), CBool2(Common.WebRequest.Data("IsFranchisee"))))
        End Function

        <HttpPost()>
        Function UpdateFranchiseOrderStatus() As String
            Dim d As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            d.InvoiceID = Common.WebRequest.Data("OrderID")


            Return JsonDictionary.Serialize("Status", d.UpdateOrderStatus(Common.WebRequest.Data("Status"), CInt2(Common.WebRequest.Data("DownloadCount")), CInt2(Common.WebRequest.Data("PrintCount"))))
        End Function


        <HttpPost()>
        Public Function GeneratePickingListPdf() As String

            Dim p As DashBoardsAndReports
            Dim pOut As JsonDictionary
            Dim pSalesOrderGroupNo As String
            Dim pFranchisorFowstID As String
            Dim pGroupID As String
            Dim pDtHeader As DataTable
            Dim pDtItems As DataTable
            Dim pDtFranchisees As DataTable
            Dim pReport As ProReport
            Dim pFilename As String, pFullFilename As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary()
            pFranchisorFowstID = Common.WebRequest.Data("FowstID")
            pSalesOrderGroupNo = Common.WebRequest.Data("SalesOrderGroupNo")
            pGroupID = Common.WebRequest.Data("GroupID")

            p = New DashBoardsAndReports()
            p.OrgID = WebRequest.Instance().OrgID

            pDtHeader = p.GetFranchiseeHeader(pFranchisorFowstID, "", pGroupID, pSalesOrderGroupNo)
            pDtItems = p.GetRawSalesOrderData("", "", pSalesOrderGroupNo, "", "")
            pDtFranchisees = p.GetFranchisees("", True)


            pFilename = "PickingList_" & DateTimeStamp() & "_" & GenerateString(10) & ".pdf"
            pFullFilename = Common.ServerPath & "temp\documents\" & pFilename
            pReport = New ProReport()
            pReport.AutoFillHeader = False
            pReport.SourceFilename = "PickingList"
            pReport.AddTable("header", pDtHeader)
            pReport.AddView("items", pDtItems.DefaultView)
            pReport.ExportXmlFile()
            pReport.ExportFile(ProReport.eExportFormat.Pdf, pFullFilename)
            pOut.Add("Filename", pFilename)
            pOut.Add("Result", "ok")
            pOut.Add("FranchiseeList", pDtFranchisees)
            Return pOut.Serialize()

        End Function


        <HttpPost()>
        Public Function GenerateSortingListPdf() As String
            Dim p As DashBoardsAndReports


            Dim pOut As JsonDictionary
            Dim pSalesOrderGroupNo As String
            Dim pFranchisorFowstID As String
            Dim pGroupID As String
            Dim pFranchiseeID As String
            Dim pBatchNoID As String
            Dim pDtHeader As DataTable
            Dim pDtItems As DataTable
            Dim pReport As ProReport
            Dim pFilename As String, pFullFilename As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary()
            pFranchisorFowstID = Common.WebRequest.Data("FowstID")
            pSalesOrderGroupNo = Common.WebRequest.Data("SalesOrderGroupNo")
            pGroupID = Common.WebRequest.Data("GroupID")
            pFranchiseeID = Common.WebRequest.Data("FranchiseeID")


            p = New DashBoardsAndReports()
            p.OrgID = WebRequest.Instance().OrgID
            pBatchNoID = p.GetFranchiseGroupBatchNoID(pFranchiseeID, pGroupID, pSalesOrderGroupNo)

            If (p.UpdateSortingListNo(pFranchisorFowstID, pFranchiseeID, pGroupID, pSalesOrderGroupNo, pBatchNoID)) Then

                pDtHeader = p.GetFranchiseeHeader(pFranchisorFowstID, pFranchiseeID, pGroupID, pSalesOrderGroupNo)
                pDtItems = p.GetRawSalesOrderData("", "", pSalesOrderGroupNo, pFranchiseeID, pGroupID)

                If (pDtItems.Rows.Count > 0) Then


                    pFilename = "SortingList_" & DateTimeStamp() & "_" & GenerateString(10) & ".pdf"
                    pFullFilename = Common.ServerPath & "temp\documents\" & pFilename
                    pReport = New ProReport()
                    pReport.AutoFillHeader = False
                    pReport.SourceFilename = "SortingList"
                    pReport.AddTable("header", pDtHeader)
                    pReport.AddView("items", pDtItems.DefaultView)
                    pReport.ExportXmlFile()
                    pReport.ExportFile(ProReport.eExportFormat.Pdf, pFullFilename)
                    pOut.Add("Filename", pFilename)
                Else
                    pOut.Add("Filename", "")
                End If

            End If


            pOut.Add("Result", "ok")
            Return pOut.Serialize()

        End Function

        <HttpPost()>
        Function AddFranchisee() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization


            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID

            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.AddFranchisee(Common.WebRequest.Data("FranchiseeID"), Common.WebRequest.Data("IsOwn")))
        End Function


        <HttpPost()>
        Function GetFranchiseRequests() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.GetFranchiseRequests())
        End Function

        <HttpPost()>
        Function GetFranchisorStatus() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("FranchiseData", clsDashBoardsAndReports.GetFranchisorStatus(Common.WebRequest.Data("FranchiseID")))
        End Function
#End Region

#Region "OffTake"
        Function OffTake() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function GetOffTake() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.FOWSTID = Common.WebRequest.Data("Fowst")
            clsDashBoardsAndReports.DateFrom = Common.WebRequest.Data("From")
            clsDashBoardsAndReports.DateTo = Common.WebRequest.Data("To")
            clsDashBoardsAndReports.IsRawData = Common.WebRequest.Data("IsRawData")

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetOffTakeData())

        End Function

        Function GetProductOfftakeDetails() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashBoardsAndReports.FOWSTID = Common.WebRequest.Data("Fowst")
            clsDashBoardsAndReports.DateFrom = Common.WebRequest.Data("From")
            clsDashBoardsAndReports.DateTo = Common.WebRequest.Data("To")
            clsDashBoardsAndReports.IsRawData = Common.WebRequest.Data("IsRawData")
            clsDashBoardsAndReports.ProductID = Common.WebRequest.Data("ProductID")
            clsDashBoardsAndReports.BinID = Common.WebRequest.Data("BinID")

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetProductOfftakeDetails())
        End Function

        Function GetFowstBin() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetFowstBin())
        End Function

        Function GetFowstFromLedger() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetFowstFromLedger())
        End Function
#End Region

    End Class
End Namespace