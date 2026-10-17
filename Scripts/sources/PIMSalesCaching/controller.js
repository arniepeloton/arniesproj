define([
    'promise!/Scripts/sources/PIMUpload/PIMUpload.js',
    '/Scripts/sources/ERP/Invoicing/InvoiceImage/controller.js?crc=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/PIMSalesCaching/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.PIMSalesCaching.controller = ERP.PIMSalesCaching.controller(); // singleton
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.PIMSalesCaching.controller', null, function (vParams) {

        var mOut = {};
        var mElement;
        var mDataTable;

        vParams = $.extend({
            TransType: null,
            Callback: function(){},
            ShowProductSuppliersColumn: false
        }, vParams);
        mElement = $(mHtml);
        mOut.element = mElement;

        mElement.find('[name=butAddProduct]').click(function() {
            AddProduct();
        });

        function AddProduct(vParams) {
            vParams = $.extend({
                AutoSearch: false,
                AllowEmptySearch: false
            }, vParams);
            var pColumns = [
                { 
                    Column: 'ProductNo', 
                    Label: 'Product No.',
                    Render: function(v1, v2, vData) {
                        return '<div style="text-align: center">' + CStr2(vData.ProductNo) + '</div>';
                    }
                },
                { 
                    Column: 'ProductName', 
                    Label: 'Product Name',
                    CreatedCell: function(td, cellData, rowData, row, col){
                        $(td).css('min-width', '200px');
                    },
                    Render: function(v1, v2, vData) {
                        return '<div style="text-align: center">' + CStr2(vData.ProductName) + '</div>';
                    }
                }
            ];
            var pAdditionalData = {
                IsAll:1,
                UseCacheProductSearch: 0,
                InventoryFowstID: sessionStorage.FowstID,
                FilterSupplierID: vParams.FilterSupplierID,
                IncludeBaseUnit: 1
            };

            
            pColumns = pColumns.concat([
                {
                    Label: 'Interval In Minutes',
                    Render: function(v1, v2, vData) {
                        return '<div name="divInterval" />';
                    }, DontCopyToPlainText: true
                }
            ]);

            pColumns.push({
                Label: 'Packing Size (in smallest unit)',
                Render: function(v1, v2, vData) {
                    return '<div style="text-align: center">' + CStr2(vData.UnitMeasure) + '</div>';
                }
            });
            pColumns.push({
                Label: 'Price Level',
                Column: null,
                CreatedCell: function(td, cellData, rowData, row, col){
                    var inp = $('<select data-role="none" class="form-select selPriceLevel">')
                                .append(new Option('Retail Price', '01000012-0005-0000-0000-000000000000'))
                                .data('data', rowData)
                                .click(function(e){
                                    e.stopPropagation();
                                    var dis = $(this), data = dis.data('data');

                                    if (!CBool(dis.attr('isloaded'))){
                                        Bs.Loading('show');

                                        AjaxSend({
                                            url: '/Stock/PriceTable',
                                            data: {ProductID: data.ID, CustomerID: mOut.Model.Data.CustomerID },
                                            success: function (e){
                                                Bs.Loading('hide');
                                                var vRows = e.Rows;

                                                for (var i = 0 in vRows){
                                                    if (dis.find('[value="'+ vRows[i].PriceLevelID +'"]').length == 0){
                                                        dis.append(new Option(vRows[i].PriceLevel, vRows[i].PriceLevelID))
                                                    }
                                                }

                                                dis.attr('isloaded', 1)
                                            }
                                        })
                                    }
                                })

                    $(td)
                        .css('min-width', '100px')
                        .click(function(e){
                            e.stopPropagation();
                        })
                        .html(inp)
                }
            });
            
            pAdditionalData.FilterCustomers = 1;
            pColumns.push({
                Label: 'Retail Price' ,
                Column: function(vData) {
                    var pStr = '<b>' + AccountingFormat(vData.RetailPrice, 4) + '</b>';
                    if (IsDate(vData.RetailPriceLastUpdated)) {
                        pStr += '<br />Last&nbsp;Updated:<br />' + FormatDate(vData.RetailPriceLastUpdated, { Format: 'M/d/yyyy' });
                        pStr += '<br />' + AccountingFormat((new Date().getTime() - new Date(vData.RetailPriceLastUpdated)) / 1000 / 60 / 60 / 24, 0) + ' days ago';
                    }
                    pStr += '<br /><a name="lnkRequestPriceUpdate" href="#">Request Price Update</a>';
                    pStr = '<div style="text-align: center">' + pStr + '</div>';

                    return pStr;
                }
            });
            pColumns.push({
                Label: 'Wholesale Price' ,
                Column: function(vData) {
                    var pStr = '<b>' + AccountingFormat(vData.WholesalePrice, 4) + '</b>';
                    if (IsDate(vData.WholesalePriceLastUpdated)) {
                        pStr += '<br />Last&nbsp;Updated:<br />' + FormatDate(vData.WholesalePriceLastUpdated, { Format: 'M/d/yyyy' });
                        pStr += '<br />' + AccountingFormat((new Date().getTime() - new Date(vData.WholesalePriceLastUpdated)) / 1000 / 60 / 60 / 24, 0) + ' days ago';
                    }
                    pStr += '<br /><a name="lnkRequestPriceUpdate" href="#">Request Price Update</a>';
                    pStr = '<div style="text-align: center">' + pStr + '</div>';
                    return pStr;
                }
            });

            pAdditionalData.LastInvCount = 1;
            pColumns.push({
                Label: 'Last Count Date / Counted By',
                Render: function(v1, v2, vData) {
                    return (CStr2(vData.LastInventoryCountDate) == '' ? '' : $.format.date(new Date(vData.LastInventoryCountDate), 'MM-dd-yyyy h:mm a') + ' / ' + vData.LastInventoryCountBy);
                }
            });
            pColumns.push({
                Label: 'Qty On Hand',
                Render: function(v1, v2, vData) {
                    var p = $.format.number(CDec2(vData.QtyOnHand), '#,###.##');
                    p = '<div style="text-align: center">' + p + '</div>';
                    return p;
                }, DontCopyToPlainText: true
            });
            pColumns.push({
                Label: 'Committed Qty',
                Render: function(v1, v2, vData) {
                    var p = $.format.number(CDec2(vData.CommittedQty), '#,###.##');
                    p = '<div style="text-align: center">' + p + '</div>';
                    return p;
                }, DontCopyToPlainText: true
            });
            pColumns.push({
                Label: 'Qty Available to Commit',
                Render: function(v1, v2, vData) {
                    var p = $.format.number(CDec2(vData.AvailableQtyToCommit), '#,###.##');
                    p = '<div style="text-align: center">' + p + '</div>';
                    return p;
                }, DontCopyToPlainText: true
            });
            pColumns.push(
                {
                    Label: 'Action',
                    Render: function(v1, v2, vData) {
                        return '<div name="divAction"></div>';
                    }, DontCopyToPlainText: true
                }
            );
            
            function CacheProductSearch() {
                var pData = {
                    keywords: $('#txtBrowseLibrary').val(),
                    FilterCustomers: 1,
                    IsDeactivated: 0,
                    CacheProductSearch: 1
                };
                AjaxSend({
                    url: 'Browser/LibraryPIMProduct',
                    data: pData
                });
            }

            BrowsePopup('LibraryPIMProduct', 'Product - ' + sessionStorage.FowstName,
                [
                    {
                        ModalSize: 'fullwidth',
                        AutoSearch: vParams.AutoSearch,
                        ShowOkButton: true,
                        DisplayText: pColumns,
                        CopyText: true,
                        DontCloseWhenClick: true,
                        ShowPic: true,
                        Click: function (e) {
                            if (!e.FromCacheProductSearch)
                                CacheProductSearch();
                            if ($.isArray(e)) {
                                PreSelect(e);
                            } else {
                                SelectProduct(e);
                            }
                        },
                        AllowEmptySearch: vParams.AllowEmptySearch,
                        StyleFormat: 'table',
                        InitComplete: function(vTable) {
                            vTable.find('.interval').click(function(e) {
                                e.stopPropagation();
                            });
                        },
                        rowCallback: function(row, data, dataIndex){
                            var pDiv = $(row).find('[name=divAction]');
                            if (pDiv.children().length > 0)
                                return;
                            var pLink = $('<a>')
                                .text('Info')
                                .attr('href', '#')
                                .click(function(e) {
                                    e.preventDefault();
                                    SelectProduct(data);

                                    //require(['promise!/Scripts/sources/ERP/InventoryBalance/controller.js?v=' + assetversion], function() {
                                    //    if (mInventoryBalance == null)
                                    //        mInventoryBalance = new ERP.InventoryBalance.controller();

                                    //    mInventoryBalance.Show({
                                    //        ProductID: data.ID,
                                    //        ProductNo: data.ProductNo,
                                    //        ProductName: data.ProductName,
                                    //        OrderController: mOut,
                                    //        AfterSave: function() {
                                    //            LoadTrans({
                                    //                ID: mOut.Model.Data.ID,
                                    //                LoadProductsOnly: true
                                    //            });
                                    //        }
                                    //    });
                                    //});
                                    return false;
                                });
                            pDiv.append(pLink);
                            pDiv = $(row).find('[name=divInterval]');
                            pDiv.click(function(e) {
                                e.preventDefault();
                                return false;
                            });
                            var pDisabled = '';
                            // commented on 2021_09_02__09_42_32_5538
                            //if (CDec2(data.RetailPrice) == 0)
                            //    pDisabled = ' disabled ';
                            pDiv.append('<input type=number min=0 oninput="this.value = Math.abs(this.value)" class="interval form-control" ' + pDisabled + ' style="width:70px" />');

                            pDiv = $(row).find('[name=divPoPrice]');
                            pDiv.click(function(e) {
                                e.preventDefault();
                                return false;
                            });
                            var pVal = CDec2(data.VendorPrice);
                            if (pVal != 0)
                                pVal = AccountingFormat(pVal, 4);
                            else
                                pVal = '';
                            pDiv.append('<input type=number min=0 onblur="this.value = Math.abs(this.value)" class="PoPrice form-control" style="width:90px" value="' + pVal + '" />');

                            $(row).find('[name=lnkRequestPriceUpdate]').click(function(e) {
                                e.stopPropagation();
                                PriceUpdateRequest(data);
                            });
                        }
                    }
                ], pAdditionalData
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        function PreSelect() {
            var pTxts = $('#tblBrowsePopup .interval');
            var pData = [];
            for (var p = 0; p <= pTxts.length - 1; p++) {
                var pTxt = $(pTxts[p]);
                var pInterval = CDec2(pTxt.val());
                if (pInterval > 0) {
                    var pTR = pTxt.closest('tr');
                    var pDR = pTR.data('data');
                    var pLineItem = {
                        ProductID: pDR.ID,
                        Interval: pInterval,
                    };
                    pData.push(pLineItem);
                } // todo: if qty = 0, delete at server
            }
            
            if (pData.length == 0) 
                return;
            pData = {
                Pim: pData
            };
            AjaxSend({
                url: '/PimProduct/SavePimSalesCaching',
                data: pData,
                success: function(vData) {
                    NewMessageBox('PIM Sales Caching was successfully saved.');
                    LoadData();
                }
            });
        }

        function DisplayData(vData) {
            if (mDataTable != null) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mElement.find('[name=table]').empty();
                mDataTable.destroy();
                mElement.find('[name=table] thead').empty(); // this is important
            }

            var pColumns = []

            pColumns.push(
                {
                    title: 'Product No.',
                    data: function (vDR) {
                        return CStr2(vDR.ProductNo);
                    }
                }
            );

            pColumns.push(
                {
                    title: 'Product Description.',
                    data: function (vDR) {
                        return CStr2(vDR.ProductName);
                    }
                }
            );

            pColumns.push(
                {
                    title: 'Interval In Minutes',
                    data: function (vDR) {
                        return CInt2(vDR.Interval);
                    }
                }
            );

            mDataTable = mElement.find('[name=table]');

            mDataTable = mDataTable.DataTable({
                //order: [[0, 'asc']], //vParams[0].OrderBy is array . use only in display table
                pageLength: 25,
                "ordering": true,
                paging: true,
                data: vData.Results.Rows,
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
                    { extend: 'csvHtml5', text: 'Download CSV', title: 'PIM Sales Caching' }
                ],
                sDom: '<"top"B>rt<"bottom"ip><"clear">',
                rowCallback: function(row, data, dataIndex){
                    
                }, initComplete: function () {
                    mElement.find('[name=table]').removeClass('dataTable');
                }
            })

        }

        function LoadData() {
            var pData = {
                GroupBy: $('[name=radGroup]:checked').val()
            };
            mGrouping2 = null;
            if (pData.GroupBy == 'Customer') 
                mGrouping = 'Customer';
            else if (pData.GroupBy == 'Sku') {
                mGrouping = 'Product Name';
                mGrouping2 = 'Product No.';
            }

            AjaxSend({
                url: '/Browser/PimSalesCaching',
                data: pData,
                success: function(vData) {
                    DisplayData(vData);
                }
            });
        }

        LoadData();

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});