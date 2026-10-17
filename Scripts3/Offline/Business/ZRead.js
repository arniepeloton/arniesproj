define([], function () {
    window.Business.ZRead = new function (vParent) {

        var mThis = this;

        this.HandleEnforceZRead = function (vParams) {
            // this function requires current private database
            //vParams = {
            //    silent: boolean (optional)
            //    callback: function() {}
            //};

            vParams = $.extend({}, vParams);
            var pCallback = vParams.callback;
            if ($.isFunction(pCallback) == false)
                pCallback = function () { };

            if (!Business.SystemDefaults.GetEnforceZRead()) {
                pCallback();
                return true;
            }

            var pZReadDate = mThis.GetZReadDay();
            proj.SetZReadDate(pZReadDate);

            //startclock('#clockHotButtons, #clock');

            var pToday = RemoveTime(new Date());
            var pStr;

            //if (+pZReadDate != +pToday) {
            //    if (CBool(vParams.silent)) {
            //        proj.SetZReadDate(pZReadDate);
            //        //startclock('#clockHotButtons, #clock');
            //        pCallback();
            //    } else {
            //        pStr = 'Your device date is ' + $.format.date(pToday, 'MMM d, yyyy') + ' but the Z-Read date is ' + $.format.date(pZReadDate, 'MMM d, yyyy');
            //        NewMessageBox(pStr, 'Closing Date', [
            //            {
            //                Text: 'Close the current Z-Read Date and move date forward by 1 day',
            //                Click: function () {
            //                    NewMessageBox('You will no longer be able to post sales and other transactions to the current Z-Read Date after closing.', '', [
            //                        {
            //                            Text: 'Yes',
            //                            Click: function () {
            //                                mThis.IncrementZReadDay();
            //                                mThis.HandleEnforceZRead(vParams);
            //                            }
            //                        },
            //                        {
            //                            Text: 'No',
            //                            Click: function () {
            //                                mThis.HandleEnforceZRead(vParams);
            //                            }
            //                        }
            //                    ]);

            //                }
            //            },
            //            {
            //                Text: 'Use ' + $.format.date(pZReadDate, 'MMM d, yyyy') + ' as the current date',
            //                Click: function () {
            //                    proj.SetZReadDate(pZReadDate);
            //                    //startclock('#clockHotButtons, #clock');
            //                    pCallback();
            //                }
            //            }
            //        ]);
            //    } // END if (CBool(vParams.silent)) {
            //} else // END if (+pZReadDate != +pToday) {
            //    pCallback();
        }; // END HandleEnforceZRead

        this.GetZReadDay = function () {
            if (!IsGuid(sessionStorage.OrgID)) {
                throw 'sessionStorage.OrgID must be a valid GUID before calling GetZReadDay. jsWZ3WVGHB7Y!';
            }

            if (!IsGuid(sessionStorage.FowstID)) {
                throw 'sessionStorage.FowstID must be a valid GUID before calling GetZReadDay. jsFXQVFSTGHL!';
            }

            var pData = mThis.LoadZReadDay();
            try {
                pData = JSON.parse(pData);
            } catch (e) {
                pData = {};
            }
            var pDate = GetStringObject(ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID), pData);
            if (IsDate(pDate)) {

            } else {
                pDate = Android.AppInstallDate();
            }
            pDate = RemoveTime(pDate);

            return pDate;
        };

        this.LoadZReadDay = function () {
            return Transfers.ReadFile('ZReadDay.txt');
        };

        this.IncrementZReadDay = function () {
            var pDate = mThis.GetZReadDay();
            pDate.setDate(pDate.getDate() + 1);
            pDate = FormatDate(pDate);
            var pData = mThis.LoadZReadDay();
            try {
                pData = JSON.parse(pData);
            } catch (e) {
                pData = {};
            }
            var pPath = ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID);
            CreateStringObject(pPath, pData, pDate);
            mThis.SaveZReadDay(pData);
        };

        this.SaveZReadDay = function (vData) {
            Transfers.WriteFile('ZReadDay.txt', null, JSON.stringify(vData, null, 4));
        };

        this.GenerateZeroSalesData = function (date, fowstid, deviceid) {
            //Fixed ang GUID ani
            //Insert/Update lang para masking unsa na date ang pilion, naa gihapon data ang ZRead and makaprint gihapon
            //Para nalang pud ni sa BIR if need nila idouble check ang device kung naa or walay sales ang POS

            //if (Sql.Execute('SELECT * FROM cache.CacheZRead WHERE ID <> ' + ReadyString('ccf76897-6e14-40b8-b227-b341857d8258') + ' AND DeviceID = ' + ReadyString(deviceid) + ' AND FOWSTID = ' + ReadyString(fowstid)).Rows.length > 0) {
            //    //dapat walay data ang table CacheZRead para makaproceed og generate ZERO sales sa zread
            //    return false
            //}

            var lastZread = Sql.Execute('SELECT * FROM cache.CacheZRead WHERE date(SalesDate) < ' + ReadyString($.format.date(new Date(date), 'yyyy-MM-dd')) + ' AND DeviceID = ' + ReadyString(deviceid) + ' AND FOWSTID = ' + ReadyString(fowstid) + ' ORDER BY SalesDate DESC LIMIT 1').Rows;

            if (lastZread.length == 0) {
                lastZread.push([{}]);
            } else {
                lastZread = lastZread[0];
            }

            var pIU = new InsertUpdate('cache.cachezread');

            pIU.Add('SalesDate', ReadyString($.format.date(new Date(date), 'yyyy-MM-dd')));
            pIU.Add('DeviceID', ReadyString(deviceid));
            pIU.Add('FOWSTID', ReadyString(fowstid));
            pIU.Add('BegBal', 0);
            pIU.Add('PrevGrossSales', CDec2(lastZread.PrevGrossSales) + CDec2(lastZread.CurGrossSales));
            pIU.Add('PrevTotalDiscount', CDec2(lastZread.PrevTotalDiscount) + CDec2(lastZread.CurTotalDiscount));
            pIU.Add('PrevSoloParentDiscount', CDec2(lastZread.PrevSoloParentDiscount) + CDec2(lastZread.CurSoloParentDiscount));
            pIU.Add('PrevPhNatTeamDiscount', CDec2(lastZread.PrevPhNatTeamDiscount) + CDec2(lastZread.CurPhNatTeamDiscount));
            pIU.Add('PrevSCDiscount', CDec2(lastZread.PrevSCDiscount) + CDec2(lastZread.CurSCDiscount));
            pIU.Add('PrevSCDiscountExempt', CDec2(lastZread.PrevSCDiscountExempt) + CDec2(lastZread.CurSCDiscountExempt));
            pIU.Add('PrevSalesReturnRefund', CDec2(lastZread.PrevSalesReturnRefund) + CDec2(lastZread.CurSalesReturnRefund));
            pIU.Add('PrevSalesReturnExchange', CDec2(lastZread.PrevSalesReturnExchange) + CDec2(lastZread.CurSalesReturnExchange));
            pIU.Add('PrevTotalVAT', CDec2(lastZread.PrevTotalVAT) + CDec2(lastZread.CurTotalVAT));
            pIU.Add('PrevServiceCharge', CDec2(lastZread.PrevServiceCharge) + CDec2(lastZread.CurServiceCharge));
            pIU.Add('PrevChargeAmount', CDec2(lastZread.PrevChargeAmount) + CDec2(lastZread.CurChargeAmount));
            pIU.Add('PrevVATExempt', CDec2(lastZread.PrevVATExempt) + CDec2(lastZread.CurVATExempt));
            pIU.Add('PrevSRVAmount', CDec2(lastZread.PrevSRVAmount) + CDec2(lastZread.SRVAmount));
            pIU.Add('CurGrossSales', 0);
            pIU.Add('CurTotalDiscount', 0);
            pIU.Add('CurSalesReturnRefund', 0);
            pIU.Add('CurSalesReturnExchange', 0);
            pIU.Add('CurTotalVATSales', 0);
            pIU.Add('CurTotalVAT', 0);
            pIU.Add('CurTotalVATSalesSalesReturn', 0);
            pIU.Add('CurTotalVATAmountSalesReturn', 0);
            pIU.Add('CurLocalTax', 0);
            pIU.Add('CurServiceCharge', 0);
            pIU.Add('CurNoTAXSales', 0);
            pIU.Add('CurVATExemptSales', 0);
            pIU.Add('CurZeroRatedVATSales', 0);
            pIU.Add('CurOthers', 0);
            pIU.Add('CashAmount', 0);
            pIU.Add('DebitCardAmount', 0);
            pIU.Add('CreditCardAmount', 0);
            pIU.Add('CheckPaymentAmount', 0);
            pIU.Add('SRVAmount', 0);
            pIU.Add('ChargeAmount', 0);
            pIU.Add('SMACAmount', 0);
            pIU.Add('EPlusAmount', 0);
            pIU.Add('EPlusAmount', 0);
            pIU.Add('CurBegInvoiceNo', 0);
            pIU.Add('CurEndInvoiceNo', 0);
            pIU.Add('CurTransCountInv', 0);
            pIU.Add('CurTransInvItemCount', 0);
            pIU.Add('CurTransCountMemoReturnExchange', 0);
            pIU.Add('CurSalesReturnExchangeItemCount', 0);
            pIU.Add('CurTransCountMemoRefund', 0);
            pIU.Add('CurSalesRefundItemCount', 0);
            pIU.Add('CurTermNum', 0);
            pIU.Add('FowstNo', 0);
            pIU.Add('AmountBeforeAdjustment', 0);
            pIU.Add('DateOfAdjustment', 0);
            pIU.Add('EditedBy', 0);
            pIU.Add('OtherTypeAmount', 0);
            pIU.Add('PresentationPrevGrossSales', CDec2(lastZread.PrevGrossSales) + CDec2(lastZread.CurGrossSales));
            pIU.Add('PresentationCurrGrossSales', 0);
            pIU.Add('CurSoloParentDiscount', 0);
            pIU.Add('CurPhNatTeamDiscount', 0);
            pIU.Add('CurSCDiscount', 0);
            pIU.Add('CurSCDiscountExempt', 0);
            pIU.Add('CurVATExempt', 0);
            pIU.Add('IsReported', 'NULL');
            pIU.Add('OtherTypeAmount', 0);
            pIU.Add('CurBeginningInvoiceNo', 0);
            pIU.Add('CurEndingInvoiceNo', 0);

            if (Sql.Execute("SELECT ID FROM cache.CacheZread WHERE ID = " + ReadyString('ccf76897-6e14-40b8-b227-b341857d8258') + ' AND DeviceID = ' + ReadyString(deviceid) + ' AND FOWSTID = ' + ReadyString(fowstid)).Rows.length == 0) {
                pIU.Add('ID', ReadyString('ccf76897-6e14-40b8-b227-b341857d8258'));
                Sql.Execute(pIU.GetInsert());
            } else {
                Sql.Execute(pIU.GetUpdate(' ID = ' + ReadyString('ccf76897-6e14-40b8-b227-b341857d8258')));
            }

            return true
        }

        this.GetZReadDataByPeriod = function (datefrom, dateto) {
            datefrom = $.format.date(new Date(datefrom), 'yyyy-MM-dd');
            dateto = $.format.date(new Date(dateto), 'yyyy-MM-dd');

            //delete this ID (ccf76897-6e14-40b8-b227-b341857d8258)
            //this ID is reserved for ZERO ZRead sales data of the specific date.
            //temporary lang ni sya. For BIR purposes, if need nila og ZRead for that day kahit walay POS transactions makaprint pa rin ng ZRead.
            //ang INSERT data ani naa sa ZRead dashboard (ZRead.GenerateZeroSalesData() - ZRead.js)
            //by ARNIE, added on Sep 12, 2023
            Sql.Execute('DELETE FROM cache.CacheZRead WHERE ID = ' + ReadyString('ccf76897-6e14-40b8-b227-b341857d8258'))

            var pSql = ' SELECT \n' +
                ReadyString(datefrom + ' - ' + dateto) + ' as SalesDate, \n' +
                ' CAST(SUM(BegBal) as TEXT) as BegBal, \n' +
                ' (SELECT PrevGrossSales FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY SalesDate ASC LIMIT 1) as PrevGrossSales, \n ' +
                ' (SELECT PrevTotalDiscount FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY SalesDate ASC LIMIT 1) as PrevTotalDiscount, \n ' +
                ' (SELECT PrevSCDiscount FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY SalesDate ASC LIMIT 1) as PrevSCDiscount, \n ' +
                ' (SELECT PrevSoloParentDiscount FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY SalesDate ASC LIMIT 1) as PrevSoloParentDiscount, \n ' +
                ' (SELECT PrevPhNatTeamDiscount FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY SalesDate ASC LIMIT 1) as PrevPhNatTeamDiscount, \n ' +
                ' (SELECT PrevSalesReturnRefund FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY SalesDate ASC LIMIT 1) as PrevSalesReturnRefund, \n ' +
                ' (SELECT PrevSalesReturnExchange FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY SalesDate ASC LIMIT 1) as PrevSalesReturnExchange, \n ' +
                ' (SELECT PrevTotalVAT FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY SalesDate ASC LIMIT 1) as PrevTotalVAT, \n ' +
                ' (SELECT PrevServiceCharge FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY SalesDate ASC LIMIT 1) as PrevServiceCharge, \n ' +
                ' (SELECT PrevChargeAmount FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY SalesDate ASC LIMIT 1) as PrevChargeAmount, \n ' +
                ' (SELECT PrevVATExempt FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY SalesDate ASC LIMIT 1) as PrevVATExempt, \n' +
                ' (SELECT PrevSRVAmount FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY SalesDate ASC LIMIT 1) as PrevSRVAmount, \n' +
                ' CAST(SUM(CurGrossSales) as TEXT) as CurGrossSales, \n' +
                ' CAST(SUM(CurTotalDiscount) as TEXT) as CurTotalDiscount, \n' +
                ' CAST(SUM(CurSalesReturnRefund) as TEXT) as CurSalesReturnRefund, \n' +
                ' CAST(SUM(CurSalesReturnExchange) as TEXT) as CurSalesReturnExchange, \n' +
                ' CAST(SUM(CurTotalVATSales) as TEXT) as CurTotalVATSales, \n' +
                ' CAST(SUM(CurTotalVAT) as TEXT) as CurTotalVAT, \n' +
                ' CAST(SUM(CurTotalVATSalesSalesReturn) as TEXT) as CurTotalVATSalesSalesReturn, \n' +
                ' CAST(SUM(CurTotalVATAmountSalesReturn) as TEXT) as CurTotalVATAmountSalesReturn, \n' +
                ' CAST(SUM(CurLocalTax) as TEXT) as CurLocalTax, \n' +
                ' CAST(SUM(CurServiceCharge) as TEXT) as CurServiceCharge, \n' +
                ' CAST(SUM(CurNoTAXSales) as TEXT) as CurNoTAXSales, \n' +
                ' CAST(SUM(CurVATExemptSales) as TEXT) as CurVATExemptSales, \n' +
                ' CAST(SUM(CurZeroRatedVATSales) as TEXT) as CurZeroRatedVATSales, \n' +
                ' CAST(SUM(CurOthers) as TEXT) as CurOthers, \n' +
                ' CAST(SUM(CashAmount) as TEXT) as CashAmount, \n' +
                ' CAST(SUM(DebitCardAmount) as TEXT) as DebitCardAmount, \n' +
                ' CAST(SUM(CreditCardAmount) as TEXT) as CreditCardAmount, \n' +
                ' CAST(SUM(CheckPaymentAmount) as TEXT) as CheckPaymentAmount, \n' +
                ' CAST(SUM(SRVAmount) as TEXT) as SRVAmount, \n' +
                ' CAST(SUM(ChargeAmount) as TEXT) as ChargeAmount, \n' +
                ' CAST(SUM(EPlusAmount) as TEXT) as EPlusAmount, \n' +
                ' CAST(SUM(VoucherAmount) as TEXT) as VoucherAmount, \n' +
                ' (SELECT CASE WHEN CurBegInvoiceNo = 0 THEN 1 ELSE CurBegInvoiceNo END FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' AND CurBegInvoiceNo <> 0 ORDER BY SalesDate ASC LIMIT 1) as CurBegInvoiceNo, \n' +
                ' (SELECT CurEndInvoiceNo FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY CurEndInvoiceNo DESC LIMIT 1) as CurEndInvoiceNo , \n' +
                ' SUM(CurTransCountInv) as CurTransCountInv, \n' +
                ' SUM(CurTransInvItemCount) as CurTransInvItemCount, \n' +
                ' SUM(CurTransCountMemoReturnExchange) as CurTransCountMemoReturnExchange, \n' +
                ' SUM(CurSalesReturnExchangeItemCount) as CurSalesReturnExchangeItemCount, \n' +
                ' SUM(CurTransCountMemoRefund) as CurTransCountMemoRefund, \n' +
                ' SUM(CurSalesRefundItemCount) as CurSalesRefundItemCount, \n' +
                ' SUM(CurTermNum) as CurTermNum, \n' +
                ' (SELECT FowstNo FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY FowstNo ASC LIMIT 1) as FowstNo, \n' +
                ' CAST(SUM(AmountBeforeAdjustment) as TEXT) as AmountBeforeAdjustment, \n' +
                ' "" as DateOfAdjustment , \n' +
                ' "" as EditedBy , \n' +
                ' CAST(SUM(OtherTypeAmount) as TEXT) as OtherTypeAmount, \n' +
                ' CAST((SELECT PresentationPrevGrossSales FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY SalesDate ASC LIMIT 1) as TEXT) as PresentationPrevGrossSales, \n' +
                ' CAST(SUM(PresentationCurrGrossSales) as TEXT) as PresentationCurrGrossSales, \n' +
                ' CAST(SUM(CurPhNatTeamDiscount) as TEXT) as CurPhNatTeamDiscount, \n' +
                ' CAST(SUM(CurSoloParentDiscount) as TEXT) as CurSoloParentDiscount, \n' +
                ' CAST(SUM(CurSCDiscount) as TEXT) as CurSCDiscount, \n' +
                ' CAST(SUM(CurSCDiscountExempt) as TEXT) as CurSCDiscountExempt, \n' +
                ' CAST(SUM(CurVATExempt) as TEXT) as CurVATExempt, \n' +
                ' CAST(SUM(OnCreditAmount) as TEXT) as OnCreditAmount, \n' +
                ' (SELECT CurBeginningInvoiceNo FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY CurBeginningInvoiceNo ASC LIMIT 1) as CurBeginningInvoiceNo, \n' +
                ' (SELECT CurEndingInvoiceNo FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY CurEndingInvoiceNo DESC LIMIT 1) as CurEndingInvoiceNo, \n' +
                ' (SELECT PrintCount FROM cache.CacheZRead WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' ORDER BY PrintCount DESC LIMIT 1) as PrintCount \n' +
                ' , (SELECT ZReadCount FROM TransZReadCache WHERE FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' AND SalesDate = ' + ReadyString(datefrom) +') as ZCount, \n' +
                ' CAST(SUM(CurVoid) as TEXT) as CurVoid, \n' +
                ' SUM(CurVoidItemQty) as CurVoidItemQty \n' +
                ' FROM cache.CacheZread \n' +
                ' WHERE SalesDate BETWEEN ' + ReadyString(datefrom) + ' AND ' + ReadyString(dateto) + ' \n' +
                ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' AND DeviceID = ' + ReadyString(Web.DeviceID())

            return Sql.Execute(pSql)
        }
    }
});