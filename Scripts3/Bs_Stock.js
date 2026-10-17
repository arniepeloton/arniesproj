require(['common', 'business',
    //GetPathFormat('/Scripts3/Browser/LibraryOrganizationEmployeeFOWST.js?crc='),
    GetPathFormat('/Scripts/sources/InventoryCount/InventoryCount.js?crc='),
    'promise!/Scripts3/controller/PriceChange.js?crc=', 'promise!/Scripts3/model/BomMaker.js?crc=',
    '/Scripts3/Offline/Browser/LibraryOrganizationEmployeeFOWST.js?crc='
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


                var mAllowToSellBelowCost = false;
                var mAllowFreeTypeDiscountedPrice = false;
                var mAllowFreeTypeADiscountPercentage = false;
                var mAllowFreeTypeADiscountAmount = false;
                var mIsEnableVATExemptOption = false;
                var isPriceLevelChanged = false; //additional by janine

                var bomProductUnitMeasure;
                var bomProductQty;
                var mSubTotal;
                var mHasFreeTypeAPrice = false;
                window.StockInfo = {
                    PageShow: function () {
                        $('#imgStockInfo').attr('src', '../Content/images/box.png');
                        $('#spanStockNumber, #spanStockDescription').text('');
                        $('#divStockInfoLoading').pageshow();
                        setTimeout(function () {
                            PageShow();
                        }, 200)
                    }
                }

                $("[data-role=header]").fixedtoolbar({ tapToggle: false });
                $('[name="divStockMinimumCapacity"]').hide();
                $('[name="divStockMaximumCapacity"]').hide();

                setTimeout(function () {
                    //            $.mobile.activePage.bind('pageshow', PageShow);
                }, 1000);
                $.mobile.activePage.bind('pageshow', PageShow);
                mPriceChange.onInvalid = mPriceChange_onInvalid;
                mPriceChange.onSubmit = mPriceChange_onSubmit;

                $('#btnStockInfoBack').click(function () {
                    $('#pageMain').pageshow();
                })

                if (OfflineMode) {
                    mAllowFreeTypeAPrice = Stock_offline().GetAllowFreeTypeAPrice();
                    //for selling below cost ex. buy 1 take 1 zero price
                    mAllowToSellBelowCost = Stock_offline().GetAllowToSellBelowCost();
                }

                function PageShow() {
                    if (ValidPrevPage(arguments) || window.NewStockBrowser) {
                        $('#divUnit').hide();
                        ////////////////////                                                if (proj.PosMode)
                        $('#divUnit').find('[data-role=collapsible_clone]').remove();
                        ////////////////////                                                else
                        ////////////////////                                                    $('#divUnit').find('[data-role=collapsible_clone_pos]').remove();

                        //TODO
                        if (OfflineMode) {
                            if (CBool(Business.SystemDefaults.IsEnableVATExemptOption()))
                                $('#divVATExempt').show();
                            else
                                $('#divVATExempt').hide();

                            if (CBool(Business.SystemDefaults.ShowCondensedProductInformationInItemDetailsPage())) {
                                $('#divItemDetailsStockNumber').hide();
                                $('#divItemDetailsStockNumberCell').css('width', '100%');
                                $('#divItemDetailsImage').show();
                            } else {
                                $('#divItemDetailsImage').hide();
                            }
                        }

                        window.NewStockBrowser = false;

                        if (CBool(sessionStorage.IsInventoryCountMode)) {
                            ShowStockInfo(false);
                            ShowInventoryCount(true);
                            ClearInventoryCountDetails();
                        }
                        else {
                            ShowStockInfo(true);
                            ShowInventoryCount(false);
                        }

                        ShowSalesRepresentativeList(false);
                        ShowProductSerialList(false);
                        ShowInventoryBalances(false);

                        ClearPOSSalesRepSession();
                        LoadPOSSalesRepList();

                        ClearPOSProductSerialSession();
                        LoadPOSProductSerialList();

                        // if (window.location.pathname.toLowerCase() == '/billofmaterials' && localStorage.NewCartMode != 1)
                        if (proj.BomMakerMode)
                            GetInfoBOM();
                        else
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

                    //additional by janine june 24 2015 6pm
                    //if Purchase Order, Check first if a supplier has been selected
                    if (proj.POMode) {
                      
                        if (sessionStorage.SupplierOrganizationID == '') {
                            NewMessageBox("Please select a vendor / supplier ",
                                            'Purchase Order',
                                            [
                                                { Text: 'Ok',
                                                    Click: function () {
                                                        $.mobile.changePage('');
                                                    }
                                                }

                                            ]
                                        );
                            return;
                        }

                        var pDivs = $('#btnSalesPerson').closest('.ui-grid-d').children();

                        $('#btnSalesPerson').closest('div.ui-block-b').remove();
                        $('#btnSerialNumber').closest('div.ui-block-c').remove();
                        pDivs.css({width: '33.33%'});

                    }
                    
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
                        ModifierGroupID: (proj.ModifierGroupID || proj.GroupIdOfItemModifiers),
                        CustomerID: sessionStorage.CustomerOrganizationID,
                        OrderID: sessionStorage.CartOrderID,
                        NewCartMode: localStorage.NewCartMode,
                        PosMode: proj.PosMode ? "pos" : "so" //additional by janine sept 5 2014 for sales order taker
                    };
                    if (proj.POMode)
                        pData.SupplierOrgID = sessionStorage.SupplierOrganizationID;

                    if (OfflineMode) {
                        var pInfoPos;
                        try {
                            pInfoPos = Stock_offline().GetInfoPos(pData);
                        } catch (e) {
                            Android.Toast(e.stack);
                        }

                        LoadData(pInfoPos);
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
                            NewMessageBox(vData.Error, 'Error');
                            return;
                        } else {
                            mHasError = 0;
                        };

                        ClearPOSSalesRepSession();
                        LoadPOSSalesRepList();

                        ClearPOSProductSerialSession();
                        LoadPOSProductSerialList();

                        mData = vData;

                        (function ModifyPricesDueToRebates() {
                            var pDR;

                            if (proj.RebateData == null)
                                return;

                            for (var p = 0; p <= GetStringObject('data.Rows.length', mData) - 1; p++) {
                                mData.data.Rows[p].DiscountedPrice -= CDec2(proj.RebateData.Amount);
                            }

                            for (var p = 0; p <= GetStringObject('UnitMeasures.Rows.length', mData) - 1; p++) {
                                pDR = mData.UnitMeasures.Rows[p];
                                pDR.DiscountedPrice -= CDec2(pDR.Quantity) * CDec2(proj.RebateData.Amount);
                            }
                        })(); // END (function ModifyPricesDueToRebates() {

                        //additional by janine feb 21 2015 12nn - tick the vat exempt selection
                        if (OfflineMode) {
                            if (vData.VATInfo.Rows.length > 0) {

                                if (CDec2(vData.VATInfo.Rows[0].VATExemptSale) > 0 && CDec2(vData.VATInfo.Rows[0].VATableSale) == 0)
                                    $('#selVATExempt').val('1')//.slider('refresh');
                                else
                                    $('#selVATExempt').val('0')//.slider('refresh');
                            }
                        }
                        //


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
                            // NewMessageBox(vData.Warning); // no message as of June 29
                            return;
                        };

                        if (!CBool(sessionStorage.IsInventoryCountMode)) {
                            $('#divSubTotal').show();
                        }

                        $('#divReportNoPrice').hide();

                        //if (!IsInventoryCountMode) {
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
                            var pDefaultPriceID;
                            var pHasRecord = false;

                            if (OfflineMode) {
                                var pDrFranchiseeData = Business.LibraryOrganizationFOWST.GetFranchiseeData(sessionStorage.FowstID);
                                pDefaultPriceID = pDrFranchiseeData.DefaultSellingPriceLevel;
                                if (IsGuid(pDefaultPriceID))
                                    pDefaultPriceID = ReadyGuid(pDefaultPriceID);
                                else {
                                    pDefaultPriceID = Business.SystemDefaults.GetRetailPriceID();
                                    //if (window.DebugMode)
                                    //    pDefaultPriceID = ReadyGuid(Business.SystemDefaults.GetWholesalePriceID()); // for testing only
                                }
                            }

                            pLastItem = pTemplate;
                            if (proj.PLMode)
                                pTemplate.find('[name=divPriceLevel]').hide();

                            // find if  a record exist
                            for (var p = 0; p <= vRows.length - 1; p++) {
                                pDR = vRows[p];
                                if (IsNumeric(pDR.OrderedQty)) {
                                    pHasRecord = true;
                                    break;
                                }
                            }

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
                                else if ((p == 0) 
                                    && !pHasRecord)
                                    pOrderedQty = 1;

                                pTxtQty = pItem.find('[name=txtQtyTemplate]')
                                    .attr('name', 'txtQty')
                                    .data('UnitQty', pDR.Quantity)
                                    .data('Price', pDR.DiscountedPrice)
                                    .data('OriginalPrice', pDR.OriginalPrice)
                                    .data('UnmodifiedDiscountedPrice', pDR.UnmodifiedDiscountedPrice || pDR.OriginalPrice)
                                    .data('SpanRequestedPrice', pItem.find('[name=spanRequestedUnitPrice]'))
                                    .data('spanPrice', pItem.find('[name=spanPrice]'))
                                    .data('UnitMeasure', pDR.UnitMeasure)
                                    .data('UnitMeasureID', pDR.UnitMeasureID)
                                    .val(bomProductUnitMeasure == pDR.UnitMeasure ? bomProductQty : pOrderedQty)
                                    .bind('textchange', TxtQtyChanged)
                                    .focus(function() {
                                        $(this).select();
                                    });
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
                                pItem.find('[name=spanUnitMeasureID]').text(pDR.UnitMeasureID);
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
                                var pSelect = $('<select data-role="none" class="form-select">');
                                pDivPriceLevel.append(pSelect);
                                var pCbItem;
                                var pRows = vData.data.Rows;

                                pSelect.html('');
                                for (var p1 = 0; p1 <= pRows.length - 1; p1++) {
                                    if (pRows[p1].Quantity == pDR.Quantity) {
                                        //                                            pCbItem = new Option(pRows[p1].PriceLevel + ' ' + $.format.number(CDec2(pRows[p1].DiscountedPrice), '#,##0.00'), pRows[p1].PriceLevelID);
                                        pCbItem = new Option(pRows[p1].PriceLevel, ReadyGuid(pRows[p1].PriceLevelID));
                                        pSelect.append(pCbItem);
                                    };
                                };

                                //                                if (CBool(mAllowFreeTypeAPrice)) {
                                //pCbItem = new Option('Free Type a Price', 'FreeTypeAPrice');
                                //pSelect.append(pCbItem);
                                $(pCbItem).data('div', pItem.find('[name=divRequestedUnitPrice]'));
                                //                                }

                                pSelect.change(function () {
                                    var pThis = $(this);
                                    var pItem = pThis.closest('[name=divUnit]');
                                    var pDR;
                                    var pTxtQty;

                                    pTxtQty = pItem.find('[name=txtQty]');
                                    mCurrentyTxtQty = pTxtQty;
                                    if (pThis.val() == 'FreeTypeAPrice') {
                                        // Free Type a Price
                                        mDivRequestedUnitPrice = $(pThis.find(':selected')).data('div');
                                        ShowFreeTypePriceDialog();
                                        return;
                                    }

                                    for (var p = 0; p <= mData.data.Rows.length - 1; p++) {
                                        pDR = mData.data.Rows[p];
                                        if ((pDR.Quantity == pTxtQty.data('UnitQty'))
                                            && (ReadyGuid(pDR.PriceLevelID) == ReadyGuid(pThis.val()))) {

                                            pTxtQty.data('UnitQty', pDR.Quantity)
                                                .data('Price', pDR.DiscountedPrice)
                                                .data('UnitMeasure', pDR.UnitMeasure)
                                                .data('UnmodifiedDiscountedPrice', pDR.DiscountedPrice);

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

                                pSelect.attr('name', 'cbPrice');
                                //pSelect.selectmenu();
                                //pSelect.selectmenu('refresh', true);
                                if ((pDefaultPriceID == null)
                                    // || IsNumeric(pDR.Quantity) // commented 2018_05_18__15_03_26_0392
                                    || IsNumeric(pDR.OrderedQty))

                                    pSelect.find('option[value=' + pDR.PriceLevelID + ']').prop('selected', true);
                                else
                                    pSelect.find('option[value=' + pDefaultPriceID + ']').prop('selected', true);
                                //pSelect.selectmenu('refresh');
                                //pSelect.trigger('change');

                                pTxtQty.data('cbPrice', pSelect);
                                //pDivPriceLevel.trigger('create');

                                //additional by janine oct 27. para ma refresh
                                if (CDec2(localStorage.NewCartMode) == 0)
                                    pSelect.trigger('change');

                                // Sales Reps
                                if (mData.SalesReps != undefined) {
                                    LoadPOSSalesRep(mData.SalesReps.Rows);
                                }

                                // Product Serials
                                if (mData.ProductSerial != undefined) {
                                    LoadPOSProductSerial(mData.ProductSerial.Rows);
                                }

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
                                OnTxtQtyClick.call(vTxtQty);

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
                                OnTxtQtyClick.call(vTxtQty);
                                
                            });

                                //additional by janine 

                                pTxtUnit = pItem.find('[name=spanUnit]:first');
                                pTxtPrice = pItem.find('[name=spanPrice]:first');
                                pTxtUnitQty = pItem.find('[name=spanUnitQty]:first');
                                pTxtQty
                            .data('txtUnit', pTxtUnit)
                            .data('txtPrice', pTxtPrice)
                            .data('txtUnitQty', pTxtUnitQty)
                            .on('input', OnTxtQtyClick);

                            function OnTxtQtyClick() {  
                                var $this = $(this);
                                var vTxtUnit = $this.data('txtUnit');
                                sessionStorage.ProductUnitMeasure = vTxtUnit.text();

                                var vTxtPrice = $this.data('txtPrice');
                                prodPrice = vTxtPrice.text();
                                sessionStorage.ProductPrice = prodPrice.replace(/,/g, '');

                                var vTxtUnitQty = $this.data('txtUnitQty');
                                sessionStorage.ProductUnitQty = vTxtUnitQty.text();

                                sessionStorage.ProductPriceLevelName = pItem.find('[name=cbPrice]:first option:selected').text();

                                if (window.OfflineMode) {
                                    var pOriginalPrice = $this.data('Price') || $this.data('UnmodifiedDiscountedPrice');
                                    var pNewPrice = pOriginalPrice;
                                    var pPriceLevelID = $($this.data().cbPrice).val();
                                    Business.LibraryPIMProduct.GetPrice(proj.ProductID, '', pPriceLevelID, Web.OrgID());
                                    var pDtPriceBreaks = Business.LibraryPIMProduct.GetPriceBreak({
                                        ProductID: proj.ProductID,
                                        UnitMeasureID: $this.data('UnitMeasureID'),
                                        Qty: $this.val()
                                    });
                                    if (pDtPriceBreaks.Rows.length > 0) 
                                        pNewPrice = pOriginalPrice * (1 - pDtPriceBreaks.Rows[0].Discount / 100);
                                    pNewPrice = Business.LibraryPIMProduct.RoundPrice(pNewPrice);
                                    pNewPrice = +parseFloat(pNewPrice).toFixed(2);
                                    $this.data('spanPrice').text(AccountingFormat(pNewPrice));
                                    $this.data('Price', pNewPrice);
                                }

                                DisplaySubtotal();
                            }

                            OnTxtQtyClick.call(pTxtQty);

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

                                OnTxtQtyClick.call($(this).data('txtQty'));
                                
                            });

                                pItem.show();
                            }; // for (var p = 0; p <= vRows.length - 1; p++) {

                            DisplaySubtotal();
                            
                        })(vData.UnitMeasures.Rows); // LoadUnitMeasures
                        //}
                        //else {
                        if (CBool(sessionStorage.IsInventoryCountMode)) {
                            $('[name=divUnit]').hide();
                            if (CBool(sessionStorage.IsInventoryCountEditSKUMode)) {
                                GetLastCounts(pStockID);
                            }
                            GetInventoryCountStockUnitMeasures(pStockID);
                            setTimeout(function () {
                                GetNamedFowstBin();

                                if (sessionStorage.FowstBinID != undefined) {
                                    $('#selBinsStockViewer').val(sessionStorage.FowstBinID);

                                    //try {
                                    //    $('#selBinsStockViewer').selectmenu('refresh');
                                    //} catch (e) {
                                    //    $('#selBinsStockViewer').selectmenu();
                                    //}
                                }
                            }, 500);
                        }

                        $('[name="divStockMinimumCapacity"]').hide();
                        $('[name="divStockMaximumCapacity"]').hide();

                        if (vData.MinPerProduct != undefined) {
                            $('[name="divStockMinimumCapacity"]').show();
                            $('[name="spanStockMinimumCapacity"]').text(AccountingFormat(vData.MinPerProduct, 0));
                        }
                        if (vData.MaxPerProduct != undefined) {
                            $('[name="divStockMaximumCapacity"]').show();
                            $('[name="spanStockMaximumCapcity"]').text(AccountingFormat(vData.MaxPerProduct, 0));
                        }
                        mPriceChange.setData(mData);


                        $('#divStockInfo').pageshow();
                    }; // LoadData

                    if (CBool(Business.SystemDefaults.ShowCondensedProductInformationInItemDetailsPage())) {
                        AjaxSend({
                            url: 'Organization/GetBulkPIMPictures',
                            data: { data: [{ ID: pStockID }], size: 'medium', IsDefault: true },
                            success: function (e) {
                                if (e == null) {
                                    console.warn('e is null');
                                    return; // silent error lang
                                }

                                if (e.Error) {
                                    console.warn(e.Error);
                                    return; // silent error lang
                                }

                                var vRows = e.data;
                                var sourceimg = Android.GetServer();

                                for (var i = 0; i < vRows.length; i++) {
                                    if (vRows[i].Pics != '') {
                                        vRows[i].Ctr = 1;
                                        var pSrc = sourceimg + 'Content/MobizImagePIM/' + sessionStorage.OrgID + '/Medium/' + vRows[i].Pics[0].FileName;

                                        $('#imgStockInfo').attr('src', pSrc)
                                            .attr('onerror', 'skupicerror(this)')
                                            .attr('isimgerror', 0)
                                            .attr('isimgviewer', 0)
                                            .data('data', vRows[i])
                                            .click(function (e) {
                                                e.preventDefault();
                                                e.stopPropagation();
                                                var data = $(this).data('data');

                                                $(this).parents().closest('li').removeClass('ui-btn-active');

                                                if (CInt2($(this).attr('isimgerror')) == 1) {
                                                    NewMessageBox('Can\'t load images for SKU: <i>' + data.ProductName + '</i> or the server not responding.');
                                                    return;
                                                }

                                                AjaxSend({
                                                    url: 'Organization/GetBulkPIMPictures',
                                                    data: { data: [data], size: 'big' },
                                                    success: function (e) {
                                                        vRows = e.data[0].Pics;

                                                        var div = $('#divItemDetailsStockNumberCell');

                                                        if ($('#skupics').length == 0) {
                                                            div.append($('<div id="skupics" style="display:none;">'));
                                                        } else {
                                                            $('#skupics').empty();
                                                        }

                                                        div = $('#skupics');

                                                        for (var i = 0; i < vRows.length; i++) {
                                                            vRows[i].Ctr = i + 1;
                                                            vRows[i].ID = data.ID;
                                                            vRows[i].ProductName = data.ProductName;
                                                            var srcBig = sourceimg + 'Content/MobizImagePIM/' + sessionStorage.OrgID + '/Big/' + vRows[i].FileName;

                                                            div.append($('<a data-role="none" data-lightbox="skupictures">')
                                                                        .attr('id', 'pic_' + vRows[i].ID)
                                                                        .data('data', vRows[i])
                                                                        .attr('href', srcBig)
                                                                        .append($('<img>')
                                                                            .attr('isimgerror', 0)
                                                                            .attr('isimgviewer', 1)
                                                                            .attr('onerror', 'skupicerror(this)')
                                                                            .data('data', vRows[i]).attr('src', srcBig)
                                                                        )
                                                                    )
                                                        }

                                                        var firstpic = $('#pic_' + vRows[0].ID);
                                                        firstpic.on('click', function () {
                                                            var dis = $(this);
                                                            setTimeout(function () {
                                                                if (CInt2(dis.find('> img').attr('isimgerror')) == 1) {
                                                                    NewMessageBox('Can\'t load images for SKU: <i>' + firstpic.data('data').ProductName + '</i> or the server not responding.');
                                                                }
                                                            }, 100)
                                                        })
                                                        firstpic.trigger('click');
                                                    }
                                                })
                                            })
                                    } else {
                                        $('#imgStockInfo').attr('src', '../Content/images/box.png');
                                    }
                                }
                            },
                            error: function (e) {
                                Android.AddLog('Can\'t connect to the server, SKU image not successfully loaded');
                                $('#imgStockInfo').attr('src', '../Content/images/box.png');
                            }
                        })
                    }                    
                }; // GetInfo



                function GetInfoBOM() {

                    var pParams;
                    var pStockID;

                    if (proj.ProductID != undefined)
                        pStockID = proj.ProductID;
                    else {
                        pParams = GetUrlParameters();
                        pStockID = pParams.id;
                    };

                    if (OfflineMode) {
                        var p = new App.model.BomMaker();
                        p.loadProducts({
                            id: sessionStorage.CartOrderID,
                            parentProductID: pStockID,
                            success: function (vData) {
                                if (vData.Rows.length > 0) {
                                    bomProductUnitMeasure = vData.Rows[0].ParentUnitMeasure;
                                    bomProductQty = vData.Rows[0].ParentQuantity;


                                }

                                GetInfo();
                            },
                            error: function (vMessage) {
                                NewMessageBox(vMessage);
                            }
                        });


                    } else {
                        NewMessageBox('Not yet implemented');
                    }
                }; // GetInfoBOM



                //additional by janine for hot buttons apr 10,2014
                var prodQty = '';
                var prodPrice = 0;
                //end of additional

                //                $('#btnUpdateCartTallies').click(function (e) {
                //                    $('[name=butUnitUpdateCart]').trigger('click');
                //                });

                $('[name=butUnitUpdateCart]').click(function (e) {

                    e.preventDefault();
                    Bs.Loading('show');
                    setTimeout(function () {
                        if (Verify())
                            Submit();
                    }, 300)                    

                    function Verify() {

                        var pResult = true;

                        if (mHasError) {
                            Bs.Loading('hide');
                            NewMessageBox('There are problems with this product');
                            return false;
                        };

                        if ((!proj.PosMode) && (sessionStorage.CustomerOrganizationID == undefined)) {
                            Bs.Loading('hide');
                            NewMessageBox('No selected customer');
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
                                Bs.Loading('hide');
                                NewMessageBox('Quantity must be a valid whole number.', 'Invalid quantity')
                            .on('pagehide', function () {
                                pThis.focus();
                            });
                                pResult = false;
                                return false;
                            }; // if (!IsNumeric(pThis.val())) {
                            if (CDec2(pVal) < 0) {
                                Bs.Loading('hide');
                                NewMessageBox('Quantity must not be a negative number. You entered ' + pVal, 'Invalid quantity')
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
                                    Bs.Loading('hide');
                                    NewMessageBox('Adjust Price To must be a valid number. You entered ' + pVal, 'Invalid Adjust Price To')
                        .on('pagehide', function () {
                            pThis.focus();
                        });
                                    pResult = false;
                                    return false;
                                }; // if (!IsNumeric(pThis.val())) {
                            }); // $('[name=txtNewPrice]').each(function(vItem, vIndex) {
                        }; // if (pResult) {

                        if (pResult) {
                            if (mSubTotal > proj.MaxNumber) {
                                pResult = false;
                                Bs.Loading('hide');
                                NewMessageBox('The subtotal is too large for the system to handle. Please enter a lower quantity.');
                            }
                        }

                        return pResult;

                    }; // function Verify() {

                }); // $('#butUnitUpdateCart').click(function(e) {

                function Submit(vVerifyOnly) {
                    // PARAMS:
                    //  vVerifyOnly: boolean optional, verify price request
                    console.time('Bs_Stock.Submit jsYMGQ8XLQAP!');
                    Bs.Loading('show');

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

                    pData.PosMode = proj.PosMode ? "pos" : "so"; //additional by janine sept 5 2014 for sales order taker

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

                    //additional customer demographic oct 23
                    pData.DeliveryMethod = sessionStorage.DeliveryMethod;
                    pData.OrderMethod = sessionStorage.OrderMethod;
                    pData.Gender = sessionStorage.Gender;
                    pData.Age = sessionStorage.Age;
                    pData.DeliveryAddressID = sessionStorage.DeliveryAddressID;
                    pData.BillingAddressID = sessionStorage.BillingAddressID;
                    pData.DeliveryDate = sessionStorage.DeliveryDate;
                    //additonal for sm coin VAT EXEMPT
                    pData.IsVATExempt = $('#selVATExempt').val();
                    pData.SupplierID = sessionStorage.SupplierOrganizationID;
                    pData.IsPurchaseOrder = CBool(proj.POMode);
                    if (CStr2(sessionStorage.CustomerTableNo) == '') {
                        sessionStorage.CustomerTableNo = CStr2(GetUrlParameters().TableNo);
                    }
                    if (CStr2(sessionStorage.CustomerTableNo) != '' || CStr2(sessionStorage.GasPumpNo) != '') {
                        if (CStr2(sessionStorage.GasPumpNo) != '')
                            pData.CustomerTableNo = 'GP_' + sessionStorage.GasPumpNo;

                        if (CStr2(sessionStorage.CustomerTableNo) != '')
                            pData.CustomerTableNo = 'TBL_' + sessionStorage.CustomerTableNo;
                    }
                    //end of additional

                    $('[name=txtQty]').each(function (vItem, vIndex) {
                        var pThis = $(this);
                        var pTxtAdjustPrice;
                        var pItem;
                        var pRequestedPrice;
                        var pPriceRequestData;

                        //francis h sept 22
                        if (CBool(sessionStorage.IsInventoryCountMode)) {
                            var pSql = '';
                            var pDT;
                            var Total = 0;

                            pSql = 'Select SUM(SuggestedOrder) Quantity from TransInventoryProductCountStock ' +
                                    'inner join TransInventoryProductCount on TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID ' +
                                    'where TransInventoryProductCount.SuggestedOrderDate IS NULL ' +
                                    'AND TransInventoryProductCount.FowstID = ' + ReadyString(sessionStorage.CustomerOrganizationID) +
                                    'AND TransInventoryProductCount.ProductID = ' + ReadyString(proj.ProductID)

                            pDT = Sql.Execute(pSql);

                            if (pDT.Rows[0].Quantity != undefined) {
                                Total = pDT.Rows[0].Quantity;
                            }

                            var SuggestedOrder = 0;
                            var ctr = 0;
                            $('#lstCounts li a').each(function (m_index) {
                                if ($(this).data('units') != undefined) {
                                    //note: $(this).data('units').ID is a row that already existing in the database, since we already have a query above for the suggested order, we dont include this row for computing suggested order 
                                    if ($(this).data('units').ID == undefined) {
                                        //we will only get suggested order from newly created rows in order not to double count with the current row in the database
                                        while (ctr < $(this).data('units').length) {
                                            SuggestedOrder += parseFloat($(this).data('units')[ctr].MaxCap) - parseFloat($(this).data('units')[ctr].Quantity)
                                            ctr++;
                                        }
                                        ctr = 0;
                                    }
                                } //if ($(this).data('units') != undefined) {

                            });

                            pItem = pThis.closest('[name=divUnit]');
                            pData.Stocks[0] = {
                                Qty: parseFloat(SuggestedOrder) + parseFloat(Total),
                                UnitQty: pThis.data('UnitQty'),
                                UnitPrice: pThis.data('Price'),
                                PriceLevelID: pItem.find('[name=cbPrice]').val()                                
                            };

                            $('#btnSaveTallies').click();

                            return false;
                        }
                        else {

                            if (IsNumeric(pThis.val())) {
                                pItem = pThis.closest('[name=divUnit]');
                                var im = JSON.parse(sessionStorage.ItemModifier || "[]");
                                var totalModifier = 0;
                                if (im != undefined) {
                                    for (var i = 0; i < im.length; i++) {
                                        if (im[i].SelectedModifier == undefined) {
                                            continue
                                        }
                                        for (var m = 0; m < im[i].SelectedModifier.length; m++) {
                                            if (im[i].Type == 'Inventory Affecting') {
                                                im[i].SelectedModifier[m].OrderQty = CDec2(pThis.val()) * CDec2(im[i].SelectedModifier[m].InputQty) //multiply main SKU qty to selected modifier qty
                                                totalModifier += CDec2(im[i].SelectedModifier[m].OrderQty) * CDec2(im[i].SelectedModifier[m].UnitPrice);
                                            }
                                        }
                                    }
                                }
                                pData.Stocks[p] = {
                                    Qty: pThis.val(),
                                    UnitQty: pThis.data('UnitQty'),
                                    UnitPrice: pThis.data('Price'),
                                    UnitMeasure: pThis.data('UnitMeasure'),
                                    PriceLevelID: pItem.find('[name=cbPrice]').val(),
                                    ItemModifier: im,
                                    GroupIdOfItemModifiers: proj.GroupIdOfItemModifiers,
                                    TotalModifier: totalModifier
                                };

                                pPriceRequestData = pThis.data('requestedPriceData') || {};
                                if (pPriceRequestData.netAmount != null) {
                                    pData.Stocks[p].RequestedUnitPrice = Math.max(CDec2(pPriceRequestData.netAmount), 0);
                                    pData.Stocks[p].PriceLevelID = pPriceRequestData.priceLevelID;
                                }

                                sessionStorage.ProductUnitMeasureID = pItem.find('[name=spanUnitMeasureID]:first').text();

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
                        if (window.location.pathname.toLowerCase() == '/billofmaterials') {

                            var p = new App.model.BomMaker();
                            p.save({
                                id: sessionStorage.CartOrderID,
                                products: [
                                        {
                                            productID: proj.ProductID,
                                            qty: parseFloat($('[name=txtQty]:first').val()),
                                            unitMeasureID: sessionStorage.ProductUnitMeasureID
                                        }
                                    ],
                                success: function (vData) {
                                    LoadData(vData);
                                },
                                error: function (vMessage) {
                                    Bs.Loading('hide');
                                    NewMessageBox(vMessage);
                                }
                            });

                        } else {
                            console.timeEnd('Bs_Stock.Submit jsYMGQ8XLQAP!');
                            console.time('Bs_Stock.Stock_offline().UpdatePos jsJTBUSAW2KS!');
                            pData.UpdateErrorCorrectAmount = true;
                            var pResult = Stock_offline().UpdatePos(pData);
                            console.timeEnd('Bs_Stock.Stock_offline().UpdatePos jsJTBUSAW2KS!');

                            LoadData(pResult);
                            HandleTestFreeTypeAPrice();
                        }
                    } else {
                        if (window.location.pathname.toLowerCase() == '/billofmaterials') {

                            Bs.Loading('hide');
                            NewMessageBox('Not yet implemented');

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
                        }
                    }; // if (OfflineMode) {

                    function LoadData(vData) {

                        var pMessage;
                        var pDivDetails;
                        var pLnkDetails;

                        console.time('Bs_Stock.Stock_offline().LoadData jsHF8UAWQ7HZ!');

                        if (vData == undefined) return;
                        if (vData.Error != undefined) {
                            if (CDec2(vData.ReloadBrowser) != 0)
                                NewMessageBox(vData.Error, 'Error', null, {
                                    OnClose: function () {
                                        location.reload();
                                    }
                                });
                            else
                                NewMessageBox(vData.Error);
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
                            Bs.Loading('hide');
                            if (vData.Result == 'ok') {
                                NewMessageBox('Price adjustment request is acceptable.');
                            } else {
                                NewMessageBox(pMessage, 'Price Request Denied', null, {
                                    EncodeHtml: false
                                });
                            };
                        } else {
                            if ((vData.Result == 'ok')
                                || (vData.Warning != undefined)) {

                                function Done() {

                                    if (CStr2(vData.OrderID) != '')
                                        localStorage.CartOrderID = vData.OrderID; // use for new retail view since onpage ang pagload sa payment page, ginadelete ang SESSIONSTORAGE sa payments page pag mag clear sa proj.SalesPayments.ClearSalesPaymentsInfoFields by arnie
                                        sessionStorage.CartOrderID = vData.OrderID;

                                    localStorage.removeItem('NewCartMode');

                                    //TODO
                                    if (window.location.pathname.toLowerCase() == '/billofmaterials') {
                                        sessionStorage.CartOrderID
                                        proj.ShowCart();
                                    }
                                    else {
                                        if ($.isFunction(proj.ShowCart)) {

                                            //additional by janine. remember pos mode apr 10, 2014

                                            if (sessionStorage.PosMode == 'Hot Button Mode') {


                                                //startclock('#clockHotButtons');
                                                $('#pageHotButton').pageshow();
                                                //ChangePage('/SalesMenu/HotButtons');

                                                $('#divSpace').hide();
                                                if (prodQty != '') {
                                                    sessionStorage.LastQty = prodQty;
                                                    if (parseFloat(prodQty) > 0) {
                                                        if (!CBool(sessionStorage.IsInventoryCountMode)) {
                                                            $('#txtOrder').html($('#spanStockDescription').text() + '<br/><br/>' + sessionStorage.ProductPriceLevelName + ': ' + AccountingFormat(parseFloat(sessionStorage.ProductPrice)) + '<br/>' + 'Item total: ' + AccountingFormat(parseFloat(sessionStorage.ProductPrice) * parseFloat(prodQty)));
                                                            $('#txtQtyUnit').html('<label style="font-size: 68px;">' + prodQty + '</label><br/>' + sessionStorage.ProductUnitMeasure);
                                                            if ($.isFunction(proj.HotButtonUpdateCallback))
                                                                proj.HotButtonUpdateCallback({ Quantity: prodQty });
                                                        }
                                                    } else {
                                                        proj.ProductID = '';
                                                        $('#txtOrder').html('');
                                                        $('#txtQtyUnit').html('');
                                                    }
                                                }
                                                proj.ShowCart();
                                            } else { // ELSE if (sessionStorage.PosMode == 'Hot Button Mode') {
                                                proj.ShowCart();
                                            } // END if (sessionStorage.PosMode == 'Hot Button Mode') {

                                        } else {
                                            NewMessageBox('Cart was successfully updated', 'Success');
                                            window.location = GetWindowPathFormat('salesmenu') + '#cart';
                                        };
                                        Bs.Loading('hide');
                                    }
                                }; // function Done() {

                                if (vData.Warning != undefined)
                                    NewMessageBox(pMessage, 'Warning', null, {
                                        OnClose: Done,
                                        EncodeHtml: false
                                    });
                                else
                                    Done();
                            } else {
                                NewMessageBox((vData.Error || 'An error occured while updating cart'), 'Failed',
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
                        ); // NewMessageBox((vData.Error || 'An error
                            }; // if (vData.Result == 'ok') {
                        }; // if (vVerifyOnly) {

                        console.timeEnd('Bs_Stock.Stock_offline().LoadData jsHF8UAWQ7HZ!');
                    }; // LoadData
                }; // Submit()

                function DisplaySubtotal() {

                    mSubTotal = 0;

                    $('[name=txtQty]').each(function (vItem, vIndex) {
                        var pUnitPrice;
                        var pThis = $(this);
                        if (IsNumeric(pThis.val())) {
                            //////////////////                            if (!proj.PosMode)
                            //////////////////                                pUnitPrice = pThis.data('txtAdjustPrice').val();
                            if (CDec2(pUnitPrice) <= 0)
                                pUnitPrice = pThis.data('Price');
                            mSubTotal += pThis.val() * pUnitPrice;

                        }; // if (!IsNumeric(pThis.val())) {
                    }); // $('[name=txtQty]').each(function(vItem, vIndex) {

                    if (mSubTotal > proj.MaxNumber)
                        $('#spanSubtotal').text('Please be realistic');
                    else
                        $('#spanSubtotal').text(AccountingFormat(mSubTotal));

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

                function ShowInventoryCount(vShow) {
                    if (vShow) {
                        $('#divInventoryCountContainer').show();
                        $('#rcChoiceGoodYes').attr("checked", true)//.checkboxradio("refresh");
                        $('#rcChoiceGoodNo').attr("checked", false)//.checkboxradio("refresh");
                        $('#dvReasonForBadCount').css('visibility', 'hidden');
                        $('#dvReasonForBadCount').css('display', 'none');
                    } else {
                        $('#divInventoryCountContainer').hide();
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
                            arSalesRep.Index = i;

                            if (arSalesRep[intSalesRepState] != "Remove") {                                
                                pItem = $('<a data-role="none" class="list-group-item" id="lst' + i + '">');
                                pItem.append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">').text(arSalesRep[intSalesRepName]))
                                            .append($('<td style="width:50px;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                    .data('Data', arSalesRep)
                                                    .click(function (e) {
                                                        e.preventDefault();
                                                        var data = $(this).data('Data');
                                                        sessionStorage["POSSalesRepToRemoveIndex"] = data.Index;
                                                        sessionStorage["POSSalesRepToRemove"] = JSON.stringify(data);

                                                        NewMessageBox("Remove this record?",
                                                            'Remove',
                                                            [
                                                                {
                                                                    Text: 'Yes',
                                                                    Click: function () {

                                                                        var arSalesRepToRemove = new Array();
                                                                        arSalesRepToRemove = JSON.parse(sessionStorage["POSSalesRepToRemove"]);
                                                                        intSalesRepIndex = sessionStorage["POSSalesRepToRemoveIndex"];

                                                                        arSalesRepToRemove[intSalesRepState] = "Remove";
                                                                        sessionStorage["POSSalesRep-" + intSalesRepIndex] = JSON.stringify(arSalesRepToRemove);

                                                                        LoadPOSSalesRepList();

                                                                        sessionStorage.removeItem("POSSalesRepToRemove");
                                                                        sessionStorage.removeItem("POSSalesRepToRemoveIndex");
                                                                        //ChangePage('Stock');
                                                                    }
                                                                }
                                                                ,
                                                                {
                                                                    Text: 'No'
                                                                }
                                                            ]
                                                        );

                                                    })
                                                ))
                                        )
                                    )

                                pLV.append(pItem);

                            }

                        }
                    }

                    //pLV.listview('refresh');
                };

                function GetInventoryCountStockUnitMeasures(m_ID) {
                    var data = { ProductID: m_ID }
                    var vData;
                    if (OfflineMode) {
                        var pSql = '';
                        pSql = 'Select DISTINCT LibraryPIMMeasure.ID ID, LibraryCountriesLanguageLabel.Label UnitMeasure, LibraryPIMProductMeasurement.Quantity Quantity, LibraryPIMproductPriceLevel.Price Price ' +
                                    'FROM LibraryPIMProductMeasurement INNER JOIN ' +
                                    'LibraryPIMMeasure ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                                    'LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID  ' +
                                    'inner join LibraryPIMproductPriceLevel on LibraryPIMproductPriceLevel.ProductID = LibraryPIMProductMeasurement.ProductID  ' +
                                    'inner join LibraryPIMPriceLevel on LibraryPIMPriceLevel.ID = LibraryPIMproductPriceLevel.PriceLevelID ' +
                                    'WHERE LibraryPIMProductMeasurement.ProductID = ' + ReadyString(data.ProductID) + ' and LibraryPIMPriceLevel.PriceLevel=\'Retail Price\' AND LibraryPIMproductPriceLevel.OrganizationID = ' + ReadyString(sessionStorage.OrgID) +
                                    ' Order by Quantity'

                        vData = Sql.Execute(pSql);

                        DisplayUnitMeasures(vData);
                        GetFowstBinMaxMinCap();
                    }
                    else {
                        AjaxSend({
                            url: 'Receiving/GetStockUnitMeasures',
                            data: data,
                            success: function (vData) {
                                $.mobile.changePage('#pageStockViewer');
                                DisplayUnitMeasures(vData.UnitMeasure);
                            }
                        });
                    }
                }

                function GetLastCounts(m_ID) {
                    var pSql = '';
                    var vData;

                    pSql = 'select transinventoryproductcount.ID, transinventoryproductcount.ProductID, transinventoryproductcountstock.BinID FowstBinID, LibraryFowstBin.Name, Quantity, UnitMeasureID, SuggestedOrder, MaxPerProduct MaxCap, ' +
                        'transinventoryproductcountstock.IsGoodStock IsGood, transinventoryproductcountstock.ExpirationDate  from transinventoryproductcount ' +
                        'inner join transinventoryproductcountstock on transinventoryproductcountstock.inventoryproductcountid = transinventoryproductcount.id ' +
                        'inner join LibraryFowstBin on LibraryFowstBin.ID = transinventoryproductcountstock.BinID  ' +
                        'inner join LibraryFowstBinProduct on LibraryFowstBinProduct.ProductID = transinventoryproductcount.ProductID ' +
                        'AND LibraryFowstBinProduct.BinID = transinventoryproductcountstock.BinID ' +
                        'AND LibraryFowstBinProduct.fowstid= transinventoryproductcount.fowstid ' +
                        'Where transinventoryproductcount.ProductID = ' + ReadyString(m_ID) +
                        ' AND transinventoryproductcount.FOWSTID = ' + ReadyString(sessionStorage.CustomerOrganizationID) +
                        ' AND SuggestedOrderDate IS NULL';

                    vData = Sql.Execute(pSql);

                    if (vData.Rows.length > 0) {
                        DisplayLastCounts(vData.Rows);
                    }
                }

                function DisplayUnitMeasures(vRows) {
                    var pLV = $("#lstUnitMeasure");

                    var BaseUnitMeasure = vRows.Rows[0].UnitMeasure;

                    $("#lstUnitMeasure tbody tr").remove();

                    for (var i = 0; i <= vRows.Rows.length - 1; i++) {
                        //                unit measure               

                        pItem = $('<tr>')
                            .append($('<td>')
                                .append($('<label for=' + vRows.Rows[i].UnitMeasure + '>')
                                .text(vRows.Rows[i].UnitMeasure + ' (' + vRows.Rows[i].Quantity + ')')
                                    )
                            )//td
                            .append($('<td>')
                                .append($('<input type=number />')
                                .attr('ID', 'txt' + vRows.Rows[i].ID)
                                .attr('name', vRows.Rows[i].UnitMeasure)
                                .data('data', vRows.Rows[i])
                                .attr('class', 'ui-input-text ui-corner-all ui-btn-shadow ui-body-c')
                                .val(vRows.Rows[i].QuantityGood)
                                    )
                            )//td                            

                        pLV.append(pItem)

                    }

                    //try {
                    //    $('#lstUnitMeasureFrame').listview('refresh');
                    //}
                    //catch (e) {
                    //    $('#lstUnitMeasureFrame').listview();
                    //}
                }

                function GetFowstBinMaxMinCap() {

                    var data = { FowstID: sessionStorage.CustomerOrganizationID,
                        BinID: sessionStorage.FowstBinID,
                        ProductID: proj.ProductID
                    }

                    if (OfflineMode) {
                        var mData = '';
                        //mData = InventoryCount.FowstBinProductMaxMinCap(data);


                        var pFinder = new Finder();
                        var pFields = pFinder.Fields;
                        var pField;

                        pFinder.Distinct = 1;

                        pField = pFields.Add('MinPerProduct');
                        pField = pFields.Add('MaxPerProduct');

                        pFinder.SqlFrom = 'FROM LibraryFowstBinProduct binpro'

                        pFinder.SqlWhere = 'WHERE binpro.FowstID = ' + ReadyString(sessionStorage.CustomerOrganizationID) +
                        ' AND binpro.BinID = ' + ReadyString(data.BinID) +
                        ' AND binpro.ProductID = ' + ReadyString(data.ProductID)

                        mData = pFinder.Requery();

                        if (mData.Rows.length > 0) {
                            $('#txtBinMaxCap').val(mData.Rows[0].MaxPerProduct);
                            $('#txtBinMinCap').val(mData.Rows[0].MinPerProduct);
                        }
                        else {
                            $('#txtBinMaxCap').val(0);
                            $('#txtBinMinCap').val(0);
                        }
                    }
                    else {
                        AjaxSend({
                            url: 'Browser/LibraryFowstBinProduct',
                            data: data,
                            success: function (vData) {
                                if (vData.Results.Rows.length > 0) {
                                    $('#txtBinMaxCap').val(vData.Results.Rows[0].MaxPerProduct);
                                    $('#txtBinMinCap').val(vData.Results.Rows[0].MinPerProduct);
                                }
                                else {
                                    $('#txtBinMaxCap').val(0);
                                    $('#txtBinMinCap').val(0);
                                }
                            }
                        });
                    }
                }

                function DisplayLastCounts(vRows) {
                    var pLV = $('#lstCounts')
                    var SuggestedOrder = 0;

                    for (var i = 0; i < vRows.length; i++) {

                        SuggestedOrder += parseFloat(vRows[i].SuggestedOrder);

                        pItem = $('<li>')
                            .attr('ID', vRows[i].ID)
                            .append($('<a>')
                                .attr('ID', vRows[i].ID)
                                .data('units', vRows[i])
                                .append($('<h3>')
                                    .text('SKU Info')
                                )
                                .append($('<h4>')
                                    .text('Shelf/Bin Location: ' + vRows[i].Name)
                                )
                                .append($('<h4>')
                                    .text('Expiration Date: ' + vRows[i].ExpirationDate)
                                )
                                .append($('<h4>')
                                    .text('Max Cap (Qty)' + ': ' + vRows[i].MaxCap)
                                )
                                .append($('<h4>')
                                    .text('Good Count: ' + vRows[i].Quantity)
                                )
                                .append($('<h4>')
                                    .text('Max Cap - Good Count: ' + vRows[i].SuggestedOrder)
                                )
                            )
                            .append($('<a>')
                                .attr('ID', 'close' + vRows[i].ID)
                                .click(function () {

                                    if (IsGuid($(this).attr('ID').substr(5, $(this).attr('ID').length))) {
                                        RemoveFromDatabase($(this).attr('ID').substr(5, $(this).attr('ID').length), $('#' + $(this).attr('ID').substr(5, $(this).attr('ID').length) + ' a').data('units').SuggestedOrder);
                                    }
                                    $('#' + $(this).attr('ID').substr(5, $(this).attr('ID').length)).remove();
                                    ComputeSuggestedOrder(0);
                                })
                            );

                        pLV.append(pItem);
                    }

                    $('#txtSuggestedOrder').val(SuggestedOrder);

                    //try {
                    //    pLV.listview('refresh');
                    //} catch (e) {
                    //    pLV.listview();
                    //}
                }

                function RemoveFromDatabase(vID, SuggestedOrder) {
                    var pSql = '';

                    pSql = 'Delete from TransInventoryProductCountStock WHERE InventoryProductCountID = ' + ReadyString(vID);
                    Sql.ExecuteSql(pSql);

                    pSql = 'Update TransInventoryProductCount set SuggestedOrder = SuggestedOrder - ' + SuggestedOrder + ' WHERE ID = ' + ReadyString(vID);
                    Sql.ExecuteSql(pSql);
                }

                function ClearInventoryCountDetails() {
                    $('#txtBinMaxCap').val('');
                    $('#txtBinMinCap').val('');
                    $('#txtSuggestedOrder').val('');
                    $('#lstCounts').empty();
                }

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
                            arProductSerial.Index = i;

                            if (arProductSerial[intProductSerialState] != "Remove") {
                                pItem = $('<a data-role="none" class="list-group-item" id="lst' + i + '">');
                                pItem.append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">').text(arProductSerial[intProductSerial]))
                                            .append($('<td style="width:50px;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                    .data('Data', arProductSerial)
                                                    .click(function (e) {
                                                        e.preventDefault();
                                                        var data = $(this).data('Data');
                                                        sessionStorage["POSProductSerialToRemoveIndex"] = data.Index;
                                                        sessionStorage["POSProductSerialToRemove"] = JSON.stringify(data);

                                                        NewMessageBox("Remove this record?",
                                                            'Remove',
                                                            [
                                                                {
                                                                    Text: 'Yes',
                                                                    Click: function () {

                                                                        var arPOSProductSerialToRemove = new Array();
                                                                        arPOSProductSerialToRemove = JSON.parse(sessionStorage["POSProductSerialToRemove"]);
                                                                        intProductSerialIndex = sessionStorage["POSProductSerialToRemoveIndex"];

                                                                        arPOSProductSerialToRemove[intProductSerialState] = "Remove";
                                                                        sessionStorage["POSProductSerial-" + intProductSerialIndex] = JSON.stringify(arPOSProductSerialToRemove);

                                                                        LoadPOSProductSerialList();

                                                                        sessionStorage.removeItem("POSProductSerialToRemove");
                                                                        sessionStorage.removeItem("POSProductSerialToRemoveIndex");
                                                                        //ChangePage('Stock');

                                                                    }
                                                                }
                                                                ,
                                                                {
                                                                    Text: 'No'
                                                                }
                                                            ]
                                                        );

                                                    })
                                                ))
                                        )
                                    )

                                pLV.append(pItem);

                            }

                        }
                    }

                    //pLV.listview('refresh');
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
                        BrowsePopup('LibraryOrganizationEmployeeFOWST', 'Select Sales Representative', [
                            {
                                Offline: true,
                                AutoSearch: true,
                                Search: $.trim($('#txtProductSalesRepSearch').val()),
                                DisplayText: [{ Column: 'PeopleName' }],
                                Click: function (e) {
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
                                }
                            }
                        ])
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
                                NewMessageBox(vData.Error);
                            else if (vData.Result == 'ok') {
                                NewMessageBox('Alert was successfully saved', 'Success', null, {
                                    OnClose: function () {
                                        if ($.isFunction(proj.ShowCart))
                                            proj.ShowCart(1);
                                    } // OnClose: function() {
                                }); // NewMessageBox('Alert was successfully saved', 'Success', [], {

                            }; // else if (vData.Result == 'ok') {
                        } // success: function(vData) 
                    });
                }); // $('#butReportNoPrice').click(function(e) {

                $('#btnStockInfo').click(function (e) {
                    e.preventDefault();

                    if (CBool(sessionStorage.IsInventoryCountMode)) {
                        ShowStockInfo(false);
                        ShowInventoryCount(true);
                    }
                    else {
                        ShowStockInfo(true);
                        ShowInventoryCount(false);
                    }

                    //ShowStockInfo(true);
                    ShowProductSerialList(false);
                    ShowSalesRepresentativeList(false);
                    ShowInventoryBalances(false);

                    $("#lblPageHeader").text("Stock Info");
                    //ChangePage('Stock');
                    $('#divStockInfo').pageshow();
                });

                //Edited by Edmund R. Uba - August 14, 2015
                $('#btnSalesPerson').click(function (e) {
                    if (!Business.SystemDefaults.Only1SalesRep()) {
                        e.preventDefault();

                        ShowStockInfo(false);
                        ShowInventoryCount(false);
                        ShowProductSerialList(false);
                        ShowInventoryBalances(false);
                        ShowSalesRepresentativeList(true);

                        $("#lblPageHeader").text("Indicate Sales Person");
                        //ChangePage('Stock');
                        $('#divSalesRep').pageshow();
                    }
                });
                //End of edit

                $('#btnSerialNumber').click(function (e) {
                    e.preventDefault();

                    ShowStockInfo(false);
                    ShowInventoryCount(false);
                    ShowSalesRepresentativeList(false);
                    ShowInventoryBalances(false);
                    ShowProductSerialList(true);

                    $("#lblPageHeader").text("Serial Number(s)");
                    //ChangePage('Stock');
                    $('#divSerialNumber').pageshow();
                });

                $('#btnInventoryBalances').click(function (e) {
                    e.preventDefault();

                    ShowStockInfo(false);
                    ShowInventoryCount(false);
                    ShowSalesRepresentativeList(false);
                    ShowProductSerialList(false);
                    ShowInventoryBalances(true);

                    $("#lblPageHeader").text("Inventory Balances");
                    //ChangePage('Stock');
                    $('#divInventoryBalances').pageshow();

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
                    $('#tblInventoryBalance').css('width', '100%').empty().append($('<td style="text-align:center;">').append('<div class="spinner-border text-primary" role="status"><span class="visually-hidden"></span></div>'))
                    AjaxSend({
                        url: 'Stock/GetInventoryBalance',
                        data: { OrgID: sessionStorage.OrgID, StockID: proj.ProductID },
                        success: function (vData) {
                            $('#tblInventoryBalance').css('width', 'auto').empty();

                            tData = $('<tr>')
                                    .append($('<th>')
                                        .text('FOWST')
                                    )
                                    .append($('<th style="text-align:center;">')
                                        .text('Balance')
                                    )
                                    .append($('<br />'))

                            $('#tblInventoryBalance').append(tData);

                            var Total = 0;

                            for (i = 0; i < vData.Results.Rows.length; i++) {

                                Total += parseInt(vData.Results.Rows[i].Balance || 0);

                                tData = $('<tr>')
                                    .append($('<td>')
                                        .text(vData.Results.Rows[i].FOWST)
                                    )
                                    .append($('<td style="text-align:center;">')
                                        .text(parseInt(vData.Results.Rows[i].Balance || 0))
                                    )

                                $('#tblInventoryBalance').append(tData);
                            }


                            tData = $('<tr>')
                                    .append($('<td style="font-weight:bold;">')
                                        .text('Total :')
                                    )
                                    .append($('<td style="text-align:center;">')
                                        .text(parseInt(Total))
                                    )

                            $('#tblInventoryBalance').append(tData);

                            //try {
                            //    $('#lstProductInventoryBalances').listview('refresh');
                            //}
                            //catch (e) {
                            //    $('#lstProductInventoryBalances').listview();
                            //}
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

                        mHasFreeTypeAPrice = true;
                    }

                    return false;

                } // END function mPriceChange_onSubmit() {

                function HandleTestFreeTypeAPrice() {

                    if (!mHasFreeTypeAPrice)
                        return;

                    if (!OfflineMode)
                        return;

                    if (!Business.SystemDefaults.AlwaysTestFreeTypeAPrice())
                        return;

                    Business.TransInvoice.TagUntestedFreeTypeAPrice(sessionStorage.CartOrderID);

                } // END HandleTestFreeTypeAPrice


                //make it readonly
                mPriceChange.txtRequestedPrice.focus(function () {

                    if (!mAllowFreeTypeDiscountedPrice)
                        this.blur();
                });
                mPriceChange.txtRequestedPriceDiscountPercent.focus(function () {

                    if (!mAllowFreeTypeADiscountPercentage)
                        this.blur();
                });
                mPriceChange.txtRequestedPriceDiscountAmount.focus(function () {

                    if (!mAllowFreeTypeADiscountAmount)
                        this.blur();
                });

                mPriceChange.lnkSecurityDiscountedPrice.click(function () {
                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Allow to Free Type Discounted Price', {
                        OnClickOKSuccess: function () {
                            ChangePage('Stock');
                            mAllowFreeTypeDiscountedPrice = true;
                            ShowFreeTypePriceDialog();
                        },
                        OnFailed: function () {
                            mPriceChange.popup();
                        }
                    });
                });

                mPriceChange.lnkSecurityDiscountPercentage.click(function () {
                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Allow to Free Type a Discount Percentage', {
                        OnClickOKSuccess: function () {
                            ChangePage('Stock');
                            mAllowFreeTypeADiscountPercentage = true;
                            ShowFreeTypePriceDialog();
                        },
                        OnFailed: function () {
                            mPriceChange.popup();
                        }
                    });
                });

                mPriceChange.lnkSecurityDiscountAmount.click(function () {
                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Allow to Free Type a Discount Amount', {
                        OnClickOKSuccess: function () {
                            ChangePage('Stock');
                            mAllowFreeTypeADiscountAmount = true;
                            ShowFreeTypePriceDialog();
                        },
                        OnFailed: function () {
                            mPriceChange.popup();
                        }
                    });
                });

                function ShowFreeTypePriceDialog() {
                    if (!mAllowFreeTypeDiscountedPrice)
                        mAllowFreeTypeDiscountedPrice = Stock_offline().GetAllowFreeTypeDiscountedPrice();
                    if (!mAllowFreeTypeADiscountPercentage)
                        mAllowFreeTypeADiscountPercentage = Stock_offline().GetAllowFreeTypeADiscountPercentage();
                    if (!mAllowFreeTypeADiscountAmount)
                        mAllowFreeTypeADiscountAmount = Stock_offline().GetAllowFreeTypeADiscountAmount();

                    //additional by janine sept 29 2014 for disable fields

                    if (mAllowFreeTypeDiscountedPrice)
                        mPriceChange.txtRequestedPriceRemoveDisable(mAllowFreeTypeADiscountPercentage, mAllowFreeTypeADiscountAmount);


                    if (mAllowFreeTypeADiscountPercentage)
                        mPriceChange.txtRequestedPriceDiscountPercentRemoveDisable(mAllowFreeTypeDiscountedPrice, mAllowFreeTypeADiscountAmount);

                    if (mAllowFreeTypeADiscountAmount)
                        mPriceChange.txtRequestedPriceDiscountAmountRemoveDisable(mAllowFreeTypeDiscountedPrice, mAllowFreeTypeADiscountPercentage);
                    //end of additional


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