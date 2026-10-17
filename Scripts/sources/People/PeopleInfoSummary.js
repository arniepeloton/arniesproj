require([
    'common'
    ], function () {

        $(function () {
            //            alert("1");
            $(document).ready(function () {
                AjaxSend({
                    url: '/People/IsNewEntry',
                    success: function (vData) {
                        if (OfflineMode)
                            ShowPersonNameInfo(Web.PeopleUserID());
                        else
                            ShowPersonNameInfo(vData.PeopleID);

                    }
                });

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

            $('#btnSumPeopleInfoName').click(function (e) {
                e.preventDefault();

//                window.open(GetPathFormat("PeopleInfoName"), "_self");
                if (OfflineMode)
                    window.location = '/People/PeopleInfoName?IsMobile=1';
                else
                    window.location = MergeUrlParameters(GetUrlParameters(), '/People/PeopleInfoName');
            });

            $('#btnSumPeopleInfoAlias').click(function (e) {
                e.preventDefault();

//                window.open(GetPathFormat("PeopleInfoAlias"), "_self");
                if (OfflineMode)
                    window.location = '/People/PeopleInfoAlias?IsMobile=1';
                else
                    window.location = MergeUrlParameters(GetUrlParameters(), '/People/PeopleInfoAlias');
            });

            $('#btnSumPeopleInfoAddress').click(function (e) {
                e.preventDefault();

//                window.open(GetPathFormat("PeopleInfoAddress"), "_self");
                if (OfflineMode)
                    window.location = '/People/PeopleInfoAddress?IsMobile=1';
                else
                    window.location = MergeUrlParameters(GetUrlParameters(), '/People/PeopleInfoAddress');
            });

            $('#btnSumPeoplePersonalInfo').click(function (e) {
                e.preventDefault();

//                window.open(GetPathFormat("PeoplePersonalInfo"), "_self");
                if (OfflineMode)
                    window.location = '/People/PeoplePersonalInfo?IsMobile=1';
                else
                    window.location = MergeUrlParameters(GetUrlParameters(), '/People/PeoplePersonalInfo');
            });

            $('#btnSumPeopleContactNo').click(function (e) {
                e.preventDefault();

//                window.open(GetPathFormat("PeopleContactNo"), "_self");
                if (OfflineMode)
                    window.location = '/People/PeopleContactNo?IsMobile=1';
                else
                    window.location = MergeUrlParameters(GetUrlParameters(), '/People/PeopleContactNo');
            });

            $('#btnSumPeopleEducation').click(function (e) {
                e.preventDefault();

//                window.open(GetPathFormat("PeopleEducation"), "_self");
                if (OfflineMode)
                    window.location = '/People/PeopleEducation?IsMobile=1';
                else
                    window.location = MergeUrlParameters(GetUrlParameters(), '/People/PeopleEducation');
            });

            $('#btnSumPeopleEmpHistory').click(function (e) {
                e.preventDefault();

//                window.open(GetPathFormat("PeopleEmpHistory"), "_self");
                if (OfflineMode)
                    window.location = '/People/PeopleEmpHistory?IsMobile=1';
                else
                    window.location = MergeUrlParameters(GetUrlParameters(), '/People/PeopleEmpHistory');
            });

            $('#btnSumPeopleRelatedPerson').click(function (e) {
                e.preventDefault();

//                window.open(GetPathFormat("PeopleRelatedPerson"), "_self");
                if (OfflineMode)
                    window.location = '/People/PeopleRelatedPerson?IsMobile=1';
                else
                    window.location = MergeUrlParameters(GetUrlParameters(), '/People/PeopleRelatedPerson');
            });

            $('#btnSumPeoplePrimeIDReference').click(function (e) {
                e.preventDefault();

//                window.open(GetPathFormat("PeoplePrimeIDReference"), "_self");
                if (OfflineMode)
                    window.location = '/People/PeoplePrimeIDReference?IsMobile=1';
                else
                    window.location = MergeUrlParameters(GetUrlParameters(), '/People/PeoplePrimeIDReference');
            });

            $('#btnSumPeopleUserAccount').click(function (e) {
                e.preventDefault();

//                window.open(GetPathFormat("PeopleUserAccount"), "_self");
                if (OfflineMode)
                    window.location = '/People/PeopleUserAccount?IsMobile=1';
                else
                    window.location = MergeUrlParameters(GetUrlParameters(), '/People/PeopleUserAccount');
            });

            $('#btnDTRPIN').click(function (e) {
                e.preventDefault();

                //                window.open(GetPathFormat("PeopleUserAccount"), "_self");
                if (OfflineMode)
                    window.location = '/People/PeopleDTRPIN?IsMobile=1';
                else
                    window.location = MergeUrlParameters(GetUrlParameters(), '/People/PeopleDTRPIN');
            });
            
            $('#btnPeopleSummaryDone').click(function (e) {
                e.preventDefault();

//                window.open(GetPathFormat("PeopleMenu"), "_self");
                if (OfflineMode)
                    window.location = '/People/PeopleMenu?IsMobile=1';
                else
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
            });

            function ShowPersonNameInfo(strID) {
                var pData = {};
                pData.PeopleID = strID;

                ClearPeopleNameFields();
                AjaxSend({
                    url: "People/getPeopleInfoName",
                    data: pData,
                    success: function (vData) {

                        if (vData.PeopleNameData.Rows.length > 0) {

                            $("#txtID").val(vData.PeopleNameData.Rows[0].ID || '');

                            $("#txtTitle").val(vData.PeopleNameData.Rows[0].Title || '');
                            $("#txtSurname").val(vData.PeopleNameData.Rows[0].Surname || '');
                            $("#txtFirstName").val(vData.PeopleNameData.Rows[0].FirstName || '');
                            $("#txtMiddleName").val(vData.PeopleNameData.Rows[0].MiddleName || '');
                            $("#txtPersonalSuffixName").val(vData.PeopleNameData.Rows[0].PersonalSuffixName || '');
                            $("#txtProfSuffixName").val(vData.PeopleNameData.Rows[0].ProfSuffixName || '');

                            $("#txtDateOfBirth").val((vData.PeopleNameData.Rows[0].DateOfBirthMonth || '') + '/' + (vData.PeopleNameData.Rows[0].DateOfBirthDay || '') + '/' + (vData.PeopleNameData.Rows[0].DateOfBirthYear || ''));

                            $("#txtNickName").val(vData.PeopleNameData.Rows[0].NickName || '');

                            $("#txtMotherMaidenSurname").val(vData.PeopleNameData.Rows[0].MotherMaidenSurname || '');
                            $("#txtMotherMaidenFirstName").val(vData.PeopleNameData.Rows[0].MotherMaidenFirstName || '');
                            $("#txtMotherMaidenMiddleName").val(vData.PeopleNameData.Rows[0].MotherMaidenMiddleName || '');

                            getPeopleAliasList();
                            getPeopleAddressList();
                            ShowPersonPersonalInfo();
                            getPeopleContactTelList();
                            getPeopleEducList();
                            getPeopleEmpHistList();
                            getPeopleRelatedList();
                            ShowPersonPrimaryIDDetails();
                        }
                    }
                });

            };

            function ClearPeopleNameFields() {
                $("#txtID").val('');

                $("#txtTitle").val('');
                $("#txtSurname").val('');
                $("#txtFirstName").val('');
                $("#txtMiddleName").val('');
                $("#txtPersonalSuffixName").val('');
                $("#txtProfSuffixName").val('');

                $("#txtDateOfBirth").val('');

                $("#txtNickName").val('');

                $("#txtMotherMaidenSurname").val('');
                $("#txtMotherMaidenFirstName").val('');
                $("#txtMotherMaidenMiddleName").val('');

            }

            function getPeopleAliasList() {
                AjaxSend({
                    url: 'People/getPeopleAliasList',
                    success: function (vData) {
                        LoadPeopleAliasList(vData.PeopleAliasList.Rows);
                    }
                });
            }

            function LoadPeopleAliasList(vRows) {
                var pLV = $('#lstPeopleAlias');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<h3>')
                            .attr("style", "white-space:normal;")
                            .text(vRows[p].AliasName)
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function getPeopleAddressList() {
                AjaxSend({
                    url: 'People/getPeopleAddressList',
                    success: function (vData) {
                        LoadPeopleAddressList(vData.PeopleAddressList.Rows);
                    }
                });
            }

            function LoadPeopleAddressList(vRows) {
                var pLV = $('#lstPeopleAddress');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text((vRows[p].HouseBuildingNo == "" ? "" : vRows[p].HouseBuildingNo + ", ") + vRows[p].Street)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].City + ', ' + vRows[p].Country)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text('(' + vRows[p].AddressType + ')')
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function ShowPersonPersonalInfo() {

                ClearPersonPersonalInfoFields();

                AjaxSend({
                    url: "People/getPeoplePersonalInfo",
                    success: function (vData) {

                        if (vData.PeoplePersonalInfo.Rows.length > 0) {

                            $("#txtGender").val(vData.PeoplePersonalInfo.Rows[0].Gender || '');

                            $("#txtCitizenshipID").val(vData.PeoplePersonalInfo.Rows[0].CitizenshipID || '');
                            $("#txtCitizenship").val(vData.PeoplePersonalInfo.Rows[0].Citizenship || '');

                            $("#txtPOBCountryID").val(vData.PeoplePersonalInfo.Rows[0].PlaceOfBirthCountryID || '');
                            $("#txtPOBCountry").val(vData.PeoplePersonalInfo.Rows[0].PlaceOfBirthCountry || '');

                            $("#txtPOBCityID").val(vData.PeoplePersonalInfo.Rows[0].PlaceOfBirthCityID || '');
                            $("#txtPOBCity").val(vData.PeoplePersonalInfo.Rows[0].PlaceOfBirthCity || '');

                            var dblCM = parseFloat(vData.PeoplePersonalInfo.Rows[0].Height || '0');
                            var dblFT = 0;
                            var dblFTINC = 0;

                            dblFT = Math.floor(dblCM / 30.48);
                            dblFTINC = (dblCM - (dblFT * 30.48)) / 2.54;
                            $("#txtHeight").val(dblFT.toFixed(0) + " ' " + dblFTINC.toFixed(2) + ' " ');

                            $("#txtWeight").val((vData.PeoplePersonalInfo.Rows[0].Weight + " lbs") || '');

                            $("#txtCivilStatus").val(vData.PeoplePersonalInfo.Rows[0].CivilStatus || '');

                        }
                    }
                });

            };

            function ClearPersonPersonalInfoFields() {

                $("#txtGender").val('');

                $("#txtCitizenshipID").val('');
                $("#txtCitizenship").val('');

                $("#txtPOBCountryID").val('');
                $("#txtPOBCountry").val('');

                $("#txtPOBCityID").val('');
                $("#txtPOBCity").val('');

                $("#txtHeight").val('');
                $("#txtWeight").val('');

                $("#txtCivilStatus").val('');

            }

            function getPeopleContactTelList() {
                AjaxSend({
                    url: 'People/getPeopleContactTelList',
                    success: function (vData) {
                        LoadPeopleContactTelList(vData.PeopleContactList.Rows);
                        getPeopleContactMobileList();
                    }
                });
            }

            function LoadPeopleContactTelList(vRows) {
                var pLV = $('#lstPeopleContact');
                var pItem;

                pLV.html('');
                pItem = $('<li>')
                    .attr('data-role', "list-divider")
                    .text('Telephone');
                pLV.append(pItem);
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
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
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function getPeopleContactMobileList() {
                AjaxSend({
                    url: 'People/getPeopleContactMobileList',
                    success: function (vData) {
                        LoadPeopleContactMobileList(vData.PeopleContactList.Rows);
                        getPeopleContactEmailList();
                    }
                });
            }

            function LoadPeopleContactMobileList(vRows) {
                var pLV = $('#lstPeopleContact');
                var pItem;

                pItem = $('<li>')
                    .attr('data-role', "list-divider")
                    .text('Mobile');
                pLV.append(pItem);
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
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
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
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
                var pLV = $('#lstPeopleContact');
                var pItem;

                pItem = $('<li>')
                    .attr('data-role', "list-divider")
                    .text('Email');
                pLV.append(pItem);
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].EmailAddress)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].EmailType)
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

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

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {

                    pItem = $('<li>')
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
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function getPeopleEmpHistList() {
                AjaxSend({
                    url: 'People/getPeopleEmpHistList',
                    success: function (vData) {
                        LoadPeopleEmpHistList(vData.PeopleEmpHistList.Rows);
                    }
                });
            }

            function LoadPeopleEmpHistList(vRows) {
                var pLV = $('#lstPeopleEmpHist');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {

                    pItem = $('<li>')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].CompanyName)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text($.format.date(new Date(vRows[p].DateStart), 'MMM dd, yyyy') + ' - ' + (vRows[p].IsUntilPresent == "0" ? $.format.date(new Date(vRows[p].DateEnd), 'MMM dd, yyyy') : 'Present'))
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

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

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {

                    pItem = $('<li>')
                    .append($('<h3>')
                        .attr("style", "white-space:normal")
                        .text(vRows[p].RelatedName)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text((vRows[p].IsEmergencyRef == 1 ? 'Emergency Reference' : ''))
                    );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function ShowPersonPrimaryIDDetails() {

                ClearPersonPrimaryIDFields();

                AjaxSend({
                    url: "People/getPeoplePrimaryID",
                    success: function (vData) {

                        if (vData.PeoplePrimeIDData.Rows.length > 0) {

                            $("#txtResCertNo").val(vData.PeoplePrimeIDData.Rows[0].ResidenceCertNo || '');

                            $("#txtSSSNo").val(vData.PeoplePrimeIDData.Rows[0].SSSNo || '');
                            $("#txtPhilHealthNo").val(vData.PeoplePrimeIDData.Rows[0].PhilHealthNo || '');
                            $("#txtTIN").val(vData.PeoplePrimeIDData.Rows[0].TIN || '');
                            $("#txtHDMFNo").val(vData.PeoplePrimeIDData.Rows[0].HDMFNo || '');
                            $("#txtNBINo").val(vData.PeoplePrimeIDData.Rows[0].NBINo || '');

                            $("#txtPassportNo").val(vData.PeoplePrimeIDData.Rows[0].PassportNo || '');

                        }
                    }
                });

            };

            function ClearPersonPrimaryIDFields() {

                $("#txtResCertNo").val('');

                $("#txtSSSNo").val('');
                $("#txtPhilHealthNo").val('');
                $("#txtTIN").val('');
                $("#txtHDMFNo").val('');
                $("#txtNBINo").val('');

                $("#txtPassportNo").val('');
            }
        });
    });