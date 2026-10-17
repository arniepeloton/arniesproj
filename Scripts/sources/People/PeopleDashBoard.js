require([
    'common'
    ], function () {
        var SortedBy = 1;
        var ProductFeatureID = '';

        $(function () {
    
            $('#liSales').click(function () {

                GetUser();
                function GetUser() {
                    //                    if (!IsMobile || window.InWindows) {
                    AjaxSend({
                        url: 'Login3/GetUser',
                        success: function (vData) {
                            if (vData.Error != undefined)
                                MessageBox(vData.Error);
                            else {
                                location = MergeUrlParameters({
                                    OrgID: sessionStorage.OrgID,
                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                }, '/DashBoardsAndReports');
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

            $('#liOfftake').click(function () {

                GetUser();
                function GetUser() {
                    AjaxSend({
                        url: 'Login3/GetUser',
                        success: function (vData) {
                            if (vData.Error != undefined)
                                MessageBox(vData.Error);
                            else {
                                location = MergeUrlParameters({
                                    OrgID: sessionStorage.OrgID,
                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                }, '/DashBoardsAndReports/OffTake');
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

            $('#liOrders').click(function () {
                AjaxSend({
                    url: 'Login3/GetUser',
                    success: function (vData) {
                        if (vData.Error != undefined)
                            MessageBox(vData.Error);
                        else {
                            location = MergeUrlParameters({
                                mode: "soServed",
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/DashBoardsAndReports/SalesOrder');
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
            });


            $('#liFranchiseCentral').click(function () {
                AjaxSend({
                    url: 'Login3/GetUser',
                    success: function (vData) {
                        CheckIsGroupHead();


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

            $('#liInventory').click(function () {
                GetUser();

                function GetUser() {

                    AjaxSend({
                        url: 'Login3/GetUser',
                        success: function (vData) {
                            if (vData.Error != undefined)
                                MessageBox(vData.Error);
                            else
                                window.open(GetPathFormat("/InventoryCount/Report"), "_self");
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

            $('#liTrackDevice').click(function () {
                GetUser();

                function GetUser() {

                    AjaxSend({
                        url: 'Login3/GetUser',
                        success: function (vData) {
                            if (vData.Error != undefined)
                                MessageBox(vData.Error);
                            else
                                window.open(GetPathFormat("/DashboardsAndReports/Device"), "_self");
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

            $('#liPOSTransactionDetail').click(function () {
                GetUser();

                function GetUser() {

                    AjaxSend({
                        url: 'Login3/GetUser',
                        success: function (vData) {
                            if (vData.Error != undefined)
                                MessageBox(vData.Error);
                            else
                                window.open(GetPathFormat("/DashBoardsAndReports/POSTransactionDetail"), "_self");
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

            $('#liInventoryLedger').click(function () {
                GetUser();

                function GetUser() {

                    AjaxSend({
                        url: 'Login3/GetUser',
                        success: function (vData) {
                            if (vData.Error != undefined)
                                MessageBox(vData.Error);
                            else
                                window.open(GetPathFormat("/InventoryLedger"), "_self");
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

            //tusoy oct 7, 2013 3:20pm
            $('#liMDMDashboard').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Devices', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("/DeviceDashboard") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    }
                });


            });

            $('#popPOSReportCashierSalesReport').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {

                        if (IsMobile) {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Cashier Sales Report', {
                                OnClickOKSuccess: function () {

                                    var pData = {};
                                    pData.OrgID = sessionStorage["OrgID"];
                                    pData.POSSalesReportType = 0;

                                    AjaxSend({
                                        url: 'ReportsPOS/setPOSSalesReportType',
                                        data: pData,
                                        success: function (vData) {
                                            window.open(GetPathFormat("/ReportsPOS/SalesReport") + "?OrgID=" + sessionStorage["OrgID"] + "&IsManagerSalesReport=0&IsMobile=1", "_self");
                                        }
                                    });
                                }
                            });

                        } else {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Cashier Sales Report', {
                                OnClickOKSuccess: function () {

                                    var pData = {};
                                    pData.OrgID = sessionStorage["OrgID"];
                                    pData.POSSalesReportType = 0;

                                    AjaxSend({
                                        url: 'ReportsPOS/setPOSSalesReportType',
                                        data: pData,
                                        success: function (vData) {
                                            window.open(GetPathFormat("/ReportsPOS/SalesReport") + "?OrgID=" + sessionStorage["OrgID"] + "&IsManagerSalesReport=0", "_self");
                                        }
                                    });
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


            });

            $('#popPOSReportManagerReport').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {

                        if (IsMobile) {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Manager Sales Report', {
                                OnClickOKSuccess: function () {

                                    var pData = {};
                                    pData.OrgID = sessionStorage["OrgID"];
                                    pData.POSSalesReportType = 1;

                                    AjaxSend({
                                        url: 'ReportsPOS/setPOSSalesReportType',
                                        data: pData,
                                        success: function (vData) {
                                            window.open(GetPathFormat("/ReportsPOS/SalesReport") + "?OrgID=" + sessionStorage["OrgID"] + "&IsManagerSalesReport=1&IsMobile=1", "_self");
                                        }
                                    });
                                }
                            });

                        } else {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Manager Sales Report', {
                                OnClickOKSuccess: function () {

                                    var pData = {};
                                    pData.OrgID = sessionStorage["OrgID"];
                                    pData.POSSalesReportType = 1;

                                    AjaxSend({
                                        url: 'ReportsPOS/setPOSSalesReportType',
                                        data: pData,
                                        success: function (vData) {
                                            window.open(GetPathFormat("/ReportsPOS/SalesReport") + "?OrgID=" + sessionStorage["OrgID"] + "&IsManagerSalesReport=1", "_self");
                                        }
                                    });
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


            });

            $('#popPOSReportSalesAdjustment').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess('Sales Adjustment Report', {
                            OnClickOKSuccess: function () {
                                window.open(GetPathFormat("/ReportsPOS/SalesAdjustmentReport") + "?OrgID=" + sessionStorage["OrgID"], "_self");
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

            $('#popPOSReportZReport').click(function (e) {
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
                                PromptUserAccess('Z Report', {
                                    OnClickOKSuccess: function () {
                                        window.open(GetPathFormat("/ReportsPOS/ZReport") + "?OrgID=" + sessionStorage["OrgID"], "_self");
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

            $('#popPOSReportMallLessorReport').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pagePopUpMenuPOSReportsMallLessorReport');

            });

            $('#popPOSReportMallLessorAyala').click(function (e) {
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
                                PromptUserAccess('Mall Lessor Report', {
                                    OnClickOKSuccess: function () {

                                        window.open(GetPathFormat("/MallLessorAyala") + "?OrgID=" + sessionStorage["OrgID"], "_self");
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

            $('#popPOSReportMallLessorSM').click(function (e) {
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
                                PromptUserAccess('Mall Lessor Report', {
                                    OnClickOKSuccess: function () {

                                        window.open(GetPathFormat("/MallLessorSM") + "?OrgID=" + sessionStorage["OrgID"], "_self");
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

            $('#popPOSReportPrincipal').click(function (e) {
                GetUser();

                function GetUser() {

                    AjaxSend({
                        url: 'Login3/GetUser',
                        success: function (vData) {
                            if (vData.Error != undefined)
                                MessageBox(vData.Error);
                            else
                                window.open(GetPathFormat("/ReportToPrincipal") + "?OrgID=" + sessionStorage["OrgID"], "_self");
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

            $('#popPOSReportShopperTrak').click(function (e) {
                GetUser();

                function GetUser() {

                    AjaxSend({
                        url: 'Login3/GetUser',
                        success: function (vData) {
                            if (vData.Error != undefined)
                                MessageBox(vData.Error);
                            else
                                window.open(GetPathFormat("/ShopperTrak") + "?OrgID=" + sessionStorage["OrgID"], "_self");
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

            $('#liCRM').click(function () {
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID
                }, '/DashBoardsAndReports/crm');
            });

            $('#liDashboardSettings').click(function (e) {
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID
                }, '/DashBoardsAndReports/Settings');
            });
        }); //inner function

        function CheckIsGroupHead() {
            AjaxSend({
                url: 'Franchisor/CheckIsGroupHead',
                success: function (vData) {
                     if (CStr2(vData.Franchisee) == 'True') {
                        location = MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            IsMobile: CBool(IsMobile) ? 1 : 0
                        }, '/DashBoardsAndReports/FranchiseRequest');

                    } else {

                         if (vData.Franchisor == 'True') {
                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/DashBoardsAndReports/Franchise');
                        } else {
                            MessageBox('You must be a Group Head in order to view this module.', 'Franchise Central');
                        
                        }


                        
                    }
                   
                },
                error: function (vError) {
                    MessageBox(vError);
                }
            });

        }


    });
