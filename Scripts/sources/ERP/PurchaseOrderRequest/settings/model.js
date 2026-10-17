define([
], function () {

    CreateStringObject('ERP.Invoicing.settings.model', null, function () {

        var mOut = {};

        mOut.Data = {
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
                url: '/Stock/GetInfoPos',
                data: {
                    ProductID: mOut.Data.ProductID,
                    CustomerID: mOut.Data.CustomerID,
                    NewCartMode: mOut.Data.NewCartMode,
                    OrderID: mOut.Data.OrderID
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
                url: '/ERP/SaveInvoiceSettings',
                data: {
                    AllowEncodingOfOrdersInInvoice: vParams.AllowEncodingOfOrdersInInvoice,
                    AllowEncodingOfQuantitiesInInvoice: vParams.AllowEncodingOfQuantitiesInInvoice,
                    FreeTypeAPriceMinimumGpm: vParams.FreeTypeAPriceMinimumGpm
                },
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

        mOut.Load = function(vParams) {
            AjaxSend({
                url: '/ERP/GetInvoiceSettings',
                success: function(vData) {
                    mOut.Data.AllowEncodingOfOrdersInInvoice = vData.AllowEncodingOfOrdersInInvoice;
                    mOut.Data.AllowEncodingOfQuantitiesInInvoice = vData.AllowEncodingOfQuantitiesInInvoice;
                    vParams.callback(vData);
                }
            });
        };

        mOut.LoadDocumentControl = function(vParams) {
            AjaxSend({
                url: '/ERP/GetDocumentControl',
                data: {
                    NoPo: true
                },
                success: function(vData) {
                    vParams.callback(vData);
                }
            });
        };

        mOut.SetAllCustomerUnlimitedCredit = function(vParams) {
            AjaxSend({
                url: '/ERP/SetAllCustomerUnlimitedCredit',
                data: {
                    value: vParams.value
                },
                success: function(vData) {
                    vParams.callback(vData);
                }
            });
        };

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

});