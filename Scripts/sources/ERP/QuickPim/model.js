define([
], function () {

    CreateStringObject('ERP.QuickPim.model', null, function () {

        var mOut = {};

        mOut.Save = function(vParams) {
            var pData = {
                ShortName: vParams.ShortName,
                LandedCost: vParams.LandedCost,
                RetailPrice: vParams.RetailPrice
            };

            AjaxSend({
                url: '/Stock/QuickAdd',
                data:pData,
                success: function(vData) {
                    if (vData.Error) {
                        vParams.Fail(vData.Error);
                    } else if (IsGuid(vData.Result)) {
                        vParams.Success(vData);
                    } else {
                        NewMessageBox('Unknown status');
                    }
                }
            });
        };

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

});