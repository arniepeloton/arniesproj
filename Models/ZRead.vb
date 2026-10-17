Public Class ZRead
    ReadOnly mBusiness As Business
    ReadOnly mProData As ProData

    Public Sub New(vProData As ProData)

        If Not vProData Is Nothing Then
            mProData = vProData ' ProData.CreateFromConnectionInfo()
            mBusiness = New Business(mProData)
        End If

        'CacheZReadData()
    End Sub

    Sub CacheZReadData()
        Dim pSql As String
        Dim dtDeviceFowst As DataTable

        pSql = "Select * from LibraryDeviceFowst"

        dtDeviceFowst = mBusiness.ProData.Execute(pSql)

        For i As Integer = 0 To dtDeviceFowst.Rows.Count - 1
            CacheZRead(CStr2(dtDeviceFowst.Rows(i).Item("DeviceID")), CStr2(dtDeviceFowst.Rows(i).Item("FOWSTID")))
        Next

    End Sub

    Function CacheZRead(DeviceID As String, FOWSTID As String) As Boolean
        Dim pSql As String
        Dim zRead As New ReportPOS
        Dim dtZReadData As DataTable
        Dim dtLastZReadDate As String
        Dim ZReadDate As Date

        pSql = "Select top 1 SalesDate from TransZReadCache " & vbCrLf &
            " where DeviceID = " & ReadyString(DeviceID) & vbCrLf &
            " AND FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
            " ORDER BY SalesDate Desc"

        dtLastZReadDate = CStr2(mBusiness.ProData.ExecuteScalar(pSql))

        If Not dtLastZReadDate = "" Then

            ZReadDate = CDate(dtLastZReadDate)

            'While DateDiff(DateInterval.Day, ZReadDate, Date.Now.AddDays(-1))

            If DateDiff(DateInterval.Day, ZReadDate, Date.Now.AddDays(-1)) > 0 Then
                dtZReadData = zRead.getSalesZReportSummary("By Date", CStr2(ZReadDate), FOWSTID, DeviceID)
                SaveZReadCache(dtZReadData, FOWSTID, DeviceID, CStr2(ZReadDate))

                'dtZReadData = zRead.getSalesZReportSummary("By Date", CStr2(Date.Now.AddDays(-1)), FOWSTID, DeviceID)
                'SaveZReadCache(dtZReadData, FOWSTID, DeviceID, Date.Now.AddDays(-1))

                'ZReadDate.AddDays(1)
            End If
            
            'End While
        Else

            dtZReadData = zRead.getSalesZReportSummary("By Date", Date.Now.AddDays(-1), FOWSTID, DeviceID)
            SaveZReadCache(dtZReadData, FOWSTID, DeviceID, CStr2(Date.Now.AddDays(-1)))

        End If
        
    End Function

    Function SaveZReadCache(dtZReadData As DataTable, FOWSTID As String, DeviceID As String, ZReadDate As String) As Boolean
        Dim pSql As String
        Dim pIU As New InsertUpdate("TransZReadCache")

        If dtZReadData.Rows.Count > 0 Then

            pIU("SalesDate") = ReadyString(ZReadDate)
            pIU("DeviceID") = ReadyString(DeviceID)
            pIU("FOWSTID") = ReadyString(FOWSTID)
            pIU("BegBal") = CDec2(dtZReadData.Rows(0).Item("BegBal"))
            pIU("PrevGrossSales") = CDec2(dtZReadData.Rows(0).Item("PrevGrossSales"))
            pIU("PrevTotalDiscount") = CDec2(dtZReadData.Rows(0).Item("PrevTotalDiscount"))
            pIU("PrevSalesReturnRefund") = CDec2(dtZReadData.Rows(0).Item("PrevSalesReturnRefund"))
            pIU("PrevSalesReturnExchange") = CDec2(dtZReadData.Rows(0).Item("PrevSalesReturnExchange"))
            pIU("PrevTotalVAT") = CDec2(dtZReadData.Rows(0).Item("PrevTotalVAT"))
            pIU("PrevServiceCharge") = CDec2(dtZReadData.Rows(0).Item("PrevServiceCharge"))
            pIU("PrevChargeAmount") = CDec2(dtZReadData.Rows(0).Item("PrevChargeAmount"))
            pIU("CurGrossSales") = CDec2(dtZReadData.Rows(0).Item("CurGrossSales"))
            pIU("CurTotalDiscount") = CDec2(dtZReadData.Rows(0).Item("CurTotalDiscount"))
            pIU("CurSalesReturnRefund") = CDec2(dtZReadData.Rows(0).Item("CurSalesReturnRefund"))
            pIU("CurSalesReturnExchange") = CDec2(dtZReadData.Rows(0).Item("CurSalesReturnExchange"))
            pIU("CurTotalVATSales") = CDec2(dtZReadData.Rows(0).Item("CurTotalVATSales"))
            pIU("CurTotalVAT") = CDec2(dtZReadData.Rows(0).Item("CurTotalVAT"))
            pIU("CurTotalVATSalesSalesReturn") = CDec2(dtZReadData.Rows(0).Item("CurTotalVATSalesSalesReturn"))
            pIU("CurTotalVATAmountSalesReturn") = CDec2(dtZReadData.Rows(0).Item("CurTotalVATAmountSalesReturn"))
            pIU("CurLocalTax") = CDec2(dtZReadData.Rows(0).Item("CurLocalTax"))
            pIU("CurServiceCharge") = CDec2(dtZReadData.Rows(0).Item("CurServiceCharge"))
            pIU("CurNoTAXSales") = CDec2(dtZReadData.Rows(0).Item("CurNoTAXSales"))
            pIU("CurVATExemptSales") = CDec2(dtZReadData.Rows(0).Item("CurVATExemptSales"))
            pIU("CurZeroRatedVATSales") = CDec2(dtZReadData.Rows(0).Item("CurZeroRatedVATSales"))
            pIU("CurOthers") = CDec2(dtZReadData.Rows(0).Item("CurOthers"))
            pIU("CashAmount") = CDec2(dtZReadData.Rows(0).Item("CashAmount"))
            pIU("DebitCardAmount") = CDec2(dtZReadData.Rows(0).Item("DebitCardAmount"))
            pIU("CreditCardAmount") = CDec2(dtZReadData.Rows(0).Item("CreditCardAmount"))
            pIU("CheckPaymentAmount") = CDec2(dtZReadData.Rows(0).Item("CheckPaymentAmount"))
            pIU("SRVAmount") = CDec2(dtZReadData.Rows(0).Item("SRVAmount"))
            pIU("ChargeAmount") = CDec2(dtZReadData.Rows(0).Item("ChargeAmount"))
            pIU("SMACAmount") = CDec2(dtZReadData.Rows(0).Item("SMACAmount"))
            pIU("EPlusAmount") = CDec2(dtZReadData.Rows(0).Item("EPlusAmount"))
            pIU("OnCreditAmount") = CDec2(dtZReadData.Rows(0).Item("OnCreditAmount"))
            pIU("VoucherAmount") = CDec2(dtZReadData.Rows(0).Item("VoucherAmount"))
            pIU("CurBegInvoiceNo") = CInt2(dtZReadData.Rows(0).Item("CurBegInvoiceNo"))
            pIU("CurEndInvoiceNo") = CInt2(dtZReadData.Rows(0).Item("CurEndInvoiceNo"))
            pIU("CurTransCountInv") = CInt2(dtZReadData.Rows(0).Item("CurTransCountInv"))
            pIU("CurTransInvItemCount") = CInt2(dtZReadData.Rows(0).Item("CurTransInvItemCount"))
            pIU("CurTransCountMemoReturnExchange") = CInt2(dtZReadData.Rows(0).Item("CurTransCountMemoReturnExchange"))
            pIU("CurSalesReturnExchangeItemCount") = CInt2(dtZReadData.Rows(0).Item("CurSalesReturnExchangeItemCount"))
            pIU("CurTransCountMemoRefund") = CInt2(dtZReadData.Rows(0).Item("CurTransCountMemoRefund"))
            pIU("CurSalesRefundItemCount") = CInt2(dtZReadData.Rows(0).Item("CurSalesRefundItemCount"))
            pIU("CurTermNum") = CInt2(dtZReadData.Rows(0).Item("CurTermNum"))

            pSql = pIU.GetInsert()

            mBusiness.ProData.ExecuteSQL(pSql)
        End If
    End Function
End Class
