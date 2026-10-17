require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var isIE = false || !!document.documentMode;
            var NameOfBtnForLibrary;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;

                if (!window.IsPageLoaded) {
                    $('body').scrollTop(0);
                    getOrgContactTelList();

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            window.OrgContact = {
                GetOrgContact: function () {
                    $('body').scrollTop(0);
                    getOrgContactTelList();
                }
            }

            $('body').on('click', '#btnOrgContactTelAdd', function (e) {
                e.preventDefault();
                $('body').scrollTop(0);
                //$.mobile.changePage('#pageOrgContactTelInfo');
                $('#pageContactList').hide();
                $('#pageOrgContactTelInfo').show();
                $('#pageOrgContactFaxInfo').hide();
                $('#pageOrgContactMobileInfo').hide();
                $('#pageOrgContactEmail').hide();
                $('#pageOrgContactWebsite').hide();
                ClearOrgContactTelFields();
            })
            .on('click', '#btnOrgTelCountryCode', function (e) {
                NameOfBtnForLibrary = 'OrgTel';
                SearchCountryPopUp();
                //                Browser.LibraryCountries.Click = function (e) {

                //                    $('#txtOrgTelCountryID').val(e.ID)
                //                    $('#btnOrgTelCountryCode').text(e.Code + " - " + e.CallingCode)
                //                    ;//.button('refresh');

                //                };
                //                Browser.LibraryCountries.Show();
            })
            .on('click', '#btnOrgTelAreaCode', function (e) {
                NameOfBtnForLibrary = 'OrgTel';
                SearchAreaPopUp();
                //                Browser.LibraryAreaCodes.Click = function (e) {

                //                    $('#txtOrgTelAreaCodeID').val(e.ID)
                //                    $('#btnOrgTelAreaCode').text(e.Area + ' - ' + e.CallingCode)
                //                    ;//.button('refresh');

                //                };
                //                Browser.LibraryAreaCodes.Show();
            })
            .on('keydown', '#txtOrgContactNoTel', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtOrgContactExtensionTel', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('click', '#btnOrgContactFaxAdd', function (e) {
                e.preventDefault();
                $('body').scrollTop(0);
                //$.mobile.changePage('#pageOrgContactFaxInfo');
                $('#pageContactList').hide();
                $('#pageOrgContactTelInfo').hide();
                $('#pageOrgContactFaxInfo').show();
                $('#pageOrgContactMobileInfo').hide();
                $('#pageOrgContactEmail').hide();
                $('#pageOrgContactWebsite').hide();
                ClearOrgContactFaxFields();
            })
            .on('click', '#btnOrgFaxCountryCode', function (e) {
                NameOfBtnForLibrary = 'OrgFax';
                SearchCountryPopUp();
//                Browser.LibraryCountries.Click = function (e) {

//                    $('#txtOrgFaxCountryID').val(e.ID)
//                    $('#btnOrgFaxCountryCode').text(e.Code + " - " + e.CallingCode)
//                    ;//.button('refresh');

//                };
//                Browser.LibraryCountries.Show();
            })
            .on('click', '#btnOrgFaxAreaCode', function (e) {
                NameOfBtnForLibrary = 'OrgFax';
                SearchAreaPopUp();
//                Browser.LibraryAreaCodes.Click = function (e) {

//                    $('#txtOrgFaxAreaCodeID').val(e.ID)
//                    $('#btnOrgFaxAreaCode').text(e.Area + ' - ' + e.CallingCode)
//                    ;//.button('refresh');

//                };
//                Browser.LibraryAreaCodes.Show();
            })
            .on('keydown', '#txtOrgContactNoFax', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtOrgContactExtensionFax', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('click', '#btnOrgContactMobAdd', function (e) {
                e.preventDefault();
                $('body').scrollTop(0);
                //$.mobile.changePage('#pageOrgContactMobileInfo');
                $('#pageContactList').hide();
                $('#pageOrgContactTelInfo').hide();
                $('#pageOrgContactFaxInfo').hide();
                $('#pageOrgContactMobileInfo').show();
                $('#pageOrgContactEmail').hide();
                $('#pageOrgContactWebsite').hide();
                ClearOrgContactMobileFields();
            })
            .on('click', '#btnOrgMobileCountryCode', function (e) {
                NameOfBtnForLibrary = 'OrgMobile';
                SearchCountryPopUp();
//                Browser.LibraryCountries.Click = function (e) {

//                    $('#txtOrgMobileCountryID').val(e.ID)
//                    $('#btnOrgMobileCountryCode').text(e.Code + " - " + e.CallingCode)
//                    ;//.button('refresh');

//                };
//                Browser.LibraryCountries.Show();
            })
            .on('click', '#btnOrgMobileTelCoPrefixCode', function (e) {
                NameOfBtnForLibrary = 'OrgMobile';
                SearchMobilePrefixCodePopUp();
//                Browser.LibraryTelcoPrefixes.Click = function (e) {

//                    $('#txtOrgTelCoPrefixID').val(e.ID)
//                    $('#btnOrgMobileTelCoPrefixCode').text(e.Prefix)
//                    ;//.button('refresh');

//                };
//                Browser.LibraryTelcoPrefixes.Show();
            })
            .on('keydown', '#txtOrgContactNoMobile', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('click', '#btnOrgContactEmailAdd', function (e) {
                e.preventDefault();
                $('body').scrollTop(0);
                //$.mobile.changePage('#pageOrgContactEmail');
                $('#pageContactList').hide();
                $('#pageOrgContactTelInfo').hide();
                $('#pageOrgContactFaxInfo').hide();
                $('#pageOrgContactMobileInfo').hide();
                $('#pageOrgContactEmail').show();
                $('#pageOrgContactWebsite').hide();
                ClearOrgContactEmailFields();
            })
            .on('click', '#btnOrgContactWebsiteAdd', function (e) {
                e.preventDefault();
                $('body').scrollTop(0);
                //$.mobile.changePage('#pageOrgContactWebsite');
                $('#pageContactList').hide();
                $('#pageOrgContactTelInfo').hide();
                $('#pageOrgContactFaxInfo').hide();
                $('#pageOrgContactMobileInfo').hide();
                $('#pageOrgContactEmail').hide();
                $('#pageOrgContactWebsite').show();
                ClearOrgContactWebsiteFields();
            })
            .on('click', '#btnOrgContactTelSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgTelCountryID").val()) == "") {
                    NewMessageBox('Please select country code.');
                    return;
                }

                if ($.trim($("#txtOrgTelAreaCodeID").val()) == "") {
                    NewMessageBox('Please select area code.');
                    return;
                }

                if ($.trim($("#txtOrgContactNoTel").val()) == "") {
                    NewMessageBox('Please enter 7 digit telephone number.');
                    return;
                }

                if ($.trim($("#txtOrgContactNoTel").val()).length != 7) {
                    NewMessageBox('Please enter 7 digit telephone number.');
                    return;
                }

                if (!$.isNumeric($("#txtOrgContactNoTel").val())) {
                    NewMessageBox('Please enter valid telephone number.');
                    return;
                }

                if ($.trim($("#txtOrgContactExtensionTel").val()).length > 0) {
                    if (!$.isNumeric($("#txtOrgContactExtensionTel").val())) {
                        NewMessageBox('Please enter valid extension or local number.');
                        return;
                    }
                }

                SaveOrgContactTel();
            })
            .on('click', '#btnOrgContactTelRemove', function (e) {
                e.preventDefault();
                NewMessageBox('Remove this record?', 'Confirmation Message', [
                    {
                        Text: 'Yes', Click: function () {
                            RemoveOrgContactTel();
                        }
                    }, { Text: 'No' }
                ]);
            })
            .on('click', '#btnOrgContactTelCancel', function (e) {
                e.preventDefault();
                $('#pageContactList').show();
                $('#pageOrgContactTelInfo').hide();
                $('#pageOrgContactFaxInfo').hide();
                $('#pageOrgContactMobileInfo').hide();
                $('#pageOrgContactEmail').hide();
                $('#pageOrgContactWebsite').hide();
            })
            .on('click', '#btnOrgContactFaxSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgFaxCountryID").val()) == "") {
                    NewMessageBox('Please select country code.');
                    return;
                }

                if ($.trim($("#txtOrgFaxAreaCodeID").val()) == "") {
                    NewMessageBox('Please select area code.');
                    return;
                }

                if ($.trim($("#txtOrgContactNoFax").val()) == "") {
                    NewMessageBox('Please enter 7 digit telephone number.');
                    return;
                }

                if ($.trim($("#txtOrgContactNoFax").val()).length != 7) {
                    NewMessageBox('Please enter 7 digit telephone number.');
                    return;
                }

                if (!$.isNumeric($("#txtOrgContactNoFax").val())) {
                    NewMessageBox('Please enter valid telephone number.');
                    return;
                }

                if ($.trim($("#txtOrgContactExtensionFax").val()).length > 0) {
                    if (!$.isNumeric($("#txtOrgContactExtensionFax").val())) {
                        NewMessageBox('Please enter valid extension or local number.');
                        return;
                    }
                }

                SaveOrgContactFax();
            })
            .on('click', '#btnOrgContactFaxRemove', function (e) {
                e.preventDefault();
                NewMessageBox('Remove this record?', 'Confirmation Message', [
                    {
                        Text: 'Yes', Click: function () {
                            RemoveOrgContactFax();
                        }
                    }, { Text: 'No' }
                ]);
            })
            .on('click', '#btnOrgContactFaxCancel', function (e) {
                e.preventDefault();
                $('#pageContactList').show();
                $('#pageOrgContactTelInfo').hide();
                $('#pageOrgContactFaxInfo').hide();
                $('#pageOrgContactMobileInfo').hide();
                $('#pageOrgContactEmail').hide();
                $('#pageOrgContactWebsite').hide();
            })
            .on('click', '#btnOrgContactMobileSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgMobileCountryID").val()) == "") {
                    NewMessageBox('Please select country code.');
                    return;
                }

                if ($.trim($("#txtOrgTelCoPrefixID").val()) == "") {
                    NewMessageBox('Please select prefix number.');
                    return;
                }

                if ($.trim($("#txtOrgContactNoMobile").val()) == "") {
                    NewMessageBox('Please enter 7 digit mobile number.');
                    return;
                }

                if ($.trim($("#txtOrgContactNoMobile").val()).length != 7) {
                    NewMessageBox('Please enter 7 digit mobile number.');
                    return;
                }

                if (!$.isNumeric($("#txtOrgContactNoMobile").val())) {
                    NewMessageBox('Please enter valid mobile number.');
                    return;
                }

                SaveOrgContactMobile();
            })
            .on('click', '#btnOrgContactMobileRemove', function (e) {
                e.preventDefault();
                NewMessageBox('Remove this record?', 'Confirmation Message', [
                    {
                        Text: 'Yes', Click: function () {
                            RemoveOrgContactMobile();
                        }
                    }, { Text: 'No' }
                ]);
            })
            .on('click', '#btnOrgContactMobileCancel', function (e) {
                e.preventDefault();
                $('#pageContactList').show();
                $('#pageOrgContactTelInfo').hide();
                $('#pageOrgContactFaxInfo').hide();
                $('#pageOrgContactMobileInfo').hide();
                $('#pageOrgContactEmail').hide();
                $('#pageOrgContactWebsite').hide();
            })
            .on('click', '#btnOrgContactEmailSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgContactEmail").val()) == "") {
                    NewMessageBox('Please enter email address.');
                    return;
                }

                if (IsEmail($.trim($("#txtOrgContactEmail").val())) == false) {
                    NewMessageBox('Please enter valid email address.');
                    return;
                }

                SaveOrgContactEmail();
            })
            .on('click', '#btnOrgContactEmailRemove', function (e) {
                e.preventDefault();
                NewMessageBox('Remove this record?', 'Confirmation Message', [
                    {
                        Text: 'Yes', Click: function () {
                            RemoveOrgContactEmail();
                        }
                    }, { Text: 'No' }
                ]);
            })
            .on('click', '#btnOrgContactEmailCancel', function (e) {
                e.preventDefault();
                $('#pageContactList').show();
                $('#pageOrgContactTelInfo').hide();
                $('#pageOrgContactFaxInfo').hide();
                $('#pageOrgContactMobileInfo').hide();
                $('#pageOrgContactEmail').hide();
                $('#pageOrgContactWebsite').hide();
            })
            .on('click', '#btnOrgContactWebSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgContactWebsite").val()) == "") {
                    NewMessageBox('Please enter website.');
                    return;
                }

                if (IsUrl($.trim($("#txtOrgContactWebsite").val())) == false) {
                    NewMessageBox('Please enter valid website.');
                    return;
                }

                SaveOrgContactWebsite();
            })
            .on('click', '#btnOrgContactWebRemove', function (e) {
                e.preventDefault();
                NewMessageBox('Remove this record?', 'Confirmation Message', [
                    {
                        Text: 'Yes', Click: function () {
                            RemoveOrgContactWebsite();
                        }
                    }, { Text: 'No' }
                ]);
            })
            .on('click', '#btnOrgContactWebCancel', function (e) {
                e.preventDefault();
                $('#pageContactList').show();
                $('#pageOrgContactTelInfo').hide();
                $('#pageOrgContactFaxInfo').hide();
                $('#pageOrgContactMobileInfo').hide();
                $('#pageOrgContactEmail').hide();
                $('#pageOrgContactWebsite').hide();
            })
            .on('click', '#btnOrgContactDone', function (e) {
                e.preventDefault();

//                if (window.pageSourcePage != undefined) {
//                    $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
//                }
//                else {
//                    //    ChangePage('/Organization/OrganizationSummary');
//                    window.location = MergeUrlParameters({
//                        OrgID: sessionStorage.OrgID,
//                        IsMobile: CBool(IsMobile) ? 1 : 0
//                    }, '/Organization/OrganizationSummary');
                //                }
                $('body').scrollTop(0);
                $('#UniversalDiv').html('');
                LibraryTable.OrganizationPage();
            })
            .on('keypress', '#txtSearchKeyword', function (e) {
                if (e.keyCode == 13) {
                    if ($('#TitleLibrary').text() == "CITY") {
                        SearchCityPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "COUNTRY") {
                        SearchCountryPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "AREA CODE") {
                        SearchAreaPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "MOBILE PREFIX CODE") {
                        SearchMobilePrefixCodePopUp($(this).val());
                    }
                }
            })

            function SearchMobilePrefixCodePopUp(s) {
                BrowsePopup('LibraryTelcoPrefixes', 'Select Telco', [{
                    AutoSearch: true,
                    DisplayText: [{
                        Column: function (e) {
                            return e.TelCo + ' - ' + e.Prefix;
                        }
                    }], Click: function (e) {
                        $('#txtOrgTelCoPrefixID').val(e.ID);
                        $('#btnOrgMobileTelCoPrefixCode').text(e.TelCo + ' - ' + e.Prefix);//.button('refresh');
                    }
                }])
            }

            function SearchAreaPopUp(s) {
                BrowsePopup('LibraryAreaCodes', 'Select Area Code', [{
                    AutoSearch: true,
                    DisplayText: [{
                        Column: 'Area'
                    }], Click: function (e) {
                        if (NameOfBtnForLibrary == "OrgTel") {
                            $('#txtOrgTelAreaCodeID').val(e.ID);
                            $('#btnOrgTelAreaCode').text(e.Area + ' - ' + e.CallingCode);//.button('refresh');
                        }
                        else if (NameOfBtnForLibrary == "OrgFax") {
                            $('#txtOrgFaxAreaCodeID').val(e.ID);
                            $('#btnOrgFaxAreaCode').text(e.Area + ' - ' + e.CallingCode);//.button('refresh');
                        }
                    }
                }])
            }

            function SearchCountryPopUp(s) {
                BrowsePopup('LibraryCountries', 'Select Country', [{
                    AutoSearch: true,
                    DisplayText: [{
                        Column: 'Name'
                    }], Click: function (e) {
                        if (NameOfBtnForLibrary == "OrgTel") {
                            $('#txtOrgTelCountryID').val(e.ID);
                            $('#btnOrgTelCountryCode').text(e.Code + ' - ' + e.CallingCode);//.button('refresh');
                        }
                        else if (NameOfBtnForLibrary == "OrgFax") {
                            $('#txtOrgFaxCountryID').val(e.ID);
                            $('#btnOrgFaxCountryCode').text(e.Code + ' - ' + e.CallingCode);//.button('refresh');
                        }
                        else if (NameOfBtnForLibrary == "OrgMobile") {
                            $('#txtOrgMobileCountryID').val(e.ID);
                            $('#btnOrgMobileCountryCode').text(e.Code + ' - ' + e.CallingCode);//.button('refresh');
                        }
                    }
                }])
            }

            function SearchCityPopUp(s) {
                BrowsePopup('LibraryCities', 'Select City', [{
                    AutoSearch: true,
                    DisplayText: [{
                        Column: 'Name'
                    }], Click: function (e) {
                        if (NameOfBtnForLibrary == "OrgOfficeAddress") {
                            $('#txtOrgOfficeAddressCityID').val(e.ID);
                            $('#btnOrgOfficeAddressCity').text(e.Name);//.button('refresh');
                        }
                        else if (NameOfBtnForLibrary == "OrgBusinessAddress") {
                            $('#txtOrgBusinessAddressCityID').val(e.ID);
                            $('#btnOrgBusinessAddressCity').text(e.Name);//.button('refresh');
                        }
                        else if (NameOfBtnForLibrary == "PersonAddress") {
                            $('#txtOrgPersonAddressCityID').val(e.ID);
                            $('#btnOrgPersonAddressCity').text(e.Name);//.button('refresh');
                        }
                    }
                }])
            }

            function getOrgContactTelList() {
                AjaxSend({
                    url: 'Organization/getOrgContactTelList',
                    success: function (vData) {
                        LoadOrgContactTelList(vData.OrganizationData.Rows);
                        getOrgContactFaxList();
                    }
                });
            }

            function LoadOrgContactTelList(vRows) {
                var pLV = $('#lstOrgContactTel');
                var pItem;

                $("#divOrgRecordCountTel").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageOrgContactTelInfo');
                                $('#pageContactList').hide();
                                $('#pageOrgContactTelInfo').show();
                                $('#pageOrgContactFaxInfo').hide();
                                $('#pageOrgContactMobileInfo').hide();
                                $('#pageOrgContactEmail').hide();
                                $('#pageOrgContactWebsite').hide();
                                ShowOrgContactTelInfo($(this).attr('ID'));
                            })
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text("+" + vRows[p].CountryCode + "." + vRows[p].AreaCode + '.' + vRows[p].ContactNo + ((vRows[p].ExtensionNo || '') == '' ? '' : ", Ext. " + vRows[p].ExtensionNo))
                            )
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Area + ", " + vRows[p].CountryName)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            };

            function ShowOrgContactTelInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgContactTelFields();

                AjaxSend({
                    url: "Organization/getOrgContactTelInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData.Rows.length > 0) {

                            $("#txtOrgContactTelID").val(vData.OrganizationData.Rows[0].ID || '');

                            $("#txtOrgTelCountryID").val(vData.OrganizationData.Rows[0].CountryID || '');
                            $("#btnOrgTelCountryCode").text((vData.OrganizationData.Rows[0].Country + ' - ' + vData.OrganizationData.Rows[0].CountryCode) || 'Country Code');
                            $("#btnOrgTelCountryCode");//.button('refresh');

                            $("#txtOrgTelAreaCodeID").val(vData.OrganizationData.Rows[0].AreaCodeID || '');
                            $("#btnOrgTelAreaCode").text((vData.OrganizationData.Rows[0].Area + ' - ' + vData.OrganizationData.Rows[0].AreaCode) || 'Area Code');
                            $("#btnOrgTelAreaCode");//.button('refresh');

                            $("#txtOrgContactNoTel").val(vData.OrganizationData.Rows[0].ContactNo || '');
                            $("#txtOrgContactExtensionTel").val(vData.OrganizationData.Rows[0].ExtensionNo || '');

                            $("#btnOrgContactTelRemove").show();
                        }
                    }
                });

            };

            function SaveOrgContactTel() {

                var pData = {};

                pData.OrgContactID = $("#txtOrgContactTelID").val();
                pData.ContactCountryID = $("#txtOrgTelCountryID").val();
                pData.ContactAreaCodeID = $("#txtOrgTelAreaCodeID").val();
                pData.ContacNo = $("#txtOrgContactNoTel").val();
                pData.ContacExtensionNo = $("#txtOrgContactExtensionTel").val();

                AjaxSend({
                    url: "Organization/SaveOrgContactTel",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData == "Invalid Tel. No.") {
                            NewMessageBox('Please enter valid telephone number.');
                        }
                        else if (vData.OrganizationData == "Invalid Extension or Local No.") {
                            NewMessageBox('Please enter valid telephone extension or local number.');
                        }
                        else {
                            // ChangePage('/Organization/OrganizationContact');
                            //window.location = MergeUrlParameters({
                            //    OrgID: sessionStorage.OrgID,
                            //    IsMobile: CBool(IsMobile) ? 1 : 0
                            //}, '/Organization/OrganizationContact');
                            $('#pageContactList').show();
                            $('#pageOrgContactTelInfo').hide();
                            $('#pageOrgContactFaxInfo').hide();
                            $('#pageOrgContactMobileInfo').hide();
                            $('#pageOrgContactEmail').hide();
                            $('#pageOrgContactWebsite').hide();
                            getOrgContactTelList();
                        }

                    }
                });

            };

            function RemoveOrgContactTel() {

                var pData = {};
                pData.OrgContactID = $("#txtOrgContactTelID").val();

                AjaxSend({
                    url: "Organization/RemoveOrgContactTel",
                    data: pData,
                    success: function (vData) {

                        //   ChangePage('/Organization/OrganizationContact');
                        window.location = MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            IsMobile: CBool(IsMobile) ? 1 : 0
                        }, '/Organization/OrganizationContact');
                        getOrgContactTelList();
                    }
                });

            };

            function getOrgContactFaxList() {
                AjaxSend({
                    url: 'Organization/getOrgContactFaxList',
                    success: function (vData) {
                        LoadOrgContactFaxList(vData.OrganizationData.Rows);
                        getOrgContactMobileList();
                    }
                });
            }

            function LoadOrgContactFaxList(vRows) {
                var pLV = $('#lstOrgContactFax');
                var pItem;

                $("#divOrgRecordCountFax").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageOrgContactFaxInfo');
                                $('#pageContactList').hide();
                                $('#pageOrgContactTelInfo').hide();
                                $('#pageOrgContactFaxInfo').show();
                                $('#pageOrgContactMobileInfo').hide();
                                $('#pageOrgContactEmail').hide();
                                $('#pageOrgContactWebsite').hide();
                                ShowOrgContactFaxInfo($(this).attr('ID'));
                            })
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text("+" + vRows[p].CountryCode + "." + vRows[p].AreaCode + '.' + vRows[p].ContactNo + ((vRows[p].ExtensionNo || '') == '' ? '' : ", Ext. " + vRows[p].ExtensionNo))
                            )
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Area + ", " + vRows[p].CountryName)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            };

            function ShowOrgContactFaxInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgContactFaxFields();

                AjaxSend({
                    url: "Organization/getOrgContactFaxInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData.Rows.length > 0) {

                            $("#txtOrgContactFaxID").val(vData.OrganizationData.Rows[0].ID || '');

                            $("#txtOrgFaxCountryID").val(vData.OrganizationData.Rows[0].CountryID || '');
                            $("#btnOrgFaxCountryCode").text((vData.OrganizationData.Rows[0].Country + ' - ' + vData.OrganizationData.Rows[0].CountryCode) || 'Country Code');
                            $("#btnOrgFaxCountryCode");//.button('refresh');

                            $("#txtOrgFaxAreaCodeID").val(vData.OrganizationData.Rows[0].AreaCodeID || '');
                            $("#btnOrgFaxAreaCode").text((vData.OrganizationData.Rows[0].Area + ' - ' + vData.OrganizationData.Rows[0].AreaCode) || 'Area Code');
                            $("#btnOrgFaxAreaCode");//.button('refresh');

                            $("#txtOrgContactNoFax").val(vData.OrganizationData.Rows[0].ContactNo || '');
                            $("#txtOrgContactExtensionFax").val(vData.OrganizationData.Rows[0].ExtensionNo || '');

                            $("#btnOrgContactFaxRemove").show();
                        }
                    }
                });

            };

            function SaveOrgContactFax() {

                var pData = {};

                pData.OrgContactID = $("#txtOrgContactFaxID").val();
                pData.ContactCountryID = $("#txtOrgFaxCountryID").val();
                pData.ContactAreaCodeID = $("#txtOrgFaxAreaCodeID").val();
                pData.ContacNo = $("#txtOrgContactNoFax").val();
                pData.ContacExtensionNo = $("#txtOrgContactExtensionFax").val();

                AjaxSend({
                    url: "Organization/SaveOrgContactFax",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData == "Invalid Tel. No.") {
                            NewMessageBox('Please enter valid telephone number.');
                        }
                        else if (vData.OrganizationData == "Invalid Extension or Local No.") {
                            NewMessageBox('Please enter valid telephone extension or local number.');
                        }
                        else {
                            // ChangePage('/Organization/OrganizationContact');
                            //                            window.location = MergeUrlParameters({
                            //                                OrgID: sessionStorage.OrgID,
                            //                                IsMobile: CBool(IsMobile) ? 1 : 0
                            //                            }, '/Organization/OrganizationContact');
                            $('#pageContactList').show();
                            $('#pageOrgContactTelInfo').hide();
                            $('#pageOrgContactFaxInfo').hide();
                            $('#pageOrgContactMobileInfo').hide();
                            $('#pageOrgContactEmail').hide();
                            $('#pageOrgContactWebsite').hide();
                            getOrgContactFaxList();
                        }

                    }
                });

            };

            function RemoveOrgContactFax() {

                var pData = {};
                pData.OrgContactID = $("#txtOrgContactFaxID").val();

                AjaxSend({
                    url: "Organization/RemoveOrgContactFax",
                    data: pData,
                    success: function (vData) {

                        //  ChangePage('/Organization/OrganizationContact');
                        window.location = MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            IsMobile: CBool(IsMobile) ? 1 : 0
                        }, '/Organization/OrganizationContact');
                        getOrgContactFaxList();
                    }
                });

            };

            function getOrgContactMobileList() {
                AjaxSend({
                    url: 'Organization/getOrgContactMobileList',
                    success: function (vData) {
                        LoadOrgContactMobileList(vData.OrganizationData.Rows);
                        getOrgContactEmailList();
                    }
                });
            }

            function LoadOrgContactMobileList(vRows) {
                var pLV = $('#lstOrgContactMob');
                var pItem;

                $("#divOrgRecordCountMob").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageOrgContactMobileInfo');
                                $('#pageContactList').hide();
                                $('#pageOrgContactTelInfo').hide();
                                $('#pageOrgContactFaxInfo').hide();
                                $('#pageOrgContactMobileInfo').show();
                                $('#pageOrgContactEmail').hide();
                                $('#pageOrgContactWebsite').hide();
                                ShowOrgContactMobileInfo($(this).attr('ID'));
                            })
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text("+" + vRows[p].CountryCode + "." + vRows[p].Prefix + "." + vRows[p].ContactNo)
                            )
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].TelCo + " (" + vRows[p].CountryName + ")")
                            )
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].ContactType)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            };

            function ShowOrgContactMobileInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgContactMobileFields();

                AjaxSend({
                    url: "Organization/getOrgContactMobileInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData.Rows.length > 0) {

                            $("#txtOrgContactMobileID").val(vData.OrganizationData.Rows[0].ID || '');

                            $("#txtOrgMobileCountryID").val(vData.OrganizationData.Rows[0].CountryID || '');
                            $("#btnOrgMobileCountryCode").text((vData.OrganizationData.Rows[0].Country + ' - ' + vData.OrganizationData.Rows[0].CountryCode) || 'Country Code');
                            $("#btnOrgMobileCountryCode");//.button('refresh');

                            $("#txtOrgTelCoPrefixID").val(vData.OrganizationData.Rows[0].TelCoPrefixID || '');
                            $("#btnOrgMobileTelCoPrefixCode").text((vData.OrganizationData.Rows[0].TelCo + ' - ' + vData.OrganizationData.Rows[0].Prefix) || 'Telco Code');
                            $("#btnOrgMobileTelCoPrefixCode");//.button('refresh');

                            $("#txtOrgContactNoMobile").val(vData.OrganizationData.Rows[0].ContactNo || '');

                            $("#btnOrgContactMobileRemove").show();
                        }
                    }
                });

            };

            function SaveOrgContactMobile() {

                var pData = {};
                pData.OrgContactID = $("#txtOrgContactMobileID").val();

                pData.ContactCountryID = $("#txtOrgMobileCountryID").val();
                pData.ContactTelCoPrefixID = $("#txtOrgTelCoPrefixID").val();
                pData.ContactNo = $("#txtOrgContactNoMobile").val();

                AjaxSend({
                    url: "Organization/SaveOrgContactMobile",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData == "Invalid Mobile No.") {
                            NewMessageBox('Please enter valid mobile number.');
                        }
                        else {
                            //    ChangePage('/Organization/OrganizationContact');
                            //                            window.location = MergeUrlParameters({
                            //                                OrgID: sessionStorage.OrgID,
                            //                                IsMobile: CBool(IsMobile) ? 1 : 0
                            //                            }, '/Organization/OrganizationContact');
                            $('#pageContactList').show();
                            $('#pageOrgContactTelInfo').hide();
                            $('#pageOrgContactFaxInfo').hide();
                            $('#pageOrgContactMobileInfo').hide();
                            $('#pageOrgContactEmail').hide();
                            $('#pageOrgContactWebsite').hide();
                            getOrgContactMobileList();
                        }

                    }
                });

            };

            function RemoveOrgContactMobile() {

                var pData = {};
                pData.OrgContactID = $("#txtOrgContactMobileID").val();

                AjaxSend({
                    url: "Organization/RemoveOrgContactMobile",
                    data: pData,
                    success: function (vData) {

                        // ChangePage('/Organization/OrganizationContact');
                        window.location = MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            IsMobile: CBool(IsMobile) ? 1 : 0
                        }, '/Organization/OrganizationContact');
                        getOrgContactMobileList();
                    }
                });

            };

            function getOrgContactEmailList() {
                AjaxSend({
                    url: 'Organization/getOrgContactEmailList',
                    success: function (vData) {
                        LoadOrgContactEmailList(vData.OrganizationData.Rows);
                        getOrgContactWebsiteList();
                    }
                });
            }

            function LoadOrgContactEmailList(vRows) {
                var pLV = $('#lstOrgContactEmail');
                var pItem;

                $("#divOrgRecordCountEmail").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageOrgContactEmail');
                                $('#pageContactList').hide();
                                $('#pageOrgContactTelInfo').hide();
                                $('#pageOrgContactFaxInfo').hide();
                                $('#pageOrgContactMobileInfo').hide();
                                $('#pageOrgContactEmail').show();
                                $('#pageOrgContactWebsite').hide();
                                ShowOrgContactEmailInfo($(this).attr('ID'));
                            })
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].EmailAddress)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            };

            function ShowOrgContactEmailInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgContactEmailFields();

                AjaxSend({
                    url: "Organization/getOrgContactEmailInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData.Rows.length > 0) {

                            $("#txtOrgContactEmailID").val(vData.OrganizationData.Rows[0].ID || '');
                            $("#txtOrgContactEmail").val(vData.OrganizationData.Rows[0].EmailAddress || '');

                            $("#btnOrgContactEmailRemove").show();
                        }
                    }
                });

            };

            function SaveOrgContactEmail() {

                var pData = {};
                pData.OrgContactID = $("#txtOrgContactEmailID").val();
                pData.ContactEmail = $("#txtOrgContactEmail").val();

                AjaxSend({
                    url: "Organization/SaveOrgContactEmail",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData == "Invalid Email") {
                            NewMessageBox('Please enter valid email address.');
                        }
                        else {
                            //  ChangePage('/Organization/OrganizationContact');
                            //                            window.location = MergeUrlParameters({
                            //                                OrgID: sessionStorage.OrgID,
                            //                                IsMobile: CBool(IsMobile) ? 1 : 0
                            //                            }, '/Organization/OrganizationContact');
                            $('#pageContactList').show();
                            $('#pageOrgContactTelInfo').hide();
                            $('#pageOrgContactFaxInfo').hide();
                            $('#pageOrgContactMobileInfo').hide();
                            $('#pageOrgContactEmail').hide();
                            $('#pageOrgContactWebsite').hide();
                            getOrgContactEmailList();
                        }

                    }
                });

            };


            function RemoveOrgContactEmail() {

                var pData = {};
                pData.OrgContactID = $("#txtOrgContactEmailID").val();

                AjaxSend({
                    url: "Organization/RemoveOrgContactEmail",
                    data: pData,
                    success: function (vData) {

                        //ChangePage('/Organization/OrganizationContact');
                        window.location = MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            IsMobile: CBool(IsMobile) ? 1 : 0
                        }, '/Organization/OrganizationContact');
                        getOrgContactEmailList();
                    }
                });

            };

            function getOrgContactWebsiteList() {
                AjaxSend({
                    url: 'Organization/getOrgContactWebsiteList',
                    success: function (vData) {
                        LoadOrgContactWebsiteList(vData.OrganizationData.Rows);
                    }
                });
            }

            function LoadOrgContactWebsiteList(vRows) {
                var pLV = $('#lstOrgContactWebsite');
                var pItem;

                $("#divOrgRecordCountWebsite").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageOrgContactWebsite');
                                $('#pageContactList').hide();
                                $('#pageOrgContactTelInfo').hide();
                                $('#pageOrgContactFaxInfo').hide();
                                $('#pageOrgContactMobileInfo').hide();
                                $('#pageOrgContactEmail').hide();
                                $('#pageOrgContactWebsite').show();
                                ShowOrgContactWebsiteInfo($(this).attr('ID'));
                            })
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Website)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            };

            function ShowOrgContactWebsiteInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgContactWebsiteFields();

                AjaxSend({
                    url: "Organization/getOrgContactWebsiteInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData.Rows.length > 0) {

                            $("#txtOrgContactWebsiteID").val(vData.OrganizationData.Rows[0].ID || '');
                            $("#txtOrgContactWebsite").val(vData.OrganizationData.Rows[0].Website || '');

                            $("#btnOrgContactWebRemove").show();
                        }
                    }
                });

            };

            function SaveOrgContactWebsite() {

                var pData = {};
                pData.OrgContactID = $("#txtOrgContactWebsiteID").val();
                pData.ContactWebsite = $("#txtOrgContactWebsite").val();

                AjaxSend({
                    url: "Organization/SaveOrgContactWebsite",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData == "Invalid Website") {
                            NewMessageBox('Please enter valid website.');
                        }
                        else {
                            //     ChangePage('/Organization/OrganizationContact');
                            //                            window.location = MergeUrlParameters({
                            //                                OrgID: sessionStorage.OrgID,
                            //                                IsMobile: CBool(IsMobile) ? 1 : 0
                            //                            }, '/Organization/OrganizationContact');
                            $('#pageContactList').show();
                            $('#pageOrgContactTelInfo').hide();
                            $('#pageOrgContactFaxInfo').hide();
                            $('#pageOrgContactMobileInfo').hide();
                            $('#pageOrgContactEmail').hide();
                            $('#pageOrgContactWebsite').hide();
                            getOrgContactWebsiteList();
                        }

                    }
                });

            };

            function RemoveOrgContactWebsite() {

                var pData = {};
                pData.OrgContactID = $("#txtOrgContactWebsiteID").val();

                AjaxSend({
                    url: "Organization/RemoveOrgContactWebsite",
                    data: pData,
                    success: function (vData) {

                        //   ChangePage('/Organization/OrganizationContact');
                        window.location = MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            IsMobile: CBool(IsMobile) ? 1 : 0
                        }, '/Organization/OrganizationContact');
                        getOrgContactWebsiteList();
                    }
                });

            };

            function ClearOrgContactTelFields() {

                $("#txtOrgContactTelID").val('');

                $("#txtOrgTelCountryID").val('');
                $("#btnOrgTelCountryCode").text('Country Code');
                $("#btnOrgTelCountryCode");//.button('refresh');

                $("#txtOrgTelAreaCodeID").val('');
                $("#btnOrgTelAreaCode").text('Area Code');
                $("#btnOrgTelAreaCode");//.button('refresh');

                $("#txtOrgContactNoTel").val('');
                $("#txtOrgContactExtensionTel").val('');

                $("#btnOrgContactTelRemove").hide();

            }

            function ClearOrgContactFaxFields() {

                $("#txtOrgContactFaxID").val('');

                $("#txtOrgFaxAreaCodeID").val('');
                $("#btnOrgFaxCountryCode").text('Country Code');
                $("#btnOrgFaxCountryCode");//.button('refresh');

                $("#txtOrgFaxAreaCodeID").val('');
                $("#btnOrgFaxAreaCode").text('Area Code');
                $("#btnOrgFaxAreaCode");//.button('refresh');

                $("#txtOrgContactNoFax").val('');
                $("#txtOrgContactExtensionFax").val('');

                $("#btnOrgContactFaxRemove").hide();

            }

            function ClearOrgContactMobileFields() {
                $("#txtOrgContactMobileID").val('');

                $("#txtOrgMobileCountryID").val('');
                $("#btnOrgMobileCountryCode").text('Country Code');
                $("#btnOrgMobileCountryCode");//.button('refresh');

                $("#txtOrgTelCoPrefixID").val('');
                $("#btnOrgMobileTelCoPrefixCode").text('Telco Code');
                $("#btnOrgMobileTelCoPrefixCode");//.button('refresh');

                $("#txtOrgContactNoMobile").val('');

                $("#btnOrgContactMobileRemove").hide();

            }

            function ClearOrgContactEmailFields() {
                $("#txtOrgContactEmailID").val('');
                $("#txtOrgContactEmail").val('');

                $("#btnOrgContactEmailRemove").hide();

            }

            function ClearOrgContactWebsiteFields() {
                $("#txtOrgContactWebsiteID").val('');
                $("#txtOrgContactWebsite").val('');

                $("#btnOrgContactWebRemove").hide();

            }

        });
    });
