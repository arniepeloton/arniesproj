require([
    'common'
    ], function () {

        if (IsMobile)
            Android.SetOfflineMode(true);

        require([(OfflineMode ? '/Scripts/sources/People/PeopleMenu_offline.js?crc=' : ''),
                 (OfflineMode ? '/Scripts/sources/ImportExportData/ImportExportData.js?crc=' : ''),
                 (OfflineMode ? '/Scripts/sources/ImportExportData/ImportExportData_offline.js?crc=' : '')
        ], function () {

            $(function () {

                var mIconCss = {
                    padding: '10px',
                    width: '60px',
                    height: '60px'
                };


                //ADDITIONAL BY JANINE Sept. 9, 2013 1pm
                CheckIfBackIsClicked();

                //Check hash change
                window.onhashchange = function () {
                    CheckIfBackIsClicked();
                }

                function CheckIfBackIsClicked() {
                    if (location.hash.length < 1) { //check if there is no hash (ex. #pagePopUp...)

                        ShowPeopleMenu(); //if true, it means it's safe to reload people menu, if false, dont reload so the orgID will not be removed!


                    }
                }

                function ShowPeopleMenu() {
                    // ORIGINAL CODE
                    //                AjaxSend({
                    //                    url: 'People/IsNewEntry',
                    //                    success: function (vData) {

                    //                        sessionStorage.removeItem("OrgID");
                    //                        sessionStorage.removeItem("FowstID");

                    //                        ShowPersonNameInfo(vData.PeopleID)

                    //                    }
                    //                });

                    // EDITED by JANINE

                    if (OfflineMode) {
                        ShowPersonNameInfo();
                    }
                    else {
                        AjaxSend({
                            url: 'People/IsNewEntry',
                            success: function (vData) {
                                if (vData.PeopleID == '') { //if people ID is blank, then redirect to sign in
                                    pUrl = '/SignIn';
                                    window.open(GetPathFormat(pUrl), '_self');
                                } else {
                                    var pData = {};
                                    pData.ID = vData.PeopleID;

                                    //Check if IsFirstPartySignUp is 1 or 0

                                    AjaxSend({
                                        url: "SignIn/CheckPeopleIsFirstPartySignUp",
                                        data: pData,
                                        success: function (vDataFirstParty) {
                                            //if 0, redirect Account Management to reset credentials before he/she can access PeopleMenu
                                            if (vDataFirstParty.PeopleFirstPartySignUpData.Rows[0].IsFirstPartySignUp == '0') {
                                                pUrl = '/AccountManagement';
                                                window.open(GetPathFormat(pUrl), '_self');
                                            } else {
                                                sessionStorage.removeItem("OrgID");
                                                sessionStorage.removeItem("FowstID");

                                                ShowPersonNameInfo(vData.PeopleID);

                                            }
                                        }
                                    });
                                }

                            }
                        });
                    };
                }

                $('#btnPeopleMenuPopUpMenu').click(function (e) {
                    e.preventDefault();

                    $("#divPeopleMenuPopUp").popup("open")

                });

                $('#popPeopleMenuPeopleInfoSummary').click(function (e) {
                    e.preventDefault();

                    window.open(GetPathFormat("PeopleInfoSummary"), "_self");

                });

                ///Dashboards and Reports --------------------------------------------------------------------------------------------------------------------------------------------
                //            $('#liSales').click(function (e) {
                //                e.preventDefault();

                //                $.mobile.changePage('#pagePopUpMenuPOSReports');

                //            });

                $('#popReportsPOS').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopUpMenuPOSReports');

                });

                $('#popTroubleshootingToolsReprintAReceipt').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('POS Receipt Reprint', {
                        OnClickOKSuccess: function () {
                            window.open(GetPathFormat("/SalesPayments/DuplicateReceipt") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                        }
                    });

                });

                $('#popTroubleshootingToolsUpdateDatabase').click(function (e) {
                    e.preventDefault();

                    GetUser();

                    function GetUser() {

                        AjaxSend({
                            url: 'Login3/GetUser',
                            success: function (vData) {
                                if (vData.Error != undefined)
                                    MessageBox(vData.Error);
                                else {
                                    window.pageSecuritySourcePage = $.mobile.activePage;

                                    MessageBox("Are you sure you want to get latest update from Mobiz?",
                                    "Import",
                                    [
                                        { Text: 'Yes',
                                            Click: function () {
                                                var pSql = '';
                                                var pDt;
                                                var pDTData = [];

                                                Sql.AttachDatabase(sessionStorage.OrgID);

                                                pSql = 'Select * from private.TablesLastUpdated';
                                                pDt = Sql.Execute(pSql);

                                                for (var i = 0; i < pDt.Rows.length; i++) {
                                                    pDTData[i] = pDt.Rows[i].TableName + ',' + pDt.Rows[i].LastUpdated
                                                }

                                                RequestDatabaseUpdate(sessionStorage.OrgID, pDTData);
                                                $('.ui-dialog').dialog('close');
                                            }
                                        },
                                        { Text: 'No' }
                                    ]);
                                }
                            },
                            error: function (vError) {
                                if (IsMobile)
                                    proj.PromptRequiresInternet({
                                        Retry: GetUser
                                    });
                                else {
                                    console.log('Error getting Login/GetUser. Retrying...');
                                    GetUser();
                                };
                            }
                        });
                    }

                });


                $('#popTroubleshootingToolsReconstructSalesData').click(function (e) {
                    e.preventDefault();

                    GetUser();

                    function GetUser() {

                        AjaxSend({
                            url: 'Login3/GetUser',
                            success: function (vData) {
                                if (vData.Error != undefined)
                                    MessageBox(vData.Error);
                                else {
                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                    PromptUserAccess('Reconstruct Sales Data', {
                                        OnClickOKSuccess: function () {
                                            window.open('/SalesMenu?' +
                                                        $.param({
                                                            OrgID: sessionStorage["OrgID"],
                                                            mode: 'pos',
                                                            rsd: 1
                                                        })
                                        , "_self");
                                        }
                                    });
                                }
                            },
                            error: function (vError) {
                                if (IsMobile)
                                    proj.PromptRequiresInternet({
                                        Retry: GetUser
                                    });
                                else {
                                    console.log('Error getting Login/GetUser. Retrying...');
                                    GetUser();
                                };
                            }
                        });
                    }
                });

                //                $('#popPOSReportCashierSalesReport').click(function (e) {
                //                    e.preventDefault();




                //                    if (IsMobile) {
                //                        window.open(GetPathFormat("/ReportsPOS/SalesReport") + "?OrgID=" + sessionStorage["OrgID"] + "&IsManagerSalesReport=0&IsMobile=1", "_self");
                //                    } else {

                //                        window.pageSecuritySourcePage = $.mobile.activePage;
                //                        PromptUserAccess('Cashier Sales Report', {
                //                            OnClickOKSuccess: function () {
                //                                var pData = {};
                //                                pData.POSSalesReportType = 1;

                //                                AjaxSend({
                //                                    url: 'ReportsPOS/setPOSSalesReportType',
                //                                    data: pData,
                //                                    success: function (vData) {
                //                                        window.open(GetPathFormat("/ReportsPOS/SalesReport") + "?OrgID=" + sessionStorage["OrgID"] + "&IsManagerSalesReport=0", "_self");
                //                                    }
                //                                });
                //                            }
                //                        });
                //                    }

                //                });

                //                $('#popPOSReportManagerReport').click(function (e) {
                //                    e.preventDefault();

                //                    if (IsMobile) {
                //                        window.open(GetPathFormat("/ReportsPOS/SalesReport") + "?OrgID=" + sessionStorage["OrgID"] + "&IsManagerSalesReport=1&IsMobile=1", "_self");
                //                    } else {
                //                        window.pageSecuritySourcePage = $.mobile.activePage;
                //                        PromptUserAccess('Manager Sales Report', {
                //                            OnClickOKSuccess: function () {
                //                                var pData = {};
                //                                pData.POSSalesReportType = 0;

                //                                AjaxSend({
                //                                    url: 'ReportsPOS/setPOSSalesReportType',
                //                                    data: pData,
                //                                    success: function (vData) {
                //                                        window.open(GetPathFormat("/ReportsPOS/SalesReport") + "?OrgID=" + sessionStorage["OrgID"] + "&IsManagerSalesReport=1", "_self");
                //                                    }
                //                                });
                //                            }
                //                        });
                //                    }

                //                });

                //                $('#popPOSReportSalesAdjustment').click(function (e) {
                //                    e.preventDefault();

                //                    window.pageSecuritySourcePage = $.mobile.activePage;
                //                    PromptUserAccess('Sales Adjustment Report', {
                //                        OnClickOKSuccess: function () {
                //                            window.open(GetPathFormat("/ReportsPOS/SalesAdjustmentReport") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //                        }
                //                    });

                //                });

                //                $('#popPOSReportZReport').click(function (e) {
                //                    e.preventDefault();

                //                    window.pageSecuritySourcePage = $.mobile.activePage;
                //                    PromptUserAccess('Z Report', {
                //                        OnClickOKSuccess: function () {
                //                            window.open(GetPathFormat("/ReportsPOS/ZReport") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //                        }
                //                    });

                //                });

                //                $('#popPOSReportMallLessorReport').click(function (e) {
                //                    e.preventDefault();

                //                    $.mobile.changePage('#pagePopUpMenuPOSReportsMallLessorReport');

                //                });

                //                $('#popPOSReportMallLessorAyala').click(function (e) {
                //                    e.preventDefault();

                //                    window.pageSecuritySourcePage = $.mobile.activePage;
                //                    PromptUserAccess('Mall Lessor Report', {
                //                        OnClickOKSuccess: function () {

                //                            window.open(GetPathFormat("/MallLessorAyala") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //                        }
                //                    });

                //                });

                //                $('#popPOSReportMallLessorSM').click(function (e) {
                //                    e.preventDefault();

                //                    window.pageSecuritySourcePage = $.mobile.activePage;
                //                    PromptUserAccess('Mall Lessor Report', {
                //                        OnClickOKSuccess: function () {

                //                            window.open(GetPathFormat("/MallLessorSM") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //                        }
                //                    });

                //                });

                //                $('#popPOSReportPrincipal').click(function (e) {
                //                    window.open(GetPathFormat("/ReportToPrincipal") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //                });

                //                $('#popPOSReportShopperTrak').click(function (e) {
                //                    window.open(GetPathFormat("/ShopperTrak") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //                });

                $('#btnPOSReportBack').click(function (e) {
                    e.preventDefault();

                    window.open(GetPathFormat("/People/PeopleMenu"), "_self");

                });

                $('#btnDashboardsAndReportsBack').click(function (e) {
                    e.preventDefault();

                    window.open(GetPathFormat("/People/PeopleMenu"), "_self");

                });


                ///END Dashboards and Reports --------------------------------------------------------------------------------------------------------------------------------------------

                ///Trouble Shooting Tools --------------------------------------------------------------------------------------------------------------------------------------------
                $('#btnTroubleshootingToolsBack').click(function (e) {
                    e.preventDefault();

                    window.open(GetPathFormat("/People/PeopleMenu"), "_self");

                });
                ///END Trouble Shooting Tools --------------------------------------------------------------------------------------------------------------------------------------------

                ///System Administration --------------------------------------------------------------------------------------------------------------------------------------------
                $('#popSecuritySettings').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopUpMenuSecuritySettings');

                });

                $('#popSystemSettings').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopSystemSettings');

                });

                $('#popDataLibraries').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopDataLibraries');

                });



                $('#popSecuritySettingsRoles').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Security Roles', {
                        OnClickOKSuccess: function () {
                            window.open(GetPathFormat("/SecurityRole") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                        }
                    });
                });

                $('#popSecuritySettingsDevices').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Devices', {
                        OnClickOKSuccess: function () {
                            //window.open(GetPathFormat("/Device") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/Device');
                        }
                    });


                });

                $('#popCompanyProfile').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Company Profile', {
                        OnClickOKSuccess: function () {
                            ShowOrganizationProfile();
                        }
                    });

                });

                $('#popCompanyPlaces').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Company Places (FOWST)', {
                        OnClickOKSuccess: function () {
                            ShowOrganizationPlaces();
                        }
                    });

                });

                $('#popCompanyEmployees').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopUpEmployeeSettings');
                });

                $('#popEmployeeList').click(function () {
                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Company Employee', {
                        OnClickOKSuccess: function () {
                            ShowOrganizationEmployees();
                        }
                    });
                });

                $('#popEmployeeWorkSchedule').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;

                    ShowOrganizationEmployeeWorkingHours();
                });

                $('#popCompanyProducts').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopUpMenuProducts');

                });

                $('#popPIMProductCompanyWide').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Company-Wide Product List', {
                        OnClickOKSuccess: function () {

                            ShowOrganizationProduct();
                        }
                    });

                });

                $('#popPIMFOWSTProduct').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Fowst Level Product List', {
                        OnClickOKSuccess: function () {

                            ShowOrganizationFOWSTProduct();
                        }
                    });

                });

                $('#popPIMProductPrices').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Product Price', {
                        OnClickOKSuccess: function () {

                            window.open(GetPathFormat("/PIMProductPrice") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                        }
                    });

                });

                $('#popPIMUploadDownloadProduct').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopUpMenuProductsUploadDownload');

                });

                $('#popPIMUpload').click(function (e) {
                    e.preventDefault();

                    window.open(GetPathFormat("/PIMUpload") + "?OrgID=" + sessionStorage["OrgID"], "_self");

                });

                $('#btnPIMUploadDownloadBack').click(function (e) {
                    e.preventDefault();

                    $('#btnSettingsBack').click();

                });

                $('#popPIMGroups').click(function (e) {
                    e.preventDefault();

                    location = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        IsMobile: CBool(IsMobile) ? 1 : 0
                    }, '/ProductGroup');

                });

                $('#popPIMDownloadProductTemplate').click(function (e) {
                    e.preventDefault();

                    //                window.pageSecuritySourcePage = $.mobile.activePage;
                    //                PromptUserAccess('Product Price', {
                    //                    OnClickOKSuccess: function () {

                    window.open(GetPathFormat("/PIMTemplate/PIMGenerateTemplate") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    //                    }
                    //                });

                });

                $('#popPIMImportProductTemplate').click(function (e) {
                    e.preventDefault();

                    //                window.pageSecuritySourcePage = $.mobile.activePage;
                    //                PromptUserAccess('Product Price', {
                    //                    OnClickOKSuccess: function () {

                    window.open(GetPathFormat("/PIMTemplate") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    //                    }
                    //                });

                });

                $('#popCompanyCustomers').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Company Customer', {
                        OnClickOKSuccess: function () {
                            ShowOrganizationCustomers();
                        }
                    });

                });

                $('#popCompanySuppliers').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Company Supplier', {
                        OnClickOKSuccess: function () {
                            ShowOrganizationSuppliers();
                        }
                    });

                });

                $('#popSysAdminMallLessor').click(function (e) {
                    e.preventDefault();

                    $('#popPOSReportMallLessorReport').click();

                });

                $('#popReceiptSettings').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('POS Receipt Configuration', {
                        OnClickOKSuccess: function () {
                            window.open(GetPathFormat("/ReceiptSettings") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                        }
                    });

                });

                $('#popSalesReturnsSettings').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Sales Return Settings', {
                        OnClickOKSuccess: function () {
                            window.open(GetPathFormat("/SalesReturns/Settings") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                        }
                    });

                });

                //ADDITIONAL by Janine 
                $('#popDiscountSettings').click(function (e) {
                    e.preventDefault();

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Discount Settings', {
                        OnClickOKSuccess: function () {
                            window.open(GetPathFormat("/DiscountSettings") + "?OrgID=" + sessionStorage["OrgID"], "_self");

                        }
                    });

                });

                $('#btnSettingsBack').click(function (e) {
                    e.preventDefault();

                    window.open(GetPathFormat("/People/PeopleMenu"), "_self");
                });

                $('#btnSecuritySettingsBack').click(function (e) {
                    e.preventDefault();

                    $('#btnSettingsBack').click();

                });

                $('#btnPIMBack').click(function (e) {
                    e.preventDefault();

                    $('#btnSettingsBack').click();
                });

                $('#btnSystemSettingsBack').click(function (e) {
                    e.preventDefault();

                    $('#btnSettingsBack').click();
                });

                $('#btnDataLibrariesBack').click(function (e) {
                    e.preventDefault();

                    $('#btnSettingsBack').click();
                });

                $('#btnEmployeesBack').click(function (e) {
                    e.preventDefault();

                    $('#btnSettingsBack').click();
                });

                $('#btnDashboardAndReportsSettingBack').click(function (e) {
                    e.preventDefault();

                    $('#btnSettingsBack').click();
                });
                ///END System Administration --------------------------------------------------------------------------------------------------------------------------------------------

                ///Control Panel --------------------------------------------------------------------------------------------------------------------------------------------
                //janine feb 5 5pm

                $('#btnControlPanelBack').click(function (e) {
                    e.preventDefault();

                    $('#btnSettingsBack').click();
                });

                $('#btnCreateMessageBack').click(function (e) {
                    e.preventDefault();

                    $('#btnSettingsBack').click();
                });

                $('#popSubscriptionDashboard').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopUpSubscriptionDashboard');

                    //                    window.pageSecuritySourcePage = $.mobile.activePage;
                    //                    PromptUserAccess('Devices', {
                    //                        OnClickOKSuccess: function () {
                    //                            //window.open(GetPathFormat("/Device") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    //                            location = MergeUrlParameters({
                    //                                OrgID: sessionStorage.OrgID,
                    //                                IsMobile: CBool(IsMobile) ? 1 : 0
                    //                            }, '/Device');
                    //                        }
                    //                    });
                });

                $('#popCreateMessage').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopUpCreateMessage');

                });


                $('#btnSaveCreateMessage').click(function (e) {

                    e.preventDefault();

                    if ($.trim($('#selProduct').val()) == '') {
                        MessageBox("Please select a product",
                                    "Product",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                        return;
                    }

                    if ($.trim($('#txtTitle').val()) == '') {
                        MessageBox("Please enter a title",
                                    "Title",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                        return;
                    }


                    if ($.trim($('#txtMessage').val()) == '') {
                        MessageBox("Please enter your message",
                                    "Message",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                        return;
                    }

                    SaveUpdateMessageInfo();

                });


                ///END Control Panel ---

                ///MDM Administration --------------------------------------------------------------------------------------------------------------------------------------------
                //tusoy oct 7, 2013 3:00pm
                $('#popSecuritySettingsMDM').click(function (e) {
                    e.preventDefault();
                    $.mobile.changePage('#pagePopUpMenuMDMSettings');
                });


                //tusoy oct 7, 2013 3:00pm
                $('#popSecurityMDMSettingsPolicies').click(function (e) {
                    e.preventDefault();
                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Devices', {
                        OnClickOKSuccess: function () {
                            window.open(GetPathFormat("/DeviceMDMConfig") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                        }
                    });

                });


                //tusoy oct 7, 2013 3:00pm
                $('#popSecurityMDMSettingsCompanyWide').click(function (e) {
                    e.preventDefault();
                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Devices', {
                        OnClickOKSuccess: function () {
                            window.open(GetPathFormat("/DeviceAppsWhiteList") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                        }
                    });

                });

                $('#btnSecurityMDMSettingsBack').click(function (e) {
                    e.preventDefault();

                    $('#btnSettingsBack').click();

                });


                ///END MDM Administration --------------------------------------------------------------------------------------------------------------------------------------------

                $('#btnPeopleMsgT2BackToHome').click(function (e) {
                    e.preventDefault();

                    window.open(GetPathFormat("/People/PeopleMenu"), "_self");

                });

                $('#btnPeopleMsgT2ListBackToHome').click(function (e) {
                    e.preventDefault();

                    window.open(GetPathFormat("/People/PeopleMenu"), "_self");

                });

                $('#btnSignOut').click(function () {

                    SignOut();

                });

                $('#btnPeopleMenuPeopleImage').click(function (e) {
                    e.preventDefault();

                    $('#popPeopleMenuPeopleInfoSummary').click();

                });

                $('#lblName').click(function (e) {
                    e.preventDefault();

                    $('#popPeopleMenuPeopleInfoSummary').click();

                });

                $('#btnPeopleMsgT1Confirm').click(function () {

                    ConfirmPeopleMsgT1();

                });

                $('#btnPeopleMsgT1Deny').click(function () {
                    alert("Not yet implemented.");
                });

                $('#btnPeopleMsgT1Cancel').click(function () {
                    $.mobile.changePage('#pageHome');
                });


                $('#btnPeopleMenuFindEstablishment').click(function (e) {
                    e.preventDefault();

                    window.open(GetPathFormat("/OrganizationShort"), "_self");

                });

                $('#tabHome').click(function () {

                    $('#divHome').show();
                    $('#divApps').hide();
                    $('#divFind').hide();

                    ShowNotifications();

                });

                $('#tabApps').click(function () {

                    $('#divHome').hide();
                    $('#divApps').show();
                    $('#divFind').hide();

                });

                $('#tabFind').click(function () {

                    $('#divHome').hide();
                    $('#divApps').hide();
                    $('#divFind').show();

                });

                function GotoSigninPage() {

                    var pParam = {};
                    if (IsMobile)
                        pParam.IsMobile = 1;
                    pParam = $.param(pParam);
                    window.location = '/SignIn?' + pParam;

                }; // GotoSigninPage

                function SignOut() {

                    AjaxSend({
                        url: "People/setSignOut",
                        success: function (vData) {
                            window.open(GetPathFormat("/SignIn?mode=logout"), "_self"); // window.open(GetPathFormat("/People"), "_self"); EDITED BY: JANINE , Aug 3 4pm
                        }
                    });

                }; // SignOut

                function ShowPersonNameInfo(strID) {
                    var pData = {};
                    pData.PeopleID = strID;

                    ClearPersonFields();

                    if (OfflineMode) {

                        vData = PeopleMenu_offline().getPeopleInfoName();

                        if (vData.Rows.length == 0) {
                            GotoSigninPage();
                            return;
                        };

                        //Header Info
                        $("#lblName").text(vData.Rows[0].PeopleName || '');
                        $('#tabHome').addClass('ui-btn-active');

                        //Get People Messages
                        //ShowPeopleMsgList(); 'should be done online.

                        //Get People Organization
                        ShowPeopleOrgList();

                        return true;
                    }
                    else {
                        AjaxSend({
                            url: "People/getPeopleInfoName",
                            data: pData,
                            success: function (vData) {

                                if (vData.PeopleNameData.Rows.length > 0) {

                                    //Header Info
                                    $("#lblName").text(vData.PeopleNameData.Rows[0].PeopleName || '');
                                    $('#tabHome').addClass('ui-btn-active');

                                    //Get People Messages
                                    ShowPeopleMsgList();

                                    //Get People Organization
                                    ShowPeopleOrgList();

                                }
                            }
                        });
                    };
                };

                function ShowPeopleMsgList() {
                    var intLimitMsg = 2;

                    AjaxSend({
                        url: "People/getPeopleMessageList",
                        success: function (vDataMsg) {

                            var pLV = $('#lstMessage');
                            var pItem;

                            pLV.html('');

                            if (vDataMsg.PeopleData.Rows.length > 0) {

                                for (intCntr = 0; intCntr < (vDataMsg.PeopleData.Rows.length > intLimitMsg ? intLimitMsg : vDataMsg.PeopleData.Rows.length); intCntr++) {

                                    pItem = $('<li>')
                                                    .append($('<a>')
                                                        .data('data', vDataMsg.PeopleData.Rows[intCntr])
                                                        .append($('<h3>')
                                                            .attr("style", "white-space:normal")
                                                            .text(vDataMsg.PeopleData.Rows[intCntr].Subject)
                                                        )
                                                        .append($('<p>')
                                                            .attr("style", "white-space:normal")
                                                            .text('Sender : ' + vDataMsg.PeopleData.Rows[intCntr].SenderName)
                                                        )
                                                        .append($('<p>')
                                                            .attr("style", "white-space:normal")
                                                            .text('Date/Time : ' + $.format.date(new Date(vDataMsg.PeopleData.Rows[intCntr].MessageDateTime), 'MMM dd, yyyy hh:mm a'))
                                                        )
                                                        .click(function (e) {
                                                            e.preventDefault();
                                                            ShowPeopleMsgDetails($(this).data('data'));
                                                        })
                                                    );


                                    pLV.append(pItem);

                                    pLV.trigger("create");

                                }

                                pLV.listview('refresh');
                            } else {
                                pLV.html('<div style="margin:12px">No messages</div>');
                            }
                        }
                    });
                }

                function ShowPeopleOrgList() {

                    if (OfflineMode) {
                        vData = PeopleMenu_offline().getPeopleOrganizationList();
                        DisplayPeopleMenu(vData)
                    }
                    else {
                        AjaxSend({
                            url: "People/getPeopleOrganizationList",
                            success: function (vDataOrg) {
                                DisplayPeopleMenu(vDataOrg.PeopleData);
                            }
                        });
                    }
                }

                function DisplayPeopleMenu(vData) {

                    var pParam;

                    if (vData.Rows.length > 0) {

                        var pLV = $('#divOrgApps');
                        var pItem;
                        var OrgID = [];
                        pLV.html('');
                        for (intCntr = 0; intCntr < vData.Rows.length; intCntr++) {

                            if (vData.Rows[intCntr].OrganizationID == '224a502f-8d4e-4a9b-8ab4-3c57c47c3fe7') {

                                pItem = $('<div data-role="collapsible" data-collapsed="false" data-theme="b" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">')
                                                    .append($('<h2>')
                                                        .text(vData.Rows[intCntr].CompanyName)
                                                    )
                                                    .append($('<ul data-role="listview" id="lst' + vData.Rows[intCntr].OrganizationID + '">')
                                                        .append($('<li>')

                                                            .append($('<a id="btnPOS_' + vData.Rows[intCntr].OrganizationID + '">')
                                                                .text("Point of Sales (POS)")
                                                                .data('data', vData.Rows[intCntr])
                                                                .click(function (e) {
                                                                    e.preventDefault();

                                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                                                    PromptUserAccess('Point Of Sales (POS)', {
                                                                        OnClickOKSuccess: function () {

                                                                            // MergeUrlParameters
                                                                            pParam = { mode: 'pos',
                                                                                'OrgID': sessionStorage.OrgID
                                                                            };
                                                                            if (IsMobile)
                                                                                pParam.IsMobile = 1;
                                                                            pParam = $.param(pParam);

                                                                            if (OfflineMode && Sql.IsExist(sessionStorage.OrgID) == 0) {
                                                                                window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                                                                            }
                                                                            else
                                                                                window.location = '/SalesMenu?' + pParam;
                                                                        }
                                                                    });
                                                                })
                                                                .append($('<img src="/Content/images/icons/1.png">')
                                                                    .css(mIconCss)
                                                                )
                                                            )
                                                        )
                                                        .append($('<li>')

                                                            .append($('<a id="btnSO_' + vData.Rows[intCntr].OrganizationID + '">')
                                                                .text("Sales Order Taker")
                                                                .data('data', vData.Rows[intCntr])
                                                                .click(function (e) {
                                                                    e.preventDefault();

                                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                    //                                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                                                    //                                                                    PromptUserAccess('Sales Order Taker', {
                                                                    //                                                                        OnClickOKSuccess: function () {

                                                                    // MergeUrlParameters
                                                                    pParam = { mode: 'so',
                                                                        'OrgID': sessionStorage.OrgID
                                                                    };
                                                                    if (IsMobile)
                                                                        pParam.IsMobile = 1;
                                                                    pParam = $.param(pParam);

                                                                    if (OfflineMode && Sql.IsExist(sessionStorage.OrgID) == 0) {
                                                                        window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                                                                    }
                                                                    else
                                                                        window.location = '/SalesMenu?' + pParam;
                                                                    //                                                                        }
                                                                    //                                                                    });
                                                                })
                                                                .append($('<img src="/Content/images/icons/1.png">')
                                                                    .css(mIconCss)
                                                                )
                                                            )
                                                        )
                                                        .append($('<li>')
                                                            .append($('<a id="btnSalesReturns_' + vData.Rows[intCntr].OrganizationID + '">')
                                                                .text("Sales Returns")
                                                                .data('data', vData.Rows[intCntr])
                                                                .click(function (e) {
                                                                    e.preventDefault();

                                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                                                    PromptUserAccess('Sales Return', {
                                                                        OnClickOKSuccess: function () {

                                                                            // window.open(GetPathFormat("/SalesReturns") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                                            location = MergeUrlParameters({
                                                                                OrgID: sessionStorage.OrgID,
                                                                                IsMobile: CBool(IsMobile) ? 1 : 0
                                                                            }, '/SalesReturns');
                                                                        }
                                                                    });
                                                                })
                                                                .append($('<img src="/Content/images/icons/2.png">')
                                                                    .css(mIconCss)
                                                                )
                                                            )
                                                        )
                                                        .append($('<li>')
                                                            .append($('<a id="btnReceiving_' + vData.Rows[intCntr].OrganizationID + '">')
                                                                .attr("href", GetPathFormat("/Receiving") + '?'
                                                                    + $.param({
                                                                        OrgID: vData.Rows[intCntr].OrganizationID
                                                                    }))
                                                                .text("Receiving and Transfers")
                                                                .data('data', vData.Rows[intCntr])
                                                                .click(function (e) {
                                                                    e.preventDefault();

                                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                                                    PromptUserAccess('Receiving and Transfers', {
                                                                        OnClickOKSuccess: function () {

                                                                            window.open(GetPathFormat("/Receiving") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                                        }
                                                                    });

                                                                })
                                                                .append($('<img src="/Content/images/icons/3.png">')
                                                                    .css(mIconCss)
                                                                )
                                                            )
                                                        )
                                                        .append($('<li>')
                                                            .append($('<a id="btnInventoryCount_' + vData.Rows[intCntr].OrganizationID + '">')
                                                                .attr("href", GetPathFormat("/InventoryCount") + '?'
                                                                    + $.param({
                                                                        OrgID: vData.Rows[intCntr].OrganizationID
                                                                    }))
                                                                .text("Inventory Count")
                                                                .data('data', vData.Rows[intCntr])
                                                                .click(function (e) {
                                                                    e.preventDefault();

                                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                                                    PromptUserAccess('Inventory Count', {
                                                                        OnClickOKSuccess: function () {

                                                                            window.open(GetPathFormat("/InventoryCount") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                                        }
                                                                    });

                                                                })
                                                                .append($('<img src="/Content/images/icons/4.png">')
                                                                    .css(mIconCss)
                                                                )
                                                            )
                                                        )
                                                        .append($('<li>')
                                                            .append($('<a id="btnDashboardsAndReports_' + vData.Rows[intCntr].OrganizationID + '">')
                                                                .attr("href", "#?"
                                                                    + $.param({
                                                                        OrgID: vData.Rows[intCntr].OrganizationID
                                                                    }))
                                                                .text("Dashboards and Reports")
                                                                .data('data', vData.Rows[intCntr])
                                                                .click(function (e) {
                                                                    e.preventDefault();

                                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                    $.mobile.changePage('#pagePopUpMenuDashboardsAndReports');

                                                                })
                                                                .append($('<img src="/Content/images/icons/5.png">')
                                                                    .css(mIconCss)
                                                                )
                                                            )
                                                        )
                                                        .append($('<li>')
                                                            .append($('<a id="btnTroubleshootingTools_' + vData.Rows[intCntr].OrganizationID + '">')
                                                                .text("Troubleshooting Tools")
                                                                .data('data', vData.Rows[intCntr])
                                                                .click(function (e) {
                                                                    e.preventDefault();

                                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                                    $.mobile.changePage('#pagePopUpMenuTroubleShootingTools');

                                                                })
                                                                .append($('<img src="/Content/images/icons/6.png">')
                                                                    .css(mIconCss)
                                                                )
                                                            )
                                                        )
                                                        .append($('<li>')
                                                            .append($('<a id="btnSettings_' + vData.Rows[intCntr].OrganizationID + '">')
                                                                .attr("href", "#?"
                                                                    + $.param({
                                                                        OrgID: vData.Rows[intCntr].OrganizationID
                                                                    }))
                                                                .text("System Administration")
                                                                .data('data', vData.Rows[intCntr])
                                                                .click(function (e) {
                                                                    e.preventDefault();

                                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                                    $.mobile.changePage('#pagePopUpMenuSettings');

                                                                })
                                                                .append($('<img src="/Content/images/icons/7.png">')
                                                                    .css(mIconCss)
                                                                )
                                                            )
                                                        )
                                                        .append($('<li>')
                                                            .append($('<a id="btnControlPanel_' + vData.Rows[intCntr].OrganizationID + '">')
                                                                .attr("href", "#?"
                                                                    + $.param({
                                                                        OrgID: vData.Rows[intCntr].OrganizationID
                                                                    }))
                                                                .text("Control Panel")
                                                                .data('data', vData.Rows[intCntr])
                                                                .click(function (e) {
                                                                    e.preventDefault();

                                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                                    $.mobile.changePage('#pagePopUpMenuControlPanelSettings');

                                                                })
                                                                .append($('<img src="/Content/images/icons/7.png">')
                                                                    .css(mIconCss)
                                                                )
                                                            )
                                                        )
                                //                                                    .append($('<li>')
                                //                                                        .append($('<a id="btnSFA_' + vDataOrg.PeopleData.Rows[intCntr].OrganizationID + '">')
                                //                                                            .text("SFA")
                                //                                                            .data('data', vDataOrg.PeopleData.Rows[intCntr])
                                //                                                            .click(function (e) {
                                //                                                                e.preventDefault();

                                //                                                                if ($(this).data('data') != undefined) {
                                //                                                                    pData.OrgID = $(this).data('data').OrganizationID;
                                //                                                                    AjaxSend({
                                //                                                                        url: "People/setSubscriberOrganization",
                                //                                                                        data: pData,
                                //                                                                        success: function (vData) {
                                //                                                                            if (vData.SubcriberOrg == "TRUE") {
                                //                                                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                //                                                                                PromptUserAccess('SFA', {
                                //                                                                                    OnClickOKSuccess: function () {
                                //                                                                                        window.open(GetPathFormat("/SalesMenu") + "?mode=pos", "_self");
                                //                                                                                    }
                                //                                                                                });
                                //                                                                            }
                                //                                                                        }
                                //                                                                    });
                                //                                                                }

                                //                                                            })
                                //                                                        )
                                //                                                    )
                                //                                                    .append($('<li>')
                                //                                                        .append($('<a id="btnCRM_' + vDataOrg.PeopleData.Rows[intCntr].OrganizationID + '">')
                                //                                                            .text("CRM")
                                //                                                            .data('data', vDataOrg.PeopleData.Rows[intCntr])
                                //                                                        )
                                //                                                    )

                                                    );


                                pLV.append(pItem);

                                pLV.trigger("create");

                                OrgID[intCntr] = { OrgID: vData.Rows[intCntr].OrganizationID };

                            } else {

                                pItem = $('<div data-role="collapsible" data-collapsed="false" data-theme="b" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">')
                                                .append($('<h2>')
                                                    .text(vData.Rows[intCntr].CompanyName)
                                                )
                                                .append($('<ul data-role="listview" id="lst' + vData.Rows[intCntr].OrganizationID + '">')
                                                    .append($('<li>')

                                                        .append($('<a id="btnPOS_' + vData.Rows[intCntr].OrganizationID + '">')
                                                            .text("Point of Sales (POS)")
                                                            .data('data', vData.Rows[intCntr])
                                                            .click(function (e) {
                                                                e.preventDefault();

                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                                                PromptUserAccess('Point Of Sales (POS)', {
                                                                    OnClickOKSuccess: function () {

                                                                        // MergeUrlParameters
                                                                        pParam = { mode: 'pos',
                                                                            'OrgID': sessionStorage.OrgID
                                                                        };
                                                                        if (IsMobile)
                                                                            pParam.IsMobile = 1;
                                                                        pParam = $.param(pParam);

                                                                        var IsFlgExist = 0;

                                                                        if (OfflineMode) {
                                                                            IsFlgExist = Transfers.ReadFile(sessionStorage.OrgID + '.flg')

                                                                            if (Sql.IsExist(sessionStorage.OrgID) == 0 || IsFlgExist == undefined) {
                                                                                window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                                                                            }
                                                                        }
                                                                        else
                                                                            window.location = '/SalesMenu?' + pParam;

                                                                    }
                                                                });
                                                            })
                                                            .append($('<img src="/Content/images/icons/1.png">')
                                                                .css(mIconCss)
                                                            )
                                                        )
                                                    )
                                                    .append($('<li>')

                                                        .append($('<a id="btnSO_' + vData.Rows[intCntr].OrganizationID + '">')
                                                            .text("Sales Order Taker")
                                                            .data('data', vData.Rows[intCntr])
                                                            .click(function (e) {
                                                                e.preventDefault();

                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                //                                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                                                //                                                                    PromptUserAccess('Sales Order Taker', {
                                                                //                                                                        OnClickOKSuccess: function () {

                                                                // MergeUrlParameters
                                                                pParam = { mode: 'so',
                                                                    'OrgID': sessionStorage.OrgID
                                                                };
                                                                if (IsMobile)
                                                                    pParam.IsMobile = 1;
                                                                pParam = $.param(pParam);

                                                                if (OfflineMode && Sql.IsExist(sessionStorage.OrgID) == 0) {
                                                                    window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                                                                }
                                                                else
                                                                    window.location = '/SalesMenu?' + pParam;
                                                                //                                                                        }
                                                                //                                                                    });
                                                            })
                                                            .append($('<img src="/Content/images/icons/1.png">')
                                                                .css(mIconCss)
                                                            )
                                                        )
                                                    )
                                                    .append($('<li>')
                                                        .append($('<a id="btnSalesReturns_' + vData.Rows[intCntr].OrganizationID + '">')
                                                            .text("Sales Returns")
                                                            .data('data', vData.Rows[intCntr])
                                                            .click(function (e) {
                                                                e.preventDefault();

                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                                                PromptUserAccess('Sales Return', {
                                                                    OnClickOKSuccess: function () {

                                                                        // window.open(GetPathFormat("/SalesReturns") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                                        location = MergeUrlParameters({
                                                                            OrgID: sessionStorage.OrgID,
                                                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                                                        }, '/SalesReturns');
                                                                    }
                                                                });
                                                            })
                                                            .append($('<img src="/Content/images/icons/2.png">')
                                                                .css(mIconCss)
                                                            )
                                                        )
                                                    )
                                                    .append($('<li>')
                                                        .append($('<a id="btnReceiving_' + vData.Rows[intCntr].OrganizationID + '">')
                                                            .text("Receiving and Transfers")
                                                            .data('data', vData.Rows[intCntr])
                                                            .click(function (e) {
                                                                e.preventDefault();
                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                GetUser();

                                                                function GetUser() {

                                                                    AjaxSend({
                                                                        url: 'Login3/GetUser',
                                                                        success: function (vData) {
                                                                            if (vData.Error != undefined)
                                                                                MessageBox(vData.Error);
                                                                            else {
                                                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                                                                PromptUserAccess('Receiving and Transfers', {
                                                                                    OnClickOKSuccess: function () {

                                                                                        window.open(GetPathFormat("/Receiving") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                                                    }
                                                                                });
                                                                            }
                                                                        },
                                                                        error: function (vError) {
                                                                            if (IsMobile)
                                                                                proj.PromptRequiresInternet({
                                                                                    Retry: GetUser
                                                                                });
                                                                            else {
                                                                                console.log('Error getting Login/GetUser. Retrying...');
                                                                                GetUser();
                                                                            };
                                                                        }
                                                                    });
                                                                }

                                                            })
                                                            .append($('<img src="/Content/images/icons/3.png">')
                                                                .css(mIconCss)
                                                            )
                                                        )
                                                    )
                                                    .append($('<li>')
                                                        .append($('<a id="btnInventoryCount_' + vData.Rows[intCntr].OrganizationID + '">')
                                                            .attr("href", GetPathFormat("/InventoryCount") + '?'
                                                                + $.param({
                                                                    OrgID: vData.Rows[intCntr].OrganizationID
                                                                }))
                                                            .text("Inventory Count")
                                                            .data('data', vData.Rows[intCntr])
                                                            .click(function (e) {
                                                                e.preventDefault();
                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                GetUser();

                                                                function GetUser() {

                                                                    AjaxSend({
                                                                        url: 'Login3/GetUser',
                                                                        success: function (vData) {
                                                                            if (vData.Error != undefined)
                                                                                MessageBox(vData.Error);
                                                                            else {
                                                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                                                                PromptUserAccess('Inventory Count', {
                                                                                    OnClickOKSuccess: function () {

                                                                                        window.open(GetPathFormat("/InventoryCount") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                                                    }
                                                                                });
                                                                            }
                                                                        },
                                                                        error: function (vError) {
                                                                            if (IsMobile)
                                                                                proj.PromptRequiresInternet({
                                                                                    Retry: GetUser
                                                                                });
                                                                            else {
                                                                                console.log('Error getting Login/GetUser. Retrying...');
                                                                                GetUser();
                                                                            };
                                                                        }
                                                                    });
                                                                }
                                                            })
                                                            .append($('<img src="/Content/images/icons/4.png">')
                                                                .css(mIconCss)
                                                            )
                                                        )
                                                    )
                                                    .append($('<li>')
                                                        .append($('<a id="btnDashboardsAndReports_' + vData.Rows[intCntr].OrganizationID + '">')
                                                            .attr("href", "#?"
                                                                + $.param({
                                                                    OrgID: vData.Rows[intCntr].OrganizationID
                                                                }))
                                                            .text("Dashboards and Reports")
                                                            .data('data', vData.Rows[intCntr])
                                                            .click(function (e) {
                                                                e.preventDefault();

                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                $.mobile.changePage('#pagePopUpMenuDashboardsAndReports');

                                                            })
                                                            .append($('<img src="/Content/images/icons/5.png">')
                                                                .css(mIconCss)
                                                            )
                                                        )
                                                    )
                                                    .append($('<li>')
                                                        .append($('<a id="btnTroubleshootingTools_' + vData.Rows[intCntr].OrganizationID + '">')
                                                            .text("Troubleshooting Tools")
                                                            .data('data', vData.Rows[intCntr])
                                                            .click(function (e) {
                                                                e.preventDefault();

                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                                $.mobile.changePage('#pagePopUpMenuTroubleShootingTools');

                                                            })
                                                            .append($('<img src="/Content/images/icons/6.png">')
                                                                .css(mIconCss)
                                                            )
                                                        )
                                                    )
                                                    .append($('<li>')
                                                        .append($('<a id="btnSettings_' + vData.Rows[intCntr].OrganizationID + '">')
                                                            .attr("href", "#?"
                                                                + $.param({
                                                                    OrgID: vData.Rows[intCntr].OrganizationID
                                                                }))
                                                            .text("System Administration")
                                                            .data('data', vData.Rows[intCntr])
                                                            .click(function (e) {
                                                                e.preventDefault();
                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                                GetUser();

                                                                function GetUser() {

                                                                    AjaxSend({
                                                                        url: 'Login3/GetUser',
                                                                        success: function (vData) {
                                                                            if (vData.Error != undefined)
                                                                                MessageBox(vData.Error);
                                                                            else {                                                                                
                                                                                $.mobile.changePage('#pagePopUpMenuSettings');
                                                                            }
                                                                        },
                                                                        error: function (vError) {
                                                                            if (IsMobile)
                                                                                proj.PromptRequiresInternet({
                                                                                    Retry: GetUser
                                                                                });
                                                                            else {
                                                                                console.log('Error getting Login/GetUser. Retrying...');
                                                                                GetUser();
                                                                            };
                                                                        }
                                                                    });
                                                                }
                                                            })
                                                            .append($('<img src="/Content/images/icons/7.png">')
                                                                .css(mIconCss)
                                                            )
                                                        )
                                                    )
                                //                                                    .append($('<li>')
                                //                                                        .append($('<a id="btnSFA_' + vDataOrg.PeopleData.Rows[intCntr].OrganizationID + '">')
                                //                                                            .text("SFA")
                                //                                                            .data('data', vDataOrg.PeopleData.Rows[intCntr])
                                //                                                            .click(function (e) {
                                //                                                                e.preventDefault();

                                //                                                                if ($(this).data('data') != undefined) {
                                //                                                                    pData.OrgID = $(this).data('data').OrganizationID;
                                //                                                                    AjaxSend({
                                //                                                                        url: "People/setSubscriberOrganization",
                                //                                                                        data: pData,
                                //                                                                        success: function (vData) {
                                //                                                                            if (vData.SubcriberOrg == "TRUE") {
                                //                                                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                //                                                                                PromptUserAccess('SFA', {
                                //                                                                                    OnClickOKSuccess: function () {
                                //                                                                                        window.open(GetPathFormat("/SalesMenu") + "?mode=pos", "_self");
                                //                                                                                    }
                                //                                                                                });
                                //                                                                            }
                                //                                                                        }
                                //                                                                    });
                                //                                                                }

                                //                                                            })
                                //                                                        )
                                //                                                    )
                                //                                                    .append($('<li>')
                                //                                                        .append($('<a id="btnCRM_' + vDataOrg.PeopleData.Rows[intCntr].OrganizationID + '">')
                                //                                                            .text("CRM")
                                //                                                            .data('data', vDataOrg.PeopleData.Rows[intCntr])
                                //                                                        )
                                //                                                    )

                                                );


                                pLV.append(pItem);

                                pLV.trigger("create");

                                OrgID[intCntr] = { OrgID: vData.Rows[intCntr].OrganizationID };
                            }
                        }

                        if ((OrgID.length == 0)) {
                            GetDatabaseNames(OrgID);
                        }
                    }


                }

                function GetDatabaseNames(m_data) {
                    var data = { OrgID: m_data };

                    if (!(OrgID == null)) {
                        AjaxSend({
                            url: 'People/GetDatabaseName',
                            data: data,
                            success: function (vData) {
                                var pLV = $("#tblOrganization");


                                $('#tblOrganization tr').empty();
                                $('#lstOrganization li').not(':eq(0)').not(':eq(0)').remove();

                                for (var i = 0; i <= vData.Results.Rows.length - 1; i++) {

                                    pItem = $('<tr>')
                                        .append($('<td>')
                                            .append($('<input type=checkbox />')
                                                .attr('ID', vData.Results.Rows[i].ID)
                                                .data('data', vData.Results.Rows[i])
                                            )
                                            .append($('<label>')
                                                .attr('for', vData.Results.Rows[i].ID)
                                                .text(vData.Results.Rows[i].CompanyName)
                                            )
                                        )
                                    pLV.append(pItem);
                                    $('#' + vData.Results.Rows[i].ID).checkboxradio();
                                };

                                //done button
                                pItem = $('<li>')
                                        .attr('data-icon', 'false')
                                        .attr('data-theme', 'e')
                                        .append($('<a>')
                                            .text('Done')
                                            .click(function () {
                                                if (IsMobile) {
                                                    alert('call arvin code');
                                                }
                                                else {
                                                    MessageBox("Sorry, your current device is not capable of performing this function", "People");
                                                }
                                            })
                                        )
                                //pLV.append(pItem);
                                $('#lstOrganization').append(pItem);
                                $('#lstOrganization').listview('refresh');
                            }
                        });
                    }
                }

                function ShowPeopleMsgDetails(vData) {

                    if (vData.MessageType == 1) {
                        ShowPeopleMsgT1(vData);
                    } else if (vData.MessageType == 2) {
                        ShowPeopleMsgT2(vData);
                    }

                }

                function ShowPeopleMsgT1(vMsgData) {
                    var pData = {};
                    pData.MsgID = vMsgData.ID;

                    ClearPeopleMsgT1();

                    //Get Messages Details
                    AjaxSend({
                        url: "People/getPeopleMessageT1Details",
                        data: pData,
                        success: function (vDataMsgDetails) {

                            if (vDataMsgDetails.PeopleData.Rows.length > 0) {

                                $('#txtPeopleMsgT1ID').val(vDataMsgDetails.PeopleData.Rows[0].ID);

                                $('#lblPeopleMsgT1Subject').text(vDataMsgDetails.PeopleData.Rows[0].Subject);
                                $('#lblPeopleMsgT1From').text('From : ' + vDataMsgDetails.PeopleData.Rows[0].SenderName);
                                $('#lblPeopleMsgT1DateTime').text('Date/Time : ' + $.format.date(new Date(vDataMsgDetails.PeopleData.Rows[0].MessageDateTime), 'MMM dd, yyyy hh:mm a'));

                                $('#lblPeopleMsgT1').text(vDataMsgDetails.PeopleData.Rows[0].Message);

                                $.mobile.changePage('#pagePeopleMsgType1Info');

                            }
                        }
                    });

                }

                function ClearPeopleMsgT1() {
                    $("#txtPeopleMsgT1ID").val('');

                    $('#lblPeopleMsgT1Subject').text('');
                    $('#lblPeopleMsgT1From').text('From : ');
                    $('#lblPeopleMsgT1DateTime').text('Date/Time : ');

                    $('#lblPeopleMsgT1').text('');
                }

                function ConfirmPeopleMsgT1() {
                    var pData = {};
                    pData.MsgID = $("#txtPeopleMsgT1ID").val();

                    //Get Messages Details
                    AjaxSend({
                        url: "People/ConfirmPeopleMessageT1Details",
                        data: pData,
                        success: function (vDataMsgDetails) {

                            $.mobile.changePage('#pageHome');
                            ShowPeopleMsgList();
                            ShowPeopleOrgList();

                        }
                    });

                }

                function ShowPeopleMsgT2(vMsgData) {
                    var pLV = $('#divMessageContainer');
                    var pItem;

                    //Get Messages List
                    AjaxSend({
                        url: "People/getPeopleMessageList",
                        success: function (vDataMsgList) {

                            if (vDataMsgList.PeopleData.Rows.length > 0) {

                                $.mobile.changePage('#pagePeopleMsgType2List');

                                pLV.html('');

                                for (intCntr = 0; intCntr < vDataMsgList.PeopleData.Rows.length; intCntr++) {

                                    pItem = $('<div id="divMsg_' + vDataMsgList.PeopleData.Rows[intCntr].ID + '" data-role="collapsible" data-collapsed="false" data-theme="b" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">')
                                            .append($('<h2>>')
                                                .text(vDataMsgList.PeopleData.Rows[intCntr].Subject))
                                            .append($('<ul data-role="listview" id="lst' + vDataMsgList.PeopleData.Rows[intCntr].ID + '">')
                                                .append($('<li>')
                                                    .append($('<div data-role="fieldcontain">')
                                                        .append($('<fieldset data-role="controlgroup">')
                                                            .append($('<div>')
                                                                .append($('<legend id="lblSubject_' + vDataMsgList.PeopleData.Rows[intCntr].ID + '" style="font-size: x-large;">')
                                                                    .text(vDataMsgList.PeopleData.Rows[intCntr].Subject)
                                                                )
                                                            )
                                                            .append($('<br />'))
                                                            .append($('<div>')
                                                                .append($('<legend id="lblPeopleMsgT2From_' + vDataMsgList.PeopleData.Rows[intCntr].ID + '" style="font-weight:normal;">')
                                                                    .text('From : ' + vDataMsgList.PeopleData.Rows[intCntr].SenderName)
                                                                )
                                                            )
                                                            .append($('<div>')
                                                                .append($('<legend id="lblPeopleMsgT2DateTime_' + vDataMsgList.PeopleData.Rows[intCntr].ID + '" style="font-weight:normal;">')
                                                                    .text('Date/Time : ' + $.format.date(new Date(vDataMsgList.PeopleData.Rows[intCntr].MessageDateTime), 'MMM dd, yyyy hh:mm a'))
                                                                )
                                                            )
                                                        )
                                                    )

                                                    .append($('<div data-role="fieldcontain">')
                                                        .append($('<fieldset data-role="controlgroup">')
                                                            .append($('<div>')
                                                                .append($('<legend id="lblPeopleMsgCatT2_' + vDataMsgList.PeopleData.Rows[intCntr].ID + '" style="font-weight:normal;">')
                                                                    .text((vDataMsgList.PeopleData.Rows[intCntr].Category == "" ? '' : 'Category : ' + vDataMsgList.PeopleData.Rows[intCntr].Category))
                                                                )
                                                            )
                                                            .append($('<br />'))
                                                            .append($('<div>')
                                                                .append($('<legend id="lblPeopleMsgT2_' + vDataMsgList.PeopleData.Rows[intCntr].ID + '" style="font-weight:normal;">')
                                                                    .text(vDataMsgList.PeopleData.Rows[intCntr].Message)
                                                                )
                                                            )
                                                            .append($('<br />'))
                                                        )
                                                    )
                                                )
                                            );

                                    pLV.append(pItem);

                                    pLV.trigger("create");
                                }

                                $('html, body').animate({
                                    scrollTop: $('#divMsg_' + vMsgData.ID).offset().top + 'px'
                                }, 'fast');
                            }
                        }
                    });
                }

                //                function ShowPeopleMsgT2(vMsgData) {
                //                    var pData = {};
                //                    pData.MsgID = vMsgData.ID;

                //                    ClearPeopleMsgT2();

                //                    //Get Messages Details
                //                    AjaxSend({
                //                        url: "People/getPeopleMessageT2Details",
                //                        data: pData,
                //                        success: function (vDataMsgDetails) {

                //                            if (vDataMsgDetails.PeopleData.Rows.length > 0) {

                //                                $('#txtPeopleMsgT2ID').val(vDataMsgDetails.PeopleData.Rows[0].ID);

                //                                $('#lblPeopleMsgT2Subject').text(vDataMsgDetails.PeopleData.Rows[0].Subject);
                //                                $('#lblPeopleMsgT2From').text('From : ' + vDataMsgDetails.PeopleData.Rows[0].SenderName);
                //                                $('#lblPeopleMsgT2DateTime').text('Date/Time : ' + $.format.date(new Date(vDataMsgDetails.PeopleData.Rows[0].MessageDateTime), 'MMM dd, yyyy hh:mm a'));

                //                                $('#lblPeopleMsgT2').text(vDataMsgDetails.PeopleData.Rows[0].Message);

                //                                $.mobile.changePage('#pagePeopleMsgType2Info');

                //                            }
                //                        }
                //                    });
                //                }

                function ClearPeopleMsgT2() {
                    $("#txtPeopleMsgT2ID").val('');

                    $('#lblPeopleMsgT2Subject').text('');
                    $('#lblPeopleMsgT2From').text('From : ');
                    $('#lblPeopleMsgT2DateTime').text('Date/Time : ');

                    $('#lblPeopleMsgT2').text('');
                }

                function ShowNotifications() {
                    //Load Notifications
                }

                function ClearPersonFields() {

                    //Header Info
                    $("#lblName").text('');
                    $("#lblEmployer").text('');

                }

                function ShowOrganizationProfile() {
                    var pData = {};
                    pData.OrgID = sessionStorage["OrgID"];

                    window.pageOrig = $.mobile.activePage;

                    AjaxSend({
                        url: 'Organization/getSubscriberOrganization',
                        data: pData,
                        success: function (vDataOrg) {

                            if (vDataOrg.OrganizationID != "") {
                                pData.IsNew = 0;
                                pData.ID = vDataOrg.OrganizationID;

                                AjaxSend({
                                    url: 'Organization/setCreateNew',
                                    data: pData,
                                    success: function (vData) {
                                        ChangePage('/Organization/OrganizationSummary');
                                    }
                                });
                            }
                        }
                    });

                }

                function ShowOrganizationPlaces() {
                    var pData = {};
                    pData.OrgID = sessionStorage["OrgID"];

                    window.pageSourcePage = $.mobile.activePage;

                    AjaxSend({
                        url: 'Organization/getSubscriberOrganization',
                        data: pData,
                        success: function (vDataOrg) {

                            if (vDataOrg.OrganizationID != "") {
                                pData.IsNew = 0;
                                pData.ID = vDataOrg.OrganizationID;

                                AjaxSend({
                                    url: 'Organization/setCreateNew',
                                    data: pData,
                                    success: function (vData) {

                                        ChangePage('/Organization/OrganizationAddress');
                                    }
                                });
                            }
                        }
                    });

                }

                function ShowOrganizationEmployees() {
                    var pData = {};
                    pData.OrgID = sessionStorage["OrgID"];

                    AjaxSend({
                        url: 'Organization/getSubscriberOrganization',
                        data: pData,
                        success: function (vDataOrg) {

                            if (vDataOrg.OrganizationID != "") {
                                pData.IsNew = 0;
                                pData.ID = vDataOrg.OrganizationID;

                                AjaxSend({
                                    url: 'Organization/setCreateNew',
                                    data: pData,
                                    success: function (vData) {
                                        sessionStorage.SourcePage = 'HomePage';
                                        sessionStorage.ScheduleID = '';
                                        window.location = MergeUrlParameters({
                                            OrgID: sessionStorage.OrgID,
                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                        }, '/Organization/OrganizationEmployee');
                                    }
                                });
                            }
                        }
                    });

                }

                function ShowOrganizationCustomers() {
                    var pData = {};
                    pData.OrgID = sessionStorage["OrgID"];

                    window.pageSourcePage = $.mobile.activePage;

                    AjaxSend({
                        url: 'Organization/getSubscriberOrganization',
                        data: pData,
                        success: function (vDataOrg) {

                            if (vDataOrg.OrganizationID != "") {
                                pData.IsNew = 0;
                                pData.ID = vDataOrg.OrganizationID;

                                AjaxSend({
                                    url: 'Organization/setCreateNew',
                                    data: pData,
                                    success: function (vData) {

                                        //                                        window.open(GetPathFormat("/Organization/OrganizationCustomer") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                        ChangePage('/Organization/OrganizationCustomer');
                                    }
                                });
                            }
                        }
                    });
                }

                function ShowOrganizationSuppliers() {
                    var pData = {};
                    pData.OrgID = sessionStorage["OrgID"];

                    window.pageSourcePage = $.mobile.activePage;

                    AjaxSend({
                        url: 'Organization/getSubscriberOrganization',
                        data: pData,
                        success: function (vDataOrg) {

                            if (vDataOrg.OrganizationID != "") {
                                pData.IsNew = 0;
                                pData.ID = vDataOrg.OrganizationID;

                                AjaxSend({
                                    url: 'Organization/setCreateNew',
                                    data: pData,
                                    success: function (vData) {

                                        ChangePage('/Organization/OrganizationSupplier');
                                    }
                                });
                            }
                        }
                    });

                }

                function ShowOrganizationEmployeeWorkingHours() {
                    var pData = {};
                    pData.OrgID = sessionStorage["OrgID"];

                    window.pageSourcePage = $.mobile.activePage;

                    window.location = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        IsMobile: CBool(IsMobile) ? 1 : 0
                    }, '../Organization/OrganizationEmployeeWorkingHours');

                    //window.location = '/Organization/OrganizationEmployeeWorkingHours'

                    //                    AjaxSend({
                    //                        url: 'Organization/getSubscriberOrganization',
                    //                        data: pData,
                    //                        success: function (vDataOrg) {

                    //                            if (vDataOrg.OrganizationID != "") {
                    //                                pData.IsNew = 0;
                    //                                pData.ID = vDataOrg.OrganizationID;

                    //                                AjaxSend({
                    //                                    url: 'Organization/setCreateNew',
                    //                                    data: pData,
                    //                                    success: function (vData) {

                    //                                        ChangePage('/Organization/OrganizationEmployeeWorkingHours');
                    //                                    }
                    //                                });
                    //                            }
                    //                        }
                    //                    });
                }
                function ShowOrganizationProduct() {
                    var pData = {};
                    pData.OrgID = sessionStorage["OrgID"];

                    window.pageSourcePage = $.mobile.activePage;

                    AjaxSend({
                        url: 'Organization/getSubscriberOrganization',
                        data: pData,
                        success: function (vDataOrg) {

                            if (vDataOrg.OrganizationID != "") {
                                pData.IsNew = 0;
                                pData.ID = vDataOrg.OrganizationID;

                                AjaxSend({
                                    url: 'Organization/setCreateNew',
                                    data: pData,
                                    success: function (vData) {

                                        ////////                                    window.open(GetPathFormat("/Organization/OrganizationProduct") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                        ChangePage('/Organization/OrganizationProduct');
                                    }
                                });
                            }
                        }
                    });

                }

                function ShowOrganizationFOWSTProduct() {
                    var pData = {};
                    pData.OrgID = sessionStorage["OrgID"];

                    window.pageSourcePage = $.mobile.activePage;

                    AjaxSend({
                        url: 'Organization/getSubscriberOrganization',
                        data: pData,
                        success: function (vDataOrg) {

                            if (vDataOrg.OrganizationID != "") {
                                pData.IsNew = 0;
                                pData.ID = vDataOrg.OrganizationID;

                                AjaxSend({
                                    url: 'Organization/setCreateNew',
                                    data: pData,
                                    success: function (vData) {

                                        ////////                                    window.open(GetPathFormat("/Organization/OrganizationFOWSTProduct") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                        ChangePage('/Organization/OrganizationFOWSTProduct');
                                    }
                                });
                            }
                        }
                    });

                }

                function ClearFields() {
                    $("#selProduct").val('General Update');
                    $('#selProduct').selectmenu('refresh');
                    $("#selCategory").val('');
                    $('#selCategory').selectmenu('refresh');
                    $("#txtTitle").val('');
                    $("#txtMessage").val('');
                }

                function SaveUpdateMessageInfo() {
                    var pData = {};

                    pData = {
                        Product: $.trim($("#selProduct").val()),
                        Category: $.trim($("#selCategory").val()),
                        Title: $.trim($("#txtTitle").val()),
                        Message: $.trim($("#txtMessage").val())

                    };

                    AjaxSend({
                        url: 'Dashboard/SaveUpdateInfo',
                        data: pData,
                        success: function (vData) {
                            if (vData.Error == undefined && vData.UpdateData != "Error") {

                                ClearFields();
                                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
                            }
                            else
                                alert('Something went wrong. Please contact Mobiz Support.');
                        } // success
                    }); // AjaxSend


                }
                $('#popCompanyShippers').click(function (e) {
                    e.preventDefault();

                    //                    RequestImportData('9D4B1500-13C0-49F9-A8E5-A16C13EC899E');

                });

                $('#popCompanyPrinters').click(function (e) {
                    e.preventDefault();

                    //                    window.pageSecuritySourcePage = $.mobile.activePage;
                    //                    PromptUserAccess('Company Supplier', {
                    //                        OnClickOKSuccess: function () {
                    //window.open(GetPathFormat("/Device") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    location = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID
                    }, '/DevicePrinter');
                    //                        }
                    //                    });

                });



            });
        }); // require PeopleMenu_offline.js
    });