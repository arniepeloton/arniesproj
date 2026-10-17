define([
    'promise!/Scripts/sources/ERP/AssetsManagement/settings/controller.js?v=' + assetversion,    
    'promise!/Scripts/sources/ERP/AssetsManagement/dashboard/controller.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/AssetsManagement/work/controller.js?v=' + assetversion,
    '/Scripts/sources/SetupChecklist/NewPromptUser.js?crc=',
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var mDisbursementHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/AssetsManagement/main/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.AssetsManagement.main.controller = ERP.AssetsManagement.main.controller()
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.AssetsManagement.main.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#divErpAssetsManagement').append(mElement).find('> .divspinner').remove();

        window.AssetMgmtSettingsController = new ERP.AssetsManagement.settings.controller();
        window.AssetMgmtDashboardController = new ERP.AssetsManagement.dashboard.controller();
        window.AssetMgmtWorkController = new ERP.AssetsManagement.work.controller();

        $('#btnTabAssetsMgmtSettings').off('click').click(function () {
            PromptUserAccess('Assets Management Settings', {
                OnClickOKSuccess: function () {
                    $('#tabAssetsMgmtSettings').opentabmenu();
                }
            });
        })

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});