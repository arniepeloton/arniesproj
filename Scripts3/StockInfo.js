require(['common',
    GetPathFormat('/Scripts3/Browser/LibraryOrganizationEmployeeFOWST.js?crc='),
    'promise!/Scripts3/controller/PriceChange.js?crc='
    ], function () {

    require([(OfflineMode ? '/Scripts3/Stock_offline.js?crc=' : '')], function () {
        $(function () {

            var IsNewSalesRep = true;
            var arSalesRep = new Array();
            var intSalesRepCntr = 0;
            var intSalesRepIndex = 0;

            var intSalesRepRecID = 0;
            var intSalesRepID = 1;
            var intSalesRepName = 2;
            var intSalesRepState = 3;

            var IsNewProductSerial = true;
            var arProductSerial = new Array();
            var intProductSerialCntr = 0;
            var intProductSerialIndex = 0;

            var intProductSerialRecID = 0;
            var intProductSerial = 1;
            var intProductSerialState = 2;

            var mData; // from 'Stock/GetInfoPos' or 'Stock/GetInfo'
            var mTxtSalesRep; // the textbox opening the people browser
            var mHasError = 1;
            var mDivRequestPrice = $('#divRequestPrice');
            var mTxtRequestedPrice = $('#txtRequestedPrice');
            var mFormRequestedPrice = $('#formRequestedPrice');
            var mDivRequestedUnitPrice; // set during "Request Special Price" click
            var mPriceChange = new App.controller.PriceChange();
            var mCurrentyTxtQty;
            var mAllowFreeTypeAPrice = false;

            var isPriceLevelChanged = false; //additional by janine
            $("[data-role=header]").fixedtoolbar({ tapToggle: false });
            $('#divStockMinimumCapacity').hide();
            $('#divStockMaximumCapacity').hide();

            setTimeout(function () {
                //            $.mobile.activePage.bind('pageshow', PageShow);
            }, 1000);
            $.mobile.activePage.bind('pageshow', PageShow);
            mPriceChange.onInvalid = mPriceChange_onInvalid;
            mPriceChange.onSubmit = mPriceChange_onSubmit;

            if (OfflineMode)
                mAllowFreeTypeAPrice = Stock_offline().GetAllowFreeTypeAPrice();

            function PageShow() {
                if (ValidPrevPage(arguments) || window.NewStockBrowser) {
                    $('#divUnit').hide();
                    ////////////////////                                                if (proj.PosMode)
                    $('#divUnit').find('[data-role=collapsible_clone]').remove();
                    ////////////////////                                                else
                    ////////////////////                                                    $('#divUnit').find('[data-role=collapsible_clone_pos]').remove();

                    window.NewStockBrowser = false;

                    ShowStockInfo(true);
                    ShowSalesRepresentativeList(false);
                    ShowProductSerialList(false);
                    ShowInventoryBalances(false);

                    ClearPOSSalesRepSession();
                    LoadPOSSalesRepList();

                    ClearPOSProductSerialSession();
                    LoadPOSProductSerialList();

                    GetInfo();

                    // see /Scripts3/common/common.js
                    startclock('#clockStock');

                };

                function ValidPrevPage(args) {

                    try {
                        //return args[1].prevPage.attr('data-role') != 'dialog';
                        return args[1].prevPage.data('CanRefreshStockPage');
                    } catch (e) {
                        return true;
                    };

                };
            };

            PageShow();

            function collapsibleStatic() {
                // todo: this should be in dom.js  

                $('[data-role=collapsibleStatic]').collapsible({
                    collapsed: false,
                    iconpos: 'right',
                    contentTheme: $.mobile.page.prototype.options.contentTheme

                }).children('h3').children('a').click(function (e) {

                    e.preventDefault();
                    return false;

                }).on("tap", function (e, ui) {

                    e.preventDefault();
                    return false;

                });

            }; // collapsibleStatic

            function GetInfo() {

                var pParams;
                var pStockID;
                var pUrl;
                var pData;

                $('[name=divUnit]').remove();
                if (proj.ProductID != undefined)
                    pStockID = proj.ProductID;
                else {
                    pParams = GetUrlParameters();
                    pStockID = pParams.id;
                };

                ////////////////////                                    if (proj.PosMode) {
                pUrl = 'Stock/GetInfoPos';
                ////////////////////                                    } else {
                ////////////////////                                        pUrl = 'Stock/GetInfo';
                ////////////////////                                    };
                mHasError = 1; // assume that there's an error

                pData = {
                    ProductID: pStockID,
                    CustomerID: sessionStorage.CustomerOrganizationID,
                    OrderID: sessionStorage.CartOrderID,
                    NewCartMode: localStorage.NewCartMode
                };

                if (OfflineMode) {
                    pData = Stock_offline().GetInfoPos(pData);
                    LoadData(pData);
                } else { // if (OfflineMode) {
                    AjaxSend({
                        url: pUrl,
                        data: pData,
                        success: function (vData) {
                            LoadData(vData);
                        } // success
                    }); // AjaxSend
                }; // if (OfflineMode) {

                function LoadData(vData) {

                    var pStr;

                    if (vData.Error != undefined) {
                        MessageBox(vData.Error, 'Error');
                        return;
                    } else {
                        mHasError = 0;
                    };

                    ClearPOSSalesRepSession();
                    LoadPOSSalesRepList();

                    ClearPOSProductSerialSession();
                    LoadPOSProductSerialList();

                    mData = vData;
                    $('#spanStockNumber').text(vData.ProductNo);
                    $('#spanStockDescription').text(vData.ProductName);
                    sessionStorage.ProductName = vData.ProductName; //additional by janine apr 11

                    if ((vData.Warning)
                        || (
                            (vData.HasLandedCost != undefined)
                            && (vData.HasLandedCost == false)
                            )
                        || (
                            (vData.HasRetailPrice != undefined)
                            && (vData.HasRetailPrice == false)
                            )
                        ) {

                        pStr = null;
                        if ((vData.HasLandedCost != undefined)
                            && (vData.HasLandedCost == false)
                            && (vData.HasRetailPrice != undefined)
                            && (vData.HasRetailPrice == false))

                            pStr = 'Report No Landed Cost and Selling Price';
                        else if ((vData.HasLandedCost != undefined)
                            && (vData.HasLandedCost == false))

                            pStr = 'Report No Landed Cost';
                        else if ((vData.HasRetailPrice != undefined)
                            && (vData.HasRetailPrice == false))
                            pStr = 'Report No Selling Price';

                        if (pStr != undefined)
                            $('#butReportNoPrice').find('h3').text(pStr);

                        $('#divReportNoPrice').show();
                        $('#divSubTotal').hide();
                        // MessageBox(vData.Warning); // no message as of June 29
                        return;
                    };
                    $('#divSubTotal').show();
                    $('#divReportNoPrice').hide();

                    (function LoadUnitMeasures(vRows) {

                        var pTemplate = $('#divUnit');
                        var pDR;
                        var pItem;
                        var pLastItem;
                        var pOrderedQty;
                        var pSavingsPerBase;
                        var pTxtQty;
                        var pTxtAdjustPrice;
                        var pDivPriceLevel;
                        //                            var pDivSalesRep;
                        var pRequestedPriceData;

                        pLastItem = pTemplate;
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            pDR = vRows[p];
                            pItem = pTemplate.clone(true)
                                .attr('id', '')
                                .attr('name', 'divUnit');
                            ////////////////////                                                                if (proj.PosMode)
                            pDivPriceLevel = pItem.find('[name=divPriceLevel]');
                            ////////////////////                                                                else
                            ////////////////////                                                                    pDivMore = pItem.find('[data-role=collapsible_clone]').collapsible();

                            ////////////////////                                                                pDivSalesRep.hide();

                            pOrderedQty = '';
                            if (IsNumeric(pDR.OrderedQty))
                                pOrderedQty = $.format.number(CDec2(pDR.OrderedQty), proj.QuantityFormat);

                            pTxtQty = pItem.find('[name=txtQtyTemplate]')
                                .attr('name', 'txtQty')
                                .data('UnitQty', pDR.Quantity)
                                .data('Price', pDR.DiscountedPrice)
                                .data('SpanRequestedPrice', pItem.find('[name=spanRequestedUnitPrice]'))
                                .data('spanPrice', pItem.find('[name=spanPrice]'))
                                .val(pOrderedQty)
                                .bind('textchange', TxtQtyChanged);
                            if (mData.AllowDecimal) {
                                pTxtQty.attr('step', proj.QuantityStep);
                                mTxtRequestedPrice.attr('step', proj.QuantityStep);
                            } else
                                mTxtRequestedPrice.attr('step', 1);

                            if ((p == 0) && (!IsNumeric(pItem.find('[name=txtQty]').text())))
                                pItem.find('[name=txtQty]').text('1');

                            pItem.find('[name=spanPriceLevel]').text('Price Level');
                            if (CDec2(pDR.RequestedUnitPrice) != 0) {
                                pItem.find('[name=spanRequestedUnitPrice]').text($.format.number(CDec2(pDR.RequestedUnitPrice), '#,##0.00'));
                                pItem.find('[name=divRequestedUnitPrice]').show();
                                pRequestedPriceData = {
                                    priceLevelID: pDR.PriceLevelID,
                                    netAmount: CDec2(pDR.RequestedUnitPrice)
                                };
                                pTxtQty.data('requestedPriceData', pRequestedPriceData);
                            }
                            pItem.find('[name=spanUnit]').text(pDR.UnitMeasure);
                            pItem.find('[name=spanPrice]').text($.format.number(CDec2(pDR.DiscountedPrice), '#,##0.00'));
                            pItem.find('[name=spanUnitQtyLabel]').text(pDR.BaseUnit + ' per ' + pDR.UnitMeasure);
                            pItem.find('[name=spanBaseUnit]').text(pDR.BaseUnit);
                            pItem.find('[name=spanUnitQty]').text(pDR.Quantity);
                            pItem.find('[name=spanPricePerBase]').text($.format.number(CDec2(pDR.DiscountedPricePerUnit), '#,##0.00'));
                            pSavingsPerBase = CDec2(pDR.OriginalPrice) / CDec2(pDR.Quantity);
                            pSavingsPerBase -= CDec2(pDR.DiscountedPricePerUnit);
                            pItem.find('[name=spanSavingsPerBase]').text($.format.number(pSavingsPerBase, '#,##0.00'));
                            pItem.find('[name=spanTotalSavings]').text($.format.number(CDec2(pDR.OriginalPrice) - CDec2(pDR.DiscountedPrice), '#,##0.00'));
                            if (p == 0) {
                                pItem.find('[name=divTotalSavings]').hide();
                            }
                            else {
                                pItem.find('[name=divTotalSavings]').show();
                            }
                            if (CDec2(pDR.Srp) > 0) {
                                pItem.find('[name=divSrp]').show();
                                pItem.find('[name=spanSrp]').text(AccountingFormat(pDR.Srp));
                            }

                            pLastItem.after(pItem);
                            pItem.show();
                            pLastItem = pItem;

                            //                                prodUnitMeasure = pDR.UnitMeasure;

                            ////////////////                                if (proj.PosMode) {
                            // Price Level drop down
                            var pSelect = $('<select>');
                            pDivPriceLevel.append(pSelect);
                            var pCbItem;
                            var pRows = vData.data.Rows;

                            pSelect.html('');
                            for (var p1 = 0; p1 <= pRows.length - 1; p1++) {
                                if (pRows[p1].Quantity == pDR.Quantity) {
                                    //                                            pCbItem = new Option(pRows[p1].PriceLevel + ' ' + $.format.number(CDec2(pRows[p1].DiscountedPrice), '#,##0.00'), pRows[p1].PriceLevelID);
                                    pCbItem = new Option(pRows[p1].PriceLevel, pRows[p1].PriceLevelID);
                                    pSelect.append(pCbItem);
                                };
                            };

                            if (CBool(mAllowFreeTypeAPrice)) {
                                pCbItem = new Option('Free Type a Price', 'FreeTypeAPrice');
                                pSelect.append(pCbItem);
                                $(pCbItem).data('div', pItem.find('[name=divRequestedUnitPrice]'));
                            }

                            pSelect.val(pDR.PriceLevelID).attr('selected');
                            pSelect.attr('name', 'cbPrice');
                            pSelect.selectmenu();
                            pSelect.selectmenu('refresh', true);
                            pSelect.change(function () {
                                var pThis = $(this);
                                var pItem = pThis.closest('[name=divUnit]');
                                var pDR;
                                var pTxtQty;

                                pTxtQty = pItem.find('[name=txtQty]');
                                mCurrentyTxtQty = pTxtQty;
                                if (CBool(mAllowFreeTypeAPrice)
                                    && (pThis.val() == 'FreeTypeAPrice')) {
                                    // Free Type a Price
                                    mDivRequestedUnitPrice = $(pThis.find(':selected')).data('div');
                                    ShowFreeTypePriceDialog();
                                    return;
                                }

                                for (var p = 0; p <= mData.data.Rows.length - 1; p++) {
                                    pDR = mData.data.Rows[p];
                                    if ((pDR.Quantity == pTxtQty.data('UnitQty'))
                                            && (pDR.PriceLevelID == pThis.val())) {

                                        pTxtQty.data('UnitQty', pDR.Quantity)
                                            .data('Price', pDR.DiscountedPrice)
                                            .data('UnitMeasure', pDR.UnitMeasure);

                                        pItem.find('[name=spanUnit]').text(pDR.UnitMeasure);
                                        pItem.find('[name=spanPrice]').text($.format.number(CDec2(pDR.DiscountedPrice), '#,##0.00'));
                                        pItem.find('[name=spanUnitQtyLabel]').text(pDR.BaseUnit + ' per ' + pDR.UnitMeasure);
                                        pItem.find('[name=spanBaseUnit]').text(pDR.BaseUnit);
                                        pItem.find('[name=spanUnitQty]').text(pDR.Quantity);
                                        pItem.find('[name=spanPricePerBase]').text($.format.number(CDec2(pDR.DiscountedPricePerUnit), '#,##0.00'));
                                        pSavingsPerBase = CDec2(pDR.OriginalPrice) / CDec2(pDR.Quantity);
                                        pSavingsPerBase -= CDec2(pDR.DiscountedPricePerUnit);
                                        pItem.find('[name=spanSavingsPerBase]').text($.format.number(pSavingsPerBase, '#,##0.00'));
                                        pItem.find('[name=spanTotalSavings]').text($.format.number(CDec2(pDR.OriginalPrice) - CDec2(pDR.DiscountedPrice), '#,##0.00'));
                                    };

                                }; // for (var p = 0; p<= mData.data.Rows.length - 1; p++) {

                                DisplaySubtotal();

                                //                                    prodUnitMeasure = pDR.UnitMeasure;
                            });
                            pTxtQty.data('cbPrice', pSelect);
                            pDivPriceLevel.trigger('create');

                            // Sales Reps
                            if (mData.SalesReps != undefined) {
                                LoadPOSSalesRep(mData.SalesReps.Rows);
                            }

                            // Product Serials
                            if (mData.ProductSerial != undefined) {
                                LoadPOSProductSerial(mData.ProductSerial.Rows);
                            }

                            ////////                                    var pTxtSalesRep = pDivSalesRep.find('[name=txtSalesRep]');
                            ////////                                    pTxtQty.data('txtSalesRep', pTxtSalesRep);
                            ////////                                    pTxtSalesRep.data('txtQty', pTxtQty);
                            ////////                                    pTxtSalesRep.keydown(function (e) {
                            ////////                                        if (e.which == 13) {
                            ////////                                            mTxtSalesRep = $(this);
                            ////////                                            var pTxtQty = mTxtSalesRep.data('txtQty');
                            ////////                                            //Browser.LibraryOrganizationEmployeeFOWST.InputData.EmployeesOnly = 1;
                            ////////                                            Browser.LibraryOrganizationEmployeeFOWST.Click = function (e) {
                            ////////                                                var pSalesRep = pTxtQty.data('SalesRep') || [];

                            ////////                                                // delete duplicate
                            ////////                                                for (var pIndex = 0; pIndex <= pSalesRep.length - 1; pIndex++)
                            ////////                                                    if (pSalesRep[pIndex].PeopleID == e.ID) {
                            ////////                                                        MessageBox(e.PeopleName + ' is already on the list', 'Duplicate');
                            ////////                                                        return;
                            ////////                                                    };

                            ////////                                                pSalesRep.push({
                            ////////                                                    PeopleID: e.ID,
                            ////////                                                    PeopleName: e.PeopleName
                            ////////                                                });
                            ////////                                                pTxtQty.data('SalesRep', pSalesRep);
                            ////////                                                mTxtSalesRep.val('');
                            ////////                                                pTxtSalesRep.data('lv').SetData(pSalesRep);
                            ////////                                            };
                            ////////                                            Browser.LibraryOrganizationEmployeeFOWST.Show(mTxtSalesRep.val());
                            ////////                                        };
                            ////////                                    }); // pTxtSalesRep.keydown(function(e) {

                            ////////                                    var pLV = new CreateListView({
                            ////////                                        DataFilter: false
                            ////////                                    }); // the "new" keyword is important to prevent conflict with other instances of CreateListView

                            ////////                                    pLV.Element.data('txtSalesRep', pTxtSalesRep);
                            ////////                                    pLV.Element.find('ul')
                            ////////                                        .attr('data-split-icon', 'minus');
                            ////////                                    pLV.Element.insertAfter(pDivSalesRep.find('[name=divSalesRep]'));
                            ////////                                    pLV.OnItem = function (vDataRow, vItem, e) {
                            ////////                                        vItem.append($('<a>')
                            ////////                                            .text(vDataRow.PeopleName)
                            ////////                                            .attr('href', '#')
                            ////////                                            .click(function (e) {
                            ////////                                            })
                            ////////                                    );

                            ////////                                    vItem.append($('<a>')
                            ////////                                        .text('remove')
                            ////////                                        .data('data', vDataRow)
                            ////////                                        .data('li', vItem)
                            ////////                                        .data('lv', pLV)
                            ////////                                        .attr('href', '#')
                            ////////                                        .click(function (e) {
                            ////////                                            var pLV = $(this).data('lv');
                            ////////                                            var pData = pLV.GetData();
                            ////////                                            var pPeopleID = $(this).data('data').PeopleID;

                            ////////                                            for (var pIndex = 0; pIndex <= pData.length - 1; pIndex++) {
                            ////////                                                if (pData[pIndex].PeopleID == pPeopleID) {
                            ////////                                                    pData.splice(pIndex, 1);
                            ////////                                                    pIndex--;
                            ////////                                                };
                            ////////                                            };
                            ////////                                            $(this).data('lv').Element.data('txtSalesRep').data('txtQty').data('SalesRep', pData);
                            ////////                                            pLV.SetData(pData);
                            ////////                                        })
                            ////////                                    );
                            ////////                                    }; // pLV.OnItem = function(vDataRow, vItem, e) {

                            ////////                                    if (mData.SalesReps != undefined) {
                            ////////                                        var pSalesReps;
                            ////////                                        pSalesReps = [];
                            ////////                                        for (var pSalesRepIndex = 0; pSalesRepIndex <= mData.SalesReps.Rows.length - 1; pSalesRepIndex++) {
                            ////////                                            pSalesReps.push({
                            ////////                                                PeopleID: mData.SalesReps.Rows[pSalesRepIndex].PeopleID,
                            ////////                                                PeopleName: mData.SalesReps.Rows[pSalesRepIndex].PeopleName
                            ////////                                            });
                            ////////                                        }; // for (var pSalesRepIndex = 0; pSalesRepIndex <= mData.SalesReps.length - 1; pSalesRepIndex++) {

                            ////////                                        pLV.SetData(pSalesReps);
                            ////////                                        pTxtQty.data('SalesRep', pSalesReps);
                            ////////                                    }; // if (mData.SalesReps != undefined) {
                            ////////                                    pTxtSalesRep.data('lv', pLV);

                            ////////////////////                                } else { // if (proj.PosMode) 
                            ////////////////////                                    pItem.find('[name=spanOriginalPrice]').text($.format.number(CDec2(pDR.DiscountedPrice), '#,##0.00'));
                            ////////////////////                                    pTxtAdjustPrice = pItem.find('[name=txtNewPriceTemplate]')
                            ////////////////////                                .attr('name', 'txtNewPrice')
                            ////////////////////                                .data('UnitQty', pDR.Quantity)
                            ////////////////////                                .bind('textchange', DisplaySubtotal);
                            ////////////////////                                    pTxtQty.data('txtAdjustPrice', pTxtAdjustPrice);

                            ////////////////////                                    if (CDec2(pDR.RequestedUnitPrice) > 0)
                            ////////////////////                                        pItem.find('[name=txtNewPrice]').val($.format.number(CDec2(pDR.RequestedUnitPrice), '#,##0.00'));

                            ////////////////////                                    if (IsNumeric(pDR.CustomerLastInvoicePrice)) {
                            ////////////////////                                        pItem.find('[name=spanLastPrice]').text($.format.number(CDec2(pDR.CustomerLastInvoicePrice), '#,##0.00'));
                            ////////////////////                                        pItem.find('[name=spanLastPriceDate]').text($.format.date(new Date(pDR.CustomerLastInvoiceDate), 'M/d/yyyy'));
                            ////////////////////                                    };
                            ////////////////////                                    pItem.find('[name=butValidateAdjustedPrice]')
                            ////////////////////                                .data('UnitQty', pDR.Quantity)
                            ////////////////////                                .data('StockID', proj.ProductID)
                            ////////////////////                                .click(function () {
                            ////////////////////                                    var pTxtNewPrice = $(this).closest('[name=divPriceAdjustments]')
                            ////////////////////                                        .find('[name=txtNewPrice]');

                            ////////////////////                                    if (CDec2(pTxtNewPrice.val()) <= 0) {
                            ////////////////////                                        pTxtNewPrice.focus();
                            ////////////////////                                        MessageBox('Adjust Price To must be a valid number greater than zero');
                            ////////////////////                                        return;
                            ////////////////////                                    };
                            ////////////////////                                    Submit(true);
                            ////////////////////                                });

                            ////////////////////                                }; // if (proj.PosMode) {
                            // onchange remove qty
                            pTxtQty = pItem.find('[name=txtQty]');
                            var pTxtUnit = pItem.find('[name=spanUnit]:first');
                            var pTxtPrice = pItem.find('[name=spanPrice]:first');
                            var pTxtUnitQty = pItem.find('[name=spanUnitQty]:first');
                            pItem.find('[name=butUnitMinus]')
                            .data('txtQty', pTxtQty)
                            .data('txtUnit', pTxtUnit)
                            .data('txtPrice', pTxtPrice)
                            .data('txtUnitQty', pTxtUnitQty)
                            .bind((InAndroid ? 'touchend' : 'click'), function () {
                                var vTxtQty = $(this).data('txtQty');

                                if ((CDec2(vTxtQty.val()) - 1) >= 0) {
                                    vTxtQty.val(CDec2(vTxtQty.val()) - 1);
                                } else {
                                    vTxtQty.val('0');
                                };
                                prodQty = vTxtQty.val();

                                var vTxtUnit = $(this).data('txtUnit');
                                sessionStorage.ProductUnitMeasure = vTxtUnit.text();
                                if (!isPriceLevelChanged) {
                                    var vTxtPrice = $(this).data('txtPrice');
                                    prodPrice = vTxtPrice.text();
                                    sessionStorage.ProductPrice = prodPrice.replace(/,/g, '');
                                }
                                var vTxtUnitQty = $(this).data('txtUnitQty');
                                sessionStorage.ProductUnitQty = vTxtUnitQty.text();

                                sessionStorage.ProductPriceLevelName = pItem.find('[name=cbPrice]:first option:selected').text();

                                DisplaySubtotal();
                                return;
                            });
                            // onchange add qty
                            pTxtQty = pItem.find('[name=txtQty]');
                            pTxtUnit = pItem.find('[name=spanUnit]:first');
                            pTxtPrice = pItem.find('[name=spanPrice]:first');
                            pTxtUnitQty = pItem.find('[name=spanUnitQty]:first');
                            pItem.find('[name=butUnitPlus]')
                            .data('txtQty', pTxtQty)
                            .data('txtUnit', pTxtUnit)
                            .data('txtPrice', pTxtPrice)
                            .data('txtUnitQty', pTxtUnitQty)
                            .bind((InAndroid ? 'touchend' : 'click'), function () {
                                var vTxtQty = $(this).data('txtQty');

                                if ((CDec2(vTxtQty.val()) + 1) >= 0) {
                                    vTxtQty.val(CDec2(vTxtQty.val()) + 1);
                                } else {
                                    vTxtQty.val('0');
                                };
                                prodQty = vTxtQty.val();


                                var vTxtUnit = $(this).data('txtUnit');
                                sessionStorage.ProductUnitMeasure = vTxtUnit.text();

                                var vTxtPrice = $(this).data('txtPrice');
                                prodPrice = vTxtPrice.text();
                                sessionStorage.ProductPrice = prodPrice.replace(/,/g, '');

                                var vTxtUnitQty = $(this).data('txtUnitQty');
                                sessionStorage.ProductUnitQty = vTxtUnitQty.text();

                                sessionStorage.ProductPriceLevelName = pItem.find('[name=cbPrice]:first option:selected').text();

                                DisplaySubtotal();
                                return;
                            });

                            //additional by janine 

                            pTxtUnit = pItem.find('[name=spanUnit]:first');
                            pTxtPrice = pItem.find('[name=spanPrice]:first');
                            pTxtUnitQty = pItem.find('[name=spanUnitQty]:first');
                            pTxtQty
                            .data('txtUnit', pTxtUnit)
                            .data('txtPrice', pTxtPrice)
                            .data('txtUnitQty', pTxtUnitQty).on('input', function () {

                                var vTxtUnit = $(this).data('txtUnit');
                                sessionStorage.ProductUnitMeasure = vTxtUnit.text();

                                var vTxtPrice = $(this).data('txtPrice');
                                prodPrice = vTxtPrice.text();
                                sessionStorage.ProductPrice = prodPrice.replace(/,/g, '');

                                var vTxtUnitQty = $(this).data('txtUnitQty');
                                sessionStorage.ProductUnitQty = vTxtUnitQty.text();

                                sessionStorage.ProductPriceLevelName = pItem.find('[name=cbPrice]:first option:selected').text();

                                DisplaySubtotal();
                                return;
                            });


                            // onchange price

                            pTxtQty = pItem.find('[name=txtQty]');
                            var pTxtUnit = pItem.find('[name=spanUnit]:first');
                            var pTxtPrice = pItem.find('[name=spanPrice]:first');
                            var pTxtUnitQty = pItem.find('[name=spanUnitQty]:first');
                            var pTxtPriceLevel = pItem.find('[name=cbPrice]:first');
                            pItem.find('[name=cbPrice]')
                            .data('txtQty', pTxtQty)
                            .data('txtUnit', pTxtUnit)
                            .data('txtPrice', pTxtPrice)
                            .data('txtUnitQty', pTxtUnitQty)
                            .data('txtPriceLevel', pTxtPriceLevel)
                            .bind('change', function () {

                                var vTxtUnit = $(this).data('txtUnit');
                                sessionStorage.ProductUnitMeasure = vTxtUnit.text();

                                var vTxtPrice = $(this).data('txtPrice');
                                prodPrice = vTxtPrice.text();
                                sessionStorage.ProductPrice = prodPrice.replace(/,/g, '');

                                var vTxtUnitQty = $(this).data('txtUnitQty');
                                sessionStorage.ProductUnitQty = vTxtUnitQty.text();

                                var vTxtPriceLevel = $(this).data('txtPriceLevel');
                                sessionStorage.ProductPriceLevel = vTxtPriceLevel.val();

                                sessionStorage.ProductPriceLevelName = $(this).find('option:selected').text();

                                DisplaySubtotal();
                                return;
                            });


                            pItem.show();
                        }; // for (var p = 0; p <= vRows.length - 1; p++) {

                        DisplaySubtotal();
                    })(vData.UnitMeasures.Rows); // LoadUnitMeasures

                    $('#divStockMinimumCapacity').hide();
                    $('#divStockMaximumCapacity').hide();

                    if (vData.MinPerProduct != undefined) {
                        $('#divStockMinimumCapacity').show();
                        $('#spanStockMinimumCapacity').text(AccountingFormat(vData.MinPerProduct, 0));
                    }
                    if (vData.MaxPerProduct != undefined) {
                        $('#divStockMaximumCapacity').show();
                        $('#spanStockMaximumCapcity').text(AccountingFormat(vData.MaxPerProduct, 0));
                    }
                    mPriceChange.setData(mData);
                }; // LoadData
            }; // GetInfo

            //additional by janine for hot buttons apr 10,2014
            var prodQty = '';
            var prodPrice = 0;
            //end of additional

            $('[name=butUnitUpdateCart]').click(function (e) {

                e.preventDefault();
                if (Verify())
                    Submit();

                function Verify() {

                    var pResult = true;

                    if (mHasError) {
                        MessageBox('There are problems with this product');
                        return false;
                    };

                    if ((!proj.PosMode) && (sessionStorage.CustomerOrganizationID == undefined)) {
                        MessageBox('No selected customer');
                        return false;
                    };

                    $('[name=txtQty]').each(function (vItem, vIndex) {
                        var pVal;
                        var pInvalid = false;
                        var pThis = $(this);
                        if ((this.validity) && (!this.validity.valid)) {
                            pInvalid = true;
                        }
                        pVal = pThis.val();
                        if (!pInvalid) {
                            if (CStr2(pVal).trim().length > 0) {
                                if (!mData.AllowDecimal)
                                    pInvalid = (!IsInteger(pVal));

                                prodQty = pVal; //additional by janine do not remove. para ni sa hot buttons. may 16 1pm


                            }
                        }
                        if (pInvalid) {
                            MessageBox('Quantity must be a valid whole number.', 'Invalid quantity')
                            .on('pagehide', function () {
                                pThis.focus();
                            });
                            pResult = false;
                            return false;
                        }; // if (!IsNumeric(pThis.val())) {
                        if (CDec2(pVal) < 0) {
                            MessageBox('Quantity must not be a negative number. You entered ' + pVal, 'Invalid quantity')
                            .on('pagehide', function () {
                                pThis.focus();
                            });
                            pResult = false;
                            return false;
                        };
                    }); // $('[name=txtQty]').each(function(vItem, vIndex) {

                    if (pResult) {
                        $('[name=txtNewPrice]').each(function (vItem, vIndex) {
                            var pThis = $(this);
                            var pVal = pThis.val();
                            if ((CStr2(pVal).trim().length > 0) && (!IsNumeric(pVal))) {
                                MessageBox('Adjust Price To must be a valid number. You entered ' + pVal, 'Invalid Adjust Price To')
                        .on('pagehide', function () {
                            pThis.focus();
                        });
                                pResult = false;
                                return false;
                            }; // if (!IsNumeric(pThis.val())) {
                        }); // $('[name=txtNewPrice]').each(function(vItem, vIndex) {
                    }; // if (pResult) {

                    return pResult;

                }; // function Verify() {

            }); // $('#butUnitUpdateCart').click(function(e) {

            function Submit(vVerifyOnly) {
                // PARAMS:
                //  vVerifyOnly: boolean optional, verify price request

                var pParams;
                var pData = {};
                var p = 0;
                var pStockID;
                var pUrl;

                //                    var pSalesRep;
                var pSalesRepIDs;
                var pProductSerial;
                var intIndex;

                if (proj.ProductID != undefined)
                    pStockID = proj.ProductID;
                else {
                    pParams = GetUrlParameters();
                    pStockID = pParams.id;
                };

                pData.StockID = pStockID;
                pData.CustomerID = sessionStorage.CustomerOrganizationID;
                //additional by janine apr 1 2014
                if (sessionStorage.GasPumpNo != '')
                    pData.CustomerTableNo = 'GP_' + sessionStorage.GasPumpNo;

                if (sessionStorage.CustomerTableNo != '')
                    pData.CustomerTableNo = 'TBL_' + sessionStorage.CustomerTableNo;

                //end of additional
                pData.OrderID = sessionStorage.CartOrderID;
                pData.NewCartMode = localStorage.NewCartMode;
                pData.FowstID = sessionStorage.FowstID;
                pData.ProductID = pStockID;
                pData.Stocks = [];

                pData.Remarks = '';
                pData.IsTestOnly = 0; //this means, it's not a test! additional june 2 by janine

                $('[name=txtQty]').each(function (vItem, vIndex) {
                    var pThis = $(this);
                    var pTxtAdjustPrice;
                    var pItem;
                    var pRequestedPrice;
                    var pPriceRequestData;

                    if (IsNumeric(pThis.val())) {
                        pItem = pThis.closest('[name=divUnit]');
                        pData.Stocks[p] = {
                            Qty: pThis.val(),
                            UnitQty: pThis.data('UnitQty'),
                            UnitPrice: pThis.data('Price'),
                            PriceLevelID: pItem.find('[name=cbPrice]').val()
                        };

                        pPriceRequestData = pThis.data('requestedPriceData') || {};
                        if (CDec2(pPriceRequestData.netAmount) > 0) {
                            pData.Stocks[p].RequestedUnitPrice = CDec2(pPriceRequestData.netAmount);
                            pData.Stocks[p].PriceLevelID = pPriceRequestData.priceLevelID;
                        }
                        p++;
                    }; // if (!IsNumeric(pThis.val())) {
                }); // $('[name=txtQty]').each(function(vItem, vIndex) {

                //Sales Reps
                pSalesRepIDs = [];
                intIndex = 0;

                for (var i = 0; i <= intSalesRepCntr; i++) {

                    if (sessionStorage["POSSalesRep-" + i] != undefined) {

                        arSalesRep = new Array();
                        arSalesRep = JSON.parse(sessionStorage["POSSalesRep-" + i]);


                        pSalesRepIDs[intIndex] = {
                            SalesRepID: arSalesRep[intSalesRepID],
                            ProductID: pStockID,
                            RecState: arSalesRep[intSalesRepState]
                        };

                        intIndex = intIndex + 1;

                    }
                }
                pData.SalesRep = pSalesRepIDs;
                //Sales Rep

                //Product Serial
                pProductSerial = [];
                intIndex = 0;

                for (var i = 0; i <= intProductSerialCntr; i++) {

                    if (sessionStorage["POSProductSerial-" + i] != undefined) {

                        arProductSerial = new Array();
                        arProductSerial = JSON.parse(sessionStorage["POSProductSerial-" + i]);


                        pProductSerial[intIndex] = {
                            ProductID: pStockID,
                            ProductSerial: arProductSerial[intProductSerial],
                            RecState: arProductSerial[intProductSerialState]
                        };

                        intIndex = intIndex + 1;

                    }
                }
                pData.ProductSerial = pProductSerial;
                //Product Serial

                if (OfflineMode) {
                    LoadData(
                        Stock_offline().UpdatePos(pData)
                    );
                } else {
                    if (vVerifyOnly) {
                        pUrl = 'Stock/ValidateAdjustedPrice';
                    } else {
                        pUrl = 'Stock/UpdatePos';
                    };

                    AjaxSend({
                        url: pUrl,
                        data: pData,
                        success: function (vData) {
                            LoadData(vData);
                        } // success
                    }); // AjaxSend
                }; // if (OfflineMode) {

                function LoadData(vData) {

                    var pMessage;
                    var pDivDetails;
                    var pLnkDetails;

                    if (vData == undefined) return;
                    if (vData.Error != undefined) {
                        if (CDec2(vData.ReloadBrowser) != 0)
                            MessageBox(vData.Error, 'Error', null, {
                                OnClose: function () {
                                    location.reload();
                                }
                            });
                        else
                            MessageBox(vData.Error);
                        return;
                    };
                    if (vData.Result != 'ok') {
                        pMessage = $('<div>').html('The price you have requested is too low. Please do one of the following:<br />'
                            + '<ol>'
                            + '<li>Take the original selling price</li>'
                            + '<li>Increase the requested price</li>'
                            + '<li>Try a price request again after adding some more items to your cart.</li>'
                            + '</ol>'
                            + '• Price requests are approved/ disapproved based on transaction profitability. The more items you put into the cart, the more likely that your price request will be approved.<br />'
                        );
                        pLnkDetails = $('<a>')
                    .attr('href', '#')
                    .text('Details')
                    .click(function (e) {
                        e.preventDefault();
                        pLnkDetails.hide();
                        pDivDetails.show();
                    });
                        pMessage.append(pLnkDetails);
                        pDivDetails = $('<div>').html('<h3>Details</h3>'
                    + '<h5>Decision Factors for Denying Price Request:</h5>'
                    + '<ol>'
                    + '<li><b>Is this line item’s Gross Profit Margin (GPM) within acceptable limits?: No'
                    + '</li>'
                    + '</ol>' + HtmlEncode(vData.Warning)
                ).hide();
                        pMessage.append(pDivDetails);
                    }; // if (vData.Result != 'ok') {
                    if (vVerifyOnly) {
                        if (vData.Result == 'ok') {
                            MessageBox('Price adjustment request is acceptable.');
                        } else {
                            MessageBox(pMessage, 'Price Request Denied', null, {
                                EncodeHtml: false
                            });
                        };
                    } else {
                        if ((vData.Result == 'ok')
                    || (vData.Warning != undefined)) {

                            function Done() {
                                sessionStorage.CartOrderID = vData.OrderID;
                                localStorage.removeItem('NewCartMode');
                                if ($.isFunction(proj.ShowCart)) {

                                    proj.ShowCart();

                                    //additional by janine. remember pos mode apr 10, 2014
                                    if (sessionStorage.PosMode == 'Hot Button Mode') {


                                        startclock('#clockHotButtons');

                                        ChangePage('/SalesMenu/HotButtons');

                                        $('#divSpace').hide();
                                        if (prodQty != '') {
                                            sessionStorage.LastQty = prodQty;
                                            if (parseFloat(prodQty) > 0) {
                                                $('#txtOrder').html($('#spanStockDescription').text() + '<br/><br/>' + sessionStorage.ProductPriceLevelName + ': ' + AccountingFormat(parseFloat(sessionStorage.ProductPrice)) + '<br/>' + 'Item total: ' + AccountingFormat(parseFloat(sessionStorage.ProductPrice) * parseFloat(prodQty)));
                                                $('#txtQtyUnit').html('<label style="font-size: 68px;">' + prodQty + '</label><br/>' + sessionStorage.ProductUnitMeasure);
                                            } else {
                                                proj.ProductID = '';
                                                $('#txtOrder').html('');
                                                $('#txtQtyUnit').html('');
                                            }
                                        }
                                    }

                                } else {
                                    MessageBox('Cart was successfully updated', 'Success');
                                    window.location = GetWindowPathFormat('salesmenu') + '#cart';
                                };
                            }; // function Done() {

                            if (vData.Warning != undefined)
                                MessageBox(pMessage, 'Warning', null, {
                                    OnClose: Done,
                                    EncodeHtml: false
                                });
                            else
                                Done();
                        } else {
                            MessageBox((vData.Error || 'An error occured while updating cart'), 'Failed',
                            [
                                {
                                    Text: 'Retry',
                                    Click: function () {
                                        if (Verify())
                                            Submit();
                                    }
                                },
                                {
                                    Text: 'Close this message'
                                }
                            ]
                        ); // MessageBox((vData.Error || 'An error
                        }; // if (vData.Result == 'ok') {
                    }; // if (vVerifyOnly) {
                }; // LoadData
            }; // Submit()

            function DisplaySubtotal() {

                var pTotal = 0;

                $('[name=txtQty]').each(function (vItem, vIndex) {
                    var pUnitPrice;
                    var pThis = $(this);
                    if (IsNumeric(pThis.val())) {
                        //////////////////                            if (!proj.PosMode)
                        //////////////////                                pUnitPrice = pThis.data('txtAdjustPrice').val();
                        if (CDec2(pUnitPrice) <= 0)
                            pUnitPrice = pThis.data('Price');
                        pTotal += pThis.val() * pUnitPrice;

                    }; // if (!IsNumeric(pThis.val())) {
                }); // $('[name=txtQty]').each(function(vItem, vIndex) {

                if (pTotal > proj.MaxNumber)
                    $('#spanSubtotal').text('Please be realistic');
                else
                    $('#spanSubtotal').text(AccountingFormat(pTotal));

            }; // function DisplaySubtotal() {

            function TxtQtyChanged() {

                var pThis = $(this);

                //                    if (IsNumeric(pThis.val()))
                //                        pThis.data('divMore').show();
                //                    else
                //                        pThis.data('divMore').hide();
                DisplaySubtotal();

            }; // function TxtQtyChanged() {

            function ShowStockInfo(vShow) {
                if (vShow) {
                    $('[name=divUnit]').show();
                    $('#divSubTotal').show();
                } else {
                    $('[name=divUnit]').hide();
                    $('#divSubTotal').hide();
                }
            }; // function ShowStockInfo

            function ShowSalesRepresentativeList(vShow) {
                if (vShow) {
                    $('#divProductSalesPersonContainer').show();
                } else {
                    $('#divProductSalesPersonContainer').hide();
                }
            }; // function ShowStockInfo

            function ShowProductSerialList(vShow) {
                if (vShow) {
                    $('#divProductSerialContainer').show();
                } else {
                    $('#divProductSerialContainer').hide();
                }
            }; // function ShowStockInfo

            function ShowInventoryBalances(vShow) {
                if (vShow) {
                    $('#divProductInventoryBalances').show();
                } else {
                    $('#divProductInventoryBalances').hide();
                }
            }; // function ShowStockInfo

            function LoadPOSSalesRep(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arSalesRep = new Array();
                    arSalesRep[intSalesRepRecID] = vRows[p].ID;
                    arSalesRep[intSalesRepID] = vRows[p].PeopleID;
                    arSalesRep[intSalesRepName] = vRows[p].PeopleName;
                    arSalesRep[intSalesRepState] = "New";

                    sessionStorage["POSSalesRep-" + p] = JSON.stringify(arSalesRep);

                };

                intSalesRepCntr = vRows.length;
                LoadPOSSalesRepList();

            };

            function IsSalesRepExist(vSalesRepID) {

                for (var i = 0; i <= intSalesRepCntr; i++) {

                    if (sessionStorage["POSSalesRep-" + i] != undefined) {

                        arSalesRep = new Array();
                        arSalesRep = JSON.parse(sessionStorage["POSSalesRep-" + i]);

                        if (arSalesRep[intSalesRepState] != "Remove") {

                            if (arSalesRep[intSalesRepID] == vSalesRepID) {
                                return true;
                            }

                        }

                    }
                }

                return false;
            }

            function LoadPOSSalesRepList() {
                var pLV = $('#lstSalesRep');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intSalesRepCntr; i++) {

                    if (sessionStorage["POSSalesRep-" + i] != undefined) {

                        arSalesRep = new Array();
                        arSalesRep = JSON.parse(sessionStorage["POSSalesRep-" + i]);

                        if (arSalesRep[intSalesRepState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                                .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arSalesRep)
                                    .append($('<h3>')
                                            .attr("style", "white-space:normal")
                                            .text(arSalesRep[intSalesRepName])
                                    )
                                )
                                .append($('<a>')
                                    .attr('ID', i)
                                    .attr('href', "#")
                                    .attr('data-icon', "minus")
                                    .data('Data', arSalesRep)
                                    .click(function (e) {
                                        e.preventDefault();

                                        sessionStorage["POSSalesRepToRemoveIndex"] = $(this).attr('ID');
                                        sessionStorage["POSSalesRepToRemove"] = JSON.stringify($(this).data('Data'));

                                        MessageBox("Remove this record?",
                                            'Remove',
                                            [
                                                { Text: 'Yes',
                                                    Click: function () {

                                                        var arSalesRepToRemove = new Array();
                                                        arSalesRepToRemove = JSON.parse(sessionStorage["POSSalesRepToRemove"]);
                                                        intSalesRepIndex = sessionStorage["POSSalesRepToRemoveIndex"];

                                                        arSalesRepToRemove[intSalesRepState] = "Remove";
                                                        sessionStorage["POSSalesRep-" + intSalesRepIndex] = JSON.stringify(arSalesRepToRemove);

                                                        LoadPOSSalesRepList();

                                                        sessionStorage.removeItem("POSSalesRepToRemove");
                                                        sessionStorage.removeItem("POSSalesRepToRemoveIndex");
                                                        ChangePage('Stock');
                                                    }
                                                }
                                                ,
                                                { Text: 'No'
                                                }
                                            ]
                                        );

                                    })
                            );

                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };

            function ClearPOSSalesRepSession() {
                for (var p = 0; p <= 100; p++) {
                    if (sessionStorage["POSSalesRep-" + p] != undefined) {
                        sessionStorage.removeItem("POSSalesRep-" + p);
                    }
                }
            }

            function LoadPOSProductSerial(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arProductSerial = new Array();
                    arProductSerial[intProductSerialRecID] = vRows[p].ID;
                    arProductSerial[intProductSerial] = vRows[p].SerialNumber;
                    arProductSerial[intProductSerialState] = "New";

                    sessionStorage["POSProductSerial-" + p] = JSON.stringify(arProductSerial);

                };

                intProductSerialCntr = vRows.length;
                LoadPOSProductSerialList();

            };

            function IsProductSerialExist(vProductSerial) {

                for (var i = 0; i <= intProductSerialCntr; i++) {

                    if (sessionStorage["POSProductSerial-" + i] != undefined) {

                        arProductSerial = new Array();
                        arProductSerial = JSON.parse(sessionStorage["POSProductSerial-" + i]);

                        if (arProductSerial[intSalesRepState] != "Remove") {

                            if (arProductSerial[intProductSerial] == vProductSerial) {
                                return true;
                            }

                        }

                    }
                }

                return false;
            }

            function LoadPOSProductSerialList() {
                var pLV = $('#lstProductSerial');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intProductSerialCntr; i++) {

                    if (sessionStorage["POSProductSerial-" + i] != undefined) {

                        arProductSerial = new Array();
                        arProductSerial = JSON.parse(sessionStorage["POSProductSerial-" + i]);

                        if (arProductSerial[intProductSerialState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                                .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arProductSerial)
                                    .append($('<h3>')
                                            .attr("style", "white-space:normal")
                                            .text(arProductSerial[intProductSerial])
                                    )
                                )
                                .append($('<a>')
                                    .attr('ID', i)
                                    .attr('href', "#")
                                    .attr('data-icon', "minus")
                                    .data('Data', arProductSerial)
                                    .click(function (e) {
                                        e.preventDefault();

                                        sessionStorage["POSProductSerialToRemoveIndex"] = $(this).attr('ID');
                                        sessionStorage["POSProductSerialToRemove"] = JSON.stringify($(this).data('Data'));

                                        MessageBox("Remove this record?",
                                            'Remove',
                                            [
                                                { Text: 'Yes',
                                                    Click: function () {

                                                        var arPOSProductSerialToRemove = new Array();
                                                        arPOSProductSerialToRemove = JSON.parse(sessionStorage["POSProductSerialToRemove"]);
                                                        intProductSerialIndex = sessionStorage["POSProductSerialToRemoveIndex"];

                                                        arPOSProductSerialToRemove[intProductSerialState] = "Remove";
                                                        sessionStorage["POSProductSerial-" + intProductSerialIndex] = JSON.stringify(arPOSProductSerialToRemove);

                                                        LoadPOSProductSerialList();

                                                        sessionStorage.removeItem("POSProductSerialToRemove");
                                                        sessionStorage.removeItem("POSProductSerialToRemoveIndex");
                                                        ChangePage('Stock');

                                                    }
                                                }
                                                ,
                                                { Text: 'No'
                                                }
                                            ]
                                        );

                                    })
                            );

                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };

            function ClearPOSProductSerialSession() {
                for (var p = 0; p <= 100; p++) {
                    if (sessionStorage["POSProductSerial-" + p] != undefined) {
                        sessionStorage.removeItem("POSProductSerial-" + p);
                    }
                }
            }

            function EnlistSerial(vSerial) {
                if ($.trim(vSerial) != "") {
                    //                        if (!IsProductSerialExist($('#txtProductSalesRepSearch').val())) {
                    IsNewProductSerial = true
                    intProductSerialCntr = intProductSerialCntr + 1;
                    intProductSerialIndex = intProductSerialCntr;

                    arProductSerial = new Array();
                    arProductSerial[intProductSerialRecID] = "";
                    arProductSerial[intProductSerial] = vSerial;
                    arProductSerial[intProductSerialState] = (IsNewSalesRep ? "New" : "Edit");

                    sessionStorage["POSProductSerial-" + intProductSerialIndex] = JSON.stringify(arProductSerial);

                    $('#txtProductSerial').val('');

                    LoadPOSProductSerialList();
                    //                        }
                }
            }

            $('#txtProductSalesRepSearch').keydown(function (e) {
                if (e.which == 13) {
                    //Browser.LibraryOrganizationEmployeeFOWST.InputData.EmployeesOnly = 1;
                    Browser.LibraryOrganizationEmployeeFOWST.Click = function (e) {

                        if (!IsSalesRepExist(e.ID)) {
                            IsNewSalesRep = true
                            intSalesRepCntr = intSalesRepCntr + 1;
                            intSalesRepIndex = intSalesRepCntr;

                            arSalesRep = new Array();
                            arSalesRep[intSalesRepRecID] = "";
                            arSalesRep[intSalesRepID] = e.ID;
                            arSalesRep[intSalesRepName] = e.PeopleName;
                            arSalesRep[intSalesRepState] = (IsNewSalesRep ? "New" : "Edit");

                            sessionStorage["POSSalesRep-" + intSalesRepIndex] = JSON.stringify(arSalesRep);

                            LoadPOSSalesRepList();
                        }
                    };
                    Browser.LibraryOrganizationEmployeeFOWST.Show($('#txtProductSalesRepSearch').val());
                };
            }); // pTxtSalesRep.keydown(function(e) {

            $('#txtProductSerial').keydown(function (e) {
                if (e.which == 13) {
                    EnlistSerial($('#txtProductSerial').val());
                };
            }); // txtProductSalesRepSearch.keydown(function(e) {

            $('#butSerialBarCode').click(function () {

                mobile.Scan({
                    Success: function (vData) {
                        EnlistSerial(vData.Value);
                    }
                });

            });

            $('#butReportNoPrice').click(function (e) {
                e.preventDefault();
                AjaxSend({
                    url: 'Stock/AlertNoPrice',
                    data: { ProductID: proj.ProductID },
                    success: function (vData) {
                        if (vData.Error)
                            MessageBox(vData.Error);
                        else if (vData.Result == 'ok') {
                            MessageBox('Alert was successfully saved', 'Success', null, {
                                OnClose: function () {
                                    if ($.isFunction(proj.ShowCart))
                                        proj.ShowCart(1);
                                } // OnClose: function() {
                            }); // MessageBox('Alert was successfully saved', 'Success', [], {

                        }; // else if (vData.Result == 'ok') {
                    } // success: function(vData) 
                });
            }); // $('#butReportNoPrice').click(function(e) {

            $('#btnStockInfo').click(function (e) {
                e.preventDefault();

                ShowStockInfo(true);
                ShowProductSerialList(false);
                ShowSalesRepresentativeList(false);
                ShowInventoryBalances(false);

                $("#lblPageHeader").text("Stock Info");
                ChangePage('Stock');

            });

            $('#btnSalesPerson').click(function (e) {
                e.preventDefault();

                ShowStockInfo(false);
                ShowProductSerialList(false);
                ShowInventoryBalances(false);
                ShowSalesRepresentativeList(true);

                $("#lblPageHeader").text("Indicate Sales Person");
                ChangePage('Stock');

            });

            $('#btnSerialNumber').click(function (e) {
                e.preventDefault();

                ShowStockInfo(false);
                ShowSalesRepresentativeList(false);
                ShowInventoryBalances(false);
                ShowProductSerialList(true);

                $("#lblPageHeader").text("Serial Number(s)");
                ChangePage('Stock');

            });

            $('#btnInventoryBalances').click(function (e) {
                e.preventDefault();

                ShowStockInfo(false);
                ShowSalesRepresentativeList(false);
                ShowProductSerialList(false);
                ShowInventoryBalances(true);

                $("#lblPageHeader").text("Inventory Balances");
                ChangePage('Stock');

                GetInventoryBalances();
            });

            $('#butSaveGotoStockInfo').click(function (e) {
                e.preventDefault();

                ShowSalesRepresentativeList(false);
                ShowProductSerialList(false);
                ShowInventoryBalances(false);
                ShowStockInfo(true);

                $("#lblPageHeader").text("Stock Info");
                ChangePage('Stock');

            });

            function GetInventoryBalances() {
                AjaxSend({
                    url: 'Stock/GetInventoryBalance',
                    data: { OrgID: sessionStorage.OrgID, StockID: proj.ProductID },
                    success: function (vData) {
                        $('#tblInventoryBalance').html('');

                        tData = $('<tr>')
                                    .append($('<th>')
                                        .text('FOWST')
                                    )
                                    .append($('<th>')
                                        .text('Balance')
                                    )
                                    .append($('<br />'))

                        $('#tblInventoryBalance').append(tData);

                        var Total = 0;

                        for (i = 0; i < vData.Results.Rows.length; i++) {

                            Total += parseInt(vData.Results.Rows[i].Balance);

                            tData = $('<tr>')
                                    .append($('<td>')
                                        .text(vData.Results.Rows[i].FOWST)
                                    )
                                    .append($('<td>')
                                        .text(parseInt(vData.Results.Rows[i].Balance))
                                    )

                            $('#tblInventoryBalance').append(tData);
                        }


                        tData = $('<tr>')
                                    .append($('<td>')
                                        .text('Total :')
                                    )
                                    .append($('<td>')
                                        .text(parseInt(Total))
                                    )

                        $('#tblInventoryBalance').append(tData);

                        try {
                            $('#lstProductInventoryBalances').listview('refresh');
                        }
                        catch (e) {
                            $('#lstProductInventoryBalances').listview();
                        }
                    } // success
                }); // AjaxSend
            }

            function mPriceChange_onInvalid(vResult) {

                mPriceChange.element.find('[name=divRequestPriceMessage]').text(vResult.error);

            }

            function mPriceChange_onSubmit() {

                var pResult = mPriceChange.validate();
                var pPriceRequestData;

                if (pResult.result == 'ok') {
                    mPriceChange.element.find('[name=divRequestPriceMessage]').text('');
                    pPriceRequestData = mCurrentyTxtQty.data('requestedPriceData') || {};

                    // uncomment if you want requested price
//                    if (mPriceChange.netAmount == 0) {
//                        // mDivRequestedUnitPrice.find('[name=spanRequestedUnitPrice]').text('');
//                        mCurrentyTxtQty.data('SpanRequestedPrice').text('');
//                        mDivRequestedUnitPrice.hide();
//                    } else {
//                        // mDivRequestedUnitPrice.find('[name=spanRequestedUnitPrice]').text($.format.number(CDec2(mPriceChange.model.netAmount), '#,###.00'));
//                        mCurrentyTxtQty.data('SpanRequestedPrice').text($.format.number(CDec2(mPriceChange.model.netAmount), '#,###.00'));
//                        mDivRequestedUnitPrice.show();
//                    }

                    // comment if you want requested price
                    mCurrentyTxtQty.data('spanPrice').text(AccountingFormat(mPriceChange.model.netAmount));
                    mCurrentyTxtQty.data('Price', mPriceChange.model.netAmount);

                    pPriceRequestData.netAmount = mPriceChange.model.netAmount;
                    pPriceRequestData.priceLevelID = mPriceChange.getPriceLevelID();
                    mCurrentyTxtQty.data('requestedPriceData', pPriceRequestData);
                    mPriceChange.popupClose();
                }

                return false;

            } // END function mPriceChange_onSubmit() {

            function ShowFreeTypePriceDialog() {

                var pRequestedPriceData = mCurrentyTxtQty.data('requestedPriceData') || {};
                var pNetAmount = CDec2(pRequestedPriceData.netAmount);
                
                mPriceChange.unitQuantity = mCurrentyTxtQty.data('UnitQty');
                mPriceChange.setPriceLevelID(pRequestedPriceData.priceLevelID);
                if (pNetAmount == 0)
                    mPriceChange.model.setDiscountPercent(0, 'baseAmount');
                else
                    mPriceChange.model.setNetAmount(pNetAmount);
                mPriceChange.popCombo();
                mPriceChange.popup();

            } // END function ShowFreeTypePriceDialog() {
        }); // $(function() {
    }); // require offline 
});