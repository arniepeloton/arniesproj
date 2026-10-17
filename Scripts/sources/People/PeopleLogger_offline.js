define([
    'common'
    ], function () {

        var mInitilized = false;

        window.PeopleLogger_offline = function () {

            if (mInitilized == false) {
                mInitilized = true;
                Android.SetSessionData({});
            }

            this.CheckUser = function (vData) {
                // this function does NOT require current public and private database

                // bookmark: 2015_12_28__16_01_57_6555: new database naming convention
                if (window.NewDatabaseNaming) 
                    return CheckUserMulti();
                else
                    return CheckUser('');

                function checkPeopleUserAccount() {
                    var pLastOrgID = Transfers.ReadFile("LastOrgID.txt");
                    AjaxSend({
                        url: '/People/CheckPeopleUserAccount',
                        timeout: proj.AjaxTimeout,
                        data: {
                            IsCookieless: 1,
                            Username: vData.Username,
                            UserPassword: vData.UserPassword,
                            Serial: Device.Serial(),
                            IsMobile: 1,
                            CheckLastUpdatedOrgID: pLastOrgID
                        },
                        success: function (vData) {
                            localStorage['ASP.NET_SessionId'] = vData['ASP.NET_SessionId'];

                            //location = '/CheckPrinter';
                            //window.location = '/People/PeopleMenu?IsMobile=1';

                            Proceed();
                            return; // 2023_05_24__09_45_20_0954

                            if (vData.TablesLastUpdated == null) 
                                Proceed();
                            else {
                                var pLastSyncText = Transfers.ReadFile('LastSync/' + pLastOrgID + '.txt');
                                var pLastSync = ParseDate(pLastSyncText);
                                if (pLastSync == null) {
                                    Proceed();
                                    return;
                                }
                                var pTablesLastUpdated = ParseDate(vData.TablesLastUpdated);
                                if (pTablesLastUpdated == null) {
                                    Proceed();
                                    return;
                                }

                                if (pLastSync < pTablesLastUpdated) 
                                    NewMessageBox('The server has newer data (' + FormatDate(pTablesLastUpdated, {Format:'MM/dd/yyyy hh:mm a'}) + 
                                        ') compared to the data in this device (' + FormatDate(pLastSync, {Format:'MM/dd/yyyy hh:mm a'}) + '). ' +
                                        'Do you want to synchronize database?', '', 
                                        [
                                            {
                                                Text: 'Yes, go to Troubleshooting Tools',
                                                Click: function() {
                                                    sessionStorage.OrgID = pLastOrgID;
                                                    require(['promise!/Scripts/sources/ImportExportData/ImportExportData.js?crc='], function() {
                                                        Web.SetOrgID(sessionStorage["OrgID"]);
                                                        Web.HandleOrganization();
                                                        window.SyncDataPrompt();
                                                    });
                                                }
                                            },
                                            {
                                                Text: 'No',
                                                Click: function() {
                                                    Proceed();
                                                }
                                            }
                                        ]
                                    );
                                else
                                    Proceed();

                            } // END else

                            function Proceed() {
                                var IsExists = 0;

                                IsExists = Transfers.ReadFile('IsFirstTime' + '.flg')

                                if (IsExists == undefined) {
                                    location = MergeUrlParameters({
                                        IsMobile: CBool(IsMobile) ? 1 : ''
                                    }, '/Signin/Notice');
                                }
                                else {
                                    location = MergeUrlParameters({
                                        IsMobile: CBool(IsMobile) ? 1 : ''
                                    }, '/People/PeopleMenu');

                                }
                            }
                        },
                        error: function () {
                            $('#btnSignIn').removeAttr("disabled");
                            var pConfirmResult;
                            var pText1 = 'Unable to sign in online. ';
                            // the first letter should have been "Y" but it's intentionally left out
                            var pText2 = 'ou can still use the POS offline but other modules such as Sales Returns require internet connection.';
                            if (window.confirm) {
                                pConfirmResult = confirm(pText1 + 'Would you like to continue? If you click "OK", y' + pText2);
                                if (pConfirmResult) {
                                    //                                    location = '/CheckPrinter';
                                    window.location = '/People/PeopleMenu?IsMobile=1';
                                } 
                            } else {
                                Android.Toast(pText1 + ' Y' + pText2);
                                window.location = '/People/PeopleMenu?IsMobile=1';
                            }
                            //                                MessageBox('Unable to log in online. Please login again. If you click "Continue", you may be ask to login in later.', '', 
                            //                                    [
                            //                                        {
                            //                                            Text: 'Login again',
                            //                                            Click: function() {
                            //                                                $('#btnSignIn').show();
                            //                                                $('#divSignIn').hide();
                            //                                                $('html, body').animate({
                            //                                                    scrollTop: $('#content').offset().top + 'px'
                            //                                                }, 'fast');
                            //                                            }
                            //                                        },
                            //                                        {
                            //                                            Text: 'Continue',
                            //                                            Click: function () {
                            //                                                location = '/CheckPrinter';
                            ////                                                window.location = '/People/PeopleMenu?IsMobile=1';
                            //                                            }
                            //                                        }
                            //                                    ]
                            //                                );
                        }
                    });
                }; //end of checkPeopleUserAccount

                function CheckUserMulti() {
                    var pResult;
                    var pFilename;
                    var pFilenames = Sql.GetDatabaseFiles(); // including extension
                    Android.AddLog('Databases: ' + pFilenames);
                    for (var p = 0; p <= pFilenames.length - 1; p++) {
                        pFilename = pFilenames[p];
                        pFilename = GetFilenameWithoutExtension(pFilename);
                        if (IsGuid(pFilename)) {
                            pResult = CheckUser(pFilename);
                            Android.AddLog('Result for checking user in db ' + pFilename + ' is: ' + pResult);
                            if (pResult)
                                return true;
                        }
                    }

                    return CheckUser();
                }

                function CheckUser(vOrgID) {
                    // this function requires current public and private database
                    var pDbName;
                    try {
                        // bookmark: 2015_12_28__16_01_57_6555: new database naming convention
                        if (window.NewDatabaseNaming
                            && (CStr2(vOrgID) != ''))

                            pDbName = vOrgID + '.public.db';
                        else
                            pDbName = 'Mobiz';

                        try {

                            //Check if table MobizLibraryPeopleUserAccount exist
                            if (Sql.IsExist(pDbName)) {
                                Android.AddLog(pDbName + ' database exists');
                                try {
                                    Sql.OpenDatabase(pDbName);
                                    //check if db has record
                                    var pSql = 'SELECT * FROM LibraryOrganization LIMIT 1';
                                    var pDT = Sql.Execute(pSql);

                                    if (pDT.Rows.length == 0) {
                                        
                                        return RestoreBackup();
                                        //var vFolders = Transfers.GetAllSubFolders('../data/backup/');

                                        //if (vFolders.length > 0) {                                            

                                            //Transfers.MoveFile('../data/backup/' + vFolders[0] + '/' + pDbName + '.public.db', '../data/' + pDbName + '.public.db');
                                            //    PeopleLogger_offline().CheckUser(vData);
                                            //    Android.Toast('The system found problems in your data, please Synchronize your data as soon as possible.')


                                        //}                                        
                                    }
                                }
                                catch (e) {
                                    return RestoreBackup();
                                }
                            }
                            else {
                                Android.AddLog(pDbName + ' database does not exist');
                                if (Sql.IsExist(pDbName + '.back')) {
                                    Android.Toast('The system found problems in your data, please Synchronize your data as soon as possible.')
                                
                                    return RestoreBackup();
                                }
                                else {
                                    Android.AddLog(pDbName + '.back does not exist');
                                    Android.UpdateSessionData({ PeopleData: vData });
                                    return false;
                                }
                            }
                        }
                        catch (e) {

                        }
                    } catch (e) {
                        console.trace(e.stack);
                        Android.UpdateSessionData({ PeopleData: vData });
                        return false;
                    }

                    function RestoreBackup() {
                        Android.AddLog('Restoring backup database');
                        var vFolders;

                        try {
                            Transfers.MoveFile('data/' + pDbName, 'data/' + pDbName + '.back');
                            vFolders = Transfers.GetAllSubFolders('../data/backup/');

                            if (vFolders.length > 0) {

                                Transfers.MoveFile('data/backup/' + vFolders[vFolders.length - 1] + '/' + pDbName, 'data/' + pDbName);
                                
                                PeopleLogger_offline().CheckUser(vData);
                                Android.Toast('The system found problems in your data, please Synchronize your data as soon as possible.')
                                return true
                            } else {
                                Android.AddLog('No backup database');
                                Android.UpdateSessionData({ PeopleData: vData });
                                return false
                            }
                        } catch (e) {
                            Android.AddLog(e.message);
                            Android.UpdateSessionData({ PeopleData: vData });
                            return false
                        }
                        
                    }

                    var pSql = "SELECT 1 FROM sqlite_master WHERE type = 'table' AND name = 'LibraryPeopleUserAccount'";
                    var pDT = Sql.Execute(pSql);

                    if (pDT.Rows.length > 0) {
                        pSql = 'SELECT LibraryPeopleUserAccount.*\n' +
                            ' FROM LibraryPeopleUserAccount\n' +
                            ' JOIN LibraryPeopleOrganization ON LibraryPeopleOrganization.PeopleID = LibraryPeopleUserAccount.PeopleID\n' +
                            ' WHERE LibraryPeopleUserAccount.Username = ' + ReadyString($.trim(vData.Username)) + '\n' +
                            ' AND LibraryPeopleOrganization.IsTerminated = 0';
                        pDT = Sql.Execute(pSql);

                        if (pDT.Error) {
                            MessageBox(pDT.Error);
                            return false;
                        };
                        if (pDT.Rows.length == 0) {
                            Android.AddLog('Username ' + $.trim(vData.Username) + ' does not exist in db: ' + pDbName);
                            Android.UpdateSessionData({ PeopleData: vData });
                            return false;
                        } else if (CBool(Android.VerifyHash(vData.UserPassword, pDT.Rows[0].UserPassword))) {
                            Android.Toast('Welcome: ' + vData.Username);
                            Android.UpdateSessionData({ PeopleUserID: pDT.Rows[0].PeopleID });

                            // not retrieved for now
                            Android.UpdateSessionData({
                                Username: vData.Username,
                                Password: vData.UserPassword
                            });

                            // store these for offline use.
                            localStorage.Username = vData.Username;
                            localStorage.Password = vData.UserPassword;
                            localStorage.HashedPassword = pDT.Rows[0].UserPassword;
                            Transfers.WriteFile('LastUserID.txt', null, pDT.Rows[0].PeopleID);

                            // bypass data integrity test for now
                            checkPeopleUserAccount();
                            return true;

                            //additional by janine. check if device has passed the data integrity test
                            var pSql = "SELECT 1 FROM sqlite_master WHERE type = 'table' AND name = 'TestData_Results'";
                            var pDT = Sql.Execute(pSql);

                            if (pDT.Rows.length > 0) {

                                var pSql = 'Select TestStatus, Result from TestData_Results';
                                var pDt = Sql.Execute(pSql);
                                //end of additional

                                if (pDt.Rows.length > 0) { //add to checking

                                    if (pDt.Rows[0].TestStatus == 'done') {
                                        if (pDt.Rows[0].Result == 'pass')
                                            checkPeopleUserAccount();
                                        else
                                            window.location = '/SelfTest/Failed';

                                    }
                                    else {
                                        //go back to test again. finish it!! required
                                        window.location = '/selftest';
                                    }

                                } else
                                    checkPeopleUserAccount();

                            } else
                                // window.location = '/selftest';
                                checkPeopleUserAccount(); // old code

                            return true;
                        } else {
                            return false;
                        };


                    } else {
                        Android.UpdateSessionData({ PeopleData: vData });
                        return false;
                    }
                } // END CheckUser
            }; // this.CheckUser = function() {

            return this;

        }; // window.PeopleLogger_offline = function() {

    });                                  // define

