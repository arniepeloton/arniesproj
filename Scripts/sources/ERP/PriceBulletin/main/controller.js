define([
    'promise!/Scripts/sources/ERP/PriceBulletin/settings/controller.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/PriceBulletin/dashboard/controller.js?v=' + assetversion    
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var mDisbursementHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/PriceBulletin/main/view.html?v' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.PriceBulletin.main.controller = ERP.PriceBulletin.main.controller()
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.PriceBulletin.main.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#divErpPriceBulletin').append(mElement).find('.divspinner').remove();        

        mOut.Settings = new ERP.PriceBulletin.settings.controller();
        mOut.Dashboard = new ERP.PriceBulletin.dashboard.controller();        

        mElement.find('[name="btnPriceBulletinDashboard"]').click(function () {
            mOut.Dashboard.GetPriceBulletinList();
        })

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});