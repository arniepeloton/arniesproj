require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var vDataPOS, vDataReceivingTransfer, vDataInventoryCount,
                vDataSFA, vDataCRM, vDataRequestsAndApprovals, vDataSalesReturns, vDataDashboard, vDataSettings, vDataMDM,
                vDataCRMDashboard, vDataCRMTab, vDataCRMSettings;
            var ctrList = 0, isCrm = CBool(sessionStorage.IsCrm);
            var arrModules = [
                {
                    AccordionID: 'EasAccRevenue', Name: 'Revenue', Modules: [
                        { AccordionID: 'EasAccRevenueMain', Name: 'Revenue Main Tabs' },
                        { AccordionID: 'EasAccRevenueQuotation', Name: 'Quotation' },
                        { AccordionID: 'EasAccRevenueOrderProcessing', Name: 'Order Processing' },
                        { AccordionID: 'EasAccRevenueOrderFulfillment', Name: 'Order Fulfillment' },
                        { AccordionID: 'EasAccRevenueInvoicing', Name: 'Invoicing' },
                        { AccordionID: 'EasAccRevenueDeliveryTracking', Name: 'Delivery Tracking' },
                        { AccordionID: 'EasAccRevenueCreditAndCollection', Name: 'Credit and Collection' },
                        { AccordionID: 'EasAccRevenueDeposits', Name: 'Deposits' },
                        { AccordionID: 'EasAccRevenueCompetitorPriceCanvassing', Name: 'Competitor Price Canvassing' }
                    ]
                },
                {
                    AccordionID: 'EasAccDisbursement', Name: 'Disbursement', Modules: [
                        { AccordionID: 'EasAccDisbursementDisbursements', Name: 'Disbursement Main Tabs' },
                          { AccordionID: 'EasAccDisbursementVendorPriceCanvassing', Name: 'Vendor Price Canvassing' },
                          { AccordionID: 'EasAccDisbursementPurchasing', Name: 'Purchasing' },
                          { AccordionID: 'EasAccDisbursementJobOrder', Name: 'Job Order' },
                          { AccordionID: 'EasAccDisbursementStockReceiving', Name: 'Stock Receiving' },
                          { AccordionID: 'EasAccDisbursementAccountsPayabaleVerification', Name: 'Accounts Payabale Verification' },                          
                          { AccordionID: 'EasAccDisbursementChequeClearing', Name: 'Cheque Clearing' },
                          { AccordionID: 'EasAccDisbursementExpenseTracking', Name: 'Expense Tracker' }
                    ]
                },
                { AccordionID: 'EasAccInventoryHandling', Name: 'Inventory Handling and Controls' },
                { AccordionID: 'EasAccDTR', Name: 'DTR and HR' },
                { AccordionID: 'EasAccProjectMgmt', Name: 'Project Management' },
                { AccordionID: 'EasAccCRM', Name: 'CRM' },
                { AccordionID: 'EasAccAssetMgmt', Name: 'Assets Management' },
                { AccordionID: 'EasAccGatePassCreation', Name: 'Gate Pass Creation' },
                { AccordionID: 'EasAccGPS', Name: 'Fleet Tracking & Management' },
                { AccordionID: 'EasAccFinancialStatement', Name: 'Financial Statements' },
                { AccordionID: 'EasAccComplianceReminders', Name: 'Compliance Reminders' },
                //{ AccordionID: 'EasAccSystemAdministration', Name: 'System Administration' }
            ]

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                //mLoaded = true;
                mLoaded = false;

                if (!window.IsPageLoaded) {
                    LoadEASSettings();
                    ShowSecurityRoleInfo(sessionStorage.ViewID || '');

                    //window.IsPageLoaded = true;
                    window.IsPageLoaded = false;                    
                }
            };
            if (!mLoaded) PageShow();

            window.SecurityRole = {
                ShowSecurityInfo: function () {
                    AjaxSend({
                        url: 'SecurityRole/IsNewEntry',
                        success: function (vData) {
                            isCrm = CBool(sessionStorage.IsCrm)
                            LoadEASSettings();
                            ShowSecurityRoleInfo(vData.SecurityRoleID || '');
                            ctrList = 0;                            
                        }
                    });
                }
            }

            $('body').on('click', '#btnSecurityRoleInfoDone', function () {
                if ($.trim($("#txtRoleCode").val()) == "") {

                    NewMessageBox("Please enter a code.",
                            "Code",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtRole").val()) == "") {
                    NewMessageBox("Please enter a security role.",
                            "Role",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                SaveSecurityRoleInfo();
            })
            .on('click', '#btnSecurityRoleInfoCancel', function () {
                if (sessionStorage.AddViewLibBCName == "View Security Role") {
                    sessionStorage["AddViewLibBCName"] = "View Security Role";
                    window.location.hash = '';
                    window.location.hash = 'UniversalPage';
                    if (!isCrm) {
                        LibraryTable.ViewSecurityRolePage();
                    } else {
                        CRMLibraryTable.ViewSecurityRolePage();
                    }
                }
                else {
                    $('#tblBody').html('');
                    window.location.hash = '';
                    window.location.hash = 'LibraryPage';
                    if (!isCrm) {
                        LibraryTable.SecurityRoles();
                    } else {
                        CRMLibraryTable.SecurityRoles();
                    }
                }
            })

            function LoadEASSettings() {
                var divEASModules = $('#divEASModules');

                for (var i = 0; i < arrModules.length; i++){
                    var divModules = CreateAccordion(arrModules[i], true);

                    divEASModules.append(divModules);
                }

                //divEASModules.trigger('create');
            }

            function CreateAccordion(data, isParent) {
                var acc = $('<div class="accordion mb-3 colMCR">')                    
                    .append($('<div class="card">').append($('<div class="card-header p-0">')
                        .append($('<button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">')
                            .attr({
                                'data-target': '#div' + data.AccordionID
                            })
                            .text(data.Name)
                        )
                    ).append($('<div class="collapse show">')
                        .attr({
                            id: 'div' + data.AccordionID
                        })
                    ))
                var accbody = $('<div class="card-body">')

                if(data.Modules != undefined){
                    for (var i = 0; i < data.Modules.length; i++) {
                        var accChild = CreateAccordion(data.Modules[i], false);
                        accbody.append(accChild);
                    }
                } else {
                    accbody.append($('<ul id="' + data.AccordionID + '" data-role="none" class="list-group mb-3">'));
                }

                acc.find('.collapse').append(accbody);

                return acc;
            }

            function ShowSecurityRoleInfo(strID) {
                var pLV;
                var pData = {};
                pData.SecurityRoleID = strID;

                ClearSecurityRoleFields();

                if(!isCrm){
                    $('.colMCR').show();
                    $('.colCRM').hide();
                    if (sessionStorage.OrgID != proj.BirOrgID)
                      $('#divTabSecurityRole').show();
                    $('#divTabSecurityRole li').removeClass('hidetab');
                } else {
                    $('.colMCR').hide();
                    $('.colCRM').show();
                    $('#divTabSecurityRole').hide();
                    $('#divTabSecurityRole li').addClass('hidetab');
                    $('#tabSecurityRoleInfoEas').opentabmenu();
                }

                AjaxSend({
                    url: "SecurityRole/getSecurityRoleInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.SecurityRoleData.Rows.length > 0) {

                            $("#txtSecurityRoleID").val(vData.SecurityRoleData.Rows[0].ID || '');
                            $("#txtRoleCode").val(vData.SecurityRoleData.Rows[0].Code || '');
                            $("#txtRole").val(vData.SecurityRoleData.Rows[0].Role || '');
                            $("#txtRoleDescription").val(vData.SecurityRoleData.Rows[0].Description || '');
                            $('#chkRoleIsActive').attr("checked", (vData.SecurityRoleData.Rows[0].IsActive == 1 ? true : false));//.checkboxradio("refresh");

                        }

                        if (vData.HiddenMenu != undefined) {
                            if (!vData.IsSuperAdmin) {
                                for (var i in vData.HiddenMenu.Rows) {
                                    $('[accessrightmenu="' + vData.HiddenMenu.Rows[i].MainModuleName + '"]').parents().closest('.accordion').remove();
                                }
                                $('[accessrightmenu]').parent().remove();
                            } else {
                                for (var i in vData.HiddenMenu.Rows) {
                                    $('[accessrightmenu="' + vData.HiddenMenu.Rows[i].MainModuleName + '"]').find('input[value="Hide"]').prop('checked', 1);
                                }
                                $('[accessrightmenu]').removeClass('d-none');
                            }
                        }                        

                        if (!isCrm) {
                            GetPOSModule(pData, strID);
                        } else {
                            GetCRMDashboardModule(pData, strID);
                        }
                        

                        //                        var load = setTimeout(function () {
                        //                            if ($('#lstPOS li').length == 0 || $('#lstReceivingAndTransfers li').length == 0
                        //                                || $('#lstInvCount li').length == 0 || $('#lstSFA li').length == 0) {
                        //                                $("#txtSecurityRoleID").val(vData.SecurityRoleData.Rows[0].ID || '');
                        //                                $("#txtRoleCode").val(vData.SecurityRoleData.Rows[0].Code || '');
                        //                                $("#txtRole").val(vData.SecurityRoleData.Rows[0].Role || '');
                        //                                $("#txtRoleDescription").val(vData.SecurityRoleData.Rows[0].Description || '');
                        //                                $('#chkRoleIsActive').attr("checked", (vData.SecurityRoleData.Rows[0].IsActive == 1 ? true : false));//.checkboxradio("refresh");
                        //                                LoadSecurityRoleModule(vDataPOS, $('#lstPOS'), 0, strID);
                        //                                LoadSecurityRoleModule(vDataReceivingTransfer, $('#lstReceivingAndTransfers'), 0, strID);
                        //                                LoadSecurityRoleModule(vDataInventoryCount, $('#lstInvCount'), 0, strID);
                        //                                LoadSecurityRoleModule(vDataSFA, $('#lstSFA'), 0, strID);
                        //                                LoadSecurityRoleModule(vDataCRM, $('#lstCRM'), 0, strID);
                        //                                LoadSecurityRoleModule(vDataRequestsAndApprovals, $('#lstRequestsAndApprovals'), 0, strID);
                        //                                LoadSecurityRoleModule(vDataDashboard, $('#lstDashboardsAndReports'), 0, strID);
                        //                                LoadSecurityRoleModule(vDataSettings, $('#lstSettings'), 0, strID, true);
                        //                                LoadSecurityRoleModule(vDataMDM, $('#lstMDM'), 0, strID, true);
                        //                            }
                        //                            else {
                        //                                clearInterval(load);
                        //                            }
                        //                        }, 5000)

                    }
                });

            };

            function GetCRMDashboardModule(pData, strID) {
                var pLV;
                //Load CRM Dashboard
                pData.MainModule = "CRM Dashboard";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCrmDashboard) {
                        vDataCRMDashboard = vDataRoleModuleCrmDashboard;
                        $('#lstSettingsCRMDashboard').html('');//.listview('refresh');
                        pLV = $('#lstSettingsCRMDashboard');
                        LoadSecurityRoleModule(vDataCRMDashboard.SecurityRoleData.Rows, pLV, 0, strID);
                        GetCRMTabModule(pData, strID);
                    }
                });
            }

            function GetCRMTabModule(pData, strID) {
                var pLV;
                //Load CRM Tab
                pData.MainModule = "CRM Tab";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCrmTab) {
                        vDataCRMTab = vDataRoleModuleCrmTab;
                        $('#lstSettingsCRMTab').html('');//.listview('refresh');
                        pLV = $('#lstSettingsCRMTab');
                        LoadSecurityRoleModule(vDataCRMTab.SecurityRoleData.Rows, pLV, 0, strID);
                        GetCRMSettingsModule(pData, strID);
                    }
                });
            }

            function GetCRMSettingsModule(pData, strID) {
                var pLV;
                //Load CRM Settings
                pData.MainModule = "CRM Settings Tab";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCrmSettings) {
                        vDataCRMSettings = vDataRoleModuleCrmSettings;
                        $('#lstSettingsCRMSettingsTab').html('');//.listview('refresh');
                        pLV = $('#lstSettingsCRMSettingsTab');
                        LoadSecurityRoleModule(vDataCRMSettings.SecurityRoleData.Rows, pLV, 0, strID);
                    }
                });
            }

            function GetPOSModule(pData, strID) {
                var pLV;
                //Load POS
                pData.MainModule = "POS";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModulePOS) {
                        vDataPOS = vDataRoleModulePOS;
                        $('#lstPOS').html('');//.listview('refresh');
                        pLV = $('#lstPOS');
                        LoadSecurityRoleModule(vDataRoleModulePOS.SecurityRoleData.Rows, pLV, 0, strID);
                        GetPOModule(pData, strID);
                    }
                });
            }

            function GetPOModule(pData, strID) {
                var pLV;
                //Load POS
                pData.MainModule = "PO";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModulePOS) {
                        vDataPOS = vDataRoleModulePOS;
                        $('#lstPO').html('');//.listview('refresh');
                        pLV = $('#lstPO');
                        LoadSecurityRoleModule(vDataRoleModulePOS.SecurityRoleData.Rows, pLV, 0, strID);
                        GetReceivingTransModule(pData, strID);
                    }
                });
            }

            function GetReceivingTransModule(pData, strID) {
                var pLV;
                //Load Receiving and Transfers
                pData.MainModule = "Receiving and Transfers";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleRAT) {
                        vDataReceivingTransfer = vDataRoleModuleRAT;
                        $('#lstReceivingAndTransfers').html('');//.listview('refresh');
                        pLV = $('#lstReceivingAndTransfers');
                        LoadSecurityRoleModule(vDataRoleModuleRAT.SecurityRoleData.Rows, pLV, 0, strID);
                        GetInventoryCountModule(pData, strID);
                    }
                });
            }

            function GetInventoryCountModule(pData, strID) {
                var pLV;
                //Load Inventory count
                pData.MainModule = "Inventory Count";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleIC) {
                        vDataInventoryCount = vDataRoleModuleIC
                        $('#lstInvCount').html('');//.listview('refresh');
                        pLV = $('#lstInvCount');
                        LoadSecurityRoleModule(vDataRoleModuleIC.SecurityRoleData.Rows, pLV, 0, strID);
                        GetSFAModule(pData, strID);
                    }
                });
            }

            function GetSFAModule(pData, strID) {
                var pLV;
                //Load SFA
                pData.MainModule = "SFA";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleSFA) {
                        vDataSFA = vDataRoleModuleSFA;
                        $('#lstSFA').html('');//.listview('refresh');
                        pLV = $('#lstSFA');
                        LoadSecurityRoleModule(vDataRoleModuleSFA.SecurityRoleData.Rows, pLV, 0, strID);
                        GetCRMModule(pData, strID);
                    }
                });
            }

            function GetCRMModule(pData, strID) {
                var pLV;
                //Load CRM
                pData.MainModule = "CRM";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#lstCRM').html('');//.listview('refresh');
                        pLV = $('#lstCRM');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetRequestAppModule(pData, strID);
                    }
                });
            }

            function GetRequestAppModule(pData, strID) {
                var pLV;
                pData.MainModule = "Requests and Approvals";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleDAR) {
                        vDataRequestsAndApprovals = vDataRoleModuleDAR
                        $('#lstRequestsAndApprovals').html('');//.listview('refresh');
                        pLV = $('#lstRequestsAndApprovals');
                        LoadSecurityRoleModule(vDataRoleModuleDAR.SecurityRoleData.Rows, pLV, 0, strID);
                        GetSalesReturnsModule(pData, strID);
                    }
                });
            }

            function GetSalesReturnsModule(pData, strID) {
                var pLV;
                pData.MainModule = "Sales Returns";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleDAR) {
                        vDataSalesReturns = vDataRoleModuleDAR
                        $('#lstSalesReturns').html('');//.listview('refresh');
                        pLV = $('#lstSalesReturns');
                        LoadSecurityRoleModule(vDataRoleModuleDAR.SecurityRoleData.Rows, pLV, 0, strID);
                        GetDashboardReportModule(pData, strID);
                    }
                });
            }

            function GetDashboardReportModule(pData, strID) {
                var pLV;
                //Load Dashboard and Reports
                pData.MainModule = "Dashboard and Reports";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleDAR) {
                        vDataDashboard = vDataRoleModuleDAR
                        $('#lstDashboardsAndReports').html('');//.listview('refresh');
                        pLV = $('#lstDashboardsAndReports');
                        LoadSecurityRoleModule(vDataRoleModuleDAR.SecurityRoleData.Rows, pLV, 0, strID);
                        GetSettingsModule(pData, strID);
                    }
                });
            }

            function GetSettingsModule(pData, strID) {
                var pLV;
                //Load Settings
                pData.MainModule = "Settings";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleSettings) {
                        vDataSettings = vDataRoleModuleSettings;
                        $('#EasAccSystemAdministration').html('');//.listview('refresh');
                        pLV = $('#EasAccSystemAdministration');
                        LoadSecurityRoleModule(vDataRoleModuleSettings.SecurityRoleData.Rows, pLV, 0, strID, true);
                        GetMDMModule(pData, strID);
                    }
                });
            }

            function GetMDMModule(pData, strID) {
                var pLV;
                //Load MDM
                pData.MainModule = "MDM";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleMDM) {
                        vDataMDM = vDataRoleModuleMDM;
                        $('#lstMDM').html('');//.listview('refresh');
                        pLV = $('#lstMDM');
                        LoadSecurityRoleModule(vDataRoleModuleMDM.SecurityRoleData.Rows, pLV, 0, strID, true);
                        GetTroubleShootingModule(pData, strID);
                    }
                });
            }

            function GetTroubleShootingModule(pData, strID) {
                var pLV;
                //Load CRM
                pData.MainModule = "Troubleshooting and Maintenance Tools";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#lstTroubleshooting').html('');//.listview('refresh');
                        pLV = $('#lstTroubleshooting');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetErpModule(pData, strID);
                    }
                });
            }

            function GetErpModule(pData, strID) {
                var pLV;
                //Load CRM
                pData.MainModule = "ERP";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#lstErp').html('');//.listview('refresh');
                        pLV = $('#lstErp');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetDtrModule(pData, strID);
                    }
                });
            }

            function GetDtrModule(pData, strID) {
                var pLV;
                //Load DTR
                pData.MainModule = "DTR";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccDTR').html('');//.listview('refresh');
                        pLV = $('#EasAccDTR');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetAssetsManagementModule(pData, strID);
                    }
                });
            }

            function GetAssetsManagementModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModule = "AssetsManagement";
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccAssetMgmt').html('');//.listview('refresh');
                        pLV = $('#EasAccAssetMgmt');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccRevenueQuotationModule(pData, strID);
                    }
                });
            }

            function GetEasAccRevenueQuotationModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "8AAA458C-D73C-41A4-BC28-592A594097A0";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccRevenueQuotation').html('');//.listview('refresh');
                        pLV = $('#EasAccRevenueQuotation');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccRevenueOrderProcessingModule(pData, strID);
                    }
                });
            }
            
            function GetEasAccRevenueOrderProcessingModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "4CEA31B8-CFDE-41BF-8EA0-304682909516";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccRevenueOrderProcessing').html('');//.listview('refresh');
                        pLV = $('#EasAccRevenueOrderProcessing');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccRevenueOrderFulfillmentModule(pData, strID);
                    }
                });
            }
            
            function GetEasAccRevenueOrderFulfillmentModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "5E0CC3D6-589D-436D-A12B-0C0C3A4F8F5A";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccRevenueOrderFulfillment').html('');//.listview('refresh');
                        pLV = $('#EasAccRevenueOrderFulfillment');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccRevenueInvoicingModule(pData, strID);
                    }
                });
            }
            
            function GetEasAccRevenueInvoicingModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "1C44E443-FF5C-4C4D-89B0-DACD5E800A75";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccRevenueInvoicing').html('');//.listview('refresh');
                        pLV = $('#EasAccRevenueInvoicing');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccRevenueDeliveryTrackingModule(pData, strID);
                    }
                });
            }
            
            function GetEasAccRevenueDeliveryTrackingModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "4CBBB0F4-1EF9-41E3-8573-A7FD20B0BB5E";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccRevenueDeliveryTracking').html('');//.listview('refresh');
                        pLV = $('#EasAccRevenueDeliveryTracking');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccRevenueCreditAndCollectionModule(pData, strID);
                    }
                });
            }
            
            function GetEasAccRevenueCreditAndCollectionModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "5B6230B0-339E-411B-AE90-B852A4FAD1A5";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccRevenueCreditAndCollection').html('');//.listview('refresh');
                        pLV = $('#EasAccRevenueCreditAndCollection');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccRevenueDepositsModule(pData, strID);
                    }
                });
            }
            
            function GetEasAccRevenueDepositsModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "23F85CC9-211A-4D7E-A3E1-430F4EB134CC";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccRevenueDeposits').html('');//.listview('refresh');
                        pLV = $('#EasAccRevenueDeposits');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccRevenueCompetitorPriceCanvassingModule(pData, strID);
                    }
                });
            }

            function GetEasAccRevenueCompetitorPriceCanvassingModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "07254C9C-4926-4F4F-9310-E5229D48F464";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccRevenueCompetitorPriceCanvassing').html('');//.listview('refresh');
                        pLV = $('#EasAccRevenueCompetitorPriceCanvassing');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccDisbursementVendorPriceCanvassingModule(pData, strID);
                    }
                });
            }
            
            function GetEasAccDisbursementVendorPriceCanvassingModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "50e3c106-cefe-4a62-afb0-a8dbb64cb5ae";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccDisbursementVendorPriceCanvassing').html('');//.listview('refresh');
                        pLV = $('#EasAccDisbursementVendorPriceCanvassing');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccDisbursementPurchasingModule(pData, strID);
                    }
                });
            }

            function GetEasAccDisbursementPurchasingModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "A3F37487-1B2E-4206-8F21-9E719F0C088D";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccDisbursementPurchasing').html('');//.listview('refresh');
                        pLV = $('#EasAccDisbursementPurchasing');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccDisbursementStockReceivingModule(pData, strID);
                    }
                });
            }

            function GetEasAccDisbursementStockReceivingModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "BE0A1CCC-5239-4BC1-9FB2-207C1336898B";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccDisbursementStockReceiving').html('');//.listview('refresh');
                        pLV = $('#EasAccDisbursementStockReceiving');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccDisbursementJobOrderModule(pData, strID);
                    }
                });
            }

            function GetEasAccDisbursementJobOrderModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "18595245-9A1B-4951-A727-7A38C5754C2A";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccDisbursementJobOrder').html('');//.listview('refresh');
                        pLV = $('#EasAccDisbursementJobOrder');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccDisbursementAccountsPayabaleVerificationModule(pData, strID);
                    }
                });
            }

            function GetEasAccDisbursementAccountsPayabaleVerificationModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "25207625-AE7D-406E-8D63-439AE038295E";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccDisbursementAccountsPayabaleVerification').html('');//.listview('refresh');
                        pLV = $('#EasAccDisbursementAccountsPayabaleVerification');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccDisbursementDisbursementsModule(pData, strID);
                    }
                });
            }

            function GetEasAccDisbursementDisbursementsModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "CB82BAE0-EFCC-41A5-B46E-72047D765950";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccDisbursementDisbursements').html('');//.listview('refresh');
                        pLV = $('#EasAccDisbursementDisbursements');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccDisbursementChequeClearingModule(pData, strID);
                    }
                });
            }

            function GetEasAccDisbursementChequeClearingModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "B91E3B31-ACD8-416D-ABD6-042C1E2E45A9";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccDisbursementChequeClearing').html('');//.listview('refresh');
                        pLV = $('#EasAccDisbursementChequeClearing');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccDisbursementExpenseTrackerModule(pData, strID);
                    }
                });
            }

            function GetEasAccDisbursementExpenseTrackerModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "82fff94a-1e11-4e08-a73e-4b29c09ba948";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccDisbursementExpenseTracking').html('');//.listview('refresh');
                        pLV = $('#EasAccDisbursementExpenseTracking');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccInventoryHandlingModule(pData, strID);
                    }
                });
            }

            function GetEasAccInventoryHandlingModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "5A234152-B1B4-4C94-AA9D-8ABFC44E5B55";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccInventoryHandling').html('');//.listview('refresh');
                        pLV = $('#EasAccInventoryHandling');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccProjectMgmtModule(pData, strID);
                    }
                });
            }

            function GetEasAccProjectMgmtModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "6A76BBFC-B69C-431A-AC1A-CE095F62CEC6";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccProjectMgmt').html('');//.listview('refresh');
                        pLV = $('#EasAccProjectMgmt');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccCRMModule(pData, strID);
                    }
                });
            }

            function GetEasAccCRMModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "611E251E-0AB0-4A8C-94F5-1355B33A74C5";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccCRM').html('');//.listview('refresh');
                        pLV = $('#EasAccCRM');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccGPSModule(pData, strID);
                    }
                });
            }

            function GetEasAccGPSModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "B30C9241-FF3E-4D8E-9484-A229C85BBADF";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccGPS').html('');//.listview('refresh');
                        pLV = $('#EasAccGPS');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccFinancialStatementModule(pData, strID);
                    }
                });
            }

            function GetEasAccFinancialStatementModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "559D79E4-EDD1-41A7-84F2-E21E2F5EC188";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccFinancialStatement').html('');//.listview('refresh');
                        pLV = $('#EasAccFinancialStatement');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetGatePassCreationModule(pData, strID);
                    }
                });
            }

            function GetGatePassCreationModule(pData, strID) {
                var pLV;
                //Load Asset Mgmt
                pData.MainModuleID = "511afa68-dae9-4657-8dd2-3f355449285b";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccGatePassCreation').html('');//.listview('refresh');
                        pLV = $('#EasAccGatePassCreation');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetComplianceRemindersModule(pData, strID);
                    }
                });
            }

            function GetComplianceRemindersModule(pData, strID) {
                var pLV;
                pData.MainModuleID = "8d112700-24b7-4f07-a961-274aee4155ff";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccComplianceReminders').html('');//.listview('refresh');
                        pLV = $('#EasAccComplianceReminders');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                        GetEasAccRevenueMainModule(pData, strID);
                    }
                });
            }

            function GetEasAccRevenueMainModule(pData, strID) {
                var pLV;
                pData.MainModuleID = "505AB19B-236B-4F46-86B2-BCA1002F222A";
                delete pData.MainModule;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModules",
                    data: pData,
                    success: function (vDataRoleModuleCRM) {
                        vDataCRM = vDataRoleModuleCRM;
                        $('#EasAccRevenueMain').html('');//.listview('refresh');
                        pLV = $('#EasAccRevenueMain');
                        LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);
                    }
                });
            }

            function LoadSecurityRoleModule(vRows, pLV, intCurIndex, strRoleID, isClear, vIndentation) {

                var pItem;
                var info;
                var strindentation = (vIndentation == undefined ? "" : vIndentation);

                if (intCurIndex > vRows.length - 1) {
                    return;
                }

                if (isClear) {
                    pLV.html('');
                }                

                var pLabel = vRows[intCurIndex].Label;
                if (CStr2(pLabel).trim() == '')
                    pLabel = vRows[intCurIndex].ModuleName;

                var hideModule = ['cec2dad9-a1f1-4a5f-a4c0-2964dd5e64e3', '4aaa58c9-4726-4259-822d-2d64401bfb9c',
                    'bccc8fc4-b3a0-4f8a-a04e-48659cc58ce9', 'bcc4e0b5-3f61-4008-87d3-509be101177c',
                    '15424a2d-b00b-411f-b19f-62c9191e89c6', '9219c6af-5b44-413b-9b73-9ea6df94a64a',
                    'd41ee1fd-746c-481e-a603-bb7a37e5ebe1', 'bf7bd33f-b33a-4521-abe5-156bb7b34968',
                    'd3be485f-d335-4835-adb1-eed4421f1a40', //'340331f8-3de8-440b-b74a-e3da589bcc3e',
                    'be6d8d9f-796a-4cc2-b123-08372945954d', '7f650669-f263-4d8f-9d0e-795df9bb062e',
                    'bccc8fc4-b3a0-4f8a-a04e-48659cc58ce9']
                //MNGR SR
                //reconstruct sales data, update cache db
                //online zread, order taker setup
                //Allow approval of contact person's request to access Online Store?, Allow approval of contact person's request to access Vendor Portal?
                //Allow to Free Type a Price, Allow to reset employee Mobiz's account
                //Allow to sync data from franchisor database, Zero out / clear transaction history for this Device+FOWST location
                //Reconstruct Sales Data
                if (hideModule.includes(vRows[intCurIndex].ModuleID) && sessionStorage.OrgID == '565c260c-3844-4069-ac41-d26f0598e562') {
                    //hide modules temporarily
                    LoadSecurityRoleModule(vRows, pLV, intCurIndex + 1, strRoleID, false, strindentation);
                    return;
                }

                var table = $('<table style="width:100%;">'), tr = $('<tr>'), modName = window.GetNewModuleName(pLabel);

                if ($("#lbl" + vRows[intCurIndex].ModuleID).length > 0 && $("[modname='" + modName + "']").length > 0) {
                    LoadSecurityRoleModule(vRows, pLV, intCurIndex + 1, strRoleID, false, strindentation);
                    return;
                }

                tr.append($('<td>')
                        .attr('id', "lbl" + vRows[intCurIndex].ModuleID)
                        .attr('modname', modName)
                        .data('Data', vRows[intCurIndex])
                        .html(strindentation + modName)
                        .append($('<a data-role="none" class="btn btn-light btn-sm">')
                            .css('cursor', 'pointer')
                            .attr('id', 'info-' + ctrList)
                            .html(' <img alt="' + (vRows[intCurIndex].Description == "" ? "none" : vRows[intCurIndex].Description.replace(/(["])/g, "&quot;")) + '" src="/Content/images/info.png" style="vertical-align:middle;">')
                            .click(function () {
                                if (vRows[intCurIndex].Description != "") {
                                    NewMessageBox($(this).find('img').attr('alt'), 'Info');
                                }
                            })
                        )
                    )

                ctrList = ctrList + 1;

                if (vRows[intCurIndex].IsHeader == 0) {
                    tr.append($('<td class="text-right">')
                            .append($('<div class="btn-group" role="group" aria-label="Basic radio toggle button group">')
                                .append($('<input type="radio" class="btn-check" autocomplete="off">')
                                    .attr('id', "rdoYes" + vRows[intCurIndex].ModuleID)
                                    .attr('name', "rdo" + vRows[intCurIndex].ModuleID)
                                    .attr('checked', (vRows[intCurIndex].IsHasAccess == 1 ? true : false))
                                ).append($('<label class="btn btn-outline-secondary">')
                                    .attr('for', "rdoYes" + vRows[intCurIndex].ModuleID)
                                    .text('Yes')
                                )
                                .append($('<input type="radio" class="btn-check" autocomplete="off">')
                                    .attr('id', "rdoNo" + vRows[intCurIndex].ModuleID)
                                    .attr('name', "rdo" + vRows[intCurIndex].ModuleID)
                                    .attr('checked', (vRows[intCurIndex].IsHasAccess == 0 ? true : false))
                                ).append($('<label class="btn btn-outline-secondary">')
                                    .attr('for', "rdoNo" + vRows[intCurIndex].ModuleID)
                                    .text('No')
                                )
                            )
                        )

                    table.append(tr)

                    pItem = $('<li class="list-group-item list-group-item-action">')
                    .append(table)

                    pLV.append(pItem);
                    //pLV.trigger("create");
                    //pLV;//.listview('refresh');

                    if (vRows[intCurIndex].Description == "") {
                        $('a[id="info-' + (ctrList - 1) + '"]').css({ visibility: 'hidden', display: 'none' });
                    }

                    LoadSecurityRoleModule(vRows, pLV, intCurIndex + 1, strRoleID, false, strindentation);

                } else {
                    tr.find('td').css('font-weight', 'bold');

                    table.append(tr);

                    pItem = $('<li class="list-group-item list-group-item-action">')
                    .append(table)

                    pLV.append(pItem);
                    //pLV.trigger("create");
                    //pLV;//.listview('refresh');

                    if (vRows[intCurIndex].Description == "") {
                        $('a[id="info-' + (ctrList - 1) + '"]').css({ visibility: 'hidden', display: 'none' });
                    }

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

            function ClearSecurityRoleFields() {

                $("#txtSecurityRoleID").val('');
                $("#txtRoleCode").val('');
                $("#txtRole").val('');
                $("#txtRoleDescription").val('');
                $('#chkRoleIsActive').attr("checked", true);//.checkboxradio("refresh");
            }

            function SaveSecurityRoleInfo() {
                var pData = [];
                var pDataRoleMod = [];
                var pDataRoleModCntr = 0;

                //Modules
                pData.SecurityRoleID = $.trim($("#txtSecurityRoleID").val());
                pData.IsCrm = isCrm;
                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModulesWOHeader",
                    data: pData,
                    success: function (vDataRoleModulePOS) {

                        if (vDataRoleModulePOS.SecurityRoleData.Rows.length > 0) {
                            for (intCntr = 0; intCntr <= vDataRoleModulePOS.SecurityRoleData.Rows.length - 1; intCntr++) {

                                var vModData = vDataRoleModulePOS.SecurityRoleData.Rows[intCntr];

                                if (vModData.IsHeader == 0) {

                                    if ($("#rdoYes" + vModData.ModuleID) != undefined) {
                                        pDataRoleMod[pDataRoleModCntr] = {
                                            ID: (vModData.ID || ''),
                                            ModuleID: vModData.ModuleID,
                                            IsHasAccess: ($("#rdoYes" + vModData.ModuleID).is(":checked") ? 1 : 0),
                                            RecState: "Edit"
                                        };
                                        pDataRoleModCntr = pDataRoleModCntr + 1;
                                    }
                                }
                            }

                            pData = {
                                SecurityRoleID: $.trim($("#txtSecurityRoleID").val()),
                                Code: $.trim($("#txtRoleCode").val()),
                                Role: $.trim($("#txtRole").val()),
                                Description: $.trim($("#txtRoleDescription").val()),
                                IsActive: ($("#chkRoleIsActive").is(":checked") ? 1 : 0),
                                SecurityRoleModule: pDataRoleMod,
                                AccessMenu: []
                            };

                            $('[accessrightmenu]').each(function name(params) {
                                pData.AccessMenu.push({
                                    Menu: $(this).attr('accessrightmenu'),
                                    Status: $(this).find('input[type="radio"]:checked').attr('value')
                                })
                            })

                            AjaxSend({
                                url: "SecurityRole/CheckUniqueSecurityRoleCode",
                                data: pData,
                                success: function (vData) {

                                    if (vData.SecurityRoleData.Rows.length > 0) {
                                        NewMessageBox("Code already exist.",
                                            "Code",
                                            [
                                                { Text: 'Ok',
                                                    Click: function () {
                                                        history.back();
                                                    }
                                                }
                                            ]
                                        );

                                    }
                                    else {

                                        AjaxSend({
                                            url: "SecurityRole/CheckUniqueSecurityRole",
                                            data: pData,
                                            success: function (vData) {

                                                if (vData.SecurityRoleData.Rows.length > 0) {
                                                    NewMessageBox("Security role already exist.",
                                                        "Security Role",
                                                        [
                                                            { Text: 'Ok',
                                                                Click: function () {
                                                                    history.back();
                                                                }
                                                            }
                                                        ]
                                                    );

                                                } else {

                                                    AjaxSend({
                                                        url: "SecurityRole/SaveSecurityRole",
                                                        data: pData,
                                                        success: function (vSaveData) {

                                                            //                                                            $("#txtSecurityRoleID").val(vSaveData.SecurityRoleData || '');
                                                            //window.location = GetPathFormat("/SecurityRole/SecurityRoleSummary?OrgID=" + sessionStorage.OrgID);
                                                            if (sessionStorage.AddViewLibBCName == "View Security Role") {
                                                                sessionStorage["AddViewLibBCName"] = "View Security Role";
                                                                window.location.hash = '';
                                                                window.location.hash = 'UniversalPage';
                                                                if (!isCrm) {
                                                                    LibraryTable.ViewSecurityRolePage();
                                                                } else {
                                                                    CRMLibraryTable.ViewSecurityRolePage();
                                                                }
                                                                
                                                            }
                                                            else {
                                                                $('#tblBody').html('');
                                                                window.location.hash = '';
                                                                window.location.hash = 'LibraryPage';
                                                                if (!isCrm) {
                                                                    LibraryTable.SecurityRoles();
                                                                } else {
                                                                    CRMLibraryTable.SecurityRoles();
                                                                }                                                               
                                                            }

                                                            //                                                            window.IsPageLoaded = false;
                                                            //                                                            if (window.pageSourcePage != undefined) {
                                                            //                                                                $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
                                                            //                                                            }
                                                            //                                                            else {
                                                            //                                                                ChangePage('/SecurityRole/SecurityRoleSummary');
                                                            //                                                            }
                                                        }
                                                    });
                                                }

                                            }
                                        });

                                    }

                                }
                            });




                        }

                    }
                });

            };
        });
    });