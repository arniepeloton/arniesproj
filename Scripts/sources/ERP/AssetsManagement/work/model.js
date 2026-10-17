define([
], function () {
    CreateStringObject('ERP.AssetsManagement.work.model', null, function () {
        var mOut = {};

        mOut.ListOfAssets = null;

        mOut.GetListOfAssets = function (vParams) {
            AjaxSend({
                url: 'AssetsManagement/GetListOfAssets',
                data: { data: vParams.data },
                success: function (e) {
                    var vRows = e.Results.Rows;
                    vParams.success(vRows);

                    //mOut.GetAssetsSchedule();
                }
            })
        }

        mOut.Save = function (vParams) {
            AjaxSend({
                url: 'AssetsManagement/SaveAssetNow',
                data: { data: $.extend(vParams.data, { DateNow: $.format.date(new Date(), 'MMM dd, yyyy'), TimeNow: $.format.date(new Date(), 'hh:mm a') }) },
                success: function (e) {
                    if (IsGuid(e.Results) || e.Results == 'saved') {
                        if (vParams.success != undefined) {
                            vParams.success(e.Results);
                        }
                    } else {
                        WebToast(e.Results, 'top');
                    }                    
                }
            })
        }

        mOut.GetAssetsSchedule = function () {
            AjaxSend({
                url: 'AssetsManagement/GetAssetsSchedule',
                success: function (e) {
                    console.log(e)
                }
            })
        }

        mOut.GetAssetRates= function (vParams) {
            AjaxSend({
                url: 'Organization/getOrganizationProductPriceList',
                data: vParams.data,
                success: function (e) {
                    vParams.success(e);
                }
            })
        }

        mOut.GetExistingCustomerDetails = function (vParams) {
            AjaxSend({
                url: 'AssetsManagement/GetProspectDetailsPeople',
                data: vParams.data,
                success: function (e) {
                    vParams.success(e);
                }
            })
        }

        mOut.SaveAssetCustomer = function (vParams) {
            AjaxSend({
                url: 'AssetsManagement/SaveAssetNow',
                data: { data: $.extend(vParams.data, { DateNow: $.format.date(new Date(), 'MMM dd, yyyy'), TimeNow: $.format.date(new Date(), 'hh:mm a') }) },
                success: function (e) {
                    vParams.success(e);
                }
            })
        }

        mOut.LibraryAssets = function (vParams) {
            if (mOut.ListOfAssets == null) {
                AjaxSend({
                    url: 'AssetsManagement/LibraryAssets',
                    data: vParams.data,
                    success: function (e) {
                        if (e == null) {
                            vParams.success([]);
                        } else {                            
                            vParams.success(e.Results);
                            mOut.ListOfAssets = e.Results;
                        }
                    }
                })
            } else {
                vParams.success(mOut.ListOfAssets);
            }
        }

        mOut.SearchClient = function (vParams) {
            AjaxSend({
                url: 'AssetsManagement/SearchClient',
                data: vParams.data,
                success: function (e) {
                    vParams.success(e.Results);
                }
            })
        }

        mOut.SaveCardCreation = function (vParams) {
            AjaxSend({
                url: 'AssetsManagement/SaveCardCreation',
                data: { data: vParams.data },
                success: function (e) {
                    vParams.success(e);
                }
            })
        }

        mOut.FollowupCardCreation = function (vParams) {
            AjaxSend({
                url: 'AssetsManagement/FollowupCardCreation',
                data: { data: vParams.data },
                success: function (e) {
                    vParams.success(e);
                }
            })
        }

        return mOut;
    })
})