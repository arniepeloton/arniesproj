define([
], function () {

    CreateStringObject('ERP.ProductMaker.product.model', null, function () {

        var mOut = {};

        mOut.Data = {
            ParentID: null,
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
                url: '/ProductMaker/GetStockInfo',
                data: {
                    ParentID: mOut.Data.ParentID,
                    ProductID: mOut.Data.ProductID
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
                url: '/ProductMaker/SaveProduct',
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