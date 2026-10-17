$(document).ready(function() {
    $("#txtDateOfBirth").datepicker();
    $("#txtResidenceCertIssuedOn").datepicker();
    $("#txtPassportIssuedOn").datepicker();
});

function EmployeeSearch() {

    alert("Employee Search : " + $("#txtSearch").val());

};

function BrowseCity() {

    alert("test1");
    $.ajax({
        url: "getCityList/",
        dataType: "json",
        type: "POST",
        data: { 'Keywords': 'DAVAO' },
        success: function (data) {
            alert("test");
        }
    });

}

function ShowEmployeeInfo(strID) {
    $.ajax({
        url: "getEmployeeInfo/",
        dataType: "json",
        type: "POST",
        data: { 'ID': strID },
        success: function (data) {
            if (data.length > 0) {
                $("#txtEmployeeID").val($.isEmptyObject(data[0]["EmployeeID"]) ? '' : data[0]["EmployeeID"]);
                $("#txtSurname").val($.isEmptyObject(data[0]["Surname"]) ? '' : data[0]["Surname"]);
                $("#txtFirstName").val($.isEmptyObject(data[0]["FirstName"]) ? '' : data[0]["FirstName"]);
                $("#txtMiddleName").val($.isEmptyObject(data[0]["MiddleName"]) ? '' : data[0]["MiddleName"]);

                $("#txtPresentAddressStreet").val($.isEmptyObject(data[0]["PresentAddressStreet"]) ? '' : data[0]["PresentAddressStreet"]);
                $("#txtPresentAddressCityID").val($.isEmptyObject(data[0]["PresentAddressCityID"]) ? '' : data[0]["PresentAddressCityID"]);
                $("#txtPresentAddressCity").val($.isEmptyObject(data[0]["PresentAddressCity"]) ? '' : data[0]["PresentAddressCity"]);
                $("#txtPresentAddressCountryID").val($.isEmptyObject(data[0]["PresentAddressCountryID"]) ? '' : data[0]["PresentAddressCountryID"]);
                $("#txtPresentAddressCountry").val($.isEmptyObject(data[0]["PresentAddressCountry"]) ? '' : data[0]["PresentAddressCountry"]);

                $("#txtPermanentAddressStreet").val($.isEmptyObject(data[0]["PermanentAddressStreet"]) ? '' : data[0]["PermanentAddressStreet"]);
                $("#txtPermanentAddressCityID").val($.isEmptyObject(data[0]["PermanentAddressCityID"]) ? '' : data[0]["PermanentAddressCityID"]);
                $("#txtPermanentAddressCity").val($.isEmptyObject(data[0]["PermanentAddressCity"]) ? '' : data[0]["PermanentAddressCity"]);
                $("#txtPermanentAddressCountryID").val($.isEmptyObject(data[0]["PermanentAddressCountryID"]) ? '' : data[0]["PermanentAddressCountryID"]);
                $("#txtPermanentAddressCountry").val($.isEmptyObject(data[0]["PermanentAddressCountry"]) ? '' : data[0]["PermanentAddressCountry"]);

                $("#txtDateOfBirth").val($.isEmptyObject(data[0]["DateOfBirth"]) ? '' : data[0]["DateOfBirth"]);
                $("#txtPlaceOfBirthID").val($.isEmptyObject(data[0]["PlaceOfBirthID"]) ? '' : data[0]["PlaceOfBirthID"]);
                $("#txtPlaceOfBirth").val($.isEmptyObject(data[0]["PlaceOfBirth"]) ? '' : data[0]["PlaceOfBirth"]);
                $("#selGender").val($.isEmptyObject(data[0]["Gender"]) ? '' : data[0]["Gender"]);
                $('#selGender').selectmenu('refresh');

                $("#txtCitizenshipID").val($.isEmptyObject(data[0]["CitizenshipID"]) ? '' : data[0]["CitizenshipID"]);
                $("#txtCitizenship").val($.isEmptyObject(data[0]["Citizenship"]) ? '' : data[0]["Citizenship"]);

                $("#txtHeight").val($.isEmptyObject(data[0]["Height"]) ? '' : data[0]["Height"]);
                $("#selHeightMeasure").val($.isEmptyObject(data[0]["HeightMeasure"]) ? '' : data[0]["HeightMeasure"]);
                $("#txtWeight").val($.isEmptyObject(data[0]["Weight"]) ? '' : data[0]["Weight"]);
                $("#selWeightMeasure").val($.isEmptyObject(data[0]["WeightMeasure"]) ? '' : data[0]["WeightMeasure"]);

                $("#selCivilStatus").val($.isEmptyObject(data[0]["CivilStatus"]) ? '' : data[0]["CivilStatus"]);
                $('#selCivilStatus').selectmenu('refresh');

                $("#txtSpouseSurname").val($.isEmptyObject(data[0]["SpouseSurname"]) ? '' : data[0]["SpouseSurname"]);
                $("#txtSpouseFirstName").val($.isEmptyObject(data[0]["SpouseFirstName"]) ? '' : data[0]["SpouseFirstName"]);
                $("#txtSpouseMiddleName").val($.isEmptyObject(data[0]["SpouseMiddleName"]) ? '' : data[0]["SpouseMiddleName"]);

                $("#txtFatherSurname").val($.isEmptyObject(data[0]["FatherSurname"]) ? '' : data[0]["FatherSurname"]);
                $("#txtFatherFirstName").val($.isEmptyObject(data[0]["FatherFirstName"]) ? '' : data[0]["FatherFirstName"]);
                $("#txtFatherMiddleName").val($.isEmptyObject(data[0]["FatherMiddleName"]) ? '' : data[0]["FatherMiddleName"]);
                $("#txtFatherOccupationID").val($.isEmptyObject(data[0]["FatherOccupationID"]) ? '' : data[0]["FatherOccupationID"]);
                $("#txtFatherOccupation").val($.isEmptyObject(data[0]["FatherOccupation"]) ? '' : data[0]["FatherOccupation"]);
                $("#txtMotherSurname").val($.isEmptyObject(data[0]["MotherSurname"]) ? '' : data[0]["MotherSurname"]);
                $("#txtMotherFirstName").val($.isEmptyObject(data[0]["MotherFirstName"]) ? '' : data[0]["MotherFirstName"]);
                $("#txtMotherMiddleName").val($.isEmptyObject(data[0]["MotherMiddleName"]) ? '' : data[0]["MotherMiddleName"]);
                $("#txtMotherOccupationID").val($.isEmptyObject(data[0]["MotherOccupationID"]) ? '' : data[0]["MotherOccupationID"]);
                $("#txtMotherOccupation").val($.isEmptyObject(data[0]["MotherOccupation"]) ? '' : data[0]["MotherOccupation"]);

                $("#txtParentAddressStreet").val($.isEmptyObject(data[0]["ParentAddressStreet"]) ? '' : data[0]["ParentAddressStreet"]);
                $("#txtParentAddressCityID").val($.isEmptyObject(data[0]["ParentAddressCityID"]) ? '' : data[0]["ParentAddressCityID"]);
                $("#txtParentAddressCity").val($.isEmptyObject(data[0]["ParentAddressCity"]) ? '' : data[0]["ParentAddressCity"]);
                $("#txtParentAddressCountryID").val($.isEmptyObject(data[0]["ParentAddressCountryID"]) ? '' : data[0]["ParentAddressCountryID"]);
                $("#txtParentAddressCountry").val($.isEmptyObject(data[0]["ParentAddressCountry"]) ? '' : data[0]["ParentAddressCountry"]);

                $("#txtResidenceCertNo").val($.isEmptyObject(data[0]["ResidenceCertNo"]) ? '' : data[0]["ResidenceCertNo"]);
                $("#txtResidenceCertIssuedAt").val($.isEmptyObject(data[0]["ResidenceCertIssuedAt"]) ? '' : data[0]["ResidenceCertIssuedAt"]);
                $("#txtResidenceCertIssuedOn").val($.isEmptyObject(data[0]["ResidenceCertIssuedOn"]) ? '' : data[0]["ResidenceCertIssuedOn"]);

                $("#txtSSSNo").val($.isEmptyObject(data[0]["SSSNo"]) ? '' : data[0]["SSSNo"]);
                $("#txtPhilHealthNo").val($.isEmptyObject(data[0]["PhilHealthNo"]) ? '' : data[0]["PhilHealthNo"]);
                $("#txtTIN").val($.isEmptyObject(data[0]["TIN"]) ? '' : data[0]["TIN"]);
                $("#txtHDMFNo").val($.isEmptyObject(data[0]["HDMFNo"]) ? '' : data[0]["HDMFNo"]);
                $("#txtNBINo").val($.isEmptyObject(data[0]["NBINo"]) ? '' : data[0]["NBINo"]);

                $("#txtPassportNo").val($.isEmptyObject(data[0]["PassportNo"]) ? '' : data[0]["PassportNo"]);
                $("#txtPassportIssuedAt").val($.isEmptyObject(data[0]["PassportIssuedAt"]) ? '' : data[0]["PassportIssuedAt"]);
                $("#txtPassportIssuedOn").val($.isEmptyObject(data[0]["PassportIssuedOn"]) ? '' : data[0]["PassportIssuedOn"]);
            }
        }
    });

};
