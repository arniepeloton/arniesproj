define([
], function () {

    CreateStringObject('ERP.OrderProcessing.serialNumbers.model', null, function () {

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
        
        mOut.GetData = function() {
            /*
            vParams = {
                ProductID: 'required',
                InvoiceID: 'required'
            }
            */

            return new Promise(function(vResolve, vReject) {
                var pData = {
                    InvoiceID: mOut.Data.ID,
                    ProductID: mOut.Data.ProductID
                };
                AjaxSend({
                    url: '/Stock/GetSerialNumbers',
                    data: pData,
                    success: function(vData) {
                        vResolve(vData);
                    },
                    error: function(vError) {
                        vResolve({
                            Error: vError
                        });
                    }
                });
            });
        };

        mOut.Save = function(vParams) {
            
            
            return new Promise(function(vResolve, vReject) {
                var pData = {
                    InvoiceID: vParams.InvoiceID,
                    ProductID: vParams.ProductID,
                    Serials: vParams.SerialNumbers,
                    Imeis: vParams.IMEIs
                };
                AjaxSend({
                    url: '/Stock/UpdateSerialNumbers',
                    data: pData,
                    success: function(vData) {
                        vResolve(vData);
                    },
                    error: function(vError) {
                        vResolve({
                            Error: vError
                        });
                    }
                });
            }); // END return new Promise(function(vResolve, vReject) {
        }; // END mOut.Save = function(vParams) {

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

});