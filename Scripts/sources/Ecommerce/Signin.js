require(['common'], function () {
    $(document).ready(function () {
        var accounttype, arrBusInd = [], busindid = '', arrSecQ = [], isCustomer = 1;

        $(function () {
            if (GetUrlParameters().stat == 'confirmedemail') {
                $('#divConfirmedEmail').show();
            }

            AjaxSend({
                url: MergeUrlParameters(GetUrlParameters(), '/Ecommerce/GetAdditionalDateForSignin'),
                success: function (e) {
                    LoadData(e);
                }
            })
        })

        $('body')
        .on('click', '#btnBackToLogin', function () {
            $('#divSignin').pageshow();
        })
        .on('click', '#btnPersonalAcc', function () {
            accounttype = 'person';
            isCustomer = 1;
            $('#lblSignupAccountType').text('Sign up for a Personal Account');
            $('#titlePersonalAcc').text('Personal Account');
            $('#divCreation, #divPersonalAcc').pageshow();
            $('#divBreadCrumbsForCompany').hide();
        })
        .on('click', '#btnCompanyAcc', function () {
            accounttype = 'company';
            isCustomer = 1;
            $('#lblSignupAccountType').text('Sign up for a Company Account');
            $('#titlePersonalAcc').text('Administrator Account');
            $('#divCreation, #divCompanyAcc').pageshow();
            $('#divBreadCrumbsForCompany').show();
        })
        .on('click', '#btnCreateAccountSupplier', function () {
            accounttype = 'company';
            isCustomer = 0;
            $('#lblSignupAccountType').text('Signup as Supplier');
            $('#titlePersonalAcc').text('Administrator Account');
            $('#divCreation, #divCompanyAcc').pageshow();
            $('#divBreadCrumbsForCompany').show();
        })
        .on('click', '#btnCreateAccount', function (e) {
            e.stopPropagation();
            e.preventDefault();
            $(this).text('Creating account ...').attr('disabled', true);
            CreateAccount();            
        })
        .on('click', '#btnBackToCompany', function (e) {
            e.stopPropagation();
            e.preventDefault();
            if(accounttype == 'person'){
                $('#divSignin').pageshow();
            } else {
                $('#divCompanyAcc').pageshow();
            }            
        })
        .on('click', '#btnBackToSignin, .btnGoToLogin, #btnFPBack', function (e) {
            e.stopPropagation();
            e.preventDefault();
            $('#divSignin').pageshow();
        })
        .on('click', '#btnNextStep', function (e) {
            e.stopPropagation();
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

            $('#divCompanyErrMsg').text('Please fill in the required field(s) above.').hide();

            if ($.trim(txtCompanyName.val()) == '') {
                txtCompanyName.addClass('is-invalid');
                hasError = true;
            } else {
                txtCompanyName.removeClass('is-invalid');
            }

            if ($.trim(busindid) == '') {
                txtBusInd.addClass('is-invalid');
                hasError = true;
            } else {
                txtBusInd.removeClass('is-invalid');
            }

            if ($.trim(txtCompanyEmailAdd.val()) == '') {
                txtCompanyEmailAdd.addClass('is-invalid');
                hasError = true;
            } else {
                if (!IsEmail($.trim(txtCompanyEmailAdd.val()))) {
                    $('#divCompanyErrMsg').text('Invalid email address');
                    txtCompanyEmailAdd.addClass('is-invalid');
                    hasError = true;
                } else {
                    txtCompanyEmailAdd.removeClass('is-invalid');
                }                
            }

            if ($.trim(txtCompanyMobileNo.val()) == '') {
                txtCompanyMobileNo.addClass('is-invalid');
                hasError = true;
            } else {
                if ($.trim(txtCompanyMobileNo.val()).length < 10) {
                    $('#divCompanyErrMsg').text('Invalid mobile number');
                    txtCompanyMobileNo.addClass('is-invalid');
                    hasError = true;
                } else {
                    txtCompanyMobileNo.removeClass('is-invalid');
                }                
            }

            if ($.trim(txtCompanyAddressBldgNo.val()) == '') {
                txtCompanyAddressBldgNo.addClass('is-invalid');
                hasError = true;
            } else {
                txtCompanyAddressBldgNo.removeClass('is-invalid');
            }

            if ($.trim(txtCompanyAddressStreetName.val()) == '') {
                txtCompanyAddressStreetName.addClass('is-invalid');
                hasError = true;
            } else {
                txtCompanyAddressStreetName.removeClass('is-invalid');
            }

            if ($.trim(selCompanyAddressCity.val()) == '') {
                selCompanyAddressCity.addClass('is-invalid');
                hasError = true;
            } else {
                selCompanyAddressCity.removeClass('is-invalid');
            }

            if ($.trim(selCompanyAddressCountry.val()) == '') {
                selCompanyAddressCountry.addClass('is-invalid');
                hasError = true;
            } else {
                selCompanyAddressCountry.removeClass('is-invalid');
            }

            if (hasError) {
                $('#divCompanyErrMsg').show();
                return;
            }

            $('#divPersonalAcc').pageshow();
            $(window).scrollTop(0);
        })
        .on('keydown', '#selPersonBirthDay, #txtPersonBirthYear, #txtCompanyMobileNo', function (e) {
            if(!NumberOnly(e)){
                return
            }
        })
        .on('submit', '#formLogin', function (e) {
            e.stopPropagation();
            e.preventDefault();
            var txtuname = $('#txtuname');
            var txtpword = $('#txtpword');
            var hasError = false;

            if ($.trim(txtuname.val()) == '') {
                txtuname.addClass('is-invalid');
                hasError = true;
            } else {
                txtuname.removeClass('is-invalid');
            }

            if ($.trim(txtpword.val()) == '') {
                txtpword.addClass('is-invalid');
                hasError = true;
            } else {
                txtpword.removeClass('is-invalid');
            }

            if(hasError){
                return;
            }

            AjaxSend({
                url: '/Ecommerce/Login',
                data: { uname: $.trim(txtuname.val()), pword: $.trim(txtpword.val()) },
                success: function (e) {
                    var res = e.Results.split('|');
                    if (res[0] == 'pending request') {
                        $('#divErrLoginMsg').text('Your request to have an access to E-Commerce is still pending, please wait for the approval of your request.').show();
                    } else if (res[0] == 'disapproved') {
                        $('#divErrLoginMsg').text('Sorry! You\'ve been denied to access the E-Commerce!').show();
                    } else if (res[0] == 'invalid') {
                        $('#divErrLoginMsg').show();
                    } else {
                        sessionStorage.PeopleUserID = res[1];
                        $('#divErrLoginMsg').hide();
                        window.location.href = '/Ecommerce?OrgID=' + sessionStorage.OrgID;
                    }
                }
            })
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
        .on('blur', '#txtBusInd', function (e) {
            if ($.trim(busindid) == '') {
                $('#txtBusInd').val('');
            }
        })
        .on('change', '#selSecQuestion1', function () {
            var val = $.trim($(this).val());
            $('#selSecQuestion2').find('option').removeAttr('disabled');

            if(val != ''){
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
        .on('click', '#btnForgotPassword', function () {
            $('#divForgotPassword, #divFPUser').pageshow();
            $('#btnFPNext').text('NEXT').removeAttr('disabled');
            $('#divFPErrMsg').hide();
            ClearFPFields();
        })
        .on('click', '#btnFPNext', function (e) {
            e.stopPropagation();
            e.preventDefault();            
            CheckUser();
        })
        .on('click', '#btnFPSecQBack', function (e) {
            e.stopPropagation();
            e.preventDefault();
            $('#divFPUser').pageshow();
            $('#btnFPSecQNext').text('NEXT').removeAttr('disabled');
            $('#divFPSecQErrMsg').hide();
        })
        .on('click', '#btnFPSecQNext', function (e) {
            e.stopPropagation();
            e.preventDefault();
            var txtFPSecQ1Ans = $('#txtFPSecQ1Ans');
            var txtFPSecQ2Ans = $('#txtFPSecQ2Ans');
            var hasError = false;

            $('#divFPSecQErrMsg').text('Please fill in the required field(s) above.').hide();
            $(this).text('Checking ...').attr('disabled', true);

            if ($.trim(txtFPSecQ1Ans.val()) == '') {
                txtFPSecQ1Ans.addClass('is-invalid');
                hasError = true;
            } else {
                txtFPSecQ1Ans.removeClass('is-invalid');
            }

            if ($.trim(txtFPSecQ2Ans.val()) == '') {
                txtFPSecQ2Ans.addClass('is-invalid');
                hasError = true;
            } else {
                txtFPSecQ2Ans.removeClass('is-invalid');
            }

            if (hasError) {
                $(this).text('NEXT').removeAttr('disabled');
                $('#divFPSecQErrMsg').show();
                return;
            }

            arrSecQ[0].Answer = $.trim(txtFPSecQ1Ans.val());
            arrSecQ[1].Answer = $.trim(txtFPSecQ2Ans.val());

            AjaxSend({
                url: '/Ecommerce/CheckSecQAnswer',
                data: { data: arrSecQ },
                success: function (e) {
                    $('#btnFPSecQNext').text('NEXT').removeAttr('disabled');

                    if(e.Results == 'valid'){
                        $('#divFPResetPassword').pageshow();
                        $('#txtFPNewUsername, #txtFPNewPassword').val('');
                    } else {
                        $('#divFPSecQErrMsg').text(e.Results).show();
                    }
                }
            })
        })
        .on('click', '#btnFPResetBack', function (e) {
            e.stopPropagation();
            e.preventDefault();
            $('#divFPSecQ').pageshow();
            $('#divFPResetErrMsg').hide();
        })
        .on('click', '#btnFPResetSave', function (e) {
            e.stopPropagation();
            e.preventDefault();
            var txtFPNewUsername = $('#txtFPNewUsername');
            var txtFPNewPassword = $('#txtFPNewPassword');
            var txtFPEmailAdd = $('#txtFPEmailAdd');
            var hasError = false;

            $('#divFPResetErrMsg').text('Please complete the required field(s)').hide();
            $(this).text('Saving ...').attr('disabled', true);

            if ($.trim(txtFPNewUsername.val()) == '') {
                txtFPNewUsername.addClass('is-invalid');
                hasError = true;
            } else {
                if ($.trim(txtFPNewUsername.val()).length < 8) {
                    txtFPNewUsername.addClass('is-invalid');
                    hasError = true;
                    $(this).text('SAVE CHANGES').removeAttr('disabled');
                    $('#divFPResetErrMsg').text('Username must be 8 characters or more').show();
                    return;
                } else {
                    txtFPNewUsername.removeClass('is-invalid');
                }                
            }

            if ($.trim(txtFPNewPassword.val()) == '') {
                txtFPNewPassword.addClass('is-invalid');
                hasError = true;
            } else {
                if ($.trim(txtFPNewPassword.val()).length < 8) {
                    txtFPNewPassword.addClass('is-invalid');
                    hasError = true;
                    $(this).text('SAVE CHANGES').removeAttr('disabled');
                    $('#divFPResetErrMsg').text('Password must be 8 characters or more').show();
                    return;
                } else {
                    txtFPNewPassword.removeClass('is-invalid');
                }
            }

            if(hasError){
                $('#divFPResetErrMsg').show();
                $(this).text('SAVE CHANGES').removeAttr('disabled');
                return;
            }

            var data = {
                Username: $.trim(txtFPNewUsername.val()),
                Password: $.trim(txtFPNewPassword.val()),
                DateNow: $.format.date(new Date(), 'MMMM dd, yyyy hh:mm a'),
                EmailAddress: $.trim(txtFPEmailAdd.val())
            }

            AjaxSend({
                url: '/Ecommerce/ResetCredentials',
                data: { data: data },
                success: function (e) {
                    $('#btnFPResetSave').text('SAVE CHANGES').removeAttr('disabled');

                    if(e.Results == 'saved'){
                        $('#divFPSuccess').pageshow();
                    } else {
                        $('#divFPResetErrMsg').text(e.Results).show();
                    }
                }
            })
        })

        function CheckUser() {
            var txtFPFirstname = $('#txtFPFirstname');
            var txtFPMiddlename = $('#txtFPMiddlename');
            var txtFPLastname = $('#txtFPLastname');
            var selFPPersonGenSuffix = $('#selFPPersonGenSuffix');
            var txtFPEmailAdd = $('#txtFPEmailAdd');
            var hasError = false;

            $('#divFPErrMsg').text('Please fill in the required field(s) above.').hide();
            $('#btnFPNext').text('Checking ...').attr('disabled', true);

            if ($.trim(txtFPFirstname.val()) == '') {
                txtFPFirstname.addClass('is-invalid');
                hasError = true;
            } else {
                txtFPFirstname.removeClass('is-invalid');
            }

            if ($.trim(txtFPLastname.val()) == '') {
                txtFPLastname.addClass('is-invalid');
                hasError = true;
            } else {
                txtFPLastname.removeClass('is-invalid');
            }

            if ($.trim(txtFPEmailAdd.val()) == '') {
                txtFPEmailAdd.addClass('is-invalid');
                hasError = true;
            } else {
                if (!IsEmail($.trim(txtFPEmailAdd.val()))) {
                    txtFPEmailAdd.addClass('is-invalid');
                    hasError = true;
                    $('#btnFPNext').text('NEXT').removeAttr('disabled');
                    $('#divFPErrMsg').text('Invalid email address').show();
                    return
                } else {
                    txtFPEmailAdd.removeClass('is-invalid');
                }                
            }

            if (hasError) {
                $('#btnFPNext').text('NEXT').removeAttr('disabled');
                $('#divFPErrMsg').show();
                return;
            }

            var data = {
                FirstName: $.trim(txtFPFirstname.val()),
                MiddleName: $.trim(txtFPMiddlename.val()),
                LastName: $.trim(txtFPLastname.val()),
                GenSuffix: $.trim(selFPPersonGenSuffix.val()),
                EmailAddress: $.trim(txtFPEmailAdd.val())
            }

            AjaxSend({
                url: '/Ecommerce/CheckUserForgotPassword',
                data: { data: data },
                success: function (e) {
                    $('#btnFPNext').text('NEXT').removeAttr('disabled');

                    if ($.isArray(e.Results)) {
                        arrSecQ = e.Results;
                        $('#lblSecQ1').text(e.Results[0].SecurityQuestion);                        
                        $('#lblSecQ2').text(e.Results[1].SecurityQuestion);
                        $('#txtFPSecQ1Ans, #txtFPSecQ2Ans').val('');
                        $('#divFPSecQ').pageshow();
                    } else {
                        if (e.Results == 'invalid') {
                            $('#divFPErrMsg').text('The information you entered does not match any account in our records').show()
                        }
                    }
                }
            })
        }

        function CreateAccount() {
            var txtCompanyName = $('#txtCompanyName');
            var txtCompanyTIN = $('#txtCompanyTIN');
            var selBusinessIndustry = $('#selBusinessIndustry');
            var txtCompanyEmailAdd = $('#txtCompanyEmailAdd');
            var selCompanyTelcoPrefix = $('#selCompanyTelcoPrefix');
            var txtCompanyMobileNo = $('#txtCompanyMobileNo');
            var txtCompanyAddressBldgNo = $('#txtCompanyAddressBldgNo');
            var txtCompanyAddressStreetName = $('#txtCompanyAddressStreetName');
            var selCompanyAddressCity = $('#selCompanyAddressCity');
            var selCompanyAddressCountry = $('#selCompanyAddressCountry');

            var txtPersonFirstName = $('#txtPersonFirstName');
            var txtPersonMiddleName = $('#txtPersonMiddleName');
            var txtPersonLastName = $('#txtPersonLastName');
            var selPersonGenSuffix = $('#selPersonGenSuffix');
            var selPersonBirthMonth = $('#selPersonBirthMonth');
            var selPersonBirthDay = $('#selPersonBirthDay');
            var txtPersonBirthYear = $('#txtPersonBirthYear');
            var txtPersonEmailAdd = $('#txtPersonEmailAdd');
            var txtPersonAddressBldgNo = $('#txtPersonAddressBldgNo');
            var txtPersonAddressStreetName = $('#txtPersonAddressStreetName');
            var selPersonAddressCity = $('#selPersonAddressCity');
            var selPersonAddressCountry = $('#selPersonAddressCountry');
            var txtPersonUsername = $('#txtPersonUsername');
            var txtPersonPassword = $('#txtPersonPassword');
            var txtPersonConfirmPassword = $('#txtPersonConfirmPassword');
            var selSecQuestion1 = $('#selSecQuestion1');
            var txtSecQuestion1Answer = $('#txtSecQuestion1Answer');
            var selSecQuestion2 = $('#selSecQuestion2');
            var txtSecQuestion2Answer = $('#txtSecQuestion2Answer');

            var hasError = false, data = {};

            $('#divErrMsg').text('Please fill in the required field(s) above.').hide();

            if ($.trim(txtPersonFirstName.val()) == '') {
                txtPersonFirstName.addClass('is-invalid');
                hasError = true;
            } else {
                txtPersonFirstName.removeClass('is-invalid');
            }

            if ($.trim(txtPersonLastName.val()) == '') {
                txtPersonLastName.addClass('is-invalid');
                hasError = true;
            } else {
                txtPersonLastName.removeClass('is-invalid');
            }

            if ($.trim(selPersonBirthMonth.val()) == '') {
                selPersonBirthMonth.addClass('is-invalid');
                hasError = true;
            } else {
                selPersonBirthMonth.removeClass('is-invalid');
            }

            if ($.trim(selPersonBirthDay.val()) == '') {
                selPersonBirthDay.addClass('is-invalid');
                hasError = true;
            } else {
                selPersonBirthDay.removeClass('is-invalid');
            }

            if ($.trim(txtPersonBirthYear.val()) == '') {
                txtPersonBirthYear.addClass('is-invalid');
                hasError = true;
            } else {
                txtPersonBirthYear.removeClass('is-invalid');
            }

            if ($.trim(txtPersonEmailAdd.val()) == '') {
                txtPersonEmailAdd.addClass('is-invalid');
                hasError = true;
            } else {
                txtPersonEmailAdd.removeClass('is-invalid');
            }

            if ($.trim(txtPersonAddressBldgNo.val()) == '') {
                txtPersonAddressBldgNo.addClass('is-invalid');
                hasError = true;
            } else {
                txtPersonAddressBldgNo.removeClass('is-invalid');
            }

            if ($.trim(txtPersonAddressStreetName.val()) == '') {
                txtPersonAddressStreetName.addClass('is-invalid');
                hasError = true;
            } else {
                txtPersonAddressStreetName.removeClass('is-invalid');
            }

            if ($.trim(selPersonAddressCity.val()) == '') {
                selPersonAddressCity.addClass('is-invalid');
                hasError = true;
            } else {
                selPersonAddressCity.removeClass('is-invalid');
            }

            if ($.trim(selPersonAddressCountry.val()) == '') {
                selPersonAddressCountry.addClass('is-invalid');
                hasError = true;
            } else {
                selPersonAddressCountry.removeClass('is-invalid');
            }

            if ($.trim(txtPersonUsername.val()) == '') {
                txtPersonUsername.addClass('is-invalid');
                hasError = true;
            } else {
                if ($.trim(txtPersonUsername.val()).length < 8) {
                    $('#divErrMsg').text('Username must be 8 characters or more');
                    txtPersonUsername.addClass('is-invalid');
                    hasError = true;
                } else {
                    txtPersonUsername.removeClass('is-invalid');
                }
            }

            if ($.trim(txtPersonPassword.val()) == '') {
                txtPersonPassword.addClass('is-invalid');
                hasError = true;
            } else {
                if ($.trim(txtPersonPassword.val()).length < 8) {
                    $('#divErrMsg').text('Password must be 8 characters or more');
                    txtPersonPassword.addClass('is-invalid');
                    hasError = true;
                } else {
                    txtPersonPassword.removeClass('is-invalid');
                }                
            }

            if ($.trim(txtPersonConfirmPassword.val()) == '') {
                txtPersonConfirmPassword.addClass('is-invalid');
                hasError = true;
            } else {
                if ($.trim(txtPersonPassword.val()) != $.trim(txtPersonConfirmPassword.val())) {
                    $('#divErrMsg').text('Confirm password don\'t match');
                    hasError = true;
                } else {
                    txtPersonConfirmPassword.removeClass('is-invalid');
                }
            }

            if ($.trim(selSecQuestion1.val()) == '') {
                selSecQuestion1.addClass('is-invalid');
                hasError = true;
            } else {
                selSecQuestion1.removeClass('is-invalid');
            }

            if ($.trim(txtSecQuestion1Answer.val()) == '') {
                txtSecQuestion1Answer.addClass('is-invalid');
                hasError = true;
            } else {
                txtSecQuestion1Answer.removeClass('is-invalid');
            }

            if ($.trim(selSecQuestion2.val()) == '') {
                selSecQuestion2.addClass('is-invalid');
                hasError = true;
            } else {
                selSecQuestion2.removeClass('is-invalid');
            }

            if ($.trim(txtSecQuestion2Answer.val()) == '') {
                txtSecQuestion2Answer.addClass('is-invalid');
                hasError = true;
            } else {
                txtSecQuestion2Answer.removeClass('is-invalid');
            }

            if (hasError) {
                $('#btnCreateAccount').text('CREATE ACCOUNT').removeAttr('disabled');
                $('#divErrMsg').show();
                return;
            }

            if (accounttype == 'company') {
                data.CompanyName = $.trim(txtCompanyName.val());
                data.Tin = $.trim(txtCompanyTIN.val());
                data.BusinessIndustry = busindid;
                data.CompanyEmailAdd = $.trim(txtCompanyEmailAdd.val());
                data.TelcoPrefixID = $.trim(selCompanyTelcoPrefix.val());
                data.CompanyMobileNo = $.trim(txtCompanyMobileNo.val());
                data.CompanyBldgNo = $.trim(txtCompanyAddressBldgNo.val());
                data.CompanyStreetName = $.trim(txtCompanyAddressStreetName.val());
                data.CompanyCity = $.trim(selCompanyAddressCity.val());
                data.CompanyCountry = $.trim(selCompanyAddressCountry.val());
                data.CompanyCityName = $.trim(selCompanyAddressCity.find('option:selected').text());
                data.CompanyCountryName = $.trim(selCompanyAddressCountry.find('option:selected').text());
            }

            data.FirstName = $.trim(txtPersonFirstName.val());
            data.MiddleName = $.trim(txtPersonMiddleName.val());
            data.LastName = $.trim(txtPersonLastName.val());
            data.GenSuffix = selPersonGenSuffix.val();
            data.DateOfBirth = selPersonBirthMonth.val() + ' ' + selPersonBirthDay.val() + ', ' + txtPersonBirthYear.val();
            data.PersonEmailAdd = $.trim(txtPersonEmailAdd.val());
            data.PersonBldgNo = $.trim(txtPersonAddressBldgNo.val());
            data.PersonStreetName = $.trim(txtPersonAddressStreetName.val());
            data.PersonCity = $.trim(selPersonAddressCity.val());
            data.PersonCountry = $.trim(selPersonAddressCountry.val());
            data.PersonCityName = $.trim(selPersonAddressCity.find('option:selected').text());
            data.PersonCountryName = $.trim(selPersonAddressCountry.find('option:selected').text());
            data.Username = $.trim(txtPersonUsername.val());
            data.Password = $.trim(txtPersonPassword.val());
            data.ConfirmPassword = $.trim(txtPersonConfirmPassword.val());
            data.SecQ1 = $.trim(selSecQuestion1.val());
            data.SecQ1Ans = $.trim(txtSecQuestion1Answer.val());
            data.SecQ2 = $.trim(selSecQuestion2.val());
            data.SecQ2Ans = $.trim(txtSecQuestion2Answer.val());
            data.AccountType = accounttype;
            data.DateNow = $.format.date(new Date(), 'MMM dd, yyyy hh:mm a');
            data.IsCustomer = isCustomer;

            AjaxSend({
                url: '/Ecommerce/CreateAccount?OrgID=' + sessionStorage.OrgID,
                data: { details: data, url: location.origin },
                success: function (e) {
                    $('#btnCreateAccount').text('CREATE ACCOUNT').removeAttr('disabled');
                    if(e.Results == "exist"){
                        $('#divErrMsg').text('Username already exist. Please change your username.').show();
                    } else if(e.Results == "saved") {
                        $('#lblSentTo').text(data.PersonEmailAdd);
                        $('#divSuccess').pageshow();
                    } else {
                        $('#divErrMsg').text(e.Results).show();
                    }
                }
            })
        }

        function LoadData(data) {
            //var data = JSON.parse(additionalData).d; // additionalData declared in signin.vbhtml;

            var selPersonBirthDay = $('#selPersonBirthDay');
            var selBusinessIndustry = $('#selBusinessIndustry');
            var selCompanyAddressCity = $('#selCompanyAddressCity');
            var selCompanyAddressCountry = $('#selCompanyAddressCountry');
            var selPersonAddressCity = $('#selPersonAddressCity');
            var selPersonAddressCountry = $('#selPersonAddressCountry');
            var selCompanyTelcoPrefix = $('#selCompanyTelcoPrefix');

            selBusinessIndustry.append(new Option('Please select', ''));
            selCompanyAddressCity.append(new Option('Please select', ''));
            selCompanyAddressCountry.append(new Option('Please select', ''));
            selPersonAddressCity.append(new Option('Please select', ''));
            selPersonAddressCountry.append(new Option('Please select', ''));
            selCompanyTelcoPrefix.append(new Option('Please select', ''));

            arrBusInd = data.BusinessIndustries;

            //for (var i = 0; i < data.BusinessIndustries.length; i++){
            //    selBusinessIndustry.append(new Option(data.BusinessIndustries[i].Class, data.BusinessIndustries[i].ID));
            //}

            for (var i = 0; i < data.Cities.length; i++) {
                selCompanyAddressCity.append(new Option(data.Cities[i].Name, data.Cities[i].ID));
                selPersonAddressCity.append(new Option(data.Cities[i].Name, data.Cities[i].ID));
            }

            for (var i = 0; i < data.Countries.length; i++) {
                selCompanyAddressCountry.append(new Option(data.Countries[i].Name, data.Countries[i].ID));
                selPersonAddressCountry.append(new Option(data.Countries[i].Name, data.Countries[i].ID));
            }

            selCompanyAddressCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e'); // default to PH
            selPersonAddressCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e'); // default to PH

            for (var i = 0; i < data.Telco.length; i++) {
                selCompanyTelcoPrefix.append(new Option(data.Telco[i].Prefix, data.Telco[i].ID));
            }

            selPersonBirthDay.empty().append(new Option('Day', ''));

            for (var i = 1; i <= 31; i++) {
                selPersonBirthDay.append(new Option(i, i))
            }
        }

        function ClearFields() {
            busindid = '';

            $('#txtCompanyName, #txtCompanyTIN, #selBusinessIndustry, #txtCompanyEmailAdd, ' +
                '#txtCompanyMobileNo, #txtCompanyAddressBldgNo, #txtCompanyAddressStreetName, ' +
                '#selCompanyAddressCity, #selCompanyAddressCountry, ' +
                '#txtPersonFirstName, #txtPersonMiddleName, #txtPersonLastName, ' +
                '#selPersonGenSuffix, #selPersonBirthMonth, #selPersonBirthDay, ' +
                '#txtPersonBirthYear, #txtPersonEmailAdd, #txtPersonUsername, ' +
                '#txtPersonPassword, #txtPersonConfirmPassword').val('').removeClass('is-invalid');

            $('#selCompanyAddressCountry').val('e783ed13-aa98-4cc1-918e-d49b05c0167e'); // default to PH
        }

        function ClearFPFields() {
            $('#txtFPFirstname, #txtFPMiddlename, #txtFPLastname, #selFPPersonGenSuffix, #txtFPEmailAdd').val('');
            $('#selPersonAddressCountry').val('e783ed13-aa98-4cc1-918e-d49b05c0167e'); // default to PH
        }
    })
    .click(function () {
        $("#listBusinessIndustry").hide();
    });
})