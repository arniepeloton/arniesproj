require([
    'common'
    ], function () {
        $(function () {
            $('html').removeClass('ui-mobile');

            $(document).ready(function () {
                $('.ui-loader').hide();
                AjaxSend({
                    url: 'AccountManagement/IsNewEntry',
                    success: function (vData) {
                        if (vData.IsNew == "FALSE") {
                            if (vData.PeopleID == null || vData.PeopleID == '') {
                                $('#lblMsg').show();
                            } else {
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

            function validateAlphanumeric(string) {
                var re = /^[a-zA-Z0-9\_]+$/;

                return re.test(string);
            }

            function SignOut() {

                AjaxSend({
                    url: "People/setSignOut",
                    success: function (vData) {
                        window.open(GetPathFormat("/SignIn?mode=logout"), "_self"); // window.open(GetPathFormat("/People"), "_self"); EDITED BY: JANINE , Aug 3 4pm
                    }
                });

            };

            $('#txtQ1AnswerManage').change(function () {
                //$('#spanQuestion1').hide();
                if ($.trim($(this).val()) == '') {
                    $('#spanAnswer1Manage').show();
                    $('#txtQ1AnswerManage').addClass('form-validation-error');
                } else {
                    $('#spanAnswer1Manage').hide();
                    $('#txtQ1AnswerManage').removeClass('form-validation-error');
                }

            });

            $('#txtQ2AnswerManage').change(function () {
                //$('#spanQuestion1').hide();
                if ($.trim($(this).val()) == '') {
                    $('#spanAnswer2Manage').show();
                    $('#txtQ2AnswerManage').addClass('form-validation-error');

                } else {
                    $('#spanAnswer2Manage').hide();
                    $('#txtQ2AnswerManage').removeClass('form-validation-error');
                }

            });


            $('#selQuestion1Manage').change(function () {
                $('#txtQ1AnswerManage').val('');
                $('#spanQuestion1Manage').hide();
                $('#spanQuestion2Manage').hide();
                if ($.trim($(this).val()) == '') {
                    $('#spanAnswer1Manage').show();
                    $('#selQuestion1Manage').addClass('form-validation-error');
                } else {

                    $('#spanAnswer1Manage').hide();
                    $('#spanQuestion1Manage').hide();
                    //Check if question is already identical to the other
                    if ($.trim($(this).val()) == $.trim($('#selQuestion2Manage').val())) {
                        $('#spanQuestion1Manage').show();

                    }
                    $('#selQuestion1Manage').removeClass('form-validation-error');
                }

            });

            $('#selQuestion2Manage').change(function () {

                $('#txtQ2AnswerManage').val('');
                $('#spanQuestion2Manage').hide();
                $('#spanQuestion1Manage').hide();
                if ($.trim($(this).val()) == '') {
                    $('#spanAnswer2Manage').show();
                    $('#selQuestion2Manage').addClass('form-validation-error');
                } else {

                    $('#spanAnswer2Manage').hide();
                    $('#spanQuestion2Manage').hide();
                    //Check if question is already identical to the other
                    if ($.trim($(this).val()) == $.trim($('#selQuestion1Manage').val())) {
                        $('#spanQuestion2Manage').show();

                    }
                    $('#selQuestion2Manage').removeClass('form-validation-error');
                }

            });



            $('#txtUserNameManage').change(function () {
                $('#spanUserNameManage').hide();
                $('#spanUserNameManageChars').hide();
                $('#spanUserNameManageAlphanum').hide();

                $(this).removeClass('form-validation-error');

                if ($.trim($(this).val()) == '') {
                    $('#spanUserNameManage').show();
                    $(this).addClass('form-validation-error');
                } else {
                    if ($.trim($(this).val()).length < 8) {
                        $('#spanUserNameManageChars').show();
                        $(this).addClass('form-validation-error');
                    }
                    else {
                        if (!validateAlphanumeric($.trim($(this).val()))) {

                            $('#spanUserNameManageAlphanum').show();
                            $(this).addClass('form-validation-error');
                        }


                    }
                }
            });

            $('#txtPasswordManage').change(function () {
                $('#txtConfirmPasswordManage').val('');
                $('#spanPasswordRepeatManage').hide();
                if ($.trim($(this).val()) == '') {
                    $('#spanPasswordManage').show();
                    $('#spanPasswordManageChars').hide();
                    $('#txtPasswordManage').addClass('form-validation-error');

                } else {
                    if ($.trim($(this).val()).length < 8) {
                        $('#spanPasswordManageChars').show();
                        $('#txtPasswordManage').addClass('form-validation-error');
                    } else {
                        $('#spanPasswordManageChars').hide();
                        $('#txtPasswordManage').removeClass('form-validation-error');
                    }
                    $('#spanPasswordManage').hide();
                }

            });

            $('#txtConfirmPasswordManage').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanConfirmPasswordManage').show();
                    $('#spanPasswordRepeatManage').hide();
                    $('#txtConfirmPasswordManage').addClass('form-validation-error');

                } else {
                    if ($.trim($(this).val()) != $.trim($('#txtPasswordManage').val()) && $.trim($('#txtPasswordManage').val()) != '') {
                        $('#spanPasswordRepeatManage').show();
                        $('#txtConfirmPasswordManage').addClass('form-validation-error');
                    } else {
                        $('#txtConfirmPasswordManage').removeClass('form-validation-error');
                    }

                    $('#spanConfirmPasswordManage').hide();

                }

            });

            $('#btnSubmitManage').click(function (e) {
                e.preventDefault();
                validateFields();

                if ($.trim($('#txtQ1AnswerManage').val()) == '') {
                    return;
                }

                if ($.trim($('#txtQ2AnswerManage').val()) == '') {
                    return;
                }


                if ($.trim($('#selQuestion1Manage').val()) == '') {
                    return;
                } else {
                    if ($.trim($('#txtQ1AnswerManage').val()) == '') {
                        return;
                    }
                    //Check if question is already identical to the other
                    if ($.trim($('#selQuestion1Manage').val()) == $.trim($('#selQuestion2Manage').val())) {
                        return;
                    }
                }

                if ($.trim($('#selQuestion2Manage').val()) == '') {
                    return;
                } else {
                    if ($.trim($('#txtQ2AnswerManage').val()) == '') {
                        return;
                    }
                    //Check if question is already identical to the other
                    if ($.trim($('#selQuestion2Manage').val()) == $.trim($('#selQuestion1Manage').val())) {
                        return;
                    }
                }

                if ($.trim($('#txtUserNameManage').val()) == '') {
                    return;
                } else {
                    if ($.trim($('#txtUserNameManage').val()).length < 8) {
                        return;
                    }

                    if (!validateAlphanumeric($.trim($('#txtUserNameManage').val()))) {
                        return;
                    }

                }

                if ($.trim($('#txtPasswordManage').val()) == '' || $.trim($('#txtPasswordManage').val()).length < 8) {
                    return;
                }

                if ($.trim($('#txtConfirmPasswordManage').val()) == '') {
                    return;
                }

                if ($.trim($('#txtPasswordManage').val()) != '') {
                    if ($.trim($('#txtPasswordManage').val()) != $.trim($('#txtConfirmPasswordManage').val()) && $.trim($('#txtConfirmPasswordManage').val()) != '') {
                        return;
                    }
                }

                if ($.trim($('#txtConfirmPasswordManage').val()) != '') {
                    if ($.trim($('#txtConfirmPasswordManage').val()) != $.trim($('#txtPasswordManage').val()) && $.trim($('#txtPasswordManage').val()) != '') {
                        return;
                    }
                }

                CheckPeopleUserAccountUnchanged();
            });

            function ShowPersonInfo(strID) {
                var pData = {};
                pData.ID = strID;

                AjaxSend({
                    url: "SignIn/CheckPeopleIsFirstPartySignUp",
                    data: pData,
                    success: function (vData) {

                        if (vData.PeopleFirstPartySignUpData.Rows[0].IsFirstPartySignUp == '0') {
                            $('#lblUser').text(vData.PeopleFirstPartySignUpData.Rows[0].FirstName);
                            $('#lblName').text(vData.PeopleFirstPartySignUpData.Rows[0].FirstName);
                            $('#divUserAccount').show();
                            $('#menuSignIn').hide();
                            $('#menuSignOut').show();
                            $('#lblMsg').hide();
                        } else {
                            $('#lblMsg').show();
                        }
                    }
                });

            };


            function validateFields() {
                if ($.trim($('#selQuestion1Manage').val()) == '') {
                    $('#selQuestion1Manage').addClass('form-validation-error');
                    $('#spanAnswer1Manage').show();
                }

                if ($.trim($('#txtQ1AnswerManage').val()) == '') {
                    $('#txtQ1AnswerManage').addClass('form-validation-error');
                    $('#spanAnswer1Manage').show();
                }

                if ($.trim($('#selQuestion2Manage').val()) == '') {
                    $('#selQuestion2Manage').addClass('form-validation-error');
                    $('#spanAnswer2Manage').show();
                }

                if ($.trim($('#txtQ2AnswerManage').val()) == '') {
                    $('#txtQ2AnswerManage').addClass('form-validation-error');
                    $('#spanAnswer2Manage').show();
                }

                if ($.trim($('#txtUserNameManage').val()) == '') {
                    $('#txtUserNameManage').addClass('form-validation-error');
                    $('#spanUserNameManage').show();
                } else {
                    if ($.trim($('#txtUserNameManage').val()).length < 8) {
                        $('#txtUserNameManage').addClass('form-validation-error');
                        $('#spanUserNameManageChars').show();
                    }

                    if (!validateAlphanumeric($.trim($('#txtUserNameManage').val()))) {
                        $('#spanUserNameManageAlphanum').show();
                        $('#txtUserNameManage').addClass('form-validation-error');
                    }
                }

                if ($.trim($('#txtPasswordManage').val()) != '' && $.trim($('#txtPasswordManage').val()).length < 8) {
                    $('#txtPasswordManage').addClass('form-validation-error');
                    $('#spanPasswordManageChars').show();
                }
                if ($.trim($('#txtPasswordManage').val()) == '') {
                    $('#txtPasswordManage').addClass('form-validation-error');
                    $('#spanPasswordManage').show();
                }
                if ($.trim($('#txtConfirmPasswordManage').val()) == '') {
                    $('#txtConfirmPasswordManage').addClass('form-validation-error');
                    $('#spanConfirmPasswordManage').show();
                }

            }

            function CheckPeopleUserAccountUnchanged() {
                $('#btnSubmitManage').attr("disabled", "disabled");
                var pData = {};

                pData.Username = $('#txtUserNameManage').val();
                pData.UserPassword = $('#txtPasswordManage').val();

                AjaxSend({
                    url: 'AccountManagement/CheckPeopleUserAccountUnchanged',
                    data: pData,
                    success: function (vData) {

                        if ($.trim(vData.PeopleUserAccountData) == 'True') {
                            $('#btnSubmitManage').removeAttr("disabled");
                            alert('You are required to change your user name and password.');
                            return;
                        } else {
                            SavePersonUserAccountManage();
                        }
                    } // success
                }); // AjaxSend

            } // function CheckUser() 

            function SavePersonUserAccountManage() {
                var pDataUsername = {};
                pDataUsername.Username = $('#txtUserNameManage').val();

                var pDataID = {};
                pDataID.ID = $.trim($('#txtPeopleID').val());

                var pData = {};
                pData.PeopleID = $.trim($('#txtPeopleID').val());
                pData.Username = $.trim($('#txtUserNameManage').val());
                pData.UserPassword = $.trim($('#txtPasswordManage').val());
                pData.UserConfirmPassword = $.trim($('#txtConfirmPasswordManage').val());

                pData.Question1 = $.trim($("#selQuestion1Manage").val());
                pData.Question1Answer = $.trim($("#txtQ1AnswerManage").val());
                pData.Question2 = $.trim($("#selQuestion2Manage").val());
                pData.Question1Answer2 = $.trim($("#txtQ2AnswerManage").val());

                AjaxSend({
                    url: 'SignUp/CheckPeopleUniqueUserName',
                    data: pDataUsername,
                    success: function (vData) {

                        if (vData.PeopleUserNameData.Rows.length > 0) {
                            //loop through all the results
                            for (var p = 0; p <= vData.PeopleUserNameData.Rows.length - 1; p++) {
                                if (vData.PeopleUserNameData.Rows[p].PeopleID == $.trim($("#txtPeopleID").val())) {
                                    //If username is the same as his existing username, DO NOT ALLOW
                                    if (vData.PeopleUserNameData.Rows[p].Username == $.trim($("#txtUserNameManage").val())) {
                                        alert('You are required to change your user name. Please try a different one.');
                                    }
                                } else
                                    alert('Someone already has that user name. Please try another.');
                            }
                            $('#btnSubmitManage').removeAttr("disabled");
                        }
                        else {
                            //If username is available, proceed to Save
                            AjaxSend({
                                url: "AccountManagement/SavePeopleUserAccountManage",
                                data: pData,
                                success: function (vData) {
                                    if (vData.PeopleUserAccountManageData != '' || vData.PeopleUserAccountManageData != null) {
                                        //Change IsFirstPartySignUp flag to 1

                                        AjaxSend({
                                            url: "SignUp/SaveIsFirstPartySignUp",
                                            data: pDataID,
                                            success: function (vData) {
                                                if (vData.PeopleIsFirstPartySignUpData != '' || vData.PeopleIsFirstPartySignUpData != null)
                                                    location = '/signup/success' + (sessionStorage.system == undefined ? '' : '?sys=' + sessionStorage.system);
                                                else
                                                    alert('There was an error in saving your account.');

                                            }
                                        });
                                    }
                                    else
                                        alert('There was an error in saving your account.');

                                }
                            });

                            $('#btnSubmitManage').removeAttr("disabled");

                        }
                    } // success
                }); // AjaxSend

            }
        }); //Inner function


    });

