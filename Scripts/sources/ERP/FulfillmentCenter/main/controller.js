define([
    '/Scripts/sources/SetupChecklist/NewPromptUser.js?crc=',
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var mDisbursementHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/FulfillmentCenter/main/view.html?v' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.FulfillmentCenter.main.controller = ERP.FulfillmentCenter.main.controller()
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.FulfillmentCenter.main.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#divErpFulfillmentCenter').append(mElement).find('> .divspinner').remove();

        mOut.LoadWorkTab = function () {
            AjaxSend({
                url: '/DashboardsAndReports/SalesOrder?OrgID=' + sessionStorage.OrgID,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    $('#tabOFWork').append(vData);
                }
            });
        }

        mOut.LoadWorkTab();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});