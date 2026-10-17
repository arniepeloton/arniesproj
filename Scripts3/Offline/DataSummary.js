define(['common',
    'business',
    '/Scripts3/Offline/Web.js?crc='
], function () {

    if (GetStringObject('Offline.DataSummary'))
        return;
    CreateStringObject('Offline.DataSummary');
    Offline.DataSummary = {
        Export: function (vData) {

            mData = vData;

            var mRecordCount = 0;
            var pCsvWriter;
            var mZipFilename = 'SummaryData.zip';

            // proposed directory struture
            // import summaries
            //      org id
            //          fowst id
            //              device id
            //                  datetime start
            //                  datetime end
            //                  invoice.txt
            mData = vData;
            ExportInvoiceSummary();

            if (mRecordCount > 0) {
                pCsvWriter = new CsvWriter(mZipFilename, 'dummy parameter');
                pCsvWriter.FinalCommit();
                Android.AddLog('Uploading ' + mRecordCount + ' rows of summary data.');
                pCsvWriter.Upload({
                    Destination: MergeUrlParameters({

                    }, '/OfflineData/DumpSummary'),
                    Data: {
                        OrgID: Web.OrgID(),
                        DeviceID: Web.DeviceID()
                    },
                    Success: function (vData) {
                        var pResults;

                        vData = $.extend({}, vData);
                        pResults = vData.Results;
                        try {
                            pResults = JSON.parse(pResults);
                        } catch (e) {
                            if ($.isFunction(mData.OnDone))
                                mData.OnDone({
                                    Result: 'error',
                                    Error: 'The Results value must be a valid JSON js28BMW8WC9L.'
                                });
                            return;
                        }
                        if (pResults.Error != undefined) {
                            if ($.isFunction(mData.OnDone))
                                mData.OnDone({
                                    Result: 'error',
                                    Error: pResults.Error
                                });
                            return;
                        }

                        Android.AddLog('Summary data were successfully uploaded. Processing summary data on the server.');
                        AjaxSend({
                            url: '/OfflineData/ProcessSummary',
                            data: {
                                TimeStamp: JSON.parse(vData.Results).TimeStamp,
                                DeviceID: Web.DeviceID()
                            },
                            success: function (vData) {
                                if (vData.Result == 'ok') {
                                    Android.AddLog('Summary data were successfully processed.');
                                    if ($.isFunction(mData.OnDone))
                                        mData.OnDone({
                                            Result: 'ok'
                                        });
                                } else if (vData.NotTally == '1') {
                                    if (vData.ErrorDate == undefined) {
                                        console.trace('ProcessSummary.Success: NotTally is true but ErrorDate is null.');
                                    } else if (!IsDate(vData.ErrorDate)) {
                                        console.trace('ProcessSummary.Success: NotTally is true but ErrorDate is not a valid date.');
                                    } else {
                                        console.log(vData.Error);
                                        Offline.DataSummary.ReexportInvoice({
                                            LastUpdatedDate: vData.ErrorDate
                                        });
                                    }
                                } else if (vData.Error) {
                                    Android.AddLog('jsSZ7M94U5DZ. An error occured while sending summary import command: ' + vData.Error);
                                    if ($.isFunction(mData.OnDone))
                                        mData.OnDone({
                                            Result: 'error',
                                            Error: vData.Error,
                                            ErrorLocation: 'AjaxSend:Error:ImportSummaryData'
                                        });
                                };
                            },
                            error: function (vData) {
                                Android.AddLog('A network/server error occured while processing summary data.');
                                if ($.isFunction(mData.OnDone))
                                    mData.OnDone({
                                        Result: 'error',
                                        Error: vError,
                                        ErrorLocation: 'AjaxSend:Error:ImportSummaryData'
                                    });
                            }
                        });
                    },
                    Error: function (vData) {
                        Android.AddLog('Summary data upload error: ' + vData.Error);
                        if ($.isFunction(mData.OnDone))
                            mData.OnDone({
                                Error: vData.Error,
                                Result: 'error',
                                ErrorLocation: 'Upload'
                            });
                    }
                });
            } else {
                Android.AddLog('No summary record found. Done.');
                if ($.isFunction(mData.OnDone))
                    mData.OnDone({
                        Result: 'ok'
                    });
                return;
            }

            function ExportInvoiceSummary() {

                var pDT;
                var pSql;
                var pCsvWriter;

                Android.AddLog('Creating invoice summary');
                pCsvWriter = new CsvWriter(mZipFilename, 'Invoice');
                Transfers.WriteFile('SummaryData.zip', 'info.json', {
                    OrgID: Android.GetSessionData().OrgID,
                    UUID: Device.Serial(),
                    AppVersion: Android.AppVersion(),
                    AppBuildDate: Android.AppBuildDate(),
                    AppInstallDate: Android.AppInstallDate()
                });

                pSql = ' SELECT ' + '\n' +
                    ' CAST(SUM(private.TransInvoiceItems.Quantity * private.TransInvoiceItems.UnitPrice) AS TEXT) AS ItemsSellingAmount' + '\n' +
                    ' ,CAST(SUM(private.TransInvoiceItems.SubTotal) AS TEXT) AS SellingAmount' + '\n' +
                    ' ,private.TransInvoice.FowstID' + '\n' +
                    ' ,private.TransInvoice.DeviceID' + '\n' +
                    ' ,date(private.TransInvoice.LastUpdated) AS LastUpdatedDate' + '\n' +
                    ' FROM private.TransInvoiceItems' + '\n' +
                    ' JOIN private.TransInvoice ON private.TransInvoice.ID = private.TransInvoiceItems.InvoiceID' + '\n' +
                    ' WHERE private.TransInvoice.DateImported IS NOT NULL' + '\n' +
                    ' AND private.TransInvoice.IsCancelled = 0' + '\n' +
                    ' AND private.TransInvoice.InvoiceNumber <> 0' + '\n' +
                    ' GROUP BY private.TransInvoice.FowstID' + '\n' +
                    ' ,private.TransInvoice.DeviceID' + '\n' +
                    ' ,date(private.TransInvoice.LastUpdated)' + '\n' +
                    ' ORDER BY date(private.TransInvoice.LastUpdated)';
                pDT = Sql.Execute(pSql);
                mRecordCount += pDT.Rows.length;
                if (pDT.Rows.length > 0)
                    pCsvWriter.WriteTable(pDT); // no need to commit

            } // END ExportInvoiceSummary

        }, // END ExportSummary
        ReexportInvoice: function (vParams) {
            // PARAMS:
            //  LastUpdatedDate: no time. invoice last updated

            var pSql;
            var pWhere;

            Android.AddLog('Untagging TransInvoice.');
            pWhere = ' WHERE date(private.TransInvoice.LastUpdated)  = ' + ReadyString($.format.date(ParseDate(vParams.LastUpdatedDate), 'yyyy-MM-dd')) +
                ' AND TransInvoice.InvoiceNumber > 0';
            pSql = 'UPDATE TransInvoice' +
                ' SET DateImported = NULL' +
                pWhere;
            pSql = Sql.ExecuteSql(pSql);

            Android.AddLog('Reexporting...');
            window.SalesPaymentsExport_offline().Export(mData);
        } // END ReexportInvoice
    };

});  // define