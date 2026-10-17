define(['common',
    '/Scripts3/schemaSync/Sender.js?crc='
    ], function () {

    $(function() {
        
        var txtSchemaSyncRemoteIP = $('#txtSchemaSyncRemoteIP');

        $('#butSchemaSyncSend').click(function() {

            if (txtSchemaSyncRemoteIP.val().trim() == '') {
                MessageBox('Remote IP is required');
                return;
            }
                
//            App.schemaSync.Sender.remoteIp = txtSchemaSyncRemoteIP.val();
//            App.schemaSync.Sender.send({
//                onProgress: function(vMessage) {
//                },
//                onSuccess: function() {
//                }
//            });

        }); // END $('#butSend').click(function() {

    }); // END $(function() {

}); // define

