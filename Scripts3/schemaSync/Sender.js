define(['common'], function() {
    
    CreateStringObject('App.schemaSync.Sender', null,
        (function() {
            
            var $this = {
                remoteIp: null,
                execute: execute,

                // needed only for tests
                privates: {
                    createSnapshot: createSnapshot,
                    sendSnapshot: sendSnapshot,
                    compare: compare,
                    followUp: followUp
                }
            };
            var mMessageIndex;

            function execute(vParams) {
                
                /***
                vParams = {
                    onProgress: function(vData) {
                        console.log(vData.message);
                    },
                    onSuccess: function() {
                    },
                    onError: function() {
                    }
                }; 
                ***/

                if (CStr2($this.remoteIp).trim() == '') {
                    RaiseError('App.schemaSync.Sender.remoteIp must be set before calling send js88BPU4XYEL!');
                }
            
                createSnapshot({
                    onProgress: vParams.onProgress,
                    onSuccess: function() {
                        sendSnapshot({
                            onProgress: vParams.onProgress,
                            onSuccess: function() {
                                compare({
                                    onProgress: vParams.onProgress,
                                    onSuccess: function() {
                                        followUp({
                                            onProgress: vParams.onProgress,
                                            onSuccess: vParams.onSuccess,
                                            onError: vParams.onError
                                        });
                                    },
                                    onError: vParams.onError
                                });
                            },
                            onError: vParams.onError
                        }); // END sendSnapshot
                    },
                    onError: vParams.onError
                }); // END createSnapshot

            } // END execute

            function createSnapshot(vParams) {

                /***
                vParams = {
                    onProgress: function(vData) {
                        console.log(vData.message);
                    },
                    onSuccess: function() {
                        
                    },
                    onError: function(vData) {
                        console.log(vData.message)
                    }
                }; 
                ***/

                vParams.onProgress('Sending command to local server to create database snapshot.');
                AjaxSend({
                    ExactUrl: '/SchemaSyncSender/CreateSnapshot',
                    success: function(vData) {
                        if (vData.Result == 'ok') {
                            vParams.onProgress({
                                message: 'Database snapshot was successfully created on the local server.'
                            });
                            vParams.onSuccess();
                        } else
                            vParams.onError({
                                message: vData.Error
                            });
                    },
                    error: function() {
                        vParams.onError(arguments);
                    }
                });

            } // END createSnapshot

            function sendSnapshot(vParams) {

                /***
                vParams = {
                    onProgress: function(vData) {
                        console.log(vData.message);
                    },
                    onSuccess: function() {
                        
                    },
                    onError: function() {
                    }
                }; 
                ***/

                vParams.onProgress('Sending database snapshot to remote server.');
                AjaxSend({
                    ExactUrl: '/SchemaSyncSender/SendSnapshot',
                    data: {
                        remoteIp: $this.remoteIp
                    },
                    success: function(vData) {
                        if (vData.Result == 'ok') {
                            vParams.onProgress('Database snapshot was successfully received by the remote server.');
                            vParams.onSuccess();
                        } else
                            vParams.onError();
                    },
                    error: function() {
                        console.log('js6BYCKCQVLL!');
                        vParams.onError();
                    }
                });

            } // END sendSnapshot

            function compare(vParams) {

                /***
                vParams = {
                    onProgress: function(vData) {
                        console.log(vData.message);
                    },
                    onSuccess: function() {
                        
                    },
                    onError: function() {
                    }
                }; 
                ***/

                var pUrl;

                pUrl = $this.remoteIp;
                if (!pUrl.endsWith('/'))
                    pUrl += '/';
                pUrl += 'SchemaSyncReceiver/Compare';

                vParams.onProgress('Comparing databases on the remote server.');
                AjaxSend({
                    ExactUrl: pUrl,
                    data: {
                    },
                    success: function(vData) {
                        if (vData.Result == 'ok') {
                            mMessageIndex = 0;
                            vParams.onSuccess();
                        } else
                            vParams.onError();
                    },
                    error: function() {
                        vParams.onError();
                    }
                });

            } // END compare

            function followUp(vParams) {

                /***
                vParams = {
                    onProgress: function(vData) {
                        console.log(vData.message);
                    },
                    onSuccess: function() {
                    },
                    onError: function() {
                    }
                }; 
                ***/

                var pUrl;

                pUrl = $this.remoteIp;
                if (!pUrl.endsWith('/'))
                    pUrl += '/';
                pUrl += 'SchemaSyncReceiver/GetMessages';

                AjaxSend({
                    ExactUrl: pUrl,
                    data: {
                        MessageIndex: mMessageIndex
                    },
                    success: function(vData) {
                        if (IsNumeric(vData.Index)) {
                            mMessageIndex = vData.Index;
                        
                            for (var p = 0; p <= vData.Messages.length - 1; p++)
                                vParams.onProgress({
                                    message: vData.Messages[p]
                                });
                            if (CBool(vData.IsDone))
                                if (vData.Result == 'ok')
                                    vParams.onSuccess();
                                else
                                    vParams.onError({
                                        message: vData.Error
                                    });
                            else
                                setTimeout(function() {
                                    followUp(vParams);
                                }, 5000);
                        } else
                            setTimeout(function() {
                                followUp(vParams);
                            }, 5000);
                    },
                    error: function() {
                        console.log('SchemaSyncReceiver error js5N9AQ84GVV!');
                        vParams.onError();
                    }
                });

            } // END followUp

            return $this;

        })() // END (function() {
    ); // END CreateStringObject('App.schemaSync.Sender', null,

});