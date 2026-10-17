define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ChangeProductArea/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.ChangeProductArea = ERP.ChangeProductArea.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.ChangeProductArea.controller', null, function () {

        var mOut = {};
        var mElement;
        var mNewArea;

        mElement = $(mHtml);
        mOut.element = mElement;
        var mTable = mElement.find('[name=tableProducts]');
        var mDataTable;

        mOut.Init = function() {
        };

        var mButNewArea = mElement.find('[name=butNewArea]');
        mButNewArea.click(function() {

            var pColumns = [
                { 
                    Column: 'Area', 
                    Label: 'Area Or Section' 
                }
            ];

            BrowsePopup('Areas', 'Area Or Section',
                [
                    {
                        AutoSearch: true,
                        ShowOkButton: true,
                        DisplayText: pColumns,
                        Click: function (e) {
                            mNewArea = e.Area;
                            mButNewArea.text(e.Area);//.button();//.button('refresh');
                        },
                        AllowEmptySearch: true,
                        StyleFormat: 'table',
                        InitComplete: function(vTable) {
                            
                        }
                    }
                ]
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        });

        function GetProductBrands() {

            AjaxSend({
                url: '/Stock/ProductBrands',
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else
                        DisplayProductBrands(vData);
                },
                error: function() {
                }
            });
        }

        var mDivProductBrands = mElement.find('[name=divProductBrands]');
        function DisplayProductBrands(vData) {
            var pCheckBox;
            var pDR;

            mDivProductBrands.empty();
            for (var p = 0; p <= vData.Rows.length - 1; p++) {
                pDR = vData.Rows[p];
                if (CStr2(pDR.Brand).trim() == '')
                    continue;
                pCheckBox = $('<div class="form-check mb-3" style="font-size:20px;">')
                            .append($('<input type="checkbox" class="form-check-input">')
                                .attr('id', 'chkBrand' + p)
                                .prop('checked', CBool(pDR.Included))
                                .data(pDR)
                            )
                            .append(
                                $('<label class="form-check-label">')
                                    .attr('for', 'chkBrand' + p)
                                    .text(pDR.Brand)
                            )

                mDivProductBrands.append(pCheckBox);
            }
            mDivProductBrands.find('input:checkbox');//.checkboxradio();
        }

        function GetProductCategories() {

            AjaxSend({
                url: '/Stock/ProductCategories',
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else
                        DisplayProductCategogies(vData);
                },
                error: function() {
                }
            });
        }

        var mDivProductCategories = mElement.find('[name=divProductCategories]');
        function DisplayProductCategogies(vData) {
            var pCheckBox;
            var pDR;

            mDivProductCategories.empty();
            for (var p = 0; p <= vData.Rows.length - 1; p++) {
                pDR = vData.Rows[p];
                if (CStr2(pDR.Category).trim() == '')
                    continue;
                pCheckBox = $('<div class="form-check mb-3" style="font-size:20px;">')
                            .append($('<input type="checkbox" class="form-check-input">')
                                .attr('id', 'chkCategory' + p)
                                .prop('checked', CBool(pDR.Included))
                                .data(pDR)
                            )
                            .append(
                                $('<label class="form-check-label">')
                                    .attr('for', 'chkCategory' + p)
                                    .text(pDR.Category)
                            )

                mDivProductCategories.append(pCheckBox);
            }
            mDivProductCategories.find('input:checkbox');//.checkboxradio();
        }

        mElement.find('[name=butNext]').click(function() {
            $('[custom-tab-content="#tabChangeProductAreaBrands"]').click();
        });

        mElement.find('[name=butBrandNext]').click(function() {
            $('[custom-tab-content="#tabChangeProductAreaCategories"]').click();
        });

        mElement.find('[name=butCategoryNext]').click(function() {
            $('[custom-tab-content="#tabChangeProductAreaProducts"]').click();
            var pData = {};
            pData.IsAll = 1;
            pData.ShowArea = 1;
            if (mElement.find('[name=cbPriceAdjustmentMethod]').val() == 'and')
                pData.FilterByBrandAndCategory = true;
            else
                pData.FilterByBrandOrCategory = true;
                 
            var pCheckeds = mDivProductBrands.find(':checked');
            var pBrands = [];
            for (var p = 0; p <= pCheckeds.length - 1; p++) {
                var pChecked = $(pCheckeds[p]);
                var pDR = pChecked.data();
                pBrands.push(pDR.Brand);
            }
            pData.Brands = pBrands;

            var pCheckeds = mDivProductCategories.find(':checked');
            var pCategories = [];
            for (var p = 0; p <= pCheckeds.length - 1; p++) {
                var pChecked = $(pCheckeds[p]);
                var pDR = pChecked.data();
                pCategories.push(pDR.Category);
            }
            pData.Categories = pCategories;

            AjaxSend({
                url: 'Browser/LibraryPIMProduct',
                data: pData,
                success: function(vData) {
                    DisplayProducts(vData.Results.Rows);
                }
            });
        });

        function ClearTable() {
            if ($.fn.DataTable.isDataTable(mTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTable.empty();
                mDataTable.destroy();
            }
        }

        function DisplayProducts(vRows) {
            mProducts = vRows;
            ClearTable();
            mElement.find('[custom-tab-content="#tabPIMPriceAdjustmentProducts"]').click();
            mDataTable = mTable.DataTable({
                paging: false,
                data: vRows,
                columns: [
                    {
                        title: 'Include',
                        render: function(v1, v2, vData) {
                            return '<div name=divInclude></div>';
                        }
                    },
                    {
                        title: 'Product Number',
                        data: function(vData) {
                            return vData.ProductNo || '';
                        }
                    },
                    {
                        title: 'Product Name',
                        data: function(vData) {
                            return vData.ProductName || '';
                        }
                    },
                    {
                        title: 'Brand',
                        data: function(vData) {
                            return vData.Brand || '';
                        }
                    },
                    {
                        title: 'Category',
                        data: function(vData) {
                            return vData.Category || '';
                        }
                    },
                    {
                        title: 'Old Area Or Section',
                        data: function(vData) {
                            return vData.Area || '';
                        }
                    }
                ],
                'rowCallback': function(row, data, dataIndex) {
                    $(row)
                        .data('data', data)
                        .find('[name=divInclude]').append('<input type=checkbox>');
                }
            });
            mTable.addClass('table-stripe').css('width', '100%')
            mTable.find('thead tr').addClass('custom-theme-f');
        }

        mElement.find('[name=butCheckAll]').click(function() {
            mTable.find('[type=checkbox]').prop('checked', true); 
        });

        mElement.find('[name=butCheckNone]').click(function() {
            mTable.find('[type=checkbox]').prop('checked', false); 
        });

        mElement.find('[name=butSave]').click(function() {
            var pCheckeds = mTable.find(':checked');
            if (pCheckeds.length == 0) {

                NewMessageBox('Please check at least 1 product.', 'No checked product');
                return;
            }

            if (CStr2(mNewArea) == '') {
                $('[custom-tab-content="#tabChangeProductAreaParameters"]').click();
                var pMsg = 'Please select a new area or section.';
                console.log(pMsg);
                NewMessageBox(pMsg, 'No new area or section');
                return;
            }

            var pProducts = [];
            for (var p = 0; p <= pCheckeds.length - 1; p++) {
                var pChecked = $(pCheckeds[p]);
                var pDR = pChecked.closest('tr').data();
                pProducts.push(pDR.data.ID);
            }

            AjaxSend({
                url: 'Stock/ChangeProductsArea',
                data: {
                    ProductIDs: pProducts,
                    Area: mNewArea
                },
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error, 'Error');
                    else if (vData.Count != null)
                        NewMessageBox(vData.Count + ' products were successfully updated', 'Success');
                    else
                        NewMessageBox(vData, 'Unknown response');
                }
            });
        });

        GetProductBrands();
        GetProductCategories();

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});