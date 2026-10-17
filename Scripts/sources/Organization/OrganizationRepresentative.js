require([
    'common'
    ], function () {

        $(function () {

            var IsNewOrgRepTelContact = true;
            var arOrgRepTel = new Array();
            var intOrgRepTelCntr = 0;
            var intOrgRepTelIndex = 0;

            var intOrgRepTelRecID = 0;
            var intOrgRepTelCountryID = 1;
            var intOrgRepTelCountryCode = 2;
            var intOrgRepTelCallingCode = 3;
            var intOrgRepTelAreaCodeID = 4;
            var intOrgRepTelArea = 5;
            var intOrgRepTelAreaCode = 6;
            var intOrgRepTelContactNo = 7;
            var intOrgRepTelExtNo = 8;
            var intOrgRepTelContactType = 9;
            var intOrgRepTelState = 10;

            var IsNewOrgRepMobileContact = true;
            var arOrgRepMobile = new Array();
            var intOrgRepMobileCntr = 0;
            var intOrgRepMobileIndex = 0;

            var intOrgRepMobileRecID = 0;
            var intOrgRepMobileCountryID = 1;
            var intOrgRepMobileCountryCode = 2;
            var intOrgRepMobileCallingCode = 3;
            var intOrgRepMobileTelCoPrefixID = 4;
            var intOrgRepMobilePrefix = 5;
            var intOrgRepMobileTelCo = 6;
            var intOrgRepMobileContactNo = 7;
            var intOrgRepMobileContactType = 8;
            var intOrgRepMobileState = 9;


            var IsNewOrgRepEmailContact = true;
            var arOrgRepEmail = new Array();
            var intOrgRepEmailCntr = 0;
            var intOrgRepEmailIndex = 0;

            var intOrgRepEmailRecID = 0;
            var intOrgRepEmailAddress = 1;
            var intOrgRepEmailType = 2;
            var intOrgRepEmailState = 3;

            var mLoaded = false;
            var isIE = false || !!document.documentMode;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                getOrgRepresentativeList();
            };
            if (!mLoaded) PageShow();

            window.OrgRep = {
                ShowInfo: function () {
                    getOrgRepresentativeList();
                }
            }

            $('body')
            .on('click', '#btnOrgRepAdd', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
                ClearOrgRepFields();
            })
            .on('click', '#btnOrgRepTelCountryCode', function () {
                BrowsePopup('LibraryCountries', 'Country', [
                    {
                        DisplayText: [
                            { Column: 'Name' }
                        ],
                        Click: function (e) {
                            $('#txtOrgRepTelCountryID').val(e.ID)
                            $('#btnOrgRepTelCountryCode').text(e.Code + " - " + e.CallingCode);//.button('refresh');
                        }
                    }
                ])
            })
            .on('click', '#btnOrgRepTelAreaCode', function () {
                BrowsePopup('LibraryAreaCodes', 'Area', [
                    {
                        DisplayText: [
                            { Column: 'Area', Label: 'Area' },
                            { Column: 'CallingCode', Label: 'Code' }
                        ],
                        Click: function (e) {
                            $('#txtOrgRepTelAreaCodeID').val(e.ID)
                            $('#btnOrgRepTelAreaCode').text(e.Area + ' - ' + e.CallingCode);//.button('refresh');
                        }
                    }
                ])
            })
            .on('keydown', '#txtOrgRepContactNoTel', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtOrgRepContactExtensionTel', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('click', '#btnOrgRepMobileCountryCode', function () {
                BrowsePopup('LibraryCountries', 'Country', [
                    {
                        DisplayText: [
                            { Column: 'Name' }
                        ],
                        Click: function (e) {
                            $('#txtOrgRepMobileCountryID').val(e.ID)
                            $('#btnOrgRepMobileCountryCode').text(e.Code + " - " + e.CallingCode);//.button('refresh');
                        }
                    }
                ])
            })
            .on('click', '#btnOrgRepMobileTelCoPrefixCode', function () {
                BrowsePopup('LibraryTelcoPrefixes', 'Telco Code', [
                    {
                        DisplayText: [
                            { Column: 'Prefix' },
                            { Column: 'TelCo' }
                        ],
                        Click: function (e) {
                            $('#txtOrgRepTelCoPrefixID').val(e.ID)
                            $('#btnOrgRepMobileTelCoPrefixCode').text(e.TelCo + " - " + e.Prefix);//.button('refresh');
                        }
                    }
                ])
            })
            .on('keydown', '#txtOrgRepContactNoMobile', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('click', '#btnOrgRepContactTelAdd', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepContactTelInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').show();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();

                IsNewOrgRepTelContact = true
                intOrgRepTelCntr = intOrgRepTelCntr + 1;
                intOrgRepTelIndex = intOrgRepTelCntr;

                ClearOrgRepContactTelFields();
                $(window).scrollTop(0);
            })
            .on('click', '#btnOrgRepContactMobAdd', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepContactMobileInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').show();
                $('#pageOrgRepContactEmail').hide();

                IsNewOrgRepMobileContact = true
                intOrgRepMobileCntr = intOrgRepMobileCntr + 1;
                intOrgRepMobileIndex = intOrgRepMobileCntr;

                ClearOrgRepContactMobileFields();
                $(window).scrollTop(0);
            })
            .on('click', '#btnOrgRepContactEmailAdd', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepContactEmail');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').show();

                IsNewOrgRepEmailContact = true
                intOrgRepEmailCntr = intOrgRepEmailCntr + 1;
                intOrgRepEmailIndex = intOrgRepEmailCntr;

                ClearOrgRepContactEmailFields();
                $(window).scrollTop(0);
            })
            .on('click', '#btnOrgRepContactTelSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgRepTelCountryID").val()) == "") {
                    NewMessageBox("Please select country code.",
                           "Country Code",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#txtOrgRepTelAreaCodeID").val()) == "") {
                    NewMessageBox("Please select area code.",
                           "Area Code",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#txtOrgRepContactNoTel").val()) == "") {
                    NewMessageBox("Please enter 7 digit telephone number.",
                           "Tel. No.",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#txtOrgRepContactNoTel").val()).length != 7) {
                    NewMessageBox("Please enter 7 digit telephone number.",
                           "Tel. No.",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if (!$.isNumeric($("#txtOrgRepContactNoTel").val())) {
                    NewMessageBox("Please enter valid telephone number.",
                                   "Tel. No.",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                    return;
                }

                if ($.trim($("#txtOrgRepContactExtensionTel").val()).length > 0) {
                    if (!$.isNumeric($("#txtOrgRepContactExtensionTel").val())) {
                        NewMessageBox("Please enter valid extension or local number.",
                                   "Tel. No.",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                        return;
                    }
                }

                if ($.trim($("#selOrgRepContactTypeTel").val()) == "") {
                    NewMessageBox("Please select telephone contact type.",
                           "Telephone Contact Type",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                SaveOrgRepContactTel();
            })
            .on('click', '#btnOrgRepContactTelRemove', function (e) {
                e.preventDefault();

                NewMessageBox("Remove this record?",
                    'Remove',
                    [
                        { Text: 'Yes',
                            Click: function () {
                                RemoveOrgRepContactTel();
                            }
                        }
                    ,
                        { Text: 'No'
                        }
                    ]
                );
            })
            .on('click', '#btnOrgRepContactTelCancel', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
                $(window).scrollTop(0);
            })
            .on('click', '#btnOrgRepContactMobileSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgRepMobileCountryID").val()) == "") {
                    NewMessageBox("Please select country code.",
                           "Country Code",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#txtOrgRepTelCoPrefixID").val()) == "") {
                    NewMessageBox("Please select prefix number.",
                           "Prefix Number",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#txtOrgRepContactNoMobile").val()) == "") {
                    NewMessageBox("Please enter 7 digit mobile number.",
                           "Mobile No.",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#txtOrgRepContactNoMobile").val()).length != 7) {
                    NewMessageBox("Please enter 7 digit mobile number.",
                           "Mobile No.",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if (!$.isNumeric($("#txtOrgRepContactNoMobile").val())) {
                    NewMessageBox("Please enter valid mobile number.",
                                   "Mobile No.",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                    return;
                }

                if ($.trim($("#selOrgRepContactTypeMobile").val()) == "") {
                    NewMessageBox("Please select mobile contact type.",
                           "Mobile Contact Type",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                SaveOrgRepContactMobile();
            })
            .on('click', '#btnOrgRepContactMobileRemove', function (e) {
                e.preventDefault();

                NewMessageBox("Remove this record?",
                    'Remove',
                    [
                        { Text: 'Yes',
                            Click: function () {
                                RemoveOrgRepContactMobile();
                            }
                        }
                    ,
                        { Text: 'No'
                        }
                    ]
                );
            })
            .on('click', '#btnOrgRepContactMobileCancel', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
                $(window).scrollTop(0);
            })
            .on('click', '#btnOrgRepContactEmailSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgRepContactEmail").val()) == "") {
                    NewMessageBox("Please enter email address.",
                           "Email Address",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if (IsEmail($.trim($("#txtOrgRepContactEmail").val())) == false) {
                    NewMessageBox("Please enter valid email address.",
                            "Email Address",
                            [
                                { Text: 'Ok' }
                            ]
                );

                    return;
                }

                if ($.trim($("#selOrgRepContactTypeEmail").val()) == "") {
                    NewMessageBox("Please select email type.",
                           "Email Type",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                SaveOrgRepContactEmail();
            })
            .on('click', '#btnOrgRepContactEmailRemove', function (e) {
                e.preventDefault();

                NewMessageBox("Remove this record?",
                    'Remove',
                    [
                        { Text: 'Yes',
                            Click: function () {
                                RemoveOrgRepContactEmail();
                            }
                        }
                    ,
                        { Text: 'No'
                        }
                    ]
                );
            })
            .on('click', '#btnOrgRepContactEmailCancel', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
                $(window).scrollTop(0);
            })
            .on('click', '#btnOrgRepSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgRepLastName").val()) == "") {
                    NewMessageBox("Please enter last name.",
                           "Last Name",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtOrgRepFirstName").val()) == "") {
                    NewMessageBox("Please enter first name.",
                           "First Name",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                SaveOrgRep();
            })
            .on('click', '#btnOrgRepCancel', function (e) {
                e.preventDefault();

                // ChangePage('/Organization/OrganizationRepresentative');

                $('#pageRepList').show();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
                $(window).scrollTop(0);
                getOrgRepresentativeList();
            })
            .on('click', '#btnOrgRepDone', function (e) {
                e.preventDefault();

                //////                if (window.pageSourcePage != undefined) {
                //////                    $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
                //////                }
                //////                else {
                //////                    ChangePage('/Organization/OrganizationSummary');
                //////                    window.location = MergeUrlParameters({
                //////                        OrgID: sessionStorage.OrgID,
                //////                        IsMobile: CBool(IsMobile) ? 1 : 0
                //////                    }, '/Organization/OrganizationSummary');

                //////                }

                $('body').scrollTop(0);
                $('#UniversalDiv').html('');
                LibraryTable.OrganizationPage();
            })

            function getOrgRepresentativeList() {
                AjaxSend({
                    url: 'Organization/getOrgRepList',
                    success: function (vData) {
                        LoadOrgRepList(vData.OrganizationRepData.Rows);
                    }
                });
            }

            function LoadOrgRepList(vRows) {
                var pLV = $('#lstOrgRepresentative');
                var pItem;

                $("#divOrgRepRecordCount").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageOrgRepInfo');
                                $('#pageRepList').hide();
                                $('#pageOrgRepInfo').show();
                                $('#pageOrgRepContactTelInfo').hide();
                                $('#pageOrgRepContactMobileInfo').hide();
                                $('#pageOrgRepContactEmail').hide();
                                ShowOrgRepInfo($(this).attr('ID'));
                                $(window).scrollTop(0);
                            })
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].RepresentativeName)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            };

            function ShowOrgRepInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgRepFields();

                AjaxSend({
                    url: "Organization/getOrgRepInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationRepData.Rows.length > 0) {

                            $("#txtOrgRepID").val(vData.OrganizationRepData.Rows[0].ID || '');

                            $("#txtOrgRepTitle").val(vData.OrganizationRepData.Rows[0].Title || '');
                            $("#txtOrgRepLastName").val(vData.OrganizationRepData.Rows[0].LastName || '');
                            $("#txtOrgRepFirstName").val(vData.OrganizationRepData.Rows[0].FirstName || '');
                            $("#txtOrgRepMiddleName").val(vData.OrganizationRepData.Rows[0].MiddleName || '');
                            $("#selOrgRepGenSuffixName").val(vData.OrganizationRepData.Rows[0].GenerationalSuffix || '');//.selectmenu('refresh');

                            $("#selOrgRepIsActive").val(vData.OrganizationRepData.Rows[0].IsActive || '1');//.slider("refresh");

                            pData.OrganizationID = vData.OrganizationRepData.Rows[0].ID;

                            //Get Tel
                            AjaxSend({
                                url: 'Organization/getOrgRepContactTelList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgRepContactTel(vData.OrganizationRepData.Rows);
                                }
                            });

                            //Get mobile
                            AjaxSend({
                                url: 'Organization/getOrgRepContactMobileList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgRepContactMobile(vData.OrganizationRepData.Rows);
                                }
                            });

                            //Get Email
                            AjaxSend({
                                url: 'Organization/getOrgRepContactEmailList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgRepContactEmail(vData.OrganizationRepData.Rows);
                                }
                            });
                        }

                    }
                });

            };

            function LoadOrgRepContactTel(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgRepTel = new Array();
                    arOrgRepTel[intOrgRepTelRecID] = vRows[p].ID;
                    arOrgRepTel[intOrgRepTelCountryID] = vRows[p].CountryID;
                    arOrgRepTel[intOrgRepTelCountryCode] = vRows[p].Country;
                    arOrgRepTel[intOrgRepTelCallingCode] = vRows[p].CountryCode;
                    arOrgRepTel[intOrgRepTelAreaCodeID] = vRows[p].AreaCodeID;
                    arOrgRepTel[intOrgRepTelArea] = vRows[p].Area;
                    arOrgRepTel[intOrgRepTelAreaCode] = vRows[p].AreaCode;
                    arOrgRepTel[intOrgRepTelContactNo] = vRows[p].ContactNo;
                    arOrgRepTel[intOrgRepTelExtNo] = vRows[p].ExtensionNo;
                    arOrgRepTel[intOrgRepTelContactType] = vRows[p].ContactType;
                    arOrgRepTel[intOrgRepTelState] = "";

                    sessionStorage["OrgRepTel-" + p] = JSON.stringify(arOrgRepTel);

                };

                intOrgRepTelCntr = vRows.length;
                LoadOrgRepContactTelList();

            };

            function LoadOrgRepContactMobile(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgRepMobile = new Array();
                    arOrgRepMobile[intOrgRepMobileRecID] = vRows[p].ID;
                    arOrgRepMobile[intOrgRepMobileCountryID] = vRows[p].CountryID;
                    arOrgRepMobile[intOrgRepMobileCountryCode] = vRows[p].Country;
                    arOrgRepMobile[intOrgRepMobileCallingCode] = vRows[p].CountryCode;
                    arOrgRepMobile[intOrgRepMobileTelCoPrefixID] = vRows[p].TelCoPrefixID;
                    arOrgRepMobile[intOrgRepMobilePrefix] = vRows[p].Prefix;
                    arOrgRepMobile[intOrgRepMobileTelCo] = vRows[p].TelCo;
                    arOrgRepMobile[intOrgRepMobileContactNo] = vRows[p].ContactNo;
                    arOrgRepMobile[intOrgRepMobileContactType] = vRows[p].ContactType;
                    arOrgRepMobile[intOrgRepMobileState] = "";

                    sessionStorage["OrgRepMobile-" + p] = JSON.stringify(arOrgRepMobile);

                };

                intOrgRepMobileCntr = vRows.length;
                LoadOrgRepContactMobileList();

            };

            function LoadOrgRepContactEmail(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgRepEmail = new Array();
                    arOrgRepEmail[intOrgRepEmailRecID] = vRows[p].ID;
                    arOrgRepEmail[intOrgRepEmailAddress] = vRows[p].EmailAddress;
                    arOrgRepEmail[intOrgRepEmailType] = vRows[p].EmailType;
                    arOrgRepEmail[intOrgRepEmailState] = "";

                    sessionStorage["OrgRepEmail-" + p] = JSON.stringify(arOrgRepEmail);

                };

                intOrgRepEmailCntr = vRows.length;
                LoadOrgRepContactEmailList();

            };

            function ShowOrgRepContactTelInfo(arData, index) {

                //$.mobile.changePage('#pageOrgRepContactTelInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').show();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();

                ClearOrgRepContactTelFields();

                intOrgRepTelIndex = index;

                $("#txtOrgRepContactTelID").val(arData[intOrgRepTelRecID]);

                $("#txtOrgRepTelCountryID").val(arData[intOrgRepTelCountryID]);
                $("#btnOrgRepTelCountryCode").text(arData[intOrgRepTelCountryCode] + ' - ' + arData[intOrgRepTelCallingCode]);//.button('refresh');

                $("#txtOrgRepTelAreaCodeID").val(arData[intOrgRepTelAreaCodeID]);
                $("#btnOrgRepTelAreaCode").text(arData[intOrgRepTelArea] + ' - ' + arData[intOrgRepTelAreaCode]);//.button('refresh');

                $("#txtOrgRepContactNoTel").val(arData[intOrgRepTelContactNo]);
                $("#txtOrgRepContactExtensionTel").val(arData[intOrgRepTelExtNo]);

                $("#selOrgRepContactTypeTel").val(arData[intOrgRepTelContactType]);//.selectmenu('refresh');

                IsNewOrgRepTelContact = false;

                $("#btnOrgRepContactTelRemove").show();

            };

            function ShowOrgRepContactMobileInfo(arData, index) {

                //$.mobile.changePage('#pageOrgRepContactMobileInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').show();
                $('#pageOrgRepContactEmail').hide();

                ClearOrgRepContactMobileFields();

                intOrgRepMobileIndex = index;

                $("#txtOrgRepContactMobileID").val(arData[intOrgRepMobileRecID] || '');

                $("#txtOrgRepMobileCountryID").val(arData[intOrgRepMobileCountryID] || '');
                $("#btnOrgRepMobileCountryCode").text((arData[intOrgRepMobileCountryCode] + ' - ' + arData[intOrgRepMobileCallingCode]) || 'Country Code');//.button('refresh');

                $("#txtOrgRepTelCoPrefixID").val(arData[intOrgRepMobileTelCoPrefixID] || '');
                $("#btnOrgRepMobileTelCoPrefixCode").text((arData[intOrgRepMobileTelCo] + ' - ' + arData[intOrgRepMobilePrefix]) || 'Telco Code');//.button('refresh');

                $("#txtOrgRepContactNoMobile").val(arData[intOrgRepMobileContactNo] || '');
                $("#selOrgRepContactTypeMobile").val(arData[intOrgRepMobileContactType] || '');//.selectmenu('refresh');

                IsNewOrgRepMobileContact = false;

                $("#btnOrgRepContactMobileRemove").show();

            };

            function ShowOrgRepContactEmailInfo(arData, index) {

                //$.mobile.changePage('#pageOrgRepContactEmail');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').show();

                ClearOrgRepContactEmailFields();

                intOrgRepEmailIndex = index;

                $("#txtOrgRepContactEmailID").val(arData[intOrgRepEmailRecID] || '');

                $("#txtOrgRepContactEmail").val(arData[intOrgRepEmailAddress] || '');
                $("#selOrgRepContactTypeEmail").val(arData[intOrgRepEmailType] || '');//.selectmenu('refresh');

                IsNewOrgRepEmailContact = false;

                $("#btnOrgRepContactEmailRemove").show();


            };

            function ClearOrgRepFields() {

                // dont clear
                // sessionStorage.clear();
                // proj.ClearSessionStorage(); commented by arnie 12202016 for erp

                $("#txtOrgRepID").val('');

                $("#txtOrgRepTitle").val('');
                $("#txtOrgRepLastName").val('');
                $("#txtOrgRepFirstName").val('');
                $("#txtOrgRepMiddleName").val('');
                $("#selOrgRepGenSuffixName").val('');//.selectmenu('refresh');

                $("#selOrgRepIsActive").val('1');//.slider("refresh");

                $('#lstOrgRepContactTel, #lstOrgRepContactMob, #lstOrgRepContactEmail').empty().append('<li class="list-group-item">No result(s) found</li>')

            };

            function ClearOrgRepContactTelFields() {

                $("#txtOrgRepContactTelID").val('');

                $("#txtOrgRepTelCountryID").val('');
                $("#btnOrgRepTelCountryCode").text('Country Code');//.button('refresh');

                $("#txtOrgRepTelAreaCodeID").val('');
                $("#btnOrgRepTelAreaCode").text('Area Code');//.button('refresh');

                $("#txtOrgRepContactNoTel").val('');
                $("#txtOrgRepContactExtensionTel").val('');

                $("#selOrgRepContactTypeTel").val('');//.selectmenu('refresh');

                $("#btnOrgRepContactTelRemove").hide();

            }

            function ClearOrgRepContactMobileFields() {
                $("#txtOrgRepContactMobileID").val('');

                $("#txtOrgRepMobileCountryID").val('');
                $("#btnOrgRepMobileCountryCode").text('Country Code');//.button('refresh');

                $("#txtOrgRepTelCoPrefixID").val('');
                $("#btnOrgRepMobileTelCoPrefixCode").text('Telco Code');//.button('refresh');

                $("#txtOrgRepContactNoMobile").val('');
                $("#selOrgRepContactTypeMobile").val('');//.selectmenu('refresh');

                $("#btnOrgRepContactMobileRemove").hide();

            }

            function ClearOrgRepContactEmailFields() {
                $("#txtOrgRepContactEmailID").val('');

                $("#txtOrgRepContactEmail").val('');
                $("#selOrgRepContactTypeEmail").val('');//.selectmenu('refresh');

                $("#btnOrgRepContactEmailRemove").hide();

            }

            function SaveOrgRep() {
                var pData = [];
                var pDataTel = [];
                var pDataMobile = [];
                var pDataEmail = [];
                var intIndex;

                //Tel
                intIndex = 0;
                for (var i = 0; i <= intOrgRepTelCntr; i++) {

                    if (sessionStorage["OrgRepTel-" + i] != undefined) {

                        arOrgRepTel = new Array();
                        arOrgRepTel = JSON.parse(sessionStorage["OrgRepTel-" + i]);

                        pDataTel[intIndex] = {
                            ID: arOrgRepTel[intOrgRepTelRecID],
                            CountryID: arOrgRepTel[intOrgRepTelCountryID],
                            AreaCodeID: arOrgRepTel[intOrgRepTelAreaCodeID],
                            ContactNo: arOrgRepTel[intOrgRepTelContactNo],
                            ExtensionNo: arOrgRepTel[intOrgRepTelExtNo],
                            ContactType: arOrgRepTel[intOrgRepTelContactType],
                            RecState: arOrgRepTel[intOrgRepTelState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Mobile
                intIndex = 0;
                for (var i = 0; i <= intOrgRepMobileCntr; i++) {

                    if (sessionStorage["OrgRepMobile-" + i] != undefined) {

                        arOrgRepMobile = new Array();
                        arOrgRepMobile = JSON.parse(sessionStorage["OrgRepMobile-" + i]);

                        pDataMobile[intIndex] = {
                            ID: arOrgRepMobile[intOrgRepMobileRecID],
                            CountryID: arOrgRepMobile[intOrgRepMobileCountryID],
                            TelCoPrefixID: arOrgRepMobile[intOrgRepMobileTelCoPrefixID],
                            ContactNo: arOrgRepMobile[intOrgRepMobileContactNo],
                            ContactType: arOrgRepMobile[intOrgRepMobileContactType],
                            RecState: arOrgRepMobile[intOrgRepMobileState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Email
                intIndex = 0;
                for (var i = 0; i <= intOrgRepEmailCntr; i++) {

                    if (sessionStorage["OrgRepEmail-" + i] != undefined) {

                        arOrgRepEmail = new Array();
                        arOrgRepEmail = JSON.parse(sessionStorage["OrgRepEmail-" + i]);

                        pDataEmail[intIndex] = {
                            ID: arOrgRepEmail[intOrgRepEmailRecID],
                            EmailAddress: arOrgRepEmail[intOrgRepEmailAddress],
                            EmailType: arOrgRepEmail[intOrgRepEmailType],
                            RecState: arOrgRepEmail[intOrgRepEmailState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                pData = {
                    OrgRepID: $("#txtOrgRepID").val(),
                    LastName: $("#txtOrgRepLastName").val(),
                    FirstName: $.trim($("#txtOrgRepFirstName").val()),
                    MiddleName: $.trim($("#txtOrgRepMiddleName").val()),
                    Title: $.trim($("#txtOrgRepTitle").val()),
                    GenerationalSuffix: $.trim($("#selOrgRepGenSuffixName").val()),
                    IsActive: $("#selOrgRepIsActive").val(),

                    OrgRepContactTelList: pDataTel,
                    OrgRepContactMobileList: pDataMobile,
                    OrgRepContactEmailList: pDataEmail

                };

                AjaxSend({
                    url: "Organization/SaveOrgRep",
                    data: pData,
                    success: function (vData) {

                        // ChangePage('/Organization/OrganizationRepresentative');

                        $('#pageRepList').show();
                        $('#pageOrgRepInfo').hide();
                        $('#pageOrgRepContactTelInfo').hide();
                        $('#pageOrgRepContactMobileInfo').hide();
                        $('#pageOrgRepContactEmail').hide();
                        getOrgRepresentativeList();

                    }
                });

            };

            function SaveOrgRepContactTel() {
                var arTmp;

                arOrgRepTel = new Array();
                arOrgRepTel[intOrgRepTelRecID] = $("#txtOrgRepContactTelID").val();

                arTmp = $("#btnOrgRepTelCountryCode").text().split("-")
                arOrgRepTel[intOrgRepTelCountryID] = $("#txtOrgRepTelCountryID").val();
                arOrgRepTel[intOrgRepTelCountryCode] = $.trim(arTmp[0]);
                arOrgRepTel[intOrgRepTelCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgRepTelAreaCode").text().split("-")
                arOrgRepTel[intOrgRepTelAreaCodeID] = $("#txtOrgRepTelAreaCodeID").val();
                arOrgRepTel[intOrgRepTelArea] = $.trim(arTmp[0]);
                arOrgRepTel[intOrgRepTelAreaCode] = $.trim(arTmp[1]);

                arOrgRepTel[intOrgRepTelContactNo] = $("#txtOrgRepContactNoTel").val();
                arOrgRepTel[intOrgRepTelExtNo] = $("#txtOrgRepContactExtensionTel").val();
                arOrgRepTel[intOrgRepTelContactType] = $("#selOrgRepContactTypeTel").val();
                arOrgRepTel[intOrgRepTelState] = (IsNewOrgRepTelContact ? "New" : "Edit");

                sessionStorage["OrgRepTel-" + intOrgRepTelIndex] = JSON.stringify(arOrgRepTel);

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
                LoadOrgRepContactTelList();

            };

            function SaveOrgRepContactMobile() {
                var arTmp;

                arOrgRepMobile = new Array();

                arOrgRepMobile[intOrgRepMobileRecID] = $("#txtOrgRepContactMobileID").val();
                arOrgRepMobile[intOrgRepMobileCountryID] = $("#txtOrgRepMobileCountryID").val();
                arTmp = $("#btnOrgRepMobileCountryCode").text().split("-")
                arOrgRepMobile[intOrgRepMobileCountryID] = $("#txtOrgRepMobileCountryID").val();
                arOrgRepMobile[intOrgRepMobileCountryCode] = $.trim(arTmp[0]);
                arOrgRepMobile[intOrgRepMobileCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgRepMobileTelCoPrefixCode").text().split("-")
                arOrgRepMobile[intOrgRepMobileTelCoPrefixID] = $("#txtOrgRepTelCoPrefixID").val();
                arOrgRepMobile[intOrgRepMobilePrefix] = $.trim(arTmp[1]);
                arOrgRepMobile[intOrgRepMobileTelCo] = $.trim(arTmp[0]);

                arOrgRepMobile[intOrgRepMobileContactNo] = $("#txtOrgRepContactNoMobile").val();
                arOrgRepMobile[intOrgRepMobileContactType] = $("#selOrgRepContactTypeMobile").val();
                arOrgRepMobile[intOrgRepMobileState] = (IsNewOrgRepMobileContact ? "New" : "Edit");

                sessionStorage["OrgRepMobile-" + intOrgRepMobileIndex] = JSON.stringify(arOrgRepMobile);

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
                LoadOrgRepContactMobileList();
            };

            function SaveOrgRepContactEmail() {

                arOrgRepEmail = new Array();
                arOrgRepEmail[intOrgRepEmailRecID] = $("#txtOrgRepContactEmailID").val();
                arOrgRepEmail[intOrgRepEmailAddress] = $("#txtOrgRepContactEmail").val();
                arOrgRepEmail[intOrgRepEmailType] = $("#selOrgRepContactTypeEmail").val();
                arOrgRepEmail[intOrgRepEmailState] = (IsNewOrgRepEmailContact ? "New" : "Edit");

                sessionStorage["OrgRepEmail-" + intOrgRepEmailIndex] = JSON.stringify(arOrgRepEmail);

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
                LoadOrgRepContactEmailList();
            };

            function RemoveOrgRepContactTel() {
                var arTmp;

                arOrgRepTel = new Array();
                arOrgRepTel[intOrgRepTelRecID] = $("#txtOrgRepContactTelID").val();

                arTmp = $("#btnOrgRepTelCountryCode").text().split("-")
                arOrgRepTel[intOrgRepTelCountryID] = $("#txtOrgRepTelCountryID").val();
                arOrgRepTel[intOrgRepTelCountryCode] = $.trim(arTmp[0]);
                arOrgRepTel[intOrgRepTelCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgRepTelAreaCode").text().split("-")
                arOrgRepTel[intOrgRepTelAreaCodeID] = $("#txtOrgRepTelAreaCodeID").val();
                arOrgRepTel[intOrgRepTelArea] = $.trim(arTmp[0]);
                arOrgRepTel[intOrgRepTelAreaCode] = $.trim(arTmp[1]);

                arOrgRepTel[intOrgRepTelContactNo] = $("#txtOrgRepContactNoTel").val();
                arOrgRepTel[intOrgRepTelExtNo] = $("#txtOrgRepContactExtensionTel").val();
                arOrgRepTel[intOrgRepTelContactType] = $("#selOrgRepContactTypeTel").val();
                arOrgRepTel[intOrgRepTelState] = "Remove";

                sessionStorage["OrgRepTel-" + intOrgRepTelIndex] = JSON.stringify(arOrgRepTel);

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
                LoadOrgRepContactTelList();
            };

            function RemoveOrgRepContactMobile() {
                var arTmp;

                arOrgRepMobile = new Array();

                arOrgRepMobile[intOrgRepMobileRecID] = $("#txtOrgRepContactMobileID").val();

                arTmp = $("#btnOrgRepMobileCountryCode").text().split("-")
                arOrgRepMobile[intOrgRepMobileCountryID] = $("#txtOrgRepMobileCountryID").val();
                arOrgRepMobile[intOrgRepMobileCountryCode] = $.trim(arTmp[0]);
                arOrgRepMobile[intOrgRepMobileCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgRepMobileTelCoPrefixCode").text().split("-")
                arOrgRepMobile[intOrgRepMobileTelCoPrefixID] = $("#txtOrgRepTelCoPrefixID").val();
                arOrgRepMobile[intOrgRepMobilePrefix] = $.trim(arTmp[1]);
                arOrgRepMobile[intOrgRepMobileTelCo] = $.trim(arTmp[0]);

                arOrgRepMobile[intOrgRepMobileContactNo] = $("#txtOrgRepContactNoMobile").val();
                arOrgRepMobile[intOrgRepMobileContactType] = $("#selOrgRepContactTypeMobile").val();
                arOrgRepMobile[intOrgRepMobileState] = "Remove";

                sessionStorage["OrgRepMobile-" + intOrgRepMobileIndex] = JSON.stringify(arOrgRepMobile);

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
                LoadOrgRepContactMobileList();
            };

            function RemoveOrgRepContactEmail() {

                arOrgRepEmail = new Array();
                arOrgRepEmail[intOrgRepEmailRecID] = $("#txtOrgRepContactEmailID").val();
                arOrgRepEmail[intOrgRepEmailAddress] = $("#txtOrgRepContactEmail").val();
                arOrgRepEmail[intOrgRepEmailType] = $("#selOrgRepContactTypeEmail").val();
                arOrgRepEmail[intOrgRepEmailState] = "Remove";

                sessionStorage["OrgRepEmail-" + intOrgRepEmailIndex] = JSON.stringify(arOrgRepEmail);

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
                LoadOrgRepContactEmailList();
            };

            function LoadOrgRepContactTelList() {
                var pLV = $('#lstOrgRepContactTel');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgRepTelCntr; i++) {

                    if (sessionStorage["OrgRepTel-" + i] != undefined) {

                        arOrgRepTel = new Array();
                        arOrgRepTel = JSON.parse(sessionStorage["OrgRepTel-" + i]);

                        if (arOrgRepTel[intOrgRepTelState] != "Remove") {

                            pItem = $('<li class="list-group-item" id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgRepTel)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgRepContactTelInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<div>')
                                        .attr("style", "white-space:normal")
                                        .text("+" + arOrgRepTel[intOrgRepTelCallingCode] + "." + arOrgRepTel[intOrgRepTelAreaCode] + '.' + arOrgRepTel[intOrgRepTelContactNo] + ((arOrgRepTel[intOrgRepTelExtNo] || '') == '' ? '' : ", Ext. " + arOrgRepTel[intOrgRepTelExtNo]))
                                    )
                                    .append($('<div>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgRepTel[intOrgRepTelArea] + ", " + arOrgRepTel[intOrgRepTelCountryCode])
                                    )
                                    .append($('<div>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgRepTel[intOrgRepTelContactType])
                                    )
                            );

                            pLV.append(pItem);

                        }

                    }
                }

                pLV;//.listview('refresh');
            };

            function LoadOrgRepContactMobileList() {
                var pLV = $('#lstOrgRepContactMob');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgRepMobileCntr; i++) {

                    if (sessionStorage["OrgRepMobile-" + i] != undefined) {

                        arOrgRepMobile = new Array();
                        arOrgRepMobile = JSON.parse(sessionStorage["OrgRepMobile-" + i]);

                        if (arOrgRepMobile[intOrgRepMobileState] != "Remove") {

                            pItem = $('<li class="list-group-item" id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgRepMobile)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgRepContactMobileInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<div>')
                                        .attr("style", "white-space:normal")
                                        .text("+" + arOrgRepMobile[intOrgRepMobileCallingCode] + "." + arOrgRepMobile[intOrgRepMobilePrefix] + "." + arOrgRepMobile[intOrgRepMobileContactNo])
                                    )
                                    .append($('<div>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgRepMobile[intOrgRepMobileTelCo] + " (" + arOrgRepMobile[intOrgRepMobileCountryCode] + ")")
                                    )
                                    .append($('<div>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgRepMobile[intOrgRepMobileContactType])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV;//.listview('refresh');
            };


            function LoadOrgRepContactEmailList() {
                var pLV = $('#lstOrgRepContactEmail');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgRepEmailCntr; i++) {

                    if (sessionStorage["OrgRepEmail-" + i] != undefined) {

                        arOrgRepEmail = new Array();
                        arOrgRepEmail = JSON.parse(sessionStorage["OrgRepEmail-" + i]);

                        if (arOrgRepEmail[intOrgRepEmailState] != "Remove") {

                            pItem = $('<li class="list-group-item" id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgRepEmail)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgRepContactEmailInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<div>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgRepEmail[intOrgRepEmailAddress])
                                    )
                                    .append($('<div>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgRepEmail[intOrgRepEmailType])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV;//.listview('refresh');
            };




        });
    });
