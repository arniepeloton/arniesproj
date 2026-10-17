define([
], function () {

    CreateStringObject('ERP.Invoicing.settings.popDocumentControl.model', null, function () {

        var mOut = {};

        mOut.Data = {
            ID: null,
            DocumentName: null,
            ModuleSource: null,
            PrintLimit: null,
            CopiesPerPrint: null
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
                url: '/ERP/GetDocumentControl',
                data: {
                    ID: mOut.Data.ID
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
                url: '/ERP/SaveDocumentControl',
                data: $.extend(mOut.Data, {
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