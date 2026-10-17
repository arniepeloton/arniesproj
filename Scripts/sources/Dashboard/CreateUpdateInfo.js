require([
    'common'
    ], function () {
        $(function () {

//            ShowPeopleMenu();

            //Onchange functions
            //Create new update page controls


            $('#txtTitle').change(function () {
                $('#spanTitle').hide();
                $(this).removeClass('form-validation-error');

                if ($.trim($(this).val()) == '') {
                    $('#spanTitle').show();
                    $(this).addClass('form-validation-error');

                }

            });

            $('#txtMessage').change(function () {
                $('#spanMessage').hide();
                $(this).removeClass('form-validation-error');

                if ($.trim($(this).val()) == '') {
                    $('#spanMessage').show();
                    $(this).addClass('form-validation-error');

                }
            });

            $('#selProduct').change(function () {
                $('#spanProduct').hide();
                $(this).removeClass('form-validation-error');

                if ($.trim($(this).val()) == '') {
                    $('#spanProduct').show();
                    $(this).addClass('form-validation-error');
                }
            });


            $('#btnSave').click(function (e) {

                e.preventDefault();
                validateFields();
                $('#spanAlert').hide();

                if ($.trim($('#selProduct').val()) == '') {
                    $('#spanAlert').show();
                    return;
                }

                if ($.trim($('#txtTitle').val()) == '') {
                    $('#spanAlert').show();
                    return;
                }


                if ($.trim($('#txtMessage').val()) == '') {
                    $('#spanAlert').show();
                    return;
                }

                SaveUpdateInfo();

            });


        }); //Inner function


        function ShowPeopleMenu() {
            // EDITED by JANINE
            AjaxSend({
                url: 'Dashboard/IsNewEntry',
                success: function (vData) {

                    if (vData.PeopleID == '') { //if people ID is blank, then redirect to sign in
                        pUrl = '/dashboard/signin';
                        window.open(GetPathFormat(pUrl), '_self');
                    } else {
                        var pData = {};
                        pData.ID = vData.PeopleID;

                        //Check if IsFirstPartySignUp is 1 or 0

                        AjaxSend({
                            url: "SignIn/CheckPeopleIsFirstPartySignUp",
                            data: pData,
                            success: function (vDataFirstParty) {
                                //if 0, redirect Account Management to reset credentials before he/she can access PeopleMenu
                                if (vDataFirstParty.PeopleFirstPartySignUpData.Rows[0].IsFirstPartySignUp == '0') {
                                    pUrl = '/AccountManagement';
                                    window.open(GetPathFormat(pUrl), '_self');
                                } else {
                                    sessionStorage.removeItem("OrgID");
                                    sessionStorage.removeItem("FowstID");

//                                    ShowPersonNameInfo(vData.PeopleID);

                                }
                            }
                        });
                    }

                }
            });


        }
        function validateFields() {
            if ($.trim($('#selProduct').val()) == '') {
                $('#spanProduct').show();
                $('#selProduct').addClass('form-validation-error');

            }

            if ($.trim($('#txtTitle').val()) == '') {
                $('#spanTitle').show();
                $('#txtTitle').addClass('form-validation-error');

            }

            if ($.trim($('#txtMessage').val()) == '') {
                $('#spanMessage').show();
                $('#txtMessage').addClass('form-validation-error');

            }
        }

        function ClearFields() {
            $("#selProduct").val('General Update');
            $("#selCategory").val('');
            $("#txtTitle").val('');
            $("#txtMessage").val('');
        }

        function SaveUpdateInfo() {
            var pData = {};

            pData = {
                Product: $.trim($("#selProduct").val()),
                Category: $.trim($("#selCategory").val()),
                Title: $.trim($("#txtTitle").val()),
                Message: $.trim($("#txtMessage").val())

            };

            AjaxSend({
                url: 'Dashboard/SaveUpdateInfo',
                data: pData,
                success: function (vData) {
                    if (vData.Error == undefined && vData.UpdateData != "Error") {
                        alert('Update details successfully saved.');
                        ClearFields();
                    }
                    else
                        alert('Something went wrong. Please contact Mobiz Support.');
                } // success
            }); // AjaxSend


        }


    });

