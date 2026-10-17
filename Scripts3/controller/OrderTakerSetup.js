define([
    '/Scripts3/model/OrderTakerSetup.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        if (!InAndroid) {
            mDeferred.resolve();
            return;
        }

        AjaxSend({
            ExactUrl: '/Scripts3/view/OrderTakerSetup.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    $(function () {

        CreateStringObject('App.controller.OrderTakerSetup', null, function () {

            var mOut = {};
            var mElement;
            var mPrimaryID = NewID();

            mElement = $(mHtml);
            mOut.element = mElement;
            mOut.model = new App.model.OrderTakerSetup();
            var mTxtOrderTakerSetupPosIp = mElement.find('#txtOrderTakerSetupPosIp');
            mTxtOrderTakerSetupPosIp.val(mOut.model.GetSettings().posIp);
            var mTxtOrderTakerSetupOrderTakerIps = mElement.find('#txtOrderTakerSetupOrderTakerIps');
            mTxtOrderTakerSetupOrderTakerIps.val(mOut.model.GetSettings().orderTakerIps);
            var mTxtOrderTakerSetupServerIp = mElement.find('#txtOrderTakerSetupServerIp');
            mTxtOrderTakerSetupServerIp.val(mOut.model.GetSettings().serverIp);
            var mTxtOrderTakerSetupOverrideSourceIp = mElement.find('#txtOrderTakerSetupOverrideSourceIp');
            mTxtOrderTakerSetupOverrideSourceIp.val(mOut.model.GetSettings().sourceIp);

            var chkOrderTakerSetupIsOrderTaker = mElement.find('#chkOrderTakerSetupIsOrderTaker');
            chkOrderTakerSetupIsOrderTaker.click(function() {
                mOut.model.SetSettings({
                    isOrderTaker: CBool(chkOrderTakerSetupIsOrderTaker.prop('checked'))
                });
                AfterSetOrderTaker();
            });
            chkOrderTakerSetupIsOrderTaker.prop('checked', (CBool(mOut.model.GetSettings().isOrderTaker)));
            var pDataTransmissionMethod = mOut.model.GetSettings().dataTransmissionMethod;
            mElement.find(':radio[name=radioOrderTakerSetupDataTransmissionMethod][value="' + pDataTransmissionMethod + '"]').prop('checked', true);

            function AfterSetOrderTaker() {
                if (CBool(mOut.model.GetSettings().isOrderTaker)) {
                    mElement.find('[name=divOrderTakerYes]').show();
                    mElement.find('[name=divOrderTakerNo]').hide();
                } else {
                    mElement.find('[name=divOrderTakerYes]').hide();
                    mElement.find('[name=divOrderTakerNo]').show();
                }
            }
            AfterSetOrderTaker();

            //if (InAndroid) {
            //    Transfers.StartSocketServer();
            //}

            mElement.find('#butSecondaryDisplaySetupSave').click(function(e) {
                if (mTxtSecondaryDisplaySetupClient.val().trim() == '') {
                    NewMessageBox('IP must not be blank.');
                    return;
                }

                var pMessage = {
                    command: 'SetPrimary',
                    ip: Android.IPAddress(),
                    primaryID: mPrimaryID,
                    SupplierSetting: $("[name=radioPurchaseOrderSettings]:checked").val(),
                    DataTransmissionMethod: $("[name=radioOrderTakerSetupDataTransmissionMethod]:checked").val()
                };

                try {
                    Transfers.SendMessage({
                        DestinationIp: mTxtSecondaryDisplaySetupClient.val(),
                        ClientId: 'required dummy',
                        Message: pMessage
                    });
                } catch (e) {
                    Android.AddLog(e.message);
                    NewMessageBox('Unable to connect to the primary device.');
                }
                
                return false;
            });

            Transfers.AddReceiver(function(vMessage) {
                var pJson;

                try {
                    pJson = JSON.parse(vMessage.Message);
                    if (pJson.command == 'SavedPrimaryDeviceIp') {
                        if (pJson.primaryID == mPrimaryID) {
                            mOut.model.SaveRemoteIp(mTxtSecondaryDisplaySetupClient.val());
                            NewMessageBox('Secondary device successfully accepts this device as its primary device. Secondary device IP was successfully saved.');
                        }
                    } 
                } catch (e) {
                    console.log(e);
                }
            });

            mOut.popup = function () {

                if ($('body #idOrderTakerSetup').length == 0) {
                    $('body > div:eq(0)').append(mElement);
                    //$('body').trigger('create');
                }
                $('#idOrderTakerSetup').pageshow();
                //$.mobile.changePage(mElement, {
                //    allowSamePageTransition: true
                //});
            };

            mElement.find('#btnOrderTakerBack').click(function () {
                $('#pagePopUpMenuTroubleShootingTools').pageshow();
                console.log(1)
            })

            mElement.find('[name=butOrderTakerSetupSave]').click(function() {
                if (mOut.model.GetIsOrderTaker()) {
                    if (mTxtOrderTakerSetupPosIp.val().trim() == '') {
                        NewMessageBox('POS IP address is required.');
                        return;
                    }
                    mOut.model.SavePosIp(mTxtOrderTakerSetupPosIp.val());
                } else {
                    //if (mTxtOrderTakerSetupOrderTakerIps.val().trim() == '') {
                    //    NewMessageBox('At least 1 order taker IP address is required.');
                    //    return;
                    //}
                    mOut.model.SaveOrderTakerIps(mTxtOrderTakerSetupOrderTakerIps.val());
                }
                mOut.model.SaveCommon({
                    dataTransmissionMethod: mElement.find("[name=radioOrderTakerSetupDataTransmissionMethod]:checked").val(),
                    serverIp: mTxtOrderTakerSetupServerIp.val(),
                    sourceIp: mTxtOrderTakerSetupOverrideSourceIp.val()
                });
                NewMessageBox('Settings were successfully saved.');
            });

            function DisplayIpAddress() {
                var pIpAddress = Android.IPAddress();
                mElement.find('[name=spanDeviceIP]').text(pIpAddress);
                setTimeout(DisplayIpAddress, 2000);
            }
            setTimeout(DisplayIpAddress, 1000);

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});