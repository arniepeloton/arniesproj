Imports System.Web.Helpers
Imports Aspose
Imports Aspose.Cells
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class CincoCorporation
    Public DateFrom As String
    Public DateTo As String
    Public FOWSTID As String
    Public DeviceID As String
    Public GroupID As String
    Public StoreStatus As String
    Public Filter As String
    Public SliceBy As String
    Public SliceByFilter As String
    Public DiceBy As String
    Public DiceByFilter As String
    Public SelectedMonth As String
    Public SelectedYear As Integer
    Public dtDeviceAvailabilitySummary As DataTable
    Public InvoiceMonth As Integer
    Public InvoiceYear As Integer
    Public InvoiceID As String

    'Notes in cache invoice table:
    '1. GrossSalesNetOfVATExemptions = Gross amount of the main product + promo in item modifiers
    '2. GrossInvoiceAmount = Gross amount of the main product without the promo

    Function GetVoidItems() As DataTable
        Dim pSql As String

        pSql = " SELECT void.*, pim.ProductNo, pim.ProductName, inv.ID as InvoiceID, ISNULL(inv.InvoiceNumber, 0) as InvoiceNumber, ISNULL(inv.InvoiceDate, '') as InvoiceDate " & vbCrLf &
                    "   FROM TransInvoiceItemsVoided as void " & vbCrLf &
                    "   INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = void.ProductID " & vbCrLf &
                    "   LEFT JOIN TransInvoice as inv ON inv.ID = void.InvoiceID " & vbCrLf &
                    "   WHERE void.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                    "   AND CONVERT(date, void.DateTimeRemoved) = " & ReadyString(DateFrom) & vbCrLf &
                    "   ORDER BY DateTimeRemoved DESC "

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function
    Function GetInvoiceListReport() As DataTable
        Dim pSql As String

        pSql = " SELECT ID, CONVERT(datetime, InvoiceDate, 128) as InvoiceDate, InvoiceNumber, TotalAmount as GrossSales " & vbCrLf &
                " FROM TransInvoice " & vbCrLf &
                " WHERE FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
                " AND DeviceID = " & ReadyString(DeviceID) & vbCrLf &
                " AND CONVERT(date, InvoiceDate) = " & ReadyString(DateFrom) & vbCrLf &
                " ORDER BY InvoiceDate ASC " & vbCrLf

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetInvoiceDetails(Optional isExport As Boolean = False)
        Dim pSql As String
        Dim pOut As New JsonDictionary

        pSql = " IF OBJECT_ID('tempdb..##IL_TransInvoice') IS NOT NULL DROP TABLE ##IL_TransInvoice " & vbCrLf &
                "   SELECT inv.ID, InvoiceDate, RIGHT('000000000000' + CAST(InvoiceNumber AS VARCHAR(12)), 12) as InvoiceNumber, RIGHT('000000000000' + CAST(ISNULL(DeviceFowstTransactionNumber, 0) AS VARCHAR(12)), 12) as DeviceFowstTransactionNumber, ROUND(TotalAmount, 2) as TotalAmount, ROUND(Vatable, 2) as Vatable, ROUND(VAT, 2) as VAT, ROUND(ISNULL(VATExempt, 0), 2) as VATExempt, ROUND(ISNULL(ForegoneVAT, 0), 2) as ForegoneVAT, ROUND(SCDiscount, 2) as SCDiscount, ROUND(SCDiscountExempt, 2) as SCDiscountExempt, ROUND(GrossSalesToSC, 2) as GrossSalesToSC, ROUND(GrossSalesToRegDiner, 2) as GrossSalesToRegDiner, ROUND(ISNULL(PhNatTeamDiscount, 0), 2) as PhNatTeamDiscount, ROUND(ISNULL(SoloParentDiscount, 0), 2) as SoloParentDiscount, ROUND(Charges.Amount, 2) as OtherCharges, " & vbCrLf &
                "       CASE WHEN " & CInt2(Not isExport) & " = 1 AND (SCDiscount > 0 OR PhNatTeamDiscount > 0 OR SoloParentDiscount > 0) THEN " & vbCrLf &
                "           (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + PersonFullName " & vbCrLf &
                "               FROM TransInvoiceSeniorCitizenPWDCustomer " & vbCrLf &
                "               INNER JOIN MobizLibraryOrganization as org ON org.ID = CustomerID " & vbCrLf &
                "               WHERE InvoiceID = inv.ID " & vbCrLf &
                "               FOR XML PATH('')" & vbCrLf &
                "               ), 1, 1,''))" & vbCrLf &
                "       ELSE (SELECT CASE WHEN IsOrganization = 0 THEN CASE WHEN PersonFullName IS NOT NULL THEN PersonFullName + ' ' ELSE '' END + CASE WHEN PersonFirstName IS NOT NULL THEN PersonFirstName + ' ' ELSE '' END + CASE WHEN PersonMiddleName IS NOT NULL THEN PersonMiddleName + ' ' ELSE '' END + CASE WHEN PersonSurname IS NOT NULL THEN PersonSurname + ' ' ELSE '' END ELSE CompanyName END FROM MobizLibraryOrganization WHERE ID = inv.CustomerID) END as CustomerName, " & vbCrLf &
                "       ISNULL((SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + ISNULL(PersonSeniorCitizensIDNo, '') + ISNULL(PersonPWDIDNo, '') + ISNULL(PersonOtherIDNo, '') + ISNULL(PersonPassportNo, '') + ISNULL(PersonDriversLicenseNo, '') + ISNULL(PersonSSSNo, '') + ISNULL(PersonGSISIDNo, '') + ISNULL(PersonPRCIDNo, '') + ISNULL(PersonPostalIDNo, '') + ISNULL(PersonVotersIDNo, '')  " & vbCrLf &
                "           FROM TransInvoiceSeniorCitizenPWDCustomer " & vbCrLf &
                "           INNER JOIN MobizLibraryOrganization as org ON org.ID = CustomerID " & vbCrLf &
                "           WHERE InvoiceID = inv.ID " & vbCrLf &
                "           FOR XML PATH('')" & vbCrLf &
                "       ), 1, 1,'')), (SELECT TinNumber FROM MobizLibraryOrganization WHERE ID = inv.CustomerID)) as CustomerIDNo, " & vbCrLf &
                "       (SELECT OfficeAddressBuildingNo + ', ' + OfficeAddressStreet + ', ' + MobizLibraryCities.Name FROM MobizLibraryOrganization INNER JOIN MobizLibraryCities ON MobizLibraryOrganization.OfficeAddressCityID = MobizLibraryCities.ID WHERE MobizLibraryOrganization.ID = inv.CustomerID) as CustomerAddress, " & vbCrLf &
                "       (SELECT BusinessIndustryText FROM MobizLibraryOrganization WHERE MobizLibraryOrganization.ID = inv.CustomerID) as CustomerBusinessStyle, " & vbCrLf &
                "       CASE WHEN ISNULL(SCDiscount, 0) = 0 AND ISNULL(PhNatTeamDiscount, 0) = 0 AND ISNULL(SoloParentDiscount, 0) = 0 THEN (SELECT pay.DiscountAmount FROM TransCustomerPaymentInvoices as payinv INNER JOIN TransCustomerPayment as pay ON pay.ID = payinv.CustomerPaymentID WHERE InvoiceID = inv.ID) ELSE 0 END as OtherDiscount " & vbCrLf &
                "       , ISNULL(CONVERT(varchar(MAX), discount.DiscountTypeID), '') as DiscountTypeID, ISNULL(discount.DiscountPercentage, '') as DiscountPercentage, ISNULL(discount.DiscountType, '') as DiscountType " & vbCrLf &
                "       , cashier.FirstName + ' ' + cashier.Surname as CashierName " & vbCrLf &
                "       , CASE WHEN ISNULL(MobizLibraryOrganizationFOWST.IsVAT, 1) = 1 THEN 'VAT' ELSE 'Non-VAT' END as StoreVatType " & vbCrLf &
                "   INTO ##IL_TransInvoice " & vbCrLf &
                "   FROM TransInvoice as inv " & vbCrLf &
                "   LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = inv.FowstID " & vbCrLf &
                "   INNER JOIN MobizLibraryPeople as cashier ON cashier.ID = inv.UserID " & vbCrLf &
                "   --OUTER APPLY( " & vbCrLf &
                "   --    SELECT CustomerID , PersonFullName, ISNULL(PersonSeniorCitizensIDNo, '') + ISNULL(PersonPWDIDNo, '') + ISNULL(PersonOtherIDNo, '') + ISNULL(PersonPassportNo, '') + ISNULL(PersonDriversLicenseNo, '') + ISNULL(PersonSSSNo, '') + ISNULL(PersonGSISIDNo, '') + ISNULL(PersonPRCIDNo, '') + ISNULL(PersonPostalIDNo, '') + ISNULL(PersonVotersIDNo, '') as CustomerIDNo " & vbCrLf &
                "   --    FROM TransInvoiceSeniorCitizenPWDCustomer " & vbCrLf &
                "   --    INNER JOIN MobizLibraryOrganization as org ON org.ID = CustomerID " & vbCrLf &
                "   --    WHERE InvoiceID = inv.ID " & vbCrLf &
                "   --) as customer " & vbCrLf &
                "   OUTER APPLY( " & vbCrLf &
                "       SELECT ISNULL(SUM(ChargeAmount), 0) as Amount " & vbCrLf &
                "       FROM TransInvoiceCharges " & vbCrLf &
                "       WHERE InvoiceID = inv.ID " & vbCrLf &
                "   ) as Charges " & vbCrLf &
                "   OUTER APPLY( " & vbCrLf &
                "       SELECT DiscountTypeID, DiscountPercentage, DiscountType " & vbCrLf &
                "       FROM LibraryDiscountSettings as disc " & vbCrLf &
                "       INNER JOIN TransInvoiceDiscount ON DiscountTypeID = disc.ID " & vbCrLf &
                "       WHERE InvoiceID = inv.ID " & vbCrLf &
                "   ) as discount " & vbCrLf &
                "   WHERE inv.ID = " & ReadyString(InvoiceID) & vbCrLf
        Common.WebRequest.ProData.Execute(pSql)

        pSql = " IF OBJECT_ID('tempdb..##IL_TransCustomerPaymentInvoices') IS NOT NULL DROP TABLE ##IL_TransCustomerPaymentInvoices " & vbCrLf &
                " SELECT * INTO ##IL_TransCustomerPaymentInvoices FROM( " & vbCrLf &
                "   SELECT payinv.CustomerPaymentID, InvoiceID, ISNULL(CashAmount, 0) + ISNULL(CreditCardAmount, 0) + ISNULL(DebitCardAmount, 0) + ISNULL(SRVAmount, 0) + ISNULL(OnCreditAmount, 0) as Amount, " & vbCrLf &
                "       CASE WHEN CashAmount > 0 THEN  'Cash' " & vbCrLf &
                "           WHEN CreditCardAmount > 0 THEN 'Credit Card' " & vbCrLf &
                "           WHEN DebitCardAmount > 0 THEN 'Debit Card' " & vbCrLf &
                "           WHEN SRVAmount > 0 THEN 'SRV' " & vbCrLf &
                "           WHEN OnCreditAmount > 0 THEN 'On Credit' " & vbCrLf &
                "       END AS PaymentType " & vbCrLf &
                "   , '' as CardNumber " & vbCrLf &
                "   , STUFF((SELECT ', ' + ISNULL(CONVERT(varchar(max), SrvNum),0) FROM TransSalesReturnLedger WHERE SourceMemoID = (SELECT TransCustomerPaymentSRV.MemoID " & vbCrLf &
                "       FROM TransCustomerPaymentSRV " & vbCrLf &
                "       JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentSRV.CustomerPaymentID " & vbCrLf &
                "       WHERE TransCustomerPaymentInvoices.InvoiceID = payinv.InvoiceID AND DestinationPaymentID IS NULL) FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') as SRVNo" & vbCrLf &
                "   FROM TransCustomerPaymentInvoices as payinv " & vbCrLf &
                "   INNER JOIN TransCustomerPayment as pay ON pay.ID = payinv.CustomerPaymentID " & vbCrLf &
                "   WHERE InvoiceID IN (SELECT ID FROM ##IL_TransInvoice) " & vbCrLf & vbCrLf &
                "   UNION ALL " & vbCrLf & vbCrLf &
                "   SELECT payinv.CustomerPaymentID, InvoiceID, ISNULL(otherpay.Amount, 0) as Amount, " & vbCrLf &
                "       otherpay.PaymentType AS PaymentType " & vbCrLf &
                "   , ISNULL(STUFF((SELECT ', ' + ISNULL(CONVERT(varchar(max), CardNumber),0) FROM TransCustomerPaymentOther WHERE ID = otherpay.ID FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''), '') as CardNumber " & vbCrLf &
                "   , '' as SRVNo" & vbCrLf &
                "   FROM TransCustomerPaymentInvoices as payinv " & vbCrLf &
                "   INNER JOIN TransCustomerPaymentOther as otherpay ON otherpay.CustomerPaymentID = payinv.CustomerPaymentID " & vbCrLf &
                "   WHERE InvoiceID IN (SELECT ID FROM ##IL_TransInvoice) " & vbCrLf &
                "   ) as payments " & vbCrLf

        Common.WebRequest.ProData.Execute(pSql)

        pSql = " IF OBJECT_ID('tempdb..##IL_TransInvoiceItems') IS NOT NULL DROP TABLE ##IL_TransInvoiceItems " & vbCrLf &
                "   SELECT TransInvoiceItems.ID, InvoiceID, ProductID, ProductNo, ProductName, CONVERT(int, OrderedQuantity) as OrderedQuantity, UnitPrice, SubTotal, UnitMeasure " & vbCrLf &
                "   INTO ##IL_TransInvoiceItems " & vbCrLf &
                "   FROM TransInvoiceItems " & vbCrLf &
                "   INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = ProductID " & vbCrLf &
                "   WHERE InvoiceID IN (SELECT ID FROM ##IL_TransInvoice) " & vbCrLf

        Common.WebRequest.ProData.Execute(pSql)

        pSql = " IF OBJECT_ID('tempdb..##IL_TransInvoiceCharges') IS NOT NULL DROP TABLE ##IL_TransInvoiceCharges " & vbCrLf &
                "   SELECT TransInvoiceCharges.ChargeType, ISNULL(ChargeAmount, 0) + ISNULL(TaxAmount, 0) + ISNULL(TipAmount, 0) as Amount " & vbCrLf &
                "   INTO ##IL_TransInvoiceCharges " & vbCrLf &
                "   FROM TransInvoiceCharges " & vbCrLf &
                "   WHERE InvoiceID = " & ReadyString(InvoiceID) & vbCrLf &
                "   UNION SELECT '' as ChargeType, 0 as Amount " & vbCrLf

        Common.WebRequest.ProData.Execute(pSql)

        pSql = " IF OBJECT_ID('tempdb..##IL_TransInvoiceItemsModifier') IS NOT NULL DROP TABLE ##IL_TransInvoiceItemsModifier " & vbCrLf &
                " SELECT InvoiceItemID, pim.ProductName, CONVERT(int, OrderedQty) as OrderedQty, ModifierPrice, SubTotal, modif.PremiumPrice " & vbCrLf &
                " INTO ##IL_TransInvoiceItemsModifier " & vbCrLf &
                " FROM TransInvoiceItemsModifier as modif " & vbCrLf &
                " INNER JOIN LibraryItemModifierInventoryAffecting as itemmod ON itemmod.ID = modif.ModifierID " & vbCrLf &
                " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = itemmod.ProductID " & vbCrLf &
                " WHERE InvoiceItemID IN (SELECT ID FROM ##IL_TransInvoiceItems) " & vbCrLf

        Common.WebRequest.ProData.Execute(pSql)

        Dim invoice As DataTable = Common.WebRequest.ProData.Execute("SELECT * FROM ##IL_TransInvoice ORDER BY InvoiceDate")
        Dim charges As DataTable = Common.WebRequest.ProData.Execute("SELECT * FROM ##IL_TransInvoiceCharges ")
        Dim payments As DataTable = Common.WebRequest.ProData.Execute("SELECT * FROM ##IL_TransCustomerPaymentInvoices WHERE InvoiceID = " & ReadyString(invoice.Rows(0)("ID")) & " ORDER BY CASE WHEN PaymentType = 'CASH' THEN 1 END, PaymentType")

        If isExport Then

            invoice.Columns.Add("GrossTotal")
            invoice.Columns.Add("AmountDue")
            invoice.Columns.Add("DiscountTypeAbbrev")
            invoice.Columns.Add("NetAmount")
            invoice.Columns.Add("DiscountAmount")
            invoice.Columns.Add("Change")
            invoice.Columns.Add("CustomerBusAdd")
            invoice.Columns.Add("CustomerTin")
            invoice.Columns.Add("CustomerBusStyle")

            If invoice.Rows(0).Item("DiscountTypeID").ToString().ToLower() = "055a0a33-dd98-47ec-add3-8b2009f3944a" Then
                invoice.Rows(0).Item("DiscountTypeAbbrev") = "SC"
            ElseIf invoice.Rows(0).Item("DiscountTypeID").ToString().ToLower() = "34a4032f-a142-4c15-a868-4ac4410434df" Then
                invoice.Rows(0).Item("DiscountTypeAbbrev") = "PWD"
            ElseIf invoice.Rows(0).Item("DiscountTypeID").ToString().ToLower() = "21ebf55b-526c-4276-b3b7-0cbca504f425" Then
                invoice.Rows(0).Item("DiscountTypeAbbrev") = "NAAC"
            ElseIf invoice.Rows(0).Item("DiscountTypeID").ToString().ToLower() = "e95f0b64-1eea-47b4-bde5-b795234b462d" Then
                invoice.Rows(0).Item("DiscountTypeAbbrev") = "SP"
            Else
                invoice.Rows(0).Item("DiscountTypeAbbrev") = invoice.Rows(0).Item("DiscountType")
            End If

            Dim newitems As New DataTable
            Dim grosstotal As Decimal = 0

            newitems.Columns.Add("ProductName")
            newitems.Columns.Add("Qty")
            newitems.Columns.Add("UnitMeasure")
            newitems.Columns.Add("Price")
            newitems.Columns.Add("SubTotal")
            newitems.Columns.Add("PremiumPrice")
            newitems.Columns.Add("IsModifier")

            Dim items As Object
            items = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute("SELECT * FROM ##IL_TransInvoiceItems WHERE InvoiceID = " & ReadyString(invoice.Rows(0)("ID")))))
            For Each item As Object In items
                Dim dRow As DataRow
                dRow = newitems.NewRow

                dRow(0) = item("ProductName")
                dRow(1) = item("OrderedQuantity")
                dRow(2) = item("UnitMeasure")
                dRow(3) = Format(item("UnitPrice"), "#,##0.00")
                dRow(4) = Format(item("SubTotal"), "#,##0.00")
                dRow(5) = ""
                dRow(6) = False
                newitems.Rows.Add(dRow)
                grosstotal += CDec2(item("SubTotal"))

                Dim modifiers As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute("SELECT * FROM ##IL_TransInvoiceItemsModifier WHERE InvoiceItemID = " & ReadyString(item("ID")))))

                For Each modifier As Object In modifiers
                    dRow = newitems.NewRow

                    dRow(0) = modifier("ProductName")
                    dRow(1) = modifier("OrderedQty")
                    dRow(2) = ""
                    dRow(3) = Format(modifier("ModifierPrice"), "#,##0.00")
                    dRow(4) = Format(modifier("SubTotal"), "#,##0.00")
                    dRow(5) = Format(modifier("PremiumPrice"), "#,##0.00")
                    dRow(6) = True
                    newitems.Rows.Add(dRow)
                    grosstotal += CDec2(modifier("SubTotal"))
                    grosstotal += CDec2(modifier("PremiumPrice"))
                Next
            Next

            Dim amountdue As Decimal = 0

            amountdue = grosstotal - CDec2(invoice.Rows(0).Item("VATExempt"))
            invoice.Rows(0).Item("NetAmount") = 0
            invoice.Rows(0).Item("DiscountAmount") = 0

            If CDec2(invoice.Rows(0).Item("SCDiscount")) > 0 Then
                'grosstotal = CDec2(invoice.Rows(0).Item("GrossSalesToSC"))
                amountdue = CDec2(invoice.Rows(0).Item("VATExempt")) - CDec2(invoice.Rows(0).Item("SCDiscount"))
                invoice.Rows(0).Item("NetAmount") = Format(amountdue, "#,##0.00")
                invoice.Rows(0).Item("DiscountAmount") = Format(CDec2(invoice.Rows(0).Item("SCDiscount")), "#,##0.00")
                amountdue = amountdue + CDec2(invoice.Rows(0).Item("GrossSalesToRegDiner")) + CDec2(invoice.Rows(0).Item("SCDiscountExempt"))
            End If

            If CDec2(invoice.Rows(0).Item("SoloParentDiscount")) > 0 Then
                amountdue = (amountdue - CDec2(invoice.Rows(0).Item("SoloParentDiscount")))
                invoice.Rows(0).Item("NetAmount") = Format(amountdue, "#,##0.00")
                invoice.Rows(0).Item("DiscountAmount") = Format(CDec2(invoice.Rows(0).Item("SoloParentDiscount")), "#,##0.00")
            End If

            If CDec2(invoice.Rows(0).Item("PhNatTeamDiscount")) > 0 Then
                amountdue = (amountdue - CDec2(invoice.Rows(0).Item("PhNatTeamDiscount"))) + CDec2(invoice.Rows(0).Item("VATExempt"))
                invoice.Rows(0).Item("NetAmount") = Format(amountdue, "#,##0.00")
                invoice.Rows(0).Item("DiscountAmount") = Format(CDec2(invoice.Rows(0).Item("PhNatTeamDiscount")), "#,##0.00")
            End If

            If CDec2(invoice.Rows(0).Item("OtherDiscount")) > 0 Then
                amountdue = amountdue - CDec2(invoice.Rows(0).Item("OtherDiscount"))
                invoice.Rows(0).Item("NetAmount") = Format(amountdue, "#,##0.00")
                invoice.Rows(0).Item("DiscountAmount") = Format(CDec2(invoice.Rows(0).Item("OtherDiscount")), "#,##0.00")
            End If

            Dim totalcharge As Decimal = 0
            For Each charge As Object In jss.Deserialize(Of Object)(DataTableToJSON.GetJson(charges))
                totalcharge += CDec2(charge("Amount"))
            Next
            amountdue += totalcharge
            invoice.Rows(0).Item("AmountDue") = Format(amountdue, "#,##0.00")
            invoice.Rows(0).Item("GrossTotal") = Format(grosstotal, "#,##0.00")

            Dim totalpayment As Decimal = 0
            For Each payment As Object In jss.Deserialize(Of Object)(DataTableToJSON.GetJson(payments))
                totalpayment += CDec2(payment("Amount"))
            Next
            invoice.Rows(0).Item("Change") = Format(totalpayment - amountdue, "#,##0.00")

            pOut.Add("Items", newitems)

            pSql = "SELECT DISTINCT PersonFullName as CustomerName, " & vbCrLf &
                    "   ISNULL(PersonSeniorCitizensIDNo, '') + ISNULL(PersonPWDIDNo, '') + ISNULL(PersonOtherIDNo, '') + ISNULL(PersonPassportNo, '') + ISNULL(PersonDriversLicenseNo, '') + ISNULL(PersonSSSNo, '') + ISNULL(PersonGSISIDNo, '') + ISNULL(PersonPRCIDNo, '') + ISNULL(PersonPostalIDNo, '') + ISNULL(PersonVotersIDNo, '') as CustomerID,  " & vbCrLf &
                    "   TinNumber " & vbCrLf &
                    "   FROM TransInvoiceSeniorCitizenPWDCustomer " & vbCrLf &
                    "   INNER JOIN MobizLibraryOrganization as org ON org.ID = CustomerID " & vbCrLf &
                    "   WHERE InvoiceID = " & ReadyString(InvoiceID) & vbCrLf &
                    " UNION ALL SELECT '' as CustomerName, '' as CustomerID, '' as TinNumber" & vbCrLf

            pOut.Add("Customers", Common.WebRequest.ProData.Execute(pSql)) 'for SC/PWD

            pSql = " SELECT ISNULL(NULLIF(LTRIM(RTRIM(PersonFirstName)) + ' ',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(PersonMiddleName)) + ' ',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(PersonSurname)) + ' ',' '), '') + CompanyName as CustomerName, " & vbCrLf &
                    " ISNULL(BusinessIndustryText, '') as BusinessStyle, ISNULL(TinNumber, '') as TinNumber, ISNULL(BusinessAddressBuildingNo + ', ' + BusinessAddressStreet + ', ' + MobizLibraryCities.Name, '') as BusinessAddress" & vbCrLf &
                    " FROM MobizLibraryOrganization " & vbCrLf &
                    " INNER JOIN TransInvoice ON TransInvoice.CustomerID = MobizLibraryOrganization.ID " & vbCrLf &
                    " LEFT JOIN MobizLibraryCities ON MobizLibraryCities.ID = MobizLibraryOrganization.BusinessAddressCityID " & vbCrLf &
                    " WHERE TransInvoice.ID = " & ReadyString(InvoiceID)
            Dim headerCustomer As DataRow = Common.WebRequest.ProData.ExecuteRow(pSql)
            invoice.Rows(0).Item("CustomerName") = headerCustomer("CustomerName")
            invoice.Rows(0).Item("CustomerBusAdd") = headerCustomer("BusinessAddress")
            invoice.Rows(0).Item("CustomerTin") = headerCustomer("TinNumber")
            invoice.Rows(0).Item("CustomerBusStyle") = headerCustomer("BusinessStyle")
        Else
            Dim items As Object
            items = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute("SELECT * FROM ##IL_TransInvoiceItems WHERE InvoiceID = " & ReadyString(invoice.Rows(0)("ID")))))
            For Each item As Object In items
                Dim modifiers As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute("SELECT * FROM ##IL_TransInvoiceItemsModifier WHERE InvoiceItemID = " & ReadyString(item("ID")))))

                item.Add("Modifiers", modifiers)
            Next

            pOut.Add("Items", items)
        End If

        pOut.Add("Header", invoice)

        pOut.Add("Payments", payments)

        pOut.Add("Charges", charges)

        Return pOut.Serialize()
    End Function

    Function ExportInvoice()
        Dim clsSalesReport As New ReportPOS
        Dim pOut As Object = JObject.Parse(GetInvoiceDetails(True))
        Dim pReport = New ProReport
        pReport.SourceFilename = "NewInvoice"
        pReport.AddTable("Items", JsonConvert.DeserializeObject(Of DataTable)(DirectCast(pOut("d")("Items")("Rows"), JArray).ToString()))
        pReport.AddTable("InvoiceHeader", JsonConvert.DeserializeObject(Of DataTable)(DirectCast(pOut("d")("Header")("Rows"), JArray).ToString()))
        pReport.AddTable("Payments", JsonConvert.DeserializeObject(Of DataTable)(DirectCast(pOut("d")("Payments")("Rows"), JArray).ToString()))
        pReport.AddTable("Charges", JsonConvert.DeserializeObject(Of DataTable)(DirectCast(pOut("d")("Charges")("Rows"), JArray).ToString()))
        pReport.AddTable("Customers", JsonConvert.DeserializeObject(Of DataTable)(DirectCast(pOut("d")("Customers")("Rows"), JArray).ToString()))
        pReport.AddTable("StoreHeader", clsSalesReport.getReportHeader(Common.WebRequest.ProData, FOWSTID, Nothing, DeviceID))
        pReport.ExportXmlFile()

        Dim fowstname = Common.WebRequest.ProData.ExecuteScalar("SELECT Name FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(FOWSTID))
        Dim siname = "SINo_" & pOut("d")("Header")("Rows")(0)("InvoiceNumber") & "_" & fowstname.Replace(",", "") & ".pdf"
        Dim path = Common.ServerPath & "temp\GeneratedInvoices\"
        CreateFolder(path)
        path &= CreateValidFilename(siname, True)
        pReport.ExportFile(ProReport.eExportFormat.Pdf, path)

        Return path
    End Function

    Function GetESalesReport() As DataTable
        Dim pSql As String

        If CBool2(Common.WebRequest.Data("GetFromRawData")) Then
            pSql = " IF OBJECT_ID('tempdb..##ESales_TransInvoice') IS NOT NULL DROP TABLE ##ESales_TransInvoice " & vbCrLf &
                    " SELECT * INTO ##ESales_TransInvoice FROM TransInvoice WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                    "" & vbCrLf &
                    " IF OBJECT_ID('tempdb..##ESales_TransInvoiceItems') IS NOT NULL DROP TABLE ##ESales_TransInvoiceItems " & vbCrLf &
                    " SELECT * INTO ##ESales_TransInvoiceItems FROM TransInvoiceItems WHERE InvoiceID IN (SELECT ID FROM ##ESales_TransInvoice) " & vbCrLf &
                    "" & vbCrLf &
                    " IF OBJECT_ID('tempdb..##ESales_TransCustomerPaymentInvoices') IS NOT NULL DROP TABLE ##ESales_TransCustomerPaymentInvoices " & vbCrLf &
                    " SELECT * INTO ##ESales_TransCustomerPaymentInvoices FROM TransCustomerPaymentInvoices WHERE InvoiceID IN (SELECT ID FROM ##ESales_TransInvoice) " & vbCrLf &
                    "" & vbCrLf &
                    " IF OBJECT_ID('tempdb..##ESales_TransCustomerPayment') IS NOT NULL DROP TABLE ##ESales_TransCustomerPayment " & vbCrLf &
                    " SELECT * INTO ##ESales_TransCustomerPayment FROM TransCustomerPayment WHERE ID IN (SELECT CustomerPaymentID FROM ##ESales_TransCustomerPaymentInvoices) " & vbCrLf &
                    "" & vbCrLf &
                    " IF OBJECT_ID('tempdb..##ESales_TransInvoiceDiscount') IS NOT NULL DROP TABLE ##ESales_TransInvoiceDiscount " & vbCrLf &
                    " SELECT * INTO ##ESales_TransInvoiceDiscount FROM TransInvoiceDiscount WHERE InvoiceID IN (SELECT ID FROM ##ESales_TransInvoice) " & vbCrLf &
                    "" & vbCrLf &
                    " SELECT FowstID, fowst.Code, fowst.Name, " & vbCrLf &
                    "   ISNULL(NULLIF(dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') + ', ' + ISNULL(fowst.Street, '') + ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, '')), ''), fowst.Name) as Address, " & vbCrLf &
                    "   SUM(ROUND(invitems.GrossSales, 2)) as GrossSales, " & vbCrLf &
                    "   SUM(ROUND(Vatable, 2)) as VatableSales, " & vbCrLf &
                    "   SUM(ROUND(ForegoneVAT, 2)) as VatExemptTax, " & vbCrLf &
                    "   (SELECT SUM(ISNULL(ROUND(memosr.TotalAmount, 2),0) - ISNULL(ROUND(memosr.TaxAmount, 2),0)) " & vbCrLf &
                    "       FROM TransMemoSalesReturn memosr " & vbCrLf &
                    "       INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " & vbCrLf &
                    "       INNER JOIN TransInvoice as sub_inv ON sub_inv.ID = memosr.InvoiceID " & vbCrLf &
                    "       AND CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                    "       AND memo.FOWSTID = inv.FowstID AND memo.DeviceID = inv.DeviceID " & vbCrLf &
                    "       AND ISNULL(sub_inv.SCDiscount, 0) = 0 AND ISNULL(sub_inv.SoloParentDiscount, 0) = 0 " & vbCrLf &
                    ") as CurTotalVATSalesSalesReturn, " & vbCrLf &
                    "   (SELECT SUM(ISNULL(ROUND(memosr.TaxAmount, 2),0)) " & vbCrLf &
                    "       FROM TransMemoSalesReturn memosr " & vbCrLf &
                    "       INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " & vbCrLf &
                    "       INNER JOIN TransInvoice as sub_inv ON sub_inv.ID = memosr.InvoiceID " & vbCrLf &
                    "       AND CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                    "       AND memo.FOWSTID = inv.FowstID AND memo.DeviceID = inv.DeviceID " & vbCrLf &
                    "       AND ISNULL(sub_inv.SCDiscount, 0) = 0 AND ISNULL(sub_inv.SoloParentDiscount, 0) = 0 " & vbCrLf &
                    ") as CurTotalVATAmountSalesReturn, " & vbCrLf &
                    "    (SELECT ((SUM(ROUND(invs.VAT, 2)) + SUM(ROUND(Vatable, 2))) / 1.12) * 0.12 " & vbCrLf &
                    "       FROM ##ESales_TransInvoice invs " & vbCrLf &
                    "       WHERE invs.InvoiceNumber > 0 AND invs.SCDiscount > 0 " & vbCrLf &
                    "       AND invs.DeviceID = inv.DeviceID AND invs.FowstID = inv.FowstID " & vbCrLf &
                    "    ) as SCDiscountExemptVatAmount, " & vbCrLf &
                    "    (SELECT ISNULL(SUM(ROUND(invs.SCDiscount, 2)), 0) as Total " & vbCrLf &
                    "       FROM TransSalesReturnLedger as ledger " & vbCrLf &
                    "       INNER JOIN TransMemo as memo ON memo.ID = ledger.SourceMemoID " & vbCrLf &
                    "       INNER JOIN ##ESales_TransCustomerPaymentInvoices as cuspayinv ON cuspayinv.InvoiceID = ledger.SourceInvoiceID " & vbCrLf &
                    "       INNER JOIN ##ESales_TransCustomerPayment custpay ON cuspayinv.CustomerPaymentID = custpay.ID " & vbCrLf &
                    "       INNER JOIN ##ESales_TransInvoice invs ON (cuspayinv.InvoiceID = invs.ID) " & vbCrLf &
                    "       WHERE ISNULL(RefundNum, 0) > 0 AND invs.DeviceID = inv.DeviceID AND invs.FowstID = inv.FowstID " & vbCrLf &
                    "   ) as DiscountRefund, " & vbCrLf &
                    "   ISNULL(SUM(ROUND(VATExempt, 2)), 0) - " & vbCrLf &
                    "   ISNULL((SELECT SUM(ROUND((items.UnitPrice * memosr.QuantityReturned) / 1.12, 2)) " & vbCrLf &
                    "       FROM TransMemoSalesReturn memosr " & vbCrLf &
                    "       INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) " & vbCrLf &
                    "       INNER JOIN TransInvoice as sub_inv ON sub_inv.ID = memosr.InvoiceID " & vbCrLf &
                    "       INNER JOIN TransInvoiceItems as items ON items.InvoiceID = sub_inv.ID AND items.ProductID = memosr.ProductID " & vbCrLf &
                    "       WHERE CONVERT(date, memo.LastUpdated) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                    "       AND memo.FOWSTID = inv.FowstID AND memo.DeviceID = inv.DeviceID AND (ISNULL(sub_inv.SCDiscount, 0) > 0 OR ISNULL(sub_inv.SoloParentDiscount, 0) > 0)) " & vbCrLf &
                    "   ,0)" & vbCrLf &
                    "   as VatExemptSales, " & vbCrLf &
                    "   SUM(ROUND(invitemvatamount.Amount, 2)) as VatAmount, " & vbCrLf &
                    "   SUM(ROUND(discount.DiscountAmount, 2)) as Discount, " & vbCrLf &
                    "   MIN(InvoiceNumber) as BeginningInvoiceNo, " & vbCrLf &
                    "   MAX(InvoiceNumber) as EndingInvoiceNo " & vbCrLf &
                    " FROM MobizLibraryOrganizationFOWST as fowst " & vbCrLf &
                    " INNER JOIN ##ESales_TransInvoice as inv ON inv.FowstID = FOWST.ID " & vbCrLf &
                    " LEFT OUTER JOIN dbo.MobizLibraryCities AS cty ON fowst.FowstCityID = cty.ID " & vbCrLf &
                    " LEFT OUTER JOIN dbo.MobizLibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID " & vbCrLf &
                    " OUTER APPLY ( " & vbCrLf &
                    "   SELECT SUM(ROUND(GrossSalesNetOfVATExemptions, 2)) as GrossSales, " & vbCrLf &
                    "   SUM(DiscountAmount) as DiscountAmount " & vbCrLf &
                    "   FROM ##ESales_TransInvoiceItems " & vbCrLf &
                    "   WHERE InvoiceID = inv.ID " & vbCrLf &
                    " ) as invitems " & vbCrLf &
                    " OUTER APPLY ( " & vbCrLf &
                    "   SELECT SUM(ISNULL(ROUND(TaxAmount, 2),0)- ISNULL(ROUND(DiscountTax, 2),0)) as Amount " & vbCrLf &
                    "   FROM ##ESales_TransInvoice as subinv " & vbCrLf &
                    "   INNER JOIN ##ESales_TransInvoiceItems as subinvitems ON subinvitems.InvoiceID = subinv.ID " & vbCrLf &
                    "   WHERE subinv.ID = inv.ID " & vbCrLf &
                    "   AND ISNULL(subinv.SCDiscount, 0) = 0 AND ISNULL(subinv.SoloParentDiscount, 0) = 0 " & vbCrLf &
                    "   ) as invitemvatamount " & vbCrLf &
                    " OUTER APPLY ( " & vbCrLf &
                    "   SELECT SUM(ROUND(custpay.DiscountAmount, 2)) as DiscountAmount " & vbCrLf &
                    "   FROM ##ESales_TransInvoice as subinv " & vbCrLf &
                    "   INNER JOIN ##ESales_TransInvoiceDiscount as invdisc ON invdisc.InvoiceID = subinv.ID " & vbCrLf &
                    "   INNER JOIN LibraryDiscountSettings as disc ON disc.ID = invdisc.DiscountTypeID " & vbCrLf &
                    "   INNER JOIN ##ESales_TransCustomerPaymentInvoices as payinv ON payinv.InvoiceID = subinv.ID " & vbCrLf &
                    "   INNER JOIN ##ESales_TransCustomerPayment custpay ON payinv.CustomerPaymentID = custpay.ID " & vbCrLf &
                    "   WHERE subinv.ID = inv.ID " & vbCrLf &
                    " ) as discount " & vbCrLf &
                    " WHERE ISNULL(fowst.IsActive, 0) = 1 " & vbCrLf &
                    " AND CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                    IIf(Not String.IsNullOrWhiteSpace(FOWSTID), " AND FOWSTID = " & ReadyString(FOWSTID), "") & vbCrLf &
                    IIf(Not String.IsNullOrWhiteSpace(DeviceID), " AND DeviceID = " & ReadyString(DeviceID), " AND inv.DeviceID IN (SELECT DeviceID FROM LibraryDevice as dev INNER JOIN LibraryDeviceFowst as devfowst ON devfowst.DeviceID = dev.ID WHERE devfowst.FowstID = fowst.ID AND dev.IsActive = 1) ") & vbCrLf &
                    " GROUP BY FowstID, fowst.Code, fowst.Name, fowst.StreetNumber, fowst.Street, cty.Name, ctry.Name, inv.DeviceID " & vbCrLf &
                    " ORDER BY fowst.Name " & vbCrLf
        Else
            pSql = " SELECT fowst.Code, fowst.Name " & vbCrLf &
                " , ISNULL(" & vbCrLf &
                "   NULLIF(" & vbCrLf &
                "   dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') + ', ' + ISNULL(" & vbCrLf &
                "   fowst.Street, '') + ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(" & vbCrLf &
                "   ctry.Name, '')) " & vbCrLf &
                "   , '')" & vbCrLf &
                "   , fowst.Name) as Address " & vbCrLf &
                " , (SELECT SUM (CurGrossSales) " & vbCrLf &
                "   FROM TransZReadCache " & vbCrLf &
                "   WHERE FOWSTID = zread.FOWSTID " & vbCrLf &
                "   AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " ) as GrossSales" & vbCrLf &
                " , ISNULL((SELECT SUM (CurTotalVATSales) " & vbCrLf &
                "   FROM TransZReadCache " & vbCrLf &
                "   WHERE FOWSTID = zread.FOWSTID " & vbCrLf &
                "   AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " ), 0) + ISNULL((SELECT SUM (CurTotalVatablePortionOfSCDTransaction) " & vbCrLf &
                "   FROM TransZReadCache " & vbCrLf &
                "   WHERE FOWSTID = zread.FOWSTID " & vbCrLf &
                "   AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " ), 0) - ISNULL((SELECT SUM (CurTotalVATSalesSalesReturn) " & vbCrLf &
                "   FROM TransZReadCache " & vbCrLf &
                "   WHERE FOWSTID = zread.FOWSTID " & vbCrLf &
                "   AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " ), 0) as VatableSales" & vbCrLf &
                " , (SELECT SUM (CurVATExemptSales) " & vbCrLf &
                "   FROM TransZReadCache " & vbCrLf &
                "   WHERE FOWSTID = zread.FOWSTID " & vbCrLf &
                "   AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " ) as VatExemptSales" & vbCrLf &
                " , ((SELECT SUM (CurTotalVAT) " & vbCrLf &
                "   FROM TransZReadCache " & vbCrLf &
                "   WHERE FOWSTID = zread.FOWSTID " & vbCrLf &
                "   AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " ) - (SELECT SUM (CurTotalVATAmountSalesReturn) " & vbCrLf &
                "   FROM TransZReadCache " & vbCrLf &
                "   WHERE FOWSTID = zread.FOWSTID " & vbCrLf &
                "   AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " )) + (SELECT SUM(CurSCDiscountExempt) - (SUM(CurSCDiscountExempt) / 1.12) " & vbCrLf &
                "   FROM TransZReadCache " & vbCrLf &
                "   WHERE FOWSTID = zread.FOWSTID " & vbCrLf &
                "   AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " ) as VatAmount" & vbCrLf &
                " , ISNULL((SELECT SUM (CurSCDiscount) " & vbCrLf &
                "   FROM TransZReadCache " & vbCrLf &
                "   WHERE FOWSTID = zread.FOWSTID " & vbCrLf &
                "   AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " ), 0) + ISNULL((SELECT SUM (CurTotalDiscount) " & vbCrLf &
                "   FROM TransZReadCache " & vbCrLf &
                "   WHERE FOWSTID = zread.FOWSTID " & vbCrLf &
                "   AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " ), 0) - ISNULL((SELECT SUM (DiscountRefund) " & vbCrLf &
                "   FROM TransZReadCache " & vbCrLf &
                "   WHERE FOWSTID = zread.FOWSTID " & vbCrLf &
                "   AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " ), 0) As Discount " & vbCrLf &
                " , (SELECT TOP 1 CASE WHEN CurBegInvoiceNo = 0 THEN 1 ELSE CurBegInvoiceNo END as CurBegInvoiceNo " & vbCrLf &
                "   FROM TransZReadCache " & vbCrLf &
                "   WHERE FOWSTID = zread.FOWSTID " & vbCrLf &
                "   AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                "   ORDER BY CurBegInvoiceNo ASC " & vbCrLf &
                " ) as BeginningInvoiceNo" & vbCrLf &
                " , (SELECT TOP 1 CurEndInvoiceNo " & vbCrLf &
                "   FROM TransZReadCache " & vbCrLf &
                "   WHERE FOWSTID = zread.FOWSTID " & vbCrLf &
                "   AND CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                "   ORDER BY CurEndInvoiceNo DESC " & vbCrLf &
                " ) as EndingInvoiceNo" & vbCrLf &
                " FROM TransZReadCache as zread " & vbCrLf &
                " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = zread.FOWSTID AND fowst.IsActive = 1 " & vbCrLf &
                " LEFT OUTER JOIN dbo.MobizLibraryCities AS cty ON fowst.FowstCityID = cty.ID " & vbCrLf &
                " LEFT OUTER JOIN dbo.MobizLibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID" & vbCrLf &
                " WHERE CONVERT(date, SalesDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " GROUP BY fowst.Code, fowst.Name, zread.FOWSTID, fowst.StreetNumber, fowst.Street, cty.Name, ctry.Name " & vbCrLf &
                " ORDER BY fowst.Name " & vbCrLf
        End If

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function
    Function GetSalesReportFranchisee() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim ownership As String = Common.WebRequest.Data("Ownership")
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = IIf(Not String.IsNullOrWhiteSpace(FOWSTID), True, False)
            With .Fields
                If Not String.IsNullOrWhiteSpace(FOWSTID) Then
                    With .Add("InvoiceDate", SqlDbType.Date)
                        .Expression = "CONVERT(date, CacheInvoices.InvoiceDate)"
                    End With
                End If
                With .Add("FOWSTCode", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(CacheInvoices.FOWSTCode,'-')"
                End With
                With .Add("FOWSTName", SqlDbType.UniqueIdentifier)
                    .Expression = "CacheInvoices.FOWSTName"
                End With
                With .Add("StoreType", SqlDbType.VarChar)
                    .Expression = "ISNULL(CacheInvoices.StoreType,'')"
                End With
                With .Add("StoreLocation", SqlDbType.VarChar)
                    .Expression = "ISNULL(CacheInvoices.StoreLocation,'')"
                End With
                With .Add("GroupName", SqlDbType.Int)
                    .Expression = "ISNULL(CacheInvoices.GroupName,'')"
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "CacheInvoices.FOWSTID"
                End With
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("SalesReturnAndRefund", SqlDbType.Bit)
                    .Expression = "ISNULL(" &
                        "(Select SUM(Amount) FROM TransSalesReturnLedger " & vbCrLf &
                        "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID AND TransSalesReturnLedger.FowstID = CacheInvoices.FOWSTID " & vbCrLf &
                        "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL " & vbCrLf &
                        "AND CONVERT(date,TransMemo.LastUpdated) " & If(Not String.IsNullOrWhiteSpace(FOWSTID), " = convert(date, CacheInvoices.InvoiceDate) ", " BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)) & ")" & vbCrLf &
                         ",0)"
                End With
                With .Add("SalesReturnAndRefundCount", SqlDbType.Bit)
                    .Expression = "ISNULL(" &
                        "(Select COUNT(distinct SourceMemoID) FROM TransSalesReturnLedger " & vbCrLf &
                        "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID AND TransSalesReturnLedger.FowstID = CacheInvoices.FOWSTID " & vbCrLf &
                        "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL " & vbCrLf &
                        "AND CONVERT(date,TransMemo.LastUpdated) " & If(Not String.IsNullOrWhiteSpace(FOWSTID), " = convert(date, CacheInvoices.InvoiceDate) ", " BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)) & ")" & vbCrLf &
                         ",0)"
                End With
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "ISNULL(COUNT(DISTINCT CacheInvoices.ID),0)"
                End With
                With .Add("DiscountAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.SCDiscount),0) + ISNULL(SUM(CacheInvoices.ForegoneVAT),0)"
                End With
                With .Add("CashAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.CashNetOfChange),0)"
                End With
                With .Add("CreditCardAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.CreditCardAmount),0)"
                End With
                With .Add("DebitCardAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.DebitCardAmount),0)"
                End With
                With .Add("CheckPaymentAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.PotatoCornerGC),0)"
                End With
                'With .Add("OtherPayments", SqlDbType.Bit)
                '    .Expression = "SUM(TransCustomerPayment.CheckPaymentAmount)"
                'End With
                With .Add("DiscountVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Discount Voucher / On Line Deals]),0)"
                End With
                With .Add("PAS", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.PurchaseAcknowledgementSlip),0)"
                End With
                With .Add("E-Plus", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[E-Plus]),0)"
                End With
                With .Add("FoodPanda", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[FoodPanda]),0)"
                End With
                With .Add("GCash", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[GCash]),0)"
                End With
                With .Add("PayMaya", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[PayMaya]),0)"
                End With
                With .Add("GrabFood", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[GrabFood]),0)"
                End With
                With .Add("SMAC", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[SMAC]),0)"
                End With
                With .Add("V-Plus", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[V-Plus]),0)"
                End With

                With .Add("GoldCard", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[GoldCard]),0)"
                End With

                With .Add("ShopeeEVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[ShopeeEVoucher]),0)"
                End With

                With .Add("LocalDelivery", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[LocalDelivery]),0)"
                End With

                With .Add("ShopeePay", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[ShopeePay]),0)"
                End With

                With .Add("Eatigo", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Eatigo]),0)"
                End With

                With .Add("DeliRush", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[DeliRush]),0)"
                End With

                With .Add("PoodTrip", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[PoodTrip]),0)"
                End With

                With .Add("ShareTreats", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Share Treats]),0)"
                End With

                With .Add("Booky", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Booky]),0)"
                End With

                With .Add("Lazada", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Lazada]),0)"
                End With

                With .Add("Pickaroo", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Pickaroo]),0)"
                End With

                With .Add("LatestTransactionDate", SqlDbType.Bit)
                    .Expression = "MAX(CacheInvoices.InvoiceDate)"
                End With

                With .Add("SupercardVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.SupercardVoucher),0)"
                End With

                With .Add("CashierNames", SqlDbType.VarChar)
                    Dim cashiernames As String = "STUFF((SELECT '/ ' + UpdatedByName " & vbCrLf &
                                    " FROM CacheInvoices as ci " & vbCrLf &
                                    " WHERE convert(date, ci.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

                    cashiernames += " AND ci.FowstID = CacheInvoices.FowstID " & vbCrLf
                    cashiernames += "GROUP BY UpdatedByName" & vbCrLf
                    cashiernames += "FOR XML PATH('')" & vbCrLf
                    cashiernames += "), 1, 1,'')"

                    .Expression = cashiernames
                End With
            End With


            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            '.SqlFrom = "FROM CacheInvoices " & vbCrLf

            '"INNER JOIN MobizLibraryOrganizationFOWST  " & vbCrLf &
            '"ON CacheInvoices.FowstID = MobizLibraryOrganizationFOWST.ID     " & vbCrLf &
            '"LEFT JOIN MobizLibraryFOWSTRetailStoreType on MobizLibraryFOWSTRetailStoreType.ID = MobizLibraryOrganizationFOWST.SubFOWSTTypeID " & vbCrLf &
            '"LEFT JOIN MobizLibraryFOWSTStoreLocationType on MobizLibraryFOWSTStoreLocationType.ID = MobizLibraryOrganizationFOWST.StoreLocationTypeID " & vbCrLf &
            '"INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
            '"INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            If ownership <> "All" Then
                .SqlWhere += " AND ISNULL(FromFranchisee, 0) = " & ownership
            End If

            .SqlGroup = "GROUP BY " & vbCrLf &
                            "CacheInvoices.FOWSTName " & vbCrLf &
                            ", CacheInvoices.StoreType  " & vbCrLf &
                            ", CacheInvoices.StoreLocation " & vbCrLf &
                            ",CacheInvoices.GroupName " & vbCrLf &
                            ",CacheInvoices.FowstID " & vbCrLf &
                            ",CacheInvoices.FOWSTCode "

            If Not FOWSTID = vbNullString Then
                .SqlWhere += " AND CacheInvoices.FowstID = " & ReadyString(FOWSTID) & vbCrLf
                .SqlGroup += ",CONVERT(date, CacheInvoices.InvoiceDate) "
                .SqlOrder = " ORDER BY CONVERT(date, CacheInvoices.InvoiceDate) "
            Else
                .SqlWhere += " AND CacheInvoices.FowstID IN (select distinct FowstID from LibraryOrganizationEmployeeFOWST WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleID) & ") " & vbCrLf

                If CStr2(Common.WebRequest.Data("StoreType")) <> "" Then
                    .SqlWhere += " AND CacheInvoices.SubFOWSTTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreType"))) & vbCrLf
                End If

                If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
                    .SqlWhere += " AND CacheInvoices.StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
                End If

                If Not GroupID = vbNullString Then
                    .SqlWhere &= "AND CacheInvoices.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
                End If
            End If

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetSalesReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim ownership As String = Common.WebRequest.Data("Ownership")
        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = IIf(Not String.IsNullOrWhiteSpace(FOWSTID), True, False)
            With .Fields
                If Not String.IsNullOrWhiteSpace(FOWSTID) Then
                    With .Add("InvoiceDate", SqlDbType.Date)
                        .Expression = "CONVERT(date, CacheInvoicesDaily.InvoiceDate)"
                    End With
                End If
                With .Add("FOWSTCode", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(CacheInvoicesDaily.FOWSTCode,'-')"
                End With
                With .Add("FOWSTName", SqlDbType.UniqueIdentifier)
                    .Expression = "CacheInvoicesDaily.FOWSTName"
                End With
                With .Add("StoreType", SqlDbType.VarChar)
                    .Expression = "ISNULL(CacheInvoicesDaily.StoreType,'')"
                End With
                With .Add("StoreLocation", SqlDbType.VarChar)
                    .Expression = "ISNULL(CacheInvoicesDaily.StoreLocation,'')"
                End With
                With .Add("GroupName", SqlDbType.Int)
                    .Expression = "ISNULL(CacheInvoicesDaily.GroupName,'')"
                End With
                With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                    .Expression = "CacheInvoicesDaily.FOWSTID"
                End With
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.GrossSalesNetOfVATExemptions),0)"
                End With
                'With .Add("SalesReturnAndRefund", SqlDbType.Bit)
                '    .Expression = "ISNULL(" &
                '        "(Select SUM(Amount) FROM TransSalesReturnLedger " & vbCrLf &
                '        "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID AND TransSalesReturnLedger.FowstID = CacheInvoicesDaily.FOWSTID " & vbCrLf &
                '        "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL " & vbCrLf &
                '        "AND CONVERT(date,TransMemo.LastUpdated) " & If(Not String.IsNullOrWhiteSpace(FOWSTID), " = convert(date, CacheInvoicesDaily.InvoiceDate) ", " BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)) & ")" & vbCrLf &
                '         ",0)"
                'End With

                With .Add("SalesReturnAndRefund", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CreditMemo),0)"
                End With

                With .Add("SalesReturnAndRefundCount", SqlDbType.Bit)
                    .Expression = "ISNULL(" &
                        "(Select COUNT(distinct SourceMemoID) FROM TransSalesReturnLedger " & vbCrLf &
                        "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID AND TransSalesReturnLedger.FowstID = CacheInvoicesDaily.FOWSTID " & vbCrLf &
                        "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL " & vbCrLf &
                        "AND CONVERT(date,TransMemo.LastUpdated) " & If(Not String.IsNullOrWhiteSpace(FOWSTID), " = convert(date, CacheInvoicesDaily.InvoiceDate) ", " BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)) & ")" & vbCrLf &
                         ",0)"
                End With
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(TransactionCount),0)"
                End With
                With .Add("DiscountAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.SCDiscount),0) + ISNULL(SUM(CacheInvoicesDaily.ForegoneVAT),0)"
                End With
                With .Add("CashAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.CashNetOfChange),0)"
                End With
                With .Add("CreditCardAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.CreditCardAmount),0)"
                End With
                With .Add("DebitCardAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.DebitCardAmount),0)"
                End With
                With .Add("CheckPaymentAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.PotatoCornerGC),0)"
                End With
                'With .Add("OtherPayments", SqlDbType.Bit)
                '    .Expression = "SUM(TransCustomerPayment.CheckPaymentAmount)"
                'End With
                With .Add("DiscountVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[Discount Voucher / On Line Deals]),0)"
                End With
                With .Add("PAS", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.PurchaseAcknowledgementSlip),0)"
                End With
                With .Add("E-Plus", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[E-Plus]),0)"
                End With
                With .Add("FoodPanda", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[FoodPanda]),0)"
                End With
                With .Add("GCash", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[GCash]),0)"
                End With
                With .Add("PayMaya", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[PayMaya]),0)"
                End With
                With .Add("GrabFood", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[GrabFood]),0)"
                End With
                With .Add("SMAC", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[SMAC]),0)"
                End With
                With .Add("V-Plus", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[V-Plus]),0)"
                End With

                With .Add("GoldCard", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[GoldCard]),0)"
                End With

                With .Add("ShopeeEVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[ShopeeEVoucher]),0)"
                End With

                With .Add("LocalDelivery", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[LocalDelivery]),0)"
                End With

                With .Add("ShopeePay", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[ShopeePay]),0)"
                End With

                With .Add("Eatigo", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[Eatigo]),0)"
                End With

                With .Add("DeliRush", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[DeliRush]),0)"
                End With

                With .Add("PoodTrip", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[PoodTrip]),0)"
                End With

                With .Add("ShareTreats", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[Share Treats]),0)"
                End With

                With .Add("Booky", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[Booky]),0)"
                End With

                With .Add("Lazada", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[Lazada]),0)"
                End With

                With .Add("Pickaroo", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[Pickaroo]),0)"
                End With

                With .Add("SupercardVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.SupercardVoucher),0)"
                End With

                With .Add("LatestTransactionDate", SqlDbType.Bit)
                    .Expression = "MAX(CacheInvoicesDaily.InvoiceDate)"
                End With

                With .Add("CashierNames", SqlDbType.VarChar)
                    Dim cashiernames As String = "STUFF((SELECT '/ ' + UpdatedByName " & vbCrLf &
                                    " FROM CacheInvoicesDaily as ci " & vbCrLf &
                                    " WHERE convert(date, ci.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

                    cashiernames += " AND ci.FowstID = CacheInvoicesDaily.FowstID " & vbCrLf
                    cashiernames += "GROUP BY UpdatedByName" & vbCrLf
                    cashiernames += "FOR XML PATH('')" & vbCrLf
                    cashiernames += "), 1, 1,'')"

                    .Expression = cashiernames
                End With
            End With


            .SqlFrom = "FROM CacheInvoicesDaily " & vbCrLf

            '.SqlFrom = "FROM CacheInvoicesDaily " & vbCrLf

            '"INNER JOIN MobizLibraryOrganizationFOWST  " & vbCrLf &
            '"ON CacheInvoicesDaily.FowstID = MobizLibraryOrganizationFOWST.ID     " & vbCrLf &
            '"LEFT JOIN MobizLibraryFOWSTRetailStoreType on MobizLibraryFOWSTRetailStoreType.ID = MobizLibraryOrganizationFOWST.SubFOWSTTypeID " & vbCrLf &
            '"LEFT JOIN MobizLibraryFOWSTStoreLocationType on MobizLibraryFOWSTStoreLocationType.ID = MobizLibraryOrganizationFOWST.StoreLocationTypeID " & vbCrLf &
            '"INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
            '"INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoicesDaily.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            If ownership <> "All" Then
                .SqlWhere += " AND ISNULL(FromFranchisee, 0) = " & ownership
            End If

            .SqlGroup = "GROUP BY " & vbCrLf &
                            "CacheInvoicesDaily.FOWSTName " & vbCrLf &
                            ", CacheInvoicesDaily.StoreType  " & vbCrLf &
                            ", CacheInvoicesDaily.StoreLocation " & vbCrLf &
                            ",CacheInvoicesDaily.GroupName " & vbCrLf &
                            ",CacheInvoicesDaily.FowstID " & vbCrLf &
                            ",CacheInvoicesDaily.FOWSTCode "

            If Not FOWSTID = vbNullString Then
                .SqlWhere += " AND CacheInvoicesDaily.FowstID = " & ReadyString(FOWSTID) & vbCrLf
                .SqlGroup += ",CONVERT(date, CacheInvoicesDaily.InvoiceDate) "
                .SqlOrder = " ORDER BY CONVERT(date, CacheInvoicesDaily.InvoiceDate) "
            Else
                .SqlWhere += " AND CacheInvoicesDaily.FowstID IN (select distinct FowstID from LibraryOrganizationEmployeeFOWST WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleID) & ") " & vbCrLf

                If CStr2(Common.WebRequest.Data("StoreType")) <> "" Then
                    .SqlWhere += " AND CacheInvoicesDaily.SubFOWSTTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreType"))) & vbCrLf
                End If

                If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
                    .SqlWhere += " AND CacheInvoicesDaily.StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
                End If

                If Not GroupID = vbNullString Then
                    .SqlWhere &= "AND CacheInvoicesDaily.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
                End If
            End If

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetHourlySalesReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("HourOfDay", SqlDbType.Bit)
                    .Expression = "CONVERT(varchar, DATEPART(hh, InvoiceDate)) + ':00'"
                End With
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "COUNT(DISTINCT TransInvoiceItems.InvoiceID)"
                End With

                With .Add("ItemCount", SqlDbType.Bit)
                    .Expression = "COUNT(TransInvoiceItems.ID)"
                End With
                With .Add("GrossSales", SqlDbType.Int)
                    .Expression = "(CASE WHEN SUM(ISNULL(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions, 0)) = 0 THEN SUM(dbo.TransInvoiceItems.SubTotal) ELSE SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions) END)"
                End With
                With .Add("Discount", SqlDbType.Bit)
                    .Expression = "SUM(TransInvoice.SCDiscount) + SUM(TransInvoice.ForegoneVAT) "
                    '.Expression = "SUM(TransInvoiceitems.DiscountAmount) + + SUM(TransInvoice.SCDiscount) + SUM(TransInvoice.ForegoneVAT)"
                End With
                With .Add("SalesReturn", SqlDbType.Bit)
                    .Expression = "ISNULL(( " & vbCrLf &
                                "Select SUM(Amount) FROM TransSalesReturnLedger inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID  " & vbCrLf &
                                "and CONVERT(varchar, DATEPART(hh, InvoiceDate)) = CONVERT(varchar, DATEPART(hh, TransSalesReturnLedger.LastUpdated)) " & vbCrLf &
                                "INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = TransSalesReturnLedger.FowstID " & vbCrLf &
                                "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL AND CONVERT(date,TransMemo.LastUpdated) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

                    If Not FOWSTID = vbNullString Then
                        .Expression &= " AND TransMemo.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                                "),0)  "
                    End If

                    If Not GroupID = vbNullString Then
                        .Expression &= " AND MobizLibraryOrganizationFranchisee.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf &
                                "),0)  "
                    End If

                End With

                With .Add("HourOfDayInInteger", SqlDbType.Bit)
                    .Expression = "DATEPART(hh, InvoiceDate)"
                End With
            End With

            .SqlFrom = " FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
                        "TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = TransInvoice.FOWSTID " & vbCrLf &
                        "INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf &
                         "INNER JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                        "--INNER JOIN LibraryOrganizationFOWSTQuota on LibraryOrganizationFOWSTQuota.FOWSTID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                        "LEFT JOIN MobizLibraryFOWSTStoreLocationType on MobizLibraryFOWSTStoreLocationType.ID = MobizLibraryOrganizationFOWST.StoreLocationTypeID " & vbCrLf

            .SqlWhere = " WHERE (dbo.TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0) " & vbCrLf &
                        "AND convert(date, TransInvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            If Not FOWSTID = vbNullString Then
                .SqlWhere &= " AND TransInvoice.FowstID = " & ReadyString(FOWSTID)
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= "AND MobizLibraryOrganizationFranchisee.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf


                If CStr2(Common.WebRequest.Data("StoreType")) <> "" Then
                    .SqlWhere += " AND MobizLibraryOrganizationFOWST.SubFOWSTTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreType"))) & vbCrLf
                End If

                If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
                    .SqlWhere += " AND MobizLibraryOrganizationFOWST.StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
                End If

            End If

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "CONVERT(varchar, DATEPART(hh, InvoiceDate)) " & vbCrLf &
                        ", DATEPART(hh, InvoiceDate) "

            .SqlOrder = " Order by DATEPART(hh, InvoiceDate) "



            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetInventoryMovement() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("Category", SqlDbType.Bit)
                    .Expression = "ISNULL(CacheInventoryMovement.Category,'')"
                    '.Expression = "MobizLibraryPIMProductVariableFeatures001.C0020"
                End With
                With .Add("ProductName", SqlDbType.Bit)
                    '.Expression = "ISNULL(CacheInventoryMovement.ProductName,'')"
                    .Expression = "MobizLibraryPimProduct.ProductName"
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryPimProduct.ID"
                End With
                With .Add("UnitMeasure", SqlDbType.Bit)
                    .Expression = "lbl.Label"
                End With
                With .Add("Balance", SqlDbType.Bit)
                    .Expression = "ISNULL(tblBeginningBalance.BeginningBalance, 0)"
                End With
                With .Add("Delivery", SqlDbType.Int)
                    .Expression = "SUM(ISNULL(CacheInventoryMovement.Delivery, 0))"
                End With
                With .Add("TransIn", SqlDbType.Bit)
                    .Expression = "SUM(ISNULL(CacheInventoryMovement.TransIn, 0))"
                End With
                With .Add("TransOut", SqlDbType.Bit)
                    .Expression = "SUM(ISNULL(CacheInventoryMovement.TransOut, 0))"
                End With
                With .Add("Wastage", SqlDbType.Bit)
                    .Expression = "0"
                    '.Expression = "SUM(ISNULL(CacheInventoryMovement.TransOut, 0))"
                End With
                With .Add("Ending", SqlDbType.Bit)
                    .Expression = "ISNULL(tblEndingBalance.EndingBalance, 0)"
                End With
                With .Add("Usage", SqlDbType.Bit)
                    .Expression = "SUM(ISNULL(CacheInventoryMovement.Usage, 0))"
                End With
                With .Add("Variance", SqlDbType.Bit)
                    .Expression = "ISNULL(tblBeginningBalance.BeginningBalance, 0) + SUM(ISNULL(CacheInventoryMovement.Delivery, 0)) +  SUM(ISNULL(CacheInventoryMovement.TransIn, 0)) " & vbCrLf &
                                  "- SUM(ISNULL(CacheInventoryMovement.TransOut, 0)) - ISNULL(tblEndingBalance.EndingBalance, 0) " & vbCrLf &
                                  "+ SUM(ISNULL(CacheInventoryMovement.Usage, 0)) "
                End With
                With .Add("InventoryCount", SqlDbType.Bit)
                    .Expression = "ISNULL(tblInventoryCount.Quantity, 0)"
                End With
                With .Add("Cost", SqlDbType.Bit)
                    .Expression = "ISNULL(pPrice.Price, 0)"
                End With
                With .Add("Shortlanded", SqlDbType.Bit)
                    .Expression = "SUM(ISNULL(CacheInventoryMovement.Shortlanded, 0))"
                    '.Expression = "SUM(tblShortlanded.QuantityGood)"
                End With
            End With

            '"JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
            .SqlFrom = " FROM MobizLibraryPimProduct " & vbCrLf &
                        " LEFT JOIN CacheInventoryMovement ON CacheInventoryMovement.ProductID = MobizLibraryPimProduct.ID" & vbCrLf &
                        " AND CacheInventoryMovement.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                        " AND CacheInventoryMovement.TransactionDate BETWEEN " & ProSqlite.ReadyDate(DateFrom) & " AND " & ProSqlite.ReadyDate(DateTo) & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductMeasurement as pimmsr ON pimmsr.ProductID = MobizLibraryPimProduct.ID AND MeasurementLevel = 1 " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMMeasure as msr ON msr.ID = pimmsr.UnitMeasureID  " & vbCrLf &
                        " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " & vbCrLf &
                        " INNER JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPimProduct.ID " & vbCrLf

            .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                        "SELECT TOP 1 t2.BeginningBalance " & vbCrLf &
                        "FROM CacheInventoryMovement t2 " & vbCrLf &
                        "WHERE t2.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                        "AND t2.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                        "AND t2.TransactionDate >= " & ReadyString(DateFrom) & vbCrLf &
                        "ORDER BY t2.TransactionDate ASC " & vbCrLf &
                        ") tblBeginningBalance " & vbCrLf


            .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                        "SELECT DISTINCT TOP 1 Quantity, TransInventoryProductCount.InventoryCountDate " & vbCrLf &
                        "FROM TransInventoryProductCountStock INNER JOIN TransInventoryProductCount ON TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID " & vbCrLf &
                        "WHERE TransInventoryProductCount.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                        "AND TransInventoryProductCount.ProductID = MobizLibraryPimProduct.ID  " & vbCrLf &
                        "AND CONVERT(date, TransInventoryProductCount.InventoryCountDate) = " & ReadyString(DateTo) & vbCrLf &
                        "ORDER BY TransInventoryProductCount.InventoryCountDate DESC " & vbCrLf &
                        ") tblInventoryCount "

            '.SqlFrom &= "OUTER APPLY ( " & vbCrLf &
            '            "SELECT SUM(TransReceivingStock.QuantityGood * TransReceivingStock.ActualQuantityGood) AS QuantityGood " & vbCrLf &
            '            "FROM TransReceiving " & vbCrLf &
            '            "JOIN TransReceivingStock ON TransReceivingStock.ReceivingID = TransReceiving.ID " & vbCrLf &
            '            "WHERE TransReceiving.IsReceiving = 1 " & vbCrLf &
            '            "AND TransReceivingStock.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
            '            "AND CONVERT(date, TransReceiving.LastUpdated) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
            '            "AND TransReceiving.ReceiveInOutlet = " & ReadyString(FOWSTID) & vbCrLf &
            '            ") tblTransIn "

            '.SqlFrom &= "OUTER APPLY ( " & vbCrLf &
            '            "SELECT SUM(TransReceivingStock.QuantityGood * TransReceivingStock.ActualQuantityGood) AS QuantityGood " & vbCrLf &
            '            "FROM TransReceiving " & vbCrLf &
            '            "JOIN TransReceivingStock ON TransReceivingStock.ReceivingID = TransReceiving.ID " & vbCrLf &
            '            "WHERE ISNULL(TransReceiving.IsReceiving, 0) = 0 " & vbCrLf &
            '            "AND TransReceivingStock.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
            '            "AND CONVERT(date, TransReceiving.LastUpdated) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
            '            "AND TransReceiving.ReceiveFromOutlet = " & ReadyString(FOWSTID) & vbCrLf &
            '            ") tblTransOut " & vbCrLf

            .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                        "SELECT TOP 1 t1.EndingBalance " & vbCrLf &
                        "FROM CacheInventoryMovement t1 " & vbCrLf &
                        "WHERE t1.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                        "AND t1.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                        "AND CONVERT(date, t1.TransactionDate) <= " & ReadyString(DateTo) & vbCrLf &
                        "ORDER BY t1.TransactionDate DESC " & vbCrLf &
                        ") tblEndingBalance " & vbCrLf

            '.SqlFrom &= "OUTER APPLY ( " & vbCrLf &
            '            " SELECT SUM(TransInventoryLedger.Qty) AS Qty" & vbCrLf &
            '            " FROM TransInventoryLedger " & vbCrLf &
            '            " WHERE TransInventoryLedger.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
            '            " AND TransInventoryLedger.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
            '            " AND CONVERT(date, TransInventoryLedger.TransactionDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
            '            " AND TransInventoryLedger.TransactionType = 'Invoice'" & vbCrLf &
            '            ") tblUsage " & vbCrLf

            '.SqlWhere = " WHERE ISNULL(tblBeginningBalance.Balance, 0) <> ISNULL(tblEndingBalance.Balance,0) " & vbCrLf &
            '    " OR ISNULL(tblDelivery.QuantityGood,0) <> 0" & vbCrLf &
            '    " OR ISNULL(tblTransIn.QuantityGood,0) <> 0" & vbCrLf &
            '    " OR ISNULL(tblTransOut.QuantityGood,0) <> 0" & vbCrLf &
            '    " OR ISNULL(tblUsage.Qty,0) <> 0" & vbCrLf

            '.SqlFrom &= " OUTER APPLY ( " & vbCrLf &
            '             "SELECT TOP 1 trs.QuantityGood " & vbCrLf &
            '            "FROM TransReceiving tr " & vbCrLf &
            '            "JOIN TransReceivingStock trs ON trs.ReceivingID = tr.ID " & vbCrLf &
            '            "WHERE tr.ReceivingPlaceID = " & ReadyString(FOWSTID) & vbCrLf &
            '            " AND trs.ProductID = MobizLibraryPimProduct.ID  " & vbCrLf &
            '            " AND CONVERT(date, tr.DocumentDate) = " & ReadyString(DateTo) & vbCrLf &
            '            " AND ISNULL(IsShortLanded,0) = 1 " & vbCrLf &
            '            "ORDER BY tr.DocumentDate DESC  " & vbCrLf &
            '            ") tblShortlanded " & vbCrLf

            .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                        "Select top 1 Price from MobizLibraryPIMProductPriceLevel  " & vbCrLf &
                        "where MobizLibraryPIMProductPriceLevel.ProductID = CacheInventoryMovement.ProductID AND MobizLibraryPIMProductPriceLevel .PriceLevelID = '01000012-0003-0000-0000-000000000000' " & vbCrLf &
                        "AND Organizationid=" & ReadyString(Common.WebRequest.Data("OrgID")) & ") pPrice " & vbCrLf 'landed cost 

            .SqlWhere = "where ISNULL(LibraryOrganizationPIM.IsInventoryCountItem,0) = 1 AND LibraryOrganizationPIM.InventoryRankingNo IS NOT NULL " & vbCrLf
            .SqlGroup = " GROUP BY ISNULL(LibraryOrganizationPIM.InventoryRankingNo,9999), MobizLibraryPimProduct.ID, CacheInventoryMovement.Category, MobizLibraryPimProduct.ProductName, tblBeginningBalance.BeginningBalance, tblEndingBalance.EndingBalance, lbl.Label, tblInventoryCount.Quantity, pPrice.Price " & vbCrLf
            .SqlOrder = " ORDER BY ISNULL(LibraryOrganizationPIM.InventoryRankingNo,9999) --Category,ProductName" & vbCrLf

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function InventoryReportCrewVSAMDate() As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT CONVERT(date,InventoryCountDate) InventoryCountDate from TransInventoryProductCount " & vbCrLf &
                " WHERE FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                " AND CONVERT(date,InventoryCountDate) BETWEEN " & ProSqlite.ReadyDate(DateFrom) & " AND " & ProSqlite.ReadyDate(DateTo) & vbCrLf &
                " GROUP BY CONVERT(date,InventoryCountDate) " & vbCrLf &
                " HAVING COUNT(DISTINCT UserID)>1 "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function InventoryReportCrewVSAMPerGroupDate() As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        'pSql = "SELECT DISTINCT CONVERT(date,InventoryCountDate) InventoryCountDate from TransInventoryProductCount " & vbCrLf &
        '        " INNER JOIN CacheInventoryMovement ON CacheInventoryMovement.FOWSTID = TransInventoryProductCount.FOWSTID " & vbCrLf &
        '        " WHERE CacheInventoryMovement.FranchisegroupID = " & ReadyString(GroupID) & vbCrLf &
        '        " AND CONVERT(date,InventoryCountDate) BETWEEN " & ProSqlite.ReadyDate(DateFrom) & " AND " & ProSqlite.ReadyDate(DateTo) & vbCrLf &
        '        " GROUP BY CacheInventoryMovement.FOWSTID, CONVERT(date,InventoryCountDate) " & vbCrLf &
        '        " --HAVING COUNT(DISTINCT UserID)>1 " & vbCrLf &
        '        " ORDER BY InventoryCountDate "

        pSql = "SELECT DISTINCT  CONVERT(date,InventoryCountDate) InventoryCountDate from TransInventoryProductCount " & vbCrLf &
                "inner join MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = TransInventoryProductCount.FowstID " & vbCrLf &
                 "WHERE MobizLibraryOrganizationFranchisee.FranchisegroupID = " & ReadyString(GroupID) & vbCrLf &
                "AND CONVERT(date,InventoryCountDate) BETWEEN " & ProSqlite.ReadyDate(DateFrom) & " AND " & ProSqlite.ReadyDate(DateTo) & vbCrLf &
                 "GROUP BY TransInventoryProductCount.FOWSTID, CONVERT(date,InventoryCountDate) " & vbCrLf &
                 "HAVING COUNT(DISTINCT TransInventoryProductCount.UserID)>1  " & vbCrLf &
                 "ORDER BY InventoryCountDate "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function InventoryReportCrewVSAMPerGroup() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pFilter1 As New DataTable
        Dim dtDates As New DataTable
        Dim dtAMVsCrewCount As New DataTable
        Dim InventoryCountDate As String

        dtDates = InventoryReportCrewVSAMPerGroupDate()

        For datesctr As Integer = 0 To dtDates.Rows.Count - 1

            InventoryCountDate = CStr2(dtDates.Rows(datesctr).Item("InventoryCountDate"))

            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("Category", SqlDbType.Bit)
                        .Expression = "ISNULL(Category,'-')"
                    End With
                    With .Add("ProductName", SqlDbType.Bit)
                        .Expression = "MobizLibraryPimProduct.ProductName"
                    End With
                    With .Add("ProductID", SqlDbType.UniqueIdentifier)
                        .Expression = "MobizLibraryPimProduct.ID"
                    End With
                    With .Add("UnitMeasure", SqlDbType.Bit)
                        .Expression = "ISNULL(UnitMeasure,'-')"
                    End With
                    With .Add("BeginningBalance", SqlDbType.Bit)
                        .Expression = "ISNULL(tblBeginningBalance.BeginningBalance, 0)"
                    End With

                    'With .Add("Delivery", SqlDbType.Int)
                    '    .Expression = "SUM(ISNULL(t0.Delivery, 0))"
                    'End With
                    'With .Add("TransIn", SqlDbType.Bit)
                    '    .Expression = "SUM(ISNULL(t0.TransIn, 0))"
                    'End With
                    'With .Add("TransOut", SqlDbType.Bit)
                    '    .Expression = "SUM(ISNULL(t0.TransOut, 0))"
                    'End With
                    'With .Add("Wastage", SqlDbType.Bit)
                    '    .Expression = "0"
                    '    '.Expression = "SUM(ISNULL(CacheInventoryMovement.TransOut, 0))"
                    'End With
                    With .Add("EndingBalance", SqlDbType.Bit)
                        .Expression = "ISNULL(tblEndingBalance.EndingBalance, 0)"
                    End With
                    'With .Add("Usage", SqlDbType.Bit)
                    '    .Expression = "SUM(ISNULL(t0.Usage, 0))"
                    'End With
                    'With .Add("Variance", SqlDbType.Bit)
                    '    .Expression = "ISNULL(tblBeginningBalance.BeginningBalance, 0) + SUM(ISNULL(CacheInventoryMovement.Delivery, 0)) +  SUM(ISNULL(CacheInventoryMovement.TransIn, 0)) " & vbCrLf &
                    '                  "- SUM(ISNULL(CacheInventoryMovement.TransOut, 0)) - ISNULL(tblEndingBalance.EndingBalance, 0) " & vbCrLf &
                    '                  "+ SUM(ISNULL(CacheInventoryMovement.Usage, 0)) "
                    'End With
                    With .Add("InventoryCountAM", SqlDbType.Bit)
                        .Expression = "ISNULL(tblInventoryCountCrew.Quantity, 0)"
                    End With
                    With .Add("InventoryCountAMVariance", SqlDbType.Bit)
                        .Expression = "ISNULL(tblInventoryCountCrew.Balance, 0)"
                    End With
                    With .Add("InventoryCountAMDate", SqlDbType.Bit)
                        .Expression = "ISNULL(convert(varchar(25), FORMAT(tblInventoryCountCrew.InventoryCountDate, 'dd/MMM'), 120),'-')"
                    End With
                    With .Add("InventoryCountAMName", SqlDbType.Bit)
                        .Expression = "ISNULL(tblInventoryCountCrew.CountedBy, '')"
                    End With

                    With .Add("InventoryCountCrew", SqlDbType.Bit)
                        .Expression = "ISNULL(tblInventoryCountAM.Quantity, 0)"
                    End With
                    With .Add("InventoryCountCrewVariance", SqlDbType.Bit)
                        .Expression = "ISNULL(tblInventoryCountAM.Balance, 0)"
                    End With
                    With .Add("InventoryCountCrewDate", SqlDbType.Bit)
                        .Expression = "ISNULL(convert(varchar(25), FORMAT(tblInventoryCountAM.InventoryCountDate, 'dd/MMM'), 120),'-')"
                    End With
                    With .Add("InventoryCountCrewName", SqlDbType.Bit)
                        .Expression = "ISNULL(tblInventoryCountAM.CountedBy, '')"
                    End With

                    With .Add("FOWSTName", SqlDbType.Bit)
                        .Expression = "REPLACE(ISNULL(FOWSTName,'-'),'.','')"
                    End With

                    'With .Add("Cost", SqlDbType.Bit)
                    '    .Expression = "ISNULL(pPrice.Price, 0)"
                    'End With
                    'With .Add("Shortlanded", SqlDbType.Bit)
                    '    .Expression = "SUM(tblShortlanded.QuantityGood)"
                    'End With
                End With

                .SqlFrom = " FROM MobizLibraryPimProduct " & vbCrLf &
                            " INNER JOIN CacheInventoryMovement t0 ON t0.ProductID = MobizLibraryPimProduct.ID" & vbCrLf &
                            " AND t0.FranchisegroupID = " & ReadyString(GroupID) & vbCrLf &
                            " AND t0.TransactionDate BETWEEN " & ProSqlite.ReadyDate(InventoryCountDate) & " AND " & ProSqlite.ReadyDate(InventoryCountDate) & vbCrLf &
                            " INNER JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPimProduct.ID " & vbCrLf

                '" FROM MobizLibraryPimProduct " & vbCrLf &
                '        "JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                '" INNER JOIN MobizLibraryPIMProductMeasurement as pimmsr ON pimmsr.ProductID = MobizLibraryPimProduct.ID AND MeasurementLevel = 1 " & vbCrLf &
                '" INNER JOIN MobizLibraryPIMMeasure as msr ON msr.ID = pimmsr.UnitMeasureID  " & vbCrLf &
                '" INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " & vbCrLf &


                .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                            "SELECT TOP 1 t2.BeginningBalance " & vbCrLf &
                            "FROM CacheInventoryMovement t2 " & vbCrLf &
                            "WHERE t2.FowstID  = t0.FowstID " & vbCrLf &
                            "AND t2.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                            "AND t2.TransactionDate >= " & ReadyString(InventoryCountDate) & vbCrLf &
                            "ORDER BY t2.TransactionDate ASC " & vbCrLf &
                            ") tblBeginningBalance " & vbCrLf

                .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                            "SELECT DISTINCT TOP 1 Quantity, TransInventoryProductCount.InventoryCountDate, MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.Firstname CountedBy, Balance " & vbCrLf &
                            "FROM TransInventoryProductCountStock INNER JOIN TransInventoryProductCount ON TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID " & vbCrLf &
                            " INNER JOIN MobizLibraryPeople on MobizLibraryPeople.ID = TransInventoryProductCount.UserID " & vbCrLf &
                            " INNER JOIN TransInventoryLedger on TransInventoryLedger.TransactionID = TransInventoryProductCount.ID " & vbCrLf &
                            "WHERE TransInventoryProductCount.FowstID = t0.FowstID " & vbCrLf &
                            "AND TransInventoryProductCount.ProductID = MobizLibraryPimProduct.ID  " & vbCrLf &
                            "AND CONVERT(date, TransInventoryProductCount.InventoryCountDate) = " & ReadyString(InventoryCountDate) & vbCrLf &
                            "ORDER BY TransInventoryProductCount.InventoryCountDate ASC " & vbCrLf &
                            ") tblInventoryCountCrew "

                .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                            "SELECT DISTINCT TOP 1 Quantity, TransInventoryProductCount.InventoryCountDate, MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.Firstname CountedBy, Balance " & vbCrLf &
                            "FROM TransInventoryProductCountStock INNER JOIN TransInventoryProductCount ON TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID " & vbCrLf &
                            " INNER JOIN MobizLibraryPeople on MobizLibraryPeople.ID = TransInventoryProductCount.UserID " & vbCrLf &
                            " INNER JOIN TransInventoryLedger on TransInventoryLedger.TransactionID = TransInventoryProductCount.ID " & vbCrLf &
                            "WHERE TransInventoryProductCount.FowstID = t0.FowstID " & vbCrLf &
                            "AND TransInventoryProductCount.ProductID = MobizLibraryPimProduct.ID  " & vbCrLf &
                            "AND CONVERT(date, TransInventoryProductCount.InventoryCountDate) = " & ReadyString(InventoryCountDate) & vbCrLf &
                            "ORDER BY TransInventoryProductCount.InventoryCountDate DESC " & vbCrLf &
                            ") tblInventoryCountAM "

                .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                            "SELECT TOP 1 t1.EndingBalance " & vbCrLf &
                            "FROM CacheInventoryMovement t1 " & vbCrLf &
                            "WHERE t1.FowstID = t0.FowstID " & vbCrLf &
                            "AND t1.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                            "AND CONVERT(date, t1.TransactionDate) <= " & ReadyString(InventoryCountDate) & vbCrLf &
                            "ORDER BY t1.TransactionDate DESC " & vbCrLf &
                            ") tblEndingBalance " & vbCrLf

                '.SqlFrom &= " OUTER APPLY ( " & vbCrLf &
                '             "SELECT TOP 1 trs.QuantityGood " & vbCrLf &
                '            "FROM TransReceiving tr " & vbCrLf &
                '            "JOIN TransReceivingStock trs ON trs.ReceivingID = tr.ID " & vbCrLf &
                '            "WHERE tr.ReceivingPlaceID = t0.FowstID " & vbCrLf &
                '            " AND trs.ProductID = MobizLibraryPimProduct.ID  " & vbCrLf &
                '            " AND CONVERT(date, tr.DocumentDate) = " & ReadyString(DateTo) & vbCrLf &
                '            " AND ISNULL(trs.IsShortLanded,0) = 1 " & vbCrLf &
                '            "ORDER BY tr.DocumentDate DESC  " & vbCrLf &
                '            ") tblShortlanded " & vbCrLf

                '.SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                '            "Select top 1 Price from MobizLibraryPIMProductPriceLevel  " & vbCrLf &
                '            "where MobizLibraryPIMProductPriceLevel.ProductID = t0.ProductID AND MobizLibraryPIMProductPriceLevel .PriceLevelID = '01000012-0003-0000-0000-000000000000' " & vbCrLf &
                '            "AND Organizationid=" & ReadyString(Common.WebRequest.Data("OrgID")) & ") pPrice " & vbCrLf 'landed cost 

                .SqlWhere = "where ISNULL(LibraryOrganizationPIM.IsInventoryCountItem,0) = 1 AND ISNULL(tblInventoryCountCrew.CountedBy, '') <> ISNULL(tblInventoryCountAM.CountedBy, '') " & vbCrLf

                .SqlGroup = " GROUP BY ISNULL(LibraryOrganizationPIM.InventoryRankingNo,9999), MobizLibraryPimProduct.ID, MobizLibraryPimProduct.ProductName, tblBeginningBalance.BeginningBalance, tblEndingBalance.EndingBalance, tblInventoryCountCrew.Quantity, tblInventoryCountAM.Quantity " & vbCrLf &
                            ", tblInventoryCountCrew.InventoryCountDate, ISNULL(tblInventoryCountCrew.CountedBy, ''), tblInventoryCountAM.InventoryCountDate, tblInventoryCountAM.CountedBy  " & vbCrLf &
                            ", ISNULL(tblInventoryCountCrew.Balance, 0), ISNULL(tblInventoryCountAM.Balance, 0), Category, UnitMeasure, FOWSTName " & vbCrLf

                .SqlOrder = " ORDER BY ISNULL(LibraryOrganizationPIM.InventoryRankingNo,9999) --Category,ProductName" & vbCrLf

                'AppendToLog(.CreateSql())

                pDT = .Requery


                If datesctr = 0 Then
                    dtAMVsCrewCount = pDT
                Else
                    For Each row In pDT.Rows
                        dtAMVsCrewCount.ImportRow(row)
                    Next
                End If

            End With

        Next

        If dtAMVsCrewCount.Rows.Count > 0 Then

            '"FOWSTName", , "BeginningBalance", "EndingBalance"
            pFilter1 = CreateCrossTab(dtAMVsCrewCount, {"Category", "ProductName", "UnitMeasure"}, {New CrossColumn("{FOWSTName} {InventoryCountCrewDate} BegBalance", "BeginningBalance", "BeginningBalance"),
                                                                                New CrossColumn("{FOWSTName} {InventoryCountCrewDate} EndingBalance", "EndingBalance", "EndingBalance"),
                                                                                 New CrossColumn("{FOWSTName} {InventoryCountCrewDate} CrewCount", "InventoryCountCrew", "InventoryCountCrew"),
                                                                                  New CrossColumn("{FOWSTName} {InventoryCountAMDate} AMCount", "InventoryCountAM", "InventoryCountAM")})


            pFilter1.Columns.Remove("InventoryCountCrew")
            pFilter1.Columns.Remove("InventoryCountAM")
            pFilter1.Columns.Remove("BeginningBalance")
            pFilter1.Columns.Remove("EndingBalance")

            'put 0 to null
            For i As Integer = 0 To pFilter1.Rows.Count - 1
                For j As Integer = 3 To pFilter1.Columns.Count - 1
                    If CStr2(pFilter1.Rows(i).Item(j)) = vbNullString Then
                        pFilter1.Rows(i).Item(j) = 0
                    End If
                Next
            Next

        End If


        Return pFilter1
        'Return pDT
    End Function

    Function InventoryReportCrewVSAM() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim dtAMVsCrewCount As New DataTable
        Dim dtDates As New DataTable
        Dim InventoryCountDate As String

        dtDates = InventoryReportCrewVSAMDate()

        For datesctr As Integer = 0 To dtDates.Rows.Count - 1

            InventoryCountDate = CStr2(dtDates.Rows(datesctr).Item("InventoryCountDate"))

            pFinder = New Finder
            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("Category", SqlDbType.Bit)
                        .Expression = "MobizLibraryPIMProductVariableFeatures001.C0020"
                    End With
                    With .Add("ProductName", SqlDbType.Bit)
                        .Expression = "MobizLibraryPimProduct.ProductName"
                    End With
                    With .Add("ProductID", SqlDbType.UniqueIdentifier)
                        .Expression = "MobizLibraryPimProduct.ID"
                    End With
                    With .Add("UnitMeasure", SqlDbType.Bit)
                        .Expression = "lbl.Label"
                    End With
                    With .Add("Balance", SqlDbType.Bit)
                        .Expression = "ISNULL(tblBeginningBalance.BeginningBalance, 0)"
                    End With
                    With .Add("Delivery", SqlDbType.Int)
                        .Expression = "SUM(ISNULL(CacheInventoryMovement.Delivery, 0))"
                    End With
                    With .Add("TransIn", SqlDbType.Bit)
                        .Expression = "SUM(ISNULL(CacheInventoryMovement.TransIn, 0))"
                    End With
                    With .Add("TransOut", SqlDbType.Bit)
                        .Expression = "SUM(ISNULL(CacheInventoryMovement.TransOut, 0))"
                    End With
                    With .Add("Wastage", SqlDbType.Bit)
                        .Expression = "0"
                        '.Expression = "SUM(ISNULL(CacheInventoryMovement.TransOut, 0))"
                    End With
                    With .Add("Ending", SqlDbType.Bit)
                        .Expression = "ISNULL(tblEndingBalance.EndingBalance, 0)"
                    End With
                    With .Add("Usage", SqlDbType.Bit)
                        .Expression = "SUM(ISNULL(CacheInventoryMovement.Usage, 0))"
                    End With
                    'With .Add("Variance", SqlDbType.Bit)
                    '    .Expression = "ISNULL(tblBeginningBalance.BeginningBalance, 0) + SUM(ISNULL(CacheInventoryMovement.Delivery, 0)) +  SUM(ISNULL(CacheInventoryMovement.TransIn, 0)) " & vbCrLf &
                    '                  "- SUM(ISNULL(CacheInventoryMovement.TransOut, 0)) - ISNULL(tblEndingBalance.EndingBalance, 0) " & vbCrLf &
                    '                  "+ SUM(ISNULL(CacheInventoryMovement.Usage, 0)) "
                    'End With
                    With .Add("InventoryCountCrew", SqlDbType.Bit)
                        .Expression = "ISNULL(tblInventoryCountCrew.Quantity, 0)"
                    End With
                    With .Add("InventoryCountCrewVariance", SqlDbType.Bit)
                        .Expression = "ISNULL(tblInventoryCountCrew.Balance, 0)"
                    End With
                    With .Add("InventoryCountCrewDate", SqlDbType.Bit)
                        .Expression = "tblInventoryCountCrew.InventoryCountDate"
                    End With
                    With .Add("InventoryCountCrewName", SqlDbType.Bit)
                        .Expression = "ISNULL(tblInventoryCountCrew.CountedBy, '')"
                    End With

                    With .Add("InventoryCountAM", SqlDbType.Bit)
                        .Expression = "ISNULL(tblInventoryCountAM.Quantity, 0)"
                    End With
                    With .Add("InventoryCountAMVariance", SqlDbType.Bit)
                        .Expression = "ISNULL(tblInventoryCountAM.Balance, 0)"
                    End With
                    With .Add("InventoryCountAMDate", SqlDbType.Bit)
                        .Expression = "tblInventoryCountAM.InventoryCountDate"
                    End With
                    With .Add("InventoryCountAMName", SqlDbType.Bit)
                        .Expression = "ISNULL(tblInventoryCountAM.CountedBy, '')"
                    End With

                    With .Add("Cost", SqlDbType.Bit)
                        .Expression = "ISNULL(pPrice.Price, 0)"
                    End With
                    With .Add("Shortlanded", SqlDbType.Bit)
                        .Expression = "SUM(tblShortlanded.QuantityGood)"
                    End With
                End With

                .SqlFrom = " FROM MobizLibraryPimProduct " & vbCrLf &
                            "JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                            " JOIN CacheInventoryMovement ON CacheInventoryMovement.ProductID = MobizLibraryPimProduct.ID" & vbCrLf &
                            " AND CacheInventoryMovement.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                            " AND CacheInventoryMovement.TransactionDate BETWEEN " & ProSqlite.ReadyDate(InventoryCountDate) & " AND " & ProSqlite.ReadyDate(InventoryCountDate) & vbCrLf &
                            " INNER JOIN MobizLibraryPIMProductMeasurement as pimmsr ON pimmsr.ProductID = MobizLibraryPimProduct.ID AND MeasurementLevel = 1 " & vbCrLf &
                            " INNER JOIN MobizLibraryPIMMeasure as msr ON msr.ID = pimmsr.UnitMeasureID  " & vbCrLf &
                            " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " & vbCrLf &
                            " INNER JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPimProduct.ID " & vbCrLf

                .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                            "SELECT TOP 1 t2.BeginningBalance " & vbCrLf &
                            "FROM CacheInventoryMovement t2 " & vbCrLf &
                            "WHERE t2.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                            "AND t2.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                            "AND t2.TransactionDate >= " & ReadyString(InventoryCountDate) & vbCrLf &
                            "ORDER BY t2.TransactionDate ASC " & vbCrLf &
                            ") tblBeginningBalance " & vbCrLf

                .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                            "SELECT DISTINCT TOP 1 Quantity, TransInventoryProductCount.InventoryCountDate, MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.Firstname CountedBy, Balance " & vbCrLf &
                            "FROM TransInventoryProductCountStock INNER JOIN TransInventoryProductCount ON TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID " & vbCrLf &
                            " INNER JOIN MobizLibraryPeople on MobizLibraryPeople.ID = TransInventoryProductCount.UserID " & vbCrLf &
                            " INNER JOIN TransInventoryLedger on TransInventoryLedger.TransactionID = TransInventoryProductCount.ID " & vbCrLf &
                            "WHERE TransInventoryProductCount.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                            "AND TransInventoryProductCount.ProductID = MobizLibraryPimProduct.ID  " & vbCrLf &
                            "AND CONVERT(date, TransInventoryProductCount.InventoryCountDate) = " & ReadyString(InventoryCountDate) & vbCrLf &
                            "ORDER BY TransInventoryProductCount.InventoryCountDate ASC " & vbCrLf &
                            ") tblInventoryCountCrew "

                .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                            "SELECT DISTINCT TOP 1 Quantity, TransInventoryProductCount.InventoryCountDate, MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.Firstname CountedBy, Balance " & vbCrLf &
                            "FROM TransInventoryProductCountStock INNER JOIN TransInventoryProductCount ON TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID " & vbCrLf &
                            " INNER JOIN MobizLibraryPeople on MobizLibraryPeople.ID = TransInventoryProductCount.UserID " & vbCrLf &
                            " INNER JOIN TransInventoryLedger on TransInventoryLedger.TransactionID = TransInventoryProductCount.ID " & vbCrLf &
                            "WHERE TransInventoryProductCount.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                            "AND TransInventoryProductCount.ProductID = MobizLibraryPimProduct.ID  " & vbCrLf &
                            "AND ISNULL(tblInventoryCountCrew.CountedBy, '') <> ISNULL(MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.Firstname, '') " & vbCrLf &
                            "AND CONVERT(date, TransInventoryProductCount.InventoryCountDate) = " & ReadyString(InventoryCountDate) & vbCrLf &
                            "ORDER BY TransInventoryProductCount.InventoryCountDate DESC " & vbCrLf &
                            ") tblInventoryCountAM "

                .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                            "SELECT TOP 1 t1.EndingBalance " & vbCrLf &
                            "FROM CacheInventoryMovement t1 " & vbCrLf &
                            "WHERE t1.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                            "AND t1.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                            "AND CONVERT(date, t1.TransactionDate) <= " & ReadyString(InventoryCountDate) & vbCrLf &
                            "ORDER BY t1.TransactionDate DESC " & vbCrLf &
                            ") tblEndingBalance " & vbCrLf

                .SqlFrom &= " OUTER APPLY ( " & vbCrLf &
                             "SELECT TOP 1 trs.QuantityGood " & vbCrLf &
                            "FROM TransReceiving tr " & vbCrLf &
                            "JOIN TransReceivingStock trs ON trs.ReceivingID = tr.ID " & vbCrLf &
                            "WHERE tr.ReceivingPlaceID = " & ReadyString(FOWSTID) & vbCrLf &
                            " AND trs.ProductID = MobizLibraryPimProduct.ID  " & vbCrLf &
                            " AND CONVERT(date, tr.DocumentDate) = " & ReadyString(InventoryCountDate) & vbCrLf &
                            " AND ISNULL(trs.IsShortLanded,0) = 1 " & vbCrLf &
                            "ORDER BY tr.DocumentDate DESC  " & vbCrLf &
                            ") tblShortlanded " & vbCrLf

                .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                            "Select top 1 Price from MobizLibraryPIMProductPriceLevel  " & vbCrLf &
                            "where MobizLibraryPIMProductPriceLevel.ProductID = CacheInventoryMovement.ProductID AND MobizLibraryPIMProductPriceLevel .PriceLevelID = '01000012-0003-0000-0000-000000000000' " & vbCrLf &
                            "AND Organizationid=" & ReadyString(Common.WebRequest.Data("OrgID")) & ") pPrice " & vbCrLf 'landed cost 

                .SqlWhere = "where ISNULL(LibraryOrganizationPIM.IsInventoryCountItem,0) = 1 AND ISNULL(tblInventoryCountCrew.CountedBy, '') <> ISNULL(tblInventoryCountAM.CountedBy, '')  " & vbCrLf
                .SqlGroup = " GROUP BY ISNULL(LibraryOrganizationPIM.InventoryRankingNo,9999), MobizLibraryPimProduct.ID, MobizLibraryPIMProductVariableFeatures001.C0020 " & vbCrLf &
                            ", MobizLibraryPimProduct.ProductName, tblBeginningBalance.BeginningBalance, tblEndingBalance.EndingBalance, lbl.Label, tblInventoryCountCrew.Quantity, tblInventoryCountAM.Quantity, pPrice.Price " & vbCrLf &
                            ", tblInventoryCountCrew.InventoryCountDate, ISNULL(tblInventoryCountCrew.CountedBy, ''), tblInventoryCountAM.InventoryCountDate, tblInventoryCountAM.CountedBy " & vbCrLf &
                            ", ISNULL(tblInventoryCountCrew.Balance, 0), ISNULL(tblInventoryCountAM.Balance, 0) "

                .SqlOrder = " ORDER BY ISNULL(LibraryOrganizationPIM.InventoryRankingNo,9999) --Category,ProductName" & vbCrLf

                pDT = .Requery

                If datesctr = 0 Then
                    dtAMVsCrewCount = pDT
                Else
                    For Each row In pDT.Rows
                        dtAMVsCrewCount.ImportRow(row)
                    Next
                End If

            End With
        Next

        Return dtAMVsCrewCount
    End Function

    Function GetDeliveryReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        Try
            With pFinder
                .ProData = Common.WebRequest.ProData

                .Distinct = True

                With .Fields
                    With .Add("DateReceived", SqlDbType.Bit)
                        .Expression = "TransReceiving.DocumentDate"
                    End With
                    With .Add("ReceivedIn", SqlDbType.Bit)
                        .Expression = "FOWSTSource.Name"
                    End With
                    With .Add("Supplier", SqlDbType.Bit)
                        .Expression = "ISNULL(MobizLibraryOrganizationFOWST.Name, MobizLibraryOrganization.CompanyName)"
                    End With
                    With .Add("ReceivingNo", SqlDbType.Bit)
                        .Expression = "TransReceiving.ReceivingNo"
                    End With
                    With .Add("UnitMeasure", SqlDbType.Bit)
                        .Expression = "lbl.Label"
                    End With
                    With .Add("Price", SqlDbType.Bit)
                        .Expression = "prc.Price"
                    End With
                    With .Add("Category", SqlDbType.Bit)
                        .Expression = "ISNULL(MobizLibraryPIMProductVariableFeatures001.C0020,'')"
                    End With
                    With .Add("ProductName", SqlDbType.Bit)
                        .Expression = "MobizLibraryPimProduct.ProductName"
                    End With
                    With .Add("Quantity", SqlDbType.Bit)
                        .Expression = "TransReceivingStock.QuantityGood"
                    End With
                    With .Add("TransportedBy", SqlDbType.Bit)
                        .Expression = "TransReceiving.TransportedBy"
                    End With
                    With .Add("Crew", SqlDbType.VarChar)
                        .Expression = "users.FirstName + ' ' + users.Surname"
                    End With
                End With

                .SqlFrom = " FROM TransReceiving " & vbCrLf &
                            "JOIN TransReceivingStock ON TransReceivingStock.ReceivingID = TransReceiving.ID " & vbCrLf &
                            "JOIN MobizLibraryPimProduct ON TransReceivingStock.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                            "JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
                            "INNER JOIN MobizLibraryPIMProductMeasurement as pimmsr ON pimmsr.ProductID = TransReceivingStock.ProductID  " & vbCrLf &
                            "INNER JOIN MobizLibraryPIMMeasure as msr ON msr.ID = TransReceivingStock.UnitMeasureGood  " & vbCrLf &
                            "INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " & vbCrLf &
                            "INNER JOIN MobizLibraryOrganizationFOWST FOWSTSource on FOWSTSource.ID = TransReceiving.ReceivingPlaceID " & vbCrLf &
                            "LEFT JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransReceiving.SupplierID " & vbCrLf &
                            "LEFT JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransReceiving.SupplierID " & vbCrLf &
                            "LEFT JOIN MobizLibraryPIMProductPriceLevel as prc ON prc.ProductID = TransReceivingStock.ProductID " & vbCrLf &
                            "LEFT JOIN MobizLibraryPeople as users ON users.ID = TransReceiving.UserID "

                .SqlWhere = " WHERE TransReceiving.SupplierID <> " & ReadyString(CStr2(Common.WebRequest.Data("OrgID"))) & " " & vbCrLf &
                            "AND CONVERT(date, TransReceiving.DocumentDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                            " AND prc.PriceLevelID = '01000012-0005-0000-0000-000000000000' -- retail price " & vbCrLf &
                            " AND prc.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID")) & vbCrLf

                If Not String.IsNullOrWhiteSpace(GroupID) Then
                    FOWSTID = Common.WebRequest.ProData.ExecuteScalar("DECLARE @fowstid as varchar(max) = (SELECT STUFF((SELECT " & vbCrLf &
                                " '''' +CONVERT(varchar(max), FranchiseeOrgFowstID) + ''', ' " & vbCrLf &
                                " FROM MobizLibraryOrganizationFranchisee " & vbCrLf &
                                " WHERE FranchiseGroupID IN (" & GroupID & ") " & vbCrLf &
                                " GROUP BY FranchiseeOrgFowstID " & vbCrLf &
                                " FOR XML PATH('') " & vbCrLf &
                                " ), 1, 0, '')) " & vbCrLf &
                                " SELECT STUFF(@fowstid, LEN(@fowstid), 1, '') as FOWSTID " & vbCrLf)
                End If

                'If Filter = "StockReceivingFromFOWST" Then
                '    .SqlWhere &= " AND FOWSTSource.ID = " & ReadyString(FOWSTID) & vbCrLf &
                '                " AND IsReceiving = 1 AND TransReceiving.SupplierID IN (SELECT ID from MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(CStr2(Common.WebRequest.Data("OrgID"))) & " )"
                'Else
                If Filter = "StockReceivingFromSupplier" Then
                    .SqlWhere &= " AND FOWSTSource.ID IN (" & FOWSTID & ") " & vbCrLf &
                                 " AND IsReceiving = 1 AND TransReceiving.SupplierID NOT IN (SELECT ID from MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(CStr2(Common.WebRequest.Data("OrgID"))) & " )"
                ElseIf Filter = "StockTransferFromFOWST" Then
                    .SqlWhere &= " AND IsReceiving = 0 AND TransReceiving.ReceivingPlaceID IN (" & FOWSTID & ") AND MobizLibraryOrganizationFOWST.Name <> 'Wastage' " & vbCrLf
                ElseIf Filter = "Wastage" Then
                    .SqlWhere &= " AND IsReceiving = 0 AND TransReceiving.ReceivingPlaceID IN (" & FOWSTID & ") AND MobizLibraryOrganizationFOWST.Name = 'Wastage' " & vbCrLf
                ElseIf Filter = "ForInventoryStore" Then
                    .SqlWhere &= " AND IsReceiving = 0 AND TransReceiving.ReceivingPlaceID IN (" & FOWSTID & ") AND MobizLibraryOrganizationFOWST.Name = 'For Inventory Store' " & vbCrLf
                ElseIf Filter = "Consumables" Then
                    .SqlWhere &= " AND IsReceiving = 0 AND TransReceiving.ReceivingPlaceID IN (" & FOWSTID & ") AND MobizLibraryOrganizationFOWST.Name = 'Consumables' " & vbCrLf
                ElseIf Filter = "Overlanded" Then
                    .SqlWhere &= " AND IsReceiving = 0 AND TransReceiving.ReceivingPlaceID IN (" & FOWSTID & ") AND MobizLibraryOrganizationFOWST.Name = 'Overlanded' " & vbCrLf
                ElseIf Filter = "Shortlanded" Then
                    .SqlWhere &= " AND IsReceiving = 0 AND TransReceiving.ReceivingPlaceID IN (" & FOWSTID & ") AND MobizLibraryOrganizationFOWST.Name = 'Shortlanded' " & vbCrLf
                ElseIf Filter = "ProductDefect" Then
                    .SqlWhere &= " AND IsReceiving = 0 AND TransReceiving.ReceivingPlaceID IN (" & FOWSTID & ") AND MobizLibraryOrganizationFOWST.Name = 'Product Defect' " & vbCrLf
                Else
                    .SqlWhere &= " AND FOWSTSource.ID IN (" & FOWSTID & ") " & vbCrLf &
                               " AND IsReceiving = 1 AND TransReceiving.SupplierID IN (SELECT ID from MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(CStr2(Common.WebRequest.Data("OrgID"))) & " )"
                    '.SqlWhere &= " AND TransReceiving.SupplierID = " & ReadyString(FOWSTID) & vbCrLf
                End If

                '.SqlGroup = "GROUP BY MobizLibraryPIMProductVariableFeatures001.C0020,MobizLibraryPimProduct.ProductName "

                .SqlOrder = " ORDER BY DateReceived, Category,ProductName"

                pDT = .Requery

            End With

            Return pDT
        Catch ex As Exception
            AppendToDebug(ex.Message)
        End Try
    End Function

    Function GetMonthlySalesReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True

            With .Fields
                With .Add("FOWSTCode", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(CacheInvoicesDaily.FOWSTCode,'-')"
                End With
                With .Add("FOWSTName", SqlDbType.UniqueIdentifier)
                    .Expression = "CacheInvoicesDaily.FOWSTName"
                End With
                With .Add("StoreType", SqlDbType.VarChar)
                    .Expression = "ISNULL(CacheInvoicesDaily.StoreType,'')"
                End With
                With .Add("StoreLocation", SqlDbType.VarChar)
                    .Expression = "ISNULL(CacheInvoicesDaily.StoreLocation,'')"
                End With

                With .Add("GroupName", SqlDbType.Int)
                    .Expression = "ISNULL(CacheInvoicesDaily.GroupName,'')"
                End With

                With .Add("TargetSales", SqlDbType.Bit)
                    .Expression = "ISNULL(LibraryOrganizationFOWSTQuota.Tier1Quota,0)"
                End With
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("SalesReturnAndRefund", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CreditMemo),0)"
                End With
                'With .Add("SalesReturnAndRefund", SqlDbType.Bit)
                '    .Expression = "ISNULL(" &
                '        "(Select SUM(Amount) FROM TransSalesReturnLedger " & vbCrLf &
                '        "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID AND TransSalesReturnLedger.FowstID = CacheInvoicesDaily.FowstID " & vbCrLf &
                '        "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL " & vbCrLf &
                '        "AND CONVERT(date,TransMemo.LastUpdated) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & ")" & vbCrLf &
                '         ",0)"
                'End With
                With .Add("SalesReturnAndRefundCount", SqlDbType.Bit)
                    .Expression = "ISNULL(" &
                        "(Select COUNT(distinct SourceMemoID) FROM TransSalesReturnLedger " & vbCrLf &
                        "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID AND TransSalesReturnLedger.FowstID = CacheInvoicesDaily.FOWSTID " & vbCrLf &
                        "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL " & vbCrLf &
                        "AND CONVERT(date,TransMemo.LastUpdated) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & ")" & vbCrLf &
                         ",0)"
                End With
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(TransactionCount),0)"
                End With
                'With .Add("TransactionCount", SqlDbType.Bit)
                '    .Expression = "ISNULL(COUNT(DISTINCT CacheInvoicesDaily.ID),0)"
                'End With
                With .Add("DiscountAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.SCDiscount),0) + ISNULL(SUM(CacheInvoicesDaily.ForegoneVAT),0) "
                End With
                With .Add("CashAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.CashNetOfChange),0)"
                End With
                With .Add("CreditCardAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.CreditCardAmount),0)"
                End With
                With .Add("DebitCardAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.DebitCardAmount),0)"
                End With
                With .Add("CheckPaymentAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.PotatoCornerGC),0)"
                End With

                'add Target Sales

                With .Add("DiscountVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[Discount Voucher / On Line Deals]),0)"
                End With
                With .Add("GCash", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[GCash]),0)"
                End With
                With .Add("PayMaya", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[PayMaya]),0)"
                End With
                With .Add("SMAC", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[SMAC]),0)"
                End With
                With .Add("GoldCard", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.GoldCard),0)"
                End With
                With .Add("ShopeeEVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[ShopeeEVoucher]),0)"
                End With
                With .Add("ShopeePay", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[ShopeePay]),0)"
                End With

                With .Add("FoodPanda", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[FoodPanda]),0)"
                End With
                With .Add("GrabFood", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[GrabFood]),0)"
                End With
                With .Add("LocalDelivery", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[LocalDelivery]),0)"
                End With
                With .Add("Eatigo", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[Eatigo]),0)"
                End With
                With .Add("DeliRush", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[DeliRush]),0)"
                End With
                With .Add("PoodTrip", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[PoodTrip]),0)"
                End With
                With .Add("ShareTreats", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[Share Treats]),0)"
                End With
                With .Add("Booky", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[Booky]),0)"
                End With
                With .Add("Lazada", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[Lazada]),0)"
                End With
                With .Add("Pickaroo", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesDaily.[Pickaroo]),0)"
                End With
                With .Add("LatestTransactionDate", SqlDbType.Bit)
                    .Expression = "MAX(CacheInvoicesDaily.InvoiceDate)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoicesDaily " & vbCrLf &
                        "--INNER JOIN MobizLibraryOrganizationFOWST  " & vbCrLf &
                        "--ON CacheInvoicesDaily.FowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                        "LEFT JOIN LibraryOrganizationFOWSTQuota on LibraryOrganizationFOWSTQuota.FOWSTID = CacheInvoicesDaily.FOWSTID " & vbCrLf &
                        "--LEFT JOIN MobizLibraryFOWSTStoreLocationType on MobizLibraryFOWSTStoreLocationType.ID = MobizLibraryOrganizationFOWST.StoreLocationTypeID " & vbCrLf &
                        "--LEFT JOIN TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = CacheInvoicesDaily.ID  " & vbCrLf &
                        "--LEFT JOIN TransCustomerPayment on TransCustomerPayment.ID = TransCustomerPaymentInvoices.CustomerPaymentID  " & vbCrLf &
                        "--LEFT JOIN TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPayment.ID  " & vbCrLf &
                        "--LEFT JOIN MobizLibraryFOWSTRetailStoreType on MobizLibraryFOWSTRetailStoreType.ID = MobizLibraryOrganizationFOWST.SubFOWSTTypeID " & vbCrLf &
                        "--INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                        "--INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf
            '" OUTER APPLY (" & vbCrLf &
            '" SELECT CacheInvoicesDaily2.GrossInvoiceAmount AS Total" & vbCrLf &
            '" FROM CacheInvoicesDaily CacheInvoicesDaily2" & vbCrLf &
            '" JOIN TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = CacheInvoicesDaily2.ID" & vbCrLf &
            '" JOIN TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPaymentInvoices.CustomerPaymentID" & vbCrLf &
            '" WHERE CacheInvoicesDaily.ID = CacheInvoicesDaily2.ID" & vbCrLf &
            '" AND PaymentType='GoldCard'" & vbCrLf &
            ' ") AS GoldCard " & vbCrLf &
            '"OUTER APPLY (  " & vbCrLf &
            '"SELECT * FROM    " & vbCrLf &
            '"(SELECT PaymentType, Amount " & vbCrLf &
            '" FROM TransCustomerPaymentInvoices " & vbCrLf &
            '" INNER JOIN TransCustomerPaymentOther on  TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentOther.CustomerPaymentID" & vbCrLf &
            '" WHERE TransCustomerPaymentInvoices.InvoiceID = CacheInvoicesDaily.ID " & vbCrLf &
            '") t  " & vbCrLf &
            '"PIVOT(SUM(Amount) " & vbCrLf &
            '"FOR PaymentType in ( " & vbCrLf &
            '"[Discount Voucher / On Line Deals], [E-Plus], [FoodPanda], [GCash], [GrabFood], [SMAC], [V-Plus], [PayMaya], [ShopeeEVoucher], [ShopeePay], [GoldCard], [LocalDelivery])) as OtherPayment) as OtherPayment "

            .SqlWhere = " WHERE convert(date, CacheInvoicesDaily.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & " " & vbCrLf &
                        "--AND MONTH(LibraryOrganizationFOWSTQuota.MonthYear) = Month(" & ReadyString(DateFrom) & ") " & vbCrLf &
                        "--AND YEAR(LibraryOrganizationFOWSTQuota.MonthYear) = YEAR(" & ReadyString(DateFrom) & ") " & vbCrLf

            If CStr2(Common.WebRequest.Data("StoreType")) <> "" Then
                .SqlWhere += " AND CacheInvoicesDaily.SubFOWSTTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreType"))) & vbCrLf
            End If

            If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
                .SqlWhere += " AND CacheInvoicesDaily.StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
            End If

            If Not FOWSTID = vbNullString Then
                .SqlWhere &= "AND CacheInvoicesDaily.FOWSTID = " & ReadyString(FOWSTID) & vbCrLf
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= "AND CacheInvoicesDaily.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "CacheInvoicesDaily.FOWSTName " & vbCrLf &
                        ", CacheInvoicesDaily.StoreType  " & vbCrLf &
                        ", CacheInvoicesDaily.StoreLocation " & vbCrLf &
                        ",CacheInvoicesDaily.GroupName " & vbCrLf &
                        ", LibraryOrganizationFOWSTQuota.Tier1Quota " & vbCrLf &
                        ",CacheInvoicesDaily.FowstID " & vbCrLf &
                        ",CacheInvoicesDaily.FOWSTCode "

            '.SqlGroup = " GROUP BY " & vbCrLf &
            '            "MobizLibraryOrganizationFOWST.Name " & vbCrLf &
            '            ", MobizLibraryFOWSTRetailStoreType.Name  " & vbCrLf &
            '            ", LibraryFranchiseGroup.GroupName " & vbCrLf &
            '            ", MobizLibraryFOWSTStoreLocationType.LocationType " & vbCrLf &
            '            ", LibraryOrganizationFOWSTQuota.Tier1Quota " & vbCrLf &
            '            ", MobizLibraryOrganizationFOWST.ID "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetMonthlySalesReportFranchisee() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True

            With .Fields
                With .Add("FOWSTCode", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(CacheInvoices.FOWSTCode,'-')"
                End With
                With .Add("FOWSTName", SqlDbType.UniqueIdentifier)
                    .Expression = "CacheInvoices.FOWSTName"
                End With
                With .Add("StoreType", SqlDbType.VarChar)
                    .Expression = "ISNULL(CacheInvoices.StoreType,'')"
                End With
                With .Add("StoreLocation", SqlDbType.VarChar)
                    .Expression = "ISNULL(CacheInvoices.StoreLocation,'')"
                End With

                With .Add("GroupName", SqlDbType.Int)
                    .Expression = "ISNULL(CacheInvoices.GroupName,'')"
                End With

                With .Add("TargetSales", SqlDbType.Bit)
                    .Expression = "ISNULL(LibraryOrganizationFOWSTQuota.Tier1Quota,0)"
                End With
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With
                'With .Add("SalesReturnAndRefund", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM(CreditMemo),0)"
                'End With
                With .Add("SalesReturnAndRefund", SqlDbType.Bit)
                    .Expression = "ISNULL(" &
                        "(Select SUM(Amount) FROM TransSalesReturnLedger " & vbCrLf &
                        "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID AND TransSalesReturnLedger.FowstID = CacheInvoices.FowstID " & vbCrLf &
                        "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL " & vbCrLf &
                        "AND CONVERT(date,TransMemo.LastUpdated) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & ")" & vbCrLf &
                         ",0)"
                End With
                With .Add("SalesReturnAndRefundCount", SqlDbType.Bit)
                    .Expression = "ISNULL(" &
                        "(Select COUNT(distinct SourceMemoID) FROM TransSalesReturnLedger " & vbCrLf &
                        "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID AND TransSalesReturnLedger.FowstID = CacheInvoices.FOWSTID " & vbCrLf &
                        "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL " & vbCrLf &
                        "AND CONVERT(date,TransMemo.LastUpdated) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & ")" & vbCrLf &
                         ",0)"
                End With
                'With .Add("TransactionCount", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM(TransactionCount),0)"
                'End With
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "ISNULL(COUNT(DISTINCT CacheInvoices.ID),0)"
                End With
                With .Add("DiscountAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.SCDiscount),0) + ISNULL(SUM(CacheInvoices.ForegoneVAT),0) "
                End With
                With .Add("CashAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.CashNetOfChange),0)"
                End With
                With .Add("CreditCardAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.CreditCardAmount),0)"
                End With
                With .Add("DebitCardAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.DebitCardAmount),0)"
                End With
                With .Add("CheckPaymentAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.PotatoCornerGC),0)"
                End With

                'add Target Sales

                With .Add("DiscountVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Discount Voucher / On Line Deals]),0)"
                End With
                With .Add("GCash", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[GCash]),0)"
                End With
                With .Add("PayMaya", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[PayMaya]),0)"
                End With
                With .Add("SMAC", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[SMAC]),0)"
                End With
                With .Add("GoldCard", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.GoldCard),0)"
                End With
                With .Add("ShopeeEVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[ShopeeEVoucher]),0)"
                End With
                With .Add("ShopeePay", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[ShopeePay]),0)"
                End With

                With .Add("FoodPanda", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[FoodPanda]),0)"
                End With
                With .Add("GrabFood", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[GrabFood]),0)"
                End With
                With .Add("LocalDelivery", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[LocalDelivery]),0)"
                End With
                With .Add("Eatigo", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Eatigo]),0)"
                End With
                With .Add("DeliRush", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[DeliRush]),0)"
                End With
                With .Add("PoodTrip", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[PoodTrip]),0)"
                End With
                With .Add("ShareTreats", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Share Treats]),0)"
                End With
                With .Add("Booky", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Booky]),0)"
                End With
                With .Add("Lazada", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Lazada]),0)"
                End With
                With .Add("Pickaroo", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Pickaroo]),0)"
                End With
                With .Add("LatestTransactionDate", SqlDbType.Bit)
                    .Expression = "MAX(CacheInvoices.InvoiceDate)"
                End With
                With .Add("SupercardVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.SupercardVoucher),0)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf &
                        "--INNER JOIN MobizLibraryOrganizationFOWST  " & vbCrLf &
                        "--ON CacheInvoices.FowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                        "LEFT JOIN LibraryOrganizationFOWSTQuota on LibraryOrganizationFOWSTQuota.FOWSTID = CacheInvoices.FOWSTID " & vbCrLf &
                        "--LEFT JOIN MobizLibraryFOWSTStoreLocationType on MobizLibraryFOWSTStoreLocationType.ID = MobizLibraryOrganizationFOWST.StoreLocationTypeID " & vbCrLf &
                        "--LEFT JOIN TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = CacheInvoices.ID  " & vbCrLf &
                        "--LEFT JOIN TransCustomerPayment on TransCustomerPayment.ID = TransCustomerPaymentInvoices.CustomerPaymentID  " & vbCrLf &
                        "--LEFT JOIN TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPayment.ID  " & vbCrLf &
                        "--LEFT JOIN MobizLibraryFOWSTRetailStoreType on MobizLibraryFOWSTRetailStoreType.ID = MobizLibraryOrganizationFOWST.SubFOWSTTypeID " & vbCrLf &
                        "--INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                        "--INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf
            '" OUTER APPLY (" & vbCrLf &
            '" SELECT CacheInvoicesDaily2.GrossInvoiceAmount AS Total" & vbCrLf &
            '" FROM CacheInvoicesDaily CacheInvoicesDaily2" & vbCrLf &
            '" JOIN TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = CacheInvoicesDaily2.ID" & vbCrLf &
            '" JOIN TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPaymentInvoices.CustomerPaymentID" & vbCrLf &
            '" WHERE CacheInvoicesDaily.ID = CacheInvoicesDaily2.ID" & vbCrLf &
            '" AND PaymentType='GoldCard'" & vbCrLf &
            ' ") AS GoldCard " & vbCrLf &
            '"OUTER APPLY (  " & vbCrLf &
            '"SELECT * FROM    " & vbCrLf &
            '"(SELECT PaymentType, Amount " & vbCrLf &
            '" FROM TransCustomerPaymentInvoices " & vbCrLf &
            '" INNER JOIN TransCustomerPaymentOther on  TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentOther.CustomerPaymentID" & vbCrLf &
            '" WHERE TransCustomerPaymentInvoices.InvoiceID = CacheInvoicesDaily.ID " & vbCrLf &
            '") t  " & vbCrLf &
            '"PIVOT(SUM(Amount) " & vbCrLf &
            '"FOR PaymentType in ( " & vbCrLf &
            '"[Discount Voucher / On Line Deals], [E-Plus], [FoodPanda], [GCash], [GrabFood], [SMAC], [V-Plus], [PayMaya], [ShopeeEVoucher], [ShopeePay], [GoldCard], [LocalDelivery])) as OtherPayment) as OtherPayment "

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & " " & vbCrLf &
                        "--AND MONTH(LibraryOrganizationFOWSTQuota.MonthYear) = Month(" & ReadyString(DateFrom) & ") " & vbCrLf &
                        "--AND YEAR(LibraryOrganizationFOWSTQuota.MonthYear) = YEAR(" & ReadyString(DateFrom) & ") " & vbCrLf

            If CStr2(Common.WebRequest.Data("StoreType")) <> "" Then
                .SqlWhere += " AND CacheInvoices.SubFOWSTTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreType"))) & vbCrLf
            End If

            If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
                .SqlWhere += " AND CacheInvoices.StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
            End If

            If Not FOWSTID = vbNullString Then
                .SqlWhere &= "AND CacheInvoices.FOWSTID = " & ReadyString(FOWSTID) & vbCrLf
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= "AND CacheInvoices.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "CacheInvoices.FOWSTName " & vbCrLf &
                        ", CacheInvoices.StoreType  " & vbCrLf &
                        ", CacheInvoices.StoreLocation " & vbCrLf &
                        ",CacheInvoices.GroupName " & vbCrLf &
                        ", LibraryOrganizationFOWSTQuota.Tier1Quota " & vbCrLf &
                        ",CacheInvoices.FowstID " & vbCrLf &
                        ",CacheInvoices.FOWSTCode "

            '.SqlGroup = " GROUP BY " & vbCrLf &
            '            "MobizLibraryOrganizationFOWST.Name " & vbCrLf &
            '            ", MobizLibraryFOWSTRetailStoreType.Name  " & vbCrLf &
            '            ", LibraryFranchiseGroup.GroupName " & vbCrLf &
            '            ", MobizLibraryFOWSTStoreLocationType.LocationType " & vbCrLf &
            '            ", LibraryOrganizationFOWSTQuota.Tier1Quota " & vbCrLf &
            '            ", MobizLibraryOrganizationFOWST.ID "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetSpecialPromoReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pSql As String

        pFinder = New Finder

        With pFinder

            .Distinct = True
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("Invoicedate", SqlDbType.UniqueIdentifier)
                    .Expression = "Invoicedate"
                End With

                With .Add("StoreName", SqlDbType.VarChar)
                    .Expression = "FowstName"
                End With

                With .Add("GroupName", SqlDbType.VarChar)
                    .Expression = "ISNULL(GroupName,'')"
                End With
                With .Add("Promo", SqlDbType.VarChar)
                    .Expression = "ProductName"
                End With
                With .Add("ProductID", SqlDbType.VarChar)
                    .Expression = "CacheInvoiceFinishedProducts.ProductID"
                End With
                With .Add("FowstID", SqlDbType.VarChar)
                    .Expression = "FowstID"
                End With
                'With .Add("ProductDescription", SqlDbType.VarChar)
                '    .Expression = "ModifierProductName"
                'End With
                'With .Add("UnitMeasure", SqlDbType.VarChar)
                '    .Expression = "ModifierUnitMeasure"
                'End With
                With .Add("Quantity", SqlDbType.VarChar)
                    .Expression = "SUM(ProductQuantity)"
                End With
                'With .Add("ItemModifierID", SqlDbType.VarChar)
                '    .Expression = "ItemModifierID"
                'End With
            End With


            .SqlFrom = "FROM CacheInvoiceFinishedProducts " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductVariableFeatures001 on MobizLibraryPIMProductVariableFeatures001.ProductID = CacheInvoiceFinishedProducts.ProductID " & vbCrLf

            .SqlWhere = " WHERE MobizLibraryPIMProductVariableFeatures001.C0020 like '%Promo%' " & vbCrLf &
                        " AND convert(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & " " & vbCrLf

            .SqlGroup = " GROUP by Invoicedate, " & vbCrLf &
                        "FowstName, " & vbCrLf &
                        "GroupName, " & vbCrLf &
                        "CacheInvoiceFinishedProducts.ProductID, " & vbCrLf &
                        "FowstID, " & vbCrLf &
                        "ProductName " & vbCrLf
            '"ItemModifierID " & vbCrLf

            '"ModifierProductName, " & vbCrLf &
            '"ModifierUnitMeasure, " & vbCrLf &

            If Not FOWSTID = vbNullString Then
                .SqlWhere &= " AND FOWSTID = " & ReadyString(FOWSTID) & vbCrLf
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= " AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            .SqlOrder = " ORDER BY InvoiceDate"

            pDT = .Requery

            If Common.WebRequest.Data("PromoType") <> "" Then
                'pSql = "SELECT * FROM (" & pSql & ") as T WHERE ItemModifierID = " & ReadyString(Common.WebRequest.Data("PromoType").ToString())
                pDT.DefaultView.RowFilter = "ItemModifierID = " & ReadyString(Common.WebRequest.Data("PromoType").ToString())

                pDT = pDT.DefaultView.ToTable
            End If

        End With

        Return pDT
    End Function

    Function GetSpecialPromoReportDetails() As DataTable
        Dim pSql As String

        pSql = " SELECT DISTINCT" & vbCrLf &
                " inv.InvoiceDate, " & vbCrLf &
                " inv.InvoiceNumber, " & vbCrLf &
                " fowst.Name as FowstName, " & vbCrLf &
                " ISNULL((SELECT OtherDescription FROM TransInvoiceItemsModifier LEFT JOIN LibraryItemModifiers as libmod ON libmod .ID =TransInvoiceItemsModifier.ItemModifierID WHERE InvoiceItemID = items.ID AND libmod.Type = 'Non-Inventory Affecting'), '') as EmployeeNoUsed, " & vbCrLf &
                " prod.ProductName, " & vbCrLf &
                " items.Quantity, " & vbCrLf &
                " items.SubTotal, " & vbCrLf &
                " CASE WHEN custpay.CashAmount > 0 THEN 'Cash' ELSE custpayother.PaymentType END as ModeOfPayment " & vbCrLf &
                " FROM TransInvoiceItems as items " & vbCrLf &
                " INNER JOIN TransInvoice as inv ON inv.ID = items.InvoiceID " & vbCrLf &
                " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = inv.FowstID " & vbCrLf &
                " INNER JOIN MobizLibraryPIMProduct as prod ON prod.ID = items.ProductID " & vbCrLf &
                " INNER JOIN TransCustomerPaymentInvoices as custinv ON custinv.InvoiceID = inv.ID " & vbCrLf &
                " INNER JOIN TransCustomerPayment as custpay ON custpay.ID = custinv.CustomerPaymentID " & vbCrLf &
                " LEFT JOIN TransCustomerPaymentOther as custpayother ON custpayother.CustomerPaymentID = custinv.CustomerPaymentID " & vbCrLf &
                " LEFT JOIN TransInvoiceItemsModifier as modifier ON modifier.InvoiceItemID = items.ID " & vbCrLf &
                " WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " AND items.ProductID = " & ReadyString(Common.WebRequest.Data("ProductID")) & vbCrLf &
                " AND inv.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                " ORDER BY fowst.Name, inv.InvoiceDate "

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetSpecialDiscountReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pSql As String

        pSql = "(SELECT DISTINCT TOP 100 PERCENT dbo.MobizLibraryOrganizationFOWST.ID AS [FOWSTID] " & vbCrLf &
                ", ISNULL(dbo.MobizLibraryOrganizationFOWST.Code,'-') AS [FOWSTCode] " & vbCrLf &
                ", dbo.MobizLibraryOrganizationFOWST.Name AS [FOWST] " & vbCrLf &
                ", storelocationtype.LocationType as StoreLocationType " & vbCrLf &
                ", storetype.Name as StoreType " & vbCrLf &
                ", ISNULL((SELECT GroupName FROM LibraryFranchiseGroup INNER JOIN MobizLibraryOrganizationFranchisee ON MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID WHERE MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID), 'Ungrouped') as GroupName " & vbCrLf &
                ", dbo.TransInvoiceItems.ID AS [InvoiceItemsID] " & vbCrLf &
                ", dbo.TransInvoice.ID AS [InvoiceID] " & vbCrLf &
                ", RIGHT('000000000000' + CONVERT(varchar, dbo.TransInvoice.InvoiceNumber), 12) AS [InvoiceNo] " & vbCrLf &
                ", dbo.TransInvoice.InvoiceDate AS [InvoiceDate] " & vbCrLf &
                ", ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'') AS [ProductDescription] " & vbCrLf &
                ", dbo.TransInvoiceItems.Quantity AS [Qty] " & vbCrLf &
                ", dbo.TransInvoiceItems.UnitMeasure AS [UnitMeasure] " & vbCrLf &
                ", dbo.TransInvoiceItems.UnitPrice AS [Price] " & vbCrLf &
                ", dbo.TransInvoiceItems.UnitPrice * dbo.TransInvoiceItems.Quantity AS [SubTotal] " & vbCrLf &
                ", TransInvoice.SCDiscount AS [Discount] " & vbCrLf &
                ", (CASE WHEN ISNULL(MobizLibraryOrganization.CompanyName,'')  = '' THEN ISNULL(MobizLibraryOrganization.PersonFullname, MobizLibraryOrganization.PersonSurname + ', ' + MobizLibraryOrganization.PersonFirstName) ELSE MobizLibraryOrganization.CompanyName END) AS [Customer] " & vbCrLf &
                ", MobizLibraryOrganization.PersonDateOfBirth AS [CustomerBirthday] " & vbCrLf &
                ", convert(nvarchar(50), TransInvoiceDiscount.DiscountTypeID) AS [DiscountTypeID] " & vbCrLf &
                ", TransInvoiceSeniorCitizenPWDCustomer.IDImage AS [IDImage] " & vbCrLf &
                " FROM dbo.TransInvoiceItems INNER JOIN  " & vbCrLf &
                "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN  " & vbCrLf &
                "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN  " & vbCrLf &
                "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID  " & vbCrLf &
                "LEFT JOIN MobizLibraryFOWSTStoreLocationType as storelocationtype ON storelocationtype.ID = MobizLibraryOrganizationFOWST.StoreLocationTypeID " & vbCrLf &
                "LEFT JOIN MobizLibraryFOWSTRetailStoreType as storetype ON storetype.ID = MobizLibraryOrganizationFOWST.SubFOWSTTypeID " & vbCrLf &
                "INNER JOIN TransInvoiceDiscount on TransInvoiceDiscount.InvoiceID = TransInvoice.ID  " & vbCrLf &
                "INNER JOIN TransInvoiceSeniorCitizenPWDCustomer on TransInvoiceSeniorCitizenPWDCustomer.InvoiceID = TransInvoice.ID  " & vbCrLf &
                "LEFT join MobizLibraryOrganization on MobizLibraryOrganization.ID = TransInvoiceSeniorCitizenPWDCustomer.CustomerID  " & vbCrLf &
                " outer apply (Select SUM(SCDiscount) SCDiscount from Transinvoice inv  " & vbCrLf &
                "        WHERE(inv.InvoiceNumber > 0 Or inv.FranchiseeInvoiceNumber > 0) " & vbCrLf &
                " AND CONVERT(date, dbo.TransInvoice.InvoiceDate) = CONVERT(date, inv.InvoiceDate)  " & vbCrLf &
                " AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & ") tblSCDiscount  " & vbCrLf &
                "        WHERE(dbo.TransInvoice.InvoiceNumber > 0 Or TransInvoice.FranchiseeInvoiceNumber > 0) " & vbCrLf &
                "  AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

        If FOWSTID <> "" Then
            pSql &= " AND dbo.TransInvoice.FowstID = " & ReadyString(FOWSTID) & vbCrLf
        Else
            pSql &= " AND dbo.TransInvoice.FowstID IN (SELECT FranchiseeOrgFowstID FROM MobizLibraryOrganizationFranchisee WHERE FranchiseGroupID = " & ReadyString(GroupID) & ")" & vbCrLf
        End If


        pSql &= "  --ORDER BY RIGHT('000000000000' + CONVERT(varchar, dbo.TransInvoice.InvoiceNumber), 12)  " & vbCrLf &
        "  --DESC, InvoiceDate DESC " & vbCrLf &
        "  ) " & vbCrLf

        'tanggal muna ang union for other discount para mabilis magload 6/29/2023

        '"        union " & vbCrLf &
        '"  (select dbo.MobizLibraryOrganizationFOWST.ID AS [FOWSTID],  " & vbCrLf &
        '"   dbo.MobizLibraryOrganizationFOWST.Name AS [FOWST], " & vbCrLf &
        '"   storelocationtype.LocationType as StoreLocationType, " & vbCrLf &
        '"   storetype.Name as StoreType, " & vbCrLf &
        '"   ISNULL((SELECT GroupName FROM LibraryFranchiseGroup INNER JOIN MobizLibraryOrganizationFranchisee ON MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID WHERE MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID), 'Ungrouped') as GroupName, " & vbCrLf &
        '"        item.ID InvoiceItemsID, " & vbCrLf &
        '"        inv.ID InvoiceID, " & vbCrLf &
        '"   RIGHT('000000000000' + CONVERT(varchar, inv.InvoiceNumber), 12) AS [InvoiceNo], " & vbCrLf &
        '"        inv.InvoiceDate, " & vbCrLf &
        '"        pim.ProductName ProductDescription, " & vbCrLf &
        '"        item.Quantity Qty, " & vbCrLf &
        '"        item.UnitMeasure, " & vbCrLf &
        '"        item.UnitPrice Price, " & vbCrLf &
        '"(item.SubTotal + (SELECT ISNULL(SUM(subitemmod.ModifierPrice), 0)  " & vbCrLf &
        '"	FROM TransInvoiceItemsModifier as subitemmod  " & vbCrLf &
        '"	WHERE subitemmod.InvoiceItemID = item.ID  " & vbCrLf &
        '"	AND subitemmod.ModifierPrice > 0)) as Subtotal, " & vbCrLf &
        '"(SELECT SUM(subitemmod.ModifierPrice) * -1 " & vbCrLf &
        '"	FROM TransInvoiceItemsModifier as subitemmod  " & vbCrLf &
        '"	WHERE subitemmod.InvoiceItemID = item.ID  " & vbCrLf &
        '"	AND subitemmod.ModifierPrice < 0) as Discount,	 " & vbCrLf &
        '"        '-' Customer, " & vbCrLf &
        '"        GETDATE() CustomerBirthday, " & vbCrLf &
        '"	(SELECT Name FROM LibraryItemModifiers WHERE ID = itemmod.ItemModifierID) as DiscountTypeID, " & vbCrLf &
        '"        '-' IDImage " & vbCrLf &
        '"from TransInvoice as inv " & vbCrLf &
        '"inner join TransInvoiceItems as item on item.InvoiceID = inv.ID " & vbCrLf &
        '"inner join TransInvoiceItemsModifier as itemmod on itemmod.InvoiceItemID = item.ID " & vbCrLf &
        '"inner join Mobiz.dbo.LibraryPIMProduct as pim ON pim.ID = item.ProductID " & vbCrLf &
        '"INNER JOIN MobizLibraryOrganizationFOWST ON inv.FowstID = dbo.MobizLibraryOrganizationFOWST.ID  " & vbCrLf &
        '"LEFT JOIN MobizLibraryFOWSTStoreLocationType as storelocationtype ON storelocationtype.ID = MobizLibraryOrganizationFOWST.StoreLocationTypeID " & vbCrLf &
        '"LEFT JOIN MobizLibraryFOWSTRetailStoreType as storetype ON storetype.ID = MobizLibraryOrganizationFOWST.SubFOWSTTypeID " & vbCrLf &
        '"WHERE CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

        'If FOWSTID <> "" Then
        '    pSql &= " AND inv.FowstID = " & ReadyString(FOWSTID) & vbCrLf
        'Else
        '    pSql &= " AND inv.FowstID IN (SELECT FranchiseeOrgFowstID FROM MobizLibraryOrganizationFranchisee WHERE FranchiseGroupID = " & ReadyString(GroupID) & ")" & vbCrLf
        'End If

        'pSql &= " AND itemmod.ModifierPrice < 0 " & vbCrLf &
        '"GROUP BY inv.ID, inv.InvoiceDate, " & vbCrLf &
        '"        inv.InvoiceNumber, " & vbCrLf &
        '"        pim.ProductName, " & vbCrLf &
        '"        item.SubTotal, " & vbCrLf &
        '"        item.ID, " & vbCrLf &
        '"        dbo.MobizLibraryOrganizationFOWST.ID, " & vbCrLf &
        '"        dbo.MobizLibraryOrganizationFOWST.Name, " & vbCrLf &
        '"        item.Quantity, " & vbCrLf &
        '"        item.UnitMeasure, " & vbCrLf &
        '"        itemmod.ItemModifierID, " & vbCrLf &
        '"        item.UnitPrice, " & vbCrLf &
        '"        storelocationtype.LocationType, " & vbCrLf &
        '"        storetype.Name " & vbCrLf &
        '"   )" & vbCrLf

        'pFinder = New Finder

        'With pFinder

        '    .Distinct = True
        '    .ProData = Common.WebRequest.ProData

        '    With .Fields
        '        With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
        '            .Searchable = False
        '            .Expression = "dbo.MobizLibraryOrganizationFOWST.ID"
        '        End With

        '        With .Add("FOWST", SqlDbType.VarChar)
        '            .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
        '        End With

        '        With .Add("InvoiceItemsID", SqlDbType.VarChar)
        '            .Expression = "dbo.TransInvoiceItems.ID"
        '        End With
        '        With .Add("InvoiceID", SqlDbType.VarChar)
        '            .Expression = "dbo.TransInvoice.ID"
        '        End With
        '        With .Add("InvoiceNo", SqlDbType.VarChar)
        '            .Expression = "RIGHT('000000000000' + CONVERT(varchar, dbo.TransInvoice.InvoiceNumber), 12)"
        '        End With
        '        With .Add("InvoiceDate", SqlDbType.VarChar)
        '            .Expression = "dbo.TransInvoice.InvoiceDate"
        '        End With
        '        With .Add("ProductDescription", SqlDbType.VarChar)
        '            .Expression = "ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'')"
        '        End With
        '        With .Add("Qty", SqlDbType.VarChar)
        '            .Expression = "dbo.TransInvoiceItems.Quantity"
        '        End With
        '        With .Add("UnitMeasure", SqlDbType.VarChar)
        '            .Expression = "dbo.TransInvoiceItems.UnitMeasure"
        '        End With
        '        With .Add("Price", SqlDbType.VarChar)
        '            .Expression = "dbo.TransInvoiceItems.UnitPrice"
        '        End With
        '        With .Add("SubTotal", SqlDbType.VarChar)
        '            '.Expression = "dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions"
        '            'change to this expression because GrossSalesNetOfVATExemptions is computed together with item modifiers which will make computation for line items wrong
        '            .Expression = "dbo.TransInvoiceItems.UnitPrice * dbo.TransInvoiceItems.Quantity"
        '        End With
        '        With .Add("Discount", SqlDbType.VarChar)
        '            .Expression = "dbo.TransInvoiceItems.DiscountAmount + TransInvoice.SCDiscount + TransInvoice.ForegoneVAT "
        '        End With

        '        With .Add("Customer", SqlDbType.VarChar)
        '            .Expression = "(CASE WHEN MobizLibraryOrganization.CompanyName  = '' THEN ISNULL(MobizLibraryOrganization.PersonFullname, MobizLibraryOrganization.PersonSurname + ', ' + MobizLibraryOrganization.PersonFirstName) ELSE MobizLibraryOrganization.CompanyName END)"
        '        End With

        '        With .Add("CustomerBirthday", SqlDbType.VarChar)
        '            .Expression = "MobizLibraryOrganization.PersonDateOfBirth"
        '        End With

        '        With .Add("DiscountTypeID", SqlDbType.VarChar)
        '            .Expression = "TransInvoiceDiscount.DiscountTypeID"
        '        End With

        '        With .Add("IDImage", SqlDbType.VarChar)
        '            .Expression = "TransInvoiceSeniorCitizenPWDCustomer.IDImage"
        '        End With

        '    End With


        '    .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
        '              "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
        '              "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN " & vbCrLf &
        '              "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID " & vbCrLf &
        '              "INNER JOIN TransInvoiceDiscount on TransInvoiceDiscount.InvoiceID = TransInvoice.ID " & vbCrLf &
        '              "INNER JOIN TransInvoiceSeniorCitizenPWDCustomer on TransInvoiceSeniorCitizenPWDCustomer.InvoiceID = TransInvoice.ID " & vbCrLf &
        '              "LEFT join MobizLibraryOrganization on MobizLibraryOrganization.ID = TransInvoiceSeniorCitizenPWDCustomer.CustomerID " & vbCrLf &
        '              " outer apply (Select SUM(SCDiscount) SCDiscount from Transinvoice inv " & vbCrLf &
        '              " WHERE (inv.InvoiceNumber > 0 OR inv.FranchiseeInvoiceNumber > 0) " & vbCrLf &
        '              " AND CONVERT(date, dbo.TransInvoice.InvoiceDate) = CONVERT(date, inv.InvoiceDate) " & vbCrLf &
        '              " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & ") tblSCDiscount " & vbCrLf


        '    SqlWhere = " WHERE (dbo.TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0) AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

        '    strWhereFowst = " AND (dbo.TransInvoice.FowstID = " & ReadyString(FOWSTID)

        '    strWhereFowst &= ")"

        '    SqlWhere &= strWhereFowst

        '    .SqlWhere = SqlWhere

        '    .SqlOrder = "ORDER BY RIGHT('000000000000' + CONVERT(varchar, dbo.TransInvoice.InvoiceNumber), 12) DESC, InvoiceDate DESC"

        'pDT = .Requery

        'End With

        If Common.WebRequest.Data("DiscountType") <> "" Then
            pSql = "SELECT * FROM (" & pSql & ") as T WHERE DiscountTypeID = " & ReadyString(Common.WebRequest.Data("DiscountType").ToString())
        End If

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetConsolidatedSCDReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable

        pFinder = New Finder

        With pFinder

            .Distinct = True
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("StoreName", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With
                With .Add("InvoiceDate", SqlDbType.VarChar)
                    .Expression = "convert(date,dbo.TransInvoice.InvoiceDate)"
                End With
                With .Add("Category", SqlDbType.VarChar)
                    .Expression = "cat.Category"
                End With
                With .Add("ProductCode", SqlDbType.VarChar)
                    .Expression = "ISNULL(MobizLibraryPIMProduct.ProductNo,'') "
                End With
                With .Add("ProductDescription", SqlDbType.VarChar)
                    .Expression = "ISNULL(MobizLibraryPIMProduct.ProductName,'') "
                End With
                With .Add("Qty", SqlDbType.VarChar)
                    .Expression = "COUNT(TransInvoiceItems.ProductID)"
                End With
            End With

            .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN   " & vbCrLf &
                        "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN   " & vbCrLf &
                        "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID   " & vbCrLf &
                        "INNER JOIN TransInvoiceSeniorCitizenPWDCustomer on TransInvoiceSeniorCitizenPWDCustomer.InvoiceID = TransInvoice.ID   " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID  " & vbCrLf &
                        "INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf &
                        "OUTER APPLY(" & vbCrLf &
                        " SELECT DISTINCT MobizLibraryPIMProductVariableFeatures001.C0020 AS [Category]" & vbCrLf &
                        " FROM MobizLibraryPIMProductVariableFeatures001" & vbCrLf &
                        " WHERE MobizLibraryPIMProductVariableFeatures001.ProductID = TransInvoiceItems.ProductID " & vbCrLf &
                        ") cat"

            .SqlWhere = " WHERE(dbo.TransInvoice.InvoiceNumber > 0 Or TransInvoice.FranchiseeInvoiceNumber > 0) " & vbCrLf &
                          "AND CONVERT(date, dbo.TransInvoice.InvoiceDate) between " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                          "AND TransInvoice.SCDiscount>0 " & vbCrLf

            If Not String.IsNullOrWhiteSpace(GroupID) Then
                .SqlFrom += "INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID  " & vbCrLf &
                            "INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf

                .SqlWhere += "AND LibraryFranchiseGroup.ID IN (" & GroupID & ")" & vbCrLf
            Else
                .SqlWhere += "AND MobizLibraryOrganizationFOWST.ID IN (" & FOWSTID & ")" & vbCrLf
            End If

            .SqlGroup = "GROUP BY dbo.MobizLibraryOrganizationFOWST.Name, convert(date,dbo.TransInvoice.InvoiceDate), MobizLibraryPIMProduct.ProductGroupNo, MobizLibraryPIMProduct.ProductNo, MobizLibraryPIMProduct.ProductName, Category "

            .SqlOrder &= " ORDER BY convert(date,TransInvoice.InvoiceDate) ASC "

            pDT = .Requery

            pFilter1 = CreateCrossTab(pDT, {"StoreName", "Category", "ProductCode", "ProductDescription"}, {New CrossColumn("Qty {InvoiceDate}", "Qty", "Qty")})

            pFilter1.Columns.Remove("Qty")

        End With


        'Sort Columns

        Dim tempDatatable As New DataTable
        Dim mHeaders As New List(Of Date)


        For i As Integer = 0 To pFilter1.Columns.Count - 1
            If IsDate(Trim(CStr2(pFilter1.Columns(i).ColumnName).Replace("Qty", ""))) Then
                mHeaders.Add(CDate(Trim(CStr2(pFilter1.Columns(i).ColumnName).Replace("Qty", ""))))
            End If
        Next

        mHeaders.Sort()

        tempDatatable.Columns.Add("StoreName")
        tempDatatable.Columns.Add("Category")
        tempDatatable.Columns.Add("ProductCode")
        tempDatatable.Columns.Add("ProductDescription")

        For i As Integer = 0 To mHeaders.Count - 1
            tempDatatable.Columns.Add(mHeaders(i))
        Next

        For i As Integer = 0 To pFilter1.Rows.Count - 1
            Dim dr As DataRow

            dr = tempDatatable.NewRow

            dr(0) = pFilter1.Rows(i).Item("StoreName")
            dr(1) = pFilter1.Rows(i).Item("Category")
            dr(2) = pFilter1.Rows(i).Item("ProductCode")
            dr(3) = pFilter1.Rows(i).Item("ProductDescription")

            For j As Integer = 4 To tempDatatable.Columns.Count - 1
                dr(j) = pFilter1.Rows(i).Item("Qty " & tempDatatable.Columns(j).ColumnName)
            Next

            tempDatatable.Rows.Add(dr)
        Next

        Return tempDatatable

    End Function

    Function GetCashHandlingReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1

        pFinder = New Finder

        With pFinder

            .Distinct = True
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                End With
                With .Add("StoreType", SqlDbType.VarChar)
                    .Expression = "ISNULL(MobizLibraryFOWSTRetailStoreType.Name,'')"
                End With
                With .Add("TransactionDateAndTimeFrom", SqlDbType.VarChar)
                    .Expression = "ISNULL(TransactionDateAndTimeFrom,'')"
                End With
                With .Add("DepositedByName", SqlDbType.VarChar)
                    .Expression = "ISNULL(DepositedByName,'')"
                End With
                With .Add("BankBranch", SqlDbType.VarChar)
                    .Expression = "ISNULL(BankBranch,'')"
                End With
                With .Add("ServiceCharge", SqlDbType.VarChar)
                    .Expression = "ISNULL(ServiceCharge,'')"
                End With
                With .Add("DepositReferenceNo", SqlDbType.VarChar)
                    .Expression = "ISNULL(DepositReferenceNo,'')"
                End With
                With .Add("TotalAmount", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalAmount,'')"
                End With
                With .Add("TotalCash", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalCash,'')"
                End With
                With .Add("TotalDebitCard", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalDebitCard,'')"
                End With
                With .Add("TotalCreditCard", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalCreditCard,'')"
                End With
                With .Add("TotalCheck", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalCheck,0)"
                End With
                With .Add("TotalAlipay", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalAlipay,0)"
                End With
                With .Add("TotalDiscountVoucher", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalDiscountVoucher,0)"
                End With
                With .Add("TotalEPlus", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalEPlus,0)"
                End With
                With .Add("TotalFoodPanda", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalFoodPanda,0)"
                End With
                With .Add("TotalGCash", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalGCash,0)"
                End With
                With .Add("TotalGoldCard", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalGoldCard,0)"
                End With
                With .Add("TotalGrabFood", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalGrabFood,0)"
                End With
                With .Add("TotalLocalDelivery", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalLocalDelivery,0)"
                End With
                With .Add("TotalPayMaya", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalPayMaya,0)"
                End With
                With .Add("TotalShopeeEVoucher", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalShopeeEVoucher,0)"
                End With
                With .Add("TotalShopeePay", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalShopeePay,0)"
                End With
                With .Add("TotalSMAC", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalSMAC,0)"
                End With
                With .Add("TotalVPlus", SqlDbType.VarChar)
                    .Expression = "ISNULL(TotalVPlus,0)"
                End With

            End With


            .SqlFrom = "FROM TransCashHandling " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = TransCashHandling.FOWSTID " & vbCrLf &
                        "LEFT JOIN MobizLibraryFOWSTRetailStoreType on MobizLibraryFOWSTRetailStoreType.ID = MobizLibraryOrganizationFOWST.SubFOWSTTypeID " & vbCrLf &
                        "LEFT JOIN MobizLibraryFOWSTStoreLocationType on MobizLibraryFOWSTStoreLocationType.ID = MobizLibraryOrganizationFOWST.StoreLocationTypeID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                        "INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf &
                        " where TransactionDateAndTimeFrom BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            If Not GroupID = vbNullString Then
                .SqlWhere &= " AND MobizLibraryOrganizationFranchisee.FranchiseGroupID = " & ReadyString(GroupID) & vbCrLf
            End If

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetSalesByProduct() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1

        pFinder = New Finder

        With pFinder

            .Distinct = True
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ProductDescription", SqlDbType.VarChar)
                    .Expression = "ProductName"
                End With
                With .Add("Qty", SqlDbType.VarChar)
                    .Expression = "SUM(ProductQuantity)"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "'-'"
                End With
                With .Add("SubTotal", SqlDbType.VarChar)
                    .Expression = "SUM(SalesValue)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoiceFinishedProducts " & vbCrLf

            .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf


            .SqlGroup = "GROUP BY ProductName "

            If Not GroupID = vbNullString Then

                .SqlWhere &= " AND FranchiseGroupID = " & ReadyString(GroupID) & vbCrLf

            Else

                .SqlWhere &= " AND FowstID = " & ReadyString(FOWSTID) & vbCrLf

            End If

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetSalesByProductFranchisee() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1

        pFinder = New Finder

        With pFinder

            .Distinct = True
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ProductDescription", SqlDbType.VarChar)
                    .Expression = "ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'')"
                End With
                With .Add("Qty", SqlDbType.VarChar)
                    .Expression = "SUM(dbo.TransInvoiceItems.Quantity)"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "dbo.TransInvoiceItems.UnitMeasure"
                End With
                'With .Add("Price", SqlDbType.VarChar)
                '    .Expression = "dbo.TransInvoiceItems.UnitPrice"
                'End With
                With .Add("SubTotal", SqlDbType.VarChar)
                    .Expression = "SUM(dbo.TransInvoiceItems.UnitPrice * dbo.TransInvoiceItems.Quantity) + ISNULL((SUM(modifier.Subtotal)),0) "
                End With
            End With

            .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
                        "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN  " & vbCrLf &
                        "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID  " & vbCrLf &
                        "outer apply(" & vbCrLf &
                        "SELECT SUM(TransInvoiceItemsModifier.SubTotal) SubTotal from TransInvoiceItemsModifier WHERE TransInvoiceItemsModifier.InvoiceItemID  = TransInvoiceItems.ID" & vbCrLf &
                        ") modifier "

            .SqlWhere = " WHERE (dbo.TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0) " & vbCrLf &
                         "AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf


            .SqlGroup = "GROUP BY dbo.TransInvoiceItems.UnitMeasure, ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'') "

            If Not GroupID = vbNullString Then
                .SqlFrom &= "INNER JOIN MobizLibraryOrganizationFranchisee on franchiseeorgfowstid = TransInvoice.FowstID " & vbCrLf

                .SqlWhere &= " AND MobizLibraryOrganizationFranchisee.FranchiseGroupID = " & ReadyString(GroupID) & vbCrLf
            Else

                .SqlWhere &= "AND dbo.TransInvoice.FowstID = " & ReadyString(FOWSTID) & vbCrLf

            End If

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetCombosProductMix() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            With .Fields

                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "ProductName"
                End With
                With .Add("TotalSales", SqlDbType.VarChar)
                    .Expression = "SUM(TotalSales)"
                End With

                With .Add("TransactionCount", SqlDbType.VarChar)
                    .Expression = "SUM(TransactionCount)"
                End With

                With .Add("ProductCount", SqlDbType.VarChar)
                    .Expression = "SUM(ProductCount)"
                End With

                With .Add("StoreCount", SqlDbType.VarChar)
                    .Expression = "Count(distinct FOWSTID)"
                End With

                'With .Add("ProductName", SqlDbType.VarChar)
                '    .Expression = "ISNULL(mp2.ProductName,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'')  "
                'End With
                'With .Add("TotalSales", SqlDbType.VarChar)
                '    .Expression = "SUM(ISNULL(dbo.TransInvoiceItems.SubTotal,0) + ISNULL(TransInvoiceItemsModifier .PremiumPrice,0) + ISNULL(TransInvoiceItemsModifier .ModifierPrice,0) ) "
                'End With

                'With .Add("TransactionCount", SqlDbType.VarChar)
                '    .Expression = "Count(distinct CacheInvoices.ID)"
                'End With

                'With .Add("ProductCount", SqlDbType.VarChar)
                '    .Expression = "ISNULL(SUM(dbo.TransInvoiceItems.Quantity),0)"
                'End With

                'With .Add("StoreCount", SqlDbType.VarChar)
                '    .Expression = "Count(distinct CacheInvoices.FOWSTID) "
                'End With

                'With .Add("ADQ", SqlDbType.VarChar)
                '    .Expression = "SUM(OrderedQuantity) /7/count(distinct fowstid)"
                'End With
            End With

            .SqlFrom = "FROM CacheInvoiceComboMix " & vbCrLf
            '"INNER JOIN TransInvoiceItems on TransInvoiceItems.InvoiceID = CacheInvoices.ID  " & vbCrLf &
            '"INNER JOIN TransInvoiceItemsModifier on TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID  " & vbCrLf &
            '"INNER JOIN dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID   " & vbCrLf &
            '"INNER JOIN LibraryItemModifierInventoryAffecting on LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID  " & vbCrLf &
            '"INNER JOIN dbo.MobizLibraryPIMProduct mp2 ON dbo.LibraryItemModifierInventoryAffecting.ProductID = mp2.ID   " & vbCrLf

            '.SqlWhere = " WHERE LibraryItemModifierInventoryAffecting.PremiumPrice=0 and TransInvoiceItemsModifier.SubTotal>0 AND CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
            '            " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'"

            .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991' " & vbCrLf
            If String.IsNullOrWhiteSpace(FOWSTID) = False Then
                .SqlWhere &= " AND FowstID = " & ReadyString(FOWSTID) & vbCrLf
            End If

            .SqlGroup = "GROUP BY ProductName " & vbCrLf

            .SqlOrder = "ORDER BY ProductName "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProductMix() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "ProductName"
                End With
                With .Add("TotalSales", SqlDbType.VarChar)
                    .Expression = "SUM(ProductAmount)"
                End With

                With .Add("TransactionCount", SqlDbType.VarChar)
                    .Expression = "SUM(InvoiceCount)"
                End With

                With .Add("ProductCount", SqlDbType.VarChar)
                    .Expression = "SUM(ProductQuantity)"
                End With
                
                With .Add("StoreCount", SqlDbType.VarChar)
                    .Expression = "count(distinct fowstid) "
                End With

                'With .Add("ADQ", SqlDbType.VarChar)
                '    .Expression = "SUM(OrderedQuantity) /7/count(distinct fowstid)"
                'End With
            End With

            .SqlFrom = "FROM CacheInvoiceFinishedProducts " & vbCrLf

            .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            If CStr2(Common.WebRequest.Data("OrgID")).ToLower = "d362ffdb-15d1-451f-97dc-c567b94fb410" Then
                .SqlWhere &= " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'" & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(FOWSTID) = False Then
                .SqlWhere &= " AND FowstID = " & ReadyString(FOWSTID) & vbCrLf
            End If
            .SqlGroup = "GROUP BY ProductName "

            'Combo Mix
            .SqlGroup &= " UNION " & vbCrLf &
                         " SELECT TOP 100 PERCENT ProductName AS [ProductName] " & vbCrLf &
                        ", SUM(TotalSales) AS [TotalSales] " & vbCrLf &
                        ", SUM(TransactionCount) AS [TransactionCount] " & vbCrLf &
                        ", SUM(ProductCount) AS [ProductCount] " & vbCrLf &
                        ", count(distinct fowstid)  AS [StoreCount] " & vbCrLf &
                        "            FROM CacheInvoiceComboMix " & vbCrLf &
                        "  WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf
            If String.IsNullOrWhiteSpace(FOWSTID) = False Then
                .SqlGroup &= " AND FowstID = " & ReadyString(FOWSTID) & vbCrLf
            End If
            .SqlGroup &= " GROUP BY ProductName   "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProductMixPerWeek() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable
        Dim pSql As String

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "ProductName"
                End With
                With .Add("TotalSales", SqlDbType.VarChar)
                    .Expression = "SUM(ProductAmount)"
                End With

                With .Add("TransactionCount", SqlDbType.VarChar)
                    .Expression = "SUM(InvoiceCount)"
                End With

                With .Add("ProductCount", SqlDbType.VarChar)
                    .Expression = "SUM(ProductQuantity)"
                End With

                With .Add("StoreCount", SqlDbType.VarChar)
                    .Expression = "count(distinct fowstid) "
                End With

                With .Add("WeekName", SqlDbType.VarChar)
                    .Expression = "'WeekDay'"
                End With
            End With

            .SqlFrom = "FROM CacheInvoiceFinishedProducts " & vbCrLf

            .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND DATEPART(weekday, CacheInvoiceFinishedProducts.InvoiceDate) between 2 and 5  " & vbCrLf

            If CStr2(Common.WebRequest.Data("OrgID")).ToLower = "d362ffdb-15d1-451f-97dc-c567b94fb410" Then
                .SqlWhere &= " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'" & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(FOWSTID) = False Then
                .SqlWhere &= " AND FowstID = " & ReadyString(FOWSTID) & vbCrLf
            End If
            .SqlGroup = "GROUP BY ProductName "

            'Combo Mix
            .SqlGroup &= " UNION " & vbCrLf &
                         " SELECT TOP 100 PERCENT ProductName AS [ProductName] " & vbCrLf &
                        ", SUM(TotalSales) AS [TotalSales] " & vbCrLf &
                        ", SUM(TransactionCount) AS [TransactionCount] " & vbCrLf &
                        ", SUM(ProductCount) AS [ProductCount] " & vbCrLf &
                        ", count(distinct fowstid)  AS [StoreCount] " & vbCrLf &
                        ", 'WeekDay' AS WeekName " & vbCrLf &
                        "            FROM CacheInvoiceComboMix " & vbCrLf &
                        "  WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf
            If String.IsNullOrWhiteSpace(FOWSTID) = False Then
                .SqlGroup &= " AND FowstID = " & ReadyString(FOWSTID) & vbCrLf
            End If
            .SqlGroup &= " AND DATEPART(weekday, CacheInvoiceComboMix.InvoiceDate) between 2 and 5 " & vbCrLf &
                        " GROUP BY ProductName   "

            pSql = .CreateSql

            'pDT = .Requery

        End With

        pSql &= " UNION "

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "ProductName"
                End With
                With .Add("TotalSales", SqlDbType.VarChar)
                    .Expression = "SUM(ProductAmount)"
                End With

                With .Add("TransactionCount", SqlDbType.VarChar)
                    .Expression = "SUM(InvoiceCount)"
                End With

                With .Add("ProductCount", SqlDbType.VarChar)
                    .Expression = "SUM(ProductQuantity)"
                End With

                With .Add("StoreCount", SqlDbType.VarChar)
                    .Expression = "count(distinct fowstid) "
                End With

                With .Add("WeekName", SqlDbType.VarChar)
                    .Expression = "'Weekend'"
                End With
            End With

            .SqlFrom = "FROM CacheInvoiceFinishedProducts " & vbCrLf

            .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND (DATEPART(weekday, CacheInvoiceFinishedProducts.InvoiceDate) = 1 OR DATEPART(weekday, CacheInvoiceFinishedProducts.InvoiceDate) = 6 OR DATEPART(weekday, CacheInvoiceFinishedProducts.InvoiceDate) = 7)" & vbCrLf

            If CStr2(Common.WebRequest.Data("OrgID")).ToLower = "d362ffdb-15d1-451f-97dc-c567b94fb410" Then
                .SqlWhere &= " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'" & vbCrLf
            End If
            If String.IsNullOrWhiteSpace(FOWSTID) = False Then
                .SqlWhere &= " AND FowstID = " & ReadyString(FOWSTID) & vbCrLf
            End If
            .SqlGroup = "GROUP BY ProductName "

            'Combo Mix
            .SqlGroup &= " UNION " & vbCrLf &
                         " SELECT TOP 100 PERCENT ProductName AS [ProductName] " & vbCrLf &
                        ", SUM(TotalSales) AS [TotalSales] " & vbCrLf &
                        ", SUM(TransactionCount) AS [TransactionCount] " & vbCrLf &
                        ", SUM(ProductCount) AS [ProductCount] " & vbCrLf &
                        ", count(distinct fowstid)  AS [StoreCount] " & vbCrLf &
                        ", 'Weekend' AS WeekName " & vbCrLf &
                        "            FROM CacheInvoiceComboMix " & vbCrLf &
                        "  WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf
            If String.IsNullOrWhiteSpace(FOWSTID) = False Then
                .SqlGroup &= " AND FowstID = " & ReadyString(FOWSTID) & vbCrLf
            End If
            .SqlGroup &= " AND (DATEPART(weekday, CacheInvoiceComboMix.InvoiceDate) = 1 OR DATEPART(weekday, CacheInvoiceComboMix.InvoiceDate) = 6 OR DATEPART(weekday, CacheInvoiceComboMix.InvoiceDate) = 7)" & vbCrLf &
                        " GROUP BY ProductName   "

            pSql &= .CreateSql

        End With

        pDT = Common.WebRequest.ProData.Execute(pSql)

        '        "   AND DATEPART(weekday, Cacheinvoices_2.InvoiceDate) between 2 and 5 " & vbCrLf &

        'and (DATEPART(weekday, Cacheinvoices_3.InvoiceDate) = 1 OR DATEPART(weekday, Cacheinvoices_3.InvoiceDate) = 6 OR DATEPART(weekday, Cacheinvoices_3.InvoiceDate) = 7)" & vbCrLf &
        '                       "    ) 
        Return pDT
    End Function

    Function GetFlavorMix() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("Flavor", SqlDbType.VarChar)
                    .Expression = "ModifierProductName"
                End With

                With .Add("TotalSales", SqlDbType.VarChar)
                    .Expression = "SUM(SalesValue)"
                End With

                'With .Add("TotalSales", SqlDbType.VarChar)
                '    .Expression = "(CASE WHEN SUM(ISNULL(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions, 0)) = 0 THEN SUM(dbo.TransInvoiceItems.SubTotal) ELSE SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions) END)"
                'End With

                With .Add("TransactionCount", SqlDbType.VarChar)
                    .Expression = "SUM(InvoiceCount)"
                End With

                With .Add("ProductCount", SqlDbType.VarChar)
                    .Expression = "SUM(ModifierQuantity)"
                End With

                'With .Add("ProductCount", SqlDbType.VarChar)
                '    .Expression = "SUM(dbo.TransInvoiceItems.OrderedQuantity)"
                'End With

                With .Add("StoreCount", SqlDbType.VarChar)
                    .Expression = "count(distinct fowstid) "
                End With

                'With .Add("ADQ", SqlDbType.VarChar)
                '    .Expression = "SUM(ModifierQuantity) /7/count(distinct fowstid)"
                'End With
            End With

            .SqlFrom = "FROM CacheInvoiceFlavors " & vbCrLf

            .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'" & vbCrLf
            If String.IsNullOrWhiteSpace(FOWSTID) = False Then
                .SqlWhere &= " AND FowstID = " & ReadyString(FOWSTID) & vbCrLf
            End If

            .SqlGroup = "GROUP BY ModifierProductName "

            pDT = .Requery

        End With

        'With pFinder

        '    .ProData = Common.WebRequest.ProData

        '    With .Fields
        '        With .Add("Flavor", SqlDbType.VarChar)
        '            .Expression = "ISNULL(TransInvoiceItemsModifier.Flavor, 'N/A')"
        '        End With
        '        With .Add("TotalSales", SqlDbType.VarChar)
        '            .Expression = "(CASE WHEN SUM(ISNULL(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions, 0)) = 0 THEN SUM(dbo.TransInvoiceItems.SubTotal) ELSE SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions) END)"
        '        End With

        '        With .Add("TransactionCount", SqlDbType.VarChar)
        '            .Expression = "COUNT(dbo.TransInvoice.ID)"
        '        End With

        '        With .Add("ProductCount", SqlDbType.VarChar)
        '            .Expression = "SUM(dbo.TransInvoiceItems.OrderedQuantity)"
        '        End With

        '        With .Add("StoreCount", SqlDbType.VarChar)
        '            .Expression = "count(distinct fowstid) "
        '        End With

        '        With .Add("ADQ", SqlDbType.VarChar)
        '            .Expression = "SUM(OrderedQuantity) /7/count(distinct fowstid)"
        '        End With
        '    End With

        '    .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
        '                "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID   " & vbCrLf &
        '                "INNER JOIN TransInvoiceItemsModifier on TransInvoiceItemsModifier.InvoiceItemID = TransInvoiceItems.ID " & vbCrLf

        '    .SqlWhere = " WHERE (dbo.TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0) " & vbCrLf &
        '                 " AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf


        '    .SqlGroup = "GROUP BY ISNULL(TransInvoiceItemsModifier.Flavor, 'N/A') "

        '    pDT = .Requery

        '    If pDT.Columns.Contains("Flavor") Then
        '        Dim isRepeat As Boolean

        '        While True

        '            For i As Integer = 0 To pDT.Rows.Count - 1
        '                If CStr2(pDT.Rows(i).Item("Flavor")) = "N/A" Then
        '                    pDT.Rows.Remove(pDT.Rows(i))
        '                    isRepeat = True
        '                    Exit For
        '                End If
        '            Next

        '            If Not isRepeat Then Exit While

        '            isRepeat = False

        '        End While

        '    End If

        'End With

        Return pDT
    End Function

    Function ProductPerformanceTracker() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable

        pFinder = New Finder

        If Not CBool2(Common.WebRequest.Data("ByProduct")) Then

            With pFinder

                .Distinct = True
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("StoreCode", SqlDbType.VarChar)
                        .Expression = "ISNULL(FowstCode,'-')"
                    End With
                    With .Add("StoreName", SqlDbType.VarChar)
                        .Expression = "FowstName"
                    End With

#If DEBUG Then
                    With .Add("TransactionDate", SqlDbType.Bit)
                        .Expression = "InvoiceDate"
                    End With
#Else
                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "FORMAT(InvoiceDate, 'dd/MMM')"
                End With
#End If
                    With .Add("Qty", SqlDbType.VarChar)
                        .Expression = "ModifierQuantity"
                    End With

                    With .Add("ProductDescription", SqlDbType.VarChar)
                        .Expression = "ModifierProductName"
                    End With

                    With .Add("TransactionCount", SqlDbType.VarChar)
                        .Expression = "InvoiceCount"
                    End With
                    With .Add("SubTotal", SqlDbType.VarChar)
                        .Expression = "SalesValue"
                    End With
                End With

                .SqlFrom = "FROM CacheInvoiceFlavors " & vbCrLf

                .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                            " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'"

                .SqlOrder = " ORDER BY TransactionDate"

                pDT = .Requery
            End With

        Else
            'products

            With pFinder

                .Distinct = True
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("StoreCode", SqlDbType.VarChar)
                        .Expression = "ISNULL(FowstCode,'-')"
                    End With
                    With .Add("StoreName", SqlDbType.VarChar)
                        .Expression = "FowstName"
                    End With

#If DEBUG Then
                    With .Add("TransactionDate", SqlDbType.Bit)
                        .Expression = "InvoiceDate"
                    End With
#Else
                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "FORMAT(InvoiceDate, 'dd/MMM')"
                End With
#End If
                    With .Add("Qty", SqlDbType.VarChar)
                        .Expression = "ProductQuantity"
                    End With

                    With .Add("ProductDescription", SqlDbType.VarChar)
                        .Expression = "ProductName"
                    End With

                    With .Add("TransactionCount", SqlDbType.VarChar)
                        .Expression = "InvoiceCount"
                    End With
                    With .Add("SubTotal", SqlDbType.VarChar)
                        .Expression = "SalesValue"
                    End With
                End With

                .SqlFrom = "FROM CacheInvoiceFinishedProducts " & vbCrLf

                .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                            " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'"

                .SqlOrder = " ORDER BY TransactionDate"

                pDT = .Requery
            End With

        End If

        Dim pSourceURL As String = Common.ServerPath & "temp\ProductPerformanceTracker"
        Dim strFileName = "ProductPerformanceTracker_" & DateFrom & " - " & DateTo & ".xlsx"

        If Not System.IO.Directory.Exists(pSourceURL) Then
            IO.Directory.CreateDirectory(pSourceURL)
        End If

        TableToExcelFile(pDT, "", "", pSourceURL & "\" & strFileName)

        'With pFinder

        '    .Distinct = True
        '    .ProData = Common.WebRequest.ProData

        '    With .Fields
        '        With .Add("StoreCode", SqlDbType.VarChar)
        '            .Expression = "ISNULL(MobizLibraryOrganizationFOWST.Code,'-')"
        '        End With
        '        With .Add("StoreName", SqlDbType.VarChar)
        '            .Expression = "MobizLibraryOrganizationFOWST.Name"
        '        End With

        '        With .Add("TransactionDate", SqlDbType.VarChar)
        '            .Expression = "convert(date,Transinvoice.InvoiceDate)"
        '        End With

        '        If Not CBool2(Common.WebRequest.Data("ByProduct")) Then
        '            With .Add("ProductDescription", SqlDbType.VarChar)
        '                .Expression = "ISNULL(TransInvoiceItemsModifier.Flavor,'N/A')"
        '            End With
        '        Else
        '            With .Add("ProductDescription", SqlDbType.VarChar)
        '                .Expression = "ISNULL(MobizLibraryPIMProduct.ProductName,'N/A')"
        '            End With
        '        End If

        '        With .Add("Qty", SqlDbType.VarChar)
        '            .Expression = "SUM(dbo.TransInvoiceItems.OrderedQuantity)"
        '        End With
        '        'With .Add("UnitMeasure", SqlDbType.VarChar)
        '        '    .Expression = "dbo.TransInvoiceItems.UnitMeasure"
        '        'End With
        '        'With .Add("Price", SqlDbType.VarChar)
        '        '    .Expression = "dbo.TransInvoiceItems.UnitPrice"
        '        'End With
        '        With .Add("TransactionCount", SqlDbType.VarChar)
        '            .Expression = "COUNT(distinct dbo.TransInvoice.ID)"
        '        End With
        '        With .Add("SubTotal", SqlDbType.VarChar)
        '            .Expression = "SUM(dbo.TransInvoiceItems.UnitPrice * dbo.TransInvoiceItems.OrderedQuantity)"
        '        End With
        '    End With

        '    .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
        '                "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN  " & vbCrLf &
        '                "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID   " & vbCrLf &
        '                " INNER JOIN MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID "

        '    If Not CBool2(Common.WebRequest.Data("ByProduct")) Then

        '        .SqlFrom &= " INNER JOIN TransInvoiceItemsModifier on TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID  " & vbCrLf &
        '                " INNER JOIN LibraryItemModifierInventoryAffecting on LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID  " & vbCrLf &
        '                " INNER JOIN MobizLibraryPIMProductVariableFeatures001 on MobizLibraryPIMProductVariableFeatures001.ProductID = LibraryItemModifierInventoryAffecting.ProductID  " & vbCrLf
        '    Else
        '        .SqlFrom &= " INNER JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf
        '        '" INNER JOIN MobizLibraryPIMProductContents ON MobizLibraryPIMProductContents.ParentProductID = LibraryOrganizationPIM.ProductID "
        '    End If


        '    .SqlWhere = " WHERE (dbo.TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0) " & vbCrLf &
        '                 "AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

        '    If CStr2(Common.WebRequest.Data("StoreType")) = "Franchisee" Then
        '        .SqlFrom &= " AND MobizLibraryOrganizationFOWST.OrganizationID = '892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba' " & vbCrLf 'hard coded lang muna
        '    Else
        '        .SqlFrom &= " AND MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID")) & vbCrLf
        '    End If

        '    If Not CBool2(Common.WebRequest.Data("ByProduct")) Then
        '        .SqlGroup = "GROUP BY ISNULL(TransInvoiceItemsModifier.Flavor,'N/A'), MobizLibraryOrganizationFOWST.Name, convert(date,Transinvoice.InvoiceDate), MobizLibraryOrganizationFOWST.Code "
        '    Else
        '        .SqlGroup = "GROUP BY ISNULL(MobizLibraryPIMProduct.ProductName,'N/A'), MobizLibraryOrganizationFOWST.Name, convert(date,Transinvoice.InvoiceDate), MobizLibraryOrganizationFOWST.Code "

        '        '.SqlWhere &= " AND NOT EXISTS(" & vbCrLf &
        '        '                " 	SELECT *" & vbCrLf &
        '        '                "     FROM MobizLibraryPIMProductContents" & vbCrLf &
        '        '                "     WHERE MobizLibraryPIMProductContents.ChildProductID = LibraryOrganizationPIM.ProductID" & vbCrLf &
        '        '                "     AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
        '        '                " )"
        '        '.SqlWhere &= " AND MobizLibraryPIMProduct.ID = " & ReadyString(Common.WebRequest.Data("SelectedProductID"))

        '        '" AND ISNULL(LibraryOrganizationPIM.IsInventoryCountItem,0) = 0 " & vbCrLf &
        '    End If

        '    If Not GroupID = vbNullString Then
        '        .SqlFrom &= "INNER JOIN MobizLibraryOrganizationFranchisee on franchiseeorgfowstid = TransInvoice.FowstID " & vbCrLf

        '        .SqlWhere &= " AND MobizLibraryOrganizationFranchisee.FranchiseGroupID = " & ReadyString(GroupID) & vbCrLf
        '    Else

        '        '.SqlWhere &= "AND dbo.TransInvoice.FowstID IN( " & FOWSTID & ")" & vbCrLf

        '    End If

        '    If CBool2(Common.WebRequest.Data("ByProduct")) Then
        '        If Not CStr2(Common.WebRequest.Data("SelectedProductID")) = vbNullString Then
        '            .SqlWhere &= " AND MobizLibraryPIMProduct.ID IN ( " & CStr2(Common.WebRequest.Data("SelectedProductID")) & " ) "
        '        End If
        '    End If

        '    pDT = .Requery

        'If pDT.Columns.Contains("ProductDescription") Then
        '    Dim isRepeat As Boolean

        '    While True

        '        For i As Integer = 0 To pDT.Rows.Count - 1
        '            If CStr2(pDT.Rows(i).Item("ProductDescription")) = "N/A" Then
        '                pDT.Rows.Remove(pDT.Rows(i))
        '                isRepeat = True
        '                Exit For
        '            End If
        '        Next

        '        If Not isRepeat Then Exit While

        '        isRepeat = False

        '    End While

        'End If



        pFilter1 = CreateCrossTab(pDT, {"StoreCode", "StoreName"}, {New CrossColumn("{ProductDescription} {TransactionDate} Sales", "SubTotal", "SubTotal"),
                                                                     New CrossColumn("{ProductDescription} {TransactionDate} Qty", "Qty", "Qty"),
                                                                      New CrossColumn("{ProductDescription} {TransactionDate} TransCount", "TransactionCount", "TransactionCount")})


        pFilter1.Columns.Remove("SubTotal")
        pFilter1.Columns.Remove("Qty")
        pFilter1.Columns.Remove("TransactionCount")

        Return pFilter1
    End Function

    Function ProductPerformanceTrackerComboMix() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable

        pFinder = New Finder

        'Combo Mix

        With pFinder

            .Distinct = True
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("StoreCode", SqlDbType.VarChar)
                    .Expression = "ISNULL(FowstCode,'-')"
                End With
                With .Add("StoreName", SqlDbType.VarChar)
                    .Expression = "FowstName"
                End With

#If DEBUG Then
                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "InvoiceDate"
                End With
#Else
                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "FORMAT(InvoiceDate, 'dd/MMM')"
                End With
#End If
                With .Add("Qty", SqlDbType.VarChar)
                    .Expression = "ProductCount"
                End With

                With .Add("ProductDescription", SqlDbType.VarChar)
                    .Expression = "ProductName"
                End With

                With .Add("TransactionCount", SqlDbType.VarChar)
                    .Expression = "TransactionCount"
                End With
                With .Add("SubTotal", SqlDbType.VarChar)
                    .Expression = "TotalSales"
                End With
            End With

            .SqlFrom = "FROM CacheInvoiceComboMix " & vbCrLf

            .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'"

            .SqlOrder = " ORDER BY TransactionDate"

            pDT = .Requery
        End With


        pFilter1 = CreateCrossTab(pDT, {"StoreCode", "StoreName"}, {New CrossColumn("{ProductDescription} {TransactionDate} Sales", "SubTotal", "SubTotal"),
                                                                     New CrossColumn("{ProductDescription} {TransactionDate} Qty", "Qty", "Qty"),
                                                                      New CrossColumn("{ProductDescription} {TransactionDate} TransCount", "TransactionCount", "TransactionCount")})


        pFilter1.Columns.Remove("SubTotal")
        pFilter1.Columns.Remove("Qty")
        pFilter1.Columns.Remove("TransactionCount")



        Return pFilter1
    End Function

    Function GetConsumptionReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1

        Dim pSql As String
        Dim pdtDates As DataTable

        Dim pSqlMain As String
        Dim pSqlFrom As String
        Dim pSqlOuterApply As String
        Dim pSqlDates As String
        Dim pSqlDelivery As String
        Dim pSqlTransIn As String
        Dim pSqlUSage As String
        Dim TransOut As String
        Dim EndingBalance As String

        Dim pFilter1 As DataTable


        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True '7/15/2023 francis h.

            With .Fields
                With .Add("Store Code", SqlDbType.Bit)
                    .Expression = "ISNULL(FOWSTCode,'-')"
                End With
                With .Add("Store", SqlDbType.Bit)
                    .Expression = "ISNULL(FOWSTName,'-')"
                End With

#If DEBUG Then
                With .Add("Date", SqlDbType.Bit)
                    .Expression = "TransactionDate"
                End With
#Else
                With .Add("Date", SqlDbType.Bit)
                    .Expression = "FORMAT(TransactionDate, 'dd/MMM')"
                End With
#End If

                With .Add("Category", SqlDbType.Bit)
                    .Expression = "ISNULL(Category,'-')"
                End With
                With .Add("Product Code", SqlDbType.Bit)
                    .Expression = "ISNULL(ProductNo,'-')"
                End With
                With .Add("Item", SqlDbType.Bit)
                    .Expression = "ISNULL(ProductName,'-')"
                End With
                'With .Add("ProductID", SqlDbType.UniqueIdentifier)
                '    .Expression = "CacheInventoryMovement.ProductID"
                'End With
                With .Add("Unit", SqlDbType.Bit)
                    .Expression = "ISNULL(UnitMeasure,'-')"
                End With
                With .Add("Cost", SqlDbType.Bit)
                    .Expression = "ISNULL(LandedCost, 0)"
                End With
                With .Add("Beginning Inventory", SqlDbType.Bit)
                    .Expression = "ISNULL(CacheInventoryMovement.BeginningBalance, 0)"
                End With
                With .Add("Delivery In", SqlDbType.Int)
                    .Expression = "ISNULL(CacheInventoryMovement.Delivery, 0)"
                End With
                With .Add("Trans In", SqlDbType.Bit)
                    .Expression = "ISNULL(CacheInventoryMovement.TransIn, 0)"
                End With
                With .Add("Trans Out", SqlDbType.Bit)
                    .Expression = "ISNULL(CacheInventoryMovement.TransOut, 0)"
                End With
                With .Add("Product Defect", SqlDbType.Bit)
                    .Expression = "ISNULL(CacheInventoryMovement.ProductDefect,0) "
                End With
                'With .Add("Wastage", SqlDbType.Bit)
                '    .Expression = "0"
                '    '.Expression = "SUM(ISNULL(CacheInventoryMovement.TransOut, 0))"
                'End With
                With .Add("Physical Count", SqlDbType.Bit)
                    .Expression = "ISNULL(tblInventoryCount.Quantity, 0)"
                End With
                With .Add("Actual Usage", SqlDbType.Bit)
                    .Expression = "ISNULL(CacheInventoryMovement.BeginningBalance, 0) + ISNULL(CacheInventoryMovement.Delivery, 0) +  ISNULL(CacheInventoryMovement.TransIn, 0) " & vbCrLf &
                                 "- ISNULL(CacheInventoryMovement.TransOut, 0) - ISNULL(CacheInventoryMovement.ProductDefect,0) " & vbCrLf &
                                 "- ISNULL(tblInventoryCount.Quantity, 0) "
                End With
                With .Add("Theoretical Usage", SqlDbType.Bit)
                    .Expression = "ISNULL(CacheInventoryMovement.Usage, 0) * -1"
                End With
                With .Add("Theoretical Ending Balance", SqlDbType.Bit)
                    .Expression = "ISNULL(CacheInventoryMovement.BeginningBalance, 0) + ISNULL(CacheInventoryMovement.Delivery, 0) +  ISNULL(CacheInventoryMovement.TransIn, 0) " & vbCrLf &
                                 "- ISNULL(CacheInventoryMovement.TransOut, 0) + ISNULL(CacheInventoryMovement.Usage, 0) "
                End With
                With .Add("Variance", SqlDbType.Bit)
                    .Expression = "ISNULL(CacheInventoryMovement.BeginningBalance, 0) + ISNULL(CacheInventoryMovement.Delivery, 0) +  ISNULL(CacheInventoryMovement.TransIn, 0) " & vbCrLf &
                                  "- ISNULL(CacheInventoryMovement.TransOut, 0) - ISNULL(CacheInventoryMovement.EndingBalance, 0) " & vbCrLf &
                                  "+ ISNULL(CacheInventoryMovement.Usage, 0) "
                End With

                With .Add("Total Actual Cost", SqlDbType.Bit)
                    .Expression = "ISNULL(LandedCost, 0) * " & vbCrLf &
                                    " (ISNULL(CacheInventoryMovement.BeginningBalance, 0) + ISNULL(CacheInventoryMovement.Delivery, 0) +  ISNULL(CacheInventoryMovement.TransIn, 0) " & vbCrLf &
                                    "- ISNULL(CacheInventoryMovement.TransOut, 0) - ISNULL(CacheInventoryMovement.ProductDefect,0) " & vbCrLf &
                                    "- ISNULL(tblInventoryCount.Quantity, 0) )" & vbCrLf
                End With
                'With .Add("SalesNetVAT", SqlDbType.Bit)
                '    .Expression = "ISNULL(LandedCost, 0) * ISNULL(CacheInventoryMovement.Usage, 0) * -1 / 1.12"
                'End With
                'With .Add("InventoryRankingNo", SqlDbType.Bit)
                '    .Expression = "ISNULL(LibraryOrganizationPIM.InventoryRankingNo,9999) "
                'End With
                
            End With

            .SqlFrom = " FROM CacheInventoryMovement " & vbCrLf &
                        " INNER JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = CacheInventoryMovement.ProductID " & vbCrLf

            .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                        "SELECT DISTINCT TOP 1 Quantity, TransInventoryProductCount.InventoryCountDate " & vbCrLf &
                        "FROM TransInventoryProductCountStock INNER JOIN TransInventoryProductCount ON TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID " & vbCrLf &
                        "WHERE TransInventoryProductCount.FowstID = CacheInventoryMovement.FowstID " & vbCrLf &
                        "AND TransInventoryProductCount.ProductID = CacheInventoryMovement.ProductID " & vbCrLf &
                        "AND CONVERT(date, TransInventoryProductCount.InventoryCountDate) = convert(date,CacheInventoryMovement.TransactionDate) " & vbCrLf &
                        "ORDER BY TransInventoryProductCount.InventoryCountDate DESC " & vbCrLf &
                        ") tblInventoryCount "

            .SqlFrom &= "OUTER APPLY ( " & vbCrLf &
                        "SELECT TransInvoiceItems.ProductID, SUM(TransInvoiceItems.Quantity * MobizLibraryPIMProductContents.Quantity) AS TotalQty " & vbCrLf &
                        "from TransInvoiceItems " & vbCrLf &
                        "inner JOIN transinvoice on transinvoice.ID = TransInvoiceItems.InvoiceID " & vbCrLf &
                        "inner join MobizLibraryPIMProductContents on MobizLibraryPIMProductContents.ChildProductID = TransInvoiceItems.ProductID " & vbCrLf &
                        "WHERE MobizLibraryPIMProductContents.ChildProductID = CacheInventoryMovement.ProductID " & vbCrLf &
                        "AND CONVERT(date, transinvoice.InvoiceDate) = convert(date,CacheInventoryMovement.TransactionDate)  " & vbCrLf &
                        "group by TransInvoiceItems.ProductID " & vbCrLf &
                        ") TheoreticalUsage "

            '.SqlFrom &= "OUTER APPLY (Select SUM(ISNULL(QuantityGood,0)) ProductDefect " & vbCrLf &
            '            "FROM TransReceivingStock " & vbCrLf &
            '            "INNER JOIN TransReceiving on TransReceivingStock.ReceivingID = TransReceiving.ID " & vbCrLf &
            '            "INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransReceiving.SupplierID " & vbCrLf &
            '            "WHERE  IsReceiving = 0 AND MobizLibraryOrganizationFOWST.Name = 'Product Defect' and TransReceiving.ReceivingPlaceID = CacheInventoryMovement.FowstID " & vbCrLf &
            '            " AND convert(date, TransReceiving.DocumentDate) = convert(date,CacheInventoryMovement.TransactionDate) " & vbCrLf &
            '            ") tblProductDefect "

            'AND TransReceiving.ReceivingPlaceID = " & ReadyString(FOWSTID) & " 

            .SqlWhere = " WHERE ISNULL(LibraryOrganizationPIM.IsInventoryCountItem,0) = 1 "

            If FOWSTID = vbNullString Then
                .SqlWhere &= " AND CacheInventoryMovement.FranchisegroupID = " & ReadyString(GroupID) & vbCrLf
            Else
                .SqlWhere &= " AND CacheInventoryMovement.FowstID = " & ReadyString(FOWSTID) & vbCrLf
            End If


            .SqlWhere &= " AND CacheInventoryMovement.TransactionDate BETWEEN " & ProSqlite.ReadyDate(DateFrom) & " AND " & ProSqlite.ReadyDate(DateTo) & vbCrLf


            '.SqlOrder = " ORDER BY InventoryRankingNo "

            pDT = .Requery

            'pDT.Columns.Add("TheoreticalEnding")

            'For i As Integer = 0 To pDT.Rows.Count - 1
            '    If CDec2(pDT.Rows(i).Item("InventoryCount")) > 0 Then
            '        pDT.Rows(i).Item("TheoreticalEnding") = CDec2(pDT.Rows(i).Item("InventoryCount")) - (CDec2(pDT.Rows(i).Item("Variance")) * -1)
            '    Else
            '        pDT.Rows(i).Item("TheoreticalEnding") = CDec2(pDT.Rows(i).Item("EndingBalance"))
            '    End If
            'Next

            'pFilter1 = CreateCrossTab(pDT, {"ProductName", "UnitMeasure", "Cost"}, {New CrossColumn("Beg Bal {TransactionDate}", "BeginningBalance", "BeginningBalance"),
            '                                                                New CrossColumn("Del {TransactionDate}", "Delivery", "Delivery"),
            '                                                                New CrossColumn("Trans In {TransactionDate}", "TransIn", "TransIn"),
            '                                                                New CrossColumn("Trans Out {TransactionDate}", "TransOut", "TransOut"),
            '                                                                New CrossColumn("Product Defect {TransactionDate}", "ProductDefect", "ProductDefect"),
            '                                                                New CrossColumn("Usage {TransactionDate}", "Usage", "Usage"),
            '                                                                New CrossColumn("EndBal {TransactionDate}", "EndingBalance", "EndingBalance"),
            '                                                                New CrossColumn("Variance {TransactionDate}", "Variance", "Variance"),
            '                                                                New CrossColumn("Actual Count {TransactionDate}", "InventoryCount", "InventoryCount"),
            '                                                                New CrossColumn("Theoretical Ending {TransactionDate}", "TheoreticalEnding", "TheoreticalEnding"),
            '                                                                New CrossColumn("Total Cost {TransactionDate}", "TotalCost", "TotalCost"),
            '                                                                New CrossColumn("Sales N/ VAT {TransactionDate}", "SalesNetVAT", "SalesNetVAT")})

        End With

        'pFilter1.Columns.Remove("BeginningBalance")
        'pFilter1.Columns.Remove("Delivery")
        'pFilter1.Columns.Remove("TransIn")
        'pFilter1.Columns.Remove("TransOut")
        'pFilter1.Columns.Remove("Usage")
        'pFilter1.Columns.Remove("EndingBalance")
        'pFilter1.Columns.Remove("Variance")
        'pFilter1.Columns.Remove("InventoryCount")
        'pFilter1.Columns.Remove("TheoreticalEnding")
        'pFilter1.Columns.Remove("ProductDefect")
        'pFilter1.Columns.Remove("TotalCost")
        'pFilter1.Columns.Remove("SalesNetVAT")

        'Return pFilter1

        Return pDT
    End Function

    Function GetDailySalesPerSKUReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable
        Dim pSql As String

        pFinder = New Finder

        With pFinder

            '.Distinct = True
            .ProData = Common.WebRequest.ProData

            With .Fields

#If DEBUG Then
                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "convert(date, InvoiceDate)"
                End With
#Else
                        With .Add("TransactionDate", SqlDbType.Bit)
                            .Expression = "FORMAT(InvoiceDate, 'd/MMM')"
                        End With
#End If
                With .Add("Category", SqlDbType.Bit)
                    .Expression = "ISNULL(CategoryName, '')"
                End With
                With .Add("ProductCode", SqlDbType.VarChar)
                    .Expression = "ISNULL(ProductNo,'')"
                End With
                With .Add("ProductDescription", SqlDbType.VarChar)
                    .Expression = "ISNULL(ProductName,'')"
                End With
                With .Add("Qty", SqlDbType.VarChar)
                    .Expression = "ISNULL(SUM(Qty),0)"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "ISNULL(UnitMeasure,'')"
                End With
                With .Add("SubTotal", SqlDbType.VarChar)
                    .Expression = "ISNULL(SUM(SubTotal),0) "
                End With
            End With

            .SqlFrom = "FROM CacheDailySalesPerSKUReport " & vbCrLf

            .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            .SqlGroup = "GROUP BY convert(date, InvoiceDate), ISNULL(ProductNo,''), ISNULL(ProductName,''), ISNULL(UnitMeasure,''), ISNULL(CategoryName, '') " & vbCrLf

            If Not GroupID = vbNullString Then
                .SqlWhere &= " AND FranchiseGroupID IN (" & GroupID & ") " & vbCrLf
            Else
                .SqlWhere &= " AND FowstID IN (" & FOWSTID & ")" & vbCrLf
            End If


            pDT = .Requery

        End With

        'comment date 7/3/2024

        'pSql = "SELECT TOP 100 PERCENT convert(date,CacheInvoices.InvoiceDate) AS [TransactionDate] " & vbCrLf &
        '        ", ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'') AS [ProductDescription] " & vbCrLf &
        '        ", dbo.TransInvoiceItems.UnitMeasure AS [UnitMeasure] AS [ProductDescription] " & vbCrLf &
        '        ", ISNULL(SUM(dbo.TransInvoiceItems.Quantity),0) AS [Qty] " & vbCrLf &
        '        ", ISNULL(SUM(dbo.TransInvoiceItems.UnitPrice * dbo.TransInvoiceItems.Quantity),0)  AS [SubTotal] " & vbCrLf &
        '        " FROM dbo.TransInvoiceItems INNER JOIN  " & vbCrLf &
        '        "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN   " & vbCrLf &
        '        "dbo.CacheInvoices ON dbo.TransInvoiceItems.InvoiceID = dbo.CacheInvoices.ID   " & vbCrLf &
        '        "WHERE CONVERT(date, dbo.CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf


        ''Premium Flavors

        'pSql = "SELECT convert(date,CacheInvoices.InvoiceDate) TransactionDate  " & vbCrLf &
        '        ", ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'')   " & vbCrLf &
        '        "+ ' - ' + ISNULL(mp2.ProductName,'') AS [ProductDescription]  " & vbCrLf &
        '        ", dbo.TransInvoiceItems.UnitMeasure AS [UnitMeasure]  " & vbCrLf &
        '        ", ISNULL(SUM(dbo.TransInvoiceItems.Quantity),0) AS [Qty]  " & vbCrLf &
        '        ", ISNULL(SUM(dbo.TransInvoiceItems.SubTotal),0) + ISNULL((SUM(TransInvoiceItemsModifier .PremiumPrice)),0) + ISNULL((SUM(TransInvoiceItemsModifier .ModifierPrice)),0)  AS [SubTotal]   " & vbCrLf &
        '        "        from CacheInvoices " & vbCrLf &
        '        "inner join TransInvoiceItems on TransInvoiceItems.InvoiceID = CacheInvoices.ID  " & vbCrLf &
        '        "INNER JOIN TransInvoiceItemsModifier on TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID  " & vbCrLf &
        '        "INNER JOIN dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID   " & vbCrLf &
        '        "INNER JOIN LibraryItemModifierInventoryAffecting on LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID  " & vbCrLf &
        '        "INNER JOIN dbo.MobizLibraryPIMProduct mp2 ON dbo.LibraryItemModifierInventoryAffecting.ProductID = mp2.ID   " & vbCrLf &
        '        " WHERE TransInvoiceItemsModifier.PremiumPrice > 0 " & vbCrLf &
        '        " AND CONVERT(date, dbo.CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

        'If Not GroupID = vbNullString Then
        '    pSql &= " AND dbo.CacheInvoices.FranchiseGroupID IN (" & GroupID & ") " & vbCrLf
        'Else
        '    pSql &= " AND dbo.CacheInvoices.FowstID IN (" & FOWSTID & ")" & vbCrLf
        'End If

        'pSql &= "GROUP BY dbo.TransInvoiceItems.UnitMeasure, ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'') + ' - ' + ISNULL(mp2.ProductName,''), convert(date,CacheInvoices.InvoiceDate) " & vbCrLf

        'pSql &= " UNION " & vbCrLf &
        '        "SELECT convert(date,CacheInvoices.InvoiceDate) TransactionDate  " & vbCrLf &
        '        ", ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'') AS [ProductDescription]  " & vbCrLf &
        '        ", dbo.TransInvoiceItems.UnitMeasure AS [UnitMeasure]  " & vbCrLf &
        '        ", ISNULL(SUM(dbo.TransInvoiceItems.Quantity),0) AS [Qty]  " & vbCrLf &
        '        ", ISNULL(SUM(dbo.TransInvoiceItems.SubTotal),0)  AS [SubTotal]   " & vbCrLf &
        '        " FROM CacheInvoices " & vbCrLf &
        '        "inner join TransInvoiceItems on TransInvoiceItems.InvoiceID = CacheInvoices.ID  " & vbCrLf &
        '        "INNER JOIN dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID   " & vbCrLf &
        '        "WHERE CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

        'If Not GroupID = vbNullString Then
        '    pSql &= " AND dbo.CacheInvoices.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
        'Else
        '    pSql &= " AND dbo.CacheInvoices.FowstID IN (" & FOWSTID & ")" & vbCrLf
        'End If

        'pSql &= "AND TransInvoiceItems.ID NOT IN ( " & vbCrLf &
        '        "Select TransInvoiceItems.ID " & vbCrLf &
        '        " FROM CacheInvoices  " & vbCrLf &
        '        "inner join TransInvoiceItems on TransInvoiceItems.InvoiceID = CacheInvoices.ID  " & vbCrLf &
        '        "INNER JOIN TransInvoiceItemsModifier on TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID  " & vbCrLf &
        '        "INNER JOIN LibraryItemModifierInventoryAffecting on LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID  " & vbCrLf &
        '        "WHERE TransInvoiceItemsModifier .PremiumPrice>0   " & vbCrLf &
        '        " AND CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

        'If Not GroupID = vbNullString Then
        '    pSql &= " AND dbo.CacheInvoices.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf & ")"
        'Else
        '    pSql &= " AND dbo.CacheInvoices.FowstID IN (" & FOWSTID & ")" & vbCrLf & ")"
        'End If

        'pSql &= "GROUP BY dbo.TransInvoiceItems.UnitMeasure, convert(date,CacheInvoices.InvoiceDate)  , ISNULL(MobizLibraryPIMProduct.ProductGroupNo,'') + ' ' + ISNULL(MobizLibraryPIMProduct.ProductNo,'') + ' - ' + ISNULL(MobizLibraryPIMProduct.ProductName,'')  " & vbCrLf 

        'pSql &= "UNION "

        ''Poco-llectibles
        'pSql &= "SELECT convert(date,CacheInvoices.InvoiceDate) TransactionDate  " & vbCrLf &
        '       ", ISNULL(mp2.ProductGroupNo,'') + ' ' + ISNULL(mp2.ProductName,'') AS [ProductDescription] " & vbCrLf &
        '       ", dbo.TransInvoiceItems.UnitMeasure AS [UnitMeasure]  " & vbCrLf &
        '       ", ISNULL(SUM(dbo.TransInvoiceItemsModifier.InputQty),0) AS [Qty]  " & vbCrLf &
        '       ", ISNULL((SUM(TransInvoiceItemsModifier .ModifierPrice)),0)  AS [SubTotal]   " & vbCrLf &
        '       "        from CacheInvoices " & vbCrLf &
        '       "inner join TransInvoiceItems on TransInvoiceItems.InvoiceID = CacheInvoices.ID  " & vbCrLf &
        '       "INNER JOIN TransInvoiceItemsModifier on TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID  " & vbCrLf &
        '       "INNER JOIN dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID   " & vbCrLf &
        '       "INNER JOIN LibraryItemModifierInventoryAffecting on LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID  " & vbCrLf &
        '       "INNER JOIN dbo.MobizLibraryPIMProduct mp2 ON dbo.LibraryItemModifierInventoryAffecting.ProductID = mp2.ID   " & vbCrLf &
        '        "  WHERE mp2.ID  IN ( " & vbCrLf &
        '        "   'fe53d817-feb1-44bc-bfe7-279c3a6380d5', " & vbCrLf &
        '        "   '374b4dac-5aa5-49f9-8143-b7ac3ca1483a', " & vbCrLf &
        '        "   'ddef9ae8-5083-44e1-a72a-fa0e27d4f56f', " & vbCrLf &
        '        "   '7c0e712a-6e94-4543-8af2-1ad909cba139' " & vbCrLf &
        '        ") " & vbCrLf &
        '       " AND CONVERT(date, dbo.CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

        'If Not GroupID = vbNullString Then
        '    pSql &= " AND dbo.CacheInvoices.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
        'Else
        '    pSql &= " AND dbo.CacheInvoices.FowstID IN (" & FOWSTID & ")" & vbCrLf
        'End If

        'pSql &= "GROUP BY dbo.TransInvoiceItems.UnitMeasure, ISNULL(mp2.ProductGroupNo,'') + ' ' + ISNULL(mp2.ProductName,''), convert(date,CacheInvoices.InvoiceDate) " & vbCrLf

        'pDT = Common.WebRequest.ProData.Execute(pSql)

        pFilter1 = CreateCrossTab(pDT, {"Category", "ProductCode", "ProductDescription", "UnitMeasure"}, {New CrossColumn("Qty {TransactionDate}", "Qty", "Qty"),
                                                                                New CrossColumn("Sales {TransactionDate}", "SubTotal", "SubTotal")})

        'End With

        pFilter1.Columns.Remove("Qty")
        pFilter1.Columns.Remove("SubTotal")

        Return pFilter1

    End Function

    Function GetDailySalesPerStoreReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pFilter1 As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("StoreName", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(FOWSTCode,'') + '-' + CacheInvoices.FOWSTName"
                End With

                'With .Add("StoreCode", SqlDbType.VarChar)
                '    .Expression = "ISNULL(FOWSTCode,'')"
                'End With

                'With .Add("StoreType", SqlDbType.VarChar)
                '    .Expression = "ISNULL(CacheInvoices.StoreType,'')"
                'End With
                'With .Add("StoreLocation", SqlDbType.VarChar)
                '    .Expression = "ISNULL(CacheInvoices.StoreLocation,'')"
                'End With
                'With .Add("GroupName", SqlDbType.Int)
                '    .Expression = "ISNULL(CacheInvoices.GroupName,'')"
                'End With

#If DEBUG Then
                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "convert(date,CacheInvoices.InvoiceDate)"
                End With
#Else
                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "FORMAT(CacheInvoices.InvoiceDate, 'd/MMM')"
                End With
#End If

                'With .Add("TransactionDate", SqlDbType.VarChar)
                '    .Expression = "convert(date,CacheInvoices.InvoiceDate)"
                'End With
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("SalesReturnAndRefund", SqlDbType.Bit)
                    .Expression = "ISNULL(" &
                        "(Select SUM(Amount) FROM TransSalesReturnLedger " & vbCrLf &
                        "inner join TransMemo ON TransMemo.ID = TransSalesReturnLedger.SourceMemoID AND TransSalesReturnLedger.FowstID = CacheInvoices.FOWSTID and convert(date, TransMemo.LastUpdated) = convert(date,CacheInvoices.InvoiceDate) " & vbCrLf &
                        "WHERE TransSalesReturnLedger.DestinationPaymentID IS NULL),'') " '& vbCrLf &
                    '"AND CONVERT(date,TransMemo.LastUpdated) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & ")" & vbCrLf &
                    ' ",0)"
                End With
                'With .Add("TransactionCount", SqlDbType.Bit)
                '    .Expression = "ISNULL(COUNT(DISTINCT CacheInvoices.ID),0)"
                'End With
                With .Add("DiscountAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.SCDiscount),0) + ISNULL(SUM(CacheInvoices.ForegoneVAT),0)"
                End With
                With .Add("CashAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.CashNetOfChange),0)"
                End With
                'With .Add("CreditCardAmount", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM(CacheInvoices.CreditCardAmount),0)"
                'End With
                'With .Add("DebitCardAmount", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM(CacheInvoices.DebitCardAmount),0)"
                'End With
                'With .Add("CheckPaymentAmount", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM(CacheInvoices.PotatoCornerGC),0)"
                'End With
                'With .Add("OtherPayments", SqlDbType.Bit)
                '    .Expression = "SUM(TransCustomerPayment.CheckPaymentAmount)"
                'End With
                'With .Add("DiscountVoucher", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM(CacheInvoices.[Discount Voucher / On Line Deals]),0)"
                'End With
                'With .Add("E-Plus", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM(CacheInvoices.[E-Plus]),0)"
                'End With
                With .Add("FoodPanda", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[FoodPanda]),0)"
                End With
                With .Add("GCash", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[GCash]),0)"
                End With
                With .Add("PayMaya", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[PayMaya]),0)"
                End With
                With .Add("GrabFood", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[GrabFood]),0)"
                End With
                'With .Add("SMAC", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM(CacheInvoices.[SMAC]),0)"
                'End With
                'With .Add("V-Plus", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM(CacheInvoices.[V-Plus]),0)"
                'End With

                'With .Add("GoldCard", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM(CacheInvoices.[GoldCard]),0)"
                'End With

                With .Add("ShopeeEVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[ShopeeEVoucher]),0)"
                End With

                With .Add("LocalDelivery", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[LocalDelivery]),0)"
                End With

                With .Add("ShopeePay", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[ShopeePay]),0)"
                End With

                With .Add("Eatigo", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Eatigo]),0)"
                End With

                With .Add("DeliRush", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[DeliRush]),0)"
                End With

                With .Add("PoodTrip", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[PoodTrip]),0)"
                End With

                With .Add("ShareTreats", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Share Treats]),0)"
                End With

                With .Add("Booky", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Booky]),0)"
                End With

                With .Add("Lazada", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Lazada]),0)"
                End With

                With .Add("Pickaroo", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[Pickaroo]),0)"
                End With

                With .Add("SupercardVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.SupercardVoucher),0)"
                End With

                'With .Add("LatestTransactionDate", SqlDbType.Bit)
                '    .Expression = "MAX(CacheInvoices.InvoiceDate)"
                'End With

            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf
            '& " AND CacheInvoices.FowstID IN (select distinct FowstID from LibraryOrganizationEmployeeFOWST WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleID) & ") "

            'If CStr2(Common.WebRequest.Data("StoreType")) <> "" Then
            '    .SqlWhere += " AND CacheInvoices.SubFOWSTTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreType"))) & vbCrLf
            'End If

            'If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
            '    .SqlWhere += " AND CacheInvoices.StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
            'End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= "AND CacheInvoices.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            Else
                .SqlWhere &= "AND CacheInvoices.FOWSTID IN (" & FOWSTID & ")" & vbCrLf
            End If

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(FOWSTCode,'') + '-' +CacheInvoices.FOWSTName " & vbCrLf &
                        ",CacheInvoices.FowstID " & vbCrLf &
                        ", CacheInvoices.InvoiceDate " & vbCrLf

            '", ISNULL(CacheInvoices.FOWSTCode,'')"

            '", CacheInvoices.StoreType  " & vbCrLf &
            '", CacheInvoices.StoreLocation " & vbCrLf &
            '",CacheInvoices.GroupName " & vbCrLf &

            .SqlOrder = " ORDER BY convert(date,CacheInvoices.InvoiceDate) ASC "

            pDT = .Requery

            pFilter1 = CreateCrossTab(pDT, {"StoreName"}, {New CrossColumn("GrossSales {TransactionDate}", "GrossSales", "GrossSales"),
                                                                            New CrossColumn("Refund {TransactionDate}", "SalesReturnAndRefund", "SalesReturnAndRefund"),
                                                                            New CrossColumn("Discount {TransactionDate}", "DiscountAmount", "DiscountAmount"),
                                                                            New CrossColumn("Cash {TransactionDate}", "CashAmount", "CashAmount"),
                                                                            New CrossColumn("GCash {TransactionDate}", "GCash", "GCash"),
                                                                            New CrossColumn("PayMaya {TransactionDate}", "PayMaya", "PayMaya"),
                                                                            New CrossColumn("ShopeePay {TransactionDate}", "ShopeePay", "ShopeePay"),
                                                           New CrossColumn("ShopeeEVoucher {TransactionDate}", "ShopeeEVoucher", "ShopeeEVoucher"),
                                                           New CrossColumn("GrabFood {TransactionDate}", "GrabFood", "GrabFood"),
                                                           New CrossColumn("FoodPanda {TransactionDate}", "FoodPanda", "FoodPanda"),
                                                           New CrossColumn("Eatigo {TransactionDate}", "Eatigo", "Eatigo"),
                                                           New CrossColumn("DeliRush {TransactionDate}", "DeliRush", "DeliRush"),
                                                           New CrossColumn("PoodTrip {TransactionDate}", "PoodTrip", "PoodTrip"),
                                                           New CrossColumn("ShareTreats {TransactionDate}", "ShareTreats", "ShareTreats"),
                                                           New CrossColumn("Booky {TransactionDate}", "Booky", "Booky"),
                                                           New CrossColumn("Lazada {TransactionDate}", "Lazada", "Lazada"),
                                                           New CrossColumn("Pickaroo {TransactionDate}", "Pickaroo", "Pickaroo"),
                                                           New CrossColumn("LocalDelivery {TransactionDate}", "LocalDelivery", "LocalDelivery")})
        End With

        pFilter1.Columns.Remove("GrossSales")
        pFilter1.Columns.Remove("SalesReturnAndRefund")
        pFilter1.Columns.Remove("DiscountAmount")
        pFilter1.Columns.Remove("CashAmount")
        pFilter1.Columns.Remove("GCash")
        pFilter1.Columns.Remove("PayMaya")
        pFilter1.Columns.Remove("ShopeePay")
        pFilter1.Columns.Remove("ShopeeEVoucher")
        pFilter1.Columns.Remove("GrabFood")
        pFilter1.Columns.Remove("FoodPanda")
        pFilter1.Columns.Remove("Eatigo")
        pFilter1.Columns.Remove("DeliRush")
        pFilter1.Columns.Remove("PoodTrip")
        pFilter1.Columns.Remove("ShareTreats")
        pFilter1.Columns.Remove("Booky")
        pFilter1.Columns.Remove("Lazada")
        pFilter1.Columns.Remove("Pickaroo")
        pFilter1.Columns.Remove("LocalDelivery")


        Return pFilter1

    End Function

    Function GetFoodAggregatorsReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pFilter1 As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("StoreName", SqlDbType.UniqueIdentifier)
                    .Expression = "ISNULL(CacheInvoices.FOWSTName, '')"
                End With

                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "convert(date,CacheInvoices.InvoiceDate)"
                End With

                With .Add("Size", SqlDbType.Bit)
                    .Expression = "ISNULL(CASE WHEN pv1.C0036 = '' THEN 'N/A' ELSE pv1.C0036 END,'N/A')"
                End With

                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With

                With .Add("FoodPandaProductCount", SqlDbType.Bit)
                    .Expression = "ISNULL(CASE WHEN [FoodPanda]>0 then SUM(dbo.TransInvoiceItems.Quantity) END,0)"
                End With

                With .Add("GrabProductCount", SqlDbType.Bit)
                    .Expression = "ISNULL(CASE WHEN [GrabFood]>0 then SUM(dbo.TransInvoiceItems.Quantity) END,0)"
                End With

                With .Add("FoodPanda", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[FoodPanda]),0)"
                End With

                With .Add("GrabFood", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.[GrabFood]),0)"
                End With

            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf &
                        "INNER JOIN TransInvoiceItems ON dbo.TransInvoiceItems.InvoiceID = dbo.CacheInvoices.ID " & vbCrLf &
                        "INNER JOIN MobizLibraryPIMProductVariableFeatures001 pv1 ON dbo.TransInvoiceItems.ProductID = pv1.ProductID "

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND pv1.C0036 IS NOT NULL AND (FoodPanda>0 or GrabFood > 0) " & vbCrLf

            If Not GroupID = vbNullString Then
                .SqlWhere &= "AND CacheInvoices.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            Else
                .SqlFrom &= " AND CacheInvoices.FowstID = " & ReadyString(FOWSTID) & vbCrLf
            End If

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "CacheInvoices.FOWSTName " & vbCrLf &
                        ",ISNULL(CASE WHEN pv1.C0036 = '' THEN 'N/A' ELSE pv1.C0036 END,'N/A') " & vbCrLf &
                        ", convert(date,CacheInvoices.InvoiceDate) " & vbCrLf &
                        ", GrabFood " & vbCrLf &
                        ", FoodPanda " & vbCrLf

            .SqlOrder = " ORDER BY TransactionDate ASC "

            pDT = .Requery

            pFilter1 = CreateCrossTab(pDT, {"StoreName", "Size"}, {New CrossColumn("GrabFood Count {TransactionDate}", "GrabProductCount", "GrabProductCount"),
                                                                   New CrossColumn("GrabFood Sales {TransactionDate}", "GrabFood", "GrabFood"),
                                                                   New CrossColumn("FoodPanda Count {TransactionDate}", "FoodPandaProductCount", "FoodPandaProductCount"),
                                                                   New CrossColumn("FoodPanda Sales {TransactionDate}", "FoodPanda", "FoodPanda")
                                                                   })
        End With

        pFilter1.Columns.Remove("GrabFood")
        pFilter1.Columns.Remove("FoodPanda")
        pFilter1.Columns.Remove("GrabProductCount")
        pFilter1.Columns.Remove("FoodPandaProductCount")

        Return pFilter1

        For i As Integer = 0 To pFilter1.Rows.Count - 1
            For j As Integer = 2 To pFilter1.Columns.Count - 1 'start sorting at column 6
                If CStr2(pFilter1.Rows(i).Item(j)) = vbNullString Then
                    pFilter1.Rows(i).Item(j) = 0
                End If
            Next
        Next


        Dim tempDatatable As New DataTable
        Dim mHeaders As New List(Of Date)

        For i As Integer = 0 To pFilter1.Columns.Count - 1
            Dim lblgrab As String = Trim(CStr2(pFilter1.Columns(i).ColumnName).Replace("GrabFood", ""))
            If IsDate(lblgrab) Then
                mHeaders.Add(CDate(lblgrab))
            End If
        Next

        mHeaders.Sort()

        tempDatatable.Columns.Add("Store Name")

        For i As Integer = 0 To mHeaders.Count - 1
            tempDatatable.Columns.Add("GrabFood " & mHeaders(i))
            tempDatatable.Columns.Add("FoodPanda " & mHeaders(i))
        Next

        For i As Integer = 0 To pFilter1.Rows.Count - 1
            Dim dr As DataRow

            dr = tempDatatable.NewRow

            dr(0) = pFilter1.Rows(i).Item("StoreName")

            For j As Integer = 1 To tempDatatable.Columns.Count - 1
                dr(j) = pFilter1.Rows(i).Item(tempDatatable.Columns(j).ColumnName)
            Next

            tempDatatable.Rows.Add(dr)
        Next

        Return tempDatatable

    End Function

    Function GetConsumptionReportPerType() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1

        Dim pSql As String
        Dim pdtDates As DataTable

        Dim pSqlMain As String
        Dim pSqlFrom As String
        Dim pSqlOuterApply As String
        Dim pSqlDates As String
        Dim pSqlDelivery As String
        Dim pSqlTransIn As String
        Dim pSqlUSage As String
        Dim TransOut As String
        Dim EndingBalance As String

        Dim pFilter1 As DataTable


        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            .Distinct = True

            With .Fields
                With .Add("Category", SqlDbType.Bit)
                    .Expression = "Category"
                End With
                With .Add("ProductNo", SqlDbType.Bit)
                    .Expression = "ProductNo"
                End With
                With .Add("ProductName", SqlDbType.Bit)
                    .Expression = "ProductName"
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "ProductID"
                End With
                With .Add("UnitMeasure", SqlDbType.Bit)
                    .Expression = "UnitMeasure"
                End With

                '#If DEBUG Then
                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "TransactionDate"
                End With
                '#Else
                '                With .Add("TransactionDate", SqlDbType.Bit)
                '                    .Expression = "FORMAT(TransactionDate, 'dd/MMM')"
                '                End With
                '#End If
                If CStr2(Common.WebRequest.Data("ReportType")) = "Usage" Then
                    With .Add("Usage", SqlDbType.Bit)
                        .Expression = "ISNULL(CacheInventoryMovement.Usage, 0) * -1"
                    End With
                Else
                    With .Add("EndingBalance", SqlDbType.Bit)
                        .Expression = "ISNULL(CacheInventoryMovement.EndingBalance, 0)"
                    End With
                End If

                'With .Add("BeginningBalance", SqlDbType.Bit)
                '    .Expression = "ISNULL(CacheInventoryMovement.BeginningBalance, 0)"
                'End With
                'With .Add("Delivery", SqlDbType.Int)
                '    .Expression = "ISNULL(CacheInventoryMovement.Delivery, 0)"
                'End With
                'With .Add("TransIn", SqlDbType.Bit)
                '    .Expression = "ISNULL(CacheInventoryMovement.TransIn, 0)"
                'End With
                'With .Add("TransOut", SqlDbType.Bit)
                '    .Expression = "ISNULL(CacheInventoryMovement.TransOut, 0)"
                'End With
                'With .Add("Wastage", SqlDbType.Bit)
                '    .Expression = "0"
                '    '.Expression = "SUM(ISNULL(CacheInventoryMovement.TransOut, 0))"
                'End With
                'With .Add("EndingBalance", SqlDbType.Bit)
                '    .Expression = "ISNULL(CacheInventoryMovement.EndingBalance, 0)"
                'End With
                'With .Add("Usage", SqlDbType.Bit)
                '    .Expression = "ISNULL(CacheInventoryMovement.Usage, 0)"
                'End With
                'With .Add("Variance", SqlDbType.Bit)
                '    .Expression = "ISNULL(CacheInventoryMovement.BeginningBalance, 0) + ISNULL(CacheInventoryMovement.Delivery, 0) +  ISNULL(CacheInventoryMovement.TransIn, 0) " & vbCrLf &
                '                  "- ISNULL(CacheInventoryMovement.TransOut, 0) - ISNULL(CacheInventoryMovement.EndingBalance, 0) " & vbCrLf &
                '                  "+ ISNULL(CacheInventoryMovement.Usage, 0) "
                'End With
                With .Add("Cost", SqlDbType.Bit)
                    .Expression = "ISNULL(LandedCost, 0)"
                End With
            End With

            .SqlFrom = " FROM CacheInventoryMovement " & vbCrLf
            '"JOIN MobizLibraryPIMProductVariableFeatures001 ON MobizLibraryPIMProductVariableFeatures001.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
            '" JOIN CacheInventoryMovement ON CacheInventoryMovement.ProductID = MobizLibraryPimProduct.ID" & vbCrLf

            If FOWSTID = vbNullString Then
                '.SqlFrom &= " AND CacheInventoryMovement.FowstID IN(Select distinct fowstid from MobizLibraryOrganizationFranchisee WHERE FranchiseGroupID = " & GroupID & ") "
                .SqlWhere = "WHERE FranchiseGroupID = " & GroupID & " AND CacheInventoryMovement.TransactionDate BETWEEN " & ProSqlite.ReadyDate(DateFrom) & " AND " & ProSqlite.ReadyDate(DateTo) & vbCrLf
            Else
                '.SqlFrom &= " AND CacheInventoryMovement.FowstID = " & ReadyString(FOWSTID) & vbCrLf
                .SqlWhere = "WHERE FowstID = " & ReadyString(FOWSTID) & " AND CacheInventoryMovement.TransactionDate BETWEEN " & ProSqlite.ReadyDate(DateFrom) & " AND " & ProSqlite.ReadyDate(DateTo) & vbCrLf
            End If


            '.SqlFrom &= " AND CacheInventoryMovement.TransactionDate BETWEEN " & ProSqlite.ReadyDate(DateFrom) & " AND " & ProSqlite.ReadyDate(DateTo) & vbCrLf &
            '            " INNER JOIN MobizLibraryPIMProductMeasurement as pimmsr ON pimmsr.ProductID = MobizLibraryPimProduct.ID AND MeasurementLevel = 1 " & vbCrLf &
            '            " INNER JOIN MobizLibraryPIMMeasure as msr ON msr.ID = pimmsr.UnitMeasureID  " & vbCrLf &
            '            " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " & vbCrLf &
            '            " INNER JOIN LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPimProduct.ID " & vbCrLf &
            '            " INNER JOIN MobizLibraryPIMProductPriceLevel prodPriceLvlVendor   ON prodPriceLvlVendor.ProductID = CacheInventoryMovement.ProductID "

            '.SqlWhere = "where ISNULL(LibraryOrganizationPIM.IsInventoryCountItem,0) = 1 AND prodPriceLvlVendor.PriceLevelID = '01000012-0003-0000-0000-000000000000' " 'landed cost

            .SqlOrder = " ORDER BY TransactionDate"

            pDT = .Requery

            If CStr2(Common.WebRequest.Data("ReportType")) = "Usage" Then
                pFilter1 = CreateCrossTab(pDT, {"Category", "ProductNo", "ProductName", "UnitMeasure", "Cost"}, {New CrossColumn("Usage {TransactionDate}", "Usage", "Usage")})
            Else
                pFilter1 = CreateCrossTab(pDT, {"Category", "ProductNo", "ProductName", "UnitMeasure", "Cost"}, {New CrossColumn("EndingBalance {TransactionDate}", "EndingBalance", "EndingBalance")})
            End If


        End With

        If CStr2(Common.WebRequest.Data("ReportType")) = "Usage" Then
            pFilter1.Columns.Remove("Usage")
        Else
            pFilter1.Columns.Remove("EndingBalance")
        End If

        For i As Integer = 0 To pFilter1.Rows.Count - 1
            For j As Integer = 3 To pFilter1.Columns.Count - 1
                If CStr2(pFilter1.Rows(i).Item(j)) = vbNullString Then
                    pFilter1.Rows(i).Item(j) = 0
                End If
            Next
        Next

        'Sort Columns

        Dim tempDatatable As New DataTable
        Dim mHeaders As New List(Of Date)


        For i As Integer = 0 To pFilter1.Columns.Count - 1
            If CStr2(Common.WebRequest.Data("ReportType")) = "Usage" Then
                If IsDate(Trim(CStr2(pFilter1.Columns(i).ColumnName).Replace("Usage", ""))) Then
                    mHeaders.Add(CDate(Trim(CStr2(pFilter1.Columns(i).ColumnName).Replace("Usage", ""))))
                End If
            Else
                If IsDate(Trim(CStr2(pFilter1.Columns(i).ColumnName).Replace("EndingBalance", ""))) Then
                    mHeaders.Add(CDate(Trim(CStr2(pFilter1.Columns(i).ColumnName).Replace("EndingBalance", ""))))
                End If
            End If
        Next

        mHeaders.Sort()

        tempDatatable.Columns.Add("Category")
        tempDatatable.Columns.Add("ProductCode")
        tempDatatable.Columns.Add("ProductName")
        tempDatatable.Columns.Add("UnitMeasure")
        tempDatatable.Columns.Add("Cost")

        For i As Integer = 0 To mHeaders.Count - 1
            tempDatatable.Columns.Add(mHeaders(i))
        Next

        For i As Integer = 0 To pFilter1.Rows.Count - 1
            Dim dr As DataRow

            dr = tempDatatable.NewRow

            dr(0) = pFilter1.Rows(i).Item("Category")
            dr(1) = pFilter1.Rows(i).Item("ProductNo")
            dr(2) = pFilter1.Rows(i).Item("ProductName")
            dr(3) = pFilter1.Rows(i).Item("UnitMeasure")
            dr(4) = pFilter1.Rows(i).Item("Cost")

            For j As Integer = 5 To tempDatatable.Columns.Count - 1
                If CStr2(Common.WebRequest.Data("ReportType")) = "Usage" Then
                    dr(j) = pFilter1.Rows(i).Item("Usage " & tempDatatable.Columns(j).ColumnName)
                Else
                    dr(j) = pFilter1.Rows(i).Item("EndingBalance " & tempDatatable.Columns(j).ColumnName)
                End If

            Next

            tempDatatable.Rows.Add(dr)
        Next

        Return tempDatatable

    End Function

    Function GetDailyHourlySalesReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pFilter1 As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                'With .Add("StoreName", SqlDbType.Bit)
                '    .Expression = "FOWSTNAME"
                'End With

#If DEBUG Then
                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "CONVERT(date, InvoiceDate, 104)"
                End With
#Else
                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "FORMAT(InvoiceDate, 'dd/MMM')"
                End With
#End If
                With .Add("Time", SqlDbType.Bit)
                    .Expression = "CONVERT(varchar, InvoiceHour) + ':00'"
                End With

                With .Add("GrossSales", SqlDbType.Int)
                    .Expression = "SUM(GrossInvoiceAmount)"
                End With

                With .Add("TC", SqlDbType.Bit) 'transaction count
                    .Expression = "SUM(TransactionCount)"
                End With

                With .Add("AC", SqlDbType.Int) 'average count
                    .Expression = "SUM(GrossInvoiceAmount) / SUM(TransactionCount)"
                End With

                With .Add("HourOfDayInInteger", SqlDbType.Bit)
                    .Expression = "InvoiceHour"
                End With
            End With

            .SqlFrom = " FROM CacheInvoicesHourly " & vbCrLf

            .SqlWhere = " WHERE convert(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            If Not FOWSTID = vbNullString Then
                .SqlWhere &= " AND FowstID = " & ReadyString(FOWSTID)
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= "AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

#If DEBUG Then
            .SqlGroup = "GROUP BY " & vbCrLf &
                        " CONVERT(date, InvoiceDate, 104), InvoiceHour " & vbCrLf ', FOWSTNAME 

            .SqlOrder = " Order by CONVERT(date, InvoiceDate, 104) "
#Else
            .SqlGroup = "GROUP BY " & vbCrLf &
                        " FORMAT(InvoiceDate, 'dd/MMM'), InvoiceHour " & vbCrLf ', FOWSTNAME 

            .SqlOrder = " Order by FORMAT(InvoiceDate, 'dd/MMM') "
#End If


            pDT = .Requery


            'pFilter1 = CreateCrossTab(pDT, {"StoreName", "TransactionDate"}, {New CrossColumn("Gross Sales " & vbCrLf & "{Time}", "GrossSales", "GrossSales")})
            'New CrossColumn("{Time}", "TC", "TC"),
            'New CrossColumn("{Time}", "AC", "AC")})

            pFilter1 = CreateCrossTab(pDT, {"Time", "HourOfDayInInteger"}, {New CrossColumn("Gross Sales " & vbCrLf & "{TransactionDate}", "GrossSales", "GrossSales"),
                                                                            New CrossColumn("TC " & vbCrLf & "{TransactionDate}", "TC", "TC"),
                                                                            New CrossColumn("AC " & vbCrLf & "{TransactionDate}", "AC", "AC")})
        End With

        pFilter1.DefaultView.Sort = "HourOfDayInInteger"

        pFilter1 = pFilter1.DefaultView.ToTable

        pFilter1.Columns.Remove("HourOfDayInInteger")
        pFilter1.Columns.Remove("GrossSales")
        pFilter1.Columns.Remove("TC")
        pFilter1.Columns.Remove("AC")

        For i As Integer = 0 To pFilter1.Rows.Count - 1
            If Len(CStr2(pFilter1.Rows(i).Item("Time"))) = 4 Then
                pFilter1.Rows(i).Item("Time") = "0" & pFilter1.Rows(i).Item("Time")
            End If
        Next

        Return pFilter1

    End Function

    Function GetHourlySalesByWeekCategoryReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pFilter1 As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields

                With .Add("Time", SqlDbType.Bit)
                    .Expression = "CONVERT(varchar, InvoiceHour) + ':00'"
                End With

                With .Add("GrossSales", SqlDbType.Int)
                    .Expression = "SUM(GrossSalesNetOfVATExemptions)"
                End With

                With .Add("HourOfDayInInteger", SqlDbType.Bit)
                    .Expression = "InvoiceHour"
                End With

                With .Add("WeekDaySales", SqlDbType.Bit)
                    .Expression = "ISNULL(tblWeekday.GrossSales,0)"
                End With

                With .Add("WeekDayNoOfDays", SqlDbType.Int)
                    .Expression = "ISNULL(tblWeekdayNoOfDays.NoOfDays,0)"
                End With

                With .Add("WeekEndSales", SqlDbType.Bit)
                    .Expression = "ISNULL(tblWeekEnd.GrossSales,0)"
                End With

                With .Add("WeekEndNoOfDays", SqlDbType.Int)
                    .Expression = "ISNULL(tblWeekEndNoOfDays.NoOfDays,0)"
                End With
            End With

            .SqlFrom = " FROM CacheInvoicesHourly " & vbCrLf &
                        "   OUTER APPLY ( " & vbCrLf &
                        "   SELECT SUM(GrossSalesNetOfVATExemptions) AS [GrossSales]" & vbCrLf &
                        "   FROM CacheInvoicesHourly Cacheinvoices_3" & vbCrLf &
                        " WHERE convert(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            If Not FOWSTID = vbNullString Then
                .SqlFrom &= " AND FowstID = " & ReadyString(FOWSTID)
            End If

            If Not GroupID = vbNullString Then
                .SqlFrom &= "AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            .SqlFrom &= "   and Cacheinvoices_3.InvoiceHour = CacheInvoicesHourly.InvoiceHour " & vbCrLf &
                        "   and (DATEPART(weekday, Cacheinvoices_3.InvoiceDate) = 1 OR DATEPART(weekday, Cacheinvoices_3.InvoiceDate) = 6 OR DATEPART(weekday, Cacheinvoices_3.InvoiceDate) = 7)" & vbCrLf &
                        "    ) tblWeekEnd" & vbCrLf &
                        "   OUTER APPLY ( " & vbCrLf &
                        "   SELECT SUM(GrossSalesNetOfVATExemptions) AS [GrossSales] " & vbCrLf &
                        "   FROM CacheInvoicesHourly Cacheinvoices_2 " & vbCrLf &
                        " WHERE convert(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            If Not FOWSTID = vbNullString Then
                .SqlFrom &= " AND FowstID = " & ReadyString(FOWSTID)
            End If

            If Not GroupID = vbNullString Then
                .SqlFrom &= "AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            .SqlFrom &= "   and Cacheinvoices_2.InvoiceHour = CacheInvoicesHourly.InvoiceHour " & vbCrLf &
                        "   AND DATEPART(weekday, Cacheinvoices_2.InvoiceDate) between 2 and 5 " & vbCrLf &
                        "   ) tblWeekday " & vbCrLf &
                        "   OUTER APPLY ( " & vbCrLf &
                        "   SELECT COUNT(DISTINCT convert(date,Cacheinvoices_4.InvoiceDate)) NoOfDays " & vbCrLf &
                        "   FROM CacheInvoicesHourly Cacheinvoices_4 " & vbCrLf &
                        "   WHERE convert(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        "   AND DATEPART(weekday, Cacheinvoices_4.InvoiceDate) between 2 and 5 " & vbCrLf &
                        "   ) tblWeekdayNoOfDays " & vbCrLf &
                        "   OUTER APPLY ( " & vbCrLf &
                        "   SELECT COUNT(DISTINCT convert(date,Cacheinvoices_5.InvoiceDate)) NoOfDays " & vbCrLf &
                        "   FROM CacheInvoicesHourly Cacheinvoices_5 " & vbCrLf &
                        "   WHERE convert(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        "   and (DATEPART(weekday, Cacheinvoices_5.InvoiceDate) = 1 OR DATEPART(weekday, Cacheinvoices_5.InvoiceDate) = 6 OR DATEPART(weekday, Cacheinvoices_5.InvoiceDate) = 7) " & vbCrLf &
                        "   ) tblWeekEndNoOfDays " & vbCrLf

            .SqlWhere = " WHERE convert(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            If Not FOWSTID = vbNullString Then
                .SqlWhere &= " AND FowstID = " & ReadyString(FOWSTID)
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= "AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            .SqlGroup = "GROUP BY " & vbCrLf &
                        " InvoiceHour " & vbCrLf &
                        " , ISNULL(tblWeekEnd.GrossSales,0) " & vbCrLf &
                        " , ISNULL(tblWeekday.GrossSales,0) " & vbCrLf &
                        " , ISNULL(tblWeekdayNoOfDays.NoOfDays,0) " & vbCrLf &
                        " , ISNULL(tblWeekEndNoOfDays.NoOfDays,0) " & vbCrLf

            .SqlOrder = " Order by InvoiceHour "

            pDT = .Requery


            For i As Integer = 0 To pDT.Rows.Count - 1
                If Len(CStr2(pDT.Rows(i).Item("Time"))) = 4 Then
                    pDT.Rows(i).Item("Time") = "0" & pDT.Rows(i).Item("Time")
                End If
            Next

        End With


        Return pDT

    End Function

    Function GetDisplayDailyProductMixReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransactionDate", SqlDbType.VarChar)
                    .Expression = "InvoiceDate"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "ProductName"
                End With
                With .Add("TotalSales", SqlDbType.VarChar)
                    .Expression = "SUM(ProductAmount)"
                End With

                With .Add("TransactionCount", SqlDbType.VarChar)
                    .Expression = "SUM(InvoiceCount)"
                End With

                With .Add("ProductCount", SqlDbType.VarChar)
                    .Expression = "SUM(ProductQuantity)"
                End With

                'With .Add("StoreCount", SqlDbType.VarChar)
                '    .Expression = "count(distinct fowstid) "
                'End With

                'With .Add("ADQ", SqlDbType.VarChar)
                '    .Expression = "SUM(OrderedQuantity) /7/count(distinct fowstid)"
                'End With
            End With

            .SqlFrom = "FROM CacheInvoiceFinishedProducts " & vbCrLf

            .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'" & vbCrLf

            If Not CStr2(Common.WebRequest.Data("ProductID")) = vbNullString Then
                .SqlWhere += " AND ProductID IN( " & CStr2(Common.WebRequest.Data("ProductID")) & ")" & vbCrLf
            End If

            .SqlGroup = "GROUP BY ProductName, InvoiceDate " & vbCrLf

            .SqlOrder = "ORDER BY InvoiceDate, ProductName"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetDailyProductMixPerStoreReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("FOWSTCode", SqlDbType.VarChar)
                    .Expression = "ISNULL(FOWSTCode,'')"
                End With

                With .Add("FowstName", SqlDbType.VarChar)
                    .Expression = "ISNULL(FowstName,'')"
                End With

                With .Add("Ownership", SqlDbType.VarChar)
                    .Expression = "ISNULL(FromFranchisee,'false')"
                End With

                With .Add("GroupName", SqlDbType.VarChar)
                    .Expression = "ISNULL(GroupName,'')"
                End With

                With .Add("StoreLocation", SqlDbType.VarChar)
                    .Expression = "ISNULL(StoreLocation,'')"
                End With

                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "ProductName"
                End With

                With .Add("TotalSales", SqlDbType.VarChar)
                    .Expression = "SUM(ProductAmount)/1.12"
                End With

                With .Add("QtySold", SqlDbType.VarChar)
                    .Expression = "SUM(ProductQuantity)"
                End With

                With .Add("ProductCount", SqlDbType.VarChar)
                    .Expression = "SUM(ProductQuantity)"
                End With

                With .Add("OperatingDays", SqlDbType.VarChar)
                    .Expression = "COUNT(distinct convert(date,InvoiceDate))"
                End With

            End With

            .SqlFrom = "FROM CacheInvoiceFinishedProducts " & vbCrLf

            .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'" & vbCrLf

            If Not FOWSTID = vbNullString Then
                If FOWSTID = "All" Then
                    .SqlWhere &= " AND FOWSTID IS NOT NULL " & vbCrLf
                Else
                    .SqlWhere &= " AND FOWSTID IN ('" & FOWSTID & "')" & vbCrLf
                End If

            End If

            If Not GroupID = vbNullString Then
                'check if regional group settings
                Dim pSql As String
                Dim dtRegionalGroup As DataTable

                dtRegionalGroup = Common.WebRequest.ProData.Execute("Select GroupID from LibraryRegionalGroupSettingsDetails WHERE RegionalGroupID IN (" & GroupID & ")")

                If dtRegionalGroup.Rows.Count > 0 Then
                    .SqlWhere &= " AND FranchiseGroupID IN (Select GroupID from LibraryRegionalGroupSettingsDetails WHERE RegionalGroupID IN (" & GroupID & "))" & vbCrLf
                Else
                    If GroupID = "All" Then
                        .SqlWhere &= " AND FranchiseGroupID IS NOT NULL " & vbCrLf
                    Else
                        .SqlWhere &= " AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
                    End If
                End If

            End If

            If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
                .SqlWhere += " AND StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
            End If

            If Not CStr2(Common.WebRequest.Data("ProductID")) = vbNullString Then
                .SqlWhere += " AND ProductID IN( " & CStr2(Common.WebRequest.Data("ProductID")) & ")" & vbCrLf
            End If

            .SqlGroup = "GROUP BY FOWSTCode, FowstName, FromFranchisee, GroupName,  StoreLocation, ProductName " & vbCrLf

            '.SqlOrder = "ORDER BY InvoiceDate, ProductName"

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetFlavorMonitoringReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            With .Fields
#If DEBUG Then
                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "CONVERT(date, InvoiceDate, 104)"
                End With
#Else
                With .Add("TransactionDate", SqlDbType.Bit)
                    .Expression = "FORMAT(InvoiceDate, 'dd/MMM')"
                End With
#End If
                'With .Add("TransactionDate", SqlDbType.VarChar)
                '    .Expression = "CONVERT(date,InvoiceDate)"
                'End With
                With .Add("StoreCode", SqlDbType.VarChar)
                    .Expression = "FOWSTCode"
                End With
                With .Add("StoreName", SqlDbType.VarChar)
                    .Expression = "FowstName"
                End With
                With .Add("Ownership", SqlDbType.VarChar)
                    .Expression = "CASE WHEN GroupName like '%CO%' then 'CO' ELSE 'FO' END"
                End With
                With .Add("Region", SqlDbType.VarChar)
                    .Expression = "Replace(Replace(GroupName, 'CO - ',''), 'FO - ','')"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "ModifierProductName"
                End With
                With .Add("TotalSales", SqlDbType.VarChar)
                    .Expression = "SUM(ISNULL(ModifierAmount,0))"
                End With

                With .Add("TransactionCount", SqlDbType.VarChar)
                    .Expression = "SUM(InvoiceCount)"
                End With

                With .Add("ProductCount", SqlDbType.VarChar)
                    .Expression = "SUM(ModifierQuantity)"
                End With

                With .Add("StoreCount", SqlDbType.VarChar)
                    .Expression = "count(distinct fowstid) "
                End With

                'With .Add("StoreLocation", SqlDbType.VarChar)
                '    .Expression = "'-'"
                'End With
            End With

            .SqlFrom = "FROM CacheInvoiceFlavors " & vbCrLf

            .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991' AND ISNULL(ModifierProductName,'') <> ''"

            If Not FOWSTID = vbNullString Then
                If FOWSTID = "All" Then
                    .SqlWhere &= " AND FOWSTID IS NOT NULL " & vbCrLf
                Else
                    .SqlWhere &= " AND FOWSTID IN (" & FOWSTID & ")" & vbCrLf
                End If

            End If

            If Not GroupID = vbNullString Then
                'check if regional group settings
                Dim pSql As String
                Dim dtRegionalGroup As DataTable

                dtRegionalGroup = Common.WebRequest.ProData.Execute("Select GroupID from LibraryRegionalGroupSettingsDetails WHERE RegionalGroupID IN (" & GroupID & ")")

                If dtRegionalGroup.Rows.Count > 0 Then
                    .SqlWhere &= " AND FranchiseGroupID IN (Select GroupID from LibraryRegionalGroupSettingsDetails WHERE RegionalGroupID IN (" & GroupID & "))" & vbCrLf
                Else
                    If GroupID = "All" Then
                        .SqlWhere &= " AND FranchiseGroupID IS NOT NULL " & vbCrLf
                    Else
                        .SqlWhere &= " AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
                    End If
                End If

            End If

            'If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
            '    .SqlWhere += " AND StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
            'End If

            If CStr2(Common.WebRequest.Data("Ownership")) = "CompanyOwnedStores" Then
                .SqlWhere += " AND ISNULL(FromFranchisee,0) = 0 " & vbCrLf
            ElseIf CStr2(Common.WebRequest.Data("Ownership")) = "FranchiseeStores" Then
                .SqlWhere += " AND FromFranchisee = 1 " & vbCrLf
            End If

            .SqlGroup = "GROUP BY ModifierProductName, FOWSTCode, FowstName, GroupName, CONVERT(date,InvoiceDate) " & vbCrLf

            .SqlOrder = "ORDER BY CONVERT(date,InvoiceDate), FowstName "

            pDT = .Requery

            If Not CStr2(Common.WebRequest.Data("ReportType")) = vbNullString Then

                If CStr2(Common.WebRequest.Data("ReportType")) = "Sales" Then
                    pFilter1 = CreateCrossTab(pDT, {"TransactionDate", "StoreCode", "StoreName", "Ownership", "Region"}, {New CrossColumn("{ProductName}", "TotalSales", "TotalSales")})
                    pFilter1.Columns.Remove("TotalSales")
                ElseIf CStr2(Common.WebRequest.Data("ReportType")) = "Quantity" Then
                    pFilter1 = CreateCrossTab(pDT, {"TransactionDate", "StoreCode", "StoreName", "Ownership", "Region"}, {New CrossColumn("{ProductName}", "ProductCount", "ProductCount")})
                    pFilter1.Columns.Remove("ProductCount")
                ElseIf CStr2(Common.WebRequest.Data("ReportType")) = "TransactionCount" Then
                    pFilter1 = CreateCrossTab(pDT, {"TransactionDate", "StoreCode", "StoreName", "Ownership", "Region"}, {New CrossColumn("{ProductName}", "TransactionCount", "TransactionCount")})
                    pFilter1.Columns.Remove("TransactionCount")
                End If
            End If

        End With


        Dim pSourceURL As String = Common.ServerPath & "temp\FlavorMonitoringReport\"
        Dim strFileName As String = "Flavor Monitoring Report - " & DateFrom & " - " & DateTo & ".xls"

        If (IO.File.Exists(pSourceURL & strFileName)) Then
            IO.File.Delete(pSourceURL & strFileName)
        End If

        TableToExcelFile(pFilter1, "", "", pSourceURL & "\" & strFileName)

        Return pFilter1
    End Function

    Function GetProductMonitoringReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransactionDate", SqlDbType.VarChar)
                    .Expression = "CONVERT(date,InvoiceDate)"
                End With
                With .Add("StoreCode", SqlDbType.VarChar)
                    .Expression = "FOWSTCode"
                End With
                With .Add("StoreName", SqlDbType.VarChar)
                    .Expression = "FowstName"
                End With
                With .Add("Ownership", SqlDbType.VarChar)
                    .Expression = "CASE WHEN GroupName like '%CO%' then 'CO' ELSE 'FO' END"
                End With
                With .Add("Region", SqlDbType.VarChar)
                    .Expression = "Replace(Replace(GroupName, 'CO - ',''), 'FO - ','')"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "ProductName"
                End With
                With .Add("TotalSales", SqlDbType.VarChar)
                    .Expression = "SUM(ProductAmount)"
                End With

                With .Add("TransactionCount", SqlDbType.VarChar)
                    .Expression = "SUM(InvoiceCount)"
                End With

                With .Add("ProductCount", SqlDbType.VarChar)
                    .Expression = "SUM(ProductQuantity)"
                End With

                With .Add("StoreCount", SqlDbType.VarChar)
                    .Expression = "count(distinct fowstid) "
                End With

                With .Add("StoreLocation", SqlDbType.VarChar)
                    .Expression = "ISNULL(StoreLocation,'')"
                End With
            End With

            .SqlFrom = "FROM CacheInvoiceFinishedProducts " & vbCrLf

            .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'"

            If Not FOWSTID = vbNullString Then
                If FOWSTID = "All" Then
                    .SqlWhere &= " AND FOWSTID IS NOT NULL " & vbCrLf
                Else
                    .SqlWhere &= " AND FOWSTID IN (" & FOWSTID & ")" & vbCrLf
                End If

            End If

            If Not GroupID = vbNullString Then
                'check if regional group settings
                Dim pSql As String
                Dim dtRegionalGroup As DataTable

                dtRegionalGroup = Common.WebRequest.ProData.Execute("Select GroupID from LibraryRegionalGroupSettingsDetails WHERE RegionalGroupID IN (" & GroupID & ")")

                If dtRegionalGroup.Rows.Count > 0 Then
                    .SqlWhere &= " AND FranchiseGroupID IN (Select GroupID from LibraryRegionalGroupSettingsDetails WHERE RegionalGroupID IN (" & GroupID & "))" & vbCrLf
                Else
                    If GroupID = "All" Then
                        .SqlWhere &= " AND FranchiseGroupID IS NOT NULL " & vbCrLf
                    Else
                        .SqlWhere &= " AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
                    End If
                End If

            End If

            If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
                .SqlWhere += " AND StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
            End If

            If CStr2(Common.WebRequest.Data("Ownership")) = "CompanyOwnedStores" Then
                .SqlWhere += " AND ISNULL(FromFranchisee,0) = 0 " & vbCrLf
            ElseIf CStr2(Common.WebRequest.Data("Ownership")) = "FranchiseeStores" Then
                .SqlWhere += " AND FromFranchisee = 1 " & vbCrLf
            End If

            'If CStr2(Common.WebRequest.Data("Ownership")) <> "" Then
            '    If CStr2(Common.WebRequest.Data("Ownership")) = "CompanyOwnedStores" Then
            '        .SqlWhere += " AND ISNULL(FromFranchisee,0) = 0" & vbCrLf
            '    ElseIf CStr2(Common.WebRequest.Data("Ownership")) = "" Then
            '        .SqlWhere += " AND ISNULL(FromFranchisee,0) = 1" & vbCrLf
            '    End If
            'End If

            .SqlGroup = "GROUP BY ProductName, FOWSTCode, FowstName, GroupName, StoreLocation, CONVERT(date,InvoiceDate) " & vbCrLf

            .SqlOrder = "ORDER BY CONVERT(date,InvoiceDate), FowstName "

            pDT = .Requery

            If Not CStr2(Common.WebRequest.Data("ReportType")) = vbNullString Then

                If CStr2(Common.WebRequest.Data("ReportType")) = "Sales" Then
                    pFilter1 = CreateCrossTab(pDT, {"TransactionDate", "StoreCode", "StoreName", "StoreLocation", "Ownership", "Region"}, {New CrossColumn("{ProductName}", "TotalSales", "TotalSales")})
                    pFilter1.Columns.Remove("TotalSales")
                ElseIf CStr2(Common.WebRequest.Data("ReportType")) = "Quantity" Then
                    pFilter1 = CreateCrossTab(pDT, {"TransactionDate", "StoreCode", "StoreName", "StoreLocation", "Ownership", "Region"}, {New CrossColumn("{ProductName}", "ProductCount", "ProductCount")})
                    pFilter1.Columns.Remove("ProductCount")
                ElseIf CStr2(Common.WebRequest.Data("ReportType")) = "TransactionCount" Then
                    pFilter1 = CreateCrossTab(pDT, {"TransactionDate", "StoreCode", "StoreName", "StoreLocation", "Ownership", "Region"}, {New CrossColumn("{ProductName}", "TransactionCount", "TransactionCount")})
                    pFilter1.Columns.Remove("TransactionCount")
                End If
            End If
        End With

        Dim pSourceURL As String = Common.ServerPath & "temp\ProductMonitoringReport\"
        Dim strFileName As String = "Product Monitoring Report - " & DateFrom & " - " & DateTo & ".xls"

        If (IO.File.Exists(pSourceURL & strFileName)) Then
            IO.File.Delete(pSourceURL & strFileName)
        End If

        TableToExcelFile(pFilter1, "", "", pSourceURL & "\" & strFileName)

        Return pFilter1
    End Function

    Function GetProductMonitoringPerStoreProductReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransactionDate", SqlDbType.VarChar)
                    .Expression = "CONVERT(date,InvoiceDate)"
                End With

                With .Add("StoreCode", SqlDbType.VarChar)
                    .Expression = "FOWSTCode"
                End With

                With .Add("StoreName", SqlDbType.VarChar)
                    .Expression = "FowstName"
                End With

                With .Add("Region", SqlDbType.VarChar)
                    .Expression = "GroupName"
                End With

                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "ProductNo"
                End With

                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "ProductName"
                End With

                With .Add("TotalSales", SqlDbType.VarChar)
                    .Expression = "SUM(ProductAmount)"
                End With

                With .Add("TransactionCount", SqlDbType.VarChar)
                    .Expression = "SUM(InvoiceCount)"
                End With

                With .Add("ProductCount", SqlDbType.VarChar)
                    .Expression = "SUM(ProductQuantity)"
                End With

                With .Add("StoreLocation", SqlDbType.VarChar)
                    .Expression = "ISNULL(StoreLocation,'')"
                End With
            End With

            .SqlFrom = "FROM CacheInvoiceFinishedProducts " & vbCrLf

            .SqlWhere = " WHERE CONVERT(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'"

            If Not FOWSTID = vbNullString Then
                .SqlWhere &= " AND FOWSTID IN (" & FOWSTID & ")" & vbCrLf
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= "AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
                .SqlWhere += " AND StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
            End If

            If Not CStr2(Common.WebRequest.Data("ProductID")) = vbNullString Then
                .SqlWhere += " AND ProductID IN( " & CStr2(Common.WebRequest.Data("ProductID")) & ")" & vbCrLf
            End If

            .SqlGroup = "GROUP BY ProductNo, ProductName, FOWSTCode, FowstName, GroupName, StoreLocation, CONVERT(date,InvoiceDate) " & vbCrLf

            .SqlOrder = "ORDER BY CONVERT(date,InvoiceDate), FowstName "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProductProductMonitoringMonthlySummary() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pFilter1 As DataTable

        pFinder = New Finder

        With pFinder

            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ProductCode", SqlDbType.VarChar)
                    .Expression = "ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "ProductName"
                End With
                With .Add("TotalSales", SqlDbType.VarChar)
                    .Expression = "SUM(ProductAmount)"
                End With
                With .Add("TransactionCount", SqlDbType.VarChar)
                    .Expression = "SUM(InvoiceCount)"
                End With

                With .Add("ProductCount", SqlDbType.VarChar)
                    .Expression = "SUM(ProductQuantity)"
                End With

                With .Add("Month", SqlDbType.VarChar)
                    .Expression = "FORMAT(InvoiceDate, 'MMM')"
                End With

                With .Add("MonthInt", SqlDbType.VarChar)
                    .Expression = "Month(InvoiceDate)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoiceFinishedProducts " & vbCrLf

            .SqlWhere = " WHERE YEAR(Invoicedate) = " & Year(Now) & vbCrLf &
                        " AND FranchiseGroupID <> 'C68F3E5E-BF1C-4208-B849-87CA5CBCD991'"

            .SqlGroup = "GROUP BY ProductName, ProductNo, FORMAT(InvoiceDate, 'MMM'), Month(InvoiceDate) " & vbCrLf

            .SqlOrder = "ORDER BY MonthInt, ProductName, ProductCode "

            pDT = .Requery

            If Not CStr2(Common.WebRequest.Data("ReportType")) = vbNullString Then

                If CStr2(Common.WebRequest.Data("ReportType")) = "Sales" Then
                    pFilter1 = CreateCrossTab(pDT, {"ProductCode", "ProductName"}, {New CrossColumn("{Month}", "TotalSales", "TotalSales")})
                    pFilter1.Columns.Remove("TotalSales")
                ElseIf CStr2(Common.WebRequest.Data("ReportType")) = "Quantity" Then
                    pFilter1 = CreateCrossTab(pDT, {"ProductCode", "ProductName"}, {New CrossColumn("{Month}", "ProductCount", "ProductCount")})
                    pFilter1.Columns.Remove("ProductCount")
                ElseIf CStr2(Common.WebRequest.Data("ReportType")) = "TransactionCount" Then
                    pFilter1 = CreateCrossTab(pDT, {"ProductCode", "ProductName"}, {New CrossColumn("{Month}", "TransactionCount", "TransactionCount")})
                    pFilter1.Columns.Remove("TransactionCount")
                End If
            End If
        End With

        Return pFilter1
    End Function

    Function GetAverageDailySales() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pData As New DataTable
        Dim mDateFrom As Date
        Dim mDateTo As Date
        Dim newDateTo As Date

        pData.Columns.Add("DateFrom")
        pData.Columns.Add("DateTo")

        pData.Columns.Add("Monday")
        pData.Columns.Add("Tuesday")
        pData.Columns.Add("Wednesday")
        pData.Columns.Add("Thursday")
        pData.Columns.Add("Friday")
        pData.Columns.Add("Saturday")
        pData.Columns.Add("Sunday")

        'split date range
        mDateFrom = CDate(DateFrom)
        mDateTo = CDate(DateTo)

        While mDateFrom <= mDateTo

            Dim dr As DataRow

            newDateTo = GetNextWeekday(mDateFrom, DayOfWeek.Monday)

            dr = pData.NewRow

            If newDateTo > mDateTo Then
                newDateTo = mDateTo
            End If

            dr(0) = CStr2(mDateFrom)
            dr(1) = CStr2(newDateTo)

            pFinder = New Finder

            'If IsFranchisor() Then
            With pFinder
                .ProData = Common.WebRequest.ProData
                .Distinct = True

                With .Fields

                    With .Add("Monday", SqlDbType.VarChar)
                        .Expression = "Monday"
                    End With

                    With .Add("Tuesday", SqlDbType.VarChar)
                        .Expression = "Tuesday"
                    End With

                    With .Add("Wednesday", SqlDbType.Decimal)
                        .Expression = "Wednesday"
                    End With

                    With .Add("Thursday", SqlDbType.VarChar)
                        .Expression = "Thursday"
                    End With

                    With .Add("Friday", SqlDbType.VarChar)
                        .Expression = "Friday"
                    End With

                    With .Add("Saturday", SqlDbType.VarChar)
                        .Expression = "Saturday"
                    End With

                    With .Add("Sunday", SqlDbType.VarChar)
                        .Expression = "Sunday"
                    End With

                End With

                .SqlFrom = "FROM " & vbCrLf &
                    "(select datename(WEEKDAY,InvoiceDate) Day, SUM(CacheInvoices.GrossSalesNetOfVATExemptions) Subtotal from CacheInvoices " & vbCrLf &
                        "--inner join TransInvoice on TransInvoice.ID=TransInvoiceItems.InvoiceID " & vbCrLf &
                        "WHERE convert(date,InvoiceDate) between " & ReadyString(mDateFrom) & " AND " & ReadyString(newDateTo) & vbCrLf &
                        "group by datename(WEEKDAY,InvoiceDate) " & vbCrLf &
                        ") p PIVOT(SUM(Subtotal) " & vbCrLf &
                        "FOR DAY IN ( [Monday], [Tuesday], [Wednesday], [Thursday], [Friday], [Saturday], [Sunday])) as DayOfTheWeek "

                'If New SecurityRole().CheckSecurityCurrentUserAcccess("On the Dashbooards and Reports Main Page, user is allowed to view all main page information?", Common.WebRequest.Data("OrgID"), WebSession.Instance.PeopleUserID).Rows.Count = 0 Then
                '    .SqlWhere += " AND franchisgroupMain.ID IN (SELECT FranchiseGroupID FROM LibraryFranchiseGroupHeads WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & ") "
                'End If

                '.SqlGroup = "GROUP BY franchisgroupMain.ID, franchisgroupMain.GroupName"

                pDT = .Requery

            End With
            'End If

            If pDT.Rows.Count > 0 Then
                dr(2) = CDec2(pDT.Rows(0).Item("Monday"))
                dr(3) = CDec2(pDT.Rows(0).Item("Tuesday"))
                dr(4) = CDec2(pDT.Rows(0).Item("Wednesday"))
                dr(5) = CDec2(pDT.Rows(0).Item("Thursday"))
                dr(6) = CDec2(pDT.Rows(0).Item("Friday"))
                dr(7) = CDec2(pDT.Rows(0).Item("Saturday"))
                dr(8) = CDec2(pDT.Rows(0).Item("Sunday"))
            End If

            pData.Rows.Add(dr)

            mDateFrom = newDateTo.AddDays(1)
        End While

        Return pData
    End Function

    Function GetTopAndBottomRankingStores(IsTop As Boolean) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pData As New DataTable
        Dim pSort As String
        Dim NoOfDays As Integer

        If IsTop Then
            pSort = "DESC"
        Else
            pSort = "ASC"
        End If

        NoOfDays = (CDate(DateTo) - CDate(DateFrom)).Days + 1

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True
            .MaxRecords = 5

            With .Fields
                With .Add("RowNum", SqlDbType.VarChar)
                    .Expression = "ROW_NUMBER() OVER (ORDER BY ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0) " & pSort & ") "
                End With

                With .Add("FOWSTID", SqlDbType.VarChar)
                    .Expression = "CacheInvoices.FowstID"
                End With
                With .Add("FOWSTName", SqlDbType.VarChar)
                    .Expression = "CacheInvoices.FOWSTName"
                End With

                With .Add("GrossSales", SqlDbType.Decimal)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With

                With .Add("ADS", SqlDbType.Decimal)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0) / " & NoOfDays
                End With

            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = "WHERE CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            '.SqlFrom = "FROM " & vbCrLf &
            '    "TransInvoiceItems inner join TransInvoice " & vbCrLf &
            '    "on TransInvoiceItems.InvoiceID = transinvoice.ID " & vbCrLf &
            '    "inner join MobizLibraryOrganizationFOWST  " & vbCrLf &
            '    "on MobizLibraryOrganizationFOWST.ID = TransInvoice.FOWSTID " & vbCrLf

            '.SqlWhere = "WHERE (Transinvoice.InvoiceNumber>0 OR Transinvoice.FranchiseeInvoiceNumber>0) " & vbCrLf &
            '            "AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            'If Not GroupID = vbNullString Then

            If CBool2(Common.WebRequest.Data("IsGroupStore")) Then
                '.SqlFrom &= "INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                '            "INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf

                '.SqlWhere &= " AND MobizLibraryOrganizationFranchisee.FranchiseGroupID = " & ReadyString(GroupID) & vbCrLf

                .SqlWhere &= " AND CacheInvoices.FranchiseGroupID = " & ReadyString(GroupID) & vbCrLf
            Else
                .SqlWhere &= " AND FranchiseGroupID IN (SELECT FranchiseGroupID FROM LibraryFranchiseGroupHeads WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & ") "
            End If

            .SqlGroup = "GROUP BY CacheInvoices.FowstID, CacheInvoices.FOWSTName " & vbCrLf

            .SqlOrder = "Order By GrossSales " & pSort

            pDT = .Requery

        End With
        'End If

        Return pDT
    End Function

    Function IsFranchisor() As Boolean
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "Select * from LibraryFranchiseGroup"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            pSql = "Select * from MobizLibraryOrganizationFranchisee  WHERE FranchisorOrgID = " & ReadyString(Common.WebRequest.Data("OrgID"))
            pDT = Common.WebRequest.ProData.Execute(pSql)
            If pDT.Rows.Count > 0 Then
                Return True
            End If
        End If

        Return False

    End Function

    Public Function GetNextWeekday(start As DateTime, day As DayOfWeek) As DateTime
        ' The (... + 7) % 7 ensures we end up with a value in the range [0, 6]

        Dim daysToAdd As Integer
        'daysToAdd = ((int) day - (int) start.DayOfWeek + 7) % 7;

        daysToAdd = (day - start.DayOfWeek + 7) Mod 7

        Return start.AddDays(daysToAdd)
    End Function

    Function GetSalesReportSliceAndDice() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlGroupBy As String
        Dim SqlFrom As String
        Dim mColumn As String
        Dim SqlWhere As String

        Dim pColumn As String
        Dim pTableName As String
        Dim dashboardsandreports As New DashBoardsAndReports

        If Not DiceBy = vbNullString Then
            mColumn = DiceBy
        Else
            mColumn = SliceBy
        End If

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields

                If Not DiceBy = vbNullString OrElse Not SliceBy = vbNullString Then

                    Select Case mColumn
                        Case "Brand"
                            With .Add("ProductFeature", SqlDbType.VarChar)
                                .Expression = "MobizLibraryPIMProduct.Brand"
                            End With
                            SqlGroupBy &= " MobizLibraryPIMProduct.Brand"
                        Case "DivisionDescription"
                            With .Add("ProductFeature", SqlDbType.VarChar)
                                .Expression = "MobizLibraryPIMProduct.DivisionDescription"
                            End With
                            SqlGroupBy &= ", MobizLibraryPIMProduct.DivisionDescription"
                        Case "SalesRepresentative"
                            With .Add("ProductFeature", SqlDbType.VarChar)
                                .Expression = "ISNULL(CASE WHEN MobizLibraryPeople.SurName = '' THEN 'N/A' ELSE MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName END,'N/A')"
                            End With
                            SqlGroupBy &= ", MobizLibraryPeople.SurName, MobizLibraryPeople.FirstName "
                        Case "Product"
                            With .Add("ProductFeature", SqlDbType.VarChar)
                                .Expression = "ISNULL(dbo.MobizLibraryPIMProduct.ProductName, 'N/A')"
                            End With
                            SqlGroupBy &= " MobizLibraryPIMProduct.ProductName"
                        Case "DeliveryMethod"
                            With .Add("ProductFeature", SqlDbType.VarChar)
                                .Expression = "ISNULL(TransInvoice.DeliveryMethod, 'Not Provided')"
                            End With
                            SqlGroupBy &= ", TransInvoice.DeliveryMethod"
                        Case "OrderMethod"
                            With .Add("ProductFeature", SqlDbType.VarChar)
                                .Expression = "ISNULL(dbo.TransInvoice.OrderMethod, 'Not Provided')"
                            End With
                            SqlGroupBy &= ", TransInvoice.OrderMethod"
                        Case "Gender"
                            With .Add("ProductFeature", SqlDbType.VarChar)
                                .Expression = "ISNULL(TransInvoice.CustomerDemographicGender, 'Not Provided')"
                            End With
                            SqlGroupBy &= ", TransInvoice.CustomerDemographicGender"
                        Case "Age"
                            With .Add("ProductFeature", SqlDbType.VarChar)
                                .Expression = "ISNULL(TransInvoice.CustomerDemographicAge, 'Not Provided')"
                            End With
                            SqlGroupBy &= ", TransInvoice.CustomerDemographicAge"
                        Case "Time"
                            With .Add("ProductFeature", SqlDbType.VarChar)
                                .Expression = "CONVERT(varchar, DATEPART(hh, InvoiceDate)) + ':00'"
                            End With

                            SqlGroupBy = ", DATEPART(hh, InvoiceDate)"
                        Case "Encoder"
                            With .Add("ProductFeature", SqlDbType.VarChar)
                                .Expression = "EncodedBy.Surname + ', ' + EncodedBy.FirstName"
                            End With

                            SqlFrom = "INNER JOIN MobizLibraryPeople EncodedBy on EncodedBy.ID = TransInvoice.UserID "

                            SqlGroupBy = ", EncodedBy.Surname + ', ' + EncodedBy.FirstName "
                        Case "OtherPayments"
                            With .Add("ProductFeature", SqlDbType.VarChar)
                                .Expression = "TransCustomerPaymentOther.PaymentType"
                            End With

                            SqlFrom = "inner join TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID " & vbCrLf &
                                    "inner join TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPaymentInvoices.CustomerPaymentID "

                            SqlGroupBy = ", TransCustomerPaymentOther.PaymentType "
                        Case "PaymentType"

                            SqlFrom = "INNER JOIN MobizLibraryPeople EncodedBy on EncodedBy.ID = TransInvoice.UserID "

                            SqlGroupBy = ", EncodedBy.Surname + ', ' + EncodedBy.FirstName "

                        Case "Flavor"
                            'c0092 - Flavor
                            'c0093 -Fries
                            'c0095 - Walk-in/Delivery

                            With .Add("ProductFeature", SqlDbType.VarChar)
                                .Expression = "ISNULL(MobizLibraryPIMProductVariableFeatures001.c0092,'N/A')"
                            End With

                            SqlFrom = " INNER JOIN TransInvoiceItemsModifier on TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID  " & vbCrLf &
                                        " INNER JOIN LibraryItemModifierInventoryAffecting on LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID  " & vbCrLf &
                                        " INNER JOIN MobizLibraryPIMProductVariableFeatures001 on MobizLibraryPIMProductVariableFeatures001.ProductID = LibraryItemModifierInventoryAffecting.ProductID  " & vbCrLf

                            SqlGroupBy = " MobizLibraryPIMProductVariableFeatures001.c0092 "

                            SqlGroupBy &= " ORDER BY MobizLibraryPIMProductVariableFeatures001.c0092 "

                        Case Else

                            pColumn = dashboardsandreports.GetFeatureColumnName(mColumn, pTableName)

                            pTableName = "Mobiz" & pTableName

                            SqlFrom = " LEFT JOIN " & pTableName & " pv1 ON dbo.TransInvoiceItems.ProductID = pv1.ProductID "

                            With .Add("ProductFeature", SqlDbType.VarChar)
                                '.Expression = "ISNULL(CASE WHEN dbo." & pTableName & "." & pColumn & " = '' THEN 'N/A' ELSE " & pTableName & "." & pColumn & " END,'N/A')" '"ISNULL(dbo." & pTableName & "." & pColumn & ",'N/A')"
                                .Expression = "ISNULL(CASE WHEN pv1." & pColumn & " = '' THEN 'N/A' ELSE pv1." & pColumn & " END,'N/A')"
                            End With

                            'SqlFrom = "LEFT JOIN " & pTableName & " ON dbo.TransInvoiceItems.ProductID = " & pTableName & ".ProductID "
                            'SqlGroupBy &= "ISNULL(CASE WHEN dbo." & pTableName & "." & pColumn & " = '' THEN 'N/A' ELSE " & pTableName & "." & pColumn & " END,'N/A')"

                            SqlGroupBy &= "ISNULL(CASE WHEN pv1." & pColumn & " = '' THEN 'N/A' ELSE pv1." & pColumn & " END,'N/A')"

                    End Select

                    With .Add("Total", SqlDbType.Decimal)
                        .Expression = "(CASE WHEN SUM(ISNULL(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions, 0)) = 0 THEN SUM(dbo.TransInvoiceItems.SubTotal) ELSE SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions) END)"
                    End With
                    With .Add("TransactionCount", SqlDbType.Int)
                        .Expression = "COUNT(dbo.TransInvoice.ID)"
                    End With
                    With .Add("ProductCount", SqlDbType.Decimal)
                        .Expression = "SUM(dbo.TransInvoiceItems.Quantity)"
                    End With
                    With .Add("ProductCountSO", SqlDbType.Decimal)
                        .Expression = "SUM(dbo.TransInvoiceItems.OrderedQuantity)"
                    End With

                    SqlWhere = " WHERE (dbo.TransInvoice.InvoiceNumber > 0 OR TransInvoice.FranchiseeInvoiceNumber > 0) AND CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

                    'if dili All, then it means, fowst and giselect sa user
                    If Not GroupID = "Group" AndAlso Not GroupID = "All" Then
                        SqlWhere &= " AND TransInvoice.FOWSTID = " & ReadyString(SliceByFilter)
                    ElseIf GroupID = "Group" Then
                        SqlFrom &= "INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = TransInvoice.FOWSTID " & vbCrLf &
                                    "INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID "
                        '& SqlFrom & " " & vbCrLf

                        SqlWhere &= " AND LibraryFranchiseGroup.ID = " & ReadyString(SliceByFilter)
                    End If

                    If Not DiceBy = vbNullString Then
                        Select Case SliceBy
                            Case "Time"
                                SqlWhere &= " AND DATEPART(hh, InvoiceDate) = " & ReadyString(Replace(DiceByFilter, ":00", ""))
                            Case "Brand"
                                SqlWhere &= " AND dbo.MobizLibraryPIMProduct.Brand = " & ReadyString(DiceByFilter)
                            Case "Division"
                                SqlWhere &= " AND dbo.MobizLibraryPIMProduct.DivisionDescription = " & ReadyString(DiceByFilter)
                            Case "SalesRepresentative"

                                'additional by janine oct 30
                            Case "Product"
                                SqlWhere &= " AND ISNULL(dbo.MobizLibraryPIMProduct.ProductName, '') = (CASE WHEN " & ReadyString(DiceByFilter) &
                                    " = 'N/A' THEN '' ELSE " & ReadyString(DiceByFilter) & " END) "
                            Case "DeliveryMethod"
                                SqlWhere &= " AND ISNULL(dbo.TransInvoice.DeliveryMethod, '') = (CASE WHEN " & ReadyString(DiceByFilter) &
                                    " = 'Not Provided' THEN '' ELSE " & ReadyString(DiceByFilter) & " END) "
                            Case "OrderMethod"
                                SqlWhere &= " AND ISNULL(dbo.TransInvoice.OrderMethod, '') = (CASE WHEN " & ReadyString(DiceByFilter) &
                                    " = 'Not Provided' THEN '' ELSE " & ReadyString(DiceByFilter) & " END) "
                            Case "Gender"
                                SqlWhere &= " AND ISNULL(dbo.TransInvoice.CustomerDemographicGender, '') = (CASE WHEN " & ReadyString(DiceByFilter) &
                                    " = 'Not Provided' THEN '' ELSE " & ReadyString(DiceByFilter) & " END) "
                            Case "Age"
                                SqlWhere &= " AND ISNULL(dbo.TransInvoice.CustomerDemographicAge, '') = (CASE WHEN " & ReadyString(DiceByFilter) &
                                    " = 'Not Provided' THEN '' ELSE " & ReadyString(DiceByFilter) & " END) "
                                'end of additional
                            Case "Encoder"

                                SqlWhere &= " AND EncodedBy.Surname + ', ' + EncodedBy.FirstName  LIKE " & ReadyStringWild(DiceByFilter)

                                SqlFrom &= "INNER JOIN MobizLibraryPeople EncodedBy on EncodedBy.ID = TransInvoice.UserID"

                            Case "OtherPayments"

                                SqlWhere &= " AND TransCustomerPaymentOther.PaymentType = " & ReadyString(DiceByFilter)

                                SqlFrom &= "inner join TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID " & vbCrLf &
                                        "inner join TransCustomerPaymentOther on TransCustomerPaymentOther.CustomerPaymentID = TransCustomerPaymentInvoices.CustomerPaymentID "

                            Case "Flavor"

                                SqlWhere &= " AND pv2.c0092 = " & ReadyString(DiceByFilter)

                                SqlFrom &= " INNER JOIN TransInvoiceItemsModifier on TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID  " & vbCrLf &
                                            " INNER JOIN LibraryItemModifierInventoryAffecting on LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID  " & vbCrLf &
                                            "LEFT JOIN MobizLibraryPIMProductVariableFeatures001 pv2 ON LibraryItemModifierInventoryAffecting.ProductID = pv2.ProductID "

                                'SqlFrom &= " INNER JOIN TransInvoiceItemsModifier on TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID  " & vbCrLf &
                                '            " INNER JOIN LibraryItemModifierInventoryAffecting on LibraryItemModifierInventoryAffecting.ID = TransInvoiceItemsModifier.ModifierID  " & vbCrLf

                            Case Else
                                pColumn = dashboardsandreports.GetFeatureColumnName(SliceBy, pTableName)

                                pTableName = "Mobiz" & pTableName

                                'If SqlFrom Is Nothing Then
                                SqlFrom &= "LEFT JOIN " & pTableName & " pv2 ON dbo.TransInvoiceItems.ProductID = pv2.ProductID "
                                'End If

                                If DiceByFilter = "N/A" Then
                                    SqlWhere &= " AND (pv2." & pColumn & " = " & ReadyString(DiceByFilter) & " OR " & pColumn & " = '')"
                                Else
                                    SqlWhere &= " AND pv2." & pColumn & " = " & ReadyString(DiceByFilter)
                                End If
                        End Select
                    End If
                Else

                    If Not GroupID = "Group" AndAlso Not GroupID = "All" Then
                        With .Add("ID", SqlDbType.UniqueIdentifier)
                            .Expression = "CacheInvoices.FowstID"
                        End With
                        With .Add("FOWSTOrGroupName", SqlDbType.UniqueIdentifier)
                            .Expression = "CacheInvoices.FOWSTName"
                        End With
                        With .Add("StoreType", SqlDbType.VarChar)
                            .Expression = "ISNULL(CacheInvoices.StoreType,'')"
                        End With
                        With .Add("StoreLocation", SqlDbType.VarChar)
                            .Expression = "ISNULL(CacheInvoices.StoreLocation,'')"
                        End With
                        'With .Add("GroupName", SqlDbType.Int)
                        '    .Expression = "ISNULL(CacheInvoices.StoreLocation,'')"
                        'End With

                    ElseIf GroupID = "Group" Then

                        With .Add("ID", SqlDbType.Int)
                            .Expression = "CacheInvoices.FranchiseGroupID"
                            '.Expression = "ISNULL(CacheInvoices.FranchiseGroupID,'')"
                        End With
                        With .Add("FOWSTOrGroupName", SqlDbType.Int)
                            .Expression = "ISNULL(CacheInvoices.GroupName,'')"
                        End With
                    Else
                        With .Add("ID", SqlDbType.Int)
                            .Expression = "''"
                        End With
                        With .Add("FOWSTOrGroupName", SqlDbType.Int)
                            .Expression = "'System-Wide'"
                        End With
                    End If

                    With .Add("GrossSales", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                    End With
                    With .Add("TransactionCount", SqlDbType.Bit)
                        .Expression = "ISNULL(COUNT(DISTINCT CacheInvoices.ID),0)"
                    End With
                    With .Add("DiscountAmount", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.SCDiscount),0) + ISNULL(SUM(CacheInvoices.ForegoneVAT),0)"
                    End With
                    With .Add("CashAmount", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.CashNetOfChange),0)"
                    End With
                    With .Add("CreditCardAmount", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.CreditCardAmount),0)"
                    End With
                    With .Add("DebitCardAmount", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.DebitCardAmount),0)"
                    End With
                    With .Add("CheckPaymentAmount", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.PotatoCornerGC),0)"
                    End With
                    'With .Add("OtherPayments", SqlDbType.Bit)
                    '    .Expression = "SUM(TransCustomerPayment.CheckPaymentAmount)"
                    'End With
                    With .Add("DiscountVoucher", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[Discount Voucher / On Line Deals]),0)"
                    End With
                    With .Add("E-Plus", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[E-Plus]),0)"
                    End With
                    With .Add("FoodPanda", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[FoodPanda]),0)"
                    End With
                    With .Add("GCash", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[GCash]),0)"
                    End With
                    With .Add("PayMaya", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[PayMaya]),0)"
                    End With
                    With .Add("GrabFood", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[GrabFood]),0)"
                    End With
                    With .Add("SMAC", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[SMAC]),0)"
                    End With
                    With .Add("V-Plus", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[V-Plus]),0)"
                    End With

                    With .Add("GoldCard", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[GoldCard]),0)"
                    End With

                    With .Add("ShopeeEVoucher", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[ShopeeEVoucher]),0)"
                    End With

                    With .Add("LocalDelivery", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[LocalDelivery]),0)"
                    End With

                    With .Add("ShopeePay", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[ShopeePay]),0)"
                    End With

                    'With .Add("ShopeePay", SqlDbType.Bit)
                    '    .Expression = "ISNULL(SUM(CacheInvoices.[ShopeePay]),0)"
                    'End With

                    With .Add("Eatigo", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[Eatigo]),0)"
                    End With

                    With .Add("DeliRush", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[DeliRush]),0)"
                    End With

                    With .Add("PoodTrip", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[PoodTrip]),0)"
                    End With
                    With .Add("ShareTreats", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[Share Treats]),0)"
                    End With

                    With .Add("Booky", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[Booky]),0)"
                    End With

                    With .Add("Pickaroo", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[Pickaroo]),0)"
                    End With

                    With .Add("Lazada", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.[Lazada]),0)"
                    End With

                    With .Add("LatestTransactionDate", SqlDbType.Bit)
                        .Expression = "MAX(CacheInvoices.InvoiceDate)"
                    End With

                    With .Add("SupercardVoucher", SqlDbType.Bit)
                        .Expression = "ISNULL(SUM(CacheInvoices.SupercardVoucher),0)"
                    End With

                End If

            End With

            If Not DiceBy = vbNullString OrElse Not SliceBy = vbNullString Then

                .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
                    "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                    "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID " & vbCrLf &
                    SqlFrom & vbCrLf

                .SqlWhere = SqlWhere

                .SqlGroup = "GROUP BY " & SqlGroupBy

            Else

                .SqlFrom = "FROM CacheInvoices " & vbCrLf
                '"INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = CacheInvoices.FOWSTID " & vbCrLf &
                '"INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & SqlFrom & " " & vbCrLf

                'If Not GroupID = "Group" AndAlso Not GroupID = "All" Then
                '    .SqlFrom &= "INNER JOIN MobizLibraryOrganizationFOWST ON CacheInvoices.FowstID = MobizLibraryOrganizationFOWST.ID     " & vbCrLf &
                '            "LEFT JOIN MobizLibraryFOWSTRetailStoreType on MobizLibraryFOWSTRetailStoreType.ID = MobizLibraryOrganizationFOWST.SubFOWSTTypeID " & vbCrLf &
                '            "LEFT JOIN MobizLibraryFOWSTStoreLocationType on MobizLibraryFOWSTStoreLocationType.ID = MobizLibraryOrganizationFOWST.StoreLocationTypeID " & vbCrLf
                'End If

                .SqlWhere = " WHERE --TransInvoice.IsCancelled = 0 AND TransInvoice.InvoiceNumber <> 0 AND " & vbCrLf &
                            " convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

                If CStr2(Common.WebRequest.Data("StoreType")) <> "" Then
                    .SqlWhere += " AND MobizLibraryOrganizationFOWST.SubFOWSTTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreType"))) & vbCrLf
                End If

                If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
                    .SqlWhere += " AND MobizLibraryOrganizationFOWST.StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
                End If

                If Not GroupID = "Group" AndAlso Not GroupID = "All" Then
                    .SqlWhere &= "AND CacheInvoices.FranchiseGroupID = " & ReadyString(GroupID) & vbCrLf
                End If

                If Not GroupID = "Group" AndAlso Not GroupID = "All" Then
                    '.SqlGroup = "GROUP BY " & vbCrLf &
                    '            "MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                    '            ", MobizLibraryOrganizationFOWST.Name " & vbCrLf &
                    '            ", MobizLibraryFOWSTRetailStoreType.Name  " & vbCrLf &
                    '            ", MobizLibraryFOWSTRetailStoreType.Name  " & vbCrLf &
                    '            ", MobizLibraryFOWSTStoreLocationType.LocationType " & vbCrLf &
                    '            ",LibraryFranchiseGroup.GroupName"
                    .SqlGroup = "GROUP BY " & vbCrLf &
                                "CacheInvoices.FowstID " & vbCrLf &
                                ", CacheInvoices.FOWSTName " & vbCrLf &
                                ", CacheInvoices.StoreType  " & vbCrLf &
                                ", CacheInvoices.StoreLocation " & vbCrLf &
                                ",CacheInvoices.GroupName"
                ElseIf GroupID = "Group" Then
                    .SqlGroup = "GROUP BY " & vbCrLf &
                                "FranchiseGroupID " & vbCrLf &
                                ",GroupName " & vbCrLf
                End If

            End If

            pDT = .Requery

            If pDT.Columns.Contains("ProductFeature") Then
                For i As Integer = 0 To pDT.Rows.Count - 1
                    If CStr2(pDT.Rows(i).Item("ProductFeature")) = "N/A" Then
                        pDT.Rows.Remove(pDT.Rows(i))
                        'Exit For
                    End If
                Next
            End If

        End With

        Return pDT
    End Function

    Function GetDeviceMonitoringData() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pData As New DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.VarChar)
                    .Expression = "MobizLibraryDeviceFOWSTMonitoring.ID"
                End With
                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.Name"
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "LibraryDevice.PhoneName"
                End With

                With .Add("UUID", SqlDbType.Decimal)
                    .Expression = "MobizLibraryDeviceFOWSTMonitoring.UUID"
                End With

                With .Add("LastDataFlushReceivedInServer", SqlDbType.Decimal)
                    .Expression = "ISNULL(LastDataFlushReceivedInServer,'')"
                End With

                With .Add("NoOfDaysSinceLastDataFlushReceivedInServer", SqlDbType.Decimal)
                    .Expression = "DateDiff(Day, LastDataFlushReceivedInServer,GETDATE())"
                End With

                With .Add("LastDateOfTransactionFlushedFromDevice", SqlDbType.Decimal)
                    .Expression = "ISNULL(LastDateOfTransactionFlushedFromDevice,'')"
                End With

                With .Add("NoOfDaysSinceLastDateOfTransactionFlushedFromDevice", SqlDbType.Decimal)
                    .Expression = "DateDiff(Day, LastDateOfTransactionFlushedFromDevice,GETDATE())"
                End With

                With .Add("LastDataSync", SqlDbType.Decimal)
                    .Expression = "ISNULL(LastDataSync,'')"
                End With

                With .Add("NoOfDaysSinceLastLastDataSync", SqlDbType.Decimal)
                    .Expression = "DateDiff(Day, LastDataSync,GETDATE())"
                End With

                With .Add("InstalledDatabaseCreationDate", SqlDbType.Decimal)
                    .Expression = "ISNULL(InstalledDatabaseCreationDate,'')"
                End With

                With .Add("APKVersion", SqlDbType.Decimal)
                    .Expression = "APKVersion"
                End With

                With .Add("APKBuildDate", SqlDbType.Decimal)
                    .Expression = "APKBuildDate"
                End With

                With .Add("GroupName", SqlDbType.Decimal)
                    .Expression = "LibraryFranchiseGroup.GroupName"
                End With

                With .Add("GroupHead", SqlDbType.Decimal)
                    .Expression = "MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName"
                End With

            End With

            .SqlFrom = "FROM MobizLibraryDeviceFOWSTMonitoring " & vbCrLf &
                "inner join MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = MobizLibraryDeviceFOWSTMonitoring.FOWSTID " & vbCrLf &
                "inner join LibraryDevice on LibraryDevice.ID = MobizLibraryDeviceFOWSTMonitoring.DeviceID " & vbCrLf &
                "INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                "INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf &
                "LEFT JOIN MobizLibraryPeople on MobizLibraryPeople.ID = LibraryFranchiseGroup.FranchisorGroupHead"

            .SqlWhere = "WHERE LibraryDevice.IsActive=1 AND MobizLibraryDeviceFOWSTMonitoring.OrgID =  " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                        " AND MobizLibraryOrganizationFranchisee.FranchiseGroupID IN(" & GroupID & ") " & vbCrLf &
                        " AND MobizLibraryDeviceFOWSTMonitoring.IsActive=1 "

            .SqlOrder = "Order By  LastDataFlushReceivedInServer, LastDateOfTransactionFlushedFromDevice, APKVersion "

            pDT = .Requery

        End With
        'End If

        Return pDT
    End Function

    Function GetDeviceAvailability() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pData As New DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                With .Add("ID", SqlDbType.VarChar)
                    .Expression = "TransDeviceAvailability.ID"
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "ISNULL(DeviceName,'Franchisee Device')"
                End With
                With .Add("IsAvailable", SqlDbType.VarChar)
                    .Expression = "ISNULL(IsAvailable,0)"
                End With
                With .Add("FromFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(FromFranchisee,0)"
                End With
                With .Add("Reason", SqlDbType.Decimal)
                    .Expression = "ISNULL(ReasonForInability,'')"
                End With
                'With .Add("TransDeviceAvailability. DeviceID", SqlDbType.VarChar)
                '    .Expression = "DeviceID"
                'End With
            End With

            .SqlFrom = "FROM TransDeviceAvailability " & vbCrLf &
                        "INNER JOIN LibraryDeviceFowst on LibraryDeviceFowst.DeviceID = TransDeviceAvailability.DeviceID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = LibraryDeviceFowst.FowstID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                        "INNER JOIN LibraryDevice on LibraryDevice.ID = TransDeviceAvailability.DeviceID " & vbCrLf
            '"INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID"

            .SqlWhere = "WHERE LibraryDeviceFowst.IsActive = 1 AND CONVERT(date,TransDeviceAvailability.TransactionDate) BETWEEN " & ReadyString(CStr2(Common.WebRequest.Data("DateFrom"))) & " AND " & ReadyString(CStr2(Common.WebRequest.Data("DateTo"))) & vbCrLf

            If Not CStr2(Common.WebRequest.Data("GroupFOWSTID")) = vbNullString Then
                .SqlWhere &= " AND MobizLibraryOrganizationFranchisee.FranchiseGroupID IN(" & CStr2(Common.WebRequest.Data("GroupFOWSTID")) & ") " & vbCrLf
            End If

            .SqlOrder = "ORDER BY IsAvailable"

            pDT = .Requery


            dtDeviceAvailabilitySummary = New DataTable
            dtDeviceAvailabilitySummary.Columns.Add("TotalRecordsCO")
            dtDeviceAvailabilitySummary.Columns.Add("TotalAvailableCO")
            dtDeviceAvailabilitySummary.Columns.Add("TotalUnAvailable")
            dtDeviceAvailabilitySummary.Columns.Add("TotalUnAvailableWithReason")
            dtDeviceAvailabilitySummary.Columns.Add("TotalNoOfClosedStores")
            dtDeviceAvailabilitySummary.Columns.Add("TotalRecordsFO")
            dtDeviceAvailabilitySummary.Columns.Add("TotalAvailableFO")

            Dim dr As DataRow
            dr = dtDeviceAvailabilitySummary.NewRow

            pDT.DefaultView.RowFilter = "FromFranchisee = 0"
            dr(0) = pDT.DefaultView.ToTable.Rows.Count

            pDT.DefaultView.RowFilter = "IsAvailable = 1 AND FromFranchisee = 0"
            dr(1) = pDT.DefaultView.ToTable.Rows.Count

            pDT.DefaultView.RowFilter = "IsAvailable = 0 AND Reason = ''"
            dr(2) = pDT.DefaultView.ToTable.Rows.Count

            pDT.DefaultView.RowFilter = "Reason <> ''"
            dr(3) = pDT.DefaultView.ToTable.Rows.Count

            dr(4) = GetClosedStores().Rows.Count

            pDT.DefaultView.RowFilter = "FromFranchisee = 1"
            dr(5) = pDT.DefaultView.ToTable.Rows.Count

            pDT.DefaultView.RowFilter = "IsAvailable = 1 AND FromFranchisee = 1"
            dr(6) = pDT.DefaultView.ToTable.Rows.Count

            dtDeviceAvailabilitySummary.Rows.Add(dr)

        End With
        'End If

        Return pDT
    End Function

    Function GetDeviceAvailabilitySummary() As DataTable
        Return dtDeviceAvailabilitySummary
    End Function

    Function GetDeviceStatus()
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pData As New DataTable
        Dim filter As String = Common.WebRequest.Data("Filter")

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                With .Add("ID", SqlDbType.VarChar)
                    .Expression = "TransDeviceAvailability.ID"
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "ISNULL(DeviceName,'Franchisee Device')"
                End With
                With .Add("IsAvailable", SqlDbType.VarChar)
                    .Expression = "ISNULL(IsAvailable,0)"
                End With

                With .Add("Reason", SqlDbType.Decimal)
                    .Expression = "ISNULL(ReasonForInability,'')"
                End With
                With .Add("DeviceID", SqlDbType.VarChar)
                    .Expression = "TransDeviceAvailability.DeviceID"
                End With
            End With

            .SqlFrom = "FROM TransDeviceAvailability " & vbCrLf &
                        "INNER JOIN LibraryDeviceFowst on LibraryDeviceFowst.DeviceID = TransDeviceAvailability.DeviceID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = LibraryDeviceFowst.FowstID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf
            '"INNER JOIN LibraryFranchiseGroup on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID"

            .SqlWhere = "WHERE CONVERT(date,TransDeviceAvailability.TransactionDate) BETWEEN " & ReadyString(CStr2(Common.WebRequest.Data("DateFrom"))) & " AND " & ReadyString(CStr2(Common.WebRequest.Data("DateTo"))) & vbCrLf

            If Not CStr2(Common.WebRequest.Data("GroupFOWSTID")) = vbNullString Then
                .SqlWhere &= " AND MobizLibraryOrganizationFranchisee.FranchiseGroupID IN(" & CStr2(Common.WebRequest.Data("GroupFOWSTID")) & ") " & vbCrLf
            End If

            If filter = "unavailable" Then
                .SqlWhere &= " AND IsAvailable = 0 AND ISNULL(ReasonForInability, '') = '' "
            ElseIf filter = "withreason" Then
                .SqlWhere &= " AND ISNULL(ReasonForInability, '') <> '' "
            Else
                .SqlWhere &= " AND IsAvailable = 1 "
            End If

            .SqlOrder = "ORDER BY IsAvailable"

            pDT = .Requery

        End With
        'End If

        Return pDT
    End Function

    Function GetClosedStores()
        Dim datefrom As String = CStr2(Common.WebRequest.Data("DateFrom"))
        Dim dateto As String = CStr2(Common.WebRequest.Data("DateTo"))
        Dim pSql As String = " SELECT DISTINCT fowst.ID, fowst.Name, " & vbCrLf &
                            " STUFF((SELECT ', ' + CASE WeekDay WHEN 2 THEN 'Monday' " & vbCrLf &
                            " WHEN 3 THEN 'Tuesday' " & vbCrLf &
                            " WHEN 4 THEN 'Wednesday' " & vbCrLf &
                            " WHEN 5 THEN 'Thursday' " & vbCrLf &
                            " WHEN 6 THEN 'Friday' " & vbCrLf &
                            " WHEN 7 THEN 'Saturday' " & vbCrLf &
                            " WHEN 1 THEN 'Sunday' END " & vbCrLf &
                            "       FROM MobizLibraryOrganizationFOWSTOperationHRs " & vbCrLf &
                            "       WHERE FowstID = fowst.ID " & vbCrLf &
                            "       AND IsClosed = 1 " & vbCrLf &
                            "       ORDER BY WeekDay ASC " & vbCrLf &
                            "   FOR XML PATH('') " & vbCrLf &
                            " ), 1, 1,'') as Day, " & vbCrLf &
                            " STUFF((SELECT ', ' + CONVERT(varchar, WeekDay) " & vbCrLf &
                            "       FROM MobizLibraryOrganizationFOWSTOperationHRs " & vbCrLf &
                            "       WHERE FowstID = fowst.ID " & vbCrLf &
                            "       AND IsClosed = 1 " & vbCrLf &
                            "       ORDER BY WeekDay ASC " & vbCrLf &
                            "   FOR XML PATH('') " & vbCrLf &
                            " ), 1, 1,'') as DayNum " & vbCrLf &
                            " FROM MobizLibraryOrganizationFOWSTOperationHRs as op " & vbCrLf &
                            " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = op.FOWSTID " & vbCrLf &
                            "INNER JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = fowst.ID " & vbCrLf &
                            " WHERE ISNULL(IsClosed, 0) = 1 " & vbCrLf &
                            " AND fowst.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf

        If Not CStr2(Common.WebRequest.Data("GroupFOWSTID")) = vbNullString Then
            pSql &= " AND MobizLibraryOrganizationFranchisee.FranchiseGroupID IN(" & CStr2(Common.WebRequest.Data("GroupFOWSTID")) & ") " & vbCrLf
        End If

        If CDate(datefrom) = CDate(dateto) Then
            pSql &= " AND WeekDay = DATEPART(WEEKDAY, " & ReadyString(CStr2(datefrom)) & ")"
        ElseIf CInt(DateTime.Parse(datefrom).DayOfWeek) >= CInt(DateTime.Parse(dateto).DayOfWeek) Then
            pSql &= " AND WeekDay <= DATEPART(WEEKDAY, " & ReadyString(datefrom) & ") " & vbCrLf
        Else
            pSql &= " AND ( WeekDay <= DATEPART(WEEKDAY, " & ReadyString(dateto) & ") " & vbCrLf &
                    " AND WeekDay >= DATEPART(WEEKDAY, " & ReadyString(datefrom) & ")) " & vbCrLf
        End If

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function SaveFowstOpHrs()
        Dim closed As Object = Common.WebRequest.AllData("Closed")
        Dim fowstid As String = Common.WebRequest.Data("FowstID")

        Try
            For Each newclosed As Object In closed
                Dim pIU = New InsertUpdate("MobizLibraryOrganizationFOWSTOperationHRs")

                pIU("IsClosed") = CInt2(newclosed("IsClosed"))
                pIU("WeekDay") = ReadyString(newclosed("WeekDay"))
                pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                pIU("LastUpdated") = "getdate()"

                If Common.WebRequest.ProData.ExecuteSQL(pIU.GetUpdate(" FOWSTID = " & ReadyString(fowstid) & " AND WeekDay = " & CInt2(newclosed("WeekDay")))) = 0 Then
                    pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                    pIU("FOWSTID") = ReadyString(fowstid)
                    Common.WebRequest.ProData.Execute(pIU.GetInsert())
                End If
            Next

            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function TagAsDummyDevice() As Boolean
        Dim ID As String
        Dim pSql As String

        ID = CStr2(Common.WebRequest.Data("DeviceID"))

        pSql = "Update LibraryDevice set IsDummyDevice = 1 WHERE ID = " & ReadyString(ID)

        Common.WebRequest.ProData.Execute(pSql)

        pSql = "Delete from TransDeviceAvailability WHERE DeviceID = " & ReadyString(ID)

        Common.WebRequest.ProData.Execute(pSql)


        Return True
    End Function

    Function SetReasonforUnavailability() As Boolean
        Dim ID As String
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim DeviceID As String
        Dim DateFrom As String
        Dim DateTo As String
        Dim ReasonForInability As String
        Dim fowstOperationgHrs As Object = Common.WebRequest.AllData("FowstOP")
        ID = CStr2(Common.WebRequest.Data("ID"))
        DeviceID = CStr2(Common.WebRequest.Data("DeviceID"))
        DateFrom = CStr2(Common.WebRequest.Data("DateFrom"))
        DateTo = CStr2(Common.WebRequest.Data("DateTo"))
        ReasonForInability = CStr2(Common.WebRequest.Data("ReasonforUnavailability"))

        pIU = New InsertUpdate("TransDeviceAvailability")

        pIU("TransactionDate") = ReadyString(DateFrom)
        pIU("IsAvailable") = 0
        pIU("ReasonForInability") = ReadyString(ReasonForInability)

        If ID.Length > 0 Then
            pSql = pIU.GetUpdate("ID = " & ReadyString(ID))
        Else
            pIU("ID") = ReadyString(ID)
            pIU("DeviceID") = ReadyString(DeviceID)
            pSql = pIU.GetInsert()
        End If

        Common.WebRequest.ProData.BeginTransaction()
        Common.WebRequest.ProData.Execute(pSql)

        For Each fowst As Object In fowstOperationgHrs
            For Each newclosed As Object In fowst("NewClosed")
                pIU = New InsertUpdate("MobizLibraryOrganizationFOWSTOperationHRs")

                pIU("IsClosed") = CInt2(newclosed("IsClosed"))
                pIU("WeekDay") = ReadyString(newclosed("WeekDay"))
                pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                pIU("LastUpdated") = "getdate()"

                If Common.WebRequest.ProData.ExecuteSQL(pIU.GetUpdate(" FOWSTID = " & ReadyString(fowst("ID")) & " AND WeekDay = " & CInt2(newclosed("WeekDay")))) = 0 Then
                    pIU("ID") = ReadyString(Guid.NewGuid.ToString())
                    pIU("FOWSTID") = ReadyString(fowst("ID"))
                    Common.WebRequest.ProData.Execute(pIU.GetInsert())
                End If
            Next
        Next

        Common.WebRequest.ProData.CommitTransaction()

        Return True
    End Function

    Function DeactivateDeviceMonitoringData() As Boolean
        Dim ID As String
        Dim pSql As String

        ID = Common.WebRequest.Data("ID")

        pSql = "Update MobizLibraryDeviceFOWSTMonitoring set IsActive = 0 WHERE ID = " & ReadyString(ID)

        Common.WebRequest.ProData.Execute(pSql)

        Return True
    End Function

    Function GetDeviceFlushingQueueMonitoring() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pData As New DataTable
        Dim pSql As String

        pSql = "Select top 50 * from TransUnflushedDataCount order by DateTime desc "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        pDT.DefaultView.Sort = "DateTime ASC"

        Return pDT
    End Function

    Function GetDeviceFlushingQueue() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pData As New DataTable
        Dim pSql As String

        pSql = "Select LibraryDevice.ID as DeviceID, FileSize, LibraryDevice.PhoneName DeviceName, DateReceived, ISNULL(ProcessStart,'') ProcessStart, ISNULL(ProcessEnd,'') ProcessEnd, " & vbCrLf &
                "ISNULL(DATEDIFF(SECOND,ProcessStart,ProcessEnd),'') ProcessTimeInSeconds  " & vbCrLf &
                "from TransImports " & vbCrLf &
                "inner join LibraryDevice on LibraryDevice.ID = TransImports.DeviceID " & vbCrLf

        If Not CStr2(FOWSTID) = vbNullString Then
            pSql &= "WHERE FOWSTID = " & ReadyString(FOWSTID) & vbCrLf
        Else
            pSql &= "WHERE result Is null "
        End If

        pSql &= " order by ProcessStart desc, FileSize " & vbCrLf

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetStoreUnflushedCount() As DataTable
        Dim pDT As DataTable
        Dim pData As New DataTable
        Dim pSql As String

        pSql = " SELECT FowstID, fowst.Code, fowst.Name, CONVERT(date, FlushingDate) as FlushingDate, SUM(UnflushedCount) as UnflushedCount " & vbCrLf &
                " FROM TransUnflushedCounts " & vbCrLf &
                " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = FowstID " & vbCrLf &
                " --LEFT JOIN MobizLibraryOrganizationFranchisee as fr ON fr.FranchiseeOrgFowstID = FowstID " & vbCrLf &
                " --LEFT JOIN LibraryFranchiseGroup as grp ON grp.ID = fr.FranchiseGroupID " & vbCrLf &
                " WHERE CONVERT(date, FlushingDate) = " & ReadyString(Common.WebRequest.Data("Date")) & vbCrLf &
                " AND UnflushedCount > 0 " & vbCrLf &
                " GROUP BY FowstID, fowst.Code, fowst.Name, CONVERT(date, FlushingDate)" & vbCrLf &
                " ORDER BY UnflushedCount DESC "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetSystemWideFlushingHistory() As DataTable
        Dim pDT As DataTable
        Dim pData As New DataTable
        Dim pSql As String
        Dim dtFrom As String = Common.WebRequest.Data("DateFrom")
        Dim dtTo As String = Common.WebRequest.Data("DateTo")
        Dim group As String = Common.WebRequest.Data("Group")

        pSql = " SELECT FowstID, fowst.Code, fowst.Name, ISNULL(grp.GroupName, '') as GroupName, SUM(UnflushedCount) as UnflushedCount, 0 as IsFranchise " & vbCrLf &
                " FROM MobizCinco_Corporation.dbo.TransUnflushedCounts " & vbCrLf &
                " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = FowstID " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFranchisee as fr ON fr.FranchiseeOrgFowstID = FowstID " & vbCrLf &
                " LEFT JOIN MobizCinco_Corporation.dbo.LibraryFranchiseGroup as grp ON grp.ID = fr.FranchiseGroupID " & vbCrLf &
                " WHERE CONVERT(date, FlushingDate) BETWEEN " & ReadyString(dtFrom) & " AND " & ReadyString(dtTo) & vbCrLf &
                " AND UnflushedCount > 0 " & vbCrLf

        If group <> "All" Then
            pSql += " AND ISNULL(grp.GroupName, '') = " & ReadyString(group) & vbCrLf
        Else
            pSql += " AND ISNULL(grp.GroupName, '') <> '' " & vbCrLf
        End If

        pSql += " GROUP BY FowstID, fowst.Code, fowst.Name, grp.GroupName" & vbCrLf

        pSql += " UNION ALL " & vbCrLf

        pSql += " SELECT FowstID, fowst.Code, fowst.Name, ISNULL(grp.GroupName, '') as GroupName, SUM(UnflushedCount) as UnflushedCount, 1 as IsFranchise " & vbCrLf &
                " FROM MobizPotato_Corner_Franchise.dbo.TransUnflushedCounts " & vbCrLf &
                " INNER JOIN MobizLibraryOrganizationFOWST as fowst ON fowst.ID = FowstID " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFranchisee as fr ON fr.FranchiseeOrgFowstID = FowstID " & vbCrLf &
                " LEFT JOIN MobizCinco_Corporation.dbo.LibraryFranchiseGroup as grp ON grp.ID = fr.FranchiseGroupID " & vbCrLf &
                " WHERE CONVERT(date, FlushingDate) BETWEEN " & ReadyString(dtFrom) & " AND " & ReadyString(dtTo) & vbCrLf &
                " AND UnflushedCount > 0 " & vbCrLf

        If group <> "All" Then
            pSql += " AND ISNULL(grp.GroupName, '') = " & ReadyString(group) & vbCrLf
        Else
            pSql += " AND ISNULL(grp.GroupName, '') <> '' " & vbCrLf
        End If

        pSql += " GROUP BY FowstID, fowst.Code, fowst.Name, grp.GroupName" & vbCrLf

        pSql = " SELECT * FROM (" & pSql & ") as tbl ORDER BY UnflushedCount DESC "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function StoreUnflushedCountHistory() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pData As New DataTable
        Dim pSql As String
        Dim dtFrom As String = Common.WebRequest.Data("DateFrom")
        Dim dtTo As String = Common.WebRequest.Data("DateTo")

        pSql = " SELECT CONVERT(date, FlushingDate) as FlushingDate, SUM(UnflushedCount) as UnflushedCount " & vbCrLf &
                " FROM TransUnflushedCounts " & vbCrLf &
                " WHERE FowstID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
                " AND CONVERT(date, FlushingDate) BETWEEN " & ReadyString(dtFrom) & " AND " & ReadyString(dtTo) & vbCrLf &
                " GROUP BY CONVERT(date, FlushingDate) " & vbCrLf &
                " ORDER BY CONVERT(date, FlushingDate) DESC "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function FlushingBreakdown() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pData As New DataTable
        Dim pSql As String

        pSql = " SELECT TransactionDate, UnflushedCount " & vbCrLf &
                " FROM TransUnflushedCounts " & vbCrLf &
                " WHERE FowstID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
                " AND UnflushedCount > 0 " & vbCrLf &
                " AND CONVERT(date, FlushingDate) = " & ReadyString(Common.WebRequest.Data("Date")) & vbCrLf &
                " ORDER BY CONVERT(date, TransactionDate) "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetDeviceUtilization() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pData As New DataTable
        Dim pSql As String
        Dim clsSecurityRole As New SecurityRole

        If FOWSTID = "All Stores" Then
            pSql = "Select distinct MobizLibraryOrganizationFOWST.ID as FOWSTID, MobizLibraryOrganizationFOWST.Name FOWSTName " & vbCrLf &
                    ", ISNULL(MobizLibraryOrganizationFOWST.Code, '-') StoreCode " & vbCrLf &
                    ", CASE WHEN OrganizationID = 'd362ffdb-15d1-451f-97dc-c567b94fb410' THEN 'CO' ELSE 'FO' END  [Group] " & vbCrLf &
                    ", ISNULL(LibraryFranchiseGroup.GroupName,'') GroupName " & vbCrLf &
                    ", ISNULL(Status, 'Others') as Status " & vbCrLf &
                    ", ISNULL(count(distinct convert(date,invoicedate)),0) Usage " & vbCrLf &
                    ", LastSalesDataImportedToServer " & vbCrLf &
                    " FROM MobizLibraryOrganizationFOWST " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                    " LEFT JOIN LibraryFranchiseGroup on MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID " & vbCrLf &
                    " LEFT JOIN CacheInvoices on MobizLibraryOrganizationFOWST.ID = CacheInvoices.FowstID and convert(date,InvoiceDate) Between " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                    " where FranchisorOrgID = 'd362ffdb-15d1-451f-97dc-c567b94fb410' and MobizLibraryOrganizationFOWST.IsActive=1 --MobizLibraryOrganizationFOWST.OrganizationID = 'd362ffdb-15d1-451f-97dc-c567b94fb410' or  MobizLibraryOrganizationFOWST.OrganizationID = '892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba' " & vbCrLf

            If Not String.IsNullOrWhiteSpace(StoreStatus) Then
                pSql &= " AND (MobizLibraryOrganizationFOWST.Status = " & ReadyString(StoreStatus) & vbCrLf

                If StoreStatus = "Store and device are operational." Then
                    'Available
                    pSql &= " OR ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Available'" & vbCrLf
                ElseIf StoreStatus = "Store is permanently closed." Then
                    'Closed
                    pSql &= " OR ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Closed'" & vbCrLf
                ElseIf StoreStatus = "Store is temporarily closed for holiday." Then
                    'Termporary Closed
                    pSql &= " OR ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Temporary Closed'" & vbCrLf
                End If

                pSql &= ") " & vbCrLf
            End If

            pSql &= " group by MobizLibraryOrganizationFOWST.ID, MobizLibraryOrganizationFOWST.Name, LibraryFranchiseGroup.GroupName, OrganizationID, Status, Code, LastSalesDataImportedToServer " & vbCrLf &
                    " order by Usage, [Group]"
        Else

            pSql = "Select MobizLibraryOrganizationFOWST.ID as FOWSTID, MobizLibraryOrganizationFOWST.Name FOWSTName, ISNULL(MobizLibraryOrganizationFOWST.Code, '-') StoreCode , ISNULL(LibraryFranchiseGroup.GroupName,'') GroupName " & vbCrLf &
                    ", CASE WHEN ISNULL(FromFranchisee,0) = 0 THEN 'CO' ELSE 'FO' END  [Group] " & vbCrLf &
                    ", ISNULL(Status, 'Others') as Status " & vbCrLf &
                    ", ISNULL(count(distinct convert(date,invoicedate)),0) Usage " & vbCrLf &
                    ", LastSalesDataImportedToServer " & vbCrLf &
                    " FROM MobizLibraryOrganizationFOWST " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                    " LEFT JOIN LibraryFranchiseGroup on MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID " & vbCrLf &
                    " LEFT JOIN CacheInvoices on MobizLibraryOrganizationFOWST.ID = CacheInvoices.FowstID and convert(date,InvoiceDate) Between " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf
            '" WHERE convert(date,InvoiceDate) Between " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            If Not CStr2(FOWSTID) = vbNullString Then
                pSql &= " WHERE FOWSTID = " & ReadyString(FOWSTID) & vbCrLf
            End If

            If Not CStr2(GroupID) = vbNullString Then
                pSql &= " WHERE MobizLibraryOrganizationFranchisee.FranchiseGroupID IN (" & GroupID & ") " & vbCrLf

                If clsSecurityRole.CheckSecurityCurrentUserAcccess("Show only the assigned FOWST/Group to the user", WebRequest.Instance.OrgID, WebSession.Instance.PeopleUserID).Rows.Count > 0 Then
                    pSql &= " AND MobizLibraryOrganizationFOWST.ID IN (" & vbCrLf &
                        " SELECT " & vbCrLf &
                        "   FranchiseeOrgFowstID " & vbCrLf &
                        "   FROM MobizLibraryOrganizationFranchisee " & vbCrLf &
                        "   WHERE FranchisorOrgID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                        "   AND FranchiseGroupID IN (" & GroupID & ") " & vbCrLf &
                        "   AND FranchiseeOrgFowstID IN (SELECT " & vbCrLf &
                        "   CONVERT(varchar(max), FowstID) as FowstID " & vbCrLf &
                        "   FROM LibraryOrganizationEmployeeFOWST " & vbCrLf &
                        "   WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                        " ) GROUP BY FranchiseeOrgFowstID) " & vbCrLf
                End If
            End If

            If Not String.IsNullOrWhiteSpace(StoreStatus) Then
                pSql &= " AND (ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = " & ReadyString(StoreStatus) & vbCrLf

                If StoreStatus = "Store and device are operational." Then
                    'Available
                    pSql &= " OR ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Available'" & vbCrLf
                ElseIf StoreStatus = "Store is permanently closed." Then
                    'Closed
                    pSql &= " OR ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Closed'" & vbCrLf
                ElseIf StoreStatus = "Store is temporarily closed for holiday." Then
                    'Termporary Closed
                    pSql &= " OR ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Temporary Closed'" & vbCrLf
                End If
                pSql &= ")" & vbCrLf
            End If

            pSql &= " GROUP BY MobizLibraryOrganizationFOWST.ID, MobizLibraryOrganizationFOWST.Name, LibraryFranchiseGroup.GroupName, FromFranchisee, Code, Status, LastSalesDataImportedToServer " & vbCrLf

            pSql &= " order by Usage " & vbCrLf

        End If

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT
    End Function

    Function GetDeviceUtilizationByHour() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pData As New DataTable
        Dim pSql As String

        pSql = "Select FOWSTName StoreName, ISNULL(GroupName,'') GroupName " & vbCrLf &
                ", CONVERT(date,InvoiceDate) TransactionDate " & vbCrLf &
                ", CONVERT(varchar, DATEPART(hh, InvoiceDate)) + ':00' Hour " & vbCrLf &
                ", COUNT(ID) TransactionCount " & vbCrLf &
                " FROM CacheInvoices " & vbCrLf &
                " WHERE convert(date,InvoiceDate) Between " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

        If Not CStr2(FOWSTID) = vbNullString Then
            pSql &= " AND FOWSTID = " & ReadyString(FOWSTID) & vbCrLf
        End If

        If Not CStr2(GroupID) = vbNullString Then
            pSql &= " AND FranchiseGroupID IN (" & GroupID & ") " & vbCrLf
        End If

        pSql &= " GROUP BY FOWSTName, GroupName, CONVERT(date,InvoiceDate), CONVERT(varchar, DATEPART(hh, InvoiceDate)) + ':00' " & vbCrLf

        pSql &= " order by StoreName " & vbCrLf

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Dim pFilter1 As DataTable

        pFilter1 = CreateCrossTab(pDT, {"StoreName", "GroupName", "TransactionDate"}, {New CrossColumn("{Hour}", "TransactionCount", "TransactionCount")})

        pFilter1 = pFilter1.DefaultView.ToTable

        'pFilter1.Columns.Remove("TransactionCount")


        'Sort Columns

        Dim tempDatatable As New DataTable
        Dim mHeaders As New List(Of Integer)


        For i As Integer = 3 To pFilter1.Columns.Count - 2 'loop columns
            If IsNumeric(Left(pFilter1.Columns(i).ColumnName, 2)) Then
                mHeaders.Add(CInt2(Left(pFilter1.Columns(i).ColumnName, 2)))
            Else
                mHeaders.Add(CInt2(Left(pFilter1.Columns(i).ColumnName, 1)))
            End If

        Next

        mHeaders.Sort()

        tempDatatable.Columns.Add("StoreName")
        tempDatatable.Columns.Add("GroupName")
        tempDatatable.Columns.Add("TransactionDate")
        tempDatatable.Columns.Add("TotalTransactionCount")

        'add sorted column headers
        For i As Integer = 0 To mHeaders.Count - 1
            tempDatatable.Columns.Add(mHeaders(i) & ":00")
        Next

        tempDatatable.Columns.Add("NoOfHours")

        For i As Integer = 0 To pFilter1.Rows.Count - 1
            Dim dr As DataRow
            Dim hourCount As Integer = 0

            dr = tempDatatable.NewRow

            dr(0) = pFilter1.Rows(i).Item("StoreName")
            dr(1) = pFilter1.Rows(i).Item("GroupName")
            dr(2) = pFilter1.Rows(i).Item("TransactionDate")
            dr(3) = pFilter1.Rows(i).Item("TransactionCount")

            For j As Integer = 4 To tempDatatable.Columns.Count - 2
                dr(j) = pFilter1.Rows(i).Item(tempDatatable.Columns(j).ColumnName) 'add data

                If CInt2(pFilter1.Rows(i).Item(tempDatatable.Columns(j).ColumnName)) > 0 Then
                    hourCount += 1
                End If
            Next

            dr(tempDatatable.Columns.Count - 1) = hourCount

            tempDatatable.Rows.Add(dr)
        Next

        tempDatatable.DefaultView.Sort = "NoOfHours"

        Return tempDatatable.DefaultView.ToTable

    End Function

    Function GetConsolidatedPromoReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim strWhereFowst As String
        Dim SqlGroupBy As String
        Dim SqlOrderBy As String
        Dim SqlFrom As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1
        Dim pSql As String

        pFinder = New Finder

        'With pFinder

        '    .Distinct = True
        '    .ProData = Common.WebRequest.ProData

        '    With .Fields
        '        With .Add("Invoicedate", SqlDbType.UniqueIdentifier)
        '            .Expression = "Invoicedate"
        '        End With

        '        With .Add("StoreName", SqlDbType.VarChar)
        '            .Expression = "FowstName"
        '        End With

        '        With .Add("GroupName", SqlDbType.VarChar)
        '            .Expression = "ISNULL(GroupName,'')"
        '        End With
        '        With .Add("Promo", SqlDbType.VarChar)
        '            .Expression = "ItemModifierName"
        '        End With
        '        'With .Add("ProductDescription", SqlDbType.VarChar)
        '        '    .Expression = "ModifierProductName"
        '        'End With
        '        'With .Add("UnitMeasure", SqlDbType.VarChar)
        '        '    .Expression = "ModifierUnitMeasure"
        '        'End With
        '        With .Add("Quantity", SqlDbType.VarChar)
        '            .Expression = "SUM(ProductQuantity)"
        '        End With
        '        With .Add("ItemModifierID", SqlDbType.VarChar)
        '            .Expression = "ItemModifierID"
        '        End With
        '    End With


        '    .SqlFrom = "FROM CacheInvoiceModifiers " & vbCrLf

        '    .SqlWhere = " WHERE IsPromo = 1 " & vbCrLf &
        '                " AND convert(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & " " & vbCrLf

        '    .SqlGroup = " GROUP by Invoicedate, " & vbCrLf &
        '                "FowstName, " & vbCrLf &
        '                "GroupName, " & vbCrLf &
        '                "ItemModifierName, " & vbCrLf &
        '                "ItemModifierID " & vbCrLf

        '    '"ModifierProductName, " & vbCrLf &
        '    '"ModifierUnitMeasure, " & vbCrLf &

        '    If Not FOWSTID = vbNullString Then
        '        .SqlWhere &= " AND CacheInvoiceModifiers.FOWSTID = " & ReadyString(FOWSTID) & vbCrLf
        '    End If

        '    If Not GroupID = vbNullString Then
        '        .SqlWhere &= " AND CacheInvoiceModifiers.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
        '    End If

        '    .SqlOrder = " ORDER BY InvoiceDate"

        '    pDT = .Requery

        '    If Common.WebRequest.Data("PromoType") <> "" Then
        '        'pSql = "SELECT * FROM (" & pSql & ") as T WHERE ItemModifierID = " & ReadyString(Common.WebRequest.Data("PromoType").ToString())
        '        pDT.DefaultView.RowFilter = "ItemModifierID = " & ReadyString(Common.WebRequest.Data("PromoType").ToString())

        '        pDT = pDT.DefaultView.ToTable
        '    End If

        'End With

        With pFinder

            .Distinct = True
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("Invoicedate", SqlDbType.UniqueIdentifier)
                    .Expression = "Invoicedate"
                End With

                With .Add("StoreName", SqlDbType.VarChar)
                    .Expression = "FowstName"
                End With

                With .Add("GroupName", SqlDbType.VarChar)
                    .Expression = "ISNULL(GroupName,'')"
                End With
                With .Add("Promo", SqlDbType.VarChar)
                    .Expression = "ProductName"
                End With
                With .Add("Quantity", SqlDbType.VarChar)
                    .Expression = "SUM(ProductQuantity)"
                End With

            End With

            .SqlFrom = "FROM CacheInvoiceFinishedProducts " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductVariableFeatures001 on MobizLibraryPIMProductVariableFeatures001.ProductID = CacheInvoiceFinishedProducts.ProductID " & vbCrLf

            .SqlWhere = " WHERE MobizLibraryPIMProductVariableFeatures001.C0020 like '%Promo%' " & vbCrLf &
                        " AND convert(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & " " & vbCrLf

            .SqlGroup = " GROUP by Invoicedate, " & vbCrLf &
                        "FowstName, " & vbCrLf &
                        "GroupName, " & vbCrLf &
                        "ProductName " & vbCrLf


            If Not FOWSTID = vbNullString Then
                .SqlWhere &= " AND FOWSTID = " & ReadyString(FOWSTID) & vbCrLf
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= " AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            .SqlOrder = " ORDER BY InvoiceDate"

            pDT = .Requery

        End With

        Dim pFilter1 As DataTable

        pFilter1 = CreateCrossTab(pDT, {"StoreName", "GroupName"}, {New CrossColumn("{Promo} {Invoicedate}", "Quantity", "Quantity")})

        pFilter1.Columns.Remove("Quantity")

        pFilter1 = pFilter1.DefaultView.ToTable

        Return pFilter1
    End Function

    Function GetSalesTransactionTypeAndTransactionCount() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransactionDate", SqlDbType.UniqueIdentifier)
                    .Expression = "CONVERT(date,InvoiceDate)"
                End With
                With .Add("FOWSTCode", SqlDbType.UniqueIdentifier)
                    .Expression = "FOWSTCode"
                End With
                With .Add("FOWSTName", SqlDbType.UniqueIdentifier)
                    .Expression = "FOWSTName"
                End With
                With .Add("StoreType", SqlDbType.VarChar)
                    .Expression = "ISNULL(StoreType,'')"
                End With
                With .Add("StoreLocation", SqlDbType.VarChar)
                    .Expression = "ISNULL(StoreLocation,'')"
                End With
                With .Add("GroupName", SqlDbType.Int)
                    .Expression = "ISNULL(GroupName,'')"
                End With
                With .Add("GrossInvoiceAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("SalesReturnAndRefund", SqlDbType.Bit)
                    .Expression = "SUM(SRVAmount)"
                End With
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "TransactionCount"
                End With
                With .Add("DiscountAmount", SqlDbType.Bit)
                    '.Expression = "ISNULL(SCDiscount,0) + ISNULL(ForegoneVAT,0)"
                    .Expression = "ISNULL(SCDiscount,0) + ISNULL(ForegoneVAT,0)"
                End With
                With .Add("CashAmount", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CashAmount),0)"
                End With
                'With .Add("CreditCardAmount", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM(CreditCardAmount),0)"
                'End With
                'With .Add("DebitCardAmount", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM(DebitCardAmount),0)"
                'End With
                'With .Add("CheckPaymentAmount", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM(PotatoCornerGC),0)"
                'End With
                'With .Add("OtherPayments", SqlDbType.Bit)
                '    .Expression = "SUM(TransCustomerPayment.CheckPaymentAmount)"
                'End With
                'With .Add("DiscountVoucher", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM([Discount Voucher / On Line Deals]),0)"
                'End With
                'With .Add("E-Plus", SqlDbType.Bit)
                '    .Expression = "ISNULL(SUM([E-Plus]),0)"
                'End With
                With .Add("FoodPanda", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(FoodPanda),0)"
                End With
                With .Add("GCash", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([GCash]),0)"
                End With
                With .Add("PayMaya", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([PayMaya]),0)"
                End With
                With .Add("GrabFood", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([GrabFood]),0)"
                End With
                With .Add("SMAC", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([SMAC]),0)"
                End With
                With .Add("V-Plus", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([V-Plus]),0)"
                End With

                With .Add("GoldCard", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([GoldCard]),0)"
                End With

                With .Add("ShopeeEVoucher", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([ShopeeEVoucher]),0)"
                End With

                With .Add("LocalDelivery", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([LocalDelivery]),0)"
                End With

                With .Add("ShopeePay", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([ShopeePay]),0)"
                End With

                With .Add("Eatigo", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([Eatigo]),0)"
                End With

                With .Add("DeliRush", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([DeliRush]),0)"
                End With

                With .Add("PoodTrip", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([PoodTrip]),0)"
                End With

                With .Add("ShareTreats", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([Share Treats]),0)"
                End With

                With .Add("Booky", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([Booky]),0)"
                End With

                With .Add("Lazada", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([Lazada]),0)"
                End With

                With .Add("Pickaroo", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM([Pickaroo]),0)"
                End With

                With .Add("DeliveryCount", SqlDbType.Bit)
                    .Expression = "DeliveryTransactionCount"
                End With

                With .Add("OperatingDaysCount", SqlDbType.Bit)
                    .Expression = "COUNT(DISTINCT INVOICEDATE)"
                End With

                With .Add("LatestTransactionDate", SqlDbType.Bit)
                    .Expression = "MAX(InvoiceDate)"
                End With

            End With

            .SqlFrom = "FROM CacheInvoicesDaily "
            '.SqlFrom = "FROM CacheInvoices " & vbCrLf &
            '            "OUTER APPLY( " & vbCrLf &
            '            "SELECT COUNT(DISTINCT ID) mCount from CacheInvoices ci " & vbCrLf &
            '            "WHERE CashNetOfChange = 0  " & vbCrLf &
            '            "AND convert(date, ci.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
            '            " AND ci.FOWSTID = CacheInvoices .FOWSTID " & vbCrLf &
            '            ") DelCount " & vbCrLf &
            '            "OUTER APPLY( " & vbCrLf &
            '            "SELECT COUNT(DISTINCT convert(date, ci2.InvoiceDate)) mDaysCount from CacheInvoices ci2 " & vbCrLf &
            '            "WHERE convert(date, ci2.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
            '            " AND ci2.FOWSTID = CacheInvoices .FOWSTID " & vbCrLf &
            '            ") TransactionCount " & vbCrLf

            .SqlWhere = " WHERE convert(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf


            If CStr2(Common.WebRequest.Data("StoreType")) <> "" Then
                .SqlWhere += " AND SubFOWSTTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreType"))) & vbCrLf
            End If

            If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
                .SqlWhere += " AND StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
            End If

            If Not FOWSTID = vbNullString Then
                .SqlWhere &= " AND FOWSTID IN (" & FOWSTID & ")" & vbCrLf
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= "AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "FOWSTName " & vbCrLf &
                        ", StoreType  " & vbCrLf &
                        ", StoreLocation " & vbCrLf &
                        ",GroupName " & vbCrLf &
                        ",FowstID " & vbCrLf &
                        ",FOWSTCode " & vbCrLf &
                        ", convert(date,InvoiceDate)" & vbCrLf &
                        ", TransactionCount " & vbCrLf &
                        ", DeliveryTransactionCount " & vbCrLf &
                        ", SCDiscount " & vbCrLf &
                        ", ForegoneVAT " & vbCrLf

            '", DeliveryCount " & vbCrLf &
            '            ", OperatingDaysCount" & vbCrLf &

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetSalesSummaryReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransactionDate", SqlDbType.UniqueIdentifier)
                    .Expression = "CONVERT(date,InvoiceDate)"
                End With
                With .Add("FOWSTCode", SqlDbType.UniqueIdentifier)
                    .Expression = "FOWSTCode"
                End With
                With .Add("FOWSTName", SqlDbType.UniqueIdentifier)
                    .Expression = "FOWSTName"
                End With
                With .Add("StoreType", SqlDbType.VarChar)
                    .Expression = "ISNULL(StoreType,'')"
                End With
                With .Add("StoreLocation", SqlDbType.VarChar)
                    .Expression = "ISNULL(StoreLocation,'')"
                End With
                With .Add("GroupName", SqlDbType.Int)
                    .Expression = "ISNULL(GroupName,'')"
                End With
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(GrossInvoiceAmount,0)"
                End With
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "TransactionCount"
                End With
                With .Add("DeliverySales", SqlDbType.Bit)
                    .Expression = "DeliverySales"
                End With
                With .Add("DeliveryCount", SqlDbType.Bit)
                    .Expression = "DeliveryTransactionCount"
                End With
                With .Add("WalkInSales", SqlDbType.Bit)
                    .Expression = "WalkInSales"
                End With
                With .Add("WalkInCount", SqlDbType.Bit)
                    .Expression = "WalkInTransactionCount"
                End With

                With .Add("OperatingDaysCount", SqlDbType.Bit)
                    .Expression = "COUNT(DISTINCT INVOICEDATE)"
                End With

                With .Add("LatestTransactionDate", SqlDbType.Bit)
                    .Expression = "MAX(InvoiceDate)"
                End With

            End With

            .SqlFrom = "FROM CacheInvoicesDaily "

            .SqlWhere = " WHERE convert(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf


            If CStr2(Common.WebRequest.Data("StoreType")) <> "" Then
                .SqlWhere += " AND SubFOWSTTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreType"))) & vbCrLf
            End If

            If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
                .SqlWhere += " AND StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
            End If

            If Not FOWSTID = vbNullString Then
                If FOWSTID = "All Stores" Then
                    .SqlWhere &= " AND FOWSTID IS NOT NULL " & vbCrLf
                Else
                    .SqlWhere &= " AND FOWSTID IN (" & FOWSTID & ")" & vbCrLf
                End If
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= "AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "CONVERT(date,InvoiceDate) " & vbCrLf &
                        ", FOWSTCode " & vbCrLf &
                        ", FOWSTName " & vbCrLf &
                        ", ISNULL(StoreType,'') " & vbCrLf &
                        ", ISNULL(StoreLocation,'') " & vbCrLf &
                        ", ISNULL(GroupName,'') " & vbCrLf &
                        ", ISNULL(GrossInvoiceAmount,0) " & vbCrLf &
                        ", TransactionCount " & vbCrLf &
                        ", DeliverySales " & vbCrLf &
                        ", DeliveryTransactionCount " & vbCrLf &
                        ", WalkInSales " & vbCrLf &
                        ", WalkInTransactionCount " & vbCrLf

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetSalesAndTransactionCountMixReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransactionDate", SqlDbType.UniqueIdentifier)
                    .Expression = "CONVERT(date,InvoiceDate)"
                End With
                With .Add("FOWSTCode", SqlDbType.UniqueIdentifier)
                    .Expression = "FOWSTCode"
                End With
                With .Add("FOWSTName", SqlDbType.UniqueIdentifier)
                    .Expression = "FOWSTName"
                End With
                With .Add("StoreType", SqlDbType.VarChar)
                    .Expression = "ISNULL(StoreType,'')"
                End With
                With .Add("StoreLocation", SqlDbType.VarChar)
                    .Expression = "ISNULL(StoreLocation,'')"
                End With
                With .Add("GroupName", SqlDbType.Int)
                    .Expression = "ISNULL(GroupName,'')"
                End With
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(GrossSalesNetOfVATExemptions,0)"
                End With
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "TransactionCount"
                End With
                With .Add("DeliverySales", SqlDbType.Bit)
                    .Expression = "DeliverySales"
                End With
                With .Add("DeliveryCount", SqlDbType.Bit)
                    .Expression = "DeliveryTransactionCount"
                End With
                With .Add("WalkInSales", SqlDbType.Bit)
                    .Expression = "WalkInSales"
                End With
                With .Add("WalkInCount", SqlDbType.Bit)
                    .Expression = "WalkInTransactionCount"
                End With

                'With .Add("OperatingDaysCount", SqlDbType.Bit)
                '    .Expression = "COUNT(DISTINCT INVOICEDATE)"
                'End With

                'With .Add("LatestTransactionDate", SqlDbType.Bit)
                '    .Expression = "MAX(InvoiceDate)"
                'End With

            End With

            .SqlFrom = "FROM CacheInvoicesDaily "

            .SqlWhere = " WHERE convert(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf


            If CStr2(Common.WebRequest.Data("StoreType")) <> "" Then
                .SqlWhere += " AND SubFOWSTTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreType"))) & vbCrLf
            End If

            If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
                .SqlWhere += " AND StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
            End If

            If Not FOWSTID = vbNullString Then
                .SqlWhere &= " AND FOWSTID IN (" & FOWSTID & ")" & vbCrLf
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= "AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            '.SqlGroup = "GROUP BY " & vbCrLf &
            '            "CONVERT(date,InvoiceDate) " & vbCrLf &
            '            ", FOWSTCode " & vbCrLf &
            '            ", FOWSTName " & vbCrLf &
            '            ", ISNULL(StoreType,'') " & vbCrLf &
            '            ", ISNULL(StoreLocation,'') " & vbCrLf &
            '            ", ISNULL(GroupName,'') " & vbCrLf &
            '            ", ISNULL(GrossSalesNetOfVATExemptions,0) " & vbCrLf &
            '            ", TransactionCount " & vbCrLf &
            '            ", DeliverySales " & vbCrLf &
            '            ", DeliveryTransactionCount " & vbCrLf &
            '            ", WalkInSales " & vbCrLf &
            '            ", WalkInTransactionCount " & vbCrLf

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetADSandATCperStoreReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransactionDate", SqlDbType.UniqueIdentifier)
                    .Expression = "CONVERT(date,InvoiceDate)"
                End With
                With .Add("FOWSTCode", SqlDbType.UniqueIdentifier)
                    .Expression = "FOWSTCode"
                End With
                With .Add("FOWSTName", SqlDbType.UniqueIdentifier)
                    .Expression = "FOWSTName"
                End With
                With .Add("StoreType", SqlDbType.VarChar)
                    .Expression = "ISNULL(StoreType,'')"
                End With
                With .Add("StoreLocation", SqlDbType.VarChar)
                    .Expression = "ISNULL(StoreLocation,'')"
                End With
                With .Add("GroupName", SqlDbType.Int)
                    .Expression = "ISNULL(GroupName,'')"
                End With
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(GrossSalesNetOfVATExemptions,0)"
                End With
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "TransactionCount"
                End With
                With .Add("DeliverySales", SqlDbType.Bit)
                    .Expression = "DeliverySales"
                End With
                With .Add("DeliveryCount", SqlDbType.Bit)
                    .Expression = "DeliveryTransactionCount"
                End With
                With .Add("WalkInSales", SqlDbType.Bit)
                    .Expression = "WalkInSales"
                End With
                With .Add("WalkInCount", SqlDbType.Bit)
                    .Expression = "WalkInTransactionCount"
                End With

                With .Add("OperatingDaysCount", SqlDbType.Bit)
                    .Expression = "COUNT(DISTINCT INVOICEDATE)"
                End With

                With .Add("LatestTransactionDate", SqlDbType.Bit)
                    .Expression = "MAX(InvoiceDate)"
                End With

            End With

            .SqlFrom = "FROM CacheInvoicesDaily "

            .SqlWhere = " WHERE convert(date, InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf


            If CStr2(Common.WebRequest.Data("StoreType")) <> "" Then
                .SqlWhere += " AND SubFOWSTTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreType"))) & vbCrLf
            End If

            If CStr2(Common.WebRequest.Data("StoreLocationType")) <> "" Then
                .SqlWhere += " AND StoreLocationTypeID = " & ReadyString(CStr2(Common.WebRequest.Data("StoreLocationType"))) & vbCrLf
            End If

            If Not FOWSTID = vbNullString Then
                .SqlWhere &= " AND FOWSTID IN (" & FOWSTID & ")" & vbCrLf
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= "AND FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "CONVERT(date,InvoiceDate) " & vbCrLf &
                        ", FOWSTCode " & vbCrLf &
                        ", FOWSTName " & vbCrLf &
                        ", ISNULL(StoreType,'') " & vbCrLf &
                        ", ISNULL(StoreLocation,'') " & vbCrLf &
                        ", ISNULL(GroupName,'') " & vbCrLf &
                        ", ISNULL(GrossSalesNetOfVATExemptions,0) " & vbCrLf &
                        ", TransactionCount " & vbCrLf &
                        ", DeliverySales " & vbCrLf &
                        ", DeliveryTransactionCount " & vbCrLf &
                        ", WalkInSales " & vbCrLf &
                        ", WalkInTransactionCount " & vbCrLf

            pDT = .Requery

        End With

        Return pDT
    End Function
    Function GetSalesPaymentInformationReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransactionDate", SqlDbType.UniqueIdentifier)
                    .Expression = "CONVERT(datetime,CacheInvoices.InvoiceDate)"
                End With
                With .Add("FOWSTName", SqlDbType.UniqueIdentifier)
                    .Expression = "CacheInvoices.FOWSTName"
                End With
                With .Add("InvoiceNumber", SqlDbType.VarChar)
                    .Expression = "ISNULL(CacheInvoices.InvoiceNo,'')"
                End With
                With .Add("PaymentType", SqlDbType.VarChar)
                    .Expression = "ISNULL(PaymentType,'')"
                End With
                With .Add("Amount", SqlDbType.Int)
                    .Expression = "ISNULL(Amount,0)"
                End With
                With .Add("GCashRefNo", SqlDbType.Bit)
                    .Expression = "ISNULL(GCashRefNo,'')"
                End With
                With .Add("InvoiceID", SqlDbType.Bit)
                    .Expression = "TransCustomerPaymentInvoices.InvoiceID"
                End With
            End With

            .SqlFrom = "FROM TransCustomerPaymentOther " & vbCrLf &
                        "INNER JOIN TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentOther.CustomerPaymentID " & vbCrLf &
                        "INNER JOIN CacheInvoices on CacheInvoices.ID = TransCustomerPaymentInvoices.InvoiceID " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            If Not FOWSTID = vbNullString Then
                .SqlWhere &= " AND CacheInvoices.FOWSTID IN (" & FOWSTID & ")" & vbCrLf
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= "AND CacheInvoices.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetCrewOnDutyReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("TransactionDate", SqlDbType.UniqueIdentifier)
                    .Expression = "CONVERT(date,CacheInvoices.InvoiceDate)"
                End With
                With .Add("StoreName", SqlDbType.UniqueIdentifier)
                    .Expression = "CacheInvoices.FOWSTName"
                End With
                With .Add("GroupName", SqlDbType.VarChar)
                    .Expression = "ISNULL(CacheInvoices.GroupName,'')"
                End With
                With .Add("CrewName", SqlDbType.VarChar)
                    .Expression = "ISNULL(CacheInvoices.UpdatedByName,'')"
                End With

            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            If Not FOWSTID = vbNullString Then
                .SqlWhere &= " AND CacheInvoices.FOWSTID IN (" & FOWSTID & ")" & vbCrLf
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= " AND CacheInvoices.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            .SqlOrder = " ORDER BY CONVERT(date,InvoiceDate), FOWSTName "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetFoodCostReport() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("TransactionDate", SqlDbType.UniqueIdentifier)
                    .Expression = "CONVERT(date,CacheInvoices.InvoiceDate)"
                End With
                With .Add("StoreName", SqlDbType.UniqueIdentifier)
                    .Expression = "CacheInvoices.FOWSTName"
                End With
                With .Add("GroupName", SqlDbType.VarChar)
                    .Expression = "ISNULL(CacheInvoices.GroupName,'')"
                End With
                With .Add("CrewName", SqlDbType.VarChar)
                    .Expression = "ISNULL(CacheInvoices.UpdatedByName,'')"
                End With

            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

            If Not FOWSTID = vbNullString Then
                .SqlWhere &= " AND CacheInvoices.FOWSTID IN (" & FOWSTID & ")" & vbCrLf
            End If

            If Not GroupID = vbNullString Then
                .SqlWhere &= " AND CacheInvoices.FranchiseGroupID IN (" & GroupID & ")" & vbCrLf
            End If

            .SqlOrder = " ORDER BY CONVERT(date,InvoiceDate), FOWSTName "

            pDT = .Requery

        End With

        Return pDT
    End Function

    'Management Reports 12/17/2023

    Function GetSalesByDate() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                With .Add("GrossSales", SqlDbType.Decimal)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With

                With .Add("StoreCount", SqlDbType.Decimal)
                    .Expression = "COUNT(DISTINCT FOWSTID)"
                End With

                With .Add("TransactionCount", SqlDbType.Decimal)
                    .Expression = "COUNT(DISTINCT ID)"
                End With

            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = "WHERE CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND CacheInvoices.FowstID IN (select distinct FowstID from LibraryOrganizationEmployeeFOWST WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleID) & ") "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetDeliverySales() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                With .Add("GrossSales", SqlDbType.Decimal)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With

                With .Add("TransactionCount", SqlDbType.Decimal)
                    .Expression = "COUNT(DISTINCT ID)"
                End With

            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = "WHERE CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND (ISNULL(LocalDelivery,0)>0 or ISNULL(GrabFood,0)>0 or ISNULL(FoodPanda,0)>0 or ISNULL(Pickaroo,0)>0 or ISNULL(Riderko,0)>0) " & vbCrLf &
                        " AND CacheInvoices.FowstID IN (select distinct FowstID from LibraryOrganizationEmployeeFOWST WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleID) & ") "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetTakeOutSales() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                With .Add("GrossSales", SqlDbType.Decimal)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With

                With .Add("TransactionCount", SqlDbType.Decimal)
                    .Expression = "COUNT(DISTINCT ID)"
                End With

            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = "WHERE CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND (ISNULL(LocalDelivery,0)=0 AND ISNULL(GrabFood,0)=0 AND ISNULL(FoodPanda,0)=0 AND ISNULL(Pickaroo,0)=0 AND ISNULL(Riderko,0)=0) " & vbCrLf &
                        " AND CacheInvoices.FowstID IN (select distinct FowstID from LibraryOrganizationEmployeeFOWST WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleID) & ") "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetProductMixSales() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                With .Add("GrossSales", SqlDbType.Decimal)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With

                With .Add("TransactionCount", SqlDbType.Decimal)
                    .Expression = "COUNT(DISTINCT ID)"
                End With

            End With

            .SqlFrom = "FROM TransInvoiceItems " & vbCrLf &
                        " INNER JOIN TransInvoice on TransInvoice.ID = TransInvoiceItems.InvoiceID " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProduct on MobizLibraryPIMProduct.ID = TransInvoiceItems.ProductID"

            .SqlWhere = "WHERE CONVERT(date, InvoiceDate ) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND FowstID IN (select distinct FowstID from LibraryOrganizationEmployeeFOWST WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleID) & ") "

            .SqlGroup = " GROUP BY ProductName"

            pDT = .Requery

        End With

        Return pDT
    End Function

    '========================= Weekly Daily Sales  Report =================================
    Function GetSalesperOwnership() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("FromFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(FromFranchisee,0)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(FromFranchisee,0) "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetSalesperOwnershipDelivery() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("FromFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(FromFranchisee,0)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(FromFranchisee,0) "

            .SqlWhere = "WHERE CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND (ISNULL(LocalDelivery,0)>0 or ISNULL(GrabFood,0)>0 or ISNULL(FoodPanda,0)>0 or ISNULL(Pickaroo,0)>0 or ISNULL(Riderko,0)>0) "

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetSalesperOwnershipTakeOut() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("FromFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(FromFranchisee,0)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(FromFranchisee,0) "

            .SqlWhere = "WHERE CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND (ISNULL(LocalDelivery,0)=0 AND ISNULL(GrabFood,0)=0 AND ISNULL(FoodPanda,0)=0 AND ISNULL(Pickaroo,0)=0 AND ISNULL(Riderko,0)=0) " & vbCrLf

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetSalesperRegion() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("GroupName", SqlDbType.Bit)
                    .Expression = "ISNULL(GroupName,'Unassigned')"
                End With
                'With .Add("FromFranchisee", SqlDbType.Bit)
                '    .Expression = "ISNULL(FromFranchisee,0)"
                'End With
            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(GroupName,'Unassigned') "

            'ISNULL(FromFranchisee,0), 

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetSalesperLocationType() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoices.GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("StoreLocation", SqlDbType.Bit)
                    .Expression = "ISNULL(StoreLocation,'Unassigned')"
                End With
                With .Add("FromFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(FromFranchisee,0)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(FromFranchisee,0), ISNULL(StoreLocation,'Unassigned') "
            pDT = .Requery

        End With

        Return pDT
    End Function

    '======================== Weekly ADS Report ===========================================

    '========================= Weekly Transaction Count Report =================================

    Function GetTransactionCountperOwnership() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "ISNULL(COUNT(CacheInvoices.ID),0)"
                End With
                With .Add("FromFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(FromFranchisee,0)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(FromFranchisee,0) "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetTransactionCountperOwnershipDelivery() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "ISNULL(COUNT(CacheInvoices.ID),0)"
                End With
                With .Add("FromFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(FromFranchisee,0)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(FromFranchisee,0) "

            .SqlWhere = "WHERE CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND (ISNULL(LocalDelivery,0)>0 or ISNULL(GrabFood,0)>0 or ISNULL(FoodPanda,0)>0 or ISNULL(Pickaroo,0)>0 or ISNULL(Riderko,0)>0) "

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetTransactionCountperOwnershipTakeOut() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "ISNULL(COUNT(CacheInvoices.ID),0)"
                End With
                With .Add("FromFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(FromFranchisee,0)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(FromFranchisee,0) "

            .SqlWhere = "WHERE CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                        " AND (ISNULL(LocalDelivery,0)=0 AND ISNULL(GrabFood,0)=0 AND ISNULL(FoodPanda,0)=0 AND ISNULL(Pickaroo,0)=0 AND ISNULL(Riderko,0)=0) " & vbCrLf

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetTransactionCountperRegion() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "ISNULL(COUNT(CacheInvoices.ID),0)"
                End With
                With .Add("GroupName", SqlDbType.Bit)
                    .Expression = "ISNULL(GroupName,'Unassigned')"
                End With
                'With .Add("FromFranchisee", SqlDbType.Bit)
                '    .Expression = "ISNULL(FromFranchisee,0)"
                'End With
            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(GroupName,'Unassigned') "

            'ISNULL(FromFranchisee,0), 

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetTransactionCountperLocationType() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TransactionCount", SqlDbType.Bit)
                    .Expression = "ISNULL(COUNT(CacheInvoices.ID),0)"
                End With
                With .Add("StoreLocation", SqlDbType.Bit)
                    .Expression = "ISNULL(StoreLocation,'Unassigned')"
                End With
                With .Add("FromFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(FromFranchisee,0)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoices " & vbCrLf

            .SqlWhere = " WHERE convert(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(FromFranchisee,0), ISNULL(StoreLocation,'Unassigned') "
            pDT = .Requery

        End With

        Return pDT
    End Function


    '========================= Monthly Sales  Report =================================

    Function GetMonthlySalesperOwnership() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesMonthly.GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("FromFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(FromFranchisee,0)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoicesMonthly " & vbCrLf

            .SqlWhere = " WHERE InvoiceMonth = " & InvoiceMonth & " AND InvoiceYear = " & InvoiceYear

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(FromFranchisee,0) "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetMonthlySalesperOwnershipDelivery() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesMonthly.GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("FromFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(FromFranchisee,0)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoicesMonthly " & vbCrLf

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(FromFranchisee,0) "

            .SqlWhere = " WHERE InvoiceMonth = " & InvoiceMonth & " AND InvoiceYear = " & InvoiceYear & vbCrLf &
                        " AND (ISNULL(LocalDelivery,0)>0 or ISNULL(GrabFood,0)>0 or ISNULL(FoodPanda,0)>0 or ISNULL(Pickaroo,0)>0 or ISNULL(Riderko,0)>0) "

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetMonthlySalesperOwnershipTakeOut() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesMonthly.GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("FromFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(FromFranchisee,0)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoicesMonthly " & vbCrLf

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(FromFranchisee,0) "

            .SqlWhere = " WHERE InvoiceMonth = " & InvoiceMonth & " AND InvoiceYear = " & InvoiceYear & vbCrLf &
                        " AND (ISNULL(LocalDelivery,0)=0 AND ISNULL(GrabFood,0)=0 AND ISNULL(FoodPanda,0)=0 AND ISNULL(Pickaroo,0)=0 AND ISNULL(Riderko,0)=0) " & vbCrLf

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetMonthlySalesperRegion() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesMonthly.GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("GroupName", SqlDbType.Bit)
                    .Expression = "ISNULL(GroupName,'Unassigned')"
                End With
                'With .Add("FromFranchisee", SqlDbType.Bit)
                '    .Expression = "ISNULL(FromFranchisee,0)"
                'End With
            End With

            .SqlFrom = "FROM CacheInvoicesMonthly " & vbCrLf

            .SqlWhere = " WHERE InvoiceMonth = " & InvoiceMonth & " AND InvoiceYear = " & InvoiceYear

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(GroupName,'Unassigned') "

            'ISNULL(FromFranchisee,0), 

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function GetMonthlySalesperLocationType() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("GrossSales", SqlDbType.Bit)
                    .Expression = "ISNULL(SUM(CacheInvoicesMonthly.GrossSalesNetOfVATExemptions),0)"
                End With
                With .Add("StoreLocation", SqlDbType.Bit)
                    .Expression = "ISNULL(StoreLocation,'Unassigned')"
                End With
                With .Add("FromFranchisee", SqlDbType.Bit)
                    .Expression = "ISNULL(FromFranchisee,0)"
                End With
            End With

            .SqlFrom = "FROM CacheInvoicesMonthly " & vbCrLf

            .SqlWhere = " WHERE InvoiceMonth = " & InvoiceMonth & " AND InvoiceYear = " & InvoiceYear

            'filter test stores
            .SqlWhere += " AND FranchiseGroupID <> 'c68f3e5e-bf1c-4208-b849-87ca5cbcd991' and StoreLocation IS NOT NULL "

            .SqlGroup = "GROUP BY " & vbCrLf &
                        "ISNULL(FromFranchisee,0), ISNULL(StoreLocation,'Unassigned') "
            pDT = .Requery

        End With

        Return pDT
    End Function

    'cash flow query
    '    select Name, DepositedByName, BankBranch, ServiceCharge, DepositReferenceNo, TotalAmount, TotalCash, TotalAlipay, TotalDiscountVoucher, TotalEPlus, TotalFoodPanda, TotalGCash, TotalGoldCard, TotalGrabFood, TotalLocalDelivery, TotalPayMaya, TotalShopeeEVoucher, TotalShopeePay, TotalSMAC, TotalVPlus from TransCashHandling
    'inner join MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = TransCashHandling.FOWSTID

    Function GetDeviceUtilizationDetails()
        Dim pSql As String
        Dim pDT As DataTable
        Dim clsSecurityRole As New SecurityRole
        Dim groupname As String = " ISNULL(LibraryFranchiseGroup.GroupName, '') "

        If WebRequest.Instance.OrgID.ToLower() = "d362ffdb-15d1-451f-97dc-c567b94fb410" Then
            groupname = " ISNULL(LibraryFranchiseGroup.GroupName,CacheInvoices.GroupName) "
        End If

        pSql = "Select DISTINCT MobizLibraryOrganizationFOWST.ID, MobizLibraryOrganizationFOWST.Code as StoreCode, MobizLibraryOrganizationFOWST.Name as FowstName, " & groupname & " GroupName " & vbCrLf &
                " , ISNULL(count(distinct convert(date,invoicedate)),0) Usage " & vbCrLf &
                " FROM MobizLibraryOrganizationFOWST " & vbCrLf &
                " LEFT JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                " LEFT JOIN LibraryFranchiseGroup on MobizLibraryOrganizationFranchisee.FranchiseGroupID = LibraryFranchiseGroup.ID " & vbCrLf &
                " LEFT JOIN CacheInvoices on MobizLibraryOrganizationFOWST.ID = CacheInvoices.FowstID and convert(date,InvoiceDate) Between " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf

        If Not CStr2(FOWSTID) = vbNullString Then
            If FOWSTID = "All Stores" Then
                pSql &= " WHERE FOWSTID IN (SELECT ID FROM MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)
                If WebRequest.Instance.OrgID.ToLower() = "d362ffdb-15d1-451f-97dc-c567b94fb410" Then
                    'if PC CO database
                    pSql &= " OR OrganizationID = " & ReadyString("892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba")
                End If
                pSql &= " AND ISNULL(IsActive, 1) = 1 AND (FOWSTTypeID <> 'dff0f087-4c2b-4b67-aa1d-502f02773e2b' OR FOWSTTypeID IS NULL)) " & vbCrLf
            Else
                pSql &= " WHERE FOWSTID = " & ReadyString(FOWSTID) & vbCrLf
            End If
        End If

        If Not CStr2(GroupID) = vbNullString Then
            pSql &= " WHERE MobizLibraryOrganizationFranchisee.FranchiseGroupID IN (" & GroupID & ") " & vbCrLf

            If clsSecurityRole.CheckSecurityCurrentUserAcccess("Show only the assigned FOWST/Group to the user", WebRequest.Instance.OrgID, WebSession.Instance.PeopleUserID).Rows.Count > 0 Then
                pSql &= " AND MobizLibraryOrganizationFOWST.ID IN (" & vbCrLf &
                    " SELECT " & vbCrLf &
                    "   FranchiseeOrgFowstID " & vbCrLf &
                    "   FROM MobizLibraryOrganizationFranchisee " & vbCrLf &
                    "   WHERE FranchisorOrgID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                    "   AND FranchiseGroupID IN (" & GroupID & ") " & vbCrLf &
                    "   AND FranchiseeOrgFowstID IN (SELECT " & vbCrLf &
                    "   CONVERT(varchar(max), FowstID) as FowstID " & vbCrLf &
                    "   FROM LibraryOrganizationEmployeeFOWST " & vbCrLf &
                    "   WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                    " ) GROUP BY FranchiseeOrgFowstID) " & vbCrLf
            End If
        End If

        If Not String.IsNullOrWhiteSpace(StoreStatus) Then
            pSql &= " AND (ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = " & ReadyString(StoreStatus) & vbCrLf

            If StoreStatus = "Store and device are operational." Then
                'Available
                pSql &= " OR ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Available'" & vbCrLf
            ElseIf StoreStatus = "Store is permanently closed." Then
                'Closed
                pSql &= " OR ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Closed'" & vbCrLf
            ElseIf StoreStatus = "Store is temporarily closed for holiday." Then
                'Termporary Closed
                pSql &= " OR ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Temporary Closed'" & vbCrLf
            End If
            pSql &= ")" & vbCrLf
        End If

        pSql &= " AND " & groupname & " != '' "
        pSql &= " GROUP BY MobizLibraryOrganizationFOWST.ID, MobizLibraryOrganizationFOWST.Code, MobizLibraryOrganizationFOWST.Name, " & groupname & " "
        pSql &= " ORDER BY Usage, " & groupname & ", MobizLibraryOrganizationFOWST.Name "

        Common.WebRequest.ProData.CommandTimeout = 600000
        Dim arrFOWST As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

        For i As Integer = 0 To arrFOWST.Length - 1
            pSql = " SELECT DISTINCT CONVERT(varchar, convert(date,InvoiceDate)) as Date " & vbCrLf &
                 " FROM CacheInvoices " & vbCrLf &
                 " WHERE FowstID = " & ReadyString(arrFOWST(i)("ID").ToString()) & vbCrLf &
                 " AND CONVERT(date,InvoiceDate) Between " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                 " ORDER BY CONVERT(varchar, convert(date,InvoiceDate)) " & vbCrLf

            arrFOWST(i).Add("DateUsage", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

            pSql = " SELECT opera.WeekDay " & vbCrLf &
                    " FROM MobizLibraryOrganizationFOWST fowst " & vbCrLf &
                    " INNER JOIN MobizLibraryOrganization org ON org.ID = fowst.OrganizationID " & vbCrLf &
                    " INNER JOIN MobizLibraryOrganizationFOWSTOperationHRs as opera ON opera.FowstID = fowst.ID AND opera.IsClosed = 1 --AND opera.WeekDay = (SELECT DATEPART(dw,CONVERT(date, " & ReadyString(DateFrom) & ")))" & vbCrLf &
                    " WHERE fowst.ID = " & ReadyString(arrFOWST(i)("ID").ToString()) & vbCrLf

            arrFOWST(i).Add("OperationHours", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
        Next

        Return arrFOWST
    End Function

    Function SendEmailDeviceUtilizationDetails()
        Try
            Dim fromDate As Date = CDate(DateFrom)
            Dim toDate As Date = CDate(DateTo)
            Dim ctrRow As Integer = 0
            Dim ctrColDate As Integer = 0
            Dim colsDate As String = ""
            Dim headerStyles = "padding: 10px; text-align:center; white-space:nowrap; font-weight:bold;"
            Dim bodyStyles = "padding: 5px; white-space:nowrap; "
            Dim theadDeviceUtilUsageDetails = "<thead><tr style='background-color:#CCC'>"
            Dim tbodyDeviceUtilUsageDetails = "<tbody>"

            theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "' >#</th>"
            theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Store Name</th>"
            theadDeviceUtilUsageDetails &= "<th style='" + headerStyles + "'>Region</th>"

            While (fromDate <= toDate)
                theadDeviceUtilUsageDetails += "<th style='" + headerStyles + "'>" + fromDate.ToString("MMM dd") + "</th>"
                colsDate += "<td style='" + bodyStyles + " text-align:center;'></td>"
                ctrColDate += 1
                fromDate = DateAdd("d", 1, fromDate)
            End While

            fromDate = CDate(DateFrom)
            theadDeviceUtilUsageDetails += "<th style='" + headerStyles + "'>Usage</th>"
            theadDeviceUtilUsageDetails += "</tr></thead>"

            Dim arrFowst = GetDeviceUtilizationDetails()

            For Each store As Object In arrFowst
                ctrRow += 1
                Dim tr As String = "<tr style='background-color:  " & IIf(ctrRow Mod 2 = 0, "#f5f5f5", "#FFFFFF") & "'><td style='" & bodyStyles & " text-align:Left();'>" & ctrRow & ".</td>" &
                "<td style='" & bodyStyles & " text-align:left'>" & store("FowstName") & "</td>" &
                "<td style='" & bodyStyles & " text-align:left'>" & store("GroupName") & "</td>"

                If store("DateUsage").Length > 0 Then
                    While (fromDate <= toDate)
                        Dim isUsage As Boolean = False

                        For Each dit As Object In store("DateUsage")
                            If CDate(dit("Date")).ToString("yyyy-MM-dd") = CDate(fromDate).ToString("yyyy-MM-dd") Then
                                isUsage = True
                                Exit For
                            End If
                        Next

                        If isUsage Then
                            tr &= "<td style='" + bodyStyles + " text-align:center;'>Yes</td>"
                        Else
                            tr &= "<td style='" + bodyStyles + " text-align:center;'></td>"
                        End If
                        fromDate = DateAdd("d", 1, fromDate)
                    End While

                    fromDate = CDate(DateFrom)
                    tr &= "<td style='" & bodyStyles & " text-align:center;'>" & store("DateUsage").Length & " of " & ctrColDate & " day(s)</td>"
                Else
                    tr &= colsDate
                    tr &= "<td style='" + bodyStyles + " text-align:center;'></td>"
                End If

                tr &= "</tr>"

                tbodyDeviceUtilUsageDetails &= tr
            Next

            tbodyDeviceUtilUsageDetails &= "</tbody>"

            Dim msg As String = ""

            msg &= "<table border='0' cellpadding='5' cellspacing='0' style='width:100%'>" & theadDeviceUtilUsageDetails & "" & tbodyDeviceUtilUsageDetails & "</table>"

            SendMail("customercare@mobiz-apps.com",
                        "arniepeloton17@gmail.com, francis.hernandez@mobiz-apps.com",
                        "1Mobiz Inc.",
                        "Device Utilization",
                        msg,
                        "smtp-mail.outlook.com",
                         587,
                         "customercare@mobiz-apps.com",
                         "MobizCustCare2221", , , , , True)

            Return 1
        Catch ex As Exception
            AppendToDebug(ex.Message)
            Return 0
        End Try
    End Function

    Function GenerateExcelDeviceUtilizationDetails()
        Try
            Dim fromDate As Date = CDate(DateFrom)
            Dim toDate As Date = CDate(DateTo)
            Dim ctrColDate As Integer = 0
            Dim pWB As Aspose.Cells.Workbook
            Dim pWS As Aspose.Cells.Worksheet

            pWB = New Aspose.Cells.Workbook
            pWS = pWB.Worksheets(0)

            pWS.Cells(0, 0).PutValue("Store Name")
            pWS.Cells(0, 0).Style.ForegroundColor = Color.FromArgb(146, 190, 255)
            pWS.Cells(0, 0).Style.HorizontalAlignment = TextAlignmentType.Center
            pWS.Cells(0, 0).Style.Pattern = BackgroundType.Solid
            pWS.Cells(0, 0).Style.Font.Size = 11
            pWS.Cells(0, 0).Style.Font.IsBold = True

            pWS.Cells(0, 1).PutValue("Region")
            pWS.Cells(0, 1).Style.ForegroundColor = Color.FromArgb(146, 190, 255)
            pWS.Cells(0, 1).Style.HorizontalAlignment = TextAlignmentType.Center
            pWS.Cells(0, 1).Style.Pattern = BackgroundType.Solid
            pWS.Cells(0, 1).Style.Font.Size = 11
            pWS.Cells(0, 1).Style.Font.IsBold = True

            Dim storeCtr As Integer = 1 'start row 2
            Dim dateCtr As Integer = 2 ' row 1 column C

            While (fromDate <= toDate)
                pWS.Cells(0, dateCtr).PutValue(fromDate.ToString("MMM dd, yyyy"))
                pWS.Cells(0, dateCtr).Style.ForegroundColor = Color.FromArgb(146, 190, 255)
                pWS.Cells(0, dateCtr).Style.HorizontalAlignment = TextAlignmentType.Center
                pWS.Cells(0, dateCtr).Style.Pattern = BackgroundType.Solid
                pWS.Cells(0, dateCtr).Style.Font.Size = 11
                pWS.Cells(0, dateCtr).Style.Font.IsBold = True
                dateCtr += 1
                ctrColDate += 1
                fromDate = DateAdd("d", 1, fromDate)
            End While

            pWS.Cells(0, dateCtr).PutValue("Usage")
            pWS.Cells(0, dateCtr).Style.ForegroundColor = Color.FromArgb(146, 190, 255)
            pWS.Cells(0, dateCtr).Style.HorizontalAlignment = TextAlignmentType.Center
            pWS.Cells(0, dateCtr).Style.Pattern = BackgroundType.Solid
            pWS.Cells(0, dateCtr).Style.Font.Size = 11
            pWS.Cells(0, dateCtr).Style.Font.IsBold = True

            Dim arrFowst = GetDeviceUtilizationDetails()
            fromDate = CDate(DateFrom)

            For Each store As Object In arrFowst
                pWS.Cells(storeCtr, 0).PutValue(store("FowstName"))
                pWS.Cells(storeCtr, 1).PutValue(store("GroupName"))

                If store("DateUsage").Length > 0 Then
                    dateCtr = 2
                    While (fromDate <= toDate)
                        Dim isUsage As Boolean = False

                        For Each dit As Object In store("DateUsage")
                            If CDate(dit("Date")).ToString("yyyy-MM-dd") = CDate(fromDate).ToString("yyyy-MM-dd") Then
                                isUsage = True
                                Exit For
                            End If
                        Next

                        If isUsage Then
                            pWS.Cells(storeCtr, dateCtr).PutValue("Yes")
                            pWS.Cells(storeCtr, dateCtr).Style.HorizontalAlignment = TextAlignmentType.Center
                        End If

                        dateCtr += 1
                        fromDate = DateAdd("d", 1, fromDate)
                    End While

                    fromDate = CDate(DateFrom)
                End If

                pWS.Cells(storeCtr, ctrColDate + 2).PutValue(store("DateUsage").Length & " of " & ctrColDate & " day(s)")
                pWS.Cells(storeCtr, ctrColDate + 2).Style.HorizontalAlignment = TextAlignmentType.Center

                storeCtr += 1
            Next

            pWS.Cells.StandardHeight = 30
            pWS.FreezePanes(1, 1, 1, 0)
            pWS.AutoFitColumns()

            Dim filesource = Common.ServerPath & "temp\DeviceUtilization\0259519d-1988-47e6-9fa5-b723351be852.xlsx"

            pWB.Save(filesource)

            Return 1
        Catch ex As Exception
            AppendToDebug(ex.Message)
            Return 0
        End Try
    End Function

    '================================ Device Utilization Dashboard =======================================

    Function GetOrganizationWideUtilization() As DataTable
        Dim dt As New DataTable
        Dim pSql As String
        Dim pDate As String = Common.WebRequest.Data("Date")
        pSql = "Select [Status], COUNT(*) Count from TransStoreDeviceUtilization " & vbCrLf &
                "WHERE convert(date,[date]) = convert(date," & ReadyString(pDate) & ")" & vbCrLf &
                "AND GroupName NOT LIKE '%Third Party POS%' " & vbCrLf &
                "Group by Status " & vbCrLf &
                "Order by Status DESC"

        dt = Common.WebRequest.ProData.Execute(pSql)

        Return dt

    End Function

    Function GetUtilizationByOwnershipType() As DataTable
        Dim dt As New DataTable
        Dim pSql As String
        Dim pDate As String = Common.WebRequest.Data("Date")
        pSql = "Select OwnershipType, [Status], COUNT(*) Count from TransStoreDeviceUtilization " & vbCrLf &
                "WHERE convert(date,[date]) = convert(date," & ReadyString(pDate) & ")" & vbCrLf &
                "AND GroupName NOT LIKE '%Third Party POS%' " & vbCrLf &
                "Group by OwnershipType, Status " & vbCrLf &
                "Order by OwnershipType, Status DESC "

        dt = Common.WebRequest.ProData.Execute(pSql)

        Return dt

    End Function

    Function GetUtilizationByOwnershipTypeAndGroup() As DataTable
        Dim dt As New DataTable
        Dim pSql As String
        Dim pDate As String = Common.WebRequest.Data("Date")
        pSql = "Select OwnershipType, GroupName, [Status], COUNT(*) Count from TransStoreDeviceUtilization " & vbCrLf &
                "WHERE convert(date,[date]) = convert(date," & ReadyString(pDate) & ")" & vbCrLf &
                "AND GroupName NOT LIKE '%Third Party POS%' " & vbCrLf &
                "Group by OwnershipType, GroupName, Status " & vbCrLf &
                "Order by OwnershipType, GroupName, Status DESC"

        dt = Common.WebRequest.ProData.Execute(pSql)

        Return dt

    End Function

    Function GetCountStoreWithTheFewestNoOfTransactions() As DataTable
        Dim dt As New DataTable
        Dim pSql As String
        Dim pDate As String = Common.WebRequest.Data("Date")
        Dim cnt As String = Common.WebRequest.Data("Count")
        Dim Ownership As String = Common.WebRequest.Data("Ownership")
        pSql = "Select OwnershipType, COUNT(ID) as Count from TransStoreDeviceUtilization " & vbCrLf &
                "WHERE convert(date,[date]) = convert(date," & ReadyString(pDate) & ") AND Status = 'Utilized'" & vbCrLf &
                " AND ISNULL(TransactionCount, 0) = " & cnt & vbCrLf &
                " GROUP by OwnershipType "

        dt = Common.WebRequest.ProData.Execute(pSql)

        Return dt

    End Function

    Function GetStoreWithTheFewestNoOfTransactions() As DataTable
        Dim dt As New DataTable
        Dim pSql As String
        Dim pDate As String = Common.WebRequest.Data("Date")
        Dim cnt As String = Common.WebRequest.Data("Count")
        Dim Ownership As String = Common.WebRequest.Data("Ownership")
        pSql = "Select OwnershipType as Ownership, GroupName as Region, FOWSTCode as [Store Code], FOWSTName as [Store Name], Status from TransStoreDeviceUtilization " & vbCrLf &
                "WHERE convert(date,[date]) = convert(date," & ReadyString(pDate) & ") AND Status = 'Utilized'" & vbCrLf &
                " AND ISNULL(TransactionCount, 0) = " & cnt & vbCrLf &
                IIf(Ownership = "All", "", " AND OwnershipType = " & ReadyString(Ownership) & vbCrLf) &
                "Order by OwnershipType, GroupName, FOWSTName"

        dt = Common.WebRequest.ProData.Execute(pSql)

        Return dt

    End Function

    Function GetDeviceWithoutSalesData() As DataTable
        Dim pSql As String
        Dim pDT As New DataTable

        'pSql = "Select name from MobizLibraryOrganizationFranchisee " & vbCrLf &
        '    "INNER JOIN MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
        '    " WHERE MobizLibraryOrganizationFranchisee.FranchiseGroupID = " & Common.WebRequest.Data("GroupFOWSTID") & vbCrLf &
        '    " AND MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID NOT IN (Select DISTINCT FOWSTID FROM CacheInvoicesDaily WHERE convert(date, CacheInvoicesDaily.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & ")"

        pSql = "Select DISTINCT FOWSTName from TransStoreDeviceUtilization  " & vbCrLf &
                "WHERE TransStoreDeviceUtilization.GroupID= " & Common.WebRequest.Data("GroupFOWSTID") & vbCrLf &
                " AND Status = 'Not Utilized' AND convert(date, [Date]) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo) & vbCrLf &
                " AND TransStoreDeviceUtilization.FowstID IN (select distinct FowstID from LibraryOrganizationEmployeeFOWST WHERE PeopleID = " & ReadyString(WebSession.Instance.PeopleID) & ") "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    Function GetStoreUtilStatus() As DataTable
        Dim pSql As String
        Dim ownership As String = Common.WebRequest.Data("Ownership")
        Dim status As String = Common.WebRequest.Data("Status")
        Dim pDate As String = Format(CDate(Common.WebRequest.Data("Date")), "MM-dd-yyyy")

        pSql = " SELECT GroupName as Region, FOWSTCode as [Store Code], FOWSTName as [Store Name], Status " & vbCrLf &
                " FROM TransStoreDeviceUtilization " & vbCrLf &
                " WHERE convert(date,[date]) = convert(date," & ReadyString(pDate) & ") " & vbCrLf &
                "AND GroupName NOT LIKE '%Third Party POS%' " & vbCrLf &
                IIf(status = "All", "", " AND Status = " & ReadyString(status)) & vbCrLf

        If ownership = "All" Then
            pSql &= ""
        ElseIf ownership = "CO" Then
            pSql &= " AND GroupName LIKE 'CO -%' "
        ElseIf ownership = "FO" Then
            pSql &= " AND (GroupName LIKE 'FO -%' OR GroupName LIKE '%Cebu%')"
        End If

        pSql &= " ORDER BY GroupName, FowstName " & vbCrLf

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetFORegions() As DataTable
        Dim pSql As String

        pSql = " SELECT DISTINCT GroupName " & vbCrLf &
                " FROM TransStoreDeviceUtilization " & vbCrLf &
                " WHERE GroupName LIKE 'FO -%' OR GroupName LIKE '%Cebu%' " & vbCrLf &
                " GROUP BY GroupName " & vbCrLf &
                " ORDER BY GroupName " & vbCrLf

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetDetailedDirectManagerReport(Optional dmid As String = "", Optional region As String = "") As DataTable
        Dim pSql As String = ""
        Dim dateFrom As String = Format(CDate(Common.WebRequest.Data("DateFrom")), "MM-dd-yyyy")
        Dim dateTo As String = Format(CDate(Common.WebRequest.Data("DateTo")), "MM-dd-yyyy")

        If String.IsNullOrWhiteSpace(region) Then
            region = Common.WebRequest.Data("Region")
        End If

        If String.IsNullOrWhiteSpace(dmid) Then
            dmid = Common.WebRequest.Data("DMID")
        End If

        pSql += " DECLARE @cols NVARCHAR(MAX); " & vbCrLf
        pSql += " DECLARE @query NVARCHAR(MAX); " & vbCrLf

        pSql += " SET @cols = STUFF(( " & vbCrLf &
                " SELECT DISTINCT ',' + QUOTENAME(CONVERT(date, [date])) " & vbCrLf &
                " FROM TransStoreDeviceUtilization " & vbCrLf &
                " WHERE CONVERT(date, [date]) BETWEEN " & ReadyString(dateFrom) & " AND " & ReadyString(dateTo) & vbCrLf &
                " FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)') " & vbCrLf &
                " , 1, 1, ''); " & vbCrLf

        pSql += " SET @query = ' " & vbCrLf &
                "   SELECT * " & vbCrLf &
                "   FROM ( SELECT Region, [Manager ID], (SELECT FirstName + '' '' + Surname FROM MobizLibraryPeople WHERE ID = [Manager ID]) as [Manager Name], FOWSTID, [Store Code], [Store Name], Period, Status FROM (" & vbCrLf &
                "       SELECT GroupName as Region, ManagerID2 as [Manager ID], FOWSTID, FOWSTCode as [Store Code], FOWSTName as [Store Name], CONVERT(date, [date]) as Period, Status " & vbCrLf &
                "       FROM TransStoreDeviceUtilization " & vbCrLf &
                "       WHERE " & IIf(region = "All", "(GroupName LIKE (''FO -%'') OR GroupName LIKE (''%Cebu%'')) AND ", " GroupName = '" & ReadyString(region) & "' AND ") & vbCrLf &
                "       CONVERT(date, [date]) BETWEEN '" & ReadyString(dateFrom) & "' AND '" & ReadyString(dateTo) & "' " & vbCrLf &
                "       AND ManagerID1 = '" & ReadyString(dmid) & "' " & vbCrLf &
                "       UNION ALL " & vbCrLf &
                "       SELECT GroupName as Region, ManagerID1 as [Manager ID], FOWSTID, FOWSTCode as [Store Code], FOWSTName as [Store Name], CONVERT(date, [date]) as Period, Status " & vbCrLf &
                "       FROM TransStoreDeviceUtilization " & vbCrLf &
                "       WHERE " & IIf(region = "All", "(GroupName LIKE (''FO -%'') OR GroupName LIKE (''%Cebu%'')) AND ", " GroupName = '" & ReadyString(region) & "' AND ") & vbCrLf &
                "       CONVERT(date, [date]) BETWEEN '" & ReadyString(dateFrom) & "' AND '" & ReadyString(dateTo) & "' " & vbCrLf &
                "       AND ManagerID2 = '" & ReadyString(dmid) & "' " & vbCrLf &
                "       ) tbl GROUP BY Region, [Manager ID], FOWSTID, [Store Code], [Store Name], Period, Status " & vbCrLf &
                " ) src " & vbCrLf &
                " PIVOT ( " & vbCrLf &
                "   MAX(Status) FOR Period IN (' + @cols + ') " & vbCrLf &
                " ) pvt ORDER BY Region, [Manager Name], [Store Name]'; " & vbCrLf &
                "" & vbCrLf &
                " EXEC sp_executesql @query; " & vbCrLf

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetUtilSummaryPerRegion()
        Dim pSql As String = ""
        Dim dateFrom As String = Format(CDate(Common.WebRequest.Data("DateFrom")), "MM-dd-yyyy")
        Dim dateTo As String = Format(CDate(Common.WebRequest.Data("DateTo")), "MM-dd-yyyy")
        Dim region As String = Common.WebRequest.Data("Region")

        'Get All Distict Managers
        pSql = " SELECT GroupName, ManagerID, MAX(ManagerName) AS ManagerName, RoleName FROM (SELECT DISTINCT * FROM ( " & vbCrLf &
                "   SELECT DISTINCT GroupName, ManagerID1 as ManagerID, Manager1 as ManagerName, role.RoleName " & vbCrLf &
                "   FROM TransStoreDeviceUtilization as util " & vbCrLf &
                "   LEFT JOIN LibraryOrganizationEmployee as emp ON emp.PeopleID = ManagerID1 " & vbCrLf &
                "   LEFT JOIN LibraryDTRRole as role ON role.ID = emp.RoleTagID " & vbCrLf &
                "   WHERE " & IIf(region = "All", "(GroupName LIKE ('FO -%') OR GroupName LIKE ('%Cebu%')) ", " GroupName = " & ReadyString(region) & " ") & vbCrLf &
                "   AND RoleName = 'District Manager' " & vbCrLf &
                "   AND CONVERT(date, [date]) BETWEEN " & ReadyString(dateFrom) & " AND " & ReadyString(dateTo) & " " & vbCrLf &
                " UNION ALL " & vbCrLf &
                "   SELECT DISTINCT GroupName, ManagerID2 as ManagerID, Manager2 as ManagerName, role.RoleName " & vbCrLf &
                "   FROM TransStoreDeviceUtilization as util " & vbCrLf &
                "   LEFT JOIN LibraryOrganizationEmployee as emp ON emp.PeopleID = ManagerID2 " & vbCrLf &
                "   LEFT JOIN LibraryDTRRole as role ON role.ID = emp.RoleTagID " & vbCrLf &
                "   WHERE " & IIf(region = "All", "(GroupName LIKE ('FO -%') OR GroupName LIKE ('%Cebu%')) ", " GroupName = " & ReadyString(region) & " ") & vbCrLf &
                "   AND RoleName = 'District Manager' " & vbCrLf &
                "   AND CONVERT(date, [date]) BETWEEN " & ReadyString(dateFrom) & " AND " & ReadyString(dateTo) & " " & vbCrLf &
                " ) as tbl) as tbl2 Group By GroupName, ManagerID, RoleName " & vbCrLf

        Dim dms As Object '= jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

        'GET ALL DATA FOR THE CHOSEN PERIOD
        pSql = " IF OBJECT_ID('tempdb..##UtilData') IS NOT NULL DROP TABLE ##UtilData " & vbCrLf &
                " SELECT * " & vbCrLf &
                " INTO ##UtilData " & vbCrLf &
                " FROM TransStoreDeviceUtilization " & vbCrLf &
                " WHERE CONVERT(date, [Date]) BETWEEN " & ReadyString(dateFrom) & " AND " & ReadyString(dateTo) & vbCrLf
        Common.WebRequest.ProData.Execute(pSql)

        'GET ALL FRANCHISE REGIONS
        pSql = "SELECT GroupName FROM ##UtilData WHERE GroupName LIKE 'FO -%' OR GroupName = 'Cebu' GROUP BY GroupName ORDER BY GroupName"
        Dim regions As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

        For Each reg As Object In regions
            'GET ALL MANAGERS PER REGION
            'pSql = " IF OBJECT_ID('tempdb..#ManagersPerRegion') IS NOT NULL DROP TABLE #ManagersPerRegion " & vbCrLf &
            '        " SELECT tblManagers.*, role.RoleName INTO #ManagersPerRegion FROM ( " & vbCrLf &
            '        "   SELECT ManagerID1 as ManagerID, Manager1 as ManagerName " & vbCrLf &
            '        "       FROM ##UtilData " & vbCrLf &
            '        "       WHERE GroupName = " & ReadyString(reg("GroupName")) & vbCrLf &
            '        "   UNION ALL" & vbCrLf &
            '        "   SELECT ManagerID2 as ManagerID , Manager2 as ManagerName" & vbCrLf &
            '        "       FROM ##UtilData " & vbCrLf &
            '        "       WHERE GroupName = " & ReadyString(reg("GroupName")) & vbCrLf &
            '        " ) as tblManagers " & vbCrLf &
            '        " LEFT JOIN LibraryOrganizationEmployee as emp ON emp.PeopleID = ManagerID " & vbCrLf &
            '        " LEFT JOIN LibraryDTRRole as role ON role.ID = emp.RoleTagID " & vbCrLf &
            '        " WHERE ManagerID IS NOT NULL GROUP BY ManagerID, ManagerName, role.RoleName " & vbCrLf &
            '        " ORDER BY RoleName DESC " & vbCrLf

            pSql = " IF OBJECT_ID('tempdb..#ManagersPerRegion') IS NOT NULL DROP TABLE #ManagersPerRegion; " & vbCrLf &
                    "   WITH ManagerStores AS ( " & vbCrLf &
                    "       SELECT ManagerID1 AS ManagerID, Manager1 as ManagerName, " & vbCrLf &
                    "           (SELECT DISTINCT COUNT(FOWSTID) " & vbCrLf &
                    "               FROM ##UtilData " & vbCrLf &
                    "               WHERE (ManagerID1 = main.ManagerID1 OR ManagerID2 = main.ManagerID1) " & vbCrLf &
                    "               AND GroupName = " & ReadyString(reg("GroupName")) & ") AS StoresHandle " & vbCrLf &
                    "       FROM ##UtilData AS main " & vbCrLf &
                    "       WHERE GroupName = " & ReadyString(reg("GroupName")) & vbCrLf &
                    "       GROUP BY ManagerID1, Manager1 " & vbCrLf &
                    "   UNION " & vbCrLf &
                    "       SELECT ManagerID2 AS ManagerID, Manager2 as ManagerName, " & vbCrLf &
                    "           (SELECT DISTINCT COUNT(FOWSTID) " & vbCrLf &
                    "               FROM ##UtilData " & vbCrLf &
                    "               WHERE (ManagerID1 = main.ManagerID2 Or ManagerID2 = main.ManagerID2) " & vbCrLf &
                    "               And GroupName = " & ReadyString(reg("GroupName")) & ") AS StoresHandle " & vbCrLf &
                    "       FROM ##UtilData AS main " & vbCrLf &
                    "       WHERE GroupName = " & ReadyString(reg("GroupName")) & vbCrLf &
                    "       GROUP BY ManagerID2, Manager2 " & vbCrLf &
                    "   ), " & vbCrLf &
                    "   Cleaned AS ( " & vbCrLf &
                    "       SELECT DISTINCT ManagerID, ManagerName, StoresHandle " & vbCrLf &
                    "           FROM ManagerStores " & vbCrLf &
                    "           WHERE ManagerID IS NOT NULL " & vbCrLf &
                    "   ), " & vbCrLf &
                    "   MaxStores AS ( " & vbCrLf &
                    "       SELECT MAX(StoresHandle) AS MaxHandle FROM Cleaned " & vbCrLf &
                    "   ) " & vbCrLf &
                    "   SELECT c.ManagerID, c.ManagerName, c.StoresHandle, " & vbCrLf &
                    "       CASE WHEN c.StoresHandle = m.MaxHandle THEN 'District Manager' ELSE 'Area Manager' END AS RoleName " & vbCrLf &
                    "   INTO #ManagersPerRegion " & vbCrLf &
                    "   FROM Cleaned c " & vbCrLf &
                    "   CROSS JOIN MaxStores m " & vbCrLf &
                    "   ORDER BY c.StoresHandle DESC; " & vbCrLf
            Common.WebRequest.ProData.Execute(pSql)

            dms = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute("SELECT * FROM #ManagersPerRegion WHERE RoleName = 'District Manager' ")))

            For Each dm As Object In dms
                reg.Add("ManagerID", dm("ManagerID"))
                reg.Add("ManagerName", dm("ManagerName"))
                reg.Add("RoleName", dm("RoleName"))

                GetManagerData(dm("ManagerID"), reg("GroupName"))
                Dim managerdata As DataTable = GetSummaryPerManager(dm("ManagerID"), reg("GroupName"))

                For Each mdata As Object In managerdata.Rows
                    reg.Add("FullyUtilized", mdata("FullyUtilized"))
                    reg.Add("Utilized", mdata("Utilized"))
                    reg.Add("NotUtilized", mdata("NotUtilized"))
                    reg.Add("NoOfStores", mdata("NoOfStores"))
                Next

                Dim ams As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute("SELECT * FROM #ManagersPerRegion WHERE RoleName = 'Area Manager' ")))

                For Each am As Object In ams
                    GetManagerData(am("ManagerID"), reg("GroupName"))
                    managerdata = GetSummaryPerManager(am("ManagerID"), reg("GroupName"))

                    For Each mdata As Object In managerdata.Rows
                        am.Add("FullyUtilized", mdata("FullyUtilized"))
                        am.Add("Utilized", mdata("Utilized"))
                        am.Add("NotUtilized", mdata("NotUtilized"))
                        am.Add("NoOfStores", mdata("NoOfStores"))
                    Next
                Next

                reg.Add("AM", ams)
            Next
        Next

        Return regions

        'OLD CODE, dugay mahuman, by Arnie 052225
        For Each dm As Object In dms
            GetSummaryReport(dm("ManagerID"), dm("GroupName"), dateFrom, dateTo)

            Dim sum As String = " SELECT ManagerName, " & vbCrLf &
                    "   SUM(CASE WHEN Closed >= 0 AND Utilized > 0 AND NotUtilized = 0 THEN 1 ELSE 0 END) AS FullyUtilized, " & vbCrLf &
                    "   SUM(CASE WHEN Closed >= 0 AND Utilized > 0 AND NotUtilized > 0 THEN 1 ELSE 0 END) AS Utilized, " & vbCrLf &
                    "   SUM(CASE WHEN Closed >= 0 AND Utilized = 0 AND NotUtilized >= 0 THEN 1 ELSE 0 END) AS NotUtilized, " & vbCrLf &
                    "   COUNT(*) as NoOfStores " & vbCrLf &
                    " FROM #UtilizationSummary " & vbCrLf &
                    " GROUP BY ManagerName " & vbCrLf

            Dim util As DataRow = Common.WebRequest.ProData.ExecuteRow(sum)

            dm.Add("FullyUtilized", util("FullyUtilized"))
            dm.Add("Utilized", util("Utilized"))
            dm.Add("NotUtilized", util("NotUtilized"))
            dm.Add("NoOfStores", util("NoOfStores"))

            pSql = " SELECT GroupName, ManagerID, MIN(ManagerName) AS ManagerName, RoleName FROM (SELECT DISTINCT * FROM ( " & vbCrLf &
                "   SELECT DISTINCT GroupName, ManagerID1 as ManagerID, Manager1 as ManagerName, role.RoleName " & vbCrLf &
                "   FROM TransStoreDeviceUtilization as util " & vbCrLf &
                "   LEFT JOIN LibraryOrganizationEmployee as emp ON emp.PeopleID = ManagerID1 " & vbCrLf &
                "   LEFT JOIN LibraryDTRRole as role ON role.ID = emp.RoleTagID " & vbCrLf &
                "   WHERE " & IIf(dm("GroupName") = "All", "(GroupName LIKE ('FO -%') OR GroupName LIKE ('%Cebu%')) ", " GroupName = " & ReadyString(dm("GroupName")) & " ") & vbCrLf &
                "   AND RoleName = 'Area Manager' AND ManagerID2 = " & ReadyString(dm("ManagerID")) & vbCrLf &
                "   AND CONVERT(date, [date]) BETWEEN " & ReadyString(dateFrom) & " AND " & ReadyString(dateTo) & " " & vbCrLf &
                " UNION ALL " & vbCrLf &
                "   SELECT DISTINCT GroupName, ManagerID2 as ManagerID, Manager2 as ManagerName, role.RoleName " & vbCrLf &
                "   FROM TransStoreDeviceUtilization as util " & vbCrLf &
                "   LEFT JOIN LibraryOrganizationEmployee as emp ON emp.PeopleID = ManagerID2 " & vbCrLf &
                "   LEFT JOIN LibraryDTRRole as role ON role.ID = emp.RoleTagID " & vbCrLf &
                "   WHERE " & IIf(dm("GroupName") = "All", "(GroupName LIKE ('FO -%') OR GroupName LIKE ('%Cebu%')) ", " GroupName = " & ReadyString(dm("GroupName")) & " ") & vbCrLf &
                "   AND RoleName = 'Area Manager' AND ManagerID1 = " & ReadyString(dm("ManagerID")) & vbCrLf &
                "   AND CONVERT(date, [date]) BETWEEN " & ReadyString(dateFrom) & " AND " & ReadyString(dateTo) & " " & vbCrLf &
                " ) as tbl) as tbl2 Group By GroupName, ManagerID, RoleName " & vbCrLf

            Dim ams As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

            For Each am As Object In ams
                GetSummaryReport(am("ManagerID"), am("GroupName"), dateFrom, dateTo)

                util = Common.WebRequest.ProData.ExecuteRow(sum)

                am.Add("FullyUtilized", util("FullyUtilized"))
                am.Add("Utilized", util("Utilized"))
                am.Add("NotUtilized", util("NotUtilized"))
                am.Add("NoOfStores", util("NoOfStores"))
            Next

            dm.Add("AM", ams)
        Next

        Return dms
    End Function

    Function GetManagerData(ByVal managerid As String, ByVal group As String)
        Dim pSql As String

        pSql = " IF OBJECT_ID('tempdb..#ManagerData') IS NOT NULL DROP TABLE #ManagerData " & vbCrLf &
                " SELECT * " & vbCrLf &
                " INTO #ManagerData " & vbCrLf &
                " FROM ##UtilData " & vbCrLf &
                " WHERE ManagerID1 = " & ReadyString(managerid) & " OR ManagerID2 = " & ReadyString(managerid) & vbCrLf &
                " AND GroupName = " & ReadyString(group)
        Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetSummaryPerManager(ByVal managerid As String, ByVal group As String)
        Dim pSql As String

        pSql = " SELECT ManagerName, " & vbCrLf &
                " SUM(CASE WHEN Closed >= 0 AND Utilized > 0 AND NotUtilized = 0 THEN 1 ELSE 0 END) AS FullyUtilized, " & vbCrLf &
                " SUM(CASE WHEN Closed >= 0 AND Utilized > 0 AND NotUtilized > 0 THEN 1 ELSE 0 END) AS Utilized, " & vbCrLf &
                " SUM(CASE WHEN Closed >= 0 AND Utilized = 0 AND NotUtilized >= 0 THEN 1 ELSE 0 END) AS NotUtilized, " & vbCrLf &
                " COUNT(*) as NoOfStores " & vbCrLf &
                " FROM ( " & vbCrLf &
                "   SELECT FOWSTID, FOWSTName, FirstName + ' ' + Surname as ManagerName, " & vbCrLf &
                "       (SELECT COUNT(ID) FROM #ManagerData WHERE FOWSTID = orig.FOWSTID AND Status = 'Closed') as Closed, " & vbCrLf &
                "       (SELECT COUNT(ID) FROM #ManagerData WHERE FOWSTID = orig.FOWSTID AND Status = 'Utilized') as Utilized, " & vbCrLf &
                "       (SELECT COUNT(ID) FROM #ManagerData WHERE FOWSTID = orig.FOWSTID AND Status = 'Not Utilized') as NotUtilized " & vbCrLf &
                "   FROM #ManagerData as orig " & vbCrLf &
                "   INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = " & ReadyString(managerid) & vbCrLf &
                "   WHERE GroupName = " & ReadyString(group) & vbCrLf &
                "   Group By FOWSTID, FOWSTName, FirstName, Surname " & vbCrLf &
                " ) as tbl " & vbCrLf &
                " GROUP BY ManagerName " & vbCrLf
        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetSummaryReport(ByVal id As String, ByVal region As String, ByVal datefrom As String, ByVal dateto As String)
        Dim pSql As String

        pSql = " IF OBJECT_ID('tempdb..#UtilizationSummary') IS NOT NULL DROP TABLE #UtilizationSummary " & vbCrLf &
                " CREATE TABLE #UtilizationSummary ( " & vbCrLf &
                "   FOWSTID UNIQUEIDENTIFIER, " & vbCrLf &
                "   FOWSTNAME NVARCHAR(255), " & vbCrLf &
                "   ManagerName NVARCHAR(255), " & vbCrLf &
                "   Closed INT, " & vbCrLf &
                "   Utilized INT, " & vbCrLf &
                "   NotUtilized INT) " & vbCrLf &
                " DECLARE @util NVARCHAR(MAX) " & vbCrLf &
                " SET @util = 'INSERT INTO #UtilizationSummary " & vbCrLf &
                "   SELECT FOWSTID, FOWSTNAME, MAX(ManagerName) AS ManagerName, MAX(Closed), MAX(Utilized), MAX(NotUtilized) FROM (SELECT DISTINCT util.FOWSTID, util.FOWSTNAME, Manager2 as ManagerName, " & vbCrLf &
                "   (SELECT COUNT(ID) FROM TransStoreDeviceUtilization WHERE FOWSTID = util.FOWSTID AND ManagerID2 = util.ManagerID2 AND Status = ''Closed'' AND CONVERT(date, [Date]) BETWEEN '" & ReadyString(datefrom) & "' AND '" & ReadyString(dateto) & "') as Closed, " & vbCrLf &
                "   (SELECT COUNT(ID) FROM TransStoreDeviceUtilization WHERE FOWSTID = util.FOWSTID AND ManagerID2 = util.ManagerID2 AND Status = ''Utilized'' AND CONVERT(date, [Date]) BETWEEN '" & ReadyString(datefrom) & "' AND '" & ReadyString(dateto) & "') as Utilized, " & vbCrLf &
                "   (SELECT COUNT(ID) FROM TransStoreDeviceUtilization WHERE FOWSTID = util.FOWSTID AND ManagerID2 = util.ManagerID2 AND Status = ''Not Utilized'' AND CONVERT(date, [Date]) BETWEEN '" & ReadyString(datefrom) & "' AND '" & ReadyString(dateto) & "') as NotUtilized " & vbCrLf &
                "   FROM TransStoreDeviceUtilization as util " & vbCrLf &
                "   WHERE ManagerID2 = '" & ReadyString(id) & "' " & vbCrLf &
                "   AND " & IIf(region = "All", "(GroupName LIKE (''FO -%'') OR GroupName LIKE (''%Cebu%'')) ", " GroupName = '" & ReadyString(region) & "' ") & vbCrLf &
                "   AND CONVERT(date, [Date]) BETWEEN '" & ReadyString(datefrom) & "' AND '" & ReadyString(dateto) & "'" & vbCrLf &
                " UNION ALL " & vbCrLf &
                "   SELECT DISTINCT util.FOWSTID, util.FOWSTNAME, Manager1 as ManagerName, " & vbCrLf &
                "   (SELECT COUNT(ID) FROM TransStoreDeviceUtilization WHERE FOWSTID = util.FOWSTID AND ManagerID1 = util.ManagerID1 AND Status = ''Closed'' AND CONVERT(date, [Date]) BETWEEN '" & ReadyString(datefrom) & "' AND '" & ReadyString(dateto) & "') as Closed, " & vbCrLf &
                "   (SELECT COUNT(ID) FROM TransStoreDeviceUtilization WHERE FOWSTID = util.FOWSTID AND ManagerID1 = util.ManagerID1 AND Status = ''Utilized'' AND CONVERT(date, [Date]) BETWEEN '" & ReadyString(datefrom) & "' AND '" & ReadyString(dateto) & "') as Utilized, " & vbCrLf &
                "   (SELECT COUNT(ID) FROM TransStoreDeviceUtilization WHERE FOWSTID = util.FOWSTID AND ManagerID1 = util.ManagerID1 AND Status = ''Not Utilized'' AND CONVERT(date, [Date]) BETWEEN '" & ReadyString(datefrom) & "' AND '" & ReadyString(dateto) & "') as NotUtilized " & vbCrLf &
                "   FROM TransStoreDeviceUtilization as util " & vbCrLf &
                "   WHERE ManagerID1 = '" & ReadyString(id) & "' " & vbCrLf &
                "   AND " & IIf(region = "All", "(GroupName LIKE (''FO -%'') OR GroupName LIKE (''%Cebu%'')) ", " GroupName = '" & ReadyString(region) & "' ") & vbCrLf &
                "   AND CONVERT(date, [Date]) BETWEEN '" & ReadyString(datefrom) & "' AND '" & ReadyString(dateto) & "'" & vbCrLf &
                "   ) as tbl2 Group By FOWSTID, FOWSTName' " & vbCrLf &
                " EXEC sp_executesql @util;" & vbCrLf

        Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function DownloadFranchiseReport()
        Try
            Dim dms As Object = GetUtilSummaryPerRegion()

            Dim pWB As Aspose.Cells.Workbook
            Dim worksheetSummary As Aspose.Cells.Worksheet
            Dim worksheetDetails As Aspose.Cells.Worksheet

            pWB = New Aspose.Cells.Workbook
            worksheetSummary = pWB.Worksheets(0)
            worksheetSummary.Name = "Summary"

            Dim header As New List(Of String)(New String() {"Region", "Name", "Fully Utilized", "Utilized", "Not Utilized", "Percentage"})
            Dim dataObj As New List(Of String)(New String() {"GroupName", "ManagerName", "FullyUtilized", "Utilized", "NotUtilized", "Percentage"})
            Dim headerCtr As Integer = 0
            Dim rowCtr As Integer = 0

            For Each head As String In header
                worksheetSummary.Cells(rowCtr, headerCtr).PutValue(head)
                worksheetSummary.Cells(rowCtr, headerCtr).Style.ForegroundColor = Color.FromArgb(242, 187, 70)
                worksheetSummary.Cells(rowCtr, headerCtr).Style.Pattern = BackgroundType.Solid
                worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Size = 14
                worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.IsBold = True
                worksheetSummary.Cells(rowCtr, headerCtr).Style.HorizontalAlignment = TextAlignmentType.Center
                worksheetSummary.Cells(rowCtr, headerCtr).Style.VerticalAlignment = TextAlignmentType.Center
                headerCtr += 1
            Next

            rowCtr += 1
            headerCtr = 0

            For Each dm As Object In dms
                For Each obj As String In dataObj
                    If obj = "Percentage" Then
                        worksheetSummary.Cells(rowCtr, headerCtr).PutValue(Format((dm("FullyUtilized") / dm("NoOfStores")) * 100, "0.00"))
                    Else
                        worksheetSummary.Cells(rowCtr, headerCtr).PutValue(dm(obj))
                    End If

                    worksheetSummary.Cells(rowCtr, headerCtr).Style.ForegroundColor = Color.FromArgb(89, 168, 75)
                    worksheetSummary.Cells(rowCtr, headerCtr).Style.Pattern = BackgroundType.Solid
                    worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Color = Color.FromArgb(0, 0, 0)
                    worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Size = 14
                    worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.IsBold = True
                    worksheetSummary.Cells(rowCtr, headerCtr).Style.Borders.SetColor(Color.FromArgb(0, 0, 0))
                    worksheetSummary.Cells.SetRowHeight(rowCtr, 400)

                    If obj <> "GroupName" And obj <> "ManagerName" Then
                        worksheetSummary.Cells(rowCtr, headerCtr).Style.HorizontalAlignment = TextAlignmentType.Center
                        worksheetSummary.Cells(rowCtr, headerCtr).Style.VerticalAlignment = TextAlignmentType.Center
                    End If

                    headerCtr += 1
                Next

                rowCtr += 1
                headerCtr = 0

                For Each am As Object In dm("AM")
                    For Each obj As String In dataObj
                        If obj = "Percentage" Then
                            worksheetSummary.Cells(rowCtr, headerCtr).PutValue(Format((am("FullyUtilized") / am("NoOfStores")) * 100, "0.00"))
                        ElseIf obj <> "GroupName" Then
                            worksheetSummary.Cells(rowCtr, headerCtr).PutValue(am(obj))
                        End If

                        worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Color = Color.FromArgb(0, 0, 0)
                        worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Size = 12
                        worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.IsBold = False

                        If obj <> "GroupName" And obj <> "ManagerName" Then
                            worksheetSummary.Cells(rowCtr, headerCtr).Style.HorizontalAlignment = TextAlignmentType.Center
                            worksheetSummary.Cells(rowCtr, headerCtr).Style.VerticalAlignment = TextAlignmentType.Center
                        End If

                        headerCtr += 1
                    Next

                    rowCtr += 1
                    headerCtr = 0
                Next
            Next

            worksheetDetails = pWB.Worksheets(pWB.Worksheets.Add())
            worksheetDetails.Name = "Details"

            rowCtr = 0
            header = New List(Of String)(New String() {"District Manager", "Area Manager", "Store Name", "Store Code", "Region"})
            dataObj = New List(Of String)(New String() {"Manager Name", "Store Name", "Store Code", "Region"})
            Dim utilDate As New List(Of String)

            Dim dateFrom As Date = CDate(Common.WebRequest.Data("DateFrom"))
            Dim dateTo As Date = CDate(Common.WebRequest.Data("DateTo"))

            Do While dateFrom <= dateTo
                header.Add(Format(dateFrom, "MMM dd"))
                dataObj.Add(Format(dateFrom, "yyyy-MM-dd"))
                dateFrom = dateFrom.AddDays(1)
            Loop

            For Each head As String In header
                worksheetDetails.Cells(rowCtr, headerCtr).PutValue(head)
                worksheetDetails.Cells(rowCtr, headerCtr).Style.ForegroundColor = Color.FromArgb(42, 145, 216)
                worksheetDetails.Cells(rowCtr, headerCtr).Style.Pattern = BackgroundType.Solid
                worksheetDetails.Cells(rowCtr, headerCtr).Style.Font.Size = 14
                worksheetDetails.Cells(rowCtr, headerCtr).Style.Font.IsBold = True
                worksheetDetails.Cells(rowCtr, headerCtr).Style.HorizontalAlignment = TextAlignmentType.Center
                worksheetDetails.Cells(rowCtr, headerCtr).Style.VerticalAlignment = TextAlignmentType.Center
                headerCtr += 1
            Next

            rowCtr += 1

            For Each dm As Object In dms
                headerCtr = 0
                Dim ams As DataTable = GetDetailedDirectManagerReport(dm("ManagerID"), dm("GroupName"))

                If ams.Rows.Count > 0 Then
                    For Each am As Object In ams.Rows
                        worksheetDetails.Cells(rowCtr, headerCtr).PutValue(dm("ManagerName"))
                        worksheetDetails.Cells(rowCtr, headerCtr).Style.Font.Color = Color.FromArgb(0, 0, 0)
                        worksheetDetails.Cells(rowCtr, headerCtr).Style.Font.Size = 12
                        worksheetDetails.Cells(rowCtr, headerCtr).Style.Font.IsBold = False
                        headerCtr += 1

                        For Each obj As String In dataObj
                            If IsDate(obj) Then
                                If am.Table.Columns(obj) IsNot Nothing Then
                                    If CStr2(am(obj)) = "Utilized" Then
                                        'Green
                                        worksheetDetails.Cells(rowCtr, headerCtr).Style.ForegroundColor = Color.FromArgb(89, 168, 75)
                                    ElseIf CStr2(am(obj)) = "Not Utilized" Then
                                        'Yellow
                                        worksheetDetails.Cells(rowCtr, headerCtr).Style.ForegroundColor = Color.FromArgb(242, 187, 70)
                                    ElseIf CStr2(am(obj)) = "Closed" Then
                                        'Closed Store - Gray
                                        worksheetDetails.Cells(rowCtr, headerCtr).Style.ForegroundColor = Color.FromArgb(153, 153, 153)
                                    End If
                                Else
                                    'Closed Store - Gray
                                    worksheetDetails.Cells(rowCtr, headerCtr).Style.ForegroundColor = Color.FromArgb(153, 153, 153)
                                End If

                                worksheetDetails.Cells(rowCtr, headerCtr).Style.Pattern = BackgroundType.Solid
                                worksheetDetails.Cells(rowCtr, headerCtr).Style.Font.Size = 12

                                Dim cell As Cell = worksheetDetails.Cells(rowCtr, headerCtr)
                                Dim sty As Style = cell.GetStyle()

                                sty.Borders.SetStyle(BorderStyle.Double)
                                sty.Borders.SetColor(Color.White)
                                cell.SetStyle(sty)
                            Else
                                worksheetDetails.Cells(rowCtr, headerCtr).PutValue(am(obj))
                                worksheetDetails.Cells(rowCtr, headerCtr).Style.Font.Color = Color.FromArgb(0, 0, 0)
                                worksheetDetails.Cells(rowCtr, headerCtr).Style.Font.Size = 12
                                worksheetDetails.Cells(rowCtr, headerCtr).Style.Font.IsBold = False
                            End If
                            headerCtr += 1
                        Next

                        rowCtr += 1
                        headerCtr = 0
                    Next
                End If
            Next

            worksheetSummary.FreezePanes("A2", 1, 0)
            worksheetSummary.AutoFitColumns()
            worksheetSummary.AutoFitRows()

            worksheetDetails.FreezePanes("A2", 1, 0)
            worksheetDetails.AutoFitColumns()
            worksheetDetails.AutoFitRows()

            Dim filename As String = "FranchiseReport" & Format(Now, "MM-dd-yyyyHHmm") & ".xlsx"
            Dim filesource = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID) & "\" & filename

            pWB.Save(filesource)

            Return filename
        Catch ex As Exception
            AppendToDebug(ex.Message)
            Return "Error: " & ex.Message
        End Try
    End Function

    Function DownloadMonthlyUtil(ByVal monthlyutil As Object)
        Try
            Dim pWB As Aspose.Cells.Workbook
            Dim worksheetSummary As Aspose.Cells.Worksheet
            Dim header As New List(Of String)(New String() {"Region", "No. of Stores", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"})
            Dim headerCtr As Integer = 0
            Dim rowCtr As Integer = 0
            Dim startDate As Date
            Dim endDate As Date
            Dim period As String = Common.WebRequest.Data("Period")

            pWB = New Aspose.Cells.Workbook
            worksheetSummary = pWB.Worksheets(0)
            worksheetSummary.Name = "Average"

            If period = "Custom" Then
                header = New List(Of String)(New String() {"Region", "No. of Stores"})
                startDate = CDate(Common.WebRequest.Data("DateFrom"))
                endDate = CDate(Common.WebRequest.Data("DateTo"))

                Do While startDate <= endDate
                    header.Add(Format(startDate, "MMM d"))
                    startDate = startDate.AddDays(1)
                Loop
            ElseIf period = "Yearly" Then
                header = New List(Of String)(New String() {"Region", "No. of Stores"})

                For i As Integer = 2025 To Now().Year()
                    header.Add(i)
                Next
            End If

            For Each head As String In header
                worksheetSummary.Cells(rowCtr, headerCtr).PutValue(head)
                worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Color = Color.FromArgb(255, 255, 255)
                worksheetSummary.Cells(rowCtr, headerCtr).Style.ForegroundColor = Color.FromArgb(37, 70, 100)
                worksheetSummary.Cells(rowCtr, headerCtr).Style.Pattern = BackgroundType.Solid
                worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Size = 14
                worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.IsBold = True
                worksheetSummary.Cells(rowCtr, headerCtr).Style.HorizontalAlignment = TextAlignmentType.Center
                worksheetSummary.Cells(rowCtr, headerCtr).Style.VerticalAlignment = TextAlignmentType.Center
                headerCtr += 1
            Next

            rowCtr += 1

            For Each util As Object In monthlyutil
                headerCtr = 0

                worksheetSummary.Cells(rowCtr, headerCtr).PutValue(util("GroupName"))
                worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Color = Color.FromArgb(0, 0, 0)
                worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Size = 12
                worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.IsBold = False

                headerCtr += 1

                worksheetSummary.Cells(rowCtr, headerCtr).PutValue(util("NoOfStores"))
                worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Color = Color.FromArgb(0, 0, 0)
                worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Size = 12
                worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.IsBold = False
                worksheetSummary.Cells(rowCtr, headerCtr).Style.HorizontalAlignment = TextAlignmentType.Center
                worksheetSummary.Cells(rowCtr, headerCtr).Style.VerticalAlignment = TextAlignmentType.Center

                headerCtr += 1

                If period = "Custom" Then
                    startDate = CDate(Common.WebRequest.Data("DateFrom"))
                    Do While startDate <= endDate
                        CellData(util(Format(startDate, "MM-dd-yyyy")), worksheetSummary, rowCtr, headerCtr)
                        headerCtr += 1
                        startDate = startDate.AddDays(1)
                    Loop
                ElseIf period = "Yearly" Then
                    For i As Integer = 2025 To Now().Year()
                        CellData(util(i), worksheetSummary, rowCtr, headerCtr)
                        headerCtr += 1
                    Next
                ElseIf period = "Monthly" Then
                    For i As Integer = 1 To 12
                        CellData(util(i), worksheetSummary, rowCtr, headerCtr)
                        headerCtr += 1
                    Next
                End If

                rowCtr += 1
            Next

            worksheetSummary.AutoFitColumns()
            worksheetSummary.AutoFitRows()

            Dim filename As String = "MPOSUtilizationReport_"

            If period = "Monthly" Then
                filename += Common.WebRequest.Data("Year") & ".xlsx"
            ElseIf period = "Custom" Then
                filename += Format(startDate, "MM-dd-yyyy") & "_to_" & Format(endDate, "MM-dd-yyyy") & ".xlsx"
            ElseIf period = "Yearly" Then
                filename += "Yearly.xlsx"
            End If

            Dim filesource = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID) & "\" & filename

            pWB.Save(filesource)

            Return filename
        Catch ex As Exception
            AppendToDebug(ex.Message)
            Return "Error: " & ex.Message
        End Try
    End Function

    Function CellData(ByVal avg As Decimal, ByVal worksheetSummary As Aspose.Cells.Worksheet, ByVal rowCtr As Integer, ByVal headerCtr As Integer)
        worksheetSummary.Cells(rowCtr, headerCtr).PutValue(Format(avg, "#.##"))
        worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Size = 12
        worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.IsBold = False
        worksheetSummary.Cells(rowCtr, headerCtr).Style.HorizontalAlignment = TextAlignmentType.Center
        worksheetSummary.Cells(rowCtr, headerCtr).Style.VerticalAlignment = TextAlignmentType.Center

        If CDec2(avg) >= 45 Then
            worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Color = Color.FromArgb(31, 120, 106)
        Else
            worksheetSummary.Cells(rowCtr, headerCtr).Style.Font.Color = Color.FromArgb(223, 89, 76)
        End If

        headerCtr += 1
    End Function

    Function GetMonthlyUtil()
        Dim period As String = Common.WebRequest.Data("Period")
        Dim displayby As String = Common.WebRequest.Data("DisplayBy")
        Dim pSql As String
        Dim groups As Object
        'Dim month() As Integer = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}

        If displayby = "By Group" Then
            pSql = "SELECT ID, GroupName, " & vbCrLf &
                " (SELECT DISTINCT COUNT(FranchiseeOrgFowstID) FROM MobizLibraryOrganizationFranchisee INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID WHERE FranchiseGroupID = LibraryFranchiseGroup.ID AND (ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Store and device are operational.')) as NoOfStores" & vbCrLf &
                " FROM LibraryFranchiseGroup WHERE GroupName IN (" & vbCrLf &
                " 'CO - GMAN', 'CO - GMAS', 'CO - North Luzon', 'CO - South Luzon', 'CO - Visayas', 'CO - Mindanao', " & vbCrLf &
                " 'Cebu', 'FO - GMAN', 'FO - GMAS', 'FO - North Luzon', 'FO - South Luzon', 'FO - Visayas', 'FO - Mindanao') " & vbCrLf &
                " ORDER BY CASE WHEN GroupName LIKE 'CO -%' THEN 1 " & vbCrLf &
                " WHEN GroupName = 'Cebu' THEN 2 " & vbCrLf &
                " WHEN GroupName LIKE 'FO -%' THEN 3 " & vbCrLf &
                " END, GroupName " & vbCrLf
        Else
            pSql = "SELECT 'CO' as GroupName, " & vbCrLf &
                    " (SELECT DISTINCT COUNT(FranchiseeOrgFowstID) FROM MobizLibraryOrganizationFranchisee INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID WHERE FranchiseGroupID IN (SELECT ID FROM LibraryFranchiseGroup WHERE GroupName LIKE 'CO -%') AND (ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Store and device are operational.')) as NoOfStores" & vbCrLf &
                    "UNION ALL SELECT 'FO' as GroupName, " & vbCrLf &
                    " (SELECT DISTINCT COUNT(FranchiseeOrgFowstID) FROM MobizLibraryOrganizationFranchisee INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID WHERE FranchiseGroupID IN (SELECT ID FROM LibraryFranchiseGroup WHERE GroupName LIKE 'FO -%' OR GroupName = 'Cebu') AND (ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Store and device are operational.')) as NoOfStores" & vbCrLf
        End If

        groups = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

        If period = "Custom" Then
            pSql = " IF OBJECT_ID('tempdb..#DeviceUtilData') IS NOT NULL DROP TABLE #DeviceUtilData " & vbCrLf &
                    " SELECT * " & vbCrLf &
                    " INTO #DeviceUtilData " & vbCrLf &
                    " FROM TransStoreDeviceUtilization " & vbCrLf &
                    " WHERE CONVERT(date, [Date]) BETWEEN " & ReadyString(Common.WebRequest.Data("DateFrom")) & " AND " & ReadyString(Common.WebRequest.Data("DateTo"))

            Common.WebRequest.ProData.Execute(pSql)

            For Each region As Object In groups
                GetUtilData(region("GroupName"), period, displayby)

                Dim startDate As Date = CDate(Common.WebRequest.Data("DateFrom"))
                Dim endDate As Date = CDate(Common.WebRequest.Data("DateTo"))

                Do While startDate <= endDate
                    pSql = " SELECT ISNULL(Average, 0) FROM #DeviceUtilDaily WHERE CONVERT(date, [Date]) = " & ReadyString(Format(startDate, "MM-dd-yyyy"))

                    region.add(Format(startDate, "MM-dd-yyyy"), Common.WebRequest.ProData.ExecuteScalar(pSql))
                    startDate = startDate.AddDays(1)
                Loop
            Next

        ElseIf period = "Monthly" Then
            For month As Integer = 1 To 12
                pSql = " IF OBJECT_ID('tempdb..#DeviceUtilData') IS NOT NULL DROP TABLE #DeviceUtilData " & vbCrLf &
                            " SELECT * " & vbCrLf &
                            " INTO #DeviceUtilData " & vbCrLf &
                            " FROM TransStoreDeviceUtilization " & vbCrLf &
                            " WHERE MONTH([Date]) = " & month & " AND YEAR([Date]) = " & Common.WebRequest.Data("Year")

                Common.WebRequest.ProData.Execute(pSql)

                For Each region As Object In groups
                    GetUtilData(region("GroupName"), period, displayby)

                    pSql = " SELECT ISNULL(AVG(Average), 0) as Average FROM #DeviceUtilDaily"

                    region.add(month, Common.WebRequest.ProData.ExecuteScalar(pSql))
                Next
            Next
        ElseIf period = "Yearly" Then
            pSql = " IF OBJECT_ID('tempdb..#DeviceUtilData') IS NOT NULL DROP TABLE #DeviceUtilData " & vbCrLf &
                    " SELECT * " & vbCrLf &
                    " INTO #DeviceUtilData " & vbCrLf &
                    " FROM TransStoreDeviceUtilization " & vbCrLf

            Common.WebRequest.ProData.Execute(pSql)

            For Each region As Object In groups
                GetUtilData(region("GroupName"), period, displayby)

                pSql = " IF OBJECT_ID('tempdb..#DeviceUtilMonthly') IS NOT NULL DROP TABLE #DeviceUtilMonthly " & vbCrLf &
                        " SELECT " & vbCrLf &
                        " YEAR([Date]) as [Year], " & vbCrLf &
                        " MONTH([Date]) as [Date], " & vbCrLf &
                        " ISNULL(AVG(Average), 0) as [Average] " & vbCrLf &
                        " INTO #DeviceUtilMonthly " & vbCrLf &
                        " FROM #DeviceUtilDaily " & vbCrLf &
                        " GROUP BY YEAR([Date]), MONTH([Date]) " & vbCrLf &
                        " ORDER BY YEAR([Date]), MONTH([Date]) " & vbCrLf

                Common.WebRequest.ProData.Execute(pSql)

                For i As Integer = 2025 To Now().Year()
                    pSql = " SELECT ISNULL(AVG(Average), 0) as Average FROM #DeviceUtilMonthly WHERE [Year] = " & i & " GROUP BY [Year] ORDER BY [Year] "

                    region.add(i, Common.WebRequest.ProData.ExecuteScalar(pSql))
                Next
            Next
        End If

        Return groups
    End Function

    Function GetUtilData(ByVal region As String, ByVal period As String, ByVal displayby As String)
        Dim pSql As String = " IF OBJECT_ID('tempdb..#DeviceUtilDaily') IS NOT NULL DROP TABLE #DeviceUtilDaily " & vbCrLf &
                        " SELECT " & vbCrLf &
                        "   CAST([Date] AS DATE) as Date, " & vbCrLf &
                        "   COUNT(CASE WHEN Status = 'Utilized' THEN 1 END) as Utilized, " & vbCrLf &
                        "   COUNT(CASE WHEN Status = 'Not Utilized' THEN 1 END) as NotUtilized, " & vbCrLf &
                        "   COUNT(CASE WHEN Status = 'Closed' THEN 1 END) as Closed, " & vbCrLf &
                        "   COUNT(CASE WHEN Status IN ('Utilized', 'Not Utilized') THEN 1 END) as OpenStores, " & vbCrLf &
                        "   (CAST(COUNT(CASE WHEN Status = 'Utilized' THEN 1 END) as FLOAT) / CAST(COUNT(CASE WHEN Status IN ('Utilized', 'Not Utilized') THEN 1 END) as FLOAT)) * 100 as Average " & vbCrLf &
                        " INTO #DeviceUtilDaily " & vbCrLf &
                        " FROM #DeviceUtilData " & vbCrLf

        If displayby = "By Group" Then
            pSql += " WHERE GroupName = " & ReadyString(region) & vbCrLf
        Else
            pSql += " WHERE (GroupName LIKE '" & region & "%'" & vbCrLf

            If region = "FO" Then
                pSql += "OR GroupName = 'Cebu'"
            End If

            pSql += ")"
        End If

        pSql += " GROUP BY CAST([Date] AS DATE) " & vbCrLf &
                " ORDER BY CAST([Date] AS DATE) " & vbCrLf

        Common.WebRequest.ProData.Execute(pSql)
    End Function

    '================================ MPOS Sales Dashboard =======================================
    Function GetOrganizationSummarySales()
        Dim dateFrom As String = Format(CDate(Common.WebRequest.Data("DateFrom")), "MM-dd-yyyy").ToString()
        Dim dateTo As String = Format(CDate(Common.WebRequest.Data("DateTo")), "MM-dd-yyyy").ToString()
        Dim pOut As New JsonDictionary
        Dim pSql As String

        'pSql = " SELECT " & vbCrLf &
        '        " OwnershipType, " & vbCrLf &
        '        " --SUM(GrossSales) as GrossSales, " & vbCrLf &
        '        " --COUNT(CASE WHEN Status = 'Utilized' THEN 1 END) as Utilized, " & vbCrLf &
        '        " --COUNT(CASE WHEN Status = 'Not Utilized' THEN 1 END) as NotUtilized, " & vbCrLf &
        '        " COUNT(CASE WHEN GrossSales > 0 THEN 1 END) as Completed, " & vbCrLf &
        '        " COUNT(CASE WHEN GrossSales = 0 THEN 1 END) as NotSending, " & vbCrLf &
        '        " COUNT(CASE WHEN Status = 'Closed' THEN 1 END) as Exempted, " & vbCrLf &
        '        " (CAST(COUNT(CASE WHEN GrossSales > 0 THEN 1 END) as FLOAT) + CAST(COUNT(CASE WHEN GrossSales = 0 THEN 1 END) as FLOAT)) as [Open], " & vbCrLf &
        '        " CEILING((CAST(COUNT(CASE WHEN GrossSales > 0 THEN 1 END) as FLOAT) / (CAST(COUNT(CASE WHEN GrossSales > 0 THEN 1 END) as FLOAT) + CAST(COUNT(CASE WHEN GrossSales = 0 THEN 1 END) as FLOAT)) * 100)) as [Percentage] " & vbCrLf &
        '        " FROM TransStoreDeviceUtilization " & vbCrLf &
        '        " WHERE CONVERT(date, [Date]) BETWEEN " & ReadyString(dateFrom) & " AND " & ReadyString(dateTo) & vbCrLf &
        '        " GROUP BY OwnershipType " & vbCrLf

        pSql = " DECLARE @Today DATE = " & ReadyString(dateFrom) & "; " &
                " DECLARE @Tomorrow DATE = DATEADD(DAY, 1, @Today); " & vbCrLf &
                " IF OBJECT_ID('tempdb..#UtilSalesDashboardData') IS NOT NULL DROP TABLE #UtilSalesDashboardData " & vbCrLf &
                " SELECT * INTO #UtilSalesDashboardData " & vbCrLf &
                " FROM TransStoreDeviceUtilization " & vbCrLf &
                " WHERE CONVERT(DATE, [Date]) >= @Today; " & vbCrLf &
                "" & vbCrLf &
                " WITH SummaryCounts AS ( " & vbCrLf &
                "   SELECT OwnershipType, GroupName, " & vbCrLf &
                "       (SELECT COUNT(DISTINCT tom.FOWSTID) " & vbCrLf &
                "           FROM #UtilSalesDashboardData AS tod " & vbCrLf &
                "           INNER JOIN #UtilSalesDashboardData AS tom ON tom.FOWSTID = tod.FOWSTID" & vbCrLf &
                "           WHERE CONVERT(DATE, tom.[Date]) = @Tomorrow AND tom.GrossSales > 0 " & vbCrLf &
                "           AND CONVERT(DATE, tod.[Date]) = @Today AND tod.GrossSales > 0 " & vbCrLf &
                "           AND tod.OwnershipType = main.OwnershipType " & vbCrLf &
                "           AND tod.GroupName = main.GroupName " & vbCrLf &
                "       ) AS Completed, " & vbCrLf &
                "       (SELECT COUNT(DISTINCT tom.FOWSTID) " & vbCrLf &
                "           FROM #UtilSalesDashboardData AS tom " & vbCrLf &
                "           WHERE CONVERT(DATE, tom.[Date]) = @Today AND tom.GrossSales > 0 " & vbCrLf &
                "           AND NOT EXISTS (SELECT 1 " & vbCrLf &
                "               FROM #UtilSalesDashboardData AS tod " & vbCrLf &
                "               WHERE tod.FOWSTID = tom.FOWSTID " & vbCrLf &
                "               AND CONVERT(DATE, tod.[Date]) > @Today AND tod.GrossSales > 0 " & vbCrLf &
                "               AND OwnershipType = main.OwnershipType " & vbCrLf &
                "               AND GroupName = main.GroupName " & vbCrLf &
                "           ) AND OwnershipType = main.OwnershipType " & vbCrLf &
                "           AND GroupName = main.GroupName " & vbCrLf &
                "       ) AS NotSending, " & vbCrLf &
                "       (SELECT COUNT(DISTINCT FOWSTID) " & vbCrLf &
                "           FROM #UtilSalesDashboardData " & vbCrLf &
                "           WHERE Status = 'Closed' " & vbCrLf &
                "           AND CONVERT(DATE, [Date]) = @Today " & vbCrLf &
                "           AND OwnershipType = main.OwnershipType " & vbCrLf &
                "           AND GroupName = main.GroupName " & vbCrLf &
                "       ) AS Exempted " & vbCrLf &
                "   FROM #UtilSalesDashboardData as main " & vbCrLf &
                "   GROUP BY OwnershipType, GroupName " & vbCrLf &
                " ) " & vbCrLf &
                "" & vbCrLf &
                " SELECT OwnershipType, CASE WHEN GroupName LIKE 'CO -%' THEN 'CO' WHEN GroupName LIKE 'FO -%' OR GroupName = 'Cebu' THEN 'FO' WHEN GroupName LIKE '%Third Party POS%' THEN 'Third Party POS' END AS GroupCategory," & vbCrLf &
                " SUM(Completed) as Completed, SUM(NotSending) as NotSending, SUM(Exempted) as Exempted, SUM(Completed) + SUM(NotSending) as [Open], " & vbCrLf &
                "   CAST((CASE WHEN (SUM(Completed) + SUM(NotSending)) = 0 THEN 0 ELSE CAST(SUM(Completed) AS FLOAT) / (SUM(Completed) + SUM(NotSending)) END) AS FLOAT) * 100 AS Percentage " & vbCrLf &
                " FROM SummaryCounts " & vbCrLf &
                " GROUP BY OwnershipType, CASE WHEN GroupName LIKE 'CO -%' THEN 'CO' WHEN GroupName LIKE 'FO -%' OR GroupName = 'Cebu' THEN 'FO' WHEN GroupName LIKE '%Third Party POS%' THEN 'Third Party POS' END " & vbCrLf &
                " UNION ALL " & vbCrLf &
                " SELECT OwnershipType, 'Overall' as GroupCategory, SUM(Completed) as Completed, SUM(NotSending) as NotSending, SUM(Exempted) as Exempted, SUM(Completed) + SUM(NotSending) as [Open], " & vbCrLf &
                "   CAST((CASE WHEN (SUM(Completed) + SUM(NotSending)) = 0 THEN 0 ELSE CAST(SUM(Completed) AS FLOAT) / (SUM(Completed) + SUM(NotSending)) END) AS FLOAT) * 100 AS Percentage " & vbCrLf &
                " FROM SummaryCounts " & vbCrLf &
                " GROUP BY OwnershipType " & vbCrLf

        pOut.Add("SummaryPerOwnership", Common.WebRequest.ProData.Execute(pSql))

        'CO - GMAN, CO - GMAS, CO - Mindanao, CO - North Luzon, CO - South Luzon, CO - Visayas
        pSql = " SELECT COUNT(MobizLibraryOrganizationFOWST.ID) as [Count] FROM MobizLibraryOrganizationFOWST " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                    " WHERE (ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Store and device are operational.') " & vbCrLf &
                    " AND MobizLibraryOrganizationFranchisee.FranchiseGroupID IN ('0a31a6e7-f2d9-404c-b5cd-0f970c840b2b', '70312661-460a-42ad-a85b-0c46e2e727c4', '1f41e72c-35cc-4a82-acd9-27760751f499', 'e9db8b68-b1ea-4f38-9c45-8bbdb2334e78', '5c8d6617-1db5-460f-8b61-d5ed693e0f07', '78aeb14d-9b95-47a6-82d3-2eb8f87b76bc') " & vbCrLf

        pOut.Add("CO-OperationalStores", Common.WebRequest.ProData.ExecuteScalar(pSql))

        pSql = " SELECT COUNT(MobizLibraryOrganizationFOWST.ID) as [Count] FROM MobizLibraryOrganizationFOWST " & vbCrLf &
                    " --LEFT JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                    " WHERE (ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Store and device are operational.') " & vbCrLf &
                    " AND MobizLibraryOrganizationFOWST.PosVendorID IS NOT NULL " & vbCrLf &
                    " AND MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf

        pOut.Add("3PP-COStores", Common.WebRequest.ProData.ExecuteScalar(pSql))

        'Cebu, FO - GMAN, FO - GMAS, FO - Mindanao, FO - North Luzon, FO - South Luzon, FO - Visayas
        pSql = " SELECT COUNT(MobizLibraryOrganizationFOWST.ID) as [Count] FROM MobizLibraryOrganizationFOWST " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                    " WHERE (ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Store and device are operational.') " & vbCrLf &
                    " AND MobizLibraryOrganizationFranchisee.FranchiseGroupID IN ('7c3897bd-4501-4211-93cb-0ed2a4749141', '61bab350-115f-4901-ab88-bcbbac7ada73', '3a6e4c4a-e89f-485c-aeb4-35c45143a428', 'b5e9d777-84c6-49e8-b7f9-7b446c7c4915', 'f9c92bf2-d8c0-4fa7-9812-b7e539c3356c', 'd3750fec-31ca-40a8-8b9f-77b0dfed1631', 'db5bd2cc-d4fa-4081-884f-a19d5ec34548')"

        pOut.Add("FO-OperationalStores", Common.WebRequest.ProData.ExecuteScalar(pSql))

        'PC Franchisee DB
        pSql = " SELECT COUNT(MobizLibraryOrganizationFOWST.ID) as [Count] FROM MobizLibraryOrganizationFOWST " & vbCrLf &
                    " --LEFT JOIN MobizLibraryOrganizationFranchisee on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                    " WHERE (ISNULL(MobizLibraryOrganizationFOWST.Status, 'Others') = 'Store and device are operational.') " & vbCrLf &
                    " AND MobizLibraryOrganizationFOWST.PosVendorID IS NOT NULL " & vbCrLf &
                    " AND MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString("892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba") & vbCrLf

        pOut.Add("3PP-FOStores", Common.WebRequest.ProData.ExecuteScalar(pSql))

        pSql = " SELECT " & vbCrLf &
                " OwnershipType, " & vbCrLf &
                " GroupName, " & vbCrLf &
                " SUM(TransactionCount) as TransactionCount, " & vbCrLf &
                " SUM(GrossSales) as GrossSales " & vbCrLf &
                " FROM TransStoreDeviceUtilization " & vbCrLf &
                " WHERE CONVERT(date, [Date]) BETWEEN " & ReadyString(dateFrom) & " AND " & ReadyString(dateTo) & vbCrLf &
                " GROUP BY OwnershipType, GroupName " & vbCrLf &
                " ORDER BY OwnershipType, SUM(GrossSales) DESC " & vbCrLf

        'pOut.Add("SummaryPerRegion", Common.WebRequest.ProData.Execute(pSql))

        Return pOut.Serialize()
    End Function

    Function GetMPOSDashboardDetails()
        Dim dateFrom As String = Format(CDate(Common.WebRequest.Data("DateFrom")), "MM-dd-yyyy").ToString()
        Dim dateTo As String = Format(CDate(dateFrom).AddDays(1), "MM-dd-yyyy").ToString()
        Dim ownership As String = Common.WebRequest.Data("Ownership")
        Dim group As String = Common.WebRequest.Data("Group")
        Dim status As String = Common.WebRequest.Data("Status")
        Dim pSql As String
        Dim pOut As New JsonDictionary

        'pSql = " SELECT " & vbCrLf &
        '        " GroupName, " & vbCrLf &
        '        " FOWSTCode, " & vbCrLf &
        '        " FOWSTName, " & vbCrLf &
        '        " Status, " & vbCrLf &
        '        " GrossSales " & vbCrLf &
        '        " FROM TransStoreDeviceUtilization " & vbCrLf &
        '        " WHERE CONVERT(date, [Date]) BETWEEN " & ReadyString(dateFrom) & " AND " & ReadyString(dateTo) & vbCrLf

        'If group <> "All" Then
        '    pSql += " AND GroupName = " & ReadyString(group) & vbCrLf
        'End If

        'If status <> "All" Then
        '    pSql += " AND Status = " & ReadyString(status) & vbCrLf
        'End If

        'pSql += " ORDER BY CASE WHEN GroupName LIKE 'CO -%' THEN 1 " & vbCrLf &
        '        " WHEN GroupName = 'Cebu' THEN 2 " & vbCrLf &
        '        " WHEN GroupName LIKE 'FO -%' THEN 3 " & vbCrLf &
        '        " WHEN GroupName LIKE '%Third Party POS%' THEN 4 " & vbCrLf &
        '        " END, GroupName, GrossSales DESC " & vbCrLf

        pSql = " IF OBJECT_ID('tempdb..#UtilSalesDashboardData') IS NOT NULL DROP TABLE #UtilSalesDashboardData " & vbCrLf &
                " SELECT * INTO #UtilSalesDashboardData " & vbCrLf &
                " FROM TransStoreDeviceUtilization " & vbCrLf &
                " WHERE CONVERT(DATE, [Date]) >= " & ReadyString(dateFrom) & "; " & vbCrLf

        Common.WebRequest.ProData.Execute(pSql)

        pSql = " SELECT tod.* " & vbCrLf &
                " FROM #UtilSalesDashboardData AS tod " & vbCrLf &
                " INNER JOIN #UtilSalesDashboardData AS tom ON tom.FOWSTID = tod.FOWSTID" & vbCrLf &
                " WHERE CONVERT(DATE, tom.[Date]) = " & ReadyString(dateTo) & " AND tom.GrossSales > 0 " & vbCrLf &
                " AND CONVERT(DATE, tod.[Date]) = " & ReadyString(dateFrom) & " AND tod.GrossSales > 0 " & vbCrLf

        If ownership <> "All" Then
            pSql += " AND tod.OwnershipType = " & ReadyString(ownership)
        End If

        If group <> "All" Then
            pSql += " AND tod.GroupName LIKE " & ReadyStringWild(group) & vbCrLf
        End If

        pSql += " ORDER BY tod.GrossSales DESC "

        pOut.Add("Completed", Common.WebRequest.ProData.Execute(pSql))

        pSql = " SELECT tom.* " & vbCrLf &
                " FROM #UtilSalesDashboardData AS tom " & vbCrLf &
                " WHERE CONVERT(DATE, tom.[Date]) = " & ReadyString(dateFrom) & " AND tom.GrossSales > 0 " & vbCrLf &
                " AND NOT EXISTS (SELECT 1 " & vbCrLf &
                "   FROM #UtilSalesDashboardData AS tod " & vbCrLf &
                "   WHERE tod.FOWSTID = tom.FOWSTID " & vbCrLf

        If ownership <> "All" Then
            pSql += " AND tod.OwnershipType = " & ReadyString(ownership)
        End If

        If group <> "All" Then
            pSql += " AND tod.GroupName LIKE " & ReadyStringWild(group) & vbCrLf
        End If

        pSql += "   AND CONVERT(DATE, tod.[Date]) > " & ReadyString(dateFrom) & " AND tod.GrossSales > 0 " & vbCrLf &
                " ) " & vbCrLf

        If ownership <> "All" Then
            pSql += " AND tom.OwnershipType = " & ReadyString(ownership)
        End If

        If group <> "All" Then
            pSql += " AND tom.GroupName LIKE " & ReadyStringWild(group) & vbCrLf
        End If

        pSql += " ORDER BY tom.GrossSales DESC "

        pOut.Add("NotSending", Common.WebRequest.ProData.Execute(pSql))

        pSql = " SELECT * " & vbCrLf &
                " FROM #UtilSalesDashboardData " & vbCrLf &
                " WHERE Status = 'Closed' " & vbCrLf &
                " AND CONVERT(DATE, [Date]) = " & ReadyString(dateFrom) & vbCrLf

        If ownership <> "All" Then
            pSql += " AND OwnershipType = " & ReadyString(ownership)
        End If

        If group <> "All" Then
            pSql += " AND GroupName LIKE " & ReadyStringWild(group) & vbCrLf
        End If

        pOut.Add("Exempted", Common.WebRequest.ProData.Execute(pSql))

        Return pOut.Serialize()
    End Function

    Function GetMPOSDashboardDetailedReport()
        Dim pSql As String
        Dim today As String = Now.ToString("yyyy-MM-dd")
        Dim posvendor As String = Common.WebRequest.Data("POSVendor")
        Dim pOut As New JsonDictionary

        MPOSDashboardYearData(posvendor)

        MPOSDashboardHeader(CDate(today).AddDays(-1), CDate(today).AddDays(-1))

        pOut.Add("YesterdaySales", MPOSDashboardSales())
        pOut.Add("YesterdaySalesMix", MPOSDashboardSalesMix())
        pOut.Add("YesterdayTC", MPOSDashboardTransactionCount())
        pOut.Add("YesterdayTCMix", MPOSDashboardTransactionCountMix())

        MPOSDashboardHeader(CDate(today).ToString("yyyy-MM-01"), today)

        pOut.Add("MTDSales", MPOSDashboardSales())
        pOut.Add("MTDSalesMix", MPOSDashboardSalesMix())
        pOut.Add("MTDTC", MPOSDashboardTransactionCount())
        pOut.Add("MTDTCMix", MPOSDashboardTransactionCountMix())

        MPOSDashboardHeader(CDate(today).ToString("yyyy-01-01"), today)

        pOut.Add("YTDSales", MPOSDashboardSales())
        pOut.Add("YTDSalesMix", MPOSDashboardSalesMix())
        pOut.Add("YTDTC", MPOSDashboardTransactionCount())
        pOut.Add("YTDTCMix", MPOSDashboardTransactionCountMix())

        Return pOut.Serialize()
    End Function

    Function MPOSDashboardYearData(ByVal posvendor As String)
        Dim pSql As String

        pSql = " IF OBJECT_ID('tempdb..##ThisYearData') IS NOT NULL DROP TABLE ##ThisYearData " & vbCrLf &
                " SELECT * " & vbCrLf &
                " INTO ##ThisYearData " & vbCrLf &
                " FROM CacheInvoicesDaily " & vbCrLf &
                " WHERE YEAR(InvoiceDate) = YEAR(GetDate()) AND ISNULL(FromFranchisee, 0) = 0 AND ISNULL(GrossInvoiceAmount, 0) > 0 " & vbCrLf &
                " AND (FOWSTCode LIKE 'PC%' AND FOWSTCode NOT LIKE 'PCF%') " & vbCrLf

        If posvendor = "All" Then
            pSql += " AND (GroupName LIKE 'CO -%' OR GroupName LIKE '%Third Party POS%'); "
        ElseIf posvendor = "MPOS" Then
            pSql += " AND (GroupName LIKE 'CO -%'); "
        ElseIf posvendor = "3PP" Then
            pSql += " AND (GroupName LIKE '%Third Party POS%'); "
        End If

        pSql += " " & vbCrLf &
                " IF OBJECT_ID('tempdb..##LastYearData') IS NOT NULL DROP TABLE ##LastYearData " & vbCrLf &
                " SELECT * " & vbCrLf &
                " INTO ##LastYearData " & vbCrLf &
                " FROM CacheInvoicesDaily " & vbCrLf &
                " WHERE YEAR(InvoiceDate) = (YEAR(GetDate()) - 1) AND ISNULL(FromFranchisee, 0) = 0 AND ISNULL(GrossInvoiceAmount, 0) > 0" & vbCrLf &
                " AND (FOWSTCode LIKE 'PC%' AND FOWSTCode NOT LIKE 'PCF%') " & vbCrLf

        If posvendor = "All" Then
            pSql += " AND (GroupName LIKE 'CO -%' OR GroupName LIKE '%Third Party POS%'); "
        ElseIf posvendor = "MPOS" Then
            pSql += " AND (GroupName LIKE 'CO -%'); "
        ElseIf posvendor = "3PP" Then
            pSql += " AND (GroupName LIKE '%Third Party POS%'); "
        End If

        Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function MPOSDashboardHeader(ByVal datefrom As Date, ByVal dateTo As Date)
        Dim pSql As String

        pSql = " DECLARE @DateFrom DATE = " & ReadyString(datefrom) & "; " & vbCrLf &
                " DECLARE @DateTo DATE = " & ReadyString(dateTo) & "; " & vbCrLf &
                " DECLARE @LYDateFrom DATE = DATEADD(YEAR, -1, @DateFrom); " & vbCrLf &
                " DECLARE @LYDateTo DATE = DATEADD(YEAR, -1, @DateTo); " & vbCrLf &
                " " & vbCrLf &
                " IF OBJECT_ID('tempdb..##YesterdayData') IS NOT NULL DROP TABLE ##YesterdayData " & vbCrLf &
                " SELECT DISTINCT FowstID, GrossInvoiceAmount, TransactionCount, WalkInSales, DeliverySales, WalkInTransactionCount, DeliveryTransactionCount " & vbCrLf &
                " INTO ##YesterdayData " & vbCrLf &
                " FROM ##ThisYearData " & vbCrLf &
                " WHERE CONVERT(DATE, InvoiceDate) BETWEEN @DateFrom AND @DateTo; " & vbCrLf &
                " " & vbCrLf &
                " IF OBJECT_ID('tempdb..##LastYearDateData') IS NOT NULL DROP TABLE ##LastYearDateData " & vbCrLf &
                " SELECT DISTINCT FowstID, GrossInvoiceAmount, TransactionCount " & vbCrLf &
                " INTO ##LastYearDateData " & vbCrLf &
                " FROM ##LastYearData " & vbCrLf &
                " WHERE CONVERT(DATE, InvoiceDate) BETWEEN @LYDateFrom AND @LYDateTo; " & vbCrLf

        Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function MPOSDashboardSales()
        Dim pSql As String

        pSql = " SELECT ISNULL(SUM(GrossInvoiceAmount), 0) as Actual, " & vbCrLf &
                " ISNULL((SELECT SUM(GrossInvoiceAmount) FROM ##LastYearDateData), 0) as LastYearDate, " & vbCrLf &
                " ISNULL((CAST(SUM(GrossInvoiceAmount) - (SELECT SUM(GrossInvoiceAmount) FROM ##LastYearDateData) as FLOAT) / (SELECT SUM(GrossInvoiceAmount) FROM ##LastYearDateData)) * 100, 0) as Growth, " & vbCrLf &
                " ISNULL((SELECT COUNT(DISTINCT FowstID) FROM ##YesterdayData), 0) as StoreCount, " & vbCrLf &
                " ISNULL(SUM(GrossInvoiceAmount) / (SELECT COUNT(DISTINCT FowstID) FROM ##YesterdayData), 0) as AvgDaily " & vbCrLf &
                " FROM ##YesterdayData " & vbCrLf

        Return jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))
    End Function

    Function MPOSDashboardTransactionCount()
        Dim pSql As String

        pSql = " SELECT ISNULL(SUM(TransactionCount), 0) as Actual, " & vbCrLf &
                " ISNULL((SELECT SUM(TransactionCount) FROM ##LastYearDateData), 0) as LastYearDate, " & vbCrLf &
                " ISNULL((CAST(SUM(TransactionCount) - (SELECT SUM(TransactionCount) FROM ##LastYearDateData) as FLOAT) / (SELECT SUM(TransactionCount) FROM ##LastYearDateData)) * 100, 0) as Growth, " & vbCrLf &
                " ISNULL((SELECT COUNT(DISTINCT FowstID) FROM ##YesterdayData), 0) as StoreCount, " & vbCrLf &
                " ISNULL(SUM(TransactionCount) / (SELECT COUNT(DISTINCT FowstID) FROM ##YesterdayData), 0) as AvgDaily " & vbCrLf &
                " FROM ##YesterdayData " & vbCrLf

        Return jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))
    End Function

    Function MPOSDashboardSalesMix()
        Dim pSql As String

        pSql = " SELECT ISNULL(0, 0) as TakeOut, " & vbCrLf &
                " ISNULL((0 / (SUM(DeliverySales) + SUM(WalkInSales))) * 100, 0) as TakeOutPercentage, " & vbCrLf &
                " ISNULL(SUM(DeliverySales), 0) as Delivery, " & vbCrLf &
                " ISNULL((CAST(SUM(DeliverySales) as FLOAT) / (SUM(DeliverySales) + SUM(WalkInSales))) * 100, 0) as DeliveryPercentage, " & vbCrLf &
                " ISNULL(SUM(WalkInSales), 0) as DineIn, " & vbCrLf &
                " ISNULL((CAST(SUM(WalkInSales) as FLOAT) / (SUM(DeliverySales) + SUM(WalkInSales))) * 100, 0) as DineInPercentage, " & vbCrLf &
                " ISNULL(0, 0) as DriveThru, " & vbCrLf &
                " ISNULL((0 / (SUM(DeliverySales) + SUM(WalkInSales))) * 100, 0) as DriveThruPercentage, " & vbCrLf &
                " ISNULL(SUM(DeliverySales) + SUM(WalkInSales), 0) as Total, " & vbCrLf &
                " ISNULL((CAST((SUM(DeliverySales) + SUM(WalkInSales)) as FLOAT) / (SUM(DeliverySales) + SUM(WalkInSales))) * 100, 0) as TotalPercentage " & vbCrLf &
                " FROM ##YesterdayData " & vbCrLf

        Return jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))
    End Function

    Function MPOSDashboardTransactionCountMix()
        Dim pSql As String

        pSql = " SELECT ISNULL(0, 0) as TakeOut, " & vbCrLf &
                " ISNULL((0 / (SUM(DeliveryTransactionCount) + SUM(WalkInTransactionCount))) * 100, 0) as TakeOutPercentage, " & vbCrLf &
                " ISNULL(SUM(DeliveryTransactionCount), 0) as Delivery, " & vbCrLf &
                " ISNULL((CAST(SUM(DeliveryTransactionCount) as FLOAT) / (SUM(DeliveryTransactionCount) + SUM(WalkInTransactionCount))) * 100, 0) as DeliveryPercentage, " & vbCrLf &
                " ISNULL(SUM(WalkInTransactionCount), 0) as DineIn, " & vbCrLf &
                " ISNULL((CAST(SUM(WalkInTransactionCount) as FLOAT) / (SUM(DeliveryTransactionCount) + SUM(WalkInTransactionCount))) * 100, 0) as DineInPercentage, " & vbCrLf &
                " ISNULL(0, 0) as DriveThru, " & vbCrLf &
                " ISNULL((0 / (SUM(DeliveryTransactionCount) + SUM(WalkInTransactionCount))) * 100, 0) as DriveThruPercentage, " & vbCrLf &
                " ISNULL(SUM(DeliveryTransactionCount) + SUM(WalkInTransactionCount), 0) as Total, " & vbCrLf &
                " ISNULL((CAST(SUM(DeliveryTransactionCount) + SUM(WalkInTransactionCount)as FLOAT) / (SUM(DeliveryTransactionCount) + SUM(WalkInTransactionCount))) * 100, 0) as TotalPercentage " & vbCrLf &
                " FROM ##YesterdayData " & vbCrLf

        Return jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))
    End Function
End Class
