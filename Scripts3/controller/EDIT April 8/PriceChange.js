define(['/Scripts3/model/PriceChange.js?crc='
    ], function () {

        var mDeferred = $.Deferred();
        var mHtml;

        (function staticInit() {

            AjaxSend({
                ExactUrl: '/Scripts3/view/PriceChange.html',
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mHtml = vData;
                    mDeferred.resolve();
                }
            });

        })();

        $(function () {

            CreateStringObject('App.controller.PriceChange', null, function () {

                var mOut = {};
                var mElement;
                var mData;
                var mComboInited = false;

                mOut.model = new App.model.PriceChange();

                mElement = $(mHtml);
                mOut.element = mElement;
                // $('body').append(mElement).trigger('create');
                // $('[data-role=page]:first').append(mElement).trigger('create');

                //            $('[data-role=page]:first').append(mElement);
                //            $('[data-role=page]:first').trigger('create');

                //            $.mobile.activePage.append(mElement);
                //            $.mobile.activePage.trigger('create');


                function CacheHtml(vCallback) {

                    if (mHtml == undefined) {

                    } else
                        vCallback();

                } // END function CacheHtml() {

                mOut.popup = function () {

                    var pElement;

                    // mElement.appendTo($.mobile.activePage).trigger('create');
                    //                mElement.appendTo($.mobile.activePage);
                    //                mElement.trigger('create');
                    // mElement.appendTo($('[data-role=page]:first')).trigger('create');

                    if (mElement.parent().length == 0) {
                        $.mobile.activePage.append(mElement);
                        $.mobile.activePage.trigger('create');
                    }

                    mElement.popup('open');
                    pElement = mElement.closest('.ui-popup-container');
                    if (pElement.length == 0)
                        console.trace('popup container for divRequestPrice not found');
                    else
                    // know issue: pop up dimension will revert to default after browser window resize
                        pElement.css({
                            left: 0,
                            width: '80%',
                            margin: 'auto 10%',
                            padding: 15
                        });
                };

                mOut.popupClose = function () {

                    mElement.popup('close');

                };

                mOut.divRequestPriceMessage = mElement.find('[name=divRequestPriceMessage]');
                mOut.formRequestedPrice = mElement.find('[name=formRequestedPrice]');
                mOut.txtRequestedPrice = mElement.find('[name=txtRequestedPrice]');
                mOut.txtRequestedPriceDiscountPercent = mElement.find('[name=txtRequestedPriceDiscountPercent]');
                mOut.cbRequestedPriceBasePrice = mElement.find('[name=cbRequestedPriceBasePrice]');
                mOut.txtRequestedPriceDiscountAmount = mElement.find('[name=txtRequestedPriceDiscountAmount]');
                mOut.lnkSecurityDiscountedPrice = mElement.find('[name=lnkSecurityDiscountedPrice]');
                mOut.lnkSecurityDiscountPercentage = mElement.find('[name=lnkSecurityDiscountPercentage]');
                mOut.lnkSecurityDiscountAmount = mElement.find('[name=lnkSecurityDiscountAmount]');

                //additional by janine for disable fields sept 29 2014
                mOut.txtRequestedPriceRemoveDisable = function (percentage, amount) {
                    mOut.txtRequestedPrice.show();

                    mOut.lnkSecurityDiscountedPrice.hide();
                };
                mOut.txtRequestedPriceDiscountPercentRemoveDisable = function (price, amount) {
                    mOut.txtRequestedPriceDiscountPercent.show();
                    mOut.lnkSecurityDiscountPercentage.hide();

                };
                mOut.txtRequestedPriceDiscountAmountRemoveDisable = function (price, percentage) {
               
                    mOut.txtRequestedPriceDiscountAmount.show();

                    mOut.lnkSecurityDiscountAmount.hide();
                };

                //end of additional

                mOut.txtRequestedPrice.bind('input', function () {
                    var pThis = mOut.txtRequestedPrice;

                    if (!IsNumeric(pThis.val()))
                        return;

                    mOut.model.setNetAmount(pThis.val());
                    mOut.txtRequestedPriceDiscountPercent.val(mOut.model.discountPercent.toFixed(2));
                    mOut.txtRequestedPriceDiscountAmount.val(mOut.model.discountAmount.toFixed(2));
                });

                mOut.txtRequestedPriceDiscountPercent.bind('input', function () {
                    var pThis = $(this);

                    if (!IsNumeric(pThis.val()))
                        return;

                    mOut.model.setDiscountPercent(pThis.val());
                    mOut.txtRequestedPrice.val(mOut.model.netAmount.toFixed(2));
                    mOut.txtRequestedPriceDiscountAmount.val(mOut.model.discountAmount.toFixed(2));
                });

                mOut.txtRequestedPriceDiscountAmount.bind('input', function () {
                    var pThis = $(this);

                    if (!IsNumeric(pThis.val()))
                        return;

                    mOut.model.setDiscountAmount(pThis.val());
                    mOut.txtRequestedPrice.val(mOut.model.netAmount.toFixed(2));
                    mOut.txtRequestedPriceDiscountPercent.val(mOut.model.discountPercent.toFixed(2));
                });

                mOut.unitQuantity = 1;

                mOut.setData = function (vData) {

                    mData = vData;
                    mOut.popCombo();

                };

                mOut.popCombo = function () {

                    var pCbItem;
                    var pDR;
                    var pRows = GetStringObject('data.Rows', mData);

                    if (!$.isArray(pRows))
                        RaiseError('Rows is required and must be of type array js29KZHQ23U6!');

                    mOut.cbRequestedPriceBasePrice.html('');
                    for (var pIndex = 0; pIndex <= pRows.length - 1; pIndex++) {
                        pDR = pRows[pIndex];
                        if (mOut.unitQuantity == pDR.Quantity) {
                            pCbItem = new Option(pDR.PriceLevel + ' ' + $.format.number(CDec2(pDR.DiscountedPrice), '#,##0.00'), ReadyGuid(pDR.PriceLevelID));
                            $(pCbItem).data('data', pDR);
                            // pCbItem = new Option(pRows[p1].PriceLevel, pRows[p1].PriceLevelID); 
                            mOut.cbRequestedPriceBasePrice.append(pCbItem);
                        };
                    }

                    if (!mComboInited) {
                        mComboInited = true;
                        mOut.cbRequestedPriceBasePrice.change(cbRequestedPriceBasePrice_change);
                    }
                    cbRequestedPriceBasePrice_change.apply(mOut.cbRequestedPriceBasePrice);
                    mOut.cbRequestedPriceBasePrice.selectmenu();
                    mOut.cbRequestedPriceBasePrice.selectmenu('refresh', true);

                }; // END mOut.popCombo = function() {

                mOut.setPriceLevelID = function (vID) {

                    mOut.cbRequestedPriceBasePrice.val(ReadyGuid(vID));

                };

                mOut.getPriceLevelID = function () {

                    return mOut.cbRequestedPriceBasePrice.val();

                };

                function cbRequestedPriceBasePrice_change() {
                    var pThis = $(this);
                    var pSelected = pThis.find(':selected');
                    var pDR = pSelected.data('data');
                    mOut.model.setBaseAmount(pDR.DiscountedPrice, 'netAmount');
                    mOut.txtRequestedPrice.val(mOut.model.netAmount.toFixed(2));
                    mOut.txtRequestedPriceDiscountPercent.val(mOut.model.discountPercent.toFixed(2));
                    mOut.txtRequestedPriceDiscountAmount.val(mOut.model.discountAmount.toFixed(2));
                }

                mOut.onInvalid = function (vResult) {

                    MessageBox(vResult.error);

                };

                mOut.validate = function () {

                    var pResult = mOut.model.validate();
                    if (pResult.error != undefined) {
                        if ($.isFunction(mOut.onInvalid))
                            mOut.onInvalid(pResult);
                    }

                    return pResult;

                };

                mOut.onSubmit = function () { };
                mElement.find('[name=formRequestedPrice]').submit(function (e) {
                    e.preventDefault();
                    var pResult = mOut.validate();
                    if (pResult.error != undefined) {
                        return false;
                    }
                    return mOut.onSubmit(arguments);
                });

                return mOut;

            }); // END CreateStringObject('App.controller.PriceChange', null, function() {

        }); // END $(function () {

        return mDeferred.promise();

    });