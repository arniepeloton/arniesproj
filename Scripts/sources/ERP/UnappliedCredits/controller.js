define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/UnappliedCredits/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.UnappliedCredits.controller = ERP.UnappliedCredits.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.UnappliedCredits.controller', null, function () {

        var mOut = {};
        var mElement = $(mHtml);
        var pDataTable;
        var mCustomerOrSupplierID;
        var mCbFilter = mElement.find('[name=cbFilter]');
        var mTable = mElement.find('table');
        var mDataTable;
        var mOnOk; // function

        $('body').append(mElement);

        mOut.popup = function(vParams) {
            
            mOut.UserInput = [];
            mOnOk = vParams.onOk;
            mCustomerOrSupplierID = vParams.CustomerOrSupplierID;
            
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
                CustomerOrSupplierID: mCustomerOrSupplierID,
                CacMode: proj.CacMode // AR (default) or AP
            };
            pData.Filter = mCbFilter.val();
            AjaxSend({
                url: '/CreditAndCollection/GetAvailableUnappliedCreditsDetails',
                data: pData,
                success: function (vData) {
                    DisplayData(vData.Results.Rows);
                }
            });
        }

        function DisplayData(vData) {
                    
            if ($.fn.DataTable.isDataTable(mTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTable.empty();
                mDataTable.destroy();
            }

            var pColumns = [
                //{
                //    title: '',
                //    'className': 'text-center',
                //    'data': function (data, type, full, meta) {
                //        return ((CInt2(data.NoOfAttachments) + CInt2(data.NoOfReceivingInvRefImages)) == 0 ? '' : '<button data-role="none" class="btn btn-warning btn-sm" title="View Invoice Image"><span class="bi-paperclip"></span></button>'); // attachment cell
                //    }, 'createdCell': function (td, cellData, rowData, row, col) {
                //        $(td).attr('style', 'min-width:50px;');
                //    }
                //},
                {
                    title: 'Payment No.',
                    data: function(vData) {
                        var pText = '';
                        if (CStr2(vData.ManualReceiptNumber).trim() != '')
                            pText = vData.ManualReceiptNumber;
                        else
                            pText = vData.PaymentNum;
                        pText = '<a class="lnkDetails" href="#">' + pText + '</a>';
                        return pText
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                {
                    title: 'Payment Date',
                    'className': 'dt-body-left',
                    'data': function (data, type, full, meta) {
                        return FormatDate(data.PaymentDate, {
                            DontDisplay12Am: true
                        });
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                { // 7
                    title: 'Unapplied Credit',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.UnappliedPayment);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                { // 7
                    title: 'Used Credit',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.ConsumedAmount);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                { // 7
                    title: 'Available Credit',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        return AccountingFormat(data.AvailableAmount);
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
                { // 7
                    title: 'Amount to Use',
                    'className': 'text-right',
                    'data': function (data, type, full, meta) {
                        if (CDec2(data.AvailableAmount) > 0)
                            return '<input name=txtAmountToUse class="form-control" />';
                        else
                            return '';
                    }, 'createdCell': function (td, cellData, rowData, row, col) {
                        $(td).attr('style', 'min-width:200px;');
                    }
                },
            ];

            //Array.prototype.push.apply(pColumns, 
            //[
            //    {
            //        name: 'GrossInvoiceAmount',
            //        title: 'Gross Invoice Amount',
            //        'className': 'text-right',
            //        'data': function (data, type, full, meta) {
            //            return AccountingFormat(data.GrossInvoiceAmount);
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:200px;');
            //        }
            //    },
            //    {
            //        name: 'AdjustmentAmount',
            //        title: 'Net Adjustments',
            //        'className': 'text-right',
            //        'data': function (data, type, full, meta) {
            //            return AccountingFormat(data.AdjustmentAmount);
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:200px;');
            //        }
            //    },
            //    { // 5
            //        name: 'NetInvoiceAmount',
            //        title: 'Net Invoice  Amount',
            //        'className': 'text-right',
            //        'data': function (data, type, full, meta) {
            //            return AccountingFormat(data.NetInvoiceAmount);
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:200px;');
            //        }
            //    },
            //    { // 6
            //        name: 'TotalAPPPendingApproval',
            //        title: 'Accum. Customer Payments Pending Approval',
            //        'className': 'text-right',
            //        'data': function (data, type, full, meta) {
            //            return AccountingFormat(data.APPPendingApproval);
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:200px;');
            //        }
            //    },
            //    { // 7
            //        name: 'TotalPayment',
            //        title: 'Accum. Prev.  Payments',
            //        'className': 'text-right',
            //        'data': function (data, type, full, meta) {
            //            return AccountingFormat(data.TotalPayment);
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:200px;');
            //        }
            //    },
            //    {
            //        name: 'NetInvoiceBalance',
            //        title: 'Net Invoice  Balance',
            //        'className': 'text-right',
            //        'data': function (data, type, full, meta) {
            //            return AccountingFormat(data.NetInvoiceBalance);
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:200px;');
            //        }
            //    },
            //    {   title: 'Invoice Terms',
            //        'data': function (data, type, full, meta) {
            //            return AccountingFormat(data.CreditTerms, 0);
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:150px;');
            //        }
            //    },
            //    {
            //        title: 'Invoice Due Date',
            //        'className': 'text-center',
            //        'data': function (data, type, full, meta) {
            //            return FormatDate(data.InvoiceDueDate);
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:200px;');
            //        }
            //    },
            //    {
            //        title: 'AR Age',
            //        'className': 'text-center',
            //        'data': function (data, type, full, meta) {
            //            var pOut = AccountingFormat(data.ArAge, 0);
            //            if (CDec2(data) > 0)
            //                pOut = '<span style="color:red">' + pOut + '<span>';
            //            return pOut;
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:80px;');
            //        }
            //    },
            //    {
            //        name: 'WeightRelativeToTotalAr',
            //        title: 'Weight Of Transaction Relative to Total AR of Customer',
            //        'className': 'text-right',
            //        'data': function (data, type, full, meta) {
            //            var pOut = AccountingFormat(data.WeightRelativeToTotalAr) + '%';
            //            return pOut;
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:300px;');
            //        }
            //    },
            //    {
            //        name: 'WeightedAverageDaysOverdue',
            //        title: 'Weighted Days Overdue',
            //        'className': 'text-right',
            //        'data': function (data, type, full, meta) {
            //            var pOut = AccountingFormat(data.WeightedAverageDaysOverdue);
            //            return pOut;
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:200px;');
            //        }
            //    },
            //    {   
            //        title: 'Last Modified',
            //        'className': 'text-center',
            //        'data': function (data, type, full, meta) {
            //            return FormatDate(data.LastUpdated);
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:200px;');
            //        }
            //    },
            //    {
            //        title: 'Remarks',
            //        'className': 'text-center',
            //        'data': function (data, type, full, meta) {
            //            return CStr2(data.Remarks);
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:200px;');
            //        }
            //    },
            //    {
            //        title: 'PO No.',
            //        'className': 'dt-body-left',
            //        'data': function (data, type, full, meta) {
            //            return CStr2(data.PurchaseOrderText);
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:100px;');
            //        }
            //    },
            //    {
            //        title: 'PO Date',
            //        'className': 'dt-body-left',
            //        'data': function (data, type, full, meta) {
            //            return FormatDate(data.PurchaseOrderDate, {
            //                DontDisplay12Am: true
            //            });
            //        }, 'createdCell': function (td, cellData, rowData, row, col) {
            //            $(td).attr('style', 'min-width:100px;');
            //        }
            //    }                
            //]);

            //if (mOut.Mode == 'AP') {
            //    pColumns = pColumns.concat([
            //        {
            //            title: 'Accounts Payable Ref No',
            //            'className': 'dt-body-left',
            //            'data': function (data, type, full, meta) {
            //                return CStr2(data.ApNum);
            //            }, 'createdCell': function (td, cellData, rowData, row, col) {
            //                $(td).attr('style', 'min-width:200px;');
            //            }
            //        },
            //        {
            //            title: 'Accounts Payable Date',
            //            'className': 'dt-body-left',
            //            'data': function (data, type, full, meta) {
            //                return FormatDate(data.ApDate, {
            //                    DontDisplay12Am: true
            //                });
            //            }, 'createdCell': function (td, cellData, rowData, row, col) {
            //                $(td).attr('style', 'min-width:200px;');
            //            }
            //        },
            //        {
            //            title: 'Accounts Payable Status',
            //            'className': 'dt-body-left',
            //            'data': function (data, type, full, meta) {
            //                return data.ApStatus + '';
            //            }, 'createdCell': function (td, cellData, rowData, row, col) {
            //                $(td).attr('style', 'min-width:200px;');
            //            }
            //        }
            //    ]);
            //}

            mDataTable = mTable.DataTable({
                data: vData,
                columns: pColumns,
                rowCallback: function(row, data, dataIndex) {
                    var pTR = $(row);
                    pTR.data(data);

                    var pLnkDetails = pTR.find('.lnkDetails');
                    pLnkDetails.click(function() {
                        ShowDetails(data.SourcePaymentID);
                    });
                },
                initComplete: function(){
                    mTable.removeClass('dataTable');
                }
            });
            mTable.addClass('table-stripe').css('width', '100%');
            mTable.find('thead tr').addClass('custom-theme-f');
        }

        function ShowDetails(vPaymentID) {
            require(['promise!/Scripts/sources/ERP/UnappliedCreditDetails/controller.js?v=' + assetversion], function() {
                ERP.UnappliedCreditDetails.controller.popup({
                    PaymentID: vPaymentID,
                    onOk: function(vData) {
                        
                    }
                });
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
                if (CDec2(pDR.AvailableAmount) < pAmount) {
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