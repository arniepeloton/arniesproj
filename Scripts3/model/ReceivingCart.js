define([
    'common',
    'promise!/Scripts3/model/Receiving.js?crc=', // not really needed but we need "App.model.Receiving.OfflineMode"
    GetPathFormat('/Scripts3/Browser/LibraryFowstBinStock.js?crc=')
], function() {

    var mDeferred = $.Deferred();

    CreateStringObject('App.model.ReceivingCart', null, function() {
        
        var pOut = {};

        pOut.CheckForMultipleSizes = function(vParams) {
            /***
            vParams = {
                ProductGroupNo: 'string',
                success: function(vData) {
                }
            };
            ***/

            var pResult;

            if (App.model.Receiving.OfflineMode) {
                pResult = App.model.ReceivingCart_offline.CheckForMultipleSizes(vParams.ProductGroupNo);
                vParams.success({InventoryCount: pResult});
                return;
            };

            AjaxSend({
                url: 'InventoryCount/GetCountPIMProductGroupNo',
                data: {ProductGroupNo: vParams.ProductGroupNo},
                success: function (vData) {
                    vParams.success(vData);
                }
            });
        };

        pOut.GetStockUnitMeasures = function(vParams) {
            /***
            vParams = {
                ProductID: 'string',
                ReceivingID: 'string',
                success: function(vData) {
                    
                }
            };
            ***/

            var pResult;

            if (App.model.Receiving.OfflineMode) {
                pResult = App.model.ReceivingCart_offline.GetStockUnitMeasures({
                    StockID: vParams.ProductID
                    //ReceivingID: vParams.ReceivingID
                });
                vParams.success({ UnitMeasure: pResult });
                return;
            };

            AjaxSend({
                url: 'Receiving/GetStockUnitMeasures',
                data: {
                    ProductID: vParams.ProductID,
                    ReceivingID: vParams.ReceivingID
                },
                success: function (vData) {
                    vParams.success(vData);
                }
            });
        };

        return pOut;

    }); // END CreateStringObject('App.model.ReceivingCart', null, function() {

    App.model.ReceivingCart = App.model.ReceivingCart(); // singleton

    if (OfflineMode) {
        require(['/Scripts3/model/ReceivingCart_offline.js?crc='], function() {
            mDeferred.resolve();
        });
        return mDeferred.promise();
    }

}); // END require
