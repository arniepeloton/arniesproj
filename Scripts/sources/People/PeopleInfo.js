$(function () {

    $('#btnNewPeopleInfo').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pagePeopleInfo');
        ShowPersonInfo('');

    });

    $('#btnEditPeopleInfo').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pageDialogPeopleSearch', { transition: 'pop', role: 'dialog' });

        $("#txtSearchPerson").val('');
        $("#txtSearchPerson").focus();

    });

    $('#btnPeopleInfoNext').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pagePeopleContacts');
        ShowPersonContact();

    });

    $('#btnPeopleInfoCancel').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pagePeopleMenu');

    });

    $('#btnContactPrev').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pagePeopleInfo');

    });

    $('#btnBrowsePresentAddressCity').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pageDialogCitySearch', { transition: 'pop', role: 'dialog' });

        $("#txtSearchCityFor").val('SelectPresentCity');
        $("#txtSearchCity").val('');
        $("#txtSearchCity").focus();

    });

    $('#btnBrowsePresentAddressCountry').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pageDialogCountrySearch', { transition: 'pop', role: 'dialog' });

        $("#txtSearchCountryFor").val('SelectPresentCountry');
        $("#txtSearchCountry").val('');
        $("#txtSearchCountry").focus();

    });

    $('#btnBrowsePermanentAddressCity').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pageDialogCitySearch', { transition: 'pop', role: 'dialog' });

        $("#txtSearchCityFor").val('SelectPermanentCity');
        $("#txtSearchCity").val('');
        $("#txtSearchCity").focus();

    });

    $('#btnBrowsePermanentAddressCountry').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pageDialogCountrySearch', { transition: 'pop', role: 'dialog' });

        $("#txtSearchCountryFor").val('SelectPermanentCountry');
        $("#txtSearchCountry").val('');
        $("#txtSearchCountry").focus();

    });

    $('#btnBrowsePlaceOfBirth').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pageDialogCitySearch', { transition: 'pop', role: 'dialog' });

        $("#txtSearchCityFor").val('SelectPlaceOfBirth');
        $("#txtSearchCity").val('');
        $("#txtSearchCity").focus();

    });


    $('#btnBrowseParentAddressCity').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pageDialogCitySearch', { transition: 'pop', role: 'dialog' });

        $("#txtSearchCityFor").val('SelectParentAddressCity');
        $("#txtSearchCity").val('');
        $("#txtSearchCity").focus();

    });

    $('#btnBrowseParentAddressCountry').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pageDialogCountrySearch', { transition: 'pop', role: 'dialog' });

        $("#txtSearchCountryFor").val('SelectParentAddressCountry');
        $("#txtSearchCountry").val('');
        $("#txtSearchCountry").focus();

    });

    $('#btnBrowseCitizenship').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pageDialogCitizenshipSearch', { transition: 'pop', role: 'dialog' });

        $("#txtSearchCitizenship").val('');
        $("#txtSearchCitizenship").focus();

    });

    $('#btnBrowseFatherOccupation').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pageDialogOccupationSearch', { transition: 'pop', role: 'dialog' });

        $("#txtSearchOccupationFor").val('SelectFatherOccupation');
        $("#txtSearchOccupation").val('');
        $("#txtSearchOccupation").focus();

    });

    $('#btnBrowseMotherOccupation').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pageDialogOccupationSearch', { transition: 'pop', role: 'dialog' });

        $("#txtSearchOccupationFor").val('SelectMotherOccupation');
        $("#txtSearchOccupation").val('');
        $("#txtSearchOccupation").focus();

    });

    $('#btnSearchPerson').click(function (e) {
        e.preventDefault();

        var pData = {};
        pData.Keywords = $("#txtSearchPerson").val();

        AjaxSend({
            url: 'People/getPeopleList',
            data: pData,
            success: function (vData) {

                $.mobile.changePage("#pageResultSearch");
                LoadPeopleList(vData.PeopleList.Rows);

            }
        });

    });

    $('#btnSearchCity').click(function (e) {
        e.preventDefault();

        var pData = {};
        pData.Keywords = $("#txtSearchCity").val();

        AjaxSend({
            url: 'People/getCityList',
            data: pData,
            success: function (vData) {

                $.mobile.changePage("#pageResultSearch");
                LoadCityList(vData.CityList.Rows);

            }
        });

    });

    $('#btnSearchCountry').click(function (e) {
        e.preventDefault();

        var pData = {};
        pData.Keywords = $("#txtSearchCountry").val();

        AjaxSend({
            url: 'People/getCountryList',
            data: pData,
            success: function (vData) {

                $.mobile.changePage("#pageResultSearch");
                LoadCountryList(vData.CountryList.Rows);

            }
        });

    });

    $('#btnSearchCitizenship').click(function (e) {
        e.preventDefault();

        var pData = {};
        pData.Keywords = $("#txtSearchCitizenship").val();

        AjaxSend({
            url: 'People/getCitizenshipList',
            data: pData,
            success: function (vData) {

                $.mobile.changePage("#pageResultSearch");
                LoadCitizenshipList(vData.CitizenshipList.Rows);

            }
        });

    });

    $('#btnSearchOccupation').click(function (e) {
        e.preventDefault();

        var pData = {};
        pData.Keywords = $("#txtSearchOccupation").val();

        AjaxSend({
            url: 'People/getOccupationList',
            data: pData,
            success: function (vData) {

                $.mobile.changePage("#pageResultSearch");
                LoadOccupationList(vData.OccupationList.Rows);

            }
        });

    });

    function LoadPeopleList(vRows) {
        var pLV = $('#lstResultList');
        var pItem;

        $("#divRecordCount").text(" Number of record(s) found: " + vRows.length);

        pLV.html('');
        for (var p = 0; p <= vRows.length - 1; p++) {
            pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .click(function (e) {
                            e.preventDefault();
                            $.mobile.changePage('#pagePeopleInfo');
                            ShowPersonInfo($(this).attr('ID'));
                        })
                        .append($('<h1>')
                            .text(vRows[p].PeopleName)
                        )
                    );
            pLV.append(pItem);
        };

        pLV.listview('refresh');
    };

    function LoadCityList(vRows) {
        var pLV = $('#lstResultList');
        var pItem;

        $("#divRecordCount").text(" Number of record(s) found: " + vRows.length);

        pLV.html('');
        for (var p = 0; p <= vRows.length - 1; p++) {
            pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .attr('City', vRows[p].City)
                        .click(function (e) {
                            e.preventDefault();

                            if ($("#txtSearchCityFor").val() == 'SelectPresentCity') {
                                SelectPresentCity($(this).attr('ID'), $(this).attr('City'));
                            } else if ($("#txtSearchCityFor").val() == 'SelectPermanentCity') {
                                SelectPermanentCity($(this).attr('ID'), $(this).attr('City'));
                            } else if ($("#txtSearchCityFor").val() == 'SelectPlaceOfBirth') {
                                SelectPlaceOfBirth($(this).attr('ID'), $(this).attr('City'));
                            } else if ($("#txtSearchCityFor").val() == 'SelectParentAddressCity') {
                                SelectParentAddressCity($(this).attr('ID'), $(this).attr('City'));
                            }

                        })
                        .append($('<h1>')
                            .text(vRows[p].City)
                        )
                    );
            pLV.append(pItem);
        };

        pLV.listview('refresh');
    };

    function LoadCountryList(vRows) {
        var pLV = $('#lstResultList');
        var pItem;

        $("#divRecordCount").text(" Number of record(s) found: " + vRows.length);

        pLV.html('');
        for (var p = 0; p <= vRows.length - 1; p++) {
            pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .attr('Country', vRows[p].Country)
                        .click(function (e) {
                            e.preventDefault();

                            if ($("#txtSearchCountryFor").val() == 'SelectPresentCountry') {
                                SelectPresentCountry($(this).attr('ID'), $(this).attr('Country'));
                            } else if ($("#txtSearchCountryFor").val() == 'SelectPermanentCountry') {
                                SelectPermanentCountry($(this).attr('ID'), $(this).attr('Country'));
                            } else if ($("#txtSearchCountryFor").val() == 'SelectParentAddressCountry') {
                                SelectParentAddressCountry($(this).attr('ID'), $(this).attr('Country'));
                            }

                        })
                        .append($('<h1>')
                            .text(vRows[p].Code + " - " + vRows[p].Country)
                        )
                    );
            pLV.append(pItem);
        };

        pLV.listview('refresh');
    };

    function LoadCitizenshipList(vRows) {
        var pLV = $('#lstResultList');
        var pItem;

        $("#divRecordCount").text(" Number of record(s) found: " + vRows.length);

        pLV.html('');
        for (var p = 0; p <= vRows.length - 1; p++) {
            pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .attr('Citizenship', vRows[p].Citizenship)
                        .click(function (e) {
                            e.preventDefault();
                            $("#txtCitizenshipID").val($(this).attr('ID'));
                            $("#txtCitizenship").val($(this).attr('Citizenship'));
                            $('.ui-dialog').dialog('close');
                        })
                        .append($('<h1>')
                            .text(vRows[p].Code + " - " + vRows[p].Citizenship)
                        )
                    );
            pLV.append(pItem);
        };

        pLV.listview('refresh');
    };

    function LoadOccupationList(vRows) {
        var pLV = $('#lstResultList');
        var pItem;

        $("#divRecordCount").text(" Number of record(s) found: " + vRows.length);

        pLV.html('');
        for (var p = 0; p <= vRows.length - 1; p++) {
            pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .attr('Occupation', vRows[p].Occupation)
                        .click(function (e) {
                            e.preventDefault();

                            if ($("#txtSearchOccupationFor").val() == 'SelectFatherOccupation') {
                                SelectFatherOccupation($(this).attr('ID'), $(this).attr('Occupation'));
                            } else if ($("#txtSearchOccupationFor").val() == 'SelectMotherOccupation') {
                                SelectMotherOccupation($(this).attr('ID'), $(this).attr('Occupation'));
                            }

                        })
                        .append($('<h1>')
                            .text(vRows[p].Code + ' - ' + vRows[p].Occupation)
                        )
                    );
            pLV.append(pItem);
        };

        pLV.listview('refresh');
    };

    function SelectPresentCity(strID, strCity) {

        $("#txtPresentAddressCityID").val(strID);
        $("#txtPresentAddressCity").val(strCity);

        $('.ui-dialog').dialog('close');
    };

    function SelectPermanentCity(strID, strCity) {

        $("#txtPermanentAddressCityID").val(strID);
        $("#txtPermanentAddressCity").val(strCity);

        $('.ui-dialog').dialog('close');
    };

    function SelectPlaceOfBirth(strID, strCity) {

        $("#txtPlaceOfBirthID").val(strID);
        $("#txtPlaceOfBirth").val(strCity);

        $('.ui-dialog').dialog('close');
    };

    function SelectParentAddressCity(strID, strCity) {

        $("#txtParentAddressCityID").val(strID);
        $("#txtParentAddressCity").val(strCity);

        $('.ui-dialog').dialog('close');
    };

    function SelectPresentCountry(strID, strCountry) {

        $("#txtPresentAddressCountryID").val(strID);
        $("#txtPresentAddressCountry").val(strCountry);

        $('.ui-dialog').dialog('close');
    };

    function SelectPermanentCountry(strID, strCountry) {

        $("#txtPermanentAddressCountryID").val(strID);
        $("#txtPermanentAddressCountry").val(strCountry);

        $('.ui-dialog').dialog('close');
    };

    function SelectParentAddressCountry(strID, strCountry) {

        $("#txtParentAddressCountryID").val(strID);
        $("#txtParentAddressCountry").val(strCountry);

        $('.ui-dialog').dialog('close');
    };



    function SelectFatherOccupation(strID, strOccupation) {

        $("#txtFatherOccupationID").val(strID);
        $("#txtFatherOccupation").val(strOccupation);

        $('.ui-dialog').dialog('close');
    };

    function SelectMotherOccupation(strID, strOccupation) {

        $("#txtMotherOccupationID").val(strID);
        $("#txtMotherOccupation").val(strOccupation);

        $('.ui-dialog').dialog('close');
    };

    function ShowPersonInfo(strID) {

        var pData = {};
        pData.PeopleID = strID;

        ClearPeopleFields();

        AjaxSend({
            url: "People/getPeopleInfo",
            data: pData,
            success: function (vData) {

                ClearPeopleFields();
                if (vData.PeopleData.Rows.length > 0) {

                    $("#txtID").val($.isEmptyObject(vData.PeopleData.Rows[0].ID) ? '' : vData.PeopleData.Rows[0].ID);
                    $("#txtPeopleID").val($.isEmptyObject(vData.PeopleData.Rows[0].PeopleID) ? '' : vData.PeopleData.Rows[0].PeopleID);

                    $("#txtSurname").val($.isEmptyObject(vData.PeopleData.Rows[0].Surname) ? '' : vData.PeopleData.Rows[0].Surname);
                    $("#txtFirstName").val($.isEmptyObject(vData.PeopleData.Rows[0].FirstName) ? '' : vData.PeopleData.Rows[0].FirstName);
                    $("#txtMiddleName").val($.isEmptyObject(vData.PeopleData.Rows[0].MiddleName) ? '' : vData.PeopleData.Rows[0].MiddleName);

                    $("#txtPresentAddressStreet").val($.isEmptyObject(vData.PeopleData.Rows[0].PresentAddressStreet) ? '' : vData.PeopleData.Rows[0].PresentAddressStreet);
                    $("#txtPresentAddressCityID").val($.isEmptyObject(vData.PeopleData.Rows[0].PresentAddressCityID) ? '' : vData.PeopleData.Rows[0].PresentAddressCityID);
                    $("#txtPresentAddressCity").val($.isEmptyObject(vData.PeopleData.Rows[0].PresentAddressCity) ? '' : vData.PeopleData.Rows[0].PresentAddressCity);
                    $("#txtPresentAddressCountryID").val($.isEmptyObject(vData.PeopleData.Rows[0].PresentAddressCountryID) ? '' : vData.PeopleData.Rows[0].PresentAddressCountryID);
                    $("#txtPresentAddressCountry").val($.isEmptyObject(vData.PeopleData.Rows[0].PresentAddressCountry) ? '' : vData.PeopleData.Rows[0].PresentAddressCountry);

                    $("#txtPermanentAddressStreet").val($.isEmptyObject(vData.PeopleData.Rows[0].PermanentAddressStreet) ? '' : vData.PeopleData.Rows[0].PermanentAddressStreet);
                    $("#txtPermanentAddressCityID").val($.isEmptyObject(vData.PeopleData.Rows[0].PermanentAddressCityID) ? '' : vData.PeopleData.Rows[0].PermanentAddressCityID);
                    $("#txtPermanentAddressCity").val($.isEmptyObject(vData.PeopleData.Rows[0].PermanentAddressCity) ? '' : vData.PeopleData.Rows[0].PermanentAddressCity);
                    $("#txtPermanentAddressCountryID").val($.isEmptyObject(vData.PeopleData.Rows[0].PermanentAddressCountryID) ? '' : vData.PeopleData.Rows[0].PermanentAddressCountryID);
                    $("#txtPermanentAddressCountry").val($.isEmptyObject(vData.PeopleData.Rows[0].PermanentAddressCountry) ? '' : vData.PeopleData.Rows[0].PermanentAddressCountry);

//                    $("#txtDateOfBirth").val($.isEmptyObject(vData.PeopleData.Rows[0].DateOfBirth) ? '' : vData.PeopleData.Rows[0].DateOfBirth);
                    $("#txtPlaceOfBirthID").val($.isEmptyObject(vData.PeopleData.Rows[0].PlaceOfBirthID) ? '' : vData.PeopleData.Rows[0].PlaceOfBirthID);
                    $("#txtPlaceOfBirth").val($.isEmptyObject(vData.PeopleData.Rows[0].PlaceOfBirth) ? '' : vData.PeopleData.Rows[0].PlaceOfBirth);
                    $("#selGender").val($.isEmptyObject(vData.PeopleData.Rows[0].Gender) ? '' : vData.PeopleData.Rows[0].Gender);
                    $('#selGender').selectmenu('refresh');

                    $("#txtCitizenshipID").val($.isEmptyObject(vData.PeopleData.Rows[0].CitizenshipID) ? '' : vData.PeopleData.Rows[0].CitizenshipID);
                    $("#txtCitizenship").val($.isEmptyObject(vData.PeopleData.Rows[0].Citizenship) ? '' : vData.PeopleData.Rows[0].Citizenship);

                    $("#txtHeight").val($.isEmptyObject(vData.PeopleData.Rows[0].Height) ? '' : vData.PeopleData.Rows[0].Height);
                    $("#selHeightMeasure").val($.isEmptyObject(vData.PeopleData.Rows[0].HeightMeasure) ? '' : vData.PeopleData.Rows[0].HeightMeasure);
                    $('#selHeightMeasure').selectmenu('refresh');
                    $("#txtWeight").val($.isEmptyObject(vData.PeopleData.Rows[0].Weight) ? '' : vData.PeopleData.Rows[0].Weight);
                    $("#selWeightMeasure").val($.isEmptyObject(vData.PeopleData.Rows[0].WeightMeasure) ? '' : vData.PeopleData.Rows[0].WeightMeasure);
                    $('#selWeightMeasure').selectmenu('refresh');

                    $("#selCivilStatus").val($.isEmptyObject(vData.PeopleData.Rows[0].CivilStatus) ? '' : vData.PeopleData.Rows[0].CivilStatus);
                    $('#selCivilStatus').selectmenu('refresh');

                    $("#txtSpouseSurname").val($.isEmptyObject(vData.PeopleData.Rows[0].SpouseSurname) ? '' : vData.PeopleData.Rows[0].SpouseSurname);
                    $("#txtSpouseFirstName").val($.isEmptyObject(vData.PeopleData.Rows[0].SpouseFirstName) ? '' : vData.PeopleData.Rows[0].SpouseFirstName);
                    $("#txtSpouseMiddleName").val($.isEmptyObject(vData.PeopleData.Rows[0].SpouseMiddleName) ? '' : vData.PeopleData.Rows[0].SpouseMiddleName);

                    $("#txtFatherSurname").val($.isEmptyObject(vData.PeopleData.Rows[0].FatherSurname) ? '' : vData.PeopleData.Rows[0].FatherSurname);
                    $("#txtFatherFirstName").val($.isEmptyObject(vData.PeopleData.Rows[0].FatherFirstName) ? '' : vData.PeopleData.Rows[0].FatherFirstName);
                    $("#txtFatherMiddleName").val($.isEmptyObject(vData.PeopleData.Rows[0].FatherMiddleName) ? '' : vData.PeopleData.Rows[0].FatherMiddleName);
                    $("#txtFatherOccupationID").val($.isEmptyObject(vData.PeopleData.Rows[0].FatherOccupationID) ? '' : vData.PeopleData.Rows[0].FatherOccupationID);
                    $("#txtFatherOccupation").val($.isEmptyObject(vData.PeopleData.Rows[0].FatherOccupation) ? '' : vData.PeopleData.Rows[0].FatherOccupation);

                    $("#txtMotherSurname").val($.isEmptyObject(vData.PeopleData.Rows[0].MotherSurname) ? '' : vData.PeopleData.Rows[0].MotherSurname);
                    $("#txtMotherFirstName").val($.isEmptyObject(vData.PeopleData.Rows[0].MotherFirstName) ? '' : vData.PeopleData.Rows[0].MotherFirstName);
                    $("#txtMotherMiddleName").val($.isEmptyObject(vData.PeopleData.Rows[0].MotherMiddleName) ? '' : vData.PeopleData.Rows[0].MotherMiddleName);
                    $("#txtMotherOccupationID").val($.isEmptyObject(vData.PeopleData.Rows[0].MotherOccupationID) ? '' : vData.PeopleData.Rows[0].MotherOccupationID);
                    $("#txtMotherOccupation").val($.isEmptyObject(vData.PeopleData.Rows[0].MotherOccupation) ? '' : vData.PeopleData.Rows[0].MotherOccupation);

                    $("#txtParentAddressStreet").val($.isEmptyObject(vData.PeopleData.Rows[0].ParentAddressStreet) ? '' : vData.PeopleData.Rows[0].ParentAddressStreet);
                    $("#txtParentAddressCityID").val($.isEmptyObject(vData.PeopleData.Rows[0].ParentAddressCityID) ? '' : vData.PeopleData.Rows[0].ParentAddressCityID);
                    $("#txtParentAddressCity").val($.isEmptyObject(vData.PeopleData.Rows[0].ParentAddressCity) ? '' : vData.PeopleData.Rows[0].ParentAddressCity);
                    $("#txtParentAddressCountryID").val($.isEmptyObject(vData.PeopleData.Rows[0].ParentAddressCountryID) ? '' : vData.PeopleData.Rows[0].ParentAddressCountryID);
                    $("#txtParentAddressCountry").val($.isEmptyObject(vData.PeopleData.Rows[0].ParentAddressCountry) ? '' : vData.PeopleData.Rows[0].ParentAddressCountry);

                    $("#txtResidenceCertNo").val($.isEmptyObject(vData.PeopleData.Rows[0].ResidenceCertNo) ? '' : vData.PeopleData.Rows[0].ResidenceCertNo);
                    $("#txtResidenceCertIssuedAt").val($.isEmptyObject(vData.PeopleData.Rows[0].ResidenceCertIssuedAt) ? '' : vData.PeopleData.Rows[0].ResidenceCertIssuedAt);
                    $("#txtResidenceCertIssuedOn").val($.isEmptyObject(vData.PeopleData.Rows[0].ResidenceCertIssuedOn) ? '' : vData.PeopleData.Rows[0].ResidenceCertIssuedOn);

                    $("#txtSSSNo").val($.isEmptyObject(vData.PeopleData.Rows[0].SSSNo) ? '' : vData.PeopleData.Rows[0].SSSNo);
                    $("#txtPhilHealthNo").val($.isEmptyObject(vData.PeopleData.Rows[0].PhilHealthNo) ? '' : vData.PeopleData.Rows[0].PhilHealthNo);
                    $("#txtTIN").val($.isEmptyObject(vData.PeopleData.Rows[0].TIN) ? '' : vData.PeopleData.Rows[0].TIN);
                    $("#txtHDMFNo").val($.isEmptyObject(vData.PeopleData.Rows[0].HDMFNo) ? '' : vData.PeopleData.Rows[0].HDMFNo);
                    $("#txtNBINo").val($.isEmptyObject(vData.PeopleData.Rows[0].NBINo) ? '' : vData.PeopleData.Rows[0].NBINo);

                    $("#txtPassportNo").val($.isEmptyObject(vData.PeopleData.Rows[0].PassportNo) ? '' : vData.PeopleData.Rows[0].PassportNo);
                    $("#txtPassportIssuedAt").val($.isEmptyObject(vData.PeopleData.Rows[0].PassportIssuedAt) ? '' : vData.PeopleData.Rows[0].PassportIssuedAt);
                    $("#txtPassportIssuedOn").val($.isEmptyObject(vData.PeopleData.Rows[0].PassportIssuedOn) ? '' : vData.PeopleData.Rows[0].PassportIssuedOn);
                }
            }
        });

    };

    function ClearPeopleFields() {
        $("#txtID").val('');
        $("#txtPeopleID").val('');

        $("#txtSurname").val('');
        $("#txtFirstName").val('');
        $("#txtMiddleName").val('');

        $("#txtPresentAddressStreet").val('');
        $("#txtPresentAddressCityID").val('');
        $("#txtPresentAddressCity").val('');
        $("#txtPresentAddressCountryID").val('');
        $("#txtPresentAddressCountry").val('');

        $("#txtPermanentAddressStreet").val('');
        $("#txtPermanentAddressCityID").val('');
        $("#txtPermanentAddressCity").val('');
        $("#txtPermanentAddressCountryID").val('');
        $("#txtPermanentAddressCountry").val('');

//        $("#txtDateOfBirth").val('');
        $("#txtPlaceOfBirthID").val('');
        $("#txtPlaceOfBirth").val('');
        $("#selGender").val('');
        $('#selGender').selectmenu('refresh');

        $("#txtCitizenshipID").val('');
        $("#txtCitizenship").val('');

        $("#txtHeight").val('');
        $("#selHeightMeasure").val('');
        $("#txtWeight").val('');
        $("#selWeightMeasure").val('');

        $("#selCivilStatus").val('');
        $('#selCivilStatus').selectmenu('refresh');

        $("#txtSpouseSurname").val('');
        $("#txtSpouseFirstName").val('');
        $("#txtSpouseMiddleName").val('');

        $("#txtFatherSurname").val('');
        $("#txtFatherFirstName").val('');
        $("#txtFatherMiddleName").val('');
        $("#txtFatherOccupationID").val('');
        $("#txtFatherOccupation").val('');
        $("#txtMotherSurname").val('');
        $("#txtMotherFirstName").val('');
        $("#txtMotherMiddleName").val('');
        $("#txtMotherOccupationID").val('');
        $("#txtMotherOccupation").val('');

        $("#txtParentAddressStreet").val('');
        $("#txtParentAddressCityID").val('');
        $("#txtParentAddressCity").val('');
        $("#txtParentAddressCountryID").val('');
        $("#txtParentAddressCountry").val('');

        $("#txtResidenceCertNo").val('');
        $("#txtResidenceCertIssuedAt").val('');
        $("#txtResidenceCertIssuedOn").val('');

        $("#txtSSSNo").val('');
        $("#txtPhilHealthNo").val('');
        $("#txtTIN").val('');
        $("#txtHDMFNo").val('');
        $("#txtNBINo").val('');

        $("#txtPassportNo").val('');
        $("#txtPassportIssuedAt").val('');
        $("#txtPassportIssuedOn").val('');
    }

    function ShowPersonContact() {
        var pData = {};
        pData.PeopleID = $("#txtID").val();

        AjaxSend({
            url: "People/getPeopleContactList",
            data: pData,
            success: function (vData) {
                LoadContactList(vData.PeopleContactList.Rows);
            }
        });
    };

    function LoadContactList(vRows) {
        var pLV = $('#lstContactList');
        var pItem;

        $("#divEmpContactRecordCount").text(" Number of record(s) found: " + vRows.length);

        pLV.html('');
        for (var p = 0; p <= vRows.length - 1; p++) {
            pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .click(function (e) {
                            e.preventDefault();
                        })
                        .append($('<h3>')
                            .text(vRows[p].ContactNo)
                        )
                        .append($('<p>')
                            .text(vRows[p].TelCo)
                        )
                    );
            pLV.append(pItem);
        };

        pLV.listview('refresh');
    };

});