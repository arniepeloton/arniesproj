require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryAreaCodes.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryTelCoPrefixes.js?crc=')
    ], function () {

        $(function () {

            $(document).ready(function () {
                getPeopleContactTelList();
                getPeopleContactMobileList();
                getPeopleContactEmailList();
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

            $('#btnPeopleContactPopUpMenu').click(function (e) {
                e.preventDefault();

                $("#divPeopleContactPopUpMenu").popup("open")

            });

            $('#popPeopleContactPeopleInfoName').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoName')), "_self");

            });

            $('#popPeopleContactPeopleInfoAlias').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAlias')), "_self");

            });

            $('#popPeopleContactPeopleInfoAddress').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAddress')), "_self");

            });

            $('#popPeopleContactPeoplePersonalInfo').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePersonalInfo')), "_self");

            });

            $('#popPeopleContactPeopleEducation').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEducation')), "_self");

            });

            $('#popPeopleContactPeopleEmpHistory').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEmpHistory')), "_self");

            });

            $('#popPeopleContactPeopleRelatedPerson').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleRelatedPerson')), "_self");

            });

            $('#popPeopleContactPeoplePrimeIDReference').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePrimeIDReference')), "_self");

            });

            $('#popPeopleContactPeopleUserAccount').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleUserAccount')), "_self");

            });

            $('#btnPeopleContactNoTelAdd').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pagePeopleContactTelInfo');
                ClearPeopleContactTelFields();

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

            $("#txtContactNoTel").keydown(function (e) {
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

            $("#txtContactNoMobile").keydown(function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            });

            $('#btnPeopleContactNoMobAdd').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pagePeopleContactMobileInfo');
                ClearPeopleContactMobileFields();

            });

            $('#btnPeopleContactEmailAdd').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pagePeopleContactMobileEmail');
                ClearPeopleContactEmailFields();

            });

            $('#btnPeopleContactTelSave').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtTelCountryID").val()) == "") {
                    MessageBox("Please select country code.",
                           "Country Code",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#txtTelAreaCodeID").val()) == "") {
                    MessageBox("Please select area code.",
                           "Area Code",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#txtContactNoTel").val()) == "") {
                    MessageBox("Please enter 7 digit telephone number.",
                           "Tel. No.",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#txtContactNoTel").val()).length != 7) {
                    MessageBox("Please enter 7 digit telephone number.",
                           "Tel. No.",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if (!$.isNumeric($("#txtContactNoTel").val())) {
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

                SavePeopleContactTel();

            });

            $('#btnPeopleContactMobileSave').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtMobileCountryID").val()) == "") {
                    MessageBox("Please select country code.",
                           "Country Code",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#txtTelCoPrefixID").val()) == "") {
                    MessageBox("Please select prefix number.",
                           "Prefix Number",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#txtContactNoMobile").val()) == "") {
                    MessageBox("Please enter 7 digit mobile number.",
                           "Mobile No.",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#txtContactNoMobile").val()).length != 7) {
                    MessageBox("Please enter 7 digit mobile number.",
                           "Mobile No.",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if (!$.isNumeric($("#txtContactNoMobile").val())) {
                    MessageBox("Please enter valid mobile number.",
                                   "MObile No.",
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

                SavePeopleContactMobile();

            });

            $('#btnPeopleContactEmailSave').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtContactEmail").val()) == "") {
                    MessageBox("Please enter email address.",
                           "Email Address",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if (IsEmail($.trim($("#txtContactEmail").val())) == false) {
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

                SavePeopleContactEmail();

            });

            $('#btnPeopleContactTelRemove').click(function (e) {
                e.preventDefault();

                MessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemovePeopleContactTel();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );

            });

            $('#btnPeopleContactMobileRemove').click(function (e) {
                e.preventDefault();

                MessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemovePeopleContactMobile();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );

            });

            $('#btnPeopleContactEmailRemove').click(function (e) {
                e.preventDefault();

                MessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemovePeopleContactEmail();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );

            });

            $('#btnPeopleContactTelCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pagePeopleContactList');

            });

            $('#btnPeopleContactMobileCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pagePeopleContactList');

            });

            $('#btnPeopleContactEmailCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pagePeopleContactList');

            });

            $('#btnPeopleContactDone').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoSummary')), "_self");

            });

            function getPeopleContactTelList() {
                AjaxSend({
                    url: 'People/getPeopleContactTelList',
                    success: function (vData) {
                        LoadPeopleContactTelList(vData.PeopleContactList.Rows);
                    }
                });
            }

            function LoadPeopleContactTelList(vRows) {
                var pLV = $('#lstPeopleContactTel');
                var pItem;

                $("#divRecordCountTel").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                $.mobile.changePage('#pagePeopleContactTelInfo');
                                ShowPersonContactTelInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text("+" + vRows[p].CountryCode + "." + vRows[p].AreaCode + '.' + vRows[p].ContactNo + ((vRows[p].ExtensionNo || '') == '' ? '' : ", Ext. " + vRows[p].ExtensionNo))
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Area + ", " + vRows[p].Country)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].ContactType)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function ShowPersonContactTelInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearPeopleContactTelFields();

                AjaxSend({
                    url: "People/getPeopleContactTelInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.PeopleContactInfo.Rows.length > 0) {

                            $("#txtContactTelID").val(vData.PeopleContactInfo.Rows[0].ID || '');

                            $("#txtTelCountryID").val(vData.PeopleContactInfo.Rows[0].CountryID || '');
                            $("#btnTelCountryCode").text((vData.PeopleContactInfo.Rows[0].CountryCode + ' - ' + vData.PeopleContactInfo.Rows[0].CountryCallingCode) || 'Country Code');
                            $("#btnTelCountryCode").button('refresh');

                            $("#txtTelAreaCodeID").val(vData.PeopleContactInfo.Rows[0].AreaCodeID || '');
                            $("#btnTelAreaCode").text((vData.PeopleContactInfo.Rows[0].Area + ' - ' + vData.PeopleContactInfo.Rows[0].AreaCode) || 'Area Code');
                            $("#btnTelAreaCode").button('refresh');

                            $("#txtContactNoTel").val(vData.PeopleContactInfo.Rows[0].ContactNo || '');
                            $("#txtContactExtension").val(vData.PeopleContactInfo.Rows[0].ExtensionNo || '');

                            $("#selContactTypeTel").val(vData.PeopleContactInfo.Rows[0].ContactType || '');
                            $('#selContactTypeTel').selectmenu('refresh');

                            $("#btnPeopleContactTelRemove").show();
                        }
                    }
                });

            };

            function getPeopleContactMobileList() {
                AjaxSend({
                    url: 'People/getPeopleContactMobileList',
                    success: function (vData) {
                        LoadPeopleContactMobileList(vData.PeopleContactList.Rows);
                    }
                });
            }

            function LoadPeopleContactMobileList(vRows) {
                var pLV = $('#lstPeopleContactMob');
                var pItem;

                $("#divRecordCountMob").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                $.mobile.changePage('#pagePeopleContactMobileInfo');
                                ShowPersonContactMobileInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text("+" + vRows[p].CountryCode + "." + vRows[p].Prefix + "." + vRows[p].ContactNo)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].TelCo + " (" + vRows[p].Country + ")")
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].ContactType)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function ShowPersonContactMobileInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearPeopleContactMobileFields();

                AjaxSend({
                    url: "People/getPeopleContactMobileInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.PeopleContactInfo.Rows.length > 0) {

                            $("#txtContactMobileID").val(vData.PeopleContactInfo.Rows[0].ID || '');

                            $("#txtMobileCountryID").val(vData.PeopleContactInfo.Rows[0].CountryID || '');
                            $("#btnMobileCountryCode").text((vData.PeopleContactInfo.Rows[0].CountryCode + ' - ' + vData.PeopleContactInfo.Rows[0].CountryCallingCode) || 'Country Code');
                            $("#btnMobileCountryCode").button('refresh');

                            $("#txtTelCoPrefixID").val(vData.PeopleContactInfo.Rows[0].TelCoPrefixID || '');
                            $("#btnMobileTelCoPrefixCode").text((vData.PeopleContactInfo.Rows[0].TelCo + ' - ' + vData.PeopleContactInfo.Rows[0].Prefix) || 'Telco Code');
                            $("#btnMobileTelCoPrefixCode").button('refresh');

                            $("#txtContactNoMobile").val(vData.PeopleContactInfo.Rows[0].ContactNo || '');
                            $("#selContactTypeMobile").val(vData.PeopleContactInfo.Rows[0].ContactType || '');
                            $('#selContactTypeMobile').selectmenu('refresh');

                            $("#btnPeopleContactMobileRemove").show();
                        }
                    }
                });

            };

            function getPeopleContactEmailList() {
                AjaxSend({
                    url: 'People/getPeopleContactEmailList',
                    success: function (vData) {
                        LoadPeopleContactEmailList(vData.PeopleContactList.Rows);
                    }
                });
            }

            function LoadPeopleContactEmailList(vRows) {
                var pLV = $('#lstPeopleContactEmail');
                var pItem;

                $("#divRecordCountEmail").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                $.mobile.changePage('#pagePeopleContactMobileEmail');
                                ShowPersonContactEmailInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].EmailAddress)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].EmailType)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function ShowPersonContactEmailInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearPeopleContactEmailFields();

                AjaxSend({
                    url: "People/getPeopleContactEmailInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.PeopleContactInfo.Rows.length > 0) {

                            $("#txtContactEmailID").val(vData.PeopleContactInfo.Rows[0].ID || '');

                            $("#txtContactEmail").val(vData.PeopleContactInfo.Rows[0].EmailAddress || '');
                            $("#selContactTypeEmail").val(vData.PeopleContactInfo.Rows[0].EmailType || '');
                            $('#selContactTypeEmail').selectmenu('refresh');

                            $("#btnPeopleContactEmailRemove").show();
                        }
                    }
                });

            };

            function ClearPeopleContactTelFields() {

                $("#txtContactTelID").val('');

                $("#txtTelCountryID").val('');
                $("#btnTelCountryCode").text('Country Code');
                $("#btnTelCountryCode").button('refresh');

                $("#txtTelAreaCodeID").val('');
                $("#btnTelAreaCode").text('Area Code');
                $("#btnTelAreaCode").button('refresh');

                $("#txtContactNoTel").val('');
                $("#txtContactExtension").val('');

                $("#selContactTypeTel").val('');
                $('#selContactTypeTel').selectmenu('refresh');

                $("#btnPeopleContactTelRemove").hide();

            }

            function ClearPeopleContactMobileFields() {
                $("#txtContactMobileID").val('');

                $("#txtMobileCountryID").val('');
                $("#btnMobileCountryCode").text('Country Code');
                $("#btnMobileCountryCode").button('refresh');

                $("#txtTelCoPrefixID").val('');
                $("#btnMobileTelCoPrefixCode").text('Telco Code');
                $("#btnMobileTelCoPrefixCode").button('refresh');

                $("#txtContactNoMobile").val('');
                $("#selContactTypeMobile").val('');
                $('#selContactTypeMobile').selectmenu('refresh');

                $("#btnPeopleContactMobileRemove").hide();

            }

            function ClearPeopleContactEmailFields() {
                $("#txtContactEmailID").val('');

                $("#txtContactEmail").val('');
                $("#selContactTypeEmail").val('');
                $('#selContactTypeEmail').selectmenu('refresh');

                $("#btnPeopleContactEmailRemove").hide();

            }

            function SavePeopleContactTel() {

                var pData = {};

                pData.PersonContactID = $("#txtContactTelID").val();
                pData.ContactCountryID = $("#txtTelCountryID").val();
                pData.ContactAreaCodeID = $("#txtTelAreaCodeID").val();
                pData.PersonContacNo = $("#txtContactNoTel").val();
                pData.PersonContacExtensionNo = $("#txtContactExtension").val();
                pData.PersoncontactType = $("#selContactTypeTel").val();

                AjaxSend({
                    url: "People/SavePeopleContactTel",
                    data: pData,
                    success: function (vData) {

                        if (vData.PeopleContactData == "Invalid Tel. No.") {
                            MessageBox("Please enter valid telephone number.",
                                        "Tel. No.",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                        }
                        else if (vData.PeopleContactData == "Invalid Extension or Local No.") {
                            MessageBox("Please enter valid telephone extension or local number.",
                                        "Tel. Extension or Local No.",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                        }
                        else {
                            $.mobile.changePage('#pagePeopleContactList');
                            getPeopleContactTelList();
                        }

                    }
                });

            };

            function SavePeopleContactMobile() {

                var pData = {};
                pData.PersonContactID = $("#txtContactMobileID").val();

                pData.PersonContactCountryID = $("#txtMobileCountryID").val();
                pData.PersonContactTelCoPrefixID = $("#txtTelCoPrefixID").val();
                pData.PersonContacNo = $("#txtContactNoMobile").val();
                pData.PersonContactType = $("#selContactTypeMobile").val();

                AjaxSend({
                    url: "People/SavePeopleContactMobile",
                    data: pData,
                    success: function (vData) {

                        if (vData.PeopleContactData == "Invalid Mobile No.") {
                            MessageBox("Please enter valid mobile number.",
                                        "Mobile No.",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                        }
                        else {
                            $.mobile.changePage('#pagePeopleContactList');
                            getPeopleContactMobileList();
                        }

                    }
                });

            };

            function SavePeopleContactEmail() {

                var pData = {};
                pData.PersonContactID = $("#txtContactEmailID").val();
                pData.PersonEmail = $("#txtContactEmail").val();
                pData.PersonEmailType = $("#selContactTypeEmail").val();

                AjaxSend({
                    url: "People/SavePeopleContactEmail",
                    data: pData,
                    success: function (vData) {

                        if (vData.PeopleContactData == "Invalid Email") {
                            MessageBox("Please enter valid email address.",
                                "Email Address",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                        }
                        else {
                            $.mobile.changePage('#pagePeopleContactList');
                            getPeopleContactEmailList();
                        }

                    }
                });

            };

            function RemovePeopleContactTel() {

                var pData = {};
                pData.PersonContactID = $("#txtContactTelID").val();

                AjaxSend({
                    url: "People/RemovePeopleContactTel",
                    data: pData,
                    success: function (vData) {

                        $.mobile.changePage('#pagePeopleContactList');
                        getPeopleContactTelList();

                    }
                });

            };

            function RemovePeopleContactMobile() {

                var pData = {};
                pData.PersonContactID = $("#txtContactMobileID").val();

                AjaxSend({
                    url: "People/RemovePeopleContactMobile",
                    data: pData,
                    success: function (vData) {

                        $.mobile.changePage('#pagePeopleContactList');
                        getPeopleContactMobileList();

                    }
                });

            };

            function RemovePeopleContactEmail() {

                var pData = {};
                pData.PersonContactID = $("#txtContactEmailID").val();

                AjaxSend({
                    url: "People/RemovePeopleContactEmail",
                    data: pData,
                    success: function (vData) {

                        $.mobile.changePage('#pagePeopleContactList');
                        getPeopleContactEmailList();

                    }
                });

            };

        });
    });
