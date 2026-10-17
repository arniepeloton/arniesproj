define([
    '/Scripts3/InventoryLedger.js?v=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/InventoryBalance/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.InventoryBalance.controller', null, function () {

        var mOut = {
            ObjectID: NewID()
        };
        var mElement = $(mHtml);
        var ProductID;
        var ProductNo;
        var ProductName;
        var mOrderController;
        var mAfterSave; // function
        
        $('body').append(mElement);//.trigger('create');

        mOut.Show = async function(vParams) {
            vParams = $.extend({
                ProductID: null,
                ProductNo: null,
                ProductName: null,
                OrderController: null,
                AfterSave: function() {}
            }, vParams);

            ProductID = vParams.ProductID;
            ProductNo = vParams.ProductNo;
            ProductName = vParams.ProductName;
            mOrderController = vParams.OrderController;
            mAfterSave = vParams.AfterSave;

            $('#panTableClick').popup('close');

            $('#lblCompanyWideProduct').text(ProductName);
            $('#lblProductNo').text(ProductNo);
            $('#lblFOWSTName').text(sessionStorage.SelectedFOWSTName || sessionStorage.FowstName);

            $('#lblAvailableToCommit').text('');
            $('#lblOnHand').text('');
            $('#lblCommited').text('');
            mElement.find('[name=title]').text(ProductNo + ' - ' + ProductName);
            await GetProductBalances(true);
            await GetProductBalances(false);
            await GetAvailableToCommitProducts(true);
            await GetProductPriceList(ProductID);
            //NewMessageBoxClose();
            $('#panCompanyWideInventoryBalance').modal('show');

            sessionStorage.InventoryBalanceProductID = ProductID;
            sessionStorage.Inventory = 'ledger';
            sessionStorage.IsFromInventoryBalance = true;
            window.InventoryLedger.CheckSource();
        };

        $('#btnCompanyWideInventoryBalance').click(function () {
            $('#lblCompanyWideProduct').text(ProductName);

            GetProductBalances(false);            
        });

        async function GetProductBalances(IsByFOWST) {
            Bs.Loading('show');
            var isByFOWST = IsByFOWST;

            pData = {
                DisplayType: 'fowst',
                ProductID: ProductID
            }

            if (isByFOWST) { //if single fowst
                // pData.FOWSTID = sessionStorage.SelectedFOWST;
                pData.FOWSTID = sessionStorage.SelectedFOWST || sessionStorage.FowstID; // by Arvin
            
                var pResult = await AjaxSend({
                    url: 'InventoryLedger/GetReport',
                    data: pData,
                    success: function (vData) {
                        Bs.Loading('hide');
                        if (isByFOWST) {
                            var AvailableToCommit = CDec2($('#lblOnHand').text()) - CDec2($('#lblCommited').text());;
                            $('#lblOnHand').text(AccountingFormat(vData.data.Rows[0].Balance));
                            $('#lblFOWSTName').text(vData.data.Rows[0].Fowst);
                            if ($('#lblCommited').text() != '') 
                                $('#lblAvailableToCommit').text(AccountingFormat((AvailableToCommit >= 0 ? AvailableToCommit : '0')));
                            else
                                $('#lblAvailableToCommit').text('-');
                        } 
                    }
                });

            } else {

                var pResult = await AjaxSend({
                    url: 'Stock/GetInventoryBalanceLite',
                    data: { 
                        OrgID: sessionStorage.OrgID, 
                        StockID: ProductID,
                        ProductID: ProductID,
                        InvoiceID: mOrderController.Model.Data.ID
                    },
                    success: function (vData) {
                        Bs.Loading('hide');
                        var TotalAdjusted = 0;
                        var Total = 0;
                        var pRows = [];

                        $('#tblBodyCompayWideInventoryBalance').empty();

                        for (var i = 0; i < vData.Results.Rows.length; i++) {
                            var pDR = vData.Results.Rows[i];
                            if (ReadyGuid(pDR.FowstID) == ReadyGuid(sessionStorage.FowstID))
                                pRows.unshift(pDR);
                            else
                                pRows.push(pDR);
                        }
                        vData.Results.Rows = pRows;

                        for (var i = 0; i < vData.Results.Rows.length; i++) {

                            TotalAdjusted += (parseInt(vData.Results.Rows[i].Balance) > 0 ? parseInt(vData.Results.Rows[i].Balance) : 0);
                            Total += parseInt(vData.Results.Rows[i].Balance);

                            var pTxtOrderQty = $('<input data-role="none" class="form-control text-center">')
                                .attr('name', 'txtOrderQty')
                                //.css({
                                //    width: '60px'
                                //});
                            pTxtOrderQty.val(AccountingFormat(vData.Results.Rows[i].AssignedQty, 0, ''));
                            pTxtOrderQty.on('textchange', function() {
                                if (mOrderController.Model.Data.ID == null)
                                    NewMessageBox('Transaction is not yet saved. Please save the transaction first before entering order quantities on this form.');
                                if (IsGuid(mOrderController.Model.Data.ParentOrderID))
                                    NewMessageBox('This transaction is a child of another transaction. Child transactions may not create it\'s own child transactions.');
                            });
                            var pCbDeliveryMethod = $('<select data-role="none" class="form-select">')
                                .attr('name', 'cbDeliveryMethod');
                                
                            var pOption = new Option('');                                pCbDeliveryMethod.append(pOption);
                            pOption = new Option('Pick Up by Customer');                 pCbDeliveryMethod.append(pOption);
                            pOption = new Option('Pick Up by Internal Logistics');       pCbDeliveryMethod.append(pOption);
                            pOption = new Option('Pick Up by 3rd Party Logistics');      pCbDeliveryMethod.append(pOption);
                            pOption = new Option('Deliver to Source of Original Order'); pCbDeliveryMethod.append(pOption);
                            pOption = new Option('Deliver to Customer');                 pCbDeliveryMethod.append(pOption);
                            pCbDeliveryMethod.val(vData.Results.Rows[i].DeliveryMethod);
                            tData = $('<tr>')
                                .attr('name', 'trDataRow')
                                .append($('<td>')
                                    .text(vData.Results.Rows[i].FOWST)
                                )
                                .append($('<td class="text-center">').append(pTxtOrderQty))
                                .append($('<td>').append(pCbDeliveryMethod))
                                .append($('<td class="text-right" id=lblBalance' + vData.Results.Rows[i].FowstID + '>')
                                    .text(AccountingFormat(vData.Results.Rows[i].Balance))
                                )
                                .append($('<td class="text-right" id=lblCommited' + vData.Results.Rows[i].FowstID + '>')
                                    .text(AccountingFormat('0'))
                                )
                                .append($('<td class="text-right" id=lblAvailableToCommit' + vData.Results.Rows[i].FowstID + '>')
                                    .text(CInt2(vData.Results.Rows[i].Balance) > 0 ? AccountingFormat(vData.Results.Rows[i].Balance) : 'NONE')
                                )
                                .data('data', vData.Results.Rows[i]);
                            $('#tblBodyCompayWideInventoryBalance').append(tData);
                        }

                        tData = $('<tr style="font-weight:bold; font-size:large">')
                                .append($('<td>')
                                    .text('Total :')
                                )
                                .append($('<td>'))
                                .append($('<td>'))
                                .append($('<td class="text-right" id=lblOnHandTotal>')
                                    .text(AccountingFormat(TotalAdjusted))
                                )
                                .append($('<td class="text-right" id=lblCommitedTotal>')
                                    .text(AccountingFormat('0'))
                                )
                                .append($('<td class="text-right" id=lblAvailableToCommitTotal>')
                                    .text(AccountingFormat(Total))
                                )

                        $('#tblBodyCompayWideInventoryBalance').append(tData);

                        tData = $('<tr style="font-weight:bold; font-size:large">')
                                .append($('<td>')
                                    .text('Adjusted Total (rows with  negative on-hand quantities are not included) :')
                                )
                                .append($('<td>'))
                                .append($('<td>'))
                                .append($('<td class="text-right" id=lblAdjustedOnHandTotal>')
                                    .text(AccountingFormat(TotalAdjusted))
                                )
                                .append($('<td class="text-right" id=lblAdjustedCommitedTotal>')
                                    .text(AccountingFormat('0'))
                                )
                                .append($('<td class="text-right" id=lblAdjustedAvailableToCommitTotal>')
                                    .text(AccountingFormat('0'))
                                )

                        $('#tblBodyCompayWideInventoryBalance').append(tData);
                    }
                });

                await GetAvailableToCommitProducts(false);

            }
        }

        async function GetAvailableToCommitProducts(IsByFOWST) {

            var isByFOWST = IsByFOWST;

            pData = {
                ProductID: ProductID
            }

            if (isByFOWST) {
                pData.FOWSTID = sessionStorage.FowstID;
            }

            await AjaxSend({
                url: 'SalesMenu/GetAvailableToCommitProducts',
                data: pData,
                success: function (vData) {
                    if (isByFOWST) {
                        if (vData.Results.Rows.length > 0) {
                            $('#lblCommited').text(AccountingFormat(vData.Results.Rows[0].Quantity));
                        } else {
                            $('#lblCommited').text(AccountingFormat('0'));
                        }
                    
                        var AvailableToCommit = CDec2($('#lblOnHand').text()) - CDec2($('#lblCommited').text());

                        if ($('#lblOnHand').text() != '') {
                            $('#lblAvailableToCommit').text(AccountingFormat((AvailableToCommit >= 0 ? AvailableToCommit : '0')));
                        }

                    } else {

                        var TotalAdjusted = 0;
                        var Total = 0;
                        var AvailableToCommitAdjusted;
                        var AvailableToCommit = 0;

                        for (i = 0; i < vData.Results.Rows.length; i++) {

                            TotalAdjusted += (parseInt(vData.Results.Rows[i].Quantity) > 0 ? parseInt(vData.Results.Rows[i].Quantity) : 0);
                            Total += parseInt(vData.Results.Rows[i].Quantity);

                            $('#lblCommited' + vData.Results.Rows[i].FOWSTID).text(AccountingFormat(vData.Results.Rows[i].Quantity))

                            AvailableToCommitAdjusted = CDec2($('#lblBalance' + vData.Results.Rows[i].FOWSTID).text()) -
                                        CDec2(vData.Results.Rows[i].Quantity)
                        
                            if (CDec2($('#lblBalance' + vData.Results.Rows[i].FOWSTID).text()) < 0 || AvailableToCommitAdjusted < 0) {
                                AvailableToCommitAdjusted = 'NONE';
                            }

                            $('#lblAvailableToCommit' + vData.Results.Rows[i].FOWSTID).text(
                                    (AvailableToCommitAdjusted == 'NONE' ? AvailableToCommitAdjusted : AccountingFormat(AvailableToCommitAdjusted))
                                )
                        }

                        AvailableToCommitAdjusted = CDec2($('#lblAdjustedOnHandTotal').text()) - CDec2(TotalAdjusted);
                        AvailableToCommit = CDec2($('#lblOnHandTotal').text()) - CDec2(Total);


                        $('#lblAdjustedCommitedTotal').text(AccountingFormat(TotalAdjusted));
                        $('#lblAdjustedAvailableToCommitTotal').text(
                                AccountingFormat(
                                        (AvailableToCommitAdjusted)
                                    )
                            );

                        $('#lblCommitedTotal').text(AccountingFormat(Total));
                        $('#lblAvailableToCommitTotal').text(
                                AccountingFormat(AvailableToCommit)
                            );
                    }
                }
            });
        }

        async function GetProductPriceList(strProductID) {
            var pData = [];

            pData = {
                ProductID: strProductID
            };

            await AjaxSend({
                url: 'Organization/getOrganizationProductPriceList',
                data: pData,
                success: function (vData) {
                    var count = 1;

                    $('#tblBodyProductPrice').empty();

                    if (vData.OrgProductPriceData.Rows.length == 0) {
                        $('#tblBodyProductPrice').append('<tr><td colspan="3" style="text-align:center;">No Result(s) Found</td></tr>')
                    }
                    else {
                        for (var i = 0; i < vData.OrgProductPriceData.Rows.length; i++) {
                            $('#tblBodyProductPrice').append($('<tr style="cursor:pointer;">')
                                                            .attr('ID', vData.OrgProductPriceData.Rows[i].ID)
                                                            .append($('<td>')
                                                                .text(count + '.')
                                                            )
                                                            .append($('<td>')
                                                                .text(proj.ChangePriceLevelDisplay(vData.OrgProductPriceData.Rows[i].PriceLevel))
                                                            )
                                                            .append($('<td>')
                                                                .text(AccountingFormat(vData.OrgProductPriceData.Rows[i].Price))
                                                            )
                                                            .data('data', vData.OrgProductPriceData.Rows[i])
                                                            .click(function (e) {
                                                            
                                                            })
                            )
                            count += 1;
                        }
                    }
                
                }
            });
        }

        mElement.find('[name=butSaveOrderQty]').click(function() {
            if (mOrderController.Model.Data.ID == null) {
                NewMessageBox('Transaction is not yet saved. Please save the transaction first before entering order quantities on this form.');
                return;
            }
            if (IsGuid(mOrderController.Model.Data.ParentOrderID)) {
                NewMessageBox('This transaction is a child of another transaction. Child transactions may not create it\'s own child transactions.');
                return;
            }

            var pFowstQty = [];
            var pTxtOrderQtys = $('#tblBodyCompayWideInventoryBalance [name=txtOrderQty]');
            for (var p = 0; p <= pTxtOrderQtys.length - 1; p++) {
                var pTxtOrderQty = $(pTxtOrderQtys[p]);
                var pQty = pTxtOrderQty.val();
                if (pQty.trim() == '')
                    continue;
                var pTR = pTxtOrderQty.closest('[name=trDataRow]');
                var pCbDeliveryMethod = pTR.find('[name=cbDeliveryMethod]');
                var pDR = pTR.data('data');
                if (!IsNumeric(pQty)) {
                    NewMessageBox('Ordered quantity must be a valid numerical value. You entered ' + pQty + ' for ' + pDR.FOWST + '.');
                    return;
                }
                pQty = CDec2(pQty);
                if (pQty < 0) {
                    NewMessageBox('Ordered quantity must be a equal to or greater than zero. You entered ' + pQty + ' for ' + pDR.FOWST + '.');
                    return;
                }
                if (!IsGuid(pDR.FowstID)) {
                    NewMessagebox('FowstID must be GUID. ref: js3W693N3FJR!');
                }
                pFowstQty.push({
                    FowstID: pDR.FowstID,
                    Qty: pQty,
                    DeliveryMethod: pCbDeliveryMethod.val()
                });
            }

            var pIndex;
            if (pFowstQty.length > 0) {
            	$.mobile.showPageLoadingMsg();
                function Proceed() {
                    var pParams = $.extend({
                        Fail: function(vError) {
                    	    $.mobile.hidePageLoadingMsg();
                            NewMessageBox(vError);
                            mAfterSave();
                        },
                        Success: function() {
                            pIndex++;
                            if (pIndex <= pFowstQty.length - 1)
                                Proceed();
                            else {
                    	        $.mobile.hidePageLoadingMsg();
                                NewMessageBox('Quantities were successfully saved.');
                                mAfterSave();
                            }
                        }
                    }, pFowstQty[pIndex]);
                    SavePreSelected(pParams);
                }
                pIndex = 0;
                Proceed();
            }

            function SavePreSelected(vParams) {
                var pData = {
                    CustomerID: mOrderController.Model.Data.CustomerID,
                    TransType: mOrderController.TransType,
                    DeleteExistingProduct: 0,
                    DontCheckCartValidity: 1,
                    ParentOrderID: mOrderController.Model.Data.ID,
                    StockID: ProductID,
                    SalesRep: [],
                    ProductSerial: [],
                    UpdateOrderedQty: 1,
                    FowstID: vParams.FowstID,
                    EncodingFowstID: sessionStorage.FowstID,
                    DeliveryMethod: vParams.DeliveryMethod,
                    Stocks: [
                        {
                            Qty: vParams.Qty,
                            UnitQty: 1
                        }
                    ]
                };

                AjaxSend({
                    url: '/Stock/UpdatePos',
                    data: pData,
                    NoSplash: true,
                    success: function(vData) {
                        if (vData.Error) {
                            vParams.Fail(vData.Error);
                        } else if (vData.Result == 'ok') {
                            //mOut.Model.Data.ID = vData.OrderID;
                            vParams.Success(vData);
                        } else {
                            vParams.Fail('Unknown status. ref: jsCSLRU5TDDJ!');
                        }

                    }
                });
            }
        });

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});