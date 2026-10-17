require([
    'common', (InAndroid ? '/Scripts/sources/People/PeopleLogger_offline.js' : '')
    ], function () {

        require([(OfflineMode ? '/Scripts/sources/People/PeopleLogger_offline.js?crc=' : '')], function () {

            $(function () {
                var isFirstPartySignUpValue = 1; //by default, your signing up as your own
                $('.ui-loader').hide();

                if ($.trim($('#hdnPage').val()) == 'signUp') {
                    //$('html').removeClass('ui-mobile');
                    getCallingCode();

                }
                //                $("#txtTelephone").mask("999-9999");

                setMonthCollection('selDOBMonth');
                setYearCollection('selDOBYear');

                window.OrgEmpAdd = {
                    ShowOrgEmpAddData: function () {
                        getCallingCode();
                        setMonthCollection('selDOBMonth');
                        setYearCollection('selDOBYear');
                    }
                }

                var $list = $("#selDOBYear"),
                toRemove = $();
                var curYear = new Date().getFullYear(); //get current year
                var maxYear = $("#selDOBYear option:eq(1)").val(); //get the max date. 2024 is the max date of the year list

                for (var i = curYear + 1; i <= maxYear; i++) {
                    toRemove = toRemove.add($list.find('option[value="' + i + '"]'));
                }
                toRemove.remove();

                $('body').on('change', '#chbFirstPartySignUp', function (e) {
                    //add class to checkbox so it can be detected if clicked
                    var fields = $("input[name='firstPartySignUp']").serializeArray();

                    if (fields.length > 0) {
                        $(this).addClass('checked');

                    } else {
                        $(this).removeClass('checked');
                    }
                })
                .on('click', '#chbFirstPartySignUp', function (e) {
                    //Check if not first party signup, then hide Security questions

                    //FirstPartySignUp
                    var fields = $("input[name='firstPartySignUp']").serializeArray();

                    if (fields.length > 0) { //if it is checked, then it's not a first party sign up because he's signing up on behalf of another person

                        isFirstPartySignUpValue = 0;
                        $('#lblSecurity').hide();
                        $('#secSecurity1').hide();
                        $('#secSecurity2').hide();

                    } else { //if it is not checked, then he is indeed signing up as himself (own profile)
                        isFirstPartySignUpValue = 1;
                        $('#lblSecurity').show();
                        $('#secSecurity1').show();
                        $('#secSecurity2').show();
                    }
                })
                .on('change', '#selDOBDay', function (e) {
                    $('#spanDOBGreater').hide();
                    $('#spanDateOfBirth').hide();
                    $('#selDOBDay').removeClass('form-validation-error');
                    $('#selDOBMonth').removeClass('form-validation-error');
                    $('#selDOBYear').removeClass('form-validation-error');

                    if ($.trim($(this).val()) == '') {
                        $('#spanDateOfBirth').show();
                        $('#selDOBDay').addClass('form-validation-error');
                    }
                })
                .on('change', '#selDOBMonth', function (e) {
                    $('#spanDOBGreater').hide();
                    $('#spanDateOfBirth').hide();
                    $('#selDOBDay').removeClass('form-validation-error');
                    $('#selDOBMonth').removeClass('form-validation-error');
                    $('#selDOBYear').removeClass('form-validation-error');

                    if ($.trim($(this).val()) == '') {
                        $('#spanDateOfBirth').show();
                        $('#selDOBMonth').addClass('form-validation-error');
                    }

                    setDayCollection('selDOBMonth', 'selDOBDay', 'selDOBYear', false);
                })
                .on('change', '#selDOBMonth', function (e) {
                    $('#spanDOBGreater').hide();
                    $('#spanDateOfBirth').hide();
                    $('#selDOBDay').removeClass('form-validation-error');
                    $('#selDOBMonth').removeClass('form-validation-error');
                    $('#selDOBYear').removeClass('form-validation-error');

                    $('#spanDateOfBirth').hide();
                    setDayCollection('selDOBMonth', 'selDOBDay', 'selDOBYear', false);
                    $('#selDOBYear').removeClass('form-validation-error');
                })
                .on('click', 'selDOBYear', function (e) {
                    if ($.trim($('#selDOBYear').val()) == "") {
                        $('#selDOBYear').val('1980');
                        //                $('#selDOBYear').selectmenu('refresh');
                    }

                    $('#selDOBYear').removeClass('form-validation-error');
                })
                .on('change', '#txtFirstName', function (e) {
                    if ($.trim($(this).val()) == '' || $.trim($(this).val()) == 'First Name') {
                        $('#spanFName').show();
                        $('#txtFirstName').addClass('form-validation-error');

                    } else {
                        $('#spanFName').hide();
                        $('#txtFirstName').removeClass('form-validation-error');
                    }
                })
                .on('change', '#txtMiddleName', function (e) {
                    if ($.trim($(this).val()) == '' || $.trim($(this).val()) == 'Middle Name') {
                        $('#spanMName').show();
                        $('#txtMiddleName').addClass('form-validation-error');

                    } else {
                        $('#spanMName').hide();
                        $('#txtMiddleName').removeClass('form-validation-error');
                    }
                })
                .on('change', '#txtSurname', function (e) {
                    if ($.trim($(this).val()) == '' || $.trim($(this).val()) == 'Last Name') {
                        $('#spanLName').show();
                        $('#txtSurname').addClass('form-validation-error');

                    } else {
                        $('#spanLName').hide();
                        $('#txtSurname').removeClass('form-validation-error');
                    }
                })
                .on('change', '#chbNoSuffix', function (e) {
                    var fields = $("input[name='suffix']").serializeArray();

                    if (fields.length > 0) {
                        $('#selPersonalSuffixName').attr('disabled', 'disabled');
                        $('#selPersonalSuffixName').val('');

                        $('#spanSuffix').hide();
                        $('#chbNoSuffix').removeClass('form-validation-error');
                        $('#selPersonalSuffixName').removeClass('form-validation-error');
                    } else {
                        if ($.trim($('#selPersonalSuffixName').val()) == '') {
                            $('#spanSuffix').show();
                            $('#chbNoSuffix').addClass('form-validation-error');
                            $('#selPersonalSuffixName').addClass('form-validation-error');
                        }

                        $('#selPersonalSuffixName').removeAttr('disabled');
                    }
                })
                .on('change', '#selPersonalSuffixName', function (e) {
                    var fields = $("input[name='suffix']").serializeArray();

                    if ($.trim($('#selPersonalSuffixName').val()) == '') {
                        if (fields.length > 0) {
                            $('#chbNoSuffix').removeClass('form-validation-error');
                            $('#selPersonalSuffixName').removeClass('form-validation-error');
                        } else {
                            $('#spanSuffix').show();
                            $('#selPersonalSuffixName').addClass('form-validation-error');
                            $('#chbNoSuffix').addClass('form-validation-error');
                        }

                    }
                    else {
                        $('#spanSuffix').hide();
                        $('#selPersonalSuffixName').removeClass('form-validation-error');
                        $('#chbNoSuffix').removeClass('form-validation-error');
                    }
                })
                .on('change', '#txtMotherMaidenFirstName', function (e) {
                    if ($.trim($(this).val()) == '' || $.trim($(this).val()) == "Mother's First Name") {
                        $('#txtMotherMaidenFirstName').addClass('form-validation-error');
                        $('#spanMotherFName').show();

                    } else {
                        $('#spanMotherFName').hide();
                        $('#txtMotherMaidenFirstName').removeClass('form-validation-error');
                    }
                })
                .on('change', '#txtMotherMaidenMiddleName', function (e) {
                    if ($.trim($(this).val()) == '' || $.trim($(this).val()) == "Mother's Maiden Middle Name") {
                        $('#txtMotherMaidenMiddleName').addClass('form-validation-error');
                        $('#spanMotherMName').show();

                    } else {
                        $('#spanMotherMName').hide();
                        $('#txtMotherMaidenMiddleName').removeClass('form-validation-error');
                    }
                })
                .on('change', '#txtMotherMaidenSurname', function (e) {
                    if ($.trim($(this).val()) == '' || $.trim($(this).val()) == "Mother's Maiden Last Name") {
                        $('#txtMotherMaidenSurname').addClass('form-validation-error');
                        $('#spanMotherLName').show();

                    } else {
                        $('#spanMotherLName').hide();
                        $('#txtMotherMaidenSurname').removeClass('form-validation-error');
                    }
                })
                .on('change', '#selCallingCodeMobile', function (e) {
                    if ($('#selCallingCodeMobile option:selected').text() == '63 - Philippines') { //If the user selected Philippines 
                        getTelcoPrefix();
                    } else { //clear the dropdown
                        $('#selTelcoPrefix').empty();
                        $('#selTelcoPrefix').append(new Option('Please Select Mobile Number Prefix', ''));
                        $('#selTelcoPrefix').val(-1);
                    }
                })
                .on('change', '#selCallingCodeTel', function (e) {
                    if ($('#selCallingCodeTel option:selected').text() == '63 - Philippines') { //If the user selected Philippines 
                        getAreaCode();
                    } else { //clear the dropdown
                        $('#selAreaCode').empty();
                        $('#selAreaCode').append(new Option('Please Select Area Code', ''));
                        $('#selAreaCode').val(-1);
                    }
                })
                .on('click', '#chbNoEmail', function (e) {
                    var fields = $("input[name='email']").serializeArray();

                    if (fields.length > 0) {
                        $('#txtEmail').attr('disabled', 'disabled');
                        $('#txtEmail').val('');

                        $('#chbNoEmail').removeClass('form-validation-error');

                        $('#spanEmail').hide();
                        $('#txtEmail').removeClass('form-validation-error');
                    } else {
                        $('#txtEmail').removeAttr('disabled');
                    }
                })
                .on('change', '#txtTelephone', function (e) {
                    $('#txtTelephone').removeClass('form-validation-error');
                    $('#spanTelNoValid').hide();
                    $('#selCallingCodeTel').removeClass('form-validation-error');
                    $('#spanCountryCodeTelNo').hide();
                    $('#spanAreaCode').hide();
                    $('#selAreaCode').removeClass('form-validation-error');


                    if ($.trim($(this).val()) != '') {
                        if (!validatePhoneNumber($.trim($(this).val())) || $(this).val().length < 7) {
                            $('#spanTelNoValid').show();
                            $('#txtTelephone').addClass('form-validation-error');
                        }

                        if ($.trim($('#selCallingCodeTel').val()) == '') {
                            $('#selCallingCodeTel').addClass('form-validation-error');
                            $('#spanCountryCodeTelNo').show();
                        }
                    }
                })
                .on('change', '#txtMobile', function (e) {
                    $('#spanMobileValid').hide();
                    $('#txtMobile').removeClass('form-validation-error');
                    $('#spanCountryCodeMobile').hide();
                    $('#selCallingCodeMobile').removeClass('form-validation-error');
                    $('#spanTelcoPrefix').hide();
                    $('#selTelcoPrefix').removeClass('form-validation-error');


                    if ($.trim($(this).val()) != '' && $.trim($(this).val()) != 'Enter your 7-digit mobile number') {
                        if (!validateNumber($.trim($(this).val()))) {
                            $('#txtMobile').addClass('form-validation-error');
                            $('#spanMobileValid').show();
                        }

                        if ($.trim($('#selCallingCodeMobile').val()) == '') {
                            $('#selCallingCodeMobile').addClass('form-validation-error');
                            $('#spanCountryCodeMobile').show();
                        }
                    }
                })
                .on('change', '#selCallingCodeTel', function (e) {
                    $('#txtTelephone').removeClass('form-validation-error');
                    $('#spanTelNoValid').hide();
                    $('#selCallingCodeTel').removeClass('form-validation-error');
                    $('#spanCountryCodeTelNo').hide();
                    $('#spanAreaCode').hide();
                    $('#selAreaCode').removeClass('form-validation-error');

                    //                    if ($.trim($(this).val()) == '') {
                    //                        if ($.trim($('#txtTelephone').val()) != '' && $.trim($('#txtTelephone').val()) != 'xxx-xxxx') {
                    //                            $('#selCallingCodeTel').addClass('form-validation-error');
                    //                            $('#spanCountryCodeTelNo').show();
                    //                        }
                    //                    } else {
                    //                        if ($.trim($('#txtTelephone').val()) == '' || $('#txtTelephone').val().length < 8 || $.trim($('#txtTelephone').val()) == 'xxx-xxxx') {
                    //                            $('#txtTelephone').addClass('form-validation-error');
                    //                            $('#spanTelNoValid').show();
                    //                        }
                    //                    }
                })
                .on('change', '#selCallingCodeMobile', function (e) {
                    $('#spanMobileValid').hide();
                    $('#txtMobile').removeClass('form-validation-error');
                    $('#spanCountryCodeMobile').hide();
                    $('#selCallingCodeMobile').removeClass('form-validation-error');
                    $('#spanTelcoPrefix').hide();
                    $('#selTelcoPrefix').removeClass('form-validation-error');

                    //                    if ($.trim($(this).val()) == '') {

                    //                        if ($.trim($('#txtMobile').val()) != '' && $.trim($('#txtMobile').val()) != 'Enter your 7-digit mobile number') {
                    //                            $('#selCallingCodeMobile').addClass('form-validation-error');
                    //                            $('#spanCountryCodeMobile').show();
                    //                        }
                    //                    } else {
                    //                        if ($.trim($('#txtMobile').val()) == '' || $.trim($('#txtMobile').val()) == 'Enter your 7-digit mobile number') {
                    //                            $('#spanMobileValid').show();
                    //                            $('#txtMobile').addClass('form-validation-error');
                    //                        }
                    //                    }
                })
                .on('change', '#txtEmail', function (e) {
                    var fields2 = $("input[name='email']").serializeArray();

                    if ($.trim($(this).val()) != '' && fields2.length == 0) {

                        if (!validateEmail($.trim($(this).val()))) {
                            $('#spanEmail').show();
                            $('#txtEmail').addClass('form-validation-error');
                        }
                        else {
                            $('#spanEmail').hide();
                            $('#txtEmail').removeClass('form-validation-error');
                        }

                    }

                    $('#chbNoEmail').removeClass('form-validation-error');
                })
                .on('change', '#txtUserName', function (e) {
                    $('#spanUserName').hide();
                    $('#spanUserNameChars').hide();
                    $('#spanUserNameAlphanum').hide();

                    $(this).removeClass('form-validation-error');

                    if ($.trim($(this).val()) == '') {
                        $('#spanUserName').show();
                        $(this).addClass('form-validation-error');
                    } else {
                        if ($.trim($(this).val()).length < 8) {
                            $('#spanUserNameChars').show();
                            $(this).addClass('form-validation-error');
                        }
                        else {
                            if (!validateAlphanumeric($.trim($('#txtUserName').val()))) {

                                $('#spanUserNameAlphanum').show();
                                $(this).addClass('form-validation-error');
                            }

                        }
                    }
                })
                .on('change', '#txtPassWord', function (e) {
                    $('#txtConfirmPassword').val('');
                    $('#spanPasswordRepeat').hide();
                    if ($.trim($(this).val()) == '') {
                        $('#spanPassword').show();
                        $('#spanPasswordChars').hide();
                        $('#txtPassWord').addClass('form-validation-error');

                    } else {
                        if ($.trim($(this).val()).length < 8) {
                            $('#spanPasswordChars').show();
                            $('#txtPassWord').addClass('form-validation-error');
                        } else {
                            $('#spanPasswordChars').hide();
                            $('#txtPassWord').removeClass('form-validation-error');
                        }
                        $('#spanPassword').hide();
                    }
                })
                .on('change', '#txtConfirmPassword', function (e) {
                    if ($.trim($(this).val()) == '') {
                        $('#spanConfirmPassword').show();
                        $('#spanPasswordRepeat').hide();
                        $('#txtConfirmPassword').addClass('form-validation-error');

                    } else {
                        if ($.trim($(this).val()) != $.trim($('#txtPassWord').val()) && $.trim($('#txtPassWord').val()) != '') {
                            $('#spanPasswordRepeat').show();
                            $('#txtConfirmPassword').addClass('form-validation-error');
                        } else {
                            $('#txtConfirmPassword').removeClass('form-validation-error');
                        }

                        $('#spanConfirmPassword').hide();

                    }
                })
                .on('change', '#txtQ1Answer', function (e) {
                    //$('#spanQuestion1').hide();
                    if ($.trim($(this).val()) == '') {
                        $('#spanAnswer1').show();
                        $('#txtQ1Answer').addClass('form-validation-error');
                    } else {
                        $('#spanAnswer1').hide();
                        $('#txtQ1Answer').removeClass('form-validation-error');
                    }
                })
                .on('change', '#txtQ2Answer', function (e) {
                    //$('#spanQuestion1').hide();
                    if ($.trim($(this).val()) == '') {
                        $('#spanAnswer2').show();
                        $('#txtQ2Answer').addClass('form-validation-error');

                    } else {
                        $('#spanAnswer2').hide();
                        $('#txtQ2Answer').removeClass('form-validation-error');
                    }
                })
                .on('change', '#selQuestion1', function (e) {
                    $('#txtQ1Answer').val('');
                    $('#spanQuestion1').hide();
                    $('#spanQuestion2').hide();
                    if ($.trim($(this).val()) == '') {
                        $('#spanAnswer1').show();
                        $('#selQuestion1').addClass('form-validation-error');
                    } else {

                        $('#spanAnswer1').hide();
                        $('#spanQuestion1').hide();
                        //Check if question is already identical to the other
                        if ($.trim($(this).val()) == $.trim($('#selQuestion2').val())) {
                            $('#spanQuestion1').show();

                        }
                        $('#selQuestion1').removeClass('form-validation-error');
                    }
                })
                .on('change', '#selQuestion2', function (e) {
                    $('#txtQ2Answer').val('');
                    $('#spanQuestion2').hide();
                    $('#spanQuestion1').hide();
                    if ($.trim($(this).val()) == '') {
                        $('#spanAnswer2').show();
                        $('#selQuestion2').addClass('form-validation-error');
                    } else {

                        $('#spanAnswer2').hide();
                        $('#spanQuestion2').hide();
                        //Check if question is already identical to the other
                        if ($.trim($(this).val()) == $.trim($('#selQuestion1').val())) {
                            $('#spanQuestion2').show();

                        }
                        $('#selQuestion2').removeClass('form-validation-error');
                    }
                })
                .on('change', '#txtPIN1', function (e) {
                    //$('#spanQuestion1').hide();
                    if ($.trim($(this).val()) == '') {
                        $('#spanPIN1').show();
                        $('#txtPIN1').addClass('form-validation-error');

                    } else {
                        $('#spanPIN1').hide();
                        $('#txtPIN1').removeClass('form-validation-error');
                    }
                })
                .on('keydown', '#txtPIN1', function (e) {
                    if (!NumberOnly(e)) {
                        return
                    }
                })
                .on('change', '#txtPIN2', function (e) {
                    //$('#spanQuestion1').hide();
                    if ($.trim($(this).val()) == '') {
                        $('#spanPIN2').show();
                        $('#txtPIN2').addClass('form-validation-error');

                    } else {
                        $('#spanPIN2').hide();
                        $('#txtPIN2').removeClass('form-validation-error');
                    }
                })
                .on('keydown', '#txtPIN2', function (e) {
                    if (!NumberOnly(e)) {
                        return
                    }
                })
                .on('change', '#txtQ1AnswerAccountRecovery', function (e) {
                    if ($.trim($(this).val()) == '') {
                        $('#spanAnswer1AccountRecovery').show();
                        $('#txtQ1AnswerAccountRecovery').addClass('form-validation-error');

                    } else {
                        $('#spanAnswer1AccountRecovery').hide();
                        $('#txtQ1AnswerAccountRecovery').removeClass('form-validation-error');
                    }
                })
                .on('change', '#txtQ2AnswerAccountRecovery', function (e) {
                    if ($.trim($(this).val()) == '') {
                        $('#spanAnswer2AccountRecovery').show();
                        $('#txtQ2AnswerAccountRecovery').addClass('form-validation-error');
                    } else {
                        $('#spanAnswer2AccountRecovery').hide();
                        $('#txtQ2AnswerAccountRecovery').removeClass('form-validation-error');
                    }
                })
                .on('change', '#txtUserNameUsrAcct', function (e) {
                    $('#spanUserNameUsrAcct').hide();
                    $('#spanUserNameUsrAcctChars').hide();
                    $('#spanUserNameUsrAcctAlphanum').hide();

                    $(this).removeClass('form-validation-error');

                    if ($.trim($(this).val()) == '') {
                        $('#spanUserNameUsrAcct').show();
                        $(this).addClass('form-validation-error');
                    } else {
                        if ($.trim($(this).val()).length < 8) {
                            $('#spanUserNameUsrAcctChars').show();
                            $(this).addClass('form-validation-error');
                        }
                        else {
                            if (!validateAlphanumeric($.trim($(this).val()))) {

                                $('#spanUserNameUsrAcctAlphanum').show();
                                $(this).addClass('form-validation-error');
                            }
                        }
                    }
                })
                .on('change', '#txtPasswordUsrAcct', function (e) {
                    $('#txtConfirmPasswordUsrAcct').val('');
                    $('#spanPasswordRepeatUsrAcct').hide();
                    if ($.trim($(this).val()) == '') {
                        $('#spanPasswordUsrAcct').show();
                        $('#spanPasswordUsrAcctChars').hide();
                        $('#txtPasswordUsrAcct').addClass('form-validation-error');

                    } else {
                        if ($.trim($(this).val()).length < 8) {
                            $('#spanPasswordUsrAcctChars').show();
                            $('#txtPasswordUsrAcct').addClass('form-validation-error');
                        } else {
                            $('#spanPasswordUsrAcctChars').hide();
                            $('#txtPasswordUsrAcct').removeClass('form-validation-error');
                        }
                        $('#spanPasswordUsrAcct').hide();
                    }
                })
                .on('change', '#txtConfirmPasswordUsrAcct', function (e) {
                    if ($.trim($(this).val()) == '') {
                        $('#spanConfirmPasswordUsrAcct').show();
                        $('#spanPasswordRepeatUsrAcct').hide();
                        $('#txtConfirmPasswordUsrAcct').addClass('form-validation-error');

                    } else {
                        if ($.trim($(this).val()) != $.trim($('#txtPasswordUsrAcct').val()) && $.trim($('#txtPasswordUsrAcct').val()) != '') {
                            $('#spanPasswordRepeatUsrAcct').show();
                            $('#txtConfirmPasswordUsrAcct').addClass('form-validation-error');
                        } else {
                            $('#txtConfirmPasswordUsrAcct').removeClass('form-validation-error');
                        }

                        $('#spanConfirmPasswordUsrAcct').hide();

                    }
                })
                .on('click', '#btnSignUp', function (e) {
                    e.preventDefault();
                    validateFields();
                    $('#spanAlert').hide();

                    if ($.trim($('#txtFirstName').val()) == '' || $.trim($(this).val()) == 'First Name') {

                        $('#spanAlert').show();
                        return;
                    }
                    if ($.trim($('#txtMiddleName').val()) == '' || $.trim($(this).val()) == 'Middle Name') {
                        $('#spanAlert').show();
                        return;
                    }
                    if ($.trim($('#txtSurname').val()) == '' || $.trim($(this).val()) == 'Last Name') {
                        $('#spanAlert').show();
                        return;
                    }

                    var fields = $("input[name='suffix']").serializeArray();

                    if (fields.length == 0 && $.trim($('#selPersonalSuffixName').val()) == '') {
                        $('#spanAlert').show();
                        return;
                    }

                    if ($.trim($('#selDOBMonth').val()) == '') {
                        $('#spanAlert').show();
                        return;
                    }
                    if ($.trim($('#selDOBDay').val()) == '') {
                        $('#spanAlert').show();
                        return;
                    }
                    if ($.trim($('#selDOBYear').val()) == '') {
                        $('#spanAlert').show();
                        return;
                    }
                    if ($.trim($('#txtMotherMaidenFirstName').val()) == '' || $.trim($('#txtMotherMaidenFirstName').val()) == "Mother's First Name") {
                        $('#spanAlert').show();
                        return;
                    }
                    if ($.trim($('#txtMotherMaidenMiddleName').val()) == '' || $.trim($('#txtMotherMaidenMiddleName').val()) == "Mother's Maiden Middle Name") {
                        $('#spanAlert').show();
                        return;
                    }
                    if ($.trim($('#txtMotherMaidenSurname').val()) == '' || $.trim($('#txtMotherMaidenSurname').val()) == "Mother's Maiden Last Name") {
                        $('#spanAlert').show();
                        return;
                    }

                    if ($.trim($('#txtTelephone').val()) != '' && $.trim($('#txtTelephone').val()) != 'xxx-xxxx') {
                        if (!validatePhoneNumber($.trim($('#txtTelephone').val())) || $('#txtTelephone').val().length < 7) {
                            $('#spanAlert').show();
                            return;
                        }
                    }

                    if ($.trim($('#txtMobile').val()) != '' && $.trim($('#txtMobile').val()) != 'Enter your 7-digit mobile number') {
                        if (!validateNumber($.trim($('#txtMobile').val()))) {
                            $('#spanAlert').show(); 0
                            return;
                        }
                    }


                    //                    if ($.trim($('#selCallingCodeTel').val()) != '') {
                    //                        if ($('#selCallingCodeTel option:selected').text() == '63 - Philippines') {
                    //                            if ($.trim($('#selAreaCode').val()) != '') { //if he selected an area code
                    //                                if ($.trim($('#txtTelephone').val()) == '' || $.trim($('#txtTelephone').val()) == 'xxx-xxxx') { //check if there is no tel no
                    //                                    $('#spanAlert').show();
                    //                                    return;
                    //                                }
                    //                            }
                    //                        } else { //if didnt select philippines
                    //                            if ($.trim($('#txtTelephone').val()) == '' || $.trim($('#txtTelephone').val()) == 'xxx-xxxx') {
                    //                                $('#spanAlert').show();
                    //                                return;
                    //                            }
                    //                        }
                    //                    } else { //if he DIDNT select a country
                    //                        if ($.trim($('#txtTelephone').val()) != '' && $.trim($('#txtTelephone').val()) != 'xxx-xxxx') {
                    //                            $('#spanAlert').show();
                    //                            return;
                    //                        }
                    //                    }

                    //                    if ($.trim($('#selCallingCodeMobile').val()) != '') {
                    //                        if ($('#selCallingCodeMobile option:selected').text() == '63 - Philippines') {
                    //                            if ($.trim($('#selTelcoPrefix').val()) != '') { //if he selected a telco prefix
                    //                                if ($.trim($('#txtMobile').val()) == '' || $.trim($('#txtMobile').val()) == 'Enter your 7-digit mobile number') {
                    //                                    $('#spanAlert').show();
                    //                                    return;
                    //                                }
                    //                            }
                    //                        } else { //if didnt select philippines
                    //                            if ($.trim($('#txtMobile').val()) == '' || $.trim($('#txtMobile').val()) == 'Enter your 7-digit mobile number') {
                    //                                $('#spanAlert').show();
                    //                                return;
                    //                            }
                    //                        }
                    //                    } else { //if he DIDNT select a country
                    //                        if ($.trim($('#txtMobile').val()) != '' && $.trim($('#txtMobile').val()) != 'Enter your 7-digit mobile number') {
                    //                            $('#spanAlert').show();
                    //                            return;
                    //                        }
                    //                    }



                    if ($.trim($('#txtUserName').val()) == '') {
                        $('#spanAlert').show();
                        return;
                    } else {
                        if ($.trim($('#txtUserName').val()).length < 8) {
                            $('#spanAlert').show();
                            return;
                        }

                        if (!validateAlphanumeric($.trim($('#txtUserName').val()))) {
                            $('#spanAlert').show();
                            return;
                        }

                    }

                    if ($.trim($('#txtPassWord').val()) == '' || $.trim($('#txtPassWord').val()).length < 8) {
                        $('#spanAlert').show();
                        return;
                    }

                    if ($.trim($('#txtConfirmPassword').val()) == '') {
                        $('#spanAlert').show();
                        return;
                    }

                    if ($.trim($('#txtPassWord').val()) != '') {
                        if ($.trim($('#txtPassWord').val()) != $.trim($('#txtConfirmPassword').val()) && $.trim($('#txtConfirmPassword').val()) != '') {
                            $('#spanAlert').show();
                            return;
                        }
                    }

                    if ($.trim($('#txtConfirmPassword').val()) != '') {
                        if ($.trim($('#txtConfirmPassword').val()) != $.trim($('#txtPassWord').val()) && $.trim($('#txtPassWord').val()) != '') {
                            $('#spanAlert').show();
                            return;
                        }
                    }

                    if ($.trim($('#txtPIN1').val()).length < 5) {
                        $('#spanPIN12').show();
                        return;
                    } else {
                        $('#spanPIN12').hide();
                    }

                    if ($.trim($('#txtPIN1').val()) != '' || $.trim($('#txtPIN2').val()) != '') {
                        if ($.trim($('#txtPIN1').val()) != $.trim($('#txtPIN2').val())) {
                            $('#spanPIN2').show();
                            return;
                        } else {
                            $('#spanPIN2').hide();
                        }
                    }



                    //validate only if first party sign up is true
                    if (isFirstPartySignUpValue == 1) {
                        if ($.trim($('#txtQ1Answer').val()) == '') {
                            $('#spanAlert').show();
                            return;
                        }

                        if ($.trim($('#txtQ2Answer').val()) == '') {
                            $('#spanAlert').show();
                            return;
                        }


                        if ($.trim($('#selQuestion1').val()) == '') {
                            $('#spanAlert').show();
                            return;
                        } else {
                            if ($.trim($('#txtQ1Answer').val()) == '') {
                                $('#spanAlert').show();
                                return;
                            }
                            //Check if question is already identical to the other
                            if ($.trim($('#selQuestion1').val()) == $.trim($('#selQuestion2').val())) {
                                $('#spanAlert').show();
                                return;
                            }
                        }

                        if ($.trim($('#selQuestion2').val()) == '') {
                            $('#spanAlert').show();
                            return;
                        } else {
                            if ($.trim($('#txtQ2Answer').val()) == '') {
                                $('#spanAlert').show();
                                return;
                            }
                            //Check if question is already identical to the other
                            if ($.trim($('#selQuestion2').val()) == $.trim($('#selQuestion1').val())) {
                                $('#spanAlert').show();
                                return;
                            }
                        }
                    }

                    if (!validateDate()) { //if not true
                        $('#spanAlert').show();
                        return;
                    }

                    var fields2 = $("input[name='email']").serializeArray();

                    if ($.trim($('#txtEmail').val()) == '' && fields2.length == 0) {
                        alert('Oops!  You might have forgotten to put in an email address.  Please provide an email address so that we can email you for important notifications.');

                        $('#txtEmail').addClass('focus');
                        $("#txtEmail").focus();

                        $('#chbNoEmail').addClass('form-validation-error');

                        $('#spanAlert').show();
                        return;

                    } else {
                        if (fields2.length == 0) {
                            if (!validateEmail($.trim($('#txtEmail').val()))) {
                                $('#spanAlert').show();
                                return;
                            }
                        }

                        $('#chbNoEmail').removeClass('form-validation-error');
                    }


                    var fields3 = $("input[name='certified']").serializeArray();

                    if (fields3.length > 0) {
                        $('#chbCertifiedCorrect').removeClass('hoverChecked');

                    } else {
                        $('#chbCertifiedCorrect').addClass('hoverChecked');
                        return;
                    }

                    CheckUser();
                })
                .on('change', '#btnSubmitSecurity', function (e) {
                    e.preventDefault();

                    if ($.trim($('#txtQ1AnswerAccountRecovery').val()) == '') {
                        $('#txtQ1AnswerAccountRecovery').addClass('form-validation-error');
                        $('#spanAnswer1AccountRecovery').show();
                    }
                    if ($.trim($('#txtQ2AnswerAccountRecovery').val()) == '') {
                        $('#txtQ2AnswerAccountRecovery').addClass('form-validation-error');
                        $('#spanAnswer2AccountRecovery').show();
                    }

                    if ($.trim($('#txtQ1AnswerAccountRecovery').val()) == '') {
                        return;
                    }
                    if ($.trim($('#txtQ2AnswerAccountRecovery').val()) == '') {
                        return;
                    }

                    CheckSecurityQuestion();
                })
                .on('change', '#btnSubmitUsrAcct', function (e) {
                    e.preventDefault();
                    validateFieldsUsrAcct();


                    if ($.trim($('#txtUserNameUsrAcct').val()) == '') {
                        return;
                    } else {
                        if ($.trim($('#txtUserNameUsrAcct').val()).length < 8) {
                            return;
                        }

                        if (!validateAlphanumeric($.trim($('#txtUserNameUsrAcct').val()))) {
                            return;
                        }

                    }



                    if ($.trim($('#txtPasswordUsrAcct').val()) == '' || $.trim($('#txtPasswordUsrAcct').val()).length < 8) {
                        return;
                    }

                    if ($.trim($('#txtConfirmPasswordUsrAcct').val()) == '') {
                        return;
                    }

                    if ($.trim($('#txtPasswordUsrAcct').val()) == '') {
                        return;
                    } else {

                        if ($.trim($('#txtPasswordUsrAcct').val()) != $.trim($('#txtConfirmPasswordUsrAcct').val()) && $.trim($('#txtConfirmPasswordUsrAcct').val()) != '') {
                            return;
                        }
                    }

                    if ($.trim($('#txtConfirmPasswordUsrAcct').val()) == '') {
                        return;
                    } else {

                        if ($.trim($('#txtConfirmPasswordUsrAcct').val()) != $.trim($('#txtPasswordUsrAcct').val()) && $.trim($('#txtPasswordUsrAcct').val()) != '') {
                            return;
                        }
                    }

                    //alert('ok');
                    SavePersonUserAccountRecovery();
                })
                .on('change', '#chbCertifiedCorrect', function (e) {
                    var fields = $("input[name='certified']").serializeArray();

                    if (fields.length > 0) {
                        $(this).removeClass('hoverChecked');

                    } else {
                        $(this).addClass('hoverChecked');
                    }
                })
                .on('change', '#chbAgree', function (e) {
                    var fields = $("input[name='agree']").serializeArray();

                    if (fields.length > 0) {
                        $('#chbAgree').removeClass('hoverChecked');

                    } else {
                        $('#chbAgree').addClass('hoverChecked');
                    }
                })
                .on('change', '#btnProceed', function (e) {
                    var fields = $("input[name='agree']").serializeArray();

                    if (fields.length > 0) {
                        $('#chbAgree').removeClass('hoverChecked');

                        //                        AjaxSend({
                        //                            url: "ImportantReminders/SaveIsAgreed",
                        //                            success: function (vData) {

                        location = '../signup';

                        //                            }
                        //                        });


                    } else {
                        $('#chbAgree').addClass('hoverChecked');
                    }
                })
                .on('click', '#btnSignUpCancel', function () {
                    window.location.hash = 'LibraryPage';
                })
                //                $('#selAreaCode').change(function () {
                //                    if ($.trim($(this).val()) == '') {
                //                        if ($('#selCallingCodeTel option:selected').text() == '63 - Philippines') {
                //                            $('#selAreaCode').addClass('form-validation-error');
                //                            $('#spanAreaCode').show();
                //                        }
                //                        else {
                //                            $('#spanAreaCode').hide();
                //                            $('#selAreaCode').removeClass('form-validation-error');
                //                        }

                //                    } else {
                //                        $('#spanAreaCode').hide();
                //                        $('#selAreaCode').removeClass('form-validation-error');
                //                    }

                //                });



                //                $('#selTelcoPrefix').change(function () {
                //                    if ($.trim($(this).val()) == '') {
                //                        if ($('#selCallingCodeMobile option:selected').text() == '63 - Philippines') {
                //                            $('#spanTelcoPrefix').show();
                //                            $('#selTelcoPrefix').addClass('form-validation-error');
                //                        }
                //                        else {
                //                            $('#spanTelcoPrefix').hide();
                //                            $('#selTelcoPrefix').removeClass('form-validation-error');
                //                        }
                //                    } else {
                //                        $('#spanTelcoPrefix').hide();
                //                        $('#selTelcoPrefix').removeClass('form-validation-error');
                //                    }

                //                });


                function validateEmail(email) {
                    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

                    return re.test(email);
                }

                function validateNumber(number) {
                    var re = /^[0-9]{7}$/;

                    return re.test(number);
                }

                function validatePhoneNumber(number) {
                    var re = /^\d{3}-?.?d?\d*$/;

                    return re.test(number);
                }

                function validateAlphanumeric(string) {
                    var re = /^[a-zA-Z0-9\_]+$/;

                    return re.test(string);
                }

                function validateDate() {
                    var today = new Date();
                    var dd = today.getDate();
                    var mm = today.getMonth() + 1; //January is 0!

                    var yyyy = today.getFullYear();
                    if (dd < 10) { dd = '0' + dd } if (mm < 10) { mm = '0' + mm } today = yyyy + '-' + mm + '-' + dd;
                    var dateToday = new Date(today);

                    var targetDate = '';

                    if ($("#selDOBMonth").val() != '' && $("#selDOBDay").val() != '' && $("#selDOBYear").val() != '') {
                        targetDate = new Date($("#selDOBYear").val() + '-' + $("#selDOBMonth").val() + '-' + $("#selDOBDay").val()); //javascript can read this format of date

                        if (targetDate > dateToday) {
                            $('#spanDOBGreater').show();
                            $('#spanDateOfBirth').hide();
                            $('#selDOBDay').addClass('form-validation-error');
                            $('#selDOBMonth').addClass('form-validation-error');
                            $('#selDOBYear').addClass('form-validation-error');

                            return false; //somethings wrong
                        } else {
                            return true; //everythings ok
                        }


                    }

                }

                $('#btnSubmit').click(function (e) {
                    e.preventDefault();
                    validateFieldsAccountRecovery();

                    if ($.trim($('#txtFirstName').val()) == '' || $.trim($(this).val()) == 'First Name') {
                        return;
                    }
                    if ($.trim($('#txtMiddleName').val()) == '' || $.trim($(this).val()) == 'Middle Name') {
                        return;
                    }
                    if ($.trim($('#txtSurname').val()) == '' || $.trim($(this).val()) == 'Last Name') {
                        return;
                    }
                    var fields = $("input[name='suffix']").serializeArray();

                    if (fields.length == 0 && $.trim($('#selPersonalSuffixName').val()) == '') {
                        $('#spanAlert').show();
                        return;
                    }
                    if ($.trim($('#selDOBMonth').val()) == '') {
                        return;
                    }
                    if ($.trim($('#selDOBDay').val()) == '') {
                        return;
                    }
                    if ($.trim($('#selDOBYear').val()) == '') {
                        return;
                    }
                    if ($.trim($('#txtMotherMaidenFirstName').val()) == '' || $.trim($('#txtMotherMaidenFirstName').val()) == "Mother's First Name") {
                        return;
                    }
                    if ($.trim($('#txtMotherMaidenMiddleName').val()) == '' || $.trim($('#txtMotherMaidenMiddleName').val()) == "Mother's Maiden Middle Name") {
                        return;
                    }
                    if ($.trim($('#txtMotherMaidenSurname').val()) == '' || $.trim($('#txtMotherMaidenSurname').val()) == "Mother's Maiden Last Name") {
                        return;
                    }

                    ShowSecurityQuestion();

                });


                //End of button click events


                //Validation functions

                function validateFieldsUsrAcct() {

                    if ($.trim($('#txtUserNameUsrAcct').val()) == '') {
                        $('#txtUserNameUsrAcct').addClass('form-validation-error');
                        $('#spanUserNameUsrAcct').show();
                    } else {
                        if ($.trim($('#txtUserNameUsrAcct').val()).length < 8) {
                            $('#txtUserNameUsrAcct').addClass('form-validation-error');
                            $('#spanUserNameUsrAcctChars').show();
                        }

                        if (!validateAlphanumeric($.trim($('#txtUserNameUsrAcct').val()))) {
                            $('#spanUserNameUsrAcctAlphanum').show();
                            $('#txtUserNameUsrAcct').addClass('form-validation-error');
                        }
                    }

                    if ($.trim($('#txtPasswordUsrAcct').val()) != '' && $.trim($('#txtPasswordUsrAcct').val()).length < 8) {
                        $('#txtPasswordUsrAcct').addClass('form-validation-error');
                        $('#spanPasswordUsrAcctChars').show();
                    }
                    if ($.trim($('#txtPasswordUsrAcct').val()) == '') {
                        $('#txtPasswordUsrAcct').addClass('form-validation-error');
                        $('#spanPasswordUsrAcct').show();
                    }
                    if ($.trim($('#txtConfirmPasswordUsrAcct').val()) == '') {
                        $('#txtConfirmPasswordUsrAcct').addClass('form-validation-error');
                        $('#spanConfirmPasswordUsrAcct').show();
                    }

                }


                function validateFieldsAccountRecovery() {

                    if ($.trim($('#txtFirstName').val()) == '' || $.trim($('#txtFirstName').val()) == 'First Name') {
                        $('#txtFirstName').addClass('form-validation-error');
                        $('#spanFName').show();
                    }
                    if ($.trim($('#txtMiddleName').val()) == '' || $.trim($('#txtMiddleName').val()) == 'Middle Name') {
                        $('#txtMiddleName').addClass('form-validation-error');
                        $('#spanMName').show();
                    }
                    if ($.trim($('#txtSurname').val()) == '' || $.trim($('#txtSurname').val()) == 'Last Name') {
                        $('#txtSurname').addClass('form-validation-error');
                        $('#spanLName').show();
                    }

                    var fields = $("input[name='suffix']").serializeArray();

                    if (fields.length == 0) {

                        if ($.trim($('#selPersonalSuffixName').val()) == '') {
                            $('#chbNoSuffix').addClass('form-validation-error');

                            $('#spanSuffix').show();
                            $('#selPersonalSuffixName').addClass('form-validation-error');
                        }
                    }

                    if ($.trim($('#selDOBMonth').val()) == '') {
                        $('#selDOBMonth').addClass('form-validation-error');
                        $('#spanDateOfBirth').show();
                    }
                    if ($.trim($('#selDOBDay').val()) == '') {
                        $('#selDOBDay').addClass('form-validation-error');
                        $('#spanDateOfBirth').show();
                    }
                    if ($.trim($('#selDOBYear').val()) == '') {
                        $('#selDOBYear').addClass('form-validation-error');
                        $('#spanDateOfBirth').show();
                    }

                    if ($.trim($('#txtMotherMaidenFirstName').val()) == '' || $.trim($('#txtMotherMaidenFirstName').val()) == "Mother's First Name") {
                        $('#txtMotherMaidenFirstName').addClass('form-validation-error');
                        $('#spanMotherFName').show();
                    }
                    if ($.trim($('#txtMotherMaidenMiddleName').val()) == '' || $.trim($('#txtMotherMaidenMiddleName').val()) == "Mother's Maiden Middle Name") {
                        $('#txtMotherMaidenMiddleName').addClass('form-validation-error');
                        $('#spanMotherMName').show();
                    }
                    if ($.trim($('#txtMotherMaidenSurname').val()) == '' || $.trim($('#txtMotherMaidenSurname').val()) == "Mother's Maiden Last Name") {
                        $('#txtMotherMaidenSurname').addClass('form-validation-error');
                        $('#spanMotherLName').show();
                    }

                }
                function validateFields() {

                    validateFieldsAccountRecovery();


                    if ($.trim($('#txtTelephone').val()) != '' && $.trim($('#txtTelephone').val()) != 'xxx-xxxx') {
                        if (!validatePhoneNumber($.trim($('#txtTelephone').val())) || $('#txtTelephone').val().length < 7) {
                            $('#spanTelNoValid').show();
                            $('#txtTelephone').addClass('form-validation-error');
                        }
                    }

                    if ($.trim($('#txtMobile').val()) != '' && $.trim($('#txtMobile').val()) != 'Enter your 7-digit mobile number') {
                        if (!validateNumber($.trim($('#txtMobile').val()))) {
                            $('#spanMobileValid').show();
                            $('#txtMobile').addClass('form-validation-error');
                        }
                    }


                    //                    if ($.trim($('#selCallingCodeTel').val()) != '') {
                    //                        if ($('#selCallingCodeTel option:selected').text() == '63 - Philippines') {
                    //                            if ($.trim($('#selAreaCode').val()) != '') { //if he selected an area code
                    //                                if ($.trim($('#txtTelephone').val()) == '' || $.trim($('#txtTelephone').val()) == 'xxx-xxxx') { //check if there is no tel no
                    //                                    $('#spanTelNoValid').show();
                    //                                    $('#txtTelephone').addClass('form-validation-error');
                    //                                }
                    //                            } else { //if he DIDNT select an area code
                    //                                if ($.trim($('#txtTelephone').val()) != '' && $.trim($('#txtTelephone').val()) != 'xxx-xxxx') {
                    //                                    $('#spanAreaCode').show();
                    //                                    $('#selAreaCode').addClass('form-validation-error');
                    //                                }
                    //                            }
                    //                        } else { //if didnt select philippines
                    //                            if ($.trim($('#txtTelephone').val()) == '' || $.trim($('#txtTelephone').val()) == 'xxx-xxxx') {
                    //                                $('#spanTelNoValid').show();
                    //                                $('#txtTelephone').addClass('form-validation-error');
                    //                            }
                    //                        }
                    //                    } else { //if he DIDNT select a country
                    //                        if ($.trim($('#txtTelephone').val()) != '' && $.trim($('#txtTelephone').val()) != 'xxx-xxxx') {
                    //                            $('#spanCountryCodeTelNo').show();
                    //                            $('#selCallingCodeTel').addClass('form-validation-error');
                    //                        }
                    //                    }

                    //                    if ($.trim($('#selCallingCodeMobile').val()) != '') {
                    //                        if ($('#selCallingCodeMobile option:selected').text() == '63 - Philippines') {
                    //                            if ($.trim($('#selTelcoPrefix').val()) != '') { //if he selected a telco prefix
                    //                                if ($.trim($('#txtMobile').val()) == '' || $.trim($('#txtMobile').val()) == 'Enter your 7-digit mobile number') {
                    //                                    $('#spanMobileValid').show();
                    //                                    $('#txtMobile').addClass('form-validation-error');
                    //                                }
                    //                            } else {//if he DIDNT select a telco prefix
                    //                                if ($.trim($('#txtMobile').val()) != '' && $.trim($('#txtMobile').val()) != 'Enter your 7-digit mobile number') {
                    //                                    $('#spanTelcoPrefix').show();
                    //                                    $('#selTelcoPrefix').addClass('form-validation-error');
                    //                                }
                    //                            }

                    //                        } else { //if didnt select philippines
                    //                            if ($.trim($('#txtMobile').val()) == '' || $.trim($('#txtMobile').val()) == 'Enter your 7-digit mobile number') {
                    //                                $('#spanMobileValid').show();
                    //                                $('#txtMobile').addClass('form-validation-error');
                    //                            }
                    //                        }
                    //                    } else { //if he DIDNT select a country
                    //                        if ($.trim($('#txtMobile').val()) != '' && $.trim($('#txtMobile').val()) != 'Enter your 7-digit mobile number') {
                    //                            $('#spanCountryCodeMobile').show();
                    //                            $('#selCallingCodeMobile').addClass('form-validation-error');
                    //                        }
                    //                    }

                    var fields2 = $("input[name='email']").serializeArray();

                    if ($.trim($('#txtEmail').val()) != '' && fields2.length == 0) {

                        if (!validateEmail($.trim($('#txtEmail').val()))) {
                            $('#spanEmail').show();
                            $('#txtEmail').addClass('form-validation-error');
                        }

                    }

                    if ($.trim($('#txtUserName').val()) == '') {
                        $('#txtUserName').addClass('form-validation-error');
                        $('#spanUserName').show();
                    } else {
                        if ($.trim($('#txtUserName').val()).length < 8) {
                            $('#txtUserName').addClass('form-validation-error');
                            $('#spanUserNameChars').show();
                        }

                        if (!validateAlphanumeric($.trim($('#txtUserName').val()))) {
                            $('#spanUserNameAlphanum').show();
                            $('#txtUserName').addClass('form-validation-error');
                        }

                    }

                    if ($.trim($('#txtPassWord').val()) != '' && $.trim($('#txtPassWord').val()).length < 8) {
                        $('#txtPassWord').addClass('form-validation-error');
                        $('#spanPasswordChars').show();
                    }
                    if ($.trim($('#txtPassWord').val()) == '') {
                        $('#txtPassWord').addClass('form-validation-error');
                        $('#spanPassword').show();
                    }
                    if ($.trim($('#txtConfirmPassword').val()) == '') {
                        $('#txtConfirmPassword').addClass('form-validation-error');
                        $('#spanConfirmPassword').show();
                    }

                    //validate only if first party sign up is true
                    if (isFirstPartySignUpValue == 1) {
                        if ($.trim($('#selQuestion1').val()) == '') {
                            $('#selQuestion1').addClass('form-validation-error');
                            $('#spanAnswer1').show();
                        }

                        if ($.trim($('#txtQ1Answer').val()) == '') {
                            $('#txtQ1Answer').addClass('form-validation-error');
                            $('#spanAnswer1').show();
                        }

                        if ($.trim($('#selQuestion2').val()) == '') {
                            $('#selQuestion2').addClass('form-validation-error');
                            $('#spanAnswer2').show();
                        }

                        if ($.trim($('#txtQ2Answer').val()) == '') {
                            $('#txtQ2Answer').addClass('form-validation-error');
                            $('#spanAnswer2').show();
                        }
                    }

                    //                    if ($.trim($('#txtPIN1').val()) == '' || $.trim($('#txtPIN2').val()) == '') {
                    //                        $('#txtPIN1').addClass('form-validation-error');
                    //                        $('#spanPIN1').show();
                    //                        $('#txtPIN2').addClass('form-validation-error');
                    //                        $('#spanPIN2').show();
                    //                    }

                    //                    if (!IsNumeric($.trim($('#txtPIN1').val())) || !IsNumeric($.trim($('#txtPIN2').val()))) {
                    //                        $('#txtPIN2').addClass('form-validation-error');
                    //                        $('#spanPIN2').show();
                    //                    }

                }
                //End of validation functions


                // Save functions
                function ShowSecurityQuestion() {
                    var pData = {};
                    pData.Surname = $.trim($("#txtSurname").val());
                    pData.FirstName = $.trim($("#txtFirstName").val());
                    pData.MiddleName = $.trim($("#txtMiddleName").val());
                    pData.PersonalSuffixName = $.trim($("#selPersonalSuffixName").val());
                    pData.DateOfBirth = $("#selDOBMonth").val() + '/' + $("#selDOBDay").val() + '/' + $("#selDOBYear").val();
                    pData.MotherMaidenSurname = $.trim($("#txtMotherMaidenSurname").val());
                    pData.MotherMaidenFirstName = $.trim($("#txtMotherMaidenFirstName").val());
                    pData.MotherMaidenMiddleName = $.trim($("#txtMotherMaidenMiddleName").val());

                    AjaxSend({
                        url: "AccountRecovery/getPeopleSecurityQuestionList",
                        data: pData,
                        success: function (vData) {

                            if (vData.PeopleSecurityData.Rows.length > 0) {

                                for (var p = 0; p <= vData.PeopleSecurityData.Rows.length - 1; p++) {

                                    $("#txtPeopleID").val(vData.PeopleSecurityData.Rows[p].PeopleID);
                                    if (p == 0) {
                                        $("#txtQ1ID").val(vData.PeopleSecurityData.Rows[p].ID);
                                        $("#lblQuestion1").text(vData.PeopleSecurityData.Rows[p].SecurityQuestion);

                                    }
                                    else if (p == 1) {
                                        $("#txtQ2ID").val(vData.PeopleSecurityData.Rows[p].ID);
                                        $("#lblQuestion2").text(vData.PeopleSecurityData.Rows[p].SecurityQuestion);

                                    }
                                    $("#divSecurityQuestion").show();
                                    $("#divFullDetails").hide();
                                };


                            }
                            else {
                                alert("The information you entered does not match any account in our records.");
                            }
                        }
                    });

                }

                function CheckSecurityQuestion() {
                    var pData = [];

                    pData = {
                        PeopleID: $.trim($("#txtPeopleID").val()),
                        Question1ID: $.trim($("#txtQ1ID").val()),
                        Question1Answer: $.trim($("#txtQ1AnswerAccountRecovery").val()),
                        Question2ID: $.trim($("#txtQ2ID").val()),
                        Question2Answer: $.trim($("#txtQ2AnswerAccountRecovery").val())
                    };

                    AjaxSend({
                        url: "People/CheckPeopleSecurityQuestions",
                        data: pData,
                        success: function (vData) {

                            if (vData.PeopleSecurityData == "INVALID") {
                                alert("You failed to answer the security questions correctly.");
                            }
                            else {
                                $("#divUserAccount").show();
                                $("#divSecurityQuestion").hide();
                                $("#divFullDetails").hide();
                            }

                        }
                    });

                }

                function SavePersonUserAccountRecovery() {

                    var pData = {};
                    var pDataUsername = {};

                    pDataUsername.Username = $("#txtUserNameUsrAcct").val();

                    pData.PeopleID = $.trim($("#txtPeopleID").val());
                    pData.Username = $.trim($("#txtUserNameUsrAcct").val());
                    pData.UserPassword = $.trim($("#txtPasswordUsrAcct").val());
                    pData.UserConfirmPassword = $.trim($("#txtConfirmPasswordUsrAcct").val());

                    AjaxSend({
                        url: 'SignUp/CheckPeopleUniqueUserName',
                        data: pDataUsername,
                        success: function (vData) {

                            if (vData.PeopleUserNameData.Rows.length > 0) {

                                if (vData.PeopleUserNameData.Rows[0].PeopleID == $.trim($("#txtPeopleID").val())) {
                                    //If username is the same as his existing username, let the user use it again

                                    AjaxSend({
                                        url: "AccountRecovery/SavePeopleUserAccountRecovery",
                                        data: pData,
                                        success: function (vData) {
                                            if (vData.PeopleUserAccountData != '' || vData.PeopleUserAccountData != null)
                                                location = '/signup/success';
                                            else
                                                alert('There was an error in saving your account.');

                                        }
                                    });

                                } else {
                                    alert('Someone already has that user name. Please try another.');
                                    return;
                                }


                            } else {
                                //If username is available, proceed to Save
                                AjaxSend({
                                    url: "AccountRecovery/SavePeopleUserAccountRecovery",
                                    data: pData,
                                    success: function (vData) {
                                        if (vData.PeopleUserAccountData != '' || vData.PeopleUserAccountData != null)
                                            location = '/signup/success';
                                        else
                                            alert('There was an error in saving your account.');

                                    }
                                });
                            }
                        } // success
                    }); // AjaxSend

                }

                function SavePersonDetails() {

                    var pData = {};
                    var pDataUsername = {};

                    var isTermAgreedValue;
                    var isCertifiedCorrectValue;


                    //TermAgreed
                    isTermAgreedValue = 1; //automatic value if you proceed to sign up page


                    //CertifiedCorrect
                    var fields = $("input[name='certified']").serializeArray();

                    if (fields.length > 0) {
                        isCertifiedCorrectValue = 1;

                    } else {
                        isCertifiedCorrectValue = 0;
                    }

                    //FirstPartySignUp
                    var fields = $("input[name='firstPartySignUp']").serializeArray();

                    if (fields.length > 0) { //if it is checked, then it's not a first party sign up because he's signing up on behalf of another person
                        isFirstPartySignUpValue = 0;

                    } else { //if it is not checked, then he is indeed signing up as himself (own profile)
                        isFirstPartySignUpValue = 1;
                    }


                    if ($.trim($('#txtMobile').val()) == 'Enter your 7-digit mobile number') {
                        $('#txtMobile').val('');
                    }


                    pDataUsername.Username = $("#txtUserName").val();

                    pData = {
                        PeopleID: "",
                        Title: $.trim($("#selTitle").val()),
                        Surname: $.trim($("#txtSurname").val()),
                        FirstName: $.trim($("#txtFirstName").val()),
                        MiddleName: $.trim($("#txtMiddleName").val()),
                        PersonalSuffixName: $.trim($("#selPersonalSuffixName").val()),
                        Nickname: $.trim($("#txtNickname").val()),
                        DateOfBirth: $("#selDOBMonth").val() + '/' + $("#selDOBDay").val() + '/' + $("#selDOBYear").val(),
                        MotherMaidenSurname: $.trim($("#txtMotherMaidenSurname").val()),
                        MotherMaidenFirstName: $.trim($("#txtMotherMaidenFirstName").val()),
                        MotherMaidenMiddleName: $.trim($("#txtMotherMaidenMiddleName").val()),
                        FullCheckName: "1",

                        PersonContactCountryIDMobile: $.trim($('#selCallingCodeMobile').val()),
                        PersonContactCountryIDTel: $.trim($('#selCallingCodeTel').val()),
                        PersonContactAreaCodeID: $.trim($('#selAreaCode').val()),
                        PersonContactTelCoPrefixID: $.trim($('#selTelcoPrefix').val()),
                        PersonContactNoMobile: $.trim($('#txtMobile').val()),
                        PersonContactNoTel: $.trim($('#txtTelephone').val()),
                        PersonContactTypeTel: '',
                        PersonContactTypeMobile: '',

                        PersonContactEmail: $.trim($("#txtEmail").val()),
                        PersonContactEmailType: '', //temp

                        UserName: $.trim($("#txtUserName").val()),
                        UserPassword: $.trim($("#txtPassWord").val()),
                        UserConfirmPassword: $.trim($("#txtConfirmPassword").val()),

                        Question1: $.trim($("#selQuestion1").val()),
                        Question1Answer: $.trim($("#txtQ1Answer").val()),
                        Question2: $.trim($("#selQuestion2").val()),
                        Question2Answer: $.trim($("#txtQ2Answer").val()),

                        IsTermAgreed: isTermAgreedValue,
                        IsCertifiedCorrect: isCertifiedCorrectValue,
                        IsFirstPartySignUp: isFirstPartySignUpValue,

                        PIN: $.trim($('#txtPIN1').val())

                    };

                    //                if (InAndroid) {
                    //                    PeopleLogger_offline().CheckUser(pData);
                    //                } else {
                    if (proj.IsCookieless == 1) {
                        pData.IsCookieless = 1;
                        docCookies.removeItem('ASP.NET_SessionId');
                    };

                    AjaxSend({
                        url: 'SignUp/CheckPeopleUniqueUserName',
                        data: pDataUsername,
                        success: function (vData) {

                            if (vData.PeopleUserNameData.Rows.length > 0) {
                                alert('Someone already has that user name. Please try another.');
                                $('#btnSignUp').removeAttr("disabled");
                            }
                            else {
                                //If username is available, proceed to Save
                                AjaxSend({
                                    url: "SignUp/SavePeopleInfoNameInitialForSystemAdministrationOnly",
                                    data: pData,
                                    success: function (vSaveData) {
                                        if (vSaveData.PeopleNameData == "DateOfBirthError") {
                                            alert("Please enter valid date of birth.");
                                            $('#divSignUpLoading').hide();
                                            $('#btnSignUp').removeAttr("disabled");
                                        } else if (vSaveData.PeopleNameData == "Error") {
                                            alert("There was an error in saving your data.");
                                            $('#divSignUpLoading').hide();
                                            $('#btnSignUp').removeAttr("disabled");
                                        }
                                        else {
                                            if (vSaveData.PeopleNameData != '' || vSaveData.PeopleNameData != null) {

                                                //console.log(vSaveData.PeopleNameData);
                                                sessionStorage.EmpFirstName = $("#txtFirstName").val();
                                                $('#pageOrgEmployeeAdd').hide();
                                                sessionStorage["OptionToAdd"] = "SendInvite";
                                                sessionStorage["txtOrgPeopleID"] = vSaveData.PeopleNameData;
                                                sessionStorage["btnOrgEmployeeName"] = pData.Surname + ', ' + pData.FirstName + ' ' + pData.MiddleName;
                                                sessionStorage["AddLibBCName"] = "Add Employee";
                                                window.location.hash = '';
                                                window.location.hash = 'UniversalPage';
                                                if (sessionStorage.SystemAdminFor == 'dtr') {
                                                    DTRLibraryTable.AddEmployeePage();
                                                } else {
                                                    LibraryTable.AddEmployeePage();
                                                }
                                            } else {
                                                $('#divSignUpLoading').hide();
                                                $('#btnSignUp').removeAttr("disabled");
                                                alert('Registration failed. Please sign up again.');
                                            }

                                        }

                                    }
                                });
                            }
                        } // success
                    }); // AjaxSend
                    //                }; // if (InAndroid) {

                } // function CheckUser() {
                //End of save functions


                function CheckUser() {
                    var pData = [];
                    $('#btnSignUp').attr("disabled", "disabled");
                    $('#divSignUpLoading').show();

                    pData = {
                        PeopleID: "",

                        Surname: $.trim($("#txtSurname").val()),
                        FirstName: $.trim($("#txtFirstName").val()),
                        MiddleName: $.trim($("#txtMiddleName").val()),
                        PersonalSuffixName: $.trim($("#selPersonalSuffixName").val()),

                        DateOfBirth: $("#selDOBMonth").val() + '/' + $("#selDOBDay").val() + '/' + $("#selDOBYear").val(),
                        MotherMaidenSurname: $.trim($("#txtMotherMaidenSurname").val()),
                        MotherMaidenFirstName: $.trim($("#txtMotherMaidenFirstName").val()),
                        MotherMaidenMiddleName: $.trim($("#txtMotherMaidenMiddleName").val()),
                        FullCheckName: "1"
                    };

                    AjaxSend({
                        url: "SignUp/CheckPeopleUniqueName",
                        data: pData,
                        success: function (vData) {

                            if (vData.PeopleNameData.Rows.length > 0) {

                                if (confirm('The personal information you entered already exists in our records. Do you want to retrieve your password?')) {
                                    // Show security question
                                    window.open(GetPathFormat("/AccountRecovery"), "_self");

                                } else {
                                    // Do nothing!
                                }
                                $('#divSignUpLoading').hide();
                                $('#btnSignUp').removeAttr("disabled");
                            }
                            else {
                                // Proceed to Save
                                SavePersonDetails();

                            }

                        }
                    });

                }


                function getCallingCode() {

                    AjaxSend({
                        url: 'SignUp/getCallingCodeList',
                        success: function (vData) {

                            var vRows = vData.CallingCodeData.Rows;

                            $('#selCallingCodeMobile').empty();
                            $('#selCallingCodeTel').empty();

                            $('#selCallingCodeMobile').append(new Option('Please Select Country Code', ''));
                            $('#selCallingCodeTel').append(new Option('Please Select Country Code', ''));
                            for (var p = 0; p <= vRows.length - 1; p++) {
                                $('#selCallingCodeMobile').append(new Option((vRows[p].CallingCode == undefined ? '' : vRows[p].CallingCode) + ' - ' + vRows[p].Name, vRows[p].ID, false, false));
                                $('#selCallingCodeTel').append(new Option((vRows[p].CallingCode == undefined ? '' : vRows[p].CallingCode) + ' - ' + vRows[p].Name, vRows[p].ID, false, false));
                            }

                            $("#selCallingCodeMobile option:contains('63 - Philippines')").attr('selected', 'selected');
                            $("#selCallingCodeTel option:contains('63 - Philippines')").attr('selected', 'selected');

                            getTelcoPrefix();
                            getAreaCode();
                            $('#spanCountryCodeMobile').hide();
                            $('#selCallingCodeMobile').removeClass('form-validation-error');

                            $('#spanCountryCodeTelNo').hide();
                            $('#selCallingCodeTel').removeClass('form-validation-error');


                        },
                        error: function (vError) {
                            var pDummy;
                        }
                    });

                }

                function getAreaCode() {

                    AjaxSend({
                        url: 'SignUp/getAreaCodeList',
                        success: function (vData) {

                            var vRows = vData.AreaCodeData.Rows;

                            $('#selAreaCode').empty();

                            $('#selAreaCode').append(new Option('Please Select Area Code', ''));
                            for (var p = 0; p <= vRows.length - 1; p++) {
                                $('#selAreaCode').append(new Option(vRows[p].CallingCode + ' - ' + vRows[p].Area, vRows[p].ID, false, false));
                            }
                        },
                        error: function (vError) {
                            var pDummy;
                        }
                    });

                }

                function getTelcoPrefix() {

                    AjaxSend({
                        url: 'SignUp/getTelcoPrefixList',
                        success: function (vData) {

                            var vRows = vData.TelcoPrefixData.Rows;

                            $('#selTelcoPrefix').empty();

                            $('#selTelcoPrefix').append(new Option('Please Select Mobile Number Prefix', ''));
                            for (var p = 0; p <= vRows.length - 1; p++) {
                                $('#selTelcoPrefix').append(new Option(vRows[p].Prefix + ' - ' + vRows[p].Name, vRows[p].ID, false, false));
                            }
                        },
                        error: function (vError) {
                            var pDummy;
                        }
                    });

                }


            }); //End Inner function

        }); // require([(IsMobile ? '/Scripts3/common/mobile.js?crc=' : '')], function() {

    });                   // require


    