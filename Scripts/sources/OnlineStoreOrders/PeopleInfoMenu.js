require([
    'common'
    ], function() {

    $(function () {

        var IsForgotPassword;

        $(document).ready(function () {

            setMonthCollection("selDOBMonth");
            setYearCollection("selDOBYear");

            ClearLoggerFields();
        });

        $('#frmPeopleLogger').submit(function (e) {
            e.preventDefault();

            $('#btnSignIn').click();
        });

        $('#btnSignIn').click(function (e) {

            e.preventDefault();

            if ($.trim($("#txtUsername").val()) == "" || $.trim($("#txtUsername").val()).length <= 1) {
                MessageBox("Please enter username.",
                            "Username",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtPassword").val()) == "" || $.trim($("#txtPassword").val()).length <= 1) {
                MessageBox("Please enter password.",
                            "Password",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            CheckUser();

        });

        $('#btnForgotPassword').click(function (e) {

            e.preventDefault();

            $.mobile.changePage('#pagePeopleName');
            IsForgotPassword = true;
            ClearPeopleNameFields();

        });

        $('#btnSignUp').click(function (e) {

            e.preventDefault();

            IsForgotPassword = false;
            $.mobile.changePage('#pagePeopleName');
            ClearPeopleNameFields();

        });

        $('#btnPeopleNameOk').click(function (e) {

            e.preventDefault();

            if ($.trim($("#txtSurname").val()) == "" || $.trim($("#txtSurname").val()).length <= 1) {
                MessageBox("Please enter surname.",
                            "Surname",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtFirstName").val()) == "" || $.trim($("#txtFirstName").val()).length <= 1) {
                MessageBox("Please enter first name.",
                            "First Name",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtMiddleName").val()) == "" || $.trim($("#txtMiddleName").val()).length <= 1) {
                MessageBox("Please enter middle name.",
                            "Middle Name",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#selDOBMonth").val()) == "" || $.trim($("#selDOBDay").val()) == "" || $.trim($("#selDOBYear").val()) == "") {
                MessageBox("Please enter valid date of birth.",
                            "Date Of Birth",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtMotherMaidenSurname").val()) == "" || $.trim($("#txtMotherMaidenSurname").val()).length <= 1) {
                MessageBox("Please enter mother maiden surname.",
                            "Mother Maiden Surname",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }
            if ($.trim($("#txtMotherMaidenFirstName").val()) == "" || $.trim($("#txtMotherMaidenFirstName").val()).length <= 1) {
                MessageBox("Please enter mother first name.",
                            "First Name",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            ValidateUniquePersonNameInfo();

        });

        $('#btnPeopleNameCancel').click(function (e) {

            e.preventDefault();

            $.mobile.changePage('#pagePeopleMenu');
            ClearLoggerFields();

        });

        $('#btnUserAccountOk').click(function (e) {

            e.preventDefault();

            if ($.trim($("#txtSetUsername").val()) == "" || $.trim($("#txtSetUsername").val()).length < 8) {
                MessageBox("Please enter username minimum length of 8 characters.",
                            "Username",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtSetPassword").val()) == "" || $.trim($("#txtSetPassword").val()).length < 8) {
                MessageBox("Please enter password minimum length of 8 characters.",
                            "Password",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtSetConfirmPassword").val()) == "" || $.trim($("#txtSetConfirmPassword").val()).length < 8) {
                MessageBox("Please enter confirmation password minimum length of 8 characters.",
                            "Confirm Password",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtSetPassword").val()) != $.trim($("#txtSetConfirmPassword").val())) {
                MessageBox("Password and confirmation password is not equal.",
                            "Confirm Password",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            $.mobile.changePage('#pageSetSecurityQuestion');
            ClearSetSecurityQuestionFields();

        });

        $('#btnUserAccountCancel').click(function (e) {

            e.preventDefault();

            $.mobile.changePage('#pagePeopleName');

        });

        $('#btnSetSecurityOk').click(function (e) {

            e.preventDefault();

            if ($.trim($("#selQuestion1").val()) == "") {
                MessageBox("Please select security question no. 1.",
                            "Security Question",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }
            if ($.trim($("#txtQ1Answer").val()) == "") {
                MessageBox("Please enter answer for the security question no. 1.",
                            "Answer",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($("#chkQ2").is(":checked")) {
                if ($.trim($("#selQuestion2").val()) == "") {
                    MessageBox("Please select security question no. 2.",
                            "Security Question",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    return;
                }
                if ($.trim($("#txtQ2Answer").val()) == "") {
                    MessageBox("Please enter answer for the security question no. 2.",
                            "Answer",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    return;
                }
            }

            SavePersonDetails();

        });

        $('#btnSetSecurityCancel').click(function (e) {

            e.preventDefault();

            $.mobile.changePage('#pageSetUserAccount');

        });

        $('#btnSecurityOk').click(function (e) {

            e.preventDefault();

            if ($.trim($("#lblQuestion1").text()) != "") {
                if ($.trim($("#txtQuestion1Answer").val()) == "") {
                    MessageBox("Please enter answer for the security question no. 1.",
                            "Answer",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    return;
                }
            }

            if ($.trim($("#lblQuestion2").text()) != "") {
                if ($.trim($("#txtQuestion2Answer").val()) == "") {
                    MessageBox("Please enter answer for the security question no. 2.",
                            "Answer",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    return;
                }
            }

            CheckSecurityQuestion();

        });

        $('#btnSecurityCancel').click(function (e) {

            e.preventDefault();

            $.mobile.changePage('#pagePeopleMenu');
            ClearLoggerFields();

        });

        $('#selDOBMonth').change(function (e) {
            e.preventDefault();

            setDayCollection('selDOBMonth', 'selDOBDay', 'selDOBYear', true);

        });

        $('#selDOBYear').change(function (e) {
            e.preventDefault();

            setDayCollection('selDOBMonth', 'selDOBDay', 'selDOBYear', true);

        });

        $('#selDOBYear').click(function (e) {
            e.preventDefault();

            if ($.trim($('#selDOBYear').val()) == "") {
                $('#selDOBYear').val('1980');
                $('#selDOBYear').selectmenu('refresh');
            }

        });

        $('#chkQ2').click(function () {

            setQuestion2($(this).is(":checked"));

        });

        function setQuestion2(IsEnable) {

            $('#selQuestion2').attr("disabled", !IsEnable);
            $('#txtQ2Answer').attr("disabled", !IsEnable);

            if (!IsEnable) {
                $('#selQuestion2').val('').selectmenu('refresh');
                $('#txtQ2Answer').val('');
            }

        };

        function CheckUser() {

            var pData = {};
            pData.Username = $("#txtUsername").val();
            pData.UserPassword = $("#txtPassword").val();

            AjaxSend({
                url: 'People/CheckPeopleUserAccount',
                data: pData,
                success: function (vData) {

                    if ($.trim(vData.PeopleUserAccountData) != "") {
                        CreateNewEntry(0, vData.PeopleUserAccountData);
                    }
                    else {
                        MessageBox("Invalid username and password.",
                           "Username and Password",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                        return;
                    }
                }
            });

        }

        function CreateNewEntry(IsNew, strID) {
            var pData = {};
            pData.IsNew = IsNew;
            pData.ID = strID;

            AjaxSend({
                url: 'People/setCreateNew',
                data: pData,
                success: function (vData) {

                    if ((vData.IsNew || '') == "TRUE") {
                        window.open("People/PeopleInfoName", "_self");
                    }
                    else {
                        window.open("People/PeopleInfoSummary", "_self");
                    }
                }
            });

        }

        function ClearLoggerFields() {

            $("#txtUsername").val('');
            $("#txtPassword").val('');

        }

        function ClearPeopleNameFields() {

            $("#txtSurname").val('');
            $("#txtFirstName").val('');
            $("#txtMiddleName").val('');

            $("#selDOBMonth").val('');
            $("#selDOBMonth").selectmenu('refresh');
            $("#selDOBDay").val('');
            $("#selDOBDay").selectmenu('refresh');
            $("#selDOBYear").val('');
            $("#selDOBYear").selectmenu('refresh');

            $("#txtMotherMaidenSurname").val('');
            $("#txtMotherMaidenFirstName").val('');
            $("#txtMotherMaidenMiddleName").val('');

        }

        function ClearSecurityQuestionFields() {

            $("#txtPeopleID").val('');

            $("#txtQ1ID").val('');
            $("#lblQuestion1").text('');
            $("#txtQuestion1").val('');

            $("#txtQ2ID").val('');
            $("#lblQuestion2").text('');
            $("#txtQuestion2").val('');

        }

        function ClearSetUserAccountFields() {

            $("#txtSetUsername").val('');
            $("#txtSetPassword").val('');
            $("#txtSetConfirmPassword").val('');

        }

        function ClearSetSecurityQuestionFields() {

            $("#selQuestion1").val('').selectmenu("refresh");
            $("#txtQ1Answer").val('');

            $('#chkQ2').attr("checked", false).checkboxradio("refresh");
            $("#selQuestion2").val('').selectmenu("refresh");
            $("#txtQ2Answer").val('');
            setQuestion2($("#chkQ2").is(":checked"));

        }

        function ValidateUniquePersonNameInfo() {

            var pData = {};
            pData.PeopleID = "";
            pData.Surname = $.trim($("#txtSurname").val());
            pData.FirstName = $.trim($("#txtFirstName").val());
            pData.MiddleName = $.trim($("#txtMiddleName").val());
            pData.DateOfBirth = $("#selDOBMonth").val() + '/' + $("#selDOBDay").val() + '/' + $("#selDOBYear").val();
            pData.MotherMaidenSurname = $.trim($("#txtMotherMaidenSurname").val());
            pData.MotherMaidenFirstName = $.trim($("#txtMotherMaidenFirstName").val());
            pData.MotherMaidenMiddleName = $.trim($("#txtMotherMaidenMiddleName").val());
            pData.FullCheckName = "1";

            AjaxSend({
                url: "People/CheckPeopleUniqueName",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleNameData.Rows.length > 0) {

                        if (IsForgotPassword) {
                            ShowSecurityQuestion();
                        }
                        else {
                            MessageBox("Personal information already exist. Do you want to retrieve your password?",
                               "Personal Information",
                                [
                                    { Text: 'Yes',
                                        Click: function () {
                                            ShowSecurityQuestion();
                                        }
                                    }
                                ,
                                    { Text: 'No'
                                    }
                                ]
                            );
                        }

                    }
                    else {

                        if (IsForgotPassword) {
                            MessageBox("Personal information does not exist.",
                               "Personal Information",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );
                        }
                        else {
                            $.mobile.changePage('#pageSetUserAccount');
                            ClearSetUserAccountFields();
                        }

                    }

                }
            });

        };

        function ShowSecurityQuestion() {
            var pData = {};
            pData.Surname = $.trim($("#txtSurname").val());
            pData.FirstName = $.trim($("#txtFirstName").val());
            pData.MiddleName = $.trim($("#txtMiddleName").val());
            pData.DateOfBirth = $("#selDOBMonth").val() + '/' + $("#selDOBDay").val() + '/' + $("#selDOBYear").val();
            pData.MotherMaidenSurname = $.trim($("#txtMotherMaidenSurname").val());
            pData.MotherMaidenFirstName = $.trim($("#txtMotherMaidenFirstName").val());
            pData.MotherMaidenMiddleName = $.trim($("#txtMotherMaidenMiddleName").val());

            AjaxSend({
                url: "People/getPeopleSecurityQuestionList",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleSecurityData.Rows.length > 0) {

                        $.mobile.changePage('#pageSecurityQuestion');
                        ClearSecurityQuestionFields();

                        for (var p = 0; p <= vData.PeopleSecurityData.Rows.length - 1; p++) {

                            $("#txtPeopleID").val(vData.PeopleSecurityData.Rows[p].PeopleID);
                            if (p == 0) {
                                $("#txtQ1ID").val(vData.PeopleSecurityData.Rows[p].ID);
                                $("#lblQuestion1").text(vData.PeopleSecurityData.Rows[p].SecurityQuestion);
                                $("#lstQ1Divider").show();
                                $("#lstQ1").show();
                            }
                            else if (p == 1) {
                                $("#txtQ2ID").val(vData.PeopleSecurityData.Rows[p].ID);
                                $("#lblQuestion2").text(vData.PeopleSecurityData.Rows[p].SecurityQuestion);
                                $("#lstQ2Divider").show();
                                $("#lstQ2").show();
                            }
                        };


                    }
                    else {
                        MessageBox("No security question has been set.",
                            "Personal Information",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        $.mobile.changePage('#pagePeopleName');
                                    }
                                }
                            ]
                        );
                    }
                }
            });

        }

        function SavePersonDetails() {
            var pData = [];

            pData = {
                PeopleID: "",
                Surname: $.trim($("#txtSurname").val()),
                FirstName: $.trim($("#txtFirstName").val()),
                MiddleName: $.trim($("#txtMiddleName").val()),
                DateOfBirth: $("#selDOBMonth").val() + '/' + $("#selDOBDay").val() + '/' + $("#selDOBYear").val(),
                MotherMaidenSurname: $.trim($("#txtMotherMaidenSurname").val()),
                MotherMaidenFirstName: $.trim($("#txtMotherMaidenFirstName").val()),
                MotherMaidenMiddleName: $.trim($("#txtMotherMaidenMiddleName").val()),
                FullCheckName: "1",

                Username: $.trim($("#txtSetUsername").val()),
                UserPassword: $.trim($("#txtSetPassword").val()),
                UserConfirmPassword: $.trim($("#txtSetConfirmPassword").val()),

                Question1: $.trim($("#selQuestion1").val()),
                Question1Answer: $.trim($("#txtQ1Answer").val()),
                IsHasQuestion2: ($("#chkQ2").is(":checked") ? "1" : "0"),
                Question2: $.trim($("#selQuestion2").val()),
                Question1Answer2: $.trim($("#txtQ2Answer").val())

            };

            AjaxSend({
                url: "People/CheckPeopleUniqueName",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleNameData.Rows.length > 0) {
                        MessageBox("Person with the same name, date of birth and mother maiden name already exist.",
                            "Person Unique Name",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        $('.ui-dialog').dialog('close');
                                        $.mobile.changePage('#pagePeopleName');
                                    }
                                }
                            ]
                        );

                    }
                    else {
                        AjaxSend({
                            url: "People/SavePeopleInfoNameInitial",
                            data: pData,
                            success: function (vSaveData) {

                                if (vSaveData.PeopleNameData == "DateOfBirthError") {
                                    MessageBox("Please enter valid date of birth.",
                                    "Date Of Birth",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                    );
                                }
                                else {

                                    window.open("People/PeopleInfoSummary", "_self");

                                }

                            }
                        });

                    }

                }
            });

        };

        function CheckSecurityQuestion() {
            var pData = [];

            pData = {
                PeopleID: $.trim($("#txtPeopleID").val()),
                Question1ID: $.trim($("#txtQ1ID").val()),
                Question1Answer: $.trim($("#txtQuestion1Answer").val()),
                Question2ID: $.trim($("#txtQ2ID").val()),
                Question2Answer: $.trim($("#txtQuestion2Answer").val()),
            };

            AjaxSend({
                url: "People/CheckPeopleSecurityQuestions",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleSecurityData == "INVALID") {
                        MessageBox("Sorry. Incorrect security question(s) answer.",
                            "Security Question",
                            [
                                { Text: 'Ok' }
                            ]
                        );
                    }
                    else {

                        window.open("People/PeopleInfoSummary", "_self");

                    }

                }
            });

        };
    });
});