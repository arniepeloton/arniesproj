define([], function () {

   CreateStringObject('ERP.PriceUpdateRequests.controller', null, function () {

        var mOut = {};

        mOut.show = function() {
            var pColumns = [
                {
                    Label: 'Product No.',
                    Column: 'ProductNo'
                },
                {
                    Label: 'Brand',
                    Column: 'Brand'
                },
                {
                    Label: 'Product Name.',
                    Column: 'ProductName'
                },
                {
                    Label: 'Current Retail Price',
                    Column: function(vData) {
                        var pStr = '<b>' + AccountingFormat(vData.RetailPrice) + '</b>';
                        if (IsDate(vData.RetailPriceLastUpdated)) {
                            pStr += '<br />Last&nbsp;Updated:<br />' + FormatDate(vData.RetailPriceLastUpdated, { Format: 'M/d/yyyy' });
                            pStr += '<br />' + AccountingFormat((new Date().getTime() - new Date(vData.RetailPriceLastUpdated)) / 1000 / 60 / 60 / 24, 0) + ' days ago';
                        }
                        pStr = '<div style="text-align: center">' + pStr + '</div>';

                        return pStr;
                    }
                },
                {
                    Label: 'Current Wholesale Price',
                    Column: function(vData) {
                        var pStr = '<b>' + AccountingFormat(vData.WholesalePrice) + '</b>';
                        if (IsDate(vData.WholesalePriceLastUpdated)) {
                            pStr += '<br />Last&nbsp;Updated:<br />' + FormatDate(vData.WholesalePriceLastUpdated, { Format: 'M/d/yyyy' });
                            pStr += '<br />' + AccountingFormat((new Date().getTime() - new Date(vData.WholesalePriceLastUpdated)) / 1000 / 60 / 60 / 24, 0) + ' days ago';
                        }
                        pStr = '<div style="text-align: center">' + pStr + '</div>';

                        return pStr;
                    }
                },
                {
                    Label: 'Price Update Request Reason',
                    Column: 'Remarks'
                },
                {
                    Label: 'Date Requested',
                    Column: 'DateRequested'
                },
                {
                    Label: 'Requested By',
                    Column: 'RequestedByName'
                }
            ];

            function ShowPopup(vLastModifiedFrom, vLastModifiedTo) {
                if (vLastModifiedFrom == null)
                    vLastModifiedFrom = new Date();
                if (vLastModifiedTo == null)
                    vLastModifiedTo = new Date();

                BrowsePopup('TransPriceUpdateRequest', 'Price Update Requests',
                    [
                        {
                            ModalSize: 'xl',
                            StyleFormat: 'table',
                            AutoSearch: true,
                            DisplayText: pColumns,
                            DontCloseWhenClick: true,
                            //Click: onRowClick,
                            //Validation: vParams.Validation,
                            ShowApprovedFilter: true,
                            rowCallback: function(row, data, dataIndex) {

                            },
                            Sorting: true,
                            NoInitialSorting: true,
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
                                            'width=' + window.innerWidth + ',height=' + window.innerHeight);

                                        win.onload = function () {
                                            function LoadRecord() {
                                                if (CInt2(data.InvoiceNumber) > 0)
                                                    win.location.href = '/Erp/Invoicing' + win.location.search;
                                                else if (CInt2(data.PurchaseOrderNo) > 0)
                                                    location.href = '/PurchaseOrder/Preview' + location.search;
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
                        PromptUserAccess('Allow Approval of Purchase Orders (EAS)', {
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

    ERP.PriceUpdateRequests.controller = ERP.PriceUpdateRequests.controller();
});