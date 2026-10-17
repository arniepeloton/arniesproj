define(['/Scripts3/common/common.js?v=' + assetversion,
    (IsMobile ? '/Scripts3/Android/Android.js?crc=' : ''),
    (IsMobile ? '/Scripts3/Android/Transfers.js?crc=' : ''),
    (IsMobile ? '/Scripts3/Android/Sql.js?crc=' : ''),
    (IsMobile ? '/Scripts/EscPosPrinter.js?crc=' : ''), // true code
    (IsMobile ? '/Scripts3/Android/Device.js?crc=' : '')
    ], function () {

        window.mobile = {

            // not used cguro
            LocalLink: function (vUrl) {

                var pIndex;
                var pParameters;

                pIndex = FirstIndexOf(vUrl, ['?', '#']);
                if (pIndex == 0)
                    return vUrl;
                else if (pIndex == -1)
                    pIndex = vUrl.length;

                if (vUrl.substring(0, pIndex).indexOf('.') == -1) {
                    pParameters = vUrl.substr(pIndex);
                    return vUrl.substring(0, pIndex) + '.html' + pParameters;
                } else
                    return vUrl;

                /// returns the first index (0-based, -1 if not found) of the substring that we are looking for
                function FirstIndexOf(vText, vArray) {
                    // PARAMS:
                    //  vText: string to be searched from
                    //  vArray: array of string, containing 2 or more string that we are looking for

                    var pOut = -1;
                    var pIndex;

                    for (var p = 0; p <= vArray.length - 1; p++) {
                        pIndex = vText.indexOf(vArray[p]);
                        if (pIndex > -1) {
                            if (pOut == -1)
                                pOut = pIndex;
                            else
                                pOut = Math.min(pOut, pIndex);
                        };
                    };

                    return pOut;

                }; // END FirstIndexOf
            }, // LocalLink

            _Callbacks: [],
            _Socket: null,

            CallWithCallback: function (vOptions, vCallbacks) {
                var pResult;
                var pIndex;

                // if InAndroid, "Android.CallWithCallback" is an "@JavascriptInterface"
                // if not InAndroid, "Android.CallWithCallback" will be called via SAjaxSend
                pResult = Android.CallWithCallback(JSON.stringify(vOptions));

                for (var pField in pResult.Callbacks) {
                    if (vCallbacks.hasOwnProperty(pField)
                    && pResult.Callbacks.hasOwnProperty(pField)) {

                        pIndex = pResult.Callbacks[pField];
                        this._Callbacks[pIndex] = vCallbacks[pField];
                    };
                };

                return pResult;
            },

            Callback: function (vParams) {
                // if InAndroid, this will be called from Android only. This is not called by JS
                var pError;
                var pItem;

                switch (vParams.Command) {
                    case 'Scan':
                    case 'Download':
                    case 'Upload':
                    case 'DownloadAndApplySchema':
                    case 'ExtractZipFile':
                    case 'DataExport':
                    case 'AjaxSend':
                    case 'DataSync':
                        if (vParams.Index == undefined) {
                            pError = 'Index not found';
                            break;
                        };
                        if (!IsNumeric(vParams.Index)) {
                            pError = 'Index is not a valid number: ' + vParams.Index;
                            break;
                        };
                        pItem = this._Callbacks[vParams.Index];
                        if (pItem == undefined) {
                            // pError = 'Item #' + vParams.Index + ' is undefined';
                            break;
                        };
                        if (!$.isFunction(pItem)) {
                            pError = 'Item #' + vParams.Index + ' is not a function';
                            break;
                        };

                        pItem(vParams);

                        return true;

                    default:
                        pError = 'Invalid command: ' + vParams.Command;

                }; // switch

                if (pError != undefined) {
                    console.log(pError);
                    Android.Toast('Callback Error!: ' + pError);
                    return false;
                };
            },

            Scan: function (vOptions) {

                //        vOptions = {
                //            Success: function() {
                //            },
                //            Error: function() {
                //              // optional
                //            }
                //        };

                if (vOptions.Error == undefined) {
                    vOptions.Error = function () {
                        Android.Toast('User cancelled scanning or error occured while scanning');
                    };
                };

                var pCallbacks = {
                    Success: vOptions.Success,
                    Error: vOptions.Error
                };

                vOptions.Command = 'Scan';

                mobile.CallWithCallback(vOptions, pCallbacks);
            }, // Scan

            Send: function (vOptions) {
                //        vOptions = {
                //            Success: function() {
                //            },
                //            Error: function() {
                //              // optional
                //            }
                //        };

                if (vOptions.SocketId == undefined)
                    throw 'SocketId must be specified';

                if (vOptions.Error == undefined) {
                    vOptions.Error = function () {
                        Android.Toast('Send error');
                    };
                };

                var pCallbacks = {
                    Success: vOptions.Success,
                    Error: vOptions.Error
                };

                vOptions.Command = 'Send';

                mobile.CallWithCallback(vOptions, pCallbacks);

            },

            DataExport: function (vOptions) {
                //        vOptions = {
                //            Success: function() {
                //            },
                //            Error: function() {
                //              // optional
                //            }
                //        };

                if (vOptions.Error == undefined) {
                    vOptions.Error = function () {
                        Android.Toast('DataExport error');
                    };
                };

                var pCallbacks = {
                    Success: vOptions.Success,
                    Error: vOptions.Error
                };

                vOptions.Command = 'DataExport';

                mobile.CallWithCallback(vOptions, pCallbacks);

            },

            DataSync: function (vOptions) {
                //        vOptions = {
                //            Success: function() {
                //            },
                //            Error: function() {
                //              // optional
                //            }
                //        };

                if (vOptions.Error == undefined) {
                    vOptions.Error = function () {
                        Android.Toast('DataSync error');
                    };
                };

                var pCallbacks = {
                    Success: vOptions.Success,
                    Error: vOptions.Error
                };

                vOptions.Command = 'DataSync';

                mobile.CallWithCallback(vOptions, pCallbacks);

            },

            DontFollowUp: false
        }; // END window.mobile = {

        window.ReadyString = function (vStr) {

            return '\'' + CStr2(vStr).replace(/\'/g, '\'\'') + '\'';

        }; // ReadyString

        window.ReadyDate = function (vStr) {
            // for Sqlite

            if (IsDate(vStr))
                return ReadyString($.format.date(new Date(vStr), 'yyyy-MM-dd HH:mm:ss.SSS'));
            else
                return 'null';

        }; // ReadyDate

        //added by arnie Jan 22
        var isIE = false || !!document.documentMode;
        var randomValues;

        if (isIE == true) {
            var crypto = window.msCrypto;          
            randomValues = crypto.getRandomValues.bind(crypto);
        }
        else {
            var crypto = window.crypto;
            randomValues = crypto.getRandomValues.bind(crypto);
        }//end

        var getRandomValues = randomValues;
        var buffer = new Uint16Array(8 * 24); // space for 24 uuids
        var index = 0;

        window.NewID = function () {
            // http://jsperf.com/guid-crypto-vs-math
            var guid = [], n, i = 0;
            if (index == 0) getRandomValues(buffer);
            for (; i < 8; i++) {
                n = buffer[index + i];
                if (i == 3) n = (n & 0x0FFF) + 0x4000;
                if (i == 4) n = (n & 0x3FFF) + 0x8000;
                if (i >= 2 && i < 6) guid.push("-");
                guid.push((n + 0x10000).toString(16).substr(1));
            }
            index += 8;
            if (index >= buffer.length) index = 0;
            return guid.join("");
        };

        //    window.NewID_old = function() {

        //        var pAll = '0123456789abcdef';
        //        var pOut = '';
        //        var p, p2;

        //        for (p = 1; p <= 8; p++)
        //            pOut += pAll[RandomInteger(0, 15)];
        //        pOut += '-';

        //        for (p2 = 1; p2 <= 3; p2++) {
        //            for (p = 1; p <= 4; p++) {
        //                pOut += pAll[RandomInteger(0, 15)];
        //            };
        //            pOut += '-';
        //        };

        //        for (p = 1; p <= 12; p++)
        //            pOut += pAll[RandomInteger(0, 15)];

        //        return pOut;

        //    }; // NewID

        window.InsertUpdate = function (vTableName) {

            var mMyClass = this;
            var mItem = {};
            
            mMyClass.TableName = vTableName;
            mMyClass.FromSql = null;

            this.Add = function (vField, vValue) {
                mItem[vField] = vValue;
            };

            this.GetInsert = function (vFromTable) {
                // vFromTable: bool

                var pFields = '';
                var pValues = '';
                var pOut;

                for (var pField in mItem) {
                    if (mItem.hasOwnProperty(pField)) {
                        if (pFields != '') {
                            pFields += ', ';
                            pValues += ', ';
                        };
                        pFields += pField;
                        pValues += mItem[pField];
                    };
                };

                if (vFromTable)
                    pOut = 'INSERT INTO ' + mMyClass.TableName +
                        '(' + pFields + ') SELECT ' + pValues + ' ' + mMyClass.FromSql;
                else
                    pOut = 'INSERT INTO ' + mMyClass.TableName +
                        '(' + pFields + ') VALUES (' + pValues + ')';

                return pOut;

            }; // GetInsert

            this.GetUpdate = function (vCondition) {

                var pFields = '';
                var pSql;

                for (var pField in mItem) {
                    if (mItem.hasOwnProperty(pField)) {
                        if (pFields != '') {
                            pFields += ', ';
                        };
                        pFields += pField + ' = ' + mItem[pField] + '\n';
                    };
                };

                pSql = 'UPDATE ' + mMyClass.TableName + '\n' + 
                ' SET ' + pFields +
                ' WHERE ' + vCondition;

                return pSql;

            }; // GetUpdate

            this.GetItem = function (vField) {

                return mItem[vField];

            };

            this.GetItems = function(vDR, vCallback) {
                for (var pDrName in vDR) {
                    if (vDR.hasOwnProperty(pDrName) == false)
                        continue;
                    var pValue = vDR[pDrName];
                    if (vCallback) {
                        var pArgs = {
                            FieldName: pDrName,
                            Value: pValue,
                            Cancel: false,
                            DontEscape: false,
                        }
                        if (CStr2(pValue).trim() == '')
                            pArgs.Cancel = true;
                        vCallback(pArgs);
                        if (pArgs.Cancel)
                            continue;
                        if (pArgs.DontEscape)
                            pValue = pArgs.Value;
                        else
                            pValue = ReadyString(pArgs.Value);
                        mMyClass.Add(pDrName, pValue);
                        continue;
                    } else if (CStr2(pValue).trim() == '')
                        continue;
                    mMyClass.Add(pDrName, ReadyString(pValue));
                }
            }

            return this;

        }; // InsertUpdate

        //    window.GetSessionData = function() {

        //        try {
        //            var pSessionData = Android.GetSessionData();
        //            // console.debug('pSessionData: ' + pSessionData);
        //            if (CStr2(pSessionData) != '')
        //                return JSON.parse(pSessionData);
        //            else
        //                return {};
        //        } catch (e) {
        //            console.debug('exception on GetSessionData');
        //            return {};
        //        };

        //    };

        //    window.SetSessionData = function(vValue) {

        //        if (vValue == undefined) 
        //            Android.SetSessionData('');
        //        else
        //            Android.SetSessionData(JSON.stringify(vValue));

        //    };

        //    window.UpdateSessionData = function (vValue) {

        //        var pSessionData = GetSessionData();

        //        pSessionData = $.extend(pSessionData, vValue);
        //        SetSessionData(pSessionData);
        //    
        //        return pSessionData;

        //    };

        //    window.LoadSessionStorage = function() {

        //        var pData;

        //        pData = GetSessionData();
        //        pData = pData.sessionStorage;
        //        if (pData != undefined) {
        //            try {
        //                pData = JSON.parse(pData);
        //                sessionStorage.clear();
        //                for(var pItem in pData) {
        //                    if (pData.hasOwnProperty(pItem)) {
        //                        sessionStorage[pItem] = pData[pItem];
        //                    };
        //                };
        //            } catch(e) {
        //                console.log('LoadSessionStorage error');
        //            };
        //        };

        //    }; // LoadSessionStorage

        // code below are for debugging only.
        // dont deploy [optional] to production
        (function Init() {
            if (window.IsMobile && !window.InAndroid && !window.InWindows && !window.InWinRt) {
                // http://davidwalsh.name/websocket
                mobile._Socket = new WebSocket('ws://' + localStorage.IP + ':9999');

                // pSocket = new WebSocket('ws://echo.websocket.org/');
                mobile._Socket.onopen = function () {
                    console.log('socket connected');

                    mobile._Socket.onmessage = function (e) {
                        if (CStr2(e.data).startsWith('eval:'))
                            eval(e.data.substring(5));
                        console.log('pSocket.onmessage: ' + e.data);
                    };

                    mobile._Socket.onclose = function () {
                        console.log('pSocket.onclose');
                    };
                }; // END mobile._Socket.onopen
            } // END if (window.IsMobile && !window.InAndroid && !window.InWindows) {
        })(); // END function Init

        window.Js = {
            Execute: function (vOptions) {
                // PARAMS:
                //  vOptions: json object

                var pResult; // json object
                var pError;

                try {
                    vOptions = $.extend({}, vOptions);

                    if (CBool(vOptions.ForceAjax))
                        pResult = SjaxSend({data: vOptions});
                    else if (InAndroid) {
                        pResult = JsInterface.Execute(JSON.stringify(vOptions)); // JsInterface is declared by Android
                        pResult = JSON.parse(pResult);
                    } else if (InWinRt) {
                        pResult = SjaxSend({
                            //type: 'GET',
                            data: vOptions,
                            IP: 'http://127.0.0.1:8888'
                            //IP: 'http://localhost:8888'
                        });
                    } else
                        pResult = SjaxSend({data: vOptions});

                    if (pResult != undefined)
                        if (pResult.Error != undefined)
                            pError = pResult.Error;
                        else if (pResult.value != undefined)
                            return pResult.value;
                } catch (e) {
                    console.error(e);
                    // alert('An error occured while processing Js.Execute: ' + e);
                    return null;
                };

                if (pError != undefined)
                    throw new Error(pError);

                function SjaxSend(vData) {

                    var pResult;
                    var pParams;
                    var pIP;
                    var pUrl;

                    if (!window.InWindows) {
                        if ((vData != undefined)
                        && (vData.IP != undefined)) {

                            pIP = vData.IP;
                        } else {
                            pParams = GetUrlParameters();
                            if (pParams.IP != undefined) {
                                localStorage.IP = pParams.IP;
                            } else {
                                if (CStr2(localStorage.IP).trim() == '') {
                                    // dont use MessageBox. jqm is not yet loaded at this point
                                    console.warn('Please add "IP" query string to point to the IP address of your Android device. Example: "&IP=192.168.1.123"');
                                    return;
                                };
                            };
                            pIP = localStorage.IP;
                        }
                        if (!pIP.startsWith('http://'))
                            pIP = 'http://' + pIP;
                        pUrl = ParseUrl(pIP);
                        // http://jessepollak.me/chrome-was-wrong-ie-was-right/
                        // IE sets port to 80 if blank
                        if ((CStr2(pUrl.port) == '')
                            || (pUrl.port == 80))
                            pUrl.port = 8888;
                        pUrl = pUrl.toString();
                    } // END if (!window.InWindows) {

                    vData = {
                        url: pUrl,
                        type: vData.type,
                        data: vData.data,
                        async: false
                    };
                    if (window.InWindows)
                        vData.ExactUrl = '/OfflineServer/';
                    else
                        vData.encodeURIComponent = true; // required by NanoHTTPD

                    pResult = AjaxSend(vData);
                    pResult = JSON.parse(pResult.responseText);

                    return pResult;

                }; // SjaxSend
            } // Execute
        }; // window.Js

        (function () {

            var mDeviceSerial;
            var mDeviceID;
            var mPeopleUserID;
            var mOrgID;

            window.Web = {
                DeviceSerial: function () {
                    if (mDeviceSerial == undefined)
                        mDeviceSerial = Device.Serial();
                    return mDeviceSerial;
                }, // DeviceSerial

                DeviceID: function () {
                    var pDR;
                    var pSql;

                    if (mDeviceID == undefined) {
                        pSql = 'SELECT LibraryDevice.ID' +
                        ' FROM LibraryDevice' +
                        ' JOIN LibraryDeviceFowst ON LibraryDeviceFowst.DeviceID = LibraryDevice.ID' +
                        ' WHERE LibraryDevice.UUID = ' + ReadyString(Web.DeviceSerial());
                        pDR = Sql.ExecuteRow(pSql);
                        if (pDR != undefined) {
                            mDeviceID = pDR.ID;
                        };
                    };
                    return mDeviceID;
                }, // DeviceID

                PeopleUserID: function () {
                    var pSessionData;

                    if (mPeopleUserID == undefined) {
                        pSessionData = Android.GetSessionData();
                        mPeopleUserID = pSessionData.PeopleUserID;
                    };

                    return mPeopleUserID;

                }, // PeopleUserID

                SetOrgID: function (vValue) {

                    if (IsGuid(vValue) == false)
                        throw 'Web.SetOrgID value must be a valid GUID';

                    vValue = vValue.toLowerCase();
                    mDeviceID = undefined;
                    mDeviceSerial = undefined;
                    Android.UpdateSessionData({
                        OrgID: vValue
                    });
                    sessionStorage.OrgID = vValue;
                    if (mOrgID != vValue) 
                        WriteOrgNameToTextFile(vValue);
                    mOrgID = vValue;

                    function WriteOrgNameToTextFile(vOrgID) {
                        
                        var pCompanyName;

                        try {
                            pCompanyName = Business.LibraryOrganization.ByID(vOrgID).CompanyName;
                            Transfers.WriteFile('LastOrgName.txt', null, pCompanyName);
                            Transfers.WriteFile('LastOrgID.txt', null, vOrgID);
                        } catch(e) {
                            //console.error('jsUM5Z2ZLGWG! ' + e);
                        }
                    } // END function WriteOrgNameToTextFile() {
                },

                OrgID: function () {
                    var pSessionData;
                    var pOrgID;

                    pOrgID = GetUrlParameters().OrgID;
                    if (IsGuid(pOrgID)) {
                        this.SetOrgID(pOrgID);
                    } else if (mOrgID == undefined) {
                        pSessionData = Android.GetSessionData();
                        mOrgID = pSessionData.OrgID;
                    };

                    return mOrgID;

                    
                }, // END OrgID: function () {

                HandleOrganization: function (vDontRedirect) { // returns boolean

                    if (CStr2(this.OrgID()).trim() == '') {
                        if (CBool(vDontRedirect) == false)
                            window.location = MergeUrlParameters({
                                IsMobile: IsMobile
                            }, '/People/PeopleMenu');
                        return;
                    }

                    try {
                        // bookmark: 2015_12_28__16_01_57_6555: new database naming convention
                        var pFilename;
                        pFilename = this.OrgID();
                        Sql.OpenDatabase(pFilename + '.public.db');
                        Sql.AttachDatabase(pFilename + '.private.db', 'private');
                        Sql.AttachDatabase(pFilename + '.cache.db', 'cache');
                        Business.LibraryDevice.LoadBirAccreditation();
                        if (this.OrgID() != '7c794971-1cd2-4ad4-922f-36a93edde09d') {
                            Sql.DeleteIfExist('7c794971-1cd2-4ad4-922f-36a93edde09d');
                            Sql.DeleteIfExist('7c794971-1cd2-4ad4-922f-36a93edde09d.private');
                            Sql.DeleteIfExist('7c794971-1cd2-4ad4-922f-36a93edde09d.public');
                        }
                        return true;
                    } catch (e) {
                        MessageBox('An error occured while attaching database. Please ask assistance from Mobiz reprensentative or you may try to synchronize database to get the latest version of your database. \n\n' + e.toString(), 'Attach Error', 
                            [
                                {
                                    Text: 'Okay', Click: function () {
                                        //window.location = '/SignIn?IsMobile=1';
                                    }
                                }
                            ]
                        );
                    }; // END catch

                } // END HandleOrganization
            }; // this.Web = {        
        })(); // END Web

        if (window.InWindows) {
            var mOldCallbackKeys = [];

            function FollowUpCallBacks() {

                if (window.mobile.DontFollowUp)
                    return;

                AjaxSend({
                    ExactUrl: '/FollowUp',
                    NoSplash: true,
                    async: true,
                    data: {
                        Indices: mobile.CallBackIndices,
                        command: 'Android.FollowUpCallback',
                        OldKeys: mOldCallbackKeys
                    },
                    success: function (vData) {
                        var pJson;

                        vData = $.extend({
                            Callbacks: []
                        }, vData);
                        mOldCallbackKeys = [];

                        for (var pField in vData.Callbacks) {
                            if (vData.Callbacks.hasOwnProperty(pField)) {
                                try {
                                    pJson = JSON.parse(vData.Callbacks[pField]);
                                    if (mobile.Callback(pJson))
                                        mOldCallbackKeys.push(pField);
                                } catch (e) {
                                    console.error(e);
                                }
                            };
                        };

                    },
                    error: function (vError) {
                        console.log('js4CBELW937L');
                    },
                    complete: function () {
                        setTimeout(FollowUpCallBacks, 1000);
                    }
                });

            } // END FollowUpCallBacks
            FollowUpCallBacks();
        }

    });   // define1`