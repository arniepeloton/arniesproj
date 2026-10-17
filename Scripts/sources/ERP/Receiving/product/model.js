define([
], function () {

    CreateStringObject('ERP.Receiving.product.model', null, function () {

        var mOut = {};

        mOut.Data = {
            TransID: null,
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
                url: '/Receiving/GetStockInfo',
                data: {
                    TransID: mOut.Data.TransID,
                    ProductID: mOut.Data.ProductID,
                    ReceivingStockID: mOut.Data.ReceivingStockID
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

        mOut.Save = function(vParams) {
            AjaxSend({
                url: '/Stock/UpdatePos',
                data: $.extend(mOut.Data, {
                    StockID: mOut.Data.ProductID,
                    DeleteExistingProduct: 1,
                    DontCheckCartValidity: 1
                }, vParams.data),
                success: function(vData) {
                    if (vData.Error) {
                        vParams.Fail(vData.Error);
                    } else if (vData.Result == 'ok') {
                        mOut.Data.NewCartMode = false;
                        mOut.Data.OrderID = vData.OrderID;
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