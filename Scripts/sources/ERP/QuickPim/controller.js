define([
    '/Scripts/sources/ERP/QuickPim/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/QuickPim/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = $(vData);
                $('body').append(mHtml);
                ERP.QuickPim.controller = ERP.QuickPim.controller(); // singleton
                // mHtml.trigger('create');
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.QuickPim.controller', null, function () {

        var mOut = {
            AfterSave: function(vData) {
            }
        };
        var mElement;
        var mDivUnit;
        var mTransType;
        var mOnSave;
        var mModel = new ERP.QuickPim.model();

        mElement = mHtml;
        mOut.element = mElement;
        
        var mTable = mElement.find('[name=tableProduct]');
        var mTR = mTable.find('tbody tr:first');

        mOut.popup = function(vParams) {
            //mOnSave = vParams.OnSave;
            //mOut.Model.Data.TransID = vParams.TransID;
            //mOut.Model.Data.ProductID = vParams.ProductID;
            //mOut.Model.Data.CustomerID = vParams.CustomerID;
            //mTransType = vParams.TransType;
            //mElement.find('[name=title]').text(vParams.ProductName);
            //mElement.find('[name=spanProductNumber]').text(vParams.ProductNo);
            Clear();
            // mElement.show().popup('open'); // doesnt work
            mElement.popup().trigger('create').show().popup('open');
            //mOut.Model.GetData({
            //    Success: function(vData) {
            //        vParams.Success();
            //        DisplayData(vData);
            //    },
            //    Fail: function(vMessage) {
            //        NewMessageBox(vMessage);
            //    }
            //});
        };

        function Clear() {
            
            mElement.find('[name=txtShortName]').val('');
            mElement.find('[name=txtLandedCost]').val('');
            mElement.find('[name=txtRetailPrice]').val('');

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
            pCB.trigger('change');
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

            var pShortName = mElement.find('[name=txtShortName]').val();
            var pLandedCost = mElement.find('[name=txtLandedCost]').val();
            var pRetailPrice = mElement.find('[name=txtRetailPrice]').val();

            if (pShortName.trim().length == 0) {
                NewMessageBox('Please enter the short name', 'No Short Name', 
                    [
                        {
                            Text: 'Ok',
                            Click: function() {
                                mElement.show().popup('open');
                            }
                        }
                    ]
                ); // END NewMessageBox
                return;
            }

            if (!IsNumeric(pLandedCost) 
                || (CDec2(pLandedCost) < 0)) {

                NewMessageBox('Landed cost must be a number equal to or greater than zero.', 'Landed Cost', 
                    [
                        {
                            Text: 'Ok',
                            Click: function() {
                                mElement.show().popup('open');
                            }
                        }
                    ]
                ); // END NewMessageBox
                return;
            }

            if (!IsNumeric(pRetailPrice) 
                || (CDec2(pLandedCost) < 0)) {

                NewMessageBox('Retail price must be a number equal to or greater than zero.', 'Retail Price', 
                    [
                        {
                            Text: 'Ok',
                            Click: function() {
                                mElement.show().popup('open');
                            }
                        }
                    ]
                ); // END NewMessageBox
                return;
            }

            mModel.Save({
                ShortName: pShortName,
                LandedCost: pLandedCost,
                RetailPrice: pRetailPrice,
                Success: function(vData) {
                    console.log('js8DKAMVCSL6!');
                    mOut.AfterSave({
                        ID: vData.Result,
                        ProductName: pShortName
                    });
                },
                Fail: function(vMessage) {
                    NewMessageBox(vMessage, 'Failed to add the new SKU', 
                    [
                        {
                            Text: 'Ok',
                            Click: function() {
                                mElement.show().popup('open');
                            }
                        }
                    ]
                ); // END NewMessageBox
                }
            });

            //mOnSave(pData);
        });

        mOut.Init = function() {
            //mOut.Model = new ERP.Receiving.product.model();
            mElement.trigger('create');
        };

        mOut.Init();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});