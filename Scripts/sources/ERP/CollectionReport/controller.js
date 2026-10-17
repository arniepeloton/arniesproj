define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/CollectionReport/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.CollectionReport = ERP.CollectionReport.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.CollectionReport.controller', null, function () {

        var mOut = {};
        var mElement;
        var mMode; // AR (default) or AP
        mElement = $('<div>').append(mHtml);

        if (proj.IsBootstrapDesign()) {
            mElement.find('.popup-content').appendTo(mElement.find('.modal-body'));
            mElement.find('[custom-data-role="popup"]').remove();
            mElement = mElement.find('.modal');
        } else {
            mElement.find('.modal').remove();
            mElement = mElement.find('[custom-data-role="popup"]');
        }

        $.mobile.activePage.append(mElement);
        mOut.element = mElement;

        var mTable = mElement.find('[name=tableData]');
        var mDataTable;

        mOut.Popup = function(vParams) {
            vParams = $.extend({
                Mode: 'AR'
            }, vParams);
            mMode = vParams.Mode;
            if (mMode == 'AP') {
                mElement.find('[name=spanTitle]').text('Vendor Payment Report');
                mElement.find('[name=spanLabel]').text('Vendor Payment Date');
            }
            if (proj.IsBootstrapDesign()) {
                mElement.modal('show');
            } else 
                mElement.show();
            mOut.LoadCollectionReport();
        };

        mElement.find('[name=butPaymentDate]')
            .text(
                FormatDate(new Date(), {
                    Format: 'MMM dd, yyyy'
                }) + ' to ' +
                FormatDate(new Date(), {
                    Format: 'MMM dd, yyyy'
                }))
            .click(function () {
                DualCalendar({
                    Click: function (e) {
                        mElement.find('[name=butPaymentDate]').text(
                            FormatDate(e.From, {
                                Format: 'MMM dd, yyyy'
                            }) + ' to ' +
                            FormatDate(e.To, {
                                Format: 'MMM dd, yyyy'
                            }));
                        mOut.LoadCollectionReport(e);
                    }
                }, new Date(), new Date());    
            }
        );

        mOut.LoadCollectionReport = function (dataDateRange) {
            if ($.fn.DataTable.isDataTable($('#tblCollectionReport'))) 
                dtCollectionReport.destroy();

            var dateNow = $.format.date(new Date(), 'MMM dd, yyyy');
            var from = (dataDateRange == undefined ? dateNow : dataDateRange.From);
            var to = (dataDateRange == undefined ? dateNow : dataDateRange.To);
            mOut.GetCollectionReport({
                dataDateRange: (dataDateRange == undefined ? { From: dateNow, To: dateNow } : dataDateRange),
                Success: function (vData) {                    
                    DisplayCollectionReport(vData.Results.Rows);
                    mElement.find('#btnCRPaymentDate').text(from + ' to ' + to);
                },
                Fail: function (vMessage) {
                    NewMessageBox(vMessage);
                }
            });
        }

        function DisplayCollectionReport(vData) {
            var tblCollectionReport = $('#tblCollectionReport');

            var pCsvTitle = 'Collection Report';
            if (mMode == 'AP') 
                pTitle = 'Vendor Payment Report';
            dtCollectionReport = tblCollectionReport.DataTable({
                //order: [[0, 'asc']], //vParams[0].OrderBy is array . use only in display table
                data: vData,
                columns: [
                    {
                        title: 'Customer Name',
                        data: 'CustomerName',
                        defaultContent: ''
                    },
                    {
                        title: 'Manual-Encoded Receipt Number',
                        data: 'ManualReceiptNumber',
                        defaultContent: ''
                    },
                    {
                        title: 'System-Generated Receipt Number',
                        data: 'ReceiptNumber',
                        defaultContent: ''
                    },
                    {
                        title: 'Collecting FOWST',
                        data: 'FowstName',
                        defaultContent: ''
                    },
                    {
                        title: 'Inv. No.',
                        data: 'InvoiceNumber',
                        defaultContent: ''
                    },
                    {
                        title: 'Gross Invoice Amount',
                        data: function(data) {
                            return AccountingFormat(data.GrossInvoiceAmount);
                        }
                    },
                    {
                        title: 'Invoice Amount Net of Debits and Credit Memos',
                        data: function(data) {
                            return AccountingFormat(data.NetInvoiceAmount);
                        }
                    },
                    {
                        title: 'Invoice Due Date',
                        data: function(data) {
                            if (CStr2(data.CheckDate) == '') {
                                return '';
                            }
                            return $.format.date(new Date(data.InvoiceDueDate), 'MMM dd, yyyy');
                        }
                    },
                    {
                        title: 'Payment Amount Applied to Invoice',
                        data: function(data) {
                            return AccountingFormat(data.TotalPayment);
                        }
                    },
                    {
                        title: 'Manual Inv. Ref. No.',
                        data: 'RefInvoiceNum',
                        defaultContent: ''
                    },
                    {
                        title: 'Bank Name',
                        data: 'BankName',
                        defaultContent: ''
                    },
                    {
                        title: 'Bank Branch',
                        data: 'BankBranch',
                        defaultContent: ''
                    },
                    {
                        title: 'Bank Account Name',
                        data: 'AccountName',
                        defaultContent: ''
                    },
                    {
                        title: 'Chck No.',
                        data: 'CheckNumber',
                        defaultContent: ''
                    },
                    {
                        title: 'Chck Date',
                        data: function(data) {
                            if (CStr2(data.CheckDate) == '') {
                                return '';
                            }
                            return $.format.date(new Date(data.CheckDate), 'MMM dd, yyyy');
                        }
                    },
                    {
                        title: 'Check/Online Transaction Amount Paid', // 'Chck Amt',
                        data: function(data) {
                            return AccountingFormat(data.CheckAmountPaid);
                        }
                    },
                    {
                        title: 'Total of Face Value of Check(s)', // 'Chck Amt',
                        data: function(data) {
                            return AccountingFormat(data.CheckAmount);
                        }
                    },
                    {
                        title: 'Online Transaction Source Bank Name',
                        data: 'BankNameOnline',
                        defaultContent: ''
                    },
                    {
                        title: 'Online Transaction Source Bank Branch',
                        data: 'BankBranchOnline',
                        defaultContent: ''
                    },
                    {
                        title: 'Online Transaction Source Bank Account Name',
                        data: 'AccountNameOnline',
                        defaultContent: ''
                    },
                    {
                        title: 'Online Transaction Transaction Number',
                        data: 'CheckNumberOnline',
                        defaultContent: ''
                    },
                    {
                        title: 'Online Transaction Transaction Date',
                        data: function(data) {
                            if (CStr2(data.CheckDateOnline) == '') {
                                return '';
                            }
                            return $.format.date(new Date(data.CheckDateOnline), 'MMM dd, yyyy');
                        }
                    },
                    {
                        title: 'Total of Online Transaction Amount', // 'Chck Amt',
                        data: function(data) {
                            return AccountingFormat(data.CheckAmountOnline);
                        }
                    },
                    {
                        title: 'Online Transaction Destination Bank Name',
                        data: 'BankNameOnline2',
                        defaultContent: ''
                    },
                    {
                        title: 'Online Transaction Destination Bank Branch',
                        data: 'BankBranchOnline2',
                        defaultContent: ''
                    },
                    {
                        title: 'Online Transaction Destination Bank Account Name',
                        data: 'AccountNameOnline2',
                        defaultContent: ''
                    },


//{
                    //    title: 'Chck Total',
                    //    data: function (data) {
                    //        return AccountingFormat(data.CheckTotal);
                    //    }
                    //},
                    {
                        title: 'Cash Amt',
                        data: function(data) {
                            return AccountingFormat(data.CashAmount);
                        }
                    },
                    {
                        title: 'Total',
                        data: function(data) {
                            return AccountingFormat(data.Total);
                        }
                    }
                ],
                initComplete: function() {
                    tblCollectionReport.removeClass('dataTable');
                    PopupReposition();
                },
                "aaSorting": [],
                buttons: [{ extend: 'excel', text: 'Download Excel File' }],
                sDom: '<"top"Bf>rt<"bottom"ip><"clear">', //hide search label and field
                buttons: [{ extend: 'csvHtml5', text: 'Download CSV', title: pCsvTitle}]
            });

            tblCollectionReport.find('thead tr').addClass('custom-theme-f align-top text-center');
        }

        mOut.GetCollectionReport = function (vParams) {
            var pData = vParams.dataDateRange;
            pData.FowstID = sessionStorage.FowstID;
            var pUrl = '/Browser/GetCollectionReport';
            if (mMode == 'AP')
                pUrl = '/CreditAndCollection/SupplierPaymentReport';
            AjaxSend({
                url: pUrl,
                data: pData,
                success: function (vData) {
                    if (vData.Error) {
                        vParams.Fail(vData.Error);
                    } else {
                        vParams.Success(vData);
                    }
                }
            });
        }

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});