define([], function() {
    var mPackageName;

    window.Android = {
        Toast: function(vText) {
            console.log('Toast: ' + vText);
            return Js.Execute({
                command: 'Android.Toast',
                Text: vText
            });
        },
        HideSplash: function() {
            return Js.Execute({
                command: 'Android.HideSplash'
            });
        },
        //Function for inventory count
        OfflineBalances: function() {
            return Js.Execute({
                command: 'Android.OfflineBalances'
            });
        },
        //Function for PDF Creation
        PDFCreation: function(vOperation, vData) {
            return Js.Execute({
                operation: vOperation,
                data: vData,
                command: 'Android.PDFCreation'
            });
        },
        //Function for Printing via Windows
        WindowsPrinting: function(vData) {
            return Js.Execute({
                data: vData,
                command: 'Android.WindowsPrinting'
            });
        },
        GetServer: function() {
            return Js.Execute({
                command: 'Android.GetServer'
            });
        },
        GetSessionData: function() {
            return Js.Execute({
                command: 'Android.GetSessionData'
            });
        },
        SetSessionData: function(vValue) {
            return Js.Execute({
                command: 'Android.SetSessionData',
                value: vValue
            });
        },
        UpdateSessionData: function(vValue) {
            var pSessionData = this.GetSessionData();

            pSessionData = $.extend(pSessionData, vValue);
            this.SetSessionData(pSessionData);
    
            return pSessionData;
        },
        OfflineMode: function() {
            return Js.Execute({
                command: 'Android.OfflineMode'
            });
        },
        VerifyHash: function(vRaw, vHash) {
            return Js.Execute({
                Raw: vRaw,
                Hash: vHash,
                command: 'Android.VerifyHash'
            });
        },
        SaltedHashedString: function(vRaw, vHash) {
            return Js.Execute({
                Raw: vRaw,
                command: 'Android.SaltedHashedString'
            });
        },
        CallWithCallback: function(vOptions) {
            return Js.Execute({
                command: 'Android.CallWithCallback',
                options: vOptions
            });
        },
        AppVersion: function() {
            return Js.Execute({
                command: 'Android.AppVersion'
            });
        },
        AppVersionName: function() {
            return Js.Execute({
                command: 'Android.AppVersionName'
            });
        },
        AppBuildDate: function() {
            return Js.Execute({
                command: 'Android.AppBuildDate'
            });
        },
        AppInstallDate: function() {
            return Js.Execute({
                command: 'Android.AppInstallDate'
            });
        },
        IsNetworked: function() {
            return Js.Execute({
                command: 'Android.IsNetworked'
            });
        },
        VerifyHash: function(vRaw, vHash) {
            return Js.Execute({
                Raw: vRaw,
                Hash: vHash,
                command: 'Android.VerifyHash'
            });
        },
        AddLog: function(vValue) {
            console.log(vValue);
            return Js.Execute({
                Value: vValue,
                command: 'Android.AddLog'
            });
        },
        SendToClient: function(vClientID, vMessage) {
            return Js.Execute({
                command: 'Android.SendToClient',
                ClientID: vClientID,
                Message: vMessage
            });
        },
        StopFlushing: function() {
            return Js.Execute({
                command: 'Android.StopFlushing'
            });
        },
        LogToFile: function(vValue) {
            return Js.Execute({
                Value: vValue,
                command: 'Android.LogToFile'
            });
        },
        EmailLogs: function(vValue) {
            return Js.Execute({
                command: 'Android.EmailLogs'
            });
        },
        DebugMode: function() {
            var p = Js.Execute({
                command: 'Android.DebugMode'
            }); 
            return CBool(p);
        },
        DoneSchema: function() {
            var p = Js.Execute({
                command: 'Android.DoneSchema'
            }); 
            return CBool(p);
        },
        PartialImportInProgress: function() {
            var p = Js.Execute({
                command: 'Android.PartialImportInProgress'
            }); 
            return CBool(p);
        },
        IsOfflineMode: function() {
            var p = Js.Execute({
                command: 'Android.IsOfflineMode'
            }); 
            return CBool(p);
        },
        PackageName: function() {
            if (mPackageName == null)
                mPackageName = Js.Execute({
                    command: 'Android.PackageName'
                });
            return mPackageName;
        },
        CreatePdf: function(vTemplateName, vData) {
            return Js.Execute({
                command: 'Android.CreatePdf',
                TemplateName: vTemplateName,
                Data: vData
            });
        },
        ShowAbout: function() {
            return Js.Execute({
                command: 'Android.ShowAbout'
            });
        },
        PromptRestart: function() {
            return Js.Execute({
                command: 'Android.PromptRestart'
            });
        },
        Restart: function() {
            return Js.Execute({
                command: 'Android.Restart'
            });
        },
        UpdateApk: function() {
            return Js.Execute({
                command: 'Android.UpdateApk'
            });
        },
        ExitApp: function() {
            return Js.Execute({
                command: 'Android.ExitApp'
            });
        },
        IPAddress: function() {
            return Js.Execute({
                command: 'Android.IPAddress'
            });
        },
        LoadToWebView2: function(vParams) {
            return Js.Execute({
                command: 'Android.LoadToWebView2',
                Url: vParams.Url
            });
        },
        ScriptToWebView: function(vParams) {
            return Js.Execute({
                command: 'Android.ScriptToWebView',
                Script: vParams.Script,
                WebViewIndex: vParams.WebViewIndex
            });
        },
        SetCurrentTab: function(vTabIndex) {
            return Js.Execute({
                command: 'Android.SetCurrentTab',
                TabIndex: vTabIndex
            });
        },
        SetSessionStorage: function(vParams) {
            var pScript = '';

            for (var pField in sessionStorage) {
                if (sessionStorage.hasOwnProperty(pField)) {
                    if (sessionStorage[pField] == null)
                        pScript += 'sessionStorage.removeItem(\'' + CStr2(pField).replace(/\'/g, '\\\'') + ');';
                    else
                        pScript += 'sessionStorage[\'' + CStr2(pField).replace(/\'/g, '\\\'') + '\'] = \'' + CStr2(sessionStorage[pField]).replace(/\'/g, '\\\'') + '\';';
                    pScript += '\n';
                };
            };
            return Js.Execute({
                command: 'Android.ScriptToWebView',
                Script: pScript,
                WebViewIndex: vParams.WebViewIndex
            });
        },
        IsAutoOfflineMode: function() {
            return Js.Execute({
                command: 'Android.IsAutoOfflineMode'
            });
        },
        SetAutoOfflineMode: function(vValue) {
            return Js.Execute({
                command: 'Android.SetAutoOfflineMode',
                value: vValue
            });
        },
        SetOrgID: function(vOrgID) {
            return Js.Execute({
                command: 'Android.SetOrgID',
                OrgID: vOrgID
            });
        },
        GetOrgID: function() {
            return Js.Execute({
                command: 'Android.GetOrgID'
            });
        },
        AjaxSend: function(vParams) {
            var pOptions = $.extend({
                Command: 'AjaxSend',
                url: vParams.url,
                data: 'your data to send'
            }, vParams);
            var pCallbacks = {
                Success: function(vData) {
                    vParams.Success(vData);
                },
                Error: vParams.Error
            };
            mobile.CallWithCallback(pOptions, pCallbacks);
        },
        GetStickerPrinterIP: function() {
            return Js.Execute({
                command: 'Android.StickerPrinterIP'
            });
        },
        PinApp: function() {
            return Js.Execute({
                command: 'Android.PinApp'
            });
        },
        UnpinApp: function() {
            return Js.Execute({
                command: 'Android.UnpinApp'
            });
        },
        RemoveAsDeviceOwner: function() {
            return Js.Execute({
                command: 'Android.RemoveAsDeviceOwner'
            });
        },
        UsbSerialSend: function(vParams) {
            return Js.Execute({
                command: 'UsbSerial.Send',
                value: JSON.stringify(vParams)
            })
        },
        Alert: function(vMessage, vTitle) {
            return Js.Execute({
                command: 'Android.Alert',
                message: vMessage,
                title: vTitle
            })
        },
        SetMobizAppVersion: function(vVersion) {
            return Js.Execute({
                command: 'Android.SetMobizAppVersion',
                value: vVersion || 1
            })
        }
    }; // Android
}); // define