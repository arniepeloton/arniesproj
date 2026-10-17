define([
//    'common',
    //GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
    'promise!/Scripts3/Browser/LibraryFowst.js?crc=',
], function () {

    CreateStringObject('Reports.Robinsons', null, function (vParams) {
        var mOut = {}; 

        mOut.Resend = function(vParams) {
            vParams = $.extend({
                FowstID: null,
                DeviceID: null,
                SalesDate: null
            }, vParams);

            var pSalesDate = ParseDate(vParams.SalesDate);
            if (pSalesDate == null) {
                throw new Error('SalesDate must be a valid date. js9FKJZ8WL6D!');
            }
            pSalesDate = $.format.date(pSalesDate, 'yyyy-MM-dd');

            var pGrossSalesNetOfVATExemptionsSql = 'case when [quantity]=(0) then (0) when (((ifnull([vatablesale],(0))+ifnull([vatexemptsale],(0)))+ifnull([zeroratedsale],(0)))+ifnull([nonvatsale],(0)))=(0) then ifnull([subtotal],(0)) else ((ifnull([vatablesale],(0))+ifnull([vatexemptsale],(0)))+ifnull([zeroratedsale],(0)))+ifnull([nonvatsale],(0)) end';

            pSql = ' SELECT ' + ReadyString(pSalesDate) + ' AS [TransDate]' + '\n' + 
                ' ,  IFNULL(devf.BegBalance,0)  AS [BegBal]' + '\n' + 
                ' ,  IFNULL((SELECT SUM(' + pGrossSalesNetOfVATExemptionsSql + ') ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> \'RF\' ' + '\n' + 
                '  AND date(inv.InvoiceDate) < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0)' + '\n' + 
                '  AS [PrevGrossSales]' + '\n' + // included in ExportToCsvZip or MobizExportController
                ' ,  IFNULL((SELECT SUM(custpay.DiscountAmount) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID ' + '\n' + 
                '  JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID' + '\n' + 
                '  AND LibraryDiscountSettings.DiscountType NOT IN (\'Person With Disability Discount\', \'Senior Citizen Discount\')' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0 And custpay.ReceiptType <> \'RF\' AND IFNULL(disc.NoOfSeniorCitizens, 0) =  0 ' + '\n' + 
                '  AND date(inv.InvoiceDate) < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [PrevTotalDiscount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(inv.SCDiscount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [PrevTotalSCDiscount]' + '\n' + 
                ' , IFNULL((SELECT SUM(srledger.GrossRefundAmount) ' + '\n' + 
                '  FROM TransSalesReturnLedger srledger ' + '\n' + 
                '  INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' + '\n' + 
                '  WHERE date(memo.LastUpdated) < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND memo.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND memo.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [PrevSalesReturnRefund]' + '\n' + 
                ' , IFNULL((SELECT SUM(srledger.GrossSrvAmount) ' + '\n' + 
                '  FROM TransSalesReturnLedger srledger ' + '\n' + 
                '  INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' + '\n' + 
                '  WHERE date(memo.LastUpdated) < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND memo.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND memo.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [PrevSalesReturnExchange]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(invitem.TaxAmount,0) - IFNULL(invitem.DiscountTax,0)) ' + '\n' + 
                '  FROM TransInvoiceItems invitem ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) ' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0 ' + '\n' + 
                ' AND date(inv.InvoiceDate) < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) ' + '\n' + 
                '  - ' + '\n' + 
                ' IFNULL((SELECT SUM(sr.TaxAmount) ' + '\n' + 
                '  FROM TransMemoSalesReturn sr ' + '\n' + 
                '  INNER JOIN TransMemo memo ON (sr.MemoID = memo.ID) ' + '\n' + 
                '  WHERE date(memo.LastUpdated) < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND memo.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND memo.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [PrevTotalVAT]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(srledger.Fees, 0) + IFNULL(srledger.RefundFees, 0)) ' + '\n' + 
                '  FROM TransSalesReturnLedger srledger ' + '\n' + 
                '  INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' + '\n' + 
                '  WHERE date(memo.LastUpdated)  < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND memo.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND memo.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) +' + '\n' + 
                ' IFNULL((SELECT SUM(IFNULL(charge.ChargeAmount, 0)) ' + '\n' + 
                '  FROM TransInvoice inv ' + '\n' + 
                '  INNER JOIN TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) ' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0 ' + '\n' + 
                '  AND date(inv.InvoiceDate)  < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID)' + '\n' + 
                ' ,0) AS [PrevServiceCharge]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(custpay.ChargeAmount,0))  FROM TransCustomerPaymentInvoices custpayinv  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)   ' + '\n' + 
                ' WHERE date(inv.InvoiceDate) < ' + ReadyString(pSalesDate) + '\n' + 
                ' AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID) ,0) AS [PrevChargeAmount]' + '\n' + 
                ' ,  IFNULL((SELECT SUM(inv.ForegoneVAT)  FROM TransCustomerPaymentInvoices custpayinv  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)   WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> \'RF\'  ' + '\n' + 
                ' AND date(inv.InvoiceDate) < ' + ReadyString(pSalesDate) + '\n' + 
                ' AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID) ,0) AS [PrevVATExempt]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(custpay.ChargeAmount,0))  FROM TransCustomerPaymentInvoices custpayinv  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)   ' + '\n' + 
                ' WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID) ,0) AS [ChargeAmount]' + '\n' + 
                ' ,  IFNULL((SELECT SUM(' + pGrossSalesNetOfVATExemptionsSql + ') ' + '\n' + 
                '  FROM TransInvoiceItems invitem ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) ' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0 ' + '\n' + 
                '  AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [CurGrossSales]' + '\n' + // included in ExportToCsvZip or MobizExportController
                ' ,  IFNULL((SELECT SUM(' + pGrossSalesNetOfVATExemptionsSql + ') ' + '\n' + 
                '  FROM TransInvoiceItems invitem ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) ' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0 ' + '\n' + 
                ' AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND IFNULL(inv.SCDiscount, 0) = 0 ' + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [CurGrossSalesExcludingSenior]' + '\n' + 
                ' , IFNULL(tblDiscount.Sum, 0) AS [CurTotalDiscount]' + '\n' + 
                ' , IFNULL(tblDiscount.Count, 0) AS [CurDiscountCount]' + '\n' + 
                ' , IFNULL(tblScDiscount.Sum, 0) AS [CurTotalScDiscount]' + '\n' + 
                ' , IFNULL(tblScDiscount.Count, 0) AS [CurDiscountScCount]' + '\n' + 
                ' , IFNULL(tblRefund.Sum, 0) AS [CurSalesReturnRefund]' + '\n' + 
                ' , IFNULL(tblRefund.Count, 0) AS [CurSalesReturnRefundCount]' + '\n' + 
                ' , IFNULL(tblRefund2.Sum, 0) AS [CurSalesReturnRefund2]' + '\n' + 
                ' , IFNULL(tblRefund2.Count, 0) AS [CurSalesReturnRefundCount2]' + '\n' + 
                ' , IFNULL(tblInvoice.ErrorCorrectAmount, 0) AS [ErrorCorrectAmount]' + '\n' + 
                ' , IFNULL(tblInvoice.ErrorCorrectCount, 0) AS [ErrorCorrectCount]' + '\n' + 
                ' , IFNULL((SELECT SUM(srledger.GrossSrvAmount) ' + '\n' + 
                '  FROM TransSalesReturnLedger srledger ' + '\n' + 
                '  INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' + '\n' + 
                '  WHERE date(memo.LastUpdated) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND memo.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND memo.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [CurSalesReturnExchange]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(invitem.TaxAmount,0)- IFNULL(invitem.DiscountTax,0)) ' + '\n' + 
                '  FROM --TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  --INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  --INNER JOIN ' + '\n' + 
                '  TransInvoice inv --ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0 --AND custpay.ReceiptType <> \'RF\' ' + '\n' + 
                ' AND (CAST(inv.SCDiscount AS INTEGER)=0 OR inv.SCDiscount IS NULL) -- strftime(\n' + 
                ' AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [CurTotalVAT]' + '\n';
            pSql += ' , IFNULL((SELECT MIN(inv.InvoiceNumber) ' + '\n' + 
                '  FROM TransInvoice inv ' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0 ' + '\n' + 
                '  AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID)' + '\n' + 
                ' ,0) AS [CurBegInvoiceNo]' + '\n' + 
                ' , IFNULL((SELECT MAX(inv.InvoiceNumber) ' + '\n' + 
                '  FROM TransInvoice inv ' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0 ' + '\n' + 
                '  AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID)' + '\n' + 
                ' ,0) AS [CurEndInvoiceNo]' + '\n' + 
                ' , IFNULL((SELECT COUNT(inv.InvoiceNumber) ' + '\n' + 
                '  FROM TransInvoice inv ' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0 ' + '\n' + 
                '  AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID)' + '\n' + 
                ' ,0) AS [CurTransCountInv]' + '\n' + 
                ' , IFNULL((SELECT DISTINCT COUNT(memo.MemoNumber)  FROM TransSalesReturnLedger srledger  INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)  ' + '\n' + 
                ' WHERE date(memo.LastUpdated) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID  AND srledger.DestinationPaymentID IS NULL  AND srledger.SrvNum > 0) ,0) AS [CurTransCountMemoReturnExchange]' + '\n' + 
                ' , IFNULL((SELECT DISTINCT COUNT(memo.MemoNumber)  FROM TransSalesReturnLedger srledger  INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)  ' + '\n' + 
                ' WHERE date(memo.LastUpdated) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID  AND srledger.DestinationPaymentID IS NULL  AND srledger.RefundNum > 0) ,0) AS [CurTransCountMemoRefund]' + '\n' + 
                ' , IFNULL((SELECT COUNT(invitem.ProductID)  FROM TransCustomerPaymentInvoices custpayinv  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)   INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID)  WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> \'RF\'  ' + '\n' + 
                ' AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID),0) AS [CurTransInvItemCount]' + '\n' + 
                ' , IFNULL((SELECT COUNT(memoret.ProductID)  FROM TransSalesReturnLedger srledger  INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)  INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID)  ' + '\n' + 
                ' WHERE date(memo.LastUpdated) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID  AND srledger.DestinationPaymentID IS NULL  AND srledger.SrvNum > 0) ,0) AS [CurSalesReturnExchangeItemCount]' + '\n' + 
                ' , IFNULL((SELECT COUNT(memoret.ProductID)  FROM TransSalesReturnLedger srledger  INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)  INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID)  ' + '\n' + 
                ' WHERE date(memo.LastUpdated) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID  AND srledger.DestinationPaymentID IS NULL  AND srledger.RefundNum > 0) ,0) AS [CurSalesRefundItemCount]' + '\n' + 
                ' ,  IFNULL((SELECT SUM(inv.Vatable)  FROM TransCustomerPaymentInvoices custpayinv  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)   WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> \'RF\' AND inv.ForegoneVAT>0 ' + '\n' + 
                ' AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID) ,0) AS [CurTotalVatablePortionOfSCDTransaction]' + '\n' + 
                ' , IFNULL((SELECT COUNT(memo.MemoNumber) ' + '\n' + 
                '  FROM TransMemo memo ' + '\n' + 
                '  WHERE date(memo.LastUpdated) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND memo.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND memo.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [CurTransCountMemo]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(charge.TaxAmount, 0)) ' + '\n' + 
                '  FROM TransInvoice inv ' + '\n' + 
                '  INNER JOIN TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) ' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0 ' + '\n' + 
                ' AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID)' + '\n' + 
                ' ,0) AS [CurLocalTax]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(srledger.Fees, 0) + IFNULL(srledger.RefundFees, 0)) ' + '\n' + 
                '  FROM TransSalesReturnLedger srledger ' + '\n' + 
                '  INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' + '\n' + 
                '  WHERE date(memo.LastUpdated) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND memo.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND memo.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) +' + '\n' + 
                ' IFNULL((SELECT SUM(IFNULL(charge.ChargeAmount, 0)) ' + '\n' + 
                '  FROM TransInvoice inv ' + '\n' + 
                '  INNER JOIN TransInvoiceCharges charge ON (inv.ID = charge.InvoiceID) ' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0 ' + '\n' + 
                ' AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID)' + '\n' + 
                ' ,0) AS [CurServiceCharge]' + '\n' + 
                ' , IFNULL((SELECT SUM(invitem.NonVATSale)  FROM TransCustomerPaymentInvoices custpayinv  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)   INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID)  ' + '\n' + 
                ' WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID) ,0) AS [CurNoTAXSales]' + '\n' + 
                ' , IFNULL((SELECT SUM(inv.VATExempt - inv.SCDiscount)  FROM TransCustomerPaymentInvoices custpayinv  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)   WHERE (inv.InvoiceNumber > 0 AND custpay.ReceiptType <> \'RF\' ) ' + '\n' + 
                ' AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID) ,0) AS [CurVATExemptSales]' + '\n' + 
                ' ,  IFNULL((SELECT SUM(inv.ForegoneVAT)  FROM TransCustomerPaymentInvoices custpayinv  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)   WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> \'RF\'  ' + '\n' + 
                ' AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID) ,0) AS [CurVATExempt]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(custpay.CashAmount,0) - IFNULL(custpay.CustomerChangeCash,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [CashAmount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(custpay.CashAmount,0) - IFNULL(custpay.CustomerChangeCash,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [PrevCashAmount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(custpay.DebitCardAmount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [DebitCardAmount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(custpay.DebitCardAmount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [PrevDebitCardAmount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(custpay.CreditCardAmount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [CreditCardAmount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(custpay.CreditCardAmount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [PrevCreditCardAmount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(custpay.CheckPaymentAmount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [CheckPaymentAmount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(custpay.CheckPaymentAmount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [PrevCheckPaymentAmount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(other.Amount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID ' + '\n' + 
                '  AND other.PaymentType = \'PotatoCornerGC\' ' + '\n' + 
                ' ),0) AS [CurPotatoCornerGC]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(other.Amount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID ' + '\n' + 
                '  AND other.PaymentType = \'PotatoCornerGC\' ' + '\n' + 
                ' ),0) AS [PrevPotatoCornerGC]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(other.Amount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE other.PaymentType = \'Discount Voucher / On Line Deals\' ' + '\n' + 
                '  AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [VoucherAmount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(other.Amount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE other.PaymentType = \'Discount Voucher / On Line Deals\' ' + '\n' + 
                '  AND date(inv.InvoiceDate) < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [PrevVoucherAmount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(other.Amount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID ' + '\n' + 
                ' ),0) AS [CurOthers]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(other.Amount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) < ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID ' + '\n' + 
                ' ),0) AS [PrevOthers]' + '\n' + 
                ' , dev.DeviceNo AS [CurTermNum]' + '\n' + 
                ' , dev.ID AS [DeviceID]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(inv.SCDiscount,0)) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [TotalSCDiscount]' + '\n' + // included in ExportToCsvZip or MobizExportController
                ' , IFNULL((SELECT SUM(inv.TotalNumberOfCustomer) ' + '\n' + 
                '  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> \'RF\' ' + '\n' + 
                '  AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID)' + '\n' + 
                ' ,0) AS [CurCustomerCount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(custpay.SRVAmount,0))  FROM TransCustomerPaymentInvoices custpayinv  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)   ' + '\n' + 
                ' WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID) ,0) AS [SRVAmount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(custpay.OnCreditAmount,0))  FROM TransCustomerPaymentInvoices custpayinv  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID)  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)   ' + '\n' + 
                ' WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID) ,0) AS [OnCreditAmount]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(invitem.TaxSales,0)- IFNULL(invitem.DiscountSales,0)) ' + '\n' + 
                '  FROM --TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '  --INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '  --INNER JOIN ' + '\n' + 
                '  TransInvoice inv --ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '  INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' + '\n' + 
                '  WHERE inv.InvoiceNumber > 0  --AND custpay.ReceiptType <> \'RF\'' + '\n' + 
                ' AND (CAST(inv.SCDiscount AS INTEGER) = 0 OR inv.SCDiscount IS NULL)  -- strftime(\n' + 
                ' AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                '  AND inv.DeviceID = devf.DeviceID) ' + '\n' + 
                ' ,0) AS [CurTotalVATSales]' + '\n' + 
                ' , IFNULL((SELECT SUM(IFNULL(memosr.TotalAmount,0) - IFNULL(memosr.TaxAmount,0))  FROM TransMemoSalesReturn memosr  INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID)  ' + '\n' + 
                ' WHERE date(memo.LastUpdated) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID) ,0) AS [CurTotalVATSalesSalesReturn]' + '\n' + 
                ' , IFNULL((SELECT SUM(memosr.TaxAmount)  FROM TransMemoSalesReturn memosr  INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID)  ' + '\n' + 
                ' WHERE date(memo.LastUpdated) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID) ,0) AS [CurTotalVATAmountSalesReturn]' + '\n' + 
                ' , devf.ZReadCount AS [ZCount]' + '\n' + 
                ' , IFNULL(tblReprint.Sum, 0) AS [ReprintedAmount]' + '\n' + 
                ' , IFNULL(tblReprint.Count, 0) AS [ReprintedCount]' + '\n' + 
                ' , LibraryOrganizationFOWST.RobinsonTenantId AS [RobinsonTenantId]' + '\n' + 
                ' , LibraryOrganizationFOWST.RobinsonPosTerminalNum AS [RobinsonPosTerminalNum]' + '\n' + 
                ' , IFNULL(tblInvoiceWithScAndPwdDiscount.VATExempt, 0) AS [SeniorAndPwdVatExempt]' + '\n' + 
                ' , IFNULL(tblInvoiceWithScAndPwdDiscount.SCDiscount, 0) AS [SeniorAndPwdDiscount]' + '\n' + 
                ' , IFNULL(tblInvoiceWithScOnlyDiscount.VATExempt, 0) AS [SeniorOnlyVatExempt]' + '\n' + 
                ' , IFNULL(tblInvoiceWithScOnlyDiscount.SCDiscount, 0) AS [SeniorOnlyDiscount]' + '\n' + 
                ' , IFNULL(tblInvoiceWithPwdOnlyDiscount.VATExempt, 0) AS [PwdOnlyVatExempt]' + '\n' + 
                ' , IFNULL(tblInvoiceWithPwdOnlyDiscount.SCDiscount, 0) AS [PwdOnlyDiscount]' + '\n' + 
                ' , IFNULL(tblCreditCardPayment.Amount, 0) AS [CreditCardPaymentAmount]' + '\n' + 
                ' , IFNULL(tblCreditCardPayment.Count, 0) AS [CreditCardPaymentCount]' + '\n' + 
                ' , IFNULL(tblCreditCardPayment.CreditCardVat, 0) AS [CreditCardPaymentVat]' + '\n';
            pSql += '   -- main FROM' + '\n' + 
                '  FROM LibraryDeviceFowst devf ' + '\n' + 
                '  INNER JOIN LibraryDevice dev ON (devf.DeviceID = dev.ID) ' + '\n' + 
                '  JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = devf.FowstID ' + '\n';
                
            // included in ExportToCsvZip or MobizExportController
            pSql += ' LEFT JOIN (' + '\n' + 
                '   SELECT SUM(custpay.DiscountAmount) AS Sum ' + '\n' + 
                '   ,COUNT(DISTINCT custpayinv.InvoiceID) AS Count ' + '\n' + 
                '   ,inv.FOWSTID ' + '\n' + 
                '   ,inv.DeviceID' + '\n' + 
                '   FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '   INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '   INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '   INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID ' + '\n' + 
                '   JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID' + '\n' + 
                '   AND LibraryDiscountSettings.DiscountType NOT IN (\'Person With Disability Discount\', \'Senior Citizen Discount\')' + '\n' + 
                '   WHERE inv.InvoiceNumber > 0 ' + '\n' + 
                '   AND custpay.ReceiptType <> \'RF\' ' + '\n' + 
                '   AND IFNULL(disc.NoOfSeniorCitizens, 0) =  0 ' + '\n' + 
                '   AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '   GROUP BY inv.FOWSTID ' + '\n' + 
                '   ,inv.DeviceID' + '\n' + 
                ') tblDiscount ON tblDiscount.FOWSTID = devf.FowstID ' + '\n' + 
                'AND tblDiscount.DeviceID = devf.DeviceID\n';

                //'  OUTER APPLY(' + '\n' + 
                //' SELECT SUM(custpay.DiscountAmount) AS Sum ' + '\n' + 
                //' ,COUNT(DISTINCT custpayinv.InvoiceID) AS Count ' + '\n' + 
                //'  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                //'  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                //'  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                //'  INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID ' + '\n' + 
                //'  JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID' + '\n' + 
                //'  AND LibraryDiscountSettings.DiscountType NOT IN (\'Person With Disability Discount\', \'Senior Citizen Discount\')' + '\n' + 
                //'  WHERE inv.InvoiceNumber > 0 And custpay.ReceiptType <> \'RF\' AND IFNULL(disc.NoOfSeniorCitizens, 0) =  0 ' + '\n' + 
                //'  AND CONVERT(date, inv.InvoiceDate) BETWEEN \'07/31/2024\' AND \'07/31/2024\'' + '\n' + 
                //'  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                //'  AND inv.DeviceID = devf.DeviceID' + '\n' + 
                //' ) tblDiscount' + '\n;';

            // all included in ExportToCsvZip or MobizExportController
            pSql += ' LEFT JOIN (' + '\n' + 
                '   SELECT SUM(custpay.DiscountAmount) AS Sum ' + '\n' + 
                '   ,COUNT(DISTINCT custpayinv.InvoiceID) AS Count ' + '\n' + 
                '   ,inv.FOWSTID ' + '\n' + 
                '   ,inv.DeviceID ' + '\n' + 
                '   FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                '   INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                '   INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                '   INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID ' + '\n' + 
                '   JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID' + '\n' + 
                '   WHERE inv.InvoiceNumber > 0 ' + '\n' + 
                '   AND custpay.ReceiptType <> \'RF\' ' + '\n' + 
                '   AND IFNULL(disc.NoOfSeniorCitizens, 0) > 0' + '\n' + 
                '   AND LibraryDiscountSettings.DiscountType = \'Senior Citizen Discount\' ' + '\n' + 
                '   AND date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                '   GROUP BY inv.FOWSTID ' + '\n' + 
                '   ,inv.DeviceID ' + '\n' + 
                ') tblScDiscount ON tblScDiscount.FOWSTID = devf.FowstID ' + '\n' + 
                'AND tblScDiscount.DeviceID = devf.DeviceID\n';

                //'  OUTER APPLY(' + '\n' + 
                //' SELECT SUM(custpay.DiscountAmount) AS Sum ' + '\n' + 
                //' ,COUNT(DISTINCT custpayinv.InvoiceID) AS Count ' + '\n' + 
                //'  FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                //'  INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                //'  INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                //'  INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID ' + '\n' + 
                //'  JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID' + '\n' + 
                //'  WHERE inv.InvoiceNumber > 0 And custpay.ReceiptType <> \'RF\' AND IFNULL(disc.NoOfSeniorCitizens, 0) > 0' + '\n' + 
                //'  AND LibraryDiscountSettings.DiscountType = \'Senior Citizen Discount\' ' + '\n' + 
                //'  AND CONVERT(date, inv.InvoiceDate) BETWEEN \'07/31/2024\' AND \'07/31/2024\'' + '\n' + 
                //'  AND inv.FOWSTID = devf.FowstID ' + '\n' + 
                //'  AND inv.DeviceID = devf.DeviceID' + '\n' + 
                //' ) tblScDiscount' + '\n';

            // included in ExportToCsvZip or MobizExportController
            pSql += ' LEFT JOIN (' + '\n' + 
                '   SELECT SUM(srledger.GrossRefundAmount) AS Sum' + '\n' + 
                '   ,COUNT(DISTINCT memo.id) AS Count' + '\n' + 
                '   ,memo.FOWSTID ' + '\n' + 
                '   ,memo.DeviceID ' + '\n' + 
                '   FROM TransSalesReturnLedger srledger' + '\n' + 
                '   JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)' + '\n' + 
                '   WHERE date(memo.LastUpdated) = ' + ReadyString(pSalesDate) + '\n' + 
                '   AND srledger.GrossRefundAmount > 0' + '\n' + 
                '   GROUP BY memo.FOWSTID ' + '\n' + 
                '   ,memo.DeviceID ' + '\n' + 
                ') tblRefund ON tblRefund.FOWSTID = devf.FowstID' + '\n' + 
                'AND tblRefund.DeviceID = devf.DeviceID\n';

                //'  OUTER APPLY (' + '\n' + 
                //'  	SELECT SUM(srledger.GrossRefundAmount) AS Sum' + '\n' + 
                //'    ,COUNT(DISTINCT memo.id) AS Count' + '\n' + 
                //'   	FROM TransSalesReturnLedger srledger' + '\n' + 
                //'   	JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID)' + '\n' + 
                //'    WHERE CONVERT(date, memo.LastUpdated) BETWEEN \'07/31/2024\' AND \'07/31/2024\'' + '\n' + 
                //'   	AND memo.FOWSTID = devf.FowstID' + '\n' + 
                //'   	AND memo.DeviceID = devf.DeviceID' + '\n' + 
                //'  	AND srledger.GrossRefundAmount > 0' + '\n' + 
                //'  ) tblRefund' + '\n';

            // included in ExportToCsvZip or MobizExportController (for testing)
            pSql += ' LEFT JOIN (' + '\n' + 
                ' SELECT SUM(srledger.GrossRefundAmount) AS Sum' + '\n' + 
                ' ,COUNT(DISTINCT srledger.id) AS Count' + '\n' + 
                ' ,TransInvoice.FOWSTID' + '\n' + 
                ' ,TransInvoice.DeviceID' + '\n' + 
                ' FROM TransSalesReturnLedger srledger' + '\n' + 
                ' JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = srledger.SourceMemoID' + '\n' + 
                ' JOIN TransInvoice ON TransInvoice.ID = TransMemoSalesReturn.InvoiceID' + '\n' + 
                ' WHERE date(TransInvoice.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND srledger.GrossRefundAmount > 0' + '\n' + 
                ' GROUP BY  TransInvoice.FOWSTID' + '\n' + 
                ' ,TransInvoice.DeviceID' + '\n' + 
                ') tblRefund2 ON tblRefund2 .FOWSTID = devf.FowstID' + '\n' + 
                ' AND tblRefund2 .DeviceID = devf.DeviceID\n';

            //pSql += '  OUTER APPLY (' + '\n' + 
            //    '  	SELECT SUM(srledger.GrossRefundAmount) AS Sum' + '\n' + 
            //    '    ,COUNT(DISTINCT srledger.id) AS Count' + '\n' + 
            //    '   	FROM TransSalesReturnLedger srledger' + '\n' + 
            //    '   	JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = srledger.SourceMemoID' + '\n' + 
            //    '   	JOIN TransInvoice ON TransInvoice.ID = TransMemoSalesReturn.InvoiceID' + '\n' + 
            //    '    WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN \'07/31/2024\' AND \'07/31/2024\'' + '\n' + 
            //    '   	AND TransInvoice.FOWSTID = devf.FowstID' + '\n' + 
            //    '   	AND TransInvoice.DeviceID = devf.DeviceID' + '\n' + 
            //    '  	AND srledger.GrossRefundAmount > 0' + '\n' + 
            //    '  ) tblRefund2' + '\n';

            // included in ExportToCsvZip or MobizExportController
            pSql += ' LEFT JOIN (' + '\n' + 
                ' SELECT SUM(TransInvoice.TotalAmount) AS Sum' + '\n' + 
                ' ,COUNT(DISTINCT TransInvoice.ID) AS Count' + '\n' + 
                ' ,transInvoice.FOWSTID ' + '\n' + 
                ' ,TransInvoice.DeviceID' + '\n' + 
                ' FROM TransInvoice' + '\n' + 
                ' WHERE date(TransInvoice.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND TransInvoice.PrintCount > 1' + '\n' + 
                ' GROUP BY transInvoice.FOWSTID ' + '\n' + 
                ' ,TransInvoice.DeviceID' + '\n' + 
                ' ) tblReprint ON tblReprint.FOWSTID = devf.FowstID' + '\n' + 
                ' AND tblReprint.DeviceID = devf.DeviceID\n';

            //pSql += '  OUTER APPLY (' + '\n' + 
            //    '  	SELECT SUM(TransInvoice.TotalAmount) AS Sum' + '\n' + 
            //    '    	,COUNT(DISTINCT TransInvoice.ID) AS Count' + '\n' + 
            //    '   	FROM TransInvoice' + '\n' + 
            //    '   	WHERE date(TransInvoice.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
            //    '   	AND TransInvoice.FOWSTID = devf.FowstID' + '\n' + 
            //    '   	AND TransInvoice.DeviceID = devf.DeviceID' + '\n' + 
            //    '  	AND TransInvoice.PrintCount > 1' + '\n' + 
            //    '  ) tblReprint' + '\n';

            pSql += ' LEFT JOIN ' + '\n' + 
                ' (SELECT SUM(TransInvoice.ErrorCorrectAmount) AS ErrorCorrectAmount' + '\n' + 
                ' ,SUM(TransInvoice.ErrorCorrectCount) AS ErrorCorrectCount' + '\n' + 
                ' ,transInvoice.FOWSTID ' + '\n' + 
                ' ,TransInvoice.DeviceID' + '\n' + 
                ' FROM TransInvoice' + '\n' + 
                ' WHERE date(TransInvoice.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' GROUP BY transInvoice.FOWSTID ' + '\n' + 
                ' ,TransInvoice.DeviceID' + '\n' + 
                ' ) tblInvoice ON tblInvoice.FOWSTID = devf.FowstID' + '\n' + 
                ' AND tblInvoice.DeviceID = devf.DeviceID' + '\n';
                
            // included in ExportToCsvZip or MobizExportController
            pSql += ' LEFT JOIN (' + '\n' + 
                ' SELECT SUM(TransInvoice.VATExempt) AS VATExempt' + '\n' + 
                ' ,SUM(TransInvoice.SCDiscount) AS SCDiscount' + '\n' + 
                ' ,TransInvoice.FOWSTID ' + '\n' + 
                ' ,TransInvoice.DeviceID' + '\n' + 
                ' FROM TransInvoice' + '\n' + 
                ' WHERE date(TransInvoice.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND TransInvoice.SCDiscount > 0' + '\n' + 
                ' GROUP BY TransInvoice.FOWSTID ' + '\n' + 
                ' ,TransInvoice.DeviceID' + '\n' + 
                ') tblInvoiceWithScAndPwdDiscount ON tblInvoiceWithScAndPwdDiscount.FOWSTID = devf.FowstID' + '\n' + 
                ' AND tblInvoiceWithScAndPwdDiscount.DeviceID = devf.DeviceID\n';

            //pSql += ' /*' + '\n' + 
            //    '   OUTER APPLY (' + '\n' + 
            //    '   	SELECT SUM(TransInvoice.VATExempt) AS VATExempt' + '\n' + 
            //    '    ,SUM(TransInvoice.SCDiscount) AS SCDiscount' + '\n' + 
            //    '    FROM TransInvoice' + '\n' + 
            //    '   	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN \'07/31/2024\' AND \'07/31/2024\'' + '\n' + 
            //    '    AND TransInvoice.FOWSTID = devf.FowstID' + '\n' + 
            //    '    AND TransInvoice.DeviceID = devf.DeviceID' + '\n' + 
            //    '    AND TransInvoice.SCDiscount > 0' + '\n' + 
            //    '   ) tblInvoiceWithScAndPwdDiscount' + '\n' + 

            pSql += ' LEFT JOIN (' + '\n' + 
                ' SELECT SUM(TransInvoice.VATExempt) AS VATExempt' + '\n' + 
                ' ,SUM(TransInvoice.SCDiscount) AS SCDiscount' + '\n' + 
                ' ,TransInvoice.FOWSTID ' + '\n' + 
                ' ,TransInvoice.DeviceID ' + '\n' + 
                ' FROM TransInvoice' + '\n' + 
                ' INNER JOIN TransInvoiceDiscount ON TransInvoiceDiscount.InvoiceID = TransInvoice.ID ' + '\n' + 
                ' JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = TransInvoiceDiscount.DiscountTypeID' + '\n' + 
                ' WHERE date(TransInvoice.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND LibraryDiscountSettings.DiscountType = \'Senior Citizen Discount\' ' + '\n' + 
                ' AND TransInvoice.SCDiscount > 0' + '\n' + 
                ' GROUP BY TransInvoice.FOWSTID ' + '\n' + 
                ' ,TransInvoice.DeviceID ' + '\n' + 
                ') tblInvoiceWithScOnlyDiscount ON tblInvoiceWithScOnlyDiscount.FOWSTID = devf.FowstID' + '\n' + 
                ' AND tblInvoiceWithScOnlyDiscount.DeviceID = devf.DeviceID\n';

                //'   OUTER APPLY (' + '\n' + 
                //'   	SELECT SUM(TransInvoice.VATExempt) AS VATExempt' + '\n' + 
                //'    ,SUM(TransInvoice.SCDiscount) AS SCDiscount' + '\n' + 
                //'    FROM TransInvoice' + '\n' + 
                //'  INNER JOIN TransInvoiceDiscount ON TransInvoiceDiscount.InvoiceID = TransInvoice.ID ' + '\n' + 
                //'  JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = TransInvoiceDiscount.DiscountTypeID' + '\n' + 
                //'   	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN \'07/31/2024\' AND \'07/31/2024\'' + '\n' + 
                //'  AND LibraryDiscountSettings.DiscountType = \'Senior Citizen Discount\' ' + '\n' + 
                //'    AND TransInvoice.FOWSTID = devf.FowstID' + '\n' + 
                //'    AND TransInvoice.DeviceID = devf.DeviceID' + '\n' + 
                //'    AND TransInvoice.SCDiscount > 0' + '\n' + 
                //'   ) tblInvoiceWithScOnlyDiscount' + '\n' + 

            // all included in ExportToCsvZip or MobizExportController
            pSql += ' LEFT JOIN (' + '\n' + 
                ' SELECT SUM(TransInvoice.VATExempt) AS VATExempt' + '\n' + 
                ' ,SUM(TransInvoice.SCDiscount) AS SCDiscount' + '\n' + 
                ' ,TransInvoice.FOWSTID ' + '\n' + 
                ' ,TransInvoice.DeviceID ' + '\n' + 
                ' FROM TransInvoice' + '\n' + 
                ' INNER JOIN TransInvoiceDiscount ON TransInvoiceDiscount.InvoiceID = TransInvoice.ID ' + '\n' + 
                ' JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = TransInvoiceDiscount.DiscountTypeID' + '\n' + 
                ' WHERE date(TransInvoice.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND LibraryDiscountSettings.DiscountType = \'Person With Disability Discount\' ' + '\n' + 
                ' AND TransInvoice.SCDiscount > 0' + '\n' + 
                ' GROUP BY TransInvoice.FOWSTID ' + '\n' + 
                ' ,TransInvoice.DeviceID ' + '\n' + 
                ' ) tblInvoiceWithPwdOnlyDiscount ON tblInvoiceWithPwdOnlyDiscount.FOWSTID = devf.FowstID' + '\n' + 
                ' AND tblInvoiceWithPwdOnlyDiscount .DeviceID = devf.DeviceID\n';

                //'   OUTER APPLY (' + '\n' + 
                //'   	SELECT SUM(TransInvoice.VATExempt) AS VATExempt' + '\n' + 
                //'    ,SUM(TransInvoice.SCDiscount) AS SCDiscount' + '\n' + 
                //'    FROM TransInvoice' + '\n' + 
                //'  INNER JOIN TransInvoiceDiscount ON TransInvoiceDiscount.InvoiceID = TransInvoice.ID ' + '\n' + 
                //'  JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = TransInvoiceDiscount.DiscountTypeID' + '\n' + 
                //'   	WHERE CONVERT(date, TransInvoice.InvoiceDate) BETWEEN \'07/31/2024\' AND \'07/31/2024\'' + '\n' + 
                //'  AND LibraryDiscountSettings.DiscountType = \'Person With Disability Discount\' ' + '\n' + 
                //'    AND TransInvoice.FOWSTID = devf.FowstID' + '\n' + 
                //'    AND TransInvoice.DeviceID = devf.DeviceID' + '\n' + 
                //'    AND TransInvoice.SCDiscount > 0' + '\n' + 
                //'   ) tblInvoiceWithPwdOnlyDiscount' + '\n' + 

            pSql += '   LEFT JOIN (' + '\n' + 
                ' SELECT SUM(custpay.CreditCardAmount) AS Amount' + '\n' + 
                ' ,COUNT(DISTINCT custpay.id) AS Count' + '\n' + 
                ' ,SUM(custpay.CreditCardVat) AS CreditCardVat' + '\n' + 
                ' ,inv.FOWSTID' + '\n' + 
                ' ,inv.DeviceID ' + '\n' + 
                ' FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
                ' JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
                ' JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                ' WHERE date(inv.InvoiceDate) = ' + ReadyString(pSalesDate) + '\n' + 
                ' AND custpay.CreditCardAmount > 0' + '\n' + 
                ' GROUP BY inv.FOWSTID' + '\n' + 
                ' ,inv.DeviceID ' + '\n' + 
                ' ) tblCreditCardPayment ON	tblCreditCardPayment.FOWSTID = devf.FowstID ' + '\n' + 
                ' AND tblCreditCardPayment.DeviceID  = devf.DeviceID\n';

            //pSql += ' /*' + '\n' + 
            //    '  OUTER APPLY (' + '\n' + 
            //    '  	SELECT SUM(custpay.CreditCardAmount) AS Amount' + '\n' + 
            //    '    ,COUNT(DISTINCT custpay.id) AS Count' + '\n' + 
            //    '    ,SUM(custpay.CreditCardVat) AS CreditCardVat' + '\n' + 
            //    '  	FROM TransCustomerPaymentInvoices custpayinv ' + '\n' + 
            //    '  	JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' + '\n' + 
            //    '  	JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
            //    '   	WHERE CONVERT(date, inv.InvoiceDate) BETWEEN \'07/31/2024\' AND \'07/31/2024\'' + '\n' + 
            //    '    AND custpay.CreditCardAmount > 0' + '\n' + 
            //    '  	AND inv.FOWSTID = devf.FowstID ' + '\n' + 
            //    '    AND inv.DeviceID = devf.DeviceID' + '\n' + 
            //    ' ) tblCreditCardPayment' + '\n' + 
            //    ' */' + '\n' + 

            pSql += ' WHERE devf.FowstID = ' + ReadyString(vParams.FowstID) + '\n' + 
                '  AND devf.DeviceID = ' + ReadyString(vParams.DeviceID) + '\n' + 
                '  /* vbA7XJ7CTDKZ vb92AQQE34NK */   '
            Transfers.WriteFile('robinsons resend 13.txt', null, pSql);
            var pDT = Sql.Execute(pSql);
            return pDT;
        };

        function GetAmount(cover, vSalesDate, vFowstID, vDeviceID, type) {
            var pSalesDate = ParseDate(vSalesDate);
            if (pSalesDate == null) {
                throw new Error('SalesDate must be a valid date. js9FKJZ8WL6D!');
            }
            pSalesDate = $.format.date(pSalesDate, 'yyyy-MM-dd');

            var pSql = "SELECT IFNULL(SUM(";

            if (type === "discount") {
                pSql += "inv.SCDiscount";
            } else if (type === "debit") {
                pSql += "custpay.DebitCardAmount";
            } else if (type === "credit") {
                pSql += "custpay.CreditCardAmount";
            } else {
                pSql += "ledger.Amount";
            }

            pSql += "), 0) as Total " +
                    "FROM TransSalesReturnLedger as ledger " +
                    "INNER JOIN TransMemo as memo ON memo.ID = ledger.SourceMemoID " +
                    "INNER JOIN TransCustomerPaymentInvoices as cuspayinv ON cuspayinv.InvoiceID = ledger.SourceInvoiceID " +
                    "INNER JOIN TransCustomerPayment custpay ON cuspayinv.CustomerPaymentID = custpay.ID ";

            if (type === "cash") {
                pSql += "AND custpay.CashAmount > 0 ";
            }

            if (type === "otherpayment") {
                pSql += "INNER JOIN TransCustomerPaymentOther other ON other.CustomerPaymentID = custpay.ID ";
            }

            pSql += "INNER JOIN TransInvoice inv ON cuspayinv.InvoiceID = inv.ID " +
                    "WHERE ledger.FowstID = " + ReadyString(vFowstID) + " " +
                    "AND ledger.DeviceID = " + ReadyString(vDeviceID) + " " +
                    "AND IFNULL(RefundNum, 0) > 0 ";

            if (cover === "today") {
                pSql += "AND DATE(ledger.LastUpdated) = " + ReadyString(pSalesDate) + " ";
            } else if (cover === "prev") {
                pSql += "AND DATE(ledger.LastUpdated) < " + ReadyString(pSalesDate) + " ";
            } else if (cover === "period") {
                pSql += "AND DATE(ledger.LastUpdated) = " + ReadyString(pSalesDate);
            }

            var pOut = Sql.ExecuteScalar(pSql);
            pOut = CDec2(pOut);
            pOut = RoundOff(pOut);

            return pOut;
        }

        function EndOfDayCounter(vSalesDate, vFowstID, vDeviceID, vCover, vFirstEodDate) {
            // PARAMS:
            //  vCover = today | prev
            var pSalesDate = new Date(vSalesDate);
            if (!IsDate(pSalesDate))
                throw new Error('vSalesDate must be a valid date');
            var pFirstEodDate = new Date(vFirstEodDate);
            if (!IsDate(pFirstEodDate))
                throw new Error('vFirstEodDate must be a valid date');
            var pDateDiff = pSalesDate - pFirstEodDate;
            pDateDiff = pDateDiff / 1000 / 60 / 60 / 24;

            if (vCover == 'today')
                return pDateDiff + 1;
            else if (vCover == 'prev')
                return pDateDiff;
            else
                throw new Error('Invalid vCover. ref: js8Q3GPE3KXD!');

            //var pSalesDate = $.format.date(new Date(vSalesDate), 'yyyy-MM-dd');

            //if (vCover == 'today')
            //    vCover = '=';
            //else if (vCover == 'prev')
            //    vCover = '<';
            //else
            //    throw new Error('Invalid vCover. ref: js8Q3GPE3KXD!');

            //var pSql = 'SELECT EndOfDayCounter\n' +
            //    'FROM TenantMonitoringLogs WHERE date(SalesDate) ' + vCover + ' ' + ReadyString(pSalesDate) + '\n' +
            //    'AND FOWSTID = ' + ReadyString(vFowstID) + '\n' +
            //    'AND DeviceID = ' + ReadyString(vDeviceID) + '\n' +
            //    'ORDER BY SalesDate DESC\n' + 
            //    'LIMIT 1';
            //var pOut = CDec2(Sql.ExecuteScalar(pSql));
            //return pOut;
        }

        function GetFirstEodDate(vParams) {
            //// BEGIN for debugging only 2024_09_26__11_56_57_0982
            //vParams.Callback({
            //    Results: new Date()
            //});
            //return;
            //// END for debugging only 2024_09_26__11_56_57_0982

            vParams = $.extend({
                FowstID: null,
                DeviceID: null,
                Callback: function(vDate){}
            }, vParams);

            var pData = Transfers.ReadFile('FirstEodDate.txt');
            try {
                pData = JSON.parse(pData);
            } catch (e) {
                pData = {};
            }

            var pPath = ReadyGuid(vParams.FowstID) + '.' + ReadyGuid(vParams.DeviceID);
            var pDate = GetStringObject(pPath, pData);
            if (IsDate(pDate)) {
                vParams.Callback({
                    Results: pDate
                });
                return;
            }

            //var pSql = 'SELECT SalesDate\n ' + 
            //    ' FROM TenantMonitoringLogs\n' +
            //    ' WHERE FowstID = ' + ReadyString(vParams.FowstID) + '\n' +
            //    ' AND DeviceID = ' + ReadyString(vParams.DeviceID) + '\n' +
            //    ' AND EndOfDayCounter = 1' + '\n' + 
            //    ' ORDER BY SalesDate';
            //pDate = Sql.ExecuteScalar(pSql);
            //if (IsDate(pDate)) {
            //    pDate = $.format.date(new Date(pDate), 'yyyy-MM-dd');
            //    CreateStringObject(pPath, pData, pDate);
            //    Transfers.WriteFile('FirstEodDate.txt', null, JSON.stringify(pData, null, 4));
            //    vParams.Callback({
            //        Results: pDate
            //    });
            //    return;
            //}
            
            AjaxSend({
                url: '/SalesMenu/GetFirstEodDate',
                data: {
                    FowstID: vParams.FowstID,
                    DeviceID: vParams.DeviceID
                },
                success: function(vData) {
                    if (IsDate(vData.Results)) {
                        CreateStringObject(pPath, pData, vData.Results);
                        Transfers.WriteFile('FirstEodDate.txt', null, JSON.stringify(pData, null, 4));
                    }
                    vParams.Callback({
                        Results: pDate
                    });
                },
                error: function() {
                    vParams.Callback({
                        Error: 'Local data for the first EOD date is unavailable, and the server connection failed. Please check your network or try again later.'
                    });
                }
            });

        } // END function GetFirstEodDate(vParams) 

        mOut.CreateRlcFile = function(vParams) {
            vParams = $.extend({
                FowstID: null,
                DeviceID: null,
                SalesDate: null,
                FileStatus: null,
                Filename: null,
                BatchNo: null,
                DontUpload: false,
                FirstEodDate: null,
                Callback: function(){}
            }, vParams);

            if (CStr2(vParams.Filename) != '') {
                var pFileText = Transfers.ReadFile('RLC Files/' + vParams.Filename);
                if (CStr2(pFileText).length < 500)
                    vParams.Filename = null;
            }

            var pSalesDate = $.format.date(new Date(vParams.SalesDate), 'yyyy-MM-dd');
            if (CStr2(vParams.Filename) == '') {
                if (vParams.FirstEodDate == null) {
                    GetFirstEodDate({
                        FowstID: vParams.FowstID,
                        DeviceID: vParams.DeviceID,
                        Callback: function(vData) {
                            if (vData.Error) {
                                NewMessageBox(vData.Error);
                                return;
                            }
                            vParams.FirstEodDate = vData.Results;
                            mOut.CreateRlcFile(vParams);
                        }
                    });
                    return;
                }

                var pRequireDownload = RequireDownload({
                    FowstID: vParams.FowstID,
                    DeviceID: vParams.DeviceID,
                    SalesDate: vParams.SalesDate
                });
                if (pRequireDownload) {
                    CreateZip({
                        FowstID: vParams.FowstID,
                        DeviceID: vParams.DeviceID,
                        SalesDate: vParams.SalesDate,
                        Callback: function() {
                            mOut.CreateRlcFile(vParams);
                        }
                    });
                    return;
                }

                var pFileText = this.CreateRlcText(vParams);
                vParams.BatchNo = CInt2(vParams.BatchNo);
                if (vParams.BatchNo < 1)
                    vParams.BatchNo = 1;

                var pDestinationFilename = vParams.RobinsonTenantId;
                if (pDestinationFilename.length < 4)
                    pDestinationFilename = 'eror'; // eor = intentional spelling
                else
                    pDestinationFilename = pDestinationFilename.substring(pDestinationFilename.length - 4, pDestinationFilename.length);
                pDestinationFilename += $.format.date(new Date(vParams.SalesDate), 'MMdd') + '.01' + vParams.BatchNo;
                Transfers.WriteFile('RLC Files/' + pDestinationFilename, null, pFileText);
                vParams.Filename = pDestinationFilename;
                var pIU = new InsertUpdate('TenantMonitoringLogs');
                pIU.Add('FileStatus', '\'created\'');
                pIU.Add('Filename', ReadyString('a' + pDestinationFilename)); // prefix with 'a' to prevent saving as number (ex: 3.43208e+07)
                var pSql = pIU.GetUpdate('FOWSTID = ' + ReadyString(vParams.FowstID) + '\n' +
                    ' AND DeviceID = ' + ReadyString(vParams.DeviceID) + '\n' +
                    ' AND date(SalesDate) = ' + ReadyString(pSalesDate));
                Sql.ExecuteSql(pSql);
                mOut.CreateRlcFile(vParams);
            } else if (vParams.DontUpload == false)
                UploadToFtp(vParams);
            else
                vParams.Callback();
        };

        function UploadToFtp(vParams) {
            vParams = $.extend({
                FowstID: null,
                DeviceID: null,
                SalesDate: null,
                FileStatus: null,
                Filename: null,
                BatchNo: null,
                Callback: function(){}
            }, vParams);

            var pBatchNo = vParams.BatchNo;
            if (!IsNumeric(pBatchNo))
                throw new Error('vParams.BatchNo must be an integer. ref: jsUMK3J6L3AH!');
            pBatchNo = CInt2(pBatchNo);

            AjaxSend({
                url: 'Home/GetDate',
                success: function(vData) {
                    if (mHasDisconnection
                        && !m1stDisconnection) {

                        m1stDisconnection = true;
                        NewMessageBox('Trying to send unsent files.', 'Sending...', null, {
                            Close: function() {
                                //setTimeout(function() {
                                //    vParams.Callback();
                                //}, 5000);
                            }
                        });
                    }

                    var pSalesDate = $.format.date(new Date(vParams.SalesDate), 'yyyy-MM-dd');
                    Android.AddLog('Uploading to FTP: ' + vParams.Filename);
                    Transfers.Upload({
                        SourceFilename: 'RLC Files/' + vParams.Filename,
                        Destination: 'MallLessorMegaworld/UploadToRobinsonFtp',
                        Data: {
                            OrgID: sessionStorage.OrgID,
                            FowstID: vParams.FowstID
                        },
                        Success: function(vData) {
                            if (CStr2(vData.Results).trim().length > 0) {
                                try {
                                    var pResult = JSON.parse(vData.Results);
                                    if (pResult.Error) {
                                        if (mHasDisconnection)
                                            setTimeout(function() {
                                                vParams.Callback();
                                            }, 5000);
                                        else
                                            NewMessageBox('Sales file is not sent to RLC server. Please contact your POS vendor. ' + pResult.Error, 'Server Error', null, {
                                                Close: function() {
                                                    setTimeout(function() {
                                                        vParams.Callback();
                                                    }, 5000);
                                                }
                                            });
                                        mHasDisconnection = true;
                                    } else if (pResult.Result == 'ok') {
                                        var pFilenameWithoutBatchNo = vParams.Filename.substring(0, vParams.Filename.length - pBatchNo.toString().length);
                                        var pFilename = 'a' + pFilenameWithoutBatchNo + (pBatchNo + 0); // prefix with "a" due to sqlite
                                        console.log('jsQ4RMMYXHW6: Transfers.Upload ok: ' + vData.SourceFilename);
                                        var pIU = new InsertUpdate('TenantMonitoringLogs');
                                        pIU.Add('Filename', ReadyString(pFilename));
                                        pIU.Add('FileStatus', '\'ftp-send\'');
                                        pIU.Add('BatchNo', pBatchNo + 1);
                                        var pSql = pIU.GetUpdate('FOWSTID = ' + ReadyString(vParams.FowstID) + '\n' +
                                            ' AND DeviceID = ' + ReadyString(vParams.DeviceID) + '\n' +
                                            ' AND date(SalesDate) = ' + ReadyString(pSalesDate));
                                        Sql.ExecuteSql(pSql);
                                        if (!mHasDisconnection
                                            && (mPending == 1))
                                            NewMessageBox('Sales file successfully sent to RLC server.', 'Success', null, {
                                                Close: function() {
                                                    vParams.Callback();
                                                }
                                            });
                                        else
                                            vParams.Callback();
                                    }
                                } catch (e) {
                                    NewMessageBox('Unexpected result: ' + vData.Results, 'Fail', null, {
                                        Close: function() {
                                            setTimeout(function() {
                                                vParams.Callback();
                                            }, 5000);
                                        }
                                    });
                                }   
                            } else {
                                if (mHasDisconnection)
                                    setTimeout(function() {
                                        vParams.Callback();
                                    }, 5000);
                                else
                                    NewMessageBox('Sales file is not sent to RLC server. Please contact your POS vendor.', 'Server Error', null, {
                                        Close: function() {
                                            setTimeout(function() {
                                                vParams.Callback();
                                            }, 5000);
                                        }
                                    });
                                mHasDisconnection = true;
                            }
                        },
                        Error: function(vData) {
                            console.log('Error: Upload');
                            if (mHasDisconnection)
                                setTimeout(function() {
                                    vParams.Callback();
                                }, 5000);
                            else
                                NewMessageBox('Sales file is not sent to RLC server. Please contact your POS vendor.', 'Server Error', null, {
                                    Close: function() {
                                        setTimeout(function() {
                                            vParams.Callback();
                                        }, 5000);
                                    }
                                });
                            mHasDisconnection = true;
                        }
                    });
                }, // ajax success: function(vData) {
                error: function(vError) {
                    if (mHasDisconnection)
                        setTimeout(function() {
                            vParams.Callback();
                        }, 5000);
                    else
                        NewMessageBox('Sales file is not sent to RLC server. Please contact your POS vendor.', 'Server Error', null, {
                            Close: function() {
                                setTimeout(function() {
                                    vParams.Callback();
                                }, 5000);
                            }
                        });
                    mHasDisconnection = true;
                }
            });
        }

        mOut.CreateRlcText = function(vParams) {
            var pParams = $.extend({
                FowstID: null,
                DeviceID: null,
                SalesDate: null,
                RobinsonTenantId: null, // this is an output parameter
                BatchNo: null // this is an output parameter
            }, vParams);

            var pDtFowsts = OfflineBrowser.LibraryFowst.GetData({
                ID: pParams.FowstID,
                IsThirdPartyOrganization: true,
                ThirdPartyOrganizationID: sessionStorage.OrgID
            });
            if (pDtFowsts.Rows.length == 0)
                return;
            if (pDtFowsts.Rows.length != 1)
                throw new Error('Fowst count must be 1. ref: jsKUGH449ME7!');
            var pDrFowst = pDtFowsts.Rows[0];
            if (!CBool(pDrFowst.IsFOWSTInsideAMall))
                return;
            if (pDrFowst.FOWSTMallName != 'Robinson')
                return;
            vParams.RobinsonTenantId = pDrFowst.RobinsonTenantId;

            var pDT = mOut.Resend(pParams);
            if (pDT.Rows.length == 0) {
                console.warn('no data. ref: jsZSKDH2NYVF!');
                return null;
            }
            var pDR = pDT.Rows[0];

            var pSql = '  SELECT * -- SalesDate, CurBegInvoiceNo, CurEndInvoiceNo, DeviceID, FOWSTID' + '\n' + 
                '  FROM cache.cachezread' + '\n' + 
                '  WHERE FOWSTID = ' + ReadyString(vParams.FowstID) + '\n' + 
                '  AND DeviceID = ' + ReadyString(vParams.DeviceID) + '\n' + 
                '  AND date(SalesDate) = ' + ReadyString($.format.date(new Date(pSalesDate), 'yyyy-MM-dd'))  + '\n' + 
                '  limit 1';
            var pDrCacheZread = Sql.ExecuteRow(pSql);
            if ($.isEmptyObject(pDrCacheZread)) {
                Business.ZRead.GenerateZeroSalesData(pSalesDate, vParams.FowstID, vParams.DeviceID);
                pDrCacheZread = Sql.ExecuteRow(pSql);
            }
            pDrCacheZread = pDrCacheZread || {};
            pDR.PrevGrossSales = pDrCacheZread.PrevGrossSales;
            pDR.PrevTotalSCDiscount = pDrCacheZread.PrevSCDiscount;
            pDR.PrevTotalDiscount = pDrCacheZread.PrevTotalDiscount;
            pDR.PrevSalesReturnRefund = pDrCacheZread.PrevSalesReturnRefund;
            pDR.PrevSalesReturnExchange = pDrCacheZread.PrevSalesReturnExchange;
            pDR.PrevVATExempt = pDrCacheZread.PrevVATExempt;
            pDR.PrevServiceCharge = pDrCacheZread.PrevServiceCharge;

            var pOtherDiscount = RoundOff(CDec2(pDR.CurTotalDiscount)); // all included in ExportToCsvZip or MobizExportController
            var pSalesRefund2 = CDec2(pDR.CurSalesReturnRefund2); // all included in ExportToCsvZip or MobizExportController
            var pRlc03 = CDec2(pDR.CurGrossSalesExcludingSenior) + CDec2(pDR.SeniorAndPwdVatExempt) - pSalesRefund2; // all included in ExportToCsvZip or MobizExportController
            var pRlc11 = CDec2(pDR.CurTotalScDiscount); // all included in ExportToCsvZip or MobizExportController
            var pRlc24 = CDec2(pDR.SeniorAndPwdDiscount) / 0.2 * 0.8; // all included in ExportToCsvZip or MobizExportController
            var pRlc27 = CDec2(pDR.PwdOnlyDiscount); // all included in ExportToCsvZip or MobizExportController
            var pRlc04 = (pRlc03 - pRlc11 - pRlc24 - pRlc27) / 1.12 * 0.12; // all included in ExportToCsvZip or MobizExportController
            var pEndOfDayCounterPrev = EndOfDayCounter(pParams.SalesDate, pParams.FowstID, pParams.DeviceID, 'prev', pParams.FirstEodDate); // all included in ExportToCsvZip or MobizExportController
            var pEndOfDayCounterToday = EndOfDayCounter(pParams.SalesDate, pParams.FowstID, pParams.DeviceID, 'today', pParams.FirstEodDate); // all included in ExportToCsvZip or MobizExportController
            var pPrevNetSales = CDec2(pDR.PrevGrossSales) - (CDec2(pDR.PrevTotalSCDiscount) + CDec2(pDR.PrevTotalDiscount)) - CDec2(pDR.PrevSalesReturnRefund) - CDec2(pDR.PrevSalesReturnExchange) - CDec2(pDR.PrevVATExempt) + CDec2(pDR.PrevServiceCharge); // in tenant monitoring logs
            pPrevNetSales = RoundOff(pPrevNetSales); // all included in ExportToCsvZip or MobizExportController
            var pNetSales = CDec2(pDR.CurGrossSales) - (CDec2(pDR.TotalSCDiscount) + CDec2(pDR.CurTotalDiscount)) - CDec2(pDR.CurSalesReturnRefund) - CDec2(pDR.CurSalesReturnExchange) - CDec2(pDR.CurVATExempt) + CDec2(pDR.CurServiceCharge); // included in ExportToCsvZip or MobizExportController
            pNetSales = RoundOff(pNetSales);

            var pDiscountRefund = GetAmount("today", pParams.SalesDate, pParams.FowstID, pParams.DeviceID, "discount");

            var pRlcText = '01' + PadTrimText(pDrFowst.RobinsonTenantId);
            pRlcText += '02' + PadTrimText(1);
            pRlcText += '03' + PadTrimText($.format.number(pRlc03, '0.00'));
            pRlcText += '04' + PadTrimText($.format.number(pRlc04, '0.00'));
            pRlcText += '05' + PadTrimText('0.00');
            pRlcText += '06' + PadTrimText('0');
            pRlcText += '07' + PadTrimText($.format.number(pOtherDiscount - pDiscountRefund, '0.00'));
            pRlcText += '08' + PadTrimText(CDec2(pDR.CurDiscountCount)); // 
            pRlcText += '09' + PadTrimText($.format.number(RoundOff(pDR.CurSalesReturnRefund), '0.00'));
            pRlcText += '10' + PadTrimText(CDec2(pDR.CurSalesReturnRefundCount));
            pRlcText += '11' + PadTrimText($.format.number(pRlc11, '0.00')); //
            pRlcText += '12' + PadTrimText(CDec2(pDR.CurDiscountScCount)); //
            pRlcText += '13' + PadTrimText($.format.number(CDec2(pDR.CurServiceCharge), '0.00')); // 
            pRlcText += '14' + PadTrimText(pEndOfDayCounterPrev); //
            pRlcText += '15' + PadTrimText($.format.number(pPrevNetSales, '0.00'));
            pRlcText += '16' + PadTrimText(pEndOfDayCounterToday);
            pRlcText += '17' + PadTrimText($.format.number(pPrevNetSales + pNetSales, '0.00'));
            pRlcText += '18' + PadTrimText($.format.date(new Date(pParams.SalesDate), 'MM/dd/yyyy'));
            pRlcText += '19' + PadTrimText('0.00');
            pRlcText += '20' + PadTrimText('0.00');
            pRlcText += '21' + PadTrimText('0.00');
            pRlcText += '22' + PadTrimText($.format.number(CDec2(pDR.CreditCardPaymentAmount), '0.00'));
            pRlcText += '23' + PadTrimText($.format.number(CDec2(pDR.CreditCardPaymentAmount) / 1.12 * 0.12, '0.00')); //
            pRlcText += '24' + PadTrimText($.format.number(pRlc24, '0.00'));
            pRlcText += '25' + PadTrimText('0.00');
            pRlcText += '26' + PadTrimText('0.00');
            pRlcText += '27' + PadTrimText($.format.number(pRlc27, '0.00'));
            pRlcText += '28' + PadTrimText('0.00');
            pRlcText += '29' + PadTrimText($.format.number(CDec2(pDR.ReprintedAmount), '0.00')); //
            pRlcText += '30' + PadTrimText(CDec2(pDR.ReprintedCount));

            pSql = 'SELECT TenantMonitoringLogs.BatchNo\n' + 
                ' FROM TenantMonitoringLogs\n' +
                ' WHERE FowstID = ' + ReadyString(vParams.FowstID) + '\n' +
                ' AND DeviceID = ' + ReadyString(vParams.DeviceID) + '\n' + 
                ' AND SalesDate = ' + ReadyString($.format.date(new Date(pSalesDate), 'yyyy-MM-dd'));
            vParams.BatchNo = Sql.ExecuteScalar(pSql);

            return pRlcText;
        };

        function PadTrimText(vText) {
            var pOut = '0'.repeat(16) + vText;
            pOut = pOut.substring(pOut.length - 16, pOut.length) + '\n';
            return pOut;
        }
        
        mOut.UnsentLogs = function() {
            var pSql = ' SELECT TenantMonitoringLogs.*' + '\n' + 
                ' ,CASE IFNULL(TenantMonitoringLogs.FileStatus, \'\')' + '\n' + 
                ' WHEN \'\' THEN 1' + '\n' + 
                ' WHEN \'created\' THEN 2' + '\n' + 
                ' END AS FileStatus2' + '\n' + 
                ',SUBSTR(filename, 2) AS Filename2' + '\n' + 
                ' FROM TenantMonitoringLogs' + '\n' + 
                ' JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = TenantMonitoringLogs.FowstID' + '\n' + 
                ' AND LibraryOrganizationFOWST.IsFOWSTInsideAMall = 1' + '\n' + 
                ' AND LibraryOrganizationFOWST.FOWSTMallName = \'Robinson\'' + '\n' + 
                ' WHERE TenantMonitoringLogs.Mall = \'Robinson\'' + '\n' + 
                ' AND IFNULL(TenantMonitoringLogs.FileStatus, \'\') IN (\'created\', \'\')' + '\n' + 
                ' ORDER BY TenantMonitoringLogs.SalesDate'; // dont order by FileStatus2

            var pDtUnsents = Sql.Execute(pSql);
            return pDtUnsents;
        };

        mOut.SetUnsent = function(vIDs) {
            var pIDs = '';

            for(var p = 0; p <= vIDs.length - 1; p++) {
                if (pIDs != '')
                    pIDs += ', ';
                pIDs += ReadyString(vIDs[p]);
            }

            var pIU = new InsertUpdate('TenantMonitoringLogs');
            pIU.Add('FileStatus', ReadyString('created'));
            pIU.Add('Filename', 'null');
            var pSql = pIU.GetUpdate('ID IN (' + pIDs + ')');
            Sql.ExecuteSql(pSql);
        }; // END mOut.SetUnsent

        var mHasUnsent = false;
        var mPending = 0;
        var mHasDisconnection = false;
        var m1stDisconnection = false;
        mOut.SendUnsentLogs = function(vParams) {
            vParams = $.extend({
                ResetMessages: false,
                Callback: function(){}
            }, vParams);

            if (mPending > 0) {
                if (vParams.ResetMessages) {
                    mHasDisconnection = false;
                    m1stDisconnection = false;
                }
                vParams.Callback();
                return;
            }

            var pDtUnsents = mOut.UnsentLogs();
            mPending = pDtUnsents.Rows.length;
            if (mPending > 0) {
                var pDR = pDtUnsents.Rows[0];
                mHasUnsent = true;
                pDR.BatchNo = CInt2(pDR.BatchNo);
                pDR.BatchNo = Math.max(pDR.BatchNo, 1);

                mOut.CreateRlcFile({
                    FowstID: pDR.FOWSTID,
                    DeviceID: pDR.DeviceID,
                    SalesDate: pDR.SalesDate,
                    FileStatus: pDR.FileStatus,
                    BatchNo: pDR.BatchNo,
                    Filename: pDR.Filename2,
                    Callback: function() {
                        mPending = 0;
                        mOut.SendUnsentLogs(vParams);
                    }
                });
            } else {
                if (mHasUnsent
                    && mHasDisconnection) {
                    mHasUnsent = false;
                    mHasDisconnection = false;
                    m1stDisconnection = false;
                    NewMessageBox('Trying to send unsent files…successful.', 'Success', null, {
                        Close: function() {
                            setTimeout(function() {
                                vParams.Callback();
                            }, 5000);
                        }
                    });
                } else
                    vParams.Callback();
            }
        };
        
        function RequireDownload(vParams) {
            vParams = $.extend({
                FowstID: null,
                DeviceID: null,
                SalesDate: null
            }, vParams);

            pSalesDate = $.format.date(new Date(vParams.SalesDate), 'yyyy-MM-dd');

            var pSql = ' SELECT SalesDate, CurBegInvoiceNo, CurEndInvoiceNo, DeviceID, FOWSTID' + '\n' + 
                ' FROM cache.cachezread' + '\n' + 
                ' WHERE FOWSTID = ' + ReadyString(vParams.FowstID) + '\n' +
                ' AND DeviceID = ' + ReadyString(vParams.DeviceID) + '\n' +
                ' AND date(SalesDate) = ' + ReadyString(pSalesDate) + '\n' +
                ' limit 10';
            var pDR = Sql.ExecuteRow(pSql);
            if (pDR == null) 
                return false;
            if ((CInt2(pDR.CurBegInvoiceNo) == 0)
                && (CInt2(pDR.CurEndInvoiceNo) == 0))
                return false;
            pSql = 'SELECT MIN(InvoiceNumber) AS MinInvoiceNumber\n' +
                ',MAX(InvoiceNumber) AS MaxInvoiceNumber\n' +
                ' FROM TransInvoice\n' +
                ' WHERE FOWSTID = ' + ReadyString(vParams.FowstID) + '\n' +
                ' AND DeviceID = ' + ReadyString(vParams.DeviceID) + '\n' +
                ' AND date(InvoiceDate) = ' + ReadyString(pSalesDate);
            var pDrInvoice = Sql.ExecuteRow(pSql);
            (pDrInvoice == null) 
            if ($.isEmptyObject(pDrInvoice)) // (pDrInvoice == null)  will not work
                return true;
            if (CInt2(pDR.CurBegInvoiceNo) < CInt2(pDrInvoice.MinInvoiceNumber))
                return true;
            if (CInt2(pDR.CurEndInvoiceNo) > CInt2(pDrInvoice.MaxInvoiceNumber))
                return true;
            return false;
        } // END function RequireDownload(vParams) {

        function CreateZip(vParams) {
            vParams = $.extend({
                FowstID: null,
                DeviceID: null,
                SalesDate: null,
                Callback: null
            }, vParams);

            Android.AddLog('Creating zip file');
            var pUrl = '/SalesMenu/ExportInvoiceToCsvZip';
            AjaxSend({
                url: pUrl,
                data: {
                    DateFrom: vParams.SalesDate,
                    DateTo: vParams.SalesDate,
                    FowstID: vParams.FowstID,
                    DeviceID: vParams.DeviceID
                },
                success: function(vData) {
                    if (vData.Error) {
                        NewMessageBox(vData.Error);
                    } else if (vData.Results) {
                        DownloadZip(vData.Results, vParams.Callback);
                    } else
                        NewMessageBox('Unexpected result. ref: jsFQ79KZ9LWV!');
                }
            })
        }

        function DownloadZip(vBaseFilename, vCallback) {
            Android.AddLog('Downloading zip file.');
            Transfers.Download({
                Source: MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        BaseFilename: vBaseFilename 
                    }, '/SalesMenu/DownloadInvoiceCsvZip'),
                Destination: 'temp/InvoiceCsvZip/' + vBaseFilename + '.zip',
                Success: function (vData) {
                    console.log('jsWY5RF9UWFN ' + vData);
                    ExtractZip(vData.Destination, vCallback);
                },
                Error: function(vData) {
                    console.log('jsL2HBKYGNNK Error: ' + vData);
                }
            });
        }

        function ExtractZip(vFilename, vCallback) {
            Android.AddLog('Extracting zip file.');
            Transfers.ExtractZipFile({
                Source: vFilename,
                Success: function (vData) {
                    ReadCsvs(vData.Destination);
                    vCallback();
                }
            });
        } // END function ExtractZip() {

        function ReadCsvs(vPath) {
            Android.AddLog('Reading CSV files.');
            var pTables = Transfers.GetAllSubFolders(vPath);
            var pCsvReader = new CsvReader(vPath); 
            for (var pTableIndex = 0; pTableIndex <= pTables.length - 1; pTableIndex++) {
                var pTable = pTables[pTableIndex];
                pCsvReader.Open(pTable);
                var pSql = 'select name from pragma_table_info(' + ReadyString(pTable) + ')\n' +
                    ' WHERE name = \'DateImported\'';
                var pDrColumn = Sql.ExecuteRow(pSql);
                var pHasDateImportedColumn = (pDrColumn != null);
                do {
                    var pDR = pCsvReader.ReadLine();
                    if (pDR == null)
                        break;
                    var pIU = new InsertUpdate(pTable);
                    var pID = null;
                    for (var pField in pDR) 
                        if (pDR.hasOwnProperty(pField)) {
                            if (pField.toUpperCase() == 'ID')
                                pID = pDR[pField];
                            if (pDR[pField] != null)
                                pIU.Add(pField, ReadyString(pDR[pField]));
                        }
                    if (pID == null)
                        continue;
                    pSql = 'SELECT COUNT(*)\n' +
                        ' FROM ' + pTable + '\n' +
                        ' WHERE ID = ' + ReadyString(pID);
                    var pCount = Sql.ExecuteScalar(pSql);
                    if (CInt2(pCount) > 0)
                        continue;
                    // var pSql = pIU.GetUpdate('WHERE insert');
                    if (pHasDateImportedColumn)
                        pIU.Add('DateImported', proj.SqliteNow);
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql);
                    console.log(pSql);
                } while (true);
            }
        }

        mOut.SetTenantMonitoringLogs = function(vSalesDate) {
            // this function is for troubleshooting only

            if (CStr2(sessionStorage.MallReportFowstID).trim() == '')
                throw new Error('sessionStorage.MallReportFowstID is blank');
            if (CStr2(sessionStorage.MallReportDeviceID).trim() == '')
                throw new Error('sessionStorage.MallReportDeviceID is blank');

            var pSql = 'SELECT CurGrossSales\n' +
                ' FROM cache.cachezread\n' +
                ' WHERE SalesDate = ' + ReadyString(vSalesDate) + '\n' +
                ' AND FowstID = ' + ReadyString(sessionStorage.MallReportFowstID) + '\n' +
                ' AND DeviceID = ' + ReadyString(sessionStorage.MallReportDeviceID);
            var pDrCache = Sql.ExecuteRow(pSql);
            if (pDrCache == null)
                throw new Error('cachezread record not found. jsJX33X59VHS!');

            pSql = 'DELETE FROM TenantMonitoringLogs';
            Sql.ExecuteSql(pSql);

            var pIU = new InsertUpdate('TenantMonitoringLogs');
            pIU.Add('ID', ReadyString(NewID()));
            pIU.Add('SalesDate', ReadyString(vSalesDate));
            pIU.Add('Mall', "'Robinson'");
            pIU.Add('FowstID', ReadyString(sessionStorage.MallReportFowstID));
            pIU.Add('DeviceID', ReadyString(sessionStorage.MallReportDeviceID));
            pIU.Add('DateImported', "'Dont Upload jsSCLZFD77SA!'");
            pSql = pIU.GetInsert();
            Sql.ExecuteSql(pSql);

            pSql = 'UPDATE LibraryOrganizationFOWST\n' +
                ' SET IsFOWSTInsideAMall = 1\n' +
                ' WHERE ID = ' + ReadyString(sessionStorage.MallReportFowstID);
            Sql.ExecuteSql(pSql);
        };

        return mOut;
    });

    Reports.Robinsons = Reports.Robinsons(); // singleton
});