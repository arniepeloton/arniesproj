define([
], function () {

    CreateStringObject('ERP.ActivityTracker.model', null, function () {

        var mOut = {};

        mOut.Data = {
            ID: null,
            TransType: null
        };
        
        mOut.GetData = function(vParams) {
            /*
            vParams = {
                ProductID: 'required',
                Success: function(vArray) {
                },
                Fail: function(vMessage) {
                }
            }
            */

            AjaxSend({
                url: '/Erp/GetActivity',
                data: {
                    ID: mOut.Data.ID,
                    TransType: mOut.Data.TransType,
                    Serial: vParams.Serial
                },
                success: function(vData) {
                    if (vData.Error) {
                        vParams.Fail(vData.Error);
                    } else {
                        vParams.Success(vData);
                    }
                }
            });
        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

});