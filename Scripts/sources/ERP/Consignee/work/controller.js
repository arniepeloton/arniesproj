define([
    '/Scripts/sources/ERP/Consignee/work/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var mSalesOrderHtml;
    var mInvoiceHtml;
    var mStockReceivingHtml;
    var mStockTransferHtml;
    var mPurchaseOrderRequestHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Consignee/work/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.Consignee.work.controller', null, function (vParams) {

        var mOut = {};
        var mElement;
        
        mElement = $(mHtml);
        mOut.element = mElement;

        $('#tabConsigneeWork').append(mElement).trigger('create');

        $('#butNewConsignee').click(function () {
            NewMessageBox('Hello World');
        })

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});