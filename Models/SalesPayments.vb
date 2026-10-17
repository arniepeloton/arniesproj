Public Class SalesPayments

    Public Property ID As String
    Public Property InvoiceID As String
    Public Property CustomerID As String
    Public Property FowstID As String
    Public Property SubscriberDeviceID As String
 
    Public Property ReceiptType As String
    Public Property ReceiptNumber As String
    Public Property PaymentDate As String

    Public Property TotalAmountDue As String
    Public Property TotalAmountTendered As String

    Public Property CashAmount As String
    Public Property DebitCardAmount As String
    Public Property CreditCardAmount As String
    Public Property CheckPaymentAmount As String
    Public Property SRVAmount As String
    Public Property DiscountAmount As String
    Public Property OtherTypeAmount As String
    Public Property ChargeAmount As String

    Public Property AmountChange As String
    Public Property CustomerChangeCash As String
    Public Property IsForGovReport As String

    Public Property SalesPaymentCheckList As Object()
    Public Property SalesPaymentDebitCardList As Object()
    Public Property SalesPaymentCreditCardList As Object()
    Public Property SalesPaymentSRVList As Object()
    Public Property SalesPaymentDiscountList As Object()
    Public Property SalesPaymentSeniorCitizenPWDCustomerList As Object()
    Public Property SalesPaymentOtherTypeList As Object()

    Public Property NoOfPeople As String
    Public Property NoOfSeniorCitizens As String

    Public Property TotalDiscountAmount As String

    Function getInvoiceTotalAmountDue(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("InvoiceID", SqlDbType.UniqueIdentifier)
                    .Expression = "siitems.InvoiceID"
                    .Searchable = False
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "si.CustomerID"
                    .Searchable = False
                End With
                With .Add("TotalAmountDue", SqlDbType.Money)
                    .Expression = "SUM(ISNULL(siitems.VATableSale,SubTotal) + ISNULL(siitems.VATExemptSale,0) + ISNULL(siitems.ZeroRatedSale,0)+ ISNULL(siitems.NonVATSale,0))"
                End With
                With .Add("TotalAmountDueAllowedForDiscount", SqlDbType.Money)
                    .Expression = "(SELECT SUM(ISNULL(SubTotal,0)) FROM TransInvoiceItems WHERE InvoiceID = si.ID AND OriginalUnitPrice = UnitPrice) "
                End With
                With .Add("RefNum", SqlDbType.VarChar)
                    .Expression = "si.RefNum"
                End With
                With .Add("TransInvoiceItems", SqlDbType.Money)
                    .Expression = "SUM(ISNULL(siitems.SubTotal,0))"
                End With
                With .Add("TaxPercent", SqlDbType.Float)
                    .Expression = "(SELECT TOP 1 ISNULL(TaxPercent, 0) FROM TransInvoiceItems WHERE InvoiceID = si.ID)"
                End With
            End With

            .SqlFrom = " FROM TransInvoiceItems siitems " & vbCrLf &
                " INNER JOIN TransInvoice si ON (siitems.InvoiceID = si.ID)"

            .SqlGroup = " GROUP BY siitems.InvoiceID, si.CustomerID " &
                ",si.RefNum,si.ID"
            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE siitems.InvoiceID IS NULL", " WHERE siitems.InvoiceID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getInvoiceDetails(ByVal strInvNo As String, ByVal vFowstID As String, ByVal vDeviceID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.ID"
                    .Searchable = False
                End With
                With .Add("InvoiceNumber", SqlDbType.VarChar)
                    .Expression = "inv.InvoiceNumber"
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.CustomerID"
                End With
            End With

            .SqlFrom = " FROM dbo.TransInvoice inv "

            .SqlWhere = " WHERE inv.InvoiceNumber = " + strInvNo.ToString() &
                " AND inv.FowstID = " & ReadyString(vFowstID) &
                " AND inv.DeviceID = " & ReadyString(vDeviceID)

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function IsInvoiceSaved(ByVal strID) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "custpayinv.ID"
                    .Searchable = False
                End With
                With .Add("InvoiceID", SqlDbType.UniqueIdentifier)
                    .Expression = "custpayinv.InvoiceID"
                End With
                With .Add("InvoiceNumber", SqlDbType.Int)
                    .Expression = "inv.InvoiceNumber"
                End With
                With .Add("CustomerPaymentID", SqlDbType.UniqueIdentifier)
                    .Expression = "custpay.ID"
                End With
            End With

            .SqlFrom = " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) "

            .SqlWhere = " WHERE custpayinv.InvoiceID = " + ReadyString(strID) &
                " AND custpay.IsCancelled = 0 AND custpay.ReceiptType <> 'RF'" 'additional by janine may 18 2015. add condition receipt type must not be refund it must be OR

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getCustomerEmails(ByVal strID As String) As DataTable
        Dim pSQL As String
        Dim pDT As DataTable

        pSQL = "SELECT custemail.EmailAddress " &
        " FROM dbo.MobizLibraryOrganizationContactEmail custemail " &
        " Where custemail.OrganizationID = " & ReadyString(strID) &
        " UNION " &
        " Select custaddemail.EmailAddress " &
        " FROM dbo.MobizLibraryOrganizationFOWST custfowst " &
        " INNER JOIN dbo.MobizLibraryOrganizationFOWSTEmail custaddemail ON (custfowst.ID = custaddemail.FOWSTID) " &
        " Where custfowst.OrganizationID = " & ReadyString(strID) &
        " UNION " &
        " Select custrepemail.EmailAddress " &
        " FROM dbo.MobizLibraryOrganizationRepresentatives custrep " &
        " INNER JOIN dbo.MobizLibraryOrganizationRepresentativesEmail custrepemail ON (custrep.ID = custrepemail.OrgRepID)  " &
        " Where custrep.OrganizationID = " & ReadyString(strID)

        pDT = Common.WebRequest.ProData.Execute(pSQL)

        Return pDT

    End Function

    Function getSalesPaymentCheckList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaychk.ID"
                    .Searchable = False
                End With
                With .Add("CustomerPaymentID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaychk.CustomerPaymentID"
                    .Searchable = False
                End With
                With .Add("BankID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaychk.BankID"
                    .Searchable = False
                End With
                With .Add("BankName", SqlDbType.VarChar)
                    .Expression = "salespaychk.BankName"
                End With
                With .Add("BankBranchID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaychk.BankBranchID"
                    .Searchable = False
                End With
                With .Add("BankBranchName", SqlDbType.VarChar)
                    .Expression = "salespaychk.BankBranchName"
                End With
                With .Add("AccountName", SqlDbType.VarChar)
                    .Expression = "salespaychk.AccountName"
                End With
                With .Add("AccountNumber", SqlDbType.VarChar)
                    .Expression = "salespaychk.AccountNumber"
                End With
                With .Add("CheckNumber", SqlDbType.VarChar)
                    .Expression = "salespaychk.CheckNumber"
                End With
                With .Add("CheckDateMonth", SqlDbType.Date)
                    .Expression = "MONTH(salespaychk.CheckDate)"
                End With
                With .Add("CheckDateDay", SqlDbType.Date)
                    .Expression = "DAY(salespaychk.CheckDate)"
                End With
                With .Add("CheckDateYear", SqlDbType.Date)
                    .Expression = "YEAR(salespaychk.CheckDate)"
                End With
                With .Add("CheckAmount", SqlDbType.Money)
                    .Expression = "salespaychk.CheckAmount"
                End With
            End With

            .SqlFrom = " FROM dbo.TransCustomerPaymentChecks salespaychk " 

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE salespaychk.CustomerPaymentID IS NULL", " salespaychk.CustomerPaymentID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesPaymentDebitCardList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaydc.ID"
                    .Searchable = False
                End With
                With .Add("CustomerPaymentID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaydc.CustomerPaymentID"
                    .Searchable = False
                End With
                With .Add("DebitCardType", SqlDbType.VarChar)
                    .Expression = "salespaydc.DebitCardType"
                End With
                With .Add("DebitCardBankID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaydc.DebitCardBankID"
                    .Searchable = False
                End With
                With .Add("DebitCardBankName", SqlDbType.VarChar)
                    .Expression = "salespaydc.DebitCardBankName"
                End With
                With .Add("BankCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaydc.BankCountryID"
                    .Searchable = False
                End With
                With .Add("BankCountry", SqlDbType.VarChar)
                    .Expression = "ctry.Name"
                End With
                With .Add("HolderLastName", SqlDbType.VarChar)
                    .Expression = "salespaydc.HolderLastName"
                End With
                With .Add("HolderFirstName", SqlDbType.VarChar)
                    .Expression = "salespaydc.HolderFirstName"
                End With
                With .Add("HolderMiddleName", SqlDbType.VarChar)
                    .Expression = "salespaydc.HolderMiddleName"
                End With
                With .Add("HolderGenerationalSuffix", SqlDbType.VarChar)
                    .Expression = "salespaydc.HolderGenerationalSuffix"
                End With
                With .Add("DebitCardNumber", SqlDbType.VarChar)
                    .Expression = "salespaydc.DebitCardNumber"
                End With
                With .Add("DebitCardIssuanceDateMonth", SqlDbType.Date)
                    .Expression = "MONTH(salespaydc.DebitCardIssuanceDate)"
                End With
                With .Add("DebitCardIssuanceDateDay", SqlDbType.Date)
                    .Expression = "DAY(salespaydc.DebitCardIssuanceDate)"
                End With
                With .Add("DebitCardIssuanceDateYear", SqlDbType.Date)
                    .Expression = "YEAR(salespaydc.DebitCardIssuanceDate)"
                End With
                With .Add("DebitCardExpirationDateMonth", SqlDbType.Date)
                    .Expression = "MONTH(salespaydc.DebitCardExpirationDate)"
                End With
                With .Add("DebitCardExpirationDateDay", SqlDbType.Date)
                    .Expression = "DAY(salespaydc.DebitCardExpirationDate)"
                End With
                With .Add("DebitCardExpirationDateYear", SqlDbType.Date)
                    .Expression = "YEAR(salespaydc.DebitCardExpirationDate)"
                End With
                With .Add("ApprovalNo", SqlDbType.VarChar)
                    .Expression = "salespaydc.ApprovalNo"
                End With
                With .Add("NoOfInstallmentPayments", SqlDbType.Int)
                    .Expression = "salespaydc.NoOfInstallmentPayments"
                End With
                With .Add("DebitCardSurcharge", SqlDbType.Float)
                    .Expression = "salespaydc.DebitCardSurcharge"
                End With
                With .Add("DebitCardAmount", SqlDbType.Money)
                    .Expression = "salespaydc.DebitCardAmount"
                End With
            End With

            .SqlFrom = " FROM dbo.TransCustomerPaymentdebitCard salespaydc " &
                " INNER JOIN dbo.MobizLibraryCountries ctry ON (salespaydc.BankCountryID = ctry.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE salespaydc.CustomerPaymentID IS NULL", " salespaydc.CustomerPaymentID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesPaymentCreditCardList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaycc.ID"
                    .Searchable = False
                End With
                With .Add("CustomerPaymentID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaycc.CustomerPaymentID"
                    .Searchable = False
                End With
                With .Add("CreditCardType", SqlDbType.VarChar)
                    .Expression = "salespaycc.CreditCardType"
                End With
                With .Add("CreditCardBankID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaycc.CreditCardBankID"
                    .Searchable = False
                End With
                With .Add("CreditCardBankName", SqlDbType.VarChar)
                    .Expression = "salespaycc.CreditCardBankName"
                End With
                With .Add("BankCountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaycc.BankCountryID"
                    .Searchable = False
                End With
                With .Add("BankCountry", SqlDbType.VarChar)
                    .Expression = "ctry.Name"
                End With
                With .Add("HolderLastName", SqlDbType.VarChar)
                    .Expression = "salespaycc.HolderLastName"
                End With
                With .Add("HolderFirstName", SqlDbType.VarChar)
                    .Expression = "salespaycc.HolderFirstName"
                End With
                With .Add("HolderMiddleName", SqlDbType.VarChar)
                    .Expression = "salespaycc.HolderMiddleName"
                End With
                With .Add("HolderGenerationalSuffix", SqlDbType.VarChar)
                    .Expression = "salespaycc.HolderGenerationalSuffix"
                End With
                With .Add("CreditCardNumber", SqlDbType.VarChar)
                    .Expression = "salespaycc.CreditCardNumber"
                End With
                With .Add("CreditCardIssuanceDateMonth", SqlDbType.Date)
                    .Expression = "MONTH(salespaycc.CreditCardIssuanceDate)"
                End With
                With .Add("CreditCardIssuanceDateDay", SqlDbType.Date)
                    .Expression = "DAY(salespaycc.CreditCardIssuanceDate)"
                End With
                With .Add("CreditCardIssuanceDateYear", SqlDbType.Date)
                    .Expression = "YEAR(salespaycc.CreditCardIssuanceDate)"
                End With
                With .Add("CreditCardExpirationDateMonth", SqlDbType.Date)
                    .Expression = "MONTH(salespaycc.CreditCardExpirationDate)"
                End With
                With .Add("CreditCardExpirationDateDay", SqlDbType.Date)
                    .Expression = "DAY(salespaycc.CreditCardExpirationDate)"
                End With
                With .Add("CreditCardExpirationDateYear", SqlDbType.Date)
                    .Expression = "YEAR(salespaycc.CreditCardExpirationDate)"
                End With
                With .Add("ApprovalNo", SqlDbType.VarChar)
                    .Expression = "salespaycc.ApprovalNo"
                End With
                With .Add("NoOfInstallmentPayments", SqlDbType.Int)
                    .Expression = "salespaycc.NoOfInstallmentPayments"
                End With
                With .Add("CreditCardSurcharge", SqlDbType.Float)
                    .Expression = "salespaycc.CreditCardSurcharge"
                End With
                With .Add("CreditCardAmount", SqlDbType.Money)
                    .Expression = "salespaycc.CreditCardAmount"
                End With
            End With

            .SqlFrom = " FROM dbo.TransCustomerPaymentCreditCard salespaycc " &
                " INNER JOIN dbo.MobizLibraryCountries ctry ON (salespaycc.BankCountryID = ctry.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE salespaycc.CustomerPaymentID IS NULL", " salespaycc.CustomerPaymentID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesPaymentSRVList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaysrv.ID"
                    .Searchable = False
                End With
                With .Add("CustomerPaymentID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaysrv.CustomerPaymentID"
                    .Searchable = False
                End With
                With .Add("MemoID", SqlDbType.UniqueIdentifier)
                    .Expression = "salespaysrv.MemoID"
                    .Searchable = False
                End With
                With .Add("SRVNo", SqlDbType.Int)
                    .Expression = "(SELECT TOP 1 SrvNum " &
                                  " FROM TransSalesReturnLedger " &
                                  " WHERE SourceMemoID = salespaysrv.MemoID)"
                End With
                With .Add("PinCode", SqlDbType.Int)
                    .Expression = "(SELECT TOP 1 Pin " &
                                  " FROM TransSalesReturnLedger " &
                                  " WHERE SourceMemoID = salespaysrv.MemoID)"
                End With
                With .Add("SRVAmount", SqlDbType.Money)
                    .Expression = "(SELECT ISNULL(SUM(Amount),0) " &
                                  " FROM TransSalesReturnLedger " &
                                  " WHERE SourceMemoID = salespaysrv.MemoID)"
                End With
                With .Add("ConsumedAmount", SqlDbType.Money)
                    .Expression = "salespaysrv.ConsumedAmount"
                End With
            End With

            .SqlFrom = " FROM dbo.TransCustomerPaymentSRV salespaysrv "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE salespaysrv.CustomerPaymentID IS NULL", " salespaysrv.CustomerPaymentID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesPaymentInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "custpay.ID"
                    .Searchable = False
                End With
                With .Add("ReceiptType", SqlDbType.VarChar)
                    .Expression = "custpay.ReceiptType"
                End With
                With .Add("ReceiptNumber", SqlDbType.VarChar)
                    .Expression = "custpay.ReceiptNumber"
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "custpay.CustomerID"
                    .Searchable = False
                End With
                With .Add("PaymentDateMonth", SqlDbType.Date)
                    .Expression = "MONTH(custpay.PaymentDate)"
                End With
                With .Add("PaymentDateDay", SqlDbType.Date)
                    .Expression = "DAY(custpay.PaymentDate)"
                End With
                With .Add("PaymentDateYear", SqlDbType.Date)
                    .Expression = "YEAR(custpay.PaymentDate)"
                End With
                With .Add("CashAmount", SqlDbType.Money)
                    .Expression = "custpay.CashAmount"
                End With
                With .Add("DebitCardAmount", SqlDbType.Money)
                    .Expression = "custpay.DebitCardAmount"
                End With
                With .Add("CreditCardAmount", SqlDbType.Money)
                    .Expression = "custpay.CreditCardAmount"
                End With
                With .Add("CheckPaymentAmount", SqlDbType.Money)
                    .Expression = "custpay.CheckPaymentAmount"
                End With
                With .Add("TotalAmountDue", SqlDbType.Money)
                    .Expression = "custpay.TotalAmountDue"
                End With
                With .Add("DiscountAmount", SqlDbType.Money)
                    .Expression = "custpay.DiscountAmount"
                End With
                With .Add("CustomerChangeCash", SqlDbType.Money)
                    .Expression = "custpay.CustomerChangeCash"
                End With
                With .Add("SRVAmount", SqlDbType.Money)
                    .Expression = "custpay.SRVAmount"
                End With
                With .Add("OtherTypeAmount", SqlDbType.Money)
                    .Expression = "custpay.OtherTypeAmount"
                End With
            End With

            .SqlFrom = " FROM dbo.TransCustomerPayment custpay "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE custpay.ID IS NULL", " WHERE custpay.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveSalesPayments(ByVal vOrgID As String) As String
        Dim p As New InsertUpdate("TransCustomerPayment")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDRec As String
        Dim strError As String
        Dim pDR As DataRow
        Dim pNow As Date

        Try
            pDR = WebRequest.Instance.Business.TransInvoice.ById(InvoiceID)

            p("ReceiptType") = ReadyString(ReceiptType)
            p("ReceiptNumber") = "dbo.GetOrganizationFOWSTInvNo(" & ReadyString(FowstID) & "," & ReadyString(WebRequest.Instance.DeviceID) & ") + 1"
            p("CustomerID") = If(CustomerID.Trim().Length > 0, ReadyString(CustomerID), "NULL")
            If CStr2(pDR("RefNum")) = "" Then
                pNow = Common.WebRequest.ProData.GetDate
                p("PaymentDate") = ReadyString(pNow)
            Else
                p("PaymentDate") = ReadyString(pDR("InvoiceDate"))
            End If

            p("CashAmount") = If(CashAmount.Trim().Length > 0, CDec2(CashAmount), 0)
            p("DebitCardAmount") = If(DebitCardAmount.Trim().Length > 0, CDec2(DebitCardAmount), 0)
            p("CreditCardAmount") = If(CreditCardAmount.Trim().Length > 0, CDec2(CreditCardAmount), 0)
            p("CheckPaymentAmount") = If(CheckPaymentAmount.Trim().Length > 0, CDec2(CheckPaymentAmount), 0)
            p("SRVAmount") = If(SRVAmount.Trim().Length > 0, CDec2(SRVAmount), 0)
            p("DiscountAmount") = If(DiscountAmount.Trim().Length > 0, CDec2(DiscountAmount), 0)
            p("OtherTypeAmount") = If(OtherTypeAmount.Trim().Length > 0, CDec2(OtherTypeAmount), 0)
            p("ChargeAmount") = If(ChargeAmount.Trim().Length > 0, CDec2(ChargeAmount), 0)

            p("TotalAmountDue") = If(TotalAmountDue.Trim().Length > 0, CDec2(TotalAmountDue), 0)

            p("AmountChange") = If(AmountChange.Trim().Length > 0, CDec2(AmountChange), 0)
            p("CustomerChangeCash") = If(CustomerChangeCash.Trim().Length > 0, CDec2(CustomerChangeCash), 0)
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Update Invoice Number

            p = New InsertUpdate("TransInvoice")
            p("InvoiceNumber") = "dbo.GetOrganizationFOWSTInvNo(" & ReadyString(WebRequest.Instance.FowstID) & "," & ReadyString(WebRequest.Instance.DeviceID) & ") + 1"
            p("FowstID") = ReadyString(WebRequest.Instance.FowstID)
            p("DeviceID") = IIf(SubscriberDeviceID.Trim().Length = 0, "NULL", ReadyString(SubscriberDeviceID.Trim()))
            p("IsForGovReport") = IsForGovReport.Trim()
            p("DiscountAmount") = If(DiscountAmount.Trim().Length > 0, CDec2(DiscountAmount), 0)
            If CStr2(pDR("RefNum")) = "" Then
                p("InvoiceDate") = ReadyString(pNow)
            End If
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

            pSql = p.GetUpdate(" ID = " & IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim())))
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Invoice
            p = New InsertUpdate("TransCustomerPaymentInvoices")

            p("CustomerPaymentID") = ReadyString(strIDReturn)
            p("InvoiceID") = IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim()))
            p("InvoiceAmount") = IIf(TotalAmountDue.Trim().Length > 0, Convert.ToDouble(TotalAmountDue), 0)
            p("DiscountAmount") = IIf(DiscountAmount.Trim().Length > 0, Convert.ToDouble(DiscountAmount), 0)
            p("Balance") = IIf(TotalAmountDue.Trim().Length > 0, Convert.ToDouble(TotalAmountDue), 0)
            p("AmountPaid") = Convert.ToDouble(IIf(CashAmount.Trim().Length = 0, "0", CashAmount)) + Convert.ToDouble(IIf(DebitCardAmount.Trim().Length = 0, "0", DebitCardAmount)) + Convert.ToDouble(IIf(CreditCardAmount.Trim().Length = 0, "0", CreditCardAmount)) + Convert.ToDouble(IIf(CheckPaymentAmount.Trim().Length = 0, "0", CheckPaymentAmount)) + Convert.ToDouble(IIf(SRVAmount.Trim().Length = 0, "0", SRVAmount)) + Convert.ToDouble(IIf(OtherTypeAmount.Trim().Length = 0, "0", OtherTypeAmount)) - Convert.ToDouble(IIf(CustomerChangeCash.Trim().Length = 0, "0", CustomerChangeCash))

            strIDRec = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDRec)
            pSql = p.GetInsert()

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Check
            For Each objSalesPayCheck As Object In SalesPaymentCheckList

                If (objSalesPayCheck("RecState").ToString().Trim().Length > 0) Then

                    If (objSalesPayCheck("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("TransCustomerPaymentChecks")

                        p("CustomerPaymentID") = ReadyString(strIDReturn)
                        p("BankID") = IIf(objSalesPayCheck("BankID").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesPayCheck("BankID").ToString().Trim()))
                        p("BankName") = ReadyString(objSalesPayCheck("BankName").ToString())
                        p("BankBranchID") = IIf(objSalesPayCheck("BankBranchID").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesPayCheck("BankBranchID").ToString().Trim()))
                        p("BankBranchName") = ReadyString(objSalesPayCheck("BankBranchName").ToString())
                        p("AccountName") = ReadyString(objSalesPayCheck("AccountName").ToString())
                        p("AccountNumber") = ReadyString(objSalesPayCheck("AccountNumber").ToString())
                        p("CheckNumber") = ReadyString(objSalesPayCheck("CheckNumber").ToString())
                        p("CheckDate") = IIf(objSalesPayCheck("CheckDate").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesPayCheck("CheckDate").ToString().Trim()))
                        p("CheckAmount") = Convert.ToDouble(objSalesPayCheck("CheckAmount").ToString())
                        p("IsBankFromOrg") = 1

                        If (objSalesPayCheck("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objSalesPayCheck("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.TransCustomerPaymentChecks " &
                               " WHERE ID " & IIf(objSalesPayCheck("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objSalesPayCheck("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objSalesPayCheck

            '''Save Debit Card
            For Each objSalesPayDebitCard As Object In SalesPaymentDebitCardList

                If (objSalesPayDebitCard("RecState").ToString().Trim().Length > 0) Then

                    If (objSalesPayDebitCard("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("TransCustomerPaymentDebitCard")

                        p("CustomerPaymentID") = ReadyString(strIDReturn)
                        p("DebitCardType") = ReadyString(objSalesPayDebitCard("DebitCardType").ToString())
                        p("DebitCardBankID") = IIf(objSalesPayDebitCard("DebitCardBankID").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesPayDebitCard("DebitCardBankID").ToString().Trim()))
                        p("DebitCardBankName") = ReadyString(objSalesPayDebitCard("DebitCardBank").ToString())
                        p("BankCountryID") = IIf(objSalesPayDebitCard("BankCountryID").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesPayDebitCard("BankCountryID").ToString().Trim()))
                        p("HolderLastName") = ReadyString(objSalesPayDebitCard("HolderLastName").ToString())
                        p("HolderFirstName") = ReadyString(objSalesPayDebitCard("HolderFirstName").ToString())
                        p("HolderMiddleName") = ReadyString(objSalesPayDebitCard("HolderMiddleName").ToString())
                        p("HolderGenerationalSuffix") = ReadyString(objSalesPayDebitCard("HolderGenerationalSuffix").ToString())
                        p("DebitCardNumber") = ReadyString(objSalesPayDebitCard("DebitCardNumber").ToString())
                        p("DebitCardIssuanceDate") = IIf(objSalesPayDebitCard("DebitCardIssuanceDate").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesPayDebitCard("DebitCardIssuanceDate").ToString().Trim()))
                        p("DebitCardExpirationDate") = IIf(objSalesPayDebitCard("DebitCardExpirationDate").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesPayDebitCard("DebitCardExpirationDate").ToString().Trim()))
                        p("ApprovalNo") = ReadyString(objSalesPayDebitCard("ApprovalNo").ToString())
                        p("NoOfInstallmentPayments") = IIf(objSalesPayDebitCard("NoOfInstallmentPayments").ToString().Trim().Length > 0, objSalesPayDebitCard("NoOfInstallmentPayments").ToString(), "0")
                        p("DebitCardAmount") = Convert.ToDouble(objSalesPayDebitCard("DebitCardAmount").ToString())
                        p("IsBankFromOrg") = 1

                        If (objSalesPayDebitCard("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objSalesPayDebitCard("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.TransCustomerPaymentDebitCard " &
                               " WHERE ID " & IIf(objSalesPayDebitCard("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objSalesPayDebitCard("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objSalesPayDebitCard

            '''Save Credit Card
            For Each objSalesPayCreditCard As Object In SalesPaymentCreditCardList

                If (objSalesPayCreditCard("RecState").ToString().Trim().Length > 0) Then

                    If (objSalesPayCreditCard("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("TransCustomerPaymentCreditCard")

                        p("CustomerPaymentID") = ReadyString(strIDReturn)
                        p("CreditCardType") = ReadyString(objSalesPayCreditCard("CreditCardType").ToString())
                        p("CreditCardBankID") = IIf(objSalesPayCreditCard("CreditCardBankID").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesPayCreditCard("CreditCardBankID").ToString().Trim()))
                        p("CreditCardBankName") = ReadyString(objSalesPayCreditCard("CreditCardBank").ToString())
                        p("BankCountryID") = IIf(objSalesPayCreditCard("BankCountryID").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesPayCreditCard("BankCountryID").ToString().Trim()))
                        p("HolderLastName") = ReadyString(objSalesPayCreditCard("HolderLastName").ToString())
                        p("HolderFirstName") = ReadyString(objSalesPayCreditCard("HolderFirstName").ToString())
                        p("HolderMiddleName") = ReadyString(objSalesPayCreditCard("HolderMiddleName").ToString())
                        p("HolderGenerationalSuffix") = ReadyString(objSalesPayCreditCard("HolderGenerationalSuffix").ToString())
                        p("CreditCardNumber") = ReadyString(objSalesPayCreditCard("CreditCardNumber").ToString())
                        p("CreditCardIssuanceDate") = IIf(objSalesPayCreditCard("CreditCardIssuanceDate").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesPayCreditCard("CreditCardIssuanceDate").ToString().Trim()))
                        p("CreditCardExpirationDate") = IIf(objSalesPayCreditCard("CreditCardExpirationDate").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesPayCreditCard("CreditCardExpirationDate").ToString().Trim()))
                        p("ApprovalNo") = ReadyString(objSalesPayCreditCard("ApprovalNo").ToString())
                        p("NoOfInstallmentPayments") = objSalesPayCreditCard("NoOfInstallmentPayments").ToString()
                        p("CreditCardAmount") = Convert.ToDouble(objSalesPayCreditCard("CreditCardAmount").ToString())
                        p("IsBankFromOrg") = 1

                        If (objSalesPayCreditCard("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objSalesPayCreditCard("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.TransCustomerPaymentCreditCard " &
                               " WHERE ID " & IIf(objSalesPayCreditCard("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objSalesPayCreditCard("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objSalesPayCreditCard

            '''Save SRV
            For Each objSalesSRV As Object In SalesPaymentSRVList

                If (objSalesSRV("RecState").ToString().Trim().Length > 0) Then

                    If (objSalesSRV("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("TransCustomerPaymentSRV")

                        p("CustomerPaymentID") = ReadyString(strIDReturn)
                        p("MemoID") = IIf(objSalesSRV("MemoID").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesSRV("MemoID").ToString().Trim()))
                        p("ConsumedAmount") = Convert.ToDouble(objSalesSRV("ConsumedAmount").ToString())

                        If (objSalesSRV("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objSalesSRV("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.TransCustomerPaymentSRV " &
                               " WHERE ID " & IIf(objSalesSRV("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objSalesSRV("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                    '''Update Memo ledger
                    strError = ""
                    WebRequest.Instance.Business.TransSalesReturnLedger.Consume(IIf(objSalesSRV("MemoID").ToString().Trim().Length = 0, "NULL", objSalesSRV("MemoID").ToString().Trim()), strIDReturn, Convert.ToDecimal(objSalesSRV("ConsumedAmount").ToString()), WebRequest.Instance.FowstID, strError)

                End If

            Next objSalesSRV

            '''Save Other tender
            For Each objSalesOtherType As Object In SalesPaymentOtherTypeList

                If (objSalesOtherType("RecState").ToString().Trim().Length > 0) Then

                    If (objSalesOtherType("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("TransCustomerPaymentOther")

                        p("CustomerPaymentID") = ReadyString(strIDReturn)
                        p("PaymentType") = ReadyString(objSalesOtherType("PaymentType").ToString())
                        p("Amount") = Convert.ToDouble(objSalesOtherType("Amount").ToString())
                        p("IssuingOrganization") = ReadyString(objSalesOtherType("IssuingOrganization").ToString())
                        p("CardNumber") = ReadyString(objSalesOtherType("CardNo").ToString())
                        p("CardHolderName") = ReadyString(objSalesOtherType("CardHolder").ToString())
                        p("CardIssuanceDate") = IIf(objSalesOtherType("IssuanceDate").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesOtherType("IssuanceDate").ToString().Trim()))
                        p("CardExpirationDate") = IIf(objSalesOtherType("ExpirationDate").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesOtherType("ExpirationDate").ToString().Trim()))
                        p("ApprovalNo") = ReadyString(objSalesOtherType("ApprovalNo").ToString())

                        If (objSalesOtherType("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objSalesOtherType("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.TransCustomerPaymentOther " &
                               " WHERE ID " & IIf(objSalesOtherType("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objSalesOtherType("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If


                End If

            Next objSalesOtherType

            '''Save Discount
            pSql = "DELETE FROM dbo.TransInvoiceDiscount WHERE InvoiceID = " & ReadyString(InvoiceID.Trim())

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            For Each objSalesDiscount As Object In SalesPaymentDiscountList

                If (objSalesDiscount("ID").ToString().Trim() <> "") Then
                    p = New InsertUpdate("TransInvoiceDiscount")

                    p("InvoiceID") = IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim()))
                    p("DiscountTypeID") = IIf(objSalesDiscount("ID").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesDiscount("ID").ToString().Trim()))
                    p("DiscountPercentage") = CDec2(objSalesDiscount("DiscountPercent").ToString())
                    p("DiscountRemarks") = ReadyString(objSalesDiscount("DiscountRemarks").ToString())
                    'additional by janine
                    p("NoOfPeople") = IIf(NoOfPeople = "0", "NULL", CDec2(NoOfPeople))
                    p("NoOfSeniorCitizens") = IIf(NoOfSeniorCitizens = "0", "NULL", CDec2(NoOfSeniorCitizens))
                    'end of additional

                    strIDRec = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(strIDRec)
                    pSql = p.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If (CStr2(objSalesDiscount("ID").ToString().Trim()).ToUpper = "055A0A33-DD98-47EC-ADD3-8B2009F3944A" Or
                  CStr2(objSalesDiscount("ID").ToString().Trim()).ToUpper = "674AD791-188B-4238-B95C-97B5C2B9D544") Then
                        '''Save Discount on Invoice Item
                        p = New InsertUpdate("TransInvoiceItems")
                        'additional by janine 'feb 17
                        p("VATExemptSale") = "(Subtotal) / (cast((100 + TaxPercent) as float)/100)"
                        p("ZeroRatedSale") = "0"
                        p("NonVATSale") = "0"
                        p("VATableSale") = "0"
                        p("TaxSales") = "0"
                        p("TaxAmount") = "0"
                        pSql = p.GetUpdate(" InvoiceID = " & IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim())) & " AND OriginalUnitPrice = UnitPrice")
                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        '''Save Discount on Invoice Item
                        p = New InsertUpdate("TransInvoiceItems")
                        p("DiscountAmount") = "VATExemptSale - (VATExemptSale * (cast((100 - " & CDec2(objSalesDiscount("DiscountPercent").ToString()) & ") as float)/100))"
                        p("DiscountSales") = "0" 'just make this 0
                        pSql = p.GetUpdate(" InvoiceID = " & IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim())) & " AND OriginalUnitPrice = UnitPrice")
                        Common.WebRequest.ProData.ExecuteSQL(pSql)


                    Else


                        '''Save Discount on Invoice Item
                        p = New InsertUpdate("TransInvoiceItems")
                        p("DiscountAmount") = "Subtotal - (Subtotal * (cast((100 - " & CDec2(objSalesDiscount("DiscountPercent").ToString()) & ") as float)/100))"
                        p("DiscountSales") = " (" & "Subtotal - (Subtotal * (cast((100 - " & CDec2(objSalesDiscount("DiscountPercent").ToString()) & ") as float)/100))" & ")/ (cast((100 + TaxPercent) as float)/100)"
                        pSql = p.GetUpdate(" InvoiceID = " & IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim())) & " AND OriginalUnitPrice = UnitPrice")
                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        '''Save Discount on Invoice Item
                        p = New InsertUpdate("TransInvoiceItems")
                        p("TaxSales") = "(Subtotal - DiscountAmount) / (cast((100 + TaxPercent) as float)/100)"
                        p("TaxAmount") = "(Subtotal - DiscountAmount) - ((Subtotal - DiscountAmount) / (cast((100 + TaxPercent) as float)/100))"
                        'additional by janine 'feb 17
                        p("VATExemptSale") = "0"
                        p("ZeroRatedSale") = "0"
                        p("NonVATSale") = "0"
                        p("VATableSale") = "(Subtotal)"
                        pSql = p.GetUpdate(" InvoiceID = " & IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim())) & " AND OriginalUnitPrice = UnitPrice")
                        Common.WebRequest.ProData.ExecuteSQL(pSql)



                    End If



                End If

            Next objSalesDiscount

            ''' 


            '''check if discount type is Senior Citizen Discount, PWD, or special senior citizen discount. if yes, thats the only time you save
            pSql = "DELETE FROM dbo.TransInvoiceSeniorCitizenPWDCustomer WHERE InvoiceID = " & ReadyString(InvoiceID.Trim())

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            For Each objSalesSeniorCitizenPWD As Object In SalesPaymentSeniorCitizenPWDCustomerList

                If (objSalesSeniorCitizenPWD("ID").ToString().Trim() <> "") Then
                    '''Save Senior Citizen PWD Customer
                    ''' 
                    p = New InsertUpdate("TransInvoiceSeniorCitizenPWDCustomer")

                    p("InvoiceID") = IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim()))
                    p("CustomerID") = IIf(objSalesSeniorCitizenPWD("ID").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesSeniorCitizenPWD("ID").ToString().Trim()))

                    strIDRec = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(strIDRec)
                    pSql = p.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql)


                End If

            Next objSalesSeniorCitizenPWD




            '''Update Places POS OR Counter
            p = New InsertUpdate("LibraryDeviceFowst")
            p("POSLastORNo") = "dbo.GetOrganizationFOWSTInvNo(" & ReadyString(WebRequest.Instance.FowstID) & "," & ReadyString(WebRequest.Instance.DeviceID) & ") + 1 "
            pSql = p.GetUpdate(" FowstID = " & IIf(FowstID.Trim().Length = 0, "NULL", ReadyString(FowstID.Trim())) & " AND DeviceID = " & IIf(WebRequest.Instance.DeviceID.Trim().Length = 0, "NULL", ReadyString(WebRequest.Instance.DeviceID.Trim())))
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            WebRequest.Instance.Business.TransInvoice.AddToInventoryLedger(InvoiceID, vOrgID)
            ' ''WebRequest.Instance.Business.TransInvoice.AddToInventoryLedgerBOM(InvoiceID)

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Throw
        End Try

        Return strIDReturn

    End Function

    Function SaveBillingDiscount() As String
        Dim p As New InsertUpdate("TransInvoiceDiscount")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDRec As String
        Dim strError As String
        Dim pDR As DataRow
        Dim pNow As Date

        Try
            Common.WebRequest.ProData.BeginTransaction()
            '''Save Discount
            pSql = "DELETE FROM dbo.TransInvoiceDiscount WHERE InvoiceID = " & ReadyString(InvoiceID.Trim())

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            For Each objSalesDiscount As Object In SalesPaymentDiscountList

                If (objSalesDiscount("ID").ToString().Trim() <> "") Then
                    p = New InsertUpdate("TransInvoiceDiscount")

                    p("InvoiceID") = IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim()))
                    p("DiscountTypeID") = IIf(objSalesDiscount("ID").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesDiscount("ID").ToString().Trim()))
                    p("DiscountPercentage") = CDec2(objSalesDiscount("DiscountPercent").ToString())
                    p("DiscountRemarks") = ReadyString(objSalesDiscount("DiscountRemarks").ToString())
                    'additional by janine
                    p("NoOfPeople") = IIf(NoOfPeople = "0", "NULL", CDec2(NoOfPeople))
                    p("NoOfSeniorCitizens") = IIf(NoOfSeniorCitizens = "0", "NULL", CDec2(NoOfSeniorCitizens))
                    'end of additional

                    strIDRec = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(strIDRec)
                    pSql = p.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    If (CStr2(objSalesDiscount("ID").ToString().Trim()).ToUpper = "055A0A33-DD98-47EC-ADD3-8B2009F3944A" Or
                  CStr2(objSalesDiscount("ID").ToString().Trim()).ToUpper = "674AD791-188B-4238-B95C-97B5C2B9D544") Then
                        '''Save Discount on Invoice Item
                        p = New InsertUpdate("TransInvoiceItems")
                        'additional by janine 'feb 17
                        p("VATExemptSale") = "(Subtotal) / (cast((100 + TaxPercent) as float)/100)"
                        p("ZeroRatedSale") = "0"
                        p("NonVATSale") = "0"
                        p("VATableSale") = "0"
                        p("TaxSales") = "0"
                        p("TaxAmount") = "0"
                        pSql = p.GetUpdate(" InvoiceID = " & IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim())) & " AND OriginalUnitPrice = UnitPrice")
                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        '''Save Discount on Invoice Item
                        p = New InsertUpdate("TransInvoiceItems")
                        p("DiscountAmount") = "VATExemptSale - (VATExemptSale * (cast((100 - " & CDec2(objSalesDiscount("DiscountPercent").ToString()) & ") as float)/100))"
                        p("DiscountSales") = "0" 'just make this 0
                        pSql = p.GetUpdate(" InvoiceID = " & IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim())) & " AND OriginalUnitPrice = UnitPrice")
                        Common.WebRequest.ProData.ExecuteSQL(pSql)


                    Else


                        '''Save Discount on Invoice Item
                        p = New InsertUpdate("TransInvoiceItems")
                        p("DiscountAmount") = "Subtotal - (Subtotal * (cast((100 - " & CDec2(objSalesDiscount("DiscountPercent").ToString()) & ") as float)/100))"
                        p("DiscountSales") = " (" & "Subtotal - (Subtotal * (cast((100 - " & CDec2(objSalesDiscount("DiscountPercent").ToString()) & ") as float)/100))" & ")/ (cast((100 + TaxPercent) as float)/100)"
                        pSql = p.GetUpdate(" InvoiceID = " & IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim())) & " AND OriginalUnitPrice = UnitPrice")
                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                        '''Save Discount on Invoice Item
                        p = New InsertUpdate("TransInvoiceItems")
                        p("TaxSales") = "(Subtotal - DiscountAmount) / (cast((100 + TaxPercent) as float)/100)"
                        p("TaxAmount") = "(Subtotal - DiscountAmount) - ((Subtotal - DiscountAmount) / (cast((100 + TaxPercent) as float)/100))"
                        'additional by janine 'feb 17
                        p("VATExemptSale") = "0"
                        p("ZeroRatedSale") = "0"
                        p("NonVATSale") = "0"
                        p("VATableSale") = "(Subtotal)"
                        pSql = p.GetUpdate(" InvoiceID = " & IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim())) & " AND OriginalUnitPrice = UnitPrice")
                        Common.WebRequest.ProData.ExecuteSQL(pSql)



                    End If



                End If

            Next objSalesDiscount

            ''' 


            '''check if discount type is Senior Citizen Discount, PWD, or special senior citizen discount. if yes, thats the only time you save
            pSql = "DELETE FROM dbo.TransInvoiceSeniorCitizenPWDCustomer WHERE InvoiceID = " & ReadyString(InvoiceID.Trim())

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            For Each objSalesSeniorCitizenPWD As Object In SalesPaymentSeniorCitizenPWDCustomerList

                If (objSalesSeniorCitizenPWD("ID").ToString().Trim() <> "") Then
                    '''Save Senior Citizen PWD Customer
                    ''' 
                    p = New InsertUpdate("TransInvoiceSeniorCitizenPWDCustomer")

                    p("InvoiceID") = IIf(InvoiceID.Trim().Length = 0, "NULL", ReadyString(InvoiceID.Trim()))
                    p("CustomerID") = IIf(objSalesSeniorCitizenPWD("ID").ToString().Trim().Length = 0, "NULL", ReadyString(objSalesSeniorCitizenPWD("ID").ToString().Trim()))

                    strIDRec = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(strIDRec)
                    pSql = p.GetInsert()

                    Common.WebRequest.ProData.ExecuteSQL(pSql)


                End If

            Next objSalesSeniorCitizenPWD


            Common.WebRequest.ProData.CommitTransaction()
            Return "ok"
        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return ex.Message
        End Try



    End Function
    Function UpdateInvoicePrintCount() As String
        Dim p As New InsertUpdate("TransInvoice")
        Dim pSql As String
        Dim strIDReturn As String

        Try

            p("PrintCount") = "ISNULL(PrintCount,0) + 1"
            p("ReprintedDate") = "getdate()"

            strIDReturn = InvoiceID
            pSql = p.GetUpdate(" ID = " & ReadyString(InvoiceID))

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Throw
        End Try

        Return strIDReturn

    End Function

    Function getReceiptHeaderInfo(ByVal strID As String, Optional mProdata As ProData = Nothing, Optional POSAccredNo As String = "", Optional DeviceID As String = "") As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder

            Try
                .ProData = Common.WebRequest.ProData
            Catch ex As Exception
                .ProData = mProdata
            End Try

            Try
                POSAccredNo = WebRequest.Instance.Business.SystemDefaults.GetSystemDefaultPublic("Mobiz POS Accreditation Number")
            Catch ex As Exception
            End Try

            With .Fields
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "rsetbr.OrganizationID"
                    .Searchable = False
                End With
                With .Add("CorporateName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(dev.BusinessRegisteredName)), ''), rsetorg.CompanyName)"
                End With
                With .Add("OrganizationBranchID", SqlDbType.UniqueIdentifier)
                    .Expression = "rsetplace.FowstID"
                    .Searchable = False
                End With
                With .Add("BusinessTradeName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(dev.BusinessName)), ''), rsettrdname.BusinessTradeName)"
                End With
                With .Add("CompleteAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(rsetbr.StreetNumber)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(rsetbr.Street)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cty.Name)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ctry.Name)) + ', ',', '), '')"
                End With
                With .Add("TelNo", SqlDbType.VarChar)
                    .Expression = "dbo.GetOrganizationFOWSTTelNos(rsetplace.FowstID)"
                End With
                With .Add("TIN", SqlDbType.VarChar)
                    .Expression = "ISNULL(dev.TinDisplay, rsetorg.TinNumber + '-' + rsetbr.TINBranchCode)"
                End With
                With .Add("POSPermitNo", SqlDbType.VarChar)
                    .Expression = "devfowst.POSRegistrationNo"
                End With
                With .Add("PTUDateIssued", SqlDbType.VarChar)
                    .Expression = "devfowst.PTUDateIssued"
                End With
                With .Add("POSAccreditationNo", SqlDbType.VarChar)
                    .Expression = "'" & POSAccredNo & "'"
                End With
                With .Add("MIN", SqlDbType.VarChar)
                    '''                    .Expression = "LTRIM(RTRIM(CAST(rsetbr.FOWSTNo as CHAR))) +'-'+ LTRIM(RTRIM(CAST(dev.DeviceNo as CHAR)))"
                    .Expression = "devfowst.BIRMachineNo"
                End With
                With .Add("IsShowSalesRep", SqlDbType.Bit)
                    .Expression = "rset.IsShowSalesRep"
                End With
                With .Add("IsAuthorizedForPOS", SqlDbType.Bit)
                    .Expression = "devfowst.IsAuthorizedForPOS"
                End With
                With .Add("ShowCompanyDetailsInHeader", SqlDbType.Bit)
                    .Expression = "devfowst.ShowCompanyDetailsInHeader"
                End With
            End With

            Try
                If String.IsNullOrWhiteSpace(DeviceID) Then
                    DeviceID = Common.WebRequest.Data("DeviceID")
                    If DeviceID Is Nothing Then
                        DeviceID = WebRequest.Instance.FowstID
                    End If
                End If
            Catch ex As Exception
                Debug.Print("vbURVXKMPB9M! unknown catch")
            End Try

            .SqlFrom = " FROM dbo.LibraryReceiptSettingsFowst rsetplace" & vbCrLf & _
                " INNER JOIN dbo.LibraryReceiptSettings rset ON (rsetplace.ReceiptSettingsID = rset.ID)" & vbCrLf & _
                " INNER JOIN dbo.MobizLibraryOrganizationFOWST rsetbr ON (rsetplace.FowstID = rsetbr.ID)" & vbCrLf & _
                " INNER JOIN dbo.MobizLibraryOrganization rsetorg ON (rsetbr.OrganizationID = rsetorg.ID)" & vbCrLf & _
                " LEFT JOIN dbo.MobizLibraryOrganizationTradeName rsettrdname ON (rset.OrgTradeNameID = rsettrdname.ID)" & vbCrLf & _
                " LEFT JOIN dbo.MobizLibraryCities cty ON (rsetbr.FowstCityID = cty.ID)" & vbCrLf & _
                " LEFT JOIN dbo.MobizLibraryCountries ctry ON (rsetbr.FowstCountryID = ctry.ID)" & vbCrLf & _
                " LEFT JOIN dbo.LibraryDeviceFowst devfowst ON (rsetplace.FowstID = devfowst.FOWSTID " & " AND devfowst.DeviceID = " & ReadyString(DeviceID) & ")" & vbCrLf & _
                " LEFT JOIN dbo.LibraryDevice dev ON (devfowst.DeviceID = dev.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE rsetplace.FowstID IS NULL", " WHERE rsetplace.FowstID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesPaymentInvoiceHeaderInfo(ByVal strID As String, Optional mProdata As ProData = Nothing) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            Try
                .ProData = Common.WebRequest.ProData
            Catch ex As Exception
                .ProData = mProdata
            End Try

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.ID"
                    .Searchable = False
                End With
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "subscrbr.OrganizationID"
                    .Searchable = False
                End With
                With .Add("OrganizationBranchID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.FowstID"
                    .Searchable = False
                End With
                With .Add("DeviceFowstTransactionNumber", SqlDbType.Int)
                    .Expression = "inv.DeviceFowstTransactionNumber"
                End With
                With .Add("InvoiceNumber", SqlDbType.Int)
                    .Expression = "inv.InvoiceNumber"
                End With
                With .Add("InvoiceDate", SqlDbType.DateTime)
                    .Expression = "inv.InvoiceDate"
                End With
                'additional by janine august 1
                With .Add("SalesOrderNo", SqlDbType.Int)
                    .Expression = "inv.SalesOrderNo"
                End With
                With .Add("SalesOrderDate", SqlDbType.DateTime)
                    .Expression = "inv.SalesOrderDate"
                End With
                'end of additional
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.CustomerID"
                End With
                With .Add("DeliveryMethod", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.DeliveryMethod"
                End With
                With .Add("CustomerName", SqlDbType.VarChar)
                    .Expression = "(CASE inv.IsForGovReport " &
                                  " WHEN 1 THEN " &
                                  "        (CASE cust.IsOrganization " &
                                  "             WHEN 1 THEN  cust.CompanyName " &
                                  "             ELSE  " &
                                  "                 ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonSurname)) + ', ',', '), '') + " &
                                  "                 ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonFirstName)) + ' ',' '), '') + " &
                                  "                 ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonMiddleName)) + ' ',' '), '') + " &
                                  "                 ISNULL(NULLIF(', ' + LTRIM(RTRIM(cust.PersonSuffixName)),', '), '') " &
                                  "         END) " &
                                  " ELSE " &
                                  "     (CASE inv.DeliveryMethod  " &
                                  "      WHEN '' THEN '[CASH PAYMENT]' " &
                                  "      ELSE " &
                                  "        (CASE cust.IsOrganization " &
                                  "             WHEN 1 THEN  cust.CompanyName " &
                                  "             ELSE  " &
                                  "                 ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonSurname)) + ', ',', '), '') + " &
                                  "                 ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonFirstName)) + ' ',' '), '') + " &
                                  "                 ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonMiddleName)) + ' ',' '), '') + " &
                                  "                 ISNULL(NULLIF(', ' + LTRIM(RTRIM(cust.PersonSuffixName)),', '), '') " &
                                  "         END) " &
                                  "     END) " &
                                  " END) "

                End With
                With .Add("CustomerAddress", SqlDbType.VarChar)
                    .Expression = "(CASE inv.IsForGovReport " &
                                  " WHEN 1 THEN " &
                                  "         (CASE inv.DeliveryMethod  " &
                                  "          WHEN 'Pickup' THEN ISNULL(NULLIF(LTRIM(RTRIM(custbilladd.StreetNumber)), ''), '') + " &
                                  "                             ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbilladd.Street)),', '), '') + " &
                                  "                             ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillcty.Name)),', '), '') + " &
                                  "                             ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillctry.Name)),', '), '') " &
                                  "          ELSE ISNULL(NULLIF(LTRIM(RTRIM(custdeladd.StreetNumber)), ''), '') + " &
                                  "               ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdeladd.Street)),', '), '') + " &
                                  "               ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdeladdcty.Name)),', '), '') + " &
                                  "               ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdeladdctry.Name)),', '), '') " &
                                  "          END) " &
                                  " ELSE " &
                                  "     (CASE inv.DeliveryMethod  " &
                                  "       WHEN 'Pickup' THEN '' " &
                                  "       ELSE ISNULL(NULLIF(LTRIM(RTRIM(custbilladd.StreetNumber)), ''), '') + " &
                                  "            ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbilladd.Street)),', '), '') + " &
                                  "            ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillcty.Name)),', '), '') + " &
                                  "            ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillctry.Name)),', '), '') " &
                                  "       END) " &
                                  " END)"
                End With
                With .Add("CustomerTIN", SqlDbType.VarChar)
                    .Expression = "(CASE inv.IsForGovReport " &
                            " WHEN 1 THEN cust.TinNumber " &
                            " ELSE '' " &
                            " END)"
                End With
                With .Add("BillingAddressID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.BillingAddressID"
                    .Searchable = False
                End With
                With .Add("BillingAddress", SqlDbType.VarChar)
                    .Expression = "(CASE inv.DeliveryMethod  " &
                                  " WHEN 'Pickup' THEN '' " &
                                  " ELSE ISNULL(NULLIF(LTRIM(RTRIM(custbilladd.StreetNumber)), ''), '') + " &
                                  "      ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbilladd.Street)),', '), '') + " &
                                  "      ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillcty.Name)),', '), '') + " &
                                  "      ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillctry.Name)),', '), '') " &
                                  " END)"
                End With
                With .Add("DeliveryAddressID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.DeliveryAddressID"
                    .Searchable = False
                End With
                With .Add("DeliveryAddress", SqlDbType.VarChar)
                    .Expression = "(CASE inv.DeliveryMethod  " &
                                  " WHEN 'Pickup' THEN '' " &
                                  " ELSE ISNULL(NULLIF(LTRIM(RTRIM(custdeladd.StreetNumber)), ''), '') + " &
                                  "      ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdeladd.Street)),', '), '') + " &
                                  "      ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdeladdcty.Name)),', '), '') + " &
                                  "      ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdeladdctry.Name)),', '), '') " &
                                  " END)"
                End With
                With .Add("CashierName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(cshr.SurName)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '') "
                End With
                With .Add("ReceiptFooterMsg", SqlDbType.VarChar)
                    .Expression = "LTRIM(RTRIM(ISNULL((SELECT DefaultValue" &
                            " FROM SystemDefaults " &
                            " WHERE Description = 'Receipt Footer Message'),'This Serves as Your Sales Invoice.'))) "
                End With
                With .Add("CustomerTableNo", SqlDbType.VarChar)
                    .Expression = "inv.CustomerTableNo"
                End With

                With .Add("OfficeAddress", SqlDbType.VarChar)
                    .Expression = "CASE WHEN ISNULL(cust.OfficeAddressBuildingNo, '') = '' OR ISNULL(cust.OfficeAddressStreet, '') = '' THEN '' ELSE ISNULL(NULLIF(LTRIM(RTRIM(cust.OfficeAddressBuildingNo)), ''), '') + " &
                        "ISNULL(NULLIF(', ' + LTRIM(RTRIM(cust.OfficeAddressStreet)),', '), '') +  " &
                        "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillcty.Name)),', '), '') +  " &
                        "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custctry.Name)),', '), '') END "
                End With

                With .Add("UuidDisplay", SqlDbType.VarChar)
                    .Expression = "(SELECT ISNULL(NULLIF(LTRIM(RTRIM(UuidDisplay)), ''), UUID)" & vbCrLf &
                        " FROM LibraryDevice" & vbCrLf &
                        " WHERE ID = inv .DeviceID)"
                End With

                With .Add("TaxSales", SqlDbType.VarChar)
                    .Expression = "inv.Vatable"
                End With

                With .Add("TaxAmount", SqlDbType.VarChar)
                    .Expression = "inv.VAT"
                End With

                With .Add("VATExemptSale", SqlDbType.VarChar)
                    .Expression = "inv.VATExempt"
                End With

                With .Add("ZeroRatedSale", SqlDbType.VarChar)
                    .Expression = "0"
                End With

                With .Add("ForegoneVAT", SqlDbType.VarChar)
                    .Expression = "inv.ForegoneVAT"
                End With

                With .Add("PrintCount", SqlDbType.VarChar)
                    .Expression = "inv.PrintCount"
                End With
            End With

            .SqlFrom = " FROM dbo.TransInvoice inv " &
                    " LEFT JOIN dbo.MobizLibraryOrganization cust ON (inv.CustomerID = cust.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCities custcty ON (cust.OfficeAddressCityID = custcty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCountries custctry ON (cust.OfficeAddressCountryID = custctry.ID) " &
                    " LEFT JOIN dbo.MobizLibraryOrganizationFOWST custbilladd ON (inv.BillingAddressID = custbilladd.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCities custbillcty ON (custbilladd.FowstCityID = custbillcty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCountries custbillctry ON (custbilladd.FowstCountryID = custbillctry.ID) " &
                    " LEFT JOIN dbo.MobizLibraryOrganizationFOWST custdeladd ON (inv.DeliveryAddressID = custdeladd.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCities custdeladdcty ON (custdeladd.FowstCityID = custdeladdcty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCountries custdeladdctry ON (custdeladd.FowstCountryID = custdeladdctry.ID) " &
                    " LEFT JOIN dbo.MobizLibraryOrganizationFOWST subscrbr ON (inv.FowstID = subscrbr.ID)  " &
                    " LEFT JOIN dbo.MobizLibraryPeople cshr ON (inv.UserID = cshr.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE inv.ID IS NULL", " WHERE inv.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesPaymentReceiptBodyInfo(ByVal strID As String, Optional mProdata As ProData = Nothing) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            Try
                .ProData = Common.WebRequest.ProData
            Catch ex As Exception
                .ProData = mProdata
            End Try


            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "invitem.ID"
                    .Searchable = False
                End With
                With .Add("StockID", SqlDbType.UniqueIdentifier)
                    .Expression = "invitem.ProductID"
                    .Searchable = False
                End With
                With .Add("StockNumber", SqlDbType.VarChar)
                    .Expression = "prd.ProductNo"
                End With
                With .Add("StockDescription", SqlDbType.VarChar)
                    .Expression = " prd.ProductName"
                End With
                With .Add("OrderedQuantity", SqlDbType.Int)
                    .Expression = "invitem.OrderedQuantity"
                End With
                With .Add("Quantity", SqlDbType.Int)
                    .Expression = "invitem.Quantity"
                End With
                With .Add("ActualQuantity", SqlDbType.Int)
                    .Expression = "invitem.ActualQuantity"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "invitem.UnitMeasure"
                End With
                With .Add("UnitPrice", SqlDbType.Money)
                    .Expression = "invitem.UnitPrice"
                End With
                With .Add("SubTotal", SqlDbType.Money)
                    .Expression = "invitem.SubTotal"
                End With
                With .Add("SalesPerson", SqlDbType.VarChar)
                    .Expression = "(SELECT ISNULL(STUFF(" &
                            " (SELECT '; ' + (ppl.Surname + ', ' + ppl.FirstName + ' ' + ppl.MiddleName) " &
                            " FROM TransInvoiceItemSalesReps srep " &
                            " INNER JOIN MobizLibraryPeople ppl ON (srep.PeopleID = ppl.ID) " &
                            " WHERE srep.InvoiceID = invitem.InvoiceID " &
                            " AND srep.ProductID = invitem.ProductID " &
                            " ORDER BY (ppl.Surname + ', ' + ppl.FirstName + ' ' + ppl.MiddleName) ASC " &
                            " FOR XML PATH ('')) " &
                            " , 1, 1, ''),'') " &
                            ")"
                End With
            End With

            .SqlFrom = " FROM TransInvoiceItems invitem " &
                    " INNER JOIN MobizLibraryPIMProduct prd ON (invitem.ProductID = prd.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE invitem.InvoiceID IS NULL", " WHERE invitem.InvoiceID = " + ReadyString(strID)) & vbCrLf &
                " AND invitem.Quantity > 0" & vbCrLf

            .SqlOrder = " ORDER BY prd.ProductNo ASC, invitem.ActualQuantity DESC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesPaymentReceiptSCPWDDiscount(ByVal strInvoiceID As String)
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pOut As New JsonDictionary

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                .Add("Vatable", SqlDbType.Money)
                .Add("VAT", SqlDbType.Money)
                .Add("VATExempt", SqlDbType.Money)
                .Add("ForegoneVAT", SqlDbType.Money)
                .Add("PhNatTeamDiscount", SqlDbType.Money)
                .Add("SoloParentDiscount", SqlDbType.Money)
                .Add("SCDiscount", SqlDbType.Money)
                .Add("SCDiscountExempt", SqlDbType.Money)
                .Add("GrossSalesToRegDiner", SqlDbType.Money)
                .Add("GrossSalesToSC", SqlDbType.Money)
                With .Add("DiscountPercentage", SqlDbType.Int)
                    .Expression = " disc.DiscountPercentage "
                End With
                With .Add("NoOfPeople", SqlDbType.Int)
                    .Expression = " disc.NoOfPeople "
                End With
                With .Add("NoOfSeniorCitizens", SqlDbType.Int)
                    .Expression = " disc.NoOfSeniorCitizens "
                End With
                With .Add("DiscountTypeID", SqlDbType.VarChar)
                    .Expression = " disc.DiscountTypeID "
                End With
            End With

            .SqlFrom = " FROM TransInvoice " & vbCrLf &
                        " OUTER APPLY (SELECT DiscountTypeID, DiscountPercentage, NoOfPeople, NoOfSeniorCitizens FROM TransInvoiceDiscount WHERE InvoiceID = " & ReadyString(strInvoiceID) & ") as disc "

            .SqlWhere = " WHERE ID = " + ReadyString(strInvoiceID)

            pDT = .Requery

        End With

        pOut.Add("SalesPaymentData", pDT)

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("CustomerName", SqlDbType.VarChar)
                    .Expression = "CASE WHEN PersonFullName IS NULL OR PersonFullName = '' THEN PersonFirstName + ' ' + PersonSurname ELSE PersonFullName END "
                End With
                With .Add("IDNo", SqlDbType.VarChar)
                    .Expression = "CASE WHEN PersonSeniorCitizensIDNo IS NOT NULL THEN PersonSeniorCitizensIDNo" & vbCrLf &
                    " WHEN PersonPWDIDNo IS NOT NULL THEN PersonPWDIDNo" & vbCrLf &
                    " WHEN PersonPassportNo IS NOT NULL THEN PersonPassportNo" & vbCrLf &
                    " WHEN PersonDriversLicenseNo IS NOT NULL THEN PersonDriversLicenseNo" & vbCrLf &
                    " WHEN PersonSSSNo IS NOT NULL THEN PersonSSSNo" & vbCrLf &
                    " WHEN PersonGSISIDNo IS NOT NULL THEN PersonGSISIDNo" & vbCrLf &
                    " WHEN PersonPRCIDNo IS NOT NULL THEN PersonPRCIDNo" & vbCrLf &
                    " WHEN PersonPostalIDNo IS NOT NULL THEN PersonPostalIDNo" & vbCrLf &
                    " WHEN PersonVotersIDNo IS NOT NULL THEN PersonVotersIDNo" & vbCrLf &
                    " WHEN PersonOtherIDNo IS NOT NULL THEN PersonOtherIDNo" & vbCrLf &
                    " ELSE '' " & vbCrLf &
                    " END "
                End With
            End With

            .SqlFrom = " FROM TransInvoiceSeniorCitizenPWDCustomer as invcus " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganization as cus ON cus.ID = invcus.CustomerID " & vbCrLf

            .SqlWhere = " WHERE invcus.InvoiceID = " + ReadyString(strInvoiceID)

            pDT = .Requery

        End With

        pOut.Add("People", pDT)

        Return pOut.Serialize()

    End Function

    Function getSalesPaymentReceiptSerial(ByVal strInvoiceID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ProductID", SqlDbType.VarChar)
                    .Expression = "invserial.ProductID"
                End With
                With .Add("SerialNumber", SqlDbType.VarChar)
                    .Expression = "invserial.SerialNumber"
                End With
            End With

            .SqlFrom = " FROM TransInvoiceItemSerials invserial "

            .SqlWhere = " WHERE invserial.InvoiceID = " + ReadyString(strInvoiceID)

            .SqlOrder = " ORDER BY invserial.ProductID ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesPaymentReceiptPaymentInfo(ByVal strID As String, Optional mProdata As ProData = Nothing) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            Try
                .ProData = Common.WebRequest.ProData
            Catch ex As Exception
                .ProData = mProdata
            End Try


            With .Fields
                With .Add("CashPayment", SqlDbType.Money)
                    .Expression = "SUM(ISNULL(cpay.CashAmount,0))"
                End With
                With .Add("CheckPaymentAmount", SqlDbType.Money)
                    .Expression = "SUM(ISNULL(cpay.CheckPaymentAmount,0))"
                End With
                With .Add("DebitCardAmount", SqlDbType.Money)
                    .Expression = "SUM(ISNULL(cpay.DebitCardAmount,0))"
                End With
                With .Add("CreditCardAmount", SqlDbType.Money)
                    .Expression = "SUM(ISNULL(cpay.CreditCardAmount,0))"
                End With
                With .Add("SRVAmount", SqlDbType.Money)
                    .Expression = "SUM(ISNULL(cpay.SRVAmount,0))"
                End With
                With .Add("SRVNo", SqlDbType.Money)
                    .Expression = "STUFF((SELECT ', ' + ISNULL(CONVERT(varchar(max), SrvNum),0) FROM TransSalesReturnLedger WHERE SourceMemoID = (SELECT TransCustomerPaymentSRV.MemoID " & vbCrLf &
                            " FROM TransCustomerPaymentSRV " & vbCrLf &
                            " JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentSRV.CustomerPaymentID " & vbCrLf &
                            " WHERE TransCustomerPaymentInvoices.InvoiceID = " & ReadyString(strID) & " AND DestinationPaymentID IS NULL) FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') " & vbCrLf
                End With
                With .Add("CustomerChangeCash", SqlDbType.Money)
                    .Expression = "SUM(ISNULL(cpay.CustomerChangeCash,0))"
                End With
                With .Add("OtherTypeAmount", SqlDbType.Money)
                    .Expression = "SUM(ISNULL(cpay.OtherTypeAmount,0))"
                End With
                With .Add("CustomerEmailAddresses", SqlDbType.VarChar)
                    .Expression = "cpay.CustomerEmailAddresses"
                End With
            End With

            .SqlFrom = " FROM dbo.TransInvoice inv " &
                    " INNER JOIN dbo.TransCustomerPaymentInvoices cpayinv ON (inv.ID = cpayinv.InvoiceID) " &
                    " INNER JOIN dbo.TransCustomerPayment cpay ON (cpayinv.CustomerPaymentID = cpay.ID) "
            .SqlGroup = "GROUP BY cpay.CustomerEmailAddresses"
            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE inv.ID IS NULL", " WHERE inv.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesPaymentReceiptFooterAmountsInfo(ByVal strID As String, Optional mProdata As ProData = Nothing) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            Try
                .ProData = Common.WebRequest.ProData
            Catch ex As Exception
                .ProData = mProdata
            End Try


            With .Fields
                With .Add("SKUCount", SqlDbType.Money)
                    .Expression = "COUNT(invitem.ProductID)"
                End With
                With .Add("TotalAmountDue", SqlDbType.Money)
                    .Expression = "SUM(invitem.GrossSalesNetOfVATExemptions)"
                End With
                With .Add("DiscountAmount", SqlDbType.Money)
                    .Expression = "SUM(ISNULL(invitem.DiscountAmount,0))"
                End With
                With .Add("NetAmountDue", SqlDbType.Money)
                    .Expression = "SUM(invitem.GrossSalesNetOfVATExemptions - ISNULL(invitem.DiscountAmount,0))" ' computed in sql server formula
                End With
                With .Add("TaxSales", SqlDbType.Money)
                    .Expression = "SUM(invitem.TaxSales - invitem.DiscountSales)" 'edited by janine feb 11 2015 kay mali sauna
                End With
                With .Add("TaxAmount", SqlDbType.Money)
                    .Expression = "SUM(invitem.TaxAmount  - invitem.DiscountTax)"
                End With
                With .Add("VATExemptTax", SqlDbType.Money)
                    .Expression = "SUM(inv.ForegoneVAT)"
                End With
                With .Add("NonVATSale", SqlDbType.Money)
                    .Expression = "SUM(ISNULL(invitem.NonVATSale,0))"
                End With
                With .Add("VATExemptSale", SqlDbType.Money)
                    .Expression = "SUM(ISNULL(invitem.VATExemptSale,0))"
                End With
                With .Add("ZeroRatedSale", SqlDbType.Money)
                    .Expression = "SUM(ISNULL(invitem.ZeroRatedSale,0))"
                End With
                With .Add("TaxPercent", SqlDbType.Float)
                    .Expression = "ISNULL((SELECT TOP 1 ISNULL(TaxPercent, 0) FROM dbo.TransInvoiceItems WHERE InvoiceID = invitem.InvoiceID), 0)"
                End With
                With .Add("SubTotal", SqlDbType.Money)
                    .Expression = "SUM(invitem.SubTotal)"
                End With
                With .Add("ChargeType", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT ChargeType FROM dbo.TransInvoiceCharges WHERE InvoiceID = invitem.InvoiceID), 'Charges')"
                End With
                With .Add("ChargeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(ChargeAmount, 0)) FROM dbo.TransInvoiceCharges WHERE InvoiceID = invitem.InvoiceID), 0)"
                End With
                With .Add("TipAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(TipAmount, 0)) FROM dbo.TransInvoiceCharges WHERE InvoiceID = invitem.InvoiceID), 0)"
                End With
                With .Add("LocalTaxAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(TaxAmount, 0)) FROM dbo.TransInvoiceCharges WHERE InvoiceID = invitem.InvoiceID), 0)"
                End With
                With .Add("DiscountPercentage", SqlDbType.Money)
                    .Expression = "tblDiscount.DiscountPercentage"
                End With
                With .Add("DiscountType", SqlDbType.Money)
                    .Expression = "tblDiscount.DiscountType"
                End With
            End With

            .SqlFrom = " FROM TransInvoiceItems invitem " &
                    " LEFT JOIN dbo.TransInvoice inv ON (invitem.InvoiceID = inv.ID) " & vbCrLf &
                    " OUTER APPLY (SELECT InvoiceID, DiscountPercentage, DiscountType FROM TransInvoiceDiscount " & vbCrLf &
                    "   LEFT JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = TransInvoiceDiscount.DiscountTypeID WHERE InvoiceID = inv.ID) as tblDiscount"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE invitem.InvoiceID IS NULL", " WHERE invitem.InvoiceID = " + ReadyString(strID))
            .SqlGroup = " GROUP BY invitem.InvoiceID, tblDiscount.DiscountPercentage, tblDiscount.DiscountType"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesPaymentReceiptFooterMessagesInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("MessageFooter", SqlDbType.VarChar)
                    .Expression = "rsetmsg.MessageFooter"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryReceiptSettingsMessage rsetmsg "

            .SqlOrder = " ORDER BY rsetmsg.SortOrder ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesPaymentSRVReceiptHeaderInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "srvledger.ID"
                    .Searchable = False
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.CustomerID"
                    .Searchable = False
                End With
                With .Add("CustomerName", SqlDbType.VarChar)
                    .Expression = " (CASE cust.IsOrganization " & vbCrLf &
                                  "     WHEN 1 THEN  cust.CompanyName " & vbCrLf &
                                  "     ELSE  " & vbCrLf &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonSurname)) + ', ',', '), '') + " & vbCrLf &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonFirstName)) + ' ',' '), '') + " & vbCrLf &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonMiddleName)) + ' ',' '), '') + " & vbCrLf &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(cust.PersonSuffixName)),', '), '') " & vbCrLf &
                                  " END)" & vbCrLf
                    .Searchable = False
                End With
                With .Add("CustomerAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(cust.OfficeAddressBuildingNo)) + ' ', ' '), '') + " & vbCrLf &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cust.OfficeAddressStreet)) + ', ',', '), '') + " & vbCrLf &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custcty.Name)) + ', ',', '), '') + " & vbCrLf &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custctry.Name)) + ', ',', '), '')" & vbCrLf
                End With
                With .Add("CustomerTIN", SqlDbType.VarChar)
                    .Expression = "cust.TinNumber"
                    .Searchable = False
                End With
                With .Add("SourceMemoID", SqlDbType.UniqueIdentifier)
                    .Expression = "srvledger.SourceMemoID"
                    .Searchable = False
                End With
                With .Add("CustomerPaymentID", SqlDbType.UniqueIdentifier)
                    .Expression = "srvledger.DestinationPaymentID"
                End With
                With .Add("SRVNumber", SqlDbType.VarChar)
                    .Expression = "srvledger.SrvNum"
                    .Searchable = False
                End With
                With .Add("PinCode", SqlDbType.VarChar)
                    .Expression = "srvledger.Pin"
                    .Searchable = False
                End With
                With .Add("SRVDateTime", SqlDbType.DateTime)
                    .Expression = "memo.LastUpdated"
                    .Searchable = False
                End With
                With .Add("SubscriberOrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "retTo.OrganizationID"
                    .Searchable = False
                End With
                With .Add("BoughtToBranchID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.FowstID"
                    .Searchable = False
                End With
                With .Add("BoughtToBranchName", SqlDbType.VarChar)
                    .Expression = "invsubsbr.[Name]"
                End With
                With .Add("ReturnToBranchID", SqlDbType.UniqueIdentifier)
                    .Expression = "srvledger.FowstID"
                    .Searchable = False
                End With
                With .Add("ReturnToBranchName", SqlDbType.VarChar)
                    .Expression = "retTo.[Name]"
                End With
                With .Add("CashierName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(cshr.SurName)) + ', ',', '), '') + " & vbCrLf &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') + " & vbCrLf &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '')" & vbCrLf
                End With
                With .Add("InvoiceNumber", SqlDbType.Int)
                    .Expression = "inv.InvoiceNumber"
                End With
                With .Add("InvoiceAmount", SqlDbType.Money)
                    .Expression = "ISNULL(" & vbCrLf &
                                  "     (SELECT SUM(ISNULL(siitems.SubTotal,0) - ISNULL(siitems.DiscountAmount,0)) as InvoiceAmount " & vbCrLf &
                                  "      FROM TransInvoiceItems siitems " & vbCrLf &
                                  "      WHERE siitems.InvoiceID = inv.ID " & vbCrLf &
                                  "     ) " & vbCrLf &
                                  ",0)" & vbCrLf
                End With
                With .Add("InvoicePayment", SqlDbType.VarChar)
                    .Expression = "ISNULL(" & vbCrLf &
                                  "     (SELECT SUM(ISNULL(sipayment.AmountPaid,0)) as AmountPaid " & vbCrLf &
                                  "      FROM TransCustomerPaymentInvoices sipayment " & vbCrLf &
                                  "      INNER JOIN TransCustomerPayment custpayment ON (sipayment.CustomerPaymentID = custpayment.ID)" & vbCrLf &
                                  "      WHERE sipayment.InvoiceID = inv.ID " & vbCrLf &
                                  "      AND custpayment.IsCancelled = 0 " & vbCrLf &
                                  "     ) " & vbCrLf &
                                  ",0)" & vbCrLf
                End With
                With .Add("SRVOrigAmount", SqlDbType.Money)
                    .Expression = "ISNULL(" & vbCrLf &
                                  "     (SELECT ISNULL(SUM(GrossSrvAmount), 0) AS Amount " & vbCrLf &
                                  "      FROM TransSalesReturnLedger " & vbCrLf &
                                  "      WHERE SourceMemoID = srvledger.SourceMemoID " & vbCrLf &
                                  "      AND DestinationPaymentID IS NULL" & vbCrLf &
                                  "     )" & vbCrLf &
                                  ",0)" & vbCrLf
                End With
                With .Add("SRVNewAmount", SqlDbType.Money)
                    .Expression = "ISNULL(" & vbCrLf &
                                  "     (SELECT SUM(ISNULL(Amount, 0)) - SUM(ISNULL(ForfeitedAmount, 0)) AS Amount " & vbCrLf &
                                  "      FROM TransSalesReturnLedger " & vbCrLf &
                                  "      WHERE SourceMemoID = srvledger.SourceMemoID" & vbCrLf &
                                  "     )" & vbCrLf &
                                  ",0)" & vbCrLf
                End With
                With .Add("SRVRestockingFee", SqlDbType.Money)
                    .Expression = "ISNULL(" & vbCrLf &
                                  "     (SELECT ISNULL(SUM(Fees), 0) AS Amount " & vbCrLf &
                                  "      FROM TransSalesReturnLedger " & vbCrLf &
                                  "      WHERE SourceMemoID = srvledger.SourceMemoID " & vbCrLf &
                                  "      AND DestinationPaymentID IS NULL" & vbCrLf &
                                  "     )" & vbCrLf &
                                  ",0)" & vbCrLf
                End With
            End With

            .SqlFrom = " FROM dbo.TransSalesReturnLedger srvledger " & vbCrLf &
                    " INNER JOIN dbo.TransMemo memo ON (srvledger.SourceMemoID = memo.ID) " & vbCrLf &
                    " INNER JOIN dbo.TransMemoSalesReturn memosret ON (srvledger.SourceMemoID = memosret.MemoID) " & vbCrLf &
                    " INNER JOIN dbo.TransInvoice inv ON (memosret.InvoiceID = inv.ID) " & vbCrLf &
                    " INNER JOIN dbo.MobizLibraryOrganization cust ON (inv.CustomerID = cust.ID) " & vbCrLf &
                    " LEFT JOIN dbo.MobizLibraryCities custcty ON (cust.OfficeAddressCityID = custcty.ID) " & vbCrLf &
                    " LEFT JOIN dbo.MobizLibraryCountries custctry ON (cust.OfficeAddressCountryID = custctry.ID) " & vbCrLf &
                    " LEFT JOIN dbo.MobizLibraryOrganizationFOWST invsubsbr ON (inv.FowstID = invsubsbr.ID)" & vbCrLf &
                    " LEFT JOIN dbo.MobizLibraryOrganizationFOWST retTo ON (srvledger.FowstID = retTo.ID) " & vbCrLf &
                    " LEFT JOIN dbo.MobizLibraryPeople cshr ON (memo.UserID = cshr.ID) " & vbCrLf

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE srvledger.SourceMemoID IS NULL", " WHERE srvledger.SourceMemoID = " + ReadyString(strID)) & vbCrLf &
                    " AND srvledger.DestinationPaymentID IS NULL"

            pDT = .Requery

        End With

        Return pDT


    End Function

    Function getSalesPaymentSRVRefundHeaderInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "srvledger.ID"
                    .Searchable = False
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.CustomerID"
                    .Searchable = False
                End With
                With .Add("CustomerName", SqlDbType.VarChar)
                    .Expression = " (CASE cust.IsOrganization " &
                                  "     WHEN 1 THEN  cust.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(cust.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(cust.PersonSuffixName)),', '), '') " &
                                  " END)"
                    .Searchable = False
                End With
                With .Add("CustomerAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(cust.OfficeAddressBuildingNo)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cust.OfficeAddressStreet)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custcty.Name)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custctry.Name)) + ', ',', '), '')"
                End With
                With .Add("CustomerTIN", SqlDbType.VarChar)
                    .Expression = "cust.TinNumber"
                    .Searchable = False
                End With
                With .Add("SourceMemoID", SqlDbType.UniqueIdentifier)
                    .Expression = "srvledger.SourceMemoID"
                    .Searchable = False
                End With
                With .Add("CustomerPaymentID", SqlDbType.UniqueIdentifier)
                    .Expression = "srvledger.DestinationPaymentID"
                End With
                With .Add("SRVNumber", SqlDbType.VarChar)
                    .Expression = "srvledger.RefundNum"
                    .Searchable = False
                End With
                With .Add("SRVDateTime", SqlDbType.DateTime)
                    .Expression = "memo.LastUpdated"
                    .Searchable = False
                End With
                With .Add("SubscriberOrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "retTo.OrganizationID"
                    .Searchable = False
                End With
                With .Add("BoughtToBranchID", SqlDbType.UniqueIdentifier)
                    .Expression = "inv.FowstID"
                    .Searchable = False
                End With
                With .Add("BoughtToBranchName", SqlDbType.VarChar)
                    .Expression = "invsubsbr.[Name]"
                End With
                With .Add("ReturnToBranchID", SqlDbType.UniqueIdentifier)
                    .Expression = "srvledger.FowstID"
                    .Searchable = False
                End With
                With .Add("ReturnToBranchName", SqlDbType.VarChar)
                    .Expression = "retTo.[Name]"
                End With
                With .Add("CashierName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(cshr.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '')"
                End With
                With .Add("InvoiceNumber", SqlDbType.Int)
                    .Expression = "inv.InvoiceNumber"
                End With
                With .Add("InvoiceAmount", SqlDbType.Money)
                    .Expression = "ISNULL(" &
                                  "     (SELECT SUM(ISNULL(siitems.SubTotal,0) - ISNULL(siitems.DiscountAmount,0)) as InvoiceAmount " &
                                  "      FROM TransInvoiceItems siitems " &
                                  "      WHERE siitems.InvoiceID = inv.ID " &
                                  "     ) " &
                                  ",0)"
                End With
                With .Add("InvoicePayment", SqlDbType.VarChar)
                    .Expression = "ISNULL(" &
                                  "     (SELECT SUM(ISNULL(sipayment.AmountPaid,0)) as AmountPaid " &
                                  "      FROM TransCustomerPaymentInvoices sipayment " &
                                  "      INNER JOIN TransCustomerPayment custpayment ON (sipayment.CustomerPaymentID = custpayment.ID)" &
                                  "      WHERE sipayment.InvoiceID = inv.ID " &
                                  "      AND custpayment.IsCancelled = 0 " &
                                  "     ) " &
                                  ",0)"
                End With
                With .Add("SRVOrigAmount", SqlDbType.Money)
                    .Expression = "ISNULL(" &
                                  "     (SELECT ISNULL(SUM(GrossRefundAmount), 0) AS Amount " &
                                  "      FROM TransSalesReturnLedger " &
                                  "      WHERE SourceMemoID = srvledger.SourceMemoID " &
                                  "      AND DestinationPaymentID IS NULL" &
                                  "     )" &
                                  ",0)"
                End With
                With .Add("SRVNewAmount", SqlDbType.Money)
                    .Expression = "ISNULL(" &
                                  "     (SELECT ISNULL(SUM(GrossRefundAmount), 0) - ISNULL(SUM(RefundFees), 0) AS Amount " &
                                  "      FROM TransSalesReturnLedger " &
                                  "      WHERE SourceMemoID = srvledger.SourceMemoID" &
                                  "     )" &
                                  ",0)"
                End With
                With .Add("SRVRestockingFee", SqlDbType.Money)
                    .Expression = "ISNULL(" &
                                  "     (SELECT ISNULL(SUM(RefundFees), 0) AS Amount " &
                                  "      FROM TransSalesReturnLedger " &
                                  "      WHERE SourceMemoID = srvledger.SourceMemoID " &
                                  "      AND DestinationPaymentID IS NULL" &
                                  "     )" &
                                  ",0)"
                End With
            End With

            .SqlFrom = " FROM dbo.TransSalesReturnLedger srvledger " &
                    " INNER JOIN dbo.TransMemo memo ON (srvledger.SourceMemoID = memo.ID) " &
                    " INNER JOIN dbo.TransMemoSalesReturn memosret ON (srvledger.SourceMemoID = memosret.MemoID) " &
                    " INNER JOIN dbo.TransInvoice inv ON (memosret.InvoiceID = inv.ID) " &
                    " INNER JOIN dbo.MobizLibraryOrganization cust ON (inv.CustomerID = cust.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCities custcty ON (cust.OfficeAddressCityID = custcty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCountries custctry ON (cust.OfficeAddressCountryID = custctry.ID) " &
                    " LEFT JOIN dbo.MobizLibraryOrganizationFOWST invsubsbr ON (inv.FowstID = invsubsbr.ID)" &
                    " LEFT JOIN dbo.MobizLibraryOrganizationFOWST retTo ON (srvledger.FowstID = retTo.ID) " &
                    " LEFT JOIN dbo.MobizLibraryPeople cshr ON (memo.UserID = cshr.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE srvledger.SourceMemoID IS NULL", " WHERE srvledger.SourceMemoID = " + ReadyString(strID)) &
                    " AND srvledger.DestinationPaymentID IS NULL"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesPaymentSRVReceiptInvoices(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("InvoiceID", SqlDbType.UniqueIdentifier)
                    .Expression = "custpayinv.InvoiceID"
                End With
                With .Add("InvoiceNumber", SqlDbType.Int)
                    .Expression = "inv.InvoiceNumber"
                End With
                With .Add("InvoiceDate", SqlDbType.DateTime)
                    .Expression = "inv.InvoiceDate"
                End With
                With .Add("UsedAmount", SqlDbType.Money)
                    .Expression = "(srledger.Amount * -1)"
                End With
            End With

            .SqlFrom = " FROM TransSalesReturnLedger srledger " &
                    " INNER JOIN TransCustomerPaymentInvoices custpayinv ON (srledger.DestinationPaymentID = custpayinv.CustomerPaymentID) " &
                    " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, " WHERE srledger.SourceMemoID = IS NULL", " WHERE srledger.SourceMemoID = " + ReadyString(strID)) &
                    " AND srledger.DestinationPaymentID IS NOT NULL"

            .SqlOrder = " ORDER BY inv.InvoiceDate ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getSalesOrderHeaderInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "rsetbr.OrganizationID"
                    .Searchable = False
                End With
                With .Add("CorporateName", SqlDbType.VarChar)
                    .Expression = "rsetorg.CompanyName"
                End With
                With .Add("OrganizationBranchID", SqlDbType.UniqueIdentifier)
                    .Expression = "rsetplace.FowstID"
                    .Searchable = False
                End With
                With .Add("BusinessTradeName", SqlDbType.VarChar)
                    .Expression = "rsettrdname.BusinessTradeName"
                End With
                With .Add("CompleteAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(rsetbr.StreetNumber)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(rsetbr.Street)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cty.Name)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ctry.Name)) + ', ',', '), '')"
                End With
                With .Add("TelNo", SqlDbType.VarChar)
                    .Expression = "dbo.GetOrganizationFOWSTTelNos(rsetplace.FowstID)"
                End With
                With .Add("TIN", SqlDbType.VarChar)
                    .Expression = "rsetorg.TinNumber"
                End With
                With .Add("IsShowSalesRep", SqlDbType.Bit)
                    .Expression = "rset.IsShowSalesRep"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryReceiptSettingsFowst rsetplace" & vbCrLf & _
                " INNER JOIN dbo.LibraryReceiptSettings rset ON (rsetplace.ReceiptSettingsID = rset.ID)" & vbCrLf & _
                " INNER JOIN dbo.MobizLibraryOrganizationFOWST rsetbr ON (rsetplace.FowstID = rsetbr.ID)" & vbCrLf & _
                " INNER JOIN dbo.MobizLibraryOrganization rsetorg ON (rsetbr.OrganizationID = rsetorg.ID)" & vbCrLf & _
                " LEFT JOIN dbo.MobizLibraryOrganizationTradeName rsettrdname ON (rset.OrgTradeNameID = rsettrdname.ID)" & vbCrLf & _
                " LEFT JOIN dbo.MobizLibraryCities cty ON (rsetbr.FowstCityID = cty.ID)" & vbCrLf & _
                " LEFT JOIN dbo.MobizLibraryCountries ctry ON (rsetbr.FowstCountryID = ctry.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE rsetplace.FowstID IS NULL", " WHERE rsetplace.FowstID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getCustomerPaymentOtherDetails(ByVal strID) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "custpayoth.ID"
                    .Searchable = False
                End With
                With .Add("PaymentType", SqlDbType.VarChar)
                    .Expression = "custpayoth.PaymentType"
                End With
                With .Add("Amount", SqlDbType.Money)
                    .Expression = "custpayoth.Amount"
                End With
                With .Add("CardNumber", SqlDbType.Money)
                    .Expression = "custpayoth.CardNumber"
                End With
            End With

            .SqlFrom = " FROM dbo.TransCustomerPaymentOther custpayoth"

            .SqlWhere = " WHERE custpayoth.CustomerPaymentID = " + ReadyString(strID)
              
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getCustomerPaymentCreditCardDetails(ByVal strID) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "custpaycc.ID"
                    .Searchable = False
                End With
                With .Add("CreditCardType", SqlDbType.VarChar)
                    .Expression = "custpaycc.CreditCardType"
                End With
                With .Add("CreditCardAmount", SqlDbType.Money)
                    .Expression = "custpaycc.CreditCardAmount"
                End With
            End With

            .SqlFrom = " FROM dbo.TransCustomerPaymentCreditCard custpaycc"

            .SqlWhere = " WHERE custpaycc.CustomerPaymentID = " + ReadyString(strID)

            pDT = .Requery

        End With

        Return pDT

    End Function

End Class
