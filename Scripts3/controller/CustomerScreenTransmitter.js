define(['common',
    '/Scripts3/model/PeerConnect.js?crc='
], function () {

    var mDeferred = $.Deferred();

    $(function () {

        CreateStringObject('App.controller.CustomerScreenTransmitter', null, function () {

            var mOut = {};
            var mClients = [];
            var pTimeoutHandle;
            var mRemoteIp;

            if (InAndroid) {
                mOut.model = new App.model.PeerConnect();
                mRemoteIp = mOut.model.GetSettings().remoteIp;
            };

            Transfers.AddClientReceiver(OnReceiver);

            Transfers.AddReceiver(OnReceiver);

            function OnReceiver(vMessage) {
                var pJson;

                try {
                    pJson = JSON.parse(vMessage.Message);
                    if (pJson.command == 'CustomerScreenInit') {
                        mClients.push(vMessage.ClientId);
                    }
                } catch (e) {
                    console.log(e);
                }
            }

            //Transfers.AddOnClientConnected(function(vData) {
        
            //});

            mOut.AddProduct = function(vParams) {
                if (pTimeoutHandle != null)
                    clearTimeout(pTimeoutHandle);
                if (CStr2(mRemoteIp).trim() == '')
                    return;
                var pMessageID = NewID();
                var pData = $.extend(vParams, {
                    command: 'CustomerScreenItem',
                    MessageID: pMessageID,
                    ip: Android.IPAddress()
                });
                //for (var p = 0; p <= mClients.length - 1; p++)
                //    Transfers.SendMessageToClient(pData, mClients[p]);
                try {
                    Transfers.SendMessage({
                        DestinationIp: mRemoteIp,
                        ClientId: 'secondary',
                        Message: pData
                    });
                } catch (e) {
                }

                //pTimeoutHandle = setTimeout(function() {
                //    mOut.AddProduct(vParams);
                //}, 2000);
            };

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});