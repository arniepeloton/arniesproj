define([
    '/Scripts3/model/PeerConnect.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts3/view/SecondScreenConnect.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    $(function () {

        CreateStringObject('App.controller.SecondScreenConnect', null, function () {

            var mOut = {};
            var mElement;
            
            mElement = $(mHtml);
            mOut.element = mElement;
            mOut.model = new App.model.PeerConnect();

            if (InAndroid) {
                Transfers.StartSocketServer();
            }

            var mTxtPeerConnectServer = mElement.find('#txtPeerConnectServer');

            var mCbPeerConnectLocalRole = mElement.find('#cbPeerConnectLocalRole');
            var mCbPeerConnectRemoteRole = mElement.find('#cbPeerConnectRemoteRole');

            mOut.model.SetRoleCallback = function(vJson) {
                mCbPeerConnectLocalRole.val(vJson.value).selectmenu('refresh', true);;
                NewMessageBox('This device\'s role was set to ' + vJson.value + ' by a remote device');
            };
            
            mCbPeerConnectLocalRole.val(mOut.model.GetSettings().localRole); //.selectmenu('refresh', true);;
            mTxtPeerConnectServer.val(mOut.model.GetSettings().remoteIp);

            mElement.find('#butPeerConnectLocalRole').click(function() {
                mOut.model.SaveLocalRole(mCbPeerConnectLocalRole.val());
                return false;
            });

            mElement.find('#butPeerConnectRemoteRole').click(function(e) {
                mOut.model.SaveRemoteIp(mTxtPeerConnectServer.val());
                NewMessageBox('Settings were successfully saved.');
                return false;
            });

            mElement.find('#butSecondScreenConnectScan').click(function() {
                mobile.Scan({
                    Success: function (vData) {
                        mTxtPeerConnectServer.val(vData.Value);
                        $(mElement.find('#butPeerConnectRemoteRole')).click();
                    }
                });
            });

            function Init() {
                var pIpAddress = Android.IPAddress();
                if (CStr2(pIpAddress).trim() == '') {
                    $('#divSecondScreenConnectInstructionsNoIp').show();
                    $('#divSecondScreenConnectInstructions').hide();
                } else {
                    $('#divSecondScreenConnectInstructionsNoIp').hide();
                    $('#spanSecondScreenConnectInstructionsIp').text(pIpAddress);
                    $('#divSecondScreenConnectInstructions').show();
                }
            }

            Transfers.AddReceiver(function(vMessage) {
                var pJson;

                try {
                    pJson = JSON.parse(vMessage.Message);
                    if (pJson.command == 'SetPrimary') {
                        SendPrimaryConfirm(pJson);
                    }
                } catch (e) {
                    console.log(e);
                    Android.AddLog(e.message);
                    NewMessageLog(e.message);
                }
            });

            function SendPrimaryConfirm(vJson) {
                mOut.model.SavePrimaryDeviceIp(vJson.ip);
                var pMessage = {
                    command: 'SavedPrimaryDeviceIp',
                    primaryID: vJson.primaryID
                };
                Transfers.SendMessage({
                    DestinationIp: vJson.ip,
                    ClientId: vJson.primaryID,
                    Message: pMessage
                });
                NewMessageBox('You will now be able to get data from ' + vJson.ip + '.');
            }

            mOut.popup = function () {

                if (mElement.parent().length == 0) {
                    $('body').append(mElement);
                    //$('body').trigger('create');
                }

                $.mobile.changePage(mElement, {
                    allowSamePageTransition: true
                });
                Init();
              
            };
            
            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});