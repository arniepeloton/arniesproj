define([
    '/Scripts/sources/ERP/OrderProcessing/product/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/OrderProcessing/product/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                // ERP.OrderProcessing.product.controller = ERP.OrderProcessing.product.controller(); // singleton
                // mHtml.trigger('create');
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.OrderProcessing.product.controller', null, function () {

        var mOut = {};
        var mElement;
        var mDivUnit;
        var mTransType;
        var mProductNo;
        var mProductName;
        var mInventoryBalance; // ERP.InventoryBalance.controller();
        var mParentController;
        var popupid = 'popOrderInput' + NewID();
        var isValidTradeDisc = false, selectedItemTR;
        var mFromSalesOrder;

        //$('body').append($(mHtml));
        //mElement = $('[idx="popOrderInput"]').attr('id', popupid); // popOrderInput only

        mElement = $(mHtml);
        mElement.attr('id', 'popOrderInput' + NewID());
        $('body').append(mElement);

        //mElement;
        var mElementTradeDiscount = $('#popOrderTradeDiscount');
        
        mOut.element = mElement;
        mOut.Model = null;
        
        var mTable = mElement.find('[name=tableProduct]');
        var mTR = mTable.find('tbody tr:first');

        //mOut.element.trigger('create');        

        mOut.popup = function(vParams) {
            mInventoryBalance = vParams.InventoryBalance;
            mParentController = vParams.ParentController;
            mProductNo = vParams.ProductNo;
            mFromSalesOrder = vParams.FromSalesOrder;
            mElement.find('.FreeTypePrice').hide();
            mOut.Model.Data.ProductID = vParams.ProductID;
            mOut.Model.Data.CustomerID = vParams.CustomerID;
            mTransType = vParams.TransType;
            if ((mTransType == 'PurchaseOrder') 
                || (mTransType == 'AccountsPayable')) {

                mElement.find('.PriceLevel').hide();
                mElement.find('.VendorPrice').show();
                if (mTransType == 'AccountsPayable') {
                    mElement.find('th[name=thOrderQty]').text('Qty to be Paid');
                    mElement.find('th.FreeTypePrice').text('Unit Price to be Paid');
                } else {
                    mElement.find('th.FreeTypePrice').text('PO Price');
                }
            } else {
                mElement.find('.PriceLevel').show();
                mElement.find('.VendorPrice').hide();
                mElement.find('th.FreeTypePrice').text('Net Price per Selling Unit');
            }
            
            switch (mTransType) {
                case 'Quote':
                    proj.CacMode = 'AR';
                    break;

                case 'SalesOrder':
                    proj.CacMode = 'AR';
                    break;
                
                case 'Invoice':
                    proj.CacMode = 'AR';
                    break;
                
                case 'PurchaseOrder':
                    proj.CacMode = 'AP';
                    break;

                case 'AccountsPayable':
                    proj.CacMode = 'AP';
                    break;

            }

            mProductName = vParams.ProductName;
            mElement.find('[name=title]').text(mProductName);
            mElement.find('[name=spanProductNumber]').text(vParams.ProductNo);
            Clear();
            
            mOut.Model.GetData({
                Success: function(vData) {
                    vParams.Success();
                    DisplayData(vData);
                    HandleFreeTypePriceElements();
                    try {
                        mElement.show().popup('open');
                    } catch (ex) {
                        // just ignore the error
                    }
                },
                Fail: function(vMessage) {
                    NewMessageBox(vMessage);
                }
            });
            CreatePriceHistoryDataTable();
        };        
        
        function HandleFreeTypePriceElements() {
            var pModuleName;
            if (mTransType == 'Quote') {
                pModuleName = 'Allow to Free Type Price in Sales Quotation (EAS)';
            } else if (mTransType == 'SalesOrder') {
                pModuleName = 'Allow to Free Type Price in Sales Order (EAS)';
            } else if (mTransType == 'Invoice') {
                pModuleName = 'Allow to Free Type Price in Sales Invoice (EAS)';
            } else if (mTransType == 'PurchaseOrder') {
                pModuleName = 'Allow to Free Type Price in Purchasing (EAS)';
            } else if (mTransType == 'AccountsPayable') {
                pModuleName = 'Allow to Free Type Price in Purchasing (EAS)';
            } else
                return;
            GetSecurityCurrentUserAcccess({
                ModuleName: pModuleName,
                Callback: function(vAllowed) {
                    if (vAllowed)
                        mElement.find('.FreeTypePrice').show();
                }
            });
        }

        function Clear() {
            
            if (mDivUnit == null)
                mDivUnit = $(mElement.find('[name=divUnit]:first')).clone();
            mElement.find('[name=divUnit]').remove();
            mTable.find('tbody tr').remove();
        }

        function DisplayData(vData) {
            var pTRs = [];

            if (vData.UnitMeasures == null) {
                mElement.show().popup('close');
                NewMessageBox('The product, ' + mProductName + ', has no price or unit.', 'No price or unit');
            }

            for (var p = 0; p <= vData.UnitMeasures.Rows.length - 1; p++) {
                var pDR = vData.UnitMeasures.Rows[p];

                if (p == 0)
                    mElement.find('[name=BaseUnit]').text(pDR.BaseUnit);
                AddRow(pDR, null, vData);
                if (proj.CacMode != 'AP') {
                    var pPriceBreaks = FilterPriceBreaks(vData.PriceBreaks.Rows, pDR.UnitMeasure);
                    for (var p2 = 0; p2 <= pPriceBreaks.length - 1; p2++) {
                        AddRow(pDR, pPriceBreaks[p2], vData);
                    }
                }
            }
            SortRows();
            for (var p3 = 0; p3 <= pTRs.length - 1; p3++) {
                mTable.append(pTRs[p3]);
            }
            OptimizeQuantities();

            if (mTransType == 'Invoice') {
                mElement.find('[name=thServeredQty]').show();
                mElement.find('[name=ServedQty]').show();
                mElement.find('[name=txtQty]').hide();
                mElement.find('[name=lblOrderedQty]').show();
            } else {
                mElement.find('[name=thServeredQty]').hide();
                mElement.find('[name=ServedQty]').hide();
                mElement.find('[name=txtQty]').show();
                mElement.find('[name=lblOrderedQty]').hide();
            }
            // if (mTransType == 'PurchaseOrder') {
            if (true) {
                mElement.find('[name=NetPricePerSellingUnit] input').show();
                mElement.find('[name=NetPricePerSellingUnit] span').hide();
            } else {
                mElement.find('[name=NetPricePerSellingUnit] input').hide();
                mElement.find('[name=NetPricePerSellingUnit] span').show();
            }

            function AddRow(vDR, vDrPriceBreak, vData) {
                var pTR = mTR.clone();
                var pSellingUnit = vDR.UnitMeasure;
                var pDR = vDR;
                var pUnitQty = CDec2(vDR.Quantity);
                var pTxtOrdered, pTxtServed;

                pTR.data('data', pDR);
                if (vDrPriceBreak) {
                    pSellingUnit = vDrPriceBreak.Qty + '-' + pSellingUnit;  
                    pUnitQty *= CDec2(vDrPriceBreak.Qty);
                }
                pTR.find('[name=SellingUnit]').text(pSellingUnit);
                pTR.find('[name=QtyInBaseUnit]').text('(' + pUnitQty + ' ' + vDR.BaseUnit + ')');
                pTR.find('[name=SRP]').text(AccountingFormat(vDR.SRP * pUnitQty));
                pTR.data('UnitQty', pUnitQty);
                var pCbPrice = pTR.find('[name=PriceLevel] select') ;
                var pDrQty = DisplayPrices(
                    pCbPrice,
                    vDR.UnitMeasure,
                    vData.data.Rows,
                    vDrPriceBreak,
                    vDR.Quantity
                );
                pDrQty = $.extend({}, pDrQty);
                if ((mTransType == 'PurchaseOrder') 
                    || (mTransType == 'AccountsPayable'))
                    if (CDec2(pDrQty.OrderedQuantity) == 0) 
                        pDrQty.OrderedQuantity = pDrQty.ServedQuantity;
                pTxtOrdered = pTR.find('[name=txtQty]').val(pDrQty.OrderedQuantity);

                pTR.find('[name=lblOrderedQty]').text(pDrQty.OrderedQuantity);
                pTxtServed = pTR.find('[name=txtServedQty]').val(pDrQty.ServedQuantity);
                if (mTransType == 'Invoice') {
                    pTxtServed.on('textchange', txtQty_Change);
                    if (mFromSalesOrder) {
                        //pTxtServed.attr('disabled', 'disabled');
                        pTxtServed.prop('disabled', true);
                        pTxtServed.click(function() {
                            // click not triggered
                            NewMessageBox('You may not modify served quantities of invoices that came from sales order.');
                        });
                    }
                } else
                    pTxtOrdered.on('textchange', txtQty_Change);
                
                pCbPrice.find('option[value=' + vDR.PriceLevelID + ']').attr('selected', true);
                pCbPrice.on('change', CbPrice_Change);
                pCbPrice.trigger('change');

                if (CStr2(pDrQty.TradeDiscountLess) != '') {
                    pTR.data('data').TradeDiscountLess = pDrQty.TradeDiscountLess;
                    pTR.data('data').NetDiscountFactor = pDrQty.NetDiscountFactor;
                    pTR.data('data').NetDiscountRate = pDrQty.NetDiscountRate;
                    pTR.find('[name=DiscountRate] a').text(pDrQty.TradeDiscountLess + ' (' + pDrQty.NetDiscountRate + '% NDR)')
                }

                pTR.find('[name=DiscountRate] a').click(function () {
                    selectedItemTR = $(this).parents().closest('tr');
                    $('#' + popupid).popup('close');
                    $('#popOrderTradeDiscount').popup('open');
                    $('#txtOrderTradeDiscountLess').val('').focus();
                    $('#lblOrderTradeDiscountFactor').text('');
                    $('#lblOrderTradeDiscountRate').text('');

                    if (CStr2(selectedItemTR.data('data').TradeDiscountLess) != '') {
                        $('#txtOrderTradeDiscountLess').val(selectedItemTR.data('data').TradeDiscountLess);
                        TradeDiscount(selectedItemTR.data('data').TradeDiscountLess);
                    }
                    AfterPopupClose(function () {
                        $('#' + popupid).popup('open');
                    })
                })
                pTR.find('[name=NetPricePerSellingUnit] input').change(function () {
                    var tr = $(this).parents().closest('tr');
                    var data = tr.data('data');

                    tr.find('[name="SubTotal"]').text(CDec2(tr.find('[name="txtQty"]').val()) * CDec2($(this).val()));
                    tr.find('[name="DiscountRate"] a').text('Set Trade Discount');
                    data.TradeDiscountLess = '';
                    data.NetDiscountFactor = 0;
                    data.NetDiscountRate = 0;
                });
                if (vData.VendorPriceInfo == null)
                    pTR.find('[name=VendorPrice]').text('N / A');
                else {
                    vData.VendorPriceInfo.Price = CDec2(vData.VendorPriceInfo.Price);
                    var pText = HtmlEncode(AccountingFormat(vData.VendorPriceInfo.Price * pUnitQty)) + '<br />';
                    pText += HtmlEncode(FormatDate(vData.VendorPriceInfo.LastUpdated, {
                            DontDisplay12Am: true,
                            Format: 'MMM dd, yyyy hh:mm a',
                            FormatNoTime: 'MMM dd, yyyy'
                        }));
                    pTR.find('[name=VendorPrice]').html(pText);
                }
                    
                pTRs.push(pTR);
            }

            //if ((mTransType == 'Invoice') 
            //    && mFromSalesOrder)
            //    NewMessageBox('You may not modify served quantities of invoices that came from sales order.');

            function DisplayPrices(vComboBox, vUnit, vPrices, vDrPriceBreak) { 
                var pUniquePriceLevels = [];
                var pPriceBreakQty;
                var pDrFilteredCart;

                for (var p = 0; p <= vPrices.length - 1; p++) {
                    var pDR = vPrices[p];
                    if (pDR.UnitMeasure != vUnit)
                        continue;
                    if (pUniquePriceLevels.indexOf(pDR.PriceLevel.toLowerCase()) >= 0)
                        continue;

                    var pOption = new Option(pDR.PriceLevel, pDR.PriceLevelID);
                    pPriceBreakQty = 1;
                    if (vDrPriceBreak) {
                        $(pOption).data('PriceBreak', vDrPriceBreak);
                        pDR = JSON.parse(JSON.stringify(pDR)); // clone vDR
                        pDR.DiscountedPrice = CDec2(pDR.DiscountedPrice) * (1 - CDec2(vDrPriceBreak.Discount) / 100);
                        pDR.DiscountedPricePerUnit = pDR.DiscountedPrice / CDec2(pDR.Quantity);
                        pPriceBreakQty = CDec2(vDrPriceBreak.Qty);
                        pDR.DiscountedPrice *= pPriceBreakQty;
                    }
                    if (pDrFilteredCart == null) {
                        pDrFilteredCart = FilterCart(pDR.PriceLevelID, pDR.Quantity, pDR.DiscountedPrice / pPriceBreakQty, pPriceBreakQty);
                        if (pDrFilteredCart != null)
                            pDR.DiscountedPrice = pDrFilteredCart.UnitPrice;
                    }

                    $(pOption).data('data', pDR);
                    vComboBox.append(pOption);
                    pUniquePriceLevels.push(pDR.PriceLevel.toLowerCase());
                }

                return pDrFilteredCart;
            }

            function FilterCart(vPriceLevelID, vUnitQty, vUnitPrice, vPriceBreakQty) {
                if (vData.Cart == null)
                    return;
                for (var p = 0; p <= vData.Cart.Rows.length - 1; p++) {
                    var pDR = vData.Cart.Rows[p];
                    if ((ReadyGuid(pDR.PriceLevelID) == ReadyGuid(vPriceLevelID))
                        && !pDR.Used
                        && (CDec2(pDR.ActualQuantity) == CDec2(vUnitQty))) {
                        // && (Math.abs(CDec2(pDR.UnitPrice) - CDec2(vUnitPrice)) < .01)) { // floating number does not exactly match. so, use tolerance
                        pDR.Used = true;

                        return {
                            UnitPrice: pDR.UnitPrice,
                            OrderedQuantity: pDR.OrderedQuantity / vPriceBreakQty,
                            ServedQuantity: pDR.Quantity / vPriceBreakQty,
                            TradeDiscountLess: CStr2(pDR.TradeDiscountLess),
                            NetDiscountFactor: CDec2(pDR.NetDiscountFactor),
                            NetDiscountRate: CDec2(pDR.NetDiscountRate),
                        };
                    }
                }
            }

            function SortRows() {
                pTRs.sort(function(vA, vB) {
                    if (CDec2(vA.data('UnitQty')) < CDec2(vB.data('UnitQty')))
                        return -1;
                    if (CDec2(vA.data('UnitQty')) > CDec2(vB.data('UnitQty')))
                        return 1;
                    return 0;
                });
            }

        } // END function DisplayData(vData) {

        function FilterPriceBreaks(vPriceBreaks, vUnitMeasure) {
            var pOut = [];
            for (var p = 0; p <= vPriceBreaks.length - 1; p++) {
                var pPriceBreak = vPriceBreaks[p];
                if (pPriceBreak.UnitMeasure == vUnitMeasure) {
                    pOut.push(pPriceBreak);
                }
            }

            return pOut;
        }

        function CbPrice_Change(e) {
            var pTR = $(this).closest('tr[name=data]');
            var pDrPrice = $($(this).find('option:selected')).data('data');
            pTR.find('[name=NetPricePerBaseUnit]').text(AccountingFormat(pDrPrice.DiscountedPricePerUnit));
            var p = AccountingFormat(pDrPrice.DiscountedPrice);
            pTR.find('[name=NetPricePerSellingUnit] span').text(p);
            pTR.find('[name=NetPricePerSellingUnit] input').val(AccountingFormat(p, null, ''));
            var pSubTotal = CDec2(pDrPrice.DiscountedPrice) * CDec2(pTR.find('[name=OptimizedOrderQtyForDiscounts]').val());
            pTR.find('[name=SubTotal]').text(AccountingFormat(pSubTotal));
            DisplayTotal();
        }

        function txtQty_Change(e) {
            //var pTR = $(this).closest('tr[name=data]');
            //var pDrPrice = pTR.find('[name=PriceLevel] option:selected').data('data');
            //var pSubTotal = CDec2(pDrPrice.DiscountedPrice) * CDec2($(this).val());
            //pTR.find('[name=SubTotal]').text(AccountingFormat(pSubTotal));
            OptimizeQuantities();
        }

        function OptimizeQuantities() {
            var pTRs = mTable.find('tr[name=data]');
            var pTotalQty = 0; // in base unit
            var pName;
            var pBasePrice = 0;

            for (var p = 0; p <= pTRs.length - 1; p++) {
                var pTR = $(pTRs[p]);
                var pDR = pTR.data('data');
                var pDrUnitQty = pTR.data('UnitQty');
                if (pDrUnitQty == 0)
                    pDrUnitQty = 1;
                if (pDR == null)
                    continue;
                if (mTransType == 'Invoice')
                    pName= 'txtServedQty';
                else
                    pName = 'txtQty';
                pTotalQty += CDec2(pTR.find('[name=' + pName + ']').val()) * pDrUnitQty;
                if (pBasePrice == 0) {
                    var pTxtPrice = pTR.find('[name=NetPricePerSellingUnit] input');
                    var pPrice = CDec2(pTxtPrice.val());
                    if (pPrice > 0)
                        pBasePrice = pPrice / pDrUnitQty;
                }
            }

            var pRemaining = pTotalQty;
            for (var p = pTRs.length - 1; p >= 0; p--) {
                var pTR = $(pTRs[p]);
                var pDR = pTR.data('data');
                var pDrUnitQty = pTR.data('UnitQty');
                pDrUnitQty = CDec2(pDrUnitQty);
                if (pDrUnitQty == 0)
                    pDrUnitQty = 1;
                if (pDR == null)
                    continue;
                
                var pQty = Math.floor(pRemaining / pDrUnitQty);
                if (p == 0)
                    pQty = pRemaining;
                pRemaining -= pQty * pDrUnitQty;
                pTR.find('[name=OptimizedOrderQtyForDiscounts]').text(AccountingFormat(pQty, 0));
                var pDrPrice = pTR.find('[name=PriceLevel] option:selected').data('data');
                if (CDec2(pDrPrice.DiscountedPrice) == 0) {
                    pDrPrice.DiscountedPrice = pBasePrice * pDrUnitQty;
                }
                var pTxt = pTR.find('[name=NetPricePerSellingUnit] input');
                if ((CDec2(pDrPrice.DiscountedPrice) > 0)
                    && (CDec2(pTxt.val()) == 0))
                    pTxt.val(pDrPrice.DiscountedPrice);

                var pSubTotal = CDec2(pDrPrice.DiscountedPrice) * pQty;
                pTR.find('[name=SubTotal]').text(AccountingFormat(pSubTotal));
                
            }
            DisplayTotal();
        }

        function GetTotal() {
            var pTRs = mTable.find('tr[name=data]');
            var pTotal = 0; // in base unit

            for (var p = 0; p <= pTRs.length - 1; p++) {
                var pTR = $(pTRs[p]);
                pTotal += CDec2(pTR.find('[name=SubTotal]').text());
            }

            return pTotal;
        }

        function DisplayTotal() {
            var pTotal = GetTotal();
            mElement.find('[name=ProductTotal]').text(AccountingFormat(pTotal));
        }

        mElement.find('[name=butOk]').click(function() {
            var pData = {
                Stocks: [],
                TransType: mParentController.TransType
            };
            var pDrPrice;
            var pDrPriceBreak;

            var pDivUnits = mTable.find('tr[name=data]');
            for (var p = 0; p <= pDivUnits.length - 1; p++) {
                var pStock = {};
                var pDivUnit = $(pDivUnits[p]);
                var pDivUnitData = pDivUnit.data('data');
                pStock.Qty = CDec2(pDivUnit.find('[name=OptimizedOrderQtyForDiscounts]').text());
                if (pStock.Qty <= 0)
                    continue;
                var pCB = pDivUnit.find('[name=PriceLevel] select');
                var pOption =  $(pCB.find(':selected'));
                pDrPrice = pOption.data('data');
                pDrPriceBreak = pOption.data('PriceBreak');
                pStock.UnitQty = pDrPrice.Quantity;
                // if (mTransType == 'PurchaseOrder')
                if (true) 
                    pStock.EditPrice = pDivUnit.find('[name=NetPricePerSellingUnit] input').val();
                else
                    pStock.EditPrice = pDrPrice.DiscountedPrice;
                if (pDrPriceBreak) {
                    pStock.EditPrice /= CDec2(pDrPriceBreak.Qty);
                    pStock.Qty *= CDec2(pDrPriceBreak.Qty);
                }
                pStock.PriceLevelID = pCB.val();

                if (CStr2(pDivUnitData.TradeDiscountLess) != '') {
                    pStock.TradeDiscountLess = pDivUnitData.TradeDiscountLess;
                    pStock.NetDiscountFactor = pDivUnitData.NetDiscountFactor;
                    pStock.NetDiscountRate = pDivUnitData.NetDiscountRate;
                }                

                pData.Stocks.push(pStock);
            }

            mOut.Model.Save({
                data: pData,
                Success: function(vData) {
                    mParentController.AfterProductSave();
                    mElement.show().popup('close');
                }, 
                Fail: function(vMessage) {
                    NewMessageBox(vMessage, '', [
                        {
                            Text: 'Ok',
                            Click: function() {
                                setTimeout(function() {
                                    mElement.show().popup('open');
                                }, 500);
                            }
                        }
                    ]);
                }
            });
        });

        function GetSecurityCurrentUserAcccess(vParams) {
            vParams = $.extend({
                ModuleName: 'string',
                Callback: function(vAllowed) {
                }
            }, vParams);
            var pData = {
                OrgID: sessionStorage["OrgID"],
                UserID: sessionStorage.PeopleID,
                ModuleName: vParams.ModuleName
            };
            AjaxSend({
                url: "SecurityRole/CheckSecurityCurrentUserAcccess",
                data: pData,
                success: function (vData) {
                    if (vData.SecurityRoleData.Rows.length > 0) 
                        vParams.Callback(true);
                    else 
                        vParams.Callback(false);
                }
            });
        }

        mOut.Init = function() {
            mOut.Model = new ERP.OrderProcessing.product.model();
        };

        var mTablePriceHistory = mElement.find('[name=tablePriceHistory]');
        var mPriceHistoryDataTable;

        function ClearPriceHistory() {
            if ($.fn.DataTable.isDataTable(mTablePriceHistory)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTablePriceHistory.empty();
                mPriceHistoryDataTable.destroy();
            }
        }

        mElement.find('[name=butInventory]').click(function() {
            mInventoryBalance.Show({
                ProductID: mOut.Model.Data.ProductID,
                ProductNo: mProductNo,
                ProductName: mProductName,
                OrderController: mParentController
            });
        });

        function CreatePriceHistoryDataTable() {
            ClearPriceHistory();
            mOut.Model.GetPriceHistory({
                CustomerID: mOut.Model.Data.CustomerID,
                ProductID: mOut.Model.Data.ProductID,
                Success: function(vData) {
                    mPriceHistoryDataTable = mTablePriceHistory.DataTable({
                        data: vData.Result.Rows,
                        columns: [
                            {data: 'InvoiceNum'},
                            {
                                data: function(vData) {
                                    return $.format.date(new Date(vData.InvoiceDate), 'MMM dd, yyyy');
                                }
                            },
                            {
                                data: function(vData) {
                                    return $.format.number(CDec2(vData.Quantity), '#,###.##');
                                }
                            },
                            {data: 'PriceLevelName'},
                            {
                                data: function(vData) {
                                    return AccountingFormat(vData.UnitPrice);
                                }
                            },
                            {
                                data: function(vData) {
                                    return AccountingFormat(vData.SubTotal);
                                }
                            }
                        ]
                    });
                },
                Fail: function(vError) {
                    NewMessageBox(vError);
                }
            });
        } // END CreatePriceHistoryDataTable

        mElementTradeDiscount.find('#txtOrderTradeDiscountLess').keyup(function () {
            if (CDec2(selectedItemTR.data('data').SRP) == 0) {
                WebToast('Can\'t encode trade discount since SRP is 0', 'top');
                return;
            }
            TradeDiscount($(this).val());
        })

        mElementTradeDiscount.find('#btnOkOrderTradeDiscount').click(function () {
            if ($('#txtOrderTradeDiscountLess').val() == '' || !isValidTradeDisc) {
                WebToast('Please enter valid trade discount', 'top');
                return;
            }

            var data = selectedItemTR.data('data');
            var td = TradeDiscount($('#txtOrderTradeDiscountLess').val());
            
            data.TradeDiscountLess = $('#txtOrderTradeDiscountLess').val();
            data.NetDiscountFactor = td.NetFactor;
            data.NetDiscountRate = td.NetRate;

            selectedItemTR.find('[name="DiscountRate"] a').text(data.TradeDiscountLess + ' (' + td.NetRate + '% NDR)');
            selectedItemTR.find('[name="NetPricePerSellingUnit"] input').val((CDec2(td.Factor / 100) * CDec2(data.SRP)).toFixed(2));
            selectedItemTR.find('[name="SubTotal"]').text(CDec2(selectedItemTR.find('[name="txtQty"]').val()) * CDec2(selectedItemTR.find('[name="NetPricePerSellingUnit"] input').val()))
            $('#popOrderTradeDiscount').popup('close');
        })

        function TradeDiscount(val) {
            var less = val.split(',');
            var factor = 0, rate = 0;

            for (var i = 0; i < less.length; i++) {
                if (IsNumeric(less[i])) {
                    if (factor == 0) {
                        factor = 1 - (CDec2(less[i]) / 100);
                    } else {
                        factor = factor * (1 - (CDec2(less[i]) / 100));
                    }
                    isValidTradeDisc = true;
                } else {
                    isValidTradeDisc = false;
                    factor = 0;
                    break;
                }
            }
            var realfactor = factor * 100;
            var realrate = (1 - factor) * 100;
            var netfactor = CDec2(realfactor.toFixed(2));
            var netrate = CDec2(realrate.toFixed(2));
            $('#lblOrderTradeDiscountFactor').text(netfactor + '%');
            $('#lblOrderTradeDiscountRate').text(netrate + '%');

            return {
                Factor: realfactor,
                Rate: realrate,
                NetFactor: netfactor,
                NetRate: netrate
            }
        }

        mOut.Init();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});