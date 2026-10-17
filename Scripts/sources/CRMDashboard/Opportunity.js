require(['common'],
    function () {
        $(document).ready(function () {
            var arrOppTelInfo = [], OppTelState = '', intOppTelInfoCtr = 0, TelRankNo = "";
            var arrOppMobileInfo = [], OppMobileState = '', intOppMobileInfoCtr = 0, MobileRankNo = "";
            var arrOppEmailInfo = [], OppEmailState = '', intOppEmailInfoCtr = 0, EmailRankNo = "";
            var arrOppContactPersonList = [];
            var intOppContactPersonCtr = 0;
            var ContactPersonState = "";
            var OppContactPersonRankNo = "";
            var intOppContactPersonCtr = 0;
            var arrOppContactPersonTelInfoList = [];
            var intOppContactPersonTelInfoCtr = 0;
            var OppContactPersonTelState = "";
            var OppContactPersonTelRankNo = "";
            var arrOppContactPersonMobileInfoList = [];
            var intOppContactPersonMobileInfoCtr = 0;
            var OppContactPersonMobileState = "";
            var OppContactPersonMobileRankNo = "";
            var arrOppContactPersonEmailInfoList = [];
            var intOppContactPersonEmailInfoCtr = 0;
            var OppContactPersonEmailState = "";
            var OppContactPersonEmailRankNo = "";
            var isByClient = false;
            var oppClientID = '', templateID = '', parentOppID = '';
            var dtOpportunity;
            var caretPos;
            var listOppTemplate, isAddOpportunity = true;

            window.Opportunity = {
                LoadRequiredData: function () {
                    DisplayCities();
                    GetClientList();
                },
                GetOpportunityByClient: function (id) {
                    isByClient = true;
                    oppClientID = id;
                    GetOpportunityByClient(id);
                },
                GetOpportunity: function () {
                    isByClient = false;
                    GetOpportunity();
                },
                GetFunnelTemplate: function () {
                    GetFunnelTemplate();
                }
            }

            $('body')
            .on('click', '#btnAddOpportunity', function () {
                PromptUserAccess('Add Opportunity', {
                    OnClickOKSuccess: function () {
                        isAddOpportunity = true;
                        $('#popupNewOpportunity').modal('show');
                        $('#txtOpportunityName').focus();
                        $('#btnTabOppComment, #divOppStatus').hide();
                        ClearOpportunityFields();
                    }
                });
            })
            .on('click', '#btnAddOppTelephone', function () {
                ClearOppTelInfo();
                $('#popOppTelInfo').modal('show');
            })
            .on('click', '#btnCancelOppTelephone', function () {
                $('#popOppTelInfo').modal('hide');
                OppTelState = '';
            })
            .on('click', '#btnOppTelAreaCode', function () {
                BrowsePopup('LibraryAreaCodes', 'Area Codes',
                    [
                        {
                            DisplayText: [{ Column: 'Area' }, { Label: 'Code', Column: 'CallingCode' }],
                            Click: function (e) {
                                $('#txtOppTelAreaCodeID').val(e.ID);
                                $('#txtOppAreaCode').text(e.Area + ' - ' + e.CallingCode)
                            }
                        }
                    ]
                )
            })
            .on('keydown', '#txtOppTelephoneNum', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('click', '#btnSaveOppTelephone', function () {
                if ($('#txtOppAreaCode').text() == "Select") {
                    WebToast('Area Code is required', 'top');
                    return false;
                }

                if ($('#txtOppTelephoneNum').val() == "") {
                    WebToast('Please enter telephone number', 'top');
                    return false;
                }

                //if ($('#txtOppTelephoneNum').val().length != 7) {
                //    $('#spanerrorOppTelephoneNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                //    return false;
                //}

                if ($('#selOppTelType').val() == "") {
                    WebToast('Please select contact type', 'top');
                    return false;
                }
                intOppTelInfoCtr = intOppTelInfoCtr + 1;

                if (OppTelState == "") {
                    arrOppTelInfo.push({
                        "TelRankNo": intOppTelInfoCtr,
                        "CountryCodeID": $('#txtOppTelCountryID').val(),
                        "CountryCode": $('#txtOppCountryCode').text(),
                        "AreaCode": $('#txtOppAreaCode').text(),
                        "AreaCodeID": $('#txtOppTelAreaCodeID').val(),
                        "TelephoneNo": $('#txtOppTelephoneNum').val(),
                        "ExtensionNo": $('#txtOppTelephoneExt').val(),
                        "ContactType": $('#selOppTelType').val(),
                        "TelState": "Add",
                        "IsNew": 1,
                        "TelID": $('#txtOppTelID').val()
                    })
                }
                else {
                    arrOppTelInfo[sessionStorage.Telindex].TelRankNo = TelRankNo;
                    arrOppTelInfo[sessionStorage.Telindex].CountryCodeID = $('#txtOppTelCountryID').val();
                    arrOppTelInfo[sessionStorage.Telindex].CountryCode = $('#txtOppCountryCode').text();
                    arrOppTelInfo[sessionStorage.Telindex].AreaCodeID = $('#txtOppTelAreaCodeID').val();
                    arrOppTelInfo[sessionStorage.Telindex].AreaCode = $('#txtOppAreaCode').text();
                    arrOppTelInfo[sessionStorage.Telindex].TelephoneNo = $('#txtOppTelephoneNum').val();
                    arrOppTelInfo[sessionStorage.Telindex].ExtensionNo = $('#txtOppTelephoneExt').val();
                    arrOppTelInfo[sessionStorage.Telindex].ContactType = $('#selOppTelType').val();
                    arrOppTelInfo[sessionStorage.Telindex].TelState = sessionStorage.TelState;
                    arrOppTelInfo[sessionStorage.Telindex].IsNew = sessionStorage.TelIsNew;
                    arrOppTelInfo[sessionStorage.Telindex].TelID = $('#txtOppTelID').val();
                }

                AddOppTelInfo();
                ClearOppTelInfo();
                OppTelState = "";
            })
            .on('click', '#btnAddOppMobile', function () {
                ClearOppMobileInfo();
                $('#popOppMobileInfo').modal('show');
            })
            .on('click', '#btnCancelOppMobile', function () {
                $('#popOppMobileInfo').modal('hide');
                OppMobileState = '';
            })
            .on('click', '#btnOppMobileTelCode', function () {
                BrowsePopup('LibraryTelcoPrefixes', 'Telco Codes',
                    [
                        {
                            DisplayText: [{ Column: 'Prefix' }, { Column: 'TelCo' }],
                            Click: function (e) {
                                $('#txtOppTelCoPrefixID').val(e.ID);
                                $('#txtOppTelCode').text(e.TelCo + ' - ' + e.Prefix)
                            }
                        }
                    ]
                )
            })
            .on('keydown', '#txtOppMobileNo', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('click', '#btnSaveOppMobile', function () {
                if ($('#txtOppTelCode').text() == "Select") {
                    WebToast('Tel Code is required', 'top');
                    return false;
                }

                if ($('#txtOppMobileNo').val() == "") {
                    WebToast('Please enter mobile number', 'top');
                    return false;
                }

                //if ($('#txtOppMobileNo').val().length != 7) {
                //    $('#spanerrorOppMobileNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                //    return false;
                //}

                if ($('#selOppMobileType').val() == "") {
                    WebToast('Please select contact type', 'top');
                    return false;
                }

                intOppMobileInfoCtr = intOppMobileInfoCtr + 1;

                if (OppMobileState == "") {
                    arrOppMobileInfo.push({
                        "MobileRankNo": intOppMobileInfoCtr,
                        "CountryCodeID": $('#txtOppMobileCountryID').val(),
                        "CountryCode": $('#txtOppMobileCountryCode').text(),
                        "TelCode": $('#txtOppTelCode').text(),
                        "TelCodeID": $('#txtOppTelCoPrefixID').val(),
                        "MobileNo": $('#txtOppMobileNo').val(),
                        "ContactType": $('#selOppMobileType').val(),
                        "MobileID": $('#txtOppMobileID').val(),
                        "MobileState": "Add",
                        "IsNew": 1,
                        "MobileNos": '+' + $('#txtOppMobileCountryCode').text() + $('#txtOppTelCode').text() + $('#txtOppMobileNo').val()
                    })
                }
                else {
                    arrOppMobileInfo[sessionStorage.Mobileindex].MobileRankNo = MobileRankNo;
                    arrOppMobileInfo[sessionStorage.Mobileindex].CountryCodeID = $('#txtOppMobileCountryID').val();
                    arrOppMobileInfo[sessionStorage.Mobileindex].CountryCode = $('#txtOppMobileCountryCode').text();
                    arrOppMobileInfo[sessionStorage.Mobileindex].TelCodeID = $('#txtOppTelCoPrefixID').val();
                    arrOppMobileInfo[sessionStorage.Mobileindex].TelCode = $('#txtOppTelCode').text();
                    arrOppMobileInfo[sessionStorage.Mobileindex].MobileNo = $('#txtOppMobileNo').val();
                    arrOppMobileInfo[sessionStorage.Mobileindex].ContactType = $('#selOppMobileType').val();
                    arrOppMobileInfo[sessionStorage.Mobileindex].MobileState = sessionStorage.MobileState;
                    arrOppMobileInfo[sessionStorage.Mobileindex].IsNew = sessionStorage.MobileIsNew;
                    arrOppMobileInfo[sessionStorage.Mobileindex].MobileNos = '+' + $('#txtOppMobileCountryCode').text() + $('#txtOppTelCode').text() + $('#txtOppMobileNo').val()
                }

                AddOppMobileInfo();
                OppMobileState = "";
                ClearOppMobileInfo();
            })
            .on('click', '#btnAddOppEmail', function () {
                ClearOppEmailInfoFields();
                $('#popOppEmailInfo').modal('show');
            })
            .on('click', '#btnCancelOppEmailAddress', function () {
                $('#popOppEmailInfo').modal('hide');
                OppEmailState = '';
            })
            .on('click', '#btnSaveOppEmailAddress', function () {
                if (IsEmail($.trim($("#txtOppEmailAdd").val())) == false) {
                    WebToast('Please enter valid email address', 'top');
                    return false;
                }

                if ($.trim($("#txtOppEmailAdd").val()) == "") {
                    WebToast('Please enter email address', 'top');
                    return false;
                }

                if ($('#selOppEmailType').val() == "") {
                    WebToast('Please select email type', 'top');
                    return false;
                }

                intOppEmailInfoCtr = intOppEmailInfoCtr + 1;

                if (OppEmailState == "") {
                    spanerrorOppContactFname
                    arrOppEmailInfo.push({
                        "EmailRankNo": intOppEmailInfoCtr,
                        "EmailAddID": $('#txtOppEmailID').val(),
                        "EmailAdd": $('#txtOppEmailAdd').val(),
                        "EmailType": $('#selOppEmailType').val(),
                        "EmailState": "Add",
                        "IsNew": 1
                    })
                }
                else {
                    arrOppEmailInfo[sessionStorage.Emailindex].EmailRankNo = EmailRankNo;
                    arrOppEmailInfo[sessionStorage.Emailindex].EmailAddID = $('#txtOppEmailID').val();
                    arrOppEmailInfo[sessionStorage.Emailindex].EmailAdd = $('#txtOppEmailAdd').val();
                    arrOppEmailInfo[sessionStorage.Emailindex].EmailType = $('#selOppEmailType').val();
                    arrOppEmailInfo[sessionStorage.Emailindex].EmailState = sessionStorage.EmailState;
                    arrOppEmailInfo[sessionStorage.Emailindex].IsNew = sessionStorage.EmailIsNew;
                }

                AddOppEmailInfo();
                OppEmailState = "";
            })

            .on('click', '#btnAddOppContactPerson', function () {
                ClearOppContactPersonFields();
                ClearOppContactPersonEmailInfoFields();
                ClearOppContactPersonMobileInfo();
                ClearOppContactPersonTelInfo();
                $('#popOppContactPersonProfile').modal('show');
            })
            .on('click', '#btnOppCancelContact', function () {
                $('#popOppContactPersonProfile').modal('hide');
                ContactPersonState = '';
            })
            .on('click', '#btnOppOkContact', function () {
                if ($.trim($('#txtOppContactFirstName').val()) == "") {
                    WebToast('Please enter your first name', 'top');
                    $('#spanerrorOppContactFname').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                intOppContactPersonCtr = intOppContactPersonCtr + 1;

                if (ContactPersonState == "") {
                    arrOppContactPersonList.push({
                        "OppContactPersonRankNo": intOppContactPersonCtr,
                        "PeopleID": $('#txtPeopleID').val(),
                        "ID": $('#txtOppContactPersonID').val(),
                        "Firstname": $('#txtOppContactFirstName').val(),
                        "Middlename": $('#txtOppContactMiddleName').val(),
                        "Lastname": $('#txtOppContactLastName').val(),
                        "Position": $('#txtOppContactPersonPosition').val(),
                        "PositionDescription": $('#txtOppContactPersonPositionDescription').val(),
                        "AddressID": $('#txtOppContactPersonAddressID').val(),
                        "HouseBuildingNo": $('#txtOppContactPersonHouseBuildingNo').val(),
                        "Street": $('#txtOppContactPersonStreet').val(),
                        "City": $('#selOppContactPersonCity').val(),
                        "Country": $('#selOppContactPersonCountry').val(),
                        "TypeAddress": $('#selOppContactPersonTypeAddress').val(),
                        "ContactPersonState": "Add",
                        "IsNew": 1,
                        "Tel": arrOppContactPersonTelInfoList,
                        "Mobile": arrOppContactPersonMobileInfoList,
                        "Email": arrOppContactPersonEmailInfoList
                    })
                }
                else {
                    arrOppContactPersonList[sessionStorage.Contactindex].OppContactPersonRankNo = OppContactPersonRankNo;
                    arrOppContactPersonList[sessionStorage.Contactindex].PeopleID = $('#txtPeopleID').val();
                    arrOppContactPersonList[sessionStorage.Contactindex].ID = $('#txtOppContactPersonID').val();
                    arrOppContactPersonList[sessionStorage.Contactindex].Firstname = $('#txtOppContactFirstName').val();
                    arrOppContactPersonList[sessionStorage.Contactindex].Middlename = $('#txtOppContactMiddleName').val();
                    arrOppContactPersonList[sessionStorage.Contactindex].Lastname = $('#txtOppContactLastName').val();
                    arrOppContactPersonList[sessionStorage.Contactindex].Position = $('#txtOppContactPersonPosition').val();
                    arrOppContactPersonList[sessionStorage.Contactindex].PositionDescription = $('#txtOppContactPersonPositionDescription').val();
                    arrOppContactPersonList[sessionStorage.Contactindex].AddressID = $('#txtOppContactPersonAddressID').val();
                    arrOppContactPersonList[sessionStorage.Contactindex].HouseBuildingNo = $('#txtOppContactPersonHouseBuildingNo').val();
                    arrOppContactPersonList[sessionStorage.Contactindex].Street = $('#txtOppContactPersonStreet').val();
                    arrOppContactPersonList[sessionStorage.Contactindex].City = $('#selOppContactPersonCity').val();
                    arrOppContactPersonList[sessionStorage.Contactindex].Country = $('#selOppContactPersonCountry').val();
                    arrOppContactPersonList[sessionStorage.Contactindex].TypeAddress = $('#selOppContactPersonTypeAddress').val();
                    arrOppContactPersonList[sessionStorage.Contactindex].ContactPersonState = sessionStorage.ContactPersonState;
                    arrOppContactPersonList[sessionStorage.Contactindex].IsNew = sessionStorage.OppContactPersonIsNew;
                    arrOppContactPersonList[sessionStorage.Contactindex].Tel = arrOppContactPersonTelInfoList;
                    arrOppContactPersonList[sessionStorage.Contactindex].Mobile = arrOppContactPersonMobileInfoList;
                    arrOppContactPersonList[sessionStorage.Contactindex].Email = arrOppContactPersonEmailInfoList;
                }

                AddOppContactPersonInfo();
                ContactPersonState = "";
            })
            .on('click', '#btnAddOppContactPersonTelephone', function () {
                ClearOppContactPersonTelInfo();
                $('#popOppContactPersonTelInfo').modal('show');
            })
            .on('click', '#btnCancelOppContactPersonTelephone', function () {
                $('#popOppContactPersonTelInfo').modal('hide');
                OppContactPersonTelState = '';
            })
            .on('click', '#btnOppContactPersonTelAreaCode', function () {
                BrowsePopup('LibraryAreaCodes', 'Area Codes',
                    [
                        {
                            DisplayText: [{ Column: 'Area' }, { Label: 'Code', Column: 'CallingCode' }],
                            Click: function (e) {
                                $('#txtOppContactPersonTelAreaCodeID').val(e.ID);
                                $('#txtOppContactPersonAreaCode').text(e.Area + ' - ' + e.CallingCode)
                            }
                        }
                    ]
                )
            })
            .on('keydown', '#txtOppContactPersonTelephoneNum', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('click', '#btnSaveOppContactPersonTelephone', function () {
                if ($('#txtOppContactPersonAreaCode').text() == "Select") {
                    WebToast('Area Code is required', 'top');
                    return false;
                }

                if ($('#txtOppContactPersonTelephoneNum').val() == "") {
                    WebToast('Please enter telephone number', 'top');
                    $('#spanerrorOppContactPersonTelephoneNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                //if ($('#txtOppContactPersonTelephoneNum').val().length != 7) {
                //    $('#spanerrorOppContactPersonTelephoneNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                //    return false;
                //}

                if ($('#selOppContactPersonTelType').val() == "") {
                    WebToast('Please select contact type', 'top');
                    $('#spanerrorOppContactPersonTelType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }
                intOppContactPersonTelInfoCtr = intOppContactPersonTelInfoCtr + 1;

                if (OppContactPersonTelState == "") {
                    arrOppContactPersonTelInfoList.push({
                        "TelRankNo": intOppContactPersonTelInfoCtr,
                        "CountryCodeID": $('#txtOppContactPersonTelCountryID').val(),
                        "CountryCode": $('#txtOppContactPersonCountryCode').text(),
                        "AreaCode": $('#txtOppContactPersonAreaCode').text(),
                        "AreaCodeID": $('#txtOppContactPersonTelAreaCodeID').val(),
                        "TelephoneNo": $('#txtOppContactPersonTelephoneNum').val(),
                        "ExtensionNo": $('#txtOppContactPersonTelephoneExt').val(),
                        "ContactType": $('#selOppContactPersonTelType').val(),
                        "TelState": "Add",
                        "IsNew": 1,
                        "TelID": $('#txtOppContactPersonTelID').val()
                    })
                }
                else {
                    arrOppContactPersonTelInfoList[sessionStorage.Telindex].TelRankNo = OppContactPersonTelRankNo;
                    arrOppContactPersonTelInfoList[sessionStorage.Telindex].CountryCodeID = $('#txtOppContactPersonTelCountryID').val();
                    arrOppContactPersonTelInfoList[sessionStorage.Telindex].CountryCode = $('#txtOppContactPersonCountryCode').text();
                    arrOppContactPersonTelInfoList[sessionStorage.Telindex].AreaCodeID = $('#txtOppContactPersonTelAreaCodeID').val();
                    arrOppContactPersonTelInfoList[sessionStorage.Telindex].AreaCode = $('#txtOppContactPersonAreaCode').text();
                    arrOppContactPersonTelInfoList[sessionStorage.Telindex].TelephoneNo = $('#txtOppContactPersonTelephoneNum').val();
                    arrOppContactPersonTelInfoList[sessionStorage.Telindex].ExtensionNo = $('#txtOppContactPersonTelephoneExt').val();
                    arrOppContactPersonTelInfoList[sessionStorage.Telindex].ContactType = $('#selOppContactPersonTelType').val();
                    arrOppContactPersonTelInfoList[sessionStorage.Telindex].TelState = sessionStorage.TelState;
                    arrOppContactPersonTelInfoList[sessionStorage.Telindex].IsNew = sessionStorage.TelIsNew;
                    arrOppContactPersonTelInfoList[sessionStorage.Telindex].TelID = $('#txtOppContactPersonTelID').val();
                }

                AddOppContactPersonTelInfo();
                ClearOppContactPersonTelInfo();
                OppContactPersonTelState = "";
            })
            .on('click', '#btnAddOppContactPersonMobile', function () {
                ClearOppContactPersonMobileInfo();
                $('#popOppContactPersonMobileInfo').modal('show');
            })
            .on('click', '#btnCancelOppContactPersonMobile', function () {
                $('#popOppContactPersonMobileInfo').modal('hide');
                OppContactPersonMobileState = '';
            })
            .on('click', '#btnOppContactPersonMobileTelCode', function () {
                BrowsePopup('LibraryTelcoPrefixes', 'Telco Codes',
                    [
                        {
                            DisplayText: [{ Column: 'Prefix' }, { Column: 'TelCo' }],
                            Click: function (e) {
                                $('#txtOppContactPersonTelCoPrefixID').val(e.ID);
                                $('#txtOppContactPersonTelCode').text(e.TelCo + ' - ' + e.Prefix)
                            }
                        }
                    ]
                )
            })
            .on('keydown', '#txtOppContactPersonMobileNo', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('click', '#btnSaveOppContactPersonMobile', function () {
                if ($('#txtOppContactPersonTelCode').text() == "Select") {
                    WebToast('Tel Code is required', 'top');
                    $('#spanerrorOppContactPersonMobileTelCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                if ($('#txtOppContactPersonMobileNo').val() == "") {
                    WebToast('Please enter mobile number', 'top');
                    $('#spanerrorOppContactPersonMobileNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                //if ($('#txtOppContactPersonMobileNo').val().length != 7) {
                //    $('#spanerrorOppContactPersonMobileNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                //    return false;
                //}

                if ($('#selOppContactPersonMobileType').val() == "") {
                    WebToast('Please select coontact type', 'top');
                    $('#spanerrorOppContactPersonMobileType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                intOppContactPersonMobileInfoCtr = intOppContactPersonMobileInfoCtr + 1;

                if (OppContactPersonMobileState == "") {
                    arrOppContactPersonMobileInfoList.push({
                        "MobileRankNo": intOppContactPersonMobileInfoCtr,
                        "CountryCodeID": $('#txtOppContactPersonMobileCountryID').val(),
                        "CountryCode": $('#txtOppContactPersonMobileCountryCode').text(),
                        "TelCode": $('#txtOppContactPersonTelCode').text(),
                        "TelCodeID": $('#txtOppContactPersonTelCoPrefixID').val(),
                        "MobileNo": $('#txtOppContactPersonMobileNo').val(),
                        "ContactType": $('#selOppContactPersonMobileType').val(),
                        "MobileID": $('#txtOppContactPersonMobileID').val(),
                        "MobileState": "Add",
                        "IsNew": 1
                    })
                }
                else {
                    arrOppContactPersonMobileInfoList[sessionStorage.Mobileindex].MobileRankNo = OppContactPersonMobileRankNo;
                    arrOppContactPersonMobileInfoList[sessionStorage.Mobileindex].CountryCodeID = $('#txtOppContactPersonMobileCountryID').val();
                    arrOppContactPersonMobileInfoList[sessionStorage.Mobileindex].CountryCode = $('#txtOppContactPersonMobileCountryCode').text();
                    arrOppContactPersonMobileInfoList[sessionStorage.Mobileindex].TelCodeID = $('#txtOppContactPersonTelCoPrefixID').val();
                    arrOppContactPersonMobileInfoList[sessionStorage.Mobileindex].TelCode = $('#txtOppContactPersonTelCode').text();
                    arrOppContactPersonMobileInfoList[sessionStorage.Mobileindex].MobileNo = $('#txtOppContactPersonMobileNo').val();
                    arrOppContactPersonMobileInfoList[sessionStorage.Mobileindex].ContactType = $('#selOppContactPersonMobileType').val();
                    arrOppContactPersonMobileInfoList[sessionStorage.Mobileindex].MobileState = sessionStorage.MobileState;
                    arrOppContactPersonMobileInfoList[sessionStorage.Mobileindex].IsNew = sessionStorage.MobileIsNew;
                }

                AddOppContactPersonMobileInfo();
                OppContactPersonMobileState = "";
                ClearOppContactPersonMobileInfo();
            })
            .on('click', '#btnAddOppContactPersonEmail', function () {
                ClearOppContactPersonEmailInfoFields();
                $('#popOppContactPersonEmailInfo').modal('show');
            })
            .on('click', '#btnCancelOppContactPersonEmailAddress', function () {
                $('#popOppContactPersonEmailInfo').modal('hide');
                OppContactPersonEmailState = '';
            })
            .on('click', '#btnSaveOppContactPersonEmailAddress', function () {
                if (IsEmail($.trim($("#txtOppContactPersonEmailAdd").val())) == false) {
                    WebToast('Please enter valid email address', 'top');
                    return false;
                }

                if ($.trim($("#txtOppContactPersonEmailAdd").val()) == "") {
                    WebToast('Please enter email address', 'top');
                    return false;
                }

                if ($('#selOppContactPersonEmailType').val() == "") {
                    WebToast('Please select email type', 'top');
                    return false;
                }

                intOppContactPersonEmailInfoCtr = intOppContactPersonEmailInfoCtr + 1;

                if (OppContactPersonEmailState == "") {
                    arrOppContactPersonEmailInfoList.push({
                        "EmailRankNo": intOppContactPersonEmailInfoCtr,
                        "EmailAddID": $('#txtOppContactPersonEmailID').val(),
                        "EmailAdd": $('#txtOppContactPersonEmailAdd').val(),
                        "EmailType": $('#selOppContactPersonEmailType').val(),
                        "EmailState": "Add",
                        "IsNew": 1,
                    })
                }
                else {
                    arrOppContactPersonEmailInfoList[sessionStorage.Emailindex].EmailRankNo = OppContactPersonEmailRankNo;
                    arrOppContactPersonEmailInfoList[sessionStorage.Emailindex].EmailAddID = $('#txtOppContactPersonEmailID').val();
                    arrOppContactPersonEmailInfoList[sessionStorage.Emailindex].EmailAdd = $('#txtOppContactPersonEmailAdd').val();
                    arrOppContactPersonEmailInfoList[sessionStorage.Emailindex].EmailType = $('#selOppContactPersonEmailType').val();
                    arrOppContactPersonEmailInfoList[sessionStorage.Emailindex].EmailState = sessionStorage.EmailState;
                    arrOppContactPersonEmailInfoList[sessionStorage.Emailindex].IsNew = sessionStorage.EmailIsNew;
                }

                AddOppContactPersonEmailInfo();
                OppContactPersonEmailState = "";
            })

            .on('click', '#btnSaveOpp', function () {
                SaveOpportunity();
            })
            .on('click', '#btnOppAddComment', function () {
                $('#popOppNewComment').modal('show');
                $('#txtOppNewComment').empty();
                placeCaretAtEnd($('#txtOppNewComment')[0]);
            })
            .on('click', '#btnSaveOppComment', function () {
                //if ($.trim($('#txtOppNewComment').val()) == "") {
                //    WebToast('Please type your comment', 'top');
                //    return;
                //}

                $('#txtOppNewComment').find('img').removeAttr('src').attr('alt', 'Saving image...');

                var noOfPics = $('#txtOppNewComment').find('.crmpic.new').length
                var hasPics = (noOfPics > 0 ? true : false);
                $('#btnSaveOppComment').prop('disabled', true);
                NewMessageBox("Please wait while saving ...", "Message");
                AjaxSend({
                    url: 'CRMDashboard/SaveOppComment',
                    data: { Comment: HTMLEncode($('#txtOppNewComment').html()), Date: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a'), ClientID: oppClientID, OppID: $('#txtOpportunityID').val() },
                    success: function (vData) {

                        if (hasPics) {
                            var ctrSaving = 0;
                            NewMessageBox("Please wait while uploading attachment(s) ...", "Message");
                            $('#txtOppNewComment').find('.crmpic.new').each(function () {
                                var formdatas = new FormData();
                                formdatas.append('files', $(this).data('file'));

                                $.ajax({
                                    url: '/CRMDashboard/SaveAttachFile?OrgID=' + sessionStorage.OrgID + '&RelatedID=' + vData.Results + '&ImgID=' + $(this).attr('imgid'),
                                    type: 'POST',
                                    data: formdatas,
                                    async: true,
                                    cache: false,
                                    contentType: false,
                                    processData: false,
                                    success: function (e) {
                                        ctrSaving += 1;

                                        if (e == "saved") {
                                            if (noOfPics == ctrSaving) {
                                                ConfirmationOfSavingOppComment(vData)
                                            }
                                        } else {
                                            NewMessageBox(e, 'Message');
                                        }
                                    }
                                });
                            })
                        } else {
                            ConfirmationOfSavingOppComment(vData)
                        }
                    }
                })

                function ConfirmationOfSavingOppComment(vData) {
                    if (IsGuid(vData.Results)) {
                        $('#popOppNewComment').modal('hide');
                        GetOppComments();
                        NewMessageBoxClose();
                    } else {
                        NewMessageBox(vData.Results, 'Message');
                    }
                    $('#btnSaveOppComment').removeAttr('disabled');
                }
            })
            .on('change', '#selOpportunityFunnelTemplate', function () {
                if ($(this).val() != templateID) {
                    $('#listOppTemplateOnChange').show();
                    $('#listOppTemplate').hide();
                    DisplayStages($(this).children('option:selected').data('template'), true)
                } else {
                    $('#listOppTemplateOnChange').hide();
                    $('#listOppTemplate').show();
                }
            })

            .on('click', '.btnOppAttachImage', function () {
                $('#fileOppPic').click();
            })
            .on('submit', '#formOppPic', function (e) {
                e.preventDefault();
                var formData = new FormData($(this)[0]);

                var reader = new FileReader();

                reader.onload = function (e) {
                    //setCaretPosition(caretPos.row, caretPos.col)
                    var file;
                    var imgID = NewID();

                    $('#fileOppPic')[0].files[0].name = imgID + '_' + $('#fileOppPic')[0].files[0].name;
                    placeCaretAtEnd($('#txtOppNewComment')[0])
                    insertAtCursor('img', 'img', e.target.result, imgID, 'uploading', $('#fileOppPic')[0].files[0])
                    placeCaretAtEnd($('#txtOppNewComment')[0])
                };
                reader.readAsDataURL($('#fileOppPic')[0].files[0]);
            })
            .on('change', '#fileOppPic', function (e) {
                e.preventDefault();
                if ($(this).val() != "") {
                    $('#formOppPic').submit()
                }
            })
            .on('click', '#fileOppPic', function (e) {
                caretPos = getCaretPosition();
            })

            .on('click', '#btnDashboardAddOpp', function () {
                $('#divOppClientList').show();
                $('#btnAddOpportunity').click();
                sessionStorage.AddOppViaDashboard = true;
            })

            function GetOppComments(id) {
                var listOppComments = $('#listOppComments');
                listOppComments.empty();
                AjaxSend({
                    url: 'CRMDashboard/GetOppComments',
                    data: { ClientID: oppClientID, OppID: $('#txtOpportunityID').val() },
                    success: function (vData) {
                        var vRows = vData.data

                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                listOppComments.append($('<li class="list-group-item">')
                                    .append($('<div style="color:#333 !important">')
                                        .append($('<div style="font-size:12px; color:#757575; float:left;">')
                                            .html(vRows[i].ActionName)
                                        )
                                        .append($('<div style="font-size:12px; color:#757575; float:right;">')
                                            .html('By ' + vRows[i].CommentedBy + ' on ' + $.format.date(new Date(vRows[i].CommentedDate), 'MMM-dd-yyyy hh:mm a'))
                                        )
                                        .append('<br>' + HTMLDecode(vRows[i].Comment))
                                    )
                                )

                                LoadAttachments(vRows[i].Attachments);
                            }
                        }
                    }
                })
            }

            function GetOpportunity() {
                if ($.fn.DataTable.isDataTable('#dtOpportunity')) {
                    dtOpportunity.draw();
                    return
                }

                $.fn.dataTable.ext.errMode = 'none'; // hide jDatatable error popup

                dtOpportunity = $('#dtOpportunity').DataTable({
                    retrieve: true,
                    "order": [[1, "asc"]],
                    "pageLength": 10,
                    "bProcessing": true,
                    "bServerSide": true,
                    "ajax": {
                        url: "/CRMDashboard/GetOpportunity?OrgID=" + sessionStorage.OrgID,
                        type: 'POST',
                        error: function (e) {
                            NewMessageBox('ERROR: ' + e.responseText, 'Message')
                        }
                    },
                    "autoWidth": false,
                    "columns": [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Client Name</b>' + data.ClientName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Total</b>' + data.NoOfOpportunity;
                            },
                            "width": "200px", className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Won</b>' + (CInt2(data.Won) == 0 ? 0 : AccountingFormat(data.Won)) + ' (' + CDec2(Math.round((data.Won / data.Total) * 100)) + '%)';
                            },
                            "width": "100px", className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Lost</b>' + (CInt2(data.Lost) == 0 ? 0 : AccountingFormat(data.Lost)) + ' (' + CDec2(Math.round((data.Lost / data.Total) * 100)) + '%)';
                            },
                            "width": "100px", className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">In Progress</b>' + (CInt2(data.InProgress) == 0 ? 0 : AccountingFormat(data.InProgress)) + ' (' + CDec2(Math.round((data.InProgress / data.Total) * 100)) + '%)';
                            },
                            "width": "110px", className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Total</b>' + (CInt2(data.Total) == 0 ? 0 : AccountingFormat(data.Total)) + ' (' + CDec2(((data.Total / data.Total) * 100)) + '%)';
                            },
                            "width": "100px", className: 'text-center'
                        }
                    ],
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                    "responsive": true,
                    "columnDefs": [{ "targets": 0, "orderable": true }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">',
                    initComplete: function () {
                        $('#dtOpportunity').removeClass('dataTable');
                    }
                    //"stateSave": true
                })

                $('#dtOpportunity tbody').on('click', 'tr', function () {
                    if (dtOpportunity.row(this).data() != undefined) {
                        isByClient = false;
                        oppClientID = dtOpportunity.row(this).data().ClientID;
                        ClearOpportunityFields();
                        GetOpportunityByClient(dtOpportunity.row(this).data().ClientID);
                        $('#lblOppClientName').text(dtOpportunity.row(this).data().ClientName);
                        $('#popupOpportunity').modal('show');
                        $('#btnTabOppComment, #divOppStatus').show();

                        $('#txtAbsAmtWon, #txtAbsAmtLost, #txtAbsAmtInProgress, #txtAbsAmtTotal').text('0');

                        $('#txtContributionWon, #txtContributionLost, #txtContributionInProgress, #txtContributionTotal').text('0%');
                    }
                });

                $("#tabCrmOpportunities #txtSearchOpportunity").unbind().keypress(function (e) {
                    if (e.keyCode == 13) {
                        dtOpportunity.search($.trim($(this).val())).draw();
                    }
                });
            }

            var dtOpportunityByClient;
            function GetOpportunityByClient(id) {
                var url = '/CRMDashboard/GetOpportunityByClient?OrgID=' + sessionStorage.OrgID + '&ID=' + id;
                var tbodyClientOpportunity;
                if (isByClient) {
                    tbodyClientOpportunity = '#tblDataTableClientOpportunity';
                } else {
                    tbodyClientOpportunity = '#tblDataTableClientOpportunityPopup';
                }
                $('#divOppClientList').hide();
                sessionStorage.AddOppViaDashboard = false;
                //tbodyClientOpportunity.empty().append('<tr><td colspan="5" style="text-align:center;">Loading ...</td></tr>');
                if ($.fn.DataTable.isDataTable(tbodyClientOpportunity)) {
                    dtOpportunityByClient = $(tbodyClientOpportunity).DataTable().ajax.url(url).load(function (json) {
                        OppinitComplete(json);
                    });
                    return;
                }

                dtOpportunityByClient = $(tbodyClientOpportunity).DataTable({
                    retrieve: true,
                    "order": [[1, "asc"]],
                    "pageLength": 10,
                    "bProcessing": true,
                    "bServerSide": true,
                    "ajax": {
                        url: url,
                        type: 'POST',
                        error: function (e) {
                            NewMessageBox('ERROR: ' + e.responseText, 'Message')
                        }
                    },
                    "autoWidth": false,
                    "columns": [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Name</b>' + data.Name;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Current Stage</b>' + data.CurrentStage;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Status</b>' + data.Status;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Size</b>' + (CInt2(data.Size) == 0 ? 0 : AccountingFormat(data.Size));
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Last Updated</b>' + $.format.date(new Date(data.LastUpdated), 'MMM dd, yyyy');
                            }, width: '150px'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Action</b>' +
                                    '<button data-role="none" title="Move to Bin" class="btn btn-danger"><span class="bi-trash"></span></button>'
                            }, width: '100px', className: 'text-center'
                        }
                    ],
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": [0, 5, 6] }],
                    "responsive": true,
                    "columnDefs": [{ "targets": 0, "orderable": true }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">',
                    "initComplete": function (settings, json) {
                        $('#tblDataTableClientOpportunityPopup').removeClass('dataTable');
                        OppinitComplete(json);
                    }
                    //"stateSave": true
                })

                dtOpportunityByClient.on('click', 'td', function () {
                    if (dtOpportunityByClient.row(this).data() != undefined) {
                        var data = dtOpportunityByClient.row(this).data();

                        if (dtOpportunityByClient.cell(this).index().columnVisible == 6) {
                            PromptUserAccess('Deactivate Opportunity', {
                                OnClickOKSuccess: function () {
                                    RemoveOpportunity(data);
                                }
                            })
                            return;
                        }
                        PromptUserAccess('View Opportunity', {
                            OnClickOKSuccess: function () {
                                ClearOpportunityFields();
                                $('#txtOpportunityID').val(data.ID);
                                $('#txtOpportunityName').val(data.Name);
                                $('#txtOpportunityDescription').val(data.Description);
                                $('#txtOpportunitySize').val(data.Size);
                                $('#txtOpportunitySizePeriod').val(data.Period);
                                $('[name="radOppStatus"][value="' + data.Status + '"]').prop('checked', true);//.checkboxradio('refresh');
                                arrOppContactPersonList = data.ContactPerson;
                                parentOppID = data.ParentOppID;
                                isAddOpportunity = false;
                                AddOppContactPersonInfo();
                                GetOppComments();
                                DisplayStages(data.Template, false);
                                CheckStaging(data.Checklist);
                                listOppTemplate = $('#listOppTemplate');
                                $('#listOppTemplateOnChange').hide();
                                $('#btnTabOppComment, #divOppStatus, #listOppTemplate').show();
                                $('#popupNewOpportunity').modal('show');

                                if (data.Status == 'Win') {
                                    $('#txtOpportunityName, #txtOpportunityDescription, #txtOpportunitySize, #txtOpportunitySizePeriod');//.textinput('disable');
                                    $('[name="radOppStatus"]');//.checkboxradio('disable');
                                } else {
                                    $('#txtOpportunityName, #txtOpportunityDescription, #txtOpportunitySize, #txtOpportunitySizePeriod');//.textinput('enable');
                                    $('[name="radOppStatus"]');//.checkboxradio('enable');
                                }
                            }
                        })
                    }
                });

                $("#tabCrmTicketService #txtSearchTicket").unbind().keypress(function (e) {
                    if (e.keyCode == 13) {
                        tblDataTableTickets.search($.trim($(this).val())).draw();
                    }
                });

                function OppinitComplete(json) {
                    var data = json.data;
                    var won = 0, lost = 0, inprogress = 0, total = 0;

                    for (i = 0; i < data.length; i++) {
                        if (data[i].Status == 'In Progress') {
                            inprogress += CDec2(data[i].Size);
                        } else if (data[i].Status == 'Win') {
                            won += CDec2(data[i].Size);
                        } else if (data[i].Status == 'Lost') {
                            lost += CDec2(data[i].Size);
                        }

                        total += CDec2(data[i].Size);
                    }

                    $('#txtAbsAmtWon').text(AccountingFormat(won));
                    $('#txtAbsAmtLost').text(AccountingFormat(lost));
                    $('#txtAbsAmtInProgress').text(AccountingFormat(inprogress));
                    $('#txtAbsAmtTotal').text(AccountingFormat(total));

                    $('#txtContributionWon').text(CDec2(Math.round((won / total) * 100)) + '%');
                    $('#txtContributionLost').text(CDec2(Math.round((lost / total) * 100)) + '%');
                    $('#txtContributionInProgress').text(CDec2(Math.round((inprogress / total) * 100)) + '%');
                    $('#txtContributionTotal').text(CDec2((total / total) * 100) + '%');
                }
            }

            function RemoveOpportunity(data) {
                data.ActionName = "Opportunity";
                NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                        {
                            Text: 'Yes', Click: function () {
                                AjaxSend({
                                    url: '/CRMDashboard/RemoveAction',
                                    data: data,
                                    success: function (vData) {
                                        if (vData.Results == "saved") {
                                            WebToast('Successfully moved to bin', 'top');
                                            GetOpportunityByClient(oppClientID);
                                        } else {
                                            NewMessageBox(vData.Results, 'Message');
                                        }
                                    }
                                })
                            }
                        }, { Text: 'No' }
                ])
            }

            function DisplayStages(data, onchange) {
                if (CStr2(data) == "") {
                    return
                }

                if (onchange) {
                    listOppTemplate = $('#listOppTemplateOnChange');
                } else {
                    listOppTemplate = $('#listOppTemplate');
                    templateID = data.ID;
                }

                listOppTemplate.empty();

                $('#selOpportunityFunnelTemplate').val(data.ID);//.selectmenu('refresh');

                for (var i = 0; i < data.Stages.length; i++) {
                    var dStage = data.Stages[i];
                    var child = $('<ul class="list-group" style="margin-left:20px;">');

                    if (dStage.Child.length > 0) {
                        for (var c = 0; c < dStage.Child.length; c++) {
                            var dChild = dStage.Child[c];

                            child.append($('<li class="list-group-item">')
                                    .append($('<div class="form-check">')
                                        .append($('<label for="chb-' + dChild.SpecialID + '" class="form-check-label">')
                                            .append($('<input type="checkbox" name="oppStage" id="chb-' + dChild.SpecialID + '" data-role="none" class="form-check-input">')
                                                .data('data', dChild)
                                            ).append(dChild.Name)
                                        )
                                        .append($('<div>'))
                                    )
                                )
                        }

                        listOppTemplate.append($('<li class="list-group-item">')
                                .text(dStage.Name)
                                .append(child)
                            )
                    } else {
                        listOppTemplate.append($('<li class="list-group-item">')
                                    .append($('<div class="form-check">')
                                        .append($('<label for="chb-' + dStage.SpecialID + '" class="form-check-label">')
                                            .append($('<input type="checkbox" name="oppStage" id="chb-' + dStage.SpecialID + '" data-role="none" class="form-check-input">')
                                                .data('data', dStage)
                                            ).append(dStage.Name)
                                        )
                                    )
                                )
                    }

                    listOppTemplate.find('[name="oppStage"]');//.checkboxradio();//.checkboxradio('refresh');
                }
            }

            function CheckStaging(data) {
                if (CStr2(data) == "") {
                    return;
                }

                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        $('#chb-' + data[i].DescriptionID).prop('checked', true);//.checkboxradio('refresh');
                    }
                }
            }

            function SaveOpportunity() {
                var txtOpportunityID = $.trim($('#txtOpportunityID').val());
                var txtOpportunityName = $.trim($('#txtOpportunityName').val());
                var txtOpportunityDescription = $.trim($('#txtOpportunityDescription').val());
                var txtOpportunitySize = $.trim($('#txtOpportunitySize').val());
                var txtOpportunitySizePeriod = $.trim($('#txtOpportunitySizePeriod').val());
                var checklist = [];

                if (txtOpportunityName == '') {
                    WebToast('Please enter name.', 'top');
                    return;
                }

                if (listOppTemplate.find('> li').length > 0) {
                    listOppTemplate.find('> li').each(function () {
                        var dis = $(this);
                        if (dis.find('> ul').length > 0) {
                            var sublist = dis.find('> ul')

                            sublist.find('> li').each(function () {
                                if ($(this).find('input[type="checkbox"]').is(':checked')) {
                                    checklist.push($(this).find('input[type="checkbox"]').data('data'))
                                }
                            })
                        } else {
                            if (dis.find('input[type="checkbox"]').is(':checked')) {
                                checklist.push(dis.find('input[type="checkbox"]').data('data'))
                            }
                        }
                    })
                }

                if (CBool(sessionStorage.AddOppViaDashboard)) {
                    oppClientID = $('#selOppClientName').val();
                    if (oppClientID == "") {
                        WebToast('Please select client name.', 'top');
                        return;
                    }
                }

                var pData = {
                    ClientID: (CBool(sessionStorage.AddOppViaDashboard) ? oppClientID : (isByClient ? JSON.parse(sessionStorage.ClientInfo).ID : oppClientID)),
                    ID: txtOpportunityID,
                    Name: txtOpportunityName,
                    Description: txtOpportunityDescription,
                    Size: txtOpportunitySize,
                    Period: txtOpportunitySizePeriod,
                    Status: ($('[name="radOppStatus"]:checked').val() || 'In Progress'),
                    ContactPerson: arrOppContactPersonList,
                    ContactPersonTelphone: arrOppContactPersonTelInfoList,
                    ContactPersonMobile: arrOppContactPersonMobileInfoList,
                    ContactPersonEmail: arrOppContactPersonEmailInfoList,
                    OrgID: sessionStorage.OrgID,
                    Template: $('#selOpportunityFunnelTemplate').val(),
                    Checklist: checklist,
                    DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm:ss a'),
                    ParentOppID: CStr2(parentOppID)
                }

                $('#popupNewOpportunity').modal('hide');

                if (pData.Status != 'In Progress') {
                    NewMessageBox('Marking this opportunity as "' + pData.Status + '" will close this opportunity. ' +
                            'You will not be able to edit this opportunity after saving. ' +
                            'However, you can still add comments to this opportunity in the future.<br><br> ' +
                            'Are you sure you want to mark this opportunity as ' + pData.Status + '?', 'Message',
                            [{
                                Text: 'Yes', Click: function () {
                                    ValidateOpportunity();
                                }
                            }, { Text: 'No' }])
                } else {
                    ValidateOpportunity()
                }

                function ValidateOpportunity() {
                    if (!isAddOpportunity) {
                        PromptUserAccess('Edit Opportunity', {
                            OnClickOKSuccess: function () {
                                SaveOppNow();
                            }
                        })
                    } else {
                        SaveOppNow();
                    }
                }

                function SaveOppNow() {
                    AjaxSend({
                        url: 'CRMDashboard/SaveOpportunity',
                        data: pData,
                        success: function (vData) {
                            if (vData.Results == "saved") {
                                WebToast('Successfully Saved', 'top');
                                ClearOpportunityFields();
                                GetOpportunityByClient(pData.ClientID);
                            } else {
                                NewMessageBox(vData.Results, 'Message');
                            }
                        }
                    })
                }
            }

            function AddOppContactPersonEmailInfo() {

                var pLV = $('#lstOppContactPersonEmail');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrOppContactPersonEmailInfoList.length; i++) {
                    if (arrOppContactPersonEmailInfoList[i].EmailState != "Remove") {
                        arrOppContactPersonEmailInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex">')
                                .data(arrOppContactPersonEmailInfoList[i])
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        .data(arrOppContactPersonEmailInfoList[i])
                                        .html($.trim(arrOppContactPersonEmailInfoList[i].EmailAdd) +
                                            "<br>" + $.trim(arrOppContactPersonEmailInfoList[i].EmailType)
                                        )
                                        .click(function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            EditOppContactPersonEmailInfo($(this).data().arrindex);
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
                                        .data('data', arrOppContactPersonEmailInfoList[i])
                                        .click(function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            var EmailRankNo = $(this).data('data').EmailRankNo;

                                            if ($(this).data('data').IsNew == 1) {
                                                //remove selected email
                                                arrOppContactPersonEmailInfoList = $.grep(arrOppContactPersonEmailInfoList, function (val) {
                                                    return val.EmailRankNo != EmailRankNo;
                                                })
                                            }
                                            else {
                                                arrOppContactPersonEmailInfoList[$(this).data('data').arrindex].EmailState = "Remove";
                                            }

                                            AddOppContactPersonEmailInfo();
                                        })
                                )

                        pLV.append(pItem);
                    }
                }

                pLV;//.listview('refresh');
                $('#popOppContactPersonEmailInfo').modal('hide');
            }

            function EditOppContactPersonEmailInfo(arrindex) {
                $('#popOppContactPersonEmailInfo').modal('show');
                OppContactPersonEmailRankNo = arrOppContactPersonEmailInfoList[arrindex].EmailRankNo;
                OppContactPersonEmailState = "Edit";
                sessionStorage["Emailindex"] = arrindex;
                sessionStorage["EmailIsNew"] = arrOppContactPersonEmailInfoList[arrindex].IsNew;
                sessionStorage["EmailState"] = arrOppContactPersonEmailInfoList[arrindex].EmailState;
                $('#txContacttPersonEmailID').val(arrOppContactPersonEmailInfoList[arrindex].EmailAddID == undefined ? "" : arrOppContactPersonEmailInfoList[arrindex].EmailAddID);
                $('#txtOppContactPersonEmailAdd').val(arrOppContactPersonEmailInfoList[arrindex].EmailAdd);
                $('#selOppContactPersonEmailType').val(arrOppContactPersonEmailInfoList[arrindex].EmailType);//.selectmenu('refresh');
            }

            function ClearOppContactPersonEmailInfoFields() {
                $('#txtOppContactPersonEmailID').val('');
                $('#txtOppContactPersonEmailAdd').val('');
                $('#selOppContactPersonEmailType').val('');//.selectmenu('refresh');
                $('#divOppContactPersonEmailHome').pageshow();
            }

            function AddOppContactPersonMobileInfo() {

                var pLV = $('#lstOppContactPersonMobile');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrOppContactPersonMobileInfoList.length; i++) {
                    if (arrOppContactPersonMobileInfoList[i].MobileState != "Remove") {
                        arrOppContactPersonMobileInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex">')
                                    .data(arrOppContactPersonMobileInfoList[i])
                                    .append($('<p class="p-0 m-0 flex-grow-1">')
                                            .attr('ID', arrOppContactPersonMobileInfoList[i].id)
                                            .html("+" + $.trim(arrOppContactPersonMobileInfoList[i].CountryCode.split('-')[1]) + " (" + $.trim(arrOppContactPersonMobileInfoList[i].TelCode.split('-')[1]) + ") " + $.trim(arrOppContactPersonMobileInfoList[i].MobileNo) +
                                                "<br>" + $.trim(arrOppContactPersonMobileInfoList[i].TelCode.split('-')[0]) + " (" + $.trim(arrOppContactPersonMobileInfoList[i].CountryCode.split('-')[0]) + ") " +
                                                "<br>" + $.trim(arrOppContactPersonMobileInfoList[i].ContactType)
                                            )
                                            .data(arrOppContactPersonMobileInfoList[i])
                                            .click(function (e) {
                                                e.preventDefault();
                                                e.stopPropagation();
                                                EditOppContactPersonMobileInfo($(this).data().arrindex);
                                            })
                                    )
                                    .append($('<button data-role="none" class="btn btn-danger">')
                                            .append('<span class="bi-trash"></span>')
                                            .data('data', arrOppContactPersonMobileInfoList[i])
                                            .click(function (e) {
                                                e.preventDefault();
                                                e.stopPropagation();
                                                var MobileRankNo = $(this).data('data').MobileRankNo;

                                                if ($(this).data('data').IsNew == 1) {
                                                    //remove selected mobile
                                                    arrOppContactPersonMobileInfoList = $.grep(arrOppContactPersonMobileInfoList, function (val) {
                                                        return val.MobileRankNo != MobileRankNo;
                                                    })
                                                }
                                                else {
                                                    arrOppContactPersonMobileInfoList[$(this).data('data').arrindex].MobileState = "Remove";
                                                }

                                                AddOppContactPersonMobileInfo();
                                            })
                                    )

                        pLV.append(pItem);
                    }
                }

                pLV;//.listview('refresh');
                $('#popOppContactPersonMobileInfo').modal('hide');
            }

            function EditOppContactPersonMobileInfo(arrindex) {
                $('#popOppContactPersonMobileInfo').modal('show');
                OppContactPersonMobileRankNo = arrOppContactPersonMobileInfoList[arrindex].MobileRankNo;
                OppContactPersonMobileState = "Edit";
                sessionStorage["Mobileindex"] = arrindex;
                sessionStorage["MobileIsNew"] = arrOppContactPersonMobileInfoList[arrindex].IsNew;
                sessionStorage["MobileState"] = arrOppContactPersonMobileInfoList[arrindex].MobileState;
                $('#txtOppContactPersonMobileID').val(arrOppContactPersonMobileInfoList[arrindex].MobileID == undefined ? "" : arrOppContactPersonMobileInfoList[arrindex].MobileID);
                $('#txtOppContactPersonTelCoPrefixID').val(arrOppContactPersonMobileInfoList[arrindex].TelCodeID);
                $('#txtOppContactPersonMobileCountryID').val(arrOppContactPersonMobileInfoList[arrindex].CountryCodeID);
                $('#txtOppContactPersonMobileCountryCode').text(arrOppContactPersonMobileInfoList[arrindex].CountryCode);
                $('#txtOppContactPersonTelCode').text(arrOppContactPersonMobileInfoList[arrindex].TelCode);
                $('#txtOppContactPersonMobileNo').val(arrOppContactPersonMobileInfoList[arrindex].MobileNo);
                $('#selOppContactPersonMobileType').val(arrOppContactPersonMobileInfoList[arrindex].ContactType);//.selectmenu('refresh');
            }

            function ClearOppContactPersonMobileInfo() {
                $('#txtOppContactPersonMobileID').val('');
                $('#txtOppContactPersonTelCoPrefixID').val('');
                $('#txtOppContactPersonMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtOppContactPersonMobileCountryCode').text('PHL - 63');
                $('#txtOppContactPersonTelCode').text('Select');
                $('#txtOppContactPersonMobileNo').val('');
                $('#selOppContactPersonMobileType').val('');//.selectmenu('refresh');
                $('#divOppContactPersonMobileHome').pageshow();
            }

            function AddOppContactPersonTelInfo() {

                var pLV = $('#lstOppContactPersonTelephone');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrOppContactPersonTelInfoList.length; i++) {
                    if (arrOppContactPersonTelInfoList[i].TelState != "Remove") {
                        arrOppContactPersonTelInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex">')
                                .data(arrOppContactPersonTelInfoList[i])
                                .append($('<p class="p-0 m-0 flex-grow-1">')
                                        .attr('ID', arrOppContactPersonTelInfoList[i].id)
                                        .html("+" + $.trim(arrOppContactPersonTelInfoList[i].CountryCode.split('-')[1]) + " (" + $.trim(arrOppContactPersonTelInfoList[i].AreaCode.split('-')[1]) + ") " + $.trim(arrOppContactPersonTelInfoList[i].TelephoneNo) + ($.trim(arrOppContactPersonTelInfoList[i].ExtensionNo) == "" ? "" : "." + $.trim(arrOppContactPersonTelInfoList[i].ExtensionNo)) +
                                            "<br>" + $.trim(arrOppContactPersonTelInfoList[i].AreaCode.split('-')[0]) + ", " + $.trim(arrOppContactPersonTelInfoList[i].CountryCode.split('-')[0]) +
                                            "<br>" + $.trim(arrOppContactPersonTelInfoList[i].ContactType)
                                        )
                                        .data('data', arrOppContactPersonTelInfoList[i])
                                        .click(function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            EditOppContactPersonTelInfo($(this).data('data').arrindex);
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
                                        .data('data', arrOppContactPersonTelInfoList[i])
                                        .click(function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            var TelRankNo = $(this).data('data').TelRankNo;

                                            if ($(this).data('data').IsNew == 1) {
                                                //remove selected telephone
                                                arrOppContactPersonTelInfoList = $.grep(arrOppContactPersonTelInfoList, function (val) {
                                                    return val.TelRankNo != TelRankNo;
                                                })
                                            }
                                            else {
                                                arrOppContactPersonTelInfoList[$(this).data('data').arrindex].TelState = "Remove";
                                            }

                                            AddOppContactPersonTelInfo();
                                        })
                                )

                        pLV.append(pItem);
                    }

                }

                pLV;//.listview('refresh');
                $('#popOppContactPersonTelInfo').modal('hide');
            }

            function EditOppContactPersonTelInfo(arrindex) {
                $('#popOppContactPersonTelInfo').modal('show');

                OppContactPersonTelRankNo = arrOppContactPersonTelInfoList[arrindex].TelRankNo;
                OppContactPersonTelState = "Edit";
                sessionStorage["Telindex"] = arrindex;
                sessionStorage["TelIsNew"] = arrOppContactPersonTelInfoList[arrindex].IsNew;
                sessionStorage["TelState"] = arrOppContactPersonTelInfoList[arrindex].TelState;
                $('#txtOppContactPersonTelID').val(arrOppContactPersonTelInfoList[arrindex].TelID == undefined ? "" : arrOppContactPersonTelInfoList[arrindex].TelID);
                $('#txtOppContactPersonTelAreaCodeID').val(arrOppContactPersonTelInfoList[arrindex].AreaCodeID);
                $('#txtOppContactPersonTelCountryID').val(arrOppContactPersonTelInfoList[arrindex].CountryCodeID);
                $('#txtOppContactPersonCountryCode').text(arrOppContactPersonTelInfoList[arrindex].CountryCode);
                $('#txtOppContactPersonAreaCode').text(arrOppContactPersonTelInfoList[arrindex].AreaCode);
                $('#txtOppContactPersonTelephoneNum').val(arrOppContactPersonTelInfoList[arrindex].TelephoneNo);
                $('#txtOppContactPersonTelephoneExt').val(arrOppContactPersonTelInfoList[arrindex].ExtensionNo);
                $('#selOppContactPersonTelType').val(arrOppContactPersonTelInfoList[arrindex].ContactType);//.selectmenu('refresh');
            }

            function ClearOppContactPersonTelInfo() {
                $('#txtOppContactPersonTelID').val('');
                $('#txtOppContactPersonTelAreaCodeID').val('');
                $('#txtOppContactPersonTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtOppContactPersonCountryCode').text('PHL - 63');
                $('#txtOppContactPersonAreaCode').text('Select');
                $('#txtOppContactPersonTelephoneNum').val('');
                $('#txtOppContactPersonTelephoneExt').val('');
                $('#selOppContactPersonTelType').val('');//.selectmenu('refresh');
                $('#divOppContactPersonTel').pageshow();
            }

            function AddOppContactPersonInfo() {
                var pLV = $('#lstOppContactPerson');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrOppContactPersonList.length; i++) {
                    if (arrOppContactPersonList[i].ContactPersonState != "Remove") {
                        arrOppContactPersonList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex">')
                                .data(arrOppContactPersonList[i])
                                .append($('<p class="p-0 m-0 flex-grow-1">')
                                        .data(arrOppContactPersonList[i])
                                        .html($.trim(arrOppContactPersonList[i].Firstname) + ' ' +
                                            $.trim(arrOppContactPersonList[i].Middlename) + ' ' +
                                            $.trim(arrOppContactPersonList[i].Lastname) +
                                            ($.trim(arrOppContactPersonList[i].Position) == '' ? '' : '<br> Position: ' + $.trim(arrOppContactPersonList[i].Position))
                                        )
                                        .click(function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            EditOppContactPersonInfo($(this).data().arrindex);
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
                                        .data('data', arrOppContactPersonList[i])
                                        .click(function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            var OppContactPersonRankNo = $(this).data('data').OppContactPersonRankNo;

                                            if ($(this).data('data').IsNew == 1) {
                                                //remove selected person
                                                arrOppContactPersonList = $.grep(arrOppContactPersonList, function (val) {
                                                    return val.OppContactPersonRankNo != OppContactPersonRankNo;
                                                })
                                            }
                                            else {
                                                arrOppContactPersonList[$(this).data('data').arrindex].ContactPersonState = "Remove";
                                            }

                                            AddOppContactPersonInfo();
                                        })
                                )

                        pLV.append(pItem);
                    }
                }

                pLV;//.listview('refresh');
                $('#popOppContactPersonProfile').modal('hide');

                ClearOppContactPersonFields();
            }

            function EditOppContactPersonInfo(arrindex) {
                ClearOppContactPersonFields();

                $('#popOppContactPersonProfile').modal('show');
                OppContactPersonRankNo = arrOppContactPersonList[arrindex].OppContactPersonRankNo;
                ContactPersonState = "Edit";
                sessionStorage["Contactindex"] = arrindex;
                sessionStorage["OppContactPersonIsNew"] = arrOppContactPersonList[arrindex].IsNew;
                sessionStorage["ContactPersonState"] = arrOppContactPersonList[arrindex].ContactPersonState;
                $('#txtOppContactPersonID').val(arrOppContactPersonList[arrindex].ID == undefined ? "" : arrOppContactPersonList[arrindex].ID);
                $('#txtPeopleID').val(arrOppContactPersonList[arrindex].PeopleID == undefined ? "" : arrOppContactPersonList[arrindex].PeopleID);
                $('#txtOppContactFirstName').val(arrOppContactPersonList[arrindex].Firstname);
                $('#txtOppContactMiddleName').val(arrOppContactPersonList[arrindex].Middlename);
                $('#txtOppContactLastName').val(arrOppContactPersonList[arrindex].Lastname);
                $('#txtOppContactPersonPosition').val(arrOppContactPersonList[arrindex].Position);
                $('#txtOppContactPersonPositionDescription').val(arrOppContactPersonList[arrindex].PositionDescription);
                $('#txtOppContactPersonAddressID').val(arrOppContactPersonList[arrindex].AddressID);
                $('#txtOppContactPersonHouseBuildingNo').val(arrOppContactPersonList[arrindex].HouseBuildingNo);
                $('#txtOppContactPersonStreet').val(arrOppContactPersonList[arrindex].Street);
                $('#selOppContactPersonCity').val(arrOppContactPersonList[arrindex].City);//.selectmenu('refresh');
                $('#selOppContactPersonCountry').val((arrOppContactPersonList[arrindex].Country || 'e783ed13-aa98-4cc1-918e-d49b05c0167e'));//.selectmenu('refresh');
                $('#selOppContactPersonTypeAddress').val(arrOppContactPersonList[arrindex].TypeAddress);//.selectmenu('refresh');

                if (arrOppContactPersonList[arrindex].IsNew == "0") {
                    if (arrOppContactPersonList[arrindex].FromServer == undefined) {
                        AjaxSend({
                            url: 'CRMDashboard/GetContactDetailsPeople',
                            data: { ID: arrOppContactPersonList[arrindex].PeopleID },
                            success: function (vData) {
                                arrOppContactPersonList[arrindex].FromServer = true;
                                arrOppContactPersonTelInfoList = (vData.Telephone || []);
                                arrOppContactPersonMobileInfoList = (vData.Mobile || []);
                                arrOppContactPersonEmailInfoList = (vData.Email || []);
                                arrOppContactPersonList[arrindex].Tel = arrOppContactPersonTelInfoList;
                                arrOppContactPersonList[arrindex].Mobile = arrOppContactPersonMobileInfoList
                                arrOppContactPersonList[arrindex].Email = arrOppContactPersonEmailInfoList;
                                AddOppContactPersonTelInfo();
                                AddOppContactPersonEmailInfo();
                                AddOppContactPersonMobileInfo();
                                intOppContactPersonTelInfoCtr = arrOppContactPersonTelInfoList.length;
                                intOppContactPersonMobileInfoCtr = arrOppContactPersonMobileInfoList.length;
                                intOppContactPersonEmailInfoCtr = arrOppContactPersonEmailInfoList.length;
                            }
                        })
                    }
                    else {
                        arrOppContactPersonTelInfoList = (arrOppContactPersonList[arrindex].Tel || []);
                        arrOppContactPersonMobileInfoList = (arrOppContactPersonList[arrindex].Mobile || []);
                        arrOppContactPersonEmailInfoList = (arrOppContactPersonList[arrindex].Email || []);
                        AddOppContactPersonTelInfo();
                        AddOppContactPersonEmailInfo();
                        AddOppContactPersonMobileInfo();
                    }
                }
                else {
                    arrOppContactPersonTelInfoList = (arrOppContactPersonList[arrindex].Tel || []);
                    arrOppContactPersonMobileInfoList = (arrOppContactPersonList[arrindex].Mobile || []);
                    arrOppContactPersonEmailInfoList = (arrOppContactPersonList[arrindex].Email || []);
                    AddOppContactPersonTelInfo();
                    AddOppContactPersonEmailInfo();
                    AddOppContactPersonMobileInfo();
                }
            }

            function ClearOppContactPersonFields() {
                $('#txtOppPeopleID').val('');
                $('#txtOppContactPersonID').val('');
                $('#txtOppContactFirstName').val('');
                $('#txtOppContactMiddleName').val('');
                $('#txtOppContactLastName').val('');
                $('#txtOppContactPersonPosition').val('');
                $('#txtOppContactPersonPositionDescription').val('');
                $('#txtOppContactPersonAddressID').val('');
                $('#txtOppContactPersonHouseBuildingNo').val('');
                $('#txtOppContactPersonStreet').val('');
                $('#selOppContactPersonCity').val('');//.selectmenu('refresh');
                $('#selOppContactPersonCountry').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');//.selectmenu('refresh');
                $('#selOppContactPersonTypeAddress').val();
                $('#lstOppContactPersonTelephone').empty();
                $('#lstOppContactPersonMobile').empty();
                $('#lstOppContactPersonEmail').empty();

                arrOppContactPersonTelInfoList = [];
                intOppContactPersonTelInfoCtr = 0;
                OppContactPersonTelState = "";
                OppContactPersonTelRankNo = "";
                arrOppContactPersonMobileInfoList = [];
                intOppContactPersonMobileInfoCtr = 0;
                OppContactPersonMobileState = "";
                OppContactPersonMobileRankNo = "";
                arrOppContactPersonEmailInfoList = [];
                intOppContactPersonEmailInfoCtr = 0;
                OppContactPersonEmailState = "";
                OppContactPersonEmailRankNo = "";
            }

            function AddOppEmailInfo() {

                var pLV = $('#lstOppEmail');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrOppEmailInfo.length; i++) {
                    if (arrOppEmailInfo[i].EmailState != "Remove") {
                        arrOppEmailInfo[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex">')
                                .data(arrOppEmailInfo[i])
                                .append($('<p class="p-0 m-0 flex-grow-1">')
                                        .html($.trim(arrOppEmailInfo[i].EmailAdd) +
                                            "<br>" + $.trim(arrOppEmailInfo[i].EmailType)
                                        )
                                        .data(arrOppEmailInfo[i])
                                        .click(function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            EditOppEmailInfo($(this).data().arrindex);
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
                                        .data('data', arrOppEmailInfo[i])
                                        .click(function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            var EmailRankNo = $(this).data('data').EmailRankNo;

                                            if ($(this).data('data').IsNew == 1) {
                                                //remove selected email
                                                arrOppEmailInfo = $.grep(arrOppEmailInfo, function (val) {
                                                    return val.EmailRankNo != EmailRankNo;
                                                })
                                            }
                                            else {
                                                arrOppEmailInfo[$(this).data('data').arrindex].EmailState = "Remove";
                                            }

                                            AddOppEmailInfo();
                                        })
                                )

                        pLV.append(pItem);
                    }
                }

                pLV;//.listview('refresh');
                $('#popOppEmailInfo').modal('hide');
            }

            function EditOppEmailInfo(arrindex) {
                $('#popOppEmailInfo').modal('show');
                OppEmailRankNo = arrOppEmailInfo[arrindex].EmailRankNo;
                OppEmailState = "Edit";
                sessionStorage["Emailindex"] = arrindex;
                sessionStorage["EmailIsNew"] = arrOppEmailInfo[arrindex].IsNew;
                sessionStorage["EmailState"] = arrOppEmailInfo[arrindex].EmailState;
                $('#txtOppEmailID').val(arrOppEmailInfo[arrindex].EmailAddID == undefined ? "" : arrOppEmailInfo[arrindex].EmailAddID);
                $('#txtOppEmailAdd').val(arrOppEmailInfo[arrindex].EmailAdd);
                $('#selOppEmailType').val(arrOppEmailInfo[arrindex].EmailType);//.selectmenu('refresh');
            }

            function ClearOppEmailInfoFields() {
                $('#txtOppEmailID').val('');
                $('#txtOppEmailAdd').val('');
                $('#selOppEmailType').val('');//.selectmenu('refresh');
            }

            function AddOppMobileInfo() {

                var pLV = $('#lstOppMobile');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrOppMobileInfo.length; i++) {
                    if (arrOppMobileInfo[i].MobileState != "Remove") {
                        arrOppMobileInfo[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex">')
                                    .data(arrOppMobileInfo[i])
                                    .append($('<p class="p-0 m-0 flex-grow-1">')
                                            .data(arrOppMobileInfo[i])
                                            .attr('ID', arrOppMobileInfo[i].id)
                                            .html("+" + $.trim(arrOppMobileInfo[i].CountryCode.split('-')[1]) + " (" + $.trim(arrOppMobileInfo[i].TelCode.split('-')[1]) + ") " + $.trim(arrOppMobileInfo[i].MobileNo) +
                                                "<br>" + $.trim(arrOppMobileInfo[i].TelCode.split('-')[0]) + " (" + $.trim(arrOppMobileInfo[i].CountryCode.split('-')[0]) + ") " +
                                                "<br>" + $.trim(arrOppMobileInfo[i].ContactType)
                                            )
                                            .click(function (e) {
                                                e.preventDefault();
                                                e.stopPropagation();
                                                EditOppMobileInfo($(this).data().arrindex);
                                            })
                                    )
                                    .append($('<button data-role="none" class="btn btn-danger">')
                                            .append('<span class="bi-trash"></span>')
                                            .data('data', arrOppMobileInfo[i])
                                            .click(function (e) {
                                                e.preventDefault();
                                                e.stopPropagation();
                                                var MobileRankNo = $(this).data('data').MobileRankNo;

                                                if ($(this).data('data').IsNew == 1) {
                                                    //remove selected mobile
                                                    arrOppMobileInfo = $.grep(arrOppMobileInfo, function (val) {
                                                        return val.MobileRankNo != MobileRankNo;
                                                    })
                                                }
                                                else {
                                                    arrOppMobileInfo[$(this).data('data').arrindex].MobileState = "Remove";
                                                }

                                                AddOppMobileInfo();
                                            })
                                    )

                        pLV.append(pItem);
                    }
                }

                pLV;//.listview('refresh');
                $('#popOppMobileInfo').modal('hide');
            }

            function EditOppMobileInfo(arrindex) {
                $('#popOppMobileInfo').modal('show');
                OppMobileRankNo = arrOppMobileInfo[arrindex].MobileRankNo;
                OppMobileState = "Edit";
                sessionStorage["Mobileindex"] = arrindex;
                sessionStorage["MobileIsNew"] = arrOppMobileInfo[arrindex].IsNew;
                sessionStorage["MobileState"] = arrOppMobileInfo[arrindex].MobileState;
                $('#txtOppMobileID').val(arrOppMobileInfo[arrindex].MobileID == undefined ? "" : arrOppMobileInfo[arrindex].MobileID);
                $('#txtOppTelCoPrefixID').val(arrOppMobileInfo[arrindex].TelCodeID);
                $('#txtOppMobileCountryID').val(arrOppMobileInfo[arrindex].CountryCodeID);
                $('#txtOppMobileCountryCode').text(arrOppMobileInfo[arrindex].CountryCode);
                $('#txtOppTelCode').text(arrOppMobileInfo[arrindex].TelCode);
                $('#txtOppMobileNo').val(arrOppMobileInfo[arrindex].MobileNo);
                $('#selOppMobileType').val(arrOppMobileInfo[arrindex].ContactType);//.selectmenu('refresh');
            }

            function ClearOppMobileInfo() {
                $('#txtOppMobileID').val('');
                $('#txtOppTelCoPrefixID').val('');
                $('#txtOppMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtOppMobileCountryCode').text('PHL - 63');
                $('#txtOppTelCode').text('Select');
                $('#txtOppMobileNo').val('');
                $('#selOppMobileType').val('');//.selectmenu('refresh');
            }

            function AddOppTelInfo() {

                var pLV = $('#lstOppTelephone');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrOppTelInfo.length; i++) {
                    if (arrOppTelInfo[i].TelState != "Remove") {
                        arrOppTelInfo[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex">')
                                .data(arrOppTelInfo[i])
                                .append($('<p class="p-0 m-0 flex-grow-1">')
                                        .html("+" + $.trim(arrOppTelInfo[i].CountryCode.split('-')[1]) + " (" + $.trim(arrOppTelInfo[i].AreaCode.split('-')[1]) + ") " + $.trim(arrOppTelInfo[i].TelephoneNo) + ($.trim(arrOppTelInfo[i].ExtensionNo) == "" ? "" : "." + $.trim(arrOppTelInfo[i].ExtensionNo)) +
                                            "<br>" + $.trim(arrOppTelInfo[i].AreaCode.split('-')[0]) + ", " + $.trim(arrOppTelInfo[i].CountryCode.split('-')[0]) +
                                            "<br>" + $.trim(arrOppTelInfo[i].ContactType)
                                        )
                                        .data('data', arrOppTelInfo[i])
                                        .click(function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            EditOppTelInfo($(this).data('data').arrindex);
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
                                        .data('data', arrOppTelInfo[i])
                                        .click(function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            var TelRankNo = $(this).data('data').TelRankNo;

                                            if ($(this).data('data').IsNew == 1) {
                                                //remove selected telephone
                                                arrOppTelInfo = $.grep(arrOppTelInfo, function (val) {
                                                    return val.TelRankNo != TelRankNo;
                                                })
                                            }
                                            else {
                                                arrOppTelInfo[$(this).data('data').arrindex].TelState = "Remove";
                                            }

                                            AddOppTelInfo();
                                        })
                                )

                        pLV.append(pItem);
                    }

                }

                pLV;//.listview('refresh');
                $('#popOppTelInfo').modal('hide');
            }

            function EditOppTelInfo(arrindex) {
                $('#popOppTelInfo').modal('show');

                OppTelRankNo = arrOppTelInfo[arrindex].TelRankNo;
                OppTelState = "Edit";
                sessionStorage["Telindex"] = arrindex;
                sessionStorage["TelIsNew"] = arrOppTelInfo[arrindex].IsNew;
                sessionStorage["TelState"] = arrOppTelInfo[arrindex].TelState;
                $('#txtOppTelID').val(arrOppTelInfo[arrindex].TelID == undefined ? "" : arrOppTelInfo[arrindex].TelID);
                $('#txtOppTelAreaCodeID').val(arrOppTelInfo[arrindex].AreaCodeID);
                $('#txtOppTelCountryID').val(arrOppTelInfo[arrindex].CountryCodeID);
                $('#txtOppCountryCode').text(arrOppTelInfo[arrindex].CountryCode);
                $('#txtOppAreaCode').text(arrOppTelInfo[arrindex].AreaCode);
                $('#txtOppTelephoneNum').val(arrOppTelInfo[arrindex].TelephoneNo);
                $('#txtOppTelephoneExt').val(arrOppTelInfo[arrindex].ExtensionNo);
                $('#selOppTelType').val(arrOppTelInfo[arrindex].ContactType);//.selectmenu('refresh');
            }

            function ClearOppTelInfo() {
                $('#txtOppTelID').val('');
                $('#txtOppTelAreaCodeID').val('');
                $('#txtOppTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtOppCountryCode').text('PHL - 63');
                $('#txtOppAreaCode').text('Select');
                $('#txtOppTelephoneNum').val('');
                $('#txtOppTelephoneExt').val('');
                $('#selOppTelType').val('');//.selectmenu('refresh');
            }

            function ClearOpportunityFields() {
                arrOppTelInfoList = [];
                intOppTelInfoCtr = 0;
                OppTelState = "";
                TelRankNo = "";
                arrOppMobileInfoList = [];
                intOppMobileInfoCtr = 0;
                OppMobileState = "";
                MobileRankNo = "";
                arrOppEmailInfoList = [];
                intOppEmailInfoCtr = 0;
                OppEmailState = "";
                EmailRankNo = "";
                arrOppContactPersonList = [];
                intOppContactPersonCtr = 0;
                ContactPersonState = "";
                ContactPersonRankNo = "";
                templateID = '';
                listOppTemplate = $('#listOppTemplate');

                $('#txtOpportunityID').val('');
                $('#txtOpportunityName').val('');
                $('#txtOpportunityDescription').val('');
                $('#txtOpportunitySize').val('');
                $('#txtOpportunitySizePeriod').val('');
                $('#lstOppContactPerson').empty();
                $('[name="radOppStatus"]').removeAttr('checked');//.checkboxradio('refresh');
                $('#selOpportunityFunnelTemplate').val('');//.selectmenu('refresh');
                $('#tabOpportunity').opentabmenu();
                $('#txtOpportunityName, #txtOpportunityDescription, #txtOpportunitySize, #txtOpportunitySizePeriod');//.textinput('enable');
                $('[name="radOppStatus"]');//.checkboxradio('enable');
                listOppTemplate.empty();
                $('#listOppTemplateOnChange').empty();
            }

            function DisplayCities() {
                var selOppContactPersonCity = $('#selOppContactPersonCity');

                selOppContactPersonCity.empty();
                selOppContactPersonCity.append(new Option('Please Select City', ''));

                AjaxSend({
                    url: 'CRMDashboard/getCityNameList',
                    success: function (vData) {


                        for (var p = 0; p < vData.CityData.Rows.length; p++) {
                            if (vData.CityData.Rows[p].Name != undefined) {
                                selOppContactPersonCity.append(new Option(vData.CityData.Rows[p].Name, vData.CityData.Rows[p].ID))
                            }
                        }
                        selOppContactPersonCity;//.selectmenu('refresh');
                        DisplayCountries()
                    }
                });
            }

            function DisplayCountries() {
                var selOppContactPersonCountry = $('#selOppContactPersonCountry');

                selOppContactPersonCountry.empty();
                selOppContactPersonCountry.append(new Option('Please Select Country', ''));

                AjaxSend({
                    url: 'CRMDashboard/getCountryNameList',
                    success: function (vData) {

                        for (var p = 0; p < vData.CountryData.Rows.length; p++) {
                            selOppContactPersonCountry.append(new Option(vData.CountryData.Rows[p].Name, vData.CountryData.Rows[p].ID))
                        }
                        selOppContactPersonCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e');//.selectmenu('refresh');
                    }
                });
            }

            function GetClientList() {
                AjaxSend({
                    url: 'CRMDashboard/GetAllClientList', data: { OrganizationID: sessionStorage.OrgID },
                    success: function (e) {
                        var vRows = e.Results.Rows;
                        var selOppClientName = $('#selOppClientName');

                        selOppClientName.empty();
                        selOppClientName.append(new Option('Please Select', '', true))

                        for (var i = 0; i < vRows.length; i++) {
                            selOppClientName.append(new Option(vRows[i].ClientName, vRows[i].ID))
                        }

                        selOppClientName;//.selectmenu('refresh');
                    }
                })
            }

            function GetFunnelTemplate() {
                var selOpportunityFunnelTemplate = $('#selOpportunityFunnelTemplate');

                selOpportunityFunnelTemplate.empty();

                AjaxSend({
                    url: 'CRMDashboard/GetFunnelTemplate',
                    data: { IsFromSystemAdmin: false },
                    success: function (vData) {
                        var vRows = vData.data;
                        selOpportunityFunnelTemplate.append(new Option('Please Select', ''));

                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                //selOpportunityFunnelTemplate.append(new Option(vRows[i].TemplateName, vRows[i].ID))
                                selOpportunityFunnelTemplate.append($('<option>')
                                        .data('template', vRows[i])
                                        .attr('value', vRows[i].ID)
                                        .text(vRows[i].TemplateName)
                                    )
                            }
                        }

                        selOpportunityFunnelTemplate;//.selectmenu('refresh');
                    }
                })
            }
        })
    }
)