define(['common',
    '/Scripts3/model/UnitDefinition.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts3/view/UnitDefinition.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    $(function () {

        CreateStringObject('App.controller.UnitDefinition', null, function () {

            var mOut = {};
            var mElement;
            var mData;
            var mComboInited = false;
                
            mOut.model = new App.model.UnitDefinition();
            mOut.model.parent = mOut;

            mElement = $(mHtml);
            $('body').append(mElement);
            mOut.element = mElement;

            var mSpanParentUnit = mElement.find('[name=spanParentUnit]');
            var mSpanParentProduct = mElement.find('[name=spanParentProduct]');
            var mTxtQty =  mElement.find('[name=txtQty]');
            var mCbUnit = mElement.find('[name=cbUnit]');
            var mButSave = mElement.find('[name=butSave]');

            mButSave.click(function() {
                mOut.model.qty = mTxtQty.val();
                mOut.model.destinationMeasure = mCbUnit.val();
                mOut.model.save({
                    success: function(vData) {
                        MessageBox('Data was successfully saved.', null, null, {
                            OnClose: mOut.saveCallback
                        });
                    },
                    error: function(vError) {
                        MessageBox(vError);
                    }
                });
            });

            mOut.show = function() {
                mCbUnit.html('');
                $.mobile.changePage(mElement);
                loadData({
                    success: function() {
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

                mOut.model.getData({
                    success: function(vData) {
                        popMaster(vData);
                        popContent();
                        popCombo(vData.families);
                        vParams.success();
                    },
                    error: function(vMessage) {
                        MessageBox(vMessage, 'Error');
                    }
                });
            }

            function popMaster(vData) {

                mSpanParentUnit.text(vData.parent.BaseUnit);
                mSpanParentProduct.text(vData.parent.ProductName);

            }

            function popContent() {
                    
                mTxtQty.val($.format.number(mOut.model.qty,  proj.QuantityFormat));

            }

            function popCombo(vData) {

                var pCbItem;
                var pDR;
                var pRows = GetStringObject('Rows', vData);
                var pSelectedUnitMeasureID;
                var pFamily;

                mCbUnit.html('');
                if (!$.isArray(pRows))
                    RaiseError('Rows is required and must be of type array jsL73KD8ZHFL!');

//                    if (mOut.model.unitMeasureID != undefined)
//                        pSelectedUnitMeasureID = ReadyGuid(mOut.model.unitMeasureID);

                for (var pIndex = 0; pIndex <= pRows.length - 1; pIndex++) {
                    pDR = pRows[pIndex];
                    if (pFamily != pDR.MeasurementFamily) {
                        pFamily = pDR.MeasurementFamily;
                        pCbItem = $('<option>')
                            .attr('disabled', 'disabled')
                            .css('font-weight', 'bold') // doesnt work
                            .text('[' + pFamily + ']');
                        mCbUnit.append(pCbItem);
                    }
                        
                    pCbItem = $('<option>')
                        .val(pDR.DestMeasure)
                        .text(pDR.DestMeasure);
                    if (mOut.model.destinationMeasure)
                        if (pDR.DestMeasure == mOut.model.destinationMeasure)
                            pCbItem.attr('selected', true);
                    mCbUnit.append(pCbItem);
                }
                    
                mCbUnit.selectmenu();
                mCbUnit.selectmenu('refresh', true);
            };

//                mCbUnit.change(function(e) {
//                    var pThis = $(this);
//                });

            mOut.saveCallback = function() {};

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});