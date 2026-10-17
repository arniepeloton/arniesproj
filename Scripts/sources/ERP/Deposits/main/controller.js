define([
    'promise!/Scripts/sources/ERP/Deposits/work/controller.js?v=' + assetversion,
    //'promise!/Scripts/sources/ERP/Deposits/settings/controller.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var mDisbursementHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Deposits/main/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                // ERP.Deposits.main.controller = ERP.Deposits.main.controller();
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.Deposits.main.controller', null, function (vParams) {

        vParams = $.extend({
            mode: 'revenue',
            dontLoadData: false
        }, vParams);
        var mOut = {};
        var mElement;
        mElement = $(mHtml);
        mOut.element = mElement;

        if (vParams.mode != 'revenue')
            $('#divErpDeposits').append(mElement);
        else
            $('#divErpDeposits').append(mElement);

        mOut.Work = new ERP.Deposits.work.controller(vParams);
        ////var settings = new ERP.AssetsManagement.settings.controller();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});