require([
    'common'
    ], function () {

        $(function () {

            setMonthCollection('selDOBMonth');
            setYearCollection('selDOBYear');

            //Onchange functions
            //Sign Up page controls

            $('#txtFirstName').change(function () {
                if ($.trim($(this).val()) == '' || $.trim($(this).val()) == 'First Name') {
                    $('#spanName').show();
                    $('#txtFirstName').addClass('form-validation-error');

                } else {
                    $('#spanName').hide();
                    $('#txtFirstName').removeClass('form-validation-error');
                }

            });

            $('#txtMiddleName').change(function () {
                if ($.trim($(this).val()) == '' || $.trim($(this).val()) == 'Middle Name') {
                    $('#spanName').show();
                    $('#txtMiddleName').addClass('form-validation-error');

                } else {
                    $('#spanName').hide();
                    $('#txtMiddleName').removeClass('form-validation-error');
                }

            });

            $('#txtSurname').change(function () {
                if ($.trim($(this).val()) == '' || $.trim($(this).val()) == 'Last Name') {
                    $('#spanName').show();
                    $('#txtSurname').addClass('form-validation-error');

                } else {
                    $('#spanName').hide();
                    $('#txtSurname').removeClass('form-validation-error');
                }

            });

            $('#chbNoSuffix').click(function () {
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


            });

            $('#selPersonalSuffixName').change(function () {
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


            });

            $('#selDOBDay').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanDateOfBirth').show();
                    $('#selDOBDay').addClass('form-validation-error');
                }
                else {
                    $('#spanDateOfBirth').hide();
                    $('#selDOBDay').removeClass('form-validation-error');
                }
            });


            $('#selDOBMonth').change(function () {

                if ($.trim($(this).val()) == '') {
                    $('#spanDateOfBirth').show();
                    $('#selDOBMonth').addClass('form-validation-error');
                }
                else {
                    $('#spanDateOfBirth').hide();
                    $('#selDOBMonth').removeClass('form-validation-error');
                }

                setDayCollection('selDOBMonth', 'selDOBDay', 'selDOBYear', false);

            });

            $('#selDOBYear').change(function () {
                $('#spanDateOfBirth').hide();
                setDayCollection('selDOBMonth', 'selDOBDay', 'selDOBYear', false);
                $('#selDOBYear').removeClass('form-validation-error');
            }).click(function () {
                if ($.trim($('#selDOBYear').val()) == "") {
                    $('#selDOBYear').val('1980');
                    //                $('#selDOBYear').selectmenu('refresh');
                }

                $('#selDOBYear').removeClass('form-validation-error');
            });


            $('#txtMotherMaidenFirstName').change(function () {
                if ($.trim($(this).val()) == '' || $.trim($(this).val()) == "Mother's First Name") {
                    $('#txtMotherMaidenFirstName').addClass('form-validation-error');
                    $('#spanMotherName').show();

                } else {
                    $('#spanMotherName').hide();
                    $('#txtMotherMaidenFirstName').removeClass('form-validation-error');
                }

            });

            $('#txtMotherMaidenMiddleName').change(function () {
                if ($.trim($(this).val()) == '' || $.trim($(this).val()) == "Mother's Maiden Middle Name") {
                    $('#txtMotherMaidenMiddleName').addClass('form-validation-error');
                    $('#spanMotherName').show();

                } else {
                    $('#spanMotherName').hide();
                    $('#txtMotherMaidenMiddleName').removeClass('form-validation-error');
                }

            });

            $('#txtMotherMaidenSurname').change(function () {
                if ($.trim($(this).val()) == '' || $.trim($(this).val()) == "Mother's Maiden Last Name") {
                    $('#txtMotherMaidenSurname').addClass('form-validation-error');
                    $('#spanMotherName').show();

                } else {
                    $('#spanMotherName').hide();
                    $('#txtMotherMaidenSurname').removeClass('form-validation-error');
                }

            });

            //End of Sign Up page controls

            //Account Recovery controls
            $('#txtQ1AnswerAccountRecovery').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanAnswer1AccountRecovery').show();
                    $('#txtQ1AnswerAccountRecovery').addClass('form-validation-error');

                } else {
                    $('#spanAnswer1AccountRecovery').hide();
                    $('#txtQ1AnswerAccountRecovery').removeClass('form-validation-error');
                }

            });

            $('#txtQ2AnswerAccountRecovery').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanAnswer2AccountRecovery').show();
                    $('#txtQ2AnswerAccountRecovery').addClass('form-validation-error');
                } else {
                    $('#spanAnswer2AccountRecovery').hide();
                    $('#txtQ2AnswerAccountRecovery').removeClass('form-validation-error');
                }

            });
            //End of Account Recovery controls

            //User Account controls

            $('#txtUserNameUsrAcct').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanUserNameUsrAcct').show();
                    $('#spanUserNameUsrAcctChars').hide();
                    $('#txtUserNameUsrAcct').addClass('form-validation-error');
                } else {
                    if ($.trim($(this).val()).length < 8) {
                        $('#spanUserNameUsrAcctChars').show();
                        $('#txtUserNameUsrAcct').addClass('form-validation-error');
                    }
                    else {
                        $('#spanUserNameUsrAcctChars').hide();
                        $('#txtUserNameUsrAcct').removeClass('form-validation-error');
                    }

                    $('#spanUserNameUsrAcct').hide();
                }

            });


            $('#txtPasswordUsrAcct').change(function () {
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

            });

            $('#txtConfirmPasswordUsrAcct').change(function () {
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

            });

            //End of User Account controls
            //End of onchange functions

            //Button click events


            $('#btnSubmitUsrAcct').click(function (e) {
                e.preventDefault();
                validateFieldsUsrAcct();


                if ($.trim($('#txtUserNameUsrAcct').val()) == '' || $.trim($('#txtUserNameUsrAcct').val()).length < 8) {
                    return;
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
                // CheckUser();


            });

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

                // alert('ok');
                ShowSecurityQuestion();

            });

            $('#btnSubmitSecurity').click(function (e) {
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

            });
            //End of button click events


            //Validation functions

            function validateFieldsUsrAcct() {

                if ($.trim($('#txtUserNameUsrAcct').val()) == '') {
                    $('#txtUserNameUsrAcct').addClass('form-validation-error');
                    $('#spanUserNameUsrAcct').show();
                }
                if ($.trim($('#txtUserNameUsrAcct').val()) != '' && $.trim($('#txtUserNameUsrAcct').val()).length < 8) {
                    $('#txtUserNameUsrAcct').addClass('form-validation-error');
                    $('#spanUserNameUsrAcctChars').show();
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
                    $('#spanName').show();
                }
                if ($.trim($('#txtMiddleName').val()) == '' || $.trim($('#txtMiddleName').val()) == 'Middle Name') {
                    $('#txtMiddleName').addClass('form-validation-error');
                    $('#spanName').show();
                }
                if ($.trim($('#txtSurname').val()) == '' || $.trim($('#txtSurname').val()) == 'Last Name') {
                    $('#txtSurname').addClass('form-validation-error');
                    $('#spanName').show();
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
                    $('#spanMotherName').show();
                }
                if ($.trim($('#txtMotherMaidenMiddleName').val()) == '' || $.trim($('#txtMotherMaidenMiddleName').val()) == "Mother's Maiden Middle Name") {
                    $('#txtMotherMaidenMiddleName').addClass('form-validation-error');
                    $('#spanMotherName').show();
                }
                if ($.trim($('#txtMotherMaidenSurname').val()) == '' || $.trim($('#txtMotherMaidenSurname').val()) == "Mother's Maiden Last Name") {
                    $('#txtMotherMaidenSurname').addClass('form-validation-error');
                    $('#spanMotherName').show();
                }

            }

            //End of validation functions


            // Save functions


            //End of save functions

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
                    url: "People/getPeopleSecurityQuestionList",
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
                            //loop through all the results
                            for (var p = 0; p <= vData.PeopleUserNameData.Rows.length - 1; p++) {
                                if (vData.PeopleUserNameData.Rows[p].PeopleID == $.trim($("#txtPeopleID").val())) {
                                    //If username is the same as his existing username, let the user use it again
                                    
                                    AjaxSend({
                                        url: "SignUp/SavePeopleUserAccountRecovery",
                                        data: pData,
                                        success: function (vData) {
                                            if (vData.PeopleUserAccountData != '' || vData.PeopleUserAccountData != null)
                                                window.open(GetPathFormat('/Success'), '_self');
                                            else
                                                alert('There was an error in saving your account.');

                                        }
                                    });

                                }
                                else
                                    alert('Someone already has that user name. Please try another.');
                            }



                        }
                        else {
                            //If username is available, proceed to Save
                            AjaxSend({
                                url: "AccountRecovery/SavePeopleUserAccount",
                                data: pData,
                                success: function (vData) {
                                    if (vData.PeopleUserAccountData != '' || vData.PeopleUserAccountData != null)
                                        window.open(GetPathFormat('/Success'), '_self');
                                    else
                                        alert('There was an error in saving your account.');

                                }
                            });
                        }
                    } // success
                }); // AjaxSend

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




        }); //End Inner function


    });


    