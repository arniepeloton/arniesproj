define([

], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/FinancialDetails/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.FinancialDetails.controller', null, function () {

        var mOut = {};
        var mElement = $(mHtml);
        var mAfterSave; // function
        var mCustomerOrgID;
        var mFromEcommerce = false;
        var mFromWebPos = false;
        var mPaymentType; // cheque (default) or online-payment

        var mBankID;
        $('body').append(mElement);//.trigger('create');
        var mTxtBranch = mElement.find('[name=txtBranch]');
        var mTxtAccountName = mElement.find('[name=txtAccountName]');
        var mTxtAccountNumber = mElement.find('[name=txtAccountNumber]');
        var mSpanMessage = mElement.find('[name=spanMessage]');
        var mSpanRequired = mElement.find('[name=spanRequired]');
        var mButBank = mElement.find('[name=butBank]');

        if (proj.IsBootstrapDesign()) {
            $('#popFinancialDetailscontent > *').appendTo('#popFinancialDetailscontent-bs');
        }

        mOut.Show = function(vParams) {
            vParams = $.extend({
                AfterSave: function() {},
                FromEcommerce: false,
                PaymentType: null // cheque (default) or online-payment
            }, vParams);

            mCustomerOrgID = vParams.CustomerOrgID;
            mBankID = null;
            mFromEcommerce = vParams.FromEcommerce;
            mFromWebPos = vParams.FromWebPos;
            mPaymentType = vParams.PaymentType;
            mButBank.text('Select');
            mTxtBranch.val('');
            mTxtAccountName.val('');
            mTxtAccountNumber.val('');

            mAfterSave = vParams.AfterSave;

            if (!CBool(mFromWebPos)
                && !CBool(vParams.FromEcommerce)) {
                //NewMessageBoxClose();
            }                

            if (proj.IsBootstrapDesign()) {
                $('#popFinancialDetails-bs').modal('show');
            } else {
                $('#popFinancialDetails').show().popup('open');
            }            
        };

        mButBank.click(function() {
            window.CacPopup.BrowseBank({
                Click: function(vData) {
                    mBankID = vData.ID;
                    mButBank.text(vData.CompanyName);
                    if (proj.IsBootstrapDesign()) {
                        $('#popFinancialDetails-bs').modal('show');
                    } else {
                        $('#popFinancialDetails').popup('open');
                    }
                },
                Close: function() {
                    if (proj.IsBootstrapDesign()) {
                        $('#popFinancialDetails-bs').modal('show');
                    } else {
                        $('#popFinancialDetails').popup('open');
                    }
                }
            });
        });

        mElement.find('[name=butSave]').click(function() {
            if (!HandleVerify())
                return;

            Save();
        });

        mElement.find('[name=butCancel]').click(function () {
            if (proj.IsBootstrapDesign()) {
                $('#popFinancialDetails-bs').modal('hide');
            } else {
                $('#popFinancialDetails').popup('close');
            }
            mAfterSave();
        });


        function HandleVerify() {
            mSpanMessage.hide();
            mTxtBranch.val(mTxtBranch.val().trim());
            mTxtAccountName.val(mTxtAccountName.val().trim());
            mTxtAccountNumber.val(mTxtAccountNumber.val().trim());

            if (mTxtBranch.val() == '') {
                mSpanRequired.show();
                return false;
            }

            if (mTxtAccountName.val() == '') {
                mSpanRequired.show();
                return false;
            }

            if (mTxtAccountNumber.val() == '') {
                mSpanRequired.show();
                return false;
            }

            mSpanRequired.hide();
            return true;
        }
            
        function Save() {
            var pData = {
                CustomerOrgID: mCustomerOrgID,
                BankOrgID: mBankID,
                BankBranch: mTxtBranch.val(),
                AccountName: mTxtAccountName.val(),
                AccountNumber: mTxtAccountNumber.val(),
                FromEcommerce: mFromEcommerce,
                PaymentType: mPaymentType
            };

            AjaxSend({
                url: '/CreditAndCollection/SaveFinancialDetails',
                data: pData,
                success: function(vData) {
                    if (vData.Result == 'ok') {
                        if (proj.IsBootstrapDesign()) {
                            $('#popFinancialDetails-bs').modal('hide');
                        } else {
                            $('#popFinancialDetails').popup('close');
                        }
                        mAfterSave();
                    } else if (vData.Error) {
                        mSpanMessage.text(vData.Error);
                        mSpanMessage.show();
                    }
                        
                }
            });
        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});