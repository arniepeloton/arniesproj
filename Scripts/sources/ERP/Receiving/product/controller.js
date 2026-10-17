define([
    '/Scripts/sources/ERP/Receiving/product/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Receiving/product/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = $(vData);
                $('body').append(mHtml);
                ERP.Receiving.product.controller = ERP.Receiving.product.controller(); // singleton
                // mHtml.trigger('create');
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.Receiving.product.controller', null, function () {

        var mOut = {};
        var mElement;
        var mDivUnit;
        var mTransType;
        var mOnSave;

        mElement = mHtml;
        mOut.element = mElement;
        mOut.Model = null;
        
        var mTable = mElement.find('[name=tableProduct]');
        var mTR = mTable.find('tbody tr:first');

        mOut.popup = function(vParams) {
            mOnSave = vParams.OnSave;
            mOut.Model.Data.ReceivingStockID = vParams.ReceivingStockID;
            mOut.Model.Data.TransID = vParams.TransID;
            mOut.Model.Data.ProductID = vParams.ProductID;
            mOut.Model.Data.CustomerID = vParams.CustomerID;
            mTransType = vParams.TransType;
            mElement.find('[name=title]').text(vParams.ProductName);
            mElement.find('[name=spanProductNumber]').text(vParams.ProductNo);
            Clear();
            // mElement.show().popup('open'); // doesnt work
            $('#divPopupReceivingProduct').modal('show');
            mOut.Model.GetData({
                Success: function(vData) {
                    vParams.Success();
                    DisplayData(vData);
                },
                Fail: function(vMessage) {
                    NewMessageBox(vMessage);
                }
            });
        };

        function Clear() {
            
            if (mDivUnit == null)
                mDivUnit = $(mElement.find('[name=divUnit]:first')).clone();
            mElement.find('[name=divUnit]').remove();
            mTable.find('tbody tr').remove();

        }

        function DisplayData(vData) {
            var pBadOrderReason;

            mElement.find('[name=BaseUnit]').text(vData.BaseUnit);
            var pUnitQty = vData.BaseUnitQty;
            for (var p = 0; p <= vData.Units.length - 1; p++) {
                var pDR = vData.Units[p];
                AddRow(pDR);
            }
          
            function AddRow(vDR) {
                var pTR = mTR.clone();
                var pQty;
                pTR.data('data', pDR);
                pTR.find('[name=Unit]').text(vDR.UnitMeasure);
                pTR.find('[name=QtyInBaseUnit]').text('(' + pDR.UnitQuantity + ' ' + vData.BaseUnit + ')');
                pTR.data('UnitQty', pUnitQty);
              
                pQty = CDec2(pDR.QtyPo);
                if (pQty == 0)
                    pQty = '';
                pTR.find('[name=tdPoQty]').text(pQty);

                pQty = CDec2(pDR.QtyGood);
                if (pQty == 0)
                    pQty = '';
                pTR.find('[name=txtGoodQty]').val(pQty);
                pQty = CDec2(pDR.QtyBad);
                if (pQty == 0)
                    pQty = '';
                pTR.find('[name=txtBadQty]').val(pQty);
                if (pBadOrderReason == null)
                    pBadOrderReason = pDR.BadOrderReason;
                mTable.append(pTR);
            }

            DisplayBadReasons(pBadOrderReason);
        } // END function DisplayData(vData) {

        function DisplayBadReasons(vReason) {
            vReason = CStr2(vReason);
            var pCB = mElement.find('[name=cbBadOrderReason]');
            var pData = [
                {
                    value: '',
                    text: 'Please select'
                },
                {
                    value: 'Expireditem',
                    text: 'Expired item'
                },
                {
                    value: 'About-to-expireitem',
                    text: 'About-to-expire item'
                },
                {
                    value: 'Factory-defectivepackaging',
                    text: 'Factory-defective packaging'
                },
                {
                    value: 'Factory-defectiveproduct',
                    text: 'Factory-defective product'
                },
                {
                    value: 'Non-Factory-DamagedGoods',
                    text: 'Non-Factory-Damaged Goods'
                },
                {
                    value: 'Non-Factory-DamagedPackaging',
                    text: 'Non-Factory-Damaged Packaging'
                },
                {
                    value: 'Obsoleteitem',
                    text: 'Obsolete item'
                },
                {
                    value: 'Productrecall',
                    text: 'Product recall'
                }
            ];
            pCB.html('');
            for (var p = 0; p <= pData.length - 1; p++) {
                var pDatum = pData[p];
                var pOption = new Option(pDatum.text, pDatum.value);
                pCB.append(pOption);
            }
            pCB.find('option[value=' + vReason + ']').attr('selected', true);
            //pCB.select('refresh');
            //pCB.trigger('change');
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

        mElement.find('[name=butOk]').click(function(e) {
            e.preventDefault();
            var pCB = mElement.find('[name=cbBadOrderReason]');
            var pTotalPo = 0; // qty
            var pTotalReceived = 0; // qty
            var pData = {
                ProductID: mOut.Model.Data.ProductID,
                Items: []
            };
            var pTRs = mTable.find('tr[name=data]');
            for (var p = 0; p <= pTRs.length - 1; p++) {
                var pStock = {};
                var pTR = $(pTRs[p]);
                var pDR = pTR.data('data');
                pStock.UnitQty = pDR.UnitQuantity;
                pStock.UnitMeasure = pDR.MeasureID;
                pStock.ReceivingStockID = pDR.ReceivingStockID;
                pStock.GoodQty = CDec2(pTR.find('[name=txtGoodQty]').val());
                if (pStock.GoodQty < 0) {
                    NewMessageBox('Good quantity must be equal to or greater than zero.');
                    return;
                }
                pStock.BadQty = CDec2(pTR.find('[name=txtBadQty]').val());
                if (pStock.BadQty < 0) {
                    NewMessageBox('Bad quantity must be equal to or greater than zero.');
                    return;
                }
                pStock.BadOrderReason = pCB.find('option:selected').val();
                pStock.PoUnitPrice = pDR.PoUnitPrice;
                pData.Items.push(pStock);
                pTotalPo += CDec2(pDR.QtyPo);
                pTotalReceived += pStock.GoodQty + pStock.BadQty;
            }

            if (pTotalPo > 0) {
                var pMessage;
                if (pTotalPo < pTotalReceived) 
                    pMessage = 'Total received quantity (' + pTotalReceived + ' in base unit) is greater than the PO quantity (' + pTotalPo + ' in base unit).\n' +
                        'Do you still want to save this transaction?';
                else if (pTotalPo > pTotalReceived) 
                    pMessage = 'Total received quantity (' + pTotalReceived + ' in base unit) is lesser than the PO quantity (' + pTotalPo + ' in base unit).\n' +
                        'Do you still want to save this transaction?';
                    
                if (pMessage) {
                    NewMessageBox(pMessage, 'Received quantity is greater than PO quantity.', 
                        [
                            {
                                Text: 'Yes',
                                Click: function () {
                                    $('#divPopupReceivingProduct').modal('hide');
                                    mOnSave(pData);
                                }
                            },
                            {
                                Text: 'No',
                                Click: function() {
                                    mElement.show().popup('open');
                                }
                            }
                        ]
                    ); // END NewMessageBox
                    return;
                }
            } // END if (pTotalPo > 0) {
            $('#divPopupReceivingProduct').modal('hide');
            mOnSave(pData);
        });

        mOut.Init = function() {
            mOut.Model = new ERP.Receiving.product.model();
            //mElement.trigger('create');
        };

        mOut.Init();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});