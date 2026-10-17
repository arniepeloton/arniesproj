define([
    '/Scripts/sources/ERP/ProductMaker/product/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ProductMaker/product/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = $(vData);
                $('body').append(mHtml);
                ERP.ProductMaker.product.controller = ERP.ProductMaker.product.controller(); // singleton
                // mHtml.trigger('create');
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.ProductMaker.product.controller', null, function () {

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
            mOut.Model.Data.ParentID = vParams.TransID;
            mOut.Model.Data.ProductID = vParams.ProductID;
            mOut.Model.Data.CustomerID = vParams.CustomerID;
            mTransType = vParams.TransType;
            mElement.find('[name=title]').text(vParams.ProductName);
            mElement.find('[name=spanProductNumber]').text(vParams.ProductNo);
            Clear();
            // mElement.show().popup('open'); // doesnt work
            $('#divPopupProductMakerProduct').show().popup('open');
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
              
                pQty = CDec2(pDR.Qty);
                if (pQty == 0)
                    pQty = '';
                pTR.find('[name=txtQty]').val(pQty);
                
                mTable.append(pTR);
            }

        } // END function DisplayData(vData) {

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
            var pData = {
                ProductID: mOut.Model.Data.ProductID,
                Items: []
            };
            var pTRs = mTable.find('tr[name=data]');
            for (var p = 0; p <= pTRs.length - 1; p++) {
                var pStock = {};
                var pTR = $(pTRs[p]);
                var pDR = pTR.data('data');
                var pQty = CDec2(pTR.find('[name=txtQty]').val());
                if (pQty <= 0)
                    continue;
                pStock.UnitQty = pDR.UnitQuantity;
                pStock.UnitMeasureID = pDR.MeasureID;
                pStock.Qty = pQty;
                pData.Items.push(pStock);
            }

            mOut.Model.Save({
                data: pData,
                Success: function() {
                    mOnSave(pData);
                },
                Fail: function(vData) {
                    NewMessageBox(vData);
                }
            });
        });

        mOut.Init = function() {
            mOut.Model = new ERP.ProductMaker.product.model();
            mElement.trigger('create');
        };

        mOut.Init();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});