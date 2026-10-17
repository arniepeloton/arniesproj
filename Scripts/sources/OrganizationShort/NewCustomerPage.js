require([
    'common',
    '/Scripts/sources/crm/qrcodeNew.js',//, 'tablesort'
    '/Scripts/sources/Organization/OrganizationSupplier.js?v=' + assetversion
], function () {

    require([
        '/Scripts/jquery.dataTables.min.js',
        'promise!/Scripts/jquery.filedownload.min.js'
    ], function () {
        var qr = new qrcode();
        var tBodyTopicContentExcel = $('#tBodyTopicContent');
        var tHeadTopicContentExcel = $('#theadTopicContent');
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
        var arrSalesReps = [];
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
        var mSalesChannelID;

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
        var RevSalesRepID = '', arrRevPriceLevels = [], RevPaymentTerms = '', bankID = '', dbbankid = '', arrEditVendorPrices = [], RevFowstID = '';

        $(document).ready(function () {

            $(window).scroll(function () {
                if ($(this).scrollTop()) {
                    $('#btnBackToTop').fadeIn();
                } else {
                    $('#btnBackToTop').fadeOut();
                }
            });

            //used for calendar
            window.DisplayListOfEvents = function (arr) {
                $('#pageListofEvents').pageshow();
                DisplayListOfEvents(arr);
            }

            window.ShowProspectList = function () {
                $('#pageMainDashboard').pageshow();
                if (sessionStorage.CRMDashboardPage == 'leadgen') {
                    $('#divtblLeadGen').pageshow();
                }
                else {
                    $('#divtblSalesGrinder').pageshow();
                }
                $('#divLoading').hide();
                $('#divMainPage').show();
                ShowProspectList();
            }

            window.LeadGenSave = {
                Profile: function () {
                    if (JSON.parse(sessionStorage.ClientInfo).ProspectType == 'Org') {
                        $('#btnSaveAddingContact').click();
                    }
                    else {
                        $('#btnSavePersonAddingContact').click();
                    }
                },
                ColdCall: function () {
                    $('#btnSaveColdCall').click();
                }
            }

            window.NewCustomer = {
                LoadData: function () {
                    setTimeout(function () {
                        LoadRequiredData();
                    }, 200);                    
                },
                Search: function (s) {
                    datatable.search($.trim(s)).draw();
                }
            }

            LoadRequiredData();

            $('body')
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
                $('#popOrgTelInfo').modal('show');
            })
            .on('click', '#btnCancelOrgTelephone', function () {
                $('#popOrgTelInfo').modal('hide');
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
                $('#popOrgMobileInfo').modal('show');
            })
            .on('click', '#btnCancelOrgMobile', function () {
                $('#popOrgMobileInfo').modal('hide');
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

                if ($('#txtMobileNo').val().length < 10) {
                    NewMessageBox('Mobile number must be 10 digits. Eg. 9101234567');
                    return false;
                }

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
                $('#popOrgEmailInfo').modal('show');
            })
            .on('click', '#btnCancelOrgEmailAddress', function () {
                $('#popOrgEmailInfo').modal('hide');
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
                $('#popOrgWebsiteInfo').modal('show');
            })
            .on('click', '#btnCancelOrgWebsite', function () {
                $('#popOrgWebsiteInfo').modal('hide');
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
                //$('#popContactPerson').modal('show');
                $('#popContactPersonProfile').modal('show');
            })
            .on('click', '#butAddSalesRep', function () {
                BrowsePopup('LibraryOrganizationEmployee', 'Sales Rep',
                    [
                        {
                            DisplayText: [
                                {
                                    Column: 'PeopleName'
                                }
                            ],
                            Click: function (e) {
                                var pFound = arrSalesReps.find(function(vElement) {
                                    if (ReadyGuid(vElement.PeopleID) == ReadyGuid(e.ID))
                                        if (vElement.State != 'Remove')
                                            return true;    
                                });
                                if (pFound) {
                                    NewMessageBox('That sales rep is already in the list.', 'Duplicate');
                                    return;
                                }
                                var pData = {
                                    PeopleID: e.ID,
                                    PeopleName: e.PeopleName,
                                    State: 'New'
                                }
                                arrSalesReps.push(pData);
                                DisplaySalesReps();
                            },
                            Close: function () {
                                
                            }
                        }
                    ]
                )
            })
            .on('click', '#btnCancelContact', function () {
                $('#popContactPersonProfile').modal('hide');
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
                $('#popContactPersonProfile').modal('hide');
                $('#popContactPersonTelInfo').modal('show');
            })
            .on('click', '#btnCancelContactPersonTelephone', function () {
                $('#popContactPersonTelInfo').modal('hide');
                $('#popContactPersonProfile').modal('show');
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
                            },
                            Close: function () {
                                $('#popContactPersonTelInfo').modal('show');
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
                $('#popContactPersonProfile').modal('hide');
                $('#popContactPersonMobileInfo').modal('show');
            })
            .on('click', '#btnCancelContactPersonMobile', function () {
                $('#popContactPersonMobileInfo').modal('hide');
                $('#popContactPersonProfile').modal('show');
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

                if ($('#txtContactPersonMobileNo').val().length < 10) {
                    NewMessageBox('Mobile number must be 10 digits. Eg. 9101234567');
                    return false;
                }

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
                $('#popContactPersonProfile').modal('hide');
                $('#popContactPersonEmailInfo').modal('show');
            })
            .on('click', '#btnCancelContactPersonEmailAddress', function () {
                $('#popContactPersonEmailInfo').modal('hide');
                $('#popContactPersonProfile').modal('show');
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
                                    $('#btnProspectCompanyIndustry').text(e.Class);//.button('refresh');
                                }
                            }
                        ]
                    )
            })
            .on('click', '#butSalesChannel', function () {
                BrowsePopup('LibrarySalesChannels', 'Sales Channel',
                        [
                            {
                                DisplayText: [{ Column: 'Name' }],
                                Click: function (e) {
                                    mSalesChannelID = e.ID;
                                    $('#butSalesChannel').text(e.Name);//.button('refresh');
                                }
                            }
                        ]
                    )
            })
            .on('click', '#btnProspectCompanyUserDefinedIndustry', function () {
                NewMessageBox('This feature is not yet implemented. ref: jsK78QKYFWF4!');
                return;
                BrowsePopup('LibraryIndustryClassification', 'Business Industry',
                    [
                        {
                            DisplayText: [{ Column: 'Class' }],
                            Click: function (e) {
                                $('#txtBusIndustryID').val(e.ID);
                                $('#btnProspectCompanyIndustry').text(e.Class);//.button('refresh');
                            }
                        }
                    ]
                )
            })

            .on('click', '#btnPersonBirthDate', function () {
                SingleCalendar({
                    Click: function (e) {
                        $('#btnPersonBirthDate').text(e);//.button('refresh');
                    }
                })
            })
            .on('click', '#btnAddPersonTelephone', function () {
                ClearPersonTelInfo();
                $('#popPersonTelInfo').modal('show');
            })
            .on('click', '#btnCancelPersonTelephone', function () {
                $('#popPersonTelInfo').modal('hide');
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
                            },
                            Close: function () {
                                $('#popPersonTelInfo').modal('show');
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
                $('#popPersonMobileInfo').modal('show');
            })
            .on('click', '#btnCancelPersonMobile', function () {
                $('#popPersonMobileInfo').modal('hide');
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
                $('#popPersonEmailInfo').modal('show');
            })
            .on('click', '#btnCancelPersonEmailAddress', function () {
                $('#popPersonEmailInfo').modal('hide');
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
                $('#popInteractionHistoryDetails').modal('hide');
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
                $('#popNewToDo').modal('show');
            })
            .on('click', '#btnCancelToDo', function () {
                $('#popNewToDo').modal('hide');
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
                            $('#btnEditToDoSetDate').text('--');//.button('refresh');
                        }
                        else {
                            $('#btnEditToDoSetDate').text($.format.date(new Date(e), 'MMM dd, yyyy'));//.button('refresh');
                        }
                        if (selector == "btnEditToDoSetDate") {
                            $('#popToDo').modal('show');
                        }
                    },
                    Cancel: function () {
                        if (selector == "btnEditToDoSetDate") {
                            $('#popToDo').modal('show');
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
                $('#popToDo').modal('hide');
            })
            .on('click', '#btnOkEditToDo', function () {
                $('#popToDo').modal('hide');

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
                        $('#tabPersonRevenue').appendTo($('#divCompanyProfile').find('.tab-content'));
                        $('#tabPersonRevenue').attr('id', 'tabRevenue');
                    } else {
                        $('#tabRevenue').appendTo($('#divCompanyProfile').find('.tab-content'));
                    }

                    if ($('#tabPersonEcommerce').length > 0) {
                        $('#tabPersonEcommerce').appendTo($('#divCompanyProfile').find('.tab-content'));
                        $('#tabPersonEcommerce').attr('id', 'tabEcommerce');
                    } else {
                        $('#tabEcommerce').appendTo($('#divCompanyProfile').find('.tab-content'));
                    }

                    if ($('#tabPersonBankInfo').length > 0) {
                        $('#tabPersonBankInfo').appendTo($('#divCompanyProfile').find('.tab-content'));
                        $('#tabPersonBankInfo').attr('id', 'tabBankInfo');
                    } else {
                        $('#tabBankInfo').appendTo($('#divCompanyProfile').find('.tab-content'));
                    }

                    $('#divTabCompany ul > li:first').click()
                }
                else {
                    GetProspectDetailsPeople(e.ID);
                    $('#pageProfilePeople').pageshow();
                    if ($('#tabRevenue').length > 0) {
                        $('#tabRevenue').appendTo($('#pageProfilePeople').find('.tab-content'));
                        $('#tabRevenue').attr('id', 'tabPersonRevenue');
                    } else {
                        $('#tabPersonRevenue').appendTo($('#pageProfilePeople').find('.tab-content'));
                    }

                    if ($('#tabEcommerce').length > 0) {
                        $('#tabEcommerce').appendTo($('#pageProfilePeople').find('.tab-content'));
                        $('#tabEcommerce').attr('id', 'tabPersonEcommerce');
                    } else {
                        $('#tabPersonEcommerce').appendTo($('#pageProfilePeople').find('.tab-content'));
                    }

                    if ($('#tabBankInfo').length > 0) {
                        $('#tabBankInfo').appendTo($('#pageProfilePeople').find('.tab-content'));
                        $('#tabBankInfo').attr('id', 'tabPersonBankInfo');
                    } else {
                        $('#tabPersonBankInfo').appendTo($('#pageProfilePeople').find('.tab-content'));
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
                $('#divtooltipNotes').modal('hide');
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
                $('#divtooltipNotes').modal('show');
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
                $('#divtooltipNotes').modal('show');
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
                $('#divtooltipNotes').modal('hide');
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
                                $('#lstPersonOrgAffiliation a').each(function () {
                                    if ($(this).data().OrgID == e.OrgID) {
                                        isExist = true;
                                    }
                                })

                                if (isExist) {
                                    WebToast(e.CompanyName + ' already exist');
                                    return false;
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
                    ], { IsMcr : true }
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
                                $('#lstAffiliatedPerson a').each(function () {
                                    if ($(this).data().PeopleID == e.PeopleID) {
                                        isExist = true;
                                    }
                                })

                                if (isExist) {
                                    WebToast(e.PeopleName + ' already exist')
                                    return false;
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
                                $('#lstOrgAffiliation a').each(function () {
                                    if ($(this).data().OrgID == e.OrgID) {
                                        isExist = true;
                                    }
                                })

                                if (isExist) {
                                    WebToast(e.CompanyName + ' already exist');
                                    return false;
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
                    ], {IsMcr : true}
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
            .on('change', '#fileCustomerDataMapping', function (e) {
                $('#txtCustomerDataMapping').val(sessionStorage.OrgID)
                $('#formUploadCustomerDataMapping').submit();
            })
            .on('submit', '#formUploadCustomerDataMapping', function (e) {
                e.preventDefault();
                var formData = new FormData($(this)[0]);
                NewMessageBox('This might take a few moments. Please wait while the system uploading ...', 'Message');
                setTimeout(function () {
                    AjaxSend({
                        url: '/OrganizationShort/UploadCustomerVendorDataMapping?OrgID=' + sessionStorage.OrgID,
                        type: 'POST',
                        data: formData,
                        async: false,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            $('#fileCustomerDataMapping').val('');
                            if (vData.Results == "saved") {
                                NewMessageBox('Successfully uploaded', 'Message');
                            } else if (vData.Validation != undefined) {
                                NewMessageBox('While we validating your file template, these are the error we found: <br> ' + vData.Validation, 'Validation Message');
                            } else {
                                NewMessageBox(vData.Error, 'ERROR');
                            }
                        }
                    });
                }, 1000)
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
                                $('#popSMS').modal('show');
                                $('#txtSMSMessage').val('').focus().height('100px');
                                arrSMSRecipient = [];
                                DisplayListOfReceipient();
                            }
                        },
                        {
                            Text: 'View SMS Logs',
                            Click: function () {
                                $('#popSMSLogs').modal('show');
                                GetSmSUsage();
                            }
                        }
                    ]
                )
            })
            .on('click', '#btnCancelSMS', function () {
                $('#popSMS').modal('hide');
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
                $('#popSMSRecipient').modal('hide');
                DisplayListOfReceipient();
            })
            .on('click', '#btnCancelSMSSelectedReceipient', function () {
                isSendingBulkMsg = false;
                arrSMSRecipient = [];
                ctrUnknownNumber = 1;
                $('#listofRecipient').html('');
                $('#listSelectedReceipient').empty();
                $('#txtSMSMessage').val('');
                $('input[name="chbbulk"]').prop('checked', false);//.checkboxradio('refresh');
                $('.bulk, .spanbtnSMS, #popSMS, #popSMSRecipient').hide();
            })
            .on('click', '#btnViewSMSSelectedReceipient, #btnViewRecipientsDetails', function () {
                $('#popSMS').modal('hide');
                $('#popSMSRecipient').modal('show');
                DisplayListOfSelectedReceipient();
            })
            .on('click', '#btnComposeSMSSelectedReceipient', function () {
                $('#popSMS').modal('show');
                DisplayListOfReceipient();
            })
            .on('click', '#btnSMSLogs', function () {
                $('#popSMSLogs').modal('show');
                GetSmSUsage();
            })
            .on('change', '#chbbulk', function () {
                var chbBulk = $(this)
                $('#tblProspectbody tr').each(function () {
                    if ($(this).css('display') != 'none') {
                        if (chbBulk.is(':checked')) {
                            $(this).find('td:eq(0)').find('input[type="checkbox"]').prop('checked', true);//.checkboxradio('refresh');
                        }
                        else {
                            $(this).find('td:eq(0)').find('input[type="checkbox"]').prop('checked', false);//.checkboxradio('refresh');
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
                $('#popSMS').modal('hide');
                $('#popSearchContacts').show();
                $('#txtSearchContacts').val('').focus();
                SearchClientContact('');
            })
            .on('click', '#btnAddNewNumber', function () {
                $('#btnNewNumberTelcoPrefix').text('Please select');//.button('refresh');
                $('#txtNewNumber').val('');
                $('#popSMS').modal('hide');
                $('#popNewNumber').modal('show')
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
                $('#popNewNumber').modal('hide');
                $('#popSMS').modal('show');
            })
            .on('click', '#btnCancelNewNumber', function () {
                $('#popNewNumber').modal('hide');
            })
            .on('click', '#btnNewNumberTelcoPrefix', function () {
                BrowsePopup('LibraryTelcoPrefixes', 'Telco Codes',
                    [
                        {
                            AddButton: [{
                                Text: 'Back',
                                Click: function () {
                                    $('#popNewNumber').modal('show');
                                    $('#txtNewNumber').focus();
                                }
                            }],
                            DisplayText: [{ Column: 'Prefix' }, { Column: 'TelCo' }],
                            Click: function (e) {
                                $('#popNewNumber').modal('show');
                                $('#btnNewNumberTelcoPrefix').text(e.Prefix);//.button('refresh');
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
                        $('#btnDateTimeTo').text($.format.date(tempDate, 'h:mm a'));//.button('refresh');
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
                        $('#btnDateTimeTo').text($.format.date(tempDate, 'h:mm a'));//.button('refresh');
                        $('#btnDateTimeFrom').text(e.time);//.button('refresh');
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
                            $('#btnDateTimeFrom').text($.format.date(tempDate, 'h:mm a'));//.button('refresh');
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
                            $('#btnDateTimeFrom').text($.format.date(tempDate, 'h:mm a'));//.button('refresh');
                        }
                        $('#btnDateTimeTo').text(e.time);//.button('refresh');
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
                $('#popFilter').modal('show');
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
                $('#popFilter').modal('hide');
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
                        $('#btnRevCreditBegAsOf').text(e);//.button('refresh');
                    }
                })
                $('#tabRevenue').opentabmenu();
            })

            .on('click', '#btnRevThirdPartyBegBalanceAsOf', function () {
                SingleCalendar({
                    Click: function (e) {
                        $('#btnRevThirdPartyBegBalanceAsOf').text(e);//.button('refresh');
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
                $('#prevOrgProfile').modal('hide');
            })
            .on('click', '#btnPreviewClose', function () {
                $('#prevOrgProfile').modal('hide');
            })

            .on('click', '#btnAddNewCustomer', function () {
                AjaxSend({
                    url: 'SetupChecklist/CheckAddLibrary',
                    data: { LibraryName: (sessionStorage.LibraryName).replaceAll(' ', '').toLowerCase() },
                    success: function (e) {
                        if (e.IsSearchRecords == 1) {
                            $('#panAddCustomerLibrary').modal('show');
                            $('#txtCustomerSearch').val('').focus();
                            $('#CustomerlstSearchFound').empty();
                            $('#divCustomerNoResultsFound, #btnAddToCustomerLibrary').hide();
                            PopupReposition();
                        } else {
                            $('#btnAddToCustomerLibrary').click();
                        }
                    }
                })                
            })
            .on('click', '#btnSearchCustomerLibrary', function () {
                CustomerSearch();
            })
            .on('keyup', '#txtCustomerSearch', function (e) {
                if(e.which == 13){
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
                                    $('#tabPersonRevenue').appendTo($('#divCompanyProfile').find('.tab-content'));
                                    $('#tabPersonRevenue').attr('id', 'tabRevenue');
                                } else {
                                    $('#tabRevenue').appendTo($('#divCompanyProfile').find('.tab-content'));
                                }
                                $('#tabCompanyProfile').opentabmenu();
                                NewCustomerOptions('Organization Details');
                            }
                        },
                        {
                            Text: 'Add Person',
                            Click: function () {
                                ClearPersonProspectVariablesFields();
                                if (location.pathname.toLowerCase() == '/erp') {
                                    $('#divSystemSettings').find('#pageActions, #pageProfile, #pageProfilePeople').pageshow();
                                } else {
                                    $('#pageActions, #pageProfile, #pageProfilePeople').pageshow();
                                }

                                if ($('#tabRevenue').length > 0) {
                                    $('#tabRevenue').appendTo($('#pageProfilePeople').find('.tab-content'));
                                    $('#tabRevenue').attr('id', 'tabPersonRevenue');
                                } else {
                                    $('#tabPersonRevenue').appendTo($('#pageProfilePeople').find('.tab-content'));
                                }
                                $('#tabPersonProfile').opentabmenu();
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
                            Text: 'Upload Customers Data Mapping for 3rd Party Systems',
                            Click: function () {
                                $('#fileCustomerDataMapping').trigger('click');
                            }
                        },
                        {
                            Text: 'Download Customers Data Mapping for 3rd Party Systems',
                            Click: function () {
                                window.location = GetPathFormat("/OrganizationShort/DownloadCustomerVendorDataMappingTemplate");
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
                                        function() {
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
                                $('#btnRevSalesRepresentative').text(e.PeopleName);//.button('refresh');
                            }
                        }
                    ])

                $('#tabRevenue').opentabmenu();
            })
            .on('click', '#btnRevPriceLevel', function () {
                if (CStr2(RevFowstID) == '') {
                    WebToast('Please select FOWST first');
                    return;
                }

                BrowsePopup('LibraryPIMPriceLevel', 'Search Price Level',
                    [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'PriceLevel' }],
                            Validation: function(e){
                                var isExist = false;

                                for (var i = 0; i < arrRevPriceLevels.length; i++){
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
                                //$('#btnRevPriceLevel').text(e.PriceLevel);//.button('refresh');
                            }
                        }
                    ], { FowstID: RevFowstID })
            })
            .on('click', '#btnRevCreditTerms', function () {
                BrowsePopup('LibraryPaymentTerms', 'Search Payment Terms',
                    [
                        {
                            DisplayText: [{ Column: 'Name' }],
                            AutoSearch: true,
                            Click: function (e) {
                                RevPaymentTerms = e.ID;
                                $('#btnRevCreditTerms').text(e.Name);//.button('refresh');
                            }
                        }
                    ])
                $('#tabRevenue').opentabmenu();
            })

            .on('keyup', '#txtSearchNewCustomer', function (e) {
                if(e.keyCode == 13){
                    NewCustomer.Search($(this).val());
                }
            })
            .on('click', '#btnAddCompanyOutlet', function () {
                $('#popCompanyOutlet').modal('show')//popup('open');                
                $('#txtOutletHouseBuildingNo').val('');
                $('#txtOutletStreet').val('');
                $('#txtCompanyOutlet3rdPartyID').val('');
                $('#selOutletCity').val('');//.selectmenu('refresh');
                $('#selOutletCountry').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');//.selectmenu('refresh');
                $('#lstOutletTelephone, #lstOutletMobile, #lstOutletEmail, #lstOutletDepartment').empty();
                $('input[name="radAutoSku"]').removeAttr('checked');//.checkboxradio('refresh');
                $('input[name="radAutoSku"][value="all"]').attr('checked', true);//.checkboxradio('refresh');
                $('#tbodyOutletSkus').empty();
                $('#divForSomeSkus').hide();
                $('#tabOutletInfo').opentabmenu();
                $('#txtCompanyOutlet').val('').focus();
                CompanyOutletState = "";
                arrOutletTelInfoList = [];
                arrOutletMobileInfoList = [];
                arrOutletEmailInfoList = [];
                arrCompanyOutletDepartment = [];
            })
            .on('change', 'input[name="radAutoSku"]', function () {
                if($(this).val() == 'all'){
                    $('#divForSomeSkus').hide();
                } else {
                    $('#divForSomeSkus').show();
                }
            })
            .on('click', '#btnAddOutletDepartment', function () {
                //$('#popCompanyOutlet').modal('hide');
                $('#popCompanyOutletDepartment').modal('show');
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

                // dont require 2024_03_13__10_48_57_4829
                //if (txtCompanyOutlet == "") {
                //    WebToast('Please enter outlet name.', 'top');
                //    return;
                //}

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
                $('#popCompanyOutlet').modal('hide');//popup('close');
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
                    ], { HidePrevPopup: false })
                } else {
                    arrCompanyOutletDepartment[sessionStorage.DepartmentIndex].Department = txtCompanyOutletDepartment;
                    arrCompanyOutletDepartment[sessionStorage.DepartmentIndex].ThirdPartyID = txtCompanyOutletDepartment3rdPartyID;
                }

                DisplayCompanyOutletDepartment();
                $('#popCompanyOutletDepartment').modal('hide');
                $('#popCompanyOutlet').modal('show');
            })
            .on('click', '#btnCancelCompanyOutletDepartment', function () {
                $('#popCompanyOutletDepartment').modal('hide');
                $('#popCompanyOutlet').modal('show');
            })
            .on('click', '#btnOutletTelAreaCode', function () {
                BrowsePopup('LibraryAreaCodes', 'Area Codes',
                    [
                        {
                            DisplayText: [{ Column: 'Area' }, { Label: 'Code', Column: 'CallingCode' }],
                            Click: function (e) {
                                $('#txtOutletTelAreaCodeID').val(e.ID);
                                $('#txtOutletAreaCode').text(e.Area + ' - ' + e.CallingCode)
                            },
                            Close: function () {
                                $('#popOutletTelInfo').modal('show');
                            }
                        }
                    ]
                )
            })
            .on('click', '#btnAddOutletTelephone', function () {
                ClearOutletTelInfo();
                $('#popCompanyOutlet').modal('hide');
                $('#popOutletTelInfo').modal('show')
            })
            .on('click', '#btnCancelOutletTelephone', function () {
                $('#popOutletTelInfo').modal('hide');
                $('#popCompanyOutlet').modal('show');
                OutletTelState = '';
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
                $('#popCompanyOutlet').modal('hide');
                $('#popOutletMobileInfo').modal('show');
            })
            .on('click', '#btnCancelOutletMobile', function () {
                $('#popOutletMobileInfo').modal('hide');
                $('#popCompanyOutlet').modal('show');
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
                            },
                            Close: function () {
                                $('#popOutletMobileInfo').modal('show');
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
                $('#popCompanyOutlet').modal('hide');
                $('#popOutletEmailInfo').modal('show');
            })
            .on('click', '#btnCancelOutletEmailAddress', function () {
                $('#popOutletEmailInfo').modal('hide');
                $('#popCompanyOutlet').modal('show');
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
                $('#popCompanyOutlet').modal('hide');
                $('#popCompanyOutletExSku').modal('show');
            })
            .on('click', '#btnOutletAddSku', function () {
                BrowsePopup('LibraryPIMProduct', 'Select SKU', [
                    {
                        HidePrevPopup: false,
                        Message: 'You can select multiple SKUs. Click OK when you\'re done.',
                        DisplayText: [{Label: 'Product No.', Column: 'ProductNo' }, { Column: 'ProductName' }],
                        SelectMultiple: true,
                        Click: function (e) {
                            DisplayOutletSkus(e);
                        },
                        Close: function () {
                            $('#popCompanyOutlet').modal('show');
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
                        if(e.Results == 'saved'){
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
                if ((sessionStorage.ViewID || sessionStorage.ClientInfo) == undefined) {
                    NewMessageBox('You need to create a profile first before you can generate a Public URL.')
                    return;
                }

                AjaxSend({
                    url: 'OrganizationShort/GeneratePublicUrl',
                    data: {
                        IsModule: true,
                        ID: (sessionStorage.ViewID || JSON.parse(sessionStorage.ClientInfo).ID),
                        Module: (sessionStorage.LibraryName == 'Customer / Client List' ? 'customer' : 'supplier'),
                        ProfileType: (sessionStorage.LibraryName == 'Customer / Client List' ? JSON.parse(sessionStorage.ClientInfo).ProspectType : 'Org')
                    }, success: function (e) {
                        if (e.Error != undefined) {
                            NewMessageBox(e.Error, 'Message');
                        } else {
                            GetGeneratedPublicUrl();
                        }                        
                    }
                })
            })
            .on('click', '#btnCopyGenUrl', function () {
                CopyToClipboard('#txtPublicUrl');
            })
            .on('click', '#btnCPGeneratePublicUrl', function () {
                if (ContactPersonState == "") {
                    NewMessageBox('Please save this contact person details first before you can generate a public URL.', 'Message', undefined, {
                        Close: function () {
                            $('#popContactPersonProfile').modal('show');
                        }
                    })
                    return;
                }

                AjaxSend({
                    url: 'OrganizationShort/GeneratePublicUrl',
                    data: {
                        IsModule: false,
                        ID: arrContactPersonList[CInt2(sessionStorage["Contactindex"])].PeopleID,
                        CompanyID: (sessionStorage.ViewID || JSON.parse(sessionStorage.ClientInfo).ID),
                        Module: (sessionStorage.LibraryName == 'Customer / Client List' ? 'customer' : 'supplier')
                    }, success: function (e) {
                        if (e.Error != undefined) {
                            NewMessageBox(e.Error, 'Message');
                        } else {
                            DisplayContactPersonUrl([{ Link: e.Link }]);
                        }
                    }
                })
            })
            .on('click', '#btnCPCopyUrl', function () {
                CopyToClipboard('#txtCPLink');
            })
            .on('click', '#btnUploadAttachments', function () {
                if ((sessionStorage.ViewID || sessionStorage.ClientInfo) == undefined) {
                    NewMessageBox('You need to create a profile first before you can generate a Public URL.')
                    return;
                }

                $('#fileUploadAttachments').val('')
                $('#fileUploadAttachments').click()
            })
            .on('submit', '#formUploadAttachments', function (e) {
                e.stopPropagation();
                e.preventDefault();

                var formdata = new FormData();
                var fileUpload = $('#fileUploadAttachments').get(0);
                var files = fileUpload.files;

                NewMessageBox('Please wait while uploading your file', 'Message');

                for (var i = 0; i < files.length; i++) {
                    formdata.append(files[i].ID, files[i]);
                }

                AjaxSend({
                    url: 'OrganizationShort/UploadFiles?OrgID=' + sessionStorage.OrgID + '&id=' + (sessionStorage.ViewID || JSON.parse(sessionStorage.ClientInfo).ID),
                    data: formdata,
                    async: true,
                    cache: false,
                    contentType: false,
                    processData: false,
                    dataType: 'json',
                    success: function (vData) {
                        if (vData.Results == 'uploaded') {
                            NewMessageBoxClose();
                            WebToast('Successfully uploaded', 'top');
                            GetFiles()
                        } else {
                            NewMessageBox(vData.Results, 'Error');
                        }
                    }
                })
            })
            .on('change', '#fileUploadAttachments', function () {
                $('#formUploadAttachments').submit()
            })
            .on('click', '#btnAddSupplierCanvassForm', function () {
                if ((sessionStorage.ViewID || sessionStorage.ClientInfo) == undefined) {
                    NewMessageBox('You need to create a profile first before you can add Canvass Form.')
                    return;
                }

                BrowsePopup('LibraryCanvassForm', 'Select Canvass Form', [
                    {
                        DisplayText: [{ Column: 'CanvassName' }],
                        SelectMultiple: true,
                        AutoSearch: true,
                        Message: 'You can select multiple form. Click OK when you\'re done.',
                        Click: function (e) {
                            AjaxSend({
                                url: 'OrganizationShort/SaveSupplierCanvassForm',
                                data: { forms: e, SupplierID: sessionStorage.ViewID },
                                success: function (e) {
                                    if (e.Results == 'saved'){
                                        GetSupplierCanvassForm();
                                    } else {
                                        NewMessageBox(e.Error, 'Error')
                                    }
                                }
                            })
                        }
                    }
                ])
            })
            .on('click', '#btnCusSupAddBankInfo', function () {
                if (sessionStorage.ViewID == undefined) {
                    NewMessageBox('You need to save first the profile before you can add a Bank Information', 'Message');
                    return;
                }

                ClearBankInfoFields();
                $('#popCusSupFinancialDetails').modal('show');
            })
            .on('click', '[name="butCusSupBank"]', function () {
                BrowsePopup('LibraryOrganizationBanks', 'Search Bank', [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'CompanyName' }],
                        Click: function (e) {
                            bankID = e.ID;
                            $('[name="butCusSupBank"]').text(e.CompanyName);
                        },
                        Close: function () {
                            $('#popCusSupFinancialDetails').modal('show');
                        }
                    }
                ], { RankIfEmptySeach: 1 })
            })
            .on('click', '[name="butCusSupSave"]', function () {
                if (bankID == '') {
                    WebToast('Please select a Bank', 'top');
                    return;
                }

                if ($.trim($('[name="txtCusSupAccountNumber"]').val()) == '') {
                    WebToast('Please enter Account Number', 'top');
                    return;
                }

                AjaxSend({
                    url: 'CreditAndCollection/SaveFinancialDetails',
                    data: {
                        ID: dbbankid,
                        CustomerOrgID: sessionStorage.ViewID,
                        BankOrgID: bankID,
                        BankBranch: $.trim($('[name="txtCusSupBranch"]').val()),
                        AccountName: $.trim($('[name="txtCusSupAccountName"]').val()),
                        AccountNumber: $.trim($('[name="txtCusSupAccountNumber"]').val()),
                        PaymentType: $.trim($('[name="cbCusSupAccountyType"]').val()),
                        Deactivated: $.trim($('[name="chkDeactivated"]').prop('checked') ? 1 : 0)
                    }, success: function (e) {
                        if (e.Error) {
                            NewMessageBox(e.Error, 'Failed');
                            return;
                        }
                        WebToast('Successfully saved', 'top');
                        $('#popCusSupFinancialDetails').modal('hide');
                        GetCusSupBankInfo();
                    }
                })
            })
            .on('keydown', '#txtCompanyOutletSearch', function (e) {
                if (e.which == 13) {
                    var dis = $(this);
                    Bs.Loading('show', 'Searching ...');
                    setTimeout(function () {
                        var keywords = $.trim(dis.val()).toLowerCase();
                        var li = $('#lstCompanyOutlet a')

                        li.show().filter(function () {
                            return $(this).find('td:first-child').text().toLowerCase().trim().indexOf(keywords) == -1;
                        }).hide();

                        Bs.Loading('hide');
                    }, 100);
                }                
            })
            .on('click', '#btnDownloadVendorSKUs', function () {
                window.location = GetPathFormat("/Organization/DownloadCompanyProductList?OrgID=" + sessionStorage.OrgID + '&VendorID=' + sessionStorage.ViewID + '&IsEditVendorPrice=1');
            })
            .on('click', '#btnUploadVendorSKUs', function () {
                $('#fileUploadVendorSKUs').val('').click();
            })
            .on('change', '#fileUploadVendorSKUs', function () {
                $('#formUploadVendorSKUs').submit();
            })
            .on('submit', '#formUploadVendorSKUs', function (e) {
                e.stopPropagation();
                e.preventDefault();
                var formData = new FormData($(this)[0]);
                mUploaderKey = NewID();
                formData.append('OrgID', sessionStorage.OrgID);
                formData.append('VendorID', sessionStorage.ViewID);

                NewMessageBox('We will be updating ' + $.trim($('#txtProspectCompanyName').val()) + '’s prices for the products listed on this file. <br><br>' +
                    'Please note that the system will not update product names, unit measure details and all other non-price information. <br><br>' +
                    'Please use the PIM module in the system settings for changes to product descriptions, unit measures, selling prices and other product-centric information. <br><br>' +
                    'Thank you.', 'Important Note', [
                        {
                            Text: 'Upload Vendor PIM Template', Click: function () {
                                Bs.Loading('show', 'Please wait ...');

                                setTimeout(function () {
                                    AjaxSend({
                                        url: '/OrganizationShort/UploadVendorPim?OrgID=' + sessionStorage.OrgID + '&VendorID=' + sessionStorage.ViewID,
                                        type: 'POST',
                                        data: formData,
                                        async: false,
                                        cache: false,
                                        contentType: false,
                                        processData: false,
                                        success: function (vData) {
                                            Bs.Loading('hide');
                                            $('#fileUploadVendorSKUs').val('');
                                            if (vData.Prices == "" && vData.ProductNos == "") {
                                                GetVendorSKUs();
                                                NewMessageBox('Successfully uploaded', 'Message');
                                            } else if (vData.Prices != "" || vData.ProductNos != "") {
                                                NewMessageBox('While we validating your file template, these are the error we found: <br><br> ' + vData.ProductNos + ' <br> ' + vData.Prices, 'Validation Message');
                                            } else {
                                                NewMessageBox(vData.Error, 'ERROR');
                                            }
                                        }
                                    });
                                }, 1000)
                            }
                        }, { Text: 'Cancel' }
                    ])
            })
            .on('click', '#btnCompanyNotes', function () {
                Bs.Loading('show');
                require(['promise!/Scripts/sources/OrganizationShort/notes/controller.js?v=' + assetversion], function () {
                    Bs.Loading('hide');
                    ERP.Company.main.controller.Load({
                        module: 'systemsettings',
                        CompanyID: sessionStorage.ViewID,
                    });
                })
            })
            .on('click', '#btnRevFowst', function () {
                BrowsePopup('LibraryFowst', 'Select FOWST', [
                    {
                        DisplayText: [{ Column: 'FowstName' }],
                        AutoSearch: true,
                        Click: function (e) {
                            if (arrRevPriceLevels.length > 0) {
                                NewMessageBox('The Price Level list will also be removed if the chosen FOWST location is changed.<br><br> Do you want to continue?', 'Confirmation Message', [
                                    {
                                        Text: 'Yes', Click: function () {
                                            $.grep(arrRevPriceLevels, function (p) {
                                                p.State = 'remove';
                                            })

                                            DisplayRevPriceLevels();

                                            RevFowstID = e.ID;
                                            $('#btnRevFowst').text(e.FowstName);
                                        }
                                    }, { Text: 'No' }
                                ])
                            } else {
                                RevFowstID = e.ID;
                                $('#btnRevFowst').text(e.FowstName);
                            }
                        }
                    }
                ], { IsAll: true })
            })
            .on('click', '#btnRevAddCreditTerms', function () {
                ClearCusRevenue();
                var pMode = $('#selRevenueSingleFowst option:selected').val();
                if (pMode == 'single') {

                    if ($('#tbodyRevCreditTerms tr').length > 0) {
                        NewMessageBox('For single credit term mode, please click a row below.')
                        return;
                    }
                    $('#divRevenueFowst').hide();
                }  else
                    $('#divRevenueFowst').show();
                $('#popCusCreditTerms').modal('show');

                $('[name="butCusCreditTerms"]').unbind('click').click(function () {
                    var data = ValidateRevenue();

                    if (data != false) {
                        DisplayRevCreditTerms(data, 'new');
                        $('#popCusCreditTerms').modal('hide');
                    }
                })
            })
            .on('change', '#selRevenueSingleFowst', function() {
                var pMode = $('#selRevenueSingleFowst option:selected').val();
                    $('#tbodyRevCreditTerms tr').each(function() {
                        var tr = $(this);
                        var data = tr.data('data');
                        if (pMode == 'single') {
                            if (CStr2(data.FowstID) != '')
                                tr.hide();
                            else
                                tr.show();
                        } else {
                            if (CStr2(data.FowstID) == '')
                                tr.hide();
                            else
                                tr.show();
                        }
                    });
                });

            function DisplayRevCreditTerms(data, status) {
                var pMode = $('#selRevenueSingleFowst option:selected').val();

                var tr = $('<tr>')
                    .data('data', data)
                    .append($('<td div style="text-align: center">').text(CStr2(data.FowstName)))
                    .append($('<td div style="text-align: center">').text(CStr2(data.SalesRepName)))
                    .append($('<td div style="text-align: center">').text(CStr2(data.TermName)))
                    .append($('<td div style="text-align: center">').text(CStr2(data.CreditLimit)))
                    .append($('<td div style="text-align: center">').text(CStr2(data.EncodedByName)))
                    .append($('<td div style="text-align: center">').text(CStr2(data.ApprovedByName)))
                    .click(function() {
                        var dis = $(this), data = dis.data('data');
                        console.log(data);
                        ClearCusRevenue();
                        RevFowstID = data.FowstID;
                        RevSalesRepID = data.SalesRepID;
                        RevPaymentTerms = data.TermID;
                        arrRevPriceLevels = data.PriceLevels;
                        DisplayRevPriceLevels();
                        $('#btnRevFowst').text((CStr2(data.FowstName) == '' ? 'Search FOWST' : data.FowstName));
                        $('#btnRevSalesRepresentative').text((CStr2(data.SalesRepName) == '' ? 'Search Representative' : data.SalesRepName));
                        $('#btnRevCreditTerms').text((CStr2(data.TermName) == '' ? 'Search Payment Terms' : data.TermName));
                        $('#txtRevCreditLimit').val(data.CreditLimit);
                        $('#chkRevUnlimitedCredit').prop('checked', data.UnlimitedCredit);
                        $('#chkRevRequireCnCApproval').prop('checked', data.RequireCnCApproval);
                        $('#chkRevCustomerRequirePoNum').prop('checked', data.CustomerRequirePoNum);
                        $('#txtRevCreditBegBal').val(data.CreditBegBalance);
                        $('#btnRevCreditBegAsOf').text((CStr2(data.CreditAsOf) == '' ? 'Set Date' : $.format.date(new Date(data.CreditAsOf), 'MMM dd, yyyy')));
                        $('#txtRevThirdPartyBegBalance').val(data.ThirdPartyBegBalance);
                        $('#btnRevThirdPartyBegBalanceAsOf').text((CStr2(data.ThirdPartyBegBalanceAsOf) == '' ? 'Set Date' : $.format.date(new Date(data.ThirdPartyBegBalanceAsOf), 'MMM dd, yyyy')));
                        $('#popCusCreditTerms').modal('show');

                        $('[name="butCusCreditTerms"]').unbind('click').click(function() {
                            var newdata = ValidateRevenue();

                            if (newdata != false) {
                                newdata = $.extend(data, newdata);
                                tr = DisplayRevCreditTerms(newdata, 'edit');
                                dis.data('data', newdata).empty().append(tr.find('td'));
                                $('#popCusCreditTerms').modal('hide');
                            }
                        })
                    });
                if (pMode == 'single') {
                    if (CStr2(data.FowstID) != '')
                        tr.hide();
                    else
                        tr.show();
                } else {
                    if (CStr2(data.FowstID) == '')
                        tr.hide();
                    else
                        tr.show();
                }

                if (status == 'edit') {
                    return tr;
                } else {
                    $('#tbodyRevCreditTerms').append(tr);
                }
            }

            function ValidateRevenue() {
                var pMode = $('#selRevenueSingleFowst option:selected').val();
                if (pMode != 'single')
                    if (RevFowstID == '') {
                        WebToast('Please select FOWST location');
                        return false;
                    }

                if (RevPaymentTerms == '') {
                    WebToast('Please select credit terms');
                    return false;
                }

                if ($('#txtRevCreditLimit').val() == '') {
                    WebToast('Please enter credit limit amount');
                    return false;
                }

                return GetRevenueData();
            }

            function GetRevenueData(){
                return {
                    FowstID: RevFowstID,
                    FowstName: ($('#btnRevFowst').text() == 'Search FOWST' ? '' : $('#btnRevFowst').text()),
                    SalesRepID: RevSalesRepID,
                    TermID: RevPaymentTerms,
                    SalesRepName: ($('#btnRevSalesRepresentative').text() == 'Search Representative' ? '' : $('#btnRevSalesRepresentative').text()),
                    TermName: ($('#btnRevCreditTerms').text() == 'Search Payment Terms' ? '' : $('#btnRevCreditTerms').text()),
                    CreditLimit: $('#txtRevCreditLimit').val(),
                    CreditBegBalance: $('#txtRevCreditBegBal').val(),
                    CreditAsOf: ($('#btnRevCreditBegAsOf').text() == 'Set Date' ? '' : $('#btnRevCreditBegAsOf').text()),
                    ThirdPartyBegBalance: $('#txtRevThirdPartyBegBalance').val(),
                    ThirdPartyBegBalanceAsOf: ($('#btnRevThirdPartyBegBalanceAsOf').text() == 'Set Date' ? '' : $('#btnRevThirdPartyBegBalanceAsOf').text()),
                    UnlimitedCredit: $('#chkRevUnlimitedCredit').is(':checked'),
                    RequireCnCApproval: $('#chkRevRequireCnCApproval').is(':checked'),
                    CustomerRequirePoNum: $('#chkRevCustomerRequirePoNum').is(':checked'),
                    PriceLevels: arrRevPriceLevels
                }
            }

            function ClearCusRevenue() {
                RevFowstID = '';
                RevSalesRepID = '';
                RevPaymentTerms = '';
                arrRevPriceLevels = [];
                $('#btnRevFowst').text('Search FOWST');
                $('#btnRevSalesRepresentative').text('Search Representative');
                $('#btnRevPriceLevel').text('Search Price Level');
                $('#btnRevCreditTerms').text('Search Payment Terms');
                $('#txtRevCreditLimit').val('');
                $('#chkRevUnlimitedCredit').prop('checked', 0);
                $('#chkRevRequireCnCApproval').prop('checked', 0);
                $('#chkRevCustomerRequirePoNum').prop('checked', 0);
                $('#txtRevCreditBegBal').val('');
                $('#btnRevCreditBegAsOf').text('Set Date');
                $('#txtRevThirdPartyBegBalance').val('');
                $('#btnRevThirdPartyBegBalanceAsOf').text('Set Date');
                $('#lstOrgPriceLevel').empty();
            }

            function GetCusSupBankInfo() {
                var tbodyCusSupBankInfo = $('#tbodyCusSupBankInfo');

                tbodyCusSupBankInfo.empty();

                AjaxSend({
                    url: 'CreditAndCollection/GetFinancialDetails',
                    data: {
                        CustomerOrgID: sessionStorage.ViewID,
                        IncludeDeactivated: 1
                    }, success: function (e) {
                        var vRows = e.Rows;

                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                var pType = 'Checking account';
                                if (CBool(vRows[i].OnlinePayment))
                                    pType = 'Online banking account';
                                tbodyCusSupBankInfo.append($('<tr>')
                                    .data('data', vRows[i])
                                    .append($('<td>').text(i + 1 + '.'))
                                    .append($('<td>').text(vRows[i].BankName))
                                    .append($('<td>').text(vRows[i].BankBranch))
                                    .append($('<td>').text(vRows[i].AccountName))
                                    .append($('<td>').text(vRows[i].AccountNumber))
                                    .append($('<td>').text(pType))
                                    .append($('<td>').text(CStr2(vRows[i].DeactivatedByID).length > 0 ? 'Deactivated' : 'Active'))
                                    .click(function() {
                                        ClearBankInfoFields();

                                        var data = $(this).data('data');

                                        dbbankid = data.ID;
                                        bankID = data.BankOrgID;
                                        $('[name="butCusSupBank"]').text(data.BankName);
                                        $('[name="txtCusSupBranch"]').val(data.BankBranch);
                                        $('[name="txtCusSupAccountName"]').val(data.AccountName);
                                        $('[name="txtCusSupAccountNumber"]').val(data.AccountNumber);
                                        $('[name="chkDeactivated"]').prop('checked', CStr2(data.DeactivatedByID).length > 0);
                                        var pAccountType = 'cheque';
                                        if (CBool(data.OnlinePayment))
                                            pAccountType = 'online-payment';
                                        $('[name="cbCusSupAccountyType"]').val(pAccountType);

                                        $('#popCusSupFinancialDetails').modal('show');
                                    })
                                );
                            }
                        } else {
                            tbodyCusSupBankInfo.append($('<tr>')
                                    .append($('<td colspan="5" class="txt-center">').text('No results found'))
                                )
                        }
                    }
                })
            }

            function GetVendorSKUs() {
                if (sessionStorage.LibraryName == 'Customer / Client List') {
                    return
                }

                AjaxSend({
                    url: '/Browser/LibraryPIMProduct',
                    data: {
                        IsAll: 1,
                        VendorID: sessionStorage.ViewID,
                        FilterSupplierID: sessionStorage.ViewID
                    }, success: function (e) {
                        if ($.fn.DataTable.isDataTable("#tblVendorSKUs")) {
                            //check if datatable is created 
                            $('#tblVendorSKUs').DataTable().clear().destroy();
                        }

                        var ctr = 0, pRows = e.Results.Rows, tblVendorSKUs = $('#tblVendorSKUs').DataTable({
                            "bdestroy": true,
                            "pageLength": 25,
                            "order": [[0, "asc"]],
                            data: pRows,
                            "autoWidth": false,
                            "columns": [
                                {
                                    "data": function (data, type, full, meta) {
                                        ctr += 1;
                                        return ctr + '.';
                                    },
                                    "width": "30px"
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return data.ProductNo;
                                    }
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return CStr2(data.Brand);
                                    }
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return data.ProductNameOnly;
                                    }
                                },
                                {
                                    "data": null,
                                    createdCell: function (td, cellData, rowData, row, col) {
                                        var inp = $('<input type="number" data-role="none" class="form-control text-center">')
                                                    .data('data', rowData)
                                                    .val(CDec2(rowData.VendorPrice))
                                                    .blur(function () {
                                                        var dis = $(this), val = CDec2(dis.val()), data = dis.data('data'), index = null;
                                                        
                                                        $.grep(arrEditVendorPrices, function (e, ind) {
                                                            if (e.ID == data.ID) {
                                                                index = ind;
                                                                return false;
                                                            }
                                                        })

                                                        if (CDec2(data.VendorPrice) == val) {
                                                            if (index != null)
                                                                arrEditVendorPrices.splice(index, 1);
                                                            console.log(arrEditVendorPrices)
                                                            return
                                                        }

                                                        if (index == null) {
                                                            arrEditVendorPrices.push({
                                                                ID: data.ID,
                                                                NewPrice: val,
                                                                VendorID: sessionStorage.ViewID
                                                            });
                                                        } else {
                                                            arrEditVendorPrices[index].NewPrice = val;
                                                        }
                                                        console.log(arrEditVendorPrices)
                                                    })

                                        $(td).css('width', '200px').html(inp);
                                    }, className: 'text-right'
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return $.format.date(new Date(data.VendorPriceLastUpdated), 'MM-dd-yy hh:mm a');
                                    }, className: 'text-center'
                                },
                            ],
                            rowCallback: function (row, data) {
                                
                            },
                            "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                            "responsive": true,
                            "columnDefs": [{ "targets": 0, "orderable": false }],
                            "sDom": '<"top">rt<"bottom"ip><"clear">',
                            "initComplete": function () {
                                $('#tblVendorSKUs').removeClass('dataTable');
                            }
                            //"stateSave": true
                        })

                        $('#tblVendorSKUs tbody').unbind('click').on('click', 'tr', function () {
                            if (CStr2(tblVendorSKUs.row(this).data()) != '') {
                                //do nothing for now
                            }
                        });

                        $('#btnCopyVendorSKUs').unbind('click').click(function () {
                            var msg = 'VENDOR PRICE OF ' + $('#txtProspectCompanyName').val() + '\n\n';
                            msg += 'ITEMS: \n';
                            for (var i = 0; i < pRows.length; i++) {
                                msg += '\n' + (i + 1) + '. \n';
                                msg += 'Product No.: ' + pRows[i].ProductNo + ' \n';
                                msg += 'Brand: ' + pRows[i].Brand + ' \n';
                                msg += 'Product Description: ' + pRows[i].ProductNameOnly + ' \n';
                                msg += 'Price: ' + AccountingFormat(pRows[i].VendorPrice) + ' \n';
                                msg += 'Price Last Updated: ' + $.format.date(new Date(pRows[i].VendorPriceLastUpdated), 'MM-dd-yy hh:mm a') + ' \n';
                            }

                            $('#txtVendorSKUCopyToPlainText').val(msg).select();
                            document.execCommand("copy");
                            WebToast('Copied to clipboard');
                        })
                    }
                })
            }

            function ClearBankInfoFields() {
                dbbankid = '';
                bankID = '';
                $('[name="butCusSupBank"]').text('Search Bank');
                $('[name="txtCusSupBranch"], [name="txtCusSupAccountName"], [name="txtCusSupAccountNumber"]').val('');
                $('[name="cbCusSupAccountyType"]').val('cheque');
                $('[name="chkDeactivated"]').prop('checked', 0);
            }

            var tableSupplierCanvassForm;
            function GetSupplierCanvassForm() {
                if (sessionStorage.LibraryName == 'Customer / Client List') {
                    return;
                }

                AjaxSend({
                    url: 'OrganizationShort/GetSupplierCanvassForm',
                    data: { SupplierID: sessionStorage.ViewID },
                    success: function (e) {
                        var vRows = e.Results.Rows;
                        var ctr = 0;

                        $('#tbodySupplierCanvassForm').empty()

                        if ($.fn.DataTable.isDataTable("#tableSupplierCanvassForm")) {
                            //check if datatable is created 
                            $('#tableSupplierCanvassForm').DataTable().clear().destroy();
                        }

                        tableSupplierCanvassForm = $('#tableSupplierCanvassForm').DataTable({
                            retrieve: true,
                            "bdestroy": true,
                            "pageLength": 10,
                            "data": vRows,
                            "autoWidth": false,
                            "ordering": false,
                            "columns": [
                                {
                                    "data": function (data, type, full, meta) {
                                        ctr += 1;
                                        return ctr + '.';
                                    },
                                    "width": "30px"
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return data.CanvassName;
                                    }
                                },
                                {
                                    data: null,
                                    createdCell: function (td, cellData, rowData, row, col) {
                                        var b = $('<button data-role="none" class="btn custom-btn-theme-d btnCircle-xs">')
                                            .data('data', rowData)
                                            .append($('<span class="glyphicon glyphicon-remove">'))
                                            .click(function () {
                                                var data = $(this).data('data');
                                                
                                                NewMessageBox('Are you sure you want to remove canvass form <b>' + data.CanvassName + '</b>?', 'Message', [
                                                    {
                                                        Text: 'Yes', Click: function () {
                                                            AjaxSend({
                                                                url: 'OrganizationShort/RemoveSupplierCanvassForm',
                                                                data: { ID: data.ID },
                                                                success: function (e) {
                                                                    WebToast('Successfully removed canvass form <b>' + data.CanvassName + '</b>', 'top');
                                                                    GetSupplierCanvassForm();
                                                                }
                                                            })
                                                        }
                                                    }, { Text: 'No' }
                                                ])
                                            });
                                        $(td).html(b);
                                    },
                                    "width": "100px"
                                }
                            ],
                            "initComplete": function (row, data) {
                                $('#tableSupplierCanvassForm').removeClass('dataTable');//.table('refresh');
                            },
                            "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                            "responsive": true,
                            "columnDefs": [{ "orderable": false }],
                            "sDom": '<"top">rt<"bottom"ip><"clear">'
                            //"stateSave": true
                        })
                    }
                })
            }

            var tableAttachments;
            function GetFiles() {
                if ((sessionStorage.ViewID || sessionStorage.ClientInfo) == undefined) {
                    return;
                }

                AjaxSend({
                    url: 'OrganizationShort/GetFiles',
                    data: { ID: (sessionStorage.ViewID || JSON.parse(sessionStorage.ClientInfo).ID) },
                    success: function (e) {
                        var vRows = e.Results.Rows;
                        var ctr = 0;

                        $('#tbodyAttachments').empty()

                        if ($.fn.DataTable.isDataTable("#tableAttachments")) {
                            //check if datatable is created 
                            $('#tableAttachments').DataTable().clear().destroy();
                        }

                        tableAttachments = $('#tableAttachments').DataTable({
                            retrieve: true,
                            "bdestroy": true,
                            "pageLength": 20,
                            "data": vRows,
                            "autoWidth": false,
                            "ordering": false,
                            "columns": [
                                {
                                    "data": function (data, type, full, meta) {
                                        ctr += 1;
                                        return ctr + '.';
                                    },
                                    "width": "30px"
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return data.FileName;
                                    }
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return $.format.date(new Date(data.LastUpdated), 'MM-dd-yyyy hh:mm a');
                                    },
                                    "className": "dt-center"
                                },
                                {
                                    data: null,
                                    createdCell: function (td, cellData, rowData, row, col) {
                                        var b = $('<button data-role="none" class="btn btn-dark">')
                                            .text('Download')
                                            .click(function () {
                                                $.fileDownload(MergeUrlParameters({
                                                    OrgID: sessionStorage.OrgID,
                                                    ClientID: sessionStorage.ViewID,
                                                    FileName: rowData.FileName,
                                                    DownloadFileName: rowData.FileName
                                                }, '/OrganizationShort/DownloadFile'), {
                                                    prepareCallback: function () {
                                                        Bs.Loading('show', 'Preparing your file. Please wait ...');
                                                    },
                                                    successCallback: function () {
                                                        Bs.Loading('hide');
                                                    },
                                                    failCallback: function (responseHtml, url, error) {
                                                        var res = JSON.parse(responseHtml).d.Results;
                                                        NewMessageBox(res, 'Message');
                                                        Bs.Loading('hide');
                                                    }
                                                });
                                            });
                                        $(td).html(b);
                                    },
                                    "width": "100px"
                                }
                            ],
                            "initComplete": function (row, data) {
                                $('#tableAttachments').removeClass('dataTable');//.table('refresh');
                            },
                            "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                            "responsive": true,
                            "columnDefs": [{ "orderable": false }],
                            "sDom": '<"top">rt<"bottom"ip><"clear">'
                            //"stateSave": true
                        })
                    }
                })
            }

            function GetGeneratedPublicUrl() {
                if (sessionStorage.LibraryName == 'Customer / Client List') {
                    $('.lblOrgName').text(JSON.parse(sessionStorage.ClientInfo).ClientName);

                    if (JSON.parse(sessionStorage.ClientInfo).ProspectType == 'Org') {
                        $('#lblEcommerceIntro').html('Please click the button below to generate an online store account invite for your customer’s account administrator.');
                    } else {
                        $('#lblEcommerceIntro').html('Please click the button below to generate an online store account invite for your customer’s account administrator. This is also useful when your customer’s online store administrator has forgotten his/her sign in credentials. Otherwise, please do not generate a URL link.');
                    }
                } else {
                    $('#lblEcommerceIntro').html('Please click the button below to generate a Vendor Portal invite for your supplier’s account administrator.');
                }

                if ((sessionStorage.ViewID || sessionStorage.ClientInfo) == undefined) {
                    return;
                }

                AjaxSend({
                    url: 'OrganizationShort/GetGeneratedPublicUrl',
                    data: {
                        ID: (sessionStorage.ViewID || JSON.parse(sessionStorage.ClientInfo).ID),
                        Module: (sessionStorage.LibraryName == 'Customer / Client List' ? 'customer' : 'supplier')
                    }, success: function (e) {
                        var vRows = e.Results.Rows;
                        var tbodyGeneratedUrl = $('#tbodyGeneratedUrl');

                        tbodyGeneratedUrl.empty();                       

                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                var url;

                                if (sessionStorage.LibraryName == 'Customer / Client List') {
                                    url = location.origin + '/Ecommerce/PreCustomer?id=' + vRows[i].Link + '&mod=Customer' + '&type=' + JSON.parse(sessionStorage.ClientInfo).ProspectType;
                                } else {
                                    url = location.origin + '/Ecommerce/PreSupplier?id=' + vRows[i].Link + '&mod=Supplier' + '&type=Org';
                                }

                                tbodyGeneratedUrl.append($('<tr>')
                                        .append($('<td>').text($.format.date(new Date(vRows[i].DateCreated), 'MM-dd-yyyy hh:mm a')))
                                        .append($('<td>').append($('<input type"text" id="' + vRows[i].ID + '" readonly style="border:none; width:100%; outline:none; background-color:rgba(0, 0, 0, 0);' + (CStr2(vRows[i].DateEdited) == '' ? '' : 'text-decoration:line-through;') + '">').val(url)))
                                        .append($('<td>').text((CStr2(vRows[i].DateEdited) == '' ? '' : $.format.date(new Date(vRows[i].DateEdited), 'MM-dd-yyyy hh:mm a'))))
                                        .append($('<td class="text-center">').append((CStr2(vRows[i].DateEdited) != '' ? '' : $('<button data-role="none" class="btn btn-dark">').text('Copy URL Link')
                                                    .data('data', vRows[i])
                                                    .click(function () {
                                                        CopyToClipboard('#' + $(this).data('data').ID);
                                                    })
                                                )
                                            )
                                        )
                                    )
                            }
                        } else {
                            tbodyGeneratedUrl.append($('<tr>')
                                        .append($('<td colspan="7" class="txt-center">').text('No results found'))
                                    )
                        }
                    }
                })
            }

            function ActionForCustomerRecords(IsDeactivate) {
                NewMessageBox('Are you sure you want to '+ (IsDeactivate ? 'deactivate' : 'reactivate') +' all customer records?', 'Message', [
                    {
                        Text: 'Yes',
                        Click: function () {
                            $('#popConfirmCustomerDeactivation').modal('show');
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

                for (var i = 0; i < e.length; i++){
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

                var pLV = $('#lstOutletEmail'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrOutletEmailInfoList.length; i++) {
                    if (arrOutletEmailInfoList[i].EmailState != "Remove") {
                        isEmpty = false;
                        arrOutletEmailInfoList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .data(arrOutletEmailInfoList[i])
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .data(arrOutletEmailInfoList[i])
                                                .html($.trim(arrOutletEmailInfoList[i].EmailAdd) +
                                                    "<br>" + $.trim(arrOutletEmailInfoList[i].EmailType)
                                                )
                                                .click(function (e) {
                                                    EditOutletEmailInfo($(this).data().arrindex);
                                                })
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popOutletEmailInfo').modal('hide');
                $('#popCompanyOutlet').modal('show');
            }

            function EditOutletEmailInfo(arrindex) {
                $('#popCompanyOutlet').modal('hide');
                $('#popOutletEmailInfo').modal('show');
                OutletEmailRankNo = arrOutletEmailInfoList[arrindex].EmailRankNo;
                OutletEmailState = "Edit";
                sessionStorage["Emailindex"] = arrindex;
                sessionStorage["EmailIsNew"] = arrOutletEmailInfoList[arrindex].IsNew;
                sessionStorage["EmailState"] = arrOutletEmailInfoList[arrindex].EmailState;
                $('#txContacttPersonEmailID').val(arrOutletEmailInfoList[arrindex].EmailAddID == undefined ? "" : arrOutletEmailInfoList[arrindex].EmailAddID);
                $('#txtOutletEmailAdd').val(arrOutletEmailInfoList[arrindex].EmailAdd);
                $('#selOutletEmailType').val(arrOutletEmailInfoList[arrindex].EmailType);//.selectmenu('refresh');
            }

            function ClearOutletEmailInfoFields() {
                $('#txtOutletEmailID').val('');
                $('#txtOutletEmailAdd').val('');
                $('#selOutletEmailType').val('');//.selectmenu('refresh');
                $('#divOutletEmailHome').pageshow();
            }

            function AddOutletMobileInfo() {

                var pLV = $('#lstOutletMobile'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrOutletMobileInfoList.length; i++) {
                    if (arrOutletMobileInfoList[i].MobileState != "Remove") {
                        isEmpty = false;
                        arrOutletMobileInfoList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .data(arrOutletMobileInfoList[i])
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
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
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popOutletMobileInfo').modal('hide');
                $('#popCompanyOutlet').modal('show');
            }

            function EditOutletMobileInfo(arrindex) {
                $('#popCompanyOutlet').modal('hide');
                $('#popOutletMobileInfo').modal('show');
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
                $('#selOutletMobileType').val(arrOutletMobileInfoList[arrindex].ContactType);//.selectmenu('refresh');
            }

            function ClearOutletMobileInfo() {
                $('#txtOutletMobileID').val('');
                $('#txtOutletTelCoPrefixID').val('');
                $('#txtOutletMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtOutletMobileCountryCode').text('PHL - 63');
                $('#txtOutletTelCode').text('Select');
                $('#txtOutletMobileNo').val('');
                $('#selOutletMobileType').val('');//.selectmenu('refresh');
                $('#divOutletMobileHome').pageshow();
            }

            function AddOutletTelInfo() {

                var pLV = $('#lstOutletTelephone'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrOutletTelInfoList.length; i++) {
                    if (arrOutletTelInfoList[i].TelState != "Remove") {
                        isEmpty = false;
                        arrOutletTelInfoList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .data(arrOutletTelInfoList[i])
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
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
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }

                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popOutletTelInfo').modal('hide');
                $('#popCompanyOutlet').modal('show');
            }

            function EditOutletTelInfo(arrindex) {
                $('#popCompanyOutlet').modal('hide');
                $('#popOutletTelInfo').modal('show');

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
                $('#selOutletTelType').val(arrOutletTelInfoList[arrindex].ContactType);//.selectmenu('refresh');
            }

            function ClearOutletTelInfo() {
                $('#txtOutletTelID').val('');
                $('#txtOutletTelAreaCodeID').val('');
                $('#txtOutletTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtOutletCountryCode').text('PHL - 63');
                $('#txtOutletAreaCode').text('Select');
                $('#txtOutletTelephoneNum').val('');
                $('#txtOutletTelephoneExt').val('');
                $('#selOutletTelType').val('');//.selectmenu('refresh');
                $('#divOutletTel').pageshow();
            }

            function DisplayCompanyOutletDepartment() {
                var lstOutletDepartment = $('#lstOutletDepartment'), isEmpty = true;
                lstOutletDepartment.empty();

                for (var i = 0; i < arrCompanyOutletDepartment.length; i++) {
                    arrCompanyOutletDepartment[i].Index = i;
                    if ($('#selCompanyOutletDeptStatus').val() != '') {
                        if (CBool($('#selCompanyOutletDeptStatus').val()) != arrCompanyOutletDepartment[i].IsActive) {
                            continue;
                        }
                    }

                    if (arrCompanyOutletDepartment[i].State != "remove") {
                        isEmpty = false;
                        lstOutletDepartment.append($('<a href="#" class="list-group-item list-group-item-action">')                                    
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">')
                                        .html(arrCompanyOutletDepartment[i].Department + ' ')
                                        .append($('<span style="color:#949494;font-weight:normal;">')
                                            .text((arrCompanyOutletDepartment[i].State == "add" ? '(New)' :
                                                    (arrCompanyOutletDepartment[i].State == 'edit' ? (CBool(arrCompanyOutletDepartment[i].IsActive) ? '(Active)' : '(Inactive)') : '(' + arrCompanyOutletDepartment[i].State + ')')
                                                )
                                            )
                                        )
                                        .data('data', arrCompanyOutletDepartment[i])
                                        .click(function () {
                                            var data = $(this).data('data');
                                            CompanyOutletDepartmentState = "edit";
                                            sessionStorage.DepartmentIndex = data.Index;
                                            $('#popCompanyOutlet').modal('hide');
                                            $('#popCompanyOutletDepartment').modal('show');
                                            $('#txtCompanyOutletDepartment').val(data.Department).focus();
                                            $('#txtCompanyOutletDepartment3rdPartyID').val(CStr2(data.ThirdPartyID));
                                        })
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash" aria-hidden="true">'))
                                            .data('data', arrCompanyOutletDepartment[i])
                                            .data('dataDepartment', arrCompanyOutletDepartment)
                                            //.text(arrCompanyOutletDepartment[i].Department)
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

                                                NewMessageBox('Are you sure you want to <b>'+ action +'</b> this record <b>"' + data.Department + '"</b>?', 'Message', [
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
                                                            ], {
                                                                HidePrevPopup: false,
                                                                Close: function () {
                                                                    $('#popCompanyOutlet').modal('show');
                                                                }
                                                            })

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
                                    )
                                )
                            )                            
                        )
                    }
                }

                if (isEmpty) {
                    lstOutletDepartment.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
            }

            function DisplayCompanyOutlet() {
                var lstCompanyOutlet = $('#lstCompanyOutlet'), isEmpty = true;

                lstCompanyOutlet.empty();

                for (var i = 0; i < arrCompanyOutlet.length; i++) {
                    arrCompanyOutlet[i].Index = i;
                    if ($('#selCompanyOutletStatus').val() != '') {
                        if (CBool($('#selCompanyOutletStatus').val()) != arrCompanyOutlet[i].IsActive) {
                            continue;
                        }
                    }

                    if (arrCompanyOutlet[i].State != 'remove') {
                        isEmpty = false;
                        lstCompanyOutlet.append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .data(arrCompanyOutlet[i])
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .html(arrCompanyOutlet[i].FullAddress +  ' ')
                                                .append($('<span style="color:#949494;font-weight:normal;">')
                                                    .text((arrCompanyOutlet[i].State == "add" ? '(New)' :
                                                            (arrCompanyOutlet[i].State == 'edit' ? (CBool(arrCompanyOutlet[i].IsActive) ? '(Active)' : '(Inactive)') : '(' + arrCompanyOutlet[i].State + ')')
                                                        )
                                                    )
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
                                                    $('#popCompanyOutlet').modal('show');
                                                    $('#tabOutletInfo').opentabmenu();
                                                    $('#txtCompanyOutlet').val(data.Name).focus();
                                                    $('#txtOutletAddressID').val(data.AddressID);
                                                    $('#txtCompanyOutlet3rdPartyID').val(CStr2(data.ThirdPartyID));
                                                    $('#txtOutletHouseBuildingNo').val(data.HouseNo);
                                                    $('#txtOutletStreet').val(data.Street);
                                                    $('#selOutletCity').val(data.City);//.selectmenu('refresh');
                                                    $('#selOutletCountry').val(data.Country);//.selectmenu('refresh');
                                                    AddOutletTelInfo();
                                                    AddOutletMobileInfo();
                                                    AddOutletEmailInfo();
                                                    DisplayCompanyOutletDepartment();
                                                })
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-gear" aria-hidden="true">'))
                                                    .data('data', arrCompanyOutlet[i])
                                                    .click(function (e) {
                                                        e.stopPropagation();
                                                        e.preventDefault();

                                                        var data = $(this).data('data');
                                                        var action;

                                                        if(data.State == "add"){
                                                            action = "delete";
                                                        } else {
                                                            if (CBool(data.IsActive)) {
                                                                action = (data.State == "Deactivate" ? "activate" : "deactivate");
                                                            } else {
                                                                action = "activate";
                                                            }
                                                        }

                                                        NewMessageBox('Are you sure you want to <b>'+ action +'</b> this record <b>"' + data.Name + '"</b>?', 'Message', [
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
                                            )
                                        )
                                    )
                        )
                    }                    
                }
                if (isEmpty) {
                    lstCompanyOutlet.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
            }

            function DisplayRevPriceLevels() {
                var lstOrgPriceLevel = $('#lstOrgPriceLevel'), isEmpty = true;

                lstOrgPriceLevel.empty();

                for (var i = 0; i < arrRevPriceLevels.length; i++) {
                    arrRevPriceLevels[i].index = i;
                    if (arrRevPriceLevels[i].State != 'remove') {
                        isEmpty = false;
                        lstOrgPriceLevel.append($('<a href="#" class="list-group-item list-group-item-action">')
                            .data(arrRevPriceLevels[i])
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">')
                                        .html($.trim(arrRevPriceLevels[i].PriceLevel))
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash" aria-hidden="true">'))
                                            .data(arrRevPriceLevels[i])
                                            .click(function (e) {
                                                var data = $(this).data();
                                                if(data.State == 'add'){
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
                                )
                            )
                        )
                    }
                }

                if (isEmpty) {
                    lstOrgPriceLevel.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
            }

            function NewCustomerOptions(details) {
                isAddNewProspect = true;
                isCreateFromExistingRecord = false;
                $('#panAddCustomerLibrary').modal('hide');
                AddBreadCrumb({
                    text: details,
                    Click: function () {
                        if (location.pathname.toLowerCase() == '/erp') {
                            $('#divSystemSettings').find('#pageActions, #pageProfile, #pageProfileOrg').pageshow();
                        } else {
                            $('#pageActions, #pageProfile, #pageProfileOrg').pageshow();
                        }
                        $('#tabRevenue').appendTo($('#divCompanyProfile').find('.tab-content'));
                    }
                }, '.ItemsBreadCrumbs')
            }

            function CustomerSearch() {
                AjaxSend({
                    url: 'Browser/LibraryOrganizationAndPerson',
                    data: { keywords: $.trim($('#txtCustomerSearch').val()) },
                    success: function (vData) {
                        $('#CustomerlstSearchFound').empty();
                        $('#divCustomerNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            var mDate = new Date();
                            var Products = [];

                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#CustomerlstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .text(vData.Results.Rows[p].CompanyName)
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            SaveSelectedCustomerViaSearch($(this).data('data'));
                                            //isCreateFromExistingRecord = true;
                                            //isAddNewProspect = true;
                                            //if (CBool($(this).data('data').IsOrganization)) {
                                            //    ClearOrgProspectVariablesFields();
                                            //    $('#txtExistingCustomerOrgID').val($(this).data('data').ID);
                                            //    $('#txtProspectCompanyName').val($(this).data('data').CompanyName);
                                            //    $('#txtProspectOrgTin').val($(this).data('data').TinNumber);
                                            //    $('#pageActions, #pageProfile, #pageProfileOrg').pageshow();
                                            //    $('#tabRevenue').appendTo($('#divCompanyProfile').find('.tab-content'));
                                            //    $('#divTabCompany > ul > li:first').click();
                                            //} else {
                                            //    ClearPersonProspectVariablesFields();
                                            //    $('#txtExistingCustomerPersonID').val($(this).data('data').ID);
                                            //    $('#txtProspectPersonFirstName').val($(this).data('data').FirstName);
                                            //    $('#txtProspectPersonMiddleName').val($(this).data('data').MiddleName);
                                            //    $('#txtProspectPersonLastName').val($(this).data('data').Surname);
                                            //    $('#pageActions, #pageProfile, #pageProfilePeople').pageshow();
                                            //    $('#divTabPerson > ul > li:first').click();
                                            //}

                                            $('#panAddCustomerLibrary').modal('hide');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#divCustomerNoResultsFound').show();
                        }

                        $('#btnAddToCustomerLibrary').show();
                        $('#CustomerlstSearchFound');//.listview('refresh');
                        PopupReposition();
                    }
                });
            }

            function SaveSelectedCustomerViaSearch(selectedData) {
                NewMessageBox('Are you sure you want to add <b>' + selectedData.CompanyName + '</b> as your customer?', 'Message', [
                    {
                        Text: 'Yes', Click: function () {
                            NewMessageBox('Please wait while adding selected supplier ...', 'Message');

                            AjaxSend({
                                url: 'OrganizationShort/GetDataOfSelectedPublicOrgPerson',
                                data: selectedData,
                                success: function (e) {
                                    if (CBool(selectedData.IsOrganization)) {
                                        var orgdata = e.Results.Rows[0];

                                        var pData = {
                                            isCreateFromExistingRecord: 1,
                                            Telphone: [],
                                            Mobile: [],
                                            Email: [],
                                            Website: [],
                                            Outlet: [],
                                            ContactPerson: [],
                                            ContactPersonTelphone: [],
                                            ContactPersonMobile: [],
                                            ContactPersonEmail: [],
                                            OrgAffiliation: [],
                                            OrgAffiliatedPerson: [],
                                            CompanyID: CStr2(orgdata.ID),
                                            ExistingCustomerID: CStr2(selectedData.ID),
                                            CompanyName: $.trim(CStr2(orgdata.CompanyName)),
                                            Tin: CStr2(orgdata.TinNumber),
                                            OrgID: sessionStorage.OrgID,
                                            DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a'),
                                            Status: 'Active', //default to active x-CStr2(orgdata.IsActive),
                                            IsCustomerSettings: (sessionStorage.LibraryName == 'Customer / Client List' ? true : false),
                                            PriceLevel: []
                                        }

                                        if (sessionStorage.LibraryName.indexOf('Customer') != -1)
                                            pData.SaveAsCustomer = 1;

                                        if (sessionStorage.LibraryName.indexOf('Supplier') != -1) {
                                            pData.SaveAsSupplier = 1;
                                            pData.SupplierInfo = {
                                                OrgSupplierID: "",
                                                SupplierID: selectedData.ID,
                                                ActivationDate: $.format.date(new Date(), "MM/dd/yyyy"),
                                                IsDeactivated: 0,
                                                DeactivationDate: '//',
                                                DeactivationReason: '',
                                                Comments: '',
                                                IsBan: 0,
                                                CreditTermsID: null,
                                                CreditLimit: CDec2(0),
                                                CreditBegBalance: CDec2(0),
                                                CreditBalanceAsOf: null
                                            };
                                        }


                                        AjaxSend({
                                            url: 'OrganizationShort/SaveNewOrgProspect',
                                            data: pData,
                                            success: function (vData) {
                                                if (IsGuid(vData.Results)) {
                                                    NewMessageBoxClose();
                                                    WebToast('Successfully added', 'top');
                                                    if (sessionStorage.LibraryName == 'Customer / Client List') {
                                                        datatable.draw(); // refresh datatable after saving
                                                    } else {
                                                        LibraryTable.SupplierList();
                                                    }
                                                } else {
                                                    NewMessageBox(vData.Results, 'Message');
                                                }
                                            }
                                        })
                                    } else {
                                        var ppoldata = e.Results.Rows[0];
                                        var pData = {
                                            isCreateFromExistingRecord: 1,
                                            Telphone: [],
                                            Mobile: [],
                                            Email: [],
                                            OrgAffiliation: [],
                                            PersonID: ppoldata.ID,
                                            ExistingCustomerID: ppoldata.ID,
                                            FirstName: ppoldata.PersonFirstName,
                                            MiddleName: ppoldata.PersonMiddleName,
                                            LastName: ppoldata.PersonSurname,
                                            OrgID: sessionStorage.OrgID,
                                            DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a'),
                                            Status: 'Active',
                                            Elementary: [],
                                            HighSchool: [],
                                            College: [],
                                            PriceLevel: [],
                                            IsCustomerSettings: (sessionStorage.LibraryName == 'Customer / Client List' ? true : false)
                                        }
                                        //console.log(pData);
                                        AjaxSend({
                                            url: 'OrganizationShort/SaveNewPersonProspect',
                                            data: pData,
                                            success: function (vData) {
                                                if (IsGuid(vData.Results)) {
                                                    NewMessageBoxClose();
                                                    WebToast('Successfully added', 'top');
                                                    if (sessionStorage.LibraryName == 'Customer / Client List') {
                                                        datatable.draw(); // refresh datatable after saving
                                                    } else {
                                                        LibraryTable.SupplierList();
                                                    }
                                                } else {
                                                    NewMessageBox(vData.Results, 'Message');
                                                }
                                            }
                                        })
                                    }
                                }
                            })
                        }
                    }, { Text: 'No' }
                ])                
            }

            function DisplayEducation() {
                var list, arr = [], isEmpty = true;
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
                    arr[i].index = i;
                    if (arr[i].State != 'remove') {
                        isEmpty = false;
                        list.append($('<a href="#" class="list-group-item list-group-item-action">')
                            .data(arr[i])
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">')
                                        .html($.trim(arr[i].Name))
                                        .data(arr[i])
                                        .click(function (e) {
                                            EditEducation($(this).data());
                                        })
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash" aria-hidden="true">'))
                                            .data(arr[i])
                                            .data('allarr', arr)
                                            .click(function (e) {
                                                var data = $(this).data(), arr;

                                                if (data.Level == 'elem') {
                                                    arr = arrElementary;
                                                } else if (data.Level == 'hs') {
                                                    arr = arrHighSchool;
                                                } else if (data.Level == 'college') {
                                                    arr = arrCollege;
                                                }

                                                if (data.State == 'add') {
                                                    arr = $.grep(arr, function (e) {
                                                        return e.ID != data.ID;
                                                    })
                                                } else {
                                                    arr[data.index].State = 'remove';
                                                }

                                                academicLvl = $(this).data().Level;
                                                DisplayEducation();
                                            })
                                        )
                                    )
                                )
                            )
                        )
                    }
                }

                if (isEmpty) {
                    list.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popEducation').modal('hide');
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
                $('#popEducation').modal('show');
                $('#txtEdeHiID').val('');
                $('#txtEdeHiSchoolName').val('').focus();
                $('#txtEdeHiYearStarted').val('');
                $('#txtEdeHiYearGraduated').val('');
            }

            function OnKeySearch(data) {
                if (timeOut) clearTimeout(timeOut);

                var search = data.elem;

                if (data.before != undefined) {
                    data.before();
                }

                if ($.trim(search.val()) != "") {

                    if (data.during != undefined) {
                        data.during();
                    }

                    timeOut = setTimeout(function () {
                        // Cancel the last request if valid
                        if (myXHR) myXHR.abort();

                        myXHR = AjaxSend({
                            url: data.url,
                            data: { keywords: $.trim(search.val().replace(/-/g, '')), OrgID: sessionStorage.OrgID },
                            success: function (vData) {
                                var vRows = vData.Results.Rows;
                                data.after(vRows);
                            }
                        })
                    }, 500);
                }
            }

            function AddFilter() {
                var category = $('#selCategory');
                var businessindustry = $('#selFilterByBusinessIndustry');
                var city = $('#selFilterByCity');
                var companysize = $('#selFilterCompanySize');
                var scope = $('#selScope');
                var condition = $('#selCondition');
                var conditionval = $('#txtConditionValue');

                if (arrFilter.length == 5) {
                    WebToast('Maximum of 5 filters only')
                }
                else {
                    if ($.trim(category.val()) != "") {
                        if ($.trim(category.val()) == "Business Industry") {
                            //if (businessindustry.val() == "") {
                            //    $('#msgBusinessIndustry').pageshow();
                            //    return;
                            //}

                            arrFilter.push({
                                category: category.val(),
                                value: businessindustry.val(),
                                text: 'Industry: ' + businessindustry.find('option:selected').text(),
                                state: 'new'
                            })
                        }
                        else if ($.trim(category.val()) == "City") {
                            //if (city.val() == "") {
                            //    $('#msgCity').pageshow();
                            //    return;
                            //}

                            arrFilter.push({
                                category: category.val(),
                                value: city.val(),
                                text: 'City: ' + city.find('option:selected').text(),
                                state: 'new'
                            })
                        }
                        else if ($.trim(category.val()) == "Company Size") {
                            var regExp = /\(([^)]+)\)/;
                            if (companysize.val() == "ScopeOfOperations") {
                                if (scope.val() == "") {
                                    $('#msgScope').pageshow();
                                    return;
                                }
                            }
                            else if (conditionval.val() == "") {
                                $('#msgConditionValue').pageshow();
                                return;
                            }
                            arrFilter.push({
                                category: category.val(),
                                companysize: companysize.val(),
                                value: (companysize.val() == "ScopeOfOperations" ? scope.val() : condition.val() + '-' + conditionval.val()),
                                text: (companysize.val() == "ScopeOfOperations" ? scope.find('option:selected').text() : companysize.find('option:selected').text() + ' ' + condition.find('option:selected').text().split(')')[1] + ' ' + conditionval.val()),
                                state: 'new'
                            })
                        }
                    }
                    else {
                        $('#msgCategory').pageshow();
                        return;
                    }
                    $('#popFilter').modal('hide');
                    $('#selCategory').val('').change();//.selectmenu('refresh')
                    DisplayAdvanceFilter();
                    FilteredText();
                }
            }

            function FilteredText() {
                filterVal = '';
                for (var i = 0; i < arrFilter.length; i++) {
                    filterVal = filterVal + (i > 0 ? '||' : ''); // for splitting purposes only
                    if (arrFilter[i].category == "Company Size") {
                        filterVal = filterVal + arrFilter[i].category + '\/' + arrFilter[i].companysize + '\/' + arrFilter[i].value
                    }
                    else {
                        filterVal = filterVal + arrFilter[i].category + '\/' + arrFilter[i].value
                    }
                }
                datatable.columns(1).search(filterVal).draw();
                return filterVal;
            }

            function DisplayAdvanceFilter() {
                var divAdvanceFilterList = $('#divAdvanceFilterList');
                divAdvanceFilterList.empty();

                for (var i = 0; i < arrFilter.length; i++) {
                    divAdvanceFilterList.append($('<div class="chip-plain">')
                            .append($('<div>')
                                .attr('title', arrFilter[i].text)
                                .text(arrFilter[i].text)
                            )
                            .append($('<span class="closebtn">')
                                .attr('i', i)
                                .text('×')
                                .click(function () {
                                    arrFilter.splice($(this).attr('i'), 1);
                                    DisplayAdvanceFilter();
                                    FilteredText();
                                })
                            )
                        )
                }

                if (arrFilter.length == 0) {
                    $('#divAdvanceFilter').hide();
                } else {
                    $('#divAdvanceFilter').show();
                }
            }

            function EditingMode() {
                if (isEditingANote) {
                    NewMessageBox('Do you want to save your notes?', 'Message', [
                        {
                            Text: 'Yes',
                            Click: function () {
                                SaveTakeANote(sessionStorage.StateNote, sessionStorage.StateNoteID, true);
                                isEditingANote = false;
                            }
                        },
                        {
                            Text: 'No',
                            Click: function () {
                                isEditingANote = false;
                            }
                        }
                    ])
                }

                if (isEditingCallNote) {
                    NewMessageBox('Do you want to save your call notes?', 'Message', [
                        {
                            Text: 'Yes',
                            Click: function () {
                                $('#btnSaveColdCall').click();
                            }
                        },
                        {
                            Text: 'Save as Draft',
                            Click: function () {
                                isCallNoteSaveAsDraft = true;
                                isEditingCallNote = false;
                            }
                        },
                        {
                            Text: 'No',
                            Click: function () {
                                isEditingCallNote = false;
                            }
                        }
                    ])
                }

                if (isEditingProfile) {
                    NewMessageBox('Do you want to save changes in profile?', 'Message', [
                        {
                            Text: 'Yes',
                            Click: function () {
                                if (JSON.parse(sessionStorage.ClientInfo).ProspectType == "Org") {
                                    SaveNewOrgProspect(true);
                                } else {
                                    SaveNewPersonProspect(true);
                                }

                                isEditingProfile = false;
                            }
                        },
                        {
                            Text: 'No',
                            Click: function () {
                                isEditingProfile = false;
                            }
                        }
                    ])
                }
            }

            function DisplayListOfEvents(vRows) {
                var lstEvents = $('#lstEvents');

                lstEvents.empty();
                if (vRows.length > 0) {
                    for (var i = 0 ; i < vRows.length; i++) {
                        vRows[i].arrindex = i;
                        var defaultColor = {};
                        var dateFrom = new Date($.format.date(new Date(vRows[i].Detail.SchedActionDateFrom), 'MMM dd, yyyy'));
                        var dateTo = new Date($.format.date(new Date(vRows[i].Detail.SchedActionDateTo), 'MMM dd, yyyy'));
                        var datecompare = dateTo <= dateFrom;
                        var time;

                        if (vRows[i].Detail.SchedActionName == "Meeting") {
                            defaultColor = {
                                'background-color': '#FFC107',
                                'border': '1px solid #FF9800',
                                'color': '#FFF'
                            }
                        }
                        else if (vRows[i].Detail.SchedActionName == "Call") {
                            defaultColor = {
                                'background-color': '#5A6BC5',
                                'border': '1px solid #1D2E90',
                                'color': '#FFF'
                            }
                        }
                        else if (vRows[i].Detail.SchedActionName == "Email") {
                            defaultColor = {
                                'background-color': '#2196F3',
                                'border': '1px solid #2A5E75',
                                'color': '#FFF'
                            }
                        }
                        else if (vRows[i].Detail.SchedActionName == "SMS") {
                            defaultColor = {
                                'background-color': '#4CAF50',
                                'border': '1px solid #233C24',
                                'color': '#FFF'
                            }
                        }

                        if (datecompare) {
                            time = $.format.date(new Date(vRows[i].Detail.SchedActionDateFrom), 'MMM dd, yyyy hh:mm a') + ' to ' + $.format.date(new Date(vRows[i].Detail.SchedActionDateTo), ' hh:mm a')
                        }
                        else {
                            if (CBool(vRows[i].Detail.IsActionAllDay)) {
                                time = '(All Day) ' + $.format.date(new Date(vRows[i].Detail.SchedActionDateFrom), 'MMM dd, yyyy') + ' to ' + $.format.date(new Date(vRows[i].Detail.SchedActionDateTo), 'MMM dd, yyyy')
                            }
                            else {
                                time = $.format.date(new Date(vRows[i].Detail.SchedActionDateFrom), 'MMM dd, yyyy hh:mm a') + ' to ' + $.format.date(new Date(vRows[i].Detail.SchedActionDateTo), 'MMM dd, yyyy hh:mm a')
                            }
                        }

                        lstEvents.append($('<li>')
                                .attr('e', vRows[i].arrindex)
                                .append($('<div class="todoContainer">')
                                    .css('cursor', 'pointer')
                                    .css('display', 'table')
                                    .append($('<div style="display:table-cell; width:50px; vertical-align:middle; text-align:-webkit-center;">')
                                        .data(vRows[i].Detail)
                                        .append($('<span>')
                                            .addClass('custom-badge')
                                            .css(defaultColor)
                                            .text(vRows[i].Detail.SchedActionName.slice(0, 1))
                                        )
                                    )
                                    .append($('<div style="display:table-cell; width:auto; overflow:hidden; padding:8px; word-break:break-all; vertical-align:top;">')
                                        .data(vRows[i].Detail)
                                        .html('<b>' + vRows[i].Detail.Subject + '</b><br><br>' +
                                                time
                                            )
                                        .click(function () {
                                            EditEvent($(this))
                                            isFromListEvent = true;
                                            $('#pageNewSched').pageshow();
                                            sessionStorage.StateSched = 'edit';
                                        })
                                    )
                                    .append($('<div style="display:table-cell; width:50px; padding-bottom:10px; vertical-align:middle;">')
                                        .data(vRows[i].Detail)
                                        .append($('<button data-role="none" title="Set Date" class="btn custom-btn-theme-h btnCircle-sm btnShadow">')
                                            .append('<img alt="Remove" src="/Content/images/trash.png" />')
                                        )
                                        .click(function () {
                                            EditEvent($(this));
                                            sessionStorage.StateSched = 'remove';
                                            $('#btnSaveSchedAnAction').click();
                                        })
                                    )
                                )
                            )
                    }
                }
            }

            function EditEvent(dis) {
                if (CBool(dis.data('IsActionAllDay'))) {
                    $('#btnDateTimeFrom').parent().hide();
                    $('#btnDateTimeTo').parent().hide();
                }
                else {
                    $('#btnDateTimeFrom').parent().show();
                    $('#btnDateTimeTo').parent().show();
                }
                $('#txtSchedAnActionID').val(dis.data('ID'));
                $('#txtSchedAnActionSubject').val(dis.data('Subject'));
                $('#selSchedAnAction').val(dis.data('SchedActionName'));//.selectmenu('refresh');
                $('input[name="chbAllDay"]').prop('checked', CBool(dis.data('IsActionAllDay')));//.checkboxradio('refresh');
                $('#txtDateTimeFrom').val($.format.date(new Date(dis.data('SchedActionDateFrom')), 'MMMM dd, yyyy'));
                $('#btnDateTimeFrom').text($.format.date(new Date(dis.data('SchedActionDateFrom')), 'h:mm a'));//.button('refresh');
                $('#txtDateTimeTo').val($.format.date(new Date(dis.data('SchedActionDateTo')), 'MMMM dd, yyyy'));
                $('#btnDateTimeTo').text($.format.date(new Date(dis.data('SchedActionDateTo')), 'h:mm a'));//.button('refresh');
                $('#txtNextActionLocation').val(dis.data('Location'));
                $('#txtNextActionNotes').val(dis.data('Note')).height('100px');
                $('#selNextActionTeam').val(dis.data('SchedActionTeamID'));//.selectmenu('refresh');
            }

            function GetSchedule() {
                AjaxSend({
                    url: 'CRMDashboard/GetSchedule',
                    data: JSON.parse(sessionStorage.CurrentCalendarDate), //sessionStorage.CurrentCalendarDate from customcalendar.js
                    success: function (vData) {
                        var arrEvent = [];
                        var vRows = vData.Results.Rows;

                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                var dateFrom = new Date($.format.date(new Date(vRows[i].SchedActionDateFrom), 'MMM dd, yyyy'));
                                var dateTo = new Date($.format.date(new Date(vRows[i].SchedActionDateTo), 'MMM dd, yyyy'));
                                var datecompare = dateTo <= dateFrom;

                                if (datecompare) {
                                    arrEvent.push({
                                        Detail: vRows[i],
                                        eventName: vRows[i].Subject,
                                        eventWhen: $.format.date(new Date(dateFrom), 'MMMM dd, yyyy')
                                    })
                                }
                                else {
                                    while (dateFrom <= dateTo) {
                                        arrEvent.push({
                                            Detail: vRows[i],
                                            eventName: vRows[i].Subject,
                                            eventWhen: $.format.date(new Date(dateFrom), 'MMMM dd, yyyy')
                                        })
                                        dateFrom.setDate(dateFrom.getDate() + 1);
                                    }
                                }
                            }
                        }
                        $('#divCalendar').customcalendar(arrEvent);
                    }
                })
            }

            function SaveSchedAnAction() {
                var dateFrom, dateTo;
                if ($('input[name="chbAllDay"]').is(':checked')) {
                    dateFrom = "00:00 AM";
                    dateTo = "00:00 AM";
                }
                else {
                    dateFrom = $('#btnDateTimeFrom').text();
                    dateTo = $('#btnDateTimeTo').text();
                }

                var pData = {
                    OrgID: sessionStorage.OrgID,
                    SchedAnActionID: $.trim($('#txtSchedAnActionID').val()),
                    ClientID: JSON.parse(sessionStorage.ClientInfo).ID,
                    NextActionName: $('#selSchedAnAction option:selected').val(),
                    Subject: $.trim($('#txtSchedAnActionSubject').val()),
                    Location: $.trim($('#txtNextActionLocation').val()),
                    IsAllDay: $('input[name="chbAllDay"]').is(':checked'),
                    DateTimeFrom: $.trim($('#txtDateTimeFrom').val()) + ' ' + dateFrom,
                    DateTimeTo: $.trim($('#txtDateTimeTo').val()) + ' ' + dateTo,
                    Note: $.trim($('#txtNextActionNotes').val()),
                    TeamAssign: $('#selNextActionTeam option:selected').val(),
                    State: sessionStorage.StateSched,
                    Date: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a')
                }

                AjaxSend({
                    url: 'CRMDashboard/SaveSchedAnAction',
                    data: pData,
                    success: function (vData) {
                        if (vData.Results == "Saved") {
                            WebToast('Successfully Saved');
                            $('#pageSchedule').pageshow();
                            ClearSchedAnAction();
                            GetSchedule();
                        }
                        else {
                            NewMessageBox('ERROR: ' + vData.Results);
                        }
                    }
                })
            }

            function ClearSchedAnAction() {
                $('#txtSchedAnActionID').val('');
                $('#txtSchedAnActionSubject').val('');
                $('input[name="chbAllDay"]').prop('checked', false);//.checkboxradio('refresh');
                $('#txtDateTimeFrom').val(sessionStorage.CurrentCalendarSelectedDate);
                $('#btnDateTimeFrom').val('1:00 AM');//.button('refresh');
                $('#txtDateTimeTo').val(sessionStorage.CurrentCalendarSelectedDate);
                $('#btnDateTimeTo').val('2:00 AM');//.button('refresh');
                $('#txtNextActionLocation').val('');
                $('#txtNextActionNotes').val('').height('100px');
                $('#selNextActionTeam').val('');//.selectmenu('refresh');
            }

            function GetListOfTeam() {
                var selNextActionTeam = $('#selNextActionTeam');

                selNextActionTeam.empty();

                AjaxSend({
                    url: 'Browser/LibraryTeam',
                    success: function (vData) {
                        selNextActionTeam.append(new Option('Select Team', ''))

                        for (var i = 0; i < vData.Results.Rows.length; i++) {
                            selNextActionTeam.append(new Option(vData.Results.Rows[i].Name, vData.Results.Rows[i].ID))
                        }
                    }
                })
            }

            function GetTakeANote(id) {
                $('#pageListOfNote').pageshow();
                var lstTakenNotes = $('#lstTakenNotes');

                lstTakenNotes.empty().append('<li>Loading ...</li>')

                AjaxSend({
                    url: 'CRMDashboard/GetTakeANote',
                    data: { ClientID: id },
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        lstTakenNotes.empty()

                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                var newNote = vRows[i].Note;
                                if (newNote.length > 100) {
                                    vRows[i].isElippse = true;
                                    newNote = newNote.substr(0, 100) + ' ... <br> <a data-role="none" class="custom-link">(read more)</a>'
                                }
                                newNote = '<span style="float:right; font-size:12px; color:#757575;">' + $.format.date(new Date(vRows[i].LastUpdated), 'MMM dd, yyyy hh:mm a') + '</span><br><br>' + newNote

                                lstTakenNotes.append($('<li>')
                                    .append($('<div class="todoContainer" style="display:table; width:100%;">')
                                        .css('cursor', 'pointer')
                                        .append($('<div style="float:none; display:table-cell; width:auto; overflow:hidden; padding:8px; word-break:break-all;">')
                                            .data(vRows[i])
                                            .html(newNote)
                                            .click(function () {
                                                sessionStorage.StateNote = "edit";
                                                sessionStorage.StateNoteID = $(this).data().ID;
                                                $('#txtTakeANote').val($(this).data().Note);
                                                $('#pageAddNote').pageshow();
                                            })
                                        )
                                        .append($('<div style="padding:10px; display:table-cell; width:30px; vertical-align:middle;">')
                                            .data(vRows[i])
                                            .append($('<button data-role="none" title="Remove" class="btn custom-btn-theme-c btnCircle-xs btnShadow">')
                                                .append('<img alt="Remove" src="/Content/images/close-black.png" />')
                                            )
                                            .click(function () {
                                                SaveTakeANote('remove', $(this).data().ID);
                                            })
                                        )
                                    )
                                )
                            }
                        }
                        else {
                            lstTakenNotes.append('<li>No Notes Found</li>')
                        }
                    }
                })
            }

            function SaveTakeANote(state, noteid, autosave) {
                AjaxSend({
                    url: 'CRMDashboard/SaveTakeANote',
                    data: { ClientID: JSON.parse(sessionStorage.ClientInfo).ID, Note: ($('#txtTakeANote').val().replace(/[<>]/g, function (s) { return entities[s]; })).replace(/\r\n|\r|\n/g, "\n"), State: state, NoteID: noteid, Date: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a') },
                    success: function (vData) {
                        if (vData.Results == "Saved" && autosave == undefined) {
                            WebToast('Successfully Saved', 'top');
                            GetTakeANote(JSON.parse(sessionStorage.ClientInfo).ID);
                        }
                        else if (vData.Results != "Saved") {
                            NewMessageBox('ERROR: ' + vData.Results);
                        }
                        isEditingANote = false;
                    }
                })
            }

            function GetMobileNoBySelectedRecipient(e) {
                $('.txtActionClientName').text(e.ClientName);
                if (e.MobileNos != undefined) {
                    MakeArrayForSelectedRecipient(e);
                    DisplayMobileNoBySelectedRecipient(arrSMSRecipient);
                }

                if (e.ContactPerson != null) {
                    AjaxSend({
                        url: 'CRMDashboard/GetContactDetailsOrg',
                        data: e,
                        success: function (vData) {
                            var vRows = vData.ContactPerson;

                            if (vRows.length > 0) {
                                for (var m = 0; m < vRows.length; m++) {
                                    vRows[m].ClientName = vRows[m].FullName;
                                    vRows[m].ID = e.ID; // must retain the client id so that it will log to its own history
                                    if (vRows[m].MobileNos != null) {
                                        MakeArrayForSelectedRecipient(vRows[m])
                                    }
                                }
                            }
                            DisplayMobileNoBySelectedRecipient(arrSMSRecipient);
                        }
                    })
                }
            }

            function MakeArrayForSelectedRecipient(e) {
                var splitno = e.MobileNos.split(',');
                var arrmobileno = [];

                for (var i = 0; i < splitno.length; i++) {
                    arrmobileno.push(splitno[i])
                }

                arrSMSRecipient.push({
                    ID: e.ID,
                    Name: e.ClientName,
                    CPID: (CBool(e.IsCP) ? e.ContactPersonID : ''),
                    Number: arrmobileno
                })
            }

            function DisplayMobileNoBySelectedRecipient(tempArrMobileNo) {
                $('#listofManualRecipient').empty().removeData().data(tempArrMobileNo);

                if (tempArrMobileNo.length == 0) {
                    $('.txtActionClientName').text('');
                    return;
                }
                $('.txtActionClientName').text(tempArrMobileNo[0].ClientName);
                for (var i = 0; i < tempArrMobileNo.length; i++) {
                    for (var m = 0; m < tempArrMobileNo[i].Number.length; m++) {
                        var divnum = $('<div class="contactchip" title="' + tempArrMobileNo[i].Name + '">').append($('<div>')
                                .html(tempArrMobileNo[i].Name.substr(0, 1).toUpperCase())
                            )
                            .append($('<span>')
                                .html(tempArrMobileNo[i].Number[m])
                            )
                            .append($('<span>')
                                .attr('i', i)
                                .attr('m', m)
                                .attr('l', tempArrMobileNo[i].Number[m].length)
                                .addClass('contactclosebtn')
                                .html('×')
                                .attr('num', tempArrMobileNo[i].Number[m])
                                .data(tempArrMobileNo[i].Number)
                                .click(function () {
                                    var index = $(this).attr('i')
                                    var num = $(this).attr('num');
                                    var arrnum = tempArrMobileNo[CDec2($(this).attr('i'))].Number;

                                    //remove selected number
                                    arrSMSRecipient[CDec2(index)].Number = $.grep(arrnum, function (e) {
                                        return e != num;
                                    })

                                    DisplayMobileNoBySelectedRecipient(arrSMSRecipient)
                                })
                            )
                        $('#listofManualRecipient').append(divnum);
                    }
                }
            }

            function DisplayListOfReceipient() {
                var listofRecipient = $('#listofRecipient');
                $('#txtRecipientsCount').text(arrSMSRecipient.length);
                listofRecipient.html('');

                for (var i = 0; i < arrSMSRecipient.length; i++) {
                    listofRecipient.append($('<div class="contactchip">').append($('<div>')
                        //.addClass('mobilenumber')
                        .html((arrSMSRecipient[i].Name.substr(0, 1).toUpperCase() == '+' ? '#' : arrSMSRecipient[i].Name.substr(0, 1).toUpperCase()))
                    )
                    .append($('<span>')
                        .html(arrSMSRecipient[i].Name)
                    )
                    .append($('<span>')
                        .addClass('contactclosebtn')
                        .html('×')
                        .data(arrSMSRecipient[i])
                        .click(function () {
                            var id = $(this).data().ID;
                            var cpid = $(this).data().CPID;

                            if ($(this).data().CPID == undefined) {
                                arrSMSRecipient = $.grep(arrSMSRecipient, function (val) {
                                    return val.ID != id;
                                })
                            }
                            else {
                                arrSMSRecipient = $.grep(arrSMSRecipient, function (val) {
                                    return val.CPID != cpid;
                                })
                            }

                            $('#chbbulk-' + id).prop('checked', false);//.checkboxradio('refresh');
                            DisplayListOfReceipient()
                        })
                    )
                    )
                }
            }

            function DisplayListOfSelectedReceipient() {
                var listSelectedReceipient = $('#listSelectedReceipient');

                listSelectedReceipient.empty();

                for (var i = 0; i < arrSMSRecipient.length; i++) {
                    var divs = $('<div>');

                    for (var m = 0; m < arrSMSRecipient[i].Number.length; m++) {
                        var divnum = $('<div class="mobilecontainer">').append($('<div>')
                                .addClass('mobilenumber')
                                .html(arrSMSRecipient[i].Number[m])
                            )
                            .append($('<div>')
                                .attr('i', i)
                                .attr('m', m)
                                .attr('l', arrSMSRecipient[i].Number.length)
                                .addClass('closemobilenumber')
                                .html('x')
                                .attr('num', arrSMSRecipient[i].Number[m])
                                .data(arrSMSRecipient[i].Number)
                                .click(function () {
                                    var index = $(this).attr('i')
                                    var num = $(this).attr('num');
                                    var arrnum = arrSMSRecipient[CDec2($(this).attr('i'))].Number;
                                    arrSMSRecipient[CDec2(index)].Number = $.grep(arrnum, function (e) {
                                        return e != num;
                                    })

                                    if ($(this).attr('l') == 1) {
                                        $('#chbbulk-' + arrSMSRecipient[CDec2(index)].ID).prop('checked', false);//.checkboxradio('refresh');
                                        arrSMSRecipient = $.grep(arrSMSRecipient, function (e) {
                                            return e.ID != arrSMSRecipient[CDec2(index)].ID;
                                        })
                                    }

                                    DisplayListOfSelectedReceipient()
                                })
                            )

                        divs.append(divnum);
                    }

                    listSelectedReceipient.append($('<li data-icon="delete">')
                        .data(arrSMSRecipient[i])
                        .append($('<a>')
                            .html(arrSMSRecipient[i].Name + '<br><br>')
                            .append(divs)
                        )
                        .append($('<a>')
                            .data(arrSMSRecipient[i])
                            .click(function (e) {
                                var id = $(this).data().ID
                                arrSMSRecipient = $.grep(arrSMSRecipient, function (val) {
                                    return val.ID != id;
                                })
                                $('#chbbulk-' + id).prop('checked', false);//.checkboxradio('refresh');
                                DisplayListOfSelectedReceipient()
                            })
                        )

                    )
                }

                listSelectedReceipient;//.listview('refresh');
            }

            function GetSmSUsage() {
                return;
                AjaxSend({
                    url: 'CRMDashboard/GetSmSUsage',
                    success: function (vData) {
                        DisplayResults(vData.Results.Rows);
                    }
                })
            }

            function DisplayResults(vRows) {
                var tblSmSUsage = $('#tblSmSUsagebody');

                tblSmSUsage.empty();

                for (var i = 0; i <= vRows.length - 1; i++) {

                    if (vRows[i].Telco == 'Total') {
                        tblSmSUsage.append($('<tr>')
                                                .css('cursor', 'pointer')
                                                .data(vRows[i])
                                                .append($('<td style="font-weight:bold; font-size:larger">')
                                                    .text(vRows[i].Telco)
                                                )
                                                .append($('<td style="text-align:right; font-weight:bold; font-size:larger">')
                                                    .text(vRows[i].DeviceToModem)
                                                )
                                                .append($('<td style="text-align:right; font-weight:bold; font-size:larger">')
                                                    .text('-')
                                                )
                                                .append($('<td style="text-align:right">')
                                                    .text('Click Row to see Details')
                                                )
                                                .click(function () {
                                                    GetSmSDetailsByTelco('All');

                                                })
                                                )
                    } else {
                        tblSmSUsage.append($('<tr>')
                                                .css('cursor', 'pointer')
                                                .data(vRows[i])
                                                .append($('<td>')
                                                    .text(vRows[i].Telco)
                                                )
                                                .append($('<td style="text-align:right">')
                                                    .text(vRows[i].DeviceToModem)
                                                )
                                                .append($('<td style="text-align:right">')
                                                    .text('-')
                                                )
                                                .append($('<td style="text-align:right">')
                                                    .text('Click Row to see Details')
                                                )
                                                .click(function () {
                                                    GetSmSDetailsByTelco($(this).data().ID);
                                                })
                                                )
                    }
                }
            }

            function GetSmSDetailsByTelco(vFilter) {
                $('#dataTableSMSLogs').DataTable().destroy();
                var dataTableSMSLogs = $('#dataTableSMSLogs').DataTable({
                    "order": [[2, "desc"]],
                    "bProcessing": true,
                    "bServerSide": true,
                    "ajax": {
                        url: "CRMDashboard/GetSmSUsagePerTelco?OrgID=" + sessionStorage.OrgID + "&TelcoID=" + vFilter,
                        type: 'POST'
                    },
                    "columns": [
                        {
                            "data": function (data, type, full, meta) {
                                return (data.SendToName == "" ? "" : data.SendToName + " - ") + data.SendTo
                            }
                        },
                        { "data": "Message" },
                        {
                            "data": function (data, type, full, meta) {
                                return $.format.date(new Date(parseInt(data.LastUpdated.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return (data.SentBy == null ? "" : data.SentBy)
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return (data.Status == null ? "" : data.Status)
                            }
                        }
                    ],
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": [0, 1] }],
                    "responsive": true,
                    "columnDefs": [{ "targets": 0, "orderable": false }],
                    "sDom": '<"top"i>rt<"bottom"ip><"clear">'
                    //"stateSave": true
                })
                return;
                var data = { TelcoID: vFilter };

                AjaxSend({
                    url: 'CRMDashboard/GetSmSUsagePerTelco',
                    data: data,
                    success: function (vData) {
                        DisplaySmSDetailsByTelco(vData.Results.Rows);
                    }
                })
            }

            function DisplaySmSDetailsByTelco(vRows) {
                var tblSmSUsage = $('#tblSmSUsageDetailbody');

                tblSmSUsage.empty();

                for (var i = 0; i <= vRows.length - 1; i++) {
                    vRows[i].isElippse = false;
                    var newMessage = vRows[i].Message;
                    if (newMessage.length > 100) {
                        vRows[i].isElippse = true;
                        newMessage = newMessage.substr(0, 100) + ' ... <br> <a data-role="none" class="custom-link">(read more)</a>'
                    }

                    tblSmSUsage.append($('<tr>')
                                            .css('cursor', 'pointer')
                                            .data(vRows[i])
                                            .append($('<td>')
                                                .text((vRows[i].SendToName == "" ? '' : vRows[i].SendToName + ' - ') + vRows[i].SendTo)
                                            )
                                            .append($('<td style="word-break: break-all;">')
                                                .data(vRows[i])
                                                .html(newMessage.replace(/\r\n|\r|\n/g, "<br />"))
                                                .click(function () {
                                                    if ($(this).data().isElippse) {
                                                        NewMessageBox($(this).data().Message.replace(/\r\n|\r|\n/g, "<br />"), 'SMS Message')
                                                    }
                                                })
                                            )
                                            .append($('<td>')
                                                .text(vRows[i].LastUpdated)
                                            )
                                            .append($('<td>')
                                                .text(vRows[i].SentBy)
                                            )
                                            )
                }
            }

            function SendSMS() {
                if (arrSMSRecipient.length == 0) {
                    NewMessageBox('Please add recipients')
                    return;
                }
                $('#popSMS').modal('hide');
                $('.bulk').show();

                NewMessageBox('Saving, please wait ...');

                AjaxSend({
                    url: 'CRMDashboard/SendSmS',
                    data: {
                        Message: ($.trim($('#txtSMSMessage').val()) == "" ? $('#txtSMSMessageAction').val() : $.trim($('#txtSMSMessage').val())),
                        Sender: arrSMSRecipient,
                        DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a')
                    },
                    success: function (vData) {
                        arrSMSRecipient = [];
                        NewMessageBoxClose()
                        if (vData.Results == 'Saved') {
                            $('#charNum2').text('Characters left: 160');
                            $('#txtSMSMessage').val('').height('100px').focus();
                            $('#btnCancelSMSSelectedReceipient').click();
                        }
                        else {
                            NewMessageBox('Your session has expired. Redirecting to sign page.');
                            // setTimeout(function(){
                            //     location = '/SignIn/CRM';
                            // }, 3000);
                        }
                    }
                });
            }

            function SortByName(a, b) {
                var aName = a.Name.toLowerCase();
                var bName = b.Name.toLowerCase();
                return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
            }

            function AddAffiliatedPerson() {
                var lstAffiliatedPerson = $('#lstAffiliatedPerson'), isEmpty = true;

                lstAffiliatedPerson.empty();

                for (var i = 0; i < arrOrgAffPersonList.length; i++) {
                    if (arrOrgAffPersonList[i].State != 'Remove') {
                        isEmpty = false;
                        lstAffiliatedPerson.append($('<a href="#" class="list-group-item list-group-item-action">')
                            .data(arrOrgAffPersonList[i])
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">')
                                        .data(arrOrgAffPersonList[i])
                                        .text(arrOrgAffPersonList[i].PeopleName)
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                    )
                                )
                            )
                        )
                    }
                }

                if (isEmpty) {
                    lstAffiliatedPerson.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
            }

            function AddOrgAffiliation() {
                var lstOrgAffiliation = $('#lstOrgAffiliation'), isEmpty = true;

                lstOrgAffiliation.empty();

                for (var i = 0; i < arrOrgAffList.length; i++) {
                    if (arrOrgAffList[i].State != 'Remove') {
                        isEmpty = false;
                        lstOrgAffiliation.append($('<a href="#" class="list-group-item list-group-item-action">')
                            .data(arrOrgAffList[i])
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">')
                                        .data(arrOrgAffList[i])
                                        .text(arrOrgAffList[i].CompanyName)
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                    )
                                )
                            )
                        )
                    }
                }

                if (isEmpty){
                    lstOrgAffiliation.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
            }

            function AddPersonOrgAffiliation() {
                var lstPersonOrgAffiliation = $('#lstPersonOrgAffiliation'), isEmpty = true;

                lstPersonOrgAffiliation.empty();

                for (var i = 0; i < arrPersonOrgAffList.length; i++) {
                    if (arrPersonOrgAffList[i].State != 'Remove') {
                        isEmpty = false;
                        lstPersonOrgAffiliation.append($('<a href="#" class="list-group-item list-group-item-action">')
                            .data(arrPersonOrgAffList[i])
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">')
                                        .data(arrPersonOrgAffList[i])
                                        .text(arrPersonOrgAffList[i].CompanyName)
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                    )
                                )
                            )
                        );//.listview();//.listview('refresh')
                    }
                }

                if (isEmpty) {
                    lstPersonOrgAffiliation.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
            }

            function PreSaveToDo() {
                if ($.trim($('#txtToDo').val()) == '') {
                    $('#txtToDo').focus();
                    return;
                }

                arrTodoList.push({
                    TodoRankNo: (intTodoInfoCtr == 0 ? 1 : intTodoInfoCtr + 1),
                    ClientID: $('.txtActionClientName').data().ID,
                    ToDoID: $('#txtToDoID').val(),
                    ToDo: $('#txtToDo').val(),
                    ToDoDueDate: todoSetDate,
                    ToDoOrder: (intTodoInfoCtr == 0 ? 1 : intTodoInfoCtr + 1),
                    ToDoFinish: '0',
                    State: 'Add',
                    IsNew: 1,
                    IsOrg: ($('.txtActionClientName').data().ProspectType == "Org" ? 1 : 0)
                })
                SaveToDo(false);
            }

            function SaveToDo(isCheckedToDo) {
                ClearTodoFields();

                AjaxSend({
                    url: 'CRMDashboard/SaveToDo',
                    data: { ToDo: arrTodoList, DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a') },
                    success: function (vData) {
                        if (vData.Results == 'Saved') {
                            if (isCheckedToDo == false) {
                                GetClientToDo($('.txtActionClientName').data().ID);
                            }
                            WebToast('Saved', 'top');

                        }
                        else {
                            NewMessageBox('ERROR: ' + vData.Results);
                        }
                    }
                })
            }

            function GetClientToDo(ClientID) {
                $('#tbodyToDo').append('<tr colspan="5" style="text-align:center">Please wait...</tr>')
                AjaxSend({
                    url: 'CRMDashboard/GetClientToDo',
                    data: { ClientID: ClientID },
                    success: function (vData) {
                        arrTodoList = vData.Results.Rows;
                        DisplayClientToDo(vData.Results.Rows);
                    }
                })
            }

            function DisplayClientToDo(vRows) {
                var listToDo = $('#listToDo');

                listToDo.empty();
                if (vRows.length > 0) {
                    for (var i = 0 ; i < vRows.length; i++) {
                        if (vRows[i].State != 'Remove') {
                            vRows[i].arrindex = i;
                            intTodoInfoCtr = CDec2(vRows[i].TodoRankNo);

                            listToDo.append($('<li>')
                                    .append($('<div class="todoContainer">')
                                        .css('cursor', 'pointer')
                                        .append($('<div style="float:left; width:50px;">')
                                            .data(vRows[i])
                                            .append($('<label class="checkbox-only" for="chbtodo-' + i + '">')
                                                .append($('<input type="checkbox" name="todo" id="chbtodo-' + i + '" data-iconpos="notext"/>')
                                                    .data(vRows[i])
                                                    .on('click', function () {
                                                        var arrindex = $(this).data().arrindex;
                                                        arrTodoList[arrindex].ToDoFinish = ($('#chbtodo-' + arrindex).is(':checked') == true ? '1' : '0');
                                                        SaveToDo(true);
                                                    })
                                                )
                                            )
                                        )
                                        .append($('<div style="float:right; width:50px;">')
                                            .data(vRows[i])
                                            .append($('<button data-role="none" title="Set Date" class="btn custom-btn-theme-h btnCircle-sm btnShadow">')
                                                .append('<img alt="Remove" src="/Content/images/trash.png" />')
                                            )
                                            .click(function () {
                                                var TodoRankNo = $(this).data().TodoRankNo;

                                                if ($(this).data().IsNew == 1) {
                                                    //remove selected telephone
                                                    arrTodoList = $.grep(arrTodoList, function (val) {
                                                        return val.TodoRankNo != TodoRankNo;
                                                    })
                                                }
                                                else {
                                                    arrTodoList[$(this).data().arrindex].State = "Remove";
                                                }
                                                SaveToDo(false);
                                            })
                                        )
                                        .append($('<div style="float:none; width:auto; overflow:hidden; padding:8px; word-break:break-all;">')
                                            .data(vRows[i])
                                            .html('<b>' + vRows[i].ToDo + '</b><br><br>' +
                                                    'Due Date: ' + (vRows[i].ToDoDueDate == '' ? '-' : $.format.date(new Date(vRows[i].ToDoDueDate), 'MMM dd, yyyy'))
                                            )
                                            .click(function () {
                                                $('#popToDo').modal('show');
                                                todoSetDate = ($(this).data().ToDoDueDate == '' ? new Date() : $(this).data().ToDoDueDate);
                                                $('#txtEditToDo').val($(this).data().ToDo);
                                                $('#txtToDoID').val($(this).data().ToDoID);
                                                $('#btnEditToDoSetDate').text($.format.date(new Date(todoSetDate), 'MMM dd, yyyy'));//.button('refresh');
                                                sessionStorage["ToDoState"] = 'Edit';
                                                sessionStorage["ToDoIsNew"] = $(this).data().IsNew;
                                                sessionStorage["ToDoIndex"] = $(this).data().arrindex;
                                                sessionStorage["ToDoIsFinish"] = ($('#chbtodo-' + $(this).data().arrindex).is(':checked') == true ? '1' : '0');
                                                TodoRankNo = arrTodoList[$(this).data().arrindex].TodoRankNo;
                                            })
                                        )
                                    )
                                )
                        }
                        $('#chbtodo-' + i).prop('checked', (vRows[i].ToDoFinish == "0" ? false : true))
                        $('input[type="checkbox"]');//.checkboxradio();//.checkboxradio('refresh');
                    }
                }
            }

            function ClearTodoFields() {
                $('#txtToDoID').val('');
                $('#txtToDo').val('');
                //$('#btnToDoSetDate').text('Set Date');//.button('refresh');
                todoSetDate = '';
            }

            function GetInteractionHistory(clientid) {
                var tblInteractionHistory = $('#tblInteractionHistory');

                tblInteractionHistory.empty().append('<tr><td colspan="6" style="text-align:center;">Please wait ...</td></tr>');

                AjaxSend({
                    url: 'CRMDashboard/GetInteractionHistory',
                    data: { ClientID: clientid },
                    success: function (vData) {
                        var vRows = vData.Results.Rows;
                        tblInteractionHistory.empty();

                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                var interDate;
                                if (vRows[i].LastAction == "Call") {
                                    interDate = vRows[i].CallStart;
                                }
                                else {
                                    interDate = vRows[i].LastUpdated;
                                }
                                vRows[i].InteractionDate = interDate;
                                tblInteractionHistory.append($('<tr>')
                                    .data(vRows[i])
                                    .css('cursor', 'pointer')
                                    .append($('<td>')
                                        .text((i + 1) + '.')
                                    )
                                    .append($('<td>')
                                        .text($.format.date(new Date(interDate), 'MMM dd, yyyy hh:mm a'))
                                    )
                                    .append($('<td>')
                                        .text(vRows[i].LastAction)
                                    )
                                    .append($('<td>')
                                        .text(vRows[i].CreatedBy)
                                    )
                                    .append($('<td>')
                                        .text(vRows[i].CallDuration)
                                    )
                                    .append($('<td>')
                                        //.attr('title', (vRows[i].Notes || '&nbsp;'))
                                        .append($('<a>')
                                            .data(vRows[i])
                                            .addClass('hasTooltip')
                                            .html('<img src="/Content/images/info.png">')
                                            .hover(function () {
                                                $('#tooltipDateAndTime').text($.format.date(new Date($(this).data().InteractionDate), 'MMM dd, yyyy hh:mm a'))
                                                $('#tooltipNotes').html(($(this).data().Notes.replace(/\r\n|\r|\n/g, "<br />") || '&nbsp;'))
                                                $('#divtooltipNotes').modal('show');
                                            }, function () {
                                            })
                                        )
                                    )
                                    .hover(function () {
                                        $('#tooltipDateAndTime').text($.format.date(new Date($(this).data().InteractionDate), 'MMM dd, yyyy hh:mm a'))
                                        $('#tooltipNotes').html(($(this).data().Notes.replace(/\r\n|\r|\n/g, "<br />") || '&nbsp;'))
                                        $('#divtooltipNotes').modal('show');
                                    }, function () {
                                    })
                                    .click(function () {
                                        return;
                                        var data = $(this).data();
                                        $('#popInteractionHistoryDetails').modal('show');

                                        $('#txtInteractionHistoryDetailsClientName').text($('.txtActionClientName').text());
                                        $('#txtInteractionHistoryDetailsCalledBy').text(data.CreatedBy);
                                        $('#txtInteractionHistoryDetailsCallDuration').text(data.CallDuration);
                                        $('#txtInteractionHistoryDetailsStart').text($.format.date(new Date(data.InteractionDate), 'MMM dd, yyyy hh:mm:ss a'));
                                        $('#txtInteractionHistoryDetailsEnd').text($.format.date(new Date(data.CallEnd), 'MMM dd, yyyy hh:mm:ss a'));
                                        $('#txtInteractionHistoryDetailsNotes').html((data.Notes.replace(/\r\n|\r|\n/g, "<br />") || '&nbsp;'));
                                    })
                                )
                            }
                        }
                        else {
                            tblInteractionHistory.append('<tr><td colspan="6" style="text-align:center;">No Result(s) Found</td></tr>');
                        }

                        tblInteractionHistory.parent();//.table('refresh');
                    }
                })
            }

            function SaveNewPersonProspect(autosave) {
                var pData = {};

                if ($.trim($('#txtProspectPersonFirstName').val()) == '') {
                    NewMessageBox('Please enter first name', '',
                        [
                            {
                                Text: 'Ok',
                                Click: function () {
                                    $(window).scrollTop(0);
                                }
                            }
                        ]
                    );
                    return;
                }

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
                    Region: $('#selPersonRegion').val(),
                    Island: $('#selPersonIsland').val(),
                    VatType: $('#selVatType').val(),
                    Country: $('#selPersonCountry').val(),
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
                    UnlimitedCredit: $('#chkRevUnlimitedCredit').prop('checked'),
                    RequireCnCApproval: $('#chkRevRequireCnCApproval').prop('checked'),
                    CustomerRequirePoNum: $('#chkRevCustomerRequirePoNum').prop('checked'),
                    Balance: $('#txtRevCreditBegBal').val(),
                    AsOf: $('#btnRevCreditBegAsOf').text(),
                    ThirdPartyBalance: $('#txtRevThirdPartyBegBalance').val(),
                    ThirdPartyAsOf: $('#btnRevThirdPartyBegBalanceAsOf').text(),

                    IsCustomerSettings: (sessionStorage.LibraryName == 'Customer / Client List' ? true : false)
                }
                //console.log(pData);
                AjaxSend({
                    url: 'OrganizationShort/SaveNewPersonProspect',
                    data: pData,
                    success: function (vData) {
                        isEditingProfile = false;
                        if (IsGuid(vData.Results) && autosave == undefined) {
                            WebToast('Successfully Saved', 'top');
                            if (sessionStorage.LibraryName == 'Customer / Client List') {
                                datatable.draw(); // refresh datatable after saving
                            } else {
                                LibraryTable.SupplierList();
                            }

                            $('#btnCancelPersonAddingContact').click();
                            return;

                            if (isAddNewProspect) {
                                $('#listAction').find('li > a:not(:first)').removeClass('disabled-menu');
                                var mobilenos = undefined;
                                for (var i = 0; i < pData.Mobile.length; i++) {
                                    mobilenos = (i == 0 ? pData.Mobile[i].MobileNos : ',' + pData.Mobile[i].MobileNos);
                                }
                                var data = {
                                    ID: vData.Results,
                                    ClientName: $.trim(pData.FirstName + ' ' + pData.MiddleName + ' ' + pData.LastName),
                                    MobileNos: mobilenos,
                                    ProspectType: 'People'
                                }
                                sessionStorage.ClientInfo = JSON.stringify(data);
                                isAddNewProspect = false;
                                $('#txtProspectPersonID').val(vData.Results)
                            }
                            else {
                                ClearPersonProspectVariablesFields();
                                $('#pageMainDashboard').pageshow();
                                isFromColdCallPage = false;
                            }
                        }
                        else if (!IsGuid(vData.Results)) {
                            NewMessageBox(vData.Results, 'Message');
                        }
                    }
                })
            }

            function AddPersonEmailInfo() {

                var pLV = $('#lstPersonEmail'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrPersonEmailInfoList.length; i++) {
                    if (arrPersonEmailInfoList[i].EmailState != "Remove") {
                        isEmpty = false;
                        arrPersonEmailInfoList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                            .data(arrPersonEmailInfoList[i])
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">')
                                        .html($.trim(arrPersonEmailInfoList[i].EmailAdd) +
                                            "<br>" + $.trim(arrPersonEmailInfoList[i].EmailType)
                                        )
                                        .data(arrPersonEmailInfoList[i])
                                        .click(function (e) {
                                            EditPersonEmailInfo($(this).data().arrindex);
                                        })
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                    )
                                )
                            )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popPersonEmailInfo').modal('hide');
            }

            function EditPersonEmailInfo(arrindex) {
                $('#popPersonEmailInfo').modal('show');
                PersonEmailRankNo = arrPersonEmailInfoList[arrindex].EmailRankNo;
                PersonEmailState = "Edit";
                sessionStorage["Emailindex"] = arrindex;
                sessionStorage["EmailIsNew"] = arrPersonEmailInfoList[arrindex].IsNew;
                sessionStorage["EmailState"] = arrPersonEmailInfoList[arrindex].EmailState;
                $('#txtPersonEmailID').val(arrPersonEmailInfoList[arrindex].EmailAddID == undefined ? "" : arrPersonEmailInfoList[arrindex].EmailAddID);
                $('#txtPersonEmailAdd').val(arrPersonEmailInfoList[arrindex].EmailAdd);
                $('#selPersonEmailType').val(arrPersonEmailInfoList[arrindex].EmailType);//.selectmenu('refresh');
            }

            function ClearPersonEmailInfoFields() {
                $('#txtPersonEmailID').val('');
                $('#txtPersonEmailAdd').val('');
                $('#selPersonEmailType').val('');//.selectmenu('refresh');
            }

            function AddPersonMobileInfo() {

                var pLV = $('#lstPersonMobile'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrPersonMobileInfoList.length; i++) {
                    if (arrPersonMobileInfoList[i].MobileState != "Remove") {
                        isEmpty = false;
                        arrPersonMobileInfoList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                            .data(arrPersonMobileInfoList[i])
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">')
                                        .data(arrPersonMobileInfoList[i])
                                        .attr('ID', arrPersonMobileInfoList[i].id)
                                        .html("+" + $.trim(arrPersonMobileInfoList[i].CountryCode.split('-')[1]) + "" + (arrPersonMobileInfoList[i].MobileNo.length <= 7 ? + arrPersonMobileInfoList[i].Prefix : '') + $.trim(arrPersonMobileInfoList[i].MobileNo) +
                                            "<br>" + $.trim(arrPersonMobileInfoList[i].ContactType)
                                        )
                                        .click(function (e) {
                                            EditPersonMobileInfo($(this).data().arrindex);
                                        })
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                    )
                                )
                            )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popPersonMobileInfo').modal('hide');
            }

            function EditPersonMobileInfo(arrindex) {
                $('#popPersonMobileInfo').modal('show');
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
                $('#txtPersonMobileNo').val((arrPersonMobileInfoList[arrindex].MobileNo.length <= 7 ? +arrPersonMobileInfoList[arrindex].Prefix : '') + arrPersonMobileInfoList[arrindex].MobileNo);
                $('#selPersonMobileType').val(arrPersonMobileInfoList[arrindex].ContactType);//.selectmenu('refresh');
            }

            function ClearPersonMobileInfo() {
                $('#txtPersonMobileID').val('');
                $('#txtPersonTelCoPrefixID').val('');
                $('#txtPersonMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtPersonMobileCountryCode').text('PHL - 63');
                //$('#txtPersonTelCode').text('Select');
                $('#txtPersonMobileNo').val('');
                $('#selPersonMobileType').val('');//.selectmenu('refresh');
            }

            function AddPersonTelInfo() {

                var pLV = $('#lstPersonTelephone'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrPersonTelInfoList.length; i++) {
                    if (arrPersonTelInfoList[i].TelState != "Remove") {
                        isEmpty = false;
                        arrPersonTelInfoList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                            .data(arrPersonTelInfoList[i])
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">')
                                        .html("+" + $.trim(arrPersonTelInfoList[i].CountryCode.split('-')[1]) + " (" + $.trim(arrPersonTelInfoList[i].AreaCode.split('-')[1]) + ") " + $.trim(arrPersonTelInfoList[i].TelephoneNo) + ($.trim(arrPersonTelInfoList[i].ExtensionNo) == "" ? "" : "." + $.trim(arrPersonTelInfoList[i].ExtensionNo)) +
                                            "<br>" + $.trim(arrPersonTelInfoList[i].AreaCode.split('-')[0]) + ", " + $.trim(arrPersonTelInfoList[i].CountryCode.split('-')[0]) +
                                            "<br>" + $.trim(arrPersonTelInfoList[i].ContactType)
                                        )
                                        .data('data', arrPersonTelInfoList[i])
                                        .click(function (e) {
                                            EditPersonTelInfo($(this).data('data').arrindex);
                                        })
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                    )
                                )
                            )

                        pLV.append(pItem);
                    }

                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popPersonTelInfo').modal('hide');
            }

            function EditPersonTelInfo(arrindex) {
                $('#popPersonTelInfo').modal('show');

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
                $('#selPersonTelType').val(arrPersonTelInfoList[arrindex].ContactType);//.selectmenu('refresh');
            }

            function ClearPersonTelInfo() {
                $('#txtPersonTelID').val('');
                $('#txtPersonTelAreaCodeID').val('');
                $('#txtPersonTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtPersonCountryCode').text('PHL - 63');
                $('#txtPersonAreaCode').text('Select');
                $('#txtPersonTelephoneNum').val('');
                $('#txtPersonTelephoneExt').val('');
                $('#selPersonTelType').val('');//.selectmenu('refresh');
            }

            function SaveNewOrgProspect(autosave) {
                var pData = {};

                if ($.trim($('#txtProspectCompanyName').val()) == '') {
                    NewMessageBox('Please enter company name', '',
                        [
                            {
                                Text: 'Ok',
                                Click: function () {
                                    $(window).scrollTop(0);
                                }
                            }
                        ]
                    );
                    return;
                }

                if ($.trim($('#txtProspectOrgTin').val()) == '') {
                    NewMessageBox('Please enter TIN number', '',
                        [
                            {
                                Text: 'Ok',
                                Click: function () {
                                    $(window).scrollTop(0);
                                }
                            }
                        ]
                    );
                    return;
                }
                
                //if ($.trim($('#txtProspectHouseBldgNo').val()) == '') {
                //    NewMessageBox('Please enter building number', '',
                //        [
                //            {
                //                Text: 'Ok',
                //                Click: function () {
                //                    $(window).scrollTop(0);
                //                }
                //            }
                //        ]
                //    );
                //    return;
                //}

                //if ($.trim($('#txtProspectStreetName').val()) == '') {
                //    NewMessageBox('Please enter street', '',
                //        [
                //            {
                //                Text: 'Ok',
                //                Click: function () {
                //                    $(window).scrollTop(0);
                //                }
                //            }
                //        ]
                //    );
                //    return;
                //}

                if ($.isFunction(proj.VerifySupplier))
                    if (!proj.VerifySupplier())
                        return;

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
                    CompanyBusinessTradeName: $.trim($('#txtProspectCompanyBusinessTradeName').val()),
                    Tin: $('#txtProspectOrgTin').val(),
                    CompanyBuildingAddress: $('#txtProspectHouseBldgNo').val(),
                    CompanyStreetAddress: $('#txtProspectStreetName').val(),
                    CompanyCityAddress: $('#selProspectCities').val(),
                    CompanyRegionAddress: $('#selProspectOrgRegion').val(),
                    Island: $('#selProspectIsland').val(),
                    VatType: $('#selVatType').val(),
                    CompanyCountryAddress: $('#selProspectCountries').val(),
                    CompanyBackground: $('#txtProspectCompanyBackground').val(),
                    CompanyMission: $('#txtProspectCompanyMission').val(),
                    CompanyVision: $('#txtProspectCompanyVision').val(),
                    CompanyFoundingDate: $('#txtProspectCompanyFounding').val(),
                    BusinessIndustryID: $('#txtBusIndustryID').val(),
                    SalesChannelID: mSalesChannelID,
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
                    SalesReps: arrSalesReps,
                    SalesRepID: RevSalesRepID,
                    PriceLevel: [], // per fowst + credit terms na ni sya - by arnie 9-16-2022
                    Terms: RevPaymentTerms,
                    Limit: $('#txtRevCreditLimit').val(),
                    UnlimitedCredit: $('#chkRevUnlimitedCredit').prop('checked'),
                    RequireCnCApproval: $('#chkRevRequireCnCApproval').prop('checked'),
                    CustomerRequirePoNum: $('#chkRevCustomerRequirePoNum').prop('checked'),
                    Balance: $('#txtRevCreditBegBal').val(),
                    SingleCreditTerm: $('#selRevenueSingleFowst').val() == 'single',
                    AsOf: $('#btnRevCreditBegAsOf').text(),
                    ThirdPartyBalance: $('#txtRevThirdPartyBegBalance').val(),
                    ThirdPartyAsOf: $('#btnRevThirdPartyBegBalanceAsOf').text(),
                    DefaultHotButtonTemplateID: $("#selDefaultHBTemplate").val(),
                    IsCustomerSettings: (sessionStorage.LibraryName == 'Customer / Client List' ? true : false),
                    NewVendorPrices: arrEditVendorPrices,
                    UserID: sessionStorage.PeopleID
                }
                
                if (sessionStorage.LibraryName.indexOf('Customer') != -1) {
                    pData.SaveAsCustomer = 1;

                    var arrCreditTerms = [];

                    $('#tbodyRevCreditTerms tr').each(function () {
                        var data = $(this).data('data');

                        if (data != undefined) arrCreditTerms.push(data);
                    })

                    pData.CusCreditTerms = arrCreditTerms;
                }                    

                if (sessionStorage.LibraryName.indexOf('Supplier') != -1
                    || sessionStorage.LibraryName == 'Manpower Agencies' // for dtr
                    || sessionStorage.LibraryName == 'DTR Machine Co-User Organizations' // for dtr
                    || sessionStorage.LibraryName == 'Vendor/ Trading Partners') { // for dtr
                    pData.SaveAsSupplier = 1;
                    pData.SupplierInfo = proj.GetSupplierInfoFromHtml();
                }

                if (sessionStorage.LibraryName == 'Manpower Agencies') {
                    pData.SupplierType = 'manpower';
                } else if (sessionStorage.LibraryName == 'DTR Machine Co-User Organizations') {
                    pData.SupplierType = 'co-user';
                }

                AjaxSend({
                    url: 'OrganizationShort/SaveNewOrgProspect',
                    data: pData,
                    success: function (vData) {
                        isEditingProfile = false;
                        if (IsGuid(vData.Results) && autosave == undefined) {
                            WebToast('Successfully Saved', 'top');
                            if (sessionStorage.SystemAdminFor == 'dtr') {
                                if (sessionStorage.LibraryName == 'Manpower Agencies') {
                                    DTRLibraryTable.ManpowerAgencies();
                                } else if (sessionStorage.LibraryName == 'DTR Machine Co-User Organizations') {
                                    DTRLibraryTable.CoUser();
                                } else if (sessionStorage.LibraryName == 'Vendor/ Trading Partners') {
                                    DTRLibraryTable.VendorTradingPartners();
                                }
                            } else {
                                if (sessionStorage.LibraryName == 'Customer / Client List') {
                                    datatable.draw(); // refresh datatable after saving
                                } else {
                                    LibraryTable.SupplierList();
                                }

                                setTimeout(function () {
                                    var outlet = [];

                                    for (var i = 0; i < arrCompanyOutlet.length; i++) {
                                        if (arrCompanyOutlet[i].State == 'add') { // Get New Oulet
                                            outlet.push(arrCompanyOutlet[i])
                                        }
                                    }

                                    for (var i = 0; i < arrCompanyOutlet.length; i++) {
                                        if (arrCompanyOutlet[i].State != 'add') {
                                            for (var d = 0; d < arrCompanyOutlet[i].Department.length; d++) { // Get New departments of existing outlets
                                                if (arrCompanyOutlet[i].Department[d].State == 'add') {
                                                    arrNewDepartment.push(arrCompanyOutlet[i].Department[d]);
                                                }
                                            }
                                        }
                                    }

                                    if (outlet.length > 0) {
                                        var label = (sessionStorage.LibraryName == 'Customer / Client List' ? 'Customer' : 'Supplier');
                                        NewMessageBox('Would you like to add new outlet(s) and its department(s) to other ' + label + 's?', 'New outlet detected', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    BrowsePopup((sessionStorage.LibraryName == 'Customer / Client List' ? 'LibraryOrganizationCustomer' : 'LibraryOrganizationSupplier'), 'Select ' + label, [
                                                        {
                                                            Message: 'Select '+ label.toLowerCase() +' that you want to add the new outlet(s). You can select multiple ' + label.toLowerCase() + '.',
                                                            SelectMultiple: true,
                                                            AutoSearch: true,
                                                            DisplayText: [{ Column: (sessionStorage.LibraryName == 'Customer / Client List' ? 'CustomerName' : 'SupplierName') }],
                                                            Click: function (e) {
                                                                var listNewOutlet = $('#listNewOutlet');
                                                                isOutlet = true;

                                                                listNewOutlet.empty();
                                                                $('#lblNewOutletMsg').text('Outlet(s) and Department(s) with ticked checkboxes will be added to these customers. Uncheck the outlet or department if you do not want to add to the customer.');

                                                                for (var i = 0; i < e.length; i++) {
                                                                    var ul = $('<ul id="' + e[i].DbID + '" data-role="none" style="list-style:none">')

                                                                    for (var o = 0; o < outlet.length; o++) {
                                                                        var deptul = $('<ul  id="' + outlet[o].ID + '" data-role="none" style="list-style:none">')

                                                                        for (var d = 0; d < outlet[o].Department.length; d++) {
                                                                            deptul.append($('<li>')
                                                                                    .data('data', outlet[o].Department[d])
                                                                                    .append($('<div class="custom-checkbox">')
                                                                                        .append($('<label class="checkbox-only">')
                                                                                            .append($('<input type="checkbox" class="chbnewoutlet" checked data-iconpos="notext" />')
                                                                                                .data('data', outlet[o].Department[d])
                                                                                                .attr({
                                                                                                    depid: outlet[o].Department[d].ID
                                                                                                })
                                                                                            )
                                                                                        )
                                                                                        .append($('<div>').text(outlet[o].Department[d].Department))
                                                                                    )
                                                                                )
                                                                        }

                                                                        ul.append($('<li>')
                                                                            .data('data', outlet[o])
                                                                            .append($('<div class="custom-checkbox">')
                                                                                    .append($('<label class="checkbox-only">')
                                                                                        .append($('<input type="checkbox" class="chbnewoutlet" checked data-iconpos="notext" />')
                                                                                            .data('data', outlet[o])
                                                                                            .attr({
                                                                                                outletid: outlet[o].ID
                                                                                            })
                                                                                            .change(function () {
                                                                                                var chb = $(this).parents().closest('.custom-checkbox').next().find('input');

                                                                                                if (!$(this).is(':checked')) {
                                                                                                    chb.attr({
                                                                                                        'checked': false,
                                                                                                        disabled: true
                                                                                                    })
                                                                                                } else {
                                                                                                    chb.attr({
                                                                                                        disabled: false
                                                                                                    })
                                                                                                }

                                                                                                chb;//.checkboxradio('refresh');
                                                                                            })
                                                                                        )
                                                                                    )
                                                                                    .append($('<div>').text(outlet[o].Name))
                                                                                )
                                                                            .append(deptul)
                                                                        )
                                                                    }
                                                                    listNewOutlet.append($('<li>')
                                                                            .data('data', e[i])
                                                                            .append((e[i].CustomerName || e[i].SupplierName))
                                                                            .append(ul)
                                                                        )
                                                                }

                                                                $('.chbnewoutlet');//.checkboxradio();//.checkboxradio('refresh');
                                                                $('#popNewOutlet').show();
                                                            }
                                                        }
                                                    ], { FilterBy: 'Org' })
                                                }
                                            }, { Text: 'No' }
                                        ], {
                                            AutoDismiss: false
                                        })
                                    } else {
                                        ApplyNewDepartmentToOutlets();
                                    }
                                }, 500)                                
                            }

                            $('#btnCancelAddingContact').click();

                            return;

                            if (isAddNewProspect) {
                                $('#listAction').find('li > a:not(:first)').removeClass('disabled-menu');
                                var mobilenos = undefined;
                                for (var i = 0; i < pData.Mobile.length; i++) {
                                    mobilenos = (i == 0 ? pData.Mobile[i].MobileNos : ',' + pData.Mobile[i].MobileNos);
                                }
                                var data = {
                                    ID: vData.Results,
                                    ClientName: pData.CompanyName,
                                    MobileNos: mobilenos,
                                    ProspectType: 'Org'
                                }
                                sessionStorage.ClientInfo = JSON.stringify(data);
                                isAddNewProspect = false;
                                $('#txtProspectOrgID').val(vData.Results)
                            }
                            else {
                                ClearOrgProspectVariablesFields();
                                isFromColdCallPage = false;
                                $('#divCompanyProfile').show();
                                $('#pageMainDashboard').pageshow();
                            }
                            //CacheBusinessIndustry();
                        }
                        else if (!IsGuid(vData.Results)) {
                            NewMessageBox(vData.Results, 'Message');
                        }
                    }
                })
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
                                                success: function(e){
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

                                                    $('.chbnewoutlet');//.checkboxradio();//.checkboxradio('refresh');
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

                var pLV = $('#lstContactPersonEmail'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrContactPersonEmailInfoList.length; i++) {
                    if (arrContactPersonEmailInfoList[i].EmailState != "Remove") {
                        isEmpty = false;
                        arrContactPersonEmailInfoList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .data(arrContactPersonEmailInfoList[i])
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .data(arrContactPersonEmailInfoList[i])
                                                .html($.trim(arrContactPersonEmailInfoList[i].EmailAdd) +
                                                    "<br>" + $.trim(arrContactPersonEmailInfoList[i].EmailType)
                                                )
                                                .click(function (e) {
                                                    EditContactPersonEmailInfo($(this).data().arrindex);
                                                })
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popContactPersonEmailInfo').modal('hide');
                $('#popContactPersonProfile').modal('show');
            }

            function EditContactPersonEmailInfo(arrindex) {
                $('#popContactPersonProfile').modal('hide');
                $('#popContactPersonEmailInfo').modal('show');
                ContactPersonEmailRankNo = arrContactPersonEmailInfoList[arrindex].EmailRankNo;
                ContactPersonEmailState = "Edit";
                sessionStorage["Emailindex"] = arrindex;
                sessionStorage["EmailIsNew"] = arrContactPersonEmailInfoList[arrindex].IsNew;
                sessionStorage["EmailState"] = arrContactPersonEmailInfoList[arrindex].EmailState;
                $('#txContacttPersonEmailID').val(arrContactPersonEmailInfoList[arrindex].EmailAddID == undefined ? "" : arrContactPersonEmailInfoList[arrindex].EmailAddID);
                $('#txtContactPersonEmailAdd').val(arrContactPersonEmailInfoList[arrindex].EmailAdd);
                $('#selContactPersonEmailType').val(arrContactPersonEmailInfoList[arrindex].EmailType);//.selectmenu('refresh');
            }

            function ClearContactPersonEmailInfoFields() {
                $('#txtContactPersonEmailID').val('');
                $('#txtContactPersonEmailAdd').val('');
                $('#selContactPersonEmailType').val('');//.selectmenu('refresh');
                $('#divContactPersonEmailHome').pageshow();
            }

            function AddContactPersonMobileInfo() {

                var pLV = $('#lstContactPersonMobile'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrContactPersonMobileInfoList.length; i++) {
                    if (arrContactPersonMobileInfoList[i].MobileState != "Remove") {
                        isEmpty = false;
                        arrContactPersonMobileInfoList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .data(arrContactPersonMobileInfoList[i])
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .attr('ID', arrContactPersonMobileInfoList[i].id)
                                                .html("+" + $.trim(arrContactPersonMobileInfoList[i].CountryCode.split('-')[1]) + "" + (arrContactPersonMobileInfoList[i].MobileNo.length <= 7 ? +arrContactPersonMobileInfoList[i].Prefix : '') + $.trim(arrContactPersonMobileInfoList[i].MobileNo) +
                                                    "<br>" + $.trim(arrContactPersonMobileInfoList[i].ContactType)
                                                )
                                                .data(arrContactPersonMobileInfoList[i])
                                                .click(function (e) {
                                                    EditContactPersonMobileInfo($(this).data().arrindex);
                                                })
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popContactPersonMobileInfo').modal('hide');
                $('#popContactPersonProfile').modal('show');
            }

            function EditContactPersonMobileInfo(arrindex) {
                $('#popContactPersonProfile').modal('hide');
                $('#popContactPersonMobileInfo').modal('show');
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
                $('#txtContactPersonMobileNo').val((arrContactPersonMobileInfoList[arrindex].MobileNo.length <= 7 ? +arrContactPersonMobileInfoList[arrindex].Prefix : '') + arrContactPersonMobileInfoList[arrindex].MobileNo);
                $('#selContactPersonMobileType').val(arrContactPersonMobileInfoList[arrindex].ContactType);//.selectmenu('refresh');
            }

            function ClearContactPersonMobileInfo() {
                $('#txtContactPersonMobileID').val('');
                $('#txtContactPersonTelCoPrefixID').val('');
                $('#txtContactPersonMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtContactPersonMobileCountryCode').text('PHL - 63');
                //$('#txtContactPersonTelCode').text('Select');
                $('#txtContactPersonMobileNo').val('');
                $('#selContactPersonMobileType').val('');//.selectmenu('refresh');
                $('#divContactPersonMobileHome').pageshow();
            }

            function AddContactPersonTelInfo() {

                var pLV = $('#lstContactPersonTelephone'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrContactPersonTelInfoList.length; i++) {
                    if (arrContactPersonTelInfoList[i].TelState != "Remove") {
                        isEmpty = false;
                        arrContactPersonTelInfoList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .data(arrContactPersonTelInfoList[i])
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
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
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }

                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popContactPersonTelInfo').modal('hide');
                $('#popContactPersonProfile').modal('show');
            }

            function EditContactPersonTelInfo(arrindex) {
                $('#popContactPersonProfile').modal('hide');
                $('#popContactPersonTelInfo').modal('show');

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
                $('#selContactPersonTelType').val(arrContactPersonTelInfoList[arrindex].ContactType);//.selectmenu('refresh');
            }

            function ClearContactPersonTelInfo() {
                $('#txtContactPersonTelID').val('');
                $('#txtContactPersonTelAreaCodeID').val('');
                $('#txtContactPersonTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtContactPersonCountryCode').text('PHL - 63');
                $('#txtContactPersonAreaCode').text('Select');
                $('#txtContactPersonTelephoneNum').val('');
                $('#txtContactPersonTelephoneExt').val('');
                $('#selContactPersonTelType').val('');//.selectmenu('refresh');
                $('#divContactPersonTel').pageshow();
            }

            function DisplayContactPersonUrl(data) {
                if (data.length > 0) {
                    $('#divForGeneratedCPLink').pageshow();
                    var url;

                    if (sessionStorage.LibraryName == 'Customer / Client List') {
                        url = location.origin + '/Ecommerce/PreCustomer?id=' + data[0].Link + '&mod=Customer' + '&type=Org&pid=' + arrContactPersonList[CInt2(sessionStorage["Contactindex"])].PeopleID;
                    } else {
                        url = location.origin + '/Ecommerce/PreSupplier?id=' + data[0].Link + '&mod=Supplier' + '&type=Org&pid=' + arrContactPersonList[CInt2(sessionStorage["Contactindex"])].PeopleID;
                    }
                    $('#txtCPLink').val(url);
                } else {
                    $('#divForGenerateCPLink').pageshow();
                }
            }

            function AddContactPersonInfo() {
                var pLV = $('#lstOrgContactPerson'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrContactPersonList.length; i++) {
                    if (arrContactPersonList[i].ContactPersonState != "Remove") {
                        isEmpty = false;
                        arrContactPersonList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .data(arrContactPersonList[i])
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .data(arrContactPersonList[i])
                                                .html($.trim(arrContactPersonList[i].Firstname) + ' ' +
                                                    $.trim(arrContactPersonList[i].Middlename) + ' ' +
                                                    $.trim(arrContactPersonList[i].Lastname) +
                                                    ($.trim(arrContactPersonList[i].Position) == '' ? '' : '<br> Position: ' + $.trim(arrContactPersonList[i].Position))
                                                )
                                                .click(function (e) {
                                                    EditContactPersonInfo($(this).data().arrindex);
                                                })
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash" aria-hidden="true">'))
                                                    .data('data', arrContactPersonList[i])
                                                    .click(function (e) {
                                                        var ContactPersonRankNo = $(this).data('data').ContactPersonRankNo;

                                                        if ($(this).data('data').IsNew == 1) {
                                                            //remove selected person
                                                            arrContactPersonList = $.grep(arrContactPersonList, function (val) {
                                                                return val.ContactPersonRankNo != ContactPersonRankNo;
                                                            })
                                                        }
                                                        else {
                                                            arrContactPersonList[$(this).data('data').arrindex].ContactPersonState = "Remove";
                                                        }

                                                        AddContactPersonInfo();
                                                    })
                                                )
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popContactPersonProfile').modal('hide');

                ClearContactPersonFields();
            }

            function EditContactPersonInfo(arrindex) {
                ClearContactPersonFields();

                $('#popContactPersonProfile').modal('show');
                $('#tabContactPersonProfile').opentabmenu();
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
                $('#selContactPersonCity').val(arrContactPersonList[arrindex].City);//.selectmenu('refresh');
                $('#selContactPersonCountry').val((arrContactPersonList[arrindex].Country || 'e783ed13-aa98-4cc1-918e-d49b05c0167e'));//.selectmenu('refresh');
                $('#selContactPersonTypeAddress').val(arrContactPersonList[arrindex].TypeAddress);//.selectmenu('refresh');

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
                                arrContactPersonList[arrindex].Url = vData.Url;
                                AddContactPersonTelInfo();
                                AddContactPersonEmailInfo();
                                AddContactPersonMobileInfo();
                                DisplayContactPersonUrl(vData.Url);
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
                        DisplayContactPersonUrl((arrContactPersonList[arrindex].Url || []));
                    }
                }
                else {
                    arrContactPersonTelInfoList = (arrContactPersonList[arrindex].Tel || []);
                    arrContactPersonMobileInfoList = (arrContactPersonList[arrindex].Mobile || []);
                    arrContactPersonEmailInfoList = (arrContactPersonList[arrindex].Email || []);
                    AddContactPersonTelInfo();
                    AddContactPersonEmailInfo();
                    AddContactPersonMobileInfo();
                    DisplayContactPersonUrl((arrContactPersonList[arrindex].Url || []));
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
                $('#selContactPersonCity').val('');//.selectmenu('refresh');
                $('#selContactPersonCountry').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');//.selectmenu('refresh');
                $('#selContactPersonTypeAddress').val();
                $('#lstContactPersonTelephone').empty();
                $('#lstContactPersonMobile').empty();
                $('#lstContactPersonEmail').empty();
                $('#divForGenerateCPLink').pageshow();
                $('#tabContactPersonProfile').opentabmenu();

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

            function DisplaySalesReps() {
                var pLV = $('#lstOrgSalesRep'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrSalesReps.length; i++) {
                    var pData = arrSalesReps[i];
                    if (pData.State != "Remove") {
                        isEmpty = false;
//                        arrContactPersonList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                            .data(pData)
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">')
                                        .data(pData)
                                        .html($.trim(pData.PeopleName))
                                        .click(function (e) {
                                            EditContactPersonInfo($(this).data().arrindex);
                                        })
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                        .append($('<span class="bi-trash" aria-hidden="true">'))
                                        .data('data', pData)
                                        .click(function (e) {
                                            var pClickedData = $(this).data('data');

                                            if (pClickedData.State == 'New') 
                                                //remove selected person
                                                arrSalesReps = $.grep(arrSalesReps, function (val) {
                                                    return ReadyGuid(val.PeopleID) != ReadyGuid(pClickedData.PeopleID);
                                                })
                                            else 
                                                pClickedData.State = "Remove";
                                            DisplaySalesReps();
                                        })
                                    )
                                    )
                                )
                            )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
            }

            function AddWebsiteInfo() {

                var pLV = $('#lstOrgWebsite'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrWebsiteList.length; i++) {
                    if (arrWebsiteList[i].WebsiteState != "Remove") {
                        isEmpty = false;
                        arrWebsiteList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .data(arrWebsiteList[i])
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .data(arrWebsiteList[i])
                                                .html($.trim(arrWebsiteList[i].Website))
                                                .click(function (e) {
                                                    EditWebsiteInfo($(this).data().arrindex);
                                                })
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popOrgWebsiteInfo').modal('hide');
            }

            function EditWebsiteInfo(arrindex) {
                $('#popOrgWebsiteInfo').modal('show');
                WebsiteRankNo = arrWebsiteList[arrindex].WebsiteRankNo;
                WebsiteState = "Edit";
                sessionStorage["Websiteindex"] = arrindex;
                sessionStorage["WebsiteIsNew"] = arrWebsiteList[arrindex].IsNew;
                sessionStorage["WebsiteState"] = arrWebsiteList[arrindex].WebsiteState;
                $('#txtOrgWebsiteID').val(arrWebsiteList[arrindex].WebsiteID == undefined ? "" : arrWebsiteList[arrindex].WebsiteID);
                $('#txtWebsiteUrl').val(arrWebsiteList[arrindex].Website);
            }

            function ClearWebsiteFields() {
                $('#txtOrgWebsite').val('');
                $('#txtWebsiteUrl').val('');
            }

            function AddEmailInfo() {

                var pLV = $('#lstOrgEmail'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrEmailInfoList.length; i++) {
                    if (arrEmailInfoList[i].EmailState != "Remove") {
                        isEmpty = false;
                        arrEmailInfoList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .data(arrEmailInfoList[i])
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .data(arrEmailInfoList[i])
                                                .html($.trim(arrEmailInfoList[i].EmailAdd) +
                                                    "<br>" + $.trim(arrEmailInfoList[i].EmailType)
                                                )
                                                .click(function (e) {
                                                    EditEmailInfo($(this).data().arrindex);
                                                })
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popOrgEmailInfo').modal('hide');
            }

            function EditEmailInfo(arrindex) {
                $('#popOrgEmailInfo').modal('show');
                EmailRankNo = arrEmailInfoList[arrindex].EmailRankNo;
                EmailState = "Edit";
                sessionStorage["Emailindex"] = arrindex;
                sessionStorage["EmailIsNew"] = arrEmailInfoList[arrindex].IsNew;
                sessionStorage["EmailState"] = arrEmailInfoList[arrindex].EmailState;
                $('#txtOrgEmailID').val(arrEmailInfoList[arrindex].EmailAddID == undefined ? "" : arrEmailInfoList[arrindex].EmailAddID);
                $('#txtEmailAdd').val(arrEmailInfoList[arrindex].EmailAdd);
                $('#selOrgEmailType').val(arrEmailInfoList[arrindex].EmailType);//.selectmenu('refresh');
            }

            function ClearEmailInfoFields() {
                $('#txtOrgEmailID').val('');
                $('#txtEmailAdd').val('');
                $('#selOrgEmailType').val('');//.selectmenu('refresh');
            }

            function AddMobileInfo() {

                var pLV = $('#lstOrgMobile'), isEmpty = true;
                var pItem;
                pLV.html('');
                for (var i = 0; i < arrMobileInfoList.length; i++) {
                    if (arrMobileInfoList[i].MobileState != "Remove") {
                        isEmpty = false;
                        arrMobileInfoList[i].arrindex = i;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .data(arrMobileInfoList[i])
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .data(arrMobileInfoList[i])
                                                .attr('ID', arrMobileInfoList[i].id)
                                                .html("+" + $.trim(arrMobileInfoList[i].CountryCode.split('-')[1]) + "" + (arrMobileInfoList[i].MobileNo.length <= 7 ? +arrMobileInfoList[i].Prefix : '') + $.trim(arrMobileInfoList[i].MobileNo) +
                                                    "<br>" + $.trim(arrMobileInfoList[i].ContactType)
                                                )
                                                .click(function (e) {
                                                    EditMobileInfo($(this).data().arrindex);
                                                })
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popOrgMobileInfo').modal('hide');
            }

            function EditMobileInfo(arrindex) {
                $('#popOrgMobileInfo').modal('show');
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
                $('#txtMobileNo').val((arrMobileInfoList[arrindex].MobileNo.length <= 7 ? +arrMobileInfoList[arrindex].Prefix : '') + arrMobileInfoList[arrindex].MobileNo);
                $('#selOrgMobileType').val(arrMobileInfoList[arrindex].ContactType);//.selectmenu('refresh');
            }

            function ClearMobileInfo() {
                $('#txtOrgMobileID').val('');
                $('#txtOrgTelCoPrefixID').val('');
                $('#txtOrgMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtMobileCountryCode').text('PHL - 63');
                //$('#txtTelCode').text('Select');
                $('#txtMobileNo').val('');
                $('#selOrgMobileType').val('');//.selectmenu('refresh');
            }

            function AddTelInfo() {

                var pLV = $('#lstOrgTelephone'), isEmpty = true;
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrTelInfoList.length; i++) {
                    if (CStr2(arrTelInfoList[i].CountryCodeID) != "") {
                        if (arrTelInfoList[i].TelState != "Remove") {
                            isEmpty = false;
                            arrTelInfoList[i].arrindex = i;
                            pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                        .data(arrTelInfoList[i])
                                        .append($('<table style="width:100%;">')
                                            .append($('<tr>')
                                                .append($('<td style="word-break:break-all;">')
                                                    .html((CStr2(arrTelInfoList[i].CountryCode).split('-')[1] == undefined ? "" : "+" + $.trim(CStr2(arrTelInfoList[i].CountryCode).split('-')[1])) + " (" + (CStr2(arrTelInfoList[i].AreaCode).split('-')[1] == undefined ? $.trim(CStr2(arrTelInfoList[i].AreaCode).split('-')[0]) : $.trim(CStr2(arrTelInfoList[i].AreaCode).split('-')[1])) + ") " + $.trim(arrTelInfoList[i].TelephoneNo) + ($.trim(arrTelInfoList[i].ExtensionNo) == "" ? "" : "." + $.trim(arrTelInfoList[i].ExtensionNo)) +
                                                        "<br>" + $.trim(CStr2(arrTelInfoList[i].AreaCode).split('-')[0]) + ', ' + $.trim(CStr2(arrTelInfoList[i].CountryCode).split('-')[0]) +
                                                        "<br>" + $.trim(arrTelInfoList[i].ContactType)
                                                    )
                                                    .data('data', arrTelInfoList[i])
                                                    .click(function (e) {
                                                        EditTelInfo($(this).data('data').arrindex);
                                                    })
                                                )
                                                .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                        .append($('<span class="bi-trash" aria-hidden="true">'))
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
                                                )
                                            )
                                        )

                            pLV.append(pItem);
                        }
                    } else {
                        arrTelInfoList[i].TelState = 'Remove';
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
                $('#popOrgTelInfo').modal('hide');
            }

            function EditTelInfo(arrindex) {
                $('#popOrgTelInfo').modal('show');

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
                $('#selOrgTelType').val(arrTelInfoList[arrindex].ContactType);//.selectmenu('refresh');
            }

            function ClearTelInfo() {
                $('#txtOrgTelID').val('');
                $('#txtOrgTelAreaCodeID').val('');
                $('#txtOrgTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtCountryCode').text('PHL - 63');
                $('#txtAreaCode').text('Select');
                $('#txtTelephoneNum').val('');
                $('#txtTelephoneExt').val('');
                $('#selOrgTelType').val('');//.selectmenu('refresh');
            }

            function LoadRequiredData() {
                $('.OrganizationNameHeader').text(sessionStorage.OrgName);
                
                sessionStorage.CRMDashboardPage = 'leadgen'; // added only in sysadmin mcr
                if (sessionStorage.CRMDashboardPage == 'leadgen') {
                    $('#divtblLeadGen').pageshow();
                    $('#divCustomerMainPage').pageshow();
                }
                else {
                    $('#divtblSalesGrinder').pageshow();
                }
                //$('#divCalendar').customcalendar(); DISABLE
                setRegionCollection('#selProspectOrgRegion');
                setRegionCollection('#selPersonRegion');
                //GetProspectList();
                //CacheBusinessIndustry();
                
                DisplayCities();

                if (sessionStorage.LibraryName == 'Customer / Client List') {
                    $('#pageCustomerMainDashboard').pageshow();
                    $('[href="#tabRevenue"]').removeClass('hidetab')
                    $('[href="#tabDisbursement"]').addClass('hidetab')
                    $('[href="#tabVendorPortal"]').addClass('hidetab')
                    $('[href="#tabAttachments"]').addClass('hidetab')
                    $('[href="#tabVendorSKUs"]').addClass('hidetab')
                    ShowProspectList();
                    GetLibraryHotButtonTemplate();
                } else {
                    $('#pageActions, #pageProfile').pageshow();
                    $('[href="#tabRevenue"]').addClass('hidetab')
                    $('[href="#tabSalesReps"]').addClass('hidetab')
                    $('[href="#tabDisbursement"]').removeClass('hidetab')
                    $('[href="#tabVendorPortal"]').removeClass('hidetab')
                    $('[href="#tabAttachments"]').removeClass('hidetab')
                    $('[href="#tabVendorSKUs"]').removeClass('hidetab')

                    if (sessionStorage.SupplierType == 'Org') {
                        $('#pageProfileOrg').pageshow();
                        $('#tabRevenue').appendTo($('#divCompanyProfile').find('.tab-content'));
                        GetProspectDetails(sessionStorage.ViewID);
                    } else {
                        $('#pageProfilePeople').pageshow();
                        GetProspectDetailsPeople(sessionStorage.ViewID);
                    }

                    GetGeneratedPublicUrl();
                }
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

                        selFilterByBusinessIndustry;//.selectmenu('refresh');
                    }
                })
            }

            function ChangePage(pageid) {
                $('div[custom-data-role="page"]').not(pageid).hide();
                $(pageid).show();
                $(window).scrollTop(0);
                //$('#mainContainer').css('margin-top', $('#divHeaderForProjects').height());
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

                        selProspectCities;//.selectmenu('refresh');
                        selPersonCity;//.selectmenu('refresh');
                        selContactPersonCity;//.selectmenu('refresh');
                        selOutletCity;//.selectmenu('refresh');
                        selFilterByCity;//.selectmenu('refresh');
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

                        selProspectCountries.val('e783ed13-aa98-4cc1-918e-d49b05c0167e');//.selectmenu('refresh');
                        selPersonCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e');//.selectmenu('refresh');
                        selContactPersonCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e');//.selectmenu('refresh');
                        selOutletCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e');//.selectmenu('refresh');
                    }
                });
            }

            function GetProspectList() {
                $('#divLoading').show();
                $('#divMainPage').hide();

                AjaxSend({
                    url: 'CRMDashboard/IsNewEntry',
                    success: function (vData) {
                        if (vData.PeopleID == '') { //if people ID is blank, then redirect to sign in
                            $('#divLoading').text('Your session has expired. Redirecting to sign in page ...');
                            setTimeout(function () {
                                pUrl = '/SignIn/CRM';
                                window.open(GetPathFormat(pUrl), '_self');
                            }, 3000);
                        }
                        else {
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
                                        $('#divLoading').show();
                                        $('#divMainPage').hide();
                                    } else {
                                        $('#divLoading').hide();
                                        $('#divMainPage').show();
                                        $('#mainContainer').css('margin-top', $('#divHeaderForProjects').height());
                                        ShowProspectList();
                                        DisplayCities();
                                    }
                                }
                            });
                        }
                    }
                })
            }

            function ShowProspectList() {
                // if(sessionStorage.OrgID != '224a502f-8d4e-4a9b-8ab4-3c57c47c3fe7'){
                //     $('#lisAddExcelProspect').hide();
                //     $('#lisDownloadExcelProspect').hide();
                // }

                if (sessionStorage.CRMDashboardPage == 'leadgen') {
                    $('#tblDataTableNewCustomer').DataTable().destroy();
                    datatable = $('#tblDataTableNewCustomer').DataTable({
                        retrieve: true,
                        "pageLength": 50,
                        "order": [[1, "asc"]],
                        "bProcessing": true,
                        "bServerSide": true,
                        "ajax": {
                            url: "/OrganizationShort/GetProspectList?OrgID=" + sessionStorage.OrgID,
                            type: 'POST'
                        },
                        "autoWidth": false,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    return data.RowNum + '.';
                                },
                                "width": "20px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    if (data.ProspectType == "Org") {
                                        return "<button style='float:left;' class='btn btn-warning'>O</button>";
                                    }
                                    else {
                                        return "<button style='float:left;' class='btn btn-success'>P</button>";
                                    }
                                },
                                "width": "30px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Customer Code</b>' + (data.CustomerCode || '');
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Name</b>' + CStr2(data.ClientName);
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">TIN</b>' + (data.TinNumber || '');
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Business Industry</b>' + (data.IndustryName || '');
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Contact Info</b>' + (data.Telephone != undefined ? 'T: ' + data.Telephone + '<br>' : '') +
                                                    (data.Mobile != undefined ? 'M: ' + data.Mobile + '<br>' : '') +
                                                    (data.Email != undefined ? 'E: ' + data.Email : '')
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Address</b>' + (data.HouseNo != "" ? data.HouseNo + ', <br>' : '') +
                                            (data.Street != "" ? data.Street + ', <br>' : '') +
                                            (data.City != "" ? data.City + ', <br>' : '') +
                                            (data.Region != "" ? data.Region + ', <br>' : '') +
                                            (data.Country != "" ? data.Country : '')
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Sales Reps</b>' + CStr2(data.SalesReps);
                                }
                            },
                            //{
                            //    "data": function (data, type, full, meta) {
                            //        var listAffiliation = $('<ol style="margin:0 auto;">');
                            //        var ctrAffiliation = 0;

                            //        if (data.Affiliation != null) {
                            //            var affiliationLength = data.Affiliation.split(',').length;
                            //            for (var m = 0; m < affiliationLength; m++) {
                            //                if (m < 2) {
                            //                    listAffiliation.append($('<li>').text(data.Affiliation.split(',')[m]));
                            //                }
                            //                else if (m == (affiliationLength - 1)) {
                            //                    ctrAffiliation = affiliationLength - 2;
                            //                    listAffiliation.append($('<span>')
                            //                            .css('color', '#3745FD')
                            //                            .data(data)
                            //                            .text('+' + ctrAffiliation + ' more')
                            //                            .click(function (e) {
                            //                                e.stopPropagation();
                            //                                //backup 2-22-2016
                            //                            })
                            //                        );
                            //                }
                            //            }
                            //        }
                            //        return '<b class="ui-table-cell-label">Organization Affiliation</b>' + listAffiliation.get(0).outerHTML;
                            //    }
                            //},
                            {
                                "data": function (data, type, full, meta) {
                                    var LastUpdated = data.LastUpdated;
                                    if(LastUpdated == undefined){
                                        return "";
                                    }
                                    return '<b class="ui-table-cell-label">Date Created/Modified</b>' + $.format.date(new Date([LastUpdated.slice(0, -2), ' ', LastUpdated.substr(LastUpdated.length - 2)].join('')), 'MMM dd, yyyy hh:mm a') + '<br> <span style="font-size:12px;">' + (CStr2(data.ModifiedBy) == "" ? "" : "by " + data.ModifiedBy) + '</span>';
                                }
                            }
                            //,{
                            //    "data": function (data, type, full, meta) {
                            //        if (data.LastInteractionDate == null) {
                            //            return "";
                            //        }
                            //        var LastInteractionDate = data.LastInteractionDate;
                            //        return '<b class="ui-table-cell-label">Last Interaction Date</b>' + $.format.date(new Date([LastInteractionDate.slice(0, -2), ' ', LastInteractionDate.substr(LastInteractionDate.length - 2)].join('')), 'MMM dd, yyyy hh:mm a');
                            //    }
                            //}
                        ],
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": [0, 3, 4, 5] }],
                        "responsive": true,
                        "columnDefs": [{ "targets": 0, "orderable": true }],
                        "sDom": '<"top">rt<"bottom"ip><"clear">',
                        "initComplete": function () {
                            $('#tblDataTableNewCustomer').removeClass('dataTable');
                        }
                        //"stateSave": true
                    })

                    $('#tblDataTableNewCustomer tbody').on('click', 'tr', function () {
                        if (datatable.row(this).data() != undefined) {
                            //if (datatable.row(this).data().ID.toLowerCase() == 'cd71a0f7-08a9-4601-876e-5f13c064f873') {
                            //    NewMessageBox('Sorry! You are not allowed to edit default customer details.', 'Message');
                            //} else {                                
                                ClearOrgProspectVariablesFields();
                                MainFormOptions(datatable.row(this).data());
                            //}
                        }
                    });

                    $("div.dataTables_filter input").unbind().keypress(function (e) {
                        if (e.keyCode == 13) {
                            datatable.search($.trim($(this).val())).draw();
                        }
                    });

                    $('#selStatus').unbind('change').change(function () {
                        datatable.columns(0)
                        .search($(this).val())
                        .draw()
                    })
                }
                else {
                    $('#tblDataTableSalesGrinder').DataTable().destroy();
                    datatable = $('#tblDataTableSalesGrinder').DataTable({
                        retrieve: true,
                        "order": [[4, "desc"]],
                        "bProcessing": true,
                        "bServerSide": true,
                        "ajax": {
                            url: "/CRMDashboard/GetProspectList?OrgID=" + sessionStorage.OrgID,
                            type: 'POST'
                        },
                        "autoWidth": false,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    if (data.ProspectType == "Org") {
                                        return "<button style='float:left;' class='custom-theme-e btnCircle-xs noshadow'>O</button>";
                                    }
                                    else {
                                        return "<button style='float:left;' class='custom-theme-b btnCircle-xs noshadow'>P</button>";
                                    }
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Name</b>' + data.ClientName;
                                },
                                "width": "200px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Business Industry</b>' + (data.IndustryName || '');
                                },
                                "width": "100px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    var listContactPerson = $('<ol style="margin:0 auto; padding:0px;">');
                                    var ctrContactPerson = 0;

                                    if (data.ContactPerson != undefined && data.ContactPerson != '') {
                                        var contactPersonLength = data.ContactPerson.split(',').length;
                                        for (var m = 0; m < contactPersonLength; m++) {
                                            if (m < 2) {
                                                listContactPerson.append($('<li>').text(data.ContactPerson.split(',')[m]));
                                            }
                                            else if (m == (contactPersonLength - 1)) {
                                                ctrContactPerson = contactPersonLength - 2;
                                                listContactPerson.append('<span style="color:#3745FD;">+' + ctrContactPerson + ' more</span>');
                                            }
                                        }
                                    }
                                    return '<b class="ui-table-cell-label">Contact Person</b>' + listContactPerson.get(0).outerHTML;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Gross Sales This Year</b>' + AccountingFormat(data.GrossSales)
                                },
                                "width": "200px"
                            }
                        ],
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": [3] }, { className: "dt-body-right", "targets": [4] }],
                        "responsive": true,
                        "columnDefs": [{ "targets": 0, "orderable": true }],
                        "sDom": '<"top"lf>rt<"bottom"ip><"clear">'
                        //"stateSave": true
                    })
                    $('#tblDataTableSalesGrinder tbody').on('click', 'tr', function () {
                        if (datatable.row(this).data() != undefined) {
                            ClearOrgProspectVariablesFields();
                            MainFormOptions(datatable.row(this).data());
                        }
                    });

                    $("div.dataTables_filter input").unbind().keypress(function (e) {
                        if (e.keyCode == 13) {
                            datatable.search($.trim($(this).val())).draw();
                        }
                    });
                }
            }

            function GetLibraryHotButtonTemplate() {
                AjaxSend({
                    url: 'Browser/LibraryHotButtonTemplate',
                    success: function (e) {
                        var vRows = e.Results.Rows;
                        var selDefaultHBTemplate = $('#selDefaultHBTemplate');

                        selDefaultHBTemplate.empty().append($('<option value="">').text('All Hot Button Templates'));

                        for (var i = 0; i < vRows.length; i++) {
                            selDefaultHBTemplate.append($('<option value="' + vRows[i].ID + '">').text(vRows[i].Name));
                        }                                                
                    }
                })
            }

            function SearchClientContact(searchString) {
                var tblProspectbody = $('#tblProspectbody');
                arrOrgPeopleCitiesList = [];

                //$('.bulk').hide();
                $('#chbbulk').prop('checked', false);//.checkboxradio('refresh');
                tblProspectbody.empty().append('<tr><td colspan="20" style="text-align:center;">Please wait ...</td></tr>')

                AjaxSend({
                    url: 'CRMDashboard/SearchClientContact',
                    data: { SearchContact: searchString },
                    success: function (vData) {
                        var vRows = vData.data;
                        var ctrRows = 0;
                        tblProspectbody.empty();
                        if (vRows.length > 0) {
                            $('#lnkSelectAllSearchRecords').text((vData.recordcount > 1 ? 'Select all ' + vData.recordcount + ' records' : 'Select all records'))
                                .unbind('click')
                                .removeData()
                                .data(vData.records)
                                .click(function () {
                                    for (var i = 0; i < vData.records.length; i++) {
                                        var isExist = false;

                                        if (arrSMSRecipient.length > 0) {
                                            if (vData.records[i].IsFromOrg) {
                                                $.grep(arrSMSRecipient, function (e) {
                                                    if (e.CPID == vData.records[i].ID) {
                                                        isExist = true;
                                                    }
                                                })
                                            }
                                            else {
                                                $.grep(arrSMSRecipient, function (e) {
                                                    if (e.ID == vData.records[i].ID) {
                                                        isExist = true;
                                                    }
                                                })
                                            }
                                        }

                                        if (isExist == false) {
                                            if (vData.records[i].MobileNos != null) {
                                                AddSelectedRecipient(vData.records[i], true)
                                            }
                                        }
                                    }
                                    DisplayListOfReceipient();
                                    $('#popSearchContacts').modal('hide');
                                    $('#popSMS').modal('show');
                                })
                            for (var i = 0; i < vRows.length; i++) {

                                tblProspectbody.append($('<tr>')
                                    .attr('prostype', (vRows[i].ProspectType == "Org" ? 'o' : 'p'))
                                    .css('cursor', 'pointer')
                                    .data(vRows[i])
                                    .append($('<td>')
                                        .addClass('bulk')
                                        .append($('<label class="checkbox-only" for="chbbulk-' + vRows[i].ID + '">')
                                                .append($('<input type="checkbox" name="chbbulk" id="chbbulk-' + vRows[i].ID + '" data-iconpos="notext"/>')
                                                    .data(vRows[i])
                                                    .click(function () {
                                                        $(this);//.checkboxradio('refresh');
                                                        AddSelectedRecipient($(this).data(), $(this).is(':checked'));
                                                        DisplayListOfReceipient();
                                                        //$('#popSMS, #popSMSRecipient').hide();
                                                    })
                                                )
                                            )
                                        .click(function (e) {
                                            e.stopPropagation();
                                        })
                                    )
                                    .append($('<td>')
                                        .text(i + 1 + '.')
                                    )
                                    .append($('<td>')
                                        .html((vRows[i].ProspectType == "Org" ? '<button style="float:left;" class="custom-theme-e btnCircle-xs noshadow">O</button>' : '<button style="float:left;" class="custom-theme-b btnCircle-xs noshadow">P</button>'))
                                    )
                                    .append($('<td>')
                                        .text((vRows[i].ClientName || ''))
                                    )
                                    .click(function () {
                                        if (isSendingBulkMsg == true) {
                                            return false;
                                        }
                                        ClearOrgProspectVariablesFields();
                                        MainFormOptions($(this).data());
                                    })
                                )
                                ctrRows = ctrRows + 1;
                                $('input[type="checkbox"]');//.checkboxradio();//.checkboxradio('refresh');
                                if (arrSMSRecipient.length > 0) {
                                    if (vRows[i].IsFromOrg) {
                                        $.grep(arrSMSRecipient, function (e) {
                                            if (e.CPID == vRows[i].ID) {
                                                $('#chbbulk-' + vRows[i].ID).prop('checked', true);//.checkboxradio('refresh');
                                            }
                                        })
                                    }
                                    else {
                                        $.grep(arrSMSRecipient, function (e) {
                                            if (e.ID == vRows[i].ID) {
                                                $('#chbbulk-' + vRows[i].ID).prop('checked', true);//.checkboxradio('refresh');
                                            }
                                        })
                                    }
                                }
                            }
                            //$('#divCountRow').text('No. of Rows: ' + ctrRows);
                        }
                        else {
                            tblProspectbody.append('<tr><td colspan="20" style="text-align:center;">No Result(s) Found</td></tr>')
                        }

                        tblProspectbody.parent();//.table('refresh');
                    }
                });
            }

            function AddSelectedRecipient(dis, isCheck) {
                var id = dis.ID;

                if (arrSMSRecipient.length > 0) {
                    if (isCheck) {
                        ValidateSelectedRecipient(dis);
                    }
                    else {
                        if (dis.IsFromOrg) {
                            arrSMSRecipient = $.grep(arrSMSRecipient, function (e) {
                                return e.CPID != id;
                            })
                        }
                        else {
                            arrSMSRecipient = $.grep(arrSMSRecipient, function (e) {
                                return e.ID != id;
                            })
                        }
                    }
                }
                else {
                    ValidateSelectedRecipient(dis);
                }
            }

            function ValidateSelectedRecipient(dis) {
                var id = dis.ID;
                var mobileno = dis.MobileNos;
                var arrmobileno = [];
                var splitno;
                var contactperson = (dis.ContactPerson == null ? [] : dis.ContactPerson);

                if (dis.IsFromOrg) {
                    dis.IsCP = true;
                    dis.ContactPersonID = dis.ID;
                    dis.ID = dis.CPOrgID; // para sa org history sya masave
                    if (dis.MobileNos != "") {
                        MakeArrayForSelectedRecipient(dis);
                    }
                }
                else {
                    dis.IsCP = false;
                    if (mobileno != "") {
                        MakeArrayForSelectedRecipient(dis);
                    }
                }

                if (contactperson.length > 0) {
                    for (var i = 0; i < contactperson.length; i++) {
                        var iscpidexist = false;
                        iscpidexist = $.grep(arrSMSRecipient, function (e) {
                            if (e.CPID == contactperson[i].PeopleID) {
                                return true;
                            }
                        })

                        if (iscpidexist == false) {
                            contactperson[i].IsCP = true;
                            contactperson[i].ClientName = $.trim(contactperson[i].FullName);
                            contactperson[i].ContactPersonID = contactperson[i].PeopleID;
                            contactperson[i].ID = id; //must retain the orgid so that it will log on its history
                            if (contactperson[i].MobileNos != null) {
                                MakeArrayForSelectedRecipient(contactperson[i]);
                            }
                        }
                    }
                }
            }

            function MainFormOptions(data) {                
                $('#listAction').find('li > a:not(:first)').removeClass('disabled-menu');
                isAddNewProspect = false;
                isViewReadOnlyProfile = true;
                sessionStorage.ClientInfo = JSON.stringify(data);
                sessionStorage.ViewID = data.ID;
                if (data.ProspectType == "Org") {
                    GetProspectDetails(data.ID);
                }
                else {
                    GetProspectDetailsPeople(data.ID);
                }
                return;
                PopupButtonList('', 'Options',
                        [
                            {
                                Text: 'View Profile',
                                data: data,
                                Click: function (e) {
                                    sessionStorage.ClientInfo = JSON.stringify(e);
                                    $('#pageActions').pageshow();
                                    $('#btnPageProfile').click();
                                    //BreadCrumbs('.ItemsBreadCrumbs',
                                    //        [
                                    //            {Text: 'Lead Generation', Click: function() {
                                    //                $('#pageMainDashboard').pageshow();
                                    //            }},
                                    //            {Text: e.ClientName + ' Profile'}
                                    //        ]
                                    //    );
                                    //if(e.ProspectType == "Org"){
                                    //    GetProspectDetails(e.ID);
                                    //    $('#pageAddOrgProspect').pageshow()
                                    //}
                                    //else {
                                    //    GetProspectDetailsPeople(e.ID);
                                    //    $('#pageAddPersonProspect').pageshow()
                                    //}                                
                                }
                            },
                            {
                                Text: 'Cold Call',
                                data: data,
                                Click: function (e) {
                                    sessionStorage.ClientInfo = JSON.stringify(e);
                                    $('#btnPageColdCall').click();
                                }
                            },
                            {
                                Text: 'To-Do',
                                data: data,
                                Click: function (e) {
                                    sessionStorage.ClientInfo = JSON.stringify(e);
                                    $('#pageActions').pageshow();
                                    $('#btnPageToDo').click();
                                }
                            },
                            {
                                Text: 'Send SMS',
                                data: data,
                                Click: function (e) {
                                    if (e.MobileNos == undefined && e.ContactPerson == null) {
                                        NewMessageBox('No mobile number found', 'Send SMS');
                                        return false;
                                    }
                                    sessionStorage.ClientInfo = JSON.stringify(e);
                                    $('#pageActions').pageshow();
                                    $('#btnPageSMS').click();
                                }
                            },
                            {
                                Text: 'Interaction History',
                                data: data,
                                Click: function (e) {
                                    sessionStorage.ClientInfo = JSON.stringify(e);
                                    $('#pageActions').pageshow();
                                    $('#btnPageInteractionHistory').click();
                                }
                            },
                            {
                                Text: 'Take a Note',
                                data: data,
                                Click: function (e) {
                                    sessionStorage.ClientInfo = JSON.stringify(e);
                                    $('#pageActions').pageshow();
                                    $('#btnPageTakeANote').click();
                                    $('#txtTakeANote').val();
                                }
                            },
                            {
                                Text: 'Schedule an Action',
                                data: data,
                                Click: function (e) {
                                    sessionStorage.ClientInfo = JSON.stringify(e);
                                    $('#pageActions').pageshow();
                                    $('#btnPageScheduleAnAction').click();
                                }
                            }
                            //{
                            //    Text: 'Internal Sales And Marketing Info',
                            //    data: data,
                            //    Click: function (e) {
                            //        sessionStorage.ClientInfo = JSON.stringify(e);
                            //        $('#pageActions').pageshow();
                            //        $('#btnPageInternalSalesMarketing').click();
                            //    }
                            //}
                        ], { AutoDismiss: true, OverflowScroll: false }
                    )
            }

            function GetContactDetailsPeople(e) {
                AjaxSend({
                    url: 'CRMDashboard/GetContactDetailsPeople',
                    data: e,
                    success: function (vData) {
                        DisplayContactsColdCall(vData)
                    }
                })
            }

            function GetContactDetailsOrg(e) {
                AjaxSend({
                    url: 'CRMDashboard/GetContactDetailsOrg',
                    data: e,
                    success: function (vData) {
                        DisplayContactsColdCall(vData)
                    }
                })
            }

            function DisplayContactsColdCall(vData) {
                var vRowsTel = vData.Telephone;
                var vRowsMobile = vData.Mobile;
                var vRowsContactPerson = (vData.ContactPerson || []);
                var lstContactDetails = $('#lstContactDetails');
                var lstContactPerson = $('#lstContactPerson');
                var isGenerateQC = false;

                lstContactDetails.empty();
                lstContactPerson.empty();
                $('#sw_h, #sw_m, #sw_s, #sw_ms').text('00');
                $('#txtCPName').html('');

                for (var i = 0; i < vRowsTel.length; i++) {
                    if (vRowsTel[i].FullTelNo != null) {
                        if (isGenerateQC == false) {
                            MakeQRCode('TEL:' + vRowsTel[i].FullTelNo);
                            isGenerateQC = true;
                        }
                        lstContactDetails
                        .append($('<li>')
                            .css('margin-bottom', '10px')
                            .css('word-break', 'break-all')
                            .append($('<a data-role="none" class="custom-link">')
                                .data(vRowsTel[i])
                                .text(vRowsTel[i].FullTelNo)
                                .click(function () {
                                    MakeQRCode('TEL:' + $(this).data().FullTelNo);
                                })
                            )
                        )
                    }
                }

                for (var i = 0; i < vRowsMobile.length; i++) {
                    if (vRowsMobile[i].FullMobileNo != null) {
                        if (isGenerateQC == false) {
                            MakeQRCode('TEL:' + vRowsMobile[i].FullMobileNo);
                            isGenerateQC = true;
                        }
                        lstContactDetails
                        .append($('<li>')
                            .css('margin-bottom', '10px')
                            .css('word-break', 'break-all')
                            .append($('<a data-role="none" class="custom-link">')
                                .data(vRowsMobile[i])
                                .text(vRowsMobile[i].FullMobileNo)
                                .click(function () {
                                    MakeQRCode('TEL:' + $(this).data().FullMobileNo);
                                })
                            )
                        )
                    }
                }

                for (var i = 0; i < vRowsContactPerson.length; i++) {
                    lstContactPerson
                        .append($('<li>')
                            .css('margin-bottom', '10px')
                            .data(vRowsContactPerson[i])
                            .html('<a data-role="none" class="custom-link">' + vRowsContactPerson[i].FullName + '</a>')
                            .click(function () {
                                lstContactDetails.empty();
                                isGenerateQC = false;
                                var cptel = ($(this).data().TelNos == null ? [] : $(this).data().TelNos.split(','));
                                var cpmobile = ($(this).data().MobileNos == null ? [] : $(this).data().MobileNos.split(','))
                                $('#txtCPName').html($(this).data().FullName + ' #: <br>');
                                for (var i = 0; i < cptel.length; i++) {
                                    if (isGenerateQC == false) {
                                        MakeQRCode('TEL:' + cptel[i]);
                                        isGenerateQC = true;
                                    }
                                    lstContactDetails
                                    .append($('<li>')
                                        .css('margin-bottom', '10px')
                                        .css('word-break', 'break-all')
                                        .append($('<a data-role="none" class="custom-link">')
                                            .text(cptel[i])
                                            .click(function () {
                                                MakeQRCode('TEL:' + $(this).text());
                                            })
                                        )
                                    )
                                }

                                for (var i = 0; i < cpmobile.length; i++) {
                                    if (isGenerateQC == false) {
                                        MakeQRCode('TEL:' + cpmobile[i]);
                                        isGenerateQC = true;
                                    }
                                    lstContactDetails
                                    .append($('<li>')
                                        .css('margin-bottom', '10px')
                                        .css('word-break', 'break-all')
                                        .append($('<a data-role="none" class="custom-link">')
                                            .text(cpmobile[i])
                                            .click(function () {
                                                MakeQRCode('TEL:' + $(this).text());
                                            })
                                        )
                                    )
                                }
                            })
                        )
                }

                $('#btnCCName').unbind().click(function () {
                    DisplayContactsColdCall(vData);
                })
            }

            function MakeQRCode(e) {
                var qrcode;
                $('#divQrCodeTel').empty();

                qrcode = new QRCode(document.getElementById('divQrCodeTel'), {
                    width: 100,
                    height: 100
                });

                qrcode.makeCode(e);
            }

            function GetProspectDetails(id) {                
                ClearOrgProspectVariablesFields();

                if(CStr2(id) == ''){
                    return;
                }

                Bs.Loading('show');

                AjaxSend({
                    url: 'OrganizationShort/GetProspectDetails',
                    data: { ID: id, Type: sessionStorage.LabelLibraryName, IsReadOnly: CInt2(isViewReadOnlyProfile) },
                    success: function (vData) {
                        var vRowsOrgDetails = vData.OrgDetails;
                        arrContactPersonList = vData.ContactPerson;
                        arrEmailInfoList = vData.Email;
                        arrMobileInfoList = vData.Mobile;
                        arrTelInfoList = vData.Telephone;
                        arrWebsiteList = vData.Website;
                        arrOrgAffList = vData.Affiliation;
                        arrOrgAffPersonList = vData.AffiliatedPerson;
                        
                        function TableJsonToArray(vDT) {
                            if (vDT == null)
                                return [];
                            if ($.isArray(vDT.Rows))
                                return vDT.Rows;
                            return [];
                        }
                        arrSalesReps = TableJsonToArray(vData.SalesReps);

                        //arrCompanyOutlet = vData.Outlet;

                        intContactPersonCtr = vData.ContactPerson.length;
                        intTelInfoCtr = vData.Telephone.length;
                        intMobileInfoCtr = vData.Mobile.length;
                        intEmailInfoCtr = vData.Email.length;
                        intWebsiteCtr = vData.Website.length;

                        if (isViewReadOnlyProfile) {
                            ReadOnlyOrgProfile(vRowsOrgDetails[0]);
                        } else {
                            $('#txtProspectOrgID').val(vRowsOrgDetails[0].ID);
                            $('#txtCustomerOrgID').val(vRowsOrgDetails[0].CustomerID);
                            $('#txtProspectCompany3rdPartyPlatform').val(vRowsOrgDetails[0].ThirdPartyPlatform);
                            $('#txtProspectCompany3rdPartyID').val(vRowsOrgDetails[0].ThirdPartyID);
                            $('.txtCodeNumber').val(vRowsOrgDetails[0].CodeNumber);
                            $('#txtProspectCompanyName').val(vRowsOrgDetails[0].CompanyName);
                            $('#txtProspectCompanyBusinessTradeName').val(CStr2(vRowsOrgDetails[0].BusinessTradeName));
                            $('#txtProspectOrgTin').val(vRowsOrgDetails[0].TinNumber);
                            $('#txtProspectHouseBldgNo').val(vRowsOrgDetails[0].OfficeAddressBuildingNo);
                            $('#txtProspectStreetName').val(vRowsOrgDetails[0].OfficeAddressStreet);
                            $('#selProspectCities').val(vRowsOrgDetails[0].OfficeAddressCityID);//.selectmenu('refresh');
                            $('#selProspectOrgRegion').val(vRowsOrgDetails[0].Region);//.selectmenu('refresh');
                            $('#selProspectIsland').val(vRowsOrgDetails[0].Island);//.selectmenu('refresh');
                            $('#selVatType').val(vRowsOrgDetails[0].VatType);//.selectmenu('refresh');
                            $('#selProspectCountries').val(vRowsOrgDetails[0].OfficeAddressCountryID);//.selectmenu('refresh');
                            $('#txtProspectCompanyBackground').val(vRowsOrgDetails[0].Background);
                            $('#txtProspectCompanyMission').val(vRowsOrgDetails[0].Mission);
                            $('#txtProspectCompanyVision').val(vRowsOrgDetails[0].Vision);
                            $('#txtProspectCompanyFounding').val(vRowsOrgDetails[0].FoundingDate);
                            $('#txtBusIndustryID').val((vRowsOrgDetails[0].IndustryID == undefined ? '' : vRowsOrgDetails[0].IndustryID));
                            mSalesChannelID = vRowsOrgDetails[0].SalesChannelID;
                            $('#txtBusinessIndustryText').val((vRowsOrgDetails[0].BusinessIndustryText == undefined ? '' : vRowsOrgDetails[0].BusinessIndustryText));
                            $('#btnProspectCompanyIndustry').text((vRowsOrgDetails[0].IndustryName == undefined ? 'Search here' : vRowsOrgDetails[0].IndustryName));//.button('refresh');
                            $('#btnProspectCompanyUserDefinedIndustry').text((vRowsOrgDetails[0].UserDefinedIndustryName == undefined ? 'Search here' : vRowsOrgDetails[0].UserDefinedIndustryName));//.button('refresh');
                            $('#selOrgStatus').val((CBool(vRowsOrgDetails[0].IsActive) == true ? 'Active' : 'Deactive'));//.selectmenu('refresh');

                            $('#txtProspectNoOfEmployees').val((CDec2(vRowsOrgDetails[0].NoOfEmployees) == 0 ? '' : vRowsOrgDetails[0].NoOfEmployees));
                            $('#txtProspectRevenuePerYear').val((CDec2(vRowsOrgDetails[0].RevenuePerYear) == 0 ? '' : vRowsOrgDetails[0].RevenuePerYear));
                            $('#selProspectScopeOperation').val((vRowsOrgDetails[0].ScopeOfOperations == '' ? '' : vRowsOrgDetails[0].ScopeOfOperations));//.selectmenu('refresh');
                            $('#txtProspectNoFactories').val((CDec2(vRowsOrgDetails[0].NoOfFactory) == 0 ? '' : vRowsOrgDetails[0].NoOfFactory));
                            $('#txtProspectNoOffices').val((CDec2(vRowsOrgDetails[0].NoOfOffice) == 0 ? '' : vRowsOrgDetails[0].NoOfOffice));
                            $('#txtProspectNoWarehouses').val((CDec2(vRowsOrgDetails[0].NoOfWarehouse) == 0 ? '' : vRowsOrgDetails[0].NoOfWarehouse));
                            $('#txtProspectNoStores').val((CDec2(vRowsOrgDetails[0].NoOfStore) == 0 ? '' : vRowsOrgDetails[0].NoOfStore));
                            $('#txtProspectNoTrucks').val((CDec2(vRowsOrgDetails[0].NoOfTruck) == 0 ? '' : vRowsOrgDetails[0].NoOfTruck));
                            $('#selRevenueSingleFowst').val(CBool(vRowsOrgDetails[0].SingleCreditTerm) ? 'single' : 'each');
                            if (sessionStorage.LibraryName == 'Customer / Client List') {
                                //RevSalesRepID = vData.Revenue[0].SalesRepID;
                                //arrRevPriceLevels = vData.PriceLevel;
                                //RevPaymentTerms = vData.Revenue[0].TermID;
                                //$('#btnRevSalesRepresentative').text((vData.Revenue[0].SalesRepName == '' ? 'Search Representative' : vData.Revenue[0].SalesRepName));//.button('refresh');
                                //$('#btnRevPriceLevel').text((vData.Revenue[0].PriceLevelName == '' ? 'Search Price Level' : vData.Revenue[0].PriceLevelName));//.button('refresh');
                                //$('#btnRevCreditTerms').text((vData.Revenue[0].TermName == '' ? 'Search Payment Terms' : vData.Revenue[0].TermName));//.button('refresh');
                                //$('#txtRevCreditLimit').val(vData.Revenue[0].CreditLimit);
                                //$('#chkRevUnlimitedCredit').prop('checked', vData.Revenue[0].UnlimitedCredit);//.checkboxradio("refresh");
                                //$('#chkRevRequireCnCApproval').prop('checked', vData.Revenue[0].RequireCnCApproval);//.checkboxradio("refresh");
                                //$('#chkRevCustomerRequirePoNum').prop('checked', vData.Revenue[0].CustomerRequirePoNum);//.checkboxradio("refresh");
                                //$('#txtRevCreditBegBal').val(vData.Revenue[0].CreditBegBalance);
                                //$('#btnRevCreditBegAsOf').text((vData.Revenue[0].CreditAsOf == '' ? 'Set Date' : $.format.date(new Date(vData.Revenue[0].CreditAsOf), 'MMM dd, yyyy')));//.button('refresh');
                                //$('#txtRevThirdPartyBegBalance').val(vData.Revenue[0].ThirdPartyBegBalance);
                                //$('#btnRevThirdPartyBegBalanceAsOf').text((vData.Revenue[0].ThirdPartyBegBalanceAsOf == '' ? 'Set Date' : $.format.date(new Date(vData.Revenue[0].ThirdPartyBegBalanceAsOf), 'MMM dd, yyyy')));//.button('refresh');
                                $('#selDefaultHBTemplate').val(vRowsOrgDetails[0].DefaultHotButtonTemplateID)
                                GetGeneratedPublicUrl();
                            }

                            AddTelInfo();
                            AddMobileInfo();
                            AddEmailInfo()
                            AddWebsiteInfo();
                            AddContactPersonInfo();
                            DisplaySalesReps();
                            AddOrgAffiliation();
                            AddAffiliatedPerson();
                            DisplayRevPriceLevels();
                            //DisplayCompanyOutlet();
                            proj.SupplierDisplayData(vData.Disbursement);
                            GetFiles();
                            GetSupplierCanvassForm();
                            GetCusSupBankInfo();
                            GetVendorSKUs();
                            $('#tbodyRevCreditTerms').empty();
                            if (vData.Revenue != undefined) {
                                if (vData.Revenue.length > 0) {
                                    $('#butSalesChannel').text((vData.Revenue[0].SalesChannelName == undefined ? 'Search here' : vData.Revenue[0].SalesChannelName));//.button('refresh');
                                    for (var i in vData.Revenue) {
                                        DisplayRevCreditTerms(vData.Revenue[i], 'old')
                                    }
                                }
                            }

                            $('#pageActions').pageshow();
                            $('#tabCompanyProfile').opentabmenu();
                            Bs.Loading('hide');

                            $('#lstCompanyOutlet').empty().append($('<div class="text-center">')
                                    .append($('<div class="spinner-border text-primary">'))
                                )

                            AjaxSend({
                                url: 'OrganizationShort/GetCustomerOrgOutlets',
                                data: { ID: id },
                                success: function (e) {
                                    arrCompanyOutlet = e.Results;
                                    DisplayCompanyOutlet();
                                }
                            })
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
                $('#txtpreviewCompanyBusinessTradeName').text(CStr2(data.BusinessTradeName));
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

                $('#prevOrgProfile').modal('show');
                $('#roOrg').pageshow();
                Bs.Loading('hide');
            }

            function GetProspectDetailsPeople(id) {
                ClearPersonProspectVariablesFields();

                if (CStr2(id) == '') {
                    return;
                }

                Bs.Loading('show');

                AjaxSend({
                    url: 'OrganizationShort/GetProspectDetailsPeople',
                    data: { ID: id, Type: sessionStorage.LabelLibraryName },
                    success: function (vData) {
                        var vRowsPeopleDetails = vData.PeopleDetails;
                        arrPersonEmailInfoList = vData.Email;
                        arrPersonMobileInfoList = vData.Mobile;
                        arrPersonTelInfoList = vData.Telephone;
                        arrPersonOrgAffList = vData.Affiliation;
                        arrElementary = vData.Education.Elementary;
                        arrHighSchool = vData.Education.HighSchool;
                        arrCollege = vData.Education.College;

                        if (isViewReadOnlyProfile) {
                            vRowsPeopleDetails[0].Address = (vData.Address[0] || []);
                            ReadOnlyPersonProfile(vRowsPeopleDetails[0]);
                        } else {
                            $('#txtProspectPerson3rdPartyID').val(vRowsPeopleDetails[0].ThirdPartyID);
                            $('#txtCustomerPersonID').val(vRowsPeopleDetails[0].CustomerID);
                            $('#txtProspectPersonID').val(vRowsPeopleDetails[0].ID);
                            $('.txtCodeNumber').val(vRowsPeopleDetails[0].CodeNumber);
                            $('#txtProspectPersonFirstName').val(vRowsPeopleDetails[0].FirstName);
                            $('#txtProspectPersonMiddleName').val(vRowsPeopleDetails[0].MiddleName);
                            $('#txtProspectPersonLastName').val(vRowsPeopleDetails[0].Surname);
                            $('#txtProspectPersonTinNumber').val(vRowsPeopleDetails[0].TIN);
                            $('#txtProspectPersonPosition').val(vRowsPeopleDetails[0].Position);
                            $('#txtProspectPersonPositionDescription').val(vRowsPeopleDetails[0].PositionDescription);
                            $('#selPersonStatus').val((CBool(vRowsPeopleDetails[0].IsActive) == true ? 'Active' : 'Deactive'));//.selectmenu('refresh');

                            GetGeneratedPublicUrl();

                            if (CStr2(vRowsPeopleDetails[0].DateOfBirth) != '') {
                                $('#btnPersonBirthDate').text($.format.date(new Date(parseInt((vRowsPeopleDetails[0].DateOfBirth).replace('/Date(', ''))), 'MMM dd, yyyy'));//.button('refresh');
                            }

                            if (vData.Address.length > 0) {
                                $('#txtPersonAddressID').val(vData.Address[0].ID);
                                $('#txtPersonHouseBuildingNo').val(vData.Address[0].HouseBuildingNo);
                                $('#txtPersonStreet').val(vData.Address[0].Street);
                                $('#selPersonCity').val(vData.Address[0].CityID);//.selectmenu('refresh');
                                $('#selPersonRegion').val(vData.Address[0].Region);//.selectmenu('refresh');
                                $('#selPersonIsland').val(vData.Address[0].Island);//.selectmenu('refresh');
                                $('#selPersonCountry').val(vData.Address[0].CountryID);//.selectmenu('refresh');
                                $('#selPersonTypeAddress').val(vData.Address[0].AddressType);
                            }

                            AddPersonTelInfo();
                            AddPersonMobileInfo();
                            AddPersonEmailInfo();
                            AddPersonOrgAffiliation();

                            academicLvl = 'elem';
                            DisplayEducation();
                            academicLvl = 'hs';
                            DisplayEducation();
                            academicLvl = 'college';
                            DisplayEducation();

                            // revenue tab
                            if (vData.Revenue.length > 0) {
                                RevSalesRepID = vData.Revenue[0].SalesRepID;
                                arrRevPriceLevels = vData.PriceLevel;
                                RevPaymentTerms = vData.Revenue[0].TermID;
                                $('#btnRevSalesRepresentative').text((vData.Revenue[0].SalesRepName == '' ? 'Search Representative' : vData.Revenue[0].SalesRepName));//.button('refresh');
                                //$('#btnRevPriceLevel').text((vData.Revenue[0].PriceLevelName == '' ? 'Search Price Level' : vData.Revenue[0].PriceLevelName));//.button('refresh');
                                $('#btnRevCreditTerms').text((vData.Revenue[0].TermName == '' ? 'Search Payment Terms' : vData.Revenue[0].TermName));//.button('refresh');
                                $('#txtRevCreditLimit').val(vData.Revenue[0].CreditLimit);
                                $('#chkRevUnlimitedCredit').prop('checked', vData.Revenue[0].UnlimitedCredit);//.checkboxradio("refresh");
                                $('#chkRevRequireCnCApproval').prop('checked', vData.Revenue[0].RequireCnCApproval);//.checkboxradio("refresh");
                                $('#chkRevCustomerRequirePoNum').prop('checked', vData.Revenue[0].CustomerRequirePoNum);//.checkboxradio("refresh");
                                $('#txtRevCreditBegBal').val(vData.Revenue[0].CreditBegBalance);
                                $('#btnRevCreditBegAsOf').text((vData.Revenue[0].CreditAsOf == '' ? 'Set Date' : $.format.date(new Date(vData.Revenue[0].CreditAsOf), 'MMM dd, yyyy')));//.button('refresh');
                                $('#txtRevThirdPartyBegBalance').val(vData.Revenue[0].ThirdPartyBegBalance);
                                $('#btnRevThirdPartyBegBalanceAsOf').text((vData.Revenue[0].ThirdPartyBegBalanceAsOf == '' ? 'Set Date' : $.format.date(new Date(vData.Revenue[0].ThirdPartyBegBalanceAsOf), 'MMM dd, yyyy')));//.button('refresh');
                            }                            

                            DisplayRevPriceLevels();
                            GetCusSupBankInfo();

                            $('#pageActions').pageshow();
                            $('#tabPersonProfile').opentabmenu();                            
                        }

                        Bs.Loading('hide');
                    }
                })
            }

            function ReadOnlyPersonProfile(data) {
                var listpreviewPersonOrgAff = $('#listpreviewPersonOrgAff').empty();
                var listpreviewPersonTel = $('#listpreviewPersonTel').empty();
                var listpreviewPersonMobile = $('#listpreviewPersonMobile').empty();
                var listpreviewPersonEmail = $('#listpreviewPersonEmail').empty();
                var listpreviewPersonElem = $('#listpreviewPersonElem').empty();
                var listpreviewPersonHS = $('#listpreviewPersonHS').empty();
                var listpreviewPersonCollege = $('#listpreviewPersonCollege').empty();
                var listpreviewPersonOrgAff = $('#listpreviewPersonOrgAff').empty();

                $('#txtpreviewPersonName').text($.trim(CStr2(data.FirstName) + ' ' + CStr2(data.MiddleName) + ' ' + CStr2(data.Surname)));
                if (CStr2(data.DateOfBirth) != '') {
                    $('#txtpreviewBirthDate').text($.format.date(new Date(parseInt((data.DateOfBirth).replace('/Date(', ''))), 'MMM dd, yyyy'));
                }
                $('#txtpreviewPersonTin').text(data.TIN);
                $('#txtpreviewPosition').text(data.Position);
                $('#txtpreviewPositionDesc').text(data.PositionDescription);

                if (data.Address.length != 0) {
                    $('#txtprevAddress').text($.trim(data.Address.HouseBuildingNo + ' ' + data.Address.Street + ' ' + data.Address.CityName + ' ' + data.Address.CountryName));
                }
                
                for (var i = 0; i < arrPersonOrgAffList.length; i++) {
                    listpreviewPersonOrgAff.append($('<li>').text(arrPersonOrgAffList[i].CompanyName))
                }

                for (var i = 0; i < arrPersonTelInfoList.length; i++) {
                    listpreviewPersonTel.append($('<li>').text('+' + $.trim((arrPersonTelInfoList[i].CountryCode).split('-')[1]) + ' (' + $.trim((arrPersonTelInfoList[i].AreaCode).split('-')[1]) + ') ' + arrPersonTelInfoList[i].TelephoneNo))
                }

                for (var i = 0; i < arrPersonMobileInfoList.length; i++) {
                    listpreviewPersonMobile.append($('<li>').text( $.trim((arrPersonMobileInfoList[i].FullMobileNo))))
                }

                for (var i = 0; i < arrPersonEmailInfoList.length; i++) {
                    listpreviewPersonEmail.append($('<li>').text(arrPersonEmailInfoList[i].EmailAdd))
                }

                for (var i = 0; i < arrElementary.length; i++) {
                    listpreviewPersonElem.append($('<li>').text(arrElementary[i].Name))
                }

                for (var i = 0; i < arrHighSchool.length; i++) {
                    listpreviewPersonHS.append($('<li>').text(arrHighSchool[i].Name))
                }

                for (var i = 0; i < arrCollege.length; i++) {
                    listpreviewPersonCollege.append($('<li>').text(arrCollege[i].Name))
                }

                $('#prevOrgProfile').modal('show');
                $('#roPerson').pageshow();
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
                $('#selPersonStatus').val('Active');//.selectmenu('refresh');
                $('#txtProspectPersonFirstName').val('');
                $('#txtProspectPersonMiddleName').val('');
                $('#txtProspectPersonLastName').val('');
                $('#txtProspectPersonPosition').val('');
                $('#txtProspectPersonPositionDescription').val('');
                $('#txtPersonAddressID').val('');
                $('#txtPersonHouseBuildingNo').val('');
                $('#txtPersonStreet').val('');
                $('#selPersonCity').val('');//.selectmenu('refresh');
                $('#selPersonRegion').val('');//.selectmenu('refresh');
                $('#selPersonIsland').val('');//.selectmenu('refresh');
                $('#selPersonCountry').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');//.selectmenu('refresh');
                $('#selPersonTypeAddress').val('');
                $('#btnPersonBirthDate').text('Set Date');//.button('refresh');

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

                $('#tbodyGeneratedUrl').append($('<tr>').append($('<td colspan="7" class="txt-center">').text('No results found')));
                $('#tbodyAttachments').append($('<tr>').append($('<td colspan="7" class="txt-center">').text('No results found')));

                ClearContactPersonFields();
                ClearPersonEmailInfoFields();
                ClearPersonMobileInfo();
                ClearPersonTelInfo();
            }

            function ClearOrgProspectVariablesFields() {
                proj.SupplierClearFields();
                arrNewDepartment = [];
                arrCompanyOutlet = [];
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
                arrEditVendorPrices = [];

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
                $('#selOrgStatus').val('Active');//.selectmenu('refresh');
                $('#txtProspectCompany3rdPartyPlatform').val('');
                $('#txtProspectCompany3rdPartyID, .txtCodeNumber').val('');
                $('#txtProspectCompanyName').val('');
                $('#txtProspectCompanyBusinessTradeName').val('');
                $('#txtProspectOrgTin').val('');
                $('#txtProspectHouseBldgNo').val('');
                $('#txtProspectStreetName').val('');
                $('#selProspectOrgRegion').val('');//.selectmenu('refresh');
                $('#selProspectCities').val('');//.selectmenu('refresh');
                $('#selProspectIsland').val('');//.selectmenu('refresh');
                $('#selVatType').val('');//.selectmenu('refresh');
                $('#selProspectCountries').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');//.selectmenu('refresh');
                $('#txtProspectCompanyBackground').val('');
                $('#txtProspectCompanyMission').val('');
                $('#txtProspectCompanyVision').val('');
                $('#txtProspectCompanyFounding').val('');
                $('#txtBusIndustryID').val('');
                $('#txtBusinessIndustryText').val('');
                $('#butSalesChannel').text('Search here');//.button('refresh');
                $('#btnProspectCompanyIndustry').text('Search here');//.button('refresh');
                $('#btnProspectCompanyUserDefinedIndustry').text('Search here');//.button('refresh');
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
                $('#lstCompanyOutlet').empty();

                $('#txtProspectNoOfEmployees').val('');
                $('#txtProspectRevenuePerYear').val('');
                $('#selProspectScopeOperation').val('');
                $('#txtProspectNoFactories').val('');
                $('#txtProspectNoOffices').val('');
                $('#txtProspectNoWarehouses').val('');
                $('#txtProspectNoStores').val('');
                $('#txtProspectNoTrucks').val('');

                $('#txtRevCreditLimit').val('');
                $('#chkRevUnlimitedCredit').prop('checked', 0);//.checkboxradio("refresh");
                $('#chkRevCustomerRequirePoNum').prop('checked', 0);//.checkboxradio("refresh");
                $('#txtRevCreditBegBal').val('');
                $('#btnRevSalesRepresentative').text('Search Representative');//.button('refresh');

                $('#txtRevThirdPartyBegBalance').val('');
                $('#btnRevThirdPartyBegBalanceAsOf').text('Set Date');//.button('refresh');

                //$('#btnRevPriceLevel').text('Search Price Level');//.button('refresh');
                $('#btnRevCreditTerms').text('Search Payment Terms');//.button('refresh');
                $('#btnRevCreditBegAsOf').text('Set Date');//.button('refresh');
                $('#fileUploadVendorSKUs').val('');

                $('#tbodyGeneratedUrl').append($('<tr>').append($('<td colspan="7" class="txt-center">').text('No results found')));

                ClearContactPersonFields();
                ClearEmailInfoFields();
                ClearMobileInfo();
                ClearTelInfo();
                ClearWebsiteFields();
            }

            function GotoSigninPage() {

                var pParam = {};
                if (IsMobile)
                    pParam.IsMobile = 1;
                pParam = $.param(pParam);
                window.location = '/SignIn/Crm?' + pParam;

            }; // GotoSigninPage

            function SignOut() {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {

                        AjaxSend({
                            url: "CRMDashboard/setSignOut",
                            success: function (vData) {
                                window.open(GetPathFormat("/SignIn/Crm?mode=logout"), "_self"); // window.open(GetPathFormat("/People"), "_self"); EDITED BY: JANINE , Aug 3 4pm
                            }
                        });

                    },
                    error: function (vError) {
                        GotoSigninPage();
                    }
                })
            }

            $.extend({
                APP: {

                    formatTimer: function (a) {
                        if (a < 10) {
                            a = '0' + a;
                        }
                        return a;
                    },

                    startTimer: function (dir) {

                        var a;

                        // save type
                        $.APP.dir = dir;

                        // get current date
                        $.APP.d1 = new Date();

                        switch ($.APP.state) {

                            case 'pause':

                                // resume timer
                                // get current timestamp (for calculations) and
                                // substract time difference between pause and now
                                $.APP.t1 = $.APP.d1.getTime() - $.APP.td;

                                break;

                            default:

                                // get current timestamp (for calculations)
                                $.APP.t1 = $.APP.d1.getTime();

                                // if countdown add ms based on seconds in textfield
                                if ($.APP.dir === 'cd') {
                                    $.APP.t1 += parseInt($('#cd_seconds').val()) * 1000;
                                }

                                break;

                        }

                        // reset state
                        $.APP.state = 'alive';
                        $('#' + $.APP.dir + '_status').html('Running');

                        // start loop
                        $.APP.loopTimer();

                    },

                    pauseTimer: function () {

                        // save timestamp of pause
                        $.APP.dp = new Date();
                        $.APP.tp = $.APP.dp.getTime();

                        // save elapsed time (until pause)
                        $.APP.td = $.APP.tp - $.APP.t1;

                        // change button value
                        $('#' + $.APP.dir + '_start').val('Resume');

                        // set state
                        $.APP.state = 'pause';
                        $('#' + $.APP.dir + '_status').html('Paused');

                    },

                    stopTimer: function () {

                        // change button value
                        $('#' + $.APP.dir + '_start').val('Restart');

                        // set state
                        $.APP.state = 'stop';
                        $('#' + $.APP.dir + '_status').html('Stopped');

                    },

                    resetTimer: function () {

                        // reset display
                        $('#' + $.APP.dir + '_ms,#' + $.APP.dir + '_s,#' + $.APP.dir + '_m,#' + $.APP.dir + '_h').html('00');

                        // change button value
                        $('#' + $.APP.dir + '_start').val('Start');

                        // set state
                        $.APP.state = 'reset';
                        $('#' + $.APP.dir + '_status').html('Reset & Idle again');

                    },

                    endTimer: function (callback) {

                        // change button value
                        $('#' + $.APP.dir + '_start').val('Restart');

                        // set state
                        $.APP.state = 'end';

                        // invoke callback
                        if (typeof callback === 'function') {
                            callback();
                        }

                    },

                    loopTimer: function () {

                        var td;
                        var d2, t2;

                        var ms = 0;
                        var s = 0;
                        var m = 0;
                        var h = 0;

                        if ($.APP.state === 'alive') {

                            // get current date and convert it into 
                            // timestamp for calculations
                            d2 = new Date();
                            t2 = d2.getTime();

                            // calculate time difference between
                            // initial and current timestamp
                            if ($.APP.dir === 'sw') {
                                td = t2 - $.APP.t1;
                                // reversed if countdown
                            } else {
                                td = $.APP.t1 - t2;
                                if (td <= 0) {
                                    // if time difference is 0 end countdown
                                    $.APP.endTimer(function () {
                                        $.APP.resetTimer();
                                        $('#' + $.APP.dir + '_status').html('Ended & Reset');
                                    });
                                }
                            }

                            // calculate milliseconds
                            ms = td % 1000;
                            if (ms < 1) {
                                ms = 0;
                            } else {
                                // calculate seconds
                                s = (td - ms) / 1000;
                                if (s < 1) {
                                    s = 0;
                                } else {
                                    // calculate minutes   
                                    var m = (s - (s % 60)) / 60;
                                    if (m < 1) {
                                        m = 0;
                                    } else {
                                        // calculate hours
                                        var h = (m - (m % 60)) / 60;
                                        if (h < 1) {
                                            h = 0;
                                        }
                                    }
                                }
                            }

                            // substract elapsed minutes & hours
                            ms = Math.round(ms / 100);
                            s = s - (m * 60);
                            m = m - (h * 60);

                            // update display
                            $('#' + $.APP.dir + '_ms').html($.APP.formatTimer(ms));
                            $('#' + $.APP.dir + '_s').html($.APP.formatTimer(s));
                            $('#' + $.APP.dir + '_m').html($.APP.formatTimer(m));
                            $('#' + $.APP.dir + '_h').html($.APP.formatTimer(h));

                            // loop
                            $.APP.t = setTimeout($.APP.loopTimer, 1);

                        } else {

                            // kill loop
                            clearTimeout($.APP.t);
                            return true;
                        }
                    }
                }
            });
        })// end document
        .keydown(function (e) {
            if (e.keyCode == 27) {
                if (isStartCall) {
                    $.APP.stopTimer();
                    $('#btnStartColdCall').removeClass('btn-disabled');
                    isStartCall = false;
                    callEndDate = new Date();
                }
                $('.profile-preview').hide();
            }
            else if ((e.which == '115' || e.which == '83') && (e.ctrlKey || e.metaKey)) {
                // Ctrl + S
                e.preventDefault();
                if (menuActionToSave == 'profile') {
                    LeadGenSave.Profile();
                }
                else if (menuActionToSave == 'coldcall') {
                    LeadGenSave.ColdCall();
                }
            }
        })
    })

})// end require