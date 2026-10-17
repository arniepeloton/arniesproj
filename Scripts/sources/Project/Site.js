$(function () {
    var menuRight = $('#menuRight')
        .click(function () {
            return false; // prevent the menu from closing when user click inside the menu
        })
        .css('position', 'absolute')
        .hide();

    menuRight.find('a[href=#1]')
        .click(function (e) {
            e.preventDefault();
            $.growlUI('Under construction', 'This feature is not yet implemented');
            return false;
        });

    menuRight.find('a[href!=#1]')
        .click(function (e) {
            e.stopPropagation();
        });

    var lnkRightMenu = $('#lnkRightMenu')
        .click(function (e) {
            e.preventDefault();

            if (menuRight.is(':visible'))
                menuRight.fadeOut();
            else {
                menuRight.fadeIn();
                menuRight.position({
                    my: 'right top',
                    at: 'left top',
                    of: $('#lnkRightMenu'),
                    offset: '-10 0'
                });
                txtUsername.focus();
            };
        });

    $(document).click(function (e) {
        if (e.target != lnkRightMenu[0]) {
            menuRight.fadeOut();
        };
    });

    var lnkLogin = $('#lnkLogin')
        .click(function (e) {
            mLogIn.Show();
            return false;
        });

    //    var divUser = $('#divUser');
    //    if (divUser) {
    //        var divUserMenu = $('#divUserMenu');

    //        divUser.mouseenter(function (e) {
    //            divUserMenu
    //                .stop(true, true)
    //                .fadeIn()
    //                .position({
    //                    my: 'right top',
    //                    at: 'right bottom',
    //                    of: divUser
    //                });
    //        });
    //        divUser.mouseleave(function (e) {
    //            divUserMenu
    //                .stop(true, true)
    //                .fadeOut();
    //        });
    //    };

    $('.divLogType')
        .click(function (e) {
            e.preventDefault();
            mLogIn.Show();
        })
        .mouseenter(function (e) {
            $(this).css({
                '-moz-box-shadow': '0 0 5px orange',
                '-webkit-box-shadow': '0 0 5px orange',
                'box-shadow': '0 0 5px orange'
            });
        })
        .mouseleave(function (e) {
            $(this).css({
                '-moz-box-shadow': '0 0 5px #888',
                '-webkit-box-shadow': '0 0 5px #888',
                'box-shadow': '0 0 5px #888'
            });
        });

    var mLogIn = new LogInClass();

    function LogInClass() {

        var divLogin = $('#divLogin');
        var formLogIn = divLogin.find('[name=formLogIn]');
        var divBlock = formLogIn.find('[name=divBlock]');
        var txtUsername = formLogIn.find('[name=txtUsername]');
        var txtPassword = formLogIn.find('[name=txtPassword]');
        var butLogIn = formLogIn.find('[name=butLogIn]')
            .click(butLogIn_Click);
        var divResult = divLogin.find('[name=divResult]');
        var spanClose = divLogin.find('[name=spanClose]')
            .button()
            .click(function (e) {
                $.unblockUI();
            });

        this.Show = function () {

            divLogin.hide(); // needed to properly center the popup
            $.blockUI({
                message: divLogin,
                //showOverlay: false,
                css: {
                    border: 'none',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    'border-radius': '10px',
                    '-moz-box-shadow': '0 0 12px #888888',
                    '-webkit-box-shadow': '0 0 12px #888888',
                    'box-shadow': '0 0 12px #888888',
                    cursor: 'arrow',
                    width: '520px', // just fit
                    left: Math.max($(window).width() / 2 - divLogin.outerWidth() / 2, 0) + 'px',
                    top: Math.max($(window).height() / 2 - divLogin.outerHeight() / 2, 0) + 'px'
                },
                overlayCSS: {
                    opacity: 0.1    // transparent overlay
                }
            });
            $('.blockOverlay').one('click', $.unblockUI);
        }; // END this.Show

        function butLogIn_Click(e) {
            e.preventDefault();
            if (txtUsername.val().trim().length == 0) {
                txtUsername.focus();
                divResult
                    .text('Please enter your username')
                    .MakeError();
                return;
            };
            if (txtPassword.val().trim().length == 0) {
                divResult
                    .text('Please enter your password')
                    .MakeError();
                txtPassword.focus();
                return;
            };
            divBlock.block();
            AjaxSend({
                url: '/login/verify',
                data: formLogIn.serializeObject(),
                success: function (vData) {
                    divBlock.unblock();
                    if (vData.d.Text) {
                        if (vData.d.Text == 'Ok') {
                            txtUsername.hide();
                            txtPassword.hide();
                            butLogIn.hide();
                            divResult.html('Welcome.').MakeHighlight();
                            if (window.location.pathname == '/'
                                || window.location.pathname == '/logout')

                                window.location = '/welcome';
                            else
                                window.location.reload();
                        } else
                            divResult.text(vData.d.Text).MakeHighlight();
                    } else if (vData.d.Error) {
                        txtUsername.focus();
                        divResult.text(vData.d.Error).MakeError();
                    } else if (vData.d.BankUserAccess) {
                        window.location = '/OBDR/';
                    };
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    divBlock.unblock();
                    divResult.text(XMLHttpRequest.responseText).MakeError();
                }
            });
        };

    }; // END LogInClass

    // left navigation
    (function () {

        var pUrl = window.location.pathname;

        if (HandleMatch('/welcome', 'Dashboard')) return;
        if (HandleMatch('/drp', 'DRP')) return;
        if (HandleMatch('/reports', 'Reports')) return;

        function HandleMatch(vUrl, vText) {

            var pRegExp = new RegExp('^\\W*' + vUrl + '((\\W+.*)|\\W*)$', 'i');

            if (pUrl.match(pRegExp)) {
                $('#nav').find('span:contains(' + vText + ')').closest('a').addClass('active');
                return true;
            };

        }; // END HandleMatch

    })(); // END left navigation

    // DRP tabs
    (function () {

        var pUrl = window.location.pathname;
        var pDiv = $('#divDrpTab');
        var pMatches;
        var pHref;
        var pLnk;

        if (pDiv.length == 0) return;
        pMatches = /^\/drp(?:$|\/$|\/(\w+))/ig.exec(pUrl);
        if (pMatches == null) return;
        if (pMatches[1] == null
            || pMatches[1].toLowerCase() == 'index')

            pHref = '';
        else
            pHref = '\\/' + pMatches[1];
        pHref = '\\/drp' + pHref;

        pHref = 'a:[href=' + pHref + ']';
        pLnk = pDiv.find(pHref);
        if (pLnk.length > 0) pLnk.parent().addClass('ui-tabs-selected ui-state-active');

    })(); // END DRP tabs
});