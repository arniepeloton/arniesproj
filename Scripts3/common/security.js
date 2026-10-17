define(['jquery',
    'common',
    '/Scripts3/Offline/Web.js?crc=',
    GetPathFormat('/Scripts3/common/common.js?v=' + assetversion)
    ], function () {

        window.PromptUserAccess = function (strModuleName, vParams) {
            if (typeof Bs != 'undefined') {
                //\Content\bootstrap\js\bootstrap_modal.js
                Bs.PromptUser(strModuleName, vParams)
                return;
            }

            vParams = $.extend({}, vParams);

            var vDataAccess;
            var pData = $.extend({}, vParams.OtherData);
            pData.ModuleName = strModuleName;
            var pHasAccess;

            if (sessionStorage["OrgID"] != undefined) {
                pData.OrgID = sessionStorage["OrgID"];
            }

            if (OfflineMode) {

                var IsExists;

                IsExists = Transfers.ReadFile(sessionStorage.OrgID + '.flg')
                if (IsExists == undefined) {
                    window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                    return;
                }

                pData.UserID = Web.PeopleUserID();
                pData.IncludeNoAccess = CBool(vParams.IncludeNoAccess);

                Web.SetOrgID(sessionStorage["OrgID"]);
                Web.HandleOrganization();
                vDataAccess = security_offline().CheckSecurityCurrentUserAcccess(pData);

                if (vDataAccess.Rows.length > 0) {
                    if (pData.IncludeNoAccess) {
                        for (var p = 0; p <= vDataAccess.Rows.length - 1; p++) {
                            var pDR = vDataAccess.Rows[p];
                            if (CBool(pDR.IsHasAccess)) {
                                pHasAccess = true;
                                break;
                            }
                        }
                    } else
                        pHasAccess = true;
                } else if (pData.IncludeNoAccess)
                    pHasAccess = true;

                if (pHasAccess) {
                    if ($.isFunction(vParams.OnClickOKSuccess)) {
                        vParams.OnClickOKSuccess(vDataAccess);
                    }
                } else {

                    var pItem;
                    var pButton;

                    var pPage = $('<div>');
                    pPage.attr('data-role', 'dialog');
                    pPage.attr('id', 'randomSecID');

                    var pHeader = $('<div>');
                    pHeader.attr('data-role', 'header')
                    .append($('<h1>').text("Security Access"));
                    pPage.append(pHeader);

                    var pContent = $('<div data-role="content">');

                    //Message
                    if (vParams.Message != undefined) {
                        pContent.append($('<div><legend>' + vParams.Message + '.</legend></div>'));
                    } else {
                        pContent.append($('<div><legend>You don’t have access to ' + strModuleName + '.</legend></div>'));
                    }
                    
                    pContent.append($('<br/>'));
                    pContent.append($('<div><legend>Please enter a valid user name and password to gain access to this module or feature.</legend></div>'));
                    pContent.append($('<br/>'));

                    //Username 
                    pItem = $('<div data-role="fieldcontain">')
                    .append($('<label style = "font-weight:normal;">')
                        .attr('for', "txtSecurityUserName")
                        .text('Username :')
                    )
                    .append($('<fieldset data-role="controlgroup">')
                        .append($('<input>')
                            .attr('id', "txtSecurityUserName")
                            .attr('type', "text")
                        )
                    );
                    pItem = $('<form id="frmSecurityUserName">')
                        .submit(function (e) {
                            e.preventDefault();

                            $('#btnSecurityOk').click();
                        })
                        .append(pItem);

                    pContent.append(pItem);

                    //Password 
                    pItem = $('<div data-role="fieldcontain">')
                    .append($('<label style = "font-weight:normal;">')
                        .attr('for', "txtSecurityUserPassword")
                        .text('Password :')
                    )
                    .append($('<fieldset data-role="controlgroup">')
                        .append($('<input>')
                            .attr('id', "txtSecurityUserPassword")
                            .attr('type', "password")
                        )
                    );
                    pItem = $('<form id="frmSecurityPassword">')
                        .submit(function (e) {
                            e.preventDefault();

                            $('#btnSecurityOk').click();
                        })
                        .append(pItem);

                    pContent.append(pItem);

                    //Button OK
                    pButton = $('<button id="btnSecurityOk">');
                    pButton.text("Ok");
                    pButton.click(function (e) {

                        //Check if has access
                        pData = {};
                        pData.ModuleName = strModuleName;
                        pData.Username = $("#txtSecurityUserName").val();
                        pData.UserPassword = $("#txtSecurityUserPassword").val();
                        pData.UserID = security_offline().CheckSecurityUserAcccess(pData);

                        if (sessionStorage["OrgID"] != undefined) {
                            pData.OrgID = sessionStorage["OrgID"];
                        }

                        vDataAccess = security_offline().CheckSecurityCurrentUserAcccess(pData);

                        if (vDataAccess.Rows.length > 0) {
                            if ($.isFunction(vParams.OnClickOKSuccess)) {
                                $('#randomSecID').dialog('close');
                                vParams.OnClickOKSuccess(vDataAccess);
                            }
                        } else {
                            MessageBox("Invalid username and password.",
                                    "Username and Password",
                                    [
                                        { Text: 'Ok',
                                            Click: function () {
                                                PromptUserAccess(strModuleName, vParams)
                                            }
                                        }
                                    ]
                            );
                        }

                    });
                    pContent.append(pButton);

                    //Button Cancel
                    pButton = $('<button>');
                    pButton.text("Cancel");
                    pButton.click(function (e) {
                        if (window.pageSecuritySourcePage != undefined) {
                            $.mobile.changePage(window.pageSecuritySourcePage);
                        } else {
                            history.back();
                        }

                        if ($.isFunction(vParams.OnFailed)) 
                            vParams.OnFailed();
                    });
                    pContent.append(pButton);

                    //Message
                    pContent.append($('<br/>'));
                    pContent.append($('<div><legend>If you believe that you need access to this ' + strModuleName + ', please contact your “Super Administrator”</legend></div>'));
                    pContent.append($('<br/>'));

                    //Button Request Access
                    pButton = $('<button>');
                    pButton.text("Request for Access");
                    pButton.click(function (e) {
                        MessageBox("Not yet Implemented.",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        PromptUserAccess(strModuleName, vParams)
                                    }
                                }
                            ]
                        );
                    });
                    pContent.append(pButton);

                    //Button Request Access
                    pButton = $('<button>');
                    pButton.text("Don't Request for Access");
                    pButton.click(function (e) {
                        //if (window.pageSecuritySourcePage != undefined) {
                        //    $.mobile.changePage(window.pageSecuritySourcePage);
                        //    if ($.isFunction(vParams.OnFailed)) {
                        //        vParams.OnFailed();
                        //    }
                        //} else {
                        //    history.back();
                        //}

                        if (window.pageSecuritySourcePage != undefined) {
                            $.mobile.changePage(window.pageSecuritySourcePage);
                        } else {
                            history.back();
                        }

                        if ($.isFunction(vParams.OnFailed)) 
                            vParams.OnFailed();
                    });
                    pContent.append(pButton);

                    pPage.append(pContent);

                    $('body').append(pPage);

                    pPage.bind('pagehide', function (e) {
                        pPage.remove();
                    });
                    $.mobile.changePage(pPage);
                    setTimeout(function () { $('#txtSecurityUserName').focus(); }) // kung walay settimeout mogawas ang "Invalid Username and Password" popup;
                    return pPage;
                }

            } else {

                if (!vParams.PromptOnly) {
                    AjaxSend({
                        url: "SecurityRole/CheckSecurityCurrentUserAcccess",
                        data: pData,
                        success: function (vData) {

                            if (vData.SecurityRoleData.Rows.length > 0) {
                                if ($.isFunction(vParams.OnClickOKSuccess)) {
                                    vParams.OnClickOKSuccess(vData.SecurityRoleData);
                                }
                            } else {
                                ShowDialog();
                            }
                        } // END success
                    }); // END AjaxSend
                }

                if (vParams.PromptOnly)
                    ShowDialog();

                function ShowDialog() {
                            
                    var pItem;
                    var pButton;

                    var pPage = $('<div>');
                    pPage.attr('data-role', 'dialog');
                    pPage.attr('id', 'randomSecID');

                    var pHeader = $('<div>');
                    pHeader.attr('data-role', 'header')
                    .append($('<h1>').text("Security Access"));
                    pPage.append(pHeader);

                    var pContent = $('<div data-role="content">');

                    //Message
                    pContent.append($('<div><legend>You don’t have access to ' + strModuleName + '.</legend></div>'));
                    pContent.append($('<br/>'));
                    pContent.append($('<div><legend>Please enter a valid user name and password to gain access to this module or feature.</legend></div>'));
                    pContent.append($('<br/>'));

                    //Username 
                    pItem = $('<div data-role="fieldcontain">')
                    .append($('<label style = "font-weight:normal;">')
                        .attr('for', "txtSecurityUserName")
                        .text('Username :')
                    )
                    .append($('<fieldset data-role="controlgroup">')
                        .append($('<input>')
                            .attr('id', "txtSecurityUserName")
                            .attr('type', "text")
                        )
                    );
                    pItem = $('<form id="frmSecurityUserName">')
                        .submit(function (e) {
                            e.preventDefault();

                            $('#btnSecurityOk').click();
                        })
                        .append(pItem);

                    pContent.append(pItem);

                    //Password 
                    pItem = $('<div data-role="fieldcontain">')
                    .append($('<label style = "font-weight:normal;">')
                        .attr('for', "txtSecurityUserPassword")
                        .text('Password :')
                    )
                    .append($('<fieldset data-role="controlgroup">')
                        .append($('<input>')
                            .attr('id', "txtSecurityUserPassword")
                            .attr('type', "password")
                        )
                    );
                    pItem = $('<form id="frmSecurityPassword">')
                        .submit(function (e) {
                            e.preventDefault();

                            $('#btnSecurityOk').click();
                        })
                        .append(pItem);

                    pContent.append(pItem);

                    //Button OK
                    pButton = $('<button id="btnSecurityOk">');
                    pButton.text("Ok");
                    pButton.click(function (e) {
                        //Check if has access
                        var pData = {};
                        pData.ModuleName = strModuleName;
                        pData.Username = $("#txtSecurityUserName").val();
                        pData.UserPassword = $("#txtSecurityUserPassword").val();

                        if (sessionStorage["OrgID"] != undefined) {
                            pData.OrgID = sessionStorage["OrgID"];
                        }

                        if (vParams.PromptOnly) {
                            if ($.isFunction(vParams.PromptCallback))
                                vParams.PromptCallback({
                                    Username: pData.Username,
                                    Password: pData.UserPassword
                                });
                        } else {
                            AjaxSend({
                                url: "SecurityRole/CheckSecurityUserAcccess",
                                data: pData,
                                success: function (vDataUserAcess) {

                                    if (vDataUserAcess.SecurityRoleData.Rows.length > 0) {
                                        if ($.isFunction(vParams.OnClickOKSuccess)) {
                                            $('#randomSecID').dialog('close');
                                            vParams.OnClickOKSuccess(vDataUserAcess.SecurityRoleData);
                                        }
                                    } else {
                                        MessageBox("Invalid username and password.",
                                            "Username and Password",
                                            [
                                                { Text: 'Ok',
                                                    Click: function () {
                                                        PromptUserAccess(strModuleName, vParams)
                                                    }
                                                }
                                            ]
                                        );
                                    }
                                }
                            }); // END AjaxSend
                        }

                    }); // END pButton.click(function (e) {
                    pContent.append(pButton);

                    //Button Cancel
                    pButton = $('<button>');
                    pButton.text("Cancel");
                    pButton.click(function (e) {
                        //if (window.pageSecuritySourcePage != undefined) {
                        //    $.mobile.changePage(window.pageSecuritySourcePage);
                        //    if ($.isFunction(vParams.OnFailed)) {
                        //        vParams.OnFailed();
                        //    }
                        //} else {
                        //    history.back();
                        //}

                        if (window.pageSecuritySourcePage != undefined) {
                            $.mobile.changePage(window.pageSecuritySourcePage);
                        } else {
                            history.back();
                        }

                        if ($.isFunction(vParams.OnFailed)) 
                            setTimeout(function() {
                                vParams.OnFailed();
                            }, 500);
                    });
                    pContent.append(pButton);

                    //Message
                    pContent.append($('<br/>'));
                    pContent.append($('<div><legend>If you believe that you need access to this ' + strModuleName + ', please contact your “Super Administrator”</legend></div>'));
                    pContent.append($('<br/>'));

                    //Button Request Access
                    pButton = $('<button>');
                    pButton.text("Request for Access");
                    pButton.click(function (e) {
                        MessageBox("Not yet Implemented.",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        PromptUserAccess(strModuleName, vParams)
                                    }
                                }
                            ]
                        );
                    });
                    pContent.append(pButton);

                    //Button Request Access
                    pButton = $('<button>');
                    pButton.text("Don't Request for Access");
                    pButton.click(function (e) {
                        if (window.pageSecuritySourcePage != undefined) {
                            $.mobile.changePage(window.pageSecuritySourcePage);
                            if ($.isFunction(vParams.OnFailed)) {
                                vParams.OnFailed();
                            }
                        } else {
                            history.back();
                        }
                    });
                    pContent.append(pButton);

                    pPage.append(pContent);

                    $('body').append(pPage);

                    pPage.bind('pagehide', function (e) {
                        pPage.remove();
                    });
                    $.mobile.changePage(pPage);

                    return pPage;
                } // END function ShowDialog() {

            }; // END if

        }; //PromptUserAccess

    });                   // require

