require(['common'], function () {
    var isPopupOpen = false;
    var isActiveInputField = false;
    var htmlSettings = '';
    var arrTelInfoList = [];
    var intTelInfoCtr = 0;
    var TelState = "";
    var TelRankNo = "";
    var arrMobileInfoList = [];
    var intMobileInfoCtr = 0;
    var MobileState = "";
    var MobileRankNo = "";
    var arrEmailInfoList = [];
    var intEmailInfoCtr = 0;
    var EmailState = "";
    var EmailRankNo = "";
    var arrWebsiteList = [];
    var intWebsiteCtr = 0;
    var WebsiteState = "";
    var WebsiteRankNo = "";
    var arrContactPersonList = [];
    var intContactPersonCtr = 0;
    var ContactPersonState = "";
    var ContactPersonRankNo = "";

    var arrContactPersonTelInfoList = [];
    var intContactPersonTelInfoCtr = 0;
    var ContactPersonTelState = "";
    var ContactPersonTelRankNo = "";
    var arrContactPersonMobileInfoList = [];
    var intContactPersonMobileInfoCtr = 0;
    var ContactPersonMobileState = "";
    var ContactPersonMobileRankNo = "";
    var arrContactPersonEmailInfoList = [];
    var intContactPersonEmailInfoCtr = 0;
    var ContactPersonEmailState = "";
    var ContactPersonEmailRankNo = "";
    var arrSecQue = [];

    var arrOutletTelInfoList = [];
    var intOutletTelInfoCtr = 0;
    var OutletTelState = "";
    var OutletTelRankNo = "";
    var arrOutletMobileInfoList = [];
    var intOutletMobileInfoCtr = 0;
    var OutletMobileState = "";
    var OutletMobileRankNo = "";
    var arrOutletEmailInfoList = [];
    var intOutletEmailInfoCtr = 0;
    var OutletEmailState = "";
    var OutletEmailRankNo = "";

    var arrPersonTelInfoList = [];
    var intPersonTelInfoCtr = 0;
    var PersonTelState = "";
    var PersonTelRankNo = "";
    var arrPersonMobileInfoList = [];
    var intPersonMobileInfoCtr = 0;
    var PersonMobileState = "";
    var PersonMobileRankNo = "";
    var arrPersonEmailInfoList = [];
    var arrPersonLoginCredentials = [];
    var arrPersonSecQue = [];
    var intPersonEmailInfoCtr = 0;
    var PersonEmailState = "";
    var PersonEmailRankNo = "";

    var isFromColdCallPage = false;
    var isStartCall = false;
    var callStartDate = '', callEndDate = '';
    var BackPage = '', todoSetDate = '';

    var arrTodoList = [];
    var intTodoInfoCtr = 0;
    var TodoState = "";
    var TodoRankNo = "";

    var arrPersonOrgAffList = [];
    var arrOrgAffList = [];
    var arrOrgAffPersonList = [];
    var arrOrgPeopleCitiesList = [];
    var arrSMSRecipient = [];
    var tempArrMobileNo = [];

    var isSendingBulkMsg = false;
    var divCountRow = $('#divCountRow');
    var ctrUnknownNumber = 1;
    var datatable;
    var isFromListEvent = false;
    var entities = { '<': '&lt;', '>': '&gt;' };

    var isEditingANote = false;
    var isEditingCallNote = false;
    var isCallNoteSaveAsDraft = false;
    var isEditingProfile = false;

    var arrFilter = [];
    var filterVal = '';

    var isViewReadOnlyProfile = false;
    var isAddNewProspect = false;
    var menuActionToSave = '';
    var arrCompanyOutlet = [], CompanyOutletState = "", arrCompanyOutletDepartment = [], CompanyOutletDepartmentState = "";
    var arrNewDepartment = [], isOutlet = false;

    var timeOut = null, myXHR = null;

    var academicLvl = '', arrElementary = [], arrHighSchool = [], arrCollege = [], isNewEducation = true, ctrElem = 0, ctrHs = 0, ctrCollege = 0, ctrIndexEducation = 0;

    var isCreateFromExistingRecord = false;
    var RevSalesRepID = '', arrRevPriceLevels = [], RevPaymentTerms = '';

    $(document).ready(function () {
        $(function (){
            AjaxSend({
                url: 'Ecommerce/ValidateSupplierLink',
                data: {
                    id: GetUrlParameters().id
                }, success: function (e) {
                    var vRows = e.Results.Rows;

                    if (vRows.length > 0) {
                        $('#divLinkSuccess').pageshow();
                        LoadRequiredData();
                        $('.lblCompanyName').text(vRows[0].CompanyName);
                        sessionStorage.OrgID = vRows[0].OrgID;
                        var viewID;

                        if (GetUrlParameters().mod == 'Customer') {
                            viewID = (vRows[0].CustomerID || vRows[0].SupplierID); // kapag Customer Public URL, dapat CustomerID ang kwaon, kapag Contact Person Public URL, either sa duha ang kwaon na ID, possible man gud ang Customer kay supplier pud or vice versa.

                            if (GetUrlParameters().type == 'Org') {
                                $('#divMessageForCustomerOrg').pageshow();
                            } else {
                                $('#divMessageForCustomerPerson').pageshow();
                            }                            
                        } else {
                            viewID = vRows[0].SupplierID;
                            $('#divMessageForSupplier').pageshow();
                        }

                        if (GetUrlParameters().type == 'Org') {
                            GetProspectDetails(viewID);
                            $('#divForCompany').pageshow();
                        } else {
                            GetProspectDetailsPeople(viewID);
                            $('#divForPersonal').pageshow();
                        }
                        
                        if (GetUrlParameters().pid == undefined) {
                            $('.lblSuccessMsg').text('Thanks for registering!');                            
                        } else {
                            $('.lblSuccessMsg').text('Your account details has been updated!');
                        }
                    } else {
                        $('#divLinkInvalid').pageshow();
                    }
                }
            })
        })

        //LoadRequiredData();

        $('body')
        .on('click', '#btnLoginVendorPortal', function () {
            location.href = location.origin + '/VendorPortal/Signin?OrgID' + sessionStorage.OrgID;
        })
        .on('click', '#btnLoginEcommerce', function () {
            location.href = location.origin + '/Ecommerce/Signin?OrgID' + sessionStorage.OrgID;
        })
        .on('click', '#btnStartColdCall', function () {
            if (isStartCall == false) {
                callStartDate = $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a');
                $.APP.startTimer('sw');
                $('body').find('#btnStartColdCall').addClass('btn-disabled');
                isStartCall = true;
            }
        })
        .on('click', '#btnEndColdCall', function () {
            $.APP.stopTimer();
            $('#btnStartColdCall').removeClass('btn-disabled');
            isStartCall = false;
            callEndDate = new Date();
        })
        .on('click', '#btnSearchMobile', function () {
            if ($('.contentSearchEngine').css('display') == 'none') {
                $('.contentSearchEngine').show()
            }
            else {
                $('.contentSearchEngine').hide()
            }
        })
        //.on('click', '.btnHome, .btnHomeMobile', function () {
        //    SignOut();
        //})
        .on('click', '#btnAddTopic', function () {
            ChangePage('#pageAddNewTopic');
        })
        .on('click', '.btnSettings', function () {
            location = MergeUrlParameters({
                OrgID: sessionStorage.OrgID,
                IsMobile: CBool(IsMobile) ? 1 : 0
            }, '/CRMDashboard/SystemAdministration');
        })
        .on('click', '#btnCancelSetAppointment', function () {
            $('#pageMainDashboard').pageshow();
        })
        .on('click', '#btnCancelColdCall, #btnCancelSendSMS', function () {
            BreadCrumbs('.ItemsBreadCrumbs',
                    [{ Text: 'Lead Generation' }]
                )
            $('#pageMainDashboard').pageshow();
        })
        .on('click', '#btnOptionColdCall', function () {
            PopupButtonList('', 'Option',
                [
                    {
                        Text: 'View Profile',
                        data: $('.txtActionClientName').data(),
                        Click: function (e) {
                            BreadCrumbs('.ItemsBreadCrumbs',
                                [
                                    { Text: 'Lead Generation', Click: function () { $('#pageMainDashboard').pageshow() } },
                                    { Text: 'Cold Call', Click: function () { $('#pageActions').pageshow() } },
                                    { Text: (e.CompanyName || e.PeopleFullName) + ' Profile' }
                                ]
                                )
                            if (e.ProspectType == "Org") {
                                GetProspectDetails(e.ID);
                                $('#pageAddOrgProspect').pageshow();
                            }
                            else {
                                GetProspectDetailsPeople(e.ID);
                                $('#pageAddPersonProspect').pageshow();
                            }
                            isFromColdCallPage = true;
                        }
                    },
                    {
                        Text: 'Set Appointment',
                        data: $('.txtActionClientName').data(),
                        Click: function (e) {
                            console.log(e);
                        }
                    },
                    {
                        Text: 'Add Contact Person',
                        data: $('.txtActionClientName').data(),
                        Click: function (e) {
                            BreadCrumbs('.ItemsBreadCrumbs',
                                [
                                    { Text: 'Lead Generation', Click: function () { $('#pageMainDashboard').pageshow() } },
                                    { Text: 'Cold Call', Click: function () { $('#pageColdCall').pageshow() } },
                                    { Text: 'New Contact Person for ' + (e.CompanyName || e.PeopleFullName) }
                                ]
                                )
                            if (e.ProspectType == "Org") {
                                GetProspectDetails(e.ID);
                                $('#pageAddOrgProspect').pageshow();
                                $('#divCompanyProfile').hide();
                            }
                            else {
                                GetProspectDetailsPeople(e.ID);
                                $('#pageAddPersonProspect').pageshow();
                            }
                            isFromColdCallPage = true;
                        }
                    },
                    {
                        Text: 'Interaction History',
                        data: $('.txtActionClientName').data(),
                        Click: function (e) {
                            BreadCrumbs('.ItemsBreadCrumbs',
                                [
                                    { Text: 'Lead Generation', Click: function () { $('#pageMainDashboard').pageshow() } },
                                    { Text: 'Cold Call', Click: function () { $('#pageActions').pageshow() } },
                                    { Text: 'Interaction History Of ' + (e.CompanyName || e.PeopleFullName) }
                                ]
                                )
                            $('#pageActions').pageshow();
                            $('#btnPageInteractionHistory').click();
                            if (e.ProspectType == "Org") {
                                $('.txtActionClientName').text(e.CompanyName);
                            }
                            else {
                                $('.txtActionClientName').text(e.PeopleFullName);
                            }
                            BackPage = '#pageActions';
                            isFromColdCallPage = true;
                            //GetInteractionHistory(e.ID);
                        }
                    }
                ], { AutoDismiss: true }
            )
        })
        .on('click', '#btnSaveColdCall', function () {
            $.APP.stopTimer();
            $('#btnStartColdCall').removeClass('btn-disabled');
            isStartCall = false;
            callEndDate = $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a');

            var pData = {
                Client: $('.txtActionClientName').data(),
                Notes: ($('#txtCallNotes').val().replace(/[<>]/g, function (s) { return entities[s]; })).replace(/\r\n|\r|\n/g, "\n"),
                CallStartDate: (callStartDate == '' ? $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a') : callStartDate),
                CallEndDate: callEndDate,
                CallDuration: $('#sw_h').text() + ':' + $('#sw_m').text() + ':' + $('#sw_s').text() + ':' + $('#sw_ms').text(),
                IsOrg: ($('.txtActionClientName').data().ProspectType == "Org" ? 1 : 0),
                DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a')
            }

            AjaxSend({
                url: 'CRMDashboard/SaveColdCall',
                data: pData,
                success: function (vData) {
                    WebToast('Successfully Saved', 'top');
                    if (isEditingCallNote == false) {
                        BreadCrumbs('.ItemsBreadCrumbs',
                                [{ Text: 'Lead Generation' }]
                            )
                        callStartDate = '';
                        callEndDate = '';
                        $('#btnPageColdCall').click();
                    }
                    isEditingCallNote = false;
                    isCallNoteSaveAsDraft = false;
                    $('#txtCallNotes').val('').css('height', '100px').css('min-height', '100px');
                    $('#sw_h, #sw_m, #sw_s, #sw_ms').text('00');
                }
            })
        })
        .on('click', '#btnAddOrgTelephone', function () {
            ClearTelInfo();
            $('#popOrgTelInfo').show();
        })
        .on('click', '#btnCancelOrgTelephone', function () {
            $('#popOrgTelInfo').hide();
            TelState = '';
        })
        .on('click', '#btnOrgTelAreaCode', function () {
            BrowsePopup('LibraryAreaCodes', 'Area Codes',
                [
                    {
                        DisplayText: [{ Column: 'Area' }, { Label: 'Code', Column: 'CallingCode' }],
                        Click: function (e) {
                            $('#txtOrgTelAreaCodeID').val(e.ID);
                            $('#txtAreaCode').text(e.Area + ' - ' + e.CallingCode)
                        }
                    }
                ]
            )
        })
        .on('keydown', '#txtTelephoneNum', function (e) {
            if (!NumberDotOnly($(this), e)) {
                return;
            }
        })
        .on('click', '#btnSaveOrgTelephone', function () {
            //if ($('#txtAreaCode').text() == "Select") {
            //    $('#spanerrorAreaCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
            //    return false;
            //}

            if ($('#txtTelephoneNum').val() == "") {
                $('#spanerrorTelephoneNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            //if ($('#txtTelephoneNum').val().length != 7) {
            //    $('#spanerrorTelephoneNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
            //    return false;
            //}

            if ($('#selOrgTelType').val() == "") {
                $('#spanerrorTelType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }
            intTelInfoCtr = intTelInfoCtr + 1;

            if (TelState == "") {
                arrTelInfoList.push({
                    "TelRankNo": intTelInfoCtr,
                    "CountryCodeID": $('#txtOrgTelCountryID').val(),
                    "CountryCode": $('#txtCountryCode').text(),
                    "AreaCode": $('#txtAreaCode').text(),
                    "AreaCodeID": $('#txtOrgTelAreaCodeID').val(),
                    "TelephoneNo": $('#txtTelephoneNum').val(),
                    "ExtensionNo": $('#txtTelephoneExt').val(),
                    "ContactType": $('#selOrgTelType').val(),
                    "TelState": "Add",
                    "IsNew": 1,
                    "TelID": $('#txtOrgTelID').val()
                })
            }
            else {
                arrTelInfoList[sessionStorage.Telindex].TelRankNo = TelRankNo;
                arrTelInfoList[sessionStorage.Telindex].CountryCodeID = $('#txtOrgTelCountryID').val();
                arrTelInfoList[sessionStorage.Telindex].CountryCode = $('#txtCountryCode').text();
                arrTelInfoList[sessionStorage.Telindex].AreaCodeID = $('#txtOrgTelAreaCodeID').val();
                arrTelInfoList[sessionStorage.Telindex].AreaCode = $('#txtAreaCode').text();
                arrTelInfoList[sessionStorage.Telindex].TelephoneNo = $('#txtTelephoneNum').val();
                arrTelInfoList[sessionStorage.Telindex].ExtensionNo = $('#txtTelephoneExt').val();
                arrTelInfoList[sessionStorage.Telindex].ContactType = $('#selOrgTelType').val();
                arrTelInfoList[sessionStorage.Telindex].TelState = sessionStorage.TelState;
                arrTelInfoList[sessionStorage.Telindex].IsNew = sessionStorage.TelIsNew;
                arrTelInfoList[sessionStorage.Telindex].TelID = $('#txtOrgTelID').val();
            }

            AddTelInfo();
            ClearTelInfo();
            TelState = "";
        })
        .on('click', '#btnAddOrgMobile', function () {
            ClearMobileInfo();
            $('#popOrgMobileInfo').show();
        })
        .on('click', '#btnCancelOrgMobile', function () {
            $('#popOrgMobileInfo').hide();
            MobileState = '';
        })
        .on('click', '#btnOrgMobileTelCode', function () {
            BrowsePopup('LibraryTelcoPrefixes', 'Telco Codes',
                [
                    {
                        DisplayText: [{ Column: 'Prefix' }, { Column: 'TelCo' }],
                        Click: function (e) {
                            $('#txtOrgTelCoPrefixID').val(e.ID);
                            $('#txtTelCode').text(e.TelCo + ' - ' + e.Prefix)
                        }
                    }
                ]
            )
        })
        .on('keydown', '#txtMobileNo', function (e) {
            if (!NumberDotOnly($(this), e)) {
                return;
            }
        })
        .on('click', '#btnSaveOrgMobile', function () {
            //if ($('#txtTelCode').text() == "Select") {
            //    $('#spanerrorMobileTelCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
            //    return false;
            //}

            if ($('#txtMobileNo').val() == "") {
                $('#spanerrorMobileNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            //if ($('#txtMobileNo').val().length != 7) {
            //    $('#spanerrorMobileNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
            //    return false;
            //}

            if ($('#selOrgMobileType').val() == "") {
                $('#spanerrorMobileType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            intMobileInfoCtr = intMobileInfoCtr + 1;

            if (MobileState == "") {
                arrMobileInfoList.push({
                    "MobileRankNo": intMobileInfoCtr,
                    "CountryCodeID": $('#txtOrgMobileCountryID').val(),
                    "CountryCode": $('#txtMobileCountryCode').text(),
                    "TelCode": $('#txtTelCode').text(),
                    "TelCodeID": $('#txtOrgTelCoPrefixID').val(),
                    "MobileNo": $('#txtMobileNo').val(),
                    "ContactType": $('#selOrgMobileType').val(),
                    "MobileID": $('#txtOrgMobileID').val(),
                    "MobileState": "Add",
                    "IsNew": 1,
                    "MobileNos": '+' + $('#txtMobileCountryCode').text() + $('#txtTelCode').text() + $('#txtMobileNo').val()
                })
            }
            else {
                arrMobileInfoList[sessionStorage.Mobileindex].MobileRankNo = MobileRankNo;
                arrMobileInfoList[sessionStorage.Mobileindex].CountryCodeID = $('#txtOrgMobileCountryID').val();
                arrMobileInfoList[sessionStorage.Mobileindex].CountryCode = $('#txtMobileCountryCode').text();
                arrMobileInfoList[sessionStorage.Mobileindex].TelCodeID = $('#txtOrgTelCoPrefixID').val();
                arrMobileInfoList[sessionStorage.Mobileindex].TelCode = $('#txtTelCode').text();
                arrMobileInfoList[sessionStorage.Mobileindex].MobileNo = $('#txtMobileNo').val();
                arrMobileInfoList[sessionStorage.Mobileindex].ContactType = $('#selOrgMobileType').val();
                arrMobileInfoList[sessionStorage.Mobileindex].MobileState = sessionStorage.MobileState;
                arrMobileInfoList[sessionStorage.Mobileindex].IsNew = sessionStorage.MobileIsNew;
                arrMobileInfoList[sessionStorage.Mobileindex].MobileNos = '+' + $('#txtMobileCountryCode').text() + $('#txtTelCode').text() + $('#txtMobileNo').val();
            }

            AddMobileInfo();
            MobileState = "";
            ClearMobileInfo();
        })
        .on('click', '#btnAddOrgEmail', function () {
            ClearEmailInfoFields();
            $('#popOrgEmailInfo').show();
        })
        .on('click', '#btnCancelOrgEmailAddress', function () {
            $('#popOrgEmailInfo').hide();
            EmailState = '';
        })
        .on('click', '#btnSaveOrgEmailAddress', function () {
            if (IsEmail($.trim($("#txtEmailAdd").val())) == false) {
                $('#spanerrorEmailAddressValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            if ($.trim($("#txtEmailAdd").val()) == "") {
                $('#spanerrorEmailAddress').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            if ($('#selOrgEmailType').val() == "") {
                $('#spanerrorEmailType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            intEmailInfoCtr = intEmailInfoCtr + 1;

            if (EmailState == "") {
                arrEmailInfoList.push({
                    "EmailRankNo": intEmailInfoCtr,
                    "EmailAddID": $('#txtOrgEmailID').val(),
                    "EmailAdd": $('#txtEmailAdd').val(),
                    "EmailType": $('#selOrgEmailType').val(),
                    "EmailState": "Add",
                    "IsNew": 1
                })
            }
            else {
                arrEmailInfoList[sessionStorage.Emailindex].EmailRankNo = EmailRankNo;
                arrEmailInfoList[sessionStorage.Emailindex].EmailAddID = $('#txtOrgEmailID').val();
                arrEmailInfoList[sessionStorage.Emailindex].EmailAdd = $('#txtEmailAdd').val();
                arrEmailInfoList[sessionStorage.Emailindex].EmailType = $('#selOrgEmailType').val();
                arrEmailInfoList[sessionStorage.Emailindex].EmailState = sessionStorage.EmailState;
                arrEmailInfoList[sessionStorage.Emailindex].IsNew = sessionStorage.EmailIsNew;
            }

            AddEmailInfo();
            EmailState = "";
        })
        .on('click', '#btnAddOrgWebsite', function () {
            ClearWebsiteFields();
            $('#popOrgWebsiteInfo').show();
        })
        .on('click', '#btnCancelOrgWebsite', function () {
            $('#popOrgWebsiteInfo').hide();
            WebsiteState = '';
        })
        .on('click', '#btnSaveOrgWebsite', function () {
            if ($.trim($('#txtWebsiteUrl').val()) == "") {
                $('#spanerrorWebsiteUrl').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            intWebsiteCtr = intWebsiteCtr + 1;

            if (WebsiteState == "") {
                arrWebsiteList.push({
                    "WebsiteRankNo": intWebsiteCtr,
                    "WebsiteID": $('#txtOrgWebsiteID').val(),
                    "Website": $('#txtWebsiteUrl').val(),
                    "WebsiteState": "Add",
                    "IsNew": 1
                })
            }
            else {
                arrWebsiteList[sessionStorage.Websiteindex].WebsiteRankNo = EmailRankNo;
                arrWebsiteList[sessionStorage.Websiteindex].WebsiteID = $('#txtOrgWebsiteID').val();
                arrWebsiteList[sessionStorage.Websiteindex].Website = $('#txtWebsiteUrl').val();
                arrWebsiteList[sessionStorage.Websiteindex].WebsiteState = sessionStorage.WebsiteState;
                arrWebsiteList[sessionStorage.Websiteindex].IsNew = sessionStorage.WebsiteIsNew;
            }

            AddWebsiteInfo();
            WebsiteState = "";
        })
        .on('click', '#btnAddContactPerson', function () {
            ClearContactPersonFields();
            ClearContactPersonEmailInfoFields();
            ClearContactPersonMobileInfo();
            ClearContactPersonTelInfo();
            //$('#popContactPerson').popup('open');
            $('#popContactPersonProfile').show();
        })
        .on('click', '#btnCancelContact', function () {
            $('#popContactPersonProfile').hide();
            ContactPersonState = '';
        })
        .on('click', '#btnOkContact', function () {
            if ($.trim($('#txtContactFirstName').val()) == "") {
                $('#spanerrorContactFname').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            intContactPersonCtr = intContactPersonCtr + 1;

            if (ContactPersonState == "") {
                arrContactPersonList.push({
                    "ContactPersonRankNo": intContactPersonCtr,
                    "PeopleID": $('#txtPeopleID').val(),
                    "ID": $('#txtContactPersonID').val(),
                    "Firstname": $('#txtContactFirstName').val(),
                    "Middlename": $('#txtContactMiddleName').val(),
                    "Lastname": $('#txtContactLastName').val(),
                    "Position": $('#txtContactPersonPosition').val(),
                    "Tin": $('#txtContactPersonTIN').val(),
                    "PositionDescription": $('#txtContactPersonPositionDescription').val(),
                    "AddressID": $('#txtContactPersonAddressID').val(),
                    "HouseBuildingNo": $('#txtContactPersonHouseBuildingNo').val(),
                    "Street": $('#txtContactPersonStreet').val(),
                    "City": $('#selContactPersonCity').val(),
                    "Country": $('#selContactPersonCountry').val(),
                    "TypeAddress": $('#selContactPersonTypeAddress').val(),
                    "ContactPersonState": "Add",
                    "IsNew": 1,
                    "Tel": arrContactPersonTelInfoList,
                    "Mobile": arrContactPersonMobileInfoList,
                    "Email": arrContactPersonEmailInfoList
                })
            }
            else {
                arrContactPersonList[sessionStorage.Contactindex].ContactPersonRankNo = ContactPersonRankNo;
                arrContactPersonList[sessionStorage.Contactindex].PeopleID = $('#txtPeopleID').val();
                arrContactPersonList[sessionStorage.Contactindex].ID = $('#txtContactPersonID').val();
                arrContactPersonList[sessionStorage.Contactindex].Firstname = $('#txtContactFirstName').val();
                arrContactPersonList[sessionStorage.Contactindex].Middlename = $('#txtContactMiddleName').val();
                arrContactPersonList[sessionStorage.Contactindex].Lastname = $('#txtContactLastName').val();
                arrContactPersonList[sessionStorage.Contactindex].Position = $('#txtContactPersonPosition').val();
                arrContactPersonList[sessionStorage.Contactindex].Tin = $('#txtContactPersonTIN').val();
                arrContactPersonList[sessionStorage.Contactindex].PositionDescription = $('#txtContactPersonPositionDescription').val();
                arrContactPersonList[sessionStorage.Contactindex].AddressID = $('#txtContactPersonAddressID').val();
                arrContactPersonList[sessionStorage.Contactindex].HouseBuildingNo = $('#txtContactPersonHouseBuildingNo').val();
                arrContactPersonList[sessionStorage.Contactindex].Street = $('#txtContactPersonStreet').val();
                arrContactPersonList[sessionStorage.Contactindex].City = $('#selContactPersonCity').val();
                arrContactPersonList[sessionStorage.Contactindex].Country = $('#selContactPersonCountry').val();
                arrContactPersonList[sessionStorage.Contactindex].TypeAddress = $('#selContactPersonTypeAddress').val();
                arrContactPersonList[sessionStorage.Contactindex].ContactPersonState = sessionStorage.ContactPersonState;
                arrContactPersonList[sessionStorage.Contactindex].IsNew = sessionStorage.ContactPersonIsNew;
                arrContactPersonList[sessionStorage.Contactindex].Tel = arrContactPersonTelInfoList;
                arrContactPersonList[sessionStorage.Contactindex].Mobile = arrContactPersonMobileInfoList;
                arrContactPersonList[sessionStorage.Contactindex].Email = arrContactPersonEmailInfoList;
            }

            AddContactPersonInfo();
            ContactPersonState = "";
        })
        .on('click', '#btnAddContactPersonTelephone', function () {
            ClearContactPersonTelInfo();
            $('#popContactPersonTelInfo').show();
        })
        .on('click', '#btnCancelContactPersonTelephone', function () {
            $('#popContactPersonTelInfo').hide();
            ContactPersonTelState = '';
        })
        .on('click', '#btnContactPersonTelAreaCode', function () {
            BrowsePopup('LibraryAreaCodes', 'Area Codes',
                [
                    {
                        DisplayText: [{ Column: 'Area' }, { Label: 'Code', Column: 'CallingCode' }],
                        Click: function (e) {
                            $('#txtContactPersonTelAreaCodeID').val(e.ID);
                            $('#txtContactPersonAreaCode').text(e.Area + ' - ' + e.CallingCode)
                        }
                    }
                ]
            )
        })
        .on('keydown', '#txtContactPersonTelephoneNum', function (e) {
            if (!NumberDotOnly($(this), e)) {
                return;
            }
        })
        .on('click', '#btnSaveContactPersonTelephone', function () {
            if ($('#txtContactPersonAreaCode').text() == "Select") {
                $('#spanerrorContactPersonAreaCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            if ($('#txtContactPersonTelephoneNum').val() == "") {
                $('#spanerrorContactPersonTelephoneNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            //if ($('#txtContactPersonTelephoneNum').val().length != 7) {
            //    $('#spanerrorContactPersonTelephoneNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
            //    return false;
            //}

            if ($('#selContactPersonTelType').val() == "") {
                $('#spanerrorContactPersonTelType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }
            intContactPersonTelInfoCtr = intContactPersonTelInfoCtr + 1;

            if (ContactPersonTelState == "") {
                arrContactPersonTelInfoList.push({
                    "TelRankNo": intContactPersonTelInfoCtr,
                    "CountryCodeID": $('#txtContactPersonTelCountryID').val(),
                    "CountryCode": $('#txtContactPersonCountryCode').text(),
                    "AreaCode": $('#txtContactPersonAreaCode').text(),
                    "AreaCodeID": $('#txtContactPersonTelAreaCodeID').val(),
                    "TelephoneNo": $('#txtContactPersonTelephoneNum').val(),
                    "ExtensionNo": $('#txtContactPersonTelephoneExt').val(),
                    "ContactType": $('#selContactPersonTelType').val(),
                    "TelState": "Add",
                    "IsNew": 1,
                    "TelID": $('#txtContactPersonTelID').val()
                })
            }
            else {
                arrContactPersonTelInfoList[sessionStorage.Telindex].TelRankNo = ContactPersonTelRankNo;
                arrContactPersonTelInfoList[sessionStorage.Telindex].CountryCodeID = $('#txtContactPersonTelCountryID').val();
                arrContactPersonTelInfoList[sessionStorage.Telindex].CountryCode = $('#txtContactPersonCountryCode').text();
                arrContactPersonTelInfoList[sessionStorage.Telindex].AreaCodeID = $('#txtContactPersonTelAreaCodeID').val();
                arrContactPersonTelInfoList[sessionStorage.Telindex].AreaCode = $('#txtContactPersonAreaCode').text();
                arrContactPersonTelInfoList[sessionStorage.Telindex].TelephoneNo = $('#txtContactPersonTelephoneNum').val();
                arrContactPersonTelInfoList[sessionStorage.Telindex].ExtensionNo = $('#txtContactPersonTelephoneExt').val();
                arrContactPersonTelInfoList[sessionStorage.Telindex].ContactType = $('#selContactPersonTelType').val();
                arrContactPersonTelInfoList[sessionStorage.Telindex].TelState = sessionStorage.TelState;
                arrContactPersonTelInfoList[sessionStorage.Telindex].IsNew = sessionStorage.TelIsNew;
                arrContactPersonTelInfoList[sessionStorage.Telindex].TelID = $('#txtContactPersonTelID').val();
            }

            AddContactPersonTelInfo();
            ClearContactPersonTelInfo();
            ContactPersonTelState = "";
        })
        .on('click', '#btnAddContactPersonMobile', function () {
            ClearContactPersonMobileInfo();
            $('#popContactPersonMobileInfo').show();
        })
        .on('click', '#btnCancelContactPersonMobile', function () {
            $('#popContactPersonMobileInfo').hide();
            ContactPersonMobileState = '';
        })
        .on('click', '#btnContactPersonMobileTelCode', function () {
            BrowsePopup('LibraryTelcoPrefixes', 'Telco Codes',
                [
                    {
                        DisplayText: [{ Column: 'Prefix' }, { Column: 'TelCo' }],
                        Click: function (e) {
                            $('#txtContactPersonTelCoPrefixID').val(e.ID);
                            $('#txtContactPersonTelCode').text(e.TelCo + ' - ' + e.Prefix)
                        }
                    }
                ]
            )
        })
        .on('keydown', '#txtContactPersonMobileNo', function (e) {
            if (!NumberDotOnly($(this), e)) {
                return;
            }
        })
        .on('click', '#btnSaveContactPersonMobile', function () {
            //if ($('#txtContactPersonTelCode').text() == "Select") {
            //    $('#spanerrorContactPersonMobileTelCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
            //    return false;
            //}

            if ($('#txtContactPersonMobileNo').val() == "") {
                $('#spanerrorContactPersonMobileNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            //if ($('#txtContactPersonMobileNo').val().length != 7) {
            //    $('#spanerrorContactPersonMobileNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
            //    return false;
            //}

            if ($('#selContactPersonMobileType').val() == "") {
                $('#spanerrorContactPersonMobileType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            intContactPersonMobileInfoCtr = intContactPersonMobileInfoCtr + 1;

            if (ContactPersonMobileState == "") {
                arrContactPersonMobileInfoList.push({
                    "MobileRankNo": intContactPersonMobileInfoCtr,
                    "CountryCodeID": $('#txtContactPersonMobileCountryID').val(),
                    "CountryCode": $('#txtContactPersonMobileCountryCode').text(),
                    //"TelCode": $('#txtContactPersonTelCode').text(),
                    //"TelCodeID": $('#txtContactPersonTelCoPrefixID').val(),
                    "MobileNo": $('#txtContactPersonMobileNo').val(),
                    "ContactType": $('#selContactPersonMobileType').val(),
                    "MobileID": $('#txtContactPersonMobileID').val(),
                    "MobileState": "Add",
                    "IsNew": 1
                })
            }
            else {
                arrContactPersonMobileInfoList[sessionStorage.Mobileindex].MobileRankNo = ContactPersonMobileRankNo;
                arrContactPersonMobileInfoList[sessionStorage.Mobileindex].CountryCodeID = $('#txtContactPersonMobileCountryID').val();
                arrContactPersonMobileInfoList[sessionStorage.Mobileindex].CountryCode = $('#txtContactPersonMobileCountryCode').text();
                //arrContactPersonMobileInfoList[sessionStorage.Mobileindex].TelCodeID = $('#txtContactPersonTelCoPrefixID').val();
                //arrContactPersonMobileInfoList[sessionStorage.Mobileindex].TelCode = $('#txtContactPersonTelCode').text();
                arrContactPersonMobileInfoList[sessionStorage.Mobileindex].MobileNo = $('#txtContactPersonMobileNo').val();
                arrContactPersonMobileInfoList[sessionStorage.Mobileindex].ContactType = $('#selContactPersonMobileType').val();
                arrContactPersonMobileInfoList[sessionStorage.Mobileindex].MobileState = sessionStorage.MobileState;
                arrContactPersonMobileInfoList[sessionStorage.Mobileindex].IsNew = sessionStorage.MobileIsNew;
            }

            AddContactPersonMobileInfo();
            ContactPersonMobileState = "";
            ClearContactPersonMobileInfo();
        })
        .on('click', '#btnAddContactPersonEmail', function () {
            ClearContactPersonEmailInfoFields();
            $('#popContactPersonEmailInfo').show();
        })
        .on('click', '#btnCancelContactPersonEmailAddress', function () {
            $('#popContactPersonEmailInfo').hide();
            ContactPersonEmailState = '';
        })
        .on('click', '#btnSaveContactPersonEmailAddress', function () {
            if (IsEmail($.trim($("#txtContactPersonEmailAdd").val())) == false) {
                $('#spanerrorContactPersonEmailAddressValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            if ($.trim($("#txtContactPersonEmailAdd").val()) == "") {
                $('#spanerrorContactPersonEmailAddress').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            if ($('#selContactPersonEmailType').val() == "") {
                $('#spanerrorContactPersonEmailType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            intContactPersonEmailInfoCtr = intContactPersonEmailInfoCtr + 1;

            if (ContactPersonEmailState == "") {
                arrContactPersonEmailInfoList.push({
                    "EmailRankNo": intContactPersonEmailInfoCtr,
                    "EmailAddID": $('#txtContactPersonEmailID').val(),
                    "EmailAdd": $('#txtContactPersonEmailAdd').val(),
                    "EmailType": $('#selContactPersonEmailType').val(),
                    "EmailState": "Add",
                    "IsNew": 1,
                })
            }
            else {
                arrContactPersonEmailInfoList[sessionStorage.Emailindex].EmailRankNo = ContactPersonEmailRankNo;
                arrContactPersonEmailInfoList[sessionStorage.Emailindex].EmailAddID = $('#txtContactPersonEmailID').val();
                arrContactPersonEmailInfoList[sessionStorage.Emailindex].EmailAdd = $('#txtContactPersonEmailAdd').val();
                arrContactPersonEmailInfoList[sessionStorage.Emailindex].EmailType = $('#selContactPersonEmailType').val();
                arrContactPersonEmailInfoList[sessionStorage.Emailindex].EmailState = sessionStorage.EmailState;
                arrContactPersonEmailInfoList[sessionStorage.Emailindex].IsNew = sessionStorage.EmailIsNew;
            }

            AddContactPersonEmailInfo();
            ContactPersonEmailState = "";
        })
        .on('click', '#btnCancelAddingContact', function () {
            if (sessionStorage.LibraryName == 'Customer / Client List') {
                AddBreadCrumb({
                    text: 'Customer / Client List', Click: function () {
                        $('#pageCustomerMainDashboard').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
                $('#pageCustomerMainDashboard').pageshow();
                $('#divCompanyProfile').show();
                isFromColdCallPage = false;
            } else {
                location.hash = 'LibraryPage';
            }
        })
        .on('click', '#btnSaveAddingContact', function () {
            SaveNewOrgProspect();
        })
        .on('click', '#btnProspectCompanyIndustry', function () {
            BrowsePopup('LibraryIndustryClassification', 'Business Industry',
                    [
                        {
                            DisplayText: [{ Column: 'Class' }],
                            Click: function (e) {
                                $('#txtBusIndustryID').val(e.ID);
                                $('#btnProspectCompanyIndustry').text(e.Class);
                            }
                        }
                    ]
                )
        })

        .on('click', '#btnPersonBirthDate', function () {
            SingleCalendar({
                Click: function (e) {
                    $('#btnPersonBirthDate').text(e);
                }
            })
        })
        .on('click', '#btnAddPersonTelephone', function () {
            ClearPersonTelInfo();
            $('#popPersonTelInfo').show();
        })
        .on('click', '#btnCancelPersonTelephone', function () {
            $('#popPersonTelInfo').hide();
            PersonTelState = '';
        })
        .on('click', '#btnPersonTelAreaCode', function () {
            BrowsePopup('LibraryAreaCodes', 'Area Codes',
                [
                    {
                        DisplayText: [{ Column: 'Area' }, { Label: 'Code', Column: 'CallingCode' }],
                        Click: function (e) {
                            $('#txtPersonTelAreaCodeID').val(e.ID);
                            $('#txtPersonAreaCode').text(e.Area + ' - ' + e.CallingCode)
                        }
                    }
                ]
            )
        })
        .on('keydown', '#txtPersonTelephoneNum', function (e) {
            if (!NumberDotOnly($(this), e)) {
                return;
            }
        })
        .on('click', '#btnSavePersonTelephone', function () {
            if ($('#txtPersonAreaCode').text() == "Select") {
                $('#spanerrorPersonAreaCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            if ($('#txtPersonTelephoneNum').val() == "") {
                $('#spanerrorPersonTelephoneNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            //if ($('#txtPersonTelephoneNum').val().length != 7) {
            //    $('#spanerrorPersonTelephoneNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
            //    return false;
            //}

            if ($('#selPersonTelType').val() == "") {
                $('#spanerrorPersonTelType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }
            intPersonTelInfoCtr = intPersonTelInfoCtr + 1;

            if (PersonTelState == "") {
                arrPersonTelInfoList.push({
                    "TelRankNo": intTelInfoCtr,
                    "CountryCodeID": $('#txtPersonTelCountryID').val(),
                    "CountryCode": $('#txtPersonCountryCode').text(),
                    "AreaCode": $('#txtPersonAreaCode').text(),
                    "AreaCodeID": $('#txtPersonTelAreaCodeID').val(),
                    "TelephoneNo": $('#txtPersonTelephoneNum').val(),
                    "ExtensionNo": $('#txtPersonTelephoneExt').val(),
                    "ContactType": $('#selPersonTelType').val(),
                    "TelState": "Add",
                    "IsNew": 1,
                    "TelID": $('#txtPersonTelID').val()
                })
            }
            else {
                arrPersonTelInfoList[sessionStorage.Telindex].TelRankNo = TelRankNo;
                arrPersonTelInfoList[sessionStorage.Telindex].CountryCodeID = $('#txtPersonTelCountryID').val();
                arrPersonTelInfoList[sessionStorage.Telindex].CountryCode = $('#txtPersonCountryCode').text();
                arrPersonTelInfoList[sessionStorage.Telindex].AreaCodeID = $('#txtPersonTelAreaCodeID').val();
                arrPersonTelInfoList[sessionStorage.Telindex].AreaCode = $('#txtPersonAreaCode').text();
                arrPersonTelInfoList[sessionStorage.Telindex].TelephoneNo = $('#txtPersonTelephoneNum').val();
                arrPersonTelInfoList[sessionStorage.Telindex].ExtensionNo = $('#txtPersonTelephoneExt').val();
                arrPersonTelInfoList[sessionStorage.Telindex].ContactType = $('#selPersonTelType').val();
                arrPersonTelInfoList[sessionStorage.Telindex].TelState = sessionStorage.TelState;
                arrPersonTelInfoList[sessionStorage.Telindex].IsNew = sessionStorage.TelIsNew;
                arrPersonTelInfoList[sessionStorage.Telindex].TelID = $('#txtPersonTelID').val();
            }

            AddPersonTelInfo();
            ClearPersonTelInfo();
            PersonTelState = "";
        })
        .on('click', '#btnAddPersonMobile', function () {
            ClearPersonMobileInfo();
            $('#popPersonMobileInfo').show();
        })
        .on('click', '#btnCancelPersonMobile', function () {
            $('#popPersonMobileInfo').hide();
            PersonMobileState = '';
        })
        .on('click', '#btnPersonMobileTelCode', function () {
            BrowsePopup('LibraryTelcoPrefixes', 'Telco Codes',
                [
                    {
                        DisplayText: [{ Column: 'Prefix' }, { Column: 'TelCo' }],
                        Click: function (e) {
                            $('#txtPersonTelCoPrefixID').val(e.ID);
                            $('#txtPersonTelCode').text(e.TelCo + ' - ' + e.Prefix)
                        }
                    }
                ]
            )
        })
        .on('keydown', '#txtPersonMobileNo', function (e) {
            if (!NumberDotOnly($(this), e)) {
                return;
            }
        })
        .on('click', '#btnSavePersonMobile', function () {
            //if ($('#txtPersonTelCode').text() == "Select") {
            //    $('#spanerrorPersonMobileTelCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
            //    return false;
            //}

            if ($('#txtPersonMobileNo').val() == "") {
                $('#spanerrorPersonMobileNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            //if ($('#txtPersonMobileNo').val().length != 7) {
            //    $('#spanerrorPersonMobileNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
            //    return false;
            //}

            if ($('#selPersonMobileType').val() == "") {
                $('#spanerrorPersonMobileType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            intPersonMobileInfoCtr = intPersonMobileInfoCtr + 1;

            if (PersonMobileState == "") {
                arrPersonMobileInfoList.push({
                    "MobileRankNo": intMobileInfoCtr,
                    "CountryCodeID": $('#txtPersonMobileCountryID').val(),
                    "CountryCode": $('#txtPersonMobileCountryCode').text(),
                    //"TelCode": $('#txtPersonTelCode').text(),
                    //"TelCodeID": $('#txtPersonTelCoPrefixID').val(),
                    "MobileNo": $('#txtPersonMobileNo').val(),
                    "ContactType": $('#selPersonMobileType').val(),
                    "MobileID": $('#txtPersonMobileID').val(),
                    "MobileState": "Add",
                    "IsNew": 1,
                    "MobileNos": '+' + $('#txtPersonMobileCountryCode').text() + $('#txtPersonTelCode').text() + $('#txtPersonMobileNo').val()
                })
            }
            else {
                arrPersonMobileInfoList[sessionStorage.Mobileindex].MobileRankNo = MobileRankNo;
                arrPersonMobileInfoList[sessionStorage.Mobileindex].CountryCodeID = $('#txtPersonMobileCountryID').val();
                arrPersonMobileInfoList[sessionStorage.Mobileindex].CountryCode = $('#txtPersonMobileCountryCode').text();
                //arrPersonMobileInfoList[sessionStorage.Mobileindex].TelCodeID = $('#txtPersonTelCoPrefixID').val();
                //arrPersonMobileInfoList[sessionStorage.Mobileindex].TelCode = $('#txtPersonTelCode').text();
                arrPersonMobileInfoList[sessionStorage.Mobileindex].MobileNo = $('#txtPersonMobileNo').val();
                arrPersonMobileInfoList[sessionStorage.Mobileindex].ContactType = $('#selPersonMobileType').val();
                arrPersonMobileInfoList[sessionStorage.Mobileindex].MobileState = sessionStorage.MobileState;
                arrPersonMobileInfoList[sessionStorage.Mobileindex].IsNew = sessionStorage.MobileIsNew;
                arrPersonMobileInfoList[sessionStorage.Mobileindex].MobileNos = '+' + $('#txtPersonMobileCountryCode').text() + $('#txtPersonTelCode').text() + $('#txtPersonMobileNo').val()
            }

            AddPersonMobileInfo();
            PersonMobileState = "";
            ClearPersonMobileInfo();
        })
        .on('click', '#btnAddPersonEmail', function () {
            ClearPersonEmailInfoFields();
            $('#popPersonEmailInfo').show();
        })
        .on('click', '#btnCancelPersonEmailAddress', function () {
            $('#popPersonEmailInfo').hide();
            PersonEmailState = '';
        })
        .on('click', '#btnSavePersonEmailAddress', function () {
            if (IsEmail($.trim($("#txtPersonEmailAdd").val())) == false) {
                $('#spanerrorPersonEmailAddressValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            if ($.trim($("#txtPersonEmailAdd").val()) == "") {
                $('#spanerrorPersonEmailAddress').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            if ($('#selPersonEmailType').val() == "") {
                $('#spanerrorPersonEmailType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            intPersonEmailInfoCtr = intPersonEmailInfoCtr + 1;

            if (PersonEmailState == "") {
                arrPersonEmailInfoList.push({
                    "EmailRankNo": intPersonEmailInfoCtr,
                    "EmailAddID": $('#txtPersonEmailID').val(),
                    "EmailAdd": $('#txtPersonEmailAdd').val(),
                    "EmailType": $('#selPersonEmailType').val(),
                    "EmailState": "Add",
                    "IsNew": 1
                })
            }
            else {
                arrPersonEmailInfoList[sessionStorage.Emailindex].EmailRankNo = EmailRankNo;
                arrPersonEmailInfoList[sessionStorage.Emailindex].EmailAddID = $('#txtPersonEmailID').val();
                arrPersonEmailInfoList[sessionStorage.Emailindex].EmailAdd = $('#txtPersonEmailAdd').val();
                arrPersonEmailInfoList[sessionStorage.Emailindex].EmailType = $('#selPersonEmailType').val();
                arrPersonEmailInfoList[sessionStorage.Emailindex].EmailState = sessionStorage.EmailState;
                arrPersonEmailInfoList[sessionStorage.Emailindex].IsNew = sessionStorage.EmailIsNew;
            }

            AddPersonEmailInfo();
            PersonEmailState = "";
        })
        .on('click', '#btnCancelPersonAddingContact', function () {
            if (sessionStorage.LibraryName == 'Customer / Client List') {
                AddBreadCrumb({
                    text: 'Customer / Client List', Click: function () {
                        $('#pageCustomerMainDashboard').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
                $('#pageCustomerMainDashboard').pageshow();
                isFromColdCallPage = false;
            } else {
                location.hash = 'LibraryPage';
            }
        })
        .on('click', '#btnSavePersonAddingContact', function () {
            SaveNewPersonProspect();
        })
        .on('click', '#btnAddOrgProspect', function () {
            BreadCrumbs('.ItemsBreadCrumbs',
                    [
                        { Text: 'Lead Generation', Click: function () { $('#pageMainDashboard').pageshow() } },
                        { Text: 'New Company' }
                    ]
                )
            ClearOrgProspectVariablesFields();
            $('#pageActions').pageshow();
            $('#pageProfile').pageshow();
            $('#pageProfileOrg').pageshow();
            $('#tabRevenue').appendTo($('#divCompanyProfile'));
            $('#listAction').find('li > a:not(:first)').addClass('disabled-menu');
            isAddNewProspect = true;
            $(window).scrollTop(0);
            $('#divTabCompany ul > li:first').click()
            $('#btnPageProfile').click()
        })
        .on('click', '#btnAddPeopleProspect', function () {
            BreadCrumbs('.ItemsBreadCrumbs',
                    [
                        { Text: 'Lead Generation', Click: function () { $('#pageMainDashboard').pageshow() } },
                        { Text: 'New Person' }
                    ]
                )
            ClearPersonProspectVariablesFields();
            $('#pageActions').pageshow();
            $('#pageProfile').pageshow();
            $('#pageProfilePeople').pageshow();
            $('#tabRevenue').appendTo($('#pageProfilePeople'));
            $('#listAction').find('li > a:not(:first)').addClass('disabled-menu');
            isAddNewProspect = true;
            $(window).scrollTop(0);
            $('#divTabPerson ul > li:first').click()
            $('#btnPageProfile').click()
        })
        .on('click', '#btnInteractionHistoryDetailsOk', function () {
            $('#popInteractionHistoryDetails').hide();
        })
        .on('click', '#btnInteractionHistoryBack', function () {
            if (isFromColdCallPage == true) {
                BreadCrumbs('.ItemsBreadCrumbs',
                    [
                        { Text: 'Lead Generation', Click: function () { $('#pageMainDashboard').pageshow() } },
                        { Text: 'Cold Call' }
                    ]
                    )
                $('#btnPageColdCall').click();
            }
            else {
                BreadCrumbs('.ItemsBreadCrumbs',
                    [
                        { Text: 'Lead Generation' }
                    ]
                    )
            }
            $(BackPage).pageshow();
            isFromColdCallPage = false;
        })
        .on('click', '.btnCloseToolTip', function () {
            $('.tooltip-window').hide()
        })
        .on('click', '#btnAddToDo', function () {
            ClearTodoFields();
            $('#popNewToDo').show();
        })
        .on('click', '#btnCancelToDo', function () {
            $('#popNewToDo').hide();
        })
        .on('click', '#btnToDoSetDate, #btnEditToDoSetDate', function () {
            var selector = $(this)[0].id;
            SingleCalendar({
                ClearButton: 'Clear Date',
                Placeholder: 'Set Date',
                SetDate: (todoSetDate == '' ? new Date() : todoSetDate),
                Click: function (e) {
                    todoSetDate = e;
                    if (new Date(e) == "Invalid Date") {
                        todoSetDate = '';
                        $('#btnEditToDoSetDate').text('--');
                    }
                    else {
                        $('#btnEditToDoSetDate').text($.format.date(new Date(e), 'MMM dd, yyyy'));
                    }
                    if (selector == "btnEditToDoSetDate") {
                        $('#popToDo').popup('open');
                    }
                },
                Cancel: function () {
                    if (selector == "btnEditToDoSetDate") {
                        $('#popToDo').popup('open');
                    }
                }
            })
        })
        .on('click', '#btnOkToDo', function () {
            PreSaveToDo();
        })
        .on('keydown', '#txtToDo', function (e) {
            if (e.keyCode == 13) {
                PreSaveToDo();
            }
        })
        .on('click', '#btnCancelEditToDo', function () {
            todoSetDate = '';
            $('#popToDo').popup('close');
        })
        .on('click', '#btnOkEditToDo', function () {
            $('#popToDo').popup('close');

            arrTodoList[sessionStorage.ToDoIndex].TodoRankNo = TodoRankNo;
            arrTodoList[sessionStorage.ToDoIndex].ClientID = $('.txtActionClientName').data().ID;
            arrTodoList[sessionStorage.ToDoIndex].ToDoID = $('#txtToDoID').val();
            arrTodoList[sessionStorage.ToDoIndex].ToDo = $('#txtEditToDo').val();
            arrTodoList[sessionStorage.ToDoIndex].ToDoDueDate = todoSetDate;
            arrTodoList[sessionStorage.ToDoIndex].ToDoOrder = (intTodoInfoCtr == 0 ? 1 : intTodoInfoCtr + 1);
            arrTodoList[sessionStorage.ToDoIndex].ToDoFinish = sessionStorage.ToDoIsFinish;
            arrTodoList[sessionStorage.ToDoIndex].State = sessionStorage.ToDoState;
            arrTodoList[sessionStorage.ToDoIndex].IsNew = sessionStorage.ToDoIsNew;
            arrTodoList[sessionStorage.ToDoIndex].IsOrg = ($('.txtActionClientName').data().ProspectType == "Org" ? 1 : 0)

            todoSetDate = '';
            SaveToDo(false);
        })

            // LEFT MENU CLICK EVENT
        .on('click', '#btnPageProfile', function () {
            var e = JSON.parse(sessionStorage.ClientInfo);
            menuActionToSave = 'profile';
            if (isAddNewProspect) {
                return;
            }
            AddBreadCrumb({
                text: e.ClientName + ' Profile', Click: function () {
                }, UpdateLastchild: true
            }, '.ItemsBreadCrumbs')
            if (e.ProspectType == "Org") {
                GetProspectDetails(e.ID);
                $('#pageProfileOrg').pageshow();
                if ($('#tabPersonRevenue').length > 0) {
                    $('#tabPersonRevenue').appendTo($('#divCompanyProfile'));
                    $('#tabPersonRevenue').attr('id', 'tabRevenue');
                } else {
                    $('#tabRevenue').appendTo($('#divCompanyProfile'));
                }
                $('#divTabCompany ul > li:first').click()
            }
            else {
                GetProspectDetailsPeople(e.ID);
                $('#pageProfilePeople').pageshow();
                if ($('#tabRevenue').length > 0) {
                    $('#tabRevenue').appendTo($('#pageProfilePeople'));
                    $('#tabRevenue').attr('id', 'tabPersonRevenue');
                } else {
                    $('#tabPersonRevenue').appendTo($('#pageProfilePeople'));
                }
                $('#divTabPerson ul > li:first').click()
            }

            $(window).scrollTop(0)
            EditingMode();
        })
        .on('click', '#btnPageColdCall', function (v) {
            var e = JSON.parse(sessionStorage.ClientInfo);

            if ($(this).hasClass('disabled-menu')) {
                return;
            }

            menuActionToSave = 'coldcall';

            if (e.Telephone == null && e.Mobile == null && (e.ContactPerson == "" || e.ContactPerson == undefined)) {
                NewMessageBox('No contact number specified. Do you want to add contact number?', 'Cold Call',
                    [
                        {
                            Text: 'Yes',
                            Click: function () {
                                $('#pageActions').pageshow();
                                $('#btnPageProfile').click();
                            }
                        },
                        {
                            Text: 'No'
                        }
                    ]
                );
                v.stopPropagation(); // to avoid showing cold call page
                return;
            }

            $('#pageActions').pageshow();

            if (isFromColdCallPage == false) {
                BreadCrumbs('.ItemsBreadCrumbs',
                [
                    { Text: 'Lead Generation', Click: function () { $('#pageMainDashboard').pageshow(); } },
                    { Text: 'Cold Call' }
                ]
                )

                $('.txtActionClientName').text(e.ClientName).removeData().data(e);
                $('#txtCallNotes').css('height', '100px').css('min-height', '100px');
                $('#divColdCallContactPerson').show();
                if (e.ProspectType == "Org") {
                    GetContactDetailsOrg(e);
                }
                else {
                    GetContactDetailsPeople(e);
                }

                if (!isCallNoteSaveAsDraft) {
                    $('#txtCallNotes').val('');
                }
                (e.ProspectType == "Org" ? $('#divColdCallContactPerson').show() : $('#divColdCallContactPerson').hide())
            }

            BackPage = 'pageMainDashboard';
            $('#divtooltipNotes').hide();
            EditingMode();
        })
        .on('click', '#btnPageToDo', function () {
            var e = JSON.parse(sessionStorage.ClientInfo);
            if ($(this).hasClass('disabled-menu')) {
                return;
            }

            BreadCrumbs('.ItemsBreadCrumbs',
                [
                    {
                        Text: 'Lead Generation', Click: function () {
                            $('#pageMainDashboard').pageshow();
                        }
                    },
                    { Text: e.ClientName + ' To-Do List' }
                ]
            );
            menuActionToSave = '';
            $('.txtActionClientName').text(e.ClientName).removeData().data(e);
            GetClientToDo(e.ID);
            EditingMode();
            $('#divtooltipNotes').hide();
        })
        .on('click', '#btnPageSMS', function () {
            var e = JSON.parse(sessionStorage.ClientInfo);
            if (e.MobileNos == undefined && e.ContactPerson == null) {
                NewMessageBox('No mobile number found', 'Send SMS');
                return false;
            }

            if ($(this).hasClass('disabled-menu')) {
                return;
            }

            BreadCrumbs('.ItemsBreadCrumbs',
                [
                    {
                        Text: 'Lead Generation', Click: function () {
                            $('#pageMainDashboard').pageshow();
                        }
                    },
                    { Text: 'Send SMS to ' + e.ClientName }
                ]
            );

            if (e.MobileNos == undefined && e.ContactPerson == null) {
                NewMessageBox('No mobile number found', 'Send SMS');
                return false;
            }
            menuActionToSave = '';
            arrSMSRecipient = [];
            GetMobileNoBySelectedRecipient(e);
            EditingMode();
            $('#txtSMSMessageAction').val('').height('100px').focus();
            $('#charNum2').text('Characters left: 160');
            $('#divtooltipNotes').hide();
        })
        .on('click', '#btnPageInteractionHistory', function () {
            var e = JSON.parse(sessionStorage.ClientInfo);
            if ($(this).hasClass('disabled-menu')) {
                return;
            }
            BreadCrumbs('.ItemsBreadCrumbs',
                [
                    {
                        Text: 'Lead Generation', Click: function () {
                            $('#pageMainDashboard').pageshow();
                        }
                    },
                    { Text: 'Interaction History of ' + e.ClientName }
                ]
            );
            menuActionToSave = '';
            BackPage = '#pageMainDashboard';
            $('.txtActionClientName').text(e.ClientName);
            GetInteractionHistory(e.ID);
            EditingMode();
            $('#divtooltipNotes').hide();
        })
        .on('click', '#btnPageTakeANote', function () {
            var e = JSON.parse(sessionStorage.ClientInfo);
            if ($(this).hasClass('disabled-menu')) {
                return;
            }
            BreadCrumbs('.ItemsBreadCrumbs',
                [
                    {
                        Text: 'Lead Generation', Click: function () {
                            $('#pageMainDashboard').pageshow();
                        }
                    },
                    { Text: 'Take a note for ' + e.ClientName }
                ]
            );
            $('#txtTakeANote').val();
            $('.txtActionClientName').text(e.ClientName);
            if ($.trim($('#txtTakeANote').val()) == "") {
                $('#txtTakeANote').height('300px')
            }
            GetTakeANote(e.ID);
            EditingMode();
        })
        .on('click', '#btnPageScheduleAnAction', function () {
            var e = JSON.parse(sessionStorage.ClientInfo);
            if ($(this).hasClass('disabled-menu')) {
                return;
            }
            BreadCrumbs('.ItemsBreadCrumbs',
                [
                    {
                        Text: 'Lead Generation', Click: function () {
                            $('#pageMainDashboard').pageshow();
                        }
                    },
                    { Text: 'Schedule for ' + e.ClientName }
                ]
            );
            menuActionToSave = '';
            $('.txtActionClientName').text(e.ClientName);
            $('#pageSchedule').pageshow();
            $('#divCalendar').customcalendar();
            GetListOfTeam();
            GetSchedule();
        })
        .on('click', '#btnPageInternalSalesMarketing', function () {
            var e = JSON.parse(sessionStorage.ClientInfo);
            if ($(this).hasClass('disabled-menu')) {
                return;
            }
            BreadCrumbs('.ItemsBreadCrumbs',
                [
                    {
                        Text: 'Lead Generation', Click: function () {
                            $('#pageMainDashboard').pageshow();
                        }
                    },
                    { Text: 'Sales & Marketing Info of ' + e.ClientName }
                ]
            );

            $('#pageInternalSalesMarketing').pageshow();
        })
        // END LEFT MENU

        .on('change', '#selCity', function (e) {
            var val = $.trim($(this).val());
            var ctrRow = 0;
            $('#tblProspectbody').find('tr').show().each(function () {
                if ($(this).data().City != val) {
                    $(this).hide();
                }
                else if ($(this).data().City == val) {
                    ctrRow = ctrRow + 1
                }

                if (val == "" && $(this).data().City == undefined) {
                    $(this).show();
                    ctrRow = ctrRow + 1
                }
            })
            $('#divCountRow').text('No. of Rows: ' + ctrRow);
        })
        .on('click', '#btnAddPersonOrgAffiliation', function () {
            BrowsePopup('LibraryProspectOrg', 'Organization',
                [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'CompanyName' }],
                        Validation: function (e) {
                            var isExist = false;
                            $('#lstPersonOrgAffiliation li').each(function () {
                                if ($(this).find('a').data().OrgID == e.OrgID) {
                                    isExist = true;
                                }
                            })

                            if (isExist) {
                                return '<span style="color:#F00;">' + e.CompanyName + ' already exist.</span>';
                            }

                            return true;
                        },
                        Click: function (e) {
                            e.State = 'Add';
                            e.IsNew = 1;
                            arrPersonOrgAffList.push(e)
                            AddPersonOrgAffiliation();
                        }
                    }
                ], { IsMcr: true }
            )
        })
        .on('click', '#btnAddAffiliatedPerson', function () {
            BrowsePopup('LibraryProspectPeople', 'People',
                [
                    {
                        AddButton: [{
                            Text: '+ Add New Person',
                            Click: function () {
                                $('#btnAddPeopleProspect').click();
                            }
                        }],
                        AutoSearch: true,
                        DisplayText: [{ Column: 'PeopleName' }],
                        Validation: function (e) {
                            var isExist = false;
                            $('#lstAffiliatedPerson li').each(function () {
                                if ($(this).find('a').data().PeopleID == e.PeopleID) {
                                    isExist = true;
                                }
                            })

                            if (isExist) {
                                return '<span style="color:#F00;">' + e.PeopleName + ' already exist.</span>';
                            }

                            return true;
                        },
                        Click: function (e) {
                            e.State = 'Add';
                            e.IsNew = 1;
                            arrOrgAffPersonList.push(e)
                            AddAffiliatedPerson();
                        }
                    }
                ], { IsMcr: true }
            )
        })
        .on('click', '#btnAddOrgAffiliation', function () {
            BrowsePopup('LibraryProspectOrg', 'Organization',
                [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'CompanyName' }],
                        Validation: function (e) {
                            var isExist = false;
                            $('#lstOrgAffiliation li').each(function () {
                                if ($(this).find('a').data().OrgID == e.OrgID) {
                                    isExist = true;
                                }
                            })

                            if (isExist) {
                                return '<span style="color:#F00;">' + e.CompanyName + ' already exist.</span>';
                            }

                            return true;
                        },
                        Click: function (e) {
                            e.State = 'Add';
                            e.IsNew = 1;
                            arrOrgAffList.push(e)
                            AddOrgAffiliation();
                        }
                    }
                ], { IsMcr: true }
            )
        })
        .on('keyup', '.txtSearchProspect', function (e) {
            $(window).scrollTop(0);
            if (e.keyCode == 13) {
                datatable.search($(this).val()).draw();
            }
            $('.txtSearchProspect').val($(this).val())
        })
        .on('submit', '#Customerformupload', function (e) {
            e.preventDefault();
            var formData = new FormData($(this)[0]);
            NewMessageBox('This might take a few moments. Please wait while the system uploading your contacts ...', 'Message');
            setTimeout(function () {
                $.ajax({
                    url: '/OrganizationShort/UploadContacts?OrgID=' + sessionStorage.OrgID,
                    type: 'POST',
                    data: formData,
                    async: false,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (vData) {
                        $('#CustomerobjFile').val('');
                        if (vData == "Saved") {
                            WebToast('Customers has been successfully uploaded', 'top');
                            ShowProspectList();
                            NewMessageBoxClose();
                        }
                        else {
                            NewMessageBox(vData, 'ERROR');
                        }
                    }
                });
            }, 1000)
        })
        .on('click', '#btnAddExcelProspect', function () {
            $('#CustomerobjFile').trigger('click');
        })
        .on('change', '#CustomerobjFile', function (e) {
            $('#txtCustomerUploadExcelOrgID').val(sessionStorage.OrgID)
            $('#Customerformupload').submit();
        })
        .on('click', '#btnBulkAction', function () {
            PopupButtonList('', 'Bulk Actions',
                [
                    {
                        Text: 'Send Bulk SMS',
                        Click: function () {
                            isSendingBulkMsg = true;
                            // $('.bulk').show();
                            // $('.spanbtnSMS').show();
                            $('#charNum').text('Characters left: 160');
                            $('#txtRecipientsCount').text(0);
                            $('#popSMS').show();
                            $('#txtSMSMessage').val('').focus().height('100px');
                            arrSMSRecipient = [];
                            DisplayListOfReceipient();
                        }
                    },
                    {
                        Text: 'View SMS Logs',
                        Click: function () {
                            $('#popSMSLogs').show();
                            GetSmSUsage();
                        }
                    }
                ]
            )
        })
        .on('click', '#btnCancelSMS', function () {
            $('#popSMS').hide();
        })
        .on('click', '#btnSendSMS', function () {
            SendSMS();
        })
        .on('keyup', '#txtSMSMessage', function (e) {
            var len = $(this).val().length;
            if (len >= 161) {
                $(this).val($(this).val().substring(0, 160))
            } else {
                $('#charNum').text('Characters left: ' + (160 - len));
            }
        })
        .on('click', '#btnOkSMSRecipient', function () {
            $('#popSMSRecipient').hide();
            DisplayListOfReceipient();
        })
        .on('click', '#btnCancelSMSSelectedReceipient', function () {
            isSendingBulkMsg = false;
            arrSMSRecipient = [];
            ctrUnknownNumber = 1;
            $('#listofRecipient').html('');
            $('#listSelectedReceipient').empty();
            $('#txtSMSMessage').val('');
            $('input[name="chbbulk"]').prop('checked', false).checkboxradio('refresh');
            $('.bulk, .spanbtnSMS, #popSMS, #popSMSRecipient').hide();
        })
        .on('click', '#btnViewSMSSelectedReceipient, #btnViewRecipientsDetails', function () {
            $('#popSMSRecipient').show();
            DisplayListOfSelectedReceipient();
        })
        .on('click', '#btnComposeSMSSelectedReceipient', function () {
            $('#popSMS').show();
            DisplayListOfReceipient();
        })
        .on('click', '#btnSMSLogs', function () {
            $('#popSMSLogs').show();
            GetSmSUsage();
        })
        .on('change', '#chbbulk', function () {
            var chbBulk = $(this)
            $('#tblProspectbody tr').each(function () {
                if ($(this).css('display') != 'none') {
                    if (chbBulk.is(':checked')) {
                        $(this).find('td:eq(0)').find('input[type="checkbox"]').prop('checked', true).checkboxradio('refresh');
                    }
                    else {
                        $(this).find('td:eq(0)').find('input[type="checkbox"]').prop('checked', false).checkboxradio('refresh');
                    }
                    AddSelectedRecipient($(this).find('td:eq(0)').find('input[type="checkbox"]').data(), chbBulk.is(':checked'))
                }
            })
            DisplayListOfReceipient();
        })
        .on('click', '#btnBackToTop', function () {
            $("html, body").animate({ scrollTop: 0 }, 700);
        })
        .on('click', '#btnSearchContacts', function () {
            $('#popSearchContacts').show();
            $('#txtSearchContacts').val('').focus();
            SearchClientContact('');
        })
        .on('click', '#btnAddNewNumber', function () {
            $('#btnNewNumberTelcoPrefix').text('Please select');
            $('#txtNewNumber').val('');
            $('#popNewNumber').popup('open')
        })
        .on('click', '#btnOkNewNumber', function () {
            var newNumber = '+63' + $('#btnNewNumberTelcoPrefix').text() + $('#txtNewNumber').val();
            arrSMSRecipient.push({
                ID: NewID(),
                Name: newNumber,
                Number: [newNumber]
            })
            ctrUnknownNumber = ctrUnknownNumber + 1;
            DisplayListOfReceipient();
            $('#popNewNumber').popup('close');
        })
        .on('click', '#btnCancelNewNumber', function () {
            $('#popNewNumber').popup('close');
        })
        .on('click', '#btnNewNumberTelcoPrefix', function () {
            BrowsePopup('LibraryTelcoPrefixes', 'Telco Codes',
                [
                    {
                        AddButton: [{
                            Text: 'Back',
                            Click: function () {
                                $('#popNewNumber').popup('open');
                                $('#txtNewNumber').focus();
                            }
                        }],
                        DisplayText: [{ Column: 'Prefix' }, { Column: 'TelCo' }],
                        Click: function (e) {
                            $('#popNewNumber').popup('open');
                            $('#btnNewNumberTelcoPrefix').text(e.Prefix);
                            $('#txtNewNumber').focus();
                        }
                    }
                ]
            )
        })
        .on('keydown', '#txtNewNumber', function (e) {
            if (!NumberDotOnly($(this), e)) {
                return;
            }
        })
        .on('keyup', '#txtSMSMessageAction', function (e) {
            var len = $(this).val().length;
            if (len >= 161) {
                $(this).val($(this).val().substring(0, 160))
            } else {
                $('#charNum2').text('Characters left: ' + (160 - len));
            }
        })
        .on('click', '#btnActionSendSMS', function () {
            if ($.trim($('.txtActionClientName').text()) == '') {
                WebToast('Please enter recipient');
                return;
            }

            if ($.trim($('#txtSMSMessageAction').val()) == '') {
                WebToast('Please write your message');
                return;
            }
            SendSMS();
        })
        .on('click', '#btnDownloadExcelProspect', function () {
            window.location = GetPathFormat("/CRMDashboard/DownloadContactTemplate");
            return;
            NewMessageBox('To add new contacts using the contact template, please do the following: <br><br>' +
                        '1. To add Organization: <br> ' +
                            '<p style="text-indent:30px;">a. Fill the <b>Organization Name</b> column and leave blank column First name, Middle name and Last name.</p>' +
                        '2. To add Person: <br>' +
                            '<p style="text-indent:30px;">a. Fill the <b>First Name</b>, <b>Middle Name</b> and <b>Last Name</b> column and leave blank column Organization Name.</p>' +
                        'Click Download Now button to continue', 'Note',
                        [
                            {
                                Text: 'Download Now',
                                Click: function () {
                                    window.location = GetPathFormat("/CRMDashboard/DownloadContactTemplate");
                                }
                            },
                            {
                                Text: 'Cancel'
                            }
                        ]
                )
        })
        .on('keydown', '#txtSearchContacts', function (e) {
            if (e.keyCode == 13) {
                SearchClientContact($(this).val())
            }
        })
        .on('click', '#btnAddNote', function () {
            $('#pageAddNote').pageshow();
            $('#txtTakeANote').val('');
            sessionStorage.StateNote = "add";
            sessionStorage.StateNoteID = '';
        })
        .on('click', '#btnCancelTakeANote', function () {
            if (isEditingANote) {
                NewMessageBox('Do you want to save changes?', 'Message', [
                    {
                        Text: 'Yes',
                        Click: function () {
                            $('#btnSaveTakeANote').click();
                        }
                    },
                    {
                        Text: 'No',
                        Click: function () {
                            $('#pageListOfNote').pageshow();
                        }
                    }
                ])
            } else {
                $('#pageListOfNote').pageshow();
            }
        })
        .on('click', '#btnBackTakeANote, #btnBackToDo, #btnBackCalendar', function () {
            BreadCrumbs('.ItemsBreadCrumbs', [{ Text: 'Lead Generation' }]);
            $('#pageMainDashboard').pageshow();
        })
        .on('click', '#btnSaveTakeANote', function () {
            if ($.trim($('#txtTakeANote').val()) == "") {
                WebToast('Please type your note');
                return false;
            }
            SaveTakeANote(sessionStorage.StateNote, sessionStorage.StateNoteID);
        })
        .on('click', '#txtDateTimeFrom', function () {
            SingleCalendar({
                Format: 'MMMM d, yyyy',
                Click: function (e) {
                    var dateFrom = new Date(e + ' ' + $('#btnDateTimeFrom').text());
                    var dateTo = new Date($('#txtDateTimeTo').val() + ' ' + $('#btnDateTimeTo').text());
                    var isIncreaseDate = false;
                    var newHr; var tempDate;
                    $('#txtDateTimeFrom').val(e);

                    if (dateFrom > dateTo) {
                        if (dateFrom.getHours() == 0) {
                            newHr = 24;
                            isIncreaseDate = true;
                        }
                        else {
                            newHr = dateFrom.getHours();
                        }

                        tempDate = new Date($('#txtDateTimeFrom').val() + ' ' + (newHr + 1) + ':' + dateFrom.getMinutes());

                        if (isIncreaseDate) {
                            tempDate = new Date(tempDate.setDate(tempDate.getDate() + 1));
                        }
                    }
                    else if (dateFrom >= dateTo) {
                        tempDate = new Date(dateTo.setHours(dateTo.getHours() + 1))
                    }
                    $('#txtDateTimeTo').val($.format.date(tempDate, 'MMM d, yyyy'));
                    $('#btnDateTimeTo').text($.format.date(tempDate, 'h:mm a'));
                }
            })
        })
        .on('click', '#btnDateTimeFrom', function () {
            CustomSetTime({
                Click: function (e) {
                    var dateFrom = new Date($('#txtDateTimeFrom').val() + ' ' + e.time);
                    var dateTo = new Date($('#txtDateTimeTo').val() + ' ' + $('#btnDateTimeTo').text());
                    var isIncreaseDate = false;
                    var newHr; var tempDate;

                    if (dateFrom > dateTo) {

                        if (dateFrom.getHours() == 0) {
                            newHr = 24;
                            isIncreaseDate = true;
                        }
                        else {
                            newHr = dateFrom.getHours();
                        }

                        tempDate = new Date($('#txtDateTimeFrom').val() + ' ' + (newHr + 1) + ':' + dateFrom.getMinutes());

                        if (isIncreaseDate) {
                            tempDate = new Date(tempDate.setDate(tempDate.getDate() + 1));
                        }
                    }
                    else if (dateFrom >= dateTo) {
                        tempDate = new Date(dateTo.setHours(dateTo.getHours() + 1))
                    }

                    $('#txtDateTimeTo').val($.format.date(tempDate, 'MMM d, yyyy'));
                    $('#btnDateTimeTo').text($.format.date(tempDate, 'h:mm a'));
                    $('#btnDateTimeFrom').text(e.time);
                }
            })
        })
        .on('click', '#txtDateTimeTo', function () {
            SingleCalendar({
                Format: 'MMMM d, yyyy',
                Click: function (e) {
                    var dateFrom = new Date($('#txtDateTimeFrom').val() + ' ' + $('#btnDateTimeFrom').text());
                    var dateTo = new Date(e + ' ' + $('#btnDateTimeTo').text());
                    var isDecreaseDate = false;
                    var newHr; var tempDate;
                    $('#txtDateTimeTo').val($.format.date(new Date(e), 'MMM d, yyyy'));

                    if (dateTo <= dateFrom) {
                        if (dateTo < dateFrom) {
                            if (dateTo.getHours() == 0) {
                                newHr = 24;
                                isDecreaseDate = true;
                            }
                            else {
                                newHr = dateTo.getHours();
                            }

                            tempDate = new Date($('#txtDateTimeTo').val() + ' ' + (newHr - 1) + ':' + dateTo.getMinutes());

                            if (isDecreaseDate) {
                                tempDate = new Date(tempDate.setDate(tempDate.getDate() - 1));
                            }
                        }
                        else if (dateTo <= dateFrom) {
                            tempDate = new Date(dateTo.setHours(dateTo.getHours() - 1))
                        }
                        $('#txtDateTimeFrom').val($.format.date(tempDate, 'MMM d, yyyy'));
                        $('#btnDateTimeFrom').text($.format.date(tempDate, 'h:mm a'));
                    }
                }
            })
        })
        .on('click', '#btnDateTimeTo', function (e) {
            CustomSetTime({
                Click: function (e) {
                    var dateFrom = new Date($('#txtDateTimeFrom').val() + ' ' + $('#btnDateTimeFrom').text());
                    var dateTo = new Date($('#txtDateTimeTo').val() + ' ' + e.time);
                    var isDecreaseDate = false;
                    var newHr; var tempDate;

                    if (dateTo <= dateFrom) {
                        if (dateTo < dateFrom) {
                            if (dateTo.getHours() == 0) {
                                newHr = 24;
                                isDecreaseDate = true;
                            }
                            else {
                                newHr = dateTo.getHours();
                            }

                            tempDate = new Date($('#txtDateTimeTo').val() + ' ' + (newHr - 1) + ':' + dateTo.getMinutes());

                            if (isDecreaseDate) {
                                tempDate = new Date(tempDate.setDate(tempDate.getDate() - 1));
                            }
                        }
                        else if (dateTo <= dateFrom) {
                            tempDate = new Date(dateTo.setHours(dateTo.getHours() - 1))
                        }
                        $('#txtDateTimeFrom').val($.format.date(tempDate, 'MMM d, yyyy'));
                        $('#btnDateTimeFrom').text($.format.date(tempDate, 'h:mm a'));
                    }
                    $('#btnDateTimeTo').text(e.time);
                }
            })
        })
        .on('click', '#btnSaveSchedAnAction', function () {
            if ($.trim($('#txtSchedAnActionSubject').val()) == "") {
                NewMessageBox('Please type your subject', 'Schedule An Action');
                return false;
            }

            SaveSchedAnAction();
        })
        .on('change', 'input[name="chbAllDay"]', function () {
            if ($(this).is(':checked')) {
                $('#btnDateTimeFrom, #btnDateTimeTo').parent().hide();
            }
            else {
                $('#btnDateTimeFrom, #btnDateTimeTo').parent().show();
            }
        })
        .on('click', '#btnBackListOfEvents', function () {
            $('#pageSchedule').pageshow();
        })
        .on('click', '#btnBackSchedAnAction', function () {
            ClearSchedAnAction();
            if (isFromListEvent) {
                $('#pageListofEvents').pageshow();
                isFromListEvent = false;
            }
            else {
                $('#pageSchedule').pageshow();
            }
        })
        .on('click', '#btnAddSched', function () {
            ClearSchedAnAction();
            $('#pageNewSched').pageshow();
            sessionStorage.StateSched = 'add';
        })

        .on('keydown', '#txtProspectNoOfEmployees, #txtProspectNoFactories, #txtProspectNoOffices, #txtProspectNoWarehouses, #txtProspectNoStores, #txtProspectNoTrucks', function (e) {
            if (!NumberDotOnly($(this), e)) {
                return;
            }
        })
        .on('keydown', '#txtTakeANote', function (e) {
            if ($.trim($(this).val()) != "") {
                isEditingANote = true;
            }
            else {
                isEditingANote = false;
            }
        })
        .on('keydown', '#txtCallNotes', function (e) {
            if ($.trim($(this).val()) != "") {
                isEditingCallNote = true;
            }
            else {
                isEditingCallNote = false;
            }
        })
        .on('keydown', '#pageProfileOrg input', function () {
            isEditingProfile = true;
        })
        .on('change', '#pageProfileOrg select', function () {
            isEditingProfile = true;
        })
        .on('keydown', '#pageProfilePeople input', function () {
            isEditingProfile = true;
        })
        .on('change', '#pageProfilePeople select', function () {
            isEditingProfile = true;
        })

        .on('click', '#btnAddFilter', function () {
            $('#popFilter').popup('open');
            $('#divBusinessIndustry, #divCompanySize, #divCity, #msgCategory, #msgBusinessIndustry, #msgScope, #msgConditionValue, #msgCity').hide();
        })
        .on('change', '#selCategory', function () {
            var val = $.trim($(this).val());
            $('#divBusinessIndustry, #divCompanySize, #divCity, #msgCategory, #msgBusinessIndustry, #msgScope, #msgConditionValue, #msgCity').hide();

            if (val == "Business Industry") {
                $('#divBusinessIndustry').pageshow();
            }
            else if (val == "Company Size") {
                $('#divCompanySize').pageshow();
            }
            else if (val == "City") {
                $('#divCity').pageshow();
            }

            PopupReposition(); // NewMessageBox.js
        })
        .on('change', '#selScope', function () {
            if ($.trim($(this).val()) == "") {
                $('#msgScope').pageshow();
            }
            else {
                $('#msgScope').hide();
            }
            PopupReposition(); // NewMessageBox.js
        })
        .on('keydown', '#txtConditionValue', function (e) {
            if (!NumberDotOnly($(this), e)) {
                return;
            }
        })
        .on('change', '#selFilterCompanySize', function () {
            if ($.trim($(this).val()) == "ScopeOfOperations") {
                $('#divScope').pageshow();
            }
            else if ($.trim($(this).val()) != "ScopeOfOperations") {
                $('#divOthers').pageshow();
            }
            PopupReposition(); // NewMessageBox.js
        })
        .on('click', '#btnOkFilter', function () {
            AddFilter();
        })
        .on('click', '#btnCancelFilter', function () {
            $('#popFilter').popup('close');
        })

        .on('keyup', '#txtOrgTelCountryCode', function () {
            OnKeySearch({
                elem: $(this),
                url: "CRMDashboard/GetCountryCode",
                before: function () {
                    $('#txtOrgTelCountryID').val('');
                    $('#imgOrgTelCountryCodeIndicator').removeAttr('class');
                },
                during: function () {
                    $('#imgOrgTelCountryCodeIndicator').removeAttr('class').addClass('img-loading');
                },
                after: function (data) {
                    var txtOrgTelCountryCode = $('#txtOrgTelCountryCode');
                    var lstOrgTelCountryCode = $('#lstOrgTelCountryCode');
                    lstOrgTelCountryCode.empty();

                    if (data.length > 0) {
                        //if(data.length > 1){
                        //    for (var i = 0; i < data.length; i++) {
                        //        lstOrgTelCountryCode.append($('<option>')
                        //                    .attr('value', data[i].Name + ' ' + (data[i].CallingCode == undefined ? "" : '- ' + data[i].CallingCode))
                        //                    .attr('keyval', data[i].ID)
                        //                ).on('input', function () {
                        //                    console.log('input')
                        //                })
                        //    }
                        //}
                        //else {

                        //}
                        $('#txtOrgTelCountryID').val(data[0].ID);
                        if (data[0].CallingCode != undefined) {
                            txtOrgTelCountryCode.val(data[0].Code + ' - ' + data[0].CallingCode);
                            $('#imgOrgTelCountryCodeIndicator').removeAttr('class').addClass('img-check');
                        }
                        else {
                            txtOrgTelCountryCode.val(data[0].Code);
                            $('#imgOrgTelCountryCodeIndicator').removeAttr('class').addClass('img-warning');
                        }
                    }
                    else {
                        $('#imgOrgTelCountryCodeIndicator').removeAttr('class').addClass('img-error');
                    }
                }
            })
        })
        .on('keyup', '#txtOrgTelAreaCode', function () {
            OnKeySearch({
                elem: $(this),
                url: "Browser/LibraryAreaCodes",
                before: function () {
                    $('#txtOrgTelAreaCodeID').val('');
                    $('#imgOrgAreaCodeIndicator').removeAttr('class');
                },
                during: function () {
                    $('#imgOrgAreaCodeIndicator').removeAttr('class').addClass('img-loading');
                },
                after: function (data) {
                    var txtOrgTelAreaCode = $('#txtOrgTelAreaCode');
                    if (data.length > 0) {
                        $('#txtOrgTelAreaCodeID').val(data[0].ID);
                        if (data[0].CallingCode != undefined) {
                            txtOrgTelAreaCode.val(data[0].Area + ' - ' + data[0].CallingCode);
                            $('#imgOrgAreaCodeIndicator').removeAttr('class').addClass('img-check');
                        }
                        else {
                            txtOrgTelAreaCode.val(data[0].Area);
                            $('#imgOrgAreaCodeIndicator').removeAttr('class').addClass('img-warning');
                        }
                    }
                    else {
                        $('#imgOrgAreaCodeIndicator').removeAttr('class').addClass('img-error');
                    }
                }
            })
        })

        .on('click', '#btnRevCreditBegAsOf', function () {
            SingleCalendar({
                Click: function (e) {
                    $('#btnRevCreditBegAsOf').text(e);
                }
            })
            $('#tabRevenue').opentabmenu();
        })

        .on('click', '#btnRevThirdPartyBegBalanceAsOf', function () {
            SingleCalendar({
                Click: function (e) {
                    $('#btnRevThirdPartyBegBalanceAsOf').text(e);
                }
            })
            $('#tabRevenue').opentabmenu();
        })

        .on('click', '#btnAddElementary', function () {
            academicLvl = 'elem';
            isNewEducation = true;
            $('#txtAcademicLevel').text('Elementary');
            ClearEducation()
        })
        .on('click', '#btnAddHs', function () {
            academicLvl = 'hs';
            isNewEducation = true;
            $('#txtAcademicLevel').text('High School');
            ClearEducation()
        })
        .on('click', '#btnAddCollege', function () {
            academicLvl = 'college';
            isNewEducation = true;
            $('#txtAcademicLevel').text('College');
            ClearEducation()
        })
        .on('click', '#btnOkEduHi', function () {
            var name = $.trim($('#txtEdeHiSchoolName').val()),
                started = $.trim($('#txtEdeHiYearStarted').val()),
                graduated = $.trim($('#txtEdeHiYearGraduated').val());

            if (academicLvl == 'elem') {
                if (isNewEducation == true) {
                    ctrElem += 1;
                    arrElementary.push({
                        No: ctrElem,
                        Name: name,
                        Started: started,
                        Graduated: graduated,
                        Level: academicLvl,
                        State: 'add'
                    })
                } else {
                    arrElementary[ctrIndexEducation].Name = name;
                    arrElementary[ctrIndexEducation].Started = started;
                    arrElementary[ctrIndexEducation].Graduated = graduated;
                }
            } else if (academicLvl == 'hs') {
                if (isNewEducation == true) {
                    ctrHs += 1;
                    arrHighSchool.push({
                        No: ctrHs,
                        Name: name,
                        Started: started,
                        Graduated: graduated,
                        Level: academicLvl,
                        State: 'add'
                    })
                } else {
                    arrHighSchool[ctrIndexEducation].Name = name;
                    arrHighSchool[ctrIndexEducation].Started = started;
                    arrHighSchool[ctrIndexEducation].Graduated = graduated;
                }
            } else if (academicLvl == 'college') {
                if (isNewEducation == true) {
                    ctrCollege += 1;
                    arrCollege.push({
                        No: ctrCollege,
                        Name: name,
                        Started: started,
                        Graduated: graduated,
                        Level: academicLvl,
                        State: 'add'
                    })
                } else {
                    arrHighSchool[ctrIndexEducation].Name = name;
                    arrHighSchool[ctrIndexEducation].Started = started;
                    arrHighSchool[ctrIndexEducation].Graduated = graduated;
                }
            }

            DisplayEducation()
        })

        .on('click', '.btnPreview', function () {
            isViewReadOnlyProfile = false;
            $($(this).attr('menu-button')).click();
            $('#prevOrgProfile').hide();
        })
        .on('click', '#btnPreviewClose', function () {
            $('#prevOrgProfile').hide();
        })

        .on('click', '#btnAddNewCustomer', function () {
            $('#panAddCustomerLibrary').popup('open');
            $('#txtCustomerSearch').val('').focus();
            $('#CustomerlstSearchFound').empty();
            $('#divCustomerNoResultsFound, #btnAddToCustomerLibrary').hide();
            PopupReposition();
        })
        .on('click', '#btnSearchCustomerLibrary', function () {
            CustomerSearch();
        })
        .on('keyup', '#txtCustomerSearch', function (e) {
            if (e.which == 13) {
                CustomerSearch();
            }
        })
        .on('click', '#btnAddToCustomerLibrary', function () {
            PopupButtonList('', 'Select Options',
                [
                    {
                        Text: 'Add Organization',
                        Click: function () {
                            ClearOrgProspectVariablesFields();
                            if (location.pathname.toLowerCase() == '/erp') {
                                $('#divSystemSettings').find('#pageActions, #pageProfile, #pageProfileOrg').pageshow();
                            } else {
                                $('#pageActions, #pageProfile, #pageProfileOrg').pageshow();
                            }

                            if ($('#tabPersonRevenue').length > 0) {
                                $('#tabPersonRevenue').appendTo($('#divCompanyProfile'));
                                $('#tabPersonRevenue').attr('id', 'tabRevenue');
                            } else {
                                $('#tabRevenue').appendTo($('#divCompanyProfile'));
                            }
                            $('#divTabCompany > ul > li:first').click();
                            NewCustomerOptions('Organization Details');
                        }
                    },
                    {
                        Text: 'Add Person',
                        Click: function () {
                            ClearOrgProspectVariablesFields();
                            if (location.pathname.toLowerCase() == '/erp') {
                                $('#divSystemSettings').find('#pageActions, #pageProfile, #pageProfilePeople').pageshow();
                            } else {
                                $('#pageActions, #pageProfile, #pageProfilePeople').pageshow();
                            }

                            if ($('#tabRevenue').length > 0) {
                                $('#tabRevenue').appendTo($('#pageProfilePeople'));
                                $('#tabRevenue').attr('id', 'tabPersonRevenue');
                            } else {
                                $('#tabPersonRevenue').appendTo($('#pageProfilePeople'));
                            }
                            $('#divTabPerson > ul > li:first').click();
                            NewCustomerOptions('Person Details');
                        }
                    }
                ]
            )
        })
        .on('click', '#btnOptionsNewCustomer', function () {
            PopupButtonList('', 'Select Options',
                [
                    {
                        Text: 'Upload Customers Using Excel Template',
                        Click: function () {
                            $('#CustomerobjFile').trigger('click');
                        }
                    },
                    {
                        Text: 'Download Customers Excel Template',
                        Click: function () {
                            window.location = GetPathFormat("/CRMDashboard/DownloadContactTemplate");
                        }
                    },
                    {
                        Text: 'Import from CRM',
                        Click: function () {
                            NewMessageBox('Importing data from CRM. Please wait ...', 'Message');

                            AjaxSend({
                                url: '/OrganizationShort/ImportDataFromCrm',
                                success: function (e) {
                                    if (e.Results != undefined) {
                                        NewMessageBox(e.Results, 'Message');
                                        ShowProspectList();
                                    } else {
                                        NewMessageBox(e.Error, 'Error');
                                    }
                                }
                            })
                        }
                    },
                    {
                        Text: 'Deactivate all customer records',
                        Click: function () {
                            ActionForCustomerRecords(true);
                        }
                    },
                    {
                        Text: 'Reactivate all customer records',
                        Click: function () {
                            ActionForCustomerRecords(false);
                        }
                    },
                    {
                        Text: 'Merge duplicate customers...',
                        Click: function () {
                            PromptUserAccess('Merge Duplicate Customers', {
                                OnClickOKSuccess: function () {
                                    require(['promise!/Scripts/sources/ERP/OrganizationMerger/controller.js'],
                                    function () {
                                        new ERP.OrganizationMerger.controller().Show();
                                    });
                                }
                            });
                        }
                    }
                ]
            )
        })
        .on('click', '#btnRevSalesRepresentative', function () {
            BrowsePopup('LibraryPeople', 'Search Sales Representative',
                [
                    {
                        DisplayText: [{ Column: 'PeopleName' }],
                        Click: function (e) {
                            RevSalesRepID = e.ID;
                            $('#btnRevSalesRepresentative').text(e.PeopleName);
                        }
                    }
                ])

            $('#tabRevenue').opentabmenu();
        })
        .on('click', '#btnRevPriceLevel', function () {
            BrowsePopup('LibraryPIMPriceLevel', 'Search Price Level',
                [
                    {
                        DisplayText: [{ Column: 'PriceLevel' }],
                        Validation: function (e) {
                            var isExist = false;

                            for (var i = 0; i < arrRevPriceLevels.length; i++) {
                                if (arrRevPriceLevels[i].ID == e.ID) {
                                    isExist = true;
                                }
                            }

                            if (isExist) {
                                WebToast('Selected Price Level already exist', 'top');
                                return false;
                            }

                            return true;
                        },
                        Click: function (e) {
                            e.State = 'add';
                            arrRevPriceLevels.push(e);
                            DisplayRevPriceLevels();
                            //$('#btnRevPriceLevel').text(e.PriceLevel);
                        }
                    }
                ])
        })
        .on('click', '#btnRevCreditTerms', function () {
            BrowsePopup('LibraryPaymentTerms', 'Search Payment Terms',
                [
                    {
                        DisplayText: [{ Column: 'Name' }],
                        Click: function (e) {
                            RevPaymentTerms = e.ID;
                            $('#btnRevCreditTerms').text(e.Name);
                        }
                    }
                ])
            $('#tabRevenue').opentabmenu();
        })

        .on('keyup', '#txtSearchNewCustomer', function (e) {
            if (e.keyCode == 13) {
                NewCustomer.Search($(this).val());
            }
        })
        .on('click', '#btnAddCompanyOutlet', function () {
            $('#popCompanyOutlet').show()//popup('open');      
            $('#txtCompanyOutlet').val('');
            $('#txtOutletHouseBuildingNo').val('');
            $('#txtOutletStreet').val('');
            $('#txtCompanyOutlet3rdPartyID').val('');
            $('#selOutletCity').val('');
            $('#selOutletCountry').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
            $('#lstOutletTelephone, #lstOutletMobile, #lstOutletEmail, #lstOutletDepartment').empty();
            $('input[name="radAutoSku"]').removeAttr('checked').checkboxradio('refresh');
            $('input[name="radAutoSku"][value="all"]').attr('checked', true).checkboxradio('refresh');
            $('#tbodyOutletSkus').empty();
            $('#divForSomeSkus').hide();
            $('#tabOutletInfo').opentabmenu();            
            CompanyOutletState = "";
            arrOutletTelInfoList = [];
            arrOutletMobileInfoList = [];
            arrOutletEmailInfoList = [];
            arrCompanyOutletDepartment = [];
        })
        .on('change', 'input[name="radAutoSku"]', function () {
            if ($(this).val() == 'all') {
                $('#divForSomeSkus').hide();
            } else {
                $('#divForSomeSkus').show();
            }
        })
        .on('click', '#btnAddOutletDepartment', function () {
            $('#popCompanyOutletDepartment').popup('open');
            $('#txtCompanyOutletDepartment').val('').focus();
            $('#txtCompanyOutletDepartment3rdPartyID').val('')
            CompanyOutletDepartmentState = "";
        })
        .on('click', '#btnOkCompanyOutlet', function () {
            var txtCompanyOutlet = $.trim($('#txtCompanyOutlet').val());
            var txtCompanyOutlet3rdPartyID = $.trim($('#txtCompanyOutlet3rdPartyID').val());
            var txtOutletHouseBuildingNo = $.trim($('#txtOutletHouseBuildingNo').val());
            var txtOutletStreet = $.trim($('#txtOutletStreet').val());
            var selOutletCity = $.trim($('#selOutletCity').val());
            var selOutletCountry = $.trim($('#selOutletCountry').val());

            if (txtCompanyOutlet == "") {
                WebToast('Please enter outlet name.', 'top');
                return;
            }

            if (CompanyOutletState == "") {
                var data = {
                    ID: NewID(),
                    Name: txtCompanyOutlet,
                    ThirdPartyID: txtCompanyOutlet3rdPartyID,
                    HouseNo: txtOutletHouseBuildingNo,
                    Street: txtOutletStreet,
                    City: selOutletCity,
                    Country: selOutletCountry,
                    State: 'add',
                    Telephone: arrOutletTelInfoList,
                    Mobile: arrOutletMobileInfoList,
                    Email: arrOutletEmailInfoList,
                    IsActive: true,
                    Department: arrCompanyOutletDepartment
                }
                arrCompanyOutlet.push(data);
            } else {
                arrCompanyOutlet[sessionStorage.OutletIndex].Name = txtCompanyOutlet;
                arrCompanyOutlet[sessionStorage.OutletIndex].ThirdPartyID = txtCompanyOutlet3rdPartyID;
                arrCompanyOutlet[sessionStorage.OutletIndex].HouseNo = txtOutletHouseBuildingNo;
                arrCompanyOutlet[sessionStorage.OutletIndex].Street = txtOutletStreet;
                arrCompanyOutlet[sessionStorage.OutletIndex].City = selOutletCity;
                arrCompanyOutlet[sessionStorage.OutletIndex].Country = selOutletCountry;
                arrCompanyOutlet[sessionStorage.OutletIndex].Telephone = arrOutletTelInfoList;
                arrCompanyOutlet[sessionStorage.OutletIndex].Mobile = arrOutletMobileInfoList;
                arrCompanyOutlet[sessionStorage.OutletIndex].Email = arrOutletEmailInfoList;
                arrCompanyOutlet[sessionStorage.OutletIndex].Department = arrCompanyOutletDepartment;
            }

            DisplayCompanyOutlet();
            $('#popCompanyOutlet').hide();//popup('close');
        })
        .on('click', '#btnOkCompanyOutletDepartment', function () {
            var txtCompanyOutletDepartment = $.trim($('#txtCompanyOutletDepartment').val());
            var txtCompanyOutletDepartment3rdPartyID = $.trim($('#txtCompanyOutletDepartment3rdPartyID').val());

            if (txtCompanyOutletDepartment == "") {
                WebToast('Please enter department name.', 'top');
                return;
            }

            if (CompanyOutletDepartmentState == "") {
                var id = NewID();
                var data = {
                    Department: txtCompanyOutletDepartment,
                    ThirdPartyID: txtCompanyOutletDepartment3rdPartyID,
                    State: 'add',
                    IsActive: 1,
                    ID: id,
                    IsSourceDepartment: true
                }
                arrCompanyOutletDepartment.push(data)

                NewMessageBox('Would you like to add <b>' + txtCompanyOutletDepartment + '</b> to all of the other outlets belonging to ' + $('#txtProspectCompanyName').val() + '?', 'Message', [
                    {
                        Text: 'Yes', Click: function () {
                            if (arrCompanyOutlet.length > 0) {
                                for (var i = 0; i < arrCompanyOutlet.length; i++) {
                                    data = JSON.parse(JSON.stringify(data)); // create copy

                                    if (CInt2(sessionStorage.OutletIndex) == i) {
                                        continue;
                                    }

                                    var isExist = false;

                                    $.grep(arrCompanyOutlet[i].Department, function (e) {
                                        if (e.ID == id || e.RelatedFowstBinID == id) {
                                            isExist = true;
                                        }
                                    })

                                    if (!isExist) {
                                        data.RelatedFowstBinID = id;
                                        data.ID = NewID(); //create new ID
                                        data.IsSourceDepartment = false;
                                        arrCompanyOutlet[i].Department.push(data)
                                    }
                                }
                            }
                        }
                    }, { Text: 'No' }
                ])
            } else {
                arrCompanyOutletDepartment[sessionStorage.DepartmentIndex].Department = txtCompanyOutletDepartment;
                arrCompanyOutletDepartment[sessionStorage.DepartmentIndex].ThirdPartyID = txtCompanyOutletDepartment3rdPartyID;
            }

            DisplayCompanyOutletDepartment();
            $('#popCompanyOutletDepartment').popup('close');
        })
        .on('click', '#btnAddOutletTelephone', function () {
            ClearOutletTelInfo();
            $('#popOutletTelInfo').show();
        })
        .on('click', '#btnOutletTelAreaCode', function () {
            BrowsePopup('LibraryAreaCodes', 'Area Codes',
                [
                    {
                        DisplayText: [{ Column: 'Area' }, { Label: 'Code', Column: 'CallingCode' }],
                        Click: function (e) {
                            $('#txtOutletTelAreaCodeID').val(e.ID);
                            $('#txtOutletAreaCode').text(e.Area + ' - ' + e.CallingCode)
                        }
                    }
                ]
            )
        })
        .on('click', '#btnAddOutletTelephone', function () {
            ClearOutletTelInfo();
            $('#popOutletTelInfo').show();
        })
        .on('click', '#btnCancelOutletTelephone', function () {
            $('#popOutletTelInfo').hide();
            OutletTelState = '';
        })
        .on('click', '#btnOutletTelAreaCode', function () {
            BrowsePopup('LibraryAreaCodes', 'Area Codes',
                [
                    {
                        DisplayText: [{ Column: 'Area' }, { Label: 'Code', Column: 'CallingCode' }],
                        Click: function (e) {
                            $('#txtOutletTelAreaCodeID').val(e.ID);
                            $('#txtOutletAreaCode').text(e.Area + ' - ' + e.CallingCode)
                        }
                    }
                ]
            )
        })
        .on('keydown', '#txtOutletTelephoneNum', function (e) {
            if (!NumberDotOnly($(this), e)) {
                return;
            }
        })
        .on('click', '#btnSaveOutletTelephone', function () {
            if ($('#txtOutletAreaCode').text() == "Select") {
                $('#spanerrorOutletAreaCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            if ($('#txtOutletTelephoneNum').val() == "") {
                $('#spanerrorOutletTelephoneNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            //if ($('#txtOutletTelephoneNum').val().length != 7) {
            //    $('#spanerrorOutletTelephoneNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
            //    return false;
            //}

            if ($('#selOutletTelType').val() == "") {
                $('#spanerrorOutletTelType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }
            intOutletTelInfoCtr = intOutletTelInfoCtr + 1;

            if (OutletTelState == "") {
                arrOutletTelInfoList.push({
                    "TelRankNo": intOutletTelInfoCtr,
                    "CountryCodeID": $('#txtOutletTelCountryID').val(),
                    "CountryCode": $('#txtOutletCountryCode').text(),
                    "AreaCode": $('#txtOutletAreaCode').text(),
                    "AreaCodeID": $('#txtOutletTelAreaCodeID').val(),
                    "TelephoneNo": $('#txtOutletTelephoneNum').val(),
                    "ExtensionNo": $('#txtOutletTelephoneExt').val(),
                    "ContactType": $('#selOutletTelType').val(),
                    "TelState": "Add",
                    "IsNew": 1,
                    "TelID": $('#txtOutletTelID').val()
                })
            }
            else {
                arrOutletTelInfoList[sessionStorage.Telindex].TelRankNo = OutletTelRankNo;
                arrOutletTelInfoList[sessionStorage.Telindex].CountryCodeID = $('#txtOutletTelCountryID').val();
                arrOutletTelInfoList[sessionStorage.Telindex].CountryCode = $('#txtOutletCountryCode').text();
                arrOutletTelInfoList[sessionStorage.Telindex].AreaCodeID = $('#txtOutletTelAreaCodeID').val();
                arrOutletTelInfoList[sessionStorage.Telindex].AreaCode = $('#txtOutletAreaCode').text();
                arrOutletTelInfoList[sessionStorage.Telindex].TelephoneNo = $('#txtOutletTelephoneNum').val();
                arrOutletTelInfoList[sessionStorage.Telindex].ExtensionNo = $('#txtOutletTelephoneExt').val();
                arrOutletTelInfoList[sessionStorage.Telindex].ContactType = $('#selOutletTelType').val();
                arrOutletTelInfoList[sessionStorage.Telindex].TelState = sessionStorage.TelState;
                arrOutletTelInfoList[sessionStorage.Telindex].IsNew = sessionStorage.TelIsNew;
                arrOutletTelInfoList[sessionStorage.Telindex].TelID = $('#txtOutletTelID').val();
            }

            AddOutletTelInfo();
            ClearOutletTelInfo();
            OutletTelState = "";
        })
        .on('click', '#btnAddOutletMobile', function () {
            ClearOutletMobileInfo();
            $('#popOutletMobileInfo').show();
        })
        .on('click', '#btnCancelOutletMobile', function () {
            $('#popOutletMobileInfo').hide();
            OutletMobileState = '';
        })
        .on('click', '#btnOutletMobileTelCode', function () {
            BrowsePopup('LibraryTelcoPrefixes', 'Telco Codes',
                [
                    {
                        DisplayText: [{ Column: 'Prefix' }, { Column: 'TelCo' }],
                        Click: function (e) {
                            $('#txtOutletTelCoPrefixID').val(e.ID);
                            $('#txtOutletTelCode').text(e.TelCo + ' - ' + e.Prefix)
                        }
                    }
                ]
            )
        })
        .on('keydown', '#txtOutletMobileNo', function (e) {
            if (!NumberDotOnly($(this), e)) {
                return;
            }
        })
        .on('click', '#btnSaveOutletMobile', function () {
            if ($('#txtOutletTelCode').text() == "Select") {
                $('#spanerrorOutletMobileTelCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            if ($('#txtOutletMobileNo').val() == "") {
                $('#spanerrorOutletMobileNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            //if ($('#txtOutletMobileNo').val().length != 7) {
            //    $('#spanerrorOutletMobileNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
            //    return false;
            //}

            if ($('#selOutletMobileType').val() == "") {
                $('#spanerrorOutletMobileType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            intOutletMobileInfoCtr = intOutletMobileInfoCtr + 1;

            if (OutletMobileState == "") {
                arrOutletMobileInfoList.push({
                    "MobileRankNo": intOutletMobileInfoCtr,
                    "CountryCodeID": $('#txtOutletMobileCountryID').val(),
                    "CountryCode": $('#txtOutletMobileCountryCode').text(),
                    "TelCode": $('#txtOutletTelCode').text(),
                    "TelCodeID": $('#txtOutletTelCoPrefixID').val(),
                    "MobileNo": $('#txtOutletMobileNo').val(),
                    "ContactType": $('#selOutletMobileType').val(),
                    "MobileID": $('#txtOutletMobileID').val(),
                    "MobileState": "Add",
                    "IsNew": 1
                })
            }
            else {
                arrOutletMobileInfoList[sessionStorage.Mobileindex].MobileRankNo = OutletMobileRankNo;
                arrOutletMobileInfoList[sessionStorage.Mobileindex].CountryCodeID = $('#txtOutletMobileCountryID').val();
                arrOutletMobileInfoList[sessionStorage.Mobileindex].CountryCode = $('#txtOutletMobileCountryCode').text();
                arrOutletMobileInfoList[sessionStorage.Mobileindex].TelCodeID = $('#txtOutletTelCoPrefixID').val();
                arrOutletMobileInfoList[sessionStorage.Mobileindex].TelCode = $('#txtOutletTelCode').text();
                arrOutletMobileInfoList[sessionStorage.Mobileindex].MobileNo = $('#txtOutletMobileNo').val();
                arrOutletMobileInfoList[sessionStorage.Mobileindex].ContactType = $('#selOutletMobileType').val();
                arrOutletMobileInfoList[sessionStorage.Mobileindex].MobileState = sessionStorage.MobileState;
                arrOutletMobileInfoList[sessionStorage.Mobileindex].IsNew = sessionStorage.MobileIsNew;
            }

            AddOutletMobileInfo();
            OutletMobileState = "";
            ClearOutletMobileInfo();
        })
        .on('click', '#btnAddOutletEmail', function () {
            ClearOutletEmailInfoFields();
            $('#popOutletEmailInfo').show();
        })
        .on('click', '#btnCancelOutletEmailAddress', function () {
            $('#popOutletEmailInfo').hide();
            OutletEmailState = '';
        })
        .on('click', '#btnSaveOutletEmailAddress', function () {
            if (IsEmail($.trim($("#txtOutletEmailAdd").val())) == false) {
                $('#spanerrorOutletEmailAddressValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            if ($.trim($("#txtOutletEmailAdd").val()) == "") {
                $('#spanerrorOutletEmailAddress').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            if ($('#selOutletEmailType').val() == "") {
                $('#spanerrorOutletEmailType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                return false;
            }

            intOutletEmailInfoCtr = intOutletEmailInfoCtr + 1;

            if (OutletEmailState == "") {
                arrOutletEmailInfoList.push({
                    "EmailRankNo": intOutletEmailInfoCtr,
                    "EmailAddID": $('#txtOutletEmailID').val(),
                    "EmailAdd": $('#txtOutletEmailAdd').val(),
                    "EmailType": $('#selOutletEmailType').val(),
                    "EmailState": "Add",
                    "IsNew": 1,
                })
            }
            else {
                arrOutletEmailInfoList[sessionStorage.Emailindex].EmailRankNo = OutletEmailRankNo;
                arrOutletEmailInfoList[sessionStorage.Emailindex].EmailAddID = $('#txtOutletEmailID').val();
                arrOutletEmailInfoList[sessionStorage.Emailindex].EmailAdd = $('#txtOutletEmailAdd').val();
                arrOutletEmailInfoList[sessionStorage.Emailindex].EmailType = $('#selOutletEmailType').val();
                arrOutletEmailInfoList[sessionStorage.Emailindex].EmailState = sessionStorage.EmailState;
                arrOutletEmailInfoList[sessionStorage.Emailindex].IsNew = sessionStorage.EmailIsNew;
            }

            AddOutletEmailInfo();
            OutletEmailState = "";
        })
        .on('click', '#btnOutletCopyExSku', function () {
            $('#popCompanyOutletExSku').popup('open');
        })
        .on('click', '#btnOutletAddSku', function () {
            BrowsePopup('LibraryPIMProduct', 'Select SKU', [
                {
                    DisplayText: [{ Label: 'Product No.', Column: 'ProductNo' }, { Column: 'ProductName' }],
                    SelectMultiple: true,
                    Click: function (e) {
                        DisplayOutletSkus(e);
                    }
                }
            ])
        })
        .on('click', '#btnSaveNewOutlet', function () {
            var listNewOutlet = $('#listNewOutlet');
            var arrCust = [];

            listNewOutlet.find(' > li').each(function () {
                var disCus = $(this);
                var dataCus = JSON.parse(JSON.stringify(disCus.data('data')));
                var arrO = [];

                disCus.find('#' + dataCus.DbID).find(' > li').each(function () {
                    var disOutlet = $(this);
                    var dataOutlet = JSON.parse(JSON.stringify(disOutlet.data('data')));
                    var arrD = [];

                    disOutlet.find('#' + dataOutlet.ID).find(' > li').each(function () {
                        var disDept = $(this);
                        var dataDept = JSON.parse(JSON.stringify(disDept.data('data')));

                        if (disDept.find('[depid="' + dataDept.ID + '"]').is(':checked')) {
                            arrD.push(dataDept)
                        }
                    })

                    if (isOutlet) {
                        if (disOutlet.find('[outletid="' + dataOutlet.ID + '"]').is(':checked')) {
                            dataOutlet.SelectedDept = arrD;
                            arrO.push(dataOutlet)
                        }
                    } else {
                        if (arrD.length > 0) {
                            dataOutlet.SelectedDept = arrD;
                            arrO.push(dataOutlet)
                        }
                    }
                })

                if (arrO.length > 0) {
                    dataCus.SelectedOutlet = arrO;
                    arrCust.push(dataCus);
                }
            })
            console.log(arrCust)
            //return;
            AjaxSend({
                url: 'OrganizationShort/ApplyOutletsToSelectedCustomers',
                data: { data: arrCust },
                success: function (e) {
                    if (e.Results == 'saved') {
                        NewMessageBox('Successfully saved', 'Message');
                        $('#popNewOutlet').hide();

                        ApplyNewDepartmentToOutlets();
                    } else {
                        NewMessageBox(e.Results, 'Message');
                    }
                }
            })
        })
        .on('change', '#selCompanyOutletStatus', function () {
            DisplayCompanyOutlet();
        })
        .on('change', '#selCompanyOutletDeptStatus', function () {
            DisplayCompanyOutletDepartment();
        })

        .on('click', '#btnSupplierGenerateUrl', function () {
            AjaxSend({
                url: 'OrganizationShort/GeneratePublicUrl',
                data: {
                    SupplierID: sessionStorage.ViewID
                }, success: function (e) {
                    if (e.Results == 'generated') {
                        GetGeneratedPublicUrl();
                    }
                }
            })
        })
        .on('click', '#btnCopyGenUrl', function () {
            CopyToClipboard('#txtPublicUrl');
        })

        .on('change', '#selSecQuestion1', function () {
            var val = $.trim($(this).val());
            $('#selSecQuestion2').find('option').removeAttr('disabled');

            if (val != '') {
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
        .on('change', '#selPersonSecQuestion1', function () {
            var val = $.trim($(this).val());
            $('#selPersonSecQuestion2').find('option').removeAttr('disabled');

            if (val != '') {
                $('#selPersonSecQuestion2 option[value="' + $(this).val() + '"]').attr('disabled', true);
            }
        })
        .on('change', '#selPersonSecQuestion2', function () {
            var val = $.trim($(this).val());
            $('#selPersonSecQuestion1').find('option').removeAttr('disabled');

            if (val != '') {
                $('#selPersonSecQuestion1 option[value="' + $(this).val() + '"]').attr('disabled', true);
            }
        })

        function GetGeneratedPublicUrl() {
            AjaxSend({
                url: 'OrganizationShort/GetGeneratedPublicUrl',
                data: {
                    SupplierID: sessionStorage.ViewID
                }, success: function (e) {
                    var vRows = e.Results.Rows;

                    if (vRows.length > 0) {
                        vRows = vRows[0];

                        $('#txtPublicUrl').val(location.origin + '/Ecommerce/PreSupplier?id=' + vRows.Link);
                        $('#lblPublicUrlDateGenerated').text('Date Generated: ' + $.format.date(new Date(vRows.DateCreated), 'MMMM dd, yyyy hh:mm a'));

                        if (vRows.DateEdited != undefined) {
                            $('#lblPublicUrlDateEdited').text('Date Edited: ' + $.format.date(new Date(vRows.DateEdited), 'MMMM dd, yyyy hh:mm a'));
                        }

                        $('#divGeneratePublicUrl').hide();
                        $('#divGeneratedPublicUrl').show();
                    } else {
                        $('#divGeneratePublicUrl').show();
                        $('#divGeneratedPublicUrl').hide();
                    }
                }
            })
        }

        function ActionForCustomerRecords(IsDeactivate) {
            NewMessageBox('Are you sure you want to ' + (IsDeactivate ? 'deactivate' : 'reactivate') + ' all customer records?', 'Message', [
                {
                    Text: 'Yes',
                    Click: function () {
                        $('#popConfirmCustomerDeactivation').popup('open');
                        $('#txtCustomerDeactivatePassword').val('').focus()

                        $('#btnDeactivateCustomerNow').unbind('click').click(function () {
                            VerifyPassword()
                        })

                        $('#txtCustomerDeactivatePassword').unbind('keydown').keydown(function (e) {
                            if (e.which == 13) {
                                VerifyPassword()
                            }
                        })

                        function VerifyPassword() {
                            AjaxSend({
                                url: '/Login3/VerifyPassword',
                                data: {
                                    Password: $.trim($('#txtCustomerDeactivatePassword').val())
                                },
                                success: function (vData) {
                                    if (vData.Error) {
                                        WebToast(vData.Error, 'top');
                                        $('#txtCustomerDeactivatePassword').val('').focus()
                                    } else if (vData.Result == 'ok') {
                                        NewMessageBox('Please wait while ' + (IsDeactivate ? 'deactivating' : 'reactivating') + ' all customer records ...', 'Message');

                                        AjaxSend({
                                            url: 'OrganizationShort/DeactivateAllCustomerRecords',
                                            data: { Status: CInt2(IsDeactivate) },
                                            success: function (e) {
                                                NewMessageBoxClose()

                                                if (e.Results == 'saved') {
                                                    WebToast('All customer records has been successfully ' + (IsDeactivate ? 'deactivated' : 'reactivated'), 'top');
                                                    ShowProspectList();
                                                } else {
                                                    NewMessageBox(e.Results, 'Message');
                                                }
                                            }
                                        })
                                    } else
                                        NewMessageBox('Unknown error');
                                }
                            });
                        }
                    }
                }, { Text: 'No' }
            ]);
        }

        function DisplayOutletSkus(e) {
            var tbodyOutletSkus = $('#tbodyOutletSkus');

            for (var i = 0; i < e.length; i++) {
                tbodyOutletSkus.append($('<tr>')
                        .data('data', e[i])
                        .append($('<td>')
                            .text(e[i].ProductNo)
                        )
                        .append($('<td>')
                            .text(e[i].ProductName)
                        )
                        .append($('<td>')
                            .append($('<input data-role="none" type="text" class="form-control">'))
                        )
                        .append($('<td>')
                            .text('')
                        )
                        .append($('<td>')
                            .text('')
                        )
                        .append($('<td>')
                            .append($('<button data-role="none" class="btn custom-btn-theme-d btnCircle-xs">')
                                .append($('<span class="glyphicon glyphicon-remove">'))
                            )
                            .click(function () {

                            })
                        )
                    )
            }
        }

        function AddOutletEmailInfo() {

            var pLV = $('#lstOutletEmail');
            var pItem;

            pLV.html('');
            for (var i = 0; i < arrOutletEmailInfoList.length; i++) {
                if (arrOutletEmailInfoList[i].EmailState != "Remove") {
                    arrOutletEmailInfoList[i].arrindex = i;
                    pItem = $('<li data-icon="delete">')
                            .data(arrOutletEmailInfoList[i])
                            .append($('<a>')
                                    .data(arrOutletEmailInfoList[i])
                                    .html($.trim(arrOutletEmailInfoList[i].EmailAdd) +
                                        "<br>" + $.trim(arrOutletEmailInfoList[i].EmailType)
                                    )
                                    .click(function (e) {
                                        EditOutletEmailInfo($(this).data().arrindex);
                                    })
                            )
                            .append($('<a>')
                                    .data('data', arrOutletEmailInfoList[i])
                                    .click(function (e) {
                                        var EmailRankNo = $(this).data('data').EmailRankNo;

                                        if ($(this).data('data').IsNew == 1) {
                                            //remove selected email
                                            arrOutletEmailInfoList = $.grep(arrOutletEmailInfoList, function (val) {
                                                return val.EmailRankNo != EmailRankNo;
                                            })
                                        }
                                        else {
                                            arrOutletEmailInfoList[$(this).data('data').arrindex].EmailState = "Remove";
                                        }

                                        AddOutletEmailInfo();
                                    })
                            )

                    pLV.append(pItem);
                }
            }

            pLV.listview('refresh');
            $('#popOutletEmailInfo').hide();
        }

        function EditOutletEmailInfo(arrindex) {
            $('#popOutletEmailInfo').show();
            OutletEmailRankNo = arrOutletEmailInfoList[arrindex].EmailRankNo;
            OutletEmailState = "Edit";
            sessionStorage["Emailindex"] = arrindex;
            sessionStorage["EmailIsNew"] = arrOutletEmailInfoList[arrindex].IsNew;
            sessionStorage["EmailState"] = arrOutletEmailInfoList[arrindex].EmailState;
            $('#txContacttPersonEmailID').val(arrOutletEmailInfoList[arrindex].EmailAddID == undefined ? "" : arrOutletEmailInfoList[arrindex].EmailAddID);
            $('#txtOutletEmailAdd').val(arrOutletEmailInfoList[arrindex].EmailAdd);
            $('#selOutletEmailType').val(arrOutletEmailInfoList[arrindex].EmailType);
        }

        function ClearOutletEmailInfoFields() {
            $('#txtOutletEmailID').val('');
            $('#txtOutletEmailAdd').val('');
            $('#selOutletEmailType').val('');
            $('#divOutletEmailHome').pageshow();
        }

        function AddOutletMobileInfo() {

            var pLV = $('#lstOutletMobile');
            var pItem;

            pLV.html('');
            for (var i = 0; i < arrOutletMobileInfoList.length; i++) {
                if (arrOutletMobileInfoList[i].MobileState != "Remove") {
                    arrOutletMobileInfoList[i].arrindex = i;
                    pItem = $('<li data-icon="delete">')
                                .data(arrOutletMobileInfoList[i])
                                .append($('<a>')
                                        .attr('ID', arrOutletMobileInfoList[i].id)
                                        .html("+" + $.trim(arrOutletMobileInfoList[i].CountryCode.split('-')[1]) + " (" + $.trim(arrOutletMobileInfoList[i].TelCode.split('-')[1]) + ") " + $.trim(arrOutletMobileInfoList[i].MobileNo) +
                                            "<br>" + $.trim(arrOutletMobileInfoList[i].TelCode.split('-')[0]) + " (" + $.trim(arrOutletMobileInfoList[i].CountryCode.split('-')[0]) + ") " +
                                            "<br>" + $.trim(arrOutletMobileInfoList[i].ContactType)
                                        )
                                        .data(arrOutletMobileInfoList[i])
                                        .click(function (e) {
                                            EditOutletMobileInfo($(this).data().arrindex);
                                        })
                                )
                                .append($('<a>')
                                        .data('data', arrOutletMobileInfoList[i])
                                        .click(function (e) {
                                            var MobileRankNo = $(this).data('data').MobileRankNo;

                                            if ($(this).data('data').IsNew == 1) {
                                                //remove selected mobile
                                                arrOutletMobileInfoList = $.grep(arrOutletMobileInfoList, function (val) {
                                                    return val.MobileRankNo != MobileRankNo;
                                                })
                                            }
                                            else {
                                                arrOutletMobileInfoList[$(this).data('data').arrindex].MobileState = "Remove";
                                            }

                                            AddOutletMobileInfo();
                                        })
                                )

                    pLV.append(pItem);
                }
            }

            pLV.listview('refresh');
            $('#popOutletMobileInfo').hide();
        }

        function EditOutletMobileInfo(arrindex) {
            $('#popOutletMobileInfo').show();
            OutletMobileRankNo = arrOutletMobileInfoList[arrindex].MobileRankNo;
            OutletMobileState = "Edit";
            sessionStorage["Mobileindex"] = arrindex;
            sessionStorage["MobileIsNew"] = arrOutletMobileInfoList[arrindex].IsNew;
            sessionStorage["MobileState"] = arrOutletMobileInfoList[arrindex].MobileState;
            $('#txtOutletMobileID').val(arrOutletMobileInfoList[arrindex].MobileID == undefined ? "" : arrOutletMobileInfoList[arrindex].MobileID);
            $('#txtOutletTelCoPrefixID').val(arrOutletMobileInfoList[arrindex].TelCodeID);
            $('#txtOutletMobileCountryID').val(arrOutletMobileInfoList[arrindex].CountryCodeID);
            $('#txtOutletMobileCountryCode').text(arrOutletMobileInfoList[arrindex].CountryCode);
            $('#txtOutletTelCode').text(arrOutletMobileInfoList[arrindex].TelCode);
            $('#txtOutletMobileNo').val(arrOutletMobileInfoList[arrindex].MobileNo);
            $('#selOutletMobileType').val(arrOutletMobileInfoList[arrindex].ContactType);
        }

        function ClearOutletMobileInfo() {
            $('#txtOutletMobileID').val('');
            $('#txtOutletTelCoPrefixID').val('');
            $('#txtOutletMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
            $('#txtOutletMobileCountryCode').text('PHL - 63');
            $('#txtOutletTelCode').text('Select');
            $('#txtOutletMobileNo').val('');
            $('#selOutletMobileType').val('');
            $('#divOutletMobileHome').pageshow();
        }

        function AddOutletTelInfo() {

            var pLV = $('#lstOutletTelephone');
            var pItem;

            pLV.html('');
            for (var i = 0; i < arrOutletTelInfoList.length; i++) {
                if (arrOutletTelInfoList[i].TelState != "Remove") {
                    arrOutletTelInfoList[i].arrindex = i;
                    pItem = $('<li>')
                            .data(arrOutletTelInfoList[i])
                            .attr('data-icon', 'delete')
                            .append($('<a>')
                                    .attr('ID', arrOutletTelInfoList[i].id)
                                    .html("+" + $.trim(arrOutletTelInfoList[i].CountryCode.split('-')[1]) + " (" + $.trim(arrOutletTelInfoList[i].AreaCode.split('-')[1]) + ") " + $.trim(arrOutletTelInfoList[i].TelephoneNo) + ($.trim(arrOutletTelInfoList[i].ExtensionNo) == "" ? "" : "." + $.trim(arrOutletTelInfoList[i].ExtensionNo)) +
                                        "<br>" + $.trim(arrOutletTelInfoList[i].AreaCode.split('-')[0]) + ", " + $.trim(arrOutletTelInfoList[i].CountryCode.split('-')[0]) +
                                        "<br>" + $.trim(arrOutletTelInfoList[i].ContactType)
                                    )
                                    .data('data', arrOutletTelInfoList[i])
                                    .click(function (e) {
                                        EditOutletTelInfo($(this).data('data').arrindex);
                                    })
                            )
                            .append($('<a>')
                                    .data('data', arrOutletTelInfoList[i])
                                    .click(function (e) {
                                        var TelRankNo = $(this).data('data').TelRankNo;

                                        if ($(this).data('data').IsNew == 1) {
                                            //remove selected telephone
                                            arrOutletTelInfoList = $.grep(arrOutletTelInfoList, function (val) {
                                                return val.TelRankNo != TelRankNo;
                                            })
                                        }
                                        else {
                                            arrOutletTelInfoList[$(this).data('data').arrindex].TelState = "Remove";
                                        }

                                        AddOutletTelInfo();
                                    })
                            )

                    pLV.append(pItem);
                }

            }

            pLV.listview('refresh');
            $('#popOutletTelInfo').hide();
        }

        function EditOutletTelInfo(arrindex) {
            $('#popOutletTelInfo').show();

            OutletTelRankNo = arrOutletTelInfoList[arrindex].TelRankNo;
            OutletTelState = "Edit";
            sessionStorage["Telindex"] = arrindex;
            sessionStorage["TelIsNew"] = arrOutletTelInfoList[arrindex].IsNew;
            sessionStorage["TelState"] = arrOutletTelInfoList[arrindex].TelState;
            $('#txtOutletTelID').val(arrOutletTelInfoList[arrindex].TelID == undefined ? "" : arrOutletTelInfoList[arrindex].TelID);
            $('#txtOutletTelAreaCodeID').val(arrOutletTelInfoList[arrindex].AreaCodeID);
            $('#txtOutletTelCountryID').val(arrOutletTelInfoList[arrindex].CountryCodeID);
            $('#txtOutletCountryCode').text(arrOutletTelInfoList[arrindex].CountryCode);
            $('#txtOutletAreaCode').text(arrOutletTelInfoList[arrindex].AreaCode);
            $('#txtOutletTelephoneNum').val(arrOutletTelInfoList[arrindex].TelephoneNo);
            $('#txtOutletTelephoneExt').val(arrOutletTelInfoList[arrindex].ExtensionNo);
            $('#selOutletTelType').val(arrOutletTelInfoList[arrindex].ContactType);
        }

        function ClearOutletTelInfo() {
            $('#txtOutletTelID').val('');
            $('#txtOutletTelAreaCodeID').val('');
            $('#txtOutletTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
            $('#txtOutletCountryCode').text('PHL - 63');
            $('#txtOutletAreaCode').text('Select');
            $('#txtOutletTelephoneNum').val('');
            $('#txtOutletTelephoneExt').val('');
            $('#selOutletTelType').val('');
            $('#divOutletTel').pageshow();
        }

        function DisplayCompanyOutletDepartment() {
            var lstOutletDepartment = $('#lstOutletDepartment');
            lstOutletDepartment.empty();

            for (var i = 0; i < arrCompanyOutletDepartment.length; i++) {
                arrCompanyOutletDepartment[i].Index = i;
                if ($('#selCompanyOutletDeptStatus').val() != '') {
                    if (CBool($('#selCompanyOutletDeptStatus').val()) != arrCompanyOutletDepartment[i].IsActive) {
                        continue;
                    }
                }

                if (arrCompanyOutletDepartment[i].State != "remove") {
                    lstOutletDepartment.append($('<li data-icon="gear">')
                        .data('data', arrCompanyOutletDepartment[i])
                        .append($('<a>')
                            .html(arrCompanyOutletDepartment[i].Department + ' ')
                            .append($('<span style="color:#949494;font-weight:normal;">')
                                .text((arrCompanyOutletDepartment[i].State == "add" ? '(New)' :
                                        (arrCompanyOutletDepartment[i].State == 'edit' ? (CBool(arrCompanyOutletDepartment[i].IsActive) ? '(Active)' : '(Inactive)') : '(' + arrCompanyOutletDepartment[i].State + ')')
                                    )
                                )
                            )
                        )
                        .append($('<a>')
                            .data('data', arrCompanyOutletDepartment[i])
                            .data('dataDepartment', arrCompanyOutletDepartment)
                            .text(arrCompanyOutletDepartment[i].Department)
                            .click(function (e) {
                                e.stopPropagation();
                                e.preventDefault();
                                var data = $(this).data('data');
                                var dataDepartment = $(this).data('dataDepartment');
                                var action;

                                if (data.State == "add") {
                                    action = "delete";
                                } else {
                                    if (CBool(data.IsActive)) {
                                        action = (data.State == "Deactivate" ? "activate" : "deactivate");
                                    } else {
                                        action = "activate";
                                    }
                                }

                                NewMessageBox('Are you sure you want to <b>' + action + '</b> this record <b>"' + data.Department + '"</b>?', 'Message', [
                                    {
                                        Text: 'Yes',
                                        Click: function (e) {
                                            var copydata = JSON.parse(JSON.stringify(data));

                                            NewMessageBox('Would you like to ' + action + ' <b>' + data.Department + '</b> from all of the other outlets belonging to ' + JSON.parse(sessionStorage.ClientInfo).ClientName + '?', 'Message', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        for (var i = 0; i < arrCompanyOutlet.length; i++) {
                                                            if (CInt2(sessionStorage.OutletIndex) == i) {
                                                                continue;
                                                            }

                                                            for (var d = 0; d < arrCompanyOutlet[i].Department.length; d++) {
                                                                if (arrCompanyOutlet[i].Department[d].RelatedFowstBinID == (copydata.RelatedFowstBinID || copydata.ID)
                                                                    || arrCompanyOutlet[i].Department[d].ID == (copydata.RelatedFowstBinID || copydata.ID)) {
                                                                    if (copydata.State == 'add') {
                                                                        arrCompanyOutlet[i].Department.splice(d, 1);
                                                                    } else {
                                                                        if (CBool(copydata.IsActive)) {
                                                                            arrCompanyOutlet[i].Department[d].IsActive = (copydata.State == "Deactivate" ? 1 : 0);
                                                                            arrCompanyOutlet[i].Department[d].State = (copydata.State == "Deactivate" ? "Activate" : "Deactivate");
                                                                        } else {
                                                                            arrCompanyOutlet[i].Department[d].IsActive = 1;
                                                                            arrCompanyOutlet[i].Department[d].State = 'Activate';
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }, { Text: 'No' }
                                            ])

                                            if (data.State == 'add') {
                                                dataDepartment.splice(data.Index, 1);
                                            } else {
                                                if (CBool(data.IsActive)) {
                                                    dataDepartment[data.Index].IsActive = (data.State == "Deactivate" ? 1 : 0);
                                                    dataDepartment[data.Index].State = (data.State == "Deactivate" ? "Activate" : "Deactivate");
                                                } else {
                                                    dataDepartment[data.Index].IsActive = 1;
                                                    dataDepartment[data.Index].State = 'Activate';
                                                }
                                            }
                                            DisplayCompanyOutletDepartment();
                                        }
                                    }, { Text: 'No' }
                                ])
                            })
                        )
                        .click(function () {
                            var data = $(this).data('data');
                            CompanyOutletDepartmentState = "edit";
                            sessionStorage.DepartmentIndex = data.Index;
                            $('#popCompanyOutletDepartment').popup('open');
                            $('#txtCompanyOutletDepartment').val(data.Department).focus();
                            $('#txtCompanyOutletDepartment3rdPartyID').val(CStr2(data.ThirdPartyID));
                        })
                    )
                }
            }

            lstOutletDepartment.listview('refresh');
        }

        function DisplayCompanyOutlet() {
            var lstCompanyOutlet = $('#lstCompanyOutlet');

            lstCompanyOutlet.empty();

            for (var i = 0; i < arrCompanyOutlet.length; i++) {
                arrCompanyOutlet[i].Index = i;
                if ($('#selCompanyOutletStatus').val() != '') {
                    if (CBool($('#selCompanyOutletStatus').val()) != arrCompanyOutlet[i].IsActive) {
                        continue;
                    }
                }

                if (arrCompanyOutlet[i].State != 'remove') {
                    lstCompanyOutlet.append($('<li data-icon="gear">')
                        .append($('<a>')
                            .html(arrCompanyOutlet[i].Name + ' ')
                            .append($('<span style="color:#949494;font-weight:normal;">')
                                .text((arrCompanyOutlet[i].State == "add" ? '(New)' :
                                        (arrCompanyOutlet[i].State == 'edit' ? (CBool(arrCompanyOutlet[i].IsActive) ? '(Active)' : '(Inactive)') : '(' + arrCompanyOutlet[i].State + ')')
                                    )
                                )
                            )
                        )
                        .append($('<a>')
                            .data('data', arrCompanyOutlet[i])
                            .click(function (e) {
                                e.stopPropagation();
                                e.preventDefault();

                                var data = $(this).data('data');
                                var action;

                                if (data.State == "add") {
                                    action = "delete";
                                } else {
                                    if (CBool(data.IsActive)) {
                                        action = (data.State == "Deactivate" ? "activate" : "deactivate");
                                    } else {
                                        action = "activate";
                                    }
                                }

                                NewMessageBox('Are you sure you want to <b>' + action + '</b> this record <b>"' + data.Name + '"</b>?', 'Message', [
                                    {
                                        Text: 'Yes',
                                        Click: function (e) {
                                            if (data.State == 'add') {
                                                arrCompanyOutlet.splice(data.Index, 1)
                                            } else {
                                                if (CBool(data.IsActive)) {
                                                    arrCompanyOutlet[data.Index].IsActive = (data.State == "Deactivate" ? 1 : 0);
                                                    arrCompanyOutlet[data.Index].State = (data.State == "Deactivate" ? "Activate" : "Deactivate");
                                                } else {
                                                    arrCompanyOutlet[data.Index].IsActive = 1;
                                                    arrCompanyOutlet[data.Index].State = 'Activate';
                                                }
                                            }
                                            DisplayCompanyOutlet();
                                        }
                                    }, { Text: 'No' }
                                ])
                            })
                        )
                        .data('data', arrCompanyOutlet[i])
                        .click(function (e) {
                            e.stopPropagation();
                            e.preventDefault();
                            var data = $(this).data('data');
                            CompanyOutletState = "edit";
                            sessionStorage.OutletIndex = data.Index;
                            arrOutletTelInfoList = data.Telephone;
                            arrOutletMobileInfoList = data.Mobile;
                            arrOutletEmailInfoList = data.Email;
                            arrCompanyOutletDepartment = data.Department;
                            $('#popCompanyOutlet').show();
                            $('#tabOutletInfo').opentabmenu();
                            $('#txtCompanyOutlet').val(data.Name).focus();
                            $('#txtOutletAddressID').val(data.AddressID);
                            $('#txtCompanyOutlet3rdPartyID').val(CStr2(data.ThirdPartyID));
                            $('#txtOutletHouseBuildingNo').val(data.HouseNo);
                            $('#txtOutletStreet').val(data.Street);
                            $('#selOutletCity').val(data.City);
                            $('#selOutletCountry').val(data.Country);
                            AddOutletTelInfo();
                            AddOutletMobileInfo();
                            AddOutletEmailInfo();
                            DisplayCompanyOutletDepartment();
                        })
                    )
                }
            }
            lstCompanyOutlet.listview('refresh');
        }

        function DisplayRevPriceLevels() {
            var lstOrgPriceLevel = $('#lstOrgPriceLevel');

            lstOrgPriceLevel.empty();

            for (var i = 0; i < arrRevPriceLevels.length; i++) {
                arrRevPriceLevels[i].index = i;
                if (arrRevPriceLevels[i].State != 'remove') {
                    lstOrgPriceLevel.append($('<li>')
                            .data(arrRevPriceLevels[i])
                            .attr('data-icon', 'delete')
                            .append($('<a>')
                                    .html($.trim(arrRevPriceLevels[i].PriceLevel))
                            )
                            .append($('<a>')
                                    .data(arrRevPriceLevels[i])
                                    .click(function (e) {
                                        var data = $(this).data();
                                        if (data.State == 'add') {
                                            arrRevPriceLevels = $.grep(arrRevPriceLevels, function (e) {
                                                return e.ID != data.ID;
                                            })
                                        } else {
                                            arrRevPriceLevels[data.index].State = 'remove';
                                        }

                                        DisplayRevPriceLevels();
                                    })
                            )
                        )
                }
            }

            lstOrgPriceLevel.listview('refresh');
        }

        function SortByName(a, b) {
            var aName = a.Name.toLowerCase();
            var bName = b.Name.toLowerCase();
            return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
        }

        function AddAffiliatedPerson() {
            var lstAffiliatedPerson = $('#lstAffiliatedPerson');

            lstAffiliatedPerson.empty();

            for (var i = 0; i < arrOrgAffPersonList.length; i++) {
                if (arrOrgAffPersonList[i].State != 'Remove') {
                    lstAffiliatedPerson.append($('<li>')
                        .data(arrOrgAffPersonList[i])
                        .attr('data-icon', 'delete')
                        .append($('<a>')
                            .data(arrOrgAffPersonList[i])
                            .text(arrOrgAffPersonList[i].PeopleName)
                        )
                        .append($('<a>')
                            .attr('arrindex', i)
                            .data('data', arrOrgAffPersonList[i])
                            .click(function (e) {
                                var id = $(this).data('data').PeopleID;

                                if ($(this).data('data').IsNew == 1) {
                                    //remove selected email
                                    arrOrgAffPersonList = $.grep(arrOrgAffPersonList, function (val) {
                                        return val.PeopleID != id;
                                    })
                                }
                                else {
                                    arrOrgAffPersonList[$(this).attr('arrindex')].State = "Remove";
                                }

                                AddAffiliatedPerson();
                            })
                        )
                    ).listview().listview('refresh')
                }
            }
        }

        function AddOrgAffiliation() {
            var lstOrgAffiliation = $('#lstOrgAffiliation');

            lstOrgAffiliation.empty();

            for (var i = 0; i < arrOrgAffList.length; i++) {
                if (arrOrgAffList[i].State != 'Remove') {
                    lstOrgAffiliation.append($('<li>')
                        .data(arrOrgAffList[i])
                        .attr('data-icon', 'delete')
                        .append($('<a>')
                            .data(arrOrgAffList[i])
                            .text(arrOrgAffList[i].CompanyName)
                        )
                        .append($('<a>')
                            .attr('arrindex', i)
                            .data('data', arrOrgAffList[i])
                            .click(function (e) {
                                var id = $(this).data('data').OrgID;

                                if ($(this).data('data').IsNew == 1) {
                                    //remove selected email
                                    arrOrgAffList = $.grep(arrOrgAffList, function (val) {
                                        return val.OrgID != id;
                                    })
                                }
                                else {
                                    arrOrgAffList[$(this).attr('arrindex')].State = "Remove";
                                }

                                AddOrgAffiliation();
                            })
                        )
                    ).listview().listview('refresh')
                }
            }
        }

        function AddPersonOrgAffiliation() {
            var lstPersonOrgAffiliation = $('#lstPersonOrgAffiliation');

            lstPersonOrgAffiliation.empty();

            for (var i = 0; i < arrPersonOrgAffList.length; i++) {
                if (arrPersonOrgAffList[i].State != 'Remove') {
                    lstPersonOrgAffiliation.append($('<li>')
                        .data(arrPersonOrgAffList[i])
                        .attr('data-icon', 'delete')
                        .append($('<a>')
                            .data(arrPersonOrgAffList[i])
                            .text(arrPersonOrgAffList[i].CompanyName)
                        )
                        .append($('<a>')
                            .attr('arrindex', i)
                            .data('data', arrPersonOrgAffList[i])
                            .click(function (e) {
                                var id = $(this).data('data').OrgID;

                                if ($(this).data('data').IsNew == 1) {
                                    //remove selected email
                                    arrPersonOrgAffList = $.grep(arrPersonOrgAffList, function (val) {
                                        return val.OrgID != id;
                                    })
                                }
                                else {
                                    arrPersonOrgAffList[$(this).attr('arrindex')].State = "Remove";
                                }

                                AddPersonOrgAffiliation();
                            })
                        )
                    ).listview().listview('refresh')
                }
            }
        }

        function SaveNewPersonProspect(autosave) {
            var pData = {}, hasError = false;
            var txtPersonUsername = $('#txtPersonUsername');
            var txtPersonPassword = $('#txtPersonPassword');
            var txtPersonConfirmPassword = $('#txtPersonConfirmPassword');
            var selPersonSecQuestion1 = $('#selPersonSecQuestion1');
            var txtPersonSecQuestion1Answer = $('#txtPersonSecQuestion1Answer');
            var selPersonSecQuestion2 = $('#selPersonSecQuestion2');
            var txtPersonSecQuestion2Answer = $('#txtPersonSecQuestion2Answer');

            $('#divPersonErrMsg').text('Please fill in the required field(s) in every tab.').hide();
            $('#btnSavePersonAddingContact').text('SAVING ...').attr('disabled', 'disabled');

            if ($.trim($('#txtProspectPersonFirstName').val()) == '') {
                $('#txtProspectPersonFirstName').parent().addClass('has-error');
                hasError = true;
            } else {
                $('#txtProspectPersonFirstName').parent().removeClass('has-error');
            }

            if ($.trim($('#txtProspectPersonLastName').val()) == '') {
                $('#txtProspectPersonLastName').parent().addClass('has-error');
                hasError = true;
            } else {
                $('#txtProspectPersonLastName').parent().removeClass('has-error');
            }

            if ($.trim($('#txtPersonHouseBuildingNo').val()) == '') {
                $('#txtPersonHouseBuildingNo').parent().addClass('has-error');
                hasError = true;
            } else {
                $('#txtPersonHouseBuildingNo').parent().removeClass('has-error');
            }

            if ($.trim($('#txtPersonStreet').val()) == '') {
                $('#txtPersonStreet').parent().addClass('has-error');
                hasError = true;
            } else {
                $('#txtPersonStreet').parent().removeClass('has-error');
            }

            if ($.trim($('#selPersonCity').val()) == '') {
                $('#selPersonCity').parent().addClass('has-error');
                hasError = true;
            } else {
                $('#selPersonCity').parent().removeClass('has-error');
            }

            if ($.trim($('#selPersonCountry').val()) == '') {
                $('#selPersonCountry').parent().addClass('has-error');
                hasError = true;
            } else {
                $('#selPersonCountry').parent().removeClass('has-error');
            }

            if ($.trim($('#txtProspectEmailAddress').val()) == '') {
                $('#txtProspectEmailAddress').parent().addClass('has-error');
                hasError = true;
            } else {
                $('#txtProspectEmailAddress').parent().removeClass('has-error');

                if (!IsEmail($.trim($('#txtProspectEmailAddress').val()))) {
                    $('#divPersonErrMsg').append('<br>').append('- Please enter valid email address');
                    $('#txtProspectEmailAddress').parent().addClass('has-error');
                    hasError = true;
                } else {
                    $('#txtProspectEmailAddress').parent().removeClass('has-error');
                }
            }

            if ($.trim(txtPersonUsername.val()) == '') {
                txtPersonUsername.parent().addClass('has-error');
                hasError = true;
            } else {
                if ($.trim(txtPersonUsername.val()).length < 8) {
                    $('#divPersonErrMsg').append('<br>').append('- Username must be 8 characters or more');
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
                    $('#divPersonErrMsg').append('<br>').append('- Password must be 8 characters or more');
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
                    $('#divPersonErrMsg').append('<br>').append('- Confirm password don\'t match');
                    hasError = true;
                } else {
                    txtPersonConfirmPassword.parent().removeClass('has-error');
                }
            }

            if ($.trim(selPersonSecQuestion1.val()) == '') {
                selPersonSecQuestion1.parent().addClass('has-error');
                hasError = true;
            } else {
                selPersonSecQuestion1.parent().removeClass('has-error');
            }

            if ($.trim(txtPersonSecQuestion1Answer.val()) == '') {
                txtPersonSecQuestion1Answer.parent().addClass('has-error');
                hasError = true;
            } else {
                txtPersonSecQuestion1Answer.parent().removeClass('has-error');
            }

            if ($.trim(selPersonSecQuestion2.val()) == '') {
                selPersonSecQuestion2.parent().addClass('has-error');
                hasError = true;
            } else {
                selPersonSecQuestion2.parent().removeClass('has-error');
            }

            if ($.trim(txtPersonSecQuestion2Answer.val()) == '') {
                txtPersonSecQuestion2Answer.parent().addClass('has-error');
                hasError = true;
            } else {
                txtPersonSecQuestion2Answer.parent().removeClass('has-error');
            }

            if (hasError) {
                $('#divPersonErrMsg').show();
                $('#btnSavePersonAddingContact').text('SAVE').removeAttr('disabled');
                return;
            }

            if (arrPersonEmailInfoList.length > 0) {
                if (arrPersonEmailInfoList[0].EmailAdd != $('#txtProspectEmailAddress').val()) {
                    arrPersonEmailInfoList[0].EmailAddID = '';
                    arrPersonEmailInfoList[0].EmailAdd = $('#txtProspectEmailAddress').val();
                    arrPersonEmailInfoList[0].EmailState = 'Add';
                    arrPersonEmailInfoList[0].IsEmailConfirmed = 0;
                    arrPersonEmailInfoList[0].IsNew = 1;
                }
            } else {
                arrPersonEmailInfoList.push({
                    "EmailRankNo": 1,
                    "EmailAddID": '',
                    "EmailAdd": $('#txtProspectEmailAddress').val(),
                    "EmailType": "Personal Email",
                    "EmailState": "Add",
                    "IsEmailConfirmed": 0,
                    "IsNew": 1
                })
            }

            var msg, button = [], isSendToEmail = false;
            if (arrPersonEmailInfoList.length > 0) {
                if ($.trim(arrPersonEmailInfoList[0].EmailAdd) != $.trim($('#txtProspectEmailAddress').val())) {
                    msg = 'We notice that you change your email address from ' + arrPersonEmailInfoList[0].EmailAdd + ' to ' + $.trim($('#txtProspectEmailAddress').val()) + '. <br><br> Is this email addres is correct?' +
                        ' If Yes, we will send an activation link to <b>' + $.trim($('#txtProspectEmailAddress').val()) + '</b>.';
                    button = [
                        {
                            Text: 'Yes, the email address is correct',
                            Click: function (e) {
                                isSendToEmail = true;
                                SavePersonNow();
                            }
                        }, {
                            Text: 'No'
                        }
                    ]
                } else {
                    if (CBool(arrPersonEmailInfoList[0].IsEmailConfirmed)) {
                        msg = 'Your email address (<b>' + $.trim($('#txtProspectEmailAddress').val()) + '</b>) was already confirmed, no need for activation link.';
                        button = [
                            {
                                Text: 'Okay then save my info',
                                Click: function (e) {
                                    isSendToEmail = false;
                                    SavePersonNow();
                                }
                            }, {
                                Text: 'No'
                            }
                        ]
                    } else {
                        msg = 'We will send an activation link to <b>' + $.trim($('#txtProspectEmailAddress').val()) + '</b>. Is this email address is correct?';
                        button = [
                            {
                                Text: 'Yes, the email address is correct',
                                Click: function (e) {
                                    isSendToEmail = true;
                                    SavePersonNow();
                                }
                            }, {
                                Text: 'No'
                            }
                        ]
                    }
                }
            } else {
                msg = 'We will send an activation link to <b>' + $.trim($('#txtProspectEmailAddress').val()) + '</b>. Is this email address is correct?';
                button = [
                    {
                        Text: 'Yes, the email address is correct',
                        Click: function (e) {
                            isSendToEmail = true;
                            SavePersonNow();
                        }
                    }, {
                        Text: 'No'
                    }
                ]
            }

            NewMessageBox(msg, 'Message', button, {
                AutoDismiss: false
            })

            function SavePersonNow() {
                pData = {
                    isCreateFromExistingRecord: isCreateFromExistingRecord,
                    Telphone: arrPersonTelInfoList,
                    Mobile: arrPersonMobileInfoList,
                    Email: arrPersonEmailInfoList,
                    OrgAffiliation: arrPersonOrgAffList,
                    PersonID: $('#txtProspectPersonID').val(),
                    CustomerID: $('#txtCustomerPersonID').val(),
                    ExistingCustomerID: $('#txtExistingCustomerPersonID').val(),
                    ThirdPartyID: $('#txtProspectPerson3rdPartyID').val(),
                    CodeNumber: $('.txtCodeNumber').val(),
                    FirstName: $.trim($('#txtProspectPersonFirstName').val()),
                    MiddleName: $.trim($('#txtProspectPersonMiddleName').val()),
                    LastName: $.trim($('#txtProspectPersonLastName').val()),
                    Tin: $.trim($('#txtProspectPersonTinNumber').val()),
                    BirthDate: (IsDate($('#btnPersonBirthDate').text()) ? $('#btnPersonBirthDate').text() : ""),
                    Position: $('#txtProspectPersonPosition').val(),
                    PositionDescription: $('#txtProspectPersonPositionDescription').val(),
                    AddressID: $('#txtPersonAddressID').val(),
                    HouseBuildingNo: $('#txtPersonHouseBuildingNo').val(),
                    Street: $('#txtPersonStreet').val(),
                    City: $('#selPersonCity').val(),
                    CityName: $('#selPersonCity option:selected').text(),
                    Region: $('#selPersonRegion').val(),
                    Island: $('#selPersonIsland').val(),
                    Country: $('#selPersonCountry').val(),
                    CountryName: $('#selPersonCountry option:selected').text(),
                    TypeAddress: $('#selPersonTypeAddress').val(),
                    OrgID: sessionStorage.OrgID,
                    DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a'),
                    Status: $('#selPersonStatus').val(),
                    Elementary: arrElementary,
                    HighSchool: arrHighSchool,
                    College: arrCollege,

                    SalesRepID: RevSalesRepID,
                    PriceLevel: arrRevPriceLevels,
                    Terms: RevPaymentTerms,
                    Limit: $('#txtRevCreditLimit').val(),
                    Balance: $('#txtRevCreditBegBal').val(),
                    AsOf: $('#btnRevCreditBegAsOf').text(),
                    ThirdPartyBalance: $('#txtRevThirdPartyBegBalance').val(),
                    ThirdPartyAsOf: $('#btnRevThirdPartyBegBalanceAsOf').text(),

                    LoginID: (arrPersonLoginCredentials.length > 0 ? arrPersonLoginCredentials[0].ID : ''),
                    Username: $('#txtPersonUsername').val(),
                    Password: $('#txtPersonConfirmPassword').val(),
                    SecQ1ID: (arrPersonSecQue.length > 0 ? arrPersonSecQue[0].ID : ''),
                    SecQ1: $('#selPersonSecQuestion1').val(),
                    SecQ1Ans: $('#txtPersonSecQuestion1Answer').val(),
                    SecQ2ID: (arrPersonSecQue.length > 0 ? arrPersonSecQue[1].ID : ''),
                    SecQ2: $('#selPersonSecQuestion2').val(),
                    SecQ2Ans: $('#txtPersonSecQuestion2Answer').val(),

                    IsCustomerSettings: (GetUrlParameters().mod == 'Customer' ? true : false),

                    IsFromGeneratedPublicURL: true,
                    Link: GetUrlParameters().id,
                    Url: location.origin
                }
                //console.log(pData);
                //return;
                AjaxSend({
                    url: 'Ecommerce/SaveCustomerPersonalInfo',
                    data: pData,
                    success: function (vData) {
                        isEditingProfile = false;
                        if (IsGuid(vData.Results) && autosave == undefined) {
                            $('#divSavedSuccess').pageshow();

                            if (isSendToEmail) {
                                $('.foremail').show();
                                $('#lblSentTo').text($.trim($('#txtProspectEmailAddress').val()));
                            }

                            $('.lblUserType').text(GetUrlParameters().mod.toLowerCase());
                        }
                        else if (!IsGuid(vData.Results)) {
                            NewMessageBox(vData.Results, 'Message');
                        }

                        $('#btnSavePersonAddingContact').text('SAVE').removeAttr('disabled');
                    }
                })
            }            
        }

        function DisplayEducation() {
            var list, arr = [];
            if (academicLvl == 'elem') {
                list = $('#lstElementary');
                arr = arrElementary;
                ctrElem = arrElementary.length;
            } else if (academicLvl == 'hs') {
                list = $('#lstHs');
                arr = arrHighSchool;
                ctrHs = arrHighSchool.length;
            } else if (academicLvl == 'college') {
                list = $('#lstCollege');
                arr = arrCollege;
                ctrCollege = arrCollege.length;
            }

            list.empty();

            for (var i = 0; i < arr.length; i++) {
                if (arr[i].State != 'remove') {
                    list.append($('<li>')
                            .data(arr[i])
                            .attr('data-icon', 'delete')
                            .append($('<a>')
                                    .html($.trim(arr[i].Name))
                                    .data(arr[i])
                                    .click(function (e) {
                                        EditEducation($(this).data());
                                    })
                            )
                            .append($('<a>')
                                    .data(arr[i])
                                    .click(function (e) {

                                    })
                            )
                        )
                }
            }

            list.listview('refresh');
            $('#popEducation').popup('close');
        }

        function EditEducation(data) {
            ClearEducation();

            academicLvl = data.Level;
            ctrIndexEducation = CInt2(data.No) - 1;
            isNewEducation = false;

            if (data.Level == 'elem') {
                $('#txtAcademicLevel').text('Elementary');
            } else if (data.Level == 'hs') {
                $('#txtAcademicLevel').text('High School');
            } else if (data.Level == 'college') {
                $('#txtAcademicLevel').text('College');
            }

            $('#txtEdeHiSchoolName').val(data.Name).focus();
            $('#txtEdeHiYearStarted').val(data.Started);
            $('#txtEdeHiYearGraduated').val(data.Graduated);
        }

        function ClearEducation() {
            $('#popEducation').popup('open');
            $('#txtEdeHiID').val('');
            $('#txtEdeHiSchoolName').val('').focus();
            $('#txtEdeHiYearStarted').val('');
            $('#txtEdeHiYearGraduated').val('');
        }

        function AddPersonEmailInfo() {

            var pLV = $('#lstPersonEmail');
            var pItem;

            pLV.html('');
            for (var i = 0; i < arrPersonEmailInfoList.length; i++) {
                if (arrPersonEmailInfoList[i].EmailState != "Remove") {
                    arrPersonEmailInfoList[i].arrindex = i;
                    pItem = $('<li data-icon="delete">')
                            .data(arrPersonEmailInfoList[i])
                            .append($('<a>')
                                    .html($.trim(arrPersonEmailInfoList[i].EmailAdd) +
                                        "<br>" + $.trim(arrPersonEmailInfoList[i].EmailType)
                                    )
                                    .data(arrPersonEmailInfoList[i])
                                    .click(function (e) {
                                        EditPersonEmailInfo($(this).data().arrindex);
                                    })
                            )
                            .append($('<a>')
                                    .data('data', arrPersonEmailInfoList[i])
                                    .click(function (e) {
                                        var EmailRankNo = $(this).data('data').EmailRankNo;

                                        if ($(this).data('data').IsNew == 1) {
                                            //remove selected email
                                            arrPersonEmailInfoList = $.grep(arrPersonEmailInfoList, function (val) {
                                                return val.EmailRankNo != EmailRankNo;
                                            })
                                        }
                                        else {
                                            arrPersonEmailInfoList[$(this).data('data').arrindex].EmailState = "Remove";
                                        }

                                        AddPersonEmailInfo();
                                    })
                            )

                    pLV.append(pItem);
                }
            }

            pLV.listview('refresh');
            $('#popPersonEmailInfo').hide();
        }

        function EditPersonEmailInfo(arrindex) {
            $('#popPersonEmailInfo').show();
            PersonEmailRankNo = arrPersonEmailInfoList[arrindex].EmailRankNo;
            PersonEmailState = "Edit";
            sessionStorage["Emailindex"] = arrindex;
            sessionStorage["EmailIsNew"] = arrPersonEmailInfoList[arrindex].IsNew;
            sessionStorage["EmailState"] = arrPersonEmailInfoList[arrindex].EmailState;
            $('#txtPersonEmailID').val(arrPersonEmailInfoList[arrindex].EmailAddID == undefined ? "" : arrPersonEmailInfoList[arrindex].EmailAddID);
            $('#txtPersonEmailAdd').val(arrPersonEmailInfoList[arrindex].EmailAdd);
            $('#selPersonEmailType').val(arrPersonEmailInfoList[arrindex].EmailType);
        }

        function ClearPersonEmailInfoFields() {
            $('#txtPersonEmailID').val('');
            $('#txtPersonEmailAdd').val('');
            $('#selPersonEmailType').val('');
        }

        function AddPersonMobileInfo() {

            var pLV = $('#lstPersonMobile');
            var pItem;

            pLV.html('');
            for (var i = 0; i < arrPersonMobileInfoList.length; i++) {
                if (arrPersonMobileInfoList[i].MobileState != "Remove") {
                    arrPersonMobileInfoList[i].arrindex = i;
                    pItem = $('<li data-icon="delete">')
                                .data(arrPersonMobileInfoList[i])
                                .append($('<a>')
                                        .data(arrPersonMobileInfoList[i])
                                        .attr('ID', arrPersonMobileInfoList[i].id)
                                        .html("+" + $.trim(arrPersonMobileInfoList[i].CountryCode.split('-')[1]) + "" + $.trim(arrPersonMobileInfoList[i].MobileNo) +
                                            "<br>" + $.trim(arrPersonMobileInfoList[i].ContactType)
                                        )
                                        .click(function (e) {
                                            EditPersonMobileInfo($(this).data().arrindex);
                                        })
                                )
                                .append($('<a>')
                                        .data('data', arrPersonMobileInfoList[i])
                                        .click(function (e) {
                                            var MobileRankNo = $(this).data('data').MobileRankNo;

                                            if ($(this).data('data').IsNew == 1) {
                                                //remove selected mobile
                                                arrPersonMobileInfoList = $.grep(arrPersonMobileInfoList, function (val) {
                                                    return val.MobileRankNo != MobileRankNo;
                                                })
                                            }
                                            else {
                                                arrPersonMobileInfoList[$(this).data('data').arrindex].MobileState = "Remove";
                                            }

                                            AddPersonMobileInfo();
                                        })
                                )

                    pLV.append(pItem);
                }
            }

            pLV.listview('refresh');
            $('#popPersonMobileInfo').hide();
        }

        function EditPersonMobileInfo(arrindex) {
            $('#popPersonMobileInfo').show();
            PersonMobileRankNo = arrPersonMobileInfoList[arrindex].MobileRankNo;
            PersonMobileState = "Edit";
            sessionStorage["Mobileindex"] = arrindex;
            sessionStorage["MobileIsNew"] = arrPersonMobileInfoList[arrindex].IsNew;
            sessionStorage["MobileState"] = arrPersonMobileInfoList[arrindex].MobileState;
            $('#txtPersonMobileID').val(arrPersonMobileInfoList[arrindex].MobileID == undefined ? "" : arrPersonMobileInfoList[arrindex].MobileID);
            $('#txtPersonTelCoPrefixID').val(arrPersonMobileInfoList[arrindex].TelCodeID);
            $('#txtPersonMobileCountryID').val(arrPersonMobileInfoList[arrindex].CountryCodeID);
            $('#txtPersonMobileCountryCode').text(arrPersonMobileInfoList[arrindex].CountryCode);
            //$('#txtPersonTelCode').text(arrPersonMobileInfoList[arrindex].TelCode);
            $('#txtPersonMobileNo').val(arrPersonMobileInfoList[arrindex].MobileNo);
            $('#selPersonMobileType').val(arrPersonMobileInfoList[arrindex].ContactType);
        }

        function ClearPersonMobileInfo() {
            $('#txtPersonMobileID').val('');
            $('#txtPersonTelCoPrefixID').val('');
            $('#txtPersonMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
            $('#txtPersonMobileCountryCode').text('PHL - 63');
            //$('#txtPersonTelCode').text('Select');
            $('#txtPersonMobileNo').val('');
            $('#selPersonMobileType').val('');
        }

        function AddPersonTelInfo() {

            var pLV = $('#lstPersonTelephone');
            var pItem;

            pLV.html('');
            for (var i = 0; i < arrPersonTelInfoList.length; i++) {
                if (arrPersonTelInfoList[i].TelState != "Remove") {
                    arrPersonTelInfoList[i].arrindex = i;
                    pItem = $('<li>')
                            .data(arrPersonTelInfoList[i])
                            .attr('data-icon', 'delete')
                            .append($('<a>')
                                    .html("+" + $.trim(arrPersonTelInfoList[i].CountryCode.split('-')[1]) + " (" + $.trim(arrPersonTelInfoList[i].AreaCode.split('-')[1]) + ") " + $.trim(arrPersonTelInfoList[i].TelephoneNo) + ($.trim(arrPersonTelInfoList[i].ExtensionNo) == "" ? "" : "." + $.trim(arrPersonTelInfoList[i].ExtensionNo)) +
                                        "<br>" + $.trim(arrPersonTelInfoList[i].AreaCode.split('-')[0]) + ", " + $.trim(arrPersonTelInfoList[i].CountryCode.split('-')[0]) +
                                        "<br>" + $.trim(arrPersonTelInfoList[i].ContactType)
                                    )
                                    .data('data', arrPersonTelInfoList[i])
                                    .click(function (e) {
                                        EditPersonTelInfo($(this).data('data').arrindex);
                                    })
                            )
                            .append($('<a>')
                                    .data('data', arrPersonTelInfoList[i])
                                    .click(function (e) {
                                        var TelRankNo = $(this).data('data').TelRankNo;

                                        if ($(this).data('data').IsNew == 1) {
                                            //remove selected telephone
                                            arrPersonTelInfoList = $.grep(arrPersonTelInfoList, function (val) {
                                                return val.TelRankNo != TelRankNo;
                                            })
                                        }
                                        else {
                                            arrPersonTelInfoList[$(this).data('data').arrindex].TelState = "Remove";
                                        }

                                        AddPersonTelInfo();
                                    })
                            )

                    pLV.append(pItem);
                }

            }

            pLV.listview('refresh');
            $('#popPersonTelInfo').hide();
        }

        function EditPersonTelInfo(arrindex) {
            $('#popPersonTelInfo').show();

            PersonTelRankNo = arrPersonTelInfoList[arrindex].TelRankNo;
            PersonTelState = "Edit";
            sessionStorage["Telindex"] = arrindex;
            sessionStorage["TelIsNew"] = arrPersonTelInfoList[arrindex].IsNew;
            sessionStorage["TelState"] = arrPersonTelInfoList[arrindex].TelState;
            $('#txtPersonTelID').val(arrPersonTelInfoList[arrindex].TelID == undefined ? "" : arrPersonTelInfoList[arrindex].TelID);
            $('#txtPersonTelAreaCodeID').val(arrPersonTelInfoList[arrindex].AreaCodeID);
            $('#txtPersonTelCountryID').val(arrPersonTelInfoList[arrindex].CountryCodeID);
            $('#txtPersonCountryCode').text(arrPersonTelInfoList[arrindex].CountryCode);
            $('#txtPersonAreaCode').text(arrPersonTelInfoList[arrindex].AreaCode);
            $('#txtPersonTelephoneNum').val(arrPersonTelInfoList[arrindex].TelephoneNo);
            $('#txtPersonTelephoneExt').val(arrPersonTelInfoList[arrindex].ExtensionNo);
            $('#selPersonTelType').val(arrPersonTelInfoList[arrindex].ContactType);
        }

        function ClearPersonTelInfo() {
            $('#txtPersonTelID').val('');
            $('#txtPersonTelAreaCodeID').val('');
            $('#txtPersonTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
            $('#txtPersonCountryCode').text('PHL - 63');
            $('#txtPersonAreaCode').text('Select');
            $('#txtPersonTelephoneNum').val('');
            $('#txtPersonTelephoneExt').val('');
            $('#selPersonTelType').val('');
        }

        function SaveNewOrgProspect(autosave) {
            var pData = {};
            var txtPersonFirstName = $('#txtPersonFirstName');
            var txtPersonMiddleName = $('#txtPersonMiddleName');
            var txtPersonLastName = $('#txtPersonLastName');
            var selPersonGenSuffix = $('#selPersonGenSuffix');
            var selPersonBirthMonth = $('#selPersonBirthMonth');
            var selPersonBirthDay = $('#selPersonBirthDay');
            var txtPersonBirthYear = $('#txtPersonBirthYear');
            var txtPersonEmailAdd = $('#txtPersonEmailAdd');
            var txtAdminUsername = $('#txtAdminUsername');
            var txtAdminPassword = $('#txtAdminPassword');
            var txtAdminConfirmPassword = $('#txtAdminConfirmPassword');
            var selSecQuestion1 = $('#selSecQuestion1');
            var txtSecQuestion1Answer = $('#txtSecQuestion1Answer');
            var selSecQuestion2 = $('#selSecQuestion2');
            var txtSecQuestion2Answer = $('#txtSecQuestion2Answer');
            var txtProspectCompanyName = $('#txtProspectCompanyName');
            var txtProspectOrgTin = $('#txtProspectOrgTin');
            var hasError = false, data = {};

            $('#divErrMsg').text('Please fill in the required field(s) in every tab.').hide();
            $('#btnSaveAddingContact').text('SAVING ...').attr('disabled', 'disabled');

            if ($.trim(txtProspectCompanyName.val()) == '') {
                txtProspectCompanyName.parent().addClass('has-error');
                hasError = true;
            } else {
                txtProspectCompanyName.parent().removeClass('has-error');
            }

            if ($.trim(txtProspectOrgTin.val()) == '') {
                txtProspectOrgTin.parent().addClass('has-error');
                hasError = true;
            } else {
                txtProspectOrgTin.parent().removeClass('has-error');
            }

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

            if ($.trim(txtPersonEmailAdd.val()) == '' || !IsEmail($.trim(txtPersonEmailAdd.val()))) {
                $('#divErrMsg').append('<br>').append('- Please provide valid email address');
                txtPersonEmailAdd.parent().addClass('has-error');
                hasError = true;
            } else {
                txtPersonEmailAdd.parent().removeClass('has-error');
            }

            if ($.trim(txtAdminUsername.val()) == '') {
                txtAdminUsername.parent().addClass('has-error');
                hasError = true;
            } else {
                if ($.trim(txtAdminUsername.val()).length < 8) {
                    $('#divErrMsg').append('<br>').append('- Username must be 8 characters or more');
                    txtAdminUsername.parent().addClass('has-error');
                    hasError = true;
                } else {
                    txtAdminUsername.parent().removeClass('has-error');
                }
            }

            if ($.trim(txtAdminPassword.val()) == '') {
                txtAdminPassword.parent().addClass('has-error');
                hasError = true;
            } else {
                if ($.trim(txtAdminPassword.val()).length < 8) {
                    $('#divErrMsg').append('<br>').append('- Password must be 8 characters or more');
                    txtAdminPassword.parent().addClass('has-error');
                    hasError = true;
                } else {
                    txtAdminPassword.parent().removeClass('has-error');
                }
            }

            if ($.trim(txtAdminConfirmPassword.val()) == '') {
                txtAdminConfirmPassword.parent().addClass('has-error');
                hasError = true;
            } else {
                if ($.trim(txtAdminPassword.val()) != $.trim(txtAdminConfirmPassword.val())) {
                    $('#divErrMsg').append('<br>').append('- Confirm password don\'t match');
                    hasError = true;
                } else {
                    txtAdminConfirmPassword.parent().removeClass('has-error');
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

            if ($.trim($('#txtProspectHouseBldgNo').val()) == '') {
                $('#txtProspectHouseBldgNo').parent().addClass('has-error');
                hasError = true;
            } else {
                $('#txtProspectHouseBldgNo').parent().removeClass('has-error');
            }

            if ($.trim($('#txtProspectStreetName').val()) == '') {
                $('#txtProspectStreetName').parent().addClass('has-error');
                hasError = true;
            } else {
                $('#txtProspectStreetName').parent().removeClass('has-error');
            }

            if ($.trim($('#selProspectCities').val()) == '') {
                $('#selProspectCities').parent().addClass('has-error');
                hasError = true;
            } else {
                $('#selProspectCities').parent().removeClass('has-error');
            }

            if ($.trim($('#selProspectCountries').val()) == '') {
                $('#selProspectCountries').parent().addClass('has-error');
                hasError = true;
            } else {
                $('#selProspectCountries').parent().removeClass('has-error');
            }

            if (hasError) {
                $('#divErrMsg').show();
                $('#btnSaveAddingContact').text('SAVE').removeAttr('disabled');
                return;
            }            

            if (arrContactPersonList.length == 0) {
                arrContactPersonList.push({
                    "ID": '',
                    "Firstname": $('#txtPersonFirstName').val(),
                    "Middlename": $('#txtPersonMiddleName').val(),
                    "Lastname": $('#txtPersonLastName').val(),
                    "Username": $('#txtAdminUsername').val(),
                    "Password": $('#txtAdminConfirmPassword').val(),
                    "GenerationSuffix": $('#selPersonGenSuffix').val(),
                    "DateOfBirth": $('#selPersonBirthMonth').val() + ' ' + $('#selPersonBirthDay').val() + ', ' + $('#txtPersonBirthYear').val(),
                    "SecQ1ID": '',
                    "SecQ1": $('#selSecQuestion1').val(),
                    "SecQ1Ans": $('#txtSecQuestion1Answer').val(),
                    "SecQ2ID": '',
                    "SecQ2": $('#selSecQuestion2').val(),
                    "SecQ2Ans": $('#txtSecQuestion2Answer').val(),
                    "Position": '',
                    "PositionDescription": '',
                    "AddressID": NewID(),
                    "HouseBuildingNo": '',
                    "Street": '',
                    "City": '',
                    "Country": '',
                    "TypeAddress": 'Permanent Address',
                    "ContactPersonState": "Add",
                    "IsNew": 1,
                    "Tel": [],
                    "Mobile": [],
                    "Email": [{
                        IsEmailConfirmed: false,
                        "EmailRankNo": 1,
                        "EmailAddID": '',
                        "EmailAdd": $.trim(txtPersonEmailAdd.val()),
                        "EmailType": 'Personal Email',
                        "EmailState": "Add",
                        "IsNew": 1,
                    }]
                })
            } else {
                arrContactPersonList[0].Firstname = $('#txtPersonFirstName').val();
                arrContactPersonList[0].Middlename = $('#txtPersonMiddleName').val();
                arrContactPersonList[0].Lastname = $('#txtPersonLastName').val();
                arrContactPersonList[0].Username = $('#txtAdminUsername').val();
                arrContactPersonList[0].Password = $('#txtAdminConfirmPassword').val();
                arrContactPersonList[0].GenerationSuffix = $('#selPersonGenSuffix').val();
                arrContactPersonList[0].DateOfBirth = $('#selPersonBirthMonth').val() + ' ' + $('#selPersonBirthDay').val() + ', ' + $('#txtPersonBirthYear').val();
                arrContactPersonList[0].SecQ1ID = (arrSecQue.length > 0 ? arrSecQue[0].ID : '');
                arrContactPersonList[0].SecQ1 = $('#selSecQuestion1').val();
                arrContactPersonList[0].SecQ1Ans = $('#txtSecQuestion1Answer').val();
                arrContactPersonList[0].SecQ2ID = (arrSecQue.length > 0 ? arrSecQue[1].ID : '');
                arrContactPersonList[0].SecQ2 = $('#selSecQuestion2').val();
                arrContactPersonList[0].SecQ2Ans = $('#txtSecQuestion2Answer').val();
                arrContactPersonList[0].Email = [{
                    "EmailRankNo": 1,
                    "IsEmailConfirmed": (arrContactPersonEmailInfoList.length > 0 ? arrContactPersonEmailInfoList[0].IsEmailConfirmed : false),
                    "EmailAddID": (arrContactPersonEmailInfoList.length > 0 ? arrContactPersonEmailInfoList[0].EmailAddID : ''),
                    "EmailAdd": $.trim(txtPersonEmailAdd.val()),
                    "EmailType": 'Personal Email',
                    "EmailState": (arrContactPersonEmailInfoList.length > 0 ? 'Edit' : 'Add'),
                    "IsNew": 1,
                }]
            }

            var msg, button = [], isSendToEmail = false;
            if (arrContactPersonEmailInfoList.length > 0) {
                if ($.trim(arrContactPersonEmailInfoList[0].EmailAdd) != $.trim(txtPersonEmailAdd.val())) {
                    msg = 'We notice that you change your email address from ' + arrContactPersonEmailInfoList[0].EmailAdd + ' to ' + $.trim(txtPersonEmailAdd.val()) + '. <br><br> Is this email addres is correct?' +
                        ' If Yes, we will send an activation link to <b>' + $.trim(txtPersonEmailAdd.val()) + '</b>.';
                    button = [
                        {
                            Text: 'Yes, the email address is correct',
                            Click: function (e) {
                                isSendToEmail = true;
                                SaveOrgNow();
                            }
                        }, {
                            Text: 'No'
                        }
                    ]
                } else {
                    if (CBool(arrContactPersonEmailInfoList[0].IsEmailConfirmed)) {
                        msg = 'Your email address (<b>' + $.trim(txtPersonEmailAdd.val()) + '</b>) was already confirmed, no need for activation link.';
                        button = [
                            {
                                Text: 'Okay then save my info',
                                Click: function (e) {
                                    isSendToEmail = false;
                                    SaveOrgNow();
                                }
                            }, {
                                Text: 'No'
                            }
                        ]
                    } else {
                        msg = 'We will send an activation link to <b>' + $.trim(txtPersonEmailAdd.val()) + '</b>. Is this email address is correct?';
                        button = [
                            {
                                Text: 'Yes, the email address is correct',
                                Click: function (e) {
                                    isSendToEmail = true;
                                    SaveOrgNow();
                                }
                            }, {
                                Text: 'No'
                            }
                        ]
                    }
                }
            } else {
                msg = 'We will send an activation link to <b>' + $.trim(txtPersonEmailAdd.val()) + '</b>. Is this email address is correct?';
                button = [
                    {
                        Text: 'Yes, the email address is correct',
                        Click: function (e) {
                            isSendToEmail = true;
                            SaveOrgNow();
                        }
                    }, {
                        Text: 'No'
                    }
                ]
            }

            NewMessageBox(msg, 'Message', button, {
                AutoDismiss: false
            })

            function SaveOrgNow() {
                pData = {
                    isCreateFromExistingRecord: isCreateFromExistingRecord,
                    Telphone: arrTelInfoList,
                    Mobile: arrMobileInfoList,
                    Email: arrEmailInfoList,
                    Website: arrWebsiteList,
                    Outlet: arrCompanyOutlet,
                    ContactPerson: arrContactPersonList,
                    ContactPersonTelphone: arrContactPersonTelInfoList,
                    ContactPersonMobile: arrContactPersonMobileInfoList,
                    ContactPersonEmail: arrContactPersonEmailInfoList,
                    OrgAffiliation: arrOrgAffList,
                    OrgAffiliatedPerson: arrOrgAffPersonList,
                    ThirdPartyPlatform: $('#txtProspectCompany3rdPartyPlatform').val(),
                    ThirdPartyID: $('#txtProspectCompany3rdPartyID').val(),
                    CodeNumber: $('.txtCodeNumber').val(),
                    CompanyID: $('#txtProspectOrgID').val(),
                    CustomerID: $('#txtCustomerOrgID').val(),
                    ExistingCustomerID: $('#txtExistingCustomerOrgID').val(),
                    CompanyName: $.trim($('#txtProspectCompanyName').val()),
                    Tin: $('#txtProspectOrgTin').val(),
                    CompanyBuildingAddress: $('#txtProspectHouseBldgNo').val(),
                    CompanyStreetAddress: $('#txtProspectStreetName').val(),
                    CompanyCityAddress: $('#selProspectCities').val(),
                    CompanyCityNameAddress: $('#selProspectCities option:selected').text(),
                    CompanyRegionAddress: $('#selProspectOrgRegion').val(),
                    Island: $('#selProspectIsland').val(),
                    CompanyCountryAddress: $('#selProspectCountries').val(),
                    CompanyCountryNameAddress: $('#selProspectCountries option:selected').text(),
                    CompanyBackground: $('#txtProspectCompanyBackground').val(),
                    CompanyMission: $('#txtProspectCompanyMission').val(),
                    CompanyVision: $('#txtProspectCompanyVision').val(),
                    CompanyFoundingDate: $('#txtProspectCompanyFounding').val(),
                    BusinessIndustryID: $('#txtBusIndustryID').val(),
                    BusinessIndustryText: $('#txtBusinessIndustryText').val(),
                    OrgID: sessionStorage.OrgID,
                    DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a'),
                    Status: $('#selOrgStatus').val(),
                    NoOfEmployees: $('#txtProspectNoOfEmployees').val(),
                    Revenue: $('#txtProspectRevenuePerYear').val(),
                    Scope: $('#selProspectScopeOperation').val(),
                    Factories: $('#txtProspectNoFactories').val(),
                    Offices: $('#txtProspectNoOffices').val(),
                    Warehouses: $('#txtProspectNoWarehouses').val(),
                    Stores: $('#txtProspectNoStores').val(),
                    Trucks: $('#txtProspectNoTrucks').val(),

                    SalesRepID: RevSalesRepID,
                    PriceLevel: arrRevPriceLevels,
                    Terms: RevPaymentTerms,
                    Limit: $('#txtRevCreditLimit').val(),
                    Balance: $('#txtRevCreditBegBal').val(),
                    AsOf: $('#btnRevCreditBegAsOf').text(),
                    ThirdPartyBalance: $('#txtRevThirdPartyBegBalance').val(),
                    ThirdPartyAsOf: $('#btnRevThirdPartyBegBalanceAsOf').text(),
                    IsCustomerSettings: (GetUrlParameters().mod == 'Customer' ? true : false),

                    IsFromGeneratedPublicURL: true,
                    Link: GetUrlParameters().id,
                    IsFromContactPersonLink: (GetUrlParameters().pid == undefined ? false : true),
                    Url: location.origin
                }

                AjaxSend({
                    url: 'Ecommerce/SaveSupplierInfo',
                    data: pData,
                    success: function (vData) {
                        isEditingProfile = false;
                        if (IsGuid(vData.Results) && autosave == undefined) {
                            $('#divSavedSuccess').pageshow();

                            if(isSendToEmail){
                                $('.foremail').show();
                                $('#lblSentTo').text($.trim(txtPersonEmailAdd.val()));
                            }

                            $('.lblUserType').text(GetUrlParameters().mod.toLowerCase());
                        }
                        else if (!IsGuid(vData.Results)) {
                            NewMessageBox(vData.Results, 'Message');
                        }

                        $('#btnSaveAddingContact').text('SAVE').removeAttr('disabled');
                    }
                })
            }
        }

        function ApplyNewDepartmentToOutlets() {
            var label = (sessionStorage.LibraryName == 'Customer / Client List' ? 'Customer' : 'Supplier');
            if (arrNewDepartment.length > 0) {
                NewMessageBox('New department(s) has been found in existing outlet(s) of ' + $('#txtProspectCompanyName').val() + '.<br><br> Would you like to add new department(s) to other ' + label + '\'s outlets?', 'Message', [
                    {
                        Text: 'Yes', Click: function () {
                            BrowsePopup((sessionStorage.LibraryName == 'Customer / Client List' ? 'LibraryOrganizationCustomer' : 'LibraryOrganizationSupplier'), 'Select ' + label, [
                                {
                                    Message: 'You can select multiple ' + label,
                                    SelectMultiple: true,
                                    AutoSearch: true,
                                    DisplayText: [{ Column: (sessionStorage.LibraryName == 'Customer / Client List' ? 'CustomerName' : 'SupplierName') }],
                                    Click: function (e) {
                                        AjaxSend({
                                            url: 'OrganizationShort/GetOutlet',
                                            data: { data: e },
                                            success: function (e) {
                                                var listNewOutlet = $('#listNewOutlet');
                                                isOutlet = false;
                                                listNewOutlet.empty();

                                                $('#lblNewOutletMsg').text('Selected department(s) will automatically add to selected customer\'s outlet(s).');

                                                for (var i = 0; i < e.Cus.length; i++) {
                                                    var ul = $('<ol id="' + e.Cus[i].DbID + '" data-role="none" style="list-style:lower-alpha">')

                                                    for (var o = 0; o < e.Cus[i].Outlet.length; o++) {
                                                        var deptul = $('<ol  id="' + e.Cus[i].Outlet[o].ID + '" data-role="none" style="list-style:none">')

                                                        for (var d = 0; d < arrNewDepartment.length; d++) {
                                                            deptul.append($('<li>')
                                                                    .data('data', arrNewDepartment[d])
                                                                    .append($('<div class="custom-checkbox">')
                                                                        .append($('<label class="checkbox-only">')
                                                                            .append($('<input type="checkbox" class="chbnewoutlet" checked data-iconpos="notext" />')
                                                                                .data('data', arrNewDepartment[d])
                                                                                .attr({
                                                                                    depid: arrNewDepartment[d].ID
                                                                                })
                                                                            )
                                                                        )
                                                                        .append($('<div>').text(arrNewDepartment[d].Department))
                                                                    )
                                                                )
                                                        }

                                                        ul.append($('<li>')
                                                            .data('data', e.Cus[i].Outlet[o])
                                                            .text(e.Cus[i].Outlet[o].Name)
                                                            .append(deptul)
                                                        )
                                                    }
                                                    listNewOutlet.append($('<li>')
                                                            .data('data', e.Cus[i])
                                                            .append((e.Cus[i].CustomerName || e.Cus[i].SupplierName))
                                                            .append(ul)
                                                        )
                                                }

                                                $('.chbnewoutlet').checkboxradio().checkboxradio('refresh');
                                                $('#popNewOutlet').show();
                                                arrNewDepartment = [];
                                            }
                                        })
                                    }
                                }
                            ], { FilterBy: 'Org' })
                        }
                    }, { Text: 'No' }
                ], { AutoDismiss: false })
            }
        }

        function AddContactPersonEmailInfo() {

            var pLV = $('#lstContactPersonEmail');
            var pItem;

            pLV.html('');
            for (var i = 0; i < arrContactPersonEmailInfoList.length; i++) {
                if (arrContactPersonEmailInfoList[i].EmailState != "Remove") {
                    arrContactPersonEmailInfoList[i].arrindex = i;
                    pItem = $('<li data-icon="delete">')
                            .data(arrContactPersonEmailInfoList[i])
                            .append($('<a>')
                                    .data(arrContactPersonEmailInfoList[i])
                                    .html($.trim(arrContactPersonEmailInfoList[i].EmailAdd) +
                                        "<br>" + $.trim(arrContactPersonEmailInfoList[i].EmailType)
                                    )
                                    .click(function (e) {
                                        EditContactPersonEmailInfo($(this).data().arrindex);
                                    })
                            )
                            .append($('<a>')
                                    .data('data', arrContactPersonEmailInfoList[i])
                                    .click(function (e) {
                                        var EmailRankNo = $(this).data('data').EmailRankNo;

                                        if ($(this).data('data').IsNew == 1) {
                                            //remove selected email
                                            arrContactPersonEmailInfoList = $.grep(arrContactPersonEmailInfoList, function (val) {
                                                return val.EmailRankNo != EmailRankNo;
                                            })
                                        }
                                        else {
                                            arrContactPersonEmailInfoList[$(this).data('data').arrindex].EmailState = "Remove";
                                        }

                                        AddContactPersonEmailInfo();
                                    })
                            )

                    pLV.append(pItem);
                }
            }

            pLV.listview('refresh');
            $('#popContactPersonEmailInfo').hide();
        }

        function EditContactPersonEmailInfo(arrindex) {
            $('#popContactPersonEmailInfo').show();
            ContactPersonEmailRankNo = arrContactPersonEmailInfoList[arrindex].EmailRankNo;
            ContactPersonEmailState = "Edit";
            sessionStorage["Emailindex"] = arrindex;
            sessionStorage["EmailIsNew"] = arrContactPersonEmailInfoList[arrindex].IsNew;
            sessionStorage["EmailState"] = arrContactPersonEmailInfoList[arrindex].EmailState;
            $('#txContacttPersonEmailID').val(arrContactPersonEmailInfoList[arrindex].EmailAddID == undefined ? "" : arrContactPersonEmailInfoList[arrindex].EmailAddID);
            $('#txtContactPersonEmailAdd').val(arrContactPersonEmailInfoList[arrindex].EmailAdd);
            $('#selContactPersonEmailType').val(arrContactPersonEmailInfoList[arrindex].EmailType);
        }

        function ClearContactPersonEmailInfoFields() {
            $('#txtContactPersonEmailID').val('');
            $('#txtContactPersonEmailAdd').val('');
            $('#selContactPersonEmailType').val('');
            $('#divContactPersonEmailHome').pageshow();
        }

        function AddContactPersonMobileInfo() {

            var pLV = $('#lstContactPersonMobile');
            var pItem;

            pLV.html('');
            for (var i = 0; i < arrContactPersonMobileInfoList.length; i++) {
                if (arrContactPersonMobileInfoList[i].MobileState != "Remove") {
                    arrContactPersonMobileInfoList[i].arrindex = i;
                    pItem = $('<li data-icon="delete">')
                                .data(arrContactPersonMobileInfoList[i])
                                .append($('<a>')
                                        .attr('ID', arrContactPersonMobileInfoList[i].id)
                                        .html("+" + $.trim(arrContactPersonMobileInfoList[i].CountryCode.split('-')[1]) + "" + $.trim(arrContactPersonMobileInfoList[i].MobileNo) +
                                            "<br>" + $.trim(arrContactPersonMobileInfoList[i].ContactType)
                                        )
                                        .data(arrContactPersonMobileInfoList[i])
                                        .click(function (e) {
                                            EditContactPersonMobileInfo($(this).data().arrindex);
                                        })
                                )
                                .append($('<a>')
                                        .data('data', arrContactPersonMobileInfoList[i])
                                        .click(function (e) {
                                            var MobileRankNo = $(this).data('data').MobileRankNo;

                                            if ($(this).data('data').IsNew == 1) {
                                                //remove selected mobile
                                                arrContactPersonMobileInfoList = $.grep(arrContactPersonMobileInfoList, function (val) {
                                                    return val.MobileRankNo != MobileRankNo;
                                                })
                                            }
                                            else {
                                                arrContactPersonMobileInfoList[$(this).data('data').arrindex].MobileState = "Remove";
                                            }

                                            AddContactPersonMobileInfo();
                                        })
                                )

                    pLV.append(pItem);
                }
            }

            pLV.listview('refresh');
            $('#popContactPersonMobileInfo').hide();
        }

        function EditContactPersonMobileInfo(arrindex) {
            $('#popContactPersonMobileInfo').show();
            ContactPersonMobileRankNo = arrContactPersonMobileInfoList[arrindex].MobileRankNo;
            ContactPersonMobileState = "Edit";
            sessionStorage["Mobileindex"] = arrindex;
            sessionStorage["MobileIsNew"] = arrContactPersonMobileInfoList[arrindex].IsNew;
            sessionStorage["MobileState"] = arrContactPersonMobileInfoList[arrindex].MobileState;
            $('#txtContactPersonMobileID').val(arrContactPersonMobileInfoList[arrindex].MobileID == undefined ? "" : arrContactPersonMobileInfoList[arrindex].MobileID);
            $('#txtContactPersonTelCoPrefixID').val(arrContactPersonMobileInfoList[arrindex].TelCodeID);
            $('#txtContactPersonMobileCountryID').val(arrContactPersonMobileInfoList[arrindex].CountryCodeID);
            $('#txtContactPersonMobileCountryCode').text(arrContactPersonMobileInfoList[arrindex].CountryCode);
            //$('#txtContactPersonTelCode').text(arrContactPersonMobileInfoList[arrindex].TelCode);
            $('#txtContactPersonMobileNo').val(arrContactPersonMobileInfoList[arrindex].MobileNo);
            $('#selContactPersonMobileType').val(arrContactPersonMobileInfoList[arrindex].ContactType);
        }

        function ClearContactPersonMobileInfo() {
            $('#txtContactPersonMobileID').val('');
            $('#txtContactPersonTelCoPrefixID').val('');
            $('#txtContactPersonMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
            $('#txtContactPersonMobileCountryCode').text('PHL - 63');
            //$('#txtContactPersonTelCode').text('Select');
            $('#txtContactPersonMobileNo').val('');
            $('#selContactPersonMobileType').val('');
            $('#divContactPersonMobileHome').pageshow();
        }

        function AddContactPersonTelInfo() {

            var pLV = $('#lstContactPersonTelephone');
            var pItem;

            pLV.html('');
            for (var i = 0; i < arrContactPersonTelInfoList.length; i++) {
                if (arrContactPersonTelInfoList[i].TelState != "Remove") {
                    arrContactPersonTelInfoList[i].arrindex = i;
                    pItem = $('<li>')
                            .data(arrContactPersonTelInfoList[i])
                            .attr('data-icon', 'delete')
                            .append($('<a>')
                                    .attr('ID', arrContactPersonTelInfoList[i].id)
                                    .html("+" + $.trim(arrContactPersonTelInfoList[i].CountryCode.split('-')[1]) + " (" + $.trim(arrContactPersonTelInfoList[i].AreaCode.split('-')[1]) + ") " + $.trim(arrContactPersonTelInfoList[i].TelephoneNo) + ($.trim(arrContactPersonTelInfoList[i].ExtensionNo) == "" ? "" : "." + $.trim(arrContactPersonTelInfoList[i].ExtensionNo)) +
                                        "<br>" + $.trim(arrContactPersonTelInfoList[i].AreaCode.split('-')[0]) + ", " + $.trim(arrContactPersonTelInfoList[i].CountryCode.split('-')[0]) +
                                        "<br>" + $.trim(arrContactPersonTelInfoList[i].ContactType)
                                    )
                                    .data('data', arrContactPersonTelInfoList[i])
                                    .click(function (e) {
                                        EditContactPersonTelInfo($(this).data('data').arrindex);
                                    })
                            )
                            .append($('<a>')
                                    .data('data', arrContactPersonTelInfoList[i])
                                    .click(function (e) {
                                        var TelRankNo = $(this).data('data').TelRankNo;

                                        if ($(this).data('data').IsNew == 1) {
                                            //remove selected telephone
                                            arrContactPersonTelInfoList = $.grep(arrContactPersonTelInfoList, function (val) {
                                                return val.TelRankNo != TelRankNo;
                                            })
                                        }
                                        else {
                                            arrContactPersonTelInfoList[$(this).data('data').arrindex].TelState = "Remove";
                                        }

                                        AddContactPersonTelInfo();
                                    })
                            )

                    pLV.append(pItem);
                }

            }

            pLV.listview('refresh');
            $('#popContactPersonTelInfo').hide();
        }

        function EditContactPersonTelInfo(arrindex) {
            $('#popContactPersonTelInfo').show();

            ContactPersonTelRankNo = arrContactPersonTelInfoList[arrindex].TelRankNo;
            ContactPersonTelState = "Edit";
            sessionStorage["Telindex"] = arrindex;
            sessionStorage["TelIsNew"] = arrContactPersonTelInfoList[arrindex].IsNew;
            sessionStorage["TelState"] = arrContactPersonTelInfoList[arrindex].TelState;
            $('#txtContactPersonTelID').val(arrContactPersonTelInfoList[arrindex].TelID == undefined ? "" : arrContactPersonTelInfoList[arrindex].TelID);
            $('#txtContactPersonTelAreaCodeID').val(arrContactPersonTelInfoList[arrindex].AreaCodeID);
            $('#txtContactPersonTelCountryID').val(arrContactPersonTelInfoList[arrindex].CountryCodeID);
            $('#txtContactPersonCountryCode').text(arrContactPersonTelInfoList[arrindex].CountryCode);
            $('#txtContactPersonAreaCode').text(arrContactPersonTelInfoList[arrindex].AreaCode);
            $('#txtContactPersonTelephoneNum').val(arrContactPersonTelInfoList[arrindex].TelephoneNo);
            $('#txtContactPersonTelephoneExt').val(arrContactPersonTelInfoList[arrindex].ExtensionNo);
            $('#selContactPersonTelType').val(arrContactPersonTelInfoList[arrindex].ContactType);
        }

        function ClearContactPersonTelInfo() {
            $('#txtContactPersonTelID').val('');
            $('#txtContactPersonTelAreaCodeID').val('');
            $('#txtContactPersonTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
            $('#txtContactPersonCountryCode').text('PHL - 63');
            $('#txtContactPersonAreaCode').text('Select');
            $('#txtContactPersonTelephoneNum').val('');
            $('#txtContactPersonTelephoneExt').val('');
            $('#selContactPersonTelType').val('');
            $('#divContactPersonTel').pageshow();
        }

        function AddContactPersonInfo() {
            var pLV = $('#lstOrgContactPerson');
            var pItem;

            pLV.html('');
            for (var i = 0; i < arrContactPersonList.length; i++) {
                if (arrContactPersonList[i].ContactPersonState != "Remove") {
                    arrContactPersonList[i].arrindex = i;
                    
                    $('#txtPersonFirstName').val(CStr2(arrContactPersonList[i].Firstname));
                    $('#txtPersonMiddleName').val(CStr2(arrContactPersonList[i].Middlename));
                    $('#txtPersonLastName').val(CStr2(arrContactPersonList[i].Lastname));
                    $('#txtAdminUsername').val(CStr2(arrContactPersonList[i].Username));

                    if (CStr2(arrContactPersonList[i].DateOfBirth) != '') {
                        $('#selPersonBirthMonth').val($.format.date(new Date(arrContactPersonList[i].DateOfBirth), 'MMMM'));
                        $('#selPersonBirthDay').val($.format.date(new Date(arrContactPersonList[i].DateOfBirth), 'd'));
                        $('#txtPersonBirthYear').val($.format.date(new Date(arrContactPersonList[i].DateOfBirth), 'yyyy'));
                    }
                }
            }

            //pLV.listview('refresh');
            //$('#popContactPersonProfile').hide();

            //ClearContactPersonFields();
        }

        function EditContactPersonInfo(arrindex) {
            ClearContactPersonFields();

            $('#popContactPersonProfile').show();
            ContactPersonRankNo = arrContactPersonList[arrindex].ContactPersonRankNo;
            ContactPersonState = "Edit";
            sessionStorage["Contactindex"] = arrindex;
            sessionStorage["ContactPersonIsNew"] = arrContactPersonList[arrindex].IsNew;
            sessionStorage["ContactPersonState"] = arrContactPersonList[arrindex].ContactPersonState;
            $('#txtContactPersonID').val(arrContactPersonList[arrindex].ID == undefined ? "" : arrContactPersonList[arrindex].ID);
            $('#txtPeopleID').val(arrContactPersonList[arrindex].PeopleID == undefined ? "" : arrContactPersonList[arrindex].PeopleID);
            $('#txtContactFirstName').val(arrContactPersonList[arrindex].Firstname);
            $('#txtContactMiddleName').val(arrContactPersonList[arrindex].Middlename);
            $('#txtContactLastName').val(arrContactPersonList[arrindex].Lastname);
            $('#txtContactPersonPosition').val(arrContactPersonList[arrindex].Position);
            $('#txtContactPersonTIN').val(arrContactPersonList[arrindex].TIN);
            $('#txtContactPersonPositionDescription').val(arrContactPersonList[arrindex].PositionDescription);
            $('#txtContactPersonAddressID').val(arrContactPersonList[arrindex].AddressID);
            $('#txtContactPersonHouseBuildingNo').val(arrContactPersonList[arrindex].HouseBuildingNo);
            $('#txtContactPersonStreet').val(arrContactPersonList[arrindex].Street);
            $('#selContactPersonCity').val(arrContactPersonList[arrindex].City);
            $('#selContactPersonCountry').val((arrContactPersonList[arrindex].Country || 'e783ed13-aa98-4cc1-918e-d49b05c0167e'));
            $('#selContactPersonTypeAddress').val(arrContactPersonList[arrindex].TypeAddress);

            if (arrContactPersonList[arrindex].IsNew == "0") {
                if (arrContactPersonList[arrindex].FromServer == undefined) {
                    AjaxSend({
                        url: 'OrganizationShort/GetContactDetailsPeople',
                        data: { ID: arrContactPersonList[arrindex].PeopleID, IsCustomer: 1 },
                        success: function (vData) {
                            arrContactPersonList[arrindex].FromServer = true;
                            arrContactPersonTelInfoList = (vData.Telephone || []);
                            arrContactPersonMobileInfoList = (vData.Mobile || []);
                            arrContactPersonEmailInfoList = (vData.Email || []);
                            arrContactPersonList[arrindex].Tel = arrContactPersonTelInfoList;
                            arrContactPersonList[arrindex].Mobile = arrContactPersonMobileInfoList
                            arrContactPersonList[arrindex].Email = arrContactPersonEmailInfoList;
                            AddContactPersonTelInfo();
                            AddContactPersonEmailInfo();
                            AddContactPersonMobileInfo();
                            intContactPersonTelInfoCtr = arrContactPersonTelInfoList.length;
                            intContactPersonMobileInfoCtr = arrContactPersonMobileInfoList.length;
                            intContactPersonEmailInfoCtr = arrContactPersonEmailInfoList.length;
                        }
                    })
                }
                else {
                    arrContactPersonTelInfoList = (arrContactPersonList[arrindex].Tel || []);
                    arrContactPersonMobileInfoList = (arrContactPersonList[arrindex].Mobile || []);
                    arrContactPersonEmailInfoList = (arrContactPersonList[arrindex].Email || []);
                    AddContactPersonTelInfo();
                    AddContactPersonEmailInfo();
                    AddContactPersonMobileInfo();
                }
            }
            else {
                arrContactPersonTelInfoList = (arrContactPersonList[arrindex].Tel || []);
                arrContactPersonMobileInfoList = (arrContactPersonList[arrindex].Mobile || []);
                arrContactPersonEmailInfoList = (arrContactPersonList[arrindex].Email || []);
                AddContactPersonTelInfo();
                AddContactPersonEmailInfo();
                AddContactPersonMobileInfo();
            }
        }

        function ClearContactPersonFields() {
            $('#txtPeopleID').val('');
            $('#txtContactPersonID').val('');
            $('#txtContactFirstName').val('');
            $('#txtContactMiddleName').val('');
            $('#txtContactLastName').val('');
            $('#txtContactPersonPosition').val('');
            $('#txtContactPersonTIN').val('');
            $('#txtContactPersonPositionDescription').val('');
            $('#txtContactPersonAddressID').val('');
            $('#txtContactPersonHouseBuildingNo').val('');
            $('#txtContactPersonStreet').val('');
            $('#selContactPersonCity').val('');
            $('#selContactPersonCountry').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
            $('#selContactPersonTypeAddress').val();
            $('#lstContactPersonTelephone').empty();
            $('#lstContactPersonMobile').empty();
            $('#lstContactPersonEmail').empty();

            arrContactPersonTelInfoList = [];
            intContactPersonTelInfoCtr = 0;
            ContactPersonTelState = "";
            ContactPersonTelRankNo = "";
            arrContactPersonMobileInfoList = [];
            intContactPersonMobileInfoCtr = 0;
            ContactPersonMobileState = "";
            ContactPersonMobileRankNo = "";
            arrContactPersonEmailInfoList = [];
            intContactPersonEmailInfoCtr = 0;
            ContactPersonEmailState = "";
            ContactPersonEmailRankNo = "";
        }

        function AddWebsiteInfo() {

            var pLV = $('#lstOrgWebsite');
            var pItem;

            pLV.html('');
            for (var i = 0; i < arrWebsiteList.length; i++) {
                if (arrWebsiteList[i].WebsiteState != "Remove") {
                    arrWebsiteList[i].arrindex = i;
                    pItem = $('<li data-icon="delete">')
                            .data(arrWebsiteList[i])
                            .append($('<a>')
                                    .data(arrWebsiteList[i])
                                    .html($.trim(arrWebsiteList[i].Website))
                                    .click(function (e) {
                                        EditWebsiteInfo($(this).data().arrindex);
                                    })
                            )
                            .append($('<a>')
                                    .data('data', arrWebsiteList[i])
                                    .click(function (e) {
                                        var WesbiteRankNo = $(this).data('data').WesbiteRankNo;

                                        if ($(this).data('data').IsNew == 1) {
                                            //remove selected website
                                            arrWebsiteList = $.grep(arrWebsiteList, function (val) {
                                                return val.WesbiteRankNo != WesbiteRankNo;
                                            })
                                        }
                                        else {
                                            arrWebsiteList[$(this).data('data').arrindex].WebsiteState = "Remove";
                                        }

                                        AddWebsiteInfo();
                                    })
                            )

                    pLV.append(pItem);
                }
            }

            pLV.listview('refresh');
            $('#popOrgWebsiteInfo').hide();
        }

        function EditWebsiteInfo(arrindex) {
            $('#popOrgWebsiteInfo').show();
            WebsiteRankNo = arrWebsiteList[arrindex].WebsiteRankNo;
            WebsiteState = "Edit";
            sessionStorage["Websiteindex"] = arrindex;
            sessionStorage["WebsiteIsNew"] = arrWebsiteList[arrindex].IsNew;
            sessionStorage["WebsiteState"] = arrWebsiteList[arrindex].EmailState;
            $('#txtOrgWebsiteID').val(arrWebsiteList[arrindex].WebsiteID == undefined ? "" : arrWebsiteList[arrindex].WebsiteID);
            $('#txtWebsiteUrl').val(arrWebsiteList[arrindex].Website);
        }

        function ClearWebsiteFields() {
            $('#txtOrgWebsite').val('');
            $('#txtWebsiteUrl').val('');
        }

        function AddEmailInfo() {

            var pLV = $('#lstOrgEmail');
            var pItem;

            pLV.html('');
            for (var i = 0; i < arrEmailInfoList.length; i++) {
                if (arrEmailInfoList[i].EmailState != "Remove") {
                    arrEmailInfoList[i].arrindex = i;
                    pItem = $('<li data-icon="delete">')
                            .data(arrEmailInfoList[i])
                            .append($('<a>')
                                    .data(arrEmailInfoList[i])
                                    .html($.trim(arrEmailInfoList[i].EmailAdd) +
                                        "<br>" + $.trim(arrEmailInfoList[i].EmailType)
                                    )
                                    .click(function (e) {
                                        EditEmailInfo($(this).data().arrindex);
                                    })
                            )
                            .append($('<a>')
                                    .data('data', arrEmailInfoList[i])
                                    .click(function (e) {
                                        var EmailRankNo = $(this).data('data').EmailRankNo;

                                        if ($(this).data('data').IsNew == 1) {
                                            //remove selected email
                                            arrEmailInfoList = $.grep(arrEmailInfoList, function (val) {
                                                return val.EmailRankNo != EmailRankNo;
                                            })
                                        }
                                        else {
                                            arrEmailInfoList[$(this).data('data').arrindex].EmailState = "Remove";
                                        }

                                        AddEmailInfo();
                                    })
                            )

                    pLV.append(pItem);
                }
            }

            pLV.listview('refresh');
            $('#popOrgEmailInfo').hide();
        }

        function EditEmailInfo(arrindex) {
            $('#popOrgEmailInfo').show();
            EmailRankNo = arrEmailInfoList[arrindex].EmailRankNo;
            EmailState = "Edit";
            sessionStorage["Emailindex"] = arrindex;
            sessionStorage["EmailIsNew"] = arrEmailInfoList[arrindex].IsNew;
            sessionStorage["EmailState"] = arrEmailInfoList[arrindex].EmailState;
            $('#txtOrgEmailID').val(arrEmailInfoList[arrindex].EmailAddID == undefined ? "" : arrEmailInfoList[arrindex].EmailAddID);
            $('#txtEmailAdd').val(arrEmailInfoList[arrindex].EmailAdd);
            $('#selOrgEmailType').val(arrEmailInfoList[arrindex].EmailType);
        }

        function ClearEmailInfoFields() {
            $('#txtOrgEmailID').val('');
            $('#txtEmailAdd').val('');
            $('#selOrgEmailType').val('');
        }

        function AddMobileInfo() {

            var pLV = $('#lstOrgMobile');
            var pItem;
            pLV.html('');
            for (var i = 0; i < arrMobileInfoList.length; i++) {
                if (arrMobileInfoList[i].MobileState != "Remove") {
                    arrMobileInfoList[i].arrindex = i;
                    pItem = $('<li data-icon="delete">')
                                .data(arrMobileInfoList[i])
                                .append($('<a>')
                                        .data(arrMobileInfoList[i])
                                        .attr('ID', arrMobileInfoList[i].id)
                                        .html("+" + $.trim(arrMobileInfoList[i].CountryCode.split('-')[1]) + "" + $.trim(arrMobileInfoList[i].MobileNo) +
                                            "<br>" + $.trim(arrMobileInfoList[i].ContactType)
                                        )
                                        .click(function (e) {
                                            EditMobileInfo($(this).data().arrindex);
                                        })
                                )
                                .append($('<a>')
                                        .data('data', arrMobileInfoList[i])
                                        .click(function (e) {
                                            var MobileRankNo = $(this).data('data').MobileRankNo;

                                            if ($(this).data('data').IsNew == 1) {
                                                //remove selected mobile
                                                arrMobileInfoList = $.grep(arrMobileInfoList, function (val) {
                                                    return val.MobileRankNo != MobileRankNo;
                                                })
                                            }
                                            else {
                                                arrMobileInfoList[$(this).data('data').arrindex].MobileState = "Remove";
                                            }

                                            AddMobileInfo();
                                        })
                                )

                    pLV.append(pItem);
                }
            }

            pLV.listview('refresh');
            $('#popOrgMobileInfo').hide();
        }

        function EditMobileInfo(arrindex) {
            $('#popOrgMobileInfo').show();
            MobileRankNo = arrMobileInfoList[arrindex].MobileRankNo;
            MobileState = "Edit";
            sessionStorage["Mobileindex"] = arrindex;
            sessionStorage["MobileIsNew"] = arrMobileInfoList[arrindex].IsNew;
            sessionStorage["MobileState"] = arrMobileInfoList[arrindex].MobileState;
            $('#txtOrgMobileID').val(arrMobileInfoList[arrindex].MobileID == undefined ? "" : arrMobileInfoList[arrindex].MobileID);
            $('#txtOrgTelCoPrefixID').val(arrMobileInfoList[arrindex].TelCodeID);
            $('#txtOrgMobileCountryID').val(arrMobileInfoList[arrindex].CountryCodeID);
            $('#txtMobileCountryCode').text(arrMobileInfoList[arrindex].CountryCode);
            //$('#txtTelCode').text(arrMobileInfoList[arrindex].TelCode);
            $('#txtMobileNo').val(arrMobileInfoList[arrindex].MobileNo);
            $('#selOrgMobileType').val(arrMobileInfoList[arrindex].ContactType);
        }

        function ClearMobileInfo() {
            $('#txtOrgMobileID').val('');
            $('#txtOrgTelCoPrefixID').val('');
            $('#txtOrgMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
            $('#txtMobileCountryCode').text('PHL - 63');
            //$('#txtTelCode').text('Select');
            $('#txtMobileNo').val('');
            $('#selOrgMobileType').val('');
        }

        function AddTelInfo() {

            var pLV = $('#lstOrgTelephone');
            var pItem;

            pLV.html('');
            for (var i = 0; i < arrTelInfoList.length; i++) {
                if (CStr2(arrTelInfoList[i].CountryCodeID) != "") {
                    if (arrTelInfoList[i].TelState != "Remove") {
                        arrTelInfoList[i].arrindex = i;
                        pItem = $('<li>')
                                .data(arrTelInfoList[i])
                                .attr('data-icon', 'delete')
                                .append($('<a>')
                                        //
                                        //.html((arrTelInfoList[i].CountryCode.split('-')[1] == undefined ? "" : "+" + $.trim(arrTelInfoList[i].CountryCode.split('-')[1])) + " (" + (arrTelInfoList[i].AreaCode.split('-')[1] == undefined ? $.trim(arrTelInfoList[i].AreaCode.split('-')[0]) : $.trim(arrTelInfoList[i].AreaCode.split('-')[1])) + ") " + $.trim(arrTelInfoList[i].TelephoneNo) + ($.trim(arrTelInfoList[i].ExtensionNo) == "" ? "" : "." + $.trim(arrTelInfoList[i].ExtensionNo)) +
                                        //    "<br>" + (arrTelInfoList[i].AreaCode.split('-')[1] == undefined ? "" : $.trim(arrTelInfoList[i].AreaCode.split('-')[1]) + ', ') + $.trim(arrTelInfoList[i].CountryCode.split('-')[0]) +
                                        //    "<br>" + $.trim(arrTelInfoList[i].ContactType)
                                        //)
                                        .html((CStr2(arrTelInfoList[i].CountryCode).split('-')[1] == undefined ? "" : "+" + $.trim(CStr2(arrTelInfoList[i].CountryCode).split('-')[1])) + " (" + (CStr2(arrTelInfoList[i].AreaCode).split('-')[1] == undefined ? $.trim(CStr2(arrTelInfoList[i].AreaCode).split('-')[0]) : $.trim(CStr2(arrTelInfoList[i].AreaCode).split('-')[1])) + ") " + $.trim(arrTelInfoList[i].TelephoneNo) + ($.trim(arrTelInfoList[i].ExtensionNo) == "" ? "" : "." + $.trim(arrTelInfoList[i].ExtensionNo)) +
                                            "<br>" + $.trim(CStr2(arrTelInfoList[i].AreaCode).split('-')[0]) + ', ' + $.trim(CStr2(arrTelInfoList[i].CountryCode).split('-')[0]) +
                                            "<br>" + $.trim(arrTelInfoList[i].ContactType)
                                        )
                                        .data('data', arrTelInfoList[i])
                                        .click(function (e) {
                                            EditTelInfo($(this).data('data').arrindex);
                                        })
                                )
                                .append($('<a>')
                                        .data('data', arrTelInfoList[i])
                                        .click(function (e) {
                                            var TelRankNo = $(this).data('data').TelRankNo;

                                            if ($(this).data('data').IsNew == 1) {
                                                //remove selected telephone
                                                arrTelInfoList = $.grep(arrTelInfoList, function (val) {
                                                    return val.TelRankNo != TelRankNo;
                                                })
                                            }
                                            else {
                                                arrTelInfoList[$(this).data('data').arrindex].TelState = "Remove";
                                            }

                                            AddTelInfo();
                                        })
                                )

                        pLV.append(pItem);
                    }
                } else {
                    arrTelInfoList[i].TelState = 'Remove';
                }
            }

            pLV.listview('refresh');
            $('#popOrgTelInfo').hide();
        }

        function EditTelInfo(arrindex) {
            $('#popOrgTelInfo').show();

            TelRankNo = arrTelInfoList[arrindex].TelRankNo;
            TelState = "Edit";
            sessionStorage["Telindex"] = arrindex;
            sessionStorage["TelIsNew"] = arrTelInfoList[arrindex].IsNew;
            sessionStorage["TelState"] = arrTelInfoList[arrindex].TelState;
            $('#txtOrgTelID').val(arrTelInfoList[arrindex].TelID == undefined ? "" : arrTelInfoList[arrindex].TelID);
            $('#txtOrgTelAreaCodeID').val(arrTelInfoList[arrindex].AreaCodeID);
            $('#txtOrgTelCountryID').val(arrTelInfoList[arrindex].CountryCodeID);
            $('#txtCountryCode').text(arrTelInfoList[arrindex].CountryCode);
            $('#txtAreaCode').text(arrTelInfoList[arrindex].AreaCode);
            $('#txtTelephoneNum').val(arrTelInfoList[arrindex].TelephoneNo);
            $('#txtTelephoneExt').val(arrTelInfoList[arrindex].ExtensionNo);
            $('#selOrgTelType').val(arrTelInfoList[arrindex].ContactType);
        }

        function ClearTelInfo() {
            $('#txtOrgTelID').val('');
            $('#txtOrgTelAreaCodeID').val('');
            $('#txtOrgTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
            $('#txtCountryCode').text('PHL - 63');
            $('#txtAreaCode').text('Select');
            $('#txtTelephoneNum').val('');
            $('#txtTelephoneExt').val('');
            $('#selOrgTelType').val('');
        }

        function LoadRequiredData() {
            //$('#divCalendar').customcalendar(); DISABLE
            setRegionCollection('#selProspectOrgRegion');
            setRegionCollection('#selPersonRegion');

            DisplayCities();
        }

        function CacheBusinessIndustry() {
            AjaxSend({
                url: 'CRMDashboard/CacheBusinessIndustry',
                success: function (vData) {
                    var selFilterByBusinessIndustry = $('#selFilterByBusinessIndustry');
                    selFilterByBusinessIndustry.empty();

                    selFilterByBusinessIndustry.append(new Option('NA', ''))

                    for (var i = 0; i < vData.Results.Rows.length; i++) {
                        selFilterByBusinessIndustry.append(new Option(vData.Results.Rows[i].Class, vData.Results.Rows[i].ID));
                    }

                    selFilterByBusinessIndustry;
                }
            })
        }

        function DisplayCities() {
            var selProspectCities = $('#selProspectCities');
            var selPersonCity = $('#selPersonCity');
            var selContactPersonCity = $('#selContactPersonCity');
            var selFilterByCity = $('#selFilterByCity');
            var selOutletCity = $('#selOutletCity');

            selProspectCities.empty();
            selProspectCities.append(new Option('Please Select City', ''));

            selPersonCity.empty();
            selPersonCity.append(new Option('Please Select City', ''));

            selContactPersonCity.empty();
            selContactPersonCity.append(new Option('Please Select City', ''));

            selOutletCity.empty();
            selOutletCity.append(new Option('Please Select City', ''));

            selFilterByCity.empty();
            selFilterByCity.append(new Option('NA', ''));

            AjaxSend({
                url: 'OrganizationShort/getCityNameList',
                success: function (vData) {


                    for (var p = 0; p < vData.CityData.Rows.length; p++) {
                        if (vData.CityData.Rows[p].Name != undefined) {
                            selProspectCities.append(new Option(vData.CityData.Rows[p].Name, vData.CityData.Rows[p].ID))
                            selPersonCity.append(new Option(vData.CityData.Rows[p].Name, vData.CityData.Rows[p].ID))
                            selContactPersonCity.append(new Option(vData.CityData.Rows[p].Name, vData.CityData.Rows[p].ID))
                            selOutletCity.append(new Option(vData.CityData.Rows[p].Name, vData.CityData.Rows[p].ID))
                            selFilterByCity.append(new Option(vData.CityData.Rows[p].Name, vData.CityData.Rows[p].ID))
                        }
                    }

                    DisplayCountries()
                }
            });
        }

        function DisplayCountries() {
            var selProspectCountries = $('#selProspectCountries');
            var selPersonCountry = $('#selPersonCountry');
            var selContactPersonCountry = $('#selContactPersonCountry');
            var selOutletCountry = $('#selOutletCountry');

            selProspectCountries.empty();
            selProspectCountries.append(new Option('Please Select Country', ''));

            selPersonCountry.empty();
            selPersonCountry.append(new Option('Please Select Country', ''));

            selContactPersonCountry.empty();
            selContactPersonCountry.append(new Option('Please Select Country', ''));

            selOutletCountry.empty();
            selOutletCountry.append(new Option('Please Select Country', ''));

            AjaxSend({
                url: 'OrganizationShort/getCountryNameList',
                success: function (vData) {

                    for (var p = 0; p < vData.CountryData.Rows.length; p++) {
                        selProspectCountries.append(new Option(vData.CountryData.Rows[p].Name, vData.CountryData.Rows[p].ID))
                        selPersonCountry.append(new Option(vData.CountryData.Rows[p].Name, vData.CountryData.Rows[p].ID))
                        selContactPersonCountry.append(new Option(vData.CountryData.Rows[p].Name, vData.CountryData.Rows[p].ID))
                        selOutletCountry.append(new Option(vData.CountryData.Rows[p].Name, vData.CountryData.Rows[p].ID))
                    }

                    selProspectCountries.val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                    selPersonCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                    selContactPersonCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                    selOutletCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                }
            });
        }

        function GetProspectDetails(id) {
            ClearOrgProspectVariablesFields();

            if (CStr2(id) == '') {
                return;
            }

            AjaxSend({
                url: 'Ecommerce/GetSupplierInfo',
                data: { ID: id, Type: GetUrlParameters().type, OrgID: sessionStorage.OrgID, IsFromGeneratedPublicURL: true, PeopleID: CStr2(GetUrlParameters().pid) },
                success: function (vData) {
                    var vRowsOrgDetails = vData.OrgDetails;
                    arrContactPersonList = (vData.ContactPerson || []);
                    arrEmailInfoList = vData.Email;
                    arrMobileInfoList = vData.Mobile;
                    arrTelInfoList = vData.Telephone;
                    arrWebsiteList = vData.Website;
                    arrOrgAffList = vData.Affiliation;
                    arrOrgAffPersonList = vData.AffiliatedPerson;
                    arrCompanyOutlet = vData.Outlet;

                    intContactPersonCtr = 0; //vData.ContactPerson.length;
                    intTelInfoCtr = vData.Telephone.length;
                    intMobileInfoCtr = vData.Mobile.length;
                    intEmailInfoCtr = vData.Email.length;
                    intWebsiteCtr = vData.Website.length;

                    if (arrContactPersonList.length > 0) {
                        if (vData.ContactPersonEmail.length > 0) {
                            arrContactPersonList[0].Email = vData.ContactPersonEmail;
                            arrContactPersonEmailInfoList = vData.ContactPersonEmail;
                            $('#txtPersonEmailAdd').val(arrContactPersonList[0].Email[0].EmailAdd);
                        }                        
                    }

                    if ((vData.SecQue || []).length > 0) {
                        arrSecQue = vData.SecQue;
                        $('#selSecQuestion1').val(arrSecQue[0].SecurityQuestion);
                        $('#selSecQuestion2').val(arrSecQue[1].SecurityQuestion);

                        $('#selSecQuestion2 option[value="' + arrSecQue[0].SecurityQuestion + '"]').attr('disabled', true);
                        $('#selSecQuestion1 option[value="' + arrSecQue[1].SecurityQuestion + '"]').attr('disabled', true);
                    }

                    if (isViewReadOnlyProfile) {
                        ReadOnlyOrgProfile(vRowsOrgDetails[0]);
                    } else {
                        $('#txtProspectOrgID').val(vRowsOrgDetails[0].ID);
                        $('#txtCustomerOrgID').val(vRowsOrgDetails[0].CustomerID);
                        $('#txtProspectCompany3rdPartyPlatform').val(vRowsOrgDetails[0].ThirdPartyPlatform);
                        $('#txtProspectCompany3rdPartyID').val(vRowsOrgDetails[0].ThirdPartyID);
                        $('#txtCodeNumber').val(vRowsOrgDetails[0].CodeNumber);
                        $('#txtProspectCompanyName').val(vRowsOrgDetails[0].CompanyName);
                        $('#txtProspectOrgTin').val(vRowsOrgDetails[0].TinNumber);
                        $('#txtProspectHouseBldgNo').val(vRowsOrgDetails[0].OfficeAddressBuildingNo);
                        $('#txtProspectStreetName').val(vRowsOrgDetails[0].OfficeAddressStreet);
                        $('#selProspectCities').val(vRowsOrgDetails[0].OfficeAddressCityID);
                        $('#selProspectOrgRegion').val(vRowsOrgDetails[0].Region);
                        $('#selProspectIsland').val(vRowsOrgDetails[0].Island);
                        $('#selProspectCountries').val(vRowsOrgDetails[0].OfficeAddressCountryID);
                        $('#txtProspectCompanyBackground').val(vRowsOrgDetails[0].Background);
                        $('#txtProspectCompanyMission').val(vRowsOrgDetails[0].Mission);
                        $('#txtProspectCompanyVision').val(vRowsOrgDetails[0].Vision);
                        $('#txtProspectCompanyFounding').val(vRowsOrgDetails[0].FoundingDate);
                        $('#txtBusIndustryID').val((vRowsOrgDetails[0].IndustryID == undefined ? '' : vRowsOrgDetails[0].IndustryID));
                        $('#txtBusinessIndustryText').val((vRowsOrgDetails[0].BusinessIndustryText == undefined ? '' : vRowsOrgDetails[0].BusinessIndustryText));
                        $('#btnProspectCompanyIndustry').text((vRowsOrgDetails[0].IndustryName == undefined ? 'Search here' : vRowsOrgDetails[0].IndustryName));
                        $('#selOrgStatus').val((CBool(vRowsOrgDetails[0].IsActive) == true ? 'Active' : 'Deactive'));

                        $('#txtProspectNoOfEmployees').val((CDec2(vRowsOrgDetails[0].NoOfEmployees) == 0 ? '' : vRowsOrgDetails[0].NoOfEmployees));
                        $('#txtProspectRevenuePerYear').val((CDec2(vRowsOrgDetails[0].RevenuePerYear) == 0 ? '' : vRowsOrgDetails[0].RevenuePerYear));
                        $('#selProspectScopeOperation').val((vRowsOrgDetails[0].ScopeOfOperations == '' ? '' : vRowsOrgDetails[0].ScopeOfOperations));
                        $('#txtProspectNoFactories').val((CDec2(vRowsOrgDetails[0].NoOfFactory) == 0 ? '' : vRowsOrgDetails[0].NoOfFactory));
                        $('#txtProspectNoOffices').val((CDec2(vRowsOrgDetails[0].NoOfOffice) == 0 ? '' : vRowsOrgDetails[0].NoOfOffice));
                        $('#txtProspectNoWarehouses').val((CDec2(vRowsOrgDetails[0].NoOfWarehouse) == 0 ? '' : vRowsOrgDetails[0].NoOfWarehouse));
                        $('#txtProspectNoStores').val((CDec2(vRowsOrgDetails[0].NoOfStore) == 0 ? '' : vRowsOrgDetails[0].NoOfStore));
                        $('#txtProspectNoTrucks').val((CDec2(vRowsOrgDetails[0].NoOfTruck) == 0 ? '' : vRowsOrgDetails[0].NoOfTruck));

                        if (sessionStorage.LibraryName == 'Customer / Client List') {
                            RevSalesRepID = vData.Revenue[0].SalesRepID;
                            arrRevPriceLevels = vData.PriceLevel;
                            RevPaymentTerms = vData.Revenue[0].TermID;
                            $('#btnRevSalesRepresentative').text((vData.Revenue[0].SalesRepName == '' ? 'Search Representative' : vData.Revenue[0].SalesRepName));
                            //$('#btnRevPriceLevel').text((vData.Revenue[0].PriceLevelName == '' ? 'Search Price Level' : vData.Revenue[0].PriceLevelName));
                            $('#btnRevCreditTerms').text((vData.Revenue[0].TermName == '' ? 'Search Payment Terms' : vData.Revenue[0].TermName));
                            $('#txtRevCreditLimit').val(vData.Revenue[0].CreditLimit);
                            $('#txtRevCreditBegBal').val(vData.Revenue[0].CreditBegBalance);
                            $('#btnRevCreditBegAsOf').text((vData.Revenue[0].CreditAsOf == '' ? 'Set Date' : $.format.date(new Date(vData.Revenue[0].CreditAsOf), 'MMM dd, yyyy')));
                            $('#txtRevThirdPartyBegBalance').val(vData.Revenue[0].ThirdPartyBegBalance);
                            $('#btnRevThirdPartyBegBalanceAsOf').text((vData.Revenue[0].ThirdPartyBegBalanceAsOf == '' ? 'Set Date' : $.format.date(new Date(vData.Revenue[0].ThirdPartyBegBalanceAsOf), 'MMM dd, yyyy')));
                        }

                        $('#selPersonBirthDay').empty().append(new Option('Day', ''));

                        for (var i = 1; i <= 31; i++) {
                            $('#selPersonBirthDay').append(new Option(i, i))
                        }

                        AddTelInfo();
                        AddMobileInfo();
                        AddEmailInfo()
                        AddWebsiteInfo();
                        AddContactPersonInfo();
                        AddOrgAffiliation();
                        AddAffiliatedPerson();
                        DisplayRevPriceLevels();
                        DisplayCompanyOutlet();
                    }
                }
            })
        }

        function ReadOnlyOrgProfile(data) {
            var listpreviewOrgContactPerson = $('#listpreviewOrgContactPerson').empty();
            var listpreviewOrgAff = $('#listpreviewOrgAff').empty();
            var listpreviewAffPerson = $('#listpreviewAffPerson').empty();
            var listpreviewOrgTel = $('#listpreviewOrgTel').empty();
            var listpreviewOrgMobile = $('#listpreviewOrgMobile').empty();
            var listpreviewOrgEmail = $('#listpreviewOrgEmail').empty();
            var listpreviewOrgWebsite = $('#listpreviewOrgWebsite').empty();

            $('#txtpreviewCompanyName').text(data.CompanyName);
            $('#txtpreviewCompanyAddress').text($.trim(data.Address));
            $('#txtpreviewCompanyBackground').text(CStr2(data.Background));
            $('#txtpreviewCompanyMission').text(CStr2(data.Mission));
            $('#txtpreviewCompanyVision').text(CStr2(data.Vision));
            $('#txtpreviewCompanyFounding').text(CStr2(data.FoundingDate));
            $('#txtpreviewCompanyIndustry').text(CStr2(data.IndustryName));
            $('#txtprevNoOfEmp').text(CStr2(data.NoOfEmployees));
            $('#txtprevRevenue').text(CStr2(data.RevenuePerYear));
            $('#txtprevScope').text(CStr2(data.ScopeOfOperations));
            $('#txtprevNoOfFactories').text(CStr2(data.NoOfFactory));
            $('#txtprevNoOfOffices').text(CStr2(data.NoOfOffice));
            $('#txtprevNoOfWarehouse').text(CStr2(data.NoOfWarehouse));
            $('#txtprevNoOfStores').text(CStr2(data.NoOfStore));
            $('#txtprevNoOfTrucks').text(CStr2(data.NoOfTruck));
            //$('#txtprevSalesRep').text(data.);
            //$('#txtprevPriceLvl').text(data.);
            //$('#txtprevCreditTerms').text(data.);
            //$('#txtprevCreditLimit').text(data.);
            //$('#txtprevCreditBeg').text(data.);
            //$('#txtprevCreditBegAsOf').text(data.);

            for (var i = 0; i < arrOrgAffList.length; i++) {
                listpreviewOrgAff.append($('<li>').text(arrOrgAffList[i].CompanyName))
            }

            for (var i = 0; i < arrOrgAffPersonList.length; i++) {
                listpreviewAffPerson.append($('<li>').text(arrOrgAffPersonList[i].PeopleName))
            }

            for (var i = 0; i < arrTelInfoList.length; i++) {
                if (CStr2(arrTelInfoList[i].CountryCode) != "") {
                    listpreviewOrgTel.append($('<li>').text('+' + $.trim((arrTelInfoList[i].CountryCode).split('-')[1]) + ' (' + $.trim((CStr2(arrTelInfoList[i].AreaCode)).split('-')[1]) + ') ' + arrTelInfoList[i].TelephoneNo + ' (' + arrTelInfoList[i].ContactType + ')'))
                }
            }

            for (var i = 0; i < arrMobileInfoList.length; i++) {
                if (CStr2(arrMobileInfoList[i].CountryCode) != "") {
                    listpreviewOrgMobile.append($('<li>').text('+' + $.trim((arrMobileInfoList[i].CountryCode).split('-')[1]) + '' + arrMobileInfoList[i].MobileNo + ' (' + arrMobileInfoList[i].ContactType + ')'))
                }
            }

            for (var i = 0; i < arrEmailInfoList.length; i++) {
                listpreviewOrgEmail.append($('<li>').text(arrEmailInfoList[i].EmailAdd))
            }

            for (var i = 0; i < arrWebsiteList.length; i++) {
                listpreviewOrgWebsite.append($('<li>').text(arrWebsiteList[i].Website))
            }

            $('#prevOrgProfile').show();
            $('#roOrg').pageshow();
        }

        function ClearOrgProspectVariablesFields() {
            proj.SupplierClearFields();
            arrNewDepartment = [];
            arrCompanyOutlet: [];
            arrTelInfoList = [];
            intTelInfoCtr = 0;
            TelState = "";
            TelRankNo = "";
            arrMobileInfoList = [];
            intMobileInfoCtr = 0;
            MobileState = "";
            MobileRankNo = "";
            arrEmailInfoList = [];
            intEmailInfoCtr = 0;
            EmailState = "";
            EmailRankNo = "";
            arrWebsiteList = [];
            intWebsiteCtr = 0;
            WebsiteState = "";
            WebsiteRankNo = "";
            arrContactPersonList = [];
            intContactPersonCtr = 0;
            ContactPersonState = "";
            ContactPersonRankNo = "";
            arrOrgAffList = [];
            RevSalesRepID = '';
            arrRevPriceLevels = [];
            RevPaymentTerms = '';

            if (sessionStorage.LibraryName == 'Customer / Client List') {
                $('#lblCodeType').text('Customer');
            } else {
                $('#lblCodeType').text('Vendor');
            }

            $('#selCompanyOutletStatus, #selCompanyOutletDeptStatus').val(1);
            $('#divCompany3rdPartyPlatform').show(); // show even for supplier
            $('#divCompany3rdPartyID').show();
            $('#txtExistingCustomerOrgID').val('');
            $('#txtProspectOrgID').val('');
            $('#selOrgStatus').val('Active');
            $('#txtProspectCompany3rdPartyPlatform').val('');
            $('#txtProspectCompany3rdPartyID, .txtCodeNumber').val('');
            $('#txtProspectCompanyName').val('');
            $('#txtProspectOrgTin').val('');
            $('#txtProspectHouseBldgNo').val('');
            $('#txtProspectStreetName').val('');
            $('#selProspectOrgRegion').val('');
            $('#selProspectCities').val('');
            $('#selProspectIsland').val('');
            $('#selProspectCountries').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
            $('#txtProspectCompanyBackground').val('');
            $('#txtProspectCompanyMission').val('');
            $('#txtProspectCompanyVision').val('');
            $('#txtProspectCompanyFounding').val('');
            $('#txtBusIndustryID').val('');
            $('#txtBusinessIndustryText').val('');
            $('#btnProspectCompanyIndustry').text('Search here');
            $('#lstOrgAffiliation').empty();
            $('#lstAffiliatedPerson').empty()

            $('#lstOrgTelephone').empty();
            $('#txtOrgTelID').val('');
            $('#txtOrgTelAreaCodeID').val('');

            $('#lstOrgMobile').empty();
            $('#txtOrgMobileID').val('');
            $('#txtOrgTelCoPrefixID').val('');

            $('#lstOrgEmail').empty();
            $('#txtOrgEmailID').val('');

            $('#lstOrgWebsite').empty();
            $('#txtOrgWebsiteID').val('');

            $('#lstOrgContactPerson').empty();

            $('#txtProspectNoOfEmployees').val('');
            $('#txtProspectRevenuePerYear').val('');
            $('#selProspectScopeOperation').val('');
            $('#txtProspectNoFactories').val('');
            $('#txtProspectNoOffices').val('');
            $('#txtProspectNoWarehouses').val('');
            $('#txtProspectNoStores').val('');
            $('#txtProspectNoTrucks').val('');

            $('#txtRevCreditLimit').val('');
            $('#txtRevCreditBegBal').val('');
            $('#btnRevSalesRepresentative').text('Search Representative');

            $('#txtRevThirdPartyBegBalance').val('');
            $('#btnRevThirdPartyBegBalanceAsOf').text('Set Date');

            //$('#btnRevPriceLevel').text('Search Price Level');
            $('#btnRevCreditTerms').text('Search Payment Terms');
            $('#btnRevCreditBegAsOf').text('Set Date');

            ClearContactPersonFields();
            ClearEmailInfoFields();
            ClearMobileInfo();
            ClearTelInfo();
            ClearWebsiteFields();
        }

        function GetProspectDetailsPeople(id) {
            ClearPersonProspectVariablesFields();

            if (CStr2(id) == '') {
                return;
            }

            AjaxSend({
                url: 'Ecommerce/GetSupplierInfo',
                data: { ID: id, Type: GetUrlParameters().type, OrgID: sessionStorage.OrgID, IsFromGeneratedPublicURL: true },
                success: function (vData) {
                    var vRowsPeopleDetails = vData.PeopleDetails;
                    arrPersonEmailInfoList = vData.Email; //require new valid email address
                    arrPersonLoginCredentials = vData.LoginCredentials;
                    arrPersonSecQue = vData.SecQue;
                    arrPersonMobileInfoList = vData.Mobile;
                    arrPersonTelInfoList = vData.Telephone;
                    arrPersonOrgAffList = vData.Affiliation;
                    arrElementary = vData.Education.Elementary;
                    arrHighSchool = vData.Education.HighSchool;
                    arrCollege = vData.Education.College;

                    if (arrPersonSecQue.length > 0) {
                        $('#selPersonSecQuestion1').val(arrPersonSecQue[0].SecurityQuestion);
                        $('#selPersonSecQuestion2').val(arrPersonSecQue[1].SecurityQuestion);

                        $('#selPersonSecQuestion2 option[value="' + arrPersonSecQue[0].SecurityQuestion + '"]').attr('disabled', true);
                        $('#selPersonSecQuestion1 option[value="' + arrPersonSecQue[1].SecurityQuestion + '"]').attr('disabled', true);
                    }

                    if (isViewReadOnlyProfile) {
                        vRowsPeopleDetails[0].Address = (vData.Address[0] || []);
                        ReadOnlyPersonProfile(vRowsPeopleDetails[0]);
                    } else {
                        $('#txtProspectPerson3rdPartyID').val(vRowsPeopleDetails[0].ThirdPartyID);
                        $('#txtCustomerPersonID').val(vRowsPeopleDetails[0].CustomerID);
                        $('#txtProspectPersonID').val(vRowsPeopleDetails[0].ID);
                        $('#txtPersonCodeNumber').val(vRowsPeopleDetails[0].CodeNumber);
                        $('#txtProspectPersonFirstName').val(vRowsPeopleDetails[0].FirstName);
                        $('#txtProspectPersonMiddleName').val(vRowsPeopleDetails[0].MiddleName);
                        $('#txtProspectPersonLastName').val(vRowsPeopleDetails[0].Surname);
                        $('#txtProspectPersonTinNumber').val(vRowsPeopleDetails[0].TIN);
                        $('#txtProspectPersonPosition').val(vRowsPeopleDetails[0].Position);
                        $('#txtProspectPersonPositionDescription').val(vRowsPeopleDetails[0].PositionDescription);
                        $('#selPersonStatus').val((CBool(vRowsPeopleDetails[0].IsActive) == true ? 'Active' : 'Deactive'));

                        if (CStr2(vRowsPeopleDetails[0].DateOfBirth) != '') {
                            $('#btnPersonBirthDate').text($.format.date(new Date(parseInt((vRowsPeopleDetails[0].DateOfBirth).replace('/Date(', ''))), 'MMM dd, yyyy'));
                        }

                        if (vData.Address.length > 0) {
                            $('#txtPersonAddressID').val(vData.Address[0].ID);
                            $('#txtPersonHouseBuildingNo').val(vData.Address[0].HouseBuildingNo);
                            $('#txtPersonStreet').val(vData.Address[0].Street);
                            $('#selPersonCity').val(vData.Address[0].CityID);
                            $('#selPersonRegion').val(vData.Address[0].Region);
                            $('#selPersonIsland').val(vData.Address[0].Island);
                            $('#selPersonCountry').val(vData.Address[0].CountryID);
                            $('#selPersonTypeAddress').val(vData.Address[0].AddressType);
                        }

                        if (arrPersonEmailInfoList.length > 0) {
                            $('#txtProspectEmailAddress').val(arrPersonEmailInfoList[0].EmailAdd);
                        }

                        AddPersonTelInfo();
                        AddPersonMobileInfo();
                        //AddPersonEmailInfo();
                        AddPersonOrgAffiliation();

                        academicLvl = 'elem';
                        DisplayEducation();
                        academicLvl = 'hs';
                        DisplayEducation();
                        academicLvl = 'college';
                        DisplayEducation();
                    }
                }
            })
        }

        function ClearPersonProspectVariablesFields() {
            arrPersonTelInfoList = [];
            intPersonTelInfoCtr = 0;
            PersonTelState = "";
            PersonTelRankNo = "";
            arrPersonMobileInfoList = [];
            intPersonMobileInfoCtr = 0;
            PersonMobileState = "";
            PersonMobileRankNo = "";
            arrPersonEmailInfoList = [];
            intPersonEmailInfoCtr = 0;
            PersonEmailState = "";
            PersonEmailRankNo = "";
            arrPersonOrgAffList = [];
            arrRevPriceLevels = [];
            academicLvl = '', arrElementary = [], arrHighSchool = [], arrCollege = [], isNewEducation = true, ctrElem = 0, ctrHs = 0, ctrCollege = 0, ctrIndexEducation = 0;

            if (sessionStorage.LibraryName == 'Supplier / Vendor List') {
                $('#divPerson3rdPartyID').hide(); // do not show 3rd party id field in supplier page
            } else {
                $('#divPerson3rdPartyID').show();
            }

            $('#txtCustomerPersonID').val('');
            $('#txtExistingCustomerPersonID').val('');
            $('#txtProspectPerson3rdPartyID, .txtCodeNumber').val('');
            $('#txtProspectPersonID').val('');
            $('#selPersonStatus').val('Active');
            $('#txtProspectPersonFirstName').val('');
            $('#txtProspectPersonMiddleName').val('');
            $('#txtProspectPersonLastName').val('');
            $('#txtProspectPersonPosition').val('');
            $('#txtProspectPersonPositionDescription').val('');
            $('#txtPersonAddressID').val('');
            $('#txtPersonHouseBuildingNo').val('');
            $('#txtPersonStreet').val('');
            $('#selPersonCity').val('');
            $('#selPersonRegion').val('');
            $('#selPersonIsland').val('');
            $('#selPersonCountry').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
            $('#selPersonTypeAddress').val('');
            $('#btnPersonBirthDate').text('Set Date');

            $('#lstPersonOrgAffiliation').empty();

            $('#lstPersonTelephone').empty();
            $('#txtPersonTelID').val('');
            $('#txtPersonTelAreaCodeID').val('');

            $('#lstPersonMobile').empty();
            $('#txtPersonMobileID').val('');
            $('#txtPersonTelCoPrefixID').val('');

            $('#lstPersonEmail').empty();
            $('#txtPersonEmailID').val('');

            $('#lstPersonWebsite').empty();
            $('#txtPersonWebsiteID').val('');

            $('#lstPersonContactPerson').empty();

            $('#lstElementary').empty();
            $('#lstHs').empty();
            $('#lstCollege').empty();

            ClearContactPersonFields();
            ClearPersonEmailInfoFields();
            ClearPersonMobileInfo();
            ClearPersonTelInfo();
        }
    })
})