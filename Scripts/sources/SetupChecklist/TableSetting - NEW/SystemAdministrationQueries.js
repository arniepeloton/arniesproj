require([
    'common',
    '/Scripts3/model/ItemModifiers.js?crc=',
    '/Scripts/jquery.dataTables.min.js'
], function () {

    var mModel = App.model.ItemModifiers();

    $(function () {
        var intReceiptSetMessageState = 3;
        var intReceiptSetMessageFooter = 1;
        var isOrganizationPageLoaded = "";
        var isSecurityRoleSummaryPageLoaded = "";
        var isSecurityRoleInfoPageLoaded = "";
        var isDeviceInfoPageLoaded = "";
        var isDeviceInfoSummaryPageLoaded = "";
        var isOrgAddressPageLoaded = "";
        var isPrinterPageLoaded = "";
        var isReceiptSettingPageLoaded = "";
        var isAddEmployeePageLoaded = "";
        var isViewEmployeePageLoaded = "";
        var isClickedAddEmployee = false;
        var isWorkSchedulePageLoaded = "";
        var isPrinterRolePageLoaded = "";
        var isDecimalsPageLoaded = "";
        var isPOSPageLoaded = "";
        var isReceivingAndTransfersPageLoaded = "";
        var isInventoryCountPageLoaded = "";
        var isPurchaseOrderSettingsPageLoaded = "";
        var isPurchaseOrderPageLoaded = "";
        var isSalesOrderPageLoaded = "";
        var isAddHotButtonPageLoaded = "";
        var isViewHotButtonPageLoaded = "";
        var isFranchisorPageLoaded = "";
        var isAddCustomerPageLoaded = "";
        var isViewCustomerPageLoaded = "";
        var isAddCRMPageLoaded = "";
        var isLoyaltyProgramPageLoaded = "";
        var isViewCRMPageLoaded = "";
        var isAddItemModifiersPageLoaded = "";
        var isSalesReturnPageLoaded = "";
        var isDiscountPageLoaded = "";
        var isPIMPageLoaded = "";
        var isPIMWebFormPageLoaded = "";
        var isFOWSTProdListPageLoaded = "";
        var isSingleProdPricePageLoaded = "";
        var isBulkPricePageLoaded = "";
        var isCompanyWideProductPageLoaded = "";
        var isSupplierPageLoaded = "";
        var isTableViewPageLoaded = "";
        var isItemModifierClicked = false;
        var isPIMClicked = false;
        var isPIMWebFormClicked = false;
        var isFOWSTClicked = false;
        var isDeviceClicked = false;
        var isReceiptClicked = false;
        var isSecRoleClicked = false;
        var isEmployeeClicked = false;
        var isSalesReturnClicked = false;
        var isDiscountClicked = false;
        var isPrinterRoleClicked = false;
        var isDecimalClicked = false;
        var isPOSClicked = false;
        var isSalesOrderClicked = false;
        var isRecTransClicked = false;
        var isInventoryCountClicked = false;
        var isPurchaseOrderClicked = false;
        var isHotButtonClicked = false;
        var isGroupNameClicked = false;
        var isCustomerClicked = false;
        var isSupplierClicked = false;
        var isSalesTeamClicked = false;
        var isDoneClicked = false;
        var isPrintersClicked = false;
        var isTableViewClicked = false;
        var pData = {};
        var tblSysAdminDataTable = "";
        var mPrinters; // App.controller.Printer
        var mPurchaseOrderSettings; // App.controller.PurchaseOrderSettings

        function InitVal() {
            isItemModifierClicked = true;
            isPIMClicked = true;
            isFOWSTClicked = true;
            isDeviceClicked = true;
            isReceiptClicked = true;
            isSecRoleClicked = true;
            isEmployeeClicked = true;
            isSalesReturnClicked = true;
            isDiscountClicked = true;
            isPrinterRoleClicked = true;
            isDecimalClicked = true;
            isPOSClicked = true;
            isSalesOrderClicked = true;
            isRecTransClicked = true;
            isInventoryCountClicked = true;
            isPurchaseOrderClicked = true;
            isHotButtonClicked = true;
            isGroupNameClicked = true;
            isCustomerClicked = true;
            isSupplierClicked = true;
            isSalesTeamClicked = true;
            isPrintersClicked = true;
            isTableViewClicked = true;
        }

        window.LibraryTable = {

            //Display Organization Page
            OrganizationPage: function () {
                //$.getScript("/Scripts/sources/Organization/OrganizationSummary.js?crc=");
                if (isOrganizationPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationSummary?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrganizationPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isOrganizationPageLoaded).trigger('create');
                    OrgSummary.ShowSumOrgInfo();
                }

            },

            //FOWST
            Fowst: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + '&mod=fowst',
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Name</b>' + data.Name;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Address</b>' + data.CompleteFowstAddress;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Retail Price</b>' + data.ModifiedBy;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return '<b class="ui-table-cell-label">Last Modified Date</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View FOWST";
                        sessionStorage['ViewID'] = data.ID;
                        sessionStorage["FowstTitle"] = data.Name;
                        //Fowst.ShowFowstInfo(sessionStorage.ViewID);
                        if (isFOWSTClicked) {
                            LibraryTable.AddFowstPage();
                        }
                        window.location.hash = 'UniversalPage';
                        InitVal();
                    },
                    disablecols: [0, 2] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search FOWST
            FowstSearch: function (s) {
                var data = { SearchName: s }

                AjaxSend({
                    url: 'SetupCheckList/FowstSearch',
                    data: data,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.FOWST.Rows.length > 0) {
                            for (var p = 0; p < vData.FOWST.Rows.length; p++) {
                                $('#lstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.FOWST.Rows[p].Name)
                                        )
                                        .data('data', vData.FOWST.Rows[p])
                                        .click(function (e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function (m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) { alert('This record already exists in your database. To add a new unique record, click on the link below.'); return };

                                            sessionStorage["AddViewLibBCName"] = "Add FOWST";
                                            window.location.hash = 'UniversalPage';
                                            LibraryTable.AddFowstPage();
                                            InitVal();
                                        })
                                    ));

                            }

                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Fowst Page
            AddFowstPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isOrgAddressPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationAddress?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrgAddressPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData).trigger('create');
                            //Fowst.ShowFowstInfo(sessionStorage.ViewID || '');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isOrgAddressPageLoaded).trigger('create');
                    Fowst.ShowFowstInfo(sessionStorage.ViewID || '');
                }
            },

            //Authorized Devices
            Devices: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=device",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Phone Name</b>' + data.PhoneName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Phone Model</b>' + data.PhoneModel;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">IMEI</b>' + data.IMEI;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">UUID</b>' + data.UUID;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Status</b>' + (CBool(data.IsActive) ? 'Active' : 'Inactive');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Last Modified By</b>' + data.ModifiedBy;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return '<b class="ui-table-cell-label">Last Modified Date</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Device";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isDeviceClicked) {
                            LibraryTable.ViewDevicePage();
                        }
                        InitVal();
                    },
                    disablecols: [0, 2, 3, 4] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Devices
            DevicesSearch: function (s) {
                var data = { SearchName: s }
                AjaxSend({
                    url: 'SetupCheckList/DeviceSearch',
                    data: data,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.AuthorizedDevice.Rows.length > 0) {
                            for (var p = 0; p < vData.AuthorizedDevice.Rows.length; p++) {

                                $('#lstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text('Device Name: ' + vData.AuthorizedDevice.Rows[p].PhoneName)
                                        )
                                        .append($('<p>')
                                            .text('Model: ' + vData.AuthorizedDevice.Rows[p].PhoneModel)
                                        )
                                        .append($('<p>')
                                            .text('IMEI: ' + vData.AuthorizedDevice.Rows[p].IMEI)
                                        )
                                        .append($('<p>')
                                            .text('UUID: ' + vData.AuthorizedDevice.Rows[p].UUID)
                                        )
                                        .data('data', vData.AuthorizedDevice.Rows[p])
                                        .click(function (e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function (m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) { alert('This record already exists in your database. To add a new unique record, click on the link below.'); return };

                                            sessionStorage["AddViewLibBCName"] = "Add Device";
                                            window.location.hash = 'UniversalPage';
                                            LibraryTable.AddDevicePage();
                                            InitVal();
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Device Page
            AddDevicePage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isDeviceInfoPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Device/DeviceInfo?cache=1&mode=wizard&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isDeviceInfoPageLoaded = vData;
                            $('#UniversalDiv').empty(); $('#UniversalDiv').append(vData).trigger('create'); $("#txtID").val(''); $("#txtPhoneName").val(''); $("#txtPhoneModel").val(''); $("#txtIMEI").val(''); $("#txtUUID").val(''); $("#txtOS").val(''); $("#txtDeviceNo").val(''); $("#selDeviceDeactivate").val('1').slider("refresh");
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty(); $('#UniversalDiv').append(isDeviceInfoPageLoaded).trigger('create'); $("#txtID").val(''); $("#txtPhoneName").val(''); $("#txtPhoneModel").val(''); $("#txtIMEI").val(''); $("#txtUUID").val(''); $("#txtOS").val(''); $("#txtDeviceNo").val(''); $("#selDeviceDeactivate").val('1').slider("refresh");
                }
            },

            ViewDevicePage: function () {

                var pData = {
                    IsNew: 0,
                    ID: sessionStorage.ViewID
                };

                AjaxSend({
                    url: 'Device/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        if ((vData.IsNew || '') == "TRUE") {
                            PromptUserAccess('Devices', {
                                OnClickOKSuccess: function () {
                                    //window.open(GetPathFormat("/Device/DeviceInfo"), "_self");
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/Device/DeviceInfo');
                                }
                            });
                        }
                        else {

                            if (GetUrlParameters().mode == 'wizard') {
                                window.open(GetPathFormat("/Device/DeviceInfoSummary?mode=wizard"), "_self");
                            } else {
                                //window.open(GetPathFormat("/Device/DeviceInfoSummary"), "_self");
                                InitVal();
                                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                                if (isDeviceInfoSummaryPageLoaded == "") {
                                    AjaxSend({
                                        ExactUrl: '/Device/DeviceInfoSummary',
                                        type: 'GET',
                                        dataType: 'text',
                                        success: function (vData) {
                                            isDeviceInfoSummaryPageLoaded = vData;
                                            $('#UniversalDiv').html('');
                                            $('#UniversalDiv').append(vData).trigger('create');
                                            Library.View.ShowDeviceInfo();
                                            App.Deferred.fulfill('FQ26E5TP76');
                                            isGroupNameClicked = true;
                                        }
                                    });
                                }
                                else {
                                    $('#UniversalDiv').html('');
                                    $('#UniversalDiv').append(isDeviceInfoSummaryPageLoaded).trigger('create');
                                    Library.View.ShowDeviceInfo();
                                    isGroupNameClicked = true;
                                }
                            }
                        }
                    }
                });
            },

            //Receipt Settings
            ReceiptSettings: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=receipt",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Company Name</b>' + data.CompanyName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Trade Name</b>' + data.BusinessTradeName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Paper Size</b>' + data.PaperSize;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Last Modified By</b>' + data.ModifiedBy;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return '<b class="ui-table-cell-label">Last Modified Date</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Receipt Template";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        isGroupNameClicked = true;

                        var pData = {};
                        pData.IsNew = 0;
                        pData.ID = sessionStorage.ViewID;

                        AjaxSend({
                            url: 'ReceiptSettings/setCreateNew',
                            data: pData,
                            success: function (vData) {
                                LibraryTable.AddReceiptSettingsPage();
                            }
                        });

                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Receipt Settings
            ReceiptSettingsSearch: function (s) {
                var pData = {};
                pData.SearchString = s;

                AjaxSend({
                    url: 'ReceiptSettings/getReceiptSettingsList',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.ReceiptSettingsData.Rows.length > 0) {
                            for (var p = 0; p < vData.ReceiptSettingsData.Rows.length; p++) {
                                $('#lstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.ReceiptSettingsData.Rows[p].CompanyName)
                                        )
                                        .append($('<p>')
                                            .text('Model: ' + vData.ReceiptSettingsData.Rows[p].PaperSize)
                                        )
                                        .data('data', vData.ReceiptSettingsData.Rows[p])
                                        .click(function (e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function (m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) { alert('This record already exists in your database. To add a new unique record, click on the link below.'); return };
                                            InitVal();
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Security Role Page
            AddReceiptSettingsPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isReceiptSettingPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/ReceiptSettings/ReceiptSettingsInfo',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isReceiptSettingPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isReceiptSettingPageLoaded).trigger('create');
                    setTimeout(function () { ReceiptSettings.ShowReceiptInfo(); }, 1000);
                }
            },

            //Security Roles
            SecurityRoles: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=security",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Code</b>' + data.Code;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Role</b>' + data.Role;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Description</b>' + data.Description;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Last Modified By</b>' + data.ModifiedBy;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return '<b class="ui-table-cell-label">Last Modified Date</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Security Role";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isSecRoleClicked) {
                            LibraryTable.ViewSecurityRolePage();
                        }
                        InitVal();
                    },
                    disablecols: [0, 3] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Security Roles
            SecurityRolesSearch: function (s) {

                var pData = {};
                pData.SearchString = s;

                AjaxSend({
                    url: 'SecurityRole/getSecurityRoleList',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.SecurityRoleData.Rows.length > 0) {
                            for (var p = 0; p < vData.SecurityRoleData.Rows.length; p++) {
                                $('#lstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.SecurityRoleData.Rows[p].Code)
                                        )
                                        .append($('<p>')
                                            .text('Model: ' + vData.SecurityRoleData.Rows[p].Role)
                                        )
                                        .data('data', vData.SecurityRoleData.Rows[p])
                                        .click(function (e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function (m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) { alert('This record already exists in your database. To add a new unique record, click on the link below.'); return };
                                            InitVal();
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Security Role Page
            AddSecurityRolePage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isSecurityRoleInfoPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SecurityRole/SecurityRoleInfo?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isSecurityRoleInfoPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');

                            if (sessionStorage.AddViewLibBCName == "Add Security Role") {
                                ShowSecurityRoleInfo('');
                            }
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isSecurityRoleInfoPageLoaded).trigger('create');
                    ShowSecurityRoleInfo('');
                }
            },

            ViewSecurityRolePage: function () {
                var pData = {};

                pData.IsNew = 0;
                pData.ID = sessionStorage.ViewID;

                AjaxSend({
                    url: 'SecurityRole/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        window.IsPageLoaded = false;
                        if ((vData.IsNew || '') == "TRUE") {
                            PromptUserAccess('Security Roles', {
                                OnClickOKSuccess: function () {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/SecurityRole/SecurityRoleInfo');
                                }
                            });
                        }
                        else {
                            //window.open(GetPathFormat("/SecurityRole/SecurityRoleSummary"), "_self");
                            InitVal();
                            $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                            if (isSecurityRoleSummaryPageLoaded == "") {
                                AjaxSend({
                                    ExactUrl: '/SecurityRole/SecurityRoleSummary',
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function (vData) {
                                        isSecurityRoleSummaryPageLoaded = vData;
                                        $('#UniversalDiv').html('');
                                        $('#UniversalDiv').append(vData).trigger('create');

                                        Library.View.ShowSecurityInfo();
                                        App.Deferred.fulfill('FQ26E5TP76');
                                    }
                                });
                            }
                            else {
                                $('#UniversalDiv').html('');
                                $('#UniversalDiv').append(isSecurityRoleSummaryPageLoaded).trigger('create');
                                Library.View.ShowSecurityInfo();
                            }
                        }
                        isGroupNameClicked = true;
                    }
                });
            },

            //Employees
            Employees: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=employee",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Name</b>' + data.PeopleName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.DateHired == null) {
                                    return "";
                                }
                                var date = data.DateHired;
                                return '<b class="ui-table-cell-label">Date Hired</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                var status = GetEmployeeStatus(data);                                
                                return '<b class="ui-table-cell-label">Status</b>' + status;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Banned</b>' + (CBool(data.IsBan) ? 'Yes' : 'No');
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Employee";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;;
                        sessionStorage["txtOrgPeopleID"] = data.ID;;
                        sessionStorage["btnOrgEmployeeName"] = data.PeopleName;
                        sessionStorage["EmployeeStatus"] = GetEmployeeStatus(data);
                        sessionStorage["EmpFirstName"] = data.PeopleFirstName;
                        if (isEmployeeClicked) {
                            LibraryTable.ViewEmployeePage();
                        }
                        InitVal();
                    },
                    disablecols: [0, 3] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Employees
            EmployeesSearch: function (s) {

                var pData = {};
                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryPeople',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].PeopleName)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function (m_index) {
                                                if ($.trim($(this).data('data').PeopleID) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) { alert('This record already exists in your database. To add a new unique record, click on the link below.'); return };

                                            sessionStorage["OptionToAdd"] = "SendInvite";
                                            sessionStorage["txtOrgPeopleID"] = $(this).data('data').ID;
                                            sessionStorage["btnOrgEmployeeName"] = $(this).data('data').PeopleName;
                                            sessionStorage["AddViewLibBCName"] = "Add Employee";
                                            sessionStorage["EmpFirstName"] = $(this).data('data').PeopleFirstName;
                                            window.location.hash = 'UniversalPage';
                                            InitVal();
                                            LibraryTable.AddEmployeePage();
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Employee Page
            AddEmployeePage: function (option) {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isAddEmployeePageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationEmployee?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddEmployeePageLoaded = vData;
                            isViewEmployeePageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');

                            if (sessionStorage.OptionToAdd == "SendInvite") {
                                $('#pageOrgEmployeeInfo').show();
                                $('#txtOrgPeopleID').val(sessionStorage.txtOrgPeopleID);
                                $('#txtOrgEmployeeNew').val(sessionStorage.btnOrgEmployeeName);
                                $('#btnOrgEmpInfoSaveAndInvite').html('<strong><h3>' + sessionStorage.EmpFirstName + '\'s Mobiz app</h3></strong>');
                                $('#btnOrgEmpInfoSaveAndConfirm').html('<strong><h3>Let ' + sessionStorage.EmpFirstName + ' sign in and confirm now</h3></strong>');
                                $('#btnOrgEmpInfoSendConfirmation').html('<strong><h3>' + sessionStorage.EmpFirstName + '\'s Email</h3></strong>');
                                $('#btnOrgAddEmployee').trigger('click');
                                setTimeout(function () {
                                    $('#txtOrgEmployeeNew').val(sessionStorage.btnOrgEmployeeName);
                                }, 1200)
                            }
                            else if (sessionStorage.OptionToAdd == "Add Employee") {
                                $('#pageOrgEmployeeAdd').show();
                            }
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isAddEmployeePageLoaded).trigger('create');

                    if (sessionStorage.OptionToAdd == "SendInvite") {
                        $('#pageOrgEmployeeInfo').show();
                        $('#txtOrgPeopleID').val(sessionStorage.txtOrgPeopleID);
                        $('#txtOrgEmployeeNew').val(sessionStorage.btnOrgEmployeeName);
                        $('#btnOrgEmpInfoSaveAndInvite').html('<strong><h3>' + sessionStorage.EmpFirstName + '\'s Mobiz app</h3></strong>');
                        $('#btnOrgEmpInfoSaveAndConfirm').html('<strong><h3>Let ' + sessionStorage.EmpFirstName + ' sign in and confirm now</h3></strong>');
                        $('#btnOrgEmpInfoSendConfirmation').html('<strong><h3>' + sessionStorage.EmpFirstName + '\'s Email</h3></strong>');
                        $('#btnOrgAddEmployee').trigger('click');
                        setTimeout(function () {
                            $('#txtOrgEmployeeNew').val(sessionStorage.btnOrgEmployeeName);
                        }, 1200)
                        OrgEmp.GetRequiredData();
                    }
                    else if (sessionStorage.OptionToAdd == "Add Employee") {
                        $('#pageOrgEmployeeAdd').show();
                        OrgEmpAdd.ShowOrgEmpAddData();
                    }
                }

            },

            //Display View Employee Page
            ViewEmployeePage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isViewEmployeePageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationEmployee?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isViewEmployeePageLoaded = vData;
                            isAddEmployeePageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            $('#pageOrgEmployeeInfo').show();
                            setMonthCollection("selOrgDateHiredMonth2");
                            setYearCollection("selOrgDateHiredYear2");

                            setMonthCollection("selOrgDateInActiveMonth");
                            setYearCollection("selOrgDateInActiveYear");

                            $('#btnOrgEmpInfoSaveAndInvite').html('<strong><h3>' + sessionStorage.EmpFirstName + '\'s Mobiz app</h3></strong>');
                            $('#btnOrgEmpInfoSaveAndConfirm').html('<strong><h3>Let ' + sessionStorage.EmpFirstName + ' sign in and confirm now</h3></strong>');
                            $('#btnOrgEmpInfoSendConfirmation').html('<strong><h3>' + sessionStorage.EmpFirstName + '\'s Email</h3></strong>');

                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isViewEmployeePageLoaded).trigger('create');
                    $('#pageOrgEmployeeInfo').show();
                    $('#btnOrgEmpInfoSaveAndInvite').html('<strong><h3>' + sessionStorage.EmpFirstName + '\'s Mobiz app</h3></strong>');
                    $('#btnOrgEmpInfoSaveAndConfirm').html('<strong><h3>Let ' + sessionStorage.EmpFirstName + ' sign in and confirm now</h3></strong>');
                    $('#btnOrgEmpInfoSendConfirmation').html('<strong><h3>' + sessionStorage.EmpFirstName + '\'s Email</h3></strong>');
                    OrgEmp.ShowOrgEmpData();
                }

            },

            //Display Work Schedule Page
            WorkSchedulePage: function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        var pData = {};
                        pData.OrgID = sessionStorage["OrgID"];
                        InitVal();
                        if (isWorkSchedulePageLoaded == "") {
                            AjaxSend({
                                ExactUrl: '/Organization/OrganizationEmployeeWorkingHours?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                type: 'GET',
                                dataType: 'text',
                                success: function (vData) {
                                    isWorkSchedulePageLoaded = vData;
                                    $('#UniversalDiv').empty();
                                    $('#UniversalDiv').append(vData).trigger('create');
                                    setTimeout(function () {
                                        OrgEmpWorkHours.GetSchedule();
                                    }, 2000)
                                    App.Deferred.fulfill('FQ26E5TP76');
                                }
                            });
                        }
                        else {
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(isWorkSchedulePageLoaded).trigger('create');
                            OrgEmpWorkHours.GetSchedule();
                        }
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            },

            //Display PIM Page
            PIMPage: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=pim",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Short Desc.</b>' + data.ProductName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Product No.</b>' + (data.ProductNo || '');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Retail Price</b>' + AccountingFormat(data.RetailPrice);
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Brand</b>' + data.Brand;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.ActivationDate == null) {
                                    return "";
                                }
                                var ActivationDate = data.ActivationDate;
                                return '<b class="ui-table-cell-label">Activation Date</b>' + $.format.date(new Date(parseInt(ActivationDate.substr(6))), 'MMM dd, yyyy');
                            }
                        }                        
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "SKU Record";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ProductID;
                        if (isPIMClicked) {
                            LibraryTable.CompanyWideProductListPage();
                        }
                        InitVal();
                    },
                    disablecols: [0, 3] //if all '_all'
                }
                
                SysAdminDataTable(pData);
            },

            //Search PIM Search
            PIMSearch: function (s) {
                var pData = {};
                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryPIMProductPublic',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].ProductName)
                                        )
                                        .append($('<p>')
                                            .text('Published by: ' + vData.Results.Rows[p].PublishedBy + ' (Brand Owner)')
                                        )
                                        .append($('<p>')
                                            .text('Uplaoded by: ' + vData.Results.Rows[p].UploadedBy)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            sessionStorage["AddViewLibBCName"] = "Add Product";
                                            window.location.hash = 'UniversalPage';
                                            sessionStorage["ViewID"] = $(this).attr('ID');
                                            InitVal();
                                            LibraryTable.CompanyWideProductListPage();
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Sales Return Settings Page
            SalesReturnsSettingsPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isSalesReturnPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SalesReturns/Settings?cache=1&OrgID=' + sessionStorage.OrgID,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isSalesReturnPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isSalesReturnPageLoaded).trigger('create');
                    SalesReturn.GetSettings();
                }
            },

            //Display Discount Settings Page
            DiscountSettingsPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isDiscountPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/DiscountSettings?cache=1&OrgID=' + sessionStorage.OrgID,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isDiscountPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isDiscountPageLoaded).trigger('create');
                    DiscountSetting.GetDiscountSettings();
                }
            },

            //Printer Roles
            PrinterRoles: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=printerrole",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Printer Role</b>' + data.PrinterRole;
                            }
                        },                        
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Printer Type</b>' + data.PrinterType;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Active</b>' + (CBool(data.IsActive) ? 'Yes' : 'No');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Last Modified By</b>' + data.ModifiedBy;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return '<b class="ui-table-cell-label">Last Modified Date</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Printer Role";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isPrinterRoleClicked) {
                            LibraryTable.ViewPrinterRolesPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Printer Roles
            PrinterRolesSearch: function (s) {

                var pData = {};
                pData.SearchString = s;

                AjaxSend({
                    url: 'DevicePrinter/getDevicePrinterList',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.DevicePrinterData.Rows.length > 0) {
                            for (var p = 0; p < vData.DevicePrinterData.Rows.length; p++) {
                                $('#lstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text('Printer Role: ' + vData.DevicePrinterData.Rows[p].PrinterRole)
                                        )
                                        .append($('<p>')
                                            .text('Printer Type: ' + vData.DevicePrinterData.Rows[p].PrinterType)
                                        )
                                        .append($('<p>')
                                            .text('Active: ' + (vData.DevicePrinterData.Rows[p].IsActive == 1 ? 'Yes' : 'No'))
                                        )
                                        .data('data', vData.DevicePrinterData.Rows[p])
                                        .click(function (e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function (m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) { alert('This record already exists in your database. To add a new unique record, click on the link below.'); return };
                                            InitVal();
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Printer Roles Page
            AddPrinterRolesPage: function () {
                //$.getScript("/Scripts/sources/DevicePrinter/DevicePrinterInfo.js?crc=");
                AjaxSend({
                    ExactUrl: '/DevicePrinter/DevicePrinterInfo?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        $('#UniversalDiv').empty();
                        $('#UniversalDiv').append(vData).trigger('create');

                        setTimeout(function () {
                            if ($('#tblCategories tr').length == 0) {
                                var pData = {};
                                pData.IsNew = 1;
                                pData.ID = '';

                                AjaxSend({
                                    url: 'DevicePrinter/setCreateNew',
                                    data: pData,
                                    success: function (vData) {

                                        if ((vData.IsNew || '') == "TRUE") {
                                            AjaxSend({
                                                url: 'DevicePrinter/IsNewEntry',
                                                success: function (vData) {
                                                    if (vData.IsNew == 'TRUE') {

                                                        $("#divCategories").show();
                                                        GetAllCategories('');

                                                    }
                                                    else {
                                                        ShowDevicePrinterInfo(vData.DevicePrinterID || '');
                                                    }
                                                }
                                            });
                                        }
                                        else {
                                            if (GetUrlParameters().mode == 'wizard') {
                                                window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary?mode=wizard"), "_self");
                                            } else
                                                window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary"), "_self");
                                        }

                                    }
                                });
                            }
                        }, 1500)
                        App.Deferred.fulfill('FQ26E5TP76');
                    }
                });
            },

            ViewPrinterRolesPage: function () {
                var pData = {};
                pData.IsNew = 0;
                pData.ID = sessionStorage["ViewID"];

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'DevicePrinter/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        if ((vData.IsNew || '') == "TRUE") {
                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/DevicePrinter/DevicePrinterInfo');
                        }
                        else {
                            isGroupNameClicked = true;
                            if (GetUrlParameters().mode == 'wizard') {
                                AjaxSend({
                                    ExactUrl: '/DevicePrinter/DevicePrinterSummary?mode=wizard',
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function (vData) {
                                        $('#UniversalDiv').empty();
                                        $('#UniversalDiv').append(vData).trigger('create');
                                        App.Deferred.fulfill('FQ26E5TP76');
                                    }
                                });
                            } else {
                                //window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary"), "_self");
                                InitVal();
                                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                                if (isPrinterRolePageLoaded == "") {
                                    AjaxSend({
                                        ExactUrl: '/DevicePrinter/DevicePrinterSummary',
                                        type: 'GET',
                                        dataType: 'text',
                                        success: function (vData) {
                                            isPrinterRolePageLoaded = vData;
                                            $('#UniversalDiv').empty();
                                            $('#UniversalDiv').append(vData).trigger('create');
                                            Library.View.ShowDevicePrinterInfo();
                                            App.Deferred.fulfill('FQ26E5TP76');
                                        }
                                    });
                                }
                                else {
                                    $('#UniversalDiv').empty();
                                    $('#UniversalDiv').append(isPrinterRolePageLoaded).trigger('create');
                                    Library.View.ShowDevicePrinterInfo();
                                }
                            }

                        }

                    }
                });
            },

            //Display Device Printer Setup List Page
            DevicePrinterSetup: function (s) {
                AjaxSend({
                    ExactUrl: '/DevicePrinter/PrinterSetup?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        $('#UniversalDiv').empty();
                        $('#UniversalDiv').append(vData).trigger('create');
                        App.Deferred.fulfill('FQ26E5TP76');
                        InitVal();
                    }
                });
            },

            //Company Wide Product List
            CompanyWideProductList: function () {
                $('#tblBody').empty().append('<tr><td style="text-align:center" colspan="20">Please wait ...</td></tr>');

                var pData = {};
                pData.SearchKeyword = '';

                AjaxSend({
                    url: 'Organization/getOrganizationProductList',
                    data: pData,
                    success: function (vData) {
                        var count = 1;
                        $('#tblBody').empty();
                        if (vData.OrgProductData.Rows.length > 0) {
                            for (var p = 0; p < vData.OrgProductData.Rows.length; p++) {
                                $('#tblBody').append('<tr>' +
                                    '<td>' + count + '.</td>' +
                                    '<td>' + vData.OrgProductData.Rows[p].ProductName + '</td>' +
                                    '<td>' + vData.OrgProductData.Rows[p].ProductNo + '</td>' +
                                    '<td>' + $.format.date(new Date(vData.OrgProductData.Rows[p].ActivationDate), 'MMM dd, yyyy') + '</td>' +
                                '</tr>');

                                count += 1;
                            }
                        }
                        else {
                            $('#tblBody').append('<tr><td colspan="4" style="text-align:center;">No Result(s) Found</td></tr>');
                        }
                        $('#tblBody').parent().table('refresh');
                    }
                });
            },

            //Search Company Wide Product List
            CompanyWideProductListSearch: function (s) {

                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'Organization/getOrganizationProductList',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.OrgProductData.Rows.length > 0) {
                            for (var p = 0; p < vData.OrgProductData.Rows.length; p++) {
                                $('#lstSearchFound').append('<li>' +
                                        '<a style="cursor:auto !important;">' +
                                            '<br><p><span style="font-weight:bold;">' + vData.OrgProductData.Rows[p].ProductName + '</span></p>' +
                                            '<br>' +
                                            '<p>Product No.: ' + vData.OrgProductData.Rows[p].ProductNo + '</p>' +
                                        '</a>' +
                                    '</li>');
                            }

                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Company Wide Product List Page
            CompanyWideProductListPage: function (s) {
                var pData = {};
                pData.OrgID = sessionStorage["OrgID"];

                window.pageSourcePage = $.mobile.activePage;
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                AjaxSend({
                    url: 'Organization/getSubscriberOrganization',
                    data: pData,
                    success: function (vDataOrg) {

                        if (vDataOrg.OrganizationID != "") {
                            pData.IsNew = 1;
                            pData.ID = vDataOrg.OrganizationID;

                            AjaxSend({
                                url: 'Organization/setCreateNew',
                                data: pData,
                                success: function (vData) {
                                    if (isCompanyWideProductPageLoaded == "") {
                                        AjaxSend({
                                            ExactUrl: '/Organization/OrganizationProduct?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                            type: 'GET',
                                            dataType: 'text',
                                            success: function (vData) {
                                                isCompanyWideProductPageLoaded = vData;
                                                $('#UniversalDiv').empty();
                                                $('#UniversalDiv').append(vData).trigger('create');
                                                if (sessionStorage.AddViewLibBCName == "Scan Barcode / Search Product") {
                                                    $('#pageBarcodeSearch').show();
                                                }
                                                else if (sessionStorage.AddViewLibBCName == "Subscribe to Company Product List") {
                                                    $('#pageBrowseByOrgOwner').show();
                                                    setTimeout(function () {
                                                        OrgProd.GetAllOrgOwners();
                                                    }, 1000)

                                                }
                                                else {
                                                    $('#pageOrgProductInfoNew').show();
                                                    //$('#pageOrgProductInfo').show();
                                                }
                                                App.Deferred.fulfill('FQ26E5TP76');
                                                isGroupNameClicked = true;
                                            }
                                        });
                                    }
                                    else {
                                        $('#UniversalDiv').empty();
                                        $('#UniversalDiv').append(isCompanyWideProductPageLoaded).trigger('create');
                                        setTimeout(function () {
                                            OrgProd.SetReqData();
                                        }, 1000)
                                        if (sessionStorage.AddViewLibBCName == "Scan Barcode / Search Product") {
                                            $('#pageBarcodeSearch').show();
                                        }
                                        else if (sessionStorage.AddViewLibBCName == "Subscribe to Company Product List") {
                                            $('#pageBrowseByOrgOwner').show();
                                            OrgProd.GetAllOrgOwners();
                                        }
                                        else {
                                            $('#pageOrgProductInfoNew').show();
                                            //$('#pageOrgProductInfo').show();
                                            setTimeout(function () {
                                                OrgProd.ShowProductInfo();
                                            }, 1000)
                                        }
                                    }
                                }
                            });
                        }
                    },
                    error: function (vError) {
                        MessageBox("This feature requires internet connection.",
                                   "Requires Internet Connection",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        return;
                    }
                });
            },

            //Fowst Product List
            FowstProductList: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isFOWSTProdListPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationFOWSTProduct?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isFOWSTProdListPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isFOWSTProdListPageLoaded).trigger('create');
                    OrgFowstProdList.GetOrgFowstProdList();
                }
            },

            //Search Company Wide Product List
            FowstProductListSearch: function (s) {

                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'Organization/getOrgFOWSTList',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.OrganizationFOWSTData.Rows.length > 0) {
                            for (var p = 0; p < vData.OrganizationFOWSTData.Rows.length; p++) {
                                $('#lstSearchFound').append('<li>' +
                                        '<a style="cursor:auto !important;">' +
                                            '<br><p><span style="font-weight:bold;">' + vData.OrganizationFOWSTData.Rows[p].Name + '</span></p>' +
                                        '</a>' +
                                    '</li>');
                            }
                        }
                        else {
                            $('#divNoResultsFound').show();
                        }
                        InitVal();
                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display ADD FOWST Product Page
            AddFowstSKUPage: function () {
                AjaxSend({
                    ExactUrl: '/Organization/OrganizationFOWSTProductList?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        $('#UniversalDiv').empty();
                        $('#UniversalDiv').append(vData).trigger('create');
                        InitVal();
                        setTimeout(function () {
                            if ($('#lstOrgFOWSTProductList li').length == 0) {
                                LibraryTable.ShowOrgFOWSTProductList();
                            }
                        }, 1500)
                        App.Deferred.fulfill('FQ26E5TP76');
                    }
                });
            },

            //Display Import Products By Excel File Page
            ImportByExcelFilePage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isPIMPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/PIMUpload?cache=1&OrgID=' + sessionStorage.OrgID,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isPIMPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            InitVal();
                            setTimeout(function () {
                                if ($("#OrgID").val() == "") {
                                    $("#OrgID").val(sessionStorage.OrgID);
                                }
                            }, 1500)
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isPIMPageLoaded).trigger('create');
                    $("#OrgID").val(sessionStorage.OrgID);
                }
            },

            //Display PIM WebForm
            PIMWebForm: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isPIMWebFormPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/PIMUpload/PIMWebForm?cache=1&OrgID=' + sessionStorage.OrgID,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isPIMWebFormPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');

                            setTimeout(function () {
                                if ($("#txtWebFormOrgID").val() == "") {
                                    $("#txtWebFormOrgID").val(sessionStorage.OrgID);
                                }
                            }, 1500)
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isPIMWebFormPageLoaded).trigger('create');
                    $("#txtWebFormOrgID").val(sessionStorage.OrgID);
                    PimWebForm.ClearFields();
                    PimWebForm.GetPackingSizeUnit();
                }
            },

            //Display Single Product Price Adjustment
            SingleProductPriceAdjustmentPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isSingleProdPricePageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/PIMProductPrice?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isSingleProdPricePageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isSingleProdPricePageLoaded).trigger('create');
                }
            },

            //Display Bulk Price Adjustment
            BulkPriceAdjustmentPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isBulkPricePageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/PIMProductPrice/UploadDownloadExcelFile?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isBulkPricePageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            setTimeout(function () {
                                UpDownExcelFile.SetReqData();
                            }, 1000)
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isBulkPricePageLoaded).trigger('create');
                    UpDownExcelFile.SetReqData();
                }

            },

            //Display Decimals in Quantity Settings Page
            DecimalsInQuantitySettingsPage: function (s) {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isDecimalsPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Decimals?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isDecimalsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isDecimalsPageLoaded).trigger('create');
                    Decimals.GetDecimalsData();
                }
            },

            //Display POS Settings Page
            POSSettingsPage: function (s) {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isPOSPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/POSSettings?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isPOSPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isPOSPageLoaded).trigger('create');
                    POS.GetPOSData();
                }
            },

            //Display Sales Order Settings Page
            SalesOrderSettingsPage: function (s) {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isSalesOrderPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SystemBehaviorSettings/SalesOrder?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isSalesOrderPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isSalesOrderPageLoaded).trigger('create');
                    SystemBehavior.GetSystemBehaviorData();
                }
            },

            //Display Receiving and Transfer Settings Page
            ReceivingAndTransferOutPage: function (s) {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isReceivingAndTransfersPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SystemBehaviorSettings/ReceivingAndTransferOut?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isReceivingAndTransfersPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isReceivingAndTransfersPageLoaded).trigger('create');
                    //SystemBehavior.GetSystemBehaviorData();
                }
            },

            //Display Inventory Count Settings Page
            InventoryCount: function (s) {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isInventoryCountPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SystemBehaviorSettings/InventoryCount?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isInventoryCountPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isInventoryCountPageLoaded).trigger('create');
                    //SystemBehavior.GetSystemBehaviorData();
                }
            },

            DailyBeginningBalance: function (s) {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isInventoryCountPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SystemBehaviorSettings/DailyBeginningBalance?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isInventoryCountPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            //InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isInventoryCountPageLoaded).trigger('create');
                    //SystemBehavior.GetSystemBehaviorData();
                }
            },

            PurchaseOrderSettings: function (s) {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isPurchaseOrderSettingsPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Scripts3/view/PurchaseOrderSettings.html',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isPurchaseOrderSettingsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            
                            require(['/Scripts3/controller/PurchaseOrderSettings.js?crc='], function() {
                                mPurchaseOrderSettings = new App.controller.PurchaseOrderSettings();
                                mPurchaseOrderSettings.Init();
                            });
                            //InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isPurchaseOrderSettingsPageLoaded).trigger('create');
                    mPurchaseOrderSettings.Init();
                    //SystemBehavior.GetSystemBehaviorData();
                }
            },

            //Display Purchase Order Settings Page
            PurchaseOrder: function (s) {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isPurchaseOrderPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/PurchaseOrder?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isPurchaseOrderPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isPurchaseOrderPageLoaded).trigger('create');
                    PurchaseOrder.GetPOData();
                }
            },

            //Hot Buttons
            HotButtons: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=hotbutton",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Name</b>' + data.Name;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Last Modified By</b>' + data.ModifiedBy;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return '<b class="ui-table-cell-label">Last Modified Date</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Hot Button";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isPrinterRoleClicked) {
                            LibraryTable.ViewHotButtonPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Hot Buttons
            HotButtonsSearch: function (s) {
                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'ProductGroup/GetPimMenu',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.PimMenuData.Rows.length > 0) {
                            for (var p = 0; p < vData.PimMenuData.Rows.length; p++) {
                                $('#lstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.PimMenuData.Rows[p].Name)
                                        )
                                        .data('data', vData.PimMenuData.Rows[p])
                                        .click(function (e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function (m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) { alert('This record already exists in your database. To add a new unique record, click on the link below.'); return };

                                            sessionStorage["AddViewLibBCName"] = "Add Hot Button";
                                            window.location.hash = 'UniversalPage';
                                            InitVal();
                                            LibraryTable.AddHotButtonPage();
                                        })
                                    ));
                            }
                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Hot Buttons Settings Page
            AddHotButtonPage: function () {
                //$.getScript("/Scripts/sources/ProductGroup/MenuList.js?crc=");
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                AjaxSend({
                    ExactUrl: '/ProductGroup?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        $('#UniversalDiv').empty();
                        $('#UniversalDiv').append(vData).trigger('create');
                        App.Deferred.fulfill('FQ26E5TP76');
                        InitVal();
                    }
                });
            },

            ViewHotButtonPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isViewHotButtonPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/ProductGroup/MenuTemplate?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isViewHotButtonPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isViewHotButtonPageLoaded).trigger('create');
                    ProductGroup.GetProductGroupData();
                }
            },

            //Group Buttons
            GroupSettings: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=groupsettings",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Name</b>' + data.GroupName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Description</b>' + data.Description;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Last Modified By</b>' + data.ModifiedBy;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return '<b class="ui-table-cell-label">Last Modified Date</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Group";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isPrinterRoleClicked) {
                            LibraryTable.ViewGroupPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Group Settings
            GroupSettingsSearch: function (s) {
                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'Franchisor/GetAllFranchisorGroup',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].GroupName)
                                        )
                                        .append($('<p>')
                                            .text('Description: ' + vData.Results.Rows[p].Description)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function (m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) { alert('This record already exists in your database. To add a new unique record, click on the link below.'); return };

                                            sessionStorage["AddViewLibBCName"] = "Add Group";
                                            window.location.hash = 'UniversalPage';
                                            InitVal();
                                            LibraryTable.AddGroupPage();
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Group Settings Page
            AddGroupPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isFranchisorPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Franchisor?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isFranchisorPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isFranchisorPageLoaded).trigger('create');
                    Franchisor.ShowFranchisorInfo();
                }
            },

            //Display Add Group Settings Page
            ViewGroupPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isFranchisorPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Franchisor?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isFranchisorPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isFranchisorPageLoaded).trigger('create');
                    Franchisor.ShowFranchisorInfo();
                }
            },

            //Customers List
            CustomerList: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=customer",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Name</b>' + data.CustomerName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.ActivationDate == null) {
                                    return "";
                                }
                                var date = data.ActivationDate;
                                return '<b class="ui-table-cell-label">Activation Date</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Status</b>' + (CBool(data.DeActivated) ? 'Inactive' : 'Active');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Banned</b>' + (CBool(data.IsBan) ? 'Yes' : 'No');
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Customer / Client";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.OrganizationID;
                        if (isCustomerClicked) {
                            LibraryTable.ViewCustomerPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Customers List
            CustomerListSearch: function (s) {

                var pData = {};
                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryOrganization',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            var mDate = new Date();
                            var Products = [];

                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].CompanyName)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {

                                            pData = {
                                                OrgCustomerID: '',
                                                CustomerID: $.trim($(this).data('data').ID),
                                                ActivationDate: (mDate.getMonth() + 1) + '/' + mDate.getDate() + '/' + mDate.getFullYear(),
                                                IsDeactivated: 0,
                                                DeactivationDate: '',
                                                DeactivationReason: '',
                                                Comments: '',
                                                IsBan: 0,
                                                Products: Products
                                            };

                                            SaveOrganizationCustomer(pData);
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Customers Page
            AddCustomerPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isAddCustomerPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationCustomer?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddCustomerPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isAddCustomerPageLoaded).trigger('create');
                    OrgCustomer.ShowCustomerReqData();
                }
            },

            //Display Add Customers Page
            ViewCustomerPage: function () {
                var pData = {};
                pData.IsNew = 0;
                pData.ID = sessionStorage.ViewID;
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                AjaxSend({
                    url: 'OrganizationShort/setCreateNew',
                    data: pData,
                    success: function (vData) {
                        isGroupNameClicked = true;
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
                            if (isViewCustomerPageLoaded == "") {
                                AjaxSend({
                                    ExactUrl: '/OrganizationShort/OrganizationSummary?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function (vData) {
                                        isViewCustomerPageLoaded = vData;
                                        $('#UniversalDiv').empty();
                                        $('#UniversalDiv').append(vData).trigger('create');
                                        App.Deferred.fulfill('FQ26E5TP76');
                                        InitVal();
                                    }
                                });
                            }
                            else {
                                $('#UniversalDiv').empty();
                                $('#UniversalDiv').append(isViewCustomerPageLoaded).trigger('create');
                                OrgShort.ShowOrgShortSummary();
                            }
                        }
                    }
                });
            },

            //Supplier List
            SupplierList: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=supplier",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Name</b>' + data.SupplierName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.ActivationDate == null) {
                                    return "";
                                }
                                var date = data.ActivationDate;
                                return '<b class="ui-table-cell-label">Activation Date</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Status</b>' + (CBool(data.DeActivated) ? 'Inactive' : 'Active');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Banned</b>' + (CBool(data.IsBan) ? 'Yes' : 'No');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Last Modified By</b>' + data.ModifiedBy;
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Supplier / Vendor";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        sessionStorage["txtOrgSupSupplierID"] = data.ID;
                        sessionStorage["btnOrgSupSupplier"] = data.SupplierName;
                        if (isSupplierClicked) {
                            LibraryTable.ViewSupplierPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Supplier List
            SupplierListSearch: function (s) {

                var pData = {};
                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryOrganization',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            var mDate = new Date();
                            var Products = [];

                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].CompanyName)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            sessionStorage["txtOrgSupSupplierID"] = $(this).data('data').ID;
                                            sessionStorage["btnOrgSupSupplier"] = $(this).data('data').CompanyName;
                                            sessionStorage["AddViewLibBCName"] = "Add Employee";

                                            window.location.hash = 'UniversalPage';
                                            InitVal();
                                            LibraryTable.AddSupplierPage();
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Supplier Page
            AddSupplierPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isSupplierPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationSupplier?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isSupplierPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');

                            $('#txtOrgSupSupplierID').val(sessionStorage.txtOrgSupSupplierID);
                            $('#btnOrgSupSupplier').text(sessionStorage.btnOrgSupSupplier).button('refresh');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isSupplierPageLoaded).trigger('create');
                    $('#txtOrgSupSupplierID').val(sessionStorage.txtOrgSupSupplierID);
                    $('#btnOrgSupSupplier').text(sessionStorage.btnOrgSupSupplier).button('refresh');
                    OrgSup.SetReqData();
                }
            },

            //Display View Supplier Page
            ViewSupplierPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isSupplierPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationSupplier?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isSupplierPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');

                            $('#txtOrgSupSupplierID').val(sessionStorage.txtOrgSupSupplierID);
                            $('#btnOrgSupSupplier').text(sessionStorage.btnOrgSupSupplier).button('refresh');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isSupplierPageLoaded).trigger('create');
                    $('#txtOrgSupSupplierID').val(sessionStorage.txtOrgSupSupplierID);
                    $('#btnOrgSupSupplier').text(sessionStorage.btnOrgSupSupplier).button('refresh');
                    OrgSup.ShowOrgSupplierInfo();
                }
            },

            //Sales Team
            SalesTeam: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=salesteam",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Name</b>' + data.TeamName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Description</b>' + data.Description;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return '<b class="ui-table-cell-label">Last Modified Date</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Team";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isSalesTeamClicked) {
                            LibraryTable.ViewCRMPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Customers List
            SalesTeamSearch: function (s) {

                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'crm/GetAllTeams',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {

                                $('#lstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].TeamName)
                                        )
                                //                                        .data('data', vData.Results.Rows[p])
                                //                                        .click(function (e) {
                                //                                            var id = $(this).data('data').ID;
                                //                                            var IsFOWSTExist = false;

                                //                                            $('#tblBody tr').each(function (m_index) {
                                //                                                if ($.trim($(this).attr('ID')) == id) {
                                //                                                    IsFOWSTExist = true;
                                //                                                }
                                //                                            });

                                //                                            if (IsFOWSTExist) { alert('This record already exists in your database. To add a new unique record, click on the link below.'); return };
                                //                                        })
                                    ));
                            }

                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            KnowledgeManagementAndQuestionaire: function (s) {

                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'KnowledgeManagementAndQuestionaire/GetAllKnowledgeManagementAndQuestionaire',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {

                                $('#lstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].CaseNo)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].CaseDescription)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Solution)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Topic)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Product)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Language)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].EditedBy)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].LastUpdated)
                                        )
                                    ));
                            }

                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },
            //Display Add CRM Page
            AddCRMPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isAddCRMPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/CRM?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddCRMPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isAddCRMPageLoaded).trigger('create');
                }
            },

            //Display Add CRM Page
            ViewCRMPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isAddCRMPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/CRM?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddCRMPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isAddCRMPageLoaded).trigger('create');
                    CRM.ShowCRMData();
                }
            },

            ShowOrgFOWSTProductList: function () {
                var pData = {};
                var pParams = GetUrlParameters();

                pData.FowstID = pParams.OrgFowstID;
                pData.SearchKeyword = $("#txtOrgFowstProductSearch").val();

                AjaxSend({
                    url: "Organization/getOrganizationFowstProductList",
                    data: pData,
                    success: function (vData) {
                        LoadOrgFOWSTProductList(vData.OrgFowstProductData.Rows);
                    }
                });
            },

            PageItemModifiers: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isAddItemModifiersPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/ItemModifiers?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddItemModifiersPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            //$('#popItemModifierName').popup('open');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isAddItemModifiersPageLoaded).trigger('create');
                    ItemModifier.DisplayItemModifierData();
                }
            },

            SearchItemModifiers: function (vKeywords) {
                if (vKeywords != "") {
                    tblSysAdminDataTable.search($.trim(vKeywords)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=modifier",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Item Modifier Name</b>' + data.Name;
                            }
                        },                        
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Product Categories Applied To</b>' + '';
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Modified By</b>' + data.UpdatedByName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return '<b class="ui-table-cell-label">Modified Date</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["ItemModifierID"] = data.ID;
                        sessionStorage["ItemModifierName"] = data.Name;
                        sessionStorage["AddViewLibBCName"] = "Item Modifier Parameters";
                        window.location.hash = 'UniversalPage';
                        if (isItemModifierClicked == true) {
                            LibraryTable.PageItemModifiers();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
                return;
                App.model.ItemModifiers.Search({
                    Keywords: vKeywords,
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {
                            var pTable = CreateTable(vData.Results, {
                                Element: $('#tblBody').parent(),
                                attr: {
                                    'class': 'ui-table ui-responsive table-stroke table-stripe ui-table-reflow',
                                    'data-column-btn-theme': 'b'
                                },
                                theadAttr: {
                                    'class': 'custom-theme-f'
                                },
                                AddAllColumns: false,
                                Columns: {
                                    Name: {
                                        Text: 'Item Modifier Name'
                                    },

                                    ProductCategories: {
                                        Text: 'Product Categories Applied To'
                                    },
                                    UpdatedByName: {
                                        Text: 'Modified By'
                                    },
                                    LastUpdated: {
                                        Text: 'Modified Date',
                                        DataType: 'date',
                                        Format: proj.DateFormat
                                    }
                                },
                                AfterRow: function (e) {
                                    e.Row.css('cursor', 'pointer');
                                    e.Row.click(function (e1) {
                                        var pDR = $($(e1.target).closest('tr')).data('data');
                                        sessionStorage["ItemModifierID"] = pDR.ID;
                                        sessionStorage["ItemModifierName"] = pDR.Name;
                                        sessionStorage["AddViewLibBCName"] = "Item Modifier Parameters";
                                        window.location.hash = 'UniversalPage';
                                        if (isItemModifierClicked == true) {
                                            LibraryTable.PageItemModifiers();
                                        }
                                        InitVal();
                                    });
                                }
                            });
                            pTable.table();
                            pTable.trigger('create');

                            var table = $("table").stupidtable({

                                "date": function (a, b) {
                                    // Get these into date objects for comparison.

                                    aDate = new Date(a);
                                    bDate = new Date(b);

                                    return aDate - bDate;
                                }
                            });
                        }
                        else {
                            $('#tblBody').append($('<tr>')
                                                    .append($('<td>')
                                                        .css('text-align', 'center')
                                                        .attr('colspan', '6')
                                                        .text('No Result(s) Found')
                                                    )
                            );
                        }
                        $('#tblBody').parent().table('refresh');
                    } // END success: function(vData) {
                }); // END App.model.ItemModifiers.Search({
            }, // END SearchItemModifiers: function(vKeywords) {

            AddItemModifier: function () {
                var pName = $('#txtItemModifierNewName').val();

                if (pName.trim() == '') {
                    $('#divErrorItemModifier').show();
                    return false;
                }

                $('#divErrorItemModifier').hide();

                mModel.Add({
                    Name: pName,
                    success: function (vData) {
                        //MessageBox('Item modifier was successfully created.');
                        $('#popItemModifierName').popup('close');
                        $('#txtItemModifierNewName').val('');
                        LibraryTable.SearchItemModifiers();
                    },
                    error: function (vData) {
                        if (typeof vData == 'string')
                            MessageBox(vData);
                        else
                            MessageBox(JSON.stringify(vData));
                    }
                });
            },

            SearchKnowledgeManagementAndQuestionaire: function (vKeywords) {

                var pData = {};
                pData.SearchKeyword = vKeywords;

                AjaxSend({
                    url: 'KnowledgeManagementAndQuestionaire/GetAllKnowledgeManagementAndQuestionaire',
                    data: pData,
                    success: function (vData) {

                        if (vData.Results.Rows.length > 0) {
                            var pTable = CreateTable(vData.Results, {
                                Element: $('#tblBody').parent(),
                                attr: {
                                    'class': 'ui-table ui-responsive table-stroke table-stripe ui-table-reflow',
                                    'data-column-btn-theme': 'b'
                                },
                                theadAttr: {
                                    'class': 'custom-theme-f'
                                },
                                AddAllColumns: false,
                                Columns: {
                                    CaseNo: {
                                        Text: 'Case ID'
                                    },
                                    CaseDescription: {
                                        Text: 'Case'
                                    },
                                    Solution: {
                                        Text: 'Solution'
                                    },
                                    Topic: {
                                        Text: 'Topic'
                                    },
                                    Product: {
                                        Text: 'Product'
                                    },
                                    Language: {
                                        Text: 'Language'
                                    },
                                    EditedBy: {
                                        Text: 'Edited By'
                                    },
                                    LastUpdated: {
                                        Text: 'Last Edit Date',
                                        DataType: 'date',
                                        Format: proj.DateFormat
                                    }
                                },
                                AfterRow: function (e) {
                                    e.Row.css('cursor', 'pointer');
                                    e.Row.click(function (e1) {
                                        var pDR = $($(e1.target).closest('tr')).data('data');
                                        sessionStorage["ItemModifierID"] = pDR.ID;
                                        sessionStorage["ItemModifierName"] = pDR.Name;
                                        sessionStorage["AddViewLibBCName"] = "Item Modifier Parameters";
                                        window.location.hash = 'UniversalPage';
                                        if (isItemModifierClicked == true) {
                                            LibraryTable.PageItemModifiers();
                                        }
                                        InitVal();
                                    });
                                }
                            });
                            pTable.table();
                            pTable.trigger('create');

                            var table = $("table").stupidtable({

                                "date": function (a, b) {
                                    // Get these into date objects for comparison.

                                    aDate = new Date(a);
                                    bDate = new Date(b);

                                    return aDate - bDate;
                                }
                            });
                        }
                        else {
                            $('#tblBody').append($('<tr>')
                                                    .append($('<td>')
                                                        .css('text-align', 'center')
                                                        .attr('colspan', '8')
                                                        .text('No Result(s) Found')
                                                    )
                            );

                            $('#divNoResultsFound').show();

                        }

                    }
                });


            }, // END SearchItemModifiers: function(vKeywords) {

            GetKnowledgeManagementNumber: function () {
                AjaxSend({
                    url: "KnowledgeManagementAndQuestionaire/GetLastKMNumber",
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {
                            $('#txtCaseID').val(vData.Results.Rows[0].LastNumber);
                        } else {
                            $('#txtCaseID').val(0);
                        }
                    }
                });
            },
            //Display Loyalty Program Page
            LoyaltyProgram: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isLoyaltyProgramPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SystemBehaviorSettings/LoyaltyProgram?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isLoyaltyProgramPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');                            
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isLoyaltyProgramPageLoaded).trigger('create');
                    LoyaltyProgram.GetSystemBehavior();
                }
            },

            //Printers
            Printers: function (s) {
                console.log('Printers');
                
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + '&mod=printers',
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Brand</b>' + data.Brand;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                var p;

                                if (CStr2(data.Brand) + CStr2(data.Model) == '')
                                    p = data.PrinterType;
                                else
                                    p = data.Model;
                                return '<b class="ui-table-cell-label">Model</b>' + p;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Printer Name</b>' + data.PrinterName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Serial Number</b>' + data.SerialNum;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Interface Type</b>' + data.ConnectionType;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Address</b>' + data.PrinterIPAddress;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">FOWST</b>' + data.FowstName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">List of devices that use this as default printer</b>' + data.DeviceNames;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Printer Role</b>' + data.PrinterRole;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Categories</b>'; // + '<button>View</button>';
                            }
                        }
                        //{
                        //    "data": function (data, type, full, meta) {
                        //        if (data.LastUpdated == null) {
                        //            return "";
                        //        }
                        //        var date = data.LastUpdated;
                        //        return '<b class="ui-table-cell-label">Last Modified Date</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                        //    }
                        //}
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Printer";
                        sessionStorage['ViewID'] = data.ID;
                        sessionStorage["FowstTitle"] = data.Name;

                        if (isPrintersClicked) {
                            LibraryTable.AddPrinterPage();
                        }
                        window.location.hash = 'UniversalPage';
                        InitVal();
                    },
                    disablecols: [0, 2] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            AddPrinterPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isPrinterPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Scripts3/view/Printer.html',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isPrinterPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData).trigger('create');
                            //Fowst.ShowFowstInfo(sessionStorage.ViewID || '');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                            require(['/Scripts3/controller/Printer.js?crc='], function() {
                                mPrinters = new App.controller.Printer();
                                mPrinters.Init();
                                mPrinters.LoadData({ID:sessionStorage['ViewID']});
                            });
                        }
                    });
                }
                else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isPrinterPageLoaded).trigger('create');
                    mPrinters.Init();
                    mPrinters.LoadData({ID:sessionStorage['ViewID']});
                }
            },

            //Printers
            TableView: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + '&mod=tableview',
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            }, width: "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Table View</b>' + data.Name;
                            }, width: "200px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Locations</b>' + data.Locations;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Modified By</b>' + data.ModifiedBy;
                            }, width: "100px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return '<b class="ui-table-cell-label">Last Modified</b>' + $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }, width: "100px"
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Template";
                        sessionStorage['ViewID'] = data.ID;

                        if (isTableViewClicked) {
                            LibraryTable.AddTableViewPage();
                        }
                        window.location.hash = 'UniversalPage';
                        InitVal();
                    },
                    disablecols: [0, 2] //if all '_all'
                }

                SysAdminDataTable(pData);
            },
            AddTableViewPage: function () {
                $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');
                if (isTableViewPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/TableView/Settings?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isTableViewPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData).trigger('create');
                            //Fowst.ShowFowstInfo(sessionStorage.ViewID || '');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isTableViewClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isTableViewPageLoaded).trigger('create');
                    TableView.LoadData();
                }
            }

        } // END window.LibraryTable = {

        function GetEmployeeStatus(data) {
            var IsConfirmed = CStr2(data.IsConfirmed);
            var IsTerminated = CStr2(data.IsTerminated);
            var DeActivated = CBool(data.DeActivated);
            var DeactivationDate = data.DeactivationDate;
            var status = "";

            if (IsConfirmed != "" && IsTerminated != "") {
                if (CBool(IsConfirmed) == false && CBool(IsTerminated) == false) {
                    status = 'Pending Confirmation';
                }
                else if (CBool(IsConfirmed) == true && CBool(IsTerminated) == false) {
                    status = 'Active';
                }
                else if (CBool(IsConfirmed) == true && CBool(IsTerminated) == true) {
                    status = 'Terminated';
                }
                else if (CBool(IsConfirmed) == false && CBool(IsTerminated) == true) {
                    status = (DeActivated != false ? 'Deactivated ( ' + $.format.date(new Date(parseInt(DeactivationDate.substr(6))), 'MMM dd, yyyy') + ' )' : 'Active');
                }
            }
            else if (CBool(IsConfirmed) == undefined && CBool(IsTerminated) == undefined) {
                status = (DeActivated != false ? 'Deactivated ( ' + $.format.date(new Date(parseInt(DeactivationDate.substr(6))), 'MMM dd, yyyy') + ' )' : 'Active');
            }
            return status;
        }

        function SysAdminDataTable(vData) {
            $('#tblBodyDT').empty()
            
            if ($.fn.DataTable.isDataTable("#tblSysAdminDataTable")) {
                //check if datatable is created 
                $('#tblSysAdminDataTable').DataTable().clear().destroy();
            }

            tblSysAdminDataTable = $('#tblSysAdminDataTable').DataTable({
                retrieve: true,
                "bdestroy": true,
                "pageLength": 50,
                "order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": vData.ajax,
                "autoWidth": false,
                "columns": vData.columns,
                "aoColumnDefs": [{ "bSortable": false, "aTargets": vData.disablecols }],
                "responsive": true,
                "columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
                //"stateSave": true
            })

            $('#tblSysAdminDataTable tbody').unbind('click').on('click', 'tr', function () {
                vData.click(tblSysAdminDataTable.row(this).data());
            });            
        }

        function Reposition() {
            $('div[data-role="popup"]').popup().popup("reposition", { positionTo: 'window' });
            $('div[data-role="popup"]').find('input[type="text"]:first').focus();
        }

        function GetAllFowst() {
            AjaxSend({
                url: 'Browser/LibraryFowst',
                success: function (vData) {
                    DisplayFOWST(vData.Results.Rows);
                }
            });
        }

        function DisplayFOWST(vRows) {
            var pLV = $("#lstOrgEmployeeFowst");

            pLV.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {
                //unit measure
                pItem = $('<li>')
                    .attr('ID', vRows[i].ID)
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .data('data', vRows[i])
                        .click(function (e) {
                            //                            window.location = MergeUrlParameters({
                            //                                OrgID: sessionStorage.OrgID,
                            //                                IsMobile: CBool(IsMobile) ? 1 : 0,
                            //                                EmployeeName: $('#btnOrgEmployee').text(),
                            //                                EmployeeID: ID,
                            //                                FOWSTID: $(this).attr('ID'),
                            //                                FOWSTName: $(this).text()
                            //                            }, 'OrganizationEmployeeAssignWorkSchedule');
                        })
                        .append($('<h3>')
                            .text(vRows[i].FowstName)
                        )
                        .append($('<p>')
                            .text(vRows[i].Address)
                        )
                    )
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .click(function () {
                            $('#' + $(this).attr('ID')).remove();
                        })
                    )
                pLV.append(pItem)
            }

            pLV.listview('refresh');
        }

        function getCallingCode() {

            AjaxSend({
                url: 'SignUp/getCallingCodeList',
                success: function (vData) {

                    var vRows = vData.CallingCodeData.Rows;

                    $('#selCallingCodeMobile').empty();
                    $('#selCallingCodeTel').empty();

                    $('#selCallingCodeMobile').append(new Option('Please Select Country Code', ''));
                    $('#selCallingCodeTel').append(new Option('Please Select Country Code', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selCallingCodeMobile').append(new Option((vRows[p].CallingCode == undefined ? '' : vRows[p].CallingCode) + ' - ' + vRows[p].Name, vRows[p].ID, false, false));
                        $('#selCallingCodeTel').append(new Option((vRows[p].CallingCode == undefined ? '' : vRows[p].CallingCode) + ' - ' + vRows[p].Name, vRows[p].ID, false, false));
                    }

                    $("#selCallingCodeMobile option:contains('63 - Philippines')").attr('selected', 'selected');
                    $("#selCallingCodeTel option:contains('63 - Philippines')").attr('selected', 'selected');

                    getTelcoPrefix();
                    getAreaCode();
                    $('#spanCountryCodeMobile').hide();
                    $('#selCallingCodeMobile').removeClass('form-validation-error');

                    $('#spanCountryCodeTelNo').hide();
                    $('#selCallingCodeTel').removeClass('form-validation-error');


                },
                error: function (vError) {
                    var pDummy;
                }
            });

        }

        function getAreaCode() {

            AjaxSend({
                url: 'SignUp/getAreaCodeList',
                success: function (vData) {

                    var vRows = vData.AreaCodeData.Rows;

                    $('#selAreaCode').empty();

                    $('#selAreaCode').append(new Option('Please Select Area Code', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selAreaCode').append(new Option(vRows[p].CallingCode + ' - ' + vRows[p].Area, vRows[p].ID, false, false));
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });

        }

        function getTelcoPrefix() {

            AjaxSend({
                url: 'SignUp/getTelcoPrefixList',
                success: function (vData) {

                    var vRows = vData.TelcoPrefixData.Rows;

                    $('#selTelcoPrefix').empty();

                    $('#selTelcoPrefix').append(new Option('Please Select Mobile Number Prefix', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selTelcoPrefix').append(new Option(vRows[p].Prefix + ' - ' + vRows[p].Name, vRows[p].ID, false, false));
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });

        }

        function ClearSecurityRoleFields() {

            $("#txtSecurityRoleID").val('');
            $("#txtRoleCode").val('');
            $("#txtRole").val('');
            $("#txtRoleDescription").val('');
            $('#chkRoleIsActive').attr("checked", true).checkboxradio("refresh");
        }

        function ShowSecurityRoleInfo(strID) {
            var pLV;
            var pData = {};
            pData.SecurityRoleID = strID;

            ClearSecurityRoleFields();

            AjaxSend({
                url: "SecurityRole/getSecurityRoleInfo",
                data: pData,
                success: function (vData) {

                    if (vData.SecurityRoleData.Rows.length > 0) {

                        $("#txtSecurityRoleID").val(vData.SecurityRoleData.Rows[0].ID || '');
                        $("#txtRoleCode").val(vData.SecurityRoleData.Rows[0].Code || '');
                        $("#txtRole").val(vData.SecurityRoleData.Rows[0].Role || '');
                        $("#txtRoleDescription").val(vData.SecurityRoleData.Rows[0].Description || '');
                        $('#chkRoleIsActive').attr("checked", (vData.SecurityRoleData.Rows[0].IsActive == 1 ? true : false)).checkboxradio("refresh");

                    }

                    //Load POS
                    pData.MainModule = "POS";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModulePOS) {

                            $('#lstPOS').html('').listview('refresh');
                            pLV = $('#lstPOS');
                            LoadSecurityRoleModule(vDataRoleModulePOS.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    //Load Receiving and Transfers
                    pData.MainModule = "Receiving and Transfers";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleSFA) {

                            $('#lstReceivingAndTransfers').html('').listview('refresh');
                            pLV = $('#lstReceivingAndTransfers');
                            LoadSecurityRoleModule(vDataRoleModuleSFA.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    //Load Inventory count
                    pData.MainModule = "Inventory Count";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleSFA) {

                            $('#lstInvCount').html('').listview('refresh');
                            pLV = $('#lstInvCount');
                            LoadSecurityRoleModule(vDataRoleModuleSFA.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    //Load SFA
                    pData.MainModule = "SFA";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleSFA) {

                            $('#lstSFA').html('').listview('refresh');
                            pLV = $('#lstSFA');
                            LoadSecurityRoleModule(vDataRoleModuleSFA.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    //Load CRM
                    pData.MainModule = "CRM";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleCRM) {

                            $('#lstCRM').html('').listview('refresh');
                            pLV = $('#lstCRM');
                            LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    //Load Dashboard and Reports
                    pData.MainModule = "Dashboard and Reports";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleDAR) {

                            $('#lstDashboardsAndReports').html('').listview('refresh');
                            pLV = $('#lstDashboardsAndReports');
                            LoadSecurityRoleModule(vDataRoleModuleDAR.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    pData.MainModule = "Requests and Approvals";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleDAR) {

                            $('#lstDashboardsAndReports').html('').listview('refresh');
                            pLV = $('#lstDashboardsAndReports');
                            LoadSecurityRoleModule(vDataRoleModuleDAR.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    pData.MainModule = "Sales Returns";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleDAR) {
                            $('#lstSalesReturns').html('').listview('refresh');
                            pLV = $('#lstSalesReturns');
                            LoadSecurityRoleModule(vDataRoleModuleDAR.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    //Load Settings
                    pData.MainModule = "Settings";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleSettings) {

                            $('#lstSettings').html('').listview('refresh');
                            pLV = $('#lstSettings');
                            LoadSecurityRoleModule(vDataRoleModuleSettings.SecurityRoleData.Rows, pLV, 0, strID, true);

                        }
                    });


                    //Load MDM
                    pData.MainModule = "MDM";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleMDM) {

                            $('#lstMDM').html('').listview('refresh');
                            pLV = $('#lstMDM');
                            LoadSecurityRoleModule(vDataRoleModuleMDM.SecurityRoleData.Rows, pLV, 0, strID, true);

                        }
                    });



                }
            });

        };

        function LoadSecurityRoleModule(vRows, pLV, intCurIndex, strRoleID, isClear, vIndentation) {

            var pItem;
            var strindentation = (vIndentation == undefined ? "" : vIndentation);

            if (intCurIndex > vRows.length - 1) {
                return;
            }

            if (isClear) {
                pLV.html('');
            }

            pItem = $('<fieldset class="ui-grid-a">')
                        .append($('<div class="ui-block-a" style="height:38px; width: 70%;">')
                            .append($('<label>')
                                .attr('id', "lbl" + vRows[intCurIndex].ModuleID)
                                .data('Data', vRows[intCurIndex])
                                .html(strindentation + vRows[intCurIndex].ModuleName)
                            )
                        );

            if (vRows[intCurIndex].IsHeader == 0) {
                pItem = pItem
                            .append($('<div class="ui-block-b" style="height:38px; width: 30%;">')
                            .append($('<fieldset data-role="controlgroup" data-type="horizontal" data-mini="true" style="float:right; margin-top:0px;">')
                                .append($('<input type="radio">')
                                    .attr('id', "rdoYes" + vRows[intCurIndex].ModuleID)
                                    .attr('name', "rdo" + vRows[intCurIndex].ModuleID)
                                    .attr('checked', (vRows[intCurIndex].IsHasAccess == 1 ? true : false))
                                )
                                .append($('<label>')
                                    .attr('for', "rdoYes" + vRows[intCurIndex].ModuleID)
                                    .text('Yes')
                                )
                                .append($('<input type="radio">')
                                    .attr('id', "rdoNo" + vRows[intCurIndex].ModuleID)
                                    .attr('name', "rdo" + vRows[intCurIndex].ModuleID)
                                    .attr('checked', (vRows[intCurIndex].IsHasAccess == 0 ? true : false))
                                )
                                .append($('<label>')
                                    .attr('for', "rdoNo" + vRows[intCurIndex].ModuleID)
                                    .text('No')
                                )
                            )
                        );

                pItem = $('<li>')
                    .append(pItem)

                pLV.append(pItem);
                pLV.trigger("create");
                pLV.listview('refresh');

                LoadSecurityRoleModule(vRows, pLV, intCurIndex + 1, strRoleID, false, strindentation);

            } else {

                pItem = $('<li>')
                                .append(pItem)

                pLV.append(pItem);
                pLV.trigger("create");
                pLV.listview('refresh');

                var pDataChild = {};
                pDataChild.SecurityRoleID = strRoleID;
                pDataChild.SecurityModuleHeaderID = vRows[intCurIndex].ModuleID;

                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModulesChild",
                    data: pDataChild,
                    success: function (vDataRoleModuleChild) {

                        if (vDataRoleModuleChild.SecurityRoleData.Rows.length > 0) {
                            LoadSecurityRoleModule(vDataRoleModuleChild.SecurityRoleData.Rows, pLV, 0, strRoleID, false, strindentation + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                        }

                        LoadSecurityRoleModule(vRows, pLV, intCurIndex + 1, strRoleID, false, strindentation);
                    }
                });
            }

        };

        function GetDecimalSettings() {
            AjaxSend({
                url: 'Decimals/GetDecimalSettings',
                data: { OrgID: sessionStorage.OrgID },
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        DisplayGetDecimalSettings(vData.Results.Rows);
                    }
                }
            });
        }

        function DisplayGetDecimalSettings(vRows) {
            ID = vRows[0].ID;

            if (parseInt(vRows[0].IsAllowDecimal) == 1) {

                IsAllowDecimal = 1;

                $('#selSlider').val('optAllowYes');
                $('#dvIsAllowDecimal').css('visibility', 'visible');
                $('#dvIsAllowDecimal').css('display', 'block');

                if (parseInt(vRows[0].AllowDecimalMode) == 0) {
                    $("#rcAllowAllDecimals").prop("checked", true).checkboxradio("refresh");
                    $("#rcAllowOnSelected").prop("unchecked", true).checkboxradio("refresh");

                    $('#dvIsAllowOnSelectedCategories').css('visibility', 'hidden');
                    $('#dvIsAllowOnSelectedCategories').css('display', 'none');

                    DecimalMode = 0;
                }
                else {
                    $("#rcAllowOnSelected").prop("checked", true).checkboxradio("refresh");
                    $("#rcAllowAllDecimals").prop("unchecked", true).checkboxradio("refresh");

                    $('#dvIsAllowOnSelectedCategories').css('visibility', 'visible');
                    $('#dvIsAllowOnSelectedCategories').css('display', 'block');

                    DecimalMode = 1;
                }
            }
            else {

                IsAllowDecimal = 0;

                $('#selSlider').val('optAllowNo');
                $('#dvIsAllowDecimal').css('visibility', 'hidden');
                $('#dvIsAllowDecimal').css('display', 'none');
            }

            try {
                $('#selSlider').slider('refresh');
            }
            catch (e) {
                $('#selSlider').slider();
            }
        }

        function GetAllFeatures() {
            AjaxSend({
                url: 'Decimals/GetAllProductFeatures',
                data: { OrgID: sessionStorage.OrgID },
                success: function (vData) {
                    DisplayProductFeatures(vData.Results.Rows);
                }
            });
        }

        function DisplayProductFeatures(vRows) {
            var pLV = $('#tbCategories');
            var IsSelected = false;

            for (i = 0; i <= vRows.length - 1; i++) {
                IsSelected = false;

                var IsAllow = vRows[i].IsAllowDecimal;

                pItem = $('<tr>')
                            .append($('<td>')
                                .text(vRows[i].Label)
                            )
                            .append($('<td>')
                                .css('text-align', 'center')
                                .append($('<select>')
                                    .attr('ID', i)
                                    .attr('data', vRows[i].Label)
                                    .attr('data-role', 'slider')
                                    .append($('<option>')
                                        .attr('value', 'No')
                                        .text('No')
                                    )
                                    .append($('<option>')
                                        .attr('value', 'Yes')
                                        .text('Yes')
                                    )
                                )
                            )


                pLV.append(pItem);

                try {
                    if (IsAllow == 1) {
                        $('#' + i).val('Yes').slider('refresh');
                    }
                    else {
                        $('#' + i).slider('refresh');
                    }

                }
                catch (e) {
                    $('#' + i).slider();
                }
            }

            try {
                $('#lstCategories').listview('refresh');
            }
            catch (e) {
                $('#lstCategories').listview();
            }
        }

        function SaveOrganizationCustomer(pData2) {
            var pData = [];
            var Products = [];

            pData = pData2;

            AjaxSend({
                url: "Organization/CheckOrganizationCustomer",
                data: pData,
                success: function (vData) {

                    if (vData.OrgCustomerData.Rows.length > 0) {
                        $('#panAddLibrary').popup('close');
                        $('#TitleOrgEmp').text('Customer Exist');
                        $('#MsgOrgEmp').text('Customer being added already exist.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                            $('#panAddLibrary').popup('open');
                        })
                    }
                    else {
                        AjaxSend({
                            url: "Organization/SaveOrganizationCustomer",
                            data: pData,
                            success: function (vSaveData) {

                                if (vSaveData.OrgCustomerData == "ActivationDateError") {
                                    $('#panAddLibrary').popup('close');
                                    $('#TitleOrgEmp').text('Activation Date');
                                    $('#MsgOrgEmp').text('Please enter valid activation date.');
                                    $('#panOrgEmpMessage').popup('open');
                                    $('#btnOKOrgEmp').click(function (e) {
                                        e.preventDefault();
                                        $('#panOrgEmpMessage').popup('close');
                                        $('#panAddLibrary').popup('open');
                                    })
                                }
                                else if (vSaveData.OrgCustomerData == "DeactivationDateError") {
                                    $('#panAddLibrary').popup('close');
                                    $('#TitleOrgEmp').text('Deactivation Date');
                                    $('#MsgOrgEmp').text('Please enter valid deactivation date.');
                                    $('#panOrgEmpMessage').popup('open');
                                    $('#btnOKOrgEmp').click(function (e) {
                                        e.preventDefault();
                                        $('#panOrgEmpMessage').popup('close');
                                        $('#panAddLibrary').popup('open');
                                    })
                                }
                                else {
                                    LibraryTable.CustomerList();
                                    $('#panAddLibrary').popup('close');
                                }

                            }
                        });

                    }

                }
            });
        };

        function getOrgAddTradeName() {

            AjaxSend({
                url: 'Organization/getOrgTradeNameList',
                success: function (vData) {

                    var vRows = vData.OrganizationData.Rows;

                    $('#selOrgFOWSTTradeName').empty();

                    $('#selOrgFOWSTTradeName').append(new Option('Please Select', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selOrgFOWSTTradeName').append(new Option(vRows[p].BusinessTradeName, vRows[p].ID, false, false));
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });

        }

        function getOrgAddFowstType() {

            AjaxSend({
                url: 'Browser/LibraryFowstType',
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    $('#selOrgFOWSTType').empty();

                    $('#selOrgFOWSTType').append(new Option('Please Select', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selOrgFOWSTType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

        function getOrgAddWarehouseType() {
            AjaxSend({
                url: 'Browser/LibraryFOWSTWarehouseType',
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    $('#selOrgFOWSTWarehouseType').empty();

                    $('#selOrgFOWSTWarehouseType').append(new Option('Please Select', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selOrgFOWSTWarehouseType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

        function getOrgAddRetailStoreType() {
            AjaxSend({
                url: 'Browser/LibraryFOWSTRetailStoreType',
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    $('#selOrgFOWSTRetailStoreType').empty();

                    $('#selOrgFOWSTRetailStoreType').append(new Option('Please Select', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selOrgFOWSTRetailStoreType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

        function GetAllCategories(printerID) {
            data = { OrgID: sessionStorage.OrgID };

            AjaxSend({
                url: 'DevicePrinter/GetAllCategories',
                data: data,
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {

                        if (printerID != '') {
                            AjaxSend({
                                url: 'DevicePrinter/GetDevicePrinterCategories',
                                data: { PrinterID: printerID },
                                success: function (vDataCat) {

                                    if (vDataCat.Results.Rows.length > 0) {

                                        DisplayAllCategories(vData.Results.Rows, vDataCat.Results.Rows, printerID);
                                    } else {
                                        DisplayAllCategories(vData.Results.Rows, '', printerID);
                                    }
                                }
                            });
                        } else {
                            DisplayAllCategories(vData.Results.Rows, '', '');
                        }

                    }
                }
            });
        }

        var arrCategory = [];

        function DisplayAllCategories(vRows, vRowsCat, printerID) {
            var pLV = $("#tblCategories");

            $('#tblCategories').empty();

            var m_Ctr = 0;
            var categories = [];

            if (vRowsCat != '') {
                for (var j = 0; j <= vRowsCat.length - 1; j++) {
                    categories[j] = vRowsCat[j].CategoryName;
                }

            }


            for (var i = 0; i <= vRows.length - 1; i++) {

                var checked = undefined;

                if ((i % 2) == 0) {
                    pItem = $('<tr>')
                }

                if (categories.indexOf(vRows[i].Category) >= 0) {
                    checked = true;
                    pItem = pItem
                        .append($('<td>')
                            .append($('<input type="checkbox" />')
                                .attr('ID', i + 'chb')
                                .attr('checked', checked)
                                .attr('data-mini', 'true')
                                .data('data', vRows[i].Category)
                                .click(function () {
                                    if ($(this).is(':checked')) {
                                        AjaxSend({
                                            url: 'DevicePrinter/IsCategoryExist',
                                            data: { PrinterID: printerID, CategoryName: $(this).data('data') },
                                            success: function (vDataCat) {

                                                if (vDataCat.Results.Rows.length > 0) {
                                                    MessageBox('This category already belongs to a different printer role. Saving this will overwrite the previous setting.', 'Printer Role');

                                                }
                                            }
                                        });
                                    }
                                })

                            )
                            .append($('<label>')
                                .attr('for', i + 'chb')
                                .text(vRows[i].Category)
                            )
                        )//td                    


                    pLV.append(pItem);

                    try {
                        $('#' + i + 'chb').checkboxradio('refresh');
                    } catch (ex) {
                        $('#' + i + 'chb').checkboxradio();
                    }
                } else {
                    pItem = pItem
                        .append($('<td>')
                            .append($('<input type="checkbox" />')
                                .attr('ID', i + 'chb')
                                .attr('checked', checked)
                                .attr('data-mini', 'true')
                                .data('data', vRows[i].Category)
                                .click(function () {
                                    if ($(this).is(':checked')) {
                                        AjaxSend({
                                            url: 'DevicePrinter/IsCategoryExist',
                                            data: { PrinterID: printerID, CategoryName: $(this).data('data') },
                                            success: function (vDataCat) {

                                                if (vDataCat.Results.Rows.length > 0) {
                                                    MessageBox('This category already belongs to a different printer role. Saving this will overwrite the previous setting.', 'Printer Role');

                                                }
                                            }
                                        });
                                    }
                                })

                            )
                            .append($('<label>')
                                .attr('for', i + 'chb')
                                .text(vRows[i].Category)
                            )
                        )//td                    


                    pLV.append(pItem);

                    try {
                        $('#' + i + 'chb').checkboxradio('refresh');
                    } catch (ex) {
                        $('#' + i + 'chb').checkboxradio();
                    }
                }

            } //for

            try {
                $('#lstCategoriesContainer').listview('refresh');
            }
            catch (e) {
                $('#lstCategoriesContainer').listview();
            }
        }

        function LoadOrgFOWSTProductList(vRows) {
            var pLV = $('#lstOrgFOWSTProductList');
            var pItem;

            $("#divOrgFOWSTProductRecordCount").text(" Number of record(s) found: " + vRows.length);

            pLV.html('');
            for (var p = 0; p <= vRows.length - 1; p++) {
                pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                $.mobile.changePage('#pageOrgFOWSTProductInfo');
                                ShowOrgFOWSTProductInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].ProductName)
                            )
                        );
                pLV.append(pItem);
            };

            pLV.listview('refresh');
        };

        function ShowReceiptSettingsInfo(strID) {
            var pData = {};
            pData.OrganizationID = strID;

            //ClearReceiptSettingsFields();

            AjaxSend({
                url: "ReceiptSettings/getReceiptSettingsInfo",
                data: pData,
                success: function (vData) {

                    if (vData.ReceiptSettingsData.Rows.length > 0) {

                        $("#txtReceiptSetInfoID").val(vData.ReceiptSettingsData.Rows[0].ID || '');

                        $("#txtReceiptSetInfoOrganization").val(vData.ReceiptSettingsData.Rows[0].CompanyName || '');
                        $("#cmbReceiptSetInfoOrgTradeName").val(vData.ReceiptSettingsData.Rows[0].OrgTradeNameID || '').selectmenu('refresh');
                        $("#cmbReceiptSetInfoOrgPaperSize").val(vData.ReceiptSettingsData.Rows[0].PaperSize || '').selectmenu('refresh');
                        $("#txtReceiptSetInfoTIN").val(vData.ReceiptSettingsData.Rows[0].TIN || '');

                        //                        AjaxSend({
                        //                            url: "ReceiptSettings/getMessageList",
                        //                            data: pData,
                        //                            success: function (vDataMessage) {

                        //                                LoadReceiptSetMessage(vDataMessage.ReceiptSettingsData.Rows);

                        //                            }
                        //                        });

                        AjaxSend({
                            url: "ReceiptSettings/getPlacesList",
                            data: pData,
                            success: function (vDataPlaces) {

                                LoadReceiptSetPlacesList(vDataPlaces.ReceiptSettingsData.Rows);

                            }
                        });

                    }
                }
            });

        };

        function LoadReceiptSetMessage(vRows) {

            for (var p = 0; p <= vRows.length - 1; p++) {

                arReceiptSetMessage = new Array();
                arReceiptSetMessage[intReceiptSetMessageRecID] = vRows[p].ID;
                arReceiptSetMessage[intReceiptSetMessageFooter] = vRows[p].MessageFooter;
                arReceiptSetMessage[intReceiptSetSortOrder] = vRows[p].SortOrder;
                arReceiptSetMessage[intReceiptSetMessageState] = "";

                sessionStorage["ReceiptSetMessage-" + p] = JSON.stringify(arReceiptSetMessage);

            };

            intReceiptSetMessageCntr = vRows.length;
            LoadReceiptSetMessageList();

        };

        function LoadReceiptSetMessageList() {
            var pLV = $('#lstReceiptSetInfoMessages');
            var pItem;

            pLV.html('');
            for (var i = 0; i <= intReceiptSetMessageCntr; i++) {

                if (sessionStorage["ReceiptSetMessage-" + i] != undefined) {

                    arReceiptSetMessage = new Array();
                    arReceiptSetMessage = JSON.parse(sessionStorage["ReceiptSetMessage-" + i]);

                    if (arReceiptSetMessage[intReceiptSetMessageState] != "Remove") {

                        pItem = $('<li id="lst' + i + '">')
                                    .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arReceiptSetMessage)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowReceiptSetMessageInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arReceiptSetMessage[intReceiptSetMessageFooter])
                                    )
                            );


                        pLV.append(pItem);

                    }

                }
            }

            pLV.listview('refresh');
        };

        function LoadReceiptSetPlacesList(vRows) {
            var pLV = $('#fldsetPlaces');
            var pItem;

            intReceiptSetPlacesCntr = vRows.length - 1;

            pLV.html('');
            for (var i = 0; i <= vRows.length - 1; i++) {

                pItem = $('<input>')
                                .attr("type", 'checkbox')
                                .attr('id', 'chkPlaces-' + i)
                                .attr('checked', ((vRows[i].ID || '') == '' ? false : true))
                                .data('Data', vRows[i])
                            .append($('<label>')
                                .attr('for', 'chkPlaces-' + i)
                                .text((vRows[i].BranchName || 'No Branch Name'))
                            );

                pLV.append(pItem);
            }

            $('#divReceiptSettingsPlaces').trigger("create");
        };

    })

})