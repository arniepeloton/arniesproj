define([
], function () {
    CreateStringObject('ERP.AssetsManagement.settings.model', null, function () {
        var mOut = {};

        mOut.GetAssetSetting = function (vParams) {
            AjaxSend({
                url: 'AssetsManagement/GetAssetSetting',
                data: { data: vParams.data },
                success: function (e) {
                    vParams.success(e);
                }
            })
        }

        mOut.SaveSettings = function (vParams) {
            AjaxSend({
                url: 'AssetsManagement/SaveSettings',
                data: { data: vParams.data },
                success: function (e) {
                    if (e.Results == "saved") {
                        if (vParams.success != undefined) {
                            vParams.success(e.Results);
                        }
                    } else {
                        WebToast(e.Results, 'top');
                    }                    
                }
            })
        }

        return mOut;
    })
})