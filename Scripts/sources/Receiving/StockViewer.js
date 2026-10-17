require(['common'
    , GetPathFormat('/Scripts3/Browser/LibraryOrganizationEmployeeFOWST.js?crc=')
    ], function () {

        $(function () {

            var mData; // from 'Stock/GetInfoPos' or 'Stock/GetInfo'
            var mTxtSalesRep; // the textbox opening the people browser
            var mHasError = 1;
            //        collapsibleStatic();

            setTimeout(function () {
                //            $.mobile.activePage.bind('pageshow', PageShow);
            }, 1000);
            $.mobile.activePage.bind('pageshow', PageShow);

            function PageShow() {
                if (ValidPrevPage(arguments)) {
                    $('#divUnit').hide();
                    if (proj.PosMode)
                        $('#divUnit').find('[data-role=collapsible_clone]').remove();
                    else
                        $('#divUnit').find('[data-role=collapsible_clone_pos]').remove();
                    GetInfo();
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

                $('[name=divUnit]').remove();
                if (proj.ProductID != undefined)
                    pStockID = proj.ProductID;
                else {
                    pParams = GetUrlParameters();
                    pStockID = pParams.id;
                };

                if (proj.PosMode) {
                    pUrl = 'Stock/GetInfoPos';
                } else {
                    pUrl = 'Stock/GetInfo';
                };
                mHasError = 1; // assume that there's an error

                AjaxSend({
                    url: pUrl,
                    data: {
                        ProductID: pStockID,
                        CustomerID: sessionStorage.CustomerOrganizationID,
                        OrderID: sessionStorage.CartOrderID,
                        NewCartMode: localStorage.NewCartMode
                    },
                    success: function (vData) {
                        if (vData.Error != undefined) {
                            MessageBox(vData.Error, 'Error');
                        } else {
                            mHasError = 0;
                            LoadData(vData);
                        };
                    }
                });

                function LoadData(vData) {

                    mData = vData;
                    $('#spanStockNumber').text(vData.ProductNo);
                    $('#spanStockDescription').text(vData.ProductName);
                    if (vData.Warning) {
                        $('#divUpdateCart').hide();
                        $('#divReportNoPrice').show();
                        // MessageBox(vData.Warning); // no message as of June 29
                        return;
                    };
                    $('#divUpdateCart').show();
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
                        var pDivMore;

                        pLastItem = pTemplate;
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            pDR = vRows[p];
                            pItem = pTemplate.clone()
                            .attr('id', '')
                            .attr('name', 'divUnit');
                            if (proj.PosMode)
                                pDivMore = pItem.find('[data-role=collapsible_clone_pos]').collapsible();
                            else
                                pDivMore = pItem.find('[data-role=collapsible_clone]').collapsible();
                            pOrderedQty = '';
                            if (IsNumeric(pDR.OrderedQty))
                                pOrderedQty = $.format.number(CDec2(pDR.OrderedQty), '#,##0');
                            else
                                pDivMore.hide();
                            pTxtQty = pItem.find('[name=txtQtyTemplate]')
                            .attr('name', 'txtQty')
                            .data('UnitQty', pDR.Quantity)
                            .data('Price', pDR.DiscountedPrice)
                            .data('divMore', pDivMore)
                            .val(pOrderedQty)
                            .bind('textchange', TxtQtyChanged);
                            if ((p == 0) && (!IsNumeric(pItem.find('[name=txtQty]').text())))
                                pItem.find('[name=txtQty]').text('1');
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

                            pLastItem.after(pItem);
                            pItem.show();
                            pLastItem = pItem;

                            if (proj.PosMode) {
                                // Price Level drop down
                                var pSelect = $('<select>');
                                pDivMore.find('[name=content]').prepend(pSelect);
                                var pCbItem;
                                var pRows = vData.data.Rows;

                                pSelect.html('');
                                for (var p1 = 0; p1 <= pRows.length - 1; p1++) {
                                    if (pRows[p1].Quantity == pDR.Quantity) {
                                        pCbItem = new Option(pRows[p1].PriceLevel + ' ' + $.format.number(CDec2(pRows[p1].DiscountedPrice), '#,##0.00'), pRows[p1].PriceLevelID);
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

                                    pTxtQty = pItem.find('[name=txtQty]');
                                    for (var p = 0; p <= mData.data.Rows.length - 1; p++) {
                                        pDR = mData.data.Rows[p];
                                        if ((pDR.Quantity == pTxtQty.data('UnitQty'))
                                        && (pDR.PriceLevelID == pThis.val())) {

                                            pTxtQty.data('UnitQty', pDR.Quantity)
                                            .data('Price', pDR.DiscountedPrice);
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
                                });

                                // Sales Reps
                                var pTxtSalesRep = pDivMore.find('[name=txtSalesRep]');
                                pTxtQty.data('txtSalesRep', pTxtSalesRep);
                                pTxtSalesRep.data('txtQty', pTxtQty);
                                pTxtSalesRep.keydown(function (e) {
                                    if (e.which == 13) {
                                        mTxtSalesRep = $(this);
                                        var pTxtQty = mTxtSalesRep.data('txtQty');
                                        //Browser.LibraryOrganizationEmployeeFOWST.InputData.EmployeesOnly = 1;
                                        Browser.LibraryOrganizationEmployeeFOWST.Click = function (e) {
                                            var pSalesRep = pTxtQty.data('SalesRep') || [];

                                            // delete duplicate
                                            for (var pIndex = 0; pIndex <= pSalesRep.length - 1; pIndex++)
                                                if (pSalesRep[pIndex].PeopleID == e.ID) {
                                                    MessageBox(e.PeopleName + ' is already in the list.', 'Duplicate');
                                                    return;
                                                };

                                            pSalesRep.push({
                                                PeopleID: e.ID,
                                                PeopleName: e.PeopleName
                                            });
                                            pTxtQty.data('SalesRep', pSalesRep);
                                            mTxtSalesRep.val('');
                                            pTxtSalesRep.data('lv').SetData(pSalesRep);
                                        };
                                        Browser.LibraryOrganizationEmployeeFOWST.Show(mTxtSalesRep.val());
                                    };
                                }); // pTxtSalesRep.keydown(function(e) {

                                var pLV = new CreateListView({
                                    DataFilter: false
                                }); // the "new" keyword is important to prevent conflict with other instances of CreateListView
                                pLV.Element.data('txtSalesRep', pTxtSalesRep);
                                pLV.Element.find('ul')
                                .attr('data-split-icon', 'minus');
                                pLV.Element.insertAfter(pDivMore.find('[name=divSalesRep]'));
                                pLV.OnItem = function (vDataRow, vItem, e) {
                                    vItem.append($('<a>')
                                    .text(vDataRow.PeopleName)
                                    .attr('href', '#')
                                    .click(function (e) {
                                    })
                                );
                                    vItem.append($('<a>')
                                    .text('remove')
                                    .data('data', vDataRow)
                                    .data('li', vItem)
                                    .data('lv', pLV)
                                    .attr('href', '#')
                                    .click(function (e) {
                                        var pLV = $(this).data('lv');
                                        var pData = pLV.GetData();
                                        var pPeopleID = $(this).data('data').PeopleID;

                                        for (var pIndex = 0; pIndex <= pData.length - 1; pIndex++) {
                                            if (pData[pIndex].PeopleID == pPeopleID) {
                                                pData.splice(pIndex, 1);
                                                pIndex--;
                                            };
                                        };
                                        $(this).data('lv').Element.data('txtSalesRep').data('txtQty').data('SalesRep', pData);
                                        pLV.SetData(pData);
                                    })
                                );
                                }; // pLV.OnItem = function(vDataRow, vItem, e) {

                                if (mData.SalesReps != undefined) {
                                    var pSalesReps;
                                    pSalesReps = [];
                                    for (var pSalesRepIndex = 0; pSalesRepIndex <= mData.SalesReps.Rows.length - 1; pSalesRepIndex++) {
                                        pSalesReps.push({
                                            PeopleID: mData.SalesReps.Rows[pSalesRepIndex].PeopleID,
                                            PeopleName: mData.SalesReps.Rows[pSalesRepIndex].PeopleName
                                        });
                                    }; // for (var pSalesRepIndex = 0; pSalesRepIndex <= mData.SalesReps.length - 1; pSalesRepIndex++) {

                                    pLV.SetData(pSalesReps);
                                    pTxtQty.data('SalesRep', pSalesReps);
                                }; // if (mData.SalesReps != undefined) {
                                pTxtSalesRep.data('lv', pLV);

                            } else { // if (proj.PosMode) 
                                pItem.find('[name=spanOriginalPrice]').text($.format.number(CDec2(pDR.DiscountedPrice), '#,##0.00'));
                                pTxtAdjustPrice = pItem.find('[name=txtNewPriceTemplate]')
                                .attr('name', 'txtNewPrice')
                                .data('UnitQty', pDR.Quantity)
                                .bind('textchange', DisplaySubtotal);
                                pTxtQty.data('txtAdjustPrice', pTxtAdjustPrice);

                                if (CDec2(pDR.RequestedUnitPrice) > 0)
                                    pItem.find('[name=txtNewPrice]').val($.format.number(CDec2(pDR.RequestedUnitPrice), '#,##0.00'));

                                if (IsNumeric(pDR.CustomerLastInvoicePrice)) {
                                    pItem.find('[name=spanLastPrice]').text($.format.number(CDec2(pDR.CustomerLastInvoicePrice), '#,##0.00'));
                                    pItem.find('[name=spanLastPriceDate]').text($.format.date(new Date(pDR.CustomerLastInvoiceDate), 'M/d/yyyy'));
                                };
                                pItem.find('[name=butValidateAdjustedPrice]')
                                .data('UnitQty', pDR.Quantity)
                                .data('StockID', proj.ProductID)
                                .click(function () {
                                    var pTxtNewPrice = $(this).closest('[name=divPriceAdjustments]')
                                        .find('[name=txtNewPrice]');

                                    if (CDec2(pTxtNewPrice.val()) <= 0) {
                                        pTxtNewPrice.focus();
                                        MessageBox('Adjust Price To must be a whole number greater than 0.');
                                        return;
                                    };
                                    Submit(true);
                                });
                            }; // if (proj.PosMode) {
                            pItem.show();
                        }; // for (var p = 0; p <= vRows.length - 1; p++) {
                        DisplaySubtotal();
                    })(vData.UnitMeasures.Rows); // LoadUnitMeasures
                }; // LoadData
            }; // GetInfo

            $('#butUpdateCart').click(function (e) {
                alert('alert me');
                //                e.preventDefault();
                //                if (Verify())
                //                    Submit();

                                function Verify() {

                //                    var pResult = true;

                //                    if (mHasError) {
                //                        MessageBox('There are problems with this product');
                //                        return false;
                //                    };

                //                    if ((!proj.PosMode) && (sessionStorage.CustomerOrganizationID == undefined)) {
                //                        MessageBox('No selected customer');
                //                        return false;
                //                    };

                                    $('[name=txtQty]').each(function (vItem, vIndex) {
                                        var pThis = $(this);
                                        var pVal = pThis.val();
                                        if ((CStr2(pVal).trim().length > 0) && (!IsNumeric(pVal))) {
                                            MessageBox('Quantity must be a number greater than 0.', 'Invalid Quantity')
                                        .on('pagehide', function () {
                                            pThis.focus();
                                        });
                                            pResult = false;
                                            return false;
                                        }; // if (!IsNumeric(pThis.val())) {
                                        if (CDec2(pVal) < 0) {
                                            MessageBox('Quantity must be a number greater than 0.', 'Invalid Quantity')
                                        .on('pagehide', function () {
                                            pThis.focus();
                                        });
                                            pResult = false;
                                            return false;
                                        };
                                    }); // $('[name=txtQty]').each(function(vItem, vIndex) {

                //                    if (pResult) {
                //                        $('[name=txtNewPrice]').each(function (vItem, vIndex) {
                //                            var pThis = $(this);
                //                            var pVal = pThis.val();
                //                            if ((CStr2(pVal).trim().length > 0) && (!IsNumeric(pVal))) {
                //                                MessageBox('Adjust Price To must be a valid number. You entered ' + pVal, 'Invalid Adjust Price To')
                //                            .on('pagehide', function () {
                //                                pThis.focus();
                //                            });
                //                                pResult = false;
                //                                return false;
                //                            }; // if (!IsNumeric(pThis.val())) {
                //                        }); // $('[name=txtNewPrice]').each(function(vItem, vIndex) {
                //                    }; // if (pResult) {

                                    return pResult;

                                }; // function Verify() {

            }); // $('#butUpdateCart').click(function(e) {

            function Submit(vVerifyOnly) {
                // PARAMS:
                //  vVerifyOnly: boolean optional, verify price request

                var pParams;
                var pData = {};
                var p = 0;
                var pStockID;
                var pUrl;

                if (proj.ProductID != undefined)
                    pStockID = proj.ProductID;
                else {
                    pParams = GetUrlParameters();
                    pStockID = pParams.id;
                };

                pData.StockID = pStockID;
                pData.CustomerID = sessionStorage.CustomerOrganizationID;
                pData.OrderID = sessionStorage.CartOrderID;
                pData.NewCartMode = localStorage.NewCartMode;
                pData.FowstID = sessionStorage.FowstID;
                pData.Stocks = [];

                $('[name=txtQty]').each(function (vItem, vIndex) {
                    var pThis = $(this);
                    var pTxtAdjustPrice;
                    var pItem;
                    var pSalesRep;
                    var pSalesRepIDs;

                    if (IsNumeric(pThis.val())) {
                        pData.Stocks[p] = {
                            Qty: pThis.val(),
                            UnitQty: pThis.data('UnitQty')
                        };
                        if (proj.PosMode) {
                            pItem = pThis.closest('[name=divUnit]');
                            pData.Stocks[p].PriceLevelID = pItem.find('[name=cbPrice]').val();
                        } else {
                            pTxtAdjustPrice = pThis.data('txtAdjustPrice');
                            if (IsNumeric(pTxtAdjustPrice.val()))
                                pData.Stocks[p].RequestedPrice = pTxtAdjustPrice.val();
                        };

                        pSalesRep = pThis.data('SalesRep');
                        if ((pSalesRep != undefined) && (pSalesRep.length > 0)) {
                            pSalesRepIDs = [];
                            for (var pSalesRepIndex = 0; pSalesRepIndex <= pSalesRep.length - 1; pSalesRepIndex++) {
                                if (pSalesRep[pSalesRepIndex].PeopleID != null)
                                    pSalesRepIDs.push(pSalesRep[pSalesRepIndex].PeopleID);
                            }; // for (var pSalesRepIndex = 0; pSalesRepIndex <= pSalesRep.length - 1; pSalesRepIndex++) {
                            pData.Stocks[p].SalesRep = pSalesRepIDs;
                        }; // if ((pSalesRep != undefined) && (pSalesRep.length > 0)) {
                        p++;
                    }; // if (!IsNumeric(pThis.val())) {
                }); // $('[name=txtQty]').each(function(vItem, vIndex) {

                window.location = GetWindowPathFormat('Receiving') + '#pageCart';

//                if (vVerifyOnly) {
//                    pUrl = 'Stock/ValidateAdjustedPrice';
//                } else if (proj.PosMode) {
//                    pUrl = 'Stock/UpdatePos';
//                } else {
//                    pUrl = 'Receiving/Update';
//                };

//                AjaxSend({
//                    url: pUrl,
//                    data: pData,
//                    success: function (vData) {
//                        var pMessage;
//                        var pDivDetails;
//                        var pLnkDetails;

//                        if (vData == undefined) return;
//                        if (vData.Error != undefined) {
//                            if (CDec2(vData.ReloadBrowser) != 0)
//                                MessageBox(vData.Error, 'Error', null, {
//                                    OnClose: function () {
//                                        location.reload();
//                                    }
//                                });
//                            else
//                                MessageBox(vData.Error);
//                            return;
//                        };
//                        if (vData.Result != 'ok') {
//                            pMessage = $('<div>').html('The price you have requested is too low. Please do one of the following:<br />'
//                            + '<ol>'
//                            + '<li>Take the original selling price</li>'
//                            + '<li>Increase the requested price</li>'
//                            + '<li>Try a price request again after adding some more items to your cart.</li>'
//                            + '</ol>'
//                            + '• Price requests are approved/ disapproved based on transaction profitability. The more items you put into the cart, the more likely that your price request will be approved.<br />'
//                        );
//                            pLnkDetails = $('<a>')
//                            .attr('href', '#')
//                            .text('Details')
//                            .click(function (e) {
//                                e.preventDefault();
//                                pLnkDetails.hide();
//                                pDivDetails.show();
//                            });
//                            pMessage.append(pLnkDetails);
//                            pDivDetails = $('<div>').html('<h3>Details</h3>'
//                            + '<h5>Decision Factors for Denying Price Request:</h5>'
//                            + '<ol>'
//                            + '<li><b>Is this line item’s Gross Profit Margin (GPM) within acceptable limits?: No'
//                            + '</li>'
//                            + '</ol>' + HtmlEncode(vData.Warning)
//                        ).hide();
//                            pMessage.append(pDivDetails);
//                        }; // if (vData.Result != 'ok') {
//                        if (vVerifyOnly) {
//                            if (vData.Result == 'ok') {
//                                MessageBox('Price adjustment request is acceptable.');
//                            } else {
//                                MessageBox(pMessage, 'Price Request Denied', null, {
//                                    EncodeHtml: false
//                                });
//                            };
//                        } else {
//                            if ((vData.Result == 'ok')
//                            || (vData.Warning != undefined)) {

//                                function Done() {
//                                    sessionStorage.StockInfo = pData;
//                                    sessionStorage.CartOrderID = vData.OrderID;
//                                    localStorage.removeItem('NewCartMode');
//                                    if ($.isFunction(proj.ShowCart))
//                                        proj.ShowCart();
//                                    else {
//                                        MessageBox('Cart was successfully updated', 'Success');
//                                        window.location = GetWindowPathFormat('Receiving') + '#pageCart';
//                                    };
//                                }; // function Done() {

//                                if (vData.Warning != undefined)
//                                    MessageBox(pMessage, 'Warning', null, {
//                                        OnClose: Done,
//                                        EncodeHtml: false
//                                    });
//                                else
//                                    Done();
//                            } else {
//                                MessageBox((vData.Error || 'An error occured while updating cart'), 'Failed',
//                                [
//                                    {
//                                        Text: 'Retry',
//                                        Click: function () {
//                                            if (Verify())
//                                                Submit();
//                                        }
//                                    },
//                                    {
//                                        Text: 'Close this message'
//                                    }
//                                ]
//                            ); // MessageBox((vData.Error || 'An error
//                            }; // if (vData.Result == 'ok') {
//                        }; // if (vVerifyOnly) {
//                    }
//                });

            }; // Submit()

            function DisplaySubtotal() {

                var pTotal = 0;

                $('[name=txtQty]').each(function (vItem, vIndex) {
                    var pUnitPrice;
                    var pThis = $(this);
                    if (IsNumeric(pThis.val())) {
                        if (!proj.PosMode)
                            pUnitPrice = pThis.data('txtAdjustPrice').val();
                        if (CDec2(pUnitPrice) <= 0)
                            pUnitPrice = pThis.data('Price');
                        pTotal += pThis.val() * pUnitPrice;
                    }; // if (!IsNumeric(pThis.val())) {
                }); // $('[name=txtQty]').each(function(vItem, vIndex) {

                if (pTotal > proj.MaxNumber)
                    $('#spanSubtotal').text('Please be realistic.');
                else
                    $('#spanSubtotal').text(AccountingFormat(pTotal));

            }; // function DisplaySubtotal() {

            function TxtQtyChanged() {

                var pThis = $(this);

                if (IsNumeric(pThis.val()))
                    pThis.data('divMore').show();
                else
                    pThis.data('divMore').hide();
                DisplaySubtotal();

            }; // function TxtQtyChanged() {

            $('#butReportNoPrice').click(function (e) {
                e.preventDefault();
                AjaxSend({
                    url: 'Stock/AlertNoPrice',
                    data: { ProductID: proj.ProductID },
                    success: function (vData) {
                        if (vData.Error)
                            MessageBox(vData.Error);
                        else if (vData.Result == 'ok') {
                            MessageBox('Alert was successfully saved.', 'Success', null, {
                                OnClose: function () {
                                    if ($.isFunction(proj.ShowCart))
                                        proj.ShowCart(1);
                                } // OnClose: function() {
                            }); // MessageBox('Alert was successfully saved', 'Success', [], {

                        }; // else if (vData.Result == 'ok') {
                    } // success: function(vData) 
                });
            }); // $('#butReportNoPrice').click(function(e) {

        }); // $(function() {

    });