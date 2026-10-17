require([
    'common', 'business', '/Scripts3/controller/PrePos.js'
], function () {

    require([(OfflineMode ? '/Scripts/sources/People/PeopleMenu_offline.js?crc=' : ''),
             (OfflineMode ? '/Scripts/sources/ImportExportData/ImportExportData.js?crc=' : ''),
             (OfflineMode ? '/Scripts/sources/ImportExportData/ImportExportData_offline.js?crc=' : ''),
             (OfflineMode ? '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc=' : ''),
             (OfflineMode ? '/Scripts3/SalesMenu_offline.js?crc=' : ''),
             (OfflineMode ? '/Scripts3/model/PeopleMenuUserDetails_Offline.js?crc=' : ''),
             (OfflineMode ? '' : 'promise!/Scripts/sources/CRMDashboard/Tickets.js?crc=' + assetversion),
             (OfflineMode ? '' : '/Scripts/sources/SetupChecklist/NewPromptUser.js?v=' + assetversion)
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
            var mPrePos = new App.controller.PrePos();
            var mPriceChecker; // App.controller.PriceChecker
            var pParam;
            var mDatabaseCreationStart;

            var UserDetails = {};

            if (OfflineMode) {
                UserDetails = new App.model.PeopleMenuUserDetails_offline();
                var pAppVersionName = Android.AppVersionName();
                $('[name=divVersion]').text('Version: ' + pAppVersionName);
                $('[name=divVersionContainer]').css({
                    visibility: 'visible'
                });
                $('[name=divButMessages]').hide();
                $('[name=divButOnline]').show();
                var mIsConnected = null;
                (function PoolOnline() {
                    AjaxSend({
                        url: '/Home/GetDate',
                        timeout: 5000,
                        success: function(vData) {
                            setTimeout(PoolOnline, 60 * 1000);
                            mIsConnected = true;
                            $('#btnOnline span').removeClass('bi-cloud-fill');
                            $('#btnOnline span').removeClass('bi-cloud-slash-fill');
                            $('#btnOnline span').addClass('bi-cloud-check-fill');
                        },
                        error: function() {
                            setTimeout(PoolOnline, 60 * 1000);
                            mIsConnected = false;
                            $('#btnOnline span').removeClass('bi-cloud-fill');
                            $('#btnOnline span').removeClass('bi-cloud-check-fill');
                            $('#btnOnline span').addClass('bi-cloud-slash-fill');
                        }
                    });
                })();

                $('#btnOnline').click(function() {
                    if (mIsConnected == true) 
                        NewMessageBox('App is online');
                    else
                        NewMessageBox('App is offline');
                });
            } // END if (OfflineMode) {

            if (IsMobile) {
                $('#btnAnnouncement').parent().removeAttr('style');
                proj.InitDoneSchema({
                    DontKick: true
                });
                mPrePos.CanTransactWithUnflushedData();
            }

            sessionStorage.removeItem('FowstID'); // pending disconnected checkout 4/15/2014 4:51 PM

            sessionStorage.removeItem('CustomerOrganizationID'); // added by janine may 14 1pm . reset customer id

            sessionStorage.removeItem('IsVendingMachine'); // added by Arnie

            if (ReadyGuid(sessionStorage.OrgID) == '565C260C-3844-4069-AC41-D26F0598E562') {
                // temp lang ni, since dili pa accredited ang Mobiz usab. Will remove once accredited again
                $('.lblVersion').removeClass('d-none');
            }

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
                                            // 2017_01_09__16_10_13_0170 arvin commented these:
                                            //sessionStorage.removeItem("OrgID");
                                            //sessionStorage.removeItem("FowstID");

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
                                    {
                                        Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }

                });

            });

            $('#popPeopleMenuToggleFullscreen').click(function (e) {
                e.preventDefault();

                NewMessageBox('not yet implemented. ref: jsR94AQVEFDT!');

            });

            if (true) {
                $('#lnkSecondDisplaySetup').show();
                $('#lnkSecondDisplaySetup').click(function () {
                    require(['promise!/Scripts3/controller/SecondaryDisplaySetup.js?crc='], function () {
                        var p = new App.controller.SecondaryDisplaySetup();
                        p.popup();
                    });
                });

                $('#lnkOrderTakerSetup').show();
                $('#lnkSendingTransactionBySMS').show();
                $('#lnkFollowupSMSTransaction').show();

                $('#lnkOrderTakerSetup').click(function () {
                    //PromptUserAccess('Order Taking Setup', {
                    PromptUserAccess('Sales Order Taker', {
                        OnClickOKSuccess: function () {
                            require(['promise!/Scripts3/controller/OrderTakerSetup.js?crc='], function () {
                                var p = new App.controller.OrderTakerSetup();
                                p.popup();
                            });
                        }
                    });
                });

                $('#lnkFollowupSMSTransaction').click(function () {
                    var pSettings = Transfers.ReadFile('SMSGateWaySettings.txt');

                    if (pSettings != undefined) {

                        pSettings = JSON.parse(pSettings)

                        try {
                            if (CBool(pSettings.IsAllowToSendTransactionViaSMS)) {
                                if (pSettings.DefaultIPAddress.length > 0) {

                                    SmSMessageSender = pSettings.DefaultIPAddress;

                                    pSql = "Insert into TransSmSMessages(ID, SendTo, Message, SentToModemTries) Values (" +
                                        "'0c472244-ad9c-4d80-b5fe-4d3e1ecc2822'," +
                                        "'" + SmSMessageSender + "'," +
                                        "'" + "CanISendSMSMessages" + "'," +
                                        0 + ")";

                                    Sql.Execute(pSql);

                                    MessageBox("Request for follow up has been sent via SMS.");
                                }
                            }
                        } catch (e) {
                            alert(e)
                        }
                    }

                });

                $('#lnkSendingTransactionBySMS').click(function () {
                    var pSettings = Transfers.ReadFile('SMSGateWaySettings.txt');

                    if (pSettings != undefined) {

                        pSettings = JSON.parse(pSettings)
                        try {
                            if (CBool(pSettings.IsAllowToSendTransactionViaSMS)) {
                                $('#chkAllowDeviceToSendTransactionViaSMS').prop('checked', CBool(pSettings.IsAllowToSendTransactionViaSMS)).checkboxradio("refresh");
                            } else {
                                $('#chkAllowDeviceToSendTransactionViaSMS').prop('checked', CBool(pSettings.IsAllowToSendTransactionViaSMS)).checkboxradio("refresh");
                            }
                        } catch (e) {
                            $('#chkAllowDeviceToSendTransactionViaSMS').prop('checked', CBool(pSettings.IsAllowToSendTransactionViaSMS)).checkboxradio();
                        }

                        $('#txtSMSGateWayMobileNo').val(pSettings.DefaultIPAddress)
                        $('#txtOverrideSMSGateWayMobileNo').val(pSettings.OverrideIPAddress)

                    }

                    $('#panSendingTransactionBySMS').modal('show');
                })

                $('#lnkZRead').click(function () {
                    var vDataUserDetails = UserDetails.GetUserDetails(sessionStorage.OrgID);
                    var pBatchDate = '';
                    var pSalesMenu_offline;

                    PromptUserAccess('Adjust Z-Read Date', {
                        OnClickOKSuccess: function () {

                            if (vDataUserDetails != false) {
                                if (Web.HandleOrganization(true)) {
                                    if (Business.SystemDefaults.GetEnforceZRead()) {
                                        sessionStorage.FowstID = vDataUserDetails[0].Device.Rows[0].FowstID;
                                        if (pSalesMenu_offline == null)
                                            pSalesMenu_offline = SalesMenu_offline();
                                        pBatchDate = pSalesMenu_offline.GetZReadDay();
                                        //sessionStorage.removeItem('FowstID');
                                    }

                                    pBatchDate = RemoveTime(pBatchDate);
                                    pBatchDate = $.format.date(pBatchDate, 'MMM d, yyyy');

                                    $('#txtZReadDate').attr('max', $.format.date(new Date(), 'yyyy-MM-dd')).val($.format.date(new Date(pBatchDate), 'yyyy-MM-dd'));
                                    $('#lblCurrentZReadDate').text(pBatchDate);
                                }
                            }
                            $('#panEditZReadDate').modal('show');

                        },
                        OnFailed: function () {
                            NewMessageBox('Sorry, you don\'t have access adjust z-read date', '');
                        }
                    });
                })

                $('#btnSaveZReadDate').click(function () {
                    var mThis = SalesMenu_offline();
                    var pDate2 = $('#txtZReadDate').val();

                    if (IsDate(pDate2)) {

                        //make pDate javascript date;
                        pDate2 = new Date(pDate2);

                        var vParams = $.extend({
                            DontLimit: false
                        }, vParams);

                        var pDate = mThis.GetZReadDay();

                        pDate.setDate(pDate2.getDate());
                        pDate = FormatDate(pDate);
                        var pData = mThis.LoadZReadDay();
                        try {
                            pData = JSON.parse(pData);
                        } catch (e) {
                            pData = {};
                        }
                        var pPath = ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID);
                        CreateStringObject(pPath, pData, pDate);
                        mThis.SaveZReadDay(pData);

                        var pIU;

                        pIU = new InsertUpdate('private.TransActionLogs');

                        pIU.Add('ID', ReadyString(NewID()));
                        pIU.Add('ActionDate', proj.SqliteNow);
                        pIU.Add('EmployeeName', ReadyString(sessionStorage.CurrentUserFullName));
                        pIU.Add('FOWSTID', ReadyString(sessionStorage.FowstID));
                        pIU.Add('DeviceID', ReadyString(Device.Serial()));
                        pIU.Add('Action', ReadyString("Adjust Z-Read Date"));
                        pIU.Add('OriginalValue', ReadyString($('#lblCurrentZReadDate').text()));
                        pIU.Add('NewValue', ReadyString($('#txtZReadDate').val()));

                        pSql = pIU.GetInsert();

                        Sql.ExecuteSql(pSql);

                        $('#panEditZReadDate').modal('hide');

                        sessionStorage.removeItem('FowstID');
                    }
                })

                $('#btnSaveSMSGateWayMobileNo').click(function () {
                    var pSettings = {
                        IsAllowToSendTransactionViaSMS: CBool($('#chkAllowDeviceToSendTransactionViaSMS').prop('checked')),
                        DefaultIPAddress: $('#txtSMSGateWayMobileNo').val(),
                        OverrideIPAddress: $('#txtOverrideSMSGateWayMobileNo').val()
                    }

                    SaveSMSGateWaySettings(pSettings);
                })

                function SaveSMSGateWaySettings(pSettings) {
                    Transfers.WriteFile('SMSGateWaySettings.txt', null, pSettings);

                    $('#panSendingTransactionBySMS').modal('hide');
                }
            }

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
                                    window.open(GetPathFormat("/SalesPayments/BsDuplicateReceipt") + "?OrgID=" + sessionStorage["OrgID"] + "&IsMobile=1", "_self");
                                else
                                    window.open(GetPathFormat("/SalesPayments/BsDuplicateReceipt") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                            }
                        });
                    }
                );

            });

            $('#popTroubleshootingToolsReprintZRead').click(function (e) {
                e.preventDefault();

                proj.PromptTestInternet(
                    function () {
                        //window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess('Allow users to view the ZRead report online', {
                            Message: 'Sorry! You don\'t have access to this feature.',
                            OnClickOKSuccess: function () {
                                window.open(GetPathFormat("/ReportsPOS/ZReport") + "?OrgID=" + sessionStorage["OrgID"] + "&Online=1", "_self");
                            }
                        });
                    }
                );

            });

            $('#txtCacheDBDateTo').val($.format.date(new Date(), 'yyyy-MM-dd'));

            $('#selCacheDBReportType').change(function () {
                if ($(this).val() == 'zread') {
                    $('#selCacheDBCashierName').parent().hide();
                } else $('#selCacheDBCashierName').parent().show();
            })

            $('#popTroubleshootingToolsUpdateCacheDatabase').click(function (e) {
                e.preventDefault();

                proj.PromptTestInternet(
                    function () {
                        //window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess('Allow users to update data in the cached database', {
                            Message: 'Sorry! You don\'t have access to this feature.',
                            OnClickOKSuccess: function () {

                                if (SalesPaymentsExport_offline().HasUnflushedRecords()) {
                                    Bs.Loading('show', 'We\'re flushing your data to the server. Please wait a moment.')
                                }
                                
                                //SalesPaymentsExport_offline().FlushDeviceData({
                                //    Success: function () {
                                        Bs.Loading('hide');
                                        require(['/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc='], function () {
                                            var fowstdevice = OfflineBrowser.LibraryFowst.GetData({ FilterFowst: 1 }).Rows;
                                            var selCacheDBStore = $('#selCacheDBStore').empty();

                                            if (fowstdevice.length > 1) {
                                                selCacheDBStore.append(new Option('Please select', ''));
                                            }

                                            for (var i in fowstdevice) {
                                                selCacheDBStore.append(new Option(fowstdevice[i].FowstName, fowstdevice[i].ID));
                                            }

                                            selCacheDBStore.unbind().change(function () {
                                                sessionStorage.FowstID = $(this).val();
                                                AjaxSend({
                                                    url: 'Browser/LibraryFOWSTRealCashier',
                                                    data: {
                                                        FowstID: $(this).val(),
                                                        DeviceID: Web.DeviceID()
                                                    }, success: function (e) {
                                                        var arrCashiers = e.Results.Rows;
                                                        var selCacheDBCashierName = $('#selCacheDBCashierName').empty();

                                                        for (var i in arrCashiers) {
                                                            selCacheDBCashierName.append(new Option(arrCashiers[i].CashierName, arrCashiers[i].ID));
                                                        }
                                                    }
                                                })
                                            }).change()

                                            $('#txtCacheDBDateFrom, #txtCacheDBDateTo').attr('max', $.format.date(new Date(), 'yyyy-MM-dd'));
                                            
                                            $('#chbAllDayDateFrom').prop('checked', 1).unbind().change(function () {
                                                var txtCacheDBTimeFrom = $('#txtCacheDBTimeFrom').val('00:00');
                                                $(this).is(':checked') ? txtCacheDBTimeFrom.parent().addClass('d-none') : txtCacheDBTimeFrom.parent().removeClass('d-none');
                                            });
                                            $('#chbAllDayDateTo').prop('checked', 1).unbind().change(function () {
                                                var txtCacheDBTimeTo = $('#txtCacheDBTimeTo').val('23:59');
                                                $(this).is(':checked') ? txtCacheDBTimeTo.parent().addClass('d-none') : txtCacheDBTimeTo.parent().removeClass('d-none');
                                            });
                                            $('#txtCacheDBTimeTo, #txtCacheDBTimeTo').parent().addClass('d-none');
                                            $('#txtCacheDBTimeFrom').val('00:00');
                                            $('#txtCacheDBTimeTo').val('23:59');
                                            $('#panUpdateCacheDB').modal('show');
                                        })
                                //    },
                                //    Fail: function () {
                                //        Bs.Loading('hide');
                                //        NewMessageBox('Unable to flush your data. Please check your internet connection.')
                                //    }
                                //})
                            }
                        });
                    }
                );

            });

            $('#btnUpdateCacheDBNow').click(function (e) {
                e.preventDefault();
                var selCacheDBStore = CStr2($('#selCacheDBStore option:selected').val());
                var dateFrom = CStr2($('#txtCacheDBDateFrom').val());
                var dateTo = CStr2($('#txtCacheDBDateTo').val());
                var timeFrom = CStr2($('#txtCacheDBTimeFrom').val());
                var timeTo = CStr2($('#txtCacheDBTimeTo').val());
                var ctr = 1;

                if (selCacheDBStore == '') {
                    NewMessageBox('Please select store');
                    return;
                }

                if (dateFrom == '') {
                    NewMessageBox('Please set the starting date');
                    return;
                }

                sessionStorage.FowstID = selCacheDBStore;

                function Recompute(vDateFrom, vDateTo) {
                    Android.AddLog('Retrieving sales data for: ' + $.format.date(vDateFrom, 'MMM dd, yyyy'))
                    Bs.Loading('show', ctr + ' of ' + ((new Date(dateTo) - new Date(dateFrom)) / (1000 * 60 * 60 * 24) + 1) + '<br><br>We are currently retrieving sales data for ' + $.format.date(vDateFrom, 'MMM dd') + '.<br><br>The data is being fetched from the server and will be used to update the cached sales data on your device.<br><br>Please wait a moment...')

                    if ($.format.date(dateFrom, 'yyyy-MM-dd') == $.format.date(new Date(dateTo), 'yyyy-MM-dd')) {
                        vDateFrom = new Date($.format.date(vDateFrom, 'yyyy-MM-dd') + ' ' + timeFrom + ':00');
                        vDateTo = new Date($.format.date(dateTo, 'yyyy-MM-dd') + ' ' + timeTo + ':59');
                    } else {
                        if ($.format.date(vDateFrom, 'yyyy-MM-dd') == $.format.date(new Date(dateTo), 'yyyy-MM-dd')) {
                            vDateFrom = new Date($.format.date(vDateFrom, 'yyyy-MM-dd') + ' 00:00:00');
                            vDateTo = new Date($.format.date(new Date(dateTo), 'yyyy-MM-dd') + ' ' + timeTo + ':59');
                        } else {
                            vDateFrom = $.format.date(new Date(vDateFrom), 'yyyy-MM-dd') + ' 00:00:00';
                            vDateTo = $.format.date(new Date(vDateFrom), 'yyyy-MM-dd') + ' 23:59:59';
                        }                        
                    }

                    var pData = {
                        OrgID: sessionStorage.OrgID,
                        FOWSTID: selCacheDBStore,
                        DeviceID: Web.DeviceID(),
                        From: $.format.date(new Date(vDateFrom), 'yyyy-MM-dd hh:mm:ss a'),
                        To: $.format.date(new Date(vDateTo), 'yyyy-MM-dd hh:mm:ss a'),
                        RangeFrom: $.format.date(new Date(vDateFrom), 'yyyy-MM-dd hh:mm:ss a'),
                        RangeTo: $.format.date(new Date(vDateTo), 'yyyy-MM-dd hh:mm:ss a'),
                        RangeType: 'Day',
                        RangeYear: 'Day',
                        UserID: $('#selCacheDBCashierName').val(),
                        GenerateZeroSales: false,
                        DownloadDataOnly: true,
                        timeout: 600000, //10 minutes
                        ForceNONVAT: true,
                        GetFromRawData: true
                    }

                    AjaxSend({
                        url: MergeUrlParameters(pData, ($('#selCacheDBReportType').val() == 'zread' ? '/Utilities/DownloadZRead' : '/ReportsPOS/getCashierSalesTransactionSummary')),
                        data: pData,
                        success: function (pDT) {
                            console.log(pDT)
                            vDateFrom = new Date(vDateFrom);

                            if (CStr2(pDT.Error) != '') {
                                Android.AddLog('Retrieving result: ' + pDT.Error);
                                NewMessageBox('There was an error retrieving your data. Please contact a Mobiz representative for assistance.', 'Error');
                                return;
                            }
                            ctr += 1;

                            if ($('#selCacheDBReportType').val() == 'xread') {
                                pDT.Data.Rows[0].CurTotalDiscount = CDec2(pDT.Data.Rows[0].CurSalesDiscount) + CDec2(pDT.Data.Rows[0].CurSCDiscount);
                                pDT.Data.Rows[0].CurTotalVAT = CDec2(pDT.Data.Rows[0].CurTotalVatAmountPortionOfSCDTransaction) + CDec2(pDT.Data.Rows[0].CurTotalVATAmount) - CDec2(pDT.Data.Rows[0].CurTotalVATAmountSalesReturn);
                                pDT.Data.Rows[0].CurTotalVATAmount = CDec2(pDT.Data.Rows[0].CurTotalVAT).toFixed(2)
                            } else {
                                pDT.Data.Rows[0].CurTotalVAT = CDec2(pDT.Data.Rows[0].VATAmount).toFixed(2)
                                pDT.Data.Rows[0].CurTotalVATAmount = CDec2(pDT.Data.Rows[0].VATAmount).toFixed(2)
                            }

                            pDT.Data.Rows[0].CurTotalVATSales = CDec2(pDT.Data.Rows[0].CurTotalVatablePortionOfSCDTransaction) + CDec2(pDT.Data.Rows[0].CurTotalVATSales) - CDec2(pDT.Data.Rows[0].CurTotalVATSalesSalesReturn);

                            UpdateCacheDB(pDT);
                            Android.AddLog('Retrieving result: DONE')

                            if ($.format.date(vDateFrom, 'yyyy-MM-dd') == $.format.date(new Date(dateTo), 'yyyy-MM-dd')) {
                                Bs.Loading('hide');
                                //$('#panUpdateCacheDB').modal('hide');
                                NewMessageBox('Successfully updated your cached sales data for the period of ' + $.format.date(new Date(dateFrom), 'MMM dd') + ' to ' + $.format.date(new Date(dateTo), 'MMM dd') + '.');
                            } else {
                                Recompute(new Date(vDateFrom.setDate(new Date(vDateFrom.getDate() + 1))), new Date(vDateFrom.setDate(new Date(vDateFrom.getDate() + 1))))
                            }

                            return
                            AjaxSend({
                                url: '/SalesReturns/GetInvoiceDiscount',
                                data: {
                                    DeviceID: Web.DeviceID(),
                                    FowstID: sessionStorage.FowstID,
                                    SalesDateFrom: $.format.date(new Date(vDateFrom), 'yyyy-MM-dd'),
                                    SalesDateTo: $.format.date(new Date(dateTo), 'yyyy-MM-dd')
                                }, success: function (invdisc) {
                                    Bs.Loading('hide')
                                    regenZReadData = false;
                                    vDataOtherPaymentType = pDT.OtherPayments;
                                    DisplayHeader(pDT.Header, vIsView);

                                    if (pDT.Data.Rows.length == 0) {
                                        //UpdateZReadCache(dateFrom, Web.DeviceID(), sessionStorage.FowstID)
                                        //PrintSalesZReport(vIsView);

                                        if (Business.ZRead.GenerateZeroSalesData(dateFrom, sessionStorage.FowstID, Web.DeviceID())) {
                                            PrintSalesZReport(vIsView)
                                        } else {
                                            $('#divZReportNoData').show();
                                            $('#divZReportView, #divZReadPrintButton').hide();
                                            return;
                                        }
                                    } else {
                                        $('#divZReadPrintButton').show();

                                        pDT.Data.Rows[0].RefundedSCDiscount = pDT.Data.Rows[0].DiscountRefund;
                                        pDT.Data.Rows[0].LastInvoiceNo = pDT.Data.Rows[0].PrevEndInvoiceNo;
                                        pDT.Data.Rows[0].PresentationCurrGrossSales = pDT.Data.Rows[0].CurGrossSales;
                                        pDT.Data.Rows[0].CurSCDiscount = pDT.Data.Rows[0].TotalSCDiscount;
                                        pDT.Data.Rows[0].ServiceCharge = pDT.Data.Rows[0].CurServiceCharge;
                                        pDT.Data.Rows[0].OtherTypeAmount = pDT.Data.Rows[0].TotalOtherPayments;
                                        pDT.Data.Rows[0].PresentationPrevGrossSales = pDT.Data.Rows[0].PrevGrossSales;
                                        pDT.Data.Rows[0].PrevSCDiscount = pDT.Data.Rows[0].PrevTotalSCDiscount;
                                        pDT.Data.Rows[0].SalesDate = pDT.Data.Rows[0].TransDate;
                                        pDT.Data.Rows[0].CheckPaymentAmount = pDT.Data.Rows[0].CheckPaymentAmount + pDT.Data.Rows[0].CurPotatoCornerGC;
                                        pDT.Header.Rows[0].PrintedBy = sessionStorage.CurrentUserFullName;
                                        pDT.Header.Rows[0].PrintDateTime = new Date();
                                        pDT = $.extend(pDT, { ReportPOS: { Rows: pDT.Data.Rows } })
                                        onlinezreaddata = pDT;
                                        DisplaySalesZReportSummary(pDT.Header, pDT, vIsView);
                                        discountCols = pDT.Discounts.Rows;
                                        DisplayDiscountsBreakdown(discountCols);
                                        LastTotalDataMSR = [] //empty by default, no need na ni since sa online na nagakuha og data                                                
                                    }
                                }
                            })
                        }, error: function () {
                            Android.AddLog('Error in getting sales date for ' + $.format.date(new Date(vDateFrom), 'MMM dd, yyyy'));
                            NewMessageBox('There was a problem retrieving your sales data for ' + $.format.date(new Date(vDateFrom), 'MMM dd, yyyy') + '. Would you like to retry?', '', [
                                {
                                    Text: 'Yes', Click: function () {
                                        $('#txtCacheDBDateFrom').val($.format.date(new Date(vDateFrom), 'yyyy-MM-dd'));
                                        Recompute(vDateFrom, vDateTo)
                                    }
                                }, {
                                    Text: 'No, maybe later', Click: function () {

                                    }
                                }
                            ])
                        }
                    })
                }

                Recompute(new Date(dateFrom), new Date(dateTo))
            });

            $('#popTroubleshootingToolsReportAnIssue').click(function (e) {
                e.preventDefault();

                proj.PromptTestInternet(
                    function () {
                        require(['promise!/Scripts/sources/Tickets/ReportAnIssue.js?v=' + assetversion], function () {
                            $('#pageReportAnIssue').pageshow();
                            ReportAnIssue.controller.ClearFields()
                        })
                    }
                );

            });

            $('#popTroubleshootingToolsUpdateDatabase').click(function (e) {
                e.preventDefault();

                proj.PromptTestInternet(
                    function () {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        SyncDataPrompt(mDatabaseCreationStart);
                    }
                );
            });

            $('#popTroubleshootingToolsLazyUpdate').click(function (e) {
                //e.preventDefault();

                $('#panLazySync').modal('show');

                //proj.PromptTestInternet(
                //    function() {
                //        //$.mobile.changePage('#pagePopUpLazyUpdate');
                //        $('#panLazySync').popup('open');
                //    }
                //);

            });

            $('#btnRestoreSystemBack, #btnReportAnIssueBack').click(function () {
                $('#pagePopUpMenuTroubleShootingTools').pageshow();
            })

            $('#popTroubleshootingToolsRestoreDatabase').click(function (e) {
                e.preventDefault();

                $('#pagePopUpRestoreDatabase').pageshow();

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
                $('#pagePopUpMenuTroubleShootingTools').pageshow();
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

                $('#panLazySync').modal('show');
            });

            $('#btnLazyUpdateSyncNow').click(function () {
                $('#panLazySync').modal('hide');

                try {
                    NewMessageBox('Checking for new updates. Please Wait.')
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
                            NewMessageBox('Flushing Data to Server. Please wait...')
                            mInterval = setInterval(FlushDeviceData, 1000);
                        }
                    }
                );
            });

            function FlushDeviceData() {

                clearInterval(mInterval);
                //Bs.Loading('show', "Flushing Data to Server");

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

                Web.HandleOrganization(true);

                //Sql.AttachDatabase(sessionStorage.OrgID);
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
                    Bs.Loading('hide');
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
                                mPrePos.AutoLoadDefaultPosView = true;
                                mPrePos.UrlParams = $.param({
                                    OrgID: sessionStorage["OrgID"],
                                    mode: 'pos',
                                    rsd: 1,
                                    IsMobile: IsMobile ? 1 : 0
                                });
                                mPrePos.GetUser();
                            }
                        });
                    }
                );

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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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

                $('#pageHome').pageshow();

            });

            $('#btnCashHandlingBack').click(function (e) {
                e.preventDefault();

                $('#pageHome').pageshow();

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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                                        {
                                                            Text: 'Ok'
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
                                                        {
                                                            Text: 'Ok'
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
                                                        {
                                                            Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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

            $('#popERPSettings').click(function () {
                location = '/Subscription/Erp';
            })

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
            $('#btnUserManual').click(function () {
                proj.PromptTestInternet({
                    context: this,
                    method: function () {
                        location = MergeUrlParameters({
                            OrgID: (Android.DebugMode() ? sessionStorage.OrgID : '84e972ed-f208-4867-9506-5b8f9572562f') //84e972ed-f208-4867-9506-5b8f9572562f - Mobiz Org ni sa Azure, diri dapat magconnect ang KB sa tanan mobiz app
                        }, (Android.DebugMode() ? Android.GetServer() : '/') + 'Knowledgebase/AllTopics')
                    },
                    Message: 'Internet connection is required. Please try again later. Thank you!'
                });                
            })

            $('#btnMessages').click(function () {
                $('#pageMessages, #divmessagesloading').pageshow();
                ShowPeopleMsgList();
            })           

            $('#btnPeopleMsgT2BackToHome').click(function (e) {
                e.preventDefault();

                $('#pageHome').pageshow();

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
                                    {
                                        Text: 'Ok'
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

            $('#selPeopleOrg').change(function () {
                var data = $(this).find('option:selected').data('data');
                sessionStorage["OrgName"] = data.CompanyName;
                sessionStorage["OrgID"] = data.OrganizationID;

                if (OfflineMode) {
                    UserDeviceDetails();

                    if (data.DbInfo != null) {
                        pDate = ParseDate(data.DbInfo.DateStarted);
                        if (pDate != null) {
                            mDatabaseCreationStart = FormatDate(pDate, { Format: 'yyyy/MM/dd HH:mm:ss' });
                            pDate = $.format.date(pDate, proj.DateFormat);
                            $('#lblDBStart').text(pDate);
                        }

                        pDate = ParseDate(data.DbInfo.DateEnded);
                        if (pDate != null) {
                            pDate = $.format.date(pDate, proj.DateFormat);
                            $('#lblDBEnd').text(pDate);
                        }
                    }
                    if (CStr2(data.LastSync).trim() != '') {
                        pDate = ParseDate(data.LastSync);
                        pDate = $.format.date(pDate, proj.DateFormat);
                        $('#lblDBSync').text(pDate);
                    } else
                        $('#lblDBSync').text("not set");
                } else {
                    //if (CBool(data.IsFranchisee)) {
                    //    $('#btnFranchiseeUpdate').show();
                    //} else {
                    //    $('#btnFranchiseeUpdate').hide()
                    //}
                }
            })

            $('#btnPOS').click(function () {
                sessionStorage.removeItem('IsHotButtonVisited');
                sessionStorage.IsInventoryCountMode = false;

                if (window.IsMobile) {
                    if (!IsDatabaseReady()) {
                        return;
                    }
                }

                //window.pageSecuritySourcePage = $.mobile.activePage;
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
                        } else {
                            sessionStorage.IsEnableDiningMode = 'false';
                            // window.location = '/SalesMenu?' + pParam;

                        }

                        mPrePos.CheckIfNoUnflushedDataFromPrevDays({
                            success: function () {
                                mPrePos.AutoLoadDefaultPosView = true;
                                mPrePos.UrlParams = pParam;
                                mPrePos.GetUser();
                            }
                        })                        
                    }
                });
            })

            $('#btnPriceChecker').click(function () {
                require(['!promise!/Scripts3/controller/PriceChecker.js'], function () {
                    if (mPriceChecker == null)
                        mPriceChecker = new App.controller.PriceChecker();
                    mPriceChecker.popup();
                });
            });

            $('#btnPO').click(function () {
                PromptUserAccess('P.O. (Purchase Order)', {
                    OnClickOKSuccess: function () {

                        // MergeUrlParameters
                        pParam = {
                            mode: 'po',
                            'OrgID': sessionStorage.OrgID
                        };
                        if (IsMobile)
                            pParam.IsMobile = 1;
                        pParam = $.param(pParam);

                        // bookmark: 2015_12_28__16_01_57_6555: new database naming convention
                        var pFilename;
                        pFilename = sessionStorage.OrgID;
                        if (window.NewDatabaseNaming)
                            pFilename += '.private';
                        if (OfflineMode && Sql.IsExist(pFilename) == 0) {
                            window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                        } else
                            window.location = '/SalesMenu?' + pParam;
                    }
                });
            });

            $('#btnPickProducts').click(function () {
                pParam = {
                    'OrgID': sessionStorage.OrgID
                };
                if (IsMobile)
                    pParam.IsMobile = 1;
                pParam = $.param(pParam);
                window.location = '/PickProducts?' + pParam;
            });

            $('#btnWarehouseReleasingTracker').click(function () {
                pParam = {
                    'OrgID': sessionStorage.OrgID,
                    'mode': 'wrt'
                };
                if (IsMobile)
                    pParam.IsMobile = 1;
                pParam = $.param(pParam);
                window.location = '/WarehouseReleasingTracker?' + pParam;
            });

            $('#btnInvoicePickedItems').click(function () {
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
            });

            $('#btnSalesReturns').click(function () {
                UseOfflineSalesReturn();
                return;
                if (CBool(localStorage.AlwaysUseOfflineSalesReturn))
                    UseOfflineSalesReturn();
                else
                    proj.PromptTestInternet({
                        
                        // commented temporarily 2021_12_15__08_46_54_2712
                        AllowOffline: function () {
                            UseOfflineSalesReturn();
                        },
                        // commented temporarily 2021_12_15__08_46_54_2712
                        
                        method: function () {
                            window.pageSecuritySourcePage = $.mobile.activePage;
                            PromptUserAccess('Sales Return', {
                                OnClickOKSuccess: function () {
                                    if (SalesPaymentsExport_offline().HasUnflushedRecords({ SrvOnly: false })) { // SrvOnly should be true. but false due to fbDZ2AMKSP6F!
                                        NewMessageBox('This device has unflushed records. We will revert to offline mode for creation of SRVs as an internal control measure.<br /><br />' +
                                            'SRVs that are created offline and are not flushed to the server will be unavailable for retrieval from the server.<br /><br />' +
                                            'To make the offline-generated SRVs available for retrieval online, please flush SRVs by tapping on the data flushing function represented by the unflushed data notification bar at the bottom of the app page.', 'Unflushed Data',
                                            [
                                                {
                                                    Text: 'OK'
                                                },
                                                {
                                                    Text: 'Use offline data',
                                                    Click: function() {
                                                        UseOfflineSalesReturn();
                                                    }
                                                }
                                            ]);
                                        return;
                                    }
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/SalesReturns' + (CBool(IsMobile) ? '/BsIndex' : ''));
                                }
                            });
                        }
                    }); // END proj.PromptTestInternet

                function UseOfflineSalesReturn() {
                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('Sales Return', {
                        OnClickOKSuccess: function () {
                            // window.open(GetPathFormat("/SalesReturns") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0,
                                UseOfflineData: 1
                            }, '/SalesReturns' + (CBool(IsMobile) ? '/BsIndex' : ''));
                        }
                    });
                } // END function UseOfflineSalesReturn() {
            });

            $('#btnStockReceiving').click(function () {
                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Receiving and Transfers', {
                    OnClickOKSuccess: function () {
                        location = MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            IsReceiving: 1,
                            IsMobile: CBool(IsMobile) ? 1 : 0
                        }, '/Receiving' + (CBool(IsMobile) ? '/BsIndex' : ''));
                    }
                });
            });

            $('#btnErpInventoryHandling').click(function() {
                location.href = '/InventoryCount/InventoryHandling?OrgID=' + sessionStorage.OrgID;
            });

            $('#btnStockTransfers').click(function () {
                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Receiving and Transfers', {
                    OnClickOKSuccess: function () {
                        location = MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            IsReceiving: 0,
                            IsMobile: CBool(IsMobile) ? 1 : 0
                        }, '/Receiving' + (CBool(IsMobile) ? '/BsIndex' : ''));
                    }
                });
            });

            $('#btnInventoryCount').click(function () {
                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Inventory Count', {
                    OnClickOKSuccess: function () {
                        proj.CheckIfThereAreDraftStockRecTrans(function () {
                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/InventoryCount' + (CBool(IsMobile) ? '/BsIndex' : ''));
                        }, 'inventory')                        

                        //window.open(GetPathFormat("/InventoryCount") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    }
                });
            });

            $('#btnExpenseTracking').click(function () {
                window.pageSecuritySourcePage = $.mobile.activePage;
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/Expenses');
            });

            $('#btnTickets').click(function () {
                window.pageSecuritySourcePage = $.mobile.activePage;
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/Ticket');
            });

            $('#btnProductMaker').click(function () {
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/BillOfMaterials');
            });

            $('#btnShelfBinLabelling').click(function () {
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/ShelfAndBinLabeling' + (CBool(IsMobile) ? '/BsIndex' : ''));
            });

            $('#btnRequestApprovals').click(function () {
                sessionStorage.IsRequestAndApproval = 1;
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID
                }, '/DashBoardsAndReports');
            });

            $('#btnCashHandling').click(function () {
                PopupButtonList('', 'Cash Handling Options', [
                    {
                        Text: 'Deposit / Turn-over Monies',
                        Click: function () {
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
                                            },
                                            OfflineMenu: function(){
                                                sessionStorage.PrintingOfflineData = true;
                                                GotoPage()
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
                        }
                    }, {
                        Text: 'Petty Cash Fund (TBI)',
                        Click: function () {
                            NewMessageBox('This feature will be implemented soon');
                        }
                    }
                ])
            });

            $('#btnDashboardAndReports').click(function () {
                var pOfflineModeFunction = function () {
                    //Android.Toast('Cannot Connect to Server');
                    //sessionStorage.PrintingOfflineData = true;
                    //GotoPage();
                    PopupButtonList('', 'Offline Reports', [
                        {
                            Text: 'Cashiers Sales Report',
                            Click: function () {
                                sessionStorage.PrintingOfflineData = true;
                                sessionStorage.PrintingOfflineXReadData = true;
                                sessionStorage.OfflineType = 'cashier';
                                GotoPage();
                            }
                        },
                        //{
                        //    Text: 'Managers Sales Report',
                        //    Click: function () {
                        //        sessionStorage.PrintingOfflineData = true;
                        //        sessionStorage.OfflineType = 'managers';
                        //        GotoPage();
                        //    }
                        //},
                        //{
                        //    Text: 'Monies Accountability',
                        //    Click: function () {
                        //        sessionStorage.PrintingOfflineData = true;
                        //        sessionStorage.OfflineType = 'monies';
                        //        GotoPage();
                        //    }
                        //},
                        //{
                        //    Text: 'Inventories Accountability',
                        //    Click: function () {
                        //        sessionStorage.PrintingOfflineData = true;
                        //        sessionStorage.OfflineType = 'inventories';
                        //        GotoPage();
                        //    }
                        //},
                        //{
                        //    Text: 'Transaction Details',
                        //    Click: function () {
                        //        sessionStorage.PrintingOfflineData = true;
                        //        sessionStorage.OfflineType = 'transaction';
                        //        GotoPage();
                        //    }
                        //},
                        //{
                        //    Text: 'Inventory Ledger',
                        //    Click: function () {
                        //        sessionStorage.PrintingOfflineData = true;
                        //        sessionStorage.OfflineType = 'inventoryledger';
                        //        GotoPage();
                        //    }
                        //},
                        //{
                        //    Text: 'Inventory Count History',
                        //    Click: function () {
                        //        sessionStorage.PrintingOfflineData = true;
                        //        sessionStorage.OfflineType = 'inventorycounthistory';
                        //        GotoPage();
                        //    }
                        //},
                        {
                            Text: 'Z-Read',
                            Click: function () {
                                proj.CheckIfThereAreDraftStockRecTrans(function () {
                                    proj.CheckIfInventoryIsDone(function () {
                                        sessionStorage.PrintingOfflineData = true;
                                        sessionStorage.PrintingOfflineZReadData = true;
                                        sessionStorage.OfflineType = 'zreport';
                                        GotoPage();
                                    }, 'dashboard')
                                }, 'dashboard')                                
                            }
                        },
                        //{
                        //    Text: 'SM COIN Report',
                        //    Click: function () {
                        //        sessionStorage.PrintingOfflineData = true;
                        //        sessionStorage.OfflineType = 'smcoinreport';
                        //        GotoPage();
                        //    }
                        //},
                        //{
                        //    Text: 'Sales by SKU Report',
                        //    Click: function () {
                        //        sessionStorage.PrintingOfflineData = true;
                        //        sessionStorage.OfflineType = 'salesbyskureport';
                        //        GotoPage();
                        //    }
                        //},
                        //{
                        //    Text: 'Tenant Monitoring Reports',
                        //    Click: function () {
                        //        sessionStorage.PrintingOfflineData = true;
                        //        sessionStorage.OfflineType = 'tmr';
                        //        GotoPage();
                        //    }
                        //},
                        // ready na ni. 2023_12_04__14_32_32_0473
                        //{
                        //    Text: 'Receiving Report',
                        //    Click: function () {
                        //        sessionStorage.PrintingOfflineData = true;
                        //        sessionStorage.OfflineType = 'receivingreport';
                        //        GotoPage();
                        //    }
                        //},
                        {
                            Text: 'Cancel',
                            Click: function () {
                                window.location = '/People/PeopleMenu';
                            }
                        }
                    ]);
                };

                if (CBool(localStorage.ForceOfflineReports))
                    pOfflineModeFunction();
                else
                    proj.PromptTestInternet({
                        context: this,
                        method: function () {
                            sessionStorage.PrintingOfflineData = false;
                            if (OfflineMode) {
                                GotoPage();
                                SalesPaymentsExport_offline().HandleUnflushedRecords({
                                    Success: function () {
                                        //GotoPage();
                                    },
                                    OfflineMenu: pOfflineModeFunction
                                });
                            }                            
                            else
                                GotoPage();
                        },
                        error: pOfflineModeFunction
                    }); // END proj.PromptTestInternet(

                function GotoPage() {
                    sessionStorage.system = 'mcr';
                    sessionStorage.removeItem('IsRequestAndApproval');
                    location = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        IsMobile: CBool(IsMobile) ? 1 : 0
                    }, '/DashBoardsAndReports' + (CBool(IsMobile) ? '/BsIndex' : ''));                    
                }
            });

            $('#btnTroubleshooting').click(function () {
                $('#pagePopUpMenuTroubleShootingTools').pageshow();
            });

            $('#btnSystemAdmin').click(function () { 
                proj.PromptTestInternet({
                    context: this,
                    method: function () {
                        NewPromptUserAccess('System Settings', {
                            Message: 'You don\'t have access to <b>System Settings</b>.',
                            OnClickOKSuccess: function () {
                                location = MergeUrlParameters({
                                    OrgID: sessionStorage.OrgID,
                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                }, '/SetupChecklist/SystemAdministration');
                            }
                        })                        
                    }
                });
            });

            $('#btnFranchiseeUpdate').click(function () {
                PromptUserAccess('Allow to sync data from franchisor database', {
                    Message: 'Sorry! You\'re not allowed to use this feature.',
                    OnClickOKSuccess: function () {
                        AjaxSend({
                            url: 'Franchisee/GetFranchiseeLastUpdate',
                            success: function (e) {
                                var vRows = e.Results.Rows;
                                var msg = '';

                                if (vRows.length > 0) {
                                    msg = '<div  class="alert alert-info mb-3">Last update on ' + $.format.date(new Date(vRows[0].LastUpdated), 'MMM dd, yyyy hh:mm a') + ' by ' + vRows[0].UpdatedByName + '</div>';
                                }

                                NewMessageBox(msg + 'You\'re about to get the latest update from your franchisor. The data are as follows: <br><br>' +
                                    '<ol>' +
                                    '<li>Franchisor\'s SKUs</li>' +
                                    '<li>SKU Prices</li>' +
                                    '<li>Item Modifiers</li>' +
                                    '<li>Bill of Materials (BOM)</li>' +
                                    '<li>Hot Button Templates</li>' +
                                    '<li>Cashier\'s Access Rights</li>' +
                                    '</ol>' +
                                    'Are you sure you want to continue?', 'Confirmation Dialog', [
                                    {
                                        Text: 'Yes', Click: function () {
                                            Bs.Loading('show', 'Getting latest update from franchisor, please wait...')
                                            AjaxSend({
                                                url: '/Franchisee/UpdateDB',
                                                success: function (e) {
                                                    Bs.Loading('hide');
                                                    if (e.Results == 'saved') {
                                                        NewMessageBox('Your database has been succesfully updated.');
                                                    } else if (e.Results == 'invalid') {
                                                        NewMessageBox('Sorry! This feature is only for Franchisee.');
                                                    } else if (e.NoAccess) {
                                                        NewMessageBox('Sorry! You\'re not allowed to use this feature.');
                                                    } else {
                                                        NewMessageBox('There was an error while updating your database, please ask assistance from Mobiz reprensentative. Thank you!')
                                                    }
                                                }
                                            })
                                        }
                                    }, { Text: 'No, maybe later.' }
                                ])
                            }
                        })                       
                    }
                });
            });

            $('#btnPrivateMessages').click(function () {
                PromptUserAccess('Allow users to add, edit, or remove announcements', {
                    Message: 'Sorry! You\'re not allowed to use this feature.',
                    OnClickOKSuccess: function () {
                        GetPivateMessages();
                    }
                });
            })

            $('#popupCreateMessage').on('show.bs.modal', function () {
                $(this).find('input, textarea').val('');
            })

            var mIsConnected = null;
            (function PoolOnline() {
                AjaxSend({
                    url: '/Home/GetDate',
                    timeout: 5000,
                    success: function(vData) {
                        setTimeout(PoolOnline, 60 * 1000);
                        mIsConnected = true;
                        $('#btnOnline span').removeClass('bi-cloud-fill');
                        $('#btnOnline span').removeClass('bi-cloud-slash-fill');
                        $('#btnOnline span').addClass('bi-cloud-check-fill');
                    },
                    error: function() {
                        setTimeout(PoolOnline, 60 * 1000);
                        mIsConnected = false;
                        $('#btnOnline span').removeClass('bi-cloud-fill');
                        $('#btnOnline span').removeClass('bi-cloud-check-fill');
                        $('#btnOnline span').addClass('bi-cloud-slash-fill');
                    }
                });
            })();

            $('#btnOnline').click(function() {
                if (mIsConnected == true) 
                    NewMessageBox('App is online');
                else
                    NewMessageBox('App is offline');
            });

            $('#btnMessageSave').click(function () {
                var txtPMID = $.trim($('#txtPMID').val());
                var txtPMTitle = $.trim($('#txtPMTitle').val());
                var txtPMMessage = $.trim($('#txtPMMessage').val());

                if (txtPMTitle == '') {
                    WebToast('Please enter title of your message');
                    return
                }

                if (txtPMMessage == '') {
                    WebToast('Please enter your message');
                    return
                }
                Bs.Loading('show');
                AjaxSend({
                    url: 'People/SavePrivateMessage',
                    data: {
                        MsgID: txtPMID,
                        Title: txtPMTitle,
                        Message: txtPMMessage
                    }, success: function (e) {
                        Bs.Loading('hide');
                        $('#popupCreateMessage').modal('hide');
                        if (e.Results == 'saved') {
                            GetPivateMessages();
                        } else {
                            NewMessageBox(e.Error, 'Error');
                        }
                    }
                })
            })

            $('#btnMsgBack').click(function () {
                $('#colLeft').removeClass('d-none');
                $('#colRight').addClass('d-none');
            })

            $('#btnAnnouncement').click(function () {
                GetAnnouncements();
            })

            function UpdateCacheDB(cache) {
                var invdata = cache.Data.Rows[0];
                var discountCols = [];
                var discountlist = cache.Discounts.Rows;
                var name = '';
                var cacheTable; // TODO: Include 'CacheCSR' when updating the cache database. Since this is per cashier, we have not yet included it.
                var reporttype = $('#selCacheDBReportType').val();

                if (reporttype == 'zread') {
                    cacheTable = ['CacheMSR'];
                } else cacheTable = ['CacheCSR'];

                for (var i in discountlist) {
                    if (discountlist[i].ID.toLowerCase() == '8e2dca9c-433f-42fc-a644-04476dba056a') {
                        name = 'TotalRegularDiscountThree';
                    } else if (discountlist[i].ID.toLowerCase() == 'c2218c86-0450-4cd3-b019-3edd43e51741') {
                        name = 'TotalGPCDiscount';
                    } else if (discountlist[i].ID.toLowerCase() == '34a4032f-a142-4c15-a868-4ac4410434df') {
                        name = 'TotalPWDDiscount';
                    } else if (discountlist[i].ID.toLowerCase() == 'f94329ec-9453-4061-8f72-4b95ef18bf81') {
                        name = 'TotalMaxAllowableSCDiscount';
                    } else if (discountlist[i].ID.toLowerCase() == '764edacb-0131-4033-8e44-7b903368008c') {
                        name = 'TotalRegularDiscountOne';
                    } else if (discountlist[i].ID.toLowerCase() == 'c4199347-6a00-45c9-be11-8934dd63ecd1') {
                        name = 'TotalRegularDiscountTwo';
                    } else if (discountlist[i].ID.toLowerCase() == '674ad791-188b-4238-b95c-97b5c2b9d544') {
                        name = 'TotalSpecialSCDiscount';
                    } else if (discountlist[i].ID.toLowerCase() == '35f954a0-c67c-499c-af60-f08e64477fdc') {
                        name = 'TotalVIPDiscount';
                    } else if (discountlist[i].ID.toLowerCase() == '7804a35a-b531-4380-945e-fc9643e99843') {
                        name = 'TotalEmployeeDiscount';
                    } else if (discountlist[i].ID.toLowerCase() == '055a0a33-dd98-47ec-add3-8b2009f3944a') {
                        name = 'TotalSCDiscount';
                    } else {
                        name = 'Total' + discountlist[i].DiscountType.replace(/\s+/g, '');
                    }

                    discountCols.push(name);
                    discountlist[i].ColumnName = name
                }

                //iadd nalang diri ang mga new columns for sqlite
                discountCols.push('CurVoid')
                discountCols.push('CurVoidItemQty')
                discountCols.push('PrintCount')
                discountCols.push('LastDateOfReprint')

                for (var ct in cacheTable) {
                    var cachemsrColumns = Sql.Execute("PRAGMA cache.table_info(" + cacheTable[ct] +")");

                    for (var i in discountCols) {
                        var isExist = false;
                        for (var m in cachemsrColumns.Rows) {
                            if (cachemsrColumns.Rows[m].name == discountCols[i]) {
                                isExist = true;
                                break;
                            }
                        }

                        if (!isExist) {
                            pSql = " ALTER TABLE cache." + cacheTable[ct] + " " +
                                " ADD [" + discountCols[i] + "] TEXT"
                            Sql.Execute(pSql);
                        }
                    }
                }                

                var otherpayment = cache.OtherPayments.Rows;

                for (var ct in cacheTable) {
                    for (var i = 0; i < otherpayment.length; i++) {
                        var opdata = otherpayment[i];
                        opdata.PaymentType = opdata.PaymentType.replace(/\s+/g, '');

                        var vData = Sql.Execute("PRAGMA cache.table_info(" + cacheTable[ct] + ")");
                        var IsExist = false;

                        for (var m = 0; m <= vData.Rows.length - 1; m++) {
                            if (vData.Rows[m].name.toLowerCase() == CStr2('OtherPayment' + opdata.PaymentType).toLowerCase()) {
                                IsExist = true;
                                break;
                            }
                        }

                        if (!IsExist) {
                            Sql.Execute("ALTER TABLE cache." + cacheTable[ct] + " ADD COLUMN OtherPayment" + opdata.PaymentType + " TEXT");
                        }
                    }
                }                

                var cachedata = {
                    TotalSales: invdata.CurGrossSales,
                    TotalSalesDiscount: invdata.CurTotalDiscount,
                    AdjRefund: invdata.CurSalesReturnRefund,
                    AdjSRV: invdata.CurSalesReturnExchange,
                    Cash: invdata.CashAmount,
                    Check: CDec2(invdata.CheckPaymentAmount) + CDec2(invdata.CurPotatoCornerGC),
                    DebitCard: invdata.DebitCardAmount,
                    CreditCard: invdata.CreditCardAmount,
                    SRV: invdata.SRVAmount,
                    VAT: invdata.CurTotalVATSales,
                    VATAmount: invdata.CurTotalVAT,
                    NonVAT: invdata.NonVAT,
                    VATExempt: invdata.CurVATExemptSales,
                    ZeroRatedVAT: invdata.ZeroRatedVAT,
                    TotalVATSales: invdata.CurTotalVATSales,
                    TotalVATAmount: invdata.CurTotalVATAmount,
                    TotalVATSalesSalesReturn: invdata.CurTotalVATSalesSalesReturn,
                    TotalVATAmountSalesReturn: invdata.CurTotalVATAmountSalesReturn,
                    Charges: invdata.ChargeAmount,
                    TotalNoOfCustomers: invdata.CurTransCountInv,
                    TotalForeGoneVAT: invdata.CurVATExempt,
                    OnCredit: invdata.OnCreditAmount,
                    SalesDate: $.format.date(new Date(invdata.TransDate), 'yyyy-MM-dd'),
                    CashierID: CStr2(invdata.CashierID)
                }

                var totalotherpayment = 0;
                for (var i = 0; i < otherpayment.length; i++) {
                    var opdata = otherpayment[i];
                    opdata.PaymentType = opdata.PaymentType.replace(/\s+/g, '');
                    cachedata['OtherPayment' + opdata.PaymentType] = CDec2(opdata.Amount);
                    totalotherpayment += CDec2(opdata.Amount);
                }

                cachedata.OtherPaymentType = totalotherpayment;

                var invdiscount = cache.Discounts.Rows;

                for (var i in discountlist) {
                    cachedata[discountlist[i].ColumnName] = 0;

                    for (var d in invdiscount) {
                        if (invdiscount[d].DiscountTypeID == discountlist[i].ID) {
                            cachedata[discountlist[i].ColumnName] += CDec2(invdiscount[d].DiscountAmount);
                        }
                    }
                }

                for (var ct in cacheTable) {
                    SalesPayment_offline().SaveToCacheTable(cachedata, cacheTable[ct], otherpayment, discountlist);
                }                

                if (reporttype != 'zread') return;

                pSql = Sql.Execute('PRAGMA table_info(cachezread)').Rows;
                var cols = ['CurGrossSales', 'CurTotalDiscount', 'CurSalesReturnRefund', 'CurSalesReturnExchange', 'CurTotalVATSales', 'CurTotalVAT', 'CurTotalVATSalesSalesReturn', 'CurTotalVATAmountSalesReturn', 'CurLocalTax', 'CurServiceCharge', 'CurNoTAXSales', 'CurVATExemptSales', 'CurZeroRatedVATSales', 'CurOthers', 'CashAmount', 'DebitCardAmount', 'CreditCardAmount', 'CheckPaymentAmount', 'SRVAmount', 'ChargeAmount', 'SMACAmount', 'EPlusAmount', 'VoucherAmount', 'OtherTypeAmount', 'PresentationCurrGrossSales', 'CurSCDiscount', 'CurVATExempt', 'OnCreditAmount', 'CurTotalVatablePortionOfSCDTransaction', 'DiscountRefund', 'CurSCDiscountExempt', 'PrevGrossSales', 'PrevTotalDiscount', 'PrevSCDiscount', 'PrevSalesReturnRefund', 'PrevSalesReturnExchange', 'PrevTotalVAT', 'PrevServiceCharge', 'PrevChargeAmount', 'PrevVATExempt', 'PresentationPrevGrossSales', 'PrevSCDiscountExempt'];
                try {
                    for (var i in pSql) {
                        var colname = pSql[i].name;
                        if (cols.indexOf(colname) > -1) {
                            if (pSql[i].type.toLowerCase() != 'text') {
                                //change datatype to TEXT from number, para dili gina store ang values (in millions) to scientific notation
                                Sql.Execute('ALTER TABLE cachezread RENAME COLUMN ' + colname + ' to ' + colname + '_old');
                                Sql.Execute('ALTER TABLE cachezread ADD COLUMN ' + colname + ' TEXT ');
                                Sql.Execute('UPDATE cachezread SET ' + colname + ' = CAST(' + colname + '_old as TEXT)');
                            }
                        }
                    }
                } catch (error) { // near "COLUMN": syntax error (code 1 SQLITE_ERROR): , while compiling: ALTER TABLE cachezread RENAME COLUMN PrevGrossSales to PrevGrossSales_old
                    var pColumns = {};
                    for (var pColIndex = 0; pColIndex <= cols.length - 1; pColIndex++) {
                        var pCol = cols[pColIndex];
                        pColumns[pCol] = {
                            Name: pCol,
                            MajorDataType: 'Text' // Text = case sensitive
                        };
                    }
                    Sql.CreateTableFromColumns('cachezread', 'cache', pColumns);

                    // test if columns data types were updated
                    pSql = Sql.Execute('PRAGMA table_info(cachezread)').Rows;
                    for (var i in pSql) {
                        var colname = pSql[i].name;
                        if (cols.indexOf(colname) > -1) {
                            if (pSql[i].type.toLowerCase() != 'text') {
                                throw new Error('we should not be here. jsLPY9YTPYDF!');
                            }
                        }
                    }
                }

                var newCacheZreadColumns = ['CurVoid', 'CurVoidItemQty', 'PrevSRVAmount', 'PrevSCDiscount', 'PrevVATExempt', 'PrevSCDiscountExempt', 'CurSCDiscountExempt', 'PrevSoloParentDiscount', 'PrevPhNatTeamDiscount', 'CurSoloParentDiscount', 'CurPhNatTeamDiscount'];
                var cachezreadColumns = Sql.Execute("PRAGMA cache.table_info(CacheZRead)");

                for (var i in newCacheZreadColumns) {
                    var isExist = false;
                    for (var m in cachezreadColumns.Rows) {
                        if (cachezreadColumns.Rows[m].name == newCacheZreadColumns[i]) {
                            isExist = true;
                            break;
                        }
                    }

                    if (!isExist) {
                        pSql = " ALTER TABLE cachezread " +
                            " ADD " + newCacheZreadColumns[i] + " TEXT"
                        Sql.Execute(pSql);
                    }
                }

                var pIU = new InsertUpdate('CacheZRead');

                pIU.Add('BegBal', CDec2(invdata.BegBal));
                pIU.Add('PresentationPrevGrossSales', CDec2(invdata.PrevGrossSales));
                pIU.Add('PrevGrossSales', CDec2(invdata.PrevGrossSales))
                pIU.Add('PrevTotalDiscount', CDec2(invdata.PrevTotalDiscount))
                pIU.Add('PrevSalesReturnRefund', CDec2(invdata.PrevSalesReturnRefund))
                pIU.Add('PrevSalesReturnExchange', CDec2(invdata.PrevSalesReturnExchange))
                pIU.Add('PrevTotalVAT', CDec2(invdata.PrevTotalVAT))
                pIU.Add('PrevServiceCharge', CDec2(invdata.PrevServiceCharge))
                pIU.Add('PrevChargeAmount', CDec2(invdata.PrevChargeAmount))
                pIU.Add('PrevSoloParentDiscount', CDec2(invdata.PrevSoloParentDiscount))
                pIU.Add('PrevPhNatTeamDiscount', CDec2(invdata.PrevPhNatTeamDiscount))
                pIU.Add('PrevSCDiscount', CDec2(invdata.PrevTotalSCDiscount))
                pIU.Add('PrevSCDiscountExempt', CDec2(invdata.PrevSCDiscountExempt))
                pIU.Add('PrevVATExempt', CDec2(invdata.PrevVATExempt))                
                pIU.Add('PrevSRVAmount', CDec2(invdata.PrevSRVAmount))                
                pIU.Add('PresentationCurrGrossSales', CDec2(invdata.CurGrossSales));
                pIU.Add('CurGrossSales', CDec2(invdata.CurGrossSales));
                pIU.Add('CurTotalDiscount', CDec2(invdata.CurTotalDiscount));
                pIU.Add('CurSalesReturnRefund', CDec2(invdata.CurSalesReturnRefund));
                pIU.Add('CurSalesReturnExchange', CDec2(invdata.CurSalesReturnExchange));
                pIU.Add('CurSoloParentDiscount', CDec2(invdata.TotalSoloParentDiscount));
                pIU.Add('CurPhNatTeamDiscount', CDec2(invdata.TotalPhNatTeamDiscount));
                pIU.Add('CurSCDiscount', CDec2(invdata.TotalSCDiscount));                
                pIU.Add('CurSCDiscountExempt', CDec2(invdata.CurSCDiscountExempt));
                pIU.Add('CurTotalVATSales', CDec2(invdata.CurTotalVATSales));
                pIU.Add('CurTotalVATSalesSalesReturn', CDec2(invdata.CurTotalVATSalesSalesReturn));
                pIU.Add('CurTotalVAT', CDec2(invdata.CurTotalVAT));
                pIU.Add('CurTotalVATAmountSalesReturn', CDec2(invdata.CurTotalVATAmountSalesReturn));
                pIU.Add('CurVATExempt', CDec2(invdata.CurVATExempt));
                pIU.Add('CurLocalTax', CDec2(invdata.CurLocalTax));
                pIU.Add('CurServiceCharge', CDec2(invdata.CurServiceCharge));
                pIU.Add('ChargeAmount', CDec2(invdata.ChargeAmount))
                pIU.Add('CurNoTAXSales', CDec2(invdata.CurNoTAXSales));
                pIU.Add('CurVATExemptSales', CDec2(invdata.CurVATExemptSales));
                pIU.Add('CurZeroRatedVATSales', CDec2(invdata.ZeroRatedVAT));
                pIU.Add('CurOthers', CDec2(invdata.CurOthers));
                pIU.Add('CashAmount', CDec2(invdata.CashAmount));
                pIU.Add('DebitCardAmount', CDec2(invdata.DebitCardAmount));
                pIU.Add('CreditCardAmount', CDec2(invdata.CreditCardAmount));
                pIU.Add('SMACAmount', CDec2(invdata.SMACAmount));
                pIU.Add('CheckPaymentAmount', CDec2(invdata.CheckPaymentAmount) + CDec2(invdata.CurPotatoCornerGC));
                pIU.Add('OtherTypeAmount', CDec2(invdata.OtherPaymentAmount));
                pIU.Add('OnCreditAmount', CDec2(invdata.OnCreditAmount));
                pIU.Add('EPlusAmount', CDec2(invdata.EPlusAmount));
                pIU.Add('VoucherAmount', CDec2(invdata.VoucherAmount));
                pIU.Add('SRVAmount', CDec2(invdata.SRVAmount));
                pIU.Add('CurBegInvoiceNo', CDec2(invdata.CurBegInvoiceNo));
                pIU.Add('CurEndInvoiceNo', CDec2(invdata.CurEndInvoiceNo));

                pIU.Add('CurTransInvItemCount', CDec2(invdata.CurTransInvItemCount));
                pIU.Add('CurTransCountMemoReturnExchange', invdata.CurTransCountMemoReturnExchange);
                pIU.Add('CurSalesReturnExchangeItemCount', invdata.CurSalesReturnExchangeItemCount);
                pIU.Add('CurTransCountMemoRefund', invdata.CurTransCountMemoRefund);
                pIU.Add('CurSalesRefundItemCount', invdata.CurSalesRefundItemCount);
                pIU.Add('CurTransCountInv', invdata.CurTransCountInv);
                pIU.Add('CurTransInvItemCount', invdata.CurTransInvItemCount);

                if (CDec2(invdata.CurVoid) > 0) {
                    pIU.Add('CurVoid', ReadyString(invdata.CurVoid));
                }

                if (CDec2(invdata.CurVoidItemQty) > 0) {
                    pIU.Add('CurVoidItemQty', ReadyString(invdata.CurVoidItemQty));
                }

                pSql = pIU.GetUpdate(' FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                    ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                    ' AND SalesDate = ' + ReadyString($.format.date(new Date(invdata.TransDate), 'yyyy-MM-dd')));

                if (CDec2(Sql.ExecuteSql(pSql)) == 0) {
                    pIU.Add("ID", ReadyString(NewID()));
                    pIU.Add('SalesDate', ReadyString($.format.date(new Date(invdata.TransDate), 'yyyy-MM-dd')));
                    pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
                    pIU.Add('FOWSTID', ReadyString(sessionStorage.FowstID));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql)
                }
            }

            function GetAnnouncements() {
                Bs.Loading('show');
                AjaxSend({
                    url: 'People/GetAnnouncements',
                    success: function (e) {
                        Bs.Loading('hide');
                        var latest = e.Latest.Rows;
                        var others = e.Others.Rows;
                        var divLatestAnnouncements = $('#divLatestAnnouncements').empty();
                        var divOtherAnnouncements = $('#divOtherAnnouncements').empty();

                        if (latest.length > 0) {
                            for (var i in latest) {
                                var data = latest[i];
                                divLatestAnnouncements.append($('<div class="card mb-3">')
                                    .append($('<div class="card-body">')
                                        .append($('<h5 class="card-title">').text(data.Title))
                                        .append($('<p class="card-text">').html(data.Message.replace(/(?:\r\n|\r|\n)/g, '<br>')))
                                    )
                                    .append($('<div class="card-footer">')
                                        .append($('<small class="text-muted">').text($.format.date(new Date(data.CreatedOn), 'MMM dd, yyyy hh:mm a')))
                                    )
                                )
                            }
                        } else {
                            divLatestAnnouncements.append($('<div class="text-center text-dark">').text('No latest announcements'))
                        }

                        if (others.length > 0) {
                            for (var i in others) {
                                var data = others[i];
                                divOtherAnnouncements
                                    .append($('<div class="accordion mb-3">')
                                        .append($('<div class="card">')
                                            .append($('<div class="card-header p-0">')
                                                .append($('<button class="btn btn-light btn-block text-left p-3 rounded-0 font-weight-bold collapsed" data-role="none" data-toggle="collapse" data-target="#collapse-' + data.ID + '">')
                                                    .append(data.Title)
                                                )
                                            )
                                            .append($('<div class="collapse" id="collapse-' + data.ID + '">')
                                                .append($('<div class="p-3">').html(data.Message.replace(/(?:\r\n|\r|\n)/g, '<br>')))
                                                .append($('<div class="card-footer">')
                                                    .append($('<small class="text-muted">').text($.format.date(new Date(data.CreatedOn), 'MMM dd, yyyy hh:mm a')))
                                                )
                                            )                                            
                                        )
                                    )
                                //    .append($('<div class="card mb-3">')
                                //    .append($('<div class="card-body">')
                                //        .append($('<h5 class="card-title">').text(data.Title))
                                //        .append($('<p class="card-text">').html(data.Message.replace(/(?:\r\n|\r|\n)/g, '<br>')))
                                //    )
                                //    .append($('<div class="card-footer">')
                                //        .append($('<small class="text-muted">').text($.format.date(new Date(data.CreatedOn), 'MMM dd, yyyy hh:mm a')))
                                //    )
                                //)
                            }
                        } else {
                            divOtherAnnouncements.append($('<div class="text-center text-dark">').text('No previous announcements'))
                        }

                        $('#popupAnnouncement').modal('show');
                    },
                    error: function () {
                        Bs.Loading('hide');
                    }
                })
            }

            function GetPivateMessages() {
                Bs.Loading('show');
                $('#lblPMTTitle, #lblPMDateCreated').text('');
                $('#lblPMMessageContent').removeClass('rounded border').empty();
                $('#divMsgOption').hide();
                AjaxSend({
                    url: 'People/GetPrivateMessage',
                    success: function (e) {
                        Bs.Loading('hide');
                        var vRows = e.Results.Rows;
                        var divMessages = $('#divMessages').empty();

                        if (vRows.length > 0) {
                            for (var i in vRows) {
                                var data = vRows[i];
                                divMessages.append($('<div class="w-100 p-2 border-bottom messages">')
                                    .append($('<div class="row g-0 align-items-center mb-2">')
                                        .append($('<div class="col">').html('<h6>' + data.Title + '</h6>'))
                                        .append($('<div class="col-auto">').text($.format.date(new Date(data.CreatedOn), 'MM-dd-yyyy HH:mm')))
                                    )
                                    .append($('<div class="text-truncate">').text(data.Message))
                                    .data('data', data)
                                    .click(function (e) {
                                        e.preventDefault();
                                        e.stopPropagation();
                                        var data = $(this).data('data');
                                        $('.messages').removeClass('active');

                                        $('#lblPMTTitle').text(data.Title);
                                        $('#lblPMDateCreated').text($.format.date(new Date(data.CreatedOn), 'MM-dd-yyyy HH:mm'));
                                        $('#lblPMMessageContent').addClass('rounded border').html(data.Message.replace(/(?:\r\n|\r|\n)/g, '<br>'));
                                        $('#divMsgOption').show();

                                        if ($('#colRight').css('display') == 'none') {
                                            $('#colLeft').addClass('d-none');
                                            $('#colRight').removeClass('d-none');
                                        } else {
                                            $(this).addClass('active');
                                        }

                                        $('#btnMsgEdit').unbind('click').click(function () {
                                            $('#popupCreateMessage').modal('show');
                                            $('#txtPMID').val(data.ID);
                                            $('#txtPMTitle').val(data.Title);
                                            $('#txtPMMessage').val(data.Message);
                                        })

                                        $('#btnMsgDelete').unbind('click').click(function () {
                                            NewMessageBox('Are you sure you want to remove this announcement? <br><br> If Yes, it cannot be undone.', 'Confirmation Dialog', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        Bs.Loading('show');
                                                        AjaxSend({
                                                            url: 'People/RemovePrivateMessage',
                                                            data: { MsgID: data.ID },
                                                            success: function (e) {
                                                                Bs.Loading('hide');

                                                                if (e.Results == 'removed') {
                                                                    GetPivateMessages();
                                                                    WebToast('Successfully removed');
                                                                } else {
                                                                    NewMessageBox(e.Error);
                                                                }
                                                            }
                                                        })
                                                    }
                                                }, { Text: 'No' }
                                            ])
                                        })
                                    })
                                )
                            }
                        }

                        $('#popupPrivateMessage').modal('show');
                    },
                    error: function () {
                        Bs.Loading('hide');
                    }
                })
            }

            var htmlTickets = '';
            $('#btnIssueTickets').click(function () {
                Bs.Loading('show');

                function Go() {
                    //$('#popupIssueTickets').unbind('shown.bs.modal').on('shown.bs.modal', function () {
                    //    Tickets.GetClientTicketSummary();
                    //}).modal('show')
                    sessionStorage.fromServiceTicket = true;
                    sessionStorage.ClientInfo = JSON.stringify({ ID: sessionStorage.OrgID, TicketsFromMCR: 1, ViewTicketFromMobiz: 1 });
                    Tickets.GetTicket();
                    Bs.Loading('hide');
                }

                if (CStr2(htmlTickets) == '') {
                    AjaxSend({
                        ExactUrl: '/CRMDashboard/Tickets?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            htmlTickets = vData;
                            $(vData).insertAfter('#pageHome');
                            Tickets.GetAdditionalDataForTicket();
                            Go();
                        }
                    });
                } else {
                    Go()
                }
            })

            function UserDeviceDetails() {
                if (OfflineMode) {
                    Web.SetOrgID(sessionStorage.OrgID);
                    Web.HandleOrganization();
                    try {
                        var pMobizAppVersion = Business.LibraryDevice.GetMobizAppVersion();
                        Transfers.WriteFile('MobizAppVersion.txt', null, pMobizAppVersion);
                        Android.SetMobizAppVersion(pMobizAppVersion);
                    } catch(e) {
                        console.error('jsDQ5FVHNY8G! ' + e);
                    }
                    var vDataUserDetails = UserDetails.GetUserDetails(sessionStorage.OrgID)[0];
                    var device = vDataUserDetails.Device.Rows;
                    var fowst = vDataUserDetails.Fowst.Rows;
                    var userrole = vDataUserDetails.Role.Rows;

                    $('#fowstdevice').empty();

                    for (var i = 0; i < device.length; i++) {
                        var pBatchDate = '', pSalesMenu_offline;
                        if (Web.HandleOrganization(true)) {
                            if (Business.SystemDefaults.GetEnforceZRead()) {
                                sessionStorage.FowstID = device[i].FowstID;
                                if (pSalesMenu_offline == null)
                                    pSalesMenu_offline = SalesMenu_offline();
                                pBatchDate = pSalesMenu_offline.GetZReadDay();
                                sessionStorage.removeItem('FowstID');
                                pBatchDatePrefix = 'Z ';
                            } else {
                                pBatchDate = new Date();
                                pBatchDatePrefix = 'C ';
                            }
                            pBatchDate = RemoveTime(pBatchDate);
                            pBatchDate = pBatchDatePrefix + $.format.date(pBatchDate, 'MMM d, yyyy');
                        }

                        $('#fowstdevice').append($('<tr>')
                                .append($('<td class="pr-3">').text(device[i].Name))
                                .append($('<td class="pr-3">').text(pBatchDate))
                            )
                    }

                    $('#lblUserRole').text(userrole.map(function (elem) {
                        return elem.Role;
                    }).join(', '))

                    if (sessionStorage.OrgID != '7c794971-1cd2-4ad4-922f-36a93edde09d') {
                        if (IsSetDailyBeginningBalance()) {
                            SaveDailyBeginningBalance();
                        }
                    }
                }
            }

            function IsSetDailyBeginningBalance() {
                var pSql;

                pSql = "Select DefaultValue from private.SystemDefaults where Description = 'Save Daily Beginning Balance'";

                return CBool(Sql.ExecuteScalar(pSql));
            }

            function SaveDailyBeginningBalance() {
                var pSql = "Select BeginningBalanceDate from private.TransDailyBeginningBalance order by BeginningBalanceDate desc limit 1 ";
                var pDT;

                pDT = Sql.Execute(pSql);

                if (pDT.Rows.length > 0) {
                    if ($.format.date(new Date(pDT.Rows[0].BeginningBalanceDate), 'MM/dd/yyyy') != $.format.date(new Date(), 'MM/dd/yyyy')) {

                        //setTimeout(function () { StartProcessBeginningBalance($.format.date(new Date(pDT.Rows[0].BeginningBalanceDate))), 3000 });
                        StartProcessBeginningBalance($.format.date(new Date(pDT.Rows[0].BeginningBalanceDate)));
                    }
                } else {

                    //$('#popMessage').html(
                    //'Inventory Balance Caching in Progress <br> <br> ' +
                    //'Please wait while we cache your device\'s beginning balances for today. <br> <br> ' +
                    //'This is a 1-time daily caching process that happens on your first sign in of the day. This might take 3-5 minutes depending on your device\'s performance. <br> <br> ' +
                    //'Caching makes inventory balance queries fast and light. You can disable this caching function by calling your system administrator. <br> <br> ' +
                    //'Thank you for waiting.'
                    //);
                    //$("#dvpopup").popup("open");

                    //setTimeout(function () { StartProcessBeginningBalance(''), 3000 });
                    StartProcessBeginningBalance('');
                }
            }

            function StartProcessBeginningBalance(LastBalanceDate) {
                var pSql;
                var pDTItems;
                var pDTEndingBalance;
                var pDTFowst;

                pSql = 'Select distinct FowstID from private.TransInventoryLedger'
                pDTFowst = Sql.Execute(pSql);

                pSql = "SELECT distinct ProductID, FOWSTID, " +
                        "(select balance from private.TransInventoryLedger TransInventoryLedger1 " +
                            "where private.TransInventoryLedger.ProductID = TransInventoryLedger1.ProductID " +
                            "and private.TransInventoryLedger.FOWSTID = TransInventoryLedger1.FOWSTID " +
                            "order by Lastupdated desc  " +
                            "limit 1) Balance " +
                            "from private.TransInventoryLedger  " +
                            "where BinID = '10d4672a-0358-4e2d-9471-2ee42e464300'  " +
                            "and productid " +
                            "order by balance desc"
                pDTItems = Sql.Execute(pSql);


                var pDTTotals;

                for (var h = 0; h <= pDTFowst.Rows.length - 1; h++) {
                    for (var i = 0; i <= pDTItems.Rows.length - 1; i++) {

                        if (LastBalanceDate != '') {
                            pSql = 'Select IFNULL(SUM(QuantityGood),0) ReceivingCount, ' +
                                'IFNULL((Select SUM(QuantityGood) from private.TransReceivingStock inner join TransReceiving on TransReceivingStock.ReceivingID = TransReceiving.ID  ' +
                                'where TransReceiving.IsReceiving=0 AND ProductID = ' + ReadyString(pDTItems.Rows[i].ProductID) + ' AND date(LastUpdated) = ' + ReadyString($.format.date(new Date(LastBalanceDate), 'yyyy-MM-dd')) + ' AND ReceivingPlaceID = ' + ReadyString(pDTFowst.Rows[h].FowstID) + '),0) TransferOutCount , ' +
                                'IFNULL((Select SUM(Quantity) from private.TransInvoiceItems inner join TransInvoice on TransInvoiceItems.InvoiceID = TransInvoice.ID where ProductID = ' + ReadyString(pDTItems.Rows[i].ProductID) + ' AND date(InvoiceDate) = ' + ReadyString($.format.date(new Date(LastBalanceDate), 'yyyy-MM-dd')) + ' AND FOWSTID =' + ReadyString(pDTFowst.Rows[h].FowstID) + '),0) SalesCount, ' +
                                'IFNULL((Select SUM(Quantity) from private.TransInventoryProductCountStock inner join TransInventoryProductCount on TransInventoryProductCount.ID =TransInventoryProductCountStock.InventoryProductCountID where ProductID = ' + ReadyString(pDTItems.Rows[i].ProductID) + ' AND date(InventoryCountDate) = ' + ReadyString($.format.date(new Date(LastBalanceDate), 'yyyy-MM-dd')) + ' AND FOWSTID =' + ReadyString(pDTFowst.Rows[h].FowstID) + '),0) InventoryCountVariance , ' +
                                'IFNULL((Select SUM(Qty) from private.TransInventoryLedger where TransactionType = \'Sales Returns\' AND ProductID = ' + ReadyString(pDTItems.Rows[i].ProductID) + ' AND BinID = \'10d4672a-0358-4e2d-9471-2ee42e464300\' AND date(LastUpdated) = ' + ReadyString($.format.date(new Date(LastBalanceDate), 'yyyy-MM-dd')) + ' AND FOWSTID =' + ReadyString(pDTFowst.Rows[h].FowstID) + '),0) SalesReturnsCount ' +
                                ' from private.TransReceivingStock inner join TransReceiving on TransReceivingStock.ReceivingID = TransReceiving.ID  ' +
                                'where TransReceiving.IsReceiving=1 AND ProductID = ' + ReadyString(pDTItems.Rows[i].ProductID) + ' AND date(LastUpdated) = ' + ReadyString($.format.date(new Date(LastBalanceDate), 'yyyy-MM-dd')) +
                                ' AND ReceivingPlaceID = ' + ReadyString(pDTFowst.Rows[h].FowstID)

                            pDTEndingBalance = Sql.Execute(pSql);

                            pSql = 'Update TransDailyBeginningBalance set EndBalance = ' + pDTItems.Rows[i].Balance +
                                                                        ', Sales = ' + (pDTEndingBalance.Rows[0].SalesCount || 0) +
                                                                        ', Receiving = ' + (pDTEndingBalance.Rows[0].ReceivingCount || 0) +
                                                                        ', SalesReturns = ' + (pDTEndingBalance.Rows[0].SalesReturnsCount || 0) +
                                                                        ', Transfers = ' + (pDTEndingBalance.Rows[0].TransferOutCount || 0) +
                                                                        ', DateImported = null ' +
                                                                        ', InventoryCountVariance = ' + (pDTEndingBalance.Rows[0].InventoryCountVariance || 0) +
                                                                        ' WHERE date(BeginningBalanceDate) = ' + ReadyString($.format.date(new Date(LastBalanceDate), 'yyyy-MM-dd')) +
                                                                        ' AND ProductID = ' + ReadyString(pDTItems.Rows[i].ProductID) +
                                                                        ' AND FOWSTID = ' + ReadyString(pDTFowst.Rows[h].FowstID)
                            Sql.Execute(pSql);

                        }

                        pSql = 'Insert into TransDailyBeginningBalance(ID, BeginningBalanceDate, FOWSTID, ProductID, BegBalance, PeopleID) values(' +
                            ReadyString(NewID()) + ',' +
                            proj.SqliteNow + ',' +
                            ReadyString(pDTFowst.Rows[h].FowstID) + ',' +
                            ReadyString(pDTItems.Rows[i].ProductID) + ',' +
                            pDTItems.Rows[i].Balance + ',' +
                            ReadyString(Web.PeopleUserID()) + ')'

                        Sql.Execute(pSql);
                    }
                }
            }

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
                    sessionStorage["PeopleID"] = (vData.Rows[0].ID || '');
                    try {
                        Transfers.WriteFile('LastUser.txt', null, sessionStorage.CurrentUserFullName);
                    } catch (e) {
                        console.error('jsSA3JM3VGBM! ' + e);
                    }
                    $('#tabHome').addClass('ui-btn-active');

                    //Edited By: Francis H
                    //Date: 06/18/14

                    // modified by Arvin mar 23, 2015
                    //ShowPeopleMsgList(); // can be done offline

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
                                sessionStorage["PeopleID"] = (vData.PeopleNameData.Rows[0].ID || '');
                                $('#tabHome').addClass('ui-btn-active');

                                //Get Employment Invitation
                                GetEmploymentInvitation();

                                //Get People Organization
                                ShowPeopleOrgList();

                            }
                        }
                    });
                };
            };

            function GetEmploymentInvitation() {
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
                        var vRows = vDataMsg.PeopleData.Rows;
                        $('#divemploymentinvitation').empty();
                        for (var i = 0; i < vRows.length; i++) {
                            if (vRows[i].MessageType == '1') {
                                $('#divemploymentinvitation')
                                    .append($('<div class="alert alert-success mb-3" role="alert" style="font-size:20px;">')
                                        .data('data', vRows[i])
                                        .append($('<span class="bi-stars" style="font-size:30px;">'))
                                        .append($('<span>').text('Employment invitation from ' + vRows[i].CompanyName))
                                        .append($('<span  style="font-size:12px;">').text(' (Click to view more details)'))
                                        .click(function () {
                                            var data = $(this).data('data');
                                            $('#txtPeopleMsgT1ID').val(data.ID);
                                            $('#lblPeopleMsgT1From').text(data.SenderName);
                                            $('#lblPeopleMsgT1DateTime').text($.format.date(new Date(data.MessageDateTime), 'MMMM dd, yyyy hh:mm a'));
                                            $('#lblPeopleMsgT1').text('You have received an invitation to confirm your employment with ' + data.CompanyName);
                                            $('#popupEmploymentInvitation').modal('show');
                                        })
                                    )
                            }
                        }
                    },
                    error: function () {

                    }
                });
            }

            function ShowPeopleMsgList() {
                var intLimitMsg = 2;
                var pData;

                if (window.IsMobile)
                    pData = {
                        PeopleUserID: Web.PeopleUserID()
                    };
                if (window.IsOfflineMode)
                    return;
                AjaxSend({
                    url: "People/getPeopleMessageList",
                    NoSplash: true,
                    data: pData,
                    success: function (vDataMsg) {
                        if (OfflineMode)
                            PeopleMenu_offline().SaveOfflineMessages(vDataMsg);
                        ShowPeopleMsgT2(vDataMsg);
                    },
                    error: function () {
                        try {
                            var pData = PeopleMenu_offline().LoadOfflineMessages();
                            ShowPeopleMsgT2(pData);
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
                        for (intCntr = 0; intCntr < (vDataMsg.PeopleData.Rows.length > intLimitMsg ? intLimitMsg : vDataMsg.PeopleData.Rows.length) ; intCntr++) {

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
                    //$('#popMessage').html(
                    //    'Inventory Balance Caching in Progress <br> <br> ' +
                    //    'Please wait while we cache your device\'s beginning balances for today. <br> <br> ' +
                    //    'This is a 1-time daily caching process that happens on your first sign in of the day. This might take 3-5 minutes depending on your device\'s performance. <br> <br> ' +
                    //    'Caching makes inventory balance queries fast and light. You can disable this caching function by calling your system administrator. <br> <br> ' +
                    //    'Thank you for waiting.'
                    //    );
                    //$("#dvpopup").popup("open");

                    //setTimeout(function () {
                    vData = PeopleMenu_offline().getPeopleOrganizationList();
                    DisplayPeopleMenu(vData, true); //Load First Org with DBase
                    //DisplayPeopleMenu(vData, false);
                    //}, 300);

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
                var pPackage;

                var pModuleButtons = $('#butWebPos, #btnStockReceiving, #btnStockTransfers, #btnErpInventoryHandling, #btnShelfBinLabelling, #btnDashboardAndReports, #btnSystemAdmin');
                if (window.IsMobile) {
                    pPackage = Android.PackageName();
                    if (pPackage == 'mobiz.mobizFC')
                        pPackage = 'fc';
                    else if (pPackage == 'mobiz.mobizSK')
                        pPackage = 'sk';
                    else
                        pPackage = 'mobiz';
                    //Android.AddLog('Package = ' + pPackage + '. js6RRHCB2PVQ!');
                    pModuleButtons.find('.module-buttons').remove();
                } else {
                    pPackage = 'mobiz';
                    pModuleButtons.find('img, br').remove();
                    pModuleButtons.each(function () {
                        var dis = $(this);
                        if (dis.attr('id') != 'btnDashboardAndReports' && dis.attr('id') != 'btnSystemAdmin') {
                            dis.remove();
                        }
                    })
                }

                if (vData.Rows.length > 0) {
                    var firstdb, ctrHasDB = 0, isCntReports = false;
                    for (var i = 0; i < vData.Rows.length; i++) {
                        if (OfflineMode) {
                            if (!IsHasDBase(vData.Rows[i].OrganizationID)) {
                                continue;
                            }
                        }

                        $('#selPeopleOrg').append($('<option value="' + vData.Rows[i].OrganizationID + '">').data('data', vData.Rows[i]).text(vData.Rows[i].CompanyName));

                        if (CStr2(sessionStorage.OrgID) == '') {
                            sessionStorage.OrgID = vData.Rows[i].OrganizationID;
                            sessionStorage.OrgName = vData.Rows[i].CompanyName;
                        }

                        if (vData.Rows[i].OrganizationID == sessionStorage.OrgID) {
                            firstdb = vData.Rows[i];
                        }

                        ctrHasDB += 1;
                    }
                    $('#selPeopleOrg').val(sessionStorage.OrgID);
                    //sessionStorage["OrgName"] = firstdb.CompanyName;
                    //sessionStorage["OrgID"] = firstdb.OrganizationID;
                    UserDeviceDetails();

                    if (firstdb.DbInfo != null) {
                        pDate = ParseDate(firstdb.DbInfo.DateStarted);
                        if (pDate != null) {
                            mDatabaseCreationStart = FormatDate(pDate, { Format: 'yyyy/MM/dd HH:mm:ss' });
                            pDate = $.format.date(pDate, proj.DateFormat);
                            $('#lblDBStart').text(pDate);
                        }

                        pDate = ParseDate(firstdb.DbInfo.DateEnded);
                        if (pDate != null) {
                            pDate = $.format.date(pDate, proj.DateFormat);
                            $('#lblDBEnd').text(pDate);
                        }
                    }

                    if (CStr2(firstdb.LastSync).trim() != '') {
                        pDate = ParseDate(firstdb.LastSync);
                        pDate = $.format.date(pDate, proj.DateFormat);
                        $('#lblDBSync').text(pDate);
                    } else
                        $('#lblDBSync').text("not set");

                    if (ctrHasDB == 1) {
                        $('#divCompany').empty().append($('<h4 class="m-0">').text(firstdb.CompanyName));
                        sessionStorage.OrgName = firstdb.CompanyName;
                    }

                    if (pPackage == 'sk') {
                        if (OfflineMode) {
                            $('#btnPOS, #btnPriceChecker, #btnSalesReturns, #btnCashHandling, #btnDashboardAndReports').show();
                        }
                    } else if (pPackage == 'mobiz') {
                        if (OfflineMode) {
                            $('#btnPOS, #btnPriceChecker, #btnPO, #btnSalesReturns, #btnInventoryCount, #btnExpenseTracking, #btnProductMaker, #btnCashHandling').show();
                        }

                        if (CBool(firstdb.IsFranchisee)) {
                            $('#btnFranchiseeUpdate').show();
                        } else {
                            if (ctrHasDB == 1) {
                                $('#btnFranchiseeUpdate').remove();
                            }
                        }

                        $('#butWebPos, #btnStockReceiving, #btnStockTransfers, #btnShelfBinLabelling, #btnRequestApprovals, #btnDashboardAndReports, #btnErpInventoryHandling, #btnIssueTickets, #btnPrivateMessages').show();
                        if (window.IsMobile) 
                            $('#butWebPos, #btnErpInventoryHandling, #btnIssueTickets, #btnPrivateMessages').hide();
                        else {
                            $('#btnRequestApprovals').hide();
                            //$('#butWebPos a').attr('href', '/SalesMenu/WebPOSNew?OrgID=' + sessionStorage.OrgID + '&IsMobile=0');
                            $('#butWebPos').click(function () {
                                require(['/Scripts/sources/ERP/OrderProcessing/work/model.js?v=' + assetversion], function() {
                                    var pModel = new ERP.OrderProcessing.work.model();
                                    pModel.HandleWebPosDeviceFowst({
                                        Success: function () {
                                            sessionStorage.IsWebPos = true;
                                            location.href = '/SalesMenu/WebPOSNew?OrgID=' + sessionStorage.OrgID + '&IsMobile=0';
                                        },
                                        Fail: function(vMsg) {
                                            NewMessageBox(vMsg, 'Unauthorized Web POS Device');
                                        }
                                    });
                                });
                            });
                        }                            
                    } else if (pPackage == 'fc') {
                        if (OfflineMode) {
                            $('#btnInventoryCount').show();
                        }

                        $('#btnPickProducts, #btnPriceChecker, #btnWarehouseReleasingTracker, #btnInvoicePickedItems, #btnStockReceiving, #btnStockTransfers, #btnDashboardAndReports').show();
                    }

                    if (OfflineMode) {
                        $('#devicedetails').show();
                        $('#btnTroubleshooting').show();
                    } else {
                        $('#btnSystemAdmin').show();
                    }

                    if (IsMobile) {
                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModulePOS') == 'False') {
                            $('#btnPOS').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModulePriceChecker') == 'False') {
                            $('#btnPriceChecker').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModulePO') == 'False') {
                            $('#btnPO').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleSalesReturn') == 'False') {
                            $('#btnSalesReturns').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleStockReceiving') == 'False') {
                            $('#btnStockReceiving').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleStockTransfer') == 'False') {
                            $('#btnStockTransfers').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleInventoryCount') == 'False') {
                            $('#btnInventoryCount').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleRouteExpenseTracking') == 'False') {
                            $('#btnExpenseTracking').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleProductMaker') == 'False') {
                            $('#btnProductMaker').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleShelfBinLabelling') == 'False') {
                            $('#btnShelfBinLabelling').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleRequestApprovals') == 'False') {
                            $('#btnRequestApprovals').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleCashHandling') == 'False') {
                            $('#btnCashHandling').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleDashboard') == 'False') {
                            $('#btnDashboardAndReports').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleReportZRead') == 'False') {
                            $('#popTroubleshootingToolsReprintZRead').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleReconstructSalesData') == 'False') {
                            $('#popTroubleshootingToolsReconstructSalesData').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleLazyUpdate') == 'False') {
                            $('#popTroubleshootingToolsLazyUpdate').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleRestoreSystem') == 'False') {
                            $('#popTroubleshootingToolsRestoreDatabase').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleCustomerDisplay') == 'False') {
                            $('#lnkSecondDisplaySetup').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleOrderTakerSetup') == 'False') {
                            $('#lnkOrderTakerSetup').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleSMSSetup') == 'False') {
                            $('#lnkSendingTransactionBySMS').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleAdjustZReadDate') == 'False') {
                            $('#lnkZRead').remove();
                        }

                        if (Business.SystemDefaults.GetSystemDefault('ShowMCRModuleUpdateCacheDB') == 'False') {
                            $('#popTroubleshootingToolsUpdateCacheDatabase').remove();
                        }

                        if (CBool(Business.SystemDefaults.GetSystemDefault('ShowAnnouncements'))) {
                            GetAnnouncements();
                        }
                    }
                    
                    $('#divCompany').show();
                    $('#pageHome').pageshow();
                    $('#divmcrmenu .col:hidden').remove();
                } else {
                    $('#divnotamember').pageshow();
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

                // bookmark: 2015_12_28__16_01_57_6555: new database naming convention
                var pFilename;
                var pIsExist;
                var pPublic;

                pFilename = sessionStorage.OrgID;
                if (window.NewDatabaseNaming) {
                    pFilename += '.private.db';
                    pPublic = sessionStorage.OrgID + '.public.db';
                    pIsExist = Sql.IsExist(pPublic)
                        && Sql.IsExist(pFilename);
                } else {
                    pIsExist = Sql.IsExist(sessionStorage.OrgID);
                    pPublic = 'Mobiz';
                }

                if (pIsExist) {
                    Sql.OpenDatabase(pFilename);
                    //check if db has record

                    try {
                        var pSql = 'select * from librarydevice limit 1';

                        var pDT = Sql.Execute(pSql);

                        if (pDT.Rows.length == 0) {
                            return RestoreBackupDatabase();
                        }
                        else {
                            Sql.OpenDatabase(pPublic);
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

                        $('#popupEmploymentInvitation').modal('hide');
                        $('#divmcrloading').pageshow();
                        GetEmploymentInvitation();
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

                        //$.mobile.changePage('#pagePeopleMsgType2List');

                        pLV.html('');

                        for (intCntr = 0; intCntr < vDataMsgList.PeopleData.Rows.length; intCntr++) {
                            var data = vDataMsgList.PeopleData.Rows[intCntr];
                            if (data.MessageType == '2') {
                                pItem = $('<div class="accordion mb-3">')
                                        .append($('<div class="card">')
                                            .append($('<div class="card-header p-0">')
                                                .append($('<button data-role="none" class="btn btn-default bg-white btn-block text-left p-3 rounded-0" data-target="#acc-' + data.ID + '" data-toggle="collapse" aria-expanded="false">')
                                                    .text(data.Subject)
                                                )
                                            ).append($('<div id="acc-' + data.ID + '" class="collapse">')
                                                .append($('<div class="card-body">')
                                                    .append($('<div class="mb-1">').append('From: <b>' + data.SenderName + '</b>'))
                                                    .append($('<div class="mb-1">').append('Date/Time: <b>' + $.format.date(new Date(data.MessageDateTime), 'MMM dd, yyyy hh:mm a') + '</b>'))
                                                    .append($('<div class="mb-3">').append('Category: <b>' + CStr2(data.Category) + '</b>'))
                                                    .append($('<div>').append(CStr2(data.Message)))
                                                )
                                            )
                                        )

                                pLV.append(pItem);
                            }
                        }

                        $('#divMessageContainer').pageshow();
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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
                                    {
                                        Text: 'Ok'
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

            sessionStorage.removeItem('CartOrderID');
        });
    }); // require PeopleMenu_offline.js
});