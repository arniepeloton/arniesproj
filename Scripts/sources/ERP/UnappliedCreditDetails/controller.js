define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/UnappliedCreditDetails/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.UnappliedCreditDetails.controller = ERP.UnappliedCreditDetails.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.UnappliedCreditDetails.controller', null, function () {

        var mOut = {};
        var mElement = $(mHtml);
        var pDataTable;
        var mPaymentID;
        var mCbFilter = mElement.find('[name=cbFilter]');
        var mTableCheques = mElement.find('[name=tableCheques]');
        var mDataTableCheques;
        var mTableOnlinePayments = mElement.find('[name=tableOnlinePayments]');
        var mDataTableOnlinePayments;
        var mTableOtherPayments = mElement.find('[name=tableOtherPayments]');
        var mDataTableOtherPayments;
        var mTableInvoices = mElement.find('[name=tableInvoices]');
        var mDataTableInvoices;
        var mTableUsage = mElement.find('[name=tableUsage]');
        var mDataTableUsage;
        var mTotalCheque = 0;
        var mTotalOnlinePayment = 0;
        var mTotalOtherPayment = 0;
        var mTotalInvoices = 0;
        var mTotalUsage = 0;
        var mOnOk; // function

        mOut.popup = function(vParams) {
            
            mOut.UserInput = [];
            mOnOk = vParams.onOk;
            mPaymentID = vParams.PaymentID;
            
            function showPopup() {
                setTimeout(function() {
                    try {
                        mElement.modal('show');
                        mOut.GetData();
                        //mElement.show().popup('open');
                    } catch (ex) {
                        // just ignore the error
                        showPopup();
                    }
                }, 500);
            }
            showPopup();
        };    
        
      
        mOut.GetData = function() {
            var pData = {
                PaymentID: mPaymentID,
                CacMode: proj.CacMode // AR (default) or AP
            };
            pData.Filter = mCbFilter.val();
            var pUrl;
            if (proj.CacMode == 'AP')
                pUrl = '/Erp/GetPayablePaymentData';
            else
                pUrl = '/Erp/GetReceivablePaymentData';
            AjaxSend({
                url: pUrl,
                data: pData,
                success: function (vData) {
                    DisplayData(vData);
                }
            });
        }

        function DisplayData(vData) {

            DisplayCheques(vData);
            DisplayOnlinePayments(vData);
            DisplayOtherPayments(vData);
            DisplayInvoices(vData);
            DisplayUsage(vData);
            DisplayTotals(vData);

        }

        function DisplayTotals(vData) {
            mElement.find('[name=lblCash]').text(AccountingFormat(vData.Master.Rows[0].CashAmount));
            var pSourceOfUnAppliedCredits = CDec2(vData.Master.Rows[0].CashAmount)
                + mTotalCheque
                + mTotalOnlinePayment
                + mTotalOtherPayment;
            mElement.find('[name=lblSourceOfUnAppliedCredits]').text(AccountingFormat(pSourceOfUnAppliedCredits));
            mElement.find('[name=lblTotalCheque]').text(AccountingFormat(mTotalCheque));
            mElement.find('[name=lblTotalOnlinePayment]').text(AccountingFormat(mTotalOnlinePayment));
            mElement.find('[name=lblTotalOtherPayment]').text(AccountingFormat(mTotalOtherPayment));
            mElement.find('[name=lblTotalInvoices]').text(AccountingFormat(mTotalInvoices));
            mElement.find('[name=lblTotalUsage]').text(AccountingFormat(mTotalUsage));

            var pAvailable = pSourceOfUnAppliedCredits
                - mTotalInvoices
                - mTotalUsage;
            mElement.find('[name=spanAvailableUnappliedCredits]').text(AccountingFormat(pAvailable));
        }

        function DisplayCheques(vData) {
                    
            if ($.fn.DataTable.isDataTable(mTableCheques)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTableCheques.empty();
                mDataTableCheques.destroy();
            }

            if (vData.Checks == null)
                return;

            var pColumns = [
                {
                    title: 'Bank',
                    data: function(vData) {
                        return CStr2(vData.BankName);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {
                    title: 'Cheque No.',
                    data: function(vData) {
                        return CStr2(vData.CheckNumber);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {
                    title: 'Cheque Date',
                    'className': 'dt-body-left',
                    'data': function (data, type, full, meta) {
                        return FormatDate(data.CheckDate, {
                            DontDisplay12Am: true
                        });
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                { // 7
                    title: 'Cheque Amount',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.CheckAmount);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                }
            ];

            mTotalCheque = 0;
            var pRows = vData.Checks.Rows.filter(function(vItem) {
                if (!CBool(vItem.IsOnlinePayment)) {
                    mTotalCheque += CDec2(vItem.CheckAmount);
                    return true;
                }
            });
            mDataTableCheques = mTableCheques.DataTable({
                data: pRows,
                columns: pColumns,
                searching: false, 
                paging: false,
                info: false,
                rowCallback: function(row, data, dataIndex) {
                    var pTR = $(row);
                    pTR.data(data);

                    var pLnkDetails = pTR.find('.lnkDetails');
                    pLnkDetails.click(function() {
                        Print(data.SourcePaymentID);
                    });
                }
            });
            mTableCheques.addClass('table-stripe').css('width', '100%');
            mTableCheques.find('thead tr').addClass('custom-theme-f');
            mTableCheques.css({
                'border-bottom': '0'
            });

        }

        function DisplayOnlinePayments(vData) {
                    
            if ($.fn.DataTable.isDataTable(mTableOnlinePayments)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTableOnlinePayments.empty();
                mDataTableOnlinePayments.destroy();
            }

            if (vData.Checks == null)
                return;

            var pColumns = [
                {
                    title: 'Bank',
                    data: function(vData) {
                        return 'From ' + CStr2(vData.BankName) + ' to ' + CStr2(vData.BankName2);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {
                    title: 'Account No.',
                    data: function(vData) {
                        return 'From ' + CStr2(vData.AccountNumber) + ' to ' + CStr2(vData.AccountNumber2);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {
                    title: 'Online Trans No.',
                    data: function(vData) {
                        return CStr2(vData.CheckNumber);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {
                    title: 'Trans Date',
                    'className': 'dt-body-left',
                    'data': function (data, type, full, meta) {
                        return FormatDate(data.CheckDate, {
                            DontDisplay12Am: true
                        });
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                { // 7
                    title: 'Amount',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.CheckAmount);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                }
            ];

            mTotalOnlinePayment = 0;
            var pRows = vData.Checks.Rows.filter(function(vItem) {
                if (CBool(vItem.IsOnlinePayment)) {
                    mTotalOnlinePayment += CDec2(vItem.CheckAmount);
                    return true;
                }
            });
            mDataTableOnlinePayments = mTableOnlinePayments.DataTable({
                data: pRows,
                columns: pColumns,
                searching: false, 
                paging: false,
                info: false,
                rowCallback: function(row, data, dataIndex) {
                    var pTR = $(row);
                    pTR.data(data);

                    var pLnkDetails = pTR.find('.lnkDetails');
                    pLnkDetails.click(function() {
                        Print(data.SourcePaymentID);
                    });
                }
            });
            mTableOnlinePayments.addClass('table-stripe').css('width', '100%');
            mTableOnlinePayments.find('thead tr').addClass('custom-theme-f');
            mTableOnlinePayments.css({
                'border-bottom': '0'
            });
        }

        function DisplayOtherPayments(vData) {
                    
            if ($.fn.DataTable.isDataTable(mTableOtherPayments)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTableOtherPayments.empty();
                mDataTableOtherPayments.destroy();
            }

            var pColumns = [
                {
                    title: 'Payment Type',
                    data: function(vData) {
                        return CStr2(vData.PaymentType);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                { // 7
                    title: 'Amount',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.Amount);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                }
            ];

            mTotalOtherPayment = 0;
            vData.OtherPayments.Rows.filter(function(vItem) {
                 mTotalOtherPayment += CDec2(vItem.Amount);
            });
            mDataTableOtherPayments = mTableOtherPayments.DataTable({
                data: vData.OtherPayments.Rows,
                columns: pColumns,
                searching: false, 
                paging: false,
                info: false,
                rowCallback: function(row, data, dataIndex) {
                    var pTR = $(row);
                    pTR.data(data);

                    var pLnkDetails = pTR.find('.lnkDetails');
                    pLnkDetails.click(function() {
                        Print(data.SourcePaymentID);
                    });
                }
            });
            mTableOtherPayments.addClass('table-stripe').css('width', '100%');
            mTableOtherPayments.find('thead tr').addClass('custom-theme-f');
            mTableOtherPayments.css({
                'border-bottom': '0'
            });
        }

        function DisplayInvoices(vData) {
                    
            if ($.fn.DataTable.isDataTable(mTableInvoices)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTableInvoices.empty();
                mDataTableInvoices.destroy();
            }

            var pColumns = [
                {
                    title: 'Invoice Ref.',
                    data: function(vData) {
                        return CStr2(vData.PrintedInvoiceNumber);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                { // 7
                    title: 'Amount Paid',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.AmountPaid);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                }
            ];

            mTotalInvoices = 0;
            vData.Invoices.Rows.filter(function(vItem) {
                 mTotalInvoices += CDec2(vItem.AmountPaid);
            });
            mDataTableInvoices = mTableInvoices.DataTable({
                data: vData.Invoices.Rows,
                columns: pColumns,
                searching: false, 
                paging: false,
                info: false,
                rowCallback: function(row, data, dataIndex) {
                    var pTR = $(row);
                    pTR.data(data);

                    var pLnkDetails = pTR.find('.lnkDetails');
                    pLnkDetails.click(function() {
                        Print(data.SourcePaymentID);
                    });
                }
            });
            mTableInvoices.addClass('table-stripe').css('width', '100%');
            mTableInvoices.find('thead tr').addClass('custom-theme-f');
            mTableInvoices.css({
                'border-bottom': '0'
            });
        }

        function DisplayUsage(vData){
                    
            if ($.fn.DataTable.isDataTable(mTableUsage)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTableUsage.empty();
                mDataTableUsage.destroy();
            }

            var pColumns = [
                {
                    title: 'Payment No.',
                    data: function(vData) {
                        return CStr2(vData.PaymentNumText);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {
                    title: 'Payment Date',
                    data: function(vData) {
                        return FormatDate(vData.PaymentDate, {
                            DontDisplay12Am: true
                        });;
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },

                { // 7
                    title: 'Consumed Amount',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.ConsumedAmount);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                }
            ];

            mTotalUsage = 0;
            vData.DestinationUnappliedCredits.Rows.filter(function(vItem) {
                 mTotalUsage += CDec2(vItem.ConsumedAmount);
            });
            mDataTableUsage = mTableUsage.DataTable({
                data: vData.DestinationUnappliedCredits.Rows,
                columns: pColumns,
                searching: false, 
                paging: false,
                info: false,
                rowCallback: function(row, data, dataIndex) {
                    var pTR = $(row);
                    pTR.data(data);

                    var pLnkDetails = pTR.find('.lnkDetails');
                    pLnkDetails.click(function() {
                        Print(data.SourcePaymentID);
                    });
                }
            });
            mTableUsage.addClass('table-stripe').css('width', '100%');
            mTableUsage.find('thead tr').addClass('custom-theme-f');
            mTableUsage.css({
                'border-bottom': '0'
            });
        }

        function Print(vPaymentID) {
            if (mOut.WebPosInvoiceID != null)
                return;

            //if (!CBool(sessionStorage.IsSOALink))
            //    if (CStr2(localStorage.getItem('UUID')).length == 0) {
            //        console.log('wont print coz UUID is blank. ref: jsZATYCJJ3U6');
            //        return;
            //    }

            AjaxSend({
                url: '/ERP/CreatePaymentPdf',
                data: {
                    CustomerPaymentID: vPaymentID,
                    UUID: localStorage.getItem('UUID'),
                    Mode: proj.CacMode
                },
                success: function(vData) {
                    setTimeout(function() {
                        if (vData.Error) {
                            if (vData.Error.indexOf('PrintHash') == -1)
                                NewMessageBox(vData.Error);
                            else
                                NoPrintHash();
                        } else if (vData.Result == 'ok')
                            ViewReport(vData.BaseFilename);
                            WebToast('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
                    }, 700);
                }
            });

            function ViewReport(vFilename) {
                var pUrl = MergeUrlParameters({
                    Filename: vFilename
                }, '/Erp/DownloadPrintablePdf');
                window.open(pUrl, '_blank', 
                    'width=' + window.innerWidth + 
                    ',height=' + window.innerHeight + 
                    ',scrollbars=auto');
            } // END function ViewReport() {
        }

        mElement.find('[name=ok]').click(function() {
            var pTxts = mElement.find('[name=txtAmountToUse]');
            var pOut = {
                total: 0,
                details: []   
            };
            for (var pTxt of pTxts.toArray()) {
                pTxt = $(pTxt);
                var pAmount = CStr2(pTxt.val()).trim();
                if (pAmount == '')
                    continue;
                if (!IsNumeric(pAmount)) {
                    NewMessageBox('Amount To Use must be a valid number. You entered ' + pAmount);
                    return;
                }
                pAmount = CDec2(pAmount);
                if (pAmount < 0) {
                    NewMessageBox('Amount To Use must be greater than zero. You entered ' + pAmount);
                    return;
                }
                
                var pTR = pTxt.closest('tr');
                var pDR = pTR.data();
                if (pDR.AvailableAmount < pAmount) {
                    NewMessageBox('Amount To Use must be equal to or lesser than the available unapplied credit. You entered ' + pAmount);
                    return;
                }
                pOut.total += pAmount;
                var pItem = {
                    AmountToUse: pAmount,
                    SourcePaymentID: pDR.SourcePaymentID
                }
                pOut.details.push(pItem);
            }

            mOut.UserInput = pOut;
            mElement.modal('hide');
            mOnOk(pOut);
        });

        mCbFilter.change(function() {
            mOut.GetData();
        });

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});