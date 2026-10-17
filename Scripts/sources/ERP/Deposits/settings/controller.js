define([
    'promise!/Scripts/sources/ERP/AssetsManagement/settings/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/AssetsManagement/settings/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.AssetsManagement.settings.controller', null, function () {

        var mOut = {};
        var mElement, settingsModel = new ERP.AssetsManagement.settings.model();

        mElement = $(mHtml);
        mOut.element = mElement;

        GetAssetSetting();

        $('#tabAssetsMgmtSettings').append(mHtml).trigger('create');

        $('#btnSaveAssetSettings').click(function () {
            settingsModel.SaveSettings({
                data: {
                    CheckOutTimeID: $.trim($('#txtCheckOutTimeID').val()),
                    CheckOutTime: $('#selCOhour').val() + ":" + $('#selCOminute').val() + " " + $('#selCOmeredian').val()
                },
                success: function (e) {
                    WebToast('Settings has been successfully saved', 'top');
                    GetAssetSetting();
                }
            })
        })

        function GetAssetSetting() {
            settingsModel.GetAssetSetting({
                success: function (e) {
                    mOut.Clear();
                    
                    if(e.DefaultCheckOutTime.length > 0){
                        var time = (e.DefaultCheckOutTime[0].DefaultValue).split(':');
                        $('#txtCheckOutTimeID').val(e.DefaultCheckOutTime[0].ID);
                        $('#selCOhour').val(time[0]);
                        $('#selCOminute').val(time[1]);
                        $('#selCOmeredian').val((e.DefaultCheckOutTime[0].DefaultValue).split(' ')[1]);
                    }
                }
            })
        }

        mOut.Clear = function () {
            $('#txtCheckOutTimeID').val('');
            $('#selCOhour').val('12');
            $('#selCOminute').val('00');
            $('#selCOmeredian').val('PM');
        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});