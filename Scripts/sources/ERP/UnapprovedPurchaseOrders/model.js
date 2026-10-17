define([
], function () {

    CreateStringObject('ERP.UnapprovedPurchaseOrders.model', null, function () {

        var mOut = {};
        
        mOut.GetUnapproved = function(vParams) {
            /*
            vParams = {
                Success: function(vArray) {
                },
                Fail: function(vMessage) {
                }
            }
            */

            var pData = {
                TransType: 'PurchaseOrder',
                FinalizedPoOnly: 1
            };
            AjaxSend({
                url: '/Browser/TransInvoices',
                data: pData,
                success: function(vData) {
                    if (vData.Error) {
                        vParams.Fail(vData.Error);
                    } else {
                        vParams.Success(vData);
                    }
                }
            });
        }

        mOut.Approve = function(vParams) {
            AjaxSend({
                url: '/Erp/ApprovePo',
                data: {
                    ID: vParams.ID
                },
                success: function(vData) {
                    if (vData.Error) 
                        vParams.Fail(vData.Error);
                    else
                        vParams.Success();
                }
            });
        };

        mOut.GetDashboardData = function() {
        };

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    (function staticInit() {

        ERP.UnapprovedPurchaseOrders.model = ERP.UnapprovedPurchaseOrders.model(); // singleton

    })();

});