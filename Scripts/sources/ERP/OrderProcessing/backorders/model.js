define([
], function () {

    CreateStringObject('ERP.OrderProcessing.backorders.model', null, function () {

        var mOut = {};

        mOut.Data = {
            ProductID: null,
            CustomerID: null,
            OrderID: null,
            SalesRep: [],
            ProductSerial: [],
            NewCartMode: true,
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
                url: '/Browser/TransInvoices',
                data: {
                    TransType: 'SalesOrder',
                    AnyZero: 1,
                    AllowNoItems: true,
                    UneditedBackorder: true
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