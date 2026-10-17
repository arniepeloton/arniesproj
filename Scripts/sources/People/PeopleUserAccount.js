require([
    'common'
    ], function() {

    $(function () {
        var q1, q2;

        $(document).ready(function () {

            ShowUserAccountDetails();

        });

        $('.btnProfileBackToHome').click(function () {
            if (GetUrlParameters().p == 'eas') {
                location.href = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: 0
                }, '/erp')
            } else if (GetUrlParameters().p == 'crm') {
                location.href = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: 0
                }, '/CRMDashboard')
            } else {
                location.href = '/People/PeopleMenu';
            }
        })

        $('#btnPeopleUserAccountPopUpMenu').click(function (e) {
            e.preventDefault();

            $("#divPeopleUserAccountPopUpMenu").popup("open")

        });

        $('#popPeopleUserAccountPeopleInfoName').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoName')), "_self");

        });

        $('#popPeopleUserAccountPeopleInfoAlias').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAlias')), "_self");

        });

        $('#popPeopleUserAccountPeopleInfoAddress').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAddress')), "_self");

        });

        $('#popPeopleUserAccountPeoplePersonalInfo').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePersonalInfo')), "_self");

        });

        $('#popPeopleUserAccountPeopleContact').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleContactNo')), "_self");

        });

        $('#popPeopleUserAccountPeopleEducation').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEducation')), "_self");

        });

        $('#popPeopleUserAccountPeopleEmpHistory').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEmpHistory')), "_self");

        });

        $('#popPeopleUserAccountPeopleRelatedPerson').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleRelatedPerson')), "_self");

        });

        $('#popPeopleUserAccountPeoplePrimeIDReference').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePrimeIDReference')), "_self");

        });

        $('#btnUserAccountDone').click(function (e) {
            e.preventDefault();

            if ($.trim($("#txtSetUsername").val()) == "" || $.trim($("#txtSetUsername").val()).length < 8) {
                MessageBox("Please enter username minimum length of 8 characters.",
                            "Username",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            } else {

                //ADDITIONAL by JANINE Sept 2, 2013
                if (!validateAlphanumeric($.trim($("#txtSetUsername").val()))) {
                    MessageBox("Username must contain only alphanumeric and underscore characters.",
                                    "Username",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                    return;
                }
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

            if ($.trim($("#selQuestion1").val()) != q1) {
                if ($.trim($("#txtQ1Answer").val()) == "") {
                    MessageBox("You've made changes to your security question no. 1. Please enter your new answer.",
                                "Answer",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }
            }

            if ($.trim($("#selQuestion2").val()) != q2) {
                if ($.trim($("#txtQ2Answer").val()) == "") {
                    MessageBox("You've made changes to your security question no. 2. Please enter your new answer.",
                            "Answer",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }
            }

            $.mobile.changePage('#pagePeopleUserAccountConfirm', { transition: 'pop', role: 'dialog' });
            $("#txtUsername").focus();
            return;

            if ($.trim($("#selQuestion1").val()) == "") {
                MessageBox("Please select security question no. 1.",
                            "Security Question",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }
            

            if ($.trim($("#selQuestion1").val()) == $.trim($("#selQuestion2").val())) {
                MessageBox("Security question has been used already. Please select another security question.",
                        "Security Question No. 2",
                        [
                            { Text: 'Ok' }
                        ]
                );
                return;
            }
        });

        $('#formUserAccountConfirm').submit(function (e) {
            e.preventDefault();
            
            if ($.trim($("#txtUsername").val()) == "") {
                MessageBox("Please enter old username.",
                                "Old Username",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                return;
            }

            if ($.trim($("#txtPassword").val()) == "") {
                MessageBox("Please enter old password.",
                                "Old Password",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                return;
            }

            ValidatePersonAccount();
        })

        $('#btnPeopleConflictNo').click(function (e) {
            e.preventDefault();

            $('.ui-dialog').dialog('close');

        });

        $('#btnUserAccountCancel').click(function (e) {
            e.preventDefault();

            $('.btnProfileBackToHome').click();

        });

        function validateAlphanumeric(string) {
            var re = /^[a-zA-Z0-9\_]+$/;

            return re.test(string);
        }

        function ShowUserAccountDetails() {

            ClearUserAccountFields();
            ShowSecurityQuestion();
        };

        function ShowSecurityQuestion() {

            AjaxSend({
                url: "People/getPeopleSecurityQuestionByPeopleIDList",
                success: function (vData) {
                    $("#txtPeopleID").val(vData.PeopleID);
                    if (vData.PeopleSecurityData.Rows.length > 0) {

                        for (var p = 0; p <= vData.PeopleSecurityData.Rows.length - 1; p++) {
                            
                            if (p == 0) {
                                q1 = vData.PeopleSecurityData.Rows[p].SecurityQuestion;
                                $("#txtQ1ID").val(vData.PeopleSecurityData.Rows[p].ID || '');
                                $("#selQuestion1").val(q1 || '').selectmenu('refresh');
                                //$("#txtQ1Answer").val(vData.PeopleSecurityData.Rows[p].SecurityAnswer || ''); EDITED by Janine. Hide Answer because it's encrypted.
                            }
                            else if (p == 1) {
                                q2 = vData.PeopleSecurityData.Rows[p].SecurityQuestion;
                                $("#txtQ2ID").val(vData.PeopleSecurityData.Rows[p].ID || '');
                                $("#selQuestion2").val(q2 || '').selectmenu('refresh');
                                //$("#txtQ2Answer").val(vData.PeopleSecurityData.Rows[p].SecurityAnswer || '');
                            }
                        };
                    }

                }
            });

        }

        function ClearUserAccountFields() {

            $("#txtUsername").val('');
            $("#txtPassword").val('');
            $("#txtSetUsername").val('');
            $("#txtSetPassword").val('');
            $("#txtSetConfirmPassword").val('');

            $("#txtQ1ID").val('');
            $("#selQuestion1").val('').selectmenu("refresh");
            $("#txtQ1Answer").val('');

            $("#txtQ2ID").val('');
            $("#selQuestion2").val('').selectmenu("refresh");
            $("#txtQ2Answer").val('');
        }

        function ValidatePersonAccount() {

            var pData = {};
            pData.Username = $("#txtUsername").val();
            pData.UserPassword = $("#txtPassword").val();

            AjaxSend({
                url: 'People/CheckPeopleUserAccount',
                data: pData,
                success: function (vData) {

                    if ($.trim(vData.PeopleUserAccountData) != $("#txtPeopleID").val()) {
                        MessageBox("Invalid username and password.",
                           "Username and Password",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                        return;
                    }
                    else {
                        $('.ui-dialog').dialog('close');
                        SavePersonUserAccount();
                    }
                }
            });

        }

        function SavePersonUserAccount() {

            var pData = {};
            pData.PeopleID = $.trim($("#txtPeopleID").val());
            pData.Username = $.trim($("#txtSetUsername").val());
            pData.UserPassword = $.trim($("#txtSetPassword").val());
            pData.UserConfirmPassword = $.trim($("#txtSetConfirmPassword").val());

            pData.Question1ID = $.trim($("#txtQ1ID").val());
            pData.Question1 = $.trim($("#selQuestion1").val());
            pData.Question1Answer = $.trim($("#txtQ1Answer").val());
            pData.Question2ID = $.trim($("#txtQ2ID").val());
            pData.Question2 = $.trim($("#selQuestion2").val());
            pData.Question1Answer2 = $.trim($("#txtQ2Answer").val());

            AjaxSend({
                url: "People/SavePeopleUserAccount",
                data: pData,
                success: function (vData) {

                    window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoSummary')), "_self");

                }
            });

        };
    });
});