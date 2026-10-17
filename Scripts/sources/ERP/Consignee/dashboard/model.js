define([
], function () {

    CreateStringObject('ERP.OrderProcessing.dashboard.model', null, function () {

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

        mOut.GetCollectionReport = function (vParams) {
            var pData = vParams.dataDateRange;
            pData.FowstID = sessionStorage.FowstID;
            AjaxSend({
                url: '/Browser/GetCollectionReport',
                data: pData,
                success: function (vData) {
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
                    AllowEncodingOfQuantitiesInInvoice: vParams.AllowEncodingOfQuantitiesInInvoice
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
                    vParams.callback(vData);
                }
            });
        };

        mOut.CncApproveSalesOrder = function(vParams) {
            AjaxSend({
                url: '/CreditAndCollection/CncApproveSalesOrder',
                data: {
                    InvoiceID: vParams.ID
                },
                success: function(vData) {
                    if (vData.Error) 
                        vParams.Fail(vData.Error);
                    else
                        vParams.Success();
                }
            });
        };

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

});