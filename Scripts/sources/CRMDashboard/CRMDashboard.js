require([
    'common',
    '/Scripts/sources/crm/qrcode.js', //'tablesort'
    '/Scripts/sources/CRMDashboard/Opportunity.js?crc=' + assetversion,
    'promise!/Scripts/sources/CRMDashboard/Tickets.js?crc=' + assetversion,
    //'/Scripts/sources/SetupChecklist/NewPromptUser.js?crc=' + assetversion,
    'bootstrap-tooltip'
], function () {

    require([
        '/Scripts/jquery.dataTables.min.js'
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

        var arrFilter = [];
        var filterVal = '';

        var isViewReadOnlyProfile = false;
        var isAddNewProspect = false;
        var menuActionToSave = '';

        var timeOut = null, myXHR = null, IsUseCheckbox = false;

        var arrAddress = [], addressIndex = null, arrExistingFiles = [], academicLvl = '', arrElementary = [], arrHighSchool = [], arrCollege = [], isNewEducation = true, ctrElem = 0, ctrHs = 0, ctrCollege = 0, ctrIndexEducation = 0;

        var isErp = (window.location.pathname == '/erp' ? true : false), allowToUploadFiles = false;

        var htmlOpportunity, htmlTickets, htmlSettings, htmlSurvey;
        var stepCaretPos = $('#txtCRMTakeANote').prop("selectionStart");
        var caretPos, isClientActive = true;
        var TagStateTodo, TagIDTodo;
        var TagStateNote, TagIDNote;
        var TagStateCall, TagIDCall;

        $(document).ready(function () {

            $(window).scroll(function () {
                if ($(this).scrollTop()) {
                    $('#btnCRMBackToTop').fadeIn();
                } else {
                    $('#btnCRMBackToTop').fadeOut();
                }
            });

            //used for calendar
            window.DisplayListOfEvents = function (arr) {
                $('#pageCRMListofEvents').pageshow();
                DisplayListOfEvents(arr);
            }

            window.ShowProspectList = function () {
                $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                if (sessionStorage.CRMDashboardPage == 'leadgen') {
                    $('#divtblLeadGen').pageshow();
                }
                else {
                    $('#divtblSalesGrinder').pageshow();
                }
                $('#divLoading').hide();
                $('#divCRMMainPage').show();
                var tabs = $('#tabCrmDashboard, #tabCrmTicketService, #tabCrmOpportunities, #tabCrmSurvey, #tabCrmMyReport, #tabCrmSettings');
                if (sessionStorage.CrmType == 'revsalescrm') {
                    tabs.hidetabmenu();
                    $('#tabCrmSupportCrm').opentabmenu();
                    $('.lblCRMModule').text('Sales');
                    $('#selInvoiceType, #selInvoiceMadeWith').parent().show();
                } else {
                    tabs.showtabmenu();
                    $('#tabCrmDashboard').opentabmenu();
                    $('.lblCRMModule').text('Support');
                    $('#selInvoiceType, #selInvoiceMadeWith').parent().hide();
                }
                ShowProspectList();
                if (window.location.pathname == '/erp' && CStr2(sessionStorage.SystemAdminFor) == 'crm' && sessionStorage.CrmType != 'revsalescrm') {
                    AddBreadCrumb({
                        text: 'Support CRM', Click: function () {
                            $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                        }
                    }, '.ItemsBreadCrumbs')
                }
            }

            window.LeadGenSave = {
                Profile: function () {
                    if (JSON.parse(sessionStorage.ClientInfo).ProspectType == 'Org') {
                        $('#btnCRMSaveAddingContact').click();
                    }
                    else {
                        $('#btnCRMSavePersonAddingContact').click();
                    }
                },
                ColdCall: function () {
                    $('#btnCRMSaveColdCall').click();
                },
                CloseSurvey: function () {
                    $('#popCRMTakeSurvey').modal('hide');
                }
            }

            window.HTMLEncode = function (e) {
                return HTMLEncode(e)
            }

            window.HTMLDecode = function (e) {
                return HTMLDecode(e)
            }

            window.insertAtCursor = function (type, text, src, imgID, imgStatus, dataimg) {
                insertAtCursor(type, text, src, imgID, imgStatus, dataimg)
            }

            window.placeCaretAtEnd = function (e) {
                placeCaretAtEnd(e);
            }

            window.LoadAttachments = function (e) {
                LoadAttachments(e)
            }

            window.getCaretPosition = function (e) {
                return getCaretPosition(e)
            }

            window.arrNoteTicket = [];

            LoadRequiredData();

            $('body')
            .tooltip({ selector: '[data-toggle="tooltip"]' })
            .on('click', '#btnCRMStartColdCall', function () {
                if (isStartCall == false) {
                    callStartDate = $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a');
                    $.APP.startTimer('sw');
                    $('body').find('#btnCRMStartColdCall').addClass('btn-disabled');
                    isStartCall = true;
                }
            })
            .on('click', '#btnCRMEndColdCall', function () {
                $.APP.stopTimer();
                $('#btnCRMStartColdCall').removeClass('btn-disabled');
                isStartCall = false;
                callEndDate = new Date();
            })
            .on('click', '#btnCRMSearchMobile', function () {
                if ($('.contentSearchEngine').css('display') == 'none') {
                    $('.contentSearchEngine').show()
                }
                else {
                    $('.contentSearchEngine').hide()
                }
            })
            .on('click', '.btnHome, .btnHomeMobile', function () {
                SignOut();
            })
            .on('click', '#btnCRMAddTopic', function () {
                ChangePage('#pageCRMAddNewTopic');
            })
            .on('click', '.btnSettings', function () {
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/CRMDashboard/SystemAdministration');
            })
            .on('click', '#btnCRMCancelSetAppointment', function () {
                $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
            })
            .on('click', '#btnCRMCancelColdCall, #btnCRMCancelSendSMS', function () {
                //BreadCrumbs('.ItemsBreadCrumbs',
                //        [{Text: 'Lead Generation'}]
                //    )
                AddBreadCrumb({
                    text: (isErp == false ? 'Lead Generation' : (sessionStorage.SystemAdminFor == 'dtr' ? 'Daily Time Record' : 'Support CRM')), Click: function () {
                        $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
                $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
            })
            .on('click', '#btnCRMOptionColdCall', function () {
                PopupButtonList('', 'Option',
                    [
                        {
                            Text: 'View Profile',
                            data: $('.txtActionClientName').data(),
                            Click: function (e) {
                                BreadCrumbs('.ItemsBreadCrumbs',
                                    [
                                        { Text: 'Lead Generation', Click: function () { $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow() } },
                                        { Text: 'Cold Call', Click: function () { $('#pageCRMActions').pageshow() } },
                                        { Text: (e.CompanyName || e.PeopleFullName) + ' Profile' }
                                    ]
                                    )
                                if (e.ProspectType == "Org") {
                                    GetProspectDetails(e.ID);
                                    $('#pageCRMAddOrgProspect').pageshow();
                                }
                                else {
                                    GetProspectDetailsPeople(e.ID);
                                    $('#pageCRMAddPersonProspect').pageshow();
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
                                        { Text: 'Lead Generation', Click: function () { $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow() } },
                                        { Text: 'Cold Call', Click: function () { $('#pageCRMColdCall').pageshow() } },
                                        { Text: 'New Contact Person for ' + (e.CompanyName || e.PeopleFullName) }
                                    ]
                                    )
                                if (e.ProspectType == "Org") {
                                    GetProspectDetails(e.ID);
                                    $('#pageCRMAddOrgProspect').pageshow();
                                    $('#divCompanyProfile').hide();
                                }
                                else {
                                    GetProspectDetailsPeople(e.ID);
                                    $('#pageCRMAddPersonProspect').pageshow();
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
                                        { Text: 'Lead Generation', Click: function () { $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow() } },
                                        { Text: 'Cold Call', Click: function () { $('#pageCRMActions').pageshow() } },
                                        { Text: 'Interaction History Of ' + (e.CompanyName || e.PeopleFullName) }
                                    ]
                                    )
                                $('#pageCRMActions').pageshow();
                                $('#btnCRMPageInteractionHistory').click();
                                if (e.ProspectType == "Org") {
                                    $('.txtActionClientName').text(e.CompanyName);
                                }
                                else {
                                    $('.txtActionClientName').text(e.PeopleFullName);
                                }
                                BackPage = '#pageCRMActions';
                                isFromColdCallPage = true;
                                //GetInteractionHistory(e.ID);
                            }
                        }
                    ], { AutoDismiss: true }
                )
            })
            .on('click', '#btnCRMSaveColdCall', function () {
                PromptUserAccess('Call', {
                    OnClickOKSuccess: function () {
                        $.APP.stopTimer();
                        $('#btnCRMStartColdCall').removeClass('btn-disabled');
                        isStartCall = false;
                        callEndDate = $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a');

                        var pData = {
                            Client: $('.txtActionClientName').data(),
                            Notes: ($('#txtCRMCallNotes').val().replace(/[<>]/g, function (s) { return entities[s]; })).replace(/\r\n|\r|\n/g, "\n"),
                            CallStartDate: (callStartDate == '' ? $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a') : callStartDate),
                            CallEndDate: callEndDate,
                            CallDuration: $('#sw_h').text() + ':' + $('#sw_m').text() + ':' + $('#sw_s').text() + ':' + $('#sw_ms').text(),
                            IsOrg: ($('.txtActionClientName').data().ProspectType == "Org" ? 1 : 0),
                            DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a'),
                            TaggedID: TagIDCall,
                            TaggedType: TagStateCall
                        }

                        AjaxSend({
                            url: 'CRMDashboard/SaveColdCall',
                            data: pData,
                            success: function (vData) {
                                WebToast('Successfully Saved', 'top');
                                if (isEditingCallNote == false) {
                                    AddBreadCrumb({
                                        text: (isErp == false ? 'Lead Generation' : (sessionStorage.SystemAdminFor == 'dtr' ? 'Daily Time Record' : 'Support CRM')), Click: function () {
                                            $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                                        }
                                    }, '.ItemsBreadCrumbs')
                                    callStartDate = '';
                                    callEndDate = '';
                                    $('#btnCRMPageColdCall').click();
                                }
                                isEditingCallNote = false;
                                isCallNoteSaveAsDraft = false;
                                TagIDCall = '';
                                TagStateCall = 'General';
                                $('#selCRMTagInCall').val('General');
                                $('#lblTaggedAsInCall').text('');
                                $('#txtCRMCallNotes').val('').css('height', '100px').css('min-height', '100px');
                                $('#sw_h, #sw_m, #sw_s, #sw_ms').text('00');
                            }
                        })
                    }
                });
            })
            .on('click', '#btnCRMAddOrgTelephone', function () {
                ClearTelInfo();
                $('#popCRMOrgTelInfo').modal('show');
            })
            .on('click', '#btnCRMCancelOrgTelephone', function () {
                $('#popCRMOrgTelInfo').modal('hide');
                TelState = '';
            })
            .on('click', '#btnCRMOrgTelAreaCode', function () {
                BrowsePopup('LibraryAreaCodes', 'Area Codes',
                    [
                        {
                            DisplayText: [{ Column: 'Area' }, { Label: 'Code', Column: 'CallingCode' }],
                            Click: function (e) {
                                $('#txtCRMOrgTelAreaCodeID').val(e.ID);
                                $('#txtCRMAreaCode').text(e.Area + ' - ' + e.CallingCode)
                            }
                        }
                    ]
                )
            })
            .on('keydown', '#txtCRMTelephoneNum', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('click', '#btnCRMSaveOrgTelephone', function () {
                if ($('#txtCRMOrgTelAreaCodeID').val() == "") {
                    $('#spanCRMerrorAreaCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                if ($('#txtCRMTelephoneNum').val() == "") {
                    $('#spanCRMerrorTelephoneNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                //if ($('#txtCRMTelephoneNum').val().length != 7) {
                //    $('#spanCRMerrorTelephoneNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                //    return false;
                //}

                if ($('#selCRMOrgTelType').val() == "") {
                    $('#spanCRMerrorTelType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }
                intTelInfoCtr = intTelInfoCtr + 1;

                if (TelState == "") {
                    arrTelInfoList.push({
                        "TelRankNo": intTelInfoCtr,
                        "CountryCodeID": $('#txtCRMOrgTelCountryID').val(),
                        "CountryCode": $('#txtCRMCountryCode').text(),
                        "AreaCode": $('#txtCRMAreaCode').text(),
                        "AreaCodeID": $('#txtCRMOrgTelAreaCodeID').val(),
                        "TelephoneNo": $('#txtCRMTelephoneNum').val(),
                        "ExtensionNo": $('#txtCRMTelephoneExt').val(),
                        "ContactType": $('#selCRMOrgTelType').val(),
                        "TelState": "Add",
                        "IsNew": 1,
                        "TelID": $('#txtCRMOrgTelID').val()
                    })
                }
                else {
                    arrTelInfoList[sessionStorage.Telindex].TelRankNo = TelRankNo;
                    arrTelInfoList[sessionStorage.Telindex].CountryCodeID = $('#txtCRMOrgTelCountryID').val();
                    arrTelInfoList[sessionStorage.Telindex].CountryCode = $('#txtCRMCountryCode').text();
                    arrTelInfoList[sessionStorage.Telindex].AreaCodeID = $('#txtCRMOrgTelAreaCodeID').val();
                    arrTelInfoList[sessionStorage.Telindex].AreaCode = $('#txtCRMAreaCode').text();
                    arrTelInfoList[sessionStorage.Telindex].TelephoneNo = $('#txtCRMTelephoneNum').val();
                    arrTelInfoList[sessionStorage.Telindex].ExtensionNo = $('#txtCRMTelephoneExt').val();
                    arrTelInfoList[sessionStorage.Telindex].ContactType = $('#selCRMOrgTelType').val();
                    arrTelInfoList[sessionStorage.Telindex].TelState = sessionStorage.TelState;
                    arrTelInfoList[sessionStorage.Telindex].IsNew = sessionStorage.TelIsNew;
                    arrTelInfoList[sessionStorage.Telindex].TelID = $('#txtCRMOrgTelID').val();
                }

                AddTelInfo();
                ClearTelInfo();
                TelState = "";
            })
            .on('click', '#btnCRMAddOrgMobile', function () {
                ClearMobileInfo();
                $('#popCRMOrgMobileInfo').modal('show');
            })
            .on('click', '#btnCRMCancelOrgMobile', function () {
                $('#popCRMOrgMobileInfo').modal('hide');
                MobileState = '';
            })
            .on('click', '#btnCRMOrgMobileTelCode', function () {
                BrowsePopup('LibraryTelcoPrefixes', 'Telco Codes',
                    [
                        {
                            DisplayText: [{ Column: 'Prefix' }, { Column: 'TelCo' }],
                            Click: function (e) {
                                $('#txtCRMOrgTelCoPrefixID').val(e.ID);
                                $('#txtCRMTelCode').text(e.TelCo + ' - ' + e.Prefix)
                            }
                        }
                    ]
                )
            })
            .on('keydown', '#txtCRMMobileNo', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('click', '#btnCRMSaveOrgMobile', function () {
                //if ($('#txtCRMTelCode').text() == "Select") {
                //    $('#spanCRMerrorMobileTelCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                //    return false;
                //}

                if ($('#txtCRMMobileNo').val() == "") {
                    $('#spanCRMerrorMobileNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                //if ($('#txtCRMMobileNo').val().length != 7) {
                //    $('#spanCRMerrorMobileNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                //    return false;
                //}

                if ($('#selCRMOrgMobileType').val() == "") {
                    $('#spanCRMerrorMobileType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                intMobileInfoCtr = intMobileInfoCtr + 1;

                if (MobileState == "") {
                    arrMobileInfoList.push({
                        "MobileRankNo": intMobileInfoCtr,
                        "CountryCodeID": $('#txtCRMOrgMobileCountryID').val(),
                        "CountryCode": $('#txtCRMMobileCountryCode').text(),
                        //"TelCode": $('#txtCRMTelCode').text(),
                        //"TelCodeID": $('#txtCRMOrgTelCoPrefixID').val(),
                        "MobileNo": $('#txtCRMMobileNo').val(),
                        "ContactType": $('#selCRMOrgMobileType').val(),
                        "MobileID": $('#txtCRMOrgMobileID').val(),
                        "MobileState": "Add",
                        "IsNew": 1,
                        "MobileNos": '+' + $('#txtCRMMobileCountryCode').text() + $('#txtCRMTelCode').text() + $('#txtCRMMobileNo').val()
                    })
                }
                else {
                    arrMobileInfoList[sessionStorage.Mobileindex].MobileRankNo = MobileRankNo;
                    arrMobileInfoList[sessionStorage.Mobileindex].CountryCodeID = $('#txtCRMOrgMobileCountryID').val();
                    arrMobileInfoList[sessionStorage.Mobileindex].CountryCode = $('#txtCRMMobileCountryCode').text();
                    //arrMobileInfoList[sessionStorage.Mobileindex].TelCodeID = $('#txtCRMOrgTelCoPrefixID').val();
                    //arrMobileInfoList[sessionStorage.Mobileindex].TelCode = $('#txtCRMTelCode').text();
                    arrMobileInfoList[sessionStorage.Mobileindex].MobileNo = $('#txtCRMMobileNo').val();
                    arrMobileInfoList[sessionStorage.Mobileindex].ContactType = $('#selCRMOrgMobileType').val();
                    arrMobileInfoList[sessionStorage.Mobileindex].MobileState = sessionStorage.MobileState;
                    arrMobileInfoList[sessionStorage.Mobileindex].IsNew = sessionStorage.MobileIsNew;
                    arrMobileInfoList[sessionStorage.Mobileindex].MobileNos = '+' + $('#txtCRMMobileCountryCode').text() + $('#txtCRMTelCode').text() + $('#txtCRMMobileNo').val();
                }

                AddMobileInfo();
                MobileState = "";
                ClearMobileInfo();
            })
            .on('click', '#btnCRMAddOrgEmail', function () {
                ClearEmailInfoFields();
                $('#popCRMOrgEmailInfo').modal('show');
            })
            .on('click', '#btnCRMCancelOrgEmailAddress', function () {
                $('#popCRMOrgEmailInfo').modal('hide');
                EmailState = '';
            })
            .on('click', '#btnCRMSaveOrgEmailAddress', function () {
                if (IsEmail($.trim($("#txtCRMEmailAdd").val())) == false) {
                    $('#spanCRMerrorEmailAddressValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                if ($.trim($("#txtCRMEmailAdd").val()) == "") {
                    $('#spanCRMerrorEmailAddress').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                if ($('#selCRMOrgEmailType').val() == "") {
                    $('#spanCRMerrorEmailType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                intEmailInfoCtr = intEmailInfoCtr + 1;

                if (EmailState == "") {
                    arrEmailInfoList.push({
                        "EmailRankNo": intEmailInfoCtr,
                        "EmailAddID": $('#txtCRMOrgEmailID').val(),
                        "EmailAdd": $('#txtCRMEmailAdd').val(),
                        "EmailType": $('#selCRMOrgEmailType').val(),
                        "EmailState": "Add",
                        "IsNew": 1
                    })
                }
                else {
                    arrEmailInfoList[sessionStorage.Emailindex].EmailRankNo = EmailRankNo;
                    arrEmailInfoList[sessionStorage.Emailindex].EmailAddID = $('#txtCRMOrgEmailID').val();
                    arrEmailInfoList[sessionStorage.Emailindex].EmailAdd = $('#txtCRMEmailAdd').val();
                    arrEmailInfoList[sessionStorage.Emailindex].EmailType = $('#selCRMOrgEmailType').val();
                    arrEmailInfoList[sessionStorage.Emailindex].EmailState = sessionStorage.EmailState;
                    arrEmailInfoList[sessionStorage.Emailindex].IsNew = sessionStorage.EmailIsNew;
                }

                AddEmailInfo();
                EmailState = "";
            })
            .on('click', '#btnCRMAddOrgWebsite', function () {
                ClearWebsiteFields();
                $('#popCRMOrgWebsiteInfo').modal('show');
            })
            .on('click', '#btnCRMCancelOrgWebsite', function () {
                $('#popCRMOrgWebsiteInfo').modal('hide');
                WebsiteState = '';
            })
            .on('click', '#btnCRMSaveOrgWebsite', function () {
                if ($.trim($('#txtCRMWebsiteUrl').val()) == "") {
                    $('#spanCRMerrorWebsiteUrl').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                intWebsiteCtr = intWebsiteCtr + 1;

                if (WebsiteState == "") {
                    arrWebsiteList.push({
                        "WebsiteRankNo": intWebsiteCtr,
                        "WebsiteID": $('#txtCRMOrgWebsiteID').val(),
                        "Website": $('#txtCRMWebsiteUrl').val(),
                        "WebsiteState": "Add",
                        "IsNew": 1
                    })
                }
                else {
                    arrWebsiteList[sessionStorage.Websiteindex].WebsiteRankNo = EmailRankNo;
                    arrWebsiteList[sessionStorage.Websiteindex].WebsiteID = $('#txtCRMOrgWebsiteID').val();
                    arrWebsiteList[sessionStorage.Websiteindex].Website = $('#txtCRMWebsiteUrl').val();
                    arrWebsiteList[sessionStorage.Websiteindex].WebsiteState = sessionStorage.WebsiteState;
                    arrWebsiteList[sessionStorage.Websiteindex].IsNew = sessionStorage.WebsiteIsNew;
                }

                AddWebsiteInfo();
                WebsiteState = "";
            })
            .on('click', '#btnCRMAddContactPerson', function () {
                ClearContactPersonFields();
                ClearContactPersonEmailInfoFields();
                ClearContactPersonMobileInfo();
                ClearContactPersonTelInfo();
                //$('#popCRMContactPerson').modal('show');
                $('#popCRMContactPersonProfile').modal('show');
            })
            .on('click', '#btnCRMCancelContact', function () {
                $('#popCRMContactPersonProfile').modal('hide');
                ContactPersonState = '';
            })
            .on('click', '#btnCRMOkContact', function () {
                if ($.trim($('#txtCRMContactFirstName').val()) == "") {
                    $('#spanCRMerrorContactFname').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                intContactPersonCtr = intContactPersonCtr + 1;

                if (ContactPersonState == "") {
                    arrContactPersonList.push({
                        "ContactPersonRankNo": intContactPersonCtr,
                        "PeopleID": $('#txtCRMPeopleID').val(),
                        "ID": $('#txtCRMContactPersonID').val(),
                        "Firstname": $('#txtCRMContactFirstName').val(),
                        "Middlename": $('#txtCRMContactMiddleName').val(),
                        "Lastname": $('#txtCRMContactLastName').val(),
                        "Position": $('#txtCRMContactPersonPosition').val(),
                        "PositionDescription": $('#txtCRMContactPersonPositionDescription').val(),
                        "AddressID": $('#txtCRMContactPersonAddressID').val(),
                        "HouseBuildingNo": $('#txtCRMContactPersonHouseBuildingNo').val(),
                        "Street": $('#txtCRMContactPersonStreet').val(),
                        "City": $('#selCRMContactPersonCity').val(),
                        "Country": $('#selCRMContactPersonCountry').val(),
                        "TypeAddress": $('#selCRMContactPersonTypeAddress').val(),
                        "ContactPersonState": "Add",
                        "IsNew": 1,
                        "Tel": arrContactPersonTelInfoList,
                        "Mobile": arrContactPersonMobileInfoList,
                        "Email": arrContactPersonEmailInfoList
                    })
                }
                else {
                    arrContactPersonList[sessionStorage.Contactindex].ContactPersonRankNo = ContactPersonRankNo;
                    arrContactPersonList[sessionStorage.Contactindex].PeopleID = $('#txtCRMPeopleID').val();
                    arrContactPersonList[sessionStorage.Contactindex].ID = $('#txtCRMContactPersonID').val();
                    arrContactPersonList[sessionStorage.Contactindex].Firstname = $('#txtCRMContactFirstName').val();
                    arrContactPersonList[sessionStorage.Contactindex].Middlename = $('#txtCRMContactMiddleName').val();
                    arrContactPersonList[sessionStorage.Contactindex].Lastname = $('#txtCRMContactLastName').val();
                    arrContactPersonList[sessionStorage.Contactindex].Position = $('#txtCRMContactPersonPosition').val();
                    arrContactPersonList[sessionStorage.Contactindex].PositionDescription = $('#txtCRMContactPersonPositionDescription').val();
                    arrContactPersonList[sessionStorage.Contactindex].AddressID = $('#txtCRMContactPersonAddressID').val();
                    arrContactPersonList[sessionStorage.Contactindex].HouseBuildingNo = $('#txtCRMContactPersonHouseBuildingNo').val();
                    arrContactPersonList[sessionStorage.Contactindex].Street = $('#txtCRMContactPersonStreet').val();
                    arrContactPersonList[sessionStorage.Contactindex].City = $('#selCRMContactPersonCity').val();
                    arrContactPersonList[sessionStorage.Contactindex].Country = $('#selCRMContactPersonCountry').val();
                    arrContactPersonList[sessionStorage.Contactindex].TypeAddress = $('#selCRMContactPersonTypeAddress').val();
                    arrContactPersonList[sessionStorage.Contactindex].ContactPersonState = sessionStorage.ContactPersonState;
                    arrContactPersonList[sessionStorage.Contactindex].IsNew = sessionStorage.ContactPersonIsNew;
                    arrContactPersonList[sessionStorage.Contactindex].Tel = arrContactPersonTelInfoList;
                    arrContactPersonList[sessionStorage.Contactindex].Mobile = arrContactPersonMobileInfoList;
                    arrContactPersonList[sessionStorage.Contactindex].Email = arrContactPersonEmailInfoList;
                }

                AddContactPersonInfo();
                ContactPersonState = "";
            })
            .on('click', '#btnCRMProspectPersonBirthday', function () {
                SingleCalendar({
                    Click: function (e) {
                        $('#btnCRMProspectPersonBirthday').text(e)//.button('refresh');
                    }
                })
            })
            .on('click', '#btnCRMAddContactPersonTelephone', function () {
                ClearContactPersonTelInfo();
                $('#popCRMContactPersonTelInfo').modal('show');
            })
            .on('click', '#btnCRMCancelContactPersonTelephone', function () {
                $('#popCRMContactPersonTelInfo').modal('hide');
                ContactPersonTelState = '';
            })
            .on('click', '#btnCRMContactPersonTelAreaCode', function () {
                BrowsePopup('LibraryAreaCodes', 'Area Codes',
                    [
                        {
                            DisplayText: [{ Column: 'Area' }, { Label: 'Code', Column: 'CallingCode' }],
                            Click: function (e) {
                                $('#txtCRMContactPersonTelAreaCodeID').val(e.ID);
                                $('#txtCRMContactPersonAreaCode').text(e.Area + ' - ' + e.CallingCode)
                            }
                        }
                    ]
                )
            })
            .on('keydown', '#txtCRMContactPersonTelephoneNum', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('click', '#btnCRMSaveContactPersonTelephone', function () {
                if ($('#txtCRMContactPersonAreaCode').text() == "Select") {
                    $('#spanCRMerrorContactPersonAreaCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                if ($('#txtCRMContactPersonTelephoneNum').val() == "") {
                    $('#spanCRMerrorContactPersonTelephoneNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                //if ($('#txtCRMContactPersonTelephoneNum').val().length != 7) {
                //    $('#spanCRMerrorContactPersonTelephoneNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                //    return false;
                //}

                if ($('#selCRMContactPersonTelType').val() == "") {
                    $('#spanCRMerrorContactPersonTelType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }
                intContactPersonTelInfoCtr = intContactPersonTelInfoCtr + 1;

                if (ContactPersonTelState == "") {
                    arrContactPersonTelInfoList.push({
                        "TelRankNo": intContactPersonTelInfoCtr,
                        "CountryCodeID": $('#txtCRMContactPersonTelCountryID').val(),
                        "CountryCode": $('#txtCRMContactPersonCountryCode').text(),
                        "AreaCode": $('#txtCRMContactPersonAreaCode').text(),
                        "AreaCodeID": $('#txtCRMContactPersonTelAreaCodeID').val(),
                        "TelephoneNo": $('#txtCRMContactPersonTelephoneNum').val(),
                        "ExtensionNo": $('#txtCRMContactPersonTelephoneExt').val(),
                        "ContactType": $('#selCRMContactPersonTelType').val(),
                        "TelState": "Add",
                        "IsNew": 1,
                        "TelID": $('#txtCRMContactPersonTelID').val()
                    })
                }
                else {
                    arrContactPersonTelInfoList[sessionStorage.Telindex].TelRankNo = ContactPersonTelRankNo;
                    arrContactPersonTelInfoList[sessionStorage.Telindex].CountryCodeID = $('#txtCRMContactPersonTelCountryID').val();
                    arrContactPersonTelInfoList[sessionStorage.Telindex].CountryCode = $('#txtCRMContactPersonCountryCode').text();
                    arrContactPersonTelInfoList[sessionStorage.Telindex].AreaCodeID = $('#txtCRMContactPersonTelAreaCodeID').val();
                    arrContactPersonTelInfoList[sessionStorage.Telindex].AreaCode = $('#txtCRMContactPersonAreaCode').text();
                    arrContactPersonTelInfoList[sessionStorage.Telindex].TelephoneNo = $('#txtCRMContactPersonTelephoneNum').val();
                    arrContactPersonTelInfoList[sessionStorage.Telindex].ExtensionNo = $('#txtCRMContactPersonTelephoneExt').val();
                    arrContactPersonTelInfoList[sessionStorage.Telindex].ContactType = $('#selCRMContactPersonTelType').val();
                    arrContactPersonTelInfoList[sessionStorage.Telindex].TelState = sessionStorage.TelState;
                    arrContactPersonTelInfoList[sessionStorage.Telindex].IsNew = sessionStorage.TelIsNew;
                    arrContactPersonTelInfoList[sessionStorage.Telindex].TelID = $('#txtCRMContactPersonTelID').val();
                }

                AddContactPersonTelInfo();
                ClearContactPersonTelInfo();
                ContactPersonTelState = "";
            })
            .on('click', '#btnCRMAddContactPersonMobile', function () {
                ClearContactPersonMobileInfo();
                $('#popCRMContactPersonMobileInfo').modal('show');
            })
            .on('click', '#btnCRMCancelContactPersonMobile', function () {
                $('#popCRMContactPersonMobileInfo').modal('hide');
                ContactPersonMobileState = '';
            })
            .on('click', '#btnCRMContactPersonMobileTelCode', function () {
                BrowsePopup('LibraryTelcoPrefixes', 'Telco Codes',
                    [
                        {
                            DisplayText: [{ Column: 'Prefix' }, { Column: 'TelCo' }],
                            Click: function (e) {
                                $('#txtCRMContactPersonTelCoPrefixID').val(e.ID);
                                $('#txtCRMContactPersonTelCode').text(e.TelCo + ' - ' + e.Prefix)
                            }
                        }
                    ]
                )
            })
            .on('keydown', '#txtCRMContactPersonMobileNo', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('click', '#btnCRMSaveContactPersonMobile', function () {
                //if ($('#txtCRMContactPersonTelCode').text() == "Select") {
                //    $('#spanCRMerrorContactPersonMobileTelCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                //    return false;
                //}

                if ($('#txtCRMContactPersonMobileNo').val() == "") {
                    $('#spanCRMerrorContactPersonMobileNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                //if ($('#txtCRMContactPersonMobileNo').val().length != 7) {
                //    $('#spanCRMerrorContactPersonMobileNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                //    return false;
                //}

                if ($('#selCRMContactPersonMobileType').val() == "") {
                    $('#spanCRMerrorContactPersonMobileType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                intContactPersonMobileInfoCtr = intContactPersonMobileInfoCtr + 1;

                if (ContactPersonMobileState == "") {
                    arrContactPersonMobileInfoList.push({
                        "MobileRankNo": intContactPersonMobileInfoCtr,
                        "CountryCodeID": $('#txtCRMContactPersonMobileCountryID').val(),
                        "CountryCode": $('#txtCRMContactPersonMobileCountryCode').text(),
                        //"TelCode": $('#txtCRMContactPersonTelCode').text(),
                        //"TelCodeID": $('#txtCRMContactPersonTelCoPrefixID').val(),
                        "MobileNo": $('#txtCRMContactPersonMobileNo').val(),
                        "ContactType": $('#selCRMContactPersonMobileType').val(),
                        "MobileID": $('#txtCRMContactPersonMobileID').val(),
                        "MobileState": "Add",
                        "IsNew": 1
                    })
                }
                else {
                    arrContactPersonMobileInfoList[sessionStorage.Mobileindex].MobileRankNo = ContactPersonMobileRankNo;
                    arrContactPersonMobileInfoList[sessionStorage.Mobileindex].CountryCodeID = $('#txtCRMContactPersonMobileCountryID').val();
                    arrContactPersonMobileInfoList[sessionStorage.Mobileindex].CountryCode = $('#txtCRMContactPersonMobileCountryCode').text();
                    //arrContactPersonMobileInfoList[sessionStorage.Mobileindex].TelCodeID = $('#txtCRMContactPersonTelCoPrefixID').val();
                    //arrContactPersonMobileInfoList[sessionStorage.Mobileindex].TelCode = $('#txtCRMContactPersonTelCode').text();
                    arrContactPersonMobileInfoList[sessionStorage.Mobileindex].MobileNo = $('#txtCRMContactPersonMobileNo').val();
                    arrContactPersonMobileInfoList[sessionStorage.Mobileindex].ContactType = $('#selCRMContactPersonMobileType').val();
                    arrContactPersonMobileInfoList[sessionStorage.Mobileindex].MobileState = sessionStorage.MobileState;
                    arrContactPersonMobileInfoList[sessionStorage.Mobileindex].IsNew = sessionStorage.MobileIsNew;
                }

                AddContactPersonMobileInfo();
                ContactPersonMobileState = "";
                ClearContactPersonMobileInfo();
            })
            .on('click', '#btnCRMAddContactPersonEmail', function () {
                ClearContactPersonEmailInfoFields();
                $('#popCRMContactPersonEmailInfo').modal('show');
            })
            .on('click', '#btnCRMCancelContactPersonEmailAddress', function () {
                $('#popCRMContactPersonEmailInfo').modal('hide');
                ContactPersonEmailState = '';
            })
            .on('click', '#btnCRMSaveContactPersonEmailAddress', function () {
                if (IsEmail($.trim($("#txtCRMContactPersonEmailAdd").val())) == false) {
                    $('#spanCRMerrorContactPersonEmailAddressValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                if ($.trim($("#txtCRMContactPersonEmailAdd").val()) == "") {
                    $('#spanCRMerrorContactPersonEmailAddress').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                if ($('#selCRMContactPersonEmailType').val() == "") {
                    $('#spanCRMerrorContactPersonEmailType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                intContactPersonEmailInfoCtr = intContactPersonEmailInfoCtr + 1;

                if (ContactPersonEmailState == "") {
                    arrContactPersonEmailInfoList.push({
                        "EmailRankNo": intContactPersonEmailInfoCtr,
                        "EmailAddID": $('#txtCRMContactPersonEmailID').val(),
                        "EmailAdd": $('#txtCRMContactPersonEmailAdd').val(),
                        "EmailType": $('#selCRMContactPersonEmailType').val(),
                        "EmailState": "Add",
                        "IsNew": 1,
                    })
                }
                else {
                    arrContactPersonEmailInfoList[sessionStorage.Emailindex].EmailRankNo = ContactPersonEmailRankNo;
                    arrContactPersonEmailInfoList[sessionStorage.Emailindex].EmailAddID = $('#txtCRMContactPersonEmailID').val();
                    arrContactPersonEmailInfoList[sessionStorage.Emailindex].EmailAdd = $('#txtCRMContactPersonEmailAdd').val();
                    arrContactPersonEmailInfoList[sessionStorage.Emailindex].EmailType = $('#selCRMContactPersonEmailType').val();
                    arrContactPersonEmailInfoList[sessionStorage.Emailindex].EmailState = sessionStorage.EmailState;
                    arrContactPersonEmailInfoList[sessionStorage.Emailindex].IsNew = sessionStorage.EmailIsNew;
                }

                AddContactPersonEmailInfo();
                ContactPersonEmailState = "";
            })
            .on('click', '#btnCRMCancelAddingContact', function () {
                //if(isFromColdCallPage == true){
                //    BreadCrumbs('.ItemsBreadCrumbs',
                //        [
                //            {Text: 'Lead Generation', Click: function(){}},
                //            {Text: 'Cold Call'}
                //        ]
                //        )
                //    $('#pageCRMActions').pageshow();
                //}
                //else {
                //    BreadCrumbs('.ItemsBreadCrumbs',
                //        [
                //            {Text: 'Lead Generation'}
                //        ]
                //        )
                //    $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                //}
                AddBreadCrumb({
                    text: (isErp == false ? 'Lead Generation' : (sessionStorage.SystemAdminFor == 'dtr' ? 'Daily Time Record' : 'Support CRM')), Click: function () {
                        $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
                //$('#divCompanyProfile').show();
                $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                isFromColdCallPage = false;
            })
            .on('click', '#btnCRMSaveAddingContact', function () {
                SaveNewOrgProspect();
            })
            .on('click', '#btnCRMProspectCompanyIndustry', function () {
                BrowsePopup('LibraryIndustryClassification', 'Business Industry',
                        [
                            {
                                DisplayText: [{ Column: 'Class' }],
                                Click: function (e) {
                                    $('#txtCRMBusIndustryID').val(e.ID);
                                    $('#btnCRMProspectCompanyIndustry').text(e.Class)//.button('refresh');
                                }
                            }
                        ]
                    )
            })

            .on('click', '#btnCRMAddPersonTelephone', function () {
                ClearPersonTelInfo();
                $('#popCRMPersonTelInfo').modal('show');
            })
            .on('click', '#btnCRMCancelPersonTelephone', function () {
                $('#popCRMPersonTelInfo').modal('hide');
                PersonTelState = '';
            })
            .on('click', '#btnCRMPersonTelAreaCode', function () {
                BrowsePopup('LibraryAreaCodes', 'Area Codes',
                    [
                        {
                            DisplayText: [{ Column: 'Area' }, { Label: 'Code', Column: 'CallingCode' }],
                            Click: function (e) {
                                $('#txtCRMPersonTelAreaCodeID').val(e.ID);
                                $('#txtCRMPersonAreaCode').text(e.Area + ' - ' + e.CallingCode)
                            }
                        }
                    ]
                )
            })
            .on('keydown', '#txtCRMPersonTelephoneNum', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('click', '#btnCRMSavePersonTelephone', function () {
                if ($('#txtCRMPersonAreaCode').text() == "Select") {
                    $('#spanCRMerrorPersonAreaCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                if ($('#txtCRMPersonTelephoneNum').val() == "") {
                    $('#spanCRMerrorPersonTelephoneNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                //if ($('#txtCRMPersonTelephoneNum').val().length != 7) {
                //    $('#spanCRMerrorPersonTelephoneNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                //    return false;
                //}

                if ($('#selCRMPersonTelType').val() == "") {
                    $('#spanCRMerrorPersonTelType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }
                intPersonTelInfoCtr = intPersonTelInfoCtr + 1;

                if (PersonTelState == "") {
                    arrPersonTelInfoList.push({
                        "TelRankNo": intPersonTelInfoCtr,
                        "CountryCodeID": $('#txtCRMPersonTelCountryID').val(),
                        "CountryCode": $('#txtCRMPersonCountryCode').text(),
                        "AreaCode": $('#txtCRMPersonAreaCode').text(),
                        "AreaCodeID": $('#txtCRMPersonTelAreaCodeID').val(),
                        "TelephoneNo": $('#txtCRMPersonTelephoneNum').val(),
                        "ExtensionNo": $('#txtCRMPersonTelephoneExt').val(),
                        "ContactType": $('#selCRMPersonTelType').val(),
                        "TelState": "Add",
                        "IsNew": 1,
                        "TelID": $('#txtCRMPersonTelID').val()
                    })
                }
                else {
                    arrPersonTelInfoList[sessionStorage.Telindex].TelRankNo = TelRankNo;
                    arrPersonTelInfoList[sessionStorage.Telindex].CountryCodeID = $('#txtCRMPersonTelCountryID').val();
                    arrPersonTelInfoList[sessionStorage.Telindex].CountryCode = $('#txtCRMPersonCountryCode').text();
                    arrPersonTelInfoList[sessionStorage.Telindex].AreaCodeID = $('#txtCRMPersonTelAreaCodeID').val();
                    arrPersonTelInfoList[sessionStorage.Telindex].AreaCode = $('#txtCRMPersonAreaCode').text();
                    arrPersonTelInfoList[sessionStorage.Telindex].TelephoneNo = $('#txtCRMPersonTelephoneNum').val();
                    arrPersonTelInfoList[sessionStorage.Telindex].ExtensionNo = $('#txtCRMPersonTelephoneExt').val();
                    arrPersonTelInfoList[sessionStorage.Telindex].ContactType = $('#selCRMPersonTelType').val();
                    arrPersonTelInfoList[sessionStorage.Telindex].TelState = sessionStorage.TelState;
                    arrPersonTelInfoList[sessionStorage.Telindex].IsNew = sessionStorage.TelIsNew;
                    arrPersonTelInfoList[sessionStorage.Telindex].TelID = $('#txtCRMPersonTelID').val();
                }

                AddPersonTelInfo();
                ClearPersonTelInfo();
                PersonTelState = "";
            })
            .on('click', '#btnCRMAddPersonMobile', function () {
                ClearPersonMobileInfo();
                $('#popCRMPersonMobileInfo').modal('show');
            })
            .on('click', '#btnCRMCancelPersonMobile', function () {
                $('#popCRMPersonMobileInfo').modal('hide');
                PersonMobileState = '';
            })
            .on('click', '#btnCRMPersonMobileTelCode', function () {
                BrowsePopup('LibraryTelcoPrefixes', 'Telco Codes',
                    [
                        {
                            DisplayText: [{ Column: 'Prefix' }, { Column: 'TelCo' }],
                            Click: function (e) {
                                $('#txtCRMPersonTelCoPrefixID').val(e.ID);
                                $('#txtCRMPersonTelCode').text(e.TelCo + ' - ' + e.Prefix)
                            }
                        }
                    ]
                )
            })
            .on('keydown', '#txtCRMPersonMobileNo', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('click', '#btnCRMSavePersonMobile', function () {
                //if ($('#txtCRMPersonTelCode').text() == "Select") {
                //    $('#spanCRMerrorPersonMobileTelCode').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                //    return false;
                //}

                if ($('#txtCRMPersonMobileNo').val() == "") {
                    $('#spanCRMerrorPersonMobileNum').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                //if ($('#txtCRMPersonMobileNo').val().length != 7) {
                //    $('#spanCRMerrorPersonMobileNumValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                //    return false;
                //}

                if ($('#selCRMPersonMobileType').val() == "") {
                    $('#spanCRMerrorPersonMobileType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                intPersonMobileInfoCtr = intPersonMobileInfoCtr + 1;

                if (PersonMobileState == "") {
                    arrPersonMobileInfoList.push({
                        "MobileRankNo": intPersonMobileInfoCtr,
                        "CountryCodeID": $('#txtCRMPersonMobileCountryID').val(),
                        "CountryCode": $('#txtCRMPersonMobileCountryCode').text(),
                        //"TelCode": $('#txtCRMPersonTelCode').text(),
                        //"TelCodeID": $('#txtCRMPersonTelCoPrefixID').val(),
                        "MobileNo": $('#txtCRMPersonMobileNo').val(),
                        "ContactType": $('#selCRMPersonMobileType').val(),
                        "MobileID": $('#txtCRMPersonMobileID').val(),
                        "MobileState": "Add",
                        "IsNew": 1,
                        "MobileNos": '+' + $('#txtCRMPersonMobileCountryCode').text() + $('#txtCRMPersonTelCode').text() + $('#txtCRMPersonMobileNo').val()
                    })
                }
                else {
                    arrPersonMobileInfoList[sessionStorage.Mobileindex].MobileRankNo = MobileRankNo;
                    arrPersonMobileInfoList[sessionStorage.Mobileindex].CountryCodeID = $('#txtCRMPersonMobileCountryID').val();
                    arrPersonMobileInfoList[sessionStorage.Mobileindex].CountryCode = $('#txtCRMPersonMobileCountryCode').text();
                    //arrPersonMobileInfoList[sessionStorage.Mobileindex].TelCodeID = $('#txtCRMPersonTelCoPrefixID').val();
                    //arrPersonMobileInfoList[sessionStorage.Mobileindex].TelCode = $('#txtCRMPersonTelCode').text();
                    arrPersonMobileInfoList[sessionStorage.Mobileindex].MobileNo = $('#txtCRMPersonMobileNo').val();
                    arrPersonMobileInfoList[sessionStorage.Mobileindex].ContactType = $('#selCRMPersonMobileType').val();
                    arrPersonMobileInfoList[sessionStorage.Mobileindex].MobileState = sessionStorage.MobileState;
                    arrPersonMobileInfoList[sessionStorage.Mobileindex].IsNew = sessionStorage.MobileIsNew;
                    arrPersonMobileInfoList[sessionStorage.Mobileindex].MobileNos = '+' + $('#txtCRMPersonMobileCountryCode').text() + $('#txtCRMPersonTelCode').text() + $('#txtCRMPersonMobileNo').val()
                }

                AddPersonMobileInfo();
                PersonMobileState = "";
                ClearPersonMobileInfo();
            })
            .on('click', '#btnCRMAddPersonEmail', function () {
                ClearPersonEmailInfoFields();
                $('#popCRMPersonEmailInfo').modal('show');
            })
            .on('click', '#btnCRMCancelPersonEmailAddress', function () {
                $('#popCRMPersonEmailInfo').modal('hide');
                PersonEmailState = '';
            })
            .on('click', '#btnCRMSavePersonEmailAddress', function () {
                if (IsEmail($.trim($("#txtCRMPersonEmailAdd").val())) == false) {
                    $('#spanCRMerrorPersonEmailAddressValidate').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                if ($.trim($("#txtCRMPersonEmailAdd").val()) == "") {
                    $('#spanCRMerrorPersonEmailAddress').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                if ($('#selCRMPersonEmailType').val() == "") {
                    $('#spanCRMerrorPersonEmailType').delay(10).fadeIn("slow").delay(2000).fadeOut("slow");
                    return false;
                }

                intPersonEmailInfoCtr = intPersonEmailInfoCtr + 1;

                if (PersonEmailState == "") {
                    arrPersonEmailInfoList.push({
                        "EmailRankNo": intPersonEmailInfoCtr,
                        "EmailAddID": $('#txtCRMPersonEmailID').val(),
                        "EmailAdd": $('#txtCRMPersonEmailAdd').val(),
                        "EmailType": $('#selCRMPersonEmailType').val(),
                        "EmailState": "Add",
                        "IsNew": 1
                    })
                }
                else {
                    arrPersonEmailInfoList[sessionStorage.Emailindex].EmailRankNo = EmailRankNo;
                    arrPersonEmailInfoList[sessionStorage.Emailindex].EmailAddID = $('#txtCRMPersonEmailID').val();
                    arrPersonEmailInfoList[sessionStorage.Emailindex].EmailAdd = $('#txtCRMPersonEmailAdd').val();
                    arrPersonEmailInfoList[sessionStorage.Emailindex].EmailType = $('#selCRMPersonEmailType').val();
                    arrPersonEmailInfoList[sessionStorage.Emailindex].EmailState = sessionStorage.EmailState;
                    arrPersonEmailInfoList[sessionStorage.Emailindex].IsNew = sessionStorage.EmailIsNew;
                }

                AddPersonEmailInfo();
                PersonEmailState = "";
            })
            .on('click', '#btnCRMCancelPersonAddingContact', function () {
                AddBreadCrumb({
                    text: (isErp == false ? 'Lead Generation' : (sessionStorage.SystemAdminFor == 'dtr' ? 'Daily Time Record' : 'Support CRM')), Click: function () {
                        $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
                isFromColdCallPage = false;
                $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
            })
            .on('click', '#btnCRMSavePersonAddingContact', function () {
                SaveNewPersonProspect();
            })
            .on('click', '#btnCRMAddOrgProspect', function () {
                PromptUserAccess('Add Profile', {
                    OnClickOKSuccess: function () {
                        AddBreadCrumb({
                            text: 'New Company', Click: function () {
                                $('#pageCRMActions, #pageCRMProfile, #pageCRMProfileOrg').pageshow();
                            }, UpdateLastchild: true
                        }, '.ItemsBreadCrumbs')
                        ClearOrgProspectVariablesFields();
                        $('#pageCRMActions, #pageCRMProfile, #pageCRMProfileOrg').pageshow();
                        $('#listCRMAction').find('li > a:not(:first)').addClass('disabled-menu');
                        isAddNewProspect = true;
                        isClientActive = true;
                        $(window).scrollTop(0);
                        $('#divTabCompany ul > li:first').click()
                        $('#btnCRMPageProfile').click()
                    }
                });
            })
            .on('click', '#btnCRMAddPeopleProspect', function () {
                PromptUserAccess('Add Profile', {
                    OnClickOKSuccess: function () {
                        AddBreadCrumb({
                            text: 'New Person', Click: function () {
                                $('#pageCRMActions, #pageCRMProfile, #pageCRMProfilePeople').pageshow();
                            }, UpdateLastchild: true
                        }, '.ItemsBreadCrumbs')
                        ClearPersonProspectVariablesFields();
                        $('#pageCRMActions, #pageCRMProfile, #pageCRMProfilePeople').pageshow();
                        $('#listCRMAction').find('li > a:not(:first)').addClass('disabled-menu');
                        isAddNewProspect = true;
                        isClientActive = true;
                        $(window).scrollTop(0);
                        $('#divTabPerson ul > li:first').click()
                        $('#btnCRMPageProfile').click()
                    }
                });
            })
            .on('click', '#btnCRMInteractionHistoryDetailsOk', function () {
                $('#popCRMInteractionHistoryDetails').modal('hide');
            })
            .on('click', '#btnCRMInteractionHistoryBack', function () {
                //if(isFromColdCallPage == true){
                //    BreadCrumbs('.ItemsBreadCrumbs',
                //        [
                //            {Text: 'Lead Generation', Click: function(){ $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow()}},
                //            {Text: 'Cold Call'}
                //        ]
                //        )
                //    $('#btnCRMPageColdCall').click();
                //}
                //else {
                //    BreadCrumbs('.ItemsBreadCrumbs',
                //        [
                //            {Text: 'Lead Generation'}
                //        ]
                //        )
                //}
                AddBreadCrumb({
                    text: (isErp == false ? 'Lead Generation' : (sessionStorage.SystemAdminFor == 'dtr' ? 'Daily Time Record' : 'Support CRM')), Click: function () {
                        $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
                $(BackPage).pageshow();
                isFromColdCallPage = false;
            })
            .on('click', '.btnCloseToolTip', function () {
                $('.tooltip-window').hide()
            })
            .on('click', '#btnCRMAddToDo', function () {
                ClearTodoFields();
                $('#popCRMNewToDo').modal('show');
            })
            .on('click', '#btnCRMCancelToDo', function () {
                $('#popCRMNewToDo').modal('hide');
            })
            .on('click', '#btnCRMToDoSetDate, #btnCRMEditToDoSetDate', function () {
                var selector = $(this)[0].id;
                SingleCalendar({
                    ClearButton: 'Clear Date',
                    Placeholder: 'Set Date',
                    SetDate: (todoSetDate == '' ? new Date() : todoSetDate),
                    Click: function (e) {
                        todoSetDate = e;
                        if (new Date(e) == "Invalid Date") {
                            todoSetDate = '';
                            $('#btnCRMEditToDoSetDate').text('--')//.button('refresh');
                        }
                        else {
                            $('#btnCRMEditToDoSetDate').text($.format.date(new Date(e), 'MMM dd, yyyy'))//.button('refresh');
                        }
                        if (selector == "btnEditToDoSetDate") {
                            $('#popCRMToDo').modal('show');
                        }
                    },
                    Cancel: function () {
                        if (selector == "btnEditToDoSetDate") {
                            $('#popCRMToDo').modal('show');
                        }
                    }
                })
            })
            .on('click', '#btnCRMOkToDo', function () {
                PreSaveToDo();
            })
            .on('keydown', '#txtCRMToDo', function (e) {
                if (e.keyCode == 13) {
                    $('#txtCRMToDoID').val('');
                    PreSaveToDo();
                }
            })
            .on('click', '#btnCRMCancelEditToDo', function () {
                todoSetDate = '';
                $('#popCRMToDo').modal('hide');
            })
            .on('click', '#btnCRMOkEditToDo', function () {
                $('#popCRMToDo').modal('hide');

                arrTodoList[sessionStorage.ToDoIndex].TodoRankNo = TodoRankNo;
                arrTodoList[sessionStorage.ToDoIndex].ClientID = $('.txtActionClientName').data().ID;
                arrTodoList[sessionStorage.ToDoIndex].ToDoID = $('#txtCRMToDoID').val();
                arrTodoList[sessionStorage.ToDoIndex].ToDo = $('#txtCRMEditToDo').val();
                arrTodoList[sessionStorage.ToDoIndex].ToDoDueDate = todoSetDate;
                arrTodoList[sessionStorage.ToDoIndex].ToDoOrder = (intTodoInfoCtr == 0 ? 1 : intTodoInfoCtr + 1);
                arrTodoList[sessionStorage.ToDoIndex].ToDoFinish = sessionStorage.ToDoIsFinish;
                arrTodoList[sessionStorage.ToDoIndex].State = sessionStorage.ToDoState;
                arrTodoList[sessionStorage.ToDoIndex].IsNew = sessionStorage.ToDoIsNew;
                arrTodoList[sessionStorage.ToDoIndex].IsOrg = ($('.txtActionClientName').data().ProspectType == "Org" ? 1 : 0)
                arrTodoList[sessionStorage.ToDoIndex].TaggedType = TagStateTodo;
                arrTodoList[sessionStorage.ToDoIndex].TaggedID = TagIDTodo;

                todoSetDate = '';
                SaveToDo(false);
            })

                // LEFT MENU CLICK EVENT
            .on('click', '#btnCRMPageProfile', function () {
                menuActionToSave = 'profile';
                if (isAddNewProspect) {
                    return;
                }
                var e = JSON.parse(sessionStorage.ClientInfo);
                AddBreadCrumb({
                    text: e.ClientName + ' Profile', Click: function () {
                        if (e.ProspectType == "Org") {
                            GetProspectDetails(e.ID);
                            $('#pageCRMProfileOrg').pageshow();
                            $('#divTabCompany ul > li:first').click()
                        }
                        else {
                            GetProspectDetailsPeople(e.ID);
                            $('#pageCRMProfilePeople').pageshow()
                            $('#divTabPerson ul > li:first').click()
                        }
                    }, UpdateLastchild: true
                }, '.ItemsBreadCrumbs')

                if (e.ProspectType == "Org") {
                    GetProspectDetails(e.ID);
                    $('#pageCRMProfileOrg').pageshow();
                    $('#divTabCompany ul > li:first').click()
                }
                else {
                    GetProspectDetailsPeople(e.ID);
                    $('#pageCRMProfilePeople').pageshow()
                    $('#divTabPerson ul > li:first').click()
                }

                $(window).scrollTop(0)
                EditingMode();
            })
            .on('click', '#btnCRMPageColdCall', function (v) {
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
                                    $('#pageCRMActions').pageshow();
                                    $('#btnCRMPageProfile').click();
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

                $('#pageCRMActions').pageshow();

                if (isFromColdCallPage == false) {
                    AddBreadCrumb({
                        text: 'Cold Call ', Click: function () {
                            $('#pageCRMColdCall').pageshow();
                        }, UpdateLastchild: true
                    }, '.ItemsBreadCrumbs')

                    $('.txtActionClientName').text(e.ClientName).removeData().data(e);
                    $('#txtCRMCallNotes').css('height', '100px').css('min-height', '100px');
                    $('#divColdCallContactPerson').show();
                    $('#selCRMTagInCall').val('General');
                    $('#lblTaggedAsInCall').text('');
                    TagIDCall = '';
                    TagStateCall = 'General';
                    if (e.ProspectType == "Org") {
                        GetContactDetailsOrg(e);
                    }
                    else {
                        GetContactDetailsPeople(e);
                    }

                    if (!isCallNoteSaveAsDraft) {
                        $('#txtCRMCallNotes').val('');
                    }
                    (e.ProspectType == "Org" ? $('#divColdCallContactPerson').show() : $('#divColdCallContactPerson').hide())
                }

                BackPage = 'pageMainDashboard';
                $('#divtooltipNotes').modal('hide');
                EditingMode();
            })
            .on('click', '#btnCRMPageToDo', function () {
                var e = JSON.parse(sessionStorage.ClientInfo);
                if ($(this).hasClass('disabled-menu')) {
                    return;
                }

                AddBreadCrumb({
                    text: e.ClientName + ' To-Do List', Click: function () {
                        $('#pageCRMToDo').pageshow();
                    }, UpdateLastchild: true
                }, '.ItemsBreadCrumbs')
                menuActionToSave = '';
                $('.txtActionClientName').text(e.ClientName).removeData().data(e);
                ClearTodoFields();
                GetClientToDo(e.ID);
                EditingMode();
                $('#divtooltipNotes').modal('hide');
            })
            .on('click', '#btnCRMPageSMS', function () {
                var e = JSON.parse(sessionStorage.ClientInfo);
                if (e.MobileNos == undefined && e.ContactPerson == null) {
                    NewMessageBox('No mobile number found', 'Send SMS');
                    return false;
                }

                if ($(this).hasClass('disabled-menu')) {
                    return;
                }

                AddBreadCrumb({
                    text: 'Send SMS to ' + e.ClientName, Click: function () {
                        $('#pageCRMSMS').pageshow();
                    }, UpdateLastchild: true
                }, '.ItemsBreadCrumbs')

                if (e.MobileNos == undefined && e.ContactPerson == null) {
                    NewMessageBox('No mobile number found', 'Send SMS');
                    return false;
                }
                menuActionToSave = '';
                arrSMSRecipient = [];
                GetMobileNoBySelectedRecipient(e);
                EditingMode();
                $('#txtCRMSMSMessageAction').val('').height('100px').focus();
                $('#charNum2').text('Characters left: 160');
                $('#divtooltipNotes').modal('hide');
            })
            .on('click', '#btnCRMPageOrderHistory', function () {
                var e = JSON.parse(sessionStorage.ClientInfo);
                if ($(this).hasClass('disabled-menu')) {
                    return;
                }
                AddBreadCrumb({
                    text: 'Order History of ' + e.ClientName, Click: function () {
                        $('#pageCRMInteractionHistory').pageshow();
                    }, UpdateLastchild: true
                }, '.ItemsBreadCrumbs')
                $('.txtActionClientName').text(e.ClientName);
                ERP.OrderHistory.main.controller.popup({ CustomerID: e.ID, CustomerName: e.ClientName });
                EditingMode();
                $('#divtooltipNotes').modal('hide');
            })
            .on('click', '#btnCRMPageInteractionHistory', function () {
                var e = JSON.parse(sessionStorage.ClientInfo);
                if ($(this).hasClass('disabled-menu')) {
                    return;
                }
                AddBreadCrumb({
                    text: 'Interaction History of ' + e.ClientName, Click: function () {
                        $('#pageCRMInteractionHistory').pageshow();
                    }, UpdateLastchild: true
                }, '.ItemsBreadCrumbs')
                menuActionToSave = '';
                BackPage = '#pageCRMMainDashboard, #pageDTRCRMMainDashboard';
                $('.txtActionClientName').text(e.ClientName);
                $('#selCRMFilterInteractionHistory').val('All');
                GetInteractionHistory(e.ID);
                EditingMode();
                $('#divtooltipNotes').modal('hide');
            })
            .on('click', '#btnCRMPageTakeANote', function () {
                var e = JSON.parse(sessionStorage.ClientInfo);
                if ($(this).hasClass('disabled-menu')) {
                    return;
                }
                AddBreadCrumb({
                    text: 'Take a note for ' + e.ClientName, Click: function () {
                        $('#pageCRMTakeANote').pageshow();
                    }, UpdateLastchild: true
                }, '.ItemsBreadCrumbs')
                $('#txtCRMTakeANote').val();
                $('.txtActionClientName').text(e.ClientName);
                if ($.trim($('#txtCRMTakeANote').val()) == "") {
                    $('#txtCRMTakeANote').height('300px')
                }
                $('#btnCRMSaveTicket').hide();
                $('#btnCRMOKTicket').show();
                $('#lblFilterTagInNotes').text('');
                $('#selCRMFilterTagInNotes').val('All');
                sessionStorage.StateNote = "add";
                GetTakeANote(e.ID);
                EditingMode();
                //$('#btnCRMPageTickets').click();
            })
            .on('click', '#btnCRMPageScheduleAnAction', function () {
                var e = JSON.parse(sessionStorage.ClientInfo);
                if ($(this).hasClass('disabled-menu')) {
                    return;
                }
                AddBreadCrumb({
                    text: 'Set Schedule for ' + e.ClientName, Click: function () {
                        $('#pageCRMSchedule').pageshow();
                    }, UpdateLastchild: true
                }, '.ItemsBreadCrumbs')
                menuActionToSave = '';
                $('.txtActionClientName').text(e.ClientName);
                $('#pageCRMSchedule').pageshow();
                $('#divCalendar').customcalendar();
                GetListOfTeam();
                GetSchedule();
            })
            .on('click', '#btnCRMPageOpportunity', function () {
                var e = JSON.parse(sessionStorage.ClientInfo);
                if ($(this).hasClass('disabled-menu')) {
                    return;
                }
                $('.txtActionClientName').text(e.ClientName);
                AddBreadCrumb({
                    text: 'Opportunities ' + e.ClientName, Click: function () {
                        $('#pageCRMSchedule').pageshow();
                    }, UpdateLastchild: true
                }, '.ItemsBreadCrumbs');
                GetOpportunityPage(function () {
                    Opportunity.GetOpportunityByClient(e.ID) // Opportunity.js
                });
            })
            .on('click', '#btnCRMPageTickets', function () {
                var e = JSON.parse(sessionStorage.ClientInfo);
                if ($(this).hasClass('disabled-menu')) {
                    return;
                }
                AddBreadCrumb({
                    text: 'Tickets for ' + e.ClientName, Click: function () {
                        $('#pageCRMTickets').pageshow();
                    }, UpdateLastchild: true
                }, '.ItemsBreadCrumbs')
                sessionStorage.fromServiceTicket = false;
                sessionStorage.FromNote = false;
                arrNoteTicket = [];
                $('.txtActionClientName').text(e.ClientName);
                $('.btnNoteIssueTicket, #btnCRMSaveTicket').show();
                $('#btnCRMOKTicket').hide();
                GetTicketsPage(function () {
                    Tickets.GetTicket() // Tickets.js
                });
            })
            .on('click', '#btnCRMPageSurvey', function () {
                var e = JSON.parse(sessionStorage.ClientInfo);
                if ($(this).hasClass('disabled-menu')) {
                    return;
                }
                AddBreadCrumb({
                    text: 'Surveys for ' + e.ClientName, Click: function () {
                        $('#pageCRMSurveys').pageshow();
                    }, UpdateLastchild: true
                }, '.ItemsBreadCrumbs')
                $('.txtActionClientName').text(e.ClientName);
                GetClientSurvey();
            })
            .on('click', '#btnCRMPageInternalSalesMarketing', function () {
                var e = JSON.parse(sessionStorage.ClientInfo);
                if ($(this).hasClass('disabled-menu')) {
                    return;
                }
                BreadCrumbs('.ItemsBreadCrumbs',
                    [
                        {
                            Text: 'Lead Generation', Click: function () {
                                $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                            }
                        },
                        { Text: 'Sales & Marketing Info of ' + e.ClientName }
                    ]
                );

                $('#pageCRMInternalSalesMarketing').pageshow();
            })
            .on('click', '#btnCRMPageFile', function () {
                var e = JSON.parse(sessionStorage.ClientInfo);
                if ($(this).hasClass('disabled-menu')) {
                    return;
                }
                AjaxSend({
                    url: "SecurityRole/CheckSecurityCurrentUserAcccess",
                    data: {ModuleName: 'Allow to upload files', OrgID: sessionStorage.OrgID},
                    success: function (vData) {
                        allowToUploadFiles = false;
                        if (vData.SecurityRoleData.Rows.length > 0) {
                            allowToUploadFiles = true;
                        }

                        AddBreadCrumb({
                            text: 'Files for ' + e.ClientName, Click: function () {
                                $('#pageCRMFile').pageshow();
                            }, UpdateLastchild: true
                        }, '.ItemsBreadCrumbs')
                        $('.txtActionClientName').text(e.ClientName);
                        GetFiles();
                    }
                })                
            })
            // END LEFT MENU

            .on('change', '#selCRMCity', function (e) {
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
            .on('click', '#btnCRMAddPersonOrgAffiliation', function () {
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
                    ]
                )
            })
            .on('click', '#btnCRMAddAffiliatedPerson', function () {
                BrowsePopup('LibraryProspectPeople', 'People',
                    [
                        {
                            AddButton: [{
                                Text: '+ Add New Person',
                                Click: function () {
                                    $('#btnCRMAddPeopleProspect').click();
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
                    ]
                )
            })
            .on('click', '#btnCRMAddOrgAffiliation', function () {
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
                    ]
                )
            })
            .on('keyup', '.txtSearchProspect', function (e) {
                $(window).scrollTop(0);
                if (e.keyCode == 13) {
                    datatable.search($(this).val()).draw();
                }
                $('.txtSearchProspect').val($(this).val())
            })
            .on('submit', '#formupload', function (e) {
                e.preventDefault();
                var formData = new FormData($(this)[0]);
                NewMessageBox('This might take a few moments. Please wait while the system uploading your contacts ...', 'Message');
                setTimeout(function () {
                    $.ajax({
                        url: '/CRMDashboard/UploadContacts?OrgID=' + sessionStorage.OrgID + '&date=' + $.format.date(new Date(), 'MMMM dd,yyyy hh:mm:ss a'),
                        type: 'POST',
                        data: formData,
                        async: false,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            $('#objFile').val('');
                            if (vData == "Saved") {
                                WebToast('Your contacts has been successfully uploaded', 'top');
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
            .on('click', '#btnCRMAddExcelProspect', function () {
                $('#objFile').trigger('click');
            })
            .on('change', '#objFile', function (e) {
                $('#txtCRMUploadExcelOrgID').val(sessionStorage.OrgID)
                $('#formupload').submit();
            })
            .on('click', '#btnCRMBulkAction', function () {
                PopupButtonList('', 'Bulk Actions',
                    [
                        {
                            Text: 'Send Bulk SMS',
                            Click: function () {
                                isSendingBulkMsg = true;
                                // $('.bulk').show();
                                // $('.spanbtnSMS').show();
                                $('#charNum').text('Characters left: 160');
                                $('#txtCRMRecipientsCount').text(0);
                                $('#popCRMSMS').modal('show');
                                $('#txtCRMSMSMessage').val('').focus().height('100px');
                                arrSMSRecipient = [];
                                DisplayListOfReceipient();
                            }
                        },
                        {
                            Text: 'View SMS Logs',
                            Click: function () {
                                $('#popCRMSMSLogs').modal('show');
                                GetSmSUsage();
                            }
                        }
                    ]
                )
            })
            .on('click', '#btnCRMCancelSMS', function () {
                $('#popCRMSMS').modal('hide');
            })
            .on('click', '#btnCRMSendSMS', function () {
                SendSMS();
            })
            .on('keyup', '#txtCRMSMSMessage', function (e) {
                var len = $(this).val().length;
                if (len >= 161) {
                    $(this).val($(this).val().substring(0, 160))
                } else {
                    $('#charNum').text('Characters left: ' + (160 - len));
                }
            })
            .on('click', '#btnCRMOkSMSRecipient', function () {
                $('#popCRMSMSRecipient').modal('hide');
                DisplayListOfReceipient();
            })
            .on('click', '#btnCRMCancelSMSSelectedReceipient', function () {
                isSendingBulkMsg = false;
                arrSMSRecipient = [];
                ctrUnknownNumber = 1;
                $('#listCRMofRecipient').html('');
                $('#listCRMSelectedReceipient').empty();
                $('#txtCRMSMSMessage').val('');
                $('input[name="chbbulk"]').prop('checked', false)//.checkboxradio('refresh');
                $('.bulk, .spanbtnSMS, #popCRMSMS, #popCRMSMSRecipient').hide();
            })
            .on('click', '#btnCRMViewSMSSelectedReceipient, #btnCRMViewRecipientsDetails', function () {
                $('#popCRMSMSRecipient').modal('show');
                DisplayListOfSelectedReceipient();
            })
            .on('click', '#btnCRMComposeSMSSelectedReceipient', function () {
                $('#popCRMSMS').modal('show');
                DisplayListOfReceipient();
            })
            .on('click', '#btnCRMSMSLogs', function () {
                $('#popCRMSMSLogs').modal('show');
                GetSmSUsage();
            })
            .on('change', '#chbCRMbulk', function () {
                var chbBulk = $(this)
                $('#tblProspectbody tr').each(function () {
                    if ($(this).css('display') != 'none') {
                        if (chbBulk.is(':checked')) {
                            $(this).find('td:eq(0)').find('input[type="checkbox"]').prop('checked', true)//.checkboxradio('refresh');
                        }
                        else {
                            $(this).find('td:eq(0)').find('input[type="checkbox"]').prop('checked', false)//.checkboxradio('refresh');
                        }
                        AddSelectedRecipient($(this).find('td:eq(0)').find('input[type="checkbox"]').data(), chbBulk.is(':checked'))
                    }
                })
                DisplayListOfReceipient();
            })
            .on('click', '#btnCRMBackToTop', function () {
                $("html, body").animate({ scrollTop: 0 }, 700);
            })
            .on('click', '#btnCRMSearchContacts', function () {
                $('#popCRMSearchContacts').modal('show');
                $('#txtCRMSearchContacts').val('').focus();
                SearchClientContact('');
            })
            .on('click', '#btnCRMAddNewNumber', function () {
                $('#btnCRMNewNumberTelcoPrefix').text('Please select')//.button('refresh');
                $('#txtCRMNewNumber').val('');
                $('#popCRMNewNumber').modal('show')
            })
            .on('click', '#btnCRMOkNewNumber', function () {
                var newNumber = '+63' + $('#btnCRMNewNumberTelcoPrefix').text() + $('#txtCRMNewNumber').val();
                arrSMSRecipient.push({
                    ID: NewID(),
                    Name: newNumber,
                    Number: [newNumber]
                })
                ctrUnknownNumber = ctrUnknownNumber + 1;
                DisplayListOfReceipient();
                $('#popCRMNewNumber').modal('hide');
            })
            .on('click', '#btnCRMCancelNewNumber', function () {
                $('#popCRMNewNumber').modal('hide');
            })
            .on('click', '#btnCRMNewNumberTelcoPrefix', function () {
                BrowsePopup('LibraryTelcoPrefixes', 'Telco Codes',
                    [
                        {
                            AddButton: [{
                                Text: 'Back',
                                Click: function () {
                                    $('#popCRMNewNumber').modal('show');
                                    $('#txtCRMNewNumber').focus();
                                }
                            }],
                            DisplayText: [{ Column: 'Prefix' }, { Column: 'TelCo' }],
                            Click: function (e) {
                                $('#popCRMNewNumber').modal('show');
                                $('#btnCRMNewNumberTelcoPrefix').text(e.Prefix)//.button('refresh');
                                $('#txtCRMNewNumber').focus();
                            }
                        }
                    ]
                )
            })
            .on('keydown', '#txtCRMNewNumber', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('keyup', '#txtCRMSMSMessageAction', function (e) {
                var len = $(this).val().length;
                if (len >= 161) {
                    $(this).val($(this).val().substring(0, 160))
                } else {
                    $('#charNum2').text('Characters left: ' + (160 - len));
                }
            })
            .on('click', '#btnCRMActionSendSMS', function () {
                if ($.trim($('.txtActionClientName').text()) == '') {
                    WebToast('Please enter recipient');
                    return;
                }

                if ($.trim($('#txtCRMSMSMessageAction').val()) == '') {
                    WebToast('Please type your message');
                    return;
                }

                PromptUserAccess('Send SMS', {
                    OnClickOKSuccess: function () {
                        SendSMS();
                    }
                })
            })
            .on('click', '#btnCRMDownloadExcelProspect', function () {
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
            .on('click', '#btnCRMImportToCRM', function () {
                NewMessageBox('This will import your customer\'s / vendor\'s data to your CRM. Are you sure you want to import?', 'Message', [
                    {
                        Text: 'Yes', Click: function () {
                            NewMessageBox('Please wait while importing customer\'s / vendor\'s data to your CRM ...', 'Message');
                            AjaxSend({
                                url: 'CRMDashboard/ImportCustomerVendorDataToCrm',
                                success: function (e) {
                                    if (e.Results == 'saved') {
                                        ShowProspectList();
                                        NewMessageBox('Successfully imported data from customer and vendor libraries.', 'Message');
                                    } else {
                                        NewMessageBox(e.Results, 'Error');
                                    }
                                }
                            })
                        }
                    }, { Text: 'No' }
                ])
            })
            .on('keydown', '#txtCRMSearchContacts', function (e) {
                if (e.keyCode == 13) {
                    SearchClientContact($(this).val())
                }
            })
            .on('click', '#btnCRMAddNote', function () {
                PromptUserAccess('Add Note', {
                    OnClickOKSuccess: function () {
                        $('#pageCRMAddNote').pageshow();
                        $('#txtCRMTakeANote').empty();
                        placeCaretAtEnd($('#txtCRMTakeANote')[0]);
                        $('.btnNoteIssueTicket, #divNoteTag').show();
                        $('#selCRMTagInNote').val('General');
                        $('#lblTaggedAsInNote').text('');
                        sessionStorage.StateNote = "add";
                        sessionStorage.StateNoteID = '';
                        sessionStorage.FromNote = true;
                        arrNoteTicket = [];
                        TagStateNote = 'General';
                        TagIDNote = '';
                    }
                });
            })
            .on('click', '#btnCRMCancelTakeANote', function () {
                if (isEditingANote) {
                    NewMessageBox('Do you want to save changes?', 'Message', [
                        {
                            Text: 'Yes',
                            Click: function () {
                                $('#btnCRMSaveTakeANote').click();
                            }
                        },
                        {
                            Text: 'No',
                            Click: function () {
                                $('#pageCRMListOfNote').pageshow();
                            }
                        }
                    ])
                } else {
                    $('#pageCRMListOfNote').pageshow();
                }
            })
            .on('click', '#btnCRMBackTakeANote, #btnCRMBackToDo, #btnCRMBackCalendar', function () {
                //BreadCrumbs('.ItemsBreadCrumbs', [ { Text: 'Lead Generation' } ] );
                AddBreadCrumb({
                    text: (isErp == false ? 'Lead Generation' : (sessionStorage.SystemAdminFor == 'dtr' ? 'Daily Time Record' : 'Support CRM')), Click: function () {
                        $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
                $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
            })
            .on('click', '#btnCRMSaveTakeANote', function () {
                SaveTakeANote(sessionStorage.StateNote, sessionStorage.StateNoteID);
            })
            .on('click', '#txtCRMDateTimeFrom', function () {
                SingleCalendar({
                    Format: 'MMMM d, yyyy',
                    Click: function (e) {
                        var dateFrom = new Date(e + ' ' + $('#btnCRMDateTimeFrom').text());
                        var dateTo = new Date($('#txtCRMDateTimeTo').val() + ' ' + $('#btnCRMDateTimeTo').text());
                        var isIncreaseDate = false;
                        var newHr; var tempDate;
                        $('#txtCRMDateTimeFrom').val(e);

                        if (dateFrom > dateTo) {
                            if (dateFrom.getHours() == 0) {
                                newHr = 24;
                                isIncreaseDate = true;
                            }
                            else {
                                newHr = dateFrom.getHours();
                            }

                            tempDate = new Date($('#txtCRMDateTimeFrom').val() + ' ' + (newHr + 1) + ':' + dateFrom.getMinutes());

                            if (isIncreaseDate) {
                                tempDate = new Date(tempDate.setDate(tempDate.getDate() + 1));
                            }
                        }
                        else if (dateFrom >= dateTo) {
                            tempDate = new Date(dateTo.setHours(dateTo.getHours() + 1))
                        }
                        $('#txtCRMDateTimeTo').val($.format.date(tempDate, 'MMM d, yyyy'));
                        $('#btnCRMDateTimeTo').text($.format.date(tempDate, 'h:mm a'))//.button('refresh');
                    }
                })
            })
            .on('click', '#btnCRMDateTimeFrom', function () {
                CustomSetTime({
                    Click: function (e) {
                        var dateFrom = new Date($('#txtCRMDateTimeFrom').val() + ' ' + e.time);
                        var dateTo = new Date($('#txtCRMDateTimeTo').val() + ' ' + $('#btnCRMDateTimeTo').text());
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

                            tempDate = new Date($('#txtCRMDateTimeFrom').val() + ' ' + (newHr + 1) + ':' + dateFrom.getMinutes());

                            if (isIncreaseDate) {
                                tempDate = new Date(tempDate.setDate(tempDate.getDate() + 1));
                            }
                        }
                        else if (dateFrom >= dateTo) {
                            tempDate = new Date(dateTo.setHours(dateTo.getHours() + 1))
                        }

                        $('#txtCRMDateTimeTo').val($.format.date(tempDate, 'MMM d, yyyy'));
                        $('#btnCRMDateTimeTo').text($.format.date(tempDate, 'h:mm a'))//.button('refresh');
                        $('#btnCRMDateTimeFrom').text(e.time)//.button('refresh');
                    }
                })
            })
            .on('click', '#txtCRMDateTimeTo', function () {
                SingleCalendar({
                    Format: 'MMMM d, yyyy',
                    Click: function (e) {
                        var dateFrom = new Date($('#txtCRMDateTimeFrom').val() + ' ' + $('#btnCRMDateTimeFrom').text());
                        var dateTo = new Date(e + ' ' + $('#btnCRMDateTimeTo').text());
                        var isDecreaseDate = false;
                        var newHr; var tempDate;
                        $('#txtCRMDateTimeTo').val($.format.date(new Date(e), 'MMM d, yyyy'));

                        if (dateTo <= dateFrom) {
                            if (dateTo < dateFrom) {
                                if (dateTo.getHours() == 0) {
                                    newHr = 24;
                                    isDecreaseDate = true;
                                }
                                else {
                                    newHr = dateTo.getHours();
                                }

                                tempDate = new Date($('#txtCRMDateTimeTo').val() + ' ' + (newHr - 1) + ':' + dateTo.getMinutes());

                                if (isDecreaseDate) {
                                    tempDate = new Date(tempDate.setDate(tempDate.getDate() - 1));
                                }
                            }
                            else if (dateTo <= dateFrom) {
                                tempDate = new Date(dateTo.setHours(dateTo.getHours() - 1))
                            }
                            $('#txtCRMDateTimeFrom').val($.format.date(tempDate, 'MMM d, yyyy'));
                            $('#btnCRMDateTimeFrom').text($.format.date(tempDate, 'h:mm a'))//.button('refresh');
                        }
                    }
                })
            })
            .on('click', '#btnCRMDateTimeTo', function (e) {
                CustomSetTime({
                    Click: function (e) {
                        var dateFrom = new Date($('#txtCRMDateTimeFrom').val() + ' ' + $('#btnCRMDateTimeFrom').text());
                        var dateTo = new Date($('#txtCRMDateTimeTo').val() + ' ' + e.time);
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

                                tempDate = new Date($('#txtCRMDateTimeTo').val() + ' ' + (newHr - 1) + ':' + dateTo.getMinutes());

                                if (isDecreaseDate) {
                                    tempDate = new Date(tempDate.setDate(tempDate.getDate() - 1));
                                }
                            }
                            else if (dateTo <= dateFrom) {
                                tempDate = new Date(dateTo.setHours(dateTo.getHours() - 1))
                            }
                            $('#txtCRMDateTimeFrom').val($.format.date(tempDate, 'MMM d, yyyy'));
                            $('#btnCRMDateTimeFrom').text($.format.date(tempDate, 'h:mm a'))//.button('refresh');
                        }
                        $('#btnCRMDateTimeTo').text(e.time)//.button('refresh');
                    }
                })
            })
            .on('click', '#btnCRMSaveSchedAnAction', function () {
                if ($.trim($('#txtCRMSchedAnActionSubject').val()) == "") {
                    NewMessageBox('Please type your subject', 'Schedule An Action');
                    return false;
                }

                SaveSchedAnAction();
            })
            .on('change', 'input[name="chbAllDay"]', function () {
                if ($(this).is(':checked')) {
                    $('#btnCRMDateTimeFrom, #btnCRMDateTimeTo').parent().hide();
                }
                else {
                    $('#btnCRMDateTimeFrom, #btnCRMDateTimeTo').parent().show();
                }
            })
            .on('click', '#btnCRMBackListOfEvents', function () {
                $('#pageCRMSchedule').pageshow();
            })
            .on('click', '#btnCRMBackSchedAnAction', function () {
                ClearSchedAnAction();
                if (isFromListEvent) {
                    $('#pageCRMListofEvents').pageshow();
                    isFromListEvent = false;
                }
                else {
                    $('#pageCRMSchedule').pageshow();
                }
            })
            .on('click', '#btnCRMAddSched', function () {
                PromptUserAccess('Add Schedule', {
                    OnClickOKSuccess: function () {
                        ClearSchedAnAction();
                        $('#pageCRMNewSched').pageshow();
                        sessionStorage.StateSched = 'add';
                    }
                });
            })
            .on('click', '#btnCRMNewOpportunity', function () {
                $('#btnAddOpportunity').click(); //Opportunity.js
            })

            .on('keydown', '#txtCRMProspectNoOfEmployees, #txtCRMProspectNoFactories, #txtCRMProspectNoOffices, #txtCRMProspectNoWarehouses, #txtCRMProspectNoStores, #txtCRMProspectNoTrucks', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('keydown', '#txtCRMTakeANote', function (e) {
                if ($.trim($(this).val()) != "") {
                    isEditingANote = true;
                }
                else {
                    isEditingANote = false;
                }
            })
            .on('keydown', '#txtCRMCallNotes', function (e) {
                if ($.trim($(this).val()) != "") {
                    isEditingCallNote = true;
                }
                else {
                    isEditingCallNote = false;
                }
            })
            .on('keydown', '#pageCRMProfileOrg input', function () {
                isEditingProfile = true;
            })
            .on('change', '#pageCRMProfileOrg select', function () {
                isEditingProfile = true;
            })
            .on('keydown', '#pageCRMProfilePeople input', function () {
                isEditingProfile = true;
            })
            .on('change', '#pageCRMProfilePeople select', function () {
                isEditingProfile = true;
            })

            .on('click', '#btnCRMAddFilter', function () {
                $('#popCRMFilter').modal('show');
                $('#selCRMCategory').val('')//.selectmenu('refresh');
                $('#divBusinessIndustry, #divCompanySize, #divCity, #divFilterSurvey, #msgCategory, #msgBusinessIndustry, #msgScope, #msgConditionValue, #msgCity').hide();
            })
            .on('change', '#selCRMCategory', function () {
                var val = $.trim($(this).val());
                $('#divBusinessIndustry, #divCompanySize, #divCity, #divFilterSurvey, #msgCategory, #msgBusinessIndustry, #msgScope, #msgConditionValue, #msgCity').hide();

                if (val == "Business Industry") {
                    $('#divBusinessIndustry').pageshow();
                }
                else if (val == "Company Size") {
                    $('#divCompanySize').pageshow();
                }
                else if (val == "City") {
                    $('#divCity').pageshow();
                } else if (val == "Survey") {
                    $('#divFilterSurvey').pageshow();
                    $('#selCRMFilterSurvey').val('')//.selectmenu('refresh');
                }

                PopupReposition(); // NewMessageBox.js
            })
            .on('change', '#selCRMScope', function () {
                if ($.trim($(this).val()) == "") {
                    $('#msgScope').pageshow();
                }
                else {
                    $('#msgScope').hide();
                }
                PopupReposition(); // NewMessageBox.js
            })
            .on('keydown', '#txtCRMConditionValue', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('change', '#selCRMFilterCompanySize', function () {
                if ($.trim($(this).val()) == "ScopeOfOperations") {
                    $('#divScope').pageshow();
                }
                else if ($.trim($(this).val()) != "ScopeOfOperations") {
                    $('#divOthers').pageshow();
                }
                PopupReposition(); // NewMessageBox.js
            })
            .on('click', '#btnCRMOkFilter', function () {
                AddFilter();
            })
            .on('click', '#btnCRMCancelFilter', function () {
                $('#popCRMFilter').modal('hide');
            })

            .on('keyup', '#txtCRMOrgTelCountryCode', function () {
                OnKeySearch({
                    elem: $(this),
                    url: "CRMDashboard/GetCountryCode",
                    before: function () {
                        $('#txtCRMOrgTelCountryID').val('');
                        $('#imgOrgTelCountryCodeIndicator').removeAttr('class');
                    },
                    during: function () {
                        $('#imgOrgTelCountryCodeIndicator').removeAttr('class').addClass('img-loading');
                    },
                    after: function (data) {
                        var txtOrgTelCountryCode = $('#txtCRMOrgTelCountryCode');
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
                            $('#txtCRMOrgTelCountryID').val(data[0].ID);
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
            .on('keyup', '#txtCRMOrgTelAreaCode', function () {
                OnKeySearch({
                    elem: $(this),
                    url: "Browser/LibraryAreaCodes",
                    before: function () {
                        $('#txtCRMOrgTelAreaCodeID').val('');
                        $('#imgOrgAreaCodeIndicator').removeAttr('class');
                    },
                    during: function () {
                        $('#imgOrgAreaCodeIndicator').removeAttr('class').addClass('img-loading');
                    },
                    after: function (data) {
                        var txtOrgTelAreaCode = $('#txtCRMOrgTelAreaCode');
                        if (data.length > 0) {
                            $('#txtCRMOrgTelAreaCodeID').val(data[0].ID);
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

            .on('click', '#txtCRMRevCreditBegAsOf', function () {
                SingleCalendar({
                    Click: function (e) {
                        $('#txtCRMRevCreditBegAsOf').text(e)//.button('refresh');
                    }
                })
            })

            .on('click', '#btnCRMAddElementary', function () {
                academicLvl = 'elem';
                isNewEducation = true;
                $('#txtCRMAcademicLevel').text('Elementary');
                ClearEducation()
            })
            .on('click', '#btnCRMAddHs', function () {
                academicLvl = 'hs';
                isNewEducation = true;
                $('#txtCRMAcademicLevel').text('High School');
                ClearEducation()
            })
            .on('click', '#btnCRMAddCollege', function () {
                academicLvl = 'college';
                isNewEducation = true;
                $('#txtCRMAcademicLevel').text('College');
                ClearEducation()
            })
            .on('click', '#btnCRMOkEduHi', function () {
                var name = $.trim($('#txtCRMEdeHiSchoolName').val()),
                    started = $.trim($('#txtCRMEdeHiYearStarted').val()),
                    graduated = $.trim($('#txtCRMEdeHiYearGraduated').val());

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
                $('#prevCRMOrgProfile').hide();
            })
            .on('click', '#btnCRMPreviewClose', function () {
                $('#prevCRMOrgProfile').hide();
            })

            .on('click', '#btnCRMPrintSample', function () {
                WebPrint($('#divTicketToPrint').html());
            })
            .on('click', '#btnCRMTabCrmServiceTicket', function () {
                GetTicketsPage(function () {
                    Tickets.GetClientTicketSummary() // Tickets.js
                });
            })            
            .on('click', '#btnCRMTabCrmDashboard', function () {
                PromptUserAccess('View Dashboard', {
                    OnClickOKSuccess: function () {
                        GetDashboardRequiredData(); // in db.js
                    }
                })
            })
            .on('click', '#btnCRMTabCrmOpportunities', function () {
                GetOpportunityPage(function () {
                    Opportunity.GetOpportunity();
                })
            })
            .on('click', '#btnCRMTabCrmSurvey', function () {
                if ($('#tabDTRSurvey > *').length == 0 && CStr2(htmlSurvey) == '') {
                    AjaxSend({
                        ExactUrl: '/Survey/Creator?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            htmlSurvey = vData;                            
                            $('#tabCrmSurvey').empty().append(vData)//.hide()
                            require([GetPathFormat('/Scripts/summernote.js'),
                                GetPathFormat('/Scripts/sources/Survey/Creator.js?crc=' + assetversion),
                                //GetPathFormat('/Scripts/bootstrap.min.js?crc=')
                            ], function () {
                                    $('#tabCrmSurvey').opentabmenu()//.trigger('create');
                            });
                        }
                    });
                } else if ($('#tabDTRSurvey > *').length > 0) {
                    $('#tabDTRSurvey > *').appendTo('#tabCrmSurvey');
                }
            })
            .on('click', '#btnCRMTabCrmMyReport', function () {
                GetPageActivityReport(); //db.js
            })
            .on('click', '#btnCRMTabCrmSettings', function () {
                PromptUserAccess('Access CRM Settings*', {
                    Message: 'You don\'t have access to CRM Settings',
                    OnClickOKSuccess: function () {
                        $('#tabCrmSettings').opentabmenu();
                        GetCrmSettings(function () {
                            $('#tabCrmSettings').find('.divHeader').remove();
                            $('#tabCrmSettings').find('#CRMpageSetUpCheckList').removeAttr('data-role');
                        })
                    }
                });                
            })

            .on('click', '#btnCRMCRMRefresh', function () {
                ShowProspectList();
            })
            .on('click', '#btnCRMTicketRefresh', function () {
                Tickets.GetClientTicketSummary() // Tickets.js
            })
            .on('click', '#btnCRMOpportunityRefresh', function () {
                Opportunity.GetOpportunity();
            })

            .on('click', '.btnNoteAttachImage', function () {
                $('#fileNotePic').click();
            })
            .on('submit', '#formNotePic', function (e) {
                e.preventDefault();
                var formData = new FormData($(this)[0]);

                var reader = new FileReader();

                reader.onload = function (e) {
                    //setCaretPosition(caretPos.row, caretPos.col)
                    var file;
                    var imgID = NewID();

                    $('#fileNotePic')[0].files[0].name = imgID + '_' + $('#fileNotePic')[0].files[0].name;
                    placeCaretAtEnd($('#txtCRMTakeANote')[0]);
                    insertAtCursor('img', 'img', e.target.result, imgID, 'uploading', $('#fileNotePic')[0].files[0])
                    placeCaretAtEnd($('#txtCRMTakeANote')[0]);
                };
                reader.readAsDataURL($('#fileNotePic')[0].files[0]);
            })
            .on('change', '#fileNotePic', function (e) {
                e.preventDefault();
                if ($(this).val() != "") {
                    $('#formNotePic').submit()
                }
            })
            .on('click', '#fileNotePic', function (e) {
                caretPos = getCaretPosition();
            })

            .on('change', '#selCRMTagInTodo, #selCRMTagInTodoPopup', function (e) {
                var dis = $(this);
                var lbl;
                if (dis[0].id == 'selTagInTodo') {
                    lbl = '#lblTaggedAsInTodo'
                } else if (dis[0].id == 'selTagInTodoPopup') {
                    lbl = '#lblTaggedAsInTodoPopup'
                }
                SelectTag(dis, lbl, function (e) {
                    TagIDTodo = e.ID;
                    TagStateTodo = e.Type;

                    if (CStr2(TagIDTodo) == '') {
                        dis.val('General')
                    }
                }, function () {
                    if (dis[0].id == 'selTagInTodoPopup') {
                        $('#popCRMToDo').modal('show');
                    }
                })
            })
            .on('change', '#selCRMTagInNote', function (e) {
                var dis = $(this);
                var lbl;
                if (dis[0].id == 'selTagInNote') {
                    lbl = '#lblTaggedAsInNote';
                }
                SelectTag(dis, lbl, function (e) {
                    TagIDNote = e.ID;
                    TagStateNote = e.Type;

                    if (CStr2(TagIDNote) == '') {
                        dis.val('General')
                    }

                    if (e.Type == 'General') {
                        $('.btnNoteIssueTicket').show();
                    } else {
                        $('.btnNoteIssueTicket').hide();
                    }
                })
            })
            .on('change', '#selCRMTagInCall', function (e) {
                var dis = $(this);
                var lbl;
                if (dis[0].id == 'selTagInCall') {
                    lbl = '#lblTaggedAsInCall';
                }
                SelectTag(dis, lbl, function (e) {
                    TagIDCall = e.ID;
                    TagStateCall = e.Type;
                    $('#txtCRMCallNotes').focus();
                    if (CStr2(TagIDCall) == '') {
                        dis.val('General')
                    }
                })
            })
            .on('change', '#selCRMFilterTagInTodo, #selCRMFilterTagInNotes', function () {
                OnValidate($(this));
            })
            .on('click', '#lblFilterTagInNotes, #lblFilterTagInTodo', function () {
                OnValidate($(this));
            })
            .on('change', '#selCRMFilterInteractionHistory', function () {
                GetInteractionHistory();
            })
            .on('click', '#btnCRMTakeSurvey', function () {
                BrowsePopup('Survey/GetListOfSurvey?mod=' + (sessionStorage.SystemAdminFor || 'crm'), 'Select Survey', [
                    {
                        DisplayText: [{ Column: 'Title' }],
                        Click: function (e) {
                            sessionStorage.IsDone = false;
                            AjaxSend({
                                url: '/CRMDashboard/CheckIfCustomerSurveyExist',
                                data: {
                                    ClientID: JSON.parse(sessionStorage.ClientInfo).ID,
                                    QID: e.ID
                                },
                                success: function (res) {
                                    var link = location.origin + '/Survey/Questionaire?q=' + e.Link + '&w=crm&ClientID=' + JSON.parse(sessionStorage.ClientInfo).ID + '&OrgID=' + sessionStorage.OrgID +
                                                '&QID=' + e.ID + '&edit=1' ;

                                    if (res.Results.Rows.length > 0) {
                                        link += '&RespondentID=' + res.Results.Rows[0].ID
                                    }

                                    $('#popCRMTakeSurvey').modal('show');;
                                    $('#txtCRMCustomerUrl').val(link + '&viabrowser=1');
                                    $('#iframeSurvey').attr('src', link);
                                    
                                }
                            })
                            
                        },
                        AutoSearch: true,
                        WithSearchbox: false
                    }
                ])
            })
            .on('click', '#btnCRMFilterSurveyOk', function () {
                var arrResponse = [];
                $('#listCRMFilterQuestionsReadOnly').find('li').each(function () {
                    var dis = $(this);
                    var data = JSON.parse(JSON.stringify(dis.data('data')));
                    var inp = dis.find('input[type="checkbox"]:checked');
                    dis.find('#divQuestionTitle').find('span.chbradValidation').remove();
                    if (inp.length != 0) {
                        inp.each(function () {
                            var chbrad = $(this);
                            arrFilter.push({
                                value: chbrad.data('data').ID,
                                text: chbrad.val(),
                                category: 'Survey'
                            })
                        })
                    }
                })
                DisplayAdvanceFilter();
                FilteredText();
            })
            .on('click', '.btnKbHelp', function () {
                Bs.Loading('show', 'Please wait ... while checking your KB website if available');

                AjaxSend({
                    url: 'CRMDashboard/KbUrl',
                    success: function (e) {
                        Bs.Loading('hide');
                        if (e.Results.Rows.length > 0) {
                            if (CStr2(e.Results.Rows[0].DefaultValue) != '') {
                                window.open(CStr2(e.Results.Rows[0].DefaultValue), '_blank', 'location=yes,height=570,width=520,scrollbars=yes,status=yes');
                                return;
                            }                            
                        }
                        NewMessageBox('Sorry! We can\'t find your KB url. To setup, please go to Settings > Knowledgebase Settings.', 'Message');
                    }
                })
            })
            .on('click', '#btnCRMUploadFileByClient', function () {                
                if (allowToUploadFiles) {
                    arrExistingFiles = [];
                    $('#fileUploadByClient').click();
                } else {
                    NewMessageBox('You are not allowed to upload files. Please ask your system administrator.', 'Message');
                }
            })
            .on('change', '#fileUploadByClient', function (e) {
                e.preventDefault();
                var fileUpload = $(this).get(0);
                var files = fileUpload.files;
                var data = new FormData();

                for (var i = 0; i < files.length; i++) {
                    data.append(files[i].ID, files[i]);
                }

                CheckFileName(data, data)
                $(this).val('');
            })

            .on('click', '#btnCRMAddOrgAddress', function () {
                addressIndex = null;
                //$('#txtCRMAddressNo, #txtCRMAddressStreet, #selCRMAddressCity, #selCRMAddressCountry').val('');
                $('#popCRMAddress').modal('show');
                $('#txtCRMCompleteAddress').val('').focus();
            })
            .on('click', '#btnCRMOkAddress', function () {
                var txtCompleteAddress = $.trim($('#txtCRMCompleteAddress').val());
                //var txtAddressNo = $.trim($('#txtCRMAddressNo').val());
                //var txtAddressStreet = $.trim($('#txtCRMAddressStreet').val());
                //var selAddressCity = $('#selCRMAddressCity');
                //var selAddressCountry = $('#selCRMAddressCountry');

                //if (txtAddressNo == '') {
                //    WebToast('Please enter house or bldg. no', 'top');
                //    return;
                //}

                //if (txtAddressStreet == '') {
                //    WebToast('Please enter street name', 'top');
                //    return;
                //}

                //if ($.trim(selAddressCity.val()) == '') {
                //    WebToast('Please select city', 'top');
                //    return;
                //}

                //if ($.trim(selAddressCountry.val()) == '') {
                //    WebToast('Please select country', 'top');
                //    return;
                //}

                if (txtCompleteAddress == '') {
                    WebToast('Please enter complete address', 'top');
                    return;
                }

                var data = {
                    CompleteAddress: txtCompleteAddress
                    //BldgNo: txtAddressNo,
                    //Street: txtAddressStreet,
                    //CityID: $.trim(selAddressCity.val()),
                    //CityName: selAddressCity.find('option:selected').text(),
                    //CountryID: $.trim(selAddressCountry.val()),
                    //CountryName: selAddressCountry.find('option:selected').text(),
                }

                if (addressIndex == null) {
                    data.State = 'new';
                    arrAddress.push(data)
                } else {
                    arrAddress[addressIndex] = $.extend(arrAddress[addressIndex], data);
                }

                DisplayAddress();
                $('#popCRMAddress').modal('hide');
            })
            .on('keypress', '#txtCRMSearchClient', function (e) {
                if (e.keyCode == 13) {
                    datatable.search($.trim($(this).val())).draw();
                }
            })
            .on('change', '#selCRMStatus', function (e) {
                datatable.columns(0)
                .search($(this).val())
                .draw()
            })
            .on('change', '#selInvoiceType, #selInvoiceMadeWith', function (e) {
                ShowProspectList();
            })
            .on('click', '#btnCRMCustomerUrl', function () {
                CopyToClipboard('#txtCRMCustomerUrl');
            })
            .on('keydown', '#txtSearchCRMNotes', function (e) {
                if (e.which == 13) {
                    GetTakeANote(JSON.parse(sessionStorage.ClientInfo).ID);
                }
            })

            function DisplayAddress() {
                var lstOrgAddress = $('#lstOrgAddress');

                lstOrgAddress.empty();

                for (var i = 0; i < arrAddress.length; i++) {
                    arrAddress[i].Index = i;

                    if(arrAddress[i].State != 'remove'){
                        lstOrgAddress.append($('<li class="list-group-item d-flex align-items-center">')
                            .append($('<p class="pr-2 m-0 flex-grow-1">')
                                .data('data', arrAddress[i])
                                //.text(arrAddress[i].BldgNo + ', ' + arrAddress[i].Street + ', ' + arrAddress[i].CityName + ', ' + arrAddress[i].CountryName)
                                .text(arrAddress[i].CompleteAddress)
                                .click(function () {
                                    var data = $(this).data('data');
                                    addressIndex = data.Index;
                                    //$('#txtCRMAddressNo, #txtCRMAddressStreet, #selCRMAddressCity, #selCRMAddressCountry').val('');
                                    //$('#txtCRMAddressNo').val(data.BldgNo);
                                    //$('#txtCRMAddressStreet').val(data.Street);                                    
                                    //$('#selCRMAddressCity').val(data.CityID);
                                    //$('#selCRMAddressCountry').val(data.CountryID);
                                    $('#popCRMAddress').modal('show');
                                    $('#txtCRMCompleteAddress').val(data.CompleteAddress).focus();
                                })
                            )
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .append('<span class="bi-trash"></span>')
                                .data('data', arrAddress[i])
                                .click(function () {
                                    var data = $(this).data('data');

                                    NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                if (data.State == 'new') {
                                                    arrAddress.splice(data.Index, 1);
                                                } else {
                                                    arrAddress[data.Index].State = 'remove';
                                                }

                                                DisplayAddress();
                                            }
                                        }, { Text: 'No' }
                                    ]);
                                })
                            )
                        )
                    }                    
                }

                lstOrgAddress;//.listview('refresh');
            }

            function RenameExistingFile(vData, formData) {
                var vRows = vData.data, cnt = 1;
                var divExistingFile = $('#divExistingFile');

                divExistingFile.empty();

                if (vRows.length > 0) {
                    for (var i = 0; i < vRows.length; i++) {
                        var isExist = false, ind, data;

                        $.grep(arrExistingFiles, function (e, inx) {
                            if(e.FileName == vRows[i].FileName){
                                isExist = true;
                                ind = inx;
                                return false; //stop loop
                            }
                        })

                        if (!isExist) {
                            data = vRows[i];
                            arrExistingFiles.push(vRows[i]);
                        } else {
                            data = arrExistingFiles[ind];
                        }

                        divExistingFile.append($('<div class="row">')
                            .append($('<div class="col-auto">')
                                .text(cnt)
                            )
                            .append($('<div class="col">')
                                .append($('<input data-role="none" type="text" class="form-control">')
                                    .data('data', data)
                                    .val(data.FileName)
                                    .blur(function () {
                                        var data = $(this).data('data');
                                        var ind;

                                        $.grep(arrExistingFiles, function (e, inx) {
                                            if (e.FileName == data.FileName) {
                                                ind = inx;
                                                return false; //stop loop
                                            }
                                        })

                                        vRows[ind].NewName = $.trim($(this).val());
                                    })
                                )
                            )
                            .append($('<div class="col-auto">')
                                .append($('<button data-role="none" class="btn btn-danger">')
                                    .data('data', data)
                                    .append('<span class="bi-trash"><span>')
                                    .click(function () {
                                        var data = $(this).data('data');
                                        var ind;

                                        $.grep(arrExistingFiles, function (e, inx) {
                                            if (e.FileName == data.FileName) {
                                                ind = inx;
                                                return false; //stop loop
                                            }
                                        })

                                        vRows[ind].State = 'remove'
                                        $(this).parent().parent().remove();
                                    })
                                )
                            ).append('<br><br><br>')
                        )

                        cnt += 1;
                    }

                    $('#btnCRMContinueUpload').unbind().click(function () {
                        CheckFileName({ Files: vRows }, formData);
                        $('#popCRMFileExist').modal('hide');
                    })
                    NewMessageBoxClose();
                    $('#popCRMFileExist').modal('show');
                } else {
                    NewMessageBox('Uploading file(s). Please wait ....', 'Message');
                    AjaxSend({
                        url: '/CRMDashboard/UploadFiles?OrgID=' + sessionStorage.OrgID + '&clientID=' + JSON.parse(sessionStorage.ClientInfo).ID + '&rename=' + JSON.stringify(arrExistingFiles) + '&DateNow=' + $.format.date(new Date(), 'MMMM dd,yyyy hh:mm:ss a'),
                        type: 'POST',
                        data: formData,
                        async: true,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            if (vData.Results == 'uploaded') {
                                NewMessageBoxClose();
                                WebToast('File(s) has been successfully uploaded', 'top');
                                GetFiles();
                            } else {
                                NewMessageBox(vData.Results, 'Message');
                            }
                        }
                    });
                }
            }

            function CheckFileName(data, formData) {
                NewMessageBox('Checking file ...', 'Message');
                AjaxSend({
                    url: '/CRMDashboard/CheckFileName?OrgID=' + sessionStorage.OrgID + '&clientID=' + JSON.parse(sessionStorage.ClientInfo).ID,
                    type: 'POST',
                    data: data,
                    async: true,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (vData) {
                        RenameExistingFile(vData, formData);
                    }
                });
            }

            var dtFiles;
            function GetFiles() {
                AjaxSend({
                    url: '/CRMDashboard/GetFiles',
                    data: { ClientID: JSON.parse(sessionStorage.ClientInfo).ID },
                    success: function (e) {
                        var vRows = e.Results.Rows, ctr = 0;
                        var tblDataTableFile = $('#tblDataTableFile');

                        if ($.fn.DataTable.isDataTable(tblDataTableFile)) {
                            //empty the table first before destroying datatabase para dili magdoble ang header
                            tblDataTableFile.empty();
                            dtFiles.destroy();
                        }

                        dtFiles = tblDataTableFile.DataTable({
                            "data": vRows,
                            "pageLength": 10,
                            "columns": [
                                {
                                    "data": function (data, type, full, meta) {
                                        ctr += 1;
                                        return '<b class="ui-table-cell-label">#</b>' + ctr;
                                    },
                                    "width": "30px"
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return '<b class="ui-table-cell-label">File Name:</b>' + data.FileName;
                                    }
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return '<b class="ui-table-cell-label">Date Uploaded:</b>' + data.DateUploaded;
                                    }
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return '<b class="ui-table-cell-label">Action:</b> <button data-role="none" class="btn custom-btn-theme-b btnCircle-sm" title="Download"><img class="img-download"></button>'
                                    }
                                }
                            ],
                            "order": [[ 1, "asc" ]],
                            "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                            "responsive": true,
                            "columnDefs": [{ "targets": 1, "orderable": true }],
                            "sDom": '<"top">rt<"bottom"ip><"clear">',
                            "initComplete": function () {
                                $('#tblDataTableFile').removeClass('dataTable');
                            }
                            //"stateSave": true
                        })

                        dtFiles.on('click', 'td', function () {
                            if (dtFiles.row(this).data() != undefined) {
                                var data = dtFiles.row(this).data();
                                if (dtFiles.cell(this).index().columnVisible == 3) {
                                    PromptUserAccess('Allow to download files', {
                                        OnClickOKSuccess: function () {
                                            window.location = MergeUrlParameters({
                                                ClientID: JSON.parse(sessionStorage.ClientInfo).ID,
                                                FileName: data.ServerFileName,
                                                DownloadFileName: data.FileName
                                            }, '/Survey/Download');
                                        }
                                    })                                    
                                }
                            }
                        });
                    }
                })                
            }

            function GetListOfSurvey() {
                AjaxSend({
                    url: 'Survey/GetListOfSurvey', success: function (e) {
                        DisplayFilterSurvey(e.Results.Rows);
                    }
                })
            }

            window.DisplayFilterSurvey = function (vRows) {
                var selFilterSurvey = $('#selCRMFilterSurvey');
                selFilterSurvey.empty();
                selFilterSurvey.append(new Option('Please Select', ''));
                for (var i = 0; i < vRows.length; i++) {
                    selFilterSurvey.append(new Option(vRows[i].Title, vRows[i].ID));
                }
                //selFilterSurvey.unbind('change').change(function () {
                //    if ($(this).val() != "") {
                //        GetQuestionaire($(this).val())
                //    }
                //    $(this)//.selectmenu('refresh');
                //})
                selFilterSurvey//.selectmenu('refresh');
            }

            function GetQuestionaire(qID) {
                AjaxSend({
                    url: 'Survey/GetQuestionaire', data: { QID: qID }, success: function (q) {
                        var data = q.data;
                        var listFilterQuestionsReadOnly = $('#listCRMFilterQuestionsReadOnly');
                        $('#lblFilterQTitle').text(data.Title);
                        $('#popCRMFilter').modal('hide');
                        $('#popCRMFilterClientSurvey').modal('show');

                        listFilterQuestionsReadOnly.empty();
                        for (var i = 0; i < data.Questions.length; i++) {
                            if (data.Questions[i].QuestionType == "radio" || data.Questions[i].QuestionType == "checkbox") {
                                listFilterQuestionsReadOnly.append($('<li class="list-group-item">')
                                    .data('data', data.Questions[i])
                                    .attr({
                                        req: CBool(data.Questions[i].Required),
                                        type: data.Questions[i].QuestionType
                                    }).append($('<div style="display:table; margin-top:-18px;">')
                                        .append($('<div class="row">')
                                            .append($('<div class="col-sm-12">')
                                                .append($('<span style="font-weight:bold; font-size:16px;">').html((CBool(data.Questions[i].Required) ? '<span class="required">*</span> ' : '') + data.Questions[i].Question))
                                            )
                                        ).append(DisplayMultipleChoice(data.Questions[i])).append('<br>')
                                    )
                                );
                            }
                        }

                        $('[name="chbradChoices' + data.Index + '"]').trigger('create')//.checkboxradio()//.checkboxradio('refresh');
                    }
                })
            }

            function DisplayMultipleChoice(data) {
                var textInputs = $('<div class="row">');

                for (var i = 0; i < data.Fields.length; i++) {
                    textInputs.append($('<div class="col-sm-9">')
                            .data('data', data.Fields[i])
                            .append($('<div class="custom-checkbox">')
                                .append($('<label class="checkbox-only">')
                                    .append($('<input type="checkbox" value="' + data.Fields[i].Value + '" name="chbradChoices' + data.Index + '" data-role="checkbox" data-iconpos="notext"/>')
                                        .data('data', data.Fields[i])
                                    )
                                )
                                .append($('<div>').text(data.Fields[i].Label))
                            )
                        )
                }

                //if (data.Other != undefined) {
                //    if (data.Other.AsType == 'answer') {
                //        textInputs.append($('<div class="col-sm-9">')
                //                .data('data', data.Other)
                //                .append($('<div class="custom-' + data.QuestionType + '">')
                //                    .append($('<label class="' + data.QuestionType + '-only">')
                //                        .append($('<input type="' + data.QuestionType + '" value="Other" name="chbradChoices' + data.Index + '" data-role="checkbox" data-iconpos="notext"/>')
                //                            .data('data', data)
                //                        )
                //                    )
                //                    .append($('<div>')
                //                        .text(data.Other.Label)
                //                        .append('<br><br>')
                //                        .append($('<textarea data-role="none" class="form-control" style="height:100px; resize:none;">')
                //                            .data('data', data.Other)
                //                            .click(function (e) {
                //                                e.stopPropagation()
                //                                e.preventDefault();
                //                                $(this).parent().prev().find('input[type="checkbox"]').prop('checked', true)//.checkboxradio('refresh');
                //                            })
                //                        )
                //                    )
                //                )
                //            )
                //    } else {
                //        textInputs.append($('<div class="row">')
                //            .append($('<div class="col-sm-9">')
                //                .data('data', data.Other)
                //                .append($('<div>').text(data.Other.Label))
                //                .append('<br>')
                //                .append($('<textarea data-role="none" class="form-control" style="height:100px; resize:none;">')
                //                    .data('data', data.Fields[i])
                //                    .click(function (e) {
                //                        e.stopPropagation()
                //                        e.preventDefault();
                //                        $(this).parent().prev().find('input[type="checkbox"]').prop('checked', true)//.checkboxradio('refresh');
                //                    })
                //                )
                //            )
                //        )
                //    }
                //}

                return textInputs;
            }

            var dtClientSurvey;
            window.GetClientSurvey = function () {
                var url = 'Survey/GetClientSurvey?OrgID=' + sessionStorage.OrgID + '&ClientID=' + JSON.parse(sessionStorage.ClientInfo).ID;
                if ($.fn.DataTable.isDataTable('#tblDataTableClientSurvey')) {
                    dtClientSurvey = $('#tblDataTableClientSurvey').DataTable().ajax.url(url).load();
                    return;
                }

                dtClientSurvey = $('#tblDataTableClientSurvey').DataTable({
                    retrieve: true,
                    "order": [[1, "asc"]],
                    "pageLength": 50,
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
                                return '<b class="ui-table-cell-label">Questionaire</b>' + data.Title;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Score</b>' + CDec2(data.Score);
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Date Responded</b>' + data.DateResponded;
                            }
                        }
                    ],
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": [] }],
                    "responsive": true,
                    "columnDefs": [{ "targets": 0, "orderable": true }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">',
                    "initComplete": function () {
                        $('#tblDataTableClientSurvey').removeClass('dataTable');
                    }
                    //"stateSave": true
                })

                dtClientSurvey.on('click', 'td', function () {
                    if (dtClientSurvey.row(this).data() != undefined) {
                        var data = dtClientSurvey.row(this).data();
                        $('#lblROQTitle').text(data.Title);
                        $('#btnCRMRODLReq').unbind('click').click(function () {
                            window.location = '/Survey/ZipFiles?QID=' + data.ID + '&OrgID=' + sessionStorage.OrgID + '&RespondentID=' + data.RespondentID;
                        })
                        var arr = [];
                        for (var i = 0; i < data.Pages.length; i++){
                            arr = $.merge(data.Pages[i].Questions, arr);
                        }
                        DisplayReadOnlyQuestion(arr);
                        GetResponse(data.ID, data.RespondentID);
                    }
                });

                //$("#txtCRMSearchSurvey").unbind().keypress(function (e) {
                //    if (e.keyCode == 13) {
                //        dtClientSurvey.search($.trim($(this).val())).draw();
                //    }
                //});
            }

            function DisplayReadOnlyQuestion(arrQuestions) {
                var listClientQuestionsReadOnly = $('#listCRMClientQuestionsReadOnly');
                listClientQuestionsReadOnly.empty();

                for (var i = 0; i < arrQuestions.length; i++) {
                    if (arrQuestions[i].QuestionType == 'Page Text') {
                        continue;
                    }

                    if (arrQuestions[i].QuestionType != 'Matrix') {
                        listClientQuestionsReadOnly.append($('<li class="list-group-item">')
                            .append($('<div>')
                                .css('font-weight', 'bold')
                                .html(HTMLDecode(arrQuestions[i].Question))
                            )
                            .append($('<div qid="' + arrQuestions[i].ID + '" class="divResponse" style="margin-left:20px;">'))
                        )
                    } else {
                        IsUseCheckbox = arrQuestions[i].IsUseCheckbox;
                        listClientQuestionsReadOnly.append($('<li class="list-group-item">')
                            .append($('<div>')
                                .css('font-weight', 'bold')
                                .html(HTMLDecode(arrQuestions[i].Question))
                            )
                            .append('<br>')
                            .append($('<div style="overflow:auto;">')
                                .append(DisplayMatrix(arrQuestions[i], true))
                            )
                        )
                    }
                }
                $('#popCRMROClientSurvey').modal('show');
            }

            function GetResponse(questionaireID, respondentID) {
                AjaxSend({
                    url: 'Survey/GetResponse',
                    data: { QID: questionaireID, RID: respondentID },
                    success: function (e) {
                        var vRows = e.data;
                        $('.tdresponse').empty();

                        for (var i = 0; i < vRows.length; i++) {
                            if (vRows[i].Type != 'Matrix') {
                                var divAns = $('div[qid="' + vRows[i].QuestionID + '"]');

                                divAns.append((divAns.text().length == 0 ? '<br>' : '<br><br>')
                                        + (vRows[i].Type == "Multiple Inputs" ? vRows[i].Label + ': ' : '- ') + vRows[i].Answer
                                    )
                            } else {
                                $('tr[rowid="' + vRows[i].MatrixRowID + '"]').find('td[colid="' + vRows[i].FieldID + '"]')
                                    .empty()
                                    .append('<span class="glyphicon ' + (CBool(IsUseCheckbox) ? 'glyphicon-ok' : 'glyphicon-record') + '"></span>')
                            }
                        }
                    }
                })
            }

            function DisplayMatrix(data, isReadOnly) {
                var table = $('<table style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">'),
                    tHead = $('<thead>'), tHeadRow = $('<tr class="custom-theme-f">'), tBody = $('<tbody>'), matrixChoices = '';
                var choiceType = (CBool(data.IsUseCheckbox) ? 'checkbox' : 'radio');
                tHeadRow.append($('<th style="min-width:280px;">'))
                for (var i = 0; i < data.Fields.Columns.length; i++) {
                    tHeadRow.append($('<th class="txt-center" style="min-width:100px;">')
                            .data('data', data.Fields.Columns[i])
                            .text(data.Fields.Columns[i].Label)
                        )
                    matrixChoices += '<td class="tdresponse" colid="' + data.Fields.Columns[i].ID + '"></td>'
                }
                tHead.append(tHeadRow)

                for (var i = 0; i < data.Fields.Rows.length; i++) {
                    var row = $('<tr>')
                            .data('data', data.Fields.Rows[i])
                            .attr('rowid', data.Fields.Rows[i].ID)
                            .append($('<td>')
                                .text(data.Fields.Rows[i].Label)
                            )
                            .append(matrixChoices)
                    tBody.append(row)
                }

                table.append(tHead).append(tBody);
                return table;
            }

            function OnValidate(dis) {
                var container, lbl;
                if (dis[0].id == 'selFilterTagInTodo' || dis[0].id == 'lblFilterTagInTodo') {
                    container = '#listCRMToDo';
                    lbl = '#lblFilterTagInTodo';
                } else if (dis[0].id == 'selFilterTagInNotes' || dis[0].id == 'lblFilterTagInNotes') {
                    container = '#lstTakenNotes';
                    lbl = '#lblFilterTagInNotes';
                }

                if (dis.val() == 'Issue Ticket' || dis.attr('filteredby') == 'Issue Ticket') {
                    GetClientIssuesService(function (e) {
                        $(lbl).attr('filteredby', 'Issue Ticket').text('Ticket No. ' + e.No);
                        FilterByTag(dis, container, 'Ticket No. ' + e.No);
                    })
                } else if (dis.val() == 'Opportunities' || dis.attr('filteredby') == 'Opportunities') {
                    GetClientOpportunities(function (e) {
                        $(lbl).attr('filteredby', 'Opportunities').text('Opportunity named ' + e.Name);
                        FilterByTag(dis, container, 'Opportunity named ' + e.Name);
                    })
                } else {
                    $(lbl).text('');
                    FilterByTag(dis, container);
                }
            }

            function FilterByTag(selector, container, e) {
                $(container).find('> li').show();
                var val = (CStr2(selector.val()) == "" ? selector.attr('filteredby') : selector.val())
                if (val != 'All') {
                    $(container).find('> li').each(function () {
                        if ($.trim($(this).attr('taggeddetails')) != (e || val)) {
                            $(this).hide();
                        }
                    })
                }
            }

            function SelectTag(dis, lbl, callback, closecallback) {
                var arrSelectedTag = {}
                if (dis.val() == 'General') {
                    arrSelectedTag.Type = dis.val();
                    arrSelectedTag.ID = '';
                    arrSelectedTag.Text = '';
                    $(lbl).text(arrSelectedTag.Text);
                    return callback(arrSelectedTag);
                } else if (dis.val() == "Issue Ticket") {
                    GetClientIssuesService(function (e) {
                        arrSelectedTag.Type = dis.val();
                        arrSelectedTag.ID = e.ID;
                        arrSelectedTag.Text = 'Ticket No. ' + e.No;
                        $(lbl).text(arrSelectedTag.Text);
                        return callback(arrSelectedTag);
                    }, closecallback)
                } else if (dis.val() == "Opportunities") {
                    GetClientOpportunities(function (e) {
                        arrSelectedTag.Type = dis.val();
                        arrSelectedTag.ID = e.ID;
                        arrSelectedTag.Text = 'Opportunity named ' + e.Name;
                        $(lbl).text(arrSelectedTag.Text);
                        return callback(arrSelectedTag);
                    }, closecallback)
                }
            }

            function GetClientOpportunities(callback, closecallback) {
                BrowsePopup('CRMDashboard/OpportunityList', 'Opportunity', [{
                    DisplayText: [{ Label: 'Name:', Column: 'Name' }, {
                        Label: 'Size:', Column: function (e) {
                            return CDec2(e.Size);
                        }
                    }],
                    AutoSearch: true,
                    WithSearchbox: false,
                    Click: function (e) {
                        if (typeof callback != 'undefined') {
                            callback(e);
                        }

                        if (typeof closecallback != 'undefined') {
                            closecallback(e);
                        }
                    },
                    Close: closecallback,
                    ReturnObjectName: 'data'
                }], { ClientID: JSON.parse(sessionStorage.ClientInfo).ID })
            }

            function GetClientIssuesService(callback, closecallback) {
                BrowsePopup('CRMDashboard/TicketList', 'Issue', [{
                    DisplayText: [{ Label: 'Ticket #', Column: 'No' }, { Label: 'Category', Column: 'Type' }, { Label: 'Product/Service', Column: 'ProductServices', Style: { width: '200px' } }, { Label: 'Status', Column: 'Status' }],
                    AutoSearch: true,
                    WithSearchbox: false,
                    Click: function (e) {
                        if (typeof callback != 'undefined') {
                            callback(e);
                        }

                        if (typeof closecallback != 'undefined') {
                            closecallback(e);
                        }
                    },
                    StyleFormat: 'table',
                    Close: closecallback
                }], { ClientID: JSON.parse(sessionStorage.ClientInfo).ID, Type: 'Details', Filter: 'All', IsDatatable: false })
            }

            function GetCrmSettings(callback) {
                if (CStr2(htmlSettings) == '') {
                    AjaxSend({
                        ExactUrl: '/CRMDashboard/SystemAdministration?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            htmlSettings = vData;
                            $('#divCrmSettings').empty().append(vData)//.trigger('create');
                            //$('#divMainOpportunity').pageshow();
                            require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/flatui.js?v=' + assetversion),
                            GetPathFormat('/Scripts/sources/CRMDashboard/SystemAdministration.js?v=' + assetversion),
                            GetPathFormat('/Scripts/sources/SetupChecklist/NewPromptUser.js?v=' + assetversion),
                            GetPathFormat('/Scripts/sources/CRMDashboard/SystemAdministrationQueries.js?v=' + assetversion),
                            GetPathFormat('/Scripts/sources/CRMDashboard/SystemAdministrationView.js?v=' + assetversion)
                            ], function () {
                
                                if (callback != undefined) {
                                    callback();
                                    //$('#divCrmSettings').trigger('create')
                                }
                            });
                            
                        }
                    });
                } else {
                    if (callback != undefined) {
                        callback();
                    }
                }
            }

            function HTMLDecode(str) {
                var htmlstr = $('<textarea />').html(str).text();
                return htmlstr;
            }

            function HTMLEncode(str) {
                var i = str.length,
                    aRet = [];

                while (i--) {
                    var iC = str[i].charCodeAt();
                    if (iC == 60 || iC == 62) {
                        aRet[i] = '&#' + iC + ';';
                    } else {
                        aRet[i] = str[i];
                    }
                }
                return aRet.join('');
            }

            function insertAtCursor(type, text, src, imgID, imgStatus, dataimg) {
                var sel = window.getSelection && window.getSelection();
                if (sel) {
                    if (type == 'img') {
                        var img = document.createElement(text)
                        var br = document.createElement("br");
                        var range = sel.getRangeAt(0);
                        range.deleteContents();
                        range.insertNode(img);
                        img.setAttribute('class', 'crmpic new')
                        img.setAttribute('src', src)
                        img.setAttribute('style', 'max-width:200px')
                        img.setAttribute('imgid', imgID)
                        img.setAttribute('status', imgStatus)
                        img.setAttribute('alt', 'Loading Image ...')
                        $('[imgid="' + imgID + '"]').data('file', dataimg)

                    } else {
                        var txt = document.createTextNode(text)
                        var range = sel.getRangeAt(0);
                        range.deleteContents();
                        range.insertNode(txt);
                    }
                }
            }

            function placeCaretAtEnd(el) {
                el.focus();
                if (typeof window.getSelection != "undefined"
                        && typeof document.createRange != "undefined") {
                    var range = document.createRange();
                    range.selectNodeContents(el);
                    range.collapse(false);
                    var sel = window.getSelection();
                    sel.removeAllRanges();
                    sel.addRange(range);
                } else if (typeof document.body.createTextRange != "undefined") {
                    var textRange = document.body.createTextRange();
                    textRange.moveToElementText(el);
                    textRange.collapse(false);
                    textRange.select();
                }
            }

            function getCaretPosition() {
                var caretOffset = 0;
                if (typeof window.getSelection != "undefined") {
                    var range = window.getSelection().getRangeAt(0);
                    var preCaretRange = range.cloneRange();
                    preCaretRange.selectNodeContents($('#txtCRMTakeANote')[0]);
                    preCaretRange.setEnd(range.endContainer, range.endOffset);
                    caretOffset = preCaretRange.toString().length;
                } else if (typeof document.selection != "undefined" && document.selection.type != "Control") {
                    var textRange = document.selection.createRange();
                    var preCaretTextRange = document.body.createTextRange();
                    preCaretTextRange.moveToElementText($('#txtCRMTakeANote')[0]);
                    preCaretTextRange.setEndPoint("EndToEnd", textRange);
                    caretOffset = preCaretTextRange.text.length;
                }
                return caretOffset;
            }

            function GetTicketsPage(callback) {
                if (CStr2(htmlTickets) == '') {
                    AjaxSend({
                        ExactUrl: '/CRMDashboard/Tickets?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            htmlTickets = vData;
                            $('#divCRMPopups').append(vData).trigger('create');
                            //$('#divMainOpportunity').pageshow();
                            if (callback != undefined) {
                                callback();
                            }
                            Tickets.GetAdditionalDataForTicket();
                        }
                    });
                } else {
                    callback();
                }
            }

            function GetOpportunityPage(callback) {
                if (CStr2(htmlOpportunity) == '') {
                    AjaxSend({
                        ExactUrl: '/CRMDashboard/Opportunity?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            htmlOpportunity = vData;
                            $('#divCRMPopups').append(vData).trigger('create');
                            //$('#divMainOpportunity').pageshow();
                            if (callback != undefined) {
                                callback();
                            }
                            Opportunity.LoadRequiredData();
                            Opportunity.GetFunnelTemplate();
                        }
                    });
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
                        list.append($('<li class="list-group-item d-flex align-items-center">')
                                .data(arr[i])
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        .html($.trim(arr[i].Name))
                                        .data(arr[i])
                                        .click(function (e) {
                                            EditEducation($(this).data());
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
                                        .data(arr[i])
                                        .click(function (e) {

                                        })
                                )
                            )
                    }
                }

                list;//.listview('refresh');
                $('#popCRMEducation').modal('hide');
            }

            function EditEducation(data) {
                ClearEducation();

                academicLvl = data.Level;
                ctrIndexEducation = CInt2(data.No) - 1;
                isNewEducation = false;

                if (data.Level == 'elem') {
                    $('#txtCRMAcademicLevel').text('Elementary');
                } else if (data.Level == 'hs') {
                    $('#txtCRMAcademicLevel').text('High School');
                } else if (data.Level == 'college') {
                    $('#txtCRMAcademicLevel').text('College');
                }

                $('#txtCRMEdeHiSchoolName').val(data.Name).focus();
                $('#txtCRMEdeHiYearStarted').val(data.Started);
                $('#txtCRMEdeHiYearGraduated').val(data.Graduated);
            }

            function ClearEducation() {
                $('#popCRMEducation').modal('show');
                $('#txtCRMEdeHiID').val('');
                $('#txtCRMEdeHiSchoolName').val('').focus();
                $('#txtCRMEdeHiYearStarted').val('');
                $('#txtCRMEdeHiYearGraduated').val('');
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
                var category = $('#selCRMCategory');
                var businessindustry = $('#selCRMFilterByBusinessIndustry');
                var city = $('#selCRMFilterByCity');
                var companysize = $('#selCRMFilterCompanySize');
                var survey = $('#selCRMFilterSurvey');
                var scope = $('#selCRMScope');
                var condition = $('#selCRMCondition');
                var conditionval = $('#txtCRMConditionValue');

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
                        else if ($.trim(category.val()) == "Survey") {

                            arrFilter.push({
                                category: category.val(),
                                value: survey.val(),
                                text: 'Survey: ' + survey.find('option:selected').text(),
                                state: 'new'
                            })
                        }
                    }
                    else {
                        $('#msgCategory').pageshow();
                        return;
                    }
                    $('#popCRMFilter').modal('hide');
                    $('#selCRMCategory').val('').change()//.selectmenu('refresh')
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
                                $('#btnCRMSaveColdCall').click();
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
                                .append($('<div class="row mb-3 align-items-center border bg-light rounded">')
                                    .css('cursor', 'pointer')
                                    .append($('<div class="col-md-auto">')
                                        .data(vRows[i].Detail)
                                        .append($('<span>')
                                            .addClass('custom-badge')
                                            .css(defaultColor)
                                            .text(vRows[i].Detail.SchedActionName.slice(0, 1))
                                        )
                                    )
                                    .append($('<div class="col-md">')
                                        .data(vRows[i].Detail)
                                        .html('<b>' + vRows[i].Detail.Subject + '</b><br><br>' +
                                                time
                                            )
                                        .click(function () {
                                            EditEvent($(this))
                                            isFromListEvent = true;
                                            $('#pageCRMNewSched').pageshow();
                                            sessionStorage.StateSched = 'edit';
                                        })
                                    )
                                    .append($('<div class="col-md-auto">')
                                        .data(vRows[i].Detail)
                                        .append($('<button data-role="none" title="Set Date" class="btn btn-danger">')
                                            .append('<span class="bi-trash"></span>')
                                        )
                                        .click(function () {
                                            var dis = $(this);
                                            PromptUserAccess('Deactivate Schedule', {
                                                OnClickOKSuccess: function () {
                                                    NewMessageBox('Are you sure you want to deactivate this record?', 'Message', [
                                                        {
                                                            Text: 'Yes', Click: function () {
                                                                EditEvent(dis);
                                                                sessionStorage.StateSched = 'remove';
                                                                $('#btnCRMSaveSchedAnAction').click();
                                                            }
                                                        }
                                                    ])
                                                }
                                            });
                                        })
                                    )
                                )
                            )
                    }
                }
            }

            function EditEvent(dis) {
                if (CBool(dis.data('IsActionAllDay'))) {
                    $('#btnCRMDateTimeFrom').parent().hide();
                    $('#btnCRMDateTimeTo').parent().hide();
                }
                else {
                    $('#btnCRMDateTimeFrom').parent().show();
                    $('#btnCRMDateTimeTo').parent().show();
                }
                $('#txtCRMSchedAnActionID').val(dis.data('ID'));
                $('#txtCRMSchedAnActionSubject').val(dis.data('Subject'));
                $('#selCRMSchedAnAction').val(dis.data('SchedActionName'))//.selectmenu('refresh');
                $('input[name="chbAllDay"]').prop('checked', CBool(dis.data('IsActionAllDay')))//.checkboxradio('refresh');
                $('#txtCRMDateTimeFrom').val($.format.date(new Date(dis.data('SchedActionDateFrom')), 'MMMM dd, yyyy'));
                $('#btnCRMDateTimeFrom').text($.format.date(new Date(dis.data('SchedActionDateFrom')), 'h:mm a'))//.button('refresh');
                $('#txtCRMDateTimeTo').val($.format.date(new Date(dis.data('SchedActionDateTo')), 'MMMM dd, yyyy'));
                $('#btnCRMDateTimeTo').text($.format.date(new Date(dis.data('SchedActionDateTo')), 'h:mm a'))//.button('refresh');
                $('#txtCRMNextActionLocation').val(dis.data('Location'));
                $('#txtCRMNextActionNotes').val(dis.data('Note')).height('100px');
                $('#selCRMNextActionTeam').val(dis.data('SchedActionTeamID'))//.selectmenu('refresh');
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
                    dateFrom = $('#btnCRMDateTimeFrom').text();
                    dateTo = $('#btnCRMDateTimeTo').text();
                }

                var pData = {
                    OrgID: sessionStorage.OrgID,
                    SchedAnActionID: $.trim($('#txtCRMSchedAnActionID').val()),
                    ClientID: JSON.parse(sessionStorage.ClientInfo).ID,
                    NextActionName: $('#selCRMSchedAnAction option:selected').val(),
                    Subject: $.trim($('#txtCRMSchedAnActionSubject').val()),
                    Location: $.trim($('#txtCRMNextActionLocation').val()),
                    IsAllDay: $('input[name="chbAllDay"]').is(':checked'),
                    DateTimeFrom: $.trim($('#txtCRMDateTimeFrom').val()) + ' ' + dateFrom,
                    DateTimeTo: $.trim($('#txtCRMDateTimeTo').val()) + ' ' + dateTo,
                    Note: $.trim($('#txtCRMNextActionNotes').val()),
                    TeamAssign: $('#selCRMNextActionTeam option:selected').val(),
                    State: sessionStorage.StateSched,
                    Date: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a')
                }

                if (pData.State == 'edit') {
                    PromptUserAccess('Edit Schedule', {
                        OnClickOKSuccess: function () {
                            SaveScheduleNow();
                        }
                    });
                    return;
                } else {
                    SaveScheduleNow();
                }

                function SaveScheduleNow() {
                    AjaxSend({
                        url: 'CRMDashboard/SaveSchedAnAction',
                        data: pData,
                        success: function (vData) {
                            if (vData.Results == "Saved") {
                                WebToast('Successfully Saved');
                                $('#pageCRMSchedule').pageshow();
                                ClearSchedAnAction();
                                GetSchedule();
                            }
                            else {
                                NewMessageBox('ERROR: ' + vData.Results);
                            }
                        }
                    })
                }
            }

            function ClearSchedAnAction() {
                $('#txtCRMSchedAnActionID').val('');
                $('#txtCRMSchedAnActionSubject').val('');
                $('input[name="chbAllDay"]').prop('checked', false)//.checkboxradio('refresh');
                $('#txtCRMDateTimeFrom').val(sessionStorage.CurrentCalendarSelectedDate);
                $('#btnCRMDateTimeFrom').val('1:00 AM')//.button('refresh');
                $('#txtCRMDateTimeTo').val(sessionStorage.CurrentCalendarSelectedDate);
                $('#btnCRMDateTimeTo').val('2:00 AM')//.button('refresh');
                $('#txtCRMNextActionLocation').val('');
                $('#txtCRMNextActionNotes').val('').height('100px');
                $('#selCRMNextActionTeam').val('')//.selectmenu('refresh');
            }

            function GetListOfTeam() {
                var selNextActionTeam = $('#selCRMNextActionTeam');

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
                $('#pageCRMListOfNote').pageshow();
                var lstTakenNotes = $('#lstTakenNotes');

                lstTakenNotes.empty().append('<li>Loading ...</li>');
                $('#lstIssueTickets').empty();

                AjaxSend({
                    url: 'CRMDashboard/GetTakeANote',
                    data: { ClientID: id, SearchKey: $.trim($('#txtSearchCRMNotes').val()) },
                    success: function (vData) {
                        var vRows = vData.data;

                        lstTakenNotes.empty()

                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                var newNote = vRows[i].Note; var ticketnum = ''; var tag = '';
                                //if(newNote.length > 100){
                                //    vRows[i].isElippse = true;
                                //    newNote = newNote.substr(0, 100) + ' ... <br> <a data-role="none" class="custom-link">(read more)</a>'
                                //}
                                if (vRows[i].TicketNumber != undefined) {
                                    ticketnum = '<span style="float:left; font-size:12px; color:#757575;">Ticket No: ' + vRows[i].TicketNumber + ' (' + vRows[i].TicketStatus + ')</span>'
                                }

                                if (CStr2(vRows[i].TaggedID) != '') {
                                    ticketnum = '<span style="float:left; font-size:12px; color:#757575;">File Under: ' + CStr2(vRows[i].TaggedDetails) + '</span>';
                                }

                                newNote = ticketnum + '<span style="float:right; font-size:12px; color:#757575;">' + $.format.date(new Date(vRows[i].LastUpdated), 'MMM dd, yyyy hh:mm a') + '</span><br><br>' + HTMLDecode(newNote) + ' <br><br>'

                                lstTakenNotes.append($('<li>')
                                    .attr('taggeddetails', (vRows[i].TaggedDetails || vRows[i].TaggedType))
                                    .append($('<div class="todoContainer row mb-3 align-items-center border bg-light rounded">')
                                        .css('cursor', 'pointer')
                                        .append($('<div class="col-md">')
                                            .data(vRows[i])
                                            .html(newNote)
                                            .click(function () {
                                                var data = $(this).data();
                                                PromptUserAccess('View Note', {
                                                    OnClickOKSuccess: function () {
                                                        arrNoteTicket = [];
                                                        sessionStorage.StateNote = "edit";
                                                        sessionStorage.StateNoteID = data.ID;
                                                        $('#txtCRMTakeANote').html(HTMLDecode(data.Note));
                                                        $('#pageCRMAddNote').pageshow();
                                                        $('.btnNoteIssueTicket').hide();
                                                        $('#selCRMTagInNote').val(CStr2(data.TaggedType));
                                                        $('#lblTaggedAsInNote').text(CStr2(data.TaggedDetails));
                                                        TagIDNote = CStr2(data.TaggedID);
                                                        TagStateNote = CStr2(data.TaggedType);

                                                        LoadAttachments(data.Attachments);
                                                    }
                                                })
                                            })
                                        )
                                        .append($('<div class="col-md-auto">')
                                            .data(vRows[i])
                                            .append($('<button data-role="none" title="Remove" class="btn btn-danger">')
                                                .append('<span class="bi-trash"></span>')
                                            )
                                            .click(function () {
                                                var dis = $(this).data();
                                                TagIDNote = CStr2(dis.TaggedID);
                                                TagStateNote = CStr2(dis.TaggedType);
                                                PromptUserAccess('Deactivate Note', {
                                                    OnClickOKSuccess: function () {
                                                        NewMessageBox('Are you sure you want to deactivate this note?', 'Message', [
                                                            {
                                                                Text: 'Yes', Click: function () {
                                                                    SaveTakeANote('remove', dis.ID);
                                                                }
                                                            }, { Text: 'No' }
                                                        ])
                                                    }
                                                });
                                            })
                                        )
                                    )
                                )

                                LoadAttachments(vRows[i].Attachments)
                            }
                        }
                        else {
                            lstTakenNotes.append('<li>No Notes Found</li>')
                        }
                    }
                })
            }

            function LoadAttachments(attachments) {
                if (attachments.length > 0) {
                    for (var i = 0; i < attachments.length; i++) {
                        $("[imgid='" + attachments[i].ID + "']").removeClass('new').attr('src', '../../../temp/' + sessionStorage.OrgID + '/' + attachments[i].FileName);
                    }
                }
            }

            function SaveTakeANote(state, noteid, autosave) {
                if (state == 'edit') {
                    PromptUserAccess('Edit Note', {
                        OnClickOKSuccess: function () {
                            SaveNoteNow();
                        }
                    }); return;
                } else {
                    SaveNoteNow();
                }

                function SaveNoteNow() {
                    $('#txtCRMTakeANote').find('img').removeAttr('src').attr('alt', 'Saving image...');
                    var noOfPics = $('#txtCRMTakeANote').find('.crmpic.new').length
                    var hasPics = (noOfPics > 0 ? true : false);
                    $('#btnCRMSaveTakeANote').prop('disabled', true);
                    NewMessageBox("Please wait while saving comment ...", "Message");
                    AjaxSend({
                        url: 'CRMDashboard/SaveTakeANote',
                        data: { ClientID: JSON.parse(sessionStorage.ClientInfo).ID, TaggedType: TagStateNote, TaggedID: TagIDNote, Ticket: arrNoteTicket, Note: HTMLEncode($('#txtCRMTakeANote').html()), State: state, NoteID: noteid, Date: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a') },
                        //data: formdatas,
                        success: function (vData) {
                            var result = vData.Results

                            if (hasPics) {
                                var ctrSaving = 0;
                                result = ((result).indexOf("||") > -1 ? (result).split('||')[2] : result);
                                NewMessageBox("Please wait while uploading attachment(s) ...", "Message");
                                $('#txtCRMTakeANote').find('.crmpic.new').each(function () {
                                    var formdatas = new FormData();
                                    formdatas.append('files', $(this).data('file'));

                                    $.ajax({
                                        url: '/CRMDashboard/SaveAttachFile?OrgID=' + sessionStorage.OrgID + '&RelatedID=' + result + '&ImgID=' + $(this).attr('imgid'),
                                        type: 'POST', data: formdatas, async: true, cache: false, contentType: false, processData: false,
                                        success: function (e) {
                                            ctrSaving += 1;

                                            if (e == "saved") {
                                                if (noOfPics == ctrSaving) {
                                                    ConfirmationOfSavingNote(vData)
                                                }
                                            } else {
                                                NewMessageBox(e, 'Message');
                                            }
                                        }
                                    });
                                })
                            } else {
                                ConfirmationOfSavingNote(vData)
                            }
                            isEditingANote = false;
                        }
                    })
                }
            }

            function ConfirmationOfSavingNote(vData) {
                var result = vData.Results;
                if ((result).indexOf("||") > -1) {
                    result = vData.Results.split("||");
                    NewMessageBox('Your issue ticket has been saved. Reference number for this issue is: ' +
                            '<br><br> <b>' + result[1] + '</b><br><br>' +
                            'Do you want to print support ticket?', 'Message', [
                                {
                                    Text: 'Yes', Click: function () {
                                        arrNoteTicket[0].No = result[1];
                                        arrNoteTicket[0].LastUpdated = new Date();
                                        DisplayTicketDataToPrint(arrNoteTicket[0]);
                                        WebPrint($('#divTicketToPrint').html());
                                    }
                                }, { Text: 'No' }
                            ]);
                    GetTakeANote(JSON.parse(sessionStorage.ClientInfo).ID);
                }
                else if (IsGuid(result) || result == "") {
                    WebToast('Successfully Saved', 'top');
                    GetTakeANote(JSON.parse(sessionStorage.ClientInfo).ID);
                }
                else if (result != "") {
                    NewMessageBox('ERROR: ' + result);
                }
                $('#btnCRMSaveTakeANote').removeAttr('disabled');
                NewMessageBoxClose();
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
                $('#listCRMofManualRecipient').empty().removeData().data(tempArrMobileNo);

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
                        $('#listCRMofManualRecipient').append(divnum);
                    }
                }
            }

            function DisplayListOfReceipient() {
                var listofRecipient = $('#listCRMofRecipient');
                $('#txtCRMRecipientsCount').text(arrSMSRecipient.length);
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

                            $('#chbCRMbulk-' + id).prop('checked', false)//.checkboxradio('refresh');
                            DisplayListOfReceipient()
                        })
                    )
                    )
                }
            }

            function DisplayListOfSelectedReceipient() {
                var listSelectedReceipient = $('#listCRMSelectedReceipient');

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
                                        $('#chbCRMbulk-' + arrSMSRecipient[CDec2(index)].ID).prop('checked', false)//.checkboxradio('refresh');
                                        arrSMSRecipient = $.grep(arrSMSRecipient, function (e) {
                                            return e.ID != arrSMSRecipient[CDec2(index)].ID;
                                        })
                                    }

                                    DisplayListOfSelectedReceipient()
                                })
                            )

                        divs.append(divnum);
                    }

                    listSelectedReceipient.append($('<li class="list-group-item d-flex align-items-center">')
                        .data(arrSMSRecipient[i])
                        .append($('<p class="pr-2 m-0 flex-grow-1">')
                            .html(arrSMSRecipient[i].Name + '<br><br>')
                            .append(divs)
                        )
                        .append($('<button data-role="none" class="btn btn-danger">')
                            .append('<span class="bi-trash"></span>')
                            .data(arrSMSRecipient[i])
                            .click(function (e) {
                                var id = $(this).data().ID
                                arrSMSRecipient = $.grep(arrSMSRecipient, function (val) {
                                    return val.ID != id;
                                })
                                $('#chbCRMbulk-' + id).prop('checked', false)//.checkboxradio('refresh');
                                DisplayListOfSelectedReceipient()
                            })
                        )

                    )
                }

                listSelectedReceipient;//.listview('refresh');
            }

            function GetSmSUsage() {
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
                        { "data": function ( data, type, full, meta ) {
                                return (data.SendToName == "" ? "" : data.SendToName + " - ") + data.SendTo
                            }
                        },
                        {"data" : "Message"},
                        { "data": function ( data, type, full, meta ) {
                                return $.format.date(new Date(parseInt(data.LastUpdated.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        { "data": function ( data, type, full, meta ) {
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
                $('#popCRMSMS').modal('hide');
                $('.bulk').show();

                NewMessageBox('Saving, please wait ...');

                AjaxSend({
                    url: 'CRMDashboard/SendSmS',
                    data: {
                        Message: ($.trim($('#txtCRMSMSMessage').val()) == "" ? $('#txtCRMSMSMessageAction').val() : $.trim($('#txtCRMSMSMessage').val())),
                        Sender: arrSMSRecipient,
                        DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a')
                    },
                    success: function (vData) {
                        arrSMSRecipient = [];
                        NewMessageBoxClose()
                        if (vData.Results == 'Saved') {
                            $('#charNum2').text('Characters left: 160');
                            $('#txtCRMSMSMessage').val('').height('100px').focus();
                            $('#btnCRMCancelSMSSelectedReceipient').click();
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
                var lstAffiliatedPerson = $('#lstAffiliatedPerson');

                lstAffiliatedPerson.empty();

                for (var i = 0; i < arrOrgAffPersonList.length; i++) {
                    if (arrOrgAffPersonList[i].State != 'Remove') {
                        lstAffiliatedPerson.append($('<li class="list-group-item d-flex align-items-center">')
                            .data(arrOrgAffPersonList[i])
                            .append($('<p class="pr-2 m-0 flex-grow-1">')
                                .data(arrOrgAffPersonList[i])
                                .text(arrOrgAffPersonList[i].PeopleName)
                            )
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .append('<span class="bi-trash"></span>')
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
                        );//.listview();//.listview('refresh')
                    }
                }
            }

            function AddOrgAffiliation() {
                var lstOrgAffiliation = $('#lstOrgAffiliation');

                lstOrgAffiliation.empty();

                for (var i = 0; i < arrOrgAffList.length; i++) {
                    if (arrOrgAffList[i].State != 'Remove') {
                        lstOrgAffiliation.append($('<li class="list-group-item d-flex align-items-center">')
                            .data(arrOrgAffList[i])
                            .append($('<p class="pr-2 m-0 flex-grow-1">')
                                .data(arrOrgAffList[i])
                                .text(arrOrgAffList[i].CompanyName)
                            )
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .append('<span class="bi-trash"></span>')
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
                        );//.listview();//.listview('refresh')
                    }
                }
            }

            function AddPersonOrgAffiliation() {
                var lstPersonOrgAffiliation = $('#lstPersonOrgAffiliation');

                lstPersonOrgAffiliation.empty();

                for (var i = 0; i < arrPersonOrgAffList.length; i++) {
                    if (arrPersonOrgAffList[i].State != 'Remove') {
                        lstPersonOrgAffiliation.append($('<li class="list-group-item d-flex align-items-center">')
                            .data(arrPersonOrgAffList[i])
                            .append($('<p class="pr-2 m-0 flex-grow-1">')
                                .data(arrPersonOrgAffList[i])
                                .text(arrPersonOrgAffList[i].CompanyName)
                            )
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .append('<span class="bi-trash"></span>')
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
                        );//.listview();//.listview('refresh')
                    }
                }
            }

            function PreSaveToDo() {
                if ($.trim($('#txtCRMToDo').val()) == '') {
                    $('#txtCRMToDo').focus();
                    return;
                }

                PromptUserAccess('Add To-do', {
                    OnClickOKSuccess: function () {
                        arrTodoList.push({
                            TodoRankNo: (intTodoInfoCtr == 0 ? 1 : intTodoInfoCtr + 1),
                            ClientID: $('.txtActionClientName').data().ID,
                            ToDoID: $('#txtCRMToDoID').val(),
                            ToDo: $('#txtCRMToDo').val(),
                            ToDoDueDate: todoSetDate,
                            ToDoOrder: (intTodoInfoCtr == 0 ? 1 : intTodoInfoCtr + 1),
                            ToDoFinish: '0',
                            State: 'Add',
                            IsNew: 1,
                            IsOrg: ($('.txtActionClientName').data().ProspectType == "Org" ? 1 : 0),
                            TaggedType: TagStateTodo,
                            TaggedID: TagIDTodo
                        })
                        SaveToDo(false);
                    }
                });
            }

            function SaveToDo(isCheckedToDo) {
                ClearTodoFields();

                if (arrTodoList[0].State == 'Edit') {
                    PromptUserAccess('Edit To-do', {
                        OnClickOKSuccess: function () {
                            SaveTodoNow();
                        }
                    });
                } else {
                    SaveTodoNow();
                }

                function SaveTodoNow() {
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
                var listToDo = $('#listCRMToDo');

                listToDo.empty();
                if (vRows.length > 0) {
                    for (var i = 0 ; i < vRows.length; i++) {
                        if (vRows[i].State != 'Remove') {
                            vRows[i].arrindex = i;
                            intTodoInfoCtr = CDec2(vRows[i].TodoRankNo);

                            listToDo.append($('<li>')
                                    .attr('taggeddetails', (vRows[i].TaggedDetails || vRows[i].TaggedType))
                                    .append($('<div class="row align-items-center bg-light rounded mb-3 border">')
                                        .css('cursor', 'pointer')
                                        .append($('<div class="col-auto">')
                                            .data(vRows[i])
                                            .append($('<div class="form-check">')
                                                .append($('<label class="form-check-label" for="chbtodo-' + i + '">')
                                                    .append($('<input type="checkbox" name="todo" id="chbtodo-' + i + '" class="form-check-input"/>')
                                                    ).data(vRows[i])
                                                    .click(function (e) {
                                                        e.stopPropagation();
                                                        e.preventDefault();
                                                        var dis = $(this);
                                                        var isCheck = CBool($('#chbCRMtodo-' + dis.data().arrindex).is(':checked'));
                                                        PromptUserAccess('Edit To-do', {
                                                            OnClickOKSuccess: function () {
                                                                $('#chbCRMtodo-' + dis.data().arrindex).prop('checked', (isCheck ? false : true))//.checkboxradio()//.checkboxradio('refresh');
                                                                isCheck = (isCheck ? false : true);
                                                                arrTodoList[dis.data().arrindex].ToDoFinish = (isCheck == true ? '1' : '0');
                                                                SaveToDo(true);
                                                            }
                                                        });
                                                    })
                                                )
                                            )                                            
                                        )
                                        .append($('<div class="col-md">')
                                            .data(vRows[i])
                                            .html('<b>' + vRows[i].ToDo + '</b><br><br>' +
                                                    'Due Date: ' + (vRows[i].ToDoDueDate == '' ? '-' : $.format.date(new Date(vRows[i].ToDoDueDate), 'MMM dd, yyyy')) +
                                                    (CStr2(vRows[i].TaggedID) == '' ? '' : '<br><br>File Under: ' + vRows[i].TaggedDetails)
                                            )
                                            .click(function () {
                                                var data = $(this).data();
                                                PromptUserAccess('View To-do', {
                                                    OnClickOKSuccess: function () {
                                                        $('#popCRMToDo').modal('show');
                                                        todoSetDate = (data.ToDoDueDate == '' ? new Date() : data.ToDoDueDate);
                                                        $('#txtCRMEditToDo').val(data.ToDo);
                                                        $('#txtCRMToDoID').val(data.ToDoID);
                                                        $('#btnCRMEditToDoSetDate').text($.format.date(new Date(todoSetDate), 'MMM dd, yyyy'))//.button('refresh');
                                                        $('#selCRMTagInTodoPopup').val(data.TaggedType);
                                                        $('#lblTaggedAsInTodoPopup').text(CStr2(data.TaggedDetails))
                                                        TagIDTodo = data.TaggedID;
                                                        TagStateTodo = data.TaggedType;
                                                        sessionStorage["ToDoState"] = 'Edit';
                                                        sessionStorage["ToDoIsNew"] = data.IsNew;
                                                        sessionStorage["ToDoIndex"] = data.arrindex;
                                                        sessionStorage["ToDoIsFinish"] = ($('#chbCRMtodo-' + data.arrindex).is(':checked') == true ? '1' : '0');
                                                        TodoRankNo = arrTodoList[data.arrindex].TodoRankNo;
                                                    }
                                                });
                                            })
                                        )
                                        .append($('<div class="col-md-auto">')
                                            .data(vRows[i])
                                            .append($('<button data-role="none" title="Set Date" class="btn btn-danger">')
                                                .append('<span class="bi-trash"></span>')
                                            )
                                            .click(function () {
                                                var data = $(this).data();
                                                var TodoRankNo = data.TodoRankNo;
                                                PromptUserAccess('Deactivate To-do', {
                                                    OnClickOKSuccess: function () {
                                                        NewMessageBox('Are you sure you want to deactivate this record?', 'Message', [
                                                        {
                                                            Text: 'Yes', Click: function () {
                                                                if (data.IsNew == 1) {
                                                                    //remove selected todo
                                                                    arrTodoList = $.grep(arrTodoList, function (val) {
                                                                        return val.TodoRankNo != TodoRankNo;
                                                                    })
                                                                }
                                                                else {
                                                                    arrTodoList[data.arrindex].State = "Remove";
                                                                }
                                                                SaveToDo(false);
                                                            }
                                                        }, { Text: 'No' }
                                                        ])
                                                    }
                                                });
                                            })
                                        )                                        
                                    )
                                )
                        }
                        $('#chbCRMtodo-' + i).prop('checked', (vRows[i].ToDoFinish == "0" ? false : true))
                        $('input[type="checkbox"]')//.checkboxradio()//.checkboxradio('refresh');
                    }
                }
            }

            function ClearTodoFields() {
                $('#txtCRMToDoID').val('');
                $('#txtCRMToDo').val('');
                $('#selCRMTagInTodo').val('General');
                $('#selCRMFilterTagInTodo').val('All');
                $('#lblTaggedAsInTodo, #lblFilterTagInTodo').text('');
                TagStateTodo = 'General';
                TagIDTodo = '';
                //$('#btnCRMToDoSetDate').text('Set Date')//.button('refresh');
                todoSetDate = '';
            }

            var dtInteractionHistory;
            function GetInteractionHistory(clientID) {
                var tblInteractionHistory = $('#tblDataTableInteractionHistory');
                var url = '/CRMDashboard/GetInteractionHistory?OrgID=' + sessionStorage.OrgID + '&ID=' + JSON.parse(sessionStorage.ClientInfo).ID + '&Filter=' + $('#selCRMFilterInteractionHistory').val();

                //tblInteractionHistory.empty().append('<tr><td colspan="6" style="text-align:center;">Please wait ...</td></tr>');

                if ($.fn.DataTable.isDataTable('#tblDataTableInteractionHistory')) {
                    dtInteractionHistory.ajax.url(url).load();
                    return
                }

                dtInteractionHistory = tblInteractionHistory.DataTable({
                    retrieve: true,
                    "order": [[1, "desc"]],
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
                                var interDate;
                                if (data.LastAction == "Call") {
                                    interDate = $.format.date(new Date(parseInt((data.CallStart).substring(6))), 'MMM dd, yyyy hh:mm:ss a');
                                }
                                else {
                                    interDate = $.format.date(new Date(parseInt((data.LastUpdated).substring(6))), 'MMM dd, yyyy hh:mm:ss a');
                                }
                                return '<b class="ui-table-cell-label">Interaction D/T:</b>' + interDate;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Interaction Type:</b>' + data.LastAction;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Interacted With:</b>' + data.CreatedBy;
                            }
                        }
                        //,{
                        //    "data": function (data, type, full, meta) {
                        //        return '<b class="ui-table-cell-label">Interaction Duration:</b>' + CStr2(data.CallDuration);
                        //    }
                        //}
                    ],
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                    "responsive": true,
                    "columnDefs": [{ "targets": 0, "orderable": true }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">',
                    "initComplete": function () {
                        $('#tblDataTableInteractionHistory').removeClass('dataTable');
                    }
                    //"stateSave": true
                })

                dtInteractionHistory.on('click', 'tr', function () {
                    if (dtInteractionHistory.row(this).data() != undefined) {
                        var data = dtInteractionHistory.row(this).data();
                        PromptUserAccess('View Interaction History', {
                            OnClickOKSuccess: function () {
                                $('#tooltipDateAndTime').text($.format.date(new Date(parseInt((data.InteractionDate || data.LastUpdated).substring(6))), 'MMM dd, yyyy hh:mm a'))
                                $('#tooltipNotes').html(((HTMLDecode(data.Notes).replace(/\r\n|\r|\n/g, "<br />") || '&nbsp;')))
                                $('#divtooltipNotes').modal('show');

                                if (data.Attachments != undefined) {
                                    var arrAttachments = (data.Attachments).split(',');
                                    var arrFiles = [];

                                    for (var i = 0; i < arrAttachments.length; i++) {
                                        var details = (arrAttachments[i]).split('_');
                                        arrFiles.push({
                                            ID: $.trim(details[0]),
                                            FileName: $.trim(arrAttachments[i])
                                        })
                                    }

                                    LoadAttachments(arrFiles);
                                }
                            }
                        })
                    }
                });
            }

            function SaveNewPersonProspect(autosave) {
                var pData = {};

                if ($.trim($('#txtCRMProspectPersonFirstName').val()) == '') {
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
                    Telphone: arrPersonTelInfoList,
                    Mobile: arrPersonMobileInfoList,
                    Email: arrPersonEmailInfoList,
                    OrgAffiliation: arrPersonOrgAffList,
                    PersonID: $('#txtCRMProspectPersonID').val(),
                    FirstName: $.trim($('#txtCRMProspectPersonFirstName').val()),
                    MiddleName: $.trim($('#txtCRMProspectPersonMiddleName').val()),
                    LastName: $.trim($('#txtCRMProspectPersonLastName').val()),
                    Birthday: (IsDate($('#btnCRMProspectPersonBirthday').text()) ? $.trim($('#btnCRMProspectPersonBirthday').text()) : ''),
                    Position: $('#txtCRMProspectPersonPosition').val(),
                    PositionDescription: $('#txtCRMProspectPersonPositionDescription').val(),
                    AddressID: $('#txtCRMPersonAddressID').val(),
                    HouseBuildingNo: $('#txtCRMPersonHouseBuildingNo').val(),
                    Street: $('#txtCRMPersonStreet').val(),
                    City: $('#selCRMPersonCity').val(),
                    Region: $('#selCRMPersonRegion').val(),
                    Island: $('#selCRMPersonIsland').val(),
                    Country: $('#selCRMPersonCountry').val(),
                    TypeAddress: $('#selCRMPersonTypeAddress').val(),
                    OrgID: sessionStorage.OrgID,
                    DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a'),
                    Status: $('#selCRMPersonStatus').val(),
                    Elementary: arrElementary,
                    HighSchool: arrHighSchool,
                    College: arrCollege
                }

                AjaxSend({
                    url: 'CRMDashboard/SaveNewPersonProspect?mod=' + (sessionStorage.SystemAdminFor || 'crm'),
                    data: pData,
                    success: function (vData) {
                        isEditingProfile = false;
                        if (IsGuid(vData.Results) && autosave == undefined) {
                            WebToast('Successfully Saved', 'top');
                            datatable.draw(); // refresh datatable after saving

                            if (isAddNewProspect) {
                                $('#listCRMAction').find('li > a:not(:first)').removeClass('disabled-menu');
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
                                $('#txtCRMProspectPersonID').val(vData.Results)
                            }
                            else {
                                ClearPersonProspectVariablesFields();
                                $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                                isFromColdCallPage = false;
                            }
                        }
                        else if (!IsGuid(vData.Results)) {
                            NewMessageBox('ERROR: ' + vData.Results);
                        }
                    }
                })
            }

            function AddPersonEmailInfo() {

                var pLV = $('#lstPersonEmail');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrPersonEmailInfoList.length; i++) {
                    if (arrPersonEmailInfoList[i].EmailState != "Remove") {
                        arrPersonEmailInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                                .data(arrPersonEmailInfoList[i])
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        .html($.trim(arrPersonEmailInfoList[i].EmailAdd) +
                                            "<br>" + $.trim(arrPersonEmailInfoList[i].EmailType)
                                        )
                                        .data(arrPersonEmailInfoList[i])
                                        .click(function (e) {
                                            EditPersonEmailInfo($(this).data().arrindex);
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
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

                pLV;//.listview('refresh');
                $('#popCRMPersonEmailInfo').modal('hide');
            }

            function EditPersonEmailInfo(arrindex) {
                $('#popCRMPersonEmailInfo').modal('show');
                PersonEmailRankNo = arrPersonEmailInfoList[arrindex].EmailRankNo;
                PersonEmailState = "Edit";
                sessionStorage["Emailindex"] = arrindex;
                sessionStorage["EmailIsNew"] = arrPersonEmailInfoList[arrindex].IsNew;
                sessionStorage["EmailState"] = arrPersonEmailInfoList[arrindex].EmailState;
                $('#txtCRMPersonEmailID').val(arrPersonEmailInfoList[arrindex].EmailAddID == undefined ? "" : arrPersonEmailInfoList[arrindex].EmailAddID);
                $('#txtCRMPersonEmailAdd').val(arrPersonEmailInfoList[arrindex].EmailAdd);
                $('#selCRMPersonEmailType').val(arrPersonEmailInfoList[arrindex].EmailType)//.selectmenu('refresh');
            }

            function ClearPersonEmailInfoFields() {
                $('#txtCRMPersonEmailID').val('');
                $('#txtCRMPersonEmailAdd').val('');
                $('#selCRMPersonEmailType').val('')//.selectmenu('refresh');
            }

            function AddPersonMobileInfo() {

                var pLV = $('#lstPersonMobile');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrPersonMobileInfoList.length; i++) {
                    if (arrPersonMobileInfoList[i].MobileState != "Remove") {
                        arrPersonMobileInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                                    .data(arrPersonMobileInfoList[i])
                                    .append($('<p class="pr-2 m-0 flex-grow-1">')
                                            .data(arrPersonMobileInfoList[i])
                                            .attr('ID', arrPersonMobileInfoList[i].id)
                                            .html("+" + $.trim(arrPersonMobileInfoList[i].CountryCode.split('-')[1]) + "" + $.trim(arrPersonMobileInfoList[i].MobileNo) +
                                                "<br>" + $.trim(arrPersonMobileInfoList[i].ContactType)
                                            )
                                            .click(function (e) {
                                                EditPersonMobileInfo($(this).data().arrindex);
                                            })
                                    )
                                    .append($('<button data-role="none" class="btn btn-danger">')
                                            .append('<span class="bi-trash"></span>')
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

                pLV;//.listview('refresh');
                $('#popCRMPersonMobileInfo').modal('hide');
            }

            function EditPersonMobileInfo(arrindex) {
                $('#popCRMPersonMobileInfo').modal('show');
                PersonMobileRankNo = arrPersonMobileInfoList[arrindex].MobileRankNo;
                PersonMobileState = "Edit";
                sessionStorage["Mobileindex"] = arrindex;
                sessionStorage["MobileIsNew"] = arrPersonMobileInfoList[arrindex].IsNew;
                sessionStorage["MobileState"] = arrPersonMobileInfoList[arrindex].MobileState;
                $('#txtCRMPersonMobileID').val(arrPersonMobileInfoList[arrindex].MobileID == undefined ? "" : arrPersonMobileInfoList[arrindex].MobileID);
                $('#txtCRMPersonTelCoPrefixID').val(arrPersonMobileInfoList[arrindex].TelCodeID);
                $('#txtCRMPersonMobileCountryID').val(arrPersonMobileInfoList[arrindex].CountryCodeID);
                $('#txtCRMPersonMobileCountryCode').text(arrPersonMobileInfoList[arrindex].CountryCode);
                //$('#txtCRMPersonTelCode').text(arrPersonMobileInfoList[arrindex].TelCode);
                $('#txtCRMPersonMobileNo').val(arrPersonMobileInfoList[arrindex].MobileNo);
                $('#selCRMPersonMobileType').val(arrPersonMobileInfoList[arrindex].ContactType)//.selectmenu('refresh');
            }

            function ClearPersonMobileInfo() {
                $('#txtCRMPersonMobileID').val('');
                $('#txtCRMPersonTelCoPrefixID').val('');
                $('#txtCRMPersonMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtCRMPersonMobileCountryCode').text('PHL - 63');
                $('#txtCRMPersonTelCode').text('Select');
                $('#txtCRMPersonMobileNo').val('');
                $('#selCRMPersonMobileType').val('')//.selectmenu('refresh');
            }

            function AddPersonTelInfo() {

                var pLV = $('#lstPersonTelephone');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrPersonTelInfoList.length; i++) {
                    if (arrPersonTelInfoList[i].TelState != "Remove") {
                        arrPersonTelInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                                .data(arrPersonTelInfoList[i])
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        .html("+" + $.trim(arrPersonTelInfoList[i].CountryCode.split('-')[1]) + " (" + $.trim(arrPersonTelInfoList[i].AreaCode.split('-')[1]) + ") " + $.trim(arrPersonTelInfoList[i].TelephoneNo) + ($.trim(arrPersonTelInfoList[i].ExtensionNo) == "" ? "" : "." + $.trim(arrPersonTelInfoList[i].ExtensionNo)) +
                                            "<br>" + $.trim(arrPersonTelInfoList[i].AreaCode.split('-')[0]) + ", " + $.trim(arrPersonTelInfoList[i].CountryCode.split('-')[0]) +
                                            "<br>" + $.trim(arrPersonTelInfoList[i].ContactType)
                                        )
                                        .data('data', arrPersonTelInfoList[i])
                                        .click(function (e) {
                                            EditPersonTelInfo($(this).data('data').arrindex);
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
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

                pLV;//.listview('refresh');
                $('#popCRMPersonTelInfo').modal('hide');
            }

            function EditPersonTelInfo(arrindex) {
                $('#popCRMPersonTelInfo').modal('show');

                PersonTelRankNo = arrPersonTelInfoList[arrindex].TelRankNo;
                PersonTelState = "Edit";
                sessionStorage["Telindex"] = arrindex;
                sessionStorage["TelIsNew"] = arrPersonTelInfoList[arrindex].IsNew;
                sessionStorage["TelState"] = arrPersonTelInfoList[arrindex].TelState;
                $('#txtCRMPersonTelID').val(arrPersonTelInfoList[arrindex].TelID == undefined ? "" : arrPersonTelInfoList[arrindex].TelID);
                $('#txtCRMPersonTelAreaCodeID').val(arrPersonTelInfoList[arrindex].AreaCodeID);
                $('#txtCRMPersonTelCountryID').val(arrPersonTelInfoList[arrindex].CountryCodeID);
                $('#txtCRMPersonCountryCode').text(arrPersonTelInfoList[arrindex].CountryCode);
                $('#txtCRMPersonAreaCode').text(arrPersonTelInfoList[arrindex].AreaCode);
                $('#txtCRMPersonTelephoneNum').val(arrPersonTelInfoList[arrindex].TelephoneNo);
                $('#txtCRMPersonTelephoneExt').val(arrPersonTelInfoList[arrindex].ExtensionNo);
                $('#selCRMPersonTelType').val(arrPersonTelInfoList[arrindex].ContactType)//.selectmenu('refresh');
            }

            function ClearPersonTelInfo() {
                $('#txtCRMPersonTelID').val('');
                $('#txtCRMPersonTelAreaCodeID').val('');
                $('#txtCRMPersonTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtCRMPersonCountryCode').text('PHL - 63');
                $('#txtCRMPersonAreaCode').text('Select');
                $('#txtCRMPersonTelephoneNum').val('');
                $('#txtCRMPersonTelephoneExt').val('');
                $('#selCRMPersonTelType').val('')//.selectmenu('refresh');
            }

            function SaveNewOrgProspect(autosave) {
                var pData = {};

                if ($.trim($('#txtCRMProspectCompanyName').val()) == '') {
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

                pData = {
                    Telphone: arrTelInfoList,
                    Mobile: arrMobileInfoList,
                    Email: arrEmailInfoList,
                    Website: arrWebsiteList,
                    ContactPerson: arrContactPersonList,
                    ContactPersonTelphone: arrContactPersonTelInfoList,
                    ContactPersonMobile: arrContactPersonMobileInfoList,
                    ContactPersonEmail: arrContactPersonEmailInfoList,
                    OrgAffiliation: arrOrgAffList,
                    OrgAffiliatedPerson: arrOrgAffPersonList,
                    Address: arrAddress,
                    CompanyID: $('#txtCRMProspectOrgID').val(),
                    CompanyName: $.trim($('#txtCRMProspectCompanyName').val()),
                    CompanyBuildingAddress: $('#txtCRMProspectHouseBldgNo').val(),
                    CompanyStreetAddress: $('#txtCRMProspectStreetName').val(),
                    CompanyCityAddress: $('#selCRMProspectCities').val(),
                    CompanyRegionAddress: $('#selCRMProspectOrgRegion').val(),
                    Island: $('#selCRMProspectIsland').val(),
                    CompanyCountryAddress: $('#selCRMProspectCountries').val(),
                    CompanyBackground: $('#txtCRMProspectCompanyBackground').val(),
                    CompanyMission: $('#txtCRMProspectCompanyMission').val(),
                    CompanyVision: $('#txtCRMProspectCompanyVision').val(),
                    CompanyFoundingDate: $('#txtCRMProspectCompanyFounding').val(),
                    BusinessIndustryID: $('#txtCRMBusIndustryID').val(),
                    OrgID: sessionStorage.OrgID,
                    DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a'),
                    Status: $('#selCRMOrgStatus').val(),
                    NoOfEmployees: $('#txtCRMProspectNoOfEmployees').val(),
                    Revenue: $('#txtCRMProspectRevenuePerYear').val(),
                    Scope: $('#selCRMProspectScopeOperation').val(),
                    Factories: $('#txtCRMProspectNoFactories').val(),
                    Offices: $('#txtCRMProspectNoOffices').val(),
                    Warehouses: $('#txtCRMProspectNoWarehouses').val(),
                    Stores: $('#txtCRMProspectNoStores').val(),
                    Trucks: $('#txtCRMProspectNoTrucks').val()
                }

                if (isClientActive) {
                    if (pData.Status == 'Inactive') {
                        PromptUserAccess('Deactivate Profile', {
                            OnClickOKSuccess: function () {
                                SaveOrgNow();
                            }
                        })
                        return;
                    }
                }

                SaveOrgNow();

                function SaveOrgNow() {
                    AjaxSend({
                        url: 'CRMDashboard/SaveNewOrgProspect',
                        data: pData,
                        success: function (vData) {
                            isEditingProfile = false;
                            if (IsGuid(vData.Results) && autosave == undefined) {
                                WebToast('Successfully Saved', 'top');
                                datatable.draw(); // refresh datatable after saving

                                if (isAddNewProspect) {
                                    $('#listCRMAction').find('li > a:not(:first)').removeClass('disabled-menu');
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
                                    $('#txtCRMProspectOrgID').val(vData.Results)
                                }
                                else {
                                    ClearOrgProspectVariablesFields();
                                    isFromColdCallPage = false;
                                    $('#divCompanyProfile').show();
                                    $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                                }
                                CacheBusinessIndustry();
                            }
                            else if (!IsGuid(vData.Results)) {
                                NewMessageBox(vData.Results, 'Message');
                            }
                        }
                    })
                }
            }

            function AddContactPersonEmailInfo() {

                var pLV = $('#lstContactPersonEmail');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrContactPersonEmailInfoList.length; i++) {
                    if (arrContactPersonEmailInfoList[i].EmailState != "Remove") {
                        arrContactPersonEmailInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                                .data(arrContactPersonEmailInfoList[i])
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        .data(arrContactPersonEmailInfoList[i])
                                        .html($.trim(arrContactPersonEmailInfoList[i].EmailAdd) +
                                            "<br>" + $.trim(arrContactPersonEmailInfoList[i].EmailType)
                                        )
                                        .click(function (e) {
                                            EditContactPersonEmailInfo($(this).data().arrindex);
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
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

                pLV;//.listview('refresh');
                $('#popCRMContactPersonEmailInfo').modal('hide');
            }

            function EditContactPersonEmailInfo(arrindex) {
                $('#popCRMContactPersonEmailInfo').modal('show');
                ContactPersonEmailRankNo = arrContactPersonEmailInfoList[arrindex].EmailRankNo;
                ContactPersonEmailState = "Edit";
                sessionStorage["Emailindex"] = arrindex;
                sessionStorage["EmailIsNew"] = arrContactPersonEmailInfoList[arrindex].IsNew;
                sessionStorage["EmailState"] = arrContactPersonEmailInfoList[arrindex].EmailState;
                $('#txContacttPersonEmailID').val(arrContactPersonEmailInfoList[arrindex].EmailAddID == undefined ? "" : arrContactPersonEmailInfoList[arrindex].EmailAddID);
                $('#txtCRMContactPersonEmailAdd').val(arrContactPersonEmailInfoList[arrindex].EmailAdd);
                $('#selCRMContactPersonEmailType').val(arrContactPersonEmailInfoList[arrindex].EmailType)//.selectmenu('refresh');
            }

            function ClearContactPersonEmailInfoFields() {
                $('#txtCRMContactPersonEmailID').val('');
                $('#txtCRMContactPersonEmailAdd').val('');
                $('#selCRMContactPersonEmailType').val('')//.selectmenu('refresh');
                $('#divContactPersonEmailHome').pageshow();
            }

            function AddContactPersonMobileInfo() {

                var pLV = $('#lstContactPersonMobile');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrContactPersonMobileInfoList.length; i++) {
                    if (arrContactPersonMobileInfoList[i].MobileState != "Remove") {
                        arrContactPersonMobileInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                                    .data(arrContactPersonMobileInfoList[i])
                                    .append($('<p class="pr-2 m-0 flex-grow-1">')
                                            .attr('ID', arrContactPersonMobileInfoList[i].id)
                                            .html("+" + $.trim(arrContactPersonMobileInfoList[i].CountryCode.split('-')[1]) + "" + $.trim(arrContactPersonMobileInfoList[i].MobileNo) +
                                                "<br>" + $.trim(arrContactPersonMobileInfoList[i].ContactType)
                                            )
                                            .data(arrContactPersonMobileInfoList[i])
                                            .click(function (e) {
                                                EditContactPersonMobileInfo($(this).data().arrindex);
                                            })
                                    )
                                    .append($('<button data-role="none" class="btn btn-danger">')
                                            .append('<span class="bi-trash"></span>')
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

                pLV;//.listview('refresh');
                $('#popCRMContactPersonMobileInfo').modal('hide');
            }

            function EditContactPersonMobileInfo(arrindex) {
                $('#popCRMContactPersonMobileInfo').modal('show');
                ContactPersonMobileRankNo = arrContactPersonMobileInfoList[arrindex].MobileRankNo;
                ContactPersonMobileState = "Edit";
                sessionStorage["Mobileindex"] = arrindex;
                sessionStorage["MobileIsNew"] = arrContactPersonMobileInfoList[arrindex].IsNew;
                sessionStorage["MobileState"] = arrContactPersonMobileInfoList[arrindex].MobileState;
                $('#txtCRMContactPersonMobileID').val(arrContactPersonMobileInfoList[arrindex].MobileID == undefined ? "" : arrContactPersonMobileInfoList[arrindex].MobileID);
                $('#txtCRMContactPersonTelCoPrefixID').val(arrContactPersonMobileInfoList[arrindex].TelCodeID);
                $('#txtCRMContactPersonMobileCountryID').val(arrContactPersonMobileInfoList[arrindex].CountryCodeID);
                $('#txtCRMContactPersonMobileCountryCode').text(arrContactPersonMobileInfoList[arrindex].CountryCode);
                //$('#txtCRMContactPersonTelCode').text(arrContactPersonMobileInfoList[arrindex].TelCode);
                $('#txtCRMContactPersonMobileNo').val(arrContactPersonMobileInfoList[arrindex].MobileNo);
                $('#selCRMContactPersonMobileType').val(arrContactPersonMobileInfoList[arrindex].ContactType)//.selectmenu('refresh');
            }

            function ClearContactPersonMobileInfo() {
                $('#txtCRMContactPersonMobileID').val('');
                $('#txtCRMContactPersonTelCoPrefixID').val('');
                $('#txtCRMContactPersonMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtCRMContactPersonMobileCountryCode').text('PHL - 63');
                //$('#txtCRMContactPersonTelCode').text('Select');
                $('#txtCRMContactPersonMobileNo').val('');
                $('#selCRMContactPersonMobileType').val('')//.selectmenu('refresh');
                $('#divContactPersonMobileHome').pageshow();
            }

            function AddContactPersonTelInfo() {

                var pLV = $('#lstContactPersonTelephone');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrContactPersonTelInfoList.length; i++) {
                    if (arrContactPersonTelInfoList[i].TelState != "Remove") {
                        arrContactPersonTelInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                                .data(arrContactPersonTelInfoList[i])
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
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
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
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

                pLV;//.listview('refresh');
                $('#popCRMContactPersonTelInfo').modal('hide');
            }

            function EditContactPersonTelInfo(arrindex) {
                $('#popCRMContactPersonTelInfo').modal('show');

                ContactPersonTelRankNo = arrContactPersonTelInfoList[arrindex].TelRankNo;
                ContactPersonTelState = "Edit";
                sessionStorage["Telindex"] = arrindex;
                sessionStorage["TelIsNew"] = arrContactPersonTelInfoList[arrindex].IsNew;
                sessionStorage["TelState"] = arrContactPersonTelInfoList[arrindex].TelState;
                $('#txtCRMContactPersonTelID').val(arrContactPersonTelInfoList[arrindex].TelID == undefined ? "" : arrContactPersonTelInfoList[arrindex].TelID);
                $('#txtCRMContactPersonTelAreaCodeID').val(arrContactPersonTelInfoList[arrindex].AreaCodeID);
                $('#txtCRMContactPersonTelCountryID').val(arrContactPersonTelInfoList[arrindex].CountryCodeID);
                $('#txtCRMContactPersonCountryCode').text(arrContactPersonTelInfoList[arrindex].CountryCode);
                $('#txtCRMContactPersonAreaCode').text(arrContactPersonTelInfoList[arrindex].AreaCode);
                $('#txtCRMContactPersonTelephoneNum').val(arrContactPersonTelInfoList[arrindex].TelephoneNo);
                $('#txtCRMContactPersonTelephoneExt').val(arrContactPersonTelInfoList[arrindex].ExtensionNo);
                $('#selCRMContactPersonTelType').val(arrContactPersonTelInfoList[arrindex].ContactType)//.selectmenu('refresh');
            }

            function ClearContactPersonTelInfo() {
                $('#txtCRMContactPersonTelID').val('');
                $('#txtCRMContactPersonTelAreaCodeID').val('');
                $('#txtCRMContactPersonTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtCRMContactPersonCountryCode').text('PHL - 63');
                $('#txtCRMContactPersonAreaCode').text('Select');
                $('#txtCRMContactPersonTelephoneNum').val('');
                $('#txtCRMContactPersonTelephoneExt').val('');
                $('#selCRMContactPersonTelType').val('')//.selectmenu('refresh');
                $('#divContactPersonTel').pageshow();
            }

            function AddContactPersonInfo() {
                var pLV = $('#lstCRMOrgContactPerson');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrContactPersonList.length; i++) {
                    if (arrContactPersonList[i].ContactPersonState != "Remove") {
                        arrContactPersonList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                                .data(arrContactPersonList[i])
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
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
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
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

                        pLV.append(pItem);
                    }
                }

                pLV;//.listview('refresh');
                $('#popCRMContactPersonProfile').modal('hide');

                ClearContactPersonFields();
            }

            function EditContactPersonInfo(arrindex) {
                ClearContactPersonFields();

                $('#popCRMContactPersonProfile').modal('show');
                ContactPersonRankNo = arrContactPersonList[arrindex].ContactPersonRankNo;
                ContactPersonState = "Edit";
                sessionStorage["Contactindex"] = arrindex;
                sessionStorage["ContactPersonIsNew"] = arrContactPersonList[arrindex].IsNew;
                sessionStorage["ContactPersonState"] = arrContactPersonList[arrindex].ContactPersonState;
                $('#txtCRMContactPersonID').val(arrContactPersonList[arrindex].ID == undefined ? "" : arrContactPersonList[arrindex].ID);
                $('#txtCRMPeopleID').val(arrContactPersonList[arrindex].PeopleID == undefined ? "" : arrContactPersonList[arrindex].PeopleID);
                $('#txtCRMContactFirstName').val(arrContactPersonList[arrindex].Firstname);
                $('#txtCRMContactMiddleName').val(arrContactPersonList[arrindex].Middlename);
                $('#txtCRMContactLastName').val(arrContactPersonList[arrindex].Lastname);
                $('#txtCRMContactPersonPosition').val(arrContactPersonList[arrindex].Position);
                $('#txtCRMContactPersonPositionDescription').val(arrContactPersonList[arrindex].PositionDescription);
                $('#txtCRMContactPersonAddressID').val(arrContactPersonList[arrindex].AddressID);
                $('#txtCRMContactPersonHouseBuildingNo').val(arrContactPersonList[arrindex].HouseBuildingNo);
                $('#txtCRMContactPersonStreet').val(arrContactPersonList[arrindex].Street);
                $('#selCRMContactPersonCity').val(arrContactPersonList[arrindex].City)//.selectmenu('refresh');
                $('#selCRMContactPersonCountry').val((arrContactPersonList[arrindex].Country || 'e783ed13-aa98-4cc1-918e-d49b05c0167e'))//.selectmenu('refresh');
                $('#selCRMContactPersonTypeAddress').val(arrContactPersonList[arrindex].TypeAddress)//.selectmenu('refresh');

                if (arrContactPersonList[arrindex].IsNew == "0") {
                    if (arrContactPersonList[arrindex].FromServer == undefined) {
                        AjaxSend({
                            url: 'CRMDashboard/GetContactDetailsPeople',
                            data: { ID: arrContactPersonList[arrindex].PeopleID },
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
                $('#txtCRMPeopleID').val('');
                $('#txtCRMContactPersonID').val('');
                $('#txtCRMContactFirstName').val('');
                $('#txtCRMContactMiddleName').val('');
                $('#txtCRMContactLastName').val('');
                $('#txtCRMContactPersonPosition').val('');
                $('#txtCRMContactPersonPositionDescription').val('');
                $('#txtCRMContactPersonAddressID').val('');
                $('#txtCRMContactPersonHouseBuildingNo').val('');
                $('#txtCRMContactPersonStreet').val('');
                $('#selCRMContactPersonCity').val('')//.selectmenu('refresh');
                $('#selCRMContactPersonCountry').val('e783ed13-aa98-4cc1-918e-d49b05c0167e')//.selectmenu('refresh');
                $('#selCRMContactPersonTypeAddress').val();
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

                var pLV = $('#lstCRMOrgWebsite');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrWebsiteList.length; i++) {
                    if (arrWebsiteList[i].WebsiteState != "Remove") {
                        arrWebsiteList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                                .data(arrWebsiteList[i])
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        .data(arrWebsiteList[i])
                                        .html($.trim(arrWebsiteList[i].Website))
                                        .click(function (e) {
                                            EditWebsiteInfo($(this).data().arrindex);
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
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

                pLV;//.listview('refresh');
                $('#popCRMOrgWebsiteInfo').modal('hide');
            }

            function EditWebsiteInfo(arrindex) {
                $('#popCRMOrgWebsiteInfo').modal('show');
                WebsiteRankNo = arrWebsiteList[arrindex].WebsiteRankNo;
                WebsiteState = "Edit";
                sessionStorage["Websiteindex"] = arrindex;
                sessionStorage["WebsiteIsNew"] = arrWebsiteList[arrindex].IsNew;
                sessionStorage["WebsiteState"] = arrWebsiteList[arrindex].EmailState;
                $('#txtCRMOrgWebsiteID').val(arrWebsiteList[arrindex].WebsiteID == undefined ? "" : arrWebsiteList[arrindex].WebsiteID);
                $('#txtCRMWebsiteUrl').val(arrWebsiteList[arrindex].Website);
            }

            function ClearWebsiteFields() {
                $('#txtCRMOrgWebsite').val('');
                $('#txtCRMWebsiteUrl').val('');
            }

            function AddEmailInfo() {

                var pLV = $('#lstCRMOrgEmail');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrEmailInfoList.length; i++) {
                    if (arrEmailInfoList[i].EmailState != "Remove") {
                        arrEmailInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                                .data(arrEmailInfoList[i])
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        .data(arrEmailInfoList[i])
                                        .html($.trim(arrEmailInfoList[i].EmailAdd) +
                                            "<br>" + $.trim(arrEmailInfoList[i].EmailType)
                                        )
                                        .click(function (e) {
                                            EditEmailInfo($(this).data().arrindex);
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
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

                pLV;//.listview('refresh');
                $('#popCRMOrgEmailInfo').modal('hide');
            }

            function EditEmailInfo(arrindex) {
                $('#popCRMOrgEmailInfo').modal('show');
                EmailRankNo = arrEmailInfoList[arrindex].EmailRankNo;
                EmailState = "Edit";
                sessionStorage["Emailindex"] = arrindex;
                sessionStorage["EmailIsNew"] = arrEmailInfoList[arrindex].IsNew;
                sessionStorage["EmailState"] = arrEmailInfoList[arrindex].EmailState;
                $('#txtCRMOrgEmailID').val(arrEmailInfoList[arrindex].EmailAddID == undefined ? "" : arrEmailInfoList[arrindex].EmailAddID);
                $('#txtCRMEmailAdd').val(arrEmailInfoList[arrindex].EmailAdd);
                $('#selCRMOrgEmailType').val(arrEmailInfoList[arrindex].EmailType)//.selectmenu('refresh');
            }

            function ClearEmailInfoFields() {
                $('#txtCRMOrgEmailID').val('');
                $('#txtCRMEmailAdd').val('');
                $('#selCRMOrgEmailType').val('')//.selectmenu('refresh');
            }

            function AddMobileInfo() {

                var pLV = $('#lstCRMOrgMobile');
                var pItem;
                pLV.html('');
                for (var i = 0; i < arrMobileInfoList.length; i++) {
                    if (arrMobileInfoList[i].MobileState != "Remove") {
                        arrMobileInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                                    .data(arrMobileInfoList[i])
                                    .append($('<p class="pr-2 m-0 flex-grow-1">')
                                            .data(arrMobileInfoList[i])
                                            .attr('ID', arrMobileInfoList[i].id)
                                            .html("+" + $.trim(arrMobileInfoList[i].CountryCode.split('-')[1]) + "" + $.trim(arrMobileInfoList[i].MobileNo) +
                                                "<br>" + $.trim(arrMobileInfoList[i].ContactType)
                                            )
                                            .click(function (e) {
                                                EditMobileInfo($(this).data().arrindex);
                                            })
                                    )
                                    .append($('<button data-role="none" class="btn btn-danger">')
                                            .append('<span class="bi-trash"></span>')
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

                pLV;//.listview('refresh');
                $('#popCRMOrgMobileInfo').modal('hide');
            }

            function EditMobileInfo(arrindex) {
                $('#popCRMOrgMobileInfo').modal('show');
                MobileRankNo = arrMobileInfoList[arrindex].MobileRankNo;
                MobileState = "Edit";
                sessionStorage["Mobileindex"] = arrindex;
                sessionStorage["MobileIsNew"] = arrMobileInfoList[arrindex].IsNew;
                sessionStorage["MobileState"] = arrMobileInfoList[arrindex].MobileState;
                $('#txtCRMOrgMobileID').val(arrMobileInfoList[arrindex].MobileID == undefined ? "" : arrMobileInfoList[arrindex].MobileID);
                $('#txtCRMOrgTelCoPrefixID').val(arrMobileInfoList[arrindex].TelCodeID);
                $('#txtCRMOrgMobileCountryID').val(arrMobileInfoList[arrindex].CountryCodeID);
                $('#txtCRMMobileCountryCode').text(arrMobileInfoList[arrindex].CountryCode);
                //$('#txtCRMTelCode').text(arrMobileInfoList[arrindex].TelCode);
                $('#txtCRMMobileNo').val(arrMobileInfoList[arrindex].MobileNo);
                $('#selCRMOrgMobileType').val(arrMobileInfoList[arrindex].ContactType)//.selectmenu('refresh');
            }

            function ClearMobileInfo() {
                $('#txtCRMOrgMobileID').val('');
                $('#txtCRMOrgTelCoPrefixID').val('');
                $('#txtCRMOrgMobileCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtCRMMobileCountryCode').text('PHL - 63');
                //$('#txtCRMTelCode').text('Select');
                $('#txtCRMMobileNo').val('');
                $('#selCRMOrgMobileType').val('')//.selectmenu('refresh');
            }

            function AddTelInfo() {

                var pLV = $('#lstCRMOrgTelephone');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrTelInfoList.length; i++) {
                    if (arrTelInfoList[i].TelState != "Remove") {
                        arrTelInfoList[i].arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                                .data(arrTelInfoList[i])
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        //
                                        //.html((arrTelInfoList[i].CountryCode.split('-')[1] == undefined ? "" : "+" + $.trim(arrTelInfoList[i].CountryCode.split('-')[1])) + " (" + (arrTelInfoList[i].AreaCode.split('-')[1] == undefined ? $.trim(arrTelInfoList[i].AreaCode.split('-')[0]) : $.trim(arrTelInfoList[i].AreaCode.split('-')[1])) + ") " + $.trim(arrTelInfoList[i].TelephoneNo) + ($.trim(arrTelInfoList[i].ExtensionNo) == "" ? "" : "." + $.trim(arrTelInfoList[i].ExtensionNo)) +
                                        //    "<br>" + (arrTelInfoList[i].AreaCode.split('-')[1] == undefined ? "" : $.trim(arrTelInfoList[i].AreaCode.split('-')[1]) + ', ') + $.trim(arrTelInfoList[i].CountryCode.split('-')[0]) +
                                        //    "<br>" + $.trim(arrTelInfoList[i].ContactType)
                                        //)
                                        .html((arrTelInfoList[i].CountryCode.split('-')[1] == undefined ? "" : "+" + $.trim(arrTelInfoList[i].CountryCode.split('-')[1])) + " (" + (CStr2(arrTelInfoList[i].AreaCode) == '' ? '' : arrTelInfoList[i].AreaCode.split('-')[1] == undefined ? $.trim(arrTelInfoList[i].AreaCode.split('-')[0]) : $.trim(arrTelInfoList[i].AreaCode.split('-')[1])) + ") " + $.trim(arrTelInfoList[i].TelephoneNo) + ($.trim(arrTelInfoList[i].ExtensionNo) == "" ? "" : "." + $.trim(arrTelInfoList[i].ExtensionNo)) +
                                            "<br>" + (CStr2(arrTelInfoList[i].AreaCode) == '' ? '' : $.trim(arrTelInfoList[i].AreaCode.split('-')[0])) + ', ' + $.trim(arrTelInfoList[i].CountryCode.split('-')[0]) +
                                            "<br>" + $.trim(arrTelInfoList[i].ContactType)
                                        )
                                        .data('data', arrTelInfoList[i])
                                        .click(function (e) {
                                            EditTelInfo($(this).data('data').arrindex);
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
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

                }

                pLV;//.listview('refresh');
                $('#popCRMOrgTelInfo').modal('hide');
            }

            function EditTelInfo(arrindex) {
                $('#popCRMOrgTelInfo').modal('show');

                TelRankNo = arrTelInfoList[arrindex].TelRankNo;
                TelState = "Edit";
                sessionStorage["Telindex"] = arrindex;
                sessionStorage["TelIsNew"] = arrTelInfoList[arrindex].IsNew;
                sessionStorage["TelState"] = arrTelInfoList[arrindex].TelState;
                $('#txtCRMOrgTelID').val(arrTelInfoList[arrindex].TelID == undefined ? "" : arrTelInfoList[arrindex].TelID);
                $('#txtCRMOrgTelAreaCodeID').val(arrTelInfoList[arrindex].AreaCodeID);
                $('#txtCRMOrgTelCountryID').val(arrTelInfoList[arrindex].CountryCodeID);
                $('#txtCRMCountryCode').text(arrTelInfoList[arrindex].CountryCode);
                $('#txtCRMAreaCode').text(arrTelInfoList[arrindex].AreaCode);
                $('#txtCRMTelephoneNum').val(arrTelInfoList[arrindex].TelephoneNo);
                $('#txtCRMTelephoneExt').val(arrTelInfoList[arrindex].ExtensionNo);
                $('#selCRMOrgTelType').val(arrTelInfoList[arrindex].ContactType)//.selectmenu('refresh');
            }

            function ClearTelInfo() {
                $('#txtCRMOrgTelID').val('');
                $('#txtCRMOrgTelAreaCodeID').val('');
                $('#txtCRMOrgTelCountryID').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtCRMCountryCode').text('PHL - 63');
                $('#txtCRMAreaCode').text('Select');
                $('#txtCRMTelephoneNum').val('');
                $('#txtCRMTelephoneExt').val('');
                $('#selCRMOrgTelType').val('')//.selectmenu('refresh');
            }

            function LoadRequiredData() {
                $('.OrganizationNameHeader').text(sessionStorage.OrgName);
                if (sessionStorage.SystemAdminFor != 'dtr') {
                    $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                }                
                if (sessionStorage.CRMDashboardPage == 'leadgen' || CStr2(sessionStorage.SystemAdminFor) == 'dtr') {
                    $('#divtblLeadGen').pageshow();
                }
                else {
                    $('#divtblSalesGrinder').pageshow();
                }

                var tabs = $('#tabCrmDashboard, #tabCrmTicketService, #tabCrmOpportunities, #tabCrmSurvey, #tabCrmMyReport, #tabCrmSettings');
                if (sessionStorage.CrmType == 'revsalescrm') {
                    tabs.hidetabmenu();
                    $('#tabCrmSupportCrm').opentabmenu();
                    $('.lblCRMModule').text('Sales');
                    $('#selInvoiceType, #selInvoiceMadeWith').parent().show();
                } else {
                    tabs.showtabmenu();
                    $('#tabCrmDashboard').opentabmenu();
                    $('.lblCRMModule').text('Support');
                    $('#selInvoiceType, #selInvoiceMadeWith').parent().hide();
                }

                $('#divCalendar').customcalendar();
                setRegionCollection('#selCRMProspectOrgRegion');
                setRegionCollection('#selCRMPersonRegion');
                GetDashboardRequiredData(); // in db.js
                GetProspectList();
                CacheBusinessIndustry();
                GetListOfSurvey();
                //$('.ItemsBreadCrumbs').text('');
                if (window.location.pathname == '/erp') {
                    //AddBreadCrumb({
                    //    text: 'Support CRM', Click: function () {
                    //        $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                    //    }
                    //}, '.ItemsBreadCrumbs')
                }

                $('#tblSample').DataTable({
                    pageLength: 5,
                    data: [{ No: 1, Name: '1a' }, { No: 2, Name: '3b' }, { No: 3, Name: '5c' }, { No: 4, Name: '7d' }, { No: 5, Name: '9e' },
                            { No: 6, Name: '2f' }, { No: 7, Name: '4g' }, { No: 8, Name: '6h' }, { No: 9, Name: '8i' }, { No: 10, Name: '10k' }],
                    columns: [{ data: 'No' }, { data: 'Name' }],
                    sDom: '<"top">rt<"bottom"ip><"clear">'
                })
            }

            function CacheBusinessIndustry() {
                AjaxSend({
                    url: 'CRMDashboard/CacheBusinessIndustry',
                    success: function (vData) {
                        var selFilterByBusinessIndustry = $('#selCRMFilterByBusinessIndustry');
                        selFilterByBusinessIndustry.empty();

                        selFilterByBusinessIndustry.append(new Option('NA', ''))

                        for (var i = 0; i < vData.Results.Rows.length; i++) {
                            selFilterByBusinessIndustry.append(new Option(vData.Results.Rows[i].Class, vData.Results.Rows[i].ID));
                        }

                        selFilterByBusinessIndustry//.selectmenu('refresh');
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
                var selProspectCities = $('#selCRMProspectCities');
                var selPersonCity = $('#selCRMPersonCity');
                var selContactPersonCity = $('#selCRMContactPersonCity');
                var selFilterByCity = $('#selCRMFilterByCity');
                var selAddressCity = $('#selCRMAddressCity');

                selProspectCities.empty();
                selProspectCities.append(new Option('Please Select City', ''));

                selPersonCity.empty();
                selPersonCity.append(new Option('Please Select City', ''));

                selContactPersonCity.empty();
                selContactPersonCity.append(new Option('Please Select City', ''));

                selFilterByCity.empty();
                selFilterByCity.append(new Option('NA', ''));

                selAddressCity.empty();
                selAddressCity.append(new Option('Please Select City', ''));

                AjaxSend({
                    url: 'CRMDashboard/getCityNameList',
                    success: function (vData) {


                        for (var p = 0; p < vData.CityData.Rows.length; p++) {
                            if (vData.CityData.Rows[p].Name != undefined) {
                                selProspectCities.append(new Option(vData.CityData.Rows[p].Name, vData.CityData.Rows[p].ID))
                                selPersonCity.append(new Option(vData.CityData.Rows[p].Name, vData.CityData.Rows[p].ID))
                                selContactPersonCity.append(new Option(vData.CityData.Rows[p].Name, vData.CityData.Rows[p].ID))
                                selFilterByCity.append(new Option(vData.CityData.Rows[p].Name, vData.CityData.Rows[p].ID))
                                selAddressCity.append(new Option(vData.CityData.Rows[p].Name, vData.CityData.Rows[p].ID))
                            }
                        }

                        selProspectCities//.selectmenu('refresh');
                        selPersonCity//.selectmenu('refresh');
                        selContactPersonCity//.selectmenu('refresh');
                        selFilterByCity//.selectmenu('refresh');
                        selAddressCity//.selectmenu('refresh');
                        DisplayCountries()
                    }
                });
            }

            function DisplayCountries() {
                var selProspectCountries = $('#selCRMProspectCountries');
                var selPersonCountry = $('#selCRMPersonCountry');
                var selContactPersonCountry = $('#selCRMContactPersonCountry');
                var selAddressCountry = $('#selCRMAddressCountry');

                selProspectCountries.empty();
                selProspectCountries.append(new Option('Please Select Country', ''));

                selPersonCountry.empty();
                selPersonCountry.append(new Option('Please Select Country', ''));

                selContactPersonCountry.empty();
                selContactPersonCountry.append(new Option('Please Select Country', ''));

                selAddressCountry.empty();
                selAddressCountry.append(new Option('Please Select Country', ''));

                AjaxSend({
                    url: 'CRMDashboard/getCountryNameList',
                    success: function (vData) {

                        for (var p = 0; p < vData.CountryData.Rows.length; p++) {
                            selProspectCountries.append(new Option(vData.CountryData.Rows[p].Name, vData.CountryData.Rows[p].ID))
                            selPersonCountry.append(new Option(vData.CountryData.Rows[p].Name, vData.CountryData.Rows[p].ID))
                            selContactPersonCountry.append(new Option(vData.CountryData.Rows[p].Name, vData.CountryData.Rows[p].ID))
                            selAddressCountry.append(new Option(vData.CountryData.Rows[p].Name, vData.CountryData.Rows[p].ID))
                        }

                        selProspectCountries.val('e783ed13-aa98-4cc1-918e-d49b05c0167e')//.selectmenu('refresh');
                        selPersonCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e')//.selectmenu('refresh');
                        selContactPersonCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e')//.selectmenu('refresh');
                        selAddressCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e')//.selectmenu('refresh');
                    }
                });
            }

            function GetProspectList() {
                $('#divLoading').show();
                $('#divCRMMainPage').hide();

                AjaxSend({
                    url: 'CRMDashboard/IsNewEntry',
                    success: function (vData) {
                        if (CStr2(vData.PeopleID) == '') { //if people ID is blank, then redirect to sign in
                            proj.PromptLogin();
                            //$('#divLoading').text('Your session has expired. Redirecting to sign in page ...');
                            //setTimeout(function () {
                            //    if (location.pathname == '/erp') {
                            //        pUrl = '/SignIn/ERP';
                            //    } else {
                            //        pUrl = '/SignIn/CRM';
                            //    }
                            //    window.open(GetPathFormat(pUrl), '_self');
                            //}, 3000);
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
                                        $('#divCRMMainPage').hide();
                                    } else {
                                        $('#divLoading').hide();
                                        $('#divCRMMainPage').show();
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
                var url = "/CRMDashboard/GetProspectList?OrgID=" + sessionStorage.OrgID + '&mod=' + (sessionStorage.SystemAdminFor || 'crm') + '&crmtype=' + (sessionStorage.CrmType || 'crm');

                if (sessionStorage.CrmType != 'crm') {
                    url += '&invtype=' + $('#selInvoiceType').val();
                    url += '&madewithin=' + $('#selInvoiceMadeWith').val();
                    $('#btnCRMPageOrderHistory').parent().show();
                } else {
                    $('#btnCRMPageOrderHistory').parent().hide();
                }

                $.fn.dataTable.ext.errMode = 'none'; // hide jDatatable error popup

                if (sessionStorage.CRMDashboardPage == 'leadgen') {
                    if ($.fn.DataTable.isDataTable('#tblDataTable')) {
                        //datatable.ajax.url(url).load();
                        //return
                        $('#tblDataTable').DataTable().destroy();
                    }
                    
                    datatable = $('#tblDataTable').DataTable({
                        retrieve: true,
                        "order": [[1, "asc"]],
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
                            { "data": function ( data, type, full, meta ) {
                                    if(data.ProspectType == "Org"){
                                        return "<button data-role='none' style='float:left;' class='btn btn-warning'>O</button>";
                                    }
                                    else {
                                        return "<button data-role='none' style='float:left;' class='btn btn-success'>P</button>";
                                    }
                                },
                                "width": "30px"
                            },
                            { "data": function (data, type, full, meta){
                                    return '<b class="ui-table-cell-label">Name</b>' + data.ClientName;
                                }
                            },
                            { "data": function (data, type, full, meta){
                                    return '<b class="ui-table-cell-label">Business Industry</b>' + (data.IndustryName || '');
                                },
                                "visible": (CStr2(sessionStorage.SystemAdminFor) == 'dtr' ? false : true)
                            },
                            { "data": function (data, type, full, meta){
                                    return '<b class="ui-table-cell-label">Contact Info</b>' + (data.Telephone != undefined ? 'T: ' + data.Telephone + '<br>' : '') +
                                                    (data.Mobile != undefined ? 'M: ' + data.Mobile + '<br>' : '') +
                                                    (data.Email != undefined ? 'E: ' + data.Email : '')
                                } 
                            },
                            { "data": function (data, type, full, meta){
                                    return '<b class="ui-table-cell-label">Address</b>' + (data.HouseNo != "" ? data.HouseNo + ', <br>' : '') +
                                            (data.Street != "" ? data.Street + ', <br>' : '') +
                                            (data.City != "" ? data.City + ', <br>' : '') +
                                            (data.Region != "" ? data.Region + ', <br>' : '') +
                                            (data.Country != "" ? data.Country : '')
                                }
                            },
                            { "data": function (data, type, full, meta){
                                    var listAffiliation = $('<ol style="margin:0 auto;">');
                                    var ctrAffiliation = 0;

                                    if(data.Affiliation != null){                                
                                        var affiliationLength = data.Affiliation.split(',').length;                                
                                        for(var m = 0; m < affiliationLength; m++){
                                            if(m < 2){
                                                listAffiliation.append($('<li>').text(data.Affiliation.split(',')[m]));
                                            }
                                            else if(m == (affiliationLength - 1)) {
                                                ctrAffiliation = affiliationLength - 2;
                                                listAffiliation.append($('<span>')
                                                        .css('color', '#3745FD')
                                                        .data(data)
                                                        .text('+' + ctrAffiliation + ' more')
                                                        .click(function(e){
                                                            e.stopPropagation();
                                                            //backup 2-22-2016
                                                        })
                                                    );
                                            }
                                        }
                                    }
                                    return '<b class="ui-table-cell-label">Organization Affiliation</b>' + listAffiliation.get(0).outerHTML;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    if (data.SalesOrderDate == null) {
                                        return "";
                                    }
                                    var SalesOrderDate = data.SalesOrderDate;
                                    return '<b class="ui-table-cell-label">Last Ordered Date</b>' + $.format.date(CJsonDate(SalesOrderDate), 'MMM dd, yyyy');
                                },
                                "visible": (CStr2(sessionStorage.CrmType) == 'crm' ? false : true)
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    if (data.InvoiceDate == null) {
                                        return "";
                                    }
                                    var InvoiceDate = data.InvoiceDate;
                                    return '<b class="ui-table-cell-label">Last Invoiced Date</b>' + $.format.date(CJsonDate(InvoiceDate), 'MMM dd, yyyy');
                                },
                                "visible": (CStr2(sessionStorage.CrmType) == 'crm' ? false : true)
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">For Follow Up Orders</b>' + (CBool(data.IsForFollowUp) ? 'Yes' : 'No');
                                },
                                "visible": (CStr2(sessionStorage.CrmType) == 'crm' ? false : true)
                            },
                            { "data": function (data, type, full, meta){
                                    var LastUpdated = data.LastUpdated;
                                    return '<b class="ui-table-cell-label">Date Created/Modified</b>' + $.format.date(new Date([LastUpdated.slice(0, -2), ' ', LastUpdated.substr(LastUpdated.length - 2)].join('')), 'MMM dd, yyyy hh:mm a') + '<br> <span style="font-size:12px;">by ' + data.ModifiedBy + '</span>';
                                }
                            },
                            { "data": function (data, type, full, meta){
                                    if(data.LastInteractionDate == null){
                                        return "";
                                    }
                                    var LastInteractionDate = data.LastInteractionDate;
                                    return '<b class="ui-table-cell-label">Last Interaction Date</b>' + $.format.date(new Date([LastInteractionDate.slice(0, -2), ' ', LastInteractionDate.substr(LastInteractionDate.length - 2)].join('')), 'MMM dd, yyyy hh:mm a');
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Score</b>' + CDec2(data.TotalScore);
                                },
                                "visible": (CStr2(sessionStorage.SystemAdminFor) == 'dtr' ? false : true)
                            }
                        ],
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": [3, 4, 5] }],
                        "responsive": true,
                        "columnDefs": [{ "targets": 0, "orderable": true }],
                        "sDom": '<"top">rt<"bottom"ip><"clear">',
                        "initComplete": function () {
                            $('#tblDataTable').removeClass('dataTable');
                        }
                        //"stateSave": true
                    })

                    $('#tblDataTable tbody').unbind('click').on('click', 'tr', function () {
                        var data = datatable.row(this).data();
                        if (data != undefined) {
                            PromptUserAccess('View Profile', {
                                OnClickOKSuccess: function () {
                                    ClearOrgProspectVariablesFields();
                                    MainFormOptions(data);
                                }
                            })
                        }
                    });                    
                }
                else {
                    if ($.fn.DataTable.isDataTable('#tblDataTableSalesGrinder')) {
                        datatable.draw();
                        return
                    }
                    //$('#tblDataTableSalesGrinder').DataTable().destroy();
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
                            { "data": function ( data, type, full, meta ) {
                                    if(data.ProspectType == "Org"){
                                        return "<button style='float:left;' class='btn btn-warning'>O</button>";
                                    }
                                    else {
                                        return "<button style='float:left;' class='btn btn-success'>P</button>";
                                    }
                                }
                            },
                            { "data": function (data, type, full, meta){
                                    return '<b class="ui-table-cell-label">Name</b>' + data.ClientName;
                                },
                                "width": "200px"
                            },
                            { "data": function (data, type, full, meta){
                                    return '<b class="ui-table-cell-label">Business Industry</b>' + (data.IndustryName || '');
                                },
                                "width": "100px"
                            },
                            { "data": function (data, type, full, meta){
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
                            { "data": function (data, type, full, meta){
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

            function SearchClientContact(searchString) {
                var tblProspectbody = $('#tblProspectbody');
                arrOrgPeopleCitiesList = [];

                //$('.bulk').hide();
                $('#chbCRMbulk').prop('checked', false)//.checkboxradio('refresh');
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
                                    $('#popCRMSearchContacts').modal('hide');
                                })
                            for (var i = 0; i < vRows.length; i++) {

                                tblProspectbody.append($('<tr>')
                                    .attr('prostype', (vRows[i].ProspectType == "Org" ? 'o' : 'p'))
                                    .css('cursor', 'pointer')
                                    .data(vRows[i])
                                    .append($('<td>')
                                        .addClass('bulk')
                                        .append($('<label class="checkbox-only" for="chbbulk-' + vRows[i].ID + '">')
                                                .append($('<input type="checkbox" name="chbbulk" id="chbbulk-' + vRows[i].ID + '" data-role="none" class="form-check-input"/>')
                                                    .data(vRows[i])
                                                    .click(function () {
                                                        $(this)//.checkboxradio('refresh');
                                                        AddSelectedRecipient($(this).data(), $(this).is(':checked'));
                                                        DisplayListOfReceipient();
                                                        //$('#popCRMSMS, #popCRMSMSRecipient').hide();
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
                                        .html((vRows[i].ProspectType == "Org" ? '<button style="float:left;" class="btn btn-warning">O</button>' : '<button style="float:left;" class="btn btn-success">P</button>'))
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
                                $('input[type="checkbox"]')//.checkboxradio()//.checkboxradio('refresh');
                                if (arrSMSRecipient.length > 0) {
                                    if (vRows[i].IsFromOrg) {
                                        $.grep(arrSMSRecipient, function (e) {
                                            if (e.CPID == vRows[i].ID) {
                                                $('#chbCRMbulk-' + vRows[i].ID).prop('checked', true)//.checkboxradio('refresh');
                                            }
                                        })
                                    }
                                    else {
                                        $.grep(arrSMSRecipient, function (e) {
                                            if (e.ID == vRows[i].ID) {
                                                $('#chbCRMbulk-' + vRows[i].ID).prop('checked', true)//.checkboxradio('refresh');
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
                $('#listCRMAction').find('li > a:not(:first)').removeClass('disabled-menu');
                isClientActive = CBool(data.Status);
                isAddNewProspect = false;
                isViewReadOnlyProfile = true;
                sessionStorage.ClientInfo = JSON.stringify(data);
                if (sessionStorage.CrmType == 'crm' || CStr2(sessionStorage.CrmType) == '') {
                    $('#btnCRMPageProfile').click();
                } else {
                    $('#selShowOrderHistory').val($('#selInvoiceMadeWith').val())
                    $('#btnCRMPageOrderHistory').click();
                }
                $('#pageCRMActions').pageshow();
                return
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
                                    $('#pageCRMActions').pageshow();
                                    $('#btnCRMPageProfile').click();
                                    //BreadCrumbs('.ItemsBreadCrumbs',
                                    //        [
                                    //            {Text: 'Lead Generation', Click: function() {
                                    //                $('#pageCRMMainDashboard, #pageDTRCRMMainDashboard').pageshow();
                                    //            }},
                                    //            {Text: e.ClientName + ' Profile'}
                                    //        ]
                                    //    );
                                    //if(e.ProspectType == "Org"){
                                    //    GetProspectDetails(e.ID);
                                    //    $('#pageCRMAddOrgProspect').pageshow()
                                    //}
                                    //else {
                                    //    GetProspectDetailsPeople(e.ID);
                                    //    $('#pageCRMAddPersonProspect').pageshow()
                                    //}                                
                                }
                            },
                            {
                                Text: 'Cold Call',
                                data: data,
                                Click: function (e) {
                                    sessionStorage.ClientInfo = JSON.stringify(e);
                                    $('#btnCRMPageColdCall').click();
                                }
                            },
                            {
                                Text: 'To-Do',
                                data: data,
                                Click: function (e) {
                                    sessionStorage.ClientInfo = JSON.stringify(e);
                                    $('#pageCRMActions').pageshow();
                                    $('#btnCRMPageToDo').click();
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
                                    $('#pageCRMActions').pageshow();
                                    $('#btnCRMPageSMS').click();
                                }
                            },
                            {
                                Text: 'Interaction History',
                                data: data,
                                Click: function (e) {
                                    sessionStorage.ClientInfo = JSON.stringify(e);
                                    $('#pageCRMActions').pageshow();
                                    $('#btnCRMPageInteractionHistory').click();
                                }
                            },
                            {
                                Text: 'Take a Note',
                                data: data,
                                Click: function (e) {
                                    sessionStorage.ClientInfo = JSON.stringify(e);
                                    $('#pageCRMActions').pageshow();
                                    $('#btnCRMPageTakeANote').click();
                                    $('#txtCRMTakeANote').val();
                                }
                            },
                            {
                                Text: 'Schedule an Action',
                                data: data,
                                Click: function (e) {
                                    sessionStorage.ClientInfo = JSON.stringify(e);
                                    $('#pageCRMActions').pageshow();
                                    $('#btnCRMPageScheduleAnAction').click();
                                }
                            }
                            //{
                            //    Text: 'Internal Sales And Marketing Info',
                            //    data: data,
                            //    Click: function (e) {
                            //        sessionStorage.ClientInfo = JSON.stringify(e);
                            //        $('#pageCRMActions').pageshow();
                            //        $('#btnCRMPageInternalSalesMarketing').click();
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
                $('#txtCRMCPName').html('');

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
                                $('#txtCRMCPName').html($(this).data().FullName + ' #: <br>');
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

                $('#btnCRMCCName').unbind().click(function () {
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

                AjaxSend({
                    url: 'CRMDashboard/GetProspectDetails',
                    data: { ID: id },
                    success: function (vData) {
                        var vRowsOrgDetails = vData.OrgDetails;
                        arrAddress = vData.Address;
                        arrContactPersonList = vData.ContactPerson;
                        arrEmailInfoList = vData.Email;
                        arrMobileInfoList = vData.Mobile;
                        arrTelInfoList = vData.Telephone;
                        arrWebsiteList = vData.Website;
                        arrOrgAffList = vData.Affiliation;
                        arrOrgAffPersonList = vData.AffiliatedPerson;

                        intContactPersonCtr = vData.ContactPerson.length;
                        intTelInfoCtr = vData.Telephone.length;
                        intMobileInfoCtr = vData.Mobile.length;
                        intEmailInfoCtr = vData.Email.length;
                        intWebsiteCtr = vData.Website.length;

                        //if (isViewReadOnlyProfile) {
                        //    ReadOnlyOrgProfile(vRowsOrgDetails[0]);
                        //} else {
                            $('#txtCRMProspectOrgID').val(vRowsOrgDetails[0].ID);
                            $('#txtCRMProspectCompanyName').val(vRowsOrgDetails[0].CompanyName);
                            $('#txtCRMProspectHouseBldgNo').val(vRowsOrgDetails[0].OfficeAddressBuildingNo);
                            $('#txtCRMProspectStreetName').val(vRowsOrgDetails[0].OfficeAddressStreet);
                            $('#selCRMProspectCities').val(vRowsOrgDetails[0].OfficeAddressCityID)//.selectmenu('refresh');
                            $('#selCRMProspectOrgRegion').val(vRowsOrgDetails[0].Region)//.selectmenu('refresh');
                            $('#selCRMProspectIsland').val(vRowsOrgDetails[0].Island)//.selectmenu('refresh');
                            $('#selCRMProspectCountries').val(vRowsOrgDetails[0].OfficeAddressCountryID)//.selectmenu('refresh');
                            $('#txtCRMProspectCompanyBackground').val(vRowsOrgDetails[0].Background);
                            $('#txtCRMProspectCompanyMission').val(vRowsOrgDetails[0].Mission);
                            $('#txtCRMProspectCompanyVision').val(vRowsOrgDetails[0].Vision);
                            $('#txtCRMProspectCompanyFounding').val(vRowsOrgDetails[0].FoundingDate);
                            $('#txtCRMBusIndustryID').val((vRowsOrgDetails[0].IndustryID == undefined ? '' : vRowsOrgDetails[0].IndustryID));
                            $('#btnCRMProspectCompanyIndustry').text((vRowsOrgDetails[0].IndustryName == undefined ? 'Search here' : vRowsOrgDetails[0].IndustryName))//.button('refresh');
                            $('#selCRMOrgStatus').val((CBool(vRowsOrgDetails[0].IsActive) == true ? 'Active' : 'Inactive'))//.selectmenu('refresh');

                            $('#txtCRMProspectNoOfEmployees').val((CDec2(vRowsOrgDetails[0].NoOfEmployees) == 0 ? '' : vRowsOrgDetails[0].NoOfEmployees));
                            $('#txtCRMProspectRevenuePerYear').val((CDec2(vRowsOrgDetails[0].RevenuePerYear) == 0 ? '' : vRowsOrgDetails[0].RevenuePerYear));
                            $('#selCRMProspectScopeOperation').val((vRowsOrgDetails[0].ScopeOfOperations == '' ? '' : vRowsOrgDetails[0].ScopeOfOperations))//.selectmenu('refresh');
                            $('#txtCRMProspectNoFactories').val((CDec2(vRowsOrgDetails[0].NoOfFactory) == 0 ? '' : vRowsOrgDetails[0].NoOfFactory));
                            $('#txtCRMProspectNoOffices').val((CDec2(vRowsOrgDetails[0].NoOfOffice) == 0 ? '' : vRowsOrgDetails[0].NoOfOffice));
                            $('#txtCRMProspectNoWarehouses').val((CDec2(vRowsOrgDetails[0].NoOfWarehouse) == 0 ? '' : vRowsOrgDetails[0].NoOfWarehouse));
                            $('#txtCRMProspectNoStores').val((CDec2(vRowsOrgDetails[0].NoOfStore) == 0 ? '' : vRowsOrgDetails[0].NoOfStore));
                            $('#txtCRMProspectNoTrucks').val((CDec2(vRowsOrgDetails[0].NoOfTruck) == 0 ? '' : vRowsOrgDetails[0].NoOfTruck));

                            AddTelInfo();
                            AddMobileInfo();
                            AddEmailInfo()
                            AddWebsiteInfo();
                            AddContactPersonInfo();
                            AddOrgAffiliation();
                            AddAffiliatedPerson();
                            DisplayAddress();
                        //}
                    }
                })
            }

            function ReadOnlyOrgProfile(data) {
                var listpreviewOrgContactPerson = $('#listCRMpreviewOrgContactPerson').empty();
                var listpreviewOrgAff = $('#listCRMpreviewOrgAff').empty();
                var listpreviewAffPerson = $('#listCRMpreviewAffPerson').empty();
                var listpreviewOrgTel = $('#listCRMpreviewOrgTel').empty();
                var listpreviewOrgMobile = $('#listCRMpreviewOrgMobile').empty();
                var listpreviewOrgEmail = $('#listCRMpreviewOrgEmail').empty();
                var listpreviewOrgWebsite = $('#listCRMpreviewOrgWebsite').empty();

                $('#txtCRMpreviewCompanyName').text(data.CompanyName);
                $('#txtCRMpreviewCompanyAddress').text($.trim(data.Address));
                $('#txtCRMpreviewCompanyBackground').text(CStr2(data.Background));
                $('#txtCRMpreviewCompanyMission').text(CStr2(data.Mission));
                $('#txtCRMpreviewCompanyVision').text(CStr2(data.Vision));
                $('#txtCRMpreviewCompanyFounding').text(CStr2(data.FoundingDate));
                $('#txtCRMpreviewCompanyIndustry').text(CStr2(data.IndustryName));
                $('#txtCRMprevNoOfEmp').text(CStr2(data.NoOfEmployees));
                $('#txtCRMprevRevenue').text((CStr2(data.RevenuePerYear).toUpperCase() == "NULL" ? "" : CStr2(data.RevenuePerYear)));
                $('#txtCRMprevScope').text(CStr2(data.ScopeOfOperations));
                $('#txtCRMprevNoOfFactories').text(CStr2(data.NoOfFactory));
                $('#txtCRMprevNoOfOffices').text(CStr2(data.NoOfOffice));
                $('#txtCRMprevNoOfWarehouse').text(CStr2(data.NoOfWarehouse));
                $('#txtCRMprevNoOfStores').text(CStr2(data.NoOfStore));
                $('#txtCRMprevNoOfTrucks').text(CStr2(data.NoOfTruck));
                //$('#txtCRMprevSalesRep').text(data.);
                //$('#txtCRMprevPriceLvl').text(data.);
                //$('#txtCRMprevCreditTerms').text(data.);
                //$('#txtCRMprevCreditLimit').text(data.);
                //$('#txtCRMprevCreditBeg').text(data.);
                //$('#txtCRMprevCreditBegAsOf').text(data.);

                for (var i = 0; i < arrOrgAffList.length; i++) {
                    listpreviewOrgAff.append($('<li>').text(arrOrgAffList[i].CompanyName))
                }

                for (var i = 0; i < arrOrgAffPersonList.length; i++) {
                    listpreviewAffPerson.append($('<li>').text(arrOrgAffPersonList[i].PeopleName))
                }

                for (var i = 0; i < arrTelInfoList.length; i++) {
                    listpreviewOrgTel.append($('<li>').text('+' + $.trim((arrTelInfoList[i].CountryCode).split('-')[1]) + ' (' + (CStr2(arrTelInfoList[i].AreaCode) == '' ? '' : $.trim((arrTelInfoList[i].AreaCode).split('-')[1])) + ') ' + arrTelInfoList[i].TelephoneNo + ' (' + arrTelInfoList[i].ContactType + ')'))
                }

                for (var i = 0; i < arrMobileInfoList.length; i++) {
                    listpreviewOrgMobile.append($('<li>').text('+' + $.trim((arrMobileInfoList[i].CountryCode).split('-')[1]) + '' + arrMobileInfoList[i].MobileNo + ' (' + arrMobileInfoList[i].ContactType + ')'))
                }

                for (var i = 0; i < arrEmailInfoList.length; i++) {
                    listpreviewOrgEmail.append($('<li>').text(arrEmailInfoList[i].EmailAdd))
                }

                for (var i = 0; i < arrWebsiteList.length; i++) {
                    listpreviewOrgWebsite.append($('<li>').text(arrWebsiteList[i].Website))
                }

                $('#prevCRMOrgProfile').show();
                $('#roCRMOrg').pageshow();
            }

            function GetProspectDetailsPeople(id) {
                AjaxSend({
                    url: 'CRMDashboard/GetProspectDetailsPeople?mod=' + (sessionStorage.SystemAdminFor || 'crm'),
                    data: { ID: id },
                    success: function (vData) {
                        var vRowsPeopleDetails = vData.PeopleDetails;
                        arrPersonEmailInfoList = vData.Email;
                        arrPersonMobileInfoList = vData.Mobile;
                        arrPersonTelInfoList = vData.Telephone;
                        arrPersonOrgAffList = vData.Affiliation;
                        arrElementary = vData.Education.Elementary;
                        arrHighSchool = vData.Education.HighSchool;
                        arrCollege = vData.Education.College;

                        //if (isViewReadOnlyProfile) {
                        //    vRowsPeopleDetails[0].Address = vData.Address[0];
                        //    ReadOnlyPersonProfile(vRowsPeopleDetails[0]);
                        //} else {
                            $('#txtCRMProspectPersonID').val(vRowsPeopleDetails[0].ID);
                            $('#txtCRMProspectPersonFirstName').val(vRowsPeopleDetails[0].FirstName);
                            $('#txtCRMProspectPersonMiddleName').val(vRowsPeopleDetails[0].MiddleName);
                            $('#txtCRMProspectPersonLastName').val(vRowsPeopleDetails[0].Surname);
                            $('#txtCRMProspectPersonPosition').val(vRowsPeopleDetails[0].Position);
                            $('#txtCRMProspectPersonPositionDescription').val(vRowsPeopleDetails[0].PositionDescription);
                            $('#selCRMPersonStatus').val((CBool(vRowsPeopleDetails[0].IsActive) == true ? 'Active' : 'Inactive'))//.selectmenu('refresh');

                            if (vData.Address.length > 0) {
                                $('#txtCRMPersonAddressID').val(vData.Address[0].ID);
                                $('#txtCRMPersonHouseBuildingNo').val(vData.Address[0].HouseBuildingNo);
                                $('#txtCRMPersonStreet').val(vData.Address[0].Street);
                                $('#selCRMPersonCity').val(vData.Address[0].CityID)//.selectmenu('refresh');
                                $('#selCRMPersonRegion').val(vData.Address[0].Region)//.selectmenu('refresh');
                                $('#selCRMPersonIsland').val(vData.Address[0].Island)//.selectmenu('refresh');
                                $('#selCRMPersonCountry').val(vData.Address[0].CountryID)//.selectmenu('refresh');
                                $('#selCRMPersonTypeAddress').val(vData.Address[0].AddressType);
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
                        //}
                    }
                })
            }

            function ReadOnlyPersonProfile(data) {
                var listpreviewPersonOrgAff = $('#listCRMpreviewPersonOrgAff').empty();
                var listpreviewPersonTel = $('#listCRMpreviewPersonTel').empty();
                var listpreviewPersonMobile = $('#listCRMpreviewPersonMobile').empty();
                var listpreviewPersonEmail = $('#listCRMpreviewPersonEmail').empty();
                var listpreviewPersonElem = $('#listCRMpreviewPersonElem').empty();
                var listpreviewPersonHS = $('#listCRMpreviewPersonHS').empty();
                var listpreviewPersonCollege = $('#listCRMpreviewPersonCollege').empty();

                $('#txtCRMpreviewPersonName').text($.trim(data.FirstName + ' ' + data.MiddleName + ' ' + data.Surname));
                $('#txtCRMpreviewBirthDate').text((CStr2(data.DateOfBirth) == '' ? '' : $.format.date(new Date(parseInt($.trim(data.DateOfBirth).replace('/Date(', ''))), 'MMM dd, yyyy')));
                $('#txtCRMpreviewPosition').text(CStr2(data.Position));
                $('#txtCRMpreviewPositionDesc').text(CStr2(data.PositionDescription));

                if (data.Address != undefined) {
                    $('#txtCRMprevAddress').text($.trim(data.Address.HouseBuildingNo + ' ' + data.Address.Street + ' ' + data.Address.CityName + ' ' + data.Address.CountryName));
                }

                for (var i = 0; i < arrPersonTelInfoList.length; i++) {
                    listpreviewPersonTel.append($('<li>').text('+' + $.trim((arrPersonTelInfoList[i].CountryCode).split('-')[1]) + ' (' + $.trim((arrPersonTelInfoList[i].AreaCode).split('-')[1]) + ') ' + arrPersonTelInfoList[i].TelephoneNo))
                }

                for (var i = 0; i < arrPersonMobileInfoList.length; i++) {
                    listpreviewPersonMobile.append($('<li>').text('+' + $.trim((arrPersonMobileInfoList[i].CountryCode).split('-')[1]) + '' + arrPersonMobileInfoList[i].MobileNo))
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

                $('#prevCRMOrgProfile').show();
                $('#roCRMPerson').pageshow();
            }

            function ClearPersonProspectVariablesFields() {
                arrAddress = [];
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
                academicLvl = '', arrElementary = [], arrHighSchool = [], arrCollege = [], isNewEducation = true, ctrElem = 0, ctrHs = 0, ctrCollege = 0, ctrIndexEducation = 0;

                $('#txtCRMProspectPersonID').val('');
                $('#selCRMPersonStatus').val('Active')//.selectmenu('refresh');
                $('#txtCRMProspectPersonFirstName').val('');
                $('#txtCRMProspectPersonMiddleName').val('');
                $('#txtCRMProspectPersonLastName').val('');
                $('#btnCRMProspectPersonBirthday').text('Set Date')//.button('refresh');
                $('#txtCRMProspectPersonPosition').val('');
                $('#txtCRMProspectPersonPositionDescription').val('');
                $('#txtCRMPersonAddressID').val('');
                $('#txtCRMPersonHouseBuildingNo').val('');
                $('#txtCRMPersonStreet').val('');
                $('#selCRMPersonCity').val('')//.selectmenu('refresh');
                $('#selCRMPersonRegion').val('')//.selectmenu('refresh');
                $('#selCRMPersonIsland').val('')//.selectmenu('refresh');
                $('#selCRMPersonCountry').val('e783ed13-aa98-4cc1-918e-d49b05c0167e')//.selectmenu('refresh');
                $('#selCRMPersonTypeAddress').val('');

                $('#lstPersonOrgAffiliation').empty();

                $('#lstPersonTelephone').empty();
                $('#txtCRMPersonTelID').val('');
                $('#txtCRMPersonTelAreaCodeID').val('');

                $('#lstPersonMobile').empty();
                $('#txtCRMPersonMobileID').val('');
                $('#txtCRMPersonTelCoPrefixID').val('');

                $('#lstPersonEmail').empty();
                $('#txtCRMPersonEmailID').val('');

                $('#lstPersonWebsite').empty();
                $('#txtCRMPersonWebsiteID').val('');

                $('#lstPersonContactPerson').empty();

                $('#lstElementary').empty();
                $('#lstHs').empty();
                $('#lstCollege').empty();

                ClearContactPersonFields();
                ClearPersonEmailInfoFields();
                ClearPersonMobileInfo();
                ClearPersonTelInfo();
            }

            function ClearOrgProspectVariablesFields() {
                arrAddress = [];
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
                addressIndex = null;

                $('#txtCRMProspectOrgID').val('');
                $('#selCRMOrgStatus').val('Active')//.selectmenu('refresh');
                $('#txtCRMProspectCompanyName').val('');
                $('#txtCRMProspectHouseBldgNo').val('');
                $('#txtCRMProspectStreetName').val('');
                $('#selCRMProspectOrgRegion').val('')//.selectmenu('refresh');
                $('#selCRMProspectCities').val('')//.selectmenu('refresh');
                $('#selCRMProspectIsland').val('')//.selectmenu('refresh');
                $('#selCRMProspectCountries').val('e783ed13-aa98-4cc1-918e-d49b05c0167e')//.selectmenu('refresh');
                $('#txtCRMProspectCompanyBackground').val('');
                $('#txtCRMProspectCompanyMission').val('');
                $('#txtCRMProspectCompanyVision').val('');
                $('#txtCRMProspectCompanyFounding').val('');
                $('#txtCRMBusIndustryID').val('');
                $('#btnCRMProspectCompanyIndustry').text('Search here')//.button('refresh');
                $('#lstOrgAffiliation').empty();
                $('#lstAffiliatedPerson').empty()

                $('#lstCRMOrgTelephone').empty();
                $('#txtCRMOrgTelID').val('');
                $('#txtCRMOrgTelAreaCodeID').val('');

                $('#lstCRMOrgMobile').empty();
                $('#txtCRMOrgMobileID').val('');
                $('#txtCRMOrgTelCoPrefixID').val('');

                $('#lstCRMOrgEmail').empty();
                $('#txtCRMOrgEmailID').val('');

                $('#lstCRMOrgWebsite').empty();
                $('#txtCRMOrgWebsiteID').val('');

                $('#lstCRMOrgContactPerson').empty();

                $('#txtCRMProspectNoOfEmployees').val('');
                $('#txtCRMProspectRevenuePerYear').val('');
                $('#selCRMProspectScopeOperation').val('');
                $('#txtCRMProspectNoFactories').val('');
                $('#txtCRMProspectNoOffices').val('');
                $('#txtCRMProspectNoWarehouses').val('');
                $('#txtCRMProspectNoStores').val('');
                $('#txtCRMProspectNoTrucks').val('');

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
                    $('#btnCRMStartColdCall').removeClass('btn-disabled');
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