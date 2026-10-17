require(['common'
    ], function () {

    $(function () {

        var mReturnsReasons;
        var mData;
        var mInvoiceCustomerID;

        //collapsibleStatic();

        //setTimeout(function() {
        //    $.mobile.activePage.bind('pageshow', PageShow);
        //},  1000);

        function PageShow() {
            if (ValidPrevPage(arguments)) {
                $('#divUnit').hide();
                GetInfo();
            };

            function ValidPrevPage(args) {

                try {
                    return args[1].prevPage.attr('data-role') != 'dialog';
                } catch (e) {
                    return true;
                };

            };
        };

        if (proj.SalesReturnStock_PageShow == null) {
            PageShow();
            proj.SalesReturnStock_PageShow = PageShow;
        }

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
            var pProductID;

            $('[name=divUnit]').remove();
            if (proj.ProductID != undefined)
                pProductID = proj.ProductID;
            else {
                pParams = GetUrlParameters();
                pProductID = pParams.id;
            };

            pParams = {
                ProductID: pProductID,
                InvoiceID: proj.SalesReturnInvoiceID,
                MemoID: proj.SalesReturnMemoID,
                GetReturnsReasons: (mReturnsReasons == undefined ? 1 : 0)
            };
            mInvoiceCustomerID = null;
            if (proj.SalesReturnFromOffline) {
                var pDS = Business.TransSalesReturn.GetContent(pParams);
                LoadData(pDS);
            } else {
                var pUrl = 'SalesReturns/GetInfo';
                AjaxSend({
                    url: pUrl,
                    data: pParams,
                    success: function(vData) {
                        if (vData.Error != undefined)
                            NewMessageBox(vData.Error, 'Error');
                        else {
                            if (proj.IsStockLoadedFromServer) {
                                pUrl = MergeUrlParameters({}, pUrl, proj.RemoteServer);
                                AjaxSend({
                                    url: pUrl,
                                    data: pParams,
                                    success: function(vData2) {
                                        if (vData2.Error != undefined)
                                            MessageBox(vData2.Error, 'Error');
                                        else {
                                            mInvoiceCustomerID = vData2.CustomerID;
                                            for (var pRemoteIndex = 0; pRemoteIndex <= vData2.Units.Rows.length - 1; pRemoteIndex++) {
                                                var pDrRemote = vData2.Units.Rows[pRemoteIndex];
                                                for (var pLocalIndex = 0; pLocalIndex <= vData.Units.Rows.length - 1; pLocalIndex++) {
                                                    var pDrLocal = vData.Units.Rows[pLocalIndex];
                                                    if (CDec2(pDrLocal.ActualQuantity) != CDec2(pDrRemote.ActualQuantity))
                                                        continue;
                                                    pDrLocal.QtyInvoiced = CDec2(pDrRemote.QtyInvoiced);
                                                    pDrLocal.UnitPrice = CDec2(pDrRemote.UnitPrice);
                                                    pDrLocal.QtyReturnedOthers = Math.max(CDec2(pDrRemote.QtyReturnedOthers), CDec2(pDrLocal.QtyReturnedOthers));
                                                    pDrLocal.QtyReturnedOwn = Math.max(CDec2(pDrRemote.QtyReturnedOwn), CDec2(pDrLocal.QtyReturnedOwn));
                                                    pDrLocal.QtyReturnedOwnBad = Math.max(CDec2(pDrRemote.QtyReturnedOwnBad), CDec2(pDrLocal.QtyReturnedOwnBad));
                                                }
                                            }
                                            LoadData(vData);
                                        }
                                    }
                                }); // END AjaxSend({
                            } else
                                LoadData(vData);
                        }
                    }
                });
            }

            function LoadData(vData) {

                mData = vData;
                if ((vData.ReturnsReasons != undefined)
                    || (mReturnsReasons == null)) {

                    if (vData.ReturnsReasons == null) 
                        vData.ReturnsReasons = [
                            'Change of mind',
                            'Excess quantity ordered',
                            'Goods no longer needed', 
                            'Quality of goods does not meet expectations',
                            'Wrong goods ordered',
                            'Defective goods',
                            'Replace with manual invoice/receipt'
                        ];
                    
                    mReturnsReasons = vData.ReturnsReasons;
                }

                $('#spanStockNumber').text(vData.Stocks.Rows[0].ProductNo);
                $('#spanStockDescription').text(vData.Stocks.Rows[0].ProductName);

                (function LoadUnitMeasures(vRows) {

                    var pTemplate = $('#divUnit');
                    var pDR;
                    var pItem;
                    var pLastItem;
                    var pReturnedQty;
                    var pSavingsPerBase;
                    var pTxt;
                    var pSelect;
                    var pCbItem;

                    pLastItem = pTemplate;
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pDR = vRows[p];
                        pItem = pTemplate.clone()
                            .attr('id', '')
                            .attr('name', 'divUnit');

                        pReturnedQty = '';
                        if (CDec2(pDR.QtyReturnedOwn) > 0)
                            pReturnedQty = $.format.number(CDec2(pDR.QtyReturnedOwn), proj.QuantityFormat);
                        pTxt = pItem.find('[name=txtQtyTemplate]')
                            .data('data', pDR)
                            .attr('name', 'txtQty')
                            .data('UnitQty', pDR.ActualQuantity)
                            .data('UnitMeasure', pDR.UnitMeasure)
                            .data('ActualQuantity', pDR.ActualQuantity)
                            .data('UnitPrice', pDR.DiscountedUnitPrice || pDR.UnitPrice)
                            .data('NetDue', pDR.NetDue)
                            .data('BracketNumber', pDR.BracketNumber)
                            .val(pReturnedQty)
                            .bind('textchange' ,DisplaySubtotal)
                            .attr('step', proj.QuantityStep);
                        pItem.find('[name=spanUnit]').text(pDR.UnitMeasure);
                        pItem.find('[name=spanPrice]').text($.format.number(CDec2(pDR.DiscountedUnitPrice || pDR.UnitPrice), '#,##0.00'));
                        pItem.find('[name=spanUnitQtyLabel]').text(pDR.BaseUnit + ' per ' + pDR.UnitMeasure);
                        pItem.find('[name=spanBaseUnit]').text(pDR.BaseUnit);
                        pItem.find('[name=spanUnitQty]').text(pDR.Quantity);
                        pItem.find('[name=spanPricePerBase]').text($.format.number(CDec2(pDR.DiscountedPricePerUnit), '#,##0.00'));
                        pSavingsPerBase = CDec2(pDR.OriginalPrice) / CDec2(pDR.Quantity);
                        pSavingsPerBase -= CDec2(pDR.DiscountedPricePerUnit);
                        pItem.find('[name=spanSavingsPerBase]').text($.format.number(pSavingsPerBase, '#,##0.00'));
                        pItem.find('[name=spanTotalSavings]').text($.format.number(CDec2(pDR.OriginalPrice) - CDec2(pDR.DiscountedPrice), '#,##0.00'));
                        pItem.find('[name=spanQtyInvoiced]').text($.format.number(CDec2(pDR.QtyInvoiced), proj.QuantityFormat));
                        pItem.find('[name=spanQtyReturned]').text($.format.number(CDec2(pDR.QtyReturnedOthers), proj.QuantityFormat));
                        pItem.find('[name=spanQtyReturnable]').text($.format.number(CDec2(pDR.QtyInvoiced) - CDec2(pDR.QtyReturnedOthers), proj.QuantityFormat));

                        // reason 
                        pSelect = $('<select data-role="none" class="form-select">');
                        pItem.find('[name=divReason]').append(pSelect);
                        pSelect.html('');
                        pCbItem = new Option('Return Reason', 0);
                        pSelect.append(pCbItem);
                        for (var p1 = 0; p1 <= mReturnsReasons.length - 1; p1++) {
                            pCbItem = new Option(mReturnsReasons[p1]);
                            pSelect.append(pCbItem);
                        };
                        pSelect.val(pDR.ReturnReason).attr('selected');
                        pSelect.attr('name', 'cbReturnReason');
                        //pSelect.selectmenu();
                        //pSelect.selectmenu('refresh', true);
                        pTxt.data('cbReturnReason', pSelect);
                        // END: reason

                        pLastItem.after(pItem);
                        pLastItem = pItem;
                        pItem.show();
                    }; // for (var p = 0; p <= vRows.length - 1; p++) {
                    DisplaySubtotal();
                })(vData.Units.Rows); // LoadUnitMeasures
            }; // LoadData
        }; // GetInfo

        $('#butUpdateReturns').click(function (e) {

            if (Verify())
                Submit();

            function Verify() {

                var pResult = true;

                $('[name=txtQty]').each(function (vItem, vIndex) {
                    var pThis = $(this);
                    var pDR = pThis.data('data');
                    var pVal = pThis.val();
                    if (CStr2(pVal).trim().length > 0) {
                        if (!IsNumeric(pVal)) {
                            NewMessageBox('Quantity must be a valid number. You entered ' + pVal, 'Invalid quantity');
                            //.on('pagehide', function () {
                            //    pThis.focus();
                            //});
                            pResult = false;
                            return false;
                        }
                        if ((!mData.AllowDecimal) 
                            && (!IsInteger(pVal))) {

                            NewMessageBox('Quantity must be a valid whole number. You entered ' + pVal, 'Invalid quantity');
                            //.on('pagehide', function () {
                            //    pThis.focus();
                            //});
                            pResult = false;
                            return false;
                        }; // if (!IsNumeric(pVal)) {

                        if (CDec2(pVal) < 0) {
                            NewMessageBox('Quantity must be greater than or equal to zero. You entered ' + pVal, 'Invalid quantity');
                            //.on('pagehide', function () {
                            //    pThis.focus();
                            //});
                            pResult = false;
                            return false;
                        }

                        if (CDec2(pDR.QtyInvoiced) < CDec2(pDR.QtyReturnedOthers) + CDec2(pVal)) {
                            NewMessageBox('Returned quantity must not exceed invoice quantity. You entered ' + pVal, 'Over quantity');
                            //.on('pagehide', function () {
                            //    pThis.focus();
                            //});
                            pResult = false;
                            return false;
                        }

                        if (pThis.data('cbReturnReason').val() == 0) {
                            NewMessageBox('Please select a return reason');
                            pResult = false;
                            return false;
                        };
                    }; // if (CStr2(pVal).trim().length > 0) {
                }); // $('[name=txtQty]').each(function(vItem, vIndex) {

                return pResult;

            }; // function Verify() {

            function Submit() {

                var pParams;
                var pData = {};
                var p = 0;
                var pStockID;

                if (proj.ProductID != undefined)
                    pStockID = proj.ProductID;
                else {
                    pParams = GetUrlParameters();
                    pStockID = pParams.id;
                };

                pData.StockID = pStockID;
                pData.CustomerID = mData.CustomerID || sessionStorage.CustomerOrganizationID || mInvoiceCustomerID;
                pData.InvoiceID = proj.SalesReturnInvoiceID;
                pData.Stocks = [];
                pData.MemoID = proj.SalesReturnMemoID;
                
                $('[name=txtQty]').each(function (vItem, vIndex) {
                    var pThis = $(this);
                    if (IsNumeric(pThis.val())) {
                        var pTaxAmount = 0; 
                        var pVATExempt = 0; 
                        if ((CDec2(pThis.val()) > 0) 
                            && (CDec2(pThis.data('data').QtyInvoiced) > 0)) {

                            pTaxAmount = pThis.data('data').TaxAmount;
                            pTaxAmount /= CDec2(pThis.data('data').QtyInvoiced);
                            pTaxAmount *= CDec2(pThis.val());

                            if (CBool(pThis.data('data').HasSCDiscount)) {
                                pVATExempt = CDec2(pThis.data('data').GrossUnitPrice) / 1.12;
                                pVATExempt *= CDec2(pThis.val());
                            }
                        }

                        pData.Stocks[p] = {
                            Qty: pThis.val(),
                            //QtyBad: 0,
                            UnitQty: pThis.data('UnitQty'),
                            UnitMeasure: pThis.data('UnitMeasure'),
                            UnitPrice: pThis.data('UnitPrice'),
                            UnitDiscountAmount: pThis.data('data').UnitDiscountAmount, // dont use this. this is null. pThis.data('UnitDiscountAmount'),
                            TaxAmount: pTaxAmount, // dont use this. this is null. pThis.data('UnitDiscountAmount'),
                            VATExempt: pVATExempt, // dont use this. this is null. pThis.data('UnitDiscountAmount'),
                            VATSales: pThis.data('data').VATSales,
                            NetDue: pThis.data('NetDue'),
                            BracketNumber: pThis.data('BracketNumber'),
                            ReturnReason: pThis.data('cbReturnReason').val()
                        };

                        p++;
                    }; // if (!IsNumeric(pThis.val())) {
                }); // $('[name=txtQty]').each(function(vItem, vIndex) {

                //if (proj.SalesReturnFromOffline) {
                if (IsMobile) {
                    Business.TransSalesReturn.Update(pData);
                    if ($.isFunction(proj.ShowReturns))
                        proj.ShowReturns();
                    else {
                        window.location = '/SalesReturns#cart';
                    };
                } else 
                    AjaxSend({
                        url: 'SalesReturns/Update',
                        data: pData,
                        success: function (vData) {
                            if ((vData != undefined) 
                                && (vData.Result == 'ok')) {

    ////                            MessageBox('Returns was successfully updated', 'Success', null, {
    ////                                OnClose: function() {
                                        if ($.isFunction(proj.ShowReturns)) 
                                            proj.ShowReturns();
                                        else {
                                            window.location = '/SalesReturns#cart';
                                        };
    ////                                }
    ////                            });
                            } else if (vData.Error != undefined)
                                MessageBox(vData.Error); 
                            else 
                                MessageBox('An error occured while updating your returns'); 
                        }
                    }); // END AjaxSend({

            }; // Submit()

        }); // $('#butUpdateCart').click(function(e) {

        $('#btnStockBack').click(function () {
            $('#pageSalesReturn').pageshow();
        })

        function DisplaySubtotal() {

            var pTotal = 0;

            $('[name=txtQty]').each(function (vItem, vIndex) {
                var pThis = $(this);
                if (IsNumeric(pThis.val())) {
                    pTotal += pThis.val() * pThis.data('UnitPrice');
                }; // if (!IsNumeric(pThis.val())) {
            }); // $('[name=txtQty]').each(function(vItem, vIndex) {

            if (pTotal > proj.MaxNumber)
                $('#spanSubtotal').text('Please be realistic');
            else
                $('#spanSubtotal').text(AccountingFormat(pTotal));

        }; // function DisplaySubtotal() {

    }); // $(function() {

});