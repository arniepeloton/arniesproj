define(['common'
    ], function() {

    CreateStringObject('App.model.OrderTakerSetup', null, function() {
        
        var mOut = {};
        var mFilename = 'OrderTakerSetup.txt';
        var mSettings;

        try {
            mSettings = Transfers.ReadFile(mFilename);
            if (mSettings != null) {
                mSettings = JSON.parse(mSettings);
                if (InAndroid) {
                    Transfers.StartSocketServer();
                };
            }
        } catch (e) {
            NewMessageBox(e.message);
        }
        mSettings = $.extend({
            isOrderTaker: 0,
            orderTakerIps: '',
            posIp: '',
            dataTransmissionMethod: 'peerToPeer'
        }, mSettings);

        mOut.GetSettings = function() {
            return mSettings;
        };

        mOut.SetSettings = function(vJson) {
            mSettings = $.extend(mSettings, vJson);
        };

        mOut.GetIsOrderTaker = function() {
            return CBool(mSettings.isOrderTaker);
        };

        mOut.SaveIsOrderTaker = function(vValue) {
            SaveCommon({isOrderTaker: CBool(vValue)});
        };

        mOut.SaveOrderTakerIps = function(vValue) {
            SaveCommon({
                orderTakerIps: vValue
            });
        };

        mOut.SaveDataTransmissionMethod = function(vValue) {
            SaveCommon({
                dataTransmissionMethod: vValue
            });
        };

        mOut.SavePosIp = function(vValue) {
            SaveCommon({
                posIp: vValue
            });
        };

        mOut.SaveCommon = SaveCommon;
        function SaveCommon(vJson) {
            mSettings = $.extend(mSettings, vJson);
            var pStr = JSON.stringify(mSettings, null, 4);
            Transfers.WriteFile(mFilename, null, pStr);
        }

        mOut.SaveSettings = SaveCommon;

        Transfers.AddReceiver(function(vMessage) {
            var pJson;

            try {
                pJson = JSON.parse(vMessage.Message);
                if (pJson.command == 'SetRole') {
                    if ($.isFunction(mOut.SetRoleCallback))
                        mOut.SetRoleCallback(pJson);
                    SaveLocalRole(pJson.value);
                }
            } catch (e) {
                console.log(e);
            }
        });

        return mOut;
    });

});