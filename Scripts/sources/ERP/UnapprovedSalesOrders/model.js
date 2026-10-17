define([
], function () {

    CreateStringObject('ERP.UnapprovedSalesOrders.model', null, function () {

        var mOut = {};
        
        mOut.GetWithCnCUnapprovedReason = function(vParams) {
            /*
            vParams = {
                Success: function(vArray) {
                },
                Fail: function(vMessage) {
                }
            }
            */

            var pData = {
                WithCnCUnapprovedReason: vParams.WithCnCUnapprovedReason,
                WithApprovedCnC: vParams.WithApprovedCnC,
                WithCnCDisapproved: vParams.WithCnCDisapproved,
                ShowChildSos: 1,
                TransactionType: vParams.TransactionType
            };
            if (!CBool(pData.WithCnCUnapprovedReason)
                && !CBool(pData.WithApprovedCnC))
                pData.WithCnCUnapprovedReason = true;
            if (vParams.Summary)
                pData.WithCnCUnapprovedReasonSummary = 1;
            if (CBool(vParams.WithCnCDisapproved))
                pData.IsCancelled = 1;

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

        mOut.CncDisapproveSalesOrder = function(vParams) {
            AjaxSend({
                url: '/CreditAndCollection/CncDisapproveSalesOrder',
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

        ERP.UnapprovedSalesOrders.model = ERP.UnapprovedSalesOrders.model(); // singleton

    })();

});