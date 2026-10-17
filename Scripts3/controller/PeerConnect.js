define([
    '/Scripts3/model/PeerConnect.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts3/view/PeerConnect.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    $(function () {

        CreateStringObject('App.controller.PeerConnect', null, function () {

            var mOut = {};
            var mElement;
            
            mElement = $(mHtml);
            mOut.element = mElement;
            mOut.model = new App.model.PeerConnect();
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
                try {
                    Transfers.SendMessage({
                        DestinationIp: mTxtPeerConnectServer.val(),
                        ClientId: 'required dummy',
                        Message: {
                            command: 'SetRole',
                            value: mCbPeerConnectRemoteRole.val()
                        }
                    });
                } catch (e) {
                    NewMessageBox(e.message);
                }
                mOut.model.SaveRemoteIp(mTxtPeerConnectServer.val());
                return false;
            });

            mOut.popup = function () {

                if (mElement.parent().length == 0) {
                    $('body').append(mElement);
                    //$('body').trigger('create');
                }

                $.mobile.changePage(mElement);

                //mElement.popup('open');
                //pElement = mElement.closest('.ui-popup-container');
                //if (pElement.length == $('body')0)
                //    console.trace('popup container for divRequestPrice not found');
                //else
                //// known issue: pop up dimension will revert to default after browser window resize
                //    pElement.css({
                //        left: 0,
                //        width: '80%',
                //        margin: 'auto 10%',
                //        padding: 15
                //    });
            };

            Transfers.AddClientReceiver(function(vMessage) {
                Sender.ReceiveText(vMessage);
            });
            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});