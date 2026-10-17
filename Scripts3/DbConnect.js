require(['common'
    ], function () {

    $(function () {

        var mTxtServer = $('#txtServer');
        var mTxtDatabase = $('#txtDatabase');
        var mDivSynonymLogs = $('#divSynonymLogs');

        ShowUser();
        LoadData();

        $('#chkIntegratedSecurity').change(function() {
            ShowUser();
        });

        function ShowUser() {
            if ($('#chkIntegratedSecurity').prop('checked'))
                $('#divUser').fadeOut();
            else
                $('#divUser').fadeIn();
        };

        $('#formMain').submit(function(e) {

            var pDbConnectPassword = $('#txtDbConnectPassword').val();
            var pServer = mTxtServer.val();
            var pDatabase = mTxtDatabase.val();
            var pIntegratedSecurity = $('#chkIntegratedSecurity').prop('checked');
            var pUsername = $('#txtUsername').val();
            var pPassword = $('#txtPassword').val();

            if (pDbConnectPassword.trim() == '') {
                MessageBox('Please enter DB Connect password');
                return false;
            };

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
                url: 'DbConnect/Save',
                data: {Server: pServer,
                    DbConnectPassword: pDbConnectPassword,
                    Database: pDatabase,
                    IntegratedSecurity: pIntegratedSecurity,
                    Username: pUsername,
                    Password: pPassword,
                    SaveAsEcommerceServerSqlServer: $('#chkSaveAsEcommerceServerSqlServer').prop('checked'),
                    WebPosRemoteServerIp: $('#txtWebPosRemoteServerIp').val()
                },
                success: function (vData) {
                    if (vData.Result == 'ok') 
                        MessageBox('DB Connection was successfully saved');
                    else if (vData.Error != undefined)
                        MessageBox(vData.Error, 'Error');
                }
            }); // AjaxSend
            
            return false;

        }); // function GetSettings

        function LoadData() {

            AjaxSend({
                url: '/DbConnect/GetData',
                success: function(vData) {
                    if (vData.Result == 'ok') {
                        DisplayData(vData);
                    } else if (vData.Error) {
                        MessageBox(vData.Error);
                    }
                }
            });

            function DisplayData(vData) {
                
                mTxtServer.val(vData.Server);
                mTxtDatabase.val(vData.Database);
                $('#txtWebPosRemoteServerIp').val(CStr2(vData.WebPosRemoteServerIp));

                $('#txtFlushingDatabaseServer').val(vData.FlushingServer);
                if (CStr2(vData.FlushingDatabase).trim() == '')
                    $('#txtFlushingDatabaseDatabase').val("Mobiz");
                else
                    $('#txtFlushingDatabaseDatabase').val(vData.FlushingDatabase);
            } // END DisplayData
        } // END LoadData

        $('#butSetSynonyms').click(function() {
            var pDbConnectPassword = $('#txtDbConnectPassword').val();
            mDivSynonymLogs.text('Setting synonyms. Please wait.');
            AjaxSend({
                url: 'DbConnect/SetSynonyms',
                data: {
                    DbConnectPassword: pDbConnectPassword,
                    PrivateDatabases: $('#txtPrivateDatabases').val()
                },
                success: function (vData) {
                    var pHtml = '';
                    mDivSynonymLogs.html('');
                    if (vData.Error != null)
                        pHtml += 'Error: ' + HtmlEncode(vData.Error);
                    if (vData.Successes != null) {
                        pHtml += 'Success: <br />';
                        for (var p = 0; p <= vData.Successes.length - 1; p++) {
                            pHtml += '<br />';
                            pHtml += HtmlEncode(vData.Successes[p]);
                        }
                    }

                    if (vData.Errors != null) {
                        pHtml += '<br /><br />Errors: <br />';
                        for (var p = 0; p <= vData.Errors.length - 1; p++) {
                            pHtml += '<br />';
                            pHtml += 'Database: ' + HtmlEncode(vData.Errors[p].Database) + '<br />';
                            pHtml += 'Message: ' + HtmlEncode(vData.Errors[p].Message) + '<br />';
                            pHtml += '<br />';
                        }
                    }
                    mDivSynonymLogs.html(pHtml);
                },
                error: function(e) {
                    mDivSynonymLogs.text('An error occured while set synonyms.');
                }
            }); // AjaxSend
        }); // END $('#butSetSynonyms').click(function() {

        $('#butBackupArchiverExecute').closest('.ui-btn').hide(); // hide button
        $('#butBackupArchiverDeleteArchives').closest('.ui-btn').hide(); // hide button
        var mBackupArchiveResultsCrc;
        $('#butBackupArchiver').click(function() {
            BackupArchiverGetFiles(false, 'ExecuteBackupArchiver');
        }); // END $('#butBackupArchiver').click(function() {
        
        $('#butBackupArchiverExecute').click(function() {
            BackupArchiverGetFiles(true, 'ExecuteBackupArchiver');
        }); // END $('#butBackupArchiver').click(function() {

        $('#butBackupArchiverGetArchives').click(function() {
            BackupArchiverGetFiles(false, 'ExecuteArchiveDeleter');
        }); // END $('#butBackupArchiver').click(function() {
        
        $('#butBackupArchiverDeleteArchives').click(function() {
            BackupArchiverGetFiles(true, 'ExecuteArchiveDeleter');
        }); // END $('#butBackupArchiver').click(function() {

        function BackupArchiverGetFiles(vExecute, vMethod) {
            $('#divBackupArchiver').html('');

            var pData = {};
            if (vExecute) {
                pData.Execute = true;
                pData.ResultsCrc = mBackupArchiveResultsCrc;
            }
                
            AjaxSend({
                url: 'Test/' + vMethod,
                data: pData,
                success: function (vData) {
                    mBackupArchiveResultsCrc = vData.ResultsCrc;
                    var pHtml = '';
                    if (vData.Error != null)
                        pHtml += 'Error: ' + HtmlEncode(vData.Error);
                    if ($.isArray(vData.Files)) {
                        if (CStr2(vData.Message) != '')
                            pHtml += HtmlEncode(vData.Message) + '<br />';
                        pHtml += 'Results CRC: ' + vData.ResultsCrc + '<br />';
                        for (var pFileIndex = 0; pFileIndex <= vData.Files.length - 1; pFileIndex++) {
                            var pFile = vData.Files[pFileIndex];
                            if (pFile.IsNewest)
                                pHtml += 'Preserve: ' + HtmlEncode(pFile.SourceFile) + '<br />';
                            else {
                                pHtml += 'Remove: ' + HtmlEncode(pFile.SourceFile) + '<br />';
                                if (CStr2(pFile.DestinationFile) != '')
                                    pHtml += 'Destination: ' + HtmlEncode(pFile.DestinationFile) + '<br />';
                            }
                            pHtml += 'File Last Write Time: ' + FormatDate(pFile.LastWriteTime, {Format: 'MMM dd, yyyy hh:mm:ss a'}) + '<br />';
                            pHtml += 'File Size: ' + AccountingFormat(pFile.Size, 0) + '<br /><br />';
                        }
                        if (CStr2(vData.ResultsCrc) != '')
                            if (vMethod == 'ExecuteBackupArchiver')
                                $('#butBackupArchiverExecute').closest('.ui-btn').show();
                            else
                                $('#butBackupArchiverDeleteArchives').closest('.ui-btn').show();
                    }

                    if (vData.Errors != null) {
                        pHtml += '<br /><br />Errors: <br />';
                        for (var p = 0; p <= vData.Errors.length - 1; p++) {
                            pHtml += '<br />';
                            pHtml += 'Database: ' + HtmlEncode(vData.Errors[p].Database) + '<br />';
                            pHtml += 'Message: ' + HtmlEncode(vData.Errors[p].Message) + '<br />';
                            pHtml += '<br />';
                        }
                    }
                    $('#divBackupArchiver').html(pHtml);
                },
                error: function(e) {
                    $('#divBackupArchiver').text('An error occured while Test/BackupArchiver.');
                }
            }); // AjaxSend
        }; // END function BackupArchiverGetFiles() {

        $('#butSaveFlushingDatabase').click(function() {
            var pDbConnectPassword = $('#txtDbConnectPassword').val();
            var pServer = $('#txtFlushingDatabaseServer').val();
            var pDatabase = $('#txtFlushingDatabaseDatabase').val();
            var pIntegratedSecurity = $('#chkFlushingDatabaseIntegratedSecurity').prop('checked');
            var pUsername = $('#txtFlushingDatabaseUsername').val();
            var pPassword = $('#txtFlushingDatabasePassword').val();
            var pDatabaseFolder = $('#txtFlushingDatabaseFolder').val();

            if (pDbConnectPassword.trim() == '') {
                MessageBox('Please enter DB Connect password');
                return false;
            };

            if (pServer.trim() == '') {
                MessageBox('Please enter server');
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
                url: 'DbConnect/SaveFlushingDatabase',
                data: {FlushingServer: pServer,
                    DbConnectPassword: pDbConnectPassword,
                    FlushingDatabase: pDatabase,
                    FlushingIntegratedSecurity: pIntegratedSecurity,
                    FlushingUsername: pUsername,
                    FlushingPassword: pPassword,
                    DatabaseFolder: pDatabaseFolder
                },
                success: function (vData) {
                    if (vData.Result == 'ok') 
                        MessageBox('Flushing DB Connection was successfully saved');
                    else if (vData.Error != undefined)
                        MessageBox(vData.Error, 'Error');
                }
            }); // AjaxSend
            
            return false;
        });

        $('#butOthersDetachWebPosDb').click(function() {
            AjaxSend({
                url: 'DbConnect/DetachWebPosDb',
                data: {
                    DbConnectPassword: $('#txtDbConnectPassword').val()  
                },
                success: function (vData) {
                    $('#divOthersLog').html(vData.Message);
                }
            }); // AjaxSend
        });

        $('#butOthersAttachWebPosDb').click(function() {
            AjaxSend({
                url: 'DbConnect/AttachWebPosDb',
                data: {
                    DbConnectPassword: $('#txtDbConnectPassword').val()  
                },
                success: function (vData) {
                    $('#divOthersLog').html(vData.Message);
                }
            }); // AjaxSend
        });

    }); // $(function() {

});