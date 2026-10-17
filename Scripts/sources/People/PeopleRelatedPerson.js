
require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryPeoplePreSuffix.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryPeoplePostSuffix.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryAreaCodes.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryTelCoPrefixes.js?crc=')
    ], function() {

    $(function () {

        var IsNewPostSuffix = true;
        var arPostSuffix = new Array();
        var intPostSuffixCntr = 0;
        var intPostSuffixIndex = 0;

        var intPostSuffixRecID = 0;
        var intPostSuffixID = 1;
        var intPostSuffix = 2;
        var intPostSuffixState = 3;

        $(document).ready(function () {
            getPeopleRelatedList();
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

        $('#btnPeopleRelatedPopUpMenu').click(function (e) {
            e.preventDefault();

            $("#divPeopleRelatedPopUpMenu").popup("open")

        });

        $('#popPeopleRelatedPeopleInfoName').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoName')), "_self");

        });

        $('#popPeopleRelatedPeopleInfoAlias').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAlias')), "_self");

        });

        $('#popPeopleRelatedPeopleInfoAddress').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAddress')), "_self");

        });

        $('#popPeopleRelatedPeoplePersonalInfo').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePersonalInfo')), "_self");

        });

        $('#popPeopleRelatedPeopleContactNo').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleContactNo')), "_self");

        });

        $('#popPeopleRelatedPeopleEducation').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEducation')), "_self");

        });

        $('#popPeopleRelatedPeopleEmpHistory').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEmpHistory')), "_self");

        });

        $('#popPeopleRelatedPeoplePrimeIDReference').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePrimeIDReference')), "_self");

        });

        $('#popPeopleRelatedPeopleUserAccount').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleUserAccount')), "_self");

        });

        $('#btnPeopleRelPersonAdd').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeopleRelPersonInfo');
            ClearPeopleRelatedFields();

        });

        $('#btnRelPersonTitle').click(function () {

            Browser.LibraryPeoplePreSuffix.Click = function (e) {

                $('#txtRelPersonTitleID').val(e.ID)
                $('#btnRelPersonTitle').text(e.Code)
                    .button('refresh');

            };
            Browser.LibraryPeoplePreSuffix.Show();

        });

        $('#btnAddPostSuffix').click(function (e) {

            Browser.LibraryPeoplePostSuffix.Click = function (e) {

                IsNewPostSuffix = true
                intPostSuffixCntr = intPostSuffixCntr + 1;
                intPostSuffixIndex = intPostSuffixCntr;

                arPostSuffix = new Array();
                arPostSuffix[intPostSuffixRecID] = "";
                arPostSuffix[intPostSuffixID] = e.ID;
                arPostSuffix[intPostSuffix] = e.Code;
                arPostSuffix[intPostSuffixState] = (IsNewPostSuffix ? "New" : "Edit");

                sessionStorage["PostSuffix-" + intPostSuffixIndex] = JSON.stringify(arPostSuffix);

                LoadPostSuffixList();

            };

            Browser.LibraryPeoplePostSuffix.Show();

        });

        $('#btnTelCountryCode').click(function () {

            Browser.LibraryCountries.Click = function (e) {

                $('#txtTelCountryID').val(e.ID)
                $('#btnTelCountryCode').text(e.Code + " - " + e.CallingCode)
                    .button('refresh');

            };
            Browser.LibraryCountries.Show();

        });

        $('#btnTelAreaCode').click(function () {

            Browser.LibraryAreaCodes.Click = function (e) {

                $('#txtTelAreaCodeID').val(e.ID)
                $('#btnTelAreaCode').text(e.Area + ' - ' + e.CallingCode)
                    .button('refresh');

            };
            Browser.LibraryAreaCodes.Show();

        });

        $("#txtTelContactNo").keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        });

        $("#txtContactExtension").keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        });

        $('#btnMobileCountryCode').click(function () {

            Browser.LibraryCountries.Click = function (e) {

                $('#txtMobileCountryID').val(e.ID)
                $('#btnMobileCountryCode').text(e.Code + " - " + e.CallingCode)
                    .button('refresh');

            };
            Browser.LibraryCountries.Show();

        });

        $('#btnMobileTelCoPrefixCode').click(function () {

            Browser.LibraryTelcoPrefixes.Click = function (e) {

                $('#txtTelCoPrefixID').val(e.ID)
                $('#btnMobileTelCoPrefixCode').text(e.Prefix)
                    .button('refresh');

            };
            Browser.LibraryTelcoPrefixes.Show();

        });

        $("#txtMobileContactNo").keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        });

        $('#btnRelPersonSave').click(function (e) {
            e.preventDefault();

            if ($.trim($("#txtRelPersonSurname").val()) == "") {
                MessageBox("Please enter surname.",
                            "Surname",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtRelPersonFirstName").val()) == "") {
                MessageBox("Please enter first name.",
                            "First Name",
                            [
                                { Text: 'Ok' }
                            ]
                );

                return;
            }

            if ($.trim($("#selEmergencyRef").val()) == "0") {

                $("#txtTelCountryID").val('');
                $("#btnTelCountryCode").text('Country Code');
                $("#txtTelAreaCodeID").val('');
                $("#btnTelAreaCode").text('Area Code');
                $("#txtTelContactNo").val('');
                $("#txtContactExtension").val('');
                $("#selContactTypeTel").val('');
                $("#selContactTypeTel").selectmenu('refresh');

                $("#txtMobileCountryID").val('');
                $("#btnMobileCountryCode").text('Country Code');
                $("#txtTelCoPrefixID").val('');
                $("#btnMobileTelCoPrefixCode").text('Telco Code');
                $("#txtMobileContactNo").val('');
                $("#selContactTypeMobile").val('');
                $("#selContactTypeMobile").selectmenu('refresh');

                $("#txtEmailAddress").val('');
                $("#selContactTypeEmail").val('');
                $("#selContactTypeEmail").selectmenu('refresh');

            }
            else {

                if ($.trim($("#txtTelContactNo").val()) == "" && $.trim($("#txtMobileContactNo").val()) == "" && $.trim($("#txtEmailAddress").val()) == "") {
                    MessageBox("Please enter telephone contact, mobile contact or email contact.",
                            "Contact Information",
                            [
                                { Text: 'Ok' }
                            ]
                    );

                    return;
                }

                if ($.trim($("#txtTelContactNo").val()) != "") {

                    if ($.trim($("#txtTelCountryID").val()) == "") {
                        MessageBox("Please select telephone country code.",
                                   "Country Code",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                        return;
                    }

                    if ($.trim($("#txtTelAreaCodeID").val()) == "") {
                        MessageBox("Please select telephone area code.",
                                   "Area Code",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                        return;
                    }

                    if ($.trim($("#txtTelContactNo").val()).length != 7) {
                        MessageBox("Please enter 7 digit telephone number.",
                                   "Tel. No.",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                        return;
                    }

                    if (!$.isNumeric($("#txtTelContactNo").val())) {
                        MessageBox("Please enter valid telephone number.",
                                   "Tel. No.",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                        return;
                    }

                    if ($.trim($("#txtContactExtension").val()).length > 0) {
                        if (!$.isNumeric($("#txtContactExtension").val())) {
                            MessageBox("Please enter valid extension or local number.",
                                   "Tel. No.",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                            return;
                        }
                    }

                    if ($.trim($("#selContactTypeTel").val()) == "") {
                        MessageBox("Please select telephone contact type.",
                                   "Telephone Contact Type",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                        return;
                    }

                }

                if ($.trim($("#txtMobileContactNo").val()) != "") {

                    if ($.trim($("#txtMobileCountryID").val()) == "") {
                        MessageBox("Please select mobile country code.",
                                   "Country Code",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                        return;
                    }

                    if ($.trim($("#txtTelCoPrefixID").val()) == "") {
                        MessageBox("Please select mobile prefix number.",
                                   "Prefix Number",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                        return;
                    }

                    if ($.trim($("#txtMobileContactNo").val()).length != 7) {
                        MessageBox("Please enter 7 digit mobile number.",
                                   "Mobile No.",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                        return;
                    }

                    if (!$.isNumeric($("#txtMobileContactNo").val())) {
                        MessageBox("Please enter valid mobile number.",
                                   "Mobile No.",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                        return;
                    }

                    if ($.trim($("#selContactTypeMobile").val()) == "") {
                        MessageBox("Please select mobile contact type.",
                                   "Mobile Contact Type",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                        return;
                    }

                }

                if ($.trim($("#txtEmailAddress").val()) != "") {
                    if (IsEmail($.trim($("#txtEmailAddress").val())) == false) {
                        MessageBox("Please enter valid email address.",
                                    "Email Address",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );

                        return;
                    }

                    if ($.trim($("#selContactTypeEmail").val()) == "") {
                        MessageBox("Please select email type.",
                                   "Email Type",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                        );
                        return;
                    }
                }

            }

            SavePersonRelatedInfo();

        });

        $('#btnRelPersonRemove').click(function (e) {
            e.preventDefault();

            MessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemovePersonRelatedInfo();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );

        });

        $('#btnRelPersonCancel').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeopleRelPersonList');

        });

        $('#btnPeopleRelPersonDone').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoSummary')), "_self");

        });

        $('#selEmergencyRef').change(function (e) {
            e.preventDefault();

            ShowHideEmergencyDetails();

        });

        function ShowHideEmergencyDetails() {

            if ($.trim($("#selEmergencyRef").val()) == "1") {
                $("#divEmergencyDetails").show();
            }
            else {
                $("#divEmergencyDetails").hide();
            }
        }

        function getPeopleRelatedList() {
            AjaxSend({
                url: 'People/getPeopleRelatedList',
                success: function (vData) {
                    LoadPeopleRelatedList(vData.PeopleRelatedList.Rows);
                }
            });
        }

        function LoadPeopleRelatedList(vRows) {
            var pLV = $('#lstPeopleRelPerson');
            var pItem;

            $("#divRecordCount").text(" Number of record(s) found: " + vRows.length);

            pLV.html('');

            pItem = $('<li>')
                    .attr('data-role', "list-divider")
                    .text('Related Person List');
            pLV.append(pItem);


            for (var p = 0; p <= vRows.length - 1; p++) {

                pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .click(function (e) {
                            e.preventDefault();
                            $.mobile.changePage('#pagePeopleRelPersonInfo');
                            ShowPersonRelatedInfo($(this).attr('ID'));
                        })
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].RelatedName)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text((vRows[p].IsEmergencyRef == 1 ? 'Emergency Reference' : ''))
                        )
                    );
                pLV.append(pItem);
            };

            pLV.listview('refresh');
        };

        function ShowPersonRelatedInfo(strID) {
            var pData = {};
            pData.ID = strID;

            ClearPeopleRelatedFields();

            AjaxSend({
                url: "People/getPeopleRelatedInfo",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleRelatedInfo.Rows.length > 0) {

                        $("#txtRelPersonID").val(vData.PeopleRelatedInfo.Rows[0].ID || '');

                        $("#txtRelPersonTitleID").val(vData.PeopleRelatedInfo.Rows[0].TitleID || '');
                        $("#btnRelPersonTitle").text(vData.PeopleRelatedInfo.Rows[0].Title || 'Please Select').button('refresh');

                        $("#txtRelPersonSurname").val(vData.PeopleRelatedInfo.Rows[0].Surname || '');
                        $("#txtRelPersonFirstName").val(vData.PeopleRelatedInfo.Rows[0].FirstName || '');
                        $("#txtRelPersonMiddleName").val(vData.PeopleRelatedInfo.Rows[0].MiddleName || '');

                        $("#selRelPersonSuffixName").val(vData.PeopleRelatedInfo.Rows[0].PersonalSuffixName || '');
                        $('#selRelPersonSuffixName').selectmenu('refresh');

                        $("#selRelation").val(vData.PeopleRelatedInfo.Rows[0].Relation || '');
                        $('#selRelation').selectmenu('refresh');

                        $("#selEmergencyRef").val(vData.PeopleRelatedInfo.Rows[0].IsEmergencyRef || '0');
                        $("#selEmergencyRef").slider("refresh");

                        $("#txtTelCountryID").val(vData.PeopleRelatedInfo.Rows[0].TelCountryID || '');
                        $("#btnTelCountryCode").text(($.trim(vData.PeopleRelatedInfo.Rows[0].TelCountryID) != "" ? (vData.PeopleRelatedInfo.Rows[0].TelCountryCode + ' - ' + vData.PeopleRelatedInfo.Rows[0].TelCountryCallingCode) : 'Country Code')).button('refresh');
                        $("#txtTelAreaCodeID").val(vData.PeopleRelatedInfo.Rows[0].TelAreaCodeID || '');
                        $("#btnTelAreaCode").text(($.trim(vData.PeopleRelatedInfo.Rows[0].TelAreaCodeID) != "" ? (vData.PeopleRelatedInfo.Rows[0].Area + ' - ' + vData.PeopleRelatedInfo.Rows[0].AreaCode) : 'Area Code')).button('refresh');
                        $("#txtTelContactNo").val(vData.PeopleRelatedInfo.Rows[0].TelContactNo || '');
                        $("#txtContactExtension").val(vData.PeopleRelatedInfo.Rows[0].TelExtensionNo || '');
                        $("#selContactTypeTel").val(vData.PeopleRelatedInfo.Rows[0].TelContactType || '');
                        $("#selContactTypeTel").selectmenu('refresh');

                        $("#txtMobileCountryID").val(vData.PeopleRelatedInfo.Rows[0].MobileCountryID || '');
                        $("#btnMobileCountryCode").text(($.trim(vData.PeopleRelatedInfo.Rows[0].MobileCountryID) != "" ? (vData.PeopleRelatedInfo.Rows[0].MobileCountryCode + ' - ' + vData.PeopleRelatedInfo.Rows[0].MobileCountryCallingCode) : 'Country Code')).button('refresh');
                        $("#txtTelCoPrefixID").val(vData.PeopleRelatedInfo.Rows[0].MobileTelCoPrefixID || '');
                        $("#btnMobileTelCoPrefixCode").text(($.trim(vData.PeopleRelatedInfo.Rows[0].MobileTelCoPrefixID) != "" ? (vData.PeopleRelatedInfo.Rows[0].MobileTelCo + ' - ' + vData.PeopleRelatedInfo.Rows[0].MobilePrefix) : 'Telco Code')).button('refresh');
                        $("#txtMobileContactNo").val(vData.PeopleRelatedInfo.Rows[0].MobileContactNo || '');
                        $("#selContactTypeMobile").val(vData.PeopleRelatedInfo.Rows[0].MobileContactType || '');
                        $("#selContactTypeMobile").selectmenu('refresh');

                        $("#txtEmailAddress").val(vData.PeopleRelatedInfo.Rows[0].EmailAddress || '');
                        $("#selContactTypeEmail").val(vData.PeopleRelatedInfo.Rows[0].EmailType || '');
                        $("#selContactTypeEmail").selectmenu('refresh');

                        AjaxSend({
                            url: "People/getPeopleRelatedPostSuffixList",
                            data: pData,
                            success: function (vDataPostSuffix) {

                                LoadPeoplePostSuffixList(vDataPostSuffix.PeopleRelatedData.Rows);

                            }
                        });

                        ShowHideEmergencyDetails();

                        $("#btnRelPersonRemove").show();
                    }
                }
            });

        };

        function LoadPeoplePostSuffixList(vRows) {

            for (var p = 0; p <= vRows.length - 1; p++) {

                arPostSuffix = new Array();
                arPostSuffix[intPostSuffixRecID] = vRows[p].ID;
                arPostSuffix[intPostSuffixID] = vRows[p].PostSuffixID;
                arPostSuffix[intPostSuffix] = vRows[p].PostSuffixCode;
                arPostSuffix[intPostSuffixState] = "";

                sessionStorage["PostSuffix-" + p] = JSON.stringify(arPostSuffix);

            };

            intPostSuffixCntr = vRows.length;
            LoadPostSuffixList();

        };

        function ClearPeopleRelatedFields() {

            // dont clear
            // sessionStorage.clear();
            proj.ClearSessionStorage();

            $("#txtRelPersonID").val('');

            $("#txtRelPersonTitleID").val('');
            $("#btnRelPersonTitle").text('Please Select').button('refresh');

            $("#txtRelPersonSurname").val('');
            $("#txtRelPersonFirstName").val('');
            $("#txtRelPersonMiddleName").val('');

            $("#selRelPersonSuffixName").val('');
            $("#selRelPersonSuffixName").selectmenu('refresh');

            $("#selRelation").val('');
            $("#selRelation").selectmenu('refresh');

            $("#selEmergencyRef").val('0');
            $("#selEmergencyRef").slider("refresh");

            $("#txtTelCountryID").val('');
            $("#btnTelCountryCode").text('Country Code').button('refresh');
            $("#txtTelAreaCodeID").val('');
            $("#btnTelAreaCode").text('Area Code').button('refresh');
            $("#txtTelContactNo").val('');
            $("#txtContactExtension").val('');
            $("#selContactTypeTel").val('');
            $("#selContactTypeTel").selectmenu('refresh');

            $("#txtMobileCountryID").val('');
            $("#btnMobileCountryCode").text('Country Code').button('refresh');
            $("#txtTelCoPrefixID").val('');
            $("#btnMobileTelCoPrefixCode").text('Telco Code').button('refresh');
            $("#txtMobileContactNo").val('');
            $("#selContactTypeMobile").val('');
            $("#selContactTypeMobile").selectmenu('refresh');

            $("#txtEmailAddress").val('');
            $("#selContactTypeEmail").val('');
            $("#selContactTypeEmail").selectmenu('refresh');

            LoadPostSuffixList();

            ShowHideEmergencyDetails();

            $("#btnRelPersonRemove").hide();

        }

        function SavePersonRelatedInfo() {
            var pData = [];
            var intIndex;

            intIndex = 0;
            for (var i = 0; i <= intPostSuffixCntr; i++) {

                if (sessionStorage["PostSuffix-" + i] != undefined) {

                    arPostSuffix = new Array();
                    arPostSuffix = JSON.parse(sessionStorage["PostSuffix-" + i]);

                    pData[intIndex] = {
                        ID: arPostSuffix[intPostSuffixRecID],
                        PostSuffixID: arPostSuffix[intPostSuffixID],
                        RecState: arPostSuffix[intPostSuffixState]
                    };

                    intIndex = intIndex + 1;

                }
            }

            pData = {
                RelPersonID : $("#txtRelPersonID").val(),
                TitleID : $("#txtRelPersonTitleID").val(),
                Surname : $("#txtRelPersonSurname").val(),
                FirstName : $("#txtRelPersonFirstName").val(),
                MiddleName : $("#txtRelPersonMiddleName").val(),
                PersonalSuffixName : $("#selRelPersonSuffixName").val(),
                Relation : $("#selRelation").val(),
                IsEmergencyRef : $("#selEmergencyRef").val(),
                TelCountryID : $("#txtTelCountryID").val(),
                TelAreaCodeID : $("#txtTelAreaCodeID").val(),
                TelContactNo : $("#txtTelContactNo").val(),
                TelExtensionNo : $("#txtContactExtension").val(),
                TelContactType : $("#selContactTypeTel").val(),
                MobileCountryID : $("#txtMobileCountryID").val(),
                MobileTelCoPrefixID : $("#txtTelCoPrefixID").val(),
                MobileContactNo : $("#txtMobileContactNo").val(),
                MobileContactType : $("#selContactTypeMobile").val(),
                EmailAddress : $("#txtEmailAddress").val(),
                EmailType : $("#selContactTypeEmail").val(),
                PostSuffixList: pData
            };

            AjaxSend({
                url: "People/CheckPeopleUniqueRelatedName",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleRelatedData.Rows.length > 0) {
                        MessageBox("Related person with the same name already exist.",
                            "Related Person Unique Name",
                            [
                                { Text: 'Ok' }
                            ]
                        );
                    }
                    else {

                        AjaxSend({
                            url: "People/SavePeopleRelatedInfo",
                            data: pData,
                            success: function (vData) {

                                if (vData.PeopleRelatedData == "Invalid Tel. No.") {
                                    MessageBox("Please enter valid telephone number.",
                                        "Tel. No.",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                                }
                                else if (vData.PeopleRelatedData == "Invalid Extension or Local No.") {
                                    MessageBox("Please enter valid telephone extension or local number.",
                                        "Tel. Extension or Local No.",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                                }
                                else if (vData.PeopleRelatedData == "Invalid Mobile No.") {
                                    MessageBox("Please enter valid mobile number.",
                                        "Mobile No.",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                                }
                                else if (vData.PeopleRelatedData == "Invalid Email") {
                                    MessageBox("Please enter valid email address.",
                                        "Email Address",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                                }
                                else {
                                    $.mobile.changePage('#pagePeopleRelPersonList');
                                    getPeopleRelatedList();
                                }

                            }
                        });

                    }

                }
            });

        };

        function RemovePersonRelatedInfo() {

            var pData = {};
            pData.RelPersonID = $("#txtRelPersonID").val();

            AjaxSend({
                url: "People/RemovePeopleRelated",
                data: pData,
                success: function (vData) {

                    $.mobile.changePage('#pagePeopleRelPersonList');
                    getPeopleRelatedList();

                }
            });

        };

        function LoadPostSuffixList() {
            var pLV = $('#lstPeoplePostSuffix');
            var pItem;

            pLV.html('');
            pItem = $('<li>')
                    .attr('data-role', "list-divider")
                    .text('Post Nominal Suffix');
            pLV.append(pItem);

            for (var i = 0; i <= intPostSuffixCntr; i++) {

                if (sessionStorage["PostSuffix-" + i] != undefined) {

                    arPostSuffix = new Array();
                    arPostSuffix = JSON.parse(sessionStorage["PostSuffix-" + i]);

                    if (arPostSuffix[intPostSuffixState] != "Remove") {

                        pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arPostSuffix)
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arPostSuffix[intPostSuffix])
                                    )
                            )
                            .append($('<a>')
                                    .attr('ID', i)
                                    .attr('href', "#")
                                    .data('Data', arPostSuffix)
                                    .click(function (e) {
                                        e.preventDefault();

                                        sessionStorage["PostSuffixToRemoveIndex"] = $(this).attr('ID');
                                        sessionStorage["PostSuffixToRemove"] = JSON.stringify($(this).data('Data'));

                                        MessageBox("Remove this record?",
                                            'Remove',
                                            [
                                                { Text: 'Yes',
                                                    Click: function () {

                                                        var arPostSuffixToRemove = new Array();
                                                        arPostSuffixToRemove = JSON.parse(sessionStorage["PostSuffixToRemove"]);
                                                        intPostSuffixIndex = sessionStorage["PostSuffixToRemoveIndex"];

                                                        arPostSuffixToRemove[intPostSuffixState] = "Remove";
                                                        sessionStorage["PostSuffix-" + intPostSuffixIndex] = JSON.stringify(arPostSuffixToRemove);

                                                        LoadPostSuffixList();

                                                        sessionStorage["PostSuffixToRemove"] = null;
                                                        $('.ui-dialog').dialog('close');
                                                    }
                                                }
                                            ,
                                                { Text: 'No'
                                                }
                                            ]
                                        );

                                    })
                            );

                        pLV.append(pItem);

                    }

                }
            }

            pLV.listview('refresh');
        };
    });
});