define([
    'common'
], function() {

    var mDeferred = $.Deferred();

    CreateStringObject('App.model.Receiving', null, function() {
        
        var pOut = {};

        pOut.OfflineMode = false;
        pOut.isPageIdle = false;

        pOut.GetFowst = function(vParams) {
            /***
            vParams = {
                successFiltered: function(vData) {
                },
                success: function(vData) {
                }
            };
            ***/

            if (pOut.OfflineMode) {
                App.model.Receiving_offline.GetFowst(vParams);
                return;
            }

            var data = { FilterFowst: 1 };
            
            AjaxSend({
                url: 'Browser/LibraryFowst',
                data: data,
                success: function (vData) {
                    vParams.successFiltered(vData);
                }
            });

            AjaxSend({
                url: 'Browser/LibraryFowst',
                success: function (vData) {
                    vParams.success(vData);
                }
            });
        }; // END pOut.GetFowst = function(vParams) {

        pOut.GetReceivingOrTransfersInfo = function (vParams) {
            /***
            vParams = {
                data: 'string',
                success: function(vData) {
                }
            };
            ***/
            if (OfflineMode) {

                return App.model.Receiving_offline.GetReceivingOrTransfersInfo(vParams);

            } else {
                AjaxSend({
                    url: 'Receiving/GetAllReceiving',
                    data: { data: vParams.data },
                    success: function (vData) {
                        vParams.success(vData);
                    }
                });
            }
            
        }; // END pOut.GetReceivingOrTransfersInfo = function(m_data) {

        pOut.GetReceivingDetails = function (vParams) {
            /***
            vParams = {
                data: 'string',
                success: function(vData) {
                }
            };
            ***/
            if (OfflineMode) {

                return App.model.Receiving_offline.GetReceivingDetails(vParams);

            } else {
                AjaxSend({
                    url: 'Receiving/GetAllReceivingSKU',
                    data: { data: vParams.data },
                    success: function (vData) {
                        vParams.success(vData);
                    }
                });
            }

        }; // END pOut.GetReceivingOrTransfersInfo = function(m_data) {

        pOut.Save = function(vParams) {
            /***
            vParams = {
                data: {},
                success: function(vData) {
                }
            };
            ***/

            var pResult;

            if (pOut.OfflineMode) {
                pResult = App.model.Receiving_offline.Save(vParams);
                vParams.success({Save: pResult});
                return;
            }

            AjaxSend({
                url: 'Receiving/SaveReceiving',
                data: vParams.data,
                success: function (vData) {
                    vParams.success(vData);
                }
            });
        }; // END pOut.Save = function(vParams) {

        return pOut;

    }); // END CreateStringObject('App.model.Receiving', null, function() {

    App.model.Receiving = App.model.Receiving(); // singleton

    if (OfflineMode) {
        require(['/Scripts3/model/Receiving_offline.js?crc='], function() {
            mDeferred.resolve();
        });
        return mDeferred.promise();
    }

}); // END require
