Namespace WebProj
    Public Class MallLessorSMController
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
            Dim pOrgID As String
            Dim pFowstID As String
            Dim pContentType As String
            Dim pSourceURL As String
            Dim pLoginHash As String
            Dim pDR As DataRow

            pOrgID = Common.WebRequest.QueryString("ReportOrgID")
            pFowstID = Common.WebRequest.QueryString("ReportFowstID")
            pFileTitle = Common.WebRequest.QueryString("ReportLink")

            pLoginHash = Common.WebRequest.QueryString("LoginHash")
            If (Not pLoginHash Is Nothing) Then
                If CStr2(pLoginHash).Length <> 50 Then
                    If WebRequest.Instance.HandleAccess() = False Then Exit Function
                Else
                    pDR = MallLessorController.LoginData.GetByLoginHash(pLoginHash)
                    If pDR Is Nothing Then
                        Return Nothing
                    End If
                End If
            End If

            pSourceURL = Common.ServerPath & "temp\" & pOrgID & "\" & pFowstID & "\" & pFileTitle
            pContentType = "application/zip"

            Return File(pSourceURL, pContentType, pFileTitle)

        End Function

        <HttpPost()>
        Function GenerateReportFile() As String
            Dim strGeneratedLink As String
            Dim clsMallLessorSM As New MallLessorSM
            Dim dtaTable As DataTable
            Dim dtDate As Date
            Dim blnIsGenerateFlg As Boolean
            Dim strTenantCode As String
            Dim strTenantName As String
            Dim strReturn As String

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

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

            blnIsGenerateFlg = Convert.ToBoolean(Convert.ToInt32(Common.WebRequest.Data("FlagClose")))

            strGeneratedLink = clsMallLessorSM.GenerateLessorSalesReport(dtDate, blnIsGenerateFlg, "", "")

            strReturn = JsonDictionary.Serialize("GeneratedReport", strGeneratedLink)

            Return strReturn

        End Function

        Function GetOldGT() As String
            Dim dtDateFrom As Date
            Dim clsMallLessorSM As MallLessorSM
            Dim dtaHSaleTable As DataTable

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If (Common.WebRequest.Data("RangeType") = "Today") Then
                dtDateFrom = DateTime.Today
            ElseIf (Common.WebRequest.Data("RangeType") = "Yesterday") Then
                dtDateFrom = DateTime.Today.AddDays(-1)
            ElseIf (Common.WebRequest.Data("RangeType") = "By Date") Then
                dtDateFrom = Convert.ToDateTime(Common.WebRequest.Data("SalesDate"))
            End If

            Dim pFinder As Finder

            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData
                With .Fields
                    With .Add("TSL_OLD_A", SqlDbType.VarChar)
                        .Expression = "CONVERT(VARCHAR, " &
                                        "CAST(" &
                                            "ROUND(" &
                                                "(ISNULL(devf.BegBalance,0) + " &
                                                " ISNULL((SELECT SUM(custpay.TotalAmountDue) " &
                                                " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                                " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                                " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                                " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                                                " AND inv.InvoiceDate  < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                                " AND inv.FOWSTID = devf.FowstID " &
                                                " AND inv.DeviceID = devf.DeviceID) " &
                                                ",0) - " &
                                                " ISNULL((SELECT SUM(custpay.DiscountAmount) " &
                                                " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                                                " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                                                " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                                                " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                                                " AND inv.InvoiceDate  < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                                " AND inv.FOWSTID = devf.FowstID " &
                                                " AND inv.DeviceID = devf.DeviceID) " &
                                                ",0) - " &
                                                "ISNULL((SELECT SUM(srledger.GrossRefundAmount) " &
                                                " FROM TransSalesReturnLedger srledger " &
                                                " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                                " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                                " AND memo.FOWSTID = devf.FowstID " &
                                                " AND memo.DeviceID = devf.DeviceID) " &
                                                ",0) - " &
                                                "ISNULL((SELECT SUM(srledger.GrossSrvAmount) " &
                                                " FROM TransSalesReturnLedger srledger " &
                                                " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                                " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                                " AND memo.FOWSTID = devf.FowstID " &
                                                " AND memo.DeviceID = devf.DeviceID) " &
                                                ",0) - " &
                                                "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " &
                                                " FROM TransSalesReturnLedger srledger " &
                                                " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " &
                                                " WHERE memo.LastUpdated < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                                " AND memo.FOWSTID = devf.FowstID " &
                                                " AND memo.DeviceID = devf.DeviceID) " &
                                                ",0) +" &
                                                  "ISNULL((SELECT SUM(ISNULL(charge.TaxAmount, 0)) " &
                                                  " FROM dbo.TransInvoice inv " &
                                                  " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                                  " WHERE inv.InvoiceNumber > 0 " &
                                                   " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                                  " AND inv.FOWSTID = devf.FowstID " &
                                                  " AND inv.DeviceID = devf.DeviceID)" &
                                                  ",0) +" &
                                                   "ISNULL((SELECT SUM(ISNULL(charge.ChargeAmount, 0)) " &
                                                  " FROM dbo.TransInvoice inv " &
                                                  " INNER JOIN dbo.TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) " &
                                                  " WHERE inv.InvoiceNumber > 0 " &
                                                  " AND inv.InvoiceDate < " & ReadyString(dtDateFrom.ToString("MM/dd/yyyy hh:mm:ss tt")) &
                                                  " AND inv.FOWSTID = devf.FowstID " &
                                                  " AND inv.DeviceID = devf.DeviceID)" &
                                                  ",0)" &
                                                " ) " &
                                            ",2) " &
                                        " as DECIMAL(12,2))" &
                                    ",0) "
                        .Searchable = False
                    End With
                End With


                .SqlFrom = " FROM dbo.LibraryDeviceFowst devf " &
                        " INNER JOIN dbo.LibraryDevice dev ON (devf.DeviceID = dev.ID) " &
                        " INNER JOIN dbo.MobizLibraryOrganizationFOWSTMallInfoSM sm  ON (sm.FOWSTID = devf.FowstID)"

                .SqlWhere = " WHERE devf.FowstID = " & ReadyString(Common.WebRequest.Data("FOWSTID")) &
                    " AND devf.DeviceID = " & ReadyString(Common.WebRequest.Data("DeviceID")) &
                    " AND dev.IsActive != 0 " &
                    " AND ISNULL(devf.SMMachineNo,'') != ''"

                Return JsonDictionary.Serialize("Results", .Requery)

            End With

        End Function
    End Class
End Namespace