define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/PurchaseOrder/PoBySkuItem/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.PurchaseOrder.PoBySkuItem.controller = ERP.PurchaseOrder.PoBySkuItem.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.PurchaseOrder.PoBySkuItem.controller', null, function () {

        var mOut = {};
        var mElement;
        var mItemRequisitionID;
        var mProductID;
        var mGrouping;
        var mOnOk;

        mElement = $(mHtml);
        $.mobile.activePage.append(mElement);
        var mDataTable;
        var mTable = mElement.find('[name=table]');
        var mTableBody = mTable.find('tbody');

        var mDataTableUnits;
        var mTableUnits = mElement.find('[name=tableUnits]');
        var mTableUnitsBody = mTable.find('tbody');

        mOut.popup = function(vParams) {
            mItemRequisitionID = vParams.ItemRequisitionID;
            mProductID = vParams.ProductID;
            mOnOk = vParams.OnOk;
            mElement.show();
            LoadData();
        };

        mElement.find('[type=radio]').checkboxradio().checkboxradio('refresh');
        mElement.find('[type=button]').button().button('refresh');
        mElement.find('[name=txtRequestedQty]').change(function() {
            RefreshIncludedBackOrderQty();
        });

        function LoadData() {
            var pData = {
                ItemRequisitionID: mItemRequisitionID,
                ProductID: mProductID
            };

            AjaxSend({
                url: '/SalesMenu/ItemRequisitionDetail',
                data: pData,
                success: function(vData) {
                    DisplayData(vData);
                }
            });
        }

        function DisplayData(vData) {
            var pDrProduct = vData.Product.Rows[0];
            mElement.find('[name=spanProductName]').text(pDrProduct.ProductName);
            mElement.find('[name=spanUnit]').text(pDrProduct.BaseUnit);

            var pDrItemRequisition = vData.ItemRequisition.Rows[0];
            mElement.find('[name=txtRequestedQty]').val(AccountingFormat(pDrItemRequisition.RequestedQty, 0));

            if (mDataTable != null) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mElement.find('[name=table]').empty();
                mDataTable.destroy();
            }

            var pColumns = [];

            pColumns.push(
                {
                    title: 'SI Date',
                    render: function (v1, vType, vDR) {
                        // test display 1st before else https://stackoverflow.com/a/45037039/334045
                        if (vType == 'display')
                            return FormatDate(vDR.InvoiceDate, {Format: 'MMM d, yyyy'});
                        return new Date(vDR.InvoiceDate);
                    },
                    className: 'dt-head-center'
                }
            );
            pColumns.push(
                {
                    title: 'SI No.',
                    render: function (v1, vType, vDR) {
                        var pOut;
                        pOut = CStr2(vDR.DocumentName).trim();
                        if (pOut == '')
                            pOut += 'SI';
                        pOut += ' ' + CStr2(vDR.FullInvoiceNumber);
                        return pOut;
                    },
                    className: 'dt-head-center'
                }
            );
            pColumns.push(
                {
                    title: 'Customer',
                    render: function (v1, vType, vDR) {
                        return CStr2(vDR.CompanyName);
                    },
                    className: 'dt-head-center'
                }
            );

            pColumns.push(
                {
                    title: 'Back Order Qty',
                    render: function (v1, vType, vDR) {
                        return AccountingFormat(vDR.Qty, 0);
                    },
                    className: 'dt-body-right dt-head-center'
                }
            );

            pColumns.push(
                {
                    title: 'Encoded By',
                    render: function (v1, vType, vDR) {
                        return CStr2(vDR.EncodedBy);
                    },
                    className: 'dt-head-center'
                }
            );

            pColumns.push(
                {
                    title: 'SO\'s FOWST Source',
                    render: function (v1, vType, vDR) {
                        return CStr2(vDR.FowstName);
                    },
                    className: 'dt-head-center'
                }
            );

            pColumns.push(
                {
                    title: 'Include Row',
                    render: function (v1, vType, vDR) {
                        return '<div name=divInclude />';
                    },
                    className: 'dt-body-center dt-head-center'
                }
            );

            mDataTable = mElement.find('[name=table]');

            mDataTable = mDataTable.DataTable({
                //order: [[0, 'asc']], //vParams[0].OrderBy is array . use only in display table
                pageLength: 25,
                "ordering": true,
                paging: true,
                data: vData.BackOrders.Rows,
                columns: pColumns,
                initComplete: function () {
                    //PopupReposition()
                },
                "aaSorting": [],
                order: [],
                buttons: [
                    {
                        text: 'Refresh',
                        action: function (e, dt, node, config) {
                            LoadData();
                        }
                    },
                    { extend: 'csvHtml5', text: 'Download CSV', title: 'Profitability Report' }
                ],
                sDom: '<"top"B>rt<"bottom"ip><"clear">',
                rowCallback: function(row, data, dataIndex){
                    var pTR = $(row);
                    pTR.data(data);
                    var pDiv = pTR.find('[name=divInclude]');
                    var pChk = $('<input type=checkbox name=chkInclude>');
                    pChk.prop('checked', CBool(data.IncludeRow));
                    pChk.change(function() {
                        RefreshIncludedBackOrderQty();
                    });
                    pTR.find('[name=divInclude]').append(pChk);
                    //var pLinks = $(row).find('[name^=NetSales-]');
                    //for (var pLinkIndex = 0; pLinkIndex <= pLinks.length - 1; pLinkIndex++) {
                    //    var pLink = pLinks[pLinkIndex];
                    //    pLink = $(pLink);
                    //    (function(vLink) {
                    //        var pDashIndex = vLink.attr('name').indexOf('-');
                    //        var pYear = vLink.attr('name').substr(pDashIndex + 1);
                    //        var pParams = {
                    //            InvoiceYear: pYear
                    //        };
                    //        if (mGrouping == 'Customer')
                    //            pParams.CustomerID = data.GroupingID;
                    //        else
                    //            pParams.ProductID = data.GroupingID;
                    
                    //        vLink.click(function() {
                    //            ShowSkuDetails(pParams);
                    //        });
                    //    })(pLink);
                        
                    //}
                    
                }
            });

            RefreshIncludedBackOrderQty();
        }

        function LoadUnits(vData) {
            if (mDataTableUnits != null) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mElement.find('[name=tableUnits]').empty();
                mDataTableUnits.destroy();
            }

            var pColumns = [];

            pColumns.push(
                {
                    title: '',
                    render: function (v1, vType, vDR) {
                        // test display 1st before else https://stackoverflow.com/a/45037039/334045
                        return CStr2(vDR.UnitMeasure);
                    },
                    className: 'dt-head-center'
                }
            );
            pColumns.push(
                {
                    title: '',
                    render: function (v1, vType, vDR) {
                        return AccountingFormat(vDR.Quantity, 0) + ' ';// + mElement.find('[name=;
                    },
                    className: 'dt-head-center'
                }
            );
            pColumns.push(
                {
                    title: 'Customer',
                    render: function (v1, vType, vDR) {
                        return CStr2(vDR.CompanyName);
                    },
                    className: 'dt-head-center'
                }
            );

            pColumns.push(
                {
                    title: 'Back Order Qty',
                    render: function (v1, vType, vDR) {
                        return AccountingFormat(vDR.Qty, 0);
                    },
                    className: 'dt-body-right dt-head-center'
                }
            );

            pColumns.push(
                {
                    title: 'Encoded By',
                    render: function (v1, vType, vDR) {
                        return CStr2(vDR.EncodedBy);
                    },
                    className: 'dt-head-center'
                }
            );

            pColumns.push(
                {
                    title: 'SO\'s FOWST Source',
                    render: function (v1, vType, vDR) {
                        return CStr2(vDR.FowstName);
                    },
                    className: 'dt-head-center'
                }
            );

            pColumns.push(
                {
                    title: 'Include Row',
                    render: function (v1, vType, vDR) {
                        return '<div name=divInclude />';
                    },
                    className: 'dt-body-center dt-head-center'
                }
            );

            mDataTable = mElement.find('[name=table]');

            mDataTable = mDataTable.DataTable({
                //order: [[0, 'asc']], //vParams[0].OrderBy is array . use only in display table
                pageLength: 25,
                "ordering": true,
                paging: true,
                data: vData.BackOrders.Rows,
                columns: pColumns,
                initComplete: function () {
                    //PopupReposition()
                },
                "aaSorting": [],
                order: [],
                buttons: [
                    {
                        text: 'Refresh',
                        action: function (e, dt, node, config) {
                            LoadData();
                        }
                    },
                    { extend: 'csvHtml5', text: 'Download CSV', title: 'Profitability Report' }
                ],
                sDom: '<"top"B>rt<"bottom"ip><"clear">',
                rowCallback: function(row, data, dataIndex){
                    var pTR = $(row);
                    pTR.data(data);
                    var pDiv = pTR.find('[name=divInclude]');
                    var pChk = $('<input type=checkbox name=chkInclude>');
                    pChk.prop('checked', CBool(data.IncludeRow));
                    pChk.change(function() {
                        RefreshIncludedBackOrderQty();
                    });
                    pTR.find('[name=divInclude]').append(pChk);
                    //var pLinks = $(row).find('[name^=NetSales-]');
                    //for (var pLinkIndex = 0; pLinkIndex <= pLinks.length - 1; pLinkIndex++) {
                    //    var pLink = pLinks[pLinkIndex];
                    //    pLink = $(pLink);
                    //    (function(vLink) {
                    //        var pDashIndex = vLink.attr('name').indexOf('-');
                    //        var pYear = vLink.attr('name').substr(pDashIndex + 1);
                    //        var pParams = {
                    //            InvoiceYear: pYear
                    //        };
                    //        if (mGrouping == 'Customer')
                    //            pParams.CustomerID = data.GroupingID;
                    //        else
                    //            pParams.ProductID = data.GroupingID;
                    
                    //        vLink.click(function() {
                    //            ShowSkuDetails(pParams);
                    //        });
                    //    })(pLink);
                        
                    //}
                    
                }
            });
        }

        function RefreshIncludedBackOrderQty() {
            var pTotal = 0;
            var pChecked = $.makeArray(mElement.find('[name=chkInclude]:checked'));
            for (var pChk of pChecked) {
                pChk = $(pChk);
                var pTR = $(pChk.closest('tr'));
                var pDR = pTR.data();
                pTotal += CDec2(pDR.Qty);
            }
            mElement.find('[name=spanBackOrderQty]').text(AccountingFormat(pTotal, 0));
            mElement.find('[name=spanTotalQty]').text(CDec2(mElement.find('[name=txtRequestedQty]').val()) + pTotal);
        }

        mElement.find('[name=butSave]').click(function() {
            var pBackOrders = [];
            var pChecked = $.makeArray(mElement.find('[name=chkInclude]'));
            for (var pChk of pChecked) {
                pChk = $(pChk);
                var pTR = $(pChk.closest('tr'));
                var pDR = pTR.data();
                var pBackOrder = {
                    ID: pDR.ID,
                    IncludeRow: pChk.prop('checked'),
                    Qty: CDec2(pDR.Qty)
                };
                pBackOrders.push(pBackOrder);
            }
            var pData = {
                ID: mItemRequisitionID,
                RequestedQty: CDec2(mElement.find('[name=txtRequestedQty]').val()),
                RequestedQtyTotal: CDec2(mElement.find('[name=spanTotalQty]').text()),
                BackOrders: pBackOrders
            }
            AjaxSend({
                url: '/SalesMenu/SaveItemRequisitionDetail',
                data: pData,
                success: function(vData) {
                    if (vData.Error) {
                        NewMessageBox(vData.Error, 'Failed');
                    } else if (vData.Result == 'ok') {
                        NewMessageBox('Data was saved successfully', 'Success', null, {
                            Close: function() {
                                mOnOk();
                                mElement.hide();
                            }
                        });
                        return;
                    }
                }
            });
        });

        mElement.find('[name=butBack]').click(function() {
            mElement.hide();
        });

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});