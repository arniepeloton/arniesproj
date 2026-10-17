$(function () {

    var IsDone;

    var IsNewTradeName = true;
    var arTradeName = new Array();
    var intTradeNameCntr = 0;
    var intTradeNameIndex = 0;

    var intTradeNameRecID = 0;
    var intTradeName = 1;
    var intTradeNameInitial = 2;
    var intTradeNameState = 3;

    var IsNewIndustryClass = true;
    var arIndustryClass = new Array();
    var intIndustryClassCntr = 0;
    var intIndustryClassIndex = 0;

    var intIndustryClassRecID = 0;
    var intIndustryClassID = 1;
    var intIndustryClass = 2;
    var intIndustryClassState = 3;

    $(document).ready(function () {

        IsDone = false;

        $("#btnAddTradeName > span").attr("style", "text-align:left;white-space:normal");

        AjaxSend({
            url: 'Organization/IsNewEntry',
            success: function (vData) {

                setMonthCollection("selRegistrationDateMonth");
                setYearCollection("selRegistrationDateYear");

                ShowOrganizationInfo(vData.OrganizationID)

            }
        });
    });

    $('#selRegistrationType').change(function (e) {
        e.preventDefault();

        SetRegistrationInfoLabels();

    });

    $('#btnOfficeAddressCity').click(function () {

        Browser.LibraryCities.Click = function (e) {

            $('#txtOfficeAddressCityID').val(e.ID)
            $('#btnOfficeAddressCity').text(e.Name)
                .button('refresh');

        };
        Browser.LibraryCities.Show();

    });

    $('#btnOfficeAddressCountry').click(function () {

        Browser.LibraryCountries.Click = function (e) {

            $('#txtOfficeAddressCountryID').val(e.ID)
            $('#btnOfficeAddressCountry').text(e.Name)
                .button('refresh');

        };
        Browser.LibraryCountries.Show();

    });

    $('#chkSameAsPricipalAddress').change(function (e) {
        e.preventDefault();

        if ($(this).is(":checked")) {

            $("#txtBusinessAddressBuildingNo").val($("#txtOfficeAddressBuildingNo").val());
            $("#txtBusinessAddressStreet").val($("#txtOfficeAddressStreet").val());
            $("#txtBusinessAddressCityID").val($("#txtOfficeAddressCityID").val());
            $("#btnBusinessAddressCity").text($("#btnOfficeAddressCity").text()).button('refresh');
            $("#txtBusinessAddressCountryID").val($("#txtOfficeAddressCountryID").val());
            $("#btnBusinessAddressCountry").text($("#btnOfficeAddressCountry").text()).button('refresh');

        }

    });

    $('#btnBusinessAddressCity').click(function () {

        Browser.LibraryCities.Click = function (e) {

            $('#txtBusinessAddressCityID').val(e.ID)
            $('#btnBusinessAddressCity').text(e.Name)
                .button('refresh');

        };
        Browser.LibraryCities.Show();

    });

    $('#btnBusinessAddressCountry').click(function () {

        Browser.LibraryCountries.Click = function (e) {

            $('#txtBusinessAddressCountryID').val(e.ID)
            $('#btnBusinessAddressCountry').text(e.Name)
                .button('refresh');

        };
        Browser.LibraryCountries.Show();

    });

    $('#btnAddTradeName').click(function (e) {
        e.preventDefault();

        IsNewTradeName = true
        intTradeNameCntr = intTradeNameCntr + 1;
        intTradeNameIndex = intTradeNameCntr;

        $.mobile.changePage('#pageTradeName');
        ClearTradeNameFields();

    });

    $('#btnTradeNameSave').click(function (e) {
        e.preventDefault();

        if ($.trim($("#txtBusinessTradeName").val()) == "") {
            MessageBox("Please enter business or trade name.",
                       "Business or Trade Name",
                        [
                            { Text: 'Ok'
                            }
                        ]
            );
            return;
        }

        SaveTradeName();

    });

    $('#btnTradeNameRemove').click(function (e) {
        e.preventDefault();

        MessageBox("Remove this record?",
            'Remove',
            [
                { Text: 'Yes',
                    Click: function () {
                        RemoveTradeName();
                    }
                }
            ,
                { Text: 'No'
                }
            ]
        );

    });

    $('#btnTradeNameCancel').click(function (e) {
        e.preventDefault();

        $.mobile.changePage('#pageOrganization');

    });

    $('#btnAddIndustryClassification').click(function (e) {

        Browser.LibraryIndustryClassification.Click = function (e) {

            IsNewIndustryClass = true
            intIndustryClassCntr = intIndustryClassCntr + 1;
            intIndustryClassIndex = intIndustryClassCntr;

            arIndustryClass = new Array();
            arIndustryClass[intIndustryClassRecID] = "";
            arIndustryClass[intIndustryClassID] = e.ID;
            arIndustryClass[intIndustryClass] = e.Class;
            arIndustryClass[intIndustryClassState] = (IsNewIndustryClass ? "New" : "Edit");

            sessionStorage["IndustryClass-" + intIndustryClassIndex] = JSON.stringify(arIndustryClass);

            LoadIndustryClassList();

        };

        Browser.LibraryIndustryClassification.Show();

    });

    $('#btnOrgInfoNext').click(function (e) {
        e.preventDefault();

        if ($.trim($("#txtCorporateName").val()) == "" || $.trim($("#txtCorporateName").val()).length <= 1) {
            MessageBox("Please enter corporate name.",
                        "Corporate Name",
                        [
                            { Text: 'Ok' }
                        ]
            );
            return;
        }

        if ($.trim($("#selRegistrationType").val()) == "") {
            MessageBox("Please select registration type.",
                        "Registration Type",
                        [
                            { Text: 'Ok' }
                        ]
            );
            return;
        }

        if ($.trim($("#txtRegistrationNo").val()) == "" || $.trim($("#txtRegistrationNo").val()).length <= 1) {
            MessageBox("Please enter registration number.",
                        "Registration No.",
                        [
                            { Text: 'Ok' }
                        ]
            );
            return;
        }

        if ($.trim($("#selRegistrationDateMonth").val()) == "" || $.trim($("#selRegistrationDateDay").val()) == "" || $.trim($("#selRegistrationDateYear").val()) == "") {
            MessageBox("Please enter valid registration date.",
                        "Registration Date",
                        [
                            { Text: 'Ok' }
                        ]
            );
            return;
        }

        if ($.trim($("#txtOfficeAddressStreet").val()) == "" || $.trim($("#txtOfficeAddressStreet").val()).length <= 1) {
            MessageBox("Please enter office address street.",
                        "Office Address Street",
                        [
                            { Text: 'Ok' }
                        ]
            );
            return;
        }

        if ($.trim($("#txtOfficeAddressCityID").val()) == "" || $.trim($("#txtOfficeAddressCityID").val()).length <= 1) {
            MessageBox("Please select office address city.",
                        "Office Address City",
                        [
                            { Text: 'Ok' }
                        ]
            );
            return;
        }

        if ($.trim($("#txtOfficeAddressCountryID").val()) == "" || $.trim($("#txtOfficeAddressCountryID").val()).length <= 1) {
            MessageBox("Please select office address country.",
                        "Office Address Country",
                        [
                            { Text: 'Ok' }
                        ]
            );
            return;
        }

        if ($.trim($("#txtBusinessAddressStreet").val()) == "" || $.trim($("#txtBusinessAddressStreet").val()).length <= 1) {
            MessageBox("Please enter business address street.",
                        "Business Address Street",
                        [
                            { Text: 'Ok' }
                        ]
            );
            return;
        }

        if ($.trim($("#txtBusinessAddressCityID").val()) == "" || $.trim($("#txtBusinessAddressCityID").val()).length <= 1) {
            MessageBox("Please select business address city.",
                        "Business Address City",
                        [
                            { Text: 'Ok' }
                        ]
            );
            return;
        }

        if ($.trim($("#txtBusinessAddressCountryID").val()) == "" || $.trim($("#txtBusinessAddressCountryID").val()).length <= 1) {
            MessageBox("Please select business address country.",
                        "Business Address Country",
                        [
                            { Text: 'Ok' }
                        ]
            );
            return;
        }

        ValidateUniqueOrgInfo();

    });

    $('#btnOrgInfoBack').click(function (e) {
        e.preventDefault();

        if ($.trim($("#txtID").val()) == "") {
            window.open("../Organization", "_self");
        }
        else {
            window.open("OrganizationInfoSummary", "_self");
        }

    });

    $('#btnOrgInfoDone').click(function (e) {
        e.preventDefault();

        IsDone = true;
        $('#btnOrgInfoNext').click();

    });

    $('#selRegistrationDateMonth').change(function (e) {
        e.preventDefault();

        setDayCollection('selRegistrationDateMonth', 'selRegistrationDateDay', 'selDOBYear', true);

    });

    $('#selRegistrationDateYear').change(function (e) {
        e.preventDefault();

        setDayCollection('selRegistrationDateMonth', 'selRegistrationDateDay', 'selRegistrationDateYear', true);

    });

    $('#selRegistrationDateYear').click(function (e) {
        e.preventDefault();

        if ($.trim($('#selRegistrationDateYear').val()) == "") {
            $('#selRegistrationDateYear').val(new Date().getFullYear());
            $('#selRegistrationDateYear').selectmenu('refresh');
        }

    });

    $("#txtNoOfRetailOutlet").keydown(function (e) {
        if (!NumberOnly(e)) {
            return;
        }
    });

    $("#txtNoOfOffice").keydown(function (e) {
        if (!NumberOnly(e)) {
            return;
        }
    });

    $("#txtNoOfWarehouse").keydown(function (e) {
        if (!NumberOnly(e)) {
            return;
        }
    });

    $("#txtNoOfEmployees").keydown(function (e) {
        if (!NumberOnly(e)) {
            return;
        }
    });

    function SetRegistrationInfoLabels() {

        if ($("#selRegistrationType").val() == "SEC") {
            $("#lblRegistrationNo").text("SEC Registration No.: ");
            $("#lblRegistrationDate").text("SEC Registration Date: ");
        }
        else if ($("#selRegistrationType").val() == "DTI") {
            $("#lblRegistrationNo").text("DTI Registration No.: ");
            $("#lblRegistrationDate").text("DTI Registration Date: ");
        }
        else {
            $("#lblRegistrationNo").text("Registration No.: ");
            $("#lblRegistrationDate").text("Registration Date: ");
        }

    }

    function ShowOrganizationInfo(strID) {
        var pData = {};
        pData.OrganizationID = strID;

        ClearOrganizationFields();

        AjaxSend({
            url: "Organization/getOrganizationInfo",
            data: pData,
            success: function (vData) {

                if (vData.OrganizationData.Rows.length > 0) {

                    $("#txtID").val(vData.OrganizationData.Rows[0].ID || '');

                    $("#txtCorporateName").val(vData.OrganizationData.Rows[0].CorporateName || '');
                    $("#txtCorporateNameInitial").val(vData.OrganizationData.Rows[0].CorporateNameInitial || '');
                    $("#txtBusinessTradeName").val(vData.OrganizationData.Rows[0].BusinessTradeName || '');
                    $("#txtBusinessTradeNameInitial").val(vData.OrganizationData.Rows[0].BusinessTradeNameInitial || '');
                    $("#selOwnershipType").val(vData.OrganizationData.Rows[0].OwnershipType || '').selectmenu('refresh');

                    $('#chkIsFranchisor').attr("checked", (vData.OrganizationData.Rows[0].IsFranchisor == 0 ? false : true)).checkboxradio("refresh");

                    $("#selRegistrationType").val(vData.OrganizationData.Rows[0].RegistrationType || '').selectmenu('refresh');
                    $("#txtRegistrationNo").val(vData.OrganizationData.Rows[0].RegistrationNo || '');
                    $("#selRegistrationDateMonth").val(vData.OrganizationData.Rows[0].RegistrationDateMonth || '').selectmenu('refresh');
                    $("#selRegistrationDateYear").val(vData.OrganizationData.Rows[0].RegistrationDateYear || '').selectmenu('refresh');
                    setDayCollection('selRegistrationDateMonth', 'selRegistrationDateDay', 'selDOBYear', true);
                    $("#selRegistrationDateDay").val(vData.OrganizationData.Rows[0].RegistrationDateDay || '').selectmenu('refresh');
                    SetRegistrationInfoLabels();

                    $("#txtCorporateTIN").val(vData.OrganizationData.Rows[0].TIN || '');

                    $("#txtOfficeAddressBuildingNo").val(vData.OrganizationData.Rows[0].OfficeAddressBuildingNo || '');
                    $("#txtOfficeAddressStreet").val(vData.OrganizationData.Rows[0].OfficeAddressStreet || '');
                    $("#txtOfficeAddressCityID").val(vData.OrganizationData.Rows[0].OfficeAddressCityID || '');
                    $("#btnOfficeAddressCity").text(vData.OrganizationData.Rows[0].OfficeAddressCity || 'Please Select').button('refresh');
                    $("#txtOfficeAddressCountryID").val(vData.OrganizationData.Rows[0].OfficeAddressCountryID || '');
                    $("#btnOfficeAddressCountry").text(vData.OrganizationData.Rows[0].OfficeAddressCountry || 'Please Select').button('refresh');

                    $("#txtBusinessAddressBuildingNo").val(vData.OrganizationData.Rows[0].BusinessAddressBuildingNo || '');
                    $("#txtBusinessAddressStreet").val(vData.OrganizationData.Rows[0].BusinessAddressStreet || '');
                    $("#txtBusinessAddressCityID").val(vData.OrganizationData.Rows[0].BusinessAddressCityID || '');
                    $("#btnBusinessAddressCity").text(vData.OrganizationData.Rows[0].BusinessAddressCity || 'Please Select').button('refresh');
                    $("#txtBusinessAddressCountryID").val(vData.OrganizationData.Rows[0].BusinessAddressCountryID || '');
                    $("#btnBusinessAddressCountry").text(vData.OrganizationData.Rows[0].BusinessAddressCountry || 'Please Select').button('refresh');

                    $("#txtPurpose").val(vData.OrganizationData.Rows[0].Purpose || '').keyup();
                    $("#selBusinessScope").val(vData.OrganizationData.Rows[0].BusinessScope || '').selectmenu('refresh');
                    $("#txtNoOfRetailOutlet").val(vData.OrganizationData.Rows[0].NoOfRetailOutlet || '');
                    $("#txtNoOfOffice").val(vData.OrganizationData.Rows[0].NoOfOffice || '');
                    $("#txtNoOfWarehouse").val(vData.OrganizationData.Rows[0].NoOfWarehouse || '');
                    $("#txtNoOfEmployees").val(vData.OrganizationData.Rows[0].NoOfEmployees || '');

                    AjaxSend({
                        url: "Organization/getOrgTradeNameList",
                        data: pData,
                        success: function (vDataTradeName) {

                            LoadOrgTradeNameList(vDataTradeName.OrganizationData.Rows);

                        }
                    });

                    AjaxSend({
                        url: "Organization/getOrganizationIndustryClassList",
                        data: pData,
                        success: function (vDataIndustryClass) {

                            LoadOrgIndustryClassList(vDataIndustryClass.OrganizationData.Rows);

                        }
                    });

                    $("#btnGoTo").show();

                }
            }
        });

    };

    function LoadOrgTradeNameList(vRows) {

        for (var p = 0; p <= vRows.length - 1; p++) {

            arTradeName = new Array();
            arTradeName[intTradeNameRecID] = vRows[p].ID;
            arTradeName[intTradeName] = vRows[p].BusinessTradeName;
            arTradeName[intTradeNameInitial] = vRows[p].BusinessTradeNameInitial;
            arTradeName[intTradeNameState] = "";

            sessionStorage["TradeName-" + p] = JSON.stringify(arTradeName);

        };

        intTradeNameCntr = vRows.length;
        LoadTradeNameList();

    };

    function LoadOrgIndustryClassList(vRows) {

        for (var p = 0; p <= vRows.length - 1; p++) {

            arIndustryClass = new Array();
            arIndustryClass[intIndustryClassRecID] = vRows[p].ID;
            arIndustryClass[intIndustryClassID] = vRows[p].IndustryClassID;
            arIndustryClass[intIndustryClass] = vRows[p].IndustryClass;
            arIndustryClass[intIndustryClassState] = "";

            sessionStorage["IndustryClass-" + p] = JSON.stringify(arIndustryClass);

        };

        intIndustryClassCntr = vRows.length;
        LoadIndustryClassList();

    };

    function ShowTradeNameInfo(arData, index) {

        $.mobile.changePage('#pageTradeName');

        ClearTradeNameFields();

        intTradeNameIndex = index;

        $("#txtTradeNameID").val(arData[intTradeNameRecID] || '');
        $("#txtBusinessTradeName").val(arData[intTradeName] || '');
        $("#txtBusinessTradeNameInitial").val(arData[intTradeNameInitial] || '');

        IsNewTradeName = false;

        $("#btnTradeNameRemove").show();

    };

    function ClearOrganizationFields() {

        sessionStorage.clear();

        $("#txtID").val('');

        $("#txtCorporateName").val('');
        $("#txtCorporateNameInitial").val('');
        $("#selOwnershipType").val('').selectmenu('refresh');

        $('#chkIsFranchisor').attr("checked", false).checkboxradio("refresh");

        $("#selRegistrationType").val('').selectmenu('refresh');
        $("#txtRegistrationNo").val('');
        $("#selRegistrationDateMonth").val('').selectmenu('refresh');
        $("#selRegistrationDateDay").val('').selectmenu('refresh');
        $("#selRegistrationDateYear").val('').selectmenu('refresh');

        $("#txtCorporateTIN").val('');

        $("#txtOfficeAddressBuildingNo").val('');
        $("#txtOfficeAddressStreet").val('');
        $("#txtOfficeAddressCityID").val('');
        $("#btnOfficeAddressCity").text('Please Select').button('refresh');
        $("#txtOfficeAddressCountryID").val('');
        $("#btnOfficeAddressCountry").text('Please Select').button('refresh');

        $("#txtBusinessAddressBuildingNo").val('');
        $("#txtBusinessAddressStreet").val('');
        $("#txtBusinessAddressCityID").val('');
        $("#btnBusinessAddressCity").text('Please Select').button('refresh');
        $("#txtBusinessAddressCountryID").val('');
        $("#btnBusinessAddressCountry").text('Please Select').button('refresh');

        $("#txtPurpose").val('').keyup();
        $("#selBusinessScope").val('').selectmenu('refresh');
        $("#txtNoOfRetailOutlet").val('0');
        $("#txtNoOfOffice").val('0');
        $("#txtNoOfWarehouse").val('0');
        $("#txtNoOfEmployees").val('0');

        LoadTradeNameList();
        LoadIndustryClassList();

        $("#btnGoTo").hide();
    }

    function ClearTradeNameFields() {
        $("#txtTradeNameID").val('');
        $("#txtBusinessTradeName").val('');
        $("#txtBusinessTradeNameInitial").val('');
    }

    function ValidateUniqueOrgInfo() {

        var pData = {};
        pData.OrganizationID = $.trim($("#txtID").val());
        pData.CorporateName = $.trim($("#txtCorporateName").val());

        AjaxSend({
            url: "Organization/CheckOrganizationUniqueName",
            data: pData,
            success: function (vData) {

                if (vData.OrganizationData.Rows.length > 0) {

                    MessageBox("Organization with the same corporate name already exist.",
                        "Organization Info",
                        [
                            { Text: 'Ok' }
                        ]
                    );
                }
                else {

                    SaveOrganizationInfo();

                }

            }
        });

    };

    function SaveOrganizationInfo() {
        var pData = [];
        var pDataTradeName = [];
        var pDataIndustryClassification = [];
        var intIndex;

        intIndex = 0;
        for (var i = 0; i <= intTradeNameCntr; i++) {

            if (sessionStorage["TradeName-" + i] != undefined) {

                arTradeName = new Array();
                arTradeName = JSON.parse(sessionStorage["TradeName-" + i]);

                pDataTradeName[intIndex] = {
                    ID: arTradeName[intTradeNameRecID],
                    BusinessTradeName: arTradeName[intTradeName],
                    BusinessTradeNameInitial: arTradeName[intTradeNameInitial],
                    RecState: arTradeName[intTradeNameState]
                };

                intIndex = intIndex + 1;

            }
        }

        intIndex = 0;
        for (var i = 0; i <= intIndustryClassCntr; i++) {

            if (sessionStorage["IndustryClass-" + i] != undefined) {

                arIndustryClass = new Array();
                arIndustryClass = JSON.parse(sessionStorage["IndustryClass-" + i]);

                pDataIndustryClassification[intIndex] = {
                    ID: arIndustryClass[intIndustryClassRecID],
                    IndustryClassID: arIndustryClass[intIndustryClassID],
                    RecState: arIndustryClass[intIndustryClassState]
                };

                intIndex = intIndex + 1;

            }
        }

        pData = {
            OrganizationID: $.trim($("#txtID").val()),
            CorporateName: $.trim($("#txtCorporateName").val()),
            CorporateNameInitial: $.trim($("#txtCorporateNameInitial").val()),
            BusinessTradeName: $.trim($("#txtBusinessTradeName").val()),
            BusinessTradeNameInitial: $.trim($("#txtBusinessTradeNameInitial").val()),
            OwnershipType: $.trim($("#selOwnershipType").val()),
            IsFranchisor: $("#chkIsFranchisor").is(":checked"),

            RegistrationType: $.trim($("#selRegistrationType").val()),
            RegistrationNo: $("#txtRegistrationNo").val(),
            RegistrationDate: $("#selRegistrationDateMonth").val() + '/' + $("#selRegistrationDateDay").val() + '/' + $("#selRegistrationDateYear").val(),
            TIN: $.trim($("#txtCorporateTIN").val()),

            OfficeAddressBuildingNo: $.trim($("#txtOfficeAddressBuildingNo").val()),
            OfficeAddressStreet: $.trim($("#txtOfficeAddressStreet").val()),
            OfficeAddressCityID: $.trim($("#txtOfficeAddressCityID").val()),
            OfficeAddressCountryID: $.trim($("#txtOfficeAddressCountryID").val()),

            BusinessAddressBuildingNo: $.trim($("#txtBusinessAddressBuildingNo").val()),
            BusinessAddressStreet: $.trim($("#txtBusinessAddressStreet").val()),
            BusinessAddressCityID: $.trim($("#txtBusinessAddressCityID").val()),
            BusinessAddressCountryID: $.trim($("#txtBusinessAddressCountryID").val()),

            Purpose: $.trim($("#txtPurpose").val()),
            BusinessScope: $.trim($("#selBusinessScope").val()),
            NoOfRetailOutlet: $.trim($("#txtNoOfRetailOutlet").val()),
            NoOfOffice: $.trim($("#txtNoOfOffice").val()),
            NoOfWarehouse: $.trim($("#txtNoOfWarehouse").val()),
            NoOfEmployees: $.trim($("#txtNoOfEmployees").val()),

            OrgTradeNameList: pDataTradeName,
            OrgIndustryClassificationList: pDataIndustryClassification
        };


        AjaxSend({
            url: "Organization/CheckOrganizationUniqueName",
            data: pData,
            success: function (vData) {

                if (vData.OrganizationData.Rows.length > 0) {
                    MessageBox("Organization with the same corporate name and business or trade name already exist.",
                        "Organization Info",
                        [
                            { Text: 'Ok' }
                        ]
                    );

                }
                else {
                    AjaxSend({
                        url: "Organization/SaveOrganizationInfo",
                        data: pData,
                        success: function (vSaveData) {

                            if (vSaveData.OrganizationData == "RegistrationDateError") {
                                MessageBox("Please enter valid registration date.",
                                "Registration Date",
                                [
                                    { Text: 'Ok' }
                                ]
                                );
                            }
                            else {

                                $("#txtID").val(vSaveData.OrganizationData || '');

                                if (IsDone == false) {
                                    window.open("OrganizationBranch", "_self");
                                }
                                else {
                                    window.open("OrganizationInfoSummary", "_self");
                                }

                            }

                        }
                    });

                }

            }
        });

    };

    function SaveTradeName() {

        arTradeName = new Array();
        arTradeName[intTradeNameRecID] = $("#txtTradeNameID").val();
        arTradeName[intTradeName] = $("#txtBusinessTradeName").val();
        arTradeName[intTradeNameInitial] = $("#txtBusinessTradeNameInitial").val();
        arTradeName[intTradeNameState] = (IsNewTradeName ? "New" : "Edit");

        sessionStorage["TradeName-" + intTradeNameIndex] = JSON.stringify(arTradeName);

        $.mobile.changePage('#pageOrganization');
        LoadTradeNameList();
    };

    function RemoveTradeName() {

        arTradeName = new Array();
        arTradeName[intTradeNameRecID] = $("#txtTradeNameID").val();
        arTradeName[intTradeName] = $("#txtBusinessTradeName").val();
        arTradeName[intTradeNameInitial] = $("#txtBusinessTradeNameInitial").val();
        arTradeName[intTradeNameState] = "Remove";

        sessionStorage["TradeName-" + intTradeNameIndex] = JSON.stringify(arTradeName);

        $.mobile.changePage('#pageOrganization');
        LoadTradeNameList();
    };

    function LoadTradeNameList() {
        var pLV = $('#lstTradeName');
        var pItem;

        pLV.html('');
        for (var i = 0; i <= intTradeNameCntr; i++) {

            if (sessionStorage["TradeName-" + i] != undefined) {

                arTradeName = new Array();
                arTradeName = JSON.parse(sessionStorage["TradeName-" + i]);

                if (arTradeName[intTradeNameState] != "Remove") {

                    pItem = $('<li id="lst' + i + '">')
                        .append($('<a>')
                                .attr('ID', i)
                                .data('Data', arTradeName)
                                .click(function (e) {
                                    e.preventDefault();
                                    ShowTradeNameInfo($(this).data('Data'), $(this).attr('ID'));
                                })
                                .append($('<h3>')
                                    .attr("style", "white-space:normal")
                                    .text(arTradeName[intTradeName])
                                )
                        );


                    pLV.append(pItem);

                }

            }
        }

        pLV.listview('refresh');
    };

    function LoadIndustryClassList() {
        var pLV = $('#lstIndustryClassification');
        var pItem;

        pLV.html('');
        for (var i = 0; i <= intIndustryClassCntr; i++) {

            if (sessionStorage["IndustryClass-" + i] != undefined) {

                arIndustryClass = new Array();
                arIndustryClass = JSON.parse(sessionStorage["IndustryClass-" + i]);

                if (arIndustryClass[intIndustryClassState] != "Remove") {

                    pItem = $('<li id="lst' + i + '">')
                        .append($('<a>')
                                .attr('ID', i)
                                .data('Data', arIndustryClass)
                                .append($('<h3>')
                                    .attr("style", "white-space:normal;")
                                    .text(arIndustryClass[intIndustryClass])
                                )
                        )
                        .append($('<a>')
                                .attr('ID', i)
                                .attr('href', "#")
                                .data('Data', arIndustryClass)
                                .click(function (e) {
                                    e.preventDefault();

                                    sessionStorage["IndustryClassToRemoveIndex"] = $(this).attr('ID');
                                    sessionStorage["IndustryClassToRemove"] = JSON.stringify($(this).data('Data'));

                                    MessageBox("Remove this record?",
                                        'Remove',
                                        [
                                            { Text: 'Yes',
                                                Click: function () {

                                                    var arIndustryClassToRemove = new Array();
                                                    arIndustryClassToRemove = JSON.parse(sessionStorage["IndustryClassToRemove"]);
                                                    intIndustryClassIndex = sessionStorage["IndustryClassToRemoveIndex"];

                                                    arIndustryClassToRemove[intIndustryClassState] = "Remove";
                                                    sessionStorage["IndustryClass-" + intIndustryClassIndex] = JSON.stringify(arIndustryClassToRemove);

                                                    LoadIndustryClassList();

                                                    sessionStorage["IndustryClassToRemove"] = null;
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