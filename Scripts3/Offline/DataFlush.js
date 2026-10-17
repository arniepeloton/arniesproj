define([], function() {

    var mData;

    function ExportStart() {

        var pDT;
        var pCsvWriter;
        var pRecordCount = 0;
        var pSqlWhere = ' WHERE private.TransInvoice.DateImported IS NULL' +
        ' AND (' +
        '   (private.TransInvoice.InvoiceNumber > 0)' +
        '   OR (IFNULL(private.TransInvoice.SalesOrderNo, 0) > 0)' +
        ')';
        var pLimit = 25; // used to be 50; 6/26/2014 10:29 AM
        var pInvoiceIds;
        var pIds;
        var pCustomerPaymentIds;
        var pConvertedCount;

        try {
            Android.AddLog('Querying local database for exportable records');

            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransInvoice');
            pCsvWriter.Delete();
            Transfers.WriteFile('InvoiceUpload.zip', 'info.json', {
                OrgID: Web.OrgID(),
                UUID: Device.Serial(),
                AppVersion: Android.AppVersion(),
                AppBuildDate: Android.AppBuildDate(),
                AppInstallDate: Android.AppInstallDate()
            });

            function ConvertDates(vDT, vFields) {

                var pFields;
                var pValue;

                if (vDT == undefined)
                    return;

                if ($.isArray(vDT.Rows) == false)
                    return;

                if ($.isArray(vFields))
                    pFields = vFields;
                else
                    pFields = [vFields];

                for (var p = 0; p <= vDT.Rows.length - 1; p++) {
                    for (var pFieldIndex = 0; pFieldIndex <= pFields.length - 1; pFieldIndex++) {
                        pValue = pFields[pFieldIndex];
                        pValue = vDT.Rows[p][pValue];
                        if (IsDate(pValue))
                            vDT.Rows[p][pFields[pFieldIndex]] = ParseDate(pValue);
                    }
                }

            } // END ConvertDates

            Android.AddLog('Getting TransInvoice from local database.');
            pDT = Sql.Execute('SELECT * FROM private.TransInvoice' + pSqlWhere +
            ' ORDER BY TransInvoice.InvoiceNumber, TransInvoice.LastUpdated' +
            ' LIMIT ' + pLimit);
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            pInvoiceIds = ExtractValues(pDT, 'ID', true);
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting TransInvoiceItems from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransInvoiceItems');
            pDT = Sql.Execute('SELECT TransInvoiceItems.* ' +
            ' FROM private.TransInvoiceItems' +
            ' WHERE TransInvoiceItems.InvoiceID IN (' + pInvoiceIds + ')' +
            ' ORDER BY TransInvoiceItems.LineNumber');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting TransInvoiceDiscount from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransInvoiceDiscount');
            pDT = Sql.Execute('SELECT TransInvoiceDiscount.* ' +
            ' FROM private.TransInvoiceDiscount' +
            ' WHERE TransInvoiceDiscount.InvoiceID IN (' + pInvoiceIds + ')');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting TransInvoiceSeniorCitizenPWDCustomer from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransInvoiceSeniorCitizenPWDCustomer');
            pDT = Sql.Execute('SELECT TransInvoiceSeniorCitizenPWDCustomer.* ' +
            ' FROM private.TransInvoiceSeniorCitizenPWDCustomer' +
            ' WHERE TransInvoiceSeniorCitizenPWDCustomer.InvoiceID IN (' + pInvoiceIds + ')');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            //additional by janine jan 19 2015 10am
            Android.AddLog('Getting TransInvoiceCharges from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransInvoiceCharges');
            pDT = Sql.Execute('SELECT TransInvoiceCharges.* ' +
            ' FROM private.TransInvoiceCharges' +
            ' WHERE TransInvoiceCharges.InvoiceID IN (' + pInvoiceIds + ')');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);
            //end of additional

            Android.AddLog('Getting TransInvoiceItemSerials from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransInvoiceItemSerials');
            pDT = Sql.Execute('SELECT TransInvoiceItemSerials.* ' +
            ' FROM private.TransInvoiceItemSerials' +
            ' WHERE TransInvoiceItemSerials.InvoiceID IN (' + pInvoiceIds + ')');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting TransInvoiceItemSalesReps from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransInvoiceItemSalesReps');
            pDT = Sql.Execute(' SELECT TransInvoiceItemSalesReps.*' + '\n' +
            ' FROM private.TransInvoiceItemSalesReps' + '\n' +
            ' JOIN private.TransInvoiceItems ON private.TransInvoiceItemSalesReps.InvoiceID = private.TransInvoiceItems.InvoiceID' + '\n' +
            ' WHERE TransInvoiceItems.InvoiceID IN (' + pInvoiceIds + ')');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting TransCustomerPayment from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransCustomerPayment');
            pDT = Sql.Execute('SELECT * FROM private.TransCustomerPayment' +
            ' WHERE ID IN ' +
            '   (SELECT TransCustomerPaymentInvoices.CustomerPaymentID' +
            '   FROM TransCustomerPaymentInvoices' +
            '   WHERE TransCustomerPaymentInvoices.InvoiceID IN (' + pInvoiceIds + ')' +
            '   )' +
            ' ORDER BY TransCustomerPayment.LastUpdated');
            pConvertedCount = ConvertDates(pDT, 'PaymentDate');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            pCustomerPaymentIds = ExtractValues(pDT, 'ID', true);
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting TransCustomerPaymentInvoices from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransCustomerPaymentInvoices');
            // TransCustomerPayment is not really needed
            pDT = Sql.Execute('SELECT TransCustomerPaymentInvoices.* ' +
            ' FROM private.TransCustomerPaymentInvoices' +
            ' WHERE TransCustomerPaymentInvoices.CustomerPaymentID IN (' + pCustomerPaymentIds + ')');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting TransCustomerPaymentDebitCard from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransCustomerPaymentDebitCard');
            pDT = Sql.Execute(' SELECT TransCustomerPaymentDebitCard.*' + '\n' +
            ' FROM TransCustomerPaymentDebitCard' + '\n' +
            ' WHERE TransCustomerPaymentDebitCard.CustomerPaymentID IN (' + pCustomerPaymentIds + ')');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting TransCustomerPaymentCreditCard from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransCustomerPaymentCreditCard');
            pDT = Sql.Execute(' SELECT TransCustomerPaymentCreditCard.*' + '\n' +
            ' FROM TransCustomerPaymentCreditCard' + '\n' +
            ' WHERE TransCustomerPaymentCreditCard.CustomerPaymentID IN (' + pCustomerPaymentIds + ')');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting TransCustomerPaymentChecks from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransCustomerPaymentChecks');
            pDT = Sql.Execute(' SELECT TransCustomerPaymentChecks.*' + '\n' +
            ' FROM TransCustomerPaymentChecks' + '\n' +
            ' WHERE TransCustomerPaymentChecks.CustomerPaymentID IN (' + pCustomerPaymentIds + ')');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting TransCustomerPaymentOther from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransCustomerPaymentOther');
            pDT = Sql.Execute(' SELECT TransCustomerPaymentOther.*' + '\n' +
            ' FROM TransCustomerPaymentOther' + '\n' +
            ' WHERE TransCustomerPaymentOther.CustomerPaymentID IN (' + pCustomerPaymentIds + ')');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            //additional by janine. for customer added in device
            Android.AddLog('Getting LibraryOrganization from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'LibraryOrganization');
            pDT = Sql.Execute(' SELECT * FROM LibraryOrganization WHERE IsFromDevice = 1 AND FlushToServer = 1');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting LibraryOrganizationCustomer from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'LibraryOrganizationCustomer');
            pDT = Sql.Execute(' SELECT LibraryOrganizationCustomer.*' + '\n' +
            ' FROM private.LibraryOrganizationCustomer' + '\n' +
            ' JOIN LibraryOrganization ON LibraryOrganization.ID = private.LibraryOrganizationCustomer.OrganizationID' + '\n' +
            ' WHERE LibraryOrganization.IsFromDevice = 1 AND LibraryOrganization.FlushToServer = 1');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting LibraryOrganizationPriceLevel from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'LibraryOrganizationPriceLevel');
            pDT = Sql.Execute(' SELECT LibraryOrganizationPriceLevel.*' + '\n' +
            ' FROM private.LibraryOrganizationPriceLevel' + '\n' +
            ' JOIN LibraryOrganization ON LibraryOrganization.ID = private.LibraryOrganizationPriceLevel.OrganizationID' + '\n' +
            ' WHERE LibraryOrganization.IsFromDevice = 1 AND LibraryOrganization.FlushToServer = 1');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting LibraryOrganizationFOWST from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'LibraryOrganizationFOWST');
            pDT = Sql.Execute(' SELECT * FROM LibraryOrganizationFOWST WHERE IsFromDevice = 1 AND FlushToServer = 1');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting LibraryFOWSTBinProduct from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'LibraryFOWSTBinProduct');
            pDT = Sql.Execute(' SELECT * FROM LibraryFOWSTBinProduct WHERE private.LibraryFOWSTBinProduct.DateImported IS NULL and MaxPerProduct>0');
            pRecordCount += pDT.Rows.length;
            var pInventoryCountIds = ExtractValues(pDT, 'ID', true);
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting TransInventoryProductCount from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransInventoryProductCount');
            pDT = Sql.Execute(' SELECT * FROM TransInventoryProductCount WHERE private.TransInventoryProductCount.DateImported IS NULL');
            pRecordCount += pDT.Rows.length;
            var pInventoryCountIds = ExtractValues(pDT, 'ID', true);
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting TransInventoryProductCountStock from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransInventoryProductCountStock');
            pDT = Sql.Execute(' SELECT * FROM TransInventoryProductCountStock ' +
                            'WHERE private.TransInventoryProductCountStock.InventoryProductCountID IN (' + pInventoryCountIds +
                            ')');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting LibraryFOWSTBin from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'LibraryFOWSTBin');
            pDT = Sql.Execute(' SELECT * FROM private.LibraryFOWSTBin WHERE DateImported IS NULL AND Name<> \'Default Bin\'');
            pRecordCount += pDT.Rows.length;
            var pInventoryCountIds = ExtractValues(pDT, 'ID', true);
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);
            //end of additional

            Android.AddLog('Getting TransReceiving from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransReceiving');
            pDT = Sql.Execute('SELECT * FROM private.TransReceiving' +
            ' WHERE TransReceiving.DateImported IS NULL' +
            ' ORDER BY TransReceiving.LastUpdated' +
            ' LIMIT ' + pLimit);
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            pIds = ExtractValues(pDT, 'ID', true);
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting TransReceivingStock from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransReceivingStock');
            pDT = Sql.Execute('SELECT * FROM private.TransReceivingStock' +
            ' WHERE TransReceivingStock.ReceivingID IN (' + pIds + ')');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            pInvoiceIds = ExtractValues(pDT, 'ID', true);
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting LibraryDevicePrinterFowst from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'LibraryDevicePrinterFowst');
            pDT = Sql.Execute(' SELECT * FROM LibraryDevicePrinterFowst WHERE FlushToServer = 1');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            Android.AddLog('Getting LibraryDeviceFowstPrinter from local database.');
            pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'LibraryDeviceFowstPrinter');
            pDT = Sql.Execute(' SELECT * FROM LibraryDeviceFowstPrinter WHERE FlushToServer = 1');
            pRecordCount += pDT.Rows.length;
            pCsvWriter.WriteTable(pDT); // no need to commit
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            //additional by janine sales order signatures
            Android.AddLog('Getting Sales Order Signatures from local database.');
            pDT = Sql.Execute('SELECT * FROM private.TransInvoice' + pSqlWhere +
            ' AND SalesOrderSignatory IS NOT NULL ORDER BY TransInvoice.InvoiceNumber, TransInvoice.LastUpdated' +
            ' LIMIT ' + pLimit);
            Android.AddLog('Number of records: ' + pDT.Rows.length);

            if (pDT.Rows.length > 0) {
                for (var i = 0; i < pDT.Rows.length; i++) {
                    //Transfers.Upload({
                    //    SourceFilename: pDT.Rows[i].ID + '.txt',
                    //    Destination: 'SalesMenu/Upload',
                    //    Data: { OrgID: sessionStorage.OrgID, Filename: pDT.Rows[i].ID }
                    //});
                    pCsvWriter.AddFile(pDT.Rows[i].ID + '.txt', 'Signatures/' + pDT.Rows[i].ID + '.txt');
                }
            }
                    
            //end of additional oct 9 2014


            if (pRecordCount > 0) {
                if ($.isFunction(mData.OnHasExportable))
                    mData.OnHasExportable({
                });
                pCsvWriter.FinalCommit();
            } else {

                Android.AddLog('No exportable records found.');
                Offline.DataSummary.Export(mData);
                return;
            };
        } catch (e) {
            console.trace(e.stack);
            Android.AddLog('An error occured. ' + e.message);
            mData.OnDone({
                Result: 'error',
                Message: e.message
            });
            return;
        }

        Android.AddLog('Uploading ' + pRecordCount + ' records');
        pCsvWriter.Upload({
            Destination: MergeUrlParameters({
            }, '/OfflineData/Dump'),
            Data: {
                OrgID: Web.OrgID(),
                DeviceID: Web.DeviceID()
            },
            Success: function (vData) {
                var pStr;
                var pResults;

                vData = $.extend({}, vData);
                pResults = vData.Results;
                try {
                    pResults = JSON.parse(pResults);
                } catch (e) {
                    if ($.isFunction(mData.OnDone))
                        mData.OnDone({
                            Result: 'error',
                            Error: 'The Results value must be a valid JSON js7LP56PGN49.'
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
                if (CStr2(pResults.TimeStamp).trim() == '') {
                    if ($.isFunction(mData.OnDone))
                        mData.OnDone({
                            Result: 'error',
                            Error: 'No TimeStamp was given by the server.'
                        });
                    return;
                }
                pStr = 'Records were successfully uploaded. Sending import command.';
                if ($.isFunction(mData.OnProgress))
                    mData.OnProgress({
                        Message: pStr
                    });
                Android.AddLog(pStr);
                FollowUp(mData);

            }, // END Success
            Error: function (vData) {
                Android.AddLog('Record upload error: ' + vData.Error);
                if ($.isFunction(mData.OnDone))
                    mData.OnDone({
                        Result: 'error',
                        Error: vData.Error,
                        ErrorLocation: 'Upload'
                    });
            }
        }); // END pCsvWriter.Upload

    } // END ExportStart

    function DownloadImportResult(vParams) {

        //            vParams = {
        //                Filename: '',
        //                OnDone: function(vData) {
        //                    
        //                },
        //                OnProgress: function(vData) {
        //                    console.log(vData.Message);
        //                }
        //            };

        Transfers.Download({
            Source: 'OfflineData/DownloadImportResult?'
                + $.param({
                    Filename: vParams.Filename,
                    OrgID: Web.OrgID(),
                    DeviceID: Web.DeviceID()
                }),
            Destination: 'ImportResults/' + vParams.Filename,
            Success: function (vData) {
                Android.AddLog('Calling SetImportedDeviceDate...');
                AjaxSend({
                    url: '/OfflineData/SetImportedDeviceDate',
                    data: {
                        Filename: vParams.Filename,
                        OrgID: Web.OrgID(),
                        DeviceID: Web.DeviceID(),
                        DeviceDate: FormatDate(new Date())
                    },
                    success: function (vData) {
                        vData = $.extend({}, vData);
                        if (vData.Error != undefined)
                            Android.AddLog('Error in SetImportedDeviceDate. js6Q7QU6KBP2. ' + vData.Error);
                        else if (vData.Result == 'ok')
                            Android.AddLog('Success in SetImportedDeviceDate. jsRAS82FLCZG.');
                    },
                    error: function () {
                        Android.AddLog('Error in SetImportedDeviceDate.');
                    },
                    complete: function () {
                        if (Transfers.IsValidZipFile('ImportResults/' + vParams.Filename)) {
                            Android.AddLog('Import result was successfully downloaded. Tagging...');
                            UpdateImportTag({
                                Filename: 'ImportResults/' + vParams.Filename,
                                OnDone: vParams.OnDone,
                                OnProgress: vParams.OnProgress
                            });
                        } else {
                            MessageBox(Transfers.ReadFile('ImportResults/' + vParams.Filename));
                            if ($.isFunction(vParams.OnDone))
                                vParams.OnDone({
                                    Result: 'error',
                                    Error: 'Filename property was not found in the return value.'
                                });

                        }
                    }
                });
            },
            Error: function (vError) {
                MessageBox(vError);
                if ($.isFunction(vParams.OnDone))
                    vParams.OnDone({
                        Result: 'error',
                        Error: vError,
                        ErrorLocation: 'DownloadImportResult'
                    });
            }
        }); // END Transfers.Download

    }; // END DownloadImportResult

    function UpdateImportTag(vParams) {

        //            vParams = {
        //                Filename: '',
        //                OnDone: function(vData) {
        //                    
        //                },
        //                OnProgress: function(vData) {
        //                    console.log(vData.Message);
        //                }
        //            };

        Transfers.ExtractZipFile({
            Source: vParams.Filename,
            Success: function (vData) {
                var pCsvReader = new CsvReader(vData.Destination); // extracted/{guid}

                try {
                    TagDatabaseTables('TransInvoice');
                    TagDatabaseTables('TransCustomerPayment');
                    TagDatabaseTableOrg('LibraryOrganization'); //additional by janine
                    TagDatabaseTableOrg('LibraryOrganizationFOWST'); //additional by janine
                    TagDatabaseTableOrg('LibraryDevicePrinterFowst'); //additional by janine jan 30 2015
                    TagDatabaseTableOrg('LibraryDeviceFowstPrinter'); //additional by janine jan 30 201
                    TagDatabaseTables('TransInventoryProductCount'); //additional by francis september 04 2014
                    TagDatabaseTables('LibraryFOWSTBin'); //additional by francis september 04 2014
                    TagDatabaseTables('LibraryFOWSTBinProduct'); //additional by francis september 26 2014                        
                    TagDatabaseTables('TransReceiving');
                    Android.AddLog('Successfully tagged as imported.');
                    Offline.DataSummary.Export({
                        Filename: vParams.Filename,
                        OnDone: vParams.OnDone,
                        OnProgress: vParams.OnProgress
                    });
                } catch (e) {
                    Android.AddLog('ExtractZipFile:Success error: ' + e.message);
                    if ($.isFunction(vParams.OnDone))
                        vParams.OnDone({
                            Result: 'error',
                            Error: e.message
                        });
                };

                function TagDatabaseTables(vTableName) {

                    var pDR;
                    var pIU;
                    var pSql;

                    pCsvReader.Open(vTableName);
                    pIU = new InsertUpdate(vTableName);
                    do {
                        pDR = pCsvReader.ReadLine();
                        if (pDR == undefined) break;
                        pIU.Add('DateImported', proj.SqliteNow);
                        pSql = pIU.GetUpdate('ID = ' + ReadyString(pDR.ID));
                        Sql.ExecuteSql(pSql);
                    } while (true);
                }; // END TagDatabaseTables

                function TagDatabaseTableOrg(vTableName) {

                    var pDR;
                    var pIU;
                    var pSql;

                    pCsvReader.Open(vTableName);
                    pIU = new InsertUpdate(vTableName);
                    do {
                        pDR = pCsvReader.ReadLine();
                        if (pDR == undefined) break;
                        pIU.Add('FlushToServer', '0');
                        pSql = pIU.GetUpdate('ID = ' + ReadyString(pDR.ID));
                        Sql.ExecuteSql(pSql);
                    } while (true);
                }; // END TagDatabaseTableOrg


            },
            Error: function (vError) {
                Android.AddLog('UpdateImportTag Transfers.ExtractZipFile Error:: ' + vError);
                if ($.isFunction(vParams.OnDone))
                    vParams.OnDone({
                        Result: 'error',
                        Error: vError,
                        ErrorLocation: 'ExtractZipFile'
                    });
            }
        }); // END Transfers.ExtractZipFile

    }; // END UpdateImportTag

    function FollowUp(vParams) {

        //            vParams = {
        //                OnDone: function(vData) {
        //                    console.log(vData.Result); // vData.Result = 'ok' or 'error' or others
        //                    console.log(vData.Error); // vData.Error = null or string
        //                },
        //                OnProgress: function(vData) {
        //                    console.log(vData.Message); // vData.Message = string
        //                }
        //            };

        mData = vParams;
        vParams = $.extend({}, vParams);
        Android.AddLog('Following up data import.');
        AjaxSend({
            url: '/OfflineData/FollowUpImportData',
            data: {
                OrgID: Web.OrgID(),
                DeviceID: Web.DeviceID()
            },
            timeout: 15000,
            success: function (vData) {
                var pStr;

                vData = $.extend({}, vData);
                if (vData.Error != undefined)
                    Android.AddLog(vData.Error);
                if (vData.Message != undefined)
                    Android.AddLog(vData.Message);

                if (CBool(vData.MustFollowUp)) {
                    if (vData.Error != undefined)
                        Android.AddLog(vData.Error);

                    setTimeout(function () {
                        FollowUp(vParams);
                    }, 10000);
                    return;
                } // END if (CBool(vData.MustFollowUp)) {

                if (CBool(vData.MustUpload)) {
                    ExportStart();
                    return;
                }

                if (vData.Result == 'ok') {
                    Android.AddLog('Device data were successfully received by the server.');
                    if ($.isFunction(vParams.OnProgress))
                        vParams.OnProgress({
                            Message: 'Device data were successfully received by the server.'
                        });
                    if (vData.Filename)
                        DownloadImportResult({
                            Filename: vData.Filename,
                            OnDone: vParams.OnDone,
                            OnProgress: vParams.OnProgress
                        });
                    else {
                        if ($.isFunction(vParams.OnDone))
                            vParams.OnDone({
                                Result: 'error',
                                Error: 'Filename property was not found in the return value.'
                            });
                    }
                } else if (vData.Error) {
                    pStr = 'Following up data import command results in an error: ' + vData.Error;
                    Android.Toast(pStr);
                    if ($.isFunction(vParams.OnDone))
                        vParams.OnDone({
                            Result: 'error',
                            Error: pStr
                        });

                } else if (vData.Result == 'Must follow up') {
                    if (CStr2(vData.Message).trim() != '')
                        Android.AddLog(vData.Message);
                    setTimeout(
                        function () {
                            FollowUp(vParams);
                        },
                    10000);
                    if ($.isFunction(vParams.OnProgress))
                        vParams.OnProgress({
                            Message: 'Following up.'
                        });
                }
            }, // END success
            error: function (vError) {
                if (vError.statusText == 'timeout') {
                    setTimeout(
                        function () {
                            FollowUp(vParams);
                        },
                    10000);
                    if ($.isFunction(vParams.OnProgress))
                        vParams.OnProgress({
                            Message: 'Connection timeout while following up. Retrying...'
                        });
                } else {
                    localStorage.removeItem('DataImportID');
                    Android.AddLog('A network/server error occured while following up data import command.' + vError);
                    if ($.isFunction(vParams.OnDone))
                        vParams.OnDone({
                            Result: 'error',
                            Error: "Can't establish a connection to the server"
                        });
                }
            } // END error
        }); // END AjaxSend followup
    }; // END FollowUp

    function ExtractValues(vDT, vFieldName, vIsString) {

        var pOut = '';
        var pDistinct;

        if ((vDT == undefined)
            || (vDT.Rows == undefined)
            || (vDT.Rows.length == 0)) {

            if (CBool(vIsString))
                return ReadyString('');
            else
                return '';
        }

        pDistinct = GetDistinct(vDT.Rows, [vFieldName]);
        for (var p = 0; p <= pDistinct.length - 1; p++) {
            if (pOut != '')
                pOut += ', ';
            if (CBool(vIsString))
                pOut += ReadyString(pDistinct[p][vFieldName]);
            else
                pOut += CStr2(pDistinct[p][vFieldName]);
        } // END for

        return pOut;

    }; // END ExtractValues

    window.DataFlush = {
        FollowUp: FollowUp
    };
});