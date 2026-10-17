define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Invoicing/dashboard/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.Invoicing.dashboard.controller = ERP.Invoicing.dashboard.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.Invoicing.dashboard.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;
        if (!CBool(GetUrlParameters().IsReadOnly)) {
            //LoadData(); // 2023_05_03__16_43_47_5955
            //LoadDataDropShip(); // 2023_05_03__16_43_47_5955
        }

        mElement.find('#butInvoiceDashboardInvoicableOrders').click(function () {
            ShowInvoicableOrders(false);
            //PromptUserAccess('Reports (EAS)', {
            //    OnClickOKSuccess: function () {
                    
            //    },
            //    OnFailed: function () {
            //        console.log('jsCFDTPD3GXC!');
            //    }
            //});
        });
        
        mElement.find('#butInvoiceDashboardDropShip').click(function () {
            ShowInvoicableOrders(true);
        });
        
        function LoadData() {
            AjaxSend({
                url: '/Browser/TransInvoices',
                data: {
                    FilterSalesOrders: 1,
                    ExcludeInvoiced: 1,
                    CountOnly: 1,
                    FinalizedFcOnly: 1,
                    InvoiceFowstID: sessionStorage.FowstID,
                    NonZeroServedSkuCount: true
                },
                success: function(vData) {
                    mElement.find('#lblInvoiceDashboardInvoicableOrders').text(vData.Result);
                }
            });
        }
        
        function LoadDataDropShip() {
            AjaxSend({
                url: '/Browser/TransInvoices',
                data: {
                    FilterSalesOrders: 1,
                    ExcludeInvoiced: 1,
                    CountOnly: 1,
                    DisplayUnservedCount: 1,
                    InvoiceFowstID: sessionStorage.FowstID
                },
                success: function(vData) {
                    mElement.find('#lblInvoiceDashboardDropShip').text(vData.Result);
                }
            });
        }

        function ShowInvoicableOrders(vDropShip) {
            require(['promise!/Scripts/sources/ERP/DropShipOrders/controller.js?crc=' + assetversion], function () {
                ERP.DropShipOrders.popup({
                    dropShip: vDropShip
                });
            })
            return;
            var pTitle = 'Sales Orders Ready for Invoicing';
            var pColumns = [
                {
                    Label: 'Sales Order No.',
                    Column: function(vDR) {
                        var pOut = CStr2(vDR.MIN).trim();
                        if (pOut != '')
                            pOut = vDR.MIN + '-';
                        if (vDR.TransType == 'StockTransfer')
                            pOut = 'STR-' + pOut + vDR.StrNum;
                        else
                            pOut = 'SO-' + pOut + vDR.SalesOrderNo;
                            
                        return pOut;
                    }
                },
                {
                    Label: 'Parent Sales Order No.',
                    Column: function(vDR) {
                        return CStr2(vDR.ParentSalesOrderNo);
                    }
                },
                {
                    Label: 'Serving Location',
                    Column: 'FOWSTName'
                },
                {
                    Label: 'Fulfillment Status',
                    Column: 'FulfillmentStatus'
                },
                {
                    Label: 'Order Entry Location',
                    Column: function(vDR) {
                        if (CStr2(vDR.EncodingFOWSTName).trim() == '')
                            return vDR.FOWSTName;
                        else
                            return vDR.EncodingFOWSTName;
                    }
                },
                {
                    Label: 'Customer',
                    Column: 'CompanyName'
                },
                {
                    Label: 'Ordered Amount',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.OrderedAmount);
                    }
                },
                {
                    Label: 'Served Amount',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.ServedAmount);
                    }
                }
            ];

            pColumns = pColumns.concat([
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
            ]);

            BrowsePopup('TransInvoices', pTitle,
                [
                    {
                        StyleFormat: 'table',
                        AutoSearch: true,
                        DisplayText: pColumns,
                        Click: function() {},
                        Validation: false,
                        ShowHideChildSos: false,
                        rowCallback: function(row, data, dataIndex) {
                            return false; // para DILI magclose ang popup
                        },
                        Sorting: false,
                        TopText: 'All related orders that are “Saved as Final” by the Fulfillment Center (FC) staff will be merged into one invoice. Orders that are not yet “Saved as Final” will not be included in the invoice.  You may invoice orders that are not included in the merged invoice separately at a later time.'
                        //OrderBy: [1, 'asc']
                    }
                ], $.extend({ 
                    TransType: 'SalesOrder',
                    AnyZero: 1,
                    AllowNoItems: true,
                    FinalizedFcOnly: true
                }, {})
            ); // END BrowsePopup('LibraryFowst', 'Fowst',

        }

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();
});