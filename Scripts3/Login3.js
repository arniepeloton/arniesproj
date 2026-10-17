require([
    'common'
    ], function() {

    $(function() {

        (function Init() {

            var pParams;
            try {
                // error if cookies are disabled
                localStorage.removeItem('ASP.NET_SessionId');
                localStorage.removeItem('CustomerAddressID');
            } catch (e) {
            };
            pParams = GetUrlParameters();
            $('#divNoCookies').hide();
            $('#formLogin').hide();
            TestCookies();
            if (pParams.mode == 'logout') {
                
            };

        })();

        function TestCookies() {

            try {
                if (localStorage == undefined) {
                    $('#divNoCookies').show();
                    return;
                };
                $('#formLogin').show();
            } catch (e) {
                $('#divNoCookies').show();
                return;
            };

            docCookies.setItem('TestCookie', 'test');
            AjaxSend({
                url: 'Login3/TestCookies',
                success: function(vData) {
                    if ((vData.Result == 'ok')
                        && (docCookies.getItem('TestCookie') == 'success')) {

                        // success
                        proj.IsCookieless = 0;
                    } else {
                        //$('#divNoCookies').show();
                        proj.IsCookieless = 1;
                    };

                },
                error: function (vError) {
                    MessageBox('Unable to connect to server.', null, [
                        {Text: 'Try again'}
                    ], {
                        OnClose: TestCookies
                    });
                }
            });

        };

        $('#formLogin').submit(function(e) {

            var pUsername = $('#txtUsername').val();
            var pPassword = $('#txtPassword').val();

            e.preventDefault();
            if (CStr2(pUsername).length == 0) {
                $('#txtUsername').focus();
                MessageBox('Please enter your username', 'No Username');
                return;
            };

            if (CStr2(pPassword).length == 0) {
                $('#txtPassword').focus();
                MessageBox('Please enter your password', 'No Password');
                return;
            };

            if (proj.IsCookieless == 1)
                docCookies.removeItem('ASP.NET_SessionId');

            AjaxSend({
                url: 'Login3/Login',
                data: {Username: pUsername,
                    Password: pPassword,
                    IsCookieless: proj.IsCookieless},
                success: function (vData) {
                    var pPathname;
                    var pButtons = [];

                    if (vData.Error) 
                        MessageBox(vData.Error, 'Access Denied');
                    else if (vData.Text == 'Ok') {
                        localStorage.clear();
                        if (proj.IsCookieless == 1) {
                            //alert('vData[\'ASP.NET_SessionId\']: ' + vData['ASP.NET_SessionId']);
                            localStorage['ASP.NET_SessionId'] = vData['ASP.NET_SessionId'];
                        } else {
                            localStorage.removeItem('ASP.NET_SessionId');
                        };

                        if (CDec2(vData.AllowSfa)
                            + CDec2(vData.AllowPos) 
                            + CDec2(vData.AllowSalesReturns) == 1) {

                            if (vData.AllowSfa)
                                window.location = GetPathFormat('/SalesMenu');
                            else if (vData.AllowPos)
                                window.location = GetPathFormat('/SalesMenu?mode=pos');
                            else if (vData.AllowSalesReturns)
                                window.location = GetPathFormat('/SalesReturns');
                            MessageBox('Welcome', 'Access Granted');
                        } else {
                            if (vData.AllowSfa)
                                pButtons.push({
                                    Text: 'SFA',
                                    Click: function() {
                                        window.location = GetPathFormat('/SalesMenu');
                                    },
                                    CloseOnClick: false
                                });

                            if (vData.AllowPos)
                                pButtons.push({
                                    Text: 'POS',
                                    Click: function() {
                                        window.location = GetPathFormat('/SalesMenu?mode=pos');
                                    },
                                    CloseOnClick: false
                                });

                            if (vData.AllowSalesReturns)
                                pButtons.push({
                                    Text: 'Sales Returns',
                                    Click: function() {
                                        window.location = GetPathFormat('/SalesReturns');
                                    },
                                    CloseOnClick: false
                                });

                            MessageBox('Welcome', 'Access Granted', pButtons, {
                                OnClose: function(){
                                    location.reload();
                                }
                            });
                        };

//                        function GotoLogin() {
//                            if ((proj.IsCookieless == 0) 
//                                || (localStorage['ASP.NET_SessionId'] != undefined)) {

//                                pPathname = window.location.pathname.toLowerCase();
//                                if (pPathname == '/'
//                                    || pPathname == '/login3'
//                                    || pPathname == '/logout3'
//                                    || pPathname.indexOf(".html")>=0)

//                                    window.location = GetPathFormat('SalesMenu');
//                                else
//                                    window.location.reload();
//                            } else {
//                                setTimeout(GotoLogin, 300);
//                            };
//                        }; // function GotoLogin() {
//                        GotoLogin();

                    } else 
                        MessageBox('Unknown responce from the server', 'Access Denied');
                },
                error: function (vError) {
                    var pDummy;
                }
            });

            return false;

        });

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
  keys: /* optional method: you can safely remove it! */ function () {
    var aKeys = document.cookie.replace(/((?:^|\s*;)[^\=]+)(?=;|$)|^\s*|\s*(?:\=[^;]*)?(?:\1|$)/g, "").split(/\s*(?:\=[^;]*)?;\s*/);
    for (var nIdx = 0; nIdx < aKeys.length; nIdx++) { aKeys[nIdx] = unescape(aKeys[nIdx]); }
    return aKeys;
  }
};