require([
    'common',
    '/Scripts3/Offline/Web.js?crc=',
    GetPathFormat('/Scripts3/common/common.js?v=' + assetversion)
    ], function () {
        var isCreatePopUp = false;
        var isCustomeCreatePopUp = false;
        var cssOverLay = {
            background: '#000',
            bottom: '0',
            left: '0',
            position: 'fixed',
            right: '0',
            top: '0',
            zIndex: '-1',
            opacity: '0.4'
        }
        var page = $('body');
        var popup = $('<div id="popup" data-role="popup" data-theme="a" data-dismissible="false" data-corners="false" style="visibility:hidden; display:none;">' +
                        '<center><h3 style="margin-left:10px; margin-right:10px;">Security Access</h3></center>' +
                        '</div>')
                , content = $('<div data-role="content" class="ui-content ui-body-d" style="max-width:350px; word-wrap: break-word;"> ' +
                                '<form method="post" id="formNewPromptUser">' +
                                '<span id="msgNewPromptUser">You don\'t have access to <span id="SecurityAccessTitle" style="font-weight:bold; text-transform:capitalize;"></span>.</span>' +
                                '<br> <br> Please enter a valid username and password to gain access to this module or feature.' +
                                '<span id="spanPromptUserError" style="color:#F00; display:none;"><br><br>Invalid Username and Password</span>' +
                                '<br><br> Username: <br> <input type="text" id="txtPromptUserUsername">' +
                                'Password: <br> <input type="password" id="txtPromptUserPassword">' +
                                '<br><br>' +                                
                                '<button type="submit" data-role="none" class="btn custom-btn-theme-b">Ok</button>' +
                                '<br><br>' +
                                '<a id="btnPromptUserCancel" class="btn custom-btn-theme-c">Cancel</a>' +
                                '</form>' +
                        '</div>').appendTo(popup);

        var custompromptuserpopup = $('<div id="custompromptuserpopup" data-role="popup" data-theme="a" data-dismissible="false" data-corners="false" style="visibility:hidden; display:none;">' +
                        '<center><h3 style="margin-left:10px; margin-right:10px;">Security Access</h3></center>' +
                        '</div>').appendTo(page)
                , customcontent = $('<div data-role="content" class="ui-content ui-body-d" style="max-width:350px; word-wrap: break-word;"> ' +
                                '<form method="post" id="CustomformNewPromptUser">' +
                                '<span id="CustomSecurityAccessTitle" style="font-weight:bold;"></span>' +
                                '<br> <br> Please enter a valid username and password to gain access to this module or feature.' +
                                '<span id="CustomspanPromptUserError" style="color:#F00; display:none;"><br><br>Invalid Username and Password</span>' +
                                '<br><br> Username: <br> <input type="text" id="txtCustomPromptUserUsername">' +
                                'Password: <br> <input type="password" id="txtCustomPromptUserPassword">' +
                                '<br><br>' +
                                '<button type="submit" data-role="none" class="btn custom-btn-theme-b">Ok</button>' +
                                '<br><br>' +
                                '<a id="btnCustomPromptUserCancel" class="btn custom-btn-theme-c">Cancel</a>' +
                                '</form>' +
                        '</div>').appendTo(custompromptuserpopup);

        if (typeof Bs == 'undefined') {
            //\Content\bootstrap\js\bootstrap_modal.js
            popup.appendTo(page);

            try {
                popup.trigger('create');
            } catch (e) {
                // do nothing
            }

            page.on('click', '#btnCloseNewPromptUser, #Custom', function () {
                $('.popupCustomMsgBoxOverlayTheme').remove();
                $('#popup').css({ visibility: 'hidden', display: 'none' }).popup('close');
            })
        }        

        window.NewPromptUserAccess = function (strModuleName, vParams) {
            if (typeof Bs != 'undefined') {
                //\Content\bootstrap\js\bootstrap_modal.js
                Bs.PromptUser(strModuleName, vParams)
                return;
            }

            if (isCreatePopUp == false) {
                popup.popup();
                $('#txtPromptUserUsername, #txtPromptUserPassword').textinput()
                isCreatePopUp = true;
            }

            $('.ui-popup').popup('close'); //close current popup
            $("#txtPromptUserUsername").val('');
            $("#txtPromptUserPassword").val('');

            vParams = $.extend({
                ForcePrompt: false
            },vParams);

            var vDataAccess;
            var pData = {};
            pData.ModuleName = strModuleName;

            if (sessionStorage["OrgID"] != undefined) {
                pData.OrgID = sessionStorage["OrgID"];
            }            

            $('#btnPromptUserCancel').unbind('click').click(function () {
                $('.popupCustomMsgBoxOverlayTheme').remove();
                $('#popup').css({ visibility: 'hidden', display: 'none' }).popup('close');
                NewMessageBox('You have just closed a window telling you were denied access to a feature. You might see a blank page instead of the page you expected. This is not an error. If you are supposed to have access to the aforementioned feature, please notify your supervisor.', 'Access Denied');
                if (vParams.OnCancel)
                    vParams.OnCancel();
            })

            $('#formNewPromptUser').unbind('submit').submit(function (e) {
                e.preventDefault();
                //Check if has access
                pData = {};
                pData.ModuleName = strModuleName;
                pData.Username = $("#txtPromptUserUsername").val();
                pData.UserPassword = $("#txtPromptUserPassword").val();                

                if (sessionStorage["OrgID"] != undefined) {
                    pData.OrgID = sessionStorage["OrgID"];
                }

                if (OfflineMode) {
                    pData.UserID = security_offline().CheckSecurityUserAcccess(pData);
                    vDataAccess = security_offline().CheckSecurityCurrentUserAcccess(pData);
                    HasAccess(vDataAccess.Rows);
                } else {
                    AjaxSend({
                        url: "SecurityRole/CheckSecurityUserAcccess",
                        data: pData,
                        success: function (vDataUserAcess) {
                            HasAccess(vDataUserAcess.SecurityRoleData.Rows);
                        }
                    });
                }
            })

            if (CStr2(vParams.Message) != '') {
                $('#msgNewPromptUser').html(vParams.Message);
            } else {
                $('#msgNewPromptUser').html('You don\'t have access <b>' + strModuleName + '</b>');
            }

            if (OfflineMode) {
                var IsExists;

                IsExists = Transfers.ReadFile(sessionStorage.OrgID + '.flg')
                if (IsExists == undefined) {
                    window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                    return;
                }

                pData.UserID = Web.PeopleUserID();

                Web.SetOrgID(sessionStorage["OrgID"]);
                Web.HandleOrganization();
                vDataAccess = security_offline().CheckSecurityCurrentUserAcccess(pData);

                if (vDataAccess.Rows.length > 0) {
                    if ($.isFunction(vParams.OnClickOKSuccess)) {
                        vParams.OnClickOKSuccess();
                    }
                } else {
                    popup.append($('<div>').addClass('popupCustomMsgBoxOverlayTheme').css(cssOverLay));
                    $('#popup').css({ visibility: 'visible', display: 'block' }).popup('open');

                    $('#SecurityAccessTitle').text(window.GetNewModuleName(strModuleName));
                }
            }
            else if (vParams.ForcePrompt) {
                ShowPopup();
            } else {
                AjaxSend({
                    url: "SecurityRole/CheckSecurityCurrentUserAcccess",
                    data: pData,
                    success: function (vData) {

                        if (vData.SecurityRoleData.Rows.length > 0) {
                            if ($.isFunction(vParams.OnClickOKSuccess)) {
                                vParams.OnClickOKSuccess();
                            }
                        } else {
                            ShowPopup();
                        }
                    }
                });
            }

            function ShowPopup() {
                popup.append($('<div>').addClass('popupCustomMsgBoxOverlayTheme').css(cssOverLay));
                $('#popup').css({ visibility: 'visible', display: 'block' }).popup().popup('open');

                $('#SecurityAccessTitle').text(strModuleName);
            }

            function HasAccess(vDataAccess) {
                if (vDataAccess.length > 0) {
                    if ($.isFunction(vParams.OnClickOKSuccess)) {
                        $('.popupCustomMsgBoxOverlayTheme').remove();
                        $('#popup').css({ visibility: 'hidden', display: 'none' }).popup('close');
                        vParams.OnClickOKSuccess();
                        $('#spanPromptUserError').hide();
                        $("#txtPromptUserUsername, #txtPromptUserPassword").val('');
                    }
                } else {
                    $('#spanPromptUserError').show();
                    $("#txtPromptUserUsername, #txtPromptUserPassword").val('');
                }
            }
        }        

        window.GetNewModuleName = function(vModuleName) {

            var pModuleName = CStr2(vModuleName).trim().toLowerCase();

            if (pModuleName == 'Payments (EAS)'.toLowerCase())
                return 'Customer Payments';
            else if (pModuleName == 'Deactivate Customer Financial Details'.toLowerCase())
                return 'Deactivate Customer\'s Checking Account';
            else if (pModuleName == 'Adjustment (EAS)'.toLowerCase())
                return 'Make Adjustments to Customer\'s Open Invoice(s)';
            else if (pModuleName == 'Adjustment to Invoices With Payments (EAS)'.toLowerCase())
                return 'Make Adjustments to Customer\'s Partially/ Fully Paid Invoice(s)';
            else if (pModuleName == ''.toLowerCase())
                return '';
            else
                return vModuleName;

        };

        window.CustomPromptUserAccess = function (strModuleName, vParams) {

            if (isCustomeCreatePopUp == false) {
                custompromptuserpopup.popup();
                $('#txtCustomPromptUserUsername, #txtCustomPromptUserPassword').textinput()
                isCustomeCreatePopUp = true;
            }

            $('.ui-popup').popup('close'); //close current popup
            $("#txtCustomPromptUserUsername").val('');
            $("#txtCustomPromptUserPassword").val('');

            custompromptuserpopup.append($('<div>').addClass('popupCustomMsgBoxOverlayTheme').css(cssOverLay));
            $('#custompromptuserpopup').css({ visibility: 'visible', display: 'block' }).popup('open');

            $('#CustomSecurityAccessTitle').text(strModuleName);

            vParams = $.extend({
                ForcePrompt: false
            }, vParams);

            var vDataAccess;
            var pData = {};
            pData.ModuleName = strModuleName;

            if (sessionStorage["OrgID"] != undefined) {
                pData.OrgID = sessionStorage["OrgID"];
            }

            $('#btnCustomPromptUserCancel').unbind('click').click(function () {
                $('.popupCustomMsgBoxOverlayTheme').remove();
                $('#custompromptuserpopup').css({ visibility: 'hidden', display: 'none' }).popup('close');
            })

            $('#CustomformNewPromptUser').unbind('submit').submit(function (e) {
                e.preventDefault();
                //Check if has access
                pData = {};
                pData.ModuleName = strModuleName;
                pData.Username = $("#txtCustomPromptUserUsername").val();
                pData.UserPassword = $("#txtCustomPromptUserPassword").val();

                if (sessionStorage["OrgID"] != undefined) {
                    pData.OrgID = sessionStorage["OrgID"];
                }

                AjaxSend({
                    url: "People/CheckPeopleUserAccount",
                    data: pData,
                    success: function (vDataUserAcess) {
                        var res = vDataUserAcess.PeopleUserAccountData;

                        if (CStr2(res) == '') {
                            $('#CustomspanPromptUserError').show();
                        } else {
                            $('.popupCustomMsgBoxOverlayTheme').remove();
                            $('#custompromptuserpopup').css({ visibility: 'hidden', display: 'none' }).popup('close');
                            $('#CustomspanPromptUserError').hide();
                            vParams.OnClickOKSuccess(res);
                        }
                    }
                });
            })            
        }
    });