require(['common'], function () {
    require(['/Content/bootstrap/js/bootstrap.min.js'], function () {
        $(document).ready(function () {
            var mydetails, myemail, mycredentials, mysecq, isEditName = false, isChangePs = false, isEditAns1 = false, isEditAns2 = false;
            var txtMAFirstName = $('#txtMAFirstName');
            var txtMAMiddleName = $('#txtMAMiddleName');
            var txtMALastName = $('#txtMALastName');
            var selMAPersonGenSuffix = $('#selMAPersonGenSuffix');
            var selMABirthMonth = $('#selMABirthMonth');
            var selMABirthDay = $('#selMABirthDay');
            var txtMABirthYear = $('#txtMABirthYear');
            var txtMAEmailAddress = $('#txtMAEmailAddress');
            var txtMAUsername = $('#txtMAUsername');
            var txtMANewPassword = $('#txtMANewPassword');
            var selMASecQuestion1 = $('#selMASecQuestion1');
            var txtMASecQuestion1Answer = $('#txtMASecQuestion1Answer');
            var selMASecQuestion2 = $('#selMASecQuestion2');
            var txtMASecQuestion2Answer = $('#txtMASecQuestion2Answer');

            $(function () {
                AjaxSend({
                    url: MergeUrlParameters(GetUrlParameters(), '/Konstruksyon/MyProfileDetails'),
                    success: function (e) {
                        LoadData(e);
                    }
                })
            })

            $('body')
            .on('click', '#btnMAEditName', function () {
                isEditName = true;
                $('#txtMAFirstName').val(mydetails.FirstName);
                $('#txtMAMiddleName').val(mydetails.MiddleName);
                $('#txtMALastName').val(mydetails.Surname);
                $('#selMAPersonGenSuffix').val(CStr2(mydetails.PersonalSuffixName));
                $('#divEditName').pageshow();
            })
            .on('click', '#btnMACancelName', function () {
                isEditName = false;
                $('#txtMAFirstName').val(mydetails.FirstName);
                $('#txtMAMiddleName').val(mydetails.MiddleName);
                $('#txtMALastName').val(mydetails.Surname);
                $('#selMAPersonGenSuffix').val(CStr2(mydetails.PersonalSuffixName));
                $('#divActualName').pageshow();
            })
            .on('click', '#btnMAChangePassword', function () {
                isChangePs = true;
                $('#divEditPassword').pageshow();
                $('#txtMANewPassword').val('').focus();
            })
            .on('click', '#btnMACancelNewPassword', function () {
                isChangePs = false;
                $('#divActualPassword').pageshow();
                $('#txtMANewPassword').val('');
            })
            .on('click', '#btnMASaveChanges', function (e) {
                if (!Validate()) {
                    e.stopPropagation();
                    e.preventDefault();
                } else {
                    $('#txtMACurrentPassword').val('');
                }
            })
            .on('click', '#btnMAContinueSave', function () {
                SaveNow();
            })
            .on('keypress', '#txtMACurrentPassword', function (e) {
                if (e.which == 13) {
                    SaveNow();
                }
            })
            .on('change', '#selMASecQuestion1', function () {
                var val = $.trim($(this).val());
                $('#selMASecQuestion2').find('option').removeAttr('disabled');

                if (val != '') {
                    $('#selMASecQuestion2 option[value="' + $(this).val() + '"]').attr('disabled', true);
                }
            })
            .on('change', '#selMASecQuestion2', function () {
                var val = $.trim($(this).val());
                $('#selMASecQuestion1').find('option').removeAttr('disabled');

                if (val != '') {
                    $('#selMASecQuestion1 option[value="' + $(this).val() + '"]').attr('disabled', true);
                }
            })
            .on('click', '#btnMAEditSecQ1Ans', function () {
                isEditAns1 = true;
                $('#divEditSecQ1').pageshow();
                $('#txtMASecQuestion1Answer').val('').focus();
            })
            .on('click', '#btnMACancelEditSecQ1Ans', function () {
                isEditAns1 = false;
                $('#divStaticSecQ1').pageshow();
                $('#txtMASecQuestion1Answer').val('');
            })
            .on('click', '#btnMAEditSecQ2Ans', function () {
                isEditAns2 = true;
                $('#divEditSecQ2').pageshow();
                $('#txtMASecQuestion2Answer').val('').focus();
            })
            .on('click', '#btnMACancelEditSecQ2Ans', function () {
                isEditAns2 = false;
                $('#divStaticSecQ2').pageshow();
                $('#txtMASecQuestion2Answer').val('');
            })
            .on('click', '#btnMACancel', function () {
                location = location.origin + '/Konstruksyon/Dashboard'
            })

            function SaveNow() {
                var txtMACurrentPassword = $('#txtMACurrentPassword');
                $('#divMAErrMsg').hide();

                if ($.trim(txtMACurrentPassword.val()) == '') {
                    txtMACurrentPassword.parent().addClass('has-error');
                    return;
                } else {
                    txtMACurrentPassword.parent().removeClass('has-error');
                }

                $('#btnMAContinueSave').text('Saving ...').attr('disable', true);

                AjaxSend({
                    url: '/Konstruksyon/CheckPassword',
                    data: { password: $.trim(txtMACurrentPassword.val()) },
                    success: function (e) {
                        $('#btnMAContinueSave').text('Continue').removeAttr('disabled');

                        if (e.Results == 'valid') {
                            var data = {
                                FirstName: $.trim(txtMAFirstName.val()),
                                MiddleName: $.trim(txtMAMiddleName.val()),
                                LastName: $.trim(txtMALastName.val()),
                                GenSuffix: $.trim(selMAPersonGenSuffix.val()),
                                DateOfBirth: $.format.date(new Date(selMABirthMonth.val() + ' ' + $.trim(selMABirthDay.val()) + ', ' + $.trim(txtMABirthYear.val()))),
                                EmailAddress: $.trim(txtMAEmailAddress.val()),
                                Username: $.trim(txtMAUsername.val()),
                                NewPassword: $.trim(txtMANewPassword.val()),
                                SecQ1ID: mysecq[0].ID,
                                SecQ1: $.trim(selMASecQuestion1.val()),
                                SecQ1Ans: $.trim(txtMASecQuestion1Answer.val()),
                                SecQ2ID: mysecq[1].ID,
                                SecQ2: $.trim(selMASecQuestion2.val()),
                                SecQ2Ans: $.trim(txtMASecQuestion2Answer.val()),
                                IsChangePassword: isChangePs,
                                IsEditName: isEditName,
                                IsEditAns1: isEditAns1,
                                IsEditAns2: isEditAns2,
                                DateNow: $.format.date(new Date(), 'MMM dd, yyyy')
                            }

                            AjaxSend({
                                url: '/Konstruksyon/SaveMyAccount',
                                data: { data: data },
                                success: function (e) {
                                    if (e.Results == 'saved') {
                                        $('#btnMACancelSave').click();
                                        WebToast('Changes has been saved', 'top');
                                        AjaxSend({
                                            url: '/Konstruksyon/MyProfileDetails',
                                            success: function (e) {
                                                myaccountdetails = { d: e };
                                                LoadData(e);
                                            }
                                        })
                                    } else {
                                        NewMessageBox('There\'s an error while saving changes. Please contact your system administrator.', 'Message');
                                    }
                                }
                            })

                        } else if (e.Results == 'invalid') {
                            $('#divMAErrMsg').show();
                        } else {
                            NewMessageBox('Your session has expired', 'Message', [{
                                Text: 'Go to login page', Click: function () {
                                    location.href = '/Konstruksyon';
                                }
                            }])
                        }
                    }
                })
            }

            function Validate() {
                var hasError = false;

                $('#divErrMsg').text('Please complete the required field(s)').hide();

                if (isEditName) {
                    if ($.trim(txtMAFirstName.val()) == '') {
                        txtMAFirstName.parent().addClass('has-error');
                        hasError = true;
                    } else {
                        txtMAFirstName.parent().removeClass('has-error');
                    }

                    if ($.trim(txtMALastName.val()) == '') {
                        txtMALastName.parent().addClass('has-error');
                        hasError = true;
                    } else {
                        txtMALastName.parent().removeClass('has-error');
                    }
                }

                if ($.trim(selMABirthMonth.val()) == '' || $.trim(selMABirthDay.val()) == '' || $.trim(txtMABirthYear.val()) == '') {
                    selMABirthMonth.parent().addClass('has-error');
                    hasError = true;
                } else {
                    selMABirthMonth.parent().removeClass('has-error');
                }

                if ($.trim(txtMAEmailAddress.val()) == '') {
                    txtMAEmailAddress.parent().addClass('has-error');
                    hasError = true;
                } else {
                    if (!IsEmail($.trim(txtMAEmailAddress.val()))) {
                        txtMAEmailAddress.parent().addClass('has-error');
                        hasError = true;
                        $('#divErrMsg').text('Enter valid email address').show();
                        return false;
                    } else {
                        txtMAEmailAddress.parent().removeClass('has-error');
                    }
                }

                if ($.trim(txtMAUsername.val()) == '') {
                    txtMAUsername.parent().addClass('has-error');
                    hasError = true;
                } else {
                    if ($.trim(txtMAUsername.val()).length < 8) {
                        txtMAUsername.parent().addClass('has-error');
                        hasError = true;
                        $('#divErrMsg').text('Username must be 8 characters or more').show();
                        return false;
                    } else {
                        txtMAUsername.parent().removeClass('has-error');
                    }
                }

                if (isChangePs) {
                    if ($.trim(txtMANewPassword.val()) == '') {
                        txtMANewPassword.parent().addClass('has-error');
                        hasError = true;
                    } else {
                        if ($.trim(txtMANewPassword.val()).length < 8) {
                            txtMANewPassword.parent().addClass('has-error');
                            hasError = true;
                            $('#divErrMsg').text('Password must be 8 characters or more').show();
                            return false;
                        } else {
                            txtMANewPassword.parent().removeClass('has-error');
                        }
                    }
                }

                if ($.trim(selMASecQuestion1.val()) == '') {
                    selMASecQuestion1.parent().addClass('has-error');
                    hasError = true;
                } else {
                    selMASecQuestion1.parent().removeClass('has-error');
                }

                if (isEditAns1) {
                    if ($.trim(txtMASecQuestion1Answer.val()) == '') {
                        txtMASecQuestion1Answer.parent().addClass('has-error');
                        hasError = true;
                    } else {
                        txtMASecQuestion1Answer.parent().removeClass('has-error');
                    }
                }

                if ($.trim(selMASecQuestion2.val()) == '') {
                    selMASecQuestion2.parent().addClass('has-error');
                    hasError = true;
                } else {
                    selMASecQuestion2.parent().removeClass('has-error');
                }

                if (isEditAns2) {
                    if ($.trim(txtMASecQuestion2Answer.val()) == '') {
                        txtMASecQuestion2Answer.parent().addClass('has-error');
                        hasError = true;
                    } else {
                        txtMASecQuestion2Answer.parent().removeClass('has-error');
                    }
                }

                if (hasError) {
                    $('#divErrMsg').show();
                    return false;
                } else {
                    return true;
                }
            }

            function LoadData(data) {
                //var data = myaccountdetails.d; // myaccountdetails declared in EcommerceLayout.vbhtml
                mydetails = data.Details[0];
                myemail = data.Email[0];
                mycredentials = data.Credentials[0];
                mysecq = data.SecQ;

                isEditName = false;
                isChangePs = false;
                isEditAns1 = false;
                isEditAns2 = false;

                selMABirthDay.empty().append(new Option('Day', ''));

                for (var i = 1; i <= 31; i++) {
                    selMABirthDay.append(new Option(i, i))
                }

                $('#divActualName, #divActualPassword, #divStaticSecQ1, #divStaticSecQ2').pageshow();

                $('#txtMAFirstName, #txtMAMiddleName, #txtMALastName, #txtMANewPassword').val('');

                $('#txtMAFullName').text($.trim(mydetails.FirstName + ' ' + CStr2(mydetails.MiddleName) + ' ' + mydetails.Surname + ' ' + CStr2(mydetails.PersonalSuffixName)));
                $('#selMABirthMonth').val($.format.date(new Date(mydetails.DateOfBirth), 'MMMM'));
                $('#selMABirthDay').val($.format.date(new Date(mydetails.DateOfBirth), 'dd'));
                $('#txtMABirthYear').val($.format.date(new Date(mydetails.DateOfBirth), 'yyyy'));
                if (myemail != undefined) {
                    $('#txtMAEmailAddress').val(myemail.EmailAddress);
                }
                $('#txtMAUsername').val(mycredentials.Username);

                $('#selMASecQuestion1').val(CStr2(mysecq[0].SecurityQuestion));
                $('#txtMASecQuestion1Answer').val('');
                $('#selMASecQuestion2').val(CStr2(mysecq[1].SecurityQuestion));
                $('#txtMASecQuestion2Answer').val('');
            }
        })
    })    
})