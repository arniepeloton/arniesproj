define([
    'promise!/Scripts/jquery-ui-sortable.js',
    'promise!/Scripts/sources/ERP/PriceCanvassing/settings/controller.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/PriceCanvassing/work/controller.js?v=' + assetversion,
    '/Scripts/sources/ERP/PriceCanvassing/dashboard/controller.js?v=' + assetversion,
    '/Scripts/sources/SetupChecklist/NewPromptUser.js?v=' + assetversion,
], function () {

    var mDeferred = $.Deferred();
    var mHtml, workController = null, settingsController = null;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/PriceCanvassing/main/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.PriceCanvassing.main.controller', null, function () {

        var mOut = {};
        var mElement;
        var mPriceCanvassType;

        mElement = $(mHtml);
        mOut.element = mElement;

        mOut.LoadPage = function () {
            mPriceCanvassType = sessionStorage.PriceCanvassType;
            if (mPriceCanvassType == 'vendor') {
                $('#divPriceCanvassMain').empty().append(mElement);
            } else {
                $('#divRevenuePriceCanvassMain').empty().append(mElement);
            }
        }

        mOut.LoadPage();

        if (workController == null) {
            workController = ERP.PriceCanvassing.work.controller();
        } else {
            workController.LoadPage();
        }

        if (settingsController == null) {
            settingsController = ERP.PriceCanvassing.settings.controller();
        } else {
            settingsController.LoadPage();
        }

        ERP.PriceCanvassing.dashboard.controller();

        //mElement.find('#btnTabPriceCanvassWork').on('click', function () {
        //    PriceCanvass.DisplayPage()
        //})

        $('body').on('click', '#btnTabPriceCanvassSettings', function () {
            var pAccess;

            if (mPriceCanvassType == 'vendor')
                pAccess = 'Vendor Price Canvass Settings';
            else
                pAccess = 'Access Competitor Price Canvassing Settings*';

            NewPromptUserAccess(pAccess, {
                OnClickOKSuccess: function () {
                    $('#tabPriceCanvassSettings').opentabmenu();
                }
            });
        })

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});