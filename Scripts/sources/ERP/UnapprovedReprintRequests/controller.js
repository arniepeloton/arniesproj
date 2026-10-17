define([], function () {

   CreateStringObject('ERP.UnapprovedReprintRequests.controller', null, function () {

        var mOut = {};

        mOut.show = function() {
            var pColumns = [
                {
                    Label: 'Invoice No.',
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
                    Label: 'Customer',
                    Column: 'CompanyName'
                },
                {
                    Label: 'Amount',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.Total);
                    }
                },
                {
                    Label: 'Reprint Request Reason',
                    Column: 'RequestReprintReason'
                },
                {
                    Label: 'Date Requested',
                    Column: 'RequestReprintDateRequested'
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

                BrowsePopup('TransInvoices', 'Unapproved Reprint Requests',
                    [
                        {
                            ModalSize: 'xl',
                            StyleFormat: 'table',
                            AutoSearch: true,
                            DisplayText: pColumns,
                            Click: onRowClick,
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
                                var pTransNum; // full number
                                if (CStr2(data.DocumentName).trim().length > 0)
                                    pTransNum = data.DocumentName.trim() + '-' + data.MIN + '-' +  data.InvoiceNumber;
                                else if (CInt2(data.SalesOrderNo) > 0)
                                    pTransNum = 'SO-' + data.MIN + '-' + data.SalesOrderNo;
                                else if (CInt2(data.PurchaseOrderNo) > 0)
                                    pTransNum = 'PO-' + data.MIN + '-' +  data.PurchaseOrderNo;
                                else if (CInt2(data.InvoiceNumber) > 0)
                                    pTransNum = 'SI-' + data.MIN + '-' +  data.InvoiceNumber;
                                else
                                    pTransNum = data.MIN + '-' +  data.InvoiceNumber;
                                data.TransNum = pTransNum;

                                var pLink = $('<a>')
                                    .text(data.TransNum)
                                    .attr('href', '#')
                                    .click(function(e) {
                                        e.preventDefault();
                                        var win = window.open(location.origin + '/erp' + location.search + '&IsReadOnly=1&ID=' + data.ID,
                                            '_blank',
                                            'width=' + screen.availWidth + ',height=' + screen.availHeight);

                                        win.onload = function () {
                                            function LoadRecord() {
                                                if (CInt2(data.InvoiceNumber) > 0)
                                                    win.location.href = '/Erp/Invoicing' + win.location.search;
                                                else if (CInt2(data.PurchaseOrderNo) > 0)
                                                    win.location.href = '/PurchaseOrder/Preview' + win.location.search;
                                                else if (CInt2(data.SalesOrderNo) > 0)
                                                    win.location.href = '/ERP/OrderProcessing' + win.location.search;
                                            }
                                            LoadRecord();    
                                        }
                        
                                        return false; // para DILI magclose ang popup
                                    });
                                pDiv.append(pLink);
                            }
                        }
                    ], { 
                        //TransType: 'Invoice',
                        HasUnapprovedReprintRequest: 1,
                        //NonZero: 1
                        AnyZero: 1
                        //LastUpdatedDateFrom: FormatDate(vLastModifiedFrom, {Format: 'MMM d, yyyy'}),
                        //LastUpdatedDateTo: FormatDate(vLastModifiedTo, {Format: 'MMM d, yyyy'})
                        //LastUpdatedDateFrom: '2020/09/01', // for debugging only
                        //LastUpdatedDateTo: '2020/09/30' // for debugging only
                    }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            } // END function ShowPopup(vLastModifiedFrom, vLastModifiedTo) {

            ShowPopup();
        } // END mOut.show = function() {

        function onRowClick(vDR) {
            var pDR = vDR;
            NewMessageBox('', pDR.TransNum, [
                {
                    Text: 'Approve reprint request',
                    Click: function() {
                        PromptUserAccess('Reprint Invoice', {
                            OnClickOKSuccess: function () {
                                Approve({
                                    ID: pDR.RequestReprintID,
                                    Success: function() {
                                        NewMessageBox(pDR.TransNum + ' was successfully approved.', 'Success', null, {
                                            Close: function() {
                                                setTimeout(mOut.show, 300);
                                            }
                                        });
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
                url: '/Erp/ApproveReprintRequest',
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

    ERP.UnapprovedReprintRequests.controller = ERP.UnapprovedReprintRequests.controller();
});