define([
], function () {
    var pStyle = document.createElement('style');
    pStyle.innerHTML = `
        .bold {
            font-weight: bold;
        }
    `;
    document.head.appendChild(pStyle);

    CreateStringObject('ERP.ProfitabilityValidation', null, function (vParams) {
        var mOut = {};

        mOut.show = function() {
            var pColumns = [
                {
                    Label: 'Transaction Reference',
                    Column: 'TransMin'
                },
                {
                    Label: 'Transaction Date',
                    Column: 'TransDate'
                },
                {
                    Label: 'Customer',
                    Column: 'CompanyName',
                    Style: {
                        width: '300px'
                    }
                },
                {
                    Label: 'Original Transaction Created By',
                    Column: 'CreatedByName'
                },
                {
                    Label: 'SKU Description',
                    Column: 'ProductName'
                },
                {
                    Label: 'Order Qty',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.OrderQuantity);
                    },
                },
                {
                    Label: 'Unit Of Measure',
                    Column: 'UnitMeasure'
                },
                {
                    Label: 'COGS',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.COGS);
                    },
                },
                {
                    Label: 'COGS Last Updated',
                    Column: function(vDR) {
                        return CStr2(vDR.CogsLastUpdated);
                    },
                },
                {
                    Label: 'Selling Price',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.SellingPrice);
                    },
                },
                {
                    Label: 'Is Selling Price Free-Typed?',
                    Column: function(vDR) {
                        return CBool(vDR.IsPriceFreeTyped) ? 'Yes' : 'No';
                    },
                },
                {
                    Label: 'System Selling Price Last Updated (Null if Free-Typed)',
                    Column: function(vDR) {
                        return vDR.SellingPriceLastUpdated || '';
                    },
                },
                {
                    Label: 'System Selling Price Level (Null if Free-Typed)',
                    Column: function(vDR) {
                        return vDR.SellingPriceLevel || '';
                    },
                },
                {
                    Label: 'Prevailing Minimum GPM Threshold',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.GpmThreshold) + '%';
                    },
                },
                {
                    Label: 'Unapproved Selling Price\'s Unit Gross Profit',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.GP);
                    },
                },
                {
                    Label: 'Unapproved Selling Price\'s Unit Gross Profit Margin',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.GPM) + '%';
                    },
                },
                {
                    Label: 'Unapproved Selling Price\'s Line Gross Profit',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.LineGp);
                    },
                },
                {
                    Label: 'Unapproved Selling Price\'s Total Transaction Gross Profit',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.UnapprovedTransTotalGp);
                    },
                    className: 'bold'
                },
                {
                    Label: 'Unapproved Selling Price\'s Total Transaction Gross Profit Margin',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.UnapprovedTransTotalGpm) + '%';
                    },
                    className: 'bold'
                },
                {
                    Label: 'Approved Selling Price',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.ApprovedSellingPrice);
                    },
                },
                {
                    Label: 'Approved Unit Gross Profit',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.ApprovedGp);
                    },
                },
                {
                    Label: 'Approved Unit Gross Profit Margin',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.ApprovedGpm) + '%';
                    },
                },
                {
                    Label: 'Approved Line Gross Profit',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.ApprovedLineGp);
                    },
                },
                {
                    Label: 'Approved Selling Price\'s Total Transaction Gross Profit',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.ApprovedTransTotalGp);
                    },
                },
                {
                    Label: 'Approved Selling Price\'s Total Transaction Gross Profit Margin',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.ApprovedTransTotalGpm) + '%';
                    },
                },
                {
                    Label: 'Selling Price Approver',
                    Column: function(vDR) {
                        return CStr2(vDR.ApprovedByName);
                    },
                },
                {
                    Label: 'Selling Price Approval Date Time',
                    Column: function(vDR) {
                        return CStr2(vDR.ApprovedDate);
                    },
                },
                {
                    Label: 'Transaction # of Line Items',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.TransItemCount, 0);
                    },
                }
            ];

            function ShowPopup(vLastModifiedFrom, vLastModifiedTo) {
                if (vLastModifiedFrom == null)
                    vLastModifiedFrom = new Date();
                if (vLastModifiedTo == null)
                    vLastModifiedTo = new Date();

                BrowsePopup('TransProfitability', 'Gross Profitability Validation Dashboard',
                    [
                        {
                            ModalSize: 'xl',
                            StyleFormat: 'table',
                            AutoSearch: true,
                            DisplayText: pColumns,
                            Click: onRowClick,
                            //Validation: vParams.Validation,
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
                            FixedHeader: true
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
                                    TransType: vData.TransType || 'SalesOrder',
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

    ERP.ProfitabilityValidation = ERP.ProfitabilityValidation(); // singleton

})