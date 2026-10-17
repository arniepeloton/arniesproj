define([
], function () {
    var pStyle = document.createElement('style');
    pStyle.innerHTML = `
        .bold {
            font-weight: bold;
        }
    `;
    document.head.appendChild(pStyle);

    CreateStringObject('ERP.VendorPriceHistory', null, function (vParams) {
        var mOut = {};

        mOut.show = function() {
            var pColumns = [
                {
                    Label: 'Values Has Been Updated',
                    Render: function(v1, v2, vData) {
                        return '<div class=class=form-check style="text-align: center;"><input type=checkbox class=form-check-input name=chkVendorPriceHistoryUpdated></div>';
                    },
                    className: 'dt-head-body-center'
                },
                {
                    Label: 'Vendor',
                    Column: 'CompanyName'
                },
                {
                    Label: 'Product No.',
                    Column: 'ProductNo'
                },
                {
                    Label: 'Product Name',
                    Column: 'ProductName',
                    Style: {
                        width: '300px'
                    }
                },
                {
                    Label: 'Trans Ref',
                    Column: 'FullTransNum'
                },
                {
                    Label: 'Previous Cost',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.PreviousPrice);
                    },
                },
                {
                    Label: 'Current Cost',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.CurrentPrice);
                    },
                },
                {
                    Label: 'History Creation Date',
                    Column: function(vDR) {
                        return CStr2(vDR.HistoryCreationDate);
                    },
                }
            ];

            function ShowPopup(vLastModifiedFrom, vLastModifiedTo) {
                if (vLastModifiedFrom == null)
                    vLastModifiedFrom = new Date();
                if (vLastModifiedTo == null)
                    vLastModifiedTo = new Date();

                BrowsePopup('TransVendorPriceHistory', 'Vendor Price History',
                    [
                        {
                            ModalSize: 'xl',
                            StyleFormat: 'table',
                            AutoSearch: true,
                            DisplayText: pColumns,
                            DontCloseWhenClick: true,
                            ShowLastModifiedFilter: true,
                            rowCallback: function(row, data, dataIndex) {

                            },
                            Sorting: true,
                            LastModifiedFrom: vLastModifiedFrom,
                            LastModifiedTo: vLastModifiedTo,
                            //OrderBy: [1, 'asc']
                            LastModifiedFilterChange: function(e) {
                                ShowPopup(e.From, e.To);
                            },
                            FixedHeader: true,
                            rowCallback: function(row, data, dataIndex) {
                                var pTR = $(row);
                                var pChk = pTR.find('[name=chkVendorPriceHistoryUpdated]')
                                pChk.prop('checked', CBool(data.Tagged));
                                pChk.change(function(e) {
                                    //var pChk = $(e.target);
                                    Tag(data.ID, pChk.prop('checked'));
                                });
                            },
                        }
                    ], { 
                        AnyZero: 1,
                        AllowNoItems: true,
                        //LastUpdatedDateFrom: FormatDate(vLastModifiedFrom, {Format: 'MMM d, yyyy'}),
                        //LastUpdatedDateTo: FormatDate(vLastModifiedTo, {Format: 'MMM d, yyyy'})
                        //LastUpdatedDateFrom: '2020/09/01', // for debugging only
                        //LastUpdatedDateTo: '2020/09/30' // for debugging only
                    }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            } // END function ShowPopup(vLastModifiedFrom, vLastModifiedTo) {

            ShowPopup();
        } // END mOut.show = function() {

        function Tag(vID, vTagged) {
            AjaxSend({
                url: 'ERP/TagVendorPriceHistory',
                data: {
                    ID: vID,
                    Tagged: vTagged ? 1 : 0
                },
                success: function(vData) {
                    if (vData.Result == 'ok')
                        WebToast('The record was ' + (vTagged ? '' : 'un') + 'tagged successfully.', null, 5000);
                }
            });
        }

        function onRowClick(vData) {
            NewMessageBox('', '', [
                {
                    Text: 'Approve Selling Price',
                    Click: function() {
                        AjaxSend({
                            url: '/ERP/ApproveProfitability',
                            data: {
                                InvoiceItemID: vData.ItemID
                            },
                            success: function(vData2) {
                                if (vData2.Result == 'ok')
                                    NewMessageBox('Line item profitability was successfully approved');
                                else
                                    NewMessageBox('Unexpected result. jsA5SHCCQMY6');
                            }
                        });
                    }
                },
                {
                    Text: 'Edit Selling Price and/or Qty',
                    Click: function() {
                        require([
                            'promise!/Scripts/sources/ERP/OrderProcessing/product/controller.js?v=' + assetversion
                        ], function() {
                            NewMessageBoxClose();
                            setTimeout(function() {
                                var pProductPopup = new ERP.OrderProcessing.product.controller();
                                pProductPopup.Model.Data.OrderID = vData.InvoiceID;
                                pProductPopup.Model.Data.NewCartMode = false;
                                pProductPopup.popup({
                                    //InventoryBalance: mInventoryBalance,
                                    ProductID: vData.ProductID,
                                    ProductName: vData.ProductName,
                                    ProductNo: vData.ProductNo,
                                    CustomerID: vData.CustomerID,
                                    TransType: 'SalesOrder',
                                    ParentController: mOut,
                                    IncludeApprove: true,
                                    Success: function() {
                                    }
                                });
                            }, 500);
                        });
                    }
                },
                {
                    Text: 'Remove Item From Order',
                    Click: function() {
                        NewMessageBox('Are you sure you want to remove ' + vData.ProductName + ' from cart and profitability dashboard?', 'Remove from cart', [
                            {
                                Text: 'Yes',
                                Click: function() {
                                    AjaxSend({
                                        url: '/SalesMenu/DeleteStockPos',
                                        data: {
                                            OrderID: vData.InvoiceID,
                                            ProductID: vData.ProductID,
                                            UnitMeasure: vData.UnitMeasure,
                                            ItemID: vData.ItemID
                                        },
                                        success: function(vData) {
                                            if (vData.Error)
                                                NewMessageBox(vData.Error)
                                            else if (vData.Result == 'ok') {
                                                NewMessageBox('Line item was successfully remove from cart and profitability dashboard.');
                                            } else
                                                NewMessageBox('jsRDGYYKH96B! Unexpected reponse');
                                        }
                                    });
                                }
                            },
                            {
                                Text: 'No'
                            }
                        ]);
                    }
                }
            ], {
                Close: function() {
                    mOut.show();
                }
            });
        }

        return mOut;
    });

    ERP.VendorPriceHistory = ERP.VendorPriceHistory(); // singleton

})