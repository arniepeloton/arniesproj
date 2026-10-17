require([
    'common',
    '/Scripts3/Browser/LibraryPIMProduct.js?crc=',
    '/Scripts3/Stock_offline.js?crc=',
    '/Scripts3/Offline/Browser/LibraryOrganization.js?crc=',
    '/Scripts3/Offline/Browser/TransInvoices.js?crc=',
    (OfflineMode ? '/Scripts3/SalesMenu_offline.js?crc=' : ''),
    (OfflineMode ? '/Scripts3/ItemModifiers.js?crc=' : '')
], function () {
    $(document).ready(function () {
        NewMessageBox('Please be reminded that some shortcut keys are not functioning in some android OS, you may click the shortcut keys at the bottom.', 'Reminder');

        window.FmcgBasicCart = {
            isLineItemDisplay: true,
            isRealTimePrint: true,
            elem: null,
            txtQtyPlu: null,
            txtQtyPluAction: 'plu',
            divListOfPlu: null,
            currentQty: 1,
            CartTotal: 0,
            CartTotalQty: 0,
            FinalAmountDue: 0,
            intProductSerialCntr: 1,            
            totalProdQty: 0,
            productData: null,
            ulSelectedProduct: null,
            isVoidActive: false,
            isValidToVoid: false,
            isFromRecallCart: false,
            arrCartItems: new Array(),
            PrintHeader: false,
            justPrintItems: false,
            ItemToPrint: new Array(),
            isEditModifier: false,
            isGetTax: false,
            Clear: function(){
                this.txtQtyPluAction = 'plu';
                this.currentQty = 1;
                this.CartTotal = 0;
                this.CartTotalQty = 0;
                this.FinalAmountDue = 0;
                this.arrCartItems = new Array();
                this.PrintHeader = false;
                this.justPrintItems = false;
                this.ItemToPrint = new Array();
                this.isEditModifier = false;
                this.isGetTax = false;
                $('#txtTotalQty').text(0)
            },
            init: function (elem) {                
                this.elem = elem
                this.txtQtyPlu = elem.find('[name="txtQtyPlu"]')
                this.ulSelectedProduct = elem.find('#ulSelectedProduct')
                this.divListOfPlu = elem.find('#divListOfPlu')

                setInterval(function () {
                    $('#lblCurrDateTime').text($.format.date(new Date(), 'MM-dd-yyyy  hh:mm:ss a'))
                }, 1000)
                Popup.Discount.initElements()
                var pSql = Sql.Execute('SELECT DefaultValue FROM private.SystemDefaults WHERE Description = \'IsLineItemDisplay\'').Rows;
                this.isLineItemDisplay = CBool(pSql.length > 0 ? pSql[0].DefaultValue : 1);
                //pSql = Sql.Execute('SELECT DefaultValue FROM private.SystemDefaults WHERE Description = \'RealTimePrintReceipt\'').Rows; commented by arnie (May 2,17) default printing is by batch
                this.isRealTimePrint = false; //CBool(pSql.length > 0 ? pSql[0].DefaultValue : 1); to be fixed; for now (May 2,17) default printing is by batch

                if (this.isLineItemDisplay) {
                    $('#tblHeaderForLineDisplay').show();
                } else {
                    $('#tblHeaderForLineDisplay').hide();
                }

                this.txtQtyPlu.focusout(function () {
                    if (CStr2(Popup.popupname) == '') { //Popup.popupname must be blank to avoid error "Maximum call stack size exceeded"
                        $(this).focus();
                    }
                });

                this.txtQtyPlu.on('keydown', function (e) {
                    var dis = $(this);
                    $('#txtMessage').html('&nbsp;') // clear message

                    if(CBool(sessionStorage.IsNewCart)){
                        //after saving of cart
                        NewCart(); // fmcg.js                        
                    }

                    if(e.which == 35){
                        //press END
                        KeyEnd(dis);
                    }
                    else if (e.which == 34) {
                        //press Page down
                        KeyPageDown(dis);
                    }

                    if(e.which == 13 && $.trim(dis.val()) != ""){
                        //press enter
                        if (FmcgBasicCart.txtQtyPluAction == 'plu') {
                            FmcgBasicCart.isEditModifier = false;
                            $('.lblCurrentProdQty2').text($('.lblCurrentProdQty').text());
                            FmcgBasicCart.SearchProduct(dis.val());
                            dis.val('');                            
                            $('#divPLU').pageshow();
                        }
                        else if (FmcgBasicCart.txtQtyPluAction == 'void') {
                            if (!$.isNumeric(dis.val())) {
                                dis.val('');
                                BlinkMessage('Void Qty must be a number')
                                return false;
                            }
                            if (FmcgBasicCart.isValidToVoid) {
                                if ($.trim(dis.val()) == "") {
                                    BlinkMessage('Enter no of qty to void')
                                    return;
                                }
                                FmcgBasicCart.isEditModifier = true;
                                PromptUserAccess('Allow removal of cart items', {
                                    OnClickOKSuccess: function () {
                                        $('#randomSecID').dialog('close');
                                        if (FmcgBasicCart.ComputeVoid(dis.val(), FmcgBasicCart.ulSelectedProduct.find('.void'))) {
                                            FmcgBasicCart.txtQtyPluAction = 'plu';
                                            dis.attr('placeholder', 'Enter PLU');
                                            $('#divPLU').pageshow();
                                            dis.val('');                                            
                                        }
                                    },
                                    OnFailed: function () {
                                        Android.AddLog("Error in voiding an item");
                                    }
                                });                                
                            }
                            else {
                                BlinkMessage('Select a valid PLU');
                            }                            
                        }
                        else if (FmcgBasicCart.txtQtyPluAction == 'priceverifier') {
                            if(FmcgBasicCart.txtQtyPlu.val() == ""){
                                BlinkMessage('Nothing to search price');
                            }
                            else {                                
                                FmcgBasicCart.PriceVerifier();
                            }
                        }
                    }
                })                
            },
            SearchProduct: function (plu) {                
                if (OfflineMode) {
                    var vRows = OfflineBrowser.LibraryPIMProduct.GetData({ keywords: plu }).Rows;
                    if (vRows.length == 1) {                        
                        this.ValidateStock(vRows)
                    }
                    else if(vRows.length > 1){
                        BrowsePopup('LibraryPIMProduct', 'Select Item', 
                            [
                                {
                                    Search: plu,
                                    listData: { Results: { Rows: vRows } },
                                    DisplayText: [
                                        {
                                            Style: {
                                                width: '100px'
                                            }, Label: 'Product #', Column: 'ProductNo'
                                        },
                                        {
                                            Style: {
                                                width: '300px',
                                            }, Label: 'Description', Column: 'ProductName'
                                        },
                                        {
                                            Style: {
                                                width: '100px',
                                                textAlign: 'right'
                                            }, Label: 'Price', Column: function (e) {
                                            return AccountingFormat(e.DefaultPrice);
                                            }
                                        }
                                    ],
                                    Click: function (e) {
                                        FmcgBasicCart.txtQtyPlu.focus();
                                        FmcgBasicCart.ValidateStock([e]);
                                    },
                                    Offline: true,
                                    StyleFormat: 'table',
                                    OrderBy: [1, "asc"], // for display table only
                                    AllowEmptySearch: false
                                }
                            ]
                        )
                    }
                    else {
                        BlinkMessage('No Product(s) found');                        
                    }
                }
                else {
                    AjaxSend({
                        url: 'Browser/LibraryPIMProduct',
                        data: { keywords: plu },
                        success: function (vData) {
                            var vRows = vData.Results.Rows;

                            if (vRows.length > 0) {
                                vRows[0].Qty = this.currentQty;
                                FMCGBasic.GetStockInfo(vRows, false, false)
                                this.currentQty = 1;
                            }
                            else {
                                BlinkMessage('No Product(s) found')
                            }
                        }
                    })
                }
            },
            ValidateStock: function (vRows, isFromItemModifier) {
                var resStockInfo; //isFromItemModifier used in GetStockInfo when sku has modifier
                if ($.isEmptyObject(isFromItemModifier)) {
                    vRows[0].Qty = this.currentQty;
                    resStockInfo = this.GetStockInfo(vRows, false, false);
                } else {
                    resStockInfo = isFromItemModifier;
                }
                
                if (CBool(resStockInfo) != false || $.isEmptyObject(resStockInfo) == false) {
                    this.arrCartItems.push(resStockInfo[0]);
                    this.DisplayToCart(resStockInfo)
                    this.currentQty = 1;
                    if (!this.isGetTax) {
                        Popup.Discount.GetTaxPercent();
                        this.isGetTax = true;
                    }
                    //
                    //if (CDec2($('#txtLessDiscount').text()) != 0) { // if there's a discount
                        Popup.Discount.RefreshTotalDiscount();
                    //} else {
                        Popup.ComputePayment() // Compute Payment fmcgpopup.js
                    //}
                    $('#txtMessage').text('')
                    $('.lblCurrentProdQty').text('1');
                    if (this.isRealTimePrint) {
                        PrintSalesInvoiceItems(resStockInfo, false);
                        this.justPrintItems = true;
                        this.ItemToPrint = resStockInfo;
                        PrintReceiptNow(this.PrintHeader, true);
                        this.PrintHeader = false;
                    }                    
                }
            },
            GetStockInfo: function (vData, isFromVoid, isPriceCheck) {
                var pData = {}
                var pSalesRepIDs;
                var pProductSerial;
                var intIndex;

                pData = {
                    ProductID: vData[0].ID,
                    ModifierGroupID: proj.ModifierGroupID,
                    CustomerID: sessionStorage.CustomerOrganizationID,
                    OrderID: sessionStorage.CartOrderID,
                    NewCartMode: localStorage.NewCartMode,
                    PosMode: proj.PosMode ? "pos" : "so" //additional by janine sept 5 2014 for sales order taker
                }

                var arrGetInfoPos = Stock_offline().GetInfoPos(pData);

                if (arrGetInfoPos.Error != undefined) {
                    BlinkMessage('No Item was found')
                    return false;
                }

                if (sessionStorage.FowstID == undefined || sessionStorage.FowstID == 'undefined') {
                    BlinkMessage('Please select a FOWST.');
                    return false;
                }
                else {

                    //if (CBool(arrGetInfoPos.HasModifier)) {
                    //    NewItemModifier({
                    //        EditModifier: FmcgBasicCart.isEditModifier,
                    //        AllowSKUQty: true,
                    //        ProductData: vData[0],
                    //        success: function (e) {
                    //            e.HasModifier = CBool(arrGetInfoPos.HasModifier);
                    //            e.GroupIdOfItemModifiers = NewID();
                    //            e.Qty = e.Quantity;
                    //            FmcgBasicCart.ValidateStock(null, FmcgBasicCart.SaveItem(arrGetInfoPos, [e], isFromVoid, isPriceCheck));
                    //        }
                    //    });
                    //} else {
                        return this.SaveItem(arrGetInfoPos, vData, isFromVoid, isPriceCheck);
                    //}
                }
            },
            SaveItem: function (arrGetInfoPos, vData, isFromVoid, isPriceCheck) {
                this.LoadPOSProductSerial(arrGetInfoPos.ProductSerial.Rows);
                this.LoadPOSSalesRep(arrGetInfoPos.SalesReps.Rows);

                var pData = {}

                if (proj.ProductID != undefined)
                    pStockID = proj.ProductID;
                else {
                    pParams = GetUrlParameters();
                    pStockID = pParams.id;
                };

                pData.PosMode = proj.PosMode ? "pos" : "so"; //additional by janine sept 5 2014 for sales order taker

                pData.StockID = vData[0].ID;
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
                pData.ProductID = vData[0].ID;
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
                pData.IsVATExempt = "0";
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

                var totalModifier = 0;
                if (vData[0].ItemModifier != undefined) {
                    for (var i = 0; i < vData[0].ItemModifier.length; i++) {
                        for (var m = 0; m < vData[0].ItemModifier[i].SelectedModifier.length; m++) {
                            if (vData[0].ItemModifier[i].Type == 'Inventory Affecting') {
                                totalModifier += CDec2(vData[0].Quantity) * CDec2(vData[0].ItemModifier[i].SelectedModifier[m].OrderQty) * CDec2(vData[0].ItemModifier[i].SelectedModifier[m].UnitPrice);
                            }
                        }
                    }
                }

                pData.Stocks[0] = {
                    Qty: (isFromVoid ? Math.abs(vData[0].Qty) : this.CountDuplicateProduct([vData[0]], isFromVoid)),
                    UnitQty: arrGetInfoPos.UnitMeasures.Rows[0].Quantity,
                    UnitPrice: CDec2(arrGetInfoPos.UnitMeasures.Rows[0].DiscountedPrice),
                    PriceLevelID: arrGetInfoPos.UnitMeasures.Rows[0].PriceLevelID,
                    UnitMeasure: CStr2(arrGetInfoPos.UnitMeasures.Rows[0].UnitMeasure),
                    ItemModifier: vData[0].ItemModifier,
                    GroupIdOfItemModifiers: vData[0].GroupIdOfItemModifiers,
                    TotalModifier: totalModifier
                };

                pProductSerial = [];
                intIndex = 0;

                for (var i = 0; i <= this.intProductSerialCntr; i++) {

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

                var pResult = Stock_offline().UpdatePos(pData);
                var orderID = pResult.OrderID;
                if (sessionStorage.CartOrderID == undefined) {
                    sessionStorage.CartOrderID = orderID;
                }
                if (pResult.TransNum != null)
                    $('[name=divTransNum]').text(pResult.TransNum);
                if (localStorage.NewCartMode != undefined && !isPriceCheck) {
                    if (this.isRealTimePrint) {
                        PrintSalesInvoiceHeader();
                        this.PrintHeader = true;
                    }
                }

                localStorage.removeItem('NewCartMode');
                vData[0].DefaultPrice = CDec2(pData.Stocks[0].UnitPrice);
                return vData;
            },
            DisplayToCart: function (vRows) {
                this.totalProdQty = (this.ulSelectedProduct.find('li[prodid="' + vRows[0].ID + '"]:first').attr('totalqty') || vRows[0].Qty) //this.CountDuplicateProduct(vRows);
                var display, modifierTotal = 0;
                if (this.isLineItemDisplay) {
                    $("#ulSelectedProduct").addClass('lineitemdisplay');
                    //for LINE ITEM DISPLAY
                    display = $('<table border="0" cellpadding="3" cellspacing="0" style="width:100%; border:0px solid #FFF; table-layout:fixed;">')
                        .append($('<tbody>')
                            .append($('<tr>')
                                .append($('<td class="txtPLUNo" style="width:130px; word-break:break-all;">').text(vRows[0].ProductNo))
                                .append($('<td class="txtProductName">').text(vRows[0].ProductName))
                                .append($('<td class="txt-center txtPLUQty" style="width:50px">').text(vRows[0].Qty))
                                .append($('<td class="txt-center txtPLUPrice" style="width:100px">').text(AccountingFormat((OfflineMode ? CDec2(vRows[0].DefaultPrice) : CDec2(vRows[0].RetailPrice)))))
                                .append($('<td class="txt-center txtPLUItemTotal" style="width:100px">').text(AccountingFormat(CDec2(vRows[0].Qty) * (OfflineMode ? CDec2(vRows[0].DefaultPrice) : CDec2(vRows[0].RetailPrice)))))
                            )
                        )
                } else {
                    $("#ulSelectedProduct").removeClass('lineitemdisplay');
                    //for JOURNAL ENTRY DISPLAY
                    display = $('<table border="0" cellpadding="3" cellspacing="0" style="width:100%; border:0px solid #FFF;">')
                        .append($('<tbody>')
                            .append($('<tr>')
                                .append($('<td colspan="3" class="txtPLUNo">').text('PLU : ' + vRows[0].ProductNo))
                            )
                            .append($('<tr>')
                                .append($('<td colspan="3">').text(vRows[0].ProductName))
                            )
                            .append($('<tr>')
                                .append($('<td colspan="2" style="padding-left:20px;">').html(vRows[0].Qty + ' x <span class="txtPLUPrice">' + AccountingFormat((OfflineMode ? CDec2(vRows[0].DefaultPrice) : CDec2(vRows[0].RetailPrice))) + '</span>'))
                                .append($('<td style="text-align:right">').text(AccountingFormat(CDec2(vRows[0].Qty) * (OfflineMode ? CDec2(vRows[0].DefaultPrice) : CDec2(vRows[0].RetailPrice)))))
                            )
                        )
                }

                if(CBool(vRows[0].HasModifier)){
                    for (var i = 0; i < vRows[0].ItemModifier.length; i++){
                        for (var m = 0; m < vRows[0].ItemModifier[i].SelectedModifier.length; m++){
                            var mod = vRows[0].ItemModifier[i].SelectedModifier[m];
                            var modifierSubTotal = CDec2(mod.UnitPrice) * CDec2(mod.OrderQty) * CDec2(vRows[0].Qty);
                            modifierTotal += modifierSubTotal;
                            if (this.isLineItemDisplay) {
                                //display.find('.txtProductName')
                                //    .append('<br><br>').append('&nbsp; &nbsp; &raquo; ' + mod.ProductName)

                                var row = display.find('tbody')
                                            .append($('<tr class="rowim-'+ m +'">')
                                                .append($('<td>')
                                                    .text('')
                                                )
                                                .append($('<td>')
                                                    .append('&nbsp; &nbsp; &raquo; ' + (mod.ProductName || mod.Description))
                                                )
                                            )

                                if (vRows[0].ItemModifier[i].Type == "Inventory Affecting") {
                                    row.find('.rowim-' + m)
                                        .append($('<td class="txt-center">')
                                            .text(vRows[0].Qty + ' x ' + mod.OrderQty)
                                        )
                                        .append($('<td class="txt-center">')
                                            .text(AccountingFormat(mod.UnitPrice))
                                        )
                                        .append($('<td class="txt-center">')
                                            .text(AccountingFormat(modifierSubTotal))
                                        )
                                    //display.find('.txtPLUQty')
                                    //    .append('<br><br>').append(vRows[0].Qty + ' x ' + mod.OrderQty)

                                    //display.find('.txtPLUPrice')
                                    //    .append('<br><br>').append(AccountingFormat(mod.UnitPrice))

                                    //display.find('.txtPLUItemTotal')
                                    //    .append('<br><br>').append(AccountingFormat(modifierSubTotal))
                                }                                
                            } else {
                                var row = display.find('tbody')
                                    .append($('<tr>')
                                        .append($('<td colspan="3">')
                                            .append('&nbsp; &nbsp; &raquo; ' + (mod.ProductName || mod.Description))
                                        )
                                    )

                                if (vRows[0].ItemModifier[i].Type == "Inventory Affecting") {
                                    row.append($('<tr>')
                                        .append($('<td colspan="2">')
                                            .append('&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; ' + vRows[0].Qty + ' x ' + mod.OrderQty + ' x ' + AccountingFormat(mod.UnitPrice))
                                        )
                                        .append($('<td style="text-align:right;">')
                                            .append(AccountingFormat(modifierSubTotal))
                                        )
                                    )
                                }
                            }
                        }
                    }
                }

                this.ulSelectedProduct.append($('<li>')
                    .attr('prodid', vRows[0].ID)
                    .attr('totalqty', this.totalProdQty)
                    .attr('id', vRows[0].ID + '-' + (this.CountProductRow(vRows) + 1))
                    .addClass('plu')
                    .data(vRows[0])
                    .append(display)
                    .click(function (e) {
                        if (FmcgBasicCart.isVoidActive) {
                            FmcgBasicCart.Void('click', $(this).index());
                        }
                    })
                )

                this.CartTotal = CDec2(this.CartTotal + (CDec2(vRows[0].Qty) * (OfflineMode ? CDec2(vRows[0].DefaultPrice) : CDec2(vRows[0].RetailPrice))) + modifierTotal)
                this.CartTotalQty = this.CartTotalQty + CDec2(vRows[0].Qty);
                Popup.Discount.dblSubTotal = this.CartTotal;
                $('#txtTotalQty').text(CInt2(this.CartTotalQty));
                $('#txtSubTotal').text((this.CartTotal == 0 ? '0.00' : AccountingFormat(this.CartTotal)));
                //$('#txtTotal').text(AccountingFormat(CDec2($('#txtSubTotal').text()) - CDec2($('#txtLessDiscount').text())));
                $('.lblCurrentProdTotal').text(AccountingFormat(CDec2(vRows[0].Qty) * (OfflineMode ? CDec2(vRows[0].DefaultPrice) : CDec2(vRows[0].RetailPrice))))
                                
                $('#divPayment').scrollTop(-$('#divPayment').height())
                this.txtQtyPlu.focus();
                this.ScrollDownCart();
            },
            CountDuplicateProduct: function (vRows) {
                // this is to count all the same punched items
                var arrexistedProd = [];
                var existedProd = [];
                productData = this.ulSelectedProduct.find('li[prodid="' + vRows[0].ID + '"]');
                totalProdQty = 0;

                this.ulSelectedProduct.find('li').each(function () {
                    arrexistedProd.push($(this).data())
                })

                existedProd = $.grep(arrexistedProd, function (e) {
                    return e.ID == vRows[0].ID
                })

                if (existedProd.length > 0) {
                    totalProdQty = CDec2(productData.first().attr('totalqty')) + CDec2(vRows[0].Qty);
                    productData.attr('totalqty', totalProdQty); // update total qty                
                }
                else {
                    totalProdQty = vRows[0].Qty
                }

                return totalProdQty;
            },
            CountProductRow: function(vRows){
                return this.ulSelectedProduct.find('li[prodid="' + vRows[0].ID + '"]').length;
            },
            LoadPOSProductSerial: function (vRows) {
                for (var p = 0; p <= vRows.length - 1; p++) {

                    arProductSerial = new Array();
                    arProductSerial[intProductSerialRecID] = vRows[p].ID;
                    arProductSerial[intProductSerial] = vRows[p].SerialNumber;
                    arProductSerial[intProductSerialState] = "New";

                    sessionStorage["POSProductSerial-" + p] = JSON.stringify(arProductSerial);

                };

                this.intProductSerialCntr = vRows.length;
            },
            LoadPOSSalesRep: function (vRows) {
                for (var p = 0; p <= vRows.length - 1; p++) {

                    arSalesRep = new Array();
                    arSalesRep[intSalesRepRecID] = vRows[p].ID;
                    arSalesRep[intSalesRepID] = vRows[p].PeopleID;
                    arSalesRep[intSalesRepName] = vRows[p].PeopleName;
                    arSalesRep[intSalesRepState] = "New";

                    sessionStorage["POSSalesRep-" + p] = JSON.stringify(arSalesRep);

                };

                this.intSalesRepCntr = vRows.length;
            },
            SubTotal: function () {
                if (this.ulSelectedProduct.find('li').last().hasClass('subtotal') == false) {
                    this.ulSelectedProduct.append($('<li>')
                            .addClass('subtotal')
                            .append($('<table border="0" cellpadding="3" cellspacing="0" style="width:100%; border:0px solid #FFF;">')
                                .append($('<tbody>')
                                    .append($('<tr>')
                                        .append($('<td colspan="3" style="text-align:right;">').text('- - - - - - - -'))
                                    )
                                    .append($('<tr>')
                                        .append($('<td style="padding-left:20px; width:100px;">').text(this.CartTotalQty + ' Unit(s)'))
                                        .append($('<td style="text-align:center;">').text('Subtotal -->'))
                                        .append($('<td style="text-align:right; width:100px;">').text(this.CartTotal == 0 ? '0.00' : AccountingFormat(this.CartTotal))
                                    )
                                )
                            )
                        )
                    )
                    this.ScrollDownCart();
                }
            },
            Void: function (dir, ind) {
                //ind is use only when VOID is active and when user CLICK on the list
                var lastVoidProductSelected = this.ulSelectedProduct.find('li.void');
                var curVoidIndex = this.ulSelectedProduct.find('.void').index();
                var newSelectedProductToVoid = this.ulSelectedProduct.find('li:not(.subtotal):not(.voided)');

                if (this.isVoidActive) {
                    FmcgBasicCart.txtQtyPluAction = 'void';
                    $('#divVoid').pageshow();

                    if (this.ulSelectedProduct.find('li.plu').length == 0) {
                        return; // do not navigate if there is only 1 item in the cart
                    }

                    lastVoidProductSelected.removeClass('void');

                    if (dir == 'up') {
                        var isListAVoid = newSelectedProductToVoid.eq(curVoidIndex - 1);
                        if (isListAVoid.length == 0) {
                            //if 0 probably its a void or subtotal row
                            //move to next valid plu
                            newSelectedProductToVoid.eq(-2).addClass('void')
                        }
                        else {
                            isListAVoid.addClass('void')
                        }
                    }
                    else if (dir == 'down') {
                        if (lastVoidProductSelected.is('li:last-child')) {
                            newSelectedProductToVoid.eq(0).addClass('void') // go to first PLU
                        }
                        else {
                            newSelectedProductToVoid.eq(curVoidIndex + 1).addClass('void')
                        }
                    }
                    else if(dir == 'click'){
                        newSelectedProductToVoid.eq(ind).addClass('void')
                    }
                    else {
                        this.isVoidActive = false;
                        this.txtQtyPluAction = 'plu';
                        this.txtQtyPlu.attr('placeholder', 'Enter PLU');
                        $('#divPLU').pageshow();
                    }
                }
                else {
                    if(sessionStorage.CartOrderID == undefined){
                        BlinkMessage('Nothing to void. Cart is empty.')
                    } else {
                        newSelectedProductToVoid.last().addClass('void');
                        this.isVoidActive = true;
                        this.txtQtyPluAction = 'void';
                        this.txtQtyPlu.attr('placeholder', 'Enter Void Qty');
                        $('#divVoid').pageshow();
                    }                    
                }
                this.isValidToVoid = this.ulSelectedProduct.find('li.void').hasClass('plu'); // check if list is a PLU
                if (this.ulSelectedProduct.find('li.void').length > 0) {
                    //this.divListOfPlu.scrollTop(this.ulSelectedProduct.find('li.void').offset().top)
                    ScrollToMiddle('#divListOfPlu', this.ulSelectedProduct.find('li.void'))
                }
            },
            ComputeVoid: function (voidqty, selecteditemtovoid) {
                var productData = this.ulSelectedProduct.find('.void');
                var ItemPLUToVoid = productData.find('.txtPLUNo').text();
                var ItemPriceToVoid = productData.find('.txtPLUPrice').text();

                if (CDec2(productData.attr('totalqty')) == 0) {
                    BlinkMessage('Nothing to void');
                    return;
                }
                else if (CDec2(productData.attr('totalqty')) < voidqty) {
                    BlinkMessage('Input was exceeded. Remaining qty is ' + productData.attr('totalqty'));
                    return;
                }                

                this.CartTotal = CDec2(this.CartTotal) - CDec2(ItemPriceToVoid * CDec2(voidqty));
                this.CartTotalQty = this.CartTotalQty - CDec2(voidqty);
                Popup.Discount.dblSubTotal = this.CartTotal;
                $('#txtSubTotal').text((this.CartTotal == 0 ? '0.00' : AccountingFormat(this.CartTotal)));
                $('#txtTotalQty').text(this.CartTotalQty);
                //$('#txtTotal').text(AccountingFormat(CDec2($('#txtSubTotal').text()) - CDec2($('#txtLessDiscount').text())));

                if (!this.isLineItemDisplay) {
                    this.SubTotal();
                }
                
                this.Void();                
                
                this.txtQtyPlu.focus()
                //return true;
                
                if (CDec2($('#txtLessDiscount').text()) != 0) { // if there's a discount
                    Popup.Discount.RefreshTotalDiscount();
                } else {
                    Popup.ComputePayment() // Compute Payment fmcgpopup.js
                }
                
                var remainingQtyOfItem = Math.abs(CDec2(voidqty) - CDec2(productData.attr('totalqty')))
                this.ulSelectedProduct.find('li[prodid="' + selecteditemtovoid.data().ID + '"]').attr('totalqty', remainingQtyOfItem); // update totalqty of selected plu
                
                var totalVoid = '- ' + AccountingFormat(ItemPriceToVoid * CDec2(voidqty));
                var tr;
                if (this.isLineItemDisplay) {
                    tr = $('<tr>')
                            .append($('<td class="txt-center" style="width:130px">').text('VOID'))
                            .append($('<td>').text(ItemPLUToVoid))
                            .append($('<td class="txt-center" style="width:50px">').text('- ' + voidqty))
                            .append($('<td class="txt-center" style="width:100px;">').text(ItemPriceToVoid))
                            .append($('<td class="txt-center" style="width:100px;">').text(totalVoid))
                } else {
                    this.SubTotal(); // display the subtotal first

                    tr = $('<tr>')
                        .append($('<td style="padding-left:20px;">').text('VOID ' + ItemPLUToVoid))
                        //.append($('<td style="text-align:left;">').text(ItemPriceToVoid))
                        .append($('<td style="text-align:right; width:100px;">').text(totalVoid))
                }

                this.ulSelectedProduct.append($('<li>')
                                .addClass('voided')
                                .append($('<table border="0" cellpadding="3" cellspacing="0" style="width:100%; border:0px solid #FFF;">')
                                    .append($('<tbody>')
                                        .append(tr)
                                    )
                                )
                            )

                var vRows = [];

                vRows[0] = selecteditemtovoid.data();

                vRows[0].ProductName = 'VOID   ' + vRows[0].ProductName;
                vRows[0].RetailPrice = CDec2(vRows[0].RetailPrice) * -1;
                vRows[0].Qty = CInt2(voidqty) * -1;
                vRows[0].IsVoid = true; // only use in batch printing

                this.arrCartItems.push(vRows[0])

                if (this.isRealTimePrint) {
                    PrintSalesInvoiceItems(vRows, true);
                    this.justPrintItems = true;
                    this.ItemToPrint = vRows;
                    PrintReceiptNow(false, true);
                }

                var lastItem = $('.plu:last');
                var lastItemQty = CInt2(lastItem.attr('totalqty'))
                if (lastItemQty == 0) {
                    $('.lblCurrentProdQty2').text(1);
                    $('.lblCurrentProdTotal').text('0.00');
                } else {
                    $('.lblCurrentProdQty2').text(lastItemQty);
                    $('.lblCurrentProdTotal').text(AccountingFormat(lastItemQty * CDec2(lastItem.find('.txtPLUPrice').text())));
                } 

                selecteditemtovoid.data().Qty = remainingQtyOfItem;
                return this.GetStockInfo([selecteditemtovoid.data()], true, false) // update selected plu
            },
            CountCart: function () {
                if (this.ulSelectedProduct.find('li').length == 0) {
                    BlinkMessage('Cart is empty');
                    return false;
                }

                return true;
            },
            PriceVerifier: function () {
                var vRows = OfflineBrowser.LibraryPIMProduct.GetData({ keywords: $.trim(this.txtQtyPlu.val()) }).Rows;
                if (vRows.length > 0) {
                    vRows[0].Qty = this.currentQty;
                    //var resStockInfo = FmcgBasicCart.GetStockInfo(vRows, false, true);
                    //if (resStockInfo != false) {
                        //PopupButtonList('', 'Price Check', [
                        //    {
                        //        Text: 'PLU : ' + (resStockInfo[0].ReadyBarcode || resStockInfo[0].ProductNo) +
                        //              '<br /> <br /> Product Name : ' + vRows[0].ProductName +
                        //              '<br /> <br /> Retail Price : ' + AccountingFormat(resStockInfo[0].DefaultPrice),
                        //        data: ['a'],
                        //        Click: function (v) {
                        //            console.log(v)
                        //        }
                        //    }
                        //]);
                        BrowsePopup('LibraryPIMProduct', 'Price Verifier',
                            [
                                {
                                    Search: this.txtQtyPlu.val(),
                                    listData: { Results: { Rows: vRows } },
                                    DisplayText: [
                                        {
                                            Style: {
                                                width: '100px'
                                            }, Label: 'Product #', Column: 'ProductNo'
                                        },
                                        {
                                            Style: {
                                                width: '300px',
                                            }, Label: 'Description', Column: 'ProductName'
                                        },
                                        {
                                            Style: {
                                                width: '100px',
                                                textAlign: 'right'
                                            }, Label: 'Price', Column: function (e) {
                                                return AccountingFormat(e.DefaultPrice);
                                            }
                                        }
                                    ],
                                    Click: function (e) {
                                    },
                                    Offline: true,
                                    StyleFormat: 'table'
                                }
                            ]
                        )

                        $('#txtMessage').text('')
                    //}
                }
                else {
                    BlinkMessage('No Product(s) found')
                }
                FmcgBasicCart.txtQtyPlu.val('');
            },
            ScrollDownCart: function () {
                this.divListOfPlu.scrollTop(this.divListOfPlu[0].scrollHeight)
            },
            RecallCart: function () {
                BrowsePopup('TransInvoices', 'Recall Cart',
                    [
                        {
                            AutoSearch: true,
                            DisplayText: [
                                {
                                    Label: 'Sales Order No.: ',
                                    Column: 'SearchSO'
                                },
                                {
                                    Label: 'Customer: ',
                                    Column: 'CompanyName'
                                },
                                {
                                    Label: 'Date: ',
                                    Column: function (e) {
                                        return $.format.date(new Date(e.InvoiceDate), 'MMM dd, yyyy');
                                    }
                                },
                                {
                                    Label: 'FOWST: ',
                                    Column: 'FOWSTName'
                                },
                                {
                                    Label: 'Device: ',
                                    Column: 'DeviceName'
                                },
                                {
                                    Label: 'Total Value: ',
                                    Column: function (e) {
                                        return AccountingFormat(e.TotalOrder);
                                    }
                                },
                                {
                                    Label: 'SKU Count: ',
                                    Column: 'SkuCountOrder'
                                },
                                {
                                    Label: 'Created By: ',
                                    Column: 'EmployeeFullName'
                                }
                            ],
                            Click: function (e) {
                                NewCart();                               

                                var IsDraft;
                                sessionStorage.CustomerOrganizationID = e.CustomerID;
                                localStorage.removeItem('NewCartMode');

                                if (e.IsDraft == undefined) {
                                    IsDraft = 1;
                                }
                                else {
                                    IsDraft = e.IsDraft;
                                }
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
                                            },
                                            { Text: 'No' }
                                        ]
                                    );
                                } else {
                                    LoadData();
                                }

                                function LoadData() {
                                    FmcgBasicCart.isFromRecallCart = true;
                                    sessionStorage.setItem('CartOrderID', e.ID);                                    

                                    //Added By Francis to load Customer Info
                                    sessionStorage.CustomerOrganizationName = e.CompanyName;

                                    //additional by janine oct 27 for demographics
                                    sessionStorage.DeliveryMethod = e.DeliveryMethod;
                                    sessionStorage.OrderMethod = e.OrderMethod;
                                    sessionStorage.Gender = e.CustomerDemographicGender;
                                    sessionStorage.Age = e.CustomerDemographicAge;
                                    sessionStorage.TotalNumberOfCustomer = e.TotalNumberOfCustomer;

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

                                    if (proj.PosMode) {
                                        if(FmcgBasicCart.isRealTimePrint){
                                            PrintSalesInvoiceHeader();
                                        }
                                        FmcgBasicCart.UpdateOrderedQuantity();
                                        FmcgBasicCart.GetRecallCartDiscounts();
                                    }
                                    //else {
                                    //    $('#lstROPOItems').empty();

                                    //    isRecallCart = true;
                                    //    LoadCart(false, {
                                    //        SaveAsNew: vParams.SaveAsNew,
                                    //        ID: e.ID
                                    //    });

                                    //    if (CBool(sessionStorage.IsDraft)) {
                                    //        GotoFirstPage();
                                    //    }
                                    //}
                                }
                            },
                            Offline: true
                        }
                    ], {
                        CustomerID: sessionStorage.CustomerOrganizationID,
                        PoMode: CBool(proj.POMode),
                        SearchOnline: false
                    }
                )
            },
            UpdateOrderedQuantity: function () {
                var pData = {};
                pData.InvoiceID = sessionStorage.getItem('CartOrderID');

                if (OfflineMode) {

                    var vData = Stock_offline().UpdateOrderedQuantity(sessionStorage.getItem('CartOrderID'));
                    if (vData.Result == 'ok') {
                        FmcgBasicCart.GetRecallCartItems();
                    } else {
                        NewMessageBox(vData.Result);
                    }
                }
            },
            GetRecallCartItems: function(){
                var pData = {};
                pData.PosMode = proj.PosMode ? "pos" : "so";
                pData.CustomerID = FmcgBasicCart.GetCustomerID();
                pData.OrderID = sessionStorage.getItem('CartOrderID');
                pData.FowstID = sessionStorage.FowstID;
                
                var vCartItems = SalesMenu_offline().PosCart(pData).Units.Rows;
                
                for (var i = 0; i < vCartItems.length; i++){
                    var data = {
                        ID: vCartItems[i].ProductID,
                        ProductName: vCartItems[i].ProductName,
                        ProductNo: vCartItems[i].ProductNo,
                        DefaultPrice: vCartItems[i].UnitPrice,
                        Qty: CDec2(vCartItems[i].Quantity),
                        GroupIdOfItemModifiers: vCartItems[i].GroupIdOfItemModifiers,
                        HasModifier: vCartItems[i].HasModifier,
                        ItemModifier: vCartItems[i].ItemModifier,
                        SelectedItemModifierID: vCartItems[i].SelectedItemModifierID,
                        CategoryName: vCartItems[i].CategoryName
                    }
                    this.arrCartItems.push(data);
                    this.DisplayToCart([data])                    
                }

                if (this.isRealTimePrint) {
                    PrintSalesInvoiceItems(this.arrCartItems, false);                    
                    PrintReceiptNow(true, true)
                    this.PrintHeader = false;
                }
            },
            GetCustomerID: function () {
                var pCustomerID = sessionStorage.CustomerOrganizationID; // $('#lnkCustomer').data('id');

                if (pCustomerID == undefined)
                    NewMessageBox('Please select a customer first', 'No selected customer');
                else
                    return pCustomerID;
            },
            GetRecallCartDiscounts: function () {
                Popup.Discount.initElements();
                Popup.Discount.GetDiscountTypeList();
                Popup.Discount.GetCurrentDiscount();
                Popup.Discount.ShowSalesPaymentsInfo();
                //Popup.Discount.GetDiscountOnChange();
                Popup.Discount.RefreshTotalDiscount();
            }
        }

        $('#btnFMCGPLU').click(function () {
            KeyPageDown(FmcgBasicCart.txtQtyPlu);
        })

        $('#btnFMCGQty').click(function () {
            KeyEnd(FmcgBasicCart.txtQtyPlu);
        })

        $('#btnFMCGDisc').click(function () {
            KeyF11();
        })

        $('#btnFMCGVoid').click(function () {
            KeyInsert();
        })

        $('#btnFMCGSub').click(function () {
            KeyDel();
        })

        $('#btnFMCGCashKey').click(function () {
            KeyPageUp();
        })

        $('#btnFMCGSoldTo').click(function () {
            KeyHome();
        })

        $('#btnFMCGPriceVerifier').click(function () {
            KeyF1();
        })

        $('#btnFMCGClear').click(function () {
            KeyF12();
        })

        $('#btnFMCGRecall').click(function () {
            KeyF4();
        })

        $('#btnVoid').click(function () {
            FmcgBasicCart.Void()
        })

        $('#btnRecall').click(function () {
            FmcgBasicCart.RecallCart()
        })

        $('#btnSampleByBatch').click(function () {
            //sessionStorage.CartOrderID = 'bf593a06-9b95-4259-9767-45df7b3404ef'
            //PrintReceiptNow();
            BlinkMessage('Printing Receipt ...');
            FmcgPayment.SaveReceipt(true);
        })

        $('#btnCash').click(function () {
            Popup.Cash.init({
                Balance: $('#txtTotal').text(),
                success: function (cash) {
                    var bal = CDec2(cash) - CDec2($('#txtTotal').text());
                    $('#txtCash').text(AccountingFormat(cash))
                }
            });
        })
    }).keydown(function (e) {
        if (e.which == 46 && Popup.popupname == "") {
            //Del (Subtotal)
            KeyDel();
        }
        else if (e.which == 45) {
            //Insert (Void)
            KeyInsert();
        }
        else if (e.which == 38) {
            //arrow up
            if (FmcgBasicCart.isVoidActive) {
                FmcgBasicCart.Void('up');
            }
        }
        else if (e.which == 40) {
            //arrow down
            if (FmcgBasicCart.isVoidActive) {
                FmcgBasicCart.Void('down');
            }
        }
        else if (e.which == 33) {
            //PageUp (Cash)
            KeyPageUp();
        }
        else if (e.which == 122) {
            e.preventDefault();
            //F11 (Discount)
            //coming soon
            //if(OfflineMode){
            //    Android.Toast('Discount is under construction');
            //}
            //return
            KeyF11();
        }
        else if (e.keyCode == 36) {
            //Home (SoldTo)
            //$('#pageCustomerOptions').pageshow();
            KeyHome();
        }
        else if (e.which == 112) {
            //F1 - Price Verifier
            e.preventDefault();
            KeyF1();
        }
        else if (e.which == 123) {
            // F12 (Clear)
            e.preventDefault();
            KeyF12();
        }
        else if (e.which == 115) {
            // F4 (Recall Cart)
            e.preventDefault();
            KeyF4();
        }
    })

    function KeyPageDown(dis) {
        if (FmcgBasicCart.isVoidActive) {
            FmcgBasicCart.Void(); // to disable void
        }

        FmcgBasicCart.currentQty = 1;
        FmcgBasicCart.txtQtyPluAction = 'plu';
        dis.attr('placeholder', 'Enter PLU').val('');
        $('.lblCurrentProdQty, .lblCurrentProdQty2').text(FmcgBasicCart.currentQty);
        $('#divPLU').pageshow();
    }

    function KeyEnd(dis) {
        FmcgBasicCart.currentQty = dis.val();

        if (!$.isNumeric(FmcgBasicCart.currentQty)) {
            dis.val('');
            BlinkMessage('Qty must be a number')
            return false;
        }

        FmcgBasicCart.txtQtyPluAction = 'plu';
        dis.val('');
        dis.attr('placeholder', 'Enter PLU')
        $('.lblCurrentProdQty').text(FmcgBasicCart.currentQty);
        $('#divPLU').pageshow();
    }

    function KeyF11() {
        if (sessionStorage.isNewCart == false || sessionStorage.isNewCart == undefined) {
            if (FmcgBasicCart.CountCart()) {
                //executed when cart is not empty
                Popup.Discount.init({
                    success: function (e) {

                    }
                });
            }
        }
    }

    function KeyDel() {
        if (FmcgBasicCart.isLineItemDisplay) {
            BlinkMessage('Subtotal only use in "Journal Entry Display"');
        } else {
            if (sessionStorage.isNewCart == false || sessionStorage.isNewCart == undefined) {
                if (FmcgBasicCart.CountCart()) {
                    //execute when cart is not empty
                    FmcgBasicCart.SubTotal();
                }
            }
        }
    }

    function KeyPageUp() {
        if (FmcgBasicCart.CountCart()) {
            Popup.Cash.init({
                Balance: $('#txtTotal').text(),
                success: function (cash) {
                    var bal = CDec2(cash) - CDec2($('#txtTotal').text());
                    $('#txtCash').text(AccountingFormat(cash))
                }
            });
        }
    }

    function KeyHome() {
        Popup.SoldTo.init();
    }

    function KeyInsert() {
        FmcgBasicCart.Void();
    }

    function KeyF1() {
        FmcgBasicCart.txtQtyPluAction = 'priceverifier';
        FmcgBasicCart.txtQtyPlu.attr('placeholder', 'Price Verifier: Enter Barcode or Name of Product').val('').focus();
    }

    function KeyF12() {
        FmcgBasicCart.txtQtyPlu.val('').focus();
    }

    function KeyF4() {
        FmcgBasicCart.RecallCart();
    }

    function PrintSalesInvoiceHeader() {
        var intFAFCTotalLenght = 40;
        var intFBTotalLenght = 27;
        var intFCDoubleTotalLenght = 18;

        var intDesktopPrintLenght = 45;

        //Get Printer
        vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();

        //Get Header
        vDataHeader = SalesPayment_offline().getReceiptHeaderInfo(sessionStorage.CartOrderID);
        //Get Invoice Header
        vDataInvHeader = SalesPayment_offline().getSalesPaymentInvoiceHeaderInfo(sessionStorage.CartOrderID);

        //New set of text to print
        NewPrintBuilder();

        //Get Header
        IsShowSalesRep = false;
        var mDate = '';

        if (InAndroid) {
            mDate = vDataInvHeader.Rows[0].InvoiceDate;
        }
        else {
            mDate = vDataInvHeader.Rows[0].InvoiceDate.replace(/-/g, '/');
        }

        if (vDataHeader.Rows.length > 0) {

            IsShowSalesRep = (vDataHeader.Rows[0].IsShowSalesRep == 1 ? true : false);

            strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

            strPrint = '';
            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].CorporateName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            strPrint = strPrint + strTemp;

            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT TIN ' + $.trim((vDataHeader.Rows[0].TIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            strPrint = strPrint + strTemp;

            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].CompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            strPrint = strPrint + strTemp;

            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataHeader.Rows[0].TelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
            strPrint = strPrint + strTemp;

            strTemp = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
            strPrint = strPrint + strTemp;

            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

            //Get Items
            if (vDataInvHeader.Rows.length > 0) {
                strPrint = '';
                
                if (CStr2(vDataInvHeader.Rows[0].CustomerTableNo) != '') {
                    var prefix = vDataInvHeader.Rows[0].CustomerTableNo.split('_');

                    if ($.trim((vDataInvHeader.Rows[0].CustomerTableNo || '')) != '') {
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Table #', 8, ' ', 1), 7, ': ' + $.trim((prefix[1] || '')), intFAFCTotalLenght, '', 0, 0);
                        strPrint = strTemp;
                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                    }
                }

                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Trans No.: ' + vDataInvHeader.Rows[0].TransNum, intFAFCTotalLenght, '', 0, '', 0, 0);
                ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);

                strPrint = '';
                strTemp = FormatTextToPrint(intFCDoubleTotalLenght, ($.format.date(new Date(mDate), 'MMM dd, yyyy HH:mm') || ''), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                strPrint = strPrint + strTemp;

                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Cashier: ' + ($.trim(vDataInvHeader.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, '', 0, 0);
                ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);

                strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                if (vDataPrint.Rows.length > 0) {
                    PrintText(vDataPrint.Rows[0].PrinterIPAddress);
                    ClearCommandBuffer();
                }
                
            }
        }
    }

    function PrintSalesInvoiceItems(vRows, IsVoid) {
        var intFAFCTotalLenght = 40;
        var intFBTotalLenght = 27;
        var intFCDoubleTotalLenght = 18;

        var intDesktopPrintLenght = 45;

        NewPrintBuilder();

        //Get Items
        var strProductID = "";
        if (vRows.length > 0) {
            strStockID = '';
            strPrint = '';

            //if (CBool(IsVoid)) {
            //    strPrint = '';

            //    strTemp = FormatTextToPrint(intFAFCTotalLenght, vRows[0].NoOfItems + ' Item(s)', 14, ' Sub T --> ', 27, vRows[0].SubTotal, intFAFCTotalLenght, 0);
            //    strPrint = strPrint + strTemp;

            //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

            //} else {

            for (var i = 0; i < vRows.length; i++){
                var strStockDescription = $.trim((vRows[i].ProductName || ''));
                var strUnitPrice = SetPrefixPostfix(AccountingFormat(vRows[i].DefaultPrice || vRows[i].RetailPrice), intFAFCTotalLenght - 30, ' ', 0);
                var strQty = SetPrefixPostfix(AccountingFormat((vRows[i].Qty || '0'), 0), 8, ' ', 0);

                strQty = strQty.replace(/\s/g, '');

                var SubTotal = CDec2(vRows[i].Qty) * (OfflineMode ? CDec2(vRows[i].DefaultPrice) : CDec2(vRows[i].RetailPrice));
                var strSubTotal = SetPrefixPostfix((IsVoid ? SubTotal : AccountingFormat(SubTotal || '0')), intFAFCTotalLenght - 26, ' ', 0);

                strPrint = '';

                strTemp = FormatTextToPrint(intFAFCTotalLenght, '  PLU :', 5, (vRows[i].ReadyBarcode || vRows[i].ProductNo), intFAFCTotalLenght, 0);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, '', 0, strStockDescription, intFAFCTotalLenght, 0);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' ' + strQty, strQty.length + 2, ' x ' + strUnitPrice, 23, strSubTotal, intFAFCTotalLenght, 0);
                strPrint = strPrint + strTemp;

                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
            }               

                //intLimitPerBatchPrint = intLimitPerBatchPrint + 1;


            //};

            vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();

            if (vDataPrint.Rows.length > 0) {
                PrintText(vDataPrint.Rows[0].PrinterIPAddress);
                ClearCommandBuffer();
            }

            

            $('#txtTotalQty').text(CInt2(FmcgBasicCart.CartTotalQty));
        }
    }

    window.PrintSalesInvoiceFooter = function () {
        var intFAFCTotalLenght = 40;
        var intFBTotalLenght = 27;
        var intFCDoubleTotalLenght = 18;

        var intDesktopPrintLenght = 45;

        var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

        if (vDataPrinterType != undefined) {
            if (vDataPrinterType.Rows.length > 0) {
                if (vDataPrinterType.Rows[0].ConnectionType == "USB") {
                    setPrinterConnectionType(vDataPrinterType.Rows[0].ConnectionType, vDataPrinterType.Rows[0].PrinterType, false)
                }
            }
        }

        //New set of text to print
        NewPrintBuilder();

        //Get Invoice Header
        vDataInvHeader = SalesPayment_offline().getSalesPaymentInvoiceHeaderInfo(sessionStorage.CartOrderID);

        //Get Payment Info
        vDataPayment = SalesPayment_offline().getSalesPaymentReceiptPaymentInfo(sessionStorage.CartOrderID);
        //Get Invoice Items Serial
        vDataItemSerial = SalesPayment_offline().getSalesPaymentReceiptSerial(sessionStorage.CartOrderID);
        //Get Footer Amounts
        vDataFooter = SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(sessionStorage.CartOrderID);
        //Get Footer Messages
        vDataMsg = SalesPayment_offline().getSalesPaymentReceiptFooterMessagesInfo();

        //Get Header
        IsShowSalesRep = false;
        var mDate = '';

        if (InAndroid) {
            mDate = vDataInvHeader.Rows[0].InvoiceDate;
        }
        else {
            mDate = vDataInvHeader.Rows[0].InvoiceDate.replace(/-/g, '/');
        }

        //Get Footer Amounts
        if (vDataPayment.Rows.length > 0) {

            //Get Footer Amounts
            if (vDataFooter.Rows.length > 0) {
                var strSKUCount = SetPrefixPostfix(AccountingFormat((vDataFooter.Rows[0].SKUCount || '0'), 0), intFAFCTotalLenght - 16, ' ', 0);
                var strTotalAmountDue = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].SubTotal || '0'), intFAFCTotalLenght - 24, ' ', 0);
                var strTotalDiscount = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].DiscountAmount || '0'), intFAFCTotalLenght - 24, ' ', 0);
                var strNetAmountDue = SetPrefixPostfix(AccountingFormat(CDec2(vDataFooter.Rows[0].NetAmountDue) + CDec2(vDataFooter.Rows[0].ChargeAmount) + CDec2(vDataFooter.Rows[0].TipAmount) + CDec2(vDataFooter.Rows[0].LocalTaxAmount)), intFAFCTotalLenght - 24, ' ', 0);

                var strTotalCharges = SetPrefixPostfix(vDataFooter.Rows[0].ChargeAmount || '0', intFAFCTotalLenght - 24, ' ', 0);
                var strTip = SetPrefixPostfix(vDataFooter.Rows[0].TipAmount || '0', intFAFCTotalLenght - 24, ' ', 0);

                var strVatSale = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TotalAmountDue), intFAFCTotalLenght - 24, ' ', 0);
                var strVatAmount = SetPrefixPostfix(AccountingFormat(CDec2(vDataFooter.Rows[0].SubTotal) - CDec2(vDataFooter.Rows[0].TotalAmountDue)), intFAFCTotalLenght - 24, ' ', 0);

                var strOtherTax = SetPrefixPostfix(vDataFooter.Rows[0].LocalTaxAmount || '0', intFAFCTotalLenght - 24, ' ', 0);

                strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';
                strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'SI ' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                strPrint = strPrint + strTemp;

                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';
                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SKU Count: ', 12, strSKUCount, intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sub Total: ', 20, strTotalAmountDue, intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                //if (isSeniorCitizenDiscount) {
                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: ' + vDataFooter.Rows[0].TaxPercent + '% VAT ', 20, strVatAmount, intFAFCTotalLenght, '', 0, 0);
                //    strPrint = strPrint + strTemp;

                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Amount: ', 20, strVatSale, intFAFCTotalLenght, '', 0, 0);
                //    strPrint = strPrint + strTemp;


                //}

                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Discount:   ', 20, strTotalDiscount, intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

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

                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Amount Due:   ', 20, strNetAmountDue, intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;
                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                if (vDataPayment.Rows.length > 0) {

                    var strCashPayment = SetPrefixPostfix(AccountingFormat((vDataPayment.Rows[0].CashPayment || '0')), intFAFCTotalLenght - 9, ' ', 0);
                    var strCheckPayment = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CheckPaymentAmount || '0'), intFAFCTotalLenght - 10, ' ', 0);
                    var strDebitCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].DebitCardAmount || '0'), intFAFCTotalLenght - 16, ' ', 0);
                    var strCreditCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CreditCardAmount || '0'), intFAFCTotalLenght - 16, ' ', 0);
                    var strSRVAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].SRVAmount || '0'), intFAFCTotalLenght - 8, ' ', 0);
                    var strOtherTypeAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].OtherTypeAmount || '0'), intFAFCTotalLenght - 16, ' ', 0);
                    var strCustomerChangeCash = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CustomerChangeCash || '0'), intFAFCTotalLenght - 11, ' ', 0);

                    strPrint = '\n';
                    if (vDataPayment.Rows[0].CashPayment > 0) {
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Cash:', 5, strCashPayment, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;
                    }
                    if (vDataPayment.Rows[0].CheckPaymentAmount > 0) {
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Check:', 6, strCheckPayment, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;
                    }
                    if (vDataPayment.Rows[0].DebitCardAmount > 0) {
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Debit Card:', 12, strDebitCardAmount, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;
                    }
                    if (vDataPayment.Rows[0].CreditCardAmount > 0) {
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Credit Card:', 12, strCreditCardAmount, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;
                    }
                    if (vDataPayment.Rows[0].SRVAmount > 0) {
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SRV:', 4, strSRVAmount, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;
                    }
                    if (vDataPayment.Rows[0].OtherTypeAmount > 0) {
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Other Tender:', 12, strOtherTypeAmount, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;
                    }

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Change:', 7, strCustomerChangeCash, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                    if (vDataPayment.Rows[0].CheckPaymentAmount > vDataFooter.Rows[0].NetAmountDue) {
                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '* Excess payments made through checks are not entitled to cash refunds.', intFAFCTotalLenght, '', 0, '', 0, 0);
                        ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                    }

                }

                strPrint = '';
                strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';
                var strVATSales = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TaxSales || '0'), intFAFCTotalLenght - 22, ' ', 0);
                var strVATAmount = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TaxAmount || '0'), intFAFCTotalLenght - 17, ' ', 0);
                var strNonVAT = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].NonVATSale || '0'), intFAFCTotalLenght - 24, ' ', 0);
                var strVATExempt = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].VATExemptSale || '0'), intFAFCTotalLenght - 24, ' ', 0);
                var strVATZeroRated = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].ZeroRatedSale || '0'), intFAFCTotalLenght - 27, ' ', 0);

                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales: ', 18, strVATSales, intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount: ', 13, strVATAmount, intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT Sales: ', 20, strNonVAT, intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Exempt Sales: ', 20, strVATExempt, intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT Sales: ', 23, strVATZeroRated, intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';
                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Customer', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerName || '')), intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Address', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerAddress || '')), intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerTIN || '')), intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;
                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);


                strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';

                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Accredited Supplier', 20, ' ', 1), 19, ': ' + proj.AccreditedSupplier, intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Registered Address', 20, ' ', 1), 19, ': ' + proj.RegisteredAddress, intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN-Branch Code', 20, ' ', 1), 19, ': ' + proj.TinBranchCode, intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('POS Accreditation', 20, ' ', 1), 19, ': ' + $.trim((vDataHeader.Rows[0].POSAccreditationNo || '')), intFAFCTotalLenght, '', 0, 0);
                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('POS Accreditation', 20, ' ', 1), 19, ': ' + proj.PosAccreditation, intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';

                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('POS Reg. No./Permit To Use (PTU) No.', 20, ' ', 1), 19, ': ' + $.trim((vDataHeader.Rows[0].POSPermitNo || '')), intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Machine ID No. (MIN)', 20, ' ', 1), 19, ': ' + vDataHeader.Rows[0].MIN + ' \n', intFAFCTotalLenght, '', 0, 0);
                strPrint = strPrint + strTemp;

                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                // 2022_07_14__13_09_45_3756
                //strPrint = '';
                //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim('THIS INVOICE/RECEIPT SHALL BE VALID FOR FIVE(5) YEARS FROM THE DATE OF THE PERMIT TO USE'), intFAFCTotalLenght, '', 0, '', 0, 1);

                strPrint = strPrint + strTemp;

                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //Senior Citizen/Person With Disability info
                //if (isSeniorCitizenDiscount) {
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Senior Citizen/PWD TIN: ', 12, "Emilio Aguinaldo", intFAFCTotalLenght, '', 0, 0);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'OSCA ID No./PWD ID No.: ', 12, "8700-324", intFAFCTotalLenght, '', 0, 0);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Signature: ', 12, "Heiroglophyics \n", intFAFCTotalLenght, '', 0, 0);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                //}

                strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                //Get Footer Messages
                strPrint = '';
                //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Use for testing only. This is not an Official Receipt.', intFAFCTotalLenght, '', 0, '', 0, 1);
                //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'This Serves as Your Official Receipt.', intFAFCTotalLenght, '', 0, '', 0, 1);
                strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataInvHeader.Rows[0].ReceiptFooterMsg, intFAFCTotalLenght, '', 0, '', 0, 1);
                strPrint = strPrint + strTemp;

                if (vDataMsg.Rows.length > 0) {
                    for (var p = 0; p <= vDataMsg.Rows.length - 1; p++) {
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataMsg.Rows[p].MessageFooter || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                        strPrint = strPrint + strTemp;
                    };

                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', intFAFCTotalLenght, '', 0, '', 0, 1);
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                    //breakdown

                    /*//Print Barcode
                    strPrint = 'SI|' + vDataInvHeader.Rows[0].OrganizationID + '|' + vDataInvHeader.Rows[0].OrganizationBranchID + '|' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0)
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);

                    strPrint = SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0);
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 1, 0);*/

                    ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                    vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();

                    if (vDataPrint.Rows.length > 0) {
                        PrintText(vDataPrint.Rows[0].PrinterIPAddress);
                        ClearCommandBuffer();
                    }

                    

                    setPrinterConnectionType(vDataPrinterType.Rows[0].ConnectionType, vDataPrinterType.Rows[0].PrinterType, true)
                    //Print SRV
                    //PrintSalesPaymentSRVReceipt(0, pInvoiceID);

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
        }
        NewCart();
    }

    var isFinishGetRequiredData, vDataPrint, vDataHeader, vDataInvHeader, vDataPayment, vDataItemSerial, vDataFooter, vDataMsg, vDataPrinterType;

    function RequiredPrintData() {
        //Get Printer
        vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();
        //Get Header
        vDataHeader = SalesPayment_offline().getReceiptHeaderInfo(sessionStorage.CartOrderID);
        //Get Invoice Header
        vDataInvHeader = SalesPayment_offline().getSalesPaymentInvoiceHeaderInfo(sessionStorage.CartOrderID);
        //Get Payment Info

        //nagachange ang payment every transaction so dapat dili ni dire ibutang
        //vDataPayment = SalesPayment_offline().getSalesPaymentReceiptPaymentInfo(sessionStorage.CartOrderID);

        //Get Invoice Items Serial
        vDataItemSerial = SalesPayment_offline().getSalesPaymentReceiptSerial(sessionStorage.CartOrderID);        
        //Get Footer Messages
        vDataMsg = SalesPayment_offline().getSalesPaymentReceiptFooterMessagesInfo();
        //Get Printer Type
        vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();
    }

    window.PrintReceiptViaWindowsPrintServer = function (pInvoiceID) {
        vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();
        //Get Header
        vDataHeader = SalesPayment_offline().getReceiptHeaderInfo(pInvoiceID);
        //Get Invoice Header
        vDataInvHeader = SalesPayment_offline().getSalesPaymentInvoiceHeaderInfo(pInvoiceID);
        //Get Invoice Items
        vDataItem = SalesPayment_offline().getSalesPaymentReceiptBodyInfo(pInvoiceID);
        //Get Payment Info
        vDataPayment = SalesPayment_offline().getSalesPaymentReceiptPaymentInfo(pInvoiceID);
        //Get Invoice Items Serial
        vDataItemSerial = SalesPayment_offline().getSalesPaymentReceiptSerial(pInvoiceID);
        //Get Footer Amounts
        vDataFooter = SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(pInvoiceID);
        //Get Footer Messages
        vDataMsg = SalesPayment_offline().getSalesPaymentReceiptFooterMessagesInfo();

        PrintReceiptViaWindowsPrintServer(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)
    }


    window.PrintReceiptNow = function (isPrintHeader, isPrintBody, isPrintFooter) {
        //var vItems = (FmcgBasicCart.justPrintItems ? FmcgBasicCart.ItemToPrint : FmcgBasicCart.arrCartItems);
        //var intFAFCTotalLenght = 40;
        //var intFBTotalLenght = 27;
        //var intFCDoubleTotalLenght = 18;

        //var intDesktopPrintLenght = 45;
        //var IsVoid = false;

        //if(CBool(isFinishGetRequiredData) == false){
            RequiredPrintData();
            //isFinishGetRequiredData = true;
        //}
        
        //dire ni sya ibutang para always marefresh ang payments info
        vDataPayment = SalesPayment_offline().getSalesPaymentReceiptPaymentInfo(sessionStorage.CartOrderID);

        if (CBool(isPrintFooter)) {
            //print footer + drawer kick
            setTimeout(function () {
                setPrinterConnectionType(vDataPrinterType.Rows[0].ConnectionType, vDataPrinterType.Rows[0].PrinterType, false)
                FinalPrintReceiptNow(isPrintHeader, isPrintBody, isPrintFooter)
                if (isPrintFooter) {
                    NewCart(); // clear cart after save
                }                
            }, 1000)            
        } else {
            setTimeout(function () {
                setPrinterConnectionType(vDataPrinterType.Rows[0].ConnectionType, vDataPrinterType.Rows[0].PrinterType, true)
                FinalPrintReceiptNow(isPrintHeader, isPrintBody, isPrintFooter)
                if (isPrintFooter) {
                    NewCart(); // clear cart after save
                }
            }, 1000)            
        }
    }

    function FinalPrintReceiptNow (isPrintHeader, isPrintBody, isPrintFooter) {
        var vItems = (FmcgBasicCart.justPrintItems ? FmcgBasicCart.ItemToPrint : FmcgBasicCart.arrCartItems);
        var intFAFCTotalLenght = 40;
        var intFBTotalLenght = 27;
        var intFCDoubleTotalLenght = 18;

        var intDesktopPrintLenght = 45;
        var IsVoid = false;

        if (!CBool(FmcgBasicCart.isRealTimePrint)) {
            vItems = SalesPayment_offline().getSalesPaymentReceiptBodyInfo(sessionStorage.CartOrderID).Rows;
        }

        //if (CBool(isFinishGetRequiredData) == false) {
        //    RequiredPrintData();
        //    isFinishGetRequiredData = true;
        //}

        //if (CBool(isPrintFooter)) {
        //    //print footer + drawer kick
        //    setPrinterConnectionType(vDataPrinterType.Rows[0].ConnectionType, vDataPrinterType.Rows[0].PrinterType, false)
        //} else {
        //    setPrinterConnectionType(vDataPrinterType.Rows[0].ConnectionType, vDataPrinterType.Rows[0].PrinterType, true)
        //}

        //New set of text to print
        NewPrintBuilder();

        //Get Header
        IsShowSalesRep = false;
        var mDate = '';

        if (InAndroid) {
            mDate = vDataInvHeader.Rows[0].InvoiceDate;
        }
        else {
            mDate = vDataInvHeader.Rows[0].InvoiceDate.replace(/-/g, '/');
        }

        if (CBool(isPrintHeader)) {
            if (vDataHeader.Rows.length > 0) {

                IsShowSalesRep = (vDataHeader.Rows[0].IsShowSalesRep == 1 ? true : false);

                strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                strPrint = '';
                strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].CorporateName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT TIN ' + $.trim((vDataHeader.Rows[0].TIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].CompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataHeader.Rows[0].TelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                strPrint = strPrint + strTemp;

                strTemp = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                strPrint = strPrint + strTemp;

                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                //Get Items
                if (vDataInvHeader.Rows.length > 0) {
                    strPrint = '';

                    if (CStr2(vDataInvHeader.Rows[0].CustomerTableNo) != '') {
                        var prefix = vDataInvHeader.Rows[0].CustomerTableNo.split('_');

                        if ($.trim((vDataInvHeader.Rows[0].CustomerTableNo || '')) != '') {
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Table #', 8, ' ', 1), 7, ': ' + $.trim((prefix[1] || '')), intFAFCTotalLenght, '', 0, 0);
                            strPrint = strTemp;
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                        }
                    }

                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                    //to be implemented soon. this is important
                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Trans No.: ' + vDataInvHeader.Rows[0].TransNum, intFAFCTotalLenght, '', 0, '', 0, 0);
                    //ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);

                    strPrint = '';
                    strTemp = FormatTextToPrint(intFCDoubleTotalLenght, ($.format.date(new Date(mDate), 'MMM dd, yyyy HH:mm') || ''), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                    strPrint = strPrint + strTemp;

                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Cashier: ' + ($.trim(vDataInvHeader.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, '', 0, 0);
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);

                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                    //if (vDataPrint.Rows.length > 0) {
                    //    PrintText(vDataPrint.Rows[0].PrinterIPAddress);
                    //    ClearCommandBuffer();
                    //}

                }
            }
        }

        if (CBool(isPrintBody)) {
            for (var i = 0; i < vItems.length; i++) {
                var strStockDescription = $.trim((vItems[i].ProductName || vItems[i].StockDescription || ''));
                var strUnitPrice = SetPrefixPostfix(AccountingFormat(vItems[i].DefaultPrice || vItems[i].RetailPrice || vItems[i].UnitPrice), intFAFCTotalLenght - 30, ' ', 0);
                var strQty = SetPrefixPostfix(AccountingFormat((vItems[i].Qty || vItems[i].Quantity || '0'), 0), 8, ' ', 0);
                var strUnitMeasure = (vItems[i].UnitMeasure || '')
                strQty = strQty.replace(/\s/g, '');

                var SubTotal = CDec2(vItems[i].Qty) * (OfflineMode ? CDec2(vItems[i].DefaultPrice) : CDec2(vItems[i].RetailPrice));
                var strSubTotal = SetPrefixPostfix((CBool(vItems[i].IsVoid) ? SubTotal : AccountingFormat(SubTotal || '0')), intFAFCTotalLenght - 26, ' ', 0);

                strPrint = '';

                if(CBool(FmcgBasicCart.isRealTimePrint)){
                    strTemp = FormatTextToPrint(intFAFCTotalLenght, '  PLU :', 5, (vItems[i].ReadyBarcode || vItems[i].ProductNo), intFAFCTotalLenght, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, '', 0, strStockDescription, intFAFCTotalLenght, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' ' + strQty, strQty.length + 2, ' x ' + strUnitPrice, 23, strSubTotal, intFAFCTotalLenght, 0);
                    strPrint = strPrint + strTemp;
                } else {
                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, strStockDescription, 13, '   ' + strQty + ' ' + strUnitMeasure + '    @ ' + $.trim(strUnitPrice), 25, '       ' + AccountingFormat(CDec2(strQty) * CDec2(strUnitPrice)), intFAFCTotalLenght, 0);
                    //strPrint = strPrint + strTemp;

                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' ', intFAFCTotalLenght, '', 0, '', 0, 0);
                    //strPrint = strPrint + strTemp;
                    strTemp = FormatTextToPrint(intFAFCTotalLenght, strStockDescription, intFAFCTotalLenght, '', 0, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    var itemtotal = SetPrefixPostfix(AccountingFormat(CDec2(strQty) * CDec2(strUnitPrice)) || '0', intFAFCTotalLenght - 22, ' ', 0);
                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' ' + strQty + ' @ ' + $.trim(strUnitPrice) + ' / ' + strUnitMeasure, 20, itemtotal, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;
                }

                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
            }
        }

        //if (vDataPrinterType != undefined) {
        //    if (vDataPrinterType.Rows.length > 0) {
        //        if (vDataPrinterType.Rows[0].ConnectionType == "USB") {
        //            setPrinterConnectionType(vDataPrinterType.Rows[0].ConnectionType, vDataPrinterType.Rows[0].PrinterType, false)
        //        }
        //    }
        //}        

        //Get Footer Amounts
        if (CBool(isPrintFooter)) {
            //Get Invoice Header
            vDataInvHeader = SalesPayment_offline().getSalesPaymentInvoiceHeaderInfo(sessionStorage.CartOrderID);
            //Get Footer Amounts
            vDataFooter = SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(sessionStorage.CartOrderID);

            if (vDataPayment.Rows.length > 0) {
                
                //Get Footer Amounts
                if (vDataFooter.Rows.length > 0) {
                    var strSKUCount = SetPrefixPostfix(AccountingFormat((vDataFooter.Rows[0].SKUCount || '0'), 0), intFAFCTotalLenght - 22, ' ', 0);
                    var strTotalAmountDue = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].SubTotal || '0'), intFAFCTotalLenght - 22, ' ', 0);
                    var strTotalDiscount = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].DiscountAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                    var strNetAmountDue = SetPrefixPostfix(AccountingFormat(CDec2(vDataFooter.Rows[0].NetAmountDue) + CDec2(vDataFooter.Rows[0].ChargeAmount) + CDec2(vDataFooter.Rows[0].TipAmount) + CDec2(vDataFooter.Rows[0].LocalTaxAmount)), intFAFCTotalLenght - 22, ' ', 0);

                    var strTotalCharges = SetPrefixPostfix(vDataFooter.Rows[0].ChargeAmount || '0', intFAFCTotalLenght - 22, ' ', 0);
                    var strTip = SetPrefixPostfix(vDataFooter.Rows[0].TipAmount || '0', intFAFCTotalLenght - 22, ' ', 0);

                    var strVatSale = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TotalAmountDue), intFAFCTotalLenght - 22, ' ', 0);
                    var strVatAmount = SetPrefixPostfix(AccountingFormat(CDec2(vDataFooter.Rows[0].SubTotal) - CDec2(vDataFooter.Rows[0].TotalAmountDue)), intFAFCTotalLenght - 22, ' ', 0);

                    var strOtherTax = SetPrefixPostfix(vDataFooter.Rows[0].LocalTaxAmount || '0', intFAFCTotalLenght - 22, ' ', 0);

                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                    strPrint = '';
                    strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'SI ' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                    strPrint = strPrint + strTemp;

                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                    strPrint = '';
                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SKU Count: ', 20, strSKUCount, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sub Total: ', 20, strTotalAmountDue, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    if (Popup.Discount.isSeniorCitizenDiscount) {
                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: ' + vDataFooter.Rows[0].TaxPercent + '% VAT ', 20, strVatAmount, intFAFCTotalLenght, '', 0, 0);
                        //strPrint = strPrint + strTemp;

                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Amount: ', 20, strVatSale, intFAFCTotalLenght, '', 0, 0);
                        //strPrint = strPrint + strTemp;
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                        strPrint = strPrint + strTemp;

                        if (Popup.Discount.SCDiscountExempt > 0) {
                            var strSCDiscountExempt = SetPrefixPostfix(AccountingFormat(Popup.Discount.SCDiscountExempt || '0'), intFAFCTotalLenght - 22, ' ', 0);
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SC Discount-Exempt: ', 20, strSCDiscountExempt, intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;
                        }

                        var strSubTotalOfVatExemptItems = SetPrefixPostfix(AccountingFormat(Popup.Discount.SubTotalOfVatExemptItems || '0'), intFAFCTotalLenght - 31, ' ', 0);
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Gross Amount Subject to SCD: ', 29, strSubTotalOfVatExemptItems, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;

                        if (Popup.Discount.GrossSalesToNoneSC > 0) {
                            var strGrossSalesToNoneSC = SetPrefixPostfix(AccountingFormat(Popup.Discount.GrossSalesToNoneSC || '0'), intFAFCTotalLenght - 31, ' ', 0);
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Gross Sales to None-SC ' + (CInt2(Popup.Discount.txtSalesPayNoOfDiners.val()) - CInt2(Popup.Discount.txtSalesPayNoOfSC.val())) + '/' + Popup.Discount.txtSalesPayNoOfDiners.val() + ':', 29, strGrossSalesToNoneSC, intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;
                        }

                        var strGrossSalesToSC = SetPrefixPostfix(AccountingFormat(Popup.Discount.GrossSalesToSC || '0'), intFAFCTotalLenght - 23, ' ', 0);
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Gross Sales to SC ' + Popup.Discount.txtSalesPayNoOfSC.val() + '/' + Popup.Discount.txtSalesPayNoOfDiners.val() + ':', 21, strGrossSalesToSC, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;

                        var strVatExemptSales = SetPrefixPostfix(AccountingFormat(Popup.Discount.VatExemptSales || '0') + (Popup.Discount.IsApplyMaxSCDiscountableAmount ? '' : '*'), intFAFCTotalLenght - 22, ' ', 0);
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Ex for SC: ', 20, strVatExemptSales, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;

                        var strMaxSCDiscountableAmount = SetPrefixPostfix(AccountingFormat(Popup.Discount.MaxSCDiscountableAmount || '0') + (Popup.Discount.IsApplyMaxSCDiscountableAmount ? '*' : ''), intFAFCTotalLenght - 25, ' ', 0);
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Max Discountable VAT-EX(' + (Popup.Discount.MaxSCDiscountableAmount / CInt2(Popup.Discount.txtSalesPayNoOfSC.val())) + '*' + CInt2(Popup.Discount.txtSalesPayNoOfSC.val()) + ')', 23, strMaxSCDiscountableAmount, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;

                        var strDiscountPercentCeiling = SetPrefixPostfix(AccountingFormat(Popup.Discount.txtDiscountPercent.val()) + '%', intFAFCTotalLenght - 22, ' ', 0);
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SC Discount Rate: ', 20, strDiscountPercentCeiling, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;

                        var strseniorCitizenDiscount = SetPrefixPostfix(AccountingFormat(Popup.Discount.seniorCitizenDiscount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SC Discount Amount based on Vat-Ex Sale: ', 20, strseniorCitizenDiscount, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;

                        var strseniorCitizenNetAmount = SetPrefixPostfix(AccountingFormat(Popup.Discount.seniorCitizenNetAmount || '0'), intFAFCTotalLenght - 32, ' ', 0);
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Net Amount to be Paid by SC: ', 30, strseniorCitizenNetAmount, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;

                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
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

                    var strSalesPayAmountDue = SetPrefixPostfix(AccountingFormat((Popup.Discount.isSeniorCitizenDiscount ? $('#txtTotal').text() : strNetAmountDue) || '0'), intFAFCTotalLenght - 22, ' ', 0);
                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Amount Due:   ', 20, strSalesPayAmountDue, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;
                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                    if (vDataPayment.Rows.length > 0) {

                        var strCashPayment = SetPrefixPostfix(AccountingFormat((vDataPayment.Rows[0].CashPayment || '0')), intFAFCTotalLenght - 22, ' ', 0);
                        var strCheckPayment = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CheckPaymentAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                        var strDebitCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].DebitCardAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                        var strCreditCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CreditCardAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                        var strSRVAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].SRVAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                        var strOtherTypeAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].OtherTypeAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                        var strCustomerChangeCash = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CustomerChangeCash || '0'), intFAFCTotalLenght - 22, ' ', 0);

                        strPrint = '\n';
                        if (vDataPayment.Rows[0].CashPayment > 0) {
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Cash:', 20, strCashPayment, intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;
                        }
                        if (vDataPayment.Rows[0].CheckPaymentAmount > 0) {
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Check:', 20, strCheckPayment, intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;
                        }
                        if (vDataPayment.Rows[0].DebitCardAmount > 0) {
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Debit Card:', 20, strDebitCardAmount, intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;
                        }
                        if (vDataPayment.Rows[0].CreditCardAmount > 0) {
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Credit Card:', 20, strCreditCardAmount, intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;
                        }
                        if (vDataPayment.Rows[0].SRVAmount > 0) {
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SRV:', 20, strSRVAmount, intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;
                        }
                        if (vDataPayment.Rows[0].OtherTypeAmount > 0) {
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Other Tender:', 20, strOtherTypeAmount, intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;
                        }

                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Change:', 20, strCustomerChangeCash, intFAFCTotalLenght, '', 0, 0);
                        strPrint = strPrint + strTemp;

                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                        if (vDataPayment.Rows[0].CheckPaymentAmount > vDataFooter.Rows[0].NetAmountDue) {
                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '* Excess payments made through checks are not entitled to cash refunds.', intFAFCTotalLenght, '', 0, '', 0, 0);
                            ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                        }

                    }

                    strPrint = '';
                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                    strPrint = '';
                    var strVATSales = SetPrefixPostfix(AccountingFormat(vDataInvHeader.Rows[0].TaxSales || '0'), intFAFCTotalLenght - 20, ' ', 0);
                    var strVATAmount = SetPrefixPostfix(AccountingFormat(vDataInvHeader.Rows[0].TaxAmount || '0'), intFAFCTotalLenght - 15, ' ', 0);
                    var strNonVAT = SetPrefixPostfix(AccountingFormat(vDataInvHeader.Rows[0].NonVATSale || '0'), intFAFCTotalLenght - 22, ' ', 0);
                    var strVATExempt = SetPrefixPostfix(AccountingFormat(vDataInvHeader.Rows[0].VATExemptSale || '0'), intFAFCTotalLenght - 22, ' ', 0);
                    var strVATZeroRated = SetPrefixPostfix(AccountingFormat(vDataInvHeader.Rows[0].ZeroRatedSale || '0'), intFAFCTotalLenght - 25, ' ', 0);
                    var strVATExemptTax = SetPrefixPostfix('-', intFAFCTotalLenght - 22, ' ', 0);

                    if (Popup.Discount.isSeniorCitizenDiscount) {
                        strVATSales = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].VatablePortionOfSCDTransaction || '0'), intFAFCTotalLenght - 20, ' ', 0);
                        strVATAmount = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].VatAmountPortionOfSCDTransaction || '0'), intFAFCTotalLenght - 15, ' ', 0);
                        strNonVAT = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].NonVATSale || '0'), intFAFCTotalLenght - 22, ' ', 0);
                        strVATExempt = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].VATExemptSale || '0'), intFAFCTotalLenght - 22, ' ', 0);
                        strVATExemptTax = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].VATExemptTax || '0'), intFAFCTotalLenght - 22, ' ', 0);
                        strVATZeroRated = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].ZeroRatedSale || '0'), intFAFCTotalLenght - 25, ' ', 0);

                        //strVATSales = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 22, ' ', 0);
                        //strVATAmount = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 17, ' ', 0);
                        //strNonVAT = SetPrefixPostfix(AccountingFormat(strTotalAmountDue), intFAFCTotalLenght - 24, ' ', 0);
                        //strVATExempt = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 24, ' ', 0);
                        //strVATZeroRated = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 27, ' ', 0);
                    }

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales: ', 18, strVATSales, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount: ', 13, strVATAmount, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT Sales: ', 20, strNonVAT, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Exempt Sales: ', 20, strVATExempt, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Exempt Tax: ', 20, strVATExemptTax, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT Sales: ', 23, strVATZeroRated, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                    strPrint = '';
                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Customer', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerName || '')), intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Address', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerAddress || '')), intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerTIN || '')), intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);


                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                    strPrint = '';

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Accredited Supplier', 20, ' ', 1), 19, ': ' + proj.AccreditedSupplier, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Registered Address', 20, ' ', 1), 19, ': ' + proj.RegisteredAddress, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN-Branch Code', 20, ' ', 1), 19, ': ' + proj.TinBranchCode, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('POS Accreditation', 20, ' ', 1), 19, ': ' + $.trim((vDataHeader.Rows[0].POSAccreditationNo || '')), intFAFCTotalLenght, '', 0, 0);
                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('POS Accreditation', 20, ' ', 1), 19, ': ' + proj.PosAccreditation, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                    strPrint = '';

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Machine S/N', 20, ' ', 1), 19, ': ' + $.trim((vDataInvHeader.Rows[0].UuidDisplay || Web.DeviceSerial() || '')), intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('POS Reg. No./Permit To Use (PTU) No.', 20, ' ', 1), 19, ': ' + $.trim((vDataHeader.Rows[0].POSPermitNo || '')), intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Machine ID No. (MIN)', 20, ' ', 1), 19, ': ' + vDataHeader.Rows[0].MIN + ' \n', intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Date  Issued ', 20, ' ', 1), 19, ': ' + proj.PosAccreditationDateIssued, intFAFCTotalLenght, '', 0, 0);
                    strPrint = strPrint + strTemp;

                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Valid Until', 20, ' ', 1), 19, ': ' + proj.PosAccreditationValidUntil + ' \n', intFAFCTotalLenght, '', 0, 0);
                    //strPrint = strPrint + strTemp;

                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                    // 2022_07_14__13_09_45_3756
                    //strPrint = '';
                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim('THIS INVOICE/RECEIPT SHALL BE VALID FOR FIVE(5) YEARS FROM THE DATE OF THE PERMIT TO USE'), intFAFCTotalLenght, '', 0, '', 0, 1);

                    strPrint = strPrint + strTemp;

                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                    //Senior Citizen/Person With Disability info
                    //if (isSeniorCitizenDiscount) {
                    //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Senior Citizen/PWD TIN: ', 12, "Emilio Aguinaldo", intFAFCTotalLenght, '', 0, 0);
                    //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                    //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'OSCA ID No./PWD ID No.: ', 12, "8700-324", intFAFCTotalLenght, '', 0, 0);
                    //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                    //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Signature: ', 12, "Heiroglophyics \n", intFAFCTotalLenght, '', 0, 0);
                    //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                    //}

                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '----------------------------------------', intFAFCTotalLenght, '', 0, '', 0, 1);
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                    //Get Footer Messages
                    strPrint = '';
                    //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Use for testing only. This is not an Official Receipt.', intFAFCTotalLenght, '', 0, '', 0, 1);
                    //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'This Serves as Your Official Receipt.', intFAFCTotalLenght, '', 0, '', 0, 1);
                    strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataInvHeader.Rows[0].ReceiptFooterMsg, intFAFCTotalLenght, '', 0, '', 0, 1);
                    strPrint = strPrint + strTemp;

                    if (vDataMsg.Rows.length > 0) {
                        for (var p = 0; p <= vDataMsg.Rows.length - 1; p++) {
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataMsg.Rows[p].MessageFooter || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;
                        };

                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                        //gipatanggal ni sir robert (may 10)
                        //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', intFAFCTotalLenght, '', 0, '', 0, 1);
                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                        //breakdown

                        /*//Print Barcode
                        strPrint = 'SI|' + vDataInvHeader.Rows[0].OrganizationID + '|' + vDataInvHeader.Rows[0].OrganizationBranchID + '|' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0)
                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);
    
                        strPrint = SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 1, 0);*/

                        ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                        //vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();

                        //if (vDataPrint.Rows.length > 0) {
                        //    PrintText(vDataPrint.Rows[0].PrinterIPAddress);
                        //    //ClearCommandBuffer();
                        //}

                        //Print SRV
                        //PrintSalesPaymentSRVReceipt(0, pInvoiceID);

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
            }
        }

        if (vDataPrint.Rows.length > 0) {
            PrintText(vDataPrint.Rows[0].PrinterIPAddress);
            ClearCommandBuffer();
        }
    }

    function PrintReceiptViaWindowsPrintServer(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg) {
        var strTemp = '';
        var strPrint = '';
        var strPrinterIP = '';
        var strPrinterName = '';
        var strStockID = '';
        var IsShowSalesRep = false;
        var pData = {};
        var newPrinterIP = '';

        var intFAFCTotalLenght = 40;
        var isSeniorCitizenDiscount = false;

        pData.InvoiceID = pInvoiceID;
        Android.AddLog(Popup.Discount.isSeniorCitizenDiscount);

        //Get Printer IP Address
        if (vDataPrint == undefined) {
            MessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                "Printer",
                [
                    {
                        Text: 'Ok',
                        Click: function () {

                            $.mobile.changePage('');

                            if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')
                                window.pageHeader.ShowPaymentsPage(false);

                        }
                    }
                ]
        );
            return;
        }

        if (vDataPrint.Rows.length > 0) {
            //additional by janine jan 31 2015
            originalDevFowstPrinterID = vDataPrint.Rows[0].ID;
            if (CStr2(newPrinterIP) != '')
                strPrinterIP = newPrinterIP;
            else {
                strPrinterIP = vDataPrint.Rows[0].PrinterIPAddress;
                strPrinterName = vDataPrint.Rows[0].PrinterName;
            }


            //New set of text to print
            //NewPrintBuilder();

            //Get Header
            IsShowSalesRep = false;
            var mDate = '';

            if (InAndroid) {
                mDate = vDataInvHeader.Rows[0].InvoiceDate;
            }
            else {
                mDate = vDataInvHeader.Rows[0].InvoiceDate.replace(/-/g, '/');
            }

            if (vDataHeader.Rows.length > 0) {

                IsShowSalesRep = (vDataHeader.Rows[0].IsShowSalesRep == 1 ? true : false);

                if (CBool(vDataHeader.Rows[0].IsAuthorizedForPOS) || CBool(vDataHeader.Rows[0].ShowCompanyDetailsInHeader)) {
                    WindowsPrinter($.trim((vDataHeader.Rows[0].BusinessTradeName || '')), [0, 0, 0, 1, 0, 36]);

                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                    strPrint = '';
                    WindowsPrinter($.trim((vDataHeader.Rows[0].CorporateName || '')), [0, 0, 0, 1, 0, 36]);
                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].CorporateName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);                            
                    //strPrint = strPrint + strTemp;

                    WindowsPrinter('VAT TIN ' + $.trim((vDataHeader.Rows[0].TIN || '')), [0, 0, 0, 1, 0, 36]);
                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT TIN ' + $.trim((vDataHeader.Rows[0].TIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                    //strPrint = strPrint + strTemp;

                    WindowsPrinter($.trim((vDataHeader.Rows[0].CompleteAddress || '')), [1, 0, 0, 1, 0, 36]);
                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].CompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                    //strPrint = strPrint + strTemp;

                    WindowsPrinter('Tel. No.' + $.trim((vDataHeader.Rows[0].TelNo || '')), [1, 0, 0, 1, 0, 36]);
                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataHeader.Rows[0].TelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                    //strPrint = strPrint + strTemp;

                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                    //strPrint = strPrint + strTemp;

                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                }
                //Get Items
                if (vDataInvHeader.Rows.length > 0) {
                    strPrint = '';
                    WindowsPrinter('Customer : ' + $.trim((vDataInvHeader.Rows[0].CustomerName || '')), [0, 0, 0, 0, 0, 36]);
                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Customer', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerName || '')), intFAFCTotalLenght, '', 0, 0);
                    //strPrint = strPrint + strTemp;

                    WindowsPrinter('Address : ' + $.trim((vDataInvHeader.Rows[0].CustomerAddress || '')), [0, 0, 0, 0, 0, 36]);
                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Address', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerAddress || '')), intFAFCTotalLenght, '', 0, 0);
                    //strPrint = strPrint + strTemp;

                    if (CBool(vDataHeader.Rows[0].IsAuthorizedForPOS) || CBool(vDataHeader.Rows[0].ShowCompanyDetailsInHeader)) {
                        WindowsPrinter('TIN : ' + $.trim((vDataInvHeader.Rows[0].CustomerTIN || '')), [0, 0, 0, 0, 0, 36]);
                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerTIN || '')), intFAFCTotalLenght, '', 0, 0);
                        //strPrint = strPrint + strTemp;
                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                    }

                    if (CStr2(vDataInvHeader.Rows[0].CustomerTableNo) != '') {
                        var prefix = (vDataInvHeader.Rows[0].CustomerTableNo).substring((vDataInvHeader.Rows[0].CustomerTableNo).lastIndexOf('-'));

                        if ($.trim((vDataInvHeader.Rows[0].CustomerTableNo || '')) != '') {
                            WindowsPrinter('Table # : ' + CDec2(Math.abs(prefix)), [0, 0, 0, 0, 0, 36]);
                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Table #', 8, ' ', 1), 7, ': ' + $.trim((prefix[1] || '')), intFAFCTotalLenght, '', 0, 0);
                            //strPrint = strTemp;
                            //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                        }
                    }

                    if (CBool(vDataHeader.Rows[0].IsAuthorizedForPOS) || CBool(vDataHeader.Rows[0].ShowCompanyDetailsInHeader)) {
                        WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                        //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                    }

                    //strPrint = '';
                    WindowsPrinter('SI ' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0), [0, 0, 0, 1, 0, 36]);
                    //strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'SI ' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                    //strPrint = strPrint + strTemp;

                    WindowsPrinter(($.format.date(new Date(mDate), 'MMM dd, yyyy HH:mm') || ''), [0, 0, 0, 1, 0, 36]);

                    //strTemp = FormatTextToPrint(intFCDoubleTotalLenght, ($.format.date(new Date(mDate), 'MMM dd, yyyy HH:mm') || ''), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                    //strPrint = strPrint + strTemp;

                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);
                    WindowsPrinter('Cashier : ' + $.trim((vDataInvHeader.Rows[0].CashierName || '')), [0, 0, 0, 0, 0, 36]);
                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Cashier: ' + ($.trim(vDataInvHeader.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, '', 0, 0);
                    //ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);

                    if (IsShowSalesRep) {
                        var SalesRepList = [];
                        var SalesRepConcatenated;
                        var IsSalesRepExist = false;

                        for (var p = 0; p <= vDataItem.Rows.length - 1; p++) {
                            IsSalesRepExist = false;
                            for (var i = 0; i <= SalesRepList.length - 1; i++) {

                                if (SalesRepList[i] == vDataItem.Rows[p].SalesPerson) {
                                    IsSalesRepExist = true;
                                }

                            }

                            if (!IsSalesRepExist) {
                                SalesRepList[SalesRepList.length] = vDataItem.Rows[p].SalesPerson;
                                if (p == 0) {
                                    SalesRepConcatenated = vDataItem.Rows[p].SalesPerson;
                                } else {
                                    SalesRepConcatenated += ', ' + vDataItem.Rows[p].SalesPerson;
                                }
                            }

                        }

                        WindowsPrinter('Sales Person: ' + $.trim((SalesRepConcatenated || '')), [1, 0, 0, 0, 0, 36]);
                    }

                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                    //Get Items
                    var strProductID = "";
                    if (vDataItem.Rows.length > 0) {
                        strStockID = '';
                        strPrint = '';
                        var intLimitPerBatchPrint = 0;
                        for (var p = 0; p <= vDataItem.Rows.length - 1; p++) {

                            var strStockDescription = $.trim((vDataItem.Rows[p].StockDescription || ''));
                            var strUnitPrice = SetPrefixPostfix(AccountingFormat(vDataItem.Rows[p].UnitPrice || '0'), intFAFCTotalLenght - 30, ' ', 0);
                            var strQty = SetPrefixPostfix(AccountingFormat((vDataItem.Rows[p].Quantity || '0'), 0), 8, ' ', 0);
                            strQty = strQty.replace(/\s/g, '');
                            var strUnitMeasure = SetPrefixPostfix((vDataItem.Rows[p].UnitMeasure || ''), 10, ' ', 0);
                            strUnitMeasure = strUnitMeasure.replace(/\s/g, '');
                            var strSubTotal = SetPrefixPostfix(AccountingFormat(vDataItem.Rows[p].SubTotal || '0'), intFAFCTotalLenght - 26, ' ', 0);

                            strPrint = '';
                            if (strProductID == "") {
                                strProductID = vDataItem.Rows[p].StockID;
                            }

                            if (vDataItemSerial.Rows.length > 0) {
                                if (strProductID != vDataItem.Rows[p].StockID) { //Print Serials

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Serial(s) : ', intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    for (var intSerial = 0; intSerial <= vDataItemSerial.Rows.length - 1; intSerial++) {

                                        if (strProductID == vDataItemSerial.Rows[intSerial].ProductID && vDataItemSerial.Rows[intSerial].InvoiceID == pInvoiceID) {

                                            var strSerial = $.trim((vDataItemSerial.Rows[intSerial].SerialNumber || ''));

                                            WindowsPrinter('Serials(s) : ' + strSerial, [1, 0, 0, 0, 0, 36]);

                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, strSerial, intFAFCTotalLenght, '', 0, '', 0, 0);
                                            //strPrint = strPrint + strTemp;

                                        }
                                    }

                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    strProductID = vDataItem.Rows[p].StockID;
                                }
                            }


                            if (vDataPrint.Rows[0].ProductDescriptionInReceipt != undefined) {
                                if (vDataPrint.Rows[0].ProductDescriptionInReceipt == 'ProductShortDescription') {

                                    strStockDescription = $.trim((vDataItem.Rows[p].ReceiptName || vDataItem.Rows[p].StockDescription));

                                    WindowsPrinter(strQty + ' ' + strUnitMeasure + ' ' + strStockDescription, [0, 0, 0, 0, 0, 36], strSubTotal, [0, 0, 0, 0, 1, 36]);

                                } else {
                                    WindowsPrinter($.trim(strStockDescription), [1, 0, 0, 0, 0, 36]);
                                    WindowsPrinter(' ' + $.trim(strQty) + ' @ ' + $.trim(strUnitPrice) + ' / ' + $.trim(strUnitMeasure), [0, 0, 0, 0, 0, 36], AccountingFormat((CDec2($.trim(strQty)) * CDec2($.trim(strUnitPrice)) || '0')), [0, 0, 0, 0, 1, 36]);
                                }
                            } else {
                                WindowsPrinter($.trim(strStockDescription), [1, 0, 0, 0, 0, 36]);
                                WindowsPrinter(' ' + $.trim(strQty) + ' @ ' + $.trim(strUnitPrice) + ' / ' + $.trim(strUnitMeasure), [0, 0, 0, 0, 0, 36], AccountingFormat((CDec2($.trim(strQty)) * CDec2($.trim(strUnitPrice)) || '0')), [0, 0, 0, 0, 1, 36]);
                            }

                            if (vDataItemSerial.Rows.length > 0) {
                                if (p == vDataItem.Rows.length - 1) {

                                    WindowsPrinter('Serial(s) : ', [1, 16, 0, 0, 0, 36], ': ' + $.trim((vDataItem.Rows[p].SalesPerson || '')));

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Serial(s) : ', intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    for (var intSerial = 0; intSerial <= vDataItemSerial.Rows.length - 1; intSerial++) {

                                        if (strProductID == vDataItemSerial.Rows[intSerial].ProductID && vDataItemSerial.Rows[intSerial].InvoiceID == pInvoiceID) {

                                            var strSerial = $.trim((vDataItemSerial.Rows[intSerial].SerialNumber || ''));

                                            WindowsPrinter(strSerial, [1, 16, 0, 0, 0, 36]);
                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, strSerial, intFAFCTotalLenght, '', 0, '', 0, 0);
                                            //strPrint = strPrint + strTemp;

                                        }
                                    }

                                    //WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    strProductID = vDataItem.Rows[p].StockID;
                                }
                            } else {
                                //WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                //strPrint = strPrint + strTemp;
                            }

                            if (vDataItem.Rows[p].ItemModifier != undefined) {
                                if (vDataItem.Rows[p].ItemModifier.length > 0) {
                                    for (var im = 0; im < vDataItem.Rows[p].ItemModifier.length; im++) {
                                        var mod = vDataItem.Rows[p].ItemModifier[im];
                                        WindowsPrinter('* ' + mod.Description, [0, 0, 0, 0, 0, 36]);

                                        if (CStr2(mod.OrderedQty) != '') {
                                            var itemmodifiertotal = AccountingFormat(CDec2(mod.OrderedQty) * CDec2(mod.ModifierPrice) * CDec2(strQty));
                                            WindowsPrinter(' ' + mod.OrderedQty + ' @ ' + AccountingFormat($.trim(mod.ModifierPrice)) + ' @ ' + strQty + ' ' + strUnitMeasure, [0, 0, 0, 0, 0, 36], AccountingFormat((itemmodifiertotal || '0')), [0, 0, 0, 0, 1, 36]);
                                        }
                                    }
                                    WindowsPrinter('   Item Total:  ---->', [0, 0, 0, 0, 0, 36], AccountingFormat((strSubTotal || '0')), [0, 0, 0, 0, 1, 36]);
                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                }
                            } else {
                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                            }

                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                            intLimitPerBatchPrint = intLimitPerBatchPrint + 1;

                            if (intLimitPerBatchPrint >= 100 && vDataItem.Rows.length > 100) {

                                PrintText(strPrinterIP);
                                ClearCommandBuffer();

                                //New set of text to print because it causes error when you print directly all items at once.
                                NewPrintBuilder();
                                intLimitPerBatchPrint = 0;
                            }

                        };

                        //Get Footer Amounts
                        if (vDataPayment.Rows.length > 0) {

                            //Get Footer Amounts
                            if (vDataFooter.Rows.length > 0) {

                                var strSKUCount = SetPrefixPostfix(AccountingFormat((vDataFooter.Rows[0].SKUCount || '0'), 0), intFAFCTotalLenght - 14, ' ', 0);
                                var strTotalAmountDue = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].SubTotal || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                var strTotalDiscount = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].DiscountAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                var strNetAmountDue = SetPrefixPostfix(AccountingFormat(CDec2(vDataFooter.Rows[0].NetAmountDue) + (Popup.Discount.isSeniorCitizenDiscount ? 0 : CDec2(vDataFooter.Rows[0].ChargeAmount)) + CDec2(vDataFooter.Rows[0].TipAmount) + CDec2(vDataFooter.Rows[0].LocalTaxAmount)), intFAFCTotalLenght - 22, ' ', 0);

                                var strTotalCharges = SetPrefixPostfix(vDataFooter.Rows[0].ChargeAmount || '0', intFAFCTotalLenght - 22, ' ', 0);
                                var strTip = SetPrefixPostfix(vDataFooter.Rows[0].TipAmount || '0', intFAFCTotalLenght - 22, ' ', 0);

                                var strVatSale = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TotalAmountDue), intFAFCTotalLenght - 22, ' ', 0);
                                var strVatAmount = SetPrefixPostfix(AccountingFormat(CDec2(vDataFooter.Rows[0].SubTotal) - CDec2(vDataFooter.Rows[0].TotalAmountDue)), intFAFCTotalLenght - 22, ' ', 0);

                                var strOtherTax = SetPrefixPostfix(vDataFooter.Rows[0].LocalTaxAmount || '0', intFAFCTotalLenght - 22, ' ', 0);

                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);

                                WindowsPrinter('SKU Count: ', [0, 0, 0, 0, 0, 36], CDec2((vDataFooter.Rows[0].SKUCount || '0')), [0, 0, 0, 0, 1, 36]);
                                //strPrint = '';
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SKU Count: ', 12, strSKUCount, intFAFCTotalLenght, '', 0, 0);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter('Gross Total: ', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].SubTotal || '0'), [0, 0, 0, 0, 1, 36]);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sub Total: ', 20, strTotalAmountDue, intFAFCTotalLenght, '', 0, 0);
                                //strPrint = strPrint + strTemp;

                                if (Popup.Discount.isSeniorCitizenDiscount) {
                                    //WindowsPrinter('Less: ' + vDataFooter.Rows[0].TaxPercent + '% VAT ', [0, 0, 0, 0, 0, 36], AccountingFormat($.trim($('#lblVATPercent').text())), [0, 0, 0, 0, 1, 36]);
                                    ////strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: ' + vDataFooter.Rows[0].TaxPercent + '% VAT ', 20, strVatAmount, intFAFCTotalLenght, '', 0, 0);
                                    ////strPrint = strPrint + strTemp;

                                    //WindowsPrinter('Total Amount: ', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].TotalAmountDue - CDec2($.trim($('#lblVATPercent').text()))), [0, 0, 0, 0, 1, 36]);
                                    ////strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Amount: ', 20, strVatSale, intFAFCTotalLenght, '', 0, 0);
                                    ////strPrint = strPrint + strTemp;

                                    //WindowsPrinter('Total Amount: ', [0, 0, 0, 0, 0, 36], AccountingFormat($.trim($('#lblSalesPayTotalDiscount').text())), [0, 0, 0, 0, 1, 36]);


                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    WindowsPrinter('________________________________________', [0, 0, 0, 0, 0, 36], '', [0, 0, 0, 0, 1, 36]);

                                    if (Popup.Discount.SCDiscountExempt > 0) {
                                        //var strSCDiscountExempt = SetPrefixPostfix(AccountingFormat(SCDiscountExempt || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SC Discount-Exempt: ', 20, strSCDiscountExempt, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;
                                        WindowsPrinter('SC Discount-Exempt: ', [0, 0, 0, 0, 0, 36], AccountingFormat(CDec2(Popup.Discount.SCDiscountExempt)), [0, 0, 0, 0, 1, 36]);
                                    }

                                    //var strSubTotalOfVatExemptItems = SetPrefixPostfix(AccountingFormat(SubTotalOfVatExemptItems || '0'), intFAFCTotalLenght - 31, ' ', 0);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, '\nGross Amount Subject to SCD: ', 29, strSubTotalOfVatExemptItems, intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;
                                    WindowsPrinter('\nGross Amount Subject to SCD: ', [1, 22, 0, 0, 0, 36], AccountingFormat(CDec2(Popup.Discount.SubTotalOfVatExemptItems)), [0, 0, 0, 0, 1, 36]);

                                    if (Popup.Discount.GrossSalesToNoneSC > 0) {
                                        //var strGrossSalesToNoneSC = SetPrefixPostfix(AccountingFormat(Popup.Discount.GrossSalesToNoneSC || '0'), intFAFCTotalLenght - 31, ' ', 0);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Gross Sales to None-SC ' + (CInt2($('#txtSalesPayNoOfDiners').val()) - CInt2($('#txtSalesPayNoOfSC').val())) + '/' + $('#txtSalesPayNoOfDiners').val() + ':', 29, strGrossSalesToNoneSC, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;
                                        WindowsPrinter('Gross Sales to None-SC ' + (CInt2($('#txtSalesPayNoOfDiners').val()) - CInt2($('#txtSalesPayNoOfSC').val())) + '/' + $('#txtSalesPayNoOfDiners').val() + ':', [1, 22, 0, 0, 0, 36], AccountingFormat(CDec2(Popup.Discount.GrossSalesToNoneSC)), [0, 0, 0, 0, 1, 36]);
                                    }

                                    //var strGrossSalesToSC = SetPrefixPostfix(AccountingFormat(GrossSalesToSC || '0'), intFAFCTotalLenght - 23, ' ', 0);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, '\nGross Sales to SC ' + $('#txtSalesPayNoOfSC').val() + '/' + $('#txtSalesPayNoOfDiners').val() + ':', 21, strGrossSalesToSC, intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;
                                    WindowsPrinter('\nGross Sales to SC ' + $('#txtSalesPayNoOfSC').val() + '/' + $('#txtSalesPayNoOfDiners').val() + ':', [1, 22, 0, 0, 0, 36], AccountingFormat(CDec2(Popup.Discount.GrossSalesToSC)), [0, 0, 0, 0, 1, 36]);

                                    //var strVatExemptSales = SetPrefixPostfix(AccountingFormat(VatExemptSales || '0') + (IsApplyMaxSCDiscountableAmount ? '' : '*'), intFAFCTotalLenght - 22, ' ', 0);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Ex for SC: ', 20, strVatExemptSales, intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;
                                    WindowsPrinter('VAT-Ex for SC: ', [0, 0, 0, 0, 0, 36], AccountingFormat(CDec2(Popup.Discount.VatExemptSales)), [0, 0, 0, 0, 1, 36]);

                                    //var strMaxSCDiscountableAmount = SetPrefixPostfix(AccountingFormat(MaxSCDiscountableAmount || '0') + (IsApplyMaxSCDiscountableAmount ? '*' : ''), intFAFCTotalLenght - 25, ' ', 0);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Max Discountable VAT-EX(' + (MaxSCDiscountableAmount / CInt2($('#txtSalesPayNoOfSC').val())) + '*' + CInt2($('#txtSalesPayNoOfSC').val()) + ')', 23, strMaxSCDiscountableAmount, intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;
                                    WindowsPrinter('Max Discountable VAT-EX(' + (CDec2(Popup.Discount.MaxSCDiscountableAmount) / CInt2($('#txtSalesPayNoOfSC').val())) + '*' + CInt2($('#txtSalesPayNoOfSC').val()) + ')', [1, 22, 0, 0, 0, 36], AccountingFormat(CDec2(Popup.Discount.MaxSCDiscountableAmount)), [0, 0, 0, 0, 1, 36]);

                                    //var strDiscountPercentCeiling = SetPrefixPostfix(AccountingFormat($('#txtDiscountPercent').val()) + '%', intFAFCTotalLenght - 22, ' ', 0);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SC Discount Rate: ', 20, strDiscountPercentCeiling, intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;
                                    WindowsPrinter('SC Discount Rate: ', [1, 22, 0, 0, 0, 36], AccountingFormat($('#txtDiscountPercentCeiling').val()) + '%', [0, 0, 0, 0, 1, 36]);

                                    //var strseniorCitizenDiscount = SetPrefixPostfix(AccountingFormat(seniorCitizenDiscount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SC Discount Amount based on Vat-Ex Sale: ', 20, strseniorCitizenDiscount, intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;
                                    WindowsPrinter('SC Discount Amount based on Vat-Ex Sale: ', [1, 22, 0, 0, 0, 36], AccountingFormat(Popup.Discount.seniorCitizenDiscount), [0, 0, 0, 0, 1, 36]);

                                    //var strseniorCitizenNetAmount = SetPrefixPostfix(AccountingFormat(seniorCitizenNetAmount || '0'), intFAFCTotalLenght - 32, ' ', 0);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Net Amount to be Paid by SC: ', 30, strseniorCitizenNetAmount, intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;
                                    WindowsPrinter('Net Amount to be Paid by SC: ', [1, 22, 0, 0, 0, 36], AccountingFormat(Popup.Discount.seniorCitizenNetAmount), [0, 0, 0, 0, 1, 36]);

                                    WindowsPrinter('________________________________________', [0, 0, 0, 0, 0, 36], '', [0, 0, 0, 0, 1, 36]);


                                } else {
                                    WindowsPrinter('Total Discount: ', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].DiscountAmount || '0'), [0, 0, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Discount:   ', 20, strTotalDiscount, intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;
                                }


                                if (CDec2(vDataFooter.Rows[0].ChargeAmount) > 0) {
                                    WindowsPrinter('Total Charges: ', [0, 0, 0, 0, 0, 36], AccountingFormat(CDec2(vDataFooter.Rows[0].ChargeAmount || '0')), [0, 0, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Charges:   ', 20, strTotalCharges, intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;
                                }

                                if (CDec2(vDataFooter.Rows[0].TipAmount) > 0) {
                                    WindowsPrinter('Tip: ', [0, 0, 0, 0, 0, 36], AccountingFormat(CDec2(vDataFooter.Rows[0].TipAmount || '0')), [0, 0, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tip:   ', 20, strTip, intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                }

                                if (CDec2(vDataFooter.Rows[0].LocalTaxAmount) > 0) {
                                    WindowsPrinter('Local Tax:   ', [0, 0, 0, 0, 0, 36], AccountingFormat(CDec2(vDataFooter.Rows[0].LocalTaxAmount || '0')), [0, 0, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Local Tax:   ', 20, strOtherTax, intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;
                                }

                                if (Popup.Discount.isSeniorCitizenDiscount) {
                                    strNetAmountDue = AccountingFormat(CDec2(strTotalAmountDue) - CDec2($.trim($('#lblVATPercent').text())) - CDec2($.trim($('#lblSalesPayTotalDiscount').text())));
                                    WindowsPrinter('Amount Due: ', [0, 0, 0, 0, 0, 36], AccountingFormat(CDec2(strNetAmountDue)), [0, 0, 0, 0, 1, 36]);
                                } else {
                                    WindowsPrinter('Amount Due: ', [0, 0, 0, 0, 0, 36], AccountingFormat(CDec2(vDataFooter.Rows[0].NetAmountDue) + CDec2(vDataFooter.Rows[0].ChargeAmount) + CDec2(vDataFooter.Rows[0].TipAmount) + CDec2(vDataFooter.Rows[0].LocalTaxAmount)), [0, 0, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Amount Due:   ', 20, strNetAmountDue, intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                }


                                if (vDataPayment.Rows.length > 0) {

                                    var strCashPayment = SetPrefixPostfix(AccountingFormat((vDataPayment.Rows[0].CashPayment || '0')), intFAFCTotalLenght - 7, ' ', 0);
                                    var strCheckPayment = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CheckPaymentAmount || '0'), intFAFCTotalLenght - 8, ' ', 0);
                                    var strDebitCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].DebitCardAmount || '0'), intFAFCTotalLenght - 14, ' ', 0);
                                    var strCreditCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CreditCardAmount || '0'), intFAFCTotalLenght - 14, ' ', 0);
                                    var strSRVAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].SRVAmount || '0'), intFAFCTotalLenght - 6, ' ', 0);
                                    var strOtherTypeAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].OtherTypeAmount || '0'), intFAFCTotalLenght - 14, ' ', 0);
                                    var strCustomerChangeCash = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CustomerChangeCash || '0'), intFAFCTotalLenght - 9, ' ', 0);

                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                    if (vDataPayment.Rows[0].CashPayment > 0) {
                                        WindowsPrinter('Cash:', [0, 0, 0, 0, 0, 36], AccountingFormat((vDataPayment.Rows[0].CashPayment || '0')), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Cash:', 5, strCashPayment, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;
                                    }
                                    if (vDataPayment.Rows[0].CheckPaymentAmount > 0) {
                                        WindowsPrinter('Check:', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataPayment.Rows[0].CheckPaymentAmount || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Check:', 6, strCheckPayment, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;
                                    }
                                    if (vDataPayment.Rows[0].DebitCardAmount > 0) {
                                        WindowsPrinter('Debit Card:', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataPayment.Rows[0].DebitCardAmount || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Debit Card:', 12, strDebitCardAmount, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;
                                    }
                                    if (vDataPayment.Rows[0].CreditCardAmount > 0) {
                                        WindowsPrinter('Credit Card:', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataPayment.Rows[0].CreditCardAmount || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Credit Card:', 12, strCreditCardAmount, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;
                                    }
                                    if (vDataPayment.Rows[0].SRVAmount > 0) {
                                        WindowsPrinter('SRV:', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataPayment.Rows[0].SRVAmount || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SRV:', 4, strSRVAmount, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;
                                    }
                                    if (vDataPayment.Rows[0].OtherTypeAmount > 0) {
                                        var oSql = 'Select PaymentType from ' +
                                        'TransCustomerPaymentInvoices inner join ' +
                                        'TransCustomerPaymentOther on TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentOther.CustomerPaymentID ' +
                                        'where TransCustomerPaymentInvoices.InvoiceID = ' + ReadyString(sessionStorage.CartOrderID)

                                        var pdtCustomerPaymentOther = Sql.Execute(oSql);

                                        WindowsPrinter('Other Tender(' + pdtCustomerPaymentOther.Rows[0].PaymentType + '):', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataPayment.Rows[0].OtherTypeAmount || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Other Tender:', 12, strOtherTypeAmount, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;
                                    }
                                    if (vDataPayment.Rows[0].OnCreditAmount > 0) {
                                        WindowsPrinter('On Credit:', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataPayment.Rows[0].OnCreditAmount || '0'), [0, 0, 0, 0, 1, 36]);
                                        WindowsPrinter('Remarks:', [0, 0, 0, 0, 0, 36], (vDataPayment.Rows[0].OnCreditRemarks || ''), [0, 0, 0, 0, 1, 36]);
                                        WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                        WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                        WindowsPrinter('Name and Signature', [0, 0, 0, 1, 0, 36]);
                                    }

                                    WindowsPrinter('Change:', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataPayment.Rows[0].CustomerChangeCash || '0'), [0, 0, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Change:', 7, strCustomerChangeCash, intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    if (vDataPayment.Rows[0].CheckPaymentAmount > vDataFooter.Rows[0].NetAmountDue) {
                                        WindowsPrinter('* Excess payments made through checks are not entitled to cash refunds.', [0, 0, 0, 0, 0, 36]);
                                        //strPrint = FormatTextToPrint(intFAFCTotalLenght, '* Excess payments made through checks are not entitled to cash refunds.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                        //ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                    }

                                }

                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);

                                //strPrint = '';
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                if (CBool(vDataHeader.Rows[0].IsAuthorizedForPOS) || CBool(vDataHeader.Rows[0].ShowCompanyDetailsInHeader)) {
                                    strPrint = '';
                                    var strVATSales = SetPrefixPostfix(AccountingFormat(vDataInvHeader.Rows[0].TaxSales || '0'), intFAFCTotalLenght - 20, ' ', 0);
                                    var strVATAmount = SetPrefixPostfix(AccountingFormat(vDataInvHeader.Rows[0].TaxAmount || '0'), intFAFCTotalLenght - 15, ' ', 0);
                                    var strNonVAT = SetPrefixPostfix(AccountingFormat(vDataInvHeader.Rows[0].NonVATSale || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                    var strVATExempt = SetPrefixPostfix(AccountingFormat(vDataInvHeader.Rows[0].VATExemptSale || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                    var strVATZeroRated = SetPrefixPostfix(AccountingFormat(vDataInvHeader.Rows[0].ZeroRatedSale || '0'), intFAFCTotalLenght - 25, ' ', 0);

                                    if (CDec2(vDataFooter.Rows[0].VATExemptSale > 0)) {
                                        WindowsPrinter('VAT-able Sales:', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].VatablePortionOfSCDTransaction || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales: ', 18, strVATSales, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;

                                        WindowsPrinter('VAT Amount: ', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].VatAmountPortionOfSCDTransaction || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount: ', 13, strVATAmount, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;

                                        WindowsPrinter('Non-VAT Sales: ', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].NonVATSale || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT Sales: ', 20, strNonVAT, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;

                                        WindowsPrinter('VAT Exempt Sales: ', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].VATExemptSale || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Exempt Sales: ', 20, strVATExempt, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;

                                        WindowsPrinter('VAT Exempt Tax: ', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].VATExemptTax || '0'), [0, 0, 0, 0, 1, 36]);

                                        WindowsPrinter('Zero-Rated VAT Sales: ', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].ZeroRatedSale || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT Sales: ', 23, strVATZeroRated, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;
                                    } else {
                                        WindowsPrinter('VAT-able Sales:', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].TaxSales || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales: ', 18, strVATSales, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;

                                        WindowsPrinter('VAT Amount: ', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].TaxAmount || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount: ', 13, strVATAmount, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;

                                        WindowsPrinter('Non-VAT Sales: ', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].NonVATSale || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT Sales: ', 20, strNonVAT, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;

                                        WindowsPrinter('VAT Exempt Sales: ', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].VATExemptSale || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Exempt Sales: ', 20, strVATExempt, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;

                                        WindowsPrinter('Zero-Rated VAT Sales: ', [0, 0, 0, 0, 0, 36], AccountingFormat(vDataFooter.Rows[0].ZeroRatedSale || '0'), [0, 0, 0, 0, 1, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT Sales: ', 23, strVATZeroRated, intFAFCTotalLenght, '', 0, 0);
                                        //strPrint = strPrint + strTemp;
                                    }
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                    strPrint = '';

                                    WindowsPrinter('Accredited Supplier : ' + proj.AccreditedSupplier, [0, 0, 0, 0, 0, 36]);

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Accredited Supplier', 20, ' ', 1), 19, ': 1MOBIZ, INC.', intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter('Registered Address  : ' + proj.RegisteredAddress, [0, 0, 0, 0, 0, 36]);

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Registered Address', 20, ' ', 1), 19, ': DAVAO CITY', intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter('TIN-Branch Code     : ' + proj.TinBranchCode, [0, 0, 0, 0, 0, 36]);

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN-Branch Code', 20, ' ', 1), 19, ': 462756815-000', intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp; 

                                    WindowsPrinter('POS Accreditation   : ', [1, 22, 0, 0, 0, 36], proj.PosAccreditation, [1, 14, 0, 0, 0, 36]);

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('POS Accreditation', 20, ' ', 1), 19, ': ' + $.trim((vDataHeader.Rows[0].POSAccreditationNo || '')), intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                    strPrint = '';

                                    WindowsPrinter('Machine S/N', [1, 22, 0, 0, 0, 36], ': ' + $.trim(vDataInvHeader.Rows[0].UuidDisplay || Web.DeviceSerial() || ''), [0, 14, 0, 0, 0, 36]);
                                    WindowsPrinter('POS Reg. No./Permit : To Use (PTU) No.', [1, 22, 0, 0, 0, 36], ': ' + $.trim((vDataHeader.Rows[0].POSPermitNo || '')), [0, 14, 0, 0, 0, 36]);

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('POS Reg. No./Permit To Use (PTU) No.', 20, ' ', 1), 19, ': ' + $.trim((vDataHeader.Rows[0].POSPermitNo || '')), intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter('Machine ID No. (MIN): ' + vDataHeader.Rows[0].MIN, [0, 0, 0, 0, 0, 36]);
                                    WindowsPrinter('Date  Issued: ' + proj.PosAccreditationDateIssued, [0, 0, 0, 0, 0, 36]);
                                    //WindowsPrinter('Valid Until: ' + proj.PosAccreditationValidUntil, [0, 0, 0, 0, 0, 36]);

                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Machine ID No. (MIN)', 20, ' ', 1), 19, ': ' + vDataHeader.Rows[0].MIN + ' \n', intFAFCTotalLenght, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    // 2022_07_14__13_09_45_3756
                                    // WindowsPrinter('THIS INVOICE/RECEIPT SHALL BE VALID FOR FIVE(5) YEARS FROM THE DATE OF THE PERMIT TO USE', [1, 0, 0, 0, 0, 36]);

                                    //strPrint = '';
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim('THIS INVOICE/RECEIPT SHALL BE VALID FOR FIVE(5) YEARS FROM THE DATE OF THE PERMIT TO USE'), intFAFCTotalLenght, '', 0, '', 0, 1);

                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    //Senior Citizen/Person With Disability info
                                    if (Popup.Discount.isSeniorCitizenDiscount) {
                                        //WindowsPrinter($.trim('SCD Details:'), [0, 0, 0, 0, 0, 36]);

                                        //WindowsPrinter('Number of SC: ' + $('#txtSalesPayNoOfSC').val() + ' of ' + $('#txtSalesPayNoOfDiners').val(), [0, 0, 0, 0, 0, 36]);

                                        //WindowsPrinter('Senior Citizen\'s Bill: ' + AccountingFormat(seniorCitizenNetAmount), [0, 0, 0, 0, 0, 36]);

                                        //WindowsPrinter('Non-Senior Citizen\'s Bill:' + AccountingFormat(GrossSalesToNoneSC), [0, 0, 0, 0, 0, 36]);

                                        //WindowsPrinter('SCD-Exempt Amount: ' + AccountingFormat(Popup.Discount.SCDiscountExempt), [0, 0, 0, 0, 0, 36]);

                                        //WindowsPrinter('Total Bill: ' + AccountingFormat(CDec2(Popup.Discount.SCDiscountExempt) + CDec2(seniorCitizenNetAmount) + CDec2(Popup.Discount.GrossSalesToNoneSC)), [0, 0, 0, 0, 0, 36]);

                                        ////---list of Senior Citizen
                                        //intIndex = 0;

                                        //for (var i = 0; i <= intSalesPayCustomerNameCntr; i++) {

                                        //    if (sessionStorage["SalesPayCustomerName-" + i] != undefined) {

                                        //        arSalesPayCustomerName = new Array();
                                        //        arSalesPayCustomerName = JSON.parse(sessionStorage["SalesPayCustomerName-" + i]);

                                        //        if (arSalesPayCustomerName[intSalesPayCustomerNameState] != "Remove") {

                                        //            WindowsPrinter((intIndex + 1) + '. ' + arSalesPayCustomerName[3] + ' ' + arSalesPayCustomerName[5], [0, 0, 0, 0, 0, 36]);

                                        //        } //end if not remove
                                        //    } //end if session storage
                                        //} //end for

                                        //WindowsPrinter('Senior Citizen/PWD TIN: ', [1, 22, 0, 0, 0, 36]);
                                        ////strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Senior Citizen/PWD TIN: ', 12, "Emilio Aguinaldo", intFAFCTotalLenght, '', 0, 0);
                                        ////ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                        //WindowsPrinter('OSCA ID No./PWD ID No.: ', [1, 22, 0, 0, 0, 36]);
                                        ////strPrint = FormatTextToPrint(intFAFCTotalLenght, 'OSCA ID No./PWD ID No.: ', 12, "8700-324", intFAFCTotalLenght, '', 0, 0);
                                        ////ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                        //WindowsPrinter('Signature', [1, 22, 0, 0, 0, 36]);
                                        ////strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Signature: ', 12, "Heiroglophyics \n", intFAFCTotalLenght, '', 0, 0);
                                        ////ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    }

                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                }
                                //Get Footer Messages
                                strPrint = '';
                                //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Use for testing only. This is not an Official Receipt.', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'This Serves as Your Official Receipt.', intFAFCTotalLenght, '', 0, '', 0, 1);
                                WindowsPrinter(vDataInvHeader.Rows[0].ReceiptFooterMsg, [1, 0, 0, 1, 0, 36]);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataInvHeader.Rows[0].ReceiptFooterMsg, intFAFCTotalLenght, '', 0, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                if (vDataMsg.Rows.length > 0) {
                                    if (CBool(vDataHeader.Rows[0].IsAuthorizedForPOS) || CBool(vDataHeader.Rows[0].ShowCompanyDetailsInHeader)) {
                                        for (var p = 0; p <= vDataMsg.Rows.length - 1; p++) {
                                            WindowsPrinter($.trim((vDataMsg.Rows[p].MessageFooter || '')), [1, 0, 0, 1, 0, 36]);
                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataMsg.Rows[p].MessageFooter || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                            //strPrint = strPrint + strTemp;
                                        };

                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                        WindowsPrinter('This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', [1, 0, 0, 1, 0, 36]);
                                        //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                    }
                                    //breakdown

                                    /*//Print Barcode
                                    strPrint = 'SI|' + vDataInvHeader.Rows[0].OrganizationID + '|' + vDataInvHeader.Rows[0].OrganizationBranchID + '|' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0)
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);

                                    strPrint = SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 1, 0);*/

                                    //ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                    WindowsPrinter("address", [0, 0, 0, 0, 0, 0], strPrinterIP, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);

                                    if (strPrinterName.length == 0) {
                                        WindowsPrinter("printerName", [0, 0, 0, 0, 0, 0], strPrinterName, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                    }

                                    WindowsPrinter("startPrint", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                    //PrintText(strPrinterIP);
                                    ClearCommandBuffer();

                                    //Print SRV
                                    //PrintSalesPaymentSRVReceipt(0, pInvoiceID);

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
                        }
                    }
                }
            }
        }
        else {
            MessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                "Printer",
                [
                    {
                        Text: 'Ok',
                        Click: function () {

                            $.mobile.changePage('');

                            if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')
                                window.pageHeader.ShowPaymentsPage(false);

                        }
                    }
                ]
        );
        }

    }

    function WindowsPrinter(cols1, args1, cols2, args2, cols3, args3) {

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
            try {
                Android.WindowsPrinting(
                                        {
                                            "data": [
                                                      { "startPrint": "startPrint" }
                                            ]
                                        }
                );
            } catch (e) {
                Android.AddLog("A problem occured while printing receipt using WPS method.")
            }
        } else if (cols1 == "address") {
            Android.AddLog("WPS IP address.: " + cols2);

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
})