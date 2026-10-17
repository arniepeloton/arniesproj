define([
    'promise!/Scripts/sources/ERP/ProductMaker/settings/controller.js',    
    'promise!/Scripts/sources/ERP/ProductMaker/dashboard/controller.js',
    'promise!/Scripts/sources/ERP/ProductMaker/work/controller.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var mDisbursementHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ProductMaker/main/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.ProductMaker.main.controller = ERP.ProductMaker.main.controller()
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.ProductMaker.main.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#divErpProductMaker').append(mElement);

        var mSettings = new ERP.ProductMaker.settings.controller();
        var mDashboard = new ERP.ProductMaker.dashboard.controller();
        var mWork = new ERP.ProductMaker.work.controller();

        $('#tabProductMakerSettings').off('click').click(function () {
            NewPromptUserAccess('Product Maker Settings', {
                OnClickOKSuccess: function () {
                    $('#tabProductMakerSettings').opentabmenu();
                }
            });
        })

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});