require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryEducation.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryCities.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc=')
    ], function() {

    $(function () {

        $(document).ready(function () {

            setYearCollection("selYearStarted");
            setYearCollection("selYearGraduated");

            getPeopleEducList();
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

        $('#btnPeopleEducationPopUpMenu').click(function (e) {
            e.preventDefault();

            $("#divPeopleEducationPopUpMenu").popup("open")

        });

        $('#popPeopleEducationPeopleInfoName').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoName')), "_self");

        });

        $('#popPeopleEducationPeopleInfoAlias').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAlias')), "_self");

        });

        $('#popPeopleEducationPeopleInfoAddress').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAddress')), "_self");

        });

        $('#popPeopleEducationPeoplePersonalInfo').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePersonalInfo')), "_self");

        });

        $('#popPeopleEducationPeopleContactNo').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleContactNo')), "_self");

        });

        $('#popPeopleEducationPeopleEmpHistory').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEmpHistory')), "_self");

        });

        $('#popPeopleEducationPeopleRelatedPerson').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleRelatedPerson')), "_self");

        });

        $('#popPeopleEducationPeoplePrimeIDReference').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePrimeIDReference')), "_self");

        });

        $('#popPeopleEducationPeopleUserAccount').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleUserAccount')), "_self");

        });

        $('#btnPeopleEducAdd').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeopleEducInfo');
            ClearPeopleEducFields();

        });

        $('#btnEducationCourse').click(function () {

            Browser.LibraryEducation.Click = function (e) {

                $('#txtEducationCourseID').val(e.ID)
                $('#btnEducationCourse').text(e.CourseTitle)
                    .button('refresh');
            };
            Browser.LibraryEducation.Show();

        });

        $('#btnAddressCity').click(function () {

            Browser.LibraryCities.Click = function (e) {

                $('#txtAddressCityID').val(e.ID)
                $('#btnAddressCity').text(e.Name)
                    .button('refresh');

            };
            Browser.LibraryCities.Show();

        });

        $('#btnAddressCountry').click(function () {

            Browser.LibraryCountries.Click = function (e) {

                $('#txtAddressCountryID').val(e.ID)
                $('#btnAddressCountry').text(e.Name)
                    .button('refresh');

            };
            Browser.LibraryCountries.Show();

        });

        $('#selYearStarted').click(function (e) {
            e.preventDefault();

            if ($.trim($('#selYearStarted').val()) == "") {
                $('#selYearStarted').val('1980');
                $('#selYearStarted').selectmenu('refresh');
            }

        });

        $('#selYearGraduated').click(function (e) {
            e.preventDefault();

            if ($.trim($('#selYearGraduated').val()) == "") {
                $('#selYearGraduated').val('1980');
                $('#selYearGraduated').selectmenu('refresh');
            }

        });

        $('#btnPeopleEducSave').click(function (e) {
            e.preventDefault();

            if ($.trim($("#selAcademicLevel").val()) == "") {
                MessageBox("Please select academic level.",
                            "Academic Level",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtEducationCourseID").val()) == "") {
                MessageBox("Please select course.",
                                "Course",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                return;
            }

            if ($.trim($("#txtInstitutionName").val()) == "") {
                MessageBox("Please enter institution name.",
                            "Institution Name",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtAddressStreet").val()) == "") {
                MessageBox("Please select street address.",
                                "Street Address",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                return;
            }

            if ($.trim($("#txtAddressCityID").val()) == "") {
                MessageBox("Please select city address.",
                                "City Address",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                return;
            }

            if ($.trim($("#txtAddressCountryID").val()) == "") {
                MessageBox("Please select country address.",
                                "Country Address",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                return;
            }

            if ($.trim($("#selYearStarted").val()) == "") {
                MessageBox("Please select year started.",
                                "Started Year",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                return;
            }

            if ($.trim($("#selYearGraduated").val()) == "") {
                MessageBox("Please select year graduated.",
                                "Date End",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                return;
            }

            if (parseInt($("#selYearStarted").val()) > parseInt($("#selYearGraduated").val())) {
                MessageBox('Time travelling is not allowed. You cannot end earlier than when you started. Please review your “year started” and “year ended” information.',
                                "Started Year and Ended Year",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                return;
            }

            SavePeopleEduc();

        });

        $('#btnPeopleEducRemove').click(function (e) {
            e.preventDefault();

            MessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemovePeopleEduc();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );

        });

        $('#btnPeopleEducCancel').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeopleEducList');

        });

        $('#btnPeopleEducDone').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoSummary')), "_self");

        });

        function getPeopleEducList() {
            AjaxSend({
                url: 'People/getPeopleEducationList',
                success: function (vData) {
                    LoadPeopleEducList(vData.PeopleEducationData.Rows);
                }
            });
        }

        function LoadPeopleEducList(vRows) {
            var pLV = $('#lstPeopleEduc');
            var pItem;

            $("#divRecordCount").text(" Number of record(s) found: " + vRows.length);

            pLV.html('');

            pItem = $('<li>')
                    .attr('data-role', "list-divider")
                    .text('Educational Background List');
            pLV.append(pItem);

            for (var p = 0; p <= vRows.length - 1; p++) {

                pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                $.mobile.changePage('#pagePeopleEducInfo');
                                ShowPersonEducInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].InstitutionName)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].AddressCity + ', ' + vRows[p].AddressCountry)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].CourseTitle + " (" + vRows[p].AcademicLevel + ")")
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].YearStarted + ' - ' + vRows[p].YearGraduated)
                            )
                        );
                pLV.append(pItem);
            };

            pLV.listview('refresh');
        };

        function ShowPersonEducInfo(strID) {
            var pData = {};
            pData.ID = strID;

            ClearPeopleEducFields();

            AjaxSend({
                url: "People/getPeopleEducationInfo",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleEducationInfo.Rows.length > 0) {

                        $("#txtEducationID").val(vData.PeopleEducationInfo.Rows[0].ID || '');

                        $("#selAcademicLevel").val(vData.PeopleEducationInfo.Rows[0].AcademicLevel || '');
                        $('#selAcademicLevel').selectmenu('refresh');

                        $("#txtEducationCourseID").val(vData.PeopleEducationInfo.Rows[0].EducationCourseID || '');
                        $("#btnEducationCourse").text(vData.PeopleEducationInfo.Rows[0].CourseTitle || 'Please Select');
                        $("#btnEducationCourse").button('refresh');

                        $("#txtInstitutionName").val(vData.PeopleEducationInfo.Rows[0].InstitutionName || '');

                        $("#txtHouseBuildingNo").val(vData.PeopleEducationInfo.Rows[0].HouseBuildingNo || '');
                        $("#txtAddressStreet").val(vData.PeopleEducationInfo.Rows[0].AddressStreet || '');

                        $("#txtAddressCityID").val(vData.PeopleEducationInfo.Rows[0].AddressCityID || '');
                        $("#btnAddressCity").text(vData.PeopleEducationInfo.Rows[0].AddressCity || 'Please Select');
                        $("#btnAddressCity").button('refresh');

                        $("#txtAddressCountryID").val(vData.PeopleEducationInfo.Rows[0].AddressCountryID || '');
                        $("#btnAddressCountry").text(vData.PeopleEducationInfo.Rows[0].AddressCountry || 'Please Select');
                        $("#btnAddressCountry").button('refresh');

                        $("#selYearStarted").val(vData.PeopleEducationInfo.Rows[0].YearStarted || '');
                        $('#selYearStarted').selectmenu('refresh');

                        $("#selYearGraduated").val(vData.PeopleEducationInfo.Rows[0].YearGraduated || '');
                        $('#selYearGraduated').selectmenu('refresh');

                        $("#selGraduated").val(vData.PeopleEducationInfo.Rows[0].IsGraduated || '0');
                        $("#selGraduated").slider("refresh");

                        $("#btnPeopleEducRemove").show();
                    }
                }
            });

        };

        function ClearPeopleEducFields() {
            $("#txtEducationID").val('');

            $("#selAcademicLevel").val('');
            $('#selAcademicLevel').selectmenu('refresh');

            $("#txtEducationCourseID").val('');
            $("#btnEducationCourse").text('Please Select');
            $("#btnEducationCourse").button('refresh');

            $("#txtInstitutionName").val('');

            $("#txtHouseBuildingNo").val('');
            $("#txtAddressStreet").val('');

            $("#txtAddressCityID").val('');
            $("#btnAddressCity").text('Please Select');
            $("#btnAddressCity").button('refresh');

            $("#txtAddressCountryID").val('');
            $("#btnAddressCountry").text('Please Select');
            $("#btnAddressCountry").button('refresh');

            $("#selYearStarted").val('');
            $('#selYearStarted').selectmenu('refresh');

            $("#selYearGraduated").val('');
            $('#selYearGraduated').selectmenu('refresh');

            $("#selGraduated").val('0');
            $("#selGraduated").slider("refresh");

            $("#btnPeopleEducRemove").hide();

        }

        function SavePeopleEduc() {

            var pData = {};
            pData.PersonEducationID = $("#txtEducationID").val();
            pData.EducationCourseID = $("#txtEducationCourseID").val();
            pData.AcademicLevel = $("#selAcademicLevel").val();
            pData.InstitutionName = $("#txtInstitutionName").val();

            pData.HouseBuildingNo = $("#txtHouseBuildingNo").val();
            pData.Street = $("#txtAddressStreet").val();
            pData.CityID = $("#txtAddressCityID").val();
            pData.CountryID = $("#txtAddressCountryID").val();
            pData.YearStarted = $("#selYearStarted").val();
            pData.YearGraduated = $("#selYearGraduated").val();
            pData.IsGraduated = $("#selGraduated").val();

            AjaxSend({
                url: "People/SavePeopleEducation",
                data: pData,
                success: function (vData) {

                    $.mobile.changePage('#pagePeopleEducList');
                    getPeopleEducList();

                }
            });

        };

        function RemovePeopleEduc() {

            var pData = {};
            pData.PersonEducationID = $("#txtEducationID").val();

            AjaxSend({
                url: "People/RemovePeopleEducation",
                data: pData,
                success: function (vData) {

                    $.mobile.changePage('#pagePeopleEducList');
                    getPeopleEducList();

                }
            });

        };
    });
});