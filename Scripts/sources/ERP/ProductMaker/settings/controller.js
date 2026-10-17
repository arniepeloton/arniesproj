define([
    'promise!/Scripts/sources/ERP/ProductMaker/settings/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ProductMaker/settings/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.ProductMaker.settings.controller', null, function () {

        var mOut = {};
        var mElement;
        var mModel = new ERP.ProductMaker.settings.model();

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#tabProductMakerSettings').empty().append(mElement).trigger('create');

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});