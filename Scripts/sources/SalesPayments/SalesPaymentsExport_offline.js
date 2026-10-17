define(['common',
    'business',
    '/Scripts3/Offline/Web.js?crc=',
    '/Scripts3/Offline/DataSummary.js?crc=',

    '/Scripts3/Offline/DataFlush.js?crc=1'
    ], function () {

        var mData;
        var mPopup;

        if (window.SalesPaymentsExport_offline != undefined)
            return;

        window.SalesPaymentsExport_offline = function () {

            this.Export = function (vData) {
                /*
                vData = {
                    OnProgress: function(vData) {
                        console.log(vData.Message);
                    },
                    OnDone: function(vData) {
                        console.log(vData.Result == 'success');
                        console.log(vData.Result == 'error');
                        console.log(vData.Message);
                    }
                };
                */
                var pFunc;

                if (CBool(localStorage.NoFlushing)) {
                    console.log('localStorage.NoFlushing is true. data export will not execute. jsK9GRYZYUTZ!');
                    return;
                }
                if (CStr2(Web.OrgID()) == '')
                    throw 'Please set the current OrgID before calling SalesPaymentsExport_offline.Export';

                mData = $.extend({}, vData);
                if (window.InWinRt)
                // if (true) // debugging code only
                    pFunc = window.DataFlush.FollowUp;
                else
                    pFunc = mobile.DataExport;

                pFunc({
                    OrgID: Web.OrgID(),
                    Success: function(vData) {
                        if ($.isFunction(mData.OnDone))
                            mData.OnDone(vData);
                    },
                    Error: function(vData) {
                        if ($.isFunction(mData.OnDone))
                            mData.OnDone(vData);
                    }
                });


                return; 
                // dont use javascript flushing.
                // use java flushing
                if (!$.isFunction(mData.OnProgress))
                    mData.OnProgress = function () { };
                if (!$.isFunction(mData.OnDone))
                    mData.OnDone = function () { };

                FollowUp(vData);

            }; // END Export

        this.HasUnflushedRecords = function (vParams) {
            // requires:
            //  sessionStorage.OrgID
            vParams = $.extend({
                SrvOnly: false
            }, vParams);

            var pSql;

            if (!vParams.SrvOnly) {
                pSql = Sql.ExportableDataCount();
                if (CDec2(pSql) > 0)
                    return true;
                return false;
            }

            //Web.SetOrgID(sessionStorage.OrgID);
            //Sql.AttachDatabase(sessionStorage.OrgID);

            pSql = 'Select COUNT(*) AS RecordCount from TransMemo where DateImported IS NULL';
            pSql = Sql.ExecuteScalar(pSql);
            if (CDec2(pSql) > 0)
                return true;

            pSql = 'Select COUNT(*) AS RecordCount from TransMemoSalesReturn where DateImported IS NULL';
            pSql = Sql.ExecuteScalar(pSql);
            if (CDec2(pSql) > 0)
                return true;

            pSql = 'Select COUNT(*) AS RecordCount from TransSalesReturnLedger where DateImported IS NULL';
            pSql = Sql.ExecuteScalar(pSql);
            if (CDec2(pSql) > 0)
                return true;

            //pSql = 'Select * from LibraryOrganization where IsFromDevice = 1 AND FlushToServer = 1';
            //pSql = Sql.ExecuteScalar(pSql);
            //if (CDec2(pSql) > 0)
            //    return true;

            //pSql = 'Select * from LibraryDevicePrinterFowst where FlushToServer = 1';
            //pSql = Sql.ExecuteScalar(pSql);
            //if (CDec2(pSql) > 0)
            //    return true;


            //pSql = 'SELECT * FROM LibraryFOWSTBinProduct WHERE private.LibraryFOWSTBinProduct.DateImported IS NULL and MaxPerProduct>0'
            //pSql = Sql.ExecuteScalar(pSql);
            //if (CDec2(pSql) > 0)
            //    return true;

            //pSql = 'SELECT * FROM TransInventoryProductCount WHERE private.TransInventoryProductCount.DateImported IS NULL'
            //pSql = Sql.ExecuteScalar(pSql);
            //if (CDec2(pSql) > 0)
            //    return true;

            //pSql = ' SELECT * FROM private.LibraryFOWSTBin WHERE DateImported IS NULL AND Name<> \'Unnamed Bin\''
            //pSql = Sql.ExecuteScalar(pSql);
            //if (CDec2(pSql) > 0)
            //    return true;

            return false;

        }; // END HasUnflushedRecords

        var mFlushedCounter = 0;

        this.ResetFlushCounter = function() {
            mFlushedCounter = 0;
        };

        this.HandleUnflushedRecords = function (vParams) {
            //            vParams = {
            //                Success: function() {
            //                }
            //            };

            vParams = $.extend({}, vParams);
            if (this.HasUnflushedRecords()) {
                mFlushedCounter = 0;
                setTimeout(function () {
                    FlushDeviceData(
                        $.extend({}, vParams, {
                            Success: function () {
                                HideMessage();
                                if ($.isFunction(vParams.Success))
                                    setTimeout(vParams.Success, 700); // delay is needed
                                // vParams.Success(); // dont do this
                            },
                            OnProgress: function (vData) {
                                ShowMessage(vData.Message);
                            }
                        })
                    );
                }, 1000);

                mPopup = MessageBox('Oops! Looks like you still have some unflushed (unsent) data to the Mobiz Cloud Server.\n\n' +
                    'You can view online dashboards after all data in your device has been flushed into the Mobiz Cloud Server’s database.\n\n' +
                    'While waiting for the data to be flushed, would you like to:', '',
                    [
                        {
                            Text: 'View the offline dashboards instead',
                            Click: vParams.OfflineMenu
                        },
                        {
                            Text: 'Go back to the Main Menu and continue with other POS transactions. You can view online dashboards once all data in your device has been sent to Mobiz Cloud Server.',
                            Click: function() {
                                mPopup.dialog('close');
                            }
                        }
                    ]
                );
            } else {
                if ($.isFunction(vParams.Success))
                    vParams.Success();
            }

        }; // END HandleUnflushedRecords

        function FlushDeviceData(vParams) {
            // this function is and must be independent from UI elements
            //            vParams = {
            //                Success: function(vData) {
            //                },
            //                OnProgress: function(vData) {
            //                }
            //            };
            // requires:
            //  sessionStorage.OrgID

            var $this = this;
            Sql.AttachDatabase(sessionStorage.OrgID + '.private.db');
            $this.Export({
                OnProgress: vParams.OnProgress,
                OnDone: function (vData) {
                    mFlushedCounter++;
                    if ($this.HasUnflushedRecords()
                        || (vData.Result != 'ok')) {

                        if (mFlushedCounter < 3)
                            FlushDeviceData(vParams);
                        else {
                            console.log('Data flushing retry count exceeded.');
                            if ($.isFunction(vParams.Fail)) 
                                vParams.Fail();
                        }
                    } else {
                        if ($.isFunction(vParams.Success))
                            vParams.Success();
                    }
                }
            });

        } // END FlushDeviceData
        this.FlushDeviceData = FlushDeviceData;

        function ShowMessage(vText) {

            if (mPopup == undefined)
                mPopup = MessageBox(vText, '', []);
            else
                mPopup.SetText(vText);

        } // END ShowMessage

        this.ShowMessage = ShowMessage;

        function HideMessage() {

            try {
                if (mPopup != null) {
                    mPopup.dialog('close');
                    mPopup = null;
                }
            } catch (ex) {
                // just ignore the error
            }

        } // END HideMessage
        this.HideMessage = HideMessage;

        return this;
    }; // END SalesPaymentsExport_offline

});