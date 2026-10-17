require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryFowstAll.js'),
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

            var IsNewOrgIndustryClass = true;
            var arOrgIndustryClass = new Array();
            var intOrgIndustryClassCntr = 0;
            var intOrgIndustryClassIndex = 0;

            var intOrgIndustryClassRecID = 0;
            var intOrgIndustryClassID = 1;
            var intOrgIndustryClass = 2;
            var intOrgIndustryClassState = 3;

            var IsNewOrgFranchisee = true;
            var arOrgFranchisee = new Array();
            var intOrgFranchiseeCntr = 0;
            var intOrgFranchiseeIndex = 0;

            var intOrgFranchiseeRecID = 0;
            var intOrgFranchiseeID = 1;
            var intOrgFranchiseeOrgName = 2;
            var intOrgFranchiseeName = 3;
            var intOrgFranchiseeIsFranchisorConfirmed = 4;
            var intOrgOrgFranchiseeIsFranchiseeConfirmed = 5;
            var intOrgOrgFranchiseeState = 6;

            var mLoaded = false;
            var isIE = false || !!document.documentMode;

            var IsVAT = false;

            var libname, lbllibname, addlibname, orgid, orgname, NameOfBtnForLibrary;

            setMonthCollection("selOrgDOBMonth");
            setYearCollection("selOrgDOBYear");

            setMonthCollection("selOrgRegistrationDateMonth");
            setYearCollection("selOrgRegistrationDateYear");

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;

                if (!window.IsPageLoaded) {

                    AjaxSend({
                        url: 'Organization/IsNewEntry',
                        success: function (vData) {
                            libname = sessionStorage.LibraryName;
                            orgid = sessionStorage.ViewID;
                            orgname = sessionStorage.OrgName;
                            ShowOrganizationInfo(vData.OrganizationID || '');
                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            window.OrgInfo = {
                ShowOrgInfo: function () {
                    setMonthCollection("selOrgDOBMonth");
                    setYearCollection("selOrgDOBYear");

                    setMonthCollection("selOrgRegistrationDateMonth");
                    setYearCollection("selOrgRegistrationDateYear");

                    libname = sessionStorage.LibraryName;
                    orgid = sessionStorage.ViewID;
                    orgname = sessionStorage.OrgName;
                    ShowOrganizationInfo(sessionStorage.OrgID);
                }
            }
            $('#btnBackOrgInfo').click(function () {
                history.back();
            });

            $('body').on('change', 'input[name="rdoOrgType"]', function (e) {
                e.preventDefault();

                if ($("#rdoOrgOrganization").is(":checked")) {
                    $("#divOrgOrganization").show();
                    $("#divOrgPerson").hide();
                } else {
                    $("#divOrgOrganization").hide();
                    $("#divOrgPerson").show();
                }
            })
            .on('click', '#rdoOrgPerson', function (e) {
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
                SearchTitlePopUp();
                //                Browser.LibraryPeoplePreSuffix.Click = function (e) {

                //                    $('#txtOrgTitleID').val(e.ID)
                //                    $('#btnOrgTitle').text(e.Code)
                //                    //.button('refresh');

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
                    $('#selOrgDOBYear')//.selectmenu('refresh');
                }
            })
            .on('click', '#btnOrgPersonAddressCity', function (e) {
                NameOfBtnForLibrary = 'PersonAddress';
                SearchCityPopUp();
                //                Browser.LibraryCities.Click = function (e) {

                //                    $('#txtOrgPersonAddressCityID').val(e.ID)
                //                    $('#btnOrgPersonAddressCity').text(e.Name)
                //                    //.button('refresh');

                //                };
                //                Browser.LibraryCities.Show();
            })
            .on('click', '#btnOrgPersonAddressCountry', function (e) {
                NameOfBtnForLibrary = 'PersonAddress';
                SearchCountryPopUp();
                //                Browser.LibraryCountries.Click = function (e) {

                //                    $('#txtOrgPersonAddressCountryID').val(e.ID)
                //                    $('#btnOrgPersonAddressCountry').text(e.Name)
                //                    //.button('refresh');

                //                };
                //                Browser.LibraryCountries.Show();
            })
            .on('change', '#selOrgRegistrationType', function (e) {
                e.preventDefault();

                SetOrgRegistrationInfoLabels();
            })
            .on('click', '#btnOrgOfficeAddressCity', function (e) {
                NameOfBtnForLibrary = 'OrgOfficeAddress';
                SearchCityPopUp();
                //                Browser.LibraryCities.Click = function (e) {

                //                    $('#txtOrgOfficeAddressCityID').val(e.ID)
                //                    $('#btnOrgOfficeAddressCity').text(e.Name)
                //                    //.button('refresh');

                //                };
                //                Browser.LibraryCities.Show();
            })
            .on('click', '#btnOrgOfficeAddressCountry', function (e) {
                NameOfBtnForLibrary = 'OrgOfficeAddress';
                SearchCountryPopUp();
                //                Browser.LibraryCountries.Click = function (e) {

                //                    $('#txtOrgOfficeAddressCountryID').val(e.ID)
                //                    $('#btnOrgOfficeAddressCountry').text(e.Name)
                //                    //.button('refresh');

                //                };
                //                Browser.LibraryCountries.Show();
            })
            .on('change', '#chkOrgSameAsPricipalAddress', function (e) {
                e.preventDefault();

                if ($(this).is(":checked")) {

                    $("#txtOrgBusinessAddressBuildingNo").val($("#txtOrgOfficeAddressBuildingNo").val());
                    $("#txtOrgBusinessAddressStreet").val($("#txtOrgOfficeAddressStreet").val());
                    $("#txtOrgBusinessAddressCityID").val($("#txtOrgOfficeAddressCityID").val());
                    $("#btnOrgBusinessAddressCity").text($("#btnOrgOfficeAddressCity").text())//.button('refresh');
                    $("#txtOrgBusinessAddressCountryID").val($("#txtOrgOfficeAddressCountryID").val());
                    $("#btnOrgBusinessAddressCountry").text($("#btnOrgOfficeAddressCountry").text())//.button('refresh');

                }
            })
            .on('click', '#btnOrgBusinessAddressCity', function (e) {
                NameOfBtnForLibrary = 'OrgBusinessAddress';
                SearchCityPopUp();
                //                Browser.LibraryCities.Click = function (e) {

                //                    $('#txtOrgBusinessAddressCityID').val(e.ID)
                //                    $('#btnOrgBusinessAddressCity').text(e.Name)
                //                    //.button('refresh');

                //                };
                //                Browser.LibraryCities.Show();
            })
            .on('click', '#btnOrgBusinessAddressCountry', function (e) {
                NameOfBtnForLibrary = 'OrgBusinessAddress';
                SearchCountryPopUp();
                //                Browser.LibraryCountries.Click = function (e) {

                //                    $('#txtOrgBusinessAddressCountryID').val(e.ID)
                //                    $('#btnOrgBusinessAddressCountry').text(e.Name)
                //                    //.button('refresh');

                //                };
                //                Browser.LibraryCountries.Show();
            })
            .on('click', '#btnAddOrgTradeName', function (e) {
                e.preventDefault();

                IsNewOrgTradeName = true
                intOrgTradeNameCntr = intOrgTradeNameCntr + 1;
                intOrgTradeNameIndex = intOrgTradeNameCntr;

                //$.mobile.changePage('#pageOrgTradeName');
                $('#pageOrgTradeName').show();
                $('#pageOrganizationInfo').hide();
                ClearOrgTradeNameFields();
            })
            .on('click', '#btnOrgTradeNameSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgBusinessTradeName").val()) == "") {
                    NewMessageBox('Please enter business or trade name.');
                    return;
                }

                SaveOrgTradeName();
            })
            .on('click', '#btnOrgTradeNameRemove', function (e) {
                e.preventDefault();
                NewMessageBox('Remove this record?', 'Confirmation Message', [
                    {
                        Text: 'Yes', Click: function () {
                            RemoveOrgTradeName();
                        }
                    }, { Text: 'No '}
                ]);
            })
            .on('click', '#btnOrgTradeNameCancel', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrganizationInfo');
                $('#pageOrgTradeName').hide();
                $('#pageOrganizationInfo').show();
            })
            .on('click', '#btnAddOrgCategory', function (e) {
                NameOfBtnForLibrary = 'OrgCategory';
                SearchCategoryPopUp();
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
            .on('click', '#btnAddOrgIndustryClassification', function (e) {
                NameOfBtnForLibrary = 'OrgCategory';
                SearchIndustryClassPopUp();
                //                Browser.LibraryIndustryClassification.Click = function (e) {

                //                    IsNewOrgIndustryClass = true
                //                    intOrgIndustryClassCntr = intOrgIndustryClassCntr + 1;
                //                    intOrgIndustryClassIndex = intOrgIndustryClassCntr;

                //                    arOrgIndustryClass = new Array();
                //                    arOrgIndustryClass[intOrgIndustryClassRecID] = "";
                //                    arOrgIndustryClass[intOrgIndustryClassID] = e.ID;
                //                    arOrgIndustryClass[intOrgIndustryClass] = e.Class;
                //                    arOrgIndustryClass[intOrgIndustryClassState] = (IsNewOrgIndustryClass ? "New" : "Edit");

                //                    sessionStorage["OrgIndustryClass-" + intOrgIndustryClassIndex] = JSON.stringify(arOrgIndustryClass);

                //                    LoadOrgIndustryClassList();

                //                };

                //                Browser.LibraryIndustryClassification.Show();
            })
            .on('click', '#btnAddOrgFranchisee', function (e) {
                Browser.LibraryFowstAll.Click = function (e) {

                    IsNewOrgFranchisee = true
                    intOrgFranchiseeCntr = intOrgFranchiseeCntr + 1;
                    intOrgFranchiseeIndex = intOrgFranchiseeCntr;

                    arOrgFranchisee = new Array();
                    arOrgFranchisee[intOrgFranchiseeRecID] = "";
                    arOrgFranchisee[intOrgFranchiseeID] = e.ID;
                    arOrgFranchisee[intOrgFranchiseeOrgName] = e.FowstOrgName;
                    arOrgFranchisee[intOrgFranchiseeName] = e.FowstName;
                    arOrgFranchisee[intOrgFranchiseeIsFranchisorConfirmed] = 1;
                    arOrgFranchisee[intOrgOrgFranchiseeIsFranchiseeConfirmed] = 0;
                    arOrgFranchisee[intOrgOrgFranchiseeState] = (IsNewOrgFranchisee ? "New" : "Edit");

                    sessionStorage["OrgFranchisee-" + intOrgFranchiseeIndex] = JSON.stringify(arOrgFranchisee);

                    LoadOrgFranchiseeList();

                };

                Browser.LibraryFowstAll.Show();
            })
            .on('click', '#btnOrgInfoDone', function (e) {
                e.preventDefault();

                if ($("#rdoOrgOrganization").is(":checked")) {
                    if ($.trim($("#txtOrgCorporateName").val()) == "" || $.trim($("#txtOrgCorporateName").val()).length <= 1) {
                        NewMessageBox('Please enter corporate name.');
                        return;
                    }

                    if ($.trim($("#selOrgRegistrationType").val()) == "") {
                        NewMessageBox('Please select registration type.');
                        return;
                    }

                    if ($.trim($("#selOrgRegistrationType").val()) != "Unregistered") {
                        if ($.trim($("#txtOrgRegistrationNo").val()) == "" || $.trim($("#txtOrgRegistrationNo").val()).length <= 1) {
                            NewMessageBox('Please enter registration number.');
                            return;
                        }

                        if ($.trim($("#selOrgRegistrationDateMonth").val()) == "" || $.trim($("#selOrgRegistrationDateMonth").val()) == "" || $.trim($("#selOrgRegistrationDateMonth").val()) == "") {
                            NewMessageBox('Please enter valid registration date.');
                            return;
                        }
                    }

                    if ($.trim($("#txtOrgCorporateTIN").val()) == "" || $.trim($("#txtOrgCorporateTIN").val()).length <= 1) {
                        NewMessageBox('Please enter Corporate Tax Identification No. (TIN).');
                        return;
                    }

                    if ($.trim($("#txtOrgOfficeAddressStreet").val()) == "" || $.trim($("#txtOrgOfficeAddressStreet").val()).length <= 1) {
                        NewMessageBox('Please enter office address street.');
                        return;
                    }

                    if ($.trim($("#txtOrgOfficeAddressCityID").val()) == "") {
                        NewMessageBox('Please select office address city.');
                        return;
                    }

                    if ($.trim($("#txtOrgOfficeAddressCountryID").val()) == "") {
                        NewMessageBox('Please select office address country.');
                        return;
                    }

                    if ($.trim($("#txtOrgBusinessAddressStreet").val()) == "" || $.trim($("#txtOrgBusinessAddressStreet").val()).length <= 1) {
                        NewMessageBox('Please enter business address street.');
                        return;
                    }

                    if ($.trim($("#txtOrgBusinessAddressCityID").val()) == "") {
                        NewMessageBox('Please select business address city.');
                        return;
                    }

                    if ($.trim($("#txtOrgBusinessAddressCountryID").val()) == "") {
                        NewMessageBox('Please select business address country.');
                        return;
                    }

                    if (!$("#rdoOrgOrganizationVAT").is(":checked") && !$("#rdoOrgOrganizationNoneVAT").is(":checked")) {
                        NewMessageBox('Please select VAT Type.');
                        return;
                    }

                    if ($("#rdoOrgOrganizationNoneVAT").is(":checked") && IsVAT) {
                        NewMessageBox('Sorry, You cannot change from VAT to None-VAT Type.');
                        return;
                    }
                } else {
                    if ($.trim($("#txtOrgSurname").val()) == "" || $.trim($("#txtOrgSurname").val()).length <= 1) {
                        NewMessageBox('Please enter surname.');
                        return;
                    }

                    if ($.trim($("#txtOrgFirstName").val()) == "" || $.trim($("#txtOrgFirstName").val()).length <= 1) {
                        NewMessageBox('Please enter first name.');
                        return;
                    }

                    if ($.trim($("#txtOrgMiddleName").val()) == "" || $.trim($("#txtOrgMiddleName").val()).length <= 1) {
                        NewMessageBox('Please enter middle name.');
                        return;
                    }

                    if ($.trim($("#selOrgDOBMonth").val()) == "") {
                        NewMessageBox('Please enter valid date of birth.');
                        return;
                    }
                    if ($.trim($("#selOrgDOBDay").val()) == "") {
                        NewMessageBox('Please enter valid date of birth.');
                        return;
                    }
                    if ($.trim($("#selOrgDOBYear").val()) == "") {
                        NewMessageBox('Please enter valid date of birth.');
                        return;
                    }

                    if ($.trim($("#txtOrgMotherMaidenSurname").val()) == "" || $.trim($("#txtOrgMotherMaidenSurname").val()).length <= 1) {
                        NewMessageBox('Please enter mother maiden surname.');
                        return;
                    }
                    if ($.trim($("#txtOrgMotherMaidenFirstName").val()) == "" || $.trim($("#txtOrgMotherMaidenFirstName").val()).length <= 1) {
                        NewMessageBox('Please enter mother first name.');
                        return;
                    }
                }

                SaveOrganizationInfo();
            })
            .on('change', '#selOrgRegistrationDateMonth', function (e) {
                e.preventDefault();

                setDayCollection('selOrgRegistrationDateMonth', 'selOrgRegistrationDateDay', 'selOrgRegistrationDateYear', true);
            })
            .on('change', '#selOrgRegistrationDateYear', function (e) {
                e.preventDefault();

                setDayCollection('selOrgRegistrationDateMonth', 'selOrgRegistrationDateDay', 'selOrgRegistrationDateYear', true);
            })
            .on('click', '#selOrgRegistrationDateYear', function (e) {
                e.preventDefault();

                if ($.trim($('#selOrgRegistrationDateDay').val()) == "") {
                    $('#selOrgRegistrationDateDay').val(new Date().getFullYear());
                    $('#selOrgRegistrationDateDay')//.selectmenu('refresh');
                }
            })
            .on('keydown', '#txtOrgNoOfRetailOutlet', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtOrgNoOfOffice', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtOrgNoOfWarehouse', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtOrgNoOfEmployees', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('change', '#chkOrgIsFranchisor', function (e) {
                e.preventDefault();

                ShowHideFranchisee();
            })
            .on('click', '#btnOrgInfoCancel', function () {
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
                    else if ($('#TitleLibrary').text() == "CATEGORY") {
                        SearchCategoryPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "INDUSTRY CLASSIFICATION") {
                        SearchIndustryClassPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "TITLE") {
                        SearchTitlePopUp($(this).val());
                    }
                }
            })
            .on('click', '#btnAddOrgLogo', function () {
                $('#popupOrgLogo').modal('show');
            })
            .on('click', '#btnUploadOrgLogo', function () {
                $('#fileUploadOrgLogo').click();
            })
            .on('change', '#fileUploadOrgLogo', function () {
                var file = $(this)[0].files[0];
                var reader = new FileReader();
                var imgTempOrgLogo = $('#imgTempOrgLogo');
                var formdata = new FormData();

                formdata.append(file.name, file);

                $('#fileUploadOrgLogo').val('');

                reader.onload = function () {
                    var dataURL = reader.result;
                    imgTempOrgLogo.attr('src', dataURL);

                    setTimeout(function () {
                        if (file.type != "image/png") {
                            WebToast('Must be a PNG file', 'top');
                            $(this).val('');
                            return;
                        }

                        //if (imgTempOrgLogo.innerWidth() > 170 || imgTempOrgLogo.innerHeight() > 70) {
                        //    WebToast('Selected logo\'s dimensions must not be more than 170 x 70', 'top');
                        //    $(this).val('');
                        //    return;
                        //}

                        if (file.size > 512000) { // 500kb
                            WebToast('File size must not be more than 500kb', 'top');
                            $(this).val('');
                            return;
                        }

                        Bs.Loading('show', 'Please wait while uploading your logo ...');

                        AjaxSend({
                            url: 'Organization/SaveOrgLogo?OrgID=' + sessionStorage.OrgID,
                            type: 'POST',
                            data: formdata,
                            async: true,
                            cache: false,
                            contentType: false,
                            processData: false,
                            success: function (vData) {
                                Bs.Loading('hide');
                                if (vData.Results == "saved") {
                                    WebToast('Logo has been uploaded', 'top');
                                    ShowOrganizationInfo(sessionStorage.OrgID);
                                } else {
                                    NewMessagaBox(vData.Results, "Message");
                                }
                            }
                        });
                    }, 500)                    
                };
                reader.readAsDataURL(file);
            })
            .on('click', '#btnRemoveOrgLogo', function () {
                NewMessageBox('Are you sure you want to remove this logo?', 'Message', [
                    {
                        Text: 'Yes', Click: function () {
                            AjaxSend({
                                url: 'Organization/SaveOrgLogo?OrgID=' + sessionStorage.OrgID,
                                success: function (vData) {
                                    if (vData.Results == "saved") {
                                        WebToast('Logo has been removed', 'top');
                                        ShowOrganizationInfo(sessionStorage.OrgID);
                                        $('#popupOrgLogo').modal('hide');
                                    } else {
                                        NewMessagaBox(vData.Results, "Message");
                                    }
                                }
                            });
                        }
                    }, { Text: 'No' }
                ])                
            })

            function SearchTitlePopUp(s) {
                BrowsePopup('LibraryPeoplePreSuffix', 'Search Title', [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'Code' }, { Label: 'Description', Column: 'Description' }],
                        Click: function (e) {
                            $('#txtOrgTitleID').val(e.ID);
                            $('#btnOrgTitle').text(e.Code)//.button('refresh');
                        }
                    }
                ])
            }

            function SearchIndustryClassPopUp(s) {
                BrowsePopup('LibraryIndustryClassification', 'Search Industry Classification', [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'Class' }, { Label: 'Description', Column: 'Description' }],
                        Click: function (e) {
                            if (NameOfBtnForLibrary == "OrgCategory") {
                                IsNewOrgIndustryClass = true
                                intOrgIndustryClassCntr = intOrgIndustryClassCntr + 1;
                                intOrgIndustryClassIndex = intOrgIndustryClassCntr;

                                arOrgIndustryClass = new Array();
                                arOrgIndustryClass[intOrgIndustryClassRecID] = "";
                                arOrgIndustryClass[intOrgIndustryClassID] = e.ID;
                                arOrgIndustryClass[intOrgIndustryClass] = e.Class;
                                arOrgIndustryClass[intOrgIndustryClassState] = (IsNewOrgIndustryClass ? "New" : "Edit");

                                sessionStorage["OrgIndustryClass-" + intOrgIndustryClassIndex] = JSON.stringify(arOrgIndustryClass);

                                LoadOrgIndustryClassList();
                            }
                        }
                    }
                ])
            }

            function SearchCategoryPopUp(s) {
                BrowsePopup('LibraryOrganizationCategory', 'Search Category', [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Label: 'Type', Column: 'CategoryType' }, { Label: 'Category', Column: 'Category' }],
                        Click: function (e) {
                            if (NameOfBtnForLibrary == "OrgCategory") {
                                if (NameOfBtnForLibrary == "OrgCategory") {
                                    IsNewOrgCategory = true
                                    intOrgCategoryCntr = intOrgCategoryCntr + 1;
                                    intOrgCategoryIndex = intOrgCategoryCntr;

                                    arOrgCategory = new Array();
                                    arOrgCategory[intOrgCategoryRecID] = "";
                                    arOrgCategory[intOrgCategoryTypeID] = e.ID;
                                    arOrgCategory[intOrgCategory] = e.CategoryType;
                                    arOrgCategory[intOrgCategoryState] = (IsNewOrgCategory ? "New" : "Edit");

                                    sessionStorage["OrgCategory-" + intOrgCategoryIndex] = JSON.stringify(arOrgCategory);

                                    LoadOrgCategoryList();
                                }
                            }
                        }
                    }
                ]);
            }

            function SearchCountryPopUp(s) {
                BrowsePopup('LibraryCountries', 'Select Country', [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'Name' }],
                        Click: function (e) {
                            if (NameOfBtnForLibrary == "OrgOfficeAddress") {
                                $('#txtOrgOfficeAddressCountryID').val(e.ID);
                                $('#btnOrgOfficeAddressCountry').text(e.Name)//.button('refresh');
                            }
                            else if (NameOfBtnForLibrary == "OrgBusinessAddress") {
                                $('#txtOrgBusinessAddressCountryID').val(e.ID);
                                $('#btnOrgBusinessAddressCountry').text(e.Name)//.button('refresh');
                            }
                            else if (NameOfBtnForLibrary == "PersonAddress") {
                                $('#txtOrgPersonAddressCountryID').val(e.ID);
                                $('#btnOrgPersonAddressCountry').text(e.Name)//.button('refresh');
                            }
                        }
                    }
                ])
            }

            function SearchCityPopUp(s) {
                BrowsePopup('LibraryCities', 'Select City', [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'Name' }],
                        Click: function (e) {
                            if (NameOfBtnForLibrary == "OrgOfficeAddress") {
                                $('#txtOrgOfficeAddressCityID').val(e.ID);
                                $('#btnOrgOfficeAddressCity').text(e.Name)//.button('refresh');
                            }
                            else if (NameOfBtnForLibrary == "OrgBusinessAddress") {
                                $('#txtOrgBusinessAddressCityID').val(e.ID);
                                $('#btnOrgBusinessAddressCity').text(e.Name)//.button('refresh');
                            }
                            else if (NameOfBtnForLibrary == "PersonAddress") {
                                $('#txtOrgPersonAddressCityID').val(e.ID);
                                $('#btnOrgPersonAddressCity').text(e.Name)//.button('refresh');
                            }
                        }
                    }
                ])
            }

            function ShowHideFranchisee() {
                if ($("#chkOrgIsFranchisor").is(":checked")) {
                    $("#divFranchiseeContainer").show();
                } else {
                    $("#divFranchiseeContainer").hide();
                }
            }

            function SetOrgRegistrationInfoLabels() {

                if ($("#selOrgRegistrationType").val() == "SEC") {
                    $("#lblOrgRegistrationNo").text("SEC Registration No.: ");
                    $("#lblOrgRegistrationDate").text("SEC Registration Date: ");
                }
                else if ($("#selOrgRegistrationType").val() == "DTI") {
                    $("#lblOrgRegistrationNo").text("DTI Registration No.: ");
                    $("#lblOrgRegistrationDate").text("DTI Registration Date: ");
                }
                else {
                    $("#lblOrgRegistrationNo").text("Registration No.: ");
                    $("#lblOrgRegistrationDate").text("Registration Date: ");
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

                            $('#rdoOrgOrganization').attr("checked", (vData.OrganizationData.Rows[0].IsOrganization == 1 ? true : false))//.checkboxradio("refresh");
                            $('#rdoOrgPerson').attr("checked", (vData.OrganizationData.Rows[0].IsOrganization == 0 ? true : false))//.checkboxradio("refresh");

                            $("#txtOrgID").val(vData.OrganizationData.Rows[0].ID || '');

                            if ($("#rdoOrgOrganization").is(":checked")) {

                                $("#divOrgOrganization").show();
                                $("#divOrgPerson").hide();

                                $("#txtOrgCorporateName").val(vData.OrganizationData.Rows[0].CompanyName || '');
                                $("#txtOrgCorporateNameInitial").val(vData.OrganizationData.Rows[0].CompanyNameInitial || '');
                                $("#selOrgOwnershipType").val(vData.OrganizationData.Rows[0].OwnershipType || '')//.selectmenu('refresh');

                                $('#chkOrgIsFranchisor').attr("checked", (vData.OrganizationData.Rows[0].IsMobizFranchisorRequestSent == 0 ? false : true))//.checkboxradio("refresh");
                                ShowHideFranchisee();
                                if (vData.OrganizationData.Rows[0].IsMobizFranchisorRequestSent == 1 && vData.OrganizationData.Rows[0].IsMobizConfirmedFranchisor == 1)
                                    $('#divFranchisorToBeApproved').hide();
                                else
                                    $('#divFranchisorToBeApproved').show();


                                $("#selOrgRegistrationType").val(vData.OrganizationData.Rows[0].RegistrationType || '')//.selectmenu('refresh');
                                $("#txtOrgRegistrationNo").val(vData.OrganizationData.Rows[0].RegistrationNo || '');
                                $("#selOrgRegistrationDateMonth").val(vData.OrganizationData.Rows[0].RegistrationDateMonth || '')//.selectmenu('refresh');
                                $("#selOrgRegistrationDateYear").val(vData.OrganizationData.Rows[0].RegistrationDateYear || '')//.selectmenu('refresh');
                                setDayCollection('selOrgRegistrationDateMonth', 'selOrgRegistrationDateDay', 'selOrgRegistrationDateYear', false);
                                $("#selOrgRegistrationDateDay").val(vData.OrganizationData.Rows[0].RegistrationDateDay || '')//.selectmenu('refresh');
                                SetOrgRegistrationInfoLabels();

                                $("#txtOrgCorporateTIN").val(vData.OrganizationData.Rows[0].TIN || '');

                                $("#txtOrgOfficeAddressBuildingNo").val(vData.OrganizationData.Rows[0].OfficeAddressBuildingNo || '');
                                $("#txtOrgOfficeAddressStreet").val(vData.OrganizationData.Rows[0].OfficeAddressStreet || '');
                                $("#txtOrgOfficeAddressCityID").val(vData.OrganizationData.Rows[0].OfficeAddressCityID || '');
                                $("#btnOrgOfficeAddressCity").text(vData.OrganizationData.Rows[0].OfficeAddressCity || 'Please Select')//.button('refresh');
                                $("#txtOrgOfficeAddressCountryID").val(vData.OrganizationData.Rows[0].OfficeAddressCountryID || '');
                                $("#btnOrgOfficeAddressCountry").text(vData.OrganizationData.Rows[0].OfficeAddressCountry || 'Please Select')//.button('refresh');

                                $("#txtOrgBusinessAddressBuildingNo").val(vData.OrganizationData.Rows[0].BusinessAddressBuildingNo || '');
                                $("#txtOrgBusinessAddressStreet").val(vData.OrganizationData.Rows[0].BusinessAddressStreet || '');
                                $("#txtOrgBusinessAddressCityID").val(vData.OrganizationData.Rows[0].BusinessAddressCityID || '');
                                $("#btnOrgBusinessAddressCity").text(vData.OrganizationData.Rows[0].BusinessAddressCity || 'Please Select')//.button('refresh');
                                $("#txtOrgBusinessAddressCountryID").val(vData.OrganizationData.Rows[0].BusinessAddressCountryID || '');
                                $("#btnOrgBusinessAddressCountry").text(vData.OrganizationData.Rows[0].BusinessAddressCountry || 'Please Select')//.button('refresh');

                                $("#txtOrgPurpose").val(vData.OrganizationData.Rows[0].Purpose || '').keyup();
                                $("#selOrgBusinessScope").val(vData.OrganizationData.Rows[0].BusinessScope || '')//.selectmenu('refresh');
                                $("#txtOrgNoOfRetailOutlet").val(vData.OrganizationData.Rows[0].NoOfRetailOutlet || '');
                                $("#txtOrgNoOfOffice").val(vData.OrganizationData.Rows[0].NoOfOffice || '');
                                $("#txtOrgNoOfWarehouse").val(vData.OrganizationData.Rows[0].NoOfWarehouse || '');
                                $("#txtOrgNoOfEmployees").val(vData.OrganizationData.Rows[0].NoOfEmployees || '');

                                if(CStr2(vData.OrganizationData.Rows[0].LogoFileName) != ''){
                                    $('#imgOrgLogo').attr('src', '/Content/OrganizationLogo/' + vData.OrganizationData.Rows[0].LogoFileName);
                                    $('#btnRemoveOrgLogo').show();
                                } else {
                                    $('#imgOrgLogo').removeAttr('src');
                                    $('#btnRemoveOrgLogo').hide();
                                }

                                AjaxSend({
                                    url: "Organization/getOrgTradeNameList",
                                    data: pData,
                                    success: function (vDataTradeName) {

                                        LoadOrgTradeName(vDataTradeName.OrganizationData.Rows);

                                    }
                                });

                                AjaxSend({
                                    url: "Organization/getOrgCategoryList",
                                    data: pData,
                                    success: function (vDataCategory) {

                                        LoadOrgCategory(vDataCategory.OrganizationData.Rows);

                                    }
                                });

                                AjaxSend({
                                    url: "Organization/getOrganizationIndustryClassList",
                                    data: pData,
                                    success: function (vDataIndustryClass) {

                                        LoadOrgIndustryClass(vDataIndustryClass.OrganizationData.Rows);

                                    }
                                });

                                //                                AjaxSend({
                                //                                    url: "Organization/getOrgFranchiseeList",
                                //                                    data: pData,
                                //                                    success: function (vDataFranchisee) {

                                //                                        LoadOrgFranchisee(vDataFranchisee.OrganizationData.Rows);

                                //                                    }
                                //                                });


                            } else {

                                $("#divOrgOrganization").hide();
                                $("#divOrgPerson").show();

                                $("#txtOrgTitleID").val(vData.OrganizationData.Rows[0].PersonTitleID || '');
                                $("#btnOrgTitle").text(vData.OrganizationData.Rows[0].PersonTitle || 'Please Select')//.button('refresh');

                                $("#txtOrgSurname").val(vData.OrganizationData.Rows[0].PersonSurname || '');
                                $("#txtOrgFirstName").val(vData.OrganizationData.Rows[0].PersonFirstName || '');
                                $("#txtOrgMiddleName").val(vData.OrganizationData.Rows[0].PersonMiddleName || '');

                                $("#selOrgPersonalSuffixName").val(vData.OrganizationData.Rows[0].PersonSuffixName || '')//.selectmenu('refresh');

                                $("#selOrgDOBMonth").val(vData.OrganizationData.Rows[0].PersonDateOfBirthMonth || '')//.selectmenu('refresh');
                                $("#selOrgDOBYear").val(vData.OrganizationData.Rows[0].PersonDateOfBirthYear || '')//.selectmenu('refresh');
                                setDayCollection('selOrgDOBMonth', 'selOrgDOBDay', 'selOrgDOBYear', true);
                                $("#selOrgDOBDay").val(vData.OrganizationData.Rows[0].PersonDateOfBirthDay || '')//.selectmenu('refresh');

                                $("#txtOrgMotherMaidenSurname").val(vData.OrganizationData.Rows[0].PersonMotherMaidenSurname || '');
                                $("#txtOrgMotherMaidenFirstName").val(vData.OrganizationData.Rows[0].PersonMotherMaidenFirstName || '');
                                $("#txtOrgMotherMaidenMiddleName").val(vData.OrganizationData.Rows[0].PersonMotherMaidenMiddleName || '');

                                $("#txtOrgPersonAddressBuildingNo").val(vData.OrganizationData.Rows[0].OfficeAddressBuildingNo || '');
                                $("#txtOrgPersonAddressStreet").val(vData.OrganizationData.Rows[0].OfficeAddressStreet || '');

                                $("#txtOrgPersonAddressCityID").val(vData.OrganizationData.Rows[0].OfficeAddressCityID || '');
                                $("#btnOrgPersonAddressCity").text(vData.OrganizationData.Rows[0].OfficeAddressCity || 'Please Select')//.button('refresh');

                                $("#txtOrgPersonAddressCountryID").val(vData.OrganizationData.Rows[0].OfficeAddressCountryID || '');
                                $("#btnOrgPersonAddressCountry").text(vData.OrganizationData.Rows[0].OfficeAddressCountry || 'Please Select')//.button('refresh');

                            }

                            if (vData.OrganizationData.Rows[0].IsVAT != undefined) {

                                if (CBool(vData.OrganizationData.Rows[0].IsVAT)) {
                                    $("#rdoOrgOrganizationVAT").attr("checked", true);
                                    IsVAT = true;
                                } else {
                                    $("#rdoOrgOrganizationNoneVAT").attr("checked", true);
                                }
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

            function LoadOrgIndustryClass(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgIndustryClass = new Array();
                    arOrgIndustryClass[intOrgIndustryClassRecID] = vRows[p].ID;
                    arOrgIndustryClass[intOrgIndustryClassID] = vRows[p].IndustryClassID;
                    arOrgIndustryClass[intOrgIndustryClass] = vRows[p].IndustryClass;
                    arOrgIndustryClass[intOrgIndustryClassState] = "";

                    sessionStorage["OrgIndustryClass-" + p] = JSON.stringify(arOrgIndustryClass);

                };

                intOrgIndustryClassCntr = vRows.length;
                LoadOrgIndustryClassList();

            };

            function LoadOrgFranchisee(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgFranchisee = new Array();
                    arOrgFranchisee[intOrgFranchiseeRecID] = vRows[p].ID;
                    arOrgFranchisee[intOrgFranchiseeID] = vRows[p].FranchiseeID;
                    arOrgFranchisee[intOrgFranchiseeOrgName] = vRows[p].FranchiseeOrgName;
                    arOrgFranchisee[intOrgFranchiseeName] = vRows[p].BranchName;
                    arOrgFranchisee[intOrgFranchiseeIsFranchisorConfirmed] = vRows[p].IsFranchisorConfirmed;
                    arOrgFranchisee[intOrgOrgFranchiseeIsFranchiseeConfirmed] = vRows[p].IsFranchiseeConfirmed;
                    arOrgFranchisee[intOrgOrgFranchiseeState] = "";

                    sessionStorage["OrgFranchisee-" + p] = JSON.stringify(arOrgFranchisee);

                };

                intOrgFranchiseeCntr = vRows.length;
                LoadOrgFranchiseeList();

            };

            function ShowOrgFranchiseeConfirmInfo(arData, index) {

                //                $.mobile.changePage('#pageOrgTradeName');

                //                ClearOrgTradeNameFields();

                //                intOrgTradeNameIndex = index;

                //                $("#txtOrgTradeNameID").val(arData[intOrgTradeNameRecID] || '');
                //                $("#txtOrgBusinessTradeName").val(arData[intOrgTradeName] || '');
                //                $("#txtOrgBusinessTradeNameInitial").val(arData[intOrgTradeNameInitial] || '');

                //                IsNewTradeName = false;

                //                $("#btnTradeNameRemove").show();

            };

            function ShowOrgTradeNameInfo(arData, index) {

                //$.mobile.changePage('#pageOrgTradeName');
                $('#pageOrgTradeName').show();
                $('#pageOrganizationInfo').hide();

                ClearOrgTradeNameFields();

                intOrgTradeNameIndex = index;

                $("#txtOrgTradeNameID").val(arData[intOrgTradeNameRecID] || '');
                $("#txtOrgBusinessTradeName").val(arData[intOrgTradeName] || '');
                $("#txtOrgBusinessTradeNameInitial").val(arData[intOrgTradeNameInitial] || '');

                IsNewTradeName = false;

                $("#btnOrgTradeNameRemove").show();

            };

            function ClearOrganizationFields() {

                // dont clear
                // sessionStorage.clear();
                //proj.ClearSessionStorage(); commented by arnie 12202016 for erp

                sessionStorage["LibraryName"] = libname;
                sessionStorage["ViewID"] = orgid;
                sessionStorage["OrgName"] = orgname;

                $("#txtOrgID").val('');

                $("#txtOrgTitleID").val('');
                $("#btnOrgTitle").text('Please Select')//.button('refresh');

                $("#txtOrgSurname").val('');
                $("#txtOrgFirstName").val('');
                $("#txtOrgMiddleName").val('');

                $("#selOrgPersonalSuffixName").val('')//.selectmenu('refresh');

                $("#selOrgDOBMonth").val('')//.selectmenu('refresh');
                $("#selOrgDOBYear").val('')//.selectmenu('refresh');
                $("#selOrgDOBDay").val('')//.selectmenu('refresh');

                $("#txtOrgMotherMaidenSurname").val('');
                $("#txtOrgMotherMaidenFirstName").val('');
                $("#txtOrgMotherMaidenMiddleName").val('');

                $("#txtOrgPersonAddressBuildingNo").val('');
                $("#txtOrgPersonAddressStreet").val('');

                $("#txtOrgPersonAddressCityID").val('');
                $("#btnOrgPersonAddressCity").text('Please Select')//.button('refresh');

                $("#txtOrgPersonAddressCountryID").val('');
                $("#btnOrgPersonAddressCountry").text('Please Select')//.button('refresh');

                $("#txtOrgCorporateName").val('');
                $("#txtOrgCorporateNameInitial").val('');
                $("#selOrgOwnershipType").val('')//.selectmenu('refresh');

                $('#chkOrgIsFranchisor').attr("checked", false)//.checkboxradio("refresh");
                ShowHideFranchisee();

                $("#selOrgRegistrationType").val('')//.selectmenu('refresh');
                $("#txtOrgRegistrationNo").val('');
                $("#selOrgRegistrationDateMonth").val('')//.selectmenu('refresh');
                $("#selOrgRegistrationDateDay").val('')//.selectmenu('refresh');
                $("#selOrgRegistrationDateYear").val('')//.selectmenu('refresh');

                $("#txtOrgCorporateTIN").val('');

                $("#txtOrgOfficeAddressBuildingNo").val('');
                $("#txtOrgOfficeAddressStreet").val('');
                $("#txtOrgOfficeAddressCityID").val('');
                $("#btnOrgOfficeAddressCity").text('Please Select')//.button('refresh');
                $("#txtOrgOfficeAddressCountryID").val('');
                $("#btnOrgOfficeAddressCountry").text('Please Select')//.button('refresh');

                $('#chkOrgSameAsPricipalAddress').attr("checked", false)//.checkboxradio("refresh");

                $("#txtOrgBusinessAddressBuildingNo").val('');
                $("#txtOrgBusinessAddressStreet").val('');
                $("#txtOrgBusinessAddressCityID").val('');
                $("#btnOrgBusinessAddressCity").text('Please Select')//.button('refresh');
                $("#txtOrgBusinessAddressCountryID").val('');
                $("#btnOrgBusinessAddressCountry").text('Please Select')//.button('refresh');

                $("#txtOrgPurpose").val('').keyup();
                $("#selOrgBusinessScope").val('')//.selectmenu('refresh');
                $("#txtOrgNoOfRetailOutlet").val('0');
                $("#txtOrgNoOfOffice").val('0');
                $("#txtOrgNoOfWarehouse").val('0');
                $("#txtOrgNoOfEmployees").val('0');

                LoadOrgTradeNameList();
                LoadOrgCategoryList();
                LoadOrgIndustryClassList();
                LoadOrgFranchiseeList();

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
                var pDataIndustryClassification = [];
                var pDataFranchisee = [];
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

                intIndex = 0;
                for (var i = 0; i <= intOrgIndustryClassCntr; i++) {

                    if (sessionStorage["OrgIndustryClass-" + i] != undefined) {

                        arOrgIndustryClass = new Array();
                        arOrgIndustryClass = JSON.parse(sessionStorage["OrgIndustryClass-" + i]);

                        pDataIndustryClassification[intIndex] = {
                            ID: arOrgIndustryClass[intOrgIndustryClassRecID],
                            IndustryClassID: arOrgIndustryClass[intOrgIndustryClassID],
                            RecState: arOrgIndustryClass[intOrgIndustryClassState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                intIndex = 0;
                for (var i = 0; i <= intOrgFranchiseeCntr; i++) {

                    if (sessionStorage["OrgFranchisee-" + i] != undefined) {

                        arOrgFranchisee = new Array();
                        arOrgFranchisee = JSON.parse(sessionStorage["OrgFranchisee-" + i]);

                        pDataFranchisee[intIndex] = {
                            ID: arOrgFranchisee[intOrgFranchiseeRecID],
                            FranchiseeID: arOrgFranchisee[intOrgFranchiseeID],
                            IsFranchisorConfirmed: arOrgFranchisee[intOrgFranchiseeIsFranchisorConfirmed],
                            IsFranchiseeConfirmed: arOrgFranchisee[intOrgOrgFranchiseeIsFranchiseeConfirmed],
                            RecState: arOrgFranchisee[intOrgOrgFranchiseeState]
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

                    RegistrationType: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#selOrgRegistrationType").val()) : ''),
                    RegistrationNo: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgRegistrationNo").val()) : ''),
                    RegistrationDate: ($("#rdoOrgOrganization").is(":checked") ? $("#selOrgRegistrationDateMonth").val() + '/' + $("#selOrgRegistrationDateDay").val() + '/' + $("#selOrgRegistrationDateYear").val() : ''),
                    TIN: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgCorporateTIN").val()) : ''),

                    OfficeAddressBuildingNo: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgOfficeAddressBuildingNo").val()) : $.trim($("#txtOrgPersonAddressBuildingNo").val())),
                    OfficeAddressStreet: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgOfficeAddressStreet").val()) : $.trim($("#txtOrgPersonAddressStreet").val())),
                    OfficeAddressCityID: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgOfficeAddressCityID").val()) : $.trim($("#txtOrgPersonAddressCityID").val())),
                    OfficeAddressCountryID: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgOfficeAddressCountryID").val()) : $.trim($("#txtOrgPersonAddressCountryID").val())),

                    BusinessAddressBuildingNo: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgBusinessAddressBuildingNo").val()) : $.trim($("#txtOrgPersonAddressBuildingNo").val())),
                    BusinessAddressStreet: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgBusinessAddressStreet").val()) : $.trim($("#txtOrgPersonAddressStreet").val())),
                    BusinessAddressCityID: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgBusinessAddressCityID").val()) : $.trim($("#txtOrgPersonAddressCityID").val())),
                    BusinessAddressCountryID: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgBusinessAddressCountryID").val()) : $.trim($("#txtOrgPersonAddressCountryID").val())),

                    Purpose: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgPurpose").val()) : ''),
                    BusinessScope: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#selOrgBusinessScope").val()) : ''),

                    NoOfRetailOutlet: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgNoOfRetailOutlet").val()) : '0'),
                    NoOfOffice: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgNoOfOffice").val()) : '0'),
                    NoOfWarehouse: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgNoOfWarehouse").val()) : '0'),
                    NoOfEmployees: ($("#rdoOrgOrganization").is(":checked") ? $.trim($("#txtOrgNoOfEmployees").val()) : '0'),

                    OrgTradeNameList: pDataTradeName,
                    OrgCategoryList: pDataCategory,
                    OrgIndustryClassificationList: pDataIndustryClassification,
                    OrgFranchiseeList: pDataFranchisee,
                    IsVAT: $("#rdoOrgOrganizationVAT").is(":checked")
                };

                // 2019_05_24__14_18_07_4885
                // dont check for duplicate names
                // AjaxSend({
                //    url: ($("#rdoOrgOrganization").is(":checked") ? "OrganizationShort/CheckOrgUniqueOrgName" : "OrganizationShort/CheckPersonUniqueName"),
                //    data: pData,
                //    success: function (vData) {

                //        if (vData.OrganizationData.Rows.length > 0) {
                //          NewMessageBox('Organization with the same corporate name already exist.');
                //        }
                //        else {
                            AjaxSend({
                                url: "Organization/SaveOrganizationInfo",
                                data: pData,
                                success: function (vSaveData) {
                                    if (vSaveData.Error) {
                                        NewMessageBox(vSaveData.Error, 'Failed');
                                        return;
                                    }
                                    if (vSaveData.OrganizationData == "RegistrationDateError") {
                                        NewMessageBox('Please enter valid registration date.');
                                    } else if (vSaveData.OrganizationData == "PersonDOBError") {
                                        NewMessageBox('Please enter valid date of birth.');
                                    } else {

                                        $("#txtOrgID").val(vSaveData.OrganizationData || '');

                                        ////////                                        if (window.pageSourcePage != undefined) {
                                        ////////                                            $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
                                        ////////                                        }
                                        ////////                                        else {
                                        ////////                                          //  ChangePage('/Organization/OrganizationSummary');
                                        ////////                                            window.location = MergeUrlParameters({
                                        ////////                                                OrgID: sessionStorage.OrgID,
                                        ////////                                                IsMobile: CBool(IsMobile) ? 1 : 0
                                        ////////                                            }, '/Organization/OrganizationSummary');
                                        ////////                                        }

                                        //window.location = MergeUrlParameters({
                                        //    OrgID: sessionStorage.OrgID,
                                        //    IsMobile: CBool(IsMobile) ? 1 : 0
                                        //}, '/Organization/OrganizationSummary');
                                        $('#UniversalDiv').html('');
                                        LibraryTable.OrganizationPage();
                                        $('body').scrollTop(0);
                                    }

                                }
                            });



                //        }

                //    }
                //}); //
                // 2019_05_24__14_18_07_4885
                // dont check for duplicate names

            };

            function SaveOrgTradeName() {

                arOrgTradeName = new Array();
                arOrgTradeName[intOrgTradeNameRecID] = $("#txtOrgTradeNameID").val();
                arOrgTradeName[intOrgTradeName] = $("#txtOrgBusinessTradeName").val();
                arOrgTradeName[intOrgTradeNameInitial] = $("#txtOrgBusinessTradeNameInitial").val();
                arOrgTradeName[intOrgTradeNameState] = (IsNewOrgTradeName ? "New" : "Edit");

                sessionStorage["OrgTradeName-" + intOrgTradeNameIndex] = JSON.stringify(arOrgTradeName);

                //$.mobile.changePage('#pageOrganizationInfo');
                $('#pageOrgTradeName').hide();
                $('#pageOrganizationInfo').show();
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
                $('#pageOrgTradeName').hide();
                $('#pageOrganizationInfo').show();
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

                            pItem = $('<li class="list-group-item" id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgTradeName)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgTradeNameInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<div>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgTradeName[intOrgTradeName])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV//.listview('refresh');
            };

            function LoadOrgCategoryList() {
                var pLV = $('#lstOrgCategory');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgCategoryCntr; i++) {

                    if (sessionStorage["OrgCategory-" + i] != undefined) {

                        arOrgCategory = new Array();
                        arOrgCategory = JSON.parse(sessionStorage["OrgCategory-" + i]);
                        arOrgCategory.Index = i;
                        if (arOrgCategory[intOrgCategoryState] != "Remove") {

                            pItem = $('<li class="list-group-item" id="lst' + i + '">')
                                .append($('<table style="width:100%;">')
                                    .append($('<tr>')
                                        .append($('<td style="word-break:break-all;">').text(arOrgCategory[intOrgCategory])
                                        )
                                        .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                            .data('Data', arOrgCategory)
                                            .click(function (e) {
                                                e.preventDefault();

                                                sessionStorage["OrgCategoryToRemoveIndex"] = $(this).attr('ID');
                                                sessionStorage["OrgCategoryToRemove"] = JSON.stringify($(this).data('Data'));

                                                var arOrgCategoryToRemove = new Array();
                                                arOrgCategoryToRemove = JSON.parse(sessionStorage["OrgCategoryToRemove"]);
                                                intOrgCategoryIndex = sessionStorage["OrgCategoryToRemoveIndex"];

                                                arOrgCategoryToRemove[intOrgCategoryState] = "Remove";
                                                sessionStorage["OrgCategory-" + $(this).data('Data').Index] = JSON.stringify(arOrgCategoryToRemove);

                                                LoadOrgCategoryList();

                                                sessionStorage["OrgCategoryToRemoveIndex"] = null;
                                                sessionStorage["OrgCategoryToRemove"] = null;
                                            })
                                        ))
                                    )
                                )

                            pLV.append(pItem);

                        }

                    }
                }

                pLV//.listview('refresh');
            };

            function LoadOrgIndustryClassList() {
                var pLV = $('#lstOrgIndustryClassification');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgIndustryClassCntr; i++) {

                    if (sessionStorage["OrgIndustryClass-" + i] != undefined) {

                        arOrgIndustryClass = new Array();
                        arOrgIndustryClass = JSON.parse(sessionStorage["OrgIndustryClass-" + i]);
                        arOrgIndustryClass.Index = i;
                        if (arOrgIndustryClass[intOrgIndustryClassState] != "Remove") {

                            pItem = $('<li class="list-group-item" id="lst' + i + '">')
                                .append($('<table style="width:100%;">')
                                    .append($('<tr>')
                                        .append($('<td style="word-break:break-all;">').text(arOrgIndustryClass[intOrgIndustryClass]))
                                        .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                            .data('Data', arOrgIndustryClass)
                                            .click(function (e) {
                                                e.preventDefault();

                                                sessionStorage["OrgIndustryClassToRemoveIndex"] = $(this).attr('ID');
                                                sessionStorage["OrgIndustryClassToRemove"] = JSON.stringify($(this).data('Data'));

                                                var arOrgIndustryClassToRemove = new Array();
                                                arOrgIndustryClassToRemove = JSON.parse(sessionStorage["OrgIndustryClassToRemove"]);
                                                intOrgIndustryClassIndex = sessionStorage["OrgIndustryClassToRemoveIndex"];

                                                arOrgIndustryClassToRemove[intOrgIndustryClassState] = "Remove";
                                                sessionStorage["OrgIndustryClass-" + $(this).data('Data').Index] = JSON.stringify(arOrgIndustryClassToRemove);

                                                LoadOrgIndustryClassList();

                                                sessionStorage["OrgIndustryClassToRemoveIndex"] = null;
                                                sessionStorage["OrgIndustryClassToRemove"] = null;
                                            })
                                        ))
                                    )
                                )

                            pLV.append(pItem);
                        }
                    }
                }

                pLV//.listview('refresh');
            };

            function LoadOrgFranchiseeList() {
                var pLV = $('#lstOrgFranchisee');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgFranchiseeCntr; i++) {

                    if (sessionStorage["OrgFranchisee-" + i] != undefined) {

                        arOrgFranchisee = new Array();
                        arOrgFranchisee = JSON.parse(sessionStorage["OrgFranchisee-" + i]);

                        if (arOrgFranchisee[intOrgOrgFranchiseeState] != "Remove") {

                            pItem = $('<li class="list-group-item" id="lst' + i + '">')
                                .append($('<table style="width:100%;">')
                                    .append($('<tr>')
                                        .append($('<td style="word-break:break-all;">')
                                            .attr('ID', i)
                                            .data('Data', arOrgFranchisee)
                                            .click(function (e) {
                                                e.preventDefault();

                                                ShowOrgFranchiseeConfirmInfo($(this).data('Data'), $(this).attr('ID'));
                                            })
                                            .append($('<div>')
                                                .attr("style", "white-space:normal;")
                                                .text(arOrgFranchisee[intOrgFranchiseeName])
                                            )
                                            .append($('<div>')
                                                .attr("style", "white-space:normal;")
                                                .text('Corporate Name : ' + arOrgFranchisee[intOrgFranchiseeOrgName])
                                            )
                                            .append($('<div>')
                                                .attr("style", "white-space:normal;")
                                                .text('Confirmed by Franchisor: ' + (arOrgFranchisee[intOrgFranchiseeIsFranchisorConfirmed] == 1 ? 'Yes' : 'No'))
                                            )
                                            .append($('<div>')
                                                .attr("style", "white-space:normal;")
                                                .text('Confirmed by Franchisee: ' + (arOrgFranchisee[intOrgOrgFranchiseeIsFranchiseeConfirmed] == 1 ? 'Yes' : 'No'))
                                            )
                                        )
                                        .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                            .data('Data', arOrgIndustryClass)
                                            .click(function (e) {
                                                e.preventDefault();

                                                sessionStorage["OrgFranchiseeToRemoveIndex"] = $(this).attr('ID');
                                                sessionStorage["OrgFranchiseeToRemove"] = JSON.stringify($(this).data('Data'));

                                                NewMessageBox("Remove this record?",
                                                    'Remove',
                                                    [
                                                        { Text: 'Yes',
                                                            Click: function () {

                                                                var arOrgFranchiseeToRemove = new Array();
                                                                arOrgFranchiseeToRemove = JSON.parse(sessionStorage["OrgFranchiseeToRemove"]);
                                                                intOrgIndustryClassIndex = sessionStorage["OrgFranchiseeToRemoveIndex"];

                                                                arOrgFranchiseeToRemove[intOrgOrgFranchiseeState] = "Remove";
                                                                sessionStorage["OrgFranchisee-" + intOrgFranchiseeIndex] = JSON.stringify(arOrgFranchiseeToRemove);

                                                                LoadOrgFranchiseeList();

                                                                sessionStorage["OrgFranchiseeToRemoveIndex"] = null;
                                                                sessionStorage["OrgFranchiseeToRemove"] = null;
                                                            }
                                                        },
                                                        { Text: 'No'
                                                        }
                                                    ]
                                                );
                                            })
                                        ))
                                    )
                                )

                            pLV.append(pItem);

                        }

                    }
                }

                pLV//.listview('refresh');
            };


        });
    });