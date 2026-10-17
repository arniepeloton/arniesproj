define(['common',
    '/Scripts3/model/OrderTakerSetup.js?crc=',
    '/Scripts/json-stable-stringify.js'
], function () {
    var mJsonStableStringify = require('/Scripts/json-stable-stringify.js');

    $(function () {

        var mTimeoutHandle;
        var mLastLoop;
        var mMinLoopInterval = 2000;

        CreateStringObject('App.controller.OrderTransmitter', null, function () {

            var mOut = {};
            var mClients = [];
            var mRemoteIps;
            var mHandshakeReplyMaxAge = 60; // seconds
            var mHandshakeMaxInterval = 30; // seconds
            var mHandshakeIntervalIncrement = 10; // seconds
            var mServerIp;
            var mSourceIp;

            if (OfflineMode) {
                mOut.model = new App.model.OrderTakerSetup();
                if (mOut.model.GetIsOrderTaker())
                    mRemoteIps = mOut.model.GetSettings().posIp;
                else
                    mRemoteIps = mOut.model.GetSettings().orderTakerIps;
                mServerIp = CStr2(mOut.model.GetSettings().serverIp).trim();
                if (mServerIp == '') {
                    // Android.AddLog('No server IP for order transmitter');
                } else if (mServerIp.indexOf('http://') != 0)
                    mServerIp = 'http://' + mServerIp;
                mSourceIp = CStr2(mOut.model.GetSettings().sourceIp).trim();
                mRemoteIps = CStr2(mRemoteIps);
                if (mRemoteIps.trim() == '') {
                    mRemoteIps = [];
                } else {
                    mRemoteIps = mRemoteIps.split('\n');
                    mRemoteIps = mRemoteIps.map(function(vItem) {
                        return {
                            IP: vItem,
                            LastHandshake: null,
                            NextHandshake: null,
                            LastHandshakeReply: null,
                            Interval: 0 // seconds
                        }
                    });
                }
            };

            //Transfers.AddClientReceiver(OnReceiver);

            //Transfers.AddReceiver(OnReceiver);
            proj.PeerReceivers.push(OnReceive);

            function OnReceive(vDT) {
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

                try {
                    pJson = JSON.parse(vMessage);
                    if (pJson.command == 'FromOrder') {
                        mClients.push(vMessage.ClientId);
                    } else if (pJson.command == 'ImportedOrders') {
                        SaveImportedOrders(pJson.ip, pJson.Data);
                    } else if (pJson.command == 'Handshake') {
                        var pItem = GetByIP(pJson.ip);
                        if (pItem != null)
                            pItem.LastHandshakeReply = new Date();
                    } else if (pJson.command == 'CrcError') {
                        NewMessageBox('There were discrepancies between the order data sent from the source device and receiving device. The source device will transmit the order data again. ' + pJson.Data);
                    }
                } catch (e) {
                    Android.AddLog(e);
                }
            }

            function SaveImportedOrders(vDestinationIP, vData) {
                
                for (var p = 0; p <= vData.length - 1; p++) {
                    var pDR = vData[p];
                    var pIU = new InsertUpdate('private.TransPeerImport');
                    pIU.Add('TransLastUpdated', ReadyString(pDR.LastUpdated));
                    var pSql = pIU.GetUpdate('DestinationIP = ' + ReadyString(vDestinationIP) + '\n' +
                        ' AND TableName = ' + ReadyString(pDR.TableName) + '\n' +
                        ' AND TransID = ' + ReadyString(pDR.ID));
                    var pCount = CInt2(Sql.ExecuteSql(pSql));
                    if (pCount == 0) {
                        pIU.Add('DestinationIP', ReadyString(vDestinationIP));
                        pIU.Add('TableName', ReadyString(pDR.TableName));
                        pIU.Add('TransID', ReadyString(pDR.ID));
                        pSql = pIU.GetInsert();
                        Sql.Execute(pSql);
                    }
                }
            }

            mOut.Start = function(vParams) {
                if (!OfflineMode) 
                    return;
                if (window.location.pathname.toLowerCase() == '/billofmaterials')
                    return;
                if (mServerIp == '') {
                    //Android.AddLog('No server IP for order transmitter');
                    return;
                }
                if (new Date() - mLastLoop < mMinLoopInterval)
                    return;
                if (mTimeoutHandle == null)
                    setInterval(mOut.Start, 1000);
                mLastLoop = new Date();
                //Android.AddLog('OrderTransmitter.Start jsHH7Y77AYKM!');
                for (var p = 0; p <= mRemoteIps.length - 1; p++) {
                    try {
                        var pRemoteIp = mRemoteIps[p];
                        if ((pRemoteIp.LastHandshakeReply != null)
                            && ((new Date() - pRemoteIp.LastHandshakeReply) / 1000 <= mHandshakeReplyMaxAge)) {

                            Android.AddLog('pRemoteIp: ' + pRemoteIp.IP + '. js3JZPZLWT78!');
                            if (CStr2(pRemoteIp).trim() == '')
                                continue;
                            var pMessageID = NewID();
                            var pData = GetTransmittableOrders(pRemoteIp.IP);
                            pData.Crc = crc32(mJsonStableStringify(pData));
                            //Android.AddLog('jsRDQFQQ2FAM!: ' + JSON.stringify(pData));
                            // console.log('console.GetStringObject(\'TransInvoice.Rows.length\', pData) jsT79TT626MR!: ' + GetStringObject('TransInvoice.Rows.lenght', pData));
                            // console.table(GetStringObject('TransInvoice.Rows', pData));
                            pData = {
                                command: 'FromOrder',
                                MessageID: pMessageID,
                                Tables: pData,
                                ip: GetIpAddress()
                            };
                            SendMessage({
                                DestinationIp: pRemoteIp.IP,
                                ClientId: 'secondary',
                                Message: pData
                            });
                        } else if ((pRemoteIp.NextHandshake == null)
                            || (pRemoteIp.NextHandshake < new Date())) {

                            pRemoteIp.Interval += mHandshakeIntervalIncrement;
                            if (pRemoteIp.Interval > mHandshakeMaxInterval)
                                pRemoteIp.Interval = mHandshakeMaxInterval;
                            pRemoteIp.NextHandshake = new Date();
                            pRemoteIp.NextHandshake.setTime(pRemoteIp.NextHandshake.getTime() + pRemoteIp.Interval * 1000);

                            //Android.AddLog('begin handshaking jsYPE8AHHBEZ!');
                            SendMessage({
                                DestinationIp: pRemoteIp.IP,
                                ClientId: 'secondary',
                                Message: {
                                    command: 'Handshake',
                                    ip: GetIpAddress()
                                }
                            });
                            //Android.AddLog('end handshaking jsWNVHFWYWEN!');
                        } else {
                            // Android.AddLog('nothing to send; jsUVZE6FYMDL');
                            //Android.AddLog('not handshaking. not sending orders; jsTFB6F4YEQK');
                        }
                    } catch (e) {
                        Android.AddLog('ref: jsTMAZTPUQN8! ' + e.message);
                    }
                }
            };

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

            function GetTransmittableOrders(vIP) {
                var pOut = {};
                var pSqlFrom = ' FROM private.TransInvoice\n' +
                    ' WHERE NOT EXISTS(\n' +
                    '   SELECT *\n' + 
                    '   FROM private.TransPeerImport\n' +
                    '   WHERE TableName = \'TransInvoice\'\n' +
                    '   AND TransID = TransInvoice.ID\n' +
                    '   AND NOT (' +
                    '       (TransLastUpdated < TransInvoice.LastUpdated) \n' +
                    '       OR \n' +
                    '       (TransLastUpdated <> TransInvoice.LastUpdated \n' +
                    '       AND IFNULL(TransInvoice.InvoiceNumber, 0) > 0 )\n' +
                    '   ) AND DestinationIP = ' + ReadyString(vIP) + '\n' +
                    ')\n' +
                    ' ORDER BY LastUpdated\n' + 
                    ' LIMIT 10\n';
                var pSql = 'SELECT * \n' + 
                    pSqlFrom;
                var pDT = Sql.Execute(pSql);
                if (CBool(sessionStorage.getItem('IsOrderTaker')))
                    for (var p = 0; p <= pDT.Rows.length - 1; p++)
                        delete pDT.Rows[p]['DateImported'];
                pOut.TransInvoice = pDT.Rows;

                var pSqlFromItems = ' FROM private.TransInvoiceItems\n' +
                    ' WHERE InvoiceID IN (\n' +
                    '   SELECT ID\n' +
                    pSqlFrom +
                    ' )';
                pSql = 'SELECT *\n' +
                    pSqlFromItems;
                pDT = Sql.Execute(pSql);
                pOut.TransInvoiceItems = pDT.Rows;

                pSql = ' SELECT *\n' + 
                    ' FROM private.TransInvoiceItemsModifier\n' +
                    ' WHERE InvoiceItemID IN (\n' +
                    '   SELECT ID\n' +
                    pSqlFromItems +
                    ' )';
                pDT = Sql.Execute(pSql);
                pOut.TransInvoiceItemsModifier = pDT.Rows;

                pSql = 'SELECT * \n' +
                    ' FROM LibraryTableJoined \n' +
                    ' WHERE NOT EXISTS(\n' +
                    '   SELECT *\n' + 
                    '   FROM private.TransPeerImport\n' +
                    '   WHERE TableName = \'LibraryTableJoined\'\n' +
                    '   AND TransID = LibraryTableJoined.ID\n' +
                    '   AND TransLastUpdated >= LibraryTableJoined.LastUpdated\n' +
                    '   AND DestinationIP = ' + ReadyString(vIP) + '\n' +
                    ')\n';
                pDT = Sql.Execute(pSql);
                pOut.LibraryTableJoined = pDT.Rows;

                pSql = 'SELECT * \n' +
                    ' FROM LibraryTableTableInfo \n' +
                    ' WHERE NOT EXISTS(\n' +
                    '   SELECT *\n' + 
                    '   FROM private.TransPeerImport\n' +
                    '   WHERE TableName = \'LibraryTableTableInfo\'\n' +
                    '   AND TransID = LibraryTableTableInfo.ID\n' +
                    '   AND TransLastUpdated >= LibraryTableTableInfo.LastUpdated\n' +
                    '   AND DestinationIP = ' + ReadyString(vIP) + '\n' +
                    ')\n';
                pDT = Sql.Execute(pSql);
                pOut.LibraryTableTableInfo = pDT.Rows;

                pSqlFrom = ' FROM LibraryOrganization\n' +
                    ' WHERE IsFromDevice = 1 \n' + 
                    ' AND LastUpdated > DATETIME(' + proj.SqliteNow + ', \'-2 day\') \n' +
                    ' AND NOT EXISTS(\n' +
                    '   SELECT *\n' + 
                    '   FROM private.TransPeerImport\n' +
                    '   WHERE TableName = \'LibraryOrganization\'\n' +
                    '   AND TransID = LibraryOrganization.ID\n' +
                    '   AND TransLastUpdated >= LibraryOrganization.LastUpdated\n' +
                    '   AND DestinationIP = ' + ReadyString(vIP) + '\n' +
                    ')\n' +
                    ' ORDER BY LastUpdated\n' + 
                    ' LIMIT 10\n';
                pSql = 'SELECT * \n' + 
                    pSqlFrom;
                pDT = Sql.Execute(pSql);
                if (pDT.Rows.length > 0) 
                    console.log('Transmitable LibraryOrganization: ' + pDT.Rows.length + '. ref: jsWYK3RSWXXD!');
                pOut.LibraryOrganization = pDT.Rows;

                return pOut;
            } // END function GetTransmittableOrders() {
            
            function GetByIP(vIP) {
                for (var p = 0; p <= mRemoteIps.length - 1; p++) {
                    var pItem = mRemoteIps[p];
                    if (pItem.IP == vIP)
                        return  pItem;
                }
            }

            function GetIpAddress() {
                if (mSourceIp.length > 0)
                    return mSourceIp;
                else
                    return Android.IPAddress();
            }

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

});