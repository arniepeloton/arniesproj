define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/PurchaseOrder/poBySku/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                //ERP.PurchaseOrder.poBySku.controller = ERP.PurchaseOrder.poBySku.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.PurchaseOrder.poBySku.controller', null, function () {

        var mOut = {};
        var mElement;
        var mData = [];
        var mSelectedTr;
        var mFowstID = sessionStorage.FowstID;
        var mExpanded = false;
        var mProductID;

        mElement = $(mHtml);
        mOut.element = mElement;
        mOut.ItemRequisitionMode = false;

        var mTable = mElement.find('[name=myTable]');
        var mTableBody = mTable.find('tbody');
        var mTr;

        //mElement.find('[name=butSave]').click(function() {
        //    mOut.Model.Save({
        //        AllowEncodingOfOrdersInInvoice: mChkAllowEncodingOfOrdersInInvoice.prop('checked'),
        //        AllowEncodingOfQuantitiesInInvoice: mChkAllowEncodingOfQuantitiesInInvoice.prop('checked'),
        //        Success: function() {
        //            NewMessageBox('Settings were successfully saved.');
        //        }
        //    });
        //});

        mOut.Initialize = function(vParams) {
            vParams = $.extend({
                ProductID: null
            }, vParams);

            mProductID = vParams.ProductID;
            mTable.hide();
            if (mOut.ItemRequisitionMode)
                mElement.find('[name=butNew],[name=butSaveAsDraft]').hide();
            else {
                mFowstID = '';
                mElement.find('#butPoBySkuFowst')
                    .val('[All FOWSTs]');
            }
            DisplayData();
            LoadData();
        };

        function LoadData() {
            mElement.find('[name=divLoading]').show();
            mTable.hide();
            AjaxSend({
                url: '/Browser/TransItemRequisitions',
                data: {
                    SelectedFowstID: mFowstID,
                    ProductID: mProductID,
                    //DateRequested: mElement.find('#butPoBySkuFowstDateRequested').text(),
                    WeeksAgoRequested: mElement.find('[name=selPoBySkuFowstDateRequested]').val(),
                    Deactivated: mElement.find('[name=selActive]').val(),
                    keywords: mElement.find('#txtPoBySkuFilter').val().trim(),
                    PoFilter: mElement.find('[name=selPoFilter]').val(),
                    MaxRecords: mElement.find('[name=selMaxRecord]').val(),
                    Expanded: mExpanded
                },
                minimumDuration: 1000,
                success: function(vData) {
                    DisplayData(vData.Results);
                    mElement.find('[name=divLoading]').hide();
                    mTable.show();
                }
            });
        }

        mElement.find('#butPoBySkuFowst')
            .val(sessionStorage.FowstName)
            .click(DisplayFowst);

        function DisplayFowst() {
            BrowsePopup('LibraryFowst', 'Select FOWST', [
                {
                    AutoSearch: true,
                    DisplayText: [{ Column: 'FowstName' }],
                    Click: function(e) {
                        mFowstID = e.ID;
                        $('#butPoBySkuFowst').val(e.FowstName);
                        LoadData();
                    }
                }
            ], {
                IsAll: true,
                IncludeAllFowstOptions: 1
            });
        }

        mElement.find('#butPoBySkuFowstDateRequested')
            .text(FormatDate(new Date()))
            .click(function() {
                SingleCalendar({
                    Click: function (e) {
                        mElement.find('#butPoBySkuFowstDateRequested')
                            .text(FormatDate(e));
                        LoadData();
                    }
                })
            });
        mElement.find('#txtPoBySkuFilter').keypress(function (e) {
            if (e.keyCode == 13) {
                LoadData();
            }
        });

        mElement.find('#butPoBySkuAddProduct').click(AddProduct);

        mElement.find('#butPoBySkuExpand').click(function() {
            mExpanded = !mExpanded;
            if (mExpanded)
                mElement.find('#butPoBySkuExpand').text('Collapse to merge backorder quantities');
            else
                mElement.find('#butPoBySkuExpand').text('Expand to show backorders in separate rows');
            LoadData();
        });

        function DisplayData(vData) {
            
            //if (s != undefined) {
            //    tblSysAdminDataTable.search($.trim(s)).draw();
            //    return;
            //}

            var pData = {
                columns: []
            };

            pData.columns.push(
                {
                    title: 'Deactivate',
                    "data": function (data, type, full, meta) {
                        if (CStr2(data.CreatedPurchaseOrderID) != '')
                            return '';
                        return '<a name="delete" href="#">Deactivate</a>';
                    },
                    "width": "30px",
                    className: 'dt-head-center'
                }
            );

            pData.columns.push(
                {
                    title: 'Product #',
                    "data": function (data, type, full, meta) {
                        if (vData == null)
                            return '<a name="productNumber" href="#">Select product</a>';
                        if (CStr2(data.CreatedPurchaseOrderID) != '')
                            return '<span>' + HtmlEncode(data.ProductNo) + '</span>';
                        else
                            return '<a name="productNumber" href="#">' + HtmlEncode(data.ProductNo) + '</a>';
                    },
                    "width": "100px",
                    className: 'dt-head-center'
                }
            );

            pData.columns.push(
                {
                    title: 'Product Name',
                    "data": function (data, type, full, meta) {
                        if (vData == null)
                            return '<a name="product" href="#">Select product</a>';
                        if (CStr2(data.CreatedPurchaseOrderID) != '')
                            return '<span>' + HtmlEncode(data.ProductName) + '</span>';
                        else
                            return '<a name="product" href="#">' + HtmlEncode(data.ProductName) + '</a>';
                    },
                    "width": "500px",
                    className: 'dt-head-center'
                }
            );

            if (mOut.ItemRequisitionMode) {
                pData.columns.push(
                    {
                        title: 'Manual Requested Qty',
                        "data": function (data, type, full, meta) {
                            if (vData == null)
                                return '<input class="form-control" name=qty style="width:60px" />';
                            else if (CStr2(data.CreatedPurchaseOrderID) != '')
                                return '<span style="width:60px">' + AccountingFormat(data.RequestedQty, 0) + '</span>';
                            else
                                return '<input class="form-control" name=qty style="width:60px;text-align:center" value=' + AccountingFormat(data.RequestedQty, 0) + ' />';
                        },
                        "width": "80px",
                        className: 'dt-head-center dt-body-center'
                    }
                );

                pData.columns.push(
                    {
                        title: 'Total Back Order Qty',
                        "data": function (data, type, full, meta) {
                            if (vData == null)
                                return '<span style="width:60px" />';
                            else
                                return '<span style="width:60px">' + AccountingFormat(data.BackorderQty, 0) + '</span>';
                        },
                        "width": "80px",
                        className: 'dt-head-center dt-body-center'
                    }
                );

                pData.columns.push(
                    {
                        title: 'Total Back Order Qty and Requested Qty',
                        "data": function (data, type, full, meta) {
                            if (vData == null)
                                return '<span style="width:60px" />';
                            else
                                return '<span style="width:60px">' + AccountingFormat(data.RequestedQtyTotal, 0) + '</span>';
                        },
                        "width": "80px",
                        className: 'dt-head-center dt-body-center'
                    }
                );
            } else {
                pData.columns.push(
                    {
                        title: 'Manual Requested Qty',
                        "data": function (data, type, full, meta) {
                            if (vData == null)
                                return '<span style="width:60px" />';
                            else
                                return '<span style="width:60px">' + AccountingFormat(data.RequestedQty, 0) + '</span>';
                        },
                        "width": "80px",
                        className: 'dt-head-center dt-body-center'
                    }
                );

                pData.columns.push(
                    {
                        title: 'Total Back Order Qty',
                        "data": function (data, type, full, meta) {
                            if (vData == null)
                                return '<span style="width:60px" />';
                            else
                                return '<span style="width:60px">' + AccountingFormat(data.BackorderQty, 0) + '</span>';
                        },
                        "width": "80px",
                        className: 'dt-head-center dt-body-center'
                    }
                );

                pData.columns.push(
                    {
                        title: 'Total Back Order Qty and Requested Qty',
                        "data": function (data, type, full, meta) {
                            if (vData == null)
                                return '<span style="width:60px" />';
                            else
                                return '<span style="width:60px">' + AccountingFormat(data.RequestedQtyTotal, 0) + '</span>';
                        },
                        "width": "80px",
                        className: 'dt-head-center dt-body-center'
                    }
                );

                pData.columns.push(
                    {
                        title: 'Customer',
                        "data": function (data, type, full, meta) {
                            if (vData == null)
                                return '<span style="width:60px" />';
                            else
                                return '<span style="widthX:60px">' + CStr2(data.CustomerName) + '</span>';
                        },
                        //"width": "80px",
                        className: 'dt-head-center dt-body-center'
                    }
                );

                pData.columns.push(
                    {
                        title: 'PO Qty',
                        "data": function (data, type, full, meta) {
                            if (vData == null)
                                return '<input class="form-control" name=qty style="width:60px;text-align:center" />';
                            else if (CStr2(data.CreatedPurchaseOrderID) != '')
                                return '<span style="width:60px;text-align:center"' + AccountingFormat(data.RequestedQtyTotal, 0) + '</span>';
                            else
                                return '<input class="form-control" name=qty style="width:60px;text-align:center" value=' + AccountingFormat(data.RequestedQtyTotal, 0) + ' />';
                        },
                        "width": "80px",
                        className: 'dt-head-center dt-body-center'
                    }
                );
            }
            pData.columns.push(
                {
                    title: 'Unit Measure',
                    "data": function (data, type, full, meta) {
                        var pText;
                        if (vData != null)
                            pText = HtmlEncode(data.UnitMeasure);

                        if (CStr2(pText).trim() == '')
                            pText = 'Select unit measure';
                            
                        if (CStr2(data.CreatedPurchaseOrderID) != '')
                            return '<span>' + HtmlEncode(pText) + '</span>';
                        return '<a name="unit" href="#">' + HtmlEncode(pText) + '</a>';
                    },
                    "width": "100px",
                    className: 'dt-head-center dt-body-center'
                }
            );
            
            if (mOut.ItemRequisitionMode)
                pData.columns.push(
                    {
                        title: 'Requested By',
                        "data": function (data, type, full, meta) {
                            var pText = '';
                            if (vData != null)
                                pText = HtmlEncode(data.RequesterName);
                            
                            if (CStr2(data.CreatedPurchaseOrderID) != '')
                                return '<span>' + HtmlEncode(pText) + '</span>';

                            if (CStr2(pText).trim() == '')
                                pText = 'Requested By';
                            return '<a name="requestedBy" href="#">' + HtmlEncode(pText) + '</a>';

                        },
                        "width": "300px",
                        className: 'dt-head-center'
                    }
                );
            else
                pData.columns.push(
                    {
                        title: 'Requested By',
                        "data": function (data, type, full, meta) {
                            var pText;
                            if (vData != null)
                                pText = HtmlEncode(data.RequesterName);

                            if (CStr2(pText).trim() == '')
                                pText = '';
                            
                            return '<span>' + HtmlEncode(pText) + '</span>';

                        },
                        "width": "300px",
                        className: 'dt-head-center'
                    }
                );
            pData.columns.push(
                {
                    title: 'Request Encoded By',
                    "data": function (data, type, full, meta) {
                        var pText;
                        if (vData != null)
                            pText = HtmlEncode(data.EncoderName);

                        if (CStr2(pText).trim() == '')
                            pText = '';
                            
                        return '<span>' + HtmlEncode(pText) + '</span>';

                    },
                    "width": "300px",
                    className: 'dt-head-center'
                }
            );
            if (mOut.ItemRequisitionMode)
                pData.columns.push(
                    {
                        title: 'Requisition Remarks',
                        "data": function (data, type, full, meta) {
                            if (vData == null)
                                return '<textarea class="form-control" name=remarks style="width:100px;height:60px" />';
                            else if (CStr2(data.CreatedPurchaseOrderID) != '')
                                return '<span style="width:100px">' + HtmlEncode(CStr2(data.Remarks)) + '</span>';
                            else
                                return '<textarea class="form-control" name=remarks style="width:100px;height:60px">' + HtmlEncode(CStr2(data.Remarks)) + '</textarea>';
                        },
                        "width": "80px",
                        className: 'dt-head-center'
                    }
                );
            else
                pData.columns.push(
                    {
                        title: 'Requisition Remarks',
                        "data": function (data, type, full, meta) {
                            if (vData == null)
                                return '';
                            else
                                return '<span style="width:200px">' + HtmlEncode(CStr2(data.Remarks)) + '</span>';
                        },
                        "width": "80px",
                        className: 'dt-head-center'
                    }
                );
            if (!mOut.ItemRequisitionMode)
                pData.columns.push(
                    {
                        title: 'Vendor',
                        "data": function (data, type, full, meta) {
                            var pText;
                            if (vData != null)
                                pText = HtmlEncode(CStr2(data.Vendor));

                            if (CStr2(pText).trim() == '')
                                pText = 'Select vendor';

                            if (CStr2(data.CreatedPurchaseOrderID) != '')
                                return '<span>' + pText + '</span>';
                            return '<a name=vendor href="#">' + pText + '</a>';
                        },
                        "width": "300px",
                        className: 'dt-head-center'
                    }
                );
            if (!mOut.ItemRequisitionMode)
                pData.columns.push(
                    {
                        title: 'Vendor Price',
                        "data": function (data, type, full, meta) {
                            return '<span name=price />';
                        },
                        "width": "50px",
                        className: 'dt-body-right dt-head-center'
                    }
                );
            if (!mOut.ItemRequisitionMode)
                pData.columns.push(
                    {
                        title: 'Last Update of Vendor Price ',
                        "data": function (data, type, full, meta) {
                            return '<span name=lastUpdated />';
                            //return  FormatDate(new Date(), {Format: 'MMM d, yyyy h:mm a'});
                        },
                        "width": "150px",
                        className: 'dt-head-center'
                    }
                );
            if (!mOut.ItemRequisitionMode)
                pData.columns.push(
                    {
                        title: 'Payment Terms',
                        "data": function (data, type, full, meta) {
                            return '<span name=paymentTerm />';
                        },
                        "width": "30px",
                        className: 'dt-head-center'
                    }
                );
            pData.columns.push(
                {
                    title: 'Date Requested',
                    "data": function (data, type, full, meta) {
                        return  FormatDate(data.DateRequested, {Format: 'MMM d, yyyy h:mm a'});
                    },
                    "width": "50px",
                    className: 'dt-head-center'
                }
            );
            pData.columns.push(
                {
                    title: 'Requested Year|Week',
                    "data": function (data, type, full, meta) {
                        return CStr2(data.ComputedRequestedYearWeek);
                    },
                    "width": "70px",
                    className: 'dt-head-center'
                }
            );
            pData.columns.push(
                {
                    title: 'Requesting FOWST',
                    "data": function (data, type, full, meta) {
                        if (CStr2(data.FowstName) == '')
                            return '';
                        else
                            return data.FowstName;
                    },
                    "width": "70px",
                    className: 'dt-head-center'
                }
            );
            pData.columns.push(
                {
                    title: 'Created PO',
                    "data": function (data, type, full, meta) {
                        if (CStr2(data.FullPurchaseOrderNumber) == '')
                            return '';
                        else {
                            var pStr = 'PO ' + data.FullPurchaseOrderNumber;
                            if (data.PurchaseOrderDate)
                                pStr += '<br />' +
                                    FormatDate(data.PurchaseOrderDate, { Format: 'MMM d, yyyy' });
                            return pStr;
                        }
                    },
                    "width": "70px",
                    className: 'dt-head-center'
                }
            );

            if (vData == null)
                pData.data = [{}];
            else
                pData.data = vData.Rows;

            SysAdminDataTable(pData);
        }

        mTableBody.on('click', '[name=productNumber],[name=product]', null, function(e) {
            var pLink = $(e.target);
            mSelectedTr = pLink.closest('tr');
            if (mSelectedTr.data('data') == null)
                mSelectedTr.data('data', {});
            AddProduct();
        });

        mTableBody.on('blur', '[name=qty],[name=remarks]', null, function(e) {
            var pLink = $(e.target);
            mSelectedTr = pLink.closest('tr');
            if (mSelectedTr.data('data') == null)
                mSelectedTr.data('data', {});
            if (mOut.ItemRequisitionMode)
                Save();
        });

        mTableBody.on('click', '[name=vendor]', null, function(e) {
            e.preventDefault();
            var pLink = $(e.target);
            mSelectedTr = pLink.closest('tr');
            if (mSelectedTr.data('data') == null)
                mSelectedTr.data('data', {});
            AddVendor();
        });
        mTableBody.on('click', '[name=requestedBy]', null, function(e) {
            var pLink = $(e.target);
            mSelectedTr = pLink.closest('tr');
            if (mSelectedTr.data('data') == null)
                mSelectedTr.data('data', {});
            AddRequestedBy();
        });

        mTableBody.on('click', '[name=unit]', null, function(e) {
            var pLink = $(e.target);
            mSelectedTr = pLink.closest('tr');
            if (mSelectedTr.data('data') == null)
                mSelectedTr.data('data', {});
            AddUnitMeasure();
        });

        mTableBody.on('click', '[name=delete]', null, function(e) {
            var pLink = $(e.target);
            mSelectedTr = pLink.closest('tr');
            if (mSelectedTr.data('data') == null)
                mSelectedTr.data('data', {});
            
            var pID = mSelectedTr.data('data').ID;
            if (CStr2(pID) != '') {
                AjaxSend({
                    url: '/SalesMenu/DeactivateItemRequisitions',
                    data: {
                        ID: pID
                    }
                });
            }
            mSelectedTr.remove();
            AddRowIfNeeded();
        });

        mElement.on('change', '[name=selActive],[name=selPoFilter],[name=selPoBySkuFowstDateRequested],[name=selMaxRecord]', null, function(e) {
            LoadData();
        });

        function SysAdminDataTable(vData) {
            mTableBody.empty()
            
            if ($.fn.DataTable.isDataTable(mTable)) {
                //check if datatable is created 
                mTable.DataTable().clear().destroy();
            }

            mTable.DataTable({
                //retrieve: true,
                data: vData.data,
                "bdestroy": true,
                "pageLength": 50,
                "paging": false,
                //"order": [[1, "asc"]],
                "bProcessing": true,
                //"bServerSide": true,
                //"ajax": vData.ajax,
                "autoWidth": false,
                "columns": vData.columns,
                "aoColumnDefs": [{ "bSortable": true, "aTargets": vData.disablecols }],
                "responsive": true,
                "columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"top">rt<"bottom"ip><"clear">',
                rowCallback: function(row, data, dataIndex){
                    $(row).data('data', data)
                       .click(function(e) {
                            if (!$(e.target).parent().is(row))
                                return;
                            require(['promise!/Scripts/sources/ERP/PurchaseOrder/PoBySkuItem/controller.js?v=' + assetversion], function () {
                                ERP.PurchaseOrder.PoBySkuItem.controller.popup({
                                    ItemRequisitionID: data.ID,
                                    ProductID: data.ProductID,
                                    OnOk: LoadData
                                });
                            });
                       });
                },
                "initComplete": function () {
                    mTable.removeClass('dataTable');
                    mTable.find('thead tr').addClass('align-top text-center')
                }
                //"stateSave": true
            })
            if (mTr == null)
                mTr = mTableBody.find('tr').clone();

            AddRowIfNeeded();

        }

        function AddProduct(vParams) {
            vParams = $.extend({
                FilterSupplierID: null
            }, vParams);
            var pColumns = [
                { 
                    Column: 'ProductNo', 
                    Label: 'Product No.' 
                },
                { 
                    Column: 'ProductName', 
                    Label: 'Product Name' 
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
                    Label: 'Qty',
                    Render: function(v1, v2, vData) {
                        return '<div name="divQty" />';
                        //return '<input class="qty" style="width:70px" />';
                    }
                }
            ]);
            pColumns.push({
                Label: 'Packing Size (in smallest unit)',
                Render: function(v1, v2, vData) {
                    return CStr2(vData.UnitMeasure);
                }
            });
            pColumns.push({
                Label: 'Qty Available to Commit',
                Render: function(v1, v2, vData) {
                    var p = $.format.number(CDec2(vData.AvailableQtyToCommit), '#,###.##');
                    p = '<div style="text-align: center">' + p + '</div>';
                    return p;
                }
            });
            pColumns.push(
                {
                    Label: 'Action',
                    Render: function(v1, v2, vData) {
                        return '<div name="divAction"></div>';
                    }
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
                        AutoSearch: false,
                        ShowOkButton: true,
                        DisplayText: pColumns,
                        DontCloseWhenClick: true,
                        Click: function (e) {
                            if (!e.FromCacheProductSearch)
                                CacheProductSearch();
                            if ($.isArray(e)) {
                                PreSelect(e);
                            } else {
                                SelectProduct(e);
                                if (mOut.ItemRequisitionMode)
                                    Save();
                            }
                        },
                        AllowEmptySearch: false,
                        StyleFormat: 'table',
                        InitComplete: function(vTable) {
                            vTable.find('.qty').click(function(e) {
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
                                    mInventoryBalance.Show({
                                        ProductID: data.ID,
                                        ProductNo: data.ProductNo,
                                        ProductName: data.ProductName,
                                        OrderController: mOut,
                                        AfterSave: function() {
                                            LoadTrans({
                                                ID: mOut.Model.Data.ID,
                                                LoadProductsOnly: true
                                            });
                                        }
                                    });
                                    return false;
                                });
                            pDiv.append(pLink);
                            var pDiv = $(row).find('[name=divQty]');
                            pDiv.click(function(e) {
                                e.preventDefault();
                                return false;
                            });
                            var pDisabled = '';
                            // commented on 2021_09_02__09_42_32_5538
                            //if (CDec2(data.RetailPrice) == 0)
                            //    pDisabled = ' disabled ';
                            pDiv.append('<input class="qty form-control" ' + pDisabled + ' style="width:70px" />');
                        }
                    }
                ], pAdditionalData
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        } // END function AddProduct(vParams) {

        function PreSelect() {
            var pTxts = $('#tblBrowsePopup .qty');
            var pIndex;
            var pBelowGpmThreshold = false;
            var pTRs = $();

            for (var p = 0; p <= pTxts.length - 1; p++) {
                var pTxt = $(pTxts[p]);
                var pQty = CDec2(pTxt.val());
                if (pQty > 0) {
                    var pTR = pTxt.closest('tr');
                    var pDR = pTR.data('data');

                    mSelectedTr = mTableBody.find('tr:last');
                    pTRs = pTRs.add(mSelectedTr);
                    mSelectedTr.find('[name=qty]').val(pQty);
                    SelectProduct({
                        ID: pDR.ID,
                        ProductNo: pDR.ProductNo,
                        ProductName: pDR.ProductName,
                        Measurement: pDR.UnitMeasure,
                        Quantity: pDR.UnitQty
                    });
                } // todo: if qty = 0, delete at server
            }
            if (mOut.ItemRequisitionMode) {
                mSelectedTr = pTRs;
                Save();
            }
        }

        function AddVendor() {
            var pTarget;
            var pOptions;

            if ((mSelectedTr == null) 
                || (mSelectedTr.data('data').ProductID == null)) {

                NewMessageBox('Please select a product first.');
                return;
            }

            pTarget = 'Vendor';
            pOptions = { 
                ProductID: mSelectedTr.data('data').ProductID
            };

            BrowsePopup('LibraryVendorPrice', pTarget,
                [
                    {
                        // Search: mTxtCustomer.val(), // commented to prevent auto search upon popup
                        StyleFormat: 'table',
                        AutoSearch: true,
                        DisplayText: [
                            { 
                                Column: 'CompanyName',
                                Label: 'Vendor Name'
                            },
                            { 
                                Column: 'Price',
                                Label: 'Vendor Price'
                            },
                            { 
                                Column: 'PaymentTerm',
                                Label: 'Payment Term'
                            },
                            { 
                                Column: 'LastUpdated',
                                Label: 'Last Update Of Price'
                            }
                        ],
                        Click: function (e) {
                            SelectVendor(e);
                        }
                    }
                ], pOptions
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        } // END function SearchCustomer() {
        
        function AddRequestedBy() {
            var pTarget;
            var pOptions;

            if ((mSelectedTr == null) 
                || (mSelectedTr.data('data').ProductID == null)) {

                NewMessageBox('Please select a product first.');
                return;
            }

            pTarget = 'Vendor';
            pOptions = { 
                ProductID: mSelectedTr.data('data').ProductID
            };

            BrowsePopup('LibraryOrganizationEmployee', pTarget,
                [
                    {
                        // Search: mTxtCustomer.val(), // commented to prevent auto search upon popup
                        StyleFormat: 'table',
                        AutoSearch: true,
                        DisplayText: [
                            { Column: 'PeopleName' }
                        ],
                        Click: function (e) {
                            SelectRequester(e);
                        }
                    }
                ], pOptions
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        } // END function SearchCustomer() {

        function SelectProduct(vData) {
            var pData = mSelectedTr.data('data') || {};
            mSelectedTr.find('[name=productNumber]').text(vData.ProductNo);
            mSelectedTr.find('[name=product]').text(vData.ProductName);
            pData.ProductID = vData.ID;

            if (vData.Measurement != null) {
                mSelectedTr.find('[name=unit]').text(vData.Measurement);
                pData.UnitMeasure = vData.Measurement;
                pData.UnitQty = vData.Quantity;
            }

            mSelectedTr.data('data', pData)
            AddRowIfNeeded(pData);
        }

        function SelectVendor(vData) {
            var pData = mSelectedTr.data('data');
            mSelectedTr.find('[name=vendor]').text(vData.CompanyName);
            mSelectedTr.find('[name=price]').text(AccountingFormat(vData.Price * CDec2(pData.UnitQty)));
            mSelectedTr.find('[name=lastUpdated]').text(FormatDate(vData.LastUpdated, {Format: 'MMM d, yyyy h:mm a'}));
            mSelectedTr.find('[name=paymentTerm]').text(CStr2(vData.PaymentTerm));
            pData.VendorID = vData.VendorID;
            pData.Price = vData.Price;
            pData.RequestedByID = vData.RequestedByID;
            mSelectedTr.data('data', pData)
            AddRowIfNeeded(pData);
            if (mOut.ItemRequisitionMode)
                Save();
        }
        
        function SelectRequester(vData) {
            var pData = mSelectedTr.data('data');
            mSelectedTr.find('[name=requestedBy]').text(vData.PeopleName);
            pData.VendorID = vData.VendorID;
            pData.Price = vData.Price;
            pData.RequestedByID = vData.ID;
            mSelectedTr.data('data', pData)
            AddRowIfNeeded(pData);
            if (mOut.ItemRequisitionMode)
                Save();
        }

        function AddUnitMeasure() {
            var pOptions;

            if ((mSelectedTr == null) 
                || (mSelectedTr.data('data').ProductID == null)) {

                NewMessageBox('Please select a product first.');
                return;
            }

            pOptions = { 
                ProductID: mSelectedTr.data('data').ProductID
            };

            BrowsePopup('LibraryPIMProductMeasure', 'Unit Measure',
                [
                    {
                        // Search: mTxtCustomer.val(), // commented to prevent auto search upon popup
                        StyleFormat: 'table',
                        AutoSearch: true,
                        DisplayText: [
                            { 
                                Label: 'Unit Measurement',
                                Column: 'Measurement'
                            },
                            { 
                                Label: 'Unit Qty.',
                                Column: 'Quantity'
                            }
                        ],
                        Click: function (e) {
                            SelectUnitMeasure(e);
                        }
                    }
                ], pOptions
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        } // END function SearchCustomer() {

        function SelectUnitMeasure(vData) {
            var pData = mSelectedTr.data('data');
            mSelectedTr.find('[name=unit]').text(vData.Measurement);
            pData.UnitMeasure = vData.Measurement;
            pData.UnitQty = vData.Quantity;
            mSelectedTr.find('[name=price]').text(AccountingFormat(CDec2(vData.Price) * vData.Quantity));
            mSelectedTr.data('data', pData)
            AddRowIfNeeded(pData);
            if (mOut.ItemRequisitionMode)
                Save();
        }

        mElement.find('[name=butNew]').click(function() {
            NewMessageBox('Are you sure you want to clear the grid below?', 'New',
                [
                    {
                        Text: 'Yes',
                        Click: function () {
                            DisplayData();
                        }
                    }, { Text: 'No' }
                ]
            );
        });

        
        mElement.find('[name=butSaveAsDraft]').click(function() {
            Save();
        });

        function Save() {
            if (Verify() == false)
                return;
            var pMsg = '';
            if (mOut.ItemRequisitionMode) {
                // pMsg = 'Are you sure you want to save this item requisition?'; // commented on 2021_08_28__09_44_36_3204
                Proceed();
            } else {
                pMsg = 'Are you sure you want to create multiple purchase orders? Note: Rows with invalid quantity will not be included.';

                NewMessageBox(pMsg, 'Save',
                    [
                        {
                            Text: 'Yes',
                            Click: function () {
                                Proceed();
                            }
                        }, { Text: 'No' }
                    ]
                );
            }

            function Verify() {
                var pTrs = mTableBody.find('tr');
                
                var pCount = 0;
                for (var pTrIndex = 0; pTrIndex <= pTrs.length - 1; pTrIndex++) {
                    var pTR = $(pTrs[pTrIndex]);
                    var pData = pTR.data('data');
                    if (pData != null) {
                        pData.Qty = pTR.find('[name=qty]').val();
                        if (pData.ProductID == null) {
                            continue;
                            //NewMessageBox('Line item ' + (pTrIndex + 1) + ' has no product. Please input product', 'Invalid input');
                            //return false;
                        }

                        if (mOut.ItemRequisitionMode) {
                        } else {
                            if (pData.VendorID == null) {
                                continue;
                                //NewMessageBox('Line item ' + (pTrIndex + 1) + ' has no vendor. Please input vendor.', 'Invalid input');
                                //return false;
                            }
                        }
                        pCount++;
                    }
                }

                if (pCount == 0) {
                    NewMessageBox('Please input at least 1 line item.', 'No input data');
                    return false;
                }

                return true;
            }

            function Proceed() {
                var pTrs;
                if (mOut.ItemRequisitionMode) {
                    pTrs = mSelectedTr;
                } else
                    pTrs = mTableBody.find('tr');
                var pAll = [];
                for (var pTrIndex = 0; pTrIndex <= pTrs.length - 1; pTrIndex++) {
                    var pTR = $(pTrs[pTrIndex]);
                    var pData = pTR.data('data');
                    if (pData != null) {
                        if (CStr2(pData.CreatedPurchaseOrderID) != '')
                            continue;
                        if (mOut.ItemRequisitionMode) {
                            
                        } else {
                            if (CStr2(pData.VendorID) == '')
                                continue;
                        }
                        if (pData.ProductID == null) {
                            continue;
                        }
                        pData.Remarks = pTR.find('[name=remarks]').val();
                        pData.Qty = pTR.find('[name=qty]').val();
                        // if (CDec2(pData.Qty > 0)) // commented on 2021_08_28__09_44_36_3204
                        pAll.push(pData);
                    }
                }

                if (pAll.length == 0)
                    return;

                var pUrl;

                if (mOut.ItemRequisitionMode)
                    pUrl = '/SalesMenu/SaveItemRequisitions';
                else
                    pUrl = '/SalesMenu/CreateMultiplePurchaseOrders';

                var pFowstID = mFowstID;
                if (CStr2(pFowstID) == '')
                    pFowstID = sessionStorage.FowstID;

                AjaxSend({
                    url: pUrl,
                    data: {
                        FowstID: pFowstID,
                        Items: pAll,
                        Expanded: mExpanded
                    },
                    success: function(vData) {
                        if (vData.Error)
                            NewMessageBox(vData.Error);
                        else if (vData.Result == 'ok') {
                            if (!mOut.ItemRequisitionMode)
                                NewMessageBox('Your requested item(s) have been saved.<br /><br />' +
                                    'The purchasing office will be able to view your requests.  For urgent requests, please get in touch with purchasing to inform them of the urgency of your request(s).',
                                    'Successfully Saved');
                            LoadData();
                        }
                    }
                });
            }
        };

        function AddRowIfNeeded(vTrData) {

            if (vTrData == null) {
                // just do nothing here
            } else if ((vTrData.ProductID == null)
                && (vTrData.VendorID == null)) {

                return;
            }

            var pShouldAddRow = true;
            pShouldAddRow = true;
            var pTrs = mTableBody.find('tr');
            var pAll = [];
            for (var pTrIndex = 0; pTrIndex <= pTrs.length - 1; pTrIndex++) {
                var pTR = $(pTrs[pTrIndex]);
                if (pTR.children('td').hasClass('dataTables_empty')) {
                    pTR.remove();
                    break;
                }
                var pData = pTR.data('data');
                if (pData == null) {
                    return;
                }
                if ((pData.ProductID == null)
                    && (pData.VendorID == null)) {

                    return;
                }
            }

            if (pShouldAddRow) {
                mTableBody.append(mTr.clone());
            }
            return pShouldAddRow;
        }

        // mOut.Initialize();

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});