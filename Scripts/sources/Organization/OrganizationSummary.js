require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var isIE = false || !!document.documentMode;
            var isOrgInfoPageLoaded = "";
            var isOrgBrandPageLoaded = "";
            var isOrgFinRepPageLoaded = "";
            var isOrgContactPageLoaded = "";
            var isOrgRepPageLoaded = "";
            var isOrgOwnerPageLoaded = "";
            var isOrgAffPageLoad = "";

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                AjaxSend({
                    url: 'Organization/IsNewEntry',
                    success: function (vData) {
                        ShowSumOrganizationInfo(vData.OrganizationID)

                    }
                });
            };
            if (!mLoaded) PageShow();

            window.OrgSummary = {
                ShowSumOrgInfo: function () {
                    AjaxSend({
                        url: 'Organization/IsNewEntry',
                        success: function (vData) {
                            ShowSumOrganizationInfo(sessionStorage.OrgID);
                        }
                    });
                }
            }
            //Navigation Menu
            $('#btnHome').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
            });

            $('#btnSystemAdmin').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuSettings"), "_self");
            });

            $('#btnSystemSettings').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopSystemSettings"), "_self");
            });

            //End Navigation Menu

            $('body').on('click', '#btnEditSumOrgInfo', function (e) {
                e.preventDefault();

                window.IsPageLoaded = false;
                //ChangePage('/Organization/OrganizationInfo');

                AddBreadCrumb({
                    text: 'Company Info', Click: function () {

                    }
                }, '.ItemsBreadCrumbs')

                if (isOrgInfoPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationInfo?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrgInfoPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isOrgInfoPageLoaded);//.trigger('create');
                    OrgInfo.ShowOrgInfo();
                }
            })
            .on('click', '#btnEditSumOrgBrands', function (e) {
                e.preventDefault();

                window.IsPageLoaded = false;
                //ChangePage('/Organization/OrganizationBrands');
                //                window.location = MergeUrlParameters({
                //                    OrgID: sessionStorage.OrgID,
                //                    IsMobile: CBool(IsMobile) ? 1 : 0
                //                }, '/Organization/OrganizationBrands');

                if (isOrgBrandPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationBrands?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrgBrandPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isOrgBrandPageLoaded);//.trigger('create');
                    OrgBrand.GetOrgBrand();
                }
            })
            .on('click', '#btnEditSumOrgFinancialInfo', function (e) {
                e.preventDefault();

                window.IsPageLoaded = false;
                //ChangePage('/Organization/OrganizationBrands');
                //                window.location = MergeUrlParameters({
                //                    OrgID: sessionStorage.OrgID,
                //                    IsMobile: CBool(IsMobile) ? 1 : 0
                //                }, '/Organization/OrganizationFinancialReport');

                if (isOrgFinRepPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationFinancialReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrgFinRepPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isOrgFinRepPageLoaded);//.trigger('create');
                    OrgFinRep.ShowOrgFinRep()
                }
            })
            .on('click', '#btnEditSumOrgContact', function (e) {
                e.preventDefault();

                window.IsPageLoaded = false;
                //ChangePage('/Organization/OrganizationBrands');
                //                window.location = MergeUrlParameters({
                //                    OrgID: sessionStorage.OrgID,
                //                    IsMobile: CBool(IsMobile) ? 1 : 0
                //                }, '/Organization/OrganizationContact');

                if (isOrgContactPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationContact?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrgContactPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isOrgContactPageLoaded);//.trigger('create');
                    OrgContact.GetOrgContact();
                }
            })
            .on('click', '#btnEditSumOrgRepresentative', function (e) {
                e.preventDefault();

                window.IsPageLoaded = false;
                //ChangePage('/Organization/OrganizationBrands');
                //                window.location = MergeUrlParameters({
                //                    OrgID: sessionStorage.OrgID,
                //                    IsMobile: CBool(IsMobile) ? 1 : 0
                //                }, '/Organization/OrganizationRepresentative');

                if (isOrgRepPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationRepresentative?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrgRepPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isOrgRepPageLoaded);//.trigger('create');
                    OrgRep.ShowInfo();
                }
            })
            .on('click', '#btnOrgSummaryDone', function (e) {
                e.preventDefault();
                //window.open(GetPathFormat("/People/PeopleMenu"), "_self");
                window.location.hash = 'SystemAdministration';
                $(window).scrollTop(0);

                //                if (window.pageSecuritySourcePage != undefined) {
                //                    $.mobile.changePage(window.pageSecuritySourcePage);
                //                } else {
                //                    $.mobile.changePage("#" + $(window.pageOrig).attr('id'));
                //                }
            })
            .on('click', '#btnEditSumOrgOwner', function (e) {
                e.preventDefault();

                //ChangePage('/Organization/OrganizationOwner');
                //                window.location = MergeUrlParameters({
                //                    OrgID: sessionStorage.OrgID,
                //                    IsMobile: CBool(IsMobile) ? 1 : 0
                //                }, '/Organization/OrganizationOwner');
                if (isOrgOwnerPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationOwner?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrgOwnerPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isOrgOwnerPageLoaded);//.trigger('create');
                    OrgOwner.GetOrgOwnerList();
                }
            })
            .on('click', '#btnEditSumOrgAffiliation', function (e) {
                e.preventDefault();

                window.IsPageLoaded = false;
                //ChangePage('/Organization/OrganizationAffiliation');

                if (isOrgAffPageLoad == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationAffiliation?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrgAffPageLoad = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isOrgAffPageLoad);//.trigger('create');
                    OrgAff.ShowInfo();
                }
            })

            $('#btnEditSumOrgFOWST').click(function (e) {
                e.preventDefault();

                //                ChangePage('/Organization/OrganizationAddress');
                window.location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/Organization/OrganizationAddress');
            });

            $('#btnEditSumOrgEmployee').click(function (e) {
                e.preventDefault();

                //ChangePage('/Organization/OrganizationEmployee');
                window.location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/Organization/OrganizationEmployee');
            });

            $('#btnEditSumOrgCustomer').click(function (e) {
                e.preventDefault();

                //ChangePage('/Organization/OrganizationCustomer');
                window.location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/Organization/OrganizationCustomer');
            });

            $('#btnEditSumOrgSupplier').click(function (e) {
                e.preventDefault();

                //ChangePage('/Organization/OrganizationSupplier');
                window.location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/Organization/OrganizationSupplier');
            });

            $('#btnOrgSummaryGoToSetupChecklist').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/SetupChecklist"), "_self");

                //                if (window.pageSecuritySourcePage != undefined) {
                //                    $.mobile.changePage(window.pageSecuritySourcePage);
                //                } else {
                //                    $.mobile.changePage("#" + $(window.pageOrig).attr('id'));
                //                }
            });

            function ShowSumOrganizationInfo(strID) {
                var pData = {};
                pData.OrganizationID = strID;

                ClearSumOrganizationFields();

                AjaxSend({
                    url: "Organization/getOrganizationInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData.Rows.length > 0) {

                            if (vData.OrganizationData.Rows[0].IsOrganization == 1) {
                                $("#divSumOrgOrganizationInfo").show();
                                $("#divSumOrgPersonInfo").hide();
                            } else {
                                $("#divSumOrgOrganizationInfo").hide();
                                $("#divSumOrgPersonInfo").show();

                                $("#lstFinancialReportInfoHeader").hide();
                                $("#lstOrganizationBrandsHeader").hide();
                                $("#lstFinancialReportInfoDetails").hide();
                                $("#lstContactInfoHeader").hide();
                                $("#lstContactInfoDetails").hide();
                                $("#lstOwnerInfoHeader").hide();
                                $("#lstOwnerInfoDetails").hide();
                                $("#lstHoldingCompanyInfoHeader").hide();
                                $("#lstHoldingCompanyInfoDetails").hide();
                                $("#lstEmployeeInfoHeader").hide();
                                $("#lstEmployeeInfoDetails").hide();
                                $("#lstCustomerInfoHeader").hide();
                                $("#lstCustomerInfoDetails").hide();
                                $("#lstSupplierInfoHeader").hide();
                                $("#lstSupplierInfoDetails").hide();
                            }

                            $("#txtSumID").val(vData.OrganizationData.Rows[0].ID || '');
                            $("#txtSumCorporateName").val(vData.OrganizationData.Rows[0].IsOrganization == 1 ? (vData.OrganizationData.Rows[0].CompanyName || '') : (vData.OrganizationData.Rows[0].PersonName || ''));

                            $("#txtSumOwnershipType").val(vData.OrganizationData.Rows[0].OwnershipType || '');

                            $('#chkSumIsFranchisor').attr("checked", (vData.OrganizationData.Rows[0].IsFranchisor == 0 ? false : true));//.checkboxradio("refresh");
                            //                            ShowHideSumFranchisee();

                            $("#txtSumRegistrationType").val(vData.OrganizationData.Rows[0].RegistrationType || '');
                            $("#txtSumRegistrationNo").val(vData.OrganizationData.Rows[0].RegistrationNo || '');
                            $("#txtSumRegistrationDate").val(vData.OrganizationData.Rows[0].RegistrationDateMonth + '/' + vData.OrganizationData.Rows[0].RegistrationDateDay + '/' + vData.OrganizationData.Rows[0].RegistrationDateYear);
                            SetSumRegistrationInfoLabels();
                            $("#txtSumTIN").val(vData.OrganizationData.Rows[0].TIN || '');

                            $("#txtSumOrgPersonBuildingNo").val(vData.OrganizationData.Rows[0].BusinessAddressBuildingNo || '');
                            $("#txtSumOrgPersonAddressStreet").val(vData.OrganizationData.Rows[0].BusinessAddressStreet || '');
                            $("#txtSumOrgPersonAddressCity").val(vData.OrganizationData.Rows[0].BusinessAddressCity || '');
                            $("#txtSumOrgPersonAddressCountry").val(vData.OrganizationData.Rows[0].BusinessAddressCountry || '');

                            $("#txtSumOfficeAddressBuildingNo").val(vData.OrganizationData.Rows[0].OfficeAddressBuildingNo || '');
                            $("#txtSumOfficeAddressStreet").val(vData.OrganizationData.Rows[0].OfficeAddressStreet || '');
                            $("#txtSumOfficeAddressCity").val(vData.OrganizationData.Rows[0].OfficeAddressCity || '');
                            $("#txtSumOfficeAddressCountry").val(vData.OrganizationData.Rows[0].OfficeAddressCountry || '');

                            $("#txtSumBusinessAddressBuildingNo").val(vData.OrganizationData.Rows[0].BusinessAddressBuildingNo || '');
                            $("#txtSumBusinessAddressStreet").val(vData.OrganizationData.Rows[0].BusinessAddressStreet || '');
                            $("#txtSumBusinessAddressCity").val(vData.OrganizationData.Rows[0].BusinessAddressCity || '');
                            $("#txtSumBusinessAddressCountry").val(vData.OrganizationData.Rows[0].BusinessAddressCountry || '');

                            $("#txtSumPurpose").val(vData.OrganizationData.Rows[0].Purpose || '').keyup();
                            $("#txtSumBusinessScope").val(vData.OrganizationData.Rows[0].BusinessScope || '');
                            $("#txtSumNoOfRetailOutlet").val(vData.OrganizationData.Rows[0].NoOfRetailOutlet || '');
                            $("#txtSumNoOfOffice").val(vData.OrganizationData.Rows[0].NoOfOffice || '');
                            $("#txtSumNoOfWarehouse").val(vData.OrganizationData.Rows[0].NoOfWarehouse || '');
                            $("#txtSumNoOfEmployees").val(vData.OrganizationData.Rows[0].NoOfEmployees || '');

                            if (CStr2(vData.OrganizationData.Rows[0].LogoFileName) != '') {
                                $('#imgSumOrgLogo').attr('src', '/Content/OrganizationLogo/' + vData.OrganizationData.Rows[0].LogoFileName);
                            } else {
                                $('#imgSumOrgLogo').removeAttr('src');
                            }

                            AjaxSend({
                                url: "Organization/getOrgTradeNameList",
                                data: pData,
                                success: function (vDataTradeName) {

                                    LoadSumOrgTradeNameList(vDataTradeName.OrganizationData.Rows);

                                }
                            });

                            AjaxSend({
                                url: "Organization/getOrgCategoryList",
                                data: pData,
                                success: function (vDataCategory) {

                                    LoadSumOrgCategoryList(vDataCategory.OrganizationData.Rows);

                                }
                            });

                            AjaxSend({
                                url: "Organization/getOrganizationIndustryClassList",
                                data: pData,
                                success: function (vDataIndustryClass) {

                                    LoadSumOrgIndustryClassList(vDataIndustryClass.OrganizationData.Rows);

                                }
                            });

                            //                            AjaxSend({
                            //                                url: "Organization/getOrgFranchiseeList",
                            //                                data: pData,
                            //                                success: function (vDataFranchisee) {

                            //                                    LoadSumOrgFranchiseeList(vDataFranchisee.OrganizationData.Rows);

                            //                                }
                            //                            });

                            getSumOrganizationBrandList();

                            ShowSumFinancialReportDetails();

                            getSumContactTelList();
                            getSumAddressList();
                            getSumRepresentativeList();
                            getSumOwnerList();
                            ShowSumOrgAffiliationInfo();
                            getSumEmployeeList();
                            getSumSupplierList();
                            getSumCustomerList();


                        }
                    }
                });

            };

            function LoadSumOrgTradeNameList(vRows) {
                var pLV = $('#lstSumTradeName');
                var pItem;


                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal;")
                            .text(vRows[p].BusinessTradeName)
                        );

                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function LoadSumOrgCategoryList(vRows) {
                var pLV = $('#lstSumCategory');
                var pItem;


                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal;")
                            .text(vRows[p].CategoryType)
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal;")
                            .text('Category : ' + vRows[p].Category)
                        );

                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function LoadSumOrgIndustryClassList(vRows) {
                var pLV = $('#lstSumIndustryClassification');
                var pItem;


                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal;")
                            .text(vRows[p].IndustryClass)
                        );

                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function ShowHideSumFranchisee() {
                if ($("#chkSumIsFranchisor").is(":checked")) {
                    $("#divSumFranchiseeContainer").show();
                } else {
                    $("#divSumFranchiseeContainer").hide();
                }
            }

            function LoadSumOrgFranchiseeList(vRows) {
                var pLV = $('#lstSumOrgFranchisee');
                var pItem;


                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal;")
                            .text(vRows[p].BranchName)
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal;")
                            .text('Corporate Name : ' + vRows[p].FranchiseeOrgName)
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal;")
                            .text('Confirmed By Franchisor : ' + (vRows[p].IsFranchisorConfirmed == 1 ? 'Yes' : 'No'))
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal;")
                            .text('Confirmed By Franchisee : ' + (vRows[p].IsFranchiseeConfirmed == 1 ? 'Yes' : 'No'))
                        );

                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function SetSumRegistrationInfoLabels(strRegistrationType) {

                if ($("#txtSumRegistrationType").val() == "SEC - Philippines (Securities and Exchange Commission)") {
                    $("#lblSumRegistrationNo").text("SEC Registration No.: ");
                    $("#lblSumRegistrationDate").text("SEC Registration Date: ");
                }
                else if ($("#txtSumRegistrationType").val() == "DTI - Philippines (Department of Trade and Industry)") {
                    $("#lblSumRegistrationNo").text("DTI Registration No.: ");
                    $("#lblSumRegistrationDate").text("DTI Registration Date: ");
                }
                else {
                    $("#lblSumRegistrationNo").text("Registration No.: ");
                    $("#lblSumRegistrationDate").text("Registration Date: ");
                }

            };

            function ClearSumOrganizationFields() {

                $("#txtSumID").val('');

                $("#txtSumCorporateName").val('');
                $("#txtSumBusinessTradeName").val('');
                $("#txtSumOwnershipType").val('');

                $('#chkSumIsFranchisor').attr("checked", false);//.checkboxradio("refresh");
                //                ShowHideSumFranchisee();

                $("#txtSumRegistrationType").val('');
                $("#txtSumRegistrationNo").val('');
                $("#txtSumRegistrationDate").val('');
                $("#txtSumTIN").val('');

                $("#txtSumOfficeAddressBuildingNo").val('');
                $("#txtSumOfficeAddressStreet").val('');
                $("#txtSumOfficeAddressCity").val('');
                $("#txtSumOfficeAddressCountry").val('');

                $("#txtSumBusinessAddressBuildingNo").val('');
                $("#txtSumBusinessAddressStreet").val('');
                $("#txtSumBusinessAddressCity").val('');
                $("#txtSumBusinessAddressCountry").val('');

                $("#txtSumPurpose").val('').keyup();
                $("#txtSumBusinessScope").val('');
                $("#txtSumNoOfRetailOutlet").val('');
                $("#txtSumNoOfOffice").val('');
                $("#txtSumNoOfWarehouse").val('');
                $("#txtSumNoOfEmployees").val('');

            };

            function ShowSumFinancialReportDetails() {

                ClearSumFinancialReportFields();

                AjaxSend({
                    url: "Organization/getOrgFinancialReportInfo",
                    success: function (vData) {

                        if (vData.OrganizationData.Rows.length > 0) {

                            $("#txtSumFinancialFiscalYearEnd").val($.format.date(new Date(vData.OrganizationData.Rows[0].FiscalMonthEnd + "/" + vData.OrganizationData.Rows[0].FiscalDayEnd + "/2012"), 'MMMM dd') || '');
                            $("#txtSumExternalAuditor").val(vData.OrganizationData.Rows[0].NameOfExternalAuditor || '');
                            $("#txtSumSingningPartnerExternalAuditor").val(vData.OrganizationData.Rows[0].SingningPartnerOfExternalAuditor || '');
                            $("#txtSumSECAccreditationNoOfAuditor").val(vData.OrganizationData.Rows[0].SECAccreditationNoOfExternalAuditor || '');
                            $("#txtSumVATType").val(vData.OrganizationData.Rows[0].VATType || '');
                            $("#txtSumBIRTaxGroup").val(vData.OrganizationData.Rows[0].BIRTaxGroup || '');

                        }
                    }
                });

            };

            function ClearSumFinancialReportFields() {
                $("#txtSumFinancialFiscalYearEnd").val('');
                $("#txtSumExternalAuditor").val('');
                $("#txtSumSingningPartnerExternalAuditor").val('');
                $("#txtSumSECAccreditationNoOfAuditor").val('');
                $("#txtSumVATType").val('');
                $("#txtSumBIRTaxGroup").val('');
            };

            function getSumOrganizationBrandList() {
                AjaxSend({
                    url: 'Organization/getOrganizationBrandList',
                    success: function (vData) {
                        LoadSumOrganizationBrandList(vData.OrgBrandData.Rows);
                    }
                });
            }

            function LoadSumOrganizationBrandList(vRows) {
                var pLV = $('#lstSumOrganizationBrand');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].BrandName)
                        );
                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function getSumContactTelList() {
                AjaxSend({
                    url: 'Organization/getOrgContactTelList',
                    success: function (vData) {
                        LoadSumContactTelList(vData.OrganizationData.Rows);
                        getSumContactFaxList();
                    }
                });
            }

            function LoadSumContactTelList(vRows) {
                var pLV = $('#lstSumOrganizationContact');
                var pItem;

                pLV.html('');
                pItem = $('<li class="list-group-item" style="font-weight:bold;">')
                    .attr('data-role', "list-divider")
                    .text('Telephone');
                pLV.append(pItem);

                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text("+" + vRows[p].CountryCode + "." + vRows[p].AreaCode + '.' + vRows[p].ContactNo + ((vRows[p].ExtensionNo || '') == '' ? '' : ", Ext. " + vRows[p].ExtensionNo))
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].Area + ", " + vRows[p].CountryName)
                        );
                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function getSumContactFaxList() {
                AjaxSend({
                    url: 'Organization/getOrgContactFaxList',
                    success: function (vData) {
                        LoadSumContactFaxList(vData.OrganizationData.Rows);
                        getSumContactMobileList();
                    }
                });
            }

            function LoadSumContactFaxList(vRows) {
                var pLV = $('#lstSumOrganizationContact');
                var pItem;

                pItem = $('<li class="list-group-item" style="font-weight:bold;">')
                    .attr('data-role', "list-divider")
                    .text('Fax');
                pLV.append(pItem);

                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text("+" + vRows[p].CountryCode + "." + vRows[p].AreaCode + '.' + vRows[p].ContactNo + ((vRows[p].ExtensionNo || '') == '' ? '' : ", Ext. " + vRows[p].ExtensionNo))
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].Area + ", " + vRows[p].CountryName)
                        );
                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function getSumContactMobileList() {
                AjaxSend({
                    url: 'Organization/getOrgContactMobileList',
                    success: function (vData) {
                        LoadSumContactMobileList(vData.OrganizationData.Rows);
                        getSumContactEmailList();
                    }
                });
            }

            function LoadSumContactMobileList(vRows) {
                var pLV = $('#lstSumOrganizationContact');
                var pItem;

                pItem = $('<li class="list-group-item" style="font-weight:bold;">')
                    .attr('data-role', "list-divider")
                    .text('Mobile');
                pLV.append(pItem);

                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text("+" + vRows[p].CountryCode + "." + vRows[p].Prefix + "." + vRows[p].ContactNo)
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].TelCo + " (" + vRows[p].CountryName + ")")
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].ContactType)
                        );
                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function getSumContactEmailList() {
                AjaxSend({
                    url: 'Organization/getOrgContactEmailList',
                    success: function (vData) {
                        LoadSumContactEmailList(vData.OrganizationData.Rows);
                        getSumContactWebsiteList();
                    }
                });
            }

            function LoadSumContactEmailList(vRows) {
                var pLV = $('#lstSumOrganizationContact');
                var pItem;

                pItem = $('<li class="list-group-item" style="font-weight:bold;">')
                    .attr('data-role', "list-divider")
                    .text('Email');
                pLV.append(pItem);

                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].EmailAddress)
                        );
                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function getSumContactWebsiteList() {
                AjaxSend({
                    url: 'Organization/getOrgContactWebsiteList',
                    success: function (vData) {
                        LoadSumContactWebsiteList(vData.OrganizationData.Rows);
                    }
                });
            }

            function LoadSumContactWebsiteList(vRows) {
                var pLV = $('#lstSumOrganizationContact');
                var pItem;

                pItem = $('<li class="list-group-item" style="font-weight:bold;">')
                    .attr('data-role', "list-divider")
                    .text('Website');
                pLV.append(pItem);

                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].Website)
                        );
                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function getSumAddressList() {
                AjaxSend({
                    url: 'Organization/getOrgFOWSTList',
                    success: function (vData) {
                        LoadSumAddressList(vData.OrganizationFOWSTData.Rows);
                    }
                });
            }

            function LoadSumAddressList(vRows) {
                var pLV = $('#lstSumFOWST');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].Name)
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].CompleteFowstAddress)
                        );
                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function getSumRepresentativeList() {
                AjaxSend({
                    url: 'Organization/getOrgRepList',
                    success: function (vData) {
                        LoadSumRepList(vData.OrganizationRepData.Rows);
                    }
                });
            }

            function LoadSumRepList(vRows) {
                var pLV = $('#lstSumRepresentative');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].RepresentativeName)
                        );
                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function getSumOwnerList() {
                AjaxSend({
                    url: 'Organization/getOrgOwnerList',
                    success: function (vData) {
                        LoadSumOwnerList(vData.OrgOwnerList.Rows);
                    }
                });
            }

            function LoadSumOwnerList(vRows) {
                var pLV = $('#lstSumOrganizationOwner');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {

                    var OwnershipType = (vRows[p].IsIncorporator == "1" ? "Incorporator" : "");
                    OwnershipType = OwnershipType + (vRows[p].IsDirector == "1" ? (OwnershipType == "" ? "Director" : ", Director") : "");
                    OwnershipType = OwnershipType + (vRows[p].IsStockHolder == "1" ? (OwnershipType == "" ? "Stock Holder" : ", Stock Holder") : "");
                    OwnershipType = OwnershipType + (vRows[p].IsOfficer == "1" ? (OwnershipType == "" ? "Officer" : ", Officer") : "");
                    OwnershipType = OwnershipType + (vRows[p].IsExecutiveCommittee == "1" ? (OwnershipType == "" ? "Executive Committee" : ", Executive Committee") : "");
                    OwnershipType = OwnershipType + (vRows[p].IsOwner == "1" ? (OwnershipType == "" ? "Owner or Proprietor" : ", Owner or Proprietor") : "");

                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].PeopleName)
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].JobOccupation)
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].BusinessUnit)
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(OwnershipType)
                        );
                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function ShowSumOrgAffiliationInfo() {

                ClearSumOrgAffiliationFields();

                AjaxSend({
                    url: "Organization/getOrgHoldingCompanyInfo",
                    success: function (vData) {

                        if (vData.OrgAffiliationData.Rows.length > 0) {

                            $("#selSumOrgHasParentCompany").val(vData.OrgAffiliationData.Rows[0].IsHasHoldingCompany || '0');
                            $("#selSumOrgHasParentCompany");//.slider("refresh");

                            $("#txtSumOrgAffHoldingCompany").val(vData.OrgAffiliationData.Rows[0].HoldingCompanyName || '');

                        }

                        getSumOrgAffiliatesList();

                        ShowHideSumOrgAffParentCompanyDetails();
                    }
                });

            };

            function getSumOrgAffiliatesList() {
                AjaxSend({
                    url: "Organization/getOrgAffliatesList",
                    success: function (vData) {

                        LoadSumOrgAffiliatesList(vData.OrgAffiliationData.Rows);

                    }
                });
            }

            function LoadSumOrgAffiliatesList(vRows) {

                var pLV = $('#lstSumOrgAffiliates');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].AffiliateCompanyName)
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text("Remarks : " + vRows[p].Remarks)
                        );
                    pLV.append(pItem);
                };

                //pLV.listview('refresh');

            };

            function ShowHideSumOrgAffParentCompanyDetails() {

                if ($.trim($("#selSumOrgHasParentCompany").val()) == "1") {
                    $("#divSumOrgAffParentCompanyDetails").show();
                }
                else {
                    $("#divSumOrgAffParentCompanyDetails").hide();
                }
            }

            function ClearSumOrgAffiliationFields() {

                $("#selSumOrgHasParentCompany").val('0');//.slider("refresh");
                $("#txtSumOrgAffHoldingCompany").val('');

                ShowHideSumOrgAffParentCompanyDetails();

            }

            function getSumEmployeeList() {
                AjaxSend({
                    url: 'Organization/getOrganizationEmpList',
                    success: function (vData) {
                        LoadSumOrgEmpList(vData.OrgEmployeeData.Rows);
                    }
                });
            }

            function LoadSumOrgEmpList(vRows) {
                var pLV = $('#lstSumEmployee');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].PeopleName)
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text("Date Hired : " + $.format.date(new Date(vRows[p].DateHired), 'MMM dd, yyyy'))
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text("Status: " + (vRows[p].DeActivated != '0' ? 'Deactivated ( ' + $.format.date(new Date(vRows[p].DeactivationDate), 'MMM dd, yyyy') + ' )' : 'Active'))
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text("Banned: " + (vRows[p].IsBan != '0' ? 'Yes' : 'No'))
                        );

                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function getSumCustomerList() {
                AjaxSend({
                    url: 'Organization/getOrganizationCustList',
                    success: function (vData) {
                        LoadSumOrgCustList(vData.OrgCustomerData.Rows);
                    }
                });
            }

            function LoadSumOrgCustList(vRows) {
                var pLV = $('#lstSumCustomer');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].CustomerName)
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text("Activation Date: " + $.format.date(new Date(vRows[p].ActivationDate), 'MMM dd, yyyy'))
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text("Status: " + (vRows[p].DeActivated != '0' ? 'Deactivated ( ' + $.format.date(new Date(vRows[p].DeactivationDate), 'MMM dd, yyyy') + ' )' : 'Active'))
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text("Banned: " + (vRows[p].IsBan != '0' ? 'Yes' : 'No'))
                        );

                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function getSumSupplierList() {
                AjaxSend({
                    url: 'Organization/getOrganizationSupplierList',
                    success: function (vData) {
                        LoadSumOrgSupplierList(vData.OrgSupplierData.Rows);
                    }
                });
            }

            function LoadSumOrgSupplierList(vRows) {
                var pLV = $('#lstSumSupplier');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].SupplierName)
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text("Activation Date: " + $.format.date(new Date(vRows[p].ActivationDate), 'MMM dd, yyyy'))
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text("Status: " + (vRows[p].DeActivated != '0' ? 'Deactivated ( ' + $.format.date(new Date(vRows[p].DeactivationDate), 'MMM dd, yyyy') + ' )' : 'Active'))
                        )
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text("Banned: " + (vRows[p].IsBan != '0' ? 'Yes' : 'No'))
                        );

                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };












        });
    });


