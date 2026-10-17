require(['common'
    ], function () {

        //        require([(OfflineMode ? '/Scripts3/Stock_offline.js?crc=' : '')], function () {
        $(function () {


            var savedQuantity = 0;

            var mData; // from 'Stock/GetInfoPos' or 'Stock/GetInfo'
            var mTxtSalesRep; // the textbox opening the people browser
            var mHasError = 1;
            var mDivRequestPrice = $('#divRequestPrice');
            var mTxtRequestedPrice = $('#txtRequestedPrice');
            var mFormRequestedPrice = $('#formRequestedPrice');
            var mDivRequestedUnitPrice; // set during "Request Special Price" click
            //                var mPriceChange = new App.controller.PriceChange();
            var invoiceItemID = '';
            var isPriceLevelChanged = false; //additional by janine
            var isApplyFilter = 0;
            //                $("[data-role=header]").fixedtoolbar({ tapToggle: false });
            $('#divStockMinimumCapacity').hide();
            $('#divStockMaximumCapacity').hide();

            setTimeout(function () {
                //            $.mobile.activePage.bind('pageshow', PageShow);
            }, 1000);
            $.mobile.activePage.bind('pageshow', PageShow);
            //                mPriceChange.onSubmit = mPriceChange_onSubmit;

            function PageShow() {
                if (ValidPrevPage(arguments) || window.NewStockBrowser) {
                    $('#divUnit').hide();
                    ////////////////////                                                if (proj.PosMode)
                    $('#divUnit').find('[data-role=collapsible_clone]').remove();
                    ////////////////////                                                else
                    ////////////////////                                                    $('#divUnit').find('[data-role=collapsible_clone_pos]').remove();

                    window.NewStockBrowser = false;

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



            $('#lnkStockBack').click(function (e) {
                e.preventDefault();

                proj.RefreshCart();
            });


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
                    NewCartMode: '0', //localStorage.NewCartMode
                    PosMode: "so",  //additional by janine sept 5 2014 for sales order taker
                    PickingListMode: 1,
                    FowstBinID: sessionStorage.BinID,
                    SectionID: proj.SectionID

                };

                //                    if (OfflineMode) {
                //                        pData = Stock_offline().GetInfoPos(pData);
                //                        LoadData(pData);
                //                    } else { // if (OfflineMode) {
                AjaxSend({
                    url: pUrl,
                    data: pData,
                    success: function (vData) {
                        LoadData(vData);
                    } // success
                }); // AjaxSend
                //                    }; // if (OfflineMode) {

                function LoadData(vData) {

                    var pStr;

                    if (vData.Error != undefined) {
                        MessageBox(vData.Error, 'Error');
                        return;
                    } else {
                        mHasError = 0;
                    };

                    mData = vData;


                    $('#lblBinName').text(sessionStorage.BinName);

                    //                        //additional by janine feb 21 2015 12nn - tick the vat exempt selection
                    //                        if (OfflineMode) {
                    //                            if (vData.VATInfo.Rows.length > 0) {

                    //                                if (CDec2(vData.VATInfo.Rows[0].VATExemptSale) > 0 && CDec2(vData.VATInfo.Rows[0].VATableSale) == 0)
                    //                                    $('#selVATExempt').val('1').slider('refresh');
                    //                                else
                    //                                    $('#selVATExempt').val('0').slider('refresh');

                    //                            }



                    //                        }

                    //
                    $('#spanStockNumber').text(vData.ProductNo);
                    $('#spanStockDescription').text(vData.ProductName);

                    if (CStr2(sessionStorage.IsReplacement) != 'true')
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
                        var pQty;

                        pLastItem = pTemplate;
                        if (proj.PLMode)
                            pTemplate.find('[name=divPriceLevel]').hide();

                        for (var p = 0; p <= vRows.length - 1; p++) {
                            pDR = vRows[p];
                            pItem = pTemplate.clone(true)
                                .attr('id', '')
                                .attr('name', 'divUnit');
                            ////////////////////                                                                if (proj.PosMode)
                            pDivPriceLevel = pItem.find('[name=divPriceLevel]');
                            ////////////////////                                                                else
                            ////////////////////                                                                    pDivMore = pItem.find('[data-role=collapsible_clone]').collapsible();

                            ////////////////////        
                            //additional                                                        pDivSalesRep.hide();
                            savedQuantity = pDR.OrderedQty;

                            pOrderedQty = '';
                            if (IsNumeric(pDR.OrderedQty))
                                pOrderedQty = $.format.number(CDec2(pDR.OrderedQty), proj.QuantityFormat);
                            
                            function GetWithdrawnQty(vUnitQty) {
                                vUnitQty = CDec2(vUnitQty);
                                if (vUnitQty == 0)
                                    return 0;
                                var pRows = GetStringObject('Withdrawn.Rows', vData);
                                if (!$.isArray(pRows))
                                    return 0;
                                for (var p = 0; p <= pRows.length - 1; p++) {
                                    if (CDec2(pRows[p].UnitQty) == vUnitQty)
                                        return CDec2(pRows[p].PreparedQuantity);
                                }

                                return 0;
                            } // END GetWithdrawnQty

                            pQty = GetWithdrawnQty(pDR.Quantity);
                            pTxtQty = pItem.find('[name=txtQtyTemplate]')
                                .attr('name', 'txtQty')
                                .data('UnitQty', pDR.Quantity)
                                .data('UnitMeasure', pDR.UnitMeasure)
                                .data('Price', pDR.DiscountedPrice)
                                .data('SpanRequestedPrice', pItem.find('[name=spanRequestedUnitPrice]'))
                                .val(pQty) //sessionStorage.CurrentPickedQty
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

                            pSelect.val(pDR.PriceLevelID).attr('selected');
                            pSelect.attr('name', 'cbPrice');
                            pSelect.selectmenu();
                            pSelect.selectmenu('refresh', true);
                            pSelect.change(function () {
                                var pThis = $(this);
                                var pItem = pThis.closest('[name=divUnit]');
                                var pDR;
                                var pTxtQty;

                                if (CBool(sessionStorage.AllowPriceAdjustment)
                                    && (pThis.val() == 'FreeTypeAPrice')) {
                                    // Free Type a Price
                                    mDivRequestedUnitPrice = $(pThis.find(':selected')).data('div');
                                    ShowFreeTypePriceDialog();
                                    return;
                                }

                                pTxtQty = pItem.find('[name=txtQty]');
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

                            pDivPriceLevel.trigger('create');


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
                if (CStr2(sessionStorage.GasPumpNo) != '')
                    pData.CustomerTableNo = 'GP_' + sessionStorage.GasPumpNo;

                if (CStr2(sessionStorage.CustomerTableNo) != '')
                    pData.CustomerTableNo = 'TBL_' + sessionStorage.CustomerTableNo;

                //end of additional
                pData.OrderID = sessionStorage.CartOrderID;
                pData.NewCartMode = '0'; // localStorage.NewCartMode;
                pData.FowstID = sessionStorage.FowstID;
                pData.ProductID = pStockID;
                pData.Stocks = [];

                pData.Remarks = '';
                pData.IsTestOnly = 0; //this means, it's not a test! additional june 2 by janine

                //additional customer demographic oct 23
                pData.DeliveryMethod = sessionStorage.DeliveryMethod;
                pData.OrderMethod = sessionStorage.OrderMethod;
                pData.Gender = sessionStorage.Gender;
                pData.Age = sessionStorage.Age;
                //additonal for sm coin VAT EXEMPT
                pData.IsVATExempt = $('#selVATExempt').val();
                pData.BinID = sessionStorage.FowstBinID;

                proj.PosMode = true;
                pData.ApplyFilter = isApplyFilter;

                pData.RefStockID = (CStr2(sessionStorage.IsReplacement) == 'true') ? CStr2(sessionStorage.StockItemID) : CStr2(sessionStorage.RefStockID);
                pData.PosMode = (CStr2(sessionStorage.IsReplacement) == 'true') ? "so" : "pos";
                pData.StockItemID = (CStr2(sessionStorage.IsReplacement) == 'true') ? '' : CStr2(sessionStorage.StockItemID);

                var pTxtQtys = $('[name=txtQty]');

                $('[name=txtQty]').each(function (vItem, vIndex) {
                    var pThis = $(this);
                    var pTxtAdjustPrice;
                    var pItem;
                    var pRequestedPrice;

                    function Proceed() {
                        var pStockData =  {
                            Qty: pThis.val(),
                            UnitQty: pThis.data('UnitQty'),
                            UnitMeasure: pThis.data('UnitMeasure')
                        };
                        pData.Stocks.push(pStockData);
                        pItem = pThis.closest('[name=divUnit]');
                        pStockData.PriceLevelID = pItem.find('[name=cbPrice]').val();
                        pRequestedPrice = CDec2(pThis.data('SpanRequestedPrice').text());
                        if (pRequestedPrice > 0)
                            pStockData.RequestedUnitPrice = pRequestedPrice;

                        //Sales Reps
                        pSalesRepIDs = [];

                        pData.SalesRep = pSalesRepIDs;
                        //Sales Rep

                        //Product Serial
                        pProductSerial = [];

                        pData.ProductSerial = pProductSerial;
                        //Product Serial

                        //function LoadData(vData) {

                        //    var pMessage;
                        //    var pDivDetails;
                        //    var pLnkDetails;

                        //    if (vData == undefined) return;
                        //    if (vData.Error != undefined) {
                        //        if (CDec2(vData.ReloadBrowser) != 0)
                        //            MessageBox(vData.Error, 'Error', null, {
                        //                OnClose: function () {
                        //                    location.reload();
                        //                }
                        //            });
                        //        else
                        //            MessageBox(vData.Error);
                        //        return;
                        //    };


                        //    if ((vData.Result == 'ok') || (vData.Warning != undefined)) {

                        //        function Done() {
                        //            //                                            if (vData.OrderID != undefined)
                        //            //                                                sessionStorage.CartOrderID = vData.OrderID;
                        //            //                               
                        //            localStorage.removeItem('NewCartMode');
                        //            //update qty done!
                        //            sessionStorage.QtyLeft = CDec2(sessionStorage.AllowableQty) - CDec2(vData.PreparedQty);
                        //            //                                            history.back();

                        //            proj.RefreshCart();

                        //            if (CStr2(sessionStorage.IsReplacement) == 'true')
                        //                $.mobile.changePage('#pageSalesOrder');



                        //        }; // function Done() {

                        //        if (vData.Warning != undefined)
                        //            MessageBox(pMessage, 'Warning', null, {
                        //                OnClose: Done,
                        //                EncodeHtml: false
                        //            });
                        //        else
                        //            Done();
                        //    } else {
                        //        MessageBox((vData.Error || 'An error occured while updating cart'), 'Failed',
                        //                        [
                        //                            {
                        //                                Text: 'Retry',
                        //                                Click: function () {
                        //                                    if (Verify())
                        //                                        Submit();
                        //                                }
                        //                            },
                        //                            {
                        //                                Text: 'Close this message'
                        //                            }
                        //                        ]
                        //                    ); // MessageBox((vData.Error || 'An error
                        //    }; // if (vData.Result == 'ok') {

                        //}; // LoadData


                    } // Proceed()

                    if (IsNumeric(pThis.val())) {
                       if (CDec2(pThis.val()) < 0) {
                            history.back();
                            return;
                        } else {
                            if (CStr2(sessionStorage.IsReplacement) == 'true') {
                                if (CDec2(pThis.val()) == 0) {
                                    MessageBox('Order quantity must be greater than 0.')
                                        .on('pagehide', function () {
                                            pThis.focus();
                                        });
                                    return false;
                                } else {
                                    Proceed();
                                }
                            } else {
                                Proceed();
                            }
                        }
                    }; // if (!IsNumeric(pThis.val())) {
                }); // $('[name=txtQty]').each(function(vItem, vIndex) {

                AjaxSend({
                    url: '/PickProducts/saveProductBinLocation',
                    data: { BinID: sessionStorage.BinID,
                        ProductID: proj.ProductID,
                        FowstID: sessionStorage.FowstID,
                        InvoiceID: sessionStorage.CartOrderID,
                        SectionID: proj.SectionID,
                        Stocks: pData.Stocks
                    },
                    success: function (vData) {
                        if (vData.Error) {
                            NewMessageBox(vData.Error);
                            return;
                        }
                        proj.RefreshCart(); // inside this function is a history.back();
                    } // success
                }); // AjaxSend
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





        }); // $(function() {
        //        }); // require offline 
    });