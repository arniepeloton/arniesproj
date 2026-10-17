define([], function() {
    var mZipCache = 'zipcache/';

    var mReceivers = [];
    var mOnClientConnected = [];
    var mClientReceivers = [];

    window.Transfers = {
        ReadFile: function(vFilename, vInternalFilename, vUseAppPath) {
            // RETURNS: string. returns null if file not found
            // PARAMS:
            //  vFilename: vfilename inside device
            //  vInternalFilename (optional): if vFilename is a zip file, vInternalFilename is the filename inside vFilename

            return Js.Execute({
                command: 'Transfers.ReadFile',
                Filename: vFilename,
                InternalFilename: vInternalFilename,
                UseAppPath: vUseAppPath
            });
        }, // ReadFile
        WriteFile: function(vFilename, vInternalFilename, vContent) {

            try {
                if ($.isPlainObject(vContent)) {
                    vContent = JSON.stringify(vContent)
                }
            } catch (e) {
                console.trace(e.message);
            }

            if (vInternalFilename != undefined) {
                vFilename = mZipCache + vFilename + '/' + vInternalFilename;
                vInternalFilename = undefined;   
            };
                         
            return Js.Execute({
                command: 'Transfers.WriteFile',
                DestinationZipFilename: vFilename,
                InternalFilename: vInternalFilename,
                Content: vContent
            });

        }, // WriteFile 
        DeleteFile: function(vFilename, vInternalFilename) {
            // PARAMS:
            //  vInternalFilename (optional), if null = deletes the vFilename

            var pData = {command: 'Transfers.DeleteFile'};

            if (vInternalFilename == undefined) 
                pData.Filename = vFilename;
            else {
                pData.ZipFilename = vFilename;
                pData.InternalFilename = vInternalFilename;
            };
            return Js.Execute(pData);

        },
        Download: function(vOptions) {
            // vOptions:
            //  Key
            //  DestinationFilename

            if (vOptions.Error == undefined) {
                vOptions.Error = function() {
                    Android.Toast('An error occured while downloading file. This error message could use some details.');
                };
            };

            var pCallbacks = {
                Success: vOptions.Success,
                Error: vOptions.Error
            };

            vOptions.Command = 'Download';
            if (vOptions.Source == null)
                vOptions.Source = 'MobizExport/Download?' + $.param({fID: vOptions.Key});
            vOptions.Destination = vOptions.Destination;
            mobile.CallWithCallback(vOptions, pCallbacks);

        },
        Upload: function(vOptions) {
            // vOptions:

            //  requires 1 of the following:
            //  SourceFilename (optional)
            //  SourceDirectoryName (optional)

            //  Destination: destination (optional). if null, '/AndroidJsTest/Dump'
            //  Data: custom data
            if (vOptions.Error == undefined) {
                vOptions.Error = function(vData) {
                    if ((vData != undefined)
                        && (vData.Error != undefined))
                        
                        Android.Toast(vData.Error);
                    else
                        Android.Toast('An error occured while uploading file. No error message was provided by Android.');
                };
            };

            var pCallbacks = {
                Success: vOptions.Success,
                Error: vOptions.Error
            };

            vOptions.Command = 'Upload';
            vOptions.SourceFilename = vOptions.SourceFilename;
            vOptions.SourceDirectoryName = vOptions.SourceDirectoryName;
            vOptions.Data = vOptions.Data;
            if (vOptions.Destination == undefined)
                vOptions.Destination = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID
                }, '/AndroidJsTest/Dump');
            // MobizExport/UploadRequestedData
            mobile.CallWithCallback(vOptions, pCallbacks);

        }, // END Upload
        GetInternalFiles: function(vZipFilename) {
            // RETURNS: string. returns null if file not found

            return Js.Execute({
                command: 'Transfers.GetInternalFiles',
                ZipFilename: vZipFilename
            });
        }, // END GetInternalFiles
        ExtractZipFile: function(vOptions) {

            if (!$.isFunction(vOptions.Success))
                throw new Error('Success must be a function');

            var pOptions = {
                Command: 'ExtractZipFile',
                Source: vOptions.Source,
                ExtractToFiles: vOptions.ExtractToFiles,
                Destination: (vOptions.Destination == undefined) ? 'extracted/' + NewID() : vOptions.Destination,
                AutoDelete: (vOptions.AutoDelete == undefined) ? true : vOptions.AutoDelete // AutoDelete by default
            };
            var pCallbacks = {
                Success: function(vData) {
                    vOptions.Success(vData);
                    if (pOptions.AutoDelete)
                        try {
                            Transfers.DeleteDirectory(vData.Destination);
                        } catch (e) {
                            console.error('An error occured while auto deleting folder: ' + e);
                        };
                },
                Error: vOptions.Error
            };

            mobile.CallWithCallback(pOptions, pCallbacks);

        }, // END ExtractZipFile
        DeleteDirectory: function(vDirectoryName) {

            return Js.Execute({
                command: 'Transfers.DeleteDirectory',
                DirectoryName: vDirectoryName
            });

        }, // END DeleteDirectory
        CsvWrite: function(vParams) {
            
            return Js.Execute({
                command: 'Transfers.CsvWrite',
                ID: vParams.ID,
                ZipFilename: vParams.ZipFilename,
                InternalFilename: vParams.InternalFilename,
                Content: vParams.Content
            });

        }, // END CsvWrite
        CsvCommit: function(vID) {
            
            return Js.Execute({
                command: 'Transfers.CsvCommit',
                ID: vID
            });

        }, // END CsvCommit
        CsvFinalCommit: function(vID, vZipFilename) {

            if (vID != undefined)
                return Js.Execute({
                    command: 'Transfers.CsvFinalCommit',
                    ID: vID
                });
            else
                return Js.Execute({
                    command: 'Transfers.CsvFinalCommit',
                    ZipFilename: vZipFilename
                });

        }, // END CsvCommit
        CsvDelete: function(vZipFilename) {

            return Js.Execute({
                command: 'Transfers.CsvDelete',
                ZipFilename: vZipFilename // this is actually the directory name
            });

        }, // END CsvCommit
        BeginZip: function(vZipFilename) {
            return DeleteDirectory(mZipCache + vZipFilename);
        },
        CommitZip: function(vZipFilename) {
            return Js.Execute({
                command: 'Transfers.CommitZip',
                ZipFilename: vZipFilename
            });
        },
        IsValidZipFile: function(vFilename) {
            return CBool(Js.Execute({
                command: 'Transfers.IsValidZipFile',
                Filename: vFilename
            }));
        },
        MoveFile: function(vSourceFilename, vDestinationFilename) {
            return CBool(Js.Execute({
                command: 'Transfers.MoveFile',
                SourceFilename: vSourceFilename,
                DestinationFilename: vDestinationFilename
            }));
        },
        StartSocketServer: function() {
            Js.Execute({
                command: 'Transfers.StartSocketServer'
            });
        },
        AddReceiver: function(vFunction) {
            if (!$.isFunction(vFunction))
                throw 'Transfers.AddReceiver: 1st param must be a function';
            mReceivers.push(vFunction);
        },
        OnReceive: function(vMessage) {
            for (var p = 0; p <= mReceivers.length - 1; p++) 
                mReceivers[p](vMessage);
        },
        SendMessage: function(vData) {
            if (vData == null)
                throw new Error('vData must not be null. jsV4SUTRSE67!');
            if (CStr2(vData.DestinationIp).trim() == '')
                throw new Error('vData.DestinationIp must not be blank. jsKJ6MHS2CBF!');
            Js.Execute({
                command: 'Transfers.SendMessage',
                DestinationIp: vData.DestinationIp,
                ClientId: vData.ClientId,
                Message: vData.Message
            });
        },
        AddOnClientConnected: function(vFunction) {
            if (!$.isFunction(vFunction))
                throw 'Transfers.AddOnClientConnected: 1st param must be a function';
            mOnClientConnected.push(vFunction);
        },
        OnClientConnected: function(vMessage) {
            for (var p = 0; p <= mReceivers.length - 1; p++) 
                if ($.isFunction(mOnClientConnected[p]))
                    mOnClientConnected[p](vMessage);
        },
        SendMessageToClient: function(vMessage, vClientId) {
            return Js.Execute({
                command: 'Transfers.SendMessageToClient',
                ClientId: vClientId,
                Message: vMessage
            });
        },
        AddClientReceiver: function(vFunction) {
            if (!$.isFunction(vFunction))
                throw 'Transfers.AddClientReceiver: 1st param must be a function';
            mClientReceivers.push(vFunction);
        },
        OnClientReceive: function(vMessage) {
            for (var p = 0; p <= mReceivers.length - 1; p++) 
                if ($.isFunction(mClientReceivers[p]))
                    mClientReceivers[p](vMessage);
        },
        CsvAddFile: function(vParams) {
            return Js.Execute({
                command: 'Transfers.CsvAddFile',
                ID: vParams.ID,
                ZipFilename: vParams.ZipFilename,
                InternalFilename: vParams.InternalFilename,
                SourceFilename: vParams.SourceFilename,
                DestinationFilename: vParams.DestinationFilename
            });
        },
        GetAllSubFolders: function (vFilename) {
            // RETURNS: string. returns null if file not found
            // PARAMS:
            //  vFilename: vfilename inside device
            //  vInternalFilename (optional): if vFilename is a zip file, vInternalFilename is the filename inside vFilename

            return Js.Execute({
                command: 'Transfers.GetAllSubFolders',
                Filename: vFilename
            });
        }, // GetAllSubFolders
        GetFiles: function (vFolder) {
            // RETURNS: string. returns null if file not found
            // PARAMS:
            //  vFilename: vfilename inside device
            //  vInternalFilename (optional): if vFilename is a zip file, vInternalFilename is the filename inside vFilename

            return Js.Execute({
                command: 'Transfers.GetFiles',
                Folder: vFolder
            });
        } // GetAllSubFolders
    }; // Android
}); // define