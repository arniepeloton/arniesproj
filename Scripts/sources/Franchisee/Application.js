require(['common'], function () {
    $(document).ready(function () {
        var accounttype, arrBusInd = [], busindid = '', arrSecQ = [], isCustomer = 1;

        AjaxSend({
            url: MergeUrlParameters(GetUrlParameters(), '/Franchisee/GetAdditionalDateForSignin'),
            success: function (e) {
                LoadData(e);
            }
        })

        $('body')
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

            if ($.trim(txtCompanyTIN.val()) == '') {
                txtCompanyTIN.addClass('is-invalid');
                hasError = true;
            } else {
                txtCompanyTIN.removeClass('is-invalid');
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

            $('#divStoreDetails').pageshow();
            $(window).scrollTop(0);
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
        .on('click', '#btnStoreBack', function (e) {
            e.preventDefault();
            e.stopPropagation();
            $('#divCompanyAcc').pageshow();
        })
        .on('click', '#btnStoreNextStep', function (e) {
            e.stopPropagation();
            e.preventDefault();
            var txtStoreName = $('#txtStoreName');
            var selStoreBusinessName = $('#selStoreBusinessName');
            var txtStoreAddressBldgNo = $('#txtStoreAddressBldgNo');
            var txtStoreAddressStreetName = $('#txtStoreAddressStreetName');
            var selStoreAddressCity = $('#selStoreAddressCity');
            var selStoreAddressCountry = $('#selStoreAddressCountry');
            var hasError = false;

            $('#divStoreErrMsg').text('Please fill in the required field(s) above.').hide();

            if ($.trim(txtStoreName.val()) == '') {
                txtStoreName.addClass('is-invalid');
                hasError = true;
            } else {
                txtStoreName.removeClass('is-invalid');
            }

            if ($.trim(selStoreBusinessName.val()) == '') {
                selStoreBusinessName.addClass('is-invalid');
                hasError = true;
            } else {
                selStoreBusinessName.removeClass('is-invalid');
            }

            if ($.trim(txtStoreAddressBldgNo.val()) == '') {
                txtStoreAddressBldgNo.addClass('is-invalid');
                hasError = true;
            } else {
                txtStoreAddressBldgNo.removeClass('is-invalid');
            }

            if ($.trim(txtStoreAddressStreetName.val()) == '') {
                txtStoreAddressStreetName.addClass('is-invalid');
                hasError = true;
            } else {
                txtStoreAddressStreetName.removeClass('is-invalid');
            }

            if ($.trim(selStoreAddressCity.val()) == '') {
                selStoreAddressCity.addClass('is-invalid');
                hasError = true;
            } else {
                selStoreAddressCity.removeClass('is-invalid');
            }

            if ($.trim(selStoreAddressCountry.val()) == '') {
                selStoreAddressCountry.addClass('is-invalid');
                hasError = true;
            } else {
                selStoreAddressCountry.removeClass('is-invalid');
            }

            if (hasError) {
                $('#divStoreErrMsg').show();
                return;
            }

            $('#divPersonalAcc').pageshow();
            $(window).scrollTop(0);
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
            $('#divStoreDetails').pageshow();
        })

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

            var txtStoreName = $('#txtStoreName');
            var selStoreBusinessName = $('#selStoreBusinessName');
            var txtStoreAddressBldgNo = $('#txtStoreAddressBldgNo');
            var txtStoreAddressStreetName = $('#txtStoreAddressStreetName');
            var selStoreAddressCity = $('#selStoreAddressCity');
            var selStoreAddressCountry = $('#selStoreAddressCountry');

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

            data.FowstType = '6af9bc83-426c-4774-a051-044fc09c552b'; // STORE
            data.StoreName = $.trim(txtStoreName.val());
            data.BusinessTradeNameID = $.trim(selStoreBusinessName.val());
            data.StoreBldgNo = $.trim(txtStoreAddressBldgNo.val());
            data.StoreStreetName = $.trim(txtStoreAddressStreetName.val());
            data.StoreCity = $.trim(selStoreAddressCity.val());            
            data.StoreCountry = $.trim(selStoreAddressCountry.val());
            data.StoreCityName = $.trim(selStoreAddressCity.find('option:selected').text());
            data.StoreCountryName = $.trim(selStoreAddressCountry.find('option:selected').text());

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
                url: '/Franchisee/CreateAccount?OrgID=' + sessionStorage.OrgID,
                data: { details: data, url: location.origin },
                success: function (e) {
                    $('#btnCreateAccount').text('CREATE ACCOUNT').removeAttr('disabled');
                    if (e.Results == "exist") {
                        $('#divErrMsg').text('Username already exist. Please change your username.').show();
                    } else if (e.Results == "saved") {
                        sessionStorage.EmailAddress = data.PersonEmailAdd;
                        location.href = '/franchisee/success?OrgID=' + sessionStorage.OrgID;
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
            var selStoreAddressCity = $('#selStoreAddressCity');
            var selStoreAddressCountry = $('#selStoreAddressCountry');
            var selPersonAddressCity = $('#selPersonAddressCity');
            var selPersonAddressCountry = $('#selPersonAddressCountry');
            var selCompanyTelcoPrefix = $('#selCompanyTelcoPrefix');
            var selStoreBusinessName = $('#selStoreBusinessName');

            selBusinessIndustry.append(new Option('Please select', ''));
            selCompanyAddressCity.append(new Option('Please select', ''));
            selCompanyAddressCountry.append(new Option('Please select', ''));
            selStoreAddressCity.append(new Option('Please select', ''));
            selStoreAddressCountry.append(new Option('Please select', ''));
            selPersonAddressCity.append(new Option('Please select', ''));
            selPersonAddressCountry.append(new Option('Please select', ''));
            selCompanyTelcoPrefix.append(new Option('Please select', ''));

            arrBusInd = data.BusinessIndustries;

            //for (var i = 0; i < data.BusinessIndustries.length; i++){
            //    selBusinessIndustry.append(new Option(data.BusinessIndustries[i].Class, data.BusinessIndustries[i].ID));
            //}

            for (var i = 0; i < data.Cities.length; i++) {
                selCompanyAddressCity.append(new Option(data.Cities[i].Name, data.Cities[i].ID));
                selStoreAddressCity.append(new Option(data.Cities[i].Name, data.Cities[i].ID));
                selPersonAddressCity.append(new Option(data.Cities[i].Name, data.Cities[i].ID));
            }

            for (var i = 0; i < data.Countries.length; i++) {
                selCompanyAddressCountry.append(new Option(data.Countries[i].Name, data.Countries[i].ID));
                selStoreAddressCountry.append(new Option(data.Countries[i].Name, data.Countries[i].ID));
                selPersonAddressCountry.append(new Option(data.Countries[i].Name, data.Countries[i].ID));
            }

            for (var i = 0; i < data.BusinessTradeName.length; i++) {
                selStoreBusinessName.append(new Option(data.BusinessTradeName[i].BusinessTradeName, data.BusinessTradeName[i].ID));
            }

            selCompanyAddressCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e'); // default to PH
            selStoreAddressCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e'); // default to PH
            selPersonAddressCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e'); // default to PH

            for (var i = 0; i < data.Telco.length; i++) {
                selCompanyTelcoPrefix.append(new Option(data.Telco[i].Prefix, data.Telco[i].ID));
            }

            selPersonBirthDay.empty().append(new Option('Day', ''));

            for (var i = 1; i <= 31; i++) {
                selPersonBirthDay.append(new Option(i, i))
            }
        }
    }).click(function () {
        $("#listBusinessIndustry").hide();
    });
})