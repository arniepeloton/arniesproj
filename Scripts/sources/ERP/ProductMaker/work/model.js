define([
], function () {
    CreateStringObject('ERP.ProductMaker.work.model', null, function () {
        var mOut = {};
        mOut.Data = {
            Master: {}
        };

        mOut.Save = function(vParams) {
            var pData = $.extend(mOut.Data, vParams.Data);
            AjaxSend({
                url: '/ProductMaker/Save',
                data: pData,
                success: function(vData) {
                    if (vData.Error)
                        vParams.Fail(vData.Error);
                    else // if (vData.Result == 'ok') // commented the condition 2019_01_31__16_18_14_2439
                        vParams.Success(vData);
                }
            });
        }; // END Save

        mOut.GetCart = function(vParams) {
            AjaxSend({
                url: '/ProductMaker/Cart',
                data: {
                    ID: vParams.ID,
                    DontCheckCartValidity: 1,
                    IncludeZeroQty: vParams.IncludeZeroQty,
                    IncludeSuppliers: vParams.IncludeSuppliers,
                    AccountsPayableFollowCost: vParams.AccountsPayableFollowCost
                },
                success: function(vData) {
                    vParams.Success(vData);
                }
            }); // END AjaxSend({
        }; // END GetCart

        mOut.Verify = function(vParams) {

            if (!IsGuid(mOut.Data.AssignedToID))
                return vParams.Fail('Mobiz cannot save your current transaction because: Assigned To is required.');
            
            return vParams.Success();
        }; // END Verify

        return mOut;
    })
})