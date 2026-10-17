define(['common',
    '/Scripts3/model/BomContent.js?crc=',
    '/Scripts3/controller/UnitDefinition.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts3/view/BomContent.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    $(function () {

        CreateStringObject('App.controller.BomContent', null, function () {

            var mOut = {};
            var mElement;
            var mData;
            var mComboInited = false;

            mOut.model = new App.model.BomContent();
            mOut.model.parent = mOut;

            mOut.unitDefinition = new App.controller.UnitDefinition();
            mOut.unitDefinition.saveCallback = UnitDefinition_Save;

            mElement = $(mHtml);
            $('body').append(mElement);
            mOut.element = mElement;

            var mSpanParentUnit = mElement.find('[name=spanParentUnit]');
            var mSpanParentProduct = mElement.find('[name=spanParentProduct]');
            var mSpanChildProduct = mElement.find('[name=spanChildProduct]');
            var mSpanQty = mElement.find('[name=spanQty]')
                .on('textchange', UpdateHeader);
            var mSpanChildUnit = mElement.find('[name=spanChildUnit]');
            var mTxtQty = mElement.find('[name=txtQty]');
            var mCbUnit = mElement.find('[name=cbUnit]');
            var mButSave = mElement.find('[name=butSave]');

            mButSave.click(function () {
                mOut.model.qty = mTxtQty.val();

                // only 1 of these is used
                mOut.model.unitMeasureID = mCbUnit.val();
                mOut.model.unitMeasure = mCbUnit.children('option:selected').text();

                mOut.model.save({
                    success: function (vData) {
                        MessageBox('Data was successfully saved.', null, null, {
                            OnClose: function() {
                                $.mobile.changePage('#pageOrgProductInfo');
                                mOut.parent.getData({
                                    success: function(vData) {
                                    },
                                    error: function(vMessage) {
                                    }
                                });
                            }  
                        });
                    },
                    error: function (vError) {
                        MessageBox(vError);
                    }
                });
            });

            mOut.show = function () {
                mCbUnit.html('');
                $.mobile.changePage(mElement);
                loadData({
                    success: function () {
                    }
                });
            };

            function loadData(vParams) {
                /*
                vParams = {
                success: function() {
                            
                }
                };
                */

                mOut.model.getBomPageData({
                    success: function (vData) {
                        popMaster(vData);
                        popContent();
                        popCombo(vData.units);
                        UpdateHeader();
                        vParams.success();
                    },
                    error: function (vMessage) {
                        MessageBox(vMessage, 'Error');
                    }
                });
            }

            function popMaster(vData) {

                mSpanParentUnit.text(vData.parent.BaseUnit);
                mSpanParentProduct.text(vData.parent.ProductName);
                mSpanChildProduct.text(vData.child.ProductName);

            }

            function popContent() {

                mTxtQty.val($.format.number(mOut.model.qty, proj.QuantityFormat));

            }

            function popCombo(vData) {

                var pCbItem;
                var pDR;
                var pRows = GetStringObject('Rows', vData);
                var pSelectedUnitMeasureID;
                var pSelectedUnitMeasure;
                var pItem1; // 1st unit measure

                mCbUnit.html('');
                if (!$.isArray(pRows))
                    RaiseError('Rows is required and must be of type array js5PQRVKB5VA!');

                if (mOut.model.unitMeasureID != undefined)
                    pSelectedUnitMeasureID = ReadyGuid(mOut.model.unitMeasureID);

                if (mOut.model.unitMeasure != undefined)
                    pSelectedUnitMeasure = mOut.model.unitMeasure;

                for (var pIndex = 0; pIndex <= pRows.length - 1; pIndex++) {
                    pDR = pRows[pIndex];
                    if (pItem1 == undefined)
                        pItem1 = pDR;
                    pDR.ID = ReadyGuid(pDR.ID);
                    pCbItem = new Option(pDR.UnitMeasure, ReadyGuid(pDR.ID));

                    if ((pSelectedUnitMeasureID != undefined)
                        && (pSelectedUnitMeasureID == pDR.ID)) {

                        pCbItem.selected = true;
                    } else if ((pSelectedUnitMeasure != undefined) 
                        && (pSelectedUnitMeasure == pDR.UnitMeasure)) {

                        pCbItem.selected = true;
                    }

                    $(pCbItem).data('data', pDR);
                    mCbUnit.append(pCbItem);
                }
                if (pItem1 != undefined) {
                    pCbItem = $('<option>')
                        .attr('disabled', 'disabled')
                        .css('border-bottom', '1px dotted #00') // doesnt work
                        .text('──────');
                    mCbUnit.append(pCbItem);

                    pCbItem = $('<option>')
                        .val('define')
                        .css('border-bottom', '1px dotted #00') // doesnt work
                        .text('Define ' + pItem1.UnitMeasure);
                    mCbUnit.append(pCbItem);

                }

                mCbUnit.selectmenu();
                mCbUnit.selectmenu('refresh', true);

            };

            function UpdateHeader() {

                mSpanQty.text($.format.number(mTxtQty.val()));
                mSpanChildUnit.text(mCbUnit.children('option:selected').text());

            }

            mCbUnit.change(function (e) {
                var pThis = $(this);
                if (pThis.val() == 'define') {
                    mOut.unitDefinition.model.productID = mOut.model.childProductID;
                    mOut.unitDefinition.model.organizationID = mOut.model.organizationID;
                    mOut.unitDefinition.model.unitMeasureID = mOut.model.childProductBaseUnitMeasureID;
                    mOut.unitDefinition.show();
                } else
                    UpdateHeader();

            });

            function UnitDefinition_Save() {
                mOut.show();
            }

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});