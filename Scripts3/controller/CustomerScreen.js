define([
    '/Scripts3/model/PeerConnect.js?crc=',
    'promise!/Scripts3/NewMessageBox.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts3/view/CustomerScreen.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    $(function () {

        CreateStringObject('App.controller.CustomerScreen', null, function () {

            var mOut = {};
            var mElement;
            var mPeerConnect;
            var mMessageIDs = [];
            var mProductTable;
            var pAddLogIndex = 0;

            mElement = $(mHtml);
            mOut.element = mElement;
            var mDivCustomerScreenLog = mElement.find('#divCustomerScreenLog');

            if (InAndroid) {
                Transfers.StartSocketServer();
            }

            mPeerConnect = new App.model.PeerConnect();
            var pSettings = mPeerConnect.GetSettings();
            if (CStr2(pSettings.pin).trim() == '') {
                setTimeout(function() {
                    NewMessageBox('PIN is not set.', '', [
                        {
                            Text: 'Set PIN',
                            Click: function() {
                                require(['promise!/Scripts3/controller/SecondScreenChangePin.js?crc='], function() {
                                    var p = new App.controller.SecondScreenChangePin();
                                    p.popup();
                                });
                            }
                        }
                    ]);
                }, 200); // delay is needed
            } 
            //else
            //    setTimeout(ConnectToPrimary, 2000);

            CreateProductTable();

            proj.OnPinch = function() {
                $('#divCustomerScreenTopBar').show();
            };

            proj.OnFullScreen = function() {
                $('#divCustomerScreenTopBar').hide();
            };

            mOut.popup = function () {

                if (mElement.parent().length == 0) {
                    $('body').append(mElement);
                }

                $.mobile.changePage(mElement, {
                    allowSamePageTransition: true
                });
            };

            Transfers.AddReceiver(function(vMessage) {
                var pJson;

                try {
                    pJson = JSON.parse(vMessage.Message);
                    if (IsGuid(pJson.MessageID)
                        && (mMessageIDs.indexOf(pJson.MessageID) != -1))
                        return;

                    mMessageIDs.push(pJson.MessageID);
                    if (pJson.command == 'SetRole') {
                        if ($.isFunction(mOut.SetRoleCallback))
                            mOut.SetRoleCallback(pJson);
                        SaveLocalRole(pJson.value);
                    } else if (pJson.command == 'CustomerScreenItem') {
                        // AddLog('MessageID: ' + pJson.MessageID + '; ProductDescription: ' + pJson.ProductDescription);
                        AddProduct(pJson);
                    }
                } catch (e) {
                    console.log(e);
                }
            });

            function ConnectToPrimary(vOptions) {
                vOptions = $.extend({
                    ShowAsToast: false
                });
                var pIP = mPeerConnect.GetSettings().remoteIp;
                var pClientId = 'CustomerScreen ' + NewID();
                var pMessage = {
                    command: 'CustomerScreenInit'
                };
                var pStr = 'Connecting to ' + pIP;

                if (vOptions.ShowAsToast)
                    Android.Toast(pStr);
                else
                    Android.AddLog(pStr);
                try {
                    Transfers.SendMessage({
                        DestinationIp: pIP,
                        ClientId: pClientId,
                        Message: pMessage
                    });
                } catch (e) {
                    Android.Toast('Unable to connect to primary device');
                }
                setTimeout(ConnectToPrimary, 60000);
            }

            function AddLog(vText) {
                var pDiv;

                pAddLogIndex++;
                vText = pAddLogIndex + '; ' + vText;
                pDiv = $('<div>').text(vText);
                mDivCustomerScreenLog.append(pDiv);
                $('html, body').animate({
                    scrollTop: pDiv.offset().top + 'px'
                }, 'slow');
            }

            function CreateProductTable() {
                mProductTable = $('<table>')
                    .addClass('CustomerScreenTable');
                var pTr = $('<tr>');
                pTr.append($('<th>Desc.</th>').css('width', '52%'));
                pTr.append($('<th>Qty</th>').css('width', '20%'));
                pTr.append($('<th>Price</th>').css('width', '20%'));
                pTr.append($('<th>Total</th>').css('width', '38%'));
                mProductTable.append(pTr);
                mDivCustomerScreenLog.append(mProductTable);
            }

            function AddProduct(vJson) {
                var pTr;
                var pTotal = 0;

                if (pSettings.primaryDeviceIp != vJson.ip)
                    return;

                mProductTable.remove();
                CreateProductTable();
                for (var p = 0; p <= vJson.Products.length - 1; p++) {
                    var pItem = vJson.Products[p];
                    if (CDec2(pItem.Quantity) == 0)
                        continue;
                    pTr = $('<tr>');
                    pTr.append('<td>' + HtmlEncode(pItem.ProductName) + '</td>');
                    pTr.append('<td>' + HtmlEncode(pItem.Quantity + ' ' + pItem.UnitMeasure) + '</td>');
                    pTr.append('<td>' + HtmlEncode(pItem.UnitPrice) + '</td>');
                    var pSubtotal = CDec2(pItem.Quantity) * CDec2(pItem.UnitPrice);
                    pTotal += pSubtotal;

                    pTr.append('<td>' + HtmlEncode(AccountingFormat(pSubtotal)) + '</td>');
                    mProductTable.append(pTr);
                }
                pTr = $('<tr>');
                pTr.append('<td>Total:</td>');
                pTr.append('<td colspan=4>' + AccountingFormat(pTotal) + '</td>');
                mProductTable.append(pTr);

                
            } // END function AddProduct(vJson) {

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});