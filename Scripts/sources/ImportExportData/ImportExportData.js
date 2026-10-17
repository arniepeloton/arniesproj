define([
    'common',
    '/Scripts/sources/ImportExportData/ImportExportData_Offline.js?crc=',
    '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc=',
    '/Scripts3/SalesMenu_offline.js?crc=',
    'business'
    ], function () {

        var mDeferred = $.Deferred();

        var intCntr = 0;
        var IsRecordFound = false;
        var IsSuccess = false;
        var pData = {};
        var mInterval;
        var IsUpdate = 0;

        var flushedCounter = 0;
        var IsLazyUpdate = false;

        var mFOWSTID = '';
        var mDatabaseCreationStart;

        var testDatabaseOrgID = '7c794971-1cd2-4ad4-922f-36a93edde09d'; //additional by janine. this is the test db. you're not supposed to save actual data here. this should be deleted after the test

        $(function () {

            window.RequestLazyUpdate = function () {
                Bs.Loading('show', "Flushing Data to Server");

                var pDt;
                var pDt2;
                var pDt3;
                var pDt4;
                var pSql = '';
                var pSql2 = '';
                var pSql3 = '';
                var pSql4 = '';

                IsLazyUpdate = true;

                Android.UpdateSessionData({ OrgID: sessionStorage.OrgID })

                Sql.AttachDatabase(sessionStorage.OrgID + '.private.db');
                pSql = 'Select * from TransInvoice where DateImported IS NULL AND InvoiceNumber>0';
                pDt = Sql.Execute(pSql);

                //additional by janine. check if there are unflushed customer. april 21 2014
                pSql2 = 'Select * from LibraryOrganization where IsFromDevice = 1 AND FlushToServer = 1';
                pDt2 = Sql.Execute(pSql2);
                //end of additional

                //additional by janine. check if there are unflushed customer. aug 11 2014
                pSql3 = 'Select * from LibraryOrganizationFOWST where IsFromDevice = 1 AND FlushToServer = 1';
                pDt3 = Sql.Execute(pSql3);
                //end of additional

                pSql4 = 'SELECT * FROM private.TransReceiving WHERE DateImported IS NULL';
                pDt4 = Sql.Execute(pSql4);

                if (pDt.Rows.length > 0 || pDt2.Rows.length > 0 || pDt3.Rows.length > 0 || pDt4.Rows.length > 0) { //add to checking
                    flushedCounter++;
                    //flushed dire na side

                    window.SalesPaymentsExport_offline().Export({
                        OnDone: function (vData) {
                            if (vData.Result == 'ok') {
                                RequestDatabaseUpdate(sessionStorage.OrgID);
                            }
                        }
                    });
                }
                else {
                    //replace db

                    Bs.Loading('show', "Requesting Data from Server");


                    var pTables = ['MobizLibraryPeople',
                                    'MobizLibraryOrganization',
                                    'LibraryOrganizationEmployeeFOWST',
                                    'LibraryOrganizationEmployee',
                                    'LibraryOrganizationCustomer',
                                    'LibraryDiscountSettings',
                                    'LibrarySecurityModuleMain',
                                    'LibrarySecurityRole',
                                    'LibraryReceiptSettings',
                                    'LibraryDevice',
                                    'LibraryPimMenu',
                                    'SystemDefaults',
                                    'LibraryDevicePrinter',
                                    'LibrarySecurityRoleUser',
                                    'LibraryOrganizationPIM',
                                    'MobizLibraryPIMPriceLevel',
                                    'MobizLibraryPIMMeasure',
                                    'MobizLibraryPIMProductMeasurementBarcode',
                                    'MobizLibraryPIMMeasureSign',
                                    'MobizLibraryPIMProduct',
                                    'MobizLibraryPIMProductPriceLevel']

                    var pDTData = [];


                    Sql.AttachDatabase(sessionStorage.OrgID + '.private.db');
                    for (var i = 0; i < pTables.length; i++) {

                        pSql = 'Select * from private.TablesLastUpdated where TableName = ' + ReadyString(pTables[i]);
                        pDt = Sql.Execute(pSql);

                        if (pDt.Rows.length > 0) {
                            pDTData[i] = pDt.Rows[0].TableName + ',' + FormatDate(ParseDate(pDt.Rows[0].LastUpdated))
                        }
                    }

                    AjaxSend({
                        url: 'MobizExport/RequestDataFromServer',
                        data: { OrgID: sessionStorage.OrgID, PeopleID: Android.GetSessionData().PeopleUserID, DeviceUUID: Device.Serial(), Tables: pDTData, IsLazyUpdate: 1 },
                        error: function (err) {
                        },
                        success: function (vData) { //success 6
                            //Edited By: Francis H
                            //Date: 06/19/14

                            if (vData == "Creating Database") {
                                mInterval = setInterval(RequestDatabaseUpdate(vOrgID, 0), 5000);
                            }
                            else {
                                mInterval = setInterval(FollowUpRequestDataFromServer, 5000);
                            }
                        }
                    });
                }
            }

            window.SyncDataPrompt = function(vDatabaseCreationStart) {
                mDatabaseCreationStart = vDatabaseCreationStart;

                IsSyncDatabase();

                return true
                //no need na ang kani na options for now - 06092023 by Arnie
                PopupButtonList('', 'Options', [
                    {
                        Text: 'Sync Reference Records from Server',
                        Description: '(Get reference files from the database server)<br /><br />' +
                            'This sync option only updates the reference files that you have selected. This is a quick way to update your device with only the data that you need. When compared to a full data sync, this is generally lighter in terms of data usage and takes a shorter time to complete.' + '<br /><br />' + 
                            'Note that choosing this option does not sync / flush your transaction data to the server.' + '<br /><br />' + 
                            'Use the flush data sync option to send your transaction data to the server.'
                        ,
                        Click: function() {
                            NewMessageBox('This function is not yet implemented. ref: jsT6RGVUF4TU!');
                            return;
                            NewMessageBox('', 'What references would you like to retrieve from the database server?', [
                                {
                                    Text: 'A.) Product Information',
                                    Click: function() {
                                        IsSyncDatabase({
                                            ForCsv: true,
                                            IncludeProducts: true
                                        });
                                    }
                                },
                                {
                                    Text: 'B.) Hot Buttons, Item Modifiers and Promotions',
                                    Click: function() {
                                        IsSyncDatabase({
                                            ForCsv: true,
                                            IncludeHotButtons: true
                                        });
                                    }
                                },
                                {
                                    Text: 'Both A and B',
                                    Click: function() {
                                        IsSyncDatabase({
                                            ForCsv: true,
                                            IncludeProducts: true,
                                            IncludeHotButtons: true
                                        });
                                    }
                                }
                            ]);
                        }
                    },
                    {
                        Text: 'Flush Transaction Records to Server',
                        Description: '(Flush transaction data to the database server)<br /><br />' +
                            'This sync option only sends/ flushes transaction data from your device to the database server. When compared to a full data sync, this is generally lighter in terms of data usage and takes a shorter time to complete.' + '<br /><br />' + 
                            'This option does not update your device\'s reference files  such as new products, new prices, new promotions, new hot button menu layouts, etc.' + '<br /><br />' + 
                            'Use this option when there are no new reference record updates made by your sys admin -- and/or you only wish to send/ flush out transaction data from your device to the database server.' + '<br /><br />' + 
                            'Note that this option does not replace your device\'s transaction database with a new one. A transaction database that has a lot of records in it has a tendency to be slower.' + '<br /><br />' + 
                            'If your device has not had a full data sync in a long time -- and you are experiencing some slowdowns when searching for items -- please do a full sync to refresh your transaction database and help speed things up.',
                        Click: function() {
                            SalesPaymentsExport_offline().FlushDeviceData({
                                Success: function(vData) {
                                    NewMessageBox('All data were successfully sent to the server.', 'Success');
                                }   
                            }); // END FlushDeviceData
                        }
                    },
                    {
                        Text: 'Full Data Sync ',
                        Description: '(Get reference files and flush transaction data to the database server)<br /><br />' +
                            'This sync option takes the longest time among the sync options. Use full data sync when you have a good connection with the server or when your device is starting to fill up its storage capacity as this option reinstalls a new transaction database to your device for a clean slate.' + '<br /><br />' + 
                            'A new empty transaction database will be installed on this device only when all previous transaction records have been fully flushed and backed up to the database server.' + '<br /><br />' + 
                            'This process will a.) send this device\'s transaction records to the server and b.) get the latest reference files from the server.',
                        Click: function() {
                            IsSyncDatabase();
                        }
                    }
                ]);
            } // END window.SyncDataPrompt = function() {

            window.IsSyncDatabase = function (vParams) {

                if (!AllowToSync())
                    return;
                    
                vParams = $.extend({
                    ForCsv: null,
                    IncludeProducts: null, // only for ForCsv
                    IncludeHotButtons: null // only for ForCsv
                }, vParams);

                var p = MessageBox("Are you sure you want to get latest update from Mobiz?",
                    "Import",
                    [
                        {
                            Text: 'Yes',
                            Click: function () {
                                // commented to use the Java-based data sync // if (window.DebugMode) {
                                    mobile.DataSync({
                                        OrgID: sessionStorage.OrgID,
                                        PeopleID: Android.GetSessionData().PeopleUserID,
                                        IsDatabaseExists: 1,
                                        IsSelectedData: 0,
                                        ForCsv: CBool(vParams.ForCsv),
                                        IncludeProducts: CBool(vParams.IncludeProducts),
                                        IncludeHotButtons: CBool(vParams.IncludeHotButtons),
                                        DatabaseCreationStart: mDatabaseCreationStart,
                                        Success: function() {
                                            //alert('jsM24M8KPN6K!');
                                        },
                                        Error: function() {
                                            //alert('jsTPCW5S5QJY!');
                                        }
                                    });
                                    //history.back(); // close the messagebox

                                    var pActionLogs = {};
                                    pActionLogs.EmployeeName = sessionStorage.CurrentUserFullName;
                                    pActionLogs.FOWSTID = sessionStorage.FowstID;
                                    pActionLogs.DeviceID = Device.Serial();
                                    var pActionLogsText = JSON.stringify(pActionLogs, null, 4);
                                    var pActionLogsFilename = 'pending action logs/' + sessionStorage.OrgID.toUpperCase() + '.txt';
                                    Transfers.WriteFile(pActionLogsFilename, null, pActionLogsText);

                                    //var pIU;
                                    //pIU = new InsertUpdate('private.TransActionLogs');

                                    //pIU.Add('ID', ReadyString(NewID()));
                                    //pIU.Add('ActionDate', proj.SqliteNow);
                                    //pIU.Add('EmployeeName', ReadyString(sessionStorage.CurrentUserFullName));
                                    //pIU.Add('FOWSTID', ReadyString(sessionStorage.FowstID));
                                    //pIU.Add('DeviceID', ReadyString(Device.Serial()));
                                    //pIU.Add('Action', ReadyString("Synchronize Database"));

                                    //pSql = pIU.GetInsert();

                                    //Sql.ExecuteSql(pSql);

                            }
                        },
                        { Text: 'No' }
                    ]);
            }

            function AllowToSync() {
                var pUnflushedTableView = Business.TransInvoice.UnflushedTableView();
                var pMessage;
                
                if (pUnflushedTableView.Rows.length > 0) {
                    var pDR = pUnflushedTableView.Rows[0];
                    NewMessageBox('You have open tables in POS > Table View. One of these is table ' + pDR.TableName + ' in ' + pDR.DiningAreaName + ' at ' + pDR.FowstName  +  '. Mobiz Complete Retail cannot sync your database if there are open tables. Please close these tables first before data syncing.');
                    return false;
                }

                return true;
            } // END function AllowToSync() {

            window.RequestDatabaseUpdate = function (vOrgID) {

                ClearInterval();
                IsUpdate = 1;

                Bs.Loading('show', "Flushing Data to Server");

                var pDt;
                var pDt2;
                var pDt3;
                var pDt4;
                var pSql = '';
                var pSql2 = '';
                var pSql3 = '';
                var pSql4 = '';
                var pDTPresentTotal = '';

                Android.UpdateSessionData({ OrgID: sessionStorage.OrgID })

                Sql.AttachDatabase(sessionStorage.OrgID + '.private.db');

                pSql = 'Select * from TransInvoice where DateImported IS NULL AND InvoiceNumber>0';
                pDt = Sql.Execute(pSql);

                //additional by janine. check if there are unflushed customer. april 21 2014
                pSql2 = 'Select * from LibraryOrganization where IsFromDevice = 1 AND FlushToServer = 1';
                pDt2 = Sql.Execute(pSql2);
                //end of additional

                //additional by janine. check if there are unflushed customer. aug 11 2014
                pSql3 = 'Select * from LibraryOrganizationFOWST where IsFromDevice = 1 AND FlushToServer = 1';
                pDt3 = Sql.Execute(pSql3);
                //end of additional

                pSql4 = 'Select * from private.TransReceiving where DateImported IS NULL';
                pDt4 = Sql.Execute(pSql);

                if (pDt.Rows.length > 0 || pDt2.Rows.length > 0 || pDt3.Rows.length > 0 || pDt4.Rows.length > 0) { //add to checking
                    flushedCounter++;
                    //flushed dire na side

                    window.SalesPaymentsExport_offline().Export({
                        OnDone: function (vData) {
                            if (vData.Result == 'ok') {
                                RequestDatabaseUpdate(sessionStorage.OrgID);
                            }
                        }
                    });
                }
                else {
                    //replace db

                    Bs.Loading('show', "Requesting Data from Server");

                    pData.OrgID = vOrgID;
                    //sessionStorage["OrgID"] = vOrgID;

                    IsDatabaseExists = 1;

                    AjaxSend({
                        url: 'MobizExport/RequestDataFromServer',
                        data: { OrgID: sessionStorage.OrgID, PeopleID: Android.GetSessionData().PeopleUserID, DeviceUUID: Device.Serial(), IsSelectedData: 1 },
                        error: function (err) {
                        },
                        success: function (vData) { //success 6
                            //Edited By: Francis H
                            //Date: 06/19/14

                            if (vData == "Creating Database") {
                                mInterval = setInterval(RequestDatabaseUpdate(vOrgID, 0), 5000);
                            }
                            else {
                                mInterval = setInterval(FollowUpRequestDataFromServer, 5000);
                            }
                        }
                    });
                }

            }

            window.RequestImportData = function (vOrgID, DatabaseExists) {

                pData.OrgID = vOrgID;
                //sessionStorage["OrgID"] = vOrgID;
                IsDatabaseExists = DatabaseExists;

                // this is called by Download Database 2023_05_08__11_52_09_5760
                Bs.Loading('show', "Requesting Data from Server");
                Android.SetOrgID(sessionStorage.OrgID);

                mobile.DataSync({
                    OrgID: sessionStorage.OrgID,
                    PeopleID: Android.GetSessionData().PeopleUserID,
                    IsDatabaseExists: 0,
                    IsSelectedData: 0,
                    //ForCsv: CBool(vParams.ForCsv),
                    IncludeProducts: 1,
                    IncludeHotButtons: 1,
                    //DatabaseCreationStart: mDatabaseCreationStart,
                    Success: function() {
                        Bs.Loading('hide');
                        $('#pageDownloadDatabaseSuccessful').pageshow();
                    },
                    Error: function() {
                        ClearInterval();
                        Bs.Loading('hide');
                        $('#pageDownloadError').pageshow();
                        setTimeout(proj.DownloadDatabasePageShowError, 537);
                    }
                });
                //history.back(); // close the messagebox
                return;

                var pActionLogs = {};
                pActionLogs.EmployeeName = sessionStorage.CurrentUserFullName;
                pActionLogs.FOWSTID = sessionStorage.FowstID;
                pActionLogs.DeviceID = Device.Serial();
                var pActionLogsText = JSON.stringify(pActionLogs, null, 4);
                var pActionLogsFilename = 'pending action logs/' + sessionStorage.OrgID.toUpperCase() + '.txt';
                Transfers.WriteFile(pActionLogsFilename, null, pActionLogsText);

                AjaxSend({
                    url: 'MobizExport/RequestDataFromServer',
                    data: { 
                        OrgID: sessionStorage.OrgID, 
                        PeopleID: Android.GetSessionData().PeopleUserID, 
                        IsDatabaseExists: IsDatabaseExists, 
                        DeviceUUID: Device.Serial(), IsSelectedData: 0},
                    error: function (err) {
                    },
                    success: function (vData) { //success 6                        
                        //Edited By: Francis H
                        //Date: 06/19/14
                        if (vData.Error) {
                            Bs.Loading('hide');
                            if (vData.Error == 'No common device fowst person')
                                MessageBox('You and this device must be registered to a common FOWST in the selected organization.');
                            else
                                MessageBox(vData.Error);
                            return;
                        }
                        if (vData == "Creating Database") {
                            mInterval = setInterval(RequestImportData, 5000);
                        }
                        else {
                            mInterval = setInterval(FollowUpRequestDataFromServer, 5000);
                        }
                    }
                }); //ajaxsend

            };

            mDeferred.resolve();
        });

        function FollowUpRequestDataFromServer() {
            Bs.Loading('show', "Requesting Data from Server");
            AjaxSend({
                url: 'MobizExport/RequestFollowUp',
                success: function (vData) {
                    if (vData.Result != undefined) {                        
                        if (vData.Result.length == 75) {
                            ClearInterval();
                            if (IsLazyUpdate) {
                                StartComparing(vData.Result);
                            }
                            else {
                                StartCompiling(vData.Result);
                            }

                        }
                        else if (vData.Result == 'Error') {
                            ClearInterval();
                            Bs.Loading('hide');
                            $('#pageDownloadError').pageshow();
                            setTimeout(proj.DownloadDatabasePageShowError, 736);
                        }
                        else if (vData.Result.indexOf('Error') >= 0) {
                            ClearInterval();
                            Bs.Loading('hide');
                            MessageBox(vData.Result);
                        }
                        else if (vData.Result == 'nothing') {
                            ClearInterval();
                            Bs.Loading('hide');
                            MessageBox("Database is up to date.", "Import");
                            return;
                        }
                        else {
                            Bs.Loading('show', "Requesting Data from Server");
                        }
                    }
                }
            });
        }

        function ClearInterval() {
            clearInterval(mInterval);
        }

        function StartCompiling(vKey) {

            Android.Toast('Downloading Database');
            Android.StopFlushing();
            Transfers.Download({
                Key: vKey,
                Destination: vKey + '.zip',
                Success: function (vData) {

                    var pData = {};

                    var mDate = new Date();

                    if (mFOWSTID != undefined && mFOWSTID != '') {
                        pData.FowstID = CStr2(mFOWSTID);
                        pData.RangeType = $.trim('Day');
                        pData.RangeFrom = mDate;
                        pData.RangeTo = mDate;
                        pData.RangeYear = mDate.getFullYear;

                        AjaxSend({
                            url: "ReportsPOS/getCashierSalesTransactionSummary",
                            data: pData,
                            success: function (vDataInfo) {

                                vDataInfo = vDataInfo.ReportPOS;

                                var dblTotalSales = 0;
                                var dblTotalSalesDiscount = 0;
                                var dblAdjRefund = 0;
                                var dblAdjSRV = 0;
                                var dblNetSales = 0;

                                var dblCash = 0;
                                var dblCheck = 0;
                                var dblDebitCard = 0;
                                var dblCreditCard = 0;
                                var dblSRV = 0;
                                var dblTotalPayment = 0;

                                var dblVAT = 0;
                                var dblVATAmount = 0;
                                var dblNonVAT = 0;
                                var dblVATExempt = 0;
                                var dblZeroRatedVAT = 0;
                                var dblTotalVAT = 0;

                                for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                                    dblTotalSales = dblTotalSales + parseFloat(vDataInfo.Rows[p].CurGrossSales || 0);
                                    dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.Rows[p].CurSalesDiscount || 0);
                                    dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                                    dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchange || 0);

                                    dblCash = dblCash + parseFloat(vDataInfo.Rows[p].CashAmount || 0);
                                    dblCheck = dblCheck + parseFloat(vDataInfo.Rows[p].CheckPaymentAmount || 0);
                                    dblDebitCard = dblDebitCard + parseFloat(vDataInfo.Rows[p].DebitCardAmount || 0);
                                    dblCreditCard = dblCreditCard + parseFloat(vDataInfo.Rows[p].CreditCardAmount || 0);
                                    dblSRV = dblSRV + parseFloat(vDataInfo.Rows[p].SRVAmount || 0);

                                    dblVAT = dblVAT + parseFloat(vDataInfo.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                    dblVATAmount = dblVATAmount + parseFloat(vDataInfo.Rows[p].CurTotalVATAmount || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                                    dblNonVAT = dblNonVAT + parseFloat(vDataInfo.Rows[p].CurNoTAXSales || 0);
                                    dblVATExempt = dblVATExempt + parseFloat(vDataInfo.Rows[p].CurVATExemptSales || 0);
                                    dblZeroRatedVAT = dblZeroRatedVAT + parseFloat(vDataInfo.Rows[p].CurZeroRatedVATSales || 0);
                                };

                                var tmpFile = Transfers.ReadFile(sessionStorage.OrgID + ($.format.number(mDate.getFullYear(),'00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');
                                var LastInvoiceDate;

                                if (tmpFile != undefined) {

                                    pSql = "Select InvoiceDate from TransInvoice Where TransInvoice.InvoiceNumber > 0 AND date(InvoiceDate) = " + ReadyString($.format.number(mDate.getFullYear(),'00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + " Order By InvoiceDate Desc LIMIT 1"
                                    LastInvoiceDate = Sql.ExecuteScalar(pSql);

                                    var LastTotal = tmpFile.split('|');

                                    if (LastInvoiceDate != undefined) {
                                        Transfers.WriteFile(sessionStorage.OrgID + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat', null,
                                            dblTotalSales + '|' + dblTotalSalesDiscount + '|' + dblAdjRefund + '|' + dblAdjSRV + '|' +
                                                dblCash + '|' + dblCheck + '|' + dblDebitCard + '|' + dblCreditCard + '|' + dblSRV + '|' + dblVAT + '|' +
                                                dblVATAmount + '|' + dblNonVAT + '|' + dblVATExempt + '|' + dblZeroRatedVAT + '|' + LastTotal[14] + '|' + LastInvoiceDate);

                                    }
                                } else {

                                    pSql = "Select InvoiceDate from TransInvoice WHERE TransInvoice.InvoiceNumber > 0 AND date(InvoiceDate) = " + ReadyString($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + " Order By InvoiceDate Asc LIMIT 1"
                                    var FirstInvoiceDate = Sql.ExecuteScalar(pSql);

                                    pSql = "Select InvoiceDate from TransInvoice Where TransInvoice.InvoiceNumber > 0 Order By InvoiceDate Desc LIMIT 1"
                                    LastInvoiceDate = Sql.ExecuteScalar(pSql);

                                    if (FirstInvoiceDate != undefined) {
                                        Transfers.WriteFile(sessionStorage.OrgID + ($.format.number(mDate.getFullYear(),'00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat', null,
                                            dblTotalSales + '|' + dblTotalSalesDiscount + '|' + dblAdjRefund + '|' + dblAdjSRV + '|' +
                                                dblCash + '|' + dblCheck + '|' + dblDebitCard + '|' + dblCreditCard + '|' + dblSRV + '|' + dblVAT + '|' +
                                                dblVATAmount + '|' + dblNonVAT + '|' + dblVATExempt + '|' + dblZeroRatedVAT + '|' + FirstInvoiceDate + '|' + LastInvoiceDate);
                                    }
                                }
                                //delete 15 days old file
                                deleteDate = new Date();

                                deleteDate.setDate(deleteDate.getDate() - 15);

                                Transfers.DeleteFile(sessionStorage.OrgID + (deleteDate.getDate() < 10 ? '0' + deleteDate.getDate().toString() : deleteDate.getDate().toString()) + (deleteDate.getMonth() + 1).toString() + deleteDate.getFullYear().toString() + '.dat')

                                BackupAndExtract();
                            }
                        });
                    } else {
                        BackupAndExtract();
                    }

                    function BackupAndExtract() {
                        try {
                            Sql.CloseConnection(); //lets try closing the connection and see kung macorrupt ba ang db.    
                        } catch (e) {

                        }


                        if (IsUpdate == 1) {
                            var flgCounter = 1;
                            var IsFlagExist = '';

                            Transfers.DeleteFile('../data/Mobiz.backup');

                            while (flgCounter < 17) {
                                IsFlagExist = Transfers.ReadFile(sessionStorage.OrgID + '-' + flgCounter + '.flg');
                                if (IsFlagExist == undefined) {
                                    try {
                                        Transfers.MoveFile('/data/Mobiz.db', '/data/Mobiz.backup');
                                        Transfers.MoveFile('/data/' + sessionStorage.OrgID + '.db', '/data/' + sessionStorage.OrgID + '-' + flgCounter + '.backup.db');
                                        Transfers.WriteFile(sessionStorage.OrgID + '-' + flgCounter + '.flg', null, '')

                                        //delete flag file and database backup of the next count

                                        if (flgCounter == 16) {
                                            Transfers.DeleteFile(sessionStorage.OrgID + '-1.flg');
                                            Sql.DeleteIfExist(sessionStorage.OrgID + '-1.backup');
                                        } else {
                                            Transfers.DeleteFile(sessionStorage.OrgID + '-' + (flgCounter + 1) + '.flg');
                                            Sql.DeleteIfExist(sessionStorage.OrgID + '-' + (flgCounter + 1) + '.backup');
                                        }

                                    } catch (e) {

                                    }
                                    break;
                                }
                                flgCounter++;
                            }

                        }

                        ExtractZipFile(vData);

                        function ValidateDonwloadedZipOrg(vFilename) {
                            var pStr;
                            var pDownloadingOrgID = Transfers.ReadFile('DownloadingOrgID');
                            pDownloadingOrgID = CStr2(pDownloadingOrgID).trim();
                            if (pDownloadingOrgID == '') {
                                pStr = 'DownloadingOrgID is blank. Please restart app then download database again. ref: jsJ6PG6NB6AG';
                                Android.AddLog(pStr);
                                MessageBox(pStr);
                                return false;
                            }
                            pDownloadingOrgID = ReadyGuid(pDownloadingOrgID);

                            var pResult = Transfers.GetInternalFiles(vFilename);
                            if (pResult == null) {
                                pStr = 'Device file, ' + vFilename + ', does not exist. Please restart app then download database again. ref: jsP3VS6YM8QV';
                                Android.AddLog(pStr);
                                MessageBox(pStr);
                                return false;
                            }

                            var pFiles = pResult.Files;
                            for (var pIndex = 0; pIndex <= pFiles.length - 1; pIndex++) {
                                var pFile = pFiles[pIndex];
                                if (!EndsWith(pFile.FullFileName.toLowerCase(), '.txt'))
                                    continue;
                                var pFilenameOnly = pFile.FullFileName.substr(0, 36);
                                pFilenameOnly = ReadyGuid(pFilenameOnly);
                                if (pFilenameOnly != pDownloadingOrgID) {
                                    pStr = 'DownloadingOrgID (' + pDownloadingOrgID + ') does not match the org ID (' + pFilenameOnly + ') in the downloaded file. Please restart app then download database again. ref: jsJR7F34XKP9';
                                    Android.AddLog(pStr);
                                    MessageBox(pStr);
                                    return false;
                                }
                            }

                            return true;
                        } // ValidateDonwloadedZipOrg

                        function ExtractZipFile(vData) {

                            if (!ValidateDonwloadedZipOrg(vData.Destination))
                                return;

                            Android.Toast('Extracting Files');

                            try {
                                Sql.CloseConnection();
                            } catch (e) {

                            }

                            Transfers.ExtractZipFile({
                                Source: vData.Destination,
                                Destination: 'data',
                                ExtractToFiles: true,
                                Success: function (vDataExtractFiles) {


                                    Sql.DownloadAndApplySchema({
                                        ForceUpdate: true,
                                        Success: function (vDataSchema) {

                                            Sql.CopyToTestDb();
                                            Sql.OpenDatabase(sessionStorage.OrgID + '.private.db');

                                            try {
                                                var pSql = 'select * from librarydevice limit 1';
                                                var pDT = Sql.Execute(pSql);

                                                if (pDT.Rows.length == 0) {
                                                    ExtractZipFile(vData);
                                                } else {
                                                    Sql.OpenDatabase(sessionStorage.OrgID + '.public.db');
                                                    var pSql2 = 'select * from libraryorganization limit 1';
                                                    var pDT2 = Sql.Execute(pSql2);

                                                    if (pDT2.Rows.length == 0) {
                                                        ExtractZipFile(vData);
                                                    } else {

                                                        Transfers.WriteFile(sessionStorage.OrgID + '.flg', null, '');
                                                        //delete downloaded database
                                                        Transfers.DeleteFile(vData.Destination);

                                                        // if database does not exist yet (initialize not sync db)
                                                        //additional by janine
                                                        if (IsUpdate == 0) {
                                                            Bs.Loading('hide');
                                                            Android.OfflineBalances();
                                                             $('#pageDownloadDatabaseSuccessful').pageshow(); // bypass data integrity test
                                                            // location = '/selftest'; // old code
                                                        } else {
                                                            try {
                                                                //additional by janine

                                                                AjaxSend({
                                                                    url: 'SelfTest/CheckIfTestPassed',
                                                                    data: { ActualOrgID: sessionStorage.OrgID, DeviceID: sessionStorage.DeviceID },
                                                                    success: function (vData) {
                                                                        // if database does not exist yet (initialize not sync db)
                                                                        //if failed, test again
                                                                        if (vData.Result == 'No') {
                                                                            //open private db
                                                                            Sql.OpenDatabase(sessionStorage.OrgID + '.public.db');
                                                                            Sql.AttachDatabase(testDatabaseOrgID + '.private.db');

                                                                            Android.UpdateSessionData({
                                                                                PeopleUserID: sessionStorage.PeopleID
                                                                            });


                                                                            location = '/selftest';
                                                                        } else {
                                                                            try {

                                                                                Sql.DeleteIfExist(testDatabaseOrgID);

                                                                            } catch (ex) {

                                                                            }
                                                                            //francis 08/11/2014

                                                                            Sql.OpenDatabase(sessionStorage.OrgID + '.private.db');
                                                                            //for (i = 0; i < pDT.Rows.length; i++) {
                                                                            var m_FowstID = '';

                                                                            //                                                                        var pSql = 'select FowstID, POSLastORNo from LibraryDeviceFowst';
                                                                            //                                                                        var pDT = Sql.Execute(pSql);

                                                                            m_FowstID = Transfers.ReadFile(sessionStorage.OrgID + 'LastFowst.flg');

                                                                            if (m_FowstID != undefined) {

                                                                                sessionStorage.FowstID = m_FowstID;

                                                                                pSql = 'SELECT MAX(POSLastORNo)' +
                                                                                    ' FROM LibraryDeviceFowst' +
                                                                                    ' WHERE FowstID = ' + ReadyString(m_FowstID) +
                                                                                    ' AND DeviceID = ' + ReadyString(Web.DeviceID());

                                                                                var pDT = Sql.Execute(pSql);

                                                                                AjaxSend({
                                                                                    url: '/SalesMenu/VerifyCounter',
                                                                                    timeout: proj.AjaxTimeout + 5000,
                                                                                    data: {
                                                                                        FowstID: m_FowstID,
                                                                                        DeviceID: Web.DeviceID(),
                                                                                        DeviceCounter: pDT.Rows[0].POSLastORNo
                                                                                    },
                                                                                    success: function (vData) {
                                                                                        if (IsNumeric(vData.Counter)) {
                                                                                            Business.TransInvoice.UpdateCounter(vData.Counter);
                                                                                        }
                                                                                    }
                                                                                });
                                                                            }
                                                                            //                                                                        }
                                                                            Bs.Loading('hide');
                                                                            $('#pageDownloadDatabaseSuccessful').pageshow();
                                                                        }
                                                                    },
                                                                    error: function (vError) {
                                                                        try {

                                                                            Sql.DeleteIfExist(testDatabaseOrgID);

                                                                        } catch (ex) {
                                                                        }
                                                                        Bs.Loading('hide');
                                                                        $('#pageDownloadDatabaseSuccessful').pageshow();
                                                                    }
                                                                });
                                                            } catch (ex) {
                                                                try {

                                                                    Sql.DeleteIfExist(testDatabaseOrgID);

                                                                } catch (ex) {
                                                                }
                                                                Bs.Loading('hide');
                                                                $('#pageDownloadDatabaseSuccessful').pageshow();
                                                            }
                                                        }
                                                        //end of additional
                                                    }
                                                }
                                            } catch (e) {
                                                ExtractZipFile(vData);
                                            }

                                        },
                                        Error: function (vDataErr) {
                                            Bs.Loading('hide');
                                            Android.Toast('Error while applying Schema');
                                        }
                                    });
                                },
                                Error: function (e) {
                                    Bs.Loading('hide');
                                    //rollback
                                    Android.AddLog('3 | Public and Private DB Error | Oops, the database we just downloaded seems funny. We’re restoring to the last known good back-up database.');
                                    //Android.Toast('Error while extracting database. Restoring back-up database.');
                                    var flgCounter = 1;

                                    Transfers.MoveFile('/data/Mobiz.backup', '/data/Mobiz.db');

                                    while (flgCounter < 17) {
                                        IsFlagExist = Transfers.ReadFile(sessionStorage.OrgID + '-' + flgCounter + '.flg');
                                        if (IsFlagExist == undefined) {
                                            try {
                                                if (flgCounter == 1) {
                                                    if (Sql.IsExist(sessionStorage.OrgID + '-16.backup')) {
                                                        Transfers.MoveFile('/data/' + sessionStorage.OrgID + '-16.backup.db', '/data/' + sessionStorage.OrgID + '.db');
                                                        //Transfers.DeleteFile(sessionStorage.OrgID + '-16.flg');
                                                        //MessageBox('There was an error while downloading database. Please try synching database later.', null, null, {
                                                        //    OnClose: function () {
                                                        //        window.location = '/SignIn?IsMobile=1';
                                                        //    } // OnClose
                                                        //}); // MessageBox
                                                        MessageBox('Oops, the database we just downloaded seems funny. We’re restoring to the last known good back-up database. ' +
                                                                    'After we restore your database, your Mobiz Complete Retail app should work normally.  However, some new data references such as new products, modified prices, or new employees may not be available on the restored database. ', null, null, {
                                                                       OnClose: function () {
                                                                           window.location = '/SignIn?IsMobile=1';
                                                                       } // OnClose
                                                                   }); // MessageBox
                                                    }
                                                } else {
                                                    if (Sql.IsExist(sessionStorage.OrgID + '-' + (flgCounter - 1) + '.backup')) {
                                                        Transfers.MoveFile('/data/' + sessionStorage.OrgID + '-' + (flgCounter - 1) + '.backup.db', '/data/' + sessionStorage.OrgID + '.db');
                                                        Transfers.DeleteFile(sessionStorage.OrgID + '-' + (flgCounter - 1) + '.flg');
                                                        //MessageBox('There was an error while downloading database. Please try synching database later.', null, null, {
                                                        //    OnClose: function () {
                                                        //        window.location = '/SignIn?IsMobile=1';
                                                        //    } // OnClose
                                                        //}); // MessageBox
                                                        MessageBox('Oops, the database we just downloaded seems funny. We’re restoring to the last known good back-up database. ' +
                                                                    'After we restore your database, your Mobiz Complete Retail app should work normally.  However, some new data references such as new products, modified prices, or new employees may not be available on the restored database. ', null, null, {
                                                                        OnClose: function () {
                                                                            window.location = '/SignIn?IsMobile=1';
                                                                        } // OnClose
                                                                    }); // MessageBox
                                                        return true;
                                                    }
                                                }

                                            } catch (e) {

                                            }

                                        }
                                        flgCounter++;
                                    }
                                }
                            });
                        }
                    }
                },
                Error: function (e) {
                    Bs.Loading('hide');
                    Android.AddLog('2 | Downloading DB Error |There was an error while downloading your database from the Mobiz Cloud.  This may be due to some network or connection error.');

                    MessageBox('There was an error while downloading your database from the Mobiz Cloud.  This may be due to some network or connection error. ' +
                                                                    'Please try syncing your database again later. If this syncing issue persist, please get in touch with Mobiz at customercare@mobiz-apps.com.', null, null, {
                                                                        OnClose: function () {
                                                                            window.location = '/SignIn?IsMobile=1';
                                                                        } // OnClose
                                                                    }); // MessageBox
                }
            });
        }

        //        function DownloadPrimaryDatabase(vKey, OrgID) {

        //            AjaxSend({
        //                url: 'MobizExport/AdjustZipFile',
        //                data: { OrgID: OrgID },
        //                error: function (err) {
        //                },
        //                success: function (vData) { //success 6                        
        //                    Transfers.Download({
        //                        Key: vData.Results,
        //                        Destination: pData.OrgID + '.zip',
        //                        Success: function (vData) {

        //                            Sql.DeleteIfExist('Mobiz');

        //                            Transfers.ExtractZipFile({
        //                                Source: vData.Destination,
        //                                Destination: 'data',
        //                                ExtractToFiles: true,
        //                                Success: function (vData) {
        //                                    Sql.DownloadAndApplySchema({
        //                                        ForceUpdate: true,
        //                                        Success: function (vData) {
        //                                            //update private db
        //                                            var pSql = '';
        //                                            var pDt;
        //                                            var pDTData = [];

        //                                            Sql.AttachDatabase(sessionStorage.OrgID);

        //                                            pSql = 'Select * from private.TablesLastUpdated';
        //                                            pDt = Sql.Execute(pSql);

        //                                            for (var i = 0; i < pDt.Rows.length; i++) {
        //                                                pDTData[i] = pDt.Rows[i].TableName + ',' + ParseDate(pDt.Rows[i].LastUpdated)
        //                                            }

        //                                            AjaxSend({
        //                                                url: 'MobizExport/GetPrivateDatabaseUpdate',
        //                                                data: { OrgID: sessionStorage.OrgID, PeopleID: Android.GetSessionData().PeopleUserID, DeviceUUID: Device.Serial(), Tables: pDTData },
        //                                                error: function (err) {
        //                                                },
        //                                                success: function (vData) { //success 6                        
        //                                                    mInterval = setInterval(FollowUpRequestDataForPrivateDatabase, 5000);
        //                                                }
        //                                            });
        //                                        },
        //                                        Error: function (vData) {
        //                                            alert('Error in DownloadAndApplySchema: ');
        //                                            console.log('Error in DownloadAndApplySchema: ' + vData);
        //                                        }
        //                                    });
        //                                }
        //                            });
        //                        }
        //                    });
        //                }
        //            }); //ajaxsend
        //        }

        //        function FollowUpRequestDataForPrivateDatabase() {
        //            AjaxSend({
        //                url: 'MobizExport/RequestFollowUp',
        //                success: function (vData) {
        //                    if (vData.Result != undefined) {
        //                        if (vData.Result.length == 75) {
        //                            ClearInterval();
        //                            StartComparing(vData.Result);
        //                        }
        //                        else if (vData.Result == 'Error') {
        //                            ClearInterval();
        //                            $.mobile.changePage('#pageDownloadError');
        //                        }
        //                        else if (vData.Result.indexOf('Error') >= 0) {
        //                            ClearInterval();
        //                            MessageBox(vData.Result);
        //                        }
        //                        else if (vData.Result == 'nothing') {
        //                            ClearInterval();
        //                            MessageBox("Database is up to date.", "Import");
        //                            return;
        //                        }
        //                    }
        //                }
        //            });
        //        }

        function StartComparing(vKey) {

            Android.StopFlushing();
            Transfers.Download({
                Key: vKey,
                Destination: vKey + '.zip',
                Success: function (vData) { //success 5
                    Transfers.ExtractZipFile({
                        Source: vData.Destination,
                        Success: function (vData) { //success 4
                            var pCsv = new CsvReader(vData.Destination);
                            var pDirectory = Transfers.GetInternalFiles(vData.Source);
                            var pDR;
                            var pDataToBeSend = [];
                            var pDataToBeSendCount = 0;
                            var pDRCount;
                            var intRowCntr;

                            for (var i = 0; i < pDirectory.Directories.length; i++) {

                                if (pDirectory.Directories[i].FullName != undefined) {

                                    //delete first so that all users that has been denied of their access will be updated

                                    if (pDirectory.Directories[i].FullName == 'LibrarySecurityRoleUser') {
                                        pSql = 'Delete from private.LibrarySecurityRoleUser'
                                        Sql.ExecuteSql(pSql);
                                    } else if (pDirectory.Directories[i].FullName == 'LibrarySecurityRoleModules') {
                                        pSql = 'Delete from private.LibrarySecurityRoleModules';
                                        Sql.ExecuteSql(pSql);
                                    }

                                    pCsv.Open(pDirectory.Directories[i].FullName);
                                    intRowCntr = 0;
                                    do {

                                        pDR = pCsv.ReadLine();
                                        if (pDR == undefined) break;

                                        var vResults;
                                        //                                        if (IsDatabaseExists) {
                                        if (pDR.ID == undefined) {
                                            if (pDirectory.Directories[i].FullName == 'LibraryPeopleUserAccount') {
                                                ImportExportData_offline().SelectQuery(pDirectory.Directories[i].FullName, pDR.PeopleID);
                                            }
                                        }
                                        else
                                            vResults = ImportExportData_offline().SelectQuery(pDirectory.Directories[i].FullName, pDR.ID);
                                        //  }
                                        //                                        else {

                                        //                                            if (pDirectory.Directories[i].FullName.startsWith("Mobiz")) {
                                        //                                                if (pDR.ID == undefined) {
                                        //                                                    var vRows = [];
                                        //                                                    vResults = { Rows: vRows };
                                        //                                                }
                                        //                                                else {
                                        //                                                    vResults = ImportExportData_offline().SelectQuery(pDirectory.Directories[i].FullName, pDR.ID);
                                        //                                                }
                                        //                                            } else {
                                        //                                                var vRows = [];
                                        //                                                vResults = { Rows: vRows };
                                        //                                            }
                                        //                                        }

                                        switch (pDirectory.Directories[i].FullName) {
                                            case "SystemDefaults": ImportExportData_offline().SaveSystemDefaults((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizSystemDefaults": ImportExportData_offline().SaveMobizSystemDefaults((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryCountriesLanguageLabel": ImportExportData_offline().SaveMobizLibraryCountriesLanguageLabel((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryDiscountSettings": ImportExportData_offline().SaveLibraryDiscountSettings((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibrarySecurityModuleMain": ImportExportData_offline().SaveLibrarySecurityModuleMain((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibrarySecurityModule": ImportExportData_offline().SaveLibrarySecurityModule((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibrarySecurityRole": ImportExportData_offline().SaveLibrarySecurityRole((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibrarySecurityRoleModules": ImportExportData_offline().SaveLibrarySecurityRoleModules((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibrarySecurityRoleUser": ImportExportData_offline().SaveLibrarySecurityRoleUser((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryReceiptSettings": ImportExportData_offline().SaveLibraryReceiptSettings((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryReceiptSettingsFowst": ImportExportData_offline().SaveLibraryReceiptSettingsFowst((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryReceiptSettingsMessage": ImportExportData_offline().SaveLibraryReceiptSettingsMessage((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryDevice": ImportExportData_offline().SaveLibraryDevice((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryDeviceFowst": ImportExportData_offline().SaveLibraryDeviceFowst((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryDeviceFowstPrinter": ImportExportData_offline().SaveLibraryDeviceFowstPrinter((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryOrganizationPriceLevel": ImportExportData_offline().SaveLibraryOrganizationPriceLevel((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryBanks": ImportExportData_offline().SaveMobizLibraryBanks((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryBankBranch": ImportExportData_offline().SaveMobizLibraryBankBranch((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryCountries": ImportExportData_offline().SaveMobizLibraryCountries((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryCities": ImportExportData_offline().SaveMobizLibraryCities((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryAreaCodes": ImportExportData_offline().SaveMobizLibraryAreaCodes((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryTelcos": ImportExportData_offline().SaveMobizLibraryTelcos((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryTelcoPrefixes": ImportExportData_offline().SaveMobizLibraryTelcoPrefixes((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryPIMPriceLevel": ImportExportData_offline().SaveMobizLibraryPIMPriceLevel((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryPIMMeasure": ImportExportData_offline().SaveMobizLibraryPIMMeasure((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryPIMMeasureSign": ImportExportData_offline().SaveMobizLibraryPIMMeasureSign((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryPeoplePreNominalSuffix": ImportExportData_offline().SaveMobizLibraryPeoplePreNominalSuffix((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryPeople": ImportExportData_offline().SaveMobizLibraryPeople((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryPeopleUserAccount": ImportExportData_offline().SaveMobizLibraryPeopleUserAccount((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryPeopleOrganization": ImportExportData_offline().SaveMobizLibraryPeopleOrganization((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryOrganization": ImportExportData_offline().SaveMobizLibraryOrganization((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryOrganizationContactEmail": ImportExportData_offline().SaveMobizLibraryOrganizationContactEmail((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryOrganizationTradeName": ImportExportData_offline().SaveMobizLibraryOrganizationTradeName((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryOrganizationFOWST": ImportExportData_offline().SaveMobizLibraryOrganizationFOWST((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryOrganizationFOWSTTel": ImportExportData_offline().SaveMobizLibraryOrganizationFOWSTTel((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryOrganizationFOWSTEmail": ImportExportData_offline().SaveMobizLibraryOrganizationFOWSTEmail((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryOrganizationFOWSTOperationHRs": ImportExportData_offline().SaveMobizLibraryOrganizationFOWSTOperationHRs((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryOrganizationRepresentatives": ImportExportData_offline().SaveMobizLibraryOrganizationRepresentatives((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryOrganizationRepresentativesEmail": ImportExportData_offline().SaveMobizLibraryOrganizationRepresentativesEmail((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryOrganizationRepresentativesMobile": ImportExportData_offline().SaveMobizLibraryOrganizationRepresentativesMobile((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryOrganizationRepresentativesTel": ImportExportData_offline().SaveMobizLibraryOrganizationRepresentativesTel((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryOrganizationCustomer": ImportExportData_offline().SaveLibraryOrganizationCustomer((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryOrganizationEmployee": ImportExportData_offline().SaveLibraryOrganizationEmployee((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryOrganizationEmployeeFOWST": ImportExportData_offline().SaveLibraryOrganizationEmployeeFOWST((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryOrganizationPIM": ImportExportData_offline().SaveLibraryOrganizationPIM((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryPIMProduct": ImportExportData_offline().SaveMobizLibraryPIMProduct((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryPIMProductMeasurement": ImportExportData_offline().SaveMobizLibraryPIMProductMeasurement((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryPIMProductMeasurementBarcode": ImportExportData_offline().SaveMobizLibraryPIMProductMeasurementBarcode((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "MobizLibraryPIMProductPriceLevel": ImportExportData_offline().SaveMobizLibraryPIMProductPriceLevel((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryPimMenu": ImportExportData_offline().SaveLibraryPimMenu((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryPimGroups": ImportExportData_offline().SaveLibraryPimGroups((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryPimGroupPims": ImportExportData_offline().SaveLibraryPimGroupPims((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryPimGroupFowsts": ImportExportData_offline().SaveLibraryPimGroupFowsts((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "TablesLastUpdated": ImportExportData_offline().SaveTablesLastUpdated((vResults.Rows.length > 0 ? false : true), pDR); break;

                                            case "LibraryDevicePrinter": ImportExportData_offline().SaveLibraryDevicePrinter((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryDevicePrinterCategory": ImportExportData_offline().SaveLibraryDevicePrinterCategory((vResults.Rows.length > 0 ? false : true), pDR); break;
                                            case "LibraryDevicePrinterFowst": ImportExportData_offline().SaveLibraryDevicePrinterFowst((vResults.Rows.length > 0 ? false : true), pDR); break;

                                        };

                                    } while (true);
                                } //if (pDirectory.Directories[i].FullName != undefined) { 
                            }; //end for
                            Bs.Loading('hide');
                            $('#pageDownloadDatabaseSuccessful').pageshow();
                        }
                    }); //end Transfers.ExtractZipFile
                }
            }); //Transfers.Download
        }

        //        function FollowUpRequestedData(IsDatabaseExists) {
        //            AjaxSend({
        //                url: 'MobizExport/RequestFollowUp',
        //                data: { OrgID: pData.OrgID, PeopleID: Android.GetSessionData().PeopleUserID },
        //                success: function (vData) {
        //                    if (vData.Result != undefined) {
        //                        if (vData.Result.length == 75) {
        //                            ClearInterval();
        //                            SetPopUpMessage(3, 'Writing data to local database');
        //                            UpdateDatabase(vData.Result, IsDatabaseExists, pData.OrgID);
        //                        }
        //                        else {
        //                        }
        //                    }
        //                }
        //            });
        //        }

        //        function UpdateDatabase(vKey, IsDatabaseExists, OrgID) {
        //            Transfers.Download({
        //                Key: vKey,
        //                Destination: 'RequestFinalUpdate' + OrgID + '.zip',
        //                Success: function (vDataDLFinal) { //success 2                                                                
        //                    Transfers.ExtractZipFile({
        //                        Source: vDataDLFinal.Destination,
        //                        Success: function (vData) { //success 1

        //                            var pCsv = new CsvReader(vData.Destination);
        //                            var pDirectory = Transfers.GetInternalFiles(vData.Source);
        //                            var pDR;
        //                            var pDataToBeSend = [];
        //                            var pDataToBeSendCount = 0;
        //                            var pDRCount;
        //                            var intRowCntr;

        //                            for (var i = 0; i < pDirectory.Directories.length; i++) {

        //                                if (pDirectory.Directories[i].FullName != undefined) {

        //                                    pCsv.Open(pDirectory.Directories[i].FullName);
        //                                    intRowCntr = 0;
        //                                    do {

        //                                        pDR = pCsv.ReadLine();
        //                                        if (pDR == undefined) break;

        //                                        var vResults;
        //                                        if (IsDatabaseExists) {
        //                                            if (pDR.ID == undefined) {
        //                                                if (pDirectory.Directories[i].FullName == 'LibraryPeopleUserAccount') {
        //                                                    ImportExportData_offline().SelectQuery(pDirectory.Directories[i].FullName, pDR.PeopleID);
        //                                                }
        //                                            }
        //                                            else
        //                                                vResults = ImportExportData_offline().SelectQuery(pDirectory.Directories[i].FullName, pDR.ID);
        //                                        }
        //                                        else {

        //                                            if (pDirectory.Directories[i].FullName.startsWith("Mobiz")) {
        //                                                if (pDR.ID == undefined) {
        //                                                    var vRows = [];
        //                                                    vResults = { Rows: vRows };
        //                                                }
        //                                                else {
        //                                                    vResults = ImportExportData_offline().SelectQuery(pDirectory.Directories[i].FullName, pDR.ID);
        //                                                }
        //                                            } else {
        //                                                var vRows = [];
        //                                                vResults = { Rows: vRows };
        //                                            }
        //                                        }

        //                                        switch (pDirectory.Directories[i].FullName) {
        //                                            case "SystemDefaults": ImportExportData_offline().SaveSystemDefaults((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizSystemDefaults": ImportExportData_offline().SaveMobizSystemDefaults((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryCountriesLanguageLabel": ImportExportData_offline().SaveMobizLibraryCountriesLanguageLabel((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryDiscountSettings": ImportExportData_offline().SaveLibraryDiscountSettings((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibrarySecurityModuleMain": ImportExportData_offline().SaveLibrarySecurityModuleMain((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibrarySecurityModule": ImportExportData_offline().SaveLibrarySecurityModule((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibrarySecurityRole": ImportExportData_offline().SaveLibrarySecurityRole((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibrarySecurityRoleModules": ImportExportData_offline().SaveLibrarySecurityRoleModules((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibrarySecurityRoleUser": ImportExportData_offline().SaveLibrarySecurityRoleUser((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryReceiptSettings": ImportExportData_offline().SaveLibraryReceiptSettings((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryReceiptSettingsFowst": ImportExportData_offline().SaveLibraryReceiptSettingsFowst((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryReceiptSettingsMessage": ImportExportData_offline().SaveLibraryReceiptSettingsMessage((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryDevice": ImportExportData_offline().SaveLibraryDevice((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryDeviceFowst": ImportExportData_offline().SaveLibraryDeviceFowst((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryDeviceFowstPrinter": ImportExportData_offline().SaveLibraryDeviceFowstPrinter((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryOrganizationPriceLevel": ImportExportData_offline().SaveLibraryOrganizationPriceLevel((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryBanks": ImportExportData_offline().SaveMobizLibraryBanks((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryBankBranch": ImportExportData_offline().SaveMobizLibraryBankBranch((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryCountries": ImportExportData_offline().SaveMobizLibraryCountries((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryCities": ImportExportData_offline().SaveMobizLibraryCities((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryAreaCodes": ImportExportData_offline().SaveMobizLibraryAreaCodes((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryTelcos": ImportExportData_offline().SaveMobizLibraryTelcos((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryTelcoPrefixes": ImportExportData_offline().SaveMobizLibraryTelcoPrefixes((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryPIMPriceLevel": ImportExportData_offline().SaveMobizLibraryPIMPriceLevel((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryPIMMeasure": ImportExportData_offline().SaveMobizLibraryPIMMeasure((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryPIMMeasureSign": ImportExportData_offline().SaveMobizLibraryPIMMeasureSign((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryPeoplePreNominalSuffix": ImportExportData_offline().SaveMobizLibraryPeoplePreNominalSuffix((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryPeople": ImportExportData_offline().SaveMobizLibraryPeople((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryPeopleUserAccount": ImportExportData_offline().SaveMobizLibraryPeopleUserAccount((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryPeopleOrganization": ImportExportData_offline().SaveMobizLibraryPeopleOrganization((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryOrganization": ImportExportData_offline().SaveMobizLibraryOrganization((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryOrganizationContactEmail": ImportExportData_offline().SaveMobizLibraryOrganizationContactEmail((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryOrganizationTradeName": ImportExportData_offline().SaveMobizLibraryOrganizationTradeName((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryOrganizationFOWST": ImportExportData_offline().SaveMobizLibraryOrganizationFOWST((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryOrganizationFOWSTTel": ImportExportData_offline().SaveMobizLibraryOrganizationFOWSTTel((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryOrganizationFOWSTEmail": ImportExportData_offline().SaveMobizLibraryOrganizationFOWSTEmail((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryOrganizationFOWSTOperationHRs": ImportExportData_offline().SaveMobizLibraryOrganizationFOWSTOperationHRs((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryOrganizationRepresentatives": ImportExportData_offline().SaveMobizLibraryOrganizationRepresentatives((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryOrganizationRepresentativesEmail": ImportExportData_offline().SaveMobizLibraryOrganizationRepresentativesEmail((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryOrganizationRepresentativesMobile": ImportExportData_offline().SaveMobizLibraryOrganizationRepresentativesMobile((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryOrganizationRepresentativesTel": ImportExportData_offline().SaveMobizLibraryOrganizationRepresentativesTel((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryOrganizationCustomer": ImportExportData_offline().SaveLibraryOrganizationCustomer((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryOrganizationEmployee": ImportExportData_offline().SaveLibraryOrganizationEmployee((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryOrganizationEmployeeFOWST": ImportExportData_offline().SaveLibraryOrganizationEmployeeFOWST((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryOrganizationPIM": ImportExportData_offline().SaveLibraryOrganizationPIM((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryPIMProduct": ImportExportData_offline().SaveMobizLibraryPIMProduct((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryPIMProductMeasurement": ImportExportData_offline().SaveMobizLibraryPIMProductMeasurement((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryPIMProductMeasurementBarcode": ImportExportData_offline().SaveMobizLibraryPIMProductMeasurementBarcode((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "MobizLibraryPIMProductPriceLevel": ImportExportData_offline().SaveMobizLibraryPIMProductPriceLevel((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryPimMenu": ImportExportData_offline().SaveLibraryPimMenu((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryPimGroups": ImportExportData_offline().SaveLibraryPimGroups((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryPimGroupPims": ImportExportData_offline().SaveLibraryPimGroupPims((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "LibraryPimGroupFowsts": ImportExportData_offline().SaveLibraryPimGroupFowsts((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                            case "TablesLastUpdated": ImportExportData_offline().SaveTablesLastUpdated((vResults.Rows.length > 0 ? false : true), pDR); break;
        //                                        };

        //                                    } while (true);
        //                                } //if (pDirectory.Directories[i].FullName != undefined) { 
        //                            }; //end for

        //                            $('#pageDownloadDatabaseSuccessful').pageshow();
        //                        }
        //                    }); //end transfer.ExtractZipFile
        //                }
        //            }); //end transfer.download
        //        }

        //        function SaveSystemDefaults(vPvtSysDefData, iRow) {
        //            setTimeout(function () {
        //                SaveSystemDefaultsNow(vPvtSysDefData.d.Results.Rows[iRow]);
        //                iRow++;

        //                if (iRow < vPvtSysDefData.d.Results.Rows.length - 1) {
        //                    SaveSystemDefaults(vPvtSysDefData, iRow);
        //                }
        //                else
        //                    return;
        //            }, 0)

        //        };

        //        function SaveSystemDefaultsNow(vData) {

        //            IsSuccess = ImportExportData_offline().SaveSystemDefaults(true, vData);
        //            //                    }
        //        };

        //        function GetUpdateFromServer(vOrgID, vTable, vData, vRow) {

        //            if (vRow >= vData.Rows.length) {
        //                return;
        //            }

        //            var pData = {};
        //            pData.OrgID = vOrgID;
        //            pData.Table = vTable;
        //            pData.ID = vData.ID;

        //            AjaxSend({
        //                url: "MobizExportController/RequestDataFromTable",
        //                data: pData,
        //                success: function (vRetData) {

        //                    GetUpdateFromServer(vOrgID, vTable, vData, vRow + 1)

        //                }
        //            });

        //        };

        window.SetPopUpMessage = function (IsComparing, Table) {

            var vTable = '';
            var msg = '';

            //            switch (Table) {
            //                case 'MobizLibraryPeople': { vTable = 'People'; break; }
            //                case 'MobizLibraryPeoplePreNominalSuffix': { vTable = 'People Suffix'; break; }
            //                case 'MobizLibraryPeopleUserAccount': { vTable = 'People Account'; break; }
            //                case 'MobizLibraryPeopleOrganization': { vTable = 'People Organization'; break; }
            //                case 'MobizLibraryOrganization': { vTable = 'Organization'; break; }
            //                case 'MobizLibraryOrganizationContactEmail': { vTable = 'Organization Email'; break; }
            //                case 'MobizLibraryOrganizationTradeName': { vTable = 'Organization Trade Name'; break; }
            //                case 'MobizLibraryOrganizationFOWST': { vTable = 'Organization FOWST'; break; }
            //                case 'MobizLibraryOrganizationRepresentatives': { vTable = 'Organization Representative'; break; }
            //                case 'MobizLibraryPIMProduct': { vTable = 'Products'; break; }
            //                case 'MobizLibraryPIMMeasureSign': { vTable = 'Product Measurements'; break; }
            //                case 'MobizLibraryPIMProductMeasurement': { vTable = 'Product Measurements'; break; }
            //                case 'MobizLibraryPIMProductMeasurementBarcode': { vTable = 'Product Barcode'; break; }
            //                case 'MobizLibraryPIMProductPriceLevel': { vTable = 'Product Prices'; break; }
            //                case 'MobizLibraryPIMPriceLevel': { vTable = 'Product Prices'; break; }
            //                case 'MobizLibraryCities': { vTable = 'Cities'; break; }
            //                case 'MobizLibraryAreaCodes': { vTable = 'Area Codes'; break; }
            //                case 'MobizLibraryBanks': { vTable = 'Banks'; break; }
            //                case 'MobizLibraryTelcos': { vTable = 'Telcos'; break; }
            //                case 'MobizSystemDefaults': { vTable = 'System Defaults'; break; }
            //                case 'MobizLibraryTelcoPrefixes': { vTable = 'Telco Prefixes'; break; }
            //                case 'MobizLibraryCountriesLanguageLabel': { vTable = 'Countries Language'; break; }
            //                case 'MobizLibraryCountries': { vTable = 'Countries'; break; }
            //                case 'MobizLibraryPIMMeasure': { vTable = 'Product Measurements'; break; }
            //                case 'SystemDefaults': { vTable = 'System Defaults'; break; }
            //                case 'LibraryOrganizationEmployee': { vTable = 'Employees'; break; }
            //                case 'LibraryDiscountSettings': { vTable = 'Discount Settings'; break; }
            //                case 'LibraryDevice': { vTable = 'Device'; break; }
            //                case 'LibraryDeviceFowst': { vTable = 'Printers'; break; }
            //                case 'LibrarySecurityRole': { vTable = 'Security Roles'; break; }
            //                case 'LibraryOrganizationPIM': { vTable = 'Products'; break; }
            //                case 'LibraryOrganizationCustomer': { vTable = 'Customers'; break; }
            //                case 'LibraryOrganizationPriceLevel': { vTable = 'Price Level'; break; }
            //                case 'LibrarySecurityModuleMain': { vTable = 'Security Module'; break; }
            //                case 'LibrarySecurityModule': { vTable = 'Security Module'; break; }
            //                case 'LibrarySecurityRoleModules': { vTable = 'Security Role'; break; }
            //                case 'LibrarySecurityRoleUser': { vTable = 'Security Role'; break; }
            //                case 'LibraryOrganizationEmployeeFOWST': { vTable = 'Organization FOWST'; break; }
            //                case 'LibraryReceiptSettings': { vTable = 'Receipt Settings'; break; }
            //                case 'LibraryReceiptSettingsFowst': { vTable = 'Receipt FOWST'; break; }
            //                case 'LibraryReceiptSettingsMessage': { vTable = 'Receipt Messages'; break; }
            //                default: { vTable = Table }

            //            }

            if (vTable.indexOf('Error') > 0) {
                msg = vTable;
            }
            else if (IsComparing == 1) {
                msg = 'Checking updates for ' + vTable;
            }
            else if (IsComparing == 2) {
                msg = 'Updating ' + vTable;
            }
            else {
                msg = Table;
            }

            msg = 'Please Wait ... <br> <br>' + msg;

            $("#dvpopup").popup("close");
            $('#popMessage').html(msg);
            $("#dvpopup").popup("open");
        }
    });

