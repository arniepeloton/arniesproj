define([
    '/Scripts/sources/ERP/OrderProcessing/product/model.js',
    '/Content/Ecommerce/js/lightbox-2.11.1.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtmlm, isLoad = false;
    var mOut = {};

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/OrderProcessing/product/view.html?v=' + assetversion,
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
        var mMaxPaymentTerm = 0;
        var mTargetOrgType;
        var mEditMode = false;
        var arrSalesRep = [];
        var mItemRequisition;
        var mAverageWeeklyDemand;
        var mUnitPriceSurchargePercent = 0;
        var mHidePrices;

        //$('body').append($(mHtml));
        //mElement = $('[idx="popOrderInput"]').attr('id', popupid); // popOrderInput only

        if (isLoad) return mOut;

        mElement = $($.parseHTML(mHtml));

        //if (proj.IsBootstrapDesign()){
        mElement.filter('.modal:first').attr('id', popupid);
        //mElement.filter('[idx="popOrderInput"]').find('#popOrderInputcontent > [name="divTab-bs"]').show();
        //mElement.filter('[idx="popOrderInput"]').find('#popOrderInputcontent > [name="divTab"]').remove();
        //mElement.filter('[idx="popOrderInput"]').find('#popOrderInputcontent .tab-pane').removeAttr('style custom-tab-role');
        //mElement.filter('[idx="popOrderInput"]').find('#popOrderInputcontent > *').appendTo(mElement.filter('.modal').find('#popOrderInputcontent-bs'));
        //mElement.filter('#popOrderTradeDiscount').find('#popOrderTradeDiscountcontent > *').appendTo(mElement.filter('#popOrderTradeDiscount-bs').find('#popOrderTradeDiscountcontent-bs'));
        //mElement = mElement.filter('.modal');
        //} else {
        //    mElement.filter('[data-role="popup"]:first').attr('id', popupid);
        //    mElement = mElement.filter('[data-role="popup"]:first');
        //}

        $('body').append(mElement);
        isLoad = true;

        //mElement;
        var mElementTradeDiscount = $('#popOrderTradeDiscount, #popOrderTradeDiscount-bs');

        mOut.element = mElement;
        mOut.Model = null;

        var mTable = mElement.find('[name=tableProduct]');
        var mTR = mTable.find('tbody tr:first');

        //mOut.element.trigger('create');        
        var mIncludeApprove = false;
        mOut.popup = function (vParams) {
            mEditMode = CBool(vParams.EditMode);
            mInventoryBalance = vParams.InventoryBalance;
            mParentController = vParams.ParentController;
            mProductNo = vParams.ProductNo;
            mFromSalesOrder = vParams.FromSalesOrder;
            mMaxPaymentTerm = vParams.MaxPaymentTerm;
            mTargetOrgType = vParams.TargetOrgType;
            mElement.find('.FreeTypePrice, #accUPIVendorPriceMatrix').hide();
            mOut.Model.Data.ProductID = vParams.ProductID;
            mOut.Model.Data.CustomerID = vParams.CustomerID;
            delete mOut.Model.Data.DeleteExistingProduct;
            if (vParams.hasOwnProperty('DeleteExistingProduct'))
                mOut.Model.Data.DeleteExistingProduct = vParams.DeleteExistingProduct;
            mOut.Model.Data.CustomerID = vParams.CustomerID;
            mIncludeApprove = CBool(vParams.IncludeApprove);
            mUnitPriceSurchargePercent = CDec2(vParams.UnitPriceSurchargePercent);
            if (mIncludeApprove)
                mElement.find('[name=butOk]').text('Save And Approve Profitability');
            else
                mElement.find('[name=butOk]').text('Save');
            if (vParams.ReadOnly)
                mElement.find('[name=butOk]').hide();
            else
                mElement.find('[name=butOk]').show();
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
                $('#accUPIVendorPriceMatrix').show();
            } else {
                mElement.find('.PriceLevel').show();
                mElement.find('.VendorPrice').hide();
                mElement.find('th.FreeTypePrice').text('Net Price per Selling Unit');
            }

            if (!CBool(vParams.IsUPIScanner)) {
                $('[name="divforUPIScanner"]').hide();
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
            CollapsedAccordion();

            try {
                if (proj.IsBootstrapDesign()) {
                    $('#' + popupid).modal('show');
                } else {
                    mElement.show().popup().popup('open');
                }
            } catch (ex) {
                // just ignore the error
            }

            mHidePrices = vParams.HidePrices;
            if (mHidePrices)
                mElement.find('.PriceClass').hide();
            else
                mElement.find('.PriceClass').show();

            Bs.Loading('show');
            mOut.Model.GetData({
                Success: function(vData) {
                    Bs.Loading('hide');
                    vParams.Success();
                    DisplayData(vData);
                    HandleFreeTypePriceElements();
                    GetPIMPictures(vParams.ProductID);
                    LoadItemRequisition();
                    mElement.find('#tabUPIOrdering').opentabmenu();
                    //try {
                    //    if(proj.IsBootstrapDesign()){
                    //        $('#' + popupid).modal('show');
                    //    } else {
                    //        mElement.show().popup().popup('open');
                    //    }                        
                    //} catch (ex) {
                    //    // just ignore the error
                    //}
                },
                Fail: function(vMessage) {
                    Bs.Loading('hide');
                    NewMessageBox(vMessage);
                }
            });

            // delete these comment after 1 month 2021_05_26__14_04_31_6294
            //CreatePriceHistoryDataTable();
            //CreatePriceMatrixDataTable();
            CreateStocksInTransitDataTable();
            CreatePurchaseOrderHistoryDataTable();
        };

        mOut.UPIpopup = function(){
            
            AjaxSend({
                url: '/SalesMenu/GetCustomer',
                data: {
                    GetOrAddDefaultAddresses: 1,
                    IncludeZRead: 1
                },
                success: function(vData) {
                    mOut.Model.Data.CustomerID = vData.ID;

                    //mElement.find('[name=title]').text('Product Information');
                    $('[name="spanProductNumber"], [name="title"]').text('');
                    $('[name="imgDefaultPic"]').attr('src', '/Content/images/box.png').unbind('click');
                    $('[name="divforUPIScanner"]').show();
                    CollapsedAccordion();
                    $('#' + popupid).find('.nav-link').removeClass('active');
                    $('#' + popupid).find('.tab-pane').removeClass('show active');
                    $('#' + popupid).modal('show');
                    mOut.Model.Data.ProductID = '';

                    $('[name="txtSearchUPI"]').val('').unbind('keydown').keydown(function(e){
                        if (e.which == 13){
                            var val = $.trim($(this).val()), opt = {}, isdisplaysearhkey = true;

                            if (val.substr(0, 5) == 'BLT--' || val.substr(0, 5) == 'PST--') {
                                isdisplaysearhkey = false;
                                opt.Base64ToGuid = Base64ToGuid(val.substr(5));
                                $(this).val('');
                            }

                            BrowsePopup('LibraryPIMProduct', 'Search Product', [
                                {
                                    DisplayText: [{Label: 'Product No.', Column: 'ProductNo'}, {Label: 'Brand', Column: 'Brand'}, {Label: 'Description', Column: 'ProductNameOnly'}],
                                    Search: val,
                                    DisplaySearhKey: isdisplaysearhkey,
                                    HidePrevPopup: false,
                                    Click: function(vData) {
                                        mOut.popup({
                                            IsUPIScanner: true,
                                            ProductID: vData.ID,
                                            ProductName: vData.ProductName,
                                            ProductNo: vData.ProductNo,
                                            CustomerID: mOut.Model.Data.CustomerID,
                                            Success: function() {

                                            }
                                        });
                                    }
                                }
                            ], opt)
                        }
                    })

                    $('[name="btnScanUPI"]').click(function () {
                        if ((CBool(window.IsMobile)) && (!CBool(window.InWindows))) {
                            mobile.Scan({
                                Success: function (vData) {
                                    $('[name="txtSearchUPI"]').val(vData.Value).trigger($.Event( "keydown", { which: 13 } ));
                                }
                            });
                        } else {
                            NewMessageBox('Scan Barcode / QR Code feature is only available on the mobile app', 'Message');
                        }
                    });
                }
            });
        }

        function CollapsedAccordion() {
            $('#colVendorPriceMatrix, #colSellingPriceMatrix, #colPriceHistory, #colLedger, #colSalesRepresentative, ' +
                '#colPOHistory').collapse('hide');
        }

        function GetPIMPictures(id) {
            AjaxSend({
                url: 'Organization/GetPIMPictures',
                data: { ProductID: id },
                success: function (e) {
                    var vRows = e.Results.Rows;
                    var divProductImages = $('#divProductImages');
                    divProductImages.empty();

                    if(vRows.length > 0){
                        for (var i = 0; i < vRows.length; i++) {
                            vRows[i].Index = i;
                            var srcBig = '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/Big/' + vRows[i].GroupImageID + '_big.jpg';

                            if (CBool(vRows[i].IsDefault)) {
                                $('[name="imgDefaultPic"]').attr('picid', vRows[i].ID).attr('src', srcBig).unbind('click').click(function(){
                                    $('#' + $(this).attr('picid')).click()
                                });
                            }

                            divProductImages.append($('<a data-role="none" data-lightbox="productimages">')
                                .attr('id', vRows[i].ID)
                                .attr('href', srcBig)
                                .append($('<img>')
                                    .data('data', vRows[i]).attr('src', srcBig)
                                )
                            )
                        }
                    } else {
                        $('[name="imgDefaultPic"]').attr('src', '/Content/images/box.png').unbind('click');
                    }                        
                }
            })
        }
        
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

            if (!mHidePrices)
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
            mElement.find('[name="listOrderInputSalesRep"]').empty();
            arrSalesRep = [];
        }

        function DisplayData(vData) {
            var pTRs = [];           

            if (vData.UnitMeasures == null) {
                if(proj.IsBootstrapDesign()){
                    $('#' + popupid)('hide');
                } else {
                    mElement.show().popup('close');
                }
                NewMessageBox('The product, ' + mProductName + ', has no price or unit.', 'No price or unit');
            }

            for (var p = 0; p <= vData.UnitMeasures.Rows.length - 1; p++) {
                var pDR = vData.UnitMeasures.Rows[p];

                if (p == 0)
                    mElement.find('[name=BaseUnit]').text(pDR.BaseUnit);
                AddRow(pDR, null, vData);
                if (mTransType != 'AccountsPayable') {
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

            if (vData.SalesReps != undefined){
                arrSalesRep = vData.SalesReps.Rows;
                DisplaySalesRep();
            }            

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
            if (mTransType == 'PurchaseOrder')
                GetVendorPrices()
            if (true) {
                mElement.find('[name=NetPricePerSellingUnit] input').show();
                mElement.find('[name=NetPricePerSellingUnit] span').hide();
            } else {
                mElement.find('[name=NetPricePerSellingUnit] input').hide();
                mElement.find('[name=NetPricePerSellingUnit] span').show();
            }
            if (mHidePrices)
                mElement.find('.PriceClass').hide();
            else
                mElement.find('.PriceClass').show();

            function AddRow(vDR, vDrPriceBreak, vData) {
                
                var pTR = mTR.clone();
                var pCbPrice1st = pTR.find('[name=PriceLevel] select');
                var pArrQty = DisplayPrices(
                    pCbPrice1st,
                    vDR.UnitMeasure,
                    vData.data.Rows,
                    vDrPriceBreak,
                    vDR.Quantity
                );
                var pIsFirstItem = true;
                if ((pArrQty == null)
                    || (pArrQty.length == 0))
                    pArrQty = [{}];
                var pArrQty2 = [];
                for (var pDrQty of pArrQty) {
                    (function() {
                        var pCbPrice;
                        pDrQty = $.extend({}, pDrQty);
                        pArrQty2.push(pDrQty);
                        if (pIsFirstItem) {
                            pIsFirstItem = false;
                            pCbPrice = pCbPrice1st;
                        }  else {
                            pTR = mTR.clone();
                            pCbPrice = pTR.find('[name=PriceLevel] select');
                            for (var pCbPrice1stOption of pCbPrice1st.find('option').toArray()) {
                                pCbPrice1stOption = $(pCbPrice1stOption);
                                var pCbPriceOption = new Option(pCbPrice1stOption.data('data').PriceLevel, pCbPrice1stOption.data('data').PriceLevelID);
                                pCbPriceOption = $(pCbPriceOption);
                                if (pCbPrice1stOption.data('PriceBreak') != null)
                                    pCbPriceOption.data('PriceBreak', pCbPrice1stOption.data('PriceBreak'));
                                var pCbPrice1stOptionData = pCbPrice1stOption.data('data');
                                pCbPrice1stOptionData.DiscountedPrice = pDrQty.UnitPrice;
                                pCbPriceOption.data('data', pCbPrice1stOptionData);
                                pCbPrice.append(pCbPriceOption);
                            }
                        }
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
                        pTR.find('[name=SRP]').text(AccountingFormat(vDR.SRP, 4)); // removed * pUnitQty
                        var pTxtPaymentTerm = pTR.find('[name=tdPaymentTerm] input');
                        pTxtPaymentTerm.val(AccountingFormat(vDR.TermNumericValue, 0, ''));
                        pTR.data('UnitQty', pUnitQty);
                        pDrQty.UnitQty = pUnitQty;
                        pDR.OriginalUnitPrice = pDrQty.OriginalUnitPrice;
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
                                    NewMessageBox('You may not modify the served quantities of invoices that came from sales order.');
                                });
                            }
                        // // commented on 2021_10_30__14_31_07_5675 
                        //} else if (mTransType == 'AccountsPayable') {
                        //    pTxtOrdered.prop('disabled', true);
                        //    pTxtOrdered.click(function() {
                        //        // click not triggered
                        //        NewMessageBox('You may not modify the received quantities of accounts payable.');
                        //    });
                        } else
                            pTxtOrdered.on('textchange', txtQty_Change);
                        //var pSelectedPriceLevelID = pDR.PriceLevelID;
                        //if (IsGuid(pDrQty.PriceLevelID))
                        //     pSelectedPriceLevelID = pDrQty.PriceLevelID;
                        //pCbPrice.find('option[value=' + pSelectedPriceLevelID + ']').attr('selected', true);

                        // remove the custom price option
                        pCbPrice1st.find('option[value=' + proj.CustomPriceLevelID + ']').remove();

                        pDrQty.PriceSelect = pCbPrice;
                        pCbPrice.on('change', CbPrice_Change);
                        pCbPrice.on('click', CbPrice_Click);

                        //pCbPrice.trigger('change');

                        if (CStr2(pDrQty.TradeDiscountLess) != '') {
                            pTR.data('data').TradeDiscountLess = pDrQty.TradeDiscountLess;
                            pTR.data('data').NetDiscountFactor = pDrQty.NetDiscountFactor;
                            pTR.data('data').NetDiscountRate = pDrQty.NetDiscountRate;
                            pTR.find('[name=DiscountRate] a').text(pDrQty.TradeDiscountLess + ' (' + pDrQty.NetDiscountRate + '% NDR)')
                        }

                        pTR.find('[name=DiscountRate] a').click(function () {
                            selectedItemTR = $(this).parents().closest('tr');

                            if (proj.IsBootstrapDesign()){
                                //$('#' + popupid).modal('hide');
                                $('#popOrderTradeDiscount-bs').modal('show');
                                //$('#popOrderTradeDiscount-bs').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                                //    $('#' + popupid).modal('show');
                                //    $('#popOrderTradeDiscount-bs').unbind('hidden.bs.modal');
                                //})
                            } else {
                                $('#' + popupid).popup('close');
                                $('#popOrderTradeDiscount').popup('open');
                            }
                    
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
                        var pTxtPrice = pTR.find('[name=NetPricePerSellingUnit] input');

                        // commented on 2022_03_29__15_17_09_8659
                        //if (mTransType == 'AccountsPayable') {
                        //    if (CDec2(pDrQty.ServedQuantity) == 0) {
                        //        pTxtPrice.prop('disabled', true);
                        //        pTxtPaymentTerm.prop('disabled', true);
                        //    }
                        //}

                        var pLnkUpdateToLatestPrice = pTR.find('[name=NetPricePerSellingUnit] [name=lnkUpdateToLatestPrice]');
                        pLnkUpdateToLatestPrice.click(function(e) {
                            var pTR = $(e.target).closest('tr');
                            var pCbPrice = pTR.find('[name=PriceLevel] select');
                            var pDrPrice = pCbPrice.find('option:selected').data('data');
                            Bs.Loading('show');

                            mOut.Model.GetData({
                                Success: function(vData) {
                                    Bs.Loading('hide');
                                    for (var pRowIndex = 0; pRowIndex <= vData.data.Rows.length - 1; pRowIndex++) {
                                        var pRow = vData.data.Rows[pRowIndex];
                                        if ((ReadyGuid(pRow.PriceLevelID) == ReadyGuid(pCbPrice.find('option:selected').val()))
                                            && (pRow.Quantity == pDrPrice.Quantity)) {
                                            pDrPrice.DiscountedPrice = pRow.DiscountedPrice;
                                            pDrPrice.DiscountedPricePerUnit = pRow.DiscountedPricePerUnit;
                                        }
                                    }
                                    pCbPrice.trigger('change');
                                    return;
                                    var pTxtPrice = pTR.find('[name=NetPricePerSellingUnit] input');
                                    pTxtPrice.val(AccountingFormat(pDrPrice.DiscountedPrice * (1 + mUnitPriceSurchargePercent / 100), 4));

                                    var pTdOptimizedOrderQtyForDiscounts = pTR.find('[name=OptimizedOrderQtyForDiscounts]');
                                    var pOptimizedOrderQtyForDiscounts = CDec2(pTdOptimizedOrderQtyForDiscounts.val());
                                    if (pOptimizedOrderQtyForDiscounts == 0)
                                        pOptimizedOrderQtyForDiscounts = CDec2(pTdOptimizedOrderQtyForDiscounts.text());
                                    var pSubTotal = CDec2(pDrPrice.DiscountedPrice) * pOptimizedOrderQtyForDiscounts;
                                    pTR.find('[name=SubTotal]').text(AccountingFormat(pSubTotal, 4));
                                    DisplayTotal();
                                },
                                Fail: function(vMessage) {
                                    Bs.Loading('hide');
                                    NewMessageBox(vMessage);
                                }
                            });
                            
                        });
                        pTxtPrice.change(function () {
                            var tr = $(this).parents().closest('tr');
                            var data = tr.data('data');
                            var pSubTotal = CDec2(tr.find('[name="txtQty"]').val()) * CDec2($(this).val());

                            tr.find('[name="SubTotal"]').text(AccountingFormat(pSubTotal, 4));
                            tr.find('[name="DiscountRate"] a').text('Set Trade Discount');
                            data.TradeDiscountLess = '';
                            data.NetDiscountFactor = 0;
                            data.NetDiscountRate = 0;
                        });

                        if (vData.VendorPriceInfo == null)
                            pTR.find('[name=VendorPrice]').text('N / A');
                        else {
                            vData.VendorPriceInfo.Price = CDec2(vData.VendorPriceInfo.Price);
                            var pText = HtmlEncode(AccountingFormat(vData.VendorPriceInfo.Price * pUnitQty, 4)) + '<br />';
                            pText += HtmlEncode(FormatDate(vData.VendorPriceInfo.LastUpdated, {
                                    DontDisplay12Am: true,
                                    Format: 'MMM dd, yyyy hh:mm a',
                                    FormatNoTime: 'MMM dd, yyyy'
                                }));
                            pTR.find('[name=VendorPrice]').html(pText);
                        }
                    
                        pTRs.push(pTR);
                    })(); // END closure
                } // END for (var pDrQty of pArrQty) {

                // 2nd loop
                for (var pDrQty of pArrQty2) {
                    //pDrQty.PriceSelect.find('option[value=' + pDrQty.PriceLevelID + ']').attr('selected', true);
                    var pHasSelectedPrice = false;
                    if (CStr2(pDrQty.PriceLevelID) != '') {
                        pDrQty.PriceSelect.val(pDrQty.PriceLevelID);
                        pHasSelectedPrice = true;
                    }
                    var pOptionData = null;
                    if (pDrQty.TransUnitPrice != null) {
                        var pOptions = pDrQty.PriceSelect.find('option');
                        var pPriceWasFound = false;
                        for (var pOption of pOptions.toArray()) {
                            pOption = $(pOption);
                            pOptionData = pOption.data('data');
                            if ((pOptionData.PriceLevelID == pDrQty.TransPriceLevelID)
                                && (Math.abs(CDec2(pDrQty.TransUnitPrice) - CDec2(pOptionData.DiscountedPrice)) < .001)) { // floating number does not exactly match. so, use tolerance

                                pPriceWasFound = true;
                                pDrQty.PriceSelect.val(pOptionData.PriceLevelID);
                                pHasSelectedPrice = true;
                                break;
                            }
                        }
                        if (!pPriceWasFound ) {
                            var pOption = new Option('Custom Price', proj.CustomPriceLevelID);
                            pOption = $(pOption);
                            pOptionData = $.extend(pOptionData, {
                                DiscountedPrice: pDrQty.TransUnitPrice,
                                DiscountedPricePerUnit: pDrQty.TransUnitPrice,
                                Quantity: pDrQty.UnitQty,
                                PriceLevelID: proj.CustomPriceLevelID,
                                PriceLevel: 'Custom Price'
                            });
                            pOption.data('data', pOptionData);
                            pDrQty.PriceSelect.append(pOption);
                            pDrQty.PriceSelect.val(proj.CustomPriceLevelID);
                            pHasSelectedPrice = true;
                        }
                    }
                    if (!pHasSelectedPrice)
                        pDrQty.PriceSelect.val(proj.RetailPriceID);
                    pDrQty.PriceSelect.trigger('change');
                } // END for (var pDrQty of pArrQty) { 2nd loop

            } // END function AddRow(vDR, vDrPriceBreak, vData) {

            //if ((mTransType == 'Invoice') 
            //    && mFromSalesOrder)
            //    NewMessageBox('You may not modify served quantities of invoices that came from sales order.');

            function DisplayPrices(vComboBox, vUnit, vPrices, vDrPriceBreak) { 
                var pUniquePriceLevels = [];
                var pPriceBreakQty;
                var pDrFilteredCart;
                var pDR; // declare this variable outside the loop

                for (var p = 0; p <= vPrices.length - 1; p++) {
                    pDR = vPrices[p];
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
                    if ((pDrFilteredCart == null)
                        || (pDrFilteredCart.length == 0)) {
                        pDrFilteredCart = FilterCart(pDR.Quantity, pDR.DiscountedPrice / pPriceBreakQty, pPriceBreakQty);
                        if ((pDrFilteredCart != null)
                            && (pDrFilteredCart.length > 0)) {

                            //pDR.DiscountedPrice = pDrFilteredCart[0].UnitPrice;
                            //if (IsGuid(pDrFilteredCart[0].PriceLevelID))
                            //    pDR.PriceLevelID = pDrFilteredCart[0].PriceLevelID;
                        }
                    }

                    $(pOption).data('data', pDR);
                    vComboBox.append(pOption);
                    pUniquePriceLevels.push(pDR.PriceLevel.toLowerCase());
                }

                if ((pDrFilteredCart == null)
                    || (pDrFilteredCart.length == 0)) {

                    if (pDR != null) {
                        var pNewDr = {};
                        pNewDr.PriceLevelID = proj.CustomPriceLevelID;
                        pNewDr.PriceLevel = 'Custom Price';
                        pNewDr.OrderedQty= pDR.OrderedQty;
                        pNewDr.Quantity = pDR.Quantity;
                        var pOption = new Option(pNewDr.PriceLevel, pNewDr.PriceLevelID);
                        $(pOption).data('data', pNewDr);
                        vComboBox.append(pOption);
                    }
                }
                return pDrFilteredCart;
            }

            function FilterCart(vUnitQty, vUnitPrice, vPriceBreakQty) {
                var pOut = [];
                if (vData.Cart == null)
                    return;
                for (var p = 0; p <= vData.Cart.Rows.length - 1; p++) {
                    var pDR = vData.Cart.Rows[p];
                    if (!pDR.Used
                        && (CDec2(pDR.ActualQuantity) == CDec2(vUnitQty))) {
                        // && (Math.abs(CDec2(pDR.UnitPrice) - CDec2(vUnitPrice)) < .01)) { // floating number does not exactly match. so, use tolerance
                        pDR.Used = true;

                        pOut.push({
                            TransUnitPrice: pDR.UnitPrice,
                            TransPriceLevelID: pDR.PriceLevelID,
                            UnitPrice: pDR.OriginalUnitPrice, // pDR.UnitPrice,
                            OriginalUnitPrice: pDR.OriginalUnitPrice,
                            OrderedQuantity: pDR.OrderedQuantity / vPriceBreakQty,
                            ServedQuantity: pDR.Quantity / vPriceBreakQty,
                            TradeDiscountLess: CStr2(pDR.TradeDiscountLess),
                            NetDiscountFactor: CDec2(pDR.NetDiscountFactor),
                            NetDiscountRate: CDec2(pDR.NetDiscountRate)
                        });
                    }
                }

                return pOut;
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

        function GetVendorPrices() {
            AjaxSend({
                url: 'Browser/LibraryVendorPrice',
                data: { ProductID :mOut.Model.Data.ProductID },
                success: function (e) {
                    if ($.fn.DataTable.isDataTable("#tblOrderInputVendorPrices")) {
                        //check if datatable is created 
                        $('#tblOrderInputVendorPrices').DataTable().clear().destroy();
                    }

                    var ctr = 0, pRows = e.Results.Rows, tblVendorSKUs = $('#tblOrderInputVendorPrices').DataTable({
                        "bdestroy": true,
                        "pageLength": 10,
                        "order": [[0, "asc"]],
                        data: pRows,
                        "autoWidth": false,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    ctr += 1;
                                    return ctr + '.';
                                },
                                "width": "30px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.CompanyName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return AccountingFormat(data.Price, 4);
                                },
                                'createdCell':  function (td, cellData, rowData, row, col) {
                                    $(td).addClass('text-right');
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return FormatDate(data.LastUpdated, { Format: 'MM/dd/yyyy hh:mm a' });
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return CStr2(data.UpdatedByName);
                                }
                            }
                        ],
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                        "responsive": true,
                        "columnDefs": [{ "targets": 0, "orderable": false }],
                        "sDom": '<"top">rt<"bottom"ip><"clear">',
                        "initComplete": function () {
                            $('#tblOrderInputVendorPrices').removeClass('dataTable');
                        }
                        //"stateSave": true
                    })
                }
            })
        }

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
            var pHtml = '<b>' + AccountingFormat(pDrPrice.DiscountedPricePerUnit, 4) + '</b>';
            if (IsDate(pDrPrice.LastUpdated)) {
                pHtml += '<br />Last&nbsp;Updated:<br />' + FormatDate(pDrPrice.LastUpdated, { Format: 'M/d/yyyy' });
                pHtml += '<br />' + AccountingFormat((new Date().getTime() - new Date(pDrPrice.LastUpdated)) / 1000 / 60 / 60 / 24, 0) + ' days ago';
                pHtml += '<br /><a name="lnkRequestPriceUpdate" href="">Request Price Update</a>';
            }
            pTR.find('[name=NetPricePerBaseUnit]').html(pHtml);
            pTR.find('[name="lnkRequestPriceUpdate"]').click(function() {
                PriceUpdateRequest({
                    ID: mOut.Model.Data.ProductID,
                    ProductNo: mProductNo,
                    ProductName: mProductName
                });
            });
            var p = AccountingFormat(pDrPrice.DiscountedPrice * (1 + mUnitPriceSurchargePercent / 100), 4);
            pTR.find('[name=NetPricePerSellingUnit] span').text(p);
            pTR.find('[name=NetPricePerSellingUnit] input').val(AccountingFormat(p, 4, ''));
            var pTdOptimizedOrderQtyForDiscounts = pTR.find('[name=OptimizedOrderQtyForDiscounts]');
            var pOptimizedOrderQtyForDiscounts = CDec2(pTdOptimizedOrderQtyForDiscounts.val());
            if (pOptimizedOrderQtyForDiscounts == 0)
                pOptimizedOrderQtyForDiscounts = CDec2(pTdOptimizedOrderQtyForDiscounts.text());
            var pSubTotal = CDec2(pDrPrice.DiscountedPrice * (1 + mUnitPriceSurchargePercent / 100)) * pOptimizedOrderQtyForDiscounts;
            pTR.find('[name=SubTotal]').text(AccountingFormat(pSubTotal, 4));
            DisplayTotal();
        }

        function CbPrice_Click(e) {
            var pThis = $(this);
            mOut.Model.GetData({
                Success: function(vData) {
                    for (var pY = 0; pY <= vData.data.Rows.length - 1; pY++) {
                        var pDR = vData.data.Rows[pY];
                        var pOptions = $(pThis.find('option'));
                        for (var pOptionIndex = 0; pOptionIndex <= pOptions.length - 1; pOptionIndex++) {
                            var pOption = pOptions[pOptionIndex];
                            pOption = $(pOption);
                            var pOptionData = pOption.data('data');
                            if ((CDec2(pOptionData.Quantity) == CDec2(pDR.Quantity))
                                && (ReadyGuid(pOption.val()) == ReadyGuid(pDR.PriceLevelID))) {
                                
                                pOptionData.DiscountedPrice = pDR.DiscountedPrice;
                                pOptionData.DiscountedPricePerUnit = pDR.DiscountedPricePerUnit;
                            }
                        }
                    }
                    pThis.trigger('change');
                },
                Fail: function() {

                }
            });
        }

        function txtQty_Change(e) {
            //var pTR = $(this).closest('tr[name=data]');
            //var pDrPrice = pTR.find('[name=PriceLevel] option:selected').data('data');
            //var pSubTotal = CDec2(pDrPrice.DiscountedPrice) * CDec2($(this).val());
            //pTR.find('[name=SubTotal]').text(AccountingFormat(pSubTotal));
            OptimizeQuantities();
        }

        function OptimizeQuantities() {
            if (mTransType == 'AccountsPayable') {
                DisplaySubtotal();
                return;
            }

            DisplaySubtotal();
            return;
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
                //if (p == 0)
                //    pQty = pRemaining;
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

        function DisplaySubtotal() {
            var pTRs = mTable.find('tr[name=data]');
            for (var p = pTRs.length - 1; p >= 0; p--) {
                var pTR = $(pTRs[p]);
                var pTxt = pTR.find('[name=NetPricePerSellingUnit] input');

                var pQty;
                if (mTransType == 'Invoice') 
                    pQty = CDec2(pTR.find('[name=txtServedQty]').val());
                else
                    pQty = CDec2(pTR.find('[name=txtQty]').val());
                pTR.find('[name=OptimizedOrderQtyForDiscounts]').text(AccountingFormat(pQty, 0));
                var pSubTotal = CDec2(pTxt.val()) * pQty;
                pTR.find('[name=SubTotal]').text(AccountingFormat(pSubTotal, 4));
            }
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
            mElement.find('[name=ProductTotal]').text(AccountingFormat(pTotal, 4));
        }

        mElement.find('[name=butClose]').click(function() {
            if(proj.IsBootstrapDesign()){
                $('#' + popupid)('hide');
            } else {
                mElement.popup().popup('close');
            }            
        });

        mElement.find('[name=butOk]').click(function() {
            var pData = {
                Stocks: [],
                TransType: mParentController.TransType,
                IsPos: mParentController.IsPos,
                ApproveProfitability: mIncludeApprove,
                SalesRep: arrSalesRep
            };
            var pDrPrice;
            var pDrPriceBreak;

            var pDivUnits = mTable.find('tr[name=data]');
            for (var p = 0; p <= pDivUnits.length - 1; p++) {
                var pStock = {};
                var pDivUnit = $(pDivUnits[p]);
                var pDivUnitData = pDivUnit.data('data');
                pStock.Qty = CDec2(pDivUnit.find('[name=OptimizedOrderQtyForDiscounts]').text());
                if ((pStock.Qty <= 0)
                    && CBool(mOut.Model.Data.DeleteExistingProduct))
                    continue;
                var pCB = pDivUnit.find('[name=PriceLevel] select');
                if (pCB == null)
                    continue;
                var pOption = $(pCB.find(':selected'));
                if (pOption.length == 0)
                    continue;
                pDrPrice = pOption.data('data');
                pDrPriceBreak = pOption.data('PriceBreak');
                pStock.UnitQty = pDrPrice.Quantity;
                pStock.UnitMeasureID = pDrPrice.UnitMeasureID;
                pStock.UnitMeasure = pDrPrice.UnitMeasure;
                pStock.OriginalUnitPrice = pDivUnitData.OriginalUnitPrice;
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

                var pPaymentTerm = pDivUnit.find('[name=tdPaymentTerm] input').val();
                if (pPaymentTerm.trim() == '')
                    pStock.PaymentTerm = 0;
                else if (!IsNumeric(pPaymentTerm)) {
                    NewMessageBox('Payment Term must be numeric. You entered ' + pPaymentTerm, 'Invalid Input', [], {
                        Close: function() {
                            try {
                                if(proj.IsBootstrapDesign()){
                                    $('#' + popupid).modal('show');
                                } else {
                                    mElement.show().popup('open');
                                }
                            } catch (ex) {
                                // just ignore the error
                            }
                        }
                    });
                    return;
                } else 
                    pStock.PaymentTerm = CDec2(pPaymentTerm);
                if (mMaxPaymentTerm < CDec2(pPaymentTerm)) {
                    NewMessageBox('Mobiz cannot save your current transaction because: The maximum allowed payment term for this ' + mTargetOrgType + ' is ' + mMaxPaymentTerm + ' but you entered ' + pPaymentTerm + '. Please enter a value that is equal to or lesser than ' + mMaxPaymentTerm + '.', 'Invalid Input', [], {
                        Close: function() {
                            try {
                                if(proj.IsBootstrapDesign()){
                                    $('#' + popupid).modal('show');
                                } else {
                                    mElement.show().popup('open');
                                }
                            } catch (ex) {
                                // just ignore the error
                            }
                        }
                    });
                    return;
                }
                pData.Stocks.push(pStock);
            }

            if (mParentController.IsPos) {
                var pData2 = $.extend(mOut.Model.Data, {
                    ProductID: mOut.Model.Data.ProductID,
                    EditMode: mEditMode
                }, pData)
                AjaxSend({
                    url: '/Stock/UpdatePosLite',
                    data: pData2,
                    success: function(vData) {
                        if ($.isFunction(mParentController.AfterProductSave))
                            mParentController.AfterProductSave();
                        try {
                            if (proj.IsBootstrapDesign()) {
                                $('#' + popupid).modal('hide');
                            } else {
                                mElement.popup().popup('close');
                            }
                        } catch (e) {
                            // error: "cannot call methods on popup prior to initialization; attempted to call method 'close'"
                        }
                    },
                    error: function(vMessage) {
                        NewMessageBox(vMessage, '', [
                            {
                                Text: 'Ok',
                                Click: function() {
                                    setTimeout(function() {
                                        if (proj.IsBootstrapDesign()) {
                                            $('#' + popupid).modal('show');
                                        } else {
                                            mElement.show().popup('open');
                                        }
                                    }, 500);
                                }
                            }
                        ]);
                    }
                });
            } else {
                pData.TransType = mTransType;
                Bs.Loading('show');
                mOut.Model.Save({
                    data: pData,
                    Success: function(vData) {
                        Bs.Loading('hide');
                        if (vData.BelowGpmThreshold) {
                            var pMsg = `Some product(s) have a gross profit margin that is below the minimum threshold set by the company.<br /><br />
                                You will still be able to save this transaction but profitability approval needs to be made by an authorized person before this transaction can be processed further.<br /><br />
                                Please get in touch with your company’s authorized person for approvals to expedite this transaction.`;
                            NewMessageBox(pMsg, 'Gross Profitability Warning');
                        }
                        if ($.isFunction(mParentController.AfterProductSave))
                            mParentController.AfterProductSave();
                        try {
                            if (proj.IsBootstrapDesign()) {
                                $('#' + popupid).modal('hide');
                            } else {
                                mElement.popup().popup('close');
                            }
                        } catch (e) {
                            // error: "cannot call methods on popup prior to initialization; attempted to call method 'close'"
                        }
                    },
                    Fail: function(vMessage) {
                        Bs.Loading('hide');
                        NewMessageBox(vMessage, '', [
                            {
                                Text: 'Ok',
                                Click: function() {
                                    setTimeout(function() {
                                        if (proj.IsBootstrapDesign()) {
                                            $('#' + popupid).modal('show');
                                        } else {
                                            mElement.show().popup('open');
                                        }
                                    }, 500);
                                }
                            }
                        ]);
                    }
                }); // END mOut.Model.Save({
            }
        });

        // 2020_09_15__09_16_19_7156
        // GetSecurityCurrentUserAcccess was transferred to common/common.js

        mOut.Init = function() {
            mOut.Model = new ERP.OrderProcessing.product.model();

            if (CBool(sessionStorage.IsWebPos)){
                $('#accUPISalesRepresentative').show();
            } else {
                $('#accUPISalesRepresentative').hide();
            }            
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
                                    return AccountingFormat(vData.UnitPrice, 4);
                                }
                            },
                            {
                                data: function(vData) {
                                    return AccountingFormat(vData.SubTotal, 4);
                                }
                            }
                        ],
                        initComplete: function(){
                            mTablePriceHistory.removeClass('dataTable');
                        }
                    });
                },
                Fail: function(vError) {
                    NewMessageBox(vError);
                }
            });
        } // END CreatePriceHistoryDataTable
        
        var mTablePriceMatrix = mElement.find('[name=tablePriceMatrix]');
        var mPriceMatrixDataTable;

        function ClearPriceMatrix() {
            if ($.fn.DataTable.isDataTable(mTablePriceMatrix)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTablePriceMatrix.empty();
                mPriceMatrixDataTable.destroy();
            }
        }

        function CreatePriceMatrixDataTable() {
            ClearPriceMatrix();
            mOut.Model.GetPriceMatrix({
                CustomerID: mOut.Model.Data.CustomerID,
                ProductID: mOut.Model.Data.ProductID,
                Success: function(vData) {
                    mPriceMatrixDataTable = mTablePriceMatrix.DataTable({
                        data: vData.Rows,
                        order: [],
                        columns: [
                            {
                                title: 'Unit Measure',
                                render: function(v1, v2, vData) {
                                    return CStr2(vData.UnitMeasure);
                                }
                            },
                            {
                                title: 'Unit Quantity',
                                className: 'dt-body-right',
                                render: function(v1, v2, vData) {
                                    return $.format.number(CDec2(vData.Quantity), '#,###.##');
                                }
                            },
                            {
                                title: 'Price Level Name',
                                render: function(v1, v2, vData) {
                                    return CStr2(vData.PriceLevel);
                                }
                            },
                            {
                                title: 'Discount (%)',
                                className: 'dt-body-right',
                                render: function(v1, v2, vData) {
                                    return AccountingFormat(vData.Discount, 4);
                                }
                            },
                            {
                                title: 'Discounted Price',
                                className: 'dt-body-right',
                                render: function(v1, v2, vData) {
                                    return AccountingFormat(vData.DiscountedPrice, 4);
                                }
                            },
                            {
                                title: 'Discounted Price Per Base Unit',
                                className: 'dt-body-right',
                                render: function(v1, v2, vData) {
                                    return AccountingFormat(vData.DiscountedPricePerUnit, 4);
                                }
                            },
                            {
                                title: 'Last Updated',
                                render: function(v1, v2, vData) {
                                    return FormatDate(vData.LastUpdated);
                                }
                            }
                        ],
                        initComplete: function(){
                            mTablePriceMatrix.removeClass('dataTable');
                        }
                    });
                    mTablePriceMatrix.addClass('table-stripe').css('width', '100%');
                    mTablePriceMatrix.find('thead tr').addClass('custom-theme-f');
                },
                Fail: function(vError) {
                    NewMessageBox(vError);
                }
            });
        } // END CreatePriceHistoryDataTable

        var mTableStocksInTransit = mElement.find('[name=tableStocksInTransit]');
        var mStocksInTransitDataTable;

        function ClearStocksInTransit() {
            if ($.fn.DataTable.isDataTable(mTableStocksInTransit)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTableStocksInTransit.empty();
                mStocksInTransitDataTable.destroy();
            }
        }

        function CreateStocksInTransitDataTable() {
            ClearStocksInTransit();
            mOut.Model.GetStocksInTransit({
                ProductID: mOut.Model.Data.ProductID,
                Success: function(vData) {
                    mStocksInTransitDataTable = mTableStocksInTransit.DataTable({
                        data: vData.Rows,
                        order: [],
                        columns: [
                            
                            //{
                            //    title: 'ETA',
                            //    render: function(v1, v2, vData) {
                            //        return CStr2(vData.UnitMeasure);
                            //    }
                            //},
                            {
                                title: 'PO Date',
                                render: function(v1, v2, vData) {
                                    return FormatDate(vData.PurchaseOrderDate);
                                }
                            },
                            {
                                title: 'PO No,',
                                render: function(v1, v2, vData) {
                                    return '<div name="divPo" />';
                                }
                            },
                            {
                                title: 'Supplier',
                                render: function(v1, v2, vData) {
                                    return CStr2(vData.CompanyName);
                                }
                            },
                            {
                                title: 'Quantity',
                                className: 'dt-body-center',
                                render: function(v1, v2, vData) {
                                    return AccountingFormat(vData.OrderedQuantity, 0);
                                }
                            },
                            {
                                title: 'Unit Measure',
                                className: 'dt-body-center',
                                render: function(v1, v2, vData) {
                                    return CStr2(vData.UnitMeasure);
                                }
                            },
                            {
                                title: 'Running Total',
                                className: 'dt-body-right',
                                render: function(v1, v2, vData) {
                                    return AccountingFormat(vData.RunningTotal, 0);
                                }
                            }
                            
                        ],
                        initComplete: function(){
                            mTableStocksInTransit.removeClass('dataTable');
                        },
                        rowCallback: function(row, data, dataIndex) {
                            var pCell = $(row).find('[name=divPo]');
                            if (pCell.length == 0)
                                return;
                            var pUrl = location.origin + '/erp' + location.search + '&IsReadOnly=1&IsPreview=1&ID=' + data.ID;
                            var pLink = $('<a>')
                                .text(CStr2(data.MIN) + '-' + CStr2(data.PurchaseOrderNo))
                                .attr('href', pUrl)
                                .click(function(e) {
                                    e.preventDefault();
                                    var win = window.open(location.origin + '/PurchaseOrder/Preview' + location.search + '&IsReadOnly=1&ID=' + data.ID,
                                        '_blank',
                                        'width=' + screen.availWidth + ',height=' + screen.availHeight);
                                    return false; // para DILI magclose ang popup
                                });
                            pCell.append(pLink);
                        }
                    });
                    mTableStocksInTransit.addClass('table-stripe').css('width', '100%');
                    mTableStocksInTransit.find('thead tr').addClass('custom-theme-f');
                },
                Fail: function(vError) {
                    NewMessageBox(vError);
                }
            });
        } // END CreatePriceHistoryDataTable
        
        var mTablePurchaseOrderHistory = mElement.find('[name=tablePurchaseOrderHistory]');
        var mPurchaseOrderHistoryDataTable;
        
        function ClearPurchaseOrderHistory() {
            if ($.fn.DataTable.isDataTable(mTablePurchaseOrderHistory)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTablePurchaseOrderHistory.empty();
                mPurchaseOrderHistoryDataTable.destroy();
            }
        }

        function CreatePurchaseOrderHistoryDataTable() {
            ClearPurchaseOrderHistory();
            mOut.Model.GetStocksInTransit({
                ProductID: mOut.Model.Data.ProductID,
                IncludeReceived: true,
                Success: function(vData) {
                    mPurchaseOrderHistoryDataTable = mTablePurchaseOrderHistory.DataTable({
                        data: vData.Rows,
                        order: [],
                        columns: [
                            {
                                title: 'PO No,',
                                render: function(v1, v2, vData) {
                                    return '<div name="divPo" />';
                                }
                            },
                            {
                                title: 'PO Date',
                                render: function(v1, v2, vData) {
                                    return FormatDate(vData.PurchaseOrderDate);
                                }
                            },
                            {
                                title: 'Quantity',
                                className: 'dt-body-right',
                                render: function(v1, v2, vData) {
                                    return AccountingFormat(vData.OrderedQuantity, 0);
                                }
                            },
                            {
                                title: 'Unit Measure',
                                render: function(v1, v2, vData) {
                                    return CStr2(vData.UnitMeasure);
                                }
                            },
                            {
                                title: 'Supplier',
                                render: function(v1, v2, vData) {
                                    return CStr2(vData.CompanyName);
                                }
                            }
                        ],
                        initComplete: function(){
                            mTablePurchaseOrderHistory.removeClass('dataTable');
                        },
                        rowCallback: function(row, data, dataIndex) {
                            var pCell = $(row).find('[name=divPo]');
                            if (pCell.length == 0)
                                return;
                            var pUrl = location.origin + '/erp' + location.search + '&IsReadOnly=1&IsPreview=1&ID=' + data.ID;
                            var pLink = $('<a>')
                                .text(CStr2(data.MIN) + '-' + CStr2(data.PurchaseOrderNo))
                                .attr('href', pUrl)
                                .click(function(e) {
                                    e.preventDefault();
                                    var win = window.open(location.origin + '/PurchaseOrder/Preview' + location.search + '&IsReadOnly=1&ID=' + data.ID,
                                        '_blank',
                                        'width=' + screen.availWidth + ',height=' + screen.availHeight);
                                    return false; // para DILI magclose ang popup
                                });
                            pCell.append(pLink);
                        }
                    });
                    mTablePurchaseOrderHistory.addClass('table-stripe').css('width', '100%');
                    mTablePurchaseOrderHistory.find('thead tr').addClass('custom-theme-f');
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

        mElement.find('[name="btnOrderInputAddSalesRep"]').click(function(){
            BrowsePopup('LibraryOrganizationEmployee', 'Search Sales Representatives', [
                {
                    Message: 'You can select multiple. Click OK when you\'re done.',
                    DisplayText: [{Column: 'PeopleName'}],
                    SelectMultiple: true,
                    AutoSearch: true,
                    Click: function(e) {
                        arrSalesRep = $.merge(arrSalesRep, e);
                        DisplaySalesRep();
                        console.log(arrSalesRep);
                    }
                }
            ])   
        })

        mElement.find('[name="btnUPIOrdering"], [name="btnUPIProductPriceCost"], [name="btnUPIPriceHistory"], [name="btnUPIInventoryBalance"], [name="btnUPIInventoryLedger"], [name="btnUPIRequisitioning"], [name="btnUPIInventoryCounting"]').unbind('click').click(function(){
            if (CStr2(mOut.Model.Data.ProductID) == ''){
                NewMessageBox('Please search product first or you can scan a barcode.');
                return;
            }
            $($(this).attr('data-target')).opentabmenu();
        })

        mElement.find('[name="btnUPIProductPriceCost"]').unbind('click').click(function () {
            if (CStr2(mOut.Model.Data.ProductID) == '') {
                NewMessageBox('Please search product first or you can scan a barcode.');
                return;
            }

            var pData = [];

            pData = {
                ProductID: mOut.Model.Data.ProductID,
                IsActive: 1
            };
            Bs.Loading('show');
            AjaxSend({
                url: 'Organization/getOrganizationProductPriceList',
                data: pData,
                success: function (vData) {
                    var count = 1;

                    $('#tblBodyUPIProductPrice, #tblBodyUPIVendorPrice').empty();

                    if (vData.OrgProductPriceData.Rows.length == 0) {
                        $('#tblBodyUPIProductPrice').append('<tr><td colspan="3" style="text-align:center;">No Result(s) Found</td></tr>')
                    }
                    else {
                        for (var i = 0; i < vData.OrgProductPriceData.Rows.length; i++) {
                            $('#tblBodyUPIProductPrice').append($('<tr style="cursor:pointer;">')
                                .attr('ID', vData.OrgProductPriceData.Rows[i].ID)
                                .append($('<td>')
                                    .text(count + '.')
                                )
                                .append($('<td>')
                                    .text(proj.ChangePriceLevelDisplay(vData.OrgProductPriceData.Rows[i].PriceLevel))
                                )
                                .append($('<td class="text-center">')
                                    .text(AccountingFormat(vData.OrgProductPriceData.Rows[i].Price))
                                )
                                .append($('<td class="text-center">')
                                    .text(AccountingFormat(vData.OrgProductPriceData.Rows[i].GrossProfit))
                                )
                                .append($('<td class="text-center">')
                                    .text(AccountingFormat(vData.OrgProductPriceData.Rows[i].GrossProfitMargin) + '%')
                                )
                                .append($('<td class="text-center">')
                                    .text(vData.OrgProductPriceData.Rows[i].UpdaterName)
                                )
                                .append($('<td class="text-center">')
                                    .text(vData.OrgProductPriceData.Rows[i].LastUpdated)
                                )
                                .data('data', vData.OrgProductPriceData.Rows[i])
                                .click(function (e) {

                                })
                            )
                            count += 1;
                        }
                    }

                    if (vData.VendorPrices.Rows.length == 0) {
                        $('#tblBodyUPIVendorPrice').append('<tr><td colspan="3" style="text-align:center;">No Result(s) Found</td></tr>')
                    }
                    else {
                        count = 1;
                        var mBaseUnit = GetStringObject('Unit.UnitMeasure', vData);
                        var mBaseUnitQty = CDec2(GetStringObject('Unit.Quantity', vData));
                        if (mBaseUnitQty != 1)
                            mBaseUnit += ' (' + mBaseUnitQty + ')';
                        for (var i = 0; i < vData.VendorPrices.Rows.length; i++) {
                            var pUnitMeasure = mBaseUnit;
                            if (CStr2(vData.VendorPrices.Rows[i].UnitMeasure) != '') {
                                pUnitMeasure = vData.VendorPrices.Rows[i].UnitMeasure;
                                if ((CDec2(vData.VendorPrices.Rows[i].UnitQty) > 0)
                                    && (CDec2(vData.VendorPrices.Rows[i].UnitQty) != 1))
                                    pUnitMeasure += ' (' + CDec2(vData.VendorPrices.Rows[i].UnitQty) + ')';
                            }
                            $('#tblBodyUPIVendorPrice').append($('<tr style="cursor:pointer;">')
                                .attr('ID', vData.VendorPrices.Rows[i].ID)
                                .append($('<td>')
                                    .text(count + '.')
                                )
                                .append($('<td>')
                                    .text(vData.VendorPrices.Rows[i].CompanyName)
                                )
                                .append($('<td class="text-center">')
                                    .text(AccountingFormat(vData.VendorPrices.Rows[i].Price))
                                )
                                .append($('<td class="text-center">')
                                    .text(pUnitMeasure)
                                )
                                .append($('<td class="text-center">')
                                    .text(vData.VendorPrices.Rows[i].UpdatedByName)
                                )
                                .append($('<td class="text-center">')
                                    .text(vData.VendorPrices.Rows[i].LastUpdated)
                                )
                                .data('data', vData.VendorPrices.Rows[i])
                                .click(function (e) {

                                })
                            )
                            count += 1;
                        }
                    }

                    Bs.Loading('hide');
                    $('#tabUPIProductPriceCost').opentabmenu();
                }
            });
        })

        var htmlinventorybalance = '';
        function ShowInventory(callback){
            if (htmlinventorybalance == '') {
                Bs.Loading('show');
                AjaxSend({
                    ExactUrl: '/InventoryLedger/InventoryBalance?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1&v=' + assetversion,
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {                        
                        htmlinventorybalance = $('<div>').append(vData);
                        $('#tabOrderInventoryCount').prepend(htmlinventorybalance.find('#panSelectInventoryBalancecontent .table-responsive'));
                        $('#tabOrderICApprovalDisapproval').prepend(htmlinventorybalance.find('#panInventoryCountApprovalcontent .table-responsive'));
                        $('#tabOrderICApprovalDisapprovalHistory').prepend(htmlinventorybalance.find('#panInventoryCountApprovalHistorycontent .table-responsive'));
                        $('#tabOrderSetBegBalance').prepend(htmlinventorybalance.find('#panSetBegBalancecontent > *'));
                        $('#tabOrderCompanyWideIB').prepend(htmlinventorybalance.find('#panCompanyWideInventoryBalancecontent-bs > *'));
                        htmlinventorybalance.find('#divFOWSTInventoryBalance').contents().appendTo('#divFOWSTInventoryBalance-bs');
                        htmlinventorybalance.find('#divCompayWideInventoryBalance').contents().appendTo('#divCompayWideInventoryBalance-bs');
                        htmlinventorybalance.find('#divCompanyWideInventoryBalancePriceLevel').appendTo('#tabOrderCompanyWideIB');

                        var tempinterval = setInterval(function () {
                            if (typeof InventoryBalance != 'undefined') { // naa sa InventoryBalance.js
                                clearInterval(tempinterval)
                                Bs.Loading('hide');
                                callback();
                            }
                        }, 100)
                    }
                });
            } else {
                callback();
            }
        }

        var hasAccessToSBB = false;
        mElement.find('#colSetBegBalance').on('show.bs.collapse', function(){
            function Show(){
                ShowInventory(function(){
                    //$('#tabOrderSetBegBalance').opentabmenu();
                    InventoryBalance.GetProductUnitMeasure(mOut.Model.Data.ProductID);
                });
            }            

            if (!hasAccessToSBB){
                PromptUserAccess('Access Set Beginning Balance\'s tab in Universal Product Information', {
                    Message: 'You are not allowed to set beginning balance.',
                    OnClickOKSuccess: function () {
                        hasAccessToSBB = true;
                        Show();
                    }
                });
            } else {
                Show();
            }
        })

        var hasAccessToInventoryCount = false;
        mElement.find('#colInventoryCount').on('show.bs.collapse', function(){
            function Show(){
                ShowInventory(function(){
                    //$('#tabOrderInventoryCount').opentabmenu();
                    InventoryBalance.GetBinsBalances(mOut.Model.Data.ProductID);
                });
            }            

            if (!hasAccessToInventoryCount){
                PromptUserAccess('Access Inventory Count\'s tab in Universal Product Information', {
                    Message: 'You don\'t have access to <b>Inventory Count\'s</b> tab.',
                    OnClickOKSuccess: function () {
                        hasAccessToInventoryCount = true;
                        Show();
                    }
                });
            } else {
                Show();
            }
        })

        var hasAccessToICAD = false;
        mElement.find('#colInventoryCountApprovalDisapproval').on('show.bs.collapse', function(){
            function Show(){
                ShowInventory(function(){
                    //$('#tabOrderICApprovalDisapproval').opentabmenu();
                    InventoryBalance.GetInventoryCountForApproval(mOut.Model.Data.ProductID);
                });
            }            

            if (!hasAccessToICAD){
                PromptUserAccess('Access Inventory Count Approval and Disapproval\'s tab in Universal Product Information', {
                    Message: 'You don\'t have access to <b>Inventory Count Approval and Disapproval\'s</b> tab.',
                    OnClickOKSuccess: function () {
                        hasAccessToICAD = true;
                        Show();
                    }
                });
            } else {
                Show();
            }
        })

        var hasAccessToICADH = false;
        mElement.find('#colInventoryCountApprovalDisapprovalHistory').on('show.bs.collapse', function(){
            function Show(){
                ShowInventory(function(){
                    //$('#tabOrderICApprovalDisapprovalHistory').opentabmenu();
                    InventoryBalance.GetInventoryCountForApprovalHistory(mOut.Model.Data.ProductID);
                });
            }            

            if (!hasAccessToICADH){
                PromptUserAccess('Access Inventory Count Approval and Disapproval History\'s tab in Universal Product Information', {
                    Message: 'You don\'t have access to <b>Inventory Count Approval and Disapproval History\'s</b> tab.',
                    OnClickOKSuccess: function () {
                        hasAccessToICADH = true;
                        Show();
                    }
                });
            } else {
                Show();
            }
        })

        var hasAccessToCWIB = false;
        mElement.find('[name="btnUPIInventoryBalance"]').unbind('click').click(function () {
            if (CStr2(mOut.Model.Data.ProductID) == '') {
                NewMessageBox('Please search product first or you can scan a barcode.');
                return;
            }

            function Show(){
                ShowInventory(function(){
                    //$('#tabOrderCompanyWideIB').opentabmenu();
                    InventoryBalance.GetCompanyWideInventoryBalance(mOut.Model.Data.ProductID, mProductName, mProductNo);
                    $('#tabUPIInventoryBalance').opentabmenu();
                });
            }            

            if (!hasAccessToCWIB){
                PromptUserAccess('Access Company Wide Inventory Balance\'s tab in Universal Product Information', {
                    Message: 'You don\'t have access to <b>Company Wide Inventory Balance\'s</b> tab.',
                    OnClickOKSuccess: function () {
                        hasAccessToCWIB = true;
                        Show();
                    }
                });
            } else {
                Show();
            }
        })

        var hasAccessToBIS = false;
        mElement.find('[name="btnOrderInputBasicInfo"]').unbind('click').click(function(){
            if (!hasAccessToBIS){
                PromptUserAccess('Access Basic Sales Info\'s tab in Universal Product Information', {
                    Message: 'You don\'t have access to <b>Basic Sales Info\'s</b> tab.',
                    OnClickOKSuccess: function () {
                        hasAccessToBIS = true;
                        //$('#tabOrderInputBasicInfo').opentabmenu();
                    }
                });
            } else {
                //$('#tabOrderInputBasicInfo').opentabmenu();
            }
        })

        var hasAccessToIR = false;
        function LoadItemRequisition(){
            if (!hasAccessToIR){
                AjaxSend({
                    url: '/SecurityRole/CheckSecurityCurrentUserAcccess',
                    data: { ModuleName: 'Access Item Requisition\'s tab in Universal Product Information' },
                    success: function(e) {
                        if (e.SecurityRoleData.Rows.length > 0) {
                            hasAccessToIR = true;
                            if (mItemRequisition == null) {
                                require(['promise!/Scripts/sources/ERP/PurchaseOrder/poBySku/controller.js?v=' + assetversion,
                                    'promise!/Scripts/sources/ERP/AverageWeeklyDemand/controller.js?v=' + assetversion], function() {
                                        mItemRequisition = ERP.PurchaseOrder.poBySku.controller();
                                        mItemRequisition.ItemRequisitionMode = true;
                                        mItemRequisition.Initialize({
                                            ProductID: mOut.Model.Data.ProductID
                                        });
                                        $('#tabOrderInputItemRequisition').html('').append(mItemRequisition.element);
                                        mItemRequisition.element.find('[name="selActive"], [name="selPoFilter"], #txtPoBySkuFilter, [name="selMaxRecord"]').parent().hide();
                                        require(['promise!/Scripts/sources/ERP/PurchaseOrder/ItemRequisitionSettings/controller.js?v=' + assetversion], function() {
                                            //var pInstance = ERP.PurchaseOrder.ItemRequisitionSettings.controller();
                                            //pInstance.Initialize();
                                        });

                                        mAverageWeeklyDemand = ERP.AverageWeeklyDemand.controller();
                                        $('#tabOrderInputItemRequisition').append(mAverageWeeklyDemand.element);
                                        mAverageWeeklyDemand.element.css({
                                            'margin-top': '60px'
                                        });
                                        mAverageWeeklyDemand.Initialize({
                                            ProductID: mOut.Model.Data.ProductID
                                        });
                                    });
                                //require(['promise!/Scripts/sources/ERP/ItemRequisition/settings/controller.js']);
                            } else {
                                mItemRequisition.Initialize({
                                    ProductID: mOut.Model.Data.ProductID
                                });
                                mAverageWeeklyDemand.Initialize({
                                    ProductID: mOut.Model.Data.ProductID
                                });
                            }
                            //$('#tabOrderInputItemRequisition').opentabmenu();
                        }
                    }
                })
            } else {
                //$('#tabOrderInputItemRequisition').opentabmenu();
                mItemRequisition.Initialize({
                    ProductID: mOut.Model.Data.ProductID
                });
                mAverageWeeklyDemand.Initialize({
                    ProductID: mOut.Model.Data.ProductID
                });
            }
        }

        var hasAccessToPH = false;
        mElement.find('#colPriceHistory').on('show.bs.collapse', function(){
            if (!hasAccessToPH){
                PromptUserAccess('Access Price History\'s tab in Universal Product Information', {
                    Message: 'You don\'t have access to <b>Price History\'s</b> tab.',
                    OnClickOKSuccess: function () {
                        hasAccessToPH = true;
                        //$('#tabOrderInputPriceHistory').opentabmenu();
                        CreatePriceHistoryDataTable();
                    }
                });
            } else {
                //$('#tabOrderInputPriceHistory').opentabmenu();
                CreatePriceHistoryDataTable();
            }            
        })

        var hasAccessToIL = false, isFinishedLoadIL = false;
        mElement.find('[name="btnUPIInventoryLedger"]').on('click', function(){
            if (!hasAccessToIL){
                PromptUserAccess('Access Inventory Ledger\'s tab in Universal Product Information', {
                    Message: 'You don\'t have access to <b>Inventory Ledger\'s</b> tab.',
                    OnClickOKSuccess: function () {
                        hasAccessToIL = true;
                        //$('#tabOrderInputLedger').opentabmenu();

                        sessionStorage.IsFromInventoryBalance = true;
                        sessionStorage.InventoryBalanceProductID = mOut.Model.Data.ProductID;
                        sessionStorage.InventoryBalanceProductName = mProductName;

                        //if (mInventoryLedgerLoaded) {
                        //    InventoryLedger.CheckSource();
                        //    return;
                        //}
                    
                        //mInventoryLedgerLoaded = true;

                        if (isFinishedLoadIL)
                            InventoryLedger.CheckSource();
                        else 
                            AjaxSend({
                                ExactUrl: '/InventoryLedger?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                type: 'GET',
                                dataType: 'text',
                                success: function (vData) {
                                    sessionStorage["Inventory"] = "ledger";
                                    mElement.find('#tabOrderInputLedger')
                                        .empty()
                                        .append(vData)
                                        .find('#divAddLibrary').hide();

                                    if (!proj.IsBootstrapDesign()){
                                        mElement.find('#tabOrderInputLedger').trigger('create');
                                    }

                                    isFinishedLoadIL = true;
                                    if (isFinishedLoadIL) {
                                        var pInterval = setInterval(function() {
                                            if (window.InventoryLedger != null) {
                                                InventoryLedger.HideSearch();
                                                InventoryLedger.CheckSource();
                                                clearInterval(pInterval);
                                            }
                                        }, 100);
                                        
                                    }
                                }
                            });
                    }
                });
            } else {
                //$('#tabOrderInputLedger').opentabmenu();
                sessionStorage.InventoryBalanceProductID = mOut.Model.Data.ProductID;
                sessionStorage.InventoryBalanceProductName = mProductName;
                InventoryLedger.CheckSource();
            }            
        })

        var hasAccessToPM = false;
        mElement.find('[name="btnOrderInputPriceMatrix"]').unbind('click').click(function(){
            if (!hasAccessToPM){
                PromptUserAccess('Access Price Matrix\'s tab in Universal Product Information', {
                    Message: 'You don\'t have access to <b>Price Matrix\'s</b> tab.',
                    OnClickOKSuccess: function () {
                        hasAccessToPM = true;
                        //$('#tabOrderInputPriceMatrix').opentabmenu();
                        CreatePriceMatrixDataTable();
                    }
                });
            } else {
                //$('#tabOrderInputPriceMatrix').opentabmenu();
                CreatePriceMatrixDataTable();
            }            
        })

        var hasAccessToPOH = false;
        mElement.find('#colPOHistory').on('show.bs.collapse', function(){
            if (!hasAccessToPOH){
                PromptUserAccess('Access PO History\'s tab in Universal Product Information', {
                    Message: 'You don\'t have access to <b>PO History\'s</b> tab.',
                    OnClickOKSuccess: function () {
                        hasAccessToPOH = true;
                        //$('#tabOrderInputPurchaseOrderHistory').opentabmenu();
                        CreatePurchaseOrderHistoryDataTable();
                    }
                });
            } else {
                //$('#tabOrderInputPurchaseOrderHistory').opentabmenu();
                CreatePurchaseOrderHistoryDataTable();
            }
        })

        var hasAccessToSIT = false;
        mElement.find('[name="btnOrderInputStocksInTransit"]').unbind('click').click(function(){
            if (!hasAccessToSIT){
                PromptUserAccess('Access Stock In Transit\'s tab in Universal Product Information', {
                    Message: 'You don\'t have access to <b>Stock In Transit\'s</b> tab.',
                    OnClickOKSuccess: function () {
                        hasAccessToSIT = true;
                        //$('#tabOrderInputStocksInTransit').opentabmenu();
                        CreateStocksInTransitDataTable();
                        $('#popOrderStockInTransit').modal('show');
                    }
                });
            } else {
                //$('#tabOrderInputStocksInTransit').opentabmenu();
                CreateStocksInTransitDataTable();
                $('#popOrderStockInTransit').modal('show');
            }            
        })

        var hasAccessToSR = false;
        mElement.find('#colSalesRepresentative').on('show.bs.collapse', function(){
            if (!hasAccessToSR){
                PromptUserAccess('Access Sales Representatives\' tab in Universal Product Information', {
                    Message: 'You don\'t have access to <b>Sales Representatives\'</b> tab.',
                    OnClickOKSuccess: function () {
                        hasAccessToSR = true;
                        //$('#tabOrderInputSalesRepresentative').opentabmenu();
                    }
                });
            } else {
                //$('#tabOrderInputSalesRepresentative').opentabmenu();
            }
        })

        var hasAccessToVP = false;
        mElement.find('#colVendorPriceMatrix').on('show.bs.collapse', function(){
            if (!hasAccessToVP){
                PromptUserAccess('Access Vendor Prices\' tab in Universal Product Information', {
                    Message: 'You don\'t have access to <b>Vendor Prices\'</b> tab.',
                    OnClickOKSuccess: function () {
                        hasAccessToVP = true;
                        //$('#tabOrderInputVendorPrices').opentabmenu();
                    }
                });
            } else {
                //$('#tabOrderInputVendorPrices').opentabmenu();
            }
        })

        function DisplaySalesRep() {
            var list = mElement.find('[name="listOrderInputSalesRep"]');
            list.empty();

            for(var i = 0; i < arrSalesRep.length; i++){
                arrSalesRep[i].Index = i;
                list.append($('<li class="list-group-item d-flex">')
                        .append($('<p class="p-0 m-0 flex-grow-1">').text(arrSalesRep[i].PeopleName))
                        .append($('<button data-role="none" class="btn btn-danger"><span class="bi-trash"></span></button>')
                            .data('data', arrSalesRep[i])
                            .click(function(){
                                var dis = $(this), data = dis.data('data');

                                NewMessageBox('Are you sure you want to remove <b>' + data.PeopleName + '</b> as Sales Representative for this product?', 'Confirmation Dialog', [
                                    {
                                        Text: 'Yes', Click: function(){
                                            dis.parent().remove();
                                            arrSalesRep.splice(data.Index, 1);
                                        }
                                    }, { Text: 'No' }
                                ])
                            })
                        )
                    )
            }
        }

        function PriceUpdateRequest(vData) {
            require(['promise!/Scripts/sources/ERP/OrderProcessing/priceUpdateRequest/controller.js?v=' + assetversion], function() {
                ERP.OrderProcessing.priceUpdateRequest.controller.popup(vData);
            });
        }

        mOut.Init();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});