define(['common',
    'business',
    '/Scripts3/Offline/Web.js?crc=',
    '/Scripts3/controller/CustomerScreenTransmitter.js?crc='
    ], function () {
 
    var mSocket;
    var mTxtUrl = $('#txtUrl');
    var mButConnectSocket = $('#butConnectSocket');
    var mDivMessage = $('#divMessage');
    var mTxtMessage = $('#txtMessage');
    var mButSend = $('#butSend');
    var mDivLog = $('#divLog');
    var mClientID = NewID();
    var mButIncrement = $('#butIncrement');
    var mButIncrementLocal = $('#butIncrementLocal');
    var mServerCount = 0;
    var mFilename = 'AndroidJsTestSend.txt';

    try {
        mTxtUrl.val(Transfers.ReadFile(mFilename));
    } catch (e) {
        console.error(e);
    }

    if (InAndroid) {
        Transfers.StartSocketServer();
    };

    function Connect() {

    }

    function OnMessage(vData) {

        Sender.Print(vData);

    }

    var mReceivingFunction = [];
    var mReceivingFunctionIndex = 0;

    mButSend.click(function() {

        var pMessage = mTxtMessage.val();

        if (pMessage.trim() == '')
            return;

        if (mTxtUrl.val().trim() == '') {
            MessageBox('Please enter server URL first');
            mTxtUrl.focus();
            return;
        }

        mReceivingFunctionIndex++;
        mReceivingFunction[mReceivingFunctionIndex] = function(vParams) {
            console.log('jsKHF698NHHX!');
        };

        pMessage = {
            Message: pMessage,
            Index: mReceivingFunctionIndex
        };

        Transfers.SendMessage({
            DestinationIp: mTxtUrl.val(),
            ClientId: mClientID,
            Message: pMessage
        });
        mTxtMessage.val('');
        Transfers.WriteFile(mFilename, null, mTxtUrl.val());

    });

    mButIncrement.click(function() {

        var pMessage;

        if (mTxtUrl.val().trim() == '') {
            MessageBox('Please enter server URL first');
            mTxtUrl.focus();
            return;
        }

        pMessage = "Increment server count";

        Transfers.SendMessage({
            DestinationIp: mTxtUrl.val(),
            ClientId: mClientID,
            Message: pMessage
        });
        mTxtMessage.val('');
        localStorage.SocketRemoteIP = mTxtUrl.val();

    });

    mButIncrementLocal.click(function() {
        mServerCount++;
        $('#spanLocalCounter').text(mServerCount);
    });

    Transfers.AddReceiver(function(vMessage) {
        Sender.ReceiveText(vMessage);
    });

    Transfers.AddClientReceiver(function(vMessage) {
        Sender.ReceiveText(vMessage);
    });

    Transfers.AddOnClientConnected(function(vData) {
        
        var pButton;

        Sender.Print('Client, ' + vData.ClientId + ' has connected');
        pButton = $('<button>')
            .attr('id', 'but' + vData.ClientId)
            .data('id', vData.ClientId)
            .text('Send to ' + vData.ClientId)
            .click(function(e) {
                Transfers.SendMessageToClient(mTxtMessage.val(), $(this).data('id'));
                mTxtMessage.val('');
            });
        $('#divButtons')
            .append(pButton)
            .trigger('create');
    });

    var mTransmitter = new App.controller.CustomerScreenTransmitter();

    $('#butCustomerTransmit').click(function() {
        mTransmitter.AddProduct({
            "Products": [
                {
                    "ProductID": "4D325100-4BDC-48FF-A748-9C33512CDAC0",
                    "ProductName": "Sweet & Sour Fish Fillet", 
                    "ModifierGroupID":"", 
                    "DiscountedPrice": "130",
                    "UnitMeasure": "Order",
                    "UnitMeasureID": "a79de8d4-b121-494a-9d29-c095a51f64dc", 
                    "Quantity": RandomInteger(1, 999),
                    "ActualQuantity":1, 
                    "IsOrderSlipPrinted":0
                }
            ],
            "Total": 130,
            "command": "CustomerScreenItem",
            "MessageID": NewID()
        });
    });

    window.Sender = {
        Print: function(vText) {
            var pDiv;

            pDiv = $('<div>');
            pDiv.text(vText);
            mDivLog.append(pDiv);

        },

        Receive: function(vJson) {
            // called by Android websocket server. Java function name: PassToJs

            try {
                if (vJson.Command == 'ReceiveText')
                    Sender.ReceiveText(vJson);
                else if (vJson.Command == 'Connected')
                    Sender.ClientConnected(vJson);        
                else if (vJson.Command == 'Disconnected')
                    Sender.ClientDisconnected(vJson);        
            } catch (e) {
                MessageBox(e);
            }

        }, 

        ReceiveText: function(vJson) {

            var pJson;
            
            if (vJson.Message == 'Increment server count') {
                mServerCount++;
                $('#spanLocalCounter').text(mServerCount);
                Transfers.SendMessageToClient({
                    Type: 'ServerCount',
                    Value: mServerCount
                }, vJson.ClientId);
            } else {
                try {
                    pJson = JSON.parse(vJson.Message);
                    if (pJson.Type == 'ServerCount') {
                        $('#spanServerCounter').text(pJson.Value);
                    }
                } catch (e) {
                    //console.log('js2VAVFYGTF5!');
                    //console.error(e);
                } 
            }
            Sender.Print(vJson.ClientId + ' says ' + vJson.Message);

        },

        ClientConnected: function(vJson) {
            
            var pButton;

            Sender.Print(vJson.IP + ' has connected.');
            mDivMessage.show();
            pButton = $('<button>')
                .attr('id', 'but' + vJson.ID)
                .data('id', vJson.ID)
                .text('Send to ' + vJson.ID)
                .click(function(e) {
                    Android.SendToClient($(this).data('id'), mTxtMessage.val());
                    mTxtMessage.val('');
                });
            $('#divButtons')
                .append(pButton)
                .trigger('create');

        },

        ClientDisconnected: function(vJson) {

            Sender.Print(vJson.IP + ' has disconnected.');
            $('#but' + vJson.ID).closest('.ui-btn').remove();
            $('#but' + vJson.ID).remove();

        }
    }; // END window.Sender
});