define([
], function () {

    CreateStringObject('ERP.SalesReturn.work.model', null, function () {

        var mOut = {};
        mOut.Data = {};
        var mCheckOutVerify;
        var mMemoID;

        mOut.GetInvoice = function(vParams) {
            AjaxSend({
                url: '/SalesMenu/PosCart',
                data: {
                    OrderID: vParams.InvoiceID,
                    DontCheckCartValidity: 1
                },
                success: function(vData) {
                    vParams.Success(vData);
                }
            }); // END AjaxSend({
        }; // END GetCart

        mOut.GetContents = function(vParams) {
            AjaxSend({
                url: '/SalesReturns/GetContents',
                data: {
                    InvoiceID: vParams.InvoiceID
                },
                success: function(vData) {
                    mMemoID = vData.MemoID;
                    vParams.Success(vData);
                }
            }); // END AjaxSend({
        }; // END GetCart

        mOut.Verify = function(vParams) {
            if (CStr2(mOut.Data.CustomerID).trim() == '') {
                return vParams.Fail('Customer must not be blank.');
            }
            if (!IsGuid(mOut.Data.CustomerID))
                return vParams.Fail('CustomerID must be a valid GUID.');
            
            return vParams.Success();
        }; // END Verify

        mOut.VerifyOnline = function(vParams) {
            AjaxSend({
                url: 'SalesReturns/Verify',
                data: { 
                    MemoID: mMemoID
                },
                success: function (vData) {
                    if ((vData) && (vData.Result == 'ok')) {
                        mCheckOutVerify = vData;
                        vData.Message = 'You are about to check out this sales return cart. Please verify the following info: Total Returned Amount: ' + $.format.number(CDec2(vData.TotalAmount), '#,###.00') + '; SKU Count: ' + vData.SkuCount + '.';
                        vParams.success(vData);
                    } else
                        NewMessageBox(vData.Error);
                },
                error: function (vError) {
                    NewMessageBox(vError);
                }
            }); // AjaxSend
        };

        mOut.CheckOut = function(vParams) {

            var pData = { 
                MemoID: mMemoID,
                TotalAmount: mCheckOutVerify.TotalAmount,
                SkuCount: mCheckOutVerify.SkuCount,
                Refund: 1
            };
            pData = $.extend(pData, vParams.data);

            AjaxSend({
                url: 'SalesReturns/CheckOut',
                data: pData,
                success: function (vData) {
                    if ((vData) && (vData.Result == 'ok')) {
                        vParams.Success(vData);
                    } else
                        NewMessageBox(vData.Error);
                },
                error: function(vData) {
                    NewMessageBox(vData);
                }
            }); // AjaxSend                    

        }; // function CheckOut() {

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

});