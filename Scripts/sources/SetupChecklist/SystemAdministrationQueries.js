require([
    'common',
    '/Scripts3/model/ItemModifiers.js?crc='
    //, '/Scripts/jquery.dataTables.min.js'
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
        var isDailyBeginningBalanceLoaded = "";
        var isPrintSettingsLoaded = "";
        var isPurchaseOrderSettingsPageLoaded = "";
        var isProductMakerSettingsPageLoaded = '';
        var isErpSettingsPageLoaded = "";
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
        var isGeneralSettingsLoaded = "";
        var isDiscountPageLoaded = "";
        var isPIMPageLoaded = "";
        var isDormantSettingsLoaded = "";
        var isPIMWebFormPageLoaded = "";
        var isFOWSTProdListPageLoaded = "";
        var isSingleProdPricePageLoaded = "";
        var isBulkPricePageLoaded = "";
        var isCompanyWideProductPageLoaded = "";
        var isCustomerPageLoaded = "";
        var isSupplierPageLoaded = "";
        var isTableViewPageLoaded = "";
        var isPaymentTermsPageLoaded = "";
        var isAccAdjustmentPageLoaded = "";
        var isEcommerceGeneralSettingsPageLoaded = "";
        var isEcommerceBannerSettingsPageLoaded = "";
        var isEcommerceVendorPortalSettingsPageLoaded = "";
        var isFlushingReportPageLoaded = "";
        var isWebPosSettingsPageLoaded = "";
        var isAddOnlineStoreCategoriesPageLoaded = "";
        var isViewOnlineStoreCategoriesPageLoaded = "";
        var isAddVDMModelsPageLoaded = "";
        var isViewVDMModelsPageLoaded = "";
        var isGPSSettingsPageLoaded = "";
        var isCentersPageLoaded = "";
        var isPhysicalButtonSettingsPageLoaded = "";
        var isGuidessPageLoaded = "";
        var isAdsPageLoaded = "";
        var isAccAdjustmentClicked = false;
        var isItemModifierClicked = false;
        var isPIMClicked = false;
        var isDormantSettingsClicked = false;
        var isPIMWebFormClicked = false;
        var isFOWSTClicked = false;
        var isDeviceClicked = false;
        var isReceiptClicked = false;
        var isSecRoleClicked = false;
        var isEmployeeClicked = false;
        var isSalesReturnClicked = false;
        var isPaymentTermsClicked = false;
        var isDiscountClicked = false;
        var isPrinterRoleClicked = false;
        var isDecimalClicked = false;
        var isPOSClicked = false;
        var isSalesOrderClicked = false;
        var isRecTransClicked = false;
        var isInventoryCountClicked = false;
        var isDailyBeginningBalanceClicked = false;
        var isPurchaseOrderClicked = false;
        var isHotButtonClicked = false;
        var isGroupNameClicked = false;
        var isCustomerClicked = false;
        var isSupplierClicked = false;
        var isSalesTeamClicked = false;
        var isDoneClicked = false;
        var isPrintersClicked = false;
        var isTableViewClicked = false;
        var isOnlineStoreCategoriesClicked = false;
        var isVDMPhysicalButtonsClicked = false;
        var isVDMRiceDispensingGuidesClicked = false;
        var isVDMAdsClicked = false;
        var isCentersClicked = false;
        var pData = {};
        var tblSysAdminDataTable = "";
        var mPrinters; // App.controller.Printer
        var mPurchaseOrderSettings; // App.controller.PurchaseOrderSettings
        var mProductMakerSettings; // App.controller.ProductMakerSettings
        var mErpSettings; // App.controller.ErpSettings
        var mPIMPriceAdjustment; // App.controller.PIMPriceAdjustment
        var mUpdateeServerIpSettings; // App.controller.UpdateServerIpSettings
        var mUpdateServerIpSettingsPage = '';
        var mPimChangeProductArea; // ERP.ChangeProductArea.controller
        var optProductCategories = '', optProductBrands = '';
        var mAutoDataArchiving;

        window.InitVal = function () {
            InitVal();
        }

        function InitVal() {
            isItemModifierClicked = true;
            isPIMClicked = true;
            isDormantSettingsClicked = true;
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
            isDailyBeginningBalanceClicked = true;
            isPurchaseOrderClicked = true;
            isHotButtonClicked = true;
            isGroupNameClicked = true;
            isCustomerClicked = true;
            isSupplierClicked = true;
            isSalesTeamClicked = true;
            isPrintersClicked = true;
            isTableViewClicked = true;
            isPaymentTermsClicked = true;
            isAccAdjustmentClicked = true;
            isOnlineStoreCategoriesClicked = true;
            isVDMPhysicalButtonsClicked = true;
            isVDMRiceDispensingGuidesClicked = true;
            isVDMAdsClicked = true;
            isCentersClicked = true;
        }

        window.LibraryTable = {

            //Display Organization Page
            OrganizationPage: function() {
                //$.getScript("/Scripts/sources/Organization/OrganizationSummary.js?crc=");
                if (isOrganizationPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationSummary?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isOrganizationPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isOrganizationPageLoaded); //.trigger('create');
                    OrgSummary.ShowSumOrgInfo();
                }

            },

            //FOWST
            Fowst: function(s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + '&mod=fowst&isactive=' + $('#selOptionStatus').val() + '&status=' + $('#selOptionFOWSTStatus').val(),
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Name;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.CompleteFowstAddress;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.ModifiedBy;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastTransactionDate == null) {
                                    return "";
                                }
                                var pLastTransactionDate = data.LastTransactionDate;
                                return $.format.date(new Date(parseInt(pLastTransactionDate.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (CBool(data.IsActive) ? 'Active' : 'Inactive');
                            }
                        }
                    ],
                    click: function(data) {
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
            FowstSearch: function(s) {
                var data = { SearchName: s }

                AjaxSend({
                    url: 'SetupCheckList/FowstSearch',
                    data: data,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.FOWST.Rows.length > 0) {
                            for (var p = 0; p < vData.FOWST.Rows.length; p++) {
                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .text(vData.FOWST.Rows[p].Name)
                                        .data('data', vData.FOWST.Rows[p])
                                        .click(function(e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function(m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) {
                                                alert('This record already exists in your database. To add a new unique record, click on the link below.');
                                                return
                                            };

                                            sessionStorage["AddViewLibBCName"] = "Add FOWST";
                                            window.location.hash = 'UniversalPage';
                                            LibraryTable.AddFowstPage();
                                            InitVal();
                                        })
                                    ));

                            }

                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Fowst Page
            AddFowstPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isOrgAddressPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationAddress?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isOrgAddressPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            //Fowst.ShowFowstInfo(sessionStorage.ViewID || '');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isOrgAddressPageLoaded); //.trigger('create');
                    Fowst.ShowFowstInfo(sessionStorage.ViewID || '');
                }
            },

            //Authorized Devices
            Devices: function(s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=device&isactive=" + $('#selOptionStatus').val(),
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.PhoneName;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.PhoneModel;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.IMEI;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.UUID;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (CBool(data.IsActive) ? 'Active' : 'Inactive');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.ModifiedBy;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function(data) {
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
            DevicesSearch: function(s) {
                var data = { SearchName: s }
                AjaxSend({
                    url: 'SetupCheckList/DeviceSearch',
                    data: data,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.AuthorizedDevice.Rows.length > 0) {
                            for (var p = 0; p < vData.AuthorizedDevice.Rows.length; p++) {

                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .append($('<div style="font-weight:bold;">')
                                            .text('Device Name: ' + vData.AuthorizedDevice.Rows[p].PhoneName)
                                        )
                                        .append($('<div>')
                                            .text('Model: ' + vData.AuthorizedDevice.Rows[p].PhoneModel)
                                        )
                                        .append($('<div>')
                                            .text('IMEI: ' + vData.AuthorizedDevice.Rows[p].IMEI)
                                        )
                                        .append($('<div>')
                                            .text('UUID: ' + vData.AuthorizedDevice.Rows[p].UUID)
                                        )
                                        .data('data', vData.AuthorizedDevice.Rows[p])
                                        .click(function(e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function(m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) {
                                                alert('This record already exists in your database. To add a new unique record, click on the link below.');
                                                return
                                            };

                                            sessionStorage["AddViewLibBCName"] = "Add Device";
                                            window.location.hash = 'UniversalPage';
                                            LibraryTable.AddDevicePage();
                                            InitVal();
                                        })
                                    ));
                            }

                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Device Page
            AddDevicePage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isDeviceInfoPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Device/DeviceInfo?cache=1&mode=wizard&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isDeviceInfoPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create'); $("#txtID").val(''); $("#txtPhoneName").val(''); $("#txtPhoneModel").val(''); $("#txtIMEI").val(''); $("#txtUUID").val(''); $("#txtOS").val(''); $("#txtDeviceNo").val(''); $("#selDeviceDeactivate").val('1').slider("refresh");
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isDeviceInfoPageLoaded); //.trigger('create'); $("#txtID").val(''); $("#txtPhoneName").val(''); $("#txtPhoneModel").val(''); $("#txtIMEI").val(''); $("#txtUUID").val(''); $("#txtOS").val(''); $("#txtDeviceNo").val(''); $("#selDeviceDeactivate").val('1').slider("refresh");
                }
            },

            ViewDevicePage: function() {

                var pData = {
                    IsNew: 0,
                    ID: sessionStorage.ViewID
                };

                AjaxSend({
                    url: 'Device/setCreateNew',
                    data: pData,
                    success: function(vData) {

                        if ((vData.IsNew || '') == "TRUE") {
                            PromptUserAccess('Devices', {
                                OnClickOKSuccess: function() {
                                    //window.open(GetPathFormat("/Device/DeviceInfo"), "_self");
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/Device/DeviceInfo');
                                }
                            });
                        } else {

                            if (GetUrlParameters().mode == 'wizard') {
                                window.open(GetPathFormat("/Device/DeviceInfoSummary?mode=wizard"), "_self");
                            } else {
                                //window.open(GetPathFormat("/Device/DeviceInfoSummary"), "_self");
                                InitVal();
                                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                                if (isDeviceInfoSummaryPageLoaded == "") {
                                    AjaxSend({
                                        ExactUrl: '/Device/DeviceInfoSummary',
                                        type: 'GET',
                                        dataType: 'text',
                                        success: function(vData) {
                                            isDeviceInfoSummaryPageLoaded = vData;
                                            $('#UniversalDiv').html('');
                                            $('#UniversalDiv').append(vData); //.trigger('create');
                                            //Library.View.ShowDeviceInfo();
                                            App.Deferred.fulfill('FQ26E5TP76');
                                            isGroupNameClicked = true;
                                        }
                                    });
                                } else {
                                    $('#UniversalDiv').html('');
                                    $('#UniversalDiv').append(isDeviceInfoSummaryPageLoaded); //.trigger('create');
                                    AuthorizedDevices.ShowDeviceInfo();
                                    isGroupNameClicked = true;
                                }
                            }
                        }
                    }
                });
            },

            //Receipt Settings
            ReceiptSettings: function(s) {
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
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.CompanyName;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.BusinessTradeName;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.PaperSize;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.ModifiedBy;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function(data) {
                        sessionStorage["AddViewLibBCName"] = "View Receipt Template";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        isGroupNameClicked = true;

                        //var pData = {};
                        //pData.IsNew = 0;
                        //pData.ID = sessionStorage.ViewID;

                        //AjaxSend({
                        //    url: 'ReceiptSettings/setCreateNew',
                        //    data: pData,
                        //    success: function (vData) {
                        //        LibraryTable.AddReceiptSettingsPage();
                        //    }
                        //});

                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Receipt Settings
            ReceiptSettingsSearch: function(s) {
                var pData = {};
                pData.SearchString = s;

                AjaxSend({
                    url: 'ReceiptSettings/getReceiptSettingsList',
                    data: pData,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.ReceiptSettingsData.Rows.length > 0) {
                            for (var p = 0; p < vData.ReceiptSettingsData.Rows.length; p++) {
                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .append($('<div style="font-weight:bold;">')
                                            .text(vData.ReceiptSettingsData.Rows[p].CompanyName)
                                        )
                                        .append($('<div>')
                                            .text('Model: ' + vData.ReceiptSettingsData.Rows[p].PaperSize)
                                        )
                                        .data('data', vData.ReceiptSettingsData.Rows[p])
                                        .click(function(e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function(m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) {
                                                alert('This record already exists in your database. To add a new unique record, click on the link below.');
                                                return
                                            };
                                            InitVal();
                                        })
                                    ));
                            }

                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Security Role Page
            AddReceiptSettingsPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isReceiptSettingPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/ReceiptSettings/ReceiptSettingsInfo?v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isReceiptSettingPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isReceiptSettingPageLoaded); //.trigger('create');
                    setTimeout(function() { ReceiptSettings.ShowReceiptInfo(); }, 1000);
                }
            },

            //Security Roles
            SecurityRoles: function(s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=security&isactive=" + $('#selOptionStatus').val(),
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Code;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Role;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Description;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.ModifiedBy;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function(data) {
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
            SecurityRolesSearch: function(s) {

                var pData = {};
                pData.SearchString = s;

                AjaxSend({
                    url: 'SecurityRole/getSecurityRoleList',
                    data: pData,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.SecurityRoleData.Rows.length > 0) {
                            for (var p = 0; p < vData.SecurityRoleData.Rows.length; p++) {
                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .append($('<div style="font-weight:bold;">')
                                            .text(vData.SecurityRoleData.Rows[p].Code)
                                        )
                                        .append($('<div>')
                                            .text('Model: ' + vData.SecurityRoleData.Rows[p].Role)
                                        )
                                        .data('data', vData.SecurityRoleData.Rows[p])
                                        .click(function(e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function(m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) {
                                                alert('This record already exists in your database. To add a new unique record, click on the link below.');
                                                return
                                            };
                                            InitVal();
                                        })
                                    ));
                            }

                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Security Role Page
            AddSecurityRolePage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isSecurityRoleInfoPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SecurityRole/SecurityRoleInfo?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isSecurityRoleInfoPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');

                            if (sessionStorage.AddViewLibBCName == "Add Security Role") {
                                SecurityRoleSum.ShowSumSecurityRoleInfo('');
                            }
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isSecurityRoleInfoPageLoaded); //.trigger('create');
                    SecurityRoleSum.ShowSumSecurityRoleInfo('');
                }
            },

            ViewSecurityRolePage: function() {
                var pData = {};

                pData.IsNew = 0;
                pData.ID = sessionStorage.ViewID;

                AjaxSend({
                    url: 'SecurityRole/setCreateNew',
                    data: pData,
                    success: function(vData) {

                        window.IsPageLoaded = false;
                        if ((vData.IsNew || '') == "TRUE") {
                            PromptUserAccess('Security Roles', {
                                OnClickOKSuccess: function() {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/SecurityRole/SecurityRoleInfo');
                                }
                            });
                        } else {
                            //window.open(GetPathFormat("/SecurityRole/SecurityRoleSummary"), "_self");
                            InitVal();
                            $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                            if (isSecurityRoleSummaryPageLoaded == "") {
                                AjaxSend({
                                    ExactUrl: '/SecurityRole/SecurityRoleSummary',
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function(vData) {
                                        isSecurityRoleSummaryPageLoaded = vData;
                                        $('#UniversalDiv').html('');
                                        $('#UniversalDiv').append(vData); //.trigger('create');

                                        //Library.View.ShowSecurityInfo();
                                        App.Deferred.fulfill('FQ26E5TP76');
                                    }
                                });
                            } else {
                                $('#UniversalDiv').html('');
                                $('#UniversalDiv').append(isSecurityRoleSummaryPageLoaded); //.trigger('create');
                                SecurityRoleSum.ShowSumSecurityRoleInfo(sessionStorage.ViewID);
                            }
                        }
                        isGroupNameClicked = true;
                    }
                });
            },

            //Employees
            Employees: function(s) {
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
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.PeopleName;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.DateHired == null) {
                                    return "";
                                }
                                var date = data.DateHired;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                var status = GetEmployeeStatus(data);
                                return status;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (CBool(data.IsBan) ? 'Yes' : 'No');
                            }
                        }
                    ],
                    click: function(data) {
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
            EmployeesSearch: function(s) {

                var pData = {};
                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryPeople',
                    data: pData,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span style="font-weight:bold;">')
                                        .text(vData.Results.Rows[p].PeopleName)
                                    )
                                    .data('data', vData.Results.Rows[p])
                                    .click(function(e) {
                                        var id = $(this).data('data').ID;
                                        var IsFOWSTExist = false;

                                        $('#tblBody tr').each(function(m_index) {
                                            if ($.trim($(this).data('data').PeopleID) == id) {
                                                IsFOWSTExist = true;
                                            }
                                        });

                                        if (IsFOWSTExist) {
                                            alert('This record already exists in your database. To add a new unique record, click on the link below.');
                                            return
                                        };

                                        sessionStorage["OptionToAdd"] = "SendInvite";
                                        sessionStorage["txtOrgPeopleID"] = $(this).data('data').ID;
                                        sessionStorage["btnOrgEmployeeName"] = $(this).data('data').PeopleName;
                                        sessionStorage["AddViewLibBCName"] = "Add Employee";
                                        sessionStorage["EmpFirstName"] = $(this).data('data').PeopleFirstName;
                                        window.location.hash = 'UniversalPage';
                                        InitVal();
                                        LibraryTable.AddEmployeePage();
                                    })
                                );
                            }

                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Employee Page
            AddEmployeePage: function(option) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isAddEmployeePageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationEmployee?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isAddEmployeePageLoaded = vData;
                            isViewEmployeePageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');

                            if (sessionStorage.OptionToAdd == "SendInvite") {
                                $('#pageOrgEmployeeInfo').show();
                                $('#txtOrgPeopleID').val(sessionStorage.txtOrgPeopleID);
                                $('#txtOrgEmployeeNew').val(sessionStorage.btnOrgEmployeeName);
                                $('#btnOrgEmpInfoSaveAndInvite').html(sessionStorage.EmpFirstName + '\'s Mobiz app');
                                $('#btnOrgEmpInfoSaveAndConfirm').html(sessionStorage.EmpFirstName + ' sign in and confirm now');
                                $('#btnOrgEmpInfoSendConfirmation').html(sessionStorage.EmpFirstName + '\'s Email');
                                $('#btnOrgAddEmployee').trigger('click');
                                setTimeout(function() {
                                    $('#txtOrgEmployeeNew').val(sessionStorage.btnOrgEmployeeName);
                                }, 1200)

                                if (typeof OrgEmp != 'undefined') {
                                    OrgEmp.GetRequiredData();
                                }
                            } else if (sessionStorage.OptionToAdd == "Add Employee") {
                                $('#pageOrgEmployeeAdd').show();

                                if (typeof OrgEmpAdd != 'undefined') {
                                    OrgEmpAdd.ShowOrgEmpAddData();
                                }
                            }
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isAddEmployeePageLoaded); //.trigger('create');

                    if (sessionStorage.OptionToAdd == "SendInvite") {
                        $('#pageOrgEmployeeInfo').show();
                        $('#txtOrgPeopleID').val(sessionStorage.txtOrgPeopleID);
                        $('#txtOrgEmployeeNew').val(sessionStorage.btnOrgEmployeeName);
                        $('#btnOrgEmpInfoSaveAndInvite').html(sessionStorage.EmpFirstName + '\'s Mobiz app');
                        $('#btnOrgEmpInfoSaveAndConfirm').html(sessionStorage.EmpFirstName + ' sign in and confirm now');
                        $('#btnOrgEmpInfoSendConfirmation').html(sessionStorage.EmpFirstName + '\'s Email');
                        $('#btnOrgAddEmployee').trigger('click');
                        setTimeout(function() {
                            $('#txtOrgEmployeeNew').val(sessionStorage.btnOrgEmployeeName);
                        }, 1200)
                        OrgEmp.GetRequiredData();
                    } else if (sessionStorage.OptionToAdd == "Add Employee") {
                        $('#pageOrgEmployeeAdd').show();
                        OrgEmpAdd.ShowOrgEmpAddData();
                    }
                }

            },

            //Display View Employee Page
            ViewEmployeePage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isViewEmployeePageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationEmployee?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isViewEmployeePageLoaded = vData;
                            isAddEmployeePageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            $('#pageOrgEmployeeInfo').show();
                            setMonthCollection("selOrgDateHiredMonth2");
                            setYearCollection("selOrgDateHiredYear2");

                            setMonthCollection("selOrgDateInActiveMonth");
                            setYearCollection("selOrgDateInActiveYear");

                            $('#btnOrgEmpInfoSaveAndInvite').html(sessionStorage.EmpFirstName + '\'s Mobiz app');
                            $('#btnOrgEmpInfoSaveAndConfirm').html(sessionStorage.EmpFirstName + ' sign in and confirm now');
                            $('#btnOrgEmpInfoSendConfirmation').html(sessionStorage.EmpFirstName + '\'s Email');

                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isViewEmployeePageLoaded); //.trigger('create');
                    $('#pageOrgEmployeeInfo').show();
                    $('#btnOrgEmpInfoSaveAndInvite').html(sessionStorage.EmpFirstName + '\'s Mobiz app');
                    $('#btnOrgEmpInfoSaveAndConfirm').html(sessionStorage.EmpFirstName + ' sign in and confirm now');
                    $('#btnOrgEmpInfoSendConfirmation').html(sessionStorage.EmpFirstName + '\'s Email');
                    OrgEmp.ShowOrgEmpData();
                }

            },

            //Display Work Schedule Page
            WorkSchedulePage: function() {
                $('#DTRUniversalDiv').empty();
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function(vData) {
                        var pData = {};
                        pData.OrgID = sessionStorage["OrgID"];
                        InitVal();
                        if (isWorkSchedulePageLoaded == "") {
                            AjaxSend({
                                ExactUrl: '/Organization/OrganizationEmployeeWorkingHours?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                type: 'GET',
                                dataType: 'text',
                                success: function(vData) {
                                    isWorkSchedulePageLoaded = vData;
                                    $('#UniversalDiv').empty();
                                    $('#UniversalDiv').append(vData); //.trigger('create');
                                    setTimeout(function() {
                                        OrgEmpWorkHours.GetSchedule();
                                    }, 2000)
                                    App.Deferred.fulfill('FQ26E5TP76');
                                }
                            });
                        } else {
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(isWorkSchedulePageLoaded); //.trigger('create');
                            OrgEmpWorkHours.GetSchedule();
                        }
                    },
                    error: function(vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function(e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            },

            //Display PIM Page
            PIMPage: function(s) {
                var url = "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=pim&isdeactivate=" + $('#selOptionStatus option:selected').val();
                if (CStr2(s) != '') {
                    $('#tblSysAdminDataTable').DataTable().ajax.url(url);
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: url,
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.Image == null) {
                                    return '';
                                } else {
                                    return '<img src="/Content/MobizImagePIM/'+ sessionStorage.OrgID +'/Small/'+ data.Image +'" />';
                                }
                            }, createdCell: function (td, cellData, rowData, row, col) {
                                $(td).click(function (e) {
                                    if (rowData.Image != null) {
                                        e.preventDefault();
                                        e.stopPropagation();

                                        ShowProductImage(rowData.ProductID);
                                    }
                                })
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (data.ProductNo || '');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return CStr2(data.Brand);
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.ProductName;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                // return AccountingFormat(data.Srp);
                                var pStr = '<b>' + AccountingFormat(data.Srp) + '</b>';
                                if (CStr2(data.SrpLastUpdated) != '') {
                                    if (!IsDate(data.SrpLastUpdated))
                                        data.SrpLastUpdated = new Date(parseInt(data.SrpLastUpdated.substr(6)));
                                    pStr += '<br />Last&nbsp;Updated:<br />' + FormatDate(data.SrpLastUpdated, { Format: 'M/d/yyyy' });
                                    pStr += '<br />' + AccountingFormat((new Date().getTime() - new Date(data.SrpLastUpdated)) / 1000 / 60 / 60 / 24, 0) + ' days ago';
                                }
                                pStr += '<br /><a name="lnkRequestPriceUpdate" href="">Request Price Update</a>';
                                pStr = '<div style="text-align: center">' + pStr + '</div>';

                                return pStr;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                var pStr = '<b>' + AccountingFormat(data.RetailPrice) + '</b>';
                                if (CStr2(data.RetailPriceLastUpdated) != '') {
                                    if (!IsDate(data.RetailPriceLastUpdated))
                                        data.RetailPriceLastUpdated = new Date(parseInt(data.RetailPriceLastUpdated.substr(6)));
                                    pStr += '<br />Last&nbsp;Updated:<br />' + FormatDate(data.RetailPriceLastUpdated, { Format: 'M/d/yyyy' });
                                    pStr += '<br />' + AccountingFormat((new Date().getTime() - new Date(data.RetailPriceLastUpdated)) / 1000 / 60 / 60 / 24, 0) + ' days ago';
                                }
                                pStr += '<br /><a name="lnkRequestPriceUpdate" href="">Request Price Update</a>';
                                pStr = '<div style="text-align: center">' + pStr + '</div>';

                                return pStr;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                var pStr = '<b>' + AccountingFormat(data.WholesalePrice) + '</b>';
                                if (CStr2(data.WholesalePriceLastUpdated) != '') {
                                    if (!IsDate(data.WholesalePriceLastUpdated))
                                        data.WholesalePriceLastUpdated = new Date(parseInt(data.WholesalePriceLastUpdated.substr(6)));
                                    pStr += '<br />Last&nbsp;Updated:<br />' + FormatDate(data.WholesalePriceLastUpdated, { Format: 'M/d/yyyy' });
                                    pStr += '<br />' + AccountingFormat((new Date().getTime() - new Date(data.WholesalePriceLastUpdated)) / 1000 / 60 / 60 / 24, 0) + ' days ago';
                                }
                                pStr += '<br /><a name="lnkRequestPriceUpdate" href="">Request Price Update</a>';
                                pStr = '<div style="text-align: center">' + pStr + '</div>';

                                return pStr;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return CStr2(data.Category);
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.ActivationDate == null) {
                                    return "";
                                }
                                var ActivationDate = data.ActivationDate;
                                return $.format.date(new Date(parseInt(ActivationDate.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var pLastUpdated = data.LastUpdated;
                                return $.format.date(new Date(parseInt(pLastUpdated.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.DeActivated == null) {
                                    return "";
                                }
                                var ActivationDate = data.DeActivated;
                                return (data.DeActivated == 1 ? 'InActive' : 'Active');
                            }
                        }
                    ],
                    click: function(data) {
                        sessionStorage["AddViewLibBCName"] = "SKU Record";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ProductID;
                        if (isPIMClicked) {
                            LibraryTable.CompanyWideProductListPage();
                            $('.imgareaselect-outer').remove();
                        }
                        InitVal();
                    },
                    CallBack: function (data) {
                        var txtarea = $('<textarea id="txtCopyTransToPlainText" style="margin-left:-1000px;">');

                        if ($('body').find('#txtCopyTransToPlainText').length == 0) {
                            $('body').append(txtarea);
                        } else {
                            txtarea = $('#txtCopyTransToPlainText');
                            txtarea.val('');
                        }

                        $('#btnExportListAsPlainText').unbind('click').click(function () {
                            var msg = 'List of SKUs \n\n', i = 0;

                            for (i; i < data.length; i++) {
                                msg += (i + 1) + '.\n';
                                msg += data[i].ProductNo + '\n';
                                msg += data[i].ProductName + '\n\n';
                            }

                            txtarea.val(msg).select();
                            document.execCommand("copy");
                            WebToast('Copied to clipboard');
                        })
                    },
                    disablecols: [0] //if all '_all' // remove the  , 3, 4, 5
                }

                SysAdminDataTable(pData);
            },

            //Search PIM Search
            PIMSearch: function(s) {
                var pData = {};
                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryPIMProductPublic',
                    data: pData,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<div style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].ProductName)
                                        )
                                        .append($('<div>')
                                            .text('Published by: ' + vData.Results.Rows[p].PublishedBy + ' (Brand Owner)')
                                        )
                                        .append($('<div>')
                                            .text('Uplaoded by: ' + vData.Results.Rows[p].UploadedBy)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function(e) {
                                            sessionStorage["AddViewLibBCName"] = "Add Product";
                                            window.location.hash = 'UniversalPage';
                                            sessionStorage["ViewID"] = $(this).attr('ID');
                                            InitVal();
                                            LibraryTable.CompanyWideProductListPage();
                                        })
                                    ));
                            }

                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display PIM Brands
            PIMBrands: function(s) {
                var url = "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=pimbrands&isactive=" + $('#selOptionStatus').val();
                if (CStr2(s) != '') {
                    $('#tblSysAdminDataTable').DataTable().ajax.url(url);
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: url,
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Brand;
                            }
                        },
                        {
                            className: 'text-center',
                            "data": function(data, type, full, meta) {
                                return data.LastUpdatedByName;
                            }
                        },
                        {
                            className: 'text-center',
                            "data": function(data, type, full, meta) {
                                return $.format.date(CJsonDate(data.LastUpdated), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            className: 'text-center',
                            "data": function(data, type, full, meta) {
                                return (CBool(data.IsActive) ? 'Active' : 'Inactive');
                            }
                        }
                    ],
                    click: function(data) {
                        var opt = []

                        if (CBool(data.IsActive)) {
                            opt.push({
                                Text: 'Edit Name',
                                Click: function() {
                                    $('.lblPIMLib').text('Brand');
                                    $('#popupPIMLib').modal('show');
                                    $('#txtPIMLibID').val(data.ID);
                                    $('#txtPIMLib').attr('currentname', data.Brand).val(data.Brand).focus();
                                }
                            })

                            opt.push({
                                Text: 'Deactivate Brand Name',
                                Click: function() {
                                    AjaxSend({
                                        url: 'PIMProduct/GetProducts',
                                        data: {
                                            data: {
                                                type: sessionStorage.LibraryName,
                                                currentname: data.Brand
                                            }
                                        },
                                        success: function(e) {
                                            $('.lblPIMLib').text('Brand');
                                            $('#lblPIMLibNameDeac').text(data.Brand);

                                            var popupPIMLibDeacName = $('#popupPIMLibDeacName');
                                            var tables = popupPIMLibDeacName.find('table');
                                            var tbody = popupPIMLibDeacName.find('tbody');
                                            var vRows = e.Results.Rows;
                                            var newbrand;

                                            tbody.empty();

                                            if (vRows.length > 0) {
                                                $('#divForAvailablePIM').show();
                                                for (var i = 0; i < vRows.length; i++) {
                                                    tbody.append($('<tr>')
                                                        .append($('<td style="width:50px;">').text(i + 1 + '.'))
                                                        .append($('<td>').text(vRows[i].ProductNo))
                                                        .append($('<td>').text(vRows[i].ProductName))
                                                    )
                                                }
                                            } else {
                                                $('#divForAvailablePIM').hide();
                                                tbody.append($('<tr>').append($('<td colspan="3" style="text-align:center;">').text('No Products found for brand: ' + data.Brand)))
                                            }

                                            $('#btnSelectAnotherPIMLibName').text('Select Brand').unbind('click').click(function() {
                                                BrowsePopup('LibraryPimBrands', 'Select New Brand', [
                                                    {
                                                        AutoSearch: true,
                                                        DisplayText: [{ Column: 'Brand' }],
                                                        Click: function(e) {
                                                            newbrand = e.Brand;
                                                            $('#btnSelectAnotherPIMLibName').text(newbrand);
                                                            popupPIMLibDeacName.modal('show');
                                                        }
                                                    }
                                                ])
                                            })

                                            $('#btnDeactivatePIMLibName').unbind('click').click(function() {
                                                if (vRows.length > 0 && CStr2(newbrand) == '') {
                                                    WebToast('Please select new brand name', 'top');
                                                } else {
                                                    AjaxSend({
                                                        url: 'PIMProduct/DeactivateLibraryName',
                                                        data: {
                                                            data: {
                                                                type: sessionStorage.LibraryName,
                                                                currentname: data.Brand,
                                                                newbrand: newbrand
                                                            }
                                                        },
                                                        success: function(e) {
                                                            if (e.Results == 'saved') {
                                                                WebToast('Successfully deactivated', 'top');
                                                                LibraryTable.PIMBrands();
                                                                popupPIMLibDeacName.modal('hide');
                                                            } else {
                                                                NewMessageBox(e.Results, 'Error');
                                                            }
                                                        }
                                                    })
                                                }
                                            })

                                            if (tables.length > 1) {
                                                tables[1].remove(); // remove extra table, idk why naggenerate ug table
                                            }

                                            popupPIMLibDeacName.modal('show');
                                        }
                                    })
                                }
                            })
                        } else {
                            opt.push({
                                Text: 'Activate Brand Name',
                                Click: function() {
                                    NewMessageBox('Are you sure you want to activate brand name <b>' + data.Brand + '</b>?', 'Message', [
                                        {
                                            Text: 'Yes',
                                            Click: function() {
                                                AjaxSend({
                                                    url: 'PIMProduct/ActivateLibraryName',
                                                    data: {
                                                        data: {
                                                            type: sessionStorage.LibraryName,
                                                            ID: data.ID
                                                        }
                                                    },
                                                    success: function(e) {
                                                        if (e.Results == 'saved') {
                                                            WebToast('Successfully deactivated', 'top');
                                                            LibraryTable.PIMBrands();
                                                            //popupPIMLibDeacName.modal('hide');
                                                        } else {
                                                            NewMessageBox(e.Results, 'Error');
                                                        }
                                                    }
                                                })
                                            }
                                        }, { Text: 'No' }
                                    ])
                                }
                            })
                        }

                        PopupButtonList('', 'Options', opt)
                    },
                    disablecols: [0, 3] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Display PIM Categories
            PIMCategories: function(s) {
                var url = "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=pimcategories&isactive=" + $('#selOptionStatus').val();
                if (CStr2(s) != '') {
                    $('#tblSysAdminDataTable').DataTable().ajax.url(url);
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: url,
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Category;
                            }
                        },
                        {
                            className: 'text-center',
                            "data": function(data, type, full, meta) {
                                return data.LastUpdatedByName;
                            }
                        },
                        {
                            className: 'text-center',
                            "data": function(data, type, full, meta) {
                                return $.format.date(CJsonDate(data.LastUpdated), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            className: 'text-center',
                            "data": function(data, type, full, meta) {
                                return (CBool(data.IsActive) ? 'Active' : 'Inactive');
                            }
                        }
                    ],
                    click: function(data) {
                        var opt = []

                        if (CBool(data.IsActive)) {
                            opt.push({
                                Text: 'Edit Name',
                                Click: function() {
                                    Bs.Loading('show');
                                    AjaxSend({
                                        url: 'PIMProduct/GetProducts',
                                        data: {
                                            data: {
                                                type: sessionStorage.LibraryName,
                                                currentname: data.Category
                                            }
                                        },
                                        success: function(e) {
                                            $('.lblPIMLib').text('Category');
                                            $('#txtPIMLibID').val(data.ID);
                                            $('#txtPIMLibInfoName').attr('currentname', data.Category).val(data.Category).focus();

                                            var popupPIMLibInfo = $('#popupPIMLibInfo');
                                            var tables = popupPIMLibInfo.find('table');
                                            var tbody = popupPIMLibInfo.find('tbody');
                                            var vRows = e.Results.Rows, arrUpdatedProductCategory = [];

                                            tbody.empty();

                                            if (vRows.length > 0) {
                                                for (var i = 0; i < vRows.length; i++) {
                                                    vRows[i].Index = i;
                                                    tbody.append($('<tr>')
                                                        .append($('<td style="width:50px;">').text(i + 1 + '.'))
                                                        .append($('<td>').text(vRows[i].ProductNo))
                                                        .append($('<td>').text(vRows[i].ProductName))
                                                        .append($('<td>').append($('<select data-role="none" class="form-select">').append(optProductCategories).val(vRows[i].Category)
                                                            .data('data', vRows[i])
                                                            .change(function() {
                                                                var dis = $(this);
                                                                var skudata = dis.data('data');
                                                                var newcat = $(this).val();
                                                                if ($('#txtPIMLibInfoName').val() != newcat) {
                                                                    arrUpdatedProductCategory.push({
                                                                        ID: skudata.ID,
                                                                        NewCategory: newcat
                                                                    })
                                                                } else {
                                                                    arrUpdatedProductCategory.splice(skudata.Index, 1);
                                                                }
                                                            })
                                                        ))
                                                        .append($('<td>').text($.format.date(new Date(vRows[i].LastUpdated), 'MM-dd-yy hh:mm a')))
                                                        .append($('<td>').text(vRows[i].ModifiedBy))
                                                    )
                                                }
                                            } else {
                                                tbody.append($('<tr>').append($('<td colspan="3" style="text-align:center;">').text('No Products found for category: ' + data.Category)))
                                            }

                                            $('#txtPIMLibInfoName').unbind('blur').blur(function() {
                                                var dis = $(this);
                                                var newname = $.trim($(this).val());
                                                tbody.find('option[value="' + dis.attr('currentname') + '"]').attr('value', newname).text(newname);
                                            })

                                            $('#btnApplyChangesPIMLib').unbind('click').click(function() {
                                                var txtPIMLibInfoName = $('#txtPIMLibInfoName');
                                                var currentname = txtPIMLibInfoName.attr('currentname');
                                                var newname = txtPIMLibInfoName.val();

                                                Bs.Loading('show');
                                                AjaxSend({
                                                    url: 'PIMProduct/SavePIMLib',
                                                    data: {
                                                        data: {
                                                            ID: $('#txtPIMLibID').val(),
                                                            type: sessionStorage.LibraryName,
                                                            currentname: currentname,
                                                            name: newname,
                                                            UpdatedProductCategory: arrUpdatedProductCategory
                                                        }
                                                    },
                                                    success: function(e) {
                                                        Bs.Loading('hide');
                                                        if (e.Results == 'saved') {
                                                            WebToast('Successfully deactivated', 'top');
                                                            LibraryTable.PIMCategories();
                                                            popupPIMLibInfo.modal('hide');
                                                        } else {
                                                            NewMessageBox(e.Results, 'Error');
                                                        }
                                                    }
                                                })
                                            })

                                            if (tables.length > 1) {
                                                tables[1].remove(); // remove extra table, idk why naggenerate ug table
                                            }
                                            Bs.Loading('hide');
                                            popupPIMLibInfo.modal('show');
                                        }
                                    })
                                }
                            })

                            opt.push({
                                Text: 'Deactivate Category Name',
                                Click: function() {
                                    Bs.Loading('show');
                                    AjaxSend({
                                        url: 'PIMProduct/GetProducts',
                                        data: {
                                            data: {
                                                type: sessionStorage.LibraryName,
                                                currentname: data.Category
                                            }
                                        },
                                        success: function(e) {
                                            $('.lblPIMLib').text('Category');
                                            $('#lblPIMLibNameDeac').text(data.Category);

                                            var popupPIMLibDeacName = $('#popupPIMLibDeacName');
                                            var tables = popupPIMLibDeacName.find('table');
                                            var tbody = popupPIMLibDeacName.find('tbody');
                                            var vRows = e.Results.Rows;
                                            var newcategory;

                                            tbody.empty();

                                            if (vRows.length > 0) {
                                                $('#divForAvailablePIM').show();
                                                for (var i = 0; i < vRows.length; i++) {
                                                    tbody.append($('<tr>')
                                                        .append($('<td style="width:50px;">').text(i + 1 + '.'))
                                                        .append($('<td>').text(vRows[i].ProductNo))
                                                        .append($('<td>').text(vRows[i].ProductName))
                                                    )
                                                }
                                            } else {
                                                $('#divForAvailablePIM').hide();
                                                tbody.append($('<tr>').append($('<td colspan="3" style="text-align:center;">').text('No Products found for category: ' + data.Category)))
                                            }

                                            $('#btnSelectAnotherPIMLibName').text('Select Category').unbind('click').click(function() {
                                                BrowsePopup('LibraryPimCategories', 'Select New Category', [
                                                    {
                                                        AutoSearch: true,
                                                        DisplayText: [{ Column: 'Category' }],
                                                        Click: function(e) {
                                                            newcategory = e.Category;
                                                            $('#btnSelectAnotherPIMLibName').text(newcategory);
                                                            popupPIMLibDeacName.modal('show');
                                                        }
                                                    }
                                                ])
                                            })

                                            $('#btnDeactivatePIMLibName').unbind('click').click(function() {
                                                if (vRows.length > 0 && CStr2(newcategory) == '') {
                                                    WebToast('Please select new category name', 'top');
                                                } else {
                                                    AjaxSend({
                                                        url: 'PIMProduct/DeactivateLibraryName',
                                                        data: {
                                                            data: {
                                                                type: sessionStorage.LibraryName,
                                                                currentname: data.Category,
                                                                newcategory: newcategory
                                                            }
                                                        },
                                                        success: function(e) {
                                                            if (e.Results == 'saved') {
                                                                WebToast('Successfully deactivated', 'top');
                                                                LibraryTable.PIMCategories();
                                                                popupPIMLibDeacName.modal('hide');
                                                            } else {
                                                                NewMessageBox(e.Results, 'Error');
                                                            }
                                                        }
                                                    })
                                                }
                                            })

                                            if (tables.length > 1) {
                                                tables[1].remove(); // remove extra table, idk why naggenerate ug table
                                            }
                                            Bs.Loading('hide');
                                            popupPIMLibDeacName.modal('show');
                                        }
                                    })
                                }
                            })
                        } else {
                            opt.push({
                                Text: 'Activate Category Name',
                                Click: function() {
                                    NewMessageBox('Are you sure you want to activate category name <b>' + data.Category + '</b>?', 'Message', [
                                        {
                                            Text: 'Yes',
                                            Click: function() {
                                                AjaxSend({
                                                    url: 'PIMProduct/ActivateLibraryName',
                                                    data: {
                                                        data: {
                                                            type: sessionStorage.LibraryName,
                                                            ID: data.ID
                                                        }
                                                    },
                                                    success: function(e) {
                                                        if (e.Results == 'saved') {
                                                            WebToast('Successfully deactivated', 'top');
                                                            LibraryTable.PIMCategories();
                                                            //popupPIMLibDeacName.modal('hide');
                                                        } else {
                                                            NewMessageBox(e.Results, 'Error');
                                                        }
                                                    }
                                                })
                                            }
                                        }, { Text: 'No' }
                                    ])
                                }
                            })
                        }

                        PopupButtonList('', 'Options', opt)
                    },
                    disablecols: [0, 3] //if all '_all'
                }

                SysAdminDataTable(pData);
                GetProductCategories();
                GetInventoryItemCategory();
            },

            //Display Sales Return Settings Page
            SalesReturnsSettingsPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isSalesReturnPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SalesReturns/Settings?cache=1&OrgID=' + sessionStorage.OrgID + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isSalesReturnPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isSalesReturnPageLoaded); //.trigger('create');
                    SalesReturn.GetSettings();
                }
            },

            //Display Discount Settings Page
            DiscountSettingsPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isDiscountPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/DiscountSettings?cache=1&OrgID=' + sessionStorage.OrgID + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isDiscountPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isDiscountPageLoaded); //.trigger('create');
                    DiscountSetting.GetDiscountSettings();
                }
            },

            GPSSettingsPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isGPSSettingsPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/GPS/Settings?cache=1&OrgID=' + sessionStorage.OrgID + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isGPSSettingsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isGPSSettingsPageLoaded); //.trigger('create');
                    GPSSettings.Load();
                }
            },

            //Printer Roles
            PrinterRoles: function(s) {
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
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.PrinterRole;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.PrinterType;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (CBool(data.IsActive) ? 'Yes' : 'No');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.ModifiedBy;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function(data) {
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
            PrinterRolesSearch: function(s) {

                var pData = {};
                pData.SearchString = s;

                AjaxSend({
                    url: 'DevicePrinter/getDevicePrinterList',
                    data: pData,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.DevicePrinterData.Rows.length > 0) {
                            for (var p = 0; p < vData.DevicePrinterData.Rows.length; p++) {
                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .append($('<div style="font-weight:bold;">')
                                            .text('Printer Role: ' + vData.DevicePrinterData.Rows[p].PrinterRole)
                                        )
                                        .append($('<div>')
                                            .text('Printer Type: ' + vData.DevicePrinterData.Rows[p].PrinterType)
                                        )
                                        .append($('<div>')
                                            .text('Active: ' + (vData.DevicePrinterData.Rows[p].IsActive == 1 ? 'Yes' : 'No'))
                                        )
                                        .data('data', vData.DevicePrinterData.Rows[p])
                                        .click(function(e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function(m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) {
                                                alert('This record already exists in your database. To add a new unique record, click on the link below.');
                                                return
                                            };
                                            InitVal();
                                        })
                                    ));
                            }

                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Printer Roles Page
            AddPrinterRolesPage: function() {
                //$.getScript("/Scripts/sources/DevicePrinter/DevicePrinterInfo.js?crc=");
                AjaxSend({
                    ExactUrl: '/DevicePrinter/DevicePrinterInfo?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                    type: 'GET',
                    dataType: 'text',
                    success: function(vData) {
                        $('#UniversalDiv').empty();
                        $('#UniversalDiv').append(vData); //.trigger('create');

                        setTimeout(function() {
                            if ($('#tblCategories tr').length == 0) {
                                var pData = {};
                                pData.IsNew = 1;
                                pData.ID = '';

                                AjaxSend({
                                    url: 'DevicePrinter/setCreateNew',
                                    data: pData,
                                    success: function(vData) {

                                        if ((vData.IsNew || '') == "TRUE") {
                                            AjaxSend({
                                                url: 'DevicePrinter/IsNewEntry',
                                                success: function(vData) {
                                                    if (vData.IsNew == 'TRUE') {

                                                        $("#divCategories").show();
                                                        GetAllCategories('');

                                                    } else {
                                                        PrinterSummary.ShowSumDevicePrinterInfo(vData.DevicePrinterID || '');
                                                    }
                                                }
                                            });
                                        } else {
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

            ViewPrinterRolesPage: function() {
                var pData = {};
                pData.IsNew = 0;
                pData.ID = sessionStorage["ViewID"];

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'DevicePrinter/setCreateNew',
                    data: pData,
                    success: function(vData) {

                        if ((vData.IsNew || '') == "TRUE") {
                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/DevicePrinter/DevicePrinterInfo');
                        } else {
                            isGroupNameClicked = true;
                            if (GetUrlParameters().mode == 'wizard') {
                                AjaxSend({
                                    ExactUrl: '/DevicePrinter/DevicePrinterSummary?mode=wizard',
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function(vData) {
                                        $('#UniversalDiv').empty();
                                        $('#UniversalDiv').append(vData); //.trigger('create');
                                        App.Deferred.fulfill('FQ26E5TP76');
                                    }
                                });
                            } else {
                                //window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary"), "_self");
                                InitVal();
                                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                                if (isPrinterRolePageLoaded == "") {
                                    AjaxSend({
                                        ExactUrl: '/DevicePrinter/DevicePrinterSummary',
                                        type: 'GET',
                                        dataType: 'text',
                                        success: function(vData) {
                                            isPrinterRolePageLoaded = vData;
                                            $('#UniversalDiv').empty();
                                            $('#UniversalDiv').append(vData); //.trigger('create');
                                            //PrinterSummary.ShowSumDevicePrinterInfo(sessionStorage.ViewID);
                                            App.Deferred.fulfill('FQ26E5TP76');
                                        }
                                    });
                                } else {
                                    $('#UniversalDiv').empty();
                                    $('#UniversalDiv').append(isPrinterRolePageLoaded); //.trigger('create');
                                    PrinterSummary.ShowSumDevicePrinterInfo(sessionStorage.ViewID);
                                }
                            }

                        }

                    }
                });
            },

            //Display Device Printer Setup List Page
            DevicePrinterSetup: function(s) {
                AjaxSend({
                    ExactUrl: '/DevicePrinter/PrinterSetup?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                    type: 'GET',
                    dataType: 'text',
                    success: function(vData) {
                        $('#UniversalDiv').empty();
                        $('#UniversalDiv').append(vData); //.trigger('create');
                        App.Deferred.fulfill('FQ26E5TP76');
                        InitVal();
                    }
                });
            },

            //Company Wide Product List
            CompanyWideProductList: function() {
                $('#tblBody').empty().append('<tr><td style="text-align:center" colspan="20">Please wait ...</td></tr>');

                var pData = {};
                pData.SearchKeyword = '';

                AjaxSend({
                    url: 'Organization/getOrganizationProductList',
                    data: pData,
                    success: function(vData) {
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
                        } else {
                            $('#tblBody').append('<tr><td colspan="4" style="text-align:center;">No Result(s) Found</td></tr>');
                        }
                        $('#tblBody').parent().table('refresh');
                    }
                });
            },

            //Search Company Wide Product List
            CompanyWideProductListSearch: function(s) {

                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'Organization/getOrganizationProductList',
                    data: pData,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.OrgProductData.Rows.length > 0) {
                            for (var p = 0; p < vData.OrgProductData.Rows.length; p++) {
                                $('#lstSearchFound').append('<li>' +
                                    '<a style="cursor:auto !important;">' +
                                    '<br><div><span style="font-weight:bold;">' + vData.OrgProductData.Rows[p].ProductName + '</span></p>' +
                                    '<br>' +
                                    '<div>Product No.: ' + vData.OrgProductData.Rows[p].ProductNo + '</p>' +
                                    '</a>' +
                                    '</li>');
                            }

                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Company Wide Product List Page
            CompanyWideProductListPage: function(s) {
                var pData = {};
                pData.OrgID = sessionStorage["OrgID"];

                window.pageSourcePage = $.mobile.activePage;
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                AjaxSend({
                    url: 'Organization/getSubscriberOrganization',
                    data: pData,
                    success: function(vDataOrg) {

                        if (vDataOrg.OrganizationID != "") {
                            pData.IsNew = 1;
                            pData.ID = vDataOrg.OrganizationID;

                            AjaxSend({
                                url: 'Organization/setCreateNew',
                                data: pData,
                                success: function(vData) {
                                    if (isCompanyWideProductPageLoaded == "") {
                                        AjaxSend({
                                            ExactUrl: '/Organization/OrganizationProduct?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                                            type: 'GET',
                                            dataType: 'text',
                                            success: function(vData) {
                                                isCompanyWideProductPageLoaded = vData;
                                                $('#UniversalDiv').empty();
                                                $('#UniversalDiv').append(vData); //.trigger('create');
                                                if (sessionStorage.AddViewLibBCName == "Scan Barcode / Search Product") {
                                                    $('#pageBarcodeSearch').show();
                                                } else if (sessionStorage.AddViewLibBCName == "Subscribe to Company Product List") {
                                                    $('#pageBrowseByOrgOwner').show();
                                                    setTimeout(function() {
                                                        OrgProd.GetAllOrgOwners();
                                                    }, 1000)

                                                } else {
                                                    //$('#pageOrgProductInfoNew').show();
                                                    //$('#pageOrgProductInfo').show();
                                                }
                                                App.Deferred.fulfill('FQ26E5TP76');
                                                isGroupNameClicked = true;
                                            }
                                        });
                                    } else {
                                        $('#UniversalDiv').empty();
                                        $('#UniversalDiv').append(isCompanyWideProductPageLoaded); //.trigger('create');
                                        setTimeout(function() {
                                            OrgProd.SetReqData();
                                        }, 1000)
                                        if (sessionStorage.AddViewLibBCName == "Scan Barcode / Search Product") {
                                            $('#pageBarcodeSearch').show();
                                        } else if (sessionStorage.AddViewLibBCName == "Subscribe to Company Product List") {
                                            $('#pageBrowseByOrgOwner').show();
                                            OrgProd.GetAllOrgOwners();
                                        } else {
                                            //$('#pageOrgProductInfoNew').show();
                                            //$('#pageOrgProductInfo').show();
                                            setTimeout(function() {
                                                OrgProd.ShowProductInfo();
                                            }, 1000)
                                        }
                                    }
                                }
                            });
                        }
                    },
                    error: function(vError) {
                        MessageBox("This feature requires internet connection.",
                            "Requires Internet Connection",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                        );
                        return;
                    }
                });
            },

            //Fowst Product List
            FowstProductList: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isFOWSTProdListPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationFOWSTProduct?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isFOWSTProdListPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isFOWSTProdListPageLoaded); //.trigger('create');
                    OrgFowstProdList.GetOrgFowstProdList();
                }
            },

            //Search Company Wide Product List
            FowstProductListSearch: function(s) {

                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'Organization/getOrgFOWSTList',
                    data: pData,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.OrganizationFOWSTData.Rows.length > 0) {
                            for (var p = 0; p < vData.OrganizationFOWSTData.Rows.length; p++) {
                                $('#lstSearchFound').append('<li>' +
                                    '<a style="cursor:auto !important;">' +
                                    '<br><div><span style="font-weight:bold;">' + vData.OrganizationFOWSTData.Rows[p].Name + '</span></p>' +
                                    '</a>' +
                                    '</li>');
                            }
                        } else {
                            $('#divNoResultsFound').show();
                        }
                        InitVal();
                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display ADD FOWST Product Page
            AddFowstSKUPage: function() {
                AjaxSend({
                    ExactUrl: '/Organization/OrganizationFOWSTProductList?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                    type: 'GET',
                    dataType: 'text',
                    success: function(vData) {
                        $('#UniversalDiv').empty();
                        $('#UniversalDiv').append(vData); //.trigger('create');
                        InitVal();
                        setTimeout(function() {
                            if ($('#lstOrgFOWSTProductList li').length == 0) {
                                LibraryTable.ShowOrgFOWSTProductList();
                            }
                        }, 1500)
                        App.Deferred.fulfill('FQ26E5TP76');
                    }
                });
            },

            //Display Import Products By Excel File Page
            ImportByExcelFilePage: function() {
                $('#UniversalDiv, #divPriceCanvassUploadPIMPage').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isPIMPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/PIMUpload?cache=1&OrgID=' + sessionStorage.OrgID + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isPIMPageLoaded = vData;
                            $('#UniversalDiv, #divPriceCanvassUploadPIMPage').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            InitVal();
                            setTimeout(function() {
                                if ($("#OrgID").val() == "") {
                                    $("#OrgID").val(sessionStorage.OrgID);
                                }
                            }, 1500)
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                } else {
                    $('#UniversalDiv, #divPriceCanvassUploadPIMPage').empty();
                    $('#UniversalDiv').append(isPIMPageLoaded); //.trigger('create');
                    $("#OrgID").val(sessionStorage.OrgID);
                }
            },

            DormantSkusSettings: function() {
                $('#UniversalDiv, #divPriceCanvassUploadPIMPage').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isDormantSettingsLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/PIMProduct/DeactivateDormantSKUSettings?cache=1&OrgID=' + sessionStorage.OrgID + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isDormantSettingsLoaded = vData;
                            $('#UniversalDiv, #divPriceCanvassUploadPIMPage').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isDormantSettingsLoaded); //.trigger('create');
                    DormantSettings.GetData();
                }
            },

            //Display PIM WebForm
            PIMWebForm: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isPIMWebFormPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/PIMUpload/PIMWebForm?cache=1&OrgID=' + sessionStorage.OrgID + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isPIMWebFormPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');

                            setTimeout(function() {
                                if ($("#txtWebFormOrgID").val() == "") {
                                    $("#txtWebFormOrgID").val(sessionStorage.OrgID);
                                }
                            }, 1500)
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isPIMWebFormPageLoaded); //.trigger('create');
                    setTimeout(function() {
                        $("#txtWebFormOrgID").val(sessionStorage.OrgID);
                        PimWebForm.ClearFields();
                        PimWebForm.GetPackingSizeUnit();
                    }, 1000) // setTimeout kay mag.error pag call sa PimWebForm.GetPackingSizeUnit(); dapat maappend sa age page pimwebform
                }
            },

            PIMPriceAdjustment: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isPIMWebFormPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Scripts3/view/PIMPriceAdjustment.html?&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isPIMWebFormPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');

                            require(['/Scripts3/controller/PIMPriceAdjustment.js?crc='], function() {
                                mPIMPriceAdjustment = new App.controller.PIMPriceAdjustment();
                                mPIMPriceAdjustment.Init();
                            });
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isPIMWebFormPageLoaded); //.trigger('create');
                    mPIMPriceAdjustment.Init();
                }
            },

            PIMSalesCaching: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                require(['promise!/Scripts/sources/PIMSalesCaching/controller.js?v=' + assetversion], function() {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(ERP.PIMSalesCaching.controller.element); //.trigger('create');
                });
            },

            ChangeProductAreas: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (mPimChangeProductArea == null)
                    require(['promise!/Scripts/sources/ERP/ChangeProductArea/controller.js?v=' + assetversion], function() {
                        mPimChangeProductArea = ERP.ChangeProductArea;
                        $('#UniversalDiv').html('');
                        $('#UniversalDiv').append(mPimChangeProductArea.element); //.trigger('create');
                        mPimChangeProductArea.Init();
                    });
                else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(mPimChangeProductArea.element);
                }
            },

            UpdateServerIpSettings: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (mUpdateServerIpSettingsPage == "") {
                    AjaxSend({
                        ExactUrl: '/Scripts3/view/UpdateServerIpSettings.html?v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            mUpdateServerIpSettingsPage = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');

                            require(['/Scripts3/controller/UpdateServerIpSettings.js?v=' + assetversion], function() {
                                mUpdateServerIpSettings = new App.controller.UpdateServerIpSettings();
                                mUpdateServerIpSettings.Init();
                            });
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(mUpdateServerIpSettingsPage); //.trigger('create');
                    mUpdateServerIpSettings.Init();
                }
            },

            //Display Single Product Price Adjustment
            SingleProductPriceAdjustmentPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isSingleProdPricePageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/PIMProductPrice?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isSingleProdPricePageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isSingleProdPricePageLoaded); //.trigger('create');
                }
            },

            //Display Bulk Price Adjustment
            BulkPriceAdjustmentPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isBulkPricePageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/PIMProductPrice/UploadDownloadExcelFile?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isBulkPricePageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            setTimeout(function() {
                                UpDownExcelFile.SetReqData();
                            }, 1000)
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isBulkPricePageLoaded); //.trigger('create');
                    UpDownExcelFile.SetReqData();
                }

            },

            //Display Decimals in Quantity Settings Page
            DecimalsInQuantitySettingsPage: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isDecimalsPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Decimals?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isDecimalsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isDecimalsPageLoaded); //.trigger('create');
                    Decimals.GetDecimalsData();
                }
            },

            //Display POS Settings Page
            POSSettingsPage: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isPOSPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/POSSettings?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isPOSPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isPOSPageLoaded); //.trigger('create');
                    POS.GetPOSData();
                }
            },

            //Display Sales Order Settings Page
            SalesOrderSettingsPage: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isSalesOrderPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SystemBehaviorSettings/SalesOrder?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isSalesOrderPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isSalesOrderPageLoaded); //.trigger('create');
                    SystemBehavior.GetSystemBehaviorData();
                }
            },

            //Display Receiving and Transfer Settings Page
            ReceivingAndTransferOutPage: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isReceivingAndTransfersPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SystemBehaviorSettings/ReceivingAndTransferOut?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isReceivingAndTransfersPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isReceivingAndTransfersPageLoaded); //.trigger('create');
                    SystemBehavior.GetSystemBehaviorData();
                }
            },

            //Display Inventory Count Settings Page
            InventoryCount: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isInventoryCountPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SystemBehaviorSettings/InventoryCount?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isInventoryCountPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isInventoryCountPageLoaded); //.trigger('create');
                    InventoryCountAdmin.GetSystemBehavior();
                }
            },

            DailyBeginningBalance: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isDailyBeginningBalanceLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SystemBehaviorSettings/DailyBeginningBalance?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isDailyBeginningBalanceLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            //InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isDailyBeginningBalanceLoaded); //.trigger('create');
                    DailyBegBal.GetSystemBehavior();
                }
            },

            PrintSettings: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isPrintSettingsLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SystemBehaviorSettings/PrintSettings?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isPrintSettingsLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            //InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isPrintSettingsLoaded); //.trigger('create');
                    PrintSettings.LoadRequiredData();
                }
            },

            PurchaseOrderSettings: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isPurchaseOrderSettingsPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Scripts3/view/PurchaseOrderSettings.html?v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isPurchaseOrderSettingsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');

                            require(['/Scripts3/controller/PurchaseOrderSettings.js?v=' + assetversion], function() {
                                mPurchaseOrderSettings = new App.controller.PurchaseOrderSettings();
                                mPurchaseOrderSettings.Init();
                            });
                            //InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isPurchaseOrderSettingsPageLoaded); //.trigger('create');
                    mPurchaseOrderSettings.Init();
                    //SystemBehavior.GetSystemBehaviorData();
                }
            },

            ProductMakerSettings: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isProductMakerSettingsPageLoaded == '') {
                    AjaxSend({
                        ExactUrl: '/Scripts3/view/ProductMakerSettings.html?v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isProductMakerSettingsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');

                            require(['/Scripts3/controller/ProductMakerSettings.js?v=' + assetversion], function() {
                                mProductMakerSettings = new App.controller.ProductMakerSettings();
                                mProductMakerSettings.Init();
                            });
                            //InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty().append(isProductMakerSettingsPageLoaded); //.trigger('create');
                    mProductMakerSettings.Init();
                }
            },

            ErpSettings: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isErpSettingsPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Scripts3/view/ErpSettings.html?v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isErpSettingsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');

                            require(['/Scripts3/controller/ErpSettings.js?v=' + assetversion], function() {
                                mErpSettings = new App.controller.ErpSettings();
                                mErpSettings.Init();
                            });
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isErpSettingsPageLoaded); //.trigger('create');
                    mErpSettings.Init();
                }
            },

            //Display Purchase Order Settings Page
            PurchaseOrder: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isPurchaseOrderPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/PurchaseOrder?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isPurchaseOrderPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isPurchaseOrderPageLoaded); //.trigger('create');
                    PurchaseOrder.GetPOData();
                }
            },

            //Hot Buttons
            HotButtons: function(s) {
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
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Name;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.ModifiedBy;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function(data) {
                        sessionStorage["AddViewLibBCName"] = "View Hot Button";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isHotButtonClicked) {
                            LibraryTable.ViewHotButtonPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Hot Buttons
            HotButtonsSearch: function(s) {
                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'ProductGroup/GetPimMenu',
                    data: pData,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.PimMenuData.Rows.length > 0) {
                            for (var p = 0; p < vData.PimMenuData.Rows.length; p++) {
                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .append($('<div style="font-weight:bold;">')
                                            .text(vData.PimMenuData.Rows[p].Name)
                                        )
                                        .data('data', vData.PimMenuData.Rows[p])
                                        .click(function(e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function(m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) {
                                                alert('This record already exists in your database. To add a new unique record, click on the link below.');
                                                return
                                            };

                                            sessionStorage["AddViewLibBCName"] = "Add Hot Button";
                                            window.location.hash = 'UniversalPage';
                                            InitVal();
                                            LibraryTable.AddHotButtonPage();
                                        })
                                    ));
                            }
                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Hot Buttons Settings Page
            AddHotButtonPage: function() {
                //$.getScript("/Scripts/sources/ProductGroup/MenuList.js?crc=");
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                AjaxSend({
                    ExactUrl: '/ProductGroup?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                    type: 'GET',
                    dataType: 'text',
                    success: function(vData) {
                        $('#UniversalDiv').empty();
                        $('#UniversalDiv').append(vData); //.trigger('create');
                        App.Deferred.fulfill('FQ26E5TP76');
                        InitVal();
                    }
                });
            },

            ViewHotButtonPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isViewHotButtonPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/ProductGroup/MenuTemplate?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isViewHotButtonPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isViewHotButtonPageLoaded); //.trigger('create');
                    ProductGroup.GetProductGroupData();
                }
            },

            //Group Buttons
            GroupSettings: function(s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=groupsettings&isactive=" + $('#selOptionStatus option:selected').val(),
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.GroupName;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Description;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.ModifiedBy;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function(data) {
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

            //Regional Group Settings
            RegionalGroup: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + '&mod=regionalgroupsettings&isactive=' + $('#selOptionStatus').val(),
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.RegionName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return (CBool(data.IsActive) ? 'Active' : 'Inactive');
                            }, "width": "150px", className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.UpdatedByName;
                            }, className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return $.format.date(CJsonDate(data.LastUpdated), 'MMM dd, yyyy hh:mm a');
                            }, className: 'text-center'
                        }

                    ],
                    click: function (data) {
                        RegionalGroupSettingsPopup(data);

                    },
                    disablecols: [0, 2] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Group Settings
            GroupSettingsSearch: function(s) {
                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'Franchisor/GetAllFranchisorGroup',
                    data: pData,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .append($('<div style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].GroupName)
                                        )
                                        .append($('<div>')
                                            .text('Description: ' + vData.Results.Rows[p].Description)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function(e) {
                                            var id = $(this).data('data').ID;
                                            var IsFOWSTExist = false;

                                            $('#tblBody tr').each(function(m_index) {
                                                if ($.trim($(this).attr('ID')) == id) {
                                                    IsFOWSTExist = true;
                                                }
                                            });

                                            if (IsFOWSTExist) {
                                                alert('This record already exists in your database. To add a new unique record, click on the link below.');
                                                return
                                            };

                                            sessionStorage["AddViewLibBCName"] = "Add Group";
                                            window.location.hash = 'UniversalPage';
                                            InitVal();
                                            LibraryTable.AddGroupPage();
                                        })
                                    ));
                            }

                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Group Settings Page
            AddGroupPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isFranchisorPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Franchisor?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isFranchisorPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isFranchisorPageLoaded); //.trigger('create');
                    Franchisor.ShowFranchisorInfo();
                }
            },

            //Display Add Group Settings Page
            ViewGroupPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isFranchisorPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Franchisor?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isFranchisorPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isFranchisorPageLoaded); //.trigger('create');
                    Franchisor.ShowFranchisorInfo();
                }
            },

            //Customers List
            CustomerList: function(s) {
                if (s != undefined) {
                    //NewCustomer.Search(s);
                    //return;
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
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.CustomerName;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.ActivationDate == null) {
                                    return "";
                                }
                                var date = data.ActivationDate;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (CBool(data.DeActivated) ? 'Inactive' : 'Active');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (CBool(data.IsBan) ? 'Yes' : 'No');
                            }
                        }
                    ],
                    click: function(data) {
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
            CustomerListSearch: function(s) {

                var pData = {};
                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryOrganization',
                    data: pData,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            var mDate = new Date();
                            var Products = [];

                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .append($('<br><div><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].CompanyName)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function(e) {

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

                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Customers Page
            AddCustomerPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isAddCustomerPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationCustomer?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isAddCustomerPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isAddCustomerPageLoaded); //.trigger('create');
                    OrgCustomer.ShowCustomerReqData();
                }
            },

            //Display Add Customers Page
            ViewCustomerPage: function() {
                var pData = {};
                pData.IsNew = 0;
                pData.ID = sessionStorage.ViewID;
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                AjaxSend({
                    url: 'OrganizationShort/setCreateNew',
                    data: pData,
                    success: function(vData) {
                        isGroupNameClicked = true;
                        if ((vData.IsNew || '') == "TRUE") {
                            window.IsPageLoaded = false;
                            //ChangePage('/OrganizationShort/OrganizationInfo');
                            window.location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/OrganizationShort/OrganizationInfo');
                        } else {
                            //ChangePage('/OrganizationShort/OrganizationSummary');
                            if (isViewCustomerPageLoaded == "") {
                                AjaxSend({
                                    ExactUrl: '/OrganizationShort/OrganizationSummary?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function(vData) {
                                        isViewCustomerPageLoaded = vData;
                                        $('#UniversalDiv').empty();
                                        $('#UniversalDiv').append(vData); //.trigger('create');
                                        App.Deferred.fulfill('FQ26E5TP76');
                                        InitVal();
                                    }
                                });
                            } else {
                                $('#UniversalDiv').empty();
                                $('#UniversalDiv').append(isViewCustomerPageLoaded); //.trigger('create');
                                OrgShort.ShowOrgShortSummary();
                            }
                        }
                    }
                });
            },

            //Display New Customer Page
            NewCustomerPage: function() {
                $('#DTRUniversalDiv').empty(); // delete the content from dtr since supplier, manpower, vendor and co-user are same
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isCustomerPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/OrganizationShort/NewCustomerPage?cache=1&OrgID=' + sessionStorage.OrgID + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isCustomerPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();

                            if (typeof NewCustomer != "undefined") {
                                NewCustomer.LoadData();
                            }
                        }
                    });
                } else {
                    $('#UniversalDiv').text('Loading, please wait ...');
                    setTimeout(function() {
                        $('#UniversalDiv').empty();
                        $('#UniversalDiv').append(isCustomerPageLoaded); //.trigger('create');
                        NewCustomer.LoadData();
                    }, 500);
                }
            },

            //New Search Customer
            NewCustomerSearch: function(s) {
                NewCustomer.Search(s);
            },

            //Supplier List
            SupplierList: function(s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=supplier&isactive=" + $('#selOptionStatus').val(),
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.SupplierName;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.VendorCode;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (CBool(data.DeActivated) ? 'Inactive' : 'Active');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (CBool(data.IsBan) ? 'Yes' : 'No');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.ModifiedBy;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.LastUpdated;
                            }
                        },
                    ],
                    click: function(data) {
                        if (data.SupplierID.toLowerCase() == 'cd71a0f7-08a9-4601-876e-5f13c064f873') {
                            NewMessageBox('Sorry! You are not allowed to edit default customer details.', 'Message');
                            return;
                        }
                        sessionStorage["AddViewLibBCName"] = "View Supplier / Vendor";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.SupplierID; //data.ID;
                        sessionStorage["txtOrgSupSupplierID"] = data.SupplierID; //data.ID;
                        sessionStorage["btnOrgSupSupplier"] = data.SupplierName;
                        if (data.IsOrganization == null)
                            data.IsOrganization = true;
                        sessionStorage.SupplierType = (CBool(data.IsOrganization) ? 'Org' : 'Person');
                        if (isSupplierClicked) {
                            //LibraryTable.ViewSupplierPage();
                            //LibraryTable.NewCustomerPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Supplier List
            SupplierListSearch: function(s) {

                var pData = {};
                pData.keywords = s;
                pData.FilterBy = 'Org';

                AjaxSend({
                    url: 'Browser/LibraryOrganizationAndPerson',
                    data: pData,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            var mDate = new Date();
                            var Products = [];

                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .text(vData.Results.Rows[p].CompanyName)
                                        .data('data', vData.Results.Rows[p])
                                        .click(function(e) {
                                            var selectedData = $(this).data('data');
                                            selectedData.IsCustomerSettings = (sessionStorage.LibraryName == 'Customer / Client List' ? true : false);

                                            NewMessageBox('Are you sure you want to add <b>' + selectedData.CompanyName + '</b> as your supplier?', 'Message', [
                                                {
                                                    Text: 'Yes',
                                                    Click: function () {
                                                        $('#panAddLibrary').modal('hide');
                                                        Bs.Loading('show', 'Please wait while adding selected supplier ...');

                                                        AjaxSend({
                                                            url: 'OrganizationShort/GetDataOfSelectedPublicOrgPerson',
                                                            data: selectedData,
                                                            success: function(e) {
                                                                var orgdata = e.Results.Rows[0];

                                                                var pData = {
                                                                    isCreateFromExistingRecord: 1,
                                                                    Telphone: [],
                                                                    Mobile: [],
                                                                    Email: [],
                                                                    Website: [],
                                                                    Outlet: [],
                                                                    ContactPerson: [],
                                                                    ContactPersonTelphone: [],
                                                                    ContactPersonMobile: [],
                                                                    ContactPersonEmail: [],
                                                                    OrgAffiliation: [],
                                                                    OrgAffiliatedPerson: [],
                                                                    CompanyID: CStr2(orgdata.ID),
                                                                    ExistingCustomerID: CStr2(selectedData.ID),
                                                                    CompanyName: $.trim(CStr2(orgdata.CompanyName)),
                                                                    Tin: CStr2(orgdata.TinNumber),
                                                                    OrgID: sessionStorage.OrgID,
                                                                    DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a'),
                                                                    Status: "Active", // Default to Active
                                                                    IsCustomerSettings: (sessionStorage.LibraryName == 'Customer / Client List' ? true : false),
                                                                    PriceLevel: [],
                                                                    NewVendorPrices: []
                                                                }

                                                                if (sessionStorage.LibraryName.indexOf('Customer') != -1)
                                                                    pData.SaveAsCustomer = 1;

                                                                if (sessionStorage.LibraryName.indexOf('Supplier') != -1) {
                                                                    pData.SaveAsSupplier = 1;
                                                                    pData.SupplierInfo = {
                                                                        OrgSupplierID: "",
                                                                        SupplierID: selectedData.ID,
                                                                        ActivationDate: $.format.date(new Date(), "MM/dd/yyyy"),
                                                                        IsDeactivated: 0,
                                                                        DeactivationDate: '//',
                                                                        DeactivationReason: '',
                                                                        Comments: '',
                                                                        IsBan: 0,
                                                                        CreditTermsID: null,
                                                                        CreditLimit: CDec2(0),
                                                                        CreditBegBalance: CDec2(0),
                                                                        CreditBalanceAsOf: null
                                                                    };
                                                                }


                                                                AjaxSend({
                                                                    url: 'OrganizationShort/SaveNewOrgProspect',
                                                                    data: pData,
                                                                    success: function(vData) {
                                                                        Bs.Loading('hide');
                                                                        if (IsGuid(vData.Results)) {
                                                                            WebToast('Successfully added', 'top');
                                                                            LibraryTable.SupplierList();
                                                                        } else {
                                                                            NewMessageBox(vData.Results, 'Message');
                                                                        }
                                                                    }
                                                                })
                                                            }
                                                        })
                                                    }
                                                }, { Text: 'No' }
                                            ])
                                        })
                                    ));
                            }

                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Supplier Page
            AddSupplierPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isSupplierPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationSupplier?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isSupplierPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');

                            $('#txtOrgSupSupplierID').val(sessionStorage.txtOrgSupSupplierID);
                            $('#btnOrgSupSupplier').text(sessionStorage.btnOrgSupSupplier).button('refresh');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isSupplierPageLoaded); //.trigger('create');
                    $('#txtOrgSupSupplierID').val(sessionStorage.txtOrgSupSupplierID);
                    $('#btnOrgSupSupplier').text(sessionStorage.btnOrgSupSupplier).button('refresh');
                    OrgSup.SetReqData();
                }
            },

            //Display View Supplier Page
            ViewSupplierPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isSupplierPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationSupplier?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isSupplierPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');

                            $('#txtOrgSupSupplierID').val(sessionStorage.txtOrgSupSupplierID);
                            $('#btnOrgSupSupplier').text(sessionStorage.btnOrgSupSupplier).button('refresh');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isSupplierPageLoaded); //.trigger('create');
                    $('#txtOrgSupSupplierID').val(sessionStorage.txtOrgSupSupplierID);
                    $('#btnOrgSupSupplier').text(sessionStorage.btnOrgSupSupplier).button('refresh');
                    OrgSup.ShowOrgSupplierInfo();
                }
            },

            //Sales Team
            SalesTeam: function(s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=salesteam&type=team",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.TeamName;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Description;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function(data) {
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
            SalesTeamSearch: function(s) {

                var pData = {};
                pData.SearchKeyword = s;
                pData.Type = 'team';

                AjaxSend({
                    url: 'crm/GetAllTeams',
                    data: pData,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {

                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .append($('<br><div><span style="font-weight:bold;">')
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

                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },

            KnowledgeManagementAndQuestionaire: function(s) {

                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'KnowledgeManagementAndQuestionaire/GetAllKnowledgeManagementAndQuestionaire',
                    data: pData,
                    success: function(vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {

                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .append($('<br><div><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].CaseNo)
                                        )
                                        .append($('<br><div><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].CaseDescription)
                                        )
                                        .append($('<br><div><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Solution)
                                        )
                                        .append($('<br><div><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Topic)
                                        )
                                        .append($('<br><div><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Product)
                                        )
                                        .append($('<br><div><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Language)
                                        )
                                        .append($('<br><div><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].EditedBy)
                                        )
                                        .append($('<br><div><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].LastUpdated)
                                        )
                                    ));
                            }

                        } else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound'); //.listview('refresh');
                        Reposition();
                    }
                });
            },
            //Display Add CRM Page
            AddCRMPage: function() {
                $('#DTRUniversalDiv').empty();
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isAddCRMPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/CRM?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isAddCRMPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();

                            if (typeof CRM != 'undefined') {
                                CRM.ShowCRMData();
                            }
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isAddCRMPageLoaded); //.trigger('create');
                }
            },

            //Display Add CRM Page
            ViewCRMPage: function() {
                $('#DTRUniversalDiv').empty();
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isAddCRMPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/CRM?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isAddCRMPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();

                            if (typeof CRM != 'undefined') {
                                CRM.ShowCRMData();
                            }
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isAddCRMPageLoaded); //.trigger('create');
                    CRM.ShowCRMData();
                }
            },

            ShowOrgFOWSTProductList: function() {
                var pData = {};
                var pParams = GetUrlParameters();

                pData.FowstID = pParams.OrgFowstID;
                pData.SearchKeyword = $("#txtOrgFowstProductSearch").val();

                AjaxSend({
                    url: "Organization/getOrganizationFowstProductList",
                    data: pData,
                    success: function(vData) {
                        LoadOrgFOWSTProductList(vData.OrgFowstProductData.Rows);
                    }
                });
            },

            PageItemModifiers: function() {
                var imID = CStr2(sessionStorage.ItemModifierID)
                $('#divModifierContent').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isAddItemModifiersPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/ItemModifiers?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isAddItemModifiersPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            //$('#popItemModifierName').popup('open');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isAddItemModifiersPageLoaded); //.trigger('create');
                    ItemModifier.Clear();
                }
            },

            SearchItemModifiers: function(vKeywords) {
                var url = "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=modifier&isdeactivate=" + $('#selOptionStatus option:selected').val()
                if (CStr2(vKeywords) != '') {
                    $('#tblSysAdminDataTable').DataTable().ajax.url(url)
                    tblSysAdminDataTable.search($.trim(vKeywords)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: url,
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function(data, type, full, meta) {
                                return data.RowNum + '.';
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Name;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (CStr2(data.Type) == '' ? 'Return For Rebates' : CStr2(data.Type));
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (data.IsActive == null || CBool(data.IsActive) ? 'Active' : 'Inactive');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.UpdatedByName;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function(data) {
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ItemModifierData"] = JSON.stringify(data);
                        sessionStorage["AddViewLibBCName"] = "Item Modifier Parameters";
                        //$('#popModifier').show();
                        if (isItemModifierClicked) {
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
                    success: function(vData) {
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
                                AfterRow: function(e) {
                                    e.Row.css('cursor', 'pointer');
                                    e.Row.click(function(e1) {
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
                            pTable; //.trigger('create');

                            var table = $("table").stupidtable({
                                "date": function(a, b) {
                                    // Get these into date objects for comparison.

                                    aDate = new Date(a);
                                    bDate = new Date(b);

                                    return aDate - bDate;
                                }
                            });
                        } else {
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

            AddItemModifier: function() {
                var pName = $('#txtItemModifierNewName').val();

                if (pName.trim() == '') {
                    $('#divErrorItemModifier').show();
                    return false;
                }

                $('#divErrorItemModifier').hide();

                mModel.Add({
                    Name: pName,
                    success: function(vData) {
                        //MessageBox('Item modifier was successfully created.');
                        $('#popItemModifierName').popup('close');
                        $('#txtItemModifierNewName').val('');
                        LibraryTable.SearchItemModifiers();
                    },
                    error: function(vData) {
                        if (typeof vData == 'string')
                            MessageBox(vData);
                        else
                            MessageBox(JSON.stringify(vData));
                    }
                });
            },

            //Payment Terms List
            PaymentTermsList: function(s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=terms",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Name;
                            },
                            "width": "200px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Description;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Days;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (CBool(data.IsActive) ? 'Yes' : 'No');
                            },
                            "width": "50px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.ModifiedBy;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function(data) {
                        sessionStorage["AddViewLibBCName"] = "Payment Terms";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isPaymentTermsClicked) {
                            LibraryTable.AddPaymentTerms();
                        }
                        InitVal();
                    },
                    disablecols: [0, 2, 3, 4] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Display Payment Terms Page
            AddPaymentTerms: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isPaymentTermsPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/PaymentTerms?cache=1&OrgID=' + sessionStorage.OrgID + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isPaymentTermsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isPaymentTermsPageLoaded); //.trigger('create');
                    PaymentTerms.Load();
                }
            },

            //Display Acc Adjustments Page
            AccountAdjustmentsPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isAccAdjustmentPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/AccountAdjustments?cache=1&OrgID=' + sessionStorage.OrgID + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isAccAdjustmentPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isAccAdjustmentPageLoaded); //.trigger('create');
                    Adjustment.Load();
                }
            },

            //Price Level Custom List
            PriceLevelCustom: function(s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=pricelevel",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.PriceLevel;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (CStr2(data.FromDate) != '' && CStr2(data.ToDate) != '') {
                                    return 'From: ' + $.format.date(new Date(CJsonDate(data.FromDate)), 'MMM dd yyyy hh:mm a') + ' <br> To: ' + $.format.date(new Date(CJsonDate(data.ToDate)), 'MMM dd yyyy hh:mm a');
                                } else {
                                    return 'Anytime';
                                }
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (CBool(data.IsActive) ? 'Yes' : 'No');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.ModifiedBy;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function(data) {
                        if (CBool(data.IsAllowedToEdit) == true) {
                            $('#panAddPriceLevel').modal('show');
                            $('#txtPriceLevelCustomID').val(data.ID);
                            $('#txtPriceLevelName').val(data.PriceLevel).focus();
                            $('[name="rad-pricelevel"][value="' + data.PriceLevelType + '"]').prop('checked', 1); //.checkboxradio('refresh');
                            $('#selPriceLevelActive').prop('checked', CInt2(data.IsActive)); //.slider('refresh');

                            if (CStr2(data.FromDate) != '' && CStr2(data.ToDate) != '') {
                                $('#chbPriceLevelAvailability').prop('checked', 1);
                                $('#divPLAvailability').show();
                                $('#txtPLFromDate').val($.format.date(new Date(CJsonDate(data.FromDate)), 'yyyy-MM-ddTHH:mm'));
                                $('#txtPLToDate').val($.format.date(new Date(CJsonDate(data.ToDate)), 'yyyy-MM-ddTHH:mm'));
                            } else {
                                $('#chbPriceLevelAvailability').removeAttr('checked');
                                $('#divPLAvailability').hide();
                                $('#txtPLFromDate').val($.format.date(new Date(), 'yyyy-MM-ddT00:00'));
                                $('#txtPLToDate').val($.format.date(new Date(new Date().setDate(new Date().getDate() + 1)), 'yyyy-MM-ddT23:59'));
                            }
                        } else {
                            WebToast('You\'re not allowed to edit the default price level', 'top');
                        }
                    },
                    disablecols: [2] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            SalesRepresentative: function(s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=salesrep",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Name;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return (CBool(data.IsActive) ? 'Yes' : 'No');
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.ModifiedBy;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function(data) {
                        $('#panAddSalesRepresentative').modal('show');
                        $('#txtSalesRepresentativeCustomID').val(data.ID);
                        $('#txtSalesRepresentativeName').val(data.Name).focus();
                        $('#selSalesRepresentativeActive').prop('checked', CBool(data.IsActive)); //.slider('refresh');
                    },
                    disablecols: [2] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            SearchKnowledgeManagementAndQuestionaire: function(vKeywords) {

                var pData = {};
                pData.SearchKeyword = vKeywords;

                AjaxSend({
                    url: 'KnowledgeManagementAndQuestionaire/GetAllKnowledgeManagementAndQuestionaire',
                    data: pData,
                    success: function(vData) {

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
                                AfterRow: function(e) {
                                    e.Row.css('cursor', 'pointer');
                                    e.Row.click(function(e1) {
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
                            pTable; //.trigger('create');

                            var table = $("table").stupidtable({
                                "date": function(a, b) {
                                    // Get these into date objects for comparison.

                                    aDate = new Date(a);
                                    bDate = new Date(b);

                                    return aDate - bDate;
                                }
                            });
                        } else {
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

            GetKnowledgeManagementNumber: function() {
                AjaxSend({
                    url: "KnowledgeManagementAndQuestionaire/GetLastKMNumber",
                    success: function(vData) {
                        if (vData.Results.Rows.length > 0) {
                            $('#txtCaseID').val(vData.Results.Rows[0].LastNumber);
                        } else {
                            $('#txtCaseID').val(0);
                        }
                    }
                });
            },
            //Display Loyalty Program Page
            LoyaltyProgram: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isLoyaltyProgramPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SystemBehaviorSettings/LoyaltyProgram?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isLoyaltyProgramPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isLoyaltyProgramPageLoaded); //.trigger('create');
                    LoyaltyProgram.GetSystemBehavior();
                }
            },

            //Printers
            Printers: function(s) {
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
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return CStr2(data.Brand);
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                var p;

                                if (CStr2(data.Brand) + CStr2(data.Model) == '')
                                    p = data.PrinterType;
                                else
                                    p = data.Model;
                                return CStr2(p);
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.PrinterName;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return CStr2(data.SerialNum);
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return CStr2(data.ConnectionType);
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return CStr2(data.PrinterIPAddress);
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.FowstName;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.DeviceNames;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.PrinterRole;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return ''; // + '<button>View</button>';
                            }
                        }
                        //{
                        //    "data": function (data, type, full, meta) {
                        //        if (data.LastUpdated == null) {
                        //            return "";
                        //        }
                        //        var date = data.LastUpdated;
                        //        return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                        //    }
                        //}
                    ],
                    click: function(data) {
                        sessionStorage["AddViewLibBCName"] = "View Printer";
                        sessionStorage['ViewID'] = data.ID;
                        sessionStorage["FowstTitle"] = data.Name;
                        sessionStorage["IsNewSetup"] = data.IsNewSetup;

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

            AddPrinterPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isPrinterPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Scripts3/view/Printer.html?v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isPrinterPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            //Fowst.ShowFowstInfo(sessionStorage.ViewID || '');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                            InitVal();
                            require(['/Scripts3/controller/Printer.js?v=' + assetversion], function() {
                                mPrinters = new App.controller.Printer();
                                mPrinters.Init();
                                mPrinters.LoadData({ ID: sessionStorage['ViewID'], IsNewSetup: sessionStorage['IsNewSetup'] });
                            });
                        }
                    });
                } else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isPrinterPageLoaded); //.trigger('create');
                    mPrinters.Init();
                    mPrinters.LoadData({ ID: sessionStorage['ViewID'], IsNewSetup: sessionStorage['IsNewSetup'] });
                }
            },

            //TableView Settings
            TableView: function(s) {
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
                            "data": function(data, type, full, meta) {
                                return data.RowNum;
                            },
                            width: "30px"
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Name;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.Locations;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                return data.ModifiedBy;
                            }
                        },
                        {
                            "data": function(data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function(data) {
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

            AddTableViewPage: function() {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isTableViewPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/TableView/Settings?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isTableViewPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            //Fowst.ShowFowstInfo(sessionStorage.ViewID || '');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isTableViewClicked = true;
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isTableViewPageLoaded); //.trigger('create');
                    TableView.LoadData();
                }
            },

            //Display Ecommerce General Settings Page
            EcommerceGeneralSettingsPage: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isEcommerceGeneralSettingsPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Ecommerce/GeneralSettings?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isEcommerceGeneralSettingsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isEcommerceGeneralSettingsPageLoaded); //.trigger('create');
                    EcommerceSettings.LoadData();
                }
            },

            //Display Ecommerce Vendor Portal Settings Page
            EcommerceVendorPortalSettingsPage: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isEcommerceVendorPortalSettingsPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Ecommerce/VendorPortalSettings?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isEcommerceVendorPortalSettingsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isEcommerceVendorPortalSettingsPageLoaded); //.trigger('create');
                    EcommerceVendorPortalSettings.LoadData();
                }
            },

            //Display Ecommerce Banner Settings Page
            EcommerceBannerSettingsPage: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isEcommerceBannerSettingsPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Ecommerce/BannerSettings?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isEcommerceBannerSettingsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isEcommerceBannerSettingsPageLoaded); //.trigger('create');
                    EcommerceBanner.LoadBanner();
                }
            },

            FlushingReportPage: function(s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isFlushingReportPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Scripts/sources/FlushingReport/FlushingReport.html?v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function(vData) {
                            isFlushingReportPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData); //.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isFlushingReportPageLoaded); //.trigger('create');
                    //EcommerceSettings.LoadData();
                }
            },
            AutoDataArchivingPage: function (s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (mAutoDataArchiving == null) {
                    require(['promise!/Scripts/sources/AutoDataArchiving/AutoDataArchiving.js?v=' + assetversion], function() {
                        mAutoDataArchiving = new App.controller.AutoDataArchiving();
                        $('#UniversalDiv').empty();
                        $('#UniversalDiv').append(mAutoDataArchiving.element);//.trigger('create');
                        InitVal();
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(mAutoDataArchiving.element);//.trigger('create');
                    //EcommerceSettings.LoadData();
                }
            },

            WebPosSettingsPage: function (s) {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isWebPosSettingsPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Scripts/sources/WebPosSettings/WebPosSettings.html?v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isWebPosSettingsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isWebPosSettingsPageLoaded);//.trigger('create');
                    //EcommerceSettings.LoadData();
                }
            },

            //Hot Buttons
            OnlineStoreCategories: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=storecategories",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Name;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return (CBool(data.IsDefault) ? 'Yes' : 'No');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.ModifiedBy;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Store Category";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isPrinterRoleClicked) {
                            LibraryTable.ViewOnlineStoreCategoriesPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Search Hot Buttons
            OnlineStoreCategoriesSearch: function (s) {
                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'OnlineStoreCategories/GetPimMenu',
                    data: pData,
                    success: function (vData) {
                        $('#lstSearchFound').empty();
                        $('#divNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .append($('<span>')
                                        .append($('<div style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Name)
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

                                            sessionStorage["AddViewLibBCName"] = "Add Store Category Template";
                                            window.location.hash = 'UniversalPage';
                                            InitVal();
                                            LibraryTable.AddOnlineStoreCategoriesPage();
                                        })
                                    ));
                            }
                        }
                        else {
                            $('#divNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#lstSearchFound');//.listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Hot Buttons Settings Page
            AddOnlineStoreCategoriesPage: function () {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isAddOnlineStoreCategoriesPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/OnlineStoreCategories?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddOnlineStoreCategoriesPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isAddOnlineStoreCategoriesPageLoaded);//.trigger('create');
                }
            },

            ViewOnlineStoreCategoriesPage: function () {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isViewOnlineStoreCategoriesPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/OnlineStoreCategories/MenuTemplate?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isViewOnlineStoreCategoriesPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isViewOnlineStoreCategoriesPageLoaded);//.trigger('create');
                    setTimeout(function () {
                        CreateCategories.GetProductGroupData();
                    }, 100)
                }
            },

            ProprietaryBarcodeType: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=barcodetype",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.BarcodeType;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.StickerDimensions;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                var date = data.LastUpdated;
                                return $.format.date(new Date(parseInt(date.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        }
                    ],
                    click: function (data) {
                        $('#panAddBarcodeType').modal('show');
                        $('#divRemoveBarcodeType').show();
                        $('#txtBarcodeTypeID').val(data.ID);
                        $('#txtBarcodeTypeName').val(data.BarcodeType).focus();
                        $('#txtBarcodeStickerDimensions').val(data.StickerDimensions);
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            Centers: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + '&mod=centers&type=' + sessionStorage.LibraryName + '&isactive=' + $('#selOptionStatus').val(),
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Name;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Description;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return $.format.date(new Date(parseInt(data.DateCreated.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.ModifiedDate == null) {
                                    return '';
                                }
                                return $.format.date(new Date(parseInt(data.ModifiedDate.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.LastModifiedBy;
                            }
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View " + $.trim(sessionStorage.LibraryName.split(' ')[0]) + " Center";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isCentersClicked) {
                            LibraryTable.CentersPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            //Display Centers Page
            CentersPage: function () {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isCentersPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Centers?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isCentersPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isCentersPageLoaded);//.trigger('create');
                    Centers.Add();
                    Centers.View();
                }
            },

            //Display General Settings Page
            GeneralSettingsPage: function () {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isGeneralSettingsLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SetupChecklist/GeneralSettings?cache=1&OrgID=' + sessionStorage.OrgID + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isGeneralSettingsLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isGeneralSettingsLoaded);//.trigger('create');
                    GeneralSettings.Load();
                }
            },

            VDMGeneralSettingsPage: function () {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isGeneralSettingsLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/VendingMachine/GeneralSettings?cache=1&OrgID=' + sessionStorage.OrgID + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isGeneralSettingsLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#UniversalDiv').empty();
                    $('#UniversalDiv').append(isGeneralSettingsLoaded);//.trigger('create');
                    VDMGeneralSettings.Load();
                }
            },

            VDMModels: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=vdmmodels",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return CStr2(data.Name);
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.NoOfMotors;
                            }, className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.NoOfDirectConnMotors;
                            }, className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.CreatedByName + ' on ' + $.format.date(new Date(parseInt(data.DateCreated.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }, className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.ModifiedByName == '' ? '' : data.ModifiedByName + ' on ' + (data.DateModified == null ? '' : $.format.date(new Date(parseInt(data.DateModified.substr(6))), 'MMM dd, yyyy hh:mm a'));
                            }, className: 'text-center'
                        }
                    ],
                    click: function (data) {
                        LibraryTable.VDMModelPopup(data);
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            VMModelWithControllerListing: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=vmmodelwithcontrollerlisting",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return CStr2(data.Name);
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.NoOfBoards;
                            }, className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.MainBoardName;
                            }, className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.ModifiedByName == '' ? '' : data.ModifiedByName + ' on ' + (data.DateModified == null ? '' : $.format.date(new Date(parseInt(data.DateModified.substr(6))), 'MMM dd, yyyy hh:mm a'));
                            }, className: 'text-center'
                        }
                    ],
                    click: function (data) {
                        LibraryTable.VDMModelWithListingPopup(data);
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            VDMPlanogramTemplate: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=vdmplanogramtemplate",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return CStr2(data.Name);
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.ModifiedByName == '' ? '' : data.ModifiedByName + ' on ' + (data.DateModified == null ? '' : $.format.date(new Date(parseInt(data.DateModified.substr(6))), 'MMM dd, yyyy hh:mm a'));
                            }, className: 'text-center'
                        }
                    ],
                    click: function (data) {
                        LibraryTable.VDMPlanogramTemplatePopup(data);
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            VDMModelPopup: function (data) {
                var sensordevicelist = [
                    {
                        //In Alphabetical Order
                        Container: '#divSensorList',
                        Group: 'chModelSensors',
                        List: ['Accelerometer(s)', 'Drop Sensor(s)', 'Flood Sensor(s)', 'Gyroscope(s)', 'Humidity Sensor(s)', 'Temperature Sensor(s)', 'Ultrasonic Distance Sensor(s)', 'VDM AC Main Power Meter(s)', 'VMC Board DC Power Meter(s)']
                    },
                    {
                        Container: '#divDeviceList',
                        Group: 'chModelDevices',
                        List: ['Auxillary Motor(s)', 'Bill Acceptor(s)', 'Coin Acceptor(s)', 'Coin Dispenser(s)', 'Coin Hopper(s)', 'DC Power Switch 12V', 'DC Power Switch 24V', 'EXT Light(s) 12V', 'General Purpose Output (GPO) Pin(s)', 'Lock(s)', 'Relay(s)', 'Sliding Door(s)']
                    }
                ]

                for (var i = 0 in sensordevicelist) {
                    var container = $(sensordevicelist[i].Container).empty();

                    for (var d = 0 in sensordevicelist[i].List) {
                        var value = sensordevicelist[i].List[d];
                        container.append($('<div class="row mb-3 align-items-center">')
                            .append($('<div class="col-auto">')
                                .append('<input data-role="none" type="checkbox" name="' + sensordevicelist[i].Group + '" value="' + value + '" class="form-check-input" id="chb' + value.replace(/\s+/g, '') + '" />')
                            )
                            .append($('<div class="col">')
                                .append('<label for="chb' + value.replace(/\s+/g, '') +'" class="form-check-label">' + value +'</label>')
                            )
                            .append($('<div class="col-5">').append('<input data-role="none" type="number" name="txtModelSD" class="form-control" id="txt' + value.replace(/\s+/g, '') +'" placeholder="Enter Quantity" />'))
                        )
                    }
                }

                $('#txtVDMName, #selVDMModelID, #txtVDMDescription, #txtVDMNoOfSpringCoilMotors, #txtVDMNoOfDirectConnMotors, #txtVDMUUID').val('');
                $('#selVDMActive').prop('checked', 1);
                $('[name="chModelSensors"]').prop('checked', 0);
                $('[name="chModelDevices"]').prop('checked', 0);
                $('[name="txtModelSD"]').val('');
                $('#popVDMModel').modal('show');

                if (data != undefined) {
                    $('#txtVDMName').val(data.Name);
                    $('#txtVDMDescription').val(data.Description);
                    $('#selVDMModelID').val(data.VCMModelID);
                    $('#txtVDMNoOfSpringCoilMotors').val(data.NoOfMotors);
                    $('#txtVDMNoOfDirectConnMotors').val(data.NoOfDirectConnMotors);
                    $('#txtVDMUUID').val(data.UUID);
                    $('#selVDMActive').prop('checked', CInt2(data.IsActive));

                    AjaxSend({
                        url: 'VendingMachine/GetBoardSensorAndDevice',
                        data: { BoardID: data.ID },
                        success: function (e) {
                            var vRows = e.Result.Rows;

                            for (var i = 0 in vRows) {
                                var inp = $('input[value="' + vRows[i].Description + '"]');

                                inp.prop('checked', 1);
                                inp.parent().next().find('input').val(vRows[i].Qty);
                            }
                        }
                    })
                } else {
                    data = {}
                }

                $('#selVDMType').unbind().change(function () {
                    var type = $(this).val();

                    if (type == 'Model A') {
                        $('#divModelA').pageshow();
                        $('#txtVDMNoOfPhySelMotor, #txtVDMNoOfPhyNavMotor, #txtVDMNoOfMotorActuators').val('');
                    } else if (type == 'Model B') {
                        $('#divModelB').pageshow();
                        $('#txtVDMNoOfMotor').val('');
                    }
                })

                $('#btnVDMSave').unbind().click(function () {
                    var type = $('#selVDMType').val();
                    var arrSensors = [], arrDevices = [];

                    if ($('#txtVDMName').val() == '') {
                        NewMessageBox('Please enter Model Name');
                        return;
                    }

                    //if (type == 'Model A') {
                    //if (CDec2($('#txtVDMNoOfSpringCoilMotors').val()) == 0) {
                    //        NewMessageBox('Please enter the number of spring coil motors.');
                    //        return;
                    //}

                    //if (CDec2($('#txtVDMNoOfDirectConnMotors').val()) == 0) {
                    //        NewMessageBox('Please enter the number of direct connection motors.');
                    //        return;
                    //}


                    $('[name="chModelSensors"]:checked').each(function () {
                        var dis = $(this);

                        arrSensors.push({
                            Type: 'Sensor',
                            Description: dis.val(),
                            Qty: dis.parent().next().next().find('input').val()
                        })
                    })

                    $('[name="chModelDevices"]:checked').each(function () {
                        var dis = $(this);

                        arrDevices.push({
                            Type: 'Device',
                            Description: dis.val(),
                            Qty: dis.parent().next().next().find('input').val()
                        })
                    })
                    //} else if (type == 'Model B') {
                    //    if (CDec2($('#txtVDMNoOfPhySelButtons').val()) == 0) {
                    //        NewMessageBox('Please enter the number of physical selection buttons.');
                    //        return;
                    //    }

                    //    if (CDec2($('#txtVDMNoOfPhyNavButtons').val()) == 0) {
                    //        NewMessageBox('Please enter the number of physical navigation button.');
                    //        return;
                    //    }

                    //    if (CDec2($('#txtVDMNoOfMotorActuators').val()) == 0) {
                    //        NewMessageBox('Please enter the number of motor actuators.');
                    //        return;
                    //    }

                    //}

                    AjaxSend({
                        url: '/VendingMachine/SaveVDM',
                        data: {
                            data: {
                                ID: CStr2(data.ID),
                                Type: $.trim(type),
                                Name: $.trim($('#txtVDMName').val()),
                                Description: $.trim($('#txtVDMDescription').val()),
                                VCMModelID: $.trim($('#selVDMModelID').val()),
                                NoOfSpringCoilMotors: $.trim($('#txtVDMNoOfSpringCoilMotors').val()),
                                NoOfDirectConnMotors: $.trim($('#txtVDMNoOfDirectConnMotors').val()),
                                UUID: $.trim($('#txtVDMUUID').val()),
                                NoOfPhysicalSelectionButtons: $.trim($('#txtVDMNoOfPhySelButtons').val()),
                                NoOfPhysicalNavigationButtons: $.trim($('#txtVDMNoOfPhyNavButtons').val()),
                                NoOfMotorActuators: $.trim($('#txtVDMNoOfMotorActuators').val()),
                                IsActive: CInt2($('#selVDMActive').is(':checked')),
                                Sensors: arrSensors,
                                Devices: arrDevices
                            }
                        }, success: function (e) {
                            if (e.Result == 'saved') {
                                WebToast('Successfully saved');
                                $('#popVDMModel').modal('hide');
                                LibraryTable.VDMModels();
                            } else NewMessageBox(e.Error, 'Error');
                        }
                    })
                })
            },

            VDMModelWithListingPopup: function (data) {
                $('#txtVMName, #txtVDMNoOfBoards, #selMainBoard').val('');
                $('#divMainBoardMapping').empty();
                var listDaughterBoard = $('#listDaughterBoard').empty();

                Bs.Loading('show')
                AjaxSend({
                    url: 'Browser/LibraryVendingMachineBoards',
                    success: function (e) {
                        Bs.Loading('hide')

                        var selMainBoard = $('#selMainBoard').empty();

                        selMainBoard.append(new Option('Please select', ''));

                        for (var i in e.Results.Rows) {
                            selMainBoard.append(new Option(e.Results.Rows[i].Name, e.Results.Rows[i].ID));
                        }

                        selMainBoard.unbind().change(function () {
                            if ($(this).val() == '') return;

                            GetPredefinedSensorsAndDevices($(this).val(), function (html) {
                                $('#divMainBoardMapping').empty().append(html);
                            })
                        })

                        if (!$.isEmptyObject(data)) {
                            $('#txtVMName').val(data.Name);
                            $('#txtVDMNoOfBoards').val(data.NoOfBoards);
                            $('#selMainBoard').val(data.BoardID);
                            GetBoardSensorAndDeviceLabelling($('#divMainBoardMapping'), data.ID, data.BoardID);

                            AjaxSend({
                                url: 'VendingMachine/GetDaughterBoards',
                                data: { MainBoardID: data.ID },
                                success: function (e) {
                                    for (var i in e.Results.Rows) {
                                        AddDaughterBoard(e.Results.Rows[i]);
                                        GetBoardSensorAndDeviceLabelling($('div[bid="' + e.Results.Rows[i].ID + '"] .card-body'), e.Results.Rows[i].MachineID, e.Results.Rows[i].ID)
                                    }
                                }
                            })
                        }

                        $('#popVDMModelWithListing').modal('show');
                    }
                })

                if (data == undefined) {
                    data = {}
                }

                $('#btnVMWithListingSave').unbind().click(function () {
                    var listDaughterBoard = $('#listDaughterBoard'), arrDaughterBoard = [], arrMainBoardLabelling = [];

                    if ($('#txtVMName').val() == '') {
                        NewMessageBox('Please enter Name');
                        return;
                    }

                    //if (CDec2($('#txtVDMNoOfBoards').val()) == 0) {
                    //    NewMessageBox('Please enter the number of boards.');
                    //    return;
                    //}

                    if ($('#selMainBoard').val() == '') {
                        NewMessageBox('Please select main board.');
                        return;
                    }

                    $('#divMainBoardMapping').find('tbody > tr').each(function () {
                        var tr = $(this);
                        arrMainBoardLabelling.push({
                            MachineID: CStr2(data.ID),
                            BoardID: $('#selMainBoard').val(),
                            SensorDeviceID: CStr2(tr.attr('sdid')),
                            Type: tr.find('td:eq(1)').text(),
                            SensorDeviceName: tr.find('td:eq(2)').text(),
                            Label: tr.find('[name="userdeflable"]').val(),
                            SKUName: tr.find('[name="skuname"]').val(),
                            SKUID: CStr2(tr.find('[name="skuname"]').attr('pid')),
                            Criticality: tr.find('[name="criticality"]').val()
                        })
                    })

                    listDaughterBoard.find('> div').each(function () {
                        var arrLabelling = [], dis = $(this);

                        dis.find('tbody > tr').each(function () {
                            var tr = $(this);
                            arrLabelling.push({
                                MachineID: CStr2(data.ID),
                                BoardID: dis.attr('bid'),
                                SensorDeviceID: CStr2(tr.attr('sdid')),
                                Type: tr.find('td:eq(1)').text(), 
                                SensorDeviceName: tr.find('td:eq(2)').text(), 
                                Label: tr.find('[name="userdeflable"]').val(), 
                                SKUName: tr.find('[name="skuname"]').val(),
                                SKUID: CStr2(tr.find('[name="skuname"]').attr('pid')), 
                                Criticality: tr.find('[name="criticality"]').val() 
                            })
                        })

                        arrDaughterBoard.push({
                            BoardID: dis.attr('bid'),
                            Label: arrLabelling
                        })
                    })

                    AjaxSend({
                        url: '/VendingMachine/SaveVMWithListing',
                        data: {
                            data: {
                                ID: CStr2(data.ID),
                                Name: $.trim($('#txtVMName').val()),
                                NoOfBoards: $.trim($('#txtVDMNoOfBoards').val()),
                                MainBoardID: $.trim($('#selMainBoard').val()),
                                DaughterBoards: arrDaughterBoard,
                                MainBoardLabelling: arrMainBoardLabelling
                            }
                        }, success: function (e) {
                            if (e.Result == 'saved') {
                                WebToast('Successfully saved');
                                $('#popVDMModelWithListing').modal('hide');
                                LibraryTable.VMModelWithControllerListing();
                            } else NewMessageBox(e.Error, 'Error');
                        }
                    })
                })

                $('#btnAddDaughterBoard').unbind().click(function () {
                    BrowsePopup('LibraryVendingMachineBoards', 'Select Boards', [
                        {
                            Message: 'You can select multiple boards.',
                            AutoSearch: true,
                            DisplayText: [{Column:  'Name'}],
                            SelectMultiple: true,
                            Click: function (e) {
                                for (var i in e) {
                                    AddDaughterBoard(e[i]);
                                    GetPredefinedSensorsAndDevices(e[i].ID, function (html) {
                                        $('[bid="' + e[i].ID +'"] .card-body').empty().append(html);
                                    })
                                }
                            }
                        }
                    ])
                })

                function GetBoardSensorAndDeviceLabelling(container, machineid, boardid) {
                    AjaxSend({
                        url: 'VendingMachine/GetBoardSensorAndDeviceLabelling',
                        data: { MachineID: machineid, BoardID: boardid },
                        success: function (e) {
                            var vRows = e.Result.Rows;
                            container.empty();

                            if (vRows.length > 0) {
                                container.append(DisplaySensorsAndDevices(vRows));
                            } else {
                                GetPredefinedSensorsAndDevices(boardid, function (html) {
                                    container.append(html);
                                })
                            }
                        }
                    })
                }

                function GetPredefinedSensorsAndDevices(boardid, callback) {
                    AjaxSend({
                        url: 'VendingMachine/GetBoardSensorAndDevice',
                        data: { BoardID: boardid },
                        success: function (e) {
                            vRows = e.Result.Rows;
                            var arrPredefinedSensorsAndDevices = [];

                            for (var i = 0 in vRows) {
                                var qty = vRows[i].Qty
                                var ctr = 1;
                                for (var q = 1; q <= qty; q++) {
                                    arrPredefinedSensorsAndDevices.push({
                                        SensorDeviceID: vRows[i].ID,
                                        Type: vRows[i].Type,
                                        SensorDeviceName: vRows[i].Description.replace(/\(s\)/g, "") + ' ' + ctr,
                                        Label: '',
                                        SKUName: '',
                                        Criticality: 'Very Low Criticality'
                                    })

                                    ctr += 1;
                                }
                            }

                            callback(DisplaySensorsAndDevices(arrPredefinedSensorsAndDevices));
                        }
                    })                
                }

                function DisplaySensorsAndDevices(vRows) {
                    var tbl = $('<table data-role="none" class="table table-hover table-borderless">');

                    tbl.append($('<thead class="bg-light shadow-sm">')
                        .append($('<tr class="text-center align-top">')
                            .append('<th>#</th>')
                            .append('<th>Type</th>')
                            .append('<th>Name</th>')
                            .append('<th>User-Defined Label</th>')
                            .append('<th>SKU Name of Externally Connected Device</th>')
                            .append('<th>Criticality</th>')
                        )
                    )

                    var tbody = $('<tbody>');

                    for (var i = 0 in vRows) {
                        var tr = $('<tr>')
                            .attr('sdid', vRows[i].SensorDeviceID)
                            .append('<td>' + (CInt2(i) + 1) + '.</td>')
                            .append('<td>' + vRows[i].Type + '</td>')
                            .append('<td>' + vRows[i].SensorDeviceName + '</td>')
                            .append('<td><input data-role="none" type="text" class="form-control" name="userdeflable" value="' + vRows[i].Label + '"></td>')
                            .append($('<td>').append($('<div class="input-group">')
                                .append($('<input data-role="text" type="text" name="skuname" class="form-control" readonly>').attr('pid', vRows[i].SKUID).val(CStr2(vRows[i].SKUName)))
                                .append($('<button data-role="none" class="btn btn-light border" title="Clear">')
                                    .append('<span class="bi-x-lg"></span>')
                                    .click(function () {
                                        var dis = $(this);

                                        dis.prev().removeAttr('pid').val('')
                                    })
                                )
                                .append($('<button data-role="none" name="btnCategoryID" class="btn btn-light border" title="Search SKU">')
                                    .append('<span class="bi-search"></span>')
                                    .click(function () {
                                        var dis = $(this);

                                        BrowsePopup('LibraryPIMProduct', 'Select SKU', [
                                            {
                                                DisplayText: [{ Column: 'ProductNo' }, { Column: 'ProductName' }],
                                                Click: function (e) {
                                                    dis.prev().attr('pid', e.ID).val(e.ProductNo + ' - ' + e.ProductName);
                                                }
                                            }
                                        ])
                                    })
                                )
                            )
                            )
                            .append($('<td>')
                                .append($('<select data-role="none" name="criticality" class="form-select">')
                                    .append('<option value="">Please select</option>')
                                    .append('<option value="Very Low Criticality">Very Low Criticality</option>')
                                    .append('<option value="Low Criticality">Low Criticality</option>')
                                    .append('<option value="Critical">Critical</option>')
                                    .append('<option value="Very Critical">Very Critical</option>')
                                )
                        )

                        tr.find('[name="criticality"]').val(vRows[i].Criticality);

                        tbody.append(tr)
                    }

                    tbl.append(tbody);

                    return tbl;
                }

                function AddDaughterBoard(data) {
                    listDaughterBoard.append($('<div class="card">')
                        .attr('bid', data.ID)
                        .append($('<div class="card-header">')
                            .append($('<div class="accordion-button" data-toggle="collapse" data-target="#' + data.ID + '">').text(data.Name)
                                .append($('<button data-role="none" title="Remove" class="btn btn-light border text-danger float-right ml-3">')
                                    .data('data', data)
                                    .append('<span class="bi bi-trash-fill"></span>')
                                    .click(function () {
                                        var li = $(this).parents().closest('li');

                                        NewMessageBox('Are you sure you want to remove this board "' + $(this).data('data').Name + '"?', 'Confirmation Dialog', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    li.remove();
                                                }
                                            }, { Text: 'No' }
                                        ])
                                    })
                                )
                            )
                        )
                        .append($('<div class="collapse">')
                            .attr('id', data.ID)
                            .append('<div class="card-body"></div>')
                        )
                    )
                }
            },

            VDMPlanogramTemplatePopup: function (data) {
                var tblPlanogramTemplate = $('#tblPlanogramTemplate tbody').empty();
                var motorNo = 1;

                AjaxSend({
                    url: 'VendingMachine/GetVMMachines',
                    success: function (e) {
                        var selPTMachine = $('#selPTMachine').empty(), motors = [];

                        selPTMachine.append(new Option('Please Select', ''));

                        for (var i in e.MainBoard.Rows) {
                            selPTMachine.append(new Option(e.MainBoard.Rows[i].MachineName, e.MainBoard.Rows[i].ID));
                        }

                        selPTMachine.unbind().change(function () {
                            if ($(this).val() != '') {
                                AjaxSend({
                                    url: 'VendingMachine/GetVMBoardInfo',
                                    data: { ID: $(this).val() },
                                    success: function (e) {
                                        var mainboard = e.MainBoard;
                                        var subboard = e.SubBoard.Rows;
                                        var boardNo = 97; //Starts with "A"

                                        InitList(mainboard.NoOfDirectConnMotors, mainboard, boardNo, 'Direct Motor Connection');
                                        InitList(mainboard.NoOfSpringCoilMotors, mainboard, boardNo, 'Spring Coil');
                                        
                                        boardNo++;

                                        for (var i = 0 in subboard) {
                                            InitList(subboard[i].NoOfDirectConnMotors, subboard[i], boardNo, 'Direct Motor Connection');
                                            InitList(subboard[i].NoOfSpringCoilMotors, subboard[i], boardNo, 'Spring Coil');
                                            boardNo++;
                                        }

                                        for (var i = 0 in motors) {
                                            //DisplayList(motors[i]);
                                            var motorData = motors[i];
                                            var tr = tblPlanogramTemplate.find('tr[boardid="' + motorData.BoardID + '"][motorno="' + motorData.MotorNo + '"][dispensertype="' + motorData.DispenserType + '"][dispenserno="' + motorData.DispenserMotorNo + '"][boarduuid="' + motorData.BoardUUID + '"]')
                                            tr.find('input[name="txtCapacity"]').val(motorData.Capacity)
                                            tr.find('input[name="txtCategory"]').val(motorData.Category)
                                            tr.find('button[name="btnProductID"]').attr('productid', motorData.ProductID).text(motorData.ProductName)
                                            tr.attr('rowid', motorData.ID)
                                        }
                                    }
                                })
                            }                            
                        })

                        if (!$.isEmptyObject(data)) {
                            AjaxSend({
                                url: 'VendingMachine/GetPTInfo',
                                data: { PTID: data.ID },
                                success: function (e) {
                                    var header = e.Template;
                                    motors = e.Motors.Rows;

                                    $('#txtPTID').val(header.ID);
                                    $('#selPTMachine').val(header.MachineModelID).change();
                                    $('#txtPTHBName').val(header.Name);                                    
                                }
                            })
                        }
                    }
                })
                $('#txtPTID, #selPTMachine, #txtPTHBName').val('');
                $('#popVDMPlanogramTemplate').modal('show');

                if (data != undefined) {
                    $('#txtVDMName').val(data.Name);
                    $('#txtVDMDescription').val(data.Description);
                    $('#txtVDMNoOfSpringCoilMotors').val(data.NoOfMotors);
                    $('#txtVDMNoOfDirectConnMotors').val(data.NoOfDirectConnMotors);
                    $('#txtVDMUUID').val(data.UUID);
                    $('#selVDMActive').prop('checked', CInt2(data.IsActive));
                } else {
                    data = {}
                }

                $('#btnPTSave').unbind().click(function () {
                    var selPTMachine = $('#selPTMachine').val();
                    var txtPTHBName = $('#txtPTHBName').val();
                    var arrMotors = [];

                    if (selPTMachine == '') {
                        NewMessageBox('Please select Machine');
                        return;
                    }
                    
                    if (txtPTHBName == '') {
                        NewMessageBox('Please enter template name');
                        return;
                    }

                    tblPlanogramTemplate.find('tr').each(function () {
                        var dis = $(this).find('td');

                        arrMotors.push({
                            ID: dis.parent().attr('rowid'),
                            MotorNo: dis.eq(0).text(),
                            BoardNo: dis.eq(1).text(),
                            BoardID: dis.eq(2).attr('boardid'),
                            BoardUUID: dis.parent().attr('boarduuid'),
                            DispenserMotorNo: dis.eq(3).attr('DispenserMotorNo'),
                            DispenserType: dis.eq(3).text(),
                            Capacity: dis.eq(4).find('input').val(),
                            ProductID: CStr2(dis.eq(5).find('button').attr('productid')),
                            ProductName: dis.eq(5).find('button').text(),
                            Category: dis.eq(6).find('input').val(),
                        })
                    })
                    console.log(arrMotors)
                    AjaxSend({
                        url: '/VendingMachine/SavePTMotors',
                        data: {
                            PTID: $('#txtPTID').val(),
                            MachineID: selPTMachine,
                            TemplateName: txtPTHBName,
                            Motors: arrMotors,
                            UpdatedBy: sessionStorage.PeopleID
                        }, success: function (e) {
                            if (e.Result == 'saved') {
                                WebToast('Successfully saved');
                                $('#popVDMPlanogramTemplate').modal('hide');
                                LibraryTable.VDMPlanogramTemplate();
                            } else NewMessageBox(e.Error, 'Error');
                        }
                    })
                })

                var vMotorNo = 1;
                function InitList(motorcount, data, boardNo, dispensertype) {
                    if (CInt2(motorcount) > 0) {
                        for (var i = 1; i <= CInt2(motorcount); i++) {
                            DisplayList({
                                ID: '',
                                MotorNo: vMotorNo,
                                BoardNo: String.fromCharCode(boardNo).toUpperCase(),
                                BoardName: data.BoardName,
                                BoardID: data.BoardID,
                                BoardUUID: data.BoardUUID,
                                DispenserMotorNo: i,
                                DispenserType: dispensertype,
                                Capacity: '',
                                ProductID: '',
                                ProductName: 'Please Select',
                                CategoryID: '',
                                CategoryName: 'Please Select'
                            })

                            vMotorNo++;
                        }
                    }
                }

                function DisplayList(data) {
                    tblPlanogramTemplate.append($('<tr>')
                        .attr('rowid', data.ID)
                        .attr('motorno', data.DispenserMotorNo /*data.MotorNo*/)
                        .attr('boardid', data.BoardID)
                        .attr('dispenserno', data.DispenserMotorNo)
                        .attr('boarduuid', data.BoardUUID)
                        .attr('dispensertype', data.DispenserType)
                        .append('<td class="text-center">' + data.DispenserMotorNo /*data.MotorNo*/ + '</td>')
                        .append('<td class="text-center">' + data.BoardNo + '</td>')
                        .append('<td boardid="' + data.BoardID + '">' + data.BoardName + '</td>')
                        .append('<td dispensermotorno="' + data.DispenserMotorNo + '">' + data.DispenserType + '</td>')
                        .append('<td class="text-center"><input data-role="text" name="txtCapacity" type="text" class="form-control" style="width:100px;" value="' + CStr2(data.Capacity) + '"></td>')
                        .append($('<td>').append($('<div class="input-group d-flex justify-content-center">')
                                .append($('<button data-role="none" name="btnProductID" class="btn btn-light border">')
                                    .attr('productid', data.ProductID)
                                    .text(data.ProductName)
                                    .click(function () {
                                        var dis = $(this);

                                        BrowsePopup('LibraryPIMProduct', 'Select Product', [
                                            {
                                                DisplayText: [{ Column: 'ProductNo' }, { Column: 'ProductName' }],
                                                Click: function (e) {
                                                    dis.attr('productid', e.ID).text(e.ProductNo + ' - ' + e.ProductName);
                                                    dis.parent().parent().next().find('input').val(e.Category);
                                                }
                                            }
                                        ])
                                    })
                                )
                                .append($('<button data-role="none" class="btn btn-light border" title="Clear">')
                                    .append('<span class="bi-x-lg"></span>')
                                    .click(function () {
                                        var dis = $(this);

                                        dis.prev().removeAttr('productid').text('Please Select');
                                        dis.parent().parent().next().find('input').val('');
                                    })
                                )
                            )                            
                        )
                        .append($('<td>').append($('<div class="input-group">')
                            .append($('<input data-role="text" type="text" name="txtCategory" class="form-control">').val(CStr2(data.Category)))
                            .append($('<button data-role="none" class="btn btn-light border" title="Clear">')
                                .append('<span class="bi-x-lg"></span>')
                                .click(function () {
                                    var dis = $(this);

                                    dis.prev().removeAttr('pid').val('')
                                })
                            )
                            .append($('<button data-role="none" name="btnCategoryID" class="btn btn-light border">')
                                .append('<span class="bi-search"></span>')
                                .click(function () {
                                    var dis = $(this);

                                    BrowsePopup('LibraryPimCategories', 'Select Category', [
                                        {
                                            DisplayText: [{ Column: 'Category' }],
                                            Click: function (e) {
                                                dis.prev().val(e.Category);
                                            }
                                        }
                                    ])
                                })
                            )
                            )                            
                        )
                    )
                }
            },

            VDMAuthorizers: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=vdmauthorizers",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.EmployeeName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.UpdatedByName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return $.format.date(new Date(parseInt(data.UpdatedOn.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }, className: 'text-center'
                        }
                    ],
                    click: function (data) {
                        LibraryTable.VDMAuthorizersPop(data);
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            VDMAuthorizersPop: function (data) {
                var employeeid = '';
                var accessRights = [
                    { Description: 'Allow door control', Value: 'CONTROL_DOOR' },
                    { Description: 'Allow control of the RGB LED indicator', Value: 'CONTROL_LED' },
                    { Description: 'Allow door to unlock', Value: 'CONTROL_LOCK' },
                    { Description: 'Allow control power on all VMC boards', Value: 'CONTROL_POWER' },
                    { Description: 'Allow control of pump valve', Value: 'CONTROL_VALVE' },
                    { Description: 'Allow manual motor rotation (for dry goods)', Value: 'DISPENSE_ITEM' },
                    { Description: 'Allow dispensing of a specific volume of liquid soap', Value: 'DISPENSE_VOLUME' },
                    { Description: 'Allow cash deposit and dispensing', Value: 'CASH_DEPOSIT' },
                    { Description: 'Allow access to machine settings', Value: 'MACHINE_SETTINGS' },
                    { Description: 'Allow manual inventory adjustments', Value: 'MACHINE_INVENTORY' }
                ]
                var divVDMAccessRights = $('#divVDMAccessRights');

                if (divVDMAccessRights.find('*').length == 0) {
                    for (var i in accessRights) {
                        divVDMAccessRights.append($('<div class="form-check">')
                            .append('<input data-role="none" type="checkbox" id="chbVDMAccessRight' + i + '" class="form-check-input chbVDMAccessRights" value="' + accessRights[i].Value + '" />')
                            .append('<label class="form-check-label" for="chbVDMAccessRight' + i + '">' + accessRights[i].Description + '</label>')
                        )
                    }
                }

                $('#txtVDMAuthorizeEmployee, #txtVDMAuthorizeEmployeePIN').val('');
                $('.chbVDMAccessRights').prop('checked', 0);
                $('#popVDMAuthorizer').modal('show');

                if (data != undefined) {
                    employeeid = data.EmployeeID;
                    $('#txtVDMAuthorizeEmployee').val(data.EmployeeName);
                    $('#txtVDMAuthorizeEmployeePIN').val('');
                    $('.vdmauthold').show();
                    $('.vdmauthnew').hide();

                    AjaxSend({
                        url: '/VendingMachine/GetAccessRights',
                        data: { ID: data.ID },
                        success: function (e) {
                            var vRows = e.AccessRights.Rows;

                            for (var i in vRows) {
                                $('.chbVDMAccessRights[value="' + vRows[i].Description + '"]').prop('checked', 1);
                            }
                        }
                    })
                } else {
                    data = {};
                    $('.vdmauthold').hide();
                    $('.vdmauthnew').show();
                }

                $('.vdmauthold').unbind().click(function () {
                    $('.vdmauthold').hide();
                    $('.vdmauthnew').show();
                })

                $('#txtVDMAuthorizeEmployee').unbind().click(function(){
                    BrowsePopup('LibraryOrganizationEmployee', 'Select Employee', [
                        {
                            DisplayText: [{Column: 'PeopleName'}],
                            Click: function (e) {
                                employeeid = e.ID;
                                $('#txtVDMAuthorizeEmployee').val(e.PeopleName);
                            }
                        }
                    ])                
                })

                $('#txtVDMAuthorizeEmployeePIN').unbind().keydown(function(e){
                    if (e.keyCode != 110 && e.keyCode != 190) {
                        if (!NumberOnly(e)) return;
                    }
                })

                $('#btnVDMAuthorizerSave').unbind().click(function () {
                    var pin = $.trim($('#txtVDMAuthorizeEmployeePIN').val());
                    var accessRights = [];
                    var repeatingPattern = /(\d)\1\1\1/;

                    if (employeeid == '') {
                        NewMessageBox('Please select the employee you want to authorize in the vending machine.');
                        return;
                    }

                    if ($('#txtVDMAuthorizeEmployeePIN').css('display') == 'block') {
                        if (pin == '') {
                            NewMessageBox('Employee\'s PIN is required.');
                            return;
                        } else {
                            if (HasSequentialDigits(pin)) {
                                NewMessageBox('Five consecutive numbers are not allowed. Please try again!');
                                return;
                            }

                            if (repeatingPattern.test(pin)) {
                                NewMessageBox('Please avoid using more than three of the same number in a row!');
                                return;
                            }
                        }
                    }                    

                    if ($('.chbVDMAccessRights:checked').length == 0) {
                        NewMessageBox('Please assign at least one access right to the selected employee.');
                        return;
                    }

                    $('.chbVDMAccessRights:checked').each(function () {
                        accessRights.push($(this).val())
                    })

                    AjaxSend({
                        url: '/VendingMachine/SaveVDMAuthorizer',
                        data: {
                            data: {
                                ID: CStr2(data.ID),
                                EmployeeID: employeeid,
                                Pin: ($('#txtVDMAuthorizeEmployeePIN').css('display') == 'block' ? pin : ''),
                                AccessRights: accessRights
                            }
                        }, success: function (e) {
                            if (e.Result == 'saved') {
                                WebToast('Successfully saved');
                                $('#popVDMAuthorizer').modal('hide');
                                LibraryTable.VDMAuthorizers();
                            } else if (CStr2(e.Validation) != '') {
                                NewMessageBox(CStr2(e.Validation));
                            } else NewMessageBox(e.Error, 'Error');
                        }
                    })
                })
            },

            VDMPhysicalButtonSettingsPage: function () {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isPhysicalButtonSettingsPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/VendingMachine/PhysicalButtonSettings?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isPhysicalButtonSettingsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isPhysicalButtonSettingsPageLoaded);//.trigger('create');
                }
            },

            VDMPhysicalButtonSettings: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=vdmphysicalbuttons",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Name;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Description;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.UserName;
                            }, className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return $.format.date(new Date(parseInt(data.LastUpdated.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }, className: 'text-center'
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Template";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isVDMPhysicalButtonsClicked) {
                            LibraryTable.VDMPhysicalButtonSettingsPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            VDMRiceDispensingGuidesPage: function () {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isGuidessPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/VendingMachine/Guides?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isGuidessPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isGuidessPageLoaded);//.trigger('create');
                    VDM.GuidesInit();
                }
            },

            VDMRiceDispensingGuides: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=vdmricedispensingguides",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Name;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Description;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.UserName;
                            }, className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return $.format.date(new Date(parseInt(data.LastUpdated.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }, className: 'text-center'
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Template";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isVDMRiceDispensingGuidesClicked) {
                            LibraryTable.VDMRiceDispensingGuidesPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            VDMProductSelectionGuidesPage: function () {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isGuidessPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/VendingMachine/ProductSelectionGuides?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isGuidessPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isGuidessPageLoaded);//.trigger('create');
                    VDM.GuidesInit();
                }
            },

            VDMProductSelectionGuides: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=vdmricedispensingguides",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Name;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Description;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.UserName;
                            }, className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return $.format.date(new Date(parseInt(data.LastUpdated.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }, className: 'text-center'
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Template";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isVDMRiceDispensingGuidesClicked) {
                            LibraryTable.VDMProductSelectionGuidesPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            PromoCampaignCode: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=promocode&status=" + $('#selOptionStatus option:selected').val(),
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.CampaignName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return CStr2(data.PromoStart);
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return CStr2(data.PromoEnd);
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return $.format.date(new Date(parseInt(data.LastUpdated.substr(6))), 'MM-dd-yyyy hh:mm a')
                            }, className: 'text-center'
                        }
                    ],
                    click: function (data) {
                        
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            Vouchers: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=vouchers&status=" + $('#selOptionStatus option:selected').val(),
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.ReferenceNo;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return CStr2(data.VoucherDescription);
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return CBool(data.IsActive) ? 'Active' : 'Inactive';
                            }, className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return $.format.date(new Date(parseInt(data.LastUpdated.substr(6))), 'MM-dd-yyyy hh:mm a')
                            }, className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return CBool(data.IsActive) ? '' : $.format.date(new Date(parseInt(data.TransactionDate.substr(6))), 'MM-dd-yyyy hh:mm a');
                            }, className: 'text-center'
                        }
                    ],
                    click: function (data) {
                        
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },

            VDMAdsPage: function () {
                $('#UniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>');
                if (isAdsPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/VendingMachine/Ads?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAdsPageLoaded = vData;
                            $('#UniversalDiv').empty();
                            $('#UniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                } else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isAdsPageLoaded);//.trigger('create');
                    VDM.AdsInit();
                }
            },

            VDMAds: function (s) {
                if (s != undefined) {
                    tblSysAdminDataTable.search($.trim(s)).draw();
                    return;
                }

                var pData = {
                    ajax: {
                        url: "/Organization/GetDataTable?OrgID=" + sessionStorage.OrgID + "&mod=vdmads",
                        type: 'POST'
                    },
                    columns: [
                        {
                            "data": function (data, type, full, meta) {
                                return data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Name;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.Description;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return data.UserName;
                            }, className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return $.format.date(new Date(parseInt(data.LastUpdated.substr(6))), 'MMM dd, yyyy hh:mm a');
                            }, className: 'text-center'
                        }
                    ],
                    click: function (data) {
                        sessionStorage["AddViewLibBCName"] = "View Ads";
                        window.location.hash = 'UniversalPage';
                        sessionStorage["ViewID"] = data.ID;
                        if (isVDMAdsClicked) {
                            LibraryTable.VDMAdsPage();
                        }
                        InitVal();
                    },
                    disablecols: [0] //if all '_all'
                }

                SysAdminDataTable(pData);
            },
        } // END window.LibraryTable = {

        function HasSequentialDigits(pin) {
            for (var i = 0; i <= pin.length - 5; i++) {
                var isAscending = true;
                var isDescending = true;
        
                for (var j = 0; j < 4; j++) {
                  if (parseInt(pin[i + j]) + 1 !== parseInt(pin[i + j + 1])) {
                    isAscending = false;
                  }
                  if (parseInt(pin[i + j]) - 1 !== parseInt(pin[i + j + 1])) {
                    isDescending = false;
                  }
                }
        
                if (isAscending || isDescending) return true;
            }
              return false;
        }

        function GetProductCategories() {
            optProductCategories = '<option value="">Please Select</option>'

            AjaxSend({
                url: '/Browser/LibraryPimCategories',
                data: { IsAll:  1 },
                success: function (e) {
                    var vRows = e.Results.Rows, len = vRows.length, i = 0;

                    for (i; i < len; i++) {
                        optProductCategories += '<option value="' + vRows[i].Category + '">' + vRows[i].Category + '</option>';
                    }
                }
            })
        }

        function GetInventoryItemCategory() {
            AjaxSend({
                url: '/Browser/PimCategories',
                data: { IsAll: 1, IsInventoryItem: 1 },
                success: function (e) {
                    var vRows = e.Results.Rows, divIISKUCat = $('#divIISKUCat').empty();

                    for (var i in vRows) {
                        divIISKUCat.append($('<li class="list-group-item">')
                            .attr('catname', vRows[i].Name)
                            .append(vRows[i].Name)
                        )
                    }

                    $('#btnSKUCatSortingSave').unbind().click(function () {
                        var sortList = [], ctr = 1;

                        $('#divIISKUCat li').each(function () {
                            sortList.push({
                                Category: $(this).attr('catname'),
                                Order: ctr
                            })
                            ctr += 1;
                        })

                        AjaxSend({
                            url: 'SetupChecklist/SaveSortedInventoryItemCategories',
                            data: { data: sortList },
                            success: function (e) {
                                if (e.Result == 'saved') {
                                    NewMessageBox('Changes has been saved successfully')
                                    GetInventoryItemCategory();
                                }
                            }
                        })
                    })

                    $('#divIISKUCat').sortable({
                        list: '#divIISKUCat',
                        delay: 150,
                        start: function (event, ui) {
                            //ui.item.parent().css('border', '1px solid #CCC');
                        },
                        stop: function (event, ui) {
                            //ui.item.parent().css('border', '1px solid #FFF');
                        },
                        update: function (event, ui) {
                            var id = ui.item.parent().attr('id');
                            var newOrderNo = 1;
                            $('#' + id).find('> li').each(function () {
                                $(this).data('data').OrderNo = newOrderNo;
                                $(this).find('td:eq(0)').text(newOrderNo + '.');
                                newOrderNo += 1;
                            })
                        }
                    });
                }
            })
        }

        function GetEmployeeStatus(data) {
            var IsConfirmed = CStr2(data.IsConfirmed);
            var IsTerminated = CStr2(data.IsTerminated);
            var DeActivated = CBool(data.DeActivated);
            var DeactivationDate = data.DeactivationDate;
            var status = "";

            // if (IsConfirmed != "" && IsTerminated != "") { // commented by Arvin 2019_07_04__13_28_50_3315
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
                status = (DeActivated != false ? 'Deactivated ( ' + $.format.date(new Date(parseInt(DeactivationDate.substr(6))), 'MMM dd, yyyy hh:mm a') + ' )' : 'Active');
            }
            // commented by Arvin 2019_07_04__13_28_50_3315
            /*
            }
            else if (CBool(IsConfirmed) == undefined && CBool(IsTerminated) == undefined) {
                status = (DeActivated != false ? 'Deactivated ( ' + $.format.date(new Date(parseInt(DeactivationDate.substr(6))), 'MMM dd, yyyy hh:mm a') + ' )' : 'Active');
            }
            */
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
                rowReorder: {
                    selector: 'td:nth-child(1)'
                },
                "responsive": true,
                "columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"clear"><"table-fixed-container"<"table-fixed"Bt>><"bottom"ip><"clear">',
                "initComplete": function (e, data) {
                    $('#tblSysAdminDataTable').removeClass('dataTable');
                    
                    if ($.isFunction(vData.initComplete)) {
                        vData.initComplete(data);
                    }
                },
                "fnDrawCallback": function (oSettings) {
                    if ($.isFunction(vData.CallBack)) {
                        vData.CallBack(oSettings.json.data);
                    }
                },
                buttons: [
                    {
                        "extend": 'csvHtml5', text: 'Download CSV', title: sessionStorage.LibraryName,
                        "action": newexportaction, className: 'btnSystemSettingDTExportCSV d-none'
                    }
                ]
                //"stateSave": true
            })

            $('#tblSysAdminDataTable tbody').unbind('click').on('click', 'tr', function () {
                if (CStr2(tblSysAdminDataTable.row(this).data()) != '') {
                    vData.click(tblSysAdminDataTable.row(this).data());
                }
            });

            $('#btnExportToCSV').unbind().click(function () {
                $('.btnSystemSettingDTExportCSV').trigger("click");
            })
        }

        function newexportaction(e, dt, button, config) {
            var self = this;
            var oldStart = dt.settings()[0]._iDisplayStart;
            dt.one('preXhr', function (e, s, data) {
                // Just this once, load all data from the server...
                data.start = 0;
                data.length = 100000;
                dt.one('preDraw', function (e, settings) {
                    // Call the original action function
                    if (button[0].className.indexOf('buttons-copy') >= 0) {
                        $.fn.dataTable.ext.buttons.copyHtml5.action.call(self, e, dt, button, config);
                    } else if (button[0].className.indexOf('buttons-excel') >= 0) {
                        $.fn.dataTable.ext.buttons.excelHtml5.available(dt, config) ?
                            $.fn.dataTable.ext.buttons.excelHtml5.action.call(self, e, dt, button, config) :
                            $.fn.dataTable.ext.buttons.excelFlash.action.call(self, e, dt, button, config);
                    } else if (button[0].className.indexOf('buttons-csv') >= 0) {
                        $.fn.dataTable.ext.buttons.csvHtml5.available(dt, config) ?
                            $.fn.dataTable.ext.buttons.csvHtml5.action.call(self, e, dt, button, config) :
                            $.fn.dataTable.ext.buttons.csvFlash.action.call(self, e, dt, button, config);
                    } else if (button[0].className.indexOf('buttons-pdf') >= 0) {
                        $.fn.dataTable.ext.buttons.pdfHtml5.available(dt, config) ?
                            $.fn.dataTable.ext.buttons.pdfHtml5.action.call(self, e, dt, button, config) :
                            $.fn.dataTable.ext.buttons.pdfFlash.action.call(self, e, dt, button, config);
                    } else if (button[0].className.indexOf('buttons-print') >= 0) {
                        $.fn.dataTable.ext.buttons.print.action(e, dt, button, config);
                    }
                    dt.one('preXhr', function (e, s, data) {
                        // DataTables thinks the first item displayed is index 0, but we're not drawing that.
                        // Set the property to what it was before exporting.
                        settings._iDisplayStart = oldStart;
                        data.start = oldStart;
                    });
                    // Reload the grid with the original page. Otherwise, API functions like table.cell(this) don't work properly.
                    setTimeout(dt.ajax.reload, 0);
                    // Prevent rendering of the full data to the DOM
                    return false;
                });
            });
            // Requery the server with the new one-time export settings
            dt.ajax.reload();
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
                    .append($('<span>')
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
                        .append($('<div>')
                            .text(vRows[i].Address)
                        )
                    )
                    .append($('<span>')
                        .attr('ID', vRows[i].ID)
                        .click(function () {
                            $('#' + $(this).attr('ID')).remove();
                        })
                    )
                pLV.append(pItem)
            }

            pLV;//.listview('refresh');
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

                            $('#lstPOS').html('');//.listview('refresh');
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

                            $('#lstReceivingAndTransfers').html('');//.listview('refresh');
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

                            $('#lstInvCount').html('');//.listview('refresh');
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

                            $('#lstSFA').html('');//.listview('refresh');
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

                            $('#lstCRM').html('');//.listview('refresh');
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

                            $('#lstDashboardsAndReports').html('');//.listview('refresh');
                            pLV = $('#lstDashboardsAndReports');
                            LoadSecurityRoleModule(vDataRoleModuleDAR.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    pData.MainModule = "Requests and Approvals";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleDAR) {

                            $('#lstDashboardsAndReports').html('');//.listview('refresh');
                            pLV = $('#lstDashboardsAndReports');
                            LoadSecurityRoleModule(vDataRoleModuleDAR.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    pData.MainModule = "Sales Returns";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleDAR) {
                            $('#lstSalesReturns').html('');//.listview('refresh');
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

                            $('#lstSettings').html('');//.listview('refresh');
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

                            $('#lstMDM').html('');//.listview('refresh');
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
                pLV;//.listview('refresh');

                LoadSecurityRoleModule(vRows, pLV, intCurIndex + 1, strRoleID, false, strindentation);

            } else {

                pItem = $('<li>')
                                .append(pItem)

                pLV.append(pItem);
                pLV.trigger("create");
                pLV;//.listview('refresh');

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
                $('#lstCategories');//.listview('refresh');
            }
            catch (e) {
                $('#lstCategories');//.listview();
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
                $('#lstCategoriesContainer');//.listview('refresh');
            }
            catch (e) {
                $('#lstCategoriesContainer');//.listview();
            }
        }

        function LoadOrgFOWSTProductList(vRows) {
            var pLV = $('#lstOrgFOWSTProductList');
            var pItem;

            $("#divOrgFOWSTProductRecordCount").text(" Number of record(s) found: " + vRows.length);

            pLV.html('');
            for (var p = 0; p <= vRows.length - 1; p++) {
                pItem = $('<li>')
                        .append($('<span>')
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

            pLV;//.listview('refresh');
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
                                    .append($('<span>')
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

            pLV;//.listview('refresh');
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