define([
    //'promise!/Scripts/sources/ERP/Dis/work/controller.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Disbursement/main/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.OrderProcessing.main.controller = ERP.OrderProcessing.main.controller(); // singleton
                LoadDisbursement();
            }
        });

        function LoadDisbursement() {
            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/OrderProcessing/main/view.html',
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mHtml = vData;
                    ERP.OrderProcessing.main.controller = ERP.OrderProcessing.main.controller(); // singleton
                    mDeferred.resolve();
                }
            });
        }
    })();

    CreateStringObject('ERP.OrderProcessing.main.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#divErpRevenue').append(mElement);
        //$('#divOrderProcessingMain').append(ERP.OrderProcessing.work.controller.element);

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});