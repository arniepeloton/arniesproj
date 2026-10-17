Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class CacheSalesTables

    Dim mProData As ProData
    Dim mParent As Business

    Public InvoiceDate As Date
    Public FowstID As String
    Dim mLastResponse As LogInput
    Dim mLastFailResponse As LogInput
    Dim mLogFolder As String

#If DEBUG Then
    Const mStartingDate As Date = #2025-10-01#
#Else
    Const mStartingDate As Date = #2026-01-01#
#End If

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Sub Execute(ByRef oError As String)

        InvoiceDate = InvoiceDate.Date
        mParent.ProData.CommandTimeout = 10 * 60
        Dim pOtherPayments As List(Of String)
        Dim pDtSource = GetSourceData(pOtherPayments, FowstID)
        AppendToLog("ref: vbSQMS55FNU7! CacheSalesTables.Execute GetSourceData count: " & pDtSource.Rows.Count)
        Upsert(pDtSource)
        UpsertPayments(pDtSource, pOtherPayments)
        UpsertFinancials(pDtSource)

    End Sub

    Function GetOtherPaymentTypesInCacheInvoices() As List(Of String)

        Dim pDtColumns = mProData.ColumnsInformation("CacheInvoices")
        Dim pOut As New List(Of String)
        Dim pFileText = FileText(Common.ServerPath & "Content/OtherPaymentTypes.txt")
        Dim pSplits = pFileText.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        For Each pSplit In pSplits
            If pDtColumns.Select("name = " & ReadyString(EscapeSqlName(pSplit))).Count > 0 Then
                pOut.Add(pSplit)
            End If
        Next

        Return pOut

    End Function

    Function GetSourceData(ByRef oOtherPayments As List(Of String), vFowstID As String) As DataTable

        Dim pSql = "SELECT CacheInvoices.ID" & vbCrLf &
            ",CacheInvoices.FowstCode -- StoreCode" & vbCrLf &
            ",CacheInvoices.InvoiceNo -- invoiceno & payment.number" & vbCrLf &
            ",TransInvoice.DeviceFowstTransactionNumber -- tran_no" & vbCrLf &
            ",CacheInvoices.InvoiceDate --txdate & txtime & timeclosed" & vbCrLf &
            ",CacheInvoices.DeliveryMethod --txtype" & vbCrLf &
            ",CacheInvoices.CustomerID --customerid" & vbCrLf &
            ",CacheInvoices.LastUpdated --timedispatched" & vbCrLf &
            ",CacheInvoices.UpdatedByName --enteredby(50) & cashiername(50) & payment.reference(200)" & vbCrLf &
            ",CacheInvoices.UserID --cashier(50)" & vbCrLf &
            ",LibraryDevice.UUID --payment.terminal(255)" & vbCrLf &
            ",TransInvoice.DateImported --payment.date_Ingested_Frm_Raw" & vbCrLf &
            ",TransInvoice.Vatable" & vbCrLf &
            ",TransInvoice.VATExempt" & vbCrLf &
            ",TransInvoice.ForegoneVAT" & vbCrLf &
            ",TransInvoice.VAT" & vbCrLf &
            ",TransInvoice.SCDiscount" & vbCrLf &
            ",TransInvoiceDiscount.NoOfSeniorCitizens --payment.seniorcitizencount" & vbCrLf &
            ",CacheInvoices.Discount" & vbCrLf &
            ",CacheInvoices.GrossInvoiceAmount" & vbCrLf &
            ",CacheInvoices.NetInvoiceAmount" & vbCrLf &
            ",TransInvoice.GrossSalesToSC" & vbCrLf &
            ",CacheInvoices.TaxAmount" & vbCrLf &
            ",CacheInvoices.AdjustmentAmount" & vbCrLf &
            ",ISNULL(MobizLibraryOrganizationFOWST.IsVat, 0) AS IsVat" & vbCrLf
        If oOtherPayments Is Nothing Then
            oOtherPayments = GetOtherPaymentTypesInCacheInvoices()
        End If
        For Each pOtherPayment In oOtherPayments
            pSql &= ",CacheInvoices." & EscapeSqlName(pOtherPayment) & vbCrLf
        Next
        pSql &= ",CacheInvoices.TotalPayment"
        For Each pOtherPayment In oOtherPayments
            pSql &= " + "
            pSql &= "CacheInvoices." & EscapeSqlName(pOtherPayment) & vbCrLf
        Next
        pSql &= " AS TotalPayment" & vbCrLf
        pSql &= ",tblSeniorCitizen.PersonOtherIDNo" & vbCrLf
        pSql &= " FROM CacheInvoices" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = CacheInvoices.ID" & vbCrLf &
            " LEFT JOIN TransInvoiceDiscount ON TransInvoiceDiscount.InvoiceID = CacheInvoices.ID" & vbCrLf &
            " LEFT JOIN LibraryDevice ON LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FowstID" & vbCrLf &
            "  OUTER APPLY (" & vbCrLf &
            "  	SELECT MobizLibraryOrganization.PersonOtherIDNo" & vbCrLf &
            "     FROM TransInvoiceSeniorCitizenPWDCustomer" & vbCrLf &
            "     JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = TransInvoiceSeniorCitizenPWDCustomer.CustomerID" & vbCrLf &
            "     WHERE TransInvoiceSeniorCitizenPWDCustomer.InvoiceID = TransInvoice.ID" & vbCrLf &
            "  ) tblSeniorCitizen" & vbCrLf &
            " WHERE CONVERT(date, CacheInvoices.InvoiceDate) = " & ReadyString(Format(InvoiceDate, cGlobal.FormatDateSql)) & vbCrLf &
            " AND CacheInvoices.InvoiceNo > 0" & vbCrLf &
            " AND CacheInvoices.FowstID = " & ReadyString(vFowstID) & vbCrLf &
            " ORDER BY CacheInvoices.InvoiceDate"
        Dim pFilename = System.IO.Path.Combine(Common.ServerPath, "temp\logs\SalesTable", "GetSourceData.sql")
        FileText(pFilename) = pSql
        Dim pDT = mProData.Execute(pSql)
        pDT.Columns.Add("MemoAmount", GetType(Decimal))

        Return pDT

    End Function

    Sub Upsert(vDtSource As DataTable)

        For Each pDR As DataRow In vDtSource.Rows
            Dim pSql = "SELECT SUM(TotalAmount)" & vbCrLf &
                " FROM TransMemoSalesReturn" & vbCrLf &
                " WHERE InvoiceID = " & ReadyString(pDR("ID"))
            pDR("MemoAmount") = CDec2(mProData.ExecuteScalar(pSql))

            Dim pIU = New InsertUpdate("CacheSalesTableInvoice")
            pIU("ID") = ReadyString(pDR("ID"))
            pIU("store_code") = ReadyString(TrimToLength(pDR("FowstCode"), 10))
            pIU("tran_no") = CInt2(pDR("DeviceFowstTransactionNumber"))
            pIU("tx_date") = ReadyDateTime(pDR("InvoiceDate"))
            pIU("tx_time") = ReadyDateTime(pDR("InvoiceDate"))
            pIU("time_closed") = ReadyDateTime(pDR("InvoiceDate"))
            If CStr2(pDR("DeliveryMethod")).Trim() = "" Then
                pIU("tx_type") = ReadyString(TrimToLength("Dine In", 50))
            Else
                pIU("tx_type") = ReadyString(TrimToLength(pDR("DeliveryMethod"), 50))
            End If
            pIU("customer_id") = ReadyString(pDR("CustomerID"))
            'pIU("timedispatched") = ReadyDateTime(pDR("LastUpdated"))
            pIU("entered_by") = ReadyString(TrimToLength(pDR("UpdatedByName"), 50))
            pIU("cashier_name") = ReadyString(TrimToLength(pDR("UpdatedByName"), 50))
            If pDR("UserID") IsNot DBNull.Value Then
                pIU("cashier") = ReadyString(pDR("UserID"))
            End If
            pIU("invoice_no") = CInt2(pDR("InvoiceNo"))
            pIU("amount_gross") = CDec2(pDR("GrossInvoiceAmount"))
            pIU("amount_due") = CDec2(pDR("TotalPayment"))
            pIU("terminal") = ReadyString(TrimToLength(pDR("UUID"), 255))
            If CDec2(pDR("MemoAmount")) <> 0 Then
                pIU("status") = "'RF'"
            Else
                pIU("status") = "'ST'"
            End If
            pIU("loyalty_card_pan") = ReadyString("N/A")
            pIU("loyalty_points") = 0

            If CBool2(pDR("IsVat")) Then
                pIU("vatable_sales") = CDec2(pDR("Vatable")) ' dapat i-if kung non-VAT ang fowst. dapat zero ni
            Else
                pIU("vatable_sales") = 0 ' dapat i-if kung non-VAT ang fowst. dapat zero ni
            End If

            ' current code: 2026-03-30
            ' commented old code
            pIU("vat_exempt_sales") = CDec2(pDR("ForegoneVAT")) ' CDec2(pDR("VATExempt"))
            If CBool2(pDR("IsVat")) Then
                pIU("non_vatable_sales") = CDec2(pDR("VATExempt")) ' CDec2(pDR("ForegoneVAT")) ' dapat i-if kung non-VAT ang fowst. dapat invoice amount ni
                pIU("vat") = CDec2(pDR("VAT")) ' dapat i-if kung non-VAT ang fowst. dapat zero ni
            Else
                pIU("non_vatable_sales") = CDec2(pDR("GrossInvoiceAmount")) ' CDec2(pDR("ForegoneVAT")) ' dapat i-if kung non-VAT ang fowst. dapat invoice amount ni
                pIU("vat") = 0
            End If

            pIU("sc") = 0 ' service charge. always zero. CDec2(pDR("SCDiscount"))
            pIU("sr_count") = If(CDec2(pDR("SCDiscount")) > 0, 1, 0)
            pIU("discount") = CDec2(pDR("Discount"))
            pIU("sub_tx_type") = "'N/A'"
            pIU("amount_pwd_discount") = CDec2(0)
            pIU("amount_senior_citizen_discount") = CDec2(pDR("SCDiscount"))
            pIU("is_from") = "'N/A'"
            pIU("date_ingested_frm_raw") = ReadyDateTime(pDR("InvoiceDate"))
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            mProData.BeginTransaction()
            Dim pAffectedRecords = mProData.ExecuteSQL(pSql)
            If pAffectedRecords = 0 Then
                pSql = pIU.GetInsert()
                mProData.ExecuteSQL(pSql)
            End If
            UpsertItems(CStr2(pDR("ID")), TrimToLength(pDR("FowstCode"), 10), pDR)
            mProData.CommitTransaction()
        Next

    End Sub

    Function GetSourceItems(vInvoiceID As String) As DataTable

        Dim pSql = "SELECT TransInvoiceItems.DiscountedUnitPrice -- adjnet" & vbCrLf &
            ", TransInvoiceItems.DiscountTax --adjtax" & vbCrLf &
            ", tblCategory.Category --class_name(50)" & vbCrLf &
            ", TransInvoiceItems.DiscountAmount --Discount" & vbCrLf &
            ", TransInvoiceItems.ID --ID" & vbCrLf &
            ", TransInvoiceItems.SubTotal --Net" & vbCrLf &
            ", TransInvoiceItems.UnitPrice --original_price" & vbCrLf &
            ", TransInvoiceItems.Quantity --Quantity" & vbCrLf &
            ", TransInvoiceItems.TaxSales --vatable_sales" & vbCrLf &
            ", TransInvoiceItems.TaxAmount --tax" & vbCrLf &
            ", MobizLibraryPIMProduct.ProductNo --itemid(50)" & vbCrLf &
            ", MobizLibraryPIMProduct.ProductName --itemid(50)" & vbCrLf &
            ", TransInvoice.InvoiceDate --txdate & txtime & timeclosed" & vbCrLf &
            ", TransInvoice.DeviceFowstTransactionNumber --tranno & invoiceno & payment.number" & vbCrLf &
            ", LibraryDevice.UUID --payment.terminal(255)" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " JOIN TransInvoice On TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
            " LEFT JOIN LibraryDevice On LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            "   Select TOP 1 MobizLibraryPIMProductVariableFeatures001.C0020 As Category" & vbCrLf &
            "   FROM MobizLibraryPIMProductVariableFeatures001" & vbCrLf &
            "   WHERE MobizLibraryPIMProductVariableFeatures001.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            ") tblCategory" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProduct On MobizLibraryPIMProduct.ID = TransInvoiceItems.ProductID" & vbCrLf &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf
        Dim pDT = mProData.Execute(pSql)

        pSql = "SELECT CONVERT(decimal, 0) AS DiscountedUnitPrice -- TransInvoiceItems.DiscountedUnitPrice -- adjnet" & vbCrLf &
            ", CONVERT(decimal, 0) AS DiscountTax -- TransInvoiceItems.DiscountTax --adjtax -- wla ni ginagamit" & vbCrLf &
            ", tblCategory.Category --class_name(50)" & vbCrLf &
            ", CONVERT(decimal, 0) AS DiscountAmount -- TransInvoiceItems.DiscountAmount --Discount -- computed sa code" & vbCrLf &
            ", TransInvoiceItems.ID --ID" & vbCrLf &
            ", TransInvoiceItemsModifier.OrderedQty * TransInvoiceItemsModifier.ModifierPrice + TransInvoiceItemsModifier.PremiumPrice AS SubTotal -- TransInvoiceItems.SubTotal --Net" & vbCrLf &
            ", TransInvoiceItemsModifier.ModifierPrice + TransInvoiceItemsModifier.PremiumPrice AS UnitPrice --original_price" & vbCrLf &
            ", TransInvoiceItemsModifier.OrderedQty AS Quantity" & vbCrLf &
            ", (TransInvoiceItemsModifier.OrderedQty * TransInvoiceItemsModifier.ModifierPrice + TransInvoiceItemsModifier.PremiumPrice) / 1.12 AS TaxSales -- TransInvoiceItems.TaxSales --vatable_sales" & vbCrLf &
            ", (TransInvoiceItemsModifier.OrderedQty * TransInvoiceItemsModifier.ModifierPrice + TransInvoiceItemsModifier.PremiumPrice) - (TransInvoiceItemsModifier.OrderedQty * TransInvoiceItemsModifier.ModifierPrice + TransInvoiceItemsModifier.PremiumPrice) / 1.12 AS TaxAmount -- TransInvoiceItems.TaxAmount --tax" & vbCrLf &
            ", MobizLibraryPIMProduct.ProductNo --itemid(50)" & vbCrLf &
            ", MobizLibraryPIMProduct.ProductName --itemid(50)" & vbCrLf &
            ", TransInvoice.InvoiceDate --txdate & txtime & timeclosed" & vbCrLf &
            ", TransInvoice.DeviceFowstTransactionNumber --tranno & invoiceno & payment.number" & vbCrLf &
            ", LibraryDevice.UUID --payment.terminal(255)" & vbCrLf &
            " FROM LibraryItemModifierInventoryAffecting" & vbCrLf &
            " JOIN TransInvoiceItemsModifier ON TransInvoiceItemsModifier.ModifierID = LibraryItemModifierInventoryAffecting.ID" & vbCrLf &
            " JOIN TransInvoiceItems ON TransInvoiceItems.ID = TransInvoiceItemsModifier.InvoiceItemID" & vbCrLf &
            " JOIN TransInvoice On TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
            " LEFT JOIN LibraryDevice On LibraryDevice.ID = TransInvoice.DeviceID" & vbCrLf &
            " LEFT JOIN MobizLibraryPIMProduct On MobizLibraryPIMProduct.ID = LibraryItemModifierInventoryAffecting.ProductID" & vbCrLf &
            " OUTER APPLY (" & vbCrLf &
            "   Select TOP 1 MobizLibraryPIMProductVariableFeatures001.C0020 As Category" & vbCrLf &
            "   FROM MobizLibraryPIMProductVariableFeatures001" & vbCrLf &
            "   WHERE MobizLibraryPIMProductVariableFeatures001.ProductID = LibraryItemModifierInventoryAffecting.ProductID" & vbCrLf &
            ") tblCategory" & vbCrLf &
            " WHERE TransInvoiceItemsModifier.OrderedQty > 0" & vbCrLf &
            " AND TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID)
        Dim pDtModifiers = mProData.Execute(pSql)
        pDT.Merge(pDtModifiers)

        Return pDT

    End Function

    Sub UpsertItems(vInvoiceID As String, vFowstCode As String, vDrInvoice As DataRow)

        Dim pSql = "DELETE FROM CacheSalesTableOrder" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
        mProData.ExecuteSQL(pSql)

        Dim pDtSourceItems = GetSourceItems(vInvoiceID)
        For Each pDrItem As DataRow In pDtSourceItems.Rows
            Dim pIU = New InsertUpdate("CacheSalesTableOrder")
            pIU("InvoiceID") = ReadyString(vInvoiceID)
            pIU("class_name") = ReadyString(TrimToLength(pDrItem("Category"), 50))
            pIU("id") = ReadyString(pDrItem("ID"))
            pIU("[key]") = ReadyString(pDrItem("ID"))
            pIU("item_id") = ReadyString(TrimToLength(pDrItem("ProductNo"), 50))
            pIU("item_name") = ReadyString(TrimToLength(pDrItem("ProductName"), 100))
            pIU("date_ingested_frm_raw") = "GETDATE()"
            pIU("disc_perc") = "0"
            pIU("is_from") = ReadyString("N/A")
            pIU("loyalty_card_pan") = ReadyString("N/A")
            pIU("loyalty_point") = "0"
            If CBool2(vDrInvoice("IsVat")) = False Then
                pDrItem("TaxAmount") = 0
            End If
            pIU("net") = CDec2(pDrItem("SubTotal")) - CDec2(pDrItem("DiscountAmount")) - CDec2(pDrItem("TaxAmount")) '  CDec2(pDrItem("SubTotal")) / 1.12
            pIU("original_price") = CDec2(pDrItem("UnitPrice"))
            pIU("pwd_discount_amount") = 0
            pIU("quantity") = CDec2(pDrItem("Quantity"))

            Dim pCappedVatableSales = CDec2(vDrInvoice("GrossInvoiceAmount")) - CDec2(vDrInvoice("GrossSalesToSC"))
            If CDec2(pDrItem("SubTotal")) >= pCappedVatableSales _
                AndAlso CDec2(vDrInvoice("SCDiscount")) > 0 Then

                pIU("net") = CDec2(pDrItem("SubTotal")) / 1.12
                If pCappedVatableSales > 0 Then
                    ' capped
                    pIU("senior_discount_amount") = (CDec2(vDrInvoice("GrossSalesToSC")) / 1.12) - (CDec2(vDrInvoice("GrossSalesToSC")) / 1.12 * 0.8)
                    pIU("tax_exempt") = CDec2(vDrInvoice("GrossSalesToSC")) - CDec2(vDrInvoice("GrossSalesToSC")) / 1.12
                    pIU("vat_exempt") = CDec2(vDrInvoice("GrossSalesToSC")) / 1.12 ' vat exempt sales pasabot ani nila
                    pIU("vatable_sales") = pCappedVatableSales / 1.12
                    'pIU("adj_tax") = 0 ' old code 
                    pIU("adj_tax") = pCappedVatableSales - (pCappedVatableSales / 1.12)
                    pIU("discount") = CDec2(pIU("senior_discount_amount"))
                    pDrItem("TaxAmount") = pCappedVatableSales - pCappedVatableSales / 1.12
                    pIU("adj_net") = CDec2(vDrInvoice("GrossSalesToSC")) / 1.12 * 0.8 + pCappedVatableSales / 1.12
                    ' same lang ang "net" sa gawas
                Else
                    ' uncapped
                    pIU("senior_discount_amount") = (CDec2(pDrItem("SubTotal")) / 1.12) - (CDec2(pDrItem("SubTotal")) / 1.12 * 0.8)
                    pIU("tax_exempt") = CDec2(pDrItem("SubTotal")) - CDec2(pDrItem("SubTotal")) / 1.12
                    pIU("vat_exempt") = CDec2(pDrItem("SubTotal")) / 1.12 ' vat exempt sales pasabot ani nila
                    pIU("vatable_sales") = 0
                    ' pIU("adj_tax") = 0 ' old code 
                    ' pIU("adj_tax") = CDec2(pDrItem("SubTotal")) - CDec2(pDrItem("SubTotal")) / 1.12 ' 2026-03-30 ' commented on 2026-05-21
                    pIU("adj_tax") = 0 ' 2026-05-21
                    pIU("discount") = CDec2(pIU("senior_discount_amount"))
                    pIU("adj_net") = CDec2(pDrItem("SubTotal")) / 1.12 * 0.8
                End If
            Else
                pIU("senior_discount_amount") = 0
                pIU("tax_exempt") = 0
                pIU("vat_exempt") = 0
                If CBool2(vDrInvoice("IsVat")) Then
                    pIU("vatable_sales") = CDec2(pDrItem("TaxSales")) ' dapat i-if kung non-VAT ang fowst. dapat zero ni
                    pIU("adj_tax") = CDec2(pDrItem("SubTotal")) - CDec2(pDrItem("SubTotal")) / 1.12 ' CDec2(pDrItem("DiscountTax"))
                Else
                    pIU("vatable_sales") = 0
                    pIU("adj_tax") = 0
                End If
                pIU("discount") = 0
            End If
            If pIU.Items.ContainsKey("adj_net") = False Then
                pIU("adj_net") = CDec2(pDrItem("SubTotal")) - CDec2(pIU("senior_discount_amount")) - CDec2(pDrItem("TaxAmount"))
            End If
            If CDec2(vDrInvoice("MemoAmount")) <> 0 Then
                pIU("status") = "'RF'"
            Else
                pIU("status") = "'ST'"
            End If
            pIU("store_code") = ReadyString(TrimToLength(vFowstCode, 10))
            If CBool2(vDrInvoice("IsVat")) Then
                ' commented on 2026-05-25
                'pIU("tax") = CDec2(pDrItem("TaxAmount")) ' dapat i-if kung non-VAT ang fowst. dapat zero ni
                pIU("tax") = CDec2(pIU("net")) * 0.12 ' 2026-05-25
            Else
                pIU("tax") = 0
            End If
            pIU("adj_sc") = 0 ' todo
            pIU("terminal") = ReadyString(TrimToLength(pDrItem("UUID"), 255))
            pIU("tran_no") = CInt2(pDrItem("DeviceFowstTransactionNumber"))
            pIU("tx_date") = ReadyDateTime(pDrItem("InvoiceDate"))
            pIU("tx_time") = ReadyDateTime(pDrItem("InvoiceDate"))
            If CStr2(vDrInvoice("DeliveryMethod")).Trim() = "" Then
                pIU("tx_type") = ReadyString(TrimToLength("Dine In", 50))
            Else
                pIU("tx_type") = ReadyString(TrimToLength(vDrInvoice("DeliveryMethod"), 50))
            End If
            pIU("updated_by") = ReadyString(TrimToLength("", 255))
            ' pIU("adj_net") = CDec2(pDrItem("SubTotal")) / 1.12 * 0.8 ' mali ni xa 
            pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        Next

    End Sub

    Function GetOtherPayments(vInvoiceID As String) As DataTable

        Dim pSql = "SELECT TransCustomerPaymentOther.PaymentType" & vbCrLf &
            ",TransCustomerPaymentOther.Amount" & vbCrLf &
            ",TransCustomerPaymentOther.GCashRefNo" & vbCrLf &
            " FROM TransCustomerPaymentOther" & vbCrLf &
            " JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentOther.CustomerPaymentID" & vbCrLf &
            " WHERE TransCustomerPaymentInvoices.InvoiceID = " & ReadyString(vInvoiceID)
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Sub UpsertPayments(vDtSource As DataTable, vOtherPayments As List(Of String))

        Const pSeniorCitizenDiscountTenderID = "Senior Citizen Discount"

        For Each pDR As DataRow In vDtSource.Rows
            mProData.BeginTransaction()
            Dim pSql = "DELETE FROM CacheSalesTablePayments WHERE ID = " & ReadyString(pDR("ID"))
            mProData.ExecuteSQL(pSql)

            Dim pDtOtherPayments = GetOtherPayments(CStr2(pDR("ID")))
            pDtOtherPayments.Columns.Add("Status")
            pDtOtherPayments.Columns.Add("tender_name")
            pDtOtherPayments.Columns.Add("number")

            Dim pCashAmount As Decimal = CDec2(pDR("TotalPayment"))
            For Each pDrOtherPayment As DataRow In pDtOtherPayments.Rows
                pCashAmount -= CDec2(pDrOtherPayment("Amount"))
            Next pDrOtherPayment
            If pCashAmount > 0 Then
                Dim pDrOtherPayment = pDtOtherPayments.NewRow
                pDrOtherPayment("Amount") = pCashAmount
                pDrOtherPayment("PaymentType") = "Cash"
                pDtOtherPayments.Rows.Add(pDrOtherPayment)
            End If

            If CDec2(pDR("SCDiscount")) <> 0 Then
                Dim pDrOtherPayment = pDtOtherPayments.NewRow
                pDrOtherPayment("Amount") = CDec2(pDR("SCDiscount"))
                pDrOtherPayment("PaymentType") = "Cash"
                pDrOtherPayment("tender_name") = pSeniorCitizenDiscountTenderID
                pDrOtherPayment("number") = CStr2(pDR("PersonOtherIDNo"))
                pDtOtherPayments.Rows.Add(pDrOtherPayment)
            End If

            For Each pDrOtherPayment As DataRow In pDtOtherPayments.Rows
                Dim pIU = New InsertUpdate("CacheSalesTablePayments")
                pIU("id") = ReadyString(pDR("ID"))
                pIU("store_code") = ReadyString(TrimToLength(pDR("FowstCode"), 10))
                pIU("tran_no") = CInt2(pDR("DeviceFowstTransactionNumber"))
                pIU("tx_date") = ReadyDateTime(pDR("InvoiceDate"))
                If CStr2(pDrOtherPayment("tender_name")) = pSeniorCitizenDiscountTenderID Then
                    pIU("tender_id") = ReadyString(pSeniorCitizenDiscountTenderID)
                Else
                    pIU("tender_id") = ReadyString(TrimToLength(pDrOtherPayment("PaymentType"), 50))
                End If
                If pDrOtherPayment("tender_name") Is DBNull.Value Then
                    pIU("tender_name") = pIU("tender_id")
                    If pDrOtherPayment("PaymentType") <> "Cash" Then
                        pIU("number") = ReadyString(TrimToLength(pDrOtherPayment("GCashRefNo"), 200)) ' 2026-03-30  'old code CInt2(pDR("InvoiceNo"))
                    Else
                        pIU("number") = "''"
                    End If
                Else
                    pIU("tender_name") = ReadyString(pDrOtherPayment("tender_name"))
                    pIU("number") = ReadyString(TrimToLength(pDrOtherPayment("number"), 200)) ' 2026-03-30  'old code CInt2(pDR("InvoiceNo"))
                End If
                pIU("tender_type") = pIU("tender_name")

                pIU("reference") = "''" ' 2026-04-04 blank dapat pero dapat naa ang field
                pIU("amount") = CDec2(pDrOtherPayment("Amount"))
                pIU("senior_citizen_count") = CInt2(pDR("NoOfSeniorCitizens"))
                pIU("authorize_id") = ReadyString(pDR("UserID"))
                pIU("authorize_name") = ReadyString(TrimToLength(pDR("UpdatedByName"), 50))
                pIU("terminal") = ReadyString(TrimToLength(pDR("UUID"), 255))
                If IsDBNull(pDR("DateImported")) Then
                    pIU("date_Ingested_Frm_Raw") = ReadyDateTime(pDR("InvoiceDate"))
                Else
                    pIU("date_Ingested_Frm_Raw") = ReadyDateTime(pDR("DateImported"))
                End If
                pIU("tip") = 0
                pIU("is_from") = ReadyString("N/A")
                pIU("tin") = ReadyString("N/A")
                If CDec2(pDR("MemoAmount")) <> 0 Then
                    pIU("status") = "'RF'"
                Else
                    pIU("status") = "'ST'"
                End If
                pSql = pIU.GetInsert()
                mProData.ExecuteSQL(pSql)
            Next pDrOtherPayment

            Dim pIuInvoice = New InsertUpdate("TransInvoice")
            pIuInvoice("SpaviCacheDate") = "GETDATE()"
            pSql = pIuInvoice.GetUpdate("ID = " & ReadyString(pDR("ID")) &
                " AND SpaviCacheDate IS NULL")
            mProData.ExecuteSQL(pSql)

            mProData.CommitTransaction()
        Next

    End Sub

    Function GetTenderID(vDR As DataRow, vOtherPayments As List(Of String)) As String

        Dim pOut As String = ""
        For Each pOtherPayment In vOtherPayments
            If CDec2(vDR(pOtherPayment)) <> 0 Then
                If pOut <> "" Then
                    pOut &= ", "
                End If
                pOut &= pOtherPayment
            End If
        Next

        Return pOut

    End Function

    Sub UpsertFinancials(vDtSource As DataTable)

        Dim pDtGrouped = vDtSource.Clone()
        Dim pUniqueCombos As New Dictionary(Of String, DataRow)
        For Each pDR As DataRow In vDtSource.Rows
            Dim pKey = CStr2(pDR("FowstCode")) & "|" & CType(pDR("InvoiceDate"), Date).ToString("yyyyMMdd")
            Dim pDrExisting As DataRow = Nothing
            If pUniqueCombos.TryGetValue(pKey, pDrExisting) Then
                For Each pField In {"GrossInvoiceAmount", "TotalPayment", "TaxAmount", "AdjustmentAmount"}
                    pDrExisting(pField) = CDec2(pDrExisting(pField)) + CDec2(pDR(pField))
                Next pField
            Else
                Dim pDrNew = pDtGrouped.NewRow()
                pDrNew.ItemArray = pDR.ItemArray.Clone()
                pDtGrouped.Rows.Add(pDrNew)
                pUniqueCombos.Add(pKey, pDrNew)
            End If
        Next pDR

        For Each pDR As DataRow In pDtGrouped.Rows
            Dim pIU = New InsertUpdate("CacheSalesTableFinancial")
            pIU("InvoiceID") = ReadyString(pDR("ID"))
            pIU("store_code") = ReadyString(TrimToLength(pDR("FowstCode"), 10))
            pIU("tx_date") = ReadyDateTime(pDR("InvoiceDate"))
            pIU("gross") = CDec2(pDR("GrossInvoiceAmount"))
            pIU("discount") = CDec2(pDR("TotalPayment"))
            pIU("sc") = 0 ' CDec2(pDR("SCDiscount"))
            pIU("tax") = CDec2(pDR("TaxAmount"))
            pIU("adj_net") = 0 ' CDec2(pDrItem("DiscountedUnitPrice"))
            pIU("adj_tax") = CDec2(pDR("AdjustmentAmount"))
            pIU("take_out") = 0
            pIU("delivery") = 0
            pIU("refund") = 0
            pIU("cancelled") = 0
            pIU("void") = 0
            pIU("tx_count") = 0
            pIU("cash") = 0
            pIU("debit") = 0
            pIU("credit") = 0
            pIU("certificate") = 0
            pIU("[check]") = 0
            pIU("vatable_sales") = 0
            pIU("non_vatable_sales") = 0
            pIU("vat_exempt") = 0
            pIU("sr_count") = 0
            pIU("sr_discount") = 0
            pIU("eod_time") = 0
            pIU("upload_date") = "GETDATE()"
            pIU("is_from") = ReadyString("N/A")
            pIU("terminal") = ReadyString(TrimToLength(pDR("UUID"), 255))
            Dim pSql = pIU.GetUpdate("InvoiceID = " & ReadyString(pDR("ID")))
            mProData.BeginTransaction()
            Dim pAffectedRecords = mProData.ExecuteSQL(pSql)
            If pAffectedRecords = 0 Then
                pSql = pIU.GetInsert()
                mProData.ExecuteSQL(pSql)
            End If
            mProData.CommitTransaction()
        Next

    End Sub

    ' payment is per transaction
    ' financial is per transaction



    Public Class LogInput
        Public status As String
        Public uploadedTo As String
        Public s3Key As String
    End Class

    Public Class LogOutput
        Public brand As String
        Public store_code As String
        Public terminal As String
        Public [date] As String
        Public event_type As String
        Public status As String
        Public timestamp As String
        <JsonProperty(DefaultValueHandling:=DefaultValueHandling.Ignore)>
        Public s3_path As String
        Public source As String
        <JsonProperty(DefaultValueHandling:=DefaultValueHandling.Ignore)>
        Public is_delayed As Boolean?
        <JsonProperty(DefaultValueHandling:=DefaultValueHandling.Ignore)>
        Public is_reprocessed As Boolean?
        Public table As String
    End Class

    Public Enum eEventType
        DataPreparationStarted
        DataPreparationCompleted
        DataSubmitRequestSent
        DataSubmitResponse
    End Enum

    Public Shared Function ComposeLogPayload(vInput As LogInput, vEventType As eEventType, vStoreCode As String,
        vTable As String, vTerminal As String, vTransactionDate As Date) As String

        Dim pLog As New LogOutput
        pLog.table = vTable
        pLog.brand = "potatocorner"
        pLog.store_code = vStoreCode
        pLog.terminal = vTerminal
        pLog.event_type = vEventType.ToString()
        If CStr2(pLog?.status).Trim() = "" Then
            pLog.status = "SUCCESS"
        Else
            pLog.status = If(pLog.status?.Trim()?.ToLower() = "success", "SUCCESS", "FAILED")
        End If
        pLog.timestamp = Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss'Z'")
        pLog.source = "MPOS"
        If vEventType = eEventType.DataSubmitResponse _
            AndAlso pLog.status = "SUCCESS" Then

            pLog.s3_path = "s3://" & vInput?.s3Key
            pLog.is_delayed = If(vInput?.s3Key?.Contains("/delayed/"), False)
            pLog.is_reprocessed = If(vInput?.s3Key?.Contains("/reprocessed/"), False)
        End If
        pLog.date = vTransactionDate.ToString("yyyy-MM-dd")
        Return JsonConvert.SerializeObject(pLog, Formatting.Indented)

    End Function

    Function GetFirstStore() As DataRow

        Dim pSql = "SELECT TOP 1 store_code" & vbCrLf &
            ",tx_date" & vbCrLf &
            ",terminal" & vbCrLf &
            " FROM CacheSalesTableInvoice" & vbCrLf &
            " WHERE LTRIM(RTRIM(store_code)) <> ''" & vbCrLf &
            " AND tx_date >= " & ReadyDateTime(mStartingDate.Date) & vbCrLf &
            " AND DateSent IS NULL" & vbCrLf &
            " GROUP BY store_code" & vbCrLf &
            " ,tx_date" & vbCrLf &
            " ,terminal" & vbCrLf &
            " ,DateError" & vbCrLf &
            " ORDER BY DateError, tx_date, store_code"
        Dim pDR = mParent.ProData.ExecuteRow(pSql)
        Return pDR

    End Function

    Sub AddToLog(vText As String)

        AppendToLog(vText, "temp\logs\SalesTable\logs", mLogFolder)

    End Sub

    'Sub SaveJsonText(vJson As String, vTableName As String, vStoreCode As String, vTransactionDate As Date)

    '    Dim pFilename = IO.Path.Combine(AppPath, "temp\logs\SalesTable\json", vTransactionDate.ToString("yyyy-MM-dd") & " " & vStoreCode & " " & vTableName & ".log")
    '    FileText(pFilename) = vJson

    'End Sub

    'Shared mHttpClient = New HttpClient()

    'Public Async Function SendAllTables() As Task

    '    Const pApiUrl As String = "https://oedf7csjxg.execute-api.ap-southeast-1.amazonaws.com/uat/potatocorner/mpos"
    '    Const pApiKey As String = "Z98xuBjozw8YWVuRsNC1n8C76smKTWe15mzLLJPy"

    '    Try
    '        System.Net.ServicePointManager.SecurityProtocol =
    '            System.Net.SecurityProtocolType.Tls12 Or
    '            System.Net.SecurityProtocolType.Tls11 Or
    '            System.Net.SecurityProtocolType.Tls

    '        Do
    '            mLogFolder = DateTimeStamp()
    '            Dim pDrFirstStore = GetFirstStore()
    '            If pDrFirstStore Is Nothing Then
    '                Return
    '            End If

    '            AddToLog("store_code = " & CStr2(pDrFirstStore("store_code")) & ". tx_date = " & CDate(pDrFirstStore("tx_date")).ToString(cGlobal.FormatDateSql))
    '            ' temp commented during testing 2026-03-31
    '            'If CheckTotals(pDrFirstStore("store_code"), pDrFirstStore("tx_date")) = False Then
    '            '    UpdateDateSent("CacheSalesTableInvoice", pDrFirstStore("store_code"), pDrFirstStore("terminal"), pDrFirstStore("tx_date"), "DateError")
    '            '    Continue Do
    '            'End If

    '            Dim pTables As New Dictionary(Of String, String)
    '            pTables.Add("order", "CacheSalesTableOrder")
    '            pTables.Add("invoice", "CacheSalesTableInvoice")
    '            pTables.Add("payment", "CacheSalesTablePayments")
    '            pTables.Add("financial", "CacheSalesTableFinancial")

    '            Dim pFailed As Boolean
    '            For Each pTable In pTables
    '                Await SendLog(Nothing, eEventType.DataPreparationStarted, pDrFirstStore("store_code"), pTable.Key,
    '                    pDrFirstStore("terminal"), CDate(pDrFirstStore("tx_date"))).ConfigureAwait(False)
    '                Await SendLog(Nothing, eEventType.DataPreparationCompleted, pDrFirstStore("store_code"), pTable.Key,
    '                    pDrFirstStore("terminal"), CDate(pDrFirstStore("tx_date"))).ConfigureAwait(False)

    '                pFailed = Not Await SendTable(pTable.Value, pTable.Key, pDrFirstStore("store_code"), pDrFirstStore("tx_date")).ConfigureAwait(False)
    '                If pFailed Then
    '                    Await SendLog(mLastFailResponse, eEventType.DataSubmitRequestSent, pDrFirstStore("store_code"), pTable.Key,
    '                        pDrFirstStore("terminal"), CDate(pDrFirstStore("tx_date"))).ConfigureAwait(False)
    '                    Await SendLog(mLastFailResponse, eEventType.DataSubmitResponse, pDrFirstStore("store_code"), pTable.Key,
    '                        pDrFirstStore("terminal"), CDate(pDrFirstStore("tx_date"))).ConfigureAwait(False)
    '                Else
    '                    UpdateDateSent(pTable.Value, pDrFirstStore("store_code"), pDrFirstStore("terminal"), pDrFirstStore("tx_date"))
    '                    Await SendLog(mLastResponse, eEventType.DataSubmitRequestSent, pDrFirstStore("store_code"), pTable.Key,
    '                        pDrFirstStore("terminal"), CDate(pDrFirstStore("tx_date"))).ConfigureAwait(False)
    '                    Await SendLog(mLastResponse, eEventType.DataSubmitResponse, pDrFirstStore("store_code"), pTable.Key,
    '                        pDrFirstStore("terminal"), CDate(pDrFirstStore("tx_date"))).ConfigureAwait(False)
    '                End If
    '            Next
    '        Loop

    '        AddToLog("All done.")
    '    Catch ex As Exception
    '        AddToLog(ex.Message)
    '        AppendToLog(ex.Message)
    '    End Try

    'End Function

    'Sub UpdateDateSent(vTable As String, vStoreCode As String, vTerminal As String, vTransactionDate As Date, Optional vFieldName As String = "DateSent")

    '    Dim pIU = New InsertUpdate(vTable)
    '    pIU(vFieldName) = "GETDATE()"
    '    Dim pSql = pIU.GetUpdate("tx_date = " & ReadyDateTime(vTransactionDate) & vbCrLf &
    '        " AND store_code = " & ReadyString(vStoreCode) & vbCrLf &
    '        " AND terminal = " & ReadyString(vTerminal) & vbCrLf &
    '        " AND DateSent IS NULL")
    '    Dim pCount = mParent.ProData.ExecuteSQL(pSql)

    'End Sub

    Function CheckTotals(vStoreCode As String, vTransactionDate As Date) As Boolean

        Dim pDtInvoice = GetTable("CacheSalesTableInvoice", vStoreCode, vTransactionDate)
        Dim pInvoiceGrossAmount = CDec2(pDtInvoice.Compute("SUM(amount_gross)", ""))
        Dim pInvoiceAmountDue = CDec2(pDtInvoice.Compute("SUM(amount_due)", ""))

        Dim pDtOrder = GetTable("CacheSalesTableOrder", vStoreCode, vTransactionDate)
        pDtOrder.Columns.Add("gross", GetType(Decimal), "original_price * quantity")
        Dim pOrderGrossAmount = CDec2(pDtOrder.Compute("SUM(gross)", ""))
        If Math.Abs(pInvoiceGrossAmount - pOrderGrossAmount) > 0.01 Then
            AddToLog("pInvoiceGrossAmount (" & pInvoiceGrossAmount.ToString(cGlobal.FormatAccounting) & ") is not equal to pOrderGrossAmount (" & pOrderGrossAmount.ToString(cGlobal.FormatAccounting) & "). ref: vbUKNEATFAAT!")
            Return False
        End If

        'Dim pDtPayment = GetTable("CacheSalesTablePayments", vStoreCode, vTransactionDate)
        'Dim pPaymentAmountDue = CDec2(pDtPayment.Compute("SUM(amount)", ""))
        'If Math.Abs(pInvoiceAmountDue - pPaymentAmountDue) > 0.01 Then
        '    AddToLog("pInvoiceAmountDue (" & pInvoiceAmountDue.ToString(cGlobal.FormatAccounting) & ") is not equal to pPaymentAmountDue (" & pPaymentAmountDue.ToString(cGlobal.FormatAccounting) & "). ref: vbR2VJPS6VR8!")
        '    Return False
        'End If

        Dim pDtFinancial = GetTable("CacheSalesTableFinancial", vStoreCode, vTransactionDate)
        Dim pFinancialGrossAmount = CDec2(pDtFinancial.Compute("SUM(gross)", ""))
        If Math.Abs(pInvoiceGrossAmount - pFinancialGrossAmount) > 0.01 Then
            AddToLog("pInvoiceGrossAmount (" & pInvoiceGrossAmount.ToString(cGlobal.FormatAccounting) & ") is not equal to pFinancialGrossAmount (" & pFinancialGrossAmount.ToString(cGlobal.FormatAccounting) & "). ref: vbRARZ4E3PPW!")
            Return False
        End If

        'AddToLog("Store Code: " & vStoreCode & vbCrLf &
        '    "Transaction Date: " & vTransactionDate.ToString(cGlobal.FormatDateSql) & vbCrLf &
        '    "Invoices Records: " & pDtInvoice.Rows.Count.ToString(cGlobal.FormatInteger) & vbCrLf &
        '    "Invoice Gross Amount: " & pInvoiceGrossAmount.ToString(cGlobal.FormatAccounting) & vbCrLf &
        '    "Invoice Amount Due: " & pInvoiceAmountDue.ToString(cGlobal.FormatAccounting) & vbCrLf &
        '    "Orders Records: " & pDtOrder.Rows.Count.ToString(cGlobal.FormatInteger) & vbCrLf &
        '    "Orders Gross Amount: " & pOrderGrossAmount.ToString(cGlobal.FormatAccounting) & vbCrLf &
        '    "Payments Records: " & pDtPayment.Rows.Count.ToString(cGlobal.FormatInteger) & vbCrLf &
        '    "Payments Amount Due: " & pPaymentAmountDue.ToString(cGlobal.FormatAccounting) & vbCrLf &
        '    "Financials Records: " & pDtFinancial.Rows.Count.ToString(cGlobal.FormatInteger) & vbCrLf &
        '    "Financials Gross Amount: " & pFinancialGrossAmount.ToString(cGlobal.FormatAccounting))

        AddToLog("Store Code: " & vStoreCode & vbCrLf &
            "Transaction Date: " & vTransactionDate.ToString(cGlobal.FormatDateSql) & vbCrLf &
            "Invoices Records: " & pDtInvoice.Rows.Count.ToString(cGlobal.FormatInteger) & vbCrLf &
            "Invoice Gross Amount: " & pInvoiceGrossAmount.ToString(cGlobal.FormatAccounting) & vbCrLf &
            "Invoice Amount Due: " & pInvoiceAmountDue.ToString(cGlobal.FormatAccounting) & vbCrLf &
            "Orders Records: " & pDtOrder.Rows.Count.ToString(cGlobal.FormatInteger) & vbCrLf &
            "Orders Gross Amount: " & pOrderGrossAmount.ToString(cGlobal.FormatAccounting) & vbCrLf &
            "Financials Records: " & pDtFinancial.Rows.Count.ToString(cGlobal.FormatInteger) & vbCrLf &
            "Financials Gross Amount: " & pFinancialGrossAmount.ToString(cGlobal.FormatAccounting))

        Return True

    End Function

    Function GetTable(vTable As String, vStoreCode As String, vTransactionDate As Date) As DataTable

        Dim pSql = "SELECT * FROM " & vTable &
            " WHERE CONVERT(date, tx_date) = " & ReadyDateTime(vTransactionDate.Date) & vbCrLf &
            " AND store_code = " & ReadyString(vStoreCode)
        AddToLog("GetTable vbBCACHEDAUL! " & vbCrLf & pSql)
        Dim pDT = mParent.ProData.Execute(pSql)
        Return pDT

    End Function

    'Public Async Function SendTable(vTable As String, vName As String, vStoreCode As String, vTransactionDate As Date) As Task(Of Boolean)

    '    Const pApiUrl As String = "https://oedf7csjxg.execute-api.ap-southeast-1.amazonaws.com/uat/potatocorner/mpos"
    '    Const pApiKey As String = "Z98xuBjozw8YWVuRsNC1n8C76smKTWe15mzLLJPy"

    '    Try
    '        System.Net.ServicePointManager.SecurityProtocol =
    '            System.Net.SecurityProtocolType.Tls12 Or
    '            System.Net.SecurityProtocolType.Tls11 Or
    '            System.Net.SecurityProtocolType.Tls

    '        Dim pAttempts = 1
    '        Do
    '            Try
    '                AddToLog("SendTable attempt #" & pAttempts & ". vStoreCode = " & vStoreCode & ". vTransactionDate = " & vTransactionDate.ToString(cGlobal.FormatDateSql))
    '                Dim pDT = GetTable(vTable, vStoreCode, vTransactionDate)
    '                Dim pJsonPayload = CacheSalesTables.SerializeTable(pDT, vName)
    '                AddToLog("pJsonPayload:" & vbCrLf & TrimToLength(pJsonPayload, 10000))
    '                SaveJsonText(pJsonPayload, vName, vStoreCode, vTransactionDate)
    '                'Return True ' temp exit

    '                Dim payload As Byte() = Encoding.UTF8.GetBytes(pJsonPayload)
    '                Dim uploadContent As New ByteArrayContent(payload)

    '                ' 1?? Request presigned URL
    '                Dim apiContent = New StringContent(pJsonPayload, Encoding.UTF8, "application/json")
    '                apiContent.Headers.Add("x-api-key", pApiKey)

    '                Dim apiResponse = Await mHttpClient.PostAsync(pApiUrl, apiContent).ConfigureAwait(False)
    '                Dim apiBody = Await apiResponse.Content.ReadAsStringAsync()
    '                AddToLog(apiBody)
    '                mLastResponse = JsonConvert.DeserializeObject(Of LogInput)(apiBody)
    '                Dim pSuccess = mLastResponse.status?.Trim()?.ToLower() = "success"
    '                If pSuccess Then
    '                    Return True
    '                Else
    '                    pAttempts += 1
    '                    If pAttempts > 3 Then
    '                        mLastFailResponse = mLastResponse
    '                        Return False
    '                    End If
    '                End If
    '            Catch ex As Exception
    '                pAttempts += 1
    '                If pAttempts > 3 Then
    '                    Exit Do
    '                End If
    '            End Try
    '        Loop
    '    Catch ex As Exception
    '        AppendToLog(ex.Message)
    '    End Try

    'End Function

    'Async Function SendLog(vInput As LogInput, vEventType As eEventType, vStoreCode As String, vTable As String, vTerminal As String, vTransactionDate As Date) As Task

    '    'Exit Function ' temp exit

    '    Const pLogUrl As String = "https://m03xqpcs15.execute-api.ap-southeast-1.amazonaws.com/uat/potato-corner/mpos"
    '    Const pLogKey As String = "EBC9LPIhmz4jSGWZjbdl7a7oaqNOekp62J8sOG5W"

    '    Try
    '        System.Net.ServicePointManager.SecurityProtocol =
    '            System.Net.SecurityProtocolType.Tls12 Or
    '            System.Net.SecurityProtocolType.Tls11 Or
    '            System.Net.SecurityProtocolType.Tls

    '        Dim pAttempts = 1
    '        Do
    '            Try
    '                AddToLog("SendLog attempt #" & pAttempts & ". vEventType = " & vEventType.ToString() & ". vStoreCode = " & vStoreCode)
    '                Dim pJsonPayload = ComposeLogPayload(vInput, vEventType, vStoreCode, vTable, vTerminal, vTransactionDate)
    '                Dim apiContent = New StringContent(pJsonPayload, Encoding.UTF8, "application/json")
    '                apiContent.Headers.Add("x-api-key", pLogKey)
    '                Dim apiResponse = Await mHttpClient.PostAsync(pLogUrl, apiContent).ConfigureAwait(False)
    '                Dim apiBody = Await apiResponse.Content.ReadAsStringAsync()
    '                AddToLog("Response = " & apiBody)
    '                Dim pResponseJson = JsonConvert.DeserializeObject(Of LogInput)(apiBody)
    '                Dim pSuccess = pResponseJson.status?.Trim()?.ToLower() = "success"
    '                If pSuccess Then
    '                    Return
    '                Else
    '                    pAttempts += 1
    '                    If pAttempts > 3 Then
    '                        mLastFailResponse = mLastResponse
    '                        Return
    '                    End If
    '                End If
    '            Catch ex As Exception
    '                pAttempts += 1
    '                If pAttempts > 3 Then
    '                    Exit Do
    '                End If
    '            End Try
    '        Loop
    '    Catch ex As Exception
    '        AddToLog(ex.Message)
    '        AppendToLog(ex.Message)
    '    End Try

    'End Function
End Class