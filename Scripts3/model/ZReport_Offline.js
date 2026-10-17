define(['common']
, function () {
    CreateStringObject('App.model.ZReport_Offline', null, function () {
        var pOut = {};
        var pSql = '';
        var pDT;
        var dateNow = new Date();
        var data = {};

        pOut.GetSalesZReportSummary = function (data) {
            var dateFrom;
            var dateTo;
            dateNow = new Date();

            if (data.RangeType == 'Today') {
                dateFrom = $.format.date(dateNow, 'yyyy-MM-dd') + ' 00:00:00';
                dateTo = $.format.date(dateNow, 'yyyy-MM-dd') + ' 23:59:59';
            }
            else if (data.RangeType == 'Yesterday') {
                dateNow.setDate(dateNow.getDate() - 1);

                dateFrom = $.format.date(dateNow, 'yyyy-MM-dd') + ' 00:00:00';
                dateTo = $.format.date(dateNow, 'yyyy-MM-dd') + ' 23:59:59';
            }
            else if (data.RangeType == 'By Date') {
                dateFrom = $.format.date(new Date(data.ZDate), 'yyyy-MM-dd') + ' 00:00:00';
                dateTo = $.format.date(new Date(data.ZDateTo), 'yyyy-MM-dd') + ' 23:59:59';
            }

            pSql = 'SELECT IFNULL(devf.BegBalance,0)  AS [BegBal] ' +
                    ', IFNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' +
                    '           WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> "RF"  AND inv.InvoiceDate < ' + ReadyString(dateFrom) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [PrevGrossSales] ' +
                    '' +
                    ', IFNULL((SELECT SUM(invitem.DiscountAmount)  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' +
                    '           WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> "RF"  AND inv.InvoiceDate < ' + ReadyString(dateFrom) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [PrevTotalDiscount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(srledger.GrossRefundAmount)  FROM TransSalesReturnLedger srledger ' +
                    '           INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' +
                    '           WHERE date(memo.LastUpdated) < ' + ReadyString(dateFrom) +
                    '           AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [PrevSalesReturnRefund] ' +
                    '' +
                    ', IFNULL((SELECT SUM(srledger.GrossSrvAmount)  FROM TransSalesReturnLedger srledger ' +
                    '           INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' +
                    '           WHERE date(memo.LastUpdated) < ' + ReadyString(dateFrom) +
                    '           AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [PrevSalesReturnExchange] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(invitem.TaxAmount,0) - IFNULL(invitem.DiscountTax,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' +
                    '           WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> "RF" ' +
                    '           AND inv.InvoiceDate < ' + ReadyString(dateFrom) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [PrevTotalVAT] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(srledger.Fees, 0) + IFNULL(srledger.RefundFees, 0))  FROM TransSalesReturnLedger srledger ' +
                    '           INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' +
                    '           WHERE memo.LastUpdated < ' + ReadyString(dateFrom) +
                    '           AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [PrevServiceCharge] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(custpay.ChargeAmount,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE inv.InvoiceDate < ' + ReadyString(dateFrom) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [PrevChargeAmount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(case when [quantity]=(0) then (0) when (((ifnull([vatablesale],(0))+ifnull([vatexemptsale],(0)))+ifnull([zeroratedsale],(0)))+ifnull([nonvatsale],(0)))=(0) then ifnull([subtotal],(0)) else ((ifnull([vatablesale],(0))+ifnull([vatexemptsale],(0)))+ifnull([zeroratedsale],(0)))+ifnull([nonvatsale],(0)) end) ' +
                    '           FROM TransInvoice inv INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' +
                    '           WHERE inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '       ), 0) AS [CurGrossSales] ' +
                    '' +
                    ', IFNULL((SELECT SUM(invitem.DiscountAmount)  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' +
                    '           WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> "RF" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurTotalDiscount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(invitem.DiscountAmount)  FROM TransInvoiceItems invitem ' +
                    '           INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) ' +
                    '           INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) ' +
                    '           WHERE inv.InvoiceNumber > 0  AND invdisc.DiscountTypeID = "764EDACB-0131-4033-8E44-7B903368008C" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [RegularDiscount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(invitem.DiscountAmount)  FROM TransInvoiceItems invitem ' +
                    '           INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) ' +
                    '           INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) ' +
                    '           WHERE inv.InvoiceNumber > 0  AND invdisc.DiscountTypeID = "7804A35A-B531-4380-945E-FC9643E99843" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [EmployeeDiscount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(invitem.DiscountAmount)  FROM TransInvoiceItems invitem ' +
                    '           INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) ' +
                    '           INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) ' +
                    '           WHERE inv.InvoiceNumber > 0  AND invdisc.DiscountTypeID = "055A0A33-DD98-47EC-ADD3-8B2009F3944A" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [SeniorCitizenDiscount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(invitem.DiscountAmount)  FROM TransInvoiceItems invitem ' +
                    '           INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) ' +
                    '           INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) ' +
                    '           WHERE inv.InvoiceNumber > 0  AND invdisc.DiscountTypeID = "34A4032F-A142-4C15-A868-4AC4410434DF" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [PWDDiscount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(invitem.DiscountAmount)  FROM TransInvoiceItems invitem ' +
                    '           INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) ' +
                    '           INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) ' +
                    '           WHERE inv.InvoiceNumber > 0  AND invdisc.DiscountTypeID = "35F954A0-C67C-499C-AF60-F08E64477FDC" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [VIPDiscount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(invitem.DiscountAmount)  FROM TransInvoiceItems invitem ' +
                    '           INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) ' +
                    '           INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) ' +
                    '           WHERE inv.InvoiceNumber > 0  AND invdisc.DiscountTypeID = "C2218C86-0450-4CD3-B019-3EDD43E51741" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [GPCDiscount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(invitem.DiscountAmount)  FROM TransInvoiceItems invitem ' +
                    '           INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) ' +
                    '           INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) ' +
                    '           WHERE inv.InvoiceNumber > 0  AND invdisc.DiscountTypeID = "C4199347-6A00-45C9-BE11-8934DD63ECD1" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [Discount1] ' +
                    ', IFNULL((SELECT SUM(srledger.GrossRefundAmount)  FROM TransSalesReturnLedger srledger ' +
                    '           INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' +
                    '           WHERE memo.LastUpdated BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurSalesReturnRefund] ' +
                    '' +
                    ', IFNULL((SELECT SUM(srledger.GrossSrvAmount)  FROM TransSalesReturnLedger srledger ' +
                    '           INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' +
                    '           WHERE memo.LastUpdated BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurSalesReturnExchange] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(invitem.TaxSales,0)- IFNULL(invitem.DiscountSales,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' +
                    '           WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> "RF" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurTotalVATSales] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(invitem.TaxAmount,0)- IFNULL(invitem.DiscountTax,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' +
                    '           WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> "RF" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurTotalVAT] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(memosr.TotalAmount,0) - IFNULL(memosr.TaxAmount,0))  FROM TransMemoSalesReturn memosr ' +
                    '           INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) ' +
                    '           WHERE memo.LastUpdated BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurTotalVATSalesSalesReturn] ' +
                    '' +
                    ', IFNULL((SELECT SUM(memosr.TaxAmount)  FROM TransMemoSalesReturn memosr ' +
                    '           INNER JOIN TransMemo memo ON (memosr.MemoID = memo.ID) ' +
                    '           WHERE memo.LastUpdated BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurTotalVATAmountSalesReturn] ' +
                    '' +
                    ', 0 AS [CurLocalTax] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(srledger.Fees, 0) + IFNULL(srledger.RefundFees, 0))  FROM TransSalesReturnLedger srledger ' +
                    '           INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' +
                    '           WHERE memo.LastUpdated BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurServiceCharge] ' +
                    '' +
                    ', IFNULL((SELECT SUM(invitem.NonVATSale)  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' +
                    '           WHERE inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurNoTAXSales] ' +
                    '' +
                    ', IFNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions - invitem.DiscountAmount)  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' +
                    '           WHERE (inv.InvoiceNumber > 0 AND custpay.ReceiptType <> "RF" ) AND invitem.TaxAmount = 0 AND IFNULL(invitem.VATableSale,0) = 0 ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurVATExemptSales] ' +
                    '' +
                    ', IFNULL((SELECT SUM(invitem.ZeroRatedSale)  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' +
                    '           WHERE inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurZeroRatedVATSales] ' +
                    '' +
                    ', 0 AS [CurOthers] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(custpay.CashAmount,0) - IFNULL(custpay.CustomerChangeCash,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE custpay.PaymentDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CashAmount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(custpay.DebitCardAmount,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE custpay.PaymentDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [DebitCardAmount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(custpay.CreditCardAmount,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE custpay.PaymentDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CreditCardAmount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(custpay.CheckPaymentAmount,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE custpay.PaymentDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CheckPaymentAmount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(custpay.SRVAmount,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE custpay.PaymentDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [SRVAmount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(custpay.OtherTypeAmount,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE custpay.PaymentDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [OtherTypeAmount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(custpay.ChargeAmount,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE custpay.PaymentDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [ChargeAmount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(other.Amount,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE other.PaymentType = "SMAC" AND custpay.PaymentDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [SMACAmount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(other.Amount,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE other.PaymentType = "E-Plus" AND custpay.PaymentDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [EPlusAmount] ' +
                    '' +
                    ', IFNULL((SELECT SUM(IFNULL(other.Amount,0))  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE other.PaymentType = "Discount Voucher / On Line Deals" AND custpay.PaymentDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [VoucherAmount] ' +
                    '' +
                    ', 0 AS [GiftCertAmount] ' +
                    ', 0 AS [LoyaltyCardAmount] ' +
                    ', 0 AS [CreditTermsAmount] ' +
                    ', ' + ReadyString(dateFrom) + ' AS [SalesDate]' +
                    '' +
                    ', IFNULL((SELECT MIN(inv.InvoiceNumber)  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> "RF" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurBegInvoiceNo] ' +
                    '' +
                    ', IFNULL((SELECT MAX(inv.InvoiceNumber)  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> "RF" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurEndInvoiceNo] ' +
                    '' +
                    ', IFNULL((SELECT COUNT(inv.InvoiceNumber)  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> "RF" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurTransCountInv] ' +
                    '' +
                    ', IFNULL((SELECT COUNT(invitem.ProductID)  FROM TransCustomerPaymentInvoices custpayinv ' +
                    '           INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) ' +
                    '           INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) ' +
                    '           INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) ' +
                    '           WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> "RF" ' +
                    '           AND inv.InvoiceDate BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND inv.FOWSTID = devf.FowstID  AND inv.DeviceID = devf.DeviceID ' +
                    '       ), 0) AS [CurTransInvItemCount] ' +
                    '' +
                    ', IFNULL((SELECT DISTINCT COUNT(memo.MemoNumber)  FROM TransSalesReturnLedger srledger ' +
                    '           INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' +
                    '           WHERE memo.LastUpdated BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID ' +
                    '           AND srledger.DestinationPaymentID IS NULL  AND srledger.SrvNum > 0 ' +
                    '       ), 0) AS [CurTransCountMemoReturnExchange] ' +
                    '' +
                    ', IFNULL((SELECT COUNT(memoret.ProductID)  FROM TransSalesReturnLedger srledger ' +
                    '           INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' +
                    '           INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) ' +
                    '           WHERE memo.LastUpdated BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID ' +
                    '           AND srledger.DestinationPaymentID IS NULL  AND srledger.SrvNum > 0 ' +
                    '       ), 0) AS [CurSalesReturnExchangeItemCount] ' +
                    '' +
                    ', IFNULL((SELECT DISTINCT COUNT(memo.MemoNumber)  FROM TransSalesReturnLedger srledger ' +
                    '           INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' +
                    '           WHERE memo.LastUpdated BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND memo.DeviceID = devf.DeviceID  AND memo.DeviceID = devf.DeviceID ' +
                    '           AND srledger.DestinationPaymentID IS NULL  AND srledger.RefundNum  > 0 ' +
                    '       ), 0) AS [CurTransCountMemoRefund] ' +
                    '' +
                    ', IFNULL((SELECT COUNT(memoret.ProductID)  FROM TransSalesReturnLedger srledger ' +
                    '           INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) ' +
                    '           INNER JOIN TransMemoSalesReturn memoret ON (memoret.MemoID = memo.ID) ' +
                    '           WHERE memo.LastUpdated BETWEEN ' + ReadyString(dateFrom) + ' AND ' + ReadyString(dateTo) +
                    '           AND memo.FOWSTID = devf.FowstID  AND memo.DeviceID = devf.DeviceID ' +
                    '           AND srledger.DestinationPaymentID IS NULL  AND srledger.RefundNum > 0 ' +
                    '       ), 0) AS [CurSalesRefundItemCount] ' +
                    '' +
                    ', dev.DeviceNo AS [CurTermNum] ' +
                    ', fowst.FOWSTNo AS [FowstNo] ' +
                    '' +
                    'FROM LibraryDeviceFowst devf ' +
                    'INNER JOIN LibraryDevice dev ON (devf.DeviceID = dev.ID) ' +
                    'INNER JOIN LibraryOrganizationFOWST fowst ON (devf.FowstID = fowst.ID) ' +
                    'WHERE devf.FowstID = ' + ReadyString(data.FowstID)

            pDT = Sql.Execute(pSql);

            data.ReportPOS = pDT;

            return data;
        }

        pOut.GetReportHeader = function (id) {
            pSql = 'SELECT subcrbr.OrganizationID AS [SubscriberOrganizationID] ' +
                    ', subcr.CompanyName AS [SubscriberOrganizationName] ' +
                    ', rsetfowst.FowstID AS [OrganizationBranchID] ' +
                    ', rsettrdname.BusinessTradeName AS [OrgBusinessTradeName] ' +
                    ', subcrbr.[Name] AS [OrgBranchName] ' +
                    ', IFNULL(subcrbr.StreetNumber, "") || " " || IFNULL(subcrbr.Street, "") || " " || IFNULL(subcrcty.Name, "") || " " || IFNULL(subcrctry.Name, "") AS OrgCompleteAddress ' +
                    ', (SELECT ContactNo FROM LibraryOrganizationFOWSTTel WHERE FowstID = rsetfowst.FowstID) AS [OrgTelNo] ' +
                    ', subcr.TinNumber AS [OrgTIN] ' +
                    ', IFNULL((SELECT IFNULL(Surname, "") || " " || IFNULL(FirstName, "") || " " || IFNULL(MiddleName, "")  FROM LibraryPeople  WHERE ID = "c530bdf3-ea03-4dd3-95cd-a384374aa570" LIMIT 1), "") AS PrintedBy ' +
                    ', datetime("now", "localtime") AS [PrintDateTime] ' +
                    ', 0 AS [POSReportType] ' +
                    'FROM LibraryReceiptSettingsFowst rsetfowst ' +
                    'INNER JOIN LibraryReceiptSettings rset ON (rsetfowst.ReceiptSettingsID = rset.ID) ' +
                    'LEFT JOIN LibraryOrganizationTradeName rsettrdname ON (rset.OrgTradeNameID = rsettrdname.ID) ' +
                    'INNER JOIN LibraryOrganizationFOWST subcrbr ON (rsetfowst.FowstID = subcrbr.ID) ' +
                    'INNER JOIN LibraryOrganization subcr ON (subcrbr.OrganizationID = subcr.ID) ' +
                    'LEFT JOIN LibraryCities subcrcty ON (subcrbr.FowstCityID = subcrcty.ID) ' +
                    'LEFT JOIN LibraryCountries subcrctry ON (subcrbr.FowstCountryID = subcrctry.ID) ' +
                    'WHERE rsetfowst.FowstID = ' + ReadyString(id)

            pDT = Sql.Execute(pSql);

            data.ReportPOS = pDT;

            return data;
        }

        return pOut;
    })
})