define([
    //'promise!/Scripts/sources/ERP/RentableServices/settings/controller.js?v=' + assetversion,    
    'promise!/Scripts/sources/ERP/RentableServices/playroom/controller.js?v=' + assetversion,
    //'promise!/Scripts/sources/ERP/RentableServices/work/controller.js?v=' + assetversion,
    '/Scripts/sources/SetupChecklist/NewPromptUser.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var mDisbursementHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/RentableServices/main/view.html?v' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.RentableServices.main.controller = ERP.RentableServices.main.controller()
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.RentableServices.main.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#divErpRentableServices').append(mElement).find('.divspinner').remove();

        $('#btnRentableServicesDashboard').click(function () {
            require(['promise!/Scripts/sources/ERP/RentableServices/dashboard/controller.js?v=' + assetversion], function () {
                ERP.RentableServices.dashboard.controller().Initialize();
            })
        })

        //window.AssetMgmtSettingsController = new ERP.AssetsManagement.settings.controller();
        window.RentableServicesPlayRoomController = new ERP.RentableServices.playroom.controller();
        RentableServicesPlayRoomController.GetPlayers();
        //window.AssetMgmtWorkController = new ERP.AssetsManagement.work.controller();

        //$('#btnTabAssetsMgmtSettings').off('click').click(function () {
        //    PromptUserAccess('Assets Management Settings', {
        //        OnClickOKSuccess: function () {
        //            $('#tabAssetsMgmtSettings').opentabmenu();
        //        }
        //    });
        //})

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});