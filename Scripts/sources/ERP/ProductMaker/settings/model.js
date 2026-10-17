define([
], function () {
    CreateStringObject('ERP.ProductMaker.settings.model', null, function () {
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

        mOut.GetInvoicingMethod = function (vParams) {
            AjaxSend({
                url: 'AssetsManagement/GetInvoicingMethod',
                data: vParams.data,
                success: function (e) {
                    vParams.success(e);
                }
            })
        }

        //mOut.GetInvoicingMethod = function (vParams) {
        //    AjaxSend({
        //        url: 'AssetsManagement/GetDoorLockSettings',
        //        data: vParams.data,
        //        success: function (e) {
        //            vParams.success(e);
        //        }
        //    })
        //}

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

        mOut.SaveDoorLockSettings = function (vParams) {
            AjaxSend({
                url: 'AssetsManagement/SaveDoorLockSettings',
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

        mOut.SaveInvoicingMethod = function (vParams) {
            AjaxSend({
                url: 'AssetsManagement/SaveInvoicingMethod',
                data: { data: vParams.data },
                success: function (e) {
                    vParams.success(e.Results);                    
                }
            })
        }

        return mOut;
    })
})