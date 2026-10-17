define([
    
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/FinancialStatement/work/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.FinancialStatement.work.controller', null, function () {

        var mOut = {};

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#tabFinacialStatementWork').empty().append(mElement).trigger('create');

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});