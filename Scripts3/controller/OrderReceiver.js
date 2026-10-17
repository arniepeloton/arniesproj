define([
    '/Scripts3/model/PeerConnect.js?crc=',
    'promise!/Scripts3/NewMessageBox.js?crc=',
    '/Scripts3/model/OrderTakerSetup.js?crc=',
    '/Scripts/json-stable-stringify.js'
], function () {
    var mJsonStableStringify = require('/Scripts/json-stable-stringify.js');
    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        //AjaxSend({
        //    ExactUrl: '/Scripts3/view/CustomerScreen.html',
        //    type: 'GET',
        //    dataType: 'text',
        //    success: function (vData) {
        //        mHtml = vData;
        //        mDeferred.resolve();
        //    }
        //});
        mDeferred.resolve();

    })();

    $(function () {

        CreateStringObject('App.controller.OrderReceiver', null, function () {

            var mOut = {};
            var mPeerConnect;
            var mMessageIDs = [];
            var mProductTable;
            var pAddLogIndex = 0;
            var mSaveListeners = [];
            var mOrderTakerSetup = new App.model.OrderTakerSetup();
            var mServerIp;
            var mSourceIp;

            mServerIp = CStr2(mOrderTakerSetup.GetSettings().serverIp).trim();
            mSourceIp = CStr2(mOrderTakerSetup.GetSettings().sourceIp).trim();
            if (mServerIp == '') {
                //Android.AddLog('No server IP for order transmitter');
            } else if (mServerIp.indexOf('http://') != 0)
                mServerIp = 'http://' + mServerIp;
            mPeerConnect = new App.model.PeerConnect();

            mOut.AddSaveListener = function(vFunc) {
                if (!$.isFunction(vFunc))
                    throw new Error('Parameter must be a function. ref: js85B7PY984D!');
                mSaveListeners.push(vFunc);
            };

            proj.PeerReceivers.push(OnReceive);

            function OnReceive(vDT) {
                if (vDT == null)
                    return;
                if (vDT.Messages != null) {
                    for (var p = 0; p <= vDT.Messages.Rows.length - 1; p++)
                        Receiver(vDT.Messages.Rows[p].Message);
                } else {
                    if (!$.isArray(vDT.value))
                        vDT.value = JSON.parse(vDT.value);
                    for (var p = 0; p <= vDT.value.length - 1; p++)
                        Receiver(vDT.value[p].Message);
                }
            }

            function Receiver(vMessage) {
                var pJson;
                var pCrcError;
                var pData;

                try {
                    pJson = JSON.parse(vMessage);
                    if (IsGuid(pJson.MessageID)
                        && (mMessageIDs.indexOf(pJson.MessageID) != -1))
                        return;

                    mMessageIDs.push(pJson.MessageID);
                    if (pJson.command == 'FromOrder') {
                        console.log('FromOrder jsGZUUNDGPGR');
                        if (pJson.Tables.hasOwnProperty('Crc')) {
                            var pCrc = pJson.Tables.Crc;
                            if (CStr2(pCrc).trim() != '') {
                                delete pJson.Tables.Crc;
                                var pCrcReceived = crc32(mJsonStableStringify(pJson.Tables));
                                // pCrcReceived = 'jsXA94827HZG!';  // uncomment to simulate mismatch crc
                                if (pCrcReceived == pCrc) {
                                    // console.log('crc\'s matched. js5GLDYMRNYL!');
                                } else {
                                    pCrcError = 'CRC\'s do not match. CRC from source = ' + pCrc + '. CRC from destination = ' + pCrcReceived + '. ref: jsW7NFXLYJ7J!';
                                }
                            } else
                                pCrcError = 'invalid CRC. ref: jsVYWBZMVRDP!';
                        } else {
                            pCrcError = 'no CRC. ref: js8JDM8R9A6R!';
                        }

                        if (pCrcError != null) {
                            pData = {
                                command: 'CrcError',
                                Data: pCrcError,
                                ip: GetIpAddress()
                            };
                            SendMessage({
                                SourceIp: pData.ip,
                                DestinationIp: pJson.ip,
                                ClientId: 'secondary',
                                Message: pData
                            });
                            Android.AddLog(pCrcError);
                            NewMessageBox('There were discrepancies between the order data sent from the source device and receiving device. The source device will transmit the order data again. ' + pCrcError);
                            return;
                        }

                        var pImported = SaveTables(pJson.Tables);

                        pData = {
                            command: 'ImportedOrders',
                            // MessageID: pMessageID,
                            Data: pImported,
                            ip: GetIpAddress()
                        };
                        SendMessage({
                            SourceIp: pData.ip,
                            DestinationIp: pJson.ip,
                            ClientId: 'secondary',
                            Message: pData
                        });
                    } else if (pJson.command == 'Handshake') {
                        SendMessage({
                            SourceIp: GetIpAddress(),
                            DestinationIp: pJson.ip,
                            ClientId: 'secondary',
                            Message: {
                                command: 'Handshake',
                                ip: GetIpAddress()
                            }
                        });
                    }
                } catch (e) {
                    console.log(e);
                }
            };

            function SaveTables(vTables) {
                var pOut = [];
                var pImported = 0;
                
                pOut = pOut.concat(SaveLibraryTableTableInfo(vTables.LibraryTableTableInfo));
                pOut = pOut.concat(SaveLibraryTableJoined(vTables.LibraryTableJoined));
                pOut = pOut.concat(SaveTransInvoice(vTables.TransInvoice));
                SaveTransInvoiceItems(vTables.TransInvoiceItems, vTables.TransInvoiceItemsModifier, pOut);
                pOut = pOut.concat(SaveLibraryOrganization(vTables.LibraryOrganization));
                if (pOut.length > 0) {
                    for (var p = 0; p <= pOut.length - 1; p++) {
                        if (pOut[p].Imported)
                            pImported++;
                    }

                    if (pImported > 0)
                        for (var p = 0; p <= mSaveListeners.length - 1; p++) {
                            mSaveListeners[p](pOut);
                        }
                }
                    
                return pOut;
            }

            function SaveTransInvoice(vDT) {
                var pOut = [];
                var pOutItem;
                var pDrInvoice;
                var pSql;

                for (var p = 0 ; p <= vDT.length - 1; p++) {
                    var pDR = vDT[p];
                    pDrInvoice = Business.TransInvoice.GetByID(pDR.ID);
                    if ($.isEmptyObject(pDrInvoice))
                        pDrInvoice = null;
                    pOutItem = {
                        ID: pDR.ID,
                        TableName: 'TransInvoice',
                        LastUpdated: pDR.LastUpdated
                    };
                    if (pDrInvoice != null)
                        pDrInvoice = pDrInvoice.Rows[0];
                    if ((pDrInvoice == null)
                        || (new Date(pDrInvoice.LastUpdated) < new Date(pDR.LastUpdated))
                        || ((CInt2(pDrInvoice.InvoiceNumber) == 0)
                            && (CInt2(pDR.InvoiceNumber) > 0)
                            )
                        ) { 

                        var pExistingDr = pDrInvoice;
                        if (pExistingDr != null) {
                            if (
                                (CInt2(pExistingDr.InvoiceNumber) == 0)
                                &&
                                (ReadyGuid(Web.DeviceID()) != ReadyGuid(pExistingDr.DeviceID))
                                || (CInt2(pDR.InvoiceNumber) > 0)
                            )
                                pExistingDr = null;
                        }

                        if (pExistingDr == null) {
                            if (CBool(sessionStorage.getItem('IsOrderTaker')))
                                pDR.DateImported = ReadyDate(new Date());
                            SaveRow('TransInvoice', pDR);
                            var pSqlFromItems = ' FROM TransInvoiceItems WHERE InvoiceID = ' + ReadyString(pDR.ID);
                            pSql = 'DELETE ' + pSqlFromItems;
                            Sql.ExecuteSql(pSql);
                        
                            pSql = 'DELETE FROM TransInvoiceItemsModifier \n' + 
                                ' WHERE InvoiceItemID IN (\n' +
                                '   SELECT ID\n' +
                                pSqlFromItems +
                                ' )';
                            Sql.ExecuteSql(pSql);
                            pOutItem.Imported = true;
                        }
                    } else {
                        console.log('ref: jsEGNX46SZND! Existing invoice is newer than to-be-imported invoice.');
                    }
                    
                    pOut.push(pOutItem);
                }

                return pOut;
            }

            function SaveTransInvoiceItems(vDT, vDtItemsModifier, vImporteds) {
                for (var p = 0; p <= vDT.length - 1; p++) {
                    var pData = vDT[p];
                    for (var p2 = 0; p2 <= vImporteds.length - 1; p2++) {
                        var pImported = vImporteds[p2];
                        if (pImported.Imported
                            && (pImported.TableName == 'TransInvoice')
                            && (ReadyGuid(pImported.ID) == ReadyGuid(pData.InvoiceID))) {

                            SaveRow('TransInvoiceItems', pData);
                            SaveTransInvoiceItemsModifier(vDtItemsModifier, pData.ID);
                        }
                    }
                }
            }

            function SaveTransInvoiceItemsModifier(vDT, vItemID) {
                for (var p = 0 ; p <= vDT.length - 1; p++) {
                    if (ReadyGuid(vDT[p].InvoiceItemID) == ReadyGuid(vItemID))
                        SaveRow('TransInvoiceItemsModifier', vDT[p]);
                }
            }

            function SaveLibraryOrganization(vDT) {
                var pOut = [];
                var pOutItem;
                for (var p = 0 ; p <= vDT.length - 1; p++) {
                    var pDR = vDT[p];
                    pOutItem = {
                        ID: pDR.ID,
                        TableName: 'LibraryOrganization',
                        LastUpdated: pDR.LastUpdated
                    };
                    SaveRow('LibraryOrganization', pDR);
                    pOut.push(pOutItem);
                }
                return pOut;
            }

            function SaveLibraryTableJoined(vDT){               
                var pOut = [];
                var pOutItem;
                var pDrTableJoined;

                for (var p = 0 ; p <= vDT.length - 1; p++) {
                    var pDR = vDT[p];
                    pDrTableJoined = Business.LibraryTableJoined.GetByID(pDR.ID);
                    if ($.isEmptyObject(pDrTableJoined))
                        pDrTableJoined = null;
                    pOutItem = {
                        ID: pDR.ID,
                        TableName: 'LibraryTableJoined',
                        LastUpdated: pDR.LastUpdated
                    };
                    if ((pDrTableJoined == null)
                        || (new Date(pDrTableJoined.LastUpdated) < new Date(pDR.LastUpdated))) {

                        SaveRow('LibraryTableJoined', pDR);
                        pOutItem.Imported = true;
                    }
                    pOut.push(pOutItem);
                }                    
                return pOut;
            }
            
            function SaveLibraryTableTableInfo(vDT){               
                var pOut = [];
                var pOutItem;
                var pDrExisting;

                for (var p = 0 ; p <= vDT.length - 1; p++) {
                    var pDR = vDT[p];
                    pDrExisting = Business.LibraryTableTableInfo.GetByID(pDR.ID);
                    if ($.isEmptyObject(pDrExisting))
                        pDrExisting = null;
                    pOutItem = {
                        ID: pDR.ID,
                        TableName: 'LibraryTableTableInfo',
                        LastUpdated: pDR.LastUpdated
                    };
                    if ((pDrExisting == null)
                        || (new Date(pDrExisting.LastUpdated) < new Date(pDR.LastUpdated))) {

                        SaveRow('LibraryTableTableInfo', pDR);
                        pOutItem.Imported = true;
                    }
                    pOut.push(pOutItem);
                }                    
                return pOut;
            }

            function SaveRow(vTableName, vDR) {
                var pIU = new InsertUpdate(vTableName);
                var pSql;

                for (var pKey in vDR) {
                    if (!vDR.hasOwnProperty(pKey))
                        continue;

                    pIU.Add(pKey, ReadyString(vDR[pKey]));
                }
                pSql = pIU.GetUpdate('ID = ' + ReadyString(vDR.ID));
                pSql = Sql.ExecuteSql(pSql);
                if (CDec2(pSql) == 0) {
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql);
                }
            }

            function ConnectToPrimary(vOptions) {
                vOptions = $.extend({
                    ShowAsToast: false
                });
                var pIP = mPeerConnect.GetSettings().remoteIp;
                var pClientId = 'OrderReceiver ' + NewID();
                var pMessage = {
                    command: 'OrderReceiverInit'
                };
                var pStr = 'Connecting to ' + pIP;

                if (vOptions.ShowAsToast)
                    Android.Toast(pStr);
                else
                    Android.AddLog(pStr);
                try {
                    SendMessage({
                        DestinationIp: pIP,
                        ClientId: pClientId,
                        Message: pMessage
                    });
                } catch (e) {
                    Android.Toast('Unable to connect to primary device');
                }
                setTimeout(ConnectToPrimary, 60000);
            }

            //function AddLog(vText) {
            //    var pDiv;

            //    pAddLogIndex++;
            //    vText = pAddLogIndex + '; ' + vText;
            //    pDiv = $('<div>').text(vText);
            //    mDivCustomerScreenLog.append(pDiv);
            //    $('html, body').animate({
            //        scrollTop: pDiv.offset().top + 'px'
            //    }, 'slow');
            //}

            function SendMessage(vParams) {
                var pData = $.extend({
                }, vParams);
                AjaxSend({
                    ExactUrl: mServerIp + '/SalesMenu/SaveAndGetPeerMessages?dontcache=1',
                    // ExactUrl: mServerIp,
                    data: {
                        command: 'Android.SaveAndGetPeerMessages',
                        Message: pData.Message,
                        SourceIP: GetIpAddress(),
                        DestinationIP: pData.DestinationIp
                    },
                    encodeURIComponent: true,
                    NoSplash: true,
                    success: function(vData) {
                        for (var p = 0; p <= proj.PeerReceivers.length - 1; p++) {
                            var pReceiver = proj.PeerReceivers[p];
                            if ($.isFunction(pReceiver))
                                pReceiver(vData);
                        }
                    },
                    error: function() {
                    }
                });
            } // SendMessage

            function GetIpAddress() {
                if (mSourceIp.length > 0)
                    return mSourceIp;
                else
                    return Android.IPAddress();
            }

            function HasNonEmptyArray(vTables) {
                // vTables = database tables
                for (var pTable in vTables) {
                    if (vTables.hasOwnProperty(pTable)) 
                        if ($.isArray(pTable))
                            if (pTable.length > 0)
                                return true;
                }; // END remove blank entries

                return false;
            }

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});