require(['common'], function () {
    require(['/Content/bootstrap/js/bootstrap.min.js'], function () {
        var arrPMA = {}, arrCompanyDetails = {}, busindid = '', arrBusInd = [], isPartOfOrganization = false;

        $(document).ready(function () {
            LoadAdditionalData();

            $('body')
            .on('click', '#btnSignUpNow', function (e) {
                e.preventDefault();
                e.stopPropagation();
                $('#modalLogin').modal('hide');
                $('#modalSignUp').modal('show');
                isPartOfOrganization = false;
            })
            .on('change', '#selSecQuestion1', function () {
                var val = $.trim($(this).val());
                $('#selSecQuestion2').find('option').removeAttr('disabled');

                if (val != '') {
                    $('#selSecQuestion2 option[value="' + $(this).val() + '"]').attr('disabled', true);
                }
            })
            .on('change', '#selSecQuestion2', function () {
                var val = $.trim($(this).val());
                $('#selSecQuestion1').find('option').removeAttr('disabled');

                if (val != '') {
                    $('#selSecQuestion1 option[value="' + $(this).val() + '"]').attr('disabled', true);
                }
            })
            .on('keydown', '#txtBirthYear', function (e) {
                if (!NumberOnly(e)) {
                    return
                }
            })
            .on('submit', '#formKonstruksyonSignUp', function (e) {
                

                if (isValid) {
                    $('#btnSubmitSignUp').attr('disabled', true).text('Creating ...');
                    AjaxSend({
                        url: 'Konstruksyon/CreateAccount',
                        //data: {
                        //    details: 
                        //},
                        success: function (e) {
                            var stat = e.Results;
                            $('#btnSubmitSignUp').removeAttr('disabled').text('Create Account');

                            if (stat == 'exist') {
                                $('#signupstat').text('Please check the required fields');
                                txtUsername.parent().addClass('has-error');
                                txtUsername.prev().append(' <i class="errmsg" style="color:red;">Username already exist</i>')
                            } else if (stat == 'saved') {
                                $('#signupstat').text('Account has been succesfully created');
                                $('#btnSubmitSignUp').attr('disabled', true).text('Loggin in ...');
                                setTimeout(function () {
                                    Login(txtUsername.val(), txtConfirmPassword.val(), function () {
                                        $('#modalSignUp').modal('hide');
                                        $('#btnSubmitSignUp').removeAttr('disabled').text('Create Account');
                                    });
                                }, 1500);
                            }
                        }
                    })
                } else {
                    
                }
            })
            .on('click', '#btnSignUpNext', function (e) {
                e.preventDefault();
                var isValid = true;
                var txtFirstname = $('#txtFirstname');
                var txtMiddlename = $('#txtMiddlename');
                var txtLastname = $('#txtLastname');
                var selGenSuffix = $('#selGenSuffix');
                var selBirthMonth = $('#selBirthMonth');
                var selBirthDay = $('#selBirthDay');
                var txtBirthYear = $('#txtBirthYear');
                var txtEmail = $('#txtEmail');
                var txtUsername = $('#txtUsername');
                var txtPassword = $('#txtPassword');
                var txtConfirmPassword = $('#txtConfirmPassword');
                var selSecQuestion1 = $('#selSecQuestion1');
                var txtSecAnswerQ1 = $('#txtSecAnswerQ1');
                var selSecQuestion2 = $('#selSecQuestion2');
                var txtSecAnswerQ2 = $('#txtSecAnswerQ2');

                $('#signupstat').text('');
                $(this).find('.has-error').removeClass('has-error');
                $(this).find('.errmsg').remove();

                if ($.trim(txtFirstname.val()) == '') {
                    isValid = false;
                    txtFirstname.parent().addClass('has-error');
                }

                if ($.trim(txtLastname.val()) == '') {
                    isValid = false;
                    txtLastname.parent().addClass('has-error');
                }

                if ($.trim(selBirthMonth.val()) == '') {
                    isValid = false;
                    selBirthMonth.parent().addClass('has-error');
                }

                if ($.trim(selBirthDay.val()) == '') {
                    isValid = false;
                    selBirthDay.parent().addClass('has-error');
                }

                if ($.trim(txtBirthYear.val()) == '') {
                    isValid = false;
                    txtBirthYear.parent().addClass('has-error');
                }

                if ($.trim(txtEmail.val()) == '') {
                    isValid = false;
                    txtEmail.parent().addClass('has-error');
                } else {
                    if (!IsEmail($.trim(txtEmail.val()))) {
                        isValid = false;
                        txtEmail.prev().append(' <i class="errmsg" style="color:red;">Please enter valid email address</i>')
                    }
                }

                if ($.trim(txtUsername.val()) == '') {
                    isValid = false;
                    txtUsername.parent().addClass('has-error');
                } else {
                    if ($.trim(txtUsername.val()).length < 8) {
                        isValid = false;
                        txtUsername.prev().append(' <i class="errmsg" style="color:red;">Username must be 8 or more characters</i>')
                    }
                }

                if ($.trim(txtPassword.val()) == '') {
                    isValid = false;
                    txtPassword.parent().addClass('has-error');
                } else {
                    if ($.trim(txtPassword.val()).length < 8) {
                        isValid = false;
                        txtPassword.prev().append(' <i class="errmsg" style="color:red;">Password must be 8 or more characters</i>')
                    }
                }

                if ($.trim(txtConfirmPassword.val()) == '') {
                    isValid = false;
                    txtConfirmPassword.parent().addClass('has-error');
                } else {
                    if ($.trim(txtPassword.val()) != $.trim(txtConfirmPassword.val())) {
                        isValid = false;
                        txtConfirmPassword.parent().addClass('has-error');
                        txtConfirmPassword.prev().append(' <i class="errmsg" style="color:red;">Confirm password don\'t match</i>')
                    }
                }

                if ($.trim(selSecQuestion1.val()) == '') {
                    isValid = false;
                    selSecQuestion1.parent().addClass('has-error');
                }

                if ($.trim(txtSecAnswerQ1.val()) == '') {
                    isValid = false;
                    txtSecAnswerQ1.parent().addClass('has-error');
                }

                if ($.trim(selSecQuestion2.val()) == '') {
                    isValid = false;
                    selSecQuestion2.parent().addClass('has-error');
                }

                if ($.trim(txtSecAnswerQ2.val()) == '') {
                    isValid = false;
                    txtSecAnswerQ2.parent().addClass('has-error');
                }

                if(isValid){
                    arrPMA = {
                        FirstName: $.trim(txtFirstname.val()),
                        MiddleName: $.trim(txtMiddlename.val()),
                        LastName: $.trim(txtLastname.val()),
                        GenSuffix: selGenSuffix.val(),
                        DateOfBirth: selBirthMonth.val() + ' ' + selBirthDay.val() + ', ' + txtBirthYear.val(),
                        EmailAddress: $.trim(txtEmail.val()),
                        Username: $.trim(txtUsername.val()),
                        Password: $.trim(txtConfirmPassword.val()),
                        SecQ1: selSecQuestion1.val(),
                        SecQ1Ans: $.trim(txtSecAnswerQ1.val()),
                        SecQ2: selSecQuestion2.val(),
                        SecQ2Ans: $.trim(txtSecAnswerQ2.val()),
                        DateNow: $.format.date(new Date(), 'MMM dd, yyyy')
                    }

                    $('#modalSignUp').modal('hide');
                    $('#modalSignUpNext').modal('show');
                } else {
                    $('#signupstat').text('Please check the required fields');
                }
            })
            .on('click', '#btnSignUpYes', function () {
                $('#modalSignUpNext').modal('hide');
                $('#modalSignUpOrganization').modal('show');
                $('#btnCancelOrg, #btnOkOrg').removeAttr('disabled');
                isPartOfOrganization = true;
            })
            .on('click', '#btnSignUpNo', function () {
                //$('#modalSignUpNext').modal('hide');
                isPartOfOrganization = false;
                $('#btnSignUpNo, #btnSignUpYes').attr('disabled', 'true');
                CreateAccount();
            })
            .on('click', '#btnSignUpBack', function () {
                $('#modalSignUpNext').modal('hide');
                $('#modalSignUp').modal('show');
            })
            .on('click', '#btnLogin', function () {
                $('#formKonstruksyonLogin').find('.has-error').removeClass('has-error');
                $('#txtLoginUsername, #txtLoginPassword').val('');
                $('#loginstat').text('');
            })
            .on('submit', '#formKonstruksyonLogin', function (e) {
                e.preventDefault();

                var isValid = true;
                var txtLoginUsername = $('#txtLoginUsername');
                var txtLoginPassword = $('#txtLoginPassword');

                $('#loginstat').html('');
                $(this).find('.has-error').removeClass('has-error');

                if ($.trim(txtLoginUsername.val()) == '') {
                    isValid = false;
                    txtLoginUsername.parent().addClass('has-error');
                }

                if ($.trim(txtLoginPassword.val()) == '') {
                    isValid = false;
                    txtLoginPassword.parent().addClass('has-error');
                }

                if (isValid) {
                    $('#btnLoginOk').attr('disabled', true).text('Logging in ...');
                    Login(txtLoginUsername.val(), txtLoginPassword.val(), function () {
                        $('#modalLogin').modal('hide');
                    });
                } else {
                    $('#loginstat').html('Invalid username or password <br><br>');
                }
            })
            .on('click', '#btnDavao', function () {
                $('#modalGeography').modal('hide');
                $('#modalLogin').modal('show');
            })
            .on('click', '#btnStartNow', function () {
                $('#modalBuildingPermit').modal('hide');
                $('#modalGeography').modal('show');
            })
            .on('keyup', '#txtBusInd', function (e) {
                var keyword = $.trim($(this).val()).toLowerCase();
                var listBusinessIndustry = $('#listBusinessIndustry');
                listBusinessIndustry.empty();

                if (keyword != '') {
                    $.grep(arrBusInd, function (e) {
                        if ((e.Class).toLowerCase().indexOf(keyword) > -1) {
                            listBusinessIndustry.append($('<a data-role="none" href="#" class="list-group-item">')
                                    .data('data', e)
                                    .text(e.Class)
                                    .click(function (e) {
                                        e.stopPropagation();
                                        e.preventDefault();
                                        busindid = $(this).data('data').ID;
                                        $('#txtBusInd').val($(this).data('data').Class);
                                        listBusinessIndustry.hide();
                                    })
                                )
                        }
                    });
                    listBusinessIndustry.show();
                } else {
                    busindid = '';
                    listBusinessIndustry.hide();
                }
            })
            .on('click', '#btnCancelOrg', function () {
                isPartOfOrganization = false;
                $('#modalSignUpOrganization').modal('hide');
                $('#modalSignUpNext').modal('show');
            })
            .on('click', '#btnOkOrg', function (e) {
                e.preventDefault();

                var txtCompanyName = $('#txtCompanyName');
                var txtCompanyTIN = $('#txtCompanyTIN');
                var txtBusInd = $('#txtBusInd');
                var txtCompanyEmailAdd = $('#txtCompanyEmailAdd');
                var txtCompanyMobileNo = $('#txtCompanyMobileNo');
                var txtCompanyAddressBldgNo = $('#txtCompanyAddressBldgNo');
                var txtCompanyAddressStreetName = $('#txtCompanyAddressStreetName');
                var selCompanyAddressCity = $('#selCompanyAddressCity');
                var selCompanyAddressCountry = $('#selCompanyAddressCountry');
                var hasError = false;
                isPartOfOrganization = true;

                if ($.trim(txtCompanyName.val()) == '') {
                    txtCompanyName.parent().addClass('has-error');
                    hasError = true;
                } else {
                    txtCompanyName.parent().removeClass('has-error');
                }

                if ($.trim(busindid) == '') {
                    txtBusInd.parent().addClass('has-error');
                    hasError = true;
                } else {
                    txtBusInd.parent().removeClass('has-error');
                }

                if ($.trim(txtCompanyEmailAdd.val()) == '') {
                    txtCompanyEmailAdd.parent().addClass('has-error');
                    hasError = true;
                } else {
                    if (!IsEmail($.trim(txtCompanyEmailAdd.val()))) {
                        $('#divCompanyErrMsg').text('Invalid email address');
                        txtCompanyEmailAdd.parent().addClass('has-error');
                        hasError = true;
                    } else {
                        txtCompanyEmailAdd.parent().removeClass('has-error');
                    }
                }

                if ($.trim(txtCompanyMobileNo.val()) == '') {
                    txtCompanyMobileNo.parent().addClass('has-error');
                    hasError = true;
                } else {
                    if ($.trim(txtCompanyMobileNo.val()).length < 10) {
                        $('#divCompanyErrMsg').text('Invalid mobile number');
                        txtCompanyMobileNo.parent().addClass('has-error');
                        hasError = true;
                    } else {
                        txtCompanyMobileNo.parent().removeClass('has-error');

                        if (CInt2(txtCompanyMobileNo.val().substring(0, 1)) != 9) {
                            $('#divCompanyErrMsg').text('Invalid mobile number');
                            txtCompanyMobileNo.parent().addClass('has-error');
                            hasError = true;
                        }
                    }
                }

                if ($.trim(txtCompanyAddressBldgNo.val()) == '') {
                    txtCompanyAddressBldgNo.parent().addClass('has-error');
                    hasError = true;
                } else {
                    txtCompanyAddressBldgNo.parent().removeClass('has-error');
                }

                if ($.trim(txtCompanyAddressStreetName.val()) == '') {
                    txtCompanyAddressStreetName.parent().addClass('has-error');
                    hasError = true;
                } else {
                    txtCompanyAddressStreetName.parent().removeClass('has-error');
                }

                if ($.trim(selCompanyAddressCity.val()) == '') {
                    selCompanyAddressCity.parent().addClass('has-error');
                    hasError = true;
                } else {
                    selCompanyAddressCity.parent().removeClass('has-error');
                }

                if ($.trim(selCompanyAddressCountry.val()) == '') {
                    selCompanyAddressCountry.parent().addClass('has-error');
                    hasError = true;
                } else {
                    selCompanyAddressCountry.parent().removeClass('has-error');
                }

                if (!hasError) {
                    arrCompanyDetails = {
                        CompanyName: $.trim(txtCompanyName.val()),
                        Tin: $.trim(txtCompanyTIN.val()),
                        BusinessIndustry: busindid,
                        CompanyEmailAdd: $.trim(txtCompanyEmailAdd.val()),
                        CompanyMobileNo: $.trim(txtCompanyMobileNo.val()),
                        CompanyBldgNo: $.trim(txtCompanyAddressBldgNo.val()),
                        CompanyStreetName: $.trim(txtCompanyAddressStreetName.val()),
                        CompanyCity: $.trim(selCompanyAddressCity.val()),
                        CompanyCountry: $.trim(selCompanyAddressCountry.val())
                    }
                    $('#divCompanyErrMsg').text('');
                    CreateAccount();
                } else {
                    $('#divCompanyErrMsg').text('Please fill in the required field(s) above.');
                }
            })

            function CreateAccount() {
                var data = $.extend(arrPMA, arrCompanyDetails);

                data.isPartOfOrganization = isPartOfOrganization;

                AjaxSend({
                    url: '/Konstruksyon/CreateAccount?OrgID=' + sessionStorage.OrgID,
                    data: { details: data, host: location.host },
                    success: function (e) {
                        $('#btnSignUpNo, #btnSignUpYes, #btnCancelOrg, #btnOkOrg').removeAttr('disabled');
                        if (e.Results == "exist") {
                            WebToast('Username already exist. Please change your username.', 'top');
                        } else if (e.Results == "saved") {
                            $('#modalSignUpOrganization').modal('hide');
                            $('#modalSignUpNext').modal('hide');
                            $('#lblUserEmail').text(data.EmailAddress);
                            $('#modalSuccess').modal('show');
                        } else {
                            WebToast('ERROR: ' + e.Results, 'top');
                        }
                    }
                })
            }

            function Login(uname, pword, callback) {
                AjaxSend({
                    url: 'Konstruksyon/Login',
                    data: {
                        uname: $.trim(uname),
                        pword: $.trim(pword)
                    },
                    success: function (e) {
                        var res = e.Results;
                        $('#btnLoginOk').removeAttr('disabled').text('Log In');
                        if (res == 'loggedin') {
                            AjaxSend({
                                url: "People/getPeopleOrganizationList",
                                success: function (vDataOrg) {
                                    callback();
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
                                                    }, '/Konstruksyon/Dashboard');
                                                }
                                            })
                                        }

                                        PopupButtonList('', 'Select Organization', a);
                                    }
                                    else if (vDataOrg.PeopleData.Rows.length == 1) {
                                        WebToast('Logged In');
                                        sessionStorage.OrgID = vDataOrg.PeopleData.Rows[0].OrganizationID;
                                        sessionStorage.OrgName = vDataOrg.PeopleData.Rows[0].CompanyName;
                                        sessionStorage.PeopleID = vDataOrg.PeopleData.Rows[0].PeopleID;

                                        location = MergeUrlParameters({
                                            OrgID: vDataOrg.PeopleData.Rows[0].OrganizationID,
                                            IsMobile: CBool(IsMobile) ? 1 : '0'
                                        }, '/Konstruksyon/Dashboard');
                                    }
                                    else {
                                        alert('Please ask your system administrator to add you as member on the organization');
                                    }
                                }
                            });                           
                        } else if (res == 'invalid') {
                            $('#loginstat').html('Invalid username or password <br><br>');
                        } else if (res == 'unconfirmemail') {
                            $('#loginstat').html('Sorry! Your account has not been activated yet. We have sent an email with an activation link to your email address. In order to complete the sign-up process, please click the activation link. Thank you! <br><br>');
                        } else {
                            $('#modalLogin').modal('hide');
                            NewMessageBox(res, 'Message');
                        }
                    }
                })
            }

            function LoadAdditionalData() {
                AjaxSend({
                    url: MergeUrlParameters(GetUrlParameters(), '/Konstruksyon/GetAdditionalDateForSignin'),
                    success: function (e) {
                        arrBusInd = e.BusinessIndustries;
                    }
                })
            }
        }) // end document
    })    
})