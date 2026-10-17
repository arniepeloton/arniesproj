define([
], function () {
    CreateStringObject('ERP.Deposits.work.model', null, function (vParams) {
        var mOut = {};

        vParams = $.extend({
            mode: 'revenue'
        }, vParams);
        var mMode = vParams.mode; // revenue or disbursement

        mOut.GetData = function (vParams) {
            var pData = vParams.data;
            pData.PaymentType = 'check';
            var pUrl = '/Browser/CustomerPaymentReport';
            if (mMode == 'disbursement')
                pUrl = '/CreditAndCollection/SupplierPaymentReport';
            AjaxSend({
                url: pUrl,
                data: pData,
                success: function (e) {
                    if (e.Error) {
                        console.log(e.Error);
                        return;
                    }
                    var vRows = e.Results.Rows;
                    vParams.success(vRows);
                }
            });
        };

        mOut.SaveData = function (vParams) {
            AjaxSend({
                url: '/CreditAndCollection/SaveDeposits',
                data: {
                    Value: vParams.value,
                    DepositoryBankID: vParams.DepositoryBankID,
                    DepositoryBankBranch: vParams.DepositoryBankBranch,
                    AccountName: vParams.AccountName,
                    AccountNumber: vParams.AccountNumber,
                    DepositDate: vParams.DepositDate,
                    Data: vParams.data,
                    Mode: mMode
                },
                success: function (e) {
                    vParams.success(e);
                }
            });
        };

        mOut.SaveAccountNameAndNumber = function(vParams) {
            AjaxSend({
                url: '/Erp/SaveAccountNameAndNumber',
                data: {
                    AccountName: vParams.AccountName,
                    AccountNumber: vParams.AccountNumber
                },
                success: function (e) {
                    vParams.success(e);
                }
            });
        };

        return mOut;
    });
})