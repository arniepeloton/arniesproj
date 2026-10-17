define([
], function () {

    CreateStringObject('ERP.UnapprovedCustomerCreditTerms.model', null, function () {

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
                IncludeApproved: vParams.IncludeApproved,
                IncludeUnapproved: vParams.IncludeUnapproved
            };

            AjaxSend({
                url: '/CreditAndCollection/CustomerCreditTerms',
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

        mOut.GetDashboardData = function() {
        };

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    (function staticInit() {

        ERP.UnapprovedCustomerCreditTerms.model = ERP.UnapprovedCustomerCreditTerms.model(); // singleton

    })();

});