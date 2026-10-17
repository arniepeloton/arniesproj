define([
    '/Scripts3/model/PeerConnect.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts3/view/SecondaryDisplaySetup.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    $(function () {

        CreateStringObject('App.controller.SecondaryDisplaySetup', null, function () {

            var mOut = {};
            var mElement;
            var mPrimaryID = NewID();

            mElement = $(mHtml);
            mOut.element = mElement;
            mOut.model = new App.model.PeerConnect();

            var mTxtSecondaryDisplaySetupClient = mElement.find('#txtSecondaryDisplaySetupClient');
            mTxtSecondaryDisplaySetupClient.val(mOut.model.GetSettings().remoteIp);

            if (InAndroid) {
                Transfers.StartSocketServer();
            }

            mElement.find('#butSecondaryDisplaySetupSave').click(function(e) {
                if (mTxtSecondaryDisplaySetupClient.val().trim() == '') {
                    NewMessageBox('IP must not be blank.');
                    return;
                }

                var pMessage = {
                    command: 'SetPrimary',
                    ip: Android.IPAddress(),
                    primaryID: mPrimaryID
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

                if (mElement.parent().length == 0) {
                    $('body').append(mElement);
                    //$('body').trigger('create');
                }

                $.mobile.changePage(mElement, {
                    allowSamePageTransition: true
                });
            };

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});