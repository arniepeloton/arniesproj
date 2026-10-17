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
                txtuname.parent().addClass('has-error');
                hasError = true;
            } else {
                txtuname.parent().removeClass('has-error');
            }

            if ($.trim(txtpword.val()) == '') {
                txtpword.parent().addClass('has-error');
                hasError = true;
            } else {
                txtpword.parent().removeClass('has-error');
            }

            if(hasError){
                return;
            }

            AjaxSend({
                url: '/VendorPortal/Login',
                data: { url: location.origin, uname: $.trim(txtuname.val()), pword: $.trim(txtpword.val()), iscanvass: (GetUrlParameters().redirect == 'canvassform' ? 1 : 0), canvasscode: CStr2(GetUrlParameters().c) },
                success: function (e) {
                    var res = e.Results;
                    if (res == 'pending request'){
                        $('#divErrLoginMsg').text('Your request to have an access to Vendor Portal is still pending, please wait for the approval of your request.').show();
                    } else if (res == 'disapproved') {
                        $('#divErrLoginMsg').text('Sorry! You\'ve been denied to access the Vendor Portal!').show();
                    } else if (res == 'invalid') {
                        $('#divErrLoginMsg').text('Invalid username / password').show();
                    } else if (res == 'confirm email') {
                        $('#divErrLoginMsg').html('We sent a verification link to <b>' + e.EmailAddress + '</b>, please use the sent link to verify your email address.').show();
                    } else if(res == 'loggedin') {
                        sessionStorage.PeopleUserID = e.PeopleUserID;
                        $('#divErrLoginMsg').hide();
                        if(e.vcode != undefined){
                            location.href = '/Canvass?v=' + e.vcode + '&c=' + CStr2(GetUrlParameters().c);
                        } else {
                            sessionStorage.SupplierID = e.SupplierID;
                            sessionStorage.SupplierName = e.SupplierName;
                            window.location.href = '/VendorPortal/Dashboard?OrgID=' + sessionStorage.OrgID;
                        }                        
                    } else {
                        //invalid canvass code
                        $('#divErrLoginMsg').text('Link is broken. Please double check the given link.').show();
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
                txtFPSecQ1Ans.parent().addClass('has-error');
                hasError = true;
            } else {
                txtFPSecQ1Ans.parent().removeClass('has-error');
            }

            if ($.trim(txtFPSecQ2Ans.val()) == '') {
                txtFPSecQ2Ans.parent().addClass('has-error');
                hasError = true;
            } else {
                txtFPSecQ2Ans.parent().removeClass('has-error');
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
                txtFPNewUsername.parent().addClass('has-error');
                hasError = true;
            } else {
                if ($.trim(txtFPNewUsername.val()).length < 8) {
                    txtFPNewUsername.parent().addClass('has-error');
                    hasError = true;
                    $(this).text('SAVE CHANGES').removeAttr('disabled');
                    $('#divFPResetErrMsg').text('Username must be 8 characters or more').show();
                    return;
                } else {
                    txtFPNewUsername.parent().removeClass('has-error');
                }                
            }

            if ($.trim(txtFPNewPassword.val()) == '') {
                txtFPNewPassword.parent().addClass('has-error');
                hasError = true;
            } else {
                if ($.trim(txtFPNewPassword.val()).length < 8) {
                    txtFPNewPassword.parent().addClass('has-error');
                    hasError = true;
                    $(this).text('SAVE CHANGES').removeAttr('disabled');
                    $('#divFPResetErrMsg').text('Password must be 8 characters or more').show();
                    return;
                } else {
                    txtFPNewPassword.parent().removeClass('has-error');
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
                txtFPFirstname.parent().addClass('has-error');
                hasError = true;
            } else {
                txtFPFirstname.parent().removeClass('has-error');
            }

            if ($.trim(txtFPLastname.val()) == '') {
                txtFPLastname.parent().addClass('has-error');
                hasError = true;
            } else {
                txtFPLastname.parent().removeClass('has-error');
            }

            if ($.trim(txtFPEmailAdd.val()) == '') {
                txtFPEmailAdd.parent().addClass('has-error');
                hasError = true;
            } else {
                if (!IsEmail($.trim(txtFPEmailAdd.val()))) {
                    txtFPEmailAdd.parent().addClass('has-error');
                    hasError = true;
                    $('#btnFPNext').text('NEXT').removeAttr('disabled');
                    $('#divFPErrMsg').text('Invalid email address').show();
                    return
                } else {
                    txtFPEmailAdd.parent().removeClass('has-error');
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
                txtPersonFirstName.parent().addClass('has-error');
                hasError = true;
            } else {
                txtPersonFirstName.parent().removeClass('has-error');
            }

            if ($.trim(txtPersonLastName.val()) == '') {
                txtPersonLastName.parent().addClass('has-error');
                hasError = true;
            } else {
                txtPersonLastName.parent().removeClass('has-error');
            }

            if ($.trim(selPersonBirthMonth.val()) == '') {
                selPersonBirthMonth.parent().addClass('has-error');
                hasError = true;
            } else {
                selPersonBirthMonth.parent().removeClass('has-error');
            }

            if ($.trim(selPersonBirthDay.val()) == '') {
                selPersonBirthDay.parent().addClass('has-error');
                hasError = true;
            } else {
                selPersonBirthDay.parent().removeClass('has-error');
            }

            if ($.trim(txtPersonBirthYear.val()) == '') {
                txtPersonBirthYear.parent().addClass('has-error');
                hasError = true;
            } else {
                txtPersonBirthYear.parent().removeClass('has-error');
            }

            if ($.trim(txtPersonEmailAdd.val()) == '') {
                txtPersonEmailAdd.parent().addClass('has-error');
                hasError = true;
            } else {
                txtPersonEmailAdd.parent().removeClass('has-error');
            }

            if ($.trim(txtPersonAddressBldgNo.val()) == '') {
                txtPersonAddressBldgNo.parent().addClass('has-error');
                hasError = true;
            } else {
                txtPersonAddressBldgNo.parent().removeClass('has-error');
            }

            if ($.trim(txtPersonAddressStreetName.val()) == '') {
                txtPersonAddressStreetName.parent().addClass('has-error');
                hasError = true;
            } else {
                txtPersonAddressStreetName.parent().removeClass('has-error');
            }

            if ($.trim(selPersonAddressCity.val()) == '') {
                selPersonAddressCity.parent().addClass('has-error');
                hasError = true;
            } else {
                selPersonAddressCity.parent().removeClass('has-error');
            }

            if ($.trim(selPersonAddressCountry.val()) == '') {
                selPersonAddressCountry.parent().addClass('has-error');
                hasError = true;
            } else {
                selPersonAddressCountry.parent().removeClass('has-error');
            }

            if ($.trim(txtPersonUsername.val()) == '') {
                txtPersonUsername.parent().addClass('has-error');
                hasError = true;
            } else {
                if ($.trim(txtPersonUsername.val()).length < 8) {
                    $('#divErrMsg').text('Username must be 8 characters or more');
                    txtPersonUsername.parent().addClass('has-error');
                    hasError = true;
                } else {
                    txtPersonUsername.parent().removeClass('has-error');
                }
            }

            if ($.trim(txtPersonPassword.val()) == '') {
                txtPersonPassword.parent().addClass('has-error');
                hasError = true;
            } else {
                if ($.trim(txtPersonPassword.val()).length < 8) {
                    $('#divErrMsg').text('Password must be 8 characters or more');
                    txtPersonPassword.parent().addClass('has-error');
                    hasError = true;
                } else {
                    txtPersonPassword.parent().removeClass('has-error');
                }                
            }

            if ($.trim(txtPersonConfirmPassword.val()) == '') {
                txtPersonConfirmPassword.parent().addClass('has-error');
                hasError = true;
            } else {
                if ($.trim(txtPersonPassword.val()) != $.trim(txtPersonConfirmPassword.val())) {
                    $('#divErrMsg').text('Confirm password don\'t match');
                    hasError = true;
                } else {
                    txtPersonConfirmPassword.parent().removeClass('has-error');
                }
            }

            if ($.trim(selSecQuestion1.val()) == '') {
                selSecQuestion1.parent().addClass('has-error');
                hasError = true;
            } else {
                selSecQuestion1.parent().removeClass('has-error');
            }

            if ($.trim(txtSecQuestion1Answer.val()) == '') {
                txtSecQuestion1Answer.parent().addClass('has-error');
                hasError = true;
            } else {
                txtSecQuestion1Answer.parent().removeClass('has-error');
            }

            if ($.trim(selSecQuestion2.val()) == '') {
                selSecQuestion2.parent().addClass('has-error');
                hasError = true;
            } else {
                selSecQuestion2.parent().removeClass('has-error');
            }

            if ($.trim(txtSecQuestion2Answer.val()) == '') {
                txtSecQuestion2Answer.parent().addClass('has-error');
                hasError = true;
            } else {
                txtSecQuestion2Answer.parent().removeClass('has-error');
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
            data.IsCustomer = 0;

            AjaxSend({
                url: '/VendorPortal/CreateAccount?OrgID=' + sessionStorage.OrgID,
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
                '#txtPersonPassword, #txtPersonConfirmPassword').val('').parent().removeClass('has-error');

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