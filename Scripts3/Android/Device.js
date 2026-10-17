define([], function() {
    
    var mSerial;
    var mName;
    var mPlatform;
    var mImei;
    var mMsgBox;

    window.Device = {
        Serial: function() {

            if (mSerial == undefined)
                mSerial = Js.Execute({
                    command: 'Device.Serial'
                });
                // mSerial = '3CQ3352DMJ'; // debug only
            return mSerial;

        }, // Serial
        
        SuggestedSerial: function() {

            return Js.Execute({
                command: 'Device.SuggestedSerial'
            });

        }, // Serial

        SetSerial: function(vValue) {

            return Js.Execute({
                command: 'Device.SetSerial',
                value: vValue
            });

        }, // SetSerial

        Name: function() {

            if (mName == undefined)
                mName = Js.Execute({
                    command: 'Device.Name'
                });
            return mName;

        }, // END Name

        Platform: function() {

            if (mPlatform == undefined)
                mPlatform = Js.Execute({
                    command: 'Device.Platform'
                });
            return mPlatform;

        }, // END Platform

        Imei: function() {

            if (mImei == undefined)
                mImei = Js.Execute({
                    command: 'Device.Imei'
                });
            return mImei;

        }, // END Imei

        GetActiveWifi: function() {
            
            return  Js.Execute({
                command: 'Device.GetActiveWifi'
            });

        },

        GetWifiEnabled: function() {
            
            return  Js.Execute({
                command: 'Device.GetWifiEnabled'
            });

        },

        SetWifiEnabled: function(vEnable) {
            
            return  Js.Execute({
                command: 'Device.SetWifiEnabled',
                value: vEnable
            });

        },

        ConnectWifi: function(vSsid, vPassKey) {
            
            return  Js.Execute({
                command: 'Device.ConnectWifi',
                Ssid: vSsid,
                PassKey: vPassKey
            });

        },

        ShowDateSettingsPage: function() {
            
            if (window.InWindows)
                return;

            return  Js.Execute({
                command: 'Device.ShowDateSettingsPage'
            });

        },
         
        CheckServerDate: function CheckServerDate() {
            if (window.IsOfflineMode)
                return;
            if (CBool(Android.IsAutoOfflineMode())) {
                Android.AddLog('Skipping CheckServerDate because IsAutoOfflineMode is true. ref: jsSVPEMR2MHM!');
                return;
            }
            AjaxSend({
                url: '/Home/GetDate',
                success: function(vData) {
                    var pMinutesTolerance = 2;
                    console.log(vData.Value);
                    var pDiff = new Date() - new Date(vData.Value);
                    pDiff /= 60000;
                    if (Math.abs(pDiff) > pMinutesTolerance) 
                        mMsgBox = MessageBox('Your device\'s date and time must be identical to Mobiz server\'s date and time. Please adjust your device date and time to ' + $.format.date(new Date(vData.Value), 'EEE MMM d, yyyy h:mm a') + ' before proceeding. Also, please make sure that your device is using the 12-hour time format.', 'Incorrect Date or Time', [
                            {
                                Text: 'Adjust date + time',
                                Click: function () {
                                    Device.ShowDateSettingsPage();
                                } // END Click
                            },
                            {
                                Text: 'My device date + time is already correct',
                                Click: function() {
                                    Device.CheckServerDate();
                                }
                            }
                        ], {
                            OnClose: function() {
                                setTimeout(function() {
                                    Device.CheckServerDate();
                                }, 5000);
                            }
                        });
                    else if (mMsgBox != null)
                        try {
                            mMsgBox.dialog('close');
                            mMsgBox = null;
                        } catch (e) {
                            
                        }
                },
                error: function() {
                    setTimeout(function() {
                        Device.CheckServerDate();
                    }, 5000);
                }
            });
        }, // END CheckServerDate: function CheckServerDate() {

        GetFreeSpace: function() {
            return  Js.Execute({
                command: 'Device.GetFreeSpace'
            });
        }
    }; // window.Device

}); // define