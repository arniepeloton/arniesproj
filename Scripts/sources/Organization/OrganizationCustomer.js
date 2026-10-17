require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var isIE = false || !!document.documentMode;

            setMonthCollection("selOrgCustActivationDateMonth");
            setYearCollection("selOrgCustActivationDateYear");

            setMonthCollection("selOrgCustDateInActiveMonth");
            setYearCollection("selOrgCustDateInActiveYear");

            //$.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                //getOrgCustomerList();
                clearOrgInfo();
                getCityName();
                getCountryName();
            };
            if (!mLoaded) PageShow();

            window.OrgCustomer = {
                ShowCustomerReqData: function () {
                    clearOrgInfo();
                    getCityName();
                    getCountryName();
                }
            }

            $('body').on('click', '#btnCustomerNameSave', function () {
                if ($('#selCustomerType').val() == 'Organization') {
                    var str = $.trim($('#txtCompanyName').val())
                    str = str.toLowerCase().replace(/\b[a-z]/g, function (letter) {
                        return letter.toUpperCase();
                    });

                    if (str == "" || str.length <= 1) {
                        if (OfflineMode) {
                            Android.Toast("Company Name is required");
                            return;
                        }

                    }
                    if ($.trim($('#txtCustomerTIN').val()) == "" || $.trim($('#txtCustomerTIN').val()).length <= 1) {
                        if (OfflineMode) {
                            Android.Toast("Company TIN is required");
                            return;
                        }
                    }

                    //                        $('#btnCustomerNameSave').attr('disabled', true);

                    var pDataTradeName = [];
                    var pDataCategory = [];
                    var pDataOrg = {
                        IsOrganization: 1,
                        OrganizationID: '',
                        PersonTitleID: '',
                        PersonDOB: '',
                        CompanyName: str,
                        TIN: $.trim($('#txtCustomerTIN').val()),
                        OrgTradeNameList: pDataTradeName,
                        OrgCategoryList: pDataCategory,
                        IsAddAsCustomer: 1,
                        CompanyNameInitial: '',
                        IsFranchisor: 0,
                        OfficeAddressBuildingNo: $.trim($('#txtCustomerOrgHouseNo').val()),
                        OfficeAddressStreet: $.trim($('#txtCustomerOrgAddress').val()), //temporarily store the free type address here
                        OfficeAddressCityID: $('#selOrgCity').val(),
                        OfficeAddressCountryID: $('#selOrgCountry').val(),
                        BusinessAddressBuildingNo: $.trim($('#txtCustomerOrgHouseNo').val()),
                        BusinessAddressStreet: $.trim($('#txtCustomerOrgAddress').val()),
                        BusinessAddressCityID: $('#selOrgCity').val(),
                        BusinessAddressCountryID: $('#selOrgCountry').val()
                    };


                    if (OfflineMode) {
                        result = SalesPayment_offline().SaveOrganizationInfo(pDataOrg);
                        if (result != 'Existing') {
                            ClearCustomerFreeTypeSession();
                            $('#popupAddNewCustomer').popup('close');

                            sessionStorage.CustomerOrganizationID = result;
                            pageCustomerDetails.LoadCustomer();
                            // MessageBox('Successfully saved customer details. Select the customer by clicking the Customer Record Lookup button.', 'Save Customer Details');

                        } else {
                            //MessageBox("Organization with the same corporate name already exists.", "Organization Info");
                            $('#panOrgEmpMessage').popup('close');
                            $('#TitleOrgEmp').text('Organization Info');
                            $('#MsgOrgEmp').text('Organization with the same corporate name already exists.');
                            $('#panOrgEmpMessage').popup('open');
                        }

                    } else {
                        AjaxSend({
                            url: "OrganizationShort/CheckOrgUniqueOrgName",
                            data: {
                                OrganizationID: '',
                                CompanyName: str
                            },
                            success: function (vData) {

                                if (vData.OrganizationData.Rows.length > 0) {
                                    //MessageBox("Organization with the same corporate name already exists.", "Organization Info");
                                    $('#TitleOrgEmp').text('Organization Info');
                                    $('#MsgOrgEmp').text('Organization with the same corporate name already exists.');
                                    $('#panOrgEmpMessage').popup('open');

                                } else {
                                    AjaxSend({
                                        url: 'OrganizationShort/SaveOrganizationInfo',
                                        data: pDataOrg,
                                        success: function (vSaveData) {

                                            //$('#txtOrgCustomerSearch').val($('#txtCompanyName').val());
                                            //$('#popupAddNewCustomer').popup('close');
                                            $('#tblBody').empty();
                                            window.location.hash = '';
                                            window.location.hash = 'LibraryPage';
                                            LibraryTable.CustomerList();
                                            sessionStorage.CustomerOrganizationID = vSaveData.OrganizationData;

                                            //var Products = [];
                                            //var mDate = new Date();

                                            //pData = {
                                            //    OrgCustomerID: '',
                                            //    CustomerID: $.trim(vSaveData.OrganizationData),
                                            //    ActivationDate: (mDate.getMonth() + 1) + '/' + mDate.getDate() + '/' + mDate.getFullYear(),
                                            //    IsDeactivated: 0,
                                            //    DeactivationDate: '',
                                            //    DeactivationReason: '',
                                            //    Comments: '',
                                            //    IsBan: 0,
                                            //    Products: Products
                                            //};

                                            //$('#popupAddNewCustomer').popup('close');

                                            //SaveOrganizationCustomer(pData);

                                            //pageCustomerDetails.LoadCustomer();
                                            // MessageBox('Successfully saved customer details. Select the customer by clicking the Customer Record Lookup button.', 'Save Customer Details');

                                        }
                                    });

                                }

                                //                                    $('#btnCustomerNameSave').removeAttr('disabled');
                            }
                        });

                    }
                } else {

                    var firstname = $.trim($('#txtFirstName').val())
                    firstname = firstname.toLowerCase().replace(/\b[a-z]/g, function (letter) {
                        return letter.toUpperCase();
                    });
                    var middlename = $.trim($('#txtMiddleName').val())
                    middlename = middlename.toLowerCase().replace(/\b[a-z]/g, function (letter) {
                        return letter.toUpperCase();
                    });
                    var surname = $.trim($('#txtSurname').val())
                    surname = surname.toLowerCase().replace(/\b[a-z]/g, function (letter) {
                        return letter.toUpperCase();
                    });


                    if (firstname == "" || firstname.length <= 1) {
                        if (OfflineMode)
                            Android.Toast("First name is required");

                        return;
                    }
                    if (middlename == "" || middlename.length <= 1) {
                        if (OfflineMode)
                            Android.Toast("Full middle name is required");

                        return;
                    }
                    if (surname == "" || surname.length <= 1) {
                        if (OfflineMode)
                            Android.Toast("Surname is required");

                        return;
                    }


                    //                        $('#btnCustomerNameSave').attr('disabled', true);
                    var pDataTradeName = [];
                    var pDataCategory = [];
                    var pDataPerson = {
                        IsOrganization: 0,
                        OrganizationID: '',
                        PersonFirstName: firstname,
                        PersonMiddleName: middlename,
                        PersonSurname: surname,
                        PersonSuffixName: '',
                        PersonTitleID: '',
                        PersonDOB: '',
                        PersonMotherMaidenSurname: '',
                        PersonMotherMaidenFirstName: '',
                        PersonMotherMaidenMiddleName: '',
                        OrgTradeNameList: pDataTradeName,
                        OrgCategoryList: pDataCategory,
                        IsAddAsCustomer: 1,
                        CompanyNameInitial: '',
                        IsFranchisor: 0,
                        TIN: $.trim($('#txtCustomerPersonTIN').val()),
                        OfficeAddressBuildingNo: '',
                        OfficeAddressStreet: '',
                        OfficeAddressCityID: '',
                        OfficeAddressCountryID: '',
                        BusinessAddressBuildingNo: '',
                        BusinessAddressStreet: '',
                        BusinessAddressCityID: '',
                        BusinessAddressCountryID: ''
                    };

                    if (OfflineMode) {
                        result = SalesPayment_offline().SaveOrganizationInfo(pDataPerson);
                        if (result != 'Existing') {
                            ClearCustomerFreeTypeSession();
                            $('#popupAddNewCustomer').popup('close');
                            sessionStorage.CustomerOrganizationID = result;
                            pageCustomerDetails.LoadCustomer();
                            //                                MessageBox('Successfully saved customer details. Select the customer by clicking the Customer Record Lookup button.', 'Save Customer Details');

                        } else {
                            //MessageBox("Person with the same information already exists.", "Person Info");
                            $('#TitleOrgEmp').text('Person Info');
                            $('#MsgOrgEmp').text('Person with the same information already exists.');
                            $('#panOrgEmpMessage').popup('open');

                        }
                    } else {
                        AjaxSend({
                            url: "OrganizationShort/CheckPersonUniqueName",
                            data: {
                                IsOrganization: 0,
                                OrganizationID: '',
                                PersonFirstName: firstname,
                                PersonMiddleName: middlename,
                                PersonSurname: surname,
                                PersonSuffixName: '',
                                PersonTitleID: '',
                                PersonDOB: '',
                                PersonMotherMaidenSurname: '',
                                PersonMotherMaidenFirstName: '',
                                PersonMotherMaidenMiddleName: '',
                                IsFull: false

                            },
                            success: function (vData) {

                                if (vData.OrganizationData.Rows.length > 0) {
                                    //MessageBox("Person with the same information already exists.", "Person Info");
                                    $('#TitleOrgEmp').text('Person Info');
                                    $('#MsgOrgEmp').text('Person with the same information already exists.');
                                    $('#panOrgEmpMessage').popup('open');
                                } else {
                                    AjaxSend({
                                        url: 'OrganizationShort/SaveOrganizationInfo',
                                        data: pDataPerson,
                                        success: function (vSaveData) {

                                            //$('#txtOrgCustomerSearch').val(surname);
                                            //$('#popupAddNewCustomer').popup('close');
                                            $('#tblBody').empty();
                                            window.location.hash = '';
                                            window.location.hash = 'LibraryPage';
                                            LibraryTable.CustomerList();
                                            sessionStorage.CustomerOrganizationID = vSaveData.OrganizationData;


                                            //ClearCustomerFreeTypeSession();
                                            //$('#popupAddNewCustomer').popup('close');
                                            //sessionStorage.CustomerOrganizationID = vSaveData.OrganizationData;
                                            //pageCustomerDetails.LoadCustomer();
                                            //                                                MessageBox('Successfully saved customer details. Select the customer by clicking the Customer Record Lookup button.', 'Save Customer Details');

                                        }
                                    });

                                }

                                //                                    $('#btnCustomerNameSave').removeAttr('disabled');
                            }
                        });

                    }


                }
            })
            .on('change', '#selCustomerType', function (e) {
                e.preventDefault();

                if ($('#selCustomerType').val() == 'Organization') {
                    $('#divCustomerOrg').show();
                    $('#divCustomerPerson').hide();
                } else {
                    $('#divCustomerOrg').hide();
                    $('#divCustomerPerson').show();
                }
            })
            .on('click', '#btnCustomerNameCancel', function (e) {
                e.preventDefault();

                window.location.hash = 'LibraryPage';
            })
            //Navigation Menu
            $('#btnHome').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
            });

            $('#btnSystemAdmin').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuSettings"), "_self");
            });

            $('#btnDataLibraries').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopDataLibraries"), "_self");
            });
            //End Navigation Menu

            $('#btnOrgCustAddCustomer').click(function (e) {
                e.preventDefault();

                sessionStorage.IsNewCustomer = true;

                Browser.LibraryOrganization.Click = function (e) {

                    var mDate = new Date();
                    var Products = [];

                    if (e.IsNew) {
                        setTimeout(function () {
                            clearOrgInfo();
                            getCityName();
                            getCountryName();
                            $('#popupAddNewCustomer').popup('open');
                        }, 1000);
                        sessionStorage.IsNewCustomer = false;
                    }
                    else {
                        pData = {
                            OrgCustomerID: '',
                            CustomerID: $.trim(e.ID),
                            ActivationDate: (mDate.getMonth() + 1) + '/' + mDate.getDate() + '/' + mDate.getFullYear(),
                            IsDeactivated: 0,
                            DeactivationDate: '',
                            DeactivationReason: '',
                            Comments: '',
                            IsBan: 0,
                            Products: Products
                        };

                        SaveOrganizationCustomer(pData);
                    }
                };

                Browser.LibraryOrganization.Show($('#txtOrgCustomerSearch').val());

                //$.mobile.changePage('#pageOrgCustomerInfo');
                //ClearOrgCustomerFields();
                //ClearOrgCustomerInActiveFields();

                //ShowHideOrgCustInActiveDetails();
            });

            $('#frmOrgCustomerSearch').submit(function (e) {
                e.preventDefault();

                getOrgCustomerList(); ;

                return false;
            });

            $('#btnOrgCustCustomer').click(function (e) {

                Browser.LibraryOrganization.InputData.ShowAdd = true;
                Browser.LibraryOrganization.Click = function (e) {

                    $('#txtOrgCustCustomerID').val(e.ID)
                    $('#btnOrgCustCustomer').text(e.CompanyName)
                .button('refresh');

                };

                Browser.LibraryOrganization.Show();

            });

            $('#btnOrgCustInfoSave').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgCustCustomerID").val()) == "") {
                    MessageBox("Please select customer.",
                            "Customer",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#selOrgCustActivationDateMonth").val()) == "" || $.trim($("#selOrgCustActivationDateDay").val()) == "" || $.trim($("#selOrgCustActivationDateYear").val()) == "") {
                    MessageBox("Please enter valid activation date.",
                            "Activation Date",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    return;
                }

                if ($.trim($("#selOrgCustDeactivate").val()) == "0") {

                    ClearOrgCustomerInActiveFields();

                }
                else {

                    if ($.trim($("#selOrgCustDateInActiveMonth").val()) == "" || $.trim($("#selOrgCustDateInActiveDay").val()) == "" || $.trim($("#selOrgCustDateInActiveYear").val()) == "") {
                        MessageBox("Please enter valid deactivation date.",
                                "Deactivation Date",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                        return;
                    }

                }

                SaveOrganizationCustomer();

            });

            $('#btnOrgCustInfoCancel').click(function (e) {
                e.preventDefault();

                //   ChangePage('/Organization/OrganizationCustomer');
                window.location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/Organization/OrganizationCustomer');

            });


            $('#selOrgCustActivationDateMonth').change(function (e) {
                e.preventDefault();

                setDayCollection('selOrgCustActivationDateMonth', 'selOrgCustActivationDateDay', 'selOrgCustActivationDateYear', true);

            });

            $('#selOrgCustActivationDateYear').change(function (e) {
                e.preventDefault();

                setDayCollection('selOrgCustActivationDateMonth', 'selOrgCustActivationDateDay', 'selOrgCustActivationDateYear', true);

            });

            $('#selOrgCustDateInActiveMonth').change(function (e) {
                e.preventDefault();

                setDayCollection('selOrgCustDateInActiveMonth', 'selOrgCustDateInActiveDay', 'selOrgCustDateInActiveYear', true);

            });

            $('#selOrgCustDateInActiveYear').change(function (e) {
                e.preventDefault();

                setDayCollection('selOrgCustDateInActiveMonth', 'selOrgCustDateInActiveDay', 'selOrgCustDateInActiveYear', true);

            });

            $('#btnOrgCustomerDone').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            });

            $('#btnOrgCustomerGoToSetupChecklist').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/SetupChecklist"), "_self");

            });

            $('#selOrgCustDeactivate').change(function (e) {
                e.preventDefault();

                ShowHideOrgCustInActiveDetails();

            });

            function ShowHideOrgCustInActiveDetails() {

                if ($.trim($("#selOrgCustDeactivate").val()) == "1") {
                    $("#divOrgCustDeactivateDetails").show();
                }
                else {
                    $("#divOrgCustDeactivateDetails").hide();
                }
            }

            function getOrgCustomerList() {
                var pData = {};
                pData.SearchKeyword = $("#txtOrgCustomerSearch").val();

                AjaxSend({
                    url: 'Organization/getOrganizationCustList',
                    data: pData,
                    success: function (vData) {
                        LoadOrgCustList(vData.OrgCustomerData.Rows);
                    }
                });
            }

            function LoadOrgCustList(vRows) {
                var pLV = $('#lstOrgCustomer');
                var pItem;

                $("#divOrgCustRecordCountCustomer").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .data('Data', vRows[p])
                        .click(function (e) {
                            e.preventDefault();

                            CreateNewOrgEntry(0, $(this).data('Data').OrganizationID);

                            //////                            $.mobile.changePage('#pageOrgCustomerInfo');
                            //////                            ShowOrgCustomerInfo($(this).attr('ID'));
                        })
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].CustomerName)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Activation Date: " + $.format.date(new Date(vRows[p].ActivationDate), 'MMM dd, yyyy'))
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Status: " + (vRows[p].DeActivated != '0' ? 'Deactivated ( ' + $.format.date(new Date(vRows[p].DeactivationDate), 'MMM dd, yyyy') + ' )' : 'Active'))
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Banned: " + (vRows[p].IsBan != '0' ? 'Yes' : 'No'))
                        )
                    );

                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function ShowOrgCustomerInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgCustomerFields();
                ClearOrgCustomerInActiveFields();

                AjaxSend({
                    url: "Organization/getOrganizationCustInfo",
                    data: pData,
                    success: function (vData) {
                        if (vData.OrgCustomerData.Rows.length > 0) {
                            $("#txtOrgCustID").val(vData.OrgCustomerData.Rows[0].ID || '');

                            $("#txtOrgCustCustomerID").val(vData.OrgCustomerData.Rows[0].CustomerID || '');
                            $("#btnOrgCustCustomer").text(vData.OrgCustomerData.Rows[0].CustomerName || 'Please Select').button('refresh');

                            $("#selOrgCustActivationDateMonth").val(vData.OrgCustomerData.Rows[0].ActivationDateMonth || '').selectmenu('refresh');
                            $("#selOrgCustActivationDateYear").val(vData.OrgCustomerData.Rows[0].ActivationDateYear || '').selectmenu('refresh');
                            setDayCollection('selOrgCustActivationDateMonth', 'selOrgCustActivationDateDay', 'selOrgCustActivationDateYear', true);
                            $("#selOrgCustActivationDateDay").val(vData.OrgCustomerData.Rows[0].ActivationDateDay || '').selectmenu('refresh');

                            $("#selOrgCustDeactivate").val(vData.OrgCustomerData.Rows[0].DeActivated || '0').slider("refresh");

                            if ($("#selOrgCustDeactivate").val() != "0") {
                                $("#selOrgCustDateInActiveMonth").val(vData.OrgCustomerData.Rows[0].DeactivationDateMonth || '').selectmenu('refresh');
                                $("#selOrgCustDateInActiveYear").val(vData.OrgCustomerData.Rows[0].DeactivationDateYear || '').selectmenu('refresh');
                                setDayCollection('selOrgCustDateInActiveMonth', 'selOrgCustDateInActiveDay', 'selOrgCustDateInActiveYear', true);
                                $("#selOrgCustDateInActiveDay").val(vData.OrgCustomerData.Rows[0].DeactivationDateDay || '').selectmenu('refresh');

                                $("#txtOrgCustInActiveReason").val(vData.OrgCustomerData.Rows[0].DeactivationReason || '').keyup();
                                $("#txtOrgCustComments").val(vData.OrgCustomerData.Rows[0].Comments || '').keyup();

                                $("#selOrgCustBan").val(vData.OrgCustomerData.Rows[0].IsBan || '0').slider("refresh");
                            }

                            ShowHideOrgCustInActiveDetails();

                        }
                    }
                });

            };

            function SaveOrganizationCustomer(pData2) {
                var pData = [];
                var Products = [];

                $('#lstBadOrder li a').each(function (m_index) {

                    Products[m_index] = { ProductID: $(this).attr('ID'),
                        BinID: $(this).data('BinID'),
                        MaxCap: $(this).data('MaxCap'),
                        MinCap: $(this).data('MinCap'),
                        CustomerBarCode: $(this).data('CustomerBarCode'),
                        SellingPrice: $(this).data('SellingPrice')
                    };
                });

                if (pData2 == undefined) {
                    pData = {
                        OrgCustomerID: $.trim($("#txtOrgCustID").val()),
                        CustomerID: $.trim($("#txtOrgCustCustomerID").val()),
                        ActivationDate: $("#selOrgCustActivationDateMonth").val() + '/' + $("#selOrgCustActivationDateDay").val() + '/' + $("#selOrgCustActivationDateYear").val(),
                        IsDeactivated: $("#selOrgCustDeactivate").val(),
                        DeactivationDate: $("#selOrgCustDateInActiveMonth").val() + '/' + $("#selOrgCustDateInActiveDay").val() + '/' + $("#selOrgCustDateInActiveYear").val(),
                        DeactivationReason: $("#txtOrgCustInActiveReason").val(),
                        Comments: $("#txtOrgCustComments").val(),
                        IsBan: $("#selOrgCustBan").val(),
                        Products: Products
                    };
                } else {
                    pData = pData2;
                }


                AjaxSend({
                    url: "Organization/CheckOrganizationCustomer",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgCustomerData.Rows.length > 0) {
                            MessageBox("Customer being added already exist.",
                            "Customer Exist",
                            [
                                { Text: 'Ok' }
                            ]
                        );
                        }
                        else {
                            AjaxSend({
                                url: "Organization/SaveOrganizationCustomer",
                                data: pData,
                                success: function (vSaveData) {

                                    if (vSaveData.OrgCustomerData == "ActivationDateError") {
                                        MessageBox("Please enter valid activation date.",
                                        "Activation Date",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                                    }
                                    else if (vSaveData.OrgCustomerData == "DeactivationDateError") {
                                        MessageBox("Please enter valid deactivation date.",
                                        "Deactivation Date",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                                    }
                                    else {
                                        $.mobile.changePage('#pageOrgCustomer');
                                        getOrgCustomerList();
                                    }

                                }
                            });

                        }

                    }
                });

            };

            function ClearOrgCustomerFields() {

                $("#txtOrgCustID").val('');

                $("#txtOrgCustCustomerID").val('');
                $("#btnOrgCustCustomer").text('Please Select').button('refresh');

                $("#selOrgCustActivationDateMonth").val('').selectmenu('refresh');
                $("#selOrgCustActivationDateDay").val('').selectmenu('refresh');
                $("#selOrgCustActivationDateYear").val('').selectmenu('refresh');

            };

            function ClearOrgCustomerInActiveFields() {

                $("#selOrgCustDeactivate").val('0').slider("refresh");

                $("#selOrgCustDateInActiveMonth").val('').selectmenu('refresh');
                $("#selOrgCustDateInActiveDay").val('').selectmenu('refresh');
                $("#selOrgCustDateInActiveYear").val('').selectmenu('refresh');

                $("#txtOrgCustInActiveReason").val('');
                $("#txtOrgCustComments").val('');

                $("#selOrgCustBan").val('0').slider("refresh");
            };

            function CreateNewOrgEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;
                window.pageSourcePage = $.mobile.activePage;

                AjaxSend({
                    url: 'OrganizationShort/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        if ((vData.IsNew || '') == "TRUE") {
                            window.IsPageLoaded = false;
                            //ChangePage('/OrganizationShort/OrganizationInfo');
                            window.location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/OrganizationShort/OrganizationInfo');
                        }
                        else {
                            //ChangePage('/OrganizationShort/OrganizationSummary');
                            window.location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/OrganizationShort/OrganizationSummary');
                        }
                    }
                });

            }
        });

        function clearOrgInfo() {
            $('#txtCompanyName').val('');
            $('#txtCustomerTIN').val('');
            $('#txtCustomerOrgHouseNo').val('');
            $('#txtCustomerOrgAddress').val('');
            $('#txtFirstName').val('');
            $('#txtMiddleName').val('');
            $('#txtSurname').val('');
            $('#txtCustomerPersonTIN').val('');
        }

        function getCityName() {
            if (OfflineMode) {
                var vCity;
                vCity = SalesMenu_offline().getCityListPH();
                DisplayCities(vCity);
            } else {
                AjaxSend({
                    url: 'City/getCityListAll',
                    success: function (vData) {

                        DisplayCities(vData);

                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }
        }


        function getCountryName() {
            if (OfflineMode) {
                var vCountry;
                vCountry = SalesMenu_offline().getCountryListAll();
                DisplayCountries(vCountry);
            } else {
                AjaxSend({
                    url: 'Country/getCountryListAll',
                    success: function (vData) {

                        DisplayCountries(vData);

                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

        }

        function DisplayCountries(vData) {
            var vRows = vData.CountryData.Rows;

            $('#selOrgCountry').empty();
            //                    $('#selPersonCountry').empty();

            $('#selOrgCountry').append(new Option('Please Select Country', ''));
            //                    $('#selPersonCountry').append(new Option('Please Select Country', ''));

            for (var p = 0; p <= vRows.length - 1; p++) {
                $('#selOrgCountry').append(new Option(vRows[p].Country, vRows[p].ID, false, false));
                //                        $('#selPersonCountry').append(new Option(vRows[p].Country, vRows[p].ID, false, false));
            }
            $("#selOrgCountry option:contains('Philippines')").attr('selected', 'selected');
            //                    $("#selPersonCountry option:contains('Philippines')").attr('selected', 'selected');

            try {
                $('#selOrgCountry').selectmenu('refresh', true);
                //                        $('#selPersonCountry').selectmenu('refresh', true);
            } catch (e) {

                $('#selOrgCountry').selectmenu();
                $('#selOrgCountry').selectmenu('refresh', true);
                //                        $('#selPersonCountry').selectmenu();
                //                        $('#selPersonCountry').selectmenu('refresh', true);

            }
        }

        function DisplayCities(vData) {
            var vRows = vData.CityData.Rows;

            $('#selOrgCity').empty();
            //                    $('#selPersonCity').empty();

            $('#selOrgCity').append(new Option('Please Select City', ''));
            //                    $('#selPersonCity').append(new Option('Please Select City', ''));

            for (var p = 0; p <= vRows.length - 1; p++) {
                $('#selOrgCity').append(new Option(vRows[p].City, vRows[p].ID, false, false));
                //                        $('#selPersonCity').append(new Option(vRows[p].City, vRows[p].ID, false, false));
            }
            //                     $("#selOrgCity option:contains('Philippines')").attr('selected', 'selected');
            //                     $("#selPersonCity option:contains('Philippines')").attr('selected', 'selected');

            try {
                $('#selOrgCity').selectmenu('refresh', true);
                //                        $('#selPersonCity').selectmenu('refresh', true);
            } catch (e) {

                $('#selOrgCity').selectmenu();
                $('#selOrgCity').selectmenu('refresh', true);
                //                        $('#selPersonCity').selectmenu();
                //                        $('#selPersonCity').selectmenu('refresh', true);
            }
        }

    });