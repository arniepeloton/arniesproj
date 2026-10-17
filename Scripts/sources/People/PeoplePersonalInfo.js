require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryCitizenship.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryCities.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc=')
    ], function() {

    $(function () {

        $(document).ready(function () {

            ShowPersonPersonalInfo();

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

        $('#btnPeoplePersonalInfoPopUpMenu').click(function (e) {
            e.preventDefault();

            $("#divPeoplePersonalInfoPopUpMenu").popup("open")

        });

        $('#popPeoplePersonalInfoPeopleInfoName').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoName')), "_self");

        });

        $('#popPeoplePersonalInfoPeopleInfoAlias').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAlias')), "_self");

        });

        $('#popPeoplePersonalInfoPeopleInfoAddress').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAddress')), "_self");

        });

        $('#popPeoplePersonalInfoPeopleContact').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleContactNo')), "_self");

        });

        $('#popPeoplePersonalInfoPeopleEducation').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEducation')), "_self");

        });

        $('#popPeoplePersonalInfoPeopleEmpHistory').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEmpHistory')), "_self");

        });

        $('#popPeoplePersonalInfoPeopleRelatedPerson').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleRelatedPerson')), "_self");

        });

        $('#popPeoplePersonalInfoPeoplePrimeIDReference').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePrimeIDReference')), "_self");

        });

        $('#popPeoplePersonalInfoPeopleUserAccount').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleUserAccount')), "_self");

        });

        $('#btnCitizenship').click(function () {

            Browser.LibraryCitizenship.Click = function (e) {

                $('#txtCitizenshipID').val(e.ID)
                $('#btnCitizenship').text(e.Citizenship)
                    .button('refresh');

            };
            Browser.LibraryCitizenship.Show();

        });

        $('#btnHeight').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pageHeight');

        });

        $("#txtHeightFt").keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        });

        $("#txtHeightFtInc").keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        });

        $("#txtHeightCM").keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        });

        $("#txtHeightInc").keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        });

        $('#btnHeightOk').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeoplePersonalInfo');
            $("#btnHeight").text($("#txtHeightFt").val() + " ' " + $("#txtHeightFtInc").val() + ' " ');
            $("#btnHeight").button('refresh');

        });

        $('#btnHeightCancel').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeoplePersonalInfo');

        });

        $('#btnWeight').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pageWeight');

        });

        $("#txtWeightKg").keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        });

        $("#txtWeightLbs").keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        });

        $('#btnWeightOk').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeoplePersonalInfo');
            $('#btnWeight').text($('#txtWeightLbs').val() + " lbs");
            $("#btnWeight").button('refresh');

        });

        $('#btnWeightCancel').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeoplePersonalInfo');

        });

        $('#txtHeightFt').change(function (e) {
            e.preventDefault();

            AutoChangeHeight("txtHeightFt");

        });

        $('#txtHeightFtInc').change(function (e) {
            e.preventDefault();

            AutoChangeHeight("txtHeightFtInc");

        });

        $('#txtHeightCM').change(function (e) {
            e.preventDefault();

            AutoChangeHeight("txtHeightCM");

        });

        $('#txtHeightInc').change(function (e) {
            e.preventDefault();

            AutoChangeHeight("txtHeightInc");

        });

        $('#txtWeightKg').change(function (e) {
            e.preventDefault();

            AutoChangeWeight("txtWeightKg");

        });

        $('#txtWeightLbs').change(function (e) {
            e.preventDefault();

            AutoChangeWeight("txtWeightLbs");

        });

        $('#btnPOBCountry').click(function () {

            Browser.LibraryCountries.Click = function (e) {

                $('#txtPOBCountryID').val(e.ID)
                $('#btnPOBCountry').text(e.Name)
                    .button('refresh');

            };
            Browser.LibraryCountries.Show();

        });

        $('#btnPOBCity').click(function () {

            Browser.LibraryCities.Click = function (e) {

                $('#txtPOBCityID').val(e.ID)
                $('#btnPOBCity').text(e.Name)
                    .button('refresh');

            };
            Browser.LibraryCities.Show();

        });

        $('#btnPeopleInfoDone').click(function (e) {
            e.preventDefault();

            if ($.trim($("#selGender").val()) == "") {
                MessageBox("Please select gender.",
                            "Gender",
                            [
                                { Text: 'Ok' }
                            ]
                );

                return;
            }

            SavePersonPersonalInfo();

        });

        function AutoChangeHeight(objName) {

            var dblFT = parseFloat($("#txtHeightFt").val() || '0');
            var dblFTINC = parseFloat($("#txtHeightFtInc").val() || '0');
            var dblCM = parseFloat($("#txtHeightCM").val() || '0');
            var dblINC = parseFloat($("#txtHeightInc").val() || '0');

            if (objName == "txtHeightFt") {

                $("#txtHeightFt").val(dblFT.toFixed(0));
                $("#txtHeightFtInc").val(dblFTINC.toFixed(2));

                dblCM = (dblFT * 30.48) + (dblFTINC * 2.54);
                $("#txtHeightCM").val(dblCM.toFixed(2));

                dblINC = (dblFT * 12) + dblFTINC;
                $("#txtHeightInc").val(dblINC.toFixed(2));

            }
            else if (objName == "txtHeightFtInc") {

                $("#txtHeightFt").val(dblFT.toFixed(0));
                $("#txtHeightFtInc").val(dblFTINC.toFixed(2));

                dblCM = (dblFT * 30.48) + (dblFTINC * 2.54);
                $("#txtHeightCM").val(dblCM.toFixed(2));

                dblINC = (dblFT * 12) + dblFTINC;
                $("#txtHeightInc").val(dblINC.toFixed(2));

            }
            else if (objName == "txtHeightCM") {

                dblFT = Math.floor(dblCM / 30.48);
                dblFTINC = (dblCM - (dblFT * 30.48)) / 2.54;
                $("#txtHeightFt").val(dblFT.toFixed(0));
                $("#txtHeightFtInc").val(dblFTINC.toFixed(2));

                $("#txtHeightCM").val(dblCM.toFixed(2));

                dblINC = dblCM / 2.54;
                $("#txtHeightInc").val(dblINC.toFixed(2));

            }
            else if (objName == "txtHeightInc") {

                dblFT = Math.floor(dblINC / 12);
                dblFTINC = dblINC - (dblFT * 12);
                $("#txtHeightFt").val(dblFT.toFixed(0));
                $("#txtHeightFtInc").val(dblFTINC.toFixed(2));

                dblCM = dblINC * 2.54;
                $("#txtHeightCM").val(dblCM.toFixed(2));

                $("#txtHeightInc").val(dblINC.toFixed(2));

            }

        }

        function AutoChangeWeight(objName) {

            var dblKG = parseFloat($("#txtWeightKg").val() || '0');
            var dblLBS = parseFloat($("#txtWeightLbs").val() || '0');

            if (objName == "txtWeightKg") {
                dblLBS = (dblKG / 0.45359);
                $("#txtWeightKg").val(dblKG.toFixed(2));
                $("#txtWeightLbs").val(dblLBS.toFixed(2));
            }
            else if (objName == "txtWeightLbs") {
                dblKG = (dblLBS * 0.45359);
                $("#txtWeightKg").val(dblKG.toFixed(2));
                $("#txtWeightLbs").val(dblLBS.toFixed(2));
            }

        }

        function ShowPersonPersonalInfo() {

            ClearPersonPersonalInfoFields();

            AjaxSend({
                url: "People/getPeoplePersonalInfo",
                success: function (vData) {

                    if (vData.PeoplePersonalInfo.Rows.length > 0) {

                        $("#txtID").val(vData.PeoplePersonalInfo.Rows[0].ID || '');

                        $("#selGender").val(vData.PeoplePersonalInfo.Rows[0].Gender || '');
                        $("#selGender").selectmenu('refresh');

                        $("#txtCitizenshipID").val(vData.PeoplePersonalInfo.Rows[0].CitizenshipID || '');
                        $("#btnCitizenship").text(vData.PeoplePersonalInfo.Rows[0].Citizenship || 'Please Select');
                        $('#btnCitizenship').button('refresh');

                        $("#txtPOBCountryID").val(vData.PeoplePersonalInfo.Rows[0].PlaceOfBirthCountryID || '');
                        $("#btnPOBCountry").text(vData.PeoplePersonalInfo.Rows[0].PlaceOfBirthCountry || 'Please Select');
                        $('#btnPOBCountry').button('refresh');

                        $("#txtPOBCityID").val(vData.PeoplePersonalInfo.Rows[0].PlaceOfBirthCityID || '');
                        $("#btnPOBCity").text(vData.PeoplePersonalInfo.Rows[0].PlaceOfBirthCity || 'Please Select');
                        $('#btnPOBCity').button('refresh');

                        $("#txtHeightCM").val(vData.PeoplePersonalInfo.Rows[0].Height || '0');
                        $("#txtWeightLbs").val(vData.PeoplePersonalInfo.Rows[0].Weight || '0');

                        $("#selCivilStatus").val(vData.PeoplePersonalInfo.Rows[0].CivilStatus || '');
                        $("#selCivilStatus").selectmenu('refresh');

                        AutoChangeHeight("txtHeightCM");
                        AutoChangeWeight("txtWeightLbs");

                        $("#btnHeight").text($("#txtHeightFt").val() + " ' " + $("#txtHeightFtInc").val() + ' " ');
                        $('#btnHeight').button('refresh');

                        $("#btnWeight").text($("#txtWeightLbs").val() + ' lbs');
                        $('#btnWeight').button('refresh');

                    }
                }
            });

        };

        function ClearPersonPersonalInfoFields() {
            $("#txtID").val('');

            $("#selGender").val('');
            $("#selGender").selectmenu('refresh');

            $("#txtCitizenshipID").val('');
            $("#btnCitizenship").text('');
            $('#btnCitizenship').button('refresh');

            $("#txtPOBCountryID").val('');
            $("#btnPOBCountry").text('');
            $('#btnPOBCountry').button('refresh');

            $("#txtPOBCityID").val('');
            $("#btnPOBCity").text('');
            $('#btnPOBCity').button('refresh');

            $("#btnHeight").text('Please Input');
            $("#txtHeightFt").val('0');
            $("#txtHeightFtInc").val('0');
            $("#txtHeightCM").val('0');
            $("#txtHeightInc").val('0');

            $("#btnWeight").val('Please Input');
            $("#txtWeightKg").val('0');
            $("#txtWeightLbs").val('0');

            $("#selCivilStatus").val('');
            $("#selCivilStatus").selectmenu('refresh');

        }

        function SavePersonPersonalInfo() {

            var pData = {};
            pData.Gender = $("#selGender").val();
            pData.CitizenshipID = $("#txtCitizenshipID").val();
            pData.PlaceOfBirthCityID = $("#txtPOBCityID").val();
            pData.PlaceOfBirthCountryID = $("#txtPOBCountryID").val();
            pData.Height = $("#txtHeightCM").val();
            pData.Weight = $("#txtWeightLbs").val();
            pData.CivilStatus = $("#selCivilStatus").val();

            AjaxSend({
                url: "People/SavePeoplePersonalInfo",
                data: pData,
                success: function (vData) {

                    if (vData.PeoplePersonalInfo == "HeightError") {
                        MessageBox("Please enter valid height.",
                                    "Height",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                    }
                    else if (vData.PeoplePersonalInfo == "WeightError") {
                        MessageBox("Please enter valid weight.",
                                    "Weight",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                    }
                    else {
                        window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoSummary')), "_self");
                    }

                }
            });

        };

    });
});