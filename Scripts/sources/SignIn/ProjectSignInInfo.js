require([
    'common',
    (OfflineMode ? '/Scripts/sources/ImportExportData/ImportExportData_Offline.js?crc=' : '')
    ], function () {

        require([(OfflineMode ? '/Scripts/sources/People/PeopleLogger_offline.js?crc=' : '')], function () {
            $(function () {
                //             $('html').removeClass('ui-mobile');

                var IsForgotPassword;
                var isIE = false || !!document.documentMode;

                $(document).ready(function () {
                    $('.ui-loader').hide();
                    if (OfflineMode) {
                        Device.CheckServerDate();
                        var IsTraxOnly = null;

                        IsTraxOnly = Transfers.ReadFile('Trax.flg');

                        if (IsTraxOnly != undefined) {
                            location = MergeUrlParameters({
                                IsMobile: CBool(IsMobile) ? 1 : ''
                            }, '/Signin/TraxInfo');
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
                            if (vData.Result == 'ok') {
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


                                            if (pParam.moduletype != undefined) {
                                                if (pParam.moduletype == 'onlinestore') {
                                                    ShowPeopleMsgList();
                                                } else {
                                                    ShowPeopleMsgList();
                                                }
                                            } else {
                                                ShowPeopleMsgList();
                                            }
                                        }
                                    } // success
                                }); // AjaxSend
                            };
                        }
                    });

                } //CreateNewEntry

                function ShowPeopleMsgList() {
                    var intLimitMsg = 2;
                    var pData;

                    if (window.IsMobile)
                        pData = {
                            PeopleUserID: Web.PeopleUserID()
                        };
                    AjaxSend({
                        url: "People/getPeopleMessageList",
                        NoSplash: true,
                        data: pData,
                        success: function (vDataMsg) {
                            if (OfflineMode)
                                PeopleMenu_offline().SaveOfflineMessages(vDataMsg);
                            ShowMessage(vDataMsg);
                        },
                        error: function () {
                            try {
                                var pData = PeopleMenu_offline().LoadOfflineMessages();
                                ShowMessage(pData);
                            } catch (e) {
                                // just ignore any errors here
                                console.trace(e.stack);
                            }
                        }
                    }); // END AjaxSend

                    function ShowMessage(vDataMsg) {

                        var pLV = $('#lstMessage');
                        var pItem;

                        pLV.html('');
                        if (vDataMsg.PeopleData.Rows.length > 0) {
                            for (intCntr = 0; intCntr < (vDataMsg.PeopleData.Rows.length > intLimitMsg ? intLimitMsg : vDataMsg.PeopleData.Rows.length); intCntr++) {
                                if (vDataMsg.PeopleData.Rows[intCntr].MessageType == 1) {
                                    ShowPeopleMsgT1(vDataMsg.PeopleData.Rows[intCntr]);
                                    return false;
                                }
                            }

                            GetPeopleOrg();
                        } else {
                            pLV.html('<div style="margin:12px">No messages</div>');
                        }

                    } // END ShowMessage
                }

                function ShowPeopleMsgT1(vMsgData) {
                    var pData = {};
                    pData.MsgID = vMsgData.ID;

                    //Get Messages Details
                    AjaxSend({
                        url: "People/getPeopleMessageT1Details",
                        data: pData,
                        success: function (vDataMsgDetails) {

                            if (vDataMsgDetails.PeopleData.Rows.length > 0) {

                                sessionStorage["Message"] = JSON.stringify(vDataMsgDetails.PeopleData.Rows[0]);
                                window.location.href = '/bomprojectexcel/employmentconfirmation';

                            }
                            else {
                                GetPeopleOrg();
                            }
                        },
                        error: function () {
                            proj.PromptRequiresInternet();
                        }
                    });

                }

                function GetPeopleOrg() {

                    $('#divOrgApps').html('');

                    if (OfflineMode) {
                        vData = PeopleMenu_offline().getPeopleOrganizationList();
                        DisplayPeopleMenu(vData, true); //Load First Org with DBase
                        DisplayPeopleMenu(vData, false);
                    }
                    else {
                        AjaxSend({
                            url: "People/getPeopleOrganizationList",
                            success: function (vDataOrg) {
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
                                                    IsMobile: CBool(IsMobile) ? 1 : '0'
                                                }, '/SetupChecklist/BOMProjectExcel');
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
                                        IsMobile: CBool(IsMobile) ? 1 : '0'
                                    }, '/SetupChecklist/BOMProjectExcel');
                                }
                                else {
                                    alert('Please ask your system administrator to add you as member on the organization');
                                }
                            }
                        });
                    }
                }

                function CheckUser() {
                    $('#btnSignIn').attr("disabled", "disabled");
                    //                    $('#divSignIn').show();


                    var pData = {};

                    pData.Username = $('#txtUsername').val();
                    pData.UserPassword = $('#txtPassword').val();

                    if (IsMobile) {
                        pData.Serial = Device.Serial();
                    };
                    pData.IsMobile = IsMobile;

                    if (OfflineMode) {
                        if (!PeopleLogger_offline().CheckUser(pData)) {
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