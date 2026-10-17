require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryCities.js?crc=')
    ], function() {

    $(function () {

        $(document).ready(function () {

            setMonthCollection("selResCertMonth");
            setYearCollection("selResCertYear");

            setMonthCollection("selPassportMonth");
            setYearCollection("selPassportYear");

            setMonthCollection("selPassportExpMonth");
            setYearCollection("selPassportExpYear");

            ShowPersonPrimaryIDDetails();

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

        $('#btnPeoplePrimeIDPopUpMenu').click(function (e) {
            e.preventDefault();

            $("#divPeoplePrimeIDPopUpMenu").popup("open")

        });

        $('#popPeoplePrimeIDPeopleInfoName').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoName')), "_self");

        });

        $('#popPeoplePrimeIDPeopleInfoAlias').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAlias')), "_self");

        });

        $('#popPeoplePrimeIDPeopleInfoAddress').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAddress')), "_self");

        });

        $('#popPeoplePrimeIDPeopleContact').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleContactNo')), "_self");

        });

        $('#popPeoplePrimeIDPeoplePersonalInfo').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePersonalInfo')), "_self");

        });

        $('#popPeoplePrimeIDPeopleEducation').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEducation')), "_self");

        });

        $('#popPeoplePrimeIDPeopleEmpHistory').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEmpHistory')), "_self");

        });

        $('#popPeoplePrimeIDPeopleRelatedPerson').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleRelatedPerson')), "_self");

        });

        $('#popPeoplePrimeIDPeopleUserAccount').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleUserAccount')), "_self");

        });

        $('#btnResidenceCertNo').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeopleResCertDetails');

        });

        $('#btnResCertIssuedAt').click(function () {

            Browser.LibraryCities.Click = function (e) {

                $('#txtResCertIssuedAtID').val(e.ID)
                $('#btnResCertIssuedAt').text(e.Name)
                    .button('refresh');

            };
            Browser.LibraryCities.Show();

        });

        $('#selResCertMonth').change(function (e) {
            e.preventDefault();

            setDayCollection('selResCertMonth', 'selResCertDay', 'selResCertYear', true);

        });

        $('#selResCertYear').change(function (e) {
            e.preventDefault();

            setDayCollection('selResCertMonth', 'selResCertDay', 'selResCertYear', true);

        });

        $('#selResCertYear').click(function (e) {
            e.preventDefault();

            if ($.trim($('#selResCertYear').val()) == "") {
                $('#selResCertYear').val('1980');
                $('#selResCertYear').selectmenu('refresh');
            }

        });

        $('#btnResCertOk').click(function (e) {
            e.preventDefault();

            if ($.trim($("#txtResCertNo").val()) != "") {
                if ($.trim($("#selResCertMonth").val()) == "") {
                    MessageBox("Please enter valid issued date of residence certification.",
                                "Residence Cert. Issued Date",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }
                if ($.trim($("#selResCertDay").val()) == "") {
                    MessageBox("Please enter valid issued date of residence certification.",
                                "Residence Cert. Issued Date",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }
                if ($.trim($("#selResCertYear").val()) == "") {
                    MessageBox("Please enter valid issued date of residence certification.",
                                "Residence Cert. Issued Date",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }
                if ($.trim($("#txtResCertIssuedAtID").val()) == "") {
                    MessageBox("Please select where the residence certificate was issued.",
                                "Residence Cert. Issued At",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }
            }

            var strCertNo = "";

            if ($.trim($("#txtResCertNo").val()) == "") {
                strCertNo = "Please Input";
            }
            else {
                strCertNo = $("#txtResCertNo").val();
            }

            $.mobile.changePage('#pagePeoplePrimeIDReference');

            $("#btnResidenceCertNo").text(strCertNo);
            $("#btnResidenceCertNo").button('refresh');

        });

        $('#btnResCertCancel').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeoplePrimeIDReference');

        });

        $('#btnPassportNo').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeoplePassportDetails');

        });

        $('#btnPassportIssuedAt').click(function () {

            Browser.LibraryCities.Click = function (e) {

                $('#txtPassportIssuedAtID').val(e.ID)
                $('#btnPassportIssuedAt').text(e.Name)
                    .button('refresh');

            };
            Browser.LibraryCities.Show();

        });

        $('#selPassportMonth').change(function (e) {
            e.preventDefault();

            setDayCollection('selPassportMonth', 'selPassportDay', 'selPassportYear', true);

        });

        $('#selPassportYear').change(function (e) {
            e.preventDefault();

            setDayCollection('selPassportMonth', 'selPassportDay', 'selPassportYear', true);

        });

        $('#selPassportYear').click(function (e) {
            e.preventDefault();

            if ($.trim($('#selPassportYear').val()) == "") {
                $('#selPassportYear').val('1980');
                $('#selPassportYear').selectmenu('refresh');
            }

        });

        $('#selPassportExpMonth').change(function (e) {
            e.preventDefault();

            setDayCollection('selPassportExpMonth', 'selPassportExpDay', 'selPassportExpYear', true);

        });

        $('#selPassportExpYear').change(function (e) {
            e.preventDefault();

            setDayCollection('selPassportExpMonth', 'selPassportExpDay', 'selPassportExpYear', true);

        });

        $('#selPassportExpYear').click(function (e) {
            e.preventDefault();

            if ($.trim($('#selPassportExpYear').val()) == "") {
                $('#selPassportExpYear').val('1980');
                $('#selPassportExpYear').selectmenu('refresh');
            }

        });

        $('#btnPassportOk').click(function (e) {
            e.preventDefault();

            if ($.trim($("#txtPassportNo").val()) != "") {
                if ($.trim($("#selPassportMonth").val()) == "") {
                    MessageBox("Please enter valid issued date of passport.",
                                "Passport Issued Date",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }
                if ($.trim($("#selPassportDay").val()) == "") {
                    MessageBox("Please enter valid issued date of passport.",
                                "Passport Issued Date",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }
                if ($.trim($("#selPassportYear").val()) == "") {
                    MessageBox("Please enter valid issued date of passport.",
                                "Passport Issued Date",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }

                if ($.trim($("#selPassportExpMonth").val()) == "") {
                    MessageBox("Please enter valid expiration date of passport.",
                                "Passport Expiration Date",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }
                if ($.trim($("#selPassportExpDay").val()) == "") {
                    MessageBox("Please enter valid expiration date of passport.",
                                "Passport Expiration Date",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }
                if ($.trim($("#selPassportExpYear").val()) == "") {
                    MessageBox("Please enter valid expiration date of passport.",
                                "Passport Expiration Date",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }

                if ($.trim($("#txtPassportIssuedAtID").val()) == "") {
                    MessageBox("Please select where the passport was issued.",
                                "Passport Issued At",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }
            }

            var strPassNo = "";

            if ($.trim($("#txtPassportNo").val()) == "") {
                strPassNo = "Please Input";
            }
            else {
                strPassNo = $("#txtPassportNo").val();
            }

            $.mobile.changePage('#pagePeoplePrimeIDReference');

            $("#btnPassportNo").text(strPassNo);
            $("#btnPassportNo").button('refresh');

        });

        $('#btnPassportCancel').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeoplePrimeIDReference');

        });

        $('#btnPrimeIDDone').click(function (e) {
            e.preventDefault();

            SavePersonPrimaryIDDetails();

        });

        function ShowPersonPrimaryIDDetails() {

            ClearPersonPrimaryIDFields();

            AjaxSend({
                url: "People/getPeoplePrimaryID",
                success: function (vData) {

                    if (vData.PeoplePrimeIDData.Rows.length > 0) {

                        $("#txtID").val(vData.PeoplePrimeIDData.Rows[0].ID || '');

                        $("#btnResidenceCertNo").text(vData.PeoplePrimeIDData.Rows[0].ResidenceCertNo || 'Please Input');
                        $("#btnResidenceCertNo").button('refresh');
                        $("#txtResCertNo").val(vData.PeoplePrimeIDData.Rows[0].ResidenceCertNo || '');
                        $("#txtResCertIssuedAtID").val(vData.PeoplePrimeIDData.Rows[0].ResidenceCertIssuedAtID || '');
                        $("#btnResCertIssuedAt").text(vData.PeoplePrimeIDData.Rows[0].ResidenceCertIssuedAt || 'Please Select');
                        $("#selResCertMonth").val(vData.PeoplePrimeIDData.Rows[0].ResidenceCertIssuedOnMonth || '');
                        $("#selResCertYear").val(vData.PeoplePrimeIDData.Rows[0].ResidenceCertIssuedOnYear || '');
                        setDayCollection('selResCertMonth', 'selResCertDay', 'selResCertYear', false);
                        $("#selResCertDay").val(vData.PeoplePrimeIDData.Rows[0].ResidenceCertIssuedOnDay || '');

                        $("#txtSSSNo").val(vData.PeoplePrimeIDData.Rows[0].SSSNo || '');
                        $("#txtPhilHealthNo").val(vData.PeoplePrimeIDData.Rows[0].PhilHealthNo || '');
                        $("#txtTIN").val(vData.PeoplePrimeIDData.Rows[0].TIN || '');
                        $("#txtHDMFNo").val(vData.PeoplePrimeIDData.Rows[0].HDMFNo || '');
                        $("#txtNBINo").val(vData.PeoplePrimeIDData.Rows[0].NBINo || '');

                        $("#btnPassportNo").text(vData.PeoplePrimeIDData.Rows[0].PassportNo || 'Please Input');
                        $("#btnPassportNo").button('refresh');
                        $("#txtPassportNo").val(vData.PeoplePrimeIDData.Rows[0].PassportNo || '');
                        $("#txtPassportIssuedAtID").val(vData.PeoplePrimeIDData.Rows[0].PassportIssuedAtID || '');
                        $("#btnPassportIssuedAt").text(vData.PeoplePrimeIDData.Rows[0].PassportIssuedAt || 'Please Select');

                        $("#selPassportMonth").val(vData.PeoplePrimeIDData.Rows[0].PassportIssuedOnMonth || '');
                        $("#selPassportYear").val(vData.PeoplePrimeIDData.Rows[0].PassportIssuedOnYear || '');
                        setDayCollection('selPassportMonth', 'selPassportDay', 'selPassportYear', false);
                        $("#selPassportDay").val(vData.PeoplePrimeIDData.Rows[0].PassportIssuedOnDay || '');

                        $("#selPassportExpMonth").val(vData.PeoplePrimeIDData.Rows[0].PassportExpirationDateMonth || '');
                        $("#selPassportExpYear").val(vData.PeoplePrimeIDData.Rows[0].PassportExpirationDateYear || '');
                        setDayCollection('selPassportExpMonth', 'selPassportExpDay', 'selPassportExpYear', false);
                        $("#selPassportExpDay").val(vData.PeoplePrimeIDData.Rows[0].PassportExpirationDateDay || '');

                    }
                }
            });

        };

        function ClearPersonPrimaryIDFields() {
            $("#txtID").val('');

            $("#btnResidenceCertNo").text('Please Input');
            $("#btnResidenceCertNo").button('refresh');
            $("#txtResCertNo").val('');
            $("#txtResCertIssuedAtID").val('');
            $("#btnResCertIssuedAt").text('Please Select');

            $("#selResCertMonth").val('');
            $("#selResCertDay").val('');
            $("#selResCertYear").val('');

            $("#txtSSSNo").val('');
            $("#txtPhilHealthNo").val('');
            $("#txtTIN").val('');
            $("#txtHDMFNo").val('');
            $("#txtNBINo").val('');

            $("#btnPassportNo").text('Please Input');
            $("#btnPassportNo").button('refresh');
            $("#txtPassportNo").val('');
            $("#txtPassportIssuedAtID").val('');
            $("#btnPassportIssuedAt").text('Please Select');

            $("#selPassportMonth").val('');
            $("#selPassportDay").val('');
            $("#selPassportYear").val('');

            $("#selPassportExpMonth").val('');
            $("#selPassportExpDay").val('');
            $("#selPassportExpYear").val('');
        }

        function SavePersonPrimaryIDDetails() {

            var dtResCertDate;
            var dtPassportDate;
            var dtPassportExpDate;
            var pData = {};

            dtResCertDate = (($("#selResCertMonth").val() + '/' + $("#selResCertDay").val() + '/' + $("#selResCertYear").val()) == '//' ? '' : ($("#selResCertMonth").val() + '/' + $("#selResCertDay").val() + '/' + $("#selResCertYear").val()));
            dtPassportDate = (($("#selPassportMonth").val() + '/' + $("#selPassportDay").val() + '/' + $("#selPassportYear").val()) == '//' ? '' : ($("#selPassportMonth").val() + '/' + $("#selPassportDay").val() + '/' + $("#selPassportYear").val()));
            dtPassportExpDate = (($("#selPassportExpMonth").val() + '/' + $("#selPassportExpDay").val() + '/' + $("#selPassportExpYear").val()) == '//' ? '' : ($("#selPassportExpMonth").val() + '/' + $("#selPassportExpDay").val() + '/' + $("#selPassportExpYear").val()));

            pData.ResidenceCertNo = $.trim($("#txtResCertNo").val());
            pData.ResidenceCertIssuedAtID = $.trim($("#txtResCertIssuedAtID").val());
            pData.ResidenceCertIssuedOn = dtResCertDate;
            pData.SSSNo = $.trim($("#txtSSSNo").val());
            pData.PhilHealthNo = $.trim($("#txtPhilHealthNo").val());
            pData.TIN = $.trim($("#txtTIN").val());
            pData.HDMFNo = $.trim($("#txtHDMFNo").val());
            pData.NBINo = $.trim($("#txtNBINo").val());
            pData.PassportNo = $.trim($("#txtPassportNo").val());
            pData.PassportIssuedAtID = $("#txtPassportIssuedAtID").val();
            pData.PassportIssuedOn = dtPassportDate;
            pData.PassportExpirationDate = dtPassportExpDate;

            AjaxSend({
                url: "People/SavePeoplePrimaryID",
                data: pData,
                success: function (vData) {

                    if (vData.PeoplePrimeIDData == "ResidenceCertIssuedOnError") {
                        MessageBox("Please enter valid issued date of residence certification.",
                                    "Residence Cert. Issued Date",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                    }
                    else if (vData.PeoplePrimeIDData == "PassportIssuedOnError") {
                        MessageBox("Please enter valid issued date of passport.",
                                    "Passport Issued Date",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                    }
                    else if (vData.PeoplePrimeIDData == "PassportExpirationDateError") {
                        MessageBox("Please enter valid expiration date of passport.",
                                    "Passport Expiration Date",
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