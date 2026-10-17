define([
    '/Scripts/sources/ERP/OrderProcessing/dashboard/model.js',
    'promise!/Scripts/sources/ERP/UnapprovedSalesOrders/controller.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var dtCollectionReport;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/OrderProcessing/dashboard/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.OrderProcessing.dashboard.controller = ERP.OrderProcessing.dashboard.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.OrderProcessing.dashboard.controller', null, function () {

        var mOut = {};
        var mElement;        
        var mFilterCustomerID;

        mElement = $(mHtml);
        mOut.element = mElement;
        mOut.Model = new ERP.OrderProcessing.dashboard.model();

        mOut.Initialize = function() {
            $('#tabCacDashboard').html('').append(mElement);

            function AttemptShow() {
                if (typeof Cac !== 'undefined') {
                    if ((proj.CacMode != null) 
                        && (proj.CacMode != 'AP')) {

                        mOut.LoadCnCUnapprovedReasonSummary();
                        LoadAndDisplayCustomerWithCreditSummary();
                        GetTotalGrossAndDiscounts();
                        mOut.LoadCollectionReport();
                    } else
                        setTimeout(AttemptShow, 100);
                } else
                    setTimeout(AttemptShow, 100);
            }
            setTimeout(AttemptShow, 100);

            $('#btnUnapprovedOrders').click(function() {
                ERP.UnapprovedSalesOrders.controller.Popup();
            });
        }; // END mOut.Initialize = function() {
        
        function LoadAndDisplayCustomerWithCreditSummary() {
            AjaxSend({
                url: 'CreditAndCollection/GetCustomerWithCreditSummary',
                success: function (vDataCredit) {
                    var vRowsCredit = vDataCredit.Results;
                    $('#lblCacSummaryBody').text('Count: ' + AccountingFormat(vRowsCredit.Count, 0));
                    $('#lblCacSummaryFooter').text('Amount: ' + AccountingFormat(vRowsCredit.NetInvoiceBalance));
                }
            });
        }

        function GetTotalGrossAndDiscounts() {
            $('#lblNetSalesFooterCac').text($.format.date(new Date(), 'MMM dd, yyyy'));
            AjaxSend({
                url: 'DashboardsAndReports/GetTotalGrossAndDiscounts',
                data: {
                    GroupBy: 'All',
                    Fowst: 'All',
                    From: new Date(),
                    To: new Date(),
                    IsRawData: 0
                },
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        $('#lblNetSalesBodyCac').text(AccountingFormat(CDec2(vData.Results.Rows[0].GrossSales) - CDec2(vData.Results.Rows[0].TotalDiscounts) - CDec2(vData.Results.Rows[0].TotalReturns)));
                    } else {
                        $('#lblNetSalesBodyCac').text('0');
                    }
                }
            });
        }

        mOut.LoadCnCUnapprovedReasonSummary = function() {
            ERP.UnapprovedSalesOrders.model.GetWithCnCUnapprovedReason({
                Summary: true,
                Success: function(vData) {
                    DisplayCnCUnapprovedReasonSummary(vData);
                },
                Fail: function(vMessage) {
                    NewMessageBox(vMessage);
                }
            });
        };

        function DisplayCnCUnapprovedReasonSummary(vData) {
            var pStr;
            pStr = 'Count: ' + AccountingFormat(vData.Result.Count, 0);
            $('#lblUnapprovedOrdersBody').text(pStr);

            pStr = 'Amount: ' + AccountingFormat(vData.Result.TotalOrder);
            $('#lblUnapprovedOrdersFooter').text(pStr);
        }

        mOut.LoadCollectionReport = function (dataDateRange) {
            if ($.fn.DataTable.isDataTable($('#tblCollectionReport'))) 
                dtCollectionReport.destroy();

            var dateNow = $.format.date(new Date(), 'MMM dd, yyyy');
            var from = (dataDateRange == undefined ? dateNow : dataDateRange.From);
            var to = (dataDateRange == undefined ? dateNow : dataDateRange.To);
            mOut.Model.GetCollectionReport({
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

        //var mWithCnCUnapprovedReasonTable = mElement.find('[name=tableUnapprovedOrders]');
        //var mWithCnCUnapprovedReasonRow = mWithCnCUnapprovedReasonTable.find('tbody tr');
        //mWithCnCUnapprovedReasonRow.remove();
        //function DisplayWithCnCUnapprovedReason(vData) {
        //    var pTotalOrder = 0;
        //    mWithCnCUnapprovedReasonTable.find('tbody tr').remove();
        //    for (var p = 0; p <= vData.Rows.length - 1; p++) {
        //        var pDR = vData.Rows[p];
        //        var pTR = mWithCnCUnapprovedReasonRow.clone();
        //        var pDate = pDR.SalesOrderDate || pDR.LastUpdated;
        //        if (IsDate(pDate)) {
        //            pDate = $.format.date(ParseDate(pDate), 'M/d/yyyy');
        //        }
        //        pTotalOrder += CDec2(pDR.TotalOrder);
        //        pTR.data('data', pDR);
        //        pTR.find('[name=CustomerName]').text(pDR.CompanyName);
        //        pTR.find('[name=OrderNum]').text(pDR.SearchSO);
        //        pTR.find('[name=OrderDate]').text(pDate);
        //        pTR.find('[name=OrderAmount]').text(AccountingFormat(pDR.TotalOrder));
        //        pTR.find('[name=RequestedBy]').text(pDR.EmployeeFullName);
        //        pTR.find('[name=UnapprovedReason]').text(pDR.CnCUnapprovedReason);
        //        pTR.click(function() {
        //            UnapprovedOrder_Click($(this));
        //        });
        //        mWithCnCUnapprovedReasonTable.append(pTR);
        //    }
        //    mWithCnCUnapprovedReasonTable.find('tfoot [name=CustomerName]').text(vData.Rows.length + ' record[s]');
        //    mWithCnCUnapprovedReasonTable.find('tfoot [name=OrderAmount]').text(AccountingFormat(pTotalOrder));
        //} // END DisplayWithCnCUnapprovedReason

        //function UnapprovedOrder_Click(vTR) {
        //    var pDR = vTR.data('data');
        //    NewMessageBox('', 'SO-' + pDR.SearchSO, [
        //        {
        //            Text: 'Approve',
        //            Click: function() {
        //                mOut.Model.CncApproveSalesOrder({
        //                    ID: pDR.ID,
        //                    Success: function() {
        //                        NewMessageBox('SO-' + pDR.SearchSO + ' was successfully approved.');
        //                        //vTR.remove();
        //                    },
        //                    Fail: function(vMessage) {
        //                        NewMessageBox(vMessage);
        //                    }
        //                });
        //            }
        //        }
        //    ]);
        //}

        function DisplayCollectionReport(vData) {
            var tblCollectionReport = $('#tblCollectionReport');

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
                        data: function (data) {
                            return AccountingFormat(data.GrossInvoiceAmount);
                        }
                    },
                    {
                        title: 'Invoice Amount Net of Debits and Credit Memos',
                        data: function (data) {
                            return AccountingFormat(data.NetInvoiceAmount);
                        }
                    },
                    {
                        title: 'Invoice Due Date',
                        data: function (data) {
                            if (CStr2(data.CheckDate) == '') {
                                return '';
                            }
                            return $.format.date(new Date(data.InvoiceDueDate), 'MMM dd, yyyy');
                        }
                    },
                    {
                        title: 'Payment Amount Applied to Invoice',
                        data: function (data) {
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
                        data: function (data) {
                            if (CStr2(data.CheckDate) == '') {
                                return '';
                            }
                            return $.format.date(new Date(data.CheckDate), 'MMM dd, yyyy');
                        }
                    },
                    {
                        title: 'Total of Face Vlaue of Check(s)', // 'Chck Amt',
                        data: function (data) {
                            return AccountingFormat(data.CheckAmount);
                        }
                    },
                    //{
                    //    title: 'Chck Total',
                    //    data: function (data) {
                    //        return AccountingFormat(data.CheckTotal);
                    //    }
                    //},
                    {
                        title: 'Cash Amt',
                        data: function (data) {
                            return AccountingFormat(data.CashAmount);
                        }
                    },
                    {
                        title: 'Total',
                        data: function (data) {
                            return AccountingFormat(data.Total);
                        }
                    }
                ],
                initComplete: function () {
                    PopupReposition();
                },
                "aaSorting": [],
                buttons: [{extend: 'excel', text: 'Download Excel File'}],
                sDom: '<"top"Bf>rt<"bottom"ip><"clear">', //hide search label and field
                buttons: [{ extend: 'csvHtml5', text: 'Download CSV', title: 'Collection Report' } ]
            })
            tblCollectionReport.find('thead tr').addClass('custom-theme-f');
        }


        mElement.find('#btnCRPaymentDate').click(function () {
            PopupPeriod({
                Click: function (e) {
                    if ($.fn.DataTable.isDataTable($('#tblCollectionReport'))) {
                        dtCollectionReport.destroy();
                    }                    
                    mOut.LoadCollectionReport(e);
                }
            })            
        });
        
        mElement.find('#butPaymentsReportCustomer').click(function (vEl) {
            BrowsePopup('LibraryOrganization', 'Customer',
                [
                    {
                        //Search: mTxtCustomer.val(),
                        DisplayText: [{ Column: 'CompanyName' }],
                        Click: function (e) {
                            mElement.find('#butPaymentsReportCustomer').text(e.CompanyName);
                            mFilterCustomerID = e.ID;
                        }
                    }
                ], {
                    FilterCustomers: 1
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        });

        mElement.find('#butPaymentsReportEncodingDate, #butPaymentsReportPaymentMaturityDate').click(function (vEl) {
            PopupPeriod({
                Click: function (e) {
                    DisplayPaymentsReportEncodingDate($(vEl.target).attr('id'), e);
                }
            });        
        });

        function DisplayPaymentsReportEncodingDate(vID, vData) {
            var dateNow = $.format.date(new Date(), 'MMM dd, yyyy');
            var from = (vData == undefined ? dateNow : vData.From);
            var to = (vData == undefined ? dateNow : vData.To);
            mElement.find('#' + vID).text(from + ' to ' + to);
        }

        DisplayPaymentsReportEncodingDate('butPaymentsReportEncodingDate');
        DisplayPaymentsReportEncodingDate('butPaymentsReportPaymentMaturityDate');

        mElement.find('#butPaymentsReportDownload').click(function (e) {
            e.preventDefault();
            var pData = {
                EncodingDateRange: $('#butPaymentsReportEncodingDate').text(),
                DoNotFilterEncodingDate: $('#chkPaymentsReportEncodingDateDoNotFilter').prop('checked'),
                MaturityDateRange: $('#butPaymentsReportPaymentMaturityDate').text(),
                DoNotFilterMaturityDate: $('#chkPaymentsReportPaymentMaturityDateDoNotFilter').prop('checked'),
                DoNotFilterDepositDate: 1,
                IncludeCash: $('#chkPaymentsReportCash').prop('checked'),
                IncludeChecks: $('#chkPaymentsReportCheck').prop('checked'),
                IncludeCreditCards: $('#chkPaymentsReportCreditCard').prop('checked'),
                IncludeDebitCards: $('#chkPaymentsReportDebitCard').prop('checked'),
                // IncludeSrv: $('#chkPaymentsReportSrv').prop('checked'),
                IncludeSrv: 0,
                IncludeDeposited: 1,
                IncludeReturned: 1,
                IncludeUndeposited: 1,
                DownloadAsCustomerPaymentPdf: 1
            };
            if ($('#chkPaymentsReportCustomerDoNotFilter').prop('checked') == false)
                pData.CustomerID = mFilterCustomerID;

            AjaxSend({
                url: '/Browser/CustomerPaymentReport',
                data: pData,
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else if (vData.Filename) 
                        ViewReport(vData.Filename) ;
                }
            });

            function ViewReport(vFilename) {
                
                var pUrl = MergeUrlParameters({
                    Filename: vFilename
                }, '/ERP/DownloadPrintablePdf');
                window.open(pUrl, '_blank', 
                    'width=' + window.innerWidth + 
                    ',height=' + window.innerHeight + 
                    ',scrollbars=auto');
                NewMessageBox('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
            } // END function ViewReport() {
        });

        mOut.Initialize();

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});