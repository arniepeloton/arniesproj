require([
    'common'
    ], function () {
        $(function () {
            //             $('html').removeClass('ui-mobile');

            var IsForgotPassword;

            $(document).ready(function () {
                TestCookies();

                AjaxSend({
                    url: 'AccountManagement/IsNewEntry',
                    success: function (vData) {
                        if (vData.IsNew == "FALSE") {
                            if (vData.PeopleID != null && vData.PeopleID != '') {
                               
                                ShowPersonInfo(vData.PeopleID);
                                $("#txtPeopleID").val(vData.PeopleID);
                            }
                        }

                    }
                });
            });

            $('#lnkSignOut').click(function () {
                SignOut();

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


            $('#btnSignIn').click(function (e) {

                e.preventDefault();

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

                docCookies.setItem('TestCookie', 'test');
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        var pAppVersion;

                        proj.IsCookieless = 1;
                        if (vData.Result == 'ok') {
                            if (docCookies.getItem('TestCookie') == 'success')
                                proj.IsCookieless = 0;
                        };

                    }, // success
                    complete: function () {
                        docCookies.removeItem('TestCookie');
                    }
                });

            } // TestCookies

            function SignOut() {

                AjaxSend({
                    url: "People/setSignOut",
                    success: function (vData) {
                        window.open(GetPathFormat("campaignmanager/signin?mode=logout"), "_self"); // window.open(GetPathFormat("/People"), "_self"); EDITED BY: JANINE , Aug 3 4pm
                    }
                });

            } //SignOut

            function ShowPersonInfo(strID) {
                var pData = {};
                pData.ID = strID;

                AjaxSend({
                    url: "SignIn/CheckPeopleIsFirstPartySignUp",
                    data: pData,
                    success: function (vData) {
                        if (vData.PeopleFirstPartySignUpData.Rows[0].IsFirstPartySignUp != '0') {
                            
                            $('#lblName').text(vData.PeopleFirstPartySignUpData.Rows[0].FirstName);

                            $('#menuSignIn').hide();
                            $('#menuSignOut').show();
                            $('#lblMsg').hide();
                        } else {
                            pUrl = '/accountmanagement';
                            window.open(GetPathFormat(pUrl), '_self');

                        }
                    }
                });

            }


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

                                        pUrl = '/campaignmanager';
                                        window.open(GetPathFormat(pUrl), '_self');

                                    }
                                } // success
                            }); // AjaxSend





                        };
                    }
                });

            } //CreateNewEntry

            function CheckUser() {

                var pData = {};

                pData.Username = $('#txtUsername').val();
                pData.UserPassword = $('#txtPassword').val();

                if (IsMobile)
                    pData.Serial = Device.Serial();
                pData.IsMobile = IsMobile;


                if (proj.IsCookieless == 1) {
                    pData.IsCookieless = 1;
                    docCookies.removeItem('ASP.NET_SessionId');
                };
                $('#spanError').hide();
                AjaxSend({
                    url: 'People/CheckPeopleUserAccount',
                    data: pData,
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
                        }
                        else {
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