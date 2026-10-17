define([
], function () {

    CreateStringObject('ERP.SalesReturn.product.model', null, function () {

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
        mOut.ReturnsReasons = null;

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
                url: '/SalesReturns/GetInfo',
                data: {
                    ProductID: mOut.Data.ProductID,
                    InvoiceID: mOut.Data.InvoiceID,
                    GetReturnsReasons: (mOut.ReturnsReasons == undefined ? 1 : 0)
                },
                success: function(vData) {
                    if (vData.Error) {
                        vParams.Fail(vData.Error);
                    } else {
                        if (vData.ReturnsReasons != undefined)
                            mOut.ReturnsReasons = vData.ReturnsReasons;
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

        mOut.GetPriceHistory = function(vParams) {
            vParams = $.extend({
                CustomerID: null,
                ProductID: null,
                Fail: function(vError) {
                },
                Success: function(vData) {
                }
            }, vParams);

            var pData = {
                CustomerID: vParams.CustomerID,
                ProductID: vParams.ProductID
            }

            AjaxSend({
                url: '/ERP/PriceHistory',
                data: pData,
                success: function(vData) {
                    if (vData.Error) {
                        vParams.Fail(vData.Error);
                    } else {
                        vParams.Success(vData);
                    }

                }
            });
        };

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

});