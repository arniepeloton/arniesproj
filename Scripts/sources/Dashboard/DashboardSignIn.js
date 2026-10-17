require([
    'common'
    ], function () {
        $(function () {

            var IsForgotPassword;
            TestCookies();

            $('#frmPeopleLogger').submit(function (e) {
                e.preventDefault();

                $('#btnSignIn').click();
            });

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



        }); //Inner function


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
                            docCookies.removeItem('TestCookie');
                        }
                    });

                }; // TestCookies

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
                                        window.open(GetPathFormat('/accountmanagement'), '_self');

                                    } else {

                                        pParam = GetUrlParameters();
                                        if (CStr2(pParam.LoginHash).length == 50)
                                            localStorage.LoginHash = pParam.LoginHash;
                                        //pUrl = '/MallLessor?' + $.param({LoginHash:pParam.LoginHash});

                                        pUrl = '/dashboard';
                                        window.open(GetPathFormat(pUrl), '_self');

                                    }
                                } // success
                            }); // AjaxSend





                        };
                    }
                });

            } //CreateNewEntry

            function CheckUser() {
                $('#btnSignIn').hide();
                $('#divSignIn').show();

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
                            $('#divSignIn').hide();
                            $('#btnSignIn').show();
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