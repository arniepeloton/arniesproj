require([
    'common', 'business'
    ], function () {

        require([(OfflineMode ? '/Scripts/sources/People/PeopleMenu_offline.js?crc=' : ''),
                 (OfflineMode ? '/Scripts/sources/ImportExportData/ImportExportData.js?crc=' : ''),
                 (OfflineMode ? '/Scripts/sources/ImportExportData/ImportExportData_offline.js?crc=' : ''),
                 (OfflineMode ? '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc=' : ''),
                 (OfflineMode ? '/Scripts3/SalesMenu_offline.js?crc=' : ''),
                 (OfflineMode ? '/Scripts3/model/PeopleMenuUserDetails_Offline.js?crc=' : '')
        ], function () {

            $(function () {

                var mInterval;
                var flushedCounter = 0;

                var mIconCss = {
                    padding: '10px',
                    width: '60px',
                    height: '60px'
                };
                var intCntr;
                var isIE = false || !!document.documentMode;

                var UserDetails = {};

                if (OfflineMode) {
                    UserDetails = new App.model.PeopleMenuUserDetails_offline();
                }

                sessionStorage.removeItem('FowstID'); // pending disconnected checkout 4/15/2014 4:51 PM

                sessionStorage.removeItem('CustomerOrganizationID'); // added by janine may 14 1pm . reset customer id

                //ADDITIONAL BY JANINE Sept. 9, 2013 1pm
                CheckIfBackIsClicked();

                //Check hash change
                window.onhashchange = function () {
                    CheckIfBackIsClicked();
                };

                if (window.location.hash.match(/\bFlushDeviceData\b/i))
                    FlushDeviceData();

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
                        try {
                            ShowPersonNameInfo();
                        } catch (e) {
                            proj.PromptLogin(e.message);
                        }
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

                    $("#divPeopleMenuPopUp").popup("open");

                });

                $('#popPeopleMenuPeopleInfoSummary').click(function (e) {
                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            //                            window.location = GetPathFormat("PeopleInfoSummary");
                            if (OfflineMode)
                                window.location = '/People/PeopleInfoSummary?IsMobile=1';
                            else
                                window.location = '/People/PeopleInfoSummary';
                            //                            window.open(GetPathFormat("PeopleInfoSummary"), "_self");
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

                    proj.PromptTestInternet(
                        function () {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('POS Receipt Reprint', {
                                OnClickOKSuccess: function () {
                                    if (OfflineMode)
                                        window.open(GetPathFormat("/SalesPayments/DuplicateReceipt") + "?OrgID=" + sessionStorage["OrgID"] + "&IsMobile=1", "_self");
                                    else
                                        window.open(GetPathFormat("/SalesPayments/DuplicateReceipt") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                }
                            });
                        }
                    );

                });

                $('#popTroubleshootingToolsUpdateDatabase').click(function (e) {
                    e.preventDefault();

                    proj.PromptTestInternet(
                        function () {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            IsSyncDatabase();
                        }
                    );

                });

                $('#popTroubleshootingToolsLazyUpdate').click(function (e) {
                    //e.preventDefault();

                    $('#panLazySync').popup('open');

                    //proj.PromptTestInternet(
                    //    function() {
                    //        //$.mobile.changePage('#pagePopUpLazyUpdate');
                    //        $('#panLazySync').popup('open');
                    //    }
                    //);

                });

                $('#popTroubleshootingToolsRestoreDatabase').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopUpRestoreDatabase');


                    //                    var pLV = $("#tblDatabase");

                    //                    var tempDB = Sql.IsExist('Mobiz.backup');

                    //                    if (tempDB) {
                    //                        pItem = $('<tr>')
                    //                                .append($('<td>')
                    //                                    .append($('<input type=checkbox />')
                    //                                        .attr('ID', 'Mobiz.backup.db')
                    //                                        .attr('checked', checked)
                    //                                    )
                    //                                    .append($('<label>')
                    //                                        .attr('for', 'Mobiz.backup.db')
                    //                                        .text('Mobiz.backup.db')
                    //                                    )
                    //                                ) //td          
                    //                        pLV.append(pItem);
                    //                    $('#Mobiz.backup.db').checkboxradio();
                    //                    }
                    //                    



                    //                    for (i = 1; i <= 15; i++) {
                    //                        var checked = undefined;

                    //                        var tempDB = Sql.IsExist(sessionStorage.OrgID + '-' + i + '.backup')

                    //                        if (tempDB) {
                    //                            pItem = $('<tr>')
                    //                                .append($('<td>')
                    //                                    .append($('<input type=checkbox />')
                    //                                        .attr('ID', sessionStorage.OrgID + '-' + i + '.backup.db')
                    //                                        .attr('checked', checked)
                    //                                    )
                    //                                    .append($('<label>')
                    //                                        .attr('for', sessionStorage.OrgID + '-' + i + '.backup.db')
                    //                                        .text(sessionStorage.OrgID + '-' + i + '.backup.db')
                    //                                    )
                    //                                ) //td          
                    //                            pLV.append(pItem);

                    //                            $('#' + sessionStorage.OrgID + '-' + i + '.backup.db').checkboxradio();
                    //                        }
                    //                    }

                });

                $('#btnSubmitProblems').click(function () {

                    SalesPaymentsExport_offline().HandleUnflushedRecords({
                        Success: function () {
                            RestoreDatabase(true);
                        },
                        Error: function () {
                            RestoreDatabase(false);
                        }
                    });


                    function RestoreDatabase(IsFlushSuccessful) {

                        if (!IsFlushSuccessful) {
                            Sql.CloseConnection();
                            Transfers.MoveFile('/data/' + sessionStorage.OrgID + '.db', '/data/' + sessionStorage.OrgID + '.ToBeFlush.db');
                        }

                        var hasRestored = false;
                        if ($('#chkPrivate1').prop("checked") == true ||
                        $('#chkPrivate2').prop("checked") == true ||
                        $('#chkPrivate3').prop("checked") == true) {
                            RestorePublicDatabase();
                            hasRestored = true;
                        }

                        if ($('#chkPublic1').prop("checked") == true) {
                            RestorePrivateDatabase();
                            hasRestored = true;
                        }

                        if (hasRestored) {
                            $.mobile.changePage('#pagePopUpRestoreDatabaseSuccessful');
                        } else {
                            MessageBox("Please select an issue that you've encountered.");
                        }
                    }

                });

                $('#lnkBackToSyncData').click(function () {
                    $.mobile.changePage('#pagePopUpMenuTroubleShootingTools');
                });

                $('#lnkBackToSignInPage').click(function () {
                    window.location = '/SignIn?IsMobile=1';
                });

                $('#btnRestorePrivateDatabase').click(function () {

                });

                $('#btnRestorePublicDatabase').click(function () {

                });

                $('#popSendLastUpdatedToMobiz').click(function () {
                    var pSql = '';

                    pSql = '';

                    $('#panLazySync').popup('open');
                });

                $('#btnLazyUpdateSyncNow').click(function () {
                    $('#panLazySync').popup('close');

                    try {
                        $('#popMessage').html("Checking for new updates. Please Wait.");
                        $("#dvpopup").popup("open");
                    } catch (e) {

                    };

                    setTimeout(function () { RequestLazyUpdate() }, 500);
                });

                $('#popTroubleshootingToolsFlushData').click(function (e) {
                    e.preventDefault();

                    proj.PromptTestInternet(
                        function () {
                            if (OfflineMode) {
                                flushedCounter = 0;
                                mInterval = setInterval(FlushDeviceData, 1000);
                            }
                        }
                    );
                });

                function FlushDeviceData() {

                    clearInterval(mInterval);
                    SetPopUpMessage(3, "Flushing Data to Server");

                    //                    if (flushedCounter < 3) {
                    var pDt;
                    var pDt2;
                    var pDt3;
                    var pDt4;
                    var pDt5;
                    var pSql = '';
                    var pSql2 = '';
                    var pSql3 = '';
                    var pSql4 = '';
                    var pSql5 = '';
                    var pSql6 = '';

                    Android.UpdateSessionData({ OrgID: sessionStorage.OrgID })

                    Sql.AttachDatabase(sessionStorage.OrgID);
                    pSql = 'Select * from TransInvoice where DateImported IS NULL AND InvoiceNumber>0';
                    pDt = Sql.Execute(pSql);

                    //additional by janine. check if there are unflushed customer. april 21 2014
                    pSql2 = 'Select * from LibraryOrganization where IsFromDevice = 1 AND FlushToServer = 1';
                    pDt2 = Sql.Execute(pSql2);
                    //end of additional

                    //additional by janine. check if there are unflushed customer. aug 11 2014
                    pSql3 = 'Select * from LibraryOrganizationFOWST where IsFromDevice = 1 AND FlushToServer = 1';
                    pDt3 = Sql.Execute(pSql3);
                    //end of additional

                    pSql4 = 'SELECT * FROM LibraryFOWSTBinProduct WHERE private.LibraryFOWSTBinProduct.DateImported IS NULL and MaxPerProduct>0'
                    pDt4 = Sql.Execute(pSql4);

                    pSql5 = 'SELECT * FROM TransInventoryProductCount WHERE private.TransInventoryProductCount.DateImported IS NULL'
                    pDt5 = Sql.Execute(pSql5);

                    pSql6 = 'SELECT * FROM private.LibraryFOWSTBin WHERE DateImported IS NULL AND Name <> \'Unnamed Bin\''
                    pDt6 = Sql.Execute(pSql6);

                    if (pDt.Rows.length > 0 || pDt2.Rows.length > 0 || pDt3.Rows.length > 0 || pDt4.Rows.length > 0 || pDt5.Rows.length > 0 || pDt6.Rows.length > 0) { //add to checking
                        flushedCounter++;
                        //flushed dire na side
                        window.SalesPaymentsExport_offline().Export({
                            //////////                            OnUploaded: function () {
                            //////////                                FollowUp();
                            //////////                            }
                            OnDone: function (vData) {
                                if (vData.Result == 'ok') {
                                    FlushDeviceData(sessionStorage.OrgID);
                                }
                            }
                        }); // END window.SalesPaymentsExport_offline().Export   

                        //////////                        function FollowUp() {
                        //////////                            var pResult; // boolean

                        //////////                            pResult = window.SalesPaymentsExport_offline().FollowUp({
                        //////////                                OnDone: function (vData) {
                        //////////                                    if (vData.Result == 'ok') {
                        //////////                                        FlushDeviceData(sessionStorage.OrgID);
                        //////////                                    }
                        //////////                                },
                        //////////                                OnProgress: function (vData) {
                        //////////                                    console.log(vData.Message); // vData.Message = string
                        //////////                                }
                        //////////                            });
                        //////////                            if (pResult) {
                        //////////                                //if pResult = false, error ang flushing?
                        //////////                                // expect OnDone (and optionally OnProgress) to be called
                        //////////                            } else {
                        //////////                                // OnDone and OnProgress will not be called
                        //////////                            }
                        //////////                        } // END FollowUp()
                    }
                    else {
                        Android.AddLog('No data to flush. Ref:jsUMUGCRW6EJ!');
                        MessageBox('Data has been flushed to server successfully.');
                    }
                    //                    }
                    //                    else {
                    //                        MessageBox('Failed flushing data to server. Please try again later.');
                    //                    }
                };

                $('#popTroubleshootingToolsReconstructSalesData').click(function (e) {
                    e.preventDefault();

                    proj.PromptTestInternet(
                        function () {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Reconstruct Sales Data', {
                                OnClickOKSuccess: function () {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage["OrgID"],
                                        mode: 'pos',
                                        rsd: 1,
                                        IsMobile: IsMobile ? 1 : 0
                                    }, '/SalesMenu');
                                }
                            });
                        }
                    );

                });

                $('#popCashHandlingDepositMonies').click(function (e) {
                    e.preventDefault();

                    if (OfflineMode)
                        pOfflineModeFunction = function () {
                            //Android.Toast('Cannot Connect to Server');
                            sessionStorage.PrintingOfflineData = true;
                            GotoPage();
                        }
                    proj.PromptTestInternet({
                        context: this,
                        method: function () {
                            sessionStorage.PrintingOfflineData = false;
                            if (OfflineMode)
                                SalesPaymentsExport_offline().HandleUnflushedRecords({
                                    Success: function () {
                                        GotoPage();
                                    }
                                });
                            else
                                GotoPage();
                        },
                        error: pOfflineModeFunction
                    }); // END proj.PromptTestInternet(

                    function GotoPage() {
                        location = MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            IsMobile: CBool(IsMobile) ? 1 : 0
                        }, '/CashHandling/MoniesForDepositOrTurnOver');
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
                //                                pData.POSSalesReportType = 0;

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

                //                    AjaxSend({
                //                        url: 'Login3/TestCookies',
                //                        success: function (vData) {

                //                            if (IsMobile) {
                //                                window.open(GetPathFormat("/ReportsPOS/SalesReport") + "?OrgID=" + sessionStorage["OrgID"] + "&IsManagerSalesReport=1&IsMobile=1", "_self");
                //                            } else {
                //                                window.pageSecuritySourcePage = $.mobile.activePage;
                //                                PromptUserAccess('Manager Sales Report', {
                //                                    OnClickOKSuccess: function () {
                //                                        var pData = {};
                //                                        pData.POSSalesReportType = 1;

                //                                        AjaxSend({
                //                                            url: 'ReportsPOS/setPOSSalesReportType',
                //                                            data: pData,
                //                                            success: function (vData) {
                //                                                window.open(GetPathFormat("/ReportsPOS/SalesReport") + "?OrgID=" + sessionStorage["OrgID"] + "&IsManagerSalesReport=1", "_self");
                //                                            }
                //                                        });
                //                                    }
                //                                });
                //                            }

                //                        },
                //                        error: function (vError) {
                //                            MessageBox("This feature requires internet connection.",
                //                                   "Requires Internet Connection",
                //                                    [
                //                                        { Text: 'Ok'
                //                                        }
                //                                    ]
                //                            );
                //                            return;
                //                        }

                //                    });

                //                });

                //                $('#popPOSReportSalesAdjustment').click(function (e) {
                //                    e.preventDefault();

                //                    AjaxSend({
                //                        url: 'Login3/TestCookies',
                //                        success: function (vData) {
                //                            window.pageSecuritySourcePage = $.mobile.activePage;
                //                            PromptUserAccess('Sales Adjustment Report', {
                //                                OnClickOKSuccess: function () {
                //                                    window.open(GetPathFormat("/ReportsPOS/SalesAdjustmentReport") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //                                }
                //                            });
                //                        },
                //                        error: function (vError) {
                //                            MessageBox("This feature requires internet connection.",
                //                                   "Requires Internet Connection",
                //                                    [
                //                                        { Text: 'Ok'
                //                                        }
                //                                    ]
                //                            );
                //                            return;
                //                        }

                //                    });

                //                });

                //                $('#popPOSReportZReport').click(function (e) {
                //                    e.preventDefault();

                //                    AjaxSend({
                //                        url: 'Login3/TestCookies',
                //                        success: function (vData) {
                //                            window.pageSecuritySourcePage = $.mobile.activePage;
                //                            PromptUserAccess('Z Report', {
                //                                OnClickOKSuccess: function () {
                //                                    window.open(GetPathFormat("/ReportsPOS/ZReport") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //                                }
                //                            });
                //                        },
                //                        error: function (vError) {
                //                            MessageBox("This feature requires internet connection.",
                //                                   "Requires Internet Connection",
                //                                    [
                //                                        { Text: 'Ok'
                //                                        }
                //                                    ]
                //                            );
                //                            return;
                //                        }

                //                    });

                //                });

                //                $('#popPOSReportMallLessorReport').click(function (e) {
                //                    e.preventDefault();

                //                    $.mobile.changePage('#pagePopUpMenuPOSReportsMallLessorReport');

                //                });

                //                $('#popPOSReportMallLessorAyala').click(function (e) {
                //                    e.preventDefault();

                //                    AjaxSend({
                //                        url: 'Login3/TestCookies',
                //                        success: function (vData) {
                //                            window.pageSecuritySourcePage = $.mobile.activePage;
                //                            PromptUserAccess('Mall Lessor Report', {
                //                                OnClickOKSuccess: function () {

                //                                    window.open(GetPathFormat("/MallLessorAyala") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //                                }
                //                            });
                //                        },
                //                        error: function (vError) {
                //                            MessageBox("This feature requires internet connection.",
                //                                   "Requires Internet Connection",
                //                                    [
                //                                        { Text: 'Ok'
                //                                        }
                //                                    ]
                //                            );
                //                            return;
                //                        }

                //                    });

                //                });

                //                $('#popPOSReportMallLessorSM').click(function (e) {
                //                    e.preventDefault();

                //                    AjaxSend({
                //                        url: 'Login3/TestCookies',
                //                        success: function (vData) {
                //                            window.pageSecuritySourcePage = $.mobile.activePage;
                //                            PromptUserAccess('Mall Lessor Report', {
                //                                OnClickOKSuccess: function () {

                //                                    window.open(GetPathFormat("/MallLessorSM") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //                                }
                //                            });
                //                        },
                //                        error: function (vError) {
                //                            MessageBox("This feature requires internet connection.",
                //                                   "Requires Internet Connection",
                //                                    [
                //                                        { Text: 'Ok'
                //                                        }
                //                                    ]
                //                            );
                //                            return;
                //                        }

                //                    });

                //                });

                $('#popPOSReportPrincipal').click(function (e) {
                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            window.open(GetPathFormat("/ReportToPrincipal") + "?OrgID=" + sessionStorage["OrgID"], "_self");
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

                $('#popPOSReportShopperTrak').click(function (e) {
                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            window.open(GetPathFormat("/ShopperTrak") + "?OrgID=" + sessionStorage["OrgID"], "_self");
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

                $('#btnCashHandlingBack').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pageHome');

                });
                ///END Trouble Shooting Tools --------------------------------------------------------------------------------------------------------------------------------------------

                ///System Administration --------------------------------------------------------------------------------------------------------------------------------------------
                $('#popSecuritySettings').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopUpMenuSecuritySettings');

                });

                $('#popSetupChecklist').click(function (e) {
                    e.preventDefault();
                    location = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        IsMobile: CBool(IsMobile) ? 1 : 0
                    }, '/SetupChecklist');

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

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Security Roles', {
                                OnClickOKSuccess: function () {

                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage["OrgID"],
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

                $('#popSecuritySettingsDevices').click(function (e) {
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

                $('#popCompanyProfile').click(function (e) {
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

                $('#popCompanyPlaces').click(function (e) {
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

                $('#popCompanyEmployees').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopUpEmployeeSettings');
                });

                $('#popEmployeeList').click(function () {
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

                $('#popEmployeeWorkSchedule').click(function (e) {
                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            window.pageSecuritySourcePage = $.mobile.activePage;

                            ShowOrganizationEmployeeWorkingHours();
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

                $('#popCompanyProducts').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopUpMenuProducts');

                });

                $('#popPIMProductCompanyWide').click(function (e) {
                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Company-Wide Product List', {
                                OnClickOKSuccess: function () {

                                    ShowOrganizationProduct();
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

                $('#popPIMFOWSTProduct').click(function (e) {
                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Fowst Level Product List', {
                                OnClickOKSuccess: function () {

                                    ShowOrganizationFOWSTProduct();
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

                //                $('#popPIMProductPrices').click(function (e) {
                //                    e.preventDefault();

                //                    AjaxSend({
                //                        url: 'Login3/TestCookies',
                //                        success: function (vData) {
                //                            window.pageSecuritySourcePage = $.mobile.activePage;
                //                            PromptUserAccess('Product Price', {
                //                                OnClickOKSuccess: function () {
                //                                    location = MergeUrlParameters({
                //                                        OrgID: sessionStorage.OrgID,
                //                                        IsMobile: CBool(IsMobile) ? 1 : 0
                //                                    }, '/PIMProductPrice');
                //                                    //                                    window.open(GetPathFormat("/PIMProductPrice") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //                                }
                //                            });
                //                        },
                //                        error: function (vError) {
                //                            MessageBox("This feature requires internet connection.",
                //                                   "Requires Internet Connection",
                //                                    [
                //                                        { Text: 'Ok'
                //                                        }
                //                                    ]
                //                            );
                //                            return;
                //                        }

                //                    });

                //                });

                /// additional by janine june 25
                $('#popPIMProductPrices').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pagePopUpMenuAdjustPrice');


                });

                $('#btnAdjustPriceBack').click(function (e) {
                    e.preventDefault();

                    //$('#btnSettingsBack').click();
                    window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuProducts"), "_self");

                });
                $('#popPriceChangeSingle').click(function (e) {
                    e.preventDefault();
                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Product Price', {
                                OnClickOKSuccess: function () {

                                    window.open(GetPathFormat("/PIMProductPrice") + "?OrgID=" + sessionStorage["OrgID"], "_self");
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

                $('#popPriceChangeBulk').click(function (e) {
                    e.preventDefault();
                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Product Price', {
                                OnClickOKSuccess: function () {

                                    window.open(GetPathFormat("/PIMProductPrice/UploadDownloadExcelFile") + "?OrgID=" + sessionStorage["OrgID"], "_self");
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

                ///// end of additional by janine june 25

                $('#popPIMUploadDownloadProduct').click(function (e) {
                    e.preventDefault();

                    //                    $.mobile.changePage('#pagePopUpMenuProductsUploadDownload');
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

                $('#popPIMUpload').click(function (e) {
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

                $('#btnPIMUploadDownloadBack').click(function (e) {
                    e.preventDefault();

                    $('#btnSettingsBack').click();

                });

                $('#popSystemBehavior').click(function (e) {

                    e.preventDefault();

                    //                    window.pageSecuritySourcePage = $.mobile.activePage;
                    //                    PromptUserAccess('System Behavior', {
                    //                        OnClickOKSuccess: function () {
                    //                            location = MergeUrlParameters({
                    //                                OrgID: sessionStorage.OrgID,
                    //                                IsMobile: CBool(IsMobile) ? 1 : 0
                    //                            }, '#pagePopSystemBehavior');
                    //                        }
                    //                    });
                    $.mobile.changePage('#pagePopSystemBehavior');
                });


                $('#popDecimalInQuantity').click(function (e) {

                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {

                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/Decimals');

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

                $('#popPOSSettings').click(function (e) {

                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {

                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/POSSettings');

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

                $('#popSalesOrderSettings').click(function (e) {
                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {

                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/SystemBehaviorSettings/SalesOrder');

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

                $('#popPIMGroups').click(function (e) {
                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {

                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Hot Buttons Settings', {
                                OnClickOKSuccess: function () {
                                    //added by arnie Jan 23 4:47
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/ProductGroup');
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

                $('#popTableView').click(function (e) {
                    e.preventDefault();

                    location = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        IsMobile: CBool(IsMobile) ? 1 : 0
                    }, '/TableView');

                    //                    AjaxSend({
                    //                        url: 'Login3/TestCookies',
                    //                        success: function (vData) {

                    //                            window.pageSecuritySourcePage = $.mobile.activePage;
                    //                            PromptUserAccess('Table View Settings', {
                    //                                OnClickOKSuccess: function () {
                    //                                    location = MergeUrlParameters({
                    //                                        OrgID: sessionStorage.OrgID,
                    //                                        IsMobile: CBool(IsMobile) ? 1 : 0
                    //                                    }, '/TableView');
                    //                                }
                    //                            });


                    //                        },
                    //                        error: function (vError) {
                    //                            MessageBox("This feature requires internet connection.",
                    //                                   "Requires Internet Connection",
                    //                                    [
                    //                                        { Text: 'Ok'
                    //                                        }
                    //                                    ]
                    //                            );
                    //                            return;
                    //                        }

                    //                    });

                });

                $('#popPrinterRoles').click(function (e) {
                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {

                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Printer Roles', {
                                OnClickOKSuccess: function () {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/DevicePrinter');
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

                $('#popPrinterSetup').click(function (e) {
                    e.preventDefault();

                    location = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        IsMobile: CBool(IsMobile) ? 1 : 0
                    }, '/DevicePrinter/PrinterSetup');

                    //                    AjaxSend({
                    //                        url: 'Login3/TestCookies',
                    //                        success: function (vData) {

                    //                            window.pageSecuritySourcePage = $.mobile.activePage;
                    //                            PromptUserAccess('Printer Roles', {
                    //                                OnClickOKSuccess: function () {
                    //                                    location = MergeUrlParameters({
                    //                                        OrgID: sessionStorage.OrgID,
                    //                                        IsMobile: CBool(IsMobile) ? 1 : 0
                    //                                    }, '/DevicePrinter');
                    //                                }
                    //                            });


                    //                        },
                    //                        error: function (vError) {
                    //                            MessageBox("This feature requires internet connection.",
                    //                                   "Requires Internet Connection",
                    //                                    [
                    //                                        { Text: 'Ok'
                    //                                        }
                    //                                    ]
                    //                            );
                    //                            return;
                    //                        }

                    //                    });

                });

                $('#popPIMDownloadProductTemplate').click(function (e) {
                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            //                window.pageSecuritySourcePage = $.mobile.activePage;
                            //                PromptUserAccess('Product Price', {
                            //                    OnClickOKSuccess: function () {


                            //                    }
                            //                });
                            window.open(GetPathFormat("/PIMTemplate/PIMGenerateTemplate") + "?OrgID=" + sessionStorage["OrgID"], "_self");

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

                $('#popPIMImportProductTemplate').click(function (e) {
                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            //                window.pageSecuritySourcePage = $.mobile.activePage;
                            //                PromptUserAccess('Product Price', {
                            //                    OnClickOKSuccess: function () {

                            window.open(GetPathFormat("/PIMTemplate") + "?OrgID=" + sessionStorage["OrgID"], "_self");

                            //                    }
                            //                });
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

                $('#popCompanyCustomers').click(function (e) {
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

                $('#popCompanySuppliers').click(function (e) {
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

                $('#popSysAdminMallLessor').click(function (e) {
                    e.preventDefault();

                    $('#popPOSReportMallLessorReport').click();

                });

                $('#popReceiptSettings').click(function (e) {
                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('POS Receipt Configuration', {
                                OnClickOKSuccess: function () {
                                    window.open(GetPathFormat("/ReceiptSettings") + "?OrgID=" + sessionStorage["OrgID"], "_self");
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

                $('#popSalesReturnsSettings').click(function (e) {
                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Sales Return Settings', {
                                OnClickOKSuccess: function () {
                                    window.open(GetPathFormat("/SalesReturns/Settings") + "?OrgID=" + sessionStorage["OrgID"], "_self");
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

                $('#btnDataLibrariesBack').click(function (e) {
                    e.preventDefault();

                    $('#btnSettingsBack').click();
                });

                //ADDITIONAL by Janine 
                $('#popDiscountSettings').click(function (e) {
                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Discount Settings', {
                                OnClickOKSuccess: function () {
                                    window.open(GetPathFormat("/DiscountSettings") + "?OrgID=" + sessionStorage["OrgID"], "_self");

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

                $('#btnSystemAdministrationBack').click(function (e) {
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

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            window.open(GetPathFormat("/OrganizationShort"), "_self");
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

                function RestorePublicDatabase() {
                    Sql.CloseConnection();
                    Transfers.MoveFile('/data/Mobiz.backup', '/data/Mobiz.db');
                    //                    MessageBox('Database has been restored. Please restart mobiz app from the menu settings.');
                }

                function RestorePrivateDatabase() {
                    var flgCounter = 1;

                    Sql.CloseConnection();

                    Transfers.MoveFile('/data/' + sessionStorage.OrgID + '.db', '/data/' + sessionStorage.OrgID + '-ToBeSync.db');

                    while (flgCounter < 17) {
                        IsFlagExist = Transfers.ReadFile(sessionStorage.OrgID + '-' + flgCounter + '.flg');
                        if (IsFlagExist == undefined) {
                            try {
                                if (flgCounter == 1) {
                                    if (Sql.IsExist(sessionStorage.OrgID + '-16.backup')) {
                                        Transfers.MoveFile('/data/' + sessionStorage.OrgID + '-16.backup.db', '/data/' + sessionStorage.OrgID + '.db');
                                        Transfers.DeleteFile(sessionStorage.OrgID + '-16.flg');
                                        //                                        MessageBox('Database has been restored. Please sign in to use Mobiz Complete Retail.', null, null, {
                                        //                                            OnClose: function () {
                                        //                                                window.location = '/SignIn?IsMobile=1';
                                        //                                            } // OnClose
                                        //                                        }); // MessageBox
                                    }
                                }
                                else {
                                    if (Sql.IsExist(sessionStorage.OrgID + '-' + (flgCounter - 1) + '.backup')) {
                                        Transfers.MoveFile('/data/' + sessionStorage.OrgID + '-' + (flgCounter - 1) + '.backup.db', '/data/' + sessionStorage.OrgID + '.db');
                                        Transfers.DeleteFile(sessionStorage.OrgID + '-' + (flgCounter - 1) + '.flg');
                                        //                                        MessageBox('Database has been restored. Please sign in to use Mobiz Complete Retail.', null, null, {
                                        //                                            OnClose: function () {
                                        //                                                window.location = '/SignIn?IsMobile=1';
                                        //                                            } // OnClose
                                        //                                        }); // MessageBox

                                    }
                                }

                            }
                            catch (e) {

                            }

                        }
                        flgCounter++;
                    }
                }

                function GotoSigninPage() {

                    var pParam = {};
                    if (IsMobile)
                        pParam.IsMobile = 1;
                    pParam = $.param(pParam);
                    window.location = '/SignIn?' + pParam;

                }; // GotoSigninPage

                function SignOut() {

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {

                            AjaxSend({
                                url: "People/setSignOut",
                                success: function (vData) {
                                    window.open(GetPathFormat("/SignIn?mode=logout"), "_self"); // window.open(GetPathFormat("/People"), "_self"); EDITED BY: JANINE , Aug 3 4pm
                                }
                            });

                        },
                        error: function (vError) {
                            GotoSigninPage();
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
                        sessionStorage["CurrentUserFullName"] = (vData.Rows[0].PeopleName || '');
                        $('#tabHome').addClass('ui-btn-active');

                        //Edited By: Francis H
                        //Date: 06/18/14

                        // modified by Arvin mar 23, 2015
                        ShowPeopleMsgList(); // can be done offline

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
                                    sessionStorage["CurrentUserFullName"] = (vData.PeopleNameData.Rows[0].PeopleName || '');
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
                    var pData;

                    if (window.IsMobile)
                        pData = {
                            PeopleUserID: Web.PeopleUserID()
                        };
                    AjaxSend({
                        url: "People/getPeopleMessageList",
                        NoSplash: true,
                        data: pData,
                        success: function (vDataMsg) {
                            if (OfflineMode)
                                PeopleMenu_offline().SaveOfflineMessages(vDataMsg);
                            ShowMessage(vDataMsg);
                        },
                        error: function () {
                            try {
                                var pData = PeopleMenu_offline().LoadOfflineMessages();
                                ShowMessage(pData);
                            } catch (e) {
                                // just ignore any errors here
                                console.trace(e.stack);
                            }
                        }
                    }); // END AjaxSend

                    function ShowMessage(vDataMsg) {

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

                    } // END ShowMessage
                } // END ShowPeopleMsgList

                function ShowPeopleOrgList() {

                    $('#divOrgApps').html('');

                    if (OfflineMode) {
                        vData = PeopleMenu_offline().getPeopleOrganizationList();
                        DisplayPeopleMenu(vData, true); //Load First Org with DBase
                        DisplayPeopleMenu(vData, false);
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

                function DisplayPeopleMenu(vData, IsLoadWithDBase) {

                    var pParam;
                    var vLoadType;

                    if (vData.Rows.length > 0) {

                        var pLV = $('#divOrgApps');
                        var pItem;
                        var pUl;
                        var OrgID = [];
                        //                        pLV.html('');
                        var pPackage;

                        if (window.IsMobile) {
                            if (Android.PackageName() == 'mobiz.mobizFC')
                                pPackage = 'fc';
                            else
                                pPackage = 'mobiz';
                        } else
                            pPackage = 'mobiz';


                        for (intCntr = 0; intCntr < vData.Rows.length; intCntr++) {

                            if (OfflineMode) {
                                vLoadType = IsHasDBase(vData.Rows[intCntr].OrganizationID);
                            } else {
                                vLoadType = undefined
                            }

                            if (IsLoadWithDBase == vLoadType) {

                                if (vData.Rows[intCntr].OrganizationID == '224a502f-8d4e-4a9b-8ab4-3c57c47c3fe7') {

                                    pItem = $('<div data-role="collapsible" data-collapsed="' + (vLoadType || vLoadType == undefined ? "false" : "true") + '" data-theme="b" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">')
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
                                                        //additional by janine apr 5 2014
                                                        sessionStorage.removeItem('IsHotButtonVisited');

                                                        ///end of additional

                                                        window.pageSecuritySourcePage = $.mobile.activePage;
                                                        PromptUserAccess('Point Of Sales (POS)', {
                                                            OnClickOKSuccess: function () {

                                                                // MergeUrlParameters
                                                                pParam = {
                                                                    mode: 'pos',
                                                                    'OrgID': sessionStorage.OrgID
                                                                };
                                                                if (IsMobile)
                                                                    pParam.IsMobile = 1;
                                                                pParam = $.param(pParam);

                                                                if (OfflineMode && Sql.IsExist(sessionStorage.OrgID) == 0) {
                                                                    window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                                                                } else
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

                                                        window.pageSecuritySourcePage = $.mobile.activePage;
                                                        PromptUserAccess('Sales Order Taker', {
                                                            OnClickOKSuccess: function () {

                                                                // MergeUrlParameters
                                                                pParam = {
                                                                    mode: 'so',
                                                                    'OrgID': sessionStorage.OrgID
                                                                };
                                                                if (IsMobile)
                                                                    pParam.IsMobile = 1;
                                                                pParam = $.param(pParam);

                                                                if (OfflineMode && Sql.IsExist(sessionStorage.OrgID) == 0) {
                                                                    window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                                                                } else
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
                                                .append($('<a id="btnSalesReturns_' + vData.Rows[intCntr].OrganizationID + '">')
                                                    .text("Sales Returns")
                                                    .data('data', vData.Rows[intCntr])
                                                    .click(function (e) {
                                                        e.preventDefault();

                                                        sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                        proj.PromptTestInternet(
                                                            function () {
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
                                                            }
                                                        ); // END proj.PromptTestInternet
                                                        //AjaxSend({
                                                        //    url: 'Login3/TestCookies',
                                                        //    success: function (vData) {


                                                        //    },
                                                        //    error: function (vError) {
                                                        //        MessageBox("This feature requires internet connection.",
                                                        //               "Requires Internet Connection",
                                                        //                [
                                                        //                    { Text: 'Ok'
                                                        //                    }
                                                        //                ]
                                                        //        );
                                                        //        return;
                                                        //    }

                                                        //});
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

                                                        // 9/9/2014 1:42 PM
                                                        // Arvin
                                                        // moved online checking within page
                                                        window.pageSecuritySourcePage = $.mobile.activePage;
                                                        PromptUserAccess('Receiving and Transfers', {
                                                            OnClickOKSuccess: function () {
                                                                location = MergeUrlParameters({
                                                                    OrgID: sessionStorage.OrgID,
                                                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                                                }, '/Receiving');

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
                                                    .attr("href",
                                                        MergeUrlParameters({
                                                            OrgID: vData.Rows[intCntr].OrganizationID,
                                                            IsMobile: IsMobile ? 1 : 0
                                                        }, '/InventoryCount')
                                                    )
                                                    .text("Inventory Count")
                                                    .data('data', vData.Rows[intCntr])
                                                    .click(function (e) {
                                                        e.preventDefault();

                                                        sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                        //                                                                    AjaxSend({
                                                        //                                                                        url: 'Login3/TestCookies',
                                                        //                                                                        success: function (vData) {

                                                        window.pageSecuritySourcePage = $.mobile.activePage;
                                                        PromptUserAccess('Inventory Count', {
                                                            OnClickOKSuccess: function () {

                                                                location = MergeUrlParameters({
                                                                    OrgID: sessionStorage.OrgID,
                                                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                                                }, '/InventoryCount');

                                                                //window.open(GetPathFormat("/InventoryCount") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                            }
                                                        });

                                                        //                                                                        },
                                                        //                                                                        error: function (vError) {
                                                        //                                                                            MessageBox("This feature requires internet connection.",
                                                        //                                                                                   "Requires Internet Connection",
                                                        //                                                                                    [
                                                        //                                                                                        { Text: 'Ok'
                                                        //                                                                                        }
                                                        //                                                                                    ]
                                                        //                                                                            );
                                                        //                                                                            return;
                                                        //                                                                        }

                                                        //                                                                    });

                                                    })
                                                    .append($('<img src="/Content/images/icons/4.png">')
                                                        .css(mIconCss)
                                                    )
                                                )
                                            )
                                            .append($('<li>')
                                                .append($('<a id="btnBillOfMaterials_' + vData.Rows[intCntr].OrganizationID + '">')
                                                    .attr("href",
                                                        MergeUrlParameters({
                                                            OrgID: vData.Rows[intCntr].OrganizationID,
                                                            IsMobile: IsMobile ? 1 : 0
                                                        }, '/BillOfMaterials')
                                                    )
                                                    .text("Product Maker")
                                                    .data('data', vData.Rows[intCntr])
                                                    .click(function (e) {
                                                        e.preventDefault();

                                                        sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                        //                                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                                        //                                                                    PromptUserAccess('Bill of Materials', {
                                                        //                                                                        OnClickOKSuccess: function () {
                                                        if ($(window).width() > 932) {
                                                            location = MergeUrlParameters({
                                                                OrgID: sessionStorage.OrgID,
                                                                IsMobile: CBool(IsMobile) ? 1 : 0
                                                            }, '/BillOfMaterials');

                                                        } else {
                                                            MessageBox('This module is temporarily unavailable on small screens. Please use a tablet or desktop to view this module. Thank you.', 'Product Maker');
                                                        }
                                                        //                                                                        }
                                                        //                                                                    });


                                                    })
                                                    .append($('<img src="/Content/images/icons/8.png">')
                                                        .css(mIconCss)
                                                    )
                                                )
                                            )
                                    //                                                        .append($('<li>')
                                    //                                                            .append($('<a id="btnCollections_' + vData.Rows[intCntr].OrganizationID + '">')
                                    //                                                                .attr("href",
                                    //                                                                    MergeUrlParameters({
                                    //                                                                        OrgID: vData.Rows[intCntr].OrganizationID,
                                    //                                                                        IsMobile: IsMobile ? 1 : 0
                                    //                                                                    }, '/Collections')
                                    //                                                                )
                                    //                                                                .text("Collections")
                                    //                                                                .data('data', vData.Rows[intCntr])
                                    //                                                                .click(function (e) {
                                    //                                                                    e.preventDefault();

                                    //                                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                    ////                                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                    ////                                                                    PromptUserAccess('Collections', {
                                    ////                                                                        OnClickOKSuccess: function () {

                                    //                                                                            location = MergeUrlParameters({
                                    //                                                                                OrgID: sessionStorage.OrgID,
                                    //                                                                                IsMobile: CBool(IsMobile) ? 1 : 0
                                    //                                                                            }, '/Collections');

                                    ////                                                                        }
                                    ////                                                                    });

                                    //                                                                })
                                    //                                                                .append($('<img src="/Content/images/icons/4.png">')
                                    //                                                                    .css(mIconCss)
                                    //                                                                )
                                    //                                                            )
                                    //                                                        )
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

                                                        proj.PromptTestInternet({
                                                            context: this,
                                                            method: function () {
                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                                if (OfflineMode) {
                                                                    SalesPaymentsExport_offline().HandleUnflushedRecords({
                                                                        Success: function () {
                                                                            sessionStorage.removeItem('IsRequestAndApproval');
                                                                            location = MergeUrlParameters({
                                                                                OrgID: sessionStorage.OrgID,
                                                                                IsMobile: CBool(IsMobile) ? 1 : 0
                                                                            }, '/DashBoardsAndReports');
                                                                        },
                                                                        Error: function () {
                                                                            sessionStorage.removeItem('IsRequestAndApproval');
                                                                            location = MergeUrlParameters({
                                                                                OrgID: sessionStorage.OrgID,
                                                                                IsMobile: CBool(IsMobile) ? 1 : 0
                                                                            }, '/DashBoardsAndReports');
                                                                        }
                                                                    });
                                                                } else {
                                                                    sessionStorage.removeItem('IsRequestAndApproval');
                                                                    location = MergeUrlParameters({
                                                                        OrgID: sessionStorage.OrgID,
                                                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                                                    }, '/DashBoardsAndReports');
                                                                }
                                                            }
                                                        }); // END proj.PromptTestInternet(
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
                                                        //if(Web.HandleOrganization(true)==true)
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

                                                        proj.PromptTestInternet({
                                                            context: this,
                                                            method: function () {
                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                                //$.mobile.changePage('#pagePopUpMenuSettings');
                                                                //edit by arnie mar23
                                                                location = MergeUrlParameters({
                                                                    OrgID: sessionStorage.OrgID,
                                                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                                                }, '/SetupChecklist/SystemAdministration');
                                                            }
                                                        });

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
                                            .append($('<li>')
                                                .append($('<a id="btnCRM_' + vData.Rows[intCntr].OrganizationID + '">')
                                                    .attr("href", "#?"
                                                        + $.param({
                                                            OrgID: vData.Rows[intCntr].OrganizationID
                                                        }))
                                                    .text("Customer Relationship Management")
                                                    .data('data', vData.Rows[intCntr])
                                                    .click(function (e) {
                                                        e.preventDefault();

                                                        sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                        sessionStorage["OrgName"] = $(this).data('data').CompanyName;
                                                        location = MergeUrlParameters({
                                                            OrgID: sessionStorage.OrgID,
                                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                                        }, '/CRMDashboard');

                                                    })
                                                    .append($('<img src="/Content/images/icons/10.png">')
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

                                    var vDataUserDetails;
                                    var userFowst = 'Not Specified';
                                    var userRole = 'Not Specified';
                                    var deviceFowst = 'Not Specified';
                                    var pBatchDate = '';
                                    var pMessageElement = $('<div>');
                                    var pDeviceElement = $('<table>').css('font-weight', 'normal');
                                    var pDeviceAllElement = pDeviceElement.clone(); // $('<table>');

                                    if (OfflineMode) {
                                        var pSalesMenu_offline;
                                        vDataUserDetails = UserDetails.GetUserDetails(vData.Rows[intCntr].OrganizationID);
                                        if (vDataUserDetails != false) {
                                            for (var i = 0; i < vDataUserDetails[0].Fowst.Rows.length; i++) {
                                                if (i == 0) {
                                                    userFowst = vDataUserDetails[0].Fowst.Rows[i].FowstName;
                                                } else {
                                                    userFowst += ', ' + vDataUserDetails[0].Fowst.Rows[i].FowstName;
                                                }
                                            }

                                            for (var m = 0; m < vDataUserDetails[0].Role.Rows.length; m++) {
                                                if (m == 0) {
                                                    userRole = vDataUserDetails[0].Role.Rows[m].Role;
                                                } else {
                                                    userRole += ', ' + vDataUserDetails[0].Role.Rows[m].Role;
                                                }
                                            }

                                            var pOrgID = vData.Rows[intCntr].OrganizationID;
                                            var pBatchDatePrefix;

                                            Web.SetOrgID(pOrgID);

                                            var pTr, pTd;
                                            pTr = $('<tr>').css('font-weight', 'bold');
                                            pTd = $('<td>');
                                            pTd.text('Device Used In');
                                            pTr.append(pTd);

                                            pTd = $('<td>');
                                            pTd.text('Posting Date');
                                            pTr.append(pTd);

                                            pDeviceElement.append(pTr.clone());
                                            pDeviceAllElement.append(pTr);

                                            for (var n = 0; n < vDataUserDetails[0].Device.Rows.length; n++) {
                                                pTr = $('<tr>');

                                                if (n == 0) {
                                                    deviceFowst = vDataUserDetails[0].Device.Rows[n].Name;
                                                } else {
                                                    deviceFowst += ', ' + vDataUserDetails[0].Device.Rows[n].Name;
                                                }
                                                pTd = $('<td>');
                                                pTd.text(vDataUserDetails[0].Device.Rows[n].Name);
                                                pTr.append(pTd);
                                                if (Web.HandleOrganization(true)) {
                                                    if (Business.SystemDefaults.GetEnforceZRead()) {
                                                        pBatchDate = new Date();
                                                        pBatchDatePrefix = 'C ';
                                                    } else {
                                                        sessionStorage.FowstID = vDataUserDetails[0].Device.Rows[n].FowstID;
                                                        if (pSalesMenu_offline == null)
                                                            pSalesMenu_offline = SalesMenu_offline();
                                                        pBatchDate = pSalesMenu_offline.GetZReadDay();
                                                        sessionStorage.removeItem('FowstID');
                                                        pBatchDatePrefix = 'Z ';
                                                    }
                                                    pBatchDate = RemoveTime(pBatchDate);
                                                    pBatchDate = pBatchDatePrefix + $.format.date(pBatchDate, 'MMM d, yyyy');
                                                    deviceFowst += '(' + pBatchDate + ')';

                                                    pTd = $('<td>');
                                                    pTd.text(pBatchDate);
                                                    pTr.append(pTd);
                                                }
                                                if (n < 2)
                                                    pDeviceElement.append(pTr.clone());
                                                pDeviceAllElement.append(pTr);
                                            }

                                            if (vDataUserDetails[0].Device.Rows.length > 2) {
                                                pTr = $('<tr>');
                                                pTd = $('<td>');
                                                pTd.attr('colspan', 2);
                                                pTd.text('Click here to show more');
                                                pTr.append(pTd);
                                                pDeviceElement.append(pTr.clone());
                                            }

                                            pDeviceElement.find('tr:last td').css('padding-bottom', '10px');
                                        }
                                    }

                                    pItem = $('<div data-role="collapsible" data-collapsed="' + (vLoadType || vLoadType == undefined ? "false" : "true") + '" data-theme="b" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">');

                                    pItem.append($('<h2>')
                                        .text(vData.Rows[intCntr].CompanyName)
                                    );
                                    pUl = $('<ul data-role="listview" id="lst' + vData.Rows[intCntr].OrganizationID + '">');
                                    pUl.append($('<li>')
                                        .attr('id', 'li_' + vData.Rows[intCntr].OrganizationID)
                                        .append($('<a id="btnUserDetails_' + vData.Rows[intCntr].OrganizationID + '">')
                                    //                                                            .append($('<p>')
                                    //                                                                .attr("style", "white-space:normal")
                                    //                                                                .html('<br> <strong>FOWST for User:</strong> ' + userFowst)
                                    //                                                            )
                                                .data('OrgData', vData.Rows[intCntr])
                                                .data('DeviceElement', pDeviceAllElement)
                                                .data('UserRole', userRole)
                                                .click(function (e) {
                                                    var pHtml;
                                                    var pOrgData = $(this).data('OrgData');
                                                    var pDeviceElement = $(this).data('DeviceElement');
                                                    var pUserRole = $(this).data('UserRole');
                                                    pHtml = 'Organization: ' + pOrgData.CompanyName + '</ br></ br>';
                                                    pHtml += pDeviceElement.prop('outerHTML') + '</ br>';
                                                    pHtml += '<strong>User Role:</strong> ' + pUserRole;
                                                    NewMessageBox(pHtml, 'Info', [
                                                        {
                                                            Text: 'Ok',
                                                            Click: function () { }
                                                        }
                                                    ]);
                                                    //$('<div>').html(pHtml).popup().popup('show');
                                                })
                                    //.append($('<p>')
                                    //.attr("style", "white-space:normal")
                                    //.html('<br><strong>FOWST for Device:</strong> ' + deviceFowst)
                                    //)
                                            .append(pDeviceElement)
                                            .append($('<p>')
                                                .attr("style", "white-space:normal")
                                                .html('<strong>User Role:</strong> ' + userRole)
                                            )
                                    //.html('Assigned Branch: <span style="font-weight:normal;">' + userFowst + '</span> <br><br> User Role: <span style="font-weight:normal;">' + userRole + '</span>')
                                        )
                                    );

                                    if (pPackage == 'mobiz') {
                                        pUl.append($('<li>')
                                            .append($('<a id="btnPOS_' + vData.Rows[intCntr].OrganizationID + '">')
                                                .text("Point of Sales (POS)")
                                                .data('data', vData.Rows[intCntr])
                                                .click(function (e) {
                                                    e.preventDefault();

                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                    //additional by janine apr 5 2014
                                                    sessionStorage.removeItem('IsHotButtonVisited');
                                                    sessionStorage.IsInventoryCountMode = false;
                                                    ///end of additional

                                                    if (window.IsMobile) {
                                                        if (!IsDatabaseReady()) {
                                                            return;
                                                        }
                                                    }

                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                                    PromptUserAccess('Point Of Sales (POS)', {
                                                        OnClickOKSuccess: function () {

                                                            // MergeUrlParameters
                                                            pParam = {
                                                                mode: 'pos',
                                                                'OrgID': sessionStorage.OrgID
                                                            };
                                                            if (IsMobile)
                                                                pParam.IsMobile = 1;
                                                            pParam = $.param(pParam);

                                                            var diningMode;
                                                            if (OfflineMode)
                                                                diningMode = CBool(Business.SystemDefaults.IsEnableDiningMode());
                                                            else
                                                                diningMode = false;

                                                            if (diningMode) {
                                                                sessionStorage.IsEnableDiningMode = 'true';
                                                                location = MergeUrlParameters({
                                                                    OrgID: sessionStorage["OrgID"],
                                                                    mode: 'pos',
                                                                    IsMobile: IsMobile ? 1 : 0
                                                                }, '/TableView/index2');

                                                            } else {
                                                                sessionStorage.IsEnableDiningMode = 'false';
                                                                window.location = '/SalesMenu?' + pParam;
                                                            }


                                                            //                                                                        if (OfflineMode) {
                                                            //                                                                            IsFlgExist = Transfers.ReadFile(sessionStorage.OrgID + '.flg') == undefined;
                                                            //                                                                            if (IsFlgExist == false) {
                                                            //                                                                                IsFlgExist = Sql.IsExist(sessionStorage.OrgID) == 0;
                                                            //                                                                            }
                                                            //                                                                        }

                                                            //                                                                        if (IsFlgExist) 
                                                            //                                                                            window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                                                            //                                                                        else
                                                            //                                                                            window.location = '/SalesMenu?' + pParam;
                                                        }
                                                    });
                                                })
                                                .append($('<img src="/Content/images/icons/1.png">')
                                                    .css(mIconCss)
                                                )
                                            )
                                        );
                                    } // END if (pPackage == 'mobiz') {

                                    if (pPackage == 'mobiz') {
                                        pUl.append($('<li>')
                                            .append($('<a id="btnSO_' + vData.Rows[intCntr].OrganizationID + '">')
                                                .text("Sales Order Taker")
                                                .data('data', vData.Rows[intCntr])
                                                .click(function (e) {
                                                    e.preventDefault();

                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                                    PromptUserAccess('Sales Order Taker', {
                                                        OnClickOKSuccess: function () {

                                                            // MergeUrlParameters
                                                            pParam = {
                                                                mode: 'so',
                                                                'OrgID': sessionStorage.OrgID
                                                            };
                                                            if (IsMobile)
                                                                pParam.IsMobile = 1;
                                                            pParam = $.param(pParam);

                                                            if (OfflineMode && Sql.IsExist(sessionStorage.OrgID) == 0) {
                                                                window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                                                            } else
                                                                window.location = '/SalesMenu?' + pParam;

                                                        }
                                                    });
                                                })
                                                .append($('<img src="/Content/images/icons/1.png">')
                                                    .css(mIconCss)
                                                )
                                            )
                                        );
                                    } // END if (pPackage == 'mobiz') {

                                    if (pPackage == 'mobiz') {
                                        pUl.append($('<li>')
                                            .append($('<a id="btnPO_' + vData.Rows[intCntr].OrganizationID + '">')
                                                .text("Purchase Order / Intra-Branch Request")
                                                .data('data', vData.Rows[intCntr])
                                                .click(function (e) {
                                                    e.preventDefault();

                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                    //                                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                                    //                                                                PromptUserAccess('Purchase Order', {
                                                    //                                                                    OnClickOKSuccess: function () {

                                                    // MergeUrlParameters
                                                    pParam = {
                                                        mode: 'po',
                                                        'OrgID': sessionStorage.OrgID
                                                    };
                                                    if (IsMobile)
                                                        pParam.IsMobile = 1;
                                                    pParam = $.param(pParam);

                                                    if (OfflineMode && Sql.IsExist(sessionStorage.OrgID) == 0) {
                                                        window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                                                    } else
                                                        window.location = '/SalesMenu?' + pParam;

                                                    //                                                                    }
                                                    //                                                                });
                                                })
                                                .append($('<img src="/Content/images/icons/1.png">')
                                                    .css(mIconCss)
                                                )
                                            )
                                        );
                                    } // END if (pPackage == 'mobiz') {

                                    if (pPackage == 'fc') {
                                        pUl.append($('<li>')
                                            .append($('<a id="btnPick_' + vData.Rows[intCntr].OrganizationID + '">')
                                                .text("Pick Products")
                                                .data('data', vData.Rows[intCntr])
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                    pParam = {
                                                        'OrgID': sessionStorage.OrgID
                                                    };
                                                    if (IsMobile)
                                                        pParam.IsMobile = 1;
                                                    pParam = $.param(pParam);
                                                    window.location = '/PickProducts?' + pParam;
                                                })
                                                .append($('<img src="/Content/images/icons/FC4.png">')
                                                    .css(mIconCss)
                                                )
                                            )
                                        );
                                    } // END if (pPackage == 'mobiz') {2

                                    if (pPackage == 'fc') {
                                        pUl.append($('<li>')
                                            .append($('<a id="btnWarehouseReleasingTracker_' + vData.Rows[intCntr].OrganizationID + '">')
                                                .text("Warehouse Releasing Tracker")
                                                .data('data', vData.Rows[intCntr])
                                                .click(function (e) {
                                                    e.preventDefault();

                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                    pParam = {
                                                        'OrgID': sessionStorage.OrgID,
                                                        'mode': 'wrt'
                                                    };
                                                    if (IsMobile)
                                                        pParam.IsMobile = 1;
                                                    pParam = $.param(pParam);
                                                    window.location = '/WarehouseReleasingTracker?' + pParam;

                                                    //If security access rights is available for this code pls uncomment this
                                                    /*window.pageSecuritySourcePage = $.mobile.activePage;
                                                    PromptUserAccess('WarehouseReleasingTracker', {
                                                    OnClickOKSuccess: function () {
                                                    location = MergeUrlParameters({
                                                    OrgID: sessionStorage["OrgID"],
                                                    mode: 'wrt',
                                                    IsMobile: IsMobile ? 1 : 0
                                                    }, '/SalesMenu');
                                                    }
                                                    });*/

                                                })
                                                .append($('<img src="/Content/images/icons/FC5.png">')
                                                    .css(mIconCss)
                                                )
                                            )
                                        );

                                        pUl.append($('<li>')
                                            .append($('<a id="btnInvoicePickedItems_' + vData.Rows[intCntr].OrganizationID + '">')
                                                .text("Invoice Picked Items")
                                                .data('data', vData.Rows[intCntr])
                                                .click(function (e) {
                                                    e.preventDefault();

                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                    pParam = {
                                                        'OrgID': sessionStorage.OrgID,
                                                        'mode': 'ipi'
                                                    };
                                                    //if (IsMobile)
                                                    //pParam.IsMobile = 1;
                                                    pParam = $.param(pParam);
                                                    window.location = MergeUrlParameters({
                                                        IsMobile: IsMobile,
                                                        'OrgID': sessionStorage.OrgID,
                                                        'mode': 'ipi'
                                                    }, '/InvoicePickedItems');

                                                    //If security access rights is available for this code pls uncomment this
                                                    /*window.pageSecuritySourcePage = $.mobile.activePage;
                                                    PromptUserAccess('WarehouseReleasingTracker', {
                                                    OnClickOKSuccess: function () {
                                                    location = MergeUrlParameters({
                                                    OrgID: sessionStorage["OrgID"],
                                                    mode: 'wrt',
                                                    IsMobile: IsMobile ? 1 : 0
                                                    }, '/SalesMenu');
                                                    }
                                                    });*/

                                                })
                                                .append($('<img src="/Content/images/icons/FC5.png">')
                                                    .css(mIconCss)
                                                )
                                            )
                                        ); // END pUl.append($('<li>')
                                    } // END if (pPackage == 'fc') {

                                    if (pPackage == 'mobiz') {
                                        pUl.append($('<li>')
                                            .append($('<a id="btnSalesReturns_' + vData.Rows[intCntr].OrganizationID + '">')
                                                .text("Sales Returns")
                                                .data('data', vData.Rows[intCntr])
                                                .click(function (e) {
                                                    e.preventDefault();

                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                    proj.PromptTestInternet(
                                                        function () {
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
                                                        }
                                                    ); // END proj.PromptTestInternet

                                                })
                                                .append($('<img src="/Content/images/icons/2.png">')
                                                    .css(mIconCss)
                                                )
                                            )
                                        );
                                    } // END if (pPackage == 'mobiz') {

                                    pUl.append($('<li>')
                                        .append($('<a id="btnReceiving_' + vData.Rows[intCntr].OrganizationID + '">')
                                            .attr("href", GetPathFormat("/Receiving") + '?'
                                                + $.param({
                                                    OrgID: vData.Rows[intCntr].OrganizationID,
                                                    IsReceiving: 1
                                                }))
                                            .text("Stock Receiving")
                                            .data('data', vData.Rows[intCntr])
                                            .click(function (e) {
                                                e.preventDefault();

                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                // 9/9/2014 1:42 PM
                                                // Arvin
                                                // moved online checking within page
                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                                PromptUserAccess('Receiving and Transfers', {
                                                    OnClickOKSuccess: function () {
                                                        location = MergeUrlParameters({
                                                            OrgID: sessionStorage.OrgID,
                                                            IsReceiving: 1,
                                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                                        }, '/Receiving');

                                                    }

                                                });

                                            })
                                            .append($('<img src="/Content/images/icons/3.1.png">')
                                                .css(mIconCss)
                                            )
                                        )
                                    );
                                    pUl.append($('<li>')
                                        .append($('<a id="btnReceiving_' + vData.Rows[intCntr].OrganizationID + '">')
                                            .attr("href", GetPathFormat("/Receiving") + '?'
                                                + $.param({
                                                    OrgID: vData.Rows[intCntr].OrganizationID,
                                                    IsReceiving: 0
                                                }))
                                            .text("Stock Transfers")
                                            .data('data', vData.Rows[intCntr])
                                            .click(function (e) {
                                                e.preventDefault();

                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                // 9/9/2014 1:42 PM
                                                // Arvin
                                                // moved online checking within page
                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                                PromptUserAccess('Receiving and Transfers', {
                                                    OnClickOKSuccess: function () {
                                                        location = MergeUrlParameters({
                                                            OrgID: sessionStorage.OrgID,
                                                            IsReceiving: 0,
                                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                                        }, '/Receiving');
                                                    }

                                                });

                                            })
                                            .append($('<img src="/Content/images/icons/3.2.png">')
                                                .css(mIconCss)
                                            )
                                        )
                                    );
                                    pUl.append($('<li>')
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

                                                //                                                                AjaxSend({
                                                //                                                                    url: 'Login3/TestCookies',
                                                //                                                                    success: function (vData) {

                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                                PromptUserAccess('Inventory Count', {
                                                    OnClickOKSuccess: function () {
                                                        location = MergeUrlParameters({
                                                            OrgID: sessionStorage.OrgID,
                                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                                        }, '/InventoryCount');

                                                        //window.open(GetPathFormat("/InventoryCount") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                    }
                                                });
                                                //                                                                    },
                                                //                                                                    error: function (vError) {
                                                //                                                                        MessageBox("This feature requires internet connection.",
                                                //                                                                                   "Requires Internet Connection",
                                                //                                                                                    [
                                                //                                                                                        { Text: 'Ok'
                                                //                                                                                        }
                                                //                                                                                    ]
                                                //                                                                            );
                                                //                                                                        return;
                                                //                                                                    }

                                                //                                                                });

                                            })
                                            .append($('<img src="/Content/images/icons/4.png">')
                                                .css(mIconCss)
                                            )
                                        )
                                    );
                                    pUl.append($('<li>')
                                        .append($('<a id="btnBillOfMaterials_' + vData.Rows[intCntr].OrganizationID + '">')
                                            .attr("href", GetPathFormat("/BillOfMaterials") + '?'
                                                + $.param({
                                                    OrgID: vData.Rows[intCntr].OrganizationID
                                                }))
                                            .text("Product Maker")
                                            .data('data', vData.Rows[intCntr])
                                            .click(function (e) {
                                                e.preventDefault();

                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                location = MergeUrlParameters({
                                                    OrgID: sessionStorage.OrgID,
                                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                                }, '/BillOfMaterials');

                                                //                                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                                //                                                                PromptUserAccess('Bill of Materials', {
                                                //                                                                    OnClickOKSuccess: function () {
                                                //                                                                if ($(window).width() > 932) {


                                                //                                                                } else {
                                                //                                                                    MessageBox('This module is temporarily unavailable on small screens. Please use a tablet or desktop to view this module. Thank you.', 'Product Maker');
                                                //                                                                }

                                                //                                                                       
                                                //                                                                    }
                                                //                                                                });


                                            })
                                            .append($('<img src="/Content/images/icons/7.png">')
                                                .css(mIconCss)
                                            )
                                        )
                                    );
                                    pUl.append($('<li>')
                                        .append($('<a id="btnShelfAndBinLabeling_' + vData.Rows[intCntr].OrganizationID + '">')
                                            .attr("href", GetPathFormat("/ShelfAndBinLabeling") + '?'
                                                + $.param({
                                                    OrgID: vData.Rows[intCntr].OrganizationID
                                                }))
                                            .text("Shelf And Bin Labelling")
                                            .data('data', vData.Rows[intCntr])
                                            .click(function (e) {
                                                e.preventDefault();

                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                //                                                                if (IsMobile) {
                                                //                                                                    MessageBox("Sorry, You can use this module using desktop/laptop computer.", "Shelf And Bin Labelling");
                                                //                                                                } else {
                                                //                                                                    
                                                //                                                                }
                                                location = MergeUrlParameters({
                                                    OrgID: sessionStorage.OrgID,
                                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                                }, '/ShelfAndBinLabeling');

                                            })
                                            .append($('<img src="/Content/images/icons/8.png">')
                                                .css(mIconCss)
                                            )
                                        )
                                    );

                                    if (pPackage == 'mobiz') {
                                        pUl.append($('<li>')
                                            .append($('<a>')
                                                .attr("href", '#')
                                                .text("Request and Approvals")
                                                .data('data', vData.Rows[intCntr])
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                    sessionStorage.IsRequestAndApproval = 1;
                                                    location = MergeUrlParameters({
                                                        OrgID: sessionStorage.OrgID
                                                    }, '/DashBoardsAndReports');
                                                })
                                                .append($('<img src="/Content/images/icons/Approvals.png">')
                                                    .css(mIconCss)
                                                )
                                            )
                                        );
                                    } // END if (pPackage == 'mobiz') {
                                    //                                                    .append($('<li>')
                                    //                                                        .append($('<a id="btnCollections_' + vData.Rows[intCntr].OrganizationID + '">')
                                    //                                                            .attr("href",
                                    //                                                                MergeUrlParameters({
                                    //                                                                    OrgID: vData.Rows[intCntr].OrganizationID,
                                    //                                                                    IsMobile: IsMobile ? 1 : 0
                                    //                                                                }, '/Collections')
                                    //                                                            )
                                    //                                                            .text("Collections")
                                    //                                                            .data('data', vData.Rows[intCntr])
                                    //                                                            .click(function (e) {
                                    //                                                                e.preventDefault();

                                    //                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                    //                                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                    ////                                                                PromptUserAccess('Collections', {
                                    ////                                                                    OnClickOKSuccess: function () {

                                    //                                                                        location = MergeUrlParameters({
                                    //                                                                            OrgID: sessionStorage.OrgID,
                                    //                                                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                    //                                                                        }, '/Collections');

                                    ////                                                                    }
                                    ////                                                                });

                                    //                                                            })
                                    //                                                            .append($('<img src="/Content/images/icons/4.png">')
                                    //                                                                .css(mIconCss)
                                    //                                                            )
                                    //                                                        )
                                    //                                                    )
                                    if (pPackage == 'mobiz') {
                                        pUl.append($('<li>')
                                            .append($('<a id="btnCashHandling_' + vData.Rows[intCntr].OrganizationID + '">')
                                                .attr("href", "#?"
                                                    + $.param({
                                                        OrgID: vData.Rows[intCntr].OrganizationID
                                                    }))
                                                .text("Cash Handling")
                                                .data('data', vData.Rows[intCntr])
                                                .click(function (e) {
                                                    var pOfflineModeFunction;

                                                    e.preventDefault();


                                                    sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                    //if (Web.HandleOrganization(true) == true)
                                                    $.mobile.changePage('#pagePopUpMenuCashHandling');

                                                }) // END click
                                                .append($('<img src="/Content/images/icons/12.png">')
                                                    .css(mIconCss)
                                                )
                                            )
                                        );
                                    } // END if (pPackage == 'mobiz') {

                                    pUl.append($('<li>')
                                        .append($('<a id="btnDashboardsAndReports_' + vData.Rows[intCntr].OrganizationID + '">')
                                            .attr("href", "#?"
                                                + $.param({
                                                    OrgID: vData.Rows[intCntr].OrganizationID
                                                }))
                                            .text("Dashboards and Reports")
                                            .data('data', vData.Rows[intCntr])
                                            .click(function (e) {
                                                var pOfflineModeFunction;

                                                e.preventDefault();
                                                sessionStorage["OrgName"] = $(this).data('data').CompanyName;
                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                if (OfflineMode)
                                                    pOfflineModeFunction = function () {
                                                        //Android.Toast('Cannot Connect to Server');
                                                        //sessionStorage.PrintingOfflineData = true;
                                                        //GotoPage();
                                                        MessageBox('', 'Offline Reports', [
                                                            {
                                                                Text: 'Cashiers Sales Report',
                                                                Click: function () {
                                                                    sessionStorage.PrintingOfflineData = true;
                                                                    sessionStorage.OfflineType = 'cashier';
                                                                    GotoPage();
                                                                }
                                                            },
                                                            {
                                                                Text: 'Monies Accountability',
                                                                Click: function () {
                                                                    sessionStorage.PrintingOfflineData = true;
                                                                    sessionStorage.OfflineType = 'monies';
                                                                    GotoPage();
                                                                }
                                                            },
                                                            {
                                                                Text: 'Inventories Accountability',
                                                                Click: function () {
                                                                    sessionStorage.PrintingOfflineData = true;
                                                                    sessionStorage.OfflineType = 'inventories';
                                                                    GotoPage();
                                                                }
                                                            },
                                                            {
                                                                Text: 'Transaction Details',
                                                                Click: function () {
                                                                    sessionStorage.PrintingOfflineData = true;
                                                                    sessionStorage.OfflineType = 'transaction';
                                                                    GotoPage();
                                                                }
                                                            },
                                                            {
                                                                Text: 'Inventory Ledger',
                                                                Click: function () {
                                                                    sessionStorage.PrintingOfflineData = true;
                                                                    sessionStorage.OfflineType = 'inventoryledger';
                                                                    GotoPage();
                                                                }
                                                            },
                                                            {
                                                                Text: 'Cancel',
                                                                Click: function () {
                                                                    window.location = '/People/PeopleMenu';
                                                                }
                                                            }
                                                        ])
                                                    }
                                                proj.PromptTestInternet({
                                                    context: this,
                                                    method: function () {
                                                        sessionStorage.PrintingOfflineData = false;
                                                        if (OfflineMode)
                                                            SalesPaymentsExport_offline().HandleUnflushedRecords({
                                                                Success: function () {
                                                                    GotoPage();
                                                                }
                                                            });
                                                        else
                                                            GotoPage();
                                                    },
                                                    error: pOfflineModeFunction
                                                }); // END proj.PromptTestInternet(

                                                function GotoPage() {
                                                    sessionStorage.removeItem('IsRequestAndApproval');
                                                    location = MergeUrlParameters({
                                                        OrgID: sessionStorage.OrgID,
                                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                                    }, '/DashBoardsAndReports');
                                                }
                                            }) // END click
                                            .append($('<img src="/Content/images/icons/5.png">')
                                                .css(mIconCss)
                                            )
                                        )
                                    );
                                    pUl.append($('<li>')
                                        .append($('<a id="btnTroubleshootingTools_' + vData.Rows[intCntr].OrganizationID + '">')
                                            .text("Troubleshooting Tools")
                                            .data('data', vData.Rows[intCntr])
                                            .click(function (e) {
                                                e.preventDefault();

                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                //if (Web.HandleOrganization(true) == true)
                                                $.mobile.changePage('#pagePopUpMenuTroubleShootingTools');

                                            })
                                            .append($('<img src="/Content/images/icons/9.png">')
                                                .css(mIconCss)
                                            )
                                        )
                                    );
                                    pUl.append($('<li>')
                                        .append($('<a id="btnBillOfMaterials_' + vData.Rows[intCntr].OrganizationID + '">')
                                            .attr("href", GetPathFormat("/BillOfMaterials") + '?'
                                                + $.param({
                                                    OrgID: vData.Rows[intCntr].OrganizationID
                                                }))
                                            .text("How To")
                                            .data('data', vData.Rows[intCntr])
                                            .click(function (e) {
                                                e.preventDefault();

                                                //sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                //location = MergeUrlParameters({
                                                //    OrgID: sessionStorage.OrgID,
                                                //    IsMobile: CBool(IsMobile) ? 1 : 0
                                                //}, '/BillOfMaterials');
                                                window.open('https://mobizblogs.wordpress.com', '_blank');
                                            })
                                            .append($('<img src="/Content/images/icons/10.png">')
                                                .css(mIconCss)
                                            )
                                        )
                                    );
                                    pUl.append($('<li>')
                                        .append($('<a id="btnSettings_' + vData.Rows[intCntr].OrganizationID + '">')
                                            .attr("href", "#?"
                                                + $.param({
                                                    OrgID: vData.Rows[intCntr].OrganizationID
                                                }))
                                            .text("System Administration")
                                            .data('data', vData.Rows[intCntr])
                                            .click(function (e) {
                                                e.preventDefault();

                                                proj.PromptTestInternet({
                                                    context: this,
                                                    method: function () {
                                                        sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                        //$.mobile.changePage('#pagePopUpMenuSettings');
                                                        //edit by arnie mar23
                                                        location = MergeUrlParameters({
                                                            OrgID: sessionStorage.OrgID,
                                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                                        }, '/SetupChecklist/SystemAdministration');
                                                    }
                                                });
                                            })
                                            .append($('<img src="/Content/images/icons/11.png">')
                                                .css(mIconCss)
                                            )
                                        )
                                    );

                                    if (vData.Rows[intCntr].OrganizationID == '28b8a792-cb65-4d92-988f-d721123c93b9') {
                                        pUl.append($('<li>')
                                            .append($('<a id="btnCRMDashboard_' + vData.Rows[intCntr].OrganizationID + '">')
                                                .attr("href", "#?"
                                                    + $.param({
                                                        OrgID: vData.Rows[intCntr].OrganizationID
                                                    }))
                                                .text("CRM")
                                                .data('data', vData.Rows[intCntr])
                                                .click(function (e) {
                                                    e.preventDefault();

                                                    proj.PromptTestInternet({
                                                        context: this,
                                                        method: function () {
                                                            sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                            sessionStorage["OrgName"] = $(this).data('data').CompanyName
                                                            //$.mobile.changePage('#pagePopUpMenuSettings');
                                                            //edit by arnie mar23
                                                            location = MergeUrlParameters({
                                                                OrgID: sessionStorage.OrgID,
                                                                IsMobile: CBool(IsMobile) ? 1 : 0
                                                            }, '/CRMDashboard');
                                                        }
                                                    });
                                                })
                                                .append($('<img src="/Content/images/icons/11.png">')
                                                    .css(mIconCss)
                                                )
                                            )
                                        );
                                    }
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

                                    pItem.append(pUl);

                                    pLV.append(pItem);

                                    pLV.trigger("create");

                                    if (OfflineMode) {
                                        if (vDataUserDetails == false) {
                                            $('#li_' + vData.Rows[intCntr].OrganizationID).hide();
                                        }
                                    }
                                    else {
                                        $('#li_' + vData.Rows[intCntr].OrganizationID).hide();
                                    }

                                    OrgID[intCntr] = { OrgID: vData.Rows[intCntr].OrganizationID };
                                } //If (OrgID
                            } //If (IsLoadWithDbase
                        } //For

                        //                        if ((OrgID.length > 0)) {
                        //                            GetDatabaseNames(OrgID);
                        //                        }

                    }

                    SetFlag('GQDVAYJ5U5');
                } // END DisplayPeopleMenu

                //                function GetDatabaseNames(m_data) {
                //                    var data = { OrgID: m_data };

                //                    if (!(OrgID == null)) {
                //                        AjaxSend({
                //                            url: 'People/GetDatabaseName',
                //                            data: data,
                //                            success: function (vData) {
                //                                var pLV = $("#tblOrganization");


                //                                $('#tblOrganization tr').empty();
                //                                $('#lstOrganization li').not(':eq(0)').not(':eq(0)').remove();

                //                                for (var i = 0; i <= vData.Results.Rows.length - 1; i++) {

                //                                    pItem = $('<tr>')
                //                                        .append($('<td>')
                //                                            .append($('<input type=checkbox />')
                //                                                .attr('ID', vData.Results.Rows[i].ID)
                //                                                .data('data', vData.Results.Rows[i])
                //                                            )
                //                                            .append($('<label>')
                //                                                .attr('for', vData.Results.Rows[i].ID)
                //                                                .text(vData.Results.Rows[i].CompanyName)
                //                                            )
                //                                        )
                //                                    pLV.append(pItem);
                //                                    $('#' + vData.Results.Rows[i].ID).checkboxradio();
                //                                };

                //                                //done button
                //                                pItem = $('<li>')
                //                                        .attr('data-icon', 'false')
                //                                        .attr('data-theme', 'e')
                //                                        .append($('<a>')
                //                                            .text('Done')
                //                                            .click(function () {
                //                                                if (IsMobile) {
                //                                                    alert('call arvin code');
                //                                                }
                //                                                else {
                //                                                    MessageBox("Sorry, your current device is not capable of performing this function", "People");
                //                                                }
                //                                            })
                //                                        )
                //                                //pLV.append(pItem);
                //                                $('#lstOrganization').append(pItem);
                //                                $('#lstOrganization').listview('refresh');
                //                            }
                //                        });
                //                    }
                //                }

                function IsDatabaseReady() {
                    if (Sql.IsExist(sessionStorage.OrgID)) {
                        Sql.OpenDatabase(sessionStorage.OrgID);
                        //check if db has record

                        try {
                            var pSql = 'select * from librarydevice limit 1';

                            var pDT = Sql.Execute(pSql);

                            if (pDT.Rows.length == 0) {
                                return RestoreBackupDatabase();
                            }
                            else {
                                Sql.OpenDatabase('Mobiz');
                                return true;
                            }
                        }
                        catch (e) {
                            return RestoreBackupDatabase();
                        }



                    }
                    else {
                        return RestoreBackupDatabase();
                    }
                }


                function RestoreBackupDatabase() {
                    var flgCounter = 1;

                    while (flgCounter < 17) {
                        IsFlagExist = Transfers.ReadFile(sessionStorage.OrgID + '-' + flgCounter + '.flg');
                        if (IsFlagExist == undefined) {
                            try {
                                if (flgCounter == 1) {
                                    if (Sql.IsExist(sessionStorage.OrgID + '-16.backup')) {
                                        Transfers.MoveFile('/data/' + sessionStorage.OrgID + '-16.backup.db', '/data/' + sessionStorage.OrgID + '.db');
                                        Transfers.DeleteFile(sessionStorage.OrgID + '-16.flg');
                                        Android.Toast('The system found problems in your data, please Synchronize your data as soon as possible.')
                                        Sql.OpenDatabase('Mobiz');
                                        return true;
                                    }
                                }
                                else {
                                    if (Sql.IsExist(sessionStorage.OrgID + '-' + (flgCounter - 1) + '.backup')) {
                                        Transfers.MoveFile('/data/' + sessionStorage.OrgID + '-' + (flgCounter - 1) + '.backup.db', '/data/' + sessionStorage.OrgID + '.db');
                                        Transfers.DeleteFile(sessionStorage.OrgID + '-' + (flgCounter - 1) + '.flg');
                                        Android.Toast('The system found problems in your data, please Synchronize your data as soon as possible.')
                                        Sql.OpenDatabase('Mobiz');
                                        return true;
                                    }
                                }

                            }
                            catch (e) {

                            }

                        }
                        flgCounter++;
                    }
                    window.location = '/SignIn/DownloadDatabase?IsMobile=1';
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
                        },
                        error: function () {
                            proj.PromptRequiresInternet();
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

                    proj.PromptTestInternet(
                        function () {
                            //Get Messages List
                            AjaxSend({
                                url: "People/getPeopleMessageList",
                                success: function (vDataMsgList) {
                                    if (OfflineMode)
                                        PeopleMenu_offline().SaveOfflineMessageList(vDataMsgList);
                                    ShowData(vMsgData, vDataMsgList);
                                },
                                error: function () {
                                    try {
                                        var pData = PeopleMenu_offline().LoadOfflineMessageList();
                                        if (pData == null)
                                            proj.PromptRequiresInternet();
                                        else
                                            ShowData(vMsgData, pData);
                                    } catch (e) {
                                        // just ignore any errors here
                                        console.trace(e.stack);
                                    }
                                }
                            });
                        }
                    );

                    function ShowData(vMsgData, vDataMsgList) {

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
                    } // END ShowData
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
                                        //added by arnie Jan 23 5:15pm
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

                function ShowOrganizationEmployeeWorkingHours() {

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            var pData = {};
                            pData.OrgID = sessionStorage["OrgID"];

                            window.pageSourcePage = $.mobile.activePage;

                            window.location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/Organization/OrganizationEmployeeWorkingHours');
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
                                        //ChangePage('/Organization/OrganizationFOWSTProduct');
                                        window.location = MergeUrlParameters({
                                            OrgID: sessionStorage.OrgID,
                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                        }, '/Organization/OrganizationFOWSTProduct');
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

                function ClearFields() {
                    $("#selProduct").val('General Update');
                    $('#selProduct').selectmenu('refresh');
                    $("#selCategory").val('');
                    $('#selCategory').selectmenu('refresh');
                    $("#txtTitle").val('');
                    $("#txtMessage").val('');
                }

                function IsHasDBase(vOrgID) {
                    var IsExists;

                    IsExists = Transfers.ReadFile(vOrgID + '.flg')

                    if (IsExists == undefined) {
                        return false;
                    }
                    else {
                        return true;
                    }
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

                $('#popCRM').click(function (e) {
                    e.preventDefault();
                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            window.pageSecuritySourcePage = $.mobile.activePage;

                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID
                            }, '/CRM');

                            //                            PromptUserAccess('CRM', {
                            //                                OnClickOKSuccess: function () {
                            //                                    location = MergeUrlParameters({
                            //                                        OrgID: sessionStorage.OrgID
                            //                                    }, '/CRM');
                            //                                }
                            //                            });
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

                $('#popFranchisorCentral').click(function (e) {
                    e.preventDefault();
                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            PromptUserAccess('Group Settings', {
                                OnClickOKSuccess: function () {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID
                                    }, '/Franchisor');
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

                $('#popCompanyPrinters').click(function (e) {
                    e.preventDefault();

                    AjaxSend({
                        url: 'Login3/TestCookies',
                        success: function (vData) {
                            //                    window.pageSecuritySourcePage = $.mobile.activePage;
                            //                    PromptUserAccess('Company Supplier', {
                            //                        OnClickOKSuccess: function () {
                            //window.open(GetPathFormat("/Device") + "?OrgID=" + sessionStorage["OrgID"], "_self");

                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID
                            }, '/DevicePrinter');

                            //                        }
                            //                    });
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

                function SetPopUpMessage(IsComparing, Table) {

                    var vTable = '';
                    var msg = '';

                    if (vTable.indexOf('Error') > 0) {
                        msg = vTable;
                    }
                    else if (IsComparing == 1) {
                        msg = 'Checking updates for ' + vTable;
                    }
                    else if (IsComparing == 2) {
                        msg = 'Updating ' + vTable;
                    }
                    else {
                        msg = Table;
                    }

                    msg = 'Please Wait ... <br> <br>' + msg;

                    try { $("#dvpopup").popup("close"); } catch (e) { }; // just ignore any errors here
                    try { $('#popMessage').html(msg); } catch (e) { };   // just ignore any errors here
                    try { $("#dvpopup").popup("open"); } catch (e) { };  // just ignore any errors here
                }

            });
        }); // require PeopleMenu_offline.js
    });