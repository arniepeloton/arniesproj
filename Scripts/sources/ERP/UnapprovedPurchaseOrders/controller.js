define([], function () {

   CreateStringObject('ERP.UnapprovedPurchaseOrders.controller', null, function () {

        var mOut = {};

        mOut.show = function() {
            var pColumns = [
                {
                    Label: 'PO No.',
                    //Column: function(vDR) {
                    //    var pOut = '';
                    //    if (CStr2(vDR.MIN).trim() != '')
                    //        pOut = vDR.MIN + '-';
                    //    if (!IsNumeric(vDR.PurchaseOrderNo))
                    //        vDR.PurchaseOrderNo = 0;
                    //    return 'PO-' + pOut + vDR.PurchaseOrderNo;
                    //},
                    Render: function(v1, v2, vData) {
                        return '<div name="divAction"></div>';
                    }
                },
                {
                    Label: 'Vendor',
                    Column: 'CompanyName'
                },
                {
                    Label: 'Amount',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.Total);
                    }
                },
                {
                    Label: 'Date Approved',
                    Column: 'ApprovedDate'
                },
                {
                    Label: 'SKU Count',
                    Column: 'SkuCount'
                },
                {
                    Label: 'Date Created',
                    Column: 'DateCreated'
                },
                {
                    Label: 'Last Updated',
                    Column: 'LastUpdated'
                },
                {
                    Label: 'Updated By',
                    Column: 'EmployeeFullName'
                }
            ];

            function ShowPopup(vLastModifiedFrom, vLastModifiedTo) {
                if (vLastModifiedFrom == null)
                    vLastModifiedFrom = new Date();
                if (vLastModifiedTo == null)
                    vLastModifiedTo = new Date();

                BrowsePopup('TransInvoices', 'Unapproved Purchase Orders',
                    [
                        {
                            StyleFormat: 'table',
                            AutoSearch: true,
                            DisplayText: pColumns,
                            Click: onRowClick,
                            DontCloseWhenClick: true,
                            ModalSize: 'xl',
                            //Validation: vParams.Validation,
                            ShowApprovedFilter: true,
                            rowCallback: function(row, data, dataIndex) {

                            },
                            Sorting: true,
                            LastModifiedFrom: vLastModifiedFrom,
                            LastModifiedTo: vLastModifiedTo,
                            //OrderBy: [1, 'asc']
                            LastModifiedFilterChange: function(e) {
                                ShowPopup(e.From, e.To);
                            },
                            //FixedHeader: true,
                            rowCallback: function(row, data, dataIndex){
                                var pDiv = $(row).find('[name=divAction]');
                                if (pDiv.children().length > 0)
                                    return;

                                data.TransNum = 'PO-' + data.MIN + '-' +  data.PurchaseOrderNo;

                                var pLink = $('<a>')
                                    .text(data.TransNum)
                                    .attr('href', '#')
                                    .click(function(e) {
                                        e.preventDefault();
                                        var win = window.open(location.origin + '/PurchaseOrder/Preview' + location.search + '&IsReadOnly=1&ID=' + data.ID,
                                            '_blank',
                                            'width=' + screen.availWidth + ',height=' + screen.availHeight);
                                                                
                                        return false; // para DILI magclose ang popup
                                    });
                                pDiv.append(pLink);
                            }
                        }
                    ], { 
                        Unapproved: 1,
                        TransType: 'PurchaseOrder',
                        FinalizedPoOnly: 1
                        //LastUpdatedDateFrom: FormatDate(vLastModifiedFrom, {Format: 'MMM d, yyyy'}),
                        //LastUpdatedDateTo: FormatDate(vLastModifiedTo, {Format: 'MMM d, yyyy'})
                        //LastUpdatedDateFrom: '2020/09/01', // for debugging only
                        //LastUpdatedDateTo: '2020/09/30' // for debugging only
                    }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            } // END function ShowPopup(vLastModifiedFrom, vLastModifiedTo) {

            ShowPopup();
        } // END mOut.show = function() {

        mOut.Popup = function() {
            mOut.LoadUnapproved();
            $('#popupUnapprovedPurchaseOrders').show();
        };

        mOut.LoadUnapproved = function() {
            mOut.Model.GetUnapproved({
                Success: function(vData) {
                    DisplayWithCnCUnapprovedReason(vData.Results);
                },
                Fail: function(vMessage) {
                    NewMessageBox(vMessage);
                }
            });
        };

        function DisplayWithCnCUnapprovedReason(vData) {
            var pTotalOrder = 0;
            mWithCnCUnapprovedReasonTable.find('tbody tr').remove();
            for (var p = 0; p <= vData.Rows.length - 1; p++) {
                var pDR = vData.Rows[p];
                var pTR = mWithCnCUnapprovedReasonRow.clone();
                var pDate = pDR.SalesOrderDate || pDR.LastUpdated;
                if (IsDate(pDate)) {
                    pDate = $.format.date(ParseDate(pDate), 'M/d/yyyy');
                }
                pTotalOrder += CDec2(pDR.TotalOrder);
                pTR.find('[name=CustomerName]').text(pDR.CompanyName);
                if (CInt2(pDR.SalesOrderNo) > 0)
                    pDR.TransNum = 'SO-' + pDR.MIN + '-' + pDR.SalesOrderNo;
                else if (CInt2(pDR.PurchaseOrderNo) > 0)
                    pDR.TransNum = 'PO-' + pDR.MIN + '-' +  pDR.PurchaseOrderNo;
                else if (CInt2(pDR.InvoiceNumber) > 0)
                    pDR.TransNum = 'SI-' + pDR.MIN + '-' +  pDR.InvoiceNumber;
                else
                    pDR.TransNum = pDR.MIN + '-' +  pDR.InvoiceNumber;

                (function(pDR) {
                    var pLink = $('<a>')
                        .text(pDR.TransNum)
                        .attr('href', '#')
                        .click(function(e) {
                            e.preventDefault();
                            var win = window.open(location.origin + location.pathname + location.search + '&IsReadOnly=1',
                                '_blank',
                                'width=' + window.innerWidth + ',height=' + window.innerHeight);

                            win.onload = function () {
                                function LoadRecord() {
                                    if (win.Z9Q3C9ASDA)
                                        win.Z9Q3C9ASDA.ShowPurchaseOrder({
                                            ID: pDR.ID,
                                            ReadOnly: true
                                        });
                                    else
                                        setTimeout(LoadRecord, 1000);
                                }
                                LoadRecord();    
                            }
                        
                            return false; // para DILI magclose ang popup
                        });
                    pTR.find('[name=OrderNum]')
                        .text('')
                        .append(pLink);
                })(pDR);

                pTR.find('[name=OrderDate]').text(pDate);
                pTR.find('[name=OrderAmount]').text(AccountingFormat(pDR.TotalOrder));
                pTR.find('[name=RequestedBy]').text(pDR.EmployeeFullName);
                pTR.find('[name=UnapprovedReason]').text(pDR.CnCUnapprovedReason);
                pTR.click(function() {
                    UnapprovedOrder_Click($(this));
                });
                pTR.data('data', pDR);
                mWithCnCUnapprovedReasonTable.append(pTR);
            }
            mWithCnCUnapprovedReasonTable.find('tfoot [name=CustomerName]').text(vData.Rows.length + ' record[s]');
            mWithCnCUnapprovedReasonTable.find('tfoot [name=OrderAmount]').text(AccountingFormat(pTotalOrder));
        } // END DisplayWithCnCUnapprovedReason

        function onRowClick(vDR) {
            var pDR = vDR;
            NewMessageBox('', pDR.TransNum, [
                {
                    Text: 'Approve',
                    Click: function() {
                        PromptUserAccess('Allow Approval of Purchase Orders (EAS)', {
                            OnClickOKSuccess: function () {
                                Approve({
                                    ID: pDR.ID,
                                    Success: function () {
                                        WebToast(pDR.TransNum + ' was successfully approved.', 'top');
                                        setTimeout(mOut.show(), 300);
                                    },
                                    Fail: function(vMessage) {
                                        NewMessageBox(vMessage, 'Failed', null, {
                                            Close: function() {
                                                setTimeout(mOut.show, 300);
                                            }
                                        });
                                    }
                                });
                            },
                            OnFailed: function () {
                            }
                        });
                    }
                },
                //{
                //    Text: 'View Customer Account Details',
                //    Click: function() {
                //        require(['/Scripts/sources/CreditAndCollection/CreditAndCollection.js?v=' + assetversion], function() {
                //            proj.CustomerPaymentID = pDR.CustomerID;
                //            ERP.AccountDetails.controller.GetCustomerCreditDetails(proj.CustomerPaymentID, 'has payable');
                //            $('#popupAccountDetails').find('.lblCustomerName').text(pDR.CompanyName);
                //            $('#popupAccountDetails').show(); 
                //            $('#popupUnapprovedPurchaseOrders').hide();
                //        });
                //    }
                //}
            ]);
        }

        function Approve (vParams) {
            AjaxSend({
                url: '/Erp/ApprovePo',
                data: {
                    ID: vParams.ID
                },
                success: function(vData) {
                    if (vData.Error) 
                        vParams.Fail(vData.Error);
                    else
                        vParams.Success();
                }
            });
        }

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    ERP.UnapprovedPurchaseOrders.controller = ERP.UnapprovedPurchaseOrders.controller();
});