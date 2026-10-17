Imports Aspose.Cells
Imports Newtonsoft.Json

Namespace WebProj
    Public Class DashBoardsAndReportsController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /DashBoardsAndReports

#Region "DashBoard"
        Function Index() As ActionResult

#If DEBUG Then

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            'If WebRequest.Instance.OrgID = "CFE39DF6-8809-4973-9515-810DF75AED12" Then
            '    Return RedirectToAction("Index", "CincoCorporation")
            'Else
            'Return View()
            Return PartialView("~/Views/DashboardsAndReports/BsIndex.vbhtml")
            If WebRequest.Instance.OrgID = "CFE39DF6-8809-4973-9515-810DF75AED12" Then
                Return RedirectToAction("Index", "CincoCorporation")
            Else
                Return View()
            End If
#Else

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            If WebRequest.Instance.OrgID = "d362ffdb-15d1-451f-97dc-c567b94fb410" AND CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                'Return RedirectToAction("Index", "CincoCorporation")
                Return PartialView("~/Views/DashboardsAndReports/BsIndex.vbhtml")
            Else
                Return PartialView("~/Views/DashboardsAndReports/BsIndex.vbhtml")
            End If
#End If

        End Function

        Function WOW() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function MPOSSalesDashboard() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function MPOSUtilization() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function VouchersReport() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function BsIndex() As ActionResult

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

        Function OfflineTransactionHistory() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function OfflineProductHistory() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function SalesOrder() As ActionResult

            'If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            '    If WebRequest.Instance.HandleAccess = False Then Exit Function
            'End If

            Return View()

        End Function

        <HttpGet()>
        Public Function RequestAndApprovals() As ActionResult

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

        Function DashboardsMenu()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pFinder As Finder
            Dim pDT As DataTable

            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("ModuleName", SqlDbType.VarChar)
                        .Expression = "md.ModuleName"
                    End With
                    With .Add("IsHidden", SqlDbType.Bit)
                        .Expression = "rmd.IshasAccess"
                    End With
                    With .Add("SortOrder", SqlDbType.Bit)
                        .Expression = "md.SortOrder"
                    End With
                End With

                .SqlFrom = " FROM dbo.LibrarySecurityRoleUser usr " &
                    " INNER JOIN dbo.LibrarySecurityRole r ON (usr.RoleID = r.ID) " &
                    " INNER JOIN dbo.LibrarySecurityRoleModules rmd ON (rmd.RoleID = r.ID) " &
                    " INNER JOIN dbo.LibrarySecurityModule md ON (rmd.ModuleID = md.ID) "

                .SqlWhere = " WHERE rmd.IshasAccess = 1 " & vbCrLf &
                            " AND usr.PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                            " AND (md.ID = 'cc2befbb-6d74-4b40-92df-affd5dd16d82' OR md.HeaderID = 'cc2befbb-6d74-4b40-92df-affd5dd16d82') " & vbCrLf &
                            " ORDER BY SortOrder ASC "

                pDT = .Requery
            End With

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function DeviceMonitoringFilters()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pFinder As Finder
            Dim pDT As DataTable

            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("ModuleName", SqlDbType.VarChar)
                        .Expression = "md.ModuleName"
                    End With
                    With .Add("IsHidden", SqlDbType.Bit)
                        .Expression = "rmd.IshasAccess"
                    End With
                    With .Add("SortOrder", SqlDbType.Bit)
                        .Expression = "md.SortOrder"
                    End With
                End With

                .SqlFrom = " FROM dbo.LibrarySecurityRoleUser usr " &
                    " INNER JOIN dbo.LibrarySecurityRole r ON (usr.RoleID = r.ID) " &
                    " INNER JOIN dbo.LibrarySecurityRoleModules rmd ON (rmd.RoleID = r.ID) " &
                    " INNER JOIN dbo.LibrarySecurityModule md ON (rmd.ModuleID = md.ID) "

                .SqlWhere = " WHERE rmd.IshasAccess = 1 " & vbCrLf &
                            " AND usr.PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                            " AND (md.ID = 'c14d332d-9103-4fe0-b6b9-cc0c404637ed' OR md.HeaderID = 'c14d332d-9103-4fe0-b6b9-cc0c404637ed') " & vbCrLf &
                            " ORDER BY SortOrder ASC "

                pDT = .Requery
            End With

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function DashboardDonutChartFilters()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pFinder As Finder
            Dim pDT As DataTable

            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("ModuleName", SqlDbType.VarChar)
                        .Expression = "md.ModuleName"
                    End With
                    With .Add("IsHidden", SqlDbType.Bit)
                        .Expression = "rmd.IshasAccess"
                    End With
                    With .Add("SortOrder", SqlDbType.Bit)
                        .Expression = "md.SortOrder"
                    End With
                End With

                .SqlFrom = " FROM dbo.LibrarySecurityRoleUser usr " &
                    " INNER JOIN dbo.LibrarySecurityRole r ON (usr.RoleID = r.ID) " &
                    " INNER JOIN dbo.LibrarySecurityRoleModules rmd ON (rmd.RoleID = r.ID) " &
                    " INNER JOIN dbo.LibrarySecurityModule md ON (rmd.ModuleID = md.ID) "

                .SqlWhere = " WHERE rmd.IshasAccess = 1 " & vbCrLf &
                            " AND usr.PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                            " AND md.ID IN ('b6f38aec-c892-48e6-bcc0-50d1ae80fe2b', '39a3ecfe-7b6f-4897-b79d-c5846a86460a') " & vbCrLf &
                            " ORDER BY SortOrder ASC "

                pDT = .Requery
            End With

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function GetSalesByProductCategory()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = " SELECT DISTINCT TOP 100 PERCENT " & vbCrLf &
                                " (CASE WHEN SUM(ISNULL(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions, 0)) = 0 THEN SUM(dbo.TransInvoiceItems.SubTotal) ELSE SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions) END) AS [Total] " & vbCrLf &
                                " , COUNT(dbo.TransInvoice.ID) AS [TransactionCount] " & vbCrLf &
                                " , SUM(dbo.TransInvoiceItems.Quantity) AS [ProductCount] " & vbCrLf &
                                " , SUM(dbo.TransInvoiceItems.OrderedQuantity) AS [ProductCountSO] " & vbCrLf &
                                " , ISNULL(CASE WHEN dbo.MobizLibraryPIMProductVariableFeatures001.C0020 = '' THEN 'N/A' ELSE MobizLibraryPIMProductVariableFeatures001.C0020 END,'N/A') AS [ProductFeature] " & vbCrLf &
                                " FROM dbo.TransInvoiceItems " & vbCrLf &
                                " INNER JOIN dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID " & vbCrLf &
                                " INNER JOIN dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID " & vbCrLf &
                                " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 ON dbo.TransInvoiceItems.ProductID = MobizLibraryPIMProductVariableFeatures001.ProductID " & vbCrLf &
                                " WHERE (dbo.TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0) AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(Common.WebRequest.Data("dateFrom")) & " AND " & ReadyString(Common.WebRequest.Data("dateTo")) & vbCrLf &
                                " AND FOWSTID in (SELECT DISTINCT FranchiseeOrgFowstID from MobizLibraryOrganizationFranchisee INNER JOIN LibraryFranchiseGroupHeads on LibraryFranchiseGroupHeads.FranchiseGroupID=MobizLibraryOrganizationFranchisee.franchisegroupid " & vbCrLf &
                                " WHERE LibraryFranchiseGroupHeads.PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & ") " & vbCrLf &
                                " GROUP BY ISNULL(CASE WHEN dbo.MobizLibraryPIMProductVariableFeatures001.C0020 = '' THEN 'N/A' ELSE MobizLibraryPIMProductVariableFeatures001.C0020 END,'N/A') " & vbCrLf &
                                " Order by Total " & vbCrLf

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        <HttpPost()>
        Function GetInventoryLevelApproveDisapprove() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            clsDashBoardsAndReports.FOWSTID = WebRequest.Instance.Data("FowstID")

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetInventoryLevelApproveDisapprove())

        End Function

        <HttpPost()>
        Function GetInventoryLevelApproveDisapproveDetails() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            clsDashBoardsAndReports.BatchID = WebRequest.Instance.Data("BatchID")
            clsDashBoardsAndReports.FOWSTID = WebRequest.Instance.Data("FowstID")

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetInventoryLevelApproveDisapproveDetails())

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

            Dim pOut As JsonDictionary
            Dim ReportPOS As DataTable

            pOut = New JsonDictionary

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization


            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
            clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))
            clsDashBoardsAndReports.FOWSTID = WebRequest.Instance.Data("Fowst")
            clsDashBoardsAndReports.IsRawData = Common.WebRequest.Data("IsRawData")
            clsDashBoardsAndReports.GroupBy = Common.WebRequest.Data("GroupBy")
            clsDashBoardsAndReports.InvoiceID = Common.WebRequest.Data("InvoiceID")
            clsDashBoardsAndReports.ShowSKU = Common.WebRequest.Data("FilterTrans")

            'If Not CBool(Common.WebRequest.Data("IsRawData")) Then
            '    clsDashBoardsAndReports.CheckSumForDashboardAndPOSTransaction()
            'End If

            'Return JsonDictionary.Serialize("ReportPOS", clsDashBoardsAndReports.GetPOSSalesReportByTrendOrSalesDistribution())

            ReportPOS = clsDashBoardsAndReports.GetPOSSalesReportByTrendOrSalesDistribution()

            'pOut.Add("ReportPOS", ReportPOS)

            If CBool(Common.WebRequest.Data("IsRawData")) AndAlso ReportPOS.Rows.Count > 0 Then
                pOut.Add("ReportPOS", clsDashBoardsAndReports.GetPOSSalesInvoiceItemModifiers(ReportPOS))
            Else
                pOut.Add("ReportPOS", ReportPOS)
            End If

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function GetPOSSalesInvoiceHeader() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization


            clsDashBoardsAndReports.OrgID = WebRequest.Instance().OrgID
            clsDashBoardsAndReports.DateFrom = CDate(WebRequest.Instance.Data("From"))
            clsDashBoardsAndReports.DateTo = CDate(WebRequest.Instance.Data("To"))
            clsDashBoardsAndReports.FOWSTID = WebRequest.Instance.Data("Fowst")
            clsDashBoardsAndReports.IsRawData = Common.WebRequest.Data("IsRawData")
            clsDashBoardsAndReports.GroupBy = Common.WebRequest.Data("GroupBy")
            clsDashBoardsAndReports.SearchString = Common.WebRequest.Data("SearchString")

            Dim pOut As JsonDictionary
            Dim pDTInvoiceHeader As DataTable
            Dim dtSummary As New DataTable

            pOut = New JsonDictionary

            pDTInvoiceHeader = clsDashBoardsAndReports.GetPOSSalesInvoiceHeader()

            pOut.Add("ReportPOS", pDTInvoiceHeader)
            pOut.Add("POSCashier", clsDashBoardsAndReports.GetPOSCashiers())

            Dim clsSalesReport As New ReportPOS

            If Not CStr2(Common.WebRequest.Data("CashierID")) = vbNullString Then
                'get cashier sales report
                pOut.Add("POSCashierSalesReport", clsSalesReport.getCashierSalesTransactionSummary(CStr2(Common.WebRequest.Data("CashierID")), "Day", CDate(WebRequest.Instance.Data("From")), CDate(WebRequest.Instance.Data("To")), Date.Today.Year.ToString()))
            Else
                pOut.Add("POSManagerSalesReport", clsSalesReport.getManagerSalesTransactionSummary("Day", CDate(WebRequest.Instance.Data("From")), CDate(WebRequest.Instance.Data("To")), Date.Today.Year.ToString()))
                pOut.Add("POSManagerSalesReportByCashier", clsSalesReport.getManagerSalesReportPaymentInfoByCashier("Day", CDate(WebRequest.Instance.Data("From")), CDate(WebRequest.Instance.Data("To")), Date.Today.Year.ToString()))
                'get manager sales report
            End If


            Return pOut.Serialize


        End Function

        <HttpPost()>
        Function GetPOSSalesReportBySlice() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("ReportPOS", clsDashBoardsAndReports.GetPOSSalesReportBySlice())

        End Function

        <HttpPost()>
        Function GetPOSSalesReportByCustomer() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("ReportPOS", clsDashBoardsAndReports.GetPOSSalesReportByCustomer())

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

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization


            dtSecurity = mSecurity.CheckSecurityCurrentUserAcccess("Show Dashboard First Page", Common.WebRequest.Data("OrgID"), WebSession.Instance.PeopleUserID)

            If dtSecurity.Rows.Count > 0 Then
                Return JsonDictionary.Serialize("Results", "True")
            Else
                Return JsonDictionary.Serialize("Results", "False")
            End If

        End Function

        <HttpPost()>
        Function IsViewAllMainPageInformation() As String
            Dim dtSecurity As DataTable
            Dim mSecurity As New SecurityRole

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization


            dtSecurity = mSecurity.CheckSecurityCurrentUserAcccess("On the Dashbooards and Reports Main Page, user is allowed to view all main page information?", Common.WebRequest.Data("OrgID"), WebSession.Instance.PeopleUserID)

            If dtSecurity.Rows.Count > 0 Then
                Return JsonDictionary.Serialize("Results", "True")
            Else
                Return JsonDictionary.Serialize("Results", "False")
            End If

        End Function

        <HttpPost()>
        Function IsRestrictedViewingMainDashboardConsolidatedSales() As String
            Dim dtSecurity As DataTable
            Dim mSecurity As New SecurityRole

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization


            dtSecurity = mSecurity.CheckSecurityCurrentUserAcccess("Restrict access to the consolidated sales data in the main dashboard", Common.WebRequest.Data("OrgID"), WebSession.Instance.PeopleUserID)

            If dtSecurity.Rows.Count > 0 Then
                Return JsonDictionary.Serialize("Results", "True")
            Else
                Return JsonDictionary.Serialize("Results", "False")
            End If

        End Function

        <HttpPost()>
        Function IsGroupHead() As String
            Dim dtSecurity As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization


            dtSecurity = Common.WebRequest.ProData.Execute("SELECT * FROM LibraryFranchiseGroupHeads WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID))

            If dtSecurity.Rows.Count > 0 Then
                Return JsonDictionary.Serialize("Results", "True")
            Else
                Return JsonDictionary.Serialize("Results", "False")
            End If

        End Function

        <HttpPost()>
        Public Function GroupInventoryLevels() As String
            ' bookmark: arvin, GroupInventoryLevels

            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            Dim pDT As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                If CBool2(Common.WebRequest.Data("Recache")) Then
                    'WebRequest.Instance.Business.TransInventoryLedger.CacheBalanceEachFowst(WebRequest.Instance.OrgID)
                    WebRequest.Instance.Business.TransInventoryLedger.CacheBalanceEachFowst()
                End If

                clsDashBoardsAndReports.OrgID = WebRequest.Instance.OrgID
                pDT = clsDashBoardsAndReports.GroupInventoryLevels(Common.WebRequest.Data("GroupID"))

                Return JsonDictionary.Serialize(pDT)
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", "vbT688YSW7VW! " & ex.Message)
            End Try

        End Function

        <HttpPost>
        Public Function GetReceiptPrintedStatus()

            Dim pFrom As String
            Dim pTo As String
            Dim pDT As DataTable
            Dim pFowstID As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pFrom = Common.WebRequest.Data("From")
            pTo = Common.WebRequest.Data("To")
            pFowstID = Common.WebRequest.Data("Fowst")

            pDT = WebRequest.Instance.Business.TransInvoice.GetReceiptPrintedStatus(pFrom, pTo, pFowstID)

            Return JsonDictionary.Serialize("Result", pDT)

        End Function

        <HttpPost()>
        Function GetIncomeStatement() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            Dim pDT As DataTable
            Dim pOut As JsonDictionary
            Dim Slice As String
            Dim pFilter1 As DataTable
            Dim SpliceFilter() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pDT = clsDashBoardsAndReports.GetIncomeStatement()

            pOut = New JsonDictionary

            pOut.Add("Results", pDT)

            Slice = WebRequest.Instance.Data("SliceBy")

            If Not Slice = vbNullString Then
                SpliceFilter = Split(Slice, ",")
                pFilter1 = CreateCrossTab(pDT, {SpliceFilter(0)}, {New CrossColumn("{Total}", "Total", "TotalAmount")})
                pOut.Add("Filter1", pFilter1)
            End If

            Return pOut.Serialize

        End Function
#End Region

#Region "POSTransactionDetail"
        Function POSTransactionDetail() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function GenerateNewAuditTrailData()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            Dim result As String = clsDashBoardsAndReports.GenerateNewAuditTrailData()

            Return JsonDictionary.Serialize("POSTransactionDetail", result)
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
            Try
                clsDashBoardsAndReports.CheckSumForDashboardAndPOSTransaction()
            Catch ex As Exception
                AppendToDebug("vbJ2QL5WUBVY! CheckSumForDashboardAndPOSTransaction", , ex, True)
            End Try
            WebRequest.Instance.Business.TransMemoSalesReturn.FixSalesReturnDeviceIdFowstId()
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
                CreateFolder(pFilename)
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

            Return "" 'For PC 5/27/2025

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
                        For i As Integer = 0 To dtOrganizations.Rows.Count - 1
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

            Return "" 'For PC 5/27/2025

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
                .SqlOrder = " ORDER by convert(varchar(50),convert(date,RegisteredDate)) desc, convert(time,RegisteredDate)  desc "

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
        Function GetSalesOrderAreas() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDT = WebRequest.Instance.Business.LibraryStock.GetAreas(WebRequest.Instance.FowstID)
            Return JsonDictionary.Serialize(pDT)

        End Function

        <HttpPost()>
        Function GetSalesOrderReport() As String

            Try
                Dim d As New DashBoardsAndReports
                If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
                If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

                d.OrgID = WebRequest.Instance().OrgID
                Dim pCacheMode As DashBoardsAndReports.eSalesOrderReportMode
                If [Enum].TryParse(Of DashBoardsAndReports.eSalesOrderReportMode)(WebRequest.Instance.Data("CacheMode"), True, pCacheMode) = False Then
                    pCacheMode = DashBoardsAndReports.eSalesOrderReportMode.QueryNormally
                End If

                If pCacheMode = DashBoardsAndReports.eSalesOrderReportMode.Recache Then
                    d.GetSalesOrderReportFinder("", "", DashBoardsAndReports.eSalesOrderReportMode.Recache,
                        vFowstID:=Nothing,
                        vStatus:=Nothing,
                        vIsInVendorPortal:=False,
                        vOrderDateFrom:=Nothing,
                        vOrderDateTo:=Nothing,
                        vVendorID:=Nothing,
                        vAreas:=Nothing,
                        vSortBy:=Nothing)
                    pCacheMode = DashBoardsAndReports.eSalesOrderReportMode.QueryFromCache
                End If
                Return JsonDictionary.Serialize("SalesOrderReport", d.GetSalesOrderReport(Common.WebRequest.Data("SearchString"), pCacheMode))
            Catch ex As Exception
                AppendToDebug(, True, ex, True)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
            
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
            Dim pInvoiceID As String

            pInvoiceID = Common.WebRequest.Data("InvoiceID")
            If IsGuid(pInvoiceID) = False Then
                Return JsonDictionary.Serialize("Error", "InvoiceID must be GUID")
            End If
            pInvoiceID = ReadyGuid(pInvoiceID)

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            ' old code:  pDT = clsDashBoardsAndReports.GetRawSalesOrderData(Common.WebRequest.Data("FowstID"), pInvoiceID, Common.WebRequest.Data("SalesOrderGroupNo"), "", "")
            pDT = clsDashBoardsAndReports.GetRawSalesOrderData(Nothing, pInvoiceID, Common.WebRequest.Data("SalesOrderGroupNo"), "", "")
            If (pDT.Rows.Count > 0) Then
                If Not System.IO.Directory.Exists(pSourceURL) Then
                    IO.Directory.CreateDirectory(pSourceURL)
                End If

                strFileName = "SaleOrderData-" & pInvoiceID & ".csv"

                If (IO.File.Exists(pSourceURL & "\SalesOrder\" & strFileName)) Then
                    IO.File.Delete(pSourceURL & "\SalesOrder\" & strFileName)
                End If

                TableToExcelFile(pDT, "", "", pSourceURL & "\" & strFileName)

            End If

            Return JsonDictionary.Serialize("SalesOrderDetail", strFileName)

        End Function

        <HttpPost>
        Public Function GetSopaDataToPrint() As String

            Dim pInvoiceID As String = Common.WebRequest.Data("InvoiceID")
            Dim pDS As DataSet

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If IsGuid(pInvoiceID) = False Then
                Return JsonDictionary.Serialize("Error", "InvoiceID must be a GUID.")
            End If
            pInvoiceID = ReadyGuid(pInvoiceID)

            pDS = WebRequest.Instance.Business.TransInvoice.GetReportData(" WHERE TransInvoice.ID = " & ReadyString(pInvoiceID), False, , , , True)

            Dim p As New JsonDictionary
            For Each pDR As DataRow In pDS.Tables("Invoices").Rows
                If CInt2(pDR("SalesOrderNo")) > 0 Then
                    pDR("PurchaseOrderNo") = pDR("SalesOrderNo")
                End If
                If CInt2(pDR("PickListPrintCount")) >= WebRequest.Instance.Business.SystemDefaults.PickListPrintCount _
                    AndAlso New SecurityAccess().CheckAccessRights(WebSession.Instance.PeopleUserID, "Reprint Pick List", False) = False Then

                    Return JsonDictionary.Serialize("Error", "Pick list for this transaction was already printed.")
                End If
            Next pDR
            p.Add("Invoices", pDS.Tables("Invoices"))
            p.Add("Stocks", pDS.Tables("Stocks"))
            WebRequest.Instance.Business.TransInvoice.IncrementPickListPrintCount(pInvoiceID)

            Return p.Serialize()

        End Function

        <HttpPost>
        Public Function CreateSopaPdf() As String

            Dim pInvoiceID As String = Common.WebRequest.Data("InvoiceID")
            Dim pDS As DataSet
            Dim pReport As ProReport
            Dim pBaseFilename As String
            Dim pFilename As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If IsGuid(pInvoiceID) = False Then
                Return JsonDictionary.Serialize("Error", "InvoiceID must be a GUID.")
            End If
            pInvoiceID = ReadyGuid(pInvoiceID)

            pDS = WebRequest.Instance.Business.TransInvoice.GetReportData(" WHERE TransInvoice.ID = " & ReadyString(pInvoiceID), False)
            pReport = New ProReport
            pReport.AutoFillHeader = False
            pReport.SourceFilename = "WarehousePickList"
            For Each pDR As DataRow In pDS.Tables("Invoices").Rows
                If CInt2(pDR("SalesOrderNo")) > 0 Then
                    pDR("PurchaseOrderNo") = pDR("SalesOrderNo")
                End If
            Next pDR
            pReport.AddTable("Master", pDS.Tables("Invoices"))
            pReport.AddTable("Products", pDS.Tables("Stocks"))
            pBaseFilename = CreateValidFilename(pInvoiceID) & "-" & DateTimeStamp()
            pFilename = Common.ServerPath & "temp\SopaPdfs\" & pBaseFilename & ".pdf"
            CreateFolder(pFilename)
            pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)
            pReport.ExportXmlFile()

            Return JsonDictionary.Serialize("Basefilename", pBaseFilename)

        End Function

        <HttpPost>
        Public Function GetSopaPrintData() As String

            Dim pInvoiceID As String = Common.WebRequest.Data("InvoiceID")
            Dim pDS As DataSet

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If IsGuid(pInvoiceID) = False Then
                Return JsonDictionary.Serialize("Error", "InvoiceID must be a GUID.")
            End If
            pInvoiceID = ReadyGuid(pInvoiceID)

            pDS = WebRequest.Instance.Business.TransInvoice.GetReportData(" WHERE TransInvoice.ID = " & ReadyString(pInvoiceID), False)
            Dim pOut = New JsonDictionary()

            pOut.Add("Master", pDS.Tables("Invoices"))
            pOut.Add("Products", pDS.Tables("Stocks"))

            Return pOut.Serialize()

        End Function

        <HttpGet>
        Public Function GetSopaPdf() As ActionResult

            Dim pFilename As String

            pFilename = Common.ServerPath & "temp\SopaPdfs\" & Common.WebRequest.QueryString("Basefilename") & ".pdf"

            Return File(pFilename, "application/pdf")

        End Function

        Function DownloadSalesOrderCsvFile() As ActionResult
            Dim pSourceFileName = Common.WebRequest.QueryString("SourceFileName")
            Dim pDestinationFilename = Common.WebRequest.QueryString("DestinationFilename")
            Dim pSourceURL As String = Common.ServerPath & "\temp\SalesOrder\" & pSourceFileName
            Dim pContentType As String = "application/csv"

            Try
                pDestinationFilename = HttpUtility.UrlDecode(pDestinationFilename) & ".csv"
                Return File(pSourceURL, pContentType, pDestinationFilename)
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

            Try
                Common.WebRequest.ProData.BeginTransaction()
                pSql = "Update TransInvoice set " & vbCrLf &
                  "InvoiceNumber = " & CInt(Common.WebRequest.Data("InvoiceNumber")) & vbCrLf &
                  ", InvoiceType = " & ReadyString(Common.WebRequest.Data("InvoiceType")) & vbCrLf &
                  ", Remarks = " & ReadyString(Common.WebRequest.Data("Remarks")) & vbCrLf &
                  ", IsDraft = 0" & vbCrLf &
                  " Where ID = " & ReadyString(Common.WebRequest.Data("ID"))
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                If WebRequest.Instance.Business.TransReceiving.CreateFromInvoice(Common.WebRequest.Data("ID"), pSql, WebRequest.Instance.OrgID) Then
                    Common.WebRequest.ProData.CommitTransaction()
                    Return "true"
                Else
                    Common.WebRequest.ProData.RollbackTransaction(, True)
                    Return pSql
                End If
            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction(, True)
                Return ex.Message
            End Try

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

        Function GetCustomerTransactionInfoForCRM() As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports
            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.GetCustomerTransactionInfoForCRM())
        End Function

        Function GetCustomerInfoFromCRM() As String
            Dim webClient As New Net.WebClient
            Dim pJsonString As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            pJsonString = webClient.DownloadString("http://localhost:8080/DashboardsAndReports/GetCustomerTransactionInfoForCRM?OrgID=CFE39DF6-8809-4973-9515-810DF75AED12&CustomerID=cd71a0f7-08a9-4601-876e-5f13c064f873&DateFrom=2016-03-22&DateTo=2016-03-22")

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.CustomerInfoFromCRM(pJsonString))

        End Function

        Function GetCustomerList() As String
            Dim webClient As New Net.WebClient
            Dim pJsonString As String
            Dim clsDashBoardsAndReports As New DashBoardsAndReports

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSourceFile As String = Common.ServerPath & "data\CustomerList.xls"

            webClient.DownloadFile("http://localhost:8080/Organization/DownloadOrganizationCustomers?OrgID=CFE39DF6-8809-4973-9515-810DF75AED12&CustomerID=cd71a0f7-08a9-4601-876e-5f13c064f873&DateFrom=2016-03-22&DateTo=2016-03-22", pSourceFile)

            Return JsonDictionary.Serialize("Results", clsDashBoardsAndReports.CustomerInfoFromCRM(pJsonString))

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


        'Private Function GetInvoiceItemsID(vDT As DataTable) As String
        '    Dim pStr As String

        '    pStr = CStr2(vDT.Rows(0).Item("InvoiceItemsID"))

        '    For i As Integer = 1 To vDT.Rows.Count - 1
        '        pStr &= "," & CStr2(vDT.Rows(0).Item("InvoiceItemsID"))
        '    Next

        '    Return pStr
        'End Function

        <HttpPost>
        Public Function GetProfitData() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pGroupBy As TransInvoice.eProfitDataGroup

            Select Case WebRequest.Instance.Data("GroupBy")
                Case "Customer"
                    pGroupBy = TransInvoice.eProfitDataGroup.Customer

                Case "Sku"
                    pGroupBy = TransInvoice.eProfitDataGroup.Sku

            End Select

            Dim pYears = New List(Of Integer) From {Now().Year - 1, Now().Year}
            Dim pDT = WebRequest.Instance.Business.TransInvoice.GetProfitData(pGroupBy, TransInvoice.eProfitDataTimeHorizon.Year, pYears)

            Dim pOut = New JsonDictionary()
            pOut.Add("Table", pDT)
            pOut.Add("YearFrom", pYears(0))
            pOut.Add("YearTo", pYears(pYears.Count - 1))

            Return pOut.Serialize()

        End Function

        <HttpPost>
        Public Function GetSalesQuotes() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDateFrom = ParseDate(Common.WebRequest.Data("DateFrom"))
            If pDateFrom Is Nothing Then
                pDateFrom = Now
            End If
            Dim pDateTo = ParseDate(Common.WebRequest.Data("DateTo"))
            If pDateTo Is Nothing Then
                pDateTo = Now
            End If
            Dim pOut = WebRequest.Instance.Business.TransInvoice.GetCountAndAmount("Quote", "QuotationNo", WebRequest.Instance.Data("FowstID"), pDateFrom, pDateTo, "OrderedAmount")

            Return JsonConvert.SerializeObject(pOut)

        End Function

        <HttpPost>
        Public Function GetSalesOrders() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDateFrom = ParseDate(Common.WebRequest.Data("DateFrom"))
            If pDateFrom Is Nothing Then
                pDateFrom = Now
            End If
            Dim pDateTo = ParseDate(Common.WebRequest.Data("DateTo"))
            If pDateTo Is Nothing Then
                pDateTo = Now
            End If
            Dim pOut = WebRequest.Instance.Business.TransInvoice.GetCountAndAmount("SalesOrder", "SalesOrderNo", WebRequest.Instance.Data("FowstID"), pDateFrom, pDateTo, "OrderedAmount")

            Return JsonConvert.SerializeObject(pOut)

        End Function

        <HttpPost>
        Public Function GetSalesInvoices() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDateFrom = ParseDate(Common.WebRequest.Data("DateFrom"))
            If pDateFrom Is Nothing Then
                pDateFrom = Now
            End If
            Dim pDateTo = ParseDate(Common.WebRequest.Data("DateTo"))
            If pDateTo Is Nothing Then
                pDateTo = Now
            End If
            Dim pOut = WebRequest.Instance.Business.TransInvoice.GetCountAndAmount("Invoice", "InvoiceNumber", WebRequest.Instance.Data("FowstID"), pDateFrom, pDateTo, "ServedAmount")

            Return JsonConvert.SerializeObject(pOut)

        End Function

        <HttpPost>
        Public Function GetSalesBySalesRep() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDateFrom = ParseDate(Common.WebRequest.Data("InvoiceDateFrom"))
            If pDateFrom Is Nothing Then
                pDateFrom = Now
            End If
            Dim pDateTo = ParseDate(Common.WebRequest.Data("InvoiceDateTo"))
            If pDateTo Is Nothing Then
                pDateTo = Now
            End If
            Dim pDT = WebRequest.Instance.Business.TransInvoice.GetSalesBySalesRep(pDateFrom, pDateTo)
            Dim pReport = New ProReport
            pReport.AutoFillHeader = False
            pReport.SourceFilename = "SalesBySalesReps"
            pReport.AddTable("Master", pDT)
            pReport.ExportXmlFile()
            Dim pBaseFilename = "SalesBySalesReps-" & DateTimeStamp() & ".pdf"
            Dim pFilename = Common.ServerPath & "temp\PrintablePdf\" & pBaseFilename
            CreateFolder(pFilename)
            pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)

            Return JsonDictionary.Serialize("BaseFilename", pBaseFilename)

        End Function

        Function GetSeniorCitizenPWDInfo() As String
            Dim clsDashboard As New DashBoardsAndReports

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDashboard.InvoiceID = CStr2(Common.WebRequest.Data("InvoiceID"))

            Return JsonDictionary.Serialize("Results", clsDashboard.GetSeniorCitizenPWDInfo())
        End Function

        Function GetVouchersReport() As Object
            Dim clsOrganization As New Organization
            Dim pDT As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                pDT = GetVouchersReportData(False)

                Dim pDTCountPage As DataTable = GetVouchersReportData(True)

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsOrganization.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                AppendToLog(ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />"))
                Return JsonDictionary.Serialize("Error", ex.Message & GetStackTrace(, ex).Replace(vbCrLf, "<br />"))
            End Try
        End Function

        Function GetVouchersReportData(ByVal IsCountPage As Boolean) As DataTable
            Dim clsDashboard As New DashBoardsAndReports

            clsDashboard.OrgID = Request.Params("OrgID")
            clsDashboard.PageStart = CInt(Request.Form.GetValues("start")(0))
            clsDashboard.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            clsDashboard.PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
            clsDashboard.IsCountPage = IsCountPage
            clsDashboard.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            clsDashboard.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
            clsDashboard.Status = Request.Params("Status")
            Return clsDashboard.GetVouchersReport()
        End Function

        Function DownloadVouchers()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim status As String = Request.Params.Get("Status")
                Dim pSql As String
                Dim whereSearch As String
                Dim pSourceURL As String = Common.ServerPath & "temp\Vouchers"

                If status = "All" Then
                    whereSearch = " WHERE (voucher.IsActive = 0 OR voucher.IsActive = 1)"
                ElseIf status = "Redeemed" Then
                    whereSearch = " WHERE voucher.IsActive = 0"
                ElseIf status = "Unredeemed" Then
                    whereSearch = " WHERE voucher.IsActive = 1"
                End If

                pSql = " SELECT TransactionDate as [Date], ReferenceNo as [Voucher Code], VoucherDescription as [Type], CASE WHEN voucher.IsActive = 0 THEN 'Redeemed' ELSE 'Unredeemed' END as [Status] " & vbCrLf &
                " , (SELECT TOP 1 GroupName " & vbCrLf &
                "   FROM LibraryFranchiseGroup as groups " & vbCrLf &
                "   INNER JOIN MobizLibraryOrganizationFranchisee as franchisee ON franchisee.FranchiseGroupID = groups.ID " & vbCrLf &
                "   AND franchisee.FranchiseeOrgFowstID = inv.FowstID) as Region " & vbCrLf &
                " , fowst.Name as [Store Name], fowst.Code as [Store Code] " & vbCrLf &
                " --, '' as SupercardNumber " & vbCrLf &
                " FROM MobizLibraryVouchers as voucher " & vbCrLf &
                " LEFT JOIN CacheInvoices as inv ON inv.ID = voucher.TransactionID " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = inv.FowstID " & vbCrLf &
                " " & whereSearch & vbCrLf &
                " ORDER BY TransactionDate DESC "

                If Not System.IO.Directory.Exists(pSourceURL) Then
                    IO.Directory.CreateDirectory(pSourceURL)
                End If

                Dim strFileName = "Vouchers Report as of " & Format(CDate(Date.Now()), "MM-dd-yyyy") & ".xls"

                If (IO.File.Exists(pSourceURL & "\" & strFileName)) Then
                    IO.File.Delete(pSourceURL & "\" & strFileName)
                End If

                TableToExcelFile(Common.WebRequest.ProData.Execute(pSql), "", "", pSourceURL & "\" & strFileName)

                Dim file As New System.IO.FileInfo(pSourceURL & "\" & strFileName)

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=" & strFileName)
                Response.AddHeader("Content-Length", File.Length.ToString())
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/")
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(File.FullName)
                Response.End()

                Return JsonDictionary.Serialize("success", file.FullName)
            Catch ex As Exception
                Return JsonDictionary.Serialize("error", ex.Message)
            End Try
        End Function
    End Class
End Namespace