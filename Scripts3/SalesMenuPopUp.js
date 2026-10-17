require(['common'
    ], function () {

        $(function () {

            $('#btnMobizPopUpMenu').click(function (e) {
                e.preventDefault();

                $("#divMobizPopUpMenu").popup("open");

            });

            $('#btnMobizGoToHome').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            });

            $('#popMobizHome').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            });

            $('#popMobizPOS').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageMobizPopUpMenuPOS');

            });

            $('#popMobizSFA').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('SFA', {
                    OnClickOKSuccess: function () {
                        window.location = GetPathFormat("/SalesMenu?OrgID=" + sessionStorage.OrgID);
                    }
                });

            });

            $('a[href="/SalesReturns"]').each(function (vIndex, vItem) {

                $(vItem).attr('href', GetPathFormat($(vItem).attr('href')));

            });

            $('#popMobizPOSPOS').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Point Of Sales (POS)', {
                    OnClickOKSuccess: function () {
                        window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);
                    }
                });

            });

            $('#popMobizPOSSalesReturn').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Sales Return', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("/SalesReturns"), "_self");
                    }
                });

            });

            $('#btnMobizPOSBack').click(function (e) {
                e.preventDefault();

                window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);

            });
            
            $('#popMobizReports').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageMobizPopUpMenuReports');

            });

            $('#btnMobizReportsBack').click(function (e) {
                e.preventDefault();

                window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);

            });

            $('#popMobizReportsPOS').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageMobizPopUpMenuPOSReports');

            });

            $('#popMobizReportsInventoryLedger').click(function (e) {
                e.preventDefault();

                ChangePage('/InventoryLedger');

            });

            $('#btnMobizPOSReportBack').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageMobizPopUpMenuReports');

            });

            $('#popMobizPOSReportSalesReport').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Cashier Sales Report', {
                    OnClickOKSuccess: function () {
                        var pData = {};
                        pData.POSSalesReportType = 1;

                        AjaxSend({
                            url: 'ReportsPOS/setPOSSalesReportType',
                            data: pData,
                            success: function (vData) {

                                window.open(GetPathFormat("ReportsPOS/SalesReport"), "_self");
                            }
                        });
                    }
                });

            });

            $('#popMobizPOSReportManagerReport').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Manager Sales Report', {
                    OnClickOKSuccess: function () {
                        var pData = {};
                        pData.POSSalesReportType = 0;

                        AjaxSend({
                            url: 'ReportsPOS/setPOSSalesReportType',
                            data: pData,
                            success: function (vData) {

                                window.open(GetPathFormat("ReportsPOS/SalesReport"), "_self");
                            }
                        });
                    }
                });

            });

            $('#popMobizPOSReportSalesAdjustment').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Sales Adjustment Report', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("ReportsPOS/SalesAdjustmentReport"), "_self");
                    }
                });

            });

            $('#popMobizPOSReportMallLessorReport').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageMobizPopUpMenuPOSReportsMallLessorReport');

            });

            $('#popMobizPOSReportMallLessorAyala').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('SFA', {
                    OnClickOKSuccess: function () {
                        
                        window.open(GetPathFormat("/MallLessorAyala"), "_self");
                    }
                });

            });

            $('#popMobizPOSReportMallLessorSM').click(function (e) {
                e.preventDefault();

                alert("under construction!");
                //////                window.pageSecuritySourcePage = $.mobile.activePage;
                //////                PromptUserAccess('Sales Adjustment Report', {
                //////                    OnClickOKSuccess: function () {
                //////                        window.open(GetPathFormat("ReportsPOS/SalesAdjustmentReport"), "_self");
                //////                    }
                //////                });

            });

            $('#btnMobizPOSReportMallLessorBack').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageMobizPopUpMenuPOSReports');

            });

            $('#popMobizSettings').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageMobizPopUpMenuSettings');

            });

            $('#popMobizSecuritySettings').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageMobizPopUpMenuSecuritySettings');

            });

            $('#popMobizSecuritySettingsDevices').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Devices', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("device"), "_self");
                    }
                });


            });

            $('#popMobizCompanyProfile').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Company Profile', {
                    OnClickOKSuccess: function () {
                        ShowOrganizationProfile();
                    }
                });

            });

            $('#popMobizCompanyPlaces').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Company Places (FOWST)', {
                    OnClickOKSuccess: function () {
                        ShowOrganizationPlaces();
                    }
                });

            });

            $('#popMobizCompanyEmployees').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Company Employee', {
                    OnClickOKSuccess: function () {
                        ShowOrganizationEmployees();
                    }
                });

            });

            $('#popMobizCompanyCustomers').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Company Customer', {
                    OnClickOKSuccess: function () {
                        ShowOrganizationCustomers();
                    }
                });

            });

            $('#popMobizCompanySuppliers').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Company Supplier', {
                    OnClickOKSuccess: function () {
                        ShowOrganizationSuppliers();
                    }
                });

            });

            $('#popMobizReceiptSettings').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('POS Receipt Configuration', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("ReceiptSettings"), "_self");
                    }
                });

            });

            $('#popMobizSalesReturnsSettings').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Sales Return Settings', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("/SalesReturns/Settings"), "_self");
                    }
                });

            });

            $('#btnMobizSettingsBack').click(function (e) {
                e.preventDefault();

                window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);

            });

            $('#btnMobizSecuritySettingsBack').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageMobizPopUpMenuSettings');

            });

            $('#popMobizCRM').click(function (e) {
                e.preventDefault();

                $.mobile.changePage(GetPathFormat('crm'));

            });

            $('#popMobizReportsCRM').click(function (e) {
                e.preventDefault();

            });

            $('#popMobizSecuritySettingsRoles').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Security Roles', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("SecurityRole"), "_self");
                    }
                });
            });

            function ShowOrganizationProfile() {
                var pData = {};

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
                                    //ChangePage('/Organization/OrganizationSummary');
                                    window.location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/Organization/OrganizationSummary');
                                }
                            });
                        }
                    }
                });

            }

            function ShowOrganizationPlaces() {
                var pData = {};

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

                                    //                                    ChangePage('/Organization/OrganizationAddress');
                                    window.location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/Organization/OrganizationAddress');
                                }
                            });
                        }
                    }
                });

            }

            function ShowOrganizationEmployees() {
                var pData = {};

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

                                   // ChangePage('/Organization/OrganizationEmployee');
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

                                    // ChangePage('/Organization/OrganizationCustomer');
                                    window.location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/Organization/OrganizationCustomer');
                                }
                            });
                        }
                    }
                });
            }

            function ShowOrganizationSuppliers() {
                var pData = {};

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

                                    //ChangePage('/Organization/OrganizationSupplier');
                                    window.location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/Organization/OrganizationSupplier');
                                }
                            });
                        }
                    }
                });

            }


        });
    });
