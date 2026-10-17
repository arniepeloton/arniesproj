/// <reference path="C:\dev\debug\Mobiz\WebErp\Scripts3/Test/Index.js" />
require([
    'common',
    '/Scripts/sources/ImportExportData/ImportExportData_Offline.js?crc='
    ], function () {

        require([(OfflineMode ? '/Scripts/sources/People/PeopleLogger_offline.js?crc=' : '')], function () {
            $(function () {
                //             $('html').removeClass('ui-mobile');

                var IsForgotPassword;
                var isIE = false || !!document.documentMode;

                // for switching user from TURN OVER OF MONIES page
                window.SignInCheckUser = function () {
                    CheckUser();
                }
                localStorage.setItem('LoginUrl', location.href);
                $(document).ready(function () {
                    $('.ui-loader').hide();
                    setPrinterConnectionType('USB', true);
                    if (OfflineMode) {
                        if (CStr2(Device.Serial()).trim() == '') {
                            $('#txtDeviceSerial').val(Device.SuggestedSerial());
                            $('#frmPeopleLogger').hide();
                            $('#divDeviceSerial').show();
                            var pMessage = 'Please Enter Your Device\'s Serial Number (UUID).<br /><br />' +
                                'Due to operating system restrictions, Mobiz is unable to get the serial number (UUID) of your device. ' + '<br /><br />' + 
                                'Mobiz needs this device\'s serial number for validating this device with your company\'s authorized devices list. ' + '<br /><br />' + 
                                'You can enter the serial number (UUID) of this device or let Mobiz generate an alternate Mobiz serial number.' + '<br /><br />' + 
                                'Thank you.';
                            $('#spanError').html(pMessage).show();
                        }
                        Device.CheckServerDate();
                        var IsTraxOnly = null;

                        IsTraxOnly = Transfers.ReadFile('Trax.flg');

                        if (IsTraxOnly != undefined) {
                            location = MergeUrlParameters({
                                IsMobile: CBool(IsMobile) ? 1 : ''
                            }, '/Signin/TraxInfo');
                        }
                        if (Android.DebugMode()) {
                            $('#divDebugTest')
                                .append($('<a>')
                                    .text('pdf print test. DEBUG ONLY')
                                    .attr('href', '#')
                                    .click(function(e) {
                                        e.preventDefault();
                                        TestPrint();
                                        //doDrawerKick();
                                        //require(['/Scripts3/Test/PdfCreationTest.js?crc='], function() {
                                            
                                        //});
                                    })
                                );
                            // temp test
                            //if (window.DebugMode)
                            //    require(['/Scripts3/Test/PdfCreationTest2.js']);
                        }
                        try {
                            var pFreeSpace = 3000 * 1000 * 1024; //Device.GetFreeSpace();
                            pFreeSpace = CDec2(pFreeSpace);
                            if (pFreeSpace < 500 * 1000 * 1024) {
                                $('#spanErrorLowFreeSpace')
                                    .text('This device only has ' + AccountingFormat(pFreeSpace / (1000 * 1024)) + 'MB of free space. A minimum of 500MB of free space is recommended for optimal operation. Please free up disk space by deleting unnecessary files.')
                                $('#divErrorLowFreeSpace').show();
                            }
                        } catch (e) {
                            console.error(e.message);
                        } 
                    }
                    else
                        TestCookies();
                });

                $('#btnSUN').click(function () {
                    Transfers.WriteFile('IsSendSmS.flg', null, '')
                });

                $('#btnNONE').click(function () {
                    Transfers.DeleteFile('IsSendSmS.flg');
                });

                $('#frmPeopleLogger').submit(function (e) {
                    e.preventDefault();

                    $('#btnSignIn').click();
                });

                //            $('#lnkForgotPassword').click(function () {
                //        
                ////                e.preventDefault();

                //////                $.mobile.changePage('#pagePeopleName');
                ////                IsForgotPassword = true;

                //            });

                function TestCookies() {

                    try {
                        if (localStorage == undefined) {
                            $('#divNoCookies').show();
                            return;
                        };
                        $('#frmPeopleLogger').show();
                    } catch (e) {
                        $('#divNoCookies').show();
                        return;
                    };

                    docCookies.setItem('TestCookie', 'test', 3 * 1000 * 60, '/'); // 3 minutes
                    AjaxSend({
                        url: 'Login3/TestCookies',
                        timeout: proj.AjaxTimeout,
                        success: function (vData) {
                            var pAppVersion;

                            proj.IsCookieless = 1;
                            if ((vData != null) 
                                && (vData.Result == 'ok')) {
                                if (docCookies.getItem('TestCookie') == 'success')
                                    proj.IsCookieless = 0;
                            };
                            if (IsMobile) {
                                pAppVersion = Android.AppVersion();
                                if (CDec2(pAppVersion) < CDec2(vData.MinVersion)) {
                                    $('#spanError')
                                    .text('Server requires a client app of at least version ' + vData.MinVersion +
                                        '. Your app is version ' + pAppVersion)
                                    .show();
                                    $('#frmPeopleLogger').hide();
                                };
                            }; // if (IsMobile) {
                        }, // success
                        complete: function () {
                            docCookies.removeItem('TestCookie', '/');
                            SetFlag('MVWD7NZ8PX');
                        }
                    });

                }; // TestCookies


                $('#btnSignIn').click(function (e) {

                    e.preventDefault();

                    //// debugging code only
                    //// Transfers.WriteFile('cfe39df6-8809-4973-9515-810df75aed12fe639d1f-cd83-4b3e-8b0a-586731f479cb7a00b4a0-d74a-48fe-bf73-e811683a729ac530bdf3-ea03-4dd3-95cd-a384374aa5702015-04-10.dat', // not ok
                    //// Transfers.WriteFile('cfe39df6-8809-4973-9515-810df75aed12fe639d1f-cd83-4b3e-8b0a-586731f479cb7a00b4a0-d74a-48fe-bf73.dat', // ok
                    //Transfers.WriteFile('cfe39df6-8809-4973-9515-810df75aed12fe639d1f-cd83-4b3e-8b0a-586731f479cb7a00b4a0-d74a-48fe-bf73-e811683a729ac530bdf3-ea03-4dd3-95cd-a384374aa5702015-04-10.dat', // not ok
                    //    null,
                    //    '186|0|0|0|186|0|0|0|0|166.071428571429|19.9285714285714|0|0|0|166.071428571429|19.9285714285714|0|0');

                    $('#spanError').hide();

                    if ($.trim($('#txtUsername').val()) == '' || $.trim($('#txtUsername').val()).length <= 1) {
                        $('#spanError').show();
                        return;
                    }

                    if ($.trim($('#txtPassword').val()) == '' || $.trim($('#txtPassword').val()).length <= 1) {
                        $('#spanError').show();
                        return;
                    }

                    CheckUser();

                });

                $('#butUseDeviceSerial').click(function() {
                    var pValue = $('#txtDeviceSerial').val().trim();
                    if (pValue.length < 6) {
                        NewMessageBox('Device serial number must be 6 characters or longer.', 'Invalid serial number');
                        return;
                    }
                    Device.SetSerial(pValue);
                    var pSavedValue = Device.Serial();
                    if (pSavedValue == pValue) {
                        NewMessageBox('New serial was successfully saved. Please restart app for the changes to take effect.', 'Success', 
                            [
                                {
                                    Text: 'Restart app'
                                }
                            ], 
                            {
                                Close: function() {
                                    Android.Restart();
                                }
                            }
                        );
                    } else
                        NewMessageBox('App failed to save the new serial, ' + pValue + '. The saved serial was ' + pSavedValue + '.', 'Failed');
                });

                $('#butGenerateDeviceSerial').click(function() {
                    $('#txtDeviceSerial').val('Mobiz' + RandomString(20).toUpperCase());
                });

                function CreateNewEntry(IsNew, strID) {
                    var pData = {};
                    pData.IsNew = IsNew;
                    pData.ID = strID;

                    AjaxSend({
                        url: 'People/setCreateNew',
                        data: pData,
                        success: function (vData) {
                            var pParam;
                            var pUrl;

                            sessionStorage.clear();
                            if ((vData.IsNew || '') == 'TRUE') {
                                window.open(GetPathFormat('/People/PeopleInfoName'), '_self');
                                //                            alert('New user. Please sign up!');
                            } else {
                                var pDataPeopleID = {};
                                pDataPeopleID.ID = strID;

                                AjaxSend({
                                    url: 'SignIn/CheckPeopleIsFirstPartySignUp',
                                    data: pDataPeopleID,
                                    success: function (vData) {
                                        //if it's not a first party signup, therefore somebody created an account for him, redirect to account management to reset credentials
                                        if (vData.PeopleFirstPartySignUpData.Rows[0].IsFirstPartySignUp == '0') {
                                            window.open(GetPathFormat('/AccountManagement'), '_self');

                                        } else {

                                            pParam = GetUrlParameters();

                                            if (CStr2(pParam.LoginHash).length == 50)
                                                localStorage.LoginHash = pParam.LoginHash;
                                            //pUrl = '/MallLessor?' + $.param({LoginHash:pParam.LoginHash});


                                            GetPeopleOrg();
                                        }
                                    } // success
                                }); // AjaxSend
                            };
                        }
                    });

                } //CreateNewEntry

                function GetPeopleOrg() {

                    $('#divOrgApps').html('');
                    AjaxSend({
                        url: "People/getPeopleOrganizationList",
                        success: function (vDataOrg) {
                            $('#btnSignIn').removeAttr("disabled");
                            if (vDataOrg.PeopleData.Rows.length > 1) {
                                var a = [];

                                for (var i = 0; i < vDataOrg.PeopleData.Rows.length; i++) {
                                    a.push({
                                        data: vDataOrg.PeopleData.Rows[i],
                                        Text: vDataOrg.PeopleData.Rows[i].CompanyName,
                                        Click: function (e) {
                                            sessionStorage.OrgID = e.OrganizationID;
                                            sessionStorage.OrgName = e.CompanyName;
                                            sessionStorage.PeopleID = e.PeopleID;

                                            location = MergeUrlParameters({
                                                OrgID: e.OrganizationID,
                                            }, '/SIDocScanner');
                                        }
                                    })
                                }

                                PopupButtonList('', 'Select Organization', a);
                            }
                            else if (vDataOrg.PeopleData.Rows.length == 1) {
                                sessionStorage.OrgID = vDataOrg.PeopleData.Rows[0].OrganizationID;
                                sessionStorage.OrgName = vDataOrg.PeopleData.Rows[0].CompanyName;
                                sessionStorage.PeopleID = vDataOrg.PeopleData.Rows[0].PeopleID;

                                location = MergeUrlParameters({
                                    OrgID: vDataOrg.PeopleData.Rows[0].OrganizationID,
                                }, '/SIDocScanner');
                            }
                            else {
                                alert('Please ask your system administrator to add you as member on the organization');
                            }
                        }
                    });
                }

                function CheckUser() {
                    $('#btnSignIn').attr("disabled", "disabled");
                    //                    $('#divSignIn').show();


                    var pData = {};

                    pData.Username = (sessionStorage.IsFromTurnOverOfMoniesPage == "true" ? sessionStorage.TOUserName : $('#txtUsername').val());
                    pData.UserPassword = (sessionStorage.IsFromTurnOverOfMoniesPage == "true" ? sessionStorage.TOPassword : $('#txtPassword').val());

                    if (IsMobile) {
                        pData.Serial = Device.Serial();
                    };
                    pData.IsMobile = IsMobile;

                    if (OfflineMode) {
                        if (PeopleLogger_offline().CheckUser(pData)) {
                            // Apr 5, 2016
                            // dont go to people menu here
                            //location = MergeUrlParameters({
                            //    IsMobile: CBool(IsMobile) ? 1 : ''
                            //}, '/SIDocScanner');
                        } else {
                            if ((InAndroid)
                                || (window.InWinRt)
                                || (window.InWindows)) {
                                pData.IsCookieless = 1;
                                docCookies.removeItem('ASP.NET_SessionId');
                            };
                            $('#spanError').hide();
                            AjaxSend({
                                url: 'People/CheckPeopleUserAccount',
                                data: pData,
                                timeout: proj.AjaxTimeout,
                                success: function (vData) {
                                    if ($.trim(vData.PeopleUserAccountData) != '') {
                                        if ((InAndroid)
                                            || (window.InWinRt)
                                            || (window.InWindows)) {
                                            localStorage['ASP.NET_SessionId'] = vData['ASP.NET_SessionId'];
                                        } else {
                                            localStorage.removeItem('ASP.NET_SessionId');
                                        };

                                        //francis may 24 2014 'add employee to local db if it exist in live db
                                        AjaxSend({
                                            url: "People/getPeopleOrganizationList",
                                            success: function (vDataOrg) {
                                                var vRows = vDataOrg.PeopleData.Rows;
                                                if (vRows.length > 0) {

                                                    var IsExists;

                                                    for (i = 0; i < vRows.length; i++) {
                                                        IsExists = Transfers.ReadFile(vRows[i].OrganizationID + '.flg')
                                                        if (IsExists != undefined) {
                                                            if (Sql.IsExist('Mobiz')) {
                                                                //database exists then download employee info
                                                                AjaxSend({
                                                                    url: "People/GetPeopleInfo",
                                                                    success: function (vDataPeopleInfo) {
                                                                        if (vDataPeopleInfo.Results.Rows.length > 0) {
                                                                            vResults = ImportExportData_offline().SelectQuery('MobizLibraryPeople', vDataPeopleInfo.Results.Rows[0].ID);
                                                                            ImportExportData_offline().SaveMobizLibraryPeople((vResults.Rows.length > 0 ? false : true), vDataPeopleInfo.Results.Rows[0]);

                                                                            AjaxSend({
                                                                                url: "People/GetPeopleSecurityRole",
                                                                                data: { OrgID: vRows[i].OrganizationID },
                                                                                success: function (vDataPeopleSecurityRoles) {
                                                                                    if (vDataPeopleSecurityRoles.Results.Rows.length > 0) {
                                                                                        Sql.AttachDatabase(vRows[i].OrganizationID);
                                                                                        vResults = ImportExportData_offline().SelectQuery('LibrarySecurityRoleUser', vDataPeopleSecurityRoles.Results.Rows[0].ID);
                                                                                        ImportExportData_offline().SaveLibrarySecurityRoleUser((vResults.Rows.length > 0 ? false : true), vDataPeopleSecurityRoles.Results.Rows[0]);

                                                                                        AjaxSend({
                                                                                            url: "People/GetOrganizationEmployeeFOWST",
                                                                                            data: { OrgID: vRows[i].OrganizationID },
                                                                                            success: function (vDataOrganizationEmployeeFOWST) {
                                                                                                if (vDataOrganizationEmployeeFOWST.Results.Rows.length > 0) {

                                                                                                    for (j = 0; j <= vDataOrganizationEmployeeFOWST.Results.Rows.length - 1; j++) {
                                                                                                        vResults = ImportExportData_offline().SelectQuery('LibraryOrganizationEmployeeFOWST', vDataOrganizationEmployeeFOWST.Results.Rows[j].ID);
                                                                                                        ImportExportData_offline().SaveLibraryOrganizationEmployeeFOWST((vResults.Rows.length > 0 ? false : true), vDataOrganizationEmployeeFOWST.Results.Rows[j]);
                                                                                                    }

                                                                                                    AjaxSend({
                                                                                                        url: "People/GetPeopleOrganizationInfo",
                                                                                                        success: function (vDataPeopleOrganizationInfo) {

                                                                                                            vResults = ImportExportData_offline().SelectQuery('MobizLibraryPeopleOrganization', vDataPeopleOrganizationInfo.Results.Rows[0].ID);
                                                                                                            ImportExportData_offline().SaveMobizLibraryPeopleOrganization((vResults.Rows.length > 0 ? false : true), vDataPeopleOrganizationInfo.Results.Rows[0]);

                                                                                                            AjaxSend({
                                                                                                                url: "People/GetPeopleUserAccountInfo",
                                                                                                                success: function (vDataPeopleUserAccount) {
                                                                                                                    if (vDataPeopleUserAccount.Results.Rows.length > 0) {
                                                                                                                        vResults = ImportExportData_offline().SelectQuery('MobizLibraryPeopleUserAccount', vDataPeopleUserAccount.Results.Rows[0].ID);
                                                                                                                        ImportExportData_offline().SaveMobizLibraryPeopleUserAccount((vResults.Rows.length > 0 ? false : true), vDataPeopleUserAccount.Results.Rows[0]);

                                                                                                                        CheckUser();
                                                                                                                    }
                                                                                                                }
                                                                                                            });
                                                                                                        }
                                                                                                    });
                                                                                                }
                                                                                            }
                                                                                        });
                                                                                    }
                                                                                }
                                                                            });
                                                                        }
                                                                    },
                                                                    error: function () {
                                                                        //                                                                        $('#divSignIn').hide();
                                                                        $('#btnSignIn').removeAttr("disabled");
                                                                        $('#spanError')
                                                                            .text("Server error")
                                                                            .show();
                                                                    }
                                                                });
                                                                return;
                                                            }
                                                        }
                                                    }
                                                    //--------------
                                                    $('#spanError').hide();
                                                    Android.UpdateSessionData({ PeopleUserID: vData.PeopleUserAccountData });
                                                    window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                                                }
                                                else {
                                                    //not member of any org
                                                    //                                                    $('#divSignIn').hide();
                                                    $('#btnSignIn').removeAttr("disabled");
                                                    $('#spanError')
                                                    .text('You should be a member of an organization/company in order to use this program')
                                                    .show();
                                                }

                                            } //success
                                        });


                                    }
                                    else {
                                        //                                        $('#divSignIn').hide();
                                        $('#btnSignIn').removeAttr("disabled");
                                        if (vData.Error == undefined)
                                            $('#spanError')
                                        .text('Invalid username and password')
                                        .show();
                                        else
                                            $('#spanError')
                                        .text(vData.Error)
                                        .show();
                                        return;
                                    }
                                    //                                    if ($.trim(vData.PeopleUserAccountData) != '') {
                                    //                                        if (proj.IsCookieless == 1) {
                                    //                                            localStorage['ASP.NET_SessionId'] = vData['ASP.NET_SessionId'];
                                    //                                        } else {
                                    //                                            localStorage.removeItem('ASP.NET_SessionId');
                                    //                                        };
                                    //                                        $('#spanError').hide();
                                    //                                        //                            alert('Sign In Successful!');
                                    //                                        CreateNewEntry(0, vData.PeopleUserAccountData);
                                    //                                    }
                                    //                                    else {
                                    //                                        if (vData.Error == undefined)
                                    //                                            $('#spanError')
                                    //                                            .text('Invalid username and password')
                                    //                                            .show();
                                    //                                        else
                                    //                                            $('#spanError')
                                    //                                            .text(vData.Error)
                                    //                                            .show();
                                    //                                        return;
                                    //                                    };
                                }, // success 
                                error: function () {
                                    //                                    $('#divSignIn').hide();
                                    $('#btnSignIn').removeAttr("disabled");
                                    $('#spanError')
                                        .text("No matching offline account. Can't establish a connection to the server. Please check your internet connection and try again.")
                                        .show();
                                }
                            }); // AjaxSend
                        }
                    } else { // ELSE if (OfflineMode) {
                        if (proj.IsCookieless == 1) {
                            pData.IsCookieless = 1;
                            docCookies.removeItem('ASP.NET_SessionId');
                        };
                        $('#spanError').hide();
                        AjaxSend({
                            url: 'People/CheckPeopleUserAccount',
                            data: pData,
                            // online mode = dont put timeout
                            success: function (vData) {
                                if ($.trim(vData.PeopleUserAccountData) != '') {
                                    if (proj.IsCookieless == 1) {
                                        localStorage['ASP.NET_SessionId'] = vData['ASP.NET_SessionId'];
                                    } else {
                                        localStorage.removeItem('ASP.NET_SessionId');
                                    };
                                    $('#spanError').hide();
                                    //                            alert('Sign In Successful!');
                                    CreateNewEntry(0, vData.PeopleUserAccountData);
                                } else {
                                    //                                    $('#divSignIn').hide();
                                    $('#btnSignIn').removeAttr("disabled");
                                    if (vData.Error == undefined)
                                        $('#spanError')
                                            .text('Invalid username and password')
                                            .show();
                                    else
                                        $('#spanError')
                                            .text(vData.Error)
                                            .show();
                                    return;
                                };
                            } // success
                        }); // AjaxSend
                    };

                    //Added By Francis H. 11/27/2013

                    ////////                    Replace By Francis A. 11/09/2013
                    ////////                    if (OfflineMode) {
                    ////////                        PeopleLogger_offline().CheckUser(pData);
                    ////////                    } else {
                    ////////                        if (proj.IsCookieless == 1) {
                    ////////                            pData.IsCookieless = 1;
                    ////////                            docCookies.removeItem('ASP.NET_SessionId');
                    ////////                        };
                    ////////                        $('#spanError').hide();
                    ////////                        AjaxSend({
                    ////////                            url: 'People/CheckPeopleUserAccount',
                    ////////                            data: pData,
                    ////////                            success: function (vData) {

                    ////////                                if ($.trim(vData.PeopleUserAccountData) != '') {
                    ////////                                    if (proj.IsCookieless == 1) {
                    ////////                                        localStorage['ASP.NET_SessionId'] = vData['ASP.NET_SessionId'];
                    ////////                                    } else {
                    ////////                                        localStorage.removeItem('ASP.NET_SessionId');
                    ////////                                    };
                    ////////                                    $('#spanError').hide();
                    ////////                                    //                            alert('Sign In Successful!');
                    ////////                                    CreateNewEntry(0, vData.PeopleUserAccountData);
                    ////////                                }
                    ////////                                else {
                    ////////                                    if (vData.Error == undefined)
                    ////////                                        $('#spanError')
                    ////////                                        .text('Invalid username and password')
                    ////////                                        .show();
                    ////////                                    else
                    ////////                                        $('#spanError')
                    ////////                                        .text(vData.Error)
                    ////////                                        .show();
                    ////////                                    return;
                    ////////                                };
                    ////////                            } // success
                    ////////                        }); // AjaxSend
                    ////////                    }; // if (InAndroid)

                } // function CheckUser() 

                function TestPrint() {
                    var intFAFCTotalLenght = 40;

                    NewPrintBuilder();

                    var strPrint = '';
                    strPrint = '';
                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim('ABC Co. Mall One Branch'), intFAFCTotalLenght, '', 0, '', 0, 1);
                    //strPrint = strPrint + strTemp;

                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT TIN ' + $.trim('12345678'), intFAFCTotalLenght, '', 0, '', 0, 1);
                    //strPrint = strPrint + strTemp;

                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim(('  Customer : [CASH PAYMENT] ' +
                    //    'Address  : 123, 7th Ave, Agusan  del    ' +
                    //'norte, Afghanistan  "' || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                    //strPrint = strPrint + strTemp;
                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                    strPrint = '';
                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Address', 8, ' ', 1), 7, ': ' + $.trim(('123, 7th ave, agusan del norte, Afghanistan' || '')), intFAFCTotalLenght - 3, '', 0, 0);
                    strPrint = strPrint + strTemp;
                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                    PrintText('');
                    ClearCommandBuffer();

                }
                //        $('#formLogin').submit(function (e) {

                //            var pUsername = $('#txtUsername').val();
                //            var pPassword = $('#txtPassword').val();

                //            e.preventDefault();
                //            if (CStr2(pUsername).length == 0) {
                //                $('#txtUsername').focus();
                //                alert('Please enter your username');
                //                return;
                //            };

                //            if (CStr2(pPassword).length == 0) {
                //                $('#txtPassword').focus();
                //                alert('Please enter your password');
                //                return;
                //            };

                //            if (proj.IsCookieless == 1)
                //                docCookies.removeItem('ASP.NET_SessionId');

                //            AjaxSend({
                //                url: 'SignIn/Login',
                //                data: { Username: pUsername,
                //                    Password: pPassword,
                //                    IsCookieless: proj.IsCookieless
                //                },
                //                success: function (vData) {
                //                    var pPathname;
                //                    var pButtons = [];

                //                    if (vData.Error)
                //                        MessageBox(vData.Error, 'Access Denied');
                //                    else if (vData.Text == 'Ok') {
                //                        localStorage.clear();
                //                        if (proj.IsCookieless == 1) {
                //                            //alert('vData[\'ASP.NET_SessionId\']: ' + vData['ASP.NET_SessionId']);
                //                            localStorage['ASP.NET_SessionId'] = vData['ASP.NET_SessionId'];
                //                        } else {
                //                            localStorage.removeItem('ASP.NET_SessionId');
                //                        };

                //                        if (CDec2(vData.AllowSfa)
                //                            + CDec2(vData.AllowPos)
                //                            + CDec2(vData.AllowSalesReturns) == 1) {

                //                            if (vData.AllowSfa)
                //                                window.location = GetPathFormat('/SalesMenu');
                //                            else if (vData.AllowPos)
                //                                window.location = GetPathFormat('/SalesMenu?mode=pos');
                //                            else if (vData.AllowSalesReturns)
                //                                window.location = GetPathFormat('/SalesReturns');
                //                            MessageBox('Welcome', 'Access Granted');
                //                        } else {
                //                            if (vData.AllowSfa)
                //                                pButtons.push({
                //                                    Text: 'SFA',
                //                                    Click: function () {
                //                                        window.location = GetPathFormat('/SalesMenu');
                //                                    },
                //                                    CloseOnClick: false
                //                                });

                //                            if (vData.AllowPos)
                //                                pButtons.push({
                //                                    Text: 'POS',
                //                                    Click: function () {
                //                                        window.location = GetPathFormat('/SalesMenu?mode=pos');
                //                                    },
                //                                    CloseOnClick: false
                //                                });

                //                            if (vData.AllowSalesReturns)
                //                                pButtons.push({
                //                                    Text: 'Sales Returns',
                //                                    Click: function () {
                //                                        window.location = GetPathFormat('/SalesReturns');
                //                                    },
                //                                    CloseOnClick: false
                //                                });

                //                            MessageBox('Welcome', 'Access Granted', pButtons, {
                //                                OnClose: function () {
                //                                    location.reload();
                //                                }
                //                            });
                //                        };

                //                        //                        function GotoLogin() {
                //                        //                            if ((proj.IsCookieless == 0) 
                //                        //                                || (localStorage['ASP.NET_SessionId'] != undefined)) {

                //                        //                                pPathname = window.location.pathname.toLowerCase();
                //                        //                                if (pPathname == '/'
                //                        //                                    || pPathname == '/login3'
                //                        //                                    || pPathname == '/logout3'
                //                        //                                    || pPathname.indexOf(".html")>=0)

                //                        //                                    window.location = GetPathFormat('SalesMenu');
                //                        //                                else
                //                        //                                    window.location.reload();
                //                        //                            } else {
                //                        //                                setTimeout(GotoLogin, 300);
                //                        //                            };
                //                        //                        }; // function GotoLogin() {
                //                        //                        GotoLogin();

                //                    } else
                //                        MessageBox('Unknown responce from the server', 'Access Denied');
                //                },
                //                error: function (vError) {
                //                    var pDummy;
                //                }
                //            });

                //            return false;

                //        });


            }); //Inner function

        });

    });


/*\
|*|
|*|  :: cookies.js ::
|*|
|*|  A complete cookies reader/writer framework with full unicode support.
|*|
|*|  https://developer.mozilla.org/en-US/docs/DOM/document.cookie
|*|
|*|  Syntaxes:
|*|
|*|  * docCookies.setItem(name, value[, end[, path[, domain[, secure]]]])
|*|  * docCookies.getItem(name)
|*|  * docCookies.removeItem(name[, path])
|*|  * docCookies.hasItem(name)
|*|  * docCookies.keys()
|*|
\*/

var docCookies = {
    getItem: function (sKey) {
        if (!sKey || !this.hasItem(sKey)) { return null; }
        return unescape(document.cookie.replace(new RegExp("(?:^|.*;\\s*)" + escape(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=\\s*((?:[^;](?!;))*[^;]?).*"), "$1"));
    },
    setItem: function (sKey, sValue, vEnd, sPath, sDomain, bSecure) {
        if (!sKey || /^(?:expires|max\-age|path|domain|secure)$/i.test(sKey)) { return; }
        var sExpires = "";
        if (vEnd) {
            switch (vEnd.constructor) {
                case Number:
                    sExpires = vEnd === Infinity ? "; expires=Tue, 19 Jan 2038 03:14:07 GMT" : "; max-age=" + vEnd;
                    break;
                case String:
                    sExpires = "; expires=" + vEnd;
                    break;
                case Date:
                    sExpires = "; expires=" + vEnd.toGMTString();
                    break;
            }
        }
        document.cookie = escape(sKey) + "=" + escape(sValue) + sExpires + (sDomain ? "; domain=" + sDomain : "") + (sPath ? "; path=" + sPath : "") + (bSecure ? "; secure" : "");
    },
    removeItem: function (sKey, sPath) {
        if (!sKey || !this.hasItem(sKey)) { return; }
        document.cookie = escape(sKey) + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT" + (sPath ? "; path=" + sPath : "");
    },
    hasItem: function (sKey) {
        return (new RegExp("(?:^|;\\s*)" + escape(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=")).test(document.cookie);
    },
    keys: /* optional method: you can safely remove it! */function () {
        var aKeys = document.cookie.replace(/((?:^|\s*;)[^\=]+)(?=;|$)|^\s*|\s*(?:\=[^;]*)?(?:\1|$)/g, "").split(/\s*(?:\=[^;]*)?;\s*/);
        for (var nIdx = 0; nIdx < aKeys.length; nIdx++) { aKeys[nIdx] = unescape(aKeys[nIdx]); }
        return aKeys;
    }
}; // docCookies