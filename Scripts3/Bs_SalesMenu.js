define(['common', 'business',
    //GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc='),
    //GetPathFormat('/Scripts3/Browser/TransLastestOrderedStockByCustomer.js?crc='),
    //GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc='),
    //GetPathFormat('/Scripts3/Browser/TransSalesOrder.js?crc='),
    //'promise!/Scripts3/Browser/TransInvoices.js?crc=',
    //'promise!/Scripts3/Browser/LibraryFowst.js?crc=',
    // '/Scripts3/SalesMenuSalesRepresentatives.js?crc=', // 2017_06_14__09_31_49_2347 // ki-pang-comment para paspas mu load
    (sessionStorage.IsWebPos == 'true' ? '' : '/Scripts3/SalesMenuOrderTaker.js?crc='),
    // '/Scripts3/controller/SignaturePage.js?crc=', // 2017_06_14__09_31_49_2347 // ki-pang-comment para paspas mu load
    (sessionStorage.IsWebPos == 'true' ? '' : '/Scripts3/Bs_CustomerDetails.js?crc='), 
    (sessionStorage.IsWebPos == 'true' ? '' : 'promise!/Scripts3/model/BomMaker.js?crc='), 
    (sessionStorage.IsWebPos == 'true' ? '' : '/Scripts3/Discounts.js?crc='),
    // 'promise!/Scripts3/controller/PeerConnect.js?crc=', // 2017_06_14__09_31_49_2347 // ki-pang-comment para paspas mu load
    // 'promise!/Scripts3/controller/PurchaseOrder.js?crc=', // 2017_06_14__09_31_49_2347 // ki-pang-comment para paspas mu load
    (sessionStorage.IsWebPos == 'true' ? '' : 'promise!/Scripts3/controller/PrePos.js'),
    (sessionStorage.IsWebPos == 'true' ? '' : 'promise!/Scripts3/controller/OrderReceiver.js'),
    (sessionStorage.IsWebPos == 'true' ? '' : '/Scripts3/controller/OrderTransmitter.js?crc='),
    (sessionStorage.IsWebPos == 'true' ? '' : '/Scripts3/model/OrderTakerSetup.js?crc='),
    (sessionStorage.IsWebPos == 'true' ? '' : '/Scripts/sources/SetupChecklist/NewPromptUser.js'),
    (sessionStorage.IsWebPos == 'true' ? '' : '/Scripts3/Offline/Browser/TransPimSalesCaching.js?crc=')
], function () {

        var mDeferred = $.Deferred();
        var mPrintedOrderSlip = false;

        // just a test 2017_02_02__14_14_00_4585
        function DisplayTableNumberOnButton() {
            var pTableNumber;
            var pTableName;

            pTableNumber = sessionStorage.CustomerTableNo;

            try {
                if (IsMobile) {
                    pTableName = mTableView.GetTableInfoByInternalName(pTableNumber);
                    if (CStr2(pTableName).trim() != '')
                        pTableNumber = pTableName;    
                }
                $('#lblLabel, #lblLabelHotButtons').html('Customer / Table No.:<br/>');
            } catch (e) {
                // 2017_06_14__09_31_49_2347 // ki-pang-comment para paspas mu load
                //debugger;
            }
            $('#lblCustomerNameHotButtons, #lblCustomerName').text(pTableNumber);
        }

        proj.goToHotButtonPage = goToHotButtonPage;
        var htmlHotButton = '';
        function goToHotButtonPage() {
            if (location.pathname.toLowerCase() == '/salesmenu/bsindex' || location.pathname.toLowerCase() == '/tableview/bsnewtable') {
                $('#pageHotButton').pageshow();

                if (htmlHotButton == '') {
                    AjaxSend({
                        ExactUrl: '/SalesMenu/BsHotButtons?cache=1&theme=bs',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            htmlHotButton = vData;
                            $('#pageHotButton').addClass('bg-dark').css({
                                position: 'fixed',
                                bottom: 0,
                                left: 0,
                                right: 0,
                                overflow: 'auto'
                            }).empty().append(vData).pageshow();
                            if (window.location.pathname.toLowerCase() == '/tableview/bsnewtable') {
                                $('#iconHBHome').removeClass('bi-house-fill').addClass('bi-arrow-left-circle');
                                $('#lblHBHome').text('Back to Table View');
                            }
                            LoadHotButton();
                        }
                    });
                } else {
                    LoadHotButton();
                    $('#pageHotButton').css('top', $('#divHotButtonHeader').height() + 'px');
                    $('#btnHotButtonMain').tap().click();
                }
            }            
        } // end of gotohotbuttonpage

        function LoadHotButton() {
            sessionStorage.PosMode = 'Hot Button Mode';
            //localStorage.setItem('PosView', 'Hot Button Mode');
            proj.ProductID = '';

            $('#txtOrder').html('');
            $('#txtQtyUnit').html('');
            startclock('#clockHotButtons');
            proj.CartStock = [];
            //ChangePage('/SalesMenu/BsHotButtons');

            //session storage cleared on peoplemenu js

            if (sessionStorage.IsHotButtonVisited == 'true') {
                $('#divSpace').hide();
                //                        $.mobile.changePage('#pageButtons');
                //                                        GetData(sessionStorage.HotButtonMenuID);
                $('#divSpace').css('height', '0px');
            }

            if (CStr2(sessionStorage.CustomerTableNo) != '') {

                DisplayTableNumberOnButton();
                try {

                    $('#selTableNoHotButtons').val(sessionStorage.CustomerTableNo).selectmenu('refresh', true);
                } catch (e) {

                    $('#selTableNoHotButtons').val(sessionStorage.CustomerTableNo);
                    $('#selTableNoHotButtons').selectmenu();

                }

                $('#lblLabelHotButtons').html('Customer / Table No.:<br/>');

            }

            if (CStr2(sessionStorage.GasPumpNo) != '') {

                $('#lblCustomerNameHotButtons').text(sessionStorage.GasPumpNo);

                try {
                    $('#selGasPumpNoHotButtons').val(sessionStorage.GasPumpNo).selectmenu('refresh', true);
                } catch (e) {
                    $('#selGasPumpNoHotButtons').val(sessionStorage.GasPumpNo);
                    $('#selGasPumpNoHotButtons').selectmenu();

                }

                $('#lblLabelHotButtons').html('Gas Pump No.:<br/>');
            }

            if (sessionStorage.GasPumpNo == '' && sessionStorage.CustomerTableNo == '') {

                $('#lblCustomerNameHotButtons').text(sessionStorage.CustomerOrganizationName);

                try {
                    $('#selTableNoHotButtons').val('').selectmenu('refresh', true);

                } catch (e) {
                    $('#selTableNoHotButtons').val('');
                    $('#selTableNoHotButtons').selectmenu();

                }
                $('#lblLabelHotButtons').html('Customer:<br/>');

            }

            if (window.location.pathname.toLowerCase() == '/tableview/bsnewtable') {
                window.pageCustomerDetails.LoadCustomer();
            }
        }

        proj.SalesMenuPage = $.mobile.activePage;
        if (!CBool(GetUrlParameters().HotButtons) && window.location.pathname.toLowerCase() != '/tableview/bsnewtable') {
            $('#pageMain').pageshow();
        }

        function SelectProduct(e, vOpt) {
            proj.ProductID = e.ID;
            e.ProductID = e.ID;
            vOpt.DontClose = true;
            sessionStorage.IsDraft = 1;
            window.NewStockBrowser = true;
            startclock('#clockStock');

            proj.RebateData = null;
            proj.ModifierGroupID = null;
            proj.GroupIdOfItemModifiers = null;
            sessionStorage.removeItem('ItemModifier');
            if (sessionStorage.PosMode == 'Widescreen')
                proj.SaveCart();

            if (!CBool(proj.POMode)
                && window.OfflineMode) {

                if (window.Business.LibraryItemModifiers.HasModifier(e.ID) && location.pathname.toLocaleLowerCase() != '/billofmaterials') {
                    NewItemModifier({
                        AllowSKUQty: location.pathname.toLocaleLowerCase() == '/salesmenu/bsposviewall',
                        ProductData: e,
                        success: function (e) {
                            if (location.pathname.toLocaleLowerCase() == '/salesmenu/bsposviewall') {
                                e.ProductID = e.ID;
                                e.UnitPrice = e.DefaultPrice;
                                $.extend(e, Business.LibraryPIMProduct.BaseUnit(e.ID));
                                var pItem = HotButtons.UpdateCartStock($.extend({
                                    ModifierGroupID: proj.ModifierGroupID,
                                    IsOrderSlipPrinted: 0,
                                    GroupIdOfItemModifiers: NewID() // for grouping the productid and selected item modifier, temporary only
                                }, e), false, e.Quantity, {
                                    DoAdd: true
                                });
                                proj.HotButtonDisplayStock(pItem);
                            } else {
                                var cartitems = proj.ItemsInCart || JSON.parse("[]");
                                var groupID = '';
                                for (var i = 0; i < cartitems.length; i++) {
                                    if ((cartitems[i].SelectedItemModifierID).join(',') == (e.SelectedItemModifierID || []).join(',')) {
                                        groupID = cartitems[i].GroupIdOfItemModifiers;
                                        break;
                                    }
                                }
                                if (groupID == '') {
                                    proj.GroupIdOfItemModifiers = NewID();
                                } else {
                                    proj.GroupIdOfItemModifiers = groupID;
                                }
                                sessionStorage.ItemModifier = JSON.stringify(e.ItemModifier);
                                Bs_ShowStockPage();
                            }
                        }
                    });
                    //ItemModifier({
                    //    ProductName: e.ProductName,
                    //    ProductID: e.ID,
                    //    OnClickNoModification: function () {
                    //        Bs_ShowStockPage();
                    //    },
                    //    OnClickRebates: function () {
                    //        ItemModifier_Rebates();
                    //    },
                    //    OnClickRebatesDone: function (vData) {
                    //        if (vData != null) {
                    //            proj.RebateData = vData;
                    //            proj.ModifierGroupID = NewID();
                    //        }
                    //        Bs_ShowStockPage();
                    //    }
                    //});
                } else
                    Bs_ShowStockPage();
            } else
                Bs_ShowStockPage();
        }
        var newui = true;
        proj.DisplayStockIteModifier = function (vData, container, isFromHotButtonsWithoutSideCart) {
            var id, ItemContainer;
            if (CBool(vData.HasModifier) == false) {
                id = (vData.ProductID).toUpperCase();
            } else {
                id = (vData.ProductID + '_' + vData.GroupIdOfItemModifiers).toUpperCase();
            }
            //if using retail view mode, container is not undefine 
            if (container == undefined) {
                ItemContainer = $("#" + id + "-Order");
            } else {
                ItemContainer = container;
            }

            var modifierContainer = ItemContainer.find('.divCartItemModifier');
            var itemmodifiersubtotal = 0;
            var itemsubtotal = 0;
            var mIsPremiumPriceApplied = false;
            var mPremiumCount = 0;
            var mIsAdditionalMaxAllowableDiscountApplied = false;

            sessionStorage.AdditionalMaxAllowableDiscount = 0;

            if (modifierContainer.length == 0) modifierContainer = ItemContainer; //new UI sidecart

            if (vData.ItemModifier.length > 0) {
                ItemContainer.find('.cartListTotal').removeClass('cartListTotal'); // remove cartListTotal if theres an item modifier
                for (var i = 0; i < vData.ItemModifier.length; i++) {
                    var im = vData.ItemModifier[i];

                    if (im.SelectedModifier == undefined) {
                        continue;
                    }

                    for (var m = 0; m < im.SelectedModifier.length; m++) {
                        var modifier = im.SelectedModifier[m];
                        if (im.Type == "Inventory Affecting") {
                            var modifiertotal, mPremiumPrice = 0;
                                   
                            if (CDec2(modifier.PremiumPrice) > 0) {                                
                                mPremiumCount += CInt2(modifier.InputQty);
                            }

                            if (CDec2(modifier.PremiumPrice) > 0 && !mIsPremiumPriceApplied) {
                                mPremiumPrice = parseFloat(modifier.PremiumPrice);
                                modifiertotal = (parseFloat(modifier.UnitPrice) * parseFloat(modifier.OrderQty)) + (parseFloat(modifier.PremiumPrice) * parseFloat(vData.Quantity));
                                mIsPremiumPriceApplied = true
                            }
                            else {
                                modifiertotal = parseFloat(modifier.UnitPrice) * parseFloat(modifier.OrderQty);
                            }

                            if (container == undefined || CBool(isFromHotButtonsWithoutSideCart)) {
                                if (location.pathname.toLowerCase() == '/salesmenu/bsposviewall' || CBool(sessionStorage.IsWebPos)) {
                                    if (m == 0 && modifierContainer.find('.line').length == 0) {
                                        modifierContainer.append($('<tr class="addons line">')
                                            .append('<td></td>')
                                            .append('<td colspan="3"><i>Add Ons Per Order</i></td>')
                                        )
                                    }

                                    modifierContainer.append($('<tr class="addons">')
                                        .append('<td class="text-center">' + modifier.InputQty + '</td>')
                                        .append('<td>' + modifier.ProductName + '</td>')
                                        .append('<td class="text-right">' + AccountingFormat(modifier.UnitPrice) + '</td>')
                                        .append('<td class="text-right">' + AccountingFormat(modifiertotal - (mPremiumPrice * parseFloat(vData.Quantity))) + '</td>')
                                    )

                                    if (mPremiumPrice > 0) {
                                        $('<tr class="addons">')
                                            .append('<td></td>')
                                            .append('<td>Premium Charge</td>')
                                            .append('<td class="text-right">' + AccountingFormat(mPremiumPrice) + '</td>')
                                            .append('<td class="text-right pr-1">' + AccountingFormat(mPremiumPrice * parseFloat(vData.Quantity)) + '</td>')
                                            .insertAfter(modifierContainer.find('.mainsku'))
                                        mPremiumPrice = 0; //meaning humanag apply sa UI
                                    }
                                } else {
                                    //for OLD UI sidecart by Arnie 10-05-2023
                                    modifierContainer.append($('<div class="row">')
                                        .append($('<div class="col-8" style="border-right: 1px solid #CCC; font-weight:normal;">')
                                            .html(' <span class="required">+<span> ')
                                            .append(modifier.OrderQty + ' x ' + modifier.ProductName + ' @ ' + (CDec2(modifier.UnitPrice) == 0 ? '0.00' : AccountingFormat($.trim(modifier.UnitPrice))) + ' each')
                                            .append('<br>')
                                            .append(mPremiumPrice > 0 ? '<div class="pl-3">+ ' + AccountingFormat(mPremiumPrice) + ' (Premium Price)</div>' : '')
                                        )
                                        .append($('<div class="col-4" style="width:60px; text-align:right; font-weight:normal;">')
                                            .text(AccountingFormat(modifiertotal))
                                        )
                                    )
                                }
                            } else {
                                var tblPremium = '<table><tbody><tr><td colspan="7" class="pl-3">+ ' + AccountingFormat(mPremiumPrice) + ' (Premium Price) = ' + AccountingFormat(modifiertotal) + '</td></tr></tbody></table>'

                                modifierContainer.append($('<div style="font-weight:normal;">')
                                    .html(' <span class="required">+<span> ').append(modifier.ProductName + '<br>')
                                    .append($('<table class="tableUnits">')
                                        .append($('<tbody>')
                                            .append($('<tr>')
                                                .append($('<td>').text(modifier.OrderQty))
                                                .append($('<td>').text('@'))
                                                .append($('<td>').text((CDec2(modifier.UnitPrice) == 0 ? '0.00' : AccountingFormat($.trim(modifier.UnitPrice)))))
                                                .append($('<td>').text('each')) // @ modified by arnie 92017
                                                .append($('<td>').text('')) // (vData.Quantity || vData.OrderedQuantity) + ' ' + vData.UnitMeasure
                                                .append($('<td>').text((CDec2(modifiertotal) == 0 ? '0.00' : (mPremiumPrice > 0 ? '' : ' = ' + AccountingFormat(modifiertotal)))))
                                            )
                                        )                                        
                                    )
                                    .append(mPremiumPrice > 0 ? tblPremium : '')
                                )
                            }                            
                            itemmodifiersubtotal += modifiertotal;

                            if (mPremiumCount > 1 && !CBool(mIsAdditionalMaxAllowableDiscountApplied)) {
                                if ((CInt2(im.Max) == 3 && mPremiumCount == 3) || (CInt2(im.Max) == 2 && mPremiumCount == 2)) { //3 = all premium tera, 2 = all premium mega and giga
                                    sessionStorage.AdditionalMaxAllowableDiscount = parseFloat(modifier.UnitPrice) * 2; //mPremiumCount; gicomment kay and premiumcount ni tera is 3
                                    mIsAdditionalMaxAllowableDiscountApplied = true;
                                }                                
                            }

                        } else if (im.Type == "Non-Inventory Affecting") {
                            if (container == undefined || CBool(isFromHotButtonsWithoutSideCart)) {
                                if (newui) {
                                    modifierContainer.append($('<tr class="addons">')
                                        .append('<td></td>')
                                        .append('<td colspan="4">' + (modifier.Description == 'Others' ? modifier.Others : modifier.Description) + '</td>')
                                    )
                                } else {
                                    //for OLD UI sidecart by Arnie 10-05-2023
                                    modifierContainer.append($('<div class="row">')
                                        .append($('<div class="col-8" style="border-right: 1px solid #CCC; font-weight:normal;">')
                                            .html(' <span class="required">+<span> ')
                                            .append((modifier.Description == 'Others' ? modifier.Others : modifier.Description))
                                        )
                                        .append($('<div class="col-4" style="width:60px; text-align:right;">'))
                                    )
                                }
                            } else {
                                modifierContainer.append($('<div style="font-weight:normal;">').append('<span class="required">+<span> ')
                                    .append((modifier.Description == 'Others' ? modifier.Others : modifier.Description)))
                            }
                        }
                    }
                }
                //(below) vData.UnitPrice use in populating data in cart when payment page is called using hot button w/ sidecart
                itemsubtotal = (CDec2(vData.Quantity) * CDec2((vData.DiscountedPrice || vData.UnitPrice))) + CDec2(itemmodifiersubtotal);

                if (container == undefined || CBool(isFromHotButtonsWithoutSideCart)) {
                    if (newui) {
                        modifierContainer.append($('<tr class="addons font-weight-bold d-none">')
                            .append($('<td colspan="4">')
                                .append($('<div class="row g-0">')
                                    //.append('<div class="col-auto font-weight-light"><i>Tap item to modifiy</i></div>')
                                    .append('<div class="col text-right">Sub Total: <span class="cartListTotal">' + AccountingFormat(itemsubtotal) + '</span></div>')
                                )
                            )
                        )
                    } else {
                        //for OLD UI sidecart by Arnie 10-05-2023
                        modifierContainer.append($('<div class="row">')
                                    .append($('<div class="col-8" style="border-top: 1px solid #CCC; text-align:right;">')
                                        .text('SUB TOTAL: ')
                                    )
                                    .append($('<div class="col-4 cartListTotal" style="width:60px; border-top: 1px solid #CCC; text-align:right;">')
                                        .text(AccountingFormat(itemsubtotal))
                                    )
                                )
                    }
                }                
            } else {
                itemsubtotal = (CDec2(vData.Quantity) * CDec2((vData.DiscountedPrice || vData.UnitPrice)));
            }

            return CDec2(itemsubtotal);
        }
        // END just a test 2017_02_02__14_14_00_4585

        if (CBool(sessionStorage.IsWebPos)) return true;

        require(['/Content/Ecommerce/js/lightbox-2.11.1.js', (OfflineMode ? '/Scripts3/SalesMenu_offline.js?crc=' : ''),
            (OfflineMode ? '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc=' : ''),
            (OfflineMode ? '/Scripts3/Offline/Business/SystemDefaults.js?crc=' : ''),
            (OfflineMode ? '/Scripts3/Offline/Business/SystemDefaults.js?crc=' : ''),
            (OfflineMode ? '/Scripts3/model/TableView_Offline.js?crc=' : '')], function () {                

            $(function () {
                // proj.SalesMenuPage = $.mobile.activePage; // move to run earlier 2017_02_02__16_34_29_2590
                proj.SalesMenuPage.data('CanRefreshStockPage', 1);

                var mScroll; // iScroll
                var mLvRecentlyBought;
                var mLvCart;
                var mRecentlyBoughtDateRangeType;
                var mPageHotButtons;
                var isRecallCart = false;
                var lblVATableSales = 0;
                //ADDITIONAL BY JANINE FEB 18
                var isCartEmpty = true;
                var isOnCartPage = true;
                var isChangeCustomer = false;
                var IsPreviewBill = false;
                var mPurchaseOrder; // App.controller.PurchaseOrder
                var arrPODetails = {};
                //                ClearCustomerFreeTypeSession();
                var mTableView;
                var mOrderTakerSetupModel; // App.model.OrderTakerSetup
                var mID;
                var mPrePos = new App.controller.PrePos();
                mPrePos.UpdateOrderedQuantity = UpdateOrderedQuantity;

                if ($.isFunction($("[data-role=header]").fixedtoolbar)) // jqm 1.4.4 doesn't have fixedtoolbar function
                    $("[data-role=header]").fixedtoolbar({ tapToggle: false });

                sessionStorage.CustomerTableNo = '';
                sessionStorage.GasPumpNo = '';
                proj.DisplayCart = LoadCart;

                sessionStorage.SupplierOrganizationID = '';
                sessionStorage.SupplierOrganizationName = '';
                sessionStorage.SupplierAddress = '';
                sessionStorage.SupplierPriceLevelID = '';
                sessionStorage.SupplierPriceLevelName = '';
                proj.ItemsInCart = [];
                //additional oct 23

                if (CStr2(sessionStorage.IsEnableDiningMode) == 'true')
                    sessionStorage.DeliveryMethod = 'Dine In';
                else
                    sessionStorage.DeliveryMethod = 'PickUp';


                sessionStorage.OrderMethod = 'Walk In';
                sessionStorage.Gender = 'NA';
                sessionStorage.Age = 'Mixed Ages/ NA';
                sessionStorage.DeliveryAddressID = '';
                sessionStorage.BillingAddressID = '';
                sessionStorage.DeliveryDate = '';
                sessionStorage.IsPaymentJSLoaded = 'false';
                var dblTotalAmountDueForDiscount = 0;
                var intSalesPayCustomerNameCntr = 0;
                var isSeniorCitizenPWDDiscount = false;
                var showDiscountDetails = false;
                sessionStorage.CurrentPage = 'pos';
                var newPrinterIP;
                var newPrinterFowstID;
                var vatSale = 0;
                var dblTotalAmountDue = 0;
                var isSeniorCitizenDiscount = false;
                var originalDevFowstPrinterID;
                var longDiscountPercent;
                var longChargeAmountBilling = 0;
                var mPeerConnect;
                var mOrderReceiver;
                var htmlPaymentPage = '';

                /////////////////////
                proj.SalesMenuInit = function (vSalesMenuInitParams) {
                    mPrePos.NewCart();
                    if (vSalesMenuInitParams == null) {
                        vSalesMenuInitParams = {
                            CartOrderID: sessionStorage.getItem('CartOrderID'),
                            TableNo: GetUrlParameters().TableNo
                        };
                    } else {
                        if (IsGuid(vSalesMenuInitParams.CartOrderID))
                            sessionStorage.setItem('CartOrderID', vSalesMenuInitParams.CartOrderID);
                        else
                            sessionStorage.removeItem('CartOrderID');
                    }

                    $('.hide1st').removeClass('hide1st');
                    mOrderTakerSetupModel = new App.model.OrderTakerSetup();
                    if ($.isFunction(GetStringObject('GetIsOrderTaker', mOrderTakerSetupModel)))
                        if (mOrderTakerSetupModel.GetIsOrderTaker())
                            sessionStorage.setItem('IsOrderTaker', 1);
                        else
                            sessionStorage.setItem('IsOrderTaker', 0);
                    else
                        sessionStorage.setItem('IsOrderTaker', 0);
                    proj.SalesMenu = proj.SalesMenu || {};
                    proj.SalesMenu.PaymentsFunction = function (vParams) {
                        /* vParams: {
                            DontPopupBilling: false
                        } */

                        if (OfflineMode) {
                            Android.Toast('Checking transaction data for validity');
                            var vData = Stock_offline().UpdateOrderedQuantity(sessionStorage.getItem('CartOrderID'));
                            if (vData.Result != 'ok') {
                                NewMessageBox(vData.Result);
                                return;
                            }
                            var pDtZeroUnitQty = Business.TransInvoice.GetZeroUnitQty(sessionStorage.getItem('CartOrderID'));
                            if (pDtZeroUnitQty.Rows.length > 0) {
                                NewMessageBox('The unit quantity for ' + pDtZeroUnitQty.Rows[0].ProductName + ' is invalid. Please update it\'s quantity in the cart.');
                                return;
                            }

                            Business.TransInvoice.HandleTestFreeTypeAPrice({
                                Callback: function () {
                                    Proceed(vParams);
                                }
                            });
                        } else
                            Proceed(vParams);

                        function Proceed(vParams) {
                            if (!CBool(proj.POMode) && CBool(sessionStorage.EnableHotButtonsContextualMenu)) {
                                PopupButtonList('', '', [
                                    {
                                        Text: 'Go to ' + (proj.SOMode ? 'sales order details' : 'payments'),
                                        Size: 2,
                                        Click: function () {
                                            if (proj.SOMode)
                                                SaveSalesOrder(vParams);
                                            else
                                                proj.SalesMenu.GoToPaymentsPage();
                                        }
                                    },
                                    //{
                                    //    Text: 'Print Sales Invoice',
                                    //    Click: function() {
                                    //        NewMessageBox('Printing sales invoice');
                                    //    }
                                    //},
                                    {
                                        Text: 'Print Billing',
                                        Size: 2,
                                        Click: function () {
                                            proj.SaveCart({
                                                ForceSaveHeader: true
                                            });
                                            sessionStorage.PrintOption = 'billingorderslip';
                                            mPrintedOrderSlip = false;
                                            var pResult;
                                            pResult = SalesMenu_offline().SaveSalesOrder({
                                                InvoiceID: sessionStorage.getItem('CartOrderID')
                                            });
                                            PrintBilling(sessionStorage.getItem('CartOrderID'));
                                            // NewMessageBox('Printing billing');
                                        }
                                    },
                                    {
                                        Text: 'Print Order Slip (and cancellation slip if applicable)',
                                        Size: 2,
                                        Click: function () {
                                            if (!proj.HandlePosIsEditable(true))
                                                return false;
                                            sessionStorage.PrintOption = 'orderslip';
                                            mPrintedOrderSlip = true;
                                            var pResult;
                                            pResult = SalesMenu_offline().SaveSalesOrder({
                                                InvoiceID: sessionStorage.getItem('CartOrderID'),
                                                AsOrderSlip: true
                                            });
                                            PrintBilling(sessionStorage.getItem('CartOrderID'), {
                                                PrintAsOrderSlip: true,
                                                ByCategory: true
                                            });
                                        }
                                    }
                                    //{
                                    //    Text: 'Print Sales Invoice and Billing',
                                    //    Click: function() {
                                    //        NewMessageBox('Printing Sales Invoice and Billing');
                                    //    }
                                    //},
                                    //{
                                    //    Text: 'Print Sales Invoice and Order Slip',
                                    //    Click: function() {
                                    //        NewMessageBox('Printing Billing and Order Slip');
                                    //    }
                                    //},

                                    // hidden for now 2016_09_29__13_00_56_3362
                                    //{
                                    //    Text: 'Print Billing and Order Slip',
                                    //    Size: 2,
                                    //    Click: function() {
                                    //        NewMessageBox('Printing Billing and Order Slip');
                                    //    }
                                    //}

                                ], { CloseButton: 'enable', AutoDismiss: true });

                                return;
                            } // END if (sessionStorage.EnableHotButtonsContextualMenu) {
                            //additional by janine
                            //Check if a customer is selected
                            if (sessionStorage.CustomerOrganizationID == undefined) {
                                NewMessageBox('Please select a customer.',
                                    'Customer',
                                    [
                                        {
                                            Text: 'Ok'
                                        }
                                    ]
                                );
                                return;
                            }
                            //Check if a FOWST is selected
                            if (sessionStorage.FowstID == undefined) {
                                NewMessageBox('Please select your current FOWST location. See "More Options".',
                                    'FOWST',
                                    [
                                        {
                                            Text: 'Ok'
                                        }
                                    ]
                                );
                                return;
                            } else {

                                if (!isOnCartPage) {
                                    if (isCartEmpty) {
                                        LoadCart(true);
                                        return;
                                    } else {
                                        LoadCart();
                                        return;

                                    }

                                } else {
                                    // commented 2016-12-09
                                    //if (isCartEmpty && CDec2(GetStringObject('proj.CartStock.length')) == 0) {
                                    //    NewMessageBox('Your shopping cart is empty',
                                    //        'Check Out',
                                    //        [
                                    //            { Text: 'Ok' }
                                    //        ]
                                    //    );
                                    //    return;
                                    //    //                                if (!CBool(Business.SystemDefaults.IsEnableBillingOption())) {
                                    //    //                                    NewMessageBox('Your shopping cart is empty',
                                    //    //                                        'Pay Now',
                                    //    //                                        [
                                    //    //                                            { Text: 'Ok' }
                                    //    //                                        ]
                                    //    //                                    );
                                    //    //                                    return;
                                    //    //                                } else {

                                    //    //                                    $('#popupPayNowMode').popup('open');


                                    //    //                                }

                                    //}


                                }

                            }


                            if (proj.SOMode || proj.POMode) {
                                //                        if (CStr2(sessionStorage.CustomerTableNo) != '' || CStr2(sessionStorage.GasPumpNo) != '') {
                                //                            var toSave;

                                //                            if (CStr2(sessionStorage.GasPumpNo) != '')
                                //                                toSave = 'GP_' + sessionStorage.GasPumpNo;

                                //                            if (CStr2(sessionStorage.CustomerTableNo) != '')
                                //                                toSave = 'TBL_' + sessionStorage.CustomerTableNo;

                                //                            if (OfflineMode) {

                                //                                if (SalesMenu_offline().SaveCustomerDetails(toSave, sessionStorage.getItem('CartOrderID')))
                                //                                    SaveSalesOrder();

                                //                            } else {
                                //                                SaveCustomerDetails(toSave);
                                //                            }

                                //                        } else
                                SaveSalesOrder(vParams);
                            } else
                                proj.SalesMenu.GoToPaymentsPage();
                        } // END function Proceed() {
                    } // END function PaymentsFunction() {
                    proj.SalesMenu.GoToPaymentsPage = function () {
                        if (mOrderTakerSetupModel.GetIsOrderTaker()) {
                            sessionStorage.setItem('IsOrderTaker', 1);
                            NewMessageBox('This device is setup as an order taker. Order takers cannot go to payments page.');
                            return;
                        } else
                            sessionStorage.setItem('IsOrderTaker', 0);

                        PromptUserAccess('Can go to payments page', {
                            IncludeNoAccess: true,
                            OnClickOKSuccess: function () {
                                if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall' ||
                                    window.location.pathname.toLowerCase() == '/salesmenu/bsindex' ||
                                    window.location.pathname.toLowerCase() == '/tableview/bsnewtable') {
                                    ShowPaymentPage();
                                } else {
                                    // bookmark: retail view go to payments page
                                    var pLocation = MergeUrlParameters({
                                        InvoiceID: sessionStorage.getItem('CartOrderID'),
                                        IsMobile: CBool(IsMobile) ? 1 : 0,
                                        theme: 'bs'
                                    }, "/SalesPayments/BsIndex");
                                    if (InAndroid &&
                                        (localStorage.PosView != 'Table View')) {

                                        //Android.ScriptToWebView({
                                        //    Script: 'proj.SalesPayments.ClearSalesPaymentsInfoFields();',
                                        //    WebViewIndex: 2
                                        //});
                                        //Android.SetSessionStorage({
                                        //    WebViewIndex: 2
                                        //});
                                        //Android.ScriptToWebView({
                                        //    Script: 'proj.ShowPaymentDetails();',
                                        //    WebViewIndex: 2
                                        //});
                                        //Android.SetCurrentTab(1);
                                    } else
                                        window.location = pLocation;
                                }

                                if (GetUrlParameters().TableNo != undefined) {
                                    $('#lblRep').text('Assign Wait Staff');
                                    $('#spanPaymentMoreOptions').text('Print Options');
                                }
                                else {
                                    $('#lblRep').text('Assign Rep');
                                }
                            }
                        });

                    } // END proj.SalesMenu.GoToPaymentsPage = function() {

                    proj.ShowPaymentPage = ShowPaymentPage
                    function ShowPaymentPage(isHide) {
                        if (!CBool(isHide)) {
                            if ($('#divCartRetail > li').length == 0 && $('#pushSection > li').length == 0 && $('#tblSideCartItems > tbody').length == 0) {
                                NewMessageBox('There are no items in the cart. Please add at least one.')
                                return;
                            }

                            $('#pagePayment').pageshow();
                        }                        

                        if (htmlPaymentPage == '') {
                            setTimeout(function () {
                                AjaxSend({
                                    ExactUrl: '/SalesPayments/BsIndex?cache=1&theme=bs',
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function (vData) {
                                        htmlPaymentPage = vData;
                                        $('#pagePayment').empty().append(vData);
                                    }
                                });
                            }, 500)
                        } else {
                            if (sessionStorage.CartOrderID != undefined) {
                                localStorage.CartOrderID = sessionStorage.CartOrderID;
                            }
                            proj.SalesPayments.ClearSalesPaymentsInfoFields();
                            if (sessionStorage.CartOrderID == undefined) {
                                sessionStorage.CartOrderID = localStorage.CartOrderID;
                            }
                            proj.ShowPaymentDetails();
                        }
                    } // END function ShowPaymentPage() {

                    proj.SalesMenu.PrintBilling = function (pInvoiceID, vOptions) {
                        var vDataPrint;
                        var vDataHeader;
                        var vDataInvHeader;
                        var vDataItem;
                        var vDataItemSerial;
                        var vDataPayment;
                        var vDataFooter;
                        var vDataMsg;
                        var pData = {};
                        var pDontReturn;
                        pData.InvoiceID = pInvoiceID;

                        // repeated this code below
                        var pOptions = $.extend({
                            ByCategory: false,
                            DtPrinter: null
                        }, vOptions);
                        SalesPayment_offline().AddDefaultServiceCharge(sessionStorage.CartOrderID);
                        if (pOptions.PrintAsOrderSlip
                            && (!CBool(pOptions.DontPrintCancellationSlip)) && (!CBool(pOptions.IsReprintAll))) {

                            var pHasPrintable = Business.TransInvoice.HasPrintableOrderSlip(pInvoiceID)
                            if (!pHasPrintable) {
                                NewMessageBox('You have already printed an order slip(s) (and cancellation slip if applicable) for the items and quantities on this cart. Would you like to re-print "ALL" order slip(s)?',
                                                'Options',
                                                [
                                                    {
                                                        Text: 'Yes, re-print "All" order slip(s)',
                                                        Size: 3,
                                                        Click: function () {
                                                            PromptUserAccess('POS Receipt Reprint', {
                                                                OnClickOKSuccess: function () {

                                                                    //vOptions.IsReprintAll = true;
                                                                    //vOptions.DontPrintCancellationSlip = true;
                                                                    //var pOptions2 = $.extend(pOptions, {
                                                                    //    PrintAsOrderSlip: true,
                                                                    //    PrintAsCancellationSlip: false
                                                                    //});
                                                                    //proj.SalesMenu.PrintBilling(pInvoiceID, pOptions2);

                                                                    PrintBilling(sessionStorage.CartOrderID, {
                                                                        PrintAsOrderSlip: true,
                                                                        ByCategory: true,
                                                                        IsReprintAll: true
                                                                    });

                                                                },
                                                                OnFailed: function () {
                                                                    NewMessageBox('Sorry, you don\'t have access to reprint order slip(s)', '');
                                                                }
                                                            });
                                                        }
                                                    }, {
                                                        Text: 'No, don\'t reprint order slip(s)',
                                                        Size: 3,
                                                        Click: function () {
                                                            return;
                                                        }
                                                    }
                                                ]
                                            );
                                return;
                            }

                            vOptions.IsReprintAll = false;
                            vOptions.DontPrintCancellationSlip = true;
                            var pOptions2 = $.extend(pOptions, {
                                PrintAsOrderSlip: false,
                                PrintAsCancellationSlip: true
                            });
                            proj.SalesMenu.PrintBilling(pInvoiceID, pOptions2);
                            //return;
                        }

                        // dont delete this repeated code.
                        pOptions = $.extend({
                            ByCategory: false,
                            DtPrinter: null
                        }, vOptions);

                        //Get Printer IP Address
                        if (OfflineMode) {
                            if ((pOptions.Category == null)
                                && (pOptions.ByCategory)) {

                                var pDrNoCategory;
                                var pDrDefaultPrinter;
                                var pCategoryPrinters = [];
                                var IsUsingWPS = false;

                                var pDtCategories = Business.TransInvoice.GetCategories(pInvoiceID, pOptions);
                                pDontReturn = pDtCategories.Rows.length == 0;

                                var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                                if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                    IsUsingWPS = true;
                                }

                                for (var p = 0; p <= pDtCategories.Rows.length - 1; p++) {
                                    var pDR = pDtCategories.Rows[p];
                                    if ($.isEmptyObject(pDR)
                                        || (CStr2(pDR.Category).trim() == '')) {

                                        if (pDrDefaultPrinter == null) {
                                            pDrDefaultPrinter = GetDefaultPrinter();
                                            pDrNoCategory = JSON.parse(JSON.stringify(pDrDefaultPrinter)); // clone object
                                            pDrNoCategory.CategoryName = '';
                                            pCategoryPrinters.push(pDrNoCategory);
                                        }
                                    }
                                    var pDtCategoryPrinter = Business.TransInvoice.GetPrinters(sessionStorage.FowstID, pDR.Category, IsUsingWPS);
                                    if (pDtCategoryPrinter.Rows.length > 0) {
                                        pCategoryPrinters.push(pDtCategoryPrinter.Rows[0]);
                                    } else {
                                        if (pDrDefaultPrinter == null)
                                            pDrDefaultPrinter = GetDefaultPrinter();
                                        var pDrDefaultPrinterClone = JSON.parse(JSON.stringify(pDrDefaultPrinter)); // clone object
                                        pDrDefaultPrinterClone.CategoryName = pDR.Category;
                                        pCategoryPrinters.push(pDrDefaultPrinterClone);
                                    }
                                }

                                pCategoryPrinters.sort(function (v1, v2) {
                                    v1.PrinterIPAddress = CStr2(v1.PrinterIPAddress).trim().toLowerCase();
                                    v2.PrinterIPAddress = CStr2(v2.PrinterIPAddress).trim().toLowerCase();

                                    if (v1.PrinterIPAddress < v2.PrinterIPAddress)
                                        return -1;
                                    else if (v1.PrinterIPAddress > v2.PrinterIPAddress)
                                        return 1;
                                    else {
                                        if (CStr2(v1.CategoryName).trim().toLowerCase() < CStr2(v2.CategoryName).trim().toLowerCase())
                                            return -1;
                                        else if (CStr2(v1.CategoryName).trim().toLowerCase() > CStr2(v2.CategoryName).trim().toLowerCase())
                                            return 1;
                                        else
                                            return 0;
                                    }
                                });

                                var pDontPrintHeader = false;
                                for (var p = 0; p <= pCategoryPrinters.length - 1; p++) {
                                    var pPrintOption = {
                                        Category: pCategoryPrinters[p].CategoryName,
                                        DtPrinter: {
                                            Rows: [pCategoryPrinters[p]]
                                        },
                                        DontPrintHeader: pDontPrintHeader,
                                        DontPrintFooter: false
                                    };

                                    if (pOptions.PrintAsOrderSlip || pOptions.PrintAsCancellationSlip)
                                        if (p < pCategoryPrinters.length - 1)
                                            if (pCategoryPrinters[p].PrinterIPAddress == pCategoryPrinters[p + 1].PrinterIPAddress) {
                                                var pFowstSetting = Business.LibraryOrganizationFOWST.ByID(sessionStorage.FowstID);
                                                if ((pFowstSetting != null)
                                                    && (CBool(pFowstSetting.ConsolidateOrderSlipPrinting)))

                                                    pPrintOption.DontPrintFooter = true;
                                            }
                                    var pOptions3 = $.extend(pOptions, pPrintOption);
                                    proj.SalesMenu.PrintBilling(pInvoiceID, pOptions3);
                                    pDontPrintHeader = pPrintOption.DontPrintFooter;
                                }

                                function GetDefaultPrinter() {
                                    var pDT = SalesPayment_offline().getOrgFOWSTPrinterType();
                                    if (pDT.Rows.length > 0)
                                        return pDT.Rows[0];
                                    else
                                        console.error('No default printer for the current FOWST and device');
                                }

                                /* old version - 2017_12_04__08_05_20_0626
                                var pDtCategories = Business.TransInvoice.GetCategories(pInvoiceID, pOptions);
                                pDontReturn = pDtCategories.Rows.length == 0;
                                for (var p = 0; p <= pDtCategories.Rows.length - 1; p++) {
                                    var pDR = pDtCategories.Rows[p];
                                    if ($.isEmptyObject(pDR)) {
                                        pDontReturn = true;
                                        break;
                                    }
                                    var pDtCategoryPrinter = null;
                                    if (CStr2(pDR.Category).trim() != '')
                                        pDtCategoryPrinter = Business.TransInvoice.GetPrinters(sessionStorage.FowstID, pDR.Category);
                                    proj.SalesMenu.PrintBilling(pInvoiceID, $.extend(pOptions, {
                                        Category: pDR.Category,
                                        DtPrinter: pDtCategoryPrinter
                                    }));
                                }
                                */
                                if (!CBool(pOptions.PrintAsCancellationSlip)) {
                                    var pSql = 'SELECT DefaultValue FROM private.SystemDefaults WHERE Description = \'PrintDispatchSlip\'';

                                    var pResult = CBool(Sql.ExecuteScalar(pSql));

                                    if (pResult) {
                                        PrintDispatchSlip(pInvoiceID, {
                                            ByCategory: false,
                                            DtPrinter: null
                                        });
                                    }
                                }

                                if (!pDontReturn)
                                    return;
                            }

                            if (CDec2(GetStringObject('DtPrinter.Rows.length', pOptions)) == 0)

                                vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterType();
                            else
                                vDataPrint = pOptions.DtPrinter;

                            //Get Header
                            vDataHeader = SalesPayment_offline().getReceiptHeaderInfo(pInvoiceID);
                            //Get Invoice Header
                            vDataInvHeader = SalesPayment_offline().getSalesPaymentInvoiceHeaderInfo(pInvoiceID);
                            //Get Invoice Items
                            vDataItem = SalesPayment_offline().getSalesPaymentReceiptBodyInfo(pInvoiceID, pOptions);
                            if (vDataItem.Rows.length == 0)
                                return;
                            //Get Invoice Items Serial
                            vDataItemSerial = SalesPayment_offline().getSalesPaymentReceiptSerial(pInvoiceID);
                            //Get Footer Amounts
                            vDataFooter = SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(pInvoiceID, pOptions);
                            //Get Footer Messages
                            vDataMsg = SalesPayment_offline().getSalesPaymentReceiptFooterMessagesInfo();

                            //ReceiptSettings
                            if (vDataHeader == undefined) {
                                NewMessageBox("No receipt settings created for this FOWST. Please contact your system administrator for assistance.",
                                        "Receipt Settings",
                                        [
                                            {
                                                Text: 'Ok'
                                            }
                                        ]
                                );
                                return;
                            } else {
                                if (vDataHeader.Rows.length <= 0) {
                                    NewMessageBox("No receipt settings created for this FOWST. Please contact your system administrator for assistance.",
                                            "Receipt Settings",
                                            [
                                                {
                                                    Text: 'Ok'
                                                }
                                            ]
                                    );
                                    return;
                                }
                            }



                            if (IsPreviewBill) {
                                //$('#popupPayNowMode').popup('close');
                                $.mobile.changePage('#PreviewBill');

                                //                            if (sessionStorage.IsPaymentJSLoaded != 'true') {
                                //                                require(['/Scripts/sources/SalesPayments/SalesPayments.js?crc=']);

                                //                            }

                                PreviewBill(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg);
                                ShowCharges();
                                window.pageDiscounts.getCurrentDiscount();

                                IsPreviewBill = false;


                            }
                            else {

                                //PrintBillingWPSNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, pOptions);

                                //PrintBillingNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, pOptions);

                                if (window.InAndroid) { // Android Mobile, query to offline

                                    //check if this device is using ami duos
                                    var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                                    if (vDataPrinterType.Rows.length > 0) {
                                        if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                            PrintBillingNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, pOptions);
                                        } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                        } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                            PrintBillingWPSNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, pOptions, vDataPrinterType);
                                        } else {
                                            PrintBillingNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, pOptions);
                                        }
                                    } else {
                                        PrintBillingNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, pOptions);
                                    }


                                } else { //Mobile (window), query to offline
                                    var japplet = document.jzebra;
                                    var blnIsPrinterFound = false;

                                    if (japplet != null) {
                                        while (!blnIsPrinterFound) {

                                            if (FindDesktopPrinter("")) {
                                                blnIsPrinterFound = true;

                                                PrintBillingNow(pInvoiceID, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);

                                            };

                                        }
                                    } else {
                                        NewMessageBox("Jzebra applet was not loaded.",
                            "Jzebra",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                        );
                                    }
                                }
                            } // end if preview
                        } else {
                            AjaxSend({
                                url: "Organization/getOrgFOWSTPrinterIP",
                                success: function (vRetDataPrint) {
                                    vDataPrint = vRetDataPrint.OrganizationFOWSTData;

                                    //Get Header
                                    AjaxSend({
                                        url: "SalesPayments/getReceiptHeaderInfo",
                                        data: pData,
                                        success: function (vRetDataHeader) {
                                            vDataHeader = vRetDataHeader.SalesPaymentData;

                                            //Get Invoice Header
                                            AjaxSend({
                                                url: "SalesPayments/getSalesPaymentInvoiceHeaderInfo",
                                                data: pData,
                                                success: function (vRetDataInvHeader) {
                                                    vDataInvHeader = vRetDataInvHeader.SalesPaymentData;

                                                    //Get Invoice Items
                                                    AjaxSend({
                                                        url: "SalesPayments/getSalesPaymentReceiptBodyInfo",
                                                        data: pData,
                                                        success: function (vRetDataItem) {
                                                            vDataItem = vRetDataItem.SalesPaymentData;

                                                            //Get Invoice Items Serial
                                                            AjaxSend({
                                                                url: "SalesPayments/getSalesPaymentReceiptSerial",
                                                                data: pData,
                                                                success: function (vRetDataItemSerial) {
                                                                    vDataItemSerial = vRetDataItemSerial.SalesPaymentData;

                                                                    //Get Payment Info
                                                                    AjaxSend({
                                                                        url: "SalesPayments/getSalesPaymentReceiptPaymentInfo",
                                                                        data: pData,
                                                                        success: function (vRetDataPayment) {
                                                                            vDataPayment = vRetDataPayment.SalesPaymentData;

                                                                            //Get Footer Amounts
                                                                            AjaxSend({
                                                                                url: "SalesPayments/getSalesPaymentReceiptFooterAmountsInfo",
                                                                                data: pData,
                                                                                success: function (vRetDataFooter) {
                                                                                    vDataFooter = vRetDataFooter.SalesPaymentData;

                                                                                    //Get Footer Messages
                                                                                    AjaxSend({
                                                                                        url: "SalesPayments/getSalesPaymentReceiptFooterMessagesInfo",
                                                                                        data: pData,
                                                                                        success: function (vRetDataMsg) {
                                                                                            vDataMsg = vRetDataMsg.SalesPaymentData;

                                                                                            if (IsPreviewBill) {
                                                                                                PreviewBill(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, strFrom, strTo, strYear);
                                                                                                IsPreviewBill = false;
                                                                                            }
                                                                                            else {
                                                                                                if (window.InAndroid) { //Android Mobile Or Mobile (window)
                                                                                                    var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                                                                                                    if (vDataPrinterType.Rows.length > 0) {
                                                                                                        if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                                                                                            PrintBillingNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, vOptions);
                                                                                                        } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                                                                                        } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                                                                                            PrintBillingWPSNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, vOptions);
                                                                                                        } else {
                                                                                                            PrintBillingNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, vOptions);
                                                                                                        }
                                                                                                    } else {
                                                                                                        PrintBillingNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, vOptions);
                                                                                                    }
                                                                                                    //PrintBillingNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vRetDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                                                                                } else { //Web App
                                                                                                    var japplet = document.jzebra;
                                                                                                    var blnIsPrinterFound = false;

                                                                                                    if (japplet != null) {
                                                                                                        while (!blnIsPrinterFound) {

                                                                                                            if (FindDesktopPrinter("")) {
                                                                                                                blnIsPrinterFound = true;

                                                                                                                PrintBillingNow(pInvoiceID, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);

                                                                                                            };

                                                                                                        }
                                                                                                    } else {
                                                                                                        NewMessageBox("Jzebra applet was not loaded.",
                                                                                            "Jzebra",
                                                                                            [
                                                                                                {
                                                                                                    Text: 'Ok'
                                                                                                }
                                                                                            ]
                                                                                        );
                                                                                                    }
                                                                                                }

                                                                                            } //end if preview


                                                                                        }
                                                                                    });

                                                                                }
                                                                            });
                                                                        }
                                                                    });

                                                                }
                                                            });
                                                        }
                                                    });
                                                }
                                            });

                                        }
                                    });

                                }
                            });
                        }
                    }; // END proj.SalesMenu.PrintBilling = function (pInvoiceID, vOptions) {

                    sessionStorage.removeItem('IsHotButtonVisited');
                    if (window.location.pathname.toLowerCase() == '/salesmenu/bsindex') {

                        sessionStorage.PosMode = 'Retail Mode';

                        //setTimeout(function () {
                        //    var pPage = $.mobile.activePage;
                        //    var pHeader = pPage.find("[data-role=header]");
                        //    var pHeight = pHeader.height() + 10;

                        //    pPage.css('padding-top', pHeight + 'px');

                        //    if ($.isFunction(pHeader.fixedtoolbar)) // jqm 1.4.4 doesn't have fixedtoolbar function
                        //        pHeader.fixedtoolbar({ tapToggle: false,
                        //            updatePagePadding: false    // http://jquerymobile.com/test/docs/toolbars/bars-fixed-options.html
                        //            // http://stackoverflow.com/questions/10545540/jquery-mobile-disable-tap-to-toggle-fixed-header-and-footer
                        //        });
                        //    App.Deferred.fulfill('R87MBPXGSQ');
                        //}, 100);
                    } else {
                        sessionStorage.PosMode = 'Widescreen';
                    }

                    if (GetUrlParameters().HotButtons == '1') {
                        sessionStorage.PosMode = 'Hot Buttons Mode';
                    }

                    if (window.IsMobile) {
                        Business.TransInvoice.FixNoCustomers();
                        Business.TransInvoice.FixNoItemCount();
                        mTableView = new App.model.TableView();
                        if (!Business.LibraryOrganization.CanTestPrice())
                            $('#divTestPriceRequest').remove();
                        if (Business.SystemDefaults.HideBarcodeScan())
                            $('.divHideBarcodeScan').remove();
                        if (Business.SystemDefaults.HideHotButtonsButton())
                            $('.divHideHotButtonsButton').remove();
                    }

                    var pParams = $.extend(GetUrlParameters(), vSalesMenuInitParams);

                    proj.PosMode = (pParams.mode == 'pos');
                    proj.ReconstructSalesDataMode = (pParams.rsd == '1');
                    proj.POMode = (pParams.mode == 'po'); //additional by janine june 23 2015 purchase order mode
                    proj.SOMode = (pParams.mode == 'so');

                    //Check if there was a CartOrder on the Retail Mode
                    //If there was a CartOrder prior to this, load the items
                    if (proj.PosMode) {
                        if (CStr2(pParams.TableNo) != '') {
                            $('#iconHome').removeClass('bi-house-fill').addClass('bi-arrow-left-circle');
                            $('#lblHome').text('Back to Table View');
                            $('#spanMoreOptions').text('Print Options');
                            //added by francis dec 03 2014
                            var mData = GetInvoiceDetails({
                                TableNo: vSalesMenuInitParams.TableNo
                            });

                            if (mData.Rows.length > 0) {

                                sessionStorage.IsDraft = true; //added by francis 05/31/16 para maload ang items sa cart bcase once IsDraft = undefined, mahide ang items sa draft
                                e = mData.Rows[0];

                                sessionStorage.setItem('CartOrderID', e.ID);

                                //Added By Francis to load Customer Info
                                sessionStorage.CustomerOrganizationID = e.CustomerID;
                                sessionStorage.CustomerOrganizationName = e.CompanyName;

                                //additional by janine oct 27 for demographics
                                sessionStorage.DeliveryMethod = e.DeliveryMethod;
                                sessionStorage.OrderMethod = e.OrderMethod;
                                sessionStorage.Gender = e.CustomerDemographicGender;
                                sessionStorage.Age = e.CustomerDemographicAge;
                                sessionStorage.TotalNumberOfCustomer = e.TotalNumberOfCustomer;
                                sessionStorage.FowstID = e.FowstID;
                                SalesMenu_offline().HandleEnforceZRead({
                                    callback: function (vResult) {
                                        //put condition if table no or gas pump no if GP or TBL
                                        if (!vResult)
                                            return;
                                        if (CStr2(e.TableNo) != '') {
                                            var prefix = e.TableNo.split('_');
                                            if (prefix[0] == 'GP')
                                                sessionStorage.GasPumpNo = prefix[1];
                                            else
                                                sessionStorage.CustomerTableNo = prefix[1];

                                        } else {
                                            sessionStorage.GasPumpNo = '';
                                            sessionStorage.CustomerTableNo = '';
                                        }
                                        //                            pageCustomerDetails.setCustomerInfo();
                                        $('#btnClient').data('id', e.CustomerID);

                                        //additional by janine sept 5 2014 
                                        if (proj.PosMode) { // && CDec2(e.SalesOrderNo) > 0
                                            UpdateOrderedQuantity();
                                        } else {
                                            isRecallCart = true;
                                            LoadCart();

                                            //GotoFirstPage();
                                        }
                                        //end of additional
                                        window.pageCustomerDetails.SetPOSMode(false);

                                        // commented by Arvin 2016_10_13__16_39_58_0843
                                        //if (CBool(Business.SystemDefaults.IsEnableBillingOption()))
                                        //    $('#popupPayNowMode').popup('open');

                                        if (CStr2(sessionStorage.CustomerTableNo) != '')
                                            DisplayTableNumberOnButton();
                                        else
                                            $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                                    } // END callback: function() {
                                }); // END SalesMenu_offline().HandleEnforceZRead({

                                // uncommented on 2017_03_21__16_13_49_7824
                                //return; //para dili maempty ang cart once nagrecall // comment during debugging 2016_09_27__10_12_00_6542
                            } else {
                                //additional by janine apr 14 2015. if coming from table view to hot buttons
                                if (CStr2(pParams.TableView) == 'true') {
                                    sessionStorage.CustomerTableNo = pParams.TableNo;

                                    if ($(window).width() > proj.WideWidth) { // 932 old . force non-widescreen
                                        //location = MergeUrlParameters({
                                        //    OrgID: sessionStorage["OrgID"],
                                        //    mode: 'pos',
                                        //    IsMobile: IsMobile ? 1 : 0
                                        //}, '/SalesMenu/BsPOSViewAll');
                                    } else {
                                        goToHotButtonPage();
                                    }
                                }

                                NewCart();
                                sessionStorage.CustomerTableNo = vSalesMenuInitParams.TableNo;
                                if (CStr2(sessionStorage.CustomerTableNo) != '')
                                    DisplayTableNumberOnButton();
                                else
                                    $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);

                            }



                            function GetInvoiceDetails(vParams) {
                                vParams = $.extend({
                                    TableNo: GetUrlParameters().TableNo
                                }, vParams);
                                var vData = [];
                                vData.keywords = '';
                                var pFinder = OfflineBrowser.Base().CreateFinder(vData);
                                var pFields = pFinder.Fields;
                                var pField;
                                var pNonZero;

                                pNonZero = vData.NonZero;

                                pField = pFields.Add('ID');
                                pField.Expression = 'TransInvoice.ID';
                                pField.Searchable = false;

                                pField = pFields.Add('CustomerID');
                                pField.Expression = 'TransInvoice.CustomerID';
                                pField.Searchable = false;

                                pField = pFields.Add('InvoiceNum');
                                pField.Expression = 'TransInvoice.InvoiceNumber';

                                pField = pFields.Add('CompanyName');
                                pField.Expression = " (CASE " +
                        "     WHEN LibraryOrganization.IsOrganization = 1 THEN LibraryOrganization.CompanyName " +
                        "     WHEN LibraryOrganization.IsOrganization = 0 THEN " +
                        "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonSurname) || ', ',', '), '') || " +
                        "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonFirstName) || ' ',' '), '') || " +
                        "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonMiddleName) || ' ',' '), '') || " +
                        "         IFNULL(NULLIF(', ' || TRIM(LibraryOrganization.PersonSuffixName),', '), '') " +
                        " END)"

                                pField = pFields.Add('InvoiceDate');
                                pField.Expression = 'TransInvoice.InvoiceDate';

                                pField = pFields.Add('Total');
                                pField.Expression = '(SELECT CAST(SUM(SubTotal) as TEXT) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)';
                                pField.Searchable = false;

                                pField = pFields.Add('SkuCount');
                                pField.Expression = '(SELECT COUNT(ID) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)';
                                pField.Searchable = false;

                                //additional by janine sept 5 2014 for sales order taker
                                pField = pFields.Add('TotalOrder');
                                pField.Expression = '(SELECT CAST(SUM(SubTotal) as TEXT) FROM TransInvoiceItems WHERE OrderedQuantity > 0 AND InvoiceID = TransInvoice.ID)';
                                pField.Searchable = false;

                                pField = pFields.Add('SkuCountOrder');
                                pField.Expression = '(SELECT COUNT(ID) FROM TransInvoiceItems WHERE OrderedQuantity > 0 AND InvoiceID = TransInvoice.ID)';
                                pField.Searchable = false;
                                ///////////////
                                pField = pFields.Add('SalesOrderNo');
                                pField.Expression = 'IFNULL(SalesOrderNo,0)';

                                pField = pFields.Add('UserID');
                                pField.Expression = 'TransInvoice.UserID';
                                pField.Searchable = false;

                                pField = pFields.Add('EmployeeFullName');
                                pField.Expression = 'LibraryPeople.Surname || \',\' || LibraryPeople.FirstName';

                                pField = pFields.Add('FowstID');
                                pField.Expression = 'LibraryOrganizationFOWST.ID';
                                pField = pFields.Add('FOWSTName');
                                pField.Expression = 'LibraryOrganizationFOWST.Name';

                                pField = pFields.Add('DeviceName');
                                pField.Expression = 'LibraryDevice.PhoneName';

                                pField = pFields.Add('CurrentUserID');
                                pField.Expression = "'" + Web.PeopleUserID() + "'";
                                pField.Searchable = false;

                                pField = pFields.Add('MIN');
                                pField.Expression = 'LibraryOrganizationFOWST.FOWSTNo || \'-\' || LibraryDevice.DeviceNo';

                                pField = pFields.Add('TableNo');
                                pField.Expression = 'TransInvoice.CustomerTableNo';

                                pField = pFields.Add('DeliveryMethod');
                                pField.Expression = 'TransInvoice.DeliveryMethod';

                                pField = pFields.Add('OrderMethod');
                                pField.Expression = 'TransInvoice.OrderMethod';

                                pField = pFields.Add('CustomerDemographicGender');
                                pField.Expression = 'TransInvoice.CustomerDemographicGender';

                                pField = pFields.Add('CustomerDemographicAge');
                                pField.Expression = 'TransInvoice.CustomerDemographicAge';

                                pField = pFields.Add('TotalNumberOfCustomer');
                                pField.Expression = 'TransInvoice.TotalNumberOfCustomer';

                                pField = pFields.Add('SearchSO');
                                pField.Expression = " LibraryOrganizationFOWST.FOWSTNo || \'-\' || LibraryDevice.DeviceNo || \'-\' || IFNULL(SalesOrderNo,0) ";

                                pField = pFields.Add('SalesOrderNoRemoveDash');
                                pField.Expression = " LibraryOrganizationFOWST.FOWSTNo ||  LibraryDevice.DeviceNo ||  IFNULL(SalesOrderNo,0) ";

                                //            pFinder.SqlFrom = 'FROM TransInvoice' +
                                //                ' JOIN LibraryOrganization ON TransInvoice.CustomerID = LibraryOrganization.ID';
                                pFinder.SqlFrom = ' FROM TransInvoice \n' +
                                    ' LEFT JOIN LibraryPeople ON TransInvoice.UserID = LibraryPeople.ID\n' +
                                    ' LEFT JOIN' +
                                    ' LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID INNER JOIN' +
                                    ' LibraryOrganizationFOWST ON TransInvoice.FowstID = LibraryOrganizationFOWST.ID' +
                                    ' JOIN LibraryOrganization ON TransInvoice.CustomerID = LibraryOrganization.ID ' +
                                    ' LEFT JOIN TransCustomerPaymentInvoices on TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID';
                                pFinder.SqlWhere = 'WHERE TransInvoice.IsCancelled = 0' +
                                    ' AND TransInvoice.CustomerTableNo = ' + ReadyString('TBL_' + vParams.TableNo) +
                                    ' AND TransInvoice.InvoiceNumber ' + (CBool(pNonZero) ? '<>' : '=') + ' 0' +
                                    ' AND EXISTS(SELECT TransInvoiceItems.ID' +
                                    '   FROM TransInvoiceItems' +
                                    '   WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID)' +
                                    ' AND NOT EXISTS(SELECT TransInvoice.ID' +
                                    '   FROM TransInvoice' +
                                    '   WHERE TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID)';

                                pFinder.SqlOrder = ' ORDER BY TransInvoice.LastUpdated DESC';

                                var pDT = pFinder.Requery();
                                for (var p = 0; p <= pDT.Rows.length - 1; p++) {
                                    var pDR = pDT.Rows[p];
                                    if (pDR.EmployeeFullName == null) {
                                        var pMessage = Math.max(CDec2(pDR.SkuCount), CDec2(pDR.SkuCountOrder));
                                        pMessage = 'This cart/table has ' + pMessage + ' items but cannot be shown because PeopleID ' + pDR.UserID + ' does not exist in LibraryPeople. ref: jsSKKV8WQEXC!';
                                        NewMessageBox(pMessage);
                                        Android.AddLog(pMessage);
                                        pDT.Rows = [];
                                        break;
                                    }
                                }
                                return pDT;
                            }

                        }
                    }
                    if (proj.PosMode) {
                        $('#iconPay').css('display', 'initial'); // para magpantay ang alignment ng text sa Menu bar
                        $('#iconSave').hide();
                        if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall' || window.location.pathname.toLowerCase() == '/tableview/bsnewtable')
                            $('#lblPayNow').html('Go');
                        else {
                            if ($.isFunction(GetStringObject('GetIsOrderTaker', mOrderTakerSetupModel)))
                                if (mOrderTakerSetupModel.GetIsOrderTaker())
                                    $('#lblPayNow').html('Go');
                                else
                                    $('#lblPayNow').html('Pay Now');
                            else
                                $('#lblPayNow').html('Pay Now');
                        }
                        $('#IsPOSHeader').html('<img alt="POS Mode" src="/Content/images/View_Mode.png" width="30px" align="center" /><br/>');
                        $('#IsPOS').html('POS Mode');
                    } else {
                        $('#iconPay').hide();
                        $('#iconSave').css('display', 'initial');
                        if (proj.POMode) {
                            $('#lblPayNow').html('Save Purchase Order');
                            // $('#IsPOSHeader').html('Purchase Order Mode: <br/>');
                            $('#IsPOSHeader').html('<img alt="POS Mode" src="/Content/images/Sales_Rep.png" width="30px" align="center" /><br/>');
                            $('#IsPOS').html('Purchase Order Mode');
                            // $('#btnFowst, #butRecentlyBought, #butReprintReceipt, #butSalesRepresentatives').remove();
                            $('#btnFowst, #butReprintReceipt, #butSalesRepresentatives').remove();
                            $('#butNewSalesOrder h3').text('New PO');
                            $('#butNewSalesOrder p').text('Create a new PO. Any unsaved POs will be saved as draft and can be retrieved and edited later.');
                            $('#lnkSalesOrder h3').text('Retrieve Previous PO');
                            $('#lnkSalesOrder p').text('Retrieve previous POs. Only draft POs may be edited. Finalized POs may be viewed but not edited.');

                            $('#butRecentlyBought h3').text('Copy items from existing PO to a new PO');
                            $('#butRecentlyBought p').text('Copy items from existing PO to a new PO');
                        } else {
                            $('#lblPayNow').html('Save Sales Order');
                            $('#IsPOSHeader').html('Sales Order Mode: <br/>');
                            $('#IsPOS').html('Sales Order Mode');
                        }
                        //GetInventoryCountBehavior();
                    }


                    //additional by janine jun 23 2015 for purchase order


                    // see /Scripts3/common/common.js
                    startclock('#clock');

                    ////////////////////                    if (proj.PosMode) {
                    $('#lnkSalesOrder [name=divNumber]').html('Recall Cart &#x25BC;'); //Show Previous Un-Checked Out Invoices. edited by Janine Oct 17 8am
                    //$('#lnkCheckout div').text('Check Out This Invoice');
                    $('#lnkHold').hide(); // .text('Hold This Invoice');

                    if (proj.ReconstructSalesDataMode) {
                        $('[name="divReconstructSalesData"]').show();
                        $('[name="hRsd"]').show();

                        $('[name="btnExitRSD"]').unbind('click').click(function () {
                            var param = GetUrlParameters();
                            param.rsd = 0;
                            location = MergeUrlParameters(param, location.origin + location.pathname);
                        })
                    }; // proj.ReconstructSalesDataMode
                    
                    ////////////////////                    } else {
                    ////////////////////                        $('#lnkSalesOrder [name=divNumber]').html('Recall Cart &#x25BC;');
                    ////////////////////                        $('#lnkCheckout h3').text('Check Out This Order');
                    ////////////////////                        $('#lnkHold h3').text('Hold This Order');
                    ////////////////////                    };

                    //$('#divRecentlyBoughtSection').hide();
                    $('#divCartSection').hide();
                    $('#divPosBranch').hide();
                    //edited by janine
                    //                    if (!IsMobile)
                    //                        $('#butBarCode').closest('.ui-btn').hide();

                    App.Deferred.done(function () {
                        console.log('App.Deferred.done ' + this.getID());
                    });
                    App.Deferred.expect([
            'R87MBPXGSQ',
            'KWXCETE5XN',
            'FQ26E5TP76',
            'BPWQQX7KGV'
                    ]);

                    $('#btnBOMFowst').click(function (e) {
                        e.preventDefault();

                        mPrePos.BrowseAddresses();
                        return false;
                    });

                    $('#txtCheckerBarcode').keydown(function (e) {
                        if (e.keyCode == 13) {
                            if (OfflineMode) {
                                var pSql = 'Select * from private.LibraryOrganizationEmployee where EmployeeBarcode = ' + ReadyString($('#txtCheckerBarcode').val());

                                var pDT = Sql.Execute(pSql);

                                if (pDT.Rows.length > 0) {
                                    sessionStorage.CherckerUserID = pDT.Rows[0].ID;
                                    history.back();
                                }
                                else {
                                    $('#spanError').css('visibility', 'visible');
                                    $('#spanError').css('display', 'block');
                                }
                            }
                        }
                    });

                    //EDITED BY Janine
                    $('#lnkLogout').attr('href', GetPathFormat('/SignIn?mode=logout')); //GetPathFormat('/people?mode=logout')

                    // start tab
                    // http://stackoverflow.com/a/6850059/334045
                    $(document).delegate('.ui-navbar ul li > a', 'click', function () {

                        //un-highlight and highlight only the buttons in the same navbar widget
                        $(this).closest('.ui-navbar').find('a').removeClass('ui-navbar-btn-active');

                        //this bit is the same, you could chain it off of the last call by using two `.end()`s
                        $(this).addClass('ui-navbar-btn-active');

                        //this starts the same but then only selects the sibling `.content_div` elements to hide rather than all in the DOM
                        $('#' + $(this).attr('data-href')).show().siblings('.content_div').hide();
                    });
                    $('.ui-navbar ul li > a:eq(0)').click();
                    // end tab

                    if (OfflineMode) {
                        //$('#butRecentlyBoughtAddAll')
                        //    .show()
                        //    .click(function () {
                        //        SalesMenu_offline().HandleAddMultiToCart(mLvRecentlyBought);
                        //    });
                        Browser.OfflineMode = true;
                        $('#butHotButtons').closest('.ui-btn').hide(); // not yet implemented in offline mode
                    } else {
                        $('#butHotButtons').click(function () {

                            ChangePage('/SalesMenu/HotButtons');

                        }).show();
                    }; // END if (!OfflineMode) {

                    if (proj.POMode) {
                        $('#lblCustomerName, #lblSupplierName').text('Supplier');
                        $('#lblCurrentPOSMode').text('Customer');
                        DisplayPoSupplierAndCustomer();
                    }

                    if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall' || window.location.pathname.toLowerCase() == '/tableview/bsnewtable') {
                        if (sessionStorage.getItem('CartOrderID')
                            && (CStr2(pParams.TableNo) == '')) {
                            //Load items that are previously on retail mode
                            LoadCart();
                            //If the sidecart is in Purchase Order Mode, load the supplier info
                            if (!(CBool(proj.PosMode))
                                && !(CBool(proj.SOMode))) {
                                // condition modified by Arvin 2016_08_18__14_10_54_4355
                                // old code:
                                // if (!(CBool(proj.PosMode) && CBool(proj.SOMode))) {
                                //Load the supplier information if sidecart is in
                                //Purchase Mode
                                sessionStorage.SupplierOrganizationID = localStorage.getItem('tempSupplierOrganizationID');
                                sessionStorage.SupplierOrganizationName = localStorage.getItem('tempSupplierOrganizationName');
                                sessionStorage.SupplierAddress = localStorage.getItem('tempSupplierAddress');
                                sessionStorage.SupplierPriceLevelID = localStorage.getItem('tempSupplierPriceLevelID');
                                sessionStorage.SupplierPriceLevelName = localStorage.getItem('tempSupplierPriceLevelName');
                                sessionStorage.BillingAddressID = localStorage.getItem('tempBillingAddressID');
                                sessionStorage.DeliveryAddressID = localStorage.getItem('tempDeliveryAddressID');
                                sessionStorage.DeliveryDate = localStorage.getItem('tempDeliveryDate');

                                //Always remember GIGO, Garbage In and Garbage Out
                                //Remove templates after use
                                localStorage.removeItem("tempSupplierOrganizationID");
                                localStorage.removeItem("tempSupplierOrganizationName");
                                localStorage.removeItem("tempSupplierAddress");
                                localStorage.removeItem("tempSupplierPriceLevelID");
                                localStorage.removeItem("tempSupplierPriceLevelName");
                                localStorage.removeItem("tempBillingAddressID");
                                localStorage.removeItem("tempDeliveryAddressID");
                                localStorage.removeItem("tempDeliveryDate");
                                $('#lblSupplierName, #lblCustomerName, #lblSupplierNameHotButtons').text('Supplier: ' + sessionStorage.SupplierOrganizationName)
                                    .data('id', sessionStorage.SupplierOrganizationName);
                                $('#lblCurrentPOSMode, #lblCurrentPOSModeHotButtons').text('Customer: ' + sessionStorage.FowstName);
                            }
                        } else { // ELSE if (sessionStorage.getItem('CartOrderID')) {
                            if (!GetUser())
                                return;
                        } // END if (sessionStorage.getItem('CartOrderID')) {
                    } else { // ELSE if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {
                        if (!GetUser())
                            return;
                    } // END if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {

                    mOrderReceiver = new App.controller.OrderReceiver();
                    mOrderReceiver.AddSaveListener(function (vData) {
                        var pShouldReload;

                        if (IsGuid(mID)) {
                            for (var p = 0; p <= vData.length - 1; p++) {
                                if (ReadyGuid(mID) == ReadyGuid(vData[p].ID)) {
                                    pShouldReload = true;
                                    break;
                                }
                            }

                            if (pShouldReload)
                                if (sessionStorage.CustomerOrganizationID == null)
                                    pShouldReload = false;
                            if (pShouldReload)
                                LoadCart(false, {
                                    ID: mID
                                });
                        }
                    });
                    // disable order transmitter in cart 2018_05_11__11_37_41_9260
                    //proj.OrderTransmitter = new App.controller.OrderTransmitter();
                    //proj.OrderTransmitter.Start();

                    if (window.DebugMode) {
                        Business.TransInvoice.CreateSalesOrderPdf('3fdc2434-ed53-4853-9695-a76870cc13f7',
                            {
                                IsPurchaseOrder: true
                            }
                        );
                        //mPeerConnect = new App.controller.PeerConnect();
                        //setTimeout(mPeerConnect.popup, 5000);

                    } // END if (window.DebugMode) {
                };
                proj.SalesMenuInit();

                //DO NOT SHOW WELCOME MSG FOR NEW UI
                //if (!Android.DebugMode()) {
                //    NewMessageBox('This new User Inteface (UI) is still under development, ' +
                //        'if you encounter some errors or if you do not find the feature that you are looking for, ' +
                //        'you can switch to the old UI anytime, just click the More Options at the top. Thank you!', 'Welcome to new UI')
                //}

                //added by arnie on 7-9-2020 9:34 AM
                lightbox.option({
                    'resizeDuration': 200,
                    'wrapAround': true,
                    'imageFadeDuration': 300,
                    'alwaysShowNavOnTouchDevices': true
                })

                function DisplayPoSupplierAndCustomer() {
                    var p = SalesMenu_offline().PoDefaultSupplierAndCustomer();

                    $('#lblSupplierName, #lblCustomerName, #lblSupplierNameHotButtons')
                        .text('Supplier: ' + p.SupplierOrgName + ' - ' + p.SupplierFowstName);
                    sessionStorage.SupplierOrganizationID = p.SupplierOrgID;
                    sessionStorage.SupplierFowstID = p.SupplierFowstID;

                    $('#lblCurrentPOSMode, #lblCurrentPOSModeHotButtons')
                        .text('Customer: ' + p.CustomerOrgName + ' - ' + p.CustomerFowstName);
                    sessionStorage.CustomerOrganizationID = p.CustomerOrgID;
                    sessionStorage.CustomerFowstID = p.CustomerFowstID;

                } // END  DisplayPoSupplierAndCustomer

                function GetInventoryCountBehavior() {
                    var pSql = '';

                    if (OfflineMode) {
                        pSql = 'Select * from private.SystemDefaults where Description = \'IsEnableInventoryCountMode\''

                        var pDT = Sql.Execute(pSql);

                        if (pDT.Rows.length > 0) {
                            if (parseInt(pDT.Rows[0].DefaultValue)) {
                                sessionStorage.IsInventoryCountMode = true;
                            }
                            else {
                                sessionStorage.IsInventoryCountMode = false;
                            }
                        }
                        else {
                            sessionStorage.IsInventoryCountMode = false;
                        }
                    }
                    else {
                        AjaxSend({
                            url: 'SystemBehaviorSettings/GetSalesOrderBehavior',
                            success: function (vData) {
                                if (vData.Results.Rows.length > 0) {
                                    if (parseInt(vData.Results.Rows[0].DefaultValue)) {
                                        sessionStorage.IsInventoryCountMode = true;
                                    }
                                    else {
                                        sessionStorage.IsInventoryCountMode = false;
                                    }
                                }
                                else {
                                    sessionStorage.IsInventoryCountMode = false;
                                }
                            }
                        });
                    }
                }

                function gotoRentableAssetsView() {
                    sessionStorage.PosMode = 'Rentable Assets VIew';
                    proj.ProductID = '';
                    $.mobile.changePage('');
                    location = MergeUrlParameters({
                        OrgID: sessionStorage["OrgID"],
                        mode: 'pos',
                        IsMobile: IsMobile ? 1 : 0
                    }, '/SalesMenu/RentableAssetView');
                }

                function goToTableViewPage() {
                    sessionStorage.PosMode = 'Table View Mode';
                    proj.ProductID = '';
                    $.mobile.changePage('');
                    location = MergeUrlParameters({
                        OrgID: sessionStorage["OrgID"],
                        mode: 'pos',
                        IsMobile: IsMobile ? 1 : 0,
                        theme: 'bs'
                    }, '/TableView/BsNewTable');

                    //                    ChangePage('/SalesMenu/TableView');
                }

                // commented by Arvin on 2017_02_02__15_06_44_8237
                //function goToHotButtonPage() {
                //    sessionStorage.PosMode = 'Hot Button Mode';
                //    proj.ProductID = '';

                //    $('#txtOrder').html('');
                //    $('#txtQtyUnit').html('');
                //    startclock('#clockHotButtons');
                //    proj.CartStock = [];
                //    ChangePage('/SalesMenu/HotButtons');

                //    //session storage cleared on peoplemenu js

                //    if (sessionStorage.IsHotButtonVisited == 'true') {
                //        $('#divSpace').hide();
                //        //                        $.mobile.changePage('#pageButtons');
                //        //                                        GetData(sessionStorage.HotButtonMenuID);
                //        $('#divSpace').css('height', '0px');
                //    }

                //    if (CStr2(sessionStorage.CustomerTableNo) != '') {

                //        DisplayTableNumberOnButton();
                //        try {

                //            $('#selTableNoHotButtons').val(sessionStorage.CustomerTableNo).selectmenu('refresh', true);
                //        } catch (e) {

                //            $('#selTableNoHotButtons').val(sessionStorage.CustomerTableNo);
                //            $('#selTableNoHotButtons').selectmenu();

                //        }

                //        $('#lblLabelHotButtons').html('Customer / Table No.:<br/>');

                //    }

                //    if (CStr2(sessionStorage.GasPumpNo) != '') {

                //        $('#lblCustomerNameHotButtons').text(sessionStorage.GasPumpNo);

                //        try {
                //            $('#selGasPumpNoHotButtons').val(sessionStorage.GasPumpNo).selectmenu('refresh', true);
                //        } catch (e) {
                //            $('#selGasPumpNoHotButtons').val(sessionStorage.GasPumpNo);
                //            $('#selGasPumpNoHotButtons').selectmenu();

                //        }

                //        $('#lblLabelHotButtons').html('Gas Pump No.:<br/>');
                //    }

                //    if (sessionStorage.GasPumpNo == '' && sessionStorage.CustomerTableNo == '') {

                //        $('#lblCustomerNameHotButtons').text(sessionStorage.CustomerOrganizationName);

                //        try {
                //            $('#selTableNoHotButtons').val('').selectmenu('refresh', true);

                //        } catch (e) {
                //            $('#selTableNoHotButtons').val('');
                //            $('#selTableNoHotButtons').selectmenu();

                //        }
                //        $('#lblLabelHotButtons').html('Customer:<br/>');

                //    }
                //} // end of gotohotbuttonpage


                function GetUser() {

                    return mPrePos.GetUser();

                    var pUser;
                    if (OfflineMode) {
                        pUser = SalesMenu_offline().GetUser();
                        if ((pUser != undefined)
                            && (pUser.FowstID == undefined)) {
                            if (pUser.FowstCount == undefined) {
                                //Edited By: Francis
                                //Date: June 24, 2014
                                //                                NewMessageBox('The device you are currently using is not registered to any FOWST in the current organization.', null,

                                //                                        [
                                //                                            { Text: 'Update',
                                //                                                Click: function () {
                                //                                                    GetDeviceInformation();
                                //                                                }
                                //                                            },
                                //                                            { Text: 'Cancel',
                                //                                                Click: function () {
                                //                                                    var pParam = {};

                                //                                                    if (IsMobile)
                                //                                                        pParam.IsMobile = 1;
                                //                                                    pParam = $.param(pParam);
                                //                                                    window.location = "/People/PeopleMenu?" + pParam;
                                //                                                }
                                //                                            }
                                //                                        ]
                                //                                ); // NewMessageBox

                                NewMessageBox('You and this device must be registered to a common FOWST.', null, null, {
                                    OnClose: function () {
                                        var pParam = {};
                                        if (IsMobile)
                                            pParam.IsMobile = 1;
                                        pParam = $.param(pParam);
                                        window.location = "/People/PeopleMenu?" + pParam;
                                    } // OnClose
                                }); // NewMessageBox

                                return false;
                            }; // if (pUser.FowstCount == undefined) {
                        }; // if (pUser.FowstID == undefined) {
                        LoadUser(pUser);
                        SalesMenu_offline().UpdateCounterFromOnline();
                    } else { // if (OfflineMode) {
                        AjaxSend({
                            url: 'Login3/GetUser',
                            data: { FowstID: sessionStorage.FowstID },
                            success: function (vData) {
                                LoadUser(vData);
                            },
                            error: function (vError) {
                                console.log('Error getting Login/GetUser. Retrying...');
                                GetUser();
                            }
                        });
                    }; // if (OfflineMode) {

                    return true;
                }; // END function GetUser() {

                function LoadUser(vData) {

                    var pUserMenu;
                    var pLink;

                    debugger; // this code should be replaced with the one from PrePos

                    if (CStr2(vData).length == 0) return;
                    if (vData.Error) {
                        NewMessageBox(vData.Error);
                        NewMessageBox(vData.Error, '', null, {
                            OnClose: function () {
                                window.location = '/People/PeopleMenu';
                            }
                        });
                        return;
                    };
                    sessionStorage.AllowPriceAdjustment = CBool(vData.AllowPriceAdjustment);
                    if (vData.FirstName != undefined) {
                        pUserMenu = $('#divUserMenu');
                        pUserMenu.html('');

                        pLink = $('<a>')
        .attr('href', '#pageUserMenu')
        .css('text-decoration', 'none')
        .html('Hi, ' + HtmlEncode(vData.FirstName) + '&#x25BC;');
                        pUserMenu.append(pLink);

                        //////////////////                        if ((CDec2(vData.IsSalesAgent) != 0) && (proj.PosMode == false)) {

                        //////////////////                            $('#divCustomer').show();
                        //////////////////                        };
                        //////////////////                         if (proj.PosMode) {
                        $('#divPosBranch').show();
                        if (CStr2(vData.FowstID).length > 0) {
                            sessionStorage.EnableHotButtonsContextualMenu = CBool(vData.EnableHotButtonsContextualMenu);
                            sessionStorage.FowstID = vData.FowstID;
                            sessionStorage.FowstTin = vData.TIN;
                            sessionStorage.FowstAddress = vData.Address;
                            SalesMenu_offline().HandleEnforceZRead({
                                callback: function () {
                                    LoadSupplier();
                                    HandleMallLessor();
                                    // #lnkPosBranchHotButtons = button in product maker
                                    $('#lnkPosBranch, #lnkPosBranchHotButtons').html(HtmlEncode('Branch: ' + vData.FowstName) + '&#x25BC;');
                                    $('.lblPosBranch').text(vData.FowstName);
                                    //if (proj.POMode)
                                    //    $('#lblCurrentPOSMode, #lblCurrentPOSModeHotButtons').text('Customer: ' + vData.FowstName);
                                    PredeterminePrinter();

                                    VerifyDevice();

                                    //added by janine
                                    sessionStorage.FowstName = vData.FowstName;
                                    WriteLastFowst();
                                    //addeed by francis
                                    if (IsMobile) {  // condition added by Arvin
                                        try {
                                            Transfers.WriteFile(sessionStorage.OrgID + 'LastFowst.flg', null, vData.FowstID);
                                        } catch (e) {
                                            console.error('jsVQWQHN6UG3! ' + e);
                                        }
                                    }
                                    //$.mobile.changePage('#dialogCheckerLogIn');
                                    if (mPrePos.OldCartID) {
                                        sessionStorage.setItem('CartOrderID', mPrePos.OldCartID);
                                        UpdateOrderedQuantity();
                                    }
                                }
                            });

                            ///////end of added by janine
                        } else { // if (CStr2(vData.FowstID).length > 0) {
                            if (OfflineMode) {
                                BrowseAddresses();
                            } else {
                                var pData = { FilterFowst: 1 };
                                AjaxSend({
                                    url: 'Browser/LibraryFowst',
                                    data: pData,
                                    success: function (vFowstData) {

                                        if (vFowstData.Results.Rows.length == 1) {
                                            sessionStorage.FowstID = vFowstData.Results.Rows[0].ID;
                                            LoadSupplier();
                                            HandleMallLessor();
                                            $('#lnkPosBranch').html(HtmlEncode('Fowst: ' + vFowstData.Results.Rows[0].FowstName) + '&#x25BC;');
                                            $('.lblPosBranch').text(vFowstData.Results.Rows[0].FowstName);
                                            PredeterminePrinter();
                                            VerifyDevice();
                                            //added by janine
                                            sessionStorage.FowstName = vFowstData.Results.Rows[0].FowstName;
                                            WriteLastFowst();
                                            ///////end of added by janine

                                            //////                                            VerifyPrinterAndReceiptSettings();

                                        } else {
                                            BrowseAddresses();
                                        }
                                    }
                                });
                            }; // if (OfflineMode) {
                        }; // if (CStr2(vData.FowstID).length > 0) {

                        ////////////////////                         };
                        //                        LoadCustomer();
                        if (window.pageCustomerDetails.LoadCustomer()) {
                            //ADDITIONAL BY JANINE FEB 18
                            //                            LoadRecentlyBought({ DateRangeType: mRecentlyBoughtDateRangeType });

                            NewCart(); // march 26, 2013
                            isCartEmpty = true;


                            //                            pageCustomerDetails.setCustomerInfo();
                        }

                    };

                    if (CDec2(vData.IsSalesAgent) != 1)
                        $('#divCompany').hide();

                }; // LoadUser

                function VerifyPrinterAndReceiptSettings() {
                    //Check if Device has assigned Printer and receipt settings
                    if (OfflineMode) {
                        CheckForPrinterAndReceiptSettings(SalesPayment_offline().getOrgFOWSTPrinterIP(), SalesPayment_offline().getReceiptHeaderInfo());
                    } else {
                        AjaxSend({
                            url: "Organization/getOrgFOWSTPrinterIP",
                            success: function (vDataPrint) {
                                AjaxSend({
                                    url: "SalesPayments/getReceiptHeaderInfo",
                                    success: function (vReceiptSettings) {
                                        CheckForPrinterAndReceiptSettings(vDataPrint.OrganizationFOWSTData, vReceiptSettings.SalesPaymentData);
                                    }
                                });
                            }
                        });
                    }
                    //End Check if Device has assigned Printer and receipt settings
                }

                function CheckForPrinterAndReceiptSettings(vDataPrint, vDataReceiptSettings) {

                    //Printer 
                    if (vDataPrint == undefined) {
                        NewMessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                            "Printer",
                            [
                                {
                                    Text: 'Ok',
                                    Click: function () {
                                        window.location = '/People/PeopleMenu';
                                    }
                                }
                            ]
                    );
                        return;
                    } else {
                        if (vDataPrint.Rows.length <= 0) {
                            NewMessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                                "Printer",
                                [
                                    {
                                        Text: 'Ok',
                                        Click: function () {
                                            window.location = '/People/PeopleMenu';
                                        }
                                    }
                                ]
                        );
                            return;
                        }
                    }

                    //ReceiptSettings
                    if (vDataReceiptSettings == undefined) {
                        NewMessageBox("No receipt settings created for this FOWST. Please contact your system administrator for assistance.",
                            "Receipt Settings",
                            [
                                {
                                    Text: 'Ok',
                                    Click: function () {
                                        window.location = '/People/PeopleMenu';
                                    }
                                }
                            ]
                    );
                        return;
                    } else {
                        if (vDataReceiptSettings.Rows.length <= 0) {
                            NewMessageBox("No receipt settings created for this FOWST. Please contact your system administrator for assistance.",
                                "Receipt Settings",
                                [
                                    {
                                        Text: 'Ok',
                                        Click: function () {
                                            window.location = '/People/PeopleMenu';
                                        }
                                    }
                                ]
                        );
                            return;
                        }
                    }

                }

                proj.BrowseCustomer = function (options) {
                    BrowsePopup('LibraryOrganization', 'Select Customer', [
                        {
                            AutoSearch: true,
                            Offline: true,
                            DisplayText: [{ Column: 'CompanyName' }, { Label: 'Address', Column: 'CompleteAddress'}, { Label: 'TIN', Column: 'TinNumber'}],
                            Click: function (e) {                                
                                sessionStorage.removeItem('CustomerFowstID');
                                sessionStorage.removeItem('CustomerFowstName');
                                sessionStorage.removeItem('CustomerFowstAddress');

                                PopupButtonList('What do you want to do?', 'Message', [
                                    {
                                        Text: 'Choose this Customer',
                                        Click: function () {
                                            $('#btnClient').val(e.CompanyName).data('id', e.ID);
                                            $('#divCustomer').show();

                                            //if ((sessionStorage.CustomerOrganizationID != e.ID)
                                            //    && (window.location.pathname.toLowerCase() != '/salesmenu/bsposviewall')) {
                                            //    NewCart();
                                            //    isCartEmpty = true;
                                            //    LoadRecentlyBought({ DateRangeType: mRecentlyBoughtDateRangeType });
                                            //}
                                            //end of additional by janine mar 17, 3pm

                                            sessionStorage.BillingAddressID = e.ID //added by francis 03/28/16 for customer address in receipt

                                            proj.CustomerID = e.ID;
                                            sessionStorage.CustomerOrganizationID = e.ID;
                                            sessionStorage.CustomerOrganizationName = e.CompanyName;
                                            if (IsGuid(e.DefaultHotButtonTemplateID))
                                                sessionStorage.HotButtonMenuID = e.DefaultHotButtonTemplateID;
                                            else
                                                sessionStorage.HotButtonMenuID = proj.FowstTemplateID;

                                            //////////ADDITIONAL BY JANINE 
                                            pageCustomerDetails.setCustomerInfo();
                                            if (OfflineMode)
                                                SalesMenu_offline().SaveCustomer(proj.CustomerID);
                                            //                        ClearCustomerFreeTypeSession();
                                            if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {

                                                location.hash = '';
                                                //$.mobile.back();
                                                window.pageHeader.LoadCustomer();

                                            }

                                            if (!OfflineMode) {
                                                if (Browser.LibraryOrganization.OutputData.OfferAsDefault == 1) {
                                                    setTimeout(function () {
                                                        NewMessageBox('You have no default customer. Do want to set ' + e.CompanyName + ' as your default customer?', 'Set as default', [
                                                            {
                                                                Text: 'Yes',
                                                                Click: function () {
                                                                    AjaxSend({
                                                                        url: 'SalesMenu/SetDefaultCustomer',
                                                                        data: { CustomerOrganizationID: e.ID },
                                                                        success: function (vData) {
                                                                            if (vData.Result == 'ok') {
                                                                                NewMessageBox('Customer was successfully set as default', 'Success');
                                                                            } else if (vData.Error) {
                                                                                NewMessageBox(vData.Error, 'Error');
                                                                            };
                                                                        }
                                                                    });
                                                                }
                                                            }, {
                                                                Text: 'No'
                                                            }
                                                        ]); // NewMessageBox
                                                    }, 1000); // setTimeout
                                                }; // if (Browser.LibraryOrganization.OutputData.OfferAsDefault == 1) {
                                            } else { // if (!OfflineMode) {
                                                return; // may 29, 17 by arnie walay fowst ang customer so dili sa ni ipakita
                                                setTimeout(function () {
                                                    SalesMenu_offline().BrowseCustomerAddress({
                                                        OrgID: sessionStorage.CustomerOrganizationID,
                                                        Click: function (e) {
                                                            sessionStorage.BillingAddressID = e.ID //added by francis 03/28/16 for customer address in receipt
                                                            sessionStorage.CustomerFowstID = e.ID;
                                                            sessionStorage.CustomerFowstName = e.FowstName;
                                                            sessionStorage.CustomerFowstAddress = e.Address;
                                                            $('#btnClient').val(HtmlEncode(sessionStorage.CustomerOrganizationName) +
                                                                '; ' + HtmlEncode(sessionStorage.CustomerFowstName) +
                                                                '; ' + HtmlEncode(sessionStorage.CustomerFowstAddress) +
                                                                '&#x25BC;')
                                                                .data('FowstID', e.ID);
                                                        }
                                                    });
                                                }, 500);
                                            } // END if (!OfflineMode) {
                                            return; // march 26, 2013
                                            LoadCart();
                                        }
                                    }, 
                                    {
                                        Text: 'Edit details and choose this customer',
                                        Click: function () {
                                            options.Edit(e.ID)
                                        }
                                    }
                                ])                                
                            }
                        }
                    ], {
                        FilterCustomers: 1
                    })
                    return
                    window.pageSourcePage = $.mobile.activePage;

                    if (!IsMobile) {
                        Browser.LibraryOrganization.InputData.ShowAdd = true;
                    }

                    Browser.LibraryOrganization.InputData.FilterCustomers = 1;
                    Browser.LibraryOrganization.Click = function (e) {
                        //additional by janine



                    };
                    Browser.LibraryOrganization.Show();

                };

                $('#btnSwitchOldUI').click(function () {
                    location = MergeUrlParameters({
                        mode: 'pos',
                        OrgID: sessionStorage.OrgID,
                        IsMobile: 1
                    }, '/SalesMenu')
                })

                function BrowseAddresses() {

                    debugger; // this code should be replaced with the one from PrePos

                    Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
                    Browser.LibraryFowst.InputData.FilterFowst = 1;
                    Browser.LibraryFowst.AfterClick = function (e) {
                        sessionStorage.FowstTin = e.TIN;
                        sessionStorage.FowstAddress = e.Address;
                        sessionStorage.FowstID = e.ID;
                        sessionStorage.EnableHotButtonsContextualMenu = CBool(e.EnableHotButtonsContextualMenu);
                        //if (proj.POMode)
                        //    $('#lblCurrentPOSMode, #lblCurrentPOSModeHotButtons').text('Customer: ' + e.FowstName);
                        // LoadSupplier(); // commented by Arvin 2016-08-11
                        HandleMallLessor();
                        $('#lnkPosBranch').html(HtmlEncode('Fowst: ' + e.FowstName) + '&#x25BC;');
                        $('.lblPosBranch').text(e.FowstName);
                        PredeterminePrinter();
                        VerifyDevice();
                        //added by janine
                        sessionStorage.FowstName = e.FowstName;
                        WriteLastFowst();
                        ///////end of added by janine
                        if (OfflineMode) {
                            SalesMenu_offline().HandleEnforceZRead();
                            Android.UpdateSessionData({
                                FowstID: sessionStorage.FowstID
                            }); // disconnected checkout
                            SalesMenu_offline().UpdateCounterFromOnline();
                        }

                        if (window.location.pathname.toLowerCase() == '/billofmaterials')
                            $('#lblCurrentFOWST').html(HtmlEncode(e.FowstName) + '&#x25BC;');
                        //////                        VerifyPrinterAndReceiptSettings();

                        //$.mobile.changePage('#dialogCheckerLogIn');
                    };
                    Browser.LibraryFowst.Show(null, {
                        description: proj.POMode ? 'Please choose the FOWST (business location) that you are doing a Purchase Order for' : ''
                    });

                };

                $('#lnkRecentlyBought').click(function (e) { //

                    if (GetCustomerID() == undefined) {
                        e.preventDefault();
                        return false;
                    };

                    var daterange = [{ Text: 'Within last week' }, { Text: 'Within the last 2 weeks' }, { Text: 'Within the last 4 weeks' },
                                    { Text: 'Within the last 3 months' }, { Text: 'Within the last 6 months' }, { Text: 'Within last year' }, { Text: 'Week to date' },
                                    { Text: 'Month to date' }, { Text: 'Quarter to date' }, { Text: 'Year to date' }]

                    for (var i = 0; i < daterange.length; i++) {
                        daterange[i].Click = function (e) {
                            $('#lnkRecentlyBought').html(e.Text + '&#x25BC;');
                            LoadRecentlyBought({ DateRangeType: e.Text });
                        }
                    }

                    PopupButtonList('', 'Date Range', daterange);
                });

                function HandleMallLessor() {

                    debugger; // this code should be replaced with the one from PrePos

                    if (localStorage.LoginHash != undefined) {
                        AjaxSend({
                            url: 'MallLessor/SetMall',
                            data: {
                                LoginHash: localStorage.LoginHash
                                // ,Mall: $('#cbMall > option:selected').val()
                            },
                            success: function (vData) {
                                if (vData.Error) {
                                    NewMessageBox(vData.Error);
                                    return;
                                } else if (vData.Result) {
                                    localStorage.removeItem('LoginHash');
                                    NewMessageBox('Mall Report Setup was successful.', 'Success');
                                };
                            }
                        }); // AjaxSend({
                    }; // if (localStorage.LoginHash != undefined) {
                }; // HandleMallLessor

                $('#pageRecentMenu a').click(function () {

                    var pCustomerID;
                    var pKey;
                    var pThis = $(this);

                    pKey = pThis.attr('href');
                    if ((pKey != undefined)
&& (pKey.length > 2)) {

                        if (pKey.substring(0, 1) == '#')
                            pKey = pKey.substring(1);   // there's an href, use that
                        else
                            pKey = undefined; // link didnt start w/ #

                    };
                    if (CStr2(pKey).length < 2)
                        pKey = pThis.text(); // no href, use text

                    switch (pKey) {
                        case 'Within last week':
                        case 'Within the last 2 weeks':
                        case 'Within the last 4 weeks':
                        case 'Within the last 3 months':
                        case 'Within the last 6 months':
                        case 'Within last year':
                        case 'Week to date':
                        case 'Month to date':
                        case 'Quarter to date':
                        case 'Year to date':
                            //Browser.TransLastestOrderedStockByCustomer.InputData.DateRangeType = pKey;
                            break;

                        default:
                            return;
                    };

                    mRecentlyBoughtDateRangeType = pKey;
                    pCustomerID = GetCustomerID();
                    if (pCustomerID != undefined) {
                        history.back();

                        setTimeout(function () {
                            $('#lnkRecentlyBought').html(pKey + '&#x25BC;');
                            LoadRecentlyBought({ DateRangeType: pKey });
                        }, 1000);
                        //                Browser.TransLastestOrderedStockByCustomer.InputData.CustomerID = pCustomerID;
                        //                Browser.TransLastestOrderedStockByCustomer.Click = function(e) {

                        //                    // user clicked an item
                        //        //            $('#butCustomer').text(e.CompanyName)
                        //        //                .button('refresh');
                        //        
                        //                };
                        //                Browser.TransLastestOrderedStockByCustomer.Show();
                    };

                });

                function GetCustomerID() {

                    var pCustomerID = sessionStorage.CustomerOrganizationID; // $('#btnClient').data('id');

                    if (pCustomerID == undefined)
                        NewMessageBox('Please select a customer first', 'No selected customer');
                    else
                        return pCustomerID;

                };

                $('#txtSearch').keydown(function (e) {
                    if (e.which == 13)
                        Search();
                });

                $('#divSearchMulti').click(function () {
                    SearchMulti();
                });

                function Search() {
                    if (!proj.HandlePosIsEditable())
                        return false;
                    sessionStorage.IsInventoryCountEditSKUMode = false;
                    window.IsBrowserloaded = false;

                    var param = {
                        IsSearchMore: 0
                    }

                    if (window.location.pathname.toLowerCase() == '/billofmaterials')
                        param.WithContentsOnly = true;
                    $('#txtSearch').blur();
                    BrowsePopup('LibraryPIMProduct', 'Products', [
                        {
                            Offline: true,
                            Search: $('#txtSearch, [name=txtSearch]').val(),
                            DisplayText: [{ Column: 'ProductName' }],
                            ShowPic: Business.SystemDefaults.ShowCondensedProductInformationInCart(),
                            Click: function (e) {
                                SelectProduct(e, {});
                            },
                            InitComplete: function (e, vRows) {
                                if (Business.SystemDefaults.ShowCondensedProductInformationInCart()) {
                                    AjaxSend({
                                        url: 'Organization/GetBulkPIMPictures',
                                        data: { data: vRows, size: 'medium', IsDefault: true },
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
                                            //var sourceimg = (window.DebugMode ? location.origin : 'http://119.93.147.163');
                                            var sourceimg = Android.GetServer();

                                            for (var i = 0; i < vRows.length; i++) {
                                                if (vRows[i].Pics != '') {
                                                    vRows[i].Ctr = 1;
                                                    var pSrc = sourceimg + 'Content/MobizImagePIM/' + sessionStorage.OrgID + '/Medium/' + vRows[i].Pics[0].FileName;

                                                    $('#cartimg_' + vRows[i].ID).attr('src', pSrc)
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
                                                                    if (e == null) {
                                                                        console.warn('e is null');
                                                                        return; // silent error lang
                                                                    }

                                                                    if (e.Error) {
                                                                        console.warn(e.Error);
                                                                        return; // silent error lang
                                                                    }
                                                                    vRows = e.data[0].Pics;

                                                                    var div = $('#divCartImage');

                                                                    if ($('#skupicscart').length == 0) {
                                                                        div.append($('<div id="skupicscart" style="display:none;">'));
                                                                    } else {
                                                                        $('#skupicscart').empty();
                                                                    }

                                                                    div = $('#skupicscart');

                                                                    for (var i = 0; i < vRows.length; i++) {
                                                                        vRows[i].Ctr = i + 1;
                                                                        vRows[i].ID = data.ID;
                                                                        vRows[i].ProductName = data.ProductName;
                                                                        var srcBig = sourceimg + 'Content/MobizImagePIM/' + sessionStorage.OrgID + '/Big/' + vRows[i].FileName;

                                                                        div.append($('<a data-role="none" data-lightbox="skupicturescart">')
                                                                                    .attr('id', 'piccart_' + vRows[i].ID)
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
                                                                    var firstpic = $('#piccart_' + vRows[0].ID);
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
                                                    $('#cartimg_' + vRows[i].ID).attr('src', '../Content/images/box.png')
                                                }
                                            }
                                        },
                                        error: function (e) {
                                            Android.AddLog('Cart SKU pics: Can\'t connect to the server, SKU image not successfully loaded');
                                        }
                                    })
                                }
                            }
                        }
                    ], param)

                    $('#txtSearch, [name=txtSearch]').val('');

                } // END function Search() {

                var htmlStockInfo = '';
                function Bs_ShowStockPage() {
                    if (htmlStockInfo == '') {
                        AjaxSend({
                            ExactUrl: '/Stock/BsIndex?cache=1&theme=bs',
                            type: 'GET',
                            dataType: 'text',
                            success: function (vData) {
                                htmlStockInfo = vData;
                                $('#pageStockInfo').append(vData).pageshow();
                            }
                        });
                    } else {
                        $('#pageStockInfo').pageshow();
                        StockInfo.PageShow();
                    }
                    $('.lblPosBranch').text(sessionStorage.FowstName);
                } // END function Bs_ShowStockPage() {
                window.Bs_ShowStockPage = Bs_ShowStockPage;

                function SearchMulti() {
                    if (!proj.HandlePosIsEditable())
                        return false;

                    if (proj.POMode) {
                        if (!IsGuid(sessionStorage.SupplierOrganizationID))
                            return NewMessageBox('Please select a supplier first.');
                        if (!IsGuid(sessionStorage.SupplierFowstID))
                            return NewMessageBox('Please select a supplier outlet or FOWST first.');
                        //if (!IsGuid(sessionStorage.SupplierBinID)) 
                        //    return NewMessageBox('Please select a supplier department first.');

                        if (!IsGuid(sessionStorage.CustomerOrganizationID))
                            return NewMessageBox('Please select a customer first.');
                        if (!IsGuid(sessionStorage.CustomerFowstID))
                            return NewMessageBox('Please select a customer outlet or FOWST first.');
                        //if (!IsGuid(sessionStorage.SupplierBinID)) 
                        //    return NewMessageBox('Please select a supplier outlet or FOWST first.');
                    }

                    sessionStorage.IsInventoryCountEditSKUMode = false;
                    window.IsBrowserloaded = false;
                    Browser.LibraryPIMProduct.InputData.IsSearchMore = 0;

                    if (window.location.pathname.toLowerCase() == '/billofmaterials')
                        Browser.LibraryPIMProduct.InputData.WithContentsOnly = true; // debug only

                    var pColumns = [
                        {
                            Column: function (e) {
                                var lbl;

                                lbl = '<b>No:</b> ' + e.ProductNo + '<br>';
                                lbl += '<b>Name:</b> ' + e.ProductName;

                                return lbl;
                            },
                            Label: 'Product'
                        }
                    ];
                    var pAdditionalData = { IsAll: 1 };
                    pAdditionalData.FilterCustomers = 1;
                    //pColumns.push({
                    //    Label: 'Retail Price' ,
                    //    Column: function(vData) {
                    //        return AccountingFormat(vData.RetailPrice);
                    //    }
                    //});

                    pColumns = pColumns.concat([
                        {
                            Label: 'Qty',
                            Style: {
                                width: '100px'
                            },
                            Render: function (v1, v2, vData) {
                                return '<div name="divQty" />';
                                //return '<input class="qty" style="width:70px" />';
                            }
                        }
                    ]);
                    //pColumns.push(
                    //    {
                    //        Label: 'Action',
                    //        Render: function(v1, v2, vData) {
                    //            return '<div name="divAction"></div>';
                    //        }
                    //    }
                    //);
                    //ERP.QuickPim.controller.AfterSave = SelectProduct;
                    //var pLandedCostID = Business.SystemDefaults.GetLandedCostID();
                    //if (IsGuid(pLandedCostID))
                    //    OfflineBrowser.LibraryPIMProduct.PriceLevelID = pLandedCostID;
                    var arrSelectedProduct = [];
                    BrowsePopup('LibraryPIMProduct', 'Search Products',
                        [
                            {
                                Message: 'You can add multiple products to the cart at once. Click OK when you\'re done.',
                                AutoSearch: false,
                                ShowOkButton: true,
                                DisplayText: pColumns,
                                OfflineBrowser: OfflineBrowser.LibraryPIMProduct,
                                Click: function (e) {
                                    if ($.isArray(e))
                                        PreSelect(e);
                                    else
                                        SelectProduct(e, {});
                                },
                                AllowEmptySearch: false,
                                StyleFormat: 'table',
                                InitComplete: function (vTable) {
                                    vTable.find('.qty').click(function (e) {
                                        e.stopPropagation();
                                    });
                                },
                                rowCallback: function (row, data, dataIndex) {
                                    //var pDiv = $(row).find('[name=divAction]');
                                    //if (pDiv.children().length > 0)
                                    //    return;
                                    //var pLink = $('<a>')
                                    //    .text('Info')
                                    //    .attr('href', '#')
                                    //    .click(function(e) {
                                    //        e.preventDefault();
                                    //        mInventoryBalance.Show({
                                    //            ProductID: data.ID,
                                    //            ProductNo: data.ProductNo,
                                    //            ProductName: data.ProductName
                                    //        });
                                    //        return false;
                                    //    });
                                    //pDiv.append(pLink);
                                    var pDiv = $(row).find('[name=divQty]');
                                    $(row).click(function (e) {
                                        e.preventDefault();
                                        return false;
                                    });
                                    pDiv.click(function (e) {
                                        e.preventDefault();
                                        return false;
                                    });

                                    if (pDiv.find('input').length == 0) {
                                        pDiv.append($('<input data-role="none" type="number" class="qty form-control" style="width:100%;min-width:70px" />')
                                                .data('data', data)
                                                .keyup(function () {
                                                    var data = $(this).data('data');
                                                    var qty = CDec2($(this).val());
                                                    var index = null;

                                                    $.grep(arrSelectedProduct, function (e, i) {
                                                        if (e.ID == data.ID) {
                                                            index = i;
                                                        }
                                                    })

                                                    data.Qty = qty;

                                                    if (index == null) {
                                                        arrSelectedProduct.push(data); // add
                                                    } else {
                                                        if (qty == 0) {
                                                            arrSelectedProduct.splice(index, 1); // remove
                                                        } else {
                                                            arrSelectedProduct[index] = data; // update
                                                        }                                                        
                                                    }
                                                })
                                            );
                                    }

                                    //display current QTY of the product
                                    $.grep(arrSelectedProduct, function (e, i) {
                                        if (e.ID == data.ID) {
                                            pDiv.find('input').val(e.Qty);
                                        }
                                    })
                                }
                            }
                        ], pAdditionalData
                    ); // END BrowsePopup('LibraryFowst', 'Fowst',

                    function PreSelect() {
                        Bs.Loading('show', 'Adding to cart... please wait');

                        setTimeout(function () {
                            var pData = [];
                            for (var p = 0; p <= arrSelectedProduct.length - 1; p++) {
                                var pDR = arrSelectedProduct[p];
                                var pPrice = pDR.DefaultPrice;
                                if (CDec2(pPrice) < 0.01)
                                    pPrice = 1;
                                var pLineItem = {
                                    ActualQuantity: 1,
                                    ProductID: pDR.ID,
                                    Quantity: pDR.Qty,
                                    DiscountedPrice: pDR.DefaultPrice
                                }
                                if (window.OfflineMode) {
                                    var pUnit = Business.LibraryPIMProduct.BaseUnit(pDR.ID);
                                    if (pUnit) {
                                        pLineItem.UnitMeasure = pUnit.UnitMeasure;
                                    }
                                }
                                pData.push(pLineItem);
                            }

                            if (pData.length > 0) {
                                sessionStorage.IsDraft = 1;
                                $.mobile.showPageLoadingMsg();
                                proj.CartStock = pData;
                                proj.SaveCart();
                                LoadCart();
                                $.mobile.hidePageLoadingMsg();
                                Android.Toast(pData.length + ' item(s) has been added to cart');
                            } else {
                                Android.Toast('No selected items')
                            }

                            Bs.Loading('hide');
                        }, 500)                        
                    } // END function PreSelect() {


                    $('#txtSearch, [name=txtSearch]').val('');

                } // END function SearchMulti() {
                $('#butSearchButtonDemo').click(function (e) {

                    $.mobile.changePage('#page2');

                });

                //        $('.ScrollWrapper').each(function() {

                //            var pScroll = new iScroll(this, {
                //	            //snap: 'li',
                //                hScrollbar: false,
                //	            hideScrollbar: true,
                //                fadeScrollbar: true,
                //	            //vScrollbar: false,
                //                ///onBeforeScrollStart: function (e) { return true; }
                //                });
                //            pScroll.refresh();

                //            if (mScroll == null) mScroll = pScroll;

                //        });

                $('#lnkScrollLeft').click(function (e) {

                    mScroll.scrollTo(-200, 0, 500, true);

                });

                $('#lnkScrollRight').click(function (e) {

                    mScroll.scrollTo(200, 0, 500, true);

                });

                //        function LoadCustomer(e) {

                //            history.back();
                //            setTimeout(function() {
                //                $('#butCustomer').text(e.CompanyName).closest('.ui-btn').show();
                //                $('#butCustomer').button('refresh');
                //            }, 1000);

                //        }; // LoadCustomer

                function LoadRecentlyBought(vOptions) {
                    //$('#divRecentlyBoughtSection').hide();
                    //$('#divRecentlyBoughtEmpty').show();

                    //additional by janine
                    //$('#divCartPanel').hide();

                    //                    if (OfflineMode) return;
                    vOptions = $.extend({
                        DateRangeType: 'Within the last 2 weeks'
                    }, vOptions);

                    AjaxSend({
                        url: 'SalesMenu/RecentlyBought',
                        data: {
                            CustomerID: GetCustomerID(),
                            DateRangeType: vOptions.DateRangeType
                        },
                        success: function (vData) {
                            Display(vData);
                        },
                        error: function (vError) {
                            var pDummy;
                        }
                    });

                    function Display(vData) {
                        var divRecentlyBoughtSection = $('#divRecentlyBoughtSection'), carttotal = 0;
                        divRecentlyBoughtSection.empty();

                        if (vData.data.Rows.length > 0) {
                            divRecentlyBoughtSection.append($('<li data-role="none" style="padding:10px !important;">').text(vData.data.Rows.length + ' result(s) found'));
                            for (var i = 0; i < vData.data.Rows.length; i++) {
                                var vRows = vData.data.Rows[i];
                                var li = $('<li data-role="none" class="list-group-item list-group-item-action" aria-current="true" style="padding:0px !important;">');
                                var table = $('<table style="width:100%;">'), tr = $('<tr>'), td = $('<td>');

                                tr.append($('<td style="width:100px">')
                                        .append($('<img style="width:100px;">')
                                            .attr('id', 'cartimg_' + vRows.ID)
                                            .attr('src', '../Content/images/box.png')
                                        )
                                        ).append($('<td style="padding:10px;">')
                                            .append($('<b>').text(vRows.ProductNo + ' ' + vRows.ProductName))
                                            .append((CBool(vRows.InCart) ? '<br>' + 'In cart: ' + $.format.number(CDec2(vRows.InCart), '#,###') + '' : ''))
                                            .data('data', vRows)
                                            .click(function (e) {
                                                proj.ProductID = $(this).data('data').ID;

                                                Bs_ShowStockPage()
                                            })
                                        ).append($('<td style="width:50px; text-align:center; font-size:20px;">')
                                            .append('<span class="bi-chevron-right"></span>')
                                        )

                                divRecentlyBoughtSection.append(li.append(table.append(tr)));
                            }

                            $('#butRecentlyBoughtAddAll')
                                .data('data', vData.data.Rows)
                                .show()
                                .click(function () {
                                    var data = $(this).data('data');

                                    SalesMenu_offline().HandleAddMultiToCart({
                                        GetData: function () {
                                            return data;
                                        }
                                    });
                                });
                        } else {
                            $('#butRecentlyBoughtAddAll').unbind('click').hide();
                            divRecentlyBoughtSection.append($('<li data-role="none" class="list-group-item list-group-item-action" aria-current="true" style="padding:0px !important;">').text('No results found'));
                        }
                    }; // Display

                }; // LoadRecentlyBought    });

                function LoadCart(vDontFetchIfBlank, vParams) {
                    // PARAMS:
                    //   vDontFetchIfBlank: boolean. if true, dont get the lastest sales order if CartOrderID is blank
                    //additional by janine
                    vParams = $.extend({
                        SaveAsNew: false
                    }, vParams);
                    //$('#divRecentlyBoughtPanel').hide();
                    if (window.location.pathname.toLowerCase() != '/tableview/bsnewtable') {
                        $('#divCartPanel').pageshow();
                        $('#pushTemplate').hide();
                    }                    

                    if (proj.PosMode) {
                        if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall' || window.location.pathname.toLowerCase() == '/tableview/bsnewtable')
                            $('#lblPayNow').html('Go');
                        else {
                            if ($.isFunction(GetStringObject('GetIsOrderTaker', mOrderTakerSetupModel)))
                                if (mOrderTakerSetupModel.GetIsOrderTaker())
                                    $('#lblPayNow').html('Go');
                                else
                                    $('#lblPayNow').html('Pay Now');
                            else
                                $('#lblPayNow').html('Pay Now');
                        }
                    } else {

                        if (proj.POMode)
                            $('#lblPayNow').html('Save Purchase Order');
                        else
                            $('#lblPayNow').html('Save Sales Order');
                    }

                    isOnCartPage = true;

                    var pUrl;
                    var pData = {};

                    //additional by janine
                    $('#divCartPanel').show();

                    $('#divCartSection').hide();
                    $('#divCartEmpty').show();
                    $('#pageMain').pageshow();

                    //added by janine mar 4
                    //show updated current customer org name
                    if (window.location.pathname.toLowerCase() != '/tableview/bsnewtable') {
                        $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                    }                    

                    if (isRecallCart) {
                        pageCustomerDetails.setCustomerInfo();
                    }


                    $('#lnkPosBranch').html(HtmlEncode('Fowst: ' + sessionStorage.FowstName) + '&#x25BC;');
                    $('.lblPosBranch').text(sessionStorage.FowstName);
                    // end of added by janine

                    //additional by janine sept 5 2014 for sales order taker
                    pData.PosMode = proj.PosMode ? "pos" : "so";
                    //end of additional

                    pData.CustomerID = GetCustomerID();
                    pData.OrderID = vParams.ID || sessionStorage.getItem('CartOrderID');
                    mID = pData.OrderID
                    pData.FowstID = sessionStorage.FowstID;
                    if (vDontFetchIfBlank)
                        pData.DontFetchIfBlank = 1;
                    localStorage.removeItem('NewCartMode');
                    proj.ItemsInCart = [];
                    if (OfflineMode) {
                        if (Browser.TransInvoices.OfflineMode == false) {
                            Browser.TransInvoices.OfflineMode = true;
                            AjaxSend({
                                url: 'SalesMenu/PosCart',
                                data: pData,
                                success: function (vData) {
                                    // import invoice data from server
                                    SalesMenu_offline().Import(
                                        $.extend(vData, {
                                            SaveAsNew: vParams.SaveAsNew
                                        })
                                    );
                                    Display(
                                        SalesMenu_offline().PosCart(
                                            $.extend(pData, {
                                                OrderID: vData.Master.ID
                                            })
                                        )
                                    );
                                },
                                error: function (vError) {
                                    var pDummy;
                                }
                            }); // AjaxSend
                        } else {
                            if (window.location.pathname.toLowerCase() == '/billofmaterials') {
                                var p = new App.model.BomMaker();
                                var pBomCart = p.bomCart({
                                    id: sessionStorage.getItem('CartOrderID')
                                });
                                proj.CartData = pBomCart;
                                proj.CartStock = [];
                                Display(pBomCart);
                            } else {
                                Display(
                        SalesMenu_offline().PosCart(pData)
                    );
                            }
                        }
                    } else {
                        if (window.location.pathname.toLowerCase() == '/billofmaterials') {
                            NewMessageBox('Not yet implemented');
                        } else {
                            //                        if (proj.PosMode) {
                            pUrl = 'SalesMenu/PosCart';
                            //                        } else {
                            //                            pUrl = 'SalesMenu/Cart';
                            //                        };

                            AjaxSend({
                                url: pUrl,
                                data: pData,
                                success: function (vData) {
                                    Display(vData);
                                },
                                error: function (vError) {
                                    var pDummy;
                                }
                            }); // AjaxSend
                        }
                    }; // if (OfflineMode) {

                    function Display(vData) {
                        var pModifiersText;

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

                        var pDivTemplate = $('#divCartStock').hide();
                        var pTrTemplate = pDivTemplate.find('[name=trTemplate]').hide();
                        var pShowCondensedProductInformationInCart = Business.SystemDefaults.ShowCondensedProductInformationInCart();

                        $('#pushSection').empty();
                        (function AttachModifiers() {
                            var pDrStock;
                            var pDrModifer;

                            for (var pStockIndex = 0; pStockIndex <= CDec2(GetStringObject('Stocks.Rows.length', vData)) - 1; pStockIndex++) {
                                pDrStock = vData.Stocks.Rows[pStockIndex];
                                if (pDrStock.ModifierGroupID == null)
                                    continue;
                                pDrStock.Modifiers = [];
                                for (var pModifierIndex = 0; pModifierIndex <= CDec2(GetStringObject('Modifiers.Rows.length', vData)) - 1; pModifierIndex++) {
                                    pDrModifer = vData.Modifiers.Rows[pModifierIndex];
                                    if (ReadyGuid(pDrStock.ModifierGroupID) == ReadyGuid(pDrModifer.ModifierGroupID))
                                        pDrStock.Modifiers.push(pDrModifer);
                                } // END pModifierIndex
                            } // END pStockIndex
                        })(); // END AttachModifiers
                        // FOR BILL OF MATERIALS MAKER
                        if (window.location.pathname.toLowerCase() == '/billofmaterials remove_this') {

                            if (vData.Rows.length == 0) {

                                //additional by janine mar 6. if cart is empty, empty the session storage
                                sessionStorage.removeItem('InvoiceProductIDList');
                                sessionStorage.removeItem('CartOrderID');
                                sessionStorage.DeliveryAddressID = '';
                                sessionStorage.BillingAddressID = '';
                                sessionStorage.DeliveryDate = '';
                                proj.ProductID = '';
                                sessionStorage.LastQty = 1;
                                sessionStorage.ProductName = '';
                                sessionStorage.ProductUnitQty = 0;
                                sessionStorage.ProductUnitMeasure = '';
                                sessionStorage.ProductPrice = 0;
                                //end of additional by janine

                                if (mLvCart != undefined)
                                    mLvCart.Element.hide();
                                //additional by janine
                                $('#divCartPanel').show();

                                $('#divCartSection').hide();
                                $('#divCartEmpty').show();

                                $('#spanCartTotal').text('0.00'); //additional by janine nov 11

                                isCartEmpty = true;
                                return;
                            };

                            //ADDITIONAL BY JANINE FEB 18
                            isCartEmpty = false;
                            if (mLvCart == undefined) {

                                mLvCart = new CreateListView(); // the "new" keyword is important to prevent conflict with other instances of CreateListView
                                mLvCart.Element.find('ul')
                                .attr('data-split-icon', 'minus');
                                mLvCart.Element.insertAfter(pDivTemplate);

                                mLvCart.OnItem = function (vDataRow, vItem, e) {

                                    var pTR;
                                    var pDiv;
                                    var pDrUnit;
                                    var pItemTotal = 0;

                                    var pLnk = $('<a>')
                                    .attr('href', '../Stock?id=' + vDataRow.ParentProductID)
                                    .data('data', vDataRow)
                                    .click(function (e) {
                                        e.preventDefault();
                                        if (!proj.HandlePosIsEditable())
                                            return false;
                                        proj.ProductID = $(this).data('data').ParentProductID;
                                        //                        sessionStorage.IsInventoryCountEditSKUMode = true;
                                        sessionStorage.LastQtyDecimal = $(this).data('data').ParentQuantity;
                                        startclock('#clockStock');
                                        if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall')
                                            ChangePage('../Stock');
                                        else
                                            ChangePage('Stock');
                                        return false;
                                    });
                                    pDiv = pDivTemplate.clone()
                                    .attr('id', '');
                                    if (pShowCondensedProductInformationInCart)
                                        pDiv.find('[name=spanStockDescription]').text(vDataRow.ParentProductName);
                                    else
                                        pDiv.find('[name=spanStockDescription]').text(vDataRow.ParentProductNo + ' ' + vDataRow.ParentProductName);

                                    //                                    if (pDrUnit.ProductID == vDataRow.ProductID) {
                                    pTR = pTrTemplate.clone();
                                    //edited by janine sept 5 for sales order taker
                                    pTR.find('[name=tdQty]').text($.format.number(CDec2(vDataRow.ParentQuantity), proj.QuantityFormat));
                                    pTR.find('[name=tdUnit]').text(vDataRow.ParentUnitMeasure);
                                    //                                            pTR.find('[name=tdPrice]').text(AccountingFormat(vDataRow.UnitPrice));
                                    //                                            pTR.find('[name=tdLineTotal]').text(AccountingFormat(vDataRow.SubTotal));
                                    pDiv.find('[name=trTemplate]').parent().append(pTR);
                                    pTR.show();
                                    //                                            pItemTotal += CDec2(pDrUnit.SubTotal);
                                    //                                    };

                                    //                                    pDiv.find('[name=spanTotal]').text(AccountingFormat(pItemTotal));
                                    //                                    mLvCart.OnItem.CartTotal += pItemTotal;
                                    if (!pShowCondensedProductInformationInCart) {
                                        pLnk.append($('<img>')
                                        .attr('src', '../Content/images/box.png'));
                                        pLnk.append(pDiv);
                                    }
                                    pDiv.show();
                                    vItem.append(pLnk)
                                    .append($('<a>')
                                        .attr('href', '#')
                                        .data('data', vDataRow)
                                        .data('li', vItem)
                                        .click(function (e) {
                                            if (!proj.HandlePosIsEditable())
                                                return false;
                                            var pDR = $(this).data('data');
                                            var pLI = $(this).data('li');

                                            NewMessageBox('Are you sure you want to remove ' + pDR.ParentProductName + ' from your cart?',
                                                'Remove',
                                                [
                                                    {
                                                        Text: 'Yes',
                                                        Click: function () {


                                                            if (OfflineMode) {

                                                                var p = new App.model.BomMaker();
                                                                p.deleteProduct({
                                                                    makerProductID: pDR.ID,
                                                                    success: function (vData) {

                                                                        ShowResult(vData);
                                                                    },
                                                                    error: function (vMessage) {
                                                                        NewMessageBox(vMessage);
                                                                    }
                                                                });

                                                            } else {
                                                                NewMessageBox('Not yet implemented');
                                                            }; // END if (OfflineMode) {

                                                            function ShowResult(vData) {
                                                                //                                                if (vData.Error) {
                                                                //                                                    NewMessageBox(vData.Error);
                                                                //                                                } else {

                                                                history.back();
                                                                LoadCart();

                                                                //                                                };
                                                            }; // END ShowResult
                                                        }
                                                    }
                                                ,
                                                    {
                                                        Text: 'No', //Added by Janine --------------------------
                                                        Click: function () {
                                                            history.back();
                                                            history.back();
                                                        }
                                                    },
                                                    {
                                                        Text: 'New Cart', //Added by Janine Oct 18 5pm--------------------------
                                                        Click: function () {
                                                            CreateNewCart();
                                                        }

                                                    } //End of Added by Janine Oct 18 5pm----------------------------------------------
                                                ]);
                                            return false;
                                        })
                                        .text('Remove')
                                    ); // .append($('<a>')

                                }; // mLvCart.OnItem
                            };  // if (mLvCart == undefined) {

                            mLvCart.vData = vData;
                            //                            mLvCart.OnItem.CartTotal = 0;
                            //                        
                            mLvCart.SetData(vData.Rows);

                            //additional by janine
                            $('#divCartPanel').show();

                            $('#divCartEmpty').hide();
                            $('#divCartSection').show();
                        } else { // ELSE if (window.location.pathname.toLowerCase() == '/billofmaterials remove_this') {

                            if ((vData.Stocks == undefined)
                                || (vData.Stocks.Rows.length == 0)) {

                                //additional by janine mar 6. if cart is empty, empty the session storage
                                sessionStorage.removeItem('InvoiceProductIDList');
                                sessionStorage.DeliveryAddressID = '';
                                sessionStorage.BillingAddressID = '';
                                sessionStorage.DeliveryDate = '';
                                //end of additional by janine

                                if (mLvCart != undefined)
                                    mLvCart.Element.hide();
                                //additional by janine
                                $('#divCartPanel').show();

                                $('#divCartSection').hide();
                                $('#divCartEmpty').show();
                                $('#divCartRetail').empty().append('<li><div style="text-align:center; color: #a9afb5; font-size:25px;"><span class="bi-cart4" style="font-size:100px;"></span><br />Cart is empty</div></li>');

                                $('#spanCartTotal').text('0.00'); //additional by janine nov 11

                                isCartEmpty = true;
                                return;
                            };
                            //ADDITIONAL BY JANINE FEB 18
                            isCartEmpty = false;
                            //additional march 1 2014
                            //save to product ids and qty to sessionStorage array


                            var arrProductIDList = new Array();

                            for (var i = 0; i < vData.Stocks.Rows.length; i++) {
                                arrProductIDList[i] = vData.Stocks.Rows[i].ProductID + '|' + vData.Units.Rows[i].Quantity;

                                sessionStorage.InvoiceProductIDList = JSON.stringify(arrProductIDList);
                            }

                            //end of ADDITIONAL BY JANINE

                            sessionStorage.setItem('CartOrderID', vData.Master.ID);

                            sessionStorage.DeliveryAddressID = CStr2(vData.Master.DeliveryAddressID);
                            sessionStorage.BillingAddressID = CStr2(vData.Master.BillingAddressID);
                            sessionStorage.DeliveryDate = CStr2(vData.Master.DeliveryDate);

                            if (proj.POMode) {
                                if (IsGuid(vData.Master.SupplierID)) {
                                    sessionStorage.SupplierOrganizationID = vData.Master.SupplierID;
                                    // todo
                                    $('#lblSupplierName, #lblCustomerName, #lblSupplierNameHotButtons')
                                        .text('Supplier: ' + vData.Master.SupplierOrgName + ' - ' + vData.Master.SupplierFowstName)
                                        .data('id', vData.Master.SupplierID);
                                }

                                if (IsGuid(vData.Master.SupplierFowstID)) {
                                    sessionStorage.SupplierFowstID = vData.Master.SupplierFowstID;
                                }

                                if (IsGuid(vData.Master.SupplierBinID)) {
                                    sessionStorage.SupplierBinID = vData.Master.SupplierBinID;
                                    sessionStorage.SupplierBinName = vData.Master.SupplierBinName;
                                }

                                sessionStorage.CustomerOrganizationID = vData.Master.CustomerID;
                                $('#lblCurrentPOSMode, #lblCurrentPOSModeHotButtons')
                                    .text('Customer: ' + vData.Master.CustomerName + ' - ' + vData.Master.CustomerFowstName);

                                if (IsGuid(vData.Master.CustomerFowstID))
                                    sessionStorage.CustomerFowstID = vData.Master.CustomerFowstID;

                                if (IsGuid(vData.Master.CustomerBinID)) {
                                    sessionStorage.CustomerBinID = vData.Master.CustomerBinID;
                                    sessionStorage.CustomerBinName = vData.Master.CustomerBinName;
                                }
                            }


                            //additional by janine nov 7
                            if (window.location.pathname.toLowerCase() == '/salesmenu')
                                DisplaySalesOrder(vData.Master);


                            pDivTemplate.hide();

                            if (mLvCart == undefined) {
                                mLvCart = new CreateListView(); // the "new" keyword is important to prevent conflict with other instances of CreateListView
                                mLvCart.Element.find('ul')
                                    .attr('data-split-icon', 'minus');
                                mLvCart.Element.insertAfter(pDivTemplate);
                                mLvCart.OnItem = function (vDataRow, vItem, e) {

                                    var pTR;
                                    var pDiv;
                                    var pDrUnit;
                                    var pItemTotal = 0;
                                    //Edited by Edmund Uba, August 26, 2015
                                    var prodIDFormat;

                                    //The reason for this is to make sure that all product IDs are same with the
                                    //id on the item, if they are same, it will create a new list.
                                    //The format of the ID should all be uppercase
                                    if (CBool(vDataRow.HasModifier) == false) {
                                        prodIDFormat = (vDataRow.ProductID).toUpperCase();
                                    } else {
                                        prodIDFormat = (vDataRow.ProductID + '_' + vDataRow.GroupIdOfItemModifiers).toUpperCase();
                                    }
                                    proj.ItemsInCart.push(vDataRow);
                                    vItem.attr('id', prodIDFormat);
                                    if (CBool(sessionStorage.IsDraft)) {
                                        var pLnk = $('<a>')
                                                    .attr('href', '../Stock?id=' + vDataRow.ProductID)
                                                    .data('data', vDataRow)
                                                    .click(function (e) {
                                                        var pData;
                                                        if (!proj.HandlePosIsEditable())
                                                            return false;
                                                        e.preventDefault();
                                                        pData = $(this).data('data');
                                                        proj.ProductID = pData.ProductID;
                                                        //proj.ModifierGroupID = pData.ModifierGroupID; commented by arnie (472017) para walay masave sa table TransInvoiceItemsRebates. see function SaveModifiers in Stock_offline.js
                                                        if (CBool(pData.HasModifier)) {
                                                            PopupButtonList('', 'Options', [
                                                                {
                                                                    Text: 'Go to Stock Info',
                                                                    Size: 3,
                                                                    Click: function () {
                                                                        proj.GroupIdOfItemModifiers = pData.GroupIdOfItemModifiers;
                                                                        sessionStorage.ItemModifier = JSON.stringify(pData.ItemModifier);
                                                                        GotoStockInfoPage();
                                                                    }
                                                                },
                                                                {
                                                                    Text: 'Edit Modifier',
                                                                    Size: 3,
                                                                    Click: function () {
                                                                        NewItemModifier({
                                                                            EditModifier: true,
                                                                            ProductData: pData,
                                                                            success: function (e) {
                                                                                //proj.GroupIdOfItemModifiers = pData.GroupIdOfItemModifiers;
                                                                                //sessionStorage.ItemModifier = JSON.stringify(e.ItemModifier);
                                                                                //GotoStockInfoPage();
                                                                                Android.Toast('Updating Item ... Please wait');
                                                                                //$("#popupLoad").popup("open");                                                                                            
                                                                                setTimeout(function () {
                                                                                    proj.CartStock = [e];
                                                                                    proj.SaveCart();
                                                                                    Display(SalesMenu_offline().PosCart({ OrderID: sessionStorage.CartOrderID }));
                                                                                    //$("#popupLoad").popup("close");
                                                                                }, 200)
                                                                            }
                                                                        });
                                                                    }
                                                                }
                                                            ], { CloseButton: 'enable', AutoDismiss: true });
                                                        } else {
                                                            //$("#popupLoad").popup("open");
                                                            proj.GroupIdOfItemModifiers = null;
                                                            setTimeout(function () {
                                                                GotoStockInfoPage()
                                                            }, 1000);
                                                        }

                                                        function GotoStockInfoPage() {
                                                            if (window.OfflineMode)
                                                                if (proj.ModifierGroupID == null)
                                                                    proj.RebateData = null;
                                                                else {
                                                                    proj.RebateData = Business.TransInvoice.GetRebates(proj.ModifierGroupID);
                                                                    proj.RebateData.ID = proj.RebateData.RebateID;
                                                                }
                                                            sessionStorage.IsInventoryCountEditSKUMode = true;
                                                            startclock('#clockStock');
                                                            Bs_ShowStockPage(); // new
                                                            return false;
                                                        }
                                                    });
                                        pDiv = pDivTemplate.clone()
                                        .attr('id', '');
                                        if (pShowCondensedProductInformationInCart)
                                            pDiv.find('[name=spanStockDescription]').text(vDataRow.ProductName);
                                        else
                                            pDiv.find('[name=spanStockDescription]').text(vDataRow.ProductNo + ' ' + vDataRow.ProductName);
                                        pModifiersText = '';

                                        pDiv.attr('id', prodIDFormat + '-Order');
                                        for (var p = 0; p <= CDec2(GetStringObject('Modifiers.length', vDataRow)) - 1; p++) {
                                            if (vDataRow.Modifiers[p].ModifierName != null) {
                                                if (pModifiersText != '')
                                                    pModifiersText += '<br />';
                                                pModifiersText += HtmlEncode(vDataRow.Modifiers[p].ModifierName);
                                            }
                                        }
                                        if (pModifiersText == '')
                                            pDiv.find('[name=spanModifiers]').remove();
                                        else
                                            pDiv.find('[name=spanModifiers]').html(pModifiersText);

                                        //for (var p = 0; p <= mLvCart.vData.Units.Rows.length - 1; p++) {
                                        pDrUnit = vDataRow //mLvCart.vData.Units.Rows[p];
                                        //if (vDataRow.ModifierGroupID != pDrUnit.ModifierGroupID)
                                        //    continue;
                                        pTR = pTrTemplate.clone();
                                        //edited by janine sept 5 for sales order taker
                                        pTR.find('[name=tdQty]').text($.format.number((proj.PosMode || proj.BomMakerMode) ? CDec2(pDrUnit.Quantity) : CDec2(pDrUnit.OrderedQuantity), proj.QuantityFormat));
                                        pTR.find('[name=tdUnit]').text(pDrUnit.UnitMeasure);
                                        pTR.find('[name=tdPrice]').text(AccountingFormat(pDrUnit.UnitPrice));
                                        // pTR.find('[name=tdLineTotal]').addClass('cartListTotal').text(AccountingFormat(CDec2(pTR.find('[name=tdQty]').text() * CDec2(pDrUnit.UnitPrice)))); //pDrUnit.SubTotal
                                        pTR.find('[name=tdLineTotal]').addClass('cartListTotal').text(AccountingFormat(CDec2(pTR.find('[name=tdQty]').text()) * CDec2(pDrUnit.UnitPrice))); //pDrUnit.SubTotal modified by arnie 92017
                                        pDiv.find('[name=trTemplate]').parent().append(pTR);
                                        pTR.show();
                                        pItemTotal += CDec2(pDrUnit.SubTotal);

                                        if (pDrUnit.ProductID == vDataRow.ProductID && window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {
                                            //Creates all the items on the cart
                                            $("#pushTemplate").clone().prependTo("#pushSection").attr("id", prodIDFormat.toUpperCase()).show();
                                            //The instructions below makes sure that the item is clickable or interactive
                                            // or ready for editing.
                                            $("#" + prodIDFormat.toUpperCase()).find('.btnListDelete').bind((InAndroid ? 'tap' : 'click'), function (e) {
                                                e.preventDefault();
                                                if (!proj.HandlePosIsEditable())
                                                    return false;
                                                var pData = $(this).data('data');
                                                var pMsg = 'Do you want to remove ' + pData.ProductName + ' from your cart?';
                                                var pParent = $(this).parent();
                                                NewMessageBox(pMsg, 'Remove from cart', [
                                                    {
                                                        Text: 'Yes',
                                                        Click: function () {
                                                            PromptUserAccess('Allow removal of cart items', {
                                                                OnClickOKSuccess: function () {
                                                                    RemoveItem();
                                                                },
                                                                OnFailed: function () {
                                                                    console.log('jsD23C4N8ETV!');
                                                                }
                                                            });

                                                            function RemoveItem() {
                                                                // Business.TransInvoice.RemoveCartStock(pData);
                                                                window.HotButtons.UpdateCartStock(pData, null, 0);
                                                                pParent.remove();
                                                                //Update the cost of all total items once removed
                                                                var cartTotal = 0;
                                                                $('.cartListTotal').each(function () {
                                                                    cartTotal = cartTotal + CDec2($(this).text());
                                                                })
                                                                $('#spanCartTotal').text((cartTotal == 0 ? "0.00" : AccountingFormat(cartTotal)));
                                                                NewMessageBox('The item was successfully removed from the cart. Please do not forget to print a cancellation slip (and order slip if applicable).', '', null, {
                                                                    Close: function () {
                                                                    }
                                                                });
                                                            }
                                                        }
                                                    },
                                                    {
                                                        Text: 'No'
                                                    }
                                                ]);
                                                return false;
                                            }).data('data', vDataRow);
                                            $("#" + prodIDFormat.toUpperCase()).find('.btnListItem').bind((InAndroid ? 'tap' : 'click'), function (e) {
                                                e.preventDefault();
                                                if (!proj.HandlePosIsEditable())
                                                    return false;
                                                var pData = $(this).data('data');

                                                if (CBool(pData.HasModifier)) {
                                                    PopupButtonList('', 'Options', [
                                                        {
                                                            Text: 'Go to Stock Info',
                                                            Size: 3,
                                                            Click: function () {
                                                                sessionStorage.ItemModifier = JSON.stringify(pData.ItemModifier);
                                                                GotoStockInfoPage();
                                                            }
                                                        },
                                                        {
                                                            Text: 'Edit Modifier',
                                                            Size: 3,
                                                            Click: function () {
                                                                NewItemModifier({
                                                                    EditModifier: true,
                                                                    ProductData: pData,
                                                                    success: function (e) {
                                                                        var pItem;

                                                                        pItem = proj.HotButtonsUpdateCartStock($.extend({
                                                                            ModifierGroupID: proj.ModifierGroupID,
                                                                            IsOrderSlipPrinted: 0
                                                                        }, e), false, 0, {
                                                                            GetByGroupIdOfItemModifiers: true
                                                                        });
                                                                        proj.HotButtonDisplayStock(pItem);

                                                                        pItem = proj.HotButtonsUpdateCartStock($.extend({
                                                                            ModifierGroupID: proj.ModifierGroupID,
                                                                            IsOrderSlipPrinted: 0
                                                                        }, e), false, e.Quantity, {
                                                                            DoAdd: true
                                                                        });
                                                                        proj.HotButtonDisplayStock(pItem);
                                                                    }
                                                                });
                                                            }
                                                        }
                                                    ], { CloseButton: 'enable', AutoDismiss: true });
                                                } else {
                                                    //$("#popupLoad").popup("open");
                                                    setTimeout(function () {
                                                        GotoStockInfoPage()
                                                    }, 1000);
                                                }

                                                function GotoStockInfoPage() {
                                                    if (CStr2(sessionStorage.CustomerTableNo) != '') {
                                                        DisplayTableNumberOnButton();

                                                        $('#selTableNo').val(sessionStorage.CustomerTableNo);
                                                        $('#lblLabel').html('Customer / Table No.:<br/>');
                                                    }
                                                    if (CStr2(sessionStorage.GasPumpNo) != '') {
                                                        $('#lblCustomerName').text(sessionStorage.GasPumpNo);

                                                        $('#selGasPumpNo').val(sessionStorage.GasPumpNo);
                                                        $('#lblLabel').html('Gas Pump No.:<br/>');
                                                    }
                                                    if (CStr2(sessionStorage.GasPumpNo) != '' && CStr2(sessionStorage.CustomerTableNo) != '') {
                                                        $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                                                        $('#selTableNo').val('');
                                                        $('#lblLabel').html('Customer:<br/>');
                                                    }
                                                    startclock('#clock');
                                                    clearTimeout(proj.RetransmitToSecondScreenTimeoutHandle);
                                                    proj.SaveCart();

                                                    proj.CartStock = [];
                                                    proj.ProductID = prodIDFormat.toUpperCase();

                                                    //Making sure that modifiers are also included for POS and SO
                                                    if (!CBool(proj.POMode)) {
                                                        proj.ModifierGroupID = vDataRow.ModifierGroupID;
                                                        if (window.OfflineMode) {
                                                            if (proj.ModifierGroupID == null || proj.ModifierGroupID == '') {
                                                                proj.RebateData = null;
                                                            } else {
                                                                proj.RebateData = Business.TransInvoice.GetRebates(proj.ModifierGroupID);
                                                                if (proj.RebateData != null)
                                                                    proj.RebateData.ID = proj.RebateData.RebateID;
                                                            }
                                                        }
                                                    }
                                                    //$("#popupLoad").popup("close");
                                                    if (CStr2(proj.ProductID) != '') {
                                                        proj.ProductID = proj.ProductID.substr(0, 36);
                                                        window.NewStockBrowser = true;
                                                        startclock('#clockStock');
                                                        //ChangePage('../Stock');
                                                        Bs_ShowStockPage();
                                                    } else {
                                                        return;
                                                    }
                                                }
                                            }).data('data', pDrUnit);
                                            $("#" + prodIDFormat.toUpperCase()).find("#txtOrder").attr("id", prodIDFormat.toUpperCase() + "-Order");
                                            $("#" + prodIDFormat.toUpperCase() + "-Order")
                                                .html($('<div class="row" style="font-size:16px;">')
                                                    .append($('<div class="col-8" style="border-right: 1px solid #CCC;">')
                                                        .text(pDrUnit.ProductName + ' @ ' + AccountingFormat(parseFloat(pDrUnit.UnitPrice)) + ' each')
                                                    )
                                                    .append($('<div class="col-4 cartListTotal" style="width:60px; text-align:right;">')
                                                        .text(AccountingFormat(parseFloat(pDrUnit.UnitPrice) * parseFloat(pDrUnit.OrderedQuantity)))
                                                    )
                                                ).append($('<div class="divCartItemModifier" style="font-size:16px;">'))
                                            //.html('<label style="font-size:16px;">' + vDataRow.ProductName + '</label><br/><br/> <label style="font-size:14px;">Retail Price: ' + AccountingFormat(pDrUnit.UnitPrice) + '<br/> Item total:<span class="cartListTotal"> ' + AccountingFormat(CDec2(pDrUnit.SubTotal)) + '</span></label>');
                                            $("#" + prodIDFormat.toUpperCase()).find("#txtQtyUnit").attr("id", prodIDFormat.toUpperCase() + "-QtyUnit");
                                            $("#" + prodIDFormat.toUpperCase() + "-QtyUnit").html('<label style="font-size: 40px;">' + CDec2(proj.BomMakerMode || proj.PosMode ? pDrUnit.Quantity : pDrUnit.OrderedQuantity) + '</label><br/>' + pDrUnit.UnitMeasure);
                                            //End of edition
                                            pItemTotal = proj.DisplayStockIteModifier(pDrUnit); //salesmenu.js
                                        } else {
                                            pItemTotal = proj.DisplayStockIteModifier(pDrUnit, pDiv); //salesmenu.js
                                        }
                                        //} // for (var p = 0; p <= vData.Units.length - 1; p++) {

                                        pDiv.find('[name=spanTotal]').text(AccountingFormat(pItemTotal));
                                        mLvCart.OnItem.CartTotal += pItemTotal;


                                        if (pShowCondensedProductInformationInCart) {
                                            pLnk.append($('<img style="margin-top:10px; margin-left:10px;">')
                                                .attr('id', 'cartimg_' + vDataRow.ID)
                                                .attr('src', '../Content/images/box.png'));
                                        }
                                        pLnk.append(pDiv);
                                        pDiv.show();
                                        vItem.append(pLnk)
                                            .append($('<a>')
                                                .attr('href', '#')
                                                .data('data', vDataRow)
                                                .data('li', vItem)
                                                .click(function (e) {
                                                    if (!proj.HandlePosIsEditable())
                                                        return false;
                                                    var pDR = $(this).data('data');
                                                    var pLI = $(this).data('li');

                                                    var pDialog = NewMessageBox('Are you sure you want to remove ' + pDR.ProductName + ' from your cart?',
                                                        'Remove',
                                                        [
                                                            {
                                                                Text: 'Yes',
                                                                Click: function () {
                                                                    var pAccessText = 'Allow removal of cart items';
                                                                    var prompt = PromptUserAccess(pAccessText, {
                                                                        OnClickOKSuccess: function () {
                                                                            if (prompt)
                                                                                pDialog = prompt;
                                                                            RemoveItem();
                                                                        },
                                                                        OnFailed: function () {
                                                                            console.log('jsCFDTPD3GXC!');
                                                                        }
                                                                    });

                                                                    function RemoveItem() {
                                                                        var pUrl;
                                                                        var pData = {
                                                                            OrderID: sessionStorage.getItem('CartOrderID'),
                                                                            ProductID: pDR.ProductID,
                                                                            UnitMeasure: '', //additional by janine. blank lang, para all measure madelete
                                                                            ModifierGroupID: pDR.ModifierGroupID
                                                                        };

                                                                        if (OfflineMode) {
                                                                            pData = SalesMenu_offline().DeleteStockPos(pData);
                                                                            ShowResult(pData);
                                                                        } else {
                                                                            //////////////////                                                                                                if (proj.PosMode)
                                                                            pUrl = 'SalesMenu/DeleteStockPos';
                                                                            //////////////////                                                                                                else
                                                                            //////////////////                                                                                                    pUrl = 'SalesMenu/DeleteStock';
                                                                            AjaxSend({
                                                                                url: pUrl,
                                                                                data: pData,
                                                                                success: function (vData) {
                                                                                    ShowResult(vData);

                                                                                },
                                                                                error: function (vError) {
                                                                                    var pDummy;
                                                                                }
                                                                            }); // AjaxSend
                                                                        }; // END if (OfflineMode) {
                                                                    }

                                                                    function ShowResult(vData) {
                                                                        if (vData.Error) {
                                                                            NewMessageBox(vData.Error);
                                                                        } else {
                                                                            NewMessageBox('The item was successfully removed from the cart. Please do not forget to print a cancellation slip (and order slip if applicable).', '', null, {
                                                                                Close: function () {
                                                                                    if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall' && (location.hash != '' && sessionStorage.IsOnWidescreen == 0)) {
                                                                                        history.back();
                                                                                        history.back();
                                                                                    } else {
                                                                                        try {
                                                                                            pDialog.dialog('close');
                                                                                        } catch (e) {
                                                                                            // cannot call methods on dialog prior to initialization; attempted to call method 'close'
                                                                                        }
                                                                                        //history.back(); // dont use this; cause hang on desktop IsMobile
                                                                                    }
                                                                                    LoadCart();
                                                                                }
                                                                            });
                                                                        }
                                                                    } // END ShowResult
                                                                } // END Click: function () {
                                                            }
                                                        ,
                                                            {
                                                                Text: 'No', //Added by Janine --------------------------
                                                                Click: function () {
                                                                    if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall' && (location.hash != '' && sessionStorage.IsOnWidescreen == 0)) {
                                                                        history.back();
                                                                        history.back();
                                                                    } else {
                                                                        history.back();
                                                                    }
                                                                }
                                                            },
                                                            {
                                                                Text: 'New Cart', //Added by Janine Oct 18 5pm--------------------------
                                                                Click: function () {
                                                                    $('#butNewSalesOrder').click();
                                                                }

                                                            } //End of Added by Janine Oct 18 5pm----------------------------------------------
                                                        ]);
                                                    return false;
                                                })
                                                .text('Remove')
                                            ); // .append($('<a>')
                                    }
                                    else {
                                        ReadOnlyCart(vDataRow, vItem, e, mLvCart.vData.Units.Rows);
                                    } // if (CBool(sessionStorage.IsDraft))

                                }; // mLvCart.OnItem
                            };  // if (mLvCart == undefined) {
                            mLvCart.vData = vData;
                            mLvCart.OnItem.CartTotal = 0;
                            //mLvCart.SetData(vData.Units.Rows); // from Arvin: mLvCart.SetData(vData.Stocks.Rows);                                        

                            var divCartRetail = $('#divCartRetail'), carttotal = 0;
                            divCartRetail.empty();
                            for (var i = 0; i < vData.Units.Rows.length; i++) {
                                var vRows = vData.Units.Rows[i];

                                if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {
                                    proj.HotButtonDisplayStock(vRows);
                                    continue;
                                }

                                var li = $('<li data-role="none" class="list-group-item list-group-item-action" aria-current="true" style="padding:0px !important;">');
                                var table = $('<table style="width:100%;">'), tr = $('<tr>'), td = $('<td>');
                                var proddesc, qty = $.format.number((proj.PosMode || proj.BomMakerMode) ? CDec2(vRows.Quantity) : CDec2(vRows.OrderedQuantity), proj.QuantityFormat);

                                if (pShowCondensedProductInformationInCart)
                                    proddesc = vRows.ProductName;
                                else
                                    proddesc = vRows.ProductNo + ' ' + vRows.ProductName;

                                tr.append($('<td style="width:100px">')
                                        .append($('<img style="width:100px;">')
                                            .attr('id', 'cartimg_' + vRows.ID)
                                            .attr('src', '../Content/images/box.png')
                                        )
                                    ).append($('<td style="padding:10px;">')
                                        .append($('<b>').text(proddesc))
                                        .append('<br><br>')
                                        .append($('<div style="font-weight:bold;">').html(qty
                                                + ' @ ' +
                                                vRows.UnitPrice + ' / ' + vRows.UnitMeasure + ' ' +
                                                AccountingFormat(CDec2(qty) * CDec2(vRows.UnitPrice))
                                            )
                                        )
                                        .append('<div class="divCartItemModifier" style="font-size:16px;">')
                                        .append('<div class="divItemTotal" style="font-weight:bold;">')
                                        .data('data', vRows)
                                        .click(function (e) {
                                            var pData;
                                            if (!proj.HandlePosIsEditable())
                                                return false;
                                            e.preventDefault();
                                            pData = $(this).data('data');
                                            proj.ProductID = pData.ProductID;
                                            //proj.ModifierGroupID = pData.ModifierGroupID; commented by arnie (472017) para walay masave sa table TransInvoiceItemsRebates. see function SaveModifiers in Stock_offline.js
                                            if (CBool(pData.HasModifier)) {
                                                PopupButtonList('', 'Options', [
                                                    {
                                                        Text: 'Go to Stock Info',
                                                        Size: 3,
                                                        Click: function () {
                                                            proj.GroupIdOfItemModifiers = pData.GroupIdOfItemModifiers;
                                                            sessionStorage.ItemModifier = JSON.stringify(pData.ItemModifier);
                                                            GotoStockInfoPage();
                                                        }
                                                    },
                                                    {
                                                        Text: 'Edit Modifier',
                                                        Size: 3,
                                                        Click: function () {
                                                            NewItemModifier({
                                                                EditModifier: true,
                                                                AllowSKUQty: true,
                                                                ProductData: pData,
                                                                success: function (e) {
                                                                    //proj.GroupIdOfItemModifiers = pData.GroupIdOfItemModifiers;
                                                                    //sessionStorage.ItemModifier = JSON.stringify(e.ItemModifier);
                                                                    //GotoStockInfoPage();
                                                                    Android.Toast('Updating Item ... Please wait');
                                                                    //$("#popupLoad").popup("open");
                                                                    setTimeout(function () {
                                                                        proj.CartStock = [e];
                                                                        proj.SaveCart();
                                                                        Display(SalesMenu_offline().PosCart({ OrderID: sessionStorage.CartOrderID }));
                                                                        //$("#popupLoad").popup("close");
                                                                    }, 200)
                                                                }
                                                            });
                                                        }
                                                    }
                                                ], { CloseButton: 'enable', AutoDismiss: true });
                                            } else {
                                                proj.GroupIdOfItemModifiers = null;
                                                GotoStockInfoPage()
                                            }

                                            function GotoStockInfoPage() {
                                                if (window.OfflineMode)
                                                    if (proj.ModifierGroupID == null)
                                                        proj.RebateData = null;
                                                    else {
                                                        proj.RebateData = Business.TransInvoice.GetRebates(proj.ModifierGroupID);
                                                        proj.RebateData.ID = proj.RebateData.RebateID;
                                                    }
                                                sessionStorage.IsInventoryCountEditSKUMode = true;
                                                startclock('#clockStock');
                                                Bs_ShowStockPage();
                                                return false;
                                            }
                                        })
                                    ).append($('<td style="width:50px; text-align:center; font-size:20px; color:#FFF;" class="bg-danger">')
                                        .append('<span class="bi-trash"></span>')
                                        .data('data', vRows)
                                        .click(function (e) {
                                            if (!proj.HandlePosIsEditable())
                                                return false;
                                            var pDR = $(this).data('data');

                                            var pDialog = NewMessageBox('Are you sure you want to remove ' + pDR.ProductName + ' from your cart?',
                                                'Remove',
                                                [
                                                    {
                                                        Text: 'Yes',
                                                        Click: function () {
                                                            var pAccessText = 'Allow removal of cart items';
                                                            var prompt = PromptUserAccess(pAccessText, {
                                                                OnClickOKSuccess: function () {
                                                                    if (prompt)
                                                                        pDialog = prompt;
                                                                    RemoveItem();
                                                                },
                                                                OnFailed: function () {
                                                                    console.log('jsCFDTPD3GXC!');
                                                                }
                                                            });

                                                            function RemoveItem() {
                                                                var pUrl;
                                                                var pData = {
                                                                    OrderID: sessionStorage.getItem('CartOrderID'),
                                                                    ProductID: pDR.ProductID,
                                                                    UnitMeasure: '', //additional by janine. blank lang, para all measure madelete
                                                                    ModifierGroupID: pDR.ModifierGroupID
                                                                };

                                                                if (OfflineMode) {
                                                                    pData = SalesMenu_offline().DeleteStockPos(pData);
                                                                    ShowResult(pData);
                                                                } else {
                                                                    //////////////////                                                                                                if (proj.PosMode)
                                                                    pUrl = 'SalesMenu/DeleteStockPos';
                                                                    //////////////////                                                                                                else
                                                                    //////////////////                                                                                                    pUrl = 'SalesMenu/DeleteStock';
                                                                    AjaxSend({
                                                                        url: pUrl,
                                                                        data: pData,
                                                                        success: function (vData) {
                                                                            ShowResult(vData);

                                                                        },
                                                                        error: function (vError) {
                                                                            var pDummy;
                                                                        }
                                                                    }); // AjaxSend
                                                                }; // END if (OfflineMode) {
                                                            }

                                                            function ShowResult(vData) {
                                                                if (vData.Error) {
                                                                    NewMessageBox(vData.Error);
                                                                } else {
                                                                    LoadCart();
                                                                    //NewMessageBox('The item was successfully removed from the cart. Please do not forget to print a cancellation slip (and order slip if applicable).', '', null, {
                                                                    //    Close: function () {
                                                                    //        if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall' && (location.hash != '' && sessionStorage.IsOnWidescreen == 0)) {
                                                                    //            history.back();
                                                                    //            history.back();
                                                                    //        } else {
                                                                    //            try {
                                                                    //                pDialog.dialog('close');
                                                                    //            } catch (e) {
                                                                    //                // cannot call methods on dialog prior to initialization; attempted to call method 'close'
                                                                    //            }
                                                                    //            //history.back(); // dont use this; cause hang on desktop IsMobile
                                                                    //        }
                                                                    //        LoadCart();
                                                                    //    }
                                                                    //});
                                                                }
                                                            } // END ShowResult
                                                        } // END Click: function () {
                                                    }
                                                ,
                                                    {
                                                        Text: 'No', //Added by Janine --------------------------
                                                        Click: function () {
                                                            //if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall' && (location.hash != '' && sessionStorage.IsOnWidescreen == 0)) {
                                                            //    history.back();
                                                            //    history.back();
                                                            //} else {
                                                            //    history.back();
                                                            //}
                                                        }
                                                    },
                                                    {
                                                        Text: 'New Cart', //Added by Janine Oct 18 5pm--------------------------
                                                        Click: function () {
                                                            $('#butNewSalesOrder').click();
                                                        }

                                                    } //End of Added by Janine Oct 18 5pm----------------------------------------------
                                                ]);
                                            return false;
                                        })
                                    )

                                var itemtotal = proj.DisplayStockIteModifier(vRows, tr);
                                carttotal += itemtotal;
                                tr.find('.divItemTotal').text('Item Total: ' + AccountingFormat(itemtotal));

                                table.append(tr);
                                li.append(table);
                                divCartRetail.append(li);
                            }

                            if ((window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall' || window.location.pathname.toLowerCase() == '/tableview/bsnewtable')) {
                                $('.cartListTotal').each(function () {
                                    carttotal = carttotal + CDec2($(this).text());
                                    $('#spanCartTotal').text(AccountingFormat(carttotal));
                                });
                            }

                            if (CBool(sessionStorage.IsDraft)) {
                                $('#spanCartTotal').text(AccountingFormat(carttotal));
                                $('#spanSkuCount').text(AccountingFormat(vData.Stocks.Rows.length, 0));
                                //additional by janine
                                $('#divCartPanel').show();

                                $('#divCartEmpty').hide();
                                $('#divCartSection').show();
                                if (window.location.hash.substring(1).toLowerCase() == 'cart')
                                    $('[data-href=Nav1d]').click();
                                GotoFirstPage();
                            } else {
                                $('#spanCartTotal').text(AccountingFormat(carttotal));
                                $.mobile.changePage('#pageViewTransactions');
                                startclock('#clockTransactionDetails');
                                $('#divContentTransPage').css('margin-top', $('#divHeaderTransPage').height() - 20 + 'px');
                                $('#pageViewTransactions').bind('pageinit', function () {
                                    $('#pageViewTransactions').trigger('create');
                                })
                            }

                            if (pShowCondensedProductInformationInCart && !($(window).width() > proj.WideWidth)) { // no pics in HB w/ sidecart
                                AjaxSend({
                                    url: 'Organization/GetBulkPIMPictures',
                                    data: { data: vData.Units.Rows, size: 'medium', IsDefault: true },
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
                                        //var sourceimg = (window.DebugMode ? location.origin : 'http://119.93.147.163');
                                        var sourceimg = Android.GetServer();

                                        for (var i = 0; i < vRows.length; i++) {
                                            if (vRows[i].Pics != '') {
                                                vRows[i].Ctr = 1;
                                                var pSrc = sourceimg + 'Content/MobizImagePIM/' + sessionStorage.OrgID + '/Medium/' + vRows[i].Pics[0].FileName;

                                                $('#cartimg_' + vRows[i].ID).attr('src', pSrc)
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
                                                                if (e == null) {
                                                                    console.warn('e is null');
                                                                    return; // silent error lang
                                                                }

                                                                if (e.Error) {
                                                                    console.warn(e.Error);
                                                                    return; // silent error lang
                                                                }
                                                                vRows = e.data[0].Pics;

                                                                var div = $('#divCartImage');

                                                                if ($('#skupicscart').length == 0) {
                                                                    div.append($('<div id="skupicscart" style="display:none;">'));
                                                                } else {
                                                                    $('#skupicscart').empty();
                                                                }

                                                                div = $('#skupicscart');

                                                                for (var i = 0; i < vRows.length; i++) {
                                                                    vRows[i].Ctr = i + 1;
                                                                    vRows[i].ID = data.ID;
                                                                    vRows[i].ProductName = data.ProductName;
                                                                    var srcBig = sourceimg + 'Content/MobizImagePIM/' + sessionStorage.OrgID + '/Big/' + vRows[i].FileName;

                                                                    div.append($('<a data-role="none" data-lightbox="skupicturescart">')
                                                                                .attr('id', 'piccart_' + vRows[i].ID)
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
                                                                var firstpic = $('#piccart_' + vRows[0].ID);
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
                                                $('#cartimg_' + vRows[i].ID).attr('src', '../Content/images/box.png')
                                            }
                                        }
                                    },
                                    error: function (e) {
                                        Android.AddLog('Cart SKU pics: Can\'t connect to the server, SKU image not successfully loaded');
                                    }
                                })
                            }
                        }


                        if (window.location.pathname.toLowerCase() == '/billofmaterials')
                            $('[name=lblItemTotal]').hide();

                        return true;
                    }; // Display

                }; // function LoadCart()

                proj.ShowCart = function (vDontFetchIfBlank) {
                    Bs.Loading('hide');
                    sessionStorage.PosMode = 'Retail Mode';
                    startclock('#clock');
                    LoadCart(vDontFetchIfBlank);

                    if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall')
                        window.pageHeader.LoadHotButtons();
                    else if (proj.SalesMenuPage != undefined) {
                        sessionStorage.removeItem('IsHotButtonVisited');
                        $('#pageMain').pageshow();
                        //$.mobile.changePage(proj.SalesMenuPage); // dont use GetWindowPathFormat here
                    }
                    proj.RetransmitToSecondScreen();
                    //                    $('[data-href=Nav1d]').click();
                }; // proj.ShowCart = function() {                           

                function lnkSalesOrder_Click(e, vParams) {

                    vParams = $.extend({
                        SaveAsNew: false,
                        UserID: null
                    }, vParams);

                    var pBrowser;
                    //additional by janine
                    //$('#divRecentlyBoughtPanel').hide();
                    $('#divCartPanel, #pageMain').pageshow();

                    if (proj.PosMode) {
                        if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall')
                            $('#lblPayNow').html('Go');
                        else {
                            if ($.isFunction(GetStringObject('GetIsOrderTaker', mOrderTakerSetupModel)))
                                if (mOrderTakerSetupModel.GetIsOrderTaker())
                                    $('#lblPayNow').html('Go');
                                else
                                    $('#lblPayNow').html('Pay Now');
                            else
                                $('#lblPayNow').html('Pay Now');
                        }

                    } else {
                        $('#lblPayNow').html('Save Sales Order');
                    }

                    isOnCartPage = true;

                    //e.preventDefault();
                    if (sessionStorage.CustomerOrganizationID == undefined) {
                        NewMessageBox('Please select a customer first');
                        return;
                    };

                    PromptUserAccess('Recall cart', {
                        OnClickOKSuccess: function () {
                            BrowsePopup('TransInvoices', (proj.POMode ? 'Purchase Order' : 'Sales Invoices'), [
                            {
                                Offline: true,
                                AutoSearch: true,
                                DisplayText: [
                                    {
                                        Label: 'Sales Order No.',
                                        Column: 'SearchSO'
                                    },
                                    {
                                        Label: 'Customer',
                                        Column: 'CompanyName'
                                    },
                                    {
                                        Label: 'Date',
                                        Column: function (e) {
                                            return $.format.date(new Date(e.InvoiceDate), 'MMM dd, yyyy hh:mm a');
                                        }
                                    },
                                    {
                                        Label: 'FOWST',
                                        Column: 'FOWSTName'
                                    },
                                    {
                                        Label: 'Device',
                                        Column: 'DeviceName'
                                    },
                                    {
                                        Label: 'Total Value',
                                        Column: function (e) {
                                            return AccountingFormat(e.Total);
                                        }
                                    },
                                    {
                                        Label: 'SKU Count',
                                        Column: 'SkuCount'
                                    },
                                    {
                                        Label: 'Created By',
                                        Column: 'EmployeeFullName'
                                    }
                                ],
                                Click: function (e) {
                                    var IsDraft;

                                    proj.SaveCart();
                                    proj.NewCartPos();

                                    if (!IsGuid(sessionStorage.CustomerOrganizationID))
                                        sessionStorage.CustomerOrganizationID = e.CustomerID;

                                    if (e.IsDraft == undefined) {
                                        IsDraft = 1;
                                    }
                                    else {
                                        IsDraft = e.IsDraft;
                                    }

                                    arrPODetails = e;
                                    sessionStorage.IsDraft = CInt2(IsDraft);

                                    if (e.UserID != e.CurrentUserID) {
                                        NewMessageBox("Warning : This cart was created by " + e.EmployeeFullName + ". Do you want to continue?",
                                        'Recall Cart',
                                        [
                                            {
                                                Text: 'Yes',
                                                Click: function () {
                                                    LoadData();
                                                }
                                            }
                                        ,
                                            {
                                                Text: 'No',
                                                Click: function () {
                                                    GotoFirstPage();
                                                    return;
                                                }
                                            }
                                        ]
                                    );
                                    } else { // ELSE if (e.UserID != e.CurrentUserID) {
                                        LoadData();
                                    } // END if (e.UserID != e.CurrentUserID) {

                                    function LoadData() {
                                        if (vParams.SaveAsNew) {
                                            sessionStorage.removeItem('CartOrderID');
                                        } else {
                                            sessionStorage.setItem('CartOrderID', e.ID);

                                            //Added By Francis to load Customer Info
                                            sessionStorage.CustomerOrganizationName = e.CompanyName;

                                            //additional by janine oct 27 for demographics
                                            sessionStorage.DeliveryMethod = e.DeliveryMethod;
                                            sessionStorage.OrderMethod = e.OrderMethod;
                                            sessionStorage.Gender = e.CustomerDemographicGender;
                                            sessionStorage.Age = e.CustomerDemographicAge;
                                            sessionStorage.TotalNumberOfCustomer = e.TotalNumberOfCustomer;
                                        }

                                        //
                                        //put condition if table no or gas pump no if GP or TBL

                                        if (CStr2(e.TableNo) != '') {
                                            var prefix = e.TableNo.split('_');
                                            if (prefix[0] == 'GP')
                                                sessionStorage.GasPumpNo = prefix[1];
                                            else
                                                sessionStorage.CustomerTableNo = prefix[1];
                                        } else {
                                            sessionStorage.GasPumpNo = '';
                                            sessionStorage.CustomerTableNo = '';
                                        }
                                        //                            pageCustomerDetails.setCustomerInfo();
                                        $('#btnClient').data('id', e.CustomerID);


                                        //additional by janine sept 5 2014 
                                        if (proj.PosMode) { // && CDec2(e.SalesOrderNo) > 0
                                            UpdateOrderedQuantity();
                                        } else {
                                            $('#lstROPOItems').empty();

                                            isRecallCart = true;
                                            LoadCart(false, {
                                                SaveAsNew: vParams.SaveAsNew,
                                                ID: e.ID
                                            });

                                            if (CBool(sessionStorage.IsDraft)) {
                                                GotoFirstPage();
                                            }
                                        }
                                        //end of additional
                                    } // END function LoadData() {
                                }
                            }
                            ], {
                                PoMode: CBool(proj.POMode),
                                SearchOnline: CBool(vParams.SaveAsNew),
                                UserID: (vParams.UserID || undefined)
                            })
                        }
                    });
                } // END function lnkSalesOrder_Click

                function DisplaySalesOrder(vDR) {
                    //                var pDate;

                    sessionStorage.setItem('CartOrderID', vDR.ID);
                    //////////////////////                                        if (proj.PosMode)
                    $('#lnkSalesOrder').find('[name=divNumber]').html('');
                    //////////////////////                                        else
                    //////////////////////                                            $('#lnkSalesOrder').find('[name=divNumber]').html('Sales Order No.: ' + HtmlEncode(vDR.OrderNo) + '&#x25BC;');
                    $('#lnkSalesOrder').find('[name=divDate]').text('Recall Cart'); // Edited by Janine Oct 19 2pm //Last updated: ' + $.format.date(new Date(vDR.LastUpdated), 'MMM d, yyyy h:mm a)
                    $('#txtRemarks').val(vDR.Remarks);

                    //                pDate = new Date(vDR.LastUpdated);
                    //                pDate.setSeconds(pDate.getSeconds() + 1); // add 1 second
                    $('#txtDate').DateCombo('val', vDR.LastUpdated);
                    $('#txtRefNum').val(vDR.RefNum);
                    $('#cbReasonForReconstructingData')
.val(vDR.ReasonForReconstructingData)
.selectmenu('refresh', true);
                }; // DisplaySalesOrder

                function ReadOnlyCart(vDataRow, vItem, e, poItems) {
                    $('#txtROPONo').text('PO No.: ' + arrPODetails.SearchSO);
                    $('#txtRODatePurchase').text($.format.date(new Date(arrPODetails.InvoiceDate), 'MMMM dd, yyyy HH:mm'));
                    $('#txtROCustomer').text(arrPODetails.FOWSTName);
                    $('#txtROStatus').text((CBool(arrPODetails.IsDraft) == 1 ? 'Draft' : 'Finalized'));
                    $('#txtSubTotal').text(AccountingFormat(arrPODetails.TotalOrder))

                    var pLV = $('#lstROPOItems');
                    var pLVLi = $('<li data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="d" class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li ui-first-child ui-last-child ui-btn-up-d">');
                    var pLVA = $('<a>');
                    var pLVData = "";
                    var pItemTotal = 0;

                    for (var i = 0; i < poItems.length; i++) {
                        if (poItems[i].ID.toLowerCase() == vDataRow.ProductID.toLowerCase()) {
                            pLVData += '<tr>' +
                                    '<td>' + CDec2(poItems[i].OrderedQuantity) + '</td>' +
                                    '<td> &nbsp; &nbsp;' + (poItems[i].UnitMeasure == undefined ? '' : poItems[i].UnitMeasure) + '</td>' +
                                    '<td> &nbsp; &nbsp;' + AccountingFormat(poItems[i].UnitPrice) + '</td>' +
                                    '<td> &nbsp; &nbsp;' + AccountingFormat(poItems[i].SubTotal) + '</td>' +
                                    '</tr>';
                            pItemTotal += CDec2(poItems[i].SubTotal);
                        }
                    }

                    pLV.append(pLVLi
                                .append($('<div style="background-color:#F2F2F2 !important;">')
                                    .addClass('ui-btn-inner ui-li ui-li-static ui-btn-up-d')
                                    .css('background-color', '#F2F2F2 !important')
                                    .append($('<div>')
                                        .addClass('ui-btn-text')
                                        .append(pLVA
                                            .html(vDataRow.ProductNo + ' ' + vDataRow.ProductName
                                                    + '<br><br>'
                                                    + '<table>' + pLVData + '</table>' +
                                                    '<br> Item Total: &nbsp; &nbsp; &nbsp; ' + AccountingFormat(pItemTotal)
                                            )
                                        )
                                    )
                                    .append($('<span class="ui-icon ui-icon-arrow-r ui-icon-shadow">'))
                                )
                    )

                    pLV.trigger('create')
                }

                function NewCart() {

                    mPrePos.NewCart();
                    return;

                    localStorage.NewCartMode = 1;
                    mOldCartID = sessionStorage.getItem('CartOrderID');
                    sessionStorage.removeItem('CartOrderID');

                    //additional by janine
                    sessionStorage.removeItem('InvoiceProductIDList');
                    proj.ProductID = '';
                    sessionStorage.LastQty = 1;
                    sessionStorage.ProductName = '';
                    sessionStorage.ProductUnitQty = 0;
                    sessionStorage.ProductUnitMeasure = '';
                    sessionStorage.ProductPrice = 0;
                    sessionStorage.removeItem('SupplierOrganizationID');
                    sessionStorage.removeItem('SupplierOrganizationName');
                    sessionStorage.removeItem('SupplierFowstID');
                    sessionStorage.removeItem('SupplierFowstName');
                    sessionStorage.removeItem('SupplierBinID');
                    sessionStorage.removeItem('SupplierBinName');
                    if (proj.POMode)
                        sessionStorage.removeItem('CustomerOrganizationID'); // clear only if PO
                    sessionStorage.removeItem('CustomerOrgName');
                    sessionStorage.removeItem('CustomerAddress');
                    sessionStorage.removeItem('CustomerFowstID');
                    // sessionStorage.removeItem('FowstID'); // dont clear FowstID
                    sessionStorage.removeItem('CustomerFowstName');
                    sessionStorage.removeItem('CustomerBinID');
                    sessionStorage.removeItem('CustomerBinName');

                    //                    sessionStorage.PosMode = 'Retail Mode'; //temporarily set retail mode as default
                    // sessionStorage.IsHotButtonVisited = 'false';
                    if (CStr2(sessionStorage.IsEnableDiningMode) == 'true')
                        sessionStorage.DeliveryMethod = 'Dine In';
                    else
                        sessionStorage.DeliveryMethod = 'PickUp';

                    sessionStorage.OrderMethod = 'Walk In';
                    sessionStorage.Gender = 'NA';
                    sessionStorage.Age = 'Mixed Ages/ NA';
                    //                    ClearCustomerFreeTypeSession();

                    $('#txtOrder').html('<label style="font-size:16px;">Your cart is empty</label>');
                    $('#txtQtyUnit').html('');

                    $('#divCartPanel').show();

                    $('#divCartSection').hide();
                    $('#divCartEmpty').show();
                    window.popSalesMenuOrderTaker.Clear();

                    //Inserted by Francis A. to load the Invoice Details from Sales Payment due to Overpayment of Check 
                    //09/18/2013
                    if (sessionStorage.CartOnHoldCheckPayment != undefined) {
                        if (sessionStorage.CartOnHoldCheckPayment == 1 && sessionStorage.CartOnHoldCheckPaymentOrderID) {
                            sessionStorage.setItem('CartOrderID', sessionStorage.CartOnHoldCheckPaymentOrderID);
                            proj.ShowCart();
                        }
                    }
                    //End Inserted by Francis A. ------------------------
                    $('#spanCartTotal').text('0.00'); //additional by janine nov 11

                    if (proj.POMode) {
                        $('#lblCustomerName, #lblSupplierName').text('Supplier');
                        $('#lblCurrentPOSMode').text('Customer');
                    }
                    startclock('#clock');
                };

                //additional by janine sept 5 2014 for sales order taker
                function UpdateOrderedQuantity() {

                    var pData = {};
                    pData.InvoiceID = sessionStorage.getItem('CartOrderID');

                    if (OfflineMode) {

                        var vData = Stock_offline().UpdateOrderedQuantity(sessionStorage.getItem('CartOrderID'));
                        if (vData.Result == 'ok') {
                            LoadCart();
                        } else {
                            NewMessageBox(vData.Result);
                        }
                    } else {
                        AjaxSend({
                            url: 'SalesMenu/UpdateOrderedQuantity',
                            data: pData,
                            success: function (vData) {
                                LoadCart();
                            },
                            error: function (vError) {
                                NewMessageBox(vError);
                            }
                        });

                    }

                }

                //end of additional

                $('#btnSalesMenuLogin').click(function (e) {
                    e.preventDefault();

                    window.open(GetPathFormat("/People"), "_self");

                });

                $('#butHotButtonsShortcut').click(function () {
                    var pMode;

                    if (OfflineMode) {
                        if ($(window).width() > proj.WideWidth) { // 932 old . force non-widescreen
                            //The purpose of this IF function is to check if there
                            //are items on the cart when it was on retail mode.
                            //If there were items, then those items should be retained.
                            if (sessionStorage.getItem('CartOrderID')) {
                                localStorage.setItem('tempData', JSON.stringify(proj.CartData));
                                localStorage.setItem('tempData2', JSON.stringify(proj.RebateData));
                                localStorage.setItem('tempModifierID', proj.ModifierGroupID);
                                //If SideCart is in Purchase Order Mode, supplier information must be retained
                                if (!(CBool(proj.PosMode))) {
                                    localStorage.setItem('tempSupplierOrganizationID', sessionStorage.SupplierOrganizationID);
                                    localStorage.setItem('tempSupplierOrganizationName', sessionStorage.SupplierOrganizationName);
                                    localStorage.setItem('tempSupplierAddress', sessionStorage.SupplierAddress);
                                    localStorage.setItem('tempSupplierPriceLevelID', sessionStorage.SupplierPriceLevelID);
                                    localStorage.setItem('tempSupplierPriceLevelName', sessionStorage.SupplierPriceLevelName);
                                    localStorage.setItem('tempBillingAddressID', sessionStorage.BillingAddressID);
                                    localStorage.setItem('tempDeliveryAddressID', sessionStorage.DeliveryAddressID);
                                    localStorage.setItem('tempDeliveryDate', sessionStorage.DeliveryDate);
                                }
                            }
                            if (CBool(proj.PosMode))
                                pMode = 'pos';
                            else if (CBool(proj.SOMode))
                                pMode = 'so';
                            else
                                pMode = 'po';
                            location = MergeUrlParameters({
                                OrgID: sessionStorage["OrgID"],
                                mode: pMode,
                                IsMobile: IsMobile ? 1 : 0,
                                theme: 'bs'
                            }, '/SalesMenu/BsPOSViewAll');
                        } else
                            goToHotButtonPage();
                    } else // ELSE if (OfflineMode) {
                        goToHotButtonPage();

                });

                $('#butBarCode').click(function () {
                    if ((CBool(window.IsMobile)) && (!CBool(window.InWindows))) {
                        mobile.Scan({
                            Success: function (vData) {
                                $('#txtSearch').val(vData.Value);
                                Search();
                            }
                        });
                    } else {
                        NewMessageBox('Scan Barcode / QR Code feature is only available on the mobile app',
                        'Scan Barcode / QR Code',
                        [
                            { Text: 'Ok' }
                        ]
            );
                    }
                    // old code
                    //                window.plugins.barcodeScanner.scan(function (result) {
                    //                    $('#txtSearch').val(result.text);
                    //                    Search();
                    //                }, function (error) {
                    //                    alert("Scanning failed: " + error);
                    //                });

                });

                $('#butSaveRemarks').click(function () {

                    var pUrl;
                    var pData;

                    //////////////////////                                        if (proj.PosMode) {
                    pUrl = 'SalesMenu/SaveRemarksPos';
                    pData = { InvoiceID: sessionStorage.getItem('CartOrderID') };
                    //////////////////////                                        } else {
                    //////////////////////                                            pUrl = 'SalesMenu/SaveRemarks';
                    //////////////////////                                            pData = { OrderID: sessionStorage.getItem('CartOrderID') };
                    //////////////////////                                        };
                    pData.Remarks = $('#txtRemarks').val();

                    AjaxSend({
                        url: pUrl,
                        data: pData,
                        success: function (vData) {
                            if (vData == undefined) return;
                            if (vData.Remarks == 'ok') {
                                NewMessageBox('Remarks was successfully saved', 'Success');
                            } else if (vData.Error != undefined) {
                                NewMessageBox(vData.Error, 'Error');
                            };
                        },
                        error: function (vError) {
                            var pDummy;
                        }
                    });

                }); // $('#butSaveRemarks').click(function() {

                $('#lnkHold').click(function () {

                    var pUrl;
                    var pData;

                    //////////////////////                                        if (proj.PosMode) {
                    pUrl = 'SalesMenu/HoldPos';
                    pData = { InvoiceID: sessionStorage.getItem('CartOrderID') };
                    //////////////////////                                        } else {
                    //////////////////////                                            pUrl = 'SalesMenu/Hold';
                    //////////////////////                                            pData = { OrderID: sessionStorage.getItem('CartOrderID') };
                    //////////////////////                                        };

                    AjaxSend({
                        url: pUrl,
                        data: pData,
                        success: function (vData) {
                            if (vData == undefined) return;
                            if (vData.Result == 'ok') {
                                sessionStorage.removeItem('CartCustomerOrganizationID');
                                //////////////////////                                                                if (proj.PosMode) {
                                $('#lnkSalesOrder [name=divNumber]').html('Recall Cart &#x25BC;');
                                //////////////////////                                                                } else {
                                //////////////////////                                                                    $('#lnkSalesOrder [name=divNumber]').html('Recall Cart &#x25BC;');
                                //////////////////////                                                                };
                                $('#btnClient')
        .val('')//.html('Please choose a customer')
        .data('id', null);
                                $('#lnkSalesOrder').find('[name=divDate]').text('');

                                //additional by janine
                                $('#divCartPanel').show();

                                $('#divCartSection').hide();
                                $('#divCartEmpty').show();

                                NewMessageBox('Transaction was held successfully', 'Success', null, {});
                            } else if (vData.Error != undefined) {
                                NewMessageBox(vData.Error, 'Error');
                            };
                        }
                    });

                }); // $('#lnkHold').click(function() {

                $('#lnkProceedToPayments').click(function () {
                    //$('#popupPayNowMode').popup('close');
                    if (isCartEmpty) {

                        NewMessageBox('Your shopping cart is empty',
                            'Pay Now',
                            [
                                { Text: 'Ok' }
                            ]
                        );
                        return;

                    } else {
                        sessionStorage.CurrentPage = 'payment';
                        sessionStorage.IsViewBill = 'false';
                        //                        if (CStr2(sessionStorage.IsPaymentJSLoaded) == 'true')
                        //                            window.pageHeader.ShowPaymentsPage(true);
                        //                        else
                        proj.SalesMenu.PaymentsFunction({
                            DontPopupBilling: true
                        });
                    }
                });

                $('#lnkBackToTableView').click(function () {
                    location = MergeUrlParameters({
                        OrgID: sessionStorage["OrgID"],
                        mode: 'pos',
                        IsMobile: IsMobile ? 1 : 0
                    }, '/TableView/NewTable');
                    //location.reload();

                });

                function ShouldAddClickEvent() {
                    try {
                        var pElements = $('#lnkCheckout');
                        if (pElements.length == 0)
                            return false;
                        var pEvents = $._data(pElements[0], 'events');
                        var pHasClick = false;

                        if (pEvents == null)
                            return true;
                        $.each(pEvents, function (vKey) {
                            if (vKey == 'click') {
                                pHasClick = true;
                                return false;
                            }
                        });
                        return !pHasClick;
                    } catch (e) {
                        console.error(e);
                    }
                    return true;
                }

                if (ShouldAddClickEvent())
                    $('#lnkCheckout:not(.bound)').addClass('bound').click(function () {
                        function PaymentsFunction() {
                            proj.SalesMenu.PaymentsFunction.apply(this, arguments);
                        }

                        App.controller.PrePos().CheckIfNoUnflushedDataFromPrevDays({
                            success: function () {
                                if (!IsGuid(sessionStorage.CartOrderID)) {
                                    NewMessageBox('Cart is empty');
                                    return;
                                }

                                if (!proj.SOMode && (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall')) {
                                    //$("#popupLoad").popup("open");
                                    setTimeout(function () {
                                        if (CStr2(sessionStorage.CustomerTableNo) != '') {
                                            DisplayTableNumberOnButton();
                                            $('#selTableNo').val(sessionStorage.CustomerTableNo);
                                            $('#lblLabel').html('Customer / Table No.:<br/>');
                                        }
                                        if (CStr2(sessionStorage.GasPumpNo) != '') {
                                            $('#lblCustomerName').text(sessionStorage.GasPumpNo);
                                            $('#selGasPumpNo').val(sessionStorage.GasPumpNo);
                                            $('#lblLabel').html('Gas Pump No.:<br/>');
                                        }
                                        if (CStr2(sessionStorage.GasPumpNo) != '' && CStr2(sessionStorage.CustomerTableNo) != '') {
                                            $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                                            $('#selTableNo').val('');
                                            $('#lblLabel').html('Customer:<br/>');
                                        }
                                        startclock('#clock');
                                        proj.SaveCart();
                                        PaymentsFunction();
                                        //$("#popupLoad").popup("close");
                                    }, 1000);
                                } else {
                                    PaymentsFunction();
                                }
                            }
                        })
                    }); // lnkCheckout.click


                function SaveSalesOrder(vParams) {
                    var pData;
                    var pResult;

                    vParams = $.extend({}, vParams);

                    ////////////////////                    if (proj.PosMode) {
                    if (proj.ReconstructSalesDataMode) {
                        if (!Verify())
                            return;
                        else {
                            pData = {
                                InvoiceID: sessionStorage.getItem('CartOrderID'),
                                DateTime: $.format.date($('#txtDate').DateCombo('val'), 'M/d/yyyy h:mm:ss a'), // format for printing and remove timezone
                                RefNum: $('#txtRefNum').val().trim(),
                                ReasonForReconstructingData: $('#cbReasonForReconstructingData').val()
                            };

                            if (OfflineMode) {
                                pResult = SalesMenu_offline().SaveRsd(pData);
                                HandleResult(pResult);
                            } else { // if OfflineMode
                                AjaxSend({
                                    url: '/SalesMenu/SaveRsd',
                                    data: pData,
                                    success: function (vData) {
                                        HandleResult(vData);


                                    }
                                }); // END AjaxSend
                            } // END if OfflineMode

                            function HandleResult(vData) {
                                vData = $.extend({}, vData);
                                if (vData.Result == 'ok') {
                                    NewMessageBox('Invoice data was successfully updated', '', null, {
                                        OnClose: function () {
                                            proj.SalesMenu.GoToPaymentsPage();
                                        }
                                    });
                                    return;
                                };
                                if (vData.Error)
                                    NewMessageBox(vData.Error);
                                else
                                    NewMessageBox('Unknown error');

                            } // END HandleResult
                        };
                    } else {
                        if (proj.PosMode) {
                            if (OfflineMode) {
                                if ((!vParams.DontPopupBilling) && CBool(Business.SystemDefaults.IsEnableBillingOption())) {
                                    //$('#popupPayNowMode').popup('open');
                                } else {
                                    proj.SalesMenu.GoToPaymentsPage();
                                }
                            } else {
                                //if online mode
                                proj.SalesMenu.GoToPaymentsPage();
                            }
                        } else {
                            pData = {
                                InvoiceID: sessionStorage.getItem('CartOrderID')
                            };

                            if (OfflineMode) {

                                popSalesMenuOrderTaker.OnSubmit = function (vData) {
                                    var pResult2;

                                    $('#divCollapse').trigger('collapse');


                                    if (Business.SystemDefaults.GetSystemDefault("Require Signature in Sales Order Taker") == '1') {
                                        require(['promise!/Scripts3/controller/SignaturePage.js?crc='], function () {
                                            var mSignaturePage = new App.controller.SignaturePage();

                                            mSignaturePage.onSubmit = function (vDataSignature) {
                                                //store image url in txt file
                                                Transfers.WriteFile(sessionStorage.getItem('CartOrderID') + '.txt', null, vDataSignature.ImageURL);

                                                pResult2 = SalesMenu_offline().SaveSalesOrder($.extend(pData, vData, vDataSignature));
                                                if (pResult2)
                                                    SaveOrPrintSalesOrderDetails(pResult2);

                                            }; // END mSignaturePage.OnSubmit = function() {

                                            mSignaturePage.show();
                                        });
                                    } else {
                                        pResult2 = SalesMenu_offline().SaveSalesOrder($.extend(pData, vData));
                                        if (pResult2)
                                            SaveOrPrintSalesOrderDetails(pResult2);
                                        else
                                            NewMessageBox("There was an error occur while saving sales order.");


                                    } //if (Business.SystemDefaults.GetSystemDefault


                                }; // END popSalesMenuOrderTaker.OnSubmit = function() {

                                if ((CBool(proj.POMode) || CBool(proj.SOMode)) && (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall')) {
                                    window.pageHeader.ShowOrderTakerPage();
                                    $("#supplierDetails").hide();
                                } else {
                                    proj.SalesMenuOrderTakerLoadView();
                                    $('#lblSalesOrderFOWST').text('FOWST location that will serve this order:');
                                    $("#supplierDetails").hide();
                                    popSalesMenuOrderTaker.Show();
                                    getSalesOrderCountryName();
                                    getSalesOrderCityName();
                                }
                                if (CBool(proj.POMode))
                                    $("#orderTakerHeader").text("Purchase Order Details");
                                else {
                                    $("#orderTakerHeader").text("Sales Order Details");
                                    $('#lblSalesOrderFOWST').text('FOWST location that will serve this order:');
                                }

                                getSalesOrderFowsts();
                                $('.lblPosBranch').text(sessionStorage.FowstName);
                            } else {

                                AjaxSend({
                                    url: '/SalesMenu/SaveSO',
                                    data: pData,
                                    success: function (vData) {

                                        if (vData.Result == 'Success') {
                                            NewMessageBox("Sales Order has been saved successfully. Would you like to print Sales Order Reference?",
                                        'Sales Order',
                                        [
                                            {
                                                Text: 'Yes',
                                                Click: function () {
                                                    alert('Not yet implemented.');
                                                    sessionStorage.removeItem('CartOrderID');
                                                    proj.CartData = null;
                                                    proj.CartStock = [];
                                                    if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {
                                                        //The following instructions are to clear the cart
                                                        //once the transaction has been finished before loading the page.
                                                        $('#pushSection').empty();
                                                        location = MergeUrlParameters({
                                                            OrgID: sessionStorage["OrgID"],
                                                            mode: 'so',
                                                            IsMobile: IsMobile ? 1 : 0,
                                                            theme: 'bs'
                                                        }, '/SalesMenu/BsPOSViewAll');
                                                    } else {
                                                        CreateNewCart();
                                                    }
                                                }
                                            }
                                        ,
                                            {
                                                Text: 'No',
                                                Click: function () {
                                                    if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {
                                                        //The following instructions are to clear the cart
                                                        //once the transaction has been finished before loading the page.
                                                        $('#pushSection').empty();
                                                        location = MergeUrlParameters({
                                                            OrgID: sessionStorage["OrgID"],
                                                            mode: 'so',
                                                            IsMobile: IsMobile ? 1 : 0,
                                                            theme: 'bs'
                                                        }, '/SalesMenu/BsPOSViewAll');
                                                        CreateNewCart();
                                                    } else {
                                                        CreateNewCart();
                                                    }
                                                }
                                            }
                                        ]
                                    );
                                            return;
                                        } else {
                                            NewMessageBox(vData.Result);
                                        }
                                    }
                                }); //AjaxSend({
                            } //if (OfflineMode) {

                        } // if (proj.PosMode) {
                    }; // if (proj.ReconstructSalesDataMode) {
                    //////////////////////                    } else {
                    //////////////////////                        window.location = '/deliveryinfo?SalesOrderID=' + sessionStorage.getItem('CartOrderID');
                    //////////////////////                    }; // if (proj.PosMode) {

                    //addtional
                    function SaveOrPrintSalesOrderDetails(vData) {


                        if (Business.SystemDefaults.GetSystemDefault("When Do You Want To Print Order Slip") == 'Before Payment') {
                            if (CBool(sessionStorage.IsInventoryCountMode)) {
                                FlagInventoryCountSalesOrderID();
                                //                                                NewMessageBox("Sales Order has been saved successfully.",
                                //                                                            'Sales Order',
                                //                                                            [
                                //                                                                { Text: 'Download PDF File',
                                //                                                                    Click: function () {
                                //                                                                        CreatePDFFile();
                                //                                                                    }
                                //                                                                }
                                //                                                            ,
                                //                                                                { Text: 'Download and E-mail PDF File',
                                //                                                                    Click: function () {

                                //                                                                    }
                                //                                                                }
                                //                                                            ,
                                //                                                                { Text: 'Cancel',
                                //                                                                    Click: function () {
                                //                                                                        popSalesMenuOrderTaker.Clear();
                                //                                                                        CreateNewCart();
                                //                                                                    }
                                //                                                                }
                                //                                                            ]
                                //                                                );
                                NewMessageBox("Inventory Based Sales Order has been saved successfully.",
                                            'Inventory Based',
                                            [
                                                {
                                                    Text: 'Ok',
                                                    Click: function () {
                                                        popSalesMenuOrderTaker.Clear();
                                                        CreateNewCart();
                                                    }
                                                }
                                            ]
                                );
                                return;
                            } else if (proj.POMode) {
                                ShowPoMessage();
                            } else {

                                if (Business.SystemDefaults.GetSystemDefault("Print Order Slip") == '1') {

                                    NewMessageBox("Sales Order has been saved successfully. Would you like to print Sales Order Slip?",
                                        'Sales Order',
                                        [
                                            {
                                                Text: 'Yes',
                                                Click: function () {
                                                    //Print Sales Order BOM

                                                    sessionStorage.removeItem('CartOrderID');
                                                    proj.CartData = null;
                                                    proj.CartStock = [];

                                                    if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {
                                                        //The following instructions are to clear the cart
                                                        //once the transaction has been finished before loading the page.
                                                        $('#pushSection').empty();
                                                        location = MergeUrlParameters({
                                                            OrgID: sessionStorage["OrgID"],
                                                            mode: 'so',
                                                            IsMobile: IsMobile ? 1 : 0,
                                                            theme: 'bs'
                                                        }, '/SalesMenu/BsPOSViewAll');
                                                    } else {
                                                        PrintSalesOrderBOM();

                                                        popSalesMenuOrderTaker.Clear();
                                                        CreateNewCart();
                                                    }
                                                }
                                            }
                                        ,
                                            {
                                                Text: 'No',
                                                Click: function () {

                                                    sessionStorage.removeItem('CartOrderID');
                                                    proj.CartData = null;
                                                    proj.CartStock = [];

                                                    if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {
                                                        //The following instructions are to clear the cart
                                                        //once the transaction has been finished before loading the page.
                                                        $('#pushSection').empty();
                                                        location = MergeUrlParameters({
                                                            OrgID: sessionStorage["OrgID"],
                                                            mode: 'so',
                                                            IsMobile: IsMobile ? 1 : 0,
                                                            theme: 'bs'
                                                        }, '/SalesMenu/BsPOSViewAll');
                                                    } else {
                                                        popSalesMenuOrderTaker.Clear();
                                                        CreateNewCart();
                                                    }
                                                }
                                            }
                                        ]
                                        );
                                } else { // if (Business.SystemDefaults.GetSystemDefault("Print Order Slip") == '1') {
                                    popSalesMenuOrderTaker.Clear();
                                    CreateNewCart();

                                }  // END if (Business.SystemDefaults.GetSystemDefault("Print Order Slip") == '1') {


                                return;
                            }
                        } else { // END if (Business.SystemDefaults.GetSystemDefault("When Do You Want To Print Order Slip") == 'Before Payment') {
                            if (proj.POMode)
                                ShowPoMessage();
                            else {
                                var pMessage;
                                var pButtons;

                                pMessage = "Sales Order has been saved successfully. "
                                    + 'Sales Order No.: ' + CDec2(vData.SalesOrderNo) + '. '
                                    + 'Transaction date: ' + $.format.date(new Date(vData.InvoiceDate), proj.DateFormat) + '.';

                                if (vData.IsDraft) {
                                    pButtons = [
                                        {
                                            Text: 'No',
                                            Click: function () {
                                                popSalesMenuOrderTaker.Clear();
                                                CreateNewCart();
                                            }
                                        }
                                    ];
                                } else {
                                    pMessage += '\n\n Do you want the app to create a PDF document of this transaction?';
                                    pButtons = [
                                        {
                                            Text: 'Yes',
                                            Click: function () {
                                                Business.TransInvoice.CreateSalesOrderPdf(vData.ID);
                                                popSalesMenuOrderTaker.Clear();
                                                CreateNewCart();
                                            }
                                        },
                                        {
                                            Text: 'No',
                                            Click: function () {

                                                popSalesMenuOrderTaker.Clear();
                                                CreateNewCart();
                                            }
                                        }
                                    ];
                                }

                                NewMessageBox(pMessage,
                                    'Sales Order',
                                    pButtons
                                );
                            }
                        }  // END if (Business.SystemDefaults.GetSystemDefault("When Do You Want To Print Order Slip") == 'Before Payment') {

                        function ShowPoMessage() {
                            NewMessageBox('Purchase Order has been saved successfully. Would you like to download a PDF file?',
                                'Purchase Order',
                                [
                                    {
                                        Text: 'Yes',
                                        Click: function () {
                                            Business.TransInvoice.CreateSalesOrderPdf(sessionStorage.getItem('CartOrderID'),
                                                {
                                                    IsPurchaseOrder: true
                                                }
                                            );
                                            return;

                                            var transRow = [];
                                            var TotalSum = 0;
                                            var pDT;
                                            var pSql;
                                            var pDt2;
                                            var pSql2;
                                            var pSql3;
                                            var pDT3;
                                            var pSql4;
                                            var pDT4;

                                            //Get necessary details of the PO Transaction
                                            pSql = "SELECT PurchaseOrderNo " +
                                                            ", PurchaseOrderDate" +
                                                            ", DeliveryDate" +
                                                            ", IFNULL(NULLIF(LibraryOrganizationFOWST.[StreetNumber] || ', ', ', '), '') " +
                                                                "|| IFNULL(NULLIF(LibraryOrganizationFOWST.[Street] || ', ', ', '), '')   " +
                                                                "|| IFNULL(LibraryCities.[Name], '') || ', ' " +
                                                                "|| IFNULL(LibraryCities.[Zip], '') || ', ' " +
                                                                "|| LibraryCountries.[Name] AS BillingAddress " +
                                                            ", '+' || LibraryCountries.[CallingCode] " +
                                                                "|| LibraryAreaCodes.[CallingCode] " +
                                                                "|| LibraryOrganizationFOWSTTel.[ContactNo] AS BillingContactDetails " +
                                                            ", ( " +
                                                                "SELECT LibraryOrganization.[CompanyName] " +
                                                                "FROM TransInvoice  " +
                                                                        "LEFT JOIN LibraryOrganizationFOWST ON TransInvoice.[FowstID] = LibraryOrganizationFOWST.[ID] " +
                                                                        "LEFT JOIN LibraryOrganization ON LibraryOrganizationFOWST.[OrganizationID] = LibraryOrganization.[ID] " +
                                                            ") AS OrgName " +
                                                            ", ( " +
                                                                "SELECT IFNULL(NULLIF(LibraryOrganization.[OfficeAddressBuildingNo] || ', ', ', '), '') " +
                                                                        "|| IFNULL(NULLIF(LibraryOrganization.[OfficeAddressStreet] || ', ', ', '), '') " +
                                                                        "|| IFNULL(LibraryCities.[Name], '') || ', ' " +
                                                                        "|| IFNULL(LibraryCities.[Zip], '') || ', ' " +
                                                                        "|| LibraryCountries.[Name] " +
                                                                "FROM TransInvoice " +
                                                                        "LEFT JOIN LibraryOrganizationFOWST ON TransInvoice.[FowstID] = LibraryOrganizationFOWST.[ID] " +
                                                                        "LEFT JOIN LibraryOrganization ON LibraryOrganizationFOWST.[OrganizationID] = LibraryOrganization.[ID] " +
                                                                        "LEFT JOIN LibraryCities ON LibraryOrganization.[OfficeAddressCityID] = LibraryCities.[ID] " +
                                                                        "LEFT JOIN LibraryCountries ON LibraryOrganization.[OfficeAddressCountryID] = LibraryCountries.[ID] " +
                                                                ") AS OrgAddress " +
                                                            ", TransInvoice.[Remarks] " +
                                                            ", TransInvoice.[TotalAmount] " +
                                                    "FROM TransInvoice " +
                                                            "LEFT JOIN LibraryOrganizationFOWST ON TransInvoice.[BillingAddressID] = LibraryOrganizationFOWST.[ID] " +
                                                            "LEFT JOIN LibraryCities ON LibraryOrganizationFOWST.[FowstCityID] = LibraryCities.[ID] " +
                                                            "LEFT JOIN LibraryCountries ON LibraryOrganizationFOWST.[FowstCountryID] = LibraryCountries.[ID] " +
                                                            "LEFT JOIN LibraryOrganizationFOWSTTel ON LibraryOrganizationFOWST.[ID] = LibraryOrganizationFOWSTTel.[FOWSTID] " +
                                                            "LEFT JOIN LibraryAreaCodes ON LibraryOrganizationFOWSTTel.[AreaCodeID] = LibraryAreaCodes.[ID] " +
                                                    "WHERE TransInvoice.[ID]='" + sessionStorage.getItem('CartOrderID') + "'";
                                            pDT = Sql.Execute(pSql);

                                            //Get the Delivery details
                                            pSql3 = "SELECT  IFNULL(NULLIF(LibraryOrganizationFOWST.[StreetNumber] || ', ', ', '), '') " +
                                                        "|| IFNULL(NULLIF(LibraryOrganizationFOWST.[Street] || ', ', ', '), '')   " +
                                                        "|| IFNULL(LibraryCities.[Name], '') || ', ' " +
                                                        "|| IFNULL(LibraryCities.[Zip], '') || ', ' " +
                                                        "|| LibraryCountries.[Name] AS DeliveryAddress " +
                                                    ", '+' || LibraryCountries.[CallingCode] " +
                                                        "|| LibraryAreaCodes.[CallingCode] " +
                                                        "|| LibraryOrganizationFOWSTTel.[ContactNo] AS DeliveryContactDetails " +
                                            "FROM TransInvoice " +
                                                    "LEFT JOIN LibraryOrganizationFOWST ON TransInvoice.[DeliveryAddressID] = LibraryOrganizationFOWST.[ID] " +
                                                    "LEFT JOIN LibraryCities ON LibraryOrganizationFOWST.[FowstCityID] = LibraryCities.[ID] " +
                                                    "LEFT JOIN LibraryCountries ON LibraryOrganizationFOWST.[FowstCountryID] = LibraryCountries.[ID] " +
                                                    "LEFT JOIN LibraryOrganizationFOWSTTel ON LibraryOrganizationFOWST.[ID] = LibraryOrganizationFOWSTTel.[FOWSTID] " +
                                                    "LEFT JOIN LibraryAreaCodes ON LibraryOrganizationFOWSTTel.[AreaCodeID] = LibraryAreaCodes.[ID] " +
                                            "WHERE TransInvoice.[ID]='" + sessionStorage.getItem('CartOrderID') + "'";
                                            pDT3 = Sql.Execute(pSql3);

                                            //Get the supplier details
                                            pSql4 = "SELECT  LibraryOrganizationFOWST.[Name] AS SupplierName" +
                                                        ", IFNULL(NULLIF(LibraryOrganizationFOWST.[StreetNumber] || ', ', ', '), '') " +
                                                        "|| IFNULL(NULLIF(LibraryOrganizationFOWST.[Street] || ', ', ', '), '')   " +
                                                        "|| IFNULL(LibraryCities.[Name], '') || ', ' " +
                                                        "|| IFNULL(LibraryCities.[Zip], '') || ', ' " +
                                                        "|| LibraryCountries.[Name] AS SupplierAddress " +
                                                        ", LibraryOrganization.[CompanyName] AS SupplierCompanyName " +
                                                        ", ( SELECT " +
                                                            "IFNULL(NULLIF(LibraryOrganization.[OfficeAddressBuildingNo] || ', ', ', '), '') " +
                                                            "|| IFNULL(NULLIF(LibraryOrganization.[OfficeAddressStreet] || ', ', ', '), '') " +
                                                            "|| IFNULL(LibraryCities.[Name], '') || ', ' " +
                                                            "|| IFNULL(LibraryCities.[Zip], '') || ', ' " +
                                                            "|| LibraryCountries.[Name] " +
                                                    "FROM TransInvoice " +
                                                        "LEFT JOIN LibraryOrganization ON TransInvoice.[SupplierID] = LibraryOrganization.[ID] " +
                                                        "LEFT JOIN LibraryCities ON LibraryOrganization.[OfficeAddressCityID] = LibraryCities.[ID] " +
                                                        "LEFT JOIN LibraryCountries ON LibraryOrganization.[OfficeAddressCountryID] = LibraryCountries.[ID] " +
                                                ")AS SupplierCompanyAddress " +
                                            "FROM TransInvoice " +
                                                    "LEFT JOIN LibraryOrganizationFOWST ON TransInvoice.[SupplierFowstID] = LibraryOrganizationFOWST.[ID] " +
                                                    "LEFT JOIN LibraryCities ON LibraryOrganizationFOWST.[FowstCityID] = LibraryCities.[ID] " +
                                                    "LEFT JOIN LibraryCountries ON LibraryOrganizationFOWST.[FowstCountryID] = LibraryCountries.[ID] " +
                                                    "LEFT JOIN LibraryOrganizationFOWSTTel ON LibraryOrganizationFOWST.[ID] = LibraryOrganizationFOWSTTel.[FOWSTID] " +
                                                    "LEFT JOIN LibraryAreaCodes ON LibraryOrganizationFOWSTTel.[AreaCodeID] = LibraryAreaCodes.[ID] " +
                                                    "LEFT JOIN LibraryOrganization ON TransInvoice.[SupplierID] = LibraryOrganization.[ID] " +
                                            "WHERE TransInvoice.[ID]='" + sessionStorage.getItem('CartOrderID') + "'";
                                            pDT4 = Sql.Execute(pSql4);

                                            //Get the products of the transaction
                                            pSql2 = "SELECT LibraryPimProduct.[ProductNo] " +
                                                        ", LibraryPimProduct.[ProductName] " +
                                                        ", TransInvoiceItems.[ActualQuantity] " +
                                                        ", TransInvoiceItems.[OrderedQuantity] " +
                                                        ", TransInvoiceItems.[UnitMeasure] " +
                                                        ", TransInvoiceItems.[UnitPrice] " +
                                                        ", TransInvoiceItems.[SubTotal] " +
                                                    "FROM TransInvoiceItems " +
                                                        "LEFT JOIN TransInvoice ON TransInvoiceItems.[InvoiceID] = TransInvoice.[ID] " +
                                                        "LEFT JOIN LibraryPimProduct ON TransInvoiceItems.[ProductID] = LibraryPimProduct.[ID]" +
                                                    "WHERE TransInvoice.[ID]='" + sessionStorage.getItem('CartOrderID') + "'";
                                            pDT2 = Sql.Execute(pSql2);

                                            //Strip the data and format it to JSON
                                            for (var i = 0; i < pDT2.Rows.length; i++) {
                                                var rowDetail = [];
                                                rowDetail.push(pDT2.Rows[i].ProductNo);
                                                rowDetail.push(pDT2.Rows[i].ProductName);
                                                rowDetail.push(pDT2.Rows[i].OrderedQuantity);
                                                rowDetail.push(pDT2.Rows[i].UnitMeasure);
                                                rowDetail.push(AccountingFormat(CDec2(pDT2.Rows[i].UnitPrice)));
                                                rowDetail.push(AccountingFormat(CDec2(pDT2.Rows[i].SubTotal)));
                                                TotalSum = parseFloat(CDec2(TotalSum)) + parseFloat(CDec2(pDT2.Rows[i].SubTotal));
                                                transRow.push(rowDetail);
                                            }

                                            //Create PDF
                                            Android.PDFCreation({
                                                "operation": [
                                                    { "caseOperation": "purchaseOrder" },
                                                    { "docType": "poNum" },
                                                    { "docNum": pDT.Rows[0].PurchaseOrderNo },
                                                    { "docTypeDate": "poDate" },
                                                    { "dateData": $.format.date(new Date(pDT.Rows[0].PurchaseOrderDate), 'EEEE MMM dd, yyyy') },
                                                    { "docTypeDelivery": "poDeliveryDate" },
                                                    { "deliveryDate": $.format.date(new Date(pDT.Rows[0].DeliveryDate), 'EEEE MMM dd, yyyy') },
                                                    { "docTypeTotal": "poTotal" },
                                                    { "totalData": AccountingFormat(CDec2(TotalSum)) },
                                                    { "orgNameRaw": pDT.Rows[0].OrgName },
                                                    { "orgAddressRaw": (pDT.Rows[0].OrgAddress == undefined ? "" : pDT.Rows[0].OrgAddress) },
                                                    { "custNameRaw": (pDT4.Rows[0].SupplierName == undefined ? pDT4.Rows[0].SupplierCompanyName : pDT4.Rows[0].SupplierName) },
                                                    { "custAddressRaw": (pDT4.Rows[0].SupplierAddress == undefined ? (pDT4.Rows[0].SupplierCompanyAddress == undefined ? "" : pDT4.Rows[0].SupplierCompanyAddress) : pDT4.Rows[0].SupplierAddress) },
                                                    { "billingAddressDetailsRaw": (pDT.Rows[0].BillingAddress == undefined ? "" : pDT.Rows[0].BillingAddress) },
                                                    { "billingContactDetailsRaw": (pDT.Rows[0].BillingContactDetails == undefined ? "" : pDT.Rows[0].BillingContactDetails) },
                                                    { "billingContactOptionalRaw": '' },
                                                    { "deliveryAddressDetailsRaw": (pDT3.Rows[0].DeliveryAddress == undefined ? "" : pDT3.Rows[0].DeliveryAddress) },
                                                    { "deliveryContactDetailsRaw": (pDT3.Rows[0].DeliveryContactDetails == undefined ? "" : pDT3.Rows[0].DeliveryContactDetails) },
                                                    { "deliveryContactOptionalRaw": '' },
                                                    { "deliveryRemarksRaw": pDT.Rows[0].Remarks },
                                                    { "qrcode": 'PO|' + sessionStorage.OrgID + '|' + sessionStorage.FowstID + '|PO ' + SetPrefixPostfix(pDT.Rows[0].PurchaseOrderNo, 12, '0', 0) },
                                                    { "countDate": "FORMOBIZPROJECT" },
                                                    { "userNote": "" }
                                                ]
                                            }, {
                                                "data": [
                                                    { "columns": ["Item Code:", "Item Description:", "Quantity", "UoM", "Unit Price", "Sub Total"] },
                                                    {
                                                        "rows": transRow
                                                    }
                                                ]
                                            });
                                            sessionStorage.removeItem('CartOrderID');
                                            proj.CartData = null;
                                            proj.CartStock = [];
                                            if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {
                                                //The following instructions are to clear the cart
                                                //once the transaction has been finished before loading the page.
                                                $('#pushSection').empty();
                                                //Clear also the Supplier and Customer details
                                                sessionStorage.removeItem('SupplierFowstID');
                                                sessionStorage.removeItem('SupplierFowstName');
                                                sessionStorage.removeItem('SupplierOrganizationID');
                                                sessionStorage.removeItem('SupplierOrganizationName');
                                                sessionStorage.removeItem('SupplierAddress');
                                                sessionStorage.removeItem('SupplierPriceLevelID');
                                                sessionStorage.removeItem('SupplierPriceLevelName');
                                                location = MergeUrlParameters({
                                                    OrgID: sessionStorage["OrgID"],
                                                    mode: 'po',
                                                    IsMobile: IsMobile ? 1 : 0,
                                                    theme: 'bs'
                                                }, '/SalesMenu/BsPOSViewAll');
                                            } else {
                                                popSalesMenuOrderTaker.Clear();
                                                CreateNewCart();
                                            }
                                        }
                                    },
                                    {
                                        Text: 'No',
                                        Click: function () {
                                            sessionStorage.removeItem('CartOrderID');
                                            proj.CartData = null;
                                            proj.CartStock = [];
                                            if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {
                                                //The following instructions are to clear the cart
                                                //once the transaction has been finished before loading the page.
                                                $('#pushSection').empty();
                                                //Clear also the Supplier and Customer details
                                                sessionStorage.removeItem('SupplierFowstID');
                                                sessionStorage.removeItem('SupplierFowstName');
                                                sessionStorage.removeItem('SupplierOrganizationID');
                                                sessionStorage.removeItem('SupplierOrganizationName');
                                                sessionStorage.removeItem('SupplierAddress');
                                                sessionStorage.removeItem('SupplierPriceLevelID');
                                                sessionStorage.removeItem('SupplierPriceLevelName');
                                                location = MergeUrlParameters({
                                                    OrgID: sessionStorage["OrgID"],
                                                    mode: 'po',
                                                    IsMobile: IsMobile ? 1 : 0,
                                                    theme: 'bs'
                                                }, '/SalesMenu/BsPOSViewAll');
                                            } else {
                                                popSalesMenuOrderTaker.Clear();
                                                CreateNewCart();
                                            }
                                        }
                                    }

                                ]
                            );
                        } // END function ShowPoMessage() {
                    } // END SaveOrPrintSalesOrderDetails
                    ///////////////////////








                    function PrintSalesOrderBOM() {
                        var vDataBOMPrinters;
                        var vDataBOM;
                        var strTemp = '';
                        var strPrint = '';
                        var intFAFCTotalLenght = 40;
                        var intFBTotalLenght = 27;
                        var intFCDoubleTotalLenght = 18;

                        vDataBOMPrinters = SalesPayment_offline().getSalesPaymentReceiptBOMPrinter(sessionStorage.getItem('CartOrderID'));

                        //Get Printer IP Address
                        if (vDataBOMPrinters != undefined) {
                            if (vDataBOMPrinters.Rows.length > 0) {

                                //Loop Print per Printer
                                for (var pCntr = 0; pCntr <= vDataBOMPrinters.Rows.length - 1; pCntr++) {

                                    vDataBOM = SalesPayment_offline().getSalesPaymentReceiptBOMByPrinter(sessionStorage.getItem('CartOrderID'), $.trim((vDataBOMPrinters.Rows[pCntr].ID || '')));

                                    if (vDataBOM.Rows.length > 0) {

                                        //New set of text to print
                                        NewPrintBuilder();

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataBOMPrinters.Rows[pCntr].PrinterRole || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Order Slip for', intFAFCTotalLenght, '', 0, '', 0, 0);
                                        ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                        strPrint = '';
                                        strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'SO ' + $.trim((vDataBOM.Rows[0].FOWSTNo || '')) + '-' + $.trim((vDataBOM.Rows[0].DeviceNo || '')) + '-' + $.trim((vDataBOM.Rows[0].SalesOrderNo || '')), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFCDoubleTotalLenght, ($.format.date(new Date(vDataBOM.Rows[0].SalesOrderDate), 'MMM dd, yyyy HH:mm') || ''), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                        strPrint = FormatTextToPrint(intFCDoubleTotalLenght, $.trim((vDataBOM.Rows[0].CustomerName || '')), intFCDoubleTotalLenght, '', 0, '', 0, 0);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                        if ($.trim((vDataBOM.Rows[0].CustomerTableNo || '')) != '') {
                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Table # ', 8, ' ', 1), 8, ': ' + $.trim((vDataBOM.Rows[0].CustomerTableNo || '')), intFCDoubleTotalLenght, '', 0, 0);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 0, 0, 0, 0);
                                        }

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Cashier: ' + ($.trim(vDataBOM.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                        ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);

                                        strPrint = 'so|' + SetPrefixPostfix($.trim((sessionStorage.getItem('CartOrderID') || '')), 12, '0', 0)
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                        strPrint = '';
                                        var intSKUCnt = 0;
                                        for (var intSKU = 0; intSKU <= vDataBOM.Rows.length - 1; intSKU++) {

                                            var strStockDescription = $.trim((vDataBOM.Rows[intSKU].StockDescription || ''));
                                            var strQty = SetPrefixPostfix(AccountingFormat((vDataBOM.Rows[intSKU].OrderedQuantity || '0'), 0), 4, ' ', 0);
                                            var strUnitMeasure = $.trim((vDataBOM.Rows[intSKU].UnitMeasure || ''));

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, strStockDescription, intFAFCTotalLenght - 14, 'Prepared Qty', intFAFCTotalLenght, '', 0, 0);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            strPrint = FormatTextToPrint(intFCDoubleTotalLenght, strQty, 4, strUnitMeasure, 11, '_____', intFCDoubleTotalLenght, 0);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 0, 0, 0, 0);

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            intSKUCnt = intSKUCnt + 1;
                                        };

                                        var strSKUCount = SetPrefixPostfix(AccountingFormat((intSKUCnt || 0), 0), intFAFCTotalLenght - 14, ' ', 0);
                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, 'SKU Count: ', 12, strSKUCount, intFAFCTotalLenght, '', 0, 0);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                        ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                        PrintText(vDataBOMPrinters.Rows[pCntr].PrinterIPAddress);
                                        //ClearCommandBuffer();

                                    }
                                } //End For

                            } //END if (vDataBOMPrinters.Rows.length > 0) {
                        }

                    };

                    function GetOrgFOWSTPrinterIP() {

                        var pSql = "SELECT devfowstpntr.ID, " +
                " fowstpntr.PrinterIPAddress " +
                " FROM private.LibraryDeviceFowstPrinter devfowstpntr " +
                " INNER JOIN private.LibraryDevicePrinterFowst fowstpntr ON (fowstpntr.ID = devfowstpntr.PrinterFowstID) " +
                " Where fowstpntr.FOWSTID = '" + sessionStorage.FowstID + "' " +
                " AND devfowstpntr.DeviceID = '" + Web.DeviceID() + "'";

                        //                        var pSql = "SELECT devfowstpntr.ID, " +
                        //                                    " devfowstpntr.PrinterIPAddress " +
                        //                                    " FROM private.LibraryDeviceFowstPrinter devfowstpntr " +
                        //                                    " Where devfowstpntr.FOWSTID = '" + sessionStorage.FowstID + "' " +
                        //                                    " AND devfowstpntr.DeviceID = '" + Web.DeviceID() + "'";

                        var pDT = Sql.Execute(pSql);
                        if (pDT == undefined) return null;

                        return pDT;

                    };

                    function GetCartSalesOrderDetails() {
                        var pSql = "Select inv.SalesOrderNo || '-' || orgfowst.FOWSTNo || '-' || dev.DeviceNo as SalesOrderNo, " +
                        " inv.SalesOrderDate, " +
                        " ppl.Surname || ', ' || ppl.Firstname as CreatedBy, " +
                        " (CASE " +
                        "     WHEN org.IsOrganization = 1 THEN org.CompanyName " +
                        "     WHEN org.IsOrganization = 0 THEN " +
                        "         IFNULL(NULLIF(TRIM(org.PersonSurname) || ', ',', '), '') || " +
                        "         IFNULL(NULLIF(TRIM(org.PersonFirstName) || ' ',' '), '') || " +
                        "         IFNULL(NULLIF(TRIM(org.PersonMiddleName) || ' ',' '), '') || " +
                        "         IFNULL(NULLIF(', ' || TRIM(org.PersonSuffixName),', '), '') " +
                        " END) as CompanyName, " +
                        " inv.CustomerTableNo " +
                        " FROM TransInvoice inv " +
                        " INNER JOIN LibraryPeople ppl on inv.SalesOrderUserID = ppl.ID " +
                        " INNER JOIN LibraryOrganization org on org.ID = inv.CustomerID" +
                        " INNER JOIN LibraryDevice dev on inv.DeviceID = dev.ID " +
                        " INNER JOIN LibraryOrganizationfowst orgfowst on orgfowst.ID = inv.FowstID " +
                        " WHERE inv.ID = '" + sessionStorage.getItem('CartOrderID') + "'";

                        var pDT = Sql.Execute(pSql);
                        if (pDT == undefined) return null;

                        return pDT;
                    }




                    function Verify() {

                        if (!IsDate($('#txtDate').DateCombo('val'))) {
                            NewMessageBox('Date & Time of Original Sales Transaction must be a valid date and time. Example: ' + $.format.date(new Date(), 'M/d/yyyy h:mm a'));
                            return false;
                        };

                        if ($('#txtRefNum').val().trim() == '') {
                            NewMessageBox('Please provide Reference (Invoice) Number of Original Sales Transaction');
                            return false;
                        };

                        if ($('#cbReasonForReconstructingData').val() == '0') {
                            NewMessageBox('Please choose a Reason for Reconstructing Data');
                            return false;
                        };

                        return true;

                    }; // Verify
                }


                var theInvoiceID;
                function VerifyPrintReceipt(pInvoiceID) {
                    //                    NewMessageBox("Your transaction has been saved. Were you able to successfully print out a receipt?",
                    //                        "Print Confirmation",
                    //                        [
                    //                            {
                    //                                Text: 'Yes'
                    //                            },
                    //                            {
                    //                                Text: "No, it didn't print",
                    //                                Click: function () {
                    //                                    window.pageSecuritySourcePage = $.mobile.activePage;
                    //                                    PromptUserAccess('POS Receipt Reprint', {
                    //                                        OnClickOKSuccess: function () {
                    //                                            PrintBilling(pInvoiceID);
                    //                                        }
                    //                                    });
                    //                                }
                    //                            }
                    //                        ], null, true
                    //                    );

                    theInvoiceID = pInvoiceID;
                    if (window.InAndroid) {
                        //Android.ScriptToWebView({
                        //    Script: 'proj.ReloadTableView()',
                        //    WebViewIndex: 1
                        //});
                        //Android.SetCurrentTab(0);
                        if (CStr2(sessionStorage.PrintOption) == '') { // sessionStorage.PrintOption = 'billingorderslip' or 'orderslip'
                            //dili iclear ang payment info basta magprint ug billing or order slip sa Payments page
                            proj.SalesMenuInit();
                            if ($.isFunction(GetStringObject('proj.SalesPayments.ClearSalesPaymentsInfoFields')))
                                proj.SalesPayments.ClearSalesPaymentsInfoFields();
                            $('#divHeader, #divHotButtons').show();
                            $('#divPayment').hide();
                        }                        
                        //var pBaseLocation = '/SalesMenu';
                        //if ($(window).width() > proj.WideWidth) //
                        //    pBaseLocation += '/BsPOSViewAll';
                        //var pLocation = MergeUrlParameters({
                        //    OrgID: sessionStorage["OrgID"],
                        //    mode: 'pos',
                        //    IsMobile: IsMobile ? 1 : 0,
                        //    // TableView: 'true', 
                        //    TableNo: 'js998QYHQHXH!'
                        //}, pBaseLocation);
                        //location = pLocation;
                    } else
                        $('#btnPrintYesBill').click();
                    //$.mobile.changePage("#dialogPrintConfirmationBill");
                }

                function VerifyDispatchSlipPrintReceipt(pInvoiceID) {
                    theInvoiceID = pInvoiceID;

                    $('#btnPrintYesDispatchSlip').click();
                    //$.mobile.changePage("#dialogPrintConfirmationDispatchSlip");
                }

                function PredeterminePrinter() {

                    debugger; // this code should be replaced with the one from PrePos

                    if (!IsMobile) return;
                    if (OfflineMode) return; // todo
                    AjaxSend({
                        url: '/Organization/getOrgFOWSTPrinterIP',
                        success: function (vData) {
                            try {
                                if (vData.OrganizationFOWSTData.Rows.length == 0) {
                                    NewMessageBox('This device has no assigned printer in the selected FOWST');
                                };
                            } catch (e) {
                                NewMessageBox('Unable to determine "vData.OrganizationFOWSTData.Rows.length"');
                                console.error(e);
                            };
                        }
                    });

                }; // PredeterminePrinter

                function VerifyDevice() {

                    debugger; // this code should be replaced with the one from PrePos

                    if (OfflineMode) return; // todo
                    AjaxSend({
                        url: '/Login3/VerifyDevice',
                        success: function (vData) {
                            if (vData.Error != undefined) {
                                NewMessageBox('This device is not assigned to this FOWST',
                                    'FOWST',
                                    [
                                        {
                                            Text: 'Ok',
                                            Click: function () {
                                                //redirect to home. kick out of POS
                                                window.location = '/People/PeopleMenu';
                                            }
                                        }
                                    ]
                        );

                            } else {
                                NewCart(); //additional by janine
                                isCartEmpty = true;
                            }

                        }
                    }); // AjaxSend


                }; // VerifyDevice

                GetPOSMode();

                function GetPOSMode() {
                    if (proj.PosMode) {
                        //set Retail mode as default
                        if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {
                            $('#lblCurrentPOSMode').text('Hot Button Mode (Widescreen)');
                        } else {
                            $('#lblCurrentPOSMode').text('Retail Mode');
                        }

                        if (GetUrlParameters().TableNo != undefined) {
                            $('#lblCurrentPOSMode').text('Table View Mode');
                        }
                    }
                    else {
                        if (CBool(sessionStorage.IsInventoryCountMode)) {
                            $('#lblCurrentPOSMode').text($('#btnPOS_7').find('h3:first').text());
                        }
                        else {
                            // commented by Arvin. lblCurrentPOSMode now holds the ordering fowst
                            // $('#lblCurrentPOSMode').text($('#btnPOS_6').find('h3:first').text());
                        }
                    }
                }

                $('#btnHome').click(function (e) {
                    e.preventDefault();
                    if (GetUrlParameters().TableNo != undefined) {
                        Bs.Loading('show');
                        setTimeout(function () {
                            proj.SaveCart();
                            sessionStorage.PosMode = 'Table View Mode';
                            proj.ProductID = '';
                            //proj.SalesMenuInit();
                            proj.ReloadTableView();
                            if ($.isFunction(GetStringObject('proj.SalesPayments.ClearSalesPaymentsInfoFields'))) {
                                proj.SalesPayments.ClearSalesPaymentsInfoFields();
                            }
                            $('#divHeader, #divHotButtons').show();
                            $('#divPayment').hide();
                            $('#TableView').pageshow();
                            Bs.Loading('hide');
                        }, 50)                        
                    }
                    else {
                        if (window.HotButtons != null)
                            window.HotButtons.SaveCart();
                        window.location = '/People/PeopleMenu';
                    }
                });

                $('#btnTrans').click(function () {
                    $.mobile.changePage('#pageViewTransactions');
                    startclock('#clockTransactionDetails');
                    $('#divContentTransPage').css('margin-top', $('#divHeaderTransPage').height() - 20 + 'px');
                })

                $('#btnHomeTransDetails').click(function () {
                    window.location = '/People/PeopleMenu';
                })

                $('#btnRecordLookup').click(function () {
                    $('#lnkSalesOrder').click();
                })

                $('#btnNewTransaction').click(function () {
                    CreateNewCart();
                })

                // TABLE NO ONCHANGE
                $('#selTableNo').change(function () {

                    $('#lblCustomerName').text($(this).val());

                    if (CStr2($(this).val()) != '') {
                        sessionStorage.GasPumpNo = '';
                        $('#selGasPumpNo').val('').selectmenu('refresh', true);

                        sessionStorage.CustomerTableNo = $(this).val();
                        DisplayTableNumberOnButton();
                        $('#lblLabel').html('Customer / Table No.:<br/>');
                    } else {
                        sessionStorage.CustomerTableNo = '';
                        $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                        $('#lblLabel').html('<img alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /><br/>');

                    }

                    return;
                });
                $('#selGasPumpNo').change(function () {

                    $('#lblCustomerName').text($(this).val());

                    if (CStr2($(this).val()) != '') {
                        sessionStorage.CustomerTableNo = '';
                        $('#selTableNo').val('').selectmenu('refresh', true);

                        sessionStorage.GasPumpNo = $(this).val();
                        $('#lblCustomerName').text(sessionStorage.GasPumpNo);
                        $('#lblLabel').html('Gas Pump No.:<br/>');

                    } else {
                        sessionStorage.GasPumpNo = '';
                        $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                        $('#lblLabel').html('<img alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /><br/>');

                    }

                    return;
                });
                //                $('#txtTableNo').on('input', function () {

                //                    $('#lblCustomerName').text($(this).val().trim());

                //                    if ($(this).val().trim() != '') {
                //                        sessionStorage.CustomerTableNo = $(this).val().trim();
                //                        $('#lblCustomerName').text(sessionStorage.CustomerTableNo);
                //                    } else {
                //                        sessionStorage.CustomerTableNo = '';
                //                        $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                //                    }

                //                    return;
                //                });


                $('body').delegate('#btnSupplier, #btnSupplierHotButtons', 'click', function (e) {
                    e.preventDefault();
                    mPrePos.BrowseSupplier();
                });

                $('#btnCustomer').click(function (e) {
                    e.preventDefault();
                    //new edited by janine oct 24 see CustomerDetails.js in Scripts3
                    if (proj.POMode)
                        mPrePos.BrowseSupplier();
                    else
                        pageCustomerDetails.Show();

                });

                function LoadSupplier() {

                    debugger; // this code should be replaced with the one from PrePos

                    if (proj.POMode) {
                        $('#divBlockCustomer').hide();
                        $('#divBlockSupplier').show();

                        if (OfflineMode) {
                            var vSupplier;
                            var vData;
                            vData = SalesMenu_offline().getSupplier(sessionStorage.FowstID);

                            if (GetStringObject('Rows.length', vData) > 0) {

                                vSupplier = vData.Rows[0];

                                if (CBool(vSupplier.IsPOExclusiveToPrincipal)) {

                                    $('#lblSupplierName').html(HtmlEncode(vSupplier.CompanyName) + '&#x25BC;')
                            .data('id', vSupplier.ID);


                                    sessionStorage.SupplierOrganizationID = CStr2(vSupplier.ID);
                                    sessionStorage.SupplierOrganizationName = CStr2(vSupplier.CompanyName);
                                    sessionStorage.SupplierPriceLevelID = CStr2(vSupplier.POPriceLevel);
                                    sessionStorage.SupplierPriceLevelName = CStr2(vSupplier.PriceLevel);
                                } else
                                    mPrePos.BrowseSupplier();

                            } else
                                mPrePos.BrowseSupplier();
                        } else {
                            NewMessageBox('This feature is only available in the mobile app',
                        'Purchase Order',
                        [
                            {
                                Text: 'Yes',
                                Click: function () {
                                    location = '/People/PeopleMenu';
                                }
                            }

                        ]
                );
                        }
                    } else {
                        sessionStorage.removeItem('SupplierOrganizationID');
                        sessionStorage.removeItem('SupplierOrganizationName');
                        sessionStorage.removeItem('SupplierPriceLevelID');
                        sessionStorage.removeItem('SupplierPriceLevelName');
                        $('#divBlockCustomer').show();
                        $('#divBlockSupplier').hide();
                    }


                }

                function BrowseSupplier() {

                    debugger; // this code should be replaced with the one from PrePos
                    return;

                    //var pDR = Business.LibraryOrganizationFOWST.GetFranchiseeData(sessionStorage.FowstID);
                    //if (pDR != null) {
                    //    if (CBool(pDR.IsPOExclusiveToPrincipal)) {
                    //        $('#lblSupplierName, #lblCustomerName, #lblSupplierNameHotButtons')
                    //            .text('Supplier: ' + pDR.FranchisorCompanyName)
                    //            .data('id', pDR.FranchisorOrgID);

                    //        NewMessageBox('The selected customer FOWST is exclusive to this franchisor.');
                    //        return;
                    //    }
                    //}

                    //Browser.LibraryOrganization.InputData.FilterCustomers = 0;
                    //Browser.LibraryOrganization.InputData.FilterConfirmedFranchisor = 1;

                    if (mPurchaseOrder == null)
                        mPurchaseOrder = new App.controller.PurchaseOrder();
                    mPurchaseOrder.ok = function (vData) {
                        $('#lblSupplierName, #lblCustomerName, #lblSupplierNameHotButtons')
                            .text('Supplier: ' + vData.CompanyName + ' - ' + vData.FowstName)
                            .data('id', vData.OrgID);

                        sessionStorage.SupplierOrganizationID = CStr2(vData.OrgID);
                        sessionStorage.SupplierOrganizationName = CStr2(vData.CompanyName);
                        sessionStorage.SupplierAddress = CStr2(vData.Address);
                        if (vData.FowstID == undefined)
                            sessionStorage.removeItem('SupplierFowstID');
                        else {
                            sessionStorage.SupplierFowstID = vData.FowstID;
                            Android.UpdateSessionData({
                                FowstID: vData.FowstID
                            }); // disconnected checkout
                        }
                        if (vData.FowstName == undefined)
                            sessionStorage.removeItem('SupplierFowstName');
                        else
                            sessionStorage.SupplierFowstName = vData.FowstName;
                        if (IsGuid(vData.BinID)) {
                            sessionStorage.SupplierBinID = vData.BinID;
                            sessionStorage.SupplierBinName = vData.BinName;
                        } else {
                            sessionStorage.removeItem('SupplierBinID');
                            sessionStorage.removeItem('SupplierBinName');
                        }

                        var pData = SalesMenu_offline().getSupplierPriceLevel(vData.OrgID, sessionStorage.FowstID);

                        if (GetStringObject('Rows.length', pData) > 0) {

                            var pSupplier = pData.Rows[0];


                            sessionStorage.SupplierPriceLevelID = CStr2(pSupplier.POPriceLevel);
                            sessionStorage.SupplierPriceLevelName = CStr2(pSupplier.PriceLevel);


                        } else {
                            //TODO. if no default price level temporarily set to SRP because SRP is required so every org product must have it
                            sessionStorage.SupplierPriceLevelID = 'EEF716C6-B290-4B28-B2A6-0A4F52CF4989';
                            sessionStorage.SupplierPriceLevelName = 'SRP (Suggested Retail Price)';

                        }
                    };

                    (function () {
                        var pPoInput = {
                            OrgID: sessionStorage.SupplierOrganizationID,
                            FowstID: sessionStorage.SupplierFowstID,
                            BinName: sessionStorage.SupplierBinName,
                            Title: 'Supplier'
                        };
                        var pSetting = Business.SystemDefaults.PurchaseOrderSupplierSetting();
                        var pDefaultOrgID;
                        var pDefaultFowstID;

                        if (pSetting == 'locked to') {
                            pDefaultOrgID = Business.SystemDefaults.PurchaseOrderSupplierID();
                            if (IsGuid(pDefaultOrgID)) {
                                pPoInput.LockedOrg = true;
                                pPoInput.OrgID = pDefaultOrgID;
                            }
                            pDefaultFowstID = Business.SystemDefaults.PurchaseOrderSupplierFowstID();
                            if (IsGuid(pDefaultFowstID)) {
                                pPoInput.LockedFowst = true;
                                pPoInput.FowstID = pDefaultFowstID;
                            }
                        }
                        if (pSetting == 'default to') {
                            if (!IsGuid(pPoInput.OrgID)) {
                                pDefaultOrgID = Business.SystemDefaults.PurchaseOrderSupplierID();
                                if (IsGuid(pDefaultOrgID)) {
                                    pPoInput.OrgID = pDefaultOrgID;
                                    pDefaultFowstID = Business.SystemDefaults.PurchaseOrderSupplierFowstID();
                                    if (IsGuid(pDefaultFowstID)) {
                                        pPoInput.FowstID = pDefaultFowstID;
                                        pPoInput.LockedFowst = pPoInput.LockedOrg;
                                    }
                                }
                            }
                        }
                        mPurchaseOrder.popup(pPoInput);
                    })();

                    return;

                    // commented 2016_07_26__14_05_11_0052
                    //Browser.LibraryOrganization.InputData.FilterPoSuppliers = 1;
                    //Browser.LibraryOrganization.Click = function (e) {

                    //    //$('#lblSupplierName').html(HtmlEncode(e.CompanyName) + '&#x25BC;')
                    //    //    .data('id', e.ID);
                    //    $('#lblSupplierName, #lblCustomerName, #lblSupplierNameHotButtons').text('Supplier: ' + e.CompanyName)
                    //        .data('id', e.ID);

                    //    sessionStorage.SupplierOrganizationID = CStr2(e.ID);
                    //    sessionStorage.SupplierOrganizationName = CStr2(e.CompanyName);
                    //    sessionStorage.SupplierAddress = CStr2(e.Address);
                    //    if (e.FowstID == undefined)
                    //        sessionStorage.removeItem('SupplierFowstID');
                    //    else
                    //        sessionStorage.SupplierFowstID = e.FowstID;
                    //    if (e.FowstName == undefined)
                    //        sessionStorage.removeItem('SupplierFowstName');
                    //    else
                    //        sessionStorage.SupplierFowstName = e.FowstName;

                    //    var vData;
                    //    vData = SalesMenu_offline().getSupplierPriceLevel(e.ID, sessionStorage.FowstID);

                    //    if (vData.Rows.length > 0) {

                    //        vSupplier = vData.Rows[0];


                    //        sessionStorage.SupplierPriceLevelID = CStr2(vSupplier.POPriceLevel);
                    //        sessionStorage.SupplierPriceLevelName = CStr2(vSupplier.PriceLevel);


                    //    } else {
                    //        //TODO. if no default price level temporarily set to SRP because SRP is required so every org product must have it
                    //        sessionStorage.SupplierPriceLevelID = 'EEF716C6-B290-4B28-B2A6-0A4F52CF4989';
                    //        sessionStorage.SupplierPriceLevelName = 'SRP (Suggested Retail Price)';

                    //    }

                    //};
                    //Browser.LibraryOrganization.Show(null, {
                    //    description: proj.POMode ? 'Please choose an external vendor/supplier or an internal FOWST (branch or warehouse location) that you want to purchase/get stocks from.' : ''
                    //});
                }

                function BrowsePoCustomer() {
                    if (mPrePos.PurchaseOrder == null)
                        mPrePos.PurchaseOrder = new App.controller.PurchaseOrder();
                    mPrePos.PurchaseOrder.ok = function (vData) {
                        $('#lblCurrentPOSMode, #lblCurrentPOSModeHotButtons')
                            .text('Customer: ' + vData.CompanyName + ' - ' + vData.FowstName)
                            .data('id', vData.OrgID);

                        sessionStorage.CustomerOrganizationID = CStr2(vData.OrgID);
                        sessionStorage.CustomerOrgName = CStr2(vData.CompanyName);
                        sessionStorage.CustomerAddress = CStr2(vData.Address);
                        if (vData.FowstID == undefined) {
                            sessionStorage.removeItem('CustomerFowstID');
                        } else {
                            sessionStorage.CustomerFowstID = vData.FowstID;
                        }
                        if (vData.FowstName == undefined)
                            sessionStorage.removeItem('CustomerFowstName');
                        else
                            sessionStorage.CustomerFowstName = vData.FowstName;
                        if (IsGuid(vData.BinID)) {
                            sessionStorage.CustomerBinID = vData.BinID;
                            sessionStorage.CustomerBinName = vData.BinName;

                        } else {
                            sessionStorage.removeItem('CustomerBinID');
                            sessionStorage.removeItem('CustomerBinName');
                        }

                        if (IsGuid(vData.NetSuiteSupplierFowstID))
                            sessionStorage.SupplierFowstID = vData.NetSuiteSupplierFowstID;

                        mPrePos.PurchaseOrder.SaveCustomer(sessionStorage.CartOrderID, null, true, vData.NetSuiteSupplierFowstID);
                    };

                    (function () {
                        var pPoInput = {
                            OrgID: sessionStorage.CustomerOrganizationID,
                            FowstID: sessionStorage.CustomerFowstID,
                            BinName: sessionStorage.CustomerBinName,
                            Title: 'Customer'
                        };
                        var pSetting = Business.SystemDefaults.PurchaseOrderCustomerSetting();
                        var pDefaultOrgID;
                        var pDefaultFowstID;

                        if (pSetting == 'locked to') {
                            pDefaultOrgID = Business.SystemDefaults.PurchaseOrderCustomerID();
                            if (IsGuid(pDefaultOrgID)) {
                                pPoInput.LockedOrg = true;
                                pPoInput.OrgID = pDefaultOrgID;
                            }
                            pDefaultFowstID = Business.SystemDefaults.PurchaseOrderCustomerFowstID();
                            if (IsGuid(pDefaultFowstID)) {
                                pPoInput.LockedFowst = true;
                                pPoInput.FowstID = pDefaultFowstID;
                            }
                        }
                        if (pSetting == 'default to') {
                            if (!IsGuid(pPoInput.OrgID)) {
                                pDefaultOrgID = Business.SystemDefaults.PurchaseOrderCustomerID();
                                if (IsGuid(pDefaultOrgID)) {
                                    pPoInput.OrgID = pDefaultOrgID;
                                    pDefaultFowstID = Business.SystemDefaults.PurchaseOrderCustomerFowstID();
                                    if (IsGuid(pDefaultFowstID)) {
                                        pPoInput.FowstID = pDefaultFowstID;
                                        pPoInput.LockedFowst = pPoInput.LockedOrg;
                                    }
                                }
                            }
                        }
                        mPrePos.PurchaseOrder.popup(pPoInput);
                    })();

                } // END function BrowsePoCustomer() {

                function SaveCustomerDetails(refNoSave) {
                    var pUrl;
                    var pData;


                    //                    pData = {
                    //                        //                        InvoiceID: sessionStorage.getItem('CartOrderID'),
                    //                        //                        CustomerNameReferenceOnly: sessionStorage.CustomerNameReferenceOnly,
                    //                        //                        CustomerReferenceNumber: sessionStorage.CustomerReferenceNumber,
                    //                        //                        TIN: sessionStorage.TIN,
                    //                        //                        Address: sessionStorage.Address,
                    //                        CustomerTableNo: sessionStorage.CustomerTableNo
                    //                        //                        CustomerTableNoOfPeople: sessionStorage.CustomerTableNoOfPeople
                    //                    };


                    AjaxSend({
                        url: 'SalesMenu/SaveCustomerDetails',
                        data: { InvoiceID: sessionStorage.getItem('CartOrderID'), CustomerTableNo: refNoSave },
                        success: function (vData) {
                            // proceed to payment only when the conditions here are met
                            if (vData.Result == 'ok')
                                SaveSalesOrder();
                            else
                                alert('error');

                        },
                        error: function (vError) {
                            var pDummy;
                        }
                    });

                }

                $('body').delegate('#btnPOSMode', 'click', function (e) { // #btnPOSModeHotButtons
                    e.preventDefault();
                    if (proj.POMode)
                        BrowsePoCustomer();
                    else
                        if (GetUrlParameters().TableNo != undefined) {
                            proj.SaveCart();
                            sessionStorage.PosMode = 'Table View Mode';
                            proj.ProductID = '';
                            if (window.InAndroid) {
                                //Android.SetCurrentTab(0);
                                proj.SalesMenuInit();
                                if ($.isFunction(GetStringObject('proj.SalesPayments.ClearSalesPaymentsInfoFields')))
                                    proj.SalesPayments.ClearSalesPaymentsInfoFields();
                                $('#divHeader, #divHotButtons').show();
                                $('#divPayment').hide();
                                //var pBaseLocation = '/SalesMenu';
                                //if ($(window).width() > proj.WideWidth) //
                                //    pBaseLocation += '/BsPOSViewAll';
                                //var pLocation = MergeUrlParameters({
                                //    OrgID: sessionStorage["OrgID"],
                                //    mode: 'pos',
                                //    IsMobile: IsMobile ? 1 : 0,
                                //    // TableView: 'true', 
                                //    TableNo: 'js998QYHQHXH!'
                                //}, pBaseLocation);
                                //location = pLocation;
                            } else
                                location = MergeUrlParameters({
                                    OrgID: sessionStorage["OrgID"],
                                    mode: 'pos',
                                    IsMobile: IsMobile ? 1 : 0
                                }, '/TableView/NewTable');
                        }
                        else {
                            OptionPOSMode();
                        }


                    return false;
                });

                window.OptionPOSMode = OptionPOSMode;
                function OptionPOSMode() {
                    if (proj.PosMode) {
                        var vData = [
                        //                                {
                        //                                    ID: 1,
                        //                                    Text: 'Expedite Mode',
                        //                                    IsActive: 1,
                        //                                    Description: 'Lets you see which customers have pending orders so that you can help <br/>  expedite their order. This also lets you view which ones have <br/>open tabs and it also lets you close their tabs via cart checkout.'
                        //                                },
                        //                                {
                        //                                    ID: 2,
                        //                                    Text: 'Kiosk Mode',
                        //                                    IsActive: 1,
                        //                                    Description: 'Used for customer self-ordering. '
                        //                                },
                        {
                            ID: 3,
                            Text: 'Retail Mode',
                            IsActive: 1,
                            Description: 'Makes it easy for retailers to search for SKUs using keyboard entry or bar code / QR code scanning (best for retailers with hundreds or thousands of SKUs).'
                        }
                        ,
                        {
                            ID: 4,
                            Text: 'Hot Button Mode',
                            IsActive: 1,
                            Description: 'Shows predefined buttons sorted by categories (best for fast food outlets and retailers whose items do not have UPC or bar code labels). Administrators can customize these buttons online.'
                        },
                        {
                            ID: 5,
                            Text: 'Table View Mode',
                            IsActive: 1,
                            Description: 'Lets you view dining patrons as they are seated in tables (best for fine dining restaurants that require a high-touch service). '
                        },
                        //{
                        //    ID: 8,
                        //    Text: 'Fast Moving Consumer Goods Mode',
                        //    IsActive: 1,
                        //    Description: '',
                        //    Key: 'fmcg'
                        //},
                        {
                            ID: 10,
                            Text: 'FMCG Mode (DOS)',
                            IsActive: 1,
                            Description: 'Fast Moving Consumer Goods',
                        },
                        {
                            ID: 11,
                            Text: 'FMCG Mode (Modern)',
                            IsActive: 1,
                            Description: 'Fast Moving Consumer Goods',
                        }
                        //, {
                        //    ID: 9,
                        //    Text: 'Rentable Assets View',
                        //    IsActive: 1,
                        //    Description: ''
                        //}
                        ];
                    }
                    else {
                        var vData = [
                            {
                                ID: 6,
                                Text: 'Straight Ordering Mode',
                                IsActive: 1,
                                Description: 'Makes it easy for retailers to search for SKUs using keyboard entry or bar code / QR code scanning (best for retailers with hundreds or thousands of SKUs).'
                            }
                            //,
                            //{
                            //    ID: 7,
                            //    Text: 'Inventory Based Ordering Mode',
                            //    IsActive: 1,
                            //    Description: '<br/> <br/>'
                            //},
                        ];
                    }

                    for (var i = 0; i < vData.length; i++) {
                        vData[i].Click = function (e) {
                            var pButtonData = e;

                            // if hot button view all mode
                            if ((pButtonData.ID == '4')
                                || (pButtonData.Key == 'fmcg')) {

                                sessionStorage.FmcgMode = CBool(pButtonData.Key == 'fmcg');
                                if (OfflineMode) {
                                    // goToHotButtonPage();
                                    if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {
                                        //                                        sessionStorage.PosMode = 'Widescreen';
                                        if (OfflineMode) {
                                            Android.Toast('You are already in Hot Button Mode');
                                            //$('#popupPOSMode').popup('close');
                                        } else {
                                            NewMessageBox("You are already in Hot Button Mode",
                                                "POS Mode",
                                                [
                                                    { Text: 'Ok' }
                                                ]
                                            );
                                        }
                                    } else {

                                        if (proj.PosMode) {
                                            if ($(window).width() > proj.WideWidth) {
                                                location = MergeUrlParameters({
                                                    OrgID: sessionStorage["OrgID"],
                                                    mode: 'pos',
                                                    IsMobile: IsMobile ? 1 : 0,
                                                    theme: 'bs'
                                                }, '/SalesMenu/BsPOSViewAll');
                                            } else {
                                                PopupButtonList('', 'Hot Button Mode Options', [
                                                    {
                                                        Text: 'With side cart',
                                                        Description: 'For widescreen displays, use this mode.',
                                                        Click: function () {
                                                            location = MergeUrlParameters({
                                                                OrgID: sessionStorage["OrgID"],
                                                                mode: 'pos',
                                                                IsMobile: IsMobile ? 1 : 0,
                                                                theme: 'bs'
                                                            }, '/SalesMenu/BsPOSViewAll');
                                                        }
                                                    },
                                                    {
                                                        Text: 'Without side cart',
                                                        Click: function () {
                                                            goToHotButtonPage();
                                                        }
                                                    }
                                                ])
                                            }
                                        } else
                                            goToHotButtonPage();
                                    }
                                } else
                                    goToHotButtonPage();


                            }


                            //TABLE VIEW MODE
                            if (pButtonData.ID == '5') {
                                sessionStorage.FmcgMode = false;
                                if (OfflineMode) {
                                    if (window.location.pathname.toLowerCase() == '/tableview/BsNewTable') {
                                        Android.Toast('You are already in Table View Mode');
                                        //$('#popupPOSMode').popup('close');
                                    } else {
                                        goToTableViewPage();
                                    }
                                } else {
                                    NewMessageBox("Table View Mode is only available on the mobile app",
                                                "Table View Mode",
                                                [
                                                    { Text: 'Ok' }
                                                ]
                                        );
                                }
                            }
                            //////////////////

                            // if retail mode
                            if (pButtonData.ID == '3') {
                                sessionStorage.FmcgMode = false;
                                //                                    pageCustomerDetails.setCustomerInfo();
                                if (window.location.pathname.toLowerCase() == '/salesmenu/bsindex' && sessionStorage.PosMode == 'Retail Mode') {
                                    //                                        sessionStorage.PosMode = 'Retail Mode';
                                    if (OfflineMode) {
                                        Android.Toast('You are already in Retail Mode');
                                        //$('#popupPOSMode').popup('close');
                                    } else {
                                        NewMessageBox("You are already in Retail Mode",
                                                "POS Mode",
                                                [
                                                    { Text: 'Ok' }
                                                ]
                                        );
                                    }
                                } else {

                                    sessionStorage.PosMode = 'Retail Mode';
                                    //check if the customer is same or not
                                    //if yes, then display his cart, if not, reload the cart page tag new cart
                                    //origCustomerID == sessionStorage.CustomerOrganizationID && localStorage.NewCartMode != 1

                                    if (sessionStorage.getItem('CartOrderID') != 'undefined' && localStorage.NewCartMode != 1 && window.location.pathname.toLowerCase() != '/salesmenu/bsposviewall') {
                                        proj.ShowCart();
                                    } else {
                                        NewCart();

                                        if ($(window).width() > proj.WideWidth) {
                                            var pParams = GetUrlParameters();

                                            location = MergeUrlParameters({
                                                mode: pParams.mode,
                                                OrgID: sessionStorage.OrgID,
                                                rsd: pParams.rsd,
                                                IsMobile: CBool(IsMobile) ? 1 : 0,
                                                theme: 'bs'
                                            }, '/SalesMenu/BsIndex');
                                        } else {
                                            var url = new URL(location.href);
                                            url.searchParams.delete('HotButtons');

                                            history.pushState({}, '', url.toString());

                                            $('#pageMain').pageshow();
                                        }
                                    }

                                }
                            }

                            // Same as Retail Mode
                            if (pButtonData.ID == '6') {
                                sessionStorage.PosMode = 'Retail Mode';

                                $('#lblCurrentPOSMode').text($('#btnPOS_6').find('h3:first').text());

                                sessionStorage.IsInventoryCountMode = false;

                                //$('#popupPOSMode').popup('close');
                            }

                            if (pButtonData.ID == '7') {
                                sessionStorage.PosMode = 'Inventory Based Ordering Mode';

                                sessionStorage.IsInventoryCountMode = true;

                                $('#lblCurrentPOSMode').text($('#btnPOS_7').find('h3:first').text());

                                //$('#popupPOSMode').popup('close');

                            }

                            if (pButtonData.Key == 'fmcg') {
                                window.location = '/SalesMenu/FMCG';
                            }

                            if (pButtonData.ID == '9') {
                                sessionStorage.PosMode = 'Rentable Assets View';

                                $('#lblCurrentPOSMode').text($('#btnPOS_9').find('h3:first').text());
                                //$('#popupPOSMode').popup('close');

                                gotoRentableAssetsView();
                            }
                            if (pButtonData.ID == '10') {
                                location = MergeUrlParameters({
                                    OrgID: sessionStorage["OrgID"],
                                    mode: 'pos',
                                    IsMobile: IsMobile ? 1 : 0
                                }, '/SalesMenu/FMCGBasic');
                            }
                            if (pButtonData.ID == '11') {
                                location = MergeUrlParameters({
                                    OrgID: sessionStorage["OrgID"],
                                    mode: 'pos',
                                    IsMobile: IsMobile ? 1 : 0
                                }, '/SalesMenu/FMCG');
                            }
                        }
                    }

                    PopupButtonList('', 'POS Mode', vData, {
                        TextAlign: 'center'
                    })
                }

                //Edited by Edmund R. Uba - August 14, 2015
                $('#btnMoreOptions').click(function (e) {
                    e.preventDefault();

                    if (GetUrlParameters().TableNo != undefined) {
                        var pButtons = [
                            {
                                Text: 'Print Billing',
                                Size: 3,
                                Click: function () {
                                    sessionStorage.PrintOption = 'billing';
                                    mPrintedOrderSlip = false;
                                    proj.SaveCart({
                                        ForceSaveHeader: true
                                    });
                                    SalesMenu_offline().SaveSalesOrder({
                                        InvoiceID: sessionStorage.getItem('CartOrderID')
                                    });
                                    proj.SalesMenu.PrintBilling(sessionStorage.getItem('CartOrderID'));
                                }
                            },
                            {
                                Text: 'Print Order Slip (and cancellation slip if applicable)',
                                Size: 3,
                                Click: function () {
                                    if (!proj.HandlePosIsEditable(true))
                                        return false;
                                    var pResult;
                                    sessionStorage.PrintOption = 'orderslip';
                                    mPrintedOrderSlip = true;
                                    proj.SaveCart();
                                    pResult = SalesMenu_offline().SaveSalesOrder({
                                        InvoiceID: sessionStorage.getItem('CartOrderID'),
                                        AsOrderSlip: true
                                    });
                                    proj.SalesMenu.PrintBilling(sessionStorage.getItem('CartOrderID'), {
                                        PrintAsOrderSlip: true,
                                        ByCategory: true
                                    });
                                }
                            },
                            {
                                Text: 'Print Dispatch Slip',
                                Size: 3,
                                Click: function () {
                                    var pResult;
                                    sessionStorage.PrintOption = 'dispatchslip';
                                    mPrintedOrderSlip = false;
                                    proj.SaveCart();
                                    pResult = SalesMenu_offline().SaveSalesOrder({
                                        InvoiceID: sessionStorage.getItem('CartOrderID'),
                                        AsOrderSlip: true
                                    });
                                    PrintDispatchSlip(sessionStorage.getItem('CartOrderID'), {
                                        ByCategory: false,
                                        DtPrinter: null
                                    });
                                }
                            },
                            {
                                Text: 'Assign Wait Staff',
                                Size: 3,
                                Click: function () {
                                    $('#butAssignRep').click();
                                }
                            }
                        ];
                        if (window.HotButtons)
                            pButtons.push(
                                {
                                    Text: 'Change Hot Button Template',
                                    Size: 3,
                                    Click: window.HotButtons.ChangeHotButtonsTemplate
                                }
                            );
                        //NewMessageBox('', 'Print Options', pButtons, { CloseButton: 'enable', AutoDismiss: true });
                        // hidden for now 2016_09_29__13_00_56_3362
                        //{
                        //    Text: 'Print Billing and Order Slip',
                        //    Size: 3,
                        //    Click: function () {
                        //        sessionStorage.PrintOption = 'billingorderslip';
                        //        proj.SaveCart();
                        //        proj.SalesMenu.PrintBilling(sessionStorage.getItem('CartOrderID'));
                        //        proj.SalesMenu.PrintBilling(sessionStorage.getItem('CartOrderID'), {
                        //            PrintAsOrderSlip: true,
                        //            ByCategory: true
                        //        });
                        //    }
                        //},
                    }
                    else {
                        var pButtons = [
                            //{
                            //    Text: 'Switch to old ' + (sessionStorage.PosMode == 'Widescreen' ? 'Hot Button with sidecart' : 'Retail View') + ' UI',
                            //    Description: '&nbsp',
                            //    Click: function () {
                            //        if (sessionStorage.PosMode == 'Widescreen') {
                            //            location = MergeUrlParameters({
                            //                OrgID: sessionStorage["OrgID"],
                            //                mode: 'pos',
                            //                IsMobile: IsMobile ? 1 : 0
                            //            }, '/SalesMenu/POSViewAll');
                            //        } else {
                            //            location = MergeUrlParameters({
                            //                OrgID: sessionStorage["OrgID"],
                            //                mode: 'pos',
                            //                IsMobile: IsMobile ? 1 : 0
                            //            }, '/SalesMenu');
                            //        }
                            //    }
                            //},
                            //{
                            //    Text: 'Current FOWST Location: ' + $('.lblPosBranch:first').text(),
                            //    Description: 'Sales, inventory movements and payments will be tagged to this location.',
                            //    Click: function (e) {
                            //        mPrePos.BrowseAddresses();
                            //    }
                            //},
                            //{
                            //    Text: 'POS Mode',
                            //    Description: 'Current POS mode is ' + $('#lblCurrentPOSMode').text() + '. Click to change POS mode.',
                            //    Click: function (e) {
                            //        setTimeout(function () {
                            //            OptionPOSMode();
                            //        }, 500)
                            //    }
                            //},
                            {
                                Text: 'New Cart',
                                Description: 'Puts the current cart on hold and displays a new one for the next customer.',
                                Click: function (e) {
                                    var pCount = 0;

                                    //additional by janine
                                    //$('#divRecentlyBoughtPanel').hide();
                                    $('#divCartPanel, #pageMain').pageshow();

                                    if (proj.PosMode) {
                                        if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall')
                                            $('#lblPayNow').html('Go');
                                        else {
                                            if ($.isFunction(GetStringObject('GetIsOrderTaker', mOrderTakerSetupModel)))
                                                if (mOrderTakerSetupModel.GetIsOrderTaker())
                                                    $('#lblPayNow').html('Go');
                                                else
                                                    $('#lblPayNow').html('Pay Now');
                                            else
                                                $('#lblPayNow').html('Pay Now');
                                        }

                                    } else {
                                        $('#lblPayNow').html('Save Sales Order');
                                    }

                                    isOnCartPage = true;

                                    if (mLvCart)
                                        pCount = mLvCart.vData.Stocks.Rows.length;

                                    NewMessageBox('You have ' + pCount + ' items on your current cart.' +
                                        ' Are you sure you want to create a New Cart?' +
                                        ' If yes, the contents of your current cart will be placed “On Hold”.  We recommend that you complete your current cart first before creating a new cart.', 'New Cart', [
                                        {
                                            Text: 'Continue with Current Cart'
                                        }, {
                                            Text: 'Create New Cart',
                                            Click: function () {
                                                //Clears the sidecart once you will be having a new cart
                                                proj.SaveCart();
                                                $("#pushSection").empty();
                                                proj.CartStock = [];
                                                proj.CartData = null;
                                                CreateNewCart();
                                                //////////                if (OfflineMode) {
                                                //////////                    GotoFirstPage(); //Added by Janine Oct 19 3pm
                                                //////////                    NewCart();
                                                //////////                    Search();
                                                //////////                } else
                                                //////////                    window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID); //Added by JAnine 0ct 22. clear contents

                                            }
                                        }
                                        ]);
                                }
                            },
                            {
                                Text: 'Recall Cart',
                                Description: 'Used to retrieve carts that were placed on hold.',
                                Click: function (e) {
                                    lnkSalesOrder_Click(e);
                                }
                            },
                            {
                                Text: 'Reprint Receipt',
                                Description: 'Allows you to reprint the receipt of your last transaction in case it did not print upon payment.',
                                Click: function (e) {
                                    ShowDuplicateReceipt();
                                }
                            },
                            //{
                            //    Text: 'Sales and BOM Usage Quantities',
                            //    Description: 'description here.',
                            //    Click: function (e) {
                            //        ShowTransPimSalesCaching();
                            //    }
                            //}
                        ]

                        if (window.location.pathname.toLowerCase() != '/salesmenu/bsposviewall') {
                            //pButtons.push({
                            //    Text: 'Recently Bought',
                            //    Description: 'View recently bought items of the selected customer. Useful for regular customers who buy similar items frequently.',
                            //    Click: function (e) {
                            //        //$('#popupMoreOptions').popup('close');
                            //        if (proj.POMode) {
                            //            lnkSalesOrder_Click(e, {
                            //                SaveAsNew: true,
                            //                UserID: Web.PeopleUserID()
                            //            });
                            //        } else {
                            //            $('#divRecentlyBoughtPanel').pageshow();
                            //            //$('#divCartPanel').hide();
                            //            isOnCartPage = false;
                            //            $('#lblPayNow').html('View Cart');

                            //            LoadRecentlyBought();
                            //        }
                            //    }
                            //})
                        }

                        //gicomment kay walay mahitabo pag-iclick --- by arnie 1-13-2021
                        //if (!Business.SystemDefaults.Only1SalesRep()) {
                        //    pButtons.push({
                        //        Text: 'Assign Sales Representatives',
                        //        Description: 'Assign sales representatives for this transaction',
                        //        Click: function (e) {

                        //        }
                        //    })
                        //}

                        var vMenu = SalesMenu_offline().GetSelectedFowstDefaultHBTemplate();

                        if (vMenu.length == 0) {
                            vMenu = SalesMenu_offline().getPimMenu('');
                        }

                        if (window.HotButtons != undefined && vMenu.Rows.length > 1)
                            pButtons.push(
                                {
                                    Text: 'Change Hot Button Template',
                                    Description: 'Lets you use a different set of hot buttons. Administrators can customize these buttons online.',
                                    Click: window.HotButtons.ChangeHotButtonsTemplate
                                }
                            );

                        if (Business.LibraryOrganization.CanTestPrice()) {
                            pButtons.push({
                                Text: 'Test Price Request',
                                Description: 'Test your requested prices for this this transaction',
                                Click: function (e) {

                                }
                            })
                        }
                    }

                    PopupButtonList('', 'More Options', pButtons, {
                        TextAlign: 'center'
                    })
                });
                //End of edit

                var htmlDuplicateReceipt = '';
                window.ShowDuplicateReceipt = function () {
                    $('#pageDuplicateReceipt').pageshow();

                    if (htmlDuplicateReceipt == '') {
                        AjaxSend({
                            ExactUrl: MergeUrlParameters({
                                ref: 'pos',
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, "/SalesPayments/BsDuplicateReceipt"),
                            type: 'GET',
                            dataType: 'text',
                            success: function (vData) {
                                htmlDuplicateReceipt = vData;
                                $('#pageDuplicateReceipt').empty().append(vData).pageshow();
                                proj.ShowPaymentPage(true)
                            }
                        });
                    } else {
                        DuplicateReceipt.ClearFields();
                    }
                }

                //More Options

                //  2017_05_09__17_44_36_6366 commented by Arvin
                //$('#btnChangeMenuButtons').click(function () {
                //    ChangePage('/SalesMenu/HotButtons');
                //    // ChangePage('/SalesReturns/Stock');
                //}).show();

                $('#butTestPriceRequest').click(function (e) {
                    SalesMenu_offline().TestPrice();
                });

                proj.NewCartPos = CreateNewCart;
                function CreateNewCart() {
                    $("#pushSection").empty();
                    $('#tblSideCartItems tbody').remove();
                    proj.CartStock = [];
                    proj.CartData = null;
                    if (OfflineMode) {
                        proj.ItemsInCart = [];
                        GotoFirstPage();
                        mPrePos.NewCart();
                        isCartEmpty = true;
                        sessionStorage.removeItem('CustomerOrganizationID'); //reset to customer org para mahimong cash ang default
                        //                        LoadCustomer();
                        if (pageCustomerDetails.LoadCustomer()) {
                            //ADDITIONAL BY JANINE FEB 18
                            LoadRecentlyBought({ DateRangeType: mRecentlyBoughtDateRangeType });

                            //                            NewCart(); // march 26, 2013
                            //                            isCartEmpty = true;


                            //                            pageCustomerDetails.setCustomerInfo();
                        }

                        //                            Search();
                    } else {
                        if (window.location.pathname.toLowerCase() == '/salesmenu') {
                            //window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);
                            window.location = location = MergeUrlParameters({
                                mode: (proj.PosMode ? "pos" : "so"),
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/SalesMenu');

                        } else {

                            location = MergeUrlParameters({
                                OrgID: sessionStorage["OrgID"],
                                mode: (proj.PosMode ? "pos" : "so"),
                                IsMobile: IsMobile ? 1 : 0
                            }, '/SalesMenu/BsPOSViewAll');

                        } //end if location
                        //                        window.location = GetPathFormat("/SalesMenu?mode=" + (proj.PosMode ? "pos" : "so") + "&OrgID=" + sessionStorage.OrgID); //Added by JAnine 0ct 22. clear contents
                    }
                }

                ////////////////////////////////////////////////////////
                //SALES ORDER TAKER POPUP

                $('#selPOMonth').change(function (e) {
                    e.preventDefault();

                    setDayCollection('selPOMonth', 'selPODay', 'selPOYear', true);

                });

                $('#selPOYear').change(function (e) {
                    e.preventDefault();

                    setDayCollection('selPOMonth', 'selPODay', 'selPOYear', true);

                });
                $('#selDeliveryMonth').change(function (e) {
                    e.preventDefault();

                    setDayCollection('selDeliveryMonth', 'selDeliveryDay', 'selDeliveryYear', true);

                });

                $('#selDeliveryYear').change(function (e) {
                    e.preventDefault();

                    setDayCollection('selDeliveryMonth', 'selDeliveryDay', 'selDeliveryYear', true);

                });
                $('#btnDeliveryAddressSave').click(function (e) {

                    if ($.trim($("#txtDeliveryStreetNo").val()) == "") {
                        Android.Toast('Please enter street / building number.');
                        return;
                    }
                    if ($.trim($("#txtDeliveryAddress").val()) == "") {
                        Android.Toast('Please enter street name.');
                        return;
                    }
                    if ($.trim($("#selDeliveryCity").val()) == "") {
                        Android.Toast('Please select city.');
                        return;
                    }
                    if ($.trim($("#selDeliveryCountry").val()) == "") {
                        Android.Toast('Please select country.');
                        return;
                    }


                    var pData = {
                        StreetNumber: $.trim($('#txtDeliveryStreetNo').val()),
                        Street: $.trim($('#txtDeliveryAddress').val()),
                        FowstCityID: $.trim($('#selDeliveryCity').val()),
                        FowstCountryID: $.trim($('#selDeliveryCountry').val()),
                        OrganizationID: sessionStorage.CustomerOrganizationID
                    };

                    var vData = SalesPayment_offline().SaveOrganizationFOWST(pData);

                    if (vData) {
                        popSalesMenuOrderTaker.Show();
                        popSalesMenuOrderTaker.Clear();
                        popSalesMenuOrderTaker.LoadCombo();
                        getSalesOrderCountryName();
                        getSalesOrderCityName();
                        getSalesOrderFowsts();


                        $('#divCollapse').trigger('collapse');

                    }
                });


                ///////////////////////
                //additional by janine sept 2
                function getSalesOrderFowsts() {

                    if (OfflineMode) {
                        var vFowsts;
                        vFowsts = SalesMenu_offline().getFowsts();
                        window.popSalesMenuOrderTaker.FillFowsts(vFowsts);
                    } else {
                        //                    AjaxSend({
                        //                        url: 'City/getFowsts',
                        //                        success: function (vData) {

                        //                            DisplayCities(vData);

                        //                        },
                        //                        error: function (vError) {
                        //                            var pDummy;
                        //                        }
                        //                    });
                    }

                }


                //additional ny janine aug 6
                function getSalesOrderCityName() {
                    if (OfflineMode) {
                        var vCity;
                        vCity = SalesMenu_offline().getCityListPH();
                        window.popSalesMenuOrderTaker.FillCities(vCity);
                    } else {
                        AjaxSend({
                            url: 'City/getCityListAll',
                            success: function (vData) {

                                DisplaySalesOrderCities(vData);

                            },
                            error: function (vError) {
                                var pDummy;
                            }
                        });
                    }

                }


                function getSalesOrderCountryName() {
                    if (OfflineMode) {
                        var vCountry;
                        vCountry = SalesMenu_offline().getCountryListAll();
                        window.popSalesMenuOrderTaker.FillCountries(vCountry);
                    } else {
                        AjaxSend({
                            url: 'Country/getCountryListAll',
                            success: function (vData) {

                                DisplaySalesOrderCountries(vData);

                            },
                            error: function (vError) {
                                var pDummy;
                            }
                        });
                    }

                }
                function DisplaySalesOrderCountries(vData) {
                    var vRows = vData.CountryData.Rows;

                    selDeliveryCountry.empty();

                    selDeliveryCountry.append(new Option('Please Select Country', ''));

                    for (var p = 0; p <= vRows.length - 1; p++) {
                        selDeliveryCountry.append(new Option(vRows[p].Country, vRows[p].ID));

                    }

                    $("#selDeliveryCountry option:contains('Philippines')").attr('selected', 'selected');

                    selDeliveryCountry.selectmenu('refresh', true);

                }

                function DisplaySalesOrderCities(vData) {
                    var vRows = vData.CityData.Rows;

                    selDeliveryCity.empty();

                    selDeliveryCity.append(new Option('Please Select City', ''));

                    for (var p = 0; p <= vRows.length - 1; p++) {
                        selDeliveryCity.append(new Option(vRows[p].City, vRows[p].ID));

                    }
                    selDeliveryCity.selectmenu('refresh', true);

                }

                function DisplaySalesOrderFowsts(vData) {
                    var vRows = vData.FowstData.Rows;

                    cbFOWSTOrderTaker.empty();

                    cbFOWSTOrderTaker.append(new Option('Please Select FOWST', ''));

                    for (var p = 0; p <= vRows.length - 1; p++) {
                        cbFOWSTOrderTaker.append(new Option(vRows[p].Name, vRows[p].ID));

                    }


                    $('#selFOWSTOrderTaker option[value="' + sessionStorage.FowstID + '"]').prop('selected', true);
                    cbFOWSTOrderTaker.selectmenu('refresh', true);

                }

                function FlagInventoryCountSalesOrderID() {
                    var pSql = '';
                    var pDT;
                    var pDTTemp;

                    pSql = "Select ProductID from TransInvoiceItems " +
                "Where TransInvoiceItems.InvoiceID = '" + sessionStorage.getItem('CartOrderID') + "' ";

                    pDT = Sql.Execute(pSql);

                    for (i = 0; i < pDT.Rows.length; i++) {
                        pSql = "Select * from TransInventoryProductCount where ProductID = " + ReadyString(pDT.Rows[i].ProductID) +
            " AND FowstID = " + ReadyString(sessionStorage.CustomerOrganizationID)

                        pDTTemp = Sql.Execute(pSql);

                        if (pDTTemp.Rows.length > 0) {
                            pSql = "Update TransInventoryProductCount set SuggestedOrderDate = " + proj.SqliteNow +
                " Where ProductID = " + ReadyString(pDT.Rows[i].ProductID) +
                " AND SuggestedOrderDate IS NULL"
                            Sql.Execute(pSql);
                        }
                    }
                }

                function CreatePDFFile() {
                    var pSql = '';
                    var pDTOrganizationDetails;
                    var pDTInvoiceDetails;
                    var pDTItems;
                    var pHtml = '';

                    pSql = "SELECT rsetbr.OrganizationID " +
        " ,rsetorg.CompanyName as CorporateName " +
        " ,rsetplace.FowstID as OrganizationBranchID " +
        " ,rsettrdname.BusinessTradeName " +
        " ,IFNULL(NULLIF(TRIM(rsetbr.StreetNumber) || ' ', ' '), '') || " +
        " IFNULL(NULLIF(TRIM(rsetbr.Street) || ', ',', '), '') || " +
        " IFNULL(NULLIF(TRIM(cty.Name) || ', ',', '), '') || " +
        " IFNULL(NULLIF(TRIM(ctry.Name) || ', ',', '), '') as CompleteAddress " +
        " ,IFNULL((SELECT GROUP_CONCAT(IFNULL(cctry.CallingCode,'') || ' ' || IFNULL(careacode.CallingCode,'') || ' ' || IFNULL(corgfowsttel.ContactNo,'') || IFNULL(NULLIF('Ext. ' || corgfowsttel.ExtensionNo,'Ext. '),''),', ') " +
        "   FROM LibraryOrganizationFOWSTTel corgfowsttel " +
        "   INNER JOIN LibraryCountries cctry ON (corgfowsttel.CountryID = cctry.ID) " +
        "   INNER JOIN LibraryAreaCodes careacode ON (corgfowsttel.AreaCodeID = careacode.ID) " +
        "   WHERE corgfowsttel.FOWSTID = '" + sessionStorage.FowstID + "' " +
        "   ORDER BY IFNULL(cctry.CallingCode,'') || ' ' || IFNULL(careacode.CallingCode,'') || ' ' + IFNULL(corgfowsttel.ContactNo,'') || IFNULL(NULLIF('Ext. ' || corgfowsttel.ExtensionNo,'Ext. '),'') ASC " +
        "   ),'') as TelNo " +
        " ,rsetorg.TinNumber as TIN " +
        " ,devfowst.POSRegistrationNo as POSPermitNo " +
        " ,(SELECT DefaultValue FROM SystemDefaults WHERE Description = 'Mobiz POS Accreditation Number') as POSAccreditationNo " +
        " ,devfowst.BIRMachineNo as MIN " +
        " ,rset.IsShowSalesRep as IsShowSalesRep " +
        " FROM private.LibraryReceiptSettingsFowst rsetplace " +
        " INNER JOIN private.LibraryReceiptSettings rset ON (rsetplace.ReceiptSettingsID = rset.ID)" +
        " INNER JOIN LibraryOrganizationFOWST rsetbr ON (rsetplace.FowstID = rsetbr.ID)" +
        " INNER JOIN LibraryOrganization rsetorg ON (rsetbr.OrganizationID = rsetorg.ID)" +
        " LEFT JOIN LibraryOrganizationTradeName rsettrdname ON (rset.OrgTradeNameID = rsettrdname.ID)" +
        " LEFT JOIN LibraryCities cty ON (rsetbr.FowstCityID = cty.ID)" +
        " LEFT JOIN LibraryCountries ctry ON (rsetbr.FowstCountryID = ctry.ID)" +
        " LEFT JOIN private.LibraryDeviceFowst devfowst ON (rsetplace.FowstID = devfowst.FOWSTID AND devfowst.DeviceID = " + ReadyString(Web.DeviceID()) + ") " +
        " LEFT JOIN private.LibraryDevice dev ON (devfowst.DeviceID = dev.ID) " +
        " WHERE rsetplace.FowstID = '" + sessionStorage.FowstID + "' ";

                    var pDTOrganizationDetails = Sql.Execute(pSql);


                    pSql = "SELECT inv.ID " +
        " ,subscrbr.OrganizationID " +
        " ,inv.FowstID as OrganizationBranchID " +
        " ,inv.InvoiceNumber " +
        " ,inv.InvoiceDate " +
        " ,inv.SalesOrderNo " +
        " ,inv.SalesOrderDate " +
        " ,inv.CustomerID " +
        " ,(CASE inv.IsForGovReport " +
        "     WHEN 1 THEN " +
        "           (CASE " +
        "               WHEN cust.IsOrganization = 1 THEN cust.CompanyName " +
        "               WHEN cust.IsOrganization = 0 THEN " +
        "                   IFNULL(NULLIF(TRIM(cust.PersonSurname) || ', ',', '), '') || " +
        "                   IFNULL(NULLIF(TRIM(cust.PersonFirstName) || ' ',' '), '') || " +
        "                   IFNULL(NULLIF(TRIM(cust.PersonMiddleName) || ' ',' '), '') || " +
        "                   IFNULL(NULLIF(', ' || TRIM(cust.PersonSuffixName),', '), '') " +
        "           END) " +
        "     ELSE " +
        "       (CASE inv.DeliveryMethod  " +
        "       WHEN 'Pickup' THEN '[CASH PAYMENT]' " +
        "       ELSE  " +
        "           (CASE " +
        "               WHEN cust.IsOrganization = 1 THEN cust.CompanyName " +
        "               WHEN cust.IsOrganization = 0 THEN " +
        "                   IFNULL(NULLIF(TRIM(cust.PersonSurname) || ', ',', '), '') || " +
        "                   IFNULL(NULLIF(TRIM(cust.PersonFirstName) || ' ',' '), '') || " +
        "                   IFNULL(NULLIF(TRIM(cust.PersonMiddleName) || ' ',' '), '') || " +
        "                   IFNULL(NULLIF(', ' || TRIM(cust.PersonSuffixName),', '), '') " +
        "           END) " +
        "       END) " +
        " END) as CustomerName " +
        " ,(CASE inv.IsForGovReport " +
        "     WHEN 1 THEN " +
        "       (CASE inv.DeliveryMethod  " +
        "           WHEN 'Pickup' THEN IFNULL(NULLIF(TRIM(custbilladd.StreetNumber), ''), '') || " +
        "                              IFNULL(NULLIF(', ' || TRIM(custbilladd.Street),', '), '') || " +
        "                              IFNULL(NULLIF(', ' || TRIM(custbillcty.Name),', '), '') || " +
        "                              IFNULL(NULLIF(', ' || TRIM(custbillctry.Name),', '), '') " +
        "           ELSE IFNULL(NULLIF(TRIM(custdeladd.StreetNumber), ''), '') || " +
        "                IFNULL(NULLIF(', ' || TRIM(custdeladd.Street),', '), '') || " +
        "                IFNULL(NULLIF(', ' || TRIM(custdeladdcty.Name),', '), '') || " +
        "                IFNULL(NULLIF(', ' || TRIM(custdeladdctry.Name),', '), '') " +
        "           END) " +
        "     ELSE " +
        "       (CASE inv.DeliveryMethod  " +
        "           WHEN 'Pickup' THEN '' " +
        "           ELSE IFNULL(NULLIF(TRIM(custbilladd.StreetNumber), ''), '') || " +
        "                IFNULL(NULLIF(', ' || TRIM(custbilladd.Street),', '), '') || " +
        "                IFNULL(NULLIF(', ' || TRIM(custbillcty.Name),', '), '') || " +
        "                IFNULL(NULLIF(', ' || TRIM(custbillctry.Name),', '), '') " +
        "       END) " +
        " END) as CustomerAddress " +
        " ,(CASE inv.IsForGovReport " +
        "     WHEN 1 THEN cust.TinNumber  " +
        "     ELSE '' " +
        " END) as CustomerTIN " +
        " ,inv.BillingAddressID " +
        " ,(CASE inv.DeliveryMethod " +
        "     WHEN 'Pickup' THEN '' " +
        "     ELSE IFNULL(NULLIF(TRIM(custbilladd.StreetNumber), ''), '') || " +
        "          IFNULL(NULLIF(', ' || TRIM(custbilladd.Street),', '), '') || " +
        "          IFNULL(NULLIF(', ' || TRIM(custbillcty.Name),', '), '') || " +
        "          IFNULL(NULLIF(', ' || TRIM(custbillctry.Name),', '), '') " +
        " END) as BillingAddress " +
        " ,inv.DeliveryAddressID " +
        " ,(CASE inv.DeliveryMethod " +
        "     WHEN 'Pickup' THEN '' " +
        "     ELSE IFNULL(NULLIF(TRIM(custdeladd.StreetNumber), ''), '') || " +
        "          IFNULL(NULLIF(', ' || TRIM(custdeladd.Street),', '), '') || " +
        "          IFNULL(NULLIF(', ' || TRIM(custdeladdcty.Name),', '), '') || " +
        "          IFNULL(NULLIF(', ' || TRIM(custdeladdctry.Name),', '), '') " +
        " END) as DeliveryAddress " +
        " ,IFNULL(NULLIF(TRIM(cshr.SurName) || ', ',', '), '') || " +
        "  IFNULL(NULLIF(TRIM(cshr.FirstName) || ' ',' '), '') || " +
        "  IFNULL(NULLIF(TRIM(cshr.MiddleName) || ' ',' '), '') as CashierName " +
        " ,TRIM(IFNULL((SELECT DefaultValue FROM private.SystemDefaults WHERE Description = 'Receipt Footer Message'), 'This Serves as Your Sales Invoice.')) as ReceiptFooterMsg " +
        " ,inv.CustomerTableNo " +
        " FROM private.TransInvoice inv " +
        " INNER JOIN LibraryOrganization cust ON (inv.CustomerID = cust.ID) " +
        " LEFT JOIN LibraryCities custcty ON (cust.OfficeAddressCityID = custcty.ID) " +
        " LEFT JOIN LibraryCountries custctry ON (cust.OfficeAddressCountryID = custctry.ID) " +
        " LEFT JOIN LibraryOrganizationFOWST custbilladd ON (inv.BillingAddressID = custbilladd.ID) " +
        " LEFT JOIN LibraryCities custbillcty ON (custbilladd.FowstCityID = custbillcty.ID) " +
        " LEFT JOIN LibraryCountries custbillctry ON (custbilladd.FowstCountryID = custbillctry.ID) " +
        " LEFT JOIN LibraryOrganizationFOWST custdeladd ON (inv.DeliveryAddressID = custdeladd.ID) " +
        " LEFT JOIN LibraryCities custdeladdcty ON (custdeladd.FowstCityID = custdeladdcty.ID) " +
        " LEFT JOIN LibraryCountries custdeladdctry ON (custdeladd.FowstCountryID = custdeladdctry.ID) " +
        " LEFT JOIN LibraryOrganizationFOWST subscrbr ON (inv.FowstID = subscrbr.ID) " +
        " LEFT JOIN LibraryPeople cshr ON (inv.UserID = cshr.ID) " +
        " WHERE inv.ID = '" + sessionStorage.getItem('CartOrderID') + "' ";

                    pDTInvoiceDetails = Sql.Execute(pSql);


                    pSql = "SELECT invitem.ID " +
        " ,invitem.ProductID as StockID " +
        " ,prd.ProductNo as StockNumber " +
        " ,prd.ProductName as StockDescription " +
        " ,invitem.OrderedquantitY as Quantity " +
        " ,invitem.UnitMeasure " +
        " ,invitem.UnitPrice " +
        " ,invitem.SubTotal " +
        " ,(SELECT GROUP_CONCAT(ppl.Surname || ', ' || ppl.FirstName || ' ' || ppl.MiddleName,'; ') FROM private.TransInvoiceItemSalesReps srep INNER JOIN LibraryPeople ppl ON (srep.PeopleID = ppl.ID) WHERE srep.InvoiceID = invitem.InvoiceID AND srep.ProductID = invitem.ProductID ORDER BY (ppl.Surname || ', ' || ppl.FirstName || ' ' || ppl.MiddleName) ASC) as SalesPerson " +
        " FROM private.TransInvoiceItems invitem " +
        " INNER JOIN LibraryPIMProduct prd ON (invitem.ProductID = prd.ID) " +
        " WHERE invitem.InvoiceID = '" + sessionStorage.getItem('CartOrderID') + "' " +
        " ORDER BY prd.ProductNo ASC, " +
        " invitem.ActualQuantity DESC";

                    pDTItems = Sql.Execute(pSql);


                    pHtml += '<h1 style="text-align:center">' + pDTOrganizationDetails.Rows[0].CorporateName + '</h1>';
                    pHtml += '<h4 style="text-align:center; margin-top:0px">' + pDTOrganizationDetails.Rows[0].CompleteAddress + '</h4>';
                    pHtml += '<h4 style="text-align:center; margin-top:0px"> TIN: ' + pDTOrganizationDetails.Rows[0].TIN + '</h4>';

                    pHtml += '<h2 style="margin-top:30px">Suggested Order</h2>';
                    pHtml += '<p> <b>Branch/Outlet: </b>' + pDTInvoiceDetails.Rows[0].CustomerName + '</p>';
                    pHtml += '<p> <b>Branch/Outlet Address: </b>' + pDTInvoiceDetails.Rows[0].CustomerAddress + '</p>';
                    pHtml += '<p> <b>Suggested Order Date: </b>' + pDTInvoiceDetails.Rows[0].SalesOrderDate + '</p>';
                    pHtml += '<p> <b>Suggested Order No: </b>' + pDTInvoiceDetails.Rows[0].SalesOrderNo + '</p>';
                    pHtml += '<table style="margin-top:30px">';
                    pHtml += '<tr>';
                    pHtml += '<th> No </th>';
                    pHtml += '<th> Product </th>';
                    pHtml += '<th> Quantity </th>';
                    pHtml += '<th> Unit Measure </th>';
                    pHtml += '</tr>';

                    for (i = 0; i < pDTItems.Rows.length; i++) {
                        pHtml += '<tr>';
                        pHtml += '<td>' + (i + 1) + '</td>';
                        pHtml += '<td>' + pDTItems.Rows[i].StockNumber + '-' + pDTItems.Rows[i].StockDescription + '</td>';
                        pHtml += '<td align="right">' + pDTItems.Rows[i].Quantity + '</td>';
                        pHtml += '<td>' + pDTItems.Rows[i].UnitMeasure + '</td>';
                        pHtml += '</tr>';
                    }

                    pHtml += '</table>';
                    pHtml += '<p style="font-weight:bold; font-size:large; margin-top:90px"> Prepared By: ' + pDTInvoiceDetails.Rows[0].CashierName + '</p> <br>';
                    pHtml += '<p style="font-weight:bold; font-size:large; margin-top:90px"> Approved By: </p>';


                    DownloadSalesOrderPDF(pHtml, 'SO-' + pDTInvoiceDetails.Rows[0].SalesOrderNo);
                    IsDownloadedSuccessfully();

                    function IsDownloadedSuccessfully() {
                        NewMessageBox("Were you able to successfully download pdf file?",
                        'Sales Order',
                        [
                            {
                                Text: 'Yes',
                                Click: function () {
                                    popSalesMenuOrderTaker.Clear();
                                    CreateNewCart();
                                }
                            }
                        ,
                            {
                                Text: 'No',
                                Click: function () {
                                    DownloadSalesOrderPDF(pHtml, 'SO-' + pDTInvoiceDetails.Rows[0].SalesOrderNo);
                                    IsDownloadedSuccessfully();
                                }
                            }
                        ]
            );
                    }
                }

                function DownloadSalesOrderPDF(pHtml, pFileName) {

                    var p = $('<iframe id="pagePrintPDF" style="visibility:hidden">');

                    //p.attr('src', '/Scripts/PrintPDF/index.html?data=' + pHtml + '&filename=' + pFileName);

                    p.attr('src', '/Scripts/PrintPDF/index.html');

                    $('body').append(p);

                    $('#pagePrintPDF').load(function (e) {
                        $('#pagePrintPDF')[0].contentWindow.CreateAndDownloadPDF(pHtml, pFileName);
                    });
                }

                /////////////////////////////////////////////////////////
                ///////// billing functions/////////////////////////////
                ////////////////////////////////////////////////////////
                //ADDITIONAL BY JANINE FOR BILLING

                $('#btnBacktoTableViewBilling').click(function () {
                    location = MergeUrlParameters({
                        OrgID: sessionStorage["OrgID"],
                        mode: 'pos',
                        IsMobile: IsMobile ? 1 : 0
                    }, '/TableView/NewTable');
                    //location.reload();

                });

                sessionStorage.IsViewBill = 'false';
                $('#lnkPrintBill').click(function () {
                    if (isCartEmpty) {

                        NewMessageBox('Your shopping cart is empty',
                            'Pay Now',
                            [
                                { Text: 'Ok' }
                            ]
                        );
                        return;

                    } else {
                        sessionStorage.CurrentPage = 'discount';
                        IsPreviewBill = true;
                        sessionStorage.IsViewBill = 'true';
                        ViewOrPrintBill();
                    }


                });
                $('#btnPrintBilling').click(function () {
                    IsPreviewBill = false;
                    ViewOrPrintBill();
                });
                $('#btnBackPreviewBill').click(function () {

                    //   history.back();
                    $.mobile.changePage('');

                });
                $('#btnPaymentBilling').click(function () {

                    if (isCartEmpty) {

                        NewMessageBox('Your shopping cart is empty',
                            'Pay Now',
                            [
                                { Text: 'Ok' }
                            ]
                        );
                        return;

                    } else {
                        $.mobile.changePage('');
                        sessionStorage.IsViewBill = 'false';
                        sessionStorage.CurrentPage = 'payment';
                        proj.SalesMenu.PaymentsFunction();
                    }
                });


                var isChargesLoaded = false;
                $('#btnAddCharges').click(function () {

                    //$('#popupCharges').popup('open');

                    ShowCharges();

                    isChargesLoaded = true;

                });

                sessionStorage.IsDiscountPageLoaded = 'false';

                $('#btnAddDiscount').click(function () {
                    //$('#popupDiscounts').popup('open');
                    if (CStr2(sessionStorage.IsDiscountPageLoaded) != 'true') {

                        getOrgDiscountTypeBilling();
                    }

                    window.pageDiscounts.getCurrentDiscount();
                });

                $('#selDiscountTypeBilling').change(function (e) {
                    e.preventDefault();


                    var strAccessType = "";
                    $('#divAddCustomerDetailsBilling').hide();
                    showDiscountDetails = false;
                    isSeniorCitizenPWDDiscount = false;
                    isSeniorCitizenDiscount = false;
                    $("#txtDiscountPercentBilling").removeAttr("disabled");
                    $("#txtDiscountAmountBilling").removeAttr("disabled");


                    if ($('#selDiscountTypeBilling').val().toUpperCase() == '35F954A0-C67C-499C-AF60-F08E64477FDC') {
                        strAccessType = "Allow to grant VIP Discount";
                    } else if ($('#selDiscountTypeBilling').val().toUpperCase() == '764EDACB-0131-4033-8E44-7B903368008C') {
                        strAccessType = "Allow to grant Regular Discount 1";
                    } else if ($('#selDiscountTypeBilling').val().toUpperCase() == 'C4199347-6A00-45C9-BE11-8934DD63ECD1') {
                        strAccessType = "Allow to grant Regular Discount 2";
                    } else if ($('#selDiscountTypeBilling').val().toUpperCase() == '8E2DCA9C-433F-42FC-A644-04476DBA056A') {
                        strAccessType = "Allow to grant Regular Discount 3";
                    } else if ($('#selDiscountTypeBilling').val().toUpperCase() == '7804A35A-B531-4380-945E-FC9643E99843') {
                        strAccessType = "Allow to grant Employee Discount";
                    } else if ($('#selDiscountTypeBilling').val().toUpperCase() == '055A0A33-DD98-47EC-ADD3-8B2009F3944A') {
                        strAccessType = "Allow to grant Senior Citizens' (SC) Discount";
                        $('#divAddCustomerDetailsBilling').show();
                        $('#divSCPWDDiscountBilling').show();
                        showDiscountDetails = true;
                        isSeniorCitizenPWDDiscount = true;
                        isSeniorCitizenDiscount = true;
                        $('#txtDiscountPercentBilling').attr("disabled", "disabled");
                        $('#txtDiscountAmountBilling').attr("disabled", "disabled");
                        var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.getItem('CartOrderID'));

                        intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, false);

                        if (window.pageDiscounts.checkDiscountDetails(showDiscountDetails)) {

                        }
                    } else if ($('#selDiscountTypeBilling').val().toUpperCase() == '34A4032F-A142-4C15-A868-4AC4410434DF') {
                        strAccessType = "Allow to grant Person With Disability (PWD) Discount";
                        $('#divAddCustomerDetailsBilling').show();
                        $('#divSCPWDDiscountBilling').hide();
                        isSeniorCitizenPWDDiscount = true;
                        $('#txtDiscountPercentBilling').attr("disabled", "disabled");
                        $('#txtDiscountAmountBilling').attr("disabled", "disabled");
                        var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.getItem('CartOrderID'));

                        intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, false);

                        if (window.pageDiscounts.checkDiscountDetails(showDiscountDetails)) {

                        }
                    } else if ($('#selDiscountTypeBilling').val().toUpperCase() == '674AD791-188B-4238-B95C-97B5C2B9D544') {
                        strAccessType = "Allow to grant Senior Citizens' (SC) Discount";
                        $('#divAddCustomerDetailsBilling').show();
                        $('#divSCPWDDiscountBilling').hide();
                        isSeniorCitizenPWDDiscount = true;
                        isSeniorCitizenDiscount = true;
                        $('#txtDiscountPercentBilling').attr("disabled", "disabled");
                        $('#txtDiscountAmountBilling').attr("disabled", "disabled");
                        var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.getItem('CartOrderID'));

                        intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, false);

                        if (window.pageDiscounts.checkDiscountDetails(showDiscountDetails)) {

                        }
                    } else if ($('#selDiscountTypeBilling').val().toUpperCase() == 'C2218C86-0450-4CD3-B019-3EDD43E51741') {
                        strAccessType = "Allow to grant GPC Discount";
                    } else {
                    }


                    if (strAccessType == "") {
                        $('#selDiscountTypeBilling').val('').selectmenu('refresh');
                        $("#txtDiscountPercentBilling").val("0.00");
                        $("#txtDiscountAmountBilling").val("0.00");

                        getOrgDiscountPercentRefreshBilling($('#selDiscountTypeBilling').val());

                    } else {

                        //                        window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess(strAccessType, {
                            OnClickOKSuccess: function () {

                                //                                $.mobile.changePage(window.pageSecuritySourcePage);

                                getOrgDiscountPercentRefreshBilling($('#selDiscountTypeBilling').val());
                                //                    if (isSeniorCitizenPWDDiscount) {
                                //                        var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.getItem('CartOrderID'));

                                //                        intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, true);
                                //                    }


                            },
                            OnFailed: function () {
                                $('#selDiscountTypeBilling').val('').selectmenu('refresh');
                                $("#txtDiscountPercentBilling").val("0.00");
                                $("#txtDiscountAmountBilling").val("0.00");

                                getOrgDiscountPercentRefreshBilling($('#selDiscountTypeBilling').val());

                            }
                        });

                    }



                });


                $("#txtDiscountPercentBilling").keydown(function (e) {
                    if (e.keyCode != 110 && e.keyCode != 190) {
                        if (!NumberOnly(e)) return;
                    }
                }).bind('change', function () {

                    var percent = CDec2($.trim($(this).val()));
                    var pSalesPayDiscountCeiling = parseFloat(CDec2($.trim($("#txtDiscountPercentCeilingBilling").val().replace(/,/g, ""))));

                    if (CDec2(pSalesPayDiscountCeiling) < percent && percent > 0) {

                        percent = CDec2($.trim($("#txtDiscountPercentCeilingBilling").val().replace(/,/g, "")));

                        NewMessageBox("Sorry. You are allowed to give maximum of " + AccountingFormat(percent) + "% discount only.",
                $("#selDiscountTypeBilling option:selected").text(),
                [
                    {
                        Text: 'Ok'
                    }
                ]
                    );

                        $(this).val(AccountingFormat((percent > 0 ? AccountingFormat(percent) : "0.00")));
                    } else {
                        $(this).val(AccountingFormat(percent));

                    }


                    var discountAmt = (CDec2($('#lblSubTotalBilling').text().replace(/,/g, "")) * (CDec2($.trim($(this).val().replace(/,/g, ""))) / 100) / 0.05) * 0.05;

                    $("#txtDiscountAmountBilling").val(AccountingFormat(discountAmt));
                    longDiscountPercent = (CDec2($.trim($("#txtDiscountAmountBilling").val())) / CDec2($('#lblSubTotalBilling').text().replace(/,/g, ""))) * 100;


                    RefreshTotalDiscountBilling(window.pageDiscounts.RefreshSalesPaymentTotalBilling());

                });

                $("#txtDiscountAmountBilling").keydown(function (e) {
                    if (e.keyCode != 110 && e.keyCode != 190) {
                        if (!NumberOnly(e)) return;
                    }
                }).bind('change', function () {
                    var percent = (CDec2($.trim($(this).val())) / CDec2($('#lblSubTotalBilling').text().replace(/,/g, ""))) * 100;
                    var pSalesPayDiscountCeiling = parseFloat(CDec2($.trim($("#txtDiscountPercentCeilingBilling").val().replace(/,/g, ""))));

                    if (CDec2(pSalesPayDiscountCeiling) < percent && percent > 0) {

                        percent = parseFloat(CDec2($.trim($("#txtDiscountPercentCeilingBilling").val().replace(/,/g, ""))));

                        NewMessageBox("Sorry. You are allowed to give maximum of " + AccountingFormat(percent) + "% discount only.",
                $("#selDiscountTypeBilling option:selected").text(),
                [
                    {
                        Text: 'Ok'
                    }
                ]
                    );

                        $("#txtDiscountPercentBilling").val(AccountingFormat((percent > 0 ? AccountingFormat(percent) : "0.00")));

                    } else {
                        $("#txtDiscountPercentBilling").val(AccountingFormat(percent));
                    }


                    var discountAmt = (CDec2($('#lblSubTotalBilling').text().replace(/,/g, "")) * (CDec2(percent) / 100) / 0.05) * 0.05;

                    $(this).val(AccountingFormat(discountAmt));
                    longDiscountPercent = percent;


                    RefreshTotalDiscountBilling(window.pageDiscounts.RefreshSalesPaymentTotalBilling());

                });





                $("#txtSalesPayNoOfSCBilling").keydown(function (e) {
                    if (e.keyCode != 110 && e.keyCode != 190) {
                        if (!NumberOnly(e)) return;
                    }
                });

                $("#txtSalesPayNoOfSCBilling").bind('keyup', function () {
                    getOrgDiscountPercentRefreshBilling($('#selDiscountTypeBilling').val());

                    if (CDec2($.trim($("#txtSalesPayNoOfDinersBilling").val())) < CDec2($(this).val()))
                        $("#txtSalesPayNoOfDinersBilling").val($(this).val());

                    if (CDec2($.trim($("#txtSalesPayNoOfSCBilling").val())) > 0 && CDec2($.trim($("#txtSalesPayNoOfDinersBilling").val())) > 0) {


                        var proportionOfSeniors = parseFloat($.trim($("#txtSalesPayNoOfSCBilling").val())) / parseFloat($.trim($("#txtSalesPayNoOfDinersBilling").val()));
                        var seniorCitizenDiscountRate = parseFloat($('#txtDiscountPercentCeilingBilling').val()) * 0.01;
                        var weightedSCDiscountRate = proportionOfSeniors * seniorCitizenDiscountRate;
                        var totalBillBeforeSCDiscount = parseFloat($('#lblSalesPayAmountDueBilling').val());
                        var totalBillToBePayed = weightedSCDiscountRate * totalBillBeforeSCDiscount;


                        $("#txtDiscountPercentBilling").val(AccountingFormat(weightedSCDiscountRate * 100));
                        $("#txtDiscountAmountBilling").val(AccountingFormat(CDec2($('#lblTotalAmountBilling').text().replace(/,/g, "")) * weightedSCDiscountRate));

                        if (window.pageDiscounts.checkDiscountDetails(true)) {
                            //                                RefreshTotalDiscountBilling();
                            // window.pageDiscounts.RefreshSalesPaymentTotalBilling();

                        }


                    }



                    RefreshTotalDiscountBilling(window.pageDiscounts.RefreshSalesPaymentTotalBilling());
                });

                $("#txtSalesPayNoOfDinersBilling").keydown(function (e) {
                    if (e.keyCode != 110 && e.keyCode != 190) {
                        if (!NumberOnly(e)) return;
                    }
                });

                $("#txtSalesPayNoOfDinersBilling").bind('keyup', function () {
                    getOrgDiscountPercentRefreshBilling($('#selDiscountTypeBilling').val());

                    if (CDec2($.trim($("#txtSalesPayNoOfSCBilling").val())) > 0 && CDec2($.trim($("#txtSalesPayNoOfDinersBilling").val())) > 0) {

                        var proportionOfSeniors = parseFloat($.trim($("#txtSalesPayNoOfSCBilling").val())) / parseFloat($.trim($("#txtSalesPayNoOfDinersBilling").val()));
                        var seniorCitizenDiscountRate = parseFloat($('#txtDiscountPercentCeilingBilling').val()) * 0.01;
                        var weightedSCDiscountRate = proportionOfSeniors * seniorCitizenDiscountRate;
                        var totalBillBeforeSCDiscount = parseFloat($('#lblSalesPayAmountDueBilling').val());
                        var totalBillToBePayed = weightedSCDiscountRate * totalBillBeforeSCDiscount;

                        $("#txtDiscountPercentBilling").val(weightedSCDiscountRate * 100);
                        $("#txtDiscountAmountBilling").val(AccountingFormat(CDec2($('#lblTotalAmountBilling').text().replace(/,/g, "")) * weightedSCDiscountRate));

                        if (window.pageDiscounts.checkDiscountDetails(true)) {
                            //                                RefreshTotalDiscountBilling();
                            // window.pageDiscounts.RefreshSalesPaymentTotalBilling();
                        }


                    }
                    //RefreshTotalDiscountBilling();

                    RefreshTotalDiscountBilling(window.pageDiscounts.RefreshSalesPaymentTotalBilling());
                });

                var pDataCharges = [];

                $('#txtAddChargePercentBilling').change(function (e) {
                    var chargeAmount = CDec2($.trim($(this).val())) / 100;
                    longChargeAmountBilling = chargeAmount * CDec2(lblVATableSales);
                    $('#txtAddChargeBilling').val(AccountingFormat(longChargeAmountBilling));

                });

                $('#txtAddChargeBilling').change(function (e) {
                    var chargePercent = CDec2($.trim($(this).val())) / CDec2(lblVATableSales);
                    longChargeAmountBilling = $(this).val();
                    $('#txtAddChargePercentBilling').val(chargePercent * 100);
                });


                //                $('#txtAddChargeBilling').keydown(function (e) {
                //                    if (e.keyCode != 110 && e.keyCode != 190) {
                //                        if (!NumberOnly(e)) return;
                //                    }


                //                });

                $('#btnAddChargeBilling').click(function (e) {
                    e.preventDefault();
                    if ($.trim($("#txtAddChargeBilling").val()) != '') {
                        var hasMatch = false;

                        if (pDataCharges[0] != undefined) {
                            for (var i = 0; i < pDataCharges.length; i++) {
                                if ($('#selChargeTypeBilling').val() == pDataCharges[i].ChargeType) {
                                    //charges

                                    pDataCharges[i] = {
                                        ChargeType: $.trim($("#selChargeTypeBilling").val()),
                                        ChargeAmount: $.trim($("#selChargeTypeBilling").val()) != 'Tip' && $.trim($("#selChargeTypeBilling").val()) != 'Local Tax' ? longChargeAmountBilling : 0,
                                        TipAmount: $.trim($("#selChargeTypeBilling").val()) == 'Tip' ? longChargeAmountBilling : 0,
                                        TaxAmount: $.trim($("#selChargeTypeBilling").val()) == 'Local Tax' ? longChargeAmountBilling : 0,
                                        Remarks: $.trim($("#txtChargeRemarksBilling").val())
                                    };
                                    hasMatch = true;
                                    break;
                                }
                            }


                            if (!hasMatch) {
                                pDataCharges[1] = {
                                    ChargeType: $.trim($("#selChargeTypeBilling").val()),
                                    ChargeAmount: $.trim($("#selChargeTypeBilling").val()) != 'Tip' && $.trim($("#selChargeTypeBilling").val()) != 'Local Tax' ? longChargeAmountBilling : 0,
                                    TipAmount: $.trim($("#selChargeTypeBilling").val()) == 'Tip' ? longChargeAmountBilling : 0,
                                    TaxAmount: $.trim($("#selChargeTypeBilling").val()) == 'Local Tax' ? longChargeAmountBilling : 0,
                                    Remarks: $.trim($("#txtChargeRemarksBilling").val())
                                };
                            }
                        } else {
                            pDataCharges[0] = {
                                ChargeType: $.trim($("#selChargeTypeBilling").val()),
                                ChargeAmount: $.trim($("#selChargeTypeBilling").val()) != 'Tip' && $.trim($("#selChargeTypeBilling").val()) != 'Local Tax' ? longChargeAmountBilling : 0,
                                TipAmount: $.trim($("#selChargeTypeBilling").val()) == 'Tip' ? longChargeAmountBilling : 0,
                                TaxAmount: $.trim($("#selChargeTypeBilling").val()) == 'Local Tax' ? longChargeAmountBilling : 0,
                                Remarks: $.trim($("#txtChargeRemarksBilling").val())
                            };

                        }

                        var pData = {
                            InvoiceID: sessionStorage.getItem('CartOrderID'),
                            SalesPaymentChargeList: pDataCharges
                        };


                        if (SalesPayment_offline().SaveBillingCharges(pData) == 'ok') {
                            ShowCharges();
                            RefreshTotalDiscountBilling(window.pageDiscounts.RefreshSalesPaymentTotalBilling());
                        }
                    } else {
                        Android.Toast('Please enter an amount');
                    }
                });

                $('#selChargeTypeBilling').change(function (e) {
                    e.preventDefault();
                    longChargeAmountBilling = 0;
                    $("#txtAddChargeBilling").val('');
                    $("#txtAddChargePercentBilling").val('');
                    $("#txtChargeRemarksBilling").val('');
                });
                //////////////

                function ViewOrPrintBill() {
                    var pInvoiceID = sessionStorage.getItem('CartOrderID');

                    var pSql = '';
                    var pDt;

                    pSql = 'Select SalesOrderNo from private.TransInvoice where ID = ' + ReadyString(pInvoiceID);
                    pDt = Sql.Execute(pSql);

                    if (parseInt(pDt.Rows[0].SalesOrderNo) == 0 || pDt.Rows[0].SalesOrderNo == undefined) {

                        pSql = 'SELECT IFNULL(devfowst.POSLastSONo,0)+1 NewInvoiceNo FROM private.LibraryDeviceFowst devfowst WHERE devfowst.FowstID = ' + ReadyString(sessionStorage.FowstID) + ' AND devfowst.DeviceID = ' + ReadyString(Web.DeviceID())

                        pDt = Sql.Execute(pSql);

                        if (pDt.Rows.length > 0) {

                            pSql = 'Update private.TransInvoice set SalesOrderNo = ' + pDt.Rows[0].NewInvoiceNo + ' Where ID = ' + ReadyString(pInvoiceID);

                            Sql.ExecuteSql(pSql);

                            var pIU;

                            pIU = new InsertUpdate('private.LibraryDeviceFowst');

                            pIU.Add('POSLastSONo', "(SELECT IFNULL(devfowst.POSLastSONo,0) + 1 FROM private.LibraryDeviceFowst devfowst WHERE devfowst.FowstID = " + ReadyString(sessionStorage.FowstID) + " AND devfowst.DeviceID = " + ReadyString(Web.DeviceID()) + ")");

                            pSql = pIU.GetUpdate("FowstID = " + ReadyString(sessionStorage.FowstID) + " AND DeviceID = " + ReadyString(Web.DeviceID()));

                            Sql.ExecuteSql(pSql);
                        }
                    }
                    PrintBilling(pInvoiceID);
                }


                function getOrgDiscountTypeBilling() {

                    if (OfflineMode) {
                        LoadOrgDiscountTypeBilling(SalesPayment_offline().getOrgDiscountType().Rows);
                    } else {
                        AjaxSend({
                            url: 'DiscountSettings/getDiscountSettings',
                            success: function (vData) {

                                LoadOrgDiscountTypeBilling(vData.DiscountData.Rows);
                                isDiscountLoaded = true;
                            },
                            error: function (vError) {
                                var pDummy;
                            }
                        });
                    };

                }

                function LoadOrgDiscountTypeBilling(vRows) {

                    $('#selDiscountTypeBilling').empty();

                    $('#selDiscountTypeBilling').append(new Option('Please Select', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selDiscountTypeBilling').append(new Option(vRows[p].DiscountType, vRows[p].ID, false, false));
                    }
                }


                function getOrgDiscountPercentRefreshBilling(strDiscountID) {
                    var pData = {}
                    pData.DiscountID = strDiscountID;

                    if (OfflineMode) {
                        var vRows = SalesPayment_offline().getOrgDiscountPercentRefresh(strDiscountID).Rows[0];

                        if (vRows != undefined) {
                            LoadOrgDiscountPercentRefreshBilling(vRows);
                        } else {

                            $('#txtDiscountPercentCeilingBilling').val('');
                            $('#txtDiscountPercentBilling').val('');
                            $('#txtDiscountAmountBilling').val('');

                            $("#lblSalesPayTotalDiscountBilling").text('');
                            lblVATableSales = 0;
                            $('#lblVATableSales').text('');
                            $('#lblVATAmount').text('');
                            $('#lblNonVATSales').text('');
                            $('#lblVATExemptSales').text('');
                            $('#lblZeroRatedVATSales').text('');

                            RefreshTotalDiscountBilling(window.pageDiscounts.RefreshSalesPaymentTotalBilling());
                        }

                    } else {
                        AjaxSend({
                            url: 'DiscountSettings/getDiscountSettingsInfo',
                            data: pData,
                            success: function (vData) {

                                if (vData.DiscountData.Rows.length > 0) {
                                    LoadOrgDiscountPercentRefreshBillingBilling(vData.DiscountData.Rows[0]);
                                } else {

                                    $('#txtDiscountPercentCeilingBilling').val('');
                                    $('#txtDiscountPercentBilling').val('');
                                    $('#txtDiscountAmountBilling').val('');
                                    $("#lblSalesPayTotalDiscountBilling").text('');
                                    lblVATableSales = 0;
                                    $('#lblVATableSales').text('');
                                    $('#lblVATAmount').text('');
                                    $('#lblNonVATSales').text('');
                                    $('#lblVATExemptSales').text('');
                                    $('#lblZeroRatedVATSales').text('');

                                    RefreshTotalDiscountBilling(window.pageDiscounts.RefreshSalesPaymentTotalBilling());
                                }

                            },
                            error: function (vError) {
                                var pDummy;
                            }
                        });
                    };

                }


                function RefreshTotalDiscountBilling(vData) {
                    var dblDiscount = 0;

                    var dblTotalSalesPay = 0;

                    //dblDiscount = parseFloat(CDec2($('#lblSalesPaySubTotal').text().replace(/,/g, ""))) * (parseFloat(CDec2($('#txtDiscountPercent').val().replace(/,/g, "")))/100);
                    //dblDiscount = Math.round(parseFloat(CDec2($('#lblSalesPaySubTotal').text().replace(/,/g, ""))) * (parseFloat(CDec2($('#txtDiscountPercent').val().replace(/,/g, ""))) / 100) / 0.05) * 0.05;

                    dblDiscount = vData.Rows[0].DiscountAmount;

                    if (isSeniorCitizenDiscount) {

                        $('#rowVATPercentBilling').show();
                        $('#rowAmountLessVATBilling').show();

                        $('#lblVATPercentValueBilling').text(vData.Rows[0].TaxPercent);

                        //                        vatSale = vData.Rows[0].TotalAmountDue / (1 + (vData.Rows[0].TaxPercent / 100));
                        $('#lblVATPercentBilling').text(AccountingFormat(-Math.abs(CDec2(vData.Rows[0].SubTotal) - CDec2(vData.Rows[0].TotalAmountDue))));
                        $('#lblTotalAmountBilling').text(AccountingFormat(vData.Rows[0].TotalAmountDue));

                        $("#txtDiscountAmountBilling").val(AccountingFormat(CDec2($('#lblTotalAmountBilling').text().replace(/,/g, "")) * (CDec2(longDiscountPercent)) / 100));
                    } else {

                        $('#rowVATPercentBilling').hide();
                        $('#rowAmountLessVATBilling').hide();

                        $('#lblVATAmount').text('');
                        $('#lblTotalAmountBilling').text('');
                        $("#txtDiscountAmountBilling").val(AccountingFormat(CDec2($('#lblSubTotalBilling').text().replace(/,/g, "")) * (CDec2(longDiscountPercent)) / 100));

                    }
                    dblTotalAmountDue = CDec2(vData.Rows[0].TotalAmountDue) - CDec2(dblDiscount) + CDec2($("#lblSalesPayChargesBilling").text().replace(/,/g, "")) + CDec2($("#lblTipBilling").text().replace(/,/g, "")) + CDec2($("#lblSalesPayOtherTaxBilling").text().replace(/,/g, ""));

                    $("#lblSalesPayAmountDueBilling").text(AccountingFormat(dblTotalAmountDue));
                    $("#lblSalesPayTotalDiscountBilling").text(AccountingFormat(-Math.abs(dblDiscount)));
                    lblVATableSales = vData.Rows[0].TaxSales || 0;
                    $('#lblVATableSales').text(AccountingFormat(vData.Rows[0].TaxSales || '0'));
                    $('#lblVATAmount').text(AccountingFormat(vData.Rows[0].TaxAmount || '0'));
                    $('#lblNonVATSales').text(AccountingFormat(vData.Rows[0].NonVATSale || '0'));
                    $('#lblVATExemptSales').text(AccountingFormat(vData.Rows[0].VATExemptSale || '0'));
                    $('#lblZeroRatedVATSales').text(AccountingFormat(vData.Rows[0].ZeroRatedSale || '0'));


                }




                function LoadOrgDiscountPercentRefreshBilling(vRows) {

                    $('#txtDiscountPercentCeilingBilling').val(AccountingFormat(vRows.PercentDiscount));
                    $('#txtDiscountPercentBilling').val(AccountingFormat(vRows.PercentDiscount));
                    longDiscountPercent = vRows.PercentDiscount;

                    if (isSeniorCitizenPWDDiscount) {

                        //                        if (window.pageDiscounts.checkDiscountDetails(true)) {


                        RefreshTotalDiscountBilling(window.pageDiscounts.RefreshSalesPaymentTotalBilling());
                        //                        }

                        var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.getItem('CartOrderID'));

                        intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, true);



                    } else {
                        RefreshTotalDiscountBilling(window.pageDiscounts.RefreshSalesPaymentTotalBilling());
                    }


                }

                function DisplayChargesListBilling(vRows) {

                    var pLV = $("#tblChargesListBilling");

                    pLV.html('');

                    for (var i = 0; i <= vRows.length - 1; i++) {

                        pItem = $('<tr style="cursor: pointer;">')
            .append($('<td>')
                .text(vRows[i].ChargeType)
            )
                .append($('<td style="text-align: right;">')
                .text(AccountingFormat(CDec2(vRows[i].ChargeAmount) + CDec2(vRows[i].LocalTaxAmount) + CDec2(vRows[i].TipAmount)))
            )
                .append($('<td>')
                .text(vRows[i].Remarks)
            )
            .append($('<td>')
                .append($('<a href="">')
                        .data('ChargeType', vRows[i].ChargeType)
                        .text('Remove')
                        .click(function () {
                            var pData = {
                                InvoiceID: sessionStorage.getItem('CartOrderID'),
                                ChargeType: $(this).data('ChargeType')
                            };

                            if (SalesPayment_offline().DeleteBillCharges(pData) == 'ok') {
                                ShowCharges();
                                RefreshTotalDiscountBilling(window.pageDiscounts.RefreshSalesPaymentTotalBilling());
                            } else {
                                NewMessageBox('There was an error while processing your request.');
                            }

                        })
                    )
            )
                        pLV.append(pItem)

                    }

                    //            $('#tblFranchiseeOrdersBody').table("refresh");
                }

                function PrintBilling() {
                    proj.SalesMenu.PrintBilling.apply(this, arguments);
                }

                function PrintBillingNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, vOptions) {
                    var strTemp = '';
                    var strPrint = '';
                    var strPrinterIP = '';
                    var strStockID = '';
                    var IsShowSalesRep = false;
                    var pData = {};
                    var intFAFCTotalLenght = 40;
                    var intFBTotalLenght = 27;
                    var intFCDoubleTotalLenght = 18;
                    var pTableNum;
                    var pTableName;

                    pData.InvoiceID = pInvoiceID;
                    vOptions = $.extend({
                        PrintAsOrderSlip: false
                    }, vOptions);

                    //Get Printer IP Address
                    if ((vDataPrint == undefined)
                        || !$.isArray(vDataPrint.Rows)
                        || (vDataPrint.Rows.length == 0)) {
                        NewMessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                            "Printer",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                        );
                        return;
                    }

                    //additional by janine jan 31 2015
                    originalDevFowstPrinterID = vDataPrint.Rows[0].ID;
                    if (vDataPrint.Rows.length > 0) {
                        if (CStr2(newPrinterIP) != '')
                            strPrinterIP = newPrinterIP;
                        else
                            strPrinterIP = vDataPrint.Rows[0].PrinterIPAddress;

                        setPrinterConnectionType(vDataPrint.Rows[0].ConnectionType, vDataPrint.Rows[0].PrinterType, true);
                        //New set of text to print
                        NewPrintBuilder();

                        //Get Header
                        IsShowSalesRep = false;
                        if (vDataHeader.Rows.length > 0) {

                            IsShowSalesRep = (vDataHeader.Rows[0].IsShowSalesRep == 1 ? true : false);

                            if (!vOptions.DontPrintHeader) {
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].FowstName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                //Get Items
                                if (vDataInvHeader.Rows.length > 0) {
                                    strPrint = '\n\n';

                                    if (CBool(vOptions.IsReprintAll)) {
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix($.trim('Re-Printed Order Slip' || ''), 8, ' ', 1), intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint += strTemp;
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);
                                    }

                                    strPrint = '';

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Customer', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerName || '')), intFAFCTotalLenght, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Address', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerAddress || '')), intFAFCTotalLenght, '', 0, 0);
                                    strPrint = strPrint + strTemp;


                                    if (!CBool(vOptions.PrintAsOrderSlip) && !CBool(vOptions.PrintAsCancellationSlip)) {
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerTIN || '')), intFAFCTotalLenght, '', 0, 0);
                                        strPrint = strPrint + strTemp;
                                    }
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    pTableNum = vDataInvHeader.Rows[0].CustomerTableNo;
                                    if (CStr2(pTableNum) != '') {
                                        if (IsMobile)
                                            pTableName = mTableView.GetTableInfoByInternalName(pTableNum);
                                        if (CStr2(pTableName).trim() == '') {
                                            var prefix = pTableNum.split('_');
                                            pTableNum = prefix[prefix.length - 1];
                                        } else
                                            pTableNum = pTableName;

                                        if ($.trim((pTableNum || '')) != '') {
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Table #', 8, ' ', 1), 7, ': ' + $.trim((pTableNum || '')), intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strTemp;
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 1, 0, 0, 0, 0);
                                        }
                                    }
                                    strPrint = '';

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix($.trim(vDataInvHeader.Rows[0].DeliveryMethod || ''), 8, ' ', 1), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint += strTemp;
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim(vDataInvHeader.Rows[0].DeliveryMethod || ''), intFAFCTotalLenght, '', 0, '', 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 1, 0, 0, 0, 0);

                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                    strPrint = '';
                                    if (CBool(vOptions.PrintAsOrderSlip)) {
                                        strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'Order Slip No.', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strTemp += FormatTextToPrint(intFCDoubleTotalLenght, SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].OrderSlipNum || '')), 12, '0', 0), intFAFCTotalLenght, '', 0, '', 0, 1);
                                    } else if (CBool(vOptions.PrintAsCancellationSlip)) {
                                        strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'Cancellation Slip of Order Slip No.', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strTemp += FormatTextToPrint(intFCDoubleTotalLenght, SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].OrderSlipNum || '')), 12, '0', 0), intFAFCTotalLenght, '', 0, '', 0, 1);
                                    } else {
                                        strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'Bill ' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].SalesOrderNo || '')), 12, '0', 0), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                    }
                                    strPrint += strTemp;

                                    strTemp = FormatTextToPrint(intFCDoubleTotalLenght, $.format.date(new Date(), 'MMM dd, yyyy H:mm'), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, (CBool(vOptions.PrintAsOrderSlip) || CBool(vOptions.PrintAsCancellationSlip) ? 'Waiter: ' : 'Cashier: ') + ($.trim(vDataInvHeader.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);

                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                } // END if (!vOptions.DontPrintHeader) {

                            } // END if (!vOptions.DontPrintHeader) {
                            if (vOptions.Category != null) {
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Cat: ' + $.trim(vOptions.Category || ''), intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 1, 0, 0, 0, 0);
                            }
                            //Get Items
                            var strProductID = "";
                            if (vDataItem.Rows.length > 0) {
                                strStockID = '';
                                strPrint = '';
                                var intLimitPerBatchPrint = 0;
                                for (var p = 0; p <= vDataItem.Rows.length - 1; p++) {
                                    if ((CBool(vOptions.PrintAsOrderSlip) || CBool(vOptions.PrintAsCancellationSlip)) && CBool(vDataItem.Rows[p].IsOrderSlipPrinted))
                                        continue;

                                    var pQtyPrinted = CDec2(vDataItem.Rows[p].QuantityPrinted);
                                    var pQty = CDec2(vDataItem.Rows[p].Quantity);
                                    var pQtyToPrint;
                                    if (CBool(vOptions.PrintAsOrderSlip) && !CBool(vOptions.IsReprintAll))
                                        pQtyToPrint = pQty - pQtyPrinted;
                                    else if (CBool(vOptions.PrintAsCancellationSlip))
                                        pQtyToPrint = pQtyPrinted - pQty;
                                    else
                                        pQtyToPrint = pQty;
                                    var strStockDescription = $.trim((vDataItem.Rows[p].StockDescription || ''));
                                    var strQty = SetPrefixPostfix(AccountingFormat(pQtyToPrint, 0), 8, ' ', 0);
                                    var strUnitMeasure = SetPrefixPostfix((vDataItem.Rows[p].UnitMeasure || ''), 10, ' ', 0);
                                    var strUnitPrice = '';
                                    var strSubTotal = '';

                                    if (!CBool(vOptions.PrintAsOrderSlip) && !CBool(vOptions.PrintAsCancellationSlip)) {
                                        strUnitPrice = SetPrefixPostfix(AccountingFormat(vDataItem.Rows[p].UnitPrice || '0'), intFAFCTotalLenght - 30, ' ', 0);
                                        strSubTotal = SetPrefixPostfix(AccountingFormat(vDataItem.Rows[p].SubTotal || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                    }

                                    strPrint = '';
                                    if (strProductID == "") {
                                        strProductID = vDataItem.Rows[p].StockID;
                                    }

                                    if (vDataItemSerial.Rows.length > 0) {
                                        if (strProductID != vDataItem.Rows[p].StockID) { //Print Serials

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Serial(s) : ', intFAFCTotalLenght, '', 0, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            for (var intSerial = 0; intSerial <= vDataItemSerial.Rows.length - 1; intSerial++) {

                                                if (strProductID == vDataItemSerial.Rows[intSerial].ProductID && vDataItemSerial.Rows[intSerial].InvoiceID == pInvoiceID) {

                                                    var strSerial = $.trim((vDataItemSerial.Rows[intSerial].SerialNumber || ''));

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, strSerial, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                }
                                            }

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            strProductID = vDataItem.Rows[p].StockID;
                                        }
                                    }

                                    if (CBool(vOptions.PrintAsOrderSlip) || CBool(vOptions.PrintAsCancellationSlip)) {
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, strStockDescription + ' x ' + CDec2(strQty) + ' ' + $.trim(strUnitMeasure), intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;
                                    } else {
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, strStockDescription, intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        var itemtotal = SetPrefixPostfix(AccountingFormat(CDec2(strQty) * CDec2(strUnitPrice)) || '0', intFAFCTotalLenght - 22, ' ', 0);
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, ' ' + $.trim(strQty) + ' @ ' + $.trim(strUnitPrice) + ' / ' + $.trim(strUnitMeasure), 20, (CBool(vOptions.PrintAsOrderSlip) || CBool(vOptions.PrintAsCancellationSlip) ? '' : itemtotal), intFAFCTotalLenght, '', 0, 0);
                                        strPrint = strPrint + strTemp;
                                    }

                                    if (vDataItem.Rows[p].ItemModifier != undefined) {
                                        if (vDataItem.Rows[p].ItemModifier.length > 0) {
                                            var imTotal = 0;
                                            var showSubTotal = false; // use in printing order slip
                                            for (var im = 0; im < vDataItem.Rows[p].ItemModifier.length; im++) {
                                                var mod = vDataItem.Rows[p].ItemModifier[im];
                                                var unitprice, subtotal;
                                                if (!CBool(vOptions.PrintAsOrderSlip) && !CBool(vOptions.PrintAsCancellationSlip)) {
                                                    unitprice = AccountingFormat(mod.ModifierPrice);
                                                    subtotal = AccountingFormat(mod.SubTotal);
                                                    showSubTotal = true;
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, '+ ' + mod.Description, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                }

                                                if (CStr2(mod.OrderedQty) != '') { // for Inventory Affecting Modifiers
                                                    if (CBool(vOptions.PrintAsOrderSlip) || CBool(vOptions.PrintAsCancellationSlip)) {
                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '+ ' + CDec2(mod.OrderedQty) + ' ' + mod.Description, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                        strPrint = strPrint + strTemp;
                                                    } else {
                                                        var itemmodifiertotal = SetPrefixPostfix(AccountingFormat(CDec2(mod.OrderedQty) * CDec2(mod.ModifierPrice)) || '0', intFAFCTotalLenght - 22, ' ', 0);
                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, ' ' + mod.OrderedQty + ' @ ' + (CInt2(mod.ModifierPrice) == 0 ? '0.00' : AccountingFormat($.trim(mod.ModifierPrice))) + ' each', 20, (!CBool(showSubTotal) ? '' : itemmodifiertotal), intFAFCTotalLenght, '', 0, 0);
                                                        strPrint = strPrint + strTemp;
                                                        imTotal += CDec2(mod.SubTotal);
                                                    }
                                                }

                                                if ((CBool(vOptions.PrintAsOrderSlip) || CBool(vOptions.PrintAsCancellationSlip)) && CStr2(mod.OrderedQty) == '') { // for Non-Inventory Affecting Modifiers
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, '+ ' + mod.Description, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                }
                                            }
                                            if (showSubTotal) {
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '   Item Total:  ---->', 20, SetPrefixPostfix(AccountingFormat(strSubTotal), intFAFCTotalLenght - 22, ' ', 0), intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;
                                            }
                                        }
                                    }

                                    //if (IsShowSalesRep) {

                                    //    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sales Person: ' + ($.trim(vDataItem.Rows[p].SalesPerson || '')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //    strPrint = strPrint + strTemp;

                                    //}

                                    if (vDataItemSerial.Rows.length > 0) {
                                        if (p == vDataItem.Rows.length - 1) {

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Serial(s) : ', intFAFCTotalLenght, '', 0, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            for (var intSerial = 0; intSerial <= vDataItemSerial.Rows.length - 1; intSerial++) {

                                                if (strProductID == vDataItemSerial.Rows[intSerial].ProductID && vDataItemSerial.Rows[intSerial].InvoiceID == pInvoiceID) {

                                                    var strSerial = $.trim((vDataItemSerial.Rows[intSerial].SerialNumber || ''));

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, strSerial, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                }
                                            }

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            strProductID = vDataItem.Rows[p].StockID;
                                        }
                                    } else {
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;
                                    }

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    intLimitPerBatchPrint = intLimitPerBatchPrint + 1;

                                    if (intLimitPerBatchPrint >= 100 && vDataItem.Rows.length > 100) {

                                        PrintText(strPrinterIP);
                                        //ClearCommandBuffer();

                                        //New set of text to print because it causes error when you print directly all items at once.
                                        NewPrintBuilder();
                                        intLimitPerBatchPrint = 0;
                                    }

                                } // END for (var p = 0; p <= vDataItem.Rows.length - 1; p++) {

                                //Get Footer Amounts
                                if (vDataFooter.Rows.length > 0) {
                                    var strSKUCount = SetPrefixPostfix(AccountingFormat((vDataFooter.Rows[0].SKUCount || '0'), 0), intFAFCTotalLenght - 14, ' ', 0);
                                    var strTotalAmountDue = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].SubTotal || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                    var strTotalDiscount = SetPrefixPostfix(AccountingFormat(-Math.abs(vDataFooter.Rows[0].DiscountAmount || '0')), intFAFCTotalLenght - 22, ' ', 0);
                                    var strNetAmountDue = SetPrefixPostfix(AccountingFormat(CDec2(vDataFooter.Rows[0].NetAmountDue) + CDec2(vDataFooter.Rows[0].ChargeAmount) + CDec2(vDataFooter.Rows[0].TipAmount) + CDec2(vDataFooter.Rows[0].LocalTaxAmount)), intFAFCTotalLenght - 22, ' ', 0);

                                    var strTotalCharges = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].ChargeAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                    var strTip = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TipAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                    var strVatSale = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TotalAmountDue || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                    var strVatAmount = SetPrefixPostfix(AccountingFormat(-Math.abs(CDec2(vDataFooter.Rows[0].SubTotal) - CDec2(vDataFooter.Rows[0].TotalAmountDue))), intFAFCTotalLenght - 22, ' ', 0);
                                    var strOtherTax = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].LocalTaxAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SKU Count: ', 12, strSKUCount, intFAFCTotalLenght, '', 0, 0);
                                    ReadyTextToPrint(strTemp, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    strPrint = '';
                                    if (!vOptions.DontPrintFooter) {

                                        if (!CBool(vOptions.PrintAsOrderSlip) && !CBool(vOptions.PrintAsCancellationSlip)) {

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sub Total: ', 20, strTotalAmountDue, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            if (CBool(vOptions.isSeniorCitizenDiscount)) {
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                strPrint = strPrint + strTemp;

                                                if (vOptions.SCDiscountExempt > 0) {
                                                    var strSCDiscountExempt = SetPrefixPostfix(AccountingFormat(SCDiscountExempt || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SC Discount-Exempt: ', 20, strSCDiscountExempt, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                }

                                                var strSubTotalOfVatExemptItems = SetPrefixPostfix(AccountingFormat(vOptions.SubTotalOfVatExemptItems || '0'), intFAFCTotalLenght - 31, ' ', 0);
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '\nGross Amount Subject to SCD: ', 29, strSubTotalOfVatExemptItems, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                if (vOptions.GrossSalesToNoneSC > 0) {
                                                    var strGrossSalesToNoneSC = SetPrefixPostfix(AccountingFormat(vOptions.GrossSalesToNoneSC || '0'), intFAFCTotalLenght - 31, ' ', 0);
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Gross Sales to None-SC ' + (CInt2($('#txtSalesPayNoOfDiners').val()) - CInt2($('#txtSalesPayNoOfSC').val())) + '/' + $('#txtSalesPayNoOfDiners').val() + ':', 29, strGrossSalesToNoneSC, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                }

                                                var strGrossSalesToSC = SetPrefixPostfix(AccountingFormat(vOptions.GrossSalesToSC || '0'), intFAFCTotalLenght - 23, ' ', 0);
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '\nGross Sales to SC ' + $('#txtSalesPayNoOfSC').val() + '/' + $('#txtSalesPayNoOfDiners').val() + ':', 21, strGrossSalesToSC, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                var strVatExemptSales = SetPrefixPostfix(AccountingFormat(vOptions.VatExemptSales || '0') + (vOptions.IsApplyMaxSCDiscountableAmount ? '' : '*'), intFAFCTotalLenght - 22, ' ', 0);
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Ex for SC: ', 20, strVatExemptSales, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                var strMaxSCDiscountableAmount = SetPrefixPostfix(AccountingFormat(vOptions.MaxSCDiscountableAmount || '0') + (vOptions.IsApplyMaxSCDiscountableAmount ? '*' : ''), intFAFCTotalLenght - 25, ' ', 0);
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Max Discountable VAT-EX(' + (CDec2(vOptions.MaxSCDiscountableAmount) / CInt2($('#txtSalesPayNoOfSC').val())) + '*' + CInt2($('#txtSalesPayNoOfSC').val()) + ')', 23, strMaxSCDiscountableAmount, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                var strDiscountPercentCeiling = SetPrefixPostfix(AccountingFormat($('#txtDiscountPercentCeiling').val()) + '%', intFAFCTotalLenght - 22, ' ', 0);
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SC Discount Rate: ', 20, strDiscountPercentCeiling, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                var strseniorCitizenDiscount = SetPrefixPostfix(AccountingFormat(vOptions.seniorCitizenDiscount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SC Discount Amount based on Vat-Ex Sale: ', 20, strseniorCitizenDiscount, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                var strseniorCitizenNetAmount = SetPrefixPostfix(AccountingFormat(vOptions.seniorCitizenNetAmount || '0'), intFAFCTotalLenght - 32, ' ', 0);
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Net Amount to be Paid by SC: ', 30, strseniorCitizenNetAmount, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                strPrint = strPrint + strTemp;

                                            } else {
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Discount:   ', 20, strTotalDiscount, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;
                                            }

                                            if (CDec2(vDataFooter.Rows[0].ChargeAmount) > 0) {
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Charges:   ', 20, strTotalCharges, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;
                                            }

                                            if (CDec2(vDataFooter.Rows[0].TipAmount) > 0) {
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tip:   ', 20, strTip, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                            }

                                            if (CDec2(vDataFooter.Rows[0].LocalTaxAmount) > 0) {
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Local Tax:   ', 20, strOtherTax, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;
                                            }

                                            var strNetAmountDue = SetPrefixPostfix(AccountingFormat((vOptions.isSeniorCitizenDiscount ? $('#lblSalesPayAmountDue').text() : strNetAmountDue) || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Amount Due:   ', 20, strNetAmountDue, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            if (CDec2(vDataInvHeader.Rows[0].DepositAmount) > 0) {
                                                var pDepositAmountString = SetPrefixPostfix(AccountingFormat(CDec2(vDataInvHeader.Rows[0].DepositAmount)), intFAFCTotalLenght - 22, ' ', 0);
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Adv. Deposit:   ', 20, pDepositAmountString, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;
                                            }
                                        } // END if (!CBool(vOptions.PrintAsOrderSlip)) {
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                        strPrint = '';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        //                                        var strVATSales = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TaxSales || '0'), intFAFCTotalLenght - 20, ' ', 0);
                                        //                                        var strVATAmount = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TaxAmount || '0'), intFAFCTotalLenght - 15, ' ', 0);
                                        //                                        var strNonVAT = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].NonVATSale || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                        //                                        var strVATExempt = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].VATExemptSale || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                        //                                        var strVATZeroRated = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].ZeroRatedSale || '0'), intFAFCTotalLenght - 25, ' ', 0);

                                        //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales: ', 18, strVATSales, intFAFCTotalLenght, '', 0, 0);
                                        //                                        strPrint = strPrint + strTemp;

                                        //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount: ', 13, strVATAmount, intFAFCTotalLenght, '', 0, 0);
                                        //                                        strPrint = strPrint + strTemp;

                                        //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT Sales: ', 20, strNonVAT, intFAFCTotalLenght, '', 0, 0);
                                        //                                        strPrint = strPrint + strTemp;

                                        //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Exempt Sales: ', 20, strVATExempt, intFAFCTotalLenght, '', 0, 0);
                                        //                                        strPrint = strPrint + strTemp;

                                        //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT Sales: ', 23, strVATZeroRated, intFAFCTotalLenght, '', 0, 0);
                                        //                                        strPrint = strPrint + strTemp;

                                        //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                        //                                        strPrint = strPrint + strTemp;

                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);



                                        //Get Footer Messages
                                        //strPrint = '';
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, '* Please ask for an official receipt *', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //strPrint = strPrint + strTemp;

                                        //if (!CBool(vOptions.PrintAsOrderSlip)) {
                                        //    strPrint = 'SO|' + vDataInvHeader.Rows[0].OrganizationID + '|' + vDataInvHeader.Rows[0].OrganizationBranchID + '|' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0)
                                        //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);
                                        //}

                                    } // end if (!vOptions.DontPrintFooter) {
                                } // END if (vDataFooter.Rows.length > 0) {

                                if (!vOptions.DontPrintFooter)
                                    ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                PrintText(strPrinterIP);
                                //ClearCommandBuffer();

                                //Print SRV
                                //PrintSalesPaymentSRVReceipt(0, pInvoiceID);
                                if (vOptions.PrintAsOrderSlip || vOptions.PrintAsCancellationSlip)
                                    Business.TransInvoice.TagOrderSlipItemsAsPrinted({
                                        Rows: vDataItem.Rows
                                    });

                                if (!vOptions.DontPrintFooter) {
                                    if (!(vOptions.DontConfirm))

                                        setTimeout(VerifyPrintReceipt(pInvoiceID), 2000);

                                    //Check Print Order Reference
                                    if (OfflineMode) {
                                        if (Business.SystemDefaults.GetSystemDefault("When Do You Want To Print Order Slip") == 'After Payment' &&
                                        Business.SystemDefaults.GetSystemDefault("Print Order Slip") == '1') {

                                            var pDataSO;
                                            pDataSO = {
                                                InvoiceID: pInvoiceID
                                            };

                                            if (SalesMenu_offline().SaveSalesOrder(pDataSO)) {
                                                PrintSalesOrderBOM(pInvoiceID);
                                            }

                                        } //if (Business.SystemDefaults.GetSystemDefault("When Do You Want To Print Order Reference") == 'Before Payment'){
                                    }
                                }
                            } // END if (vDataItem.Rows.length > 0) {
                            //                                    }

                        }
                    }
                    else {
                        NewMessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                            "Printer",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                        );
                    }
                } // END function PrintBillingNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, vOptions) {

                function PrintBillingWPSNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, vOptions, DeviceDataPrint) {
                    var strTemp = '';
                    var strPrint = '';
                    var strPrinterIP = '';
                    var strPrinterName = '';
                    var strStockID = '';
                    var IsShowSalesRep = false;
                    var pData = {};
                    var intFAFCTotalLenght = 40;
                    var intFBTotalLenght = 27;
                    var intFCDoubleTotalLenght = 18;
                    var pTableNum;
                    var pTableName;

                    pData.InvoiceID = pInvoiceID;
                    vOptions = $.extend({
                        PrintAsOrderSlip: false
                    }, vOptions);

                    //Get Printer IP Address
                    if ((vDataPrint == undefined)
                        || !$.isArray(vDataPrint.Rows)
                        || (vDataPrint.Rows.length == 0)) {
                        NewMessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                            "Printer",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                        );
                        return;
                    }

                    //additional by janine jan 31 2015
                    originalDevFowstPrinterID = vDataPrint.Rows[0].ID;
                    if (vDataPrint.Rows.length > 0) {
                        if (CStr2(newPrinterIP) != '')
                            strPrinterIP = newPrinterIP;
                        else {
                            strPrinterIP = DeviceDataPrint.Rows[0].PrinterIPAddress;
                            strPrinterName = vDataPrint.Rows[0].PrinterName;
                        }


                        setPrinterConnectionType(vDataPrint.Rows[0].ConnectionType, vDataPrint.Rows[0].PrinterType, true);
                        //New set of text to print
                        NewPrintBuilder();

                        //Get Header
                        IsShowSalesRep = false;
                        if (vDataHeader.Rows.length > 0) {

                            IsShowSalesRep = (vDataHeader.Rows[0].IsShowSalesRep == 1 ? true : false);

                            WindowsPrinter($.trim((vDataHeader.Rows[0].BusinessTradeName || '')), [0, 0, 0, 1, 0, 36]);
                            //strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                            WindowsPrinter($.trim((vDataHeader.Rows[0].FowstName || '')), [1, 0, 0, 1, 0, 36]);
                            //strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].FowstName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            //Get Items
                            if (vDataInvHeader.Rows.length > 0) {
                                strPrint = '\n\n';

                                WindowsPrinter('Customer : ' + $.trim((vDataInvHeader.Rows[0].CustomerName || '')), [0, 0, 0, 0, 0, 36]);

                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Customer', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerName || '')), intFAFCTotalLenght, '', 0, 0);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter('Address : ' + $.trim((vDataInvHeader.Rows[0].CustomerAddress || '')), [0, 0, 0, 0, 0, 36]);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Address', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerAddress || '')), intFAFCTotalLenght, '', 0, 0);
                                //strPrint = strPrint + strTemp;


                                if (!CBool(vOptions.PrintAsOrderSlip)) {
                                    WindowsPrinter('TIN : ' + $.trim((vDataInvHeader.Rows[0].CustomerTIN || '')), [0, 0, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerTIN || '')), intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                }
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                pTableNum = vDataInvHeader.Rows[0].CustomerTableNo;
                                if (CStr2(pTableNum) != '') {
                                    if (IsMobile)
                                        pTableName = mTableView.GetTableInfoByInternalName(pTableNum);
                                    if (CStr2(pTableName).trim() == '') {
                                        var prefix = pTableNum.split('_');
                                        pTableNum = prefix[prefix.length - 1];
                                    } else
                                        pTableNum = pTableName;

                                    if ($.trim((pTableNum || '')) != '') {
                                        WindowsPrinter('Table # : ' + $.trim((pTableNum || '')), [0, 0, 0, 0, 0, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Table #', 8, ' ', 1), 7, ': ' + $.trim((pTableNum || '')), intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strTemp;
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 1, 0, 0, 0, 0);
                                    }
                                }
                                strPrint = '';
                                if (vOptions.Category != null)
                                    WindowsPrinter('Cat : ' + $.trim(vOptions.Category || ''), [0, 0, 0, 0, 0, 36]);

                                WindowsPrinter($.trim(vDataInvHeader.Rows[0].DeliveryMethod || ''), [0, 0, 0, 0, 0, 36]);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Cat: ' + $.trim(vOptions.Category || ''), intFAFCTotalLenght, '', 0, '', 0, 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix($.trim(vDataInvHeader.Rows[0].DeliveryMethod || ''), 8, ' ', 1), intFAFCTotalLenght, '', 0, '', 0, 0);
                                //strPrint += strTemp;
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim(vDataInvHeader.Rows[0].DeliveryMethod || ''), intFAFCTotalLenght, '', 0, '', 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 1, 0, 0, 0, 0);

                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                //strPrint = '';
                                if (CBool(vOptions.PrintAsOrderSlip)) {
                                    WindowsPrinter('Order Slip No.', [0, 0, 0, 1, 0, 36]);
                                    WindowsPrinter(SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].OrderSlipNum || '')), 12, '0', 0), [0, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'Order Slip No.', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    //strTemp += FormatTextToPrint(intFCDoubleTotalLenght, SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].OrderSlipNum || '')), 12, '0', 0), intFAFCTotalLenght, '', 0, '', 0, 1);
                                } else {
                                    WindowsPrinter('Bill ' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].SalesOrderNo || '')), 12, '0', 0), [0, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'Bill ' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].SalesOrderNo || '')), 12, '0', 0), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                }
                                //strPrint += strTemp;

                                WindowsPrinter($.format.date(new Date(), 'MMM dd, yyyy H:mm'), [0, 0, 0, 1, 0, 36]);
                                //strTemp = FormatTextToPrint(intFCDoubleTotalLenght, $.format.date(new Date(), 'MMM dd, yyyy H:mm'), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                WindowsPrinter('newLine', [0, 0, 0, 0, 0, 36]);

                                WindowsPrinter((CBool(vOptions.PrintAsOrderSlip) ? 'Waiter: ' : 'Cashier: ') + $.trim(vDataInvHeader.Rows[0].CashierName || ''), [0, 0, 0, 0, 0, 36]);
                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Cashier: ' + ($.trim(vDataInvHeader.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                if (vOptions.Category != null) {
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Cat: ' + $.trim(vOptions.Category || ''), intFAFCTotalLenght, '', 0, '', 0, 0);

                                    WindowsPrinter('Cat: ' + $.trim(vOptions.Category || ''), [0, 0, 0, 0, 0, 36]);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 1, 0, 0, 0, 0);
                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                }

                                //Get Items
                                var strProductID = "";
                                if (vDataItem.Rows.length > 0) {
                                    strStockID = '';
                                    strPrint = '';
                                    var intLimitPerBatchPrint = 0;
                                    for (var p = 0; p <= vDataItem.Rows.length - 1; p++) {
                                        if (CBool(vOptions.PrintAsOrderSlip) && CBool(vDataItem.Rows[p].IsOrderSlipPrinted))
                                            continue;

                                        var pQtyPrinted = CDec2(vDataItem.Rows[p].QuantityPrinted);
                                        var pQty = CDec2(vDataItem.Rows[p].Quantity);
                                        var pQtyToPrint;
                                        if (CBool(vOptions.PrintAsOrderSlip))
                                            pQtyToPrint = pQty - pQtyPrinted;
                                        else
                                            pQtyToPrint = pQty;
                                        var strStockDescription = $.trim((vDataItem.Rows[p].StockDescription || ''));
                                        var strQty = SetPrefixPostfix(AccountingFormat(pQtyToPrint, 0), 8, ' ', 0);
                                        var strUnitMeasure = SetPrefixPostfix((vDataItem.Rows[p].UnitMeasure || ''), 10, ' ', 0);
                                        var strUnitPrice = '';
                                        var strSubTotal = '';

                                        if (!CBool(vOptions.PrintAsOrderSlip)) {
                                            strUnitPrice = SetPrefixPostfix(AccountingFormat(vDataItem.Rows[p].UnitPrice || '0'), intFAFCTotalLenght - 30, ' ', 0);
                                            strSubTotal = SetPrefixPostfix(AccountingFormat(vDataItem.Rows[p].SubTotal || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                        }

                                        strPrint = '';
                                        if (strProductID == "") {
                                            strProductID = vDataItem.Rows[p].StockID;
                                        }

                                        if (vDataItemSerial.Rows.length > 0) {
                                            if (strProductID != vDataItem.Rows[p].StockID) { //Print Serials

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Serial(s) : ', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                for (var intSerial = 0; intSerial <= vDataItemSerial.Rows.length - 1; intSerial++) {

                                                    if (strProductID == vDataItemSerial.Rows[intSerial].ProductID && vDataItemSerial.Rows[intSerial].InvoiceID == pInvoiceID) {

                                                        var strSerial = $.trim((vDataItemSerial.Rows[intSerial].SerialNumber || ''));

                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, strSerial, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                        strPrint = strPrint + strTemp;

                                                    }
                                                }

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                strProductID = vDataItem.Rows[p].StockID;
                                            }
                                        }

                                        if (CBool(vOptions.PrintAsOrderSlip)) {
                                            WindowsPrinter((strStockDescription || '0') + ' x ' + CDec2(strQty), [1, 0, 0, 0, 0, 36]);
                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, strStockDescription + ' x ' + CDec2(strQty) + ' ' + $.trim(strUnitMeasure), intFAFCTotalLenght, '', 0, '', 0, 0);
                                            //strPrint = strPrint + strTemp;
                                        } else {
                                            WindowsPrinter(strStockDescription, [0, 0, 0, 0, 0, 36]);

                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, strStockDescription, intFAFCTotalLenght, '', 0, '', 0, 0);
                                            //strPrint = strPrint + strTemp;

                                            var itemtotal = SetPrefixPostfix(AccountingFormat(CDec2(strQty) * CDec2(strUnitPrice)) || '0', intFAFCTotalLenght - 22, ' ', 0);
                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' ' + $.trim(strQty) + ' @ ' + $.trim(strUnitPrice) + ' / ' + $.trim(strUnitMeasure), 20, (CBool(vOptions.PrintAsOrderSlip) ? '' : itemtotal), intFAFCTotalLenght, '', 0, 0);
                                            //strPrint = strPrint + strTemp;
                                            WindowsPrinter($.trim(strQty) + ' @ ' + $.trim(strUnitPrice) + ' / ' + $.trim(strUnitMeasure), [0, 0, 0, 0, 0, 36], (CBool(vOptions.PrintAsOrderSlip) ? '' : itemtotal), [0, 0, 0, 0, 1, 36]);
                                            //WindowsPrinter($.trim(strQty) + ' @ ' + $.trim(strUnitPrice) + ' / ' + $.trim(strUnitMeasure), [1, 18, 0, 0, 0, 36], '', [0, 2, 1, 0, 0, 36], (CBool(vOptions.PrintAsOrderSlip) ? '' : itemtotal), [0, 10, 1, 0, 1, 36]);
                                        }

                                        if (vDataItem.Rows[p].ItemModifier != undefined) {
                                            if (vDataItem.Rows[p].ItemModifier.length > 0) {
                                                var imTotal = 0;
                                                var showSubTotal = false; // use in printing order slip
                                                for (var im = 0; im < vDataItem.Rows[p].ItemModifier.length; im++) {
                                                    var mod = vDataItem.Rows[p].ItemModifier[im];
                                                    var unitprice, subtotal;
                                                    if (!CBool(vOptions.PrintAsOrderSlip)) {
                                                        unitprice = AccountingFormat(mod.ModifierPrice);
                                                        subtotal = AccountingFormat(mod.SubTotal);
                                                        showSubTotal = true;
                                                        WindowsPrinter('* ' + mod.Description, [0, 0, 0, 0, 0, 36]);
                                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, '* ' + mod.Description, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                        //strPrint = strPrint + strTemp;
                                                    }

                                                    if (CStr2(mod.OrderedQty) != '') { // for Inventory Affecting Modifiers
                                                        if (CBool(vOptions.PrintAsOrderSlip)) {
                                                            WindowsPrinter('* ' + CDec2(mod.OrderedQty) + ' ' + mod.Description + ' / ' + $.trim(strUnitMeasure), [0, 0, 0, 0, 0, 36]);
                                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, '* ' + CDec2(mod.OrderedQty) + ' ' + mod.Description + ' / ' + $.trim(strUnitMeasure), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                            //strPrint = strPrint + strTemp;
                                                        } else {
                                                            var itemmodifiertotal = SetPrefixPostfix(AccountingFormat(CDec2(mod.OrderedQty) * CDec2(mod.ModifierPrice) * CDec2(strQty)) || '0', intFAFCTotalLenght - 22, ' ', 0);

                                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' ' + mod.OrderedQty + ' @ ' + AccountingFormat($.trim(mod.ModifierPrice)) + ' @ ' + $.trim(strQty) + ' ' + $.trim(strUnitMeasure), 20, (!CBool(showSubTotal) ? '' : itemmodifiertotal), intFAFCTotalLenght, '', 0, 0);
                                                            //strPrint = strPrint + strTemp;
                                                            WindowsPrinter(' ' + mod.OrderedQty + ' @ ' + AccountingFormat($.trim(mod.ModifierPrice)) + ' @ ' + $.trim(strQty) + ' ' + $.trim(strUnitMeasure), [0, 0, 0, 0, 0, 36], (!CBool(showSubTotal) ? '' : itemmodifiertotal), [0, 0, 0, 0, 1, 36]);

                                                            imTotal += CDec2(mod.SubTotal)
                                                        }
                                                    }

                                                    if (CBool(vOptions.PrintAsOrderSlip) && CStr2(mod.OrderedQty) == '') { // for Non-Inventory Affecting Modifiers
                                                        WindowsPrinter('* ' + mod.Description, [0, 0, 0, 0, 0, 36]);
                                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, '* ' + mod.Description, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                        //strPrint = strPrint + strTemp;
                                                    }
                                                }
                                                if (showSubTotal) {
                                                    WindowsPrinter('   Item Total:  ---->', [0, 0, 0, 0, 0, 36], AccountingFormat(strSubTotal), [0, 0, 0, 0, 1, 36]);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, '   Item Total:  ---->', 20, SetPrefixPostfix(AccountingFormat(strSubTotal), intFAFCTotalLenght - 22, ' ', 0), intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                }
                                            }
                                        }

                                        //if (IsShowSalesRep) {
                                        //    WindowsPrinter('Sales Person: ' + ($.trim(vDataItem.Rows[p].SalesPerson || '')), [0, 0, 0, 0, 0, 36]);
                                        //    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sales Person: ' + ($.trim(vDataItem.Rows[p].SalesPerson || '')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                        //    //strPrint = strPrint + strTemp;

                                        //}

                                        if (vDataItemSerial.Rows.length > 0) {
                                            if (p == vDataItem.Rows.length - 1) {

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Serial(s) : ', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                for (var intSerial = 0; intSerial <= vDataItemSerial.Rows.length - 1; intSerial++) {

                                                    if (strProductID == vDataItemSerial.Rows[intSerial].ProductID && vDataItemSerial.Rows[intSerial].InvoiceID == pInvoiceID) {

                                                        var strSerial = $.trim((vDataItemSerial.Rows[intSerial].SerialNumber || ''));

                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, strSerial, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                        strPrint = strPrint + strTemp;

                                                    }
                                                }

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                strProductID = vDataItem.Rows[p].StockID;
                                            }
                                        } else {
                                            WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                            //strPrint = strPrint + strTemp;
                                        }

                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        intLimitPerBatchPrint = intLimitPerBatchPrint + 1;

                                        if (intLimitPerBatchPrint >= 100 && vDataItem.Rows.length > 100) {

                                            //PrintText(strPrinterIP);
                                            //ClearCommandBuffer();

                                            //New set of text to print because it causes error when you print directly all items at once.
                                            //NewPrintBuilder();
                                            intLimitPerBatchPrint = 0;
                                        }

                                    } // END for (var p = 0; p <= vDataItem.Rows.length - 1; p++) {

                                    //Get Footer Amounts
                                    if (vDataFooter.Rows.length > 0) {
                                        var strSKUCount = SetPrefixPostfix(AccountingFormat((vDataFooter.Rows[0].SKUCount || '0'), 0), intFAFCTotalLenght - 14, ' ', 0);
                                        var strTotalAmountDue = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].SubTotal || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                        var strTotalDiscount = SetPrefixPostfix(AccountingFormat(-Math.abs(vDataFooter.Rows[0].DiscountAmount || '0')), intFAFCTotalLenght - 22, ' ', 0);
                                        var strNetAmountDue = SetPrefixPostfix(AccountingFormat(CDec2(vDataFooter.Rows[0].NetAmountDue) + CDec2(vDataFooter.Rows[0].ChargeAmount) + CDec2(vDataFooter.Rows[0].TipAmount) + CDec2(vDataFooter.Rows[0].LocalTaxAmount)), intFAFCTotalLenght - 22, ' ', 0);

                                        var strTotalCharges = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].ChargeAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                        var strTip = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TipAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                        var strVatSale = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TotalAmountDue || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                        var strVatAmount = SetPrefixPostfix(AccountingFormat(-Math.abs(CDec2(vDataFooter.Rows[0].SubTotal) - CDec2(vDataFooter.Rows[0].TotalAmountDue))), intFAFCTotalLenght - 22, ' ', 0);
                                        var strOtherTax = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].LocalTaxAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                        WindowsPrinter('SKU Count: ', [0, 0, 0, 0, 0, 36], strSKUCount, [0, 0, 0, 0, 1, 36]);

                                        //strPrint = '';
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SKU Count: ', 12, strSKUCount, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;

                                        if (!CBool(vOptions.PrintAsOrderSlip)) {

                                            WindowsPrinter('Sub Total: ', [0, 0, 0, 0, 0, 36], strTotalAmountDue, [0, 0, 0, 0, 1, 36]);

                                            if (CBool(vOptions.isSeniorCitizenDiscount)) {

                                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);

                                                if (vOptions.SCDiscountExempt > 0) {
                                                    var strSCDiscountExempt = SetPrefixPostfix(AccountingFormat(SCDiscountExempt || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                                    WindowsPrinter('SC Discount-Exempt: ', [0, 0, 0, 0, 0, 36], strSCDiscountExempt, [0, 0, 0, 0, 1, 36]);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SC Discount-Exempt: ', 20, strSCDiscountExempt, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                }

                                                var strSubTotalOfVatExemptItems = SetPrefixPostfix(AccountingFormat(vOptions.SubTotalOfVatExemptItems || '0'), intFAFCTotalLenght - 31, ' ', 0);
                                                WindowsPrinter('\nGross Amount Subject to SCD: ', [0, 0, 0, 0, 0, 36], strSubTotalOfVatExemptItems, [0, 0, 0, 0, 1, 36]);
                                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, '\nGross Amount Subject to SCD: ', 29, strSubTotalOfVatExemptItems, intFAFCTotalLenght, '', 0, 0);
                                                //strPrint = strPrint + strTemp;

                                                if (vOptions.GrossSalesToNoneSC > 0) {
                                                    var strGrossSalesToNoneSC = SetPrefixPostfix(AccountingFormat(vOptions.GrossSalesToNoneSC || '0'), intFAFCTotalLenght - 31, ' ', 0);
                                                    WindowsPrinter('Gross Sales to None-SC ' + (CInt2($('#txtSalesPayNoOfDiners').val()) - CInt2($('#txtSalesPayNoOfSC').val())) + '/' + $('#txtSalesPayNoOfDiners').val() + ':', [0, 0, 0, 0, 0, 36], strGrossSalesToNoneSC, [0, 0, 0, 0, 1, 36]);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Gross Sales to None-SC ' + (CInt2($('#txtSalesPayNoOfDiners').val()) - CInt2($('#txtSalesPayNoOfSC').val())) + '/' + $('#txtSalesPayNoOfDiners').val() + ':', 29, strGrossSalesToNoneSC, intFAFCTotalLenght, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;
                                                }

                                                var strGrossSalesToSC = SetPrefixPostfix(AccountingFormat(vOptions.GrossSalesToSC || '0'), intFAFCTotalLenght - 23, ' ', 0);
                                                WindowsPrinter('\nGross Sales to SC ' + $('#txtSalesPayNoOfSC').val() + '/' + $('#txtSalesPayNoOfDiners').val() + ':', [0, 0, 0, 0, 0, 36], strGrossSalesToSC, [0, 0, 0, 0, 1, 36]);
                                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, '\nGross Sales to SC ' + $('#txtSalesPayNoOfSC').val() + '/' + $('#txtSalesPayNoOfDiners').val() + ':', 21, strGrossSalesToSC, intFAFCTotalLenght, '', 0, 0);
                                                //strPrint = strPrint + strTemp;

                                                var strVatExemptSales = SetPrefixPostfix(AccountingFormat(vOptions.VatExemptSales || '0') + (vOptions.IsApplyMaxSCDiscountableAmount ? '' : '*'), intFAFCTotalLenght - 22, ' ', 0);
                                                WindowsPrinter('VAT-Ex for SC: ', [0, 0, 0, 0, 0, 36], strVatExemptSales, [0, 0, 0, 0, 1, 36]);
                                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Ex for SC: ', 20, strVatExemptSales, intFAFCTotalLenght, '', 0, 0);
                                                //strPrint = strPrint + strTemp;

                                                var strMaxSCDiscountableAmount = SetPrefixPostfix(AccountingFormat(vOptions.MaxSCDiscountableAmount || '0') + (vOptions.IsApplyMaxSCDiscountableAmount ? '*' : ''), intFAFCTotalLenght - 25, ' ', 0);
                                                WindowsPrinter('Max Discountable VAT-EX(' + (CDec2(vOptions.MaxSCDiscountableAmount) / CInt2($('#txtSalesPayNoOfSC').val())) + '*' + CInt2($('#txtSalesPayNoOfSC').val()) + ')', [0, 0, 0, 0, 0, 36], strMaxSCDiscountableAmount, [0, 0, 0, 0, 1, 36]);
                                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Max Discountable VAT-EX(' + (CDec2(vOptions.MaxSCDiscountableAmount) / CInt2($('#txtSalesPayNoOfSC').val())) + '*' + CInt2($('#txtSalesPayNoOfSC').val()) + ')', 23, strMaxSCDiscountableAmount, intFAFCTotalLenght, '', 0, 0);
                                                //strPrint = strPrint + strTemp;

                                                var strDiscountPercentCeiling = SetPrefixPostfix(AccountingFormat($('#txtDiscountPercentCeiling').val()) + '%', intFAFCTotalLenght - 22, ' ', 0);
                                                WindowsPrinter('SC Discount Rate: ', [0, 0, 0, 0, 0, 36], strDiscountPercentCeiling, [0, 0, 0, 0, 1, 36]);
                                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SC Discount Rate: ', 20, strDiscountPercentCeiling, intFAFCTotalLenght, '', 0, 0);
                                                //strPrint = strPrint + strTemp;

                                                var strseniorCitizenDiscount = SetPrefixPostfix(AccountingFormat(vOptions.seniorCitizenDiscount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                                WindowsPrinter('SC Discount Amount based on Vat-Ex Sale: ', [0, 0, 0, 0, 0, 36], strseniorCitizenDiscount, [0, 0, 0, 0, 1, 36]);
                                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SC Discount Amount based on Vat-Ex Sale: ', 20, strseniorCitizenDiscount, intFAFCTotalLenght, '', 0, 0);
                                                //strPrint = strPrint + strTemp;

                                                var strseniorCitizenNetAmount = SetPrefixPostfix(AccountingFormat(vOptions.seniorCitizenNetAmount || '0'), intFAFCTotalLenght - 32, ' ', 0);
                                                WindowsPrinter('Net Amount to be Paid by SC: ', [0, 0, 0, 0, 0, 36], strseniorCitizenNetAmount, [0, 0, 0, 0, 1, 36]);
                                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Net Amount to be Paid by SC: ', 30, strseniorCitizenNetAmount, intFAFCTotalLenght, '', 0, 0);
                                                //strPrint = strPrint + strTemp;

                                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);

                                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                //strPrint = strPrint + strTemp;

                                                //WindowsPrinter('Less: ' + vDataFooter.Rows[0].TaxPercent + '% VAT ', [0, 0, 0, 0, 0, 36], strVatAmount, [0, 0, 0, 0, 1, 36]);

                                                //WindowsPrinter('Total Amount: ', [0, 0, 0, 0, 0, 36], strVatSale, [0, 0, 0, 0, 1, 36]);

                                            } else {
                                                WindowsPrinter('Total Discount: ', [0, 0, 0, 0, 0, 36], strTotalDiscount, [0, 0, 0, 0, 1, 36]);
                                            }

                                            if (CDec2(vDataFooter.Rows[0].ChargeAmount) > 0) {
                                                WindowsPrinter('Total Charges: ', [0, 0, 0, 0, 0, 36], strTotalCharges, [0, 0, 0, 0, 1, 36]);

                                            }

                                            if (CDec2(vDataFooter.Rows[0].TipAmount) > 0) {
                                                WindowsPrinter('Tip: ', [0, 0, 0, 0, 0, 36], strTip, [0, 0, 0, 0, 1, 36]);

                                            }

                                            if (CDec2(vDataFooter.Rows[0].LocalTaxAmount) > 0) {
                                                WindowsPrinter('Local Tax: ', [0, 0, 0, 0, 0, 36], strOtherTax, [0, 0, 0, 0, 1, 36]);

                                            }

                                            WindowsPrinter('Amount Due: ', [0, 0, 0, 0, 0, 36], strNetAmountDue, [0, 0, 0, 0, 1, 36]);

                                        } // END if (!CBool(vOptions.PrintAsOrderSlip)) {

                                        WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);

                                        //strPrint = '';
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                        //strPrint = strPrint + strTemp;

                                        //                                        var strVATSales = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TaxSales || '0'), intFAFCTotalLenght - 20, ' ', 0);
                                        //                                        var strVATAmount = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TaxAmount || '0'), intFAFCTotalLenght - 15, ' ', 0);
                                        //                                        var strNonVAT = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].NonVATSale || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                        //                                        var strVATExempt = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].VATExemptSale || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                        //                                        var strVATZeroRated = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].ZeroRatedSale || '0'), intFAFCTotalLenght - 25, ' ', 0);

                                        //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales: ', 18, strVATSales, intFAFCTotalLenght, '', 0, 0);
                                        //                                        strPrint = strPrint + strTemp;

                                        //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount: ', 13, strVATAmount, intFAFCTotalLenght, '', 0, 0);
                                        //                                        strPrint = strPrint + strTemp;

                                        //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT Sales: ', 20, strNonVAT, intFAFCTotalLenght, '', 0, 0);
                                        //                                        strPrint = strPrint + strTemp;

                                        //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Exempt Sales: ', 20, strVATExempt, intFAFCTotalLenght, '', 0, 0);
                                        //                                        strPrint = strPrint + strTemp;

                                        //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT Sales: ', 23, strVATZeroRated, intFAFCTotalLenght, '', 0, 0);
                                        //                                        strPrint = strPrint + strTemp;

                                        //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                        //                                        strPrint = strPrint + strTemp;

                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);



                                        ////Get Footer Messages
                                        //strPrint = '';
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, '* Please ask for an official receipt *', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //strPrint = strPrint + strTemp;

                                        //if (!CBool(vOptions.PrintAsOrderSlip)) {
                                        //    strPrint = 'SO|' + vDataInvHeader.Rows[0].OrganizationID + '|' + vDataInvHeader.Rows[0].OrganizationBranchID + '|' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0)
                                        //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);
                                        //}

                                        //ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                        //PrintText(strPrinterIP);

                                        WindowsPrinter("address", [0, 0, 0, 0, 0, 0], strPrinterIP, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);

                                        if (CBool(vOptions.PrintAsOrderSlip)) {
                                            WindowsPrinter("printerName", [0, 0, 0, 0, 0, 0], strPrinterName, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                        }

                                        WindowsPrinter("startPrint", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                        //PrintText(strPrinterIP);
                                        //ClearCommandBuffer();

                                        //ClearCommandBuffer();

                                        //Print SRV
                                        //PrintSalesPaymentSRVReceipt(0, pInvoiceID);

                                        if (!(vOptions.DontConfirm))

                                            setTimeout(VerifyPrintReceipt(pInvoiceID), 1000);

                                        //Check Print Order Reference
                                        if (OfflineMode) {
                                            if (Business.SystemDefaults.GetSystemDefault("When Do You Want To Print Order Slip") == 'After Payment' &&
                                            Business.SystemDefaults.GetSystemDefault("Print Order Slip") == '1') {

                                                var pDataSO;
                                                pDataSO = {
                                                    InvoiceID: pInvoiceID
                                                };

                                                if (SalesMenu_offline().SaveSalesOrder(pDataSO)) {
                                                    PrintSalesOrderBOM(pInvoiceID);
                                                }

                                            } //if (Business.SystemDefaults.GetSystemDefault("When Do You Want To Print Order Reference") == 'Before Payment'){
                                        }

                                    }
                                }
                                //                                    }

                            }
                        }
                    }
                    else {
                        NewMessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                            "Printer",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                        );
                    }
                } // END function PrintBillingWPSNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg, vOptions) {

                function PreviewBill(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataFooter, vDataMsg) {
                    var strTemp = '';
                    var strPrint = '';
                    var strPrinterIP = '';
                    var strStockID = '';
                    var IsShowSalesRep = false;
                    var pData = {};
                    var intFAFCTotalLenght = 40;
                    var intFBTotalLenght = 27;
                    var intFCDoubleTotalLenght = 18;

                    pData.InvoiceID = pInvoiceID;



                    if (vDataHeader.Rows.length > 0) {

                        $('#lblOrganizationBusinessName').text($.trim((vDataHeader.Rows[0].BusinessTradeName || '')));

                        if (vDataInvHeader.Rows.length > 0) {

                            $('#lblCustomerNamePreviewBill').text($.trim((vDataInvHeader.Rows[0].CustomerName || '')));
                            $('#lblAddressPreviewBill').text($.trim((vDataInvHeader.Rows[0].CustomerAddress || '')));
                            $('#lblTINPreviewBill').text($.trim((vDataInvHeader.Rows[0].CustomerTIN || '')));
                            if (CStr2(vDataInvHeader.Rows[0].CustomerTableNo) != '') {
                                var prefix = vDataInvHeader.Rows[0].CustomerTableNo.split('_');
                                $('#lblTableNoPreviewBill').text($.trim((prefix[1] || '')));

                            }
                            $('#lblBillNoPreviewBill').text('Bill ' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].SalesOrderNo || '')), 12, '0', 0));
                            $('#lblDateTimePreviewBill').text($.format.date(new Date(), 'MMM dd, yyyy h:mm a'));

                            $('#lblCashierNamePreviewBill').text($.trim((vDataInvHeader.Rows[0].CashierName || '')));
                        }

                        //Get Items
                        var pLV = $("#tblBillBody");
                        var pItem;
                        pLV.html('');

                        if (vDataItem.Rows.length > 0) {
                            for (var p = 0; p <= vDataItem.Rows.length - 1; p++) {
                                var strStockDescription = $.trim(vDataItem.Rows[p].StockDescription || '');
                                var strUnitPrice = AccountingFormat(vDataItem.Rows[p].UnitPrice || '0');
                                var strQty = AccountingFormat(vDataItem.Rows[p].Quantity || '0');
                                var strUnitMeasure = vDataItem.Rows[p].UnitMeasure || '';
                                var strSubTotal = AccountingFormat(vDataItem.Rows[p].SubTotal || '0');

                                pItem = $('<tr style="cursor: pointer;">')
                    .append($('<td>')
                        .text(strStockDescription)
                    )
                        .append($('<td style="text-align: center;">')
                        .text(strQty)
                    )
                    .append($('<td>')
                        .text(strUnitMeasure)
                    )
                    .append($('<td style="text-align: right;">')
                        .text(strUnitPrice)
                    )
                    .append($('<td style="text-align: right;">')
                        .text(strSubTotal)
                    )
                                pLV.append(pItem)

                            }
                        }
                    }

                    //Get Footer Amounts
                    if (vDataFooter.Rows.length > 0) {

                        $('#lblSKUCountBilling').text(AccountingFormat((vDataFooter.Rows[0].SKUCount || '0'), 0));
                        $('#lblSubTotalBilling').text(AccountingFormat(vDataFooter.Rows[0].SubTotal || '0'));
                        $('#lblSalesPayTotalDiscountBilling').text(AccountingFormat(-Math.abs(vDataFooter.Rows[0].DiscountAmount || '0')));
                        $('#lblSalesPayAmountDueBilling').text(AccountingFormat(CDec2(vDataFooter.Rows[0].TotalAmountDue) - CDec2(vDataFooter.Rows[0].DiscountAmount) + CDec2(vDataFooter.Rows[0].ChargeAmount) + CDec2(vDataFooter.Rows[0].TipAmount) + CDec2(vDataFooter.Rows[0].LocalTaxAmount)));
                        $('#lblOriginalAmountDueBilling').text(AccountingFormat(vDataFooter.Rows[0].TotalAmountDue || '0'));
                        dblTotalAmountDueForDiscount = 0;
                        dblTotalAmountDueForDiscount = vDataFooter.Rows[0].TotalAmountDue || 0;

                        $('#lblSalesPayChargesBilling').text(AccountingFormat(vDataFooter.Rows[0].ChargeAmount || '0'));
                        $('#lblTipBilling').text(AccountingFormat(vDataFooter.Rows[0].TipAmount || '0'));
                        lblVATableSales = vDataFooter.Rows[0].TaxSales || 0;
                        $('#lblVATableSales').text(AccountingFormat(vDataFooter.Rows[0].TaxSales || '0'));
                        $('#lblVATAmount').text(AccountingFormat(vDataFooter.Rows[0].TaxAmount || '0'));
                        $('#lblNonVATSales').text(AccountingFormat(vDataFooter.Rows[0].NonVATSale || '0'));
                        $('#lblVATExemptSales').text(AccountingFormat(vDataFooter.Rows[0].VATExemptSale || '0'));
                        $('#lblZeroRatedVATSales').text(AccountingFormat(vDataFooter.Rows[0].ZeroRatedSale || '0'));
                    }

                    if (CDec2(vDataFooter.Rows[0].VATExemptSale) > 0) {

                        $('#rowVATPercentBilling').show();
                        $('#rowAmountLessVATBilling').show();

                        $('#lblVATPercentValueBilling').text(vDataFooter.Rows[0].TaxPercent);

                        $('#lblVATPercentBilling').text(AccountingFormat(-Math.abs(CDec2(vDataFooter.Rows[0].SubTotal) - CDec2(vDataFooter.Rows[0].TotalAmountDue))));
                        $('#lblTotalAmountBilling').text(AccountingFormat(vDataFooter.Rows[0].TotalAmountDue));


                    } else {
                        $('#rowVATPercentBilling').hide();
                        $('#rowAmountLessVATBilling').hide();
                    }

                    window.pageDiscounts.getCurrentDiscount();
                };

                function PrintDispatchSlip(pInvoiceID, vOptions) {

                    //Get Header
                    var vDataHeader = SalesPayment_offline().getReceiptHeaderInfo(pInvoiceID);
                    //Get Invoice Header
                    var vDataInvHeader = SalesPayment_offline().getSalesPaymentInvoiceHeaderInfo(pInvoiceID);
                    //Get Invoice Items
                    var vDataItem = SalesPayment_offline().getSalesPaymentReceiptBodyInfo(pInvoiceID, vOptions);

                    var vDataFooter = SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(pInvoiceID, vOptions);

                    var vDataPrint = SalesPayment_offline().getDipatchPrinter();

                    var DeviceDataPrint = SalesPayment_offline().getOrgFOWSTPrinterType();

                    //if this device is using WPS for printing, then use wps format
                    if (DeviceDataPrint.Rows.length > 0) {
                        if (DeviceDataPrint.Rows[0].PrinterType == 'Windows Print Server') {
                            PrintDispatchSlipWPSNow(pInvoiceID, vOptions, vDataHeader, vDataInvHeader, vDataItem, vDataFooter, vDataPrint, DeviceDataPrint);
                            return;
                        }
                    }

                    var strTemp = '';
                    var strPrint = '';
                    var strPrinterIP = '';
                    var strStockID = '';
                    var IsShowSalesRep = false;
                    var pData = {};
                    var intFAFCTotalLenght = 40;
                    var intFBTotalLenght = 27;
                    var intFCDoubleTotalLenght = 18;
                    var pTableNum;
                    var pTableName;

                    pData.InvoiceID = pInvoiceID;
                    vOptions = $.extend({
                        PrintAsOrderSlip: false
                    }, vOptions);

                    //Get Printer IP Address
                    if ((vDataPrint == undefined)
                        || !$.isArray(vDataPrint.Rows)
                        || (vDataPrint.Rows.length == 0)) {
                        NewMessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                            "Printer",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                        );
                        return;
                    }

                    //additional by janine jan 31 2015
                    originalDevFowstPrinterID = vDataPrint.Rows[0].ID;
                    if (vDataPrint.Rows.length > 0) {
                        if (CStr2(newPrinterIP) != '')
                            strPrinterIP = newPrinterIP;
                        else
                            strPrinterIP = vDataPrint.Rows[0].PrinterIPAddress;

                        setPrinterConnectionType(vDataPrint.Rows[0].ConnectionType, vDataPrint.Rows[0].PrinterType, true);
                        //New set of text to print
                        NewPrintBuilder();

                        //Get Header
                        IsShowSalesRep = false;
                        if (vDataHeader.Rows.length > 0) {

                            IsShowSalesRep = (vDataHeader.Rows[0].IsShowSalesRep == 1 ? true : false);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].FowstName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            //Get Items
                            if (vDataInvHeader.Rows.length > 0) {
                                strPrint = '\n\n';

                                pTableNum = vDataInvHeader.Rows[0].CustomerTableNo;
                                if (CStr2(pTableNum) != '') {
                                    if (IsMobile)
                                        pTableName = mTableView.GetTableInfoByInternalName(pTableNum);
                                    if (CStr2(pTableName).trim() == '') {
                                        var prefix = pTableNum.split('_');
                                        pTableNum = prefix[prefix.length - 1];
                                    } else
                                        pTableNum = pTableName;

                                    if ($.trim((pTableNum || '')) != '') {
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Table #', 8, ' ', 1), 7, ': ' + $.trim((pTableNum || '')), intFAFCTotalLenght, '', 0, 0);
                                        strPrint = strTemp;
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 1, 0, 0, 0, 0);
                                    }
                                }

                                strPrint = '';

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = '';

                                strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'Dispatch Slip ', intFCDoubleTotalLenght, '', 0, '', 0, 1);

                                strPrint += strTemp;

                                strTemp = FormatTextToPrint(intFCDoubleTotalLenght, $.format.date(new Date(), 'MMM dd, yyyy H:mm'), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //Get Items
                                var strProductID = "";
                                if (vDataItem.Rows.length > 0) {
                                    strStockID = '';
                                    strPrint = '';
                                    var intLimitPerBatchPrint = 0;
                                    for (var p = 0; p <= vDataItem.Rows.length - 1; p++) {
                                        if (CBool(vOptions.PrintAsOrderSlip) && CBool(vDataItem.Rows[p].IsOrderSlipPrinted))
                                            continue;

                                        var pQtyPrinted = CDec2(vDataItem.Rows[p].QuantityPrinted);
                                        var pQty = CDec2(vDataItem.Rows[p].Quantity);
                                        var pQtyToPrint;
                                        if (CBool(vOptions.PrintAsOrderSlip))
                                            pQtyToPrint = pQty - pQtyPrinted;
                                        else
                                            pQtyToPrint = pQty;
                                        var strStockDescription = $.trim((vDataItem.Rows[p].StockDescription || ''));
                                        var strQty = SetPrefixPostfix(AccountingFormat(pQtyToPrint, 0), 8, ' ', 0);
                                        var strUnitMeasure = SetPrefixPostfix((vDataItem.Rows[p].UnitMeasure || ''), 10, ' ', 0);
                                        var strUnitPrice = '';
                                        var strSubTotal = '';

                                        strPrint = '';
                                        if (strProductID == "") {
                                            strProductID = vDataItem.Rows[p].StockID;
                                        }

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '  ' + strStockDescription + ' x ' + CDec2(strQty) + ' ' + $.trim(strUnitMeasure), intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        if (vDataItem.Rows[p].ItemModifier != undefined) {
                                            if (vDataItem.Rows[p].ItemModifier.length > 0) {
                                                var imTotal = 0;
                                                var showSubTotal = false; // use in printing order slip
                                                for (var im = 0; im < vDataItem.Rows[p].ItemModifier.length; im++) {
                                                    var mod = vDataItem.Rows[p].ItemModifier[im];
                                                    var unitprice, subtotal;

                                                    if (CStr2(mod.OrderedQty) != '') { // for Inventory Affecting Modifiers                                                                        
                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '* ' + CDec2(mod.OrderedQty) + ' ' + mod.Description + ' / ' + $.trim(strUnitMeasure), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                        strPrint = strPrint + strTemp;
                                                    }

                                                    if (CStr2(mod.OrderedQty) == '') { // for Non-Inventory Affecting Modifiers
                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '* ' + mod.Description, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                        strPrint = strPrint + strTemp;
                                                    }
                                                }
                                            }
                                        }

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;


                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        intLimitPerBatchPrint = intLimitPerBatchPrint + 1;

                                        if (intLimitPerBatchPrint >= 100 && vDataItem.Rows.length > 100) {

                                            PrintText(strPrinterIP);
                                            //ClearCommandBuffer();

                                            //New set of text to print because it causes error when you print directly all items at once.
                                            NewPrintBuilder();
                                            intLimitPerBatchPrint = 0;
                                        }

                                    } // END for (var p = 0; p <= vDataItem.Rows.length - 1; p++) {

                                    //Get Footer Amounts
                                    if (vDataFooter.Rows.length > 0) {
                                        var strSKUCount = SetPrefixPostfix(AccountingFormat((vDataFooter.Rows[0].SKUCount || '0'), 0), intFAFCTotalLenght - 14, ' ', 0);

                                        strPrint = '';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SKU Count: ', 12, strSKUCount, intFAFCTotalLenght, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    }
                                }

                                //paper cut
                                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                PrintText(strPrinterIP);
                                //ClearCommandBuffer();

                                setTimeout(VerifyDispatchSlipPrintReceipt(pInvoiceID), 2000);
                            }
                        }
                    }
                    else {
                        NewMessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                            "Printer",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                        );
                    }
                }

                function PrintDispatchSlipWPSNow(pInvoiceID, vOptions, vDataHeader, vDataInvHeader, vDataItem, vDataFooter, vDataPrint, WPSServer) {
                    var strTemp = '';
                    var strPrint = '';
                    var strPrinterIP = '';
                    var strStockID = '';
                    var IsShowSalesRep = false;
                    var pData = {};
                    var intFAFCTotalLenght = 40;
                    var intFBTotalLenght = 27;
                    var intFCDoubleTotalLenght = 18;
                    var pTableNum;
                    var pTableName;

                    pData.InvoiceID = pInvoiceID;
                    vOptions = $.extend({
                        PrintAsOrderSlip: false
                    }, vOptions);

                    //Get Printer IP Address
                    if ((vDataPrint == undefined)
                        || !$.isArray(vDataPrint.Rows)
                        || (vDataPrint.Rows.length == 0)) {
                        NewMessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                            "Printer",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                        );
                        return;
                    }

                    //additional by janine jan 31 2015
                    originalDevFowstPrinterID = vDataPrint.Rows[0].ID;
                    if (vDataPrint.Rows.length > 0) {
                        if (CStr2(newPrinterIP) != '')
                            strPrinterIP = newPrinterIP;
                        else {
                            strPrinterIP = WPSServer.Rows[0].PrinterIPAddress;
                            strPrinterName = vDataPrint.Rows[0].PrinterName;
                        }

                        //setPrinterConnectionType(vDataPrint.Rows[0].ConnectionType, vDataPrint.Rows[0].PrinterType, true);
                        //New set of text to print
                        //NewPrintBuilder();

                        //Get Header
                        IsShowSalesRep = false;
                        if (vDataHeader.Rows.length > 0) {

                            IsShowSalesRep = (vDataHeader.Rows[0].IsShowSalesRep == 1 ? true : false);

                            WindowsPrinter($.trim((vDataHeader.Rows[0].BusinessTradeName || '')), [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter($.trim((vDataHeader.Rows[0].FowstName || '')), [1, 0, 0, 1, 0, 36]);

                            //Get Items
                            if (vDataInvHeader.Rows.length > 0) {
                                strPrint = '\n\n';

                                pTableNum = vDataInvHeader.Rows[0].CustomerTableNo;
                                if (CStr2(pTableNum) != '') {
                                    if (IsMobile)
                                        pTableName = mTableView.GetTableInfoByInternalName(pTableNum);
                                    if (CStr2(pTableName).trim() == '') {
                                        var prefix = pTableNum.split('_');
                                        pTableNum = prefix[prefix.length - 1];
                                    } else
                                        pTableNum = pTableName;

                                    if ($.trim((pTableNum || '')) != '') {
                                        WindowsPrinter('Table # : ' + $.trim((pTableNum || '')), [0, 0, 0, 0, 0, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Table #', 8, ' ', 1), 7, ': ' + $.trim((pTableNum || '')), intFAFCTotalLenght, '', 0, 0);                                                    
                                        //strPrint = strTemp;
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 1, 0, 0, 0, 0);
                                    }
                                }

                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);

                                //strPrint = '';
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                //strPrint = '';

                                WindowsPrinter('Dispatch Slip', [0, 0, 0, 1, 0, 36]);

                                //strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'Dispatch Slip ', intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                //strPrint += strTemp;

                                WindowsPrinter($.format.date(new Date(), 'MMM dd, yyyy H:mm'), [0, 0, 0, 1, 0, 36]);
                                //strTemp = FormatTextToPrint(intFCDoubleTotalLenght, $.format.date(new Date(), 'MMM dd, yyyy H:mm'), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //Get Items
                                var strProductID = "";
                                if (vDataItem.Rows.length > 0) {
                                    strStockID = '';
                                    strPrint = '';
                                    var intLimitPerBatchPrint = 0;
                                    for (var p = 0; p <= vDataItem.Rows.length - 1; p++) {
                                        if (CBool(vOptions.PrintAsOrderSlip) && CBool(vDataItem.Rows[p].IsOrderSlipPrinted))
                                            continue;

                                        var pQtyPrinted = CDec2(vDataItem.Rows[p].QuantityPrinted);
                                        var pQty = CDec2(vDataItem.Rows[p].Quantity);
                                        var pQtyToPrint;
                                        if (CBool(vOptions.PrintAsOrderSlip))
                                            pQtyToPrint = pQty - pQtyPrinted;
                                        else
                                            pQtyToPrint = pQty;
                                        var strStockDescription = $.trim((vDataItem.Rows[p].StockDescription || ''));
                                        var strQty = SetPrefixPostfix(AccountingFormat(pQtyToPrint, 0), 8, ' ', 0);
                                        var strUnitMeasure = SetPrefixPostfix((vDataItem.Rows[p].UnitMeasure || ''), 10, ' ', 0);
                                        var strUnitPrice = '';
                                        var strSubTotal = '';

                                        strPrint = '';
                                        if (strProductID == "") {
                                            strProductID = vDataItem.Rows[p].StockID;
                                        }

                                        WindowsPrinter((strStockDescription || '0') + ' x ' + CDec2(strQty), [1, 0, 0, 0, 0, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, '  ' + strStockDescription + ' x ' + CDec2(strQty) + ' ' + $.trim(strUnitMeasure), intFAFCTotalLenght, '', 0, '', 0, 0);
                                        //strPrint = strPrint + strTemp;

                                        if (vDataItem.Rows[p].ItemModifier != undefined) {
                                            if (vDataItem.Rows[p].ItemModifier.length > 0) {
                                                var imTotal = 0;
                                                var showSubTotal = false; // use in printing order slip
                                                for (var im = 0; im < vDataItem.Rows[p].ItemModifier.length; im++) {
                                                    var mod = vDataItem.Rows[p].ItemModifier[im];
                                                    var unitprice, subtotal;

                                                    if (CStr2(mod.OrderedQty) != '') { // for Inventory Affecting Modifiers                                                                        
                                                        WindowsPrinter('* ' + CDec2(mod.OrderedQty) + ' ' + mod.Description + ' / ' + $.trim(strUnitMeasure), [0, 0, 0, 0, 0, 36]);
                                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, '* ' + CDec2(mod.OrderedQty) + ' ' + mod.Description + ' / ' + $.trim(strUnitMeasure), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                        //strPrint = strPrint + strTemp;
                                                    }

                                                    if (CStr2(mod.OrderedQty) == '') { // for Non-Inventory Affecting Modifiers
                                                        WindowsPrinter('* ' + mod.Description, [0, 0, 0, 0, 0, 36]);
                                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, '* ' + mod.Description, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                        //strPrint = strPrint + strTemp;
                                                    }
                                                }
                                            }
                                        }

                                        WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);

                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                        //strPrint = strPrint + strTemp;
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        intLimitPerBatchPrint = intLimitPerBatchPrint + 1;

                                        if (intLimitPerBatchPrint >= 100 && vDataItem.Rows.length > 100) {

                                            PrintText(strPrinterIP);
                                            //ClearCommandBuffer();

                                            //New set of text to print because it causes error when you print directly all items at once.
                                            NewPrintBuilder();
                                            intLimitPerBatchPrint = 0;
                                        }

                                    } // END for (var p = 0; p <= vDataItem.Rows.length - 1; p++) {

                                    //Get Footer Amounts
                                    if (vDataFooter.Rows.length > 0) {
                                        var strSKUCount = SetPrefixPostfix(AccountingFormat((vDataFooter.Rows[0].SKUCount || '0'), 0), intFAFCTotalLenght - 14, ' ', 0);

                                        WindowsPrinter('   Item Total:  ---->', [0, 0, 0, 0, 0, 36], AccountingFormat(strSKUCount), [0, 0, 0, 0, 1, 36]);

                                        //strPrint = '';
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SKU Count: ', 12, strSKUCount, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;

                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    }
                                }

                                WindowsPrinter("address", [0, 0, 0, 0, 0, 0], strPrinterIP, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);

                                if (strPrinterName.length == 0) {
                                    WindowsPrinter("printerName", [0, 0, 0, 0, 0, 0], strPrinterName, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                }

                                WindowsPrinter("startPrint", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);

                                //paper cut
                                //ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                //PrintText(strPrinterIP);
                                //ClearCommandBuffer();

                                setTimeout(VerifyDispatchSlipPrintReceipt(pInvoiceID), 2000);
                            }
                        }
                    }
                    else {
                        NewMessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                            "Printer",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                        );
                    }
                }

                $('#btnAddTip10').live('click', function (e) {
                    e.preventDefault();
                    $('#txtAddTip').val('');
                    saveTip(10);

                });

                $('#btnAddTip20').live('click', function (e) {
                    e.preventDefault();
                    $('#txtAddTip').val('');
                    saveTip(20);

                });

                $('#btnAddTip50').live('click', function (e) {
                    e.preventDefault();
                    $('#txtAddTip').val('');
                    saveTip(50);

                });

                $('#btnAddTip100').live('click', function (e) {
                    e.preventDefault();
                    $('#txtAddTip').val('');
                    saveTip(100);


                });



                $('#txtAddTip').live('change', function (e) {
                    e.preventDefault();
                    saveTip($(this).val());
                });




                function saveTip(tipValue) {

                    var pDataCharges = [];

                    pDataCharges[0] = {
                        ChargeType: 'Tip',
                        ChargeAmount: 0,
                        TipAmount: tipValue,
                        TaxAmount: 0,
                        Remarks: ''
                    };

                    var pData = {
                        InvoiceID: sessionStorage.getItem('CartOrderID'),
                        SalesPaymentChargeList: pDataCharges
                    };


                    if (SalesPayment_offline().SaveBillingCharges(pData) == 'ok') {

                        $("#lblTipBilling").text(AccountingFormat(tipValue));
                        $("#lblSalesPayTip").text(AccountingFormat(tipValue));
                        RefreshTotalDiscountBilling(window.pageDiscounts.RefreshSalesPaymentTotalBilling());

                    }


                }

                ////////////////////////////////
                //DIAGNOSE PRINTER - janine jan 30 2015
                if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall') {
                    function DisplayPrinterList(vRows) {

                        $('#selOtherPrinter').empty();

                        $('#selOtherPrinter').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOtherPrinter').append(new Option(vRows[p].PrinterName + ' - ' + vRows[p].PrinterIPAddress, vRows[p].ID + '_' + vRows[p].PrinterIPAddress, false, false));
                        }

                        $('#selOtherPrinter').selectmenu('refresh');
                    }

                    $('#selOtherPrinter').change(function (e) {
                        e.preventDefault();

                        newPrinterFowstID = '';
                        newPrinterIP = '';

                        if ($(this).val() != '') {
                            var vPrinter = $(this).val().split('_'); //after the underscore, ip address. first, id
                            newPrinterFowstID = vPrinter[0];
                            newPrinterIP = vPrinter[1];

                            PrintBilling(sessionStorage.getItem('CartOrderID'));
                        }

                        //                    $("#selOtherPrinter option[value='" + $(this).val() + "']").attr("selected", "selected");
                        //                    $(this).selectmenu('refresh');
                    });
                    $('#btnReprint').click(function (e) {
                        e.preventDefault();


                        PrintBilling(sessionStorage.getItem('CartOrderID'));

                    });
                    $('#btnPrintOther').click(function (e) {
                        e.preventDefault();

                        $.mobile.changePage('#pageOtherPrinter');
                        $('#txtNewPrinterName').val('');
                        $('#txtNewPrinterIP').val('');

                        var result = SalesPayment_offline().getFowstPrinterList(sessionStorage.FowstID);
                        if (result.Rows != undefined) {
                            DisplayPrinterList(result.Rows);
                        }
                    });
                    $('#btnSavePrint').click(function (e) {
                        e.preventDefault();

                        if ($.trim($("#txtNewPrinterName").val()) == "") {
                            NewMessageBox("Please enter printer name.",
                "Printer Name",
                [
                    { Text: 'Ok' }
                ]
            );
                            return;
                        }
                        if ($.trim($("#txtNewPrinterIP").val()) == "") {
                            NewMessageBox("Please enter printer IP Address.",
                "Printer IP Address",
                [
                    { Text: 'Ok' }
                ]
            );
                            return;
                        } else {

                            if (!validateIP($.trim($("#txtNewPrinterIP").val()))) {
                                NewMessageBox("Please enter a valid IP address.",
                "Printer IP Address",
                [
                    {
                        Text: 'Ok'
                    }
                ]
            );
                                return;

                            }
                        }




                        //POS PRINTER ID
                        newPrinterFowstID = '';
                        newPrinterIP = '';

                        var pData = {
                            FowstID: sessionStorage.FowstID,
                            PrinterID: 'E4D81E52-3A7E-4D2A-A4F2-0098DC19C5C6',
                            PrinterName: $.trim($('#txtNewPrinterName').val()),
                            PrinterIPAddress: $.trim($('#txtNewPrinterIP').val())
                        };

                        var vDataName = SalesPayment_offline().CheckUniquePrinterName(pData);
                        if (vDataName.Rows.length > 0) {
                            NewMessageBox('There is already an existing record with the same printer name.', 'Printer Name');
                            return;
                        } else {
                            var vDataIP = SalesPayment_offline().CheckUniquePrinterIPAddress(pData);
                            if (vDataIP.Rows.length > 0) {
                                NewMessageBox('There is already an existing record with the same IP address.', 'Printer IP Address');
                                return;
                            } else {
                                var result = SalesPayment_offline().SaveFOWSTPrinterIP(pData);

                                var pInvoiceID = sessionStorage.getItem('CartOrderID');
                                if (result != '') {
                                    newPrinterFowstID = result;
                                    newPrinterIP = $.trim($("#txtNewPrinterIP").val());
                                    PrintBilling(pInvoiceID);
                                }
                            }
                        }
                    });



                    $('#lnkInstruction').click(function (e) {
                        e.preventDefault();

                        $('#popupIPAddressInstruction').popup('open');


                    });
                }

                var printAttempt = 1;
                $('#btnPrintYesBill').click(function (e) {
                    e.preventDefault();
                    if (sessionStorage.PrintOption == 'orderslip') {
                        TagOrderSlipItemsAsPrinted();
                    }
                    sessionStorage.PrintOption = ''; // clear 
                    if (CStr2(newPrinterFowstID) != '' && CStr2(originalDevFowstPrinterID) != '') {
                        var result = SalesPayment_offline().AssignFowstPrinterIPAsDefault(newPrinterFowstID, originalDevFowstPrinterID);

                        if (result == 'ok') {
                            Android.Toast(CStr2(newPrinterIP) + ' has been set as the new default IP address of this device.');
                            newPrinterFowstID = '';
                            newPrinterIP = '';


                        }
                    }

                    if (sessionStorage.CurrentPage == 'discount')
                        $.mobile.changePage('#PreviewBill');
                    else {
                        if (GetUrlParameters().TableNo != undefined) {
                            mPrePos.UpdateOrderedQuantity();
                            //$('#pageMain').pageshow()
                            printAttempt = 1;
                            return;
                        }
                        if (CStr2(sessionStorage.IsEnableDiningMode) == 'true') {
                            $.mobile.changePage('');
                            location = MergeUrlParameters({
                                OrgID: sessionStorage["OrgID"],
                                mode: 'pos',
                                IsMobile: IsMobile ? 1 : 0
                            }, '/TableView/NewTable');

                        } else {
                            proj.SalesMenu.goToSalesMenu();
                        }
                    }
                });

                $('#btnPrintNoBill').click(function (e) {
                    e.preventDefault();

                    //                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('POS Receipt Reprint', {
                        OnClickOKSuccess: function () {
                            if (printAttempt > 1) {
                                $.mobile.changePage('#pageDiagnosePrinter');
                            } else {
                                printAttempt = printAttempt + 1;
                                //                                $.mobile.changePage(window.pageSourcePage);                                            //
                                if (sessionStorage.PrintOption == 'billing') {
                                    proj.SalesMenu.PrintBilling(sessionStorage.getItem('CartOrderID'));
                                }
                                else if (sessionStorage.PrintOption == 'orderslip') {
                                    proj.SalesMenu.PrintBilling(sessionStorage.getItem('CartOrderID'), {
                                        PrintAsOrderSlip: true
                                    });
                                }
                                else if (sessionStorage.PrintOption == 'billingorderslip') {
                                    proj.SalesMenu.PrintBilling(sessionStorage.getItem('CartOrderID'));
                                    proj.SalesMenu.PrintBilling(sessionStorage.getItem('CartOrderID'), {
                                        PrintAsOrderSlip: true
                                    });
                                }
                                else {
                                    PrintBilling(theInvoiceID);
                                }
                            }

                        }
                    });
                });

                $('#btnPrintYesDispatchSlip').click(function (e) {
                    e.preventDefault();
                    if ((sessionStorage.PrintOption == 'orderslip')
                        || mPrintedOrderSlip) {

                        TagOrderSlipItemsAsPrinted();
                    }
                    $('#pageMain').pageshow()
                });

                $('#btnPrintNoDispatchSlip').click(function (e) {
                    e.preventDefault();

                    PromptUserAccess('POS Receipt Reprint', {
                        OnClickOKSuccess: function () {
                            if (printAttempt > 1) {
                                $.mobile.changePage('#pageDiagnosePrinter');
                            } else {
                                printAttempt = printAttempt + 1;
                                PrintDispatchSlip(theInvoiceID, {
                                    ByCategory: false,
                                    DtPrinter: null
                                });
                            }
                        }
                    });
                });

                function validateIP(string) {
                    var re = /^(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))\.(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))\.(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))\.(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))$/;

                    return re.test(string);
                }

                function TagOrderSlipItemsAsPrinted() {
                    var pSql = '';
                    var pIU;

                    pIU = new InsertUpdate('TransInvoiceItems');
                    pIU.Add('IsOrderSlipPrinted', 1);
                    pIU.Add('QuantityPrinted', 'Quantity');
                    pSql = pIU.GetUpdate('InvoiceID = ' + ReadyString(sessionStorage.getItem('CartOrderID')));
                    Sql.Execute(pSql);
                }
                function ShowCharges() {
                    var total = 0.00;
                    var otherTax = 0.00;
                    var charges = 0.00;
                    var result = SalesPayment_offline().GetBillCharges(sessionStorage.getItem('CartOrderID'));

                    if (result.Rows != undefined) {


                        for (var i = 0; i < result.Rows.length; i++) {
                            otherTax += parseFloat(result.Rows[i].LocalTaxAmount);
                            total += parseFloat(result.Rows[i].ChargeAmount) + parseFloat(result.Rows[i].LocalTaxAmount) + parseFloat(result.Rows[i].TipAmount);
                            charges += parseFloat(result.Rows[i].ChargeAmount) + parseFloat(result.Rows[i].TipAmount);
                        }


                        DisplayChargesListBilling(result.Rows);

                    }
                    $("#lblSalesPayChargesBilling").text(AccountingFormat(charges));
                    $("#lblBillTotalChargesBilling").text(AccountingFormat(total));
                    $("#lblSalesPayOtherTaxBilling").text(AccountingFormat(otherTax));
                }
                /////////end of billing functions

                //end of additional
                ///////////////////////////////
                //////////////////////////////////////////
                //////////////////////////////////////////
                //////////////////////////////////////////
                //END OF ADDITIONAL BY JANINE 
                ///////////////////////////////////////////
                //////////////////////////////////////////
                //////////////////////////////////////////



                function WriteLastFowst() {

                    debugger; // this code should be replaced with the one from PrePos

                    if (IsMobile) {  // condition added by Arvin
                        try {
                            Transfers.WriteFile('LastFowst.txt', null, sessionStorage.FowstName);
                            Transfers.WriteFile('LastFowstID.txt', null, sessionStorage.FowstID);
                        } catch (e) {
                            console.error('jsVQWQHN6UG3! ' + e);
                        }
                    }
                } // END function WriteLastFowst() {


                window.WindowsPrinter = function (cols1, args1, cols2, args2, cols3, args3) {

                    //cols1 = command / text
                    //args1 = position of cols 1

                    //cols2 = command / text
                    //args2 = position of cols 2

                    //cols3 = command / text
                    //args3 = position of cols 3

                    if ((cols3 == null || cols3 == undefined) && (cols2 != null || cols2 != undefined)) {
                        // 2 rows will be displayed/printed
                        Android.WindowsPrinting(
                                                {
                                                    "data": [
                                                                { "cols1": cols1 },
                                                                { "args1": [args1[0].toString(), args1[1].toString(), args1[2].toString(), args1[3].toString(), args1[4].toString(), args1[5].toString()] },
                                                                { "cols2": cols2 },
                                                                { "args2": [args2[0].toString(), args2[1].toString(), args2[2].toString(), args2[3].toString(), args2[4].toString(), args2[5].toString()] }
                                                    ]
                                                }
                        );
                    } else if ((cols3 == undefined || cols3 == null) && (cols2 == null || cols2 == undefined)) {
                        // 1 row will be displayed/printed
                        Android.WindowsPrinting(
                                                {
                                                    "data": [
                                                                { "cols1": cols1 },
                                                                { "args1": [args1[0].toString(), args1[1].toString(), args1[2].toString(), args1[3].toString(), args1[4].toString(), args1[5].toString()] }
                                                    ]
                                                }
                        );
                    } else if (cols1 == "newLine") {
                        Android.WindowsPrinting(
                                                {
                                                    "data": [
                                                                { "newLine": "newLine" }
                                                    ]
                                                }
                        );
                    } else if (cols1 == "startPrint") {
                        Android.WindowsPrinting(
                                                {
                                                    "data": [
                                                                { "startPrint": "startPrint" }
                                                    ]
                                                }
                        );
                    } else if (cols1 == "address") {
                        Android.WindowsPrinting(
                                                {
                                                    "data": [
                                                                { "address": cols2 }
                                                    ]
                                                }
                        );
                    } else if (cols1 == "printerName") {
                        Android.WindowsPrinting(
                                                {
                                                    "data": [
                                                                { "printerName": cols2 }
                                                    ]
                                                }
                        );
                    } else {
                        // 3 row will be displayed/printed
                        Android.WindowsPrinting(
                                                {
                                                    "data": [
                                                                { "cols1": cols1 },
                                                                { "args1": [args1[0].toString(), args1[1].toString(), args1[2].toString(), args1[3].toString(), args1[4].toString(), args1[5].toString()] },
                                                                { "cols2": cols2 },
                                                                { "args2": [args2[0].toString(), args2[1].toString(), args2[2].toString(), args2[3].toString(), args2[4].toString(), args2[5].toString()] },
                                                                { "cols3": cols3 },
                                                                { "args3": [args3[0].toString(), args3[1].toString(), args3[2].toString(), args3[3].toString(), args3[4].toString(), args3[5].toString()] }
                                                    ]
                                                }
                        );
                    }
                }

                if (CBool(GetUrlParameters().HotButtons))
                    goToHotButtonPage();

                function ShowTransPimSalesCaching() {
                    var pDate = new Date() - 7 * 24 * 60 * 60 * 1000;
                    pDate = new Date(pDate);
                    var pCount = window.Business.TransInventoryLedger.TransPimSalesCachingCount(sessionStorage.FowstID, pDate);
                    if (pCount == 0)
                        DownloadPimSalesCachingToJson();
                    else
                        BrowseTransPimSalesCachingCount();
                } // END ShowTransPimSalesCaching

                function BrowseTransPimSalesCachingCount() {
                    BrowsePopup('TransPimSalesCaching', 'Sales And BOM Quantity', [
                    {
                        //Offline: true,
                        AutoSearch: true,
                        OfflineBrowser: OfflineBrowser.TransPimSalesCaching,
                        StyleFormat: 'table',
                        DisplayText: [
                            {
                                Label: 'Date Interval Start',
                                Column: function (e) {
                                    return $.format.date(new Date(e.DateIntervalStart), 'MMM dd, yyyy hh:mm a');
                                }
                            },
                            {
                                Label: 'Date Interval End',
                                Column: function (e) {
                                    return $.format.date(new Date(e.DateIntervalEnd), 'MMM dd, yyyy hh:mm a');
                                }
                            },
                            {
                                Label: 'FOWST',
                                Column: 'FowstName'
                            },
                            {
                                Label: 'Product No.',
                                Column: 'ProductNo'
                            },
                            {
                                Label: 'Product Name.',
                                Column: 'ProductName'
                            },
                            {
                                Label: 'Sales Qty.',
                                Column: function (e) {
                                    return AccountingFormat(e.SalesQty);
                                }
                            },
                            {
                                Label: 'Item Modifier Qty.',
                                Column: function (e) {
                                    return AccountingFormat(e.ItemModifierQty);
                                }
                            },
                            {
                                Label: 'Used As BOM',
                                Column: function (e) {
                                    return AccountingFormat(e.BomQty);
                                }
                            }
                        ],
                        Click: function (e) {
                            var IsDraft;
                            if (!IsGuid(sessionStorage.CustomerOrganizationID))
                                sessionStorage.CustomerOrganizationID = e.CustomerID;

                            if (e.IsDraft == undefined) {
                                IsDraft = 1;
                            }
                            else {
                                IsDraft = e.IsDraft;
                            }

                            arrPODetails = e;
                            sessionStorage.IsDraft = CInt2(IsDraft);

                            if (e.UserID != e.CurrentUserID) {
                                NewMessageBox("Warning : This cart was created by " + e.EmployeeFullName + ". Do you want to continue?",
                                'Recall Cart',
                                [
                                    {
                                        Text: 'Yes',
                                        Click: function () {
                                            LoadData();
                                        }
                                    }
                                ,
                                    {
                                        Text: 'No',
                                        Click: function () {
                                            GotoFirstPage();
                                            return;
                                        }
                                    }
                                ]
                            );
                            } else { // ELSE if (e.UserID != e.CurrentUserID) {
                                LoadData();
                            } // END if (e.UserID != e.CurrentUserID) {

                            function LoadData() {
                                if (vParams.SaveAsNew) {
                                    sessionStorage.removeItem('CartOrderID');
                                } else {
                                    sessionStorage.setItem('CartOrderID', e.ID);

                                    //Added By Francis to load Customer Info
                                    sessionStorage.CustomerOrganizationName = e.CompanyName;

                                    //additional by janine oct 27 for demographics
                                    sessionStorage.DeliveryMethod = e.DeliveryMethod;
                                    sessionStorage.OrderMethod = e.OrderMethod;
                                    sessionStorage.Gender = e.CustomerDemographicGender;
                                    sessionStorage.Age = e.CustomerDemographicAge;
                                    sessionStorage.TotalNumberOfCustomer = e.TotalNumberOfCustomer;
                                }

                                //
                                //put condition if table no or gas pump no if GP or TBL

                                if (CStr2(e.TableNo) != '') {
                                    var prefix = e.TableNo.split('_');
                                    if (prefix[0] == 'GP')
                                        sessionStorage.GasPumpNo = prefix[1];
                                    else
                                        sessionStorage.CustomerTableNo = prefix[1];
                                } else {
                                    sessionStorage.GasPumpNo = '';
                                    sessionStorage.CustomerTableNo = '';
                                }
                                //                            pageCustomerDetails.setCustomerInfo();
                                $('#btnClient').data('id', e.CustomerID);


                                //additional by janine sept 5 2014 
                                if (proj.PosMode) { // && CDec2(e.SalesOrderNo) > 0
                                    UpdateOrderedQuantity();
                                } else {
                                    $('#lstROPOItems').empty();

                                    isRecallCart = true;
                                    LoadCart(false, {
                                        SaveAsNew: vParams.SaveAsNew,
                                        ID: e.ID
                                    });

                                    if (CBool(sessionStorage.IsDraft)) {
                                        GotoFirstPage();
                                    }
                                }
                                //end of additional
                            } // END function LoadData() {
                        }
                    }
                    ], {
                        params_here: null
                    })
                }

                function DownloadPimSalesCachingToJson() {
                    AjaxSend({
                        url: '/InventoryLedger/ExportPimSalesCachingToJson',
                        data: {
                            
                        },
                        success: function(vData) {
                            if (vData.Error)
                                NewMessageBox(vData.Error);
                            else if (vData.Results == 'ok') {
                                ImportFromDataSet(vData);
                                NewMessageBox('Cached data was successfully loaded.', 'Success');
                            } else
                                NewMessageBox('Unexpected result. ref: jsLLUCW8WFRZ!');
                        }
                    });
                } // END function DownloadXmlZip() {

                function ImportFromDataSet(vDS) {
                    for (var pDtName in vDS) {
                        if (vDS.hasOwnProperty(pDtName) == false)
                            continue;
                        var pDT = vDS[pDtName];
                        if (pDT.Rows == null)
                            continue;
                        for (var pDrIndex = 0; pDrIndex <= pDT.Rows.length - 1; pDrIndex++) {
                            var pDR = pDT.Rows[pDrIndex];
                            var pSql = 'SELECT COUNT(*)\n' +
                                ' FROM ' + pDtName + '\n' +
                                ' WHERE ID = ' + ReadyString(pDR.ID);
                            var pCount = Sql.ExecuteScalar(pSql);
                            if (CInt2(pCount) > 0)
                                continue;
                            var pIU = new InsertUpdate(pDtName);
                            var pHasDateImported = false;
                            pIU.GetItems(pDR, function(vArgs) {
                                if ((vArgs.FieldName.toLowerCase() == 'dateimported')
                                    && (!vArgs.Cancel))
                                    pHasDateImported = true;
                            });
                            if (!pHasDateImported
                                && pDR.hasOwnProperty('DateImported'))
                                pIU.Add('DateImported', proj.SqliteNow);
                            pSql = pIU.GetInsert();
                            Sql.ExecuteSql(pSql);
                        }
                    }
                }

                mDeferred.resolve();
                //Android.AddLog('salesmenu.js loaded');

            }); // $(function() {
        }); // require SalesMenu_offline.js
        return mDeferred.promise();
    });                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        // require

//Android.Download({
//    Source: 'http://thepapertree.com.ph/data_ba_ron?akong_query_string',
//    Destination: '/dri_bar_ron/kamo_na.zip',
//    Success: function() {
//        Android.Extract(this.Destination);
//        var pString = Android.ReadFile(this.Destination + '/sulod_sa_zip.json');
//    },
//    Error: function() {
//    },
//    Complete: function() {
//    }
//});