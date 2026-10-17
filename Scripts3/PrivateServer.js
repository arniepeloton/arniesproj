require(['common'
    ], function () {

    $(function () {

        var mTxtServer = $('#txtServer');
        var mTxtDatabase = $('#txtDatabase');
        var mMessageKey;

        ShowUser();
        LoadDbStatus();

        $('#chkIntegratedSecurity').change(function() {
            ShowUser();
        });

        function ShowUser() {
            if ($('#chkIntegratedSecurity').prop('checked'))
                $('#divUser').fadeOut();
            else
                $('#divUser').fadeIn();
        };

        $('#formDbConnect').submit(function(e) {

            var pServer = mTxtServer.val();
            var pDatabase = mTxtDatabase.val();
            var pIntegratedSecurity = $('#chkIntegratedSecurity').prop('checked');
            var pUsername = $('#txtUsername').val();
            var pPassword = $('#txtPassword').val();

            if (pServer.trim() == '') {
                MessageBox('Please enter server');
                return false;
            };

            if (pDatabase.trim() == '') {
                MessageBox('Please enter database');
                return false;
            };

            if (!pIntegratedSecurity) {
                if (pUsername.trim() == '') {
                    MessageBox('Please enter username');
                    return false;
                };

                if (pPassword == '') {
                    MessageBox('Please enter password');
                    return false;
                };
            };

            AjaxSend({
                url: 'PrivateServer/SaveDatabaseConnection',
                data: {Server: pServer,
                    NoUpdate: 1,
                    Database: pDatabase,
                    IntegratedSecurity: pIntegratedSecurity,
                    Username: pUsername,
                    Password: pPassword
                },
                success: function (vData) {
                    if (vData.Result == 'ok') {
                        mMessageKey = vData.MessageKey;
                        if (CStr2(mMessageKey).length != 20) {
                            MessageBox('Invalid message key length jsJTLDRACEFD!');
                            return;
                        }
                        MessageBox('DB Connection was successfully saved', 'Success', null, {
                            OnClose: function() {
                                setTimeout(FollowUp, 5000);
                            }
                        });
                    } else if (vData.Error != undefined)
                        MessageBox(vData.Error, 'Error');
                }
            }); // AjaxSend
            
            return false;

        }); // function GetSettings

        function LoadDbStatus() {

            AjaxSend({
                url: '/PrivateServer/DatabaseSetupStatus',
                success: function (vData) {
                    if (vData.NoSettings) {
                        $('#divDatabaseMessage').text('Please configure database connection settings. Try entering the password you have provided during SQL Server installation.');
                    }

                    if (vData.DbIsConnected) {  
                        $('#divDatabaseMessage').text('This website can connect to the configured database.');
                        $('#formDbConnect').hide();
                    }

                    if (vData.Error) {
                        MessageBox(vData.Error);
                    }
                },
                error: function (vError) {
                    MessageBox(vData.Error, 'Error');
                }
            });

        } // END LoadDbStatus

        function FollowUp() {

            AjaxSend({
                url: '/PrivateServer/GetStatus',
                data: {
                    MessageKey: mMessageKey
                },
                success: function (vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                    }

                },
                error: function (vError) {
                    MessageBox(vData.Error, 'Error');
                }
            });
        }
    }); // $(function() {

});