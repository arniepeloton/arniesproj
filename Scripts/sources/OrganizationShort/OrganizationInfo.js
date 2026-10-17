require([
    'common'
    ], function () {

        $(function () {

            var IsNewOrgTradeName = true;
            var arOrgTradeName = new Array();
            var intOrgTradeNameCntr = 0;
            var intOrgTradeNameIndex = 0;

            var intOrgTradeNameRecID = 0;
            var intOrgTradeName = 1;
            var intOrgTradeNameInitial = 2;
            var intOrgTradeNameState = 3;

            var IsNewOrgCategory = true;
            var arOrgCategory = new Array();
            var intOrgCategoryCntr = 0;
            var intOrgCategoryIndex = 0;

            var intOrgCategoryRecID = 0;
            var intOrgCategoryTypeID = 1;
            var intOrgCategory = 2;
            var intOrgCategoryState = 3;

            var mLoaded = false;
            var isIE = false || !!document.documentMode;

            var libname, lbllibname, addlibname, orgshortid;

            var NameOfBtnForLibrary;

            setMonthCollection("selOrgDOBMonth");
            setYearCollection("selOrgDOBYear");

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;

                if (!window.IsPageLoaded) {

                    AjaxSend({
                        url: 'OrganizationShort/IsNewEntry',
                        success: function (vData) {
                            lbllibname = sessionStorage.LabelLibraryName;
                            libname = sessionStorage.LibraryName;
                            addlibname = sessionStorage.AddViewLibBCName;
                            orgshortid = sessionStorage.ViewID;
                            ShowOrganizationInfo(vData.OrganizationID || '')

                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            window.OrgShortInfo = {
                ShowOrgInfo: function () {
                    lbllibname = sessionStorage.LabelLibraryName;
                    libname = sessionStorage.LibraryName;
                    addlibname = sessionStorage.AddViewLibBCName;
                    orgshortid = sessionStorage.ViewID;
                    ShowOrganizationInfo(sessionStorage.ViewID);
                }
            }

            $('body').on('change', '#rdoOrgOrganization', function (e) {
                e.preventDefault();

                if ($("#rdoOrgOrganization").is(":checked")) {
                    $("#divOrgOrganization").show();
                    $("#divOrgPerson").hide();
                } else {
                    $("#divOrgOrganization").hide();
                    $("#divOrgPerson").show();
                }
            })
            .on('change', '#rdoOrgPerson', function (e) {
                e.preventDefault();

                if ($("#rdoOrgOrganization").is(":checked")) {
                    $("#divOrgOrganization").show();
                    $("#divOrgPerson").hide();
                } else {
                    $("#divOrgOrganization").hide();
                    $("#divOrgPerson").show();
                }
            })
            .on('click', '#btnOrgTitle', function (e) {
                e.preventDefault();
                $('#TitleLibrary').text('TITLE');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of Title');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                //                Browser.LibraryPeoplePreSuffix.OfflineMode = false;
                //                Browser.LibraryPeoplePreSuffix.Click = function (e) {

                //                    $('#txtOrgTitleID').val(e.ID)
                //                    $('#btnOrgTitle').text(e.Code)
                //                    .button('refresh');

                //                };
                //                Browser.LibraryPeoplePreSuffix.Show();
            })
            .on('change', '#selOrgDOBMonth', function (e) {
                e.preventDefault();
                setDayCollection('selOrgDOBMonth', 'selOrgDOBDay', 'selOrgDOBYear', true);
            })
            .on('change', '#selDOBYear', function (e) {
                e.preventDefault();
                setDayCollection('selOrgDOBMonth', 'selOrgDOBDay', 'selOrgDOBYear', true);
            })
            .on('click', '#selOrgDOBYear', function (e) {
                e.preventDefault();
                if ($.trim($('#selOrgDOBYear').val()) == "") {
                    $('#selOrgDOBYear').val('1980');
                    $('#selOrgDOBYear').selectmenu('refresh');
                }
            })
            .on('click', '#btnOrgPersonAddressCity', function (e) {
                $('#TitleLibrary').text('CITY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of City');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'PersonAddress';
                //                Browser.LibraryCities.OfflineMode = false;
                //                Browser.LibraryCities.Click = function (e) {

                //                    $('#txtOrgPersonAddressCityID').val(e.ID)
                //                    $('#btnOrgPersonAddressCity').text(e.Name)
                //                    .button('refresh');

                //                };
                //                Browser.LibraryCities.Show();
            })
            .on('click', '#btnOrgPersonAddressCountry', function (e) {
                $('#TitleLibrary').text('COUNTRY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of Country');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'PersonAddress';
                //                Browser.LibraryCountries.OfflineMode = false;
                //                Browser.LibraryCountries.Click = function (e) {

                //                    $('#txtOrgPersonAddressCountryID').val(e.ID)
                //                    $('#btnOrgPersonAddressCountry').text(e.Name)
                //                    .button('refresh');

                //                };
                //                Browser.LibraryCountries.Show();
            })
            .on('click', '#btnOrgOfficeAddressCity', function (e) {
                $('#TitleLibrary').text('CITY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of City');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'OrgOfficeAddress';
                //                Browser.LibraryCities.OfflineMode = false;
                //                Browser.LibraryCities.Click = function (e) {

                //                    $('#txtOrgOfficeAddressCityID').val(e.ID)
                //                    $('#btnOrgOfficeAddressCity').text(e.Name)
                //                    .button('refresh');

                //                };
                //                Browser.LibraryCities.Show();
            })
            .on('click', '#btnOrgOfficeAddressCountry', function (e) {
                $('#TitleLibrary').text('COUNTRY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of Country');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'OrgOfficeAddress';
                //                Browser.LibraryCountries.OfflineMode = false;
                //                Browser.LibraryCountries.Click = function (e) {

                //                    $('#txtOrgOfficeAddressCountryID').val(e.ID)
                //                    $('#btnOrgOfficeAddressCountry').text(e.Name)
                //                    .button('refresh');

                //                };
                //                Browser.LibraryCountries.Show();
            })
            .on('change', '#chkOrgSameAsPricipalAddress', function (e) {
                e.preventDefault();

                if ($(this).is(":checked")) {

                    $("#txtOrgBusinessAddressBuildingNo").val($("#txtOrgOfficeAddressBuildingNo").val());
                    $("#txtOrgBusinessAddressStreet").val($("#txtOrgOfficeAddressStreet").val());
                    $("#txtOrgBusinessAddressCityID").val($("#txtOrgOfficeAddressCityID").val());
                    $("#btnOrgBusinessAddressCity").text($("#btnOrgOfficeAddressCity").text()).button('refresh');
                    $("#txtOrgBusinessAddressCountryID").val($("#txtOrgOfficeAddressCountryID").val());
                    $("#btnOrgBusinessAddressCountry").text($("#btnOrgOfficeAddressCountry").text()).button('refresh');

                }
            })
            .on('click', '#btnOrgBusinessAddressCity', function (e) {
                e.preventDefault();
                $('#TitleLibrary').text('CITY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of City');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'OrgBusinessAddress';
                //                Browser.LibraryCities.OfflineMode = false;
                //                Browser.LibraryCities.Click = function (e) {

                //                    $('#txtOrgBusinessAddressCityID').val(e.ID)
                //                    $('#btnOrgBusinessAddressCity').text(e.Name)
                //                    .button('refresh');

                //                };
                //                Browser.LibraryCities.Show();
            })
            .on('click', '#btnOrgBusinessAddressCountry', function (e) {
                e.preventDefault();
                $('#TitleLibrary').text('COUNTRY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of Country');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'OrgBusinessAddress';
                //                Browser.LibraryCountries.OfflineMode = false;
                //                Browser.LibraryCountries.Click = function (e) {

                //                    $('#txtOrgBusinessAddressCountryID').val(e.ID)
                //                    $('#btnOrgBusinessAddressCountry').text(e.Name)
                //                    .button('refresh');

                //                };
                //                Browser.LibraryCountries.Show();
            })
            .on('click', '#btnAddOrgTradeName', function (e) {
                e.preventDefault();

                IsNewOrgTradeName = true
                intOrgTradeNameCntr = intOrgTradeNameCntr + 1;
                intOrgTradeNameIndex = intOrgTradeNameCntr;

                //$.mobile.changePage('#pageOrgTradeName');
                $('#pageOrganizationInfo').hide();
                $('#pageOrgTradeName').show();
                ClearOrgTradeNameFields();
            })
            .on('click', '#btnOrgTradeNameSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgBusinessTradeName").val()) == "") {
                    $('#TitleOrgEmp').text('Business or Trade Name');
                    $('#MsgOrgEmp').text('Please enter business or trade name.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                SaveOrgTradeName();
            })
            .on('click', '#btnOrgTradeNameRemove', function (e) {
                e.preventDefault();
                $('#TitleOrgEmp').text('Remove');
                $('#MsgOrgEmp').text('Remove this record?');
                $('#panOrgEmpMessage').popup('open');
                $('#btnOKOrgEmp').text('Yes').click(function (e) {
                    e.preventDefault();
                    RemoveOrgTradeName();
                    $('#panOrgEmpMessage').popup('close');
                })
                $('#divBtnCancel').show();
                $('#btnCancelOrgEmp').text('No').click(function () {
                    $('#panOrgEmpMessage').popup('close');
                })
            })
            .on('click', '#btnOrgTradeNameCancel', function (e) {
                e.preventDefault();
                //$.mobile.changePage('#pageOrganizationInfo');
                $('#pageOrganizationInfo').show();
                $('#pageOrgTradeName').hide();
            })
            .on('click', '#btnAddOrgCategory', function (e) {
                $('#TitleLibrary').text('CATEGORY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of category');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'OrgCategory';
                //                Browser.LibraryOrganizationCategory.OfflineMode = false;
                //                Browser.LibraryOrganizationCategory.Click = function (e) {

                //                    IsNewOrgCategory = true
                //                    intOrgCategoryCntr = intOrgCategoryCntr + 1;
                //                    intOrgCategoryIndex = intOrgCategoryCntr;

                //                    arOrgCategory = new Array();
                //                    arOrgCategory[intOrgCategoryRecID] = "";
                //                    arOrgCategory[intOrgCategoryTypeID] = e.ID;
                //                    arOrgCategory[intOrgCategory] = e.CategoryType;
                //                    arOrgCategory[intOrgCategoryState] = (IsNewOrgCategory ? "New" : "Edit");

                //                    sessionStorage["OrgCategory-" + intOrgCategoryIndex] = JSON.stringify(arOrgCategory);

                //                    LoadOrgCategoryList();

                //                };

                //                Browser.LibraryOrganizationCategory.Show();
            })
            .on('click', '#btnOrgInfoDone', function (e) {
                e.preventDefault();

                if ($("#rdoOrgOrganization").is(":checked")) {
                    if ($.trim($("#txtOrgCorporateName").val()) == "" || $.trim($("#txtOrgCorporateName").val()).length <= 1) {
                        //                        MessageBox("Please enter corporate name.",
                        //                                "Corporate Name",
                        //                                [
                        //                                    { Text: 'Ok' }
                        //                                ]
                        //                        );
                        $('#TitleOrgEmp').text('Corporate Name');
                        $('#MsgOrgEmp').text('Please enter corporate name.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                    if ($.trim($("#txtOrgCorporateTIN").val()) == "" || $.trim($("#txtOrgCorporateTIN").val()).length <= 1) {
                        //                        MessageBox("Please enter Corporate Tax Identification No. (TIN).",
                        //                                "Corporate Tax Identification No. (TIN)",
                        //                                [
                        //                                    { Text: 'Ok' }
                        //                                ]
                        //                        );
                        $('#TitleOrgEmp').text('Corporate Tax Identification No. (TIN)');
                        $('#MsgOrgEmp').text('Please enter Corporate Tax Identification No. (TIN).');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                    if ($.trim($("#txtOrgOfficeAddressStreet").val()) == "" || $.trim($("#txtOrgOfficeAddressStreet").val()).length <= 1) {
                        //                        MessageBox("Please enter office address street.",
                        //                                "Office Address Street",
                        //                                [
                        //                                    { Text: 'Ok' }
                        //                                ]
                        //                        );
                        $('#TitleOrgEmp').text('Office Address Street');
                        $('#MsgOrgEmp').text('Please enter office address street.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                    if ($.trim($("#txtOrgOfficeAddressCityID").val()) == "") {
                        //                        MessageBox("Please select office address city.",
                        //                                "Office Address City",
                        //                                [
                        //                                    { Text: 'Ok' }
                        //                                ]
                        //                        );
                        $('#TitleOrgEmp').text('Office Address City');
                        $('#MsgOrgEmp').text('Please select office address city.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                    if ($.trim($("#txtOrgOfficeAddressCountryID").val()) == "") {
                        //                        MessageBox("Please select office address country.",
                        //                                "Office Address Country",
                        //                                [
                        //                                    { Text: 'Ok' }
                        //                                ]
                        //                        );
                        $('#TitleOrgEmp').text('Office Address Country');
                        $('#MsgOrgEmp').text('Please select office address country.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                    if ($.trim($("#txtOrgBusinessAddressStreet").val()) == "" || $.trim($("#txtOrgBusinessAddressStreet").val()).length <= 1) {
                        //                        MessageBox("Please enter business address street.",
                        //                                "Business Address Street",
                        //                                [
                        //                                    { Text: 'Ok' }
                        //                                ]
                        //                        );
                        $('#TitleOrgEmp').text('Business Address Street');
                        $('#MsgOrgEmp').text('Please enter business address street.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                    if ($.trim($("#txtOrgBusinessAddressCityID").val()) == "") {
                        //                        MessageBox("Please select business address city.",
                        //                                "Business Address City",
                        //                                [
                        //                                    { Text: 'Ok' }
                        //                                ]
                        //                        );
                        $('#TitleOrgEmp').text('Business Address City');
                        $('#MsgOrgEmp').text('Please select business address city.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                    if ($.trim($("#txtOrgBusinessAddressCountryID").val()) == "") {
                        //                        MessageBox("Please select business address country.",
                        //                                "Business Address Country",
                        //                                [
                        //                                    { Text: 'Ok' }
                        //                                ]
                        //                        );
                        $('#TitleOrgEmp').text('Business Address Country');
                        $('#MsgOrgEmp').text('Please select business address country.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                } else {
                    if ($.trim($("#txtOrgSurname").val()) == "" || $.trim($("#txtOrgSurname").val()).length <= 1) {
                        //                        MessageBox("Please enter surname.",
                        //                            "Surname",
                        //                            [
                        //                                { Text: 'Ok' }
                        //                            ]
                        //                        );
                        $('#TitleOrgEmp').text('Surname');
                        $('#MsgOrgEmp').text('Please enter surname.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                    if ($.trim($("#txtOrgFirstName").val()) == "" || $.trim($("#txtOrgFirstName").val()).length <= 1) {
                        //                        MessageBox("Please enter first name.",
                        //                            "First Name",
                        //                            [
                        //                                { Text: 'Ok' }
                        //                            ]
                        //                        );
                        $('#TitleOrgEmp').text('First Name');
                        $('#MsgOrgEmp').text('Please enter first name.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                    if ($.trim($("#txtOrgMiddleName").val()) == "" || $.trim($("#txtOrgMiddleName").val()).length <= 1) {
                        //                        MessageBox("Please enter middle name.",
                        //                            "Middle Name",
                        //                            [
                        //                                { Text: 'Ok' }
                        //                            ]
                        //                        );
                        $('#TitleOrgEmp').text('Middle Name');
                        $('#MsgOrgEmp').text('Please enter middle name.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                    //                    if ($.trim($("#selOrgDOBMonth").val()) == "") {
                    //                        MessageBox("Please enter valid date of birth.",
                    //                            "Date Of Birth",
                    //                            [
                    //                                { Text: 'Ok' }
                    //                            ]
                    //                        );
                    //                        return;
                    //                    }
                    //                    if ($.trim($("#selOrgDOBDay").val()) == "") {
                    //                        MessageBox("Please enter valid date of birth.",
                    //                            "Date Of Birth",
                    //                            [
                    //                                { Text: 'Ok' }
                    //                            ]
                    //                        );
                    //                        return;
                    //                    }
                    //                    if ($.trim($("#selOrgDOBYear").val()) == "") {
                    //                        MessageBox("Please enter valid date of birth.",
                    //                            "Date Of Birth",
                    //                            [
                    //                                { Text: 'Ok' }
                    //                            ]
                    //                        );
                    //                        return;
                    //                    }

                    //                    if ($.trim($("#txtOrgMotherMaidenSurname").val()) == "" || $.trim($("#txtOrgMotherMaidenSurname").val()).length <= 1) {
                    //                        MessageBox("Please enter mother maiden surname.",
                    //                            "Mother Maiden Surname",
                    //                            [
                    //                                { Text: 'Ok' }
                    //                            ]
                    //                        );
                    //                        return;
                    //                    }
                    //                    if ($.trim($("#txtOrgMotherMaidenFirstName").val()) == "" || $.trim($("#txtOrgMotherMaidenFirstName").val()).length <= 1) {
                    //                        MessageBox("Please enter mother first name.",
                    //                            "First Name",
                    //                            [
                    //                                { Text: 'Ok' }
                    //                            ]
                    //                        );
                    //                        return;
                    //                    }
                }

                SaveOrganizationInfo();
            })
            .on('click', '#btnOrgInfoCancel', function () {
                window.location.hash = 'LibraryPage';
            })
            .on('keypress', '#txtSearchKeyword', function (e) {
                if (e.keyCode == 13) {
                    if ($('#TitleLibrary').text() == "CITY") {
                        SearchCityPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "COUNTRY") {
                        SearchCountryPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "CATEGORY") {
                        SearchCategoryPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "TITLE") {
                        SearchTitlePopUp($(this).val());
                    }
                }
            })

            function SearchTitlePopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryPeoplePreSuffix',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Code)
                                        )
                                        .append($('<p>')
                                            .text(vData.Results.Rows[p].Description)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            $('#txtOrgTitleID').val($(this).data('data').ID);
                                            $('#btnOrgTitle').text($(this).data('data').Code).button('refresh');
                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
                    }
                });
            }

            function SearchCategoryPopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryOrganizationCategory',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].CategoryType)
                                        )
                                        .append($('<p>')
                                            .text(vData.Results.Rows[p].Category)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "OrgCategory") {
                                                IsNewOrgCategory = true
                                                intOrgCategoryCntr = intOrgCategoryCntr + 1;
                                                intOrgCategoryIndex = intOrgCategoryCntr;

                                                arOrgCategory = new Array();
                                                arOrgCategory[intOrgCategoryRecID] = "";
                                                arOrgCategory[intOrgCategoryTypeID] = $(this).attr("ID");
                                                arOrgCategory[intOrgCategory] = $(this).data('data').CategoryType;
                                                arOrgCategory[intOrgCategoryState] = (IsNewOrgCategory ? "New" : "Edit");

                                                sessionStorage["OrgCategory-" + intOrgCategoryIndex] = JSON.stringify(arOrgCategory);
                                                console.log(sessionStorage["OrgCategory-" + intOrgCategoryIndex]);
                                                LoadOrgCategoryList();
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
                    }
                });
            }

            function SearchCountryPopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryCountries',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Name)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "OrgOfficeAddress") {
                                                $('#txtOrgOfficeAddressCountryID').val($(this).data('data').ID);
                                                $('#btnOrgOfficeAddressCountry').text($(this).data('data').Name).button('refresh');
                                            }
                                            else if (NameOfBtnForLibrary == "OrgBusinessAddress") {
                                                $('#txtOrgBusinessAddressCountryID').val($(this).data('data').ID);
                                                $('#btnOrgBusinessAddressCountry').text($(this).data('data').Name).button('refresh');
                                            }
                                            else if (NameOfBtnForLibrary == "PersonAddress") {
                                                $('#txtOrgPersonAddressCountryID').val($(this).data('data').ID);
                                                $('#btnOrgPersonAddressCountry').text($(this).data('data').Name).button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
                    }
                });
            }

            function SearchCityPopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryCities',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Name)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "OrgOfficeAddress") {
                                                $('#txtOrgOfficeAddressCityID').val($(this).data('data').ID);
                                                $('#btnOrgOfficeAddressCity').text($(this).data('data').Name).button('refresh');
                                            }
                                            else if (NameOfBtnForLibrary == "OrgBusinessAddress") {
                                                $('#txtOrgBusinessAddressCityID').val($(this).data('data').ID);
                                                $('#btnOrgBusinessAddressCity').text($(this).data('data').Name).button('refresh');
                                            }
                                            else if (NameOfBtnForLibrary == "PersonAddress") {
                                                $('#txtOrgPersonAddressCityID').val($(this).data('data').ID);
                                                $('#btnOrgPersonAddressCity').text($(this).data('data').Name).button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
                    }
                });
            }

            function ShowOrganizationInfo(strID) {
                var pData = {};
                pData.OrganizationID = strID;

                ClearOrganizationFields();

                AjaxSend({
                    url: "OrganizationShort/getOrganizationInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData.Rows.length > 0) {

                            $('#rdoOrgOrganization').attr("checked", (vData.OrganizationData.Rows[0].IsOrganization == 1 ? true : false)).checkboxradio("refresh");
                            $('#rdoOrgPerson').attr("checked", (vData.OrganizationData.Rows[0].IsOrganization == 0 ? true : false)).checkboxradio("refresh");

                            $("#txtOrgID").val(vData.OrganizationData.Rows[0].ID || '');

                            if ($("#rdoOrgOrganization").is(":checked")) {

                                $("#divOrgOrganization").show();
                                $("#divOrgPerson").hide();

                                $("#txtOrgCorporateName").val(vData.OrganizationData.Rows[0].CompanyName || '');
                                $("#txtOrgCorporateNameInitial").val(vData.OrganizationData.Rows[0].CompanyNameInitial || '');
                                $("#selOrgOwnershipType").val(vData.OrganizationData.Rows[0].OwnershipType || '').selectmenu('refresh');

                                $("#txtOrgCorporateTIN").val(vData.OrganizationData.Rows[0].TIN || '');

                                $('#chkOrgIsFranchisor').attr("checked", (vData.OrganizationData.Rows[0].IsFranchisor == 0 ? false : true)).checkboxradio("refresh");

                                $("#txtOrgOfficeAddressBuildingNo").val(vData.OrganizationData.Rows[0].OfficeAddressBuildingNo || '');
                                $("#txtOrgOfficeAddressStreet").val(vData.OrganizationData.Rows[0].OfficeAddressStreet || '');
                                $("#txtOrgOfficeAddressCityID").val(vData.OrganizationData.Rows[0].OfficeAddressCityID || '');
                                $("#btnOrgOfficeAddressCity").text(vData.OrganizationData.Rows[0].OfficeAddressCity || 'Please Select').button('refresh');
                                $("#txtOrgOfficeAddressCountryID").val(vData.OrganizationData.Rows[0].OfficeAddressCountryID || '');
                                $("#btnOrgOfficeAddressCountry").text(vData.OrganizationData.Rows[0].OfficeAddressCountry || 'Please Select').button('refresh');

                                $("#txtOrgBusinessAddressBuildingNo").val(vData.OrganizationData.Rows[0].BusinessAddressBuildingNo || '');
                                $("#txtOrgBusinessAddressStreet").val(vData.OrganizationData.Rows[0].BusinessAddressStreet || '');
                                $("#txtOrgBusinessAddressCityID").val(vData.OrganizationData.Rows[0].BusinessAddressCityID || '');
                                $("#btnOrgBusinessAddressCity").text(vData.OrganizationData.Rows[0].BusinessAddressCity || 'Please Select').button('refresh');
                                $("#txtOrgBusinessAddressCountryID").val(vData.OrganizationData.Rows[0].BusinessAddressCountryID || '');
                                $("#btnOrgBusinessAddressCountry").text(vData.OrganizationData.Rows[0].BusinessAddressCountry || 'Please Select').button('refresh');

                                AjaxSend({
                                    url: "OrganizationShort/getOrgTradeNameList",
                                    data: pData,
                                    success: function (vDataTradeName) {

                                        LoadOrgTradeName(vDataTradeName.OrganizationData.Rows);

                                    }
                                });

                                AjaxSend({
                                    url: "OrganizationShort/getOrgCategoryList",
                                    data: pData,
                                    success: function (vDataCategory) {

                                        LoadOrgCategory(vDataCategory.OrganizationData.Rows);

                                    }
                                });

                            } else {

                                $("#divOrgOrganization").hide();
                                $("#divOrgPerson").show();

                                $("#txtOrgTitleID").val(vData.OrganizationData.Rows[0].PersonTitleID || '');
                                $("#btnOrgTitle").text(vData.OrganizationData.Rows[0].PersonTitle || 'Please Select').button('refresh');

                                $("#txtOrgSurname").val(vData.OrganizationData.Rows[0].PersonSurname || '');
                                $("#txtOrgFirstName").val(vData.OrganizationData.Rows[0].PersonFirstName || '');
                                $("#txtOrgMiddleName").val(vData.OrganizationData.Rows[0].PersonMiddleName || '');

                                $("#selOrgPersonalSuffixName").val(vData.OrganizationData.Rows[0].PersonSuffixName || '').selectmenu('refresh');

                                $("#selOrgDOBMonth").val(vData.OrganizationData.Rows[0].PersonDateOfBirthMonth || '').selectmenu('refresh');
                                $("#selOrgDOBYear").val(vData.OrganizationData.Rows[0].PersonDateOfBirthYear || '').selectmenu('refresh');
                                setDayCollection('selOrgDOBMonth', 'selOrgDOBDay', 'selOrgDOBYear', true);
                                $("#selOrgDOBDay").val(vData.OrganizationData.Rows[0].PersonDateOfBirthDay || '').selectmenu('refresh');

                                $("#txtOrgMotherMaidenSurname").val(vData.OrganizationData.Rows[0].PersonMotherMaidenSurname || '');
                                $("#txtOrgMotherMaidenFirstName").val(vData.OrganizationData.Rows[0].PersonMotherMaidenFirstName || '');
                                $("#txtOrgMotherMaidenMiddleName").val(vData.OrganizationData.Rows[0].PersonMotherMaidenMiddleName || '');

                                $("#txtOrgPersonAddressBuildingNo").val(vData.OrganizationData.Rows[0].OfficeAddressBuildingNo || '');
                                $("#txtOrgPersonAddressStreet").val(vData.OrganizationData.Rows[0].OfficeAddressStreet || '');

                                $("#txtOrgPersonAddressCityID").val(vData.OrganizationData.Rows[0].OfficeAddressCityID || '');
                                $("#btnOrgPersonAddressCity").text(vData.OrganizationData.Rows[0].OfficeAddressCity || 'Please Select').button('refresh');

                                $("#txtOrgPersonAddressCountryID").val(vData.OrganizationData.Rows[0].OfficeAddressCountryID || '');
                                $("#btnOrgPersonAddressCountry").text(vData.OrganizationData.Rows[0].OfficeAddressCountry || 'Please Select').button('refresh');

                            }

                        }
                    }
                });

            };

            function LoadOrgTradeName(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgTradeName = new Array();
                    arOrgTradeName[intOrgTradeNameRecID] = vRows[p].ID;
                    arOrgTradeName[intOrgTradeName] = vRows[p].BusinessTradeName;
                    arOrgTradeName[intOrgTradeNameInitial] = vRows[p].BusinessTradeNameInitial;
                    arOrgTradeName[intOrgTradeNameState] = "";

                    sessionStorage["OrgTradeName-" + p] = JSON.stringify(arOrgTradeName);

                };

                intOrgTradeNameCntr = vRows.length;
                LoadOrgTradeNameList();

            };

            function LoadOrgCategory(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgCategory = new Array();
                    arOrgCategory[intOrgCategoryRecID] = vRows[p].ID;
                    arOrgCategory[intOrgCategoryTypeID] = vRows[p].CategoryTypeID;
                    arOrgCategory[intOrgCategory] = vRows[p].CategoryType;
                    arOrgCategory[intOrgCategoryState] = "";

                    sessionStorage["OrgCategory-" + p] = JSON.stringify(arOrgCategory);

                };

                intOrgCategoryCntr = vRows.length;
                LoadOrgCategoryList();

            };

            function ShowOrgTradeNameInfo(arData, index) {

                //$.mobile.changePage('#pageOrgTradeName');
                $('#pageOrganizationInfo').hide();
                $('#pageOrgTradeName').show();

                ClearOrgTradeNameFields();

                intOrgTradeNameIndex = index;

                $("#txtOrgTradeNameID").val(arData[intOrgTradeNameRecID] || '');
                $("#txtOrgBusinessTradeName").val(arData[intOrgTradeName] || '');
                $("#txtOrgBusinessTradeNameInitial").val(arData[intOrgTradeNameInitial] || '');

                IsNewTradeName = false;

                $("#btnTradeNameRemove").show();

            };

            function ClearOrganizationFields() {

                // dont clear
                // sessionStorage.clear();
                proj.ClearSessionStorage();

                sessionStorage["LibraryName"] = libname;
                sessionStorage["LabelLibraryName"] = lbllibname;
                sessionStorage["AddViewLibBCName"] = addlibname;
                sessionStorage["ViewID"] = orgshortid;

                $("#txtOrgID").val('');

                $("#txtOrgTitleID").val('');
                $("#btnOrgTitle").text('Please Select').button('refresh');

                $("#txtOrgSurname").val('');
                $("#txtOrgFirstName").val('');
                $("#txtOrgMiddleName").val('');

                $("#selOrgPersonalSuffixName").val('').selectmenu('refresh');

                $("#selOrgDOBMonth").val('').selectmenu('refresh');
                $("#selOrgDOBYear").val('').selectmenu('refresh');
                $("#selOrgDOBDay").val('').selectmenu('refresh');

                $("#txtOrgMotherMaidenSurname").val('');
                $("#txtOrgMotherMaidenFirstName").val('');
                $("#txtOrgMotherMaidenMiddleName").val('');

                $("#txtOrgPersonAddressBuildingNo").val('');
                $("#txtOrgPersonAddressStreet").val('');

                $("#txtOrgPersonAddressCityID").val('');
                $("#btnOrgPersonAddressCity").text('Please Select').button('refresh');

                $("#txtOrgPersonAddressCountryID").val('');
                $("#btnOrgPersonAddressCountry").text('Please Select').button('refresh');

                $("#txtOrgCorporateName").val('');
                $("#txtOrgCorporateNameInitial").val('');
                $("#selOrgOwnershipType").val('').selectmenu('refresh');

                $("#txtOrgCorporateTIN").val('');

                $('#chkOrgIsFranchisor').attr("checked", false).checkboxradio("refresh");

                $("#txtOrgOfficeAddressBuildingNo").val('');
                $("#txtOrgOfficeAddressStreet").val('');
                $("#txtOrgOfficeAddressCityID").val('');
                $("#btnOrgOfficeAddressCity").text('Please Select').button('refresh');
                $("#txtOrgOfficeAddressCountryID").val('');
                $("#btnOrgOfficeAddressCountry").text('Please Select').button('refresh');

                $('#chkOrgSameAsPricipalAddress').attr("checked", false).checkboxradio("refresh");

                $("#txtOrgBusinessAddressBuildingNo").val('');
                $("#txtOrgBusinessAddressStreet").val('');
                $("#txtOrgBusinessAddressCityID").val('');
                $("#btnOrgBusinessAddressCity").text('Please Select').button('refresh');
                $("#txtOrgBusinessAddressCountryID").val('');
                $("#btnOrgBusinessAddressCountry").text('Please Select').button('refresh');

                LoadOrgTradeNameList();
                LoadOrgCategoryList();

            }

            function ClearOrgTradeNameFields() {
                $("#txtOrgTradeNameID").val('');
                $("#txtOrgBusinessTradeName").val('');
                $("#txtOrgBusinessTradeNameInitial").val('');

                $("#btnOrgTradeNameRemove").hide();

            }

            function SaveOrganizationInfo() {
                var pData = [];
                var pDataTradeName = [];
                var pDataCategory = [];
                var intIndex;

                intIndex = 0;
                for (var i = 0; i <= intOrgTradeNameCntr; i++) {

                    if (sessionStorage["OrgTradeName-" + i] != undefined) {

                        arOrgTradeName = new Array();
                        arOrgTradeName = JSON.parse(sessionStorage["OrgTradeName-" + i]);

                        pDataTradeName[intIndex] = {
                            ID: arOrgTradeName[intOrgTradeNameRecID],
                            BusinessTradeName: arOrgTradeName[intOrgTradeName],
                            BusinessTradeNameInitial: arOrgTradeName[intOrgTradeNameInitial],
                            RecState: arOrgTradeName[intOrgTradeNameState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                intIndex = 0;
                for (var i = 0; i <= intOrgCategoryCntr; i++) {

                    if (sessionStorage["OrgCategory-" + i] != undefined) {

                        arOrgCategory = new Array();
                        arOrgCategory = JSON.parse(sessionStorage["OrgCategory-" + i]);

                        pDataCategory[intIndex] = {
                            ID: arOrgCategory[intOrgCategoryRecID],
                            CategoryTypeID: arOrgCategory[intOrgCategoryTypeID],
                            RecState: arOrgCategory[intOrgCategoryState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                pData = {
                    OrganizationID: $.trim($("#txtOrgID").val()),

                    IsOrganization: ($("#rdoOrgOrganization").is(":checked") ? 1 : 0),

                    PersonTitleID: ($("#rdoOrgOrganization").is(":checked") ? '' : $.trim($("#txtOrgTitleID").val())),
                    PersonSurname: ($("#rdoOrgOrganization").is(":checked") ? '' : $.trim($("#txtOrgSurname").val())),
                    PersonFirstName: ($("#rdoOrgOrganization").is(":checked") ? '' : $.trim($("#txtOrgFirstName").val())),
                    PersonMiddleName: ($("#rdoOrgOrganization").is(":checked") ? '' : $.trim($("#txtOrgMiddleName").val())),
                    PersonSuffixName: ($("#rdoOrgOrganization").is(":checked") ? '' : $.trim($("#selOrgPersonalSuffixName").val())),
                    PersonDOB: ($("#rdoOrgOrganization").is(":checked") ? '' : $.trim($("#selOrgDOBMonth").val()) + '/' + $.trim($("#selOrgDOBDay").val()) + '/' + $.trim($("#selOrgDOBYear").val())),
                    PersonMotherMaidenSurname: ($("#rdoOrgOrganization").is(":checked") ? '' : $.trim($("#txtOrgMotherMaidenSurname").val())),
                    PersonMotherMaidenFirstName: ($("#rdoOrgOrganization").is(":checked") ? '' : $.trim($("#txtOrgMotherMaidenFirstName").val())),
                    PersonMotherMaidenMiddleName: ($("#rdoOrgOrganization").is(":checked") ? '' : $.trim($("#txtOrgMotherMaidenMiddleName").val())),

                    CompanyName: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgCorporateName").val()) : ''),
                    CompanyNameInitial: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgCorporateNameInitial").val()) : ''),
                    OwnershipType: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#selOrgOwnershipType").val()) : ''),
                    IsFranchisor: ($("#chkOrgIsFranchisor").is(":checked") ? "1" : "0"),
                    TIN: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgCorporateTIN").val()) : ''),

                    OfficeAddressBuildingNo: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgOfficeAddressBuildingNo").val()) : $.trim($("#txtOrgPersonAddressBuildingNo").val())),
                    OfficeAddressStreet: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgOfficeAddressStreet").val()) : $.trim($("#txtOrgPersonAddressStreet").val())),
                    OfficeAddressCityID: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgOfficeAddressCityID").val()) : $.trim($("#txtOrgPersonAddressCityID").val())),
                    OfficeAddressCountryID: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgOfficeAddressCountryID").val()) : $.trim($("#txtOrgPersonAddressCountryID").val())),

                    BusinessAddressBuildingNo: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgBusinessAddressBuildingNo").val()) : $.trim($("#txtOrgPersonAddressBuildingNo").val())),
                    BusinessAddressStreet: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgBusinessAddressStreet").val()) : $.trim($("#txtOrgPersonAddressStreet").val())),
                    BusinessAddressCityID: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgBusinessAddressCityID").val()) : $.trim($("#txtOrgPersonAddressCityID").val())),
                    BusinessAddressCountryID: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgBusinessAddressCountryID").val()) : $.trim($("#txtOrgPersonAddressCountryID").val())),

                    OrgTradeNameList: pDataTradeName,
                    OrgCategoryList: pDataCategory,

                    IsAddAsCustomer: (window.pageSourcePage != undefined ? 1 : 0)

                };

                AjaxSend({
                    url: ($("#rdoOrgOrganization").is(":checked") ? "OrganizationShort/CheckOrgUniqueOrgName" : "OrganizationShort/CheckPersonUniqueName"),
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData.Rows.length > 0) {
                            if ($("#rdoOrgOrganization").is(":checked")) {
                                //                                MessageBox("Organization with the same corporate name already exist.",
                                //                                    "Organization Info",
                                //                                    [
                                //                                        { Text: 'Ok' }
                                //                                    ]
                                //                                );
                                $('#TitleOrgEmp').text('Organization Info');
                                $('#MsgOrgEmp').text('Organization with the same corporate name already exist.');
                                $('#panOrgEmpMessage').popup('open');
                                $('#btnOKOrgEmp').text('Ok').click(function (e) {
                                    e.preventDefault();
                                    $('#panOrgEmpMessage').popup('close');
                                })
                                $('#divBtnCancel').hide();
                            } else {
                                //                                MessageBox("Person with the same information already exist.",
                                //                                    "Person Info",
                                //                                    [
                                //                                        { Text: 'Ok' }
                                //                                    ]
                                //                                );
                                $('#TitleOrgEmp').text('Person Info');
                                $('#MsgOrgEmp').text('Person with the same information already exist.');
                                $('#panOrgEmpMessage').popup('open');
                                $('#btnOKOrgEmp').text('Ok').click(function (e) {
                                    e.preventDefault();
                                    $('#panOrgEmpMessage').popup('close');
                                })
                                $('#divBtnCancel').hide();
                            }

                        }
                        else {
                            AjaxSend({
                                url: "OrganizationShort/SaveOrganizationInfo",
                                data: pData,
                                success: function (vSaveData) {

                                    if (vSaveData.OrganizationData == "PersonDOBError") {
                                        //                                        MessageBox("Please enter valid date of birth.",
                                        //                                            "Date Of Birth",
                                        //                                            [
                                        //                                                { Text: 'Ok' }
                                        //                                            ]
                                        //                                        );
                                        $('#TitleOrgEmp').text('Date Of Birth');
                                        $('#MsgOrgEmp').text('Please enter valid date of birth.');
                                        $('#panOrgEmpMessage').popup('open');
                                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                                            e.preventDefault();
                                            $('#panOrgEmpMessage').popup('close');
                                        })
                                        $('#divBtnCancel').hide();
                                    } else {

                                        $("#txtOrgID").val(vSaveData.OrganizationData || '');

                                        ////////                                        if (window.pageSourcePage != undefined) {
                                        ////////                                            $.mobile.changePage(window.pageSourcePage);
                                        ////////                                        }
                                        ////////                                        else {
                                        ////////                                            $("#txtOrgID").val(vSaveData.OrganizationData || '');
                                        ////////                                            ChangePage('/OrganizationShort/OrganizationSummary');
                                        ////////                                        }

                                        //                                        window.location = MergeUrlParameters({
                                        //                                            OrgID: sessionStorage.OrgID,
                                        //                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                        //                                        }, '/OrganizationShort/OrganizationSummary');
                                        $('#tblBody').html('');
                                        window.location.hash = 'LibraryPage';
                                        LibraryTable.CustomerList();
                                    }

                                }
                            });

                        }

                    }
                });

            };

            function SaveOrgTradeName() {

                arOrgTradeName = new Array();
                arOrgTradeName[intOrgTradeNameRecID] = $("#txtOrgTradeNameID").val();
                arOrgTradeName[intOrgTradeName] = $("#txtOrgBusinessTradeName").val();
                arOrgTradeName[intOrgTradeNameInitial] = $("#txtOrgBusinessTradeNameInitial").val();
                arOrgTradeName[intOrgTradeNameState] = (IsNewOrgTradeName ? "New" : "Edit");

                sessionStorage["OrgTradeName-" + intOrgTradeNameIndex] = JSON.stringify(arOrgTradeName);

                //$.mobile.changePage('#pageOrganizationInfo');
                $('#pageOrganizationInfo').show();
                $('#pageOrgTradeName').hide();
                LoadOrgTradeNameList();
            };

            function RemoveOrgTradeName() {

                arOrgTradeName = new Array();
                arOrgTradeName[intOrgTradeNameRecID] = $("#txtOrgTradeNameID").val();
                arOrgTradeName[intOrgTradeName] = $("#txtOrgBusinessTradeName").val();
                arOrgTradeName[intOrgTradeNameInitial] = $("#txtOrgBusinessTradeNameInitial").val();
                arOrgTradeName[intOrgTradeNameState] = "Remove";

                sessionStorage["OrgTradeName-" + intOrgTradeNameIndex] = JSON.stringify(arOrgTradeName);

                //$.mobile.changePage('#pageOrganizationInfo');
                $('#pageOrganizationInfo').show();
                $('#pageOrgTradeName').hide();
                LoadOrgTradeNameList();
            };

            function LoadOrgTradeNameList() {
                var pLV = $('#lstOrgTradeName');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgTradeNameCntr; i++) {

                    if (sessionStorage["OrgTradeName-" + i] != undefined) {

                        arOrgTradeName = new Array();
                        arOrgTradeName = JSON.parse(sessionStorage["OrgTradeName-" + i]);

                        if (arOrgTradeName[intOrgTradeNameState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgTradeName)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgTradeNameInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgTradeName[intOrgTradeName])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };

            function LoadOrgCategoryList() {
                var pLV = $('#lstOrgCategory');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgCategoryCntr; i++) {

                    if (sessionStorage["OrgCategory-" + i] != undefined) {

                        arOrgCategory = new Array();
                        arOrgCategory = JSON.parse(sessionStorage["OrgCategory-" + i]);

                        if (arOrgCategory[intOrgCategoryState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgCategory)
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal;")
                                        .text(arOrgCategory[intOrgCategory])
                                    )
                            )
                            .append($('<a>')
                                    .attr('ID', i)
                                    .attr('href', "#")
                                    .data('Data', arOrgCategory)
                                    .click(function (e) {
                                        e.preventDefault();

                                        sessionStorage["OrgCategoryToRemoveIndex"] = $(this).attr('ID');
                                        sessionStorage["OrgCategoryToRemove"] = JSON.stringify($(this).data('Data'));

                                        var arOrgCategoryToRemove = new Array();
                                        arOrgCategoryToRemove = JSON.parse(sessionStorage["OrgCategoryToRemove"]);
                                        intOrgCategoryIndex = sessionStorage["OrgCategoryToRemoveIndex"];

                                        arOrgCategoryToRemove[intOrgCategoryState] = "Remove";
                                        sessionStorage["OrgCategory-" + intOrgCategoryIndex] = JSON.stringify(arOrgCategoryToRemove);

                                        LoadOrgCategoryList();

                                        sessionStorage["OrgCategoryToRemoveIndex"] = null;
                                        sessionStorage["OrgCategoryToRemove"] = null;

                                        //                                        MessageBox("Remove this record?",
                                        //                                            'Remove',
                                        //                                            [
                                        //                                                { Text: 'Yes',
                                        //                                                    Click: function () {

                                        //                                                        var arOrgCategoryToRemove = new Array();
                                        //                                                        arOrgCategoryToRemove = JSON.parse(sessionStorage["OrgCategoryToRemove"]);
                                        //                                                        intOrgCategoryIndex = sessionStorage["OrgCategoryToRemoveIndex"];

                                        //                                                        arOrgCategoryToRemove[intOrgCategoryState] = "Remove";
                                        //                                                        sessionStorage["OrgCategory-" + intOrgCategoryIndex] = JSON.stringify(arOrgCategoryToRemove);

                                        //                                                        LoadOrgCategoryList();

                                        //                                                        sessionStorage["OrgCategoryToRemoveIndex"] = null;
                                        //                                                        sessionStorage["OrgCategoryToRemove"] = null;
                                        //                                                        $('.ui-dialog').dialog('close');
                                        //                                                    }
                                        //                                                }
                                        //                                            ,
                                        //                                                { Text: 'No'
                                        //                                                }
                                        //                                            ]
                                        //                                        );
                                    })
                            );

                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };




        });
    });