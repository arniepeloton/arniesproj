require([
    'common'
    ], function () {

        $(function () {

            //            var tallest = 0;
            //            $('.ui-grid-a > div').each(function () {
            //                var thisHeight = $(this).height();
            //                if (thisHeight > tallest) {
            //                    tallest = thisHeight;
            //                }
            //            });
            //            $('.ui-grid-a > div > div').height(tallest);


            $('#btnSetupOrganizationVal').height($('#btnSetupOrganization').height());
            $('#btnSetupOrganizationFOWSTVal').height($('#btnSetupOrganizationFOWST').height());
            $('#btnSetupDeviceVal').height($('#btnSetupDevice').height());
            $('#btnSetupReceiptSettingsVal').height($('#btnSetupReceiptSettings').height());
            $('#btnSetupSecurityVal').height($('#btnSetupSecurity').height());
            $('#btnSetupEmployeeVal').height($('#btnSetupEmployee').height());
            $('#btnSetupProductsVal').height($('#btnSetupProducts').height());
            $('#btnSetupCustomerVal').height($('#btnSetupCustomer').height());
            $('#btnSetupSupplierVal').height($('#btnSetupSupplier').height());

            $(document).ready(function () {

                ShowCheckList();

            });

            $('#btnSetupOrganization').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess('Company Profile', {
                            OnClickOKSuccess: function () {
                                ShowOrganizationProfile();
                            }
                        });
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

            });

            $('#btnSetupOrganizationVal').click(function (e) {
                e.preventDefault();

                $('#btnSetupOrganization').click();

            });

            $('#btnSetupOrganizationFOWST').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess('Company Places (FOWST)', {
                            OnClickOKSuccess: function () {
                                ShowOrganizationPlaces();
                            }
                        });
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

            });

            $('#btnSetupOrganizationFOWSTVal').click(function (e) {
                e.preventDefault();

                $('#btnSetupOrganizationFOWST').click();

            });

            $('#btnSetupDevice').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
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

            });

            $('#btnSetupDeviceVal').click(function (e) {
                e.preventDefault();

                $('#btnSetupDevice').click();

            });

            $('#btnSetupReceiptSettings').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess('Devices', {
                            OnClickOKSuccess: function () {
                                //window.open(GetPathFormat("/Device") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                location = MergeUrlParameters({
                                    OrgID: sessionStorage.OrgID,
                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                }, '/ReceiptSettings');
                            }
                        });
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

            });

            $('#btnSetupReceiptSettingsVal').click(function (e) {
                e.preventDefault();

                $('#btnSetupReceiptSettings').click();

            });

            $('#btnSetupSecurity').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess('Security Roles', {
                            OnClickOKSuccess: function () {

                                location = MergeUrlParameters({
                                    OrgID: sessionStorage.OrgID,
                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                }, '/SecurityRole');

                                //window.open(GetPathFormat("/SecurityRole") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                            }
                        });
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


            });

            $('#btnSetupSecurityVal').click(function (e) {
                e.preventDefault();

                $('#btnSetupSecurity').click();

            });

            $('#btnSetupEmployee').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess('Company Employee', {
                            OnClickOKSuccess: function () {
                                ShowOrganizationEmployees();
                            }
                        });
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

            });

            $('#btnSetupEmployeeVal').click(function (e) {
                e.preventDefault();

                $('#btnSetupEmployee').click();

            });

            $('#btnSetupProducts').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.open(GetPathFormat("/PIMUpload") + "?OrgID=" + sessionStorage["OrgID"], "_self");
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

            });

            $('#btnSetupProductsVal').click(function (e) {
                e.preventDefault();

                $('#btnSetupProducts').click();

            });

            $('#btnSetupCustomer').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess('Company Customer', {
                            OnClickOKSuccess: function () {
                                ShowOrganizationCustomers();
                            }
                        });
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


            });

            $('#btnSetupCustomerVal').click(function (e) {
                e.preventDefault();

                $('#btnSetupCustomer').click();

            });

            $('#btnSetupSupplier').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess('Company Supplier', {
                            OnClickOKSuccess: function () {
                                ShowOrganizationSuppliers();
                            }
                        });
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

            });

            $('#btnSetupSupplierVal').click(function (e) {
                e.preventDefault();

                $('#btnSetupSupplier').click();

            });

            $('#btnSetupChecklistDone').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu") + (IsMobile ? "?IsMobile=1" : ""), "_self");

            });

            function ShowCheckList() {
                var pData = {};
                pData.OrgID = sessionStorage["OrgID"];

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'SetupChecklist/getModuleList',
                    data: pData,
                    success: function (vData) {

                        if (vData.PIMChecklistData.Rows.length > 0) {

                            AjaxSend({
                                url: 'SetupChecklist/getModuleRequiredDataList',
                                data: pData,
                                success: function (vDataCheck) {

                                    if (vData.PIMChecklistData.Rows.length > 0) {
                                        for (var p = 0; p <= vData.PIMChecklistData.Rows.length - 1; p++) {

                                            if (vData.PIMChecklistData.Rows[p].ModuleType == "Organization") {

                                                $("#imgOrg").attr('src', '/Content/images/tick.png');
                                                for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                    if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                        $("#imgOrg").attr('src', '/Content/images/cross.png');
                                                    }
                                                }
                                                $("#lblOrgLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                            } else if (vData.PIMChecklistData.Rows[p].ModuleType == "FOWST") {
                                                $("#imgFOWST").attr('src', '/Content/images/tick.png');
                                                for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                    if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                        $("#imgFOWST").attr('src', '/Content/images/cross.png');
                                                    }
                                                }
                                                $("#lblFOWSTCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                                $("#lblFOWSTLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                            } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Authorized Device") {
                                                $("#imgDevice").attr('src', '/Content/images/tick.png');
                                                for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                    if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                        $("#imgDevice").attr('src', '/Content/images/cross.png');
                                                    }
                                                }
                                                $("#lblDeviceCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                                $("#lblDeviceLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                            } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Receipt Settings") {
                                                $("#imgReceiptSettings").attr('src', '/Content/images/tick.png');
                                                for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                    if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                        $("#imgReceiptSettings").attr('src', '/Content/images/cross.png');
                                                    }
                                                }
                                                $("#lblReceiptSettingsCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                                $("#lblReceiptSettingsLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                            } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Security Access") {
                                                $("#imgSecurityAccess").attr('src', '/Content/images/tick.png');
                                                for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                    if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                        $("#imgSecurityAccess").attr('src', '/Content/images/cross.png');
                                                    }
                                                }
                                                $("#lblSecurityAccessCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                                $("#lblSecurityAccessLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                            } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Employee") {
                                                $("#imgEmployeeAccess").attr('src', '/Content/images/tick.png');
                                                for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                    if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                        $("#imgEmployeeAccess").attr('src', '/Content/images/cross.png');
                                                    }
                                                }
                                                $("#lblEmployeeCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                                $("#lblEmployeeLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                            } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Products") {
                                                $("#imgProduct").attr('src', '/Content/images/tick.png');
                                                for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                    if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                        $("#imgProduct").attr('src', '/Content/images/cross.png');
                                                    }
                                                }
                                                $("#lblProductCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                                $("#lblProductLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                            } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Customers/Clients") {
                                                $("#imgCustomer").attr('src', '/Content/images/tick.png');
                                                for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                    if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                        $("#imgCustomer").attr('src', '/Content/images/cross.png');
                                                    }
                                                }
                                                $("#lblCustomerCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                                $("#lblCustomerLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                            } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Supplier/Vendor") {
                                                $("#imgSupplier").attr('src', '/Content/images/tick.png');
                                                for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                    if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                        $("#imgSupplier").attr('src', '/Content/images/cross.png');
                                                    }
                                                }
                                                $("#lblSupplierCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                                $("#lblSupplierLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                            }


                                        };
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
                                    // ChangePage('/Organization/OrganizationSummary');
                                    window.location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/Organization/OrganizationSummary');
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

                                    //                                        ChangePage('/Organization/OrganizationAddress'); //error na ni karon. kana sa baba na ang gamiton dpat
                                    window.location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/Organization/OrganizationAddress');
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
                                    //ChangePage('/Organization/OrganizationCustomer');
                                    window.location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/Organization/OrganizationCustomer');
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

                                    //ChangePage('/Organization/OrganizationSupplier');
                                    window.location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/Organization/OrganizationSupplier');
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
                                    //ChangePage('/Organization/OrganizationProduct');
                                    window.location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/Organization/OrganizationProduct');
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

            }



        });
    });