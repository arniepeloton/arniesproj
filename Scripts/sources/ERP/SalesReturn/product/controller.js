define([
    '/Scripts/sources/ERP/SalesReturn/product/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/SalesReturn/product/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.SalesReturn.product.controller', null, function () {

        var mOut = {};
        var mElement;
        var mDivUnit;
        var mTransType;
        var mProductName;

        mElement = $(mHtml);
        mElement.attr('id', 'popOrderInput' + NewID());
        $('body').append(mElement);
        mOut.element = mElement;
        mOut.Model = null;
        mOut.AfterSave = function(){};
        var mTable = mElement.find('[name=tableProduct]');
        var mTR = mTable.find('tbody tr:first');
        var mCbReturnReason = mElement.find('[name=cbReturnReason]');

        mOut.popup = function(vParams) {
            mElement.find('.FreeTypePrice').hide();
            mOut.Model.Data.ProductID = vParams.ProductID;
            mOut.Model.Data.CustomerID = vParams.CustomerID;
            mOut.Model.Data.InvoiceID = vParams.InvoiceID;
            mTransType = vParams.TransType;
            if (mTransType == 'PurchaseOrder')
                mElement.find('.PriceLevel').hide();
            else
                mElement.find('.PriceLevel').show();
                
            mProductName = vParams.ProductName;
            mElement.find('[name=title]').text(mProductName);
            mElement.find('[name=spanProductNumber]').text(vParams.ProductNo);
            Clear();
            mElement.modal('show')//.show().popup().popup('open');
            mOut.Model.GetData({
                Success: function(vData) {
                    vParams.Success();
                    DisplayData(vData);
                    HandleFreeTypePriceElements();
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

        function PopulateReasons() {
            var pCbItem;

            mCbReturnReason.empty();
            pCbItem = new Option('Return Reason', 0);
            mCbReturnReason.append(pCbItem);
            for (var p = 0; p <= mOut.Model.ReturnsReasons.length - 1; p++) {
                var pDR = mOut.Model.ReturnsReasons[p];
                pCbItem = new Option(pDR);
                mCbReturnReason.append(pCbItem);
            }

        }

        function DisplayData(vData) {
            var pTRs = [];
            var pCount = CInt2(GetStringObject('Units.Rows.length', vData));
            var pBaseUnit;

            if (mOut.Model.ReturnsReasons != null) 
                PopulateReasons();

            if (pCount == 0) {
                mElement.show().popup('close');
                NewMessageBox('The product, ' + mProductName + ', has no price.', 'No price');
            }

            mCbReturnReason.val(null).attr('selected');
            for (var p = 0; p <= pCount - 1; p++) {
                var pDR = vData.Units.Rows[p];
                if (p == 0) {
                    pBaseUnit = pDR.UnitMeasure;
                    mElement.find('[name=BaseUnit]').text(pBaseUnit);
                }
                AddRow(pDR, pBaseUnit);
            }
            SortRows();
            for (var p3 = 0; p3 <= pTRs.length - 1; p3++) {
                mTable.append(pTRs[p3]);
            }

            function AddRow(vDR, vBaseUnit) {
                var pTR = mTR.clone();
                var pUnitQty = CDec2(vDR.ActualQuantity);

                pTR.data('data', vDR);
                pTR.find('[name=SellingUnit]').text(vDR.UnitMeasure);
                pTR.find('[name=QtyInBaseUnit]').text('(' + pUnitQty + ' ' + vBaseUnit + ')');
                pTR.find('[name=Quantity]').text($.format.number(CDec2(vDR.Quantity), '#,###.##'));
                pTR.find('[name=QtyReturnedOthers]').text($.format.number(CDec2(vDR.QtyReturnedOthers), '#,###.##'));
                pTR.find('[name=txtQtyReturnedOwn]').val($.format.number(CDec2(vDR.QtyReturnedOwn), '#,###.##'));
                pTR.find('[name=txtQtyReturnedOwnBad]').val($.format.number(CDec2(vDR.QtyReturnedOwnBad), '#,###.##'));
                pTR.find('[name=UnitPrice]').text(AccountingFormat(vDR.DiscountedUnitPrice));
                pTRs.push(pTR);
                if (CStr2(vDR.ReturnReason) != '')
                    mCbReturnReason.val(vDR.ReturnReason).attr('selected');
            }

            //function DisplayPrices(vComboBox, vUnit, vPrices, vDrPriceBreak) { 
            //    var pUniquePriceLevels = [];
            //    var pPriceBreakQty;
            //    var pDrFilteredCart;

            //    for (var p = 0; p <= vPrices.length - 1; p++) {
            //        var pDR = vPrices[p];
            //        if (pDR.UnitMeasure != vUnit)
            //            continue;
            //        if (pUniquePriceLevels.indexOf(pDR.PriceLevel.toLowerCase()) >= 0)
            //            continue;

            //        var pOption = new Option(pDR.PriceLevel, pDR.PriceLevelID);
            //        pPriceBreakQty = 1;
            //        if (vDrPriceBreak) {
            //            $(pOption).data('PriceBreak', vDrPriceBreak);
            //            pDR = JSON.parse(JSON.stringify(pDR)); // clone vDR
            //            pDR.DiscountedPrice = CDec2(pDR.DiscountedPrice) * (1 - CDec2(vDrPriceBreak.Discount) / 100);
            //            pDR.DiscountedPricePerUnit = pDR.DiscountedPrice / CDec2(pDR.Quantity);
            //            pPriceBreakQty = CDec2(vDrPriceBreak.Qty);
            //            pDR.DiscountedPrice *= pPriceBreakQty;
            //        }
            //        if (pDrFilteredCart == null) {
            //            pDrFilteredCart = FilterCart(pDR.PriceLevelID, pDR.Quantity, pDR.DiscountedPrice / pPriceBreakQty, pPriceBreakQty);
            //            if (pDrFilteredCart != null)
            //                pDR.DiscountedPrice = pDrFilteredCart.UnitPrice;
            //        }

            //        $(pOption).data('data', pDR);
            //        vComboBox.append(pOption);
            //        pUniquePriceLevels.push(pDR.PriceLevel.toLowerCase());
            //    }

            //    return pDrFilteredCart;
            //}

            //function FilterCart(vPriceLevelID, vUnitQty, vUnitPrice, vPriceBreakQty) {
            //    if (vData.Cart == null)
            //        return;
            //    for (var p = 0; p <= vData.Cart.Rows.length - 1; p++) {
            //        var pDR = vData.Cart.Rows[p];
            //        if ((ReadyGuid(pDR.PriceLevelID) == ReadyGuid(vPriceLevelID))
            //            && !pDR.Used
            //            && (CDec2(pDR.ActualQuantity) == CDec2(vUnitQty))) {
            //            // && (Math.abs(CDec2(pDR.UnitPrice) - CDec2(vUnitPrice)) < .01)) { // floating number does not exactly match. so, use tolerance
            //            pDR.Used = true;

            //            return {
            //                UnitPrice: pDR.UnitPrice,
            //                OrderedQuantity: pDR.OrderedQuantity / vPriceBreakQty,
            //                ServedQuantity: pDR.Quantity / vPriceBreakQty
            //            };
            //        }
            //    }
            //}

            function SortRows() {
                pTRs.sort(function(vA, vB) {
                    if (CDec2(vA.data('ActualQuantity')) < CDec2(vB.data('ActualQuantity')))
                        return -1;
                    if (CDec2(vA.data('ActualQuantity')) > CDec2(vB.data('ActualQuantity')))
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
            if (Verify())
                Submit();
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
            mOut.Model = new ERP.SalesReturn.product.model();
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
                        ],
                        initComplete: function () {
                            $('[name="tablePriceHistory"]').removeClass('dataTable');
                        }
                    });
                },
                Fail: function(vError) {
                    NewMessageBox(vError);
                }
            });
        } // END CreatePriceHistoryDataTable

        function Verify() {
        
            var pResult = true;

            if (mCbReturnReason.val() == 0) {
                WebToast('Please select a return reason', 'top');
                return false;
            };

            mElement.find('[name=txtQtyReturnedOwn]').add(mElement.find('[name=txtQtyReturnedOwnBad]')).each(function (vItem, vIndex) {
                var pThis = $(this);
                var pVal = pThis.val();
                if (CStr2(pVal).trim().length > 0) {
                    if (!IsNumeric(pVal)) {
                        WebToast('Quantity must be a valid number. You entered ' + pVal, 'top');
                        pResult = false;
                        return false;
                    }
                    if (!IsInteger(pVal)) {
                        WebToast('Quantity must be a valid whole number. You entered ' + pVal, 'top');
                        pResult = false;
                        return false;
                    }; // if (!IsNumeric(pVal)) {

                    if (CDec2(pVal) < 0) {
                        WebToast('Quantity must be greater than or equal to zero. You entered ' + pVal, 'top');
                        pResult = false;
                        return false;
                    }

                  
                }; // if (CStr2(pVal).trim().length > 0) {
            }); // $('[name=txtQty]').each(function(vItem, vIndex) {

            return pResult;

        } // END function Verify() {

        function Submit() {

            var pData = {};
            var p = 0;

            pData.StockID = mOut.Model.Data.ProductID;
            pData.CustomerID = mOut.Model.Data.CustomerID;
            pData.InvoiceID = mOut.Model.Data.InvoiceID;
            pData.Stocks = [];
            //pData.MemoID = proj.SalesReturnMemoID;

            mElement.find('[name=txtQtyReturnedOwn]').each(function (vItem, vIndex) {
                var pThis = $(this);
                var pTR = pThis.closest('tr');
                var pDR = pTR.data('data');
                var pTxtBad = pTR.find('[name=txtQtyReturnedOwnBad]');
                if (IsNumeric(pThis.val())) {
                    pData.Stocks[p] = {
                        Qty: pThis.val(),
                        QtyBad: pTxtBad.val(),
                        UnitQty: pDR.ActualQuantity,
                        UnitMeasure: pDR.UnitMeasure,
                        UnitPrice: pDR.UnitPrice,
                        //NetDue: pThis.data('NetDue'),
                        //BracketNumber: pThis.data('BracketNumber'),
                        ReturnReason: mCbReturnReason.val()
                    };
                    p++;
                }; // if (!IsNumeric(pThis.val())) {
            }); // $('[name=txtQty]').each(function(vItem, vIndex) {

            AjaxSend({
                url: 'SalesReturns/Update',
                data: pData,
                success: function (vData) {
                    if ((vData != undefined) 
                        && (vData.Result == 'ok')) {
                        WebToast('The transactions was successfully saved.', 'top');
                        mElement.modal('hide');
                        $('#popupSalesReturn').modal('show');
                            mOut.AfterSave();
                    } else if (vData.Error != undefined)
                        NewMessageBox(vData.Error, null, [
                            {
                                Text: 'Ok',
                                Click: function() {
                                   mElement.modal('show');
                                }
                            }
                        ]); 
                    else 
                        NewMessageBox('An error occured while updating your returns', null, [
                            {
                                Text: 'Ok',
                                Click: function() {
                                   mElement.modal('show');
                                }
                            }
                        ]); 
                }
            });

        }; // Submit()

        mOut.Init();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});