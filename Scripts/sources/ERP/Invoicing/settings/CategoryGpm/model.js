define([
], function () {

    CreateStringObject('ERP.Invoicing.settings.CategoryGpm.model', null, function () {

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
                url: '/Browser/LibraryPimCategoryGpm',
                data: {
                    
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

        mOut.SaveCategoryGpm = function(vParams) {
            AjaxSend({
                url: '/ERP/SaveCategoryGpm',
                data: {
                    Category: vParams.Category,
                    Gpm: vParams.Gpm
                },
                success: function(vData) {
                    if (vData.Error) {
                        vParams.Fail(vData.Error);
                    } else if (vData.Result == 'ok') {
                        vParams.Success(vData);
                    } else {
                        NewMessageBox('Unknown status. ref: jsR5Q7SYB9UB!');
                    }

                }
            });
        };

        mOut.DeleteCategoryGpm = function(vParams) {
            AjaxSend({
                url: '/ERP/DeleteCategoryGpm',
                data: {
                    Categories: vParams.Data
                },
                success: function(vData) {
                    if (vData.Error) {
                        vParams.Fail(vData.Error);
                    } else if (vData.Result == 'ok') {
                        vParams.Success(vData);
                    } else {
                        NewMessageBox('Unknown status. ref: jsR5Q7SYB9UB!');
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

        mOut.LoadDocumentControl = function(vParams) {
            AjaxSend({
                url: '/ERP/GetDocumentControl',
                success: function(vData) {
                    vParams.callback(vData);
                }
            });
        };

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

});