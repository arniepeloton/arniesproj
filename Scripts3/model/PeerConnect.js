define(['common'
    ], function() {

    CreateStringObject('App.model.PeerConnect', null, function() {
        
        var mOut = {};
        var mFilename = 'PeerConnect.txt';
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
            localRole: '',
            remoteIp: ''
        }, mSettings);

        mOut.GetSettings = function() {
            return mSettings;
        };

        mOut.SaveLocalRole = function(vValue) {
            SaveCommon({localRole: vValue});
        };

        mOut.SaveRemoteIp = function(vValue) {
            SaveCommon({remoteIp: vValue});
        };

        mOut.SavePrimaryDeviceIp = function(vValue) {
            SaveCommon({primaryDeviceIp: vValue});
        };

        mOut.SavePin = function(vValue) {
            SaveCommon({pin: vValue});
        };

        function SaveCommon(vJson) {
            mSettings = $.extend(mSettings, vJson);
            var pStr = JSON.stringify(mSettings, null, 4);
            Transfers.WriteFile(mFilename, null, pStr);
        }

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