require(['common',
    'promise!/Scripts3/VendingMachine_Offline.js',
    'promise!/Scripts3/SalesMenu_Offline.js',
    'promise!/Scripts3/Stock_Offline.js',
    'promise!/Scripts3/Bs_SalesReturns.js',
    '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc=',
    '/Scripts/sources/People/PeopleMenu_offline.js?crc='
], function () {

    require(['/Scripts/sources/VendingMachine/Passcode.js?v=' + assetversion,
        '/Scripts/sources/VendingMachine/timer.js?v=' + assetversion])

    var selectedProduct = '';
    var arrOrderItems = [];
    var allSKU = []
    var sourceimg = '/Content/MobizImagePIM/' + sessionStorage.OrgID;
    var imgpath = sourceimg + '/Big/';
    var requestedAction;
    var totalAmountToPay = 0;
    var totalCashInserted = 0;
    var ctrDispenseItem = 0;
    var ctrDispenseGrain = 0;
    var ctrDispenseLiquid = 0;
    var arrCashInsertedBreakdown = [];
    var screentimeout;
    var isUserInactive = false;
    var optionpress;
    window.coin = 0;
    var isFromCart = false;
    var arrGrain = [];
    var arrLiquid = [];
    var arrGoods = [];
    var invoiceid = '';
    var inputDelay = false;
    var listenerInactivity;
    var timeoutCallback, countdown;;
    var timeout = 30000; //in ms
    var ctrAccessSettings = 0;
    var adminOption = null;
    var optCode = '';
    var slides = document.querySelector('.slides');
    var slideItems = document.querySelectorAll('.slide');
    var currentSlide = 0;
    var isTransitioning = false;
    var timerGuides;
    var dropAttempt = 1;
    var dropGrainAttempt = 1;
    var dropLiquidAttempt = 1;
    var paymentChangeAttempt = 1;
    var arrDroppedItem = [];
    var curDispenseItem = null;
    var arrFailedDropItem = [];
    var franchisegroupid;
    var IsPaymentChangeARefund = 0;
    var actualTotalDispensedChange = 0;
    var isCancelDuringPayment = false;
    var actualWeight = 0;
    var ctrTestAllMotor = 1;
    var listener_opts; 
    var arrDeviceList = [];
    var menu_ItemSelection;
    var DispenseLiquidStepNo = 1;
    var remainingVolumeQty = 0;
    var latestVolumeQty = 0;
    sessionStorage.LogID = NewID();
    var DispenseLiquidState = '';
    var isStatusBottleResponded = false;
    var htmlInAppSettings = '';
    var isUserActive = false;

    function AddToCart(qty, availableQty) {
        var isExist = false, curIndex = 0;
        //var availableQty = CDec2($('.lblProductAvailableQty:eq(0)').text());
        var orderQty = (CBool(selectedProduct.IsUseUOM) ? 1 : qty);
        var selectedUom;

        if (CBool(selectedProduct.IsUseUOM)) {
            selectedUom = $('#listUom').find('> div .btnUom.active:eq(0)');
            if (selectedUom.length == 0) {
                $('#divErrMsgProductInfo h5').text('Please choose a quantity. Just press a number from 1 to ' + $('#listUom > div').length +' to select.');
                $('#divErrMsgProductInfo').show();
                return false;
            }

            selectedProduct.DiscountedPrice = selectedUom.data('data').DiscountedPrice;
            selectedProduct.Quantity = selectedUom.data('data').Quantity;
            selectedProduct.UnitMeasure = selectedUom.data('data').UnitMeasure;
            selectedProduct.UnitMeasureID = selectedUom.data('data').UnitMeasureID;

            //one variety of rice lang sa per transaction, puhon na ang multiple daw -- by Arnie Jan 13, 2025
            //e-uncomment lang ni if pwede na multiple rice/transaction
            var tempOrderItems = JSON.parse(JSON.stringify(arrOrderItems));
            for (var i in tempOrderItems) {
                if (CBool(tempOrderItems[i].IsUseUOM)) {
                    $('#' + tempOrderItems[i].ProductID + ' > .product_container').removeClass('selectedSku').find('.sku_badge_qty').remove();
                    arrOrderItems.splice(i, 1);
                }
            }
            // end            
        } else {
            if (orderQty > availableQty) {
                //$('#txtOrderQty').addClass('is-invalid').val('');
                //$('#divErrMsgProductInfo h5').text('The quantity entered exceeds what is available.');
                //$('#divErrMsgProductInfo').show();
                NewMessageBox('The quantity entered exceeds what is available.')
                return false;
            }

            if (orderQty == 0) {
                //$('#txtOrderQty').addClass('is-invalid').val('');
                //$('#divErrMsgProductInfo h5').text('Please enter your desired quantity.');
                //$('#divErrMsgProductInfo').show();
                //RemoveItem();
                return false;
            }
        }
        
        $.grep(arrOrderItems, function (e, index) {
            if (e.ProductID == selectedProduct.ProductID) {
                isExist = true;
                curIndex = index;
                return false; // stop
            }
        })

        selectedProduct.OrderQty = orderQty;
        console.log(selectedProduct)
        if (isExist) {
            if (CBool(selectedProduct.IsUseUOM)) {
                arrOrderItems[curIndex].DiscountedPrice = selectedUom.data('data').DiscountedPrice;
                arrOrderItems[curIndex].Quantity = selectedUom.data('data').Quantity;
                arrOrderItems[curIndex].UnitMeasure = selectedUom.data('data').UnitMeasure;
                arrOrderItems[curIndex].UnitMeasureID = selectedUom.data('data').UnitMeasureID;
            }
            arrOrderItems[curIndex].OrderQty = orderQty;
        } else {
            arrOrderItems.push(selectedProduct);
        }

        if (!isFromCart) {
            //SelectSKU(selectedProduct);
            VendingMachine.AddLog('Product Info', 'Adding an item to the cart');            
            
            if (CBool(selectedProduct.IsUseUOM) && IsGuid(selectedProduct.RecommendedSKU)) {
                var tempOrderItems = JSON.parse(JSON.stringify(arrOrderItems));
                var isRecommenedItemAdded = tempOrderItems.find(item => item.ProductID == selectedProduct.RecommendedSKU);
                if (typeof isRecommenedItemAdded == 'undefined') {
                    //if not exist then recommend the item
                    var recomSKU = allSKU.find(item => item.ProductID == selectedProduct.RecommendedSKU);
                    var page_additonalitem = $('#page_additonalitem');
                    $('#imgMainSKU').attr('src', imgpath + selectedProduct.FileName + '_big.jpg');
                    $('#imgAddOnSKU').attr('src', imgpath + recomSKU.FileName + '_big.jpg');
                    page_additonalitem.find('h1').html('Add a ' + CStr2(recomSKU.ProductName).toLowerCase() + ' for only P' + AccountingFormat(recomSKU.DiscountedPrice) + '?');
                    page_additonalitem.pageshow();

                    $('#btnUseOwn').unbind().click(function () {
                        $('.btnViewOrder').click();
                    })

                    $('#btnAddRecomSKU').unbind().click(function () {
                        menu_qtyselection('+', recomSKU);
                        $('.btnViewOrder').click();
                    })
                    return;
                }
            }

            $('.btnViewOrder').click();
            //$('#popItemAdded').modal('show');
            //$('.page_menu').pageshow();
            //setTimeout(function () {
            //    $('#popItemAdded').modal('hide');
            //    var total = 0;
            //    $.grep(arrOrderItems, function (d) {
            //        total += CInt2(d.OrderQty) * CDec2(d.DiscountedPrice);
            //    })
            //    $('#lblOrderQty').text(AccountingFormat(total));
            //    $('#txtSelectedItemNo').focus();
            //}, 1000)
        } else {
            VendingMachine.AddLog('Product Info', 'Change item quantity');
            UpdateCartInfo();
        }        
        return true;
        //$('#popProductInfo').on('hidden.bs.modal', function () {
        //    $('#popItemAdded').modal('show');
        //    SelectSKU(selectedProduct);
        //    setTimeout(function () {
        //        $('#popItemAdded').modal('hide');
        //        $('#lblOrderQty').text(arrOrderItems.length + ' item(s)');
        //        //$('#listofskus').parent().scrollTop(0);
        //        $('#page_itemadded .product_itemadded').removeClass('show');
        //        $('#popProductInfo').unbind('hidden.bs.modal');
        //        //$('.page_menu').pageshow();
        //    }, 2000)
        //}).modal('hide');
        //$(".product_itemadded").delay(200).each(function (i) {
        //    $(this).addClass("show");
        //})

        console.log(arrOrderItems);
        //NewMessageBox('Item successfully added to your cart.', '', [
        //    {
        //        Text: 'Order new item',
        //        Click: function () {
        //            $('.page_menu').pageshow();
        //        }
        //    },
        //    {
        //        Text: 'View Order',
        //        Click: function () {
        //            ViewOrder();
        //        }
        //    }
        //])
    }

    function SelectSKU(sku) {
        var pc = $('#' + sku.ProductID + ' > .product_container');

        //pc.find('.sku_badge_qty').remove();

        if (sku.OrderQty > 0) {
            pc.addClass('selectedSku');
            //pc.append('<span class="sku_badge_qty position-absolute top-0 start-100 translate-middle badge rounded-pill bgvm-primary" style="font-size: 1.50em;">' + sku.OrderQty + '</span>');
        } else {
            pc.removeClass('selectedSku');
        }
    }

    function ModifyProduct(data) {
        $('#lblProductName').text(data.ProductName);
        $('#imgProductImage').attr('src', imgpath + data.FileName + '_big.jpg');

        if (CBool(data.IsUseUOM)) {
            var listUom = $('#listUom').empty();
            var uom = data.ListOfUom;
            var ctrBtn = 1;
            console.log(uom)
            //console.log(JSON.stringify(uom))
            for (var i = uom.length - 1; i <= uom.length - 1; i--) {
                var vUom = uom[i];
                if (vUom == undefined) break;
                vUom.DiscountedPrice = Business.LibraryPIMProduct.RoundPrice(vUom.DiscountedPrice);
                if (vUom.Quantity == 1 && !CBool(data.IsDoNotIncludeBaseUOM)) {
                    listUom.append($('<div class="row mb-3">')
                        .attr('uomno', ctrBtn)
                        .attr('umid', vUom.UnitMeasureID)
                        .data('data', vUom)
                        .append($('<div class="col">')
                            .append($('<div class="bg-white btnUom mb-3 w-100 position-relative h-100 border bordervm-secondary rounded-2 p-4 text-center text-secondary d-flex flex-column justify-content-center align-items-center">').data('data', vUom).html('<span class="position-absolute" style="left:77px;"></span> ' + vUom.Quantity + ' ' + vUom.BaseUnit + ' = ' + AccountingFormat(vUom.DiscountedPrice)))
                        )
                        .click(function (e) {

                        })
                    )
                    ctrBtn += 1;
                }

                if (CDec2(data.RemainingQty) > CDec2(vUom.Quantity) && CDec2(vUom.Quantity) > 1 && CDec2(vUom.Quantity) <= 1000) {
                    listUom.append($('<div class="row mb-3">')
                        .attr('uomno', ctrBtn)
                        .attr('umid', vUom.UnitMeasureID)
                        .data('data', vUom)
                        .append($('<div class="col">')
                            .append($('<div class="bg-white btnUom mb-3 w-100 position-relative h-100 border rounded-2 p-2 text-center text-secondary d-flex flex-column justify-content-center align-items-center">').data('data', vUom).html('<span class="position-absolute" style="left:77px;"></span> ' + vUom.UnitMeasure + ' = ' + AccountingFormat(vUom.DiscountedPrice)))
                        )
                        .click(function (e) {
                            $('#listUom').find('.btnUom').removeClass('active');
                            $(this).find('.btnUom').addClass('active');
                        })
                    )
                    ctrBtn += 1;
                }                
            }
            $('.btnUom:eq(0)').addClass('active');
            //$('#listUom .btnUom').removeClass('active');
            //$('#listUom').find('> div:last-child .btnUom').addClass('active')
            $('#divUomSelection').pageshow();

            $.grep(arrOrderItems, function (e, index) {
                if (e.ProductID == selectedProduct.ProductID) {
                    $('#listUom .btnUom').removeClass('light-beam active');
                    $('div[umid="' + e.UnitMeasureID + '"]').find('.btnUom').addClass('active');
                    return false; // stop
                }
            })
        } else {
            $('#lblProductPrice').text(AccountingFormat(data.DiscountedPrice) + '/' + data.UnitMeasure);
            $('#lblProductDescription').text(data.Description);
            
            $('#txtOrderQty').val(1);
            //OrderQty('+', $('#txtOrderQty'), 'product_page');

            $.grep(arrOrderItems, function (e, index) {
                if (e.ProductID == selectedProduct.ProductID) {
                    $('#txtOrderQty').val(e.OrderQty);
                    return false; // stop
                }
            })
            $('#divDefaultSelection').pageshow();
        }        
        
        //$('#popProductInfo').modal('show');
        $('.lblProductAvailableQty').text((CBool(data.IsUseUOM) ? (CInt2(data.RemainingQty) / data.LargestUom.Quantity) + ' ' + ((data.LargestUom.UnitMeasure).split('-')[1] || data.LargestUom.UnitMeasure) : data.RemainingQty));
        $('#divErrMsgProductInfo, .lblToRemoveItem').hide();
        $('.page_productinfo').pageshow();
        //$('#txtOrderQty').focus();
    }

    function SelectUom() {

    }

    function RemoveItem() {
        var isExist = false, curIndex = null;

        $.grep(arrOrderItems, function (e, index) {
            if (e.ProductID == selectedProduct.ProductID) {
                isExist = true;
                curIndex = index;
                return false; // stop
            }
        })

        if (isExist) {
            var skuContainer = $('#' + selectedProduct.ProductID + ' > .product_container').removeClass('selectedSku');
            skuContainer.find('.sku_badge_qty').addClass('d-none');
            skuContainer.find('.txtLineItemQty').val(0);
            arrOrderItems.splice(curIndex, 1);            
            UpdateCartInfo();
            return;
            dis.parents().closest('.product_cart').parent().fadeOut('slow');
            setTimeout(function () {
                dis.parents().closest('.product_cart').parent().remove();
                var total = 0;
                $('.lblLineItemTotal').each(function () {
                    total += CDec2($(this).text())
                })
                $('#lblOrderQty, .txtCartTotal').text(total > 0 ? AccountingFormat(total) : '0.00');
                //$('#lblOrderQty').text(total > 0 ? arrOrderItems.length + ' item(s)' : '');
            }, 1000)
        }
    }
        
    function DetectInactivity(seconds) {
        if (typeof countdown != 'undefined') clearInterval(countdown);
        if (typeof listenerInactivity != 'undefined') clearInterval(listenerInactivity);

        if ($('#page_screentimeout').css('display') != 'block' && $('#page_settings_pin').css('display') != 'block') {
            listenerInactivity = setInterval(function () {
                clearInterval(listenerInactivity)
                ScreenTimeout(function (response) {
                    DetectInactivity();
                    if (response == 'continue') {
                        $('#page_trans').pageshow();
                    } else {
                        NewTrans();
                    }
                })
            }, timeout)
            return false;
        }

        if (isUserInactive) {
            $('#page_trans').pageshow();
            DetectInactivity();

            var pg_ads;
            var activePage = $('div[subpage="about_item"]:visible');

            if (activePage.hasClass('page_menu')) {
                pg_ads = 'menu';
            }

            if (activePage.hasClass('page_productinfo')) {
                pg_ads = 'productdetails';
            }

            if (activePage.hasClass('page_Cart')) {
                pg_ads = 'cart';
            }

            if (activePage.hasClass('page_paynow')) {
                pg_ads = 'payment';
            }
            ShowPageGuides('menu');
            return true;
        }       
    }
    
    function ScreenTimeout(callback) {
        return
        VendingMachine.Timer('#page_screentimeout', 10, {
            Done: function () {
                if (isCancelDuringPayment) {
                    $('#page_paymentcancelled').pageshow();

                    if (totalCashInserted > 0) {
                        $('#page_paymentcancelled').find('h3').append(' Don’t worry! We’ll return the money you inserted.')
                        setTimeout(function () {
                            VendingMachine.SaveCash({ CashInsertedBreakdown: arrCashInsertedBreakdown }, sessionStorage.InvoiceID, function () {
                                CashChange(totalCashInserted, 1);
                            })
                        }, 2000)
                    } else {
                        $('#page_sessionends').pageshow();
                        setTimeout(function () {
                            NewTrans();
                        }, 2000)
                    }
                } else {
                    $('#page_sessionends').pageshow();
                    setTimeout(function () {
                        NewTrans();
                    }, 2000)
                }
            },
            Extend: function () {
                $('#page_trans').pageshow();
                isUserInactive = false;
                DetectInactivity(30);                
            }
        })
        isUserInactive = true;
        $('#page_screentimeout').pageshow('show');
        return     
        var timer = CInt2(timeout) / 1000; //seconds
        $('#lblTime').text(timer + ' second' + (timer > 1 ? 's' : ''));
        timeoutCallback = callback;

        if (typeof countdown != 'undefined') clearInterval(countdown);

        countdown = setInterval(function () {
            timer -= 1;
            $('#lblTime').text(timer + ' second' + (timer > 1 ? 's' : ''));

            if (timer == 0) {
                clearInterval(countdown);
                DetectInactivity();
                isUserInactive = false;
                if (typeof callback == 'undefined') {
                    $('#page_trans').pageshow();
                } else {
                    if (isCancelDuringPayment) {
                        $('#page_paymentcancelled').pageshow();

                        if (totalCashInserted > 0) {
                            $('#page_paymentcancelled').find('h3').append(' Don’t worry! We’ll return the money you inserted.')
                            setTimeout(function () {
                                VendingMachine.SaveCash({ CashInsertedBreakdown: arrCashInsertedBreakdown }, sessionStorage.InvoiceID, function () {
                                    CashChange(totalCashInserted, 1);
                                })
                            }, 2000)
                        } else {
                            setTimeout(function () {
                                NewTrans();
                            }, 2000)
                        }
                    } else {
                        NewTrans();
                    }
                }                
            }
        }, 1000)        
    }

    window.NewTrans = NewTrans;
    function NewTrans() {
        Android.AddLog('NEW TRANSACTION')
        sessionStorage.LogID = NewID();
        sessionStorage.InvoiceID = '';
        invoiceid = '';
        selectedProduct = '';
        arrOrderItems = [];
        requestedAction;
        totalAmountToPay = 0;
        totalCashInserted = 0;
        ctrDispenseItem = 0;
        ctrDispenseGrain = 0;
        ctrDispenseLiquid = 0;
        arrCashInsertedBreakdown = [];
        arrGrain = [];
        arrLiquid = [];
        arrGoods = [];
        dropAttempt = 1;
        dropGrainAttempt = 1;
        dropLiquidAttempt = 1;
        paymentChangeAttempt = 1;
        actualTotalDispensedChange = 0;
        arrDroppedItem = [];
        arrFailedDropItem = [];
        isCancelDuringPayment = false;
        isUserInactive = false;
        actualWeight = 0;
        $('.txtLineItem').text('0');
        $('.txtCartTotal').text('P 0.00');
        LoadMenu();
        isTransitioning = false;
        slides = document.querySelector('.slidesLandscape');
        slideItems = $('.slidesLandscape > li'); // document.querySelectorAll('.ads_Landscape');
        goToSlide(0);
        $('#page_idle, #listEmptyField').pageshow();
        $('#page_cashchangecompleted').find('h3').text('Your change is ready! Please take it from the change slot!');

        if (typeof countdown != 'undefined') clearInterval(countdown);
        if (typeof listenerInactivity != 'undefined') clearInterval(listenerInactivity);
        //$('#page_trans, .page_menu').pageshow();

        setTimeout(function () {
            NewMessageBoxClose();
        }, 2000)
    }

    VendingMachine.RefreshMachineSKU = LoadMenu;
    function LoadMenu() {
        sessionStorage.HotButtonData = null; //clear this para makuha ang latest menu data;
        sessionStorage.CustomerOrganizationID = 'cd71a0f7-08a9-4601-876e-5f13c064f873'; // default Customer [CASH PAYMENT]

        var menu = VendingMachine.GetMenu();
        var listMenu = $('#listMenu').empty();
        allSKU = [];
        menu = $.merge([{ Name: 'All Items', table_name: 'group', Sku: menu.flatMap(group => JSON.parse(JSON.stringify(group.Sku))) }], menu);

        for (var x = 0; x < 1; x++) {
            for (var i = 0; i < menu.length; i++) {
                var menudata = menu[i];

                //$.grep(menudata.Sku, function (e) {
                //    if (e.VDMDispenserType == 'Spring Coil') e.VDMMotorNo = CDec2(e.VDMMotorNo) + 10;
                //})

                if (i == 0) {
                    allSKU = $.merge(allSKU, menudata.Sku);
                }
                listMenu.append($('<li style="flex:none; white-space:nowrap;">')
                    .append($('<button data-role="none" id="' + menudata.ID + '" class="btn shadow-sm ' + (i == 0 ? 'btnvm-primary' : 'btn-light') + ' textvm-primary rounded-pill border bordervm-secondary" style="min-width:200px;">')
                        .data('data', menudata)
                        .html((i == 0 ? '<span class="bi-star-fill text-warning"></span> ' : '') + menudata.Name)
                        .click(function () {
                            var dis = $(this), data = dis.data('data');

                            if (!dis.hasClass('btnvm-secondary')) {
                                LoadSku(data.Sku);
                                document.getElementById(data.ID).scrollIntoView({ behavior: "smooth", block: "center", inline: "center" });
                            }

                            listMenu.find('button.btnvm-primary').addClass('btn-light').removeClass('btnvm-primary font-weight-bold');
                            dis.removeClass('btn-light').addClass('btnvm-primary font-weight-bold');                                                                
                        })
                    )
                )
            }
        }
        console.log(JSON.parse(JSON.stringify(allSKU)))
        LoadSku(allSKU);
    }

    function LoadSku(sku) {
        var cntCol = 1, cntRowCol = 3, row = $('<div class="row g-4">'), ctrSKUNo = 1;
        var listofskus = $('#listofskus').empty();
        
        listofskus.parent().scrollTop(0);

        if ($(window).width() < 1300) {
            cntRowCol = 5;
        } else {
            cntRowCol = 5;
        }

        for (var i = 0; i < sku.length; i++) {
            var skudata = sku[i], largestuom, vdmmotors = CStr2(skudata.VDMMotorNo).split(',');

            if (CDec2(skudata.RemainingQty) == 0) continue;

            if (CBool(skudata.IsUseUOM) && skudata.LargestUom == undefined) {
                var uom = skudata.ListOfUom;

                var maxItem = null;
                var maxQuantity = 0;

                for (var u = 0; u < uom.length; u++) {
                    var quantity = CDec2(uom[u].Quantity);

                    if (quantity <= 1000 && quantity > maxQuantity) {
                        // walay labot ang more than 1000 or 1 sack
                        maxQuantity = quantity;
                        maxItem = uom[u];
                    }
                }
                largestuom = maxItem; // Business.LibraryPIMProduct.LargestUnit(skudata.ProductID, franchisegroupid)
                skudata.LargestUom = largestuom;
                skudata.BaseUnit = CStr2(skudata.UnitMeasure);
            } else {
                largestuom = skudata.LargestUom;
            }

            if (cntCol == 1) {
                row = $('<div class="row g-4 mb-4 row-cols-' + cntRowCol + '">');
            }

            if (vdmmotors.length > 1) {
                skudata.OtherVDMMotorNos = vdmmotors.splice(1);
            }

            if (arrOrderItems.length > 0) {
                var skucart = arrOrderItems.find(item => item.ProductID == skudata.ProductID)
                skudata.OrderQty = skucart ? CDec2(skucart.OrderQty) : 0;
            }
            
            skudata.DiscountedPrice = Business.LibraryPIMProduct.RoundPrice(skudata.DiscountedPrice);
            skudata.VDMMotorNo = Math.min(...vdmmotors.map(Number)); //get least number - kay pwede multiple motors for 1 SKU, bale kwaon lang ang pinaka least motor number
            row.append($('<div class="col" id="' + skudata.ProductID + '" motorno="' + ctrSKUNo + '">')
                .append($('<div class="border bordervm-secondary shadow-sm h-100 rounded-2 bg-white text-dark product_container position-relative ' + (skudata.OrderQty > 0 ? 'selectedSku' : '') + '">')
                    .append('<div class="position-absolute rounded-circle sku_badge_qty shadow ' + (skudata.OrderQty > 0 ? '' : 'd-none') + '" style="top: -20px;right: 0px; z-index: 2;"><button data-role="none" class="btn bgvm-secondary rounded-2 bordervm-primary font-weight-bold"><span class="bi-cart-check mr-2"></span><span class="lblMenuItemQty">' + skudata.OrderQty + '</span></button></div>')
                    .append($('<div class="p-3 position-relative">')
                        .append('<img src="' + imgpath + skudata.FileName + '_big.jpg" id="skuimg_' + skudata.ProductID + '" style="width:100%;" class="rounded-2 " />')
                        .data('data', skudata)
                        .click(function () {
                            var dis = $(this), data = dis.data('data');
                            selectedProduct = JSON.parse(JSON.stringify(data));
                            ModifyProduct(selectedProduct);
                            VendingMachine.AddLog('Menu', 'selecting encoded item number');
                        })
                    )
                    .append($('<div class="row g-0 mb-3 mr-1 ml-1 item_qty_section d-none">')
                        .append($('<div class="col-3">')
                            .append($('<button data-role="none" class="btn btn-sm text-danger rounded-2 bg-light"><span class="bi-trash"></span></button>')
                                .data('data', skudata)
                                .click(function () {
                                    var dis = $(this), data = dis.data('data');

                                    NewMessageBox('Are you sure you want to remove <b>' + data.ProductName + '</i>?', 'Confirmation Dialog', [
                                        {
                                            Text: 'Yes', Theme: 'btnvm-primary', Click: function () {
                                                RemoveItem();
                                            }
                                        }, { Text: 'No, maybe next time', Theme: 'btnvm-secondary textvm-primary' }
                                    ])                                    
                                })
                            )
                        )
                        .append($('<div class="col-9">')
                            .append($('<div class="input-group rounded-2 border bg-white">')
                                .append($('<button data-role="none" name="btnMenuReduceQty" class="btn btn-sm textvm-primary rounded-2 bg-light"><span class="bi-dash-lg"></span></button>')
                                    .data('data', skudata)
                                    .click(function () {
                                        var dis = $(this), data = dis.data('data');
                                        menu_qtyselection('-', data);
                                    })
                                )
                                .append('<input data-role="none" id="txtMenuItemOrderQty_' + skudata.ProductID + '" type="text" class="form-control form-control-sm form-control-plaintext text-center shadow-none bg-white gradient-borderx txtLineItemQty" value="' + skudata.OrderQty +'" readonly>')
                                .append($('<button data-role="none" name="btnMenuAddQty" class="btn btn-sm text-white rounded-2 bgvm-primary"><span class="bi-plus-lg"></span></button>')
                                    .data('data', skudata)
                                    .click(function () {
                                        var dis = $(this), data = dis.data('data');
                                        throwImage('skuimg_' + data.ProductID);
                                        menu_qtyselection('+', data);
                                    })
                                )
                            )
                        )
                    )
                    .append($('<div class="row g-0 mr-3 ml-3 align-items-center item_price">')
                        .append($('<div class="col">')
                            .append($('<div class="text-dark font-weight-bold" style="font-size:14px;">').text('P ' + (CBool(skudata.IsUseUOM) ? AccountingFormat(Business.LibraryPIMProduct.RoundPrice(CDec2(largestuom.DiscountedPrice))) + ' / ' + largestuom.UnitMeasure : AccountingFormat(skudata.DiscountedPrice))))
                            .append($('<div style="font-size:14px; font-style:itallic;">').append('<span class="badge bg-light text-secondary font-weight-normal">' + (CBool(skudata.IsUseUOM) ? /*CInt2(skudata.RemainingQty) / largestuom.UnitQty + ' left'*/ '' : '<span class="bi-handbag"></span> ' + CDec2(skudata.RemainingQty) + ' left') + '</div>'))
                        )
                        .append($('<div class="col-auto">')
                            .append($('<button data-role="none" class="btn text-white rounded-2 bgvm-primary item_card_addbtn"><span class="bi-plus-lg"></span></button>')
                                .data('data', skudata)
                                .click(function () {
                                    var dis = $(this), data = dis.data('data');
                                    var parent = $('#' + data.ProductID);
                                    selectedProduct = JSON.parse(JSON.stringify(data));

                                    if (CBool(selectedProduct.IsUseUOM)) {
                                        ModifyProduct(selectedProduct);
                                    } else {
                                        //hide all selection qty
                                        $('.item_price').removeClass('d-none');
                                        $('.item_qty_section').addClass('d-none');

                                        parent.find('.item_price').addClass('d-none');
                                        parent.find('.item_qty_section').removeClass('d-none');
                                        //parent.find('.txtLineItemQty').val(CDec2(parent.find('.txtLineItemQty').val()) + 1);

                                        isFromCart = true; // reuse lang
                                        parent.find('[name="btnMenuAddQty"]').click();
                                    }
                                })
                            )                            
                        )                        
                )
                    .append($('<div class="p-3 text-dark item_description" style="font-size:16px;">').text(skudata.ProductName + ' - ' + skudata.VDMMotorNo))                                        
                    //.append($('<div class="p-3">')
                    //    .append($('<button data-role="none" class="btn shadow-none btnvm-primary-outline btn-block">').html('<span class="bi-plus-lg"></span> Add to Cart'))
                    //)
                )                
            )

            //$.grep(arrOrderItems, function (e, index) {
            //    if (e.ProductID == skudata.ProductID) {
            //        //SelectSKU(e);
            //        return false; // stop
            //    }
            //})

            cntCol += 1;
            ctrSKUNo += 1;

            if (cntCol > cntRowCol || i == (sku.length - 1)) {
                listofskus.append(row);
                cntCol = 1;
            }
        }

        listofskus.find('> .row:eq(0)').addClass('curshowmenu');
        $(".product_container").delay(200).each(function (i) {
            $(this).delay(100 * i).queue(function () {
                $(this).addClass("show");
            })
        })
        ScrollIndicator();
        setTimeout(function () {
            //$('#btnUnmute').click();
            //Android.UnpinApp();
            //Android.PinApp();
            //$('#txtSelectedItemNo').focus();
        }, 1000)

    }

    function menu_qtyselection(action, data) {
        var parent = $('#' + data.ProductID);
        selectedProduct = JSON.parse(JSON.stringify(data));
        clearTimeout(menu_ItemSelection);
        menu_itemCountdown();
        OrderQty(action, $('#txtMenuItemOrderQty_' + selectedProduct.ProductID), 'menu_page', selectedProduct);
        AddToCart(CDec2(selectedProduct.OrderQty), CDec2(selectedProduct.RemainingQty));
        $('#txtMenuItemOrderQty_' + selectedProduct.ProductID).val(selectedProduct.OrderQty);

        if (CDec2(selectedProduct.OrderQty) > 0) {
            parent.find('.product_container').addClass('selectedSku');
            parent.find('.sku_badge_qty').removeClass('d-none');
            parent.find('.sku_badge_qty').find('.lblMenuItemQty').text(selectedProduct.OrderQty);
            VendingMachine.AddLog('Menu', 'Adding an item to the cart');
        } else {
            parent.find('.product_container').removeClass('selectedSku');
            parent.find('.sku_badge_qty').addClass('d-none');
            RemoveItem();
            VendingMachine.AddLog('Product Info', 'Item removal confirmed');
        } 
    }

    function menu_itemCountdown() {
        menu_ItemSelection = setTimeout(function () {
            var parent = $('#' + selectedProduct.ProductID);
            isFromCart = false;
            parent.find('.item_price').removeClass('d-none');
            parent.find('.item_qty_section').addClass('d-none');
            clearTimeout(menu_ItemSelection)
        }, 5000)
    }

    function UpdateCartInfo() {
        var total = 0, totalItemsCnt = 0;

        for (var i = 0; i < arrOrderItems.length; i++) {
            totalItemsCnt += CDec2(arrOrderItems[i].OrderQty);
            total += CDec2(arrOrderItems[i].DiscountedPrice) * CDec2(arrOrderItems[i].OrderQty);
        }

        $('.txtLineItem').text(totalItemsCnt);
        $('.txtCartTotal').text('P ' + AccountingFormat(total, 2, '0.00'));
    }

    function OrderQty(action, qtyfield, page, itemData) {
        var qty = CDec2(qtyfield.val());
        var newQty = 0, total = 0, available, items = 0;

        if (action == '-') {
            newQty = qty - 1;
        }

        if (action == '+') {
            newQty = qty + 1;
        }

        if (newQty < 0) newQty = 0;

        qtyfield.val(newQty);

        if (page == 'product_page') {
            available = CDec2($('.lblProductAvailableQty').text());
            var price = CDec2($('#lblProductPrice').text());
            total = newQty * price;
            if (newQty > available) {
                newQty = available;
                qtyfield.val(newQty);
                $('#popupVDMDialog h6').text('There ' + (available > 1 ? 'are' : 'is') + ' only ' + available + ' item' + (available > 1 ? 's' : ' is') + ' left for ' + itemData.ProductName + '.');
                $('#popupVDMDialog').modal('show');
            } else {
                $('#lblProductTotal').text(total > 0 ? AccountingFormat(total) : '0.00');
            }
        } else if (page == 'cart_page') {
            available = itemData.RemainingQty;
            if (newQty > available) {
                newQty = available;
                qtyfield.val(newQty);
                $('#popupVDMDialog h6').text('There ' + (available > 1 ? 'are' : 'is') + ' only ' + available + ' item' + (available > 1 ? 's' : '') + ' left for ' + itemData.ProductName + '.');
                $('#popupVDMDialog').modal('show');
            }
            total = newQty * CDec2(itemData.DiscountedPrice);
            itemData.OrderQty = newQty;
            SelectSKU(itemData);
            RecheckOrderedItems();
            $('#lblLineItemTotal_' + itemData.ProductID).text(AccountingFormat(total, 2, '0.00'));
            UpdateCartInfo();
            $('#lblOrderQty').text(total > 0 ? arrOrderItems.length + ' item(s)' : '');
        } else if (page == 'menu_page') {
            available = itemData.RemainingQty;
            if (newQty > available) {
                newQty = available;
                qtyfield.val(newQty);
                $('#popupVDMDialog h6').text('There ' + (available > 1 ? 'are' : 'is') + ' only ' + available + ' item' + (available > 1 ? 's' : '') + ' left for ' + itemData.ProductName + '.');
                $('#popupVDMDialog').modal('show');
            }
            total = newQty * CDec2(itemData.DiscountedPrice);
            itemData.OrderQty = newQty;
            UpdateCartInfo();
        }
    }

    function RecheckOrderedItems() {
        var newArrOrderItems = [];

        $.grep(arrOrderItems, function (sku) {
            // remove sku with 0 qty
            if (CInt2(sku.OrderQty) > 0) {
                newArrOrderItems.push(sku);
            } else {
                $('#cartid_' + sku.ProductID).remove();
            }
        })
        arrOrderItems = newArrOrderItems;
    }

    function LoadSettings() {
        var opts = VendingMachine.Settings;
        var listOptions = $('#listOptions').empty();

        for (var i in opts) {
            var vOpt = opts[i];

            listOptions.append($('<li>')
                .addClass('mb-3')
                .attr('optno', (CInt2(i) + 1))
                .data('data', vOpt)
                .append($('<div class="row align-items-center">')
                    .append($('<div class="col-auto">')
                        .append('<button data-role="none" class="btn light-beam navbtn-light bgvm-primary rounded-circle text-white" style="height:60px; width:60px; font-size:1.50em;"><span>' + (CInt2(i) + 1) + '</span></button>')
                    )
                    .append($('<div class="col text-secondary">')
                        .append(vOpt.Label)
                    )
            )
                .click(function () {
                    $(this).data('data').Callback();
                })
            )
        }

        var ads = VendingMachine.GetAds();
        var PSGuides = VendingMachine.GetProductSelectionGuides();
        var listAds = $('#listAds').empty();
        var listIdleAds = $('#listIdleAds').empty();

        if (ads.length > 0) {
            for (var i in ads) {
                var vOpt = ads[i];
                var adsContainer;

                if (vOpt.Orientation == 'Landscape') {
                    adsContainer = listIdleAds;
                } else {
                    adsContainer = listAds;
                }

                if (vOpt.Media == 'image') {
                    adsContainer.append($('<li class="slide ads_' + vOpt.Orientation +'">')
                        .append($('<img class="h-100" style="margin: auto; max-width: 100vw">')
                            .attr('src', '/Content/VendingMachine/' + sessionStorage.OrgID + '/' + vOpt.FileName)
                        )
                    )
                } else {
                    adsContainer.append($('<li class="slide ads_' + vOpt.Orientation +'">')
                        .append($('<video muted class="h-100" style="margin: auto; max-width: 100vw">')
                            .append($('<source>')
                                .attr('path', '/Content/VendingMachine/' + sessionStorage.OrgID + '/' + vOpt.FileName + '?v=' + new Date().getTime())
                            )
                        )
                    )
                }
            }

            //slides = document.querySelector('.slidesLandscape');
            //slideItems = $('.slidesLandscape > li') //document.querySelectorAll('.ads_Landscape');
            //goToSlide(0);
        }

        if (PSGuides.length > 0) {
            for (var i in PSGuides) {
                var vOpt = PSGuides[i];
                var PSGContainer = $('.productselection').find('.pgs_' + vOpt.Page);
                var ul = PSGContainer.find('ul');

                if (PSGContainer.length == 0) {
                    $('.productselection').append($('<div class="pgs_' + vOpt.Page + ' h-100" subpage="productselection" style="display:none;">')
                        .append('<ul class="slides slidesPortrait"></ul>')
                    )
                    PSGContainer = $('.productselection').find('.pgs_' + vOpt.Page);
                    ul = PSGContainer.find('ul');
                }

                if (vOpt.Media == 'image') {
                    ul.append($('<li class="slide">')
                        .append($('<img class="vh-100" style="margin: auto; max-width: 100vw">')
                            .attr('src', '/Content/VendingMachine/' + sessionStorage.OrgID + '/' + vOpt.FileName + '?v=' + new Date().getTime())
                        )
                    )
                } else {
                    ul.append($('<li class="slide">')
                        .append($('<video muted class="vh-100" style="margin: auto; max-width: 100vw">')
                            .append($('<source>')
                                .attr('path', '/Content/VendingMachine/' + sessionStorage.OrgID + '/' + vOpt.FileName)
                            )
                        )
                    )
                }
            }
        }

        var videos = $("video");
        var loadedCount = 0;

        videos.each(function () {
            $(this).on("loadeddata", function () {
                loadedCount++;
                if (loadedCount === videos.length) {
                    console.log("All videos are fully loaded and ready to play!");
                }
            });
        });
        $('.btnInitDone').click();
        //$('#page_idle').pageshow();
    }

    function SaveCart(tempData, callback) {
        var transData = $.extend({
            SaveAsDraft: 1,
            InvoiceID: '',
            CashAmount: totalCashInserted,
            TotalAmountDue: totalAmountToPay,
            CustomerChangeCash: totalCashInserted - totalAmountToPay,
            NoOfPeople: 1,
            NoOfSeniorCitizens: 1,
            Items: arrOrderItems,
            CashInsertedBreakdown: []
        }, tempData)
        VendingMachine.SaveCart(transData, function (data) {
            invoiceid = data.InvoiceID;
            sessionStorage.InvoiceID = CStr2(invoiceid);
            callback(data);
        })
    }

    function Passcode(desc, pin, callback) {
        $('#page_settings_pin').pageshow();
        $('#page_settings_pin input').val('');
    }

    var currentVideoElement = null;

    function goToSlide(index) {
        if (isTransitioning || slideItems.length == 0) return;

        var totalSlides = slideItems.length;
        var previousSlide = currentSlide;

        currentSlide = index; // (index + totalSlides) % totalSlides;

        isTransitioning = true;
        //slides.style.transform = `translateX(-${currentSlide * 100}%)`;

        slideItems.each(function () {
            $(this).hide();
        })

        if (totalSlides == currentSlide) currentSlide = 0;

        var currentElement = slideItems[currentSlide];        

        $(slideItems[currentSlide]).fadeIn();

        if (currentVideoElement) {
            currentVideoElement.pause();
            currentVideoElement.removeAttribute('src'); // Removes the file from memory
            currentVideoElement.load();                 // Forces the WebView to release the resources
        }

        if (currentElement.querySelector('video')) {
            var video = currentElement.querySelector('video');
            currentVideoElement = video;
            video.muted = true;
            video.controls = true;
            //video.playbackRate = 0.10;
            //video.preservesPitch = false;
            video.className = "h-100";
            video.style = "margin: auto; max-width: 100vw";
            video.setAttribute("playsinline", "");

            video.src = video.children[0].getAttribute('path'); //'/Content/VendingMachine/cfe39df6-8809-4973-9515-810df75aed12/ads_01312025_015216pm_f43747ce-1f94-4cb9-98a1-dc46023fa3fc.mp4';
            video.addEventListener('canplay', function onCanPlay() {
                var playPromise = video.play().catch(console.error);
                if (playPromise !== undefined) {
                    playPromise.catch(e => console.log("Play interrupted or blocked:", e));
                }
            }, { once: true });

            // Force the browser to evaluate the newly added src attribute
            video.load();         
            video.onended = () => {
                //Android.AddLog('Playback: ' + video.playbackRate + ' Current Time: ' + video.currentTime);
                isTransitioning = false;
                goToSlide(currentSlide + 1);
            };
        } else {
            setTimeout(() => {
                isTransitioning = false;
                goToSlide(currentSlide + 1);
            }, 5000); // Pause for 5 seconds for images
        }
    }
    window.GuidesOnScreenTime = GuidesOnScreenTime

    function GuidesOnScreenTime(DispensingType) {
        $('#page_guides').pageshow();

        var curStep = $('.divStep.' + DispensingType +':visible');
        var data = curStep.data('data');
        var onscreentime = CInt2(data.OnScreenTime);
        if (onscreentime == 0) onscreentime = 10;

        if (CBool(data.IsDisplayInfo)) {
            curStep.find('.row > div:eq(0)').addClass('dispenseinfo');
        }

        fitTextToDiv('desc_' + data.ID);

        if (DispensingType == 'Liquid_Products') {
            //hide since it's instructional video
            curStep.find('.vid_info').hide();
        }

        if (currentVideoElement) {
            currentVideoElement.pause();
            currentVideoElement.removeAttribute('src'); // Removes the file from memory
            currentVideoElement.load();                 // Forces the WebView to release the resources
        }

        PauseAllVideos(function () {
            if (data.Media == 'video') {
                var video = curStep.find('video')[0];
                curStep.find('.btnNextStep').parent().addClass('d-none').removeClass('d-flex');
                currentVideoElement = video;
                video.muted = true;
                //video.playbackRate = 0.25;
                //video.preservesPitch = false;
                video.className = "h-100";
                video.style = "margin: auto; max-width: 100vw";
                video.setAttribute("playsinline", "");

                video.src = video.getAttribute('data-src'); //'/Content/VendingMachine/cfe39df6-8809-4973-9515-810df75aed12/ads_01312025_015216pm_f43747ce-1f94-4cb9-98a1-dc46023fa3fc.mp4';
                // 1. Set up the event listener for metadata loading
                video.addEventListener('loadedmetadata', function onMetadataLoaded() {
                    console.log("Metadata loaded successfully, attempting play.");

                    // Set a flag showing we are actively trying to play
                    video.isPlayPending = true;

                    var playPromise = video.play();

                    if (playPromise !== undefined) {
                        playPromise.then(function () {
                            // Play succeeded! Clear the lock
                            video.isPlayPending = false;
                        }).catch(function (error) {
                            video.isPlayPending = false;
                            console.log("Play interrupted or blocked:", error);
                        });
                    }
                }, { once: true });

                // 2. Intercept any native or external .pause() calls on this video
                var originalPause = video.pause;
                video.pause = function () {
                    var self = this;
                    // If a play is currently pending, don't crash! Wait until it settles, then pause.
                    if (self.isPlayPending) {
                        console.warn("Pause requested too early! Delaying pause to prevent DOMException.");
                        setTimeout(function () {
                            originalPause.call(self);
                        }, 100); // Small delay lets the play promise clear safely
                    } else {
                        originalPause.call(self);
                    }
                };

                // 3. Force the WebView to evaluate the newly added src attribute
                video.load();
                //video.currentTime = 0;

                //video.play();
                video.onended = () => {
                    //video.play();
                    var dv = curStep.next();
                    if (dv.length > 0 && dv.hasClass('divStep')) {
                        if (DispensingType == 'Liquid_Products') {
                            video.play();
                            if (DispenseLiquidStepNo == 1) {
                                curStep.find('.btnNextStep').parent().removeClass('d-none').addClass('d-flex');
                                curStep.find('.btnNextStep').unbind('click').click(function (e) {
                                    currentVideoElement.pause();
                                    $('#page_insertbottle').pageshow();
                                    DispenseLiquidState = 'opening_door';
                                    UsbSerial_Send({
                                        "action": "CONTROL_DOOR",
                                        "uuid": '000078EE35E81F84', //'000078EE35E81F84' /*arrLiquid[0].BoardUUID*/,
                                        "state": true,
                                        "door": 0
                                    })
                                })

                                $('#btnStartDispenseLiquid').unbind().click(function () {
                                    DispenseLiquidStepNo += 1;
                                    $('.divStep.Liquid_Products').hide();

                                    DispenseLiquidState = 'dispensing_liquid'
                                    remainingVolumeQty = 0;
                                    isStatusBottleResponded = true;
                                    Android.AddLog('Remaining Volume: ' + CStr2(remainingVolumeQty));
                                    UsbSerial_Send({
                                        "action": "DISPENSE_VOLUME",
                                        "motor": CDec2(arrLiquid[0].VDMMotorNo),
                                        "uuid": '000078EE35E81F84' /*arrLiquid[0].BoardUUID*/,
                                        "volume": CDec2(arrLiquid[0].Quantity)
                                    })

                                    var second_step = $('.divStep.Liquid_Products:eq(1)')
                                    second_step.show();
                                    second_step.find('.btnStop').parent().removeClass('d-none').addClass('d-flex');
                                    GuidesOnScreenTime(DispensingType);

                                    second_step.find('.btnStop').unbind().click(function () {
                                        $(this).attr('disabled', 1).html('<span class="bi-stop-fill mr-3"></span> Stopped');
                                        second_step.find('.btnContinue').removeAttr('disabled');
                                        UsbSerial_Send({
                                            "action": "DISPENSE_STOP",
                                            "motor": CDec2(arrLiquid[0].VDMMotorNo),
                                            "uuid": '000078EE35E81F84' /*arrLiquid[0].BoardUUID*/
                                        })
                                    })

                                    second_step.find('.btnContinue').unbind().click(function () {
                                        $(this).attr('disabled', 1);
                                        second_step.find('.btnStop').removeAttr('disabled').html('<span class="bi-stop-fill mr-3"></span> Stop');
                                        UsbSerial_Send({
                                            "action": "DISPENSE_VOLUME",
                                            "motor": CDec2(arrLiquid[0].VDMMotorNo),
                                            "uuid": '000078EE35E81F84' /*arrLiquid[0].BoardUUID*/,
                                            "volume": remainingVolumeQty
                                        })
                                        remainingVolumeQty = latestVolumeQty;
                                    })                                  
                                })
                            } else {

                            }
                        } else {
                            $('#' + dv.attr('id')).pageshow();
                            GuidesOnScreenTime(DispensingType)
                        }
                    } else {
                        //OrdersComplete();
                    }
                };
            } else {
                //timerGuides = setTimeout(() => {
                //    clearTimeout(timerGuides);
                //    var dv = curStep.next();
                //    if (dv.length > 0 && dv.hasClass('divStep')) {
                //        $('#' + dv.attr('id')).pageshow();
                //        GuidesOnScreenTime(DispensingType)
                //    } else {
                //        //OrdersComplete();
                //    }
                //}, CInt2(data.OnScreenTime) * 1000); // Pause for 5 seconds for images
            }
        });        
    }

    function OrdersComplete() {
        arrOrderItems = arrDroppedItem;
        setTimeout(function () {
            SaveCart({
                SaveAsDraft: 0,
                InvoiceID: sessionStorage.InvoiceID
            }, function (data) {
                if (totalAmountToPay > totalAmountToPay) {
                    $('#page_cashchange').pageshow();
                    $('#page_cashchange').find('h3').html('Your P' + AccountingFormat(totalCashInserted - totalAmountToPay) + ' change is being processed.' + (arrFailedDropItem.length > 0 ? '<br><br>After this, we\'ll refund you for the items that were not dispensed by the machine.' : ''));
                    setTimeout(function () {
                        VendingMachine.AddLog('Machine', 'Processing a payment change');
                        CashChange(totalCashInserted - totalAmountToPay, 0);
                    }, 2000)
                } else {
                    if (arrFailedDropItem.length > 0) {
                        $('#page_cashchangecompleted').find('h3').text('Your change and refund are ready! Please collect them from the change slot.');
                        FailedOrders();
                    } else {
                        VendingMachine.AddLog('Machine', 'End');
                        if (!isCancelDuringPayment) {
                            $('#page_ordercomplete').pageshow();
                        }
                        setTimeout(function () {
                            NewTrans();
                        }, 2000);
                    }
                }
            })
        }, 2000)
    }

    function FailedOrders() {
        var tbodyFailedItems = $('#tbodyFailedItems').empty(), amtRefunded = 0, items = [];

        for (var i in arrFailedDropItem) {
            var vItem = arrFailedDropItem[i];
            var linetotal = CDec2(vItem.OrderQty) * CDec2(vItem.DiscountedPrice);
            amtRefunded += linetotal;
            tbodyFailedItems.append($('<tr class="text-secondary">')
                .data('data', vItem)
                .append($('<td>').text(vItem.ProductName))
                .append($('<td class="text-center">').text(vItem.OrderQty))
                .append($('<td class="text-center">').text(AccountingFormat(vItem.DiscountedPrice)))
                .append($('<td class="text-center">').text(AccountingFormat(linetotal)))
            )

            items.push({
                ProductID: vItem.ProductID,
                Quantity: vItem.OrderQty,
                ActualQuantity: 1,
                UnitMeasure: vItem.UnitMeasure,
                UnitPrice: CDec2(vItem.DiscountedPrice),
                TaxAmount: CDec2(vItem.DiscountedPrice) - (CDec2(vItem.DiscountedPrice) / 1.12)
            })
        }

        $('#page_failedorder h2').text('Amount to be refunded: ' + AccountingFormat(amtRefunded));
        $('#page_failedorder').pageshow();

        if (amtRefunded > 0) {
            VendingMachine.AddLog('Machine', 'Processing a refund');
            setTimeout(function () {
                arrFailedDropItem = [];
                paymentChangeAttempt = 1;
                
                VendingMachine.SalesRefund({
                    InvoiceID: sessionStorage.InvoiceID,
                    Stocks: items
                }, function () {
                    CashChange(amtRefunded, 1);
                })                
            }, 2000)
        }
    }

    function fitTextToDiv(containerId) {
        return;
        var container = document.getElementById(containerId);
        var fontSize = parseInt(window.getComputedStyle(container).fontSize);

        while (container.scrollHeight > container.clientHeight || container.scrollWidth > container.clientWidth) {
            fontSize -= 1; // Reduce font size
            container.style.fontSize = fontSize + "px";

            // Break if the font size is too small
            if (fontSize <= 1) {
                break;
            }
        }
    }

    function ScrollIndicator() {
        return
        setTimeout(function () {
            var container = document.getElementById('containerSKUs')
            var scrollTop = container.scrollTop;
            var scrollBottom = container.scrollHeight - container.offsetHeight - scrollTop;

            // Show or hide the top indicator
            if (scrollTop > 0) {
                scrollTopIndicator.classList.add('show');
            } else {
                scrollTopIndicator.classList.remove('show');
            }

            // Show or hide the bottom indicator
            if (scrollBottom > 0) {
                scrollBottomIndicator.classList.add('show');
            } else {
                scrollBottomIndicator.classList.remove('show');
            }
        }, 500)        
    }

    function ShowPageGuides(page) {
        PauseAllVideos(function () {
            $('.pgs_' + page).pageshow();
            isTransitioning = false;
            slides = $('.pgs_' + page + ' > .slidesPortrait ')[0];
            slideItems = $('.pgs_' + page + ' > .slidesPortrait > li');
            goToSlide(0);
        });        
    }

    function PauseAllVideos(callback) {
        callback();
        return
        var vids = $("video"), noofvid = vids.length, ctrVid = 0;

        if (0 == noofvid && typeof callback == 'function') {
            callback();
            return
        }

        vids.each(function () {
            var videoElement = this;

            // 1. Only pause if the video is actually playing/not paused
            if (!videoElement.paused) {
                videoElement.pause();

                // 2. Wrap the timeline reset in a tiny timeout or omit it 
                // to allow the browser's media engine a chance to catch up
                setTimeout(function () {
                    videoElement.currentTime = 0;
                }, 50);
            } else {
                // If it's already paused, just reset it safely
                videoElement.currentTime = 0;
            }

            ctrVid += 1;
            if (ctrVid == noofvid && typeof callback == 'function') {
                callback();
            }
        });
    }

    function throwImage(imageId) {
        const originalImg = document.getElementById(imageId);
        const cartIcon = document.getElementById('btnMenuViewOrder');

        // 1. Get positions
        const originalRect = originalImg.getBoundingClientRect();
        const cartRect = cartIcon.getBoundingClientRect();

        // 2. Create the particle and set its source to the original image
        const particle = document.createElement('img');
        particle.src = originalImg.src; // <--- The crucial step
        particle.classList.add('flying-particle');

        // 3. Position particle exactly over the original image
        particle.style.left = `${originalRect.left}px`;
        particle.style.top = `${originalRect.top}px`;
        particle.style.width = `${originalRect.width}px`; // Match original size initially

        // 4. Calculate flight path
        const deltaX = cartRect.left - originalRect.left;
        const deltaY = cartRect.top - originalRect.top;

        // Set the flight path variables
        particle.style.setProperty('--x', `${deltaX}px`);
        particle.style.setProperty('--y', `${deltaY}px`);

        // 5. Throw it
        document.body.appendChild(particle);

        // 6. Cleanup (Match setTimeout to CSS animation duration: 1s)
        setTimeout(() => {
            particle.remove();
            //updateCartCount();
        }, 2000);
    }

    $(document).ready(function () {        

        Init();

        window.addEventListener('scroll', ScrollIndicator);
        window.addEventListener('resize', ScrollIndicator);

        $('#btnMoreOptions').on('touchend', function () {
            clearTimeout(optionpress)
        }).on('touchstart click', function () {
            optionpress = setTimeout(function () {
                VendingMachine.Passcode('MACHINE_SETTINGS', function () {
                    if (htmlInAppSettings == "") {
                        AjaxSend({
                            ExactUrl: '/VendingMachine/InAppSettings?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                            type: 'GET',
                            dataType: 'text',
                            success: function (vData) {
                                htmlInAppSettings = vData;
                                $('#page_inappsettings').append(vData);
                                $('#page_inappsettings').pageshow();
                            }
                        });
                    } else {
                        $('#page_inappsettings').pageshow();
                        VendingMachine.RefreshSalesDashboard();
                    }
                })                
            }, 1000)
            
            return
            optionpress = setTimeout(function () {
                PopupButtonList('', 'Options', [
                    {
                        Text: 'Open Product Door',
                        Click: function () {
                            UsbSerial_Send({
                                "action": "CONTROL_DOOR",
                                "uuid": "000078EE35E81F84",
                                "state": true,
                                "door": 1
                            })
                        }
                    },
                    {
                        Text: 'Close Product Door',
                        Click: function () {
                            UsbSerial_Send({
                                "action": "CONTROL_DOOR",
                                "uuid": "000078EE35E81F84",
                                "state": false,
                                "door": 1
                            })
                        }
                    },
                    {
                        Text: 'Open Liquid Door',
                        Click: function () {
                            UsbSerial_Send({
                                "action": "CONTROL_DOOR",
                                "uuid": "000078EE35E81F84",
                                "state": true,
                                "door": 0
                            })
                        }
                    },
                    {
                        Text: 'Close Liquid Door',
                        Click: function () {
                            UsbSerial_Send({
                                "action": "CONTROL_DOOR",
                                "uuid": "000078EE35E81F84",
                                "state": false,
                                "door": 0
                            })
                        }
                    },
                    {
                        Text: 'Go to Liquid Vids',
                        Click: function () {
                            DispenseLiquidStepNo = 1;
                            $('#page_guides').pageshow();
                            $('.divStep.Liquid_Products').hide();
                            $('.divStep.Liquid_Products:eq(0)').show();
                            $('#btnNextStep').parent().removeClass('d-flex').addClass('d-none');
                            //var dv = $('.divStep:visible');
                            //Android.AddLog(JSON.stringify(arrGrain[itemIndex]));
                            //dv.pageshow();
                            GuidesOnScreenTime('Liquid_Products');
                        }
                    },
                    {
                        Text: 'Dispense Liquid No. 1',
                        Click: function () {
                            UsbSerial_Send({
                                "action": "DISPENSE_VOLUME",
                                "motor": 1,
                                "uuid": '000078EE35E81F84' /*arrLiquid[0].BoardUUID*/,
                                "volume": 5, //CDec2(arrLiquid[0].Quantity)
                            })
                        }
                    },
                    {
                        Text: 'Dispense Liquid No. 2',
                        Click: function () {
                            UsbSerial_Send({
                                "action": "DISPENSE_VOLUME",
                                "motor": 2,
                                "uuid": '000078EE35E81F84' /*arrLiquid[0].BoardUUID*/,
                                "volume": 5, //CDec2(arrLiquid[0].Quantity)
                            })
                        }
                    },
                    {
                        Text: 'Dispense Liquid No. 23',
                        Click: function () {
                            UsbSerial_Send({
                                "action": "DISPENSE_VOLUME",
                                "motor": 3,
                                "uuid": '000078EE35E81F84' /*arrLiquid[0].BoardUUID*/,
                                "volume": 5, //CDec2(arrLiquid[0].Quantity)
                            })
                        }
                    },
                    {
                        Text: 'Dispense all coins',
                        Click: function () {
                            UsbSerial_Send({
                                "action": "CASH_CHANGE",
                                "uuid": "000078EE35E81F84",
                                "coin1": 5,
                                "coin2": 5,
                                "coin3": 5,
                                "coin4": 5
                            })
                        }
                    },
                    {
                        Text: 'Status Bottle',
                        Click: function () {
                            UsbSerial_Send({
                                "action": "STATUS_BOTTLE",
                                "uuid": "000078EE35E81F84"
                            })
                        }
                    },
                    {
                        Text: 'Open Merchandise Cabinet Lock',
                        Click: function () {
                            Passcode('Lock 1: Merchandise Cabinet Lock', function () {
                                UsbSerial_Send({
                                    "action": "UNLOCK_LOCK",
                                    "lock": 1
                                })
                            })
                        }
                    },
                    {
                        Text: 'Open Compute Node Cabinet Lock',
                        Click: function () {
                            Passcode('Lock 2: Compute Node Cabinet Lock', function () {
                                UsbSerial_Send({
                                    "action": "UNLOCK_LOCK",
                                    "lock": 2
                                })
                            })
                        }
                    },
                    {
                        Text: 'Open Bills and Coins Cabinet Lock',
                        Click: function () {
                            //Passcode('Lock 3: Bills and Coins Cabinet Lock', function () {
                                UsbSerial_Send({
                                    "action": "CONTROL_LOCK",
                                    "uuid": "000078EE35E81F84",
                                    "num": 2
                                })
                            //})
                        }
                    },
                    {
                        Text: 'Dispense Coins',
                        Click: function () {
                            Passcode('Dispense Coins', function () {
                                PopupButtonList('', 'Select coin to dispense', [
                                    {
                                        Text: 'P 1',
                                        Click: function () {
                                            coin = 1;
                                            UsbSerial_Send({
                                                "action": "FLUSH_COINS",
                                                "coinHopper": 1
                                            })
                                        }
                                    },
                                    {
                                        Text: 'P 5',
                                        Click: function () {
                                            coin = 5;
                                            UsbSerial_Send({
                                                "action": "FLUSH_COINS",
                                                "coinHopper": 2
                                            })
                                        }
                                    },
                                    {
                                        Text: 'P 10',
                                        Click: function () {
                                            coin = 10;
                                            UsbSerial_Send({
                                                "action": "FLUSH_COINS",
                                                "coinHopper": 3
                                            })
                                        }
                                    }
                                ])
                            })
                        }
                    },
                    {
                        Text: 'Test Machine Commands',
                        Click: function () {
                            Passcode('Test Machine Commands', function () {
                                
                            })
                        }
                    },
                    {
                        Text: 'Enable fullscreen',
                        Click: function () {
                            setTimeout(function () {
                                //Passcode('Pin/Unpin App', function () {
                                Android.PinApp();
                                //})
                            }, 3000)                            
                        }
                    },
                    {
                        Text: 'Disable fullscreen',
                        Click: function () {
                            //Passcode('Pin/Unpin App', function () {
                                Android.UnpinApp();
                            //})
                        }
                    },
                    {
                        Text: 'Open Serial',
                        Click: function () {
                            Js.Execute({ command: 'UsbSerial.Start' })
                        }
                    },
                    {
                        Text: 'Peer List',
                        Click: function () {
                            UsbSerial_Send({
                                "action": "PEER_LIST"
                            })
                        }
                    },
                    {
                        Text: 'Exit to Main Menu',
                        Click: function () {
                            //Passcode('Exit to Main Menu', function () {
                                Android.UnpinApp();
                                location.href = '/People/PeopleMenu?IsMobile=1';
                            //})
                        }
                    }
                ])
            }, 1000)
        })

        $('#btnPasscodeClear').click(function () {
            $('#txtPassCode').val('');
        })

        $('#btnPasscodeCancel').click(function () {
            $('#divExitPasscode').fadeOut();
        })

        $('#page_idle').click(function () {
            //DetectInactivity(30);
            //$('#page_trans, .page_menu').pageshow();
            //document.getElementsByTagName('video')[0].muted = false;
            //document.getElementsByTagName('video')[0].play();
        })

        $('#btnOrderReduce').click(function () {
            OrderQty('-', $('#txtOrderQty'), 'product_page');
        })

        $('#btnOrderIncrease').click(function () {
            OrderQty('+', $('#txtOrderQty'), 'product_page');
            //OrderQty('+', $('#txtOrderQty'), function (newQty, callback) {
            //    var productprice = CDec2($('#lblProductPrice').text());
            //    var qty = CDec2($('#lblProductPrice').text());
            //    var available = CDec2($('.lblProductAvailableQty').text());

            //    if (newQty > available) {
            //        NewMessageBox('Sorry! Only ' + available + ' item' + (available > 1 ? 's are' : ' is') + ' available for the item selected');
            //        callback(false);
            //    } else {
            //        callback(true);
            //    }
            //});
        })

        $('.btnItem').click(function () {
            $('.page_productinfo').pageshow();
        })

        $('.btnBackToMenu').click(function () {
            $('.page_menu').pageshow();
        })

        $('.btnViewOrder').click(function () {
            //ScreenTimeout()
            ViewOrder();
        })

        $('#btnOpenDoor').click(function () {
            UsbSerial_Send({
                "action": "CONTROL_DOOR",
                "uuid": "000078EE35E81F84",
                "state": true,
                "door": 0
            })
        })

        $('#btnCloseDoor').click(function () {
            UsbSerial_Send({
                "action": "CONTROL_DOOR",
                "uuid": "000078EE35E81F84",
                "state": false,
                "door": 0
            })
        })

        $('#btnStatusBottle').click(function () {
            UsbSerial_Send({
                "action": "STATUS_BOTTLE",
                "uuid": "000078EE35E81F84"
            })
        })

        $('#btnPayNow').click(function () {
            if (arrOrderItems.length == 0) {
                //NewMessageBox('Please add at least one item.', '', [
                //    {
                //        Text: 'Ok', Theme: 'btnvm-primary'
                //    }
                //])
                //setTimeout(function () {
                //    NewMessageBoxClose();
                //}, 2000)
                return;
            }
            
            totalAmountToPay = CDec2(CStr2($('.txtCartTotal:eq(0)').text()).match(/\d+(\.\d+)?/g)[0]);
            totalCashInserted = 0;
            ctrDispenseItem = 0;
            ctrDispenseGrain = 0;
            ctrDispenseLiquid = 0;
            RecheckOrderedItems();
            $('#lblAmountToPay').html('<span>&#8369;</span> ' + AccountingFormat(totalAmountToPay));
            $('#lblAmountInserted').html('<span>&#8369;</span> 0.00');
            $('.page_paynow').pageshow();
            ShowPageGuides('payment');
            sessionStorage.arrOrderItems = JSON.stringify(arrOrderItems);
            console.log(arrOrderItems);
            VendingMachine.AddLog('Cart', 'Paying now');
            SaveCart({ SaveAsDraft: 1 }, function (data) {
                invoiceid = data.InvoiceID;
                Android.AddLog('INVOICE ID: ' + invoiceid)
                arrCashInsertedBreakdown = [{
                    Denom: 50,
                    Count: 1
                }]
                totalCashInserted = 50;
                VendingMachine.SaveCash({ CashInsertedBreakdown: arrCashInsertedBreakdown }, invoiceid, function () {
                    var tbodyDispenseItems = $('#tbodyDispenseItems').empty();
                    $('#page_paid').pageshow();
                    invoiceid = data.InvoiceID;
                    for (var i in arrOrderItems) {
                        if (CBool(arrOrderItems[i].IsUseUOM)) {
                            if (arrOrderItems[i].SellingPreference == 'Weight') {
                                arrGrain.push(arrOrderItems[i])
                            } else arrLiquid.push(arrOrderItems[i])
                        } else {
                            for (var b = 0; b < CDec2(arrOrderItems[i].OrderQty); b++) {
                                var copyItem = JSON.parse(JSON.stringify(arrOrderItems[i]));
                                copyItem.OrderQty = 1;
                                arrGoods.push(copyItem)
                            }
                        }
                    }

                    if (arrGoods.length > 0) {
                        setTimeout(function () {
                            $('.page_dispense').pageshow();
                            for (var i in arrOrderItems) {
                                if (!CBool(arrOrderItems[i].IsUseUOM)) {
                                    tbodyDispenseItems.append($('<tr class="text-secondary align-middle">')
                                        .data('data', arrOrderItems[i])
                                        .attr('productid', arrOrderItems[i].ProductID)
                                        .append($('<td class="text-center">').html('<img class="rounded-2" src="' + imgpath + arrOrderItems[i].FileName + '_big.jpg" style="width:70px;">'))
                                        .append($('<td class="text-center">').text(arrOrderItems[i].ProductName))
                                        .append($('<td class="text-center">').text(arrOrderItems[i].OrderQty))
                                        .append($('<td class="text-center tdDispensedQty">').text(''))
                                        .append($('<td class="text-center">'))
                                    )
                                }
                            }
                            var otherItems = $.merge(JSON.parse(JSON.stringify(arrGrain)), JSON.parse(JSON.stringify(arrLiquid))); //create copy for grain and liquid
                            for (var i in otherItems) {
                                tbodyDispenseItems.append($('<tr class="text-secondary align-middle">')
                                    .data('data', otherItems[i])
                                    .attr('productid', otherItems[i].ProductID)
                                    .append($('<td class="text-center">').text(otherItems[i].ProductName))
                                    .append($('<td class="text-center">').text(otherItems[i].OrderQty))
                                    .append($('<td class="text-center">').text(''))
                                    .append($('<td class="text-center">'))
                                )
                            }
                            tbodyDispenseItems.find('tr:eq(0)').removeClass('text-secondary').addClass('textvm-primary');
                            tbodyDispenseItems.find('tr:eq(0)').find('td:last-child').html('<div class="spinner-border" role="status"></div>');
                            DispenseItems(ctrDispenseItem);
                        }, 2000)
                    } else if (arrGrain.length > 0) {
                        DispenseGrains(ctrDispenseGrain);
                    } else if (arrLiquid.length > 0) {
                        console.log(arrLiquid[ctrDispenseLiquid])                        
                        DispenseLiquid(ctrDispenseLiquid);
                    }
                })
                return
                UsbSerial_Send({
                    "action": "CASH_COLLECT",
                    "uuid": "000078EE35E81F84",
                    "accept": true
                })
            })
        })

        $('#btnStart').click(function () {
            Js.Execute({ command: 'UsbSerial.Start' })
        })

        $('#btnSend').click(function () {
            Js.Execute({
                command: 'UsbSerial.Send',
                value: '{"action": "DISPENSE_ITEM", "motor_number": 1, "turns": 1}'
            })
        })

        $('#btnAddToCart').click(function () {
            if (CBool(selectedProduct.IsUseUOM)) {
                var chosenRice = [];

                var tempOrderItems = JSON.parse(JSON.stringify(arrOrderItems));
                for (var i in tempOrderItems) {
                    if (CBool(tempOrderItems[i].IsUseUOM) && selectedProduct.ProductID != tempOrderItems[i].ProductID) {
                        chosenRice.push(tempOrderItems[i]);
                        break;
                    }
                }

                if (chosenRice.length > 0) {
                    $('.page_updatechosenuom').pageshow();
                    $('#imgOriginalProduct').attr('src', imgpath + chosenRice[0].FileName + '_big.jpg');
                    $('#imgReplacementProduct').attr('src', imgpath + selectedProduct.FileName + '_big.jpg');
                    $('.page_updatechosenuom h3').html('Would you like to replace ' + chosenRice[0].UnitMeasure + ' ' + chosenRice[0].ProductName + ' with ' + $('#listUom').find('> div .btnUom.active:eq(0)').data('data').UnitMeasure + ' ' + selectedProduct.ProductName + '?<br><br>');
                    VendingMachine.AddLog('Product Info', 'Confirming the replacement of the chosen rice variety.');
                    return;
                }
            }

            AddToCart(CDec2($('#txtOrderQty').val()), CDec2($('.lblProductAvailableQty:eq(0)').text()));
        })

        $('#btnKeepOriginal').click(function () {
            $('.page_productinfo').pageshow();
            ShowPageGuides('productdetails');
            VendingMachine.AddLog('Product Info', 'The replacement of the item was cancelled');
        })

        $('#btnReplaceOriginal').click(function () {
            VendingMachine.AddLog('Product Info', 'Confirmed to replace the chosen rice variety');
            AddToCart();
            $('.btnViewOrder').click();
        })

        $('#txtOrderQty').keydown(function () {
            $(this).removeClass('is-invalid');
            $('#divErrMsgProductInfo').hide();
        })

        $('#btnUnmute').click(function () {
            document.getElementsByTagName('video')[0].muted = false;
            document.getElementsByTagName('video')[0].play();
        })

        $('form').submit(function (e) {
            e.preventDefault();
            e.stopPropagation();
        })

        //$('#page_start').click(function () {
        //    $('#page_idle').pageshow();
        //    slides = document.querySelector('.slidesLandscape');
        //    slideItems = document.querySelectorAll('.ads_Landscape');
        //    goToSlide(0);            
        //})

        $('.btnInitDone').click(function () {
            $('#page_idle').pageshow();
            //$('#page_passcode').pageshow();
            slides = document.querySelector('.slidesLandscape');
            slideItems = $('.slidesLandscape > li') //document.querySelectorAll('.ads_Landscape');
            goToSlide(0);
        })

        $('#btnPressToStart').click(function () {
            DetectInactivity(30);
            $('#page_trans, .page_menu').pageshow();
            ShowPageGuides('menu');
        })

        //$('form input').blur(function (e) {
        //    $(this).focus();
        //})

        function DispenseItems(itemIndex) {
            if (curDispenseItem != null && curDispenseItem.ProductID != arrGoods[itemIndex].ProductID) {
                dropAttempt = 1;
            }

            curDispenseItem = arrGoods[itemIndex];

            var tbodyDispenseItems = $('#tbodyDispenseItems tr[productid="' + curDispenseItem.ProductID + '"]');
            tbodyDispenseItems.removeClass('text-secondary').addClass('textvm-primary font-weight-bold');
            tbodyDispenseItems.find('td:last-child').removeClass('completed').html('<div class="spinner-border" role="status"></div>');

            VendingMachine.CheckBin($.extend(arrGoods[itemIndex], {
                success: function (e) {
                    console.log(e)
                    Android.AddLog('Turn Motor No.: ' + e.VDMMotorNo);
                    //Android.AddLog('Item: ' + JSON.stringify(e));
                    arrGoods[ctrDispenseItem] = e;
                    ctrDispenseItem += 1;
                    VendingMachine.AddLog('Machine', 'Attempting to drop the item');
                    setTimeout(function () {
                        var currentDroppedItem = JSON.parse(JSON.stringify(arrGoods[ctrDispenseItem - 1]));
                        var tbodyDispenseItems = $('#tbodyDispenseItems tr[productid="' + currentDroppedItem.ProductID + '"]');
                        var isDroppedItemExist = false, indexDroppedItem = null, qty = CDec2(tbodyDispenseItems.find('td.tdDispensedQty').text());

                        //arrFailedDropItem.push(currentDroppedItem);
                        //OrdersComplete();
                        //return;
                        $.grep(arrDroppedItem, function (pData, index) {
                            if (pData.ProductID == currentDroppedItem.ProductID && pData.BinProductID == currentDroppedItem.BinProductID) {
                                isDroppedItemExist = true;
                                indexDroppedItem = index;
                            }
                        })

                        if (isDroppedItemExist) {
                            arrDroppedItem[indexDroppedItem].OrderQty += currentDroppedItem.OrderQty;
                            qty += currentDroppedItem.OrderQty;
                        } else {
                            arrDroppedItem.push(currentDroppedItem);
                            qty += currentDroppedItem.OrderQty;
                        }

                        tbodyDispenseItems.removeClass('text-secondary').addClass('textvm-primary');
                        tbodyDispenseItems.find('td.tdDispensedQty').text(qty);
                        var tdstatus = tbodyDispenseItems.find('td:last-child').empty();
                        tdstatus.html((tdstatus.hasClass('dropfailed') ? '<span class="bi-exclamation-circle-fill text-danger"></span>' : '<span class="bi-check-circle-fill"></span>'));

                        VendingMachine.ComputeBinProduct(currentDroppedItem);

                        if ((ctrDispenseItem - 1) == arrGoods.length - 1) {
                            if (arrGrain.length > 0 || arrLiquid.length > 0) {
                                //do nothing
                            } else {
                                $('#page_ordercomplete').find('h3').html('Your order is now complete.<br><br>Your goods are ready for pick-up from the basket below.<br />Thank you for your purchase!')
                                OrdersComplete();
                            }
                        } else {                            
                            DispenseItems(ctrDispenseItem);
                        }                        
                    }, 2000)
                    return
                    UsbSerial_Send({
                        "action": "DISPENSE_ITEM",
                        "motor": CDec2(e.VDMMotorNo),
                        "turns": CDec2(e.OrderQty),
                        "uuid": e.BoardUUID,
                    })
                }
            }))
        }

        function DispenseGrains(itemIndex) {
            ctrDispenseGrain += 1;
            VendingMachine.CheckBin($.extend(arrGrain[itemIndex], {
                success: function (e) {
                    console.log(e)
                    Android.AddLog('Turn Bin No.: ' + e.VDMBinNo);
                    $('#page_guides').pageshow();
                    $('.divStep.Rice').hide();
                    $('.divStep.Rice:eq(0)').show();
                    //var dv = $('.divStep:visible');
                    //Android.AddLog(JSON.stringify(arrGrain[itemIndex]));
                    //dv.pageshow();
                    //GuidesOnScreenTime(dv.data('data'));
                    //arrDroppedItem.push(arrGrain[0]);
                    //console.log(arrDroppedItem);
                    //return;
                    //arrFailedDropItem.push(arrGrain[itemIndex]);
                    //OrdersComplete();
                    //return;
                    UsbSerial_Send({
                        "action": "DISPENSE_WEIGHT",
                        "bin": CDec2(arrGrain[itemIndex].VDMBinNo),
                        "uuid": CDec2(arrGrain[itemIndex].BoardUUID),
                        "weight": CDec2(arrGrain[itemIndex].Quantity)
                    })
                }
            }))        
        }

        function DispenseLiquid(itemIndex) {
            ctrDispenseLiquid += 1;
            DispenseLiquidState = 'opening_door';
            VendingMachine.CheckBin($.extend(arrLiquid[itemIndex], {
                success: function (e) {
                    console.log(e)
                    Android.AddLog('Item: ' + JSON.stringify(e));
                    DispenseLiquidStepNo = 1;
                    $('#page_guides').pageshow();
                    $('.divStep.Liquid_Products').hide();
                    $('.divStep.Liquid_Products:eq(0)').show();
                    $('#btnNextStep').parent().removeClass('d-flex').addClass('d-none');
                    //var dv = $('.divStep:visible');
                    //Android.AddLog(JSON.stringify(arrGrain[itemIndex]));
                    //dv.pageshow();
                    GuidesOnScreenTime('Liquid_Products');
                    //arrDroppedItem.push(arrGrain[0]);
                    //console.log(arrDroppedItem);
                    //return;
                    //arrFailedDropItem.push(arrGrain[itemIndex]);
                    //OrdersComplete();
                    //return;                    
                }
            }))        
        }

        window.CashChange = CashChange
        function CashChange(amount, isrefund) {
            IsPaymentChangeARefund = isrefund;
            VendingMachine.CashChange({
                InvoiceID: invoiceid,
                Amount: amount,
                IsRefund: isrefund,
                success: function (e) {
                    //if (arrFailedDropItem.length > 0) {
                    //    $('#page_cashchangecompleted').find('h3').text('Your change and refund are ready! Please collect them from the change slot.');
                    //    FailedOrders();
                    //} else {
                    //    NewTrans()
                    //}
                    //return
                    //FailedOrders();
                    UsbSerial_Send(e);
                }
            })
        }

        window.UsbSerial_Send = UsbSerial_Send;
        
        function UsbSerial_Send(params) {
            sessionStorage.IsInAppSettings = false;
            requestedAction = params.action;           

            //setTimeout(function () {
                Js.Execute({
                    command: 'UsbSerial.Send',
                    value: JSON.stringify(params)
                })
            //}, 500)

            //if (DispenseLiquidState == 'dispensing_liquid') {
            //    var timeoutStatBot = setInterval(function () {
            //        Js.Execute({ command: 'UsbSerial.Start' })

            //        UsbSerial_Send({
            //            "action": "STATUS_BOTTLE",
            //            "uuid": '000078EE35E81F84' /*arrLiquid[0].BoardUUID*/
            //        })

            //        if (isStatusBottleResponded) {
            //            clearInterval(timeoutStatBot)
            //        }
            //    }, 2000)                
            //}
            
            //Android.AddLog('ACTION: ' + requestedAction)
            //var stat = Android.UsbSerialSend(params);
            //Android.AddLog(stat)
        }

        function ViewOrder() {
            var divCartItems = $('#divCartItems').empty();

            $('.page_Cart').pageshow();
            ShowPageGuides('cart');
            console.log(arrOrderItems)
            if (arrOrderItems.length == 0) {
                isFromCart = false;
                divCartItems.append('<h1 class="p-5 text-center w-100 mt-5 text-secondary">Cart is empty</h1>');
                $('.txtLineItem').text('0');
                $('.txtCartTotal').text('P 0.00');
                return;
            } else {
                divCartItems.empty();
            }

            for (var i = 0; i < arrOrderItems.length; i++) {
                arrOrderItems[i].CartLineNo = (i + 1);
                LoadCart(arrOrderItems[i]);
            }

            UpdateCartInfo();
                        
            $('.product_cart').addClass('selectedSkuCart');

            $(".product_cart").delay(200).each(function (i) {
                $(this).delay(100 * i).queue(function () {
                    $(this).addClass("show");
                })
            })            
        }        

        function LoadCart(sku) {
            var divCartItems = $('#divCartItems');

            divCartItems.append($('<div class="p-3 rounded-2 shadow-sm bg-white w-100 mb-3 product_cart" cartlineno="' + sku.CartLineNo + '" style="font-size:16px;">')
                .attr('id', 'cartid_' + sku.ProductID)
                .data('data', sku)
                .append($('<div class="row align-items-center">')
                    //.append($('<div class="col-auto">')
                    //    .append('<button data-role="none" class="btn light-beam navbtn-light bgvm-primary rounded-circle text-white" style="height:60px; width:60px; font-size:1.50em;"><label>' + sku.CartLineNo + '</label></button>')
                    //)
                    .append($('<div class="col-auto">')
                        .append('<img class="rounded" src="' + sourceimg + '/Medium/' + sku.FileName + '_medium.jpg" style="object-fit:cover; width:80px;" />')
                    )
                    .append($('<div class="col">')
                        .append($('<div class="row mb-3">')
                            .append($('<div class="col">').append($('<h4 class="mb-0 text-dark">').text(sku.ProductName)).append($('<span class="text-secondary">').text('Available: ' + (CBool(sku.IsUseUOM) ? (CInt2(sku.RemainingQty) / sku.LargestUom.Quantity) + ' ' + ((sku.LargestUom.UnitMeasure).split('-')[1] || sku.LargestUom.UnitMeasure) : sku.RemainingQty))))
                            .append($('<div class="col-auto text-right">')
                                .append($('<button data-role="none" class="btn bg-danger-light text-danger rounded-2 btnRemove justify-content-center"><span class="bi-trash"></span></button>')
                                    .data('data', sku)
                                    .click(function (e) {
                                        e.stopPropagation();
                                        e.preventDefault();
                                        var dis = $(this), data = dis.data('data');
                                        selectedProduct = data;
                                        $('#lblRemoveProductName').text(data.ProductName);
                                        $('.page_confirmation').pageshow();
                                        VendingMachine.AddLog('Product Info', 'Removing an item');

                                        $('#btnCancelRemoveItem').unbind().click(function () {
                                            isFromCart = false;
                                            $('.btnViewOrder').click();
                                        })

                                        $('#btnRemoveItem').unbind().click(function () {
                                            isFromCart = false;
                                            RemoveItem();
                                            $('.btnViewOrder').click();
                                            VendingMachine.AddLog('Product Info', 'Item removal confirmed');
                                        })
                                        //return
                                        //NewMessageBox('Are you sure you want to remove <b>' + data.ProductName + '</b>?', 'Confirmation Dialog', [
                                        //    {
                                        //        Text: 'Yes', Theme: 'btnvm-primary', Click: function () {
                                        //            RemoveItem();
                                        //            ViewOrder();
                                        //        }
                                        //    }, { Text: 'No', Theme: 'btnvm-secondary textvm-primary' }
                                        //])
                                    })
                                )
                            )
                        )
                        .append($('<div class="row g-0 align-items-center">')
                            .append($('<div class="col">').append($('<h5 class="text-secondary">').text((CBool(sku.IsUseUOM) ? sku.UnitMeasure : 'P ' + AccountingFormat(sku.DiscountedPrice)))))
                            .append($('<div class="col text-center ' + (CBool(sku.IsUseUOM) ? 'd-none' : '') + '">')
                                //.append($('<h4 class="text-secondary">').text('x ' + sku.OrderQty + ''))
                                .append($('<div class="input-group rounded-2 border bg-white">')
                                    .append($('<button data-role="none" class="btn textvm-primary m-2 rounded-2 bg-light"><span class="bi-dash-lg"></span></button>')
                                        .data('data', sku)
                                        .click(function (e) {
                                            e.stopPropagation();
                                            e.preventDefault();
                                            OrderQty('-', $('#txtCartItemOrderQty_' + $(this).data('data').ProductID), 'cart_page', $(this).data('data'));
                                        })
                                    )
                                    .append('<input data-role="none" id="txtCartItemOrderQty_' + sku.ProductID + '" type="text" class="form-control form-control-plaintext form-control-lg text-center shadow-none bg-white gradient-borderx txtLineItemQty" value="' + sku.OrderQty + '" readonly>')
                                    .append($('<button data-role="none" class="btn textvm-primary m-2 rounded-2 bgvm-secondary"><span class="bi-plus-lg"></span></button>')
                                        .data('data', sku)
                                        .click(function (e) {
                                            e.stopPropagation();
                                            e.preventDefault();
                                            OrderQty('+', $('#txtCartItemOrderQty_' + $(this).data('data').ProductID), 'cart_page', $(this).data('data'));
                                        })
                                    )
                                )
                            )
                            .append($('<div class="col text-right">').append($('<h5 class="textvm-primary font-weight-bold lblLineItemTotal" id="lblLineItemTotal_' + sku.ProductID + '">').text('P ' + AccountingFormat(CDec2(sku.OrderQty) * CDec2(sku.DiscountedPrice)))))
                        )
                    )
                )
                .click(function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                    isFromCart = true;
                    ModifyProduct($(this).data('data'));
                })
            )

            $('.page_Cart').pageshow();
        }

        function Init() {
            Js.Execute({ command: 'UsbSerial.Start' })            

            function complete() {
                var guides = VendingMachine.GetGuides();
                var listStep = $('#divSteps').empty();
                franchisegroupid = CStr2(Business.LibraryOrganizationFOWST.GetFranchiseeData(sessionStorage.FowstID).FranchiseGroupID);
                console.log(guides)
                for (var g in guides) {
                    for (var i in guides[g].Steps) {
                        var divStep = $('#divStepTemplate').clone();
                        var step = guides[g].Steps[i];

                        if (CStr2(step.Filename) != '') {
                            if (CStr2(step.Media) == 'video') {
                                divStep.find('video').attr('data-src', '/Content/VendingMachine/' + sessionStorage.OrgID + '/' + step.Filename)
                                    //.append($('<source>')
                                    //    .attr('path', '/Content/VendingMachine/' + sessionStorage.OrgID + '/' + step.Filename)
                                    //);
                            } else {
                                divStep.find('img').attr('src', '/Content/VendingMachine/' + sessionStorage.OrgID + '/' + step.Filename).removeClass('d-none');
                            }
                        }

                        divStep.find('.textContainer').attr('id', 'desc_' + step.ID).text((CInt2(i) + 1) + '. ' + step.Description);

                        //if (i == 0) {
                        //    divStep.show();
                        //}

                        divStep.data('data', step).addClass('divStep ' + CStr2(step.DispensingType).replace(/ /g, "_")).attr('id', step.ID);
                        //$(divStep).insertBefore('#divGuidesLast')
                        $('#page_guides').append(divStep)
                        divStep.find('video')[0].load();
                        fitTextToDiv('desc_' + step.ID);
                    }
                }

                UsbSerial_Send({
                    "action": "PEER_LIST"
                })

                //UsbSerial_Send({
                //    "action": "CONTROL_POWER",
                //    "uuid": "000078EE35E81F84",
                //    "power": 10,
                //    "state": true
                //})

                //UsbSerial_Send({
                //    "action": "CONTROL_POWER",
                //    "uuid": "000068EE35E81F84",
                //    "power": 10,
                //    "state": true
                //})

                $('#lblOrderQty').text('P 0.00');
                LoadMenu();
                LoadSettings();
                //GuidesOnScreenTime($('.divStep:visible').data('data'))
            }
            if (CBool(Business.SystemDefaults.GetSystemDefault('DownloadSKUPicsAfterSyncingDB', 0))) {
                //complete();
                //return
                GetNewImageFiles({
                    OrgID: sessionStorage.OrgID,
                    FowstID: sessionStorage.FowstID,
                    Folder: 'MobizImagePIM',
                    Success: function (msg) {
                        $('#page_start h3').html('Downloading product images...<br>' + msg);
                    },
                    Complete: function () {
                        GetNewImageFiles({
                            OrgID: sessionStorage.OrgID,
                            FowstID: sessionStorage.FowstID,
                            Folder: 'VendingMachine',
                            Success: function (msg) {
                                $('#page_start h3').html('Downloading video ads...<br>' + msg);
                            },
                            Complete: function () {
                                $('#page_start h3').html('Your media library is up to date.<br>Finishing setup...');
                                setTimeout(function () {
                                    complete();
                                }, 2000)                                
                            },
                            Error: function (err) {
                                Android.AddLog('GetNewImageFilenames.Error: ' + JSON.stringify(err));
                                //complete();
                            }
                        });
                    },
                    Error: function (err) {
                        Android.AddLog('GetNewImageFilenames.Error: ' + JSON.stringify(err));
                        //complete();
                    }
                });
            } else {
                complete();
            }
        }

        VendingMachine.InventoryBalance = function () {
            var vItems = VendingMachine.GetMenu();
            var vDenom = VendingMachine.Denom;
            var tbodyInventory = $('#tbodyInventory').empty();
            var ctrInv = 1;

            if (vItems.length > 0) {
                for (var i in vItems) {                    
                    for (var s in vItems[i].Sku) {
                        var vData = vItems[i].Sku[s];
                        tbodyInventory.append($('<tr>')
                            .append($('<td>').text(ctrInv + '.'))
                            .append($('<td>').text(vData.ProductNo))
                            .append($('<td>').text(vData.ProductName))
                            .append($('<td>').text(CBool(vData.IsUseUOM) ? vData.VDMBinNo : vData.VDMMotorNo))
                            .append($('<td>').text(CDec2(vData.RemainingQty) + ' ' + vData.UnitMeasure))
                        )
                        ctrInv += 1;
                    }                    
                }
            }

            if (vDenom.length > 0) {
                for (var i in vDenom) {
                    var vData = vDenom[i];
                    tbodyInventory.append($('<tr>')
                        .append($('<td>').text(ctrInv + '.'))
                        .append($('<td>').text(vData.ProductNo))
                        .append($('<td>').text(vData.ProductName))
                        .append($('<td>').text('N/A'))
                        .append($('<td>').text(CDec2(vData.Balance) + ' ' + vData.UnitMeasure))
                    )
                    ctrInv += 1;
                }
            }

            $('#page_inventory').pageshow();
        }

        proj.TestAllMotors = function () {
            var tbodyTestAllMotors = $('#tbodyTestAllMotors');
            var tr = $('<tr class="text-secondary">').attr('testmotorno', ctrTestAllMotor);
            tr.append($('<td>').text(ctrTestAllMotor));
            tr.append($('<td>').html('<div class="spinner-border" role="status"></div>'));
            tbodyTestAllMotors.append(tr)

            //setTimeout(function () {
            //    $('tr[testmotorno=' + ctrTestAllMotor + ']').removeClass('text-secondary').addClass('textvm-primary').find('td:last-child').html('<span class="bi-check-circle-fill"></span>')

            //    if (ctrTestAllMotor == VendingMachine.NoOfMotors) {
            //        ctrTestAllMotor = 1;
            //    } else {
            //        ctrTestAllMotor += 1;
            //        proj.TestAllMotors();
            //    }
            //    $(".scrollable_area").scrollTop($(".scrollable_area")[0].scrollHeight);
            //}, 100)
            //return
            UsbSerial_Send({
                "action": "TURN_MOTOR",
                "motor": ctrTestAllMotor,
                "turns": 1
            })
        }

        proj.UseSerialOnReceivedData = function (vData) {
            var response;

            try {
                for (data of [JSON.parse(JSON.stringify(vData.data))]) {
                    response = JSON.parse(data);
                }

                DetectInactivity();

                if (CBool(sessionStorage.IsInAppSettings)) {
                    InAppSettings(response)
                    return;
                }

                //Vending Machine Controller Response
                if (requestedAction == 'PEER_LIST') {
                    var boards = response.devices;

                    for (var i in boards) {
                        UsbSerial_Send({
                            "action": "CONTROL_POWER",
                            "uuid": boards[i].uuid,
                            "power": 10,
                            "state": true
                        })

                        if (boards[i].modelId == 0) {
                            UsbSerial_Send({
                                "action": "CONTROL_RELAY",
                                "uuid": boards[i].uuid,
                                "num": 1,
                                "state": true
                            })

                            UsbSerial_Send({
                                "action": "CONTROL_EXT",
                                "uuid": boards[i].uuid,
                                "num": 2,
                                "state": true
                            })
                        }
                    }
                }
                else if (requestedAction == 'CASH_COLLECT') {
                    // clear this since naay sariling timeout ang CASH COLLECT
                    if (typeof countdown != 'undefined') clearInterval(countdown);
                    if (typeof listenerInactivity != 'undefined') clearInterval(listenerInactivity);
                    //end clear

                    if (response.status == 'ERROR') {
                        //if (response.code == 'TIMEOUT') {
                        //    isCancelDuringPayment = true;
                        //    ScreenTimeout(function (response) {
                        //        if (response == 'continue') {
                        //            isCancelDuringPayment = false;
                        //            UsbSerial_Send({
                        //                "action": "CASH_COLLECT",
                        //                "amount": totalAmountToPay - totalCashInserted,
                        //                "timeout": timeout
                        //            })
                        //        } else {
                        //            isCancelDuringPayment = true;
                        //            VendingMachine.AddLog('Payment', 'Transaction cancelled during payment.');
                        //            $('#page_paymentcancelled').pageshow();
                        //            $('#page_paymentcancelled').find('h3').text('You’ve cancelled your transaction.');
                        //            if (totalCashInserted > 0) {
                        //                $('#page_paymentcancelled').find('h3').append(' Don’t worry! We’ll return the money you inserted.')
                        //                setTimeout(function () {
                        //                    VendingMachine.SaveCash({ CashInsertedBreakdown: arrCashInsertedBreakdown }, sessionStorage.InvoiceID, function () {
                        //                        CashChange(totalCashInserted, 1);
                        //                    })
                        //                }, 2000)
                        //            } else {
                        //                setTimeout(function () {
                        //                    NewTrans();
                        //                }, 2000)
                        //            }
                        //        }
                        //    });
                        //} else {
                        VendingMachine.AddLog('Payment', 'Retrying to process payment');
                        UsbSerial_Send({
                            "action": "CASH_COLLECT",
                            "uuid": "000078EE35E81F84",
                            "accept": true
                        })
                        //}
                    }
                    else if (response.status == 'CREDIT') {
                        VendingMachine.AddLog('Payment', 'Inserted P' + response.amount + ' cash');
                        totalCashInserted += CDec2(response.amount);
                        $('#lblAmountInserted').html('<span>&#8369;</span> ' + AccountingFormat(totalCashInserted));

                        var isExist = false, index = null;

                        $.grep(arrCashInsertedBreakdown, function (e, ind) {
                            if (e.Denom == CDec2(response.amount)) {
                                isExist = true;
                                index = ind;
                                return false;
                            }
                        })

                        if (isExist) {
                            arrCashInsertedBreakdown[index].Count += 1;
                        } else {
                            arrCashInsertedBreakdown.push({
                                Denom: CDec2(response.amount),
                                Count: 1
                            })
                        }
                    }
                    else if (response.status == 'CASH_INSERT') {
                        if (totalCashInserted >= totalAmountToPay) {
                            UsbSerial_Send({
                                "action": "CASH_COLLECT",
                                "uuid": "000078EE35E81F84",
                                "accept": false
                            })
                            setTimeout(function () {
                                Android.AddLog(JSON.stringify(arrCashInsertedBreakdown))
                                VendingMachine.SaveCash({ CashInsertedBreakdown: arrCashInsertedBreakdown }, invoiceid, function () {
                                    var tbodyDispenseItems = $('#tbodyDispenseItems').empty();
                                    $('#page_paid').pageshow();
                                    invoiceid = data.InvoiceID;
                                    for (var i in arrOrderItems) {
                                        if (CBool(arrOrderItems[i].IsUseUOM)) {
                                            if (arrOrderItems[i].SellingPreference == 'Weight') {
                                                arrGrain.push(arrOrderItems[i])
                                            } else arrLiquid.push(arrOrderItems[i])
                                        } else {
                                            for (var b = 0; b < CDec2(arrOrderItems[i].OrderQty); b++) {
                                                var copyItem = JSON.parse(JSON.stringify(arrOrderItems[i]));
                                                copyItem.OrderQty = 1;
                                                arrGoods.push(copyItem)
                                            }
                                        }
                                    }

                                    if (arrGoods.length > 0) {
                                        //Android.AddLog('arrOrderItems: ' + JSON.stringify(arrOrderItems))
                                        setTimeout(function () {
                                            $('.page_dispense').pageshow();
                                            for (var i in arrOrderItems) {
                                                if (!CBool(arrOrderItems[i].IsUseUOM)) {
                                                    tbodyDispenseItems.append($('<tr class="text-secondary">')
                                                        .data('data', arrOrderItems[i])
                                                        .attr('productid', arrOrderItems[i].ProductID)
                                                        .append($('<td>').text(arrOrderItems[i].ProductName))
                                                        .append($('<td class="text-center">').text(arrOrderItems[i].OrderQty))
                                                        .append($('<td class="text-center tdDispensedQty">').text(''))
                                                        .append($('<td class="text-center">'))
                                                    )
                                                }
                                            }
                                            var otherItems = $.merge(JSON.parse(JSON.stringify(arrGrain)), JSON.parse(JSON.stringify(arrLiquid))); //create copy for grain and liquid
                                            for (var i in otherItems) {
                                                tbodyDispenseItems.append($('<tr class="text-secondary">')
                                                    .data('data', otherItems[i])
                                                    .attr('productid', otherItems[i].ProductID)
                                                    .append($('<td class="text-center">').text(otherItems[i].ProductName))
                                                    .append($('<td class="text-center">').text(otherItems[i].OrderQty))
                                                    .append($('<td class="text-center">').text(''))
                                                    .append($('<td class="text-center">'))
                                                )
                                            }
                                            tbodyDispenseItems.find('tr:eq(0)').removeClass('text-secondary').addClass('textvm-primary');
                                            tbodyDispenseItems.find('tr:eq(0)').find('td:last-child').html('<div class="spinner-border" role="status"></div>');
                                            DispenseItems(ctrDispenseItem);
                                        }, 2000)
                                    } else if (arrGrain.length > 0) {
                                        DispenseGrains(ctrDispenseGrain);
                                    } else if (arrLiquid.length > 0) {
                                        DispenseLiquid(ctrDispenseLiquid);
                                    }
                                })
                            }, 2000)
                        }
                    }
                }
                else if (requestedAction == 'DISPENSE_ITEM') {
                    var currentDroppedItem = JSON.parse(JSON.stringify(arrGoods[ctrDispenseItem - 1]));
                    var tbodyDispenseItems = $('#tbodyDispenseItems tr[productid="' + currentDroppedItem.ProductID + '"]');
                    Android.AddLog('Response: ' + response.status)
                    if (response.status == 'ERROR') {
                        dropAttempt += 1;
                        ctrDispenseItem -= 1;
                        VendingMachine.AddLog('Machine', response.code);
                        VendingMachine.AddLog('Machine', 'Retrying dropping the item');
                        if (dropAttempt > 2) {
                            var isDroppedItemExist = false, indexDroppedItem = null;
                            $.grep(arrFailedDropItem, function (pData, index) {
                                if (pData.ProductID == currentDroppedItem.ProductID && pData.BinProductID == currentDroppedItem.BinProductID) {
                                    isDroppedItemExist = true;
                                    indexDroppedItem = index;
                                }
                            })

                            if (isDroppedItemExist) {
                                arrFailedDropItem[indexDroppedItem].OrderQty += currentDroppedItem.OrderQty;
                            } else {
                                arrFailedDropItem.push(currentDroppedItem);
                            }

                            tbodyDispenseItems.find('td:last-child').addClass('dropfailed completed').empty().html('<span class="bi-exclamation-circle-fill text-danger"></span>');
                            if ((ctrDispenseItem + 1) == arrGoods.length) {
                                if (arrGrain.length > 0) {
                                    DispenseGrains(ctrDispenseGrain);
                                    return;
                                } else if (arrLiquid.length > 0) {
                                    DispenseLiquid(ctrDispenseLiquid);
                                    return;
                                } else {
                                    OrdersComplete();
                                    return;
                                }
                            }

                            ctrDispenseItem += 1;
                        }
                        Android.Toast('Attempt: ' + dropAttempt)
                        Android.Toast('ctrDispenseItem: ' + ctrDispenseItem)
                        DispenseItems(ctrDispenseItem);
                    }
                    else {
                        var isDroppedItemExist = false, indexDroppedItem = null, qty = CDec2(tbodyDispenseItems.find('td.tdDispensedQty').text());
                        $.grep(arrDroppedItem, function (pData, index) {
                            if (pData.ProductID == currentDroppedItem.ProductID && pData.BinProductID == currentDroppedItem.BinProductID) {
                                isDroppedItemExist = true;
                                indexDroppedItem = index;
                            }
                        })

                        if (isDroppedItemExist) {
                            arrDroppedItem[indexDroppedItem].OrderQty += currentDroppedItem.OrderQty;
                            qty += currentDroppedItem.OrderQty;
                        } else {
                            arrDroppedItem.push(currentDroppedItem);
                            qty += currentDroppedItem.OrderQty;
                        }

                        tbodyDispenseItems.removeClass('text-secondary').addClass('textvm-primary');
                        tbodyDispenseItems.find('td.tdDispensedQty').text(qty);
                        var tdstatus = tbodyDispenseItems.find('td:last-child').empty();
                        tdstatus.addClass('completed').html((tdstatus.hasClass('dropfailed') ? '<span class="bi-exclamation-circle-fill text-danger"></span>' : '<span class="bi-check-circle-fill"></span>'))
                        VendingMachine.ComputeBinProduct(currentDroppedItem);
                        VendingMachine.AddLog('Machine', 'Item dropped successfully');
                        Android.AddLog('ctrDispenseItem: ' + ctrDispenseItem);

                        if (ctrDispenseItem == arrGoods.length) {
                            $('#page_pickupitems').pageshow();
                            VendingMachine.Timer('#page_pickupitems', 30, function () {
                                UsbSerial_Send({
                                    "action": "CONTROL_DOOR",
                                    "uuid": "000078EE35E81F84",
                                    "state": false,
                                    "door": 1
                                })
                            })
                            UsbSerial_Send({
                                "action": "CONTROL_DOOR",
                                "uuid": "000078EE35E81F84",
                                "state": true,
                                "door": 1
                            })
                        } else {
                            VendingMachine.AddLog('Machine', 'Attempting to drop the next item');
                            Android.AddLog('Current Item: ' + JSON.stringify(ctrDispenseItem))
                            DispenseItems(ctrDispenseItem);
                        }
                    }
                }
                else if (requestedAction == 'DISPENSE_WEIGHT') {
                    ctrDispenseGrain -= 1;

                    if (typeof countdown != 'undefined') clearInterval(countdown);
                    if (typeof listenerInactivity != 'undefined') clearInterval(listenerInactivity);

                    if (CStr2(response.weight) != '') {
                        actualWeight = response.weight;
                        $('.lblOrderedWeight').text(arrGrain[0].Quantity + ' ' + arrGrain[0].BaseUnit);
                        $('.lblDispensingWeight').text(response.weight + ' ' + arrGrain[0].BaseUnit);
                    }

                    if (response.status == 'ERROR') {
                        dropGrainAttempt += 1;

                        if (dropGrainAttempt > 2) {
                            arrFailedDropItem.push(arrGrain[ctrDispenseGrain]);
                            ctrDispenseGrain += 1;
                            OrdersComplete();
                            return;
                        }
                        DispenseGrains(ctrDispenseGrain);
                    } else if (response.status == 'OK') {
                        GuidesOnScreenTime('Rice')
                        $('.divStep.Rice').hide();
                        $('.divStep.Rice:eq(2)').show();
                    } else if (response.status == 'WEIGH_DONE') {
                        arrGrain[0].Quantity = actualWeight;
                        arrDroppedItem.push(arrGrain[0]);
                        GuidesOnScreenTime('Rice');
                        $('.divStep.Rice').hide();
                        $('.divStep.Rice:eq(1)').show();
                    } else if (response.status == 'DOOR_CLOSED') {
                        if (arrLiquid.length > 0) {
                            DispenseLiquid(ctrDispenseLiquid);
                        } else OrdersComplete();
                    }
                }
                else if (requestedAction == 'DISPENSE_VOLUME') {
                    if (typeof countdown != 'undefined') clearInterval(countdown);
                    if (typeof listenerInactivity != 'undefined') clearInterval(listenerInactivity);

                    if (response.status == 'ERROR') {
                        dropLiquidAttempt += 1;

                        if (dropLiquidAttempt > 2) {
                            arrFailedDropItem.push(arrLiquid[ctrDispenseLiquid]);
                            ctrDispenseLiquid += 1;
                            OrdersComplete();
                            return;
                        }
                        DispenseLiquid(ctrDispenseLiquid);
                    } else if (response.volume >= 0 && response.status == undefined) {
                        $('.lblDispensingVolume').text((remainingVolumeQty + response.volume) + ' ml');
                        latestVolumeQty = response.volume;
                    } else if (response.status == 'OK') {
                        if (arrLiquid.length > 0) {
                            arrLiquid[0].Quantity = CDec2(response.volume);
                            VendingMachine.ComputeBinProduct(arrLiquid[0]);
                            $('.lblDispensingVolume').text(response.volume + ' ml');
                            isStatusBottleResponded = false;
                            currentVideoElement.pause();
                            $('#page_removebottle').pageshow();
                            VendingMachine.Timer('#page_removebottle', 30, function () {
                                DispenseLiquidState = 'dispensingliquid_done';
                                UsbSerial_Send({
                                    "action": "CONTROL_DOOR",
                                    "uuid": "000078EE35E81F84",
                                    "state": false,
                                })
                            })
                            //UsbSerial_Send({
                            //    "action": "STATUS_BOTTLE",
                            //    "uuid": '000078EE35E81F84' /*arrLiquid[0].BoardUUID*/
                            //})
                        }                        
                    }
                }
                else if (requestedAction == 'DISPENSE_STOP') {
                    Android.AddLog(requestedAction + ' Response: ' + response.status)

                    if (response.status == 'ERROR') {
                        //do nothing for now
                    } else if (response.status == 'OK') {
                        remainingVolumeQty = CDec2(arrLiquid[0].Quantity) - latestVolumeQty;
                        Android.AddLog('Remaining Volume: ' + remainingVolumeQty);
                    }
                }
                else if (requestedAction == 'CONTROL_DOOR') {
                    Android.AddLog(requestedAction + ' Response: ' + response.status)

                    if (response.status == 'ERROR') {
                        //do nothing for now
                    } else if (response.status == 'OK') {
                        if (response.state == false && (arrGoods.length > 0 && arrLiquid.length == 0)) {
                            // If closed and with a transaction, but no liquid is dispensed, then end the transaction.
                            OrdersComplete();
                            return;
                        }

                        if (response.state) {
                            //if open and with a transaction
                            if (arrGoods.length > 0) {
                                //if (arrGrain.length == 0 && arrLiquid.length == 0) {
                                //    DispenseGrains(ctrDispenseGrain);
                                //} else if (arrLiquid.length > 0) {
                                //    DispenseLiquid(ctrDispenseLiquid);
                                //} else {
                                    
                                //}
                                return
                            }

                            if (arrLiquid.length > 0) {
                                //setTimeout(function () {
                                    DispenseLiquidState = 'checking_bottle';
                                    isStatusBottleResponded = false;
                                //    UsbSerial_Send({
                                //        "action": "STATUS_BOTTLE",
                                //        "uuid": '000078EE35E81F84' /*arrLiquid[0].BoardUUID*/
                                //    })
                                //}, 5000)
                                return;
                            }
                        } else {
                            if (response.door == 1) {
                                //Goods door
                                if (arrGrain.length > 0) {
                                    DispenseGrains(ctrDispenseGrain);
                                } else if (arrLiquid.length > 0) {
                                    DispenseLiquid(ctrDispenseLiquid);
                                }
                                return;
                            }                            

                            if (DispenseLiquidState == 'dispensingliquid_done') {
                                isStatusBottleResponded = true;
                                $('#page_ordercomplete').find('h3').html('Your order is now complete.<br><br>Thank you for your purchase!')
                                OrdersComplete();
                                return;
                            }
                        }
                    }
                }
                else if (requestedAction == 'STATUS_BOTTLE') {
                    Android.AddLog(requestedAction + ' Response: ' + response.status)

                    if (response.status == 'ERROR') {
                        //do nothing for now
                    } else if (response.status == 'OK') {
                        //close sliding door

                        if (CBool(response.bottle)) {
                            if (DispenseLiquidState == 'checking_bottle') {
                                DispenseLiquidState = 'dispensing_liquid'
                                remainingVolumeQty = 0;
                                isStatusBottleResponded = true;
                                Android.AddLog('Remaining Volume: ' + CStr2(remainingVolumeQty));
                                UsbSerial_Send({
                                    "action": "DISPENSE_VOLUME",
                                    "motor": 2 /*arrLiquid[0].VDMMotorNo*/,
                                    "uuid": '000078EE35E81F84' /*arrLiquid[0].BoardUUID*/,
                                    "volume": 5, //CDec2(arrLiquid[0].Quantity)
                                })
                            } else {
                                if (arrLiquid.length > 0) {
                                    setTimeout(function () {
                                        UsbSerial_Send({
                                            "action": "STATUS_BOTTLE",
                                            "uuid": '000078EE35E81F84' /*arrLiquid[0].BoardUUID*/
                                        })
                                    }, 2000)
                                }
                            }
                        } else {
                            if (DispenseLiquidState == 'checking_bottle') {
                                setTimeout(function () {
                                    UsbSerial_Send({
                                        "action": "STATUS_BOTTLE",
                                        "uuid": '000078EE35E81F84' /*arrLiquid[0].BoardUUID*/
                                    })
                                }, 2000)
                            } else {
                                DispenseLiquidState = 'dispensingliquid_done';
                                UsbSerial_Send({
                                    "action": "CONTROL_DOOR",
                                    "uuid": "000078EE35E81F84",
                                    "state": false,
                                })
                            }
                        }
                    }
                }
                else if (requestedAction == 'STATUS_PRODUCT_DOOR') {
                    Android.AddLog(requestedAction + ' Response: ' + response.status)

                    if (response.status == 'ERROR') {
                        //do nothing for now
                    } else if (response.status == 'OK') {
                        //close product door
                        setTimeout(function () {
                            UsbSerial_Send({
                                "action": "CONTROL_DOOR",
                                "uuid": "000078EE35E81F84",
                                "state": false,
                                "door": 1,
                            })

                            if (arrGrain.length > 0) {
                                DispenseGrains(ctrDispenseGrain);
                            } else if (arrLiquid.length > 0) {
                                DispenseLiquid(ctrDispenseLiquid);
                            } else {
                                $('#page_ordercomplete').find('h3').html('Your order is now complete.<br><br />Thank you for your purchase!')
                                OrdersComplete();
                            }
                        }, 10000)
                    }
                }
                else if (requestedAction == 'CASH_CHANGE') {
                    if (response.status == 'OK') {
                        VendingMachine.AddLog('Machine', 'Payment change is successful.');
                        if (arrFailedDropItem.length > 0) {
                            $('#page_cashchangecompleted').find('h3').text('Your change and refund are ready! Please collect them from the change slot.');
                            FailedOrders();
                        } else {
                            if (isCancelDuringPayment) {
                                setTimeout(function () {
                                    NewTrans();
                                }, 2000);
                            } else {
                                $('#page_cashchangecompleted').pageshow();
                                setTimeout(function () {
                                    $('#page_ordercomplete').find('h3').text('Thank you!');
                                    $('#page_ordercomplete').pageshow();
                                    setTimeout(function () {
                                        NewTrans()
                                    }, 2000)
                                }, 3000)
                            }
                        }
                    } else {
                        var totalDenomDispensed = (CDec2(response['25_cent']) * 0.25) + (CDec2(response['1_peso']) * 1) + (CDec2(response['5_peso']) * 5);
                        var actualChange = totalCashInserted - totalAmountToPay;
                        actualTotalDispensedChange += totalDenomDispensed;

                        VendingMachine.AddLog('Machine', 'There was an error while processing the payment change. The system was unable to dispense the following denominations: ' + (CDec2(response['25_cent']) == 0 ? '0.25,' : '') +
                            (CDec2(response['1_peso']) == 0 ? ' 1,' : '') +
                            (CDec2(response['5_peso']) == 0 ? '5' : ''));

                        if (paymentChangeAttempt > 1) {
                            $('#page_cashchangeerror').pageshow();
                            $('#page_cashchangeerror').find('h3').html('We\'ve given you P' + AccountingFormat(actualTotalDispensedChange) + ' of your change so far, but we\'re still missing P' + AccountingFormat(Math.abs(actualTotalDispensedChange - actualChange)) + ' because there\'s a problem with the machine right now.')
                            return;
                        }
                        VendingMachine.AddLog('Machine', 'The machine is processing the change again');
                        paymentChangeAttempt += 1;
                        CashChange(actualChange - totalDenomDispensed, IsPaymentChangeARefund);
                    }
                }
                else if (requestedAction == 'FLUSH_COINS') {
                    if (response.status == 'OK') {
                        $('#page_flushedcoins .desc').html('<h3>The total number of P' + coin + ' coins dispensed is ' + response.qty + ' pcs.,<br>for a total of ' + AccountingFormat(CInt2(response.qty) * coin) + '.</h3>')
                    } else {
                        Android.AddLog('FLUSH COINS: ' + response.code)
                    }
                }
                else if (requestedAction == 'TURN_MOTOR') {
                    if (response.status == 'OK') {
                        $('tr[testmotorno=' + ctrTestAllMotor + ']').removeClass('text-secondary').addClass('textvm-primary').find('td:last-child').html('<span class="bi-check-circle-fill"></span>')
                    } else {
                        $('tr[testmotorno=' + ctrTestAllMotor + ']').removeClass('text-secondary').addClass('text-danger').find('td:last-child').html('<span class="bi-exclamation-circle-fill"></span>')
                    }

                    if (ctrTestAllMotor == VendingMachine.NoOfMotors) {
                        ctrTestAllMotor = 1;
                    } else {
                        ctrTestAllMotor += 1;
                        proj.TestAllMotors();
                    }

                    $(".scrollable_area").scrollTop($(".scrollable_area")[0].scrollHeight);
                }
                else if (requestedAction == 'LIST_DEVICES') {
                    arrDeviceList = response.Devices
                }
            } catch (e) {
                Android.AddLog('Error: ' + JSON.stringify(e));
            }            
        }     
    })
        .on('mousemove click touchstart touchmove', function (e) {
            if (!isUserActive) {
                isUserActive = true;

                if ($('#page_trans').css('display') == 'block') {
                    // Execute function
                    if (typeof listenerInactivity != 'undefined') clearInterval(listenerInactivity);
                    DetectInactivity();
                }                
                
                // Limit execution to once every 1s
                setTimeout(function () {
                    isUserActive = false;
                }, 1000);
            }
        })
        .on('keydown', function (e) {
            return;
            var which = e.which;
            var key = e.key;
            var cartSelectedSKU = $('#divCartItems > .selectedSkuCart');
            Android.AddLog('KEY: ' + e.key + ', WHICH: ' + which);

            if (key == undefined) {
                return
            }

            isUserInactive = false;

            if (which == 106 || key == '*') {
                //*
                ctrAccessSettings += 1;
                if (ctrAccessSettings == 2) {
                    clearTimeout(listener_opts);
                    ctrAccessSettings = 0;
                    $('#page_settings_options').pageshow();
                    return;
                }

                listener_opts = setTimeout(function () {
                    ctrAccessSettings = 0;
                }, 1000)
                return;
            }

            //if ($('#page_start').css('display') == 'block') {
            //    $('.btnInitDone').click();
            //    return;
            //}

            if ($('#page_idle').css('display') == 'block') {
                DetectInactivity(30);
                $('#page_trans, .page_menu').pageshow();
                ShowPageGuides('menu');
                VendingMachine.AddLog('Start up', 'Start')
                return;
            }

            if (DetectInactivity()) return;

            if (which == 13) {
                // ENTER
                if ($('#page_trans').css('display') == 'block') {
                    if ($('.page_menu').css('display') == 'block') {
                        var txtSelectedItemNo = $('#txtSelectedItemNo');

                        if (txtSelectedItemNo.val() != '') {
                            var sku = $('div[motorno="' + txtSelectedItemNo.val() + '"]');
                            ShowPageGuides('productdetails');
                            if (sku.length > 0) sku.click();
                            txtSelectedItemNo.val('');
                        } else {
                            if (arrOrderItems.length == 0) {
                                alert('Please add at least one item to the cart.');
                                return;
                            }
                            VendingMachine.AddLog('Menu', 'Viewing cart');
                            $('.btnViewOrder').click();
                        }
                        $('#listEmptyField').pageshow();
                        return;
                    }

                    if ($('.page_productinfo').css('display') == 'block') {
                        if (CBool(selectedProduct.IsUseUOM)) {
                            var chosenRice = [];

                            var tempOrderItems = JSON.parse(JSON.stringify(arrOrderItems));
                            for (var i in tempOrderItems) {
                                if (CBool(tempOrderItems[i].IsUseUOM) && selectedProduct.ProductID != tempOrderItems[i].ProductID) {
                                    chosenRice.push(tempOrderItems[i]);
                                    break;
                                }
                            }

                            if (chosenRice.length > 0) {
                                $('.page_updatechosenuom').pageshow();
                                $('.page_updatechosenuom h1').html('We only allow one variety of rice per transaction. If you wish to select multiple varieties, please make separate transactions.<br><br><br>Would you like to replace ' + chosenRice[0].UnitMeasure + ' ' + chosenRice[0].ProductName + ' with ' + $('#listUom').find('> div .btnUom.active:eq(0)').data('data').UnitMeasure + ' ' + selectedProduct.ProductName + '?<br><br>');
                                VendingMachine.AddLog('Product Info', 'Confirming the replacement of the chosen rice variety.');
                                return;
                            }                            
                        }

                        if (!AddToCart()) return;

                        if (isFromCart) {
                            isFromCart = false;
                            $('.btnViewOrder').click();
                        }
                        return;
                    }

                    if ($('.page_confirmation').css('display') == 'block') {
                        isFromCart = false;
                        RemoveItem();
                        VendingMachine.AddLog('Product Info', 'Item removal confirmed');
                        return;
                    }

                    if ($('.page_updatechosenuom').css('display') == 'block') {
                        VendingMachine.AddLog('Product Info', 'Confirmed to replace the chosen rice variety');
                        AddToCart();
                        $('.btnViewOrder').click();
                        return;
                    }

                    if ($('.page_Cart').css('display') == 'block') {
                        if (arrOrderItems.length == 0) {
                            alert('Please add at least one item to the cart.');
                            return;
                        }
                        $('.page_confirmpay').pageshow();
                        $('.page_confirmpay h1').text('Are you sure you want to pay now?');
                        VendingMachine.AddLog('Cart', 'Attempting to pay');
                        return;
                    }

                    if ($('.page_confirmpay').css('display') == 'block') {
                        /*alert('Pay now')*/
                        $('#btnPayNow').click();
                        return;
                    }
                }

                if ($('#page_screentimeout').css('display') == 'block') {
                    $('#page_trans').pageshow();
                    if (typeof timeoutCallback != 'undefined') {
                        timeoutCallback('continue');
                    }                   
                    return;
                }                
            }

            if (which == 27) {
                // ESC
                if ($('#page_trans').css('display') == 'block') {
                    if ($('.page_productinfo').css('display') == 'block' || $('.page_Cart').css('display') == 'block') {
                        if (isFromCart) {
                            isFromCart = false;
                            $('.btnViewOrder').click();
                            VendingMachine.AddLog('Product Info', 'Edit cancelled');
                        } else {
                            isFromCart = false;
                            $('.page_menu').pageshow();
                            VendingMachine.AddLog('Product Info', 'Back to the menu page');
                            ShowPageGuides('menu');
                            //$('#txtSelectedItemNo').val('').focus();
                        }
                        return;
                    }

                    if ($('.page_confirmpay').css('display') == 'block') {
                        VendingMachine.AddLog('Cart', 'Attempt to pay was cancelled');
                        return
                    }

                    if ($('.page_confirmation').css('display') == 'block' || $('.page_confirmpay').css('display') == 'block') {
                        isFromCart = false;
                        $('.btnViewOrder').click();
                        return;
                    }

                    if ($('.page_menu').css('display') == 'block') {
                        $('#txtSelectedItemNo').val('');
                        $('#listEmptyField').pageshow();
                        VendingMachine.AddLog('Menu', 'Clear the encoded item code');
                        return;
                    }

                    if ($('.page_updatechosenuom').css('display') == 'block') {
                        $('.page_productinfo').pageshow();
                        ShowPageGuides('productdetails');
                        VendingMachine.AddLog('Product Info', 'The replacement of the rice variety was cancelled');
                        return;
                    }
                }

                if ($('#page_screentimeout').css('display') == 'block') {                    
                    $('#page_cancelledtrans').pageshow();
                    setTimeout(function () {
                        //$('#page_idle').pageshow();
                        if (typeof timeoutCallback != 'undefined') {
                            timeoutCallback();
                        }
                    }, 2000)                    
                    return;
                }

                if ($('#page_settings_pin').css('display') == 'block' || $('#page_testallmotors').css('display') == 'block') {
                    $('#page_settings_options').pageshow();
                    return;
                }

                if ($('#page_settings_options').css('display') == 'block') {
                    ctrAccessSettings = 0;
                    if (arrOrderItems.length > 0) {
                        $('#page_trans').pageshow();
                    } else {
                        $('#page_idle').pageshow();
                        slides = document.querySelector('.slidesLandscape');
                        slideItems = $('.slidesLandscape > li') //document.querySelectorAll('.ads_Landscape');
                        goToSlide(0);
                    }
                    return;
                }

                if ($('#page_flushedcoins').css('display') == 'block' || $('#page_inventory').css('display') == 'block') {
                    $('#page_settings_options').pageshow();
                    return;
                }
            }

            if (which == 33 || which == 38) {
                //PAGE UP
                if ($('#page_trans').css('display') == 'block') {
                    if ($('.page_menu').css('display') == 'block') {
                        var curshowmenu = $('.curshowmenu');
                        var nextrowmenu = curshowmenu.prev();
                        if (nextrowmenu.length > 0) {
                            nextrowmenu.get(0).scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'center' });
                            curshowmenu.removeClass('curshowmenu');
                            nextrowmenu.addClass('curshowmenu');
                        }
                        ScrollIndicator();
                        VendingMachine.AddLog('Menu', 'Page Up');
                        return;
                    }

                    if ($('.page_Cart').css('display') == 'block') {
                        if (cartSelectedSKU.prev().length > 0) {
                            cartSelectedSKU.removeClass('selectedSkuCart');
                            cartSelectedSKU.prev().addClass('selectedSkuCart');
                            document.getElementById($('#divCartItems > .selectedSkuCart').attr('id')).scrollIntoView({ behavior: "smooth", block: "center", inline: "center" });
                            VendingMachine.AddLog('Cart', 'Page Up');
                        }

                        return;
                    }

                    if ($('.page_cartaction').css('display') == 'block') {
                        isFromCart = true;
                        ModifyProduct(selectedProduct);
                    }
                }                

                if ($('#page_guides').css('display') == 'block') {
                    var dv = $('.divStep:visible').prev();
                    clearTimeout(timerGuides);
                    if (dv.length > 0 && dv.hasClass('divStep')) {
                        $('#' + dv.attr('id')).pageshow();
                        GuidesOnScreenTime(dv.data('data'));
                        VendingMachine.AddLog('Rice Dispensing Guides', 'Page Up');
                    }
                    return;
                }

                if ($('#page_testallmotors').css('display') == 'block') {
                    document.getElementById("scrollable_area").scrollBy({ top: -50, behavior: 'smooth' });
                }

                if ($('#page_settings_options').css('display') == 'block') {
                    document.getElementById("page_settings_options").querySelector('.overflow-auto').scrollBy({ top: -50, behavior: 'smooth' });
                }

                if ($('#page_inventory').css('display') == 'block') {
                    document.getElementById("page_inventory").querySelector('.scrollable_area').scrollBy({ top: -50, behavior: 'smooth' });
                }
            }

            if (which == 34 || which == 40) {
                //PAGE DOWN
                if ($('#page_trans').css('display') == 'block') {
                    if ($('.page_menu').css('display') == 'block') {
                        var curshowmenu = $('.curshowmenu');
                        var nextrowmenu = curshowmenu.next();
                        if (nextrowmenu.length > 0) {
                            nextrowmenu.get(0).scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'center' });
                            curshowmenu.removeClass('curshowmenu');
                            nextrowmenu.addClass('curshowmenu');
                        }
                        ScrollIndicator();
                        VendingMachine.AddLog('Menu', 'Page Down');
                        return;
                    }

                    if ($('.page_Cart').css('display') == 'block') {
                        if (cartSelectedSKU.next().length > 0) {
                            cartSelectedSKU.removeClass('selectedSkuCart');
                            cartSelectedSKU.next().addClass('selectedSkuCart');
                            document.getElementById($('#divCartItems > .selectedSkuCart').attr('id')).scrollIntoView({ behavior: "smooth", block: "center", inline: "center" });
                            VendingMachine.AddLog('Cart', 'Page Down');
                        }

                        return;
                    }

                    if ($('.page_cartaction').css('display') == 'block') {
                        cartSelectedSKU.find('.btnRemove').click();
                    }
                }                

                if ($('#page_guides').css('display') == 'block') {
                    var dv = $('.divStep:visible').next();
                    clearTimeout(timerGuides);
                    if (dv.length > 0 && dv.hasClass('divStep')) {
                        $('#' + dv.attr('id')).pageshow();
                        GuidesOnScreenTime(dv.data('data'));
                        VendingMachine.AddLog('Rice Dispensing Guides', 'Page Down');
                    }
                    return;
                }

                if ($('#page_testallmotors').css('display') == 'block') {
                    document.getElementById("scrollable_area").scrollBy({ top: 50, behavior: 'smooth' });
                }

                if ($('#page_settings_options').css('display') == 'block') {
                    document.getElementById("page_settings_options").querySelector('.overflow-auto').scrollBy({ top: 50, behavior: 'smooth' });
                }

                if ($('#page_inventory').css('display') == 'block') {
                    document.getElementById("page_inventory").querySelector('.scrollable_area').scrollBy({ top: 50, behavior: 'smooth' });
                }
            }

            if (which == 8) {
                //CLEAR
                if ($('#page_trans').css('display') == 'block') {
                    if ($('.page_Cart').css('display') == 'block') {
                        cartSelectedSKU.find('.btnRemove').click();
                        return;
                    }
                }                
            }

            if (which == 96 || which == 48) {
                //0
                if ($('#page_trans').css('display') == 'block') {
                    if ($('.page_productinfo').css('display') == 'block' && $('.lblToRemoveItem').css('display') == 'inline' && !inputDelay) {
                        cartSelectedSKU.find('.btnRemove').click();
                        return;
                    }
                }
            }

            if ((which >= 48 && which <= 57) || (which >= 96 && which <= 105)) {
                //0-9
                if ($('#page_trans').css('display') == 'block') {
                    if ($('.page_menu').css('display') == 'block') {
                        var input = $('.page_menu input[type="text"]');
                        input.val(input.val() + key);
                        $('#listNotEmptyField').pageshow();
                        VendingMachine.AddLog('Menu', 'Encoding item number');
                        return;
                    }

                    if ($('.page_productinfo').css('display') == 'block') {                        
                        if ($('#divUomSelection').css('display') == 'block') {
                            var weight = $('#listUom').find('> div[uomno="' + key + '"] .btnUom');
                            $('#listUom .btnUom').removeClass('active light-beam');
                            if (weight.length > 0) {                                
                                weight.addClass('active');
                            } else {
                                $('#listUom .btnUom').addClass('light-beam');
                            }
                            VendingMachine.AddLog('Product Info', 'Selecting rice quantity');
                        } else {
                            var input = $('.page_productinfo input[type="text"]');
                            if (!inputDelay) {
                                input.val(key)
                            } else {
                                input.val(input.val() + key)
                            }
                            inputDelay = true;
                            var inpTimeout = setTimeout(function () {
                                inputDelay = false;
                                clearTimeout(inpTimeout);
                            }, 1000)
                            VendingMachine.AddLog('Product Info', 'Encoding item quantity');
                        }                        
                        return;
                    }

                    if ($('.page_Cart').css('display') == 'block') {
                        $('#divCartItems > .selectedSkuCart').removeClass('selectedSkuCart');
                        selectedProduct = $('#divCartItems').find('> div[cartlineno="' + key + '"]').addClass('selectedSkuCart').data('data');
                        isFromCart = true;
                        ModifyProduct(selectedProduct);
                        $('.lblToRemoveItem').show();
                        VendingMachine.AddLog('Cart', 'Selecting an item to edit');
                        return;
                    }
                }          

                if ($('#page_settings_pin').css('display') == 'block') {
                    $('#lblPinMsg').hide();
                    var pinfield = $('.pin.active');

                    if (pinfield.length == 0) {
                        $('.pin:eq(0)').addClass('active');
                        pinfield = $('.pin:eq(0)').find('input');
                    } else {
                        pinfield.removeClass('active');
                        pinfield.next().addClass('active');
                        pinfield = pinfield.next().find('input');
                    }

                    pinfield.val(key);

                    if ($('.pin:last-child input').val() != '') {
                        setTimeout(function () {
                            var pin = '';
                            $('.pin').find('input').each(function name(params) {
                                pin += $(this).val();
                            })
                            var pinres = VendingMachine.CheckPasscode(adminOption.Access, pin);
                            $('#page_settings_pin input').val('');
                            $('.pin').removeClass('active');
                            if (pinres == 'allowed') {
                                $('#page_settings_options').pageshow();
                                adminOption.Callback();
                                adminOption = null;
                            } else if (pinres == 'no access') {
                                $('#lblPinMsg').text('Sorry! You don\'t have access.').show();
                            } else {
                                $('#lblPinMsg').text('Invalid PIN. Please try again.').show();
                            }
                        }, 1000)
                    }
                }

                if ($('#page_settings_options').css('display') == 'block') {
                    if (!inputDelay) {
                        optCode = key
                    } else {
                        optCode += key
                    }
                    inputDelay = true;
                    var inpTimeout = setTimeout(function () {
                        inputDelay = false;
                        clearTimeout(inpTimeout);

                        var optLi = $('li[optno="' + optCode + '"]');

                        if (optLi.length > 0) {
                            adminOption = optLi.data('data');
                            $('#page_settings_pin h2').text('Action: ' + adminOption.Label);
                            $('#page_settings_pin').pageshow();
                            $('#page_settings_pin input').val('');
                        }

                        optCode = '';                        
                    }, 1000)
                }
            }
        })
})