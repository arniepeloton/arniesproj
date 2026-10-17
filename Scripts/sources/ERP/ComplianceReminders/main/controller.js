define([
    'promise!/Scripts/sources/ERP/ComplianceReminders/dashboard/controller.js?v=' + assetversion,
    '/Scripts/sources/SetupChecklist/NewPromptUser.js?crc=',
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var mDisbursementHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ComplianceReminders/main/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.ComplianceReminders.main.controller = ERP.ComplianceReminders.main.controller()
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.ComplianceReminders.main.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#divErpComplianceReminders').append(mElement).find('> .divspinner').remove();

        var dashboard = new ERP.ComplianceReminders.dashboard.controller();

        mElement.find('#btnCRWork').click(function () {
            NewPromptUserAccess('Add/Edit Compliance Reminders', {
                Message: 'You\'re not allowed to add/edit Compliance Reminders',
                OnClickOKSuccess: function () {
                    $('#tabComplianceRemindersWork').opentabmenu();
                    require(['promise!/Scripts/sources/ERP/ComplianceReminders/work/controller.js?v=' + assetversion])
                },
                OnFailed: function () {
                    console.log('jsCFDTPD3GXC!');
                }
            });            
        })

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});