define([
    ], function () {

        var mDeferred = $.Deferred();
        var mHtml;

        //(function staticInit() {

        //    AjaxSend({
        //        ExactUrl: '/Scripts3/view/PriceChange.html',
        //        type: 'GET',
        //        dataType: 'text',
        //        success: function (vData) {
        //            mHtml = vData;
        //            mDeferred.resolve();
        //        }
        //    });

        //})();

        $(function () {

            CreateStringObject('App.controller.PIMPriceAdjustment', null, function () {

                var mOut = {};
                var mElement;
                var mData;
                var mComboInited = false;
                var mMethod;
                var mMethodText;
                var mProducts;
                var mPriceLevelToAdjustText;
                var mPriceLevelBasis;
                var mExcludeDateFrom, mExcludeDateTo;

                //mOut.model = new App.model.PriceChange();

                mElement = $('#pagePIMPriceAdjustment');
                mOut.element = mElement;
                // $('body').append(mElement).trigger('create');
                // $('[data-role=page]:first').append(mElement).trigger('create');

                //            $('[data-role=page]:first').append(mElement);
                //            $('[data-role=page]:first').trigger('create');

                //            $.mobile.activePage.append(mElement);
                //            $.mobile.activePage.trigger('create');

                var mPriceLevelToAdjustID;
                var mPriceLevelBasisID;
                
                var mCbPriceAdjustmentMethod = mElement.find('[name=cbPriceAdjustmentMethod]');
                mCbPriceAdjustmentMethod.change(function() {
                    Show1MethodDiv();
                });
                function HideDivs() {
                    mElement.find('[name=divMarkUpDown]').hide();
                    mElement.find('[name=divGrossMargin]').hide();
                    mElement.find('[name=divNominalPrice]').hide();
                }
                function Show1MethodDiv() {
                    HideDivs();
                    mMethod = mCbPriceAdjustmentMethod.val();
                    if ((mMethod == 'mark up') 
                        || (mMethod == 'mark down')) {

                        mElement.find('[name=divMarkUpDown]').show();
                        mElement.find('[name=spanMethod]').text(mElement.find('[name=cbPriceAdjustmentMethod] option:selected').text());
                    } else if (mMethod == 'gross margin') {
                        mElement.find('[name=divGrossMargin]').show();
                    } else if (mMethod == 'nominal price') 
                        mElement.find('[name=divNominalPrice]').show();
                }

                var mButPriceLevelToAdjust = mElement.find('[name=butPriceLevelToAdjust]');
                mButPriceLevelToAdjust.click(function(e) {
                    BrowsePopup('LibraryPIMPriceLevel', 'Price Level',
                        [
                            {
                                DisplayText: [{ Column: 'PriceLevel' }],
                                Click: function (e) {
                                    mPriceLevelToAdjustID = e.ID;
                                    mButPriceLevelToAdjust.text(e.PriceLevel);//.button();//.button('refresh');
                                    mElement.find('[name=spanNominalPriceLevel]').text(e.PriceLevel);
                                }
                            }
                        ]
                    );
                });

                var mButPriceLevelBasis = mElement.find('[name=butPriceLevelBasis]');
                mButPriceLevelBasis.click(function(e) {
                    BrowsePopup('LibraryPIMPriceLevel', 'Price Level',
                        [
                            {
                                DisplayText: [{ Column: 'PriceLevel' }],
                                Click: function (e) {
                                    mPriceLevelBasisID = e.ID;
                                    mButPriceLevelBasis.text(e.PriceLevel);//.button();//.button('refresh');
                                }
                            }
                        ]
                    );
                });

                mOut.popup = function () {

                    var pElement;

                    // mElement.appendTo($.mobile.activePage).trigger('create');
                    //                mElement.appendTo($.mobile.activePage);
                    //                mElement.trigger('create');
                    // mElement.appendTo($('[data-role=page]:first')).trigger('create');

                    if (mElement.parent().length == 0) {
                        $.mobile.activePage.append(mElement);
                        $.mobile.activePage.trigger('create');
                    }

                    //if (mOut.model.canTestPrice())
                    //    ChangeButtonText(mOut.butOk, 'Test price');

                    mElement.popup('open');
                    pElement = mElement.closest('.ui-popup-container');
                    if (pElement.length == 0)
                        console.trace('popup container for divRequestPrice not found');
                    else
                    // know issue: pop up dimension will revert to default after browser window resize
                        pElement.css({
                            left: 0,
                            width: '80%',
                            margin: 'auto 10%',
                            padding: 15
                        });
                };

                mOut.popupClose = function () {

                    mElement.popup('close');

                };
                
                mOut.validate = function () {

                    var pResult = mOut.model.validate();
                    if (pResult.error != undefined) {
                        if ($.isFunction(mOut.onInvalid))
                            mOut.onInvalid(pResult);
                    }

                    return pResult;

                };

                mOut.onSubmit = function () { };
                mElement.find('[name=formRequestedPrice]').submit(function (e) {
                    e.preventDefault();
                    var pResult = mOut.validate();
                    if (pResult.error != undefined) {
                        return false;
                    }
                    return mOut.onSubmit(arguments);
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
                                .attr('id', 'chk' + p)
                                .prop('checked', CBool(pDR.Included))
                                .data(pDR)
                            )
                            .append(
                                $('<label for="form-check-label">')
                                    .attr('for', 'chk' + p)
                                    .text(pDR.Brand)
                            )
                        mDivProductBrands.append(pCheckBox);
                    }
                    $('input:checkbox');//.checkboxradio();
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
                                .attr('id', 'chk' + p)
                                .prop('checked', CBool(pDR.Included))
                                .data(pDR)
                            )
                            .append(
                                $('<label class="form-check-label">')
                                    .attr('for', 'chk' + p)
                                    .text(pDR.Category)
                            )

                        mDivProductCategories.append(pCheckBox);
                    }
                    $('input:checkbox');//.checkboxradio();
                }

                var mTxtMarkUpDownRate = mElement.find('[name=txtMarkUpDownRate]');
                var mTxtGrossMarginRate = mElement.find('[name=txtGrossMarginRate]');
                var mTxtNominalPrice = mElement.find('[name=txtNominalPrice]');
                var mButNext = mElement.find('[name=butNext]');
                mButNext.click(function() {
                    var pRate;
                    var pCheckedBrands = mDivProductBrands.find(':checked');
                    var pCheckeds = mDivProductCategories.find(':checked');
                    if ((pCheckeds.length == 0)
                        && (pCheckedBrands.length == 0)) {
                        NewMessageBox('Please select at least 1 brand or category');
                        return;
                    }
                    if (mPriceLevelToAdjustID == null) {
                        NewMessageBox('Please select the price level to adjust.');
                        return;
                    }

                    if ((mMethod == 'mark up') 
                        || (mMethod == 'mark down')) {

                        if (mPriceLevelBasisID == null) {
                            NewMessageBox('Please select the basis price level.');
                            return;
                        }

                        pRate = CDec2(mTxtMarkUpDownRate.val());
                        if (pRate == 0) {
                            NewMessageBox('Rate must not be zero.');
                            return;
                        }
                    } else if (mMethod == 'gross margin') {
                        pRate = CDec2(mTxtGrossMarginRate.val());
                        if (pRate == 0) {
                            NewMessageBox('Gross margin rate must not be zero.');
                            return;
                        }
                    } else if (mMethod == 'nominal price') {
                        pRate = CDec2(mTxtNominalPrice.val());
                        if (pRate == 0) {
                            NewMessageBox('Nominal price must not be zero.');
                            return;
                        }
                    }

                    var mRoundMethod = $('[name=cbPriceAdjustmentRoundMethod]').val();
                    var mRoundIncrement = $('[name=cbPriceAdjustmentRoundIncrement]').val();

                    var pBrands = [];
                    for (var p = 0; p <= pCheckedBrands.length - 1; p++) {
                        var pChecked = $(pCheckedBrands[p]);
                        var pDR = pChecked.data();
                        pBrands.push(pDR.Brand);
                    }

                    var pCategories = [];
                    for (var p = 0; p <= pCheckeds.length - 1; p++) {
                        var pChecked = $(pCheckeds[p]);
                        var pDR = pChecked.data();
                        pCategories.push(pDR.Category);
                    }
                    GetProducts({
                        Brands: pBrands,
                        Categories: pCategories,
                        PriceLevelToAdjustID: mPriceLevelToAdjustID,
                        PriceLevelBasisID: mPriceLevelBasisID,
                        Method: mMethod,
                        Rate: pRate,
                        RoundMethod: mRoundMethod,
                        RoundIncrement: mRoundIncrement
                    });
                });

                function GetProducts(vParams) {
                    var pData = {
                        Brands: vParams.Brands,
                        Categories: vParams.Categories,
                        PriceLevelToAdjustID: vParams.PriceLevelToAdjustID,
                        PriceLevelBasisID: vParams.PriceLevelBasisID,
                        Method: vParams.Method,
                        Rate: vParams.Rate,
                        RoundMethod: vParams.RoundMethod,
                        RoundIncrement: vParams.RoundIncrement,
                        ExcludeDateFrom: mExcludeDateFrom,
                        ExcludeDateTo: mExcludeDateTo
                    };

                    mPriceLevelToAdjustText = mButPriceLevelToAdjust.text();
                    mMethodText = $('[name=cbPriceAdjustmentMethod] option:selected').text();
                    mPriceLevelBasis = mButPriceLevelBasis.text();

                    AjaxSend({
                        url: '/Browser/ProductsByBrandsOrCategories',
                        data: pData,
                        success: function(vData) {
                            if (vData.Error)
                                NewMessageBox(vData.Error);
                            else
                                DisplayProducts(vData.Results.Rows);
                        }
                    });
                }

                var mTable = mElement.find('[name=tableProducts]');
                var mDataTable;

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
                        data: vRows,
                        columns: [
                            {
                                data: function(vData) {
                                    return vData.ProductNo || '';
                                }
                            },
                            {
                                data: function(vData) {
                                    return vData.ProductName || '';
                                }
                            },
                            {
                                data: function(vData) {
                                    return vData.Category || '';
                                }
                            },
                            {
                                data: function(vData) {
                                    return mPriceLevelToAdjustText;
                                }
                            },
                            {
                                data: function(vData) {
                                    return AccountingFormat(vData.PriceToAdjust);
                                }
                            },
                            {
                                data: function(vData) {
                                    return mMethodText;
                                }
                            },
                            {
                                data: function(vData) {
                                    return mPriceLevelBasis;
                                }
                            },
                            {
                                data: function(vData) {
                                    return AccountingFormat(vData.BasisPrice);
                                }
                            },
                            {
                                data: function(vData) {
                                    return AccountingFormat(vData.ResultingPrice);
                                }
                            }
                        ],
                        'rowCallback': function(row, data, dataIndex) {
                            $(row).data('data', data);
                        }
                    });
                }

                mElement.find('[name=butSave]').click(function() {
                    NewMessageBox('Are you sure you want to update the prices?', 'Update prices?', [
                        {
                            Text: 'Yes', Click: function () {
                                Save();
                            }
                        }, { Text: 'No' }
                    ]);
                });

                function Save() {
                    // get rows to save
                    var pValidRows = [];
                    
                    for (var p = 0; p <= mProducts.length - 1; p++) {
                        var pDR = mProducts[p];
                        if (pDR == null)
                            continue;
                        if (!IsNumeric(pDR.ResultingPrice))
                            continue;
                        var pValidRow = {
                            ProductID: pDR.ID,
                            ResultingPrice: pDR.ResultingPrice
                        };
                        pValidRows.push(pValidRow);
                    }

                    var pData = {
                        PriceLevelToAdjustID: mPriceLevelToAdjustID,
                        Products: pValidRows
                    };
                    AjaxSend({
                        url: '/PIMProductPrice/PimPriceAdjust',
                        data: pData,
                        success: function(vData) {
                            if (vData.Result == 'ok') 
                                NewMessageBox('Prices were successfully updated.');
                            else if (vData.Error)
                                NewMessageBox(vData.Error);
                        }
                    });
                }

                mElement.find('[name=butPriceAdjustmentExcludeDateFrom]').click(function() {
                    DualCalendar({
                        Click: function(e) {
                            mExcludeDateFrom = e.From;
                            mExcludeDateTo = e.To;
                            mElement.find('[name=butPriceAdjustmentExcludeDateFrom]')
                                .text(FormatDate(e.From) + ' to ' + FormatDate(e.To));
                        }
                    });
                });

                mOut.Init = function() {
                    Show1MethodDiv();
                    GetProductCategories();
                    GetProductBrands();
                };
                
                //mOut.Init();

                return mOut;

            }); // END CreateStringObject('App.controller.PriceChange', null, function() {

        }); // END $(function () {

        return mDeferred.promise();

    });
