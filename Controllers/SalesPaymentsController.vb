Public Class SalesPaymentsController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function BsIndex() As ActionResult
        'bootstrap theme
        Return View()
    End Function

    Function EmailSalesReceipt() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function DuplicateReceipt() As ActionResult
        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function BsDuplicateReceipt() As ActionResult
        'bootstrap theme
        Return View()
    End Function

    Function CheckPayment() As ActionResult

        Return View()

    End Function
    Function DebitCardPayment() As ActionResult

        Return View()

    End Function
    Function CreditCardPayment() As ActionResult

        Return View()

    End Function
    Function SRVPayment() As ActionResult

        Return View()

    End Function
    Function GiftCertPayment() As ActionResult

        Return View()

    End Function
    Function OtherPaymentType() As ActionResult

        Return View()

    End Function
    Function SeniorCitizenPWD() As ActionResult

        Return View()

    End Function
    Function Payment() As ActionResult

        Return View()

    End Function

    <HttpPost()>
    Function getInvoiceTotalAmountDue() As String
        Dim pOut As New JsonDictionary
        Dim clsSalesPayments As New SalesPayments
        Dim strID As String
        Dim pDT As DataTable
        Dim dblTotalAmountDue As Double
        Dim dblTotalAmountDueForDiscount As Double

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("InvoiceID")
        pDT = clsSalesPayments.getInvoiceTotalAmountDue(strID)

        dblTotalAmountDue = 0
        dblTotalAmountDueForDiscount = 0
        If (pDT.Rows.Count > 0) Then

            WebSession.Instance.OrganizationID = pDT.Rows(0).Item("CustomerID").ToString()

            If (IsNumeric(pDT.Rows(0).Item("TotalAmountDue"))) Then
                dblTotalAmountDue = Convert.ToDouble(pDT.Rows(0).Item("TotalAmountDue"))
            End If

            If (IsNumeric(pDT.Rows(0).Item("TotalAmountDueAllowedForDiscount"))) Then
                dblTotalAmountDueForDiscount = Convert.ToDouble(pDT.Rows(0).Item("TotalAmountDueAllowedForDiscount"))
            End If

            pOut.Add("RefNum", CStr2(pDT.Rows(0)("RefNum")))
            pOut.Add("TransInvoiceItems", CStr2(pDT.Rows(0)("TransInvoiceItems")))
            pOut.Add("TaxPercent", CStr2(pDT.Rows(0)("TaxPercent")))
        End If

        pOut.Add("CustomerID", WebSession.Instance.OrganizationID)
        pOut.Add("TotalAmountDue", dblTotalAmountDue)
        pOut.Add("TotalAmountDueAllowedForDiscount", dblTotalAmountDueForDiscount)
        With WebRequest.Instance.Business.TransInvoice.CartSummary(strID)
            pOut.Add("SkuCount", .SkuCount)
            pOut.Add("TotalQuantity", .Qty)
        End With

        Return pOut.Serialize

    End Function

    <HttpPost()>
    Function getInvoiceDetails() As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String
        Dim strInvNo As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strInvNo = Common.WebRequest.Data("InvoiceNo")
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getInvoiceDetails(strInvNo, WebRequest.Instance.FowstID, WebRequest.Instance.DeviceID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getCustomerEmails() As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String
        Dim strID As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("CustomerID")
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getCustomerEmails(strID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSalesPaymentCheckList() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = ""
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getSalesPaymentCheckList(strID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSalesPaymentDebitCardList() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = ""
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getSalesPaymentDebitCardList(strID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSalesPaymentCreditCardList() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = ""
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getSalesPaymentCreditCardList(strID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSalesPaymentSRVList() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = ""
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getSalesPaymentSRVList(strID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSalesPaymentSRVInfo() As String
        Dim pOut As New JsonDictionary
        Dim dblSRVAmount As Decimal
        Dim strMemoID As String
        Dim strSRVNumber As String
        Dim strSRVPin As String
        Dim pDestinationPaymentCount As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strMemoID = ""
        strSRVNumber = Common.WebRequest.Data("SRVNumber")
        strSRVPin = Common.WebRequest.Data("SRVPin")

        dblSRVAmount = WebRequest.Instance.Business.TransSalesReturnLedger.GetRemainingValue(strSRVNumber, strSRVPin, strMemoID)
        If String.IsNullOrEmpty(strMemoID) = False Then
            pDestinationPaymentCount = WebRequest.Instance.Business.TransSalesReturnLedger.DestinationPaymentCount(strMemoID)
        End If

        pOut.Add("MemoID", strMemoID)
        pOut.Add("SRVAmount", dblSRVAmount)
        pOut.Add("DestinationPaymentCount", pDestinationPaymentCount)

        Return pOut.Serialize

    End Function

    <HttpPost()>
    Function getSalesPaymentInfo() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("ID")
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getSalesPaymentInfo(strID))

        Return strReturn

    End Function
    <HttpPost()>
    Function SaveBillingDiscount() As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String
        Dim strEmail As String()
        Dim intCntr As Integer
        Dim pOut As JsonDictionary
        Dim dtaTable As DataTable
        Dim pDT As DataTable
        Dim dblTotalAmountDue As Double

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        clsSalesPayments.InvoiceID = Common.WebRequest.Data("InvoiceID")
        clsSalesPayments.TotalDiscountAmount = Common.WebRequest.Data("TotalDiscountAmount")
        clsSalesPayments.NoOfPeople = Common.WebRequest.Data("NoOfPeople")
        clsSalesPayments.NoOfSeniorCitizens = Common.WebRequest.Data("NoOfSeniorCitizens")
        clsSalesPayments.SalesPaymentDiscountList = Common.WebRequest.AllData("SalesPaymentDiscountList")
        clsSalesPayments.SalesPaymentSeniorCitizenPWDCustomerList = Common.WebRequest.AllData("SalesPaymentSeniorCitizenPWDList")


        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.SaveBillingDiscount())

        Return strReturn

    End Function

    <HttpPost()>
    Function SaveSalesPayments() As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String
        Dim strEmail As String()
        Dim intCntr As Integer
        Dim pOut As JsonDictionary
        Dim dtaTable As DataTable
        Dim pDT As DataTable
        Dim dblTotalAmountDue As Double
        Dim pError As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebRequest.Instance.HandleInvoiceCountByMonth(pError) = False Then Return JsonDictionary.Serialize("Error", pError)

        clsSalesPayments.ID = ""
        clsSalesPayments.FowstID = WebRequest.Instance.FowstID
        clsSalesPayments.SubscriberDeviceID = WebRequest.Instance.DeviceID
        clsSalesPayments.InvoiceID = Common.WebRequest.Data("InvoiceID")
        clsSalesPayments.ReceiptType = "OR"
        clsSalesPayments.ReceiptNumber = ""
        clsSalesPayments.CustomerID = Common.WebRequest.Data("CustomerID")

        clsSalesPayments.CashAmount = CDec2(Common.WebRequest.Data("CashAmount")).ToString()
        clsSalesPayments.CreditCardAmount = CDec2(Common.WebRequest.Data("CreditCardAmount")).ToString()
        clsSalesPayments.DebitCardAmount = CDec2(Common.WebRequest.Data("DebitCardAmount")).ToString()
        clsSalesPayments.CheckPaymentAmount = CDec2(Common.WebRequest.Data("CheckPaymentAmount")).ToString()
        clsSalesPayments.SRVAmount = CDec2(Common.WebRequest.Data("SRVAmount")).ToString()
        clsSalesPayments.DiscountAmount = CDec2(Common.WebRequest.Data("TotalDiscountAmount")).ToString()
        clsSalesPayments.OtherTypeAmount = CDec2(Common.WebRequest.Data("OtherTypeAmount")).ToString()
        clsSalesPayments.ChargeAmount = CDec2(Common.WebRequest.Data("ChargeAmount")).ToString()

        clsSalesPayments.TotalAmountDue = CDec2(Common.WebRequest.Data("TotalAmountDue")).ToString()
        clsSalesPayments.TotalAmountTendered = CDec2(Common.WebRequest.Data("TotalAmountTendered")).ToString()
        clsSalesPayments.CustomerChangeCash = CDec2(Common.WebRequest.Data("CustomerChangeCash")).ToString()
        clsSalesPayments.AmountChange = 0

        clsSalesPayments.IsForGovReport = Convert.ToString(Common.WebRequest.Data("IsForGovReport")).Trim()

        clsSalesPayments.SalesPaymentCheckList = Common.WebRequest.AllData("SalesPaymentCheckList")
        clsSalesPayments.SalesPaymentDebitCardList = Common.WebRequest.AllData("SalesPaymentDebitCardList")
        clsSalesPayments.SalesPaymentCreditCardList = Common.WebRequest.AllData("SalesPaymentCreditCardList")
        clsSalesPayments.SalesPaymentSRVList = Common.WebRequest.AllData("SalesPaymentSRVList")
        clsSalesPayments.SalesPaymentDiscountList = Common.WebRequest.AllData("SalesPaymentDiscountList")
        clsSalesPayments.SalesPaymentSeniorCitizenPWDCustomerList = Common.WebRequest.AllData("SalesPaymentSeniorCitizenPWDList")
        clsSalesPayments.SalesPaymentOtherTypeList = Common.WebRequest.AllData("SalesPaymentOtherTypeList")


        'additional by janine
        clsSalesPayments.NoOfPeople = CDec2(Common.WebRequest.Data("NoOfPeople")).ToString()
        clsSalesPayments.NoOfSeniorCitizens = CDec2(Common.WebRequest.Data("NoOfSeniorCitizens")).ToString()
        'end of additional


        If (clsSalesPayments.TotalAmountDue = "") Then
            strReturn = JsonDictionary.Serialize("SalesPaymentData", "InsufficientPayment")
        ElseIf (CDec2(clsSalesPayments.TotalAmountDue) <= 0) Then
            strReturn = JsonDictionary.Serialize("SalesPaymentData", "InsufficientPayment")
        ElseIf (CDec2(clsSalesPayments.TotalAmountTendered) < (CDec2(clsSalesPayments.TotalAmountDue) - CDec2(clsSalesPayments.DiscountAmount))) Then
            strReturn = JsonDictionary.Serialize("SalesPaymentData", "InsufficientPayment")
        Else

            ''' Check if there is a changes on the total amount due 
            pDT = clsSalesPayments.getInvoiceTotalAmountDue(Common.WebRequest.Data("InvoiceID"))
            dblTotalAmountDue = 0
            If (pDT.Rows.Count > 0) Then
                If (IsNumeric(pDT.Rows(0).Item("TotalAmountDue"))) Then
                    dblTotalAmountDue = CDec2(pDT.Rows(0).Item("TotalAmountDue"))
                End If
            End If

            If (clsSalesPayments.SalesPaymentDiscountList.Length > 0) Then
                For Each obj As Object In clsSalesPayments.SalesPaymentDiscountList
                    If (obj("ID").ToString().ToUpper() <> "055A0A33-DD98-47EC-ADD3-8B2009F3944A" And obj("ID").ToString().ToUpper() <> "674AD791-188B-4238-B95C-97B5C2B9D544") Then
                        If (dblTotalAmountDue <> CDec2(clsSalesPayments.TotalAmountDue)) Then
                            Return JsonDictionary.Serialize("SalesPaymentData", "Invoice has been changed")
                        End If
                    End If
                Next
            Else
                If (dblTotalAmountDue <> CDec2(clsSalesPayments.TotalAmountDue)) Then
                    Return JsonDictionary.Serialize("SalesPaymentData", "Invoice has been changed")
                End If
            End If


            '''Check if invoice has been saved already
            dtaTable = clsSalesPayments.IsInvoiceSaved(clsSalesPayments.InvoiceID)
            If (dtaTable.Rows.Count > 0) Then
                Return JsonDictionary.Serialize("SalesPaymentData", "DoubleEntry")
            End If
            If (Common.WebRequest.Data("EmailAddress").Trim().Length > 0) Then
                strEmail = Common.WebRequest.Data("EmailAddress").Trim().Split(",")
                intCntr = 0
                While (intCntr < strEmail.Length)

                    If (Not IsEmail(strEmail(intCntr).Trim())) Then
                        Return JsonDictionary.Serialize("SalesPaymentData", "InvalidEmail")
                    End If

                    intCntr = intCntr + 1
                End While
            End If

            strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.SaveSalesPayments(WebRequest.Instance.OrgID))

        End If

        Return strReturn

    End Function

    <HttpPost()>
    Function UpdateInvoicePrintCount() As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String
        Dim pOut As JsonDictionary

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        clsSalesPayments.InvoiceID = Common.WebRequest.Data("InvoiceID")

        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.UpdateInvoicePrintCount())

        Return strReturn

    End Function

    <HttpPost()>
    Function SendEmailReceipt() As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String
        Dim dtaTable As DataTable
        Dim intCntr As Integer
        Dim strTemp As String
        Dim strInvoiceNo As String = ""
        Dim strEmail As String()
        Dim strBody As String = ""
        Dim strmReader As System.IO.StreamReader
        Dim varBusinessName As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        If (Common.WebRequest.Data("EmailAddress").Trim().Length > 0) Then
            strEmail = Common.WebRequest.Data("EmailAddress").Trim().Split(",")
            intCntr = 0
            While (intCntr < strEmail.Length)

                If (Not IsEmail(strEmail(intCntr).Trim())) Then
                    Return JsonDictionary.Serialize("SalesPaymentData", "InvalidEmail")
                End If

                intCntr = intCntr + 1
            End While
        End If

        strmReader = New System.IO.StreamReader(Server.MapPath("~/Views/SalesPayments/EmailSalesReceipt.vbhtml"))
        strBody = strmReader.ReadToEnd

        '''Get Header Info
        dtaTable = New DataTable()
        'dtaTable = clsSalesPayments.getReceiptHeaderInfo(WebRequest.Instance.FowstID)
        dtaTable = clsSalesPayments.getReceiptHeaderInfo(Common.WebRequest.Data("FowstID")) 'June 17 2020 for re-duplicate receipt via email

        If (dtaTable.Rows.Count > 0) Then
            'additional by janine. to show in email receipt
            If (dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim() <> "") Then
                varBusinessName = dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim()
            Else
                varBusinessName = dtaTable.Rows(0).Item("CorporateName").ToString().Trim()
            End If

            strBody = strBody.Replace("[TradeNameHere]", dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim())
            strBody = strBody.Replace("[CorporateNameHere]", dtaTable.Rows(0).Item("CorporateName").ToString().Trim())
            strBody = strBody.Replace("[TINHere]", "TIN " & dtaTable.Rows(0).Item("TIN").ToString().Trim())
            strBody = strBody.Replace("[AddressHere]", dtaTable.Rows(0).Item("CompleteAddress").ToString().Trim())
            strBody = strBody.Replace("[TelNoHere]", "Tel. No. " & dtaTable.Rows(0).Item("TelNo").ToString().Trim())

            strBody = strBody.Replace("[POSAccreditationNo]", "POS Accreditation No. " & dtaTable.Rows(0).Item("POSAccreditationNo").ToString().Trim())
            strBody = strBody.Replace("[POSRegistrationNo]", "POS Registration No. " & dtaTable.Rows(0).Item("POSPermitNo").ToString().Trim())
            strBody = strBody.Replace("[POSMachineNo]", "Machine ID No. (MIN) " & dtaTable.Rows(0).Item("MIN").ToString().Trim())

        End If

        '''Get Payment Info
        dtaTable = New DataTable()
        dtaTable = clsSalesPayments.getSalesPaymentInvoiceHeaderInfo(Common.WebRequest.Data("InvoiceID"))
        If (dtaTable.Rows.Count > 0) Then

            strBody = strBody.Replace("[CustomerNameHere]", "Customer : " & dtaTable.Rows(0).Item("CustomerName").ToString().Trim())
            strBody = strBody.Replace("[CustomerAddressHere]", "Address : " & dtaTable.Rows(0).Item("OfficeAddress").ToString().Trim())
            strBody = strBody.Replace("[CustomerTINHere]", "TIN : " & dtaTable.Rows(0).Item("CustomerTIN").ToString().Trim())

            strBody = strBody.Replace("[CustomerBillingAddress]", "Billing Address : " & dtaTable.Rows(0).Item("OfficeAddress").ToString().Trim())
            strBody = strBody.Replace("[CustomerDeliveryAddress]", "Delivery Address : " & dtaTable.Rows(0).Item("OfficeAddress").ToString().Trim())

            strInvoiceNo = Convert.ToInt32(dtaTable.Rows(0).Item("InvoiceNumber")).ToString("000000000000").Trim()

            strBody = strBody.Replace("[InvoiceNoHere]", "Sales Invoice No. " & Convert.ToInt32(dtaTable.Rows(0).Item("InvoiceNumber")).ToString("000000000000").Trim())
            strBody = strBody.Replace("[InvoiceDateTimeHere]", "Date/Time : " & Convert.ToDateTime(dtaTable.Rows(0).Item("InvoiceDate")).ToString("MMM dd, yyyy HH:mm").Trim())
            strBody = strBody.Replace("[CashierNameHere]", "Cashier : " & dtaTable.Rows(0).Item("CashierName").ToString().Trim())

        End If

        '''Get Payment Items
        dtaTable = New DataTable()
        dtaTable = clsSalesPayments.getSalesPaymentReceiptBodyInfo(Common.WebRequest.Data("InvoiceID"))
        If (dtaTable.Rows.Count > 0) Then

            strTemp = "<tbody>"
            intCntr = 0
            While (intCntr < dtaTable.Rows.Count)

                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 35%; height:30px; border: 1px dotted Gray;"">" & dtaTable.Rows(intCntr).Item("StockDescription").ToString().Trim() & "</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 10%; height:30px; border: 1px dotted Gray;"">" & Convert.ToInt32(dtaTable.Rows(intCntr).Item("Quantity")).ToString("#,##0").Trim() & "</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 15%; height:30px; border: 1px dotted Gray;"">" & dtaTable.Rows(intCntr).Item("UnitMeasure").ToString().Trim() & "</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:30px; border: 1px dotted Gray;"">" & Convert.ToDouble(dtaTable.Rows(intCntr).Item("UnitPrice")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:30px; border: 1px dotted Gray;"">" & Convert.ToDouble(dtaTable.Rows(intCntr).Item("SubTotal")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

                intCntr = intCntr + 1

            End While

            strTemp = strTemp & "</tbody>"

            strBody = strBody.Replace("[BodyGoesHere]", strTemp)

        End If

        '''Get Payment Amount
        dtaTable = New DataTable()
        dtaTable = clsSalesPayments.getSalesPaymentReceiptFooterAmountsInfo(Common.WebRequest.Data("InvoiceID"))
        If (dtaTable.Rows.Count > 0) Then

            '''SKU and Amount due
            strTemp = "<table style=""width:100%"">"
            strTemp = strTemp & "<thead>"
            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">SKU Count</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToInt32(dtaTable.Rows(0).Item("SKUCount")).ToString("#,##0").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Sub Total</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("SubTotal")).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            ' additional by janine feb 11 2015 
            If (IsSeniorCitizenDiscount(Common.WebRequest.Data("InvoiceID"))) Then
                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Less: " & dtaTable.Rows(0).Item("TaxPercent") & "% VAT</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(CDec2(dtaTable.Rows(0).Item("SubTotal")) - CDec2(dtaTable.Rows(0).Item("TotalAmountDue"))).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Total Amount</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TotalAmountDue")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

            End If

           
            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Discount Amount</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("DiscountAmount")).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            ' additional by janine feb 11 2015 
            If (WebRequest.Instance.Business.SystemDefaults.IsEnableBillingOption = "1") Then
                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Charges</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("ChargeAmount")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Tip</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TipAmount")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

            End If

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Net Amount Due</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(CDec2(dtaTable.Rows(0).Item("NetAmountDue")) + CDec2(dtaTable.Rows(0).Item("ChargeAmount")) + CDec2(dtaTable.Rows(0).Item("TipAmount"))).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "</thead>"
            strTemp = strTemp & "</table>"

            strTemp = strTemp & "<br />"

            '''VAT Info
            strTemp = strTemp & "<table style=""width:100%"">"
            strTemp = strTemp & "<thead>"
            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT-able Sales</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TaxSales")).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT Exempt Sales</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">0.00</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT Zero-Rated Sales</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">0.00</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT Amount</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TaxAmount")).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "</thead>"
            strTemp = strTemp & "</table>"

            strBody = strBody.Replace("[FooterGoesHere]", strTemp)

        End If

        '''Get Payment Details
        dtaTable = New DataTable()
        dtaTable = clsSalesPayments.getSalesPaymentReceiptPaymentInfo(Common.WebRequest.Data("InvoiceID"))
        If (dtaTable.Rows.Count > 0) Then

            strBody = strBody.Replace("[CASHPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("CashPayment")) <= 0, "", "Cash        : " & Convert.ToDouble(dtaTable.Rows(0).Item("CashPayment")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[CHECKPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("CheckPaymentAmount")) <= 0, "", "Check       : " & Convert.ToDouble(dtaTable.Rows(0).Item("CheckPaymentAmount")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[CREDITCARDPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("CreditCardAmount")) <= 0, "", "Credit Card : " & Convert.ToDouble(dtaTable.Rows(0).Item("CreditCardAmount")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[DEBITCARDPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("DebitCardAmount")) <= 0, "", "Debit Card : " & Convert.ToDouble(dtaTable.Rows(0).Item("DebitCardAmount")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[SRVPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("SRVAmount")) <= 0, "", "SRV         : " & Convert.ToDouble(dtaTable.Rows(0).Item("SRVAmount")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[CHANGEAMOUNT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("CustomerChangeCash")) <= 0, "", "Change      : " & Convert.ToDouble(dtaTable.Rows(0).Item("CustomerChangeCash")).ToString("#,##0.00").Trim()))

        End If

        SendMail("customercare@mobiz-apps.com",
                 Common.WebRequest.Data("EmailAddress").Trim(),
                 "Mobiz",
                 varBusinessName & " - Inv. No. " & strInvoiceNo,
                 strBody,
                 "smtp.live.com",
                 587,
                 "customercare@mobiz-apps.com",
                 "MobizCustCare2221", , , , , True)

        strReturn = JsonDictionary.Serialize("SalesPaymentData", "Sent")

        Return strReturn

    End Function

    Function IsSeniorCitizenDiscount(ByVal vInvoiceID As String) As Boolean
        Dim vDiscountType As String

        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT DiscountTypeID FROM TransInvoiceDiscount WHERE InvoiceID = " & ReadyString(vInvoiceID)
        pDT = Common.WebRequest.ProData.Execute(pSql)

        If (pDT.Rows.Count > 0) Then
            vDiscountType = pDT.Rows(0).Item("DiscountTypeID").ToString()

            If (vDiscountType.ToUpper() = "055A0A33-DD98-47EC-ADD3-8B2009F3944A" Or vDiscountType.ToUpper() = "674AD791-188B-4238-B95C-97B5C2B9D544") Then
                Return True
            Else
                Return False
            End If


        Else
            Return False
        End If


    End Function


    <HttpPost()>
    Function getReceiptHeaderInfo() As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String
        Dim pFowstID As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pFowstID = Common.WebRequest.Data("FowstID")
        If IsGuid(pFowstID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid FOWST ID")
        End If
        pFowstID = ReadyGuid(pFowstID)

        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getReceiptHeaderInfo(pFowstID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSalesPaymentInvoiceHeaderInfo() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("InvoiceID")
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getSalesPaymentInvoiceHeaderInfo(strID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSalesPaymentReceiptBodyInfo() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("InvoiceID")
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getSalesPaymentReceiptBodyInfo(strID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSalesPaymentReceiptSCPWDDiscount() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("InvoiceID")
        strReturn = clsSalesPayments.getSalesPaymentReceiptSCPWDDiscount(strID)

        Return strReturn

    End Function

    <HttpPost()>
    Function getSalesPaymentReceiptSerial() As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getSalesPaymentReceiptSerial(Common.WebRequest.Data("InvoiceID")))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSalesPaymentReceiptPaymentInfo() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("InvoiceID")
        Dim pOut As New JsonDictionary

        pOut.Add("SalesPaymentData", clsSalesPayments.getSalesPaymentReceiptPaymentInfo(strID))
        pOut.Add("OtherPayments", clsSalesPayments.getCustomerPaymentOtherDetails(Common.WebRequest.ProData.ExecuteScalar("SELECT TransCustomerPayment.ID FROM TransCustomerPayment INNER JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPayment.ID WHERE InvoiceID = " & ReadyString(strID))))

        Return pOut.Serialize()

    End Function

    <HttpPost()>
    Function getSalesPaymentReceiptFooterAmountsInfo() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("InvoiceID")
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getSalesPaymentReceiptFooterAmountsInfo(strID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSalesPaymentReceiptFooterMessagesInfo() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("InvoiceID")
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getSalesPaymentReceiptFooterMessagesInfo(strID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSalesPaymentSRVReceiptHeaderInfo() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("MemoID")
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getSalesPaymentSRVReceiptHeaderInfo(strID))

        Return strReturn


    End Function

    <HttpPost()>
    Function getSalesPaymentSRVRefundHeaderInfo() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("MemoID")
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getSalesPaymentSRVRefundHeaderInfo(strID))

        Return strReturn

    End Function

    <HttpPost()>
    Function getSalesPaymentSRVReceiptInvoices() As String
        Dim strID As String
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        strID = Common.WebRequest.Data("MemoID")
        strReturn = JsonDictionary.Serialize("SalesPaymentData", clsSalesPayments.getSalesPaymentSRVReceiptInvoices(strID))

        Return strReturn

    End Function

    <HttpPost()>
    Public Function PreConsumeSrv() As String

        Dim pFowstID As String
        Dim pCustomerPaymentID As String
        Dim pOut As JsonDictionary
        Dim pSrvList As List(Of Srv)
        Dim pSrv As Srv

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pFowstID = Common.WebRequest.Data("FowstID")
        If CStr2(pFowstID).Trim = "" Then
            Return JsonDictionary.Serialize("Error", "Missing FowstID")
        End If
        If IsGuid(pFowstID) = False Then
            Return JsonDictionary.Serialize("Error", "FowstID must be a GUID")
        End If

        pSrvList = New List(Of Srv)
        For Each pItem As Object In Common.WebRequest.AllData("SrvList")
            pSrv = New Srv(pItem)
            If pSrv.Error IsNot Nothing Then
                Return JsonDictionary.Serialize("Error", pSrv.Error)
            End If
            pSrvList.Add(pSrv)
        Next pItem

        pCustomerPaymentID = Common.WebRequest.Data("CustomerPaymentID") 'Guid.NewGuid.ToString commented by janine feb 11 2015. guid is now generated in javascript
        Common.WebRequest.ProData.BeginTransaction()
        For Each pSrv In pSrvList
            If pSrv.Verify(pCustomerPaymentID) = False Then
                Return JsonDictionary.Serialize("Error", pSrv.Error)
            End If
            If pSrv.DontConsume = False Then
                If pSrv.Save(pCustomerPaymentID, pFowstID) = False Then
                    Return JsonDictionary.Serialize("Error", pSrv.Error)
                End If
            End If
        Next pSrv
        Common.WebRequest.ProData.CommitTransaction()

        pOut = New JsonDictionary
        pOut.Add("Result", "ok")
        pOut.Add("CustomerPaymentID", pCustomerPaymentID)
        Return pOut.Serialize

    End Function

    <HttpPost()>
    Public Function GetVPlusPoints() As String
        Dim pSql As String
        Dim Balance As Double

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pSql = "Select top 1 balance from TransCustomerPaymentType where ClientID = " & ReadyString(Common.WebRequest.Data("CustomerID")) & vbCrLf &
            " order by LastUpdated desc"

        Balance = CDec2(Common.WebRequest.ProData.ExecuteScalar(pSql))

        Return JsonDictionary.Serialize("Result", Balance)
    End Function

    Private Class Srv
        ReadOnly mMemoID As String
        ReadOnly mConsumedAmount As Decimal
        ReadOnly mSrvNum As Integer
        ReadOnly mPin As String
        Dim mError As String
        Dim mDontConsume As Boolean

        Public Sub New(ByVal vData As Object)

            mMemoID = vData("MemoID")
            If CStr2(mMemoID).Trim = "" Then
                mError = "Missing MemoID"
            End If
            If IsGuid(mMemoID) = False Then
                mError = "MemoID must be a GUID"
            End If

            If Integer.TryParse(vData("SrvNum"), mSrvNum) = False Then
                mError = "SrvNum must be an integer"
            End If

            mPin = vData("Pin")
            If CStr2(mPin).Trim = "" Then
                mError = "Missing Pin"
            End If

            vData("ConsumedAmount") = CDec2(vData("ConsumedAmount"))
            If Decimal.TryParse(vData("ConsumedAmount"), mConsumedAmount) = False Then
                mError = "ConsumedAmount must be an decimal"
            End If
            If mConsumedAmount <= 0 Then
                mError = "ConsumedAmount must be greater than zero"
            End If

        End Sub

        Public Function Verify(vCustomerPaymentID As String) As Boolean

            Dim pDR As DataRow
            Dim pDrPayment As DataRow

            mDontConsume = False
            pDR = WebRequest.Instance.Business.TransSalesReturnLedger.GetAllByMemoID(mMemoID)
            If pDR Is Nothing Then
                mError = "Record with the specified MemoID does not exist."
                Return False
            End If
            Dim pDV = New DataView(pDR.Table, "DestinationPaymentID IS NOT NULL", "", DataViewRowState.CurrentRows)
            If pDV.Count > 0 Then
                pDrPayment = WebRequest.Instance.Business.TransCustomerPayment.GetByID(CStr2(pDV(0)("DestinationPaymentID")), False)
                If pDrPayment Is Nothing Then
                    If ReadyGuid(pDV(0)("DestinationPaymentID")) <> ReadyGuid(vCustomerPaymentID) Then
                        If DateDiff(DateInterval.Second, pDR("LastUpdated"), Now) < 60 Then
                            mError = "SRV is being verified by another transaction. Please try again after 1 minute."
                            Return False
                        End If
                    End If
                    mDontConsume = True
                Else
                    mError = "SRV was already consumed by previous payment."
                    Return False
                End If
            End If
            If pDR("SrvNum") <> mSrvNum Then
                mError = "SrvNum's do not match"
                Return False
            End If
            pDV = New DataView(pDR.Table, "PIN = " & ReadyString(mPin.Trim), "", DataViewRowState.CurrentRows)
            If pDV.Count = 0 Then
                mError = "Pins do not match"
                Return False
            End If

            Return True

        End Function

        Public Function Save(ByVal vCustomerPaymentID As String, ByVal vFowstID As String) As Boolean

            Return WebRequest.Instance.Business.TransSalesReturnLedger.Consume(mMemoID, vCustomerPaymentID, mConsumedAmount, vFowstID, mError)

        End Function

        'Public ReadOnly Property MemoID As String
        '    Get
        '        Return mMemoID
        '    End Get
        'End Property

        'Public ReadOnly Property ConsumedAmount As Decimal
        '    Get
        '        Return mConsumedAmount
        '    End Get
        'End Property

        'Public ReadOnly Property SrvNum As Integer
        '    Get
        '        Return mSrvNum
        '    End Get
        'End Property

        'Public ReadOnly Property Pin As String
        '    Get
        '        Return mPin
        '    End Get
        'End Property

        Public ReadOnly Property [Error] As String
            Get
                Return mError
            End Get
        End Property

        Public ReadOnly Property DontConsume As String
            Get
                Return mDontConsume

            End Get
        End Property
    End Class
End Class
