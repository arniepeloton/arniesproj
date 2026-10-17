require([
    'common'
    ], function () {
        $(function () {
            var ctrEmail = 1;
            var ctrTelephone = 1;
            var ctrMobile = 1;
            var ctrBusinessPermit = 1;
            var ctrAddresses = 0;
            var ctrRegistration = 1;
            var ctrSection = 1;

            // dont clear
            // sessionStorage.clear();
            proj.ClearSessionStorage();

            getPropertyType();
            getWarehouseType();
            getRetailStoreType();

            $('#dvDeliveryEquiptmentInfo').hide();
            $("#dvContractStart").hide();
            $("#dvContractEnd").hide();
            $("#dvCostPerMonth").hide();

            $("#dvLTORegistrationNumber").hide();
            $("#dvGrossWeight").hide();
            $("#dvNetWeight").hide();
            $("#dvGarageAddress").hide();
            $("#dvLTORegistrationNumber").hide();
            $("#dvGarageAddressButton").hide();

            $("#dvWarehouseType").hide();

            $('#btnDone').click(function () {
                savePropertyAndPlant();
            });

            //Property And Plant
            $("#butSections").click(function () {
                $('#txtSectionCode').val('');
                $('#txtSectionName').val('');
                $('#txtSectionDescription').val('');
                $('#txtSectionFloorArea').val('');
                $('#txtSectionSpecialFeatures').val('');
            })

            $('#butTelephoneCountryCode').click(function () {
                Browser.LibraryCountries.Click = function (e) {
                    $('#butTelephoneCountryCode').text(e.CallingCode)
                    $('#butTelephoneCountryCode').data('ID', e.ID)
                .button('refresh');
                };
                Browser.LibraryCountries.Show();
            });

            $('#butTelephoneAreaCode').click(function () {
                Browser.LibraryAreaCodes.Click = function (e) {
                    $('#butTelephoneAreaCode').text(e.CallingCode)
                    $('#butTelephoneAreaCode').data('ID', e.ID)
                .button('refresh');
                };
                Browser.LibraryAreaCodes.Show();
            });

            $('#butCityorTown').click(function () {
                Browser.LibraryCities.Click = function (e) {
                    $('#butCityorTown').text(e.Name)
                    $('#butCityorTown').data('ID', e.ID)
                .button('refresh');
                };
                Browser.LibraryCities.Show();
            });

            $('#butCountry').click(function () {
                Browser.LibraryCountries.Click = function (e) {
                    $('#butCountry').text(e.Name)
                    $('#butCountry').data('ID', e.ID)
                .button('refresh');
                };
                Browser.LibraryCountries.Show();
            });

            $('#butMobileCountryCode').click(function () {
                Browser.LibraryCountries.Click = function (e) {
                    $('#butMobileCountryCode').text(e.CallingCode)
                    $('#butMobileCountryCode').data('ID', e.ID)
                .button('refresh');
                };
                Browser.LibraryCountries.Show();
            });

            $('#butMobileTelcoCode').click(function () {
                Browser.LibraryTelcoPrefixes.Click = function (e) {
                    $('#butMobileTelcoCode').text(e.Prefix)
                    $('#butMobileTelcoCode').data('ID', e.ID)
                .button('refresh');
                };
                Browser.LibraryTelcoPrefixes.Show();
            });

            $("#cmbType").change(function () {

                if ($("#cmbType option:selected").text() == 'Branch') {

                    $('#dvDeliveryEquiptmentInfo').hide();
                    $("#dvSections").show();
                    $('#dvRetailStore').show();
                    $('#dvWarehouseType').hide();

                }
                else if ($("#cmbType option:selected").text() == 'Office') {
                    $('#dvDeliveryEquiptmentInfo').hide();
                    $("#dvSections").show();
                    $('#dvRetailStore').hide();
                    $('#dvWarehouseType').hide();

                }
                else if ($("#cmbType option:selected").text() == 'Delivery Equipment') {
                    $('#dvDeliveryEquiptmentInfo').show();
                    $("#dvSections").hide();
                    $('#dvRetailStore').hide();
                    $('#dvWarehouseType').hide();
                }
                else {
                    $('#dvDeliveryEquiptmentInfo').hide();
                    $("#dvSections").show();
                    $('#dvRetailStore').hide();
                    $('#dvWarehouseType').show();
                }
            });

            $("#cmbOwnershipType").change(function () {
                if ($(this).val() == 'Owned') {
                    $("#dvContractStart").hide();
                    $("#dvContractEnd").hide();
                    $("#dvCostPerMonth").hide();
                }
                else {
                    $("#dvContractStart").show();
                    $("#dvContractEnd").show();
                    $("#dvCostPerMonth").show();
                }
            });

            setMonthCollection("selContractStartMonth");
            setYearCollection("selContractStartYear");

            setMonthCollection("selContractEndMonth");
            setYearCollection("selContractEndYear");

            $('#selContractStartMonth').change(function (e) {
                setDayCollection('selContractStartMonth', 'selContractStartDay', 'selContractStartYear', true);
            });
            $('#selContractStartYear').change(function (e) {
                setDayCollection('selContractStartMonth', 'selContractStartDay', 'selContractStartYear', true);
            });

            $('#selContractEndMonth').change(function (e) {
                setDayCollection('selContractEndMonth', 'selContractEndDay', 'selContractEndYear', true);
            });
            $('#selContractEndYear').change(function (e) {
                setDayCollection('selContractEndMonth', 'selContractEndDay', 'selContractEndYear', true);
            });

            //Registration
            setMonthCollection("selRegistrationDateofIssueMonth");
            setYearCollection("selRegistrationDateofIssueYear");

            $('#selRegistrationDateofIssueMonth').change(function (e) {
                setDayCollection('selRegistrationDateofIssueMonth', 'selRegistrationDateofIssueDay', 'selRegistrationDateofIssueYear', true);
            });
            $('#selRegistrationDateofIssueYear').change(function (e) {
                setDayCollection('selRegistrationDateofIssueMonth', 'selRegistrationDateofIssueDay', 'selRegistrationDateofIssueYear', true);
            });

            setMonthCollection("selRegistrationExpirationDateMonth");
            setYearCollection("selRegistrationExpirationDateYear");

            $('#selRegistrationExpirationDateMonth').change(function (e) {
                setDayCollection('selRegistrationExpirationDateMonth', 'selRegistrationExpirationDateDay', 'selRegistrationExpirationDateYear', true);
            });
            $('#selRegistrationExpirationDateYear').change(function (e) {
                setDayCollection('selRegistrationExpirationDateMonth', 'selRegistrationExpirationDateDay', 'selRegistrationExpirationDateYear', true);
            });

            //Business Permit
            setMonthCollection("selDOPMonth");
            setYearCollection("selDOPYear");

            setMonthCollection("selDIMonth");
            setYearCollection("selDIYear");


            $('#selDOPYear').click(function (e) {
                if ($.trim($('#selDOPYear').val()) == "") {
                    $('#selDOPYear').val('1980');
                    $('#selDOPYear').selectmenu('refresh');
                }
            });

            $('#selDOPMonth').change(function (e) {
                setDayCollection('selDOPMonth', 'selDOPDay', 'selDOPYear', true);
            });

            $('#selDOPYear').change(function (e) {
                setDayCollection('selDOPMonth', 'selDOPDay', 'selDOPYear', true);
            });

            $('#selDIMonth').change(function (e) {
                setDayCollection('selDIMonth', 'selDIDay', 'selDIYear', true);
            });

            $('#selDIYear').change(function (e) {
                setDayCollection('selDIMonth', 'selDIDay', 'selDIYear', true);
            });


            $('#butGarageCityorTown').click(function () {
                Browser.LibraryCities.Click = function (e) {
                    $('#butGarageCityorTown').text(e.Name)
                    $('#butGarageCityorTown').data('ID', e.ID)
                .button('refresh');
                };
                Browser.LibraryCities.Show();
            });

            $('#butGarageCountry').click(function () {
                Browser.LibraryCountries.Click = function (e) {
                    $('#butGarageCountry').text(e.Name)
                    $('#butGarageCountry').data('ID', e.ID)
                .button('refresh');
                };
                Browser.LibraryCountries.Show();
            });

            $('#btnAddressesOk').click(function () {
                var pLV = $("#lstGarageAddress");
                var pItem;

                var data = {
                    AddressNo: ctrAddresses,
                    StreetName: $("#txtGarageStreet").val(),
                    CityID: $("#butGarageCityorTown").data('ID'),
                    City: $("#butGarageCityorTown").text(),
                    CountryID: $("#butGarageCountry").data('ID'),
                    Country: $("#butGarageCountry").text()
                }

                pItem = $('<li id=lstGarageAddress' + ctrAddresses + '>')
                    .append($('<a>')
                        .attr('href', '#')
                        .attr('ID', '')
                        .data('data', data)
                        .click(function (e) {
                            //                            $(this).remove();

                            $("#txtGarageStreet").val(data["StreetName"]);
                            $("#butGarageCityorTown").data('ID', data["CityID"]);
                            $("#butGarageCityorTown").text(data["City"]);
                            $("#butGarageCountry").data('ID', data["CountryID"]);
                            $("#butGarageCountry").text(data["Country"]);

                            $("#butGarageCityorTown").button('refresh');
                            $("#butGarageCountry").button('refresh');

                            $.mobile.changePage("#Addresses");
                        })
                        .append($('<h3>')
                            .text($("#txtGarageStreet").val() + ', ' + $("#butGarageCityorTown").text() + ', ' + $("#butGarageCountry").text())
                        )
                    )
                    .append($('<a>')
                    .attr('href', '#')
                    .attr('ID', ctrAddresses)
                    .click(function () {
                        $("#lstGarageAddress" + $(this).attr("ID")).remove();
                    })
                    .append($('<h3>')
                            .text('')
                        )
                    );


                pLV.append(pItem);
                pLV.listview('refresh');
                ctrAddresses++;

                $.mobile.changePage("#BusinessPermit");
            })

            $('#btnBusinessPermitOk').click(function () {
                var pLV = $("#lstBusinessPermit");
                var pItem;

                var ctr = 0;
                var m_Data = [];

                var data = {
                    BusinessPermitNumber: $("#txtBusinessPermitNumber").val(),
                    BusinessID: $("#txtBusinessID").val(),
                    TradeName: $("#txtTradeName").val(),
                    BuildingHouseNo: $("#txtBusinessPermitHouseNo").val(),
                    BusinessPermitStreet: $("#txtBusinessPermitStreet").val(),
                    BusinessPermitCityorTown: $("#butBusinessPermitCityorTown").text(),
                    BusinessPermitCityorTownID: $("#butBusinessPermitCityorTown").data('ID'),
                    BusinessPermitCountry: $("#butBusinessPermitCountry").text(),
                    BusinessPermitCountryID: $("#butBusinessPermitCountry").data('ID'),
                    OfficialReceiptNumber: $("#txtOfficialReceiptNumber").val(),
                    BusinessPlateNo: $("#txtBusinessPlateNo").val(),
                    DateofPayment: $("#selDOPMonth").val() + "/" + $("#selDOPDay").val() + "/" + $("#selDOPYear").val(),
                    DateIssued: $("#selDIMonth").val() + "/" + $("#selDIDay").val() + "/" + $("#selDIYear").val()
                }

                pItem = $('<li id=lstBusinessPermit' + ctrBusinessPermit + '>')
                    .append($('<a>')
                        .attr('href', '#')
                        .attr('ID', '')
                        .data('data', data)
                        .click(function (e) {

                            $(this).remove();
                            $("#" + ctrBusinessPermit).remove();

                            $("#txtBusinessPermitNumber").val(data["BusinessPermitNumber"]);
                            $("#txtBusinessID").val(data["BusinessID"]);
                            $("#txtTradeName").val(data["TradeName"]);
                            $("#txtBusinessPermitHouseNo").val(data["BuildingHouseNo"]);
                            $("#txtBusinessPermitStreet").val(data["BusinessPermitStreet"]);

                            $("#butBusinessPermitCityorTown").text(data["BusinessPermitCityorTown"]);
                            $("#butBusinessPermitCityorTown").data('ID', data["BusinessPermitCityorTownID"]);
                            $("#butBusinessPermitCountry").text(data["BusinessPermitCountry"]);
                            $("#butBusinessPermitCountry").data('ID', data["BusinessPermitCountryID"]);

                            $("#butBusinessPermitCityorTown").button('refresh');
                            $("#butBusinessPermitCountry").button('refresh');

                            $("#txtOfficialReceiptNumber").val(data["OfficialReceiptNumber"]);
                            $("#txtBusinessPlateNo").val(data["BusinessPlateNo"]);

                            var m_Date = data["DateofPayment"].split('/');
                            $("#selDOPMonth").val(m_Date[0]);
                            $("#selDOPDay").val(m_Date[1]);
                            $("#selDOPYear").val(m_Date[2]);

                            $("#selDOPMonth").selectmenu('refresh');
                            $("#selDOPDay").selectmenu('refresh');
                            $("#selDOPYear").selectmenu('refresh');

                            m_Date = data["DateIssued"].split('/');

                            $("#selDIMonth").val(m_Date[0]);
                            $("#selDIDay").val(m_Date[1]);
                            $("#selDIYear").val(m_Date[2]);

                            $("#selDIMonth").selectmenu('refresh');
                            $("#selDIDay").selectmenu('refresh');
                            $("#selDIYear").selectmenu('refresh');

                            $.mobile.changePage("#BusinessPermit");
                        })
                        .append($('<h3>')
                            .text($('#txtBusinessPermitNumber').val())
                        )
                    )
                    .append($('<a>')
                    .attr('href', '#')
                    .attr('ID', ctrBusinessPermit)
                    .click(function () {
                        $("#lstBusinessPermit" + $(this).attr("ID")).remove();
                    })
                    .append($('<h3>')
                            .text('')
                        )
                    );

                pLV.append(pItem);
                pLV.listview('refresh');
                ctrBusinessPermit++;

                $.mobile.changePage("#PropertyAndPlant");
            })

            $("#btnSectionsOk").click(function () {
                var pLV = $("#lstSection");
                var pItem;
                var data = {
                    SectionCode: $('#txtSectionCode').val(),
                    SectionName: $('#txtSectionName').val(),
                    SectionDescription: $('#txtSectionDescription').val(),
                    SectionFloorArea: $('#txtSectionFloorArea').val(),
                    SectionSpecialFeatures: $('#txtSectionSpecialFeatures').val()
                }


                pItem = $('<li id=lstSection' + ctrSection + '>')
                .append($('<a>')
                    .attr('href', '#')
                    .attr('ID', '')
                    .data('data', data)
                    .click(function (e) {
                        $(this).remove();
                        $('#txtSectionCode').val(data["SectionCode"])
                        $('#txtSectionName').val(data["SectionName"])
                        $('#txtSectionDescription').val(data["SectionDescription"])
                        $('#txtSectionFloorArea').val(data["SectionFloorArea"])
                        $('#txtSectionSpecialFeatures').val(data["SectionSpecialFeatures"])

                        $.mobile.changePage("#Sections");
                    })
                    .append($('<h3>')
                        .text($('#txtSectionCode').val() + '-' + $('#txtSectionName').val())
                    )
                )
                .append($('<a>')
                .attr('href', '#')
                .attr('ID', ctrSection)
                .click(function () {
                    $("#lstSection" + $(this).attr("ID")).remove();
                })
                .append($('<h3>')
                        .text('')
                    )
                );

                pLV.append(pItem);
                pLV.listview('refresh');
                ctrSection++;

                $.mobile.changePage("#PropertyAndPlant");

            });

            $('#butBusinessPermitCityorTown').click(function () {
                Browser.LibraryCities.Click = function (e) {
                    // user clicked an item
                    $('#butBusinessPermitCityorTown').text(e.Name)
                    $('#butBusinessPermitCityorTown').data('ID', e.ID)
                .button('refresh');
                };
                Browser.LibraryCities.Show();
            });

            $('#butBusinessPermitCountry').click(function () {
                Browser.LibraryCountries.Click = function (e) {
                    // user clicked an item
                    $('#butBusinessPermitCountry').text(e.Name)
                    $('#butBusinessPermitCountry').data('ID', e.ID)
                .button('refresh');
                };
                Browser.LibraryCountries.Show();
            });


            //Mobile
            $("#btnMobileOk").click(function (e) {
                var pLV = $("#lstMobile");
                var pItem;
                var data = {
                    MobileCountryCodeID: $('#butMobileCountryCode').data('ID'),
                    MobileCountryCode: $('#butMobileCountryCode').text(),
                    MobileTelcoCodeID: $('#butMobileTelcoCode').data('ID'),
                    MobileTelcoCode: $('#butMobileTelcoCode').text(),
                    MobileNumber: $('#txtMobileNumber').val()
                }


                if ($('#butMobileCountryCode').text() != 'Country Code'
                && $('#butMobileTelcoCode').text() != 'Telco Code'
                && $('#txtMobileNumber').val() != '') {

                    pItem = $('<li id=lstMobile' + ctrMobile + '>')
                    .append($('<a>')
                        .attr('href', '#')
                        .attr('ID', '')
                        .data('data', data)
                        .click(function (e) {
                            $(this).remove();
                            $('#butMobileCountryCode').data('ID', data["MobileCountryCodeID"])
                            $('#butMobileCountryCode').text(data["MobileCountryCode"])
                            $('#butMobileTelcoCode').data('ID', data["MobileTelcoCodeID"])
                            $('#butMobileTelcoCode').text(data["MobileTelcoCode"])
                            $('#txtMobileNumber').val(data["MobileNumber"])

                            $('#butMobileCountryCode').button('refresh');
                            $('#butMobileTelcoCode').button('refresh');
                            $.mobile.changePage("#Mobile");
                        })
                        .append($('<h3>')
                            .text($('#butMobileCountryCode').text() + $('#butMobileTelcoCode').text() + '-'
                                    + $('#txtMobileNumber').val())
                        )
                    )
                    .append($('<a>')
                    .attr('href', '#')
                    .attr('ID', ctrMobile)
                    .click(function () {
                        $("#lstMobile" + $(this).attr("ID")).remove();
                    })
                    .append($('<h3>')
                            .text('')
                        )
                    );

                    pLV.append(pItem);
                    pLV.listview('refresh');
                    ctrMobile++;

                    $.mobile.changePage("#PropertyAndPlant");
                }
                else {
                    e.preventDefault();
                    MessageBox("Please enter valid Telephone No.", "Telephone No");
                }
            });

            //Telephone
            $("#btnTelephoneOk").click(function (e) {
                var pLV = $("#lstTelephone");
                var pItem;
                var data = {
                    TelephoneCountryCodeID: $('#butTelephoneCountryCode').data('ID'),
                    TelephoneCountryCode: $('#butTelephoneCountryCode').text(),
                    TelephoneAreaCodeID: $('#butTelephoneAreaCode').data('ID'),
                    TelephoneAreaCode: $('#butTelephoneAreaCode').text(),
                    TelephoneNumber: $('#txtTelephoneNumber').val(),
                    TelephoneLocal: $('#txtTelephoneLocal').val()
                }

                if ($('#butTelephoneCountryCode').text() != 'Country Code'
                && $('#butTelephoneAreaCode').text() != 'Telco Code'
                && $('#txtTelephoneNumber').val() != '') {

                    pItem = $('<li id=lstTelephone' + ctrTelephone + '>')
                    .append($('<a>')
                        .attr('href', '#')
                        .attr('ID', '')
                        .data('data', data)
                        .click(function (e) {
                            $(this).remove();

                            $('#butTelephoneCountryCode').data('ID', data['TelephoneCountryCodeID']),
                            $('#butTelephoneCountryCode').text(data['TelephoneCountryCode']),
                            $('#butTelephoneAreaCode').data('ID', data['TelephoneAreaCodeID']),
                            $('#butTelephoneAreaCode').text(data['TelephoneAreaCode']),
                            $('#txtTelephoneNumber').val(data['TelephoneNumber']),
                            $('#txtTelephoneLocal').val(data['TelephoneLocal'])

                            $('#butTelephoneCountryCode').button('refresh');
                            $('#butTelephoneAreaCode').button('refresh');
                            $.mobile.changePage("#Telephone");

                        })
                        .append($('<h3>')
                            .text($('#butTelephoneCountryCode').text() + $('#butTelephoneAreaCode').text() + '-'
                                    + $('#txtTelephoneNumber').val() + ' loc ' + $('#txtTelephoneLocal').val())
                        )
                    )
                    .append($('<a>')
                    .attr('href', '#')
                    .attr('ID', ctrTelephone)
                    .click(function () {
                        $("#lstTelephone" + $(this).attr("ID")).remove();
                    })
                    .append($('<h3>')
                            .text('')
                        )
                    );

                    pLV.append(pItem);
                    pLV.listview('refresh');
                    ctrTelephone++;

                    $.mobile.changePage("#PropertyAndPlant");
                }
                else {
                    e.preventDefault();
                    MessageBox("Please enter valid Telephone No.", "Telephone No");
                }
            });

            //EMail
            $("#btnEMailOk").click(function (e) {
                var pLV = $("#lstEmail");
                var pItem;
                var data = { Email: $("#txtEMail").val() }

                if (IsEmail($("#txtEMail").val()) == true) {
                    //            pItem = $('<li id=lstEmail' + ctrEmail + '>')
                    pItem = $('<li>')
                    .append($('<a>')
                        .attr('href', '#EMail')
                        .attr('ID', 'lstEmailList')
                        .attr('data', data)
                        .click(function (e) {
                            $(this).remove();
                            $("#txtEMail").val(data["Email"]);
                        })
                        .append($('<h3>')
                            .text($("#txtEMail").val())
                        )
                    )
                    .append($('<a>')
                    .attr('href', '#')
                    .attr('ID', ctrEmail)
                    .click(function () {
                        $("#lstEmail" + $(this).attr("ID")).remove();
                    })
                    .append($('<h3>')
                            .text($("#txtEMail").val())
                        )
                    );

                    pLV.append(pItem);
                    pLV.listview('refresh');
                    ctrEmail++;

                    $.mobile.changePage("#PropertyAndPlant");
                }
                else {
                    e.preventDefault();
                    MessageBox("Please enter valid EMail address.", "EMail Address");
                }
            });

            var isLTO = 0;

            $("#btnRegistrationOk").click(function () {
                var pLV;
                if (LTO == 0) {
                    pLV = $("#lstLTO");
                }
                else {
                    pLV = $("#lstLTFRB");
                }

                var pItem;
                var data = {
                    Code: $("#txtSectionCode").val(),
                    Name: $("#txtSectionName").val(),
                    Description: $("#txtSectionDescription").val(),
                    FloorArea: $("#txtSectionFloorArea").val(),
                    SpecialFeatures: $("#txtSectionSpecialFeatures").val()
                }

                if (IsEmail($("#txtEMail").val()) == true) {
                    //            pItem = $('<li id=lstEmail' + ctrEmail + '>')
                    pItem = $('<li>')
                    .append($('<a>')
                        .attr('href', '')
                        .attr('ID', 'lstEmailList' + ctrRegistration)
                        .attr('data', data)
                        .click(function (e) {
                            $(this).remove();
                            $("#txtEMail").val(data["Email"]);
                        })
                        .append($('<h3>')
                            .text($("#txtEMail").val())
                        )
                    )
                    .append($('<a>')
                    .attr('href', '#')
                    .attr('ID', ctrEmail)
                    .click(function () {
                        $("#lstEmail" + $(this).attr("ID")).remove();
                    })
                    .append($('<h3>')
                            .text($("#txtSectionCode").val() + '-' + $("#txtSectionName").val())
                        )
                    );

                    pLV.append(pItem);
                    pLV.listview('refresh');
                    ctrRegistration++;

                    $.mobile.changePage("#PropertyAndPlant");
                }
                else {
                    e.preventDefault();
                    MessageBox("Please enter valid EMail address.", "EMail Address");
                }
            });

            $("#btnDone").click(function () {

                //        $("#lstEmail li[data-role!=list-divider]").each(function (index) {
                //            alert($(this).text() + ' ' + index);
                //        })
            })
            //    1 square meter = 10.7639104 square feet

            $(document).ready(function () {

                AjaxSend({
                    url: 'PropertyAndPlant/IsNewEntry',
                    success: function (vData) {

                        ShowPropertyAndPlantInfo();

                    }
                });

            });


            function getPropertyType() {
                //data: pData,
                AjaxSend({
                    url: 'Browser/LibraryPropertyAndPlantType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#cmbType').empty();

                        $('#cmbType').append(new Option(vRows[0].Name, vRows[0].ID, true, true));

                        for (var p = 1; p <= vRows.length - 1; p++) {
                            $('#cmbType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                        }

                        $('#cmbType').selectmenu('refresh');
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

            function getWarehouseType() {
                AjaxSend({
                    url: 'Browser/LibraryPropertyAndPlantWarehouseType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#cmbWarehouse').empty();

                        $('#cmbWarehouse').append(new Option(vRows[0].Name, vRows[0].ID, true, true));

                        for (var p = 1; p <= vRows.length - 1; p++) {
                            $('#cmbWarehouse').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                        }
                        $('#cmbWarehouse').selectmenu('refresh');
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

            function getRetailStoreType() {
                AjaxSend({
                    url: 'Browser/LibraryPropertyAndPlantRetailStoreType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#cmbRetailStore').empty();

                        $('#cmbRetailStore').append(new Option(vRows[0].Name, vRows[0].ID, true, true));

                        for (var p = 1; p <= vRows.length - 1; p++) {
                            $('#cmbRetailStore').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                        }
                        $('#cmbRetailStore').selectmenu('refresh');
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

            function ShowPropertyAndPlantInfo() {

                AjaxSend({
                    url: "PropertyAndPlant/getPropertyAndPlantInfo",
                    success: function (vData) {

                        if (vData.PropertyAndPlantData.Rows.length > 0) {

                            $("#cmbType").val(vData.PropertyAndPlantData.Rows[0].PropertyType || '').selectmenu('refresh');

                            $("#txtCode").val(vData.PropertyAndPlantData.Rows[0].Code || '');
                            $("#txtName").val(vData.PropertyAndPlantData.Rows[0].Name || '');
                            $("#txtDescription").val(vData.PropertyAndPlantData.Rows[0].Description || '');
                            $("#txtHouseNo").val(vData.PropertyAndPlantData.Rows[0].BuildingHouseNo || '');
                            $("#txtStreet").val(vData.PropertyAndPlantData.Rows[0].Street || '');
                            $('#butCityorTown').data('ID', vData.PropertyAndPlantData.Rows[0].CityID)
                            $("#butCityorTown").val(vData.PropertyAndPlantData.Rows[0].City || '').button('refresh');
                            $('#butCountry').data('ID', vData.PropertyAndPlantData.Rows[0].CountryID)
                            $("#butCountry").val(vData.PropertyAndPlantData.Rows[0].Country || '').button('refresh');

                            $("#cmbRetailStore").val(vData.PropertyAndPlantData.Rows[0].SubPropertyType || '').selectmenu('refresh');
                            $("#cmbWarehouse").val(vData.PropertyAndPlantData.Rows[0].SubPropertyType || '').selectmenu('refresh');

                            $("#txtPlateNo").val(vData.PropertyAndPlantData.Rows[0].PlateNo || '');
                            $("#txtNoofWheels").val(vData.PropertyAndPlantData.Rows[0].NoOfWheels || '0');
                            $("#txtBodyColor").val(vData.PropertyAndPlantData.Rows[0].BodyColor || '');
                            $("#txtLoadLimitWeight").val(vData.PropertyAndPlantData.Rows[0].Weight || '0');
                            $("#txtLoadLimitDimensions").val(vData.PropertyAndPlantData.Rows[0].Dimension || '');

                            $("#selContractStartMonth").val(vData.PropertyAndPlantData.Rows[0].ContractStartMonth || '').selectmenu('refresh');
                            $("#selContractStartYear").val(vData.PropertyAndPlantData.Rows[0].ContractStartYear || '').selectmenu('refresh');
                            setDayCollection('selContractStartMonth', 'selContractStartDay', 'selContractStartYear', true);
                            $("#selContractStartDay").val(vData.PropertyAndPlantData.Rows[0].ContractStartDay || '').selectmenu('refresh');

                            $("#selContractEndMonth").val(vData.PropertyAndPlantData.Rows[0].ContractEndMonth || '').selectmenu('refresh');
                            $("#selContractEndYear").val(vData.PropertyAndPlantData.Rows[0].ContractEndYear || '').selectmenu('refresh');
                            setDayCollection('selContractEndMonth', 'selContractEndDay', 'selContractEndYear', true);
                            $("#selContractEndDay").val(vData.PropertyAndPlantData.Rows[0].ContractEndDay || '').selectmenu('refresh');

                            $("#txtCostPerMonth").val(vData.PropertyAndPlantData.Rows[0].CostPerMonth || '0');
                            $("#optSlider").val(vData.PropertyAndPlantData.Rows[0].IsMortgaged || '0').slider("refresh");

                            getSectionList();
                            getBusinessPermitList();
                            getTelList();
                            getMobileList();
                            getEmailList();

                        }
                    }
                });

            };

            function getSectionList() {

                AjaxSend({
                    url: 'PropertyAndPlant/getSectionList',
                    success: function (vData) {
                        LoadSectionList(vData.PropertyAndPlantData.Rows);
                    }
                });
            }

            function LoadSectionList(vRows) {
                var pLV = $('#lstSection');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {

                    pItem = $('<li id=lstSection' + ctrSection + '>')
                        .append($('<a>')
                            .attr('href', '#')
                            .attr('ID', vRows[p].ID)
                            .data('data', vRows[p])
                            .click(function (e) {
                                $(this).remove();
                                $('#txtSectionCode').val(data["SectionCode"])
                                $('#txtSectionName').val(data["SectionName"])
                                $('#txtSectionDescription').val(data["SectionDescription"])
                                $('#txtSectionFloorArea').val(data["SectionFloorArea"])
                                $('#txtSectionSpecialFeatures').val(data["SectionSpecialFeatures"])

                                $.mobile.changePage("#Sections");
                            })
                            .append($('<h3>')
                                .text(vRows[p].SectionCode + '-' + vRows[p].SectionName)
                            )
                        )
                        .append($('<a>')
                        .attr('href', '#')
                        .attr('ID', p)
                        .click(function () {
                            $("#lstSection" + $(this).attr("ID")).remove();
                        })
                        .append($('<h3>')
                                .text('')
                        )
                    );

                    pLV.append(pItem);
                    ctrSection++;
                };

                pLV.listview('refresh');

            };

            function getBusinessPermitList() {

                AjaxSend({
                    url: 'PropertyAndPlant/getBusinessPermitList',
                    success: function (vData) {
                        LoadBusinessPermitList(vData.PropertyAndPlantData.Rows);
                    }
                });
            }

            function LoadBusinessPermitList(vRows) {
                var pLV = $('#lstBusinessPermit');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {

                    pItem = $('<li id=lstBusinessPermit' + ctrBusinessPermit + '>')
                        .append($('<a>')
                            .attr('href', '#')
                            .attr('ID', '')
                            .data('data', vRows[p])
                            .click(function (e) {

                                $(this).remove();

                                $("#txtBusinessPermitNumber").val(data["BusinessPermitNumber"]);
                                $("#txtBusinessID").val(data["BusinessID"]);
                                $("#txtTradeName").val(data["TradeName"]);
                                $("#txtBusinessPermitHouseNo").val(data["BuildingHouseNo"]);
                                $("#txtBusinessPermitStreet").val(data["BusinessPermitStreet"]);

                                $("#butBusinessPermitCityorTown").text(data["BusinessPermitCityorTown"]);
                                $("#butBusinessPermitCityorTown").data('ID', data["BusinessPermitCityorTownID"]);
                                $("#butBusinessPermitCountry").text(data["BusinessPermitCountry"]);
                                $("#butBusinessPermitCountry").data('ID', data["BusinessPermitCountryID"]);

                                $("#butBusinessPermitCityorTown").button('refresh');
                                $("#butBusinessPermitCountry").button('refresh');

                                $("#txtOfficialReceiptNumber").val(data["OfficialReceiptNumber"]);
                                $("#txtBusinessPlateNo").val(data["BusinessPlateNo"]);

                                var m_Date = data["DateofPayment"].split('/');
                                $("#selDOPMonth").val(m_Date[0]);
                                $("#selDOPDay").val(m_Date[1]);
                                $("#selDOPYear").val(m_Date[2]);

                                $("#selDOPMonth").selectmenu('refresh');
                                $("#selDOPDay").selectmenu('refresh');
                                $("#selDOPYear").selectmenu('refresh');

                                m_Date = data["DateIssued"].split('/');

                                $("#selDIMonth").val(m_Date[0]);
                                $("#selDIDay").val(m_Date[1]);
                                $("#selDIYear").val(m_Date[2]);

                                $("#selDIMonth").selectmenu('refresh');
                                $("#selDIDay").selectmenu('refresh');
                                $("#selDIYear").selectmenu('refresh');

                                $.mobile.changePage("#BusinessPermit");
                            })
                            .append($('<h3>')
                                .text(vRows[p].BusinessPermitNumber)
                            )
                        )
                        .append($('<a>')
                        .attr('href', '#')
                        .attr('ID', ctrBusinessPermit)
                        .click(function () {
                            $("#lstBusinessPermit" + $(this).attr("ID")).remove();
                        })
                        .append($('<h3>')
                            .text(vRows[p].BusinessPermitNumber)
                        )
                    );

                    pLV.append(pItem);
                    ctrBusinessPermit++;
                };

                pLV.listview('refresh');

            };

            function getTelList() {

                AjaxSend({
                    url: 'PropertyAndPlant/getTelephoneList',
                    success: function (vData) {
                        LoadTelList(vData.PropertyAndPlantData.Rows);
                    }
                });
            }

            function LoadTelList(vRows) {
                var pLV = $('#lstTelephone');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {

                    pItem = $('<li id=lstTelephone' + ctrTelephone + '>')
                        .append($('<a>')
                            .attr('href', '#')
                            .attr('ID', '')
                            .data('data', vRows[p])
                            .click(function (e) {
                                $(this).remove();

                                $('#butTelephoneCountryCode').data('ID', data['TelephoneCountryCodeID']),
                                $('#butTelephoneCountryCode').text(data['TelephoneCountryCode']),
                                $('#butTelephoneAreaCode').data('ID', data['TelephoneAreaCodeID']),
                                $('#butTelephoneAreaCode').text(data['TelephoneAreaCode']),
                                $('#txtTelephoneNumber').val(data['TelephoneNumber']),
                                $('#txtTelephoneLocal').val(data['TelephoneLocal'])

                                $('#butTelephoneCountryCode').button('refresh');
                                $('#butTelephoneAreaCode').button('refresh');
                                $.mobile.changePage("#Telephone");

                            })
                            .append($('<h3>')
                                .text(vRows[p].TelephoneCountryCode + vRows[p].TelephoneAreaCode + '-'
                                        + vRows[p].TelephoneNumber + ' loc ' + vRows[p].TelephoneLocal)
                            )
                        )
                        .append($('<a>')
                        .attr('href', '#')
                        .attr('ID', ctrTelephone)
                        .click(function () {
                            $("#lstTelephone" + $(this).attr("ID")).remove();
                        })
                        .append($('<h3>')
                            .text(vRows[p].TelephoneCountryCode + vRows[p].TelephoneAreaCode + '-'
                                    + vRows[p].TelephoneNumber + ' loc ' + vRows[p].TelephoneLocal)
                        )
                    );


                    pLV.append(pItem);
                    ctrTelephone++;
                };

                pLV.listview('refresh');

            };

            function getMobileList() {

                AjaxSend({
                    url: 'PropertyAndPlant/getMobileList',
                    success: function (vData) {
                        LoadMobileList(vData.PropertyAndPlantData.Rows);
                    }
                });
            }

            function LoadMobileList(vRows) {
                var pLV = $('#lstMobile');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {

                    pItem = $('<li id=lstMobile' + ctrMobile + '>')
                        .append($('<a>')
                            .attr('href', '#')
                            .attr('ID', '')
                            .data('data', vRows[p])
                            .click(function (e) {
                                $(this).remove();
                                $('#butMobileCountryCode').data('ID', data["MobileCountryCodeID"])
                                $('#butMobileCountryCode').text(data["MobileCountryCode"])
                                $('#butMobileTelcoCode').data('ID', data["MobileTelcoCodeID"])
                                $('#butMobileTelcoCode').text(data["MobileTelcoCode"])
                                $('#txtMobileNumber').val(data["MobileNumber"])

                                $('#butMobileCountryCode').button('refresh');
                                $('#butMobileTelcoCode').button('refresh');
                                $.mobile.changePage("#Mobile");
                            })
                            .append($('<h3>')
                                .text('+' + vRows[p].MobileCountryCode + ' . ' + vRows[p].MobileTelcoCode + ' . '
                                        + vRows[p].MobileNumber)
                            )
                        )
                        .append($('<a>')
                        .attr('href', '#')
                        .attr('ID', ctrMobile)
                        .click(function () {
                            $("#lstMobile" + $(this).attr("ID")).remove();
                        })
                        .append($('<h3>')
                            .text('+' + vRows[p].MobileCountryCode + ' . ' + vRows[p].MobileTelcoCode + ' . '
                                    + vRows[p].MobileNumber)
                        )
                    );



                    pLV.append(pItem);
                    ctrMobile++;
                };

                pLV.listview('refresh');

            };

            function getEmailList() {

                AjaxSend({
                    url: 'PropertyAndPlant/getEmailList',
                    success: function (vData) {
                        LoadEmailList(vData.PropertyAndPlantData.Rows);
                    }
                });
            }

            function LoadEmailList(vRows) {
                var pLV = $('#lstEmail');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {

                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('href', '#EMail')
                            .attr('ID', 'lstEmailList')
                            .attr('data', vRows[p])
                            .click(function (e) {
                                $(this).remove();
                                $("#txtEMail").val(data["Email"]);
                            })
                            .append($('<h3>')
                                .text(vRows[p].Email)
                            )
                        )
                        .append($('<a>')
                        .attr('href', '#')
                        .attr('ID', ctrEmail)
                        .click(function () {
                            $("#lstEmail" + $(this).attr("ID")).remove();
                        })
                        .append($('<h3>')
                                .text(vRows[p].Email)
                        )
                    );

                    pLV.append(pItem);
                    ctrEmail++;
                };

                pLV.listview('refresh');

            };













            function savePropertyAndPlant() {

                var subPropertyType = ""

                if ($("#cmbType option:selected").text() == 'Branch') {
                    subPropertyType = $("#cmbRetailStore").val()
                }
                else if ($("#cmbType option:selected").text() == 'Warehouse') {
                    subPropertyType = $("#cmbWarehouse").val()
                }

                var ctr = 0;
                var m_EMail = [];
                var m_Telephone = [];
                var m_Mobile = [];
                var m_BusinessPermit = [];
                var m_Section = [];

                $("#lstEmail li").each(function (m_index) {
                    m_EMail[ctr] = { EMail: $(this).text() };
                    ctr++;
                });

                ctr = 0;
                $("#lstTelephone li a").each(function (m_index) {
                    m_Telephone[ctr] = { Telephone: JSON.stringify($(this).data('data')) };
                    ctr++;
                });

                ctr = 0;
                $("#lstMobile li a").each(function (m_index) {
                    m_Mobile[ctr] = { Mobile: JSON.stringify($(this).data('data')) };
                    ctr++;
                });

                ctr = 0;
                $("#lstBusinessPermit li a").each(function (m_index) {
                    m_BusinessPermit[ctr] = { BusinessPermit: JSON.stringify($(this).data('data')) };
                    ctr++;
                });

                ctr = 0;
                $("#lstSection li a").each(function (m_index) {
                    m_Section[ctr] = { Sections: JSON.stringify($(this).data('data')) };
                    ctr++;
                });

                var data = {
                    PropertyType: $("#cmbType").val(),
                    Code: $("#txtCode").val(),
                    Name: $("#txtName").val(),
                    Description: $("#txtDescription").val(),
                    Street: $("#txtStreet").val(),
                    CityID: $("#butCityorTown").data('ID'),
                    CountryID: $("#butCountry").data('ID'),
                    SubPropertyType: subPropertyType,
                    PlateNo: $("#txtPlateNo").val(),
                    NoOfWheels: $("#txtNoofWheels").val(),
                    BodyColor: $("#txtBodyColor").val(),
                    Weight: $("#txtLoadLimitWeight").val(),
                    Dimension: $("#txtLoadLimitDimensions").val(),
                    OwnershipType: $("#cmbOwnershipType").val(),
                    ContractStart: $("#selContractStartMonth").val() + "/" + $("#selContractStartDay").val() + "/" + $("#selContractStartYear").val(),
                    ContractEnd: $("#selContractEndMonth").val() + "/" + $("#selContractEndDay").val() + "/" + $("#selContractEndYear").val(),
                    CostPerMonth: $("#txtCostPerMonth").val(),
                    IsMortgaged: $("#optSlider").val(),
                    EMail: JSON.stringify(m_EMail),
                    Telephone: JSON.stringify(m_Telephone),
                    Mobile: JSON.stringify(m_Mobile),
                    BusinessPermit: JSON.stringify(m_BusinessPermit),
                    Section: JSON.stringify(m_Section)
                }
                AjaxSend({
                    url: 'PropertyAndPlant/savePropertyAndPlant',
                    data: data,
                    success: function (vData) {
                        window.open("Organization/OrganizationBranch", "_self");
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

        });

    });


