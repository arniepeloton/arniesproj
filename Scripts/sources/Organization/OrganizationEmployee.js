require([
    'common',
    //GetPathFormat('/Scripts3/Browser/LibraryFowst.js'),
    //GetPathFormat('/Scripts3/Browser/LibrarySecurityRole.js')
    ], function () {

        var IsNew = 1;
        var IsConfirm = 0;
        var OrgName = '';
        var ID = '';
        var IsViaEmail = 0;
        var PeopleEmailAddress = '';
        var manpowerid = '', vendorid = '', couserid = '', workscheduleid = '', paytypeid = '', deminimisid = '';
        var DepartmentID = '';
        var SystemGeneratedID = '';
        var CompanyID = '';
        var TeamID = '';
        var isEditEmpName = false;

        var arrEmployeeTelInfoList = [];
        var intEmployeeTelInfoCtr = 0;
        var EmployeeTelState = "";
        var TelRankNo = "";
        var arrEmployeeMobileInfoList = [];
        var intEmployeeMobileInfoCtr = 0;
        var EmployeeMobileState = "";
        var MobileRankNo = "";
        var arrEmployeeEmailInfoList = [];
        var intEmployeeEmailInfoCtr = 0;
        var EmployeeEmailState = "";
        var EmailRankNo = "";

        $(function () {
             
            var mLoaded = false;

            setMonthCollection("selOrgDateHiredMonth2");
            setYearCollection("selOrgDateHiredYear2");

            setMonthCollection("selOrgDateInActiveMonth");
            setYearCollection("selOrgDateInActiveYear");

            //$.mobile.activePage.bind('pageshow', PageShow);

            //GetOrgName();

            function PageShow() {
                mLoaded = true;
                //getOrgEmployeeList();
                //GetRequiredDataToAddEmp();

                if (sessionStorage.AddViewLibBCName == "Add Employee" || sessionStorage.AddViewLibBCName == "Add User") {
                    setMonthCollection("selOrgDateHiredMonth2");
                    setYearCollection("selOrgDateHiredYear2");

                    setMonthCollection("selOrgDateInActiveMonth");
                    setYearCollection("selOrgDateInActiveYear");
                    IsNew = 1;
                    $('#divbtnOld').hide();
                    $('#divbtnNew').show();
                    $('#divOk, #divEmpSaveCancelBtn, #liEmployeeResetMobizAcc').hide();
                    $('#divConfirmSection').show();
                    $('#divAddEmpNav').css({ visibility: 'visible', display: 'block' });
                    ShowHideOrgInActiveDetails();
                    GetAllFowst();
                    $('.peopleFirstName').text(sessionStorage.EmpFirstName);
                    $('#txtOrgName').text(sessionStorage.OrgName);
                }
                else if (sessionStorage.AddViewLibBCName == "Work Schedule") {
                    setMonthCollection("selOrgDateHiredMonth2");
                    setYearCollection("selOrgDateHiredYear2");

                    setMonthCollection("selOrgDateInActiveMonth");
                    setYearCollection("selOrgDateInActiveYear");
                    getOrgEmployeeList();
                }
                else {
                    setMonthCollection("selOrgDateHiredMonth2");
                    setYearCollection("selOrgDateHiredYear2");

                    setMonthCollection("selOrgDateInActiveMonth");
                    setYearCollection("selOrgDateInActiveYear");
                    IsNew = 0;
                    ID = sessionStorage.ViewID;
                    $('#txtOrgEmployee').show();
                    $('#txtOrgEmployeeNew').hide();
                    ShowHideOrgInActiveDetails();
                    if (sessionStorage.EmployeeStatus != undefined) {
                        if (sessionStorage.EmployeeStatus == "Pending Confirmation") {
                            $('#divOk, #divEmpSaveCancelBtn, #liEmployeeResetMobizAcc').hide();
                            $('#divConfirmSection').show();
                            $('#btnOrgEmpInfoSaveAndInvite').show();
                            $('#btnOrgEmpInfoSaveAndConfirm').show();
                            $('#btnOrgEmpInfoSendConfirmation').show();
                            $('#divAddEmpNav').css({ visibility: 'hidden', display: 'none' });
                        }
                        else {
                            $('#divOk').show();
                            $('#divConfirmSection').hide();
                            $('#divAddEmpNav').css({ visibility: 'hidden', display: 'none' });
                        }
                    }
                    else {
                        $('#divOk, #divEmpSaveCancelBtn, #liEmployeeResetMobizAcc').show();
                        $('#divConfirmSection').hide();
                        $('#divAddEmpNav').css({ visibility: 'hidden', display: 'none' });
                    }
                    GetRoleTag(function () {
                        ShowOrgEmployeeInfo(sessionStorage.ViewID);
                    });

                    //GetAllFowst();
                }

            };
            if (!mLoaded) PageShow();

            window.OrgEmp = {
                ShowOrgEmpData: function () {
                    ID = sessionStorage.ViewID;
                    IsNew = 0;
                    setMonthCollection("selOrgDateHiredMonth2");
                    setYearCollection("selOrgDateHiredYear2");

                    setMonthCollection("selOrgDateInActiveMonth");
                    setYearCollection("selOrgDateInActiveYear");
                    $('#txtOrgEmployee').show();
                    $('#txtOrgEmployeeNew').hide();
                    if (sessionStorage.EmployeeStatus != undefined) {
                        if (sessionStorage.EmployeeStatus == "Pending Confirmation") {
                            $('#divOk, #divEmpSaveCancelBtn, #liEmployeeResetMobizAcc').hide();
                            $('#divConfirmSection').show();
                            $('#btnOrgEmpInfoSaveAndInvite').show();
                            $('#btnOrgEmpInfoSaveAndConfirm').show();
                            $('#btnOrgEmpInfoSendConfirmation').show();
                            $('#divAddEmpNav').css({ visibility: 'hidden', display: 'none' });
                        } else {
                            $('#divOk, #divEmpSaveCancelBtn, #liEmployeeResetMobizAcc').show();
                            $('#divConfirmSection').hide();
                            $('#divAddEmpNav').css({ visibility: 'hidden', display: 'none' });
                        }
                    }
                    else {
                        $('#divOk, #divEmpSaveCancelBtn, #liEmployeeResetMobizAcc').show();
                        $('#divConfirmSection').hide();
                        $('#divAddEmpNav').css({ visibility: 'hidden', display: 'none' });
                    }
                    GetRoleTag(function () {
                        ShowOrgEmployeeInfo(sessionStorage.ViewID || '');
                    });
                    $(window).scrollTop(0);
                },
                GetRequiredData: function () {
                    setMonthCollection("selOrgDateHiredMonth2");
                    setYearCollection("selOrgDateHiredYear2");

                    setMonthCollection("selOrgDateInActiveMonth");
                    setYearCollection("selOrgDateInActiveYear");

                    if (sessionStorage.AddViewLibBCName == "Add Employee" || sessionStorage.AddViewLibBCName == "Add User") {
                        IsNew = 1;
                        $('#divbtnOld').hide();
                        $('#divbtnNew').show();
                        $('#divOk, #divEmpSaveCancelBtn, #liEmployeeResetMobizAcc').hide();
                        $('#divConfirmSection').show();
                        $('#divAddEmpNav').css({ visibility: 'visible', display: 'block' });
                        $('.peopleFirstName').text(sessionStorage.EmpFirstName);
                        $('#txtOrgName').text(sessionStorage.OrgName);
                    }
                    else if (sessionStorage.AddViewLibBCName == "Work Schedule") {
                        getOrgEmployeeList();
                    }
                    else {
                        ID = sessionStorage.ViewID;
                        IsNew = 0;
                        $('#txtOrgEmployee').show();
                        $('#txtOrgEmployeeNew').hide();
                        $('#divAddEmpNav').css({ visibility: 'hidden', display: 'none' });
                        ShowOrgEmployeeInfo(sessionStorage.ViewID);
                    }

                    ShowHideOrgInActiveDetails();
                    GetAllFowst();                    
                },

                GetEmpList: function () {
                    setMonthCollection("selOrgDateHiredMonth2");
                    setYearCollection("selOrgDateHiredYear2");

                    setMonthCollection("selOrgDateInActiveMonth");
                    setYearCollection("selOrgDateInActiveYear");
                    getOrgEmployeeList();
                }
            }

            //Navigation Menu
            $('#btnHome').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
            });

            $('#btnSystemAdmin').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuSettings"), "_self");
            });

            $('#btnDataLibraries').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopDataLibraries"), "_self");
            });

            //End Navigation Menu

            $('#btnBack').click(function () {
                history.back();
            });

            $('#txtDepartment').keydown(function (e) {
                BrowsePopup('LibraryDepartment', 'Search Department', [
                           {
                               DisplayText: [
                                   { Column: 'Name' }

                               ],
                               Click: function (e) {
                                   $('#txtDepartment').val(e.Name);
                                   DepartmentID = e.ID;
                               }
                           }
                ]);
            });

            $('#txtTeam').keydown(function (e) {
                BrowsePopup('LibraryTeam', 'Search Team', [
                           {
                               DisplayText: [
                                   { Column: 'Name' }

                               ],
                               Click: function (e) {
                                   $('#txtTeam').val(e.Name);
                                   TeamID = e.ID;
                               }
                           }
                ]);
            });
            
            $('#btnBank').click(function () {
                BrowsePopup('/Browser/LibraryOrganizationBanks', 'Select Bank', [
                     {
                         DisplayText: [{ Column: 'CompanyName' }],
                         AutoSearch: true,
                         Click: function (e) {
                             $('#txtBankID').val(e.ID);
                             $('#btnBank').text(e.CompanyName);//.button('refresh');
                         }
                     }
                ]);
            })

            $('body').on('click', '#btnOrgEmpInfoSaveAndInvite', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgPeopleID").val()) == "") {
                    NewMessageBox('An employment invite will be sent to ' + $('#txtOrgEmployee').val() + ' through the Mobiz Profile <br> app and to any available email address. ' + $('#txtOrgEmployee').val() + ' will need to confirm employment to <br> ' + OrgName + ' before being added to your employee list.', 'Employee');
                    return;
                }

                if ($.trim($("#selOrgDateHiredMonth2").val()) == "" || $.trim($("#selOrgDateHiredDay2").val()) == "" || $.trim($("#selOrgDateHiredYear2").val()) == "") {
                    NewMessageBox('Please enter valid date hired.', 'Date Hired');
                    return;
                }

                if (!$("#selOrgDeactivate").is(':checked')) {

                    ClearOrgEmployeeInActiveFields();

                }
                else {

                    if ($.trim($("#selOrgDateInActiveMonth").val()) == "" || $.trim($("#selOrgDateInActiveDay").val()) == "" || $.trim($("#selOrgDateInActiveYear").val()) == "") {
                        NewMessageBox('Please enter valid deactivation date.', 'Deactivation Date');
                        return;
                    }

                }

                if (window.location.pathname == '/SetupChecklist/SystemAdministration') {
                    if ($('#lstOrgEmployeeRole a').length == 0) {
                        NewMessageBox('Please select at least one access role.', 'Access Role');
                        return;
                    }
                }
                
                AjaxSend({
                    url: 'Organization/getPeopleOrgID',
                    data: { PeopleID: $('#txtOrgPeopleID').val() },
                    success: function (vData) {
                        if (vData.Result.Rows.length != 0) {
                            if (vData.Result.Rows[0].IsConfirmed == 0) {
                                IsViaEmail = 0;
                                IsConfirm = 0;
                                SaveOrganizationEmployee();
                            }
                            else {
                                NewMessageBox(sessionStorage.EmpFirstName + ' was already employed.', 'Employee');
                            }
                        }
                        else {
                            IsViaEmail = 0;
                            IsConfirm = 0;
                            SaveOrganizationEmployee();
                        }
                    }
                })
            })
            .on('click', '#btnOrgEmpInfoSaveAndConfirm', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgPeopleID").val()) == "") {
                    NewMessageBox('Please select employee.', 'Employee');
                    return;
                }

                if ($.trim($("#selOrgDateHiredMonth2").val()) == "" || $.trim($("#selOrgDateHiredDay2").val()) == "" || $.trim($("#selOrgDateHiredYear2").val()) == "") {
                    NewMessageBox('Please enter valid date hired.', 'Date Hired');
                    return;
                }

                if (window.location.pathname == '/SetupChecklist/SystemAdministration') {
                    if ($('#lstOrgEmployeeRole a').length == 0) {
                        NewMessageBox('Please select at least one access role.', 'Access Role');
                        return;
                    }
                }                

                AjaxSend({
                    url: 'Organization/getPeopleOrgID',
                    data: { PeopleID: $('#txtOrgPeopleID').val() },
                    success: function (vData) {
                        if (vData.Result.Rows.length != 0) {
                            if (vData.Result.Rows[0].IsConfirmed == 0) {
                                IsViaEmail = 0;
                                $('#pageOrgEmployeeSignIn').show();
                                $('#pageOrgEmployeeInfo').hide();
                            }
                            else {
                                NewMessageBox(sessionStorage.EmpFirstName + ' was already employed.', 'Employee');
                            }
                        }
                        else {
                            IsViaEmail = 0;
                            $('#pageOrgEmployeeSignIn').show();
                            $('#pageOrgEmployeeInfo').hide();
                        }
                    }
                })
            })
            .on('click', '#btnOrgRemoveAllEmployeeFowst', function () {
                $('#lstOrgEmployeeFowst').empty();
            })
            .on('click', '#btnOrgAddEmployeeFowst', function () {
                PopupButtonList('', 'Options', [
                    {
                        Text: 'Select from Company\'s FOWST list', Click: function () {
                            BrowsePopup('LibraryFowst', 'Select FOWST Locations',
                            [
                                {
                                    AutoSearch: true,
                                    SelectMultiple: true,
                                    AllowToSelectAll: true,
                                    Message: 'You can select multiple records. Click OK when you\'re done.',
                                    DisplayText: [{ Column: 'FowstName' }, { Column: 'Address' }],
                                    Click: function (e) {
                                        for (var i = 0; i < e.length; i++) {
                                            AddToFowstList(e[i].ID, e[i].FowstName, e[i].Address);
                                        }
                                    }
                                }
                            ])
                        }
                    }, {
                        Text: 'Select from Group\'s FOWST list', Click: function () {
                            BrowsePopup('LibraryFranchiseGroup', 'Select Group', [
                                {
                                    DisplayText: [{ Column: 'GroupName' }],
                                    AutoSearch: true,
                                    Message: '<b>Note:</b> FOWST location(s) of the selected group will be added on the list.',
                                    Click: function (e) {
                                        Bs.Loading('show');

                                        AjaxSend({
                                            url: 'ItemModifiers/GetFowstOfSelectedGroup',
                                            data: {
                                                GroupID: e.ID
                                            }, success: function (e) {
                                                Bs.Loading('hide');
                                                for (var i = 0; i < e.Results.Rows.length; i++) {
                                                    AddToFowstList(e.Results.Rows[i].ID, e.Results.Rows[i].FowstName, e.Results.Rows[i].Address);
                                                }
                                            }
                                        })
                                    }
                                }
                            ])
                        }
                    }, {
                        Text: 'Select from Franchisee\'s FOWST list', Click: function () {
                            BrowsePopup('LibraryOrganizationFranchiseeFowst', 'Select Franchisee\'s FOWSTs', [
                                {
                                    DisplayText: [{ Column: 'FowstName' }, { Column: 'Address' }],
                                    AutoSearch: true,
                                    SelectMultiple: true,
                                    Message: 'You can select multiple records. Click OK when you\'re done.',
                                    Click: function (e) {
                                        for (var i = 0; i < e.length; i++) {
                                            AddToFowstList(e[i].ID, e[i].FowstName, e[i].Address);
                                        }
                                    }
                                }
                            ])
                        }
                    }
                ])                
            })
            .on('click', '#btnOrgRemoveEmployeeSelectedFowst', function () {
                var chbEmpFowst = $('[name="chbEmpFowst"]:checked');

                if (chbEmpFowst.length == 0) {
                    NewMessageBox('No selected FOWST to be removed. Please tick the box of the FOWST.');
                    return;
                }

                chbEmpFowst.each(function () {
                    $('#' + $(this).data('id')).remove();
                })

                WebToast('Selected FOWST has been removed', 'top');
            })
            .on('click', '#btnOrgSelectionEmployeeFowst', function () {
                var chbEmpFowst = $('[name="chbEmpFowst"]'), val = CInt2($(this).attr('val'));

                if (val == 1) {
                    $(this).attr('val', 0);
                    $(this).text('Select All FOWST');
                    $('[name="chbEmpFowst"]').prop('checked', 0);
                } else {
                    $(this).attr('val', 1);
                    $(this).text('Deselect All FOWST');
                    $('[name="chbEmpFowst"]').prop('checked', 1);
                }
            })
            .on('click', '#btnOrgAddEmployeeRole', function () {
                BrowsePopup('LibrarySecurityRole', 'Access Role',
                    [
                        {
                            AutoSearch: true,
                            DisplayText: [{Column: 'Role'}, {Column: 'Description', Label: 'Description'}],
                            Click: function(e){
                                AddToRoleList(e.ID, e.Code, e.Description);
                            }
                        }
                    ])
            })
            .on('change', '#selOrgDateHiredMonth2', function (e) {
                e.preventDefault();
                $('#txtOrgDateHiredMonth').text($(this).val());
                setDayCollection('selOrgDateHiredMonth2', 'selOrgDateHiredDay2', 'selOrgDateHiredYear2', false);
            })
            .on('change', '#selOrgDateHiredYear2', function (e) {
                e.preventDefault();
                $('#txtOrgDateHiredYear').text($(this).val());
                setDayCollection('selOrgDateHiredMonth2', 'selOrgDateHiredDay2', 'selOrgDateHiredYear2', false);
            })
            .on('change', '#selOrgDateInActiveMonth', function (e) {
                e.preventDefault();
                $('#txtOrgDateInActiveMonth').text($(this).val());
                setDayCollection('selOrgDateInActiveMonth', 'selOrgDateInActiveDay', 'selOrgDateInActiveYear', false);
            })
            .on('change', '#selOrgDateInActiveYear', function (e) {
                e.preventDefault();
                $('#txtOrgDateInActiveYear').text($(this).val());
                setDayCollection('selOrgDateInActiveMonth', 'selOrgDateInActiveDay', 'selOrgDateInActiveYear', false);
            })
            .on('click', '#btnOKOrgEmp', function (e) {
                e.preventDefault();
                $('#panOrgEmpMessage').popup('close');
            })
            .on('keyup', '#txtSearchKeyword', function (e) {
                if (e.keyCode == 13) {
                    if ($('#TitleLibrary').text() == "FOWST") {
                        SearchFowst($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "EMPLOYEE") {
                        SearchEmployeePopUp($(this).val());
                    }
                    else {
                        SearchRole($(this).val());
                    }
                }
            })
            .on('change', '#selOrgDeactivate', function (e) {
                e.preventDefault();
                ShowHideOrgInActiveDetails();
            })
            .on('click', '#btnOrgEmpSignIn', function (e) {
                e.preventDefault();
                IsViaEmail = 0;
                VerifyUsernameAndPassword();
            })
            .on('keypress', '#txtPassword', function (e) {
                if (e.keyCode == 13) {
                    VerifyUsernameAndPassword();
                }
            })
            .on('click', '#btnOrgEmpCancel', function (e) {
                e.preventDefault();
                $('#pageOrgEmployeeSignIn').hide();
                $('#pageOrgEmployeeInfo').show();
            })
            .on('click', '#btnOrgEmpInfoCancel', function (e) {
                window.location.hash = 'LibraryPage';
            })
            .on('click', '#btnOrgEmpInfoSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgPeopleID").val()) == "") {
                    NewMessageBox('Please select employee.', 'Employee');
                    return;
                }

                if ($.trim($("#selOrgDateHiredMonth2").val()) == "" || $.trim($("#selOrgDateHiredDay2").val()) == "" || $.trim($("#selOrgDateHiredYear2").val()) == "") {
                    NewMessageBox('Please enter valid date hired.', 'Date Hired');
                    return;
                }

                if (!$("#selOrgDeactivate").is(':checked')) {

                    ClearOrgEmployeeInActiveFields();

                }
                else {

                    if ($.trim($("#selOrgDateInActiveMonth").val()) == "" || $.trim($("#selOrgDateInActiveDay").val()) == "" || $.trim($("#selOrgDateInActiveYear").val()) == "") {
                        $('#divBtnCancel').hide();
                        NewMessageBox('Please enter valid deactivation date.', 'Deactivation Date');
                        return;
                    }

                }

                //if ($('[name="radRoletag"]:checked').length == 0) {
                //    NewMessageBox("Please choose user label.", "User Labels");
                //    return;
                //}

                if ($.trim($('#txtOrg3rdPartySystemName').val()) != '') {
                    if ($.trim($('#txtOrg3rdPartySystemID').val()) == '') {
                        NewMessageBox("Please enter third party system id.", "Message");
                        return;
                    }
                }

                if (isEditEmpName) {
                    if ($.trim($('#txtOrgEmployeeFirstName').val()) == '') {
                        NewMessageBox("Please enter employee's first name", "Message");
                    }

                    if ($.trim($('#txtOrgEmployeeLastName').val()) == '') {
                        NewMessageBox("Please enter employee's last name", "Message");
                    }
                }

                if ($.trim($('#txtEmployeeNewUsername').val()) != '' || $.trim($('#txtEmployeeNewPassword').val()) != '') {
                    if ($.trim($('#txtEmployeeNewUsername').val()) == '' || $.trim($('#txtEmployeeNewPassword').val()) == '') {
                        NewMessageBox("Please enter your new username and password.", "Message");
                        return;
                    } else if ($.trim($('#txtEmployeeNewUsername').val()).length < 6 || $.trim($('#txtEmployeeNewPassword').val()).length < 6) {
                        NewMessageBox("Username and Password must be at least 6 characters.", "Message");
                        return;
                    } else {
                        NewPromptUserAccess('Allow to reset employee Mobiz\'s account', {
                            Message: 'You\'re not allowed to reset employee Mobiz\'s account. Please contact your system administrator for assistance.',
                            OnClickOKSuccess: function (e) {
                                //Check username if exist
                                AjaxSend({
                                    url: 'SignUp/CheckPeopleUniqueUserName',
                                    data: { Username: $.trim($('#txtEmployeeNewUsername').val()) },
                                    success: function (vData) {
                                        if (vData.PeopleUserNameData.Rows.length > 0) {
                                            if (vData.PeopleUserNameData.Rows[0].PeopleID == $.trim($("#txtOrgPeopleID").val())) {
                                                //If username is the same as his existing username, let the user use it again
                                                SaveOrganizationEmployee(e.AllowedUserID);
                                            } else {
                                                NewMessageBox('Someone already has that user name. Please try another.');
                                            }
                                        } else {
                                            SaveOrganizationEmployee(e.AllowedUserID);
                                        }
                                    } // success
                                }); // AjaxSend
                            }
                        });                        
                    }
                } else {
                    SaveOrganizationEmployee();
                }                
            })
            .on('click', '#btnOrgEmpInfoSendConfirmation', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgPeopleID").val()) == "") {
                    NewMessageBox('An employment invite will be sent to ' + $('#txtOrgEmployee').val() + ' through the Mobiz Profile <br> app and to any available email address. ' + $('#txtOrgEmployee').val() + ' will need to confirm employment to <br> ' + OrgName + ' before being added to your employee list.', 'Employee');
                    return;
                }

                if ($.trim($("#selOrgDateHiredMonth2").val()) == "" || $.trim($("#selOrgDateHiredDay2").val()) == "" || $.trim($("#selOrgDateHiredYear2").val()) == "") {
                    NewMessageBox('Please enter valid date hired.', 'Date Hired');
                    return;
                }

                if (!$("#selOrgDeactivate").is(':checked')) {

                    ClearOrgEmployeeInActiveFields();

                }
                else {

                    if ($.trim($("#selOrgDateInActiveMonth").val()) == "" || $.trim($("#selOrgDateInActiveDay").val()) == "" || $.trim($("#selOrgDateInActiveYear").val()) == "") {
                        NewMessageBox('Please enter valid deactivation date.', 'Deactivation Date')
                        return;
                    }

                }

                if (window.location.pathname == '/SetupChecklist/SystemAdministration') {
                    if ($('#lstOrgEmployeeRole a').length == 0) {
                        NewMessageBox('Please select at least one access role.', 'Access Role');
                        return;
                    }
                }                

                IsViaEmail = 1;
                CheckPeopleEmailAddress();
            })
            .on('click', '#btnChangeEmployee', function () {
                BrowsePopup('LibraryPeople', 'Employee',
                    [
                        {
                            DisplayText: [{Column: 'PeopleName'}],
                            Click: function(e){
                                sessionStorage.txtOrgPeopleID = e.ID;
                                sessionStorage.btnOrgEmployeeName = e.PeopleName;
                                sessionStorage.EmpFirstName = e.PeopleFirstName;
                                $('#txtOrgPeopleID').val(e.ID);
                                $('#txtOrgEmployeeNew').val(e.PeopleName);
                                $('#btnOrgEmpInfoSaveAndInvite').html(e.PeopleFirstName + '\'s Mobiz app');
                                $('#btnOrgEmpInfoSaveAndConfirm').html(e.PeopleFirstName + ' sign in and confirm now');
                                $('#btnOrgEmpInfoSendConfirmation').html(e.PeopleFirstName + '\'s Email');
                                $('.peopleFirstName').text(e.PeopleFirstName);
                            }
                        }
                    ])
            })
            .on('click', '#btnAddNewEmployee', function () {
                $('#pageOrgEmployee').hide();
                $('#pageOrgEmployeeInfo').hide();
                $('#pageOrgEmployeeSignIn').hide();
                $('#pageOrgEmployeeAdd').show();
            })
            .on('change', '#selSelfEmployed', function () {
                if ((CBool($(this).val()) && CBool($('#selManPower').is(':checked'))) || (!CBool($(this).val()) && !CBool($('#selManPower').is(':checked')))) {
                    manpowerid = '';
                    vendorid = '';
                    couserid = '';
                    $('#selManPower').prop('checked', 0);//.slider('refresh');
                    $('#divOtherRelationship, #divManPower, #divCoUserOrgName').hide();
                    $('#btnEmpManPower, #btnEmpVendorTradingPartner, #btnEmpCoUser, #btnEmpWorkSchedule, #btnEmpPayType').text('Please select');//.button('refresh');
                } else if (CBool($(this).is(':checked')) && !CBool($('#selManPower').is(':checked'))) {
                    $('#divOtherRelationship, #divManPower').hide();
                }

                if (!CBool($(this).is(':checked')) && !CBool($('#selManPower').is(':checked'))) {
                    $('#divOtherRelationship, #divManPower').show();
                }
            })
            .on('change', '#selManPower', function () {
                if ((CBool($(this).val()) && !CBool($('#selSelfEmployed').is(':checked'))) || (!CBool($(this).val()) && !CBool($('#selSelfEmployed').is(':checked')))) {
                    manpowerid = '';
                    vendorid = '';
                    couserid = '';
                    $('#btnEmpManPower, #btnEmpVendorTradingPartner, #btnEmpCoUser, #btnEmpWorkSchedule, #btnEmpPayType').text('Please select');//.button('refresh');
                    $('#divManPower, #divOtherRelationship').hide();
                    $('#selVendorTradingPartner').prop('checked', 1);//.slider('refresh');
                    $('#divVenderTradingPartner').show();
                    $('#selEmpCoUser').prop('checked', 0);//.slider('refresh');
                    $('#divCoUserOrgName').hide();
                } else if (CBool($(this).is(':checked')) && CBool($('#selSelfEmployed').is(':checked'))) {
                    $('#selSelfEmployed').prop('checked', 0);//.slider('refresh');
                }

                if (!CBool($(this).is(':checked')) && !CBool($('#selSelfEmployed').is(':checked'))) {
                    $('#divManPower, #divOtherRelationship').show();
                }
            })
            .on('change', '#selVendorTradingPartner', function () {
                vendorid = '';
                //manpowerid = '';
                $('#btnEmpVendorTradingPartner').text('Please select');//.button('refresh');

                if (CBool($(this).is(':checked'))) {
                    $('#divVenderTradingPartner').show();
                } else {
                    $('#divVenderTradingPartner').hide();
                }

                if (CBool($(this).is(':checked')) && CBool($('#selEmpCoUser').is(':checked'))) {
                    $('#selEmpCoUser').prop('checked', 0);//.slider('refresh');
                    $('#divCoUserOrgName').hide();
                    $('#btnEmpCoUser').text('Please select');//.button('refresh');
                    couserid = '';
                }
            })
            .on('change', '#selEmpCoUser', function () {
                couserid = '';
                //manpowerid = '';
                $('#btnEmpCoUser').text('Please select');//.button('refresh');

                if (CBool($(this).is(':checked'))) {
                    $('#divCoUserOrgName').show();
                } else {
                    $('#divCoUserOrgName').hide();
                }

                if (CBool($(this).is(':checked')) && CBool($('#selVendorTradingPartner').is(':checked'))) {
                    $('#selVendorTradingPartner').prop('checked', 0);//.slider('refresh');
                    $('#divVenderTradingPartner').hide();
                    $('#btnEmpVendorTradingPartner').text('Please select');//.button('refresh');
                    vendorid = '';
                }
            })
            .on('click', '#btnEmpWorkSchedule', function () {
                SearchWorkSchedule('workschedule', function (e) {
                    workscheduleid = e.ID;
                    GetWorkScheduleDetails(workscheduleid);
                    $('#btnEmpWorkSchedule').text(e.ScheduleName);//.button('refresh');
                });
            })
            .on('click', '#btnEmpPayType', function () {
                SearchPayType('paytype', function (e) {
                    paytypeid = e.ID;
                    GetPayGradeDetails(paytypeid)
                    $('#btnEmpPayType').text(e.SalaryGradeName);//.button('refresh');
                    GetDeminimis();
                });
            })
            .on('click', '#btnOrgAddEmployeeDeminimis', function () {
                //$('#popDeminimis').modal('show');
                //SearchDeminimis('deminimis', function (e) {
                    
                //});
            })
            .on('click', '#btnDeminimis', function () {                
                SearchDeminimis('deminimis', function (e) {
                    deminimisid = e.ID;
                    $('#btnDeminimis').text(e.Name);//.button('refresh');
                    $('#lblMaxAmount').text(e.MaxAmount);
                    $('#lblMaxTaxExemptAmount').text(e.MaxTaxExemptAmount);
                    $('#lblFrequencyofAllowance').text(e.FrequencyOfAllowance);
                });
            })
            .on('click', '#btnDoneDeminimis', function () {
                var tbl = $('#tblDeminimisBody');

                var data = {
                    DeminimisID: deminimisid,
                    Name: $('#btnDeminimis').text(),
                    Amount: $('#txtDeminimisAmount').val(),
                    MaxAmount: $('#lblMaxAmount').text(),
                    MaxTaxExemptAmount: $('#lblMaxTaxExemptAmount').text(),
                    FrequencyofAllowance: $('#lblFrequencyofAllowance').text()
                };

                tbl.append($('<tr>')
                        .data('data', data)
                        .append($('<td>')
                            .css('text-align', 'center')
                            .text(data.Name)
                        )
                        .append($('<td>')
                            .css('text-align', 'center')
                            .text(data.Amount)
                        )
                        .append($('<td>')
                            .css('text-align', 'center')
                            .text(data.MaxAmount)
                        )
                        .append($('<td>')
                            .css('text-align', 'center')
                            .text(data.FrequencyofAllowance)
                        )
                        .append($('<td>')
                            .css('text-align', 'center')
                            .append($('<a data-role="button" data-theme="b" data-icon="delete" data-iconpos="notext">')
                            )
                        )
                        );
            })
            .on('click', '#btnEmpManPower', function () {
                SearchPersonOrgRelationship('manpower', function (e) {
                    manpowerid = e.SupplierID;
                    $('#btnEmpManPower').text(e.SupplierName);//.button('refresh');
                });
            })
            .on('click', '#btnEmpVendorTradingPartner', function () {
                SearchPersonOrgRelationship('partner', function (e) {
                    vendorid = e.SupplierID;
                    $('#btnEmpVendorTradingPartner').text(e.SupplierName);//.button('refresh');
                });
            })
            .on('click', '#btnEmpCoUser', function () {
                SearchPersonOrgRelationship('co-user', function (e) {
                    couserid = e.SupplierID;
                    $('#btnEmpCoUser').text(e.SupplierName);//.button('refresh');
                });
            })
                .on('click', '#btnAddEmployeeTelephone', function () {
                    ClearEmployeeTelInfo();
                    $('#popEmployeeTelInfo').modal('show');
                })
                .on('click', '#btnCancelEmployeeTelephone', function () {
                    $('#popEmployeeTelInfo').modal('hide');
                    EmployeeTelState = '';
                })
                .on('click', '#btnEmployeeTelAreaCode', function () {
                    BrowsePopup('LibraryAreaCodes', 'Area Codes',
                        [
                            {
                                DisplayText: [{ Column: 'Area' }, { Label: 'Code', Column: 'CallingCode' }],
                                Click: function (e) {
                                    $('#txtEmployeeTelAreaCodeID').val(e.ID);
                                    $('#txtEmployeeAreaCode').text(e.Area + ' - ' + e.CallingCode)
                                }
                            }
                        ]
                    )
                })
                .on('keydown', '#txtEmployeeTelephoneNum', function (e) {
                    if (!NumberDotOnly($(this), e)) {
                        return;
                    }
                })
                .on('click', '#btnSaveEmployeeTelephone', function () {
                    if ($('#txtEmployeeAreaCode').text() == "Select") {
                        $('#spanerrorEmployeeAreaCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                        return false;
                    }

                    if ($('#txtEmployeeTelephoneNum').val() == "") {
                        $('#spanerrorEmployeeTelephoneNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                        return false;
                    }

                    //if ($('#txtEmployeeTelephoneNum').val().length != 7) {
                    //    $('#spanerrorEmployeeTelephoneNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    //    return false;
                    //}

                    if ($('#selEmployeeTelType').val() == "") {
                        $('#spanerrorEmployeeTelType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                        return false;
                    }
                    intEmployeeTelInfoCtr = intEmployeeTelInfoCtr + 1;

                    if (EmployeeTelState == "") {
                        arrEmployeeTelInfoList.push({
                            "TelRankNo": intEmployeeTelInfoCtr,
                            "CountryCodeID": $('#txtEmployeeTelCountryID').val(),
                            "CountryCode": $('#txtEmployeeCountryCode').text(),
                            "AreaCode": $('#txtEmployeeAreaCode').text(),
                            "AreaCodeID": $('#txtEmployeeTelAreaCodeID').val(),
                            "TelephoneNo": $('#txtEmployeeTelephoneNum').val(),
                            "ExtensionNo": $('#txtEmployeeTelephoneExt').val(),
                            "ContactType": $('#selEmployeeTelType').val(),
                            "TelState": "Add",
                            "IsNew": 1,
                            "TelID": $('#txtEmployeeTelID').val()
                        })
                    }
                    else {
                        arrEmployeeTelInfoList[sessionStorage.Telindex].TelRankNo = TelRankNo;
                        arrEmployeeTelInfoList[sessionStorage.Telindex].CountryCodeID = $('#txtEmployeeTelCountryID').val();
                        arrEmployeeTelInfoList[sessionStorage.Telindex].CountryCode = $('#txtEmployeeCountryCode').text();
                        arrEmployeeTelInfoList[sessionStorage.Telindex].AreaCodeID = $('#txtEmployeeTelAreaCodeID').val();
                        arrEmployeeTelInfoList[sessionStorage.Telindex].AreaCode = $('#txtEmployeeAreaCode').text();
                        arrEmployeeTelInfoList[sessionStorage.Telindex].TelephoneNo = $('#txtEmployeeTelephoneNum').val();
                        arrEmployeeTelInfoList[sessionStorage.Telindex].ExtensionNo = $('#txtEmployeeTelephoneExt').val();
                        arrEmployeeTelInfoList[sessionStorage.Telindex].ContactType = $('#selEmployeeTelType').val();
                        arrEmployeeTelInfoList[sessionStorage.Telindex].TelState = sessionStorage.TelState;
                        arrEmployeeTelInfoList[sessionStorage.Telindex].IsNew = sessionStorage.TelIsNew;
                        arrEmployeeTelInfoList[sessionStorage.Telindex].TelID = $('#txtEmployeeTelID').val();
                    }

                    AddEmployeeTelInfo();
                    ClearEmployeeTelInfo();
                    EmployeeTelState = "";
                })
                .on('click', '#btnAddEmployeeMobile', function () {
                    ClearEmployeeMobileInfo();
                    $('#popEmployeeMobileInfo').modal('show');
                })
                .on('click', '#btnCancelEmployeeMobile', function () {
                    $('#popEmployeeMobileInfo').modal('hide');
                    EmployeeMobileState = '';
                })
                .on('click', '#btnEmployeeMobileTelCode', function () {
                    BrowsePopup('LibraryTelcoPrefixes', 'Telco Codes',
                        [
                            {
                                DisplayText: [{ Column: 'Prefix' }, { Column: 'TelCo' }],
                                Click: function (e) {
                                    $('#txtEmployeeTelCoPrefixID').val(e.ID);
                                    $('#txtEmployeeTelCode').text(e.TelCo + ' - ' + e.Prefix)
                                }
                            }
                        ]
                    )
                })
                .on('keydown', '#txtEmployeeMobileNo', function (e) {
                    if (!NumberDotOnly($(this), e)) {
                        return;
                    }
                })
                .on('click', '#btnSaveEmployeeMobile', function () {
                    //if ($('#txtEmployeeTelCode').text() == "Select") {
                    //    $('#spanerrorEmployeeMobileTelCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    //    return false;
                    //}

                    if ($('#txtEmployeeMobileNo').val() == "") {
                        $('#spanerrorEmployeeMobileNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                        return false;
                    }

                    //if ($('#txtEmployeeMobileNo').val().length != 7) {
                    //    $('#spanerrorEmployeeMobileNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    //    return false;
                    //}

                    if ($('#selEmployeeMobileType').val() == "") {
                        $('#spanerrorEmployeeMobileType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                        return false;
                    }

                    intEmployeeMobileInfoCtr = intEmployeeMobileInfoCtr + 1;

                    if (EmployeeMobileState == "") {
                        arrEmployeeMobileInfoList.push({
                            "MobileRankNo": intEmployeeMobileInfoCtr,
                            "CountryCodeID": $('#txtEmployeeMobileCountryID').val(),
                            "CountryCode": $('#txtEmployeeMobileCountryCode').text(),
                            //"TelCode": $('#txtEmployeeTelCode').text(),
                            //"TelCodeID": $('#txtEmployeeTelCoPrefixID').val(),
                            "MobileNo": $('#txtEmployeeMobileNo').val(),
                            "ContactType": $('#selEmployeeMobileType').val(),
                            "MobileID": $('#txtEmployeeMobileID').val(),
                            "MobileState": "Add",
                            "IsNew": 1,
                            "MobileNos": '+' + $('#txtEmployeeMobileCountryCode').text() + $('#txtEmployeeTelCode').text() + $('#txtEmployeeMobileNo').val()
                        })
                    }
                    else {
                        arrEmployeeMobileInfoList[sessionStorage.Mobileindex].MobileRankNo = MobileRankNo;
                        arrEmployeeMobileInfoList[sessionStorage.Mobileindex].CountryCodeID = $('#txtEmployeeMobileCountryID').val();
                        arrEmployeeMobileInfoList[sessionStorage.Mobileindex].CountryCode = $('#txtEmployeeMobileCountryCode').text();
                        //arrEmployeeMobileInfoList[sessionStorage.Mobileindex].TelCodeID = $('#txtEmployeeTelCoPrefixID').val();
                        //arrEmployeeMobileInfoList[sessionStorage.Mobileindex].TelCode = $('#txtEmployeeTelCode').text();
                        arrEmployeeMobileInfoList[sessionStorage.Mobileindex].MobileNo = $('#txtEmployeeMobileNo').val();
                        arrEmployeeMobileInfoList[sessionStorage.Mobileindex].ContactType = $('#selEmployeeMobileType').val();
                        arrEmployeeMobileInfoList[sessionStorage.Mobileindex].MobileState = sessionStorage.MobileState;
                        arrEmployeeMobileInfoList[sessionStorage.Mobileindex].IsNew = sessionStorage.MobileIsNew;
                        arrEmployeeMobileInfoList[sessionStorage.Mobileindex].MobileNos = '+' + $('#txtEmployeeMobileCountryCode').text() + $('#txtEmployeeTelCode').text() + $('#txtEmployeeMobileNo').val()
                    }

                    AddEmployeeMobileInfo();
                    EmployeeMobileState = "";
                    ClearEmployeeMobileInfo();
                })
                .on('click', '#btnAddEmployeeEmail', function () {
                    ClearEmployeeEmailInfoFields();
                    $('#popEmployeeEmailInfo').modal('show');
                })
                .on('click', '#btnCancelEmployeeEmailAddress', function () {
                    $('#popEmployeeEmailInfo').modal('hide');
                    EmployeeEmailState = '';
                })
                .on('click', '#btnSaveEmployeeEmailAddress', function () {
                    if (IsEmail($.trim($("#txtEmployeeEmailAdd").val())) == false) {
                        $('#spanerrorEmployeeEmailAddressValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                        return false;
                    }

                    if ($.trim($("#txtEmployeeEmailAdd").val()) == "") {
                        $('#spanerrorEmployeeEmailAddress').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                        return false;
                    }

                    if ($('#selEmployeeEmailType').val() == "") {
                        $('#spanerrorEmployeeEmailType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                        return false;
                    }

                    intEmployeeEmailInfoCtr = intEmployeeEmailInfoCtr + 1;

                    if (EmployeeEmailState == "") {
                        arrEmployeeEmailInfoList.push({
                            "EmailRankNo": intEmployeeEmailInfoCtr,
                            "EmailAddID": $('#txtEmployeeEmailID').val(),
                            "EmailAdd": $('#txtEmployeeEmailAdd').val(),
                            "EmailType": $('#selEmployeeEmailType').val(),
                            "EmailState": "Add",
                            "IsNew": 1
                        })
                    }
                    else {
                        arrEmployeeEmailInfoList[sessionStorage.Emailindex].EmailRankNo = EmailRankNo;
                        arrEmployeeEmailInfoList[sessionStorage.Emailindex].EmailAddID = $('#txtEmployeeEmailID').val();
                        arrEmployeeEmailInfoList[sessionStorage.Emailindex].EmailAdd = $('#txtEmployeeEmailAdd').val();
                        arrEmployeeEmailInfoList[sessionStorage.Emailindex].EmailType = $('#selEmployeeEmailType').val();
                        arrEmployeeEmailInfoList[sessionStorage.Emailindex].EmailState = sessionStorage.EmailState;
                        arrEmployeeEmailInfoList[sessionStorage.Emailindex].IsNew = sessionStorage.EmailIsNew;
                    }

                    AddEmployeeEmailInfo();
                    EmployeeEmailState = "";
                })

            $('#frmOrgEmployeeSearch').submit(function (e) {
                e.preventDefault();

                getOrgEmployeeList();

                return false;
            });

            $('#btnOrgAddEmployee').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageOrgEmployeeInfo');
                ClearOrgEmployeeFields();
                ClearOrgEmployeeInActiveFields();

                $('#btnOrgEmpInfoSave').hide()
                $('#btnOrgEmpInfoCancel').hide()
                $('#btnOrgEmpInfoSaveAndInvite').show()
                $('#btnOrgEmpInfoSaveAndConfirm').show();
                $('#btnOrgEmpInfoSendConfirmation').show();

                ShowHideOrgInActiveDetails();
                GetAllFowst();

                IsNew = 1;
            });

            $('#btnOrgEmployee').click(function (e) {

                Browser.LibraryPeople.Click = function (e) {

                    $('#txtOrgPeopleID').val(e.ID)
                    $('#btnOrgEmployee').text(e.PeopleName)
                    ;//.button('refresh');
                    $('#btnOrgEmpInfoSaveAndInvite .ui-btn-text').html('<h3> Send ' + e.PeopleName + ' Invite </h3> ')
                    $('#btnOrgEmpInfoSaveAndConfirm .ui-btn-text').html('<h3> Let ' + e.PeopleName + ' confirm employment by signing in now </h3> ')

                    GetAllFowst()
                };

                Browser.LibraryPeople.Show();

            });

            $('#btnOrgEmployeeDone').click(function (e) {
                e.preventDefault();

                sessionStorage.removeItem('SourcePage');
                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            });

            $('#btnOrgEmpGoToSetupChecklist').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/SetupChecklist"), "_self");

            });

            function AddEmployeeEmailInfo() {

                var pLV = $('#lstEmployeeEmail');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrEmployeeEmailInfoList.length; i++) {
                    if (arrEmployeeEmailInfoList[i].EmailState != "Remove") {
                        arrEmployeeEmailInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                            .data(arrEmployeeEmailInfoList[i])
                            .append($('<p class="pr-2 m-0 flex-grow-1">')
                                .html($.trim(arrEmployeeEmailInfoList[i].EmailAdd) +
                                    "<br>" + $.trim(arrEmployeeEmailInfoList[i].EmailType)
                                )
                                .data(arrEmployeeEmailInfoList[i])
                                .click(function (e) {
                                    EditEmployeeEmailInfo($(this).data().arrindex);
                                })
                            )
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .append('<span class="bi-trash"></span>')
                                .data('data', arrEmployeeEmailInfoList[i])
                                .click(function (e) {
                                    var EmailRankNo = $(this).data('data').EmailRankNo;

                                    if ($(this).data('data').IsNew == 1) {
                                        //remove selected email
                                        arrEmployeeEmailInfoList = $.grep(arrEmployeeEmailInfoList, function (val) {
                                            return val.EmailRankNo != EmailRankNo;
                                        })
                                    }
                                    else {
                                        arrEmployeeEmailInfoList[$(this).data('data').arrindex].EmailState = "Remove";
                                    }

                                    AddEmployeeEmailInfo();
                                })
                            )

                        pLV.append(pItem);
                    }
                }

                pLV;//.listview('refresh');
                $('#popEmployeeEmailInfo').modal('hide');
            }

            function EditEmployeeEmailInfo(arrindex) {
                $('#popEmployeeEmailInfo').modal('show');
                EmployeeEmailRankNo = arrEmployeeEmailInfoList[arrindex].EmailRankNo;
                EmployeeEmailState = "Edit";
                sessionStorage["Emailindex"] = arrindex;
                sessionStorage["EmailIsNew"] = arrEmployeeEmailInfoList[arrindex].IsNew;
                sessionStorage["EmailState"] = arrEmployeeEmailInfoList[arrindex].EmailState;
                $('#txtEmployeeEmailID').val(arrEmployeeEmailInfoList[arrindex].EmailAddID == undefined ? "" : arrEmployeeEmailInfoList[arrindex].EmailAddID);
                $('#txtEmployeeEmailAdd').val(arrEmployeeEmailInfoList[arrindex].EmailAdd);
                $('#selEmployeeEmailType').val(arrEmployeeEmailInfoList[arrindex].EmailType)//.selectmenu('refresh');
            }

            function ClearEmployeeEmailInfoFields() {
                $('#txtEmployeeEmailID').val('');
                $('#txtEmployeeEmailAdd').val('');
                $('#selEmployeeEmailType').val('')//.selectmenu('refresh');
            }

            function AddEmployeeMobileInfo() {

                var pLV = $('#lstEmployeeMobile');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrEmployeeMobileInfoList.length; i++) {
                    if (arrEmployeeMobileInfoList[i].MobileState != "Remove") {
                        arrEmployeeMobileInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                            .data(arrEmployeeMobileInfoList[i])
                            .append($('<p class="pr-2 m-0 flex-grow-1">')
                                .data(arrEmployeeMobileInfoList[i])
                                .attr('ID', arrEmployeeMobileInfoList[i].id)
                                .html("+" + $.trim(arrEmployeeMobileInfoList[i].CountryCode.split('-')[1]) + "" + $.trim(arrEmployeeMobileInfoList[i].MobileNo) +
                                    "<br>" + $.trim(arrEmployeeMobileInfoList[i].ContactType)
                                )
                                .click(function (e) {
                                    EditEmployeeMobileInfo($(this).data().arrindex);
                                })
                            )
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .append('<span class="bi-trash"></span>')
                                .data('data', arrEmployeeMobileInfoList[i])
                                .click(function (e) {
                                    var MobileRankNo = $(this).data('data').MobileRankNo;

                                    if ($(this).data('data').IsNew == 1) {
                                        //remove selected mobile
                                        arrEmployeeMobileInfoList = $.grep(arrEmployeeMobileInfoList, function (val) {
                                            return val.MobileRankNo != MobileRankNo;
                                        })
                                    }
                                    else {
                                        arrEmployeeMobileInfoList[$(this).data('data').arrindex].MobileState = "Remove";
                                    }

                                    AddEmployeeMobileInfo();
                                })
                            )

                        pLV.append(pItem);
                    }
                }

                pLV;//.listview('refresh');
                $('#popEmployeeMobileInfo').modal('hide');
            }

            function EditEmployeeMobileInfo(arrindex) {
                $('#popEmployeeMobileInfo').modal('show');
                EmployeeMobileRankNo = arrEmployeeMobileInfoList[arrindex].MobileRankNo;
                EmployeeMobileState = "Edit";
                sessionStorage["Mobileindex"] = arrindex;
                sessionStorage["MobileIsNew"] = arrEmployeeMobileInfoList[arrindex].IsNew;
                sessionStorage["MobileState"] = arrEmployeeMobileInfoList[arrindex].MobileState;
                $('#txtEmployeeMobileID').val(arrEmployeeMobileInfoList[arrindex].MobileID == undefined ? "" : arrEmployeeMobileInfoList[arrindex].MobileID);
                $('#txtEmployeeTelCoPrefixID').val(arrEmployeeMobileInfoList[arrindex].TelCodeID);
                $('#txtEmployeeMobileCountryID').val(arrEmployeeMobileInfoList[arrindex].CountryCodeID);
                $('#txtEmployeeMobileCountryCode').text(arrEmployeeMobileInfoList[arrindex].CountryCode);
                //$('#txtEmployeeTelCode').text(arrEmployeeMobileInfoList[arrindex].TelCode);
                $('#txtEmployeeMobileNo').val(arrEmployeeMobileInfoList[arrindex].MobileNo);
                $('#selEmployeeMobileType').val(arrEmployeeMobileInfoList[arrindex].ContactType)//.selectmenu('refresh');
            }

            function ClearEmployeeMobileInfo() {
                $('#txtEmployeeMobileID').val('');
                $('#txtEmployeeTelCoPrefixID').val('');
                $('#txtEmployeeMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtEmployeeMobileCountryCode').text('PHL - 63');
                $('#txtEmployeeTelCode').text('Select');
                $('#txtEmployeeMobileNo').val('');
                $('#selEmployeeMobileType').val('')//.selectmenu('refresh');
            }

            function AddEmployeeTelInfo() {

                var pLV = $('#lstEmployeeTelephone');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrEmployeeTelInfoList.length; i++) {
                    if (arrEmployeeTelInfoList[i].TelState != "Remove") {
                        arrEmployeeTelInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                            .data(arrEmployeeTelInfoList[i])
                            .append($('<p class="pr-2 m-0 flex-grow-1">')
                                .html("+" + $.trim(CStr2(arrEmployeeTelInfoList[i].CountryCode).split('-')[1]) + " (" + $.trim(CStr2(arrEmployeeTelInfoList[i].AreaCode).split('-')[1]) + ") " + $.trim(CStr2(arrEmployeeTelInfoList[i].TelephoneNo)) + ($.trim(CStr2(arrEmployeeTelInfoList[i].ExtensionNo)) == "" ? "" : "." + $.trim(CStr2(arrEmployeeTelInfoList[i].ExtensionNo))) +
                                "<br>" + $.trim(CStr2(arrEmployeeTelInfoList[i].AreaCode).split('-')[0]) + ", " + $.trim(CStr2(arrEmployeeTelInfoList[i].CountryCode).split('-')[0]) +
                                "<br>" + $.trim(CStr2(arrEmployeeTelInfoList[i].ContactType))
                                )
                                .data('data', arrEmployeeTelInfoList[i])
                                .click(function (e) {
                                    EditEmployeeTelInfo($(this).data('data').arrindex);
                                })
                            )
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .append('<span class="bi-trash"></span>')
                                .data('data', arrEmployeeTelInfoList[i])
                                .click(function (e) {
                                    var TelRankNo = $(this).data('data').TelRankNo;

                                    if ($(this).data('data').IsNew == 1) {
                                        //remove selected telephone
                                        arrEmployeeTelInfoList = $.grep(arrEmployeeTelInfoList, function (val) {
                                            return val.TelRankNo != TelRankNo;
                                        })
                                    }
                                    else {
                                        arrEmployeeTelInfoList[$(this).data('data').arrindex].TelState = "Remove";
                                    }

                                    AddEmployeeTelInfo();
                                })
                            )

                        pLV.append(pItem);
                    }

                }

                pLV;//.listview('refresh');
                $('#popEmployeeTelInfo').modal('hide');
            }

            function EditEmployeeTelInfo(arrindex) {
                $('#popEmployeeTelInfo').modal('show');

                EmployeeTelRankNo = arrEmployeeTelInfoList[arrindex].TelRankNo;
                EmployeeTelState = "Edit";
                sessionStorage["Telindex"] = arrindex;
                sessionStorage["TelIsNew"] = arrEmployeeTelInfoList[arrindex].IsNew;
                sessionStorage["TelState"] = arrEmployeeTelInfoList[arrindex].TelState;
                $('#txtEmployeeTelID').val(arrEmployeeTelInfoList[arrindex].TelID == undefined ? "" : arrEmployeeTelInfoList[arrindex].TelID);
                $('#txtEmployeeTelAreaCodeID').val(arrEmployeeTelInfoList[arrindex].AreaCodeID);
                $('#txtEmployeeTelCountryID').val(arrEmployeeTelInfoList[arrindex].CountryCodeID);
                $('#txtEmployeeCountryCode').text(arrEmployeeTelInfoList[arrindex].CountryCode);
                $('#txtEmployeeAreaCode').text(arrEmployeeTelInfoList[arrindex].AreaCode);
                $('#txtEmployeeTelephoneNum').val(arrEmployeeTelInfoList[arrindex].TelephoneNo);
                $('#txtEmployeeTelephoneExt').val(arrEmployeeTelInfoList[arrindex].ExtensionNo);
                $('#selEmployeeTelType').val(arrEmployeeTelInfoList[arrindex].ContactType)//.selectmenu('refresh');
            }

            function ClearEmployeeTelInfo() {
                $('#txtEmployeeTelID').val('');
                $('#txtEmployeeTelAreaCodeID').val('');
                $('#txtEmployeeTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtEmployeeCountryCode').text('PHL - 63');
                $('#txtEmployeeAreaCode').text('Select');
                $('#txtEmployeeTelephoneNum').val('');
                $('#txtEmployeeTelephoneExt').val('');
                $('#selEmployeeTelType').val('')//.selectmenu('refresh');
            }

            function SearchWorkSchedule(type, callback) {
                BrowsePopup('Browser/LibraryDTRSchedule', 'Select Work Schedule', [
                   {
                       AutoSearch: true,
                       DisplayText: [{ Column: 'ScheduleName' }],
                       Click: function (e) {
                           callback(e);
                       }
                   }
                ], { Type: type });
            }

            function GetDeminimis(ID) {

                data = {
                    PayTypeID: paytypeid
                }

                AjaxSend({
                    url: 'Organization/GetDeminimisByPayType',
                    data: data,
                    success: function (vData) {
                        DisplayDeminimis(vData)
                    }
                });
            }

            function DisplayDeminimis(vData) {
                var tbl = $('#tblDeminimisBody');

                tbl.empty();

                for (i = 0; i <= vData.Deminimis.Rows.length - 1; i++) {

                    var data = vData.Deminimis.Rows[i];

                    var val = CDec2(vData.Deminimis.Rows[i].Amount);

                    tbl.append($('<tr>')
                        .data('data', data)
                        .append($('<td>')
                            .css('text-align', 'center')
                            .text(data.AllowanceName)
                        )
                        .append($('<td>')
                        .append($('<input type=number value=' + val + ' />')
                            .attr('class', 'form-control')
                            )
                        )//td
                        .append($('<td>')
                            .css('text-align', 'center')
                            .text(data.MaxAmount)
                        )
                        .append($('<td>')
                            .css('text-align', 'center')
                            .text(data.FrequencyofAllowance)
                        )
                        .append($('<td>')
                            .css('text-align', 'center')
                            .append($('<a data-role="button" data-theme="b" data-icon="delete" data-iconpos="notext">')
                            .click(function () {
                                $(this).closest('tr').remove();
                            })
                            )
                        )
                        );
                }
            }


            function GetWorkScheduleDetails(ID) {
                AjaxSend({
                    url: 'Organization/GetWorkScheduleDetails',
                    data: { ID: ID },
                    success: function (vData) {
                        var vRows = vData.Results.Rows
                        var tbody = $('#tblWorkScheduleBody')

                        tbody.empty();

                        for (var i = 0; i <= vRows.length - 1; i++) {
                            tbody.append($('<tr>')
                            .append($('<td>')
                                .text(vRows[i].ScheduleName)
                            )
                            .append($('<td>')
                                .text(vRows[i].ScheduleDescription)
                            )
                            .append($('<td>')
                                .text(vRows[i].Type)
                            )
                            .append($('<td>')
                                .text(vRows[i].TimeFrom)
                            )
                            .append($('<td>')
                                .text(vRows[i].TimeTo)
                            )
                            .append($('<td>')
                                .text(
                                    (vRows[i].Monday == 1 ? 'Monday, ' : '') + 
                                    (vRows[i].Tuesday == 1 ? 'Tuesday, ' : '') + 
                                    (vRows[i].Wednesday == 1 ? 'Wednesday, ' : '') + 
                                    (vRows[i].Thursday == 1 ? 'Thursday, ' : '') + 
                                    (vRows[i].Friday == 1 ? 'Friday, ' : '') + 
                                    (vRows[i].Saturday == 1 ? 'Saturday, ' : '') + 
                                    (vRows[i].Sunday == 1 ? 'Sunday, ' : '')
                                )
                            )
                            )
                        }
                        
                    }
                });
            }

            function GetPayGradeDetails(ID) {
                AjaxSend({
                    url: 'Organization/GetPayGradeDetails',
                    data: { ID: ID },
                    success: function (vData) {
                        var vRows = vData.Results.Rows
                        var tbody = $('#tblPayGradeBody')

                        tbody.empty();

                        for (var i = 0; i <= vRows.length - 1; i++) {
                            tbody.append($('<tr>')
                            .append($('<td>')
                                .text(vRows[i].SalaryGradeName)
                            )
                            .append($('<td>')
                                .text(vRows[i].PayType)
                            )
                            .append($('<td>')
                                .text((CDec2(vRows[i].HoursPerDayRate) == 0 ?  AccountingFormat(vRows[i].MonthlyRate) : AccountingFormat(vRows[i].HoursPerDayRate)))
                            )
                            .append($('<td>')
                                .text(vRows[i].HoursPerDay)
                            )                            
                            )
                        }

                    }
                });
            }

            function SearchPayType(type, callback) {
                BrowsePopup('Browser/LibrarySalaryGrade', 'Select Pay', [
                   {
                       AutoSearch: true,
                       DisplayText: [{ Column: 'SalaryGradeName' }],
                       Click: function (e) {
                           callback(e);
                       }
                   }
                ], { Type: type });
            }

            function SearchDeminimis(type, callback) {
                BrowsePopup('Browser/LibraryDeminimis', 'Select De Minimis', [
                   {
                       AutoSearch: true,
                       DisplayText: [{ Column: 'Name' }],
                       Click: function (e) {
                           callback(e);
                       }
                   }
                ], { Type: type });
            }

            function SearchPersonOrgRelationship(type, callback) {
                var title;

                if(type == 'manpower'){
                    title = 'Manpower Agencies';
                } else if(type == 'partner'){
                    title = 'Vendor/ Trading Partner';
                } else if(type == 'co-user'){
                    title = 'Co-User Organizations';
                }

                BrowsePopup('Organization/SearchPersonOrganizationRelationship', 'Select ' + title, [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'SupplierName' }],
                        Click: function (e) {
                            callback(e);
                        }
                    }
                ], { Type: type });
            }

            function GetRoleTag(callback) {
                var divEmpRoleTags = $('#divEmpRoleTags');

                divEmpRoleTags.empty()

                AjaxSend({
                    url: 'Organization/GetEmployeeDesignation',
                    data: { Search: $.trim($('#txtRoleSearch').val()) },
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                divEmpRoleTags.append($('<div class="form-check mb-3" style="font-size:20px;">')
                                    .append($('<label for="' + vRows[i].ID + '" class="form-check-label">')
                                        .text(vRows[i].RoleName)
                                    )
                                    .append($('<input type="radio" name="radRoletag" id="' + vRows[i].ID + '" value="' + vRows[i].ID + '" class="form-check-input">')
                                        .data('data', vRows[i])
                                    )
                                )
                            }
                        }

                        $('[name="radRoletag"]');//.checkboxradio();//.checkboxradio('refresh');

                        callback();
                    }
                })
            }

            function SearchEmployeePopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryPeople',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].PeopleName)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            sessionStorage.txtOrgPeopleID = $(this).attr('ID');
                                            sessionStorage.btnOrgEmployeeName = $(this).data('data').PeopleName;
                                            sessionStorage.EmpFirstName = $(this).data('data').PeopleFirstName;
                                            $('#txtOrgPeopleID').val($(this).attr('ID'));
                                            $('#txtOrgEmployeeNew').val($(this).data('data').PeopleName);
                                            $('#btnOrgEmpInfoSaveAndInvite').html($(this).data('data').PeopleFirstName + '\'s Mobiz app');
                                            $('#btnOrgEmpInfoSaveAndConfirm').html($(this).data('data').PeopleFirstName + ' sign in and confirm now');
                                            $('#btnOrgEmpInfoSendConfirmation').html($(this).data('data').PeopleFirstName + '\'s Email');
                                            $('.peopleFirstName').text($(this).data('data').PeopleFirstName);
                                            $('#panSearchLibrary').popup('close');
                                            $('#txtSearchKeyword').val('');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }

            function CheckPeopleEmailAddress() {
                var pData = { PeopleID: $('#txtOrgPeopleID').val() }

                AjaxSend({
                    url: 'Organization/getPeopleEmailAddress',
                    data: pData,
                    success: function (getResult) {
                        if (getResult.PeopleEmailAddress.Rows.length >= 1) {
                            SaveOrganizationEmployee();
                            PeopleEmailAddress = getResult.PeopleEmailAddress.Rows[0].EmailAddress;
                        }
                        else {
                            //                            $('#panOrgEmpMessage').popup('close');
                            //                            $('#panAddEmail').popup('open');
                            //                            $('.peopleFirstName').text(sessionStorage.EmpFirstName);
                            //                            $('#btnSaveAndSend').text('Save and Send Employment Confirmation').unbind('click').click(function (e) {
                            //                                e.preventDefault();
                            //                                SaveEmpEmailAddress();
                            //                                $('#panAddEmail').popup('close');
                            //                            })
                            //                            $('#btnCancelEmail').unbind('click').click(function (e) {
                            //                                e.preventDefault();
                            //                                $('#panAddEmail').popup('close');
                            //                            });
                            NewMessageBox('There is no existing email address on record for <strong>' + sessionStorage.EmpFirstName + '</strong>' +
                                '. Please enter his/her email address on the email entry field below: <br><br>' +
                                '<input type="text" placeholder="Enter email address" id="txtEmpEmailAddress">',
                                'Employee Email Address',
                                [
                                    {
                                        Text: 'Save and Send Confirmation',
                                        Click: function () {
                                            SaveEmpEmailAddress();
                                        }
                                    },
                                    {
                                        Text: 'Cancel'
                                    }
                                ]
                            )
                        }
                    }
                })
            }

            function validateEmail(email) {
                var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

                return re.test(email);
            }

            function SaveEmpEmailAddress() {
                var pData = { PeopleID: $('#txtOrgPeopleID').val(), EmailAddress: $('#txtEmpEmailAddress').val() }

                if (!validateEmail($.trim($('#txtEmpEmailAddress').val()))) {
                    NewMessageBox('Enter a valid email address', 'Email Address', [
                        {
                            Text: 'Ok',
                            Click: function () {
                                $('#txtEmpEmailAddress').focus().val('');
                                CheckPeopleEmailAddress();
                            }
                        }
                    ])
                }
                else {
                    AjaxSend({
                        url: 'Organization/SaveEmpEmailAddress',
                        data: pData,
                        success: function (vData) {
                            if (vData.SaveEmailResult == "Saved") {
                                $('#panAddEmail').popup('close');
                                SaveOrganizationEmployee();
                                PeopleEmailAddress = $('#txtEmpEmailAddress').val();
                            }
                            else {
                                $('#panAddEmail').popup('close');
                                alert("There was an error occured.")
                            }
                        }
                    })
                }
            }

            function SendConfirmationViaEmail() {
                var pData = { PeopleID: $('#txtOrgPeopleID').val() }

                //                $('#panOrgEmpMessage').popup('close');
                //                $('#TitleOrgEmp').text('Employment Confirmation');
                //                $('#MsgOrgEmp').text('Sending Employment Confirmation via email. Please wait ...');
                //                $('#panOrgEmpMessage').popup('open');
                //                $('#btnOKOrgEmp').hide();

                NewMessageBox('Sending Employment Confirmation via email. Please wait ...', 'Employment Confirmation', [
                    {
                        Text: 'Hide'
                    }
                ])

                AjaxSend({
                    url: 'Organization/getPeopleOrgID',
                    data: pData,
                    success: function (vData) {
                        if (vData.Result.Rows.length >= 1) {
                            AjaxSend({
                                url: 'Organization/SendConfirmationViaEmail',
                                data: {
                                    PeopleEmailAddress: PeopleEmailAddress,
                                    PeopleName: sessionStorage.btnOrgEmployeeName,
                                    ConfirmKeyID: sessionStorage.OrgID,
                                    PeopleID: $('#txtOrgPeopleID').val(),
                                    OrgName: sessionStorage.OrgName,
                                    Product: (GetUrlParameters().mp == undefined ? "" : GetUrlParameters().mp)
                                },
                                success: function (sendResult) {
                                    if (sendResult.PeopleEmailAddress == "Sent") {
                                        NewMessageBox('An employment confirmation link has been emailed to ' + sessionStorage.EmpFirstName + '. <br> <br>' + sessionStorage.EmpFirstName + ' should receive the email in 5-10 minutes depending on your internet speed. <br><br> Please tell ' + sessionStorage.EmpFirstName + ' to check the Junk or Spam folder in case the “Employment Confirmation” email is not found in the inbox.',
                                            'Employment Confirmation',
                                            [
                                                {
                                                    Text: 'Ok',
                                                    Click: function () {
                                                        window.location.hash = 'LibraryPage';
                                                        if (sessionStorage.SystemAdminFor == 'dtr') {
                                                            DTRLibraryTable.Employees();
                                                        } else {
                                                            if (!CBool(sessionStorage.IsCrm)) {
                                                                $('#tblBody').html('');
                                                                LibraryTable.Employees();
                                                            } else {
                                                                CRMLibraryTable.Employees();
                                                            }
                                                        }                                                                                                                
                                                    }
                                                }
                                            ]
                                        )
                                    }
                                    else {
                                        NewMessageBox('This feature requires internet connection.', 'Requires Internet Connection',
                                            [
                                                {
                                                    Text: 'Ok'
                                                }
                                            ]
                                        );
                                    }
                                }
                            })
                        }
                        else {
                            NewMessageBox(sessionStorage.EmpFirstName + ' was already employed.', 'Employee');
                        }
                    }
                })

            }

            function GetOrgName() {
                AjaxSend({
                    url: 'Organization/getOrganizationName',
                    success: function (vData) {
                        OrgName = vData.OrganizationName;
                    }
                });
            }

            function VerifyUsernameAndPassword() {

                var data = { Username: $('#txtUsername').val(),
                    UserPassword: $('#txtPassword').val()
                };
                AjaxSend({
                    url: 'People/VerifyPeopleUserAccount',
                    data: data,
                    success: function (vData) {
                        if (vData.PeopleUserAccountData == $("#txtOrgPeopleID").val()) {
                            IsConfirm = 1;
                            SaveOrganizationEmployee();
                        }
                        else {
                            NewMessageBox('Invalid Username and/or Password.', 'Access Denied')
                        }
                    }
                });
            }

            function GetLibraryFowst() {
                var IsFOWSTExist = false;

                Browser.LibraryFowst.Click = function (e) {

                    $('#lstOrgEmployeeFowst a').each(function (m_index) {
                        if ($.trim($(this).attr('ID')) == e.ID) {
                            IsFOWSTExist = true;
                        }
                    });

                    if (IsFOWSTExist) return;

                    var pLV = $("#lstOrgEmployeeFowst");

                    pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                .attr('ID', e.ID)
                                .append($('<table style="width:100%;">')
                                    .append($('<tr>')
                                        .append($('<td style="word-break:break-all;">')
                                            .data('data', e)
                                            .append($('<h5>')
                                                .attr("style", "white-space:normal")
                                                .text(e.FowstName)
                                            )
                                            .append($('<div>')
                                                .text(e.Address)
                                            )
                                        )
                                        .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                .attr('fID', e.ID)
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    $('#' + $(this).attr('fID')).remove();
                                                })
                                            )
                                        )
                                    )
                                )
                    pLV.append(pItem);

                    pLV;//.listview('refresh');
                };
                Browser.LibraryFowst.Show();
            }

            function GetSecurityRole() {
                var IsRoleExist = false;

                Browser.LibrarySecurityRole.Click = function (e) {

                    $('#lstOrgEmployeeRole li a').each(function (m_index) {
                        if ($.trim($(this).attr('ID')) == e.ID) {
                            IsRoleExist = true;
                        }
                    });

                    if (IsRoleExist) return;

                    var pLV = $("#lstOrgEmployeeRole");

                    pItem = $('<li>')
                    .attr('ID', e.ID)
                    .attr('href', "#")
                    .append($('<a>')
                        .attr('ID', e.ID)
                        .data('data', e)
                        .click(function (e) {
                        })
                        .append($('<h3>')
                            .text(e.Code + ' - ' + e.Role)
                        )
                        .append($('<p>')
                            .text('Description : ' + e.Decription)
                        )
                    )
                    .append($('<a>')
                        .attr('ID', e.ID)
                        .attr('href', "#")
                        .click(function () {
                            $('#' + $(this).attr('ID')).remove();
                        })
                    )
                    pLV.append(pItem);

                    pLV;//.listview('refresh');
                };
                Browser.LibrarySecurityRole.Show();
            }

            function SearchFowst(s) {
                var pData = {};
                pData.keywords = s;
                AjaxSend({
                    url: 'Browser/LibraryFowst',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].FowstName)
                                        )
                                        .append($('<p>')
                                            .text(vData.Results.Rows[p].Address)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            AddToFowstList($(this).data('data').ID, $(this).data('data').FowstName, $(this).data('data').Address);
                                            $('#txtSearchKeyword').val('');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }

            function AddToFowstList(id, name, address) {
                var IsRoleExist = false;

                $('#lstOrgEmployeeFowst li').each(function (m_index) {
                    if ($.trim($(this).attr('ID')) == id) {
                        IsRoleExist = true;
                    }
                });

                if (IsRoleExist) {
                    WebToast(name + ' already exist in a list.', 'top');
                    return;
                }

                //$('#panSearchLibrary').popup('close');
                $('#lstOrgEmployeeFowst').append($('<li class="list-group-item list-group-item-action">')
                                .attr('ID', id)
                                .append($('<table style="width:100%;">')
                                    .append($('<tr>')
                                        .append($('<td style="width:50px; text-align:center;">')
                                            .append($('<input data-role="none" type="checkbox" class="form-check-input" name="chbEmpFowst">')
                                                .data('id', id)
                                            )
                                        )
                                        .append($('<td style="word-break:break-all;">')
                                            .append($('<h5>')
                                                .attr("style", "white-space:normal")
                                                .text(name)
                                            )
                                            .append($('<div>')
                                                .text(address)
                                            )
                                        )
                                        .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                .attr('fID', id)
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    $('#' + $(this).attr('fID')).remove();
                                                })
                                            )
                                        )
                                    )
                                ));
                $('#lstOrgEmployeeFowst');//.listview('refresh');
            }

            function SearchRole(s) {
                var pData = {};
                pData.keywords = s;
                AjaxSend({
                    url: 'Browser/LibrarySecurityRole',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Code)
                                        )
                                        .append($('<p>')
                                            .text('Description: ' + vData.Results.Rows[p].Description)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            AddToRoleList($(this).data('data').ID, $(this).data('data').Code, $(this).data('data').Description);
                                            $('#txtSearchKeyword').val('');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }

            function AddToRoleList(id, code, description) {
                var IsRoleExist = false;

                $('#lstOrgEmployeeRole a').each(function (m_index) {
                    if ($.trim($(this).attr('ID')) == id) {
                        IsRoleExist = true;
                    }
                });

                if (IsRoleExist) {
                    alert(code + ' already exist in a list.')
                    return;
                }

                //$('#panSearchLibrary').popup('close');
                $('#lstOrgEmployeeRole').append($('<a href="#" class="list-group-item list-group-item-action">')
                                .attr('ID', id)
                                .append($('<table style="width:100%;">')
                                    .append($('<tr>')
                                        .append($('<td style="word-break:break-all;">')
                                            .append($('<h5>')
                                                .attr("style", "white-space:normal")
                                                .text(code)
                                            )
                                            .append($('<div>')
                                                .text('Description: ' + description)
                                            )
                                        )
                                        .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                .attr('fID', id)
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    $('#' + $(this).attr('fID')).remove();
                                                })
                                            )
                                        )
                                    )
                                ));
                $('#lstOrgEmployeeRole');//.listview('refresh');
            }

            function GetAccessRole() {
                AjaxSend({
                    url: 'Browser/LibrarySecurityRole',
                    success: function (vData) {
                        DisplayAccessRole(vData.Results.Rows);
                    }
                });
            }

            function DisplayAccessRole(vRows) {
                var pLV = $("#lstOrgEmployeeRole");

                pLV.empty();

                for (var i = 0; i <= 0; i++) {
                    pItem = $('<li>')
                    .attr('ID', vRows[i].ID)
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .append($('<br><p style="font-weight:bold;">')
                            .text(vRows[i].Code)
                        )
                        .append($('<p>')
                            .text('Description: ' + vRows[i].Description)
                        )
                    )
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .click(function () {
                            $('#' + $(this).attr('ID')).remove();
                            $('#lstOrgEmployeeRole');//.listview('refresh');
                        })
                    )
                    pLV.append(pItem)
                }
                pLV;//.listview('refresh');
            }

            function GetAllFowst() {
                if (window.location.pathname == '/SetupChecklist/SystemAdministration') {
                    AjaxSend({
                        url: 'Browser/LibraryFowst',
                        success: function (vData) {
                            DisplayFOWST(vData.Results.Rows);
                        }
                    });
                }                
            }

            function DisplayFOWST(vRows) {
                var pLV = $("#lstOrgEmployeeFowst");

                pLV.empty();

                for (var i = 0; i <= vRows.length - 1; i++) {
                    pItem = $('<li class="list-group-item list-group-item-action">')
                                .attr('ID', vRows[i].ID)
                                .append($('<table style="width:100%;">')
                                    .append($('<tr>')
                                        .append($('<td style="width:50px; text-align:center;">')
                                            .append($('<input data-role="none" type="checkbox" class="form-check-input" name="chbEmpFowst">')
                                                .data('id', vRows[i].ID)
                                            )
                                        )
                                        .append($('<td style="word-break:break-all;">')                                            
                                            .data('data', vRows[i])
                                            .append($('<h5>')
                                                .attr("style", "white-space:normal")
                                                .text(vRows[i].FowstName)
                                            )
                                            .append($('<div>')
                                                .text(vRows[i].Address)
                                            )
                                            .click(function (e) {
                                                e.preventDefault();
                                                //                            window.location = MergeUrlParameters({
                                                //                                OrgID: sessionStorage.OrgID,
                                                //                                IsMobile: CBool(IsMobile) ? 1 : 0,
                                                //                                EmployeeName: $('#btnOrgEmployee').text(),
                                                //                                EmployeeID: ID,
                                                //                                FOWSTID: $(this).attr('ID'),
                                                //                                FOWSTName: $(this).text()
                                                //                            }, 'OrganizationEmployeeAssignWorkSchedule');
                                            })
                                        )
                                        .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                .attr('fID', vRows[i].ID)
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    $('#' + $(this).attr('fID')).remove();
                                                })
                                            )
                                        )
                                    )
                                )
                    pLV.append(pItem)
                }

                pLV;//.listview('refresh');
            }

            function DisplaySecurityRole(vRows) {
                var pLV = $("#lstOrgEmployeeRole");

                pLV.empty();

                for (var i = 0; i <= vRows.length - 1; i++) {
                    pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                .attr('ID', vRows[i].ID)
                                .append($('<table style="width:100%;">')
                                    .append($('<tr>')
                                        .append($('<td style="word-break:break-all;">')
                                            .data('data', vRows[i])
                                            .append($('<h5>')
                                                .attr("style", "white-space:normal")
                                                .text(vRows[i].Code + ' - ' + vRows[i].Role)
                                            )
                                            .append($('<div>')
                                                .text('Description : ' + vRows[i].Description)
                                            )
                                        )
                                        .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                .attr('fID', vRows[i].ID)
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    $('#' + $(this).attr('fID')).remove();
                                                })
                                            )
                                        )
                                    )
                                )
                    pLV.append(pItem)
                }

                pLV;//.listview('refresh');
            }

            function ShowHideOrgInActiveDetails() {

                if ($("#selOrgDeactivate").is(':checked')) {
                    $("#divOrgDeactivateDetails").show();
                }
                else {
                    $("#divOrgDeactivateDetails").hide();
                }
            }

            function getOrgEmployeeList() {
                var pData = {};
                pData.SearchKeyword = $("#txtOrgEmployeeSearch").val();

                AjaxSend({
                    url: 'Organization/getOrganizationEmpList',
                    data: pData,
                    success: function (vData) {

                        LoadOrgEmpList(vData.OrgEmployeeData.Rows);
                    }
                });
            }

            function LoadOrgEmpList(vRows) {
                var pLV = $('#lstOrgEmployee');
                var pItem;

                $("#divOrgRecordCountEmployee").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .click(function (e) {
                            e.preventDefault();
                            //$.mobile.changePage('#pageOrgEmployeeInfo');
                            $('#pageOrgEmployee').hide();
                            $('#pageOrgEmployeeInfo').show();
                            $('#pageOrgEmployeeSignIn').hide();
                            $('#pageOrgEmployeeAdd').hide();
                            ShowOrgEmployeeInfo($(this).attr('ID'));
                            ID = $(this).attr('ID');
                            IsNew = 0;
                        })
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].PeopleName)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Date Hired: " + $.format.date(new Date(vRows[p].DateHired), 'MMM dd, yyyy'))
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Status: " + (vRows[p].DeActivated != '0' ? 'Deactivated ( ' + $.format.date(new Date(vRows[p].DeactivationDate), 'MMM dd, yyyy') + ' )' : 'Active'))
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Banned: " + (vRows[p].IsBan != '0' ? 'Yes' : 'No'))
                        )
                    );

                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            };

            function ShowOrgEmployeeInfo(strID) {
                AjaxSend({
                    url: "SecurityRole/CheckSecurityCurrentUserAcccess",
                    data: { ModuleName: 'Allow to reset employee Mobiz\'s account' },
                    success: function (vData) {
                        if (vData.SecurityRoleData.Rows.length > 0) {
                            $('#liEmployeeResetMobizAcc').show();
                        } else $('#liEmployeeResetMobizAcc').remove();

                        AjaxSend({
                            url: "SecurityRole/CheckSecurityCurrentUserAcccess",
                            data: { ModuleName: 'Grant access to edit an employee\'s profile' },
                            success: function (vData) {
                                if (vData.SecurityRoleData.Rows.length == 0) {
                                    isEditEmpName = false;
                                    $('#divEditEmpName').remove();
                                    $('#divbtnOld').show();
                                } else {
                                    isEditEmpName = true;
                                    $('#divEditEmpName').show();
                                    $('#divbtnOld, #divbtnNew').remove();
                                }

                                var pData = {};
                                pData.ID = strID;
                                pData.IsCustomer = 0;

                                ClearOrgEmployeeFields();
                                ClearOrgEmployeeInActiveFields();

                                AjaxSend({
                                    url: "Organization/getOrganizationEmpInfo",
                                    data: pData,
                                    success: function (vData) {
                                        arrEmployeeTelInfoList = vData.Telephone.Rows;
                                        arrEmployeeMobileInfoList = vData.Mobile.Rows;
                                        arrEmployeeEmailInfoList = vData.Email.Rows;
                                        AddEmployeeTelInfo();
                                        AddEmployeeMobileInfo();
                                        AddEmployeeEmailInfo();

                                        if (vData.OrgEmployeeData.Rows.length > 0) {
                                            $("#txtOrgOrgEmpID").val(vData.OrgEmployeeData.Rows[0].ID || '');

                                            $("#txtOrgPeopleID").val(vData.OrgEmployeeData.Rows[0].PeopleID || '');
                                            //$("#btnOrgEmployee").text(vData.OrgEmployeeData.Rows[0].PeopleName || 'Please Select');//.button('refresh');
                                            $("#txtOrgEmployee").val(vData.OrgEmployeeData.Rows[0].PeopleName);
                                            $("#txtOrgEmployeeFirstName").val(vData.OrgEmployeeData.Rows[0].FirstName);
                                            $("#txtOrgEmployeeMiddleName").val(vData.OrgEmployeeData.Rows[0].MiddleName);
                                            $("#txtOrgEmployeeLastName").val(vData.OrgEmployeeData.Rows[0].Surname);

                                            if (CStr2(vData.OrgEmployeeData.Rows[0].ThirdPartySystemName) != '') {
                                                $('#txtOrg3rdPartySystemName').val(CStr2(vData.OrgEmployeeData.Rows[0].ThirdPartySystemName));
                                                $('#txtOrg3rdPartySystemID').val(CStr2(vData.OrgEmployeeData.Rows[0].ThirdPartySystemID));
                                            } else {
                                                $('#txtOrg3rdPartySystemName').val('');
                                                $('#txtOrg3rdPartySystemID').val('');
                                            }
                                            $('#txtOrg3rdPartySystemID').val(CStr2(vData.OrgEmployeeData.Rows[0].ThirdPartySystemID));

                                            $('[name="payfrequency"][value="' + CStr2(vData.OrgEmployeeData.Rows[0].PayFrequency) + '"]').prop('checked', true);//.checkboxradio('refresh');

                                            $("#selOrgDateHiredMonth2").val(vData.OrgEmployeeData.Rows[0].DateHiredMonth || '');//.selectmenu('refresh');
                                            $("#selOrgDateHiredYear2").val(vData.OrgEmployeeData.Rows[0].DateHiredYear || '');//.selectmenu('refresh');
                                            setDayCollection('selOrgDateHiredMonth2', 'selOrgDateHiredDay2', 'selOrgDateHiredYear2', false);
                                            $("#selOrgDateHiredDay2").val(vData.OrgEmployeeData.Rows[0].DateHiredDay || '');//.selectmenu('refresh');

                                            $("#selOrgDeactivate").prop('checked', CInt2(vData.OrgEmployeeData.Rows[0].DeActivated || '0'));//.slider("refresh");
                                            $('[name="radRoletag"][value="' + vData.OrgEmployeeData.Rows[0].RoleTagID + '"]').prop('checked', true);//.checkboxradio('refresh')

                                            if ($("#selOrgDeactivate").is(':checked')) {
                                                $("#selOrgDateInActiveMonth").val(vData.OrgEmployeeData.Rows[0].DeactivationDateMonth || '');//.selectmenu('refresh');
                                                $("#selOrgDateInActiveYear").val(vData.OrgEmployeeData.Rows[0].DeactivationDateYear || '');//.selectmenu('refresh');
                                                setDayCollection('selOrgDateInActiveMonth', 'selOrgDateInActiveDay', 'selOrgDateInActiveYear', false);
                                                $("#selOrgDateInActiveDay").val(vData.OrgEmployeeData.Rows[0].DeactivationDateDay || '');//.selectmenu('refresh');

                                                $("#txtOrgInActiveReason").val(vData.OrgEmployeeData.Rows[0].DeactivationReason || '').keyup();
                                                $("#txtOrgComments").val(vData.OrgEmployeeData.Rows[0].Comments || '').keyup();

                                                $("#selOrgBan").prop('checked', CInt2(vData.OrgEmployeeData.Rows[0].IsBan || '0'));//.slider("refresh");
                                            }

                                            DepartmentID = CStr2(vData.OrgEmployeeData.Rows[0].DepartmentID);
                                            SystemGeneratedID = CStr2(vData.OrgEmployeeData.Rows[0].SystemGeneratedID);
                                            CompanyID = CStr2(vData.OrgEmployeeData.Rows[0].CompanyID);
                                            TeamID = CStr2(vData.OrgEmployeeData.Rows[0].TeamID)

                                            $('#txtSystemGeneratedID').val(SystemGeneratedID);
                                            $('#txtCompanyID').val(CompanyID);

                                            $('#txtDepartment').val(CStr2(vData.OrgEmployeeData.Rows[0].Department));
                                            $('#txtTeam').val(CStr2(vData.OrgEmployeeData.Rows[0].Team));

                                            GetEmployeeFowst(vData.OrgEmployeeData.Rows[0].PeopleID);
                                            GetEmployeeSecurityRole(vData.OrgEmployeeData.Rows[0].PeopleID);
                                            GetEmployeeWorkSchedule(vData.OrgEmployeeData.Rows[0].PeopleID)
                                            GetEmployeePayType(vData.OrgEmployeeData.Rows[0].PeopleID)
                                            GetEmployeeIDs(vData.OrgEmployeeData.Rows[0].PeopleID)
                                            GetEmployeeBankAccount(vData.OrgEmployeeData.Rows[0].PeopleID)
                                            ShowHideOrgInActiveDetails();
                                            GetMyMobizAccount(vData.OrgEmployeeData.Rows[0].PeopleID);

                                            //                            $('#btnOrgEmpInfoSave').show()
                                            //                            $('#btnOrgEmpInfoCancel').show()
                                            //                            $('#btnOrgEmpInfoSaveAndInvite').hide()
                                            //                            $('#btnOrgEmpInfoSaveAndConfirm').hide();
                                            //                            $('#btnOrgEmpInfoSendConfirmation').hide();

                                        }

                                        if (vData.Relations.Rows.length > 0) {
                                            var rel = vData.Relations.Rows[0];
                                            $('#selSelfEmployed').prop('checked', CInt2(rel.IsSelfEmployed));//.slider('refresh');
                                            $('#selManPower').prop('checked', CInt2(rel.IsDirectHired));//.slider('refresh');

                                            if (!CBool(rel.IsDirectHired) && !CBool(rel.IsSelfEmployed)) {
                                                $('#divManPower, #divOtherRelationship').show();

                                                if (rel.DirectEmployerIDDescription == 'manpower') {
                                                    manpowerid = rel.DirectEmployerID;
                                                    $('#btnEmpManPower').text(rel.DirectEmployerName);//.button('refresh');
                                                } else if (rel.DirectEmployerIDDescription == 'vendor') {
                                                    vendorid = rel.DirectEmployerID;
                                                    $('#selVendorTradingPartner').prop('checked', 1);//.slider('refresh');
                                                    $('#btnEmpVendorTradingPartner').text(rel.DirectEmployerName);//.button('refresh');
                                                    $('#divVenderTradingPartner').show();
                                                } else if (rel.DirectEmployerIDDescription == 'couser') {
                                                    couserid = rel.DirectEmployerID;
                                                    $('#selEmpCoUser').prop('checked', 1);//.slider('refresh');
                                                    $('#btnEmpCoUser').text(rel.DirectEmployerName);//.button('refresh');
                                                    $('#divCoUserOrgName').show();
                                                }

                                                if (rel.ManpowerAgencyClientIDDescription == 'vendor') {
                                                    vendorid = rel.ManpowerAgencyClientID;
                                                    $('#selVendorTradingPartner').prop('checked', 1);//.slider('refresh');                                    
                                                    $('#btnEmpVendorTradingPartner').text(rel.ManpowerAgencyName);//.button('refresh');
                                                    $('#divVenderTradingPartner').show();
                                                }

                                                if (rel.ManpowerAgencyClientIDDescription == 'couser') {
                                                    couserid = rel.ManpowerAgencyClientID;
                                                    $('#selEmpCoUser').prop('checked', 1);//.slider('refresh');                                    
                                                    $('#btnEmpCoUser').text(rel.ManpowerAgencyName);//.button('refresh');
                                                    $('#divCoUserOrgName').show();
                                                }
                                            } else {
                                                $('#divManPower, #divOtherRelationship').hide();
                                            }
                                        }
                                    }
                                });
                            }
                        });
                    }
                })                
            };

            function GetMyMobizAccount(EmployeeID) {
                data = { EmployeeID: EmployeeID }

                AjaxSend({
                    url: 'Organization/GetMyMobizAccount',
                    data: data,
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        if (vRows.length > 0) {
                            $('#txtEmployeeCurrentUsername').val(vRows[0].Username);

                            if (CStr2(vRows[0].ActivationDate) != '') {
                                $('#lblMobizAccountInfo').text('Mobiz account was created on ' + vRows[0].ActivationDate);
                            }

                            if (CStr2(vRows[0].LastUpdated) != '') {
                                $('#lblMobizAccountInfo').append('<br>Last Updated on ' + vRows[0].LastUpdated);
                            }

                            if (CStr2(vRows[0].ApprovedByName) != '') {
                                $('#lblMobizAccountInfo').append(' by ' + vRows[0].ApprovedByName);
                            }
                        }
                    }
                });
            }

            function GetEmployeeFowst(EmployeeID) {
                data = { EmployeeID: EmployeeID }

                AjaxSend({
                    url: 'Organization/GetEmployeeFowst',
                    data: data,
                    success: function (vData) {
                        DisplayFOWST(vData.Results.Rows);
                    }
                });
            }

            function GetEmployeeSecurityRole(EmployeeID) {
                data = { EmployeeID: EmployeeID }

                AjaxSend({
                    url: 'Organization/GetEmployeeSecurityRole',
                    data: data,
                    success: function (vData) {
                        DisplaySecurityRole(vData.Results.Rows);
                    }
                });
            }

            function GetEmployeeWorkSchedule(EmployeeID) {
                data = { EmployeeID: EmployeeID }

                AjaxSend({
                    url: 'Organization/GetEmployeeWorkSchedule',
                    data: data,
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {
                            workscheduleid = vData.Results.Rows[0].ID
                            GetWorkScheduleDetails(workscheduleid)
                            $('#btnEmpWorkSchedule').text(vData.Results.Rows[0].ScheduleName);//.button('refresh');
                        }                        
                    }
                });
            }

            function GetEmployeePayType(EmployeeID) {
                data = { EmployeeID: EmployeeID }

                AjaxSend({
                    url: 'Organization/GetEmployeePayType',
                    data: data,
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {
                            paytypeid = vData.Results.Rows[0].ID
                            GetPayGradeDetails(paytypeid);
                            $('#btnEmpPayType').text(vData.Results.Rows[0].SalaryGradeName);//.button('refresh');
                        }

                        if (vData.Deminimis != undefined) {
                            if (vData.Deminimis.Rows.length > 0) {

                                DisplayDeminimis(vData);

                            }
                        }
                    }
                });
            }
            
            function GetEmployeeIDs(EmployeeID) {
                data = { EmployeeID: EmployeeID }

                AjaxSend({
                    url: 'Organization/GetEmployeeIDs',
                    data: data,
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {
                            var vRows = vData.Results.Rows;

                            for (var i = 0; i <= vRows.length - 1; i++) {
                                if (vRows[i].IDType == 'PagIbig') {
                                    $('#txtPagIbigID').val(vRows[i].IDNo);
                                } else if (vRows[i].IDType == 'TIN') {
                                    $('#txtTINID').val(vRows[i].IDNo);
                                } else if (vRows[i].IDType == 'SSS') {
                                    $('#txtSSSID').val(vRows[i].IDNo);
                                } else if (vRows[i].IDType == 'PhilHealth') {
                                    $('#txtPhilHealthID').val(vRows[i].IDNo);
                                }
                            }
                        }
                    }
                });
            }

            function GetEmployeeBankAccount(EmployeeID) {
                data = { EmployeeID: EmployeeID }

                AjaxSend({
                    url: 'Organization/GetEmployeeBankAccount',
                    data: data,
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {
                            var data = vData.Results.Rows[0];
                            
                            $('#txtBankID').val(data.BankID)
                            $('#btnBank').text(data.BankName);//.button('refresh')
                            $('#btnBankBranch').val(data.BankBranch)
                            $('#txtAccountName').val(data.AccountName)
                            $('#txtAccountNo').val(data.AccountNo)
                            $('#selBankRole').val(data.BankRole)
                            $('#selIsDefaultAccount').prop('checked', CInt2(data.IsDefaultForThisRole))

                            try {
                                $('#selBankRole');//.selectmenu('refresh')
                                $('#selIsDefaultAccount');//.slider('refresh')
                            } catch (e) {
                                $('#selBankRole');//.selectmenu()
                                $('#selIsDefaultAccount');//.slider()
                            }
                        }
                    }
                });
            }

            function SaveOrganizationEmployee(approvedresetmobizaccby) {
                var pData = [];
                var ctr = 0;
                var Fowst = [];
                var SecurityRole = [];
                var Deminimis = [];
                var ValidIDs = [];

                //FOWST
                ctr = 0;
                $('#lstOrgEmployeeFowst li').each(function (m_index) {
                    if ($.trim($(this).text()) != '') {
                        Fowst[ctr] = { FowstID: $(this).attr('ID') };
                        ctr++;
                    }
                });

                //SecurityRole
                ctr = 0;
                $('#lstOrgEmployeeRole a').each(function (m_index) {
                    if ($.trim($(this).text()) != '') {
                        SecurityRole[ctr] = { SecRoleID: $(this).attr('ID') };
                        ctr++;
                    }
                });

                //if ($('[name="payfrequency"]:checked').length == 0) {
                //    WebToast('Please select Pay Frequency', 'top');
                //    return;
                //}

                $('#tblDeminimisBody tr input').each(function () {
                    var deminimis = {
                        ID: $(this).closest('tr').data('data').ID,
                        Amount: $(this).val()
                    };

                    Deminimis.push(deminimis)
                })

                if ($.trim($('#txtSSSID').val()) != '') {
                    var vID = {
                        IDType: 'SSS',
                        IDNo: $.trim($('#txtSSSID').val())
                    }

                    ValidIDs.push(vID);
                }

                if ($.trim($('#txtPhilHealthID').val()) != '') {
                    var vID = {
                        IDType: 'PhilHealth',
                        IDNo: $.trim($('#txtPhilHealthID').val())
                    }

                    ValidIDs.push(vID);
                }

                if ($.trim($('#txtPagIbigID').val()) != '') {
                    var vID = {
                        IDType: 'PagIbig',
                        IDNo: $.trim($('#txtPagIbigID').val())
                    }

                    ValidIDs.push(vID);
                }

                if ($.trim($('#txtTINID').val()) != '') {
                    var vID = {
                        IDType: 'TIN',
                        IDNo: $.trim($('#txtTINID').val())
                    }

                    ValidIDs.push(vID);
                }

                pData = {
                    ID: ID,
                    OrgEmployeeID: $.trim($("#txtOrgEmpID").val()),
                    SystemGeneratedID: $.trim($("#txtSystemGeneratedID").val()),
                    CompanyID: $.trim($("#txtCompanyID").val()),
                    PeopleID: $.trim($("#txtOrgPeopleID").val()),
                    DateHired: ($("#selOrgDateHiredMonth2").val() == "" ? $("#txtOrgDateHiredMonth").text() : $("#selOrgDateHiredMonth2").val()) + '/' + $("#selOrgDateHiredDay2").val() + '/' + ($("#selOrgDateHiredYear2").val() == "" ? $("#txtOrgDateHiredYear").text() : $("#selOrgDateHiredYear2").val()),
                    IsDeactivated: CInt2($("#selOrgDeactivate").is(':checked')),
                    DeactivationDate: ($("#selOrgDateInActiveMonth").val() == "" ? $("#txtOrgDateInActiveMonth").text() : $("#selOrgDateInActiveMonth").val()) + '/' + $("#selOrgDateInActiveDay").val() + '/' + ($("#selOrgDateInActiveYear").val() == "" ? $("#txtOrgDateInActiveYear").text() : $("#selOrgDateInActiveYear").val()),
                    DeactivationReason: $("#txtOrgInActiveReason").val(),
                    Comments: $("#txtOrgComments").val(),
                    IsBan: CInt2($("#selOrgBan").is(':checked')),
                    IsNew: IsNew,
                    Fowst: Fowst,
                    SecurityRole: SecurityRole,
                    IsConfirm: IsConfirm,
                    RoleTag: $('[name="radRoletag"]:checked').val(),
                    IsDirectHired: $('#selManPower').is(':checked'),
                    IsSelfEmployed: CInt2($('#selSelfEmployed').is(':checked')),
                    ManpowerID: manpowerid,
                    VendorID: vendorid,
                    CoUserID: couserid,
                    ThirdPartySystemName: $.trim($('#txtOrg3rdPartySystemName').val()),
                    ThirdPartySystemID: ($.trim($('#txtOrg3rdPartySystemName').val()) == '' ? '' : $.trim($('#txtOrg3rdPartySystemID').val())),
                    PayFrequency: $('[name="payfrequency"]:checked').val(),
                    WorkScheduleID: workscheduleid,
                    PayTypeID: paytypeid,
                    Deminimis: Deminimis,
                    ValidIDs: ValidIDs,
                    BankID: $('#txtBankID').val(),
                    BankBranch: $('#btnBankBranch').val(),
                    AccountName: $('#txtAccountName').val(),
                    AccountNo: $('#txtAccountNo').val(),
                    BankRole: $('#selBankRole').val(),
                    IsDefaultBank: CInt2($('#selIsDefaultAccount').is(':checked')),
                    Username: $.trim($('#txtEmployeeNewUsername').val()),
                    Userpassword: $.trim($('#txtEmployeeNewPassword').val()),
                    ApprovedResetMobizAccBy: CStr2(approvedresetmobizaccby),
                    Telephone: arrEmployeeTelInfoList,
                    Mobile: arrEmployeeMobileInfoList,
                    Email: arrEmployeeEmailInfoList
                };

                if (isEditEmpName) {
                    pData.FirstName = $.trim($('#txtOrgEmployeeFirstName').val());
                    pData.MiddleName = $.trim($('#txtOrgEmployeeMiddleName').val());
                    pData.LastName = $.trim($('#txtOrgEmployeeLastName').val());
                }

                if ($('#txtDepartment').val() != '') {
                    pData.DepartmentID = DepartmentID;
                }

                if ($('#txtTeam').val() != '') {
                    pData.TeamID = TeamID;
                }

                $('#tblBody').html('');

                AjaxSend({
                    url: "Organization/CheckOrganizationEmployee",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgEmployeeData.Rows.length > 0 && IsNew == 1) {
                            if (IsViaEmail != 0) {
                                SendConfirmationViaEmail();
                            }
                            else {
                                NewMessageBox('Employee being added already exist.', 'Employee Exist', [
                                    {
                                        Text: 'Ok',
                                        Click: function () {
                                            window.location.hash = 'LibraryPage';
                                            if(sessionStorage.SystemAdminFor == 'dtr'){
                                                DTRLibraryTable.Employees();
                                            } else {
                                                if (!CBool(sessionStorage.IsCrm)) {
                                                    $('#tblBody').html('');
                                                    LibraryTable.Employees();
                                                } else {
                                                    CRMLibraryTable.Employees();
                                                }
                                            }
                                        }
                                    }
                                ])
                            }
                        }
                        else {
                            AjaxSend({
                                url: "Organization/SaveOrganizationEmployee",
                                data: pData,
                                success: function (vSaveData) {

                                    if (IsViaEmail == 0) {
                                        //$('#panOrgEmpMessage').popup('close');
                                        if (vSaveData.OrgEmployeeData == "DateHiredError") {
                                            NewMessageBox('Please enter valid date hired information.', 'Date Hired')
                                        }
                                        else if (vSaveData.OrgEmployeeData == "DeactivationDateError") {
                                            NewMessageBox('Please enter valid deactivation date.', 'Deactivation Date');
                                        }
                                        else {
                                            //$.mobile.changePage('#pageOrgEmployee');
                                            //getOrgEmployeeList();
                                            window.location.hash = 'LibraryPage';
                                            if (sessionStorage.SystemAdminFor == 'dtr') {
                                                DTRLibraryTable.Employees();
                                            } else {
                                                if (!CBool(sessionStorage.IsCrm)) {
                                                    $('#tblBody').html('');
                                                    LibraryTable.Employees();
                                                } else {
                                                    CRMLibraryTable.Employees();
                                                }
                                            }
                                            
                                        }
                                    }
                                    else {
                                        SendConfirmationViaEmail();
                                    }
                                }
                            });

                        }

                    }
                });

            };

            function ClearOrgEmployeeFields() {
                manpowerid = '';
                vendorid = '';
                couserid = '';
                DepartmentID = '';
                SystemGeneratedID = '';
                CompanyID = '';
                TeamID = '';

                $("#txtOrgEmpID").val('');

                $("#txtOrgPeopleID").val('');
                //$("#btnOrgEmployee").text('Please Select');//.button('refresh');
                $('#txtOrgEmployee').val('');
                $("#selOrgDateHiredMonth2").val('');//.selectmenu('refresh');
                $("#selOrgDateHiredDay2").val('');//.selectmenu('refresh');
                $("#selOrgDateHiredYear2").val('');//.selectmenu('refresh');
                $('[name="radRoletag"]').removeAttr('checked');//.checkboxradio('refresh');
                $('#lstOrgEmployeeRole').empty();
                $('.lblUserOrgName').text(sessionStorage.OrgName);
                $('#selSelfEmployed').prop('checked', 0);//.slider('refresh');
                $('#selManPower').prop('checked', 1);//.slider('refresh');
                $('#selVendorTradingPartner').prop('checked', 0);//.slider('refresh');
                $('#selEmpCoUser').prop('checked', 0);//.slider('refresh');
                $('#divOtherRelationship, #divManPower, #divVenderTradingPartner, #divCoUserOrgName').hide();
                $('[name="payfrequency"]').removeAttr('checked');//.checkboxradio('refresh');
                $('#txtOrg3rdPartySystemName').val('');
                $('#txtOrg3rdPartySystemID').val('');
                $('#txtDepartmentID').val('');
                $('#txtSystemGeneratedID').val('');
                $('#txtCompanyID').val('');
                $('#txtTeamID').val('');

                $('#txtSSSID').val('');
                $('#txtPhilHealthID').val('');
                $('#txtPagIbigID').val('');
                $('#txtTINID').val('');
            };

            function ClearOrgEmployeeInActiveFields() {

                $("#selOrgDeactivate").val('0');//.slider("refresh");

                $("#selOrgDateInActiveMonth").val('');//.selectmenu('refresh');
                $("#selOrgDateInActiveDay").val('');//.selectmenu('refresh');
                $("#selOrgDateInActiveYear").val('');//.selectmenu('refresh');

                $("#txtOrgInActiveReason").val('');
                $("#txtOrgComments").val('');

                $("#selOrgBan").prop('checked', 0);//.slider("refresh");
            };

        });
    });